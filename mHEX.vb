Imports System.Text.RegularExpressions

Module mHEX

    Public Function CHex(input As Byte) As String
        Dim r As String = ""
        Dim a As Byte = input \ 16
        If a < 10 Then
            r = r & ChrW(a + AscW("0"))
        Else
            r = r & ChrW(a + AscW("7"))
        End If

        Dim b As Byte = input Mod 16
        If b < 10 Then
            r = r & ChrW(b + AscW("0"))
        Else
            r = r & ChrW(b + AscW("7"))
        End If

        Return r
    End Function

    Public Function HEXByte(input As String) As Byte
        Dim a As Char = input.Substring(0, 1)
        Dim b As Char = input.Substring(1, 1)

        Dim c As Byte = AscW(a) - AscW("0")
        If c >= 10 Then c -= 7
        Dim d As Byte = AscW(b) - AscW("0")
        If d >= 10 Then d -= 7

        Return (c * 16 + d)
    End Function

    Public Function DoubleTo2Bytes(input As Double, pos As Byte) As Byte
        Dim temp1 As Integer = 0
        If input > 0 Then
            temp1 = Convert.ToInt32(input * 32767)
        ElseIf input < 0 Then
            temp1 = Convert.ToInt32(65535 + (input * 32768))
        End If

        If pos = 1 Then
            Return (temp1 \ 256)
        Else
            Return (temp1 Mod 256)
        End If

    End Function

    Public Sub IntTo4Bytes(input As Long, ByRef buffer() As Byte)
        'little indian
        buffer(3) = input \ (2 ^ 24)
        buffer(2) = (input Mod (2 ^ 24)) \ (2 ^ 16)
        buffer(1) = (input Mod (2 ^ 16)) \ 256
        buffer(0) = (input Mod 256)

    End Sub

    Public Function BFindFirstByte(ByRef BContent As List(Of String), input As String, Optional start As Integer = 0) As Integer

        If start > ((BContent.Count - 1) * 80 + (BContent(BContent.Count - 1).Length) / 3) Then Return -1

        Dim st1 As Integer = start \ 80
        Dim st2 As Integer = start Mod 80

        For i = st1 To BContent.Count - 1
            If i = st1 Then
                If BContent(i).Contains(input) Then
                    Dim tstr() As String = Regex.Split(BContent(i), input)
                    Dim tp As Integer = 0
                    If tstr.Length >= 2 Then
                        Dim jm As Integer = tstr.Length - 2
                        For j = 0 To jm
                            For k = 0 To j
                                tp += tstr(k).Length
                            Next
                            tp += j * input.Length

                            tp /= 3
                            If tp >= st2 Then
                                Return (i * 80 + tp)
                            End If
                            tp = 0
                        Next
                    End If
                End If
            Else
                If BContent(i).Contains(input) Then
                    Dim tstr() As String = Regex.Split(BContent(i), input)
                    Dim tp As Integer = 0
                    If tstr.Length >= 2 Then
                        tp = tstr(0).Length / 3
                        Return (i * 80 + tp)
                    End If
                End If
            End If
        Next

        Return -1

    End Function

    Public Function BFindTwoBytes(ByRef BC As List(Of String), s1 As String, s2 As String, Optional start As Integer = 0) As Integer

        Dim marg As Integer = 0
        Do
            Dim tst As Integer = start + marg
            Dim a As Integer = BFindFirstByte(BC, s1, tst)
            If a <> -1 Then
                Dim b As String = BGetByte(BC, a + 1, False)
                If b = s2 Then
                    Return a
                End If
                marg += (a - tst + 1)
            Else
                Return -1
            End If
        Loop

    End Function

    Public Function BGetByte(ByRef BContent As List(Of String), pos As Integer, Optional pure As Boolean = True) As String
        Dim tl As Integer = pos \ 80
        Dim tp As Integer = pos Mod 80
        Dim r As String = ""
        If tl > BContent.Count - 1 Then Return r
        Dim ol As String = BContent(tl)

        If tp * 3 + 3 > ol.Length Then Return r
        If pure Then
            r = ol.Substring(tp * 3, 2)
        Else
            r = ol.Substring(tp * 3, 3)
        End If

        Return r
    End Function

End Module

Module mWav

    Public Function WavHead(ByRef buffer() As Byte) As Integer
        buffer(0) = HEXByte("52")
        buffer(1) = HEXByte("49")
        buffer(2) = HEXByte("46")
        buffer(3) = HEXByte("46")

        For i = 4 To 7  '总大小
            buffer(i) = 0
        Next

        buffer(8) = HEXByte("57")
        buffer(9) = HEXByte("41")
        buffer(10) = HEXByte("56")
        buffer(11) = HEXByte("45")

        buffer(12) = HEXByte("66")
        buffer(13) = HEXByte("6D")
        buffer(14) = HEXByte("74")
        buffer(15) = HEXByte("20")

        buffer(16) = 16
        buffer(17) = 0
        buffer(18) = 0
        buffer(19) = 0

        buffer(20) = 1
        buffer(21) = 0
        buffer(22) = 2
        buffer(23) = 0

        buffer(24) = HEXByte("C0")  '采样频率
        buffer(25) = HEXByte("5D")
        buffer(26) = 0
        buffer(27) = 0

        buffer(28) = 0              '字节率
        buffer(29) = HEXByte("77")
        buffer(30) = 1
        buffer(31) = 0

        buffer(32) = 4
        buffer(33) = 0
        buffer(34) = 16
        buffer(35) = 0

        buffer(36) = HEXByte("64")
        buffer(37) = HEXByte("61")
        buffer(38) = HEXByte("74")
        buffer(39) = HEXByte("61")

        For i = 40 To 43  'data大小
            buffer(i) = 0
        Next

        Return 44
    End Function

End Module

Module mMid
    Public MidC As Double = 131
    Public playingtime As Integer = 0
    Public VolDecay0 As Integer = 55
    Public VolDecay1 As Integer = 178
    Public VolDecay2 As Integer = 330
    Public VolDecay3 As Integer = 517
    Public VolDecay4 As Integer = 295

    Public Function GetTrack(input As String) As Byte
        Dim a As Char = input.Substring(1, 1)

        Dim b As Byte = AscW(a) - AscW("0")
        If b >= 10 Then b -= 7

        Return b
    End Function

    Public Function GetFirstNotePos(ByRef BC As List(Of String), Optional StartAt As Integer = 0) As Integer
        Dim p9(15) As Integer
        Dim tk1 As String = "00/", tk2 As String = ""
        For i = 0 To 15
            tk2 = "9" & CHex(i).Substring(1, 1) & "/"
            p9(i) = BFindTwoBytes(BC, tk1, tk2, StartAt)
        Next

        Dim min As Integer = 999999

        For i = 0 To 15
            If p9(i) <> -1 Then
                If p9(i) < min Then
                    min = p9(i)
                End If
            End If
        Next

        'Dim pb

        Return min

    End Function

    Public Function GetTimeSpan(ByRef BC As List(Of String), start As Integer, ByRef Count As Byte) As Integer
        Dim c As Byte = 1
        Dim r As Integer = 0
        Dim con As Boolean = False

        Do
            Dim t As String = BGetByte(BC, start + c)
            If (AscW(t.Substring(0, 1)) - AscW("9") <> 0 And AscW(t.Substring(0, 1)) - AscW("8") <> 0 And AscW(t.Substring(0, 1)) - AscW("B") <> 0 And AscW(t.Substring(0, 1)) - AscW("C") <> 0 And AscW(t.Substring(0, 1)) - AscW("E") <> 0 And t <> "FF") Or con Then
                If HEXByte(t) > 128 Then
                    con = True
                Else
                    con = False
                End If
                c += 1  '存在漏洞
                '比如 82 b1
            Else
                Exit Do
            End If
        Loop

        Count = c

        If c >= 1 Then
            For i = 0 To c - 1
                Dim t As String = BGetByte(BC, start + i)
                If i <> c - 1 Then  '不是最后一位的话，都要给实际数值加上16进制的80
                    Dim a As Byte = HEXByte(t) - 128
                    r += a * (128 ^ (c - 1 - i))
                Else    '最后一位不用减，直接算
                    Dim a As Byte = HEXByte(t)
                    r += a
                End If
            Next
        End If

        Return r

    End Function

    Public Function GetFreq(p As Byte, instrument As Byte) As Integer

        Dim r As Double = 0

        'Select Case instrument
        '    Case 0  '音叉 纯正弦
        '        MidC = 262
        '    Case 1  '长笛
        '        MidC = 262
        '    Case 2  '钢琴
        '        MidC = 131
        '    Case 3  '小提琴
        '        MidC = 262

        'End Select

        r = MidC * (2 ^ ((p - 60) / 12))

        Return Convert.ToInt32(r)

    End Function

    Public Function PrintPitch(p As Byte) As String
        Dim a As Byte = p \ 12
        Dim b As Byte = p Mod 12

        Dim str As String = ""

        Select Case b
            Case 0
                str = "1"
            Case 1
                str = "1#"
            Case 2
                str = "2"
            Case 3
                str = "2#"
            Case 4
                str = "3"
            Case 5
                str = "4"
            Case 6
                str = "4#"
            Case 7
                str = "5"
            Case 8
                str = "5#"
            Case 9
                str = "6"
            Case 10
                str = "6#"
            Case 11
                str = "7"
        End Select

        If a < 5 Then
            For i = a To 4
                str = "." & str
            Next
        ElseIf a > 5 Then
            For i = 6 To a
                str = str & "'"
            Next
        End If

        Return str

    End Function

    Public Function FxFlute(x As Long, Amp As Double, Freq As Double, SampleRate As Double, TimePassed As Integer, TimeDur As Integer) As Double
        Dim y As Double = 0
        Dim T As Double = SampleRate / Freq
        Dim p1 As Double = T / 4
        Dim p2 As Double = 13 * T / 16

        Dim temp1 As Long = Math.Truncate(x / T)
        Dim tx As Double = x - (temp1 * T)

        If tx < 0 Then
            tx = T - tx
        End If

        If tx >= 0 AndAlso tx <= p1 Then
            y = 0.5 * Amp * Math.Sin((4 * Math.PI * Freq * tx) / SampleRate - (Math.PI * Freq * T) / (2 * SampleRate)) + 0.5 * Amp
        ElseIf tx > p1 AndAlso tx <= p2 Then
            y = Amp * Math.Sin((8 * Math.PI * Freq * tx) / (3 * SampleRate) - (T * Math.PI * Freq) / (6 * SampleRate))
        ElseIf tx > p2 Then
            y = 0.25 * Amp * Math.Sin((16 * Math.PI * Freq * tx) / (3 * SampleRate) - (13 * Math.PI * T * Freq) / (3 * SampleRate))
        End If
        y += 0.03 * Amp * Math.Sin((34 * Math.PI * Freq * tx) / SampleRate)

        'If TimeDur > 150 Then
        'VolDecay1 = TimeDur / 4
        'If VolDecay1 > 80 Then VolDecay1 = 80
        Dim trem As Integer = TimeDur - TimePassed
        If trem <= 0 Then
            y = 0
        ElseIf TimePassed < (0.5 * VolDecay1) Then
            Dim ta As Double = -1 / ((0.5 * VolDecay1) ^ 2)
            y = y * (ta * (TimePassed ^ 2) + (-2 * ta * (0.5 * VolDecay1)) * TimePassed)
        ElseIf trem < VolDecay1 Then
            Dim ta As Double = -1 / (VolDecay1 ^ 2)
            y = y * (ta * (trem ^ 2) + (-2 * ta * VolDecay1) * trem)
        End If
        'End If
        If TimeDur > TimePassed Then
            y = y * (-TimePassed / (4 * TimeDur) + 1)
        End If

        If y > 1 Then y = 1
        Return y

    End Function

    Public Function sinewave(ByVal x As Double, tAmp As Double, tOmega As Double, Amp As Double, Freq As Double, SR As Double, Optional Phi As Single = 0) As Double
        sinewave = tAmp * Amp * Math.Sin(2 * Math.PI * tOmega * Freq * x / SR)

    End Function

    Public Function FxSineWave(x As Long, Amp As Double, Freq As Double, SR As Double, TimePassed As Integer, TimeDur As Integer) As Double

        Dim y As Double = 0
        Dim T As Double = SR / Freq

        Dim temp1 As Long = Math.Truncate(x / T)
        Dim tx As Double = x - (temp1 * T)

        y = sinewave(tx, 1, 1, Amp, Freq, SR) + sinewave(tx, 1 / 2, 2, Amp, Freq, SR) + sinewave(tx, 1 / 4, 4, Amp, Freq, SR) + sinewave(tx, 1 / 3, 3, Amp, Freq, SR)
        y *= 0.5
        'y += 0.03 * Amp * Math.Sin((40 * Math.PI * Freq * tx) / SR)

        If TimeDur > TimePassed AndAlso TimePassed > TimeDur - VolDecay0 Then
            y = y * ((TimeDur - TimePassed) / VolDecay0)
        Else
            y = 0
        End If

        If y > 1 Then y = 1
        Return y
    End Function

    Public Function FxPiano(x As Long, Amp As Double, Freq As Double, SR As Double, TimePassed As Integer, TimeDur As Integer, Optional Var As Integer = 0) As Double

        Dim y As Double = 0
        Dim T As Double = SR / Freq
        Dim p1 As Double = T / 6
        Dim p2 As Double = 2 * T / 9
        Dim p3 As Double = T / 2
        Dim p4 As Double = 11 * T / 18
        Dim p5 As Double = 13 * T / 18

        Dim temp1 As Long = Math.Truncate(x / T)
        Dim tx As Double = x - (temp1 * T)

        If tx < 0 Then
            tx = T - tx
        End If
        tx += (T / 18)
        If tx > T Then
            tx -= T
        End If

        If tx <= p1 Then
            y = (9 * Amp * tx) / T - (Amp / 2)
        ElseIf tx > p1 AndAlso tx <= p2 Then
            y = ((-18) * Amp * tx) / (5 * T) + (8 * Amp / 5)
        ElseIf tx > p2 AndAlso tx <= p3 Then
            y = 4 * Amp / 5
        ElseIf tx > p3 AndAlso tx <= p4 Then
            y = (13 / 20) * Amp * Math.Sin((9 * Math.PI * Freq * tx) / SR - (4 * Math.PI * Freq * T) / SR) + (3 / 20) * Amp
        ElseIf tx > p4 AndAlso tx <= p5 Then
            y = 0.25 * Amp * Math.Sin((9 * Math.PI * Freq * tx) / SR - (11 * Math.PI * Freq * T) / (2 * SR)) - 0.5 * Amp
        ElseIf tx > p5 AndAlso tx <= T Then
            y = 0.5 * Amp * Math.Sin((18 * Math.PI * Freq * tx) / (5 * SR) - (8 * Math.PI * Freq * T) / (5 * SR)) - 0.5 * Amp
        End If

        'y += FxNoise(Amp)
        y += 0.03 * Amp * Math.Sin((36 * Math.PI * Freq * tx) / SR)

        If Var = 1 Then
            y += 0.03 * Amp * Math.Sin((53 * Math.PI * Freq * (tx - (T / 18))) / SR)
            y *= 1.5
        End If
        If y > Amp Then y = Amp
        If y < -Amp Then y = -Amp

        'If TimeDur > 100 Then
        Dim trem As Integer = TimeDur - TimePassed
        If trem <= 0 Then
            y = 0
        ElseIf TimePassed < (0.25 * VolDecay2) Then
            Dim ta As Double = -1 / ((0.25 * VolDecay2) ^ 2)
            y = y * (ta * (TimePassed ^ 2) + (-2 * ta * (0.25 * VolDecay2)) * TimePassed)
        ElseIf trem < VolDecay2 AndAlso TimePassed > 20 Then
            Dim ta As Double = -1 / ((TimeDur - 20) ^ 2)
            y = y * (ta * (trem ^ 2) + (-2 * ta * (TimeDur - 20)) * trem)
        End If
        'End If
        If TimeDur > TimePassed Then
            y = y * (-TimePassed / (4 * TimeDur) + 1)
        End If

        If y > 1 Then y = 1
        Return y

    End Function

    Public Function FxViolin(x As Long, Amp As Double, Freq As Double, SR As Double, TimePassed As Integer, TimeDur As Integer) As Double

        Dim y As Double = 0
        Dim T As Double = SR / Freq
        Dim p1 As Double = T / 3
        Dim p2 As Double = T / 2
        Dim p3 As Double = 5 * T / 8
        Dim p4 As Double = 5 * T / 6

        Dim temp1 As Long
        temp1 = Math.Truncate(x / T)
        Dim tx As Double
        tx = x - (temp1 * T)

        If tx < 0 Then
            tx = T - tx
        End If

        If tx >= 0 AndAlso tx <= p4 Then
            y = (-6 * tx / (5 * T)) * Amp * Math.Sin(6 * Math.PI * Freq * tx / SR)
        ElseIf tx > p4 AndAlso tx <= T Then
            y = (-6 * (tx - (2 * T / 3)) / (5 * T)) * Amp * Math.Sin(6 * Math.PI * Freq * (tx - (2 * T / 3)) / SR)
        End If

        If tx > p1 AndAlso tx < p2 Then
            y = y - ((Amp / 6) * Math.Sin(18 * Math.PI * Freq * (tx - p1) / SR))
            'y = y + ((-(tx - T / 3) * Amp / T) * Sin(336 * pi * (tx - T / 3) / (T * (0.5 + ((2 * tx + 0.25) ^ 0.5)))))
        ElseIf tx > p3 AndAlso tx < p4 Then
            y = y + ((Amp / 6) * Math.Sin(24 * Math.PI * Freq * (tx - (2 * T / 3)) / SR))
        End If

        y += 0.03 * Amp * Math.Sin((36 * Math.PI * Freq * tx) / SR)

        'If TimeDur > 150 Then
        '    VolDecay3 = TimeDur / 3
        'Else
        '    VolDecay3 = TimeDur / 2
        'End If
        'If TimeDur > 150 Then
        'VolDecay3 = TimeDur / 3
        'If VolDecay3 > 80 Then VolDecay3 = 80
        Dim trem As Integer = TimeDur - TimePassed
        If trem <= 0 Then
            y = 0
        ElseIf TimePassed < (0.5 * VolDecay3) Then
            Dim ta As Double = -1 / ((0.5 * VolDecay3) ^ 2)
            y = y * (ta * (TimePassed ^ 2) + (-2 * ta * (0.5 * VolDecay3)) * TimePassed)
        ElseIf trem < VolDecay3 Then
            Dim ta As Double = -1 / (VolDecay3 ^ 2)
            y = y * (ta * (trem ^ 2) + (-2 * ta * VolDecay3) * trem)
        End If
        'End If

        If TimeDur > TimePassed Then
            y = y * (-TimePassed / (3 * TimeDur) + 1)
        End If

        If y > 1 Then y = 1
        Return y

    End Function

    Public Function FxGuitar(x As Long, Amp As Double, Freq As Double, SR As Double, TimePassed As Integer, TimeDur As Integer) As Double
        Dim y As Double = 0
        Dim T As Double = SR / Freq
        Dim p1 As Double = 3 * T / 7
        Dim p2 As Double = 4 * T / 7
        Dim p3 As Double = 6 * T / 7
        Dim p4 As Double = 2 * T / 7

        Dim temp1 As Long
        temp1 = Math.Truncate(x / T)
        Dim tx As Double
        tx = x - (temp1 * T)

        If tx < 0 Then
            tx = T - tx
        End If
        tx += (T / 7)
        If tx > T Then
            tx -= T
        End If

        If tx >= 0 AndAlso tx <= p1 Then
            y = Amp * Math.Sin((7 * Math.PI * Freq / (2 * SR)) * (tx - (T / 7)))
        ElseIf tx > p1 AndAlso tx <= p2 Then
            y = (-0.4) * Amp * Math.Sin(7 * Math.PI * Freq * (tx - p1) / SR)
        ElseIf tx > p2 AndAlso tx <= p3 Then
            y = 0
        ElseIf tx > p3 AndAlso tx <= T Then
            y = (-7 * Amp / T) * tx + 6 * Amp
        End If

        If tx > p4 AndAlso tx < p3 Then
            y = y + 0.2 * Amp * Math.Sin(21 * Math.PI * Freq * tx / SR)
        End If

        y += 0.04 * Amp * Math.Sin((35 * Math.PI * Freq * tx) / SR)

        y = y * 0.8

        'If TimeDur > 100 Then
        Dim trem As Integer = TimeDur - TimePassed
        If trem <= 0 Then
            y = 0
        ElseIf TimePassed < (0.5 * VolDecay4) Then
            Dim ta As Double = -1 / ((0.5 * VolDecay4) ^ 2)
            y = y * (ta * (TimePassed ^ 2) + (-2 * ta * (0.5 * VolDecay4)) * TimePassed)
        ElseIf trem < VolDecay4 Then
            Dim ta As Double = -1 / ((TimeDur - 20) ^ 2)
            y = y * (ta * (trem ^ 2) + (-2 * ta * (TimeDur - 20)) * trem)
        End If
        'End If
        If TimeDur > TimePassed Then
            y = y * (-TimePassed / (1.5 * TimeDur) + 1)
        End If

        If y > 1 Then y = 1
        Return y



    End Function

    'Public Function FxNoise(amp As Double) As Double

    '    Dim r As Double = rand.NextDouble * 0.3 - 0.15
    '    Return (r * amp)

    'End Function

    Public Function GetTimePassed(sa As Integer) As Integer
        Dim r As Integer = playingtime - sa
        If r < 0 Then r = 0
        Return r
    End Function

End Module

Module mPng
    Const Base As Integer = 65521

    Public Function adler32(Input As List(Of Char)) As Long
        '此函数会改动传入的列表
        If Input.Count = 0 Then Return 0
        Dim ta As Long = 1
        Dim tb As Long = 0
        Dim tx As Char
        Dim r As Long = 0

        Do
            If Input.Count <> 0 Then
                tx = Input(0)
                Input.RemoveAt(0)
            Else    '最后一步输出
                r = (tb * 65536) Or ta
                Return r
            End If

            ta = (ta + (AscW(tx) And &HFF)) Mod Base
            tb = (ta + tb) Mod Base
        Loop


    End Function

    Public Function testadl() As Long
        Dim tl As New List(Of Char)
        'tl.Add(ChrW(HEXByte("78")))
        'tl.Add(ChrW(HEXByte("DA")))
        'tl.Add(ChrW(HEXByte("01")))
        'tl.Add(ChrW(HEXByte("28")))
        'tl.Add(ChrW(HEXByte("00")))
        'tl.Add(ChrW(HEXByte("D7")))
        'tl.Add(ChrW(HEXByte("FF")))
        tl.Add(ChrW(HEXByte("00")))
        tl.Add(ChrW(HEXByte("CB")))
        tl.Add(ChrW(HEXByte("A9")))
        tl.Add(ChrW(HEXByte("87")))
        tl.Add(ChrW(HEXByte("65")))
        tl.Add(ChrW(HEXByte("00")))
        tl.Add(ChrW(HEXByte("BA")))
        tl.Add(ChrW(HEXByte("98")))
        tl.Add(ChrW(HEXByte("76")))
        tl.Add(ChrW(HEXByte("54")))
        tl.Add(ChrW(HEXByte("00")))
        tl.Add(ChrW(HEXByte("A9")))
        tl.Add(ChrW(HEXByte("87")))
        tl.Add(ChrW(HEXByte("65")))
        tl.Add(ChrW(HEXByte("43")))
        tl.Add(ChrW(HEXByte("00")))
        tl.Add(ChrW(HEXByte("98")))
        tl.Add(ChrW(HEXByte("76")))
        tl.Add(ChrW(HEXByte("54")))
        tl.Add(ChrW(HEXByte("32")))
        tl.Add(ChrW(HEXByte("00")))
        tl.Add(ChrW(HEXByte("87")))
        tl.Add(ChrW(HEXByte("65")))
        tl.Add(ChrW(HEXByte("43")))
        tl.Add(ChrW(HEXByte("21")))
        tl.Add(ChrW(HEXByte("00")))
        tl.Add(ChrW(HEXByte("76")))
        tl.Add(ChrW(HEXByte("54")))
        tl.Add(ChrW(HEXByte("32")))
        tl.Add(ChrW(HEXByte("10")))
        tl.Add(ChrW(HEXByte("00")))
        tl.Add(ChrW(HEXByte("65")))
        tl.Add(ChrW(HEXByte("43")))
        tl.Add(ChrW(HEXByte("21")))
        tl.Add(ChrW(HEXByte("00")))
        tl.Add(ChrW(HEXByte("00")))
        tl.Add(ChrW(HEXByte("54")))
        tl.Add(ChrW(HEXByte("32")))
        tl.Add(ChrW(HEXByte("10")))
        tl.Add(ChrW(HEXByte("00")))

        '结果需要得到24a70ba4
        Dim r = adler32(tl)
        'correct
        Return r

    End Function


End Module

Class clsNote
    Public pitch As Byte = 0
    Public volume As Byte = 0
    Public startat As Integer = 0
    Public duration As Integer = 0
    Public status As Byte = 0    '1-已经按下 2-已经松开
    Public playingstatus As Byte = 0    '1-已经演奏
    Public playingcur As Integer = 0
    Public tag As Integer = -1

    Public Sub New(pit As Byte, vol As Byte, st As Integer)
        pitch = pit
        volume = vol
        startat = st
        status = 1
    End Sub

    Public Function AddAmpControl(inst As Byte) As clsNoteEx
        Dim r As New clsNoteEx(pitch, volume, startat)

        With r
            .duration = duration
            .InstTag = inst
            If inst = 1 Then    '键盘乐器
                If duration > 70 Then
                    .AttackTime = 35
                    .DecayTime = 35
                    .SustainTime = duration - 70
                    .ReleaseTime = 35
                    '.ReleaseTime = 75
                Else    '最少为145
                    .AttackTime = 35
                    .DecayTime = 35
                    .SustainTime = 0
                    .ReleaseTime = 35
                    '.ReleaseTime = 75               
                End If
            ElseIf inst = 2 Then    '弦乐器
                If duration > 90 Then
                    .AttackTime = 90
                    .DecayTime = 0
                    .SustainTime = duration - 90
                    .ReleaseTime = 60
                Else
                    .AttackTime = 75
                    .DecayTime = 15
                    .SustainTime = 0
                    .ReleaseTime = 60
                End If

            End If

        End With
        Return r
    End Function

End Class

Class clsNoteEx
    Inherits clsNote
    Public AttackTime As Integer = 0
    Public DecayTime As Integer = 0
    Public SustainTime As Integer = 0
    Public ReleaseTime As Integer = 0
    Public InstTag As Short = 1

    Public Sub New(pit As Byte, vol As Byte, st As Integer)
        MyBase.New(pit, vol, st)
    End Sub

    Public Function GetAmp(tsamplecount As Short) As Double  'ADSR包络控制
        Dim r As Double = 0
        Dim pc As Double = playingcur / tsamplecount

        If InstTag = 1 Then
            If pc <= AttackTime Then
                r = pc / AttackTime
            ElseIf pc > AttackTime AndAlso pc <= (AttackTime + DecayTime) Then
                r = 0.7 + 0.3 * (1 - ((pc - AttackTime) / DecayTime))
            ElseIf pc > (AttackTime + DecayTime) AndAlso pc <= (AttackTime + DecayTime + SustainTime) Then
                'r = 0.6 + 0.1 * (1 - ((pc - AttackTime - DecayTime) / SustainTime))
                If pc - AttackTime - DecayTime <= 800 Then
                    r = 0.7 - (pc - AttackTime - DecayTime) * 0.00025
                Else
                    r = 0.5
                End If
                ReleaseTime = CInt(r / (0.7 / 75))
            Else
                Dim tmax As Double = 0.7 - SustainTime * 0.00025
                If tmax < 0.5 Then tmax = 0.5
                r = tmax - (0.7 * (pc - AttackTime - DecayTime - SustainTime) / 75)
            End If
        ElseIf InstTag = 2 Then
            If DecayTime = 0 Then
                If pc <= AttackTime Then
                    r = pc * 0.8 / AttackTime
                ElseIf pc > AttackTime AndAlso pc <= (AttackTime + SustainTime) Then
                    r = 0.8
                Else
                    r = 0.8 - (0.8 * (pc - AttackTime - SustainTime) / 60)
                End If
            Else
                If pc <= AttackTime Then
                    r = pc / AttackTime
                ElseIf pc > AttackTime AndAlso pc <= (AttackTime + DecayTime) Then
                    r = 0.7 + 0.3 * (1 - ((pc - AttackTime) / DecayTime))
                Else
                    r = 0.7 - (0.7 * (pc - AttackTime - DecayTime) / 60)
                End If
            End If

        End If

        If r > 1 Then
            r = 1
        ElseIf r < 0 Then
            r = 0
        End If

        Return r
    End Function


End Class

Class clsController
    Public startat As Integer = 0
    Public target As Byte = 0
    Public value As Byte = 80
    Public status As Byte = 0   '0-未生效 1-已生效

    Public Sub New(st As Integer, tar As Byte, v As Byte)
        startat = st
        target = tar
        value = v
        status = 0
    End Sub
End Class

Public Class clsChangeInst
    Public StartAt As Integer = 0
    Public Value As Short = 0

    Public Sub New(st As Integer, v As Byte)
        StartAt = st
        Value = CInt(v)
    End Sub

End Class

'Class clsExTimer
'    Inherits DispatcherTimer
'    Public EndAt As Integer = 0
'    Public wo As NAudio.Win8.Wave.WaveOutputs.WasapiOutRT

'    'Public tmrbody As System.Threading.Timer
'End Class

'Class clsExTimer2
'    Inherits DispatcherTimer
'    Public TrackIndex As Byte = 0

'End Class
