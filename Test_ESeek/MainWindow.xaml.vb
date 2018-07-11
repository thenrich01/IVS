Imports System.IO.Ports
Imports IVS.Eseek
'Imports System.ComponentModel

Class MainWindow

    Private WithEvents objSerialPort1 As ESeekApi
    Private WithEvents objSerialPort2 As ESeekApi
    'Private WithEvents BWSerialPort As New BackgroundWorker


    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        cboDevicePort_Load()
        cboDevices_Load()

    End Sub

    Private Sub cboDevicePort_Load()

        Dim ActiveComPorts As String()
        Dim intActiveComPorts As Integer

        Try
            ActiveComPorts = SerialPort.GetPortNames
            intActiveComPorts = ActiveComPorts.Count

            If intActiveComPorts > 0 Then

                For Each strActivePort As String In ActiveComPorts

                    Me.cboDevicePort1.Items.Add(strActivePort)
                    Me.cboDevicePort2.Items.Add(strActivePort)

                Next

            End If

            Me.cboDevicePort1.SelectedIndex = 0
            Me.cboDevicePort2.SelectedIndex = 0

        Catch ex As Exception
            Me.lblStatus.Content = ex.ToString
        End Try

    End Sub

    Private Sub cboDevices_Load()

        Try
            Me.cboDevices1.Items.Add("ESEEK M250")
            Me.cboDevices1.Items.Add("ESEEK M280")
            Me.cboDevices1.SelectedIndex = 0

            Me.cboDevices2.Items.Add("ESEEK M250")
            Me.cboDevices2.Items.Add("ESEEK M280")
            Me.cboDevices2.SelectedIndex = 0

        Catch ex As Exception
            Me.lblStatus.Content = ex.ToString
        End Try

    End Sub

    Private Sub cmdConnect1_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdConnect1.Click

        Dim strComPort As String
        Dim isSerialPortOpen As Boolean = False

        Try
            strComPort = Me.cboDevicePort1.SelectedItem

            objSerialPort1 = New ESeekApi(strComPort, 20)
            isSerialPortOpen = objSerialPort1.SerialPortOpen()

            If isSerialPortOpen = False Then
                MessageBox.Show("Unable to connect to ESeek reader1.  Ensure reader is properly connected and open window again.", "IVS", MessageBoxButton.OK)
            End If

        Catch ex As Exception
            Me.lblStatus.Content = ex.ToString
        End Try

    End Sub

    Private Sub cmdConnect2_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdConnect2.Click

        Dim strComPort As String
        Dim isSerialPortOpen As Boolean = False

        Try
            strComPort = Me.cboDevicePort2.SelectedItem

            objSerialPort2 = New ESeekApi(strComPort, 20)
            isSerialPortOpen = objSerialPort2.SerialPortOpen()

            If isSerialPortOpen = False Then
                MessageBox.Show("Unable to connect to ESeek reader2.  Ensure reader is properly connected and open window again.", "IVS", MessageBoxButton.OK)
            End If

        Catch ex As Exception
            Me.lblStatus.Content = ex.ToString
        End Try

    End Sub

#Region "ESeek"

    Private Sub objSerialPort1_OnDataReceived(data As String) Handles objSerialPort1.OnDataReceived

        Try
            data = "SP1:" & data
            Me.Dispatcher.BeginInvoke(New StartTheInvoke_OnDataReceived(AddressOf NowStartTheInvoke_OnDataReceived), data)
        Catch ex As Exception
            Me.lblStatus.Content = ex.ToString
        End Try

    End Sub

    Private Sub objSerialPort2_OnDataReceived(data As String) Handles objSerialPort2.OnDataReceived

        Try
            data = "SP2:" & data
            Me.Dispatcher.BeginInvoke(New StartTheInvoke_OnDataReceived(AddressOf NowStartTheInvoke_OnDataReceived), data)
        Catch ex As Exception
            Me.lblStatus.Content = ex.ToString
        End Try

    End Sub

    Delegate Sub StartTheInvoke_OnDataReceived(ByVal StrRawSerialPortData As String)

    Friend Sub NowStartTheInvoke_OnDataReceived(ByVal StrRawSerialPortData As String)

        Try
            Me.lblStatus.Content += "DATA:" & StrRawSerialPortData & vbCrLf

        Catch ex As Exception
            Me.lblStatus.Content = ex.ToString
        End Try

    End Sub

#End Region

    Private Sub MainWindow_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        Try
            If objSerialPort1 IsNot Nothing Then
                objSerialPort1.SerialPortClose()
            End If
            If objSerialPort2 IsNot Nothing Then
                objSerialPort2.SerialPortClose()
            End If
        Catch ex As Exception
            Me.lblStatus.Content = ex.ToString
        End Try
    End Sub

End Class
