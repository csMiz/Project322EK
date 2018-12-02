Imports System.Threading.Tasks

Module Module1

    Public Const r1 As Double = 0.09
    Public Const r2 As Double = 10

    Public list1 As New List(Of String)
    Public playingspeed As Integer = 120    'midi文件里一拍的时值
    Public basicT As Double = 60 / 135   '每一拍（一个四分音符）的时间 单位为秒
    'Public playingtime As Integer = 0  '写在module里了
    Public SamplePerT As Integer = 24
    Public musicend As Integer = 0
    Public track(15) As List(Of clsNote)
    Public controller(15) As List(Of clsController)
    Public trackmood(15) As Byte
    Public trackpos(15) As Byte
    Public trackvol(15) As Byte
    Public LOffset(15) As Double
    Public ROffset(15) As Double
    Public LOffsetSample(15) As Integer
    Public ROffsetSample(15) As Integer
    Public NoteEndPoint(15) As Long
    Public InstrumentTag(15) As Integer
    Public GlobalTrackVol(15) As Byte
    Public ChangeInst(15) As List(Of clsChangeInst)

    Public TrackRunningListL(15) As List(Of clsNoteEx)
    Public TrackRunningListR(15) As List(Of clsNoteEx)


    Public MyPlayer As PlayerService = PlayerService.Instance
    Public PlayingCursor As Integer = 0

    Public UsingMidiInfo As New MidiInfo

    Public Async Function WriteIntoPlayerBuffer() As Task(Of Boolean)
        Form1.PostMsg("start writing:" & PlayingCursor)

        For Each co As Control In Form1.Controls
            If TypeOf (co) Is TextBox Then
                If CType(co, TextBox).Tag > 0 Then
                    InstrumentTag(CType(co, TextBox).Tag - 1) = CInt(CType(co, TextBox).Text)
                End If
            End If
        Next

        Dim list As List(Of Byte()) = MyPlayer.PlayBuffers.Pool
        Dim arr(16000) As Byte

        Dim SR As Integer = 4000
        SamplePerT = Convert.ToInt32(SR / (playingspeed / basicT))
        Dim TwTrim As Double
        Dim Tw As Integer

        For twi = 0 To 3999
            Tw = PlayingCursor * 4000 + twi

            Dim tbufL As Double = 0
            Dim tbufR As Double = 0
            TwTrim = Tw / SamplePerT

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
            'For i = 0 To SamplePerT - 1
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

            arr(twi * 4 + 1) = DoubleTo2Bytes(tbufL, 1)
            arr(twi * 4) = DoubleTo2Bytes(tbufL, 2)

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

            arr(twi * 4 + 3) = DoubleTo2Bytes(tbufR, 1)
            arr(twi * 4 + 2) = DoubleTo2Bytes(tbufR, 2)

            '----------------------------------------------------------------------


            If (TwTrim - ((r1 + r2) * SR / (340 * SamplePerT))) > (musicend + 75) Then
                '此处的75是ReleaseTime
                '暂时没有找到更好的处理方法
                Return False
                'Exit For
            End If
            'Next
        Next
        list.Add(arr)
        PlayingCursor += 1
        'PostMsg("write buffer complete")

        Return True
    End Function


    Public Function GetInst(inst As Short) As Short
        Dim r As Short = 1
        If inst = 15 OrElse inst = 16 OrElse inst = 17 Then
            r = 2
        End If
        Return r
    End Function


    Public Function GetFinalVolume(tnote As clsNote, trackindex As Byte, LR As Byte) As Double
        Dim r = tnote.volume / 512
        r *= (trackmood(trackindex) / 127)
        'r *= (GlobalTrackVol(trackindex) / 128)    暂时先不处理

        Dim temp1 As Double = 1

        If LR = 0 Then  '0-left 1-right
            temp1 = (1 - (((LOffset(trackindex) * 340) - (r2 - r1)) / (2 * r1)))
        Else
            temp1 = (1 - (((ROffset(trackindex) * 340) - (r2 - r1)) / (2 * r1)))
        End If

        If temp1 > 0.5 Then
            Dim temp2 As Double = temp1 - 0.5
            temp2 = temp2 ^ (1.5)
            temp1 = 0.5 + temp2
        ElseIf temp1 < 0.5 Then
            Dim temp2 As Double = 0.5 - temp1
            temp2 = temp2 ^ (1.5)
            temp1 = 0.5 - temp2
        End If

        r *= temp1

        Return r
    End Function
End Module
