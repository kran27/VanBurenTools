using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using AltUI.Controls;
using AltUI.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace VBLauncher
{

    [DesignerGenerated()]
    public partial class Editor : DarkForm
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
            var resources = new ComponentResourceManager(typeof(Editor));
            EMAPs3l = new DarkLabel();
            EMAPs2l = new DarkLabel();
            EMAPs1l = new DarkLabel();
            EMAPilcb = new DarkCheckBox();
            EMAPilcb.CheckedChanged += new EventHandler(EMAPilcb_CheckedChanged);
            EMAPslb = new DarkButton();
            EMAPslb.Click += new EventHandler(PickLightingColour);
            EMAPs3 = new DarkTextBox();
            EMAPs3.TextChanged += new EventHandler(EMAPs3_TextChanged);
            EMAPs2 = new DarkTextBox();
            EMAPs2.TextChanged += new EventHandler(EMAPs2_TextChanged);
            EMAPs1 = new DarkTextBox();
            EMAPs1.TextChanged += new EventHandler(EMAPs1_TextChanged);
            Mapgb = new DarkGroupBox();
            _2MWTgb = new DarkGroupBox();
            _2MWTdw = new DarkCheckBox();
            _2MWTdw.CheckedChanged += new EventHandler(_2MWTdw_CheckedChanged);
            _2MWTfr = new DarkCheckBox();
            _2MWTfr.CheckedChanged += new EventHandler(_2MWTfr_CheckedChanged);
            _2MWTlmy = new DarkTextBox();
            _2MWTlmy.TextChanged += new EventHandler(_2MWTlmy_TextChanged);
            _2MWTlmy.KeyPress += new KeyPressEventHandler(FloatsOnly);
            DarkLabel3 = new DarkLabel();
            _2MWTlmx = new DarkTextBox();
            _2MWTlmx.TextChanged += new EventHandler(_2MWTlmx_TextChanged);
            _2MWTlmx.KeyPress += new KeyPressEventHandler(FloatsOnly);
            DarkLabel2 = new DarkLabel();
            _2MWTtex = new DarkTextBox();
            _2MWTtex.TextChanged += new EventHandler(_2MWTtex_TextChanged);
            _2MWTmpf = new DarkTextBox();
            _2MWTmpf.TextChanged += new EventHandler(_2MWTmpf_TextChanged);
            _2MWTm = new DarkButton();
            _2MWTm.Click += new EventHandler(_2MWTm_Click);
            _2MWTp = new DarkButton();
            _2MWTp.Click += new EventHandler(_2MWTp_Click);
            _2MWTcb = new DarkComboBox();
            _2MWTcb.SelectedIndexChanged += new EventHandler(_2MWTcb_SelectedIndexChanged);
            DarkLabel1 = new DarkLabel();
            _2MWTz = new DarkTextBox();
            _2MWTz.TextChanged += new EventHandler(_2MWTz_TextChanged);
            _2MWTz.KeyPress += new KeyPressEventHandler(FloatsOnly);
            _2MWTy = new DarkTextBox();
            _2MWTy.TextChanged += new EventHandler(_2MWTy_TextChanged);
            _2MWTy.KeyPress += new KeyPressEventHandler(FloatsOnly);
            _2MWTx = new DarkTextBox();
            _2MWTx.TextChanged += new EventHandler(_2MWTx_TextChanged);
            _2MWTx.KeyPress += new KeyPressEventHandler(FloatsOnly);
            Triggergb = new DarkGroupBox();
            Triggerpm = new DarkButton();
            Triggerpm.Click += new EventHandler(Triggerpm_Click);
            Triggernud = new DarkNumericUpDown();
            Triggernud.ValueChanged += new EventHandler(Triggernud_ValueChanged);
            Triggerm = new DarkButton();
            Triggerm.Click += new EventHandler(Triggerm_Click);
            Triggertcb = new DarkComboBox();
            Triggertcb.SelectedIndexChanged += new EventHandler(Triggertcb_SelectedIndexChanged);
            Triggerp = new DarkButton();
            Triggerp.Click += new EventHandler(Triggerp_Click);
            Triggern = new DarkTextBox();
            Triggern.TextChanged += new EventHandler(Triggern_TextChanged);
            Triggercb = new DarkComboBox();
            Triggercb.SelectedIndexChanged += new EventHandler(TriggerToUI);
            Triggerx = new DarkTextBox();
            Triggerx.TextChanged += new EventHandler(Triggerx_TextChanged);
            Triggerx.KeyPress += new KeyPressEventHandler(FloatsOnly);
            Triggerxl = new DarkLabel();
            Triggery = new DarkTextBox();
            Triggery.TextChanged += new EventHandler(Triggery_TextChanged);
            Triggery.KeyPress += new KeyPressEventHandler(FloatsOnly);
            Triggerz = new DarkTextBox();
            Triggerz.TextChanged += new EventHandler(Triggerz_TextChanged);
            Triggerz.KeyPress += new KeyPressEventHandler(FloatsOnly);
            EPTHGB = new DarkGroupBox();
            EPTHpm = new DarkButton();
            EPTHpm.Click += new EventHandler(EPTHpm_Click);
            EPTHm = new DarkButton();
            EPTHm.Click += new EventHandler(EPTHm_Click);
            EPTHnud = new DarkNumericUpDown();
            EPTHnud.ValueChanged += new EventHandler(EPTHnud_ValueChanged);
            EPTHr = new DarkTextBox();
            EPTHr.TextChanged += new EventHandler(EPTHr_TextChanged);
            EPTHr.KeyPress += new KeyPressEventHandler(FloatsOnly);
            EPTHp = new DarkButton();
            EPTHp.Click += new EventHandler(EPTHp_Click);
            EPTHn = new DarkTextBox();
            EPTHn.TextChanged += new EventHandler(EPTHn_TextChanged);
            EPTHcb = new DarkComboBox();
            EPTHcb.SelectedIndexChanged += new EventHandler(EPTHToUI);
            EPTHx = new DarkTextBox();
            EPTHx.TextChanged += new EventHandler(EPTHx_TextChanged);
            EPTHx.KeyPress += new KeyPressEventHandler(FloatsOnly);
            EPTHxl = new DarkLabel();
            EPTHy = new DarkTextBox();
            EPTHy.TextChanged += new EventHandler(EPTHy_TextChanged);
            EPTHy.KeyPress += new KeyPressEventHandler(FloatsOnly);
            EPTHz = new DarkTextBox();
            EPTHz.TextChanged += new EventHandler(EPTHz_TextChanged);
            EPTHz.KeyPress += new KeyPressEventHandler(FloatsOnly);
            EMSDgb = new DarkGroupBox();
            EMSDm = new DarkButton();
            EMSDm.Click += new EventHandler(EMSDm_Click);
            EMSDcb = new DarkComboBox();
            EMSDcb.SelectedIndexChanged += new EventHandler(EMSDToUI);
            EMSDp = new DarkButton();
            EMSDp.Click += new EventHandler(EMSDp_Click);
            EMSDs2l = new DarkLabel();
            EMSDs1 = new DarkTextBox();
            EMSDs1.TextChanged += new EventHandler(EMSDs1_TextChanged);
            EMSDs2 = new DarkTextBox();
            EMSDs2.TextChanged += new EventHandler(EMSDs2_TextChanged);
            EMSDcl = new DarkLabel();
            EMSDz = new DarkTextBox();
            EMSDz.TextChanged += new EventHandler(EMSDz_TextChanged);
            EMSDz.KeyPress += new KeyPressEventHandler(FloatsOnly);
            EMSDy = new DarkTextBox();
            EMSDy.TextChanged += new EventHandler(EMSDy_TextChanged);
            EMSDy.KeyPress += new KeyPressEventHandler(FloatsOnly);
            EMSDx = new DarkTextBox();
            EMSDx.TextChanged += new EventHandler(EMSDx_TextChanged);
            EMSDx.KeyPress += new KeyPressEventHandler(FloatsOnly);
            EME2gb = new DarkGroupBox();
            EME2m = new DarkButton();
            EME2m.Click += new EventHandler(EME2m_Click);
            EME2p = new DarkButton();
            EME2p.Click += new EventHandler(EME2p_Click);
            EME2r = new DarkTextBox();
            EME2r.TextChanged += new EventHandler(EME2r_TextChanged);
            EME2r.KeyPress += new KeyPressEventHandler(FloatsOnly);
            EME2cb = new DarkComboBox();
            EME2cb.SelectedIndexChanged += new EventHandler(EME2ToUI);
            EME2dsb = new DarkScrollBar();
            EME2dsb.MouseDown += new MouseEventHandler(EME2dsb_Click);
            EME2dsb.MouseUp += new MouseEventHandler(EME2dsb_MouseUp);
            EME2s5l = new DarkLabel();
            EME2s4l = new DarkLabel();
            EME2s3l = new DarkLabel();
            EME2z = new DarkTextBox();
            EME2z.TextChanged += new EventHandler(EME2z_TextChanged);
            EME2z.KeyPress += new KeyPressEventHandler(FloatsOnly);
            EME2y = new DarkTextBox();
            EME2y.TextChanged += new EventHandler(EME2y_TextChanged);
            EME2y.KeyPress += new KeyPressEventHandler(FloatsOnly);
            EME2s5 = new DarkTextBox();
            EME2s5.TextChanged += new EventHandler(EME2s5_TextChanged);
            EME2s4 = new DarkTextBox();
            EME2s4.TextChanged += new EventHandler(EME2s4_TextChanged);
            EME2x = new DarkTextBox();
            EME2x.TextChanged += new EventHandler(EME2x_TextChanged);
            EME2x.KeyPress += new KeyPressEventHandler(FloatsOnly);
            EME2dgv = new DataGridView();
            EME2dgv.CellEndEdit += new DataGridViewCellEventHandler(EME2dgv_CellEndEdit);
            EME2dgv.Scroll += new ScrollEventHandler(EME2dgv_ScrollChanged);
            EME2cl = new DarkLabel();
            EME2s3 = new DarkTextBox();
            EME2s3.TextChanged += new EventHandler(EME2s3_TextChanged);
            EME2s2 = new DarkTextBox();
            EME2s2.TextChanged += new EventHandler(EME2s2_TextChanged);
            EME2s1 = new DarkTextBox();
            EME2s1.TextChanged += new EventHandler(EME2s1_TextChanged);
            EME2n = new DarkTextBox();
            EME2n.TextChanged += new EventHandler(EME2n_TextChanged);
            EMEFgb = new DarkGroupBox();
            EMEFm = new DarkButton();
            EMEFm.Click += new EventHandler(EMEFm_Click);
            EMEFr = new DarkTextBox();
            EMEFr.TextChanged += new EventHandler(EMEFr_TextChanged);
            EMEFr.KeyPress += new KeyPressEventHandler(FloatsOnly);
            EMEFp = new DarkButton();
            EMEFp.Click += new EventHandler(EMEFp_Click);
            EMEFcb = new DarkComboBox();
            EMEFcb.SelectedIndexChanged += new EventHandler(EMEFToUI);
            EMEFs2l = new DarkLabel();
            EMEFs1 = new DarkTextBox();
            EMEFs1.TextChanged += new EventHandler(EMEFs1_TextChanged);
            EMEFs2 = new DarkTextBox();
            EMEFs2.TextChanged += new EventHandler(EMEFs2_TextChanged);
            EMEFxl = new DarkLabel();
            EMEFz = new DarkTextBox();
            EMEFz.TextChanged += new EventHandler(EMEFz_TextChanged);
            EMEFz.KeyPress += new KeyPressEventHandler(FloatsOnly);
            EMEFy = new DarkTextBox();
            EMEFy.TextChanged += new EventHandler(EMEFy_TextChanged);
            EMEFy.KeyPress += new KeyPressEventHandler(FloatsOnly);
            EMEFx = new DarkTextBox();
            EMEFx.TextChanged += new EventHandler(EMEFx_TextChanged);
            EMEFx.KeyPress += new KeyPressEventHandler(FloatsOnly);
            ECAMgb = new DarkGroupBox();
            ECAMr = new DarkTextBox();
            ECAMr.TextChanged += new EventHandler(ECAMr_TextChanged);
            ECAMr.KeyPress += new KeyPressEventHandler(FloatsOnly);
            ECAMcl = new DarkLabel();
            ECAMz = new DarkTextBox();
            ECAMz.TextChanged += new EventHandler(ECAMz_TextChanged);
            ECAMz.KeyPress += new KeyPressEventHandler(FloatsOnly);
            ECAMy = new DarkTextBox();
            ECAMy.TextChanged += new EventHandler(ECAMy_TextChanged);
            ECAMy.KeyPress += new KeyPressEventHandler(FloatsOnly);
            ECAMx = new DarkTextBox();
            ECAMx.TextChanged += new EventHandler(ECAMx_TextChanged);
            ECAMx.KeyPress += new KeyPressEventHandler(FloatsOnly);
            EMAPgb = new DarkGroupBox();
            EMEPgb = new DarkGroupBox();
            EMEPm = new DarkButton();
            EMEPm.Click += new EventHandler(EMEPm_Click);
            EMEPp = new DarkButton();
            EMEPp.Click += new EventHandler(EMEPp_Click);
            EMEPcb = new DarkComboBox();
            EMEPcb.SelectedIndexChanged += new EventHandler(EMEPToUI);
            EMEPr = new DarkTextBox();
            EMEPr.TextChanged += new EventHandler(EMEPr_TextChanged);
            EMEPr.KeyPress += new KeyPressEventHandler(FloatsOnly);
            EMEPcl = new DarkLabel();
            EMEPepl = new DarkLabel();
            EMEPz = new DarkTextBox();
            EMEPz.TextChanged += new EventHandler(EMEPz_TextChanged);
            EMEPz.KeyPress += new KeyPressEventHandler(FloatsOnly);
            EMEPy = new DarkTextBox();
            EMEPy.TextChanged += new EventHandler(EMEPy_TextChanged);
            EMEPy.KeyPress += new KeyPressEventHandler(FloatsOnly);
            EMEPnud = new DarkNumericUpDown();
            EMEPnud.ValueChanged += new EventHandler(EMEPnud_ValueChanged);
            EMEPx = new DarkTextBox();
            EMEPx.TextChanged += new EventHandler(EMEPx_TextChanged);
            EMEPx.KeyPress += new KeyPressEventHandler(FloatsOnly);
            EME2tmr = new Timer(components);
            EME2tmr.Tick += new EventHandler(EME2tmr_Tick);
            DarkMainMenuStrip = new DarkMenuStrip();
            FileToolStripMenuItem = new ToolStripMenuItem();
            NewToolStripMenuItem = new ToolStripMenuItem();
            AmoToolStripMenuItem = new ToolStripMenuItem();
            AmoToolStripMenuItem.Click += new EventHandler(NewAmo);
            ArmToolStripMenuItem = new ToolStripMenuItem();
            ArmToolStripMenuItem.Click += new EventHandler(NewArm);
            ConToolStripMenuItem = new ToolStripMenuItem();
            ConToolStripMenuItem.Click += new EventHandler(NewCon);
            CrtToolStripMenuItem = new ToolStripMenuItem();
            CrtToolStripMenuItem.Click += new EventHandler(NewCrt);
            DorToolStripMenuItem = new ToolStripMenuItem();
            DorToolStripMenuItem.Click += new EventHandler(NewDor);
            IntToolStripMenuItem = new ToolStripMenuItem();
            IntToolStripMenuItem.Click += new EventHandler(NewInt);
            ItmToolStripMenuItem = new ToolStripMenuItem();
            ItmToolStripMenuItem.Click += new EventHandler(NewItm);
            MapToolStripMenuItem = new ToolStripMenuItem();
            MapToolStripMenuItem.Click += new EventHandler(NewMap);
            UseToolStripMenuItem = new ToolStripMenuItem();
            UseToolStripMenuItem.Click += new EventHandler(NewUse);
            WeaToolStripMenuItem = new ToolStripMenuItem();
            WeaToolStripMenuItem.Click += new EventHandler(NewWea);
            OpenToolStripMenuItem = new ToolStripMenuItem();
            OpenToolStripMenuItem.Click += new EventHandler(OpenFile);
            OpenFromgrpToolStripMenuItem = new ToolStripMenuItem();
            OpenFromgrpToolStripMenuItem.Click += new EventHandler(OpenFromgrpToolStripMenuItem_Click);
            SaveToolStripMenuItem = new ToolStripMenuItem();
            SaveToolStripMenuItem.Click += new EventHandler(SaveFile);
            OptionsToolStripMenuItem = new ToolStripMenuItem();
            SetEnglishstfLocationToolStripMenuItem = new ToolStripMenuItem();
            SetEnglishstfLocationToolStripMenuItem.Click += new EventHandler(SetEngStfLocation);
            EnableSTFEdit = new ToolStripMenuItem();
            EnableSTFEdit.CheckedChanged += new EventHandler(EnableEnglishstfEditingToolStripMenuItem_Click);
            ToolsToolStripMenuItem = new ToolStripMenuItem();
            StfToolStripMenuItem = new ToolStripMenuItem();
            StfToolStripMenuItem.Click += new EventHandler(FullSTFToText);
            TxtTostfToolStripMenuItem = new ToolStripMenuItem();
            TxtTostfToolStripMenuItem.Click += new EventHandler(FullTextToSTF);
            ExtractgrpFilesToolStripMenuItem = new ToolStripMenuItem();
            ExtractgrpFilesToolStripMenuItem.Click += new EventHandler(ExtractgrpFilesToolStripMenuItem_Click);
            ExtractAndConvertToolStripMenuItem = new ToolStripMenuItem();
            ExtractAndConvertToolStripMenuItem.Click += new EventHandler(ExtractAndConvertToolStripMenuItem_Click);
            GrpBrowserToolStripMenuItem = new ToolStripMenuItem();
            GrpBrowserToolStripMenuItem.Click += new EventHandler(GrpBrowserToolStripMenuItem_Click);
            ToolTip = new DarkToolTip();
            GWAMsf = new DarkNumericUpDown();
            GWAMsf.ValueChanged += new EventHandler(GWAMsf_ValueChanged);
            EEN2dgv = new DataGridView();
            EEN2dgv.CellEndEdit += new DataGridViewCellEventHandler(EEN2dgv_CellEndEdit);
            EEN2dgv.Scroll += new ScrollEventHandler(EEN2dgv_ScrollChanged);
            CRTgb = new DarkGroupBox();
            GCHRgb = new DarkGroupBox();
            GCHRn = new DarkTextBox();
            GCHRn.TextChanged += new EventHandler(GCHRn_TextChanged);
            GCREgb = new DarkGroupBox();
            GCREdsb = new DarkScrollBar();
            GCREdsb.MouseDown += new MouseEventHandler(GCREdsb_Click);
            GCREdsb.MouseUp += new MouseEventHandler(GCREdsb_MouseUp);
            GCREtlp = new TableLayoutPanel();
            GCREdgv = new DataGridView();
            GCREdgv.CellEndEdit += new DataGridViewCellEventHandler(GCREdgv_CellEndEdit);
            GCREdgv.Scroll += new ScrollEventHandler(GCREdgv_ScrollChanged);
            GCREskv = new DarkNumericUpDown();
            GCREskv.ValueChanged += new EventHandler(GCREskv_ValueChanged);
            GCREspv = new DarkNumericUpDown();
            GCREspv.ValueChanged += new EventHandler(GCREspv_ValueChanged);
            GCREspl = new DarkLabel();
            GCREspcb = new DarkComboBox();
            GCREspcb.SelectedIndexChanged += new EventHandler(GCREspcb_SelectedIndexChanged);
            GCREskl = new DarkLabel();
            GCREtrl = new DarkLabel();
            GCREskcb = new DarkComboBox();
            GCREskcb.SelectedIndexChanged += new EventHandler(GCREskcb_SelectedIndexChanged);
            GCREtrcb = new DarkComboBox();
            GCREtrcb.SelectedIndexChanged += new EventHandler(GCREtrcb_SelectedIndexChanged);
            GCREtsl = new DarkLabel();
            GCREtscb = new DarkComboBox();
            GCREtscb.SelectedIndexChanged += new EventHandler(GCREtscb_SelectedIndexChanged);
            GCREtrv = new DarkCheckBox();
            GCREtrv.CheckedChanged += new EventHandler(GCREtrv_CheckedChanged);
            GCREtsv = new DarkCheckBox();
            GCREtsv.CheckedChanged += new EventHandler(GCREtsv_CheckedChanged);
            GCREpl = new DarkLabel();
            GCREp = new DarkTextBox();
            GCREp.TextChanged += new EventHandler(GCREp_TextChanged);
            GCREsocl = new DarkLabel();
            GCREsocm = new DarkTextBox();
            GCREsocm.TextChanged += new EventHandler(GCREsocm_TextChanged);
            GCREsoct = new DarkTextBox();
            GCREsoct.TextChanged += new EventHandler(GCREsoct_TextChanged);
            GCREsocml = new DarkLabel();
            GCREsoctl = new DarkLabel();
            GCREsoccb = new DarkComboBox();
            GCREsoccb.SelectedIndexChanged += new EventHandler(GCREsoccb_SelectedIndexChanged);
            GCREeil = new DarkLabel();
            GCREsoctel = new DarkLabel();
            GCREpel = new DarkLabel();
            GCREage = new DarkNumericUpDown();
            GCREage.ValueChanged += new EventHandler(GCREage_ValueChanged);
            GCREagel = new DarkLabel();
            GWAMgb = new DarkGroupBox();
            GWAMm = new DarkButton();
            GWAMm.Click += new EventHandler(GWAMm_Click);
            GWAMcb = new DarkComboBox();
            GWAMp = new DarkButton();
            GWAMp.Click += new EventHandler(GWAMp_Click);
            GWAMtlp = new TableLayoutPanel();
            GWAManil = new DarkLabel();
            GWAMani = new DarkNumericUpDown();
            GWAMani.ValueChanged += new EventHandler(GWAMani_ValueChanged);
            GWAMdtl = new DarkLabel();
            GWAMdt = new DarkComboBox();
            GWAMdt.SelectedIndexChanged += new EventHandler(GWAMdt_SelectedIndexChanged);
            GWAMapl = new DarkLabel();
            GWAMap = new DarkNumericUpDown();
            GWAMap.ValueChanged += new EventHandler(GWAMap_ValueChanged);
            GWAMsfl = new DarkLabel();
            GWAMrl = new DarkLabel();
            GWAMr = new DarkNumericUpDown();
            GWAMr.ValueChanged += new EventHandler(GWAMr_ValueChanged);
            GWAMminl = new DarkLabel();
            GWAMmin = new DarkNumericUpDown();
            GWAMmin.ValueChanged += new EventHandler(GWAMmin_ValueChanged);
            GWAMmaxl = new DarkLabel();
            GWAMmax = new DarkNumericUpDown();
            GWAMmax.ValueChanged += new EventHandler(GWAMmax_ValueChanged);
            GWAManl = new DarkLabel();
            GWAManSR = new DarkNumericUpDown();
            GWAManSR.ValueChanged += new EventHandler(GWAManSR_ValueChanged);
            GWAMef = new DarkTextBox();
            GWAMef.TextChanged += new EventHandler(GWAMef_TextChanged);
            GWAMefl = new DarkLabel();
            GWAMan = new DarkTextBox();
            GWAMan.TextChanged += new EventHandler(GWAMan_TextChanged);
            GWAMefel = new DarkLabel();
            GENTgb = new DarkGroupBox();
            GENTu = new DarkTextBox();
            GENTu.TextChanged += new EventHandler(GENTu_TextChanged);
            GENTn = new DarkTextBox();
            GENTn.TextChanged += new EventHandler(GENTn_TextChanged);
            GENTl = new DarkTextBox();
            GENTl.TextChanged += new EventHandler(GENTl_TextChanged);
            GENTh = new DarkTextBox();
            GENTh.TextChanged += new EventHandler(GENTh_TextChanged);
            GENTihpl = new DarkLabel();
            GENTmhpl = new DarkLabel();
            GENTihp = new DarkNumericUpDown();
            GENTihp.ValueChanged += new EventHandler(GENTihp_ValueChanged);
            GENTmhp = new DarkNumericUpDown();
            GENTmhp.ValueChanged += new EventHandler(GENTmhp_ValueChanged);
            GENTuSR = new DarkNumericUpDown();
            GENTuSR.ValueChanged += new EventHandler(GENTuSR_ValueChanged);
            GENTnSR = new DarkNumericUpDown();
            GENTnSR.ValueChanged += new EventHandler(GENTnSR_ValueChanged);
            GENTlSR = new DarkNumericUpDown();
            GENTlSR.ValueChanged += new EventHandler(GENTlSR_ValueChanged);
            GENThSR = new DarkNumericUpDown();
            GENThSR.ValueChanged += new EventHandler(GENThSR_ValueChanged);
            EEN2gb = new DarkGroupBox();
            EEN2dsb = new DarkScrollBar();
            EEN2dsb.MouseDown += new MouseEventHandler(EEN2dsb_Click);
            EEN2dsb.MouseUp += new MouseEventHandler(EEN2dsb_MouseUp);
            EEN2acttl = new DarkLabel();
            EEN2invtl = new DarkLabel();
            EEN2s5l = new DarkLabel();
            EEN2s4l = new DarkLabel();
            EEN2s3l = new DarkLabel();
            EEN2s5 = new DarkTextBox();
            EEN2s5.TextChanged += new EventHandler(EEN2s5_TextChanged);
            EEN2s4 = new DarkTextBox();
            EEN2s4.TextChanged += new EventHandler(EEN2s4_TextChanged);
            EEN2s3 = new DarkTextBox();
            EEN2s3.TextChanged += new EventHandler(EEN2s3_TextChanged);
            EEN2s2 = new DarkTextBox();
            EEN2s2.TextChanged += new EventHandler(EEN2s2_TextChanged);
            EEN2s1 = new DarkTextBox();
            EEN2s1.TextChanged += new EventHandler(EEN2s1_TextChanged);
            EEN2sel = new DarkCheckBox();
            EEN2sel.CheckedChanged += new EventHandler(EEN2sel_CheckedChanged);
            EEN2actt = new DarkTextBox();
            EEN2actt.TextChanged += new EventHandler(EEN2actt_TextChanged);
            EEN2invt = new DarkTextBox();
            EEN2invt.TextChanged += new EventHandler(EEN2invt_TextChanged);
            EEN2skl = new DarkTextBox();
            EEN2skl.TextChanged += new EventHandler(EEN2skl_TextChanged);
            GCREtmr = new Timer(components);
            GCREtmr.Tick += new EventHandler(GCREtmr_Tick);
            EEN2tmr = new Timer(components);
            EEN2tmr.Tick += new EventHandler(EEN2tmr_Tick);
            ITMgb = new DarkGroupBox();
            GITMgb = new DarkGroupBox();
            TableLayoutPanel1 = new TableLayoutPanel();
            DarkLabel4 = new DarkLabel();
            GITMeq = new DarkCheckBox();
            GITMeq.CheckedChanged += new EventHandler(GITMeq_CheckedChanged);
            DarkLabel5 = new DarkLabel();
            GITMslot = new DarkComboBox();
            GITMslot.SelectedIndexChanged += new EventHandler(GITMslot_SelectedIndexChanged);
            GITMml = new DarkCheckBox();
            GITMml.CheckedChanged += new EventHandler(GITMml_CheckedChanged);
            GITMrl = new DarkTextBox();
            GITMrl.TextChanged += new EventHandler(GITMrl_TextChanged);
            DarkLabel6 = new DarkLabel();
            DarkLabel7 = new DarkLabel();
            GITMhhai = new DarkCheckBox();
            GITMhhai.CheckedChanged += new EventHandler(GITMhhai_CheckedChanged);
            GITMhbea = new DarkCheckBox();
            GITMhbea.CheckedChanged += new EventHandler(GITMhbea_CheckedChanged);
            GITMhvan = new DarkCheckBox();
            GITMhvan.CheckedChanged += new EventHandler(GITMhvan_CheckedChanged);
            GITMheye = new DarkCheckBox();
            GITMheye.CheckedChanged += new EventHandler(GITMheye_CheckedChanged);
            GITMhmus = new DarkCheckBox();
            GITMhmus.CheckedChanged += new EventHandler(GITMhmus_CheckedChanged);
            GITMhpon = new DarkCheckBox();
            GITMhpon.CheckedChanged += new EventHandler(GITMhpon_CheckedChanged);
            GITMtype = new DarkComboBox();
            GITMtype.SelectedIndexChanged += new EventHandler(GITMtype_SelectedIndexChanged);
            DarkLabel8 = new DarkLabel();
            GITMsoccb = new DarkComboBox();
            GITMsoccb.SelectedIndexChanged += new EventHandler(GITMsocgb_SelectedIndexChanged);
            DarkLabel9 = new DarkLabel();
            DarkLabel10 = new DarkLabel();
            GITMsoct = new DarkTextBox();
            GITMsoct.TextChanged += new EventHandler(GITMsoct_TextChanged);
            GITMsocm = new DarkTextBox();
            GITMsocm.TextChanged += new EventHandler(GITMsocm_TextChanged);
            Mapgb.SuspendLayout();
            _2MWTgb.SuspendLayout();
            Triggergb.SuspendLayout();
            ((ISupportInitialize)Triggernud).BeginInit();
            EPTHGB.SuspendLayout();
            ((ISupportInitialize)EPTHnud).BeginInit();
            EMSDgb.SuspendLayout();
            EME2gb.SuspendLayout();
            ((ISupportInitialize)EME2dgv).BeginInit();
            EMEFgb.SuspendLayout();
            ECAMgb.SuspendLayout();
            EMAPgb.SuspendLayout();
            EMEPgb.SuspendLayout();
            ((ISupportInitialize)EMEPnud).BeginInit();
            DarkMainMenuStrip.SuspendLayout();
            ((ISupportInitialize)GWAMsf).BeginInit();
            ((ISupportInitialize)EEN2dgv).BeginInit();
            CRTgb.SuspendLayout();
            GCHRgb.SuspendLayout();
            GCREgb.SuspendLayout();
            GCREtlp.SuspendLayout();
            ((ISupportInitialize)GCREdgv).BeginInit();
            ((ISupportInitialize)GCREskv).BeginInit();
            ((ISupportInitialize)GCREspv).BeginInit();
            ((ISupportInitialize)GCREage).BeginInit();
            GWAMgb.SuspendLayout();
            GWAMtlp.SuspendLayout();
            ((ISupportInitialize)GWAMani).BeginInit();
            ((ISupportInitialize)GWAMap).BeginInit();
            ((ISupportInitialize)GWAMr).BeginInit();
            ((ISupportInitialize)GWAMmin).BeginInit();
            ((ISupportInitialize)GWAMmax).BeginInit();
            ((ISupportInitialize)GWAManSR).BeginInit();
            GENTgb.SuspendLayout();
            ((ISupportInitialize)GENTihp).BeginInit();
            ((ISupportInitialize)GENTmhp).BeginInit();
            ((ISupportInitialize)GENTuSR).BeginInit();
            ((ISupportInitialize)GENTnSR).BeginInit();
            ((ISupportInitialize)GENTlSR).BeginInit();
            ((ISupportInitialize)GENThSR).BeginInit();
            EEN2gb.SuspendLayout();
            ITMgb.SuspendLayout();
            GITMgb.SuspendLayout();
            TableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // EMAPs3l
            // 
            EMAPs3l.AutoSize = true;
            EMAPs3l.Location = new Point(265, 114);
            EMAPs3l.Name = "EMAPs3l";
            EMAPs3l.Size = new Size(29, 15);
            EMAPs3l.TabIndex = 14;
            EMAPs3l.Text = ".dds";
            // 
            // EMAPs2l
            // 
            EMAPs2l.AutoSize = true;
            EMAPs2l.Location = new Point(271, 85);
            EMAPs2l.Name = "EMAPs2l";
            EMAPs2l.Size = new Size(23, 15);
            EMAPs2l.TabIndex = 13;
            EMAPs2l.Text = ".rle";
            // 
            // EMAPs1l
            // 
            EMAPs1l.AutoSize = true;
            EMAPs1l.Location = new Point(278, 56);
            EMAPs1l.Name = "EMAPs1l";
            EMAPs1l.Size = new Size(16, 15);
            EMAPs1l.TabIndex = 12;
            EMAPs1l.Text = ".8";
            // 
            // EMAPilcb
            // 
            EMAPilcb.AutoSize = true;
            EMAPilcb.Location = new Point(187, 24);
            EMAPilcb.Name = "EMAPilcb";
            EMAPilcb.Offset = 1;
            EMAPilcb.Size = new Size(107, 19);
            EMAPilcb.TabIndex = 11;
            EMAPilcb.Text = "Ignore Lighting";
            // 
            // EMAPslb
            // 
            EMAPslb.BorderColour = Color.Empty;
            EMAPslb.CustomColour = false;
            EMAPslb.FlatBottom = false;
            EMAPslb.FlatTop = false;
            EMAPslb.Location = new Point(6, 22);
            EMAPslb.Name = "EMAPslb";
            EMAPslb.Padding = new Padding(5);
            EMAPslb.Size = new Size(144, 24);
            EMAPslb.TabIndex = 9;
            EMAPslb.Text = "Set Lighting Colour";
            // 
            // EMAPs3
            // 
            EMAPs3.BackColor = Color.FromArgb(26, 26, 28);
            EMAPs3.BorderStyle = BorderStyle.FixedSingle;
            EMAPs3.ForeColor = Color.FromArgb(213, 213, 213);
            EMAPs3.Location = new Point(6, 110);
            EMAPs3.Name = "EMAPs3";
            EMAPs3.Size = new Size(253, 23);
            EMAPs3.TabIndex = 2;
            // 
            // EMAPs2
            // 
            EMAPs2.BackColor = Color.FromArgb(26, 26, 28);
            EMAPs2.BorderStyle = BorderStyle.FixedSingle;
            EMAPs2.ForeColor = Color.FromArgb(213, 213, 213);
            EMAPs2.Location = new Point(6, 81);
            EMAPs2.Name = "EMAPs2";
            EMAPs2.Size = new Size(259, 23);
            EMAPs2.TabIndex = 1;
            // 
            // EMAPs1
            // 
            EMAPs1.BackColor = Color.FromArgb(26, 26, 28);
            EMAPs1.BorderStyle = BorderStyle.FixedSingle;
            EMAPs1.ForeColor = Color.FromArgb(213, 213, 213);
            EMAPs1.Location = new Point(6, 52);
            EMAPs1.Name = "EMAPs1";
            EMAPs1.Size = new Size(266, 23);
            EMAPs1.TabIndex = 0;
            // 
            // Mapgb
            // 
            Mapgb.Controls.Add(_2MWTgb);
            Mapgb.Controls.Add(Triggergb);
            Mapgb.Controls.Add(EPTHGB);
            Mapgb.Controls.Add(EMSDgb);
            Mapgb.Controls.Add(EME2gb);
            Mapgb.Controls.Add(EMEFgb);
            Mapgb.Controls.Add(ECAMgb);
            Mapgb.Controls.Add(EMAPgb);
            Mapgb.Controls.Add(EMEPgb);
            Mapgb.Location = new Point(942, 27);
            Mapgb.Name = "Mapgb";
            Mapgb.Size = new Size(924, 522);
            Mapgb.TabIndex = 2;
            Mapgb.TabStop = false;
            Mapgb.Text = ".MAP Editor";
            // 
            // _2MWTgb
            // 
            _2MWTgb.Controls.Add(_2MWTdw);
            _2MWTgb.Controls.Add(_2MWTfr);
            _2MWTgb.Controls.Add(_2MWTlmy);
            _2MWTgb.Controls.Add(DarkLabel3);
            _2MWTgb.Controls.Add(_2MWTlmx);
            _2MWTgb.Controls.Add(DarkLabel2);
            _2MWTgb.Controls.Add(_2MWTtex);
            _2MWTgb.Controls.Add(_2MWTmpf);
            _2MWTgb.Controls.Add(_2MWTm);
            _2MWTgb.Controls.Add(_2MWTp);
            _2MWTgb.Controls.Add(_2MWTcb);
            _2MWTgb.Controls.Add(DarkLabel1);
            _2MWTgb.Controls.Add(_2MWTz);
            _2MWTgb.Controls.Add(_2MWTy);
            _2MWTgb.Controls.Add(_2MWTx);
            _2MWTgb.Location = new Point(6, 406);
            _2MWTgb.Name = "_2MWTgb";
            _2MWTgb.Size = new Size(606, 110);
            _2MWTgb.TabIndex = 41;
            _2MWTgb.TabStop = false;
            _2MWTgb.Text = "2MWT";
            // 
            // _2MWTdw
            // 
            _2MWTdw.AutoSize = true;
            _2MWTdw.Location = new Point(427, 24);
            _2MWTdw.Name = "_2MWTdw";
            _2MWTdw.Offset = 1;
            _2MWTdw.Size = new Size(84, 19);
            _2MWTdw.TabIndex = 57;
            _2MWTdw.Text = "Dark Water";
            // 
            // _2MWTfr
            // 
            _2MWTfr.AutoSize = true;
            _2MWTfr.Location = new Point(306, 24);
            _2MWTfr.Name = "_2MWTfr";
            _2MWTfr.Offset = 1;
            _2MWTfr.Size = new Size(116, 19);
            _2MWTfr.TabIndex = 56;
            _2MWTfr.Text = "Unmoving Water";
            // 
            // _2MWTlmy
            // 
            _2MWTlmy.BackColor = Color.FromArgb(26, 26, 28);
            _2MWTlmy.BorderStyle = BorderStyle.FixedSingle;
            _2MWTlmy.ForeColor = Color.FromArgb(213, 213, 213);
            _2MWTlmy.Location = new Point(480, 81);
            _2MWTlmy.Name = "_2MWTlmy";
            _2MWTlmy.Size = new Size(120, 23);
            _2MWTlmy.TabIndex = 55;
            // 
            // DarkLabel3
            // 
            DarkLabel3.AutoSize = true;
            DarkLabel3.Location = new Point(310, 85);
            DarkLabel3.Name = "DarkLabel3";
            DarkLabel3.Size = new Size(38, 15);
            DarkLabel3.TabIndex = 54;
            DarkLabel3.Text = "LMXY";
            // 
            // _2MWTlmx
            // 
            _2MWTlmx.BackColor = Color.FromArgb(26, 26, 28);
            _2MWTlmx.BorderStyle = BorderStyle.FixedSingle;
            _2MWTlmx.ForeColor = Color.FromArgb(213, 213, 213);
            _2MWTlmx.Location = new Point(354, 81);
            _2MWTlmx.Name = "_2MWTlmx";
            _2MWTlmx.Size = new Size(120, 23);
            _2MWTlmx.TabIndex = 53;
            // 
            // DarkLabel2
            // 
            DarkLabel2.AutoSize = true;
            DarkLabel2.Location = new Point(571, 56);
            DarkLabel2.Name = "DarkLabel2";
            DarkLabel2.Size = new Size(29, 15);
            DarkLabel2.TabIndex = 52;
            DarkLabel2.Text = ".dds";
            // 
            // _2MWTtex
            // 
            _2MWTtex.BackColor = Color.FromArgb(26, 26, 28);
            _2MWTtex.BorderStyle = BorderStyle.FixedSingle;
            _2MWTtex.ForeColor = Color.FromArgb(213, 213, 213);
            _2MWTtex.Location = new Point(306, 52);
            _2MWTtex.Name = "_2MWTtex";
            _2MWTtex.Size = new Size(259, 23);
            _2MWTtex.TabIndex = 51;
            // 
            // _2MWTmpf
            // 
            _2MWTmpf.BackColor = Color.FromArgb(26, 26, 28);
            _2MWTmpf.BorderStyle = BorderStyle.FixedSingle;
            _2MWTmpf.ForeColor = Color.FromArgb(213, 213, 213);
            _2MWTmpf.Location = new Point(6, 23);
            _2MWTmpf.Name = "_2MWTmpf";
            _2MWTmpf.Size = new Size(294, 23);
            _2MWTmpf.TabIndex = 50;
            // 
            // _2MWTm
            // 
            _2MWTm.BorderColour = Color.Empty;
            _2MWTm.CustomColour = false;
            _2MWTm.FlatBottom = false;
            _2MWTm.FlatTop = false;
            _2MWTm.Location = new Point(276, 52);
            _2MWTm.Name = "_2MWTm";
            _2MWTm.Padding = new Padding(5);
            _2MWTm.Size = new Size(24, 24);
            _2MWTm.TabIndex = 49;
            _2MWTm.Text = "-";
            // 
            // _2MWTp
            // 
            _2MWTp.BorderColour = Color.Empty;
            _2MWTp.CustomColour = false;
            _2MWTp.FlatBottom = false;
            _2MWTp.FlatTop = false;
            _2MWTp.Location = new Point(248, 52);
            _2MWTp.Name = "_2MWTp";
            _2MWTp.Padding = new Padding(5);
            _2MWTp.Size = new Size(24, 24);
            _2MWTp.TabIndex = 48;
            _2MWTp.Text = "+";
            // 
            // _2MWTcb
            // 
            _2MWTcb.DrawMode = DrawMode.OwnerDrawVariable;
            _2MWTcb.FormattingEnabled = true;
            _2MWTcb.Location = new Point(6, 52);
            _2MWTcb.Name = "_2MWTcb";
            _2MWTcb.Size = new Size(236, 24);
            _2MWTcb.TabIndex = 47;
            // 
            // DarkLabel1
            // 
            DarkLabel1.AutoSize = true;
            DarkLabel1.Location = new Point(6, 86);
            DarkLabel1.Name = "DarkLabel1";
            DarkLabel1.Size = new Size(28, 15);
            DarkLabel1.TabIndex = 9;
            DarkLabel1.Text = "XYZ";
            // 
            // _2MWTz
            // 
            _2MWTz.BackColor = Color.FromArgb(26, 26, 28);
            _2MWTz.BorderStyle = BorderStyle.FixedSingle;
            _2MWTz.ForeColor = Color.FromArgb(213, 213, 213);
            _2MWTz.Location = new Point(217, 82);
            _2MWTz.Name = "_2MWTz";
            _2MWTz.Size = new Size(83, 23);
            _2MWTz.TabIndex = 8;
            // 
            // _2MWTy
            // 
            _2MWTy.BackColor = Color.FromArgb(26, 26, 28);
            _2MWTy.BorderStyle = BorderStyle.FixedSingle;
            _2MWTy.ForeColor = Color.FromArgb(213, 213, 213);
            _2MWTy.Location = new Point(128, 82);
            _2MWTy.Name = "_2MWTy";
            _2MWTy.Size = new Size(83, 23);
            _2MWTy.TabIndex = 7;
            // 
            // _2MWTx
            // 
            _2MWTx.BackColor = Color.FromArgb(26, 26, 28);
            _2MWTx.BorderStyle = BorderStyle.FixedSingle;
            _2MWTx.ForeColor = Color.FromArgb(213, 213, 213);
            _2MWTx.Location = new Point(39, 82);
            _2MWTx.Name = "_2MWTx";
            _2MWTx.Size = new Size(83, 23);
            _2MWTx.TabIndex = 6;
            // 
            // Triggergb
            // 
            Triggergb.Controls.Add(Triggerpm);
            Triggergb.Controls.Add(Triggernud);
            Triggergb.Controls.Add(Triggerm);
            Triggergb.Controls.Add(Triggertcb);
            Triggergb.Controls.Add(Triggerp);
            Triggergb.Controls.Add(Triggern);
            Triggergb.Controls.Add(Triggercb);
            Triggergb.Controls.Add(Triggerx);
            Triggergb.Controls.Add(Triggerxl);
            Triggergb.Controls.Add(Triggery);
            Triggergb.Controls.Add(Triggerz);
            Triggergb.Location = new Point(618, 428);
            Triggergb.Name = "Triggergb";
            Triggergb.Size = new Size(300, 88);
            Triggergb.TabIndex = 14;
            Triggergb.TabStop = false;
            Triggergb.Text = "Trigger";
            // 
            // Triggerpm
            // 
            Triggerpm.BorderColour = Color.Empty;
            Triggerpm.CustomColour = false;
            Triggerpm.FlatBottom = false;
            Triggerpm.FlatTop = false;
            Triggerpm.Location = new Point(38, 59);
            Triggerpm.Name = "Triggerpm";
            Triggerpm.Padding = new Padding(5);
            Triggerpm.Size = new Size(15, 23);
            Triggerpm.TabIndex = 47;
            Triggerpm.Text = "-";
            // 
            // Triggernud
            // 
            Triggernud.Location = new Point(6, 59);
            Triggernud.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            Triggernud.Name = "Triggernud";
            Triggernud.Size = new Size(32, 23);
            Triggernud.TabIndex = 31;
            // 
            // Triggerm
            // 
            Triggerm.BorderColour = Color.Empty;
            Triggerm.CustomColour = false;
            Triggerm.FlatBottom = false;
            Triggerm.FlatTop = false;
            Triggerm.Location = new Point(270, 0);
            Triggerm.Name = "Triggerm";
            Triggerm.Padding = new Padding(5);
            Triggerm.Size = new Size(24, 24);
            Triggerm.TabIndex = 46;
            Triggerm.Text = "-";
            // 
            // Triggertcb
            // 
            Triggertcb.DrawMode = DrawMode.OwnerDrawVariable;
            Triggertcb.FormattingEnabled = true;
            Triggertcb.Items.AddRange(new object[] { "B", "S", "T" });
            Triggertcb.Location = new Point(241, 28);
            Triggertcb.Name = "Triggertcb";
            Triggertcb.Size = new Size(53, 24);
            Triggertcb.TabIndex = 38;
            // 
            // Triggerp
            // 
            Triggerp.BorderColour = Color.Empty;
            Triggerp.CustomColour = false;
            Triggerp.FlatBottom = false;
            Triggerp.FlatTop = false;
            Triggerp.Location = new Point(243, 0);
            Triggerp.Name = "Triggerp";
            Triggerp.Padding = new Padding(5);
            Triggerp.Size = new Size(24, 24);
            Triggerp.TabIndex = 45;
            Triggerp.Text = "+";
            // 
            // Triggern
            // 
            Triggern.BackColor = Color.FromArgb(26, 26, 28);
            Triggern.BorderStyle = BorderStyle.FixedSingle;
            Triggern.ForeColor = Color.FromArgb(213, 213, 213);
            Triggern.Location = new Point(6, 29);
            Triggern.Name = "Triggern";
            Triggern.Size = new Size(229, 23);
            Triggern.TabIndex = 36;
            // 
            // Triggercb
            // 
            Triggercb.DrawMode = DrawMode.OwnerDrawVariable;
            Triggercb.FormattingEnabled = true;
            Triggercb.Location = new Point(58, 0);
            Triggercb.Name = "Triggercb";
            Triggercb.Size = new Size(179, 24);
            Triggercb.TabIndex = 0;
            // 
            // Triggerx
            // 
            Triggerx.BackColor = Color.FromArgb(26, 26, 28);
            Triggerx.BorderStyle = BorderStyle.FixedSingle;
            Triggerx.ForeColor = Color.FromArgb(213, 213, 213);
            Triggerx.Location = new Point(93, 59);
            Triggerx.Name = "Triggerx";
            Triggerx.Size = new Size(63, 23);
            Triggerx.TabIndex = 29;
            // 
            // Triggerxl
            // 
            Triggerxl.AutoSize = true;
            Triggerxl.Location = new Point(59, 63);
            Triggerxl.Name = "Triggerxl";
            Triggerxl.Size = new Size(28, 15);
            Triggerxl.TabIndex = 32;
            Triggerxl.Text = "XYZ";
            // 
            // Triggery
            // 
            Triggery.BackColor = Color.FromArgb(26, 26, 28);
            Triggery.BorderStyle = BorderStyle.FixedSingle;
            Triggery.ForeColor = Color.FromArgb(213, 213, 213);
            Triggery.Location = new Point(162, 59);
            Triggery.Name = "Triggery";
            Triggery.Size = new Size(63, 23);
            Triggery.TabIndex = 30;
            // 
            // Triggerz
            // 
            Triggerz.BackColor = Color.FromArgb(26, 26, 28);
            Triggerz.BorderStyle = BorderStyle.FixedSingle;
            Triggerz.ForeColor = Color.FromArgb(213, 213, 213);
            Triggerz.Location = new Point(231, 59);
            Triggerz.Name = "Triggerz";
            Triggerz.Size = new Size(63, 23);
            Triggerz.TabIndex = 31;
            // 
            // EPTHGB
            // 
            EPTHGB.Controls.Add(EPTHpm);
            EPTHGB.Controls.Add(EPTHm);
            EPTHGB.Controls.Add(EPTHnud);
            EPTHGB.Controls.Add(EPTHr);
            EPTHGB.Controls.Add(EPTHp);
            EPTHGB.Controls.Add(EPTHn);
            EPTHGB.Controls.Add(EPTHcb);
            EPTHGB.Controls.Add(EPTHx);
            EPTHGB.Controls.Add(EPTHxl);
            EPTHGB.Controls.Add(EPTHy);
            EPTHGB.Controls.Add(EPTHz);
            EPTHGB.Location = new Point(618, 313);
            EPTHGB.Name = "EPTHGB";
            EPTHGB.Size = new Size(300, 88);
            EPTHGB.TabIndex = 39;
            EPTHGB.TabStop = false;
            EPTHGB.Text = "EPTH";
            // 
            // EPTHpm
            // 
            EPTHpm.BorderColour = Color.Empty;
            EPTHpm.CustomColour = false;
            EPTHpm.FlatBottom = false;
            EPTHpm.FlatTop = false;
            EPTHpm.Location = new Point(38, 59);
            EPTHpm.Name = "EPTHpm";
            EPTHpm.Padding = new Padding(5);
            EPTHpm.Size = new Size(15, 23);
            EPTHpm.TabIndex = 49;
            EPTHpm.Text = "-";
            // 
            // EPTHm
            // 
            EPTHm.BorderColour = Color.Empty;
            EPTHm.CustomColour = false;
            EPTHm.FlatBottom = false;
            EPTHm.FlatTop = false;
            EPTHm.Location = new Point(270, 0);
            EPTHm.Name = "EPTHm";
            EPTHm.Padding = new Padding(5);
            EPTHm.Size = new Size(24, 24);
            EPTHm.TabIndex = 44;
            EPTHm.Text = "-";
            // 
            // EPTHnud
            // 
            EPTHnud.Location = new Point(6, 59);
            EPTHnud.Name = "EPTHnud";
            EPTHnud.Size = new Size(32, 23);
            EPTHnud.TabIndex = 48;
            // 
            // EPTHr
            // 
            EPTHr.BackColor = Color.FromArgb(26, 26, 28);
            EPTHr.BorderStyle = BorderStyle.FixedSingle;
            EPTHr.ForeColor = Color.FromArgb(213, 213, 213);
            EPTHr.Location = new Point(248, 59);
            EPTHr.Name = "EPTHr";
            EPTHr.Size = new Size(46, 23);
            EPTHr.TabIndex = 37;
            // 
            // EPTHp
            // 
            EPTHp.BorderColour = Color.Empty;
            EPTHp.CustomColour = false;
            EPTHp.FlatBottom = false;
            EPTHp.FlatTop = false;
            EPTHp.Location = new Point(243, 0);
            EPTHp.Name = "EPTHp";
            EPTHp.Padding = new Padding(5);
            EPTHp.Size = new Size(24, 24);
            EPTHp.TabIndex = 43;
            EPTHp.Text = "+";
            // 
            // EPTHn
            // 
            EPTHn.BackColor = Color.FromArgb(26, 26, 28);
            EPTHn.BorderStyle = BorderStyle.FixedSingle;
            EPTHn.ForeColor = Color.FromArgb(213, 213, 213);
            EPTHn.Location = new Point(6, 30);
            EPTHn.Name = "EPTHn";
            EPTHn.Size = new Size(288, 23);
            EPTHn.TabIndex = 36;
            // 
            // EPTHcb
            // 
            EPTHcb.DrawMode = DrawMode.OwnerDrawVariable;
            EPTHcb.FormattingEnabled = true;
            EPTHcb.Location = new Point(47, 0);
            EPTHcb.Name = "EPTHcb";
            EPTHcb.Size = new Size(190, 24);
            EPTHcb.TabIndex = 0;
            // 
            // EPTHx
            // 
            EPTHx.BackColor = Color.FromArgb(26, 26, 28);
            EPTHx.BorderStyle = BorderStyle.FixedSingle;
            EPTHx.ForeColor = Color.FromArgb(213, 213, 213);
            EPTHx.Location = new Point(92, 59);
            EPTHx.Name = "EPTHx";
            EPTHx.Size = new Size(46, 23);
            EPTHx.TabIndex = 29;
            // 
            // EPTHxl
            // 
            EPTHxl.AutoSize = true;
            EPTHxl.Location = new Point(56, 63);
            EPTHxl.Name = "EPTHxl";
            EPTHxl.Size = new Size(35, 15);
            EPTHxl.TabIndex = 32;
            EPTHxl.Text = "XYZR";
            // 
            // EPTHy
            // 
            EPTHy.BackColor = Color.FromArgb(26, 26, 28);
            EPTHy.BorderStyle = BorderStyle.FixedSingle;
            EPTHy.ForeColor = Color.FromArgb(213, 213, 213);
            EPTHy.Location = new Point(144, 59);
            EPTHy.Name = "EPTHy";
            EPTHy.Size = new Size(46, 23);
            EPTHy.TabIndex = 30;
            // 
            // EPTHz
            // 
            EPTHz.BackColor = Color.FromArgb(26, 26, 28);
            EPTHz.BorderStyle = BorderStyle.FixedSingle;
            EPTHz.ForeColor = Color.FromArgb(213, 213, 213);
            EPTHz.Location = new Point(196, 59);
            EPTHz.Name = "EPTHz";
            EPTHz.Size = new Size(46, 23);
            EPTHz.TabIndex = 31;
            // 
            // EMSDgb
            // 
            EMSDgb.Controls.Add(EMSDm);
            EMSDgb.Controls.Add(EMSDcb);
            EMSDgb.Controls.Add(EMSDp);
            EMSDgb.Controls.Add(EMSDs2l);
            EMSDgb.Controls.Add(EMSDs1);
            EMSDgb.Controls.Add(EMSDs2);
            EMSDgb.Controls.Add(EMSDcl);
            EMSDgb.Controls.Add(EMSDz);
            EMSDgb.Controls.Add(EMSDy);
            EMSDgb.Controls.Add(EMSDx);
            EMSDgb.Location = new Point(618, 166);
            EMSDgb.Name = "EMSDgb";
            EMSDgb.Size = new Size(300, 117);
            EMSDgb.TabIndex = 31;
            EMSDgb.TabStop = false;
            EMSDgb.Text = "EMSD";
            // 
            // EMSDm
            // 
            EMSDm.BorderColour = Color.Empty;
            EMSDm.CustomColour = false;
            EMSDm.FlatBottom = false;
            EMSDm.FlatTop = false;
            EMSDm.Location = new Point(270, 0);
            EMSDm.Name = "EMSDm";
            EMSDm.Padding = new Padding(5);
            EMSDm.Size = new Size(24, 24);
            EMSDm.TabIndex = 42;
            EMSDm.Text = "-";
            // 
            // EMSDcb
            // 
            EMSDcb.DrawMode = DrawMode.OwnerDrawVariable;
            EMSDcb.FormattingEnabled = true;
            EMSDcb.Location = new Point(51, 0);
            EMSDcb.Name = "EMSDcb";
            EMSDcb.Size = new Size(186, 24);
            EMSDcb.TabIndex = 39;
            // 
            // EMSDp
            // 
            EMSDp.BorderColour = Color.Empty;
            EMSDp.CustomColour = false;
            EMSDp.FlatBottom = false;
            EMSDp.FlatTop = false;
            EMSDp.Location = new Point(243, 0);
            EMSDp.Name = "EMSDp";
            EMSDp.Padding = new Padding(5);
            EMSDp.Size = new Size(24, 24);
            EMSDp.TabIndex = 41;
            EMSDp.Text = "+";
            // 
            // EMSDs2l
            // 
            EMSDs2l.AutoSize = true;
            EMSDs2l.Location = new Point(268, 63);
            EMSDs2l.Name = "EMSDs2l";
            EMSDs2l.Size = new Size(26, 15);
            EMSDs2l.TabIndex = 27;
            EMSDs2l.Text = ".psf";
            // 
            // EMSDs1
            // 
            EMSDs1.BackColor = Color.FromArgb(26, 26, 28);
            EMSDs1.BorderStyle = BorderStyle.FixedSingle;
            EMSDs1.ForeColor = Color.FromArgb(213, 213, 213);
            EMSDs1.Location = new Point(6, 30);
            EMSDs1.Name = "EMSDs1";
            EMSDs1.Size = new Size(288, 23);
            EMSDs1.TabIndex = 9;
            // 
            // EMSDs2
            // 
            EMSDs2.BackColor = Color.FromArgb(26, 26, 28);
            EMSDs2.BorderStyle = BorderStyle.FixedSingle;
            EMSDs2.ForeColor = Color.FromArgb(213, 213, 213);
            EMSDs2.Location = new Point(6, 59);
            EMSDs2.Name = "EMSDs2";
            EMSDs2.Size = new Size(256, 23);
            EMSDs2.TabIndex = 8;
            // 
            // EMSDcl
            // 
            EMSDcl.AutoSize = true;
            EMSDcl.Location = new Point(6, 92);
            EMSDcl.Name = "EMSDcl";
            EMSDcl.Size = new Size(28, 15);
            EMSDcl.TabIndex = 5;
            EMSDcl.Text = "XYZ";
            // 
            // EMSDz
            // 
            EMSDz.BackColor = Color.FromArgb(26, 26, 28);
            EMSDz.BorderStyle = BorderStyle.FixedSingle;
            EMSDz.ForeColor = Color.FromArgb(213, 213, 213);
            EMSDz.Location = new Point(213, 88);
            EMSDz.Name = "EMSDz";
            EMSDz.Size = new Size(81, 23);
            EMSDz.TabIndex = 3;
            // 
            // EMSDy
            // 
            EMSDy.BackColor = Color.FromArgb(26, 26, 28);
            EMSDy.BorderStyle = BorderStyle.FixedSingle;
            EMSDy.ForeColor = Color.FromArgb(213, 213, 213);
            EMSDy.Location = new Point(126, 88);
            EMSDy.Name = "EMSDy";
            EMSDy.Size = new Size(81, 23);
            EMSDy.TabIndex = 2;
            // 
            // EMSDx
            // 
            EMSDx.BackColor = Color.FromArgb(26, 26, 28);
            EMSDx.BorderStyle = BorderStyle.FixedSingle;
            EMSDx.ForeColor = Color.FromArgb(213, 213, 213);
            EMSDx.Location = new Point(39, 88);
            EMSDx.Name = "EMSDx";
            EMSDx.Size = new Size(81, 23);
            EMSDx.TabIndex = 0;
            // 
            // EME2gb
            // 
            EME2gb.Controls.Add(EME2m);
            EME2gb.Controls.Add(EME2p);
            EME2gb.Controls.Add(EME2r);
            EME2gb.Controls.Add(EME2cb);
            EME2gb.Controls.Add(EME2dsb);
            EME2gb.Controls.Add(EME2s5l);
            EME2gb.Controls.Add(EME2s4l);
            EME2gb.Controls.Add(EME2s3l);
            EME2gb.Controls.Add(EME2z);
            EME2gb.Controls.Add(EME2y);
            EME2gb.Controls.Add(EME2s5);
            EME2gb.Controls.Add(EME2s4);
            EME2gb.Controls.Add(EME2x);
            EME2gb.Controls.Add(EME2dgv);
            EME2gb.Controls.Add(EME2cl);
            EME2gb.Controls.Add(EME2s3);
            EME2gb.Controls.Add(EME2s2);
            EME2gb.Controls.Add(EME2s1);
            EME2gb.Controls.Add(EME2n);
            EME2gb.Location = new Point(6, 167);
            EME2gb.Name = "EME2gb";
            EME2gb.Size = new Size(606, 233);
            EME2gb.TabIndex = 13;
            EME2gb.TabStop = false;
            EME2gb.Text = "EME2";
            // 
            // EME2m
            // 
            EME2m.BorderColour = Color.Empty;
            EME2m.CustomColour = false;
            EME2m.FlatBottom = false;
            EME2m.FlatTop = false;
            EME2m.Location = new Point(333, 0);
            EME2m.Name = "EME2m";
            EME2m.Padding = new Padding(5);
            EME2m.Size = new Size(24, 24);
            EME2m.TabIndex = 28;
            EME2m.Text = "-";
            // 
            // EME2p
            // 
            EME2p.BorderColour = Color.Empty;
            EME2p.CustomColour = false;
            EME2p.FlatBottom = false;
            EME2p.FlatTop = false;
            EME2p.Location = new Point(306, 0);
            EME2p.Name = "EME2p";
            EME2p.Padding = new Padding(5);
            EME2p.Size = new Size(24, 24);
            EME2p.TabIndex = 27;
            EME2p.Text = "+";
            // 
            // EME2r
            // 
            EME2r.BackColor = Color.FromArgb(26, 26, 28);
            EME2r.BorderStyle = BorderStyle.FixedSingle;
            EME2r.ForeColor = Color.FromArgb(213, 213, 213);
            EME2r.Location = new Point(241, 204);
            EME2r.Name = "EME2r";
            EME2r.Size = new Size(59, 23);
            EME2r.TabIndex = 25;
            // 
            // EME2cb
            // 
            EME2cb.DrawMode = DrawMode.OwnerDrawVariable;
            EME2cb.FormattingEnabled = true;
            EME2cb.Location = new Point(49, 0);
            EME2cb.Name = "EME2cb";
            EME2cb.Size = new Size(251, 24);
            EME2cb.TabIndex = 24;
            // 
            // EME2dsb
            // 
            EME2dsb.Location = new Point(582, 30);
            EME2dsb.Maximum = 500;
            EME2dsb.Name = "EME2dsb";
            EME2dsb.Size = new Size(18, 197);
            EME2dsb.TabIndex = 14;
            EME2dsb.Text = "EME2dsb";
            // 
            // EME2s5l
            // 
            EME2s5l.AutoSize = true;
            EME2s5l.Location = new Point(271, 179);
            EME2s5l.Name = "EME2s5l";
            EME2s5l.Size = new Size(29, 15);
            EME2s5l.TabIndex = 23;
            EME2s5l.Text = ".veg";
            // 
            // EME2s4l
            // 
            EME2s4l.AutoSize = true;
            EME2s4l.Location = new Point(271, 150);
            EME2s4l.Name = "EME2s4l";
            EME2s4l.Size = new Size(29, 15);
            EME2s4l.TabIndex = 22;
            EME2s4l.Text = ".dds";
            // 
            // EME2s3l
            // 
            EME2s3l.AutoSize = true;
            EME2s3l.Location = new Point(267, 121);
            EME2s3l.Name = "EME2s3l";
            EME2s3l.Size = new Size(33, 15);
            EME2s3l.TabIndex = 21;
            EME2s3l.Text = ".amx";
            // 
            // EME2z
            // 
            EME2z.BackColor = Color.FromArgb(26, 26, 28);
            EME2z.BorderStyle = BorderStyle.FixedSingle;
            EME2z.ForeColor = Color.FromArgb(213, 213, 213);
            EME2z.Location = new Point(176, 204);
            EME2z.Name = "EME2z";
            EME2z.Size = new Size(59, 23);
            EME2z.TabIndex = 10;
            // 
            // EME2y
            // 
            EME2y.BackColor = Color.FromArgb(26, 26, 28);
            EME2y.BorderStyle = BorderStyle.FixedSingle;
            EME2y.ForeColor = Color.FromArgb(213, 213, 213);
            EME2y.Location = new Point(111, 204);
            EME2y.Name = "EME2y";
            EME2y.Size = new Size(59, 23);
            EME2y.TabIndex = 9;
            // 
            // EME2s5
            // 
            EME2s5.BackColor = Color.FromArgb(26, 26, 28);
            EME2s5.BorderStyle = BorderStyle.FixedSingle;
            EME2s5.ForeColor = Color.FromArgb(213, 213, 213);
            EME2s5.Location = new Point(6, 175);
            EME2s5.Name = "EME2s5";
            EME2s5.Size = new Size(259, 23);
            EME2s5.TabIndex = 18;
            // 
            // EME2s4
            // 
            EME2s4.BackColor = Color.FromArgb(26, 26, 28);
            EME2s4.BorderStyle = BorderStyle.FixedSingle;
            EME2s4.ForeColor = Color.FromArgb(213, 213, 213);
            EME2s4.Location = new Point(6, 146);
            EME2s4.Name = "EME2s4";
            EME2s4.Size = new Size(259, 23);
            EME2s4.TabIndex = 17;
            // 
            // EME2x
            // 
            EME2x.BackColor = Color.FromArgb(26, 26, 28);
            EME2x.BorderStyle = BorderStyle.FixedSingle;
            EME2x.ForeColor = Color.FromArgb(213, 213, 213);
            EME2x.Location = new Point(46, 204);
            EME2x.Name = "EME2x";
            EME2x.Size = new Size(59, 23);
            EME2x.TabIndex = 8;
            // 
            // EME2dgv
            // 
            EME2dgv.AllowUserToResizeColumns = false;
            EME2dgv.AllowUserToResizeRows = false;
            EME2dgv.BackgroundColor = Color.FromArgb(16, 16, 16);
            EME2dgv.BorderStyle = BorderStyle.None;
            EME2dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            EME2dgv.ColumnHeadersVisible = false;
            EME2dgv.Location = new Point(306, 30);
            EME2dgv.MultiSelect = false;
            EME2dgv.Name = "EME2dgv";
            EME2dgv.RowHeadersVisible = false;
            EME2dgv.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            EME2dgv.RowTemplate.Height = 25;
            EME2dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            EME2dgv.Size = new Size(293, 197);
            EME2dgv.TabIndex = 19;
            // 
            // EME2cl
            // 
            EME2cl.AutoSize = true;
            EME2cl.Location = new Point(6, 208);
            EME2cl.Name = "EME2cl";
            EME2cl.Size = new Size(35, 15);
            EME2cl.TabIndex = 11;
            EME2cl.Text = "XYZR";
            // 
            // EME2s3
            // 
            EME2s3.BackColor = Color.FromArgb(26, 26, 28);
            EME2s3.BorderStyle = BorderStyle.FixedSingle;
            EME2s3.ForeColor = Color.FromArgb(213, 213, 213);
            EME2s3.Location = new Point(6, 117);
            EME2s3.Name = "EME2s3";
            EME2s3.Size = new Size(255, 23);
            EME2s3.TabIndex = 16;
            // 
            // EME2s2
            // 
            EME2s2.BackColor = Color.FromArgb(26, 26, 28);
            EME2s2.BorderStyle = BorderStyle.FixedSingle;
            EME2s2.ForeColor = Color.FromArgb(213, 213, 213);
            EME2s2.Location = new Point(6, 88);
            EME2s2.Name = "EME2s2";
            EME2s2.Size = new Size(294, 23);
            EME2s2.TabIndex = 15;
            // 
            // EME2s1
            // 
            EME2s1.BackColor = Color.FromArgb(26, 26, 28);
            EME2s1.BorderStyle = BorderStyle.FixedSingle;
            EME2s1.ForeColor = Color.FromArgb(213, 213, 213);
            EME2s1.Location = new Point(6, 59);
            EME2s1.Name = "EME2s1";
            EME2s1.Size = new Size(294, 23);
            EME2s1.TabIndex = 14;
            // 
            // EME2n
            // 
            EME2n.BackColor = Color.FromArgb(26, 26, 28);
            EME2n.BorderStyle = BorderStyle.FixedSingle;
            EME2n.ForeColor = Color.FromArgb(213, 213, 213);
            EME2n.Location = new Point(6, 30);
            EME2n.Name = "EME2n";
            EME2n.Size = new Size(294, 23);
            EME2n.TabIndex = 12;
            // 
            // EMEFgb
            // 
            EMEFgb.Controls.Add(EMEFm);
            EMEFgb.Controls.Add(EMEFr);
            EMEFgb.Controls.Add(EMEFp);
            EMEFgb.Controls.Add(EMEFcb);
            EMEFgb.Controls.Add(EMEFs2l);
            EMEFgb.Controls.Add(EMEFs1);
            EMEFgb.Controls.Add(EMEFs2);
            EMEFgb.Controls.Add(EMEFxl);
            EMEFgb.Controls.Add(EMEFz);
            EMEFgb.Controls.Add(EMEFy);
            EMEFgb.Controls.Add(EMEFx);
            EMEFgb.Location = new Point(618, 22);
            EMEFgb.Name = "EMEFgb";
            EMEFgb.Size = new Size(300, 117);
            EMEFgb.TabIndex = 40;
            EMEFgb.TabStop = false;
            EMEFgb.Text = "EMEF";
            // 
            // EMEFm
            // 
            EMEFm.BorderColour = Color.Empty;
            EMEFm.CustomColour = false;
            EMEFm.FlatBottom = false;
            EMEFm.FlatTop = false;
            EMEFm.Location = new Point(270, 0);
            EMEFm.Name = "EMEFm";
            EMEFm.Padding = new Padding(5);
            EMEFm.Size = new Size(24, 24);
            EMEFm.TabIndex = 30;
            EMEFm.Text = "-";
            // 
            // EMEFr
            // 
            EMEFr.BackColor = Color.FromArgb(26, 26, 28);
            EMEFr.BorderStyle = BorderStyle.FixedSingle;
            EMEFr.ForeColor = Color.FromArgb(213, 213, 213);
            EMEFr.Location = new Point(237, 88);
            EMEFr.Name = "EMEFr";
            EMEFr.Size = new Size(57, 23);
            EMEFr.TabIndex = 40;
            // 
            // EMEFp
            // 
            EMEFp.BorderColour = Color.Empty;
            EMEFp.CustomColour = false;
            EMEFp.FlatBottom = false;
            EMEFp.FlatTop = false;
            EMEFp.Location = new Point(243, 0);
            EMEFp.Name = "EMEFp";
            EMEFp.Padding = new Padding(5);
            EMEFp.Size = new Size(24, 24);
            EMEFp.TabIndex = 29;
            EMEFp.Text = "+";
            // 
            // EMEFcb
            // 
            EMEFcb.DrawMode = DrawMode.OwnerDrawVariable;
            EMEFcb.FormattingEnabled = true;
            EMEFcb.Location = new Point(48, 0);
            EMEFcb.Name = "EMEFcb";
            EMEFcb.Size = new Size(189, 24);
            EMEFcb.TabIndex = 39;
            // 
            // EMEFs2l
            // 
            EMEFs2l.AutoSize = true;
            EMEFs2l.Location = new Point(265, 63);
            EMEFs2l.Name = "EMEFs2l";
            EMEFs2l.Size = new Size(29, 15);
            EMEFs2l.TabIndex = 27;
            EMEFs2l.Text = ".veg";
            // 
            // EMEFs1
            // 
            EMEFs1.BackColor = Color.FromArgb(26, 26, 28);
            EMEFs1.BorderStyle = BorderStyle.FixedSingle;
            EMEFs1.ForeColor = Color.FromArgb(213, 213, 213);
            EMEFs1.Location = new Point(6, 30);
            EMEFs1.Name = "EMEFs1";
            EMEFs1.Size = new Size(288, 23);
            EMEFs1.TabIndex = 9;
            // 
            // EMEFs2
            // 
            EMEFs2.BackColor = Color.FromArgb(26, 26, 28);
            EMEFs2.BorderStyle = BorderStyle.FixedSingle;
            EMEFs2.ForeColor = Color.FromArgb(213, 213, 213);
            EMEFs2.Location = new Point(6, 59);
            EMEFs2.Name = "EMEFs2";
            EMEFs2.Size = new Size(253, 23);
            EMEFs2.TabIndex = 8;
            // 
            // EMEFxl
            // 
            EMEFxl.AutoSize = true;
            EMEFxl.Location = new Point(6, 92);
            EMEFxl.Name = "EMEFxl";
            EMEFxl.Size = new Size(35, 15);
            EMEFxl.TabIndex = 5;
            EMEFxl.Text = "XYZR";
            // 
            // EMEFz
            // 
            EMEFz.BackColor = Color.FromArgb(26, 26, 28);
            EMEFz.BorderStyle = BorderStyle.FixedSingle;
            EMEFz.ForeColor = Color.FromArgb(213, 213, 213);
            EMEFz.Location = new Point(174, 88);
            EMEFz.Name = "EMEFz";
            EMEFz.Size = new Size(57, 23);
            EMEFz.TabIndex = 3;
            // 
            // EMEFy
            // 
            EMEFy.BackColor = Color.FromArgb(26, 26, 28);
            EMEFy.BorderStyle = BorderStyle.FixedSingle;
            EMEFy.ForeColor = Color.FromArgb(213, 213, 213);
            EMEFy.Location = new Point(111, 88);
            EMEFy.Name = "EMEFy";
            EMEFy.Size = new Size(57, 23);
            EMEFy.TabIndex = 2;
            // 
            // EMEFx
            // 
            EMEFx.BackColor = Color.FromArgb(26, 26, 28);
            EMEFx.BorderStyle = BorderStyle.FixedSingle;
            EMEFx.ForeColor = Color.FromArgb(213, 213, 213);
            EMEFx.Location = new Point(48, 88);
            EMEFx.Name = "EMEFx";
            EMEFx.Size = new Size(57, 23);
            EMEFx.TabIndex = 0;
            // 
            // ECAMgb
            // 
            ECAMgb.Controls.Add(ECAMr);
            ECAMgb.Controls.Add(ECAMcl);
            ECAMgb.Controls.Add(ECAMz);
            ECAMgb.Controls.Add(ECAMy);
            ECAMgb.Controls.Add(ECAMx);
            ECAMgb.Location = new Point(312, 22);
            ECAMgb.Name = "ECAMgb";
            ECAMgb.Size = new Size(300, 47);
            ECAMgb.TabIndex = 12;
            ECAMgb.TabStop = false;
            ECAMgb.Text = "ECAM";
            // 
            // ECAMr
            // 
            ECAMr.BackColor = Color.FromArgb(26, 26, 28);
            ECAMr.BorderStyle = BorderStyle.FixedSingle;
            ECAMr.ForeColor = Color.FromArgb(213, 213, 213);
            ECAMr.Location = new Point(236, 17);
            ECAMr.Name = "ECAMr";
            ECAMr.Size = new Size(57, 23);
            ECAMr.TabIndex = 29;
            // 
            // ECAMcl
            // 
            ECAMcl.AutoSize = true;
            ECAMcl.Location = new Point(6, 22);
            ECAMcl.Name = "ECAMcl";
            ECAMcl.Size = new Size(35, 15);
            ECAMcl.TabIndex = 5;
            ECAMcl.Text = "XYZR";
            // 
            // ECAMz
            // 
            ECAMz.BackColor = Color.FromArgb(26, 26, 28);
            ECAMz.BorderStyle = BorderStyle.FixedSingle;
            ECAMz.ForeColor = Color.FromArgb(213, 213, 213);
            ECAMz.Location = new Point(173, 17);
            ECAMz.Name = "ECAMz";
            ECAMz.Size = new Size(57, 23);
            ECAMz.TabIndex = 3;
            // 
            // ECAMy
            // 
            ECAMy.BackColor = Color.FromArgb(26, 26, 28);
            ECAMy.BorderStyle = BorderStyle.FixedSingle;
            ECAMy.ForeColor = Color.FromArgb(213, 213, 213);
            ECAMy.Location = new Point(110, 17);
            ECAMy.Name = "ECAMy";
            ECAMy.Size = new Size(57, 23);
            ECAMy.TabIndex = 2;
            // 
            // ECAMx
            // 
            ECAMx.BackColor = Color.FromArgb(26, 26, 28);
            ECAMx.BorderStyle = BorderStyle.FixedSingle;
            ECAMx.ForeColor = Color.FromArgb(213, 213, 213);
            ECAMx.Location = new Point(47, 17);
            ECAMx.Name = "ECAMx";
            ECAMx.Size = new Size(57, 23);
            ECAMx.TabIndex = 0;
            // 
            // EMAPgb
            // 
            EMAPgb.Controls.Add(EMAPs3l);
            EMAPgb.Controls.Add(EMAPs2l);
            EMAPgb.Controls.Add(EMAPs1l);
            EMAPgb.Controls.Add(EMAPilcb);
            EMAPgb.Controls.Add(EMAPslb);
            EMAPgb.Controls.Add(EMAPs3);
            EMAPgb.Controls.Add(EMAPs2);
            EMAPgb.Controls.Add(EMAPs1);
            EMAPgb.Location = new Point(6, 22);
            EMAPgb.Name = "EMAPgb";
            EMAPgb.Size = new Size(300, 139);
            EMAPgb.TabIndex = 1;
            EMAPgb.TabStop = false;
            EMAPgb.Text = "EMAP";
            // 
            // EMEPgb
            // 
            EMEPgb.Controls.Add(EMEPm);
            EMEPgb.Controls.Add(EMEPp);
            EMEPgb.Controls.Add(EMEPcb);
            EMEPgb.Controls.Add(EMEPr);
            EMEPgb.Controls.Add(EMEPcl);
            EMEPgb.Controls.Add(EMEPepl);
            EMEPgb.Controls.Add(EMEPz);
            EMEPgb.Controls.Add(EMEPy);
            EMEPgb.Controls.Add(EMEPnud);
            EMEPgb.Controls.Add(EMEPx);
            EMEPgb.Location = new Point(312, 75);
            EMEPgb.Name = "EMEPgb";
            EMEPgb.Size = new Size(300, 86);
            EMEPgb.TabIndex = 11;
            EMEPgb.TabStop = false;
            EMEPgb.Text = "EMEP";
            // 
            // EMEPm
            // 
            EMEPm.BorderColour = Color.Empty;
            EMEPm.CustomColour = false;
            EMEPm.FlatBottom = false;
            EMEPm.FlatTop = false;
            EMEPm.Location = new Point(270, 0);
            EMEPm.Name = "EMEPm";
            EMEPm.Padding = new Padding(5);
            EMEPm.Size = new Size(24, 24);
            EMEPm.TabIndex = 30;
            EMEPm.Text = "-";
            // 
            // EMEPp
            // 
            EMEPp.BorderColour = Color.Empty;
            EMEPp.CustomColour = false;
            EMEPp.FlatBottom = false;
            EMEPp.FlatTop = false;
            EMEPp.Location = new Point(243, 0);
            EMEPp.Name = "EMEPp";
            EMEPp.Padding = new Padding(5);
            EMEPp.Size = new Size(24, 24);
            EMEPp.TabIndex = 29;
            EMEPp.Text = "+";
            // 
            // EMEPcb
            // 
            EMEPcb.DrawMode = DrawMode.OwnerDrawVariable;
            EMEPcb.FormattingEnabled = true;
            EMEPcb.Location = new Point(49, 0);
            EMEPcb.Name = "EMEPcb";
            EMEPcb.Size = new Size(188, 24);
            EMEPcb.TabIndex = 27;
            // 
            // EMEPr
            // 
            EMEPr.BackColor = Color.FromArgb(26, 26, 28);
            EMEPr.BorderStyle = BorderStyle.FixedSingle;
            EMEPr.ForeColor = Color.FromArgb(213, 213, 213);
            EMEPr.Location = new Point(237, 57);
            EMEPr.Name = "EMEPr";
            EMEPr.Size = new Size(57, 23);
            EMEPr.TabIndex = 27;
            // 
            // EMEPcl
            // 
            EMEPcl.AutoSize = true;
            EMEPcl.Location = new Point(6, 61);
            EMEPcl.Name = "EMEPcl";
            EMEPcl.Size = new Size(35, 15);
            EMEPcl.TabIndex = 5;
            EMEPcl.Text = "XYZR";
            // 
            // EMEPepl
            // 
            EMEPepl.AutoSize = true;
            EMEPepl.Location = new Point(6, 32);
            EMEPepl.Name = "EMEPepl";
            EMEPepl.Size = new Size(65, 15);
            EMEPepl.TabIndex = 4;
            EMEPepl.Text = "Entry Point";
            // 
            // EMEPz
            // 
            EMEPz.BackColor = Color.FromArgb(26, 26, 28);
            EMEPz.BorderStyle = BorderStyle.FixedSingle;
            EMEPz.ForeColor = Color.FromArgb(213, 213, 213);
            EMEPz.Location = new Point(174, 57);
            EMEPz.Name = "EMEPz";
            EMEPz.Size = new Size(57, 23);
            EMEPz.TabIndex = 3;
            // 
            // EMEPy
            // 
            EMEPy.BackColor = Color.FromArgb(26, 26, 28);
            EMEPy.BorderStyle = BorderStyle.FixedSingle;
            EMEPy.ForeColor = Color.FromArgb(213, 213, 213);
            EMEPy.Location = new Point(111, 57);
            EMEPy.Name = "EMEPy";
            EMEPy.Size = new Size(57, 23);
            EMEPy.TabIndex = 2;
            // 
            // EMEPnud
            // 
            EMEPnud.Location = new Point(77, 28);
            EMEPnud.Name = "EMEPnud";
            EMEPnud.Size = new Size(49, 23);
            EMEPnud.TabIndex = 1;
            // 
            // EMEPx
            // 
            EMEPx.BackColor = Color.FromArgb(26, 26, 28);
            EMEPx.BorderStyle = BorderStyle.FixedSingle;
            EMEPx.ForeColor = Color.FromArgb(213, 213, 213);
            EMEPx.Location = new Point(49, 57);
            EMEPx.Name = "EMEPx";
            EMEPx.Size = new Size(57, 23);
            EMEPx.TabIndex = 0;
            // 
            // EME2tmr
            // 
            EME2tmr.Interval = 7;
            // 
            // MainMenuStrip
            // 
            DarkMainMenuStrip.BackColor = Color.FromArgb(31, 31, 32);
            DarkMainMenuStrip.ForeColor = Color.FromArgb(213, 213, 213);
            DarkMainMenuStrip.Items.AddRange(new ToolStripItem[] { FileToolStripMenuItem, OptionsToolStripMenuItem, ToolsToolStripMenuItem });
            DarkMainMenuStrip.Location = new Point(0, 0);
            DarkMainMenuStrip.Name = "DarkMainMenuStrip";
            DarkMainMenuStrip.Padding = new Padding(3, 2, 0, 2);
            DarkMainMenuStrip.Size = new Size(624, 24);
            DarkMainMenuStrip.TabIndex = 14;
            DarkMainMenuStrip.Text = "DarkMenuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            FileToolStripMenuItem.BackColor = Color.FromArgb(31, 31, 32);
            FileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { NewToolStripMenuItem, OpenToolStripMenuItem, OpenFromgrpToolStripMenuItem, SaveToolStripMenuItem });
            FileToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            FileToolStripMenuItem.Size = new Size(37, 20);
            FileToolStripMenuItem.Text = "File";
            // 
            // NewToolStripMenuItem
            // 
            NewToolStripMenuItem.BackColor = Color.FromArgb(31, 31, 32);
            NewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { AmoToolStripMenuItem, ArmToolStripMenuItem, ConToolStripMenuItem, CrtToolStripMenuItem, DorToolStripMenuItem, IntToolStripMenuItem, ItmToolStripMenuItem, MapToolStripMenuItem, UseToolStripMenuItem, WeaToolStripMenuItem });
            NewToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            NewToolStripMenuItem.Name = "NewToolStripMenuItem";
            NewToolStripMenuItem.Size = new Size(158, 22);
            NewToolStripMenuItem.Text = "New";
            // 
            // AmoToolStripMenuItem
            // 
            AmoToolStripMenuItem.BackColor = Color.FromArgb(31, 31, 32);
            AmoToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            AmoToolStripMenuItem.Name = "AmoToolStripMenuItem";
            AmoToolStripMenuItem.Size = new Size(101, 22);
            AmoToolStripMenuItem.Text = ".amo";
            // 
            // ArmToolStripMenuItem
            // 
            ArmToolStripMenuItem.BackColor = Color.FromArgb(31, 31, 32);
            ArmToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            ArmToolStripMenuItem.Name = "ArmToolStripMenuItem";
            ArmToolStripMenuItem.Size = new Size(101, 22);
            ArmToolStripMenuItem.Text = ".arm";
            // 
            // ConToolStripMenuItem
            // 
            ConToolStripMenuItem.BackColor = Color.FromArgb(31, 31, 32);
            ConToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            ConToolStripMenuItem.Name = "ConToolStripMenuItem";
            ConToolStripMenuItem.Size = new Size(101, 22);
            ConToolStripMenuItem.Text = ".con";
            // 
            // CrtToolStripMenuItem
            // 
            CrtToolStripMenuItem.BackColor = Color.FromArgb(31, 31, 32);
            CrtToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            CrtToolStripMenuItem.Name = "CrtToolStripMenuItem";
            CrtToolStripMenuItem.Size = new Size(101, 22);
            CrtToolStripMenuItem.Text = ".crt";
            // 
            // DorToolStripMenuItem
            // 
            DorToolStripMenuItem.BackColor = Color.FromArgb(31, 31, 32);
            DorToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            DorToolStripMenuItem.Name = "DorToolStripMenuItem";
            DorToolStripMenuItem.Size = new Size(101, 22);
            DorToolStripMenuItem.Text = ".dor";
            // 
            // IntToolStripMenuItem
            // 
            IntToolStripMenuItem.BackColor = Color.FromArgb(31, 31, 32);
            IntToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            IntToolStripMenuItem.Name = "IntToolStripMenuItem";
            IntToolStripMenuItem.Size = new Size(101, 22);
            IntToolStripMenuItem.Text = ".int";
            // 
            // ItmToolStripMenuItem
            // 
            ItmToolStripMenuItem.BackColor = Color.FromArgb(31, 31, 32);
            ItmToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            ItmToolStripMenuItem.Name = "ItmToolStripMenuItem";
            ItmToolStripMenuItem.Size = new Size(101, 22);
            ItmToolStripMenuItem.Text = ".itm";
            // 
            // MapToolStripMenuItem
            // 
            MapToolStripMenuItem.BackColor = Color.FromArgb(31, 31, 32);
            MapToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            MapToolStripMenuItem.Name = "MapToolStripMenuItem";
            MapToolStripMenuItem.Size = new Size(101, 22);
            MapToolStripMenuItem.Text = ".map";
            // 
            // UseToolStripMenuItem
            // 
            UseToolStripMenuItem.BackColor = Color.FromArgb(31, 31, 32);
            UseToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            UseToolStripMenuItem.Name = "UseToolStripMenuItem";
            UseToolStripMenuItem.Size = new Size(101, 22);
            UseToolStripMenuItem.Text = ".use";
            // 
            // WeaToolStripMenuItem
            // 
            WeaToolStripMenuItem.BackColor = Color.FromArgb(31, 31, 32);
            WeaToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            WeaToolStripMenuItem.Name = "WeaToolStripMenuItem";
            WeaToolStripMenuItem.Size = new Size(101, 22);
            WeaToolStripMenuItem.Text = ".wea";
            // 
            // OpenToolStripMenuItem
            // 
            OpenToolStripMenuItem.BackColor = Color.FromArgb(31, 31, 32);
            OpenToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            OpenToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            OpenToolStripMenuItem.Size = new Size(158, 22);
            OpenToolStripMenuItem.Text = "Open";
            // 
            // OpenFromgrpToolStripMenuItem
            // 
            OpenFromgrpToolStripMenuItem.BackColor = Color.FromArgb(31, 31, 32);
            OpenFromgrpToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            OpenFromgrpToolStripMenuItem.Name = "OpenFromgrpToolStripMenuItem";
            OpenFromgrpToolStripMenuItem.Size = new Size(158, 22);
            OpenFromgrpToolStripMenuItem.Text = "Open From .grp";
            // 
            // SaveToolStripMenuItem
            // 
            SaveToolStripMenuItem.BackColor = Color.FromArgb(31, 31, 32);
            SaveToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            SaveToolStripMenuItem.Name = "SaveToolStripMenuItem";
            SaveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            SaveToolStripMenuItem.Size = new Size(158, 22);
            SaveToolStripMenuItem.Text = "Save";
            // 
            // OptionsToolStripMenuItem
            // 
            OptionsToolStripMenuItem.BackColor = Color.FromArgb(31, 31, 32);
            OptionsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { SetEnglishstfLocationToolStripMenuItem, EnableSTFEdit });
            OptionsToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem";
            OptionsToolStripMenuItem.Size = new Size(61, 20);
            OptionsToolStripMenuItem.Text = "Options";
            // 
            // SetEnglishstfLocationToolStripMenuItem
            // 
            SetEnglishstfLocationToolStripMenuItem.BackColor = Color.FromArgb(31, 31, 32);
            SetEnglishstfLocationToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            SetEnglishstfLocationToolStripMenuItem.Name = "SetEnglishstfLocationToolStripMenuItem";
            SetEnglishstfLocationToolStripMenuItem.Size = new Size(206, 22);
            SetEnglishstfLocationToolStripMenuItem.Text = "Set English.stf Location";
            // 
            // EnableSTFEdit
            // 
            EnableSTFEdit.BackColor = Color.FromArgb(16, 16, 17);
            EnableSTFEdit.CheckOnClick = true;
            EnableSTFEdit.ForeColor = Color.FromArgb(213, 213, 213);
            EnableSTFEdit.Name = "EnableSTFEdit";
            EnableSTFEdit.Size = new Size(206, 22);
            EnableSTFEdit.Text = "Enable English.stf Editing";
            EnableSTFEdit.ToolTipText = "Determines whether individual" + '\r' + '\n' + "STF strings can be edited." + '\r' + '\n' + "With this disabled, yo" + "u can still" + '\r' + '\n' + "change which string is referenced.";
            // 
            // ToolsToolStripMenuItem
            // 
            ToolsToolStripMenuItem.BackColor = Color.FromArgb(31, 31, 32);
            ToolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { StfToolStripMenuItem, TxtTostfToolStripMenuItem, ExtractgrpFilesToolStripMenuItem, ExtractAndConvertToolStripMenuItem, GrpBrowserToolStripMenuItem });
            ToolsToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem";
            ToolsToolStripMenuItem.Size = new Size(46, 20);
            ToolsToolStripMenuItem.Text = "Tools";
            // 
            // StfToolStripMenuItem
            // 
            StfToolStripMenuItem.BackColor = Color.FromArgb(31, 31, 32);
            StfToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            StfToolStripMenuItem.Name = "StfToolStripMenuItem";
            StfToolStripMenuItem.Size = new Size(210, 22);
            StfToolStripMenuItem.Text = ".stf to .txt";
            // 
            // TxtTostfToolStripMenuItem
            // 
            TxtTostfToolStripMenuItem.BackColor = Color.FromArgb(31, 31, 32);
            TxtTostfToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            TxtTostfToolStripMenuItem.Name = "TxtTostfToolStripMenuItem";
            TxtTostfToolStripMenuItem.Size = new Size(210, 22);
            TxtTostfToolStripMenuItem.Text = ".txt to .stf";
            // 
            // ExtractgrpFilesToolStripMenuItem
            // 
            ExtractgrpFilesToolStripMenuItem.BackColor = Color.FromArgb(31, 31, 32);
            ExtractgrpFilesToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            ExtractgrpFilesToolStripMenuItem.Name = "ExtractgrpFilesToolStripMenuItem";
            ExtractgrpFilesToolStripMenuItem.Size = new Size(210, 22);
            ExtractgrpFilesToolStripMenuItem.Text = "Extract All Data";
            // 
            // ExtractAndConvertToolStripMenuItem
            // 
            ExtractAndConvertToolStripMenuItem.BackColor = Color.FromArgb(31, 31, 32);
            ExtractAndConvertToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            ExtractAndConvertToolStripMenuItem.Name = "ExtractAndConvertToolStripMenuItem";
            ExtractAndConvertToolStripMenuItem.Size = new Size(210, 22);
            ExtractAndConvertToolStripMenuItem.Text = "Extract + Convert All Data";
            // 
            // GrpBrowserToolStripMenuItem
            // 
            GrpBrowserToolStripMenuItem.BackColor = Color.FromArgb(31, 31, 32);
            GrpBrowserToolStripMenuItem.ForeColor = Color.FromArgb(213, 213, 213);
            GrpBrowserToolStripMenuItem.Name = "GrpBrowserToolStripMenuItem";
            GrpBrowserToolStripMenuItem.Size = new Size(210, 22);
            GrpBrowserToolStripMenuItem.Text = ".grp Browser";
            // 
            // ToolTip
            // 
            ToolTip.OwnerDraw = true;
            // 
            // GWAMsf
            // 
            GWAMtlp.SetColumnSpan(GWAMsf, 2);
            GWAMsf.Dock = DockStyle.Fill;
            GWAMsf.Location = new Point(198, 19);
            GWAMsf.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            GWAMsf.Name = "GWAMsf";
            GWAMsf.Size = new Size(94, 23);
            GWAMsf.TabIndex = 54;
            ToolTip.SetToolTip(GWAMsf, "You should set this to 0 for a .CRT");
            // 
            // EEN2dgv
            // 
            EEN2dgv.AllowUserToResizeColumns = false;
            EEN2dgv.AllowUserToResizeRows = false;
            EEN2dgv.BackgroundColor = Color.FromArgb(16, 16, 16);
            EEN2dgv.BorderStyle = BorderStyle.None;
            EEN2dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            EEN2dgv.ColumnHeadersVisible = false;
            EEN2dgv.Location = new Point(307, 48);
            EEN2dgv.MultiSelect = false;
            EEN2dgv.Name = "EEN2dgv";
            EEN2dgv.RowHeadersVisible = false;
            EEN2dgv.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            EEN2dgv.RowTemplate.Height = 25;
            EEN2dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            EEN2dgv.Size = new Size(293, 202);
            EEN2dgv.TabIndex = 36;
            // 
            // CRTgb
            // 
            CRTgb.Controls.Add(GCHRgb);
            CRTgb.Controls.Add(GCREgb);
            CRTgb.Controls.Add(GWAMgb);
            CRTgb.Location = new Point(12, 517);
            CRTgb.Name = "CRTgb";
            CRTgb.Size = new Size(924, 532);
            CRTgb.TabIndex = 15;
            CRTgb.TabStop = false;
            CRTgb.Text = ".CRT Editor";
            // 
            // GCHRgb
            // 
            GCHRgb.Controls.Add(GCHRn);
            GCHRgb.Location = new Point(618, 475);
            GCHRgb.Name = "GCHRgb";
            GCHRgb.Size = new Size(300, 51);
            GCHRgb.TabIndex = 5;
            GCHRgb.TabStop = false;
            GCHRgb.Text = "GCHR";
            // 
            // GCHRn
            // 
            GCHRn.BackColor = Color.FromArgb(26, 26, 28);
            GCHRn.BorderStyle = BorderStyle.FixedSingle;
            GCHRn.ForeColor = Color.FromArgb(213, 213, 213);
            GCHRn.Location = new Point(6, 22);
            GCHRn.Name = "GCHRn";
            GCHRn.Size = new Size(288, 23);
            GCHRn.TabIndex = 0;
            // 
            // GCREgb
            // 
            GCREgb.Controls.Add(GCREdsb);
            GCREgb.Controls.Add(GCREtlp);
            GCREgb.Location = new Point(6, 284);
            GCREgb.Name = "GCREgb";
            GCREgb.Padding = new Padding(2, 0, 2, 2);
            GCREgb.Size = new Size(606, 242);
            GCREgb.TabIndex = 3;
            GCREgb.TabStop = false;
            GCREgb.Text = "GCRE";
            // 
            // GCREdsb
            // 
            GCREdsb.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            GCREdsb.Location = new Point(584, 124);
            GCREdsb.Maximum = 500;
            GCREdsb.Name = "GCREdsb";
            GCREdsb.Size = new Size(18, 113);
            GCREdsb.TabIndex = 37;
            GCREdsb.Text = "DarkScrollBar2";
            // 
            // GCREtlp
            // 
            GCREtlp.ColumnCount = 6;
            GCREtlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33444f));
            GCREtlp.ColumnStyles.Add(new ColumnStyle());
            GCREtlp.ColumnStyles.Add(new ColumnStyle());
            GCREtlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33445f));
            GCREtlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33111f));
            GCREtlp.ColumnStyles.Add(new ColumnStyle());
            GCREtlp.Controls.Add(GCREdgv, 3, 5);
            GCREtlp.Controls.Add(GCREskv, 1, 3);
            GCREtlp.Controls.Add(GCREspv, 1, 1);
            GCREtlp.Controls.Add(GCREspl, 0, 0);
            GCREtlp.Controls.Add(GCREspcb, 0, 1);
            GCREtlp.Controls.Add(GCREskl, 0, 2);
            GCREtlp.Controls.Add(GCREtrl, 0, 4);
            GCREtlp.Controls.Add(GCREskcb, 0, 3);
            GCREtlp.Controls.Add(GCREtrcb, 0, 5);
            GCREtlp.Controls.Add(GCREtsl, 0, 6);
            GCREtlp.Controls.Add(GCREtscb, 0, 7);
            GCREtlp.Controls.Add(GCREtrv, 1, 5);
            GCREtlp.Controls.Add(GCREtsv, 1, 7);
            GCREtlp.Controls.Add(GCREpl, 0, 8);
            GCREtlp.Controls.Add(GCREp, 0, 9);
            GCREtlp.Controls.Add(GCREsocl, 3, 0);
            GCREtlp.Controls.Add(GCREsocm, 3, 3);
            GCREtlp.Controls.Add(GCREsoct, 4, 3);
            GCREtlp.Controls.Add(GCREsocml, 3, 2);
            GCREtlp.Controls.Add(GCREsoctl, 4, 2);
            GCREtlp.Controls.Add(GCREsoccb, 3, 1);
            GCREtlp.Controls.Add(GCREeil, 3, 4);
            GCREtlp.Controls.Add(GCREsoctel, 5, 3);
            GCREtlp.Controls.Add(GCREpel, 2, 9);
            GCREtlp.Controls.Add(GCREage, 4, 1);
            GCREtlp.Controls.Add(GCREagel, 4, 0);
            GCREtlp.Dock = DockStyle.Fill;
            GCREtlp.Location = new Point(2, 16);
            GCREtlp.Name = "GCREtlp";
            GCREtlp.RowCount = 10;
            GCREtlp.RowStyles.Add(new RowStyle());
            GCREtlp.RowStyles.Add(new RowStyle());
            GCREtlp.RowStyles.Add(new RowStyle());
            GCREtlp.RowStyles.Add(new RowStyle());
            GCREtlp.RowStyles.Add(new RowStyle());
            GCREtlp.RowStyles.Add(new RowStyle());
            GCREtlp.RowStyles.Add(new RowStyle());
            GCREtlp.RowStyles.Add(new RowStyle());
            GCREtlp.RowStyles.Add(new RowStyle());
            GCREtlp.RowStyles.Add(new RowStyle());
            GCREtlp.Size = new Size(602, 224);
            GCREtlp.TabIndex = 0;
            // 
            // GCREdgv
            // 
            GCREdgv.AllowUserToResizeColumns = false;
            GCREdgv.AllowUserToResizeRows = false;
            GCREdgv.BackgroundColor = Color.FromArgb(16, 16, 16);
            GCREdgv.BorderStyle = BorderStyle.None;
            GCREdgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            GCREdgv.ColumnHeadersVisible = false;
            GCREtlp.SetColumnSpan(GCREdgv, 3);
            GCREdgv.Dock = DockStyle.Fill;
            GCREdgv.Location = new Point(235, 108);
            GCREdgv.MultiSelect = false;
            GCREdgv.Name = "GCREdgv";
            GCREdgv.RowHeadersVisible = false;
            GCREdgv.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            GCREtlp.SetRowSpan(GCREdgv, 5);
            GCREdgv.RowTemplate.Height = 25;
            GCREdgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            GCREdgv.Size = new Size(364, 113);
            GCREdgv.TabIndex = 37;
            // 
            // GCREskv
            // 
            GCREtlp.SetColumnSpan(GCREskv, 2);
            GCREskv.Dock = DockStyle.Fill;
            GCREskv.Location = new Point(170, 63);
            GCREskv.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            GCREskv.Name = "GCREskv";
            GCREskv.Size = new Size(59, 23);
            GCREskv.TabIndex = 66;
            // 
            // GCREspv
            // 
            GCREtlp.SetColumnSpan(GCREspv, 2);
            GCREspv.Dock = DockStyle.Fill;
            GCREspv.Location = new Point(170, 18);
            GCREspv.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            GCREspv.Name = "GCREspv";
            GCREspv.Size = new Size(59, 23);
            GCREspv.TabIndex = 59;
            // 
            // GCREspl
            // 
            GCREspl.AutoSize = true;
            GCREspl.Location = new Point(3, 0);
            GCREspl.Name = "GCREspl";
            GCREspl.Size = new Size(44, 15);
            GCREspl.TabIndex = 0;
            GCREspl.Text = "Special";
            // 
            // GCREspcb
            // 
            GCREspcb.Dock = DockStyle.Fill;
            GCREspcb.DrawMode = DrawMode.OwnerDrawVariable;
            GCREspcb.FormattingEnabled = true;
            GCREspcb.Items.AddRange(new object[] { "Strength", "Perception", "Endurance", "Charisma", "Intelligence", "Agility", "Luck" });
            GCREspcb.Location = new Point(3, 18);
            GCREspcb.Name = "GCREspcb";
            GCREspcb.Size = new Size(161, 24);
            GCREspcb.TabIndex = 1;
            // 
            // GCREskl
            // 
            GCREskl.AutoSize = true;
            GCREskl.Location = new Point(3, 45);
            GCREskl.Name = "GCREskl";
            GCREskl.Size = new Size(33, 15);
            GCREskl.TabIndex = 60;
            GCREskl.Text = "Skills";
            // 
            // GCREtrl
            // 
            GCREtrl.AutoSize = true;
            GCREtrl.Location = new Point(3, 90);
            GCREtrl.Name = "GCREtrl";
            GCREtrl.Size = new Size(34, 15);
            GCREtrl.TabIndex = 61;
            GCREtrl.Text = "Traits";
            // 
            // GCREskcb
            // 
            GCREskcb.Dock = DockStyle.Fill;
            GCREskcb.DrawMode = DrawMode.OwnerDrawVariable;
            GCREskcb.FormattingEnabled = true;
            GCREskcb.Items.AddRange(new object[] { "Firearms", "Melee", "Unarmed", "Barter", "Persuasion", "Deception", "Mechanics", "Medic", "Outdoorsman", "Science", "Security", "Sneak", "Steal" });
            GCREskcb.Location = new Point(3, 63);
            GCREskcb.Name = "GCREskcb";
            GCREskcb.Size = new Size(161, 24);
            GCREskcb.TabIndex = 62;
            // 
            // GCREtrcb
            // 
            GCREtrcb.Dock = DockStyle.Fill;
            GCREtrcb.DrawMode = DrawMode.OwnerDrawVariable;
            GCREtrcb.FormattingEnabled = true;
            GCREtrcb.Items.AddRange(new object[] { "Bruiser", "Chem Reliant", "Clean Living", "Fast Shot", "Feral Kid", "Finesse", "Gifted", "Good Natured", "Increased Metabolism", "Kamikaze", "Night Person", "One Hander", "One In a Million", "Red Scare", "Skilled", "Small Frame" });
            GCREtrcb.Location = new Point(3, 108);
            GCREtrcb.Name = "GCREtrcb";
            GCREtrcb.Size = new Size(161, 24);
            GCREtrcb.TabIndex = 63;
            // 
            // GCREtsl
            // 
            GCREtsl.AutoSize = true;
            GCREtsl.Location = new Point(3, 135);
            GCREtsl.Name = "GCREtsl";
            GCREtsl.Size = new Size(54, 15);
            GCREtsl.TabIndex = 64;
            GCREtsl.Text = "Tag Skills";
            // 
            // GCREtscb
            // 
            GCREtscb.Dock = DockStyle.Fill;
            GCREtscb.DrawMode = DrawMode.OwnerDrawVariable;
            GCREtscb.FormattingEnabled = true;
            GCREtscb.Items.AddRange(new object[] { "Firearms", "Melee", "Unarmed", "Barter", "Persuasion", "Deception", "Mechanics", "Medic", "Outdoorsman", "Science", "Security", "Sneak", "Steal" });
            GCREtscb.Location = new Point(3, 153);
            GCREtscb.Name = "GCREtscb";
            GCREtscb.Size = new Size(161, 24);
            GCREtscb.TabIndex = 65;
            // 
            // GCREtrv
            // 
            GCREtrv.AutoSize = true;
            GCREtlp.SetColumnSpan(GCREtrv, 2);
            GCREtrv.Dock = DockStyle.Left;
            GCREtrv.Location = new Point(170, 108);
            GCREtrv.Name = "GCREtrv";
            GCREtrv.Offset = -2;
            GCREtrv.Size = new Size(59, 24);
            GCREtrv.TabIndex = 67;
            GCREtrv.Text = "Active";
            // 
            // GCREtsv
            // 
            GCREtsv.AutoSize = true;
            GCREtlp.SetColumnSpan(GCREtsv, 2);
            GCREtsv.Dock = DockStyle.Left;
            GCREtsv.Location = new Point(170, 153);
            GCREtsv.Name = "GCREtsv";
            GCREtsv.Offset = -2;
            GCREtsv.Size = new Size(59, 24);
            GCREtsv.TabIndex = 68;
            GCREtsv.Text = "Active";
            // 
            // GCREpl
            // 
            GCREpl.AutoSize = true;
            GCREpl.Location = new Point(3, 180);
            GCREpl.Name = "GCREpl";
            GCREpl.Size = new Size(46, 15);
            GCREpl.TabIndex = 69;
            GCREpl.Text = "Portrait";
            // 
            // GCREp
            // 
            GCREp.BackColor = Color.FromArgb(26, 26, 28);
            GCREp.BorderStyle = BorderStyle.FixedSingle;
            GCREtlp.SetColumnSpan(GCREp, 2);
            GCREp.Dock = DockStyle.Fill;
            GCREp.ForeColor = Color.FromArgb(213, 213, 213);
            GCREp.Location = new Point(3, 198);
            GCREp.Name = "GCREp";
            GCREp.Size = new Size(191, 23);
            GCREp.TabIndex = 70;
            // 
            // GCREsocl
            // 
            GCREsocl.AutoSize = true;
            GCREsocl.Location = new Point(235, 0);
            GCREsocl.Name = "GCREsocl";
            GCREsocl.Size = new Size(42, 15);
            GCREsocl.TabIndex = 71;
            GCREsocl.Text = "Socket";
            // 
            // GCREsocm
            // 
            GCREsocm.BackColor = Color.FromArgb(26, 26, 28);
            GCREsocm.BorderStyle = BorderStyle.FixedSingle;
            GCREsocm.Dock = DockStyle.Fill;
            GCREsocm.ForeColor = Color.FromArgb(213, 213, 213);
            GCREsocm.Location = new Point(235, 63);
            GCREsocm.Name = "GCREsocm";
            GCREsocm.Size = new Size(161, 23);
            GCREsocm.TabIndex = 25;
            // 
            // GCREsoct
            // 
            GCREsoct.BackColor = Color.FromArgb(26, 26, 28);
            GCREsoct.BorderStyle = BorderStyle.FixedSingle;
            GCREsoct.Dock = DockStyle.Fill;
            GCREsoct.ForeColor = Color.FromArgb(213, 213, 213);
            GCREsoct.Location = new Point(402, 63);
            GCREsoct.Name = "GCREsoct";
            GCREsoct.Size = new Size(161, 23);
            GCREsoct.TabIndex = 24;
            // 
            // GCREsocml
            // 
            GCREsocml.AutoSize = true;
            GCREsocml.Location = new Point(235, 45);
            GCREsocml.Name = "GCREsocml";
            GCREsocml.Size = new Size(41, 15);
            GCREsocml.TabIndex = 72;
            GCREsocml.Text = "Model";
            // 
            // GCREsoctl
            // 
            GCREsoctl.AutoSize = true;
            GCREsoctl.Location = new Point(402, 45);
            GCREsoctl.Name = "GCREsoctl";
            GCREsoctl.Size = new Size(45, 15);
            GCREsoctl.TabIndex = 73;
            GCREsoctl.Text = "Texture";
            // 
            // GCREsoccb
            // 
            GCREsoccb.Dock = DockStyle.Fill;
            GCREsoccb.DrawMode = DrawMode.OwnerDrawVariable;
            GCREsoccb.FormattingEnabled = true;
            GCREsoccb.Items.AddRange(new object[] { "Head", "Hair", "Ponytail", "Moustache", "Beard", "Eye", "Body", "Hand", "Feet", "Back", "Shoulder", "Vanity" });
            GCREsoccb.Location = new Point(235, 18);
            GCREsoccb.Name = "GCREsoccb";
            GCREsoccb.Size = new Size(161, 24);
            GCREsoccb.TabIndex = 74;
            // 
            // GCREeil
            // 
            GCREeil.AutoSize = true;
            GCREeil.Location = new Point(235, 90);
            GCREeil.Name = "GCREeil";
            GCREeil.Size = new Size(89, 15);
            GCREeil.TabIndex = 75;
            GCREeil.Text = "Equipped Items";
            // 
            // GCREsoctel
            // 
            GCREsoctel.AutoSize = true;
            GCREsoctel.Dock = DockStyle.Left;
            GCREsoctel.Location = new Point(569, 60);
            GCREsoctel.Name = "GCREsoctel";
            GCREsoctel.Size = new Size(29, 30);
            GCREsoctel.TabIndex = 76;
            GCREsoctel.Text = ".dds";
            // 
            // GCREpel
            // 
            GCREpel.AutoSize = true;
            GCREpel.Dock = DockStyle.Left;
            GCREpel.Location = new Point(200, 195);
            GCREpel.Name = "GCREpel";
            GCREpel.Size = new Size(29, 29);
            GCREpel.TabIndex = 77;
            GCREpel.Text = ".dds";
            // 
            // GCREage
            // 
            GCREage.Dock = DockStyle.Left;
            GCREage.Location = new Point(402, 18);
            GCREage.Name = "GCREage";
            GCREage.Size = new Size(80, 23);
            GCREage.TabIndex = 78;
            // 
            // GCREagel
            // 
            GCREagel.AutoSize = true;
            GCREagel.Location = new Point(402, 0);
            GCREagel.Name = "GCREagel";
            GCREagel.Size = new Size(28, 15);
            GCREagel.TabIndex = 79;
            GCREagel.Text = "Age";
            // 
            // GWAMgb
            // 
            GWAMgb.Controls.Add(GWAMm);
            GWAMgb.Controls.Add(GWAMcb);
            GWAMgb.Controls.Add(GWAMp);
            GWAMgb.Controls.Add(GWAMtlp);
            GWAMgb.Location = new Point(618, 206);
            GWAMgb.Name = "GWAMgb";
            GWAMgb.Padding = new Padding(2, 11, 2, 2);
            GWAMgb.Size = new Size(300, 252);
            GWAMgb.TabIndex = 2;
            GWAMgb.TabStop = false;
            GWAMgb.Text = "GWAM";
            // 
            // GWAMm
            // 
            GWAMm.BorderColour = Color.Empty;
            GWAMm.CustomColour = false;
            GWAMm.FlatBottom = false;
            GWAMm.FlatTop = false;
            GWAMm.Location = new Point(270, 0);
            GWAMm.Name = "GWAMm";
            GWAMm.Padding = new Padding(5);
            GWAMm.Size = new Size(24, 24);
            GWAMm.TabIndex = 42;
            GWAMm.Text = "-";
            // 
            // GWAMcb
            // 
            GWAMcb.DrawMode = DrawMode.OwnerDrawVariable;
            GWAMcb.FormattingEnabled = true;
            GWAMcb.Location = new Point(58, 0);
            GWAMcb.Name = "GWAMcb";
            GWAMcb.Size = new Size(179, 24);
            GWAMcb.TabIndex = 43;
            // 
            // GWAMp
            // 
            GWAMp.BorderColour = Color.Empty;
            GWAMp.CustomColour = false;
            GWAMp.FlatBottom = false;
            GWAMp.FlatTop = false;
            GWAMp.Location = new Point(243, 0);
            GWAMp.Name = "GWAMp";
            GWAMp.Padding = new Padding(5);
            GWAMp.Size = new Size(24, 24);
            GWAMp.TabIndex = 41;
            GWAMp.Text = "+";
            // 
            // GWAMtlp
            // 
            GWAMtlp.ColumnCount = 4;
            GWAMtlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333f));
            GWAMtlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333f));
            GWAMtlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 21.05263f));
            GWAMtlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.2807f));
            GWAMtlp.Controls.Add(GWAManil, 0, 0);
            GWAMtlp.Controls.Add(GWAMani, 0, 1);
            GWAMtlp.Controls.Add(GWAMdtl, 1, 0);
            GWAMtlp.Controls.Add(GWAMdt, 1, 1);
            GWAMtlp.Controls.Add(GWAMapl, 0, 6);
            GWAMtlp.Controls.Add(GWAMap, 0, 7);
            GWAMtlp.Controls.Add(GWAMsfl, 2, 0);
            GWAMtlp.Controls.Add(GWAMsf, 2, 1);
            GWAMtlp.Controls.Add(GWAMrl, 0, 2);
            GWAMtlp.Controls.Add(GWAMr, 0, 3);
            GWAMtlp.Controls.Add(GWAMminl, 1, 2);
            GWAMtlp.Controls.Add(GWAMmin, 1, 3);
            GWAMtlp.Controls.Add(GWAMmaxl, 2, 2);
            GWAMtlp.Controls.Add(GWAMmax, 2, 3);
            GWAMtlp.Controls.Add(GWAManl, 0, 8);
            GWAMtlp.Controls.Add(GWAManSR, 0, 9);
            GWAMtlp.Controls.Add(GWAMef, 0, 11);
            GWAMtlp.Controls.Add(GWAMefl, 0, 10);
            GWAMtlp.Controls.Add(GWAMan, 1, 9);
            GWAMtlp.Controls.Add(GWAMefel, 3, 11);
            GWAMtlp.Dock = DockStyle.Fill;
            GWAMtlp.Location = new Point(2, 27);
            GWAMtlp.Name = "GWAMtlp";
            GWAMtlp.Padding = new Padding(1);
            GWAMtlp.RowCount = 10;
            GWAMtlp.RowStyles.Add(new RowStyle());
            GWAMtlp.RowStyles.Add(new RowStyle());
            GWAMtlp.RowStyles.Add(new RowStyle());
            GWAMtlp.RowStyles.Add(new RowStyle());
            GWAMtlp.RowStyles.Add(new RowStyle());
            GWAMtlp.RowStyles.Add(new RowStyle());
            GWAMtlp.RowStyles.Add(new RowStyle());
            GWAMtlp.RowStyles.Add(new RowStyle());
            GWAMtlp.RowStyles.Add(new RowStyle());
            GWAMtlp.RowStyles.Add(new RowStyle());
            GWAMtlp.RowStyles.Add(new RowStyle());
            GWAMtlp.RowStyles.Add(new RowStyle());
            GWAMtlp.Size = new Size(296, 223);
            GWAMtlp.TabIndex = 44;
            // 
            // GWAManil
            // 
            GWAManil.AutoSize = true;
            GWAManil.Location = new Point(4, 1);
            GWAManil.Name = "GWAManil";
            GWAManil.Size = new Size(63, 15);
            GWAManil.TabIndex = 0;
            GWAManil.Text = "Animation";
            // 
            // GWAMani
            // 
            GWAMani.Dock = DockStyle.Fill;
            GWAMani.Location = new Point(4, 19);
            GWAMani.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            GWAMani.Name = "GWAMani";
            GWAMani.Size = new Size(91, 23);
            GWAMani.TabIndex = 49;
            // 
            // GWAMdtl
            // 
            GWAMdtl.AutoSize = true;
            GWAMdtl.Location = new Point(101, 1);
            GWAMdtl.Name = "GWAMdtl";
            GWAMdtl.Size = new Size(78, 15);
            GWAMdtl.TabIndex = 50;
            GWAMdtl.Text = "Damage Type";
            // 
            // GWAMdt
            // 
            GWAMdt.DrawMode = DrawMode.OwnerDrawVariable;
            GWAMdt.FormattingEnabled = true;
            GWAMdt.Items.AddRange(new object[] { "Ballistic", "Bio", "Electric", "EMP", "General", "Heat" });
            GWAMdt.Location = new Point(101, 19);
            GWAMdt.Name = "GWAMdt";
            GWAMdt.Size = new Size(91, 24);
            GWAMdt.TabIndex = 51;
            // 
            // GWAMapl
            // 
            GWAMapl.AutoSize = true;
            GWAMtlp.SetColumnSpan(GWAMapl, 4);
            GWAMapl.Location = new Point(4, 90);
            GWAMapl.Name = "GWAMapl";
            GWAMapl.Size = new Size(78, 15);
            GWAMapl.TabIndex = 60;
            GWAMapl.Text = "Action Points";
            // 
            // GWAMap
            // 
            GWAMap.Dock = DockStyle.Fill;
            GWAMap.Location = new Point(4, 108);
            GWAMap.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            GWAMap.Name = "GWAMap";
            GWAMap.Size = new Size(91, 23);
            GWAMap.TabIndex = 61;
            // 
            // GWAMsfl
            // 
            GWAMsfl.AutoSize = true;
            GWAMtlp.SetColumnSpan(GWAMsfl, 2);
            GWAMsfl.Location = new Point(198, 1);
            GWAMsfl.Name = "GWAMsfl";
            GWAMsfl.Size = new Size(65, 15);
            GWAMsfl.TabIndex = 52;
            GWAMsfl.Text = "Shots Fired";
            // 
            // GWAMrl
            // 
            GWAMrl.AutoSize = true;
            GWAMrl.Location = new Point(4, 46);
            GWAMrl.Name = "GWAMrl";
            GWAMrl.Size = new Size(40, 15);
            GWAMrl.TabIndex = 53;
            GWAMrl.Text = "Range";
            // 
            // GWAMr
            // 
            GWAMr.Dock = DockStyle.Fill;
            GWAMr.Location = new Point(4, 64);
            GWAMr.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            GWAMr.Name = "GWAMr";
            GWAMr.Size = new Size(91, 23);
            GWAMr.TabIndex = 55;
            // 
            // GWAMminl
            // 
            GWAMminl.AutoSize = true;
            GWAMminl.Location = new Point(101, 46);
            GWAMminl.Name = "GWAMminl";
            GWAMminl.Size = new Size(57, 15);
            GWAMminl.TabIndex = 56;
            GWAMminl.Text = "Min Dmg";
            // 
            // GWAMmin
            // 
            GWAMmin.Dock = DockStyle.Fill;
            GWAMmin.Location = new Point(101, 64);
            GWAMmin.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            GWAMmin.Name = "GWAMmin";
            GWAMmin.Size = new Size(91, 23);
            GWAMmin.TabIndex = 58;
            // 
            // GWAMmaxl
            // 
            GWAMmaxl.AutoSize = true;
            GWAMtlp.SetColumnSpan(GWAMmaxl, 2);
            GWAMmaxl.Location = new Point(198, 46);
            GWAMmaxl.Name = "GWAMmaxl";
            GWAMmaxl.Size = new Size(59, 15);
            GWAMmaxl.TabIndex = 57;
            GWAMmaxl.Text = "Max Dmg";
            // 
            // GWAMmax
            // 
            GWAMtlp.SetColumnSpan(GWAMmax, 2);
            GWAMmax.Dock = DockStyle.Fill;
            GWAMmax.Location = new Point(198, 64);
            GWAMmax.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            GWAMmax.Name = "GWAMmax";
            GWAMmax.Size = new Size(94, 23);
            GWAMmax.TabIndex = 59;
            // 
            // GWAManl
            // 
            GWAManl.AutoSize = true;
            GWAMtlp.SetColumnSpan(GWAManl, 4);
            GWAManl.Location = new Point(4, 134);
            GWAManl.Name = "GWAManl";
            GWAManl.Size = new Size(76, 15);
            GWAManl.TabIndex = 64;
            GWAManl.Text = "Attack Name";
            // 
            // GWAManSR
            // 
            GWAManSR.Dock = DockStyle.Fill;
            GWAManSR.Location = new Point(4, 152);
            GWAManSR.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            GWAManSR.Name = "GWAManSR";
            GWAManSR.Size = new Size(91, 23);
            GWAManSR.TabIndex = 62;
            // 
            // GWAMef
            // 
            GWAMef.BackColor = Color.FromArgb(26, 26, 28);
            GWAMef.BorderStyle = BorderStyle.FixedSingle;
            GWAMtlp.SetColumnSpan(GWAMef, 3);
            GWAMef.Dock = DockStyle.Fill;
            GWAMef.ForeColor = Color.FromArgb(213, 213, 213);
            GWAMef.Location = new Point(4, 196);
            GWAMef.Name = "GWAMef";
            GWAMef.Size = new Size(249, 23);
            GWAMef.TabIndex = 66;
            // 
            // GWAMefl
            // 
            GWAMefl.AutoSize = true;
            GWAMefl.Dock = DockStyle.Fill;
            GWAMefl.Location = new Point(4, 178);
            GWAMefl.Name = "GWAMefl";
            GWAMefl.Size = new Size(91, 15);
            GWAMefl.TabIndex = 65;
            GWAMefl.Text = "Effect";
            // 
            // GWAMan
            // 
            GWAMan.BackColor = Color.FromArgb(26, 26, 28);
            GWAMan.BorderStyle = BorderStyle.FixedSingle;
            GWAMtlp.SetColumnSpan(GWAMan, 3);
            GWAMan.Dock = DockStyle.Fill;
            GWAMan.ForeColor = Color.FromArgb(213, 213, 213);
            GWAMan.Location = new Point(101, 152);
            GWAMan.Name = "GWAMan";
            GWAMan.Size = new Size(191, 23);
            GWAMan.TabIndex = 63;
            // 
            // GWAMefel
            // 
            GWAMefel.AutoSize = true;
            GWAMefel.Dock = DockStyle.Fill;
            GWAMefel.Location = new Point(259, 193);
            GWAMefel.Name = "GWAMefel";
            GWAMefel.Size = new Size(33, 29);
            GWAMefel.TabIndex = 6;
            GWAMefel.Text = ".veg";
            // 
            // GENTgb
            // 
            GENTgb.Controls.Add(GENTu);
            GENTgb.Controls.Add(GENTn);
            GENTgb.Controls.Add(GENTl);
            GENTgb.Controls.Add(GENTh);
            GENTgb.Controls.Add(GENTihpl);
            GENTgb.Controls.Add(GENTmhpl);
            GENTgb.Controls.Add(GENTihp);
            GENTgb.Controls.Add(GENTmhp);
            GENTgb.Controls.Add(GENTuSR);
            GENTgb.Controls.Add(GENTnSR);
            GENTgb.Controls.Add(GENTlSR);
            GENTgb.Controls.Add(GENThSR);
            GENTgb.Location = new Point(618, 66);
            GENTgb.Name = "GENTgb";
            GENTgb.Size = new Size(300, 168);
            GENTgb.TabIndex = 1;
            GENTgb.TabStop = false;
            GENTgb.Text = "GENT";
            // 
            // GENTu
            // 
            GENTu.BackColor = Color.FromArgb(26, 26, 28);
            GENTu.BorderStyle = BorderStyle.FixedSingle;
            GENTu.ForeColor = Color.FromArgb(213, 213, 213);
            GENTu.Location = new Point(56, 110);
            GENTu.Name = "GENTu";
            GENTu.Size = new Size(238, 23);
            GENTu.TabIndex = 58;
            // 
            // GENTn
            // 
            GENTn.BackColor = Color.FromArgb(26, 26, 28);
            GENTn.BorderStyle = BorderStyle.FixedSingle;
            GENTn.ForeColor = Color.FromArgb(213, 213, 213);
            GENTn.Location = new Point(56, 80);
            GENTn.Name = "GENTn";
            GENTn.Size = new Size(238, 23);
            GENTn.TabIndex = 57;
            // 
            // GENTl
            // 
            GENTl.BackColor = Color.FromArgb(26, 26, 28);
            GENTl.BorderStyle = BorderStyle.FixedSingle;
            GENTl.ForeColor = Color.FromArgb(213, 213, 213);
            GENTl.Location = new Point(56, 51);
            GENTl.Name = "GENTl";
            GENTl.Size = new Size(238, 23);
            GENTl.TabIndex = 56;
            // 
            // GENTh
            // 
            GENTh.BackColor = Color.FromArgb(26, 26, 28);
            GENTh.BorderStyle = BorderStyle.FixedSingle;
            GENTh.ForeColor = Color.FromArgb(213, 213, 213);
            GENTh.Location = new Point(56, 22);
            GENTh.Name = "GENTh";
            GENTh.Size = new Size(238, 23);
            GENTh.TabIndex = 37;
            // 
            // GENTihpl
            // 
            GENTihpl.AutoSize = true;
            GENTihpl.Location = new Point(150, 141);
            GENTihpl.Name = "GENTihpl";
            GENTihpl.Size = new Size(77, 15);
            GENTihpl.TabIndex = 55;
            GENTihpl.Text = "Initial Health:";
            // 
            // GENTmhpl
            // 
            GENTmhpl.AutoSize = true;
            GENTmhpl.Location = new Point(6, 141);
            GENTmhpl.Name = "GENTmhpl";
            GENTmhpl.Size = new Size(71, 15);
            GENTmhpl.TabIndex = 54;
            GENTmhpl.Text = "Max Health:";
            // 
            // GENTihp
            // 
            GENTihp.Location = new Point(233, 139);
            GENTihp.Maximum = new decimal(new int[] { 2147483647, 0, 0, 0 });
            GENTihp.Name = "GENTihp";
            GENTihp.Size = new Size(61, 23);
            GENTihp.TabIndex = 53;
            // 
            // GENTmhp
            // 
            GENTmhp.Location = new Point(83, 139);
            GENTmhp.Maximum = new decimal(new int[] { 2147483647, 0, 0, 0 });
            GENTmhp.Name = "GENTmhp";
            GENTmhp.Size = new Size(61, 23);
            GENTmhp.TabIndex = 52;
            // 
            // GENTuSR
            // 
            GENTuSR.Location = new Point(6, 110);
            GENTuSR.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            GENTuSR.Name = "GENTuSR";
            GENTuSR.Size = new Size(44, 23);
            GENTuSR.TabIndex = 51;
            // 
            // GENTnSR
            // 
            GENTnSR.Location = new Point(6, 80);
            GENTnSR.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            GENTnSR.Name = "GENTnSR";
            GENTnSR.Size = new Size(44, 23);
            GENTnSR.TabIndex = 50;
            // 
            // GENTlSR
            // 
            GENTlSR.Location = new Point(6, 51);
            GENTlSR.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            GENTlSR.Name = "GENTlSR";
            GENTlSR.Size = new Size(44, 23);
            GENTlSR.TabIndex = 49;
            // 
            // GENThSR
            // 
            GENThSR.Location = new Point(6, 22);
            GENThSR.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            GENThSR.Name = "GENThSR";
            GENThSR.Size = new Size(44, 23);
            GENThSR.TabIndex = 48;
            // 
            // EEN2gb
            // 
            EEN2gb.Controls.Add(EEN2dsb);
            EEN2gb.Controls.Add(EEN2dgv);
            EEN2gb.Controls.Add(EEN2acttl);
            EEN2gb.Controls.Add(EEN2invtl);
            EEN2gb.Controls.Add(EEN2s5l);
            EEN2gb.Controls.Add(EEN2s4l);
            EEN2gb.Controls.Add(EEN2s3l);
            EEN2gb.Controls.Add(EEN2s5);
            EEN2gb.Controls.Add(EEN2s4);
            EEN2gb.Controls.Add(EEN2s3);
            EEN2gb.Controls.Add(EEN2s2);
            EEN2gb.Controls.Add(EEN2s1);
            EEN2gb.Controls.Add(EEN2sel);
            EEN2gb.Controls.Add(EEN2actt);
            EEN2gb.Controls.Add(EEN2invt);
            EEN2gb.Controls.Add(EEN2skl);
            EEN2gb.Location = new Point(6, 22);
            EEN2gb.Name = "EEN2gb";
            EEN2gb.Size = new Size(606, 256);
            EEN2gb.TabIndex = 0;
            EEN2gb.TabStop = false;
            EEN2gb.Text = "EEN2";
            // 
            // EEN2dsb
            // 
            EEN2dsb.Location = new Point(583, 48);
            EEN2dsb.Maximum = 500;
            EEN2dsb.Name = "EEN2dsb";
            EEN2dsb.Size = new Size(18, 202);
            EEN2dsb.TabIndex = 35;
            // 
            // EEN2acttl
            // 
            EEN2acttl.AutoSize = true;
            EEN2acttl.Location = new Point(271, 84);
            EEN2acttl.Name = "EEN2acttl";
            EEN2acttl.Size = new Size(29, 15);
            EEN2acttl.TabIndex = 33;
            EEN2acttl.Text = ".dds";
            // 
            // EEN2invtl
            // 
            EEN2invtl.AutoSize = true;
            EEN2invtl.Location = new Point(271, 55);
            EEN2invtl.Name = "EEN2invtl";
            EEN2invtl.Size = new Size(29, 15);
            EEN2invtl.TabIndex = 32;
            EEN2invtl.Text = ".dds";
            // 
            // EEN2s5l
            // 
            EEN2s5l.AutoSize = true;
            EEN2s5l.Location = new Point(271, 229);
            EEN2s5l.Name = "EEN2s5l";
            EEN2s5l.Size = new Size(29, 15);
            EEN2s5l.TabIndex = 31;
            EEN2s5l.Text = ".veg";
            // 
            // EEN2s4l
            // 
            EEN2s4l.AutoSize = true;
            EEN2s4l.Location = new Point(271, 200);
            EEN2s4l.Name = "EEN2s4l";
            EEN2s4l.Size = new Size(29, 15);
            EEN2s4l.TabIndex = 30;
            EEN2s4l.Text = ".dds";
            // 
            // EEN2s3l
            // 
            EEN2s3l.AutoSize = true;
            EEN2s3l.Location = new Point(267, 171);
            EEN2s3l.Name = "EEN2s3l";
            EEN2s3l.Size = new Size(33, 15);
            EEN2s3l.TabIndex = 29;
            EEN2s3l.Text = ".amx";
            // 
            // EEN2s5
            // 
            EEN2s5.BackColor = Color.FromArgb(26, 26, 28);
            EEN2s5.BorderStyle = BorderStyle.FixedSingle;
            EEN2s5.ForeColor = Color.FromArgb(213, 213, 213);
            EEN2s5.Location = new Point(6, 225);
            EEN2s5.Name = "EEN2s5";
            EEN2s5.Size = new Size(259, 23);
            EEN2s5.TabIndex = 28;
            // 
            // EEN2s4
            // 
            EEN2s4.BackColor = Color.FromArgb(26, 26, 28);
            EEN2s4.BorderStyle = BorderStyle.FixedSingle;
            EEN2s4.ForeColor = Color.FromArgb(213, 213, 213);
            EEN2s4.Location = new Point(6, 196);
            EEN2s4.Name = "EEN2s4";
            EEN2s4.Size = new Size(259, 23);
            EEN2s4.TabIndex = 27;
            // 
            // EEN2s3
            // 
            EEN2s3.BackColor = Color.FromArgb(26, 26, 28);
            EEN2s3.BorderStyle = BorderStyle.FixedSingle;
            EEN2s3.ForeColor = Color.FromArgb(213, 213, 213);
            EEN2s3.Location = new Point(6, 167);
            EEN2s3.Name = "EEN2s3";
            EEN2s3.Size = new Size(255, 23);
            EEN2s3.TabIndex = 26;
            // 
            // EEN2s2
            // 
            EEN2s2.BackColor = Color.FromArgb(26, 26, 28);
            EEN2s2.BorderStyle = BorderStyle.FixedSingle;
            EEN2s2.ForeColor = Color.FromArgb(213, 213, 213);
            EEN2s2.Location = new Point(6, 138);
            EEN2s2.Name = "EEN2s2";
            EEN2s2.Size = new Size(294, 23);
            EEN2s2.TabIndex = 25;
            // 
            // EEN2s1
            // 
            EEN2s1.BackColor = Color.FromArgb(26, 26, 28);
            EEN2s1.BorderStyle = BorderStyle.FixedSingle;
            EEN2s1.ForeColor = Color.FromArgb(213, 213, 213);
            EEN2s1.Location = new Point(6, 109);
            EEN2s1.Name = "EEN2s1";
            EEN2s1.Size = new Size(294, 23);
            EEN2s1.TabIndex = 24;
            // 
            // EEN2sel
            // 
            EEN2sel.AutoSize = true;
            EEN2sel.Location = new Point(306, 23);
            EEN2sel.Name = "EEN2sel";
            EEN2sel.Offset = 1;
            EEN2sel.Size = new Size(79, 19);
            EEN2sel.TabIndex = 3;
            EEN2sel.Text = "Selectable";
            // 
            // EEN2actt
            // 
            EEN2actt.BackColor = Color.FromArgb(26, 26, 28);
            EEN2actt.BorderStyle = BorderStyle.FixedSingle;
            EEN2actt.ForeColor = Color.FromArgb(213, 213, 213);
            EEN2actt.Location = new Point(6, 80);
            EEN2actt.Name = "EEN2actt";
            EEN2actt.Size = new Size(259, 23);
            EEN2actt.TabIndex = 2;
            // 
            // EEN2invt
            // 
            EEN2invt.BackColor = Color.FromArgb(26, 26, 28);
            EEN2invt.BorderStyle = BorderStyle.FixedSingle;
            EEN2invt.ForeColor = Color.FromArgb(213, 213, 213);
            EEN2invt.Location = new Point(6, 51);
            EEN2invt.Name = "EEN2invt";
            EEN2invt.Size = new Size(259, 23);
            EEN2invt.TabIndex = 1;
            // 
            // EEN2skl
            // 
            EEN2skl.BackColor = Color.FromArgb(26, 26, 28);
            EEN2skl.BorderStyle = BorderStyle.FixedSingle;
            EEN2skl.ForeColor = Color.FromArgb(213, 213, 213);
            EEN2skl.Location = new Point(6, 22);
            EEN2skl.Name = "EEN2skl";
            EEN2skl.Size = new Size(294, 23);
            EEN2skl.TabIndex = 0;
            // 
            // GCREtmr
            // 
            GCREtmr.Interval = 7;
            // 
            // EEN2tmr
            // 
            EEN2tmr.Interval = 7;
            // 
            // ITMgb
            // 
            ITMgb.Controls.Add(GITMgb);
            ITMgb.Controls.Add(EEN2gb);
            ITMgb.Controls.Add(GENTgb);
            ITMgb.Location = new Point(12, 27);
            ITMgb.Name = "ITMgb";
            ITMgb.Size = new Size(924, 395);
            ITMgb.TabIndex = 16;
            ITMgb.TabStop = false;
            ITMgb.Text = ".ITM Editor";
            // 
            // GITMgb
            // 
            GITMgb.Controls.Add(TableLayoutPanel1);
            GITMgb.Location = new Point(6, 284);
            GITMgb.Name = "GITMgb";
            GITMgb.Padding = new Padding(2, 0, 2, 2);
            GITMgb.Size = new Size(912, 105);
            GITMgb.TabIndex = 0;
            GITMgb.TabStop = false;
            GITMgb.Text = "GITM";
            // 
            // TableLayoutPanel1
            // 
            TableLayoutPanel1.ColumnCount = 10;
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            TableLayoutPanel1.Controls.Add(DarkLabel4, 0, 0);
            TableLayoutPanel1.Controls.Add(GITMeq, 1, 1);
            TableLayoutPanel1.Controls.Add(DarkLabel5, 2, 0);
            TableLayoutPanel1.Controls.Add(GITMslot, 2, 1);
            TableLayoutPanel1.Controls.Add(GITMml, 4, 1);
            TableLayoutPanel1.Controls.Add(GITMrl, 3, 1);
            TableLayoutPanel1.Controls.Add(DarkLabel6, 3, 0);
            TableLayoutPanel1.Controls.Add(DarkLabel7, 0, 2);
            TableLayoutPanel1.Controls.Add(GITMhhai, 0, 3);
            TableLayoutPanel1.Controls.Add(GITMhbea, 1, 3);
            TableLayoutPanel1.Controls.Add(GITMhvan, 5, 3);
            TableLayoutPanel1.Controls.Add(GITMheye, 3, 3);
            TableLayoutPanel1.Controls.Add(GITMhmus, 2, 3);
            TableLayoutPanel1.Controls.Add(GITMhpon, 4, 3);
            TableLayoutPanel1.Controls.Add(GITMtype, 0, 1);
            TableLayoutPanel1.Controls.Add(DarkLabel8, 6, 0);
            TableLayoutPanel1.Controls.Add(GITMsoccb, 6, 1);
            TableLayoutPanel1.Controls.Add(DarkLabel9, 6, 2);
            TableLayoutPanel1.Controls.Add(DarkLabel10, 8, 2);
            TableLayoutPanel1.Controls.Add(GITMsoct, 8, 3);
            TableLayoutPanel1.Controls.Add(GITMsocm, 6, 3);
            TableLayoutPanel1.Dock = DockStyle.Fill;
            TableLayoutPanel1.Location = new Point(2, 16);
            TableLayoutPanel1.Name = "TableLayoutPanel1";
            TableLayoutPanel1.RowCount = 4;
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.Size = new Size(908, 87);
            TableLayoutPanel1.TabIndex = 0;
            // 
            // DarkLabel4
            // 
            DarkLabel4.AutoSize = true;
            DarkLabel4.Dock = DockStyle.Fill;
            DarkLabel4.Location = new Point(3, 0);
            DarkLabel4.Name = "DarkLabel4";
            DarkLabel4.Size = new Size(60, 15);
            DarkLabel4.TabIndex = 0;
            DarkLabel4.Text = "Type";
            // 
            // GITMeq
            // 
            GITMeq.AutoSize = true;
            GITMeq.Dock = DockStyle.Fill;
            GITMeq.Location = new Point(69, 18);
            GITMeq.Name = "GITMeq";
            GITMeq.Offset = 1;
            GITMeq.Size = new Size(80, 24);
            GITMeq.TabIndex = 3;
            GITMeq.Text = "Can Equip";
            // 
            // DarkLabel5
            // 
            DarkLabel5.AutoSize = true;
            DarkLabel5.Dock = DockStyle.Fill;
            DarkLabel5.Location = new Point(155, 0);
            DarkLabel5.Name = "DarkLabel5";
            DarkLabel5.Size = new Size(85, 15);
            DarkLabel5.TabIndex = 4;
            DarkLabel5.Text = "Slot";
            // 
            // GITMslot
            // 
            GITMslot.Dock = DockStyle.Fill;
            GITMslot.DrawMode = DrawMode.OwnerDrawVariable;
            GITMslot.FormattingEnabled = true;
            GITMslot.Items.AddRange(new object[] { "Body", "Head", "Hands" });
            GITMslot.Location = new Point(155, 18);
            GITMslot.Name = "GITMslot";
            GITMslot.Size = new Size(85, 24);
            GITMslot.TabIndex = 5;
            // 
            // GITMml
            // 
            GITMml.AutoSize = true;
            GITMml.Dock = DockStyle.Fill;
            GITMml.Location = new Point(299, 18);
            GITMml.Name = "GITMml";
            GITMml.Offset = 1;
            GITMml.Size = new Size(69, 24);
            GITMml.TabIndex = 6;
            GITMml.Text = "Melee";
            // 
            // GITMrl
            // 
            GITMrl.BackColor = Color.FromArgb(26, 26, 28);
            GITMrl.BorderStyle = BorderStyle.FixedSingle;
            GITMrl.Dock = DockStyle.Fill;
            GITMrl.ForeColor = Color.FromArgb(213, 213, 213);
            GITMrl.Location = new Point(246, 18);
            GITMrl.Name = "GITMrl";
            GITMrl.Size = new Size(47, 23);
            GITMrl.TabIndex = 7;
            // 
            // DarkLabel6
            // 
            DarkLabel6.AutoSize = true;
            DarkLabel6.Dock = DockStyle.Fill;
            DarkLabel6.Location = new Point(246, 0);
            DarkLabel6.Name = "DarkLabel6";
            DarkLabel6.Size = new Size(47, 15);
            DarkLabel6.TabIndex = 8;
            DarkLabel6.Text = "Reload";
            // 
            // DarkLabel7
            // 
            DarkLabel7.AutoSize = true;
            TableLayoutPanel1.SetColumnSpan(DarkLabel7, 3);
            DarkLabel7.Dock = DockStyle.Fill;
            DarkLabel7.Location = new Point(3, 45);
            DarkLabel7.Name = "DarkLabel7";
            DarkLabel7.Size = new Size(237, 15);
            DarkLabel7.TabIndex = 9;
            DarkLabel7.Text = "Hide Socket:";
            // 
            // GITMhhai
            // 
            GITMhhai.AutoSize = true;
            GITMhhai.Dock = DockStyle.Fill;
            GITMhhai.Location = new Point(3, 63);
            GITMhhai.Name = "GITMhhai";
            GITMhhai.Offset = 1;
            GITMhhai.Size = new Size(60, 23);
            GITMhhai.TabIndex = 10;
            GITMhhai.Text = "Hair";
            // 
            // GITMhbea
            // 
            GITMhbea.AutoSize = true;
            GITMhbea.Dock = DockStyle.Fill;
            GITMhbea.Location = new Point(69, 63);
            GITMhbea.Name = "GITMhbea";
            GITMhbea.Offset = 1;
            GITMhbea.Size = new Size(80, 23);
            GITMhbea.TabIndex = 11;
            GITMhbea.Text = "Beard";
            // 
            // GITMhvan
            // 
            GITMhvan.AutoSize = true;
            GITMhvan.Dock = DockStyle.Fill;
            GITMhvan.Location = new Point(374, 63);
            GITMhvan.Name = "GITMhvan";
            GITMhvan.Offset = 1;
            GITMhvan.Size = new Size(58, 23);
            GITMhvan.TabIndex = 15;
            GITMhvan.Text = "Vanity";
            // 
            // GITMheye
            // 
            GITMheye.AutoSize = true;
            GITMheye.Dock = DockStyle.Fill;
            GITMheye.Location = new Point(246, 63);
            GITMheye.Name = "GITMheye";
            GITMheye.Offset = 1;
            GITMheye.Size = new Size(47, 23);
            GITMheye.TabIndex = 13;
            GITMheye.Text = "Eye";
            // 
            // GITMhmus
            // 
            GITMhmus.AutoSize = true;
            GITMhmus.Dock = DockStyle.Fill;
            GITMhmus.Location = new Point(155, 63);
            GITMhmus.Name = "GITMhmus";
            GITMhmus.Offset = 1;
            GITMhmus.Size = new Size(85, 23);
            GITMhmus.TabIndex = 12;
            GITMhmus.Text = "Moustache";
            // 
            // GITMhpon
            // 
            GITMhpon.AutoSize = true;
            GITMhpon.Dock = DockStyle.Fill;
            GITMhpon.Location = new Point(299, 63);
            GITMhpon.Name = "GITMhpon";
            GITMhpon.Offset = 1;
            GITMhpon.Size = new Size(69, 23);
            GITMhpon.TabIndex = 14;
            GITMhpon.Text = "Ponytail";
            // 
            // GITMtype
            // 
            GITMtype.Dock = DockStyle.Fill;
            GITMtype.DrawMode = DrawMode.OwnerDrawVariable;
            GITMtype.FormattingEnabled = true;
            GITMtype.Items.AddRange(new object[] { "ITM", "AMO", "ARM", "???", "WEA" });
            GITMtype.Location = new Point(3, 18);
            GITMtype.Name = "GITMtype";
            GITMtype.Size = new Size(60, 24);
            GITMtype.TabIndex = 22;
            // 
            // DarkLabel8
            // 
            DarkLabel8.AutoSize = true;
            DarkLabel8.Dock = DockStyle.Fill;
            DarkLabel8.Location = new Point(438, 0);
            DarkLabel8.Name = "DarkLabel8";
            DarkLabel8.Size = new Size(112, 15);
            DarkLabel8.TabIndex = 16;
            DarkLabel8.Text = "Socket:";
            // 
            // GITMsoccb
            // 
            TableLayoutPanel1.SetColumnSpan(GITMsoccb, 2);
            GITMsoccb.Dock = DockStyle.Fill;
            GITMsoccb.DrawMode = DrawMode.OwnerDrawVariable;
            GITMsoccb.FormattingEnabled = true;
            GITMsoccb.Items.AddRange(new object[] { "Head", "Eye", "Body", "Back", "Hands", "Feet", "Shoes", "Vanity", "In-Hand" });
            GITMsoccb.Location = new Point(438, 18);
            GITMsoccb.Name = "GITMsoccb";
            GITMsoccb.Size = new Size(230, 24);
            GITMsoccb.TabIndex = 21;
            // 
            // DarkLabel9
            // 
            DarkLabel9.AutoSize = true;
            TableLayoutPanel1.SetColumnSpan(DarkLabel9, 2);
            DarkLabel9.Dock = DockStyle.Fill;
            DarkLabel9.Location = new Point(438, 45);
            DarkLabel9.Name = "DarkLabel9";
            DarkLabel9.Size = new Size(230, 15);
            DarkLabel9.TabIndex = 17;
            DarkLabel9.Text = "Model";
            // 
            // DarkLabel10
            // 
            DarkLabel10.AutoSize = true;
            TableLayoutPanel1.SetColumnSpan(DarkLabel10, 2);
            DarkLabel10.Dock = DockStyle.Fill;
            DarkLabel10.Location = new Point(674, 45);
            DarkLabel10.Name = "DarkLabel10";
            DarkLabel10.Size = new Size(231, 15);
            DarkLabel10.TabIndex = 18;
            DarkLabel10.Text = "Texture";
            // 
            // GITMsoct
            // 
            GITMsoct.BackColor = Color.FromArgb(26, 26, 28);
            GITMsoct.BorderStyle = BorderStyle.FixedSingle;
            TableLayoutPanel1.SetColumnSpan(GITMsoct, 2);
            GITMsoct.Dock = DockStyle.Fill;
            GITMsoct.ForeColor = Color.FromArgb(213, 213, 213);
            GITMsoct.Location = new Point(674, 63);
            GITMsoct.Name = "GITMsoct";
            GITMsoct.Size = new Size(231, 23);
            GITMsoct.TabIndex = 20;
            // 
            // GITMsocm
            // 
            GITMsocm.BackColor = Color.FromArgb(26, 26, 28);
            GITMsocm.BorderStyle = BorderStyle.FixedSingle;
            TableLayoutPanel1.SetColumnSpan(GITMsocm, 2);
            GITMsocm.Dock = DockStyle.Fill;
            GITMsocm.ForeColor = Color.FromArgb(213, 213, 213);
            GITMsocm.Location = new Point(438, 63);
            GITMsocm.Name = "GITMsocm";
            GITMsocm.Size = new Size(230, 23);
            GITMsocm.TabIndex = 19;
            // 
            // Editor
            // 
            AutoScaleDimensions = new SizeF(7f, 15f);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(624, 441);
            Controls.Add(ITMgb);
            Controls.Add(CRTgb);
            Controls.Add(Mapgb);
            Controls.Add(DarkMainMenuStrip);
            CornerStyle = CornerPreference.Default;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Editor";
            Text = "Van Buren Editor";
            TransparencyKey = Color.FromArgb(31, 31, 32);
            Mapgb.ResumeLayout(false);
            _2MWTgb.ResumeLayout(false);
            _2MWTgb.PerformLayout();
            Triggergb.ResumeLayout(false);
            Triggergb.PerformLayout();
            ((ISupportInitialize)Triggernud).EndInit();
            EPTHGB.ResumeLayout(false);
            EPTHGB.PerformLayout();
            ((ISupportInitialize)EPTHnud).EndInit();
            EMSDgb.ResumeLayout(false);
            EMSDgb.PerformLayout();
            EME2gb.ResumeLayout(false);
            EME2gb.PerformLayout();
            ((ISupportInitialize)EME2dgv).EndInit();
            EMEFgb.ResumeLayout(false);
            EMEFgb.PerformLayout();
            ECAMgb.ResumeLayout(false);
            ECAMgb.PerformLayout();
            EMAPgb.ResumeLayout(false);
            EMAPgb.PerformLayout();
            EMEPgb.ResumeLayout(false);
            EMEPgb.PerformLayout();
            ((ISupportInitialize)EMEPnud).EndInit();
            DarkMainMenuStrip.ResumeLayout(false);
            DarkMainMenuStrip.PerformLayout();
            ((ISupportInitialize)GWAMsf).EndInit();
            ((ISupportInitialize)EEN2dgv).EndInit();
            CRTgb.ResumeLayout(false);
            GCHRgb.ResumeLayout(false);
            GCHRgb.PerformLayout();
            GCREgb.ResumeLayout(false);
            GCREtlp.ResumeLayout(false);
            GCREtlp.PerformLayout();
            ((ISupportInitialize)GCREdgv).EndInit();
            ((ISupportInitialize)GCREskv).EndInit();
            ((ISupportInitialize)GCREspv).EndInit();
            ((ISupportInitialize)GCREage).EndInit();
            GWAMgb.ResumeLayout(false);
            GWAMtlp.ResumeLayout(false);
            GWAMtlp.PerformLayout();
            ((ISupportInitialize)GWAMani).EndInit();
            ((ISupportInitialize)GWAMap).EndInit();
            ((ISupportInitialize)GWAMr).EndInit();
            ((ISupportInitialize)GWAMmin).EndInit();
            ((ISupportInitialize)GWAMmax).EndInit();
            ((ISupportInitialize)GWAManSR).EndInit();
            GENTgb.ResumeLayout(false);
            GENTgb.PerformLayout();
            ((ISupportInitialize)GENTihp).EndInit();
            ((ISupportInitialize)GENTmhp).EndInit();
            ((ISupportInitialize)GENTuSR).EndInit();
            ((ISupportInitialize)GENTnSR).EndInit();
            ((ISupportInitialize)GENTlSR).EndInit();
            ((ISupportInitialize)GENThSR).EndInit();
            EEN2gb.ResumeLayout(false);
            EEN2gb.PerformLayout();
            ITMgb.ResumeLayout(false);
            GITMgb.ResumeLayout(false);
            TableLayoutPanel1.ResumeLayout(false);
            TableLayoutPanel1.PerformLayout();
            Load += new EventHandler(InitialSetup);
            ResumeLayout(false);
            PerformLayout();

        }
        internal DarkTextBox EMAPs3;
        internal DarkTextBox EMAPs2;
        internal DarkTextBox EMAPs1;
        internal DarkButton EMAPslb;
        internal DarkCheckBox EMAPilcb;
        internal DarkGroupBox Mapgb;
        internal DarkGroupBox EMEPgb;
        internal DarkLabel EMEPcl;
        internal DarkLabel EMEPepl;
        internal DarkTextBox EMEPz;
        internal DarkTextBox EMEPy;
        internal DarkNumericUpDown EMEPnud;
        internal DarkTextBox EMEPx;
        internal DarkGroupBox ECAMgb;
        internal DarkTextBox ECAMz;
        internal DarkTextBox ECAMy;
        internal DarkTextBox ECAMx;
        internal DarkTextBox EME2n;
        internal DarkTextBox EME2x;
        internal DarkTextBox EME2y;
        internal DarkTextBox EME2z;
        internal DataGridView EME2dgv;
        internal DarkTextBox EME2s5;
        internal DarkTextBox EME2s4;
        internal DarkTextBox EME2s3;
        internal DarkTextBox EME2s2;
        internal DarkTextBox EME2s1;
        internal DarkScrollBar EME2dsb;
        internal Timer EME2tmr;
        internal DarkLabel EMAPs3l;
        internal DarkLabel EMAPs2l;
        internal DarkLabel EMAPs1l;
        internal DarkLabel EME2s5l;
        internal DarkLabel EME2s4l;
        internal DarkLabel EME2s3l;
        internal DarkComboBox EME2cb;
        internal DarkTextBox EME2r;
        internal DarkTextBox EMEPr;
        internal DarkComboBox EMEPcb;
        internal DarkTextBox ECAMr;
        internal DarkGroupBox EPTHGB;
        internal DarkTextBox EPTHr;
        internal DarkTextBox EPTHn;
        internal DarkComboBox EPTHcb;
        internal DarkTextBox EPTHx;
        internal DarkLabel EPTHxl;
        internal DarkTextBox EPTHy;
        internal DarkTextBox EPTHz;
        internal DarkGroupBox EMSDgb;
        internal DarkComboBox EMSDcb;
        internal DarkLabel EMSDs2l;
        internal DarkTextBox EMSDs1;
        internal DarkTextBox EMSDs2;
        internal DarkLabel EMSDcl;
        internal DarkTextBox EMSDz;
        internal DarkTextBox EMSDy;
        internal DarkTextBox EMSDx;
        internal DarkGroupBox EMEFgb;
        internal DarkComboBox EMEFcb;
        internal DarkLabel EMEFs2l;
        internal DarkTextBox EMEFs1;
        internal DarkTextBox EMEFs2;
        internal DarkLabel EMEFxl;
        internal DarkTextBox EMEFz;
        internal DarkTextBox EMEFy;
        internal DarkTextBox EMEFx;
        internal DarkTextBox EMEFr;
        internal DarkGroupBox Triggergb;
        internal DarkComboBox Triggertcb;
        internal DarkTextBox Triggern;
        internal DarkComboBox Triggercb;
        internal DarkTextBox Triggerx;
        internal DarkLabel Triggerxl;
        internal DarkTextBox Triggery;
        internal DarkTextBox Triggerz;
        internal DarkGroupBox EME2gb;
        internal DarkLabel EME2cl;
        internal DarkLabel ECAMcl;
        internal DarkGroupBox EMAPgb;
        internal DarkButton EME2m;
        internal DarkButton EME2p;
        internal DarkButton EMEPm;
        internal DarkButton EMEPp;
        internal DarkButton EMEFm;
        internal DarkButton EMEFp;
        internal DarkButton EMSDm;
        internal DarkButton EMSDp;
        internal DarkButton Triggerm;
        internal DarkButton Triggerp;
        internal DarkButton EPTHm;
        internal DarkButton EPTHp;
        internal DarkButton Triggerpm;
        internal DarkNumericUpDown Triggernud;
        internal DarkButton EPTHpm;
        internal DarkNumericUpDown EPTHnud;
        internal DarkMenuStrip DarkMainMenuStrip;
        internal ToolStripMenuItem FileToolStripMenuItem;
        internal ToolStripMenuItem NewToolStripMenuItem;
        internal ToolStripMenuItem OpenToolStripMenuItem;
        internal ToolStripMenuItem SaveToolStripMenuItem;
        internal ToolStripMenuItem ToolsToolStripMenuItem;
        internal ToolStripMenuItem StfToolStripMenuItem;
        internal ToolStripMenuItem TxtTostfToolStripMenuItem;
        internal ToolStripMenuItem AmoToolStripMenuItem;
        internal ToolStripMenuItem ArmToolStripMenuItem;
        internal ToolStripMenuItem ConToolStripMenuItem;
        internal ToolStripMenuItem CrtToolStripMenuItem;
        internal ToolStripMenuItem DorToolStripMenuItem;
        internal ToolStripMenuItem IntToolStripMenuItem;
        internal ToolStripMenuItem ItmToolStripMenuItem;
        internal ToolStripMenuItem MapToolStripMenuItem;
        internal ToolStripMenuItem UseToolStripMenuItem;
        internal ToolStripMenuItem WeaToolStripMenuItem;
        internal ToolStripMenuItem OptionsToolStripMenuItem;
        internal ToolStripMenuItem SetEnglishstfLocationToolStripMenuItem;
        internal DarkToolTip ToolTip;
        internal DarkGroupBox CRTgb;
        internal DarkGroupBox EEN2gb;
        internal DarkGroupBox GWAMgb;
        internal DarkGroupBox GENTgb;
        internal DarkGroupBox GCREgb;
        internal DarkGroupBox GCHRgb;
        internal DarkTextBox GCHRn;
        internal DarkTextBox EEN2actt;
        internal DarkTextBox EEN2invt;
        internal DarkTextBox EEN2skl;
        internal DarkCheckBox EEN2sel;
        internal DarkLabel EEN2s5l;
        internal DarkLabel EEN2s4l;
        internal DarkLabel EEN2s3l;
        internal DarkTextBox EEN2s5;
        internal DarkTextBox EEN2s4;
        internal DarkTextBox EEN2s3;
        internal DarkTextBox EEN2s2;
        internal DarkTextBox EEN2s1;
        internal DarkLabel EEN2acttl;
        internal DarkLabel EEN2invtl;
        internal DarkScrollBar EEN2dsb;
        internal DataGridView EEN2dgv;
        internal DarkTextBox GENTu;
        internal DarkTextBox GENTn;
        internal DarkTextBox GENTl;
        internal DarkTextBox GENTh;
        internal DarkLabel GENTihpl;
        internal DarkLabel GENTmhpl;
        internal DarkNumericUpDown GENTihp;
        internal DarkNumericUpDown GENTmhp;
        internal DarkNumericUpDown GENTuSR;
        internal DarkNumericUpDown GENTnSR;
        internal DarkNumericUpDown GENTlSR;
        internal DarkNumericUpDown GENThSR;
        internal DarkButton GWAMm;
        internal DarkComboBox GWAMcb;
        internal DarkButton GWAMp;
        internal TableLayoutPanel GWAMtlp;
        internal DarkLabel GWAManil;
        internal DarkNumericUpDown GWAMani;
        internal DarkLabel GWAMdtl;
        internal DarkComboBox GWAMdt;
        internal DarkNumericUpDown GWAMmax;
        internal DarkNumericUpDown GWAMmin;
        internal DarkLabel GWAMmaxl;
        internal DarkLabel GWAMminl;
        internal DarkNumericUpDown GWAMr;
        internal DarkNumericUpDown GWAMsf;
        internal DarkLabel GWAMsfl;
        internal DarkLabel GWAMrl;
        internal DarkLabel GWAMapl;
        internal DarkNumericUpDown GWAMap;
        internal DarkLabel GWAManl;
        internal DarkNumericUpDown GWAManSR;
        internal DarkTextBox GWAMef;
        internal DarkLabel GWAMefl;
        internal DarkTextBox GWAMan;
        internal DarkLabel GWAMefel;
        internal TableLayoutPanel GCREtlp;
        internal DarkNumericUpDown GCREskv;
        internal DarkNumericUpDown GCREspv;
        internal DarkLabel GCREspl;
        internal DarkComboBox GCREspcb;
        internal DarkLabel GCREskl;
        internal DarkLabel GCREtrl;
        internal DarkComboBox GCREskcb;
        internal DarkComboBox GCREtrcb;
        internal DarkLabel GCREtsl;
        internal DarkComboBox GCREtscb;
        internal DarkCheckBox GCREtrv;
        internal DarkCheckBox GCREtsv;
        internal DarkLabel GCREpl;
        internal DarkTextBox GCREp;
        internal DarkTextBox GCREsoct;
        internal DarkLabel GCREsoctl;
        internal DarkScrollBar GCREdsb;
        internal DarkLabel GCREsoctel;
        internal DataGridView GCREdgv;
        internal DarkLabel GCREsocl;
        internal DarkTextBox GCREsocm;
        internal DarkLabel GCREsocml;
        internal DarkComboBox GCREsoccb;
        internal DarkLabel GCREeil;
        internal DarkLabel GCREpel;
        internal Timer GCREtmr;
        internal Timer EEN2tmr;
        internal DarkNumericUpDown GCREage;
        internal DarkLabel GCREagel;
        internal ToolStripMenuItem EnableSTFEdit;
        internal DarkGroupBox _2MWTgb;
        internal DarkLabel DarkLabel2;
        internal DarkTextBox _2MWTtex;
        internal DarkTextBox _2MWTmpf;
        internal DarkButton _2MWTm;
        internal DarkButton _2MWTp;
        internal DarkComboBox _2MWTcb;
        internal DarkLabel DarkLabel1;
        internal DarkTextBox _2MWTz;
        internal DarkTextBox _2MWTy;
        internal DarkTextBox _2MWTx;
        internal DarkTextBox _2MWTlmy;
        internal DarkLabel DarkLabel3;
        internal DarkTextBox _2MWTlmx;
        internal DarkCheckBox _2MWTdw;
        internal DarkCheckBox _2MWTfr;
        internal DarkGroupBox ITMgb;
        internal DarkGroupBox GITMgb;
        internal TableLayoutPanel TableLayoutPanel1;
        internal DarkCheckBox GITMeq;
        internal DarkComboBox GITMslot;
        internal DarkCheckBox GITMml;
        internal DarkTextBox GITMrl;
        internal DarkLabel DarkLabel4;
        internal DarkLabel DarkLabel5;
        internal DarkLabel DarkLabel6;
        internal DarkLabel DarkLabel7;
        internal DarkCheckBox GITMhhai;
        internal DarkCheckBox GITMhbea;
        internal DarkCheckBox GITMhvan;
        internal DarkCheckBox GITMheye;
        internal DarkCheckBox GITMhmus;
        internal DarkCheckBox GITMhpon;
        internal DarkLabel DarkLabel8;
        internal DarkLabel DarkLabel9;
        internal DarkLabel DarkLabel10;
        internal DarkTextBox GITMsocm;
        internal DarkTextBox GITMsoct;
        internal DarkComboBox GITMsoccb;
        internal DarkComboBox GITMtype;
        internal ToolStripMenuItem OpenFromgrpToolStripMenuItem;
        internal ToolStripMenuItem ExtractgrpFilesToolStripMenuItem;
        internal ToolStripMenuItem ExtractAndConvertToolStripMenuItem;
        internal ToolStripMenuItem GrpBrowserToolStripMenuItem;
    }
}