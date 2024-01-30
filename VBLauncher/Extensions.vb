Imports System.Collections.Concurrent
Imports System.Runtime.CompilerServices
Imports System.Text

Public Module Extensions

#Region "Byte array to Class"

    ' Functions to convert from F3-readable byte arrays extracted from files, into easily-manipulatable custom classes
    <Extension>
    Public Function ToEMAPc(ByRef b As Byte()) As EMAPc
        Dim s1o = 18 : Dim s1l = b(s1o - 2)
        Dim s2o = s1o + s1l + 2 : Dim s2l = b(s2o - 2)
        Dim s3o = s2o + s2l + 2 : Dim s3l = b(s3o - 2)
        Return New EMAPc With {
            .s1 = GetString(b, s1o, s1l),
            .s2 = GetString(b, s2o, s2l),
            .s3 = GetString(b, s3o, s3l),
            .col = Color.FromArgb(b(s3o + s3l + 2), b(s3o + s3l + 3), b(s3o + s3l + 4)),
            .il = b(4) = 0,
            .le = b(s3o + s3l)
        }
    End Function

    <Extension>
    Public Function ToEMTRc(ByRef b As Byte()) As EMTRc
        Dim l As New List(Of Point3)
        For i = 20 To b.Count() - 1 Step 12
            l.Add(New Point3(BitConverter.ToSingle(b, i), BitConverter.ToSingle(b, i + 4), BitConverter.ToSingle(b, i + 8)))
        Next
        Return New EMTRc With {
            .n = b(12),
            .r = l
        }
    End Function

    <Extension>
    Public Function ToExTRc(ByRef b As Byte()) As ExTRc
        Dim type = GetString(b.Skip(1).Take(1))
        Return New ExTRc With {
            .type = type,
            .s = If(type = "B", b(12), GetString(b.Skip(14).Take(b(12))))
            }
    End Function

    <Extension>
    Public Function ToECAMc(ByRef b As Byte()) As ECAMc
        Return New ECAMc With {
            .p = New Point4(BitConverter.ToSingle(b, 12), BitConverter.ToSingle(b, 16), BitConverter.ToSingle(b, 20),
                           BitConverter.ToSingle(b, 24))
        }
    End Function

    <Extension>
    Public Function ToEMEPc(ByRef b As Byte()) As EMEPc
        Return New EMEPc With {
            .index = b(12),
            .p = New Point3(BitConverter.ToSingle(b, 73), BitConverter.ToSingle(b, 77), BitConverter.ToSingle(b, 81)),
            .r = BitConverter.ToSingle(b, 105)
        }
    End Function

    <Extension>
    Public Function ToEMEFc(ByRef b As Byte()) As EMEFc
        Return New EMEFc With {
            .s1 = GetString(b, 14, b(12)),
            .l = New Point4(BitConverter.ToSingle(b, 14 + b(12)), BitConverter.ToSingle(b, 18 + b(12)),
                           BitConverter.ToSingle(b, 22 + b(12)), BitConverter.ToSingle(b, 26 + b(12))),
            .s2 = GetString(b, 41 + b(12), b(39 + b(12))),
            .b = b(41 + b(12) + b(39 + b(12)))
        }
    End Function

    <Extension>
    Public Function ToEMSDc(ByRef b As Byte()) As EMSDc
        Return New EMSDc With {
            .s1 = GetString(b, 14, b(12)),
            .s2 = GetString(b, 28 + b(12), b(26 + b(12))),
            .l = New Point3(BitConverter.ToSingle(b, 14 + b(12)), BitConverter.ToSingle(b, 18 + b(12)),
                           BitConverter.ToSingle(b, 22 + b(12)))
        }
    End Function

    <Extension>
    Public Function ToEPTHc(ByRef b As Byte()) As EPTHc
        Dim l As New List(Of Point4)
        For i = 18 + b(12) To b.Count() - 1 Step 24
            l.Add(New Point4(BitConverter.ToSingle(b, i), BitConverter.ToSingle(b, i + 4),
                             BitConverter.ToSingle(b, i + 8), BitConverter.ToSingle(b, i + 12)))
        Next
        Return New EPTHc With {
            .name = GetString(b, 14, b(12)),
            .p = l
        }
    End Function

    <Extension>
    Public Function ToEME2c(ByRef b As Byte()) As EME2c
        Dim cl = b.Locate(Encoding.ASCII.GetBytes("EEOV"))(0)
        Return New EME2c With {
            .name = GetString(b, 14, b(12)),
            .l = New Point4(BitConverter.ToSingle(b, 14 + b(12)), BitConverter.ToSingle(b, 18 + b(12)),
                           BitConverter.ToSingle(b, 22 + b(12)), BitConverter.ToSingle(b, 26 + b(12))),
            .EEOV = b.Skip(cl).Take(BitConverter.ToInt32(b, cl + 8)).ToArray().ToEEOVc
        }
    End Function

    <Extension>
    Public Function ToEEOVc(ByRef b As Byte()) As EEOVc
        Dim s1o = 14 : Dim s1l = b(s1o - 2)
        Dim s2o = s1o + s1l + 13 : Dim s2l = b(s2o - 2)
        Dim s3o = s2o + s2l + 2 : Dim s3l = b(s3o - 2)
        Dim s4o = s3o + s3l + 11 : Dim s4l = b(s4o - 2)
        Dim s5o = s4o + s4l + 3

        Dim ps4 = b(s4o + s4l)

        If ps4 = 2 Then
            s5o += 2
        End If

        Dim s5l = b(s5o - 2)
        If s5l = 1 Then s5l = 0

        Dim inv = New List(Of String)
        Dim io = s5o + s5l + 6
        Dim itemN As String
        For i = io To b.Count() - 1
            Try : itemN = GetString(b.Skip(io).Take(b(io - 2))) : Catch : Exit For : End Try
            If Not itemN.Length = 0 Then inv.Add(itemN)
            io += b(io - 2) + 2
        Next

        Return New EEOVc With {
            .s1 = GetString(b.Skip(s1o).Take(s1l)),
            .s2 = GetString(b.Skip(s2o).Take(s2l)),
            .s3 = GetString(b.Skip(s3o).Take(s3l)),
            .s4 = GetString(b.Skip(s4o).Take(s4l)),
            .s5 = If(ps4 > 0, GetString(b.Skip(s5o).Take(s5l)), ""),
            .ps4 = ps4,
            .inv = inv
        }
    End Function

    <Extension>
    Public Function ToEEN2c(ByRef b As Byte()) As EEN2c
        Dim cl = b.Locate(Encoding.ASCII.GetBytes("EEOV"))(0)
        Dim s1o = 14 : Dim s1l = b(s1o - 2)
        Dim s2o = s1o + s1l + 2 : Dim s2l = b(s2o - 2)
        Dim s3o = s2o + s2l + 2 : Dim s3l = b(s3o - 2)
        Return New EEN2c With {
            .skl = GetString(b, s1o, s1l),
            .invtex = GetString(b, s2o, s2l),
            .acttex = GetString(b, s3o, s3l),
            .sel = b(s3o + s3l + 1),
            .EEOV = b.Skip(cl).Take(BitConverter.ToInt32(b, cl + 8)).ToArray().ToEEOVc
            }
    End Function

    <Extension>
    Public Function ToGENTc(ByRef b As Byte()) As GENTc
        Return New GENTc() With {
            .HoverSR = BitConverter.ToInt32(b, 12),
            .LookSR = BitConverter.ToInt32(b, 16),
            .NameSR = BitConverter.ToInt32(b, 20),
            .UnkwnSR = BitConverter.ToInt32(b, 24),
            .MaxHealth = BitConverter.ToInt32(b, 36),
            .StartHealth = BitConverter.ToInt32(b, 40)
        }
    End Function

    <Extension>
    Public Function ToGCHRc(ByRef b As Byte()) As GCHRc
        Return New GCHRc() With {
            .name = GetString(b, 14, b(12))
        }
    End Function

    <Extension>
    Public Function ToGWAMc(ByRef b As Byte()) As GWAMc
        Return New GWAMc() With {
            .Anim = BitConverter.ToInt32(b, 12),
            .DmgType = BitConverter.ToInt32(b, 16),
            .ShotsFired = BitConverter.ToInt32(b, 20),
            .Range = BitConverter.ToInt32(b, 36),
            .MinDmg = BitConverter.ToInt32(b, 48),
            .MaxDmg = BitConverter.ToInt32(b, 52),
            .AP = BitConverter.ToInt32(b, 62),
            .NameSR = BitConverter.ToInt32(b, 72),
            .VegName = GetString(b, 78, b(76))
        }
    End Function

    <Extension>
    Public Function ToGCREc(b As Byte()) As GCREc

#Region "Offsets, Lengths"

        Dim sl = b(72) * 8 ' Skills added length
        Dim cl = b(76 + sl) * 8 ' Characters added length
        Dim tl = b(80 + sl + cl) * 4 ' Traits added length
        Dim tsl = b(84 + sl + cl + tl) * 4 ' Tag Skills added length
        Dim po = 94 + sl + cl + tl + tsl ' Portrait String offset
        Dim pl = b(92 + sl + cl + tl + tsl) ' Portrait String Length
        Dim Heamo = 131 + sl + cl + tl + tsl + pl
        Dim Heato = Heamo + 2 + b(Heamo - 2)
        Dim Haimo = Heato + 2 + b(Heato - 2)
        Dim Haito = Haimo + 2 + b(Haimo - 2)
        Dim Ponmo = Haito + 2 + b(Haito - 2)
        Dim Ponto = Ponmo + 2 + b(Ponmo - 2)
        Dim Musmo = Ponto + 2 + b(Ponto - 2)
        Dim Musto = Musmo + 2 + b(Musmo - 2)
        Dim Beamo = Musto + 2 + b(Musto - 2)
        Dim Beato = Beamo + 2 + b(Beamo - 2)
        Dim Eyemo = Beato + 2 + b(Beato - 2)
        Dim Eyeto = Eyemo + 2 + b(Eyemo - 2)
        Dim Bodmo = Eyeto + 2 + b(Eyeto - 2)
        Dim Bodto = Bodmo + 2 + b(Bodmo - 2)
        Dim Hanmo = Bodto + 2 + b(Bodto - 2)
        Dim Hanto = Hanmo + 2 + b(Hanmo - 2)
        Dim Feemo = Hanto + 2 + b(Hanto - 2)
        Dim Feeto = Feemo + 2 + b(Feemo - 2)
        Dim Bacmo = Feeto + 2 + b(Feeto - 2)
        Dim Bacto = Bacmo + 2 + b(Bacmo - 2)
        Dim Shomo = Bacto + 2 + b(Bacto - 2)
        Dim Shoto = Shomo + 2 + b(Shomo - 2)
        Dim Vanmo = Shoto + 2 + b(Shoto - 2)
        Dim Vanto = Vanmo + 2 + b(Vanmo - 2)
        Dim psl = sl + cl + tl + tsl + pl + b(Heamo - 2) + b(Heato - 2) + b(Haimo - 2) + b(Haito - 2) + b(Ponmo - 2) +
                  b(Ponto - 2) + b(Musmo - 2) + b(Musto - 2) + b(Beamo - 2) + b(Beato - 2) + b(Eyemo - 2) + b(Eyeto - 2) +
                  b(Bodmo - 2) + b(Bodto - 2) + b(Hanmo - 2) + b(Hanto - 2) + b(Feemo - 2) + b(Feeto - 2) + b(Bacmo - 2) +
                  b(Bacto - 2) + b(Shomo - 2) + b(Shoto - 2) + b(Vanmo - 2) + b(Vanto - 2)

#End Region

#Region "Build Sections"

        Dim gl = b.Locate(Encoding.ASCII.GetBytes("GWAM"))
        Dim tr = New List(Of Integer)
        Dim io = 84 + cl + sl
        For i = 0 To b(80 + cl + sl) - 1
            tr.Add(b(io))
            io += 4
        Next
        Dim ts = New List(Of Integer)
        io = 84 + sl + cl + tl
        For i = 1 To b(84 + sl + cl + tl)
            ts.Add(b(io))
            io += 4
        Next
        Dim skills = New List(Of Skill)
        io = 76
        For i = 0 To b(72) - 1
            skills.Add(New Skill(b(io), b(io + 4)))
            io += 8
        Next

        Dim inv = New List(Of String)
        io = 279 + psl
        Dim itemN As String
        For i = 0 To b(io - 6) - 1
            Try : itemN = GetString(b.Skip(io).Take(b(io - 2))) : Catch : Exit For : End Try
            If Not itemN.Length = 0 Then inv.Add(itemN)
            io += b(io - 2) + 2
        Next

#End Region

        Return New GCREc() With {
            .Special = New Integer() {b(12), b(16), b(20), b(24), b(28), b(32), b(36)},
            .Age = b(56),
            .Skills = skills,
            .Traits = tr,
            .TagSkills = ts,
            .PortStr = GetString(b, po, pl),
            .Hea = New Socket(GetString(b, Heamo, b(Heamo - 2)),
                              GetString(b, Heato, b(Heato - 2))),
            .Hai = New Socket(GetString(b, Haimo, b(Haimo - 2)),
                              GetString(b, Haito, b(Haito - 2))),
            .Pon = New Socket(GetString(b, Ponmo, b(Ponmo - 2)),
                              GetString(b, Ponto, b(Ponto - 2))),
            .Mus = New Socket(GetString(b, Musmo, b(Musmo - 2)),
                              GetString(b, Musto, b(Musto - 2))),
            .Bea = New Socket(GetString(b, Beamo, b(Beamo - 2)),
                              GetString(b, Beato, b(Beato - 2))),
            .Eye = New Socket(GetString(b, Eyemo, b(Eyemo - 2)),
                              GetString(b, Eyeto, b(Eyeto - 2))),
            .Bod = New Socket(GetString(b, Bodmo, b(Bodmo - 2)),
                              GetString(b, Bodto, b(Bodto - 2))),
            .Han = New Socket(GetString(b, Hanmo, b(Hanmo - 2)),
                              GetString(b, Hanto, b(Hanto - 2))),
            .Fee = New Socket(GetString(b, Feemo, b(Feemo - 2)),
                              GetString(b, Feeto, b(Feeto - 2))),
            .Bac = New Socket(GetString(b, Bacmo, b(Bacmo - 2)),
                              GetString(b, Bacto, b(Bacto - 2))),
            .Sho = New Socket(GetString(b, Shomo, b(Shomo - 2)),
                              GetString(b, Shoto, b(Shoto - 2))),
            .Van = New Socket(GetString(b, Vanmo, b(Vanmo - 2)),
                              GetString(b, Vanto, b(Vanto - 2))),
            .Inventory = inv,
            .GWAM = (From i In gl Select b.Skip(i).Take(BitConverter.ToInt32(b, i + 8)).ToArray().ToGWAMc()).ToList()
            }
    End Function

    <Extension>
    Private Function Read2MWTChunk(ByRef b As Byte(), offset As Integer) As _2MWTChunk
        Dim p3 = New Point3(BitConverter.ToSingle(b, offset), BitConverter.ToSingle(b, offset + 4), BitConverter.ToSingle(b, offset + 8))
        Dim s = GetString(b, offset + 14, b(offset + 12))
        Dim p2 = New Point2(BitConverter.ToSingle(b, offset + 14 + s.Length), BitConverter.ToSingle(b, offset + 18 + s.Length))
        Return New _2MWTChunk(s, p3, p2)
    End Function

    <Extension>
    Public Function To2MWTc(ByRef b As Byte()) As _2MWTc
        Dim cl = New List(Of _2MWTChunk)
        Dim io = 158 + b(12)
        For i = 1 To BitConverter.ToInt32(b, 154 + b(12))
            cl.Add(b.Read2MWTChunk(io))
            io += b(io + 12) + 22
        Next
        Return New _2MWTc With {
            .mpf = GetString(b, 14, b(12)),
            .frozen = b(29 + b(12)) = 0,
            .dark = b(27 + b(12)) = 0,
            .chunks = cl
            }
    End Function

    <Extension>
    Public Function ToGITMc(b As Byte()) As GITMc
        Dim a = If(b(20) = 1, 4, 0)
        Dim Hea = b.ReadSocket(24 + a)
        Dim Eye = b.ReadSocket(48 + a + Hea.Length)
        Dim Bod = b.ReadSocket(58 + a + Hea.Length + Eye.Length)
        Dim Bac = b.ReadSocket(62 + a + Hea.Length + Eye.Length + Bod.Length)
        Dim Han = b.ReadSocket(66 + a + Hea.Length + Eye.Length + Bod.Length + Bac.Length)
        Dim Fee = b.ReadSocket(71 + a + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length)
        Dim Sho = b.ReadSocket(75 + a + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length + Fee.Length)
        Dim Van = b.ReadSocket(79 + a + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length + Fee.Length + Sho.Length)
        Dim IHS = b.ReadSocket(84 + a + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length + Fee.Length + Sho.Length + Van.Length)
        Return New GITMc() With
        {
            .type = b(8 + a),
            .equip = b(16 + a) <> 0,
            .eqslot = b(20 + a),
            .Hea = Hea,
            .hHai = b(28 + a + Hea.Length) <> 0,
            .hBea = b(29 + a + Hea.Length) <> 0,
            .hMus = b(30 + a + Hea.Length) <> 0,
            .hEye = b(31 + a + Hea.Length) <> 0,
            .hPon = b(32 + a + Hea.Length) <> 0,
            .hVan = b(33 + a + Hea.Length) <> 0,
            .Eye = Eye,
            .Bod = Bod,
            .Bac = Bac,
            .Han = Han,
            .Fee = Fee,
            .Sho = Sho,
            .Van = Van,
            .IHS = IHS,
            .reload = BitConverter.ToInt32(b, BitConverter.ToInt32(b, 8) - 5)
        }
    End Function

    <Extension>
    Public Function ToGIARc(b As Byte()) As GIARc
        Return New GIARc() With
        {
            .BallR = BitConverter.ToInt32(b, 12),
            .BioR = BitConverter.ToInt32(b, 16),
            .ElecR = BitConverter.ToInt32(b, 20),
            .EMPR = BitConverter.ToInt32(b, 24),
            .NormR = BitConverter.ToInt32(b, 28),
            .HeatR = BitConverter.ToInt32(b, 32)
        }
    End Function

    <Extension>
    Public Function ReadSocket(b As Byte(), i As Integer) As Socket
        Dim model = GetString(b, i + 2, b(i))
        Dim tex = GetString(b, i + 4 + b(i), b(i + 2 + b(i)))
        Return New Socket(model, tex)
    End Function

    <Extension>
    Public Function ReadEMNPChunk(b As Byte(), offset As Integer) As EMNPChunk
        Dim bool = b(offset) <> 0
        Dim p3 = New Point3(BitConverter.ToSingle(b, offset + 1), BitConverter.ToSingle(b, offset + 5), BitConverter.ToSingle(b, offset + 9))
        Dim b1 = b(offset + 13)
        Dim b2 = b(offset + 14)
        Dim b3 = b(offset + 15)
        Dim b4 = b(offset + 16)
        Dim b5 = b(offset + 17)

        Return New EMNPChunk() With {
            .bool = bool,
            .l = p3,
            .b1 = b1,
            .b2 = b2,
            .b3 = b3,
            .b4 = b4,
            .b5 = b5
        }
    End Function

    <Extension>
    Public Function ToEMNPc(b As Byte()) As EMNPc
        Dim cl = New List(Of EMNPChunk)
        Dim io = 16
        For i = 1 To BitConverter.ToInt32(b, 12)
            cl.Add(b.ReadEMNPChunk(io))
            io += 30
        Next
        Return New EMNPc() With {
            .chunks = cl
        }
    End Function

    <Extension>
    Public Function ToEMNOc(b As Byte()) As EMNOc
        Dim l = New Point2(BitConverter.ToSingle(b, 12), BitConverter.ToSingle(b, 20))
        Dim tex = GetString(b, 26, b(24))
        Dim sr = BitConverter.ToInt32(b, 26 + tex.Length)

        Return New EMNOc() With {
                    .l = l,
                    .tex = tex,
                    .sr = sr
                }
    End Function

    <Extension>
    Public Function ToEMFGc(b As Byte()) As EMFGc
        Dim enabled = b(12) <> 0
        Dim colour = Color.FromArgb(b(13), b(14), b(15))
        Dim base_height = BitConverter.ToSingle(b, 16)
        Dim anim1Speed = BitConverter.ToSingle(b, 20)
        Dim anim1Height = BitConverter.ToSingle(b, 24)
        Dim total_height = BitConverter.ToSingle(b, 28)
        Dim anim2Speed = BitConverter.ToSingle(b, 32)
        Dim anim2Height = BitConverter.ToSingle(b, 36)
        Dim verticalOffset = BitConverter.ToSingle(b, 40)
        Dim max_fog_density = BitConverter.ToSingle(b, 44)

        Return New EMFGc() With {
                    .enabled = enabled,
                    .colour = colour,
                    .base_height = base_height,
                    .anim1Speed = anim1Speed,
                    .anim1Height = anim1Height,
                    .total_height = total_height,
                    .anim2Speed = anim2Speed,
                    .anim2Height = anim2Height,
                    .verticalOffset = verticalOffset,
                    .max_fog_density = max_fog_density
                }
    End Function

    <Extension>
    Public Function ToGOBJc(b As Byte()) As GOBJc
        Dim t = b(12)

        Return New GOBJc() With {
                    .Type = t
                }
    End Function

#End Region

#Region ".stf Stuff"

    Public Function STFToTXT(b As Byte()) As IEnumerable(Of String)
        b.PreParse()
        Dim s As New List(Of String)
        Dim oi = 12
        Dim li = 16
        For i = 0 To BitConverter.ToInt32(b, 8) - 1
            s.Add(GetString(b, BitConverter.ToInt32(b, oi), BitConverter.ToInt32(b, li)))
            oi += 16
            li += 16
        Next
        Return s
    End Function

    Public Function TXTToSTF(s As IEnumerable(Of String)) As Byte()
        Dim b As New List(Of Byte)
        b.AddRange(New Byte() {3, 0, 0, 0, 1, 0, 0, 0})
        b.AddRange(BitConverter.GetBytes(s.Count()))
        Dim o = s.Count() * 16 + 12
        For i = 0 To s.Count() - 1
            b.AddRange(BitConverter.GetBytes(o))
            b.AddRange(BitConverter.GetBytes(s(i).Length))
            b.AddRange(New Byte() {&H7E, &HE3, &H3, &H0, &H0, &H0, &H0, &H0})
            o += s(i).Length
        Next
        b.AddRange(s.ToFixedBytes())
        Return b.ToArray()
    End Function

    ' Replace CrLf with "|~" and replace em dash with minus/hyphen
    <Extension>
    Public Sub PreParse(ByRef b As Byte())
        Dim strStart = BitConverter.ToInt32(b, 12)
        Dim l = b.Locate(New Byte() {&HD, &HA})
        For Each m In l
            If m >= strStart Then
                b.Write(m, New Byte() {&H7C, &H7E})
            End If
        Next
        l = b.Locate(New Byte() {&H96})
        For Each m In l
            If m >= strStart Then
                b.Write(m, New Byte() {&H2D})
            End If
        Next
    End Sub

    ' Turn the string array into the chunk of bytes, replacing "|~" with CrLf
    <Extension>
    Public Function ToFixedBytes(ByRef s As IEnumerable(Of String)) As Byte()
        Dim bl = New List(Of Byte)
        For Each stri In s
            bl.AddRange(Encoding.ASCII.GetBytes(stri))
        Next
        Dim b = bl
        Dim l = b.ToArray().Locate(New Byte() {&H7C, &H7E})
        For Each m In l
            b.ToArray().Write(m, New Byte() {&HD, &HA})
        Next
        Return b.ToArray()
    End Function

#End Region

#Region "Read Classes From Bytes"

    <Extension>
    Public Function ReadCRT(b As Byte()) As CRT
        Dim cf = New CRT()
        cf.EEN2 = b.GetRegions("EEN2")(0).ToEEN2c()
        cf.GENT = b.GetRegions("GENT")(0).ToGENTc()
        cf.GCRE = b.GetRegions("GCRE")(0).ToGCREc()
        Try : cf.GCHR = b.GetRegions("GCHR")(0).ToGCHRc() : Catch : End Try
        Return cf
    End Function

    <Extension>
    Public Function ReadMap(b As Byte()) As Map
        Dim cf = New Map
        cf.EMAP = b.GetRegions("EMAP")(0).ToEMAPc
        cf.EME2 = (From x In b.GetRegions("EME2") Select x.ToEME2c).ToList
        cf.EMEP = (From x In b.GetRegions("EMEP") Select x.ToEMEPc).ToList
        Try : cf.ECAM = b.GetRegions("ECAM")(0).ToECAMc : Catch : End Try
        Try : cf._2MWT = b.GetRegions("2MWT")(0).To2MWTc : Catch : End Try
        cf.Triggers = b.GetTriggers
        cf.EPTH = (From x In b.GetRegions("EPTH") Select x.ToEPTHc).ToList
        cf.EMSD = (From x In b.GetRegions("EMSD") Select x.ToEMSDc).ToList
        Try : cf.EMNP = b.GetRegions("EMNP")(0).ToEMNPc : Catch : End Try
        Try : cf.EMFG = b.GetRegions("EMFG")(0).ToEMFGc : Catch : End Try
        cf.EMNO = (From x In b.GetRegions("EMNO") Select x.ToEMNOc).ToList
        cf.EMEF = (From x In b.GetRegions("EMEF") Select x.ToEMEFc).ToList
        Return cf
    End Function

    <Extension>
    Public Function ReadITM(b As Byte()) As ITM
        Dim cf = New ITM()
        cf.EEN2 = b.GetRegions("EEN2")(0).ToEEN2c()
        cf.GENT = b.GetRegions("GENT")(0).ToGENTc()
        cf.GITM = b.GetRegions("GITM")(0).ToGITMc()
        Return cf
    End Function

    <Extension>
    Public Function ReadARM(b As Byte()) As ARM
        Dim cf = New ARM()
        cf.EEN2 = b.GetRegions("EEN2")(0).ToEEN2c()
        cf.GENT = b.GetRegions("GENT")(0).ToGENTc()
        cf.GITM = b.GetRegions("GITM")(0).ToGITMc()
        cf.GIAR = b.GetRegions("GIAR")(0).ToGIARc()
        Return cf
    End Function

    <Extension>
    Public Function ReadUSE(b As Byte()) As USE
        Dim cf = New USE()
        cf.EEN2 = b.GetRegions("EEN2")(0).ToEEN2c()
        cf.GENT = b.GetRegions("GENT")(0).ToGENTc()
        cf.GOBJ = b.GetRegions("GOBJ")(0).ToGOBJc()
        Return cf
    End Function

#End Region

#Region "Byte array search"

    Private ReadOnly Empty(-1) As Integer

    ''' <summary>Searches for a byte array within another byte array.</summary>
    ''' <returns>An integer array containing all locations of the given bytes</returns>
    <Extension>
    Public Function Locate(self As Byte(), candidate As Byte()) As Integer()
        If self Is Nothing Or candidate Is Nothing Or self.Count() = 0 Or candidate.Count() = 0 Or
           candidate.Count() > self.Count() Then
            Return Empty
        End If
        Dim list As New ConcurrentBag(Of Integer)()
        Parallel.For(0, self.Count() - candidate.Count(),
                     Sub(i As Integer)
                         Dim match = True
                         For j = 0 To candidate.Count() - 1
                             If self(i + j) <> candidate(j) Then
                                 match = False
                                 Exit For
                             End If
                         Next j
                         If match Then
                             list.Add(i)
                         End If
                     End Sub)
        Dim sortedList = list.OrderBy(Function(i) i).ToArray() ' ensure list is in order of location for (hopefully) identical file output
        Return If(sortedList.Length = 0, Empty, sortedList)
    End Function

#End Region

    Public Function GetString(b As IEnumerable(Of Byte)) As String
        Return Encoding.ASCII.GetString(b.ToArray())
    End Function

    Public Function GetString(b As Byte(), i1 As Integer, i2 As Integer) As String
        Return Encoding.ASCII.GetString(b.ToArray(), i1, i2)
    End Function

    ' Finds all locations of a given header, reads size, copies that section into byte array, puts array in list.
    <Extension>
    Public Function GetRegions(b As Byte(), hs As String) As Byte()()
        Dim hn = Encoding.ASCII.GetBytes(hs)
        Dim hc = b.Locate(hn)
        Return (From l In hc Let tl = BitConverter.ToInt32(b, l + 8) Select b.Skip(l).Take(tl).ToArray()).ToArray()
    End Function

    ' Finds all triggers for .map files, and the subsequent trigger info chunk
    <Extension>
    Public Function GetTriggers(b As Byte()) As List(Of Trigger)
        Dim hc = b.Locate(Encoding.ASCII.GetBytes("EMTR"))
        Return (From l In hc Let tl = BitConverter.ToInt32(b, l + 8) Let h1 = b.Skip(l).Take(tl).ToArray()
                Let h2 = b.Skip(l + tl).Take(b(l + tl + 8)).ToArray()
                Select New Trigger With {.EMTR = h1.ToEMTRc, .ExTR = h2.ToExTRc}).ToList()
    End Function

    ''' <summary>
    ''' This function copies a byte array into another
    ''' </summary>
    <Extension>
    Public Sub Write(ByRef b As Byte(), offset As Integer, value As IEnumerable(Of Byte))
        Buffer.BlockCopy(value.ToArray(), 0, b, offset, value.Count())
    End Sub

    ''' <summary>
    ''' This function copies a string as ascii bytes into a byte array
    ''' </summary>
    <Extension>
    Public Sub Write(ByRef b As Byte(), offset As Integer, value As String)
        b.Write(offset, Encoding.ASCII.GetBytes(value))
    End Sub

    ''' <summary>
    ''' This function copies a byte into a byte array
    ''' </summary>
    <Extension>
    Public Sub Write(ByRef b As Byte(), offset As Integer, value As Byte)
        b.Write(offset, New Byte() {value})
    End Sub

    ''' <summary>
    ''' This function copies a boolean into a byte array
    ''' </summary>
    <Extension>
    Public Sub Write(ByRef b As Byte(), offset As Integer, value As Boolean)
        b.Write(offset, New Byte() {If(value, 1, 0)})
    End Sub

    ''' <summary>
    ''' This function copies a single precision float into a byte array
    ''' </summary>
    <Extension>
    Public Sub Write(ByRef b As Byte(), offset As Integer, value As Single)
        b.Write(offset, BitConverter.GetBytes(value))
    End Sub

    ''' <summary>
    ''' This function copies a single precision float into a byte array
    ''' </summary>
    <Extension>
    Public Sub Write(ByRef b As Byte(), offset As Integer, value As Integer)
        If value > 255 Then
            b.Write(offset, BitConverter.GetBytes(value))
        Else
            b.Write(offset, CByte(value))
        End If
    End Sub

    ''' <summary>This function reads the values in a DataGridView control into a string array</summary>
    <Extension>
    Public Function GetStringArray(dgv As DataGridView) As List(Of String)
        Return (From r As DataGridViewRow In dgv.Rows Where r.Cells(0).Value IsNot Nothing Select r.Cells(0).Value).Cast(Of String)().ToList()
    End Function

    ''' <summary>Returns the specified color as an array of bytes.</summary>
    ''' <returns>An array of bytes with length 3.</returns>
    <Extension>
    Public Function ToByte(color As Color) As Byte()
        Return {color.R, color.G, color.B}
    End Function

End Module