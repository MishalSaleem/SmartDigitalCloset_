# Build Clean Script for SmartDigitalCloset
# This script ensures a clean build by stopping any running processes first

Write-Host "Stopping any running SmartDigitalCloset processes..." -ForegroundColor Yellow

# Stop specific process ID if provided as parameter
if ($args.Count -gt 0) {
    $processId = $args[0]
    Write-Host "Stopping process ID: $processId" -ForegroundColor Cyan
    Get-Process -Id $processId -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
}

# Stop specific SmartDigitalCloset processes
Get-Process -Name "*SmartDigitalCloset*" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue

# Stop any dotnet processes
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue

# Stop and remove any background jobs
Get-Job -ErrorAction SilentlyContinue | Stop-Job -ErrorAction SilentlyContinue
Get-Job -ErrorAction SilentlyContinue | Remove-Job -ErrorAction SilentlyContinue

# Remove locked exe file if it exists
Write-Host "Removing any locked executable files..." -ForegroundColor Yellow
Remove-Item -Path "SmartDigitalCloset\bin\Debug\net7.0\SmartDigitalCloset.exe" -Force -ErrorAction SilentlyContinue

Write-Host "Cleaning project..." -ForegroundColor Yellow
Set-Location "SmartDigitalCloset"
dotnet clean

Write-Host "Building project..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host "Build completed successfully!" -ForegroundColor Green
    Write-Host "You can now run: dotnet run" -ForegroundColor Cyan
} else {
    Write-Host "Build failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}
