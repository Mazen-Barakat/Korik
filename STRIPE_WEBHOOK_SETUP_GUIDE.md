# Stripe Webhook Setup Guide for Local Development

## Problem
When debugging locally, Stripe cannot reach your `localhost` webhook endpoint because:
- Your application runs on `localhost` (not publicly accessible)
- Stripe webhooks need a public URL to send events to

## Solution: Use Stripe CLI

### Step 1: Install Stripe CLI

**Windows (using Scoop):**
```powershell
scoop bucket add stripe https://github.com/stripe/scoop-stripe-cli.git
scoop install stripe
```

**Or download directly:**
https://github.com/stripe/stripe-cli/releases/latest

**Mac/Linux:**
```bash
brew install stripe/stripe-cli/stripe
```

### Step 2: Login to Stripe CLI
```bash
stripe login
```
This will open your browser to authenticate with your Stripe account.

### Step 3: Forward Webhooks to Your Local API

Run this command **while your API is running**:
```bash
stripe listen --forward-to https://localhost:44352/api/payment/webhook
```

**Important:** Replace the port `44352` with your actual API port.

### Step 4: Get the Webhook Signing Secret

When you run `stripe listen`, it will output something like:
```
> Ready! Your webhook signing secret is whsec_xxxxxxxxxxxxx (^C to quit)
```

**Copy this secret!** This is your webhook secret for local development.

### Step 5: Update appsettings.json

Replace the placeholder webhook secret in `Korik.API/appsettings.json`:

```json
"Stripe": {
  "PublishableKey": "pk_test_...",
  "SecretKey": "sk_test_...",
  "WebhookSecret": "whsec_xxxxxxxxxxxxx"  // ?? Paste the secret from stripe listen
}
```

### Step 6: Test the Webhook

In a **new terminal**, trigger a test webhook:
```bash
stripe trigger payment_intent.succeeded
```

You should see:
1. In Stripe CLI terminal: Event forwarded successfully
2. In your API console: Webhook received and processed logs
3. In your database: Payment status updated to "Succeeded"

---

## Common Issues & Solutions

### Issue 1: "Webhook secret not configured"
**Solution:** Make sure you updated `appsettings.json` with the webhook secret from `stripe listen`

### Issue 2: "No signatures found matching the expected signature for payload"
**Solution:** 
- Restart `stripe listen`
- Get the new webhook secret
- Update `appsettings.json`
- Restart your API

### Issue 3: Webhook not triggered during payment
**Solution:** 
- Ensure `stripe listen` is running
- Check the Stripe CLI terminal for forwarded events
- Verify your API is running on the correct port

### Issue 4: Payment succeeds but status doesn't update
**Solution:**
- Check your API console logs for "[Webhook] Received event: payment_intent.succeeded"
- Verify the PaymentIntent ID exists in your database
- Check for any exceptions in the `HandlePaymentSuccessAsync` method

---

## Testing Flow

1. **Start your API** (Visual Studio Debug or `dotnet run`)
2. **Start Stripe CLI listener** in a separate terminal:
   ```bash
   stripe listen --forward-to https://localhost:44352/api/payment/webhook
   ```
3. **Update webhook secret** in `appsettings.json`
4. **Create a payment** through your frontend or Swagger
5. **Complete the payment** using test card: `4242 4242 4242 4242`
6. **Watch Stripe CLI** - you should see:
   ```
   payment_intent.succeeded [evt_xxxxx]
   POST https://localhost:44352/api/payment/webhook [200]
   ```
7. **Verify in database** - Payment status should be "Succeeded"

---

## Production Webhook Setup

For production, you'll need to:

1. **Create a webhook endpoint** in Stripe Dashboard:
   - Go to: https://dashboard.stripe.com/webhooks
   - Click "Add endpoint"
   - Enter: `https://yourdomain.com/api/payment/webhook`
   - Select events: `payment_intent.succeeded`, `payment_intent.payment_failed`

2. **Get the signing secret** from the webhook details page

3. **Update production appsettings.json** with the production webhook secret

---

## Debugging Tips

### Enable detailed logging in your webhook:
The webhook now includes console logging. Check your API output for:
```
[Webhook] Received event: payment_intent.succeeded with ID: evt_xxxxx
[Webhook] Processing payment success for PaymentIntent: pi_xxxxx
```

### Test webhook manually with curl:
```bash
stripe trigger payment_intent.succeeded --override payment_intent:id=pi_your_test_id
```

### View all webhook events:
```bash
stripe events list --limit 10
```

### Resend a specific webhook event:
```bash
stripe events resend evt_xxxxx
```

---

## Quick Reference

| Command | Description |
|---------|-------------|
| `stripe login` | Authenticate Stripe CLI |
| `stripe listen --forward-to URL` | Forward webhooks to local API |
| `stripe trigger EVENT_NAME` | Trigger test event |
| `stripe events list` | View recent events |
| `stripe logs tail` | Watch API logs in real-time |

---

## Test Card Numbers

| Card | Scenario |
|------|----------|
| `4242 4242 4242 4242` | Successful payment |
| `4000 0000 0000 9995` | Payment declined |
| `4000 0025 0000 3155` | Requires authentication (3D Secure) |

**Expiry:** Any future date (e.g., 12/25)  
**CVC:** Any 3 digits (e.g., 123)  
**ZIP:** Any 5 digits (e.g., 12345)
