# clip2load

A Windows Forms application for processing GTA V clip files by removing or replacing resource references. Ideal for content creators who need to clean up clip files before uploading or sharing.

## Overview

clip2load allows you to process GTA V `.clip` files by identifying and patching specific resource names within the binary clip data. This is useful for removing references to mods, resources, or other identifiable content that you may not want included in your clips.

## Features

- 📁 **Clip File Management** - Browse and select GTA V clip files from your clips folder
- 🔍 **Resource Blocking** - Define resource names or patterns to remove from clip files
- 🎯 **Pattern Matching** - Support for wildcards (`*` and `?`) to match multiple resource variations
- 💾 **Auto-Save Resources** - Blocked resource lists are automatically saved and loaded
- 🔄 **Automatic Backups** - All original files are backed up before processing with timestamped folders
- 📊 **Real-time Logging** - Track all operations with detailed timestamped logs
- 🛡️ **Error Tracking** - Integrated Sentry SDK for monitoring and error reporting

## System Requirements

- **OS**: Windows (with .NET 8.0 Windows Runtime)
- **Target Framework**: .NET 8.0-windows
- **GTA V**: Installed with valid clips folder

## Installation

1. Download the latest release from the releases page
2. Extract the archive to your desired location
3. Run `clip2load.exe`

## Usage

### Getting Started

1. **Launch the application** - The default GTA V clips folder will be automatically detected:
   ```
   C:\Users\[YourUsername]\AppData\Local\Rockstar Games\GTA V\videos\clips
   ```

2. **Add Blocked Resources**:
   - Enter a resource name in the "Resource Name" field
   - Press Enter or click "Add Resource"
   - Resources are automatically saved for future sessions

3. **Select Clips to Process**:
   - Browse available clips in the "All Clips" list
   - Select one or more clips
   - Click "Move to Selected" to add them to the processing queue

4. **Start Processing**:
   - Click "Start Conversion Process"
   - Monitor progress in the logging window
   - Original files are automatically backed up before modification

### Resource Patterns

You can use patterns to match multiple resource variations:

- **Exact match**: `my_resource_name`
- **Wildcard**: `my_resource_*` (matches anything starting with "my_resource_")
- **Single character**: `resource_?` (matches any single character)
- **Case sensitivity**: Currently case-insensitive by default

### Examples

**Block specific resources:**
```
fivem-racing
custom_cars
my_private_mod
```

**Block resource families:**
```
vMenu_*
esx_*
qb-*
```

## Data Storage

### Blocked Resources
Blocked resource lists are stored in:
```
storage/blocked_resources.dat
```

The file is automatically created and updated when you add or remove resources.

### Backups
Each processing session creates a new backup folder with the format:
```
backups/YYYY-MM-DD_HH-MM-SS/
```

Original clip files are preserved with their original filenames in these folders.

## Configuration

### Changing the Clips Folder
If your GTA V clips are in a different location:
1. Click "Browse Folder" in the application
2. Select your clips directory
3. The new path will be used for the current session

## Troubleshooting

### Clips folder not found
- Verify GTA V is installed
- Check the clips folder path in the application
- Use "Browse Folder" to manually select your clips directory

### No patterns found in clips
- Ensure resource names are spelled correctly
- Try using wildcards for broader matching
- Check the log for processing details

### Processing errors
- Ensure clip files are not in use by another application
- Verify you have write permissions to the clips folder
- Check the log window for specific error messages

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues for bugs and feature requests.

## Support

For issues, questions, or feature requests, please visit the [GitHub Issues](https://github.com/Frostcloud-Development/clip2load-repository/issues) page.

## Disclaimer

This tool modifies GTA V clip files. While automatic backups are created, always ensure you have copies of important clips before processing. Use at your own risk.
