# Run the SmartDigitalCloset application
Write-Host "Starting SmartDigitalCloset application..." -ForegroundColor Green

# Navigate to project directory
Set-Location "C:\Users\Dell\source\repos\SmartDigitalCloset\SmartDigitalCloset"

# Build the project
Write-Host "Building project..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host "Build successful! Starting application..." -ForegroundColor Green
    
    # Start the application
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "dotnet run"
    
    # Wait a moment for the app to start
    Start-Sleep 3
    
    # Open browser
    Write-Host "Opening browser..." -ForegroundColor Cyan
    Start-Process "http://localhost:5000"
    
    Write-Host "Application should be running at http://localhost:5000" -ForegroundColor Green
} else {
    Write-Host "Build failed. Please check the errors above." -ForegroundColor Red
}
