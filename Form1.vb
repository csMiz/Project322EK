Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Threading.Tasks

Public Class Form1




    Public MultiEndFlag As Integer = -1


    Public midfilename As String

    Public selecttype As Integer = 0    '0-empty 1-note 2-track
    Private selectednote As clsNote = Nothing
    Private selectedtrack As Integer = -1

    Public selectedws As Short = -1

    Const fps60 As Double = 50 / 3
    Const fps45 As Double = 200 / 9

    Public PrintPage As Integer = 0

    Private font1 As Font
    Private font2 As Font
    Private brush1 As SolidBrush
    Private pen1 As Pen
    Private pen2 As Pen
    Public BlueBrush As SolidBrush
    Public GreenBrush As SolidBrush
    Public RedBrush As SolidBrush
    Public VioletBrush As SolidBrush
    Public CfbPen As Pen
    Public PurplePen As Pen



    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Me.Size = New Size(950, 600)

        For i = 0 To 15
            GlobalTrackVol(i) = 128
        Next

        InstrumentTag(0) = 2
        InstrumentTag(1) = 3
        InstrumentTag(2) = 2
        InstrumentTag(3) = 3
        InstrumentTag(4) = 4
        InstrumentTag(5) = 1
        InstrumentTag(6) = 4
        InstrumentTag(7) = 1
        InstrumentTag(8) = 4
        InstrumentTag(9) = 0
        InstrumentTag(10) = 2
        InstrumentTag(11) = 4
        InstrumentTag(12) = 1
        InstrumentTag(13) = 3
        InstrumentTag(14) = 4
        InstrumentTag(15) = 3
        For Each co As Control In Me.Controls
            If TypeOf (co) Is TextBox Then
                If CType(co, TextBox).Tag > 0 Then
                    CType(co, TextBox).Text = "19"
                End If
            End If
        Next

        Call LoadWS()

        For i = 0 To 15
            TrackRunningListL(i) = New List(Of clsNoteEx)
            TrackRunningListR(i) = New List(Of clsNoteEx)
            ChangeInst(i) = New List(Of clsChangeInst)
        Next

        font1 = New Font("Microsoft YaHei", 18)
        font2 = New Font("Microsoft YaHei", 14)
        brush1 = New SolidBrush(Color.Black)
        pen1 = New Pen(Color.Blue)
        pen2 = New Pen(Color.Red)
        BlueBrush = New SolidBrush(Color.Blue)
        GreenBrush = New SolidBrush(Color.LightGreen)
        RedBrush = New SolidBrush(Color.Red)
        VioletBrush = New SolidBrush(Color.Violet)
        CfbPen = New Pen(Color.CornflowerBlue)
        PurplePen = New Pen(Color.Purple)



    End Sub

    Public Sub PostMsg(msg As String)
        tbmsg.Text = tbmsg.Text & msg & vbCrLf
        Application.DoEvents()
    End Sub

    Private Sub Btn1_Click(sender As Object, e As EventArgs) Handles Btn1.Click

        Dim openFile As New OpenFileDialog
        openFile.Filter = "midi文件|*.mid"
        openFile.Title = "打开mid文件"
        openFile.AddExtension = True
        openFile.AutoUpgradeEnabled = True
        openFile.InitialDirectory = "D:\Music\"
        If openFile.ShowDialog() = DialogResult.OK Then
            midfilename = openFile.SafeFileName
            midfilename = midfilename.Remove(midfilename.Length - 4)
            Dim tstr As FileStream = openFile.OpenFile
            Dim r As BinaryReader = New BinaryReader(tstr)
            '正在读取

            Dim tempcontent As Byte = 0
            Dim jmax As Integer = r.BaseStream.Length \ 80

            For j = 0 To jmax
                Dim tcline As String = ""
                For i = 0 To 79
                    If j = jmax Then
                        If jmax * 80 + i >= r.BaseStream.Length Then Exit For
                    End If
                    tempcontent = r.ReadByte()
                    tcline = tcline & CHex(tempcontent) & "/"
                Next
                list1.Add(tcline)
            Next

            r.Close()
            '读取完成，分析中

            For i = 0 To 15
                track(i) = New List(Of clsNote)
                trackmood(i) = 80
                trackpos(i) = 64
                controller(i) = New List(Of clsController)
            Next

            trackpos(0) = HEXByte("40")
            trackpos(1) = HEXByte("46")
            trackpos(2) = HEXByte("48")
            trackpos(3) = HEXByte("34")
            trackpos(4) = HEXByte("1E")
            trackpos(5) = HEXByte("40")
            trackpos(6) = HEXByte("5A")
            trackpos(7) = HEXByte("3A")
            trackpos(8) = HEXByte("4B")
            trackpos(9) = HEXByte("40")
            trackpos(10) = HEXByte("39")
            trackpos(11) = HEXByte("50")
            trackpos(12) = HEXByte("32")
            trackpos(13) = HEXByte("40")
            trackpos(14) = HEXByte("5A")
            trackpos(15) = HEXByte("28")

            playingspeed = HEXByte(list1(0).Substring(36, 2)) * 256 + HEXByte(list1(0).Substring(39, 2))

            Call GetNotes()

            Call PrintNotes()  '某一首曲子元素太多，先不显示

            Dim memax As Integer = 0
            For i = 0 To 15
                If track(i).Count <> 0 Then
                    Dim ttr As Integer = track(i).Count - 1
                    If track(i)(ttr).startat + track(i)(ttr).duration > memax Then
                        memax = track(i)(ttr).startat + track(i)(ttr).duration
                    End If
                End If
            Next
            musicend = memax

            Dim HaveErrNotes As Integer = 0
            For i = 0 To 15
                If track(i).Count Then
                    For Each tn As clsNote In track(i)
                        If tn.status <> 2 Then
                            HaveErrNotes += 1
                        End If
                    Next
                End If
            Next
            If HaveErrNotes Then
                PostMsg("警告：存在" & HaveErrNotes.ToString & "个不完整的音符")
            End If


        End If



    End Sub

    Public Sub GetNotes()

        Dim tempsearchstart As Integer = 0

STL:
        Dim pos As Integer = GetFirstNotePos(list1, tempsearchstart)

        Dim ttr As Byte = 0
        Dim tst As Integer = 0
        Dim tpit As Byte = 0
        Dim tvol As Byte = 0

        Dim is9 As Short = 0


        Do     '未完成，先不要无限循环//已经补上了，等待测试

            Dim tsp As Integer = 0
            Dim tbc As Byte = 1
            tsp = GetTimeSpan(list1, pos, tbc)
            tst += tsp
            pos += tbc

            If BGetByte(list1, pos).Substring(0, 1) = "9" Then
                ttr = GetTrack(BGetByte(list1, pos))
                pos += 1
                tpit = HEXByte(BGetByte(list1, pos))
                pos += 1
                tvol = HEXByte(BGetByte(list1, pos))
                pos += 1
                is9 = 1
            ElseIf BGetByte(list1, pos).Substring(0, 1) = "8" Then
                ttr = GetTrack(BGetByte(list1, pos))
                pos += 1
                tpit = HEXByte(BGetByte(list1, pos))
                pos += 1
                tvol = HEXByte(BGetByte(list1, pos))
                pos += 1
                is9 = 2
            ElseIf BGetByte(list1, pos) = "FF" Then
                If BGetByte(list1, pos + 1) = "51" Then
                    If BGetByte(list1, pos + 2) = "03" Then
                        pos += 6    '跳过ff速度码
                        Continue Do
                    End If
                ElseIf BGetByte(list1, pos + 1) = "01" Then '跳过ff01的描述
                    Dim tlen01 As Integer = HEXByte(BGetByte(list1, pos + 2))
                    '可能存在超过一位的时间argument  <-存在bug
                    pos += (3 + tlen01)
                    Continue Do
                ElseIf BGetByte(list1, pos + 1) = "2F" Then
                    '结束
                    '存在bug -> 有些音乐的写法不同，存在多个 ff 2f

                    If ((list1.Count - 1) * 80 + (list1(list1.Count - 1).Length) / 3) - pos - 1 > 10 Then
                        '后面可能还有内容
                        MultiEndFlag = pos + 2
                    Else
                        MultiEndFlag = -1
                    End If
                    Exit Do


                ElseIf BGetByte(list1, pos + 1) = "58" Then
                    '节拍控制
                    pos += 7    '意味不明
                    '节拍控制主要是给鼓声作参考的，暂时用不上
                    Continue Do
                Else


                End If
            ElseIf BGetByte(list1, pos).Substring(0, 1) = "B" Then
                If BGetByte(list1, pos + 1) = "0B" Then '情绪控制器
                    ttr = GetTrack(BGetByte(list1, pos))
                    Dim ttarget As Byte = HEXByte(BGetByte(list1, pos + 1))
                    Dim tconvalue As Byte = HEXByte(BGetByte(list1, pos + 2))

                    Dim tcon As New clsController(tst, ttarget, tconvalue)
                    controller(ttr).Add(tcon)

                    pos += 3
                    Continue Do

                Else
                    '跳过别的控制器
                    '可能有别的控制器不止3个参数<-存在bug

                    pos += 3
                    Continue Do
                End If
            ElseIf BGetByte(list1, pos).Substring(0, 1) = "C" Then  '改变乐器
                ttr = GetTrack(BGetByte(list1, pos))
                Dim tci As New clsChangeInst(tst, HEXByte(BGetByte(list1, pos + 1)))
                ChangeInst(ttr).Add(tci)

                pos += 2
                Continue Do
            Else
                pos += 3    '跳过滑音
                Continue Do
            End If


            If is9 = 1 Then
                If tvol <> 0 Then   '按下
                    Dim tnote As New clsNote(tpit, tvol, tst)
                    track(ttr).Add(tnote)
                Else    '松开
                    Dim jm As Integer = track(ttr).Count - 1
                    For j = jm To 0 Step -1
                        Dim tn As clsNote = track(ttr)(j)
                        If tn.status = 1 Then
                            If tn.pitch = tpit Then
                                tn.duration = tst - tn.startat
                                tn.status = 2
                            End If
                        End If
                    Next
                End If
            ElseIf is9 = 2 Then
                Dim jm As Integer = track(ttr).Count - 1
                For j = jm To 0 Step -1
                    Dim tn As clsNote = track(ttr)(j)
                    If tn.status = 1 Then
                        If tn.pitch = tpit Then
                            tn.duration = tst - tn.startat
                            tn.status = 2
                        End If
                    End If
                Next
            End If

        Loop

        If MultiEndFlag <> -1 Then
            tempsearchstart = MultiEndFlag
            GoTo STL

        End If

    End Sub

    Public Sub PrintNotes()

        P.Image = Nothing
        Dim bm As New Bitmap(1500, 700)
        Dim G As Graphics = Graphics.FromImage(bm)
        G.Clear(Color.White)

        Dim stx As Integer = PrintPage * 1000
        G.DrawString(stx.ToString, font1, brush1, New PointF(0, 10))
        G.DrawString((stx + 1000).ToString, font1, brush1, New PointF(500, 10))
        G.DrawString((stx + 2000).ToString, font1, brush1, New PointF(1000, 10))

        For i = 0 To 15
            If track(i).Count Then
                Dim jm As Integer = track(i).Count - 1
                For j = 0 To jm
                    Dim tn As clsNote = track(i)(j)
                    If tn.status Then
                        If (tn.startat + tn.duration) > stx Then
                            If tn.startat < (stx + 3000) Then
                                Dim tcv As Byte = 255 - tn.volume
                                Dim tbrush As New SolidBrush(Color.FromArgb(255, tcv, tcv, tcv))
                                G.FillRectangle(tbrush, New RectangleF((tn.startat - stx) * 0.5, 100 + i * 35, tn.duration * 0.5, 30))
                                G.DrawString(PrintPitch(tn.pitch), font2, brush1, (tn.startat - stx) * 0.5, 100 + i * 35)
                            Else
                                Exit For
                            End If
                        End If
                    End If
                Next
            End If
        Next
        For i = 0 To 15
            If ChangeInst(i).Count Then
                Dim jm As Integer = ChangeInst(i).Count - 1
                For j = 0 To jm
                    Dim tci As clsChangeInst = ChangeInst(i)(j)
                    If tci.StartAt > stx AndAlso tci.StartAt < (stx + 3000) Then
                        G.DrawString(j, font2, Brushes.Red, (tci.StartAt - stx) * 0.5, 100 + i * 35)
                    End If
                Next
            End If
        Next
        For i = 0 To 16
            If i = 4 Or i = 9 Or i = 14 Then
                G.DrawLine(pen2, 0, 100 + i * 35, 1500, 100 + i * 35)
            Else
                G.DrawLine(pen1, 0, 100 + i * 35, 1500, 100 + i * 35)
            End If
        Next

        P.Image = bm
        P.Refresh()
        G.Dispose()

    End Sub

    Private Sub Btn2_Click(sender As Object, e As EventArgs) Handles Btn2.Click

        Dim saveFile As New SaveFileDialog
        saveFile.Filter = "Wav文件|*.wav"
        saveFile.Title = "生成音乐"
        saveFile.AddExtension = True
        saveFile.AutoUpgradeEnabled = True
        saveFile.InitialDirectory = "C:\Users\sscs\Desktop\"
        saveFile.FileName = midfilename
        If saveFile.ShowDialog() = DialogResult.OK Then
            Dim tstream As FileStream = saveFile.OpenFile
            Dim r As BinaryWriter = New BinaryWriter(tstream)
            PostMsg("开始写入" & saveFile.FileName)

            For Each co As Control In Me.Controls
                If TypeOf (co) Is TextBox Then
                    If CType(co, TextBox).Tag > 0 Then
                        InstrumentTag(CType(co, TextBox).Tag - 1) = CInt(CType(co, TextBox).Text)
                    End If
                End If
            Next

            For i = 0 To 15
                If track(i).Count Then
                    For j = 0 To track(i).Count - 1
                        track(i)(j).playingstatus = 0
                    Next
                End If
            Next
            'MidC = 131

            Dim tempcontent As Byte = 0

            Dim SR As Integer = 24000
            Dim wbuffer(16383) As Byte
            '可以控制播放速度
            SamplePerT = Convert.ToInt32(SR / (playingspeed / basicT))
            Dim Tw As Integer = 0   't for write
            Dim TwTrim As Double = 0
            Dim Nw As Integer = 0
            Dim SampleCount As Long = 0
            Dim UIrefCount As Long = 0

            Dim len1 As Integer = WavHead(wbuffer)
            r.Write(wbuffer, 0, len1)

            Do
                Dim tbufL As Double = 0
                Dim tbufR As Double = 0
                TwTrim = SampleCount / SamplePerT

                For i = 0 To 15
                    If controller(i).Count >= 1 Then    '控制器
                        For j = 0 To controller(i).Count - 1
                            If controller(i)(j).status = 0 AndAlso Tw >= controller(i)(j).startat Then
                                If controller(i)(j).target = 11 Then    '情绪控制器
                                    trackmood(i) = controller(i)(j).value
                                ElseIf controller(i)(j).target = 10 Then    '声像控制器
                                    trackpos(i) = controller(i)(j).value
                                End If
                                controller(i)(j).status = 1
                            End If
                        Next
                    End If

                    '计算左右声道延时
                    Dim theta As Double = (127 - trackpos(i)) * Math.PI / 127
                    LOffset(i) = (((r2 * Math.Cos(theta) + r1) ^ 2 + (r2 * Math.Sin(theta)) ^ 2) ^ 0.5) / 340
                    ROffset(i) = (((r2 * Math.Cos(theta) - r1) ^ 2 + (r2 * Math.Sin(theta)) ^ 2) ^ 0.5) / 340
                    LOffsetSample(i) = LOffset(i) * SR
                    ROffsetSample(i) = ROffset(i) * SR

                    If track(i).Count >= 1 Then
                        For j = 0 To track(i).Count - 1
                            If (track(i)(j).playingstatus = 0 Or track(i)(j).playingstatus = 2) AndAlso TwTrim - (LOffsetSample(i) / SamplePerT) >= track(i)(j).startat Then
                                track(i)(j).playingstatus += 1
                                TrackRunningListL(i).Add(track(i)(j).AddAmpControl(GetInst(InstrumentTag(i))))  '参数1表示非打击乐器，详见类定义
                            End If
                            If (track(i)(j).playingstatus = 0 Or track(i)(j).playingstatus = 1) AndAlso TwTrim - (ROffsetSample(i) / SamplePerT) >= track(i)(j).startat Then
                                track(i)(j).playingstatus += 2
                                TrackRunningListR(i).Add(track(i)(j).AddAmpControl(GetInst(InstrumentTag(i))))
                            End If

                        Next
                    End If


                Next

                '得到一个合成后的音
                '再进行samplepert次取样
                For i = 0 To SamplePerT - 1
                    '------------------------------------left-------------------------------
                    tbufL = 0
                    For j = 0 To 15 'j->tracks
                        If TrackRunningListL(j).Count Then
                            Dim ttrllmax As Short = TrackRunningListL(j).Count - 1
                            For k = ttrllmax To 0 Step -1
                                Dim tnex As clsNoteEx = TrackRunningListL(j)(k)
                                '取消对预设乐器的调用支持
                                '利用tnex.getamp加入包络控制
                                With tnex
                                    tbufL += FxWS(.playingcur, .GetAmp(SamplePerT) * GetFinalVolume(tnex, j, 0), .pitch, SR, Tw - .startat, .duration, InstrumentTag(j))

                                    .playingcur += 1
                                    If .playingcur > ((.AttackTime + .DecayTime + .SustainTime + .ReleaseTime) * SamplePerT) Then
                                        TrackRunningListL(j).Remove(tnex)
                                    End If

                                End With
                            Next

                        Else
                            'tbuf no change
                        End If
                    Next
                    If tbufL > 1 Then tbufL = 1
                    If tbufL < -1 Then tbufL = -1

                    wbuffer(Nw * 4 + 1) = DoubleTo2Bytes(tbufL, 1)
                    wbuffer(Nw * 4) = DoubleTo2Bytes(tbufL, 2)

                    '--------------------------------------right---------------------------------
                    tbufR = 0
                    For j = 0 To 15 'j->tracks
                        If TrackRunningListR(j).Count Then

                            Dim ttrlrmax As Short = TrackRunningListR(j).Count - 1
                            For k = ttrlrmax To 0 Step -1
                                Dim tnex As clsNoteEx = TrackRunningListR(j)(k)
                                With tnex
                                    tbufR += FxWS(.playingcur, .GetAmp(SamplePerT) * GetFinalVolume(tnex, j, 0), .pitch, SR, Tw - .startat, .duration, InstrumentTag(j))

                                    .playingcur += 1
                                    If .playingcur > ((.AttackTime + .DecayTime + .SustainTime + .ReleaseTime) * SamplePerT) Then
                                        TrackRunningListR(j).Remove(tnex)
                                    End If

                                End With
                            Next

                        Else
                            'tbuf no change
                        End If
                    Next
                    If tbufR > 1 Then tbufR = 1
                    If tbufR < -1 Then tbufR = -1

                    wbuffer(Nw * 4 + 3) = DoubleTo2Bytes(tbufR, 1)
                    wbuffer(Nw * 4 + 2) = DoubleTo2Bytes(tbufR, 2)

                    '----------------------------------------------------------------------
                    Nw += 1
                    SampleCount += 1
                    TwTrim = SampleCount / SamplePerT
                    If Nw >= 4096 Then
                        Nw = 0
                        r.Write(wbuffer, 0, 16384)

                        UIrefCount += 1
                        If UIrefCount >= 16 Then
                            Me.Text = "Mid Viewer(Win32) - " & CInt(TwTrim * 100 / (musicend + 150)).ToString & "%"
                            Application.DoEvents()

                            UIrefCount = 0
                        End If
                    End If

                Next

                If (TwTrim - ((r1 + r2) * SR / (340 * SamplePerT))) > (musicend + 75) Then
                    '此处的75是ReleaseTime
                    '暂时没有找到更好的处理方法
                    Exit Do
                End If
                Tw += 1

            Loop
            If Nw >= 1 Then
                r.Write(wbuffer, 0, Nw * 4)
            End If

            Call IntTo4Bytes(4 * SampleCount, wbuffer)
            r.Seek(40, SeekOrigin.Begin)
            r.Write(wbuffer, 0, 4)

            Call IntTo4Bytes(4 * SampleCount + 36, wbuffer)
            r.Seek(4, SeekOrigin.Begin)
            r.Write(wbuffer, 0, 4)


            r.Close()

            tstream.Close()
            PostMsg("写入完成")
            Me.Text = "Mid Viewer(Win32)"
        End If

    End Sub

    Public Sub LoadInstConfig()
        For Each co As Control In Me.Controls
            If TypeOf (co) Is TextBox Then
                If CType(co, TextBox).Tag > 0 Then
                    InstrumentTag(CType(co, TextBox).Tag - 1) = CInt(CType(co, TextBox).Text)
                End If
            End If
        Next

        For i = 0 To 15
            If track(i).Count Then
                For j = 0 To track(i).Count - 1
                    track(i)(j).playingstatus = 0
                Next
            End If
        Next
    End Sub



    Private Async Sub tbcont_KeyPress(sender As Object, e As KeyPressEventArgs) Handles tbcont.KeyPress
        If e.KeyChar = ChrW(13) Then
            Dim tin As String = tbcont.Text.Trim
            PostMsg(tin)
            tbcont.Text = ""
            Dim tst() As String = Regex.Split(tin, " ")
            If tst.Length >= 2 Then
                Dim tcmd As String = tst(0)
                If tcmd.Contains("showwave") OrElse tcmd = "ws" Then
                    If tst.Length = 2 Then
                        Dim tindex As Short = CInt(tst(1))
                        selectedws = tindex
                        Call PaintWS(tindex)
                    End If
                ElseIf tcmd.Contains("phi") Then
                    Dim tv As Short = CInt(tst(1))
                    If selectedws <> -1 Then
                        Call MovePhi(tv)
                        Call FilterApply(ListWS(selectedws))
                        Call PaintWS(selectedws)
                    End If
                ElseIf tcmd.Contains("fm") Then
                    If tst.Length = 2 Then
                        Dim tv As Short = CInt(tst(1))
                        If selectedws <> -1 Then
                            Call FMApply(tv, 50)
                            Call FilterApply(ListWS(selectedws))
                            Call PaintWS(selectedws)
                        End If
                    ElseIf tst.Length = 3 Then
                        Dim tv As Short = CInt(tst(1))
                        Dim tv2 As Short = CInt(tst(2))
                        If selectedws <> -1 Then
                            Call FMApply(tv, tv2)
                            Call FilterApply(ListWS(selectedws))
                            Call PaintWS(selectedws)
                        End If
                    End If
                ElseIf tcmd.Contains("setall") Then
                    For Each co As Control In Me.Controls
                        If TypeOf (co) Is TextBox Then
                            If CType(co, TextBox).Tag > 0 Then
                                CType(co, TextBox).Text = tst(1)
                            End If
                        End If
                    Next
                ElseIf tcmd.Contains("setmidc") Then
                    MidC = CDbl(tst(1))
                ElseIf tcmd = "set" Then
                    If tst.Length = 3 Then
                        For Each co As Control In Me.Controls
                            If TypeOf (co) Is TextBox Then
                                If CType(co, TextBox).Tag = tst(1) Then
                                    CType(co, TextBox).Text = tst(2)
                                End If
                            End If
                        Next
                    End If
                ElseIf tcmd.Contains("setspeed") Then
                    basicT = 60.0 / CDbl(CInt(tst(1)))
                ElseIf tcmd = "cut" Then
                    If tst.Length = 4 Then
                        Dim ttr As Short = CInt(tst(1))
                        Dim stt As Integer = CInt(tst(2))
                        Dim tar As Short = CInt(tst(3))
                        Dim temp As New List(Of clsNote)

                        Dim trmax As Integer = track(ttr).Count - 1
                        For i = trmax To 0 Step -1
                            Dim tn As clsNote = track(ttr)(i)
                            If tn.startat >= stt Then
                                track(ttr).Remove(tn)
                                temp.Add(tn)
                            End If
                        Next

                        temp.Reverse()
                        track(tar).AddRange(temp)
                        Call PrintNotes()

                    End If
                Else
                    PostMsg("未知指令")

                End If

            ElseIf tst.Length = 1 Then
                If tin.Contains("cls") Then
                    tbmsg.Text = ""
                ElseIf tin.Contains("showmidc") Then
                    PostMsg(MidC)
                ElseIf tin.Contains("showspeed") Then
                    PostMsg((60 / basicT).ToString)
                ElseIf tin = "instc" Then
                    Call LoadInstConfig()
                ElseIf tin = "buf" Then
                    For i = 0 To 5
                        Await WriteIntoPlayerBuffer()
                    Next
                ElseIf tin = "play" Then
                    MyPlayer.Init()
                    MyPlayer.SetFormat(Nothing)
                    MyPlayer.LoadBufferAndPlay()
                    Timer1.Start()
                Else
                    PostMsg("未知指令")

                End If

            End If



        End If
    End Sub

    Public Sub PaintWS(index As Short)
        P2.Image = Nothing
        Dim bm As New Bitmap(400, 200)
        Dim G As Graphics = Graphics.FromImage(bm)
        G.Clear(Color.White)

        Dim ListPoints As List(Of PointD) = ListWS(index).KeyPoints

        If ListPoints.Count >= 2 Then
            For i = 0 To ListPoints.Count - 2
                Dim tp As PointD = ListPoints(i)
                Dim tp2 As PointD = ListPoints(i + 1)

                G.DrawLine(CfbPen, CSng(tp.X * 400), CSng(100 - tp.Y * 100), CSng(tp2.X * 400), CSng(100 - tp2.Y * 100))

            Next

        End If

        P2.Image = bm
        P2.Refresh()
        G.Dispose()

    End Sub

    Public Sub FMApply(ws As Short, weight As Short)
        Dim tfmlist As List(Of PointD) = ListWS(ws).KeyPoints
        Dim lp As List(Of PointD) = ListWS(selectedws).KeyPoints

        Dim rlist As New List(Of PointD)
        For i = 0 To lp.Count - 1
            rlist.Add(lp(i).Copy)
        Next
        For i = 0 To tfmlist.Count - 1
            rlist.Add(tfmlist(i).Copy)
        Next
        Dim trmax As Short = rlist.Count - 1
        Call SortX(rlist)
        For i = trmax To 1 Step -1
            If rlist(i).X = rlist(i - 1).X Then
                rlist.Remove(rlist(i))
            End If
        Next

        '取平均值或权重值
        For i = 0 To rlist.Count - 1
            Dim tx As Double = rlist(i).X
            Dim ty As Double = 0
            ty = GetY(tx, lp) * (100 - weight) / 100 + GetY(tx, tfmlist) * weight / 100
            rlist(i).Y = ty
        Next

        lp.Clear()
        lp = Nothing
        ListWS(selectedws).KeyPoints = rlist

    End Sub

    Public Sub MovePhi(value As Short)
        Dim lp As List(Of PointD) = ListWS(selectedws).KeyPoints
        For i = 1 To lp.Count - 2
            Dim tp As PointD = lp(i)
            tp.X += CDbl(value / 100)
            If tp.X > 1 Then tp.X -= 1
        Next

        Call SortX(lp)
        'Dim tcomp As New Comparison(Of PointD)(AddressOf PointDCompare)
        'Call lp.Sort(tcomp)

    End Sub

    Private Sub P_MouseClick(sender As Object, e As MouseEventArgs) Handles P.MouseClick
        If e.X <= 100 Then
            PrintPage -= 1
            If PrintPage < 0 Then PrintPage = 0
        ElseIf e.X >= 650 Then
            PrintPage += 1
        End If
        Call PrintNotes()
    End Sub

    Private Async Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If MyPlayer.PlayBuffers.Pool.Count <= 3 Then
            Await WriteIntoPlayerBuffer()
        End If
    End Sub


    'Private Function PointDCompare(a As PointD, b As PointD) As Double
    '    Return (a.X - b.X)
    'End Function



End Class

Public Class MidiInfo

End Class
