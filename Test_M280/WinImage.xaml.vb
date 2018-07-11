Imports IVS.Eseek.M280

Public Class WinImage

    Public WithEvents MyM280 As IVS.Eseek.M280.ESeekM280Api
    Private WithEvents timCheckStatus As New Timers.Timer
    Dim M280Status As Byte() = New Byte(M280DEF.STATUS_SIZE) {}
    Dim bInitM280 As Boolean
    Dim bSysBusy As Boolean
    ' Private WithEvents objSerialPort As IVS.Eseek.ESeekApi

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Try

        Catch ex As Exception
            Me.TextBox1.Text = ex.ToString
        End Try

    End Sub

    Private Sub WinImage_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        Me.timCheckStatus.Stop()
        Me.timCheckStatus.Dispose()
        MyM280 = Nothing

    End Sub

    Private Sub WinImage_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        MyM280 = New IVS.Eseek.M280.ESeekM280Api
        Me.timCheckStatus.Interval = 100
        Me.timCheckStatus.Start()
    End Sub

    Public Sub MyTest_OnImageReceived(ImageReceived As System.Drawing.Bitmap) Handles MyM280.OnImageReceived
        'Me.imgScannedDocument.Source = GetBitmapSource(ImageReceived)
        Me.Dispatcher.BeginInvoke(New StartTheInvoke_OnImageReceived(AddressOf NowStartTheInvoke_OnImageReceived), ImageReceived)

    End Sub

    Delegate Sub StartTheInvoke_OnImageReceived(ByVal Image As System.Drawing.Bitmap)

    Friend Sub NowStartTheInvoke_OnImageReceived(ByVal Image As System.Drawing.Bitmap)

        Try
            'Me.TextBox1.Text = "NowStartTheInvoke_OnImageReceived"
            Me.imgScannedDocument.Source = GetBitmapSource(Image)

        Catch ex As Exception
            Me.TextBox1.Text = ex.ToString
        End Try

    End Sub

    Private Sub CheckState_Tick(sender As Object, e As EventArgs) Handles timCheckStatus.Elapsed
        Me.timCheckStatus.Stop()

        Dim len As Integer = M280DEF.STATUS_SIZE
        Dim StatusValue As UInt16

        MyM280.ReadCommand(M280DEF.CMD_GET_STATE, M280Status, len, 0, 0)

        StatusValue = BitConverter.ToUInt16(M280Status, 0)

        ' Check Push button
        If (StatusValue And M280DEF.stat_CapDet) = M280DEF.stat_CapDet Then
            'imgPushedButton.Dispatcher.BeginInvoke(New StartTheInvoke_UpdatePushedButton(AddressOf NowStartTheInvoke_UpdatePushedButton), True)
            MyM280.btnCapture_Click()
        Else
            'imgPushedButton.Dispatcher.BeginInvoke(New StartTheInvoke_UpdatePushedButton(AddressOf NowStartTheInvoke_UpdatePushedButton), False)
        End If

        Me.timCheckStatus.Start()
    End Sub

    Public Shared Function GetBitmapSource(source As System.Drawing.Bitmap) As BitmapSource

        Dim ip As IntPtr = source.GetHbitmap()
        Dim bs As BitmapSource = Nothing

        Try
            bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions())

        Catch ex As Exception
            'MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            DeleteObject(ip)

        End Try

        Return bs

    End Function

    <System.Runtime.InteropServices.DllImport("gdi32.dll")> _
    Public Shared Function DeleteObject(hObject As IntPtr) As Boolean
    End Function

    Private Sub cmdCapture_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCapture.Click
        MyM280.btnCapture_Click()
    End Sub
End Class
