using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace clip2load
{
    partial class clip2load
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lstAllClips = new ListBox();
            lstSelectedClips = new ListBox();
            btnMoveToSelected = new Button();
            btnRemoveFromSelected = new Button();
            btnBrowseFolder = new Button();
            lblClipsPath = new Label();
            lblAllClips = new Label();
            lblSelectedClips = new Label();
            btnRefresh = new Button();
            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            loggingBox1 = new TextBox();
            label1 = new Label();
            StartConversion = new Button();
            groupBox3 = new GroupBox();
            label3 = new Label();
            ResourceName = new TextBox();
            AddResource = new Button();
            RemoveResource = new Button();
            label2 = new Label();
            ResourceNameListbox = new ListBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // lstAllClips
            // 
            lstAllClips.FormattingEnabled = true;
            lstAllClips.ItemHeight = 15;
            lstAllClips.Location = new Point(15, 37);
            lstAllClips.Name = "lstAllClips";
            lstAllClips.SelectionMode = SelectionMode.MultiExtended;
            lstAllClips.Size = new Size(254, 184);
            lstAllClips.TabIndex = 0;
            lstAllClips.SelectedIndexChanged += lstAllClips_SelectedIndexChanged;
            // 
            // lstSelectedClips
            // 
            lstSelectedClips.FormattingEnabled = true;
            lstSelectedClips.ItemHeight = 15;
            lstSelectedClips.Location = new Point(329, 37);
            lstSelectedClips.Name = "lstSelectedClips";
            lstSelectedClips.SelectionMode = SelectionMode.MultiExtended;
            lstSelectedClips.Size = new Size(254, 184);
            lstSelectedClips.TabIndex = 1;
            // 
            // btnMoveToSelected
            // 
            btnMoveToSelected.Location = new Point(275, 37);
            btnMoveToSelected.Name = "btnMoveToSelected";
            btnMoveToSelected.Size = new Size(48, 30);
            btnMoveToSelected.TabIndex = 2;
            btnMoveToSelected.Text = "→";
            btnMoveToSelected.UseVisualStyleBackColor = true;
            btnMoveToSelected.Click += btnMoveToSelected_Click;
            // 
            // btnRemoveFromSelected
            // 
            btnRemoveFromSelected.Location = new Point(275, 73);
            btnRemoveFromSelected.Name = "btnRemoveFromSelected";
            btnRemoveFromSelected.Size = new Size(48, 30);
            btnRemoveFromSelected.TabIndex = 3;
            btnRemoveFromSelected.Text = "←";
            btnRemoveFromSelected.UseVisualStyleBackColor = true;
            btnRemoveFromSelected.Click += btnRemoveFromSelected_Click;
            // 
            // btnBrowseFolder
            // 
            btnBrowseFolder.Location = new Point(15, 227);
            btnBrowseFolder.Name = "btnBrowseFolder";
            btnBrowseFolder.Size = new Size(137, 24);
            btnBrowseFolder.TabIndex = 4;
            btnBrowseFolder.Text = "Browse Folder";
            btnBrowseFolder.UseVisualStyleBackColor = true;
            btnBrowseFolder.Click += btnBrowseFolder_Click;
            // 
            // lblClipsPath
            // 
            lblClipsPath.AutoSize = true;
            lblClipsPath.Location = new Point(15, 254);
            lblClipsPath.Name = "lblClipsPath";
            lblClipsPath.Size = new Size(70, 15);
            lblClipsPath.TabIndex = 5;
            lblClipsPath.Text = "Clips folder:";
            // 
            // lblAllClips
            // 
            lblAllClips.AutoSize = true;
            lblAllClips.Location = new Point(15, 19);
            lblAllClips.Name = "lblAllClips";
            lblAllClips.Size = new Size(53, 15);
            lblAllClips.TabIndex = 6;
            lblAllClips.Text = "All Clips:";
            lblAllClips.Click += lblAllClips_Click;
            // 
            // lblSelectedClips
            // 
            lblSelectedClips.AutoSize = true;
            lblSelectedClips.Location = new Point(329, 18);
            lblSelectedClips.Name = "lblSelectedClips";
            lblSelectedClips.Size = new Size(93, 15);
            lblSelectedClips.TabIndex = 7;
            lblSelectedClips.Text = "Clips to Process:";
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(158, 227);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(111, 24);
            btnRefresh.TabIndex = 8;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(lblAllClips);
            groupBox1.Controls.Add(btnRefresh);
            groupBox1.Controls.Add(lblClipsPath);
            groupBox1.Controls.Add(btnBrowseFolder);
            groupBox1.Controls.Add(lstAllClips);
            groupBox1.Controls.Add(lblSelectedClips);
            groupBox1.Controls.Add(lstSelectedClips);
            groupBox1.Controls.Add(btnRemoveFromSelected);
            groupBox1.Controls.Add(btnMoveToSelected);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(594, 281);
            groupBox1.TabIndex = 9;
            groupBox1.TabStop = false;
            groupBox1.Text = "Clip Processing List";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(loggingBox1);
            groupBox2.Controls.Add(label1);
            groupBox2.Location = new Point(612, 12);
            groupBox2.Name = "groupBox2";
            groupBox2.RightToLeft = RightToLeft.Yes;
            groupBox2.Size = new Size(478, 547);
            groupBox2.TabIndex = 10;
            groupBox2.TabStop = false;
            groupBox2.Text = "build clip2load@1.0.0 | developed by: github.com/Avenze";
            groupBox2.Enter += groupBox2_Enter;
            // 
            // loggingBox1
            // 
            loggingBox1.BackColor = SystemColors.Control;
            loggingBox1.BorderStyle = BorderStyle.None;
            loggingBox1.Location = new Point(6, 22);
            loggingBox1.Multiline = true;
            loggingBox1.Name = "loggingBox1";
            loggingBox1.ReadOnly = true;
            loggingBox1.RightToLeft = RightToLeft.No;
            loggingBox1.ScrollBars = ScrollBars.Vertical;
            loggingBox1.Size = new Size(466, 519);
            loggingBox1.TabIndex = 10;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(15, 0);
            label1.Name = "label1";
            label1.RightToLeft = RightToLeft.No;
            label1.Size = new Size(51, 15);
            label1.TabIndex = 9;
            label1.Text = "Logging";
            // 
            // StartConversion
            // 
            StartConversion.Location = new Point(12, 535);
            StartConversion.Name = "StartConversion";
            StartConversion.Size = new Size(186, 24);
            StartConversion.TabIndex = 9;
            StartConversion.Text = "Start Conversion Process";
            StartConversion.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(label3);
            groupBox3.Controls.Add(ResourceName);
            groupBox3.Controls.Add(AddResource);
            groupBox3.Controls.Add(RemoveResource);
            groupBox3.Controls.Add(label2);
            groupBox3.Controls.Add(ResourceNameListbox);
            groupBox3.Location = new Point(12, 299);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(594, 230);
            groupBox3.TabIndex = 11;
            groupBox3.TabStop = false;
            groupBox3.Text = "Blocked Resources";
            groupBox3.Enter += groupBox3_Enter;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(15, 182);
            label3.Name = "label3";
            label3.Size = new Size(93, 15);
            label3.TabIndex = 12;
            label3.Text = "Resource Name:";
            // 
            // ResourceName
            // 
            ResourceName.Location = new Point(15, 200);
            ResourceName.Name = "ResourceName";
            ResourceName.Size = new Size(282, 23);
            ResourceName.TabIndex = 11;
            // 
            // AddResource
            // 
            AddResource.Location = new Point(303, 200);
            AddResource.Name = "AddResource";
            AddResource.Size = new Size(137, 23);
            AddResource.TabIndex = 10;
            AddResource.Text = "Add Resource";
            AddResource.UseVisualStyleBackColor = true;
            // 
            // RemoveResource
            // 
            RemoveResource.Location = new Point(446, 201);
            RemoveResource.Name = "RemoveResource";
            RemoveResource.Size = new Size(137, 23);
            RemoveResource.TabIndex = 9;
            RemoveResource.Text = "Remove Resource";
            RemoveResource.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(15, 19);
            label2.Name = "label2";
            label2.Size = new Size(98, 15);
            label2.TabIndex = 9;
            label2.Text = "Resource Names:";
            // 
            // ResourceNameListbox
            // 
            ResourceNameListbox.FormattingEnabled = true;
            ResourceNameListbox.ItemHeight = 15;
            ResourceNameListbox.Location = new Point(15, 37);
            ResourceNameListbox.Name = "ResourceNameListbox";
            ResourceNameListbox.SelectionMode = SelectionMode.MultiExtended;
            ResourceNameListbox.Size = new Size(568, 139);
            ResourceNameListbox.TabIndex = 9;
            // 
            // clip2load
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1102, 571);
            Controls.Add(groupBox3);
            Controls.Add(StartConversion);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Name = "clip2load";
            Text = "clip2load | 1.0.0-patch1 | github.com/Avenze/clip2load-repository";
            Load += clip2load_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ListBox lstAllClips;
        private ListBox lstSelectedClips;
        private Button btnMoveToSelected;
        private Button btnRemoveFromSelected;
        private Button btnBrowseFolder;
        private Label lblClipsPath;
        private Label lblAllClips;
        private Label lblSelectedClips;
        private Button btnRefresh;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Label label1;
        private TextBox loggingBox1;
        private Button StartConversion;
        private GroupBox groupBox3;
        private Label label3;
        private TextBox ResourceName;
        private Button AddResource;
        private Button RemoveResource;
        private Label label2;
        private ListBox ResourceNameListbox;
    }
}
