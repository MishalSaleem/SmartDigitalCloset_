# Simple Profile Functionality Verification
Write-Host "Profile Functionality Verification" -ForegroundColor Cyan

# Check Profile.razor
if (Test-Path "C:\Users\Dell\source\repos\SmartDigitalCloset\SmartDigitalCloset\Pages\Profile.razor") {
    Write-Host "✓ Profile.razor exists" -ForegroundColor Green
} else {
    Write-Host "✗ Profile.razor missing" -ForegroundColor Red
}

# Check Profile.razor.css
if (Test-Path "C:\Users\Dell\source\repos\SmartDigitalCloset\SmartDigitalCloset\Pages\Profile.razor.css") {
    Write-Host "✓ Profile.razor.css exists" -ForegroundColor Green
} else {
    Write-Host "✗ Profile.razor.css missing" -ForegroundColor Red
}

# Test build
Write-Host "Testing build..." -ForegroundColor Yellow
Set-Location "C:\Users\Dell\source\repos\SmartDigitalCloset\SmartDigitalCloset"
$buildResult = dotnet build --verbosity quiet
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Build successful" -ForegroundColor Green
} else {
    Write-Host "✗ Build failed" -ForegroundColor Red
}

Write-Host "Verification complete!" -ForegroundColor Cyan
