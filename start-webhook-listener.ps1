# Auto-Start Stripe Webhook Listener for Korik API
# This script automatically starts the Stripe CLI webhook listener with the correct port

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Korik API - Stripe Webhook Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if we're in the Stripe CLI directory
$currentDir = Get-Location
$stripeExe = Join-Path $currentDir "stripe.exe"

if (-not (Test-Path $stripeExe)) {
    Write-Host "? ERROR: stripe.exe not found in current directory!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please navigate to the Stripe CLI directory first:" -ForegroundColor Yellow
    Write-Host "cd C:\Users\Ahmad\Downloads\Compressed\stripe_1.33.0_windows_x86_64" -ForegroundColor White
    Write-Host ""
    Write-Host "Then run this script again." -ForegroundColor Yellow
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "? Found Stripe CLI in: $currentDir" -ForegroundColor Green
Write-Host ""

# Check if logged in to Stripe
Write-Host "Checking Stripe authentication..." -ForegroundColor Yellow
$configTest = & $stripeExe config --list 2>&1

if ($configTest -like "*not authenticated*" -or $configTest -like "*No account*") {
    Write-Host "? Not authenticated with Stripe!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please run: stripe login" -ForegroundColor Yellow
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "? Authenticated with Stripe!" -ForegroundColor Green
Write-Host ""

# Your API configuration
$apiPort = "7046"  # Default https port from launchSettings.json
$webhookPath = "/api/payment/webhook"
$webhookUrl = "https://localhost:$apiPort$webhookPath"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Configuration" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "API Port: $apiPort" -ForegroundColor White
Write-Host "Webhook URL: $webhookUrl" -ForegroundColor White
Write-Host ""

# Important instructions
Write-Host "========================================" -ForegroundColor Yellow
Write-Host "   IMPORTANT: Before Starting" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. Make sure your API is running in Visual Studio (F5)" -ForegroundColor White
Write-Host "2. Check Visual Studio Output for 'Now listening on: https://localhost:7046'" -ForegroundColor White
Write-Host "3. If your API uses a different port, edit this script and update the apiPort variable" -ForegroundColor White
Write-Host ""

$confirmation = Read-Host "Is your API running on https://localhost:$apiPort? (y/n)"

if ($confirmation -ne "y" -and $confirmation -ne "Y") {
    Write-Host ""
    Write-Host "Please start your API first, then run this script again." -ForegroundColor Yellow
    Write-Host ""
    
    # Ask if they want to use a different port
    $customPort = Read-Host "If your API uses a different port, enter it now (or press Enter to exit)"
    
    if ($customPort -and $customPort -match '^\d+$') {
        $apiPort = $customPort
        $webhookUrl = "https://localhost:$apiPort$webhookPath"
        Write-Host ""
        Write-Host "Updated webhook URL to: $webhookUrl" -ForegroundColor Cyan
        Write-Host ""
    } else {
        Write-Host ""
        Write-Host "Exiting..." -ForegroundColor Yellow
        Read-Host "Press Enter to exit"
        exit 0
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "   Starting Stripe Webhook Listener" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Forwarding webhooks to: $webhookUrl" -ForegroundColor Cyan
Write-Host ""
Write-Host "??  IMPORTANT: When the listener starts, you'll see a webhook secret like:" -ForegroundColor Yellow
Write-Host "    'Your webhook signing secret is whsec_xxxxx...'" -ForegroundColor Yellow
Write-Host ""
Write-Host "??  This secret is ALREADY configured in your appsettings.json:" -ForegroundColor Green
Write-Host "    whsec_8d33954895e8919a707da97351f669eea1f4c51c05246021f0560467f486a92d" -ForegroundColor Gray
Write-Host ""
Write-Host "??  If the secret shown below is DIFFERENT, you must:" -ForegroundColor Yellow
Write-Host "    1. Copy the new secret" -ForegroundColor Yellow
Write-Host "    2. Update appsettings.json with the new secret" -ForegroundColor Yellow
Write-Host "    3. Restart your API (Shift+F5 then F5 in Visual Studio)" -ForegroundColor Yellow
Write-Host ""
Write-Host "Press Ctrl+C to stop the listener when done testing." -ForegroundColor White
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Start the listener
& $stripeExe listen --forward-to $webhookUrl

# This point is reached only if stripe listen exits
Write-Host ""
Write-Host "Stripe webhook listener stopped." -ForegroundColor Yellow
Write-Host ""
Read-Host "Press Enter to exit"
