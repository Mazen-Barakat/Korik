# Automated Booking Confirmation System - Implementation Guide

## ?? Overview

This document describes the automated booking confirmation system that requires both car owner and workshop owner to confirm attendance before transitioning a booking to "InProgress" status.

## ?? Booking Status Flow

```
???????????    ????????????    ???????????????    ?????????????
? Pending ? ???? Confirmed? ???? InProgress  ? ???? Completed ?
???????????    ????????????    ???????????????    ?????????????
                    ?                                    
                    ?         ???????????????????????????
                    ????????????      No Show        ?
                              ???????????????????????????
                    (If either party declines or timeout)
```

## ??? Database Schema Changes

### Updated `Booking` Entity

New fields added to track confirmation status:

| Field | Type | Description |
|-------|------|-------------|
| `CarOwnerConfirmed` | `bool?` | Car owner's confirmation response |
| `WorkshopOwnerConfirmed` | `bool?` | Workshop owner's confirmation response |
| `ConfirmationSentAt` | `DateTime?` | When confirmation notifications were sent |
| `ConfirmationDeadline` | `DateTime?` | Response timeout (15 minutes default) |

### Migration Required

After deploying this update, run:

```bash
# Create migration
dotnet ef migrations add AddBookingConfirmationFields --project Korik.Infrastructure --startup-project Korik.API

# Update database
dotnet ef database update --project Korik.Infrastructure --startup-project Korik.API
```

## ?? New API Endpoints

### 1. Confirm Appointment

**Endpoint:** `POST /api/Booking/confirm-appointment`

**Authorization:** Required (Bearer Token)

**Request Body:**
```json
{
  "bookingId": 123,
  "isConfirmed": true
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Appointment confirmation updated successfully.",
  "data": {
    "id": 123,
    "status": "InProgress",
    "carOwnerConfirmed": true,
    "workshopOwnerConfirmed": true,
    "confirmationSentAt": "2024-01-15T10:00:00Z",
    "confirmationDeadline": "2024-01-15T10:15:00Z"
    // ... other booking fields
  }
}
```

**Error Responses:**
```json
// Not authorized
{
  "success": false,
  "message": "You are not authorized to confirm this appointment."
}

// Deadline passed
{
  "success": false,
  "message": "Confirmation deadline has passed."
}

// Wrong status
{
  "success": false,
  "message": "Booking is not in Confirmed status."
}
```

## ?? Notification Types

New notification types added:

```csharp
public enum NotificationType
{
    // ... existing types
    AppointmentReminder,             // General reminder
    AppointmentConfirmationRequest   // Confirmation request
}
```

## ?? Background Service

### `AppointmentConfirmationBackgroundService`

**Purpose:** Automatically monitors bookings and sends confirmation requests when appointment time arrives.

**Check Interval:** Every 1 minute

**Features:**
1. **Monitor Confirmed Bookings:** Checks for bookings where appointment time has arrived
2. **Send Dual Notifications:** Sends to both car owner and workshop owner simultaneously
3. **Track Timeout:** Automatically marks bookings as "NoShow" if deadline passes without confirmation

**Process Flow:**

```
Every 1 minute:
  ?? Find Confirmed bookings where appointment time <= now
  ?  and ConfirmationSentAt is null
  ?
  ?? For each booking:
  ?  ?? Send notification to car owner
  ?  ?? Send notification to workshop owner
  ?  ?? Set ConfirmationSentAt = now
  ?  ?? Set ConfirmationDeadline = now + 15 minutes
  ?
  ?? Find bookings with expired deadlines
     ?? Mark as NoShow if not fully confirmed
```

## ?? Confirmation Logic Matrix

| Car Owner | Workshop Owner | Result |
|-----------|----------------|--------|
| ? Confirm | ? Confirm | ? **InProgress** |
| ? Confirm | ? Decline | ? **NoShow** |
| ? Decline | ? Confirm | ? **NoShow** |
| ? Decline | ? Decline | ? **NoShow** |
| ? Pending | ? Pending | ? **NoShow** (after timeout) |
| ? Confirm | ? Pending | ? Wait for response |
| ? Pending | ? Confirm | ? Wait for response |

## ?? Authorization

- **Car Owner:** Can only confirm their own bookings
- **Workshop Owner:** Can only confirm bookings for their workshop
- **System:** Validates user identity via JWT token

## ?? Frontend Integration Guide

### 1. Display Confirmation UI

Show confirmation dialog when notification type is `AppointmentConfirmationRequest`:

```typescript
interface BookingDTO {
  id: number;
  status: string;
  carOwnerConfirmed?: boolean;
  workshopOwnerConfirmed?: boolean;
  confirmationSentAt?: string;
  confirmationDeadline?: string;
  // ... other fields
}

function shouldShowConfirmationDialog(booking: BookingDTO): boolean {
  return booking.status === 'Confirmed' 
    && booking.confirmationSentAt !== null
    && new Date(booking.confirmationDeadline!) > new Date();
}
```

### 2. Submit Confirmation

```typescript
async function confirmAppointment(bookingId: number, isConfirmed: boolean) {
  try {
    const response = await fetch('/api/Booking/confirm-appointment', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        bookingId: bookingId,
        isConfirmed: isConfirmed
      })
    });

    const result = await response.json();
    
    if (result.success) {
      console.log('Confirmation successful:', result.data);
      // Update UI based on new status
    } else {
      console.error('Confirmation failed:', result.message);
    }
  } catch (error) {
    console.error('Error confirming appointment:', error);
  }
}
```

### 3. Real-time Notification Handling

```typescript
// SignalR connection
connection.on('ReceiveNotification', (notification) => {
  if (notification.type === 'AppointmentConfirmationRequest') {
    // Show confirmation dialog
    showConfirmationDialog(notification.bookingId);
  }
});
```

### 4. Countdown Timer

Display time remaining to respond:

```typescript
function getRemainingTime(deadline: string): string {
  const now = new Date();
  const deadlineDate = new Date(deadline);
  const diff = deadlineDate.getTime() - now.getTime();
  
  if (diff <= 0) {
    return 'Expired';
  }
  
  const minutes = Math.floor(diff / 60000);
  const seconds = Math.floor((diff % 60000) / 1000);
  
  return `${minutes}m ${seconds}s`;
}
```

## ?? Configuration Options

### Timeout Duration

Default: 15 minutes

To customize, modify in `AppointmentConfirmationBackgroundService.cs`:

```csharp
// Line in SendConfirmationNotifications
booking.ConfirmationDeadline = DateTime.UtcNow.AddMinutes(15); // Change this value
```

### Check Interval

Default: 1 minute

To customize, modify in `AppointmentConfirmationBackgroundService.cs`:

```csharp
private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1); // Change this value
```

## ?? Testing Scenarios

### Scenario 1: Both Confirm
1. Create booking with status "Confirmed"
2. Set appointment date to current time
3. Wait for background service (max 1 minute)
4. Car owner confirms
5. Workshop owner confirms
6. ? Status changes to "InProgress"

### Scenario 2: Car Owner Declines
1. Follow steps 1-4 from Scenario 1
2. Car owner declines
3. ? Status changes to "NoShow" immediately

### Scenario 3: Timeout
1. Follow steps 1-3 from Scenario 1
2. Don't respond within 15 minutes
3. ? Status automatically changes to "NoShow"

### Scenario 4: Late Response
1. Follow Scenario 3
2. Try to confirm after timeout
3. ? Returns error: "Confirmation deadline has passed"

## ?? Notification Messages

### To Car Owner
```
Title: "Appointment Reminder"
Body: "Your appointment at [Workshop Name] is now. Please confirm your arrival."
Type: AppointmentConfirmationRequest
```

### To Workshop Owner
```
Title: "Incoming Appointment"
Body: "Appointment with [Owner Name] is scheduled now. Please confirm you're ready."
Type: AppointmentConfirmationRequest
```

### After Both Confirm
```
To Both Parties:
"Appointment confirmed! Service is now in progress."
Type: BookingAccepted
```

### After Decline or Timeout
```
To Both Parties:
"Appointment marked as No Show due to confirmation decline."
Type: BookingCreated
```

## ?? Error Handling

The system includes comprehensive error handling:

1. **Validator Level:** Checks booking existence and user authorization
2. **Handler Level:** Validates booking status and deadline
3. **Background Service:** Catches and logs all exceptions without crashing
4. **Database Level:** Transactional updates ensure consistency

## ?? Monitoring & Logging

The background service logs important events:

```
[Information] Appointment Confirmation Background Service started.
[Information] Confirmation notifications sent for Booking ID: 123
[Information] Booking ID 456 marked as NoShow due to timeout.
[Error] Failed to send confirmation notifications for Booking ID: 789
```

Monitor these logs to track system health and booking confirmations.

## ? Acceptance Criteria

- [x] Notifications sent automatically when appointment time arrives
- [x] Both car owner and workshop owner receive notifications
- [x] Confirm/Decline actions work correctly for both parties
- [x] Status changes to "InProgress" only when both confirm
- [x] Status changes to "NoShow" if any party declines
- [x] Response timeout handled gracefully (15 minutes)
- [x] Notification history logged in database
- [x] Real-time updates via SignalR
- [x] Authorization checks enforce user permissions
- [x] Background service runs continuously
- [x] Database fields added for tracking

## ?? Deployment Checklist

1. ? Pull latest code
2. ? Run database migration
3. ? Restart application (background service starts automatically)
4. ? Test with sample booking
5. ? Monitor logs for errors
6. ? Update frontend to handle new notification types
7. ? Deploy frontend changes

## ?? Support

For issues or questions:
- Check logs first: `[Information]`, `[Error]` messages
- Verify database migration was successful
- Ensure background service is registered in `Program.cs`
- Check SignalR connection for real-time notifications
