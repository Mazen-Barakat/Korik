# Webhook Debugging - Issue Fixed! ?

## What Was Wrong

### 1. **Bug in EventUtility.ConstructEvent Call** ?
**Location:** `PaymentController.cs` - Line 65

**Before (Incorrect):**
```csharp
var stripeEvent = EventUtility.ConstructEvent(
    json,
    Request.Headers["Stripe-Signature"],  // ? Passing signature from header
    signature,                             // ? AND passing it again
    webhookSecret
);
```

**After (Fixed):** ?
```csharp
var stripeEvent = EventUtility.ConstructEvent(
    json,
    signature,      // ? Only pass signature once
    webhookSecret
);
```

**Why this matters:**
- `EventUtility.ConstructEvent` expects 3 parameters: (json, signature, secret)
- You were passing 4 parameters, causing Stripe to fail validation
- This would cause webhooks to always fail with signature validation errors

---

### 2. **Placeholder Webhook Secret** ??
**Location:** `appsettings.json`

**Current (Invalid):**
```json
"Stripe": {
  "WebhookSecret": "whsec_your_webhook_secret_here"  // ? Placeholder
}
```

**Needs to be:**
```json
"Stripe": {
  "WebhookSecret": "whsec_actual_secret_from_stripe_cli"  // ? Real secret
}
```

---

### 3. **Localhost Not Publicly Accessible** ??
**The Core Problem:**
- During debugging, your API runs on `localhost` (e.g., https://localhost:44352)
- Stripe servers **cannot** reach `localhost` directly
- Stripe needs a public URL to send webhook events

**The Solution: Stripe CLI**
- Stripe CLI creates a secure tunnel from Stripe ? your localhost
- Acts as a proxy: Stripe ? Stripe CLI ? Your local API

---

## How to Fix It - Quick Start

### Step 1: Install Stripe CLI (One-Time Setup)

**Windows:**
```powershell
scoop bucket add stripe https://github.com/stripe/scoop-stripe-cli.git
scoop install stripe
```

Or download: https://github.com/stripe/stripe-cli/releases/latest

**Mac/Linux:**
```bash
brew install stripe/stripe-cli/stripe
```

---

### Step 2: Login to Stripe (One-Time Setup)
```bash
stripe login
```
This opens your browser to authenticate.

---

### Step 3: Start Your API
- Press F5 in Visual Studio to start debugging
- Or run: `dotnet run --project Korik.API`

---

### Step 4: Start Stripe Webhook Listener
In a **separate terminal**, run:
```bash
stripe listen --forward-to https://localhost:44352/api/payment/webhook
```

**Output will look like:**
```
> Ready! Your webhook signing secret is whsec_xxxxxxxxxxxxxxxxxxxxx (^C to quit)
```

**? COPY THAT SECRET!**

---

### Step 5: Update appsettings.json
Replace the placeholder in `Korik.API/appsettings.json`:

```json
"Stripe": {
  "PublishableKey": "pk_test_...",
  "SecretKey": "sk_test_...",
  "WebhookSecret": "whsec_xxxxxxxxxxxxxxxxxxxxx"  // ?? Paste the secret here
}
```

---

### Step 6: Restart Your API
- **Important:** Stop debugging and restart
- API needs to reload the new webhook secret from appsettings.json

---

### Step 7: Test It!

**Option A: Test with Stripe CLI**
```bash
stripe trigger payment_intent.succeeded
```

**Option B: Test with Real Payment**
1. Go to your frontend
2. Create a booking
3. Make a payment with test card: `4242 4242 4242 4242`
4. Complete the payment

---

## What You Should See When It Works ?

### In Stripe CLI Terminal:
```
payment_intent.succeeded [evt_1abc2def3ghi]
POST https://localhost:44352/api/payment/webhook [200]
```

### In Your API Console:
```
[Webhook] Received event: payment_intent.succeeded with ID: evt_1abc2def3ghi
[Webhook] Processing payment success for PaymentIntent: pi_1xyz2abc3def
[PaymentService] Starting HandlePaymentSuccessAsync for PaymentIntent: pi_1xyz2abc3def
[PaymentService] Found payment ID: 123, Current status: Pending
[PaymentService] Confirming payment with Stripe...
[PaymentService] Stripe confirmation successful. Status: succeeded
[PaymentService] Updating booking ID: 456
[PaymentService] ? Payment processed successfully. Payment ID: 123, Booking ID: 456
```

### In Your Database:
```sql
-- Payments table
SELECT Id, StripePaymentStatus, PaidAt, StripePaymentIntentId 
FROM Payments 
WHERE Id = 123

-- Result:
-- StripePaymentStatus = 2 (Succeeded)
-- PaidAt = 2024-12-10 14:30:00  (recent timestamp)

-- Bookings table
SELECT Id, PaymentStatus, Status, PaidAmount 
FROM Bookings 
WHERE Id = 456

-- Result:
-- PaymentStatus = 1 (Paid)
-- Status = Completed
-- PaidAmount = 100.00
```

---

## Files Changed

### ? Fixed Files:
1. **Korik.API/Controllers/Payment/PaymentController.cs**
   - Fixed `EventUtility.ConstructEvent` call
   - Added detailed console logging

2. **Korik.Application/Services/Payment/PaymentService.cs**
   - Added detailed console logging in `HandlePaymentSuccessAsync`

### ?? New Documentation Files:
1. **STRIPE_WEBHOOK_SETUP_GUIDE.md** - Complete setup instructions
2. **WEBHOOK_TROUBLESHOOTING.md** - Troubleshooting checklist
3. **setup-stripe-webhooks.ps1** - Automated setup script
4. **THIS FILE** - Quick summary

---

## Quick Reference Commands

```bash
# Login (one-time)
stripe login

# Start webhook listener (every debug session)
stripe listen --forward-to https://localhost:44352/api/payment/webhook

# Test webhook manually
stripe trigger payment_intent.succeeded

# View recent events
stripe events list --limit 10

# View real-time Stripe logs
stripe logs tail
```

---

## Important Notes

### ?? Development vs Production

**Development (Local Debugging):**
- Use Stripe CLI with `stripe listen`
- Webhook secret changes each time you run `stripe listen`
- Must update `appsettings.json` each session

**Production (Deployed App):**
- Create webhook endpoint in Stripe Dashboard
- Webhook secret is permanent (doesn't change)
- Set webhook secret in production configuration

### ?? Each Debug Session:
1. Start API
2. Start `stripe listen` in separate terminal
3. Copy webhook secret
4. Update `appsettings.json`
5. Restart API

**OR** use the automated script:
```powershell
.\setup-stripe-webhooks.ps1
```

### ?? If Webhook Secret Changes:
You'll see this error:
```
StripeException: No signatures found matching the expected signature for payload
```

**Solution:**
1. Restart `stripe listen`
2. Copy new secret
3. Update `appsettings.json`
4. Restart API

---

## Testing Checklist

- [ ] Stripe CLI installed
- [ ] Logged into Stripe (`stripe login`)
- [ ] API running
- [ ] `stripe listen` running in separate terminal
- [ ] Webhook secret copied from `stripe listen` output
- [ ] `appsettings.json` updated with webhook secret
- [ ] API restarted after updating secret
- [ ] Test: `stripe trigger payment_intent.succeeded`
- [ ] See success logs in API console
- [ ] Verify database updated

---

## Need Help?

1. **Check the detailed guides:**
   - `STRIPE_WEBHOOK_SETUP_GUIDE.md` - Step-by-step setup
   - `WEBHOOK_TROUBLESHOOTING.md` - Common issues & solutions

2. **Run the setup script:**
   ```powershell
   .\setup-stripe-webhooks.ps1
   ```

3. **Check your logs:**
   - API console output
   - Stripe CLI terminal output
   - Browser console (frontend)

4. **Verify in Stripe Dashboard:**
   - Payments: https://dashboard.stripe.com/test/payments
   - Events: https://dashboard.stripe.com/test/events
   - Webhooks: https://dashboard.stripe.com/test/webhooks

---

## Summary

**Problem:** Webhooks not triggering during local debugging

**Root Causes:**
1. ? Bug in webhook signature validation code
2. ?? Placeholder webhook secret in config
3. ?? Localhost not publicly accessible to Stripe

**Solutions:**
1. ? Fixed signature validation in `PaymentController.cs`
2. ? Use Stripe CLI to forward webhooks to localhost
3. ? Update webhook secret from `stripe listen` output
4. ? Restart API after configuration change
5. ? Added detailed logging for debugging

**Status:** ?? **FIXED AND READY TO TEST!**
