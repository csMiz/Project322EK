Imports SlimDX.Multimedia
Imports SlimDX.XAudio2
Imports SlimDX
Imports System.Math
Imports System.Threading.Tasks

Public Class PlayerService
    Private Shared Me_Instance As PlayerService = Nothing

    Private format As WaveFormatExtensible = Nothing
    Private device As XAudio2 = Nothing
    Private sourceVoice As SourceVoice = Nothing
    Private masteringVoice As MasteringVoice = Nothing

    Public PlayBuffers As New WavBuffer     '音源缓冲区

    'Private pState As XAUDIO2_VOICE_STATE       '缓冲区状态

    Private Status As Integer = 0

    Private Sub New()
    End Sub

    Public Shared ReadOnly Property Instance() As PlayerService
        Get
            If Me_Instance Is Nothing Then
                Me_Instance = New PlayerService()
            End If
            Return Me_Instance
        End Get
    End Property

    Public Sub Init()
        device = New XAudio2
    End Sub

    Public Sub SetFormat(inputFormat As WaveFormat)
        'format = inputFormat
        Dim f As New WaveFormatExtensible
        With f
            .Channels = 2
            .BitsPerSample = 16
            .FormatTag = WaveFormatTag.Extensible
            .BlockAlignment = 4
            .SamplesPerSecond = 4000
            .AverageBytesPerSecond = 16000
            .SubFormat = New Guid("00000001-0000-0010-8000-00aa00389b71")
        End With
        format = f
    End Sub

    Public Sub LoadBufferAndPlay()
        masteringVoice = New MasteringVoice(device)

        Dim tmpBuffer As AudioBuffer = PlayBuffers.GetBuffer(0)

        sourceVoice = New SourceVoice(device, format, VoiceFlags.None, 8)
        'sourceVoice.BufferStart += New EventHandler < ContextEventArgs > (sourceVoice_BufferStart);
        'sourceVoice.BufferEnd += New EventHandler < ContextEventArgs > (sourceVoice_BufferEnd);
        AddHandler sourceVoice.BufferEnd, AddressOf NextBuffer

        sourceVoice.SubmitSourceBuffer(tmpBuffer)

        sourceVoice.Start()

    End Sub

    Private Sub NextBuffer()
        Status += 1
        'sourceVoice.Stop()
        'If Status < 20 Then
        Dim tmpBuffer As AudioBuffer = PlayBuffers.GetBuffer(0)
        If tmpBuffer Is Nothing Then
            Call Dispose()
        End If
        sourceVoice.SubmitSourceBuffer(tmpBuffer)
        'sourceVoice.Start()
        'End If
    End Sub

    Public Function Dispose() As Integer
        sourceVoice.Stop()
        sourceVoice.Dispose()
        masteringVoice.Dispose()
        Return 0
    End Function


End Class


Public Class WavBuffer
    Public Pool As New List(Of Byte())

    Public Function GetBuffer(index As Integer) As AudioBuffer
        If Pool.Count = 0 Then Return Nothing
        Dim content As Byte() = Pool(index)
        Dim r As New AudioBuffer
        With r
            .AudioData = New DataStream(content, True, False)
            .AudioBytes = 16000
        End With
        Pool.RemoveAt(0)
        Return r

        'Dim arr(32000) As Byte
        'For i = 0 To 16000 - 1
        '    Dim value As Short
        '    If index >= 0 Then
        '        value = CShort(Sin(i * (0.5 + index * 0.05)) * 32767)
        '    End If
        '    Dim bts() As Byte = BitConverter.GetBytes(value)
        '    For j = 0 To 1
        '        arr(i * 2 + j) = bts(j)
        '    Next
        'Next
        'Dim r As New AudioBuffer
        'With r
        '    .AudioData = New DataStream(arr, True, False)
        '    .AudioBytes = 16000
        'End With
        'Return r
    End Function
End Class