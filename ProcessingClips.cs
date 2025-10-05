using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace clip2load
{
    public class ProcessingClips
    {
        // Processing configuration
        public enum PatchMode
        {
            Null,
            Placeholder
        }

        // Event for progress reporting
        public event Action<string> OnProgress;
        public event Action<string> OnError;
        public event Action<string> OnComplete;

        private readonly string backupDirectory;
        private readonly string storageDirectory;
        private readonly string resourcesFilePath;

        public ProcessingClips()
        {
            // Create backup directory structure in the executable root directory
            var appDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                ?? Environment.CurrentDirectory;

            // Create main backups folder
            var backupsRoot = Path.Combine(appDirectory, "backups");

            // Create timestamped subfolder for this execution
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            backupDirectory = Path.Combine(backupsRoot, timestamp);

            // Create storage directory and resources file path
            storageDirectory = Path.Combine(appDirectory, "storage");
            resourcesFilePath = Path.Combine(storageDirectory, "blocked_resources.dat");

            // Ensure storage directory exists
            Directory.CreateDirectory(storageDirectory);
        }

        /// <summary>
        /// Save blocked resources to the .dat file
        /// </summary>
        public async Task<bool> SaveBlockedResourcesAsync(List<string> blockedResources)
        {
            try
            {
                OnProgress?.Invoke($"Saving {blockedResources.Count} blocked resources to storage...");

                // Create storage data structure
                var storageData = new ResourceStorageData
                {
                    SavedDate = DateTime.Now,
                    Version = "1.0",
                    BlockedResources = blockedResources.ToList()
                };

                // Serialize to JSON and save to .dat file
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var jsonString = JsonSerializer.Serialize(storageData, jsonOptions);
                await File.WriteAllTextAsync(resourcesFilePath, jsonString, Encoding.UTF8);

                OnProgress?.Invoke($"Blocked resources saved to: {resourcesFilePath}");
                return true;
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Failed to save blocked resources: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Load blocked resources from the .dat file
        /// </summary>
        public async Task<List<string>> LoadBlockedResourcesAsync()
        {
            try
            {
                if (!File.Exists(resourcesFilePath))
                {
                    OnProgress?.Invoke("No saved blocked resources found - starting with empty list");
                    return new List<string>();
                }

                OnProgress?.Invoke("Loading blocked resources from storage...");

                var jsonString = await File.ReadAllTextAsync(resourcesFilePath, Encoding.UTF8);

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var storageData = JsonSerializer.Deserialize<ResourceStorageData>(jsonString, jsonOptions);

                if (storageData?.BlockedResources != null)
                {
                    OnProgress?.Invoke($"Loaded {storageData.BlockedResources.Count} blocked resources from {storageData.SavedDate:yyyy-MM-dd HH:mm:ss}");
                    return storageData.BlockedResources;
                }
                else
                {
                    OnProgress?.Invoke("Storage file exists but contains no valid data");
                    return new List<string>();
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Failed to load blocked resources: {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Get storage directory path
        /// </summary>
        public string GetStorageDirectory() => storageDirectory;

        /// <summary>
        /// Get resources file path
        /// </summary>
        public string GetResourcesFilePath() => resourcesFilePath;

        /// <summary>
        /// Check if resources file exists
        /// </summary>
        public bool ResourcesFileExists() => File.Exists(resourcesFilePath);

        /// <summary>
        /// Delete the resources file (for resetting)
        /// </summary>
        public async Task<bool> DeleteResourcesFileAsync()
        {
            try
            {
                if (File.Exists(resourcesFilePath))
                {
                    File.Delete(resourcesFilePath);
                    OnProgress?.Invoke("Blocked resources file deleted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Failed to delete resources file: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get information about the stored resources file
        /// </summary>
        public async Task<ResourceFileInfo> GetResourceFileInfoAsync()
        {
            var info = new ResourceFileInfo();

            try
            {
                if (!File.Exists(resourcesFilePath))
                {
                    info.Exists = false;
                    return info;
                }

                info.Exists = true;
                info.FilePath = resourcesFilePath;
                info.LastModified = File.GetLastWriteTime(resourcesFilePath);
                info.FileSizeBytes = new FileInfo(resourcesFilePath).Length;

                // Try to read the data to get more details
                var jsonString = await File.ReadAllTextAsync(resourcesFilePath, Encoding.UTF8);
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var storageData = JsonSerializer.Deserialize<ResourceStorageData>(jsonString, jsonOptions);
                if (storageData != null)
                {
                    info.ResourceCount = storageData.BlockedResources?.Count ?? 0;
                    info.SavedDate = storageData.SavedDate;
                    info.Version = storageData.Version;
                }
            }
            catch (Exception ex)
            {
                info.Error = ex.Message;
            }

            return info;
        }

        /// <summary>
        /// Process multiple clip files with the specified resource patterns
        /// </summary>
        public async Task<ProcessingResult> ProcessClipsAsync(List<string> clipFilePaths, List<string> blockedResources,
            PatchMode mode = PatchMode.Null, string placeholder = "REMOVED", bool caseInsensitive = false)
        {
            var result = new ProcessingResult();

            try
            {
                OnProgress?.Invoke("Starting clip processing...");

                if (!clipFilePaths.Any())
                {
                    OnError?.Invoke("No clip files selected for processing");
                    return result;
                }

                if (!blockedResources.Any())
                {
                    OnError?.Invoke("No blocked resources specified");
                    return result;
                }

                // Create backup directory structure
                if (!Directory.Exists(backupDirectory))
                {
                    Directory.CreateDirectory(backupDirectory);
                    OnProgress?.Invoke($"Created backup directory: {backupDirectory}");
                }
                else
                {
                    OnProgress?.Invoke($"Using existing backup directory: {backupDirectory}");
                }

                // Log backup structure info
                var backupsRoot = Path.GetDirectoryName(backupDirectory);
                OnProgress?.Invoke($"Backups root folder: {backupsRoot}");
                OnProgress?.Invoke($"Current execution backup folder: {Path.GetFileName(backupDirectory)}");

                result.TotalFiles = clipFilePaths.Count;
                result.ProcessedFiles = 0;
                result.PatchedFiles = 0;
                result.TotalPatches = 0;

                // Process each clip file
                foreach (var clipPath in clipFilePaths)
                {
                    OnProgress?.Invoke($"Processing: {Path.GetFileName(clipPath)}");

                    var fileResult = await ProcessSingleClipAsync(clipPath, blockedResources, mode, placeholder, caseInsensitive);

                    result.ProcessedFiles++;
                    result.TotalPatches += fileResult.PatchCount;

                    if (fileResult.PatchCount > 0)
                    {
                        result.PatchedFiles++;
                        OnProgress?.Invoke($"✓ Patched {fileResult.PatchCount} patterns in {Path.GetFileName(clipPath)}");
                    }
                    else
                    {
                        OnProgress?.Invoke($"- No patterns found in {Path.GetFileName(clipPath)}");
                    }

                    result.FileResults.Add(fileResult);
                }

                result.Success = true;
                result.BackupDirectory = backupDirectory;

                OnComplete?.Invoke($"Processing complete! Processed {result.ProcessedFiles} files, " +
                    $"patched {result.PatchedFiles} files with {result.TotalPatches} total patches.");

                return result;
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Processing failed: {ex.Message}");
                result.Success = false;
                result.ErrorMessage = ex.Message;
                return result;
            }
        }

        /// <summary>
        /// Process a single clip file
        /// </summary>
        private async Task<FileProcessingResult> ProcessSingleClipAsync(string clipPath, List<string> blockedResources,
            PatchMode mode, string placeholder, bool caseInsensitive)
        {
            var result = new FileProcessingResult
            {
                FilePath = clipPath,
                FileName = Path.GetFileName(clipPath)
            };

            try
            {
                if (!File.Exists(clipPath))
                {
                    result.ErrorMessage = "File not found";
                    return result;
                }

                // Create backup with original filename in the timestamped backup folder
                var backupPath = Path.Combine(backupDirectory, Path.GetFileName(clipPath));

                // Ensure backup directory exists (in case this is the first file)
                Directory.CreateDirectory(backupDirectory);

                // Copy original file to backup location
                File.Copy(clipPath, backupPath, true);
                result.BackupPath = backupPath;

                OnProgress?.Invoke($"Backed up: {Path.GetFileName(clipPath)} → {Path.GetFileName(backupDirectory)}");

                // Read file data
                var fileData = await File.ReadAllBytesAsync(clipPath);
                var originalData = (byte[])fileData.Clone();
                bool hasChanges = false;

                // Process each blocked resource pattern
                foreach (var resource in blockedResources)
                {
                    var matches = FindPatternMatches(fileData, resource, caseInsensitive);

                    foreach (var match in matches)
                    {
                        ApplyPatch(fileData, match.StartIndex, match.Length, mode, placeholder);
                        result.PatchCount++;
                        hasChanges = true;

                        result.PatchDetails.Add(new PatchDetail
                        {
                            Pattern = resource,
                            MatchedText = match.MatchedText,
                            StartIndex = match.StartIndex,
                            Length = match.Length
                        });
                    }
                }

                // Write changes if any patches were applied
                if (hasChanges)
                {
                    await File.WriteAllBytesAsync(clipPath, fileData);
                    result.Success = true;
                }
                else
                {
                    result.Success = true; // No changes needed is still success
                }

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                return result;
            }
        }

        /// <summary>
        /// Find pattern matches in binary data
        /// </summary>
        private List<PatternMatch> FindPatternMatches(byte[] data, string pattern, bool caseInsensitive)
        {
            var matches = new List<PatternMatch>();

            // Check if pattern contains wildcards
            if (pattern.Contains('*') || pattern.Contains('?'))
            {
                matches.AddRange(FindWildcardMatches(data, pattern, caseInsensitive));
            }
            else
            {
                matches.AddRange(FindExactMatches(data, pattern, caseInsensitive));
            }

            return matches;
        }

        /// <summary>
        /// Find exact pattern matches
        /// </summary>
        private List<PatternMatch> FindExactMatches(byte[] data, string pattern, bool caseInsensitive)
        {
            var matches = new List<PatternMatch>();
            var patternBytes = Encoding.ASCII.GetBytes(pattern);

            if (caseInsensitive)
            {
                var lowerPattern = Encoding.ASCII.GetBytes(pattern.ToLower());
                var upperPattern = Encoding.ASCII.GetBytes(pattern.ToUpper());

                matches.AddRange(FindBytesPattern(data, patternBytes, pattern));
                matches.AddRange(FindBytesPattern(data, lowerPattern, pattern.ToLower()));
                matches.AddRange(FindBytesPattern(data, upperPattern, pattern.ToUpper()));
            }
            else
            {
                matches.AddRange(FindBytesPattern(data, patternBytes, pattern));
            }

            return matches.Distinct().ToList();
        }

        /// <summary>
        /// Find wildcard pattern matches
        /// </summary>
        private List<PatternMatch> FindWildcardMatches(byte[] data, string pattern, bool caseInsensitive)
        {
            var matches = new List<PatternMatch>();

            // Convert wildcard pattern to regex
            var regexPattern = "^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$";
            var regex = new Regex(regexPattern, caseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None);

            // Extract ASCII strings from binary data
            var asciiStrings = ExtractAsciiStrings(data);

            foreach (var asciiString in asciiStrings)
            {
                if (regex.IsMatch(asciiString.Text))
                {
                    matches.Add(new PatternMatch
                    {
                        StartIndex = asciiString.StartIndex,
                        Length = asciiString.Text.Length,
                        MatchedText = asciiString.Text
                    });
                }
            }

            return matches;
        }

        /// <summary>
        /// Find byte pattern in data
        /// </summary>
        private List<PatternMatch> FindBytesPattern(byte[] data, byte[] pattern, string originalText)
        {
            var matches = new List<PatternMatch>();

            for (int i = 0; i <= data.Length - pattern.Length; i++)
            {
                bool found = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (data[i + j] != pattern[j])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    matches.Add(new PatternMatch
                    {
                        StartIndex = i,
                        Length = pattern.Length,
                        MatchedText = originalText
                    });
                }
            }

            return matches;
        }

        /// <summary>
        /// Extract ASCII strings from binary data
        /// </summary>
        private List<AsciiString> ExtractAsciiStrings(byte[] data)
        {
            var strings = new List<AsciiString>();
            var currentString = new StringBuilder();
            int startIndex = 0;

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] >= 32 && data[i] <= 126) // Printable ASCII range
                {
                    if (currentString.Length == 0)
                        startIndex = i;

                    currentString.Append((char)data[i]);
                }
                else
                {
                    if (currentString.Length > 0)
                    {
                        strings.Add(new AsciiString
                        {
                            StartIndex = startIndex,
                            Text = currentString.ToString()
                        });
                        currentString.Clear();
                    }
                }
            }

            // Handle string at end of file
            if (currentString.Length > 0)
            {
                strings.Add(new AsciiString
                {
                    StartIndex = startIndex,
                    Text = currentString.ToString()
                });
            }

            return strings;
        }

        /// <summary>
        /// Apply patch to binary data
        /// </summary>
        private void ApplyPatch(byte[] data, int startIndex, int length, PatchMode mode, string placeholder)
        {
            byte[] replacement;

            switch (mode)
            {
                case PatchMode.Null:
                    replacement = new byte[length]; // All zeros
                    break;
                case PatchMode.Placeholder:
                    var placeholderBytes = Encoding.ASCII.GetBytes(placeholder);
                    replacement = new byte[length];

                    // Repeat placeholder to fill the length
                    for (int i = 0; i < length; i++)
                    {
                        replacement[i] = placeholderBytes[i % placeholderBytes.Length];
                    }
                    break;
                default:
                    throw new ArgumentException($"Unknown patch mode: {mode}");
            }

            // Apply the replacement
            Array.Copy(replacement, 0, data, startIndex, length);
        }

        /// <summary>
        /// Get backup directory path
        /// </summary>
        public string GetBackupDirectory() => backupDirectory;

        /// <summary>
        /// Get the main backups root directory
        /// </summary>
        public string GetBackupsRootDirectory()
        {
            return Path.GetDirectoryName(backupDirectory) ?? "";
        }

        /// <summary>
        /// Get list of all backup execution folders
        /// </summary>
        public List<string> GetBackupExecutionFolders()
        {
            var backupsRoot = GetBackupsRootDirectory();
            if (!Directory.Exists(backupsRoot))
                return new List<string>();

            return Directory.GetDirectories(backupsRoot)
                .Select(Path.GetFileName)
                .Where(name => !string.IsNullOrEmpty(name))
                .OrderByDescending(name => name) // Most recent first
                .ToList();
        }
    }

    // Storage data structure
    public class ResourceStorageData
    {
        public DateTime SavedDate { get; set; }
        public string Version { get; set; } = "1.0";
        public List<string> BlockedResources { get; set; } = new();
    }

    // Resource file information
    public class ResourceFileInfo
    {
        public bool Exists { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public DateTime LastModified { get; set; }
        public DateTime SavedDate { get; set; }
        public long FileSizeBytes { get; set; }
        public int ResourceCount { get; set; }
        public string Version { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }

    // Supporting classes
    public class ProcessingResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public int TotalFiles { get; set; }
        public int ProcessedFiles { get; set; }
        public int PatchedFiles { get; set; }
        public int TotalPatches { get; set; }
        public string BackupDirectory { get; set; } = string.Empty;
        public List<FileProcessingResult> FileResults { get; set; } = new();
    }

    public class FileProcessingResult
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string BackupPath { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public int PatchCount { get; set; }
        public List<PatchDetail> PatchDetails { get; set; } = new();
    }

    public class PatchDetail
    {
        public string Pattern { get; set; } = string.Empty;
        public string MatchedText { get; set; } = string.Empty;
        public int StartIndex { get; set; }
        public int Length { get; set; }
    }

    public class PatternMatch
    {
        public int StartIndex { get; set; }
        public int Length { get; set; }
        public string MatchedText { get; set; } = string.Empty;
    }

    public class AsciiString
    {
        public int StartIndex { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
