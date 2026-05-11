#region Filetype Classes

// Classes containing the headers used by that file type, in the order they should be put back into a new file.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace VBLauncher;

public class Map
{
    public _2MWTc _2MWT = null;
    public ECAMc ECAM = null;
    public EMAPc EMAP = new();
    public List<EME2c> EME2 = [];
    public List<EMEFc> EMEF = [];
    public List<EMEPc> EMEP = [];
    public EMFGc EMFG = null;
    public List<EMNOc> EMNO = [];
    public EMNPc EMNP = null;
    public List<EMSDc> EMSD = [];
    public List<EPTHc> EPTH = [];
    public List<Trigger> Triggers = [];

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
        b.AddRange(EMNP.ToByte());
        //b.AddRange([0x45, 0x4D, 0x4E, 0x50, 0x0, 0x0, 0x0, 0x0, 0x10, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0]); // EMNP Chunk
        if (EMFG is not null)
            b.AddRange(EMFG.ToByte());
        b.AddRange(EMNO.SelectMany(x => x.ToByte()));
        b.AddRange(EMEF.SelectMany(x => x.ToByte()));
        return b;
    }
}

public class Entity
{
    public EEN2c EEN2 = new();
    public GENTc GENT = new();
}

public class CRT : Entity
{
    public GCHRc GCHR = new();
    public GCREc GCRE = new();

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

public class ITM : Entity
{
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

public class ARM : ITM
{
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

public class AMO : ITM
{
    public GIAMc GIAM = new();

    public IEnumerable<byte> ToByte()
    {
        var b = new List<byte>();
        b.AddRange(EEN2.ToByte());
        b.AddRange(GENT.ToByte());
        b.AddRange(GITM.ToByte());
        b.AddRange(GIAM.ToByte());
        return b;
    }
}

public enum USEType
{
    USE,
    DOR,
    CON
}

public class USE : Entity
{
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
        switch ((USEType)GOBJ.Type)
        {
            case USEType.USE:
                break;
            case USEType.DOR:
                b.AddRange([
                    0x47, 0x44, 0x4F, 0x52, 0x1, 0x0, 0x0, 0x0, 0x18, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,
                    0x0, 0x0, 0x0, 0x0
                ]); // GDOR chunk (static in all Van Buren files)
                break;
            case USEType.CON:
                b.AddRange([
                    0x47, 0x43, 0x4F, 0x4E, 0x1, 0x0, 0x0, 0x0, 0x1A, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,
                    0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0
                ]); // GCON chunk (static in all Van Buren files)
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return b;
    }
}

public class WEA : ITM
{
    public GIWPc GIWP = new();

    public IEnumerable<byte> ToByte()
    {
        var b = new List<byte>();
        b.AddRange(EEN2.ToByte());
        b.AddRange(GENT.ToByte());
        b.AddRange(GITM.ToByte());
        b.AddRange(GIWP.ToByte());
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
    public Color col = Color.Black;
    public bool il = false;
    public int le = 0;
    public string s1 = "";
    public string s2 = "";
    public string s3 = "";

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
    public EEOVc EEOV = new();
    public Vector4 l = new(0f, 0f, 0f, 0f);
    public string name = "";

    // Bytes between `l` (ends at offset 30+name) and the EEOV chunk start. Up to 8 in
    // typical files. Captured raw because we suspect a per-instance orientation flag
    // we don't currently parse lives in here (B3D and CRT signals are identical
    // between broken and correctly-rotated entities).
    public byte[] PostLGap = [];

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
    public string[] inv = [];
    public int ps4 = 0;
    public string s1 = "";
    public string s2 = "";
    public string s3 = "";
    public string s4 = "";
    public string s5 = "";

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
    public byte b = 0;
    public Vector4 l = new(0f, 0f, 0f, 0f);
    public string s1 = "";
    public string s2 = "";

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
    public Vector3 l = new(0f, 0f, 0f);
    public string s1 = "";
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
    public string s = "";

    public string
        type = "T"; // So we know which file is being created, T, S, or B. (or M, but it's ignored if that happens)

    public IEnumerable<byte> ToByte()
    {
        switch (type ?? "")
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
    public string acttex = "";
    public EEOVc EEOV = new();
    public string invtex = "";
    public bool sel = false;
    public string skl = "";

    // 4 bytes between `sel` (offset 19+lens) and the EEOV chunk start (offset 23+lens)
    // that the existing reader skips. Captured raw so we can correlate against
    // mis-rotated entities — the user's hypothesis is that an orientation flag we
    // don't currently parse lives in here.
    public byte[] PostSelGap = new byte[4];

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
    public int MaxHealth = 0;
    public int NameSR = 0; // String of the entities' name
    public int StartHealth = 0;
    public int UnkwnSR = 0; // String used ???

    // Bytes 28..35 in the GENT chunk that the reader skips (jumps from UnkwnSR@24
    // straight to MaxHealth@36). 8 raw bytes — could be flags or a small struct.
    public byte[] Gap28to35 = new byte[8];

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
    public int Age = 0;
    public Socket Bac = new("", "");
    public Socket Bea = new("", "");
    public Socket Bod = new("", "");
    public Socket Eye = new("", "");
    public Socket Fee = new("", "");
    public List<GWAMc> GWAM = [];
    public Socket Hai = new("", "");
    public Socket Han = new("", "");
    public Socket Hea = new("", "");
    public string[] Inventory = [];
    public Socket Mus = new("", "");
    public Socket Pon = new("", "");
    public string PortStr = "";
    public Socket Sho = new("", "");
    public List<Skill> Skills = [];
    public int[] Special = [0, 0, 0, 0, 0, 0, 0];
    public List<int> TagSkills = [];
    public List<int> Traits = [];
    public Socket Van = new("", "");

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
        var socl = Hea.Length + Hai.Length + Pon.Length + Mus.Length + Bea.Length + Eye.Length + Bod.Length +
                   Han.Length + Fee.Length + Bac.Length + Sho.Length + Van.Length; // Socket String Length
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
        for (var i = 0; i < Skills.Count; i++)
            ret.Write(76 + i * 8, Skills[i].ToByte());
        ret.Write(80 + sl, Traits.Count);
        for (var i = 0; i < Traits.Count; i++)
            ret.Write(84 + sl + i * 4, Traits[i]);
        ret.Write(84 + sl + tl, TagSkills.Count);
        for (var i = 0; i < TagSkills.Count; i++)
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
    public int Version = 5;

    public int Anim;
    public int DmgType;
    public int ShotsFired;
    public int F3;
    public int F4;
    public int F5;
    public int Range;
    public int F7;
    public int F8;
    public int MinDmg;
    public int MaxDmg;
    public byte Bool0;

    public int F12;
    public byte Bool1;
    public int AP;
    public byte Consumed;
    public byte Thrown;

    public int PoseOverride;

    public int NameSR;

    public string VegName = "";

    public byte Byte31 = 1;
    public byte Byte32;

    public IEnumerable<byte> ToByte()
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);

        bw.Write(Anim);
        bw.Write(DmgType);
        bw.Write(ShotsFired);
        bw.Write(F3);
        bw.Write(F4);
        bw.Write(F5);
        bw.Write(Range);
        bw.Write(F7);
        bw.Write(F8);
        bw.Write(MinDmg);
        bw.Write(MaxDmg);
        bw.Write(Bool0);

        if (Version != 0)
        {
            bw.Write(F12);
            bw.Write(Bool1);
            bw.Write(AP);
            bw.Write(Consumed);
            bw.Write(Thrown);
        }

        if (Version >= 2) bw.Write(PoseOverride);
        if (Version >= 3) bw.Write(NameSR);
        if (Version >= 4)
        {
            var nb = Encoding.ASCII.GetBytes(VegName);
            bw.Write((ushort)nb.Length);
            bw.Write(nb);
        }
        if (Version >= 5)
        {
            bw.Write(Byte31);
            bw.Write(Byte32);
        }

        var body = ms.ToArray();
        var total = 12 + body.Length;
        var ret = new byte[total];
        ret.Write(0, "GWAM");
        ret.Write(4, Version);
        ret.Write(8, total);
        ret.Write(12, body);
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
    public List<_2MWTChunk> chunks = [];
    public bool dark;
    public bool frozen;
    public string mpf = "";

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

public class GITMc // "Missing" Sockets:
{
    public Socket Bac = new("", "");
    public Socket Bod = new("", "");
    public int eqslot = 0; // ? Mus
    public bool equip = false; // ? Pon
    public Socket Eye = new("", "");
    public Socket Fee = new("", "");
    public Socket Han = new("", "");
    public bool hBea = false;
    public Socket Hea = new("", ""); // ? Bea
    public bool hEye = false;
    public bool hHai = false;
    public bool hMus = false;
    public bool hPon = false;
    public bool hVan = false;
    public Socket IHS = new("", "");
    public int reload = 0;
    public Socket Sho = new("", "");
    public int type = 0; // ? Hai
    public Socket Van = new("", "");

    public IEnumerable<byte> ToByte()
    {
        var socl = Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length + Fee.Length + Sho.Length +
                   Van.Length + IHS.Length;
        var ret = new byte[95 + socl + 1];
        ret.Write(0, "GITM");
        ret.Write(4, 1);
        ret.Write(8, 96 + socl);
        ret.Write(12, type);
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
        ret.Write(83 + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length + Fee.Length + Sho.Length,
            Van.ToByte());
        ret.Write(
            87 + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length + Fee.Length + Sho.Length + Van.Length,
            1);
        ret.Write(
            88 + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length + Fee.Length + Sho.Length + Van.Length,
            IHS.ToByte());
        ret.Write(
            92 + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length + Fee.Length + Sho.Length + Van.Length +
            IHS.Length, reload);
        return ret;
    }
}

public class GIARc
{
    public int BallR = 0;
    public int BioR = 0;
    public int ElecR = 0;
    public int EMPR = 0;
    public int HeatR = 0;
    public int NormR = 0;

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
    public int Type;

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
    public int sr = 0;
    public string tex = "";

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
    public float anim1Height = 0f;
    public float anim1Speed = 0f;
    public float anim2Height = 0f;
    public float anim2Speed = 0f;
    public float base_height = 0f;
    public Color colour = Color.Black;
    public bool enabled = false;
    public float max_fog_density = 0f;
    public float total_height = 0f;
    public float verticalOffset = 0f;

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
        if (base_height <= 0f) throw new Exception("base_height <= 0.0f!");
        if (base_height > total_height) throw new Exception("base_height > total_height!");
        if (total_height <= 0f) throw new Exception("total_height <= 0.0f!");
        if ((max_fog_density <= 0f) | (max_fog_density > 1f)) throw new Exception("invalid max_fog_density");
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
    public byte b1 = 0;
    public byte b2 = 0;
    public byte b3 = 0;
    public byte b4 = 0;
    public byte b5 = 0;
    public byte @bool = 0;
    public Vector3 l = new(0f, 0f, 0f);

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
        return ret;
    }
}

#endregion

#region Other Classes

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

    public Socket(string Model, string Tex)
    {
        this.Model = Model;
        this.Tex = Tex;
    }

    /// <summary>
    ///     Gets the length of both strings
    /// </summary>
    public int Length => Model.Length + Tex.Length;

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
    public Vector3 loc;
    public string tex;
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

// GIAM (ammo) — 9 dwords, version 2 in shipping data.
// Field semantics named from F3.exe reads in sub_408160 (ammo body parser) and the
// combat math in GameWeapon::CalculateDamage / CalculateRangedToHit / GetEffectiveRange.
// Disk version gating: payload+0..+16 always; +20/+24 if v>=1; +28/+32 if v>=2.
public class GIAMc
{
    public int Version = 2;

    public int Caliber = 0;          // ammo[+236]; matches weapon's PrimaryCaliber/SecondaryCaliber for the load-check.
    public int AmmoExtra238 = 0;     // ammo[+238]; written by parser, read in destructor/copy paths but not in combat math.
    public int DamageBonus = 0;      // ammo[+239]; CalculateDamage: weapon_damage += DamageBonus.
    public int DRPenalty = 0;        // ammo[+240]; CalculateDamage: target_DR is reduced by this (negative = adds DR like buckshot).
    public int SkillBonus = 0;       // ammo[+241]; CalculateRangedToHit/CalculateMeleeToHit: skill += SkillBonus.

    // version >= 1
    public int DamageType = 0;       // ammo[+243]; energy ammo all = 5 (matches GWAM.DmgType=5 for energy weapons).
    public int AmmoCategory = 0;     // ammo[+244]; Small Energy Cell=3, Micro Fusion=2, Naphate=1 — energy-cell grouping.

    // version >= 2
    public int AmmoModifier = 0;     // ammo[+237]; paired with Caliber for ammo lookup. ReloadPrimaryAmmo: weapon[+238] := ammo[+237].
    public int RangeBonus = 0;       // ammo[+242]; GetEffectiveRange: range = weapon_base_range + RangeBonus. (Was missing entirely from prior tool.)

    public IEnumerable<byte> ToByte()
    {
        var payloadSize = 5 * 4;
        if (Version >= 1) payloadSize += 8;
        if (Version >= 2) payloadSize += 8;
        var total = 12 + payloadSize;
        var ret = new byte[total];
        ret.Write(0, "GIAM");
        ret.Write(4, Version);
        ret.Write(8, total);
        ret.Write(12, Caliber);
        ret.Write(16, AmmoExtra238);
        ret.Write(20, DamageBonus);
        ret.Write(24, DRPenalty);
        ret.Write(28, SkillBonus);
        if (Version >= 1)
        {
            ret.Write(32, DamageType);
            ret.Write(36, AmmoCategory);
        }
        if (Version >= 2)
        {
            ret.Write(40, AmmoModifier);
            ret.Write(44, RangeBonus);
        }
        return ret;
    }
}

// GIWP fields named from IDA cross-references against F3.exe (GameWeapon::* methods).
// WeaponClass enum: 1..5 = small arms (CalculateRangedToHit path, skill 0),
// 6..8 = melee (CalculateMeleeToHit path, skill 1), 9..10 = big guns. Thrown weapons
// override via the Thrown bool on each AttackMode. 0 in 40/51 shipped files = unset (test data).
public class GIWPc
{
    public int Version = 1;

    public int WeaponClass;
    public int PrimaryCaliber = -1;
    public int PrimaryReloadDuration;
    public int SecondaryCaliber = -1;
    public int SecondaryReloadDuration;
    public string ResourceName = "";  // Always empty in shipped data; std::string slot present in disk format.
    public int Unknown250;            // Constructor default 1; copied across saves; no gameplay reader found.
    public int PrimaryAmmoLoaded;
    public int PrimaryAmmoCapacity;
    public int SecondaryAmmoLoaded;
    public int SecondaryAmmoCapacity;
    public int Unknown255;            // 0 in 50/51 files; one weapon has 50.
    public int Unknown256;            // Always 0 across all 51 shipped weapons; reserved.

    public int PrimaryModifier;
    public int SecondaryModifier;

    public List<GWAMc> AttackModes = [];

    public IEnumerable<byte> ToByte()
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);

        bw.Write(WeaponClass);
        bw.Write(PrimaryCaliber);
        bw.Write(PrimaryReloadDuration);
        bw.Write(SecondaryCaliber);
        bw.Write(SecondaryReloadDuration);

        var nb = Encoding.ASCII.GetBytes(ResourceName);
        bw.Write((ushort)nb.Length);
        bw.Write(nb);

        bw.Write(Unknown250);
        bw.Write(PrimaryAmmoLoaded);
        bw.Write(PrimaryAmmoCapacity);
        bw.Write(SecondaryAmmoLoaded);
        bw.Write(SecondaryAmmoCapacity);
        bw.Write(Unknown255);
        bw.Write(Unknown256);
        bw.Write(AttackModes.Count);

        if (Version != 0)
        {
            bw.Write(PrimaryModifier);
            bw.Write(SecondaryModifier);
        }

        foreach (var am in AttackModes)
        {
            am.Version = Version;
            bw.Write(am.ToByte().ToArray());
        }

        var body = ms.ToArray();
        var total = 12 + body.Length;
        var ret = new byte[total];
        ret.Write(0, "GIWP");
        ret.Write(4, Version);
        ret.Write(8, total);
        ret.Write(12, body);
        return ret;
    }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class VEG
{
    public string Text = "";

    public byte[] Compile()
    {
        // Parse the Text form. We'll assume we have it as a single string `Text`.
        // Steps:
        // 1. Extract lines inside the "VEG { ... }" block.
        // 2. Parse each line of the form: "<type> <name> = <value>;"
        // 3. Convert them into binary.

        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms, Encoding.UTF8);

        // Write the "VEG v1.1" header (8 bytes)
        var vegHeader = "VEG V1.1";
        bw.Write(Encoding.ASCII.GetBytes(vegHeader));

        // Extract lines from Text
        var lines = Text.Replace("\r", "").Split('\n');
        // Find the block starting line "VEG {" and ending line "}";
        var startIndex = Array.FindIndex(lines, l => l.Trim() == "VEG {");
        var endIndex = Array.LastIndexOf(lines, "\t};");

        if (startIndex < 0 || endIndex < 0 || endIndex <= startIndex)
            throw new Exception("Invalid VEG format");

        var propertyLines = lines
            .Skip(startIndex + 1)
            .Take(endIndex - startIndex)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToList();

        // Split properties into chunks based on VFX delimiters
        var vfxChunks = new List<List<string>>();
        List<string> currentChunk = new();

        foreach (var line in propertyLines)
        {
            if (line.StartsWith("VFX {"))
            {
                if (currentChunk.Count > 0) vfxChunks.Add(currentChunk);
                currentChunk = new List<string>();
            }
            currentChunk.Add(line);
        }

        if (currentChunk.Count > 0) vfxChunks.Add(currentChunk);

        var vfx_count = vfxChunks.Count;

        // Write vfx_count (int32) at offset 8
        bw.Write(vfx_count);

        // Write 12 unknown bytes as 0
        bw.Write(new byte[12]);

        void ProcessProperties(List<string> props)
        {
            foreach (var line in props) // Skip "VFX {" and "}" lines
            {
                // Parse the line
                var trimmedLine = line.EndsWith(";") ? line.Substring(0, line.Length - 1) : line;

                var eqParts = trimmedLine.Split('=');
                if (eqParts.Length != 2) continue;
                var leftPart = eqParts[0].Trim();
                var rightPart = eqParts[1].Trim();

                var leftTokens = leftPart.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                if (leftTokens.Length < 2) throw new Exception("Invalid property definition: " + line);

                var typeToken = leftTokens[0];
                string name;
                var fullType = typeToken;

                if (enums.Contains(typeToken))
                {
                    name = leftTokens[1];
                    fullType = "enum " + typeToken;
                }
                else
                {
                    var secondTokenParts = leftTokens[1].Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                    name = secondTokenParts[^1];
                }

                bw.Write((short)name.Length);
                bw.Write(Encoding.ASCII.GetBytes(name));

                if (fullType == "enum VFX_Target") fullType = "VFX_Target";

                bw.Write((short)fullType.Length);
                bw.Write(Encoding.ASCII.GetBytes(fullType));

                bw.Write(0xFFFFFFFF);

                switch (typeToken)
                {
                    case "int":
                    case "VFX_TriggeredEffect":
                        bw.Write(4);
                        bw.Write(int.Parse(rightPart));
                        break;
                    case "float":
                        bw.Write(4);
                        bw.Write(float.Parse(rightPart.TrimEnd('f')));
                        break;
                    case "bool":
                        bw.Write(1);
                        bw.Write(rightPart == "true" ? (byte)0xDC : (byte)0); // idk why bool is 0xDC
                        break;
                    case "VFX_Byte":
                        bw.Write(1);
                        bw.Write(byte.Parse(rightPart));
                        break;
                    case "VFX_Vector":
                    case "VFX_Rotation":
                        bw.Write(12);
                        var vecParts = rightPart.Trim('(', ')').Split(',');
                        bw.Write(float.Parse(vecParts[0].Trim().TrimEnd('f')));
                        bw.Write(float.Parse(vecParts[1].Trim().TrimEnd('f')));
                        bw.Write(float.Parse(vecParts[2].Trim().TrimEnd('f')));
                        break;
                    case "VFX_Vector2":
                        bw.Write(8);
                        var v2 = rightPart.Trim('(', ')').Split(',');
                        bw.Write(float.Parse(v2[0].Trim().TrimEnd('f')));
                        bw.Write(float.Parse(v2[1].Trim().TrimEnd('f')));
                        break;
                    case "VFX_TextureUVs":
                        bw.Write(16);
                        var uv = rightPart.Trim('(', ')').Split(',');
                        bw.Write(float.Parse(uv[0].Trim().TrimEnd('f')));
                        bw.Write(float.Parse(uv[1].Trim().TrimEnd('f')));
                        bw.Write(float.Parse(uv[2].Trim().TrimEnd('f')));
                        bw.Write(float.Parse(uv[3].Trim().TrimEnd('f')));
                        break;
                    case "VFX_Color":
                        bw.Write(3);
                        var col = rightPart.Trim('(', ')').Split(',');
                        bw.Write(byte.Parse(col[0].Trim()));
                        bw.Write(byte.Parse(col[1].Trim()));
                        bw.Write(byte.Parse(col[2].Trim()));
                        break;
                    case "VFX_Resource":
                        var openParen = rightPart.IndexOf('(');
                        var closeParen = rightPart.LastIndexOf(')');
                        var resTypeName = rightPart.Substring(0, openParen).Trim();
                        var fileNameQuoted = rightPart.Substring(openParen + 1, closeParen - openParen - 1).Trim();

                        if (!fileNameQuoted.StartsWith("\"") || !fileNameQuoted.EndsWith("\""))
                            throw new Exception("Resource filename not quoted properly: " + rightPart);

                        var filenameInner = fileNameQuoted.Substring(1, fileNameQuoted.Length - 2);

                        bw.Write(4 + resTypeName.Length + filenameInner.Length);

                        bw.Write((short)resTypeName.Length);
                        bw.Write(Encoding.ASCII.GetBytes(resTypeName));

                        bw.Write((short)filenameInner.Length);
                        bw.Write(Encoding.ASCII.GetBytes(filenameInner));

                        break;
                    default:
                        if (enums.Contains(typeToken))
                        {
                            bw.Write(2 + rightPart.Length);
                            bw.Write((short)rightPart.Length);
                            bw.Write(Encoding.ASCII.GetBytes(rightPart));
                        }
                        else
                        {
                            var strVal = rightPart.Substring(1, rightPart.Length - 2);
                            bw.Write(2 + strVal.Length);
                            bw.Write((short)strVal.Length);
                            bw.Write(Encoding.ASCII.GetBytes(strVal));
                        }
                        break;
                }
            }
        }

        // Process each chunk
        foreach (var chunk in vfxChunks)
        {
            // Write "VFX v1.0" at offset 24
            var vfxHeader = "VFX V1.0";
            bw.Write(Encoding.ASCII.GetBytes(vfxHeader));

            // Write 8 unknown bytes after "VFX v1.0"
            bw.Write(new byte[8]);
            // Write the number of properties in the chunk (4 bytes)
            var propertyCount = chunk.Count - 2; // Exclude "VFX {" and "}" lines
            bw.Write(propertyCount);

            ProcessProperties(chunk.Skip(1).Take(chunk.Count - 2).ToList());
        }

        // get number of lines after the last VFX block (excluding the last "};")
        var rootValCount = lines.Length - vfxChunks.Sum(c => c.Count) - 3;
        bw.Write(rootValCount);
        var rootLines = lines.Skip(endIndex + 1).Take(rootValCount).ToList();

        ProcessProperties(rootLines);

        return ms.ToArray();
    }

    public VEG(byte[] b)
    {
        if (b.Length == 0) return;
        var ms = new MemoryStream();
        ms.Write(b, 0, b.Length);
        var br = new BinaryReader(ms);

        var sb = new StringBuilder();

        br.BaseStream.Seek(8, SeekOrigin.Begin); // skip veg header
        var vfx_count = br.ReadInt32();

        sb.AppendLine("VEG {");

        br.BaseStream.Seek(24, SeekOrigin.Begin); // skip unknown bytes

        string ReadValue()
        {
            var namel = br.ReadInt16();
            var name = new string(br.ReadChars(namel));
            var typel = br.ReadInt16();
            var typename = new string(br.ReadChars(typel));
            br.BaseStream.Seek(8, SeekOrigin.Current);

            var enum_name = "";
            if (typename.StartsWith("enum"))
            {
                enum_name = typename[5..];
                typename = "enum";
            }

            // handle like an enum
            if (typename == "VFX_Target")
            {
                enum_name = typename;
                typename = "enum";
            }

            var val = "";
            switch (typename)
            {
                case "VFX_TriggeredEffect":
                case "int":
                    val = br.ReadInt32().ToString();
                    break;
                case "VFX_Byte":
                    val = br.ReadByte().ToString();
                    break;
                case "float":
                    val = br.ReadSingle().ToString() + 'f';
                    break;
                case "bool":
                    val = br.ReadBoolean().ToString().ToLower();
                    break;
                case "VFX_Rotation":
                case "VFX_Vector":
                    val = $"({br.ReadSingle()}f, {br.ReadSingle()}f, {br.ReadSingle()}f)";
                    break;
                case "VFX_Vector2":
                    val = $"({br.ReadSingle()}f, {br.ReadSingle()}f)";
                    break;
                case "VFX_Color":
                    val = $"({br.ReadByte()}, {br.ReadByte()}, {br.ReadByte()})";
                    break;
                case "VFX_Resource":
                    var r_l = br.ReadInt16();
                    var res_type = new string(br.ReadChars(r_l));
                    var f_l = br.ReadInt16();
                    var filename = new string(br.ReadChars(f_l));
                    val = $"{res_type}(\"{filename}\")";
                    break;
                case "VFX_TextureUVs":
                    val = $"({br.ReadSingle()}f, {br.ReadSingle()}f, {br.ReadSingle()}f, {br.ReadSingle()}f)";
                    break;
                case "enum":
                    var l_e = br.ReadInt16();
                    val = $"{new string(br.ReadChars(l_e))}";
                    break;
                default:
                    var l = br.ReadInt16();
                    val = $"\"{new string(br.ReadChars(l))}\"";
                    break;
            }

            typename = enum_name == "" ? typename : enum_name;
            return $"\t{typename} {name} = {val};";
        }

        for (var i = 0; i < vfx_count; i++)
        {
            sb.AppendLine("\tVFX {");
            br.BaseStream.Seek(16, SeekOrigin.Current); // skip vfx header and unknown bytes
            var val_count = br.ReadInt32();
            for (var j = 0; j < val_count; j++)
            {
                sb.AppendLine("\t" + ReadValue());
            }
            sb.AppendLine("\t};");
        }
        var root_val_count = br.ReadInt32();
        for (var j = 0; j < root_val_count; j++)
        {
            sb.AppendLine(ReadValue());
        }

        sb.AppendLine("};");
        br.Close();
        ms.Close();
        Text = sb.ToString();
    }

    public static string[] enums =
    [
        "VFX_EffectType",
        "GFX_BONE_ID",
        "GFX_RENDER_PHASE",
        "VFX_WaveType",
        "VFX_AlignmentType",
        "VFX_EmitterShape",
        "VFX_CollisionShape",
        "VFX_CollisionType",
        "VFX_ParticleTrailType",
        "VFX_WaterMotionType",
        "VFX_LightningWidthStyle",
        "VFX_LightType",
        "VFX_ShadowVolumeType",
        "VFX_AABBType",
        "VFX_WindType",
        "VFX_Target"
    ];

    public static string[] GFX_BONE_ID =
    [
        "BONE_NONE",
        "BONE_ROOT",
        "BONE_PELVIS",
        "BONE_R_HIP",
        "BONE_R_THIGH",
        "BONE_R_CALF",
        "BONE_R_FOOT",
        "BONE_R_TOE",
        "BONE_L_HIP",
        "BONE_L_THIGH",
        "BONE_L_CALF",
        "BONE_L_FOOT",
        "BONE_L_TOE",
        "BONE_SPINE_1",
        "BONE_SPINE_2",
        "BONE_NECK",
        "BONE_HEAD",
        "BONE_R_CLAVICLE",
        "BONE_R_CLAVICLE_1",
        "BONE_R_CLAVICLE_2",
        "BONE_R_UPPERARM",
        "BONE_R_FOREARM",
        "BONE_R_WRIST",
        "BONE_R_HAND",
        "BONE_R_FINGER",
        "BONE_R_FINGER_1",
        "BONE_R_FINGER_2",
        "BONE_R_FINGER_INDEX",
        "BONE_R_FINGER_POINT",
        "BONE_R_FINGER_PINKY",
        "BONE_R_HANDLE_1",
        "BONE_R_HANDLE_2",
        "BONE_L_CLAVICLE",
        "BONE_L_CLAVICLE_1",
        "BONE_L_CLAVICLE_2",
        "BONE_L_UPPERARM",
        "BONE_L_FOREARM",
        "BONE_L_WRIST",
        "BONE_L_HAND",
        "BONE_L_FINGER",
        "BONE_L_FINGER_1",
        "BONE_L_FINGER_2",
        "BONE_L_FINGER_INDEX",
        "BONE_L_FINGER_POINT",
        "BONE_L_FINGER_PINKY",
        "BONE_L_HANDLE_1",
        "BONE_L_HANDLE_2",
        "BONE_PONYTAIL1",
        "BONE_PONYTAIL2",
        "BONE_PONYTAIL3",
        "BONE_CLOAK_1",
        "BONE_CLOAK_2",
        "BONE_CLOAK_3",
        "BONE_CLOAK_4",
        "BONE_JAW",
        "BONE_R_FOREHIP",
        "BONE_R_FORETHIGH",
        "BONE_R_FORECALF",
        "BONE_R_FOREFOOT",
        "BONE_R_FORETOES",
        "BONE_L_FOREHIP",
        "BONE_L_FORETHIGH",
        "BONE_L_FORECALF",
        "BONE_L_FOREFOOT",
        "BONE_L_FORETOES",
        "BONE_R_HINDHIP",
        "BONE_R_HINDTHIGH",
        "BONE_R_HINDCALF",
        "BONE_R_HINDFOOT",
        "BONE_R_HINDTOES",
        "BONE_L_HINDHIP",
        "BONE_L_HINDTHIGH",
        "BONE_L_HINDCALF",
        "BONE_L_HINDFOOT",
        "BONE_L_HINDTOES",
        "BONE_1_HIP",
        "BONE_2_HIP",
        "BONE_3_HIP",
        "BONE_1_THIGH",
        "BONE_2_THIGH",
        "BONE_3_THIGH",
        "BONE_1_CALF",
        "BONE_2_CALF",
        "BONE_3_CALF",
        "BONE_1_FOOT",
        "BONE_2_FOOT",
        "BONE_3_FOOT",
        "BONE_1_TOES",
        "BONE_2_TOES",
        "BONE_3_TOES",
        "BONE_SPINE_3",
        "BONE_1_CLAVICLE",
        "BONE_2_CLAVICLE",
        "BONE_3_CLAVICLE",
        "BONE_1_UPPERARM",
        "BONE_2_UPPERARM",
        "BONE_3_UPPERARM",
        "BONE_1_FOREARM",
        "BONE_2_FOREARM",
        "BONE_3_FOREARM",
        "BONE_1_HAND",
        "BONE_2_HAND",
        "BONE_3_HAND",
        "BONE_1_FINGERS",
        "BONE_2_FINGERS",
        "BONE_3_FINGERS",
        "BONE_1_MOUTH_0",
        "BONE_1_MOUTH_1",
        "BONE_2_MOUTH_0",
        "BONE_2_MOUTH_1",
        "BONE_3_MOUTH_0",
        "BONE_3_MOUTH_1",
        "BONE_1_NECK",
        "BONE_2_NECK",
        "BONE_3_NECK",
        "BONE_JAW_1",
        "BONE_JAW_2",
        "BONE_L_HINDHIP_2",
        "BONE_L_HINDTHIGH_2",
        "BONE_L_HINDCALF_2",
        "BONE_L_HINDFOOT_2",
        "BONE_L_HINDTOES_2",
        "BONE_R_HINDHIP_2",
        "BONE_R_HINDTHIGH_2",
        "BONE_R_HINDCALF_2",
        "BONE_R_HINDFOOT_2",
        "BONE_R_HINDTOES_2",
        "BONE_L_HINDHIP_3",
        "BONE_L_HINDTHIGH_3",
        "BONE_L_HINDCALF_3",
        "BONE_L_HINDFOOT_3",
        "BONE_L_HINDTOES_3",
        "BONE_R_HINDHIP_3",
        "BONE_R_HINDTHIGH_3",
        "BONE_R_HINDCALF_3",
        "BONE_R_HINDFOOT_3",
        "BONE_R_HINDTOES_3",
        "BONE_L_MANDIBLE_1",
        "BONE_L_MANDIBLE_2",
        "BONE_L_MANDIBLE_3",
        "BONE_R_MANDIBLE_1",
        "BONE_R_MANDIBLE_2",
        "BONE_R_MANDIBLE_3",
        "BONE_TAIL_1",
        "BONE_TAIL_2",
        "BONE_TAIL_3",
        "BONE_R_BROW",
        "BONE_L_BROW",
        "BONE_L_THUMB_1",
        "BONE_L_THUMB_2",
        "BONE_R_THUMB_1",
        "BONE_R_THUMB_2",
        "BONE_L_WING_1",
        "BONE_L_WING_2",
        "BONE_L_WING_3",
        "BONE_R_WING_1",
        "BONE_R_WING_2",
        "BONE_R_WING_3",
        "BONE_L_TENTCL_1",
        "BONE_R_TENTCL_1",
        "BONE_L_TENTCL_2",
        "BONE_R_TENTCL_2",
        "BONE_L_TENTCL_3",
        "BONE_R_TENTCL_3",
        "BONE_L_TENTCL_4",
        "BONE_R_TENTCL_4",
        "BONE_L_TENTCL_5",
        "BONE_R_TENTCL_5",
        "BONE_L_TENTCL_6",
        "BONE_R_TENTCL_6",
        "BONE_L_BRAIN",
        "BONE_R_BRAIN",
        "BONE_WEAPON_1",
        "BONE_WEAPON_2",
        "BONE_WEAPON_3",
        "BONE_WEAPON_4",
        "BONE_FX_MUZZLE",
        "BONE_FX_BRASS",
        "BONE_TREE_01",
        "BONE_TREE_02",
        "BONE_TREE_03",
        "BONE_TREE_04",
        "BONE_01",
        "BONE_02",
        "BONE_03",
        "BONE_04",
        "BONE_05",
        "BONE_06",
        "BONE_07",
        "BONE_08",
        "BONE_09",
        "BONE_10",
        "BONE_11",
        "BONE_12",
        "BONE_13",
        "BONE_14",
        "BONE_15",
        "BONE_16",
        "BONE_17",
        "BONE_18",
        "BONE_19",
        "BONE_20",
        "BONE_21",
        "BONE_22",
        "BONE_23",
        "BONE_24",
        "BONE_25",
        "BONE_26",
        "BONE_27",
        "BONE_28",
        "BONE_29",
        "BONE_30",
        "MAX_BONE_ID"
    ];

    public static string[] GFX_RENDER_PHASE =
    [
        "RP_OPAQUE",
        "RP_TRANSLUSCENT",
        "RP_BLEND_MULTIPLY",
        "RP_BLEND_SUBTRACT",
        "RP_BLEND_REVSUBTRACT",
        "RP_BLEND_ADD",
        "RP_TYPE_MAX"
    ];

    public static string[] VFX_AABBType =
    [
        "VFXAABB_OPTIMIZED",
        "VFXAABB_AUTOMATIC",
        "VFXAABB_FIXED"
    ];

    public static string[] VFX_AlignmentType =
    [
        "VFXALIGN_ORIGIN",
        "VFXALIGN_CENTER",
        "VFXALIGN_ENDPOINT"
    ];

    public static string[] VFX_CollisionType =
    [
        "VFXCOLLISIONTYPE_BOUNCE",
        "VFXCOLLISIONTYPE_STICK",
        "VFXCOLLISIONTYPE_ERASE",
        "VFXCOLLISIONTYPE_ATTRACT",
        "VFXCOLLISIONTYPE_REPEL"
    ];

    public static string[] VFX_EffectType =
    [
        "VFXEFFECTTYPE_PARTICLESYSTEM",
        "VFXEFFECTTYPE_MOTIONBLUR",
        "VFXEFFECTTYPE_SPRITE",
        "VFXEFFECTTYPE_MODEL",
        "VFXEFFECTTYPE_TRAIL",
        "VFXEFFECTTYPE_COLLISION",
        "VFXEFFECTTYPE_WATER",
        "VFXEFFECTTYPE_LIGHTNING",
        "VFXEFFECTTYPE_MODELEFFECT",
        "VFXEFFECTTYPE_LIGHT",
        "VFXEFFECTTYPE_RAIN",
        "VFXEFFECTTYPE_SNOW",
        "VFXEFFECTTYPE_WIND",
        "VFXEFFECTTYPE_SOUND"
    ];

    public static string[] VFX_EmitterShape =
    [
        "VFXEMITTERSHAPE_BOX",
        "VFXEMITTERSHAPE_SPHERE"
    ];

    public static string[] VFX_LightningWidthStyle =
    [
        "VFXLIGHTNINGWIDTHSTYLE_MINTOMAX",
        "VFXLIGHTNINGWIDTHSTYLE_MAXTOMIN",
        "VFXLIGHTNINGWIDTHSTYLE_RANDOM"
    ];

    public static string[] VFX_LightType =
    [
        "VFXLIGHTTYPE_POINT",
        "VFXLIGHTTYPE_SPOT",
        "VFXLIGHTTYPE_DIRECTIONAL"
    ];

    public static string[] VFX_ParticleTrailType =
    [
        "VFXPARTICLETRAILTYPE_NONE",
        "VFXPARTICLETRAILTYPE_AFTERIMAGE",
        "VFXPARTICLETRAILTYPE_RIBBON",
        "VFXPARTICLETRAILTYPE_LINE"
    ];

    public static string[] VFX_ShadowVolumeType =
    [
        "VFXSHADOWVOLUME_DEFAULT",
        "VFXSHADOWVOLUME_OFF",
        "VFXSHADOWVOLUME_ON"
    ];

    public static string[] VFX_WaterMotionType =
    [
        "VFXWATERMOTIONTYPE_NONE",
        "VFXWATERMOTIONTYPE_RANDOMDROPS",
        "VFXWATERMOTIONTYPE_FLOW",
        "VFXWATERMOTIONTYPE_SWIRL"
    ];

    public static string[] VFX_WaveType =
    [
        "VFXWAVE_NONE",
        "VFXWAVE_LINEAR",
        "VFXWAVE_SQUARE",
        "VFXWAVE_SAW",
        "VFXWAVE_SINE"
    ];

    public static string[] VFX_WindType =
    [
        "VFXWIND_CONSTANT",
        "VFXWIND_RANDOMGUSTS"
    ];

    // technically not an enumeration but it works like one
    // inconsistency is handled in compilation/decompilation
    public static string[] VFX_Target =
    [
        "VFXTARGET_NONE",
        "VFXTARGET_SELF",
        "VFXTARGET_TARGET",
        "VFXTARGET_EFFECT",
        "VFXTARGET_COUNT"
    ];

    public static string[] AllEnumValues()
    {
        return GFX_BONE_ID.Concat(GFX_RENDER_PHASE).Concat(VFX_AABBType).Concat(VFX_AlignmentType)
            .Concat(VFX_CollisionType).Concat(VFX_EffectType).Concat(VFX_EmitterShape).Concat(VFX_LightningWidthStyle)
            .Concat(VFX_LightType).Concat(VFX_ParticleTrailType).Concat(VFX_ShadowVolumeType).Concat(VFX_WaterMotionType)
            .Concat(VFX_WaveType).Concat(VFX_WindType).Concat(VFX_Target).ToArray();
    }
}

public class INT
{
    public enum flags
    {
        Stretch,
        TileX,
        TileY,
        TileBoth,
    }

    public enum magic
    {
        window = 0,
        button = 1,
        picture = 2,
        label = 3,
        edit = 4,
    }

    public struct rect
    {
        public int x1;
        public int y1;
        public int x2;
        public int y2;
        public int Width => x2 - x1;
        public int Height => y2 - y1;
    }

    public struct fragment
    {
        public int srcW;     // source TGA intrinsic width (corners 32; edges/center 1)
        public int srcH;     // source TGA intrinsic height
        public int fA, fB;   // observed always 0
        public int fC;       // observed almost always 1
        public string texture;
        public float fFloat; // observed ~0.114 — likely alpha/blend
        public uint tint;    // BGRA, B/G/R clamped to 0x80 max (so 0x808080 ≈ "full white"); A is opacity
        public rect rect;    // destination rect within parent window
        public int fEnd;     // slot-anchor: per-slot constants (UL=1, UM=0, UR=2, L=3/0, M=2, R=0, LL=1, LM=0, LR=-1)
    }

    public struct FontBlock
    {
        public int stringRef;  // string-table reference (label text lookup; varies per label)
        public string font;    // font face name e.g. "Gothic 821 Condensed BT", "Fallouty"
        public int fontSize;
        public int flag;       // observed always 1
        public uint color;     // BGRA, B/G/R clamped to 0x80 max
        public byte[] pad;     // 26 bytes; pad[2] = horizontal justification (0=left, 1=right, 2=center)
    }

    public struct ButtonSounds
    {
        public byte[] head5;        // 5 bytes, observed all 00
        public string soundDown;    // pressed wav
        public string soundUp;      // released wav
        public string soundHover;   // hover wav
    }

    public struct obj
    {
        public magic magic;
        public ButtonSounds? buttonSounds;  // present when magic==button in rev3
        public int? buttonRev12Int;         // present when magic==button in rev<3
        public rect rect1;
        public string name;
        public byte[] postName3;            // 3 bytes after name
        public string ini;
        public uint colorTint;              // 4 bytes after ini, BGRA (B/G/R clamped to 0x80)
        public rect rect2;
        public byte postRect2Byte;
        // rev>=2 label/font block
        public int lblW;
        public int lblH;
        public int lblA;
        public int lblB;
        public uint strRef;                 // often 0xFFFFFFFF
        public string label;                // label/string text
        public float lblFloat;              // ~font size or 0
        // rev==1 fallback
        public int rev1A;
        public int rev1B;
        public List<fragment> fragments;
        public int last;
        public List<FontBlock> trailerBlocks;  // present when magic in {label,edit} && last==0; carries label string-refs + font
    }

    public string name = "";
    public int revision;
    public byte headerPad;
    public int headerInt;
    public List<obj> objects = [];

    public INT(byte[] b)
    {
        using var ms = new MemoryStream(b);
        using var br = new BinaryReader(ms);

        var sig = br.ReadBytes(7);
        // sig should be "GUI V1." — engine signature; revision follows as ASCII digit
        revision = br.ReadByte() - 0x30;
        var name_length = br.ReadInt32();
        name = new string(br.ReadChars(name_length));
        headerPad = br.ReadByte();
        headerInt = br.ReadInt32();

        while (br.BaseStream.Position < br.BaseStream.Length)
        {
            var obj = new obj();
            obj.magic = (magic)br.ReadInt32();
            if (obj.magic == magic.button)
            {
                if (revision == 3)
                {
                    var bs = new ButtonSounds();
                    bs.head5 = br.ReadBytes(5);
                    bs.soundDown  = new string(br.ReadChars(br.ReadInt32()));
                    bs.soundUp    = new string(br.ReadChars(br.ReadInt32()));
                    bs.soundHover = new string(br.ReadChars(br.ReadInt32()));
                    obj.buttonSounds = bs;
                }
                else
                {
                    obj.buttonRev12Int = br.ReadInt32();
                }
            }
            obj.rect1 = ReadRect(br);
            obj.name = new string(br.ReadChars(br.ReadInt32()));
            obj.postName3 = br.ReadBytes(3);
            obj.ini = new string(br.ReadChars(br.ReadInt32()));
            obj.colorTint = br.ReadUInt32();
            obj.rect2 = ReadRect(br);
            obj.postRect2Byte = br.ReadByte();
            if (revision != 1)
            {
                obj.lblW = br.ReadInt32();
                obj.lblH = br.ReadInt32();
                obj.lblA = br.ReadInt32();
                obj.lblB = br.ReadInt32();
                obj.strRef = br.ReadUInt32();
                obj.label = new string(br.ReadChars(br.ReadInt32()));
                obj.lblFloat = br.ReadSingle();
            }
            else
            {
                obj.rev1A = br.ReadInt32();
                obj.rev1B = br.ReadInt32();
                obj.label = "";
            }
            obj.fragments = new List<fragment>(9);
            for (var i = 0; i < 9; i++)
            {
                var frag = new fragment();
                frag.srcW = br.ReadInt32();
                frag.srcH = br.ReadInt32();
                frag.fA = br.ReadInt32();
                frag.fB = br.ReadInt32();
                frag.fC = br.ReadInt32();
                frag.texture = new string(br.ReadChars(br.ReadInt32()));
                frag.fFloat = br.ReadSingle();
                frag.tint = br.ReadUInt32();
                frag.rect = ReadRect(br);
                frag.fEnd = br.ReadInt32();
                obj.fragments.Add(frag);
            }
            obj.last = br.ReadInt32();
            obj.trailerBlocks = new List<FontBlock>();
            if ((int)obj.magic >= 3 && obj.last == 0)
            {
                obj.trailerBlocks.Add(ReadFontBlock(br));
                while (LooksLikeFontBlockStart(br))
                    obj.trailerBlocks.Add(ReadFontBlock(br));
            }
            objects.Add(obj);
        }
    }

    private static rect ReadRect(BinaryReader br) => new()
    {
        x1 = br.ReadInt32(),
        y1 = br.ReadInt32(),
        x2 = br.ReadInt32(),
        y2 = br.ReadInt32(),
    };

    // The engine's GUI::Window constructor pre-populates each fragment slot with a
    // default texture name of the form "zzmain{kind}-{slot-role}.tga" at a default
    // size (16×16 corners, 64×16/16×64 edges, 64×64 center). Files that don't use
    // nine-slice for a given slot leave those defaults in place. This detects them.
    public static bool IsPlaceholderFragment(fragment f, magic m, int slot)
    {
        if (string.IsNullOrEmpty(f.texture)) return true;
        var kindWord = m switch
        {
            magic.window => "window",
            magic.button => "button",
            magic.picture => "picture",
            magic.label => "label",
            magic.edit => "edit",
            _ => null,
        };
        if (kindWord == null) return false;
        if (!f.texture.StartsWith($"zzmain{kindWord}-", StringComparison.OrdinalIgnoreCase))
            return false;
        var (defW, defH) = slot switch
        {
            0 or 2 or 6 or 8 => (16, 16),
            1 or 7           => (64, 16),
            3 or 5           => (16, 64),
            4                => (64, 64),
            _                => (16, 16),
        };
        return f.srcW == defW && f.srcH == defH;
    }

    // Color stored in .int files is BGRA with B/G/R clamped to a 0x80 maximum
    // (so 0x808080 represents "full white"). Expand to standard 0..255 RGBA for
    // display in editors / ImGui.
    public static uint ExpandBGRA(uint bgra)
    {
        var b = (bgra >> 0) & 0xFF;
        var g = (bgra >> 8) & 0xFF;
        var r = (bgra >> 16) & 0xFF;
        var a = (bgra >> 24) & 0xFF;
        // Scale 0..0x80 → 0..0xFF; clamp anything above.
        static uint scale(uint c) => Math.Min(255u, c * 2);
        return (a << 24) | (scale(r) << 16) | (scale(g) << 8) | scale(b);
    }

    // ImGui draw list expects ABGR (alpha in MSB). Convert from BGRA.
    public static uint ToImGuiColor(uint bgra)
    {
        var b = (bgra >> 0) & 0xFF;
        var g = (bgra >> 8) & 0xFF;
        var r = (bgra >> 16) & 0xFF;
        var a = (bgra >> 24) & 0xFF;
        static uint scale(uint c) => Math.Min(255u, c * 2);
        return (a << 24) | (scale(b) << 16) | (scale(g) << 8) | scale(r);
    }

    private static FontBlock ReadFontBlock(BinaryReader br) => new()
    {
        // Schema: stringRef(int) + name(len-prefixed) + 4 NULs + fontSize(int) + flag(int) + color(BGRA) + pad(26).
        stringRef = br.ReadInt32(),
        font = new string(br.ReadChars(br.ReadInt32())),
        fontSize = ReadFontBlockFontSize(br),
        flag = br.ReadInt32(),
        color = br.ReadUInt32(),
        pad = br.ReadBytes(26),
    };

    private static int ReadFontBlockFontSize(BinaryReader br)
    {
        br.ReadBytes(4);
        return br.ReadInt32();
    }

    // Heuristic to detect a *second* font block after the first in a multi-block trailer.
    // The first block is always present when last==0 (magic ∈ {label, edit}), so this
    // only gates the loop. A font block starts with stringRef + len-prefixed font name.
    // Bound: stringRef >= 16 disambiguates against the next object's magic (0..4).
    private static bool LooksLikeFontBlockStart(BinaryReader br)
    {
        var s = br.BaseStream;
        long start = s.Position;
        if (s.Length - start < 16) return false;
        try
        {
            int stringRef = br.ReadInt32();
            int strLen = br.ReadInt32();
            if (stringRef < 16 || stringRef >= 65536) return false;
            if (strLen <= 0 || strLen >= 64) return false;
            if (s.Length - s.Position < strLen) return false;
            for (int i = 0; i < strLen; i++)
            {
                int c = br.ReadByte();
                if (c < 32 || c >= 127) return false;
            }
            return true;
        }
        finally
        {
            s.Position = start;
        }
    }
}

#endregion