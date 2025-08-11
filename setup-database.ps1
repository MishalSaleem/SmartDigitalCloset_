$server = "(localdb)\MSSQLLocalDB" # Change this if you're using a different SQL Server instance
$database = "master"
$sqlScriptPath = ".\SmartDigitalCloset\Data\EnsureDatabase.sql"

Write-Host "Setting up your database..." -ForegroundColor Green

# Run the SQL script
try {
    sqlcmd -S $server -d $database -i $sqlScriptPath
    Write-Host "Database setup successfully!" -ForegroundColor Green
    Write-Host "Your items will now be saved to both the JSON file and SQL database." -ForegroundColor Green
}
catch {
    Write-Host "Error setting up database: $_" -ForegroundColor Red
    Write-Host "You may need to manually run the SQL script in SQL Server Management Studio." -ForegroundColor Yellow
}
