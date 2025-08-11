@echo off
REM Quick Build Script - Stops processes and builds the project
echo Stopping SmartDigitalCloset processes...
taskkill /f /im SmartDigitalCloset.exe 2>nul
taskkill /f /im dotnet.exe 2>nul

echo Cleaning and building project...
cd SmartDigitalCloset
dotnet clean
dotnet build

if %ERRORLEVEL% EQU 0 (
    echo Build completed successfully!
    echo You can now run: dotnet run
) else (
    echo Build failed!
    pause
)
