using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VBLauncher;

public static class Extensions
{
    #region Byte array to Class

    // Functions to convert from F3-readable byte arrays extracted from files, into easily-manipulatable custom classes
    public static EMAPc ToEMAPc(this byte[] b)
    {
        var s1o = 18;
        var s1l = b[s1o - 2];
        var s2o = s1o + s1l + 2;
        var s2l = b[s2o - 2];
        var s3o = s2o + s2l + 2;
        var s3l = b[s3o - 2];
        return new EMAPc
        {
            s1 = GetString(b, s1o, s1l),
            s2 = GetString(b, s2o, s2l),
            s3 = GetString(b, s3o, s3l),
            col = Color.FromArgb(b[s3o + s3l + 2], b[s3o + s3l + 3], b[s3o + s3l + 4]),
            il = b[4] == 0,
            le = b[s3o + s3l]
        };
    }

    public static EMTRc ToEMTRc(this byte[] b)
    {
        var l = new List<Vector3>();
        for (int i = 20, loopTo = b.Count() - 1; i <= loopTo; i += 12)
            l.Add(new Vector3(BitConverter.ToSingle(b, i), BitConverter.ToSingle(b, i + 4),
                BitConverter.ToSingle(b, i + 8)));
        return new EMTRc
        {
            n = b[12],
            r = l
        };
    }

    public static ExTRc ToExTRc(this byte[] b)
    {
        var type = Encoding.ASCII.GetString(b, 1, 1);
        var s = type == "B" ? b[12].ToString() : Encoding.ASCII.GetString(b, 14, b[12]);

        return new ExTRc
        {
            @type = type,
            s = s
        };
    }

    public static ECAMc ToECAMc(this byte[] b)
    {
        return new ECAMc
        {
            p = new Vector4(BitConverter.ToSingle(b, 12), BitConverter.ToSingle(b, 16), BitConverter.ToSingle(b, 20),
                BitConverter.ToSingle(b, 24))
        };
    }

    public static EMEPc ToEMEPc(this byte[] b)
    {
        return new EMEPc
        {
            index = b[12],
            p = new Vector3(BitConverter.ToSingle(b, 73), BitConverter.ToSingle(b, 77),
                BitConverter.ToSingle(b, 81)),
            r = BitConverter.ToSingle(b, 105)
        };
    }

    public static EMEFc ToEMEFc(this byte[] b)
    {
        return new EMEFc
        {
            s1 = GetString(b, 14, b[12]),
            l = new Vector4(BitConverter.ToSingle(b, 14 + b[12]), BitConverter.ToSingle(b, 18 + b[12]),
                BitConverter.ToSingle(b, 22 + b[12]), BitConverter.ToSingle(b, 26 + b[12])),
            s2 = GetString(b, 41 + b[12], b[39 + b[12]]),
            b = b[41 + b[12] + b[39 + b[12]]]
        };
    }

    public static EMSDc ToEMSDc(this byte[] b)
    {
        return new EMSDc
        {
            s1 = GetString(b, 14, b[12]),
            s2 = GetString(b, 28 + b[12], b[26 + b[12]]),
            l = new Vector3(BitConverter.ToSingle(b, 14 + b[12]), BitConverter.ToSingle(b, 18 + b[12]),
                BitConverter.ToSingle(b, 22 + b[12]))
        };
    }

    public static EPTHc ToEPTHc(this byte[] b)
    {
        var l = new List<Vector4>();
        for (int i = 18 + b[12], loopTo = b.Count() - 1; i <= loopTo; i += 24)
            l.Add(new Vector4(BitConverter.ToSingle(b, i), BitConverter.ToSingle(b, i + 4),
                BitConverter.ToSingle(b, i + 8), BitConverter.ToSingle(b, i + 12)));
        return new EPTHc
        {
            name = GetString(b, 14, b[12]),
            p = l
        };
    }

    public static EME2c ToEME2c(this byte[] b)
    {
        var cl = b.Locate("EEOV"u8.ToArray())[0];
        return new EME2c
        {
            name = GetString(b, 14, b[12]),
            l = new Vector4(BitConverter.ToSingle(b, 14 + b[12]), BitConverter.ToSingle(b, 18 + b[12]),
                BitConverter.ToSingle(b, 22 + b[12]), BitConverter.ToSingle(b, 26 + b[12])),
            EEOV = b.Skip(cl).Take(BitConverter.ToInt32(b, cl + 8)).ToArray().ToEEOVc()
        };
    }

    public static EEOVc ToEEOVc(this byte[] b)
    {
        var s1o = 14;
        var s1l = b[s1o - 2];
        var s2o = s1o + s1l + 13;
        var s2l = b[s2o - 2];
        var s3o = s2o + s2l + 2;
        var s3l = b[s3o - 2];
        var s4o = s3o + s3l + 11;
        var s4l = b[s4o - 2];
        var s5o = s4o + s4l + 3;

        var ps4 = b[s4o + s4l];

        if (ps4 == 2)
        {
            s5o += 2;
        }

        var s5l = b[s5o - 2];
        if (s5l == 1)
            s5l = 0;

        var inv = new List<string>();
        var io = s5o + s5l + 6;
        string itemN;
        for (int i = io, loopTo = b.Count() - 1; i <= loopTo; i++)
        {
            try
            {
                itemN = GetString(b.Skip(io).Take(b[io - 2]));
            }
            catch
            {
                break;
            }

            if (!(itemN.Length == 0))
                inv.Add(itemN);
            io += b[io - 2] + 2;
        }

        return new EEOVc
        {
            s1 = GetString(b.Skip(s1o).Take(s1l)),
            s2 = GetString(b.Skip(s2o).Take(s2l)),
            s3 = GetString(b.Skip(s3o).Take(s3l)),
            s4 = GetString(b.Skip(s4o).Take(s4l)),
            s5 = ps4 > 0 ? GetString(b.Skip(s5o).Take(s5l)) : "",
            ps4 = ps4,
            inv = inv.ToArray()
        };
    }

    public static EEN2c ToEEN2c(this byte[] b)
    {
        var cl = b.Locate("EEOV"u8.ToArray())[0];
        var s1o = 14;
        var s1l = b[s1o - 2];
        var s2o = s1o + s1l + 2;
        var s2l = b[s2o - 2];
        var s3o = s2o + s2l + 2;
        var s3l = b[s3o - 2];
        return new EEN2c
        {
            skl = GetString(b, s1o, s1l),
            invtex = GetString(b, s2o, s2l),
            acttex = GetString(b, s3o, s3l),
            sel = b[s3o + s3l + 1] != 0,
            EEOV = b.Skip(cl).Take(BitConverter.ToInt32(b, cl + 8)).ToArray().ToEEOVc()
        };
    }

    public static GENTc ToGENTc(this byte[] b)
    {
        return new GENTc
        {
            HoverSR = BitConverter.ToInt32(b, 12),
            LookSR = BitConverter.ToInt32(b, 16),
            NameSR = BitConverter.ToInt32(b, 20),
            UnkwnSR = BitConverter.ToInt32(b, 24),
            MaxHealth = BitConverter.ToInt32(b, 36),
            StartHealth = BitConverter.ToInt32(b, 40)
        };
    }

    public static GCHRc ToGCHRc(this byte[] b)
    {
        return new GCHRc { name = GetString(b, 14, b[12]) };
    }

    public static GWAMc ToGWAMc(this byte[] b)
    {
        return new GWAMc
        {
            Anim = BitConverter.ToInt32(b, 12),
            DmgType = BitConverter.ToInt32(b, 16),
            ShotsFired = BitConverter.ToInt32(b, 20),
            Range = BitConverter.ToInt32(b, 36),
            MinDmg = BitConverter.ToInt32(b, 48),
            MaxDmg = BitConverter.ToInt32(b, 52),
            AP = BitConverter.ToInt32(b, 62),
            NameSR = BitConverter.ToInt32(b, 72),
            VegName = GetString(b, 78, b[76])
        };
    }

    public static GCREc ToGCREc(this byte[] b)
    {
        #region Offsets, Lengths

        var sl = b[72] * 8; // Skills added length
        var cl = b[76 + sl] * 8; // Characters added length
        var tl = b[80 + sl + cl] * 4; // Traits added length
        var tsl = b[84 + sl + cl + tl] * 4; // Tag Skills added length
        var po = 94 + sl + cl + tl + tsl; // Portrait String offset
        var pl = b[92 + sl + cl + tl + tsl]; // Portrait String Length
        var Heamo = 131 + sl + cl + tl + tsl + pl;
        var Heato = Heamo + 2 + b[Heamo - 2];
        var Haimo = Heato + 2 + b[Heato - 2];
        var Haito = Haimo + 2 + b[Haimo - 2];
        var Ponmo = Haito + 2 + b[Haito - 2];
        var Ponto = Ponmo + 2 + b[Ponmo - 2];
        var Musmo = Ponto + 2 + b[Ponto - 2];
        var Musto = Musmo + 2 + b[Musmo - 2];
        var Beamo = Musto + 2 + b[Musto - 2];
        var Beato = Beamo + 2 + b[Beamo - 2];
        var Eyemo = Beato + 2 + b[Beato - 2];
        var Eyeto = Eyemo + 2 + b[Eyemo - 2];
        var Bodmo = Eyeto + 2 + b[Eyeto - 2];
        var Bodto = Bodmo + 2 + b[Bodmo - 2];
        var Hanmo = Bodto + 2 + b[Bodto - 2];
        var Hanto = Hanmo + 2 + b[Hanmo - 2];
        var Feemo = Hanto + 2 + b[Hanto - 2];
        var Feeto = Feemo + 2 + b[Feemo - 2];
        var Bacmo = Feeto + 2 + b[Feeto - 2];
        var Bacto = Bacmo + 2 + b[Bacmo - 2];
        var Shomo = Bacto + 2 + b[Bacto - 2];
        var Shoto = Shomo + 2 + b[Shomo - 2];
        var Vanmo = Shoto + 2 + b[Shoto - 2];
        var Vanto = Vanmo + 2 + b[Vanmo - 2];
        var psl = sl + cl + tl + tsl + pl + b[Heamo - 2] + b[Heato - 2] + b[Haimo - 2] + b[Haito - 2] +
                  b[Ponmo - 2] + b[Ponto - 2] + b[Musmo - 2] + b[Musto - 2] + b[Beamo - 2] + b[Beato - 2] +
                  b[Eyemo - 2] + b[Eyeto - 2] + b[Bodmo - 2] + b[Bodto - 2] + b[Hanmo - 2] + b[Hanto - 2] +
                  b[Feemo - 2] + b[Feeto - 2] + b[Bacmo - 2] + b[Bacto - 2] + b[Shomo - 2] + b[Shoto - 2] +
                  b[Vanmo - 2] + b[Vanto - 2];

        #endregion

        #region Build Sections

        var gl = b.Locate("GWAM"u8.ToArray());
        var tr = new List<int>();
        var io = 84 + cl + sl;
        for (int i = 0, loopTo = b[80 + cl + sl] - 1; i <= loopTo; i++)
        {
            tr.Add(b[io]);
            io += 4;
        }

        var ts = new List<int>();
        io = 84 + sl + cl + tl;
        for (int i = 1, loopTo1 = b[84 + sl + cl + tl]; i <= loopTo1; i++)
        {
            ts.Add(b[io]);
            io += 4;
        }

        var skills = new List<Skill>();
        io = 76;
        for (int i = 0, loopTo2 = b[72] - 1; i <= loopTo2; i++)
        {
            skills.Add(new Skill(b[io], b[io + 4]));
            io += 8;
        }

        var inv = new List<string>();
        io = 279 + psl;
        string itemN;
        for (int i = 0, loopTo3 = b[io - 6] - 1; i <= loopTo3; i++)
        {
            try
            {
                itemN = GetString(b.Skip(io).Take(b[io - 2]));
            }
            catch
            {
                break;
            }

            if (!(itemN.Length == 0))
                inv.Add(itemN);
            io += b[io - 2] + 2;
        }

        #endregion

        return new GCREc
        {
            Special = [b[12], b[16], b[20], b[24], b[28], b[32], b[36]],
            Age = b[56],
            Skills = skills,
            Traits = tr,
            TagSkills = ts,
            PortStr = GetString(b, po, pl),
            Hea = new Socket(GetString(b, Heamo, b[Heamo - 2]), GetString(b, Heato, b[Heato - 2])),
            Hai = new Socket(GetString(b, Haimo, b[Haimo - 2]), GetString(b, Haito, b[Haito - 2])),
            Pon = new Socket(GetString(b, Ponmo, b[Ponmo - 2]), GetString(b, Ponto, b[Ponto - 2])),
            Mus = new Socket(GetString(b, Musmo, b[Musmo - 2]), GetString(b, Musto, b[Musto - 2])),
            Bea = new Socket(GetString(b, Beamo, b[Beamo - 2]), GetString(b, Beato, b[Beato - 2])),
            Eye = new Socket(GetString(b, Eyemo, b[Eyemo - 2]), GetString(b, Eyeto, b[Eyeto - 2])),
            Bod = new Socket(GetString(b, Bodmo, b[Bodmo - 2]), GetString(b, Bodto, b[Bodto - 2])),
            Han = new Socket(GetString(b, Hanmo, b[Hanmo - 2]), GetString(b, Hanto, b[Hanto - 2])),
            Fee = new Socket(GetString(b, Feemo, b[Feemo - 2]), GetString(b, Feeto, b[Feeto - 2])),
            Bac = new Socket(GetString(b, Bacmo, b[Bacmo - 2]), GetString(b, Bacto, b[Bacto - 2])),
            Sho = new Socket(GetString(b, Shomo, b[Shomo - 2]), GetString(b, Shoto, b[Shoto - 2])),
            Van = new Socket(GetString(b, Vanmo, b[Vanmo - 2]), GetString(b, Vanto, b[Vanto - 2])),
            Inventory = inv.ToArray(),
            GWAM = (from i in gl
                    select b.Skip(i).Take(BitConverter.ToInt32(b, i + 8)).ToArray().ToGWAMc()).ToList()
        };
    }

    private static _2MWTChunk Read2MWTChunk(this byte[] b, int offset)
    {
        var p3 = new Vector3(BitConverter.ToSingle(b, offset), BitConverter.ToSingle(b, offset + 4),
            BitConverter.ToSingle(b, offset + 8));
        var s = GetString(b, offset + 14, b[offset + 12]);
        var p2 = new Vector2(BitConverter.ToSingle(b, offset + 14 + s.Length),
            BitConverter.ToSingle(b, offset + 18 + s.Length));
        return new _2MWTChunk(s, p3, p2);
    }

    public static _2MWTc To2MWTc(this byte[] b)
    {
        var cl = new List<_2MWTChunk>();
        var io = 158 + b[12];
        for (int i = 1, loopTo = BitConverter.ToInt32(b, 154 + b[12]); i <= loopTo; i++)
        {
            cl.Add(Read2MWTChunk(b, io));
            io += b[io + 12] + 22;
        }

        return new _2MWTc
        {
            mpf = GetString(b, 14, b[12]),
            frozen = b[29 + b[12]] == 0,
            dark = b[27 + b[12]] == 0,
            chunks = cl
        };
    }

    public static GITMc ToGITMc(this byte[] b)
    {
        var a = b[20] == 1 ? 4 : 0;
        var Hea = b.ReadSocket(24 + a);
        var Eye = b.ReadSocket(48 + a + Hea.Length);
        var Bod = b.ReadSocket(58 + a + Hea.Length + Eye.Length);
        var Bac = b.ReadSocket(62 + a + Hea.Length + Eye.Length + Bod.Length);
        var Han = b.ReadSocket(66 + a + Hea.Length + Eye.Length + Bod.Length + Bac.Length);
        var Fee = b.ReadSocket(71 + a + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length);
        var Sho = b.ReadSocket(75 + a + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length +
                               Fee.Length);
        var Van = b.ReadSocket(79 + a + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length +
                               Fee.Length + Sho.Length);
        var IHS = b.ReadSocket(84 + a + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length +
                               Fee.Length + Sho.Length + Van.Length);
        return new GITMc
        {
            @type = b[12],
            equip = b[16 + a] != 0,
            eqslot = b[20 + a],
            Hea = Hea,
            hHai = b[28 + a + Hea.Length] != 0,
            hBea = b[29 + a + Hea.Length] != 0,
            hMus = b[30 + a + Hea.Length] != 0,
            hEye = b[31 + a + Hea.Length] != 0,
            hPon = b[32 + a + Hea.Length] != 0,
            hVan = b[33 + a + Hea.Length] != 0,
            Eye = Eye,
            Bod = Bod,
            Bac = Bac,
            Han = Han,
            Fee = Fee,
            Sho = Sho,
            Van = Van,
            IHS = IHS,
            reload = BitConverter.ToInt32(b, BitConverter.ToInt32(b, 8) - 4)
        };
    }

    public static GIARc ToGIARc(this byte[] b)
    {
        return new GIARc
        {
            BallR = BitConverter.ToInt32(b, 12),
            BioR = BitConverter.ToInt32(b, 16),
            ElecR = BitConverter.ToInt32(b, 20),
            EMPR = BitConverter.ToInt32(b, 24),
            NormR = BitConverter.ToInt32(b, 28),
            HeatR = BitConverter.ToInt32(b, 32)
        };
    }

    public static GIAMc ToGIAMc(this byte[] b)
    {
        return new GIAMc
        {
            ammoType = BitConverter.ToInt32(b, 12),
            minDmg = BitConverter.ToInt32(b, 16),
            maxDmg = BitConverter.ToInt32(b, 20),
            unk1 = BitConverter.ToInt32(b, 24),
            critChance = BitConverter.ToInt32(b, 28),
            engUnk1 = BitConverter.ToInt32(b, 32),
            unk2 = BitConverter.ToInt32(b, 36),
            unk3 = BitConverter.ToInt32(b, 40)
        };
    }

    public static GIWPc ToGIWPc(this byte[] b)
    {
        return new GIWPc();
    }

    public static Socket ReadSocket(this byte[] b, int i)
    {
        var model = GetString(b, i + 2, b[i]);
        var tex = GetString(b, i + 4 + b[i], b[i + 2 + b[i]]);
        return new Socket(model, tex);
    }

    public static EMNPChunk ReadEMNPChunk(this byte[] b, int offset)
    {
        var @bool = b[offset] != 0;
        var p3 = new Vector3(BitConverter.ToSingle(b, offset + 1), BitConverter.ToSingle(b, offset + 5),
            BitConverter.ToSingle(b, offset + 9));
        var b1 = b[offset + 13];
        var b2 = b[offset + 14];
        var b3 = b[offset + 15];
        var b4 = b[offset + 16];
        var b5 = b[offset + 17];

        return new EMNPChunk
        {
            @bool = @bool ? (byte)1 : (byte)0,
            l = p3,
            b1 = b1,
            b2 = b2,
            b3 = b3,
            b4 = b4,
            b5 = b5
        };
    }

    public static EMNPc ToEMNPc(this byte[] b)
    {
        var cl = new List<EMNPChunk>();
        var io = 16;
        for (int i = 1, loopTo = BitConverter.ToInt32(b, 12); i <= loopTo; i++)
        {
            cl.Add(b.ReadEMNPChunk(io));
            io += 30;
        }

        return new EMNPc { chunks = cl };
    }

    public static EMNOc ToEMNOc(this byte[] b)
    {
        var l = new Vector2(BitConverter.ToSingle(b, 12), BitConverter.ToSingle(b, 20));
        var tex = GetString(b, 26, b[24]);
        var sr = BitConverter.ToInt32(b, 26 + tex.Length);

        return new EMNOc
        {
            l = l,
            tex = tex,
            sr = sr
        };
    }

    public static EMFGc ToEMFGc(this byte[] b)
    {
        var enabled = b[12] != 0;
        var colour = Color.FromArgb(b[13], b[14], b[15]);
        var base_height = BitConverter.ToSingle(b, 16);
        var anim1Speed = BitConverter.ToSingle(b, 20);
        var anim1Height = BitConverter.ToSingle(b, 24);
        var total_height = BitConverter.ToSingle(b, 28);
        var anim2Speed = BitConverter.ToSingle(b, 32);
        var anim2Height = BitConverter.ToSingle(b, 36);
        var verticalOffset = BitConverter.ToSingle(b, 40);
        var max_fog_density = BitConverter.ToSingle(b, 44);

        return new EMFGc
        {
            enabled = enabled,
            colour = colour,
            base_height = base_height,
            anim1Speed = anim1Speed,
            anim1Height = anim1Height,
            total_height = total_height,
            anim2Speed = anim2Speed,
            anim2Height = anim2Height,
            verticalOffset = verticalOffset,
            max_fog_density = max_fog_density
        };
    }

    public static GOBJc ToGOBJc(this byte[] b)
    {
        var t = b[12];

        return new GOBJc { Type = t };
    }

    #endregion

    #region .stf Stuff

    public static IEnumerable<string> STFToTXT(byte[] b)
    {
        PreParse(b);
        var s = new List<string>();
        var oi = 12;
        var li = 16;
        for (int i = 0, loopTo = BitConverter.ToInt32(b, 8) - 1; i <= loopTo; i++)
        {
            s.Add(GetString(b, BitConverter.ToInt32(b, oi), BitConverter.ToInt32(b, li)));
            oi += 16;
            li += 16;
        }

        return s;
    }

    public static byte[] TXTToSTF(IEnumerable<string> s)
    {
        var b = new List<byte>();
        b.AddRange([3, 0, 0, 0, 1, 0, 0, 0]);
        b.AddRange(BitConverter.GetBytes(s.Count()));
        var o = s.Count() * 16 + 12;
        for (int i = 0, loopTo = s.Count() - 1; i <= loopTo; i++)
        {
            b.AddRange(BitConverter.GetBytes(o));
            b.AddRange(BitConverter.GetBytes(s.ElementAtOrDefault(i).Length));
            b.AddRange([0x7E, 0xE3, 0x3, 0x0, 0x0, 0x0, 0x0, 0x0]);
        o += s.ElementAtOrDefault(i).Length;
    }

    b.AddRange(ToFixedBytes(ref s));
        return b.ToArray();
    }

// Replace CrLf with "|~" and replace em dash with minus/hyphen
public static void PreParse(this byte[] b)
{
    var strStart = BitConverter.ToInt32(b, 12);
    var l = b.Locate([0xD, 0xA]);
    foreach (var m in l)
    {
        if (m >= strStart)
        {
            Write(b, m, [0x7C, 0x7E]);
        }
    }

    l = b.Locate([0x96]);
    foreach (var m in l)
    {
        if (m >= strStart)
        {
            Write(b, m, [0x2D]);
        }
    }
}

// Turn the string array into the chunk of bytes, replacing "|~" with CrLf
public static byte[] ToFixedBytes(ref IEnumerable<string> s)
{
    var bl = new List<byte>();
    foreach (var stri in s)
        bl.AddRange(Encoding.ASCII.GetBytes(stri));
    var b = bl;
    var l = b.ToArray().Locate([0x7C, 0x7E]);
    foreach (var m in l)
        Write(b.ToArray(), m, [0xD, 0xA]);
    return b.ToArray();
}

#endregion

#region Read Classes From Bytes

public static CRT ReadCRT(this byte[] b)
{
    var cf = new CRT
    {
        EEN2 = b.GetRegions("EEN2")[0].ToEEN2c(),
        GENT = b.GetRegions("GENT")[0].ToGENTc(),
        GCRE = b.GetRegions("GCRE")[0].ToGCREc()
    };
    try
    {
        cf.GCHR = b.GetRegions("GCHR")[0].ToGCHRc();
    }
    catch
    {
    }

    return cf;
}

public static Map ReadMap(this byte[] b)
{
    var cf = new Map
    {
        EMAP = b.GetRegions("EMAP")[0].ToEMAPc(),
        EME2 = (from x in b.GetRegions("EME2")
                select x.ToEME2c()).ToList(),
        EMEP = (from x in b.GetRegions("EMEP")
                select x.ToEMEPc()).ToList()
    };
    try
    {
        cf.ECAM = b.GetRegions("ECAM")[0].ToECAMc();
    }
    catch
    {
    }

    try
    {
        cf._2MWT = b.GetRegions("2MWT")[0].To2MWTc();
    }
    catch
    {
    }

    cf.Triggers = b.GetTriggers();
    cf.EPTH = (from x in b.GetRegions("EPTH")
               select x.ToEPTHc()).ToList();
    cf.EMSD = (from x in b.GetRegions("EMSD")
               select x.ToEMSDc()).ToList();
    try
    {
        cf.EMNP = b.GetRegions("EMNP")[0].ToEMNPc();
    }
    catch
    {
    }

    try
    {
        cf.EMFG = b.GetRegions("EMFG")[0].ToEMFGc();
    }
    catch
    {
    }

    cf.EMNO = (from x in b.GetRegions("EMNO")
               select x.ToEMNOc()).ToList();
    cf.EMEF = (from x in b.GetRegions("EMEF")
               select x.ToEMEFc()).ToList();
    return cf;
}

public static ITM ReadITM(this byte[] b)
{
    var cf = new ITM
    {
        EEN2 = b.GetRegions("EEN2")[0].ToEEN2c(),
        GENT = b.GetRegions("GENT")[0].ToGENTc(),
        GITM = b.GetRegions("GITM")[0].ToGITMc()
    };
    return cf;
}

public static ARM ReadARM(this byte[] b)
{
    var cf = new ARM
    {
        EEN2 = b.GetRegions("EEN2")[0].ToEEN2c(),
        GENT = b.GetRegions("GENT")[0].ToGENTc(),
        GITM = b.GetRegions("GITM")[0].ToGITMc(),
        GIAR = b.GetRegions("GIAR")[0].ToGIARc()
    };
    return cf;
}

public static USE ReadUSE(this byte[] b)
{
    var cf = new USE
    {
        EEN2 = b.GetRegions("EEN2")[0].ToEEN2c(),
        GENT = b.GetRegions("GENT")[0].ToGENTc(),
        GOBJ = b.GetRegions("GOBJ")[0].ToGOBJc()
    };
    return cf;
}

public static WEA ReadWEA(this byte[] b)
{
    var cf = new WEA
    {
        EEN2 = b.GetRegions("EEN2")[0].ToEEN2c(),
        GENT = b.GetRegions("GENT")[0].ToGENTc(),
        GITM = b.GetRegions("GITM")[0].ToGITMc(),
        GIWP = b.GetRegions("GIWP")[0].ToGIWPc()
    };
    return cf;
}

public static AMO ReadAMO(this byte[] b)
{
    var cf = new AMO
    {
        EEN2 = b.GetRegions("EEN2")[0].ToEEN2c(),
        GENT = b.GetRegions("GENT")[0].ToGENTc(),
        GITM = b.GetRegions("GITM")[0].ToGITMc(),
        GIAM = b.GetRegions("GIAM")[0].ToGIAMc()
    };
    return cf;
}

#endregion

#region Byte array search

private static readonly int[] Empty = [];

/// <summary>Searches for a byte array within another byte array.</summary>
/// <returns>An integer array containing all locations of the given bytes</returns>
public static int[] Locate(this byte[] self, byte[] candidate)
{
    if (self is null | candidate is null | !self.Any() | !candidate.Any() |
        candidate.Count() > self.Count())
    {
        return Empty;
    }

    var list = new ConcurrentBag<int>();
    Parallel.For(0, self.Count() - candidate.Count(), (i) =>
    {
        var match = true;
        for (int j = 0, loopTo = candidate.Count() - 1; j <= loopTo; j++)
        {
            if (self[i + j] == candidate[j]) continue;
            match = false;
            break;
        }

        if (match)
        {
            list.Add(i);
        }
    });
    var sortedList =
        list.OrderBy(i => i)
            .ToArray(); // ensure list is in order of location for (hopefully) identical file output
    return sortedList.Length == 0 ? Empty : sortedList;
}

#endregion

public static string GetString(IEnumerable<byte> b)
{
    return Encoding.ASCII.GetString(b.ToArray());
}

public static string GetString(byte[] b, int i1, int i2)
{
    return Encoding.ASCII.GetString(b.ToArray(), i1, i2);
}

// Finds all locations of a given header, reads size, copies that section into byte array, puts array in list.
public static byte[][] GetRegions(this byte[] b, string hs)
{
    var hn = Encoding.ASCII.GetBytes(hs);
    var hc = b.Locate(hn);
    return (from l in hc
            let tl = BitConverter.ToInt32(b, l + 8)
            select b.Skip(l).Take(tl).ToArray()).ToArray();
}

// Finds all triggers for .map files, and the subsequent trigger info chunk
public static List<Trigger> GetTriggers(this byte[] b)
{
    var hc = b.Locate("EMTR"u8.ToArray());
    return (from l in hc
            let tl = BitConverter.ToInt32(b, l + 8)
            let h1 = b.Skip(l).Take(tl).ToArray()
            let h2 = b.Skip(l + tl).Take(b[l + tl + 8]).ToArray()
            select new Trigger { EMTR = h1.ToEMTRc(), ExTR = h2.ToExTRc() }).ToList();
}

/// <summary>
/// This function copies a byte array into another
/// </summary>
public static void Write(this byte[] b, int offset, IEnumerable<byte> value)
{
    var enumerable = value as byte[] ?? value.ToArray();
    Buffer.BlockCopy(enumerable.ToArray(), 0, b, offset, enumerable.Length);
}

/// <summary>
/// This function copies a string as ascii bytes into a byte array
/// </summary>
public static void Write(this byte[] b, int offset, string value)
{
    Write(b, offset, Encoding.ASCII.GetBytes(value));
}

/// <summary>
/// This function copies a byte into a byte array
/// </summary>
public static void Write(this byte[] b, int offset, byte value)
{
    Write(b, offset, [value]);
}

/// <summary>
/// This function copies a boolean into a byte array
/// </summary>
public static void Write(this byte[] b, int offset, bool value)
{
    Write(b, offset, [(byte)(value ? 1 : 0)]);
}

/// <summary>
/// This function copies a single precision float into a byte array
/// </summary>
public static void Write(this byte[] b, int offset, float value)
{
    Write(b, offset, BitConverter.GetBytes(value));
}

/// <summary>
/// This function copies a 32-bit integer into a byte array
/// </summary>
public static void Write(this byte[] b, int offset, int value)
{
    if (value > 255)
    {
        Write(b, offset, BitConverter.GetBytes(value));
    }
    else
    {
        Write(b, offset, (byte)value);
    }
}

/// <summary>This function reads the values in a DataGridView control into a string array</summary>
public static List<string> GetStringArray(this DataGridView dgv)
{
    return (from DataGridViewRow r in dgv.Rows
            where r.Cells[0].Value != null
            select r.Cells[0].Value).Cast<string>().ToList();
}

/// <summary>Returns the specified color as an array of bytes.</summary>
/// <returns>An array of bytes with length 3.</returns>
public static byte[] ToByte(this Color color)
{
    return [color.R, color.G, color.B];
}

public static byte[] ToByte(this Vector4 vec)
{
    var b = new List<byte>();
    b.AddRange(BitConverter.GetBytes(vec.X));
    b.AddRange(BitConverter.GetBytes(vec.Y));
    b.AddRange(BitConverter.GetBytes(vec.Z));
    b.AddRange(BitConverter.GetBytes(vec.W));
    return b.ToArray();
}

public static byte[] ToByte(this Vector3 vec)
{
    var b = new List<byte>();
    b.AddRange(BitConverter.GetBytes(vec.X));
    b.AddRange(BitConverter.GetBytes(vec.Y));
    b.AddRange(BitConverter.GetBytes(vec.Z));
    return b.ToArray();
}

public static byte[] ToByte(this Vector2 vec)
{
    var b = new List<byte>();
    b.AddRange(BitConverter.GetBytes(vec.X));
    b.AddRange(BitConverter.GetBytes(vec.Y));
    return b.ToArray();
}
}