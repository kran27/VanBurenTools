using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AltUI.Config;
using AltUI.Controls;
using AltUI.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace VBLauncher
{

    public partial class Editor
    {

        #region Variables

        private List<string> stf;
        private string f;
        private string ext;
        private object cf; // As ITM ' Current file, ambiguous type (I set the type while coding to avoid errors)
        private readonly string[] DontWipe = new string[] { "Triggertcb", "GCREspcb", "GCREskcb", "GCREtrcb", "GCREtscb", "GCREsoccb" };
        private bool HCtrl;
        private bool HN;
        private bool H1;
        private bool H2;
        private bool H3;
        private bool H4;
        private bool H5;
        private bool H6;
        private bool H7;
        private bool H8;
        private bool H9;
        private bool H0;

        public Editor()
        {
            InitializeComponent();
        }

        #endregion

        private void InitialSetup()
        {
            Size = new Size(640, 480);
            EnableSTFEdit.Checked = My.MySettingsProperty.Settings.STFEditEnabled;
            Mapgb.Hide();
            CRTgb.Hide();
            ITMgb.Hide();
            EPTHnud.Enabled = false;
            Triggernud.Enabled = false;
            EPTHnud.Minimum = 1m;
            Triggernud.Minimum = 1m;
            EMAPslb.CustomColour = true;
            var CellStyle = new DataGridViewCellStyle() { BackColor = ThemeProvider.BackgroundColour, ForeColor = ThemeProvider.Theme.Colors.LightText, SelectionBackColor = ThemeProvider.Theme.Colors.BlueSelection, SelectionForeColor = ThemeProvider.Theme.Colors.LightText };
            EME2dgv.GridColor = ThemeProvider.Theme.Colors.GreySelection;
            EME2dgv.BackgroundColor = ThemeProvider.BackgroundColour;
            EME2dgv.DefaultCellStyle = CellStyle;
            EEN2dgv.GridColor = ThemeProvider.Theme.Colors.GreySelection;
            EEN2dgv.BackgroundColor = ThemeProvider.BackgroundColour;
            EEN2dgv.DefaultCellStyle = CellStyle;
            GCREdgv.GridColor = ThemeProvider.Theme.Colors.GreySelection;
            GCREdgv.BackgroundColor = ThemeProvider.BackgroundColour;
            GCREdgv.DefaultCellStyle = CellStyle;
        }

        #region New Files

        private void NewAmo()
        {
        }

        private void NewArm()
        {
        }

        private void NewCon()
        {
        }

        private void NewCrt()
        {
            if (CheckAndLoadStf())
            {
                ext = ".crt";
                cf = new CRT();
                CRT argcf = (CRT)cf;
                CRTSetupUI(ref argcf);
                cf = argcf;
            }
            else
            {
                DarkMessageBox.ShowError($".STF Not selected, file creation aborted", ".STF Not Selected");
            }
        }

        private void NewDor()
        {
        }

        private void NewInt()
        {
        }

        private void NewItm()
        {
            if (CheckAndLoadStf())
            {
                ext = ".itm";
                cf = new ITM();
                ITM argcf = (ITM)cf;
                ITMSetupUI(ref argcf);
                cf = argcf;
            }
            else
            {
                DarkMessageBox.ShowError($".STF Not selected, file creation aborted", ".STF Not Selected");
            }
        }

        private void NewMap()
        {
            ext = ".map";
            cf = new Map();
            Map argcf = (Map)cf;
            MapSetupUI(ref argcf);
            cf = argcf;
        }

        private void NewUse()
        {
        }

        private void NewWea()
        {
        }

        #endregion

        private void OpenFile()
        {
            // Loads all regions of a given file into their respective classes, and from there into the UI
            var ofd = new OpenFileDialog() { Filter = "Van Buren Data File|*.amo;*.arm;*.con;*.crt;*.dor;*.int;*.itm;*.map;*.use;*.wea", Multiselect = false, ValidateNames = true };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                f = ofd.FileName;
                ext = f.Substring(f.LastIndexOf("."), 4).ToLower();
                byte[] fb = File.ReadAllBytes(f);
                LoadFile(fb, ext);
            }
        }

        private void LoadFile(byte[] fb, string ext)
        {
            if (CheckAndLoadStf())
            {
                switch (ext ?? "")
                {
                    case ".amo":
                        {
                            Interaction.MsgBox("Not yet implemented");
                            break;
                        }
                    case ".arm":
                        {
                            Interaction.MsgBox("Not yet implemented");
                            break;
                        }
                    case ".con":
                        {
                            Interaction.MsgBox("Not yet implemented");
                            break;
                        }
                    case ".crt":
                        {
                            cf = fb.ReadCRT();
                            CRTgb.Text = $".CRT Editor ({f.Substring(f.LastIndexOf(@"\") + 1)})";
                            CRT argcf = (CRT)cf;
                            CRTSetupUI(ref argcf);
                            cf = argcf;
                            break;
                        }
                    case ".dor":
                        {
                            Interaction.MsgBox("Not yet implemented");
                            break;
                        }
                    case ".int":
                        {
                            Interaction.MsgBox("Not yet implemented");
                            break;
                        }
                    case ".itm":
                        {
                            cf = fb.ReadITM();
                            ITMgb.Text = $".ITM Editor ({f.Substring(f.LastIndexOf(@"\") + 1)})";
                            ITM argcf1 = (ITM)cf;
                            ITMSetupUI(ref argcf1);
                            cf = argcf1;
                            break;
                        }
                    case ".map":
                        {
                            cf = fb.ReadMap();
                            Mapgb.Text = $".MAP Editor ({f.Substring(f.LastIndexOf(@"\") + 1)})";
                            Map argcf2 = (Map)cf;
                            MapSetupUI(ref argcf2);
                            cf = argcf2;
                            break;
                        }
                    case ".use":
                        {
                            Interaction.MsgBox("Not yet implemented");
                            break;
                        }
                    case ".wea":
                        {
                            Interaction.MsgBox("Not yet implemented");
                            break;
                        }
                }
            }
            else
            {
                DarkMessageBox.ShowError($".STF Not selected, loading of {f} aborted", ".STF Not Selected");
            }
        }

        private void OpenFromgrpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var grpb = new GrpBrowser(new string[] { "amo", "arm", "con", "crt", "dor", "int", "itm", "map", "use", "wea" }))
            {
                grpb.ShowDialog();
                f = grpb.FileName;
                LoadFile(grpb.FileBytes, "." + grpb.Extension);
            }
        }

        private void SaveFile()
        {
            var sfd = new SaveFileDialog() { Filter = $"Van Buren Data File|*{ext}", ValidateNames = true, DefaultExt = ext };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (My.MySettingsProperty.Settings.STFEditEnabled && stf is not null)
                    File.WriteAllBytes(My.MySettingsProperty.Settings.STFDir, Extensions.TXTToSTF(stf.ToArray()));
                File.WriteAllBytes(sfd.FileName, (byte[])((dynamic)cf).ToByte().ToArray());
            }
        }

        private void ResetUI()
        {
            bool en;
            foreach (DarkGroupBox c in from con in Controls.OfType<DarkGroupBox>()
                                       select con)
            {
                foreach (DarkGroupBox c2 in from con in c.Controls.OfType<DarkGroupBox>()
                                            select con)
                {
                    foreach (var c3 in c2.Controls)
                    {
                        en = Conversions.ToBoolean(((dynamic)c3).Enabled);
                        ((dynamic)c3).Enabled = false;
                        if (c3 is DarkTextBox)
                        {
                            try
                            {
                                ((dynamic)c3).Text = "";
                            }
                            catch
                            {
                                Interaction.MsgBox(((dynamic)c3).Name);
                            }
                        }
                        else if (c3 is DarkNumericUpDown)
                        {
                            try
                            {
                                ((dynamic)c3).Value = ((dynamic)c3).Minimum;
                            }
                            catch
                            {
                            }
                        }
                        else if (c3 is DarkComboBox && !DontWipe.Contains(((DarkComboBox)c3).Name))
                        {
                            ((dynamic)c3).Items.Clear();
                            ((dynamic)c3).Refresh(); // Refresh control so the text region is not transparent
                        }
                        else if (c3 is DataGridView)
                        {
                            ((dynamic)c3).DataSource = (object)null;
                        } ((dynamic)c3).Enabled = en;
                    }
                }
            }
        }

        #region File-Specific UI Setup

        private void MapSetupUI(ref Map cf) // Only way I found to bypass the issue of functions like .Any() not being found
        {
            Mapgb.Location = new Point(12, 27);
            Size = new Size(Mapgb.Width + Mapgb.Padding.Left * 13, Mapgb.Height + DarkMainMenuStrip.Height + Mapgb.Padding.Top * 17);
            CRTgb.Hide();
            Mapgb.Show();
            ITMgb.Hide();
            ResetUI();
            EMAPToUI();
            foreach (var EME2 in cf.EME2)
                EME2cb.Items.Add(EME2.name);
            if (cf.EME2.Any())
            {
                EME2cb.SelectedIndex = 0;
                foreach (Control c in EME2gb.Controls)
                    c.Enabled = true;
                EME2ToUI();
            }
            else
            {
                foreach (Control c in EME2gb.Controls)
                    c.Enabled = false;
                EME2cb.Enabled = true;
                EME2p.Enabled = true;
            }
            if (cf.ECAM is not null)
                ECAMToUI();
            if (cf.EMEP.Any())
            {
                for (int i = 1, loopTo = cf.EMEP.Count; i <= loopTo; i++)
                    EMEPcb.Items.Add(i);
                EMEPcb.SelectedIndex = 0;
                foreach (Control c in EMEPgb.Controls)
                    c.Enabled = true;
                EMEPToUI();
            }
            else
            {
                foreach (Control c in EMEPgb.Controls)
                    c.Enabled = false;
                EMEPcb.Enabled = true;
                EMEPp.Enabled = true;
            }
            if (cf.Triggers.Any())
            {
                string a = "";
                for (int i = 1, loopTo1 = cf.Triggers.Count; i <= loopTo1; i++)
                {
                    if (cf.Triggers[i - 1].ExTR.s == "S" | cf.Triggers[i - 1].ExTR.s == "T")
                        a = $" ({cf.Triggers[i - 1].ExTR.s})";
                    Triggercb.Items.Add($"{i}{a}");
                    a = "";
                }
                Triggercb.SelectedIndex = 0;
                foreach (Control c in Triggergb.Controls)
                    c.Enabled = true;
                TriggerToUI();
            }
            else
            {
                foreach (Control c in Triggergb.Controls)
                    c.Enabled = false;
                Triggercb.Enabled = true;
                Triggerp.Enabled = true;
            }
            if (cf.EPTH.Any())
            {
                for (int i = 1, loopTo2 = cf.EPTH.Count; i <= loopTo2; i++)
                    EPTHcb.Items.Add($"{i} ({cf.EPTH[i - 1].name})");
                EPTHcb.SelectedIndex = 0;
                foreach (Control c in EPTHGB.Controls)
                    c.Enabled = true;
                EPTHToUI();
            }
            else
            {
                foreach (Control c in EPTHGB.Controls)
                    c.Enabled = false;
                EPTHcb.Enabled = true;
                EPTHp.Enabled = true;
            }
            if (cf.EMSD.Any())
            {
                for (int i = 1, loopTo3 = cf.EMSD.Count; i <= loopTo3; i++)
                    EMSDcb.Items.Add($"{i} ({cf.EMSD[i - 1].s2.Replace(".psf", "")})");
                EMSDcb.SelectedIndex = 0;
                foreach (Control c in EMSDgb.Controls)
                    c.Enabled = true;
                EMSDToUI();
            }
            else
            {
                foreach (Control c in EMSDgb.Controls)
                    c.Enabled = false;
                EMSDcb.Enabled = true;
                EMSDp.Enabled = true;
            }
            if (cf.EMEF.Any())
            {
                for (int i = 1, loopTo4 = cf.EMEF.Count; i <= loopTo4; i++)
                    EMEFcb.Items.Add($"{i} ({cf.EMEF[i - 1].s2.Replace(".veg", "")})");
                EMEFcb.SelectedIndex = 0;
                foreach (Control c in EMEFgb.Controls)
                    c.Enabled = true;
                EMEFToUI();
            }
            else
            {
                foreach (Control c in EMEFgb.Controls)
                    c.Enabled = false;
                EMEFcb.Enabled = true;
                EMEFp.Enabled = true;
            }
            if (cf._2MWT is null || !cf._2MWT.chunks.Any())
            {
                foreach (Control c in _2MWTgb.Controls)
                    c.Enabled = false;
                _2MWTcb.Enabled = true;
                _2MWTp.Enabled = true;
            }
            else
            {
                _2MWTToUI();
            }
        }

        private void CRTSetupUI(ref CRT cf) // Only way I found to bypass the issue of functions like .Any() not being found
        {
            CRTgb.Controls.Add(EEN2gb);
            CRTgb.Controls.Add(GENTgb);
            GENTgb.Location = new Point(618, 22);
            CRTgb.Location = new Point(12, 27);
            Size = new Size(CRTgb.Width + CRTgb.Padding.Left * 13, CRTgb.Height + DarkMainMenuStrip.Height + CRTgb.Padding.Top * 17);
            CRTgb.Show();
            Mapgb.Hide();
            ITMgb.Hide();
            ResetUI();

            EEN2ToUI();
            GENTToUI();
            GCHRn.Text = cf.GCHR.name;

            if (cf.GCRE.GWAM.Any())
            {
                for (int i = 1, loopTo = cf.GCRE.GWAM.Count; i <= loopTo; i++)
                {
                    int ind = cf.GCRE.GWAM[i - 1].NameSR - 1;
                    GWAMcb.Items.Add($"{i} ({(ind == -1 ? "null" : stf[ind])})");
                }
                GWAMcb.SelectedIndex = 0;
                foreach (Control c in GWAMgb.Controls)
                    c.Enabled = true;
                GWAMToUI();
            }
            else
            {
                foreach (Control c in GWAMgb.Controls)
                    c.Enabled = false;
                GWAMgb.Enabled = true;
                GWAMp.Enabled = true;
            }

            GCREToUI();
        }

        private void ITMSetupUI(ref ITM cf) // Only way I found to bypass the issue of functions like .Any() not being found
        {
            ITMgb.Controls.Add(EEN2gb);
            ITMgb.Controls.Add(GENTgb);
            GENTgb.Location = new Point(618, 66);
            ITMgb.Location = new Point(12, 27);
            Size = new Size(ITMgb.Width + ITMgb.Padding.Left * 13, ITMgb.Height + DarkMainMenuStrip.Height + ITMgb.Padding.Top * 17);
            ITMgb.Show();
            CRTgb.Hide();
            Mapgb.Hide();
            ResetUI();

            EEN2ToUI();
            GENTToUI();
            GITMToUI();
        }

        #endregion

        #region Load classes into UI

        private void _2MWTToUI()
        {
            Map argcf = (Map)cf;
            _2MWTToUI(ref argcf);
            cf = argcf;
        }

        private void _2MWTToUI(ref Map cf)
        {
            if (cf._2MWT.chunks.Count == 0)
            {
                foreach (Control c in _2MWTgb.Controls)
                    c.Enabled = false;
                _2MWTcb.Enabled = true;
                _2MWTp.Enabled = true;
            }
            else
            {
                _2MWTmpf.Text = cf._2MWT.mpf;
                _2MWTfr.Checked = cf._2MWT.frozen;
                _2MWTdw.Checked = cf._2MWT.dark;
                for (int i = 1, loopTo = cf._2MWT.chunks.Count; i <= loopTo; i++)
                    _2MWTcb.Items.Add(i);
                _2MWTcb.SelectedIndex = 0;
                _2MWTx.Text = cf._2MWT.chunks[0].loc.x.ToString();
                _2MWTy.Text = cf._2MWT.chunks[0].loc.y.ToString();
                _2MWTz.Text = cf._2MWT.chunks[0].loc.z.ToString();
                _2MWTlmx.Text = cf._2MWT.chunks[0].texloc.x.ToString();
                _2MWTlmy.Text = cf._2MWT.chunks[0].texloc.y.ToString();
                try
                {
                    cf._2MWT.chunks[0].tex.Substring(0, cf._2MWT.chunks[0].tex.LastIndexOf("."));
                }
                catch
                {
                    _2MWTtex.Text = "";
                }
            }
        }

        private void EMAPToUI()
        {
            EMAPslb.BorderColour = (Color)((dynamic)cf).EMAP.col;
            EMAPs1.Text = Conversions.ToString(((dynamic)cf).EMAP.s1.Replace(".8", ""));
            EMAPs2.Text = Conversions.ToString(((dynamic)cf).EMAP.s2.Replace(".rle", ""));
            EMAPs3.Text = Conversions.ToString(((dynamic)cf).EMAP.s3.Replace(".dds", ""));
            EMAPilcb.Checked = Conversions.ToBoolean(((dynamic)cf).EMAP.il);
        }

        private void EME2ToUI()
        {
            EME2n.Text = Conversions.ToString(((dynamic)cf).EME2(EME2cb.SelectedIndex).name);
            EME2s1.Text = Conversions.ToString(((dynamic)cf).EME2(EME2cb.SelectedIndex).EEOV.s1);
            EME2s2.Text = Conversions.ToString(((dynamic)cf).EME2(EME2cb.SelectedIndex).EEOV.s2);
            EME2s3.Text = Conversions.ToString(((dynamic)cf).EME2(EME2cb.SelectedIndex).EEOV.s3.Replace(".amx", ""));
            EME2s4.Text = Conversions.ToString(((dynamic)cf).EME2(EME2cb.SelectedIndex).EEOV.s4.Replace(".dds", ""));
            EME2s5.Text = Conversions.ToString(((dynamic)cf).EME2(EME2cb.SelectedIndex).EEOV.s5.Replace(".veg", ""));
            EME2x.Text = Conversions.ToString(((dynamic)cf).EME2(EME2cb.SelectedIndex).l.x);
            EME2y.Text = Conversions.ToString(((dynamic)cf).EME2(EME2cb.SelectedIndex).l.y);
            EME2z.Text = Conversions.ToString(((dynamic)cf).EME2(EME2cb.SelectedIndex).l.z);
            EME2r.Text = Conversions.ToString(((dynamic)cf).EME2(EME2cb.SelectedIndex).l.r);
            var dt = new DataTable();
            dt.Columns.Add("item name", typeof(string));
            foreach (var item in (IEnumerable)((dynamic)cf).EME2(EME2cb.SelectedIndex).EEOV.inv)
                dt.Rows.Add(item);
            EME2dgv.DataSource = dt;
            foreach (DataGridViewColumn col in EME2dgv.Columns)
            {
                col.MinimumWidth = EME2dgv.Width - 18;
                col.Width = EME2dgv.Width - 18;
            }
        }

        private void EMEPToUI()
        {
            EMEPnud.Value = Conversions.ToDecimal(((dynamic)cf).EMEP(EMEPcb.SelectedIndex).index);
            EMEPx.Text = Conversions.ToString(((dynamic)cf).EMEP(EMEPcb.SelectedIndex).p.x);
            EMEPy.Text = Conversions.ToString(((dynamic)cf).EMEP(EMEPcb.SelectedIndex).p.y);
            EMEPz.Text = Conversions.ToString(((dynamic)cf).EMEP(EMEPcb.SelectedIndex).p.z);
            EMEPr.Text = Conversions.ToString(((dynamic)cf).EMEP(EMEPcb.SelectedIndex).r);
        }

        private void ECAMToUI()
        {
            ECAMx.Text = Conversions.ToString(((dynamic)cf).ECAM.p.x);
            ECAMy.Text = Conversions.ToString(((dynamic)cf).ECAM.p.y);
            ECAMz.Text = Conversions.ToString(((dynamic)cf).ECAM.p.z);
            ECAMr.Text = Conversions.ToString(((dynamic)cf).ECAM.p.r);
        }

        private void TriggerToUI()
        {
            Triggernud.Value = 1m;
            Triggertcb.SelectedItem = ((dynamic)cf).Triggers(Triggercb.SelectedIndex).ExTR.@type;
            Triggern.Enabled = true;
            Triggern.Text = Conversions.ToString(((dynamic)cf).Triggers(Triggercb.SelectedIndex).ExTR.s);
            Triggernud_ValueChanged();
        }

        private void Triggernud_ValueChanged()
        {
            if (Triggernud.Enabled)
            {
                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectGreater(Triggernud.Value, ((dynamic)cf).Triggers(Triggercb.SelectedIndex).EMTR.r.Count, false)))
                {
                    ((dynamic)cf).Triggers(Triggercb.SelectedIndex).EMTR.r.Add(new Point3(0f, 0f, 0f));
                }
                Triggerx.Text = Conversions.ToString(((dynamic)cf).Triggers(Triggercb.SelectedIndex).EMTR.r(Triggernud.Value - 1m).x);
                Triggery.Text = Conversions.ToString(((dynamic)cf).Triggers(Triggercb.SelectedIndex).EMTR.r(Triggernud.Value - 1m).y);
                Triggerz.Text = Conversions.ToString(((dynamic)cf).Triggers(Triggercb.SelectedIndex).EMTR.r(Triggernud.Value - 1m).z);
            }
        }

        private void EPTHToUI()
        {
            EPTHnud.Value = 1m;
            EPTHn.Text = Conversions.ToString(((dynamic)cf).EPTH(EPTHcb.SelectedIndex).name);
            EPTHnud_ValueChanged();
        }

        private void EPTHnud_ValueChanged()
        {
            if (EPTHnud.Enabled)
            {
                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectGreater(EPTHnud.Value, ((dynamic)cf).EPTH(EPTHcb.SelectedIndex).p.Count, false)))
                {
                    ((dynamic)cf).EPTH(EPTHcb.SelectedIndex).p.Add(new Point4(0f, 0f, 0f, 0f));
                }
                EPTHx.Text = Conversions.ToString(((dynamic)cf).EPTH(EPTHcb.SelectedIndex).p(EPTHnud.Value - 1m).x);
                EPTHy.Text = Conversions.ToString(((dynamic)cf).EPTH(EPTHcb.SelectedIndex).p(EPTHnud.Value - 1m).y);
                EPTHz.Text = Conversions.ToString(((dynamic)cf).EPTH(EPTHcb.SelectedIndex).p(EPTHnud.Value - 1m).z);
                EPTHr.Text = Conversions.ToString(((dynamic)cf).EPTH(EPTHcb.SelectedIndex).p(EPTHnud.Value - 1m).r);
            }
        }

        private void Triggerpm_Click()
        {
            decimal i = Triggernud.Value - 1m;
            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectNotEqual(((dynamic)cf).Triggers(Triggercb.SelectedIndex).EMTR.r.Count, 1, false)))
            {
                ((dynamic)cf).Triggers(Triggercb.SelectedIndex).EMTR.r.RemoveAt(i);
                Triggernud.Value = i;
            }
        }

        private void EPTHpm_Click()
        {
            decimal i = EPTHnud.Value - 1m;
            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectNotEqual(((dynamic)cf).EPTH(EPTHcb.SelectedIndex).p.Count, 1, false)))
            {
                ((dynamic)cf).EPTH(EPTHcb.SelectedIndex).p.RemoveAt(i);
                EPTHnud.Value = i;
            }
        }

        private void EMSDToUI()
        {
            EMSDs1.Text = Conversions.ToString(((dynamic)cf).EMSD(EMSDcb.SelectedIndex).s1);
            EMSDs2.Text = Conversions.ToString(((dynamic)cf).EMSD(EMSDcb.SelectedIndex).s2.Replace(".psf", ""));
            EMSDx.Text = Conversions.ToString(((dynamic)cf).EMSD(EMSDcb.SelectedIndex).l.x);
            EMSDy.Text = Conversions.ToString(((dynamic)cf).EMSD(EMSDcb.SelectedIndex).l.y);
            EMSDz.Text = Conversions.ToString(((dynamic)cf).EMSD(EMSDcb.SelectedIndex).l.z);
        }

        private void EMEFToUI()
        {
            EMEFs1.Text = Conversions.ToString(((dynamic)cf).EMEF(EMEFcb.SelectedIndex).s1);
            EMEFs2.Text = Conversions.ToString(((dynamic)cf).EMEF(EMEFcb.SelectedIndex).s2.Replace(".veg", ""));
            EMEFx.Text = Conversions.ToString(((dynamic)cf).EMEF(EMEFcb.SelectedIndex).l.x);
            EMEFy.Text = Conversions.ToString(((dynamic)cf).EMEF(EMEFcb.SelectedIndex).l.y);
            EMEFz.Text = Conversions.ToString(((dynamic)cf).EMEF(EMEFcb.SelectedIndex).l.z);
            EMEFr.Text = Conversions.ToString(((dynamic)cf).EMEF(EMEFcb.SelectedIndex).l.r);
        }

        private void EEN2ToUI()
        {
            EEN2skl.Text = Conversions.ToString(((dynamic)cf).EEN2.skl);
            EEN2invt.Text = Conversions.ToString(((dynamic)cf).EEN2.invtex.Replace(".dds", ""));
            EEN2actt.Text = Conversions.ToString(((dynamic)cf).EEN2.acttex.Replace(".dds", ""));
            EEN2sel.Checked = Conversions.ToBoolean(((dynamic)cf).EEN2.sel);
            EEN2s1.Text = Conversions.ToString(((dynamic)cf).EEN2.EEOV.s1);
            EEN2s2.Text = Conversions.ToString(((dynamic)cf).EEN2.EEOV.s2);
            EEN2s3.Text = Conversions.ToString(((dynamic)cf).EEN2.EEOV.s3.Replace(".amx", ""));
            EEN2s4.Text = Conversions.ToString(((dynamic)cf).EEN2.EEOV.s4.Replace(".dds", ""));
            EEN2s5.Text = Conversions.ToString(((dynamic)cf).EEN2.EEOV.s5.Replace(".veg", ""));
            var dt = new DataTable();
            dt.Columns.Add("item name", typeof(string));
            foreach (var item in (IEnumerable)((dynamic)cf).EEN2.EEOV.inv)
                dt.Rows.Add(item);
            EEN2dgv.DataSource = dt;
            foreach (DataGridViewColumn col in EEN2dgv.Columns)
            {
                col.MinimumWidth = EEN2dgv.Width - 18;
                col.Width = EEN2dgv.Width - 18;
            }
        }

        private void GENTToUI()
        {
            GENThSR.Value = Conversions.ToDecimal(((dynamic)cf).GENT.HoverSR);
            try
            {
                GENTh.Text = stf[Conversions.ToInteger(Operators.SubtractObject(((dynamic)cf).GENT.HoverSR, 1))];
            }
            catch
            {
            }
            GENTh.Enabled = Conversions.ToBoolean(My.MySettingsProperty.Settings.STFEditEnabled && Operators.ConditionalCompareObjectNotEqual(((dynamic)cf).GENT.HoverSR, 0, false));
            GENTlSR.Value = Conversions.ToDecimal(((dynamic)cf).GENT.LookSR);
            try
            {
                GENTl.Text = stf[Conversions.ToInteger(Operators.SubtractObject(((dynamic)cf).GENT.LookSR, 1))];
            }
            catch
            {
            }
            GENTl.Enabled = Conversions.ToBoolean(My.MySettingsProperty.Settings.STFEditEnabled && Operators.ConditionalCompareObjectNotEqual(((dynamic)cf).GENT.LookSR, 0, false));
            GENTnSR.Value = Conversions.ToDecimal(((dynamic)cf).GENT.NameSR);
            try
            {
                GENTn.Text = stf[Conversions.ToInteger(Operators.SubtractObject(((dynamic)cf).GENT.NameSR, 1))];
            }
            catch
            {
            }
            GENTn.Enabled = Conversions.ToBoolean(My.MySettingsProperty.Settings.STFEditEnabled && Operators.ConditionalCompareObjectNotEqual(((dynamic)cf).GENT.NameSR, 0, false));
            GENTuSR.Value = Conversions.ToDecimal(((dynamic)cf).GENT.UnkwnSR);
            try
            {
                GENTu.Text = stf[Conversions.ToInteger(Operators.SubtractObject(((dynamic)cf).GENT.UnkwnSR, 1))];
            }
            catch
            {
            }
            GENTu.Enabled = Conversions.ToBoolean(My.MySettingsProperty.Settings.STFEditEnabled && Operators.ConditionalCompareObjectNotEqual(((dynamic)cf).GENT.UnkwnSR, 0, false));
            GENTmhp.Value = Conversions.ToDecimal(((dynamic)cf).GENT.MaxHealth);
            GENTihp.Value = Conversions.ToDecimal(((dynamic)cf).GENT.StartHealth);
        }

        private void GWAMToUI()
        {
            GWAMani.Value = Conversions.ToDecimal(((dynamic)cf).GCRE.GWAM(GWAMcb.SelectedIndex).Anim);
            GWAMdt.SelectedIndex = Conversions.ToInteger(((dynamic)cf).GCRE.GWAM(GWAMcb.SelectedIndex).DmgType);
            GWAMsf.Value = Conversions.ToDecimal(((dynamic)cf).GCRE.GWAM(GWAMcb.SelectedIndex).ShotsFired);
            GWAMr.Value = Conversions.ToDecimal(((dynamic)cf).GCRE.GWAM(GWAMcb.SelectedIndex).Range);
            GWAMmin.Value = Conversions.ToDecimal(((dynamic)cf).GCRE.GWAM(GWAMcb.SelectedIndex).MinDmg);
            GWAMmax.Value = Conversions.ToDecimal(((dynamic)cf).GCRE.GWAM(GWAMcb.SelectedIndex).MaxDmg);
            GWAMap.Value = Conversions.ToDecimal(((dynamic)cf).GCRE.GWAM(GWAMcb.SelectedIndex).AP);
            GWAManSR.Value = Conversions.ToDecimal(((dynamic)cf).GCRE.GWAM(GWAMcb.SelectedIndex).NameSR);
            try
            {
                GWAMan.Text = stf[Conversions.ToInteger(Operators.SubtractObject(((dynamic)cf).GCRE.GWAM(GWAMcb.SelectedIndex).NameSR, 1))];
            }
            catch
            {
            }
            GWAMan.Enabled = Conversions.ToBoolean(My.MySettingsProperty.Settings.STFEditEnabled && Operators.ConditionalCompareObjectNotEqual(((dynamic)cf).GCRE.GWAM(GWAMcb.SelectedIndex).NameSR, 0, false));
            GWAMef.Text = Conversions.ToString(((dynamic)cf).GCRE.GWAM(GWAMcb.SelectedIndex).VegName.Replace(".veg", ""));
        }

        private void GCREToUI()
        {
            GCREage.Value = Conversions.ToDecimal(((dynamic)cf).GCRE.Age);
            GCREspcb.SelectedIndex = 0;
            GCREspv.Value = Conversions.ToDecimal(((dynamic)cf).GCRE.Special[0]);
            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectGreater(((dynamic)cf).GCRE.Skills.Count, 0, false)))
            {
                GCREskcb.SelectedIndex = Conversions.ToInteger(((dynamic)cf).GCRE.Skills[0].Index);
                GCREskv.Value = Conversions.ToDecimal(((dynamic)cf).GCRE.Skills[0].Value);
            }
            else
            {
                GCREskcb.SelectedIndex = 0;
            }
            GCREtrcb.SelectedIndex = 0;
            GCREtrv.Checked = Conversions.ToBoolean(((dynamic)cf).GCRE.Traits.Contains(0));
            GCREtscb.SelectedIndex = 0;
            GCREtsv.Checked = Conversions.ToBoolean(((dynamic)cf).GCRE.TagSkills.Contains(0));
            GCREp.Text = Conversions.ToString(((dynamic)cf).GCRE.PortStr.Replace(".dds", ""));
            GCREsoccb.SelectedIndex = 0;
            GCREsocm.Text = Conversions.ToString(((dynamic)cf).GCRE.Hea.Model);
            GCREsoct.Text = Conversions.ToString(((dynamic)cf).GCRE.Hea.Tex.Replace(".dds", ""));
            var dt = new DataTable();
            dt.Columns.Add("item name", typeof(string));
            foreach (var item in (IEnumerable)((dynamic)cf).GCRE.Inventory)
                dt.Rows.Add(item);
            GCREdgv.DataSource = dt;
            foreach (DataGridViewColumn col in GCREdgv.Columns)
            {
                col.MinimumWidth = GCREdgv.Width - 18;
                col.Width = GCREdgv.Width - 18;
            }
        }

        private void GITMToUI()
        {
            GITMtype.SelectedIndex = Conversions.ToInteger(((dynamic)cf).GITM.@type);
            GITMeq.Checked = Conversions.ToBoolean(((dynamic)cf).GITM.equip);
            GITMslot.SelectedIndex = Conversions.ToInteger(((dynamic)cf).GITM.eqslot);
            GITMrl.Text = Conversions.ToString(((dynamic)cf).GITM.reload);
            // GITMml.Checked = cf.GITM.reload =
            GITMhhai.Checked = Conversions.ToBoolean(((dynamic)cf).GITM.hHai);
            GITMhbea.Checked = Conversions.ToBoolean(((dynamic)cf).GITM.hBea);
            GITMhmus.Checked = Conversions.ToBoolean(((dynamic)cf).GITM.hMus);
            GITMheye.Checked = Conversions.ToBoolean(((dynamic)cf).GITM.hEye);
            GITMhpon.Checked = Conversions.ToBoolean(((dynamic)cf).GITM.hPon);
            GITMhvan.Checked = Conversions.ToBoolean(((dynamic)cf).GITM.hVan);
            GITMsoct.Text = Conversions.ToString(((dynamic)cf).GITM.Hea.Tex);
            GITMsocm.Text = Conversions.ToString(((dynamic)cf).GITM.Hea.Model);
        }

        #endregion

        #region Load UI into classes

        private void PickLightingColour(object sender, EventArgs e)
        {
            var cd = new ColorDialog() { Color = (Color)((dynamic)cf).EMAP.col, FullOpen = true };
            if (cd.ShowDialog() == DialogResult.OK)
            {
                EMAPslb.BorderColour = cd.Color;
                ((dynamic)cf).EMAP.col = cd.Color;
            }
        }

        private void EMAPilcb_CheckedChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EMAP.il = EMAPilcb.Checked;
        }

        private void EMAPs1_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EMAP.s1 = EMAPs1.Text + (string.IsNullOrWhiteSpace(EMAPs1.Text) ? "" : ".8");
        }

        private void EMAPs2_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EMAP.s2 = EMAPs2.Text + (string.IsNullOrWhiteSpace(EMAPs2.Text) ? "" : ".rle");
        }

        private void EMAPs3_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EMAP.s3 = EMAPs3.Text + (string.IsNullOrWhiteSpace(EMAPs3.Text) ? "" : ".dds");
        }

        private void ECAMx_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
            {
                if (((dynamic)cf).ECAM is null)
                    ((dynamic)cf).ECAM = new ECAMc();
                ((dynamic)cf).ECAM.p.x = string.IsNullOrEmpty(ECAMx.Text) ? 0 : int.Parse(ECAMx.Text);
            }
        }

        private void ECAMy_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
            {
                if (((dynamic)cf).ECAM is null)
                    ((dynamic)cf).ECAM = new ECAMc();
                ((dynamic)cf).ECAM.p.y = string.IsNullOrEmpty(ECAMy.Text) ? 0 : int.Parse(ECAMy.Text);
            }
        }

        private void ECAMz_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
            {
                if (((dynamic)cf).ECAM is null)
                    ((dynamic)cf).ECAM = new ECAMc();
                ((dynamic)cf).ECAM.p.z = string.IsNullOrEmpty(ECAMz.Text) ? 0 : int.Parse(ECAMz.Text);
            }
        }

        private void ECAMr_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
            {
                if (((dynamic)cf).ECAM is null)
                    ((dynamic)cf).ECAM = new ECAMc();
                ((dynamic)cf).ECAM.p.r = string.IsNullOrEmpty(ECAMr.Text) ? 0 : int.Parse(ECAMr.Text);
            }
        }

        private void EMEFs1_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EMEF(EMEFcb.SelectedIndex).s1 = EMEFs1.Text;
        }

        private void EMEFs2_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EMEF(EMEFcb.SelectedIndex).s2 = EMEFs2.Text + (string.IsNullOrWhiteSpace(EMEFs2.Text) ? "" : ".veg");
        }

        private void EMEFx_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EMEF(EMEFcb.SelectedIndex).l.x = string.IsNullOrEmpty(EMEFx.Text) ? 0 : int.Parse(EMEFx.Text);
        }

        private void EMEFy_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EMEF(EMEFcb.SelectedIndex).l.y = string.IsNullOrEmpty(EMEFy.Text) ? 0 : int.Parse(EMEFy.Text);
        }

        private void EMEFz_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EMEF(EMEFcb.SelectedIndex).l.z = string.IsNullOrEmpty(EMEFz.Text) ? 0 : int.Parse(EMEFz.Text);
        }

        private void EMEFr_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EMEF(EMEFcb.SelectedIndex).l.r = string.IsNullOrEmpty(EMEFr.Text) ? 0 : int.Parse(EMEFr.Text);
        }

        private void EMEPnud_ValueChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EMEP(EMEPcb.SelectedIndex).index = (int)EMEPnud.Value;
        }

        private void EMEPx_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EMEP(EMEPcb.SelectedIndex).p.x = string.IsNullOrEmpty(EMEPx.Text) ? 0 : int.Parse(EMEPx.Text);
        }

        private void EMEPy_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EMEP(EMEPcb.SelectedIndex).p.y = string.IsNullOrEmpty(EMEPy.Text) ? 0 : int.Parse(EMEPy.Text);
        }

        private void EMEPz_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EMEP(EMEPcb.SelectedIndex).p.z = string.IsNullOrEmpty(EMEPz.Text) ? 0 : int.Parse(EMEPz.Text);
        }

        private void EMEPr_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EMEP(EMEPcb.SelectedIndex).r = string.IsNullOrEmpty(EMEPr.Text) ? 0 : int.Parse(EMEPr.Text);
        }

        private void EME2dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EME2(EME2cb.SelectedIndex).EEOV.inv = EME2dgv.GetStringArray();
        }

        private void EME2n_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EME2(EME2cb.SelectedIndex).name = EME2n.Text;
        }

        private void EME2s1_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EME2(EME2cb.SelectedIndex).EEOV.s1 = EME2s1.Text;
        }

        private void EME2s2_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EME2(EME2cb.SelectedIndex).EEOV.s2 = EME2s2.Text;
        }

        private void EME2s3_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EME2(EME2cb.SelectedIndex).EEOV.s3 = EME2s3.Text + (string.IsNullOrWhiteSpace(EME2s3.Text) ? "" : ".amx");
        }

        private void EME2s4_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EME2(EME2cb.SelectedIndex).EEOV.s4 = EME2s4.Text + (string.IsNullOrWhiteSpace(EME2s4.Text) ? "" : ".dds");
        }

        private void EME2s5_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EME2(EME2cb.SelectedIndex).EEOV.s5 = EME2s5.Text + (string.IsNullOrWhiteSpace(EME2s5.Text) ? "" : ".veg");
        }

        private void EME2x_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EME2(EME2cb.SelectedIndex).l.x = string.IsNullOrEmpty(EME2x.Text) ? 0 : int.Parse(EME2x.Text);
        }

        private void EME2y_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EME2(EME2cb.SelectedIndex).l.y = string.IsNullOrEmpty(EME2y.Text) ? 0 : int.Parse(EME2y.Text);
        }

        private void EME2z_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EME2(EME2cb.SelectedIndex).l.z = string.IsNullOrEmpty(EME2z.Text) ? 0 : int.Parse(EME2z.Text);
        }

        private void EME2r_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EME2(EME2cb.SelectedIndex).l.r = string.IsNullOrEmpty(EME2r.Text) ? 0 : int.Parse(EME2r.Text);
        }

        private void EMSDs1_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EMSD(EMSDcb.SelectedIndex).s1 = EMSDs1.Text;
        }

        private void EMSDs2_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EMSD(EMSDcb.SelectedIndex).s2 = EMSDs2.Text + (string.IsNullOrWhiteSpace(EMSDs2.Text) ? "" : ".psf");
        }

        private void EMSDx_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EMSD(EMSDcb.SelectedIndex).l.x = string.IsNullOrEmpty(EMSDx.Text) ? 0 : int.Parse(EMSDx.Text);
        }

        private void EMSDy_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EMSD(EMSDcb.SelectedIndex).l.y = string.IsNullOrEmpty(EMSDy.Text) ? 0 : int.Parse(EMSDy.Text);
        }

        private void EMSDz_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EMSD(EMSDcb.SelectedIndex).l.z = string.IsNullOrEmpty(EMSDz.Text) ? 0 : int.Parse(EMSDz.Text);
        }

        private void EPTHn_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EPTH(EPTHcb.SelectedIndex).name = EPTHn.Text;
        }

        private void EPTHx_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EPTH(EPTHcb.SelectedIndex).p(EPTHnud.Value - 1m).x = string.IsNullOrEmpty(EPTHx.Text) ? 0 : int.Parse(EPTHx.Text);
        }

        private void EPTHy_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EPTH(EPTHcb.SelectedIndex).p(EPTHnud.Value - 1m).y = string.IsNullOrEmpty(EPTHy.Text) ? 0 : int.Parse(EPTHy.Text);
        }

        private void EPTHz_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EPTH(EPTHcb.SelectedIndex).p(EPTHnud.Value - 1m).z = string.IsNullOrEmpty(EPTHz.Text) ? 0 : int.Parse(EPTHz.Text);
        }

        private void EPTHr_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EPTH(EPTHcb.SelectedIndex).p(EPTHnud.Value - 1m).r = string.IsNullOrEmpty(EPTHr.Text) ? 0 : int.Parse(EPTHr.Text);
        }

        private void Triggern_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).Triggers(Triggercb.SelectedIndex).ExTR.s = Triggern.Text;
        }

        private void Triggertcb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).Triggers(Triggercb.SelectedIndex).ExTR.@type = Triggertcb.SelectedItem;
        }

        private void Triggerx_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).Triggers(Triggercb.SelectedIndex).EMTR.r(Triggernud.Value - 1m).x = string.IsNullOrEmpty(Triggerx.Text) ? 0 : int.Parse(Triggerx.Text);
        }

        private void Triggery_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).Triggers(Triggercb.SelectedIndex).EMTR.r(Triggernud.Value - 1m).y = string.IsNullOrEmpty(Triggery.Text) ? 0 : int.Parse(Triggery.Text);
        }

        private void Triggerz_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).Triggers(Triggercb.SelectedIndex).EMTR.r(Triggernud.Value - 1m).z = string.IsNullOrEmpty(Triggerz.Text) ? 0 : int.Parse(Triggerz.Text);
        }

        private void GENThSR_ValueChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GENT.HoverSR = (int)GENThSR.Value;
            if (GENThSR.Value == 0m)
            {
                GENTh.Enabled = false;
                GENTh.Text = "";
            }
            else
            {
                GENTh.Enabled = My.MySettingsProperty.Settings.STFEditEnabled;
                if (GENThSR.Value - 1m < stf.Count)
                    stf.Add("");
                GENTh.Text = stf[(int)Math.Round(GENThSR.Value - 1m)];
            }
        }

        private void GENTlSR_ValueChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GENT.LookSR = (int)GENTlSR.Value;
            if (GENTlSR.Value == 0m)
            {
                GENTl.Enabled = false;
                GENTl.Text = "";
            }
            else
            {
                GENTl.Enabled = My.MySettingsProperty.Settings.STFEditEnabled;
                if (GENTlSR.Value - 1m < stf.Count)
                    stf.Add("");
                GENTl.Text = stf[(int)Math.Round(GENTlSR.Value - 1m)];
            }
        }

        private void GENTnSR_ValueChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GENT.NameSR = (int)GENTnSR.Value;
            if (GENTnSR.Value == 0m)
            {
                GENTn.Enabled = false;
                GENTn.Text = "";
            }
            else
            {
                GENTn.Enabled = My.MySettingsProperty.Settings.STFEditEnabled;
                if (GENTnSR.Value - 1m < stf.Count)
                    stf.Add("");
                GENTn.Text = stf[(int)Math.Round(GENTnSR.Value - 1m)];
            }
        }

        private void GENTuSR_ValueChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GENT.UnkwnSR = (int)GENTuSR.Value;
            if (GENTuSR.Value == 0m)
            {
                GENTu.Enabled = false;
                GENTu.Text = "";
            }
            else
            {
                GENTu.Enabled = My.MySettingsProperty.Settings.STFEditEnabled;
                if (GENTuSR.Value - 1m < stf.Count)
                    stf.Add("");
                GENTu.Text = stf[(int)Math.Round(GENTuSR.Value - 1m)];
            }
        }

        private void GWAManSR_ValueChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GCRE.GWAM(GWAMcb.SelectedIndex).NameSR = (int)GWAManSR.Value;
            if (GWAManSR.Value == 0m)
            {
                GWAMan.Enabled = false;
                GWAMan.Text = "";
            }
            else
            {
                GWAMan.Enabled = My.MySettingsProperty.Settings.STFEditEnabled;
                if (GWAManSR.Value - 1m < stf.Count)
                    stf.Add("");
                GWAMan.Text = stf[(int)Math.Round(GWAManSR.Value - 1m)];
            }
        }

        private void EEN2skl_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EEN2.skl = EEN2skl.Text;
        }

        private void EEN2invt_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EEN2.invtex = EEN2invt.Text + (string.IsNullOrWhiteSpace(EEN2invt.Text) ? "" : ".dds");
        }

        private void EEN2actt_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EEN2.acttex = EEN2actt.Text + (string.IsNullOrWhiteSpace(EEN2actt.Text) ? "" : ".dds");
        }

        private void EEN2s1_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EEN2.EEOV.s1 = EEN2s1.Text;
        }

        private void EEN2s2_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EEN2.EEOV.s2 = EEN2s2.Text;
        }

        private void EEN2s3_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EEN2.EEOV.s3 = EEN2s3.Text + (string.IsNullOrWhiteSpace(EEN2s3.Text) ? "" : ".amx");
        }

        private void EEN2s4_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EEN2.EEOV.s4 = EEN2s4.Text + (string.IsNullOrWhiteSpace(EEN2s4.Text) ? "" : ".dds");
        }

        private void EEN2s5_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EEN2.EEOV.s5 = EEN2s5.Text + (string.IsNullOrWhiteSpace(EEN2s5.Text) ? "" : ".veg");
        }

        private void EEN2sel_CheckedChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EEN2.sel = EEN2sel.Checked;
        }

        private void EEN2dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).EEN2.EEOV.inv = EEN2dgv.GetStringArray();
        }

        private void GENTmhp_ValueChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GENT.MaxHealth = (int)GENTmhp.Value;
        }

        private void GENTihp_ValueChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GENT.StartHealth = (int)GENTihp.Value;
        }

        private void GENTh_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                stf[(int)Math.Round(GENThSR.Value - 1m)] = GENTh.Text;
        }

        private void GENTl_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                stf[(int)Math.Round(GENTlSR.Value - 1m)] = GENTl.Text;
        }

        private void GENTn_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                stf[(int)Math.Round(GENTnSR.Value - 1m)] = GENTn.Text;
        }

        private void GENTu_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                stf[(int)Math.Round(GENTuSR.Value - 1m)] = GENTu.Text;
        }

        private void GWAMan_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                stf[(int)Math.Round(GWAManSR.Value - 1m)] = GWAMan.Text;
        }

        private void GCREspcb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                GCREspv.Value = Conversions.ToDecimal(((dynamic)cf).GCRE.Special[GCREspcb.SelectedIndex]);
        }

        private void GCREspv_ValueChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GCRE.Special[GCREspcb.SelectedIndex] = (int)GCREspv.Value;
        }

        private void GCREskcb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
            {
                var existingSkill = ((List<Skill>)((dynamic)cf).GCRE.Skills).FirstOrDefault(sk => sk.Index == GCREskcb.SelectedIndex, null); // Determine whether or not the list of skills contains a skill with the current index
                GCREskv.Value = existingSkill != null ? (decimal)Conversions.ToDecimal(((dynamic)cf).GCRE.Skills(((dynamic)cf).GCRE.Skills.IndexOf(existingSkill)).Value) : 0m;
            }
        }

        private void GCREskv_ValueChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
            {
                var existingSkill = ((List<Skill>)((dynamic)cf).GCRE.Skills).FirstOrDefault(sk => sk.Index == GCREskcb.SelectedIndex, null);
                if (existingSkill != null)                                                                            
                {
                    var skillIndex = ((dynamic)cf).GCRE.Skills.IndexOf(existingSkill);
                    if (GCREskv.Value == 0m)
                    {
                        ((dynamic)cf).GCRE.Skills.RemoveAt(skillIndex);
                    }
                    else
                    {
                        ((dynamic)cf).GCRE.Skills(skillIndex).Value = (int)GCREskv.Value;
                    }
                }
                else if (GCREskv.Value != 0m)
                    ((dynamic)cf).GCRE.Skills.Add(new Skill(GCREskcb.SelectedIndex, (int)Math.Round(GCREskv.Value)));
            }
        }

        private void GCREtrcb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                GCREtrv.Checked = Conversions.ToBoolean(((dynamic)cf).GCRE.Traits.Contains(GCREtrcb.SelectedIndex));
        }

        private void GCREtrv_CheckedChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
            {
                if (GCREtrv.Checked)
                {
                    if (Conversions.ToBoolean(!((dynamic)cf).GCRE.Traits.Contains(GCREtrcb.SelectedIndex)))
                        ((dynamic)cf).GCRE.Traits.Add(GCREtrcb.SelectedIndex);
                }
                else
                {
                    ((dynamic)cf).GCRE.Traits.Remove(GCREtrcb.SelectedIndex);
                }
            }
        }

        private void GCREtscb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                GCREtsv.Checked = Conversions.ToBoolean(((dynamic)cf).GCRE.TagSkills.Contains(GCREtscb.SelectedIndex));
        }

        private void GCREtsv_CheckedChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
            {
                if (GCREtsv.Checked)
                {
                    if (Conversions.ToBoolean(!((dynamic)cf).GCRE.TagSkills.Contains(GCREtscb.SelectedIndex)))
                        ((dynamic)cf).GCRE.TagSkills.Add(GCREtscb.SelectedIndex);
                }
                else
                {
                    ((dynamic)cf).GCRE.TagSkills.Remove(GCREtscb.SelectedIndex);
                }
            }
        }

        private void GCREp_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GCRE.PortStr = GCREp.Text + (string.IsNullOrWhiteSpace(GCREp.Text) ? "" : ".dds");
        }

        private void GCREsoccb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
            {
                switch (GCREsoccb.SelectedItem)
                {
                    case "Head":
                        {
                            GCREsocm.Text = Conversions.ToString(((dynamic)cf).GCRE.Hea.Model);
                            GCREsoct.Text = Conversions.ToString(((dynamic)cf).GCRE.Hea.Tex.Replace(".dds", ""));
                            break;
                        }

                    case "Hair":
                        {
                            GCREsocm.Text = Conversions.ToString(((dynamic)cf).GCRE.Hai.Model);
                            GCREsoct.Text = Conversions.ToString(((dynamic)cf).GCRE.Hai.Tex.Replace(".dds", ""));
                            break;
                        }

                    case "Ponytail":
                        {
                            GCREsocm.Text = Conversions.ToString(((dynamic)cf).GCRE.Pon.Model);
                            GCREsoct.Text = Conversions.ToString(((dynamic)cf).GCRE.Pon.Tex.Replace(".dds", ""));
                            break;
                        }

                    case "Moustache":
                        {
                            GCREsocm.Text = Conversions.ToString(((dynamic)cf).GCRE.Mus.Model);
                            GCREsoct.Text = Conversions.ToString(((dynamic)cf).GCRE.Mus.Tex.Replace(".dds", ""));
                            break;
                        }

                    case "Beard":
                        {
                            GCREsocm.Text = Conversions.ToString(((dynamic)cf).GCRE.Bea.Model);
                            GCREsoct.Text = Conversions.ToString(((dynamic)cf).GCRE.Bea.Tex.Replace(".dds", ""));
                            break;
                        }

                    case "Eye":
                        {
                            GCREsocm.Text = Conversions.ToString(((dynamic)cf).GCRE.Eye.Model);
                            GCREsoct.Text = Conversions.ToString(((dynamic)cf).GCRE.Eye.Tex.Replace(".dds", ""));
                            break;
                        }

                    case "Body":
                        {
                            GCREsocm.Text = Conversions.ToString(((dynamic)cf).GCRE.Bod.Model);
                            GCREsoct.Text = Conversions.ToString(((dynamic)cf).GCRE.Bod.Tex.Replace(".dds", ""));
                            break;
                        }

                    case "Hand":
                        {
                            GCREsocm.Text = Conversions.ToString(((dynamic)cf).GCRE.Han.Model);
                            GCREsoct.Text = Conversions.ToString(((dynamic)cf).GCRE.Han.Tex.Replace(".dds", ""));
                            break;
                        }

                    case "Feet":
                        {
                            GCREsocm.Text = Conversions.ToString(((dynamic)cf).GCRE.Fee.Model);
                            GCREsoct.Text = Conversions.ToString(((dynamic)cf).GCRE.Fee.Tex.Replace(".dds", ""));
                            break;
                        }

                    case "Back":
                        {
                            GCREsocm.Text = Conversions.ToString(((dynamic)cf).GCRE.Bac.Model);
                            GCREsoct.Text = Conversions.ToString(((dynamic)cf).GCRE.Bac.Tex.Replace(".dds", ""));
                            break;
                        }

                    case "Shoulder":
                        {
                            GCREsocm.Text = Conversions.ToString(((dynamic)cf).GCRE.Sho.Model);
                            GCREsoct.Text = Conversions.ToString(((dynamic)cf).GCRE.Sho.Tex.Replace(".dds", ""));
                            break;
                        }

                    case "Vanity":
                        {
                            GCREsocm.Text = Conversions.ToString(((dynamic)cf).GCRE.Van.Model);
                            GCREsoct.Text = Conversions.ToString(((dynamic)cf).GCRE.Van.Tex.Replace(".dds", ""));
                            break;
                        }
                }
            }
        }

        private void GCREsocm_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
            {
                switch (GCREsoccb.SelectedItem)
                {
                    case "Head":
                        {
                            ((dynamic)cf).GCRE.Hea.Model = GCREsocm.Text;
                            break;
                        }

                    case "Hair":
                        {
                            ((dynamic)cf).GCRE.Hai.Model = GCREsocm.Text;
                            break;
                        }

                    case "Ponytail":
                        {
                            ((dynamic)cf).GCRE.Pon.Model = GCREsocm.Text;
                            break;
                        }

                    case "Moustache":
                        {
                            ((dynamic)cf).GCRE.Mus.Model = GCREsocm.Text;
                            break;
                        }

                    case "Beard":
                        {
                            ((dynamic)cf).GCRE.Bea.Model = GCREsocm.Text;
                            break;
                        }

                    case "Eye":
                        {
                            ((dynamic)cf).GCRE.Eye.Model = GCREsocm.Text;
                            break;
                        }

                    case "Body":
                        {
                            ((dynamic)cf).GCRE.Bod.Model = GCREsocm.Text;
                            break;
                        }

                    case "Hand":
                        {
                            ((dynamic)cf).GCRE.Han.Model = GCREsocm.Text;
                            break;
                        }

                    case "Feet":
                        {
                            ((dynamic)cf).GCRE.Fee.Model = GCREsocm.Text;
                            break;
                        }

                    case "Back":
                        {
                            ((dynamic)cf).GCRE.Bac.Model = GCREsocm.Text;
                            break;
                        }

                    case "Shoulder":
                        {
                            ((dynamic)cf).GCRE.Sho.Model = GCREsocm.Text;
                            break;
                        }

                    case "Vanity":
                        {
                            ((dynamic)cf).GCRE.Van.Model = GCREsocm.Text;
                            break;
                        }
                }
            }
        }

        private void GCREsoct_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
            {
                string s = GCREsoct.Text + (string.IsNullOrWhiteSpace(GCREsoct.Text) ? "" : ".dds");
                switch (GCREsoccb.SelectedItem)
                {
                    case "Head":
                        {
                            ((dynamic)cf).GCRE.Hea.Tex = s;
                            break;
                        }

                    case "Hair":
                        {
                            ((dynamic)cf).GCRE.Hai.Tex = s;
                            break;
                        }

                    case "Ponytail":
                        {
                            ((dynamic)cf).GCRE.Pon.Tex = s;
                            break;
                        }

                    case "Moustache":
                        {
                            ((dynamic)cf).GCRE.Mus.Tex = s;
                            break;
                        }

                    case "Beard":
                        {
                            ((dynamic)cf).GCRE.Bea.Tex = s;
                            break;
                        }

                    case "Eye":
                        {
                            ((dynamic)cf).GCRE.Eye.Tex = s;
                            break;
                        }

                    case "Body":
                        {
                            ((dynamic)cf).GCRE.Bod.Tex = s;
                            break;
                        }

                    case "Hand":
                        {
                            ((dynamic)cf).GCRE.Han.Tex = s;
                            break;
                        }

                    case "Feet":
                        {
                            ((dynamic)cf).GCRE.Fee.Tex = s;
                            break;
                        }

                    case "Back":
                        {
                            ((dynamic)cf).GCRE.Bac.Tex = s;
                            break;
                        }

                    case "Shoulder":
                        {
                            ((dynamic)cf).GCRE.Sho.Tex = s;
                            break;
                        }

                    case "Vanity":
                        {
                            ((dynamic)cf).GCRE.Van.Tex = s;
                            break;
                        }
                }
            }
        }

        private void GCREdgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GCRE.Inventory = GCREdgv.GetStringArray();
        }

        private void GCHRn_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GCHR.name = GCHRn.Text;
        }

        private void GWAMani_ValueChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GCRE.GWAM(GWAMcb.SelectedIndex).Anim = (int)GWAMani.Value;
        }

        private void GWAMdt_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GCRE.GWAM(GWAMcb.SelectedIndex).DmgType = GWAMdt.SelectedIndex;
        }

        private void GWAMsf_ValueChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GCRE.GWAM(GWAMcb.SelectedIndex).ShotsFired = (int)GWAMsf.Value;
        }

        private void GWAMr_ValueChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GCRE.GWAM(GWAMcb.SelectedIndex).Range = (int)GWAMr.Value;
        }

        private void GWAMmin_ValueChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GCRE.GWAM(GWAMcb.SelectedIndex).MinDmg = (int)GWAMmin.Value;
        }

        private void GWAMmax_ValueChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GCRE.GWAM(GWAMcb.SelectedIndex).MaxDmg = (int)GWAMmax.Value;
        }

        private void GWAMap_ValueChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GCRE.GWAM(GWAMcb.SelectedIndex).AP = (int)GWAMap.Value;
        }

        private void GWAMef_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GCRE.GWAM(GWAMcb.SelectedIndex).VegName = GWAMef.Text;
        }

        private void GCREage_ValueChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GCRE.Age = (int)GCREage.Value;
        }

        private void _2MWTcb_SelectedIndexChanged(object sender, EventArgs e)
        {
            _2MWTx.Text = Conversions.ToString(((dynamic)cf)._2MWT.chunks(_2MWTcb.SelectedIndex).loc.x);
            _2MWTy.Text = Conversions.ToString(((dynamic)cf)._2MWT.chunks(_2MWTcb.SelectedIndex).loc.y);
            _2MWTz.Text = Conversions.ToString(((dynamic)cf)._2MWT.chunks(_2MWTcb.SelectedIndex).loc.z);
            try
            {
                _2MWTtex.Text = Conversions.ToString(((dynamic)cf)._2MWT.chunks(_2MWTcb.SelectedIndex).tex.Substring(0, ((dynamic)cf)._2MWT.chunks(_2MWTcb.SelectedIndex).tex.LastIndexOf(".")));
            }
            catch
            {
                _2MWTtex.Text = "";
            }
            _2MWTlmx.Text = Conversions.ToString(((dynamic)cf)._2MWT.chunks(_2MWTcb.SelectedIndex).texloc.x);
            _2MWTlmy.Text = Conversions.ToString(((dynamic)cf)._2MWT.chunks(_2MWTcb.SelectedIndex).texloc.y);
        }

        private void _2MWTmpf_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf)._2MWT.mpf = _2MWTmpf.Text;
        }

        private void _2MWTfr_CheckedChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf)._2MWT.frozen = ((dynamic)sender).@checked;
        }

        private void _2MWTdw_CheckedChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf)._2MWT.dark = ((dynamic)sender).@checked;
        }

        private void _2MWTtex_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf)._2MWT.chunks(_2MWTcb.SelectedIndex).tex = _2MWTtex.Text + ".dds";
        }

        private void _2MWTx_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf)._2MWT.chunks(_2MWTcb.SelectedIndex).loc.x = string.IsNullOrEmpty(_2MWTx.Text) ? 0 : int.Parse(_2MWTx.Text);
        }

        private void _2MWTy_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf)._2MWT.chunks(_2MWTcb.SelectedIndex).loc.y = string.IsNullOrEmpty(_2MWTy.Text) ? 0 : int.Parse(_2MWTy.Text);
        }

        private void _2MWTz_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf)._2MWT.chunks(_2MWTcb.SelectedIndex).loc.z = string.IsNullOrEmpty(_2MWTz.Text) ? 0 : int.Parse(_2MWTz.Text);
        }

        private void _2MWTlmx_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf)._2MWT.chunks(_2MWTcb.SelectedIndex).texloc.x = _2MWTlmx.Text;
        }

        private void _2MWTlmy_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf)._2MWT.chunks(_2MWTcb.SelectedIndex).texloc.y = _2MWTlmy.Text;
        }

        // Prevent invalid floats from being entered into text boxes (Add any float textboxes to the "Handles" section)
        private static void FloatsOnly(object? s, KeyPressEventArgs e)
        {
            var sender = (TextBox)s;
            if (char.IsDigit(e.KeyChar) | Conversions.ToString(e.KeyChar) == "." | Conversions.ToString(e.KeyChar) == "-" | e.KeyChar == '\b')
            {
                string text = sender.Text;
                if (Conversions.ToString(e.KeyChar) == ".")
                {
                    if (text.IndexOf(".") > -1)
                        e.Handled = true;
                }
                else if (Conversions.ToString(e.KeyChar) == "-")
                {
                    if (text.IndexOf("-") > -1 | sender.SelectionStart != 0)
                        e.Handled = true;
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        // Prevent invalid ints from being entered into text boxes (Add any numeric textboxes to the "Handles" section)
        private void IntsOnly(TextBox sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        #endregion

        #region Scroll Bars

        private void EME2dgv_ScrollChanged()
        {
            EME2dsb.ScrollTo((int)Math.Round(EME2dgv.FirstDisplayedScrollingRowIndex / (double)(EME2dgv.Rows.Count - EME2dgv.DisplayedRowCount(false)) * EME2dsb.Maximum));
        }

        private void EME2dsb_Click(object sender, EventArgs e)
        {
            EME2tmr.Start();
        }

        private void EME2dsb_MouseUp(object sender, EventArgs e)
        {
            EME2tmr.Stop();
        }

        private void EME2tmr_Tick(object sender, EventArgs e)
        {
            EME2dgv.FirstDisplayedScrollingRowIndex = (int)Math.Round(EME2dsb.Value / (double)EME2dsb.Maximum * (EME2dgv.Rows.Count - EME2dgv.DisplayedRowCount(false)));
        }

        private void EEN2dgv_ScrollChanged()
        {
            EEN2dsb.ScrollTo((int)Math.Round(EEN2dgv.FirstDisplayedScrollingRowIndex / (double)(EEN2dgv.Rows.Count - EEN2dgv.DisplayedRowCount(false)) * EEN2dsb.Maximum));
        }

        private void EEN2dsb_Click(object sender, EventArgs e)
        {
            EEN2tmr.Start();
        }

        private void EEN2dsb_MouseUp(object sender, EventArgs e)
        {
            EEN2tmr.Stop();
        }

        private void EEN2tmr_Tick(object sender, EventArgs e)
        {
            EEN2dgv.FirstDisplayedScrollingRowIndex = (int)Math.Round(EEN2dsb.Value / (double)EEN2dsb.Maximum * (EEN2dgv.Rows.Count - EEN2dgv.DisplayedRowCount(false)));
        }

        private void GCREdgv_ScrollChanged()
        {
            GCREdsb.ScrollTo((int)Math.Round(GCREdgv.FirstDisplayedScrollingRowIndex / (double)(GCREdgv.Rows.Count - GCREdgv.DisplayedRowCount(false)) * GCREdsb.Maximum));
        }

        private void GCREdsb_Click(object sender, EventArgs e)
        {
            GCREtmr.Start();
        }

        private void GCREdsb_MouseUp(object sender, EventArgs e)
        {
            GCREtmr.Stop();
        }

        private void GCREtmr_Tick(object sender, EventArgs e)
        {
            GCREdgv.FirstDisplayedScrollingRowIndex = (int)Math.Round(GCREdsb.Value / (double)GCREdsb.Maximum * (GCREdgv.Rows.Count - GCREdgv.DisplayedRowCount(false)));
        }

        #endregion

        #region Remove/Add Chunks

        private void EMEPp_Click(object sender, EventArgs e)
        {
            foreach (Control c in EMEPgb.Controls)
                c.Enabled = true;
            ((dynamic)cf).EMEP.Add(new EMEPc());
            EMEPcb.Items.Add(EMEPcb.Items.Count + 1);
            EMEPcb.SelectedIndex = EMEPcb.Items.Count - 1;
        }

        private void EMEPm_Click(object sender, EventArgs e)
        {
            int i = EMEPcb.SelectedIndex;
            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(((dynamic)cf).EMEP.Count, 1, false)))
            {
                ((dynamic)cf).EMEP = new List<EMEPc>();
                Map argcf = (Map)cf;
                MapSetupUI(ref argcf);
                cf = argcf;
                foreach (Control c in EMEPgb.Controls)
                    c.Enabled = false;
                EMEPcb.Enabled = true;
                EMEPp.Enabled = true;
            }
            else
            {
                ((dynamic)cf).EMEP.RemoveAt(i);
                EMEPcb.Items.RemoveAt(i);
                EMEPcb.SelectedIndex = i == 0 ? 0 : i - 1;
            }
        }

        private void EME2p_Click(object sender, EventArgs e)
        {
            foreach (Control c in EME2gb.Controls)
                c.Enabled = true;
            ((dynamic)cf).EME2.Add(new EME2c());
            EME2cb.Items.Add(EME2cb.Items.Count + 1);
            EME2cb.SelectedIndex = EME2cb.Items.Count - 1;
        }

        private void EME2m_Click(object sender, EventArgs e)
        {
            int i = EME2cb.SelectedIndex;
            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(((dynamic)cf).EME2.Count, 1, false)))
            {
                ((dynamic)cf).EME2 = new List<EME2c>();
                Map argcf = (Map)cf;
                MapSetupUI(ref argcf);
                cf = argcf;
                foreach (Control c in EME2gb.Controls)
                    c.Enabled = false;
                EME2cb.Enabled = true;
                EME2p.Enabled = true;
            }
            else
            {
                ((dynamic)cf).EME2.RemoveAt(i);
                EME2cb.Items.RemoveAt(i);
                EME2cb.SelectedIndex = i == 0 ? 0 : i - 1;
            }
        }

        private void EMEFm_Click(object sender, EventArgs e)
        {
            int i = EMEFcb.SelectedIndex;
            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(((dynamic)cf).EMEF.Count, 1, false)))
            {
                ((dynamic)cf).EMEF = new List<EMEFc>();
                Map argcf = (Map)cf;
                MapSetupUI(ref argcf);
                cf = argcf;
                foreach (Control c in EMEFgb.Controls)
                    c.Enabled = false;
                EMEFcb.Enabled = true;
                EMEFp.Enabled = true;
            }
            else
            {
                ((dynamic)cf).EMEF.RemoveAt(i);
                EMEFcb.Items.RemoveAt(i);
                EMEFcb.SelectedIndex = i == 0 ? 0 : i - 1;
            }
        }

        private void EMEFp_Click(object sender, EventArgs e)
        {
            foreach (Control c in EMEFgb.Controls)
                c.Enabled = true;
            ((dynamic)cf).EMEF.Add(new EMEFc());
            EMEFcb.Items.Add(EMEFcb.Items.Count + 1);
            EMEFcb.SelectedIndex = EMEFcb.Items.Count - 1;
        }

        private void EMSDm_Click(object sender, EventArgs e)
        {
            int i = EMSDcb.SelectedIndex;
            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(((dynamic)cf).EMSD.Count, 1, false)))
            {
                ((dynamic)cf).EMSD = new List<EMSDc>();
                Map argcf = (Map)cf;
                MapSetupUI(ref argcf);
                cf = argcf;
                foreach (Control c in EMSDgb.Controls)
                    c.Enabled = false;
                EMSDcb.Enabled = true;
                EMSDp.Enabled = true;
            }
            else
            {
                ((dynamic)cf).EMSD.RemoveAt(i);
                EMSDcb.Items.RemoveAt(i);
                EMSDcb.SelectedIndex = i == 0 ? 0 : i - 1;
            }
        }

        private void EMSDp_Click(object sender, EventArgs e)
        {
            foreach (Control c in EMSDgb.Controls)
                c.Enabled = true;
            ((dynamic)cf).EMSD.Add(new EMSDc());
            EMSDcb.Items.Add(EMSDcb.Items.Count + 1);
            EMSDcb.SelectedIndex = EMSDcb.Items.Count - 1;
        }

        private void EPTHm_Click(object sender, EventArgs e)
        {
            int i = EPTHcb.SelectedIndex;
            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(((dynamic)cf).EPTH.Count, 1, false)))
            {
                ((dynamic)cf).EPTH = new List<EPTHc>();
                Map argcf = (Map)cf;
                MapSetupUI(ref argcf);
                cf = argcf;
                foreach (Control c in EPTHGB.Controls)
                    c.Enabled = false;
                EPTHcb.Enabled = true;
                EPTHp.Enabled = true;
            }
            else
            {
                ((dynamic)cf).EPTH.RemoveAt(i);
                EPTHcb.Items.RemoveAt(i);
                EPTHcb.SelectedIndex = i == 0 ? 0 : i - 1;
            }
        }

        private void EPTHp_Click(object sender, EventArgs e)
        {
            foreach (Control c in EPTHGB.Controls)
                c.Enabled = true;
            ((dynamic)cf).EPTH.Add(new EPTHc());
            EPTHcb.Items.Add(EPTHcb.Items.Count + 1);
            EPTHcb.SelectedIndex = EPTHcb.Items.Count - 1;
        }

        private void Triggerm_Click(object sender, EventArgs e)
        {
            int i = Triggercb.SelectedIndex;
            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(((dynamic)cf).Triggers.Count, 1, false)))
            {
                ((dynamic)cf).Triggers = new List<Trigger>();
                Map argcf = (Map)cf;
                MapSetupUI(ref argcf);
                cf = argcf;
                foreach (Control c in Triggergb.Controls)
                    c.Enabled = false;
                Triggercb.Enabled = true;
                Triggerp.Enabled = true;
            }
            else
            {
                ((dynamic)cf).Triggers.RemoveAt(i);
                Triggercb.Items.RemoveAt(i);
                Triggercb.SelectedIndex = i == 0 ? 0 : i - 1;
            }
        }

        private void Triggerp_Click(object sender, EventArgs e)
        {
            foreach (Control c in Triggergb.Controls)
                c.Enabled = true;
            ((dynamic)cf).Triggers.Add(new Trigger());
            Triggercb.Items.Add(Triggercb.Items.Count + 1);
            Triggercb.SelectedIndex = Triggercb.Items.Count - 1;
        }

        private void GWAMm_Click(object sender, EventArgs e)
        {
            int i = GWAMcb.SelectedIndex;
            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(((dynamic)cf).GCRE.GWAM.Count, 1, false)))
            {
                ((dynamic)cf).GCRE.GWAM = new List<GWAMc>();
                CRT argcf = (CRT)cf;
                CRTSetupUI(ref argcf);
                cf = argcf;
                foreach (Control c in GWAMgb.Controls)
                    c.Enabled = false;
                GWAMcb.Enabled = true;
                GWAMp.Enabled = true;
            }
            else
            {
                ((dynamic)cf).GCRE.GWAM.RemoveAt(i);
                GWAMcb.Items.RemoveAt(i);
                GWAMcb.SelectedIndex = i == 0 ? 0 : i - 1;
            }
        }

        private void GWAMp_Click(object sender, EventArgs e)
        {
            foreach (Control c in GWAMgb.Controls)
                c.Enabled = true;
            ((dynamic)cf).GCRE.GWAM.Add(new GWAMc());
            GWAMcb.Items.Add(GWAMcb.Items.Count + 1);
            GWAMcb.SelectedIndex = GWAMcb.Items.Count - 1;
            GWAMToUI();
        }

        private void _2MWTp_Click(object sender, EventArgs e)
        {
            if (((dynamic)cf)._2MWT is null)
                ((dynamic)cf)._2MWT = new _2MWTc();
            foreach (Control c in _2MWTgb.Controls)
                c.Enabled = true;
            ((dynamic)cf)._2MWT.chunks.Add(new _2MWTChunk("", new Point3(0f, 0f, 0f), new Point2(0f, 0f)));
            _2MWTcb.Items.Add(_2MWTcb.Items.Count + 1);
            _2MWTcb.SelectedIndex = _2MWTcb.Items.Count - 1;
        }

        private void _2MWTm_Click(object sender, EventArgs e)
        {
            int i = _2MWTcb.SelectedIndex;
            int co = Conversions.ToInteger(((dynamic)cf)._2MWT.chunks.Count);
            if (co == 1)
            {
                ((dynamic)cf)._2MWT.chunks = new List<_2MWTChunk>();
                Map argcf = (Map)cf;
                MapSetupUI(ref argcf);
                cf = argcf;
                foreach (Control c in _2MWTgb.Controls)
                    c.Enabled = false;
                _2MWTcb.Enabled = true;
                _2MWTp.Enabled = true;
            }
            else
            {
                ((dynamic)cf)._2MWT.chunks.RemoveAt(i);
                _2MWTcb.Items.RemoveAt(i);
                _2MWTcb.SelectedIndex = Math.Max(0, i - 1);
            }
        }

        #endregion

        #region .stf Stuff

        private void FullSTFToText()
        {
            var ofd = new OpenFileDialog() { Filter = "String Table File|*.stf", Multiselect = false };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var sfd = new SaveFileDialog() { Filter = "Text File|*.txt" };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllLines(sfd.FileName, Extensions.STFToTXT(File.ReadAllBytes(ofd.FileName)));
                }
            }
        }

        private void FullTextToSTF()
        {
            var ofd = new OpenFileDialog() { Filter = "Text File|*.txt", Multiselect = false };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var sfd = new SaveFileDialog() { Filter = "String Table File|*.stf" };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sfd.FileName, Extensions.TXTToSTF(File.ReadAllLines(ofd.FileName)));
                }
            }
        }

        private bool CheckAndLoadStf()
        {
            if (string.IsNullOrEmpty(My.MySettingsProperty.Settings.STFDir))
            {
                if (DarkMessageBox.ShowInformation("English.STF Location not set, please locate it.", "English.stf Not Selected") == DialogResult.OK)
                {
                    SetEngStfLocation();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (!File.Exists(My.MySettingsProperty.Settings.STFDir))
            {
                if (DarkMessageBox.ShowInformation("Previous English.STF not found, please select a new one.", "English.stf Not Found") == DialogResult.OK)
                {
                    SetEngStfLocation();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                stf = (List<string>)Extensions.STFToTXT(File.ReadAllBytes(My.MySettingsProperty.Settings.STFDir));
                return true;
            }
        }

        private void SetEngStfLocation()
        {
            var ofd = new OpenFileDialog() { Multiselect = false, CheckFileExists = true, Filter = "English.stf|*.stf" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                My.MySettingsProperty.Settings.STFDir = ofd.FileName;
                stf = (List<string>)Extensions.STFToTXT(File.ReadAllBytes(ofd.FileName));
            }
        }

        private void EnableEnglishstfEditingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            My.MySettingsProperty.Settings.STFEditEnabled = EnableSTFEdit.Checked;
        }

        #endregion

        #region Custom Accelerators

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ControlKey:
                    {
                        HCtrl = true;
                        break;
                    }
                case Keys.N:
                    {
                        HN = true;
                        break;
                    }
                case Keys.D1:
                    {
                        H1 = true;
                        break;
                    }
                case Keys.D2:
                    {
                        H2 = true;
                        break;
                    }
                case Keys.D3:
                    {
                        H3 = true;
                        break;
                    }
                case Keys.D4:
                    {
                        H4 = true;
                        break;
                    }
                case Keys.D5:
                    {
                        H5 = true;
                        break;
                    }
                case Keys.D6:
                    {
                        H6 = true;
                        break;
                    }
                case Keys.D7:
                    {
                        H7 = true;
                        break;
                    }
                case Keys.D8:
                    {
                        H8 = true;
                        break;
                    }
                case Keys.D9:
                    {
                        H9 = true;
                        break;
                    }
                case Keys.D0:
                    {
                        H0 = true;
                        break;
                    }
            }
            if (HCtrl && HN)
            {
                if (H1)
                {
                    NewAmo();
                }
                else if (H2)
                {
                    NewArm();
                }
                else if (H3)
                {
                    NewCon();
                }
                else if (H4)
                {
                    NewCrt();
                }
                else if (H5)
                {
                    NewDor();
                }
                else if (H6)
                {
                    NewInt();
                }
                else if (H7)
                {
                    NewItm();
                }
                else if (H8)
                {
                    NewMap();
                }
                else if (H9)
                {
                    NewUse();
                }
                else if (H0)
                {
                    NewWea();
                }
            }
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ControlKey:
                    {
                        HCtrl = false;
                        break;
                    }
                case Keys.N:
                    {
                        HN = false;
                        break;
                    }
                case Keys.D1:
                    {
                        H1 = false;
                        break;
                    }
                case Keys.D2:
                    {
                        H2 = false;
                        break;
                    }
                case Keys.D3:
                    {
                        H3 = false;
                        break;
                    }
                case Keys.D4:
                    {
                        H4 = false;
                        break;
                    }
                case Keys.D5:
                    {
                        H5 = false;
                        break;
                    }
                case Keys.D6:
                    {
                        H6 = false;
                        break;
                    }
                case Keys.D7:
                    {
                        H7 = false;
                        break;
                    }
                case Keys.D8:
                    {
                        H8 = false;
                        break;
                    }
                case Keys.D9:
                    {
                        H9 = false;
                        break;
                    }
                case Keys.D0:
                    {
                        H0 = false;
                        break;
                    }
            }
            base.OnKeyUp(e);
        }

        private void GITMtype_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GITM.@type = ((dynamic)sender).selectedindex;
        }

        private void GITMeq_CheckedChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).gitm.equip = ((dynamic)sender).@checked;
        }

        private void GITMslot_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GITM.eqslot = ((dynamic)sender).selectedindex;
        }

        private void GITMrl_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GITM.reload = ((dynamic)sender).text;
        }

        private void GITMml_CheckedChanged(object sender, EventArgs e)
        {
            // TODO: Figure out Melee
        }

        private void GITMhhai_CheckedChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GITM.hHai = ((dynamic)sender).@checked;
        }

        private void GITMhbea_CheckedChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GITM.hBea = ((dynamic)sender).@checked;
        }

        private void GITMhmus_CheckedChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GITM.hMus = ((dynamic)sender).@checked;
        }

        private void GITMheye_CheckedChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GITM.hEye = ((dynamic)sender).@checked;
        }

        private void GITMhpon_CheckedChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GITM.hPon = ((dynamic)sender).@checked;
        }

        private void GITMhvan_CheckedChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
                ((dynamic)cf).GITM.hVan = ((dynamic)sender).@checked;
        }

        private void GITMsocgb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
            {
                switch (((dynamic)sender).selecteditem)
                {
                    case "Head":
                        {
                            GITMsoct.Text = Conversions.ToString(((dynamic)cf).GITM.Hea.Tex);
                            GITMsocm.Text = Conversions.ToString(((dynamic)cf).GITM.Hea.Model);
                            break;
                        }

                    case "Eye":
                        {
                            GITMsoct.Text = Conversions.ToString(((dynamic)cf).GITM.Eye.Tex);
                            GITMsocm.Text = Conversions.ToString(((dynamic)cf).GITM.Eye.Model);
                            break;
                        }

                    case "Body":
                        {
                            GITMsoct.Text = Conversions.ToString(((dynamic)cf).GITM.Bod.Tex);
                            GITMsocm.Text = Conversions.ToString(((dynamic)cf).GITM.Bod.Model);
                            break;
                        }

                    case "Back":
                        {
                            GITMsoct.Text = Conversions.ToString(((dynamic)cf).GITM.Bac.Tex);
                            GITMsocm.Text = Conversions.ToString(((dynamic)cf).GITM.Bac.Model);
                            break;
                        }

                    case "Hands":
                        {
                            GITMsoct.Text = Conversions.ToString(((dynamic)cf).GITM.Han.Tex);
                            GITMsocm.Text = Conversions.ToString(((dynamic)cf).GITM.Han.Model);
                            break;
                        }

                    case "Feet":
                        {
                            GITMsoct.Text = Conversions.ToString(((dynamic)cf).GITM.Fee.Tex);
                            GITMsocm.Text = Conversions.ToString(((dynamic)cf).GITM.Fee.Model);
                            break;
                        }

                    case "Shoes":
                        {
                            GITMsoct.Text = Conversions.ToString(((dynamic)cf).GITM.Sho.Tex);
                            GITMsocm.Text = Conversions.ToString(((dynamic)cf).GITM.Sho.Model);
                            break;
                        }

                    case "Vanity":
                        {
                            GITMsoct.Text = Conversions.ToString(((dynamic)cf).GITM.Van.Tex);
                            GITMsocm.Text = Conversions.ToString(((dynamic)cf).GITM.Van.Model);
                            break;
                        }

                    case "In-Hand":
                        {
                            GITMsoct.Text = Conversions.ToString(((dynamic)cf).GITM.IHS.Tex);
                            GITMsocm.Text = Conversions.ToString(((dynamic)cf).GITM.IHS.Model);
                            break;
                        }
                }
            }
        }

        private void GITMsocm_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
            {
                switch (GITMsoccb.SelectedItem)
                {
                    case "Head":
                        {
                            ((dynamic)cf).GITM.Hea.Model = GITMsocm.Text;
                            break;
                        }

                    case "Eye":
                        {
                            ((dynamic)cf).GITM.Eye.Model = GITMsocm.Text;
                            break;
                        }

                    case "Body":
                        {
                            ((dynamic)cf).GITM.Bod.Model = GITMsocm.Text;
                            break;
                        }

                    case "Back":
                        {
                            ((dynamic)cf).GITM.Bac.Model = GITMsocm.Text;
                            break;
                        }

                    case "Hands":
                        {
                            ((dynamic)cf).GITM.Han.Model = GITMsocm.Text;
                            break;
                        }

                    case "Feet":
                        {
                            ((dynamic)cf).GITM.Fee.Model = GITMsocm.Text;
                            break;
                        }

                    case "Shoes":
                        {
                            ((dynamic)cf).GITM.Sho.Model = GITMsocm.Text;
                            break;
                        }

                    case "Vanity":
                        {
                            ((dynamic)cf).GITM.Van.Model = GITMsocm.Text;
                            break;
                        }

                    case "In-Hand":
                        {
                            ((dynamic)cf).GITM.IHS.Model = GITMsocm.Text;
                            break;
                        }
                }
            }
        }

        private void GITMsoct_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(((dynamic)sender).Enabled))
            {
                switch (GITMsoccb.SelectedItem)
                {
                    case "Head":
                        {
                            ((dynamic)cf).GITM.Hea.Tex = GITMsoct.Text;
                            break;
                        }

                    case "Eye":
                        {
                            ((dynamic)cf).GITM.Eye.Tex = GITMsoct.Text;
                            break;
                        }

                    case "Body":
                        {
                            ((dynamic)cf).GITM.Bod.Tex = GITMsoct.Text;
                            break;
                        }

                    case "Back":
                        {
                            ((dynamic)cf).GITM.Bac.Tex = GITMsoct.Text;
                            break;
                        }

                    case "Hands":
                        {
                            ((dynamic)cf).GITM.Han.Tex = GITMsoct.Text;
                            break;
                        }

                    case "Feet":
                        {
                            ((dynamic)cf).GITM.Fee.Tex = GITMsoct.Text;
                            break;
                        }

                    case "Shoes":
                        {
                            ((dynamic)cf).GITM.Sho.Tex = GITMsoct.Text;
                            break;
                        }

                    case "Vanity":
                        {
                            ((dynamic)cf).GITM.Van.Tex = GITMsoct.Text;
                            break;
                        }

                    case "In-Hand":
                        {
                            ((dynamic)cf).GITM.IHS.Tex = GITMsoct.Text;
                            break;
                        }
                }
            }
        }

        #endregion

        private void ExtractgrpFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var grpb = new GrpBrowser(null))
            {
                grpb.ReadRHT();
                grpb.readExtensions();
                grpb.extractFile(-1);
                DarkMessageBox.ShowInformation("Done Extracting", "Finished");
            }
        }

        private void ExtractAndConvertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var grpb = new GrpBrowser(null))
            {
                grpb.ReadRHT();
                grpb.readExtensions();
                grpb.extractFile(-1, true);
                DarkMessageBox.ShowInformation("Done Extracting", "Finished");
            }
        }

        private void GrpBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var grpb = new GrpBrowser(null, true))
            {
                grpb.ShowDialog();
            }
        }

        private void InitialSetup(object sender, EventArgs e) => InitialSetup();
        private void NewAmo(object sender, EventArgs e) => NewAmo();
        private void NewArm(object sender, EventArgs e) => NewArm();
        private void NewCon(object sender, EventArgs e) => NewCon();
        private void NewCrt(object sender, EventArgs e) => NewCrt();
        private void NewDor(object sender, EventArgs e) => NewDor();
        private void NewInt(object sender, EventArgs e) => NewInt();
        private void NewItm(object sender, EventArgs e) => NewItm();
        private void NewMap(object sender, EventArgs e) => NewMap();
        private void NewUse(object sender, EventArgs e) => NewUse();
        private void NewWea(object sender, EventArgs e) => NewWea();
        private void OpenFile(object sender, EventArgs e) => OpenFile();
        private void SaveFile(object sender, EventArgs e) => SaveFile();
        private void EME2ToUI(object sender, EventArgs e) => EME2ToUI();
        private void EMEPToUI(object sender, EventArgs e) => EMEPToUI();
        private void TriggerToUI(object sender, EventArgs e) => TriggerToUI();
        private void Triggernud_ValueChanged(object sender, EventArgs e) => Triggernud_ValueChanged();
        private void EPTHToUI(object sender, EventArgs e) => EPTHToUI();
        private void EPTHnud_ValueChanged(object sender, EventArgs e) => EPTHnud_ValueChanged();
        private void Triggerpm_Click(object sender, EventArgs e) => Triggerpm_Click();
        private void EPTHpm_Click(object sender, EventArgs e) => EPTHpm_Click();
        private void EMSDToUI(object sender, EventArgs e) => EMSDToUI();
        private void EMEFToUI(object sender, EventArgs e) => EMEFToUI();
        private void EME2dgv_ScrollChanged(object sender, ScrollEventArgs e) => EME2dgv_ScrollChanged();
        private void EEN2dgv_ScrollChanged(object sender, ScrollEventArgs e) => EEN2dgv_ScrollChanged();
        private void GCREdgv_ScrollChanged(object sender, ScrollEventArgs e) => GCREdgv_ScrollChanged();
        private void FullSTFToText(object sender, EventArgs e) => FullSTFToText();
        private void FullTextToSTF(object sender, EventArgs e) => FullTextToSTF();
        private void SetEngStfLocation(object sender, EventArgs e) => SetEngStfLocation();

    }
}