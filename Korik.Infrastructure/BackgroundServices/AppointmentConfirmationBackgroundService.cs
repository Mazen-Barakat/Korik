using Korik.Application;
using Korik.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Korik.Infrastructure
{
    public class AppointmentConfirmationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AppointmentConfirmationBackgroundService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1); // Check every minute

        public AppointmentConfirmationBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<AppointmentConfirmationBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("?? Appointment Confirmation Background Service started at {Time}", DateTime.UtcNow);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessAppointments(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "? Error occurred while processing appointments");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("?? Appointment Confirmation Background Service stopped at {Time}", DateTime.UtcNow);
        }

        private async Task ProcessAppointments(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var context = scope.ServiceProvider.GetRequiredService<Korik>();

            var now = DateTime.UtcNow;

            // Get all Confirmed bookings where appointment time has arrived
            // Use AsNoTracking for the initial query since we'll re-fetch for update
            var bookingIds = await context.Bookings
                .Where(b => b.Status == BookingStatus.Confirmed
                            && b.AppointmentDate <= now
                            && b.ConfirmationSentAt == null)
                .Select(b => b.Id)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("?? Found {Count} bookings needing confirmation at {Time}", 
                bookingIds.Count, now);

            foreach (var bookingId in bookingIds)
            {
                try
                {
                    // Create a fresh scope for each booking to avoid DbContext tracking issues
                    using var bookingScope = _serviceProvider.CreateScope();
                    var bookingContext = bookingScope.ServiceProvider.GetRequiredService<Korik>();
                    var bookingNotificationService = bookingScope.ServiceProvider.GetRequiredService<INotificationService>();

                    // Fetch the booking fresh with tracking enabled
                    var booking = await bookingContext.Bookings
                        .Include(b => b.Car)
                            .ThenInclude(c => c.CarOwnerProfile)
                        .Include(b => b.WorkShopProfile)
                        .FirstOrDefaultAsync(b => b.Id == bookingId, cancellationToken);

                    if (booking == null)
                    {
                        _logger.LogWarning("?? Booking ID {BookingId} not found", bookingId);
                        continue;
                    }

                    // Double-check conditions (in case another process updated it)
                    if (booking.Status != BookingStatus.Confirmed || booking.ConfirmationSentAt != null)
                    {
                        _logger.LogInformation("?? Booking ID {BookingId} already processed or status changed", bookingId);
                        continue;
                    }

                    _logger.LogInformation("?? Processing confirmation for Booking ID: {BookingId}", booking.Id);
                    
                    // First, update the booking with confirmation times
                    var confirmationSentTime = DateTime.UtcNow;
                    var confirmationDeadline = confirmationSentTime.AddMinutes(15);
                    
                    booking.ConfirmationSentAt = confirmationSentTime;
                    booking.ConfirmationDeadline = confirmationDeadline;
                    
                    // Save the booking update FIRST before sending notifications
                    bookingContext.Entry(booking).State = EntityState.Modified;
                    
                    _logger.LogInformation("?? Updating Booking ID: {BookingId} - ConfirmationSentAt: {SentAt}, ConfirmationDeadline: {Deadline}", 
                        booking.Id, confirmationSentTime, confirmationDeadline);
                    
                    var savedChanges = await bookingContext.SaveChangesAsync(cancellationToken);
                    
                    if (savedChanges > 0)
                    {
                        _logger.LogInformation("?? Booking ID {BookingId} updated successfully. Rows affected: {RowsAffected}", 
                            booking.Id, savedChanges);
                        
                        // NOW send notifications (after booking is saved)
                        await SendConfirmationNotificationsSimultaneously(booking, bookingNotificationService, confirmationDeadline);
                        
                        _logger.LogInformation("? Confirmation notifications sent for Booking ID: {BookingId}", booking.Id);
                    }
                    else
                    {
                        _logger.LogWarning("?? SaveChangesAsync returned 0 rows affected for Booking ID: {BookingId}", booking.Id);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "? Failed to process Booking ID: {BookingId}", bookingId);
                }
            }

            // Handle expired confirmation deadlines
            await ProcessExpiredConfirmations(cancellationToken);
        }

        private async Task SendConfirmationNotificationsSimultaneously(Booking booking, INotificationService notificationService, DateTime confirmationDeadline)
        {
            var carOwnerUserId = booking.Car.CarOwnerProfile.ApplicationUserId;
            var workshopOwnerUserId = booking.WorkShopProfile.ApplicationUserId;
            var workshopName = booking.WorkShopProfile.Name;
            var carOwnerName = $"{booking.Car.CarOwnerProfile.FirstName} {booking.Car.CarOwnerProfile.LastName}";

            _logger.LogInformation("?? Sending notifications to BOTH parties simultaneously for Booking {BookingId}", booking.Id);
            _logger.LogInformation("?? Confirmation Deadline: {Deadline}", confirmationDeadline);

            // Send to BOTH parties simultaneously using Task.WhenAll
            // Pass the confirmationDeadline so the notification includes the correct time
            var carOwnerTask = notificationService.SendNotificationAsync(
                senderId: workshopOwnerUserId,
                receiverId: carOwnerUserId,
                message: $"Your appointment at {workshopName} is now. Please confirm your arrival.",
                type: NotificationType.AppointmentConfirmationRequest,
                bookingId: booking.Id,
                title: "Confirm Your Appointment",
                priority: "high",
                confirmationDeadline: confirmationDeadline
            );

            var workshopTask = notificationService.SendNotificationAsync(
                senderId: carOwnerUserId,
                receiverId: workshopOwnerUserId,
                message: $"Appointment with {carOwnerName} is scheduled now. Please confirm you're ready.",
                type: NotificationType.AppointmentConfirmationRequest,
                bookingId: booking.Id,
                title: "Incoming Appointment",
                priority: "high",
                confirmationDeadline: confirmationDeadline
            );

            var results = await Task.WhenAll(carOwnerTask, workshopTask);

            if (results[0].Success)
                _logger.LogInformation("? Notification sent to Car Owner successfully");
            else
                _logger.LogError("? Failed to send notification to Car Owner: {Message}", results[0].Message);

            if (results[1].Success)
                _logger.LogInformation("? Notification sent to Workshop Owner successfully");
            else
                _logger.LogError("? Failed to send notification to Workshop Owner: {Message}", results[1].Message);
        }

        private async Task ProcessExpiredConfirmations(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<Korik>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            
            var now = DateTime.UtcNow;

            var expiredBookingIds = await context.Bookings
                .Where(b => b.Status == BookingStatus.Confirmed
                            && b.ConfirmationDeadline.HasValue
                            && b.ConfirmationDeadline.Value < now
                            && (!b.CarOwnerConfirmed.HasValue || !b.WorkshopOwnerConfirmed.HasValue))
                .Select(b => b.Id)
                .ToListAsync(cancellationToken);

            if (expiredBookingIds.Any())
            {
                _logger.LogInformation("? Found {Count} bookings with expired confirmation deadlines", 
                    expiredBookingIds.Count);
            }

            foreach (var bookingId in expiredBookingIds)
            {
                try
                {
                    using var bookingScope = _serviceProvider.CreateScope();
                    var bookingContext = bookingScope.ServiceProvider.GetRequiredService<Korik>();
                    var bookingNotificationService = bookingScope.ServiceProvider.GetRequiredService<INotificationService>();

                    var booking = await bookingContext.Bookings
                        .Include(b => b.Car)
                            .ThenInclude(c => c.CarOwnerProfile)
                        .Include(b => b.WorkShopProfile)
                        .FirstOrDefaultAsync(b => b.Id == bookingId, cancellationToken);

                    if (booking == null) continue;

                    // Double-check conditions
                    if (booking.Status != BookingStatus.Confirmed) continue;

                    var carOwnerUserId = booking.Car.CarOwnerProfile.ApplicationUserId;
                    var workshopOwnerUserId = booking.WorkShopProfile.ApplicationUserId;

                    // Mark as NoShow
                    booking.Status = BookingStatus.NoShow;
                    bookingContext.Entry(booking).State = EntityState.Modified;
                    
                    var savedChanges = await bookingContext.SaveChangesAsync(cancellationToken);

                    // Send real-time updates to dismiss dialogs
                    await bookingNotificationService.SendConfirmationStatusUpdateAsync(
                        carOwnerUserId, booking.Id,
                        booking.CarOwnerConfirmed, booking.WorkshopOwnerConfirmed,
                        BookingStatus.NoShow.ToString(), shouldDismissDialog: true);

                    await bookingNotificationService.SendConfirmationStatusUpdateAsync(
                        workshopOwnerUserId, booking.Id,
                        booking.CarOwnerConfirmed, booking.WorkshopOwnerConfirmed,
                        BookingStatus.NoShow.ToString(), shouldDismissDialog: true);

                    // Send notifications
                    await bookingNotificationService.SendNotificationAsync(
                        workshopOwnerUserId, carOwnerUserId,
                        "Appointment marked as No Show due to confirmation timeout.",
                        NotificationType.BookingCancelled, booking.Id,
                        "Appointment Expired", "normal");

                    await bookingNotificationService.SendNotificationAsync(
                        carOwnerUserId, workshopOwnerUserId,
                        "Appointment marked as No Show due to confirmation timeout.",
                        NotificationType.BookingCancelled, booking.Id,
                        "Appointment Expired", "normal");

                    _logger.LogWarning("?? Booking ID {BookingId} marked as NoShow. Rows affected: {RowsAffected}", 
                        booking.Id, savedChanges);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "? Failed to process expired Booking ID: {BookingId}", bookingId);
                }
            }
        }
    }
}
