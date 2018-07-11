Imports System.IO.Ports
Imports IVS.Honeywell
Imports System.Text

Class MainWindow

    Private WithEvents objSerialPort As HoneywellApi
    Private isVerifyDevice As Boolean = False

    Private Sub MainWindow_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Me.Closing

        Try

            If objSerialPort IsNot Nothing Then

                objSerialPort.SerialPortClose()

            End If

        Catch ex As Exception
            Me.TextBox1.Text += ex.ToString
        End Try

    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

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
            Me.TextBox1.Text += ex.ToString
        End Try

    End Sub

    Private Sub cmdConnect_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdConnect.Click

        Try
            Dim isSerialPortOpen As Boolean = False

            objSerialPort = New HoneywellApi(Me.cboDevicePort.SelectedItem.ToString)
            isSerialPortOpen = objSerialPort.SerialPortOpen()

            If isSerialPortOpen = False Then
                MessageBox.Show("Unable to connect to Honeywell scanner.  Ensure scanner is properly connected and open window again.", "IVS", MessageBoxButton.OK)

            Else
                Me.TextBox1.Text += "Successful connection to " & Me.cboDevicePort.SelectedItem.ToString & vbCrLf

            End If

        Catch ex As Exception
            Me.TextBox1.Text += ex.ToString
        End Try

    End Sub

    Private Sub objSerialPort_OnDataReceived(data As String) Handles objSerialPort.OnDataReceived

        Try
            Me.Dispatcher.BeginInvoke(New StartTheInvoke_OnDataReceived(AddressOf NowStartTheInvoke_OnDataReceived), data)
        Catch ex As Exception
            Me.TextBox1.Text += ex.ToString
        End Try

    End Sub

    Delegate Sub StartTheInvoke_OnDataReceived(ByVal StrRawSerialPortData As String)

    Friend Sub NowStartTheInvoke_OnDataReceived(ByVal StrRawSerialPortData As String)

        Try
            If isVerifyDevice = False Then

                Me.TextBox1.Text += StrRawSerialPortData & vbCrLf

            Else

                StrRawSerialPortData = Replace(StrRawSerialPortData, Encoding.ASCII.GetChars({10}), ":")
                StrRawSerialPortData = Replace(StrRawSerialPortData, Encoding.ASCII.GetChars({13}), "")

                StrRawSerialPortData = Trim(StrRawSerialPortData)

                Dim strDeviceIdentity As String()

                strDeviceIdentity = StrRawSerialPortData.Split(":")

                Me.TextBox1.Text += "HardwareRev:" & Trim(strDeviceIdentity(4)) & vbCrLf
                Me.TextBox1.Text += "FirmwareDate:" & Trim(strDeviceIdentity(6)) & vbCrLf
                Me.TextBox1.Text += "FirmwareRev:" & Trim(strDeviceIdentity(9)) & vbCrLf
                Me.TextBox1.Text += "SerialNo:" & Trim(strDeviceIdentity(11)) & vbCrLf

                Me.TextBox1.Text += "Model: " & Trim(strDeviceIdentity(1).Replace("Area-Imaging Scanner", "")) & vbCrLf
                'Me.TextBox1.Text += StrRawSerialPortData & vbCrLf

                isVerifyDevice = False

            End If

        Catch ex As Exception
            Me.TextBox1.Text += ex.ToString
        End Try

    End Sub

    Private Sub cmdVerifyDevice_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdVerifyDevice.Click

        objSerialPort.VerifyDevice()
        isVerifyDevice = True

    End Sub

End Class
