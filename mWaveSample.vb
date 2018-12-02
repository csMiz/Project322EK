Imports System.IO
Imports System.Text.RegularExpressions

Module mWaveSample
    'Public Const HalfRoot3 As Double = 0.866
    Public FilterMaxFreqK As Double = 3

    Public ListWS(50) As CWaveSample

    Public Sub LoadWS()
        '在目录下寻找所有匹配文件
        '循环->依次读取
        For i = 1 To 49
            Dim tfilename As String = "type" + i.ToString + ".txt"

            Dim reader As StreamReader = Nothing
            Dim er As Boolean = False
            Try
                reader = File.OpenText(Application.StartupPath & "/" & tfilename)
            Catch ex As Exception
                er = True
            End Try
            If er Then Continue For

            If reader IsNot Nothing Then
                Dim tcws As New CWaveSample
                Do
                    Dim tst As String = reader.ReadLine()
                    If tst Is vbNullString Then
                        Exit Do
                    End If
                    Dim tst2() As String = Regex.Split(tst, ",")
                    If tst2.Length = 2 Then
                        tcws.KeyPoints.Add(New PointD(CDbl(tst2(0)), CDbl(tst2(1))))
                    End If
                Loop
                reader.Dispose()

                '波形缩放
                Dim aymax As Double = 0
                For j = 0 To tcws.KeyPoints.Count - 1
                    If Math.Abs(tcws.KeyPoints(j).Y) > aymax Then
                        aymax = Math.Abs(tcws.KeyPoints(j).Y)
                    End If
                Next
                Dim yk As Double = 1 / aymax
                For j = 0 To tcws.KeyPoints.Count - 1
                    tcws.KeyPoints(j).Y *= yk
                Next


                Call FilterApply(tcws)
                ListWS(i) = tcws

            End If
        Next

    End Sub

    Public Function FilterThreshold(pit As Double) As Double
        'Dim r As Double = 1
        'If pit > 75 AndAlso pit < 120 Then
        '    r = 1 - ((pit - 75) / 45)
        'ElseIf pit >= 120 Then
        '    r = 1 / 45
        'End If
        'Return r

        Dim r As Double = 1
        If pit > 75 Then
            r = 1 - ((pit - 75) / 75)
        End If
        Return r
    End Function

    Public Sub FilterApply(ws As CWaveSample)

        Dim FilterPoints As List(Of PointD) = ws.KeyPoints
        If FilterPoints.Count < 3 Then Exit Sub
        'Dim fv As Short = 50

        '写入CWS
        Dim tlist3 As New List(Of PointD)
        tlist3.Add(FilterPoints(0).Copy)
        For i = 0 To FilterPoints.Count - 3
            Dim tp1 As PointD = FilterPoints(i)
            Dim tp2 As PointD = FilterPoints(i + 1)
            Dim tp3 As PointD = FilterPoints(i + 2)

            Dim mid1 As New PointD((tp1.X + tp2.X) / 2, (tp1.Y + tp2.Y) / 2)
            Dim mid2 As New PointD((tp2.X + tp3.X) / 2, (tp2.Y + tp3.Y) / 2)

            Dim cmar As Single = 0.25

            Dim sub1 As New PointD(tp2.X - (tp2.X - tp1.X) * cmar, tp2.Y - (tp2.Y - tp1.Y) * cmar)
            Dim sub2 As New PointD(tp2.X - (tp2.X - tp3.X) * cmar, tp2.Y - (tp2.Y - tp3.Y) * cmar)

            Dim submid As New PointD((sub1.X + sub2.X) / 2, (sub1.Y + sub2.Y) / 2)

            Dim sub3 As New PointD(submid.X - (submid.X - mid1.X) * cmar, submid.Y - (submid.Y - mid1.Y) * cmar)
            Dim sub4 As New PointD(submid.X - (submid.X - mid2.X) * cmar, submid.Y - (submid.Y - mid2.Y) * cmar)

            tlist3.Add(mid1)
            tlist3.Add(sub3)
            tlist3.Add(sub4)
            tlist3.Add(mid2)

        Next
        tlist3.Add(FilterPoints(FilterPoints.Count - 1).Copy)
        ws.CWS = tlist3

        '拟合正弦波
        Dim tcws As List(Of PointD) = ws.CWS
        Dim tlist As New List(Of PointD)
        For i = 0 To tcws.Count - 2
            Dim tp1 As PointD = tcws(i)
            Dim tp2 As PointD = tcws(i + 1)
            If (tp1.Y >= 0 AndAlso tp2.Y < 0) Or (tp1.Y <= 0 AndAlso tp2.Y > 0) Then    '穿过X轴
                Dim k As Double = (tp2.Y - tp1.Y) / (tp2.X - tp1.X)
                Dim tx As Double = tp1.X + (-tp1.Y) / k
                Dim tp As New PointD(tx, 0)
                tlist.Add(tp)   '得到函数与X轴的焦点
            End If
        Next
        tlist.Add(New PointD(1, 0))

        '选取每一段的最高点
        Dim tlist2 As New List(Of PointD)
        For i = 0 To tlist.Count - 2
            Dim tp1 As PointD = tlist(i)
            Dim tp2 As PointD = tlist(i + 1)
            Dim ttip As Double = 0
            For j = 0 To tcws.Count - 1
                Dim tp3 As PointD = tcws(j)
                If tp3.X > tp1.X AndAlso tp3.X < tp2.X Then
                    If ttip >= 0 AndAlso tp3.Y > ttip Then
                        ttip = tp3.Y
                    ElseIf ttip <= 0 AndAlso tp3.Y < ttip Then
                        ttip = tp3.Y
                    End If
                ElseIf tp3.X > tp2.X Then
                    Exit For
                End If
            Next
            tlist2.Add(tp1.Copy)
            tlist2.Add(New PointD((tp1.X + tp2.X) / 2, ttip))
        Next
        tlist2.Add(New PointD(1, 0))
        tlist.Clear()
        tlist = Nothing

        '振幅太小改平






        ''删去振幅太小的点
        'If tlist2.Count >= 7 Then
        '    Dim tchanged As Boolean = False
        '    Do
        '        tchanged = False
        '        For i = 3 To tlist2.Count - 4 Step 2
        '            Dim tpm As PointD = tlist2(i)
        '            If Math.Abs(tpm.Y) < 0.2 Then
        '                Dim tp1 As PointD = tlist2(i - 1)
        '                Dim tp2 As PointD = tlist2(i + 1)
        '                Dim tp3 As PointD = tlist2(i - 2)
        '                Dim tp4 As PointD = tlist2(i + 2)
        '                tlist2.Remove(tpm)
        '                tlist2.Remove(tp1)
        '                tlist2.Remove(tp2)
        '                If Math.Abs(tp3.Y) < Math.Abs(tp4.Y) Then
        '                    tlist2.Remove(tp3)
        '                Else
        '                    tlist2.Remove(tp4)
        '                End If
        '                tchanged = True
        '                Exit For
        '            End If
        '        Next
        '    Loop While tchanged

        'End If

        ''削弱过大的斜率
        'For i = 1 To tlist2.Count - 2 Step 2
        '    Dim tpm As PointD = tlist2(i)
        '    Dim tp1 As PointD = tlist2(i - 1)
        '    Dim tp2 As PointD = tlist2(i + 1)

        '    If tpm.Y > 0 AndAlso tpm.Y > (tp2.X - tp1.X) * FilterMaxFreqK Then
        '        tpm.Y = (tp2.X - tp1.X) * FilterMaxFreqK
        '    ElseIf tpm.Y < 0 AndAlso tpm.Y < -((tp2.X - tp1.X) * FilterMaxFreqK) Then
        '        tpm.Y = -((tp2.X - tp1.X) * FilterMaxFreqK)
        '    End If
        'Next


        ws.FWS = tlist2




    End Sub


    Public Function FxWS(x As Long, Amp As Double, pit As Short, SR As Double, TimePassed As Integer, TimeDur As Integer, WSIndex As Short) As Double
        Dim y As Double = 0
        Dim T As Double = SR / GetFreq(pit, 2)

        Dim temp1 As Long = Math.Truncate(x / T)
        Dim tx As Double = (x - (temp1 * T)) / T

        If tx < 0 Then
            tx = 0
        ElseIf tx > 1 Then
            tx = 1
        End If

        If pit > 40 Then
            Dim tv As Double = (pit - 28) * 0.01
            Dim tv2 As Double = (pit - 65) * 0.005

            Dim rv3 As Double = tv2
            Dim rv2 As Double = tv
            Dim rv1 As Double = 1 - tv2 - tv
            If rv1 < 0 Then
                rv1 = 0
                rv3 = 1 - rv2
            End If

            y = Amp * FilterThreshold(pit) * (ListWS(WSIndex).GetSinValue(tx) * rv3 + ListWS(WSIndex).GetValue(tx) * rv1 + ListWS(WSIndex).GetValue(tx, 1) * rv2)
            'y = Amp * FilterThreshold(pit) * (ListWS(WSIndex).GetSinValue(tx) * tv + ListWS(WSIndex).GetValue(tx) * (1 - tv))
        ElseIf pit <= 40 Then
            y = Amp * FilterThreshold(pit) * ListWS(WSIndex).GetValue(tx)
        End If
        'If pit > 50 AndAlso pit <= 100 Then
        '    Dim tv As Double = (pit - 50) * 0.02
        '    y = Amp * FilterThreshold(pit) * (ListWS(WSIndex).GetSinValue(tx) * tv + ListWS(WSIndex).GetValue(tx) * (1 - tv))
        'ElseIf pit <= 50 Then
        '    y = Amp * FilterThreshold(pit) * ListWS(WSIndex).GetValue(tx)
        'Else
        '    y = Amp * FilterThreshold(pit) * ListWS(WSIndex).GetSinValue(tx)
        'End If


        If y > Amp Then y = Amp
        If y < -Amp Then y = -Amp

        Return y


    End Function

    Private Function ModuleGetIndex(rx As Double, tl As List(Of PointD)) As Short
        If tl.Count <= 2 Then Return -1
        If rx <= 0 OrElse rx >= 1 Then Return -1
        For i = 0 To tl.Count - 2
            If rx > tl(i).X AndAlso rx <= tl(i + 1).X Then
                Return i
            End If
        Next
        Return -1
    End Function
    Public Function GetY(rx As Double, tl As List(Of PointD)) As Double
        Dim ti As Short = ModuleGetIndex(rx, tl)
        If ti = -1 Then
            Return 0
        End If
        Dim pta As PointD = tl(ti)
        Dim ptb As PointD = tl(ti + 1)
        Dim tk As Double = (ptb.Y - pta.Y) / (ptb.X - pta.X)
        Dim result As Double = (rx - pta.X) * tk + pta.Y
        Return result
    End Function
    Public Sub SortX(tl As List(Of PointD))
        For i = tl.Count - 1 To 1 Step -1
            For j = 1 To i
                Dim tp1 As PointD = tl(j - 1)
                Dim tp2 As PointD = tl(j)
                If tp2.X < tp1.X Then
                    tl.Insert(j - 1, tp2.Copy)
                    tl.Remove(tp2)
                End If
            Next
        Next
    End Sub

End Module

Public Class CWaveSample
    Public KeyPoints As New List(Of PointD)
    Public FWS As List(Of PointD)
    Public CWS As List(Of PointD)
    Public ADSRType As Short = 1

    Private Function GetIndex(rX As Double, kp As List(Of PointD)) As Short
        If kp.Count <= 2 Then Return -1
        If rX <= 0 OrElse rX >= 1 Then Return -1

        Dim ub As Integer = kp.Count - 1
        Dim lb As Integer = 0
        Dim midd As Integer

        Do
            If ub - lb <= 1 Then Exit Do
            midd = CInt((ub + lb) / 2)

            If rX > kp(midd).X Then
                lb = midd
            ElseIf rX <= kp(midd).X Then
                ub = midd
            End If
        Loop
        Return lb

        'For i = 0 To kp.Count - 2
        '    If rX > kp(i).X AndAlso rX <= kp(i + 1).X Then
        '        Return i
        '    End If
        'Next
        'Return -1

    End Function

    Private Function GetIndex2(rX As Double) As Short
        If FWS.Count <= 2 Then Return -1
        If rX <= 0 OrElse rX >= 1 Then Return -1

        Dim ub As Integer = FWS.Count - 1
        Dim lb As Integer = 0
        Dim midd As Integer

        Do
            If ub - lb <= 1 Then Exit Do
            midd = CInt((ub + lb) / 2)

            If rX > FWS(midd).X Then
                lb = midd
            ElseIf rX <= FWS(midd).X Then
                ub = midd
            End If
        Loop
        Return lb

        'For i = 0 To FWS.Count - 2
        '    If rX > FWS(i).X AndAlso rX <= FWS(i + 1).X Then
        '        Return i
        '    End If
        'Next
        'Return -1
    End Function

    Public Function GetValue(rX As Double, Optional cTag As Short = 0) As Double
        Dim kp As List(Of PointD) = Nothing
        If cTag = 0 Then
            kp = KeyPoints
        ElseIf cTag = 1 Then
            kp = CWS
        End If
        If kp Is Nothing Then Return 0

        Dim ti As Short = GetIndex(rX, kp)
        If ti = -1 Then
            Return 0
        End If
        Dim pta As PointD = kp(ti)
        Dim ptb As PointD = kp(ti + 1)
        Dim tk As Double = (ptb.Y - pta.Y) / (ptb.X - pta.X)
        Dim result As Double = (rX - pta.X) * tk + pta.Y
        Return result
    End Function

    Public Function GetSinValue(rX As Double) As Double
        Dim ti As Short = GetIndex2(rX)
        If ti = -1 Then
            Return 0
        End If
        If ti Mod 2 = 1 Then
            ti -= 1
        End If
        Dim tpx1 As Double = FWS(ti).X
        Dim tpx2 As Double = FWS(ti + 2).X
        Dim amp As Double = FWS(ti + 1).Y

        Dim tx As Double = rX - tpx1
        Dim result As Double = amp * Math.Sin((Math.PI * tx) / (tpx2 - tpx1))
        Return result

    End Function

    'Public Function GetValue(rX As Double, k As Double) As Double

    '    Return 0
    'End Function
End Class

Public Class PointD
    Public X As Double
    Public Y As Double

    Public Sub New(tx As Double, ty As Double)
        X = tx
        Y = ty

    End Sub


    Public Function Copy() As PointD
        Dim r As New PointD(X, Y)
        Return r

    End Function
End Class