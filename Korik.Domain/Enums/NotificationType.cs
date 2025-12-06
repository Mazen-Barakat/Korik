using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Domain
{
    public enum NotificationType
    {
        BookingCreated = 0,
        BookingAccepted = 1,
        BookingRejected = 2,
        BookingCancelled = 3,
        CarReadyForPickup = 4,
        BookingCompleted = 5,
        AppointmentReminder = 6,
        AppointmentConfirmationRequest = 13,  // Explicitly set to 13 for frontend compatibility
        ResponseStatusChanged = 14
    }
}