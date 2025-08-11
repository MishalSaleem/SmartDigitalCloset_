# This script builds and runs the SmartDigitalCloset application
# It handles common errors and provides solutions

Write-Host "Building SmartDigitalCloset app..." -ForegroundColor Cyan

# Navigate to the project directory
cd "$PSScriptRoot\SmartDigitalCloset"

# Clean the project first
dotnet clean
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error cleaning project. Continuing anyway..." -ForegroundColor Yellow
}

# Build the project
dotnet build
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed. Checking for common issues..." -ForegroundColor Red
    
    # Check for issues with NavigateTo calls
    $navIssues = Select-String -Path .\Pages\*.razor -Pattern '@onclick="() => Navigation.NavigateTo\("' -SimpleMatch

    if ($navIssues.Count -gt 0) {
        Write-Host "Found potential issues with NavigateTo calls:" -ForegroundColor Yellow
        foreach ($issue in $navIssues) {
            Write-Host "  - $($issue.Path): line $($issue.LineNumber)" -ForegroundColor Yellow
            Write-Host "    Fix: Replace with @onclick='() => Navigation.NavigateTo(\"/path\")'" -ForegroundColor Green
        }
    }
    
    Write-Host "Please fix the errors and run the script again." -ForegroundColor Red
    exit 1
}

Write-Host "Build successful! Running SmartDigitalCloset app..." -ForegroundColor Green

# Run the application
dotnet run
