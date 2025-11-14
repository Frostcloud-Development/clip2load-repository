using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace clip2load
{
    public partial class clip2load : Form
    {
        private string clipsFolder = @"C:\Users\Avenzey\AppData\Local\Rockstar Games\GTA V\videos\clips";
        private ProcessingClips clipProcessor;

        public clip2load()
        {
            InitializeComponent();
            SetupEventHandlers();

            // Initialize clip processor
            clipProcessor = new ProcessingClips();
            SetupProcessingEvents();
        }
        private void SetupProcessingEvents()
        {
            clipProcessor.OnProgress += (message) =>
            {
                if (InvokeRequired)
                    Invoke(() => LogMessage(message));
                else
                    LogMessage(message);
            };

            clipProcessor.OnError += (message) =>
            {
                if (InvokeRequired)
                    Invoke(() => LogMessage($"ERROR: {message}"));
                else
                    LogMessage($"ERROR: {message}");
            };

            clipProcessor.OnComplete += (message) =>
            {
                if (InvokeRequired)
                    Invoke(() => LogMessage($"COMPLETE: {message}"));
                else
                    LogMessage($"COMPLETE: {message}");
            };
        }
        private void SetupEventHandlers()
        {
            // Wire up the resource management event handlers
            AddResource.Click += AddResource_Click;
            RemoveResource.Click += RemoveResource_Click;
            StartConversion.Click += StartConversion_Click; // Add this line

            // Allow Enter key to add resource when in ResourceName textbox
            ResourceName.KeyDown += ResourceName_KeyDown;

            // Update the label text for resources
            label2.Text = "Resource Names:";
        }

        private async Task SaveResourcesAutomatically()
        {
            var resources = GetBlockedResources();
            if (resources.Any())
            {
                await clipProcessor.SaveBlockedResourcesAsync(resources);
            }
        }

        // Change return type from void to Task
        private async Task LoadSavedResources()
        {
            try
            {
                SentrySdk.AddBreadcrumb("Loading saved resources", "data", level: Sentry.BreadcrumbLevel.Info);
                
                var savedResources = await clipProcessor.LoadBlockedResourcesAsync();

                ResourceNameListbox.Items.Clear();
                foreach (var resource in savedResources)
                {
                    ResourceNameListbox.Items.Add(resource);
                }

                UpdateResourceCountLabel();

                if (savedResources.Any())
                {
                    SentrySdk.AddBreadcrumb($"Loaded {savedResources.Count} saved resources", "data", level: Sentry.BreadcrumbLevel.Info);
                    LogMessage($"Loaded {savedResources.Count} saved blocked resources");
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex, scope =>
                {
                    scope.SetTag("operation", "load-saved-resources");
                });
                LogMessage($"Error loading saved resources: {ex.Message}");
            }
        }

        private async void clip2load_Load(object sender, EventArgs e)
        {
            var transaction = SentrySdk.StartTransaction("app-load", "ui.load");
            SentrySdk.ConfigureScope(scope => scope.Transaction = transaction);
            
            try
            {
                // Log application startup
                SentrySdk.AddBreadcrumb("Application started", "app.lifecycle", level: Sentry.BreadcrumbLevel.Info);
                LogMessage("Application started - clip2load v1.0.0");

                // Log clips folder status
                var folderSpan = transaction.StartChild("check-clips-folder");
                LogClipsFolderStatus();
                folderSpan.Finish();

                UpdateClipsPathLabel();
                
                var loadSpan = transaction.StartChild("load-clip-files");
                LoadClipFiles();
                loadSpan.Finish();
                
                UpdateResourceCountLabel();

                // Load saved resources
                var resourcesSpan = transaction.StartChild("load-saved-resources");
                await LoadSavedResources();
                resourcesSpan.Finish();

                UpdateResourceCountLabel();
                
                transaction.Finish(SpanStatus.Ok);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                transaction.Finish(SpanStatus.InternalError);
                LogMessage($"Error during application load: {ex.Message}");
            }
        }

        private void LogClipsFolderStatus()
        {
            if (Directory.Exists(clipsFolder))
            {
                LogMessage($"Clips folder found: {clipsFolder}");

                try
                {
                    var clipCount = Directory.GetFiles(clipsFolder, "*.clip").Length;
                    LogMessage($"Found {clipCount} clip files in folder");
                }
                catch (Exception ex)
                {
                    LogMessage($"Error reading clips folder: {ex.Message}");
                }
            }
            else
            {
                LogMessage($"Clips folder not found: {clipsFolder}");
            }
        }

        private void UpdateClipsPathLabel()
        {
            lblClipsPath.Text = $"Clips folder: {clipsFolder}";
        }

        private void LoadClipFiles()
        {
            var transaction = SentrySdk.GetSpan() ?? SentrySdk.StartTransaction("load-clips", "file.load");
            var span = transaction.StartChild("load-clip-files", "Load all clip files from folder");
            
            try
            {
                lstAllClips.Items.Clear();

                if (!Directory.Exists(clipsFolder))
                {
                    SentrySdk.AddBreadcrumb($"Clips folder not found: {clipsFolder}", "file", level: Sentry.BreadcrumbLevel.Warning);
                    MessageBox.Show($"Clips folder not found: {clipsFolder}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    span.Finish(SpanStatus.NotFound);
                    return;
                }

                // Get all .clip files
                var clipFiles = Directory.GetFiles(clipsFolder, "*.clip")
                    .Select(Path.GetFileName)
                    .OrderBy(name => name)
                    .ToArray();

                lstAllClips.Items.AddRange(clipFiles);

                // Update status
                lblAllClips.Text = $"All Clips ({clipFiles.Length}):";

                // Log the loaded clips
                SentrySdk.AddBreadcrumb($"Loaded {clipFiles.Length} clip files", "file", level: Sentry.BreadcrumbLevel.Info);
                LogMessage($"Loaded {clipFiles.Length} clip files");
                
                span.SetExtra("clip_count", clipFiles.Length);
                span.Finish(SpanStatus.Ok);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex, scope =>
                {
                    scope.SetTag("operation", "load-clips");
                    scope.SetExtra("clips_folder", clipsFolder);
                });
                
                MessageBox.Show($"Error loading clip files: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogMessage($"Error loading clip files: {ex.Message}");
                span.Finish(SpanStatus.InternalError);
            }
        }

        private async void StartConversion_Click(object sender, EventArgs e)
        {
            var transaction = SentrySdk.StartTransaction("clip-conversion", "process");
            SentrySdk.ConfigureScope(scope => scope.Transaction = transaction);
            
            var selectedClips = GetSelectedClipPaths();
            var blockedResources = GetBlockedResources();

            transaction.SetExtra("selected_clips_count", selectedClips.Count);
            transaction.SetExtra("blocked_resources_count", blockedResources.Count);

            if (!selectedClips.Any())
            {
                SentrySdk.AddBreadcrumb("No clips selected for processing", "user", level: Sentry.BreadcrumbLevel.Warning);
                MessageBox.Show("Please select clips to process.", "No Clips Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                transaction.Finish(SpanStatus.InvalidArgument);
                return;
            }

            if (!blockedResources.Any())
            {
                SentrySdk.AddBreadcrumb("No blocked resources specified", "user", level: Sentry.BreadcrumbLevel.Warning);
                MessageBox.Show("Please add blocked resources to process.", "No Resources Specified",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                transaction.Finish(SpanStatus.InvalidArgument);
                return;
            }

            // Disable UI during processing
            StartConversion.Enabled = false;
            StartConversion.Text = "Processing...";

            try
            {
                SentrySdk.AddBreadcrumb("Starting clip conversion process", "process", level: Sentry.BreadcrumbLevel.Info);
                LogMessage("Starting clip conversion process...");

                var processingSpan = transaction.StartChild("process-clips-async");
                var options = new ProcessingClips.ProcessingOptions
                {
                    Mode = ProcessingClips.PatchMode.Null, // You can add UI controls to select this
                    Placeholder = "REMOVED",
                    CaseInsensitive = false // Case sensitive by default
                };
                var result = await clipProcessor.ProcessClipsAsync(
                    selectedClips,
                    blockedResources,
                    options
                );
                processingSpan.Finish();

                if (result.Success)
                {
                    SentrySdk.AddBreadcrumb("Processing completed successfully", "process", level: Sentry.BreadcrumbLevel.Info);
                    transaction.SetExtra("files_processed", result.ProcessedFiles);
                    transaction.SetExtra("files_patched", result.PatchedFiles);
                    transaction.SetExtra("total_patches", result.TotalPatches);
                    
                    MessageBox.Show($"Processing completed successfully!\n\n" +
                        $"Files processed: {result.ProcessedFiles}\n" +
                        $"Files patched: {result.PatchedFiles}\n" +
                        $"Total patches: {result.TotalPatches}\n" +
                        $"Backups saved to: {result.BackupDirectory}",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    transaction.Finish(SpanStatus.Ok);
                }
                else
                {
                    SentrySdk.CaptureMessage($"Processing failed: {result.ErrorMessage}", Sentry.SentryLevel.Error);
                    transaction.SetExtra("error_message", result.ErrorMessage);
                    
                    MessageBox.Show($"Processing failed: {result.ErrorMessage}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                    transaction.Finish(SpanStatus.UnknownError);
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex, scope =>
                {
                    scope.SetTag("operation", "clip-conversion");
                    scope.SetExtra("selected_clips_count", selectedClips.Count);
                    scope.SetExtra("blocked_resources_count", blockedResources.Count);
                });
                
                LogMessage($"Processing error: {ex.Message}");
                MessageBox.Show($"An error occurred during processing: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                transaction.Finish(SpanStatus.InternalError);
            }
            finally
            {
                // Re-enable UI
                StartConversion.Enabled = true;
                StartConversion.Text = "Start Conversion Process";
            }
        }

        private void btnBrowseFolder_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select GTA V Clips Folder";
                folderDialog.SelectedPath = clipsFolder;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string oldFolder = clipsFolder;
                    clipsFolder = folderDialog.SelectedPath;

                    SentrySdk.AddBreadcrumb($"Clips folder changed to: {clipsFolder}", "user", level: Sentry.BreadcrumbLevel.Info);
                    
                    LogMessage($"Clips folder changed from: {oldFolder}");
                    LogMessage($"Clips folder changed to: {clipsFolder}");

                    UpdateClipsPathLabel();
                    LoadClipFiles();
                }
            }
        }

        private void btnMoveToSelected_Click(object sender, EventArgs e)
        {
            var selectedItems = lstAllClips.SelectedItems.Cast<string>().ToList();

            int addedCount = 0;
            foreach (var item in selectedItems)
            {
                if (!lstSelectedClips.Items.Contains(item))
                {
                    lstSelectedClips.Items.Add(item);
                    addedCount++;
                    LogMessage($"Added clip to processing list: {item}");
                }
            }

            if (addedCount > 0)
            {
                LogMessage($"Added {addedCount} clip(s) to processing list");
            }
            else if (selectedItems.Count > 0)
            {
                LogMessage("No clips added - all selected clips already in processing list");
            }

            UpdateSelectedClipsLabel();
        }

        private void btnRemoveFromSelected_Click(object sender, EventArgs e)
        {
            var selectedItems = lstSelectedClips.SelectedItems.Cast<string>().ToList();

            if (selectedItems.Count > 0)
            {
                foreach (var item in selectedItems)
                {
                    lstSelectedClips.Items.Remove(item);
                    LogMessage($"Removed clip from processing list: {item}");
                }

                LogMessage($"Removed {selectedItems.Count} clip(s) from processing list");
            }

            UpdateSelectedClipsLabel();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LogMessage("Refreshing clip files list");
            LoadClipFiles();
        }

        private void UpdateSelectedClipsLabel()
        {
            lblSelectedClips.Text = $"Clips to Process ({lstSelectedClips.Items.Count}):";
        }

        // Resource Management Event Handlers
        private void AddResource_Click(object sender, EventArgs e)
        {
            AddResourceToList();
        }

        private void RemoveResource_Click(object sender, EventArgs e)
        {
            RemoveSelectedResources();
        }

        private void ResourceName_KeyDown(object sender, KeyEventArgs e)
        {
            // Allow adding resource by pressing Enter
            if (e.KeyCode == Keys.Enter)
            {
                AddResourceToList();
                e.Handled = true; // Prevent the "ding" sound
            }
        }

        private async void AddResourceToList()
        {
            string resourceName = ResourceName.Text.Trim();

            // Validate input
            if (string.IsNullOrWhiteSpace(resourceName))
            {
                SentrySdk.AddBreadcrumb("Empty resource name entered", "user", level: Sentry.BreadcrumbLevel.Warning);
                MessageBox.Show("Please enter a resource name.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ResourceName.Focus();
                return;
            }

            // Check for duplicates
            if (ResourceNameListbox.Items.Cast<string>().Any(item =>
                string.Equals(item, resourceName, StringComparison.OrdinalIgnoreCase)))
            {
                SentrySdk.AddBreadcrumb($"Duplicate resource attempted: {resourceName}", "user", level: Sentry.BreadcrumbLevel.Info);
                MessageBox.Show($"Resource '{resourceName}' already exists in the list.", "Duplicate Resource",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResourceName.Focus();
                ResourceName.SelectAll();
                return;
            }

            try
            {
                // Add to listbox
                ResourceNameListbox.Items.Add(resourceName);

                // Clear textbox and update count
                ResourceName.Clear();
                ResourceName.Focus();
                UpdateResourceCountLabel();

                // Log the addition
                SentrySdk.AddBreadcrumb($"Resource added: {resourceName}", "user", level: Sentry.BreadcrumbLevel.Info);
                LogMessage($"Added resource: {resourceName}");

                // Auto-save resources
                await SaveResourcesAutomatically();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex, scope =>
                {
                    scope.SetTag("operation", "add-resource");
                    scope.SetExtra("resource_name", resourceName);
                });
                LogMessage($"Error adding resource: {ex.Message}");
            }
        }

        private async void RemoveSelectedResources()
        {
            var selectedItems = ResourceNameListbox.SelectedItems.Cast<string>().ToList();

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Please select one or more resources to remove.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Confirm removal if multiple items selected
            if (selectedItems.Count > 1)
            {
                var result = MessageBox.Show($"Are you sure you want to remove {selectedItems.Count} resources?",
                    "Confirm Removal", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                    return;
            }

            // Remove selected items
            foreach (var item in selectedItems)
            {
                ResourceNameListbox.Items.Remove(item);
                LogMessage($"Removed resource: {item}");
            }

            UpdateResourceCountLabel();

            // Auto-save resources
            await SaveResourcesAutomatically();
        }

        private void UpdateResourceCountLabel()
        {
            label2.Text = $"Resource Names ({ResourceNameListbox.Items.Count}):";
        }

        private void LogMessage(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            loggingBox1.AppendText($"[{timestamp}] {message}{Environment.NewLine}");

            // Auto-scroll to bottom
            loggingBox1.SelectionStart = loggingBox1.Text.Length;
            loggingBox1.ScrollToCaret();
        }

        // Property to get selected clips for processing
        public List<string> GetSelectedClips()
        {
            return lstSelectedClips.Items.Cast<string>().ToList();
        }

        // Property to get full paths of selected clips
        public List<string> GetSelectedClipPaths()
        {
            return lstSelectedClips.Items.Cast<string>()
                .Select(filename => Path.Combine(clipsFolder, filename))
                .ToList();
        }

        // Property to get blocked resources
        public List<string> GetBlockedResources()
        {
            return ResourceNameListbox.Items.Cast<string>().ToList();
        }

        private void lblAllClips_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void lstAllClips_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }
    }
}

