using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;
using AltUI.Config;
using static AltUI.Config.ThemeProvider;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace VBLauncher
{

    public partial class ModLoader
    {
        private List<ModInfo> modList = [];
        private string[] bArray;

        public ModLoader()
        {
            InitializeComponent();
        }

        public void LoadMods()
        {
            var directory = new DirectoryInfo("Mods");
            foreach (var file in directory.GetFiles("*.zip"))
            {
                var zip = ZipFile.OpenRead(file.FullName);
                foreach (var entry in zip.Entries)
                {
                    if (entry.Name == "mod.info")
                    {
                        using var reader = new StreamReader(entry.Open());
                        var iniData = reader.ReadToEnd().Split(Constants.vbCrLf);
                        var mname = IniManager.Ini(ref iniData, "Info", "Name");
                        var description = IniManager.Ini(ref iniData, "Info", "Description", IniManager.KeyType.Multiline);
                        var version = IniManager.Ini(ref iniData, "Info", "Version");
                        var entries = (from ent in zip.Entries
                                       where ent.Name.ToLower() != "mod.info"
                                       select ent.Name).ToList();
                        modList.Add(new ModInfo(mname, description, version, new FileInfo(file.FullName), entries));
                        break;
                    }
                }
            }

            foreach (var m in modList)
                ListView1.Items.Add(m.ToListViewItem());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DarkRichTextBox1.SelectionBackColor = BackgroundColour;
            ListView1.Scrollable = false;
            ListView1.Items.Clear();
            DarkRichTextBox1.BackColor = BackgroundColour;
            Timer1.Start();
            ListView1.BackColor = BackgroundColour;
            ListView1.ForeColor = Theme.Colors.LightText;
            ListView1.AllowColumnReorder = false;

            var files = Directory.GetFiles("Mods");
            var directories = Directory.GetDirectories("Mods");

            LoadMods();
        }

        private class ModInfo
        {
            public string Name { get; set; }
            public ConflictStatus Conflict { get; set; }
            public int Priority { get; set; }
            public string Description { get; set; }
            public string Version { get; set; }
            public FileInfo Zip { get; set; }
            public List<string> Entries { get; set; } = [];

            public ModInfo(string Name, string Description, string Version, FileInfo Zip, List<string> Entries)
            {
                this.Name = Name;
                Conflict = ConflictStatus.Clear;
                Priority = 0;
                this.Zip = Zip;
                this.Description = Description;
                this.Version = Version;
                this.Entries = Entries;
            }

            public ListViewItem ToListViewItem()
            {
                var ret = new ListViewItem(Name);
                ret.SubItems.Add(Conflict.ToString());
                ret.SubItems.Add(Priority.ToString());
                return ret;
            }
        }

        private enum ConflictStatus
        {
            Clear,
            Overwrite,
            Overwritten,
            Mixed,
            Redundant
        }

        #region Reordering

        private void ListView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            ListView1.DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private static void ListView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        private void ListView1_DragLeave(object sender, EventArgs e)
        {
            ListView1.InsertionMark.Index = -1;
        }

        private void ListView1_DragOver(object sender, DragEventArgs e)
        {
            var targetPoint = ListView1.PointToClient(new Point(e.X, e.Y));
            var targetIndex = ListView1.InsertionMark.NearestIndex(targetPoint);
            if (targetIndex > -1)
            {
                var itemBounds = ListView1.GetItemRect(targetIndex);
                if (targetPoint.Y > itemBounds.Top + itemBounds.Height / 2)
                {
                    ListView1.InsertionMark.AppearsAfterItem = true;
                }
                else
                {
                    ListView1.InsertionMark.AppearsAfterItem = false;
                }
            }
            ListView1.InsertionMark.Index = targetIndex;
        }

        private void ListView1_DragDrop(object sender, DragEventArgs e)
        {
            var targetIndex = ListView1.InsertionMark.Index;
            if (targetIndex == -1)
            {
                return;
            }
            if (ListView1.InsertionMark.AppearsAfterItem)
            {
                targetIndex += 1;
            }
            var draggedItem = (ListViewItem)e.Data.GetData(typeof(ListViewItem));

            var tmp = modList[ListView1.Items.IndexOf(draggedItem)];
            modList.RemoveAt(ListView1.Items.IndexOf(draggedItem));
            if (targetIndex > modList.Count - 1)
            {
                modList.Add(tmp);
            }
            else
            {
                modList.Insert(targetIndex, tmp);
            }
            ListView1.Items.Insert(targetIndex, (ListViewItem)draggedItem.Clone());
            ListView1.Items.Remove(draggedItem);
            UpdateStatus(sender, e);
        }

        #endregion

        #region Paint

        private void ListView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            using var txtbrsh = new SolidBrush(Theme.Colors.LightText);
            e.Graphics.FillRectangle(new SolidBrush(BackgroundColour), e.Bounds);
            e.Graphics.DrawString(e.Header.Text, e.Font, txtbrsh, (int)Math.Round(e.Bounds.Left + e.Bounds.Width / 2d - (double)(e.Graphics.MeasureString(e.Header.Text, e.Font).Width / 2f)), e.Bounds.Top + 5);

            e.Graphics.DrawLine(new Pen(Theme.Colors.GreySelection, 1f), e.Bounds.Left, e.Bounds.Top, e.Bounds.Right, e.Bounds.Top);

            if (e.Header.DisplayIndex == 1)
            {
                e.Graphics.DrawLine(new Pen(Theme.Colors.GreySelection, 1f), e.Bounds.Left, e.Bounds.Top + 3, e.Bounds.Left, e.Bounds.Bottom - 3);
                e.Graphics.DrawLine(new Pen(Theme.Colors.GreySelection, 1f), e.Bounds.Right - 1, e.Bounds.Top + 3, e.Bounds.Right - 1, e.Bounds.Bottom - 3);
            }
        }

        private void ListView1_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            using var txtbrsh = new SolidBrush(Theme.Colors.LightText);
            if (ListView1.SelectedIndices.Contains(e.ItemIndex) & ListView1.Focused)
            {
                e.Graphics.FillRectangle(new SolidBrush(Theme.Colors.BlueHighlight), e.Bounds);
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(BackgroundColour), e.Bounds);
            }
            if (ReferenceEquals(e.Item.SubItems[0], e.SubItem))
            {
                var g = e.Graphics;
                g.Clip = new Region(new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height));
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = e.Bounds;

                var size = Theme.Sizes.CheckBoxSize;

                var textColor = Theme.Colors.LightText;
                var borderColor = Theme.Colors.GreySelection;
                var fillColor = e.Item.Checked ? Theme.Colors.LightestBackground : Theme.Colors.GreyBackground;

                using (var b = new SolidBrush(Theme.Colors.LightBackground))
                {
                    var boxRect = new Rectangle(e.Bounds.Left + 2, (int)Math.Round(e.Bounds.Top + rect.Height / 2 - size / 2d), size, size);
                    g.FillRoundedRectangle(b, boxRect, 2);
                }

                using (var p = new Pen(borderColor))
                {
                    var boxRect = new Rectangle(e.Bounds.Left + 2, (int)Math.Round(e.Bounds.Top + rect.Height / 2 - size / 2d), size, size);
                    g.DrawRoundedRectangle(p, boxRect, 2);
                }

                if (e.Item.Checked)
                {
                    using var p = new Pen(fillColor, 1f);
                    g.DrawLine(p, e.Bounds.Left + 5, e.Bounds.Top + 9, e.Bounds.Left + 7, e.Bounds.Top + 12);
                    g.DrawLine(p, e.Bounds.Left + 7, e.Bounds.Top + 12, e.Bounds.Left + 11, e.Bounds.Top + 6);
                }

                using (var b = new SolidBrush(textColor))
                {
                    g.DrawString(e.Item.Text, e.Item.Font, b, e.Bounds.Left + size + 4, e.Bounds.Top + 1);
                }
            }
            else if (ReferenceEquals(e.Item.SubItems[1], e.SubItem))
            {
                if (!e.Item.Checked) return;
                using var sf = new StringFormat();
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;
                sf.FormatFlags = StringFormatFlags.NoWrap;
                sf.Trimming = StringTrimming.EllipsisCharacter;
                e.Graphics.DrawImage((Image)My.Resources.Resources.ResourceManager.GetObject($"conflict_{e.SubItem.Text.ToLower()}"), e.Bounds.Left + e.Bounds.Width / 2.0f - 8f, e.Bounds.Top, 16f, 16f);
            }
            else if (ReferenceEquals(e.Item.SubItems[2], e.SubItem))
            {
                if (e.Item.Checked)
                {
                    e.Graphics.DrawString(e.SubItem.Text, e.Item.Font, txtbrsh, (int)Math.Round(e.Bounds.Left + e.Bounds.Width / 2d) - e.Graphics.MeasureString(e.SubItem.Text, e.SubItem.Font).Width / 2f, e.Bounds.Top + 2);
                }
            }
            else
            {
                e.DrawDefault = true;
            }
        }

        private static void ToolStripLabel_MouseEnter(object sender, EventArgs e)
        {
            ((dynamic)sender).ForeColor = Theme.Colors.BlueHighlight;
        }

        private static void ToolStripLabel_MouseLeave(object sender, EventArgs e)
        {
            ((dynamic)sender).ForeColor = Theme.Colors.LightText;
        }

        private static void TableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(new Pen(Theme.Colors.GreySelection, 1f), 0, 0, ((dynamic)sender).Width, 0);
            e.Graphics.DrawLine(new Pen(Theme.Colors.GreySelection, 1f), 0, 0, 0, ((dynamic)sender).Height);
        }

        private static void DarkToolStrip2_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(new Pen(Theme.Colors.GreySelection, 1f), 0, 0, ((dynamic)sender).Width, 0);
        }

        public class DoubleBufferedListView : ListView
        {

            public DoubleBufferedListView()
            {
                DoubleBuffered = true;
                SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
                SetStyle(ControlStyles.EnableNotifyMessage, true);
            }

            protected sealed override bool DoubleBuffered
            {
                get => base.DoubleBuffered;
                set => base.DoubleBuffered = value;
            }

            protected override void OnNotifyMessage(Message m)
            {
                if (m.Msg != 0x14)
                {
                    base.OnNotifyMessage(m);
                }
            }

            protected override void OnHandleCreated(EventArgs e)
            {
                base.OnHandleCreated(e);
                Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.ClientAndNonClientAreasEnabled;
            }
        }

        #endregion

        private void SizeAdjust()
        {
            var sz = (int)Math.Round(ListView1.Width / 4d);
            ListView1.Columns[0].Width = sz * 2;
            ListView1.Columns[1].Width = sz;
            ListView1.Columns[2].Width = sz + (ListView1.Width - sz * 4);
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            ScrollControlIntoView(ListView1);
            SizeAdjust();
        }

        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListView1.SelectedItems.Count == 0)
            {
                return;
            }

            var selectedMod = modList[ListView1.SelectedItems[0].Index];
            DarkRichTextBox1.Text = selectedMod.Description;
            DarkLabel1.Text = selectedMod.Name;
            DarkLabel3.Text = "Version: " + selectedMod.Version;
        }

        private void UpdateStatus(object sender, EventArgs e)
        {
            foreach (int i in ListView1.CheckedIndices)
            {
                var lpe = new List<string>();
                var hpe = new List<string>();
                // get all entries of checked mods
                try
                {
                    foreach (var j in ListView1.CheckedIndices)
                    {
                        if (Conversions.ToBoolean(Operators.ConditionalCompareObjectLess(j, i, false)))
                        {
                            lpe.AddRange(modList[Conversions.ToInteger(j)].Entries);
                        }
                        else if (Conversions.ToBoolean(Operators.ConditionalCompareObjectGreater(j, i, false)))
                        {
                            hpe.AddRange(modList[Conversions.ToInteger(j)].Entries);
                        }
                    }
                }
                catch
                {
                    return;
                }

                object cfs;
                var overwrites = modList[i].Entries.Any(s => s.Equals("English.stf", StringComparison.OrdinalIgnoreCase) && lpe.Contains(s));
                var overwritten = modList[i].Entries.Any(s => s.Equals("English.stf", StringComparison.OrdinalIgnoreCase) && hpe.Contains(s));
                if (overwrites && overwritten)
                {
                    cfs = ConflictStatus.Mixed;
                    modList[i].Conflict = (ConflictStatus)Conversions.ToInteger(cfs);
                }
                else if (overwrites)
                {
                    cfs = ConflictStatus.Overwrite;
                    modList[i].Conflict = (ConflictStatus)Conversions.ToInteger(cfs);
                }
                else if (overwritten)
                {
                    cfs = ConflictStatus.Overwritten;
                    modList[i].Conflict = (ConflictStatus)Conversions.ToInteger(cfs);
                }
                else
                {
                    cfs = ConflictStatus.Clear;
                    modList[i].Conflict = (ConflictStatus)Conversions.ToInteger(cfs);
                }
                var redundant = modList[i].Entries.Where(s => s.Equals("English.stf", StringComparison.OrdinalIgnoreCase)).All(s => hpe.Contains(s));
                if (redundant)
                {
                    cfs = ConflictStatus.Redundant;
                    modList[i].Conflict = (ConflictStatus)Conversions.ToInteger(cfs);
                }
                ListView1.Items[i].SubItems[1].Text = cfs.ToString();
                modList[i].Priority = ListView1.CheckedItems.IndexOf(ListView1.Items[i]);
                ListView1.Items[i].SubItems[2].Text = ListView1.CheckedItems.IndexOf(ListView1.Items[i]).ToString();
            }
            ToolStripLabel1.Text = $"Active: {ListView1.CheckedItems.Count}";
        }

        private void EnableAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem itm in ListView1.Items)
                itm.Checked = true;
        }

        private void DisableAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem itm in ListView1.Items)
                itm.Checked = false;
        }

        private void ToolStripLabel3_Click(object sender, EventArgs e)
        {
            Close();
        }

        public (string[], int) MergeSTF(string[] lArray, string[] hArray)
        {
            var oArray = new string[3282];
            var additionalStrings = 0;

            // Copy low-priority array's Vanilla string section into output, to preserve any changes.
            Array.Copy(lArray, oArray, 3281);

            // If high-priority array has modified Vanilla strings, overwrite that string.
            // this will effectively merge changes from both low and high, prioritizing high.
            for (var i = 0; i <= 3281; i++)
            {
                if ((hArray[i] ?? "") != (bArray[i] ?? ""))
                {
                    oArray[i] = hArray[i];
                }
            }

            // if low priority array has additional strings, append those to the Vanilla segment, and save that value
            // this value will be used to shift string references to non-vanilla strings.
            if (lArray.Length > 3281)
            {
                additionalStrings = lArray.Length - 3281;
                Array.Resize(ref oArray, oArray.Length + additionalStrings + 1);
                Array.Copy(lArray, 3281, oArray, 3281, additionalStrings);
            }

            // if high-priority array has additional strings, append those to the end. this allows both high and low priority arrays to add to the base.
            // no value needs to be stored. this comparison will happen again for the next array using this output as the low priority.
            // this way, it can go through the list and update the files for each mod consecutively.
            if (hArray.Length > 3281)
            {
                Array.Resize(ref oArray, oArray.Length + (hArray.Length - 3281) + 1);
                Array.Copy(hArray, 3281, oArray, 3281 + additionalStrings, hArray.Length - 3281);
            }

            // returns the string array for future use, and the # of strings between the vanilla and high-priority new.
            // this value is used to shift string references in the high priority mod's files.
            return (oArray, additionalStrings);
        }

        private void IncSTFRefs(string fp, int incVal)
        {

            var b = File.ReadAllBytes(fp);

            var file = default(object);

            switch (fp.GetExtension().ToLower() ?? "")
            {
                case ".amo":
                    {
                        break;
                    }
                // file = b.ReadAMO()
                case ".arm":
                    {
                        break;
                    }
                // file = b.ReadARM()
                case ".con":
                    {
                        break;
                    }
                // file = b.ReadCON()
                case ".crt":
                    {
                        file = b.ReadCRT();
                        break;
                    }
                case ".dor":
                    {
                        break;
                    }
                // file = b.ReadDOR()
                case ".itm":
                    {
                        file = b.ReadITM();
                        break;
                    }
                case ".use":
                    {
                        break;
                    }
                // file = b.ReadUSE()
                case ".wea":
                    {
                        break;
                    }
                    // file = b.ReadWEA()
            }

            if (file is null)
                return;

            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectGreater(((dynamic)file).GENT.HoverSR, 3281, false)))
                ((dynamic)file).GENT.HoverSR += incVal;
            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectGreater(((dynamic)file).GENT.LookSR, 3281, false)))
                ((dynamic)file).GENT.LookSR += incVal;
            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectGreater(((dynamic)file).GENT.NameSR, 3281, false)))
                ((dynamic)file).GENT.NameSR += incVal;
            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectGreater(((dynamic)file).GENT.UnkwnSR, 3281, false)))
                ((dynamic)file).GENT.UnkwnSR += incVal;

            File.WriteAllBytes(fp, (byte[])((dynamic)file).ToByte());
        }

        private void ToolStripLabel2_Click(object sender, EventArgs e)
        {
            bArray = (string[])Extensions.STFToTXT(File.ReadAllBytes(My.MySettingsProperty.Settings.STFDir));
            var tmpArray = bArray;
            foreach (var i in ListView1.CheckedIndices)
            {
                if (modList[Conversions.ToInteger(i)].Entries.Any(x => x.Equals("English.stf", StringComparison.OrdinalIgnoreCase)))
                {
                    var zip = ZipFile.OpenRead(modList[Conversions.ToInteger(i)].Zip.FullName);
                    foreach (var entry in zip.Entries)
                    {
                        if (entry.Name.ToLower() == "english.stf")
                        {
                            using var memoryStream = new MemoryStream();
                            entry.Open().CopyTo(memoryStream);
                            var stfData = memoryStream.ToArray();
                            var stfArray = Extensions.STFToTXT(stfData);

                            var tmp = MergeSTF(tmpArray, (string[])stfArray);
                            tmpArray = tmp.Item1;

                            var tmpDir = @"Mods\tmp";
                            Directory.CreateDirectory(tmpDir);
                            foreach (var entry2 in zip.Entries)
                            {
                                if (!(entry2.Name.ToLower() == "english.stf") && !(entry2.Name.ToLower() == "mod.info"))
                                {
                                    entry2.ExtractToFile(Path.Combine(tmpDir, entry2.Name), true);
                                }
                            }
                            foreach (var f in Directory.GetFiles(tmpDir))
                            {
                                var fi = new FileInfo(f);
                                IncSTFRefs(f, tmp.Item2);
                                File.Move(f, $@"Overrides\Deployed\{fi.Name}", true);
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}