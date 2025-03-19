using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using AltUI.Controls;
using AltUI.Forms;

namespace VBLauncher
{
    public partial class ModLoader : DarkForm
    {

        // Form overrides dispose to clean up the component list.
        [DebuggerNonUserCode()]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components is not null)
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        // Required by the Windows Form Designer
        private IContainer components;

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.  
        // Do not modify it using the code editor.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            components = new Container();
            var resources = new ComponentResourceManager(typeof(ModLoader));
            DarkToolStrip1 = new DarkToolStrip();
            ToolStripButton1 = new ToolStripButton();
            ToolStripDropDownButton1 = new ToolStripDropDownButton();
            AddModToolStripMenuItem = new ToolStripMenuItem();
            EnableAllToolStripMenuItem = new ToolStripMenuItem();
            EnableAllToolStripMenuItem.Click += new EventHandler(EnableAllToolStripMenuItem_Click);
            DisableAllToolStripMenuItem = new ToolStripMenuItem();
            DisableAllToolStripMenuItem.Click += new EventHandler(DisableAllToolStripMenuItem_Click);
            ToolStripDropDownButton2 = new ToolStripDropDownButton();
            OpenGameFolderToolStripMenuItem = new ToolStripMenuItem();
            OpenModsFolderToolStripMenuItem = new ToolStripMenuItem();
            ToolStripLabel1 = new ToolStripLabel();
            ListView1 = new DoubleBufferedListView();
            ListView1.ItemDrag += new ItemDragEventHandler(ListView1_ItemDrag);
            ListView1.DragEnter += new DragEventHandler(ListView1_DragEnter);
            ListView1.DragLeave += new EventHandler(ListView1_DragLeave);
            ListView1.DragOver += new DragEventHandler(ListView1_DragOver);
            ListView1.DragDrop += new DragEventHandler(ListView1_DragDrop);
            ListView1.DrawColumnHeader += new DrawListViewColumnHeaderEventHandler(ListView1_DrawColumnHeader);
            ListView1.DrawSubItem += new DrawListViewSubItemEventHandler(ListView1_DrawSubItem);
            ListView1.SelectedIndexChanged += new EventHandler(ListView1_SelectedIndexChanged);
            ListView1.ItemChecked += new ItemCheckedEventHandler(UpdateStatus);
            ModName = new ColumnHeader();
            Conflicts = new ColumnHeader();
            Priority = new ColumnHeader();
            DarkToolStrip2 = new DarkToolStrip();
            DarkToolStrip2.Paint += new PaintEventHandler(DarkToolStrip2_Paint);
            ToolStripLabel2 = new ToolStripLabel();
            ToolStripLabel2.MouseEnter += new EventHandler(ToolStripLabel_MouseEnter);
            ToolStripLabel2.MouseLeave += new EventHandler(ToolStripLabel_MouseLeave);
            ToolStripLabel2.Click += new EventHandler(DeployMods);
            ToolStripSeparator1 = new ToolStripSeparator();
            ToolStripLabel3 = new ToolStripLabel();
            ToolStripLabel3.MouseEnter += new EventHandler(ToolStripLabel_MouseEnter);
            ToolStripLabel3.MouseLeave += new EventHandler(ToolStripLabel_MouseLeave);
            ToolStripLabel3.Click += new EventHandler(ToolStripLabel3_Click);
            ToolStripSeparator2 = new ToolStripSeparator();
            TableLayoutPanel1 = new TableLayoutPanel();
            TableLayoutPanel1.Paint += new PaintEventHandler(TableLayoutPanel1_Paint);
            DarkLabel3 = new DarkLabel();
            DarkLabel1 = new DarkLabel();
            DarkLabel2 = new DarkLabel();
            DarkRichTextBox1 = new DarkRichTextBox();
            Timer1 = new Timer(components);
            Timer1.Tick += new EventHandler(Timer1_Tick);
            DarkToolStrip1.SuspendLayout();
            DarkToolStrip2.SuspendLayout();
            TableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // DarkToolStrip1
            // 
            DarkToolStrip1.AutoSize = false;
            DarkToolStrip1.BackColor = Color.FromArgb(16, 16, 17);
            DarkToolStrip1.ForeColor = Color.FromArgb(213, 213, 213);
            DarkToolStrip1.Items.AddRange(new ToolStripItem[] { ToolStripButton1, ToolStripDropDownButton1, ToolStripDropDownButton2, ToolStripLabel1 });
            DarkToolStrip1.Location = new Point(0, 0);
            DarkToolStrip1.Name = "DarkToolStrip1";
            DarkToolStrip1.Padding = new Padding(5, 0, 1, 0);
            DarkToolStrip1.Size = new Size(800, 28);
            DarkToolStrip1.TabIndex = 2;
            DarkToolStrip1.Text = "DarkToolStrip1";
            // 
            // ToolStripButton1
            // 
            ToolStripButton1.BackColor = Color.FromArgb(16, 16, 17);
            ToolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ToolStripButton1.ForeColor = Color.FromArgb(213, 213, 213);
            ToolStripButton1.Image = (Image)resources.GetObject("ToolStripButton1.Image");
            ToolStripButton1.ImageTransparentColor = Color.Magenta;
            ToolStripButton1.Name = "ToolStripButton1";
            ToolStripButton1.Size = new Size(23, 25);
            ToolStripButton1.Text = "ToolStripButton1";
            ToolStripButton1.ToolTipText = "Refresh";
            // 
            // ToolStripDropDownButton1
            // 
            ToolStripDropDownButton1.BackColor = Color.FromArgb(16, 16, 17);
            ToolStripDropDownButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ToolStripDropDownButton1.DropDownItems.AddRange(new ToolStripItem[] { AddModToolStripMenuItem, EnableAllToolStripMenuItem, DisableAllToolStripMenuItem });
            ToolStripDropDownButton1.ForeColor = Color.FromArgb(213, 213, 213);
            ToolStripDropDownButton1.Image = (Image)resources.GetObject("ToolStripDropDownButton1.Image");
            ToolStripDropDownButton1.ImageTransparentColor = Color.Magenta;
            ToolStripDropDownButton1.Name = "ToolStripDropDownButton1";
            ToolStripDropDownButton1.Size = new Size(29, 25);
            ToolStripDropDownButton1.Text = "ToolStripDropDownButton1";
            ToolStripDropDownButton1.ToolTipText = "List Options";
            // 
            // AddModToolStripMenuItem
            // 
            AddModToolStripMenuItem.BackColor = Color.FromArgb(16, 16, 17);
            AddModToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            AddModToolStripMenuItem.Name = "AddModToolStripMenuItem";
            AddModToolStripMenuItem.Size = new Size(129, 22);
            AddModToolStripMenuItem.Text = "Add Mod";
            // 
            // EnableAllToolStripMenuItem
            // 
            EnableAllToolStripMenuItem.BackColor = Color.FromArgb(16, 16, 17);
            EnableAllToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            EnableAllToolStripMenuItem.Name = "EnableAllToolStripMenuItem";
            EnableAllToolStripMenuItem.Size = new Size(129, 22);
            EnableAllToolStripMenuItem.Text = "Enable All";
            // 
            // DisableAllToolStripMenuItem
            // 
            DisableAllToolStripMenuItem.BackColor = Color.FromArgb(16, 16, 17);
            DisableAllToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            DisableAllToolStripMenuItem.Name = "DisableAllToolStripMenuItem";
            DisableAllToolStripMenuItem.Size = new Size(129, 22);
            DisableAllToolStripMenuItem.Text = "Disable All";
            // 
            // ToolStripDropDownButton2
            // 
            ToolStripDropDownButton2.BackColor = Color.FromArgb(16, 16, 17);
            ToolStripDropDownButton2.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ToolStripDropDownButton2.DropDownItems.AddRange(new ToolStripItem[] { OpenGameFolderToolStripMenuItem, OpenModsFolderToolStripMenuItem });
            ToolStripDropDownButton2.ForeColor = Color.FromArgb(213, 213, 213);
            ToolStripDropDownButton2.Image = (Image)resources.GetObject("ToolStripDropDownButton2.Image");
            ToolStripDropDownButton2.ImageTransparentColor = Color.Magenta;
            ToolStripDropDownButton2.Name = "ToolStripDropDownButton2";
            ToolStripDropDownButton2.Size = new Size(29, 25);
            ToolStripDropDownButton2.Text = "ToolStripDropDownButton2";
            ToolStripDropDownButton2.ToolTipText = "Folder Menu";
            // 
            // OpenGameFolderToolStripMenuItem
            // 
            OpenGameFolderToolStripMenuItem.BackColor = Color.FromArgb(16, 16, 17);
            OpenGameFolderToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            OpenGameFolderToolStripMenuItem.Name = "OpenGameFolderToolStripMenuItem";
            OpenGameFolderToolStripMenuItem.Size = new Size(173, 22);
            OpenGameFolderToolStripMenuItem.Text = "Open Game Folder";
            // 
            // OpenModsFolderToolStripMenuItem
            // 
            OpenModsFolderToolStripMenuItem.BackColor = Color.FromArgb(16, 16, 17);
            OpenModsFolderToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            OpenModsFolderToolStripMenuItem.Name = "OpenModsFolderToolStripMenuItem";
            OpenModsFolderToolStripMenuItem.Size = new Size(173, 22);
            OpenModsFolderToolStripMenuItem.Text = "Open Mods Folder";
            // 
            // ToolStripLabel1
            // 
            ToolStripLabel1.Alignment = ToolStripItemAlignment.Right;
            ToolStripLabel1.BackColor = Color.FromArgb(16, 16, 17);
            ToolStripLabel1.ForeColor = Color.FromArgb(213, 213, 213);
            ToolStripLabel1.Name = "ToolStripLabel1";
            ToolStripLabel1.Size = new Size(52, 25);
            ToolStripLabel1.Text = "Active: 0";
            // 
            // ListView1
            // 
            ListView1.AllowDrop = true;
            ListView1.BackColor = Color.FromArgb(32, 32, 32);
            ListView1.BorderStyle = BorderStyle.None;
            ListView1.CheckBoxes = true;
            ListView1.Columns.AddRange(new ColumnHeader[] { ModName, Conflicts, Priority });
            ListView1.Dock = DockStyle.Fill;
            ListView1.FullRowSelect = true;
            ListView1.HideSelection = true;
            ListView1.Location = new Point(0, 28);
            ListView1.MultiSelect = false;
            ListView1.Name = "ListView1";
            ListView1.OwnerDraw = true;
            ListView1.Size = new Size(600, 397);
            ListView1.TabIndex = 3;
            ListView1.UseCompatibleStateImageBehavior = false;
            ListView1.View = View.Details;
            // 
            // ModName
            // 
            ModName.Text = "Mod Name";
            ModName.Width = 300;
            // 
            // Conflicts
            // 
            Conflicts.Text = "Conflicts";
            Conflicts.Width = 150;
            // 
            // Priority
            // 
            Priority.Text = "Priority";
            Priority.Width = 150;
            // 
            // DarkToolStrip2
            // 
            DarkToolStrip2.AutoSize = false;
            DarkToolStrip2.BackColor = Color.FromArgb(16, 16, 17);
            DarkToolStrip2.Dock = DockStyle.Bottom;
            DarkToolStrip2.ForeColor = Color.FromArgb(213, 213, 213);
            DarkToolStrip2.GripStyle = ToolStripGripStyle.Hidden;
            DarkToolStrip2.Items.AddRange(new ToolStripItem[] { ToolStripLabel2, ToolStripSeparator1, ToolStripLabel3, ToolStripSeparator2 });
            DarkToolStrip2.Location = new Point(0, 425);
            DarkToolStrip2.Name = "DarkToolStrip2";
            DarkToolStrip2.Padding = new Padding(5, 0, 1, 0);
            DarkToolStrip2.Size = new Size(800, 25);
            DarkToolStrip2.TabIndex = 4;
            DarkToolStrip2.Text = "DarkToolStrip2";
            // 
            // ToolStripLabel2
            // 
            ToolStripLabel2.Alignment = ToolStripItemAlignment.Right;
            ToolStripLabel2.BackColor = Color.FromArgb(16, 16, 17);
            ToolStripLabel2.ForeColor = Color.FromArgb(213, 213, 213);
            ToolStripLabel2.Image = (Image)resources.GetObject("ToolStripLabel2.Image");
            ToolStripLabel2.Name = "ToolStripLabel2";
            ToolStripLabel2.Size = new Size(96, 22);
            ToolStripLabel2.Text = " Deploy Mods";
            // 
            // ToolStripSeparator1
            // 
            ToolStripSeparator1.Alignment = ToolStripItemAlignment.Right;
            ToolStripSeparator1.BackColor = Color.FromArgb(16, 16, 17);
            ToolStripSeparator1.ForeColor = Color.FromArgb(213, 213, 213);
            ToolStripSeparator1.Margin = new Padding(0, 0, 2, 0);
            ToolStripSeparator1.Name = "ToolStripSeparator1";
            ToolStripSeparator1.Size = new Size(6, 25);
            // 
            // ToolStripLabel3
            // 
            ToolStripLabel3.Alignment = ToolStripItemAlignment.Right;
            ToolStripLabel3.BackColor = Color.FromArgb(16, 16, 17);
            ToolStripLabel3.ForeColor = Color.FromArgb(213, 213, 213);
            ToolStripLabel3.Image = (Image)resources.GetObject("ToolStripLabel3.Image");
            ToolStripLabel3.Name = "ToolStripLabel3";
            ToolStripLabel3.Size = new Size(45, 22);
            ToolStripLabel3.Text = " Exit";
            // 
            // ToolStripSeparator2
            // 
            ToolStripSeparator2.Alignment = ToolStripItemAlignment.Right;
            ToolStripSeparator2.BackColor = Color.FromArgb(16, 16, 17);
            ToolStripSeparator2.ForeColor = Color.FromArgb(213, 213, 213);
            ToolStripSeparator2.Margin = new Padding(0, 0, 2, 0);
            ToolStripSeparator2.Name = "ToolStripSeparator2";
            ToolStripSeparator2.Size = new Size(6, 25);
            // 
            // TableLayoutPanel1
            // 
            TableLayoutPanel1.ColumnCount = 1;
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100.0f));
            TableLayoutPanel1.Controls.Add(DarkLabel3, 0, 3);
            TableLayoutPanel1.Controls.Add(DarkLabel1, 0, 0);
            TableLayoutPanel1.Controls.Add(DarkLabel2, 0, 1);
            TableLayoutPanel1.Controls.Add(DarkRichTextBox1, 0, 2);
            TableLayoutPanel1.Dock = DockStyle.Right;
            TableLayoutPanel1.Location = new Point(600, 28);
            TableLayoutPanel1.Name = "TableLayoutPanel1";
            TableLayoutPanel1.Padding = new Padding(0, 0, 0, 6);
            TableLayoutPanel1.RowCount = 4;
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100.0f));
            TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20.0f));
            TableLayoutPanel1.Size = new Size(200, 397);
            TableLayoutPanel1.TabIndex = 5;
            // 
            // DarkLabel3
            // 
            DarkLabel3.AutoSize = true;
            DarkLabel3.Dock = DockStyle.Fill;
            DarkLabel3.Location = new Point(9, 377);
            DarkLabel3.Margin = new Padding(9, 6, 9, 0);
            DarkLabel3.Name = "DarkLabel3";
            DarkLabel3.Size = new Size(182, 14);
            DarkLabel3.TabIndex = 3;
            DarkLabel3.Text = "";
            // 
            // DarkLabel1
            // 
            DarkLabel1.AutoSize = true;
            DarkLabel1.Font = new Font("Segoe UI", 12.0f, FontStyle.Bold, GraphicsUnit.Point);
            DarkLabel1.Location = new Point(3, 2);
            DarkLabel1.Margin = new Padding(3, 2, 3, 0);
            DarkLabel1.Name = "DarkLabel1";
            DarkLabel1.Size = new Size(83, 21);
            DarkLabel1.TabIndex = 1;
            DarkLabel1.Text = "";
            // 
            // DarkLabel2
            // 
            DarkLabel2.AutoSize = true;
            DarkLabel2.Location = new Point(9, 29);
            DarkLabel2.Margin = new Padding(9, 6, 3, 0);
            DarkLabel2.Name = "DarkLabel2";
            DarkLabel2.Size = new Size(70, 15);
            DarkLabel2.TabIndex = 2;
            DarkLabel2.Text = "";
            // 
            // DarkRichTextBox1
            // 
            DarkRichTextBox1.BackColor = Color.FromArgb(16, 16, 17);
            DarkRichTextBox1.BorderStyle = BorderStyle.None;
            DarkRichTextBox1.Dock = DockStyle.Fill;
            DarkRichTextBox1.ForeColor = Color.FromArgb(213, 213, 213);
            DarkRichTextBox1.Location = new Point(12, 47);
            DarkRichTextBox1.Margin = new Padding(12, 3, 12, 12);
            DarkRichTextBox1.Name = "DarkRichTextBox1";
            DarkRichTextBox1.ReadOnly = true;
            DarkRichTextBox1.Size = new Size(176, 312);
            DarkRichTextBox1.TabIndex = 0;
            DarkRichTextBox1.Text = "";
            // 
            // Timer1
            // 
            Timer1.Interval = 30;
            // 
            // ModLoader
            // 
            AutoScaleDimensions = new SizeF(7f, 15f);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(ListView1);
            Controls.Add(TableLayoutPanel1);
            Controls.Add(DarkToolStrip2);
            Controls.Add(DarkToolStrip1);
            CornerStyle = CornerPreference.Default;
            Name = "ModLoader";
            Text = "ModLoader";
            TransparencyKey = Color.FromArgb(31, 31, 32);
            DarkToolStrip1.ResumeLayout(false);
            DarkToolStrip1.PerformLayout();
            DarkToolStrip2.ResumeLayout(false);
            DarkToolStrip2.PerformLayout();
            TableLayoutPanel1.ResumeLayout(false);
            TableLayoutPanel1.PerformLayout();
            Load += new EventHandler(Form1_Load);
            ResumeLayout(false);

        }
        internal DarkToolStrip DarkToolStrip1;
        internal ToolStripButton ToolStripButton1;
        internal DoubleBufferedListView ListView1;
        internal ToolStripSplitButton ToolStripSplitButton1;
        internal ToolStripDropDownButton ToolStripDropDownButton1;
        internal ToolStripMenuItem AaaToolStripMenuItem;
        internal ToolStripMenuItem AaaaToolStripMenuItem;
        internal ToolStripDropDownButton ToolStripDropDownButton2;
        internal ToolStripMenuItem AddModToolStripMenuItem;
        internal ToolStripMenuItem EnableAllToolStripMenuItem;
        internal ToolStripMenuItem DisableAllToolStripMenuItem;
        internal ToolStripMenuItem OpenGameFolderToolStripMenuItem;
        internal ToolStripMenuItem OpenModsFolderToolStripMenuItem;
        internal ToolStripLabel ToolStripLabel1;
        internal DarkToolStrip DarkToolStrip2;
        internal ToolStripLabel ToolStripLabel2;
        internal ToolStripSeparator ToolStripSeparator1;
        internal ToolStripLabel ToolStripLabel3;
        internal ToolStripSeparator ToolStripSeparator2;
        internal ColumnHeader ModName;
        internal ColumnHeader Conflicts;
        internal ColumnHeader Priority;
        internal TableLayoutPanel TableLayoutPanel1;
        internal Timer Timer1;
        internal DarkLabel DarkLabel1;
        internal DarkLabel DarkLabel2;
        internal DarkRichTextBox DarkRichTextBox1;
        internal DarkLabel DarkLabel3;
    }
}