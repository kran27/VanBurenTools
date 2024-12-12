using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

#region Filetype Classes

// Classes containing the headers used by that file type, in the order they should be put back into a new file.
using System.Text;

namespace VBLauncher
{

    public class Map
    {
        public EMAPc EMAP;
        public List<EME2c> EME2;
        public List<EMEPc> EMEP;
        public ECAMc ECAM;
        public _2MWTc _2MWT;
        public List<Trigger> Triggers;
        public List<EPTHc> EPTH;
        public List<EMSDc> EMSD;
        public EMNPc EMNP;
        public EMFGc EMFG;
        public List<EMNOc> EMNO;
        public List<EMEFc> EMEF;

        public Map()
        {
            EMAP = new EMAPc();
            EMEP = [];
            EME2 = [];
            EMFG = null;
            EMNO = [];
            EMEF = [];
            EMSD = [];
            EPTH = [];
            Triggers = [];
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
            b.AddRange([0x45, 0x4D, 0x4E, 0x50, 0x0, 0x0, 0x0, 0x0, 0x10, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0]); // EMNP Chunk
            if (EMFG is not null)
                b.AddRange(EMFG.ToByte());
            b.AddRange(EMNO.SelectMany(x => x.ToByte()));
            b.AddRange(EMEF.SelectMany(x => x.ToByte()));
            return b;
        }

    }

    public class CRT
    {
        public EEN2c EEN2 = new();
        public GENTc GENT = new();
        public GCREc GCRE = new();
        public GCHRc GCHR = new();

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
        public EEN2c EEN2 = new();
        public GENTc GENT = new();
        public GITMc GITM = new();

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
        public EEN2c EEN2 = new();
        public GENTc GENT = new();
        public GITMc GITM = new();
        public GIARc GIAR = new();

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
        public EEN2c EEN2 = new();
        public GENTc GENT = new();

        public GITMc GITM = new();
        // Property GIAM As GIAMc

        // GIAM = New GIAMc()

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

    public enum USEType
    {
        USE,
        DOR,
        CON
    }
    
    public class USE // also encapsulates CON and DOR, which use static chunks that are not read from or written to, if/which chunk is used is controlled by GOBJ.
    {
        public EEN2c EEN2 = new();
        public GENTc GENT = new();
        public GOBJc GOBJ = new();
        
        public USE(USEType type = USEType.USE)
        {
            GOBJ.Type = (int)type;
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
                b.AddRange([0x47, 0x44, 0x4F, 0x52, 0x1, 0x0, 0x0, 0x0, 0x18, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0
                ]); // GDOR chunk (static in all Van Buren files)
            }
            else if (GOBJ.Type == 2)
            {
                b.AddRange([0x47, 0x43, 0x4F, 0x4E, 0x1, 0x0, 0x0, 0x0, 0x1A, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0
                ]); // GCON chunk (static in all Van Buren files)
            }
            return b;
        }

    }

    public class WEA
    {
        public EEN2c EEN2 = new();
        public GENTc GENT = new();

        public GITMc GITM = new();
        // Property GIWP As GIWPc

        // GIWP = New GIWPc()

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
        public EMTRc EMTR = new();

        public ExTRc ExTR = new();

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
        public string s1 = "";
        public string s2 = "";
        public string s3 = "";
        public Color col = Color.Black;
        public bool il = false;
        public int le = 0;

        public IEnumerable<byte> ToByte()
        {
            var s = 49 + s1.Length + s2.Length + s3.Length;
            var ret = new byte[s];
            ret.Write(0, "EMAP");
            ret.Write(4, il ? 0 : 5);
            ret.Write(8, s);
            ret.Write(16, s1.Length);
            ret.Write(18, s1);
            ret.Write(18 + s1.Length, s2.Length);
            ret.Write(20 + s1.Length, s2);
            ret.Write(20 + s1.Length + s2.Length, s3.Length);
            ret.Write(22 + s1.Length + s2.Length, s3);
            ret.Write(22 + s1.Length + s2.Length + s3.Length, le);
            ret.Write(24 + s1.Length + s2.Length + s3.Length, col.ToByte());
            ret.Write(32 + s1.Length + s2.Length + s3.Length, 1);
            return ret;
        }

    }

    public class EME2c
    {
        public string name = "";
        public Vector4 l = new(0f, 0f, 0f, 0f);
        public EEOVc EEOV = new();

        public IEnumerable<byte> ToByte()
        {
            var oEEOV = EEOV.ToByte();
            var ret = new byte[38 + name.Length + oEEOV.Count() + 1];
            ret.Write(0, "EME2");
            ret.Write(8, 39 + name.Length + oEEOV.Count());
            ret.Write(12, name.Length);
            ret.Write(14, name);
            ret.Write(14 + name.Length, l.ToByte());
            ret.Write(38 + name.Length, 1);
            ret.Write(39 + name.Length, oEEOV);
            return ret;
        }

    }

    public class EEOVc
    {
        public string s1 = "";
        public string s2 = "";
        public string s3 = "";
        public string s4 = "";
        public int ps4 = 0;
        public string s5 = "";
        public string[] inv = [];

        public IEnumerable<byte> ToByte()
        {
            var invl = inv.Sum(e => e.Length + 2);
            var a = ps4 == 2 ? 2 : 0;
            var ret = new byte[46 + s1.Length + s2.Length + s3.Length + s4.Length + s5.Length + invl + a + 1];
            ret.Write(0, "EEOV");
            if (inv.Any())
                ret.Write(4, 2);
            ret.Write(8, 47 + s1.Length + s2.Length + s3.Length + s4.Length + s5.Length + invl + a);
            ret.Write(12, s1.Length);
            ret.Write(14, s1);
            ret.Write(25 + s1.Length, s2.Length);
            ret.Write(27 + s1.Length, s2);
            ret.Write(27 + s1.Length + s2.Length, s3.Length);
            ret.Write(29 + s1.Length + s2.Length, s3);
            ret.Write(38 + s1.Length + s2.Length + s3.Length, s4.Length);
            ret.Write(40 + s1.Length + s2.Length + s3.Length, s4);
            ret.Write(40 + s1.Length + s2.Length + s3.Length + s4.Length, ps4);
            ret.Write(41 + s1.Length + s2.Length + s3.Length + s4.Length + a, s5.Length);
            ret.Write(43 + s1.Length + s2.Length + s3.Length + s4.Length + a, s5);
            ret.Write(43 + s1.Length + s2.Length + s3.Length + s4.Length + s5.Length + a, inv.Length);
            var i = 0;
            foreach (var e in inv)
            {
                ret.Write(47 + s1.Length + s2.Length + s3.Length + s4.Length + s5.Length + a + i, e.Length);
                ret.Write(49 + s1.Length + s2.Length + s3.Length + s4.Length + s5.Length + a + i, e);
                i += e.Length + 2;
            }
            return ret;
        }

    }

    public class EMEPc
    {
        public int index = 0;
        public Vector3 p = new(0f, 0f, 0f);
        public float r = 0f;

        public IEnumerable<byte> ToByte()
        {
            var ret = new byte[109];
            ret.Write(0, "EMEP");
            ret.Write(8, 109);
            ret.Write(12, index);
            ret.Write(73, p.ToByte());
            ret.Write(105, r);
            return ret;
        }

    }

    public class ECAMc
    {
        public Vector4 p = new(0f, 0f, 0f, 0f);

        public IEnumerable<byte> ToByte()
        {
            var ret = new byte[28];
            ret.Write(0, "ECAM");
            ret.Write(8, 28);
            ret.Write(12, p.ToByte());
            return ret;
        }

    }

    public class EMEFc
    {
        public string s1 = "";
        public string s2 = "";
        public Vector4 l = new(0f, 0f, 0f, 0f);
        public byte b = 0;

        public IEnumerable<byte> ToByte()
        {
            var ret = new byte[41 + s1.Length + s2.Length + 1];
            ret.Write(0, "EMEF");
            ret.Write(8, 42 + s1.Length + s2.Length);
            ret.Write(12, s1.Length);
            ret.Write(14, s1);
            ret.Write(14 + s1.Length, l.ToByte());
            ret.Write(38 + s1.Length, 1);
            ret.Write(39 + s1.Length, s2.Length);
            ret.Write(41 + s1.Length, s2);
            ret.Write(41 + s1.Length + s2.Length, b);
            return ret;
        }

    }

    public class EMSDc
    {
        public string s1 = "";
        public Vector3 l = new(0f, 0f, 0f);
        public string s2 = "";

        public IEnumerable<byte> ToByte()
        {
            var ret = new byte[29 + s1.Length + s2.Length + 1];
            ret.Write(0, "EMSD");
            ret.Write(8, 30 + s1.Length + s2.Length);
            ret.Write(12, s1.Length);
            ret.Write(14, s1);
            ret.Write(14 + s1.Length, l.ToByte());
            ret.Write(26 + s1.Length, s2.Length);
            ret.Write(28 + s1.Length, s2);
            ret.Write(28 + s1.Length + s2.Length, [1, 1]);
            return ret;
        }

    }

    public class EPTHc
    {
        public string name = "";
        public List<Vector4> p = [new(0f, 0f, 0f, 0f)];

        public IEnumerable<byte> ToByte()
        {
            var ret = new byte[17 + p.Count * 24 + name.Length + 1];
            ret.Write(0, "EPTH");
            ret.Write(8, 18 + p.Count * 24 + name.Length);
            ret.Write(12, name.Length);
            ret.Write(14, name);
            ret.Write(14 + name.Length, p.Count);
            var i = 0;
            foreach (var l in p)
            {
                ret.Write(18 + name.Length + i, l.ToByte());
                i += 24;
            }
            return ret;
        }

    }

    public class EMTRc
    {
        public int n = 0;
        public List<Vector3> r = [new(0f, 0f, 0f)];

        public IEnumerable<byte> ToByte()
        {
            var ret = new byte[19 + r.Count * 12 + 1];
            ret.Write(0, "EMTR");
            ret.Write(8, 20 + r.Count * 12);
            ret.Write(12, n);
            ret.Write(16, r.Count);
            var i = 0;
            foreach (var l in r)
            {
                ret.Write(20 + i, l.ToByte());
                i += 12;
            }
            return ret;
        }

    }

    public class ExTRc // Called ExTR instead of E(T/S/B)TR for easier handling within triggers
    {
        public string @type = "T"; // So we know which file is being created, T, S, or B. (or M, but it's ignored if that happens)
        public string s = "";

        public IEnumerable<byte> ToByte()
        {
            switch (@type ?? "")
            {
                case "B":
                    {
                        var ret = new byte[19];
                        ret.Write(0, "EBTR");
                        ret.Write(8, 19);
                        ret.Write(12, s);
                        ret.Write(16, "FFF");
                        return ret;
                    }
                case "S":
                    {
                        var ret = new byte[17 + s.Length + 1];
                        ret.Write(0, "ESTR");
                        ret.Write(8, 18 + s.Length);
                        ret.Write(12, s.Length);
                        ret.Write(14, s);
                        return ret;
                    }
                case "T":
                    {
                        var ret = new byte[15 + s.Length + 1];
                        ret.Write(0, "ETTR");
                        ret.Write(8, 16 + s.Length);
                        ret.Write(12, s.Length);
                        ret.Write(14, s);
                        ret.Write(14 + s.Length, [1, 1]);
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
        public string skl = "";
        public string invtex = "";
        public string acttex = "";
        public bool sel = false;
        public EEOVc EEOV = new();

        public IEnumerable<byte> ToByte()
        {
            var oEEOV = EEOV.ToByte();
            var ret = new byte[22 + oEEOV.Count() + skl.Length + invtex.Length + acttex.Length + 1];
            ret.Write(0, "EEN2");
            ret.Write(8, 23 + oEEOV.Count() + skl.Length + invtex.Length + acttex.Length);
            ret.Write(12, skl.Length);
            ret.Write(14, skl);
            ret.Write(14 + skl.Length, invtex.Length);
            ret.Write(16 + skl.Length, invtex);
            ret.Write(16 + skl.Length + invtex.Length, acttex.Length);
            ret.Write(18 + skl.Length + invtex.Length, acttex);
            ret.Write(19 + skl.Length + invtex.Length + acttex.Length, sel);
            ret.Write(23 + skl.Length + invtex.Length + acttex.Length, oEEOV);
            return ret;
        }

    }

    public class GENTc
    {
        public int HoverSR = 0; // String used when moused over
        public int LookSR = 0; // String used when "Look" option is used
        public int NameSR = 0; // String of the entities' name
        public int UnkwnSR = 0; // String used ???
        public int MaxHealth = 0;
        public int StartHealth = 0;

        public IEnumerable<byte> ToByte()
        {
            var ret = new byte[44];
            ret.Write(0, "GENT");
            ret.Write(4, 1);
            ret.Write(8, 44);
            ret.Write(12, HoverSR);
            ret.Write(16, LookSR);
            ret.Write(20, NameSR);
            ret.Write(24, UnkwnSR);
            ret.Write(36, MaxHealth);
            ret.Write(40, StartHealth);
            return ret;
        }

    }

    public class GCREc
    {
        public int[] Special = [0, 0, 0, 0, 0, 0, 0];
        public int Age = 0;
        public List<Skill> Skills = [];
        public List<int> Traits = [];
        public List<int> TagSkills = [];
        public string PortStr = "";
        public Socket Hea = new("", "");
        public Socket Hai = new("", "");
        public Socket Pon = new("", "");
        public Socket Mus = new("", "");
        public Socket Bea = new("", "");
        public Socket Eye = new("", "");
        public Socket Bod = new("", "");
        public Socket Han = new("", "");
        public Socket Fee = new("", "");
        public Socket Bac = new("", "");
        public Socket Sho = new("", "");
        public Socket Van = new("", "");
        public string[] Inventory = [];
        public List<GWAMc> GWAM = [];

        public IEnumerable<byte> ToByte()
        {
            var GWAMb = new List<byte>();
            foreach (var g in GWAM)
                GWAMb.AddRange(g.ToByte());
            var socs = new[] { Hea, Hai, Pon, Mus, Bea, Eye, Bod, Han, Fee, Bac, Sho, Van };
            var sock = new List<byte>();
            foreach (var s in socs)
                sock.AddRange(s.ToByte());
            var inv = new List<byte>();
            foreach (var i in Inventory)
            {
                inv.AddRange([(byte)i.Length, 0]);
                inv.AddRange(Encoding.ASCII.GetBytes(i));
            }

            var sl = Skills.Count * 8; // Skills length
            var tl = Traits.Count * 4; // Traits length
            var tsl = TagSkills.Count * 4; // Tag Skills length
            var il = Inventory.Sum(i => i.Length + 2); // Inventory Length
            var socl = Hea.Length + Hai.Length + Pon.Length + Mus.Length + Bea.Length + Eye.Length + Bod.Length + Han.Length + Fee.Length + Bac.Length + Sho.Length + Van.Length; // Socket String Length
            var TDL = sl + tl + tsl + il + GWAMb.Count + PortStr.Length + socl; // Total Dynamic Length

            var ret = new byte[276 + TDL + 1];
            ret.Write(0, "GCRE");
            ret.Write(4, 4);
            ret.Write(8, 277 + TDL);
            ret.Write(12, Special[0]);
            ret.Write(16, Special[1]);
            ret.Write(20, Special[2]);
            ret.Write(24, Special[3]);
            ret.Write(28, Special[4]);
            ret.Write(32, Special[5]);
            ret.Write(36, Special[6]);
            ret.Write(56, Age);
            ret.Write(72, Skills.Count);
            for (int i = 0; i < Skills.Count; i++)
                ret.Write(76 + i * 8, Skills[i].ToByte());
            ret.Write(80 + sl, Traits.Count);
            for (int i = 0; i < Traits.Count; i++)
                ret.Write(84 + sl + i * 4, Traits[i]);
            ret.Write(84 + sl + tl, TagSkills.Count);
            for (int i = 0; i < TagSkills.Count; i++)
                ret.Write(88 + sl + tl + i * 4, TagSkills[i]);
            ret.Write(92 + sl + tl + tsl, PortStr.Length);
            ret.Write(94 + sl + tl + tsl, PortStr);
            ret.Write(129 + sl + tl + tsl + PortStr.Length, sock);
            ret.Write(189 + sl + tl + tsl + PortStr.Length + socl, GWAM.Count);
            ret.Write(273 + sl + tl + tsl + PortStr.Length + socl, Inventory.Length);
            ret.Write(277 + sl + tl + tsl + PortStr.Length + socl, inv);
            ret.Write(277 + sl + tl + tsl + PortStr.Length + socl + il, GWAMb);
            return ret;
        }

    }

    public class GWAMc
    {
        public int Anim = 0;
        public int DmgType = 0;
        public int ShotsFired = 0;
        public int Range = 0;
        public int MinDmg = 0;
        public int MaxDmg = 0;
        public int AP = 0;
        public int NameSR = 0;
        public string VegName = 0.ToString();

        public IEnumerable<byte> ToByte()
        {
            var ret = new byte[79 + VegName.Length + 1];
            ret.Write(0, "GWAM");
            ret.Write(4, 5);
            ret.Write(8, 80 + VegName.Length);
            ret.Write(12, Anim);
            ret.Write(16, DmgType);
            ret.Write(20, ShotsFired);
            ret.Write(36, Range);
            ret.Write(48, MinDmg);
            ret.Write(52, MaxDmg);
            ret.Write(62, AP);
            ret.Write(72, NameSR);
            ret.Write(78, VegName);
            ret.Write(ret.Length - 2, 1);
            return ret;
        }

    }

    public class GCHRc
    {
        public string name = "";

        public IEnumerable<byte> ToByte()
        {
            var ret = new byte[13 + name.Length + 1];
            ret.Write(0, "GCHR");
            ret.Write(8, 14 + name.Length);
            ret.Write(12, name.Length);
            ret.Write(14, name);
            return ret;
        }

    }

    public class _2MWTc
    {
        public string mpf = "";
        public bool frozen;
        public bool dark;
        public List<_2MWTChunk> chunks = [];

        public IEnumerable<byte> ToByte()
        {
            if (chunks.Count == 0)
                return Array.Empty<byte>();
            var sl = chunks.Sum(x => x.tex.Length) + mpf.Length; // length of strings
            var wl = chunks.Count * 22; // added length for each water chunk

            var ret = new byte[157 + sl + wl + 1];
            ret.Write(0, "2MWT");
            ret.Write(8, 158 + sl + wl);
            ret.Write(12, mpf.Length);
            ret.Write(14, mpf);
            ret.Write(27 + mpf.Length, !dark);
            ret.Write(29 + mpf.Length, !frozen);
            ret.Write(154 + mpf.Length, chunks.Count);
            var io = 158 + mpf.Length;
            foreach (var w in chunks)
            {
                ret.Write(io, w.ToByte());
                io += 22 + w.tex.Length;
            }
            return ret;
        }

    }

    public class GITMc            // "Missing" Sockets:
    {
        public int @type = 0; // ? Hai
        public bool equip = false; // ? Pon
        public int eqslot = 0; // ? Mus
        public Socket Hea = new("", ""); // ? Bea
        public bool hHai = false;
        public bool hBea = false;
        public bool hMus = false;
        public bool hEye = false;
        public bool hPon = false;
        public bool hVan = false;
        public Socket Eye = new("", "");
        public Socket Bod = new("", "");
        public Socket Bac = new("", "");
        public Socket Han = new("", "");
        public Socket Fee = new("", "");
        public Socket Sho = new("", "");
        public Socket Van = new("", "");
        public Socket IHS = new("", "");
        public int reload = 0;

        public IEnumerable<byte> ToByte()
        {
            var socl = Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length + Fee.Length + Sho.Length + Van.Length + IHS.Length;
            var ret = new byte[95 + socl + 1];
            ret.Write(0, "GITM");
            ret.Write(4, 1);
            ret.Write(8, 96 + socl);
            ret.Write(12, @type);
            ret.Write(20, equip);
            ret.Write(24, eqslot);
            ret.Write(28, Hea.ToByte());
            ret.Write(32 + Hea.Length, hHai);
            ret.Write(33 + Hea.Length, hBea);
            ret.Write(34 + Hea.Length, hMus);
            ret.Write(35 + Hea.Length, hEye);
            ret.Write(36 + Hea.Length, hPon);
            ret.Write(37 + Hea.Length, hVan);
            ret.Write(41 + Hea.Length, 1);
            ret.Write(46 + Hea.Length, 1);
            ret.Write(51 + Hea.Length, 1);
            ret.Write(52 + Hea.Length, Eye.ToByte());
            ret.Write(56 + Hea.Length + Eye.Length, 1);
            ret.Write(61 + Hea.Length + Eye.Length, 1);
            ret.Write(62 + Hea.Length + Eye.Length, Bod.ToByte());
            ret.Write(66 + Hea.Length + Eye.Length + Bod.Length, Bac.ToByte());
            ret.Write(70 + Hea.Length + Eye.Length + Bod.Length + Bac.Length, Han.ToByte());
            ret.Write(75 + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length, Fee.ToByte());
            ret.Write(79 + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length + Fee.Length, Sho.ToByte());
            ret.Write(83 + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length + Fee.Length + Sho.Length, Van.ToByte());
            ret.Write(87 + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length + Fee.Length + Sho.Length + Van.Length, 1);
            ret.Write(88 + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length + Fee.Length + Sho.Length + Van.Length, IHS.ToByte());
            ret.Write(92 + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length + Fee.Length + Sho.Length + Van.Length + IHS.Length, reload);
            return ret;
        }

    }

    public class GIARc
    {
        public int BallR = 0;
        public int BioR = 0;
        public int ElecR = 0;
        public int EMPR = 0;
        public int NormR = 0;
        public int HeatR = 0;

        public IEnumerable<byte> ToByte()
        {
            var ret = new byte[40];
            ret.Write(0, "GIAR");
            ret.Write(8, 40);
            ret.Write(12, BallR);
            ret.Write(16, BioR);
            ret.Write(20, ElecR);
            ret.Write(24, EMPR);
            ret.Write(28, NormR);
            ret.Write(32, HeatR);
            return ret;
        }

    }

    public class GOBJc
    {
        public int Type = 0;

        public IEnumerable<byte> ToByte()
        {
            var ret = new byte[40];
            ret.Write(0, "GOBJ");
            ret.Write(8, 40);
            ret.Write(12, Type);
            return ret;
        }

    }

    public class EMNOc
    {
        public Vector2 l = new(0f, 0f);
        public string tex = "";
        public int sr = 0;

        public IEnumerable<byte> ToByte()
        {
            var ret = new byte[29 + tex.Length + 1];
            ret.Write(0, "EMNO");
            ret.Write(8, 30 + tex.Length);
            ret.Write(12, l.X);
            ret.Write(20, l.Y);
            ret.Write(24, tex.Length);
            ret.Write(26, tex);
            ret.Write(26 + tex.Length, sr);
            return ret;
        }

    }

    public class EMFGc
    {
        public bool enabled = false;
        public Color colour = Color.Black;
        public float base_height = 0f;
        public float anim1Speed = 0f;
        public float anim1Height = 0f;
        public float total_height = 0f;
        public float anim2Speed = 0f;
        public float anim2Height = 0f;
        public float verticalOffset = 0f;
        public float max_fog_density = 0f;
        
        // implement == operator
        private bool _isNull()
        {
            return !enabled &&
                   base_height == 0f &&
                   anim1Speed == 0f &&
                   anim1Height == 0f &&
                   total_height == 0f &&
                   anim2Speed == 0f &&
                   anim2Height == 0f &&
                   verticalOffset == 0f &&
                   max_fog_density == 0f;
        }
        
        public IEnumerable<byte> ToByte()
        {
            // return empty array if blank (editor adds chunk by default)
            if (_isNull())
                return [];
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
            var ret = new byte[72];
            ret.Write(0, "EMFG");
            ret.Write(8, 72);
            ret.Write(12, enabled);
            ret.Write(13, colour.ToByte());
            ret.Write(16, base_height);
            ret.Write(20, anim1Speed);
            ret.Write(24, anim1Height);
            ret.Write(28, total_height);
            ret.Write(32, anim2Speed);
            ret.Write(36, anim2Height);
            ret.Write(40, verticalOffset);
            ret.Write(44, max_fog_density);
            return ret;
        }

    }

    public class EMNPChunk
    {
        public byte @bool = 0;
        public Point3 l = new(0f, 0f, 0f);
        public byte b1 = 0;
        public byte b2 = 0;
        public byte b3 = 0;
        public byte b4 = 0;
        public byte b5 = 0;

        public IEnumerable<byte> ToByte()
        {
            var ret = new byte[30];
            ret.Write(0, @bool);
            ret.Write(1, l.ToByte());
            ret.Write(25, b1);
            ret.Write(26, b2);
            ret.Write(27, b3);
            ret.Write(28, b4);
            ret.Write(29, b5);
            return ret;
        }

    }

    public class EMNPc
    {
        public List<EMNPChunk> chunks = [];

        public IEnumerable<byte> ToByte()
        {
            var ret = new byte[15 + chunks.Count * 30 + 1];
            ret.Write(0, "EMNP");
            ret.Write(8, 16 + chunks.Count * 30);
            ret.Write(12, chunks.Count);
            for (int i = 0, loopTo = chunks.Count - 1; i <= loopTo; i++)
                ret.Write(16 + i * 30, chunks[i].ToByte());
            return default;
        }

    }

    #endregion

    #region Other Classes

    public class Point2
    {
        public float x;
        public float y;

        public Point2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public IEnumerable<byte> ToByte()
        {
            var ret = new byte[8];
            ret.Write(0, x);
            ret.Write(4, y);
            return ret;
        }

    }

    public class Point3
    {
        public float x;
        public float z;
        public float y;

        public Point3(float x, float z, float y)
        {
            this.x = x;
            this.z = z;
            this.y = y;
        }

        public IEnumerable<byte> ToByte()
        {
            var ret = new byte[12];
            ret.Write(0, x);
            ret.Write(4, z);
            ret.Write(8, y);
            return ret;
        }

    }
    
    public class Skill
    {
        public int Index;
        public int Value;
        public Skill(int Index, int Value)
        {
            this.Index = Index;
            this.Value = Value;
        }

        public IEnumerable<byte> ToByte()
        {
            var ret = new byte[8];
            ret.Write(0, Index);
            ret.Write(4, Value);
            return ret;
        }

    }

    public class Socket
    {
        public string Model;
        public string Tex;

        /// <summary>
        /// Gets the length of both strings
        /// </summary>
        public int Length => Model.Length + Tex.Length;

        public Socket(string Model, string Tex)
        {
            this.Model = Model;
            this.Tex = Tex;
        }

        public IEnumerable<byte> ToByte()
        {
            var ret = new byte[3 + Model.Length + Tex.Length + 1];
            ret.Write(0, Model.Length);
            ret.Write(2, Model);
            ret.Write(2 + Model.Length, Tex.Length);
            ret.Write(4 + Model.Length, Tex);
            return ret;
        }

    }

    public class _2MWTChunk
    {
        public string tex;
        public Vector3 loc;
        public Vector2 texloc;

        public _2MWTChunk(string tex = "", Vector3 loc = new(), Vector2 texloc = new())
        {
            this.tex = tex;
            this.loc = loc;
            this.texloc = texloc;
        }

        public IEnumerable<byte> ToByte()
        {
            var ret = new byte[21 + tex.Length + 1];
            ret.Write(0, loc.ToByte());
            ret.Write(12, tex.Length);
            ret.Write(14, tex);
            ret.Write(14 + tex.Length, texloc.ToByte());
            return ret;
        }

    }
}

#endregion
