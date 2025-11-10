![clip2load-banner](https://github.com/user-attachments/assets/97d2fc64-b038-4e0b-9875-8c580f4bd1a5)

*This project is maintained as an open-source project by us at Frostcloud, and contributions are always welcome!* <br>
*We'd love to be part of your next project that requires hosting, check us out!* https://thefrostcloud.com

# clip2load
clip2load allows you to process GTA V `.clip` files by identifying and patching specific resource names within the binary clip data. This is useful for removing references to mods, resources, or other identifiable content that you may not want included in your clips. Useful for preventing R* Editor crashes while attempting to load escrowed assets that are stored in the .clip files.

## Features

- 📁 **Clip File Management** - Browse and select GTA V clip files from your clips folder
- 🔍 **Resource Blocking** - Define resource names or patterns to remove from clip files
- 🎯 **Pattern Matching** - Support for wildcards (`*` and `?`) to match multiple resource variations
- 💾 **Auto-Save Resources** - Blocked resource lists are automatically saved and loaded
- 🔄 **Automatic Backups** - All original files are backed up before processing with timestamped folders

## System Requirements

- **Operating System**: Windows 10/11
- **.NET Desktop Runtime**: Install at least 8.0.21, preferably newer if available.
- **GTAV**: Installed with valid clips folder

Download link for .NET: https://dotnet.microsoft.com/en-us/download/dotnet/8.0

## Installation

1. Install the .NET Desktop Runtime 8.0.21 (or newer if available) 
2. Download the latest release from the releases page
3. Extract the archive to your desired location
4. Run `clip2load.exe`

## Support
For issues, questions, or feature requests, please visit the [GitHub Issues](https://github.com/Frostcloud-Development/clip2load-repository/issues) page.

## Usage Guide

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

## Engineered with ❤️ by Frostcloud!
We at Frostcloud believe in giving back to the community, therefore we release these tools for free, for you to use!
If you would like to support us in continuing to give back, please consider using us for your next project that requires hosting!
We provide all various services, and would love to be part of your next project!

Consider visiting our website, https://thefrostcloud.com, and thank you for using clip2load!

[<img width="256" height="256" alt="logo_variation_five" src="https://github.com/user-attachments/assets/b1ac9a9d-40e4-44b5-902e-fe5322b2cba0" />](https://thefrostcloud.com/)
