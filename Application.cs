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
                var savedResources = await clipProcessor.LoadBlockedResourcesAsync();

                ResourceNameListbox.Items.Clear();
                foreach (var resource in savedResources)
                {
                    ResourceNameListbox.Items.Add(resource);
                }

                UpdateResourceCountLabel();

                if (savedResources.Any())
                {
                    LogMessage($"Loaded {savedResources.Count} saved blocked resources");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error loading saved resources: {ex.Message}");
            }
        }

        private async void clip2load_Load(object sender, EventArgs e)
        {
            // Log application startup
            LogMessage("Application started - clip2load v1.0.0");

            // Log clips folder status
            LogClipsFolderStatus();

            UpdateClipsPathLabel();
            LoadClipFiles();
            UpdateResourceCountLabel();

            // Load saved resources
            await LoadSavedResources();

            UpdateResourceCountLabel();
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
            lstAllClips.Items.Clear();

            if (!Directory.Exists(clipsFolder))
            {
                MessageBox.Show($"Clips folder not found: {clipsFolder}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Get all .clip files
                var clipFiles = Directory.GetFiles(clipsFolder, "*.clip")
                    .Select(Path.GetFileName)
                    .OrderBy(name => name)
                    .ToArray();

                lstAllClips.Items.AddRange(clipFiles);

                // Update status
                lblAllClips.Text = $"All Clips ({clipFiles.Length}):";

                // Log the loaded clips
                LogMessage($"Loaded {clipFiles.Length} clip files");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading clip files: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogMessage($"Error loading clip files: {ex.Message}");
            }
        }

        private async void StartConversion_Click(object sender, EventArgs e)
        {
            var selectedClips = GetSelectedClipPaths();
            var blockedResources = GetBlockedResources();

            if (!selectedClips.Any())
            {
                MessageBox.Show("Please select clips to process.", "No Clips Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!blockedResources.Any())
            {
                MessageBox.Show("Please add blocked resources to process.", "No Resources Specified",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Disable UI during processing
            StartConversion.Enabled = false;
            StartConversion.Text = "Processing...";

            try
            {
                LogMessage("Starting clip conversion process...");

                var result = await clipProcessor.ProcessClipsAsync(
                    selectedClips,
                    blockedResources,
                    ProcessingClips.PatchMode.Null, // You can add UI controls to select this
                    "REMOVED",
                    false // Case sensitive by default
                );

                if (result.Success)
                {
                    MessageBox.Show($"Processing completed successfully!\n\n" +
                        $"Files processed: {result.ProcessedFiles}\n" +
                        $"Files patched: {result.PatchedFiles}\n" +
                        $"Total patches: {result.TotalPatches}\n" +
                        $"Backups saved to: {result.BackupDirectory}",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Processing failed: {result.ErrorMessage}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Processing error: {ex.Message}");
                MessageBox.Show($"An error occurred during processing: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Please enter a resource name.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ResourceName.Focus();
                return;
            }

            // Check for duplicates
            if (ResourceNameListbox.Items.Cast<string>().Any(item =>
                string.Equals(item, resourceName, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show($"Resource '{resourceName}' already exists in the list.", "Duplicate Resource",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResourceName.Focus();
                ResourceName.SelectAll();
                return;
            }

            // Add to listbox
            ResourceNameListbox.Items.Add(resourceName);

            // Clear textbox and update count
            ResourceName.Clear();
            ResourceName.Focus();
            UpdateResourceCountLabel();

            // Log the addition
            LogMessage($"Added resource: {resourceName}");

            // Auto-save resources
            await SaveResourcesAutomatically();
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

