using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using AltUI.Config;
using VBLauncher.My.Resources;
using VBLauncher.Properties;
using static AltUI.Config.ThemeProvider;

namespace VBLauncher;

public partial class ModLoader
{
    private List<ModInfo> _modList = [];
    private string[] _bArray;
    private string[] _deployedMods = [];
    
    public ModLoader()
    {
        InitializeComponent();
    }

    private void LoadMods()
    {
        _deployedMods = Settings.Default.DeployedMods.Split(',');
        var directory = new DirectoryInfo("Mods");
        foreach (var file in directory.GetFiles("*.zip"))
        {
            var zip = ZipFile.OpenRead(file.FullName);
            foreach (var entry in zip.Entries)
            {
                if (entry.Name != "mod.info") continue;
                using var reader = new StreamReader(entry.Open());
                var iniData = reader.ReadToEnd().Split("\r\n");
                var mname = IniManager.Ini(ref iniData, "Info", "Name");
                var description = IniManager.Ini(ref iniData, "Info", "Description", IniManager.KeyType.Multiline);
                var version = IniManager.Ini(ref iniData, "Info", "Version");
                var entries = (from ent in zip.Entries
                               where !ent.Name.Equals("mod.info", StringComparison.CurrentCultureIgnoreCase)
                               select ent.Name).ToList();
                _modList.Add(new ModInfo(mname, description, version, new FileInfo(file.FullName), entries));
                break;
            }
        }

        foreach (var m in _modList)
        {
            var li = m.ToListViewItem();
            li.Checked = _deployedMods.Contains(m.Zip.Name);
            ListView1.Items.Add(li);
        }
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
        
        LoadMods();
    }

    private class ModInfo(string name, string description, string version, FileInfo zip, List<string> entries)
    {
        public string Name { get; } = name;
        public ConflictStatus Conflict { get; set; } = ConflictStatus.Clear;
        public int Priority { get; set; }
        public string Description { get; } = description;
        public string Version { get; } = version;
        public FileInfo Zip { get; } = zip;
        public List<string> Entries { get; } = entries;

        public ListViewItem ToListViewItem()
        {
            var ret = new ListViewItem
            {
                Text = Name,
                Name = Zip.Name
            };
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
            ListView1.InsertionMark.AppearsAfterItem = targetPoint.Y > itemBounds.Top + itemBounds.Height / 2;
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

        var tmp = _modList[ListView1.Items.IndexOf(draggedItem)];
        _modList.RemoveAt(ListView1.Items.IndexOf(draggedItem));
        if (targetIndex > _modList.Count - 1)
        {
            _modList.Add(tmp);
        }
        else
        {
            _modList.Insert(targetIndex, tmp);
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
        e.Graphics.DrawString(e.Header.Text, e.Font, txtbrsh, (int)Math.Round(e.Bounds.Left + e.Bounds.Width / 2d - e.Graphics.MeasureString(e.Header.Text, e.Font).Width / 2f), e.Bounds.Top + 5);

        e.Graphics.DrawLine(new Pen(Theme.Colors.GreySelection, 1f), e.Bounds.Left, e.Bounds.Top, e.Bounds.Right, e.Bounds.Top);

        if (e.Header.DisplayIndex != 1) return;
        e.Graphics.DrawLine(new Pen(Theme.Colors.GreySelection, 1f), e.Bounds.Left, e.Bounds.Top + 3, e.Bounds.Left, e.Bounds.Bottom - 3);
        e.Graphics.DrawLine(new Pen(Theme.Colors.GreySelection, 1f), e.Bounds.Right - 1, e.Bounds.Top + 3, e.Bounds.Right - 1, e.Bounds.Bottom - 3);
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
        if (ReferenceEquals(e.Item!.SubItems[0], e.SubItem))
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
                var boxRect = new Rectangle(e.Bounds.Left + 2,
                    (int)Math.Round(e.Bounds.Top + rect.Height / 2 - size / 2d), size, size);
                g.FillRoundedRectangle(b, boxRect, 2);
            }

            using (var p = new Pen(borderColor))
            {
                var boxRect = new Rectangle(e.Bounds.Left + 2,
                    (int)Math.Round(e.Bounds.Top + rect.Height / 2 - size / 2d), size, size);
                g.DrawRoundedRectangle(p, boxRect, 2);
            }

            if (e.Item.Checked)
            {
                using var p = new Pen(fillColor, 1f);
                g.DrawLine(p, e.Bounds.Left + 5, e.Bounds.Top + 9, e.Bounds.Left + 7, e.Bounds.Top + 12);
                g.DrawLine(p, e.Bounds.Left + 7, e.Bounds.Top + 12, e.Bounds.Left + 11, e.Bounds.Top + 6);
            }
            
            g.DrawString(e.Item.Text, e.Item.Font, txtbrsh, e.Bounds.Left + size + 4, e.Bounds.Top + 1);
        }
        else if (ReferenceEquals(e.Item.SubItems[1], e.SubItem))
        {
            if (!e.Item.Checked) return;
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;
            sf.FormatFlags = StringFormatFlags.NoWrap;
            sf.Trimming = StringTrimming.EllipsisCharacter;
            e.Graphics.DrawImage((Image)Resources.ResourceManager.GetObject($"conflict_{e.SubItem.Text.ToLower()}")!,
                e.Bounds.Left + e.Bounds.Width / 2.0f - 8f, e.Bounds.Top, 16f, 16f);
        }
        else if (ReferenceEquals(e.Item.SubItems[2], e.SubItem))
            if (e.Item.Checked)
                e.Graphics.DrawString(e.SubItem.Text, e.Item.Font, txtbrsh,
                    (int)Math.Round(e.Bounds.Left + e.Bounds.Width / 2d) -
                    e.Graphics.MeasureString(e.SubItem.Text, e.SubItem.Font).Width / 2f, e.Bounds.Top + 2);
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
            Application.VisualStyleState = VisualStyleState.ClientAndNonClientAreasEnabled;
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

        var selectedMod = _modList[ListView1.SelectedItems[0].Index];
        DarkRichTextBox1.Text = selectedMod.Description;
        DarkLabel1.Text = selectedMod.Name;
        DarkLabel2.Text = "Description:";
        DarkLabel3.Text = "Version: " + selectedMod.Version;
    }

    private void UpdateStatus(object sender, EventArgs e)
    {
        foreach (int i in ListView1.CheckedIndices)
        {
            var lpe = new List<string>();
            var hpe = new List<string>();
            // get all entries of checked mods
            foreach (int j in ListView1.CheckedIndices)
            {
                if (j < i)
                {
                    lpe.AddRange(_modList[j].Entries);
                }
                else if (j > i)
                {
                    hpe.AddRange(_modList[j].Entries);
                }
            }

            object cfs;
            var overwrites = _modList[i].Entries.Any(s => lpe.Contains(s));
            var overwritten = _modList[i].Entries.Any(s => hpe.Contains(s));
            var redundant = _modList[i].Entries.All(s => hpe.Contains(s));
            if (redundant)
            {
                cfs = ConflictStatus.Redundant;
            }
            else if (overwrites && overwritten)
            {
                cfs = ConflictStatus.Mixed;
            }
            else if (overwrites)
            {
                cfs = ConflictStatus.Overwrite;
            }
            else if (overwritten)
            {
                cfs = ConflictStatus.Overwritten;
            }
            else
            {
                cfs = ConflictStatus.Clear;
            }
            _modList[i].Conflict = (ConflictStatus)cfs;
            ListView1.Items[i].SubItems[1].Text = cfs.ToString();
            _modList[i].Priority = ListView1.CheckedItems.IndexOf(ListView1.Items[i]);
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

    private (string[], int) MergeSTF(string[] lArray, string[] hArray)
    {
        var oArray = new string[3281];
        var additionalStrings = 0;

        // Copy low-priority array's Vanilla string section into output, to preserve any changes.
        Array.Copy(lArray, oArray, 3281);

        // If high-priority array has modified Vanilla strings, overwrite that string.
        // this will effectively merge changes from both low and high, prioritizing high.
        for (var i = 0; i < 3281; i++)
            if (hArray[i] != _bArray[i])
                oArray[i] = hArray[i];

        // if low priority array has additional strings, append those to the Vanilla segment, and save that value
        // this value will be used to shift string references to non-vanilla strings.
        if (lArray.Length > 3281)
        {
            additionalStrings = lArray.Length - 3281;
            Array.Resize(ref oArray, oArray.Length + additionalStrings);
            Array.Copy(lArray, 3281, oArray, 3281, additionalStrings);
        }

        // if high-priority array has additional strings, append those to the end. this allows both high and low priority arrays to add to the base.
        // no value needs to be stored. this comparison will happen again for the next array using this output as the low priority.
        // this way, it can go through the list and update the files for each mod consecutively.
        if (hArray.Length > 3281)
        {
            Array.Resize(ref oArray, oArray.Length + (hArray.Length - 3281));
            Array.Copy(hArray, 3281, oArray, 3281 + additionalStrings, hArray.Length - 3281);
        }

        // returns the string array for future use, and the # of strings between the vanilla and high-priority new.
        // this value is used to shift string references in the high priority mod's files.
        return (oArray, additionalStrings);
    }

    // increases string references in a file by a given value if they're beyond the vanilla strings.
    private void IncSTFRefs(string fp, int incVal)
    {
        var b = File.ReadAllBytes(fp);
        
        dynamic? file = (fp.GetExtension().ToLower()) switch
        {
            "amo" => b.ReadAMO(),
            "arm" => b.ReadARM(),
            "con" => b.ReadUSE(),
            "crt" => b.ReadCRT(),
            "dor" => b.ReadUSE(),
            "itm" => b.ReadITM(),
            "use" => b.ReadUSE(),
            "wea" => b.ReadWEA(),
            "map" => b.ReadMap(),
            _ => null
        };

        if (file is null)
            return;

        if (file.GENT is GENTc gent)
        {
            if (gent.HoverSR > 3281)
                gent.HoverSR += incVal;
            if (gent.LookSR > 3281)
                gent.LookSR += incVal;
            if (gent.NameSR > 3281)
                gent.NameSR += incVal;
            if (gent.UnkwnSR > 3281)
                gent.UnkwnSR += incVal;
        }

        if (file.GCRE is GCREc gcre)
            foreach (var gwam in gcre.GWAM.Where(gwam => gwam.NameSR > 3281))
                gwam.NameSR += incVal;
        
        if (file is Map map)
            foreach (var note in map.EMNO.Where(note => note.sr > 3281))
                note.sr += incVal;

        File.WriteAllBytes(fp, file.ToByte().ToArray());
    }

    private void DeployMods(object sender, EventArgs e)
    {
        if (File.Exists(Settings.Default.STFDir + ".bak"))
            File.Copy(Settings.Default.STFDir + ".bak", Settings.Default.STFDir, true);
        else
            File.Copy(Settings.Default.STFDir, Settings.Default.STFDir + ".bak");
        
        const string tmpDir = @"Mods\tmp";
        if (Directory.Exists(tmpDir))
            Directory.Delete(tmpDir, true);
        if (Directory.Exists("Override\\Deployed"))
            Directory.Delete("Override\\Deployed", true);
        
        _bArray = Extensions.STFToTXT(File.ReadAllBytes(Settings.Default.STFDir)).ToArray();
        var originalStf = _bArray;

        Settings.Default.DeployedMods =
            string.Join(",", ListView1.CheckedItems.Cast<ListViewItem>().Select(lvi => lvi.Name));
        Settings.Default.Save();
        
        foreach (int i in ListView1.CheckedIndices)
        {
            var zip = ZipFile.OpenRead(_modList[i].Zip.FullName);
            Directory.CreateDirectory(tmpDir);
            // extract all mod files
            foreach (var entry2 in zip.Entries)
            {
                if (!entry2.Name.Equals("english.stf", StringComparison.CurrentCultureIgnoreCase) &&
                    !entry2.Name.Equals("mod.info", StringComparison.CurrentCultureIgnoreCase))
                {
                    entry2.ExtractToFile(Path.Combine(tmpDir, entry2.Name), true);
                }
            }

            // load the mod's english.stf file (if present)
            var modStf = zip.Entries.FirstOrDefault(entry =>
                entry.Name.Equals("english.stf", StringComparison.CurrentCultureIgnoreCase));
            if (modStf == null) continue;
            using var memoryStream = new MemoryStream();
            modStf.Open().CopyTo(memoryStream);
            var stfData = memoryStream.ToArray();
            var stfArray = Extensions.STFToTXT(stfData).ToArray();

            // merge the mod's english.stf with the original english.stf (or previously merged english.stf)
            var tmp = MergeSTF(originalStf, stfArray);
            originalStf = tmp.Item1;

            // increase string references in all files by the number of new strings added by the current mod.
            foreach (var entry in zip.Entries)
            {
                if (entry.Name.Equals("english.stf", StringComparison.CurrentCultureIgnoreCase) ||
                    entry.Name.Equals("mod.info", StringComparison.CurrentCultureIgnoreCase)) continue;
                var fp = Path.Combine(tmpDir, entry.Name);
                IncSTFRefs(fp, tmp.Item2);
            }
        }
        
        // write the merged english.stf to the output file
        File.WriteAllBytes(Settings.Default.STFDir, Extensions.TXTToSTF(originalStf));
        
        if (Directory.Exists(tmpDir))
            Directory.Move(tmpDir, "Override\\Deployed");
    }
}