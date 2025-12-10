# ? COMPLETE SETUP GUIDE - Stripe Webhook Local Testing

## ?? Your Correct Configuration

Based on your `launchSettings.json`, here are your **actual API ports**:

| Launch Profile | URL |
|----------------|-----|
| **https** (Default) | `https://localhost:7046` |
| **http** | `http://localhost:5154` |
| **IIS Express** | `https://localhost:44316` |

---

## ?? Step-by-Step Setup

### Step 1: Make Sure Your API is Running

1. Open Visual Studio
2. Press **F5** to start debugging
3. Check the **console output** to see which port it's using
4. You should see something like:
   ```
   Now listening on: https://localhost:7046
   Now listening on: http://localhost:5154
   ```

5. **Note the HTTPS port** (usually 7046)

---

### Step 2: Start Stripe CLI with Correct Port

Open **PowerShell** or **Command Prompt** in the Stripe CLI directory:

```powershell
cd C:\Users\Ahmad\Downloads\Compressed\stripe_1.33.0_windows_x86_64
```

Then run **ONE** of these commands based on your launch profile:

#### ? If using "https" profile (MOST COMMON):
```powershell
stripe listen --forward-to https://localhost:7046/api/payment/webhook
```

#### If using "IIS Express" profile:
```powershell
stripe listen --forward-to https://localhost:44316/api/payment/webhook
```

#### If using "http" profile (NOT RECOMMENDED):
```powershell
stripe listen --forward-to http://localhost:5154/api/payment/webhook
```

---

### Step 3: Copy the Webhook Secret

After running the command, you'll see:

```
> Ready! Your webhook signing secret is whsec_xxxxxxxxxxxxx (^C to quit)
```

**? The secret you got is:**
```
whsec_8d33954895e8919a707da97351f669eea1f4c51c05246021f0560467f486a92d
```

**Good news!** You've already updated `appsettings.json` with this secret! ?

---

### Step 4: Verify Everything is Connected

You should now see in the Stripe CLI terminal:
```
> Ready! You are using Stripe API Version [2025-03-31.basil]. 
  Your webhook signing secret is whsec_8d33954895e8919a707da97351f669eea1f4c51c05246021f0560467f486a92d
  (^C to quit)
```

**No more "[ERROR] Failed to POST" messages!** ?

---

### Step 5: Test the Webhook

In a **NEW PowerShell terminal** (keep Stripe CLI running in the first one):

```powershell
cd C:\Users\Ahmad\Downloads\Compressed\stripe_1.33.0_windows_x86_64

stripe trigger payment_intent.succeeded
```

---

## ? Success Indicators

### In Stripe CLI Terminal:
```
2025-12-10 19:50:00   --> payment_intent.created [evt_xxxxx]
2025-12-10 19:50:00            POST https://localhost:7046/api/payment/webhook [200] OK
2025-12-10 19:50:00   --> payment_intent.succeeded [evt_xxxxx]
2025-12-10 19:50:00            POST https://localhost:7046/api/payment/webhook [200] OK
2025-12-10 19:50:00   --> charge.succeeded [evt_xxxxx]
2025-12-10 19:50:00            POST https://localhost:7046/api/payment/webhook [200] OK
```

? **[200] OK** means success!

### In Your API Console (Visual Studio Output):
```
[Webhook] Received event: payment_intent.created with ID: evt_xxxxx
[Webhook] Unhandled event type: payment_intent.created

[Webhook] Received event: payment_intent.succeeded with ID: evt_xxxxx
[Webhook] Processing payment success for PaymentIntent: pi_xxxxx
[PaymentService] Starting HandlePaymentSuccessAsync for PaymentIntent: pi_xxxxx
[PaymentService] ? Payment processed successfully. Payment ID: X, Booking ID: Y

[Webhook] Received event: charge.succeeded with ID: evt_xxxxx
[Webhook] Unhandled event type: charge.succeeded
```

---

## ?? Troubleshooting

### Problem: Still getting "connection refused"

**Check 1: Is your API actually running?**
```
Look for "Now listening on: https://localhost:7046" in Visual Studio output
```

**Check 2: Are you using the correct port?**
```
The port in stripe listen command MUST match the port your API is running on
```

**Check 3: Check which profile you're using**
- Look at the dropdown next to the green play button in Visual Studio
- It should show "https", "http", or "IIS Express"
- Most common is "https" which uses port 7046

---

### Problem: Webhook called but payment status not updating

**Solution 1: Check you have a payment in the database**
```sql
SELECT TOP 1 * FROM Payments ORDER BY Id DESC
```

**Solution 2: Check the PaymentIntent ID**
The test webhook creates a fake PaymentIntent ID that doesn't exist in your database.

**For real testing:**
1. Create a booking through your frontend/Swagger
2. Create a payment intent (this creates a Payment record)
3. Complete the payment with test card `4242 4242 4242 4242`
4. The webhook will automatically be triggered with the REAL PaymentIntent ID

---

### Problem: "Webhook secret not configured"

**Solution:**
Make sure `appsettings.json` has the webhook secret (you've already done this! ?)

**After updating, you MUST restart your API:**
- Stop debugging (Shift+F5)
- Start debugging again (F5)

---

## ?? Complete Testing Workflow

### Test with Real Payment Flow:

1. **Start API** (F5 in Visual Studio)
   - Should show: `Now listening on: https://localhost:7046`

2. **Start Stripe CLI** (in separate terminal):
   ```powershell
   stripe listen --forward-to https://localhost:7046/api/payment/webhook
   ```

3. **Go to Swagger** (or your frontend)
   - Navigate to: `https://localhost:7046/swagger`

4. **Create a Booking** (if you don't have one)
   - Use: `POST /api/booking`

5. **Create Payment Intent**
   - Use: `POST /api/payment/create-payment-intent`
   - Request body:
     ```json
     {
       "bookingId": 1,
       "totalAmount": 100.00
     }
     ```
   - Copy the `clientSecret` from response

6. **Complete Payment** (in your frontend)
   - Use the test card: `4242 4242 4242 4242`
   - Any future expiry: `12/25`
   - Any CVC: `123`
   - Any ZIP: `12345`

7. **Watch the Magic Happen!**
   - Stripe CLI shows: `POST .../webhook [200] OK`
   - API logs show: `[PaymentService] ? Payment processed successfully`
   - Database shows: Payment status = Succeeded, Booking status = Completed

---

## ?? Quick Reference

### Your API Ports:
- **HTTPS (default):** `https://localhost:7046`
- **HTTP:** `http://localhost:5154`
- **IIS Express:** `https://localhost:44316`

### Stripe CLI Commands:
```powershell
# Start listener (use your actual port!)
stripe listen --forward-to https://localhost:7046/api/payment/webhook

# Test webhook
stripe trigger payment_intent.succeeded

# View recent events
stripe events list --limit 10

# View logs
stripe logs tail
```

### Test Card:
```
Card Number: 4242 4242 4242 4242
Expiry: 12/25 (any future date)
CVC: 123 (any 3 digits)
ZIP: 12345 (any 5 digits)
```

---

## ?? Important Notes

1. **Keep Stripe CLI Running**
   - Don't close the terminal where `stripe listen` is running
   - It must stay open while you're testing

2. **Every Time You Restart Stripe CLI**
   - You get a NEW webhook secret
   - You must update `appsettings.json`
   - You must restart your API

3. **Use HTTPS**
   - Always use `https://` not `http://` for webhooks
   - Stripe requires HTTPS for webhooks

4. **Port Must Match**
   - The port in `stripe listen` command
   - MUST match the port your API is actually running on
   - Check Visual Studio output to confirm

---

## ?? You're All Set!

Your webhook setup is now correct. The issue was:
- ? You were using port `44352` in stripe listen
- ? Your API actually runs on port `7046`

**What to do next:**
1. Make sure API is running (F5)
2. Run: `stripe listen --forward-to https://localhost:7046/api/payment/webhook`
3. Test: `stripe trigger payment_intent.succeeded`
4. Should see `[200] OK` in Stripe CLI ?

Happy testing! ??
