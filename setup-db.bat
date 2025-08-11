@echo off
echo Setting up your Smart Digital Closet database...
sqlcmd -S "DESKTOP-ACEH8N2\SQLEXPRESS" -i setup-database.sql
echo Database setup completed!
echo You can now run your application and add/delete items - they will sync with the database.
pause
