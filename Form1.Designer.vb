<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.Btn1 = New System.Windows.Forms.Button()
        Me.Btn2 = New System.Windows.Forms.Button()
        Me.tb1 = New System.Windows.Forms.TextBox()
        Me.tb2 = New System.Windows.Forms.TextBox()
        Me.tb3 = New System.Windows.Forms.TextBox()
        Me.tb4 = New System.Windows.Forms.TextBox()
        Me.tb8 = New System.Windows.Forms.TextBox()
        Me.tb7 = New System.Windows.Forms.TextBox()
        Me.tb6 = New System.Windows.Forms.TextBox()
        Me.tb5 = New System.Windows.Forms.TextBox()
        Me.tb12 = New System.Windows.Forms.TextBox()
        Me.tb11 = New System.Windows.Forms.TextBox()
        Me.tb10 = New System.Windows.Forms.TextBox()
        Me.tb9 = New System.Windows.Forms.TextBox()
        Me.tb16 = New System.Windows.Forms.TextBox()
        Me.tb15 = New System.Windows.Forms.TextBox()
        Me.tb14 = New System.Windows.Forms.TextBox()
        Me.tb13 = New System.Windows.Forms.TextBox()
        Me.P = New System.Windows.Forms.PictureBox()
        Me.tbmsg = New System.Windows.Forms.TextBox()
        Me.tbcont = New System.Windows.Forms.TextBox()
        Me.P2 = New System.Windows.Forms.PictureBox()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        CType(Me.P, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.P2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Btn1
        '
        Me.Btn1.Location = New System.Drawing.Point(34, 29)
        Me.Btn1.Name = "Btn1"
        Me.Btn1.Size = New System.Drawing.Size(210, 77)
        Me.Btn1.TabIndex = 0
        Me.Btn1.Text = "打开midi文件"
        Me.Btn1.UseVisualStyleBackColor = True
        '
        'Btn2
        '
        Me.Btn2.Location = New System.Drawing.Point(261, 29)
        Me.Btn2.Name = "Btn2"
        Me.Btn2.Size = New System.Drawing.Size(210, 77)
        Me.Btn2.TabIndex = 1
        Me.Btn2.Text = "生成wave声音"
        Me.Btn2.UseVisualStyleBackColor = True
        '
        'tb1
        '
        Me.tb1.Location = New System.Drawing.Point(34, 165)
        Me.tb1.Name = "tb1"
        Me.tb1.Size = New System.Drawing.Size(105, 35)
        Me.tb1.TabIndex = 2
        Me.tb1.Tag = "1"
        '
        'tb2
        '
        Me.tb2.Location = New System.Drawing.Point(34, 206)
        Me.tb2.Name = "tb2"
        Me.tb2.Size = New System.Drawing.Size(105, 35)
        Me.tb2.TabIndex = 3
        Me.tb2.Tag = "2"
        '
        'tb3
        '
        Me.tb3.Location = New System.Drawing.Point(34, 247)
        Me.tb3.Name = "tb3"
        Me.tb3.Size = New System.Drawing.Size(105, 35)
        Me.tb3.TabIndex = 4
        Me.tb3.Tag = "3"
        '
        'tb4
        '
        Me.tb4.Location = New System.Drawing.Point(34, 288)
        Me.tb4.Name = "tb4"
        Me.tb4.Size = New System.Drawing.Size(105, 35)
        Me.tb4.TabIndex = 5
        Me.tb4.Tag = "4"
        '
        'tb8
        '
        Me.tb8.Location = New System.Drawing.Point(34, 452)
        Me.tb8.Name = "tb8"
        Me.tb8.Size = New System.Drawing.Size(105, 35)
        Me.tb8.TabIndex = 9
        Me.tb8.Tag = "8"
        '
        'tb7
        '
        Me.tb7.Location = New System.Drawing.Point(34, 411)
        Me.tb7.Name = "tb7"
        Me.tb7.Size = New System.Drawing.Size(105, 35)
        Me.tb7.TabIndex = 8
        Me.tb7.Tag = "7"
        '
        'tb6
        '
        Me.tb6.Location = New System.Drawing.Point(34, 370)
        Me.tb6.Name = "tb6"
        Me.tb6.Size = New System.Drawing.Size(105, 35)
        Me.tb6.TabIndex = 7
        Me.tb6.Tag = "6"
        '
        'tb5
        '
        Me.tb5.Location = New System.Drawing.Point(34, 329)
        Me.tb5.Name = "tb5"
        Me.tb5.Size = New System.Drawing.Size(105, 35)
        Me.tb5.TabIndex = 6
        Me.tb5.Tag = "5"
        '
        'tb12
        '
        Me.tb12.Location = New System.Drawing.Point(34, 616)
        Me.tb12.Name = "tb12"
        Me.tb12.Size = New System.Drawing.Size(105, 35)
        Me.tb12.TabIndex = 13
        Me.tb12.Tag = "12"
        '
        'tb11
        '
        Me.tb11.Location = New System.Drawing.Point(34, 575)
        Me.tb11.Name = "tb11"
        Me.tb11.Size = New System.Drawing.Size(105, 35)
        Me.tb11.TabIndex = 12
        Me.tb11.Tag = "11"
        '
        'tb10
        '
        Me.tb10.Location = New System.Drawing.Point(34, 534)
        Me.tb10.Name = "tb10"
        Me.tb10.Size = New System.Drawing.Size(105, 35)
        Me.tb10.TabIndex = 11
        Me.tb10.Tag = "10"
        '
        'tb9
        '
        Me.tb9.Location = New System.Drawing.Point(34, 493)
        Me.tb9.Name = "tb9"
        Me.tb9.Size = New System.Drawing.Size(105, 35)
        Me.tb9.TabIndex = 10
        Me.tb9.Tag = "9"
        '
        'tb16
        '
        Me.tb16.Location = New System.Drawing.Point(34, 780)
        Me.tb16.Name = "tb16"
        Me.tb16.Size = New System.Drawing.Size(105, 35)
        Me.tb16.TabIndex = 17
        Me.tb16.Tag = "16"
        '
        'tb15
        '
        Me.tb15.Location = New System.Drawing.Point(34, 739)
        Me.tb15.Name = "tb15"
        Me.tb15.Size = New System.Drawing.Size(105, 35)
        Me.tb15.TabIndex = 16
        Me.tb15.Tag = "15"
        '
        'tb14
        '
        Me.tb14.Location = New System.Drawing.Point(34, 698)
        Me.tb14.Name = "tb14"
        Me.tb14.Size = New System.Drawing.Size(105, 35)
        Me.tb14.TabIndex = 15
        Me.tb14.Tag = "14"
        '
        'tb13
        '
        Me.tb13.Location = New System.Drawing.Point(34, 657)
        Me.tb13.Name = "tb13"
        Me.tb13.Size = New System.Drawing.Size(105, 35)
        Me.tb13.TabIndex = 14
        Me.tb13.Tag = "13"
        '
        'P
        '
        Me.P.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.P.Location = New System.Drawing.Point(153, 120)
        Me.P.Name = "P"
        Me.P.Size = New System.Drawing.Size(1500, 700)
        Me.P.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.P.TabIndex = 18
        Me.P.TabStop = False
        '
        'tbmsg
        '
        Me.tbmsg.AcceptsReturn = True
        Me.tbmsg.Location = New System.Drawing.Point(34, 844)
        Me.tbmsg.Multiline = True
        Me.tbmsg.Name = "tbmsg"
        Me.tbmsg.ReadOnly = True
        Me.tbmsg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.tbmsg.Size = New System.Drawing.Size(1000, 200)
        Me.tbmsg.TabIndex = 19
        '
        'tbcont
        '
        Me.tbcont.Location = New System.Drawing.Point(34, 1050)
        Me.tbcont.Multiline = True
        Me.tbcont.Name = "tbcont"
        Me.tbcont.Size = New System.Drawing.Size(1000, 35)
        Me.tbcont.TabIndex = 20
        '
        'P2
        '
        Me.P2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.P2.Location = New System.Drawing.Point(1253, 844)
        Me.P2.Name = "P2"
        Me.P2.Size = New System.Drawing.Size(400, 200)
        Me.P2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.P2.TabIndex = 21
        Me.P2.TabStop = False
        '
        'Timer1
        '
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 24.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1730, 1129)
        Me.Controls.Add(Me.P2)
        Me.Controls.Add(Me.tbcont)
        Me.Controls.Add(Me.tbmsg)
        Me.Controls.Add(Me.P)
        Me.Controls.Add(Me.tb16)
        Me.Controls.Add(Me.tb15)
        Me.Controls.Add(Me.tb14)
        Me.Controls.Add(Me.tb13)
        Me.Controls.Add(Me.tb12)
        Me.Controls.Add(Me.tb11)
        Me.Controls.Add(Me.tb10)
        Me.Controls.Add(Me.tb9)
        Me.Controls.Add(Me.tb8)
        Me.Controls.Add(Me.tb7)
        Me.Controls.Add(Me.tb6)
        Me.Controls.Add(Me.tb5)
        Me.Controls.Add(Me.tb4)
        Me.Controls.Add(Me.tb3)
        Me.Controls.Add(Me.tb2)
        Me.Controls.Add(Me.tb1)
        Me.Controls.Add(Me.Btn2)
        Me.Controls.Add(Me.Btn1)
        Me.Name = "Form1"
        Me.Text = "Mid Viewer(Win32)"
        CType(Me.P, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.P2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Btn1 As Button
    Friend WithEvents Btn2 As Button
    Friend WithEvents tb1 As TextBox
    Friend WithEvents tb2 As TextBox
    Friend WithEvents tb3 As TextBox
    Friend WithEvents tb4 As TextBox
    Friend WithEvents tb8 As TextBox
    Friend WithEvents tb7 As TextBox
    Friend WithEvents tb6 As TextBox
    Friend WithEvents tb5 As TextBox
    Friend WithEvents tb12 As TextBox
    Friend WithEvents tb11 As TextBox
    Friend WithEvents tb10 As TextBox
    Friend WithEvents tb9 As TextBox
    Friend WithEvents tb16 As TextBox
    Friend WithEvents tb15 As TextBox
    Friend WithEvents tb14 As TextBox
    Friend WithEvents tb13 As TextBox
    Friend WithEvents P As PictureBox
    Friend WithEvents tbmsg As TextBox
    Friend WithEvents tbcont As TextBox
    Friend WithEvents P2 As PictureBox
    Friend WithEvents Timer1 As Timer
End Class
