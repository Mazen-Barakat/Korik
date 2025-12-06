# Appointment Confirmation System - Frontend Testing Guide

## Overview

This document provides a comprehensive testing scenario for the Appointment Confirmation System. The system allows both car owners and workshop owners to confirm their arrival/readiness when an appointment time arrives.

---

## System Flow (Updated)

```
???????????????????????????????????????????????????????????????????????????????
?                        APPOINTMENT CONFIRMATION FLOW                         ?
???????????????????????????????????????????????????????????????????????????????
?                                                                              ?
?  1. Booking Created (Status: Pending)                                        ?
?           ?                                                                  ?
?  2. Workshop Accepts (Status: Confirmed)                                     ?
?           ?                                                                  ?
?  3. Appointment Time Arrives                                                 ?
?           ?                                                                  ?
?  ??????????????????????????????????????????????????????????????????????     ?
?  ?  Background Service Triggers:                                       ?     ?
?  ?  • Sets ConfirmationSentAt & ConfirmationDeadline (15 min window)  ?     ?
?  ?  • Sends SignalR "ReceiveNotification" to BOTH parties             ?     ?
?  ?  • Type: AppointmentConfirmationRequest (13)                       ?     ?
?  ??????????????????????????????????????????????????????????????????????     ?
?           ?                                                                  ?
?  4. Both parties see Confirmation Dialog                                     ?
?           ?                                                                  ?
?  ???????????????????              ???????????????????                       ?
?  ?   Car Owner     ?              ? Workshop Owner  ?                       ?
?  ? [Confirm] [Close]              ? [Confirm] [Close]                       ?
?  ???????????????????              ???????????????????                       ?
?           ?                                ?                                 ?
?  ????????????????????????????????????????????????????                       ?
?  ?  "Confirm" ? API call with isConfirmed: true     ?                       ?
?  ?  "Close"   ? Just close dialog (NO API call)     ?                       ?
?  ?              Dialog won't auto-reopen for this   ?                       ?
?  ?              booking (tracked in shownBookingIds)?                       ?
?  ????????????????????????????????????????????????????                       ?
?           ?                                                                  ?
?  5. Both Confirmed ? Status: InProgress                                      ?
?     SignalR "ConfirmationStatusUpdate" sent with shouldDismissDialog: true  ?
?                                                                              ?
?  Timeout Path (15 min):                                                      ?
?  • Background Service marks as NoShow                                       ?
?  • Sends ConfirmationStatusUpdate with shouldDismissDialog: true            ?
?                                                                              ?
?  Reopen from Panel:                                                          ?
?  • User clicks notification ? clears shownBookingIds ? shows dialog         ?
?                                                                              ?
???????????????????????????????????????????????????????????????????????????????
```

---

## Key Behavior Changes

### "Close" Button (Previously "Decline")
- **NO API call** is made when clicking "Close"
- Dialog simply closes locally
- Booking ID is added to `shownBookingIds` set
- Dialog will NOT auto-reopen for that booking
- User can still reopen from notification panel

### "Confirm" Button
- Calls `POST /api/Booking/confirm-appointment` with `isConfirmed: true`
- Records user's confirmation
- When both parties confirm ? Status changes to InProgress

### NoShow Status
- **Only** set by background service when 15-minute timeout expires
- NOT set by user action (no decline button)

---

## API Endpoints

### 1. Confirm Appointment (Only Positive Confirmations)
```http
POST /api/Booking/confirm-appointment
Authorization: Bearer {token}
Content-Type: application/json

{
  "bookingId": 123,
  "isConfirmed": true
}
```

**Response when first party confirms (200 OK):**
```json
{
  "success": true,
  "message": "Confirmation recorded. Waiting for the other party to confirm.",
  "data": {
    "id": 123,
    "status": 1,  // Still Confirmed
    "carOwnerConfirmed": true,
    "workshopOwnerConfirmed": null,
    "confirmationSentAt": "2024-12-03T14:00:00Z",
    "confirmationDeadline": "2024-12-03T14:15:00Z"
  }
}
```

**Response when both parties confirm (200 OK):**
```json
{
  "success": true,
  "message": "Both parties confirmed! Appointment is now in progress.",
  "data": {
    "id": 123,
    "status": 2,  // InProgress
    "carOwnerConfirmed": true,
    "workshopOwnerConfirmed": true
  }
}
```

### 2. Get Confirmation Status
```http
GET /api/Booking/{bookingId}/confirmation-status
Authorization: Bearer {token}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "bookingId": 123,
    "carOwnerConfirmed": true,
    "workshopConfirmed": null,
    "bothConfirmed": false,
    "status": "Confirmed",
    "confirmationSentAt": "2024-12-03T14:00:00Z",
    "confirmationDeadline": "2024-12-03T14:15:00Z",
    "remainingSeconds": 542
  }
}
```

### 3. Get Pending Confirmations (For Page Load)
```http
GET /api/Notification/pending-confirmations
Authorization: Bearer {token}
```

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "notificationId": 456,
      "bookingId": 123,
      "notificationType": 13,
      "message": "Your appointment at AutoFix Workshop is now. Please confirm your arrival.",
      "title": "Appointment Confirmation",
      "confirmationSentAt": "2024-12-03T14:00:00Z",
      "confirmationDeadline": "2024-12-03T14:15:00Z",
      "remainingSeconds": 542,
      "isExpired": false,
      "carOwnerConfirmed": null,
      "workshopOwnerConfirmed": null,
      "carOwnerName": "Ahmed Ali",
      "workshopName": "AutoFix Workshop",
      "canStillConfirm": true,
      "serviceName": "Oil Change"
    }
  ]
}
```

### 4. Get Notification Details (For Reopening from Panel)
```http
GET /api/Notification/{notificationId}/details
Authorization: Bearer {token}
```

---

## SignalR Events

### Event: ReceiveNotification
**Triggered when:** Appointment time arrives

```typescript
connection.on('ReceiveNotification', (notification: NotificationDto) => {
  if (notification.type === 13) { // AppointmentConfirmationRequest
    // Check if this booking was already shown and closed
    if (!shownBookingIds.has(notification.bookingId)) {
      showConfirmationDialog(notification);
    }
  }
});
```

### Event: ConfirmationStatusUpdate
**Triggered when:** One or both parties confirm, or timeout occurs

```typescript
connection.on('ConfirmationStatusUpdate', (update) => {
  if (update.shouldDismissDialog) {
    // Close dialog - booking is now InProgress or NoShow (timeout)
    closeConfirmationDialog(update.bookingId);
    
    if (update.newBookingStatus === 'InProgress') {
      showSuccessToast('Appointment confirmed! Service is now in progress.');
    } else if (update.newBookingStatus === 'NoShow') {
      showWarningToast('Appointment expired due to timeout.');
    }
  } else {
    // Update dialog to show other party's confirmation
    updateDialogStatus({
      bookingId: update.bookingId,
      carOwnerConfirmed: update.carOwnerConfirmed,
      workshopOwnerConfirmed: update.workshopOwnerConfirmed
    });
  }
});
```

**Payload:**
```json
{
  "bookingId": 123,
  "carOwnerConfirmed": true,
  "workshopOwnerConfirmed": true,
  "newBookingStatus": "InProgress",
  "shouldDismissDialog": true,
  "updatedAt": "2024-12-03T14:05:00Z"
}
```

---

## Testing Scenarios

### Scenario 1: Happy Path - Both Parties Confirm

1. **Appointment time arrives** ? Both receive `ReceiveNotification`
2. **Car Owner clicks "Confirm":**
   ```http
   POST /api/Booking/confirm-appointment
   { "bookingId": 123, "isConfirmed": true }
   ```
3. **Workshop receives update:**
   ```
   ConfirmationStatusUpdate: { carOwnerConfirmed: true, shouldDismissDialog: false }
   ```
4. **Workshop clicks "Confirm":**
   ```http
   POST /api/Booking/confirm-appointment
   { "bookingId": 123, "isConfirmed": true }
   ```
5. **Both receive dismiss signal:**
   ```
   ConfirmationStatusUpdate: { newBookingStatus: "InProgress", shouldDismissDialog: true }
   ```

---

### Scenario 2: User Closes Dialog (No API Call)

1. **User receives confirmation dialog**
2. **User clicks "Close" (X button)**
   - Dialog closes locally
   - `bookingId` added to `shownBookingIds`
   - **NO API call is made**
3. **Dialog will NOT auto-reopen** (booking is in shownBookingIds)
4. **User can still reopen from notification panel**
   - When clicking notification, clear from `shownBookingIds`
   - Call `GET /api/Notification/{id}/details`
   - Show dialog with preserved timer

---

### Scenario 3: Timeout (15 Minutes, No Response)

1. **Both receive confirmation dialog**
2. **Neither clicks "Confirm" within 15 minutes**
3. **Background service marks as NoShow**
4. **Both receive:**
   ```
   ConfirmationStatusUpdate: { newBookingStatus: "NoShow", shouldDismissDialog: true }
   ```

---

### Scenario 4: One Confirms, Other Times Out

1. **Car Owner clicks "Confirm"**
2. **Workshop receives update showing car owner confirmed**
3. **Workshop closes dialog (or doesn't respond)**
4. **15-minute timeout expires**
5. **Background service marks as NoShow**
6. **Both receive:**
   ```
   ConfirmationStatusUpdate: { newBookingStatus: "NoShow", shouldDismissDialog: true }
   ```

---

### Scenario 5: Reopen from Notification Panel

1. **User previously closed dialog**
2. **User clicks notification in panel**
3. **Frontend calls:**
   ```http
   GET /api/Notification/{notificationId}/details
   ```
4. **Check response:**
   - If `canStillConfirm === true` ? Show dialog with `remainingSeconds` timer
   - If `canStillConfirm === false` ? Show "Confirmation window expired" message
5. **Clear booking from `shownBookingIds`** so dialog can show

---

## Frontend Implementation Checklist

### Dialog Behavior
- [ ] "Confirm" button calls API with `isConfirmed: true`
- [ ] "Close" button just closes dialog (NO API call)
- [ ] Track shown bookings in `shownBookingIds` Set
- [ ] Don't auto-show dialog if booking is in `shownBookingIds`
- [ ] Clear from `shownBookingIds` when reopening from panel

### SignalR Listeners
- [ ] `ReceiveNotification` - Show dialog (if not in shownBookingIds)
- [ ] `ConfirmationStatusUpdate` - Update or dismiss dialog

### Page Load
- [ ] Call `GET /api/Notification/pending-confirmations`
- [ ] Show dialogs only for bookings NOT in `shownBookingIds`
- [ ] Respect `canStillConfirm` flag

### Notification Panel
- [ ] Click on notification ? Call `GET /api/Notification/{id}/details`
- [ ] Clear booking from `shownBookingIds`
- [ ] Show dialog if `canStillConfirm === true`

---

## Quick Reference

| Action | API Call | Result |
|--------|----------|--------|
| Click "Confirm" | `POST /confirm-appointment` with `isConfirmed: true` | Records confirmation |
| Click "Close" | **None** | Dialog closes locally |
| Both confirm | - | Status ? InProgress |
| Timeout (15 min) | Background service | Status ? NoShow |
| Reopen from panel | `GET /notification/{id}/details` | Shows dialog with remaining time |

---

## Booking Status Values

| Status | Value | Description |
|--------|-------|-------------|
| Pending | 0 | Awaiting workshop response |
| Confirmed | 1 | Accepted, waiting for appointment |
| **InProgress** | **2** | **Both confirmed, service started** |
| Cancelled | 3 | Booking cancelled |
| ReadyForPickup | 4 | Car ready for pickup |
| Completed | 5 | Service completed |
| Rejected | 6 | Rejected by workshop |
| **NoShow** | **7** | **Timeout - neither/one confirmed** |
