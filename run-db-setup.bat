@echo off
echo Setting up your Smart Digital Closet database...
sqlcmd -S "(localdb)\MSSQLLocalDB" -d master -i .\SmartDigitalCloset\Data\EnsureDatabase.sql
echo Database setup completed!
echo You can now run your application and add/delete items - they will sync with the database.
pause
