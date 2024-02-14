using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace VBLauncher
{
    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class EEN2Editor : UserControl
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
            DarkLabel3 = new AltUI.Controls.DarkLabel();
            DarkTextBox2 = new AltUI.Controls.DarkTextBox();
            DarkTextBox3 = new AltUI.Controls.DarkTextBox();
            DarkCheckBox1 = new AltUI.Controls.DarkCheckBox();
            TableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // TableLayoutPanel1
            // 
            TableLayoutPanel1.BackColor = Color.FromArgb(16, 16, 17);
            TableLayoutPanel1.ColumnCount = 1;
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            TableLayoutPanel1.Controls.Add(DarkLabel1, 0, 0);
            TableLayoutPanel1.Controls.Add(DarkTextBox1, 0, 1);
            TableLayoutPanel1.Controls.Add(DarkLabel2, 0, 2);
            TableLayoutPanel1.Controls.Add(DarkLabel3, 0, 4);
            TableLayoutPanel1.Controls.Add(DarkTextBox2, 0, 3);
            TableLayoutPanel1.Controls.Add(DarkTextBox3, 0, 5);
            TableLayoutPanel1.Controls.Add(DarkCheckBox1, 0, 6);
            TableLayoutPanel1.Dock = DockStyle.Fill;
            TableLayoutPanel1.Location = new Point(0, 0);
            TableLayoutPanel1.Name = "TableLayoutPanel1";
            TableLayoutPanel1.RowCount = 7;
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.Size = new Size(308, 157);
            TableLayoutPanel1.TabIndex = 0;
            // 
            // DarkLabel1
            // 
            DarkLabel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            DarkLabel1.AutoSize = true;
            DarkLabel1.Location = new Point(3, 0);
            DarkLabel1.Name = "DarkLabel1";
            DarkLabel1.Size = new Size(74, 15);
            DarkLabel1.TabIndex = 0;
            DarkLabel1.Text = "Model Prefix";
            // 
            // DarkTextBox1
            // 
            DarkTextBox1.BackColor = Color.FromArgb(26, 26, 28);
            DarkTextBox1.BorderStyle = BorderStyle.FixedSingle;
            DarkTextBox1.Dock = DockStyle.Fill;
            DarkTextBox1.ForeColor = Color.FromArgb(213, 213, 213);
            DarkTextBox1.Location = new Point(3, 18);
            DarkTextBox1.Name = "DarkTextBox1";
            DarkTextBox1.Size = new Size(302, 23);
            DarkTextBox1.TabIndex = 1;
            // 
            // DarkLabel2
            // 
            DarkLabel2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            DarkLabel2.AutoSize = true;
            DarkLabel2.Location = new Point(3, 44);
            DarkLabel2.Name = "DarkLabel2";
            DarkLabel2.Size = new Size(98, 15);
            DarkLabel2.TabIndex = 2;
            DarkLabel2.Text = "Inventory Texture";
            // 
            // DarkLabel3
            // 
            DarkLabel3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            DarkLabel3.AutoSize = true;
            DarkLabel3.Location = new Point(3, 88);
            DarkLabel3.Name = "DarkLabel3";
            DarkLabel3.Size = new Size(103, 15);
            DarkLabel3.TabIndex = 3;
            DarkLabel3.Text = "Action Bar Texture";
            // 
            // DarkTextBox2
            // 
            DarkTextBox2.BackColor = Color.FromArgb(26, 26, 28);
            DarkTextBox2.BorderStyle = BorderStyle.FixedSingle;
            DarkTextBox2.Dock = DockStyle.Fill;
            DarkTextBox2.ForeColor = Color.FromArgb(213, 213, 213);
            DarkTextBox2.Location = new Point(3, 62);
            DarkTextBox2.Name = "DarkTextBox2";
            DarkTextBox2.Size = new Size(302, 23);
            DarkTextBox2.TabIndex = 4;
            // 
            // DarkTextBox3
            // 
            DarkTextBox3.BackColor = Color.FromArgb(26, 26, 28);
            DarkTextBox3.BorderStyle = BorderStyle.FixedSingle;
            DarkTextBox3.Dock = DockStyle.Fill;
            DarkTextBox3.ForeColor = Color.FromArgb(213, 213, 213);
            DarkTextBox3.Location = new Point(3, 106);
            DarkTextBox3.Name = "DarkTextBox3";
            DarkTextBox3.Size = new Size(302, 23);
            DarkTextBox3.TabIndex = 5;
            // 
            // DarkCheckBox1
            // 
            DarkCheckBox1.AutoSize = true;
            DarkCheckBox1.Location = new Point(3, 135);
            DarkCheckBox1.Name = "DarkCheckBox1";
            DarkCheckBox1.Offset = 1;
            DarkCheckBox1.Size = new Size(79, 19);
            DarkCheckBox1.TabIndex = 6;
            DarkCheckBox1.Text = "Selectable";
            // 
            // EEN2Editor
            // 
            AutoScaleDimensions = new SizeF(7f, 15f);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(TableLayoutPanel1);
            Name = "EEN2Editor";
            Size = new Size(308, 157);
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
        internal AltUI.Controls.DarkCheckBox DarkCheckBox1;

    }
}