Imports System.Drawing
'Imports IVS.Eseek
Imports System.ComponentModel
Imports System.IO.Ports
Imports IVS.Eseek.M280

Public Class MainWindow

    Public WithEvents MyM280 As IVS.Eseek.M280.ESeekM280Api
    Private WithEvents timCheckStatus As New Timers.Timer
    Dim M280Status As Byte() = New Byte(M280DEF.STATUS_SIZE) {}
    Dim bInitM280 As Boolean
    Dim bSysBusy As Boolean
    Private WithEvents objSerialPort As IVS.Eseek.ESeekApi

    ' Event TestOnImageReceived(ByVal ImageReceived As Bitmap)

    Private Sub Window1_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        If objSerialPort IsNot Nothing Then
            objSerialPort.SerialPortClose()
        End If
    End Sub

    Private Sub Window1_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        Try
            cboDevicePort_Load()
            'MyM280 = New IVS.Eseek.M280.ESeekM280Api
            'Me.timCheckStatus.Interval = 100
            'Me.timCheckStatus.Start()

        Catch ex As Exception
            Me.lblStatus.Content = ex.ToString
        End Try

    End Sub

    Private Sub cboDevicePort_Load()

        Dim ActiveComPorts As String()
        Dim intActiveComPorts As Integer

        Try
            ActiveComPorts = SerialPort.GetPortNames
            intActiveComPorts = ActiveComPorts.Count

            If intActiveComPorts > 0 Then

                For Each strActivePort As String In ActiveComPorts

                    Me.cboDevicePort.Items.Add(strActivePort)

                Next

            End If

            Me.cboDevicePort.SelectedIndex = 0

        Catch ex As Exception
            Me.lblStatus.Content = ex.ToString
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
            imgPushedButton.Dispatcher.BeginInvoke(New StartTheInvoke_UpdatePushedButton(AddressOf NowStartTheInvoke_UpdatePushedButton), True)
            MyM280.btnCapture_Click()
        Else
            imgPushedButton.Dispatcher.BeginInvoke(New StartTheInvoke_UpdatePushedButton(AddressOf NowStartTheInvoke_UpdatePushedButton), False)
        End If

        ' Check Busy LED
        If (StatusValue And M280DEF.stat_BusyLED) = M280DEF.stat_BusyLED Then
            imgBusyLED.Dispatcher.BeginInvoke(New StartTheInvoke_UpdateBusyLED(AddressOf NowStartTheInvoke_UpdateBusyLED), True)
        Else
            imgBusyLED.Dispatcher.BeginInvoke(New StartTheInvoke_UpdateBusyLED(AddressOf NowStartTheInvoke_UpdateBusyLED), False)
        End If

        ' Check Ready LED
        If (StatusValue And M280DEF.stat_ReadyLED) = M280DEF.stat_ReadyLED Then
            imgReadyLED.Dispatcher.BeginInvoke(New StartTheInvoke_UpdateReadyLED(AddressOf NowStartTheInvoke_UpdateReadyLED), True)
        Else
            imgReadyLED.Dispatcher.BeginInvoke(New StartTheInvoke_UpdateReadyLED(AddressOf NowStartTheInvoke_UpdateReadyLED), False)
        End If

        ' Check System Ready
        If (StatusValue And M280DEF.stat_CamInit) = M280DEF.stat_CamInit Then
            Me.bInitM280 = True
        Else
            Me.bInitM280 = False
        End If

        imgSystemReady.Dispatcher.BeginInvoke(New StartTheInvoke_UpdateSystemReady(AddressOf NowStartTheInvoke_UpdateSystemReady), bInitM280)

        ' Check System Busy
        If (StatusValue And M280DEF.stat_SysBusy) = M280DEF.stat_SysBusy Then
            Me.bSysBusy = True
        Else
            Me.bSysBusy = False
        End If

        imgSystemBusy.Dispatcher.BeginInvoke(New StartTheInvoke_UpdateSystemBusy(AddressOf NowStartTheInvoke_UpdateSystemBusy), bSysBusy)

        Me.timCheckStatus.Start()
    End Sub

    Delegate Sub StartTheInvoke_UpdateSystemReady(ByVal Status As Boolean)

    Friend Sub NowStartTheInvoke_UpdateSystemReady(ByVal Status As Boolean)

        Try
            Select Case Status

                Case True
                    Dim myBitmapImage As New BitmapImage(New Uri("/Test_M280;component/Resources/circle_green.png", UriKind.Relative))
                    imgSystemReady.Source = myBitmapImage
                Case False
                    Dim myBitmapImage As New BitmapImage(New Uri("/Test_M280;component/Resources/circle_gray.png", UriKind.Relative))
                    imgSystemReady.Source = myBitmapImage

            End Select

        Catch ex As Exception
            Me.lblStatus.Content = ex.ToString
        End Try

    End Sub

    Delegate Sub StartTheInvoke_UpdateSystemBusy(ByVal Status As Boolean)

    Friend Sub NowStartTheInvoke_UpdateSystemBusy(ByVal Status As Boolean)

        Try
            Select Case Status

                Case True
                    Dim myBitmapImage As New BitmapImage(New Uri("/Test_M280;component/Resources/circle_green.png", UriKind.Relative))
                    imgSystemBusy.Source = myBitmapImage
                Case False
                    Dim myBitmapImage As New BitmapImage(New Uri("/Test_M280;component/Resources/circle_gray.png", UriKind.Relative))
                    imgSystemBusy.Source = myBitmapImage

            End Select

        Catch ex As Exception
            Me.lblStatus.Content = ex.ToString
        End Try

    End Sub

    Delegate Sub StartTheInvoke_UpdateReadyLED(ByVal Status As Boolean)

    Friend Sub NowStartTheInvoke_UpdateReadyLED(ByVal Status As Boolean)

        Try
            Select Case Status

                Case True
                    Dim myBitmapImage As New BitmapImage(New Uri("/Test_M280;component/Resources/circle_green.png", UriKind.Relative))
                    imgReadyLED.Source = myBitmapImage
                Case False
                    Dim myBitmapImage As New BitmapImage(New Uri("/Test_M280;component/Resources/circle_gray.png", UriKind.Relative))
                    imgReadyLED.Source = myBitmapImage

            End Select

        Catch ex As Exception
            Me.lblStatus.Content = ex.ToString
        End Try

    End Sub

    Delegate Sub StartTheInvoke_UpdateBusyLED(ByVal Status As Boolean)

    Friend Sub NowStartTheInvoke_UpdateBusyLED(ByVal Status As Boolean)

        Try
            Select Case Status

                Case True
                    Dim myBitmapImage As New BitmapImage(New Uri("/Test_M280;component/Resources/circle_red.png", UriKind.Relative))
                    imgBusyLED.Source = myBitmapImage
                Case False
                    Dim myBitmapImage As New BitmapImage(New Uri("/Test_M280;component/Resources/circle_gray.png", UriKind.Relative))
                    imgBusyLED.Source = myBitmapImage

            End Select

        Catch ex As Exception
            Me.lblStatus.Content = ex.ToString
        End Try

    End Sub

    Delegate Sub StartTheInvoke_UpdatePushedButton(ByVal Status As Boolean)

    Friend Sub NowStartTheInvoke_UpdatePushedButton(ByVal Status As Boolean)

        Try
            Select Case Status

                Case True
                    Dim myBitmapImage As New BitmapImage(New Uri("/Test_M280;component/Resources/circle_green.png", UriKind.Relative))
                    imgPushedButton.Source = myBitmapImage
                Case False
                    Dim myBitmapImage As New BitmapImage(New Uri("/Test_M280;component/Resources/circle_gray.png", UriKind.Relative))
                    imgPushedButton.Source = myBitmapImage

            End Select

        Catch ex As Exception
            Me.lblStatus.Content = ex.ToString
        End Try

    End Sub

    Private Sub cmdCapture_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCapture.Click

        Try
            MyM280.btnCapture_Click()
        Catch ex As Exception
            Me.lblStatus.Content = ex.ToString
        End Try

    End Sub

    'Private Sub objSerialPort_OnImageReceived(Image As Bitmap) Handles MyM280.OnImageReceived

    '    Try
    '        'RaiseEvent TestOnImageReceived(Image)
    '        Me.Dispatcher.BeginInvoke(New StartTheInvoke_OnImageReceived(AddressOf NowStartTheInvoke_OnImageReceived), Image)
    '    Catch ex As Exception
    '        Me.lblStatus.Content = ex.ToString
    '    End Try

    'End Sub

    Delegate Sub StartTheInvoke_OnImageReceived(ByVal Image As Bitmap)

    Friend Sub NowStartTheInvoke_OnImageReceived(ByVal Image As Bitmap)

        Try
            Me.picImage.Source = MyM280.GetBitmapSource(Image)
            'RaiseEvent TestOnImageReceived(Image)

        Catch ex As Exception
            Me.lblStatus.Content = ex.ToString
        End Try

    End Sub

#Region "ESeek"

    Private Sub objSerialPort_OnDataReceived(data As String) Handles objSerialPort.OnDataReceived

        Try
            Me.Dispatcher.BeginInvoke(New StartTheInvoke_OnDataReceived(AddressOf NowStartTheInvoke_OnDataReceived), data)
        Catch ex As Exception
            Me.lblStatus.Content = ex.ToString
        End Try

    End Sub

    Delegate Sub StartTheInvoke_OnDataReceived(ByVal StrRawSerialPortData As String)

    Friend Sub NowStartTheInvoke_OnDataReceived(ByVal StrRawSerialPortData As String)

        Try
            Me.lblStatus.Content = StrRawSerialPortData

        Catch ex As Exception
            Me.lblStatus.Content = ex.ToString
        End Try

    End Sub

#End Region

    Private Sub cmdConnect_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdConnect.Click

        Dim strDevicePort As String

        Try
            strDevicePort = Me.cboDevicePort.SelectedItem

            objSerialPort = New IVS.Eseek.ESeekApi(strDevicePort, 100)
            objSerialPort.SerialPortOpen()

        Catch ex As Exception
            Me.lblStatus.Content = ex.ToString
        End Try

    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles Button1.Click
        Try
            Dim winImage As New WinImage()

            winImage.Owner = Me
            winImage.ShowDialog()

        Catch ex As Exception

        End Try
    End Sub
End Class
