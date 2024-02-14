using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
#region Filetype Classes

// Classes containing the headers used by that file type, in the order they should be put back into a new file.
using System.Text;

namespace VBLauncher
{

    public class Map
    {
        public EMAPc EMAP { get; set; }
        public List<EME2c> EME2 { get; set; }
        public List<EMEPc> EMEP { get; set; }
        public ECAMc ECAM { get; set; }
        public _2MWTc _2MWT { get; set; }
        public List<Trigger> Triggers { get; set; }
        public List<EPTHc> EPTH { get; set; }
        public List<EMSDc> EMSD { get; set; }
        public EMNPc EMNP { get; set; }
        public EMFGc EMFG { get; set; }
        public List<EMNOc> EMNO { get; set; } = new List<EMNOc>();
        public List<EMEFc> EMEF { get; set; }

        public Map()
        {
            EMAP = new EMAPc();
            EMEP = new List<EMEPc>();
            EME2 = new List<EME2c>();
            EMFG = null;
            EMNO = new List<EMNOc>();
            EMEF = new List<EMEFc>();
            EMSD = new List<EMSDc>();
            EPTH = new List<EPTHc>();
            Triggers = new List<Trigger>();
            ECAM = null;
            _2MWT = null;
        }

        public IEnumerable<byte> ToByte()
        {
            var b = new List<byte>();
            b.AddRange(EMAP.ToByte());
            b.AddRange(EME2.SelectMany(x => x.ToByte()));
            b.AddRange(EMEP.SelectMany(x => x.ToByte()));
            if (ECAM is not null)
                b.AddRange(ECAM.ToByte());
            if (_2MWT is not null)
                b.AddRange(_2MWT.ToByte());
            b.AddRange(Triggers.SelectMany(x => x.ToByte()));
            b.AddRange(EPTH.SelectMany(x => x.ToByte()));
            b.AddRange(EMSD.SelectMany(x => x.ToByte()));
            b.AddRange(new byte[] { 0x45, 0x4D, 0x4E, 0x50, 0x0, 0x0, 0x0, 0x0, 0x10, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 }); // EMNP Chunk
            if (EMFG is not null)
                b.AddRange(EMFG.ToByte());
            b.AddRange(EMNO.SelectMany(x => x.ToByte()));
            b.AddRange(EMEF.SelectMany(x => x.ToByte()));
            return b;
        }

    }

    public class CRT
    {
        public EEN2c EEN2 { get; set; }
        public GENTc GENT { get; set; }
        public GCREc GCRE { get; set; }
        public GCHRc GCHR { get; set; }

        public CRT()
        {
            EEN2 = new EEN2c();
            GENT = new GENTc();
            GCRE = new GCREc();
            GCHR = new GCHRc();
        }

        public IEnumerable<byte> ToByte()
        {
            var b = new List<byte>();
            b.AddRange(EEN2.ToByte());
            b.AddRange(GENT.ToByte());
            b.AddRange(GCRE.ToByte());
            if (!string.IsNullOrEmpty(GCHR.name))
                b.AddRange(GCHR.ToByte());
            return b;
        }

    }

    public class ITM
    {
        public EEN2c EEN2 { get; set; }
        public GENTc GENT { get; set; }
        public GITMc GITM { get; set; }

        public ITM()
        {
            EEN2 = new EEN2c();
            GENT = new GENTc();
            GITM = new GITMc();
        }

        public IEnumerable<byte> ToByte()
        {
            var b = new List<byte>();
            b.AddRange(EEN2.ToByte());
            b.AddRange(GENT.ToByte());
            b.AddRange(GITM.ToByte());
            return b;
        }

    }

    public class ARM
    {
        public EEN2c EEN2 { get; set; }
        public GENTc GENT { get; set; }
        public GITMc GITM { get; set; }
        public GIARc GIAR { get; set; }

        public ARM()
        {
            EEN2 = new EEN2c();
            GENT = new GENTc();
            GITM = new GITMc();
            GIAR = new GIARc();
        }

        public IEnumerable<byte> ToByte()
        {
            var b = new List<byte>();
            b.AddRange(EEN2.ToByte());
            b.AddRange(GENT.ToByte());
            b.AddRange(GITM.ToByte());
            b.AddRange(GIAR.ToByte());
            return b;
        }

    }

    public class AMO
    {
        public EEN2c EEN2 { get; set; }
        public GENTc GENT { get; set; }
        public GITMc GITM { get; set; }
        // Property GIAM As GIAMc

        public AMO()
        {
            EEN2 = new EEN2c();
            GENT = new GENTc();
            GITM = new GITMc();
            // GIAM = New GIAMc()
        }

        public IEnumerable<byte> ToByte()
        {
            var b = new List<byte>();
            b.AddRange(EEN2.ToByte());
            b.AddRange(GENT.ToByte());
            b.AddRange(GITM.ToByte());
            // b.AddRange(GIAM.ToByte())
            return b;
        }

    }

    public class USE // also encapsulates CON and DOR, which use static chunks that are not read from or written to, if/which chunk is used is controlled by GOBJ.
    {
        public EEN2c EEN2 { get; set; }
        public GENTc GENT { get; set; }
        public GOBJc GOBJ { get; set; }

        public USE()
        {
            EEN2 = new EEN2c();
            GENT = new GENTc();
            GOBJ = new GOBJc();
        }

        public IEnumerable<byte> ToByte()
        {
            var b = new List<byte>();
            b.AddRange(EEN2.ToByte());
            b.AddRange(GENT.ToByte());
            b.AddRange(GOBJ.ToByte());
            if (GOBJ.Type == 0)
            {
            }
            else if (GOBJ.Type == 1)
            {
                b.AddRange(new byte[] { 0x47, 0x44, 0x4F, 0x52, 0x1, 0x0, 0x0, 0x0, 0x18, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 }); // GDOR chunk
            }
            else if (GOBJ.Type == 2)
            {
                b.AddRange(new byte[] { 0x47, 0x43, 0x4F, 0x4E, 0x1, 0x0, 0x0, 0x0, 0x1A, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 }); // GCON chunk
            }
            return b;
        }

    }

    public class WEA
    {
        public EEN2c EEN2 { get; set; }
        public GENTc GENT { get; set; }
        public GITMc GITM { get; set; }
        // Property GIWP As GIWPc

        public WEA()
        {
            EEN2 = new EEN2c();
            GENT = new GENTc();
            GITM = new GITMc();
            // GIWP = New GIWPc()
        }

        public IEnumerable<byte> ToByte()
        {
            var b = new List<byte>();
            b.AddRange(EEN2.ToByte());
            b.AddRange(GENT.ToByte());
            b.AddRange(GITM.ToByte());
            // b.AddRange(GIWP.ToByte())
            return b;
        }

    }

    #endregion

    #region Header Classes

    // Classes to hold the data in easily manipulatable format
    public class Trigger
    {

        // Triggers are made of 3 different headers (separate, unlike EME2 and EEOV), so there is a class to hold both types so they aren't separated.
        public EMTRc EMTR { get; set; }

        public ExTRc ExTR { get; set; }

        public Trigger()
        {
            EMTR = new EMTRc();
            ExTR = new ExTRc();
        }

        public IEnumerable<byte> ToByte()
        {
            var b = new List<byte>();
            b.AddRange(EMTR.ToByte());
            b.AddRange(ExTR.ToByte());
            return b;
        }

    }

    public class EMAPc
    {
        public string s1 { get; set; }
        public string s2 { get; set; }
        public string s3 { get; set; }
        public Color col { get; set; }
        public bool il { get; set; }
        public int le { get; set; }

        public EMAPc()
        {
            s1 = "";
            s2 = "";
            s3 = "";
            col = Color.Black;
            il = false;
            le = 0;
        }

        public IEnumerable<byte> ToByte()
        {
            int s = 49 + s1.Length + s2.Length + s3.Length;
            byte[] ret = new byte[s];
            Extensions.Write(ret, 0, "EMAP");
            Extensions.Write(ret, 4, il ? 0 : 5);
            Extensions.Write(ret, 8, s);
            Extensions.Write(ret, 16, s1.Length);
            Extensions.Write(ret, 18, s1);
            Extensions.Write(ret, 18 + s1.Length, s2.Length);
            Extensions.Write(ret, 20 + s1.Length, s2);
            Extensions.Write(ret, 20 + s1.Length + s2.Length, s3.Length);
            Extensions.Write(ret, 22 + s1.Length + s2.Length, s3);
            Extensions.Write(ret, 22 + s1.Length + s2.Length + s3.Length, le);
            Extensions.Write(ret, 24 + s1.Length + s2.Length + s3.Length, col.ToByte());
            Extensions.Write(ret, 32 + s1.Length + s2.Length + s3.Length, 1);
            return ret;
        }

    }

    public class EME2c
    {
        public string name { get; set; }
        public Point4 l { get; set; }
        public EEOVc EEOV { get; set; }

        public EME2c()
        {
            name = "";
            l = new Point4(0f, 0f, 0f, 0f);
            EEOV = new EEOVc();
        }

        public IEnumerable<byte> ToByte()
        {
            var oEEOV = EEOV.ToByte();
            byte[] ret = new byte[38 + name.Length + oEEOV.Count() + 1];
            Extensions.Write(ret, 0, "EME2");
            Extensions.Write(ret, 8, 39 + name.Length + oEEOV.Count());
            Extensions.Write(ret, 12, name.Length);
            Extensions.Write(ret, 14, name);
            Extensions.Write(ret, 14 + name.Length, l.ToByte());
            Extensions.Write(ret, 38 + name.Length, 1);
            Extensions.Write(ret, 39 + name.Length, oEEOV);
            return ret;
        }

    }

    public class EEOVc
    {
        public string s1 { get; set; }
        public string s2 { get; set; }
        public string s3 { get; set; }
        public string s4 { get; set; }
        public int ps4 { get; set; }
        public string s5 { get; set; }
        public List<string> inv { get; set; }

        public EEOVc()
        {
            s1 = "";
            s2 = "";
            s3 = "";
            s4 = "";
            ps4 = 0;
            s5 = "";
            inv = new List<string>();
        }

        public IEnumerable<byte> ToByte()
        {
            int invl = inv.Sum(e => e.Length + 2);
            int a = ps4 == 2 ? 2 : 0;
            byte[] ret = new byte[46 + s1.Length + s2.Length + s3.Length + s4.Length + s5.Length + invl + a + 1];
            Extensions.Write(ret, 0, "EEOV");
            if (inv.Any())
                Extensions.Write(ret, 4, 2);
            Extensions.Write(ret, 8, 47 + s1.Length + s2.Length + s3.Length + s4.Length + s5.Length + invl + a);
            Extensions.Write(ret, 12, s1.Length);
            Extensions.Write(ret, 14, s1);
            Extensions.Write(ret, 25 + s1.Length, s2.Length);
            Extensions.Write(ret, 27 + s1.Length, s2);
            Extensions.Write(ret, 27 + s1.Length + s2.Length, s3.Length);
            Extensions.Write(ret, 29 + s1.Length + s2.Length, s3);
            Extensions.Write(ret, 38 + s1.Length + s2.Length + s3.Length, s4.Length);
            Extensions.Write(ret, 40 + s1.Length + s2.Length + s3.Length, s4);
            Extensions.Write(ret, 40 + s1.Length + s2.Length + s3.Length + s4.Length, ps4);
            Extensions.Write(ret, 41 + s1.Length + s2.Length + s3.Length + s4.Length + a, s5.Length);
            Extensions.Write(ret, 43 + s1.Length + s2.Length + s3.Length + s4.Length + a, s5);
            Extensions.Write(ret, 43 + s1.Length + s2.Length + s3.Length + s4.Length + s5.Length + a, inv.Count);
            int i = 0;
            foreach (var e in inv)
            {
                Extensions.Write(ret, 47 + s1.Length + s2.Length + s3.Length + s4.Length + s5.Length + a + i, e.Length);
                Extensions.Write(ret, 49 + s1.Length + s2.Length + s3.Length + s4.Length + s5.Length + a + i, e);
                i += e.Length + 2;
            }
            return ret;
        }

    }

    public class EMEPc
    {
        public int index { get; set; }
        public Point3 p { get; set; }
        public float r { get; set; }

        public EMEPc()
        {
            index = 0;
            p = new Point3(0f, 0f, 0f);
            r = 0f;
        }

        public IEnumerable<byte> ToByte()
        {
            byte[] ret = new byte[109];
            Extensions.Write(ret, 0, "EMEP");
            Extensions.Write(ret, 8, 109);
            Extensions.Write(ret, 12, index);
            Extensions.Write(ret, 73, p.ToByte());
            Extensions.Write(ret, 105, r);
            return ret;
        }

    }

    public class ECAMc
    {
        public Point4 p { get; set; }

        public ECAMc()
        {
            p = new Point4(0f, 0f, 0f, 0f);
        }

        public IEnumerable<byte> ToByte()
        {
            byte[] ret = new byte[28];
            Extensions.Write(ret, 0, "ECAM");
            Extensions.Write(ret, 8, 28);
            Extensions.Write(ret, 12, p.ToByte());
            return ret;
        }

    }

    public class EMEFc
    {
        public string s1 { get; set; }
        public string s2 { get; set; }
        public Point4 l { get; set; }
        public byte b { get; set; }

        public EMEFc()
        {
            s1 = "";
            s2 = "";
            l = new Point4(0f, 0f, 0f, 0f);
            b = 0;
        }

        public IEnumerable<byte> ToByte()
        {
            byte[] ret = new byte[41 + s1.Length + s2.Length + 1];
            Extensions.Write(ret, 0, "EMEF");
            Extensions.Write(ret, 8, 42 + s1.Length + s2.Length);
            Extensions.Write(ret, 12, s1.Length);
            Extensions.Write(ret, 14, s1);
            Extensions.Write(ret, 14 + s1.Length, l.ToByte());
            Extensions.Write(ret, 38 + s1.Length, 1);
            Extensions.Write(ret, 39 + s1.Length, s2.Length);
            Extensions.Write(ret, 41 + s1.Length, s2);
            Extensions.Write(ret, 41 + s1.Length + s2.Length, b);
            return ret;
        }

    }

    public class EMSDc
    {
        public string s1 { get; set; }
        public Point3 l { get; set; }
        public string s2 { get; set; }

        public EMSDc()
        {
            s1 = "";
            l = new Point3(0f, 0f, 0f);
            s2 = "";
        }

        public IEnumerable<byte> ToByte()
        {
            byte[] ret = new byte[29 + s1.Length + s2.Length + 1];
            Extensions.Write(ret, 0, "EMSD");
            Extensions.Write(ret, 8, 30 + s1.Length + s2.Length);
            Extensions.Write(ret, 12, s1.Length);
            Extensions.Write(ret, 14, s1);
            Extensions.Write(ret, 14 + s1.Length, l.ToByte());
            Extensions.Write(ret, 26 + s1.Length, s2.Length);
            Extensions.Write(ret, 28 + s1.Length, s2);
            Extensions.Write(ret, 28 + s1.Length + s2.Length, new byte[] { 1, 1 });
            return ret;
        }

    }

    public class EPTHc
    {
        public string name { get; set; }
        public List<Point4> p { get; set; }

        public EPTHc()
        {
            name = "";
            p = new List<Point4>() { new Point4(0f, 0f, 0f, 0f) };
        }

        public IEnumerable<byte> ToByte()
        {
            byte[] ret = new byte[17 + p.Count * 24 + name.Length + 1];
            Extensions.Write(ret, 0, "EPTH");
            Extensions.Write(ret, 8, 18 + p.Count * 24 + name.Length);
            Extensions.Write(ret, 12, name.Length);
            Extensions.Write(ret, 14, name);
            Extensions.Write(ret, 14 + name.Length, p.Count);
            int i = 0;
            foreach (var l in p)
            {
                Extensions.Write(ret, 18 + name.Length + i, l.ToByte());
                i += 24;
            }
            return ret;
        }

    }

    public class EMTRc
    {
        public int n { get; set; }
        public List<Point3> r { get; set; }

        public EMTRc()
        {
            n = 0;
            r = new List<Point3>() { new Point3(0f, 0f, 0f) };
        }

        public IEnumerable<byte> ToByte()
        {
            byte[] ret = new byte[19 + r.Count * 12 + 1];
            Extensions.Write(ret, 0, "EMTR");
            Extensions.Write(ret, 8, 20 + r.Count * 12);
            Extensions.Write(ret, 12, n);
            Extensions.Write(ret, 16, r.Count);
            int i = 0;
            foreach (var l in r)
            {
                Extensions.Write(ret, 20 + i, l.ToByte());
                i += 12;
            }
            return ret;
        }

    }

    public class ExTRc // Called ExTR instead of E(T/S/B)TR for easier handling within triggers
    {
        public string @type { get; set; } // So we know which file is being created, T, S, or B. (or M, but it's ignored if that happens)
        public string s { get; set; }

        public ExTRc()
        {
            @type = "T";
            s = "";
        }

        public IEnumerable<byte> ToByte()
        {
            switch (@type ?? "")
            {
                case "B":
                    {
                        byte[] ret = new byte[19];
                        Extensions.Write(ret, 0, "EBTR");
                        Extensions.Write(ret, 8, 19);
                        Extensions.Write(ret, 12, s);
                        Extensions.Write(ret, 16, "FFF");
                        return ret;
                    }
                case "S":
                    {
                        byte[] ret = new byte[17 + s.Length + 1];
                        Extensions.Write(ret, 0, "ESTR");
                        Extensions.Write(ret, 8, 18 + s.Length);
                        Extensions.Write(ret, 12, s.Length);
                        Extensions.Write(ret, 14, s);
                        return ret;
                    }
                case "T":
                    {
                        byte[] ret = new byte[15 + s.Length + 1];
                        Extensions.Write(ret, 0, "ETTR");
                        Extensions.Write(ret, 8, 16 + s.Length);
                        Extensions.Write(ret, 12, s.Length);
                        Extensions.Write(ret, 14, s);
                        Extensions.Write(ret, 14 + s.Length, new byte[] { 1, 1 });
                        return ret;
                    }

                default:
                    {
                        return Array.Empty<byte>();
                    }
            }
        }

    }

    public class EEN2c
    {
        public string skl { get; set; }
        public string invtex { get; set; }
        public string acttex { get; set; }
        public bool sel { get; set; }
        public EEOVc EEOV { get; set; }

        public EEN2c()
        {
            skl = "";
            invtex = "";
            acttex = "";
            sel = false;
            EEOV = new EEOVc();
        }

        public IEnumerable<byte> ToByte()
        {
            var oEEOV = EEOV.ToByte();
            byte[] ret = new byte[22 + oEEOV.Count() + skl.Length + invtex.Length + acttex.Length + 1];
            Extensions.Write(ret, 0, "EEN2");
            Extensions.Write(ret, 8, 23 + oEEOV.Count() + skl.Length + invtex.Length + acttex.Length);
            Extensions.Write(ret, 12, skl.Length);
            Extensions.Write(ret, 14, skl);
            Extensions.Write(ret, 14 + skl.Length, invtex.Length);
            Extensions.Write(ret, 16 + skl.Length, invtex);
            Extensions.Write(ret, 16 + skl.Length + invtex.Length, acttex.Length);
            Extensions.Write(ret, 18 + skl.Length + invtex.Length, acttex);
            Extensions.Write(ret, 19 + skl.Length + invtex.Length + acttex.Length, sel);
            Extensions.Write(ret, 23 + skl.Length + invtex.Length + acttex.Length, oEEOV);
            return ret;
        }

    }

    public class GENTc
    {
        public int HoverSR { get; set; } // String used when moused over
        public int LookSR { get; set; } // String used when "Look" option is used
        public int NameSR { get; set; } // String of the entities' name
        public int UnkwnSR { get; set; } // String used ???
        public int MaxHealth { get; set; }
        public int StartHealth { get; set; }

        public GENTc()
        {
            HoverSR = 0;
            LookSR = 0;
            NameSR = 0;
            UnkwnSR = 0;
            MaxHealth = 0;
            StartHealth = 0;
        }

        public IEnumerable<byte> ToByte()
        {
            byte[] ret = new byte[44];
            Extensions.Write(ret, 0, "GENT");
            Extensions.Write(ret, 4, 1);
            Extensions.Write(ret, 8, 44);
            Extensions.Write(ret, 12, HoverSR);
            Extensions.Write(ret, 16, LookSR);
            Extensions.Write(ret, 20, NameSR);
            Extensions.Write(ret, 24, UnkwnSR);
            Extensions.Write(ret, 36, MaxHealth);
            Extensions.Write(ret, 40, StartHealth);
            return ret;
        }

    }

    public class GCREc
    {
        public int[] Special { get; set; }
        public int Age { get; set; }
        public List<Skill> Skills { get; set; }
        public List<int> Traits { get; set; }
        public List<int> TagSkills { get; set; }
        public string PortStr { get; set; }
        public Socket Hea { get; set; }
        public Socket Hai { get; set; }
        public Socket Pon { get; set; }
        public Socket Mus { get; set; }
        public Socket Bea { get; set; }
        public Socket Eye { get; set; }
        public Socket Bod { get; set; }
        public Socket Han { get; set; }
        public Socket Fee { get; set; }
        public Socket Bac { get; set; }
        public Socket Sho { get; set; }
        public Socket Van { get; set; }
        public List<string> Inventory { get; set; }
        public List<GWAMc> GWAM { get; set; }

        public GCREc()
        {
            Special = new int[] { 0, 0, 0, 0, 0, 0, 0 };
            Age = 0;
            Skills = new List<Skill>();
            Traits = new List<int>();
            TagSkills = new List<int>();
            PortStr = "";
            Hea = new Socket("", "");
            Hai = new Socket("", "");
            Pon = new Socket("", "");
            Mus = new Socket("", "");
            Bea = new Socket("", "");
            Eye = new Socket("", "");
            Bod = new Socket("", "");
            Han = new Socket("", "");
            Fee = new Socket("", "");
            Bac = new Socket("", "");
            Sho = new Socket("", "");
            Van = new Socket("", "");
            Inventory = new List<string>();
            GWAM = new List<GWAMc>();
        }

        public IEnumerable<byte> ToByte()
        {
            var GWAMb = new List<byte>();
            foreach (var g in GWAM)
                GWAMb.AddRange(g.ToByte());
            Socket[] socs = new Socket[] { Hea, Hai, Pon, Mus, Bea, Eye, Bod, Han, Fee, Bac, Sho, Van };
            var sock = new List<byte>();
            foreach (var s in socs)
                sock.AddRange(s.ToByte());
            var inv = new List<byte>();
            foreach (var i in Inventory)
            {
                inv.AddRange(new byte[] { (byte)i.Length, 0 });
                inv.AddRange(Encoding.ASCII.GetBytes(i));
            }

            int sl = Skills.Count * 8; // Skills length
            int tl = Traits.Count * 4; // Traits length
            int tsl = TagSkills.Count * 4; // Tag Skills length
            int il = Inventory.Sum(i => i.Length + 2); // Inventory Length
            int socl = Hea.Length + Hai.Length + Pon.Length + Mus.Length + Bea.Length + Eye.Length + Bod.Length + Han.Length + Fee.Length + Bac.Length + Sho.Length + Van.Length; // Socket String Length
            int TDL = sl + tl + tsl + il + GWAMb.Count + PortStr.Length + socl; // Total Dynamic Length

            byte[] ret = new byte[276 + TDL + 1];
            Extensions.Write(ret, 0, "GCRE");
            Extensions.Write(ret, 4, 4);
            Extensions.Write(ret, 8, 277 + TDL);
            Extensions.Write(ret, 12, Special[0]);
            Extensions.Write(ret, 16, Special[1]);
            Extensions.Write(ret, 20, Special[2]);
            Extensions.Write(ret, 24, Special[3]);
            Extensions.Write(ret, 28, Special[4]);
            Extensions.Write(ret, 32, Special[5]);
            Extensions.Write(ret, 36, Special[6]);
            Extensions.Write(ret, 56, Age);
            Extensions.Write(ret, 72, Skills.Count);
            for (int i = 0, loopTo = Skills.Count - 1; i <= loopTo; i++)
                Extensions.Write(ret, 76 + i * 8, Skills[i].ToByte());
            Extensions.Write(ret, 80 + sl, Traits.Count);
            for (int i = 0, loopTo1 = Traits.Count - 1; i <= loopTo1; i++)
                Extensions.Write(ret, 84 + sl + i * 4, Traits[i]);
            Extensions.Write(ret, 84 + sl + tl, TagSkills.Count);
            for (int i = 0, loopTo2 = TagSkills.Count - 1; i <= loopTo2; i++)
                Extensions.Write(ret, 88 + sl + tl + i * 4, TagSkills[i]);
            Extensions.Write(ret, 92 + sl + tl + tsl, PortStr.Length);
            Extensions.Write(ret, 94 + sl + tl + tsl, PortStr);
            Extensions.Write(ret, 129 + sl + tl + tsl + PortStr.Length, sock);
            Extensions.Write(ret, 189 + sl + tl + tsl + PortStr.Length + socl, GWAM.Count);
            Extensions.Write(ret, 273 + sl + tl + tsl + PortStr.Length + socl, Inventory.Count);
            Extensions.Write(ret, 277 + sl + tl + tsl + PortStr.Length + socl, inv);
            Extensions.Write(ret, 277 + sl + tl + tsl + PortStr.Length + socl + il, GWAMb);
            return ret;
        }

    }

    public class GWAMc
    {
        public int Anim { get; set; }
        public int DmgType { get; set; }
        public int ShotsFired { get; set; }
        public int Range { get; set; }
        public int MinDmg { get; set; }
        public int MaxDmg { get; set; }
        public int AP { get; set; }
        public int NameSR { get; set; }
        public string VegName { get; set; }

        public GWAMc()
        {
            Anim = 0;
            DmgType = 0;
            ShotsFired = 0;
            Range = 0;
            MinDmg = 0;
            MaxDmg = 0;
            AP = 0;
            NameSR = 0;
            VegName = 0.ToString();
        }

        public IEnumerable<byte> ToByte()
        {
            byte[] ret = new byte[79 + VegName.Length + 1];
            Extensions.Write(ret, 0, "GWAM");
            Extensions.Write(ret, 4, 5);
            Extensions.Write(ret, 8, 80 + VegName.Length);
            Extensions.Write(ret, 12, Anim);
            Extensions.Write(ret, 16, DmgType);
            Extensions.Write(ret, 20, ShotsFired);
            Extensions.Write(ret, 36, Range);
            Extensions.Write(ret, 48, MinDmg);
            Extensions.Write(ret, 52, MaxDmg);
            Extensions.Write(ret, 62, AP);
            Extensions.Write(ret, 72, NameSR);
            Extensions.Write(ret, 78, VegName);
            Extensions.Write(ret, ret.Length - 2, 1);
            return ret;
        }

    }

    public class GCHRc
    {
        public string name { get; set; }

        public GCHRc()
        {
            name = "";
        }

        public IEnumerable<byte> ToByte()
        {
            byte[] ret = new byte[13 + name.Length + 1];
            Extensions.Write(ret, 0, "GCHR");
            Extensions.Write(ret, 8, 14 + name.Length);
            Extensions.Write(ret, 12, name.Length);
            Extensions.Write(ret, 14, name);
            return ret;
        }

    }

    public class _2MWTc
    {
        public string mpf { get; set; }
        public bool frozen { get; set; }
        public bool dark { get; set; }
        public List<_2MWTChunk> chunks { get; set; }

        public _2MWTc()
        {
            mpf = "";
            chunks = new List<_2MWTChunk>();
        }

        public IEnumerable<byte> ToByte()
        {
            int sl = chunks.Sum(x => x.tex.Length) + mpf.Length; // length of strings
            int wl = chunks.Count * 22; // added length for each water chunk

            byte[] ret = new byte[157 + sl + wl + 1];
            Extensions.Write(ret, 0, "2MWT");
            Extensions.Write(ret, 8, 158 + sl + wl);
            Extensions.Write(ret, 12, mpf.Length);
            Extensions.Write(ret, 14, mpf);
            Extensions.Write(ret, 27 + mpf.Length, !dark);
            Extensions.Write(ret, 29 + mpf.Length, !frozen);
            Extensions.Write(ret, 154 + mpf.Length, chunks.Count);
            int io = 158 + mpf.Length;
            foreach (var w in chunks)
            {
                Extensions.Write(ret, io, w.ToByte());
                io += 22 + w.tex.Length;
            }
            return ret;
        }

    }

    public class GITMc            // "Missing" Sockets:
    {
        public int @type { get; set; }  // ? Hai
        public bool equip { get; set; } // ? Pon
        public int eqslot { get; set; } // ? Mus
        public Socket Hea { get; set; }    // ? Bea
        public bool hHai { get; set; }
        public bool hBea { get; set; }
        public bool hMus { get; set; }
        public bool hEye { get; set; }
        public bool hPon { get; set; }
        public bool hVan { get; set; }
        public Socket Eye { get; set; }
        public Socket Bod { get; set; }
        public Socket Bac { get; set; }
        public Socket Han { get; set; }
        public Socket Fee { get; set; }
        public Socket Sho { get; set; }
        public Socket Van { get; set; }
        public Socket IHS { get; set; }
        public int reload { get; set; }

        public GITMc()
        {
            @type = 0;
            equip = false;
            eqslot = 0;
            Hea = new Socket("", "");
            hHai = false;
            hBea = false;
            hMus = false;
            hEye = false;
            hPon = false;
            hVan = false;
            Eye = new Socket("", "");
            Bod = new Socket("", "");
            Bac = new Socket("", "");
            Han = new Socket("", "");
            Fee = new Socket("", "");
            Sho = new Socket("", "");
            Van = new Socket("", "");
            IHS = new Socket("", "");
            reload = 0;
        }

        public IEnumerable<byte> ToByte()
        {
            int socl = Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length + Fee.Length + Sho.Length + Van.Length + IHS.Length;
            byte[] ret = new byte[95 + socl + 1];
            Extensions.Write(ret, 0, "GITM");
            Extensions.Write(ret, 4, 1);
            Extensions.Write(ret, 8, 96 + socl);
            Extensions.Write(ret, 12, @type);
            Extensions.Write(ret, 20, equip);
            Extensions.Write(ret, 24, eqslot);
            Extensions.Write(ret, 28, Hea.ToByte());
            Extensions.Write(ret, 32 + Hea.Length, hHai);
            Extensions.Write(ret, 33 + Hea.Length, hBea);
            Extensions.Write(ret, 34 + Hea.Length, hMus);
            Extensions.Write(ret, 35 + Hea.Length, hEye);
            Extensions.Write(ret, 36 + Hea.Length, hPon);
            Extensions.Write(ret, 37 + Hea.Length, hVan);
            Extensions.Write(ret, 41 + Hea.Length, 1);
            Extensions.Write(ret, 46 + Hea.Length, 1);
            Extensions.Write(ret, 51 + Hea.Length, 1);
            Extensions.Write(ret, 52 + Hea.Length, Eye.ToByte());
            Extensions.Write(ret, 56 + Hea.Length + Eye.Length, 1);
            Extensions.Write(ret, 61 + Hea.Length + Eye.Length, 1);
            Extensions.Write(ret, 62 + Hea.Length + Eye.Length, Bod.ToByte());
            Extensions.Write(ret, 66 + Hea.Length + Eye.Length + Bod.Length, Bac.ToByte());
            Extensions.Write(ret, 70 + Hea.Length + Eye.Length + Bod.Length + Bac.Length, Han.ToByte());
            Extensions.Write(ret, 75 + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length, Fee.ToByte());
            Extensions.Write(ret, 79 + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length + Fee.Length, Sho.ToByte());
            Extensions.Write(ret, 83 + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length + Fee.Length + Sho.Length, Van.ToByte());
            Extensions.Write(ret, 87 + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length + Fee.Length + Sho.Length + Van.Length, 1);
            Extensions.Write(ret, 88 + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length + Fee.Length + Sho.Length + Van.Length, IHS.ToByte());
            Extensions.Write(ret, 92 + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length + Fee.Length + Sho.Length + Van.Length + IHS.Length, reload);
            return ret;
        }

    }

    public class GIARc
    {
        public int BallR { get; set; }
        public int BioR { get; set; }
        public int ElecR { get; set; }
        public int EMPR { get; set; }
        public int NormR { get; set; }
        public int HeatR { get; set; }

        public GIARc()
        {
            BallR = 0;
            BioR = 0;
            ElecR = 0;
            EMPR = 0;
            NormR = 0;
            HeatR = 0;
        }

        public IEnumerable<byte> ToByte()
        {
            byte[] ret = new byte[40];
            Extensions.Write(ret, 0, "GIAR");
            Extensions.Write(ret, 8, 40);
            Extensions.Write(ret, 12, BallR);
            Extensions.Write(ret, 16, BioR);
            Extensions.Write(ret, 20, ElecR);
            Extensions.Write(ret, 24, EMPR);
            Extensions.Write(ret, 28, NormR);
            Extensions.Write(ret, 32, HeatR);
            return ret;
        }

    }

    public class GOBJc
    {
        public int Type { get; set; }

        public GOBJc()
        {
            Type = 0;
        }

        public IEnumerable<byte> ToByte()
        {
            byte[] ret = new byte[40];
            Extensions.Write(ret, 0, "GOBJ");
            Extensions.Write(ret, 8, 40);
            Extensions.Write(ret, 12, Type);
            return ret;
        }

    }

    public class EMNOc
    {
        public Point2 l { get; set; } // technically XYZ Coordinates
        public string tex { get; set; }
        public int sr { get; set; }

        public EMNOc()
        {
            l = new Point2(0f, 0f);
            tex = "";
            sr = 0;
        }

        public IEnumerable<byte> ToByte()
        {
            byte[] ret = new byte[29 + tex.Length + 1];
            Extensions.Write(ret, 0, "EMNO");
            Extensions.Write(ret, 8, 30 + tex.Length);
            Extensions.Write(ret, 12, l.x);
            Extensions.Write(ret, 20, l.y);
            Extensions.Write(ret, 24, tex.Length);
            Extensions.Write(ret, 26, tex);
            Extensions.Write(ret, 26 + tex.Length, sr);
            return ret;
        }

    }

    public class EMFGc
    {
        public bool enabled { get; set; }
        public Color colour { get; set; }
        public float base_height { get; set; }
        public float anim1Speed { get; set; }
        public float anim1Height { get; set; }
        public float total_height { get; set; }
        public float anim2Speed { get; set; }
        public float anim2Height { get; set; }
        public float verticalOffset { get; set; }
        public float max_fog_density { get; set; }

        public EMFGc()
        {
            enabled = false;
            colour = Color.Black;
            base_height = 0f;
            anim1Speed = 0f;
            anim1Height = 0f;
            total_height = 0f;
            anim2Speed = 0f;
            anim2Height = 0f;
            verticalOffset = 0f;
            max_fog_density = 0f;
        }

        public IEnumerable<byte> ToByte()
        {
            // using in-game errors to prevent saving broken map
            if (base_height <= 0f)
            {
                throw new Exception("base_height <= 0.0f!");
            }
            if (base_height > total_height)
            {
                throw new Exception("base_height > total_height!");
            }
            if (total_height <= 0f)
            {
                throw new Exception("total_height <= 0.0f!");
            }
            if (max_fog_density <= 0f | max_fog_density > 1f)
            {
                throw new Exception("invalid max_fog_density");
            }
            byte[] ret = new byte[72];
            Extensions.Write(ret, 0, "EMFG");
            Extensions.Write(ret, 8, 72);
            Extensions.Write(ret, 12, enabled);
            Extensions.Write(ret, 13, colour.ToByte());
            Extensions.Write(ret, 16, base_height);
            Extensions.Write(ret, 20, anim1Speed);
            Extensions.Write(ret, 24, anim1Height);
            Extensions.Write(ret, 28, total_height);
            Extensions.Write(ret, 32, anim2Speed);
            Extensions.Write(ret, 36, anim2Height);
            Extensions.Write(ret, 40, verticalOffset);
            Extensions.Write(ret, 44, max_fog_density);
            return ret;
        }

    }

    public class EMNPChunk
    {
        public byte @bool { get; set; }
        public Point3 l { get; set; }
        public byte b1 { get; set; }
        public byte b2 { get; set; }
        public byte b3 { get; set; }
        public byte b4 { get; set; }
        public byte b5 { get; set; }

        public EMNPChunk()
        {
            @bool = 0;
            l = new Point3(0f, 0f, 0f);
            b1 = 0;
            b2 = 0;
            b3 = 0;
            b4 = 0;
            b5 = 0;
        }

        public IEnumerable<byte> ToByte()
        {
            byte[] ret = new byte[30];
            Extensions.Write(ret, 0, @bool);
            Extensions.Write(ret, 1, l.ToByte());
            Extensions.Write(ret, 25, b1);
            Extensions.Write(ret, 26, b2);
            Extensions.Write(ret, 27, b3);
            Extensions.Write(ret, 28, b4);
            Extensions.Write(ret, 29, b5);
            return ret;
        }

    }

    public class EMNPc
    {
        public List<EMNPChunk> chunks { get; set; }

        public EMNPc()
        {
            chunks = new List<EMNPChunk>();
        }

        public IEnumerable<byte> ToByte()
        {
            byte[] ret = new byte[15 + chunks.Count * 30 + 1];
            Extensions.Write(ret, 0, "EMNP");
            Extensions.Write(ret, 8, 16 + chunks.Count * 30);
            Extensions.Write(ret, 12, chunks.Count);
            for (int i = 0, loopTo = chunks.Count - 1; i <= loopTo; i++)
                Extensions.Write(ret, 16 + i * 30, chunks[i].ToByte());
            return default;
        }

    }

    #endregion

    #region Other Classes

    public class Point2
    {
        public float x { get; set; }
        public float y { get; set; }

        public Point2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public IEnumerable<byte> ToByte()
        {
            byte[] ret = new byte[8];
            Extensions.Write(ret, 0, x);
            Extensions.Write(ret, 4, y);
            return ret;
        }

    }

    public class Point3
    {
        public float x { get; set; }
        public float z { get; set; }
        public float y { get; set; }

        public Point3(float x, float z, float y)
        {
            this.x = x;
            this.z = z;
            this.y = y;
        }

        public IEnumerable<byte> ToByte()
        {
            byte[] ret = new byte[12];
            Extensions.Write(ret, 0, x);
            Extensions.Write(ret, 4, z);
            Extensions.Write(ret, 8, y);
            return ret;
        }

    }

    public class Point4
    {
        public float x { get; set; }
        public float z { get; set; }
        public float y { get; set; }
        public float r { get; set; }

        public Point4(float x, float z, float y, float r)
        {
            this.x = x;
            this.z = z;
            this.y = y;
            this.r = r;
        }

        public IEnumerable<byte> ToByte()
        {
            byte[] ret = new byte[16];
            Extensions.Write(ret, 0, x);
            Extensions.Write(ret, 4, z);
            Extensions.Write(ret, 8, y);
            Extensions.Write(ret, 12, r);
            return ret;
        }

    }

    public class Skill
    {
        public int Index { get; set; }
        public int Value { get; set; }

        public Skill(int Index, int Value)
        {
            this.Index = Index;
            this.Value = Value;
        }

        public IEnumerable<byte> ToByte()
        {
            byte[] ret = new byte[8];
            Extensions.Write(ret, 0, Index);
            Extensions.Write(ret, 4, Value);
            return ret;
        }

    }

    public class Socket
    {
        public string Model { get; set; }
        public string Tex { get; set; }

        /// <summary>
    /// Gets the length of both strings
    /// </summary>
        public int Length
        {
            get
            {
                return Model.Length + Tex.Length;
            }
        }

        public Socket(string Model, string Tex)
        {
            this.Model = Model;
            this.Tex = Tex;
        }

        public IEnumerable<byte> ToByte()
        {
            byte[] ret = new byte[3 + Model.Length + Tex.Length + 1];
            Extensions.Write(ret, 0, Model.Length);
            Extensions.Write(ret, 2, Model);
            Extensions.Write(ret, 2 + Model.Length, Tex.Length);
            Extensions.Write(ret, 4 + Model.Length, Tex);
            return ret;
        }

    }

    public class _2MWTChunk
    {
        public string tex { get; set; }
        public Point3 loc { get; set; }
        public Point2 texloc { get; set; }

        public _2MWTChunk(string tex, Point3 loc, Point2 texloc)
        {
            this.tex = tex;
            this.loc = loc;
            this.texloc = texloc;
        }

        public IEnumerable<byte> ToByte()
        {
            byte[] ret = new byte[21 + tex.Length + 1];
            Extensions.Write(ret, 0, loc.x);
            Extensions.Write(ret, 4, loc.z);
            Extensions.Write(ret, 8, loc.y);
            Extensions.Write(ret, 12, tex.Length);
            Extensions.Write(ret, 14, tex);
            Extensions.Write(ret, 14 + tex.Length, texloc.ToByte());
            return ret;
        }

    }
}

#endregion
