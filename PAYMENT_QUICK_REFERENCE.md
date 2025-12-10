# ? Payment System - Quick Reference Card

## ?? Next Steps (3 Commands)

```powershell
# 1. Install EF Core Tools (if needed)
dotnet tool install --global dotnet-ef

# 2. Create Migration
dotnet ef migrations add AddPaymentEntity --project Korik.Infrastructure --startup-project Korik.API

# 3. Update Database
dotnet ef database update --project Korik.Infrastructure --startup-project Korik.API
```

---

## ?? What's Included

| Layer | Files Created | Status |
|-------|--------------|--------|
| Domain | 1 | ? Complete |
| Application | 13 | ? Complete |
| Infrastructure | 3 | ? Complete |
| API | 1 | ? Complete |
| **Total** | **18** | **? Ready** |

---

## ?? Key Features

- ? Stripe payment processing
- ? Automatic 12% commission
- ? Pending payout tracking
- ? Manual payout recording
- ? Payment history
- ? Refund support
- ? Webhook handling

---

## ?? Commission Formula

```
Total: $100.00
Commission (12%): $12.00
Workshop Gets: $88.00
```

---

## ?? API Endpoints (7)

1. **POST** `/api/Payment/create-payment-intent` - Create payment
2. **GET** `/api/Payment/booking/{id}` - Get payment details
3. **GET** `/api/Payment/pending-payouts` - List pending (Admin)
4. **POST** `/api/Payment/mark-paid-out` - Record payout (Admin)
5. **GET** `/api/Payment/payout-history` - View history (Admin)
6. **POST** `/api/Payment/refund/{id}` - Refund (Admin)
7. **POST** `/api/Payment/webhook` - Stripe callback

---

## ? Build Status

```
? Build: SUCCESSFUL
? Package: Stripe.NET v50.0.0 installed
? DI: All services registered
? Config: Stripe keys in appsettings.json
```

---

## ?? Testing Order

1. Run migration (3 commands above)
2. Start app (F5)
3. Open Swagger
4. Login to get JWT token
5. Test `/create-payment-intent`
6. Check database for payment record
7. Test other endpoints

---

## ?? Need Help?

- **Setup**: See `PAYMENT_IMPLEMENTATION_COMPLETE.md`
- **Testing**: See `SWAGGER_STEP_BY_STEP_GUIDE.md`
- **Frontend**: See `FRONTEND_IMPLEMENTATION_GUIDE.md`
- **Troubleshooting**: See `SWAGGER_TROUBLESHOOTING.md`

---

**?? Payment system is ready! Run the 3 commands above to finish setup.**
