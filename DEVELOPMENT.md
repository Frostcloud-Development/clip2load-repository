## Building from Source

### Prerequisites
- Visual Studio 2022 or later
- .NET 8.0 SDK
- Windows Forms development workload

### Build Steps

1. Clone the repository:
   ```powershell
   git clone https://github.com/Frostcloud-Development/clip2load-repository.git
   cd clip2load-repository
   ```

2. Open the solution:
   ```powershell
   start clip2load.sln
   ```

3. Build the project:
   - Press `Ctrl+Shift+B` or
   - Use Visual Studio menu: Build → Build Solution

4. Run the application:
   - Press `F5` (Debug) or `Ctrl+F5` (Release)

### Command Line Build

```powershell
dotnet build clip2load.sln --configuration Release
```

The compiled executable will be in:
```
bin/Release/net8.0-windows/clip2load.exe
```

## Project Structure

```
clip2load/
├── Program.cs              # Application entry point with Sentry initialization
├── Application.cs          # Main form with UI logic and event handlers
├── ProcessingClips.cs      # Core clip processing engine
├── clip2load.csproj        # Project configuration
└── bin/
    └── Debug|Release/
        └── net8.0-windows/
            ├── backups/        # Timestamped backup folders
            └── storage/        # Persistent resource list 
```