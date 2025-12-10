# ?? FINAL SETUP - COPY & PASTE COMMANDS

## ? Your Current Status

- ? Webhook secret updated in `appsettings.json`
- ? Code fixed with proper event handling
- ? PaymentService has all required methods
- ? Build successful
- ? Your API runs on port **7046** (https profile)

---

## ?? EXECUTE THESE COMMANDS NOW

### Terminal 1: Navigate to Stripe CLI Directory

```powershell
cd C:\Users\Ahmad\Downloads\Compressed\stripe_1.33.0_windows_x86_64
```

### Terminal 1: Start Stripe Webhook Listener

```powershell
stripe listen --forward-to https://localhost:7046/api/payment/webhook
```

**Expected Output:**
```
> Ready! You are using Stripe API Version [2025-03-31.basil]. 
  Your webhook signing secret is whsec_8d33954895e8919a707da97351f669eea1f4c51c05246021f0560467f486a92d 
  (^C to quit)
```

? **KEEP THIS TERMINAL OPEN!** Don't close it.

---

### Visual Studio: Start Your API

1. Press **F5** or click the green play button
2. Wait for the browser to open with Swagger
3. Check Visual Studio **Output** window for:
   ```
   Now listening on: https://localhost:7046
   ```

---

### Terminal 2: Test Webhook (Open NEW PowerShell)

```powershell
cd C:\Users\Ahmad\Downloads\Compressed\stripe_1.33.0_windows_x86_64

stripe trigger payment_intent.succeeded
```

---

## ? SUCCESS INDICATORS

### In Terminal 1 (Stripe CLI):
You should see:
```
2025-12-10 XX:XX:XX   --> payment_intent.created [evt_xxxxx]
2025-12-10 XX:XX:XX            POST https://localhost:7046/api/payment/webhook [200] OK

2025-12-10 XX:XX:XX   --> payment_intent.succeeded [evt_xxxxx]
2025-12-10 XX:XX:XX            POST https://localhost:7046/api/payment/webhook [200] OK

2025-12-10 XX:XX:XX   --> charge.succeeded [evt_xxxxx]
2025-12-10 XX:XX:XX            POST https://localhost:7046/api/payment/webhook [200] OK
```

? **[200] OK** = SUCCESS! No more connection errors!

### In Visual Studio Output Window:
```
[Webhook] Received event: payment_intent.created with ID: evt_xxxxx
[Webhook] Unhandled event type: payment_intent.created

[Webhook] Received event: payment_intent.succeeded with ID: evt_xxxxx
[Webhook] Processing payment success for PaymentIntent: pi_xxxxx
[PaymentService] Starting HandlePaymentSuccessAsync for PaymentIntent: pi_xxxxx
[PaymentService] ERROR: Payment not found for PaymentIntent: pi_xxxxx
```

?? **Note:** "Payment not found" is EXPECTED because the test event uses a fake PaymentIntent ID.
The webhook IS working correctly! ?

---

## ?? Test with REAL Payment Flow

### Step 1: Create a Booking (if you don't have one)

Go to Swagger: `https://localhost:7046/swagger`

Use: **POST /api/booking** (adjust based on your booking endpoint)

### Step 2: Create Payment Intent

**Endpoint:** `POST /api/payment/create-payment-intent`

**Request Body:**
```json
{
  "bookingId": 1,
  "totalAmount": 100.00
}
```

**Response:** Copy the `clientSecret` value

### Step 3: Test Payment in Frontend

Use your frontend with test card:
```
Card: 4242 4242 4242 4242
Expiry: 12/25
CVC: 123
ZIP: 12345
```

### Step 4: Watch the Logs

**Stripe CLI Terminal:**
```
payment_intent.succeeded [evt_xxxxx]
POST https://localhost:7046/api/payment/webhook [200] OK
```

**Visual Studio Output:**
```
[Webhook] Received event: payment_intent.succeeded
[PaymentService] ? Payment processed successfully. Payment ID: X, Booking ID: Y
```

**Database:**
```sql
SELECT * FROM Payments WHERE Id = X
-- StripePaymentStatus = 2 (Succeeded)
-- PaidAt = recent timestamp

SELECT * FROM Bookings WHERE Id = Y
-- PaymentStatus = 1 (Paid)
-- Status = Completed
-- PaidAmount = 100.00
```

---

## ?? If You Still Get Connection Errors

### Check Your API Port

In Visual Studio Output window, look for:
```
Now listening on: https://localhost:XXXX
```

If the port is **NOT 7046**, update your stripe listen command:
```powershell
stripe listen --forward-to https://localhost:XXXX/api/payment/webhook
```

Replace `XXXX` with your actual port.

### Check Which Profile You're Using

1. Look at the dropdown next to the green play button in Visual Studio
2. Common options:
   - **"https"** ? uses port `7046`
   - **"IIS Express"** ? uses port `44316`
   - **"http"** ? uses port `5154`

3. Use the correct port in your stripe listen command

---

## ?? Quick Checklist

- [ ] API is running (F5 in Visual Studio)
- [ ] Stripe CLI listener is running in Terminal 1
- [ ] Webhook secret is in `appsettings.json` (already done ?)
- [ ] Port in stripe listen matches port in Visual Studio output
- [ ] No "[ERROR] Failed to POST" in Stripe CLI
- [ ] See "[200] OK" when triggering test webhook
- [ ] See "[Webhook] Received event" in Visual Studio output

---

## ?? YOU'RE DONE!

If you see **[200] OK** in Stripe CLI, your webhook is working perfectly!

The "Payment not found" error in the logs is normal for test webhooks because they use fake IDs.

**Next steps:**
1. Test with a real payment flow (create booking ? payment intent ? complete payment)
2. Verify status updates in database
3. Deploy to production and update webhook URL in Stripe Dashboard

---

## ?? Need More Help?

Check these files for detailed guides:
- `CORRECT_PORT_SETUP_GUIDE.md` - Port configuration
- `WEBHOOK_TROUBLESHOOTING.md` - Common issues
- `STRIPE_WEBHOOK_SETUP_GUIDE.md` - Complete setup guide
- `QUICK_START_WEBHOOKS.txt` - 5-minute quick start

---

## ?? Every Time You Debug

You only need to do 2 things:

1. **Start API** (F5 in Visual Studio)

2. **Start Stripe CLI** (in terminal):
   ```powershell
   stripe listen --forward-to https://localhost:7046/api/payment/webhook
   ```

That's it! The webhook secret is already configured and won't change (unless you restart stripe listen).

---

**Happy Coding! ??**
