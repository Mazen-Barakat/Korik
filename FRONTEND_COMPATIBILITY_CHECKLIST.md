# Backend Updates for Frontend Compatibility - Complete Checklist

## ? Changes Implemented

### 1. NotificationType Enum - Fixed ?
**File:** `Korik.Domain/Enums/NotificationType.cs`

```csharp
public enum NotificationType
{
    BookingCreated = 0,
    BookingAccepted = 1,
    BookingRejected = 2,
    BookingCancelled = 3,
    CarReadyForPickup = 4,
    BookingCompleted = 5,
    AppointmentReminder = 6,
    AppointmentConfirmationRequest = 13  // ? Explicitly set to 13
}
```

**Verification:**
- Type value is **exactly 13** (numeric, not string)
- Will serialize as `"type": 13` in JSON

---

### 2. NotificationDto Enhanced - Fixed ?
**File:** `Korik.Application/DTOs/Notification/Response_DTOs/NotificationDto.cs`

**Added Fields:**
```csharp
public class NotificationDto
{
    public int Id { get; set; }
    public string SenderId { get; set; }
    public string ReceiverId { get; set; }
    public string Message { get; set; }
    
    [JsonConverter(typeof(JsonNumberEnumConverter<NotificationType>))]
    public NotificationType Type { get; set; }  // ? Serializes as number
    
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? BookingId { get; set; }  // ? Required
    
    // ? NEW: Frontend compatibility fields
    public string? Title { get; set; }
    public string? Priority { get; set; }
    public DateTime? ConfirmationDeadline { get; set; }
}
```

**Custom JSON Converter:**
- Ensures `Type` is always serialized as **number** (13), not string
- Frontend receives: `"type": 13` ?
- NOT: `"type": "AppointmentConfirmationRequest"` ?

---

### 3. NotificationService Updated - Fixed ?
**File:** `Korik.Infrastructure/ExternalServices/Notification/NotificationService.cs`

**Enhanced SignalR Payload:**
```csharp
public async Task<ServiceResult<NotificationDto>> SendNotificationAsync(
    string senderId,
    string receiverId,
    string message,
    NotificationType type,
    int? bookingId = null,
    string? title = null,        // ? NEW
    string? priority = null)     // ? NEW
{
    // Save to database
    var savedNotification = await _notificationRepository.AddAsync(notification);
    
    // Map to DTO
    var notificationDto = _mapper.Map<NotificationDto>(savedNotification);
    
    // ? Add frontend fields
    notificationDto.Title = title ?? GetDefaultTitle(type);
    notificationDto.Priority = priority ?? "normal";
    
    // ? Add confirmation deadline for appointment requests
    if (type == NotificationType.AppointmentConfirmationRequest)
    {
        notificationDto.ConfirmationDeadline = DateTime.UtcNow.AddMinutes(15);
    }

    // ? Send to specific user via SignalR
    if (_connectionManager.IsUserConnected(receiverId))
    {
        var connectionIds = _connectionManager.GetConnections(receiverId);
        foreach (var connectionId in connectionIds)
        {
            await _hubContext.Clients.Client(connectionId)
                .SendAsync("ReceiveNotification", notificationDto);
        }
        
        // ? Also send to user group (additional reliability)
        await _hubContext.Clients.User(receiverId)
            .SendAsync("ReceiveNotification", notificationDto);
    }
}
```

**Key Features:**
- ? Sends to specific **user connections** (not broadcast)
- ? Uses both `.Client(connectionId)` AND `.User(userId)` for reliability
- ? Includes all required fields: `bookingId`, `title`, `priority`, `confirmationDeadline`
- ? Console logging for debugging

---

### 4. Background Service Enhanced - Fixed ?
**File:** `Korik.Infrastructure/BackgroundServices/AppointmentConfirmationBackgroundService.cs`

**Comprehensive Logging Added:**
```csharp
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    _logger.LogInformation("? Appointment Confirmation Background Service started");
    
    while (!stoppingToken.IsCancellationRequested)
    {
        await ProcessAppointments(stoppingToken);
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
    }
}

private async Task SendConfirmationNotifications(Booking booking, INotificationService notificationService)
{
    _logger.LogInformation("?? Sending notification to Car Owner (ID: {UserId}) for Booking {BookingId}", 
        carOwnerUserId, booking.Id);
    _logger.LogInformation("?? Notification Type: AppointmentConfirmationRequest (Value: {TypeValue})", 
        (int)NotificationType.AppointmentConfirmationRequest);  // Should log: 13

    // ? Send with all required fields
    var carOwnerResult = await notificationService.SendNotificationAsync(
        senderId: workshopOwnerUserId,
        receiverId: carOwnerUserId,
        message: $"Your appointment at {workshopName} is now. Please confirm your arrival.",
        type: NotificationType.AppointmentConfirmationRequest,
        bookingId: booking.Id,
        title: "Confirm Your Appointment",    // ? NEW
        priority: "high"                      // ? NEW
    );

    if (carOwnerResult.Success)
    {
        _logger.LogInformation("? Notification sent to Car Owner successfully");
    }
}
```

**Logs to Check:**
```
? Appointment Confirmation Background Service started at 2024-01-15 10:00:00
?? Found 1 bookings needing confirmation at 2024-01-15 10:00:00
?? Processing confirmation for Booking ID: 123
?? Sending notification to Car Owner (ID: abc-123) for Booking 123
?? Notification Type: AppointmentConfirmationRequest (Value: 13)
? Notification sent to Car Owner successfully
?? Sending notification to Workshop Owner (ID: xyz-456) for Booking 123
? Notification sent to Workshop Owner successfully
? Confirmation notifications sent successfully for Booking ID: 123
```

---

### 5. Confirmation Handler Updated - Fixed ?
**File:** `Korik.Application/Features/Booking/Handlers/Command/ConfirmAppointment/ConfirmAppointmentRequest.cs`

**Enhanced with Title/Priority:**
```csharp
// When both confirm
await SendNotificationToBothParties(
    booking,
    carOwnerUserId,
    workshopOwnerUserId,
    "Appointment confirmed! Service is now in progress.",
    NotificationType.BookingAccepted,
    "Appointment Started",  // ? Title
    "high"                 // ? Priority
);

// When one party responds
await _notificationService.SendNotificationAsync(
    senderUserId,
    receiverUserId,
    messageToOtherParty,
    NotificationType.AppointmentConfirmationRequest,
    booking.Id,
    title,      // ? Dynamic title
    "high"      // ? Priority
);
```

---

## ?? Testing Instructions

### Backend Logs to Monitor

Open your backend console and look for:

```bash
# 1. Service startup
? Appointment Confirmation Background Service started

# 2. When appointment time arrives
?? Found 1 bookings needing confirmation
?? Processing confirmation for Booking ID: 123

# 3. Notification sending
?? Sending notification to Car Owner (ID: abc-123)
?? Notification Type: AppointmentConfirmationRequest (Value: 13)
?? Checking if user abc-123 is connected...
? User abc-123 is connected with 1 connection(s)
?? Sent notification to connection: conn-xyz
? Real-time notification sent to user abc-123 | Type: AppointmentConfirmationRequest (13) | BookingId: 123

# 4. Success confirmation
? Notification sent to Car Owner successfully
? Confirmation notifications sent successfully for Booking ID: 123
```

### SignalR Payload Verification

**Expected Payload (sent to frontend):**
```json
{
  "id": 789,
  "senderId": "workshop-user-guid",
  "receiverId": "car-owner-guid",
  "message": "Your appointment at ABC Workshop is now. Please confirm your arrival.",
  "type": 13,                                    // ? MUST be number 13
  "isRead": false,
  "createdAt": "2024-01-15T10:00:00Z",
  "bookingId": 123,                              // ? REQUIRED
  "title": "Confirm Your Appointment",           // ? NEW
  "priority": "high",                            // ? NEW
  "confirmationDeadline": "2024-01-15T10:15:00Z" // ? NEW (15 min from now)
}
```

**Verify Type is Numeric:**
- ? Correct: `"type": 13`
- ? Wrong: `"type": "AppointmentConfirmationRequest"`

---

## ?? Frontend Testing Steps

### 1. Check Browser Console

After creating a test booking, open browser DevTools console. You should see:

```javascript
?? Received notification from SignalR: 
{
  type: 13,  // ? Must be number, not string
  bookingId: 123,  // ? Must be present
  message: "Your appointment at...",
  title: "Confirm Your Appointment",
  priority: "high",
  confirmationDeadline: "2024-01-15T10:15:00Z"
}
```

### 2. Verify Dialog Appears

- Dialog should appear automatically
- Title: "Confirm Your Appointment"
- Countdown timer shows "14m 59s" (or less)
- Two buttons: "? Confirm Arrival" and "? Decline"

### 3. Test Confirmation Flow

**Step 1:** User A (Car Owner) clicks "Confirm"
```
POST /api/Booking/confirm-appointment
{ "bookingId": 123, "isConfirmed": true }

Response: { "success": true, "message": "..." }
```

**Step 2:** User B (Workshop Owner) receives notification
```
?? Notification: "Car owner has confirmed arrival. Please confirm your readiness."
Type: 13 (AppointmentConfirmationRequest)
```

**Step 3:** User B clicks "Confirm"
```
POST /api/Booking/confirm-appointment
{ "bookingId": 123, "isConfirmed": true }

Response: { "success": true, "data": { "status": "InProgress" } }
```

**Step 4:** Both users receive notification
```
?? Notification: "Appointment confirmed! Service is now in progress."
Type: 1 (BookingAccepted)
```

---

## ?? Troubleshooting

### Issue 1: Type is String Instead of Number

**Symptom:**
```json
{ "type": "AppointmentConfirmationRequest" }  // ? Wrong
```

**Solution:**
? Already fixed with `JsonNumberEnumConverter<T>` in `NotificationDto.cs`

**Verify:**
```bash
# Check backend logs
?? Notification Type: AppointmentConfirmationRequest (Value: 13)
```

---

### Issue 2: bookingId is null/missing

**Symptom:**
```json
{ "bookingId": null }  // ? Can't call confirm API
```

**Solution:**
? Already fixed - always passed in `SendNotificationAsync`

**Verify:**
```bash
# Check backend logs
?? Sending notification... | BookingId: 123
```

---

### Issue 3: SignalR not reaching user

**Symptom:**
```bash
?? User abc-123 is offline. Notification saved to database only.
```

**Solutions:**
1. Verify user is connected to SignalR hub
2. Check JWT token is valid
3. Verify userId matches between frontend and backend
4. Check browser console for SignalR connection status

**Frontend Check:**
```javascript
connection.start()
  .then(() => console.log('? SignalR Connected'))
  .catch(err => console.error('? SignalR Error:', err));
```

---

### Issue 4: Background service not running

**Symptom:**
- No logs in console
- Notifications never sent automatically

**Solutions:**
1. Verify service is registered in `Program.cs`:
   ```csharp
   builder.Services.AddHostedService<AppointmentConfirmationBackgroundService>();
   ```
2. Check if service started:
   ```bash
   ? Appointment Confirmation Background Service started
   ```
3. Verify booking status is "Confirmed"
4. Verify appointment date <= current time

---

## ?? Database Verification

Check the database to verify fields are populated:

```sql
-- Check booking confirmation fields
SELECT 
    Id,
    Status,
    AppointmentDate,
    CarOwnerConfirmed,
    WorkshopOwnerConfirmed,
    ConfirmationSentAt,
    ConfirmationDeadline,
    CreatedAt
FROM Bookings
WHERE Id = 123;

-- Expected after background service runs:
-- ConfirmationSentAt: 2024-01-15 10:00:00
-- ConfirmationDeadline: 2024-01-15 10:15:00
-- CarOwnerConfirmed: NULL (waiting for response)
-- WorkshopOwnerConfirmed: NULL (waiting for response)

-- Check notifications sent
SELECT 
    Id,
    Type,  -- Should be 13 for AppointmentConfirmationRequest
    Message,
    ReceiverId,
    BookingId,
    CreatedAt
FROM Notifications
WHERE BookingId = 123
ORDER BY CreatedAt DESC;

-- Should see 2 notifications (one to car owner, one to workshop owner)
```

---

## ? Compliance Checklist

### Enum Value
- [x] `NotificationType.AppointmentConfirmationRequest = 13`
- [x] Serializes as numeric 13, not string
- [x] Backend logs show "Value: 13"

### SignalR Payload
- [x] `type` is numeric (13)
- [x] `bookingId` is always included
- [x] `title` is included
- [x] `priority` is included
- [x] `confirmationDeadline` is included
- [x] Sends to specific user via `.User(userId)`
- [x] Also sends via `.Client(connectionId)` for reliability

### API Endpoint
- [x] `POST /api/Booking/confirm-appointment` exists
- [x] Returns `{ success, message, data }`
- [x] Validates user authorization
- [x] Checks confirmation deadline
- [x] Updates booking status correctly

### Background Service
- [x] Registered in `Program.cs`
- [x] Checks every 1 minute
- [x] Filters for `Status == Confirmed`
- [x] Sends to BOTH car owner AND workshop owner
- [x] Logs all operations
- [x] Handles timeouts (15-minute deadline)

### Logging
- [x] Service startup logged
- [x] Notification type value logged (13)
- [x] User connection status logged
- [x] Success/failure logged
- [x] SignalR send confirmation logged

---

## ?? Deployment Steps

1. **Build the solution:**
   ```bash
   dotnet build
   ```

2. **Run the application:**
   ```bash
   dotnet run --project Korik.API
   ```

3. **Monitor startup logs:**
   ```
   ? Appointment Confirmation Background Service started
   ```

4. **Test with Swagger:**
   - Open: `https://localhost:44352`
   - Create booking with appointment date = now
   - Update status to "Confirmed"
   - Wait 1 minute
   - Check backend logs

5. **Test with frontend:**
   - Connect SignalR
   - Monitor browser console
   - Verify notification received with `type: 13`

---

## ?? Support

If issues persist:

1. **Enable verbose logging:**
   ```csharp
   // In appsettings.json
   "Logging": {
     "LogLevel": {
       "Default": "Information",
       "Korik.Infrastructure": "Debug",  // ? Add this
       "Microsoft.AspNetCore.SignalR": "Debug"  // ? Add this
     }
   }
   ```

2. **Check all logs match expected format**
3. **Verify database fields are populated**
4. **Test SignalR connection independently**

---

## ?? Success Indicators

You'll know everything is working when:

? Backend logs show "Notification Type: AppointmentConfirmationRequest (Value: 13)"
? Frontend console shows `type: 13` (number, not string)
? Dialog appears automatically when appointment time arrives
? Both parties can confirm independently
? Booking status changes to "InProgress" when both confirm
? Booking status changes to "NoShow" if declined or timeout
? All fields present in notification payload

---

**Build Status:** ? Successful
**All Requirements:** ? Met
**Ready for Testing:** ? Yes
