# Stripe Webhook Local Development Setup Script
# This script helps you set up Stripe CLI for local webhook testing

Write-Host "=================================================" -ForegroundColor Cyan
Write-Host "   Stripe Webhook Local Development Setup" -ForegroundColor Cyan
Write-Host "=================================================" -ForegroundColor Cyan
Write-Host ""

# Check if Stripe CLI is installed
Write-Host "Checking for Stripe CLI installation..." -ForegroundColor Yellow
$stripeCLI = Get-Command stripe -ErrorAction SilentlyContinue

if (-not $stripeCLI) {
    Write-Host "? Stripe CLI not found!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please install Stripe CLI first:" -ForegroundColor Yellow
    Write-Host "  Option 1 (Scoop):" -ForegroundColor White
    Write-Host "    scoop bucket add stripe https://github.com/stripe/scoop-stripe-cli.git" -ForegroundColor Gray
    Write-Host "    scoop install stripe" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  Option 2 (Direct Download):" -ForegroundColor White
    Write-Host "    https://github.com/stripe/stripe-cli/releases/latest" -ForegroundColor Gray
    Write-Host ""
    exit 1
}

Write-Host "? Stripe CLI found!" -ForegroundColor Green
Write-Host ""

# Check if logged in
Write-Host "Checking Stripe CLI authentication..." -ForegroundColor Yellow
$stripeAccount = stripe config --list 2>&1

if ($stripeAccount -like "*No account*" -or $stripeAccount -like "*not authenticated*") {
    Write-Host "? Not logged in to Stripe CLI" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please run: stripe login" -ForegroundColor Yellow
    Write-Host ""
    exit 1
}

Write-Host "? Authenticated with Stripe!" -ForegroundColor Green
Write-Host ""

# Get API URL from launchSettings.json
$launchSettingsPath = "Korik.API\Properties\launchSettings.json"
$apiUrl = "https://localhost:44352"  # Default

if (Test-Path $launchSettingsPath) {
    try {
        $launchSettings = Get-Content $launchSettingsPath | ConvertFrom-Json
        $httpsUrl = $launchSettings.profiles.https.applicationUrl
        if ($httpsUrl) {
            $apiUrl = $httpsUrl
        }
    } catch {
        Write-Host "??  Could not read launchSettings.json, using default: $apiUrl" -ForegroundColor Yellow
    }
}

Write-Host "API URL detected: $apiUrl" -ForegroundColor Cyan
Write-Host ""

# Instructions
Write-Host "=================================================" -ForegroundColor Cyan
Write-Host "   Next Steps:" -ForegroundColor Cyan
Write-Host "=================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Start your API (Debug or 'dotnet run')" -ForegroundColor White
Write-Host ""
Write-Host "2. Run this command in a NEW terminal:" -ForegroundColor White
Write-Host "   stripe listen --forward-to $apiUrl/api/payment/webhook" -ForegroundColor Green
Write-Host ""
Write-Host "3. Copy the webhook secret (whsec_xxxxx) from the output" -ForegroundColor White
Write-Host ""
Write-Host "4. Update appsettings.json with the webhook secret:" -ForegroundColor White
Write-Host '   "Stripe": {' -ForegroundColor Gray
Write-Host '     "WebhookSecret": "whsec_xxxxx"  <- paste here' -ForegroundColor Gray
Write-Host '   }' -ForegroundColor Gray
Write-Host ""
Write-Host "5. Test the webhook:" -ForegroundColor White
Write-Host "   stripe trigger payment_intent.succeeded" -ForegroundColor Green
Write-Host ""

# Ask if user wants to start listening now
Write-Host "=================================================" -ForegroundColor Cyan
Write-Host ""
$response = Read-Host "Do you want to start Stripe webhook listener now? (y/n)"

if ($response -eq "y" -or $response -eq "Y") {
    Write-Host ""
    Write-Host "Starting Stripe webhook listener..." -ForegroundColor Yellow
    Write-Host "Press Ctrl+C to stop" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "??  COPY THE WEBHOOK SECRET (whsec_xxxxx) AND UPDATE appsettings.json!" -ForegroundColor Yellow
    Write-Host ""
    
    # Start stripe listen
    stripe listen --forward-to "$apiUrl/api/payment/webhook"
} else {
    Write-Host ""
    Write-Host "Run this command when ready:" -ForegroundColor Cyan
    Write-Host "stripe listen --forward-to $apiUrl/api/payment/webhook" -ForegroundColor Green
    Write-Host ""
}
