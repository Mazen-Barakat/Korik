# Webhook Troubleshooting Checklist

## ? Pre-Flight Checklist

Before testing webhooks, verify:

- [ ] Stripe CLI is installed (`stripe --version`)
- [ ] Logged into Stripe CLI (`stripe login`)
- [ ] API is running (check browser at https://localhost:44352/swagger)
- [ ] Stripe listener is running (`stripe listen --forward-to ...`)
- [ ] Webhook secret is updated in `appsettings.json`
- [ ] API has been restarted after updating webhook secret

---

## ?? Common Issues & Solutions

### Issue 1: Webhook Not Called At All

**Symptoms:**
- Payment completes on frontend
- No webhook logs in API console
- No event shown in Stripe CLI terminal

**Solutions:**
1. **Check if Stripe CLI listener is running**
   ```bash
   # You should see "Ready! Your webhook signing secret is..."
   stripe listen --forward-to https://localhost:44352/api/payment/webhook
   ```

2. **Verify the payment is actually completing**
   - Check frontend console for payment status
   - Look for "payment_intent.succeeded" in Stripe Dashboard > Developers > Events

3. **Test webhook manually**
   ```bash
   stripe trigger payment_intent.succeeded
   ```
   - Should see in Stripe CLI: `POST https://localhost:44352/api/payment/webhook [200]`
   - Should see in API logs: `[Webhook] Received event: payment_intent.succeeded`

---

### Issue 2: "No signatures found matching the expected signature"

**Symptoms:**
- Webhook is called
- Error: `StripeException: No signatures found matching the expected signature for payload`

**Solutions:**
1. **Restart Stripe CLI listener**
   - Stop current listener (Ctrl+C)
   - Restart: `stripe listen --forward-to https://localhost:44352/api/payment/webhook`
   - **Copy the NEW webhook secret**

2. **Update appsettings.json with the new secret**
   ```json
   "Stripe": {
     "WebhookSecret": "whsec_NEW_SECRET_HERE"
   }
   ```

3. **Restart your API**
   - Stop debugging
   - Rebuild and start again

**Why this happens:**
- Each time you run `stripe listen`, it generates a new signing secret
- Old secrets become invalid

---

### Issue 3: Payment Status Not Updating

**Symptoms:**
- Webhook is called successfully
- No errors in logs
- Payment/Booking status remains unchanged

**Solutions:**
1. **Check API console logs**
   Look for these log messages:
   ```
   [Webhook] Received event: payment_intent.succeeded with ID: evt_xxxxx
   [Webhook] Processing payment success for PaymentIntent: pi_xxxxx
   [PaymentService] Starting HandlePaymentSuccessAsync for PaymentIntent: pi_xxxxx
   [PaymentService] Found payment ID: 123, Current status: Pending
   [PaymentService] ? Payment processed successfully
   ```

2. **Verify PaymentIntent ID exists in database**
   ```sql
   SELECT * FROM Payments WHERE StripePaymentIntentId = 'pi_xxxxx'
   ```
   - If not found: Payment wasn't created properly during CreatePaymentIntent
   - Check CreatePaymentIntent logs

3. **Check for exceptions**
   Look for `[PaymentService] EXCEPTION` in logs

4. **Verify Booking relationship**
   ```sql
   SELECT p.*, b.* 
   FROM Payments p
   INNER JOIN Bookings b ON p.BookingId = b.Id
   WHERE p.StripePaymentIntentId = 'pi_xxxxx'
   ```

---

### Issue 4: "Webhook secret not configured"

**Symptoms:**
- API returns: `{"error": "Webhook secret not configured"}`

**Solutions:**
1. **Check appsettings.json**
   ```json
   "Stripe": {
     "WebhookSecret": "whsec_xxxxxxxxxxxxx"  // Must NOT be the placeholder
   }
   ```

2. **Ensure you copied the correct secret**
   - From Stripe CLI output after running `stripe listen`
   - Should start with `whsec_`

3. **Restart API after updating**

---

### Issue 5: Webhook Works with stripe trigger but Not Real Payment

**Symptoms:**
- `stripe trigger payment_intent.succeeded` works fine
- Real payment from frontend doesn't trigger webhook

**Solutions:**
1. **Check frontend is using correct PaymentIntent**
   - Verify frontend is using the `client_secret` returned from CreatePaymentIntent
   - Frontend should call Stripe.confirmCardPayment with this secret

2. **Verify payment actually succeeds**
   - Check Stripe Dashboard > Payments
   - Look for the payment with status "Succeeded"

3. **Check if event was sent**
   - Stripe Dashboard > Developers > Events
   - Search for `payment_intent.succeeded` with your PaymentIntent ID

4. **Frontend integration issue**
   - See FRONTEND_PAYMENT_INTEGRATION_GUIDE.md

---

## ?? Debugging Commands

### View recent Stripe events
```bash
stripe events list --limit 10
```

### View specific event details
```bash
stripe events retrieve evt_xxxxxxxxxxxxx
```

### Resend a webhook event
```bash
stripe events resend evt_xxxxxxxxxxxxx
```

### Watch Stripe API logs in real-time
```bash
stripe logs tail
```

### Test specific PaymentIntent
```bash
# Get your PaymentIntent ID from database or API response
stripe trigger payment_intent.succeeded --override payment_intent:id=pi_xxxxxxxxxxxxx
```

---

## ?? Verification Flow

After a successful payment, verify each step:

1. **Frontend Payment Succeeded**
   - Frontend console shows: "Payment succeeded!"
   - No errors in browser console

2. **Stripe Event Created**
   - Stripe Dashboard > Developers > Events
   - Find `payment_intent.succeeded` event

3. **Webhook Forwarded**
   - Stripe CLI terminal shows:
     ```
     payment_intent.succeeded [evt_xxxxx]
     POST https://localhost:44352/api/payment/webhook [200]
     ```

4. **API Received Webhook**
   - API console shows:
     ```
     [Webhook] Received event: payment_intent.succeeded with ID: evt_xxxxx
     ```

5. **Payment Service Processed**
   - API console shows:
     ```
     [PaymentService] ? Payment processed successfully. Payment ID: X, Booking ID: Y
     ```

6. **Database Updated**
   ```sql
   -- Payment should have:
   SELECT StripePaymentStatus, PaidAt FROM Payments WHERE Id = X
   -- Result: StripePaymentStatus = 2 (Succeeded), PaidAt = recent timestamp
   
   -- Booking should have:
   SELECT PaymentStatus, Status, PaidAmount FROM Bookings WHERE Id = Y
   -- Result: PaymentStatus = 1 (Paid), Status = Completed, PaidAmount = amount
   ```

---

## ?? Still Having Issues?

### Collect this information:
1. Full API console output
2. Stripe CLI terminal output
3. Frontend browser console output
4. Screenshot of Stripe Dashboard > Developers > Events
5. Database query results:
   ```sql
   SELECT TOP 5 * FROM Payments ORDER BY CreatedAt DESC
   SELECT TOP 5 * FROM Bookings ORDER BY CreatedAt DESC
   ```

### Enable verbose logging:
In `appsettings.json`:
```json
"Logging": {
  "LogLevel": {
    "Default": "Debug",
    "Microsoft.AspNetCore": "Information"
  }
}
```

---

## ? Success Indicators

You know everything is working when:
- ? Stripe CLI shows: `POST .../api/payment/webhook [200]`
- ? API logs show: `[PaymentService] ? Payment processed successfully`
- ? Database shows: Payment status = Succeeded, Booking status = Completed
- ? Frontend shows: Payment successful message
