using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace VBLauncher
{
    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class GrpBrowser : AltUI.Forms.DarkForm
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
        private System.ComponentModel.IContainer components;

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.  
        // Do not modify it using the code editor.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            HexBox1 = new Be.Windows.Forms.HexBox();
            DarkButton1 = new AltUI.Controls.DarkButton();
            DarkButton1.Click += new EventHandler(DarkButton1_Click);
            DarkTreeView1 = new AltUI.Controls.DarkTreeView();
            DarkTreeView1.SelectedNodesChanged += new EventHandler(DarkButton1_Click);
            TableLayoutPanel1 = new TableLayoutPanel();
            DarkButton2 = new AltUI.Controls.DarkButton();
            DarkButton2.Click += new EventHandler(DarkButton2_Click);
            DarkSectionPanel1 = new AltUI.Controls.DarkSectionPanel();
            DarkSectionPanel2 = new AltUI.Controls.DarkSectionPanel();
            PictureBox1 = new PictureBox();
            DarkRichTextBox1 = new AltUI.Controls.DarkRichTextBox();
            TableLayoutPanel2 = new TableLayoutPanel();
            TrackBar1 = new TrackBar();
            TableLayoutPanel1.SuspendLayout();
            DarkSectionPanel1.SuspendLayout();
            DarkSectionPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PictureBox1).BeginInit();
            TableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)TrackBar1).BeginInit();
            SuspendLayout();
            // 
            // HexBox1
            // 
            HexBox1.BackColor = Color.FromArgb(32, 32, 32);
            HexBox1.Dock = DockStyle.Fill;
            HexBox1.Font = new Font("Cascadia Mono", 9f, FontStyle.Regular, GraphicsUnit.Point);
            HexBox1.LineInfoVisible = true;
            HexBox1.Location = new Point(403, 3);
            HexBox1.Name = "HexBox1";
            HexBox1.ReadOnly = true;
            HexBox1.ShadowSelectionColor = Color.FromArgb(100, 60, 188, 255);
            HexBox1.Size = new Size(394, 415);
            HexBox1.StringViewVisible = true;
            HexBox1.TabIndex = 2;
            HexBox1.VScrollBarVisible = true;
            // 
            // DarkButton1
            // 
            DarkButton1.BorderColour = Color.Empty;
            DarkButton1.CustomColour = false;
            DarkButton1.Dock = DockStyle.Right;
            DarkButton1.FlatBottom = false;
            DarkButton1.FlatTop = false;
            DarkButton1.Location = new Point(722, 424);
            DarkButton1.Name = "DarkButton1";
            DarkButton1.Padding = new Padding(5);
            DarkButton1.Size = new Size(75, 23);
            DarkButton1.TabIndex = 0;
            DarkButton1.Text = "Open";
            // 
            // DarkTreeView1
            // 
            DarkTreeView1.Dock = DockStyle.Fill;
            DarkTreeView1.Location = new Point(1, 25);
            DarkTreeView1.MaxDragChange = 20;
            DarkTreeView1.Name = "DarkTreeView1";
            DarkTreeView1.Padding = new Padding(0, 0, 0, 2);
            DarkTreeView1.Size = new Size(392, 389);
            DarkTreeView1.TabIndex = 1;
            DarkTreeView1.Text = "DarkTreeView1";
            // 
            // TableLayoutPanel1
            // 
            TableLayoutPanel1.ColumnCount = 2;
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            TableLayoutPanel1.Controls.Add(DarkButton1, 1, 1);
            TableLayoutPanel1.Controls.Add(DarkButton2, 0, 1);
            TableLayoutPanel1.Controls.Add(DarkSectionPanel1, 0, 0);
            TableLayoutPanel1.Controls.Add(DarkSectionPanel2, 1, 0);
            TableLayoutPanel1.Dock = DockStyle.Fill;
            TableLayoutPanel1.Location = new Point(0, 0);
            TableLayoutPanel1.Name = "TableLayoutPanel1";
            TableLayoutPanel1.RowCount = 2;
            TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.Size = new Size(800, 450);
            TableLayoutPanel1.TabIndex = 2;
            // 
            // DarkButton2
            // 
            DarkButton2.BorderColour = Color.Empty;
            DarkButton2.CustomColour = false;
            DarkButton2.FlatBottom = false;
            DarkButton2.FlatTop = false;
            DarkButton2.Location = new Point(3, 424);
            DarkButton2.Name = "DarkButton2";
            DarkButton2.Padding = new Padding(5);
            DarkButton2.Size = new Size(75, 23);
            DarkButton2.TabIndex = 2;
            DarkButton2.Text = "Extract";
            // 
            // DarkSectionPanel1
            // 
            DarkSectionPanel1.BackColor = Color.FromArgb(28, 28, 30);
            DarkSectionPanel1.Controls.Add(DarkTreeView1);
            DarkSectionPanel1.Dock = DockStyle.Fill;
            DarkSectionPanel1.Location = new Point(3, 3);
            DarkSectionPanel1.Name = "DarkSectionPanel1";
            DarkSectionPanel1.SectionHeader = null;
            DarkSectionPanel1.Size = new Size(394, 415);
            DarkSectionPanel1.TabIndex = 3;
            // 
            // DarkSectionPanel2
            // 
            DarkSectionPanel2.BackColor = Color.FromArgb(28, 28, 30);
            DarkSectionPanel2.Dock = DockStyle.Fill;
            DarkSectionPanel2.Location = new Point(403, 3);
            DarkSectionPanel2.Name = "DarkSectionPanel2";
            DarkSectionPanel2.SectionHeader = "";
            DarkSectionPanel2.Size = new Size(394, 415);
            DarkSectionPanel2.TabIndex = 4;
            // 
            // PictureBox1
            // 
            PictureBox1.BackgroundImageLayout = ImageLayout.Center;
            PictureBox1.Dock = DockStyle.Fill;
            PictureBox1.Location = new Point(403, 3);
            PictureBox1.Name = "PictureBox1";
            PictureBox1.Size = new Size(394, 415);
            PictureBox1.TabIndex = 2;
            PictureBox1.TabStop = false;
            // 
            // DarkRichTextBox1
            // 
            DarkRichTextBox1.BackColor = Color.FromArgb(31, 31, 32);
            DarkRichTextBox1.BorderStyle = BorderStyle.None;
            DarkRichTextBox1.Dock = DockStyle.Fill;
            DarkRichTextBox1.ForeColor = Color.FromArgb(213, 213, 213);
            DarkRichTextBox1.Location = new Point(403, 3);
            DarkRichTextBox1.Name = "DarkRichTextBox1";
            DarkRichTextBox1.Size = new Size(394, 415);
            DarkRichTextBox1.TabIndex = 2;
            DarkRichTextBox1.Text = "";
            // 
            // TableLayoutPanel2
            // 
            TableLayoutPanel2.ColumnCount = 1;
            TableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            TableLayoutPanel2.Controls.Add(TrackBar1, 0, 1);
            TableLayoutPanel2.Dock = DockStyle.Fill;
            TableLayoutPanel2.Location = new Point(1, 25);
            TableLayoutPanel2.Name = "TableLayoutPanel2";
            TableLayoutPanel2.RowCount = 2;
            TableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            TableLayoutPanel2.RowStyles.Add(new RowStyle());
            TableLayoutPanel2.Size = new Size(392, 389);
            TableLayoutPanel2.TabIndex = 0;
            // 
            // TrackBar1
            // 
            TrackBar1.Dock = DockStyle.Fill;
            TrackBar1.Location = new Point(3, 341);
            TrackBar1.Name = "TrackBar1";
            TrackBar1.RightToLeft = RightToLeft.No;
            TrackBar1.Size = new Size(386, 45);
            TrackBar1.TabIndex = 0;
            // 
            // GrpBrowser
            // 
            AutoScaleDimensions = new SizeF(7f, 15f);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(TableLayoutPanel1);
            CornerStyle = CornerPreference.Default;
            Name = "GrpBrowser";
            Text = "GrpBrowser";
            TableLayoutPanel1.ResumeLayout(false);
            DarkSectionPanel1.ResumeLayout(false);
            DarkSectionPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)PictureBox1).EndInit();
            TableLayoutPanel2.ResumeLayout(false);
            TableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)TrackBar1).EndInit();
            Load += new EventHandler(GrpBrowser_Load);
            ResumeLayout(false);

        }
        internal Be.Windows.Forms.HexBox HexBox1;
        internal AltUI.Controls.DarkButton DarkButton1;
        internal AltUI.Controls.DarkTreeView DarkTreeView1;
        internal TableLayoutPanel TableLayoutPanel1;
        internal PictureBox PictureBox1;
        internal AltUI.Controls.DarkRichTextBox DarkRichTextBox1;
        internal AltUI.Controls.DarkButton DarkButton2;
        internal AltUI.Controls.DarkSectionPanel DarkSectionPanel1;
        internal AltUI.Controls.DarkSectionPanel DarkSectionPanel2;
        internal TableLayoutPanel TableLayoutPanel2;
        internal TrackBar TrackBar1;
    }
}