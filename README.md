# Smart Digital Closet

A Blazor Server application for managing your digital wardrobe and interests.

## Prerequisites

Before you begin, ensure you have installed:
- [.NET SDK](https://dotnet.microsoft.com/download) (version 7.0 or later)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express edition or higher)
- [Visual Studio](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) with C# extensions

## Setup Instructions

1. **Clone the Repository**
   ```powershell
   git clone https://github.com/MishalSaleem/SmartDigitalCloset_.git
   cd SmartDigitalCloset_
   ```

2. **Database Setup**
   - Open SQL Server Management Studio or Azure Data Studio
   - Run the following scripts in order:
     1. `setup-database.sql`
     2. `SmartDigitalCloset/Data/CreateDatabase.sql`
     3. `SmartDigitalCloset/Data/CreateTables.sql`
   
   Alternatively, use the provided setup script:
   ```powershell
   .\setup-database.ps1
   ```

3. **Configure Connection String**
   - Open `SmartDigitalCloset/appsettings.json`
   - Update the connection string with your SQL Server details:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=ClosetAppDB2;Trusted_Connection=True;"
     }
   }
   ```

4. **Install Dependencies**
   ```powershell
   dotnet restore
   ```

5. **Build and Run**
   ```powershell
   dotnet build
   dotnet run --project SmartDigitalCloset
   ```

   Or use the provided scripts:
   ```powershell
   .\build-and-run.ps1
   ```

## Features

- 👕 Digital Closet Management
- 👔 Outfit Planner
- ❤️ Favorites System
- 👤 User Profiles
- 🎵 Interest Categories (Music, Art, Fashion, etc.)
- 📱 Responsive Design

## Development Tools

- **Quick Build**: Use `quick-build.bat` for rapid development
- **Database Setup**: Use `setup-db.bat` for database initialization
- **Verification**: Use `verify-*.ps1` scripts for feature verification

## Troubleshooting

1. **Database Connection Issues**
   - Verify SQL Server is running
   - Check connection string in appsettings.json
   - Ensure Windows Authentication is enabled

2. **Build Errors**
   - Run `dotnet clean` followed by `dotnet build`
   - Check for any missing NuGet packages
   - Verify .NET SDK version matches project requirements

## Contributing

1. Fork the repository
2. Create your feature branch
3. Commit your changes
4. Push to the branch
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details