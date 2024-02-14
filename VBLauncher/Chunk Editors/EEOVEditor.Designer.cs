using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace VBLauncher
{
    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class EEOVEditor : UserControl
    {

        // UserControl overrides dispose to clean up the component list.
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
            TableLayoutPanel1 = new TableLayoutPanel();
            DarkLabel1 = new AltUI.Controls.DarkLabel();
            DarkTextBox1 = new AltUI.Controls.DarkTextBox();
            DarkLabel2 = new AltUI.Controls.DarkLabel();
            DarkTextBox2 = new AltUI.Controls.DarkTextBox();
            DarkLabel3 = new AltUI.Controls.DarkLabel();
            DarkTextBox3 = new AltUI.Controls.DarkTextBox();
            DarkLabel4 = new AltUI.Controls.DarkLabel();
            DarkLabel5 = new AltUI.Controls.DarkLabel();
            DarkTextBox4 = new AltUI.Controls.DarkTextBox();
            DarkTextBox5 = new AltUI.Controls.DarkTextBox();
            DarkLabel6 = new AltUI.Controls.DarkLabel();
            TableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // TableLayoutPanel1
            // 
            TableLayoutPanel1.BackColor = Color.FromArgb(16, 16, 17);
            TableLayoutPanel1.ColumnCount = 2;
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            TableLayoutPanel1.Controls.Add(DarkTextBox5, 0, 9);
            TableLayoutPanel1.Controls.Add(DarkTextBox4, 0, 7);
            TableLayoutPanel1.Controls.Add(DarkLabel1, 0, 0);
            TableLayoutPanel1.Controls.Add(DarkTextBox1, 0, 1);
            TableLayoutPanel1.Controls.Add(DarkLabel2, 0, 2);
            TableLayoutPanel1.Controls.Add(DarkLabel3, 0, 4);
            TableLayoutPanel1.Controls.Add(DarkTextBox2, 0, 3);
            TableLayoutPanel1.Controls.Add(DarkTextBox3, 0, 5);
            TableLayoutPanel1.Controls.Add(DarkLabel4, 0, 6);
            TableLayoutPanel1.Controls.Add(DarkLabel5, 0, 8);
            TableLayoutPanel1.Controls.Add(DarkLabel6, 1, 0);
            TableLayoutPanel1.Dock = DockStyle.Fill;
            TableLayoutPanel1.Location = new Point(0, 0);
            TableLayoutPanel1.Name = "TableLayoutPanel1";
            TableLayoutPanel1.RowCount = 10;
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.Size = new Size(308, 220);
            TableLayoutPanel1.TabIndex = 0;
            // 
            // DarkLabel1
            // 
            DarkLabel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            DarkLabel1.AutoSize = true;
            DarkLabel1.Location = new Point(3, 0);
            DarkLabel1.Name = "DarkLabel1";
            DarkLabel1.Size = new Size(72, 15);
            DarkLabel1.TabIndex = 0;
            DarkLabel1.Text = "Entity Name";
            // 
            // DarkTextBox1
            // 
            DarkTextBox1.BackColor = Color.FromArgb(26, 26, 28);
            DarkTextBox1.BorderStyle = BorderStyle.FixedSingle;
            DarkTextBox1.Dock = DockStyle.Fill;
            DarkTextBox1.ForeColor = Color.FromArgb(213, 213, 213);
            DarkTextBox1.Location = new Point(3, 18);
            DarkTextBox1.Name = "DarkTextBox1";
            DarkTextBox1.Size = new Size(148, 23);
            DarkTextBox1.TabIndex = 1;
            // 
            // DarkLabel2
            // 
            DarkLabel2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            DarkLabel2.AutoSize = true;
            DarkLabel2.Location = new Point(3, 44);
            DarkLabel2.Name = "DarkLabel2";
            DarkLabel2.Size = new Size(41, 15);
            DarkLabel2.TabIndex = 2;
            DarkLabel2.Text = "Dialog";
            // 
            // DarkTextBox2
            // 
            DarkTextBox2.BackColor = Color.FromArgb(26, 26, 28);
            DarkTextBox2.BorderStyle = BorderStyle.FixedSingle;
            DarkTextBox2.Dock = DockStyle.Fill;
            DarkTextBox2.ForeColor = Color.FromArgb(213, 213, 213);
            DarkTextBox2.Location = new Point(3, 62);
            DarkTextBox2.Name = "DarkTextBox2";
            DarkTextBox2.Size = new Size(148, 23);
            DarkTextBox2.TabIndex = 4;
            // 
            // DarkLabel3
            // 
            DarkLabel3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            DarkLabel3.AutoSize = true;
            DarkLabel3.Location = new Point(3, 88);
            DarkLabel3.Name = "DarkLabel3";
            DarkLabel3.Size = new Size(37, 15);
            DarkLabel3.TabIndex = 3;
            DarkLabel3.Text = "Script";
            // 
            // DarkTextBox3
            // 
            DarkTextBox3.BackColor = Color.FromArgb(26, 26, 28);
            DarkTextBox3.BorderStyle = BorderStyle.FixedSingle;
            DarkTextBox3.Dock = DockStyle.Fill;
            DarkTextBox3.ForeColor = Color.FromArgb(213, 213, 213);
            DarkTextBox3.Location = new Point(3, 106);
            DarkTextBox3.Name = "DarkTextBox3";
            DarkTextBox3.Size = new Size(148, 23);
            DarkTextBox3.TabIndex = 5;
            // 
            // DarkLabel4
            // 
            DarkLabel4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            DarkLabel4.AutoSize = true;
            DarkLabel4.Location = new Point(3, 132);
            DarkLabel4.Name = "DarkLabel4";
            DarkLabel4.Size = new Size(70, 15);
            DarkLabel4.TabIndex = 6;
            DarkLabel4.Text = "Skin Texture";
            // 
            // DarkLabel5
            // 
            DarkLabel5.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            DarkLabel5.AutoSize = true;
            DarkLabel5.Location = new Point(3, 176);
            DarkLabel5.Name = "DarkLabel5";
            DarkLabel5.Size = new Size(37, 15);
            DarkLabel5.TabIndex = 7;
            DarkLabel5.Text = "Effect";
            // 
            // DarkTextBox4
            // 
            DarkTextBox4.BackColor = Color.FromArgb(26, 26, 28);
            DarkTextBox4.BorderStyle = BorderStyle.FixedSingle;
            DarkTextBox4.Dock = DockStyle.Fill;
            DarkTextBox4.ForeColor = Color.FromArgb(213, 213, 213);
            DarkTextBox4.Location = new Point(3, 150);
            DarkTextBox4.Name = "DarkTextBox4";
            DarkTextBox4.Size = new Size(148, 23);
            DarkTextBox4.TabIndex = 8;
            // 
            // DarkTextBox5
            // 
            DarkTextBox5.BackColor = Color.FromArgb(26, 26, 28);
            DarkTextBox5.BorderStyle = BorderStyle.FixedSingle;
            DarkTextBox5.Dock = DockStyle.Fill;
            DarkTextBox5.ForeColor = Color.FromArgb(213, 213, 213);
            DarkTextBox5.Location = new Point(3, 194);
            DarkTextBox5.Name = "DarkTextBox5";
            DarkTextBox5.Size = new Size(148, 23);
            DarkTextBox5.TabIndex = 9;
            // 
            // DarkLabel6
            // 
            DarkLabel6.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            DarkLabel6.AutoSize = true;
            DarkLabel6.Location = new Point(157, 0);
            DarkLabel6.Name = "DarkLabel6";
            DarkLabel6.Size = new Size(57, 15);
            DarkLabel6.TabIndex = 10;
            DarkLabel6.Text = "Inventory";
            // 
            // EEOVEditor
            // 
            AutoScaleDimensions = new SizeF(7f, 15f);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(TableLayoutPanel1);
            Name = "EEOVEditor";
            Size = new Size(308, 220);
            TableLayoutPanel1.ResumeLayout(false);
            TableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        internal TableLayoutPanel TableLayoutPanel1;
        internal AltUI.Controls.DarkLabel DarkLabel1;
        internal AltUI.Controls.DarkTextBox DarkTextBox1;
        internal AltUI.Controls.DarkLabel DarkLabel2;
        internal AltUI.Controls.DarkLabel DarkLabel3;
        internal AltUI.Controls.DarkTextBox DarkTextBox2;
        internal AltUI.Controls.DarkTextBox DarkTextBox3;
        internal AltUI.Controls.DarkTextBox DarkTextBox5;
        internal AltUI.Controls.DarkTextBox DarkTextBox4;
        internal AltUI.Controls.DarkLabel DarkLabel4;
        internal AltUI.Controls.DarkLabel DarkLabel5;
        internal AltUI.Controls.DarkLabel DarkLabel6;

    }
}