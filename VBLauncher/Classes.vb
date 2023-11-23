#Region "Filetype Classes"

' Classes containing the headers used by that file type, in the order they should be put back into a new file.
Imports System.Text

Public Class Map
    Property EMAP As EMAPc
    Property EME2 As List(Of EME2c)
    Property EMEP As List(Of EMEPc)
    Property ECAM As ECAMc
    Property _2MWT As _2MWTc
    Property Triggers As List(Of Trigger)
    Property EPTH As List(Of EPTHc)
    Property EMSD As List(Of EMSDc)

    ' EMNP
    Property EMEF As List(Of EMEFc)

    Sub New()
        EMAP = New EMAPc()
        EMEP = New List(Of EMEPc)
        EME2 = New List(Of EME2c)
        EMEF = New List(Of EMEFc)
        EMSD = New List(Of EMSDc)
        EPTH = New List(Of EPTHc)
        Triggers = New List(Of Trigger)
        ECAM = Nothing
        _2MWT = Nothing
    End Sub

    Function ToByte() As IEnumerable(Of Byte)
        Dim b As New List(Of Byte)
        b.AddRange(EMAP.ToByte)
        b.AddRange(EME2.SelectMany(Function(x) x.ToByte()))
        b.AddRange(EMEP.SelectMany(Function(x) x.ToByte()))
        If ECAM IsNot Nothing Then b.AddRange(ECAM.ToByte())
        If _2MWT IsNot Nothing Then b.AddRange(_2MWT.ToByte())
        b.AddRange(Triggers.SelectMany(Function(x) x.ToByte()))
        b.AddRange(EPTH.SelectMany(Function(x) x.ToByte()))
        b.AddRange(EMSD.SelectMany(Function(x) x.ToByte()))
        b.AddRange(New Byte() {&H45, &H4D, &H4E, &H50, &H0, &H0, &H0, &H0, &H10, &H0, &H0, &H0, &H0, &H0, &H0, &H0}) ' EMNP Chunk
        b.AddRange(EMEF.SelectMany(Function(x) x.ToByte()))
        Return b
    End Function

End Class

Public Class CRT
    Property EEN2 As EEN2c
    Property GENT As GENTc
    Property GCRE As GCREc
    Property GCHR As GCHRc

    Sub New()
        EEN2 = New EEN2c()
        GENT = New GENTc()
        GCRE = New GCREc()
        GCHR = New GCHRc()
    End Sub

    Function ToByte() As IEnumerable(Of Byte)
        Dim b As New List(Of Byte)
        b.AddRange(EEN2.ToByte)
        b.AddRange(GENT.ToByte)
        b.AddRange(GCRE.ToByte)
        If GCHR.name <> "" Then b.AddRange(GCHR.ToByte)
        Return b
    End Function

End Class

Public Class ITM
    Property EEN2 As EEN2c
    Property GENT As GENTc
    Property GITM As GITMc

    Sub New()
        EEN2 = New EEN2c()
        GENT = New GENTc()
        GITM = New GITMc()
    End Sub

    Function ToByte() As IEnumerable(Of Byte)
        Dim b = New List(Of Byte)
        b.AddRange(EEN2.ToByte())
        b.AddRange(GENT.ToByte())
        b.AddRange(GITM.ToByte())
        Return b
    End Function

End Class

Public Class ARM
    Property EEN2 As EEN2c
    Property GENT As GENTc
    Property GITM As GITMc
    Property GIAR As GIARc

    Sub New()
        EEN2 = New EEN2c()
        GENT = New GENTc()
        GITM = New GITMc()
        GIAR = New GIARc()
    End Sub

    Function ToByte() As IEnumerable(Of Byte)
        Dim b = New List(Of Byte)
        b.AddRange(EEN2.ToByte())
        b.AddRange(GENT.ToByte())
        b.AddRange(GITM.ToByte())
        b.AddRange(GIAR.ToByte())
        Return b
    End Function

End Class

Public Class AMO
    Property EEN2 As EEN2c
    Property GENT As GENTc
    Property GITM As GITMc
    'Property GIAM As GIAMc

    Sub New()
        EEN2 = New EEN2c()
        GENT = New GENTc()
        GITM = New GITMc()
        'GIAM = New GIAMc()
    End Sub

    Function ToByte() As IEnumerable(Of Byte)
        Dim b = New List(Of Byte)
        b.AddRange(EEN2.ToByte())
        b.AddRange(GENT.ToByte())
        b.AddRange(GITM.ToByte())
        'b.AddRange(GIAM.ToByte())
        Return b
    End Function

End Class

Public Class CON
    Property EEN2 As EEN2c
    Property GENT As GENTc
    'Property GOBJ As GOBJc
    'Property GCON as GCONc

    Sub New()
        EEN2 = New EEN2c()
        GENT = New GENTc()
        'GOBJ = New GOBJc()
        'GCON = New GCONc()
    End Sub

    Function ToByte() As IEnumerable(Of Byte)
        Dim b = New List(Of Byte)
        b.AddRange(EEN2.ToByte())
        b.AddRange(GENT.ToByte())
        'b.AddRange(GOBJ.ToByte())
        'b.AddRange(GCON.ToByte())
        Return b
    End Function

End Class

Public Class DOR
    Property EEN2 As EEN2c
    Property GENT As GENTc
    'Property GOBJ As GOBJc
    'Property GDOR As GDORc

    Sub New()
        EEN2 = New EEN2c()
        GENT = New GENTc()
        'GOBJ = New GOBJc()
        'GDOR = New GDORc()
    End Sub

    Function ToByte() As IEnumerable(Of Byte)
        Dim b = New List(Of Byte)
        b.AddRange(EEN2.ToByte())
        b.AddRange(GENT.ToByte())
        'b.AddRange(GOBJ.ToByte())
        'b.AddRange(GDOR.ToByte())
        Return b
    End Function

End Class

Public Class USE
    Property EEN2 As EEN2c
    Property GENT As GENTc
    'Property GOBJ As GOBJc

    Sub New()
        EEN2 = New EEN2c()
        GENT = New GENTc()
        'GOBJ = New GOBJc()
    End Sub

    Function ToByte() As IEnumerable(Of Byte)
        Dim b = New List(Of Byte)
        b.AddRange(EEN2.ToByte())
        b.AddRange(GENT.ToByte())
        'b.AddRange(GOBJ.ToByte())
        Return b
    End Function
    End Class

Public Class WEA
    Property EEN2 As EEN2c
    Property GENT As GENTc
    Property GITM As GITMc
    'Property GIWP As GIWPc

    Sub New()
        EEN2 = New EEN2c()
        GENT = New GENTc()
        GITM = New GITMc()
        'GIWP = New GIWPc()
    End Sub

    Function ToByte() As IEnumerable(Of Byte)
        Dim b = New List(Of Byte)
        b.AddRange(EEN2.ToByte())
        b.AddRange(GENT.ToByte())
        b.AddRange(GITM.ToByte())
        'b.AddRange(GIWP.ToByte())
        Return b
    End Function

End Class

#End Region

#Region "Header Classes"

    ' Classes to hold the data in easily manipulatable format
    Public Class Trigger

        ' Triggers are made of 3 different headers (separate, unlike EME2 and EEOV), so there is a class to hold both types so they aren't separated.
        Property EMTR As EMTRc

        Property ExTR As ExTRc

        Sub New()
            EMTR = New EMTRc()
            ExTR = New ExTRc()
        End Sub

        Function ToByte() As IEnumerable(Of Byte)
            Dim b As New List(Of Byte)
            b.AddRange(EMTR.ToByte())
            b.AddRange(ExTR.ToByte())
            Return b
        End Function

    End Class

    Public Class EMAPc
        Property s1 As String
        Property s2 As String
        Property s3 As String
        Property col As Color
        Property il As Boolean
        Property le As Integer

        Sub New()
            s1 = ""
            s2 = ""
            s3 = ""
            col = Color.Black
            il = False
            le = 0
        End Sub

        Function ToByte() As IEnumerable(Of Byte)
            Dim s = 49 + s1.Length + s2.Length + s3.Length
            Dim out = New Byte(s - 1) {}
            out.Write(0, "EMAP")
            out.Write(4, If(il, 0, 5))
            out.Write(8, s)
            out.Write(16, s1.Length)
            out.Write(18, s1)
            out.Write(18 + s1.Length, s2.Length)
            out.Write(20 + s1.Length, s2)
            out.Write(20 + s1.Length + s2.Length, s3.Length)
            out.Write(22 + s1.Length + s2.Length, s3)
            out.Write(22 + s1.Length + s2.Length + s3.Length, le)
            out.Write(24 + s1.Length + s2.Length + s3.Length, col.ToByte())
            out.Write(32 + s1.Length + s2.Length + s3.Length, 1)
            Return out
        End Function

    End Class

    Public Class EME2c
        Property name As String
        Property l As Point4
        Property EEOV As EEOVc

        Sub New()
            name = ""
            l = New Point4(0, 0, 0, 0)
            EEOV = New EEOVc()
        End Sub

        Function ToByte() As IEnumerable(Of Byte)
            Dim oEEOV = EEOV.ToByte()
            Dim out = New Byte(38 + name.Length + oEEOV.Count()) {}
            out.Write(0, "EME2")
            out.Write(8, 39 + name.Length + oEEOV.Count())
            out.Write(12, name.Length)
            out.Write(14, name)
            out.Write(14 + name.Length, l.ToByte())
            out.Write(38 + name.Length, 1)
            out.Write(39 + name.Length, oEEOV)
            Return out
        End Function

    End Class

    Public Class EEOVc
        Property s1 As String
        Property s2 As String
        Property s3 As String
        Property s4 As String
        Property ps4 As Integer
        Property s5 As String
        Property inv As List(Of String)

        Sub New()
            s1 = ""
            s2 = ""
            s3 = ""
            s4 = ""
            ps4 = 0
            s5 = ""
            inv = New List(Of String)
        End Sub

        Function ToByte() As IEnumerable(Of Byte)
            Dim invl = inv.Sum(Function(e) e.Length + 2)
            Dim a = If(ps4 = 2, 2, 0)
            Dim out = New Byte(46 + s1.Length + s2.Length + s3.Length + s4.Length + s5.Length + invl + a) {}
            out.Write(0, "EEOV")
            If inv.Any() Then out.Write(4, 2)
            out.Write(8, 47 + s1.Length + s2.Length + s3.Length + s4.Length + s5.Length + invl + a)
            out.Write(12, s1.Length)
            out.Write(14, s1)
            out.Write(25 + s1.Length, s2.Length)
            out.Write(27 + s1.Length, s2)
            out.Write(27 + s1.Length + s2.Length, s3.Length)
            out.Write(29 + s1.Length + s2.Length, s3)
            out.Write(38 + s1.Length + s2.Length + s3.Length, s4.Length)
            out.Write(40 + s1.Length + s2.Length + s3.Length, s4)
            out.Write(40 + s1.Length + s2.Length + s3.Length + s4.Length, ps4)
            out.Write(41 + s1.Length + s2.Length + s3.Length + s4.Length + a, s5.Length)
            out.Write(43 + s1.Length + s2.Length + s3.Length + s4.Length + a, s5)
            out.Write(43 + s1.Length + s2.Length + s3.Length + s4.Length + s5.Length + a, inv.Count())
            Dim i = 0
            For Each e In inv
                out.Write(47 + s1.Length + s2.Length + s3.Length + s4.Length + s5.Length + a + i, e.Length)
                out.Write(49 + s1.Length + s2.Length + s3.Length + s4.Length + s5.Length + a + i, e)
                i += e.Length + 2
            Next
            Return out
        End Function

    End Class

    Public Class EMEPc
        Property index As Integer
        Property p As Point3
        Property r As Single

        Sub New()
            index = 0
            p = New Point3(0, 0, 0)
            r = 0
        End Sub

        Function ToByte() As IEnumerable(Of Byte)
            Dim out = New Byte(108) {}
            out.Write(0, "EMEP")
            out.Write(8, 109)
            out.Write(12, index)
            out.Write(73, p.ToByte())
            out.Write(105, r)
            Return out
        End Function

    End Class

    Public Class ECAMc
        Property p As Point4

        Sub New()
            p = New Point4(0, 0, 0, 0)
        End Sub

        Function ToByte() As IEnumerable(Of Byte)
            Dim out = New Byte(27) {}
            out.Write(0, "ECAM")
            out.Write(8, 28)
            out.Write(12, p.ToByte())
            Return out
        End Function

    End Class

    Public Class EMEFc
        Property s1 As String
        Property s2 As String
        Property l As Point4

        Sub New()
            s1 = ""
            s2 = ""
            l = New Point4(0, 0, 0, 0)
        End Sub

        Function ToByte() As IEnumerable(Of Byte)
            Dim out = New Byte(41 + s1.Length + s2.Length) {}
            out.Write(0, "EMEF")
            out.Write(8, 42 + s1.Length + s2.Length)
            out.Write(12, s1.Length)
            out.Write(14, s1)
            out.Write(14 + s1.Length, l.ToByte())
            out.Write(38 + s1.Length, 1)
            out.Write(39 + s1.Length, s2.Length)
            out.Write(41 + s1.Length, s2)
            out.Write(41 + s1.Length + s2.Length, 1)
            Return out
        End Function

    End Class

    Public Class EMSDc
        Property s1 As String
        Property l As Point3
        Property s2 As String

        Sub New()
            s1 = ""
            l = New Point3(0, 0, 0)
            s2 = ""
        End Sub

        Function ToByte() As IEnumerable(Of Byte)
            Dim out = New Byte(29 + s1.Length + s2.Length) {}
            out.Write(0, "EMSD")
            out.Write(8, 30 + s1.Length + s2.Length)
            out.Write(12, s1.Length)
            out.Write(14, s1)
            out.Write(14 + s1.Length, l.ToByte())
            out.Write(26 + s1.Length, s2.Length)
            out.Write(28 + s1.Length, s2)
            out.Write(28 + s1.Length + s2.Length, New Byte() {1, 1})
            Return out
        End Function

    End Class

    Public Class EPTHc
        Property name As String
        Property p As List(Of Point4)

        Sub New()
            name = ""
            p = New List(Of Point4) From {New Point4(0, 0, 0, 0)}
        End Sub

        Function ToByte() As IEnumerable(Of Byte)
            Dim out = New Byte(17 + (p.Count * 24) + name.Length) {}
            out.Write(0, "EPTH")
            out.Write(8, 18 + (p.Count * 24) + name.Length)
            out.Write(12, name.Length)
            out.Write(14, name)
            out.Write(14 + name.Length, p.Count)
            Dim i = 0
            For Each l In p
                out.Write(18 + name.Length + i, l.ToByte())
                i += 24
            Next
            Return out
        End Function

    End Class

    Public Class EMTRc
        Property n As Integer
        Property r As List(Of Point3)

        Sub New()
            n = 0
            r = New List(Of Point3) From {New Point3(0, 0, 0)}
        End Sub

        Function ToByte() As IEnumerable(Of Byte)
            Dim out = New Byte(19 + (r.Count * 12)) {}
            out.Write(0, "EMTR")
            out.Write(8, 20 + (r.Count * 12))
            out.Write(12, n)
            out.Write(16, r.Count)
            Dim i = 0
            For Each l In r
                out.Write(20 + i, l.ToByte())
                i += 12
            Next
            Return out
        End Function

    End Class

    Public Class ExTRc ' Called ExTR instead of E(T/S/B)TR for easier handling within triggers
        Property type As String ' So we know which file is being created, T, S, or B. (or M, but it's ignored if that happens)
        Property s As String

        Sub New()
            type = "T"
            s = ""
        End Sub

        Function ToByte() As IEnumerable(Of Byte)
            Select Case type
                Case "B"
                    Dim out = New Byte(18) {}
                    out.Write(0, "EBTR")
                    out.Write(8, 19)
                    out.Write(12, s)
                    out.Write(16, "FFF")
                    Return out
                Case "S"
                    Dim out = New Byte(17 + s.Length) {}
                    out.Write(0, "ESTR")
                    out.Write(8, 18 + s.Length)
                    out.Write(12, s.Length)
                    out.Write(14, s)
                    Return out
                Case "T"
                    Dim out = New Byte(15 + s.Length) {}
                    out.Write(0, "ETTR")
                    out.Write(8, 16 + s.Length)
                    out.Write(12, s.Length)
                    out.Write(14, s)
                    out.Write(14 + s.Length, New Byte() {1, 1})
                    Return out
                Case Else
                    Return Array.Empty(Of Byte)
            End Select
        End Function

    End Class

    Public Class EEN2c
        Property skl As String
        Property invtex As String
        Property acttex As String
        Property sel As Boolean
        Property EEOV As EEOVc

        Sub New()
            skl = ""
            invtex = ""
            acttex = ""
            sel = False
            EEOV = New EEOVc()
        End Sub

        Function ToByte() As IEnumerable(Of Byte)
            Dim oEEOV = EEOV.ToByte
            Dim out = New Byte(22 + oEEOV.Count() + skl.Length + invtex.Length + acttex.Length) {}
            out.Write(0, "EEN2")
            out.Write(8, 23 + oEEOV.Count() + skl.Length + invtex.Length + acttex.Length)
            out.Write(12, skl.Length)
            out.Write(14, skl)
            out.Write(14 + skl.Length, invtex.Length)
            out.Write(16 + skl.Length, invtex)
            out.Write(16 + skl.Length + invtex.Length, acttex.Length)
            out.Write(18 + skl.Length + invtex.Length, acttex)
            out.Write(19 + skl.Length + invtex.Length + acttex.Length, sel)
            out.Write(23 + skl.Length + invtex.Length + acttex.Length, oEEOV)
            Return out
        End Function

    End Class

    Public Class GENTc
        Property HoverSR As Integer ' String used when moused over
        Property LookSR As Integer ' String used when "Look" option is used
        Property NameSR As Integer ' String of the entities' name
        Property UnkwnSR As Integer ' String used ???
        Property MaxHealth As Integer
        Property StartHealth As Integer

        Sub New()
            HoverSR = 0
            LookSR = 0
            NameSR = 0
            UnkwnSR = 0
            MaxHealth = 0
            StartHealth = 0
        End Sub

        Function ToByte() As IEnumerable(Of Byte)
            Dim out = New Byte(43) {}
            out.Write(0, "GENT")
            out.Write(4, 1)
            out.Write(8, 44)
            out.Write(12, HoverSR)
            out.Write(16, LookSR)
            out.Write(20, NameSR)
            out.Write(24, UnkwnSR)
            out.Write(36, MaxHealth)
            out.Write(40, StartHealth)
            Return out
        End Function

    End Class

    Public Class GCREc
        Property Special As Integer()
        Property Age As Integer
        Property Skills As List(Of Skill)
        Property Traits As List(Of Integer)
        Property TagSkills As List(Of Integer)
        Property PortStr As String
        Property Hea As Socket
        Property Hai As Socket
        Property Pon As Socket
        Property Mus As Socket
        Property Bea As Socket
        Property Eye As Socket
        Property Bod As Socket
        Property Han As Socket
        Property Fee As Socket
        Property Bac As Socket
        Property Sho As Socket
        Property Van As Socket
        Property Inventory As List(Of String)
        Property GWAM As List(Of GWAMc)

        Sub New()
            Special = New Integer() {0, 0, 0, 0, 0, 0, 0}
            Age = 0
            Skills = New List(Of Skill)
            Traits = New List(Of Integer)
            TagSkills = New List(Of Integer)
            PortStr = ""
            Hea = New Socket("", "")
            Hai = New Socket("", "")
            Pon = New Socket("", "")
            Mus = New Socket("", "")
            Bea = New Socket("", "")
            Eye = New Socket("", "")
            Bod = New Socket("", "")
            Han = New Socket("", "")
            Fee = New Socket("", "")
            Bac = New Socket("", "")
            Sho = New Socket("", "")
            Van = New Socket("", "")
            Inventory = New List(Of String)()
            GWAM = New List(Of GWAMc)
        End Sub

        Function ToByte() As IEnumerable(Of Byte)
            Dim GWAMb = New List(Of Byte)
            For Each g In GWAM
                GWAMb.AddRange(g.ToByte())
            Next
            Dim socs = New Socket() {Hea, Hai, Pon, Mus, Bea, Eye, Bod, Han, Fee, Bac, Sho, Van}
            Dim sock = New List(Of Byte)
            For Each s In socs
                sock.AddRange(s.ToByte())
            Next
            Dim inv = New List(Of Byte)
            For Each i In Inventory
                inv.AddRange(New Byte() {i.Length, 0})
                inv.AddRange(Encoding.ASCII.GetBytes(i))
            Next

            Dim sl = Skills.Count * 8 ' Skills length
            Dim tl = Traits.Count * 4 ' Traits length
            Dim tsl = TagSkills.Count * 4 ' Tag Skills length
            Dim il = Inventory.Sum(Function(i) i.Length + 2) ' Inventory Length
            Dim socl = Hea.Length + Hai.Length + Pon.Length + Mus.Length + Bea.Length + Eye.Length + Bod.Length + Han.Length +
                       Fee.Length + Bac.Length + Sho.Length + Van.Length ' Socket String Length
            Dim TDL = sl + tl + tsl + il + GWAMb.Count + PortStr.Length + socl ' Total Dynamic Length

            Dim out = New Byte(276 + TDL) {}
            out.Write(0, "GCRE")
            out.Write(4, 4)
            out.Write(8, 277 + TDL)
            out.Write(12, Special(0))
            out.Write(16, Special(1))
            out.Write(20, Special(2))
            out.Write(24, Special(3))
            out.Write(28, Special(4))
            out.Write(32, Special(5))
            out.Write(36, Special(6))
            out.Write(56, Age)
            out.Write(72, Skills.Count)
            For i = 0 To Skills.Count - 1
                out.Write(76 + (i * 8), Skills(i).ToByte())
            Next
            out.Write(80 + sl, Traits.Count)
            For i = 0 To Traits.Count - 1
                out.Write(84 + sl + (i * 4), Traits(i))
            Next
            out.Write(84 + sl + tl, TagSkills.Count)
            For i = 0 To TagSkills.Count - 1
                out.Write(88 + sl + tl + (i * 4), TagSkills(i))
            Next
            out.Write(92 + sl + tl + tsl, PortStr.Length)
            out.Write(94 + sl + tl + tsl, PortStr)
            out.Write(129 + sl + tl + tsl + PortStr.Length, sock)
            out.Write(189 + sl + tl + tsl + PortStr.Length + socl, GWAM.Count)
            out.Write(273 + sl + tl + tsl + PortStr.Length + socl, Inventory.Count())
            out.Write(277 + sl + tl + tsl + PortStr.Length + socl, inv)
            out.Write(277 + sl + tl + tsl + PortStr.Length + socl + il, GWAMb)
            Return out
        End Function

    End Class

    Public Class GWAMc
        Property Anim As Integer
        Property DmgType As Integer
        Property ShotsFired As Integer
        Property Range As Integer
        Property MinDmg As Integer
        Property MaxDmg As Integer
        Property AP As Integer
        Property NameSR As Integer
        Property VegName As String

        Sub New()
            Anim = 0
            DmgType = 0
            ShotsFired = 0
            Range = 0
            MinDmg = 0
            MaxDmg = 0
            AP = 0
            NameSR = 0
            VegName = 0
        End Sub

        Function ToByte() As IEnumerable(Of Byte)
            Dim out = New Byte(79 + VegName.Length) {}
            out.Write(0, "GWAM")
            out.Write(4, 5)
            out.Write(8, 80 + VegName.Length)
            out.Write(12, Anim)
            out.Write(16, DmgType)
            out.Write(20, ShotsFired)
            out.Write(36, Range)
            out.Write(48, MinDmg)
            out.Write(52, MaxDmg)
            out.Write(62, AP)
            out.Write(72, NameSR)
            out.Write(78, VegName)
            out.Write(out.Length - 2, 1)
            Return out
        End Function

    End Class

    Public Class GCHRc
        Property name As String

        Sub New()
            name = ""
        End Sub

        Function ToByte() As IEnumerable(Of Byte)
            Dim out = New Byte(13 + name.Length) {}
            out.Write(0, "GCHR")
            out.Write(8, 14 + name.Length)
            out.Write(12, name.Length)
            out.Write(14, name)
            Return out
        End Function

    End Class

    Public Class _2MWTc
        Property mpf As String
        Property frozen As Boolean
        Property dark As Boolean
        Property chunks As List(Of _2MWTChunk)

        Sub New()
            mpf = ""
            chunks = New List(Of _2MWTChunk)
        End Sub

        Function ToByte() As IEnumerable(Of Byte)
            Dim sl = chunks.Sum(Function(x) x.tex.Length) + mpf.Length ' length of strings
            Dim wl = chunks.Count * 22 ' added length for each water chunk

            Dim out = New Byte(157 + sl + wl) {}
            out.Write(0, "2MWT")
            out.Write(8, 158 + sl + wl)
            out.Write(12, mpf.Length)
            out.Write(14, mpf)
            out.Write(27 + mpf.Length, Not dark)
            out.Write(29 + mpf.Length, Not frozen)
            out.Write(154 + mpf.Length, chunks.Count)
            Dim io = 158 + mpf.Length
            For Each w In chunks
                out.Write(io, w.ToByte())
                io += 22 + w.tex.Length
            Next
            Return out
        End Function

    End Class

    Public Class GITMc            '"Missing" Sockets:
        Property type As Integer  '? Hai
        Property equip As Boolean '? Pon
        Property eqslot As Integer '? Mus
        Property Hea As Socket    '? Bea
        Property hHai As Boolean
        Property hBea As Boolean
        Property hMus As Boolean
        Property hEye As Boolean
        Property hPon As Boolean
        Property hVan As Boolean
        Property Eye As Socket
        Property Bod As Socket
        Property Bac As Socket
        Property Han As Socket
        Property Fee As Socket
        Property Sho As Socket
        Property Van As Socket
        Property IHS As Socket
        Property reload As Integer

        Sub New()
            type = 0
            equip = False
            eqslot = 0
            Hea = New Socket("", "")
            hHai = False
            hBea = False
            hMus = False
            hEye = False
            hPon = False
            hVan = False
            Eye = New Socket("", "")
            Bod = New Socket("", "")
            Bac = New Socket("", "")
            Han = New Socket("", "")
            Fee = New Socket("", "")
            Sho = New Socket("", "")
            Van = New Socket("", "")
            IHS = New Socket("", "")
            reload = 0
        End Sub

        Function ToByte() As IEnumerable(Of Byte)
            Dim socl = Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length + Fee.Length + Sho.Length + Van.Length + IHS.Length
            Dim out = New Byte(95 + socl) {}
            out.Write(0, "GITM")
            out.Write(4, 1)
            out.Write(8, 96 + socl)
            out.Write(12, type)
            out.Write(20, equip)
            out.Write(24, eqslot)
            out.Write(28, Hea.ToByte())
            out.Write(32 + Hea.Length, hHai)
            out.Write(33 + Hea.Length, hBea)
            out.Write(34 + Hea.Length, hMus)
            out.Write(35 + Hea.Length, hEye)
            out.Write(36 + Hea.Length, hPon)
            out.Write(37 + Hea.Length, hVan)
            out.Write(41 + Hea.Length, 1)
            out.Write(46 + Hea.Length, 1)
            out.Write(51 + Hea.Length, 1)
            out.Write(52 + Hea.Length, Eye.ToByte())
            out.Write(56 + Hea.Length + Eye.Length, 1)
            out.Write(61 + Hea.Length + Eye.Length, 1)
            out.Write(62 + Hea.Length + Eye.Length, Bod.ToByte())
            out.Write(66 + Hea.Length + Eye.Length + Bod.Length, Bac.ToByte())
            out.Write(70 + Hea.Length + Eye.Length + Bod.Length + Bac.Length, Han.ToByte())
            out.Write(75 + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length, Fee.ToByte())
            out.Write(79 + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length + Fee.Length, Sho.ToByte())
            out.Write(83 + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length + Fee.Length + Sho.Length, Van.ToByte())
            out.Write(87 + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length + Fee.Length + Sho.Length + Van.Length, 1)
            out.Write(88 + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length + Fee.Length + Sho.Length + Van.Length, IHS.ToByte())
            out.Write(92 + Hea.Length + Eye.Length + Bod.Length + Bac.Length + Han.Length + Fee.Length + Sho.Length + Van.Length + IHS.Length, reload)
            Return out
        End Function

    End Class

    Public Class GIARc
        Property BallR As Integer
        Property BioR As Integer
        Property ElecR As Integer
        Property EMPR As Integer
        Property NormR As Integer
        Property HeatR As Integer

        Sub New()
            BallR = 0
            BioR = 0
            ElecR = 0
            EMPR = 0
            NormR = 0
            HeatR = 0
        End Sub

        Function ToByte() As IEnumerable(Of Byte)
            Dim out = New Byte(39) {}
            out.Write(0, "GIAR")
            out.Write(8, 40)
            out.Write(12, BallR)
            out.Write(16, BioR)
            out.Write(20, ElecR)
            out.Write(24, EMPR)
            out.Write(28, NormR)
            out.Write(32, HeatR)
            Return out
        End Function

    End Class

#End Region

#Region "Other Classes"

    Public Class Point2
        Property x As Single
        Property y As Single

        Sub New(x As Single, y As Single)
            Me.x = x
            Me.y = y
        End Sub

        Function ToByte() As IEnumerable(Of Byte)
            Dim out = New Byte(7) {}
            out.Write(0, x)
            out.Write(4, y)
            Return out
        End Function

    End Class

    Public Class Point3
        Property x As Single
        Property z As Single
        Property y As Single

        Sub New(x As Single, z As Single, y As Single)
            Me.x = x
            Me.z = z
            Me.y = y
        End Sub

        Function ToByte() As IEnumerable(Of Byte)
            Dim out = New Byte(11) {}
            out.Write(0, x)
            out.Write(4, z)
            out.Write(8, y)
            Return out
        End Function

    End Class

    Public Class Point4
        Property x As Single
        Property z As Single
        Property y As Single
        Property r As Single

        Sub New(x As Single, z As Single, y As Single, r As Single)
            Me.x = x
            Me.z = z
            Me.y = y
            Me.r = r
        End Sub

        Function ToByte() As IEnumerable(Of Byte)
            Dim out = New Byte(15) {}
            out.Write(0, x)
            out.Write(4, z)
            out.Write(8, y)
            out.Write(12, r)
            Return out
        End Function

    End Class

    Public Class Skill
        Property Index As Integer
        Property Value As Integer

        Sub New(Index As Integer, Value As Integer)
            Me.Index = Index
            Me.Value = Value
        End Sub

        Function ToByte() As IEnumerable(Of Byte)
            Dim out = New Byte(7) {}
            out.Write(0, Index)
            out.Write(4, Value)
            Return out
        End Function

    End Class

    Public Class Socket
        Property Model As String
        Property Tex As String

        ''' <summary>
        ''' Gets the length of both strings
        ''' </summary>
        ReadOnly Property Length As Integer
            Get
                Return Model.Length + Tex.Length
            End Get
        End Property

        Sub New(Model As String, Tex As String)
            Me.Model = Model
            Me.Tex = Tex
        End Sub

        Function ToByte() As IEnumerable(Of Byte)
            Dim out = New Byte(3 + Model.Length + Tex.Length) {}
            out.Write(0, Model.Length)
            out.Write(2, Model)
            out.Write(2 + Model.Length, Tex.Length)
            out.Write(4 + Model.Length, Tex)
            Return out
        End Function

    End Class

    Public Class _2MWTChunk
        Property tex As String
        Property loc As Point3
        Property texloc As Point2

        Sub New(tex As String, loc As Point3, texloc As Point2)
            Me.tex = tex
            Me.loc = loc
            Me.texloc = texloc
        End Sub

        Function ToByte() As IEnumerable(Of Byte)
            Dim out = New Byte(21 + tex.Length) {}
            out.Write(0, loc.x)
            out.Write(4, loc.z)
            out.Write(8, loc.y)
            out.Write(12, tex.Length)
            out.Write(14, tex)
            out.Write(14 + tex.Length, texloc.ToByte())
            Return out
        End Function

    End Class


#End Region