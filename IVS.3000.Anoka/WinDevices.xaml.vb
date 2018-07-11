Imports System.IO.Ports
Imports System.ComponentModel
Imports System.Text
Imports System.Windows.Forms
Imports IVS.Eseek
Imports IVS.CTS
Imports IVS.Data
Imports IVS.Honeywell

Public Class WinDevices

    Private WithEvents objESeekDevice As ESeekApi
    Private WithEvents objHoneywellDevice As HoneywellApi
    Private strDevicePort As String
    'Private isContentChanged As Boolean = False
    Private WithEvents BWSerialPort As New BackgroundWorker

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Try
            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner

            Me.lblDeviceModelNumber.Content = Nothing
            Me.lblDeviceSerialNumber.Content = Nothing
            Me.lblDeviceApplicationRev.Content = Nothing
            Me.lblDeviceHardwareRev.Content = Nothing
            Me.lblDeviceVerifyStatus.Content = Nothing
            Me.lblDeviceVerifyStatus.Content = Nothing

            cboDevicePort_Load()
            cboDevices_Load()

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub WinDevices_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        Try

            Dim objDeviceInfo As New DeviceInfo
            objDeviceInfo = DataAccess.GetDeviceInfo(WinMain.objClientSettings.ClientID)

            Select Case objDeviceInfo.DeviceType

                Case "LS_40_USB"

                    Me.cboDevices.SelectedIndex = 0
                    Me.cboDevicePort.SelectedItem = "USB"
                    Me.lblDeviceModelNumber.Content = objDeviceInfo.ModelNo
                    Me.LabelDeviceSerialNumber.Content = "Unit ID:"
                    Me.lblDeviceSerialNumber.Content = objDeviceInfo.SerialNo
                    Me.LabelDeviceApplicationRev.Content = "Firmware Revision:"
                    Me.lblDeviceApplicationRev.Content = objDeviceInfo.FirmwareRev
                    Me.LabelDeviceHardwareRev.Content = "Firmware Date:"
                    Me.lblDeviceHardwareRev.Content = objDeviceInfo.FirmwareDate
                    Me.lblDeviceVerifyStatus.Content = "Updated on " & objDeviceInfo.UpdateTS

                Case "LS_150_USB"

                    Me.cboDevices.SelectedIndex = 1
                    Me.cboDevicePort.SelectedItem = "USB"
                    Me.lblDeviceModelNumber.Content = objDeviceInfo.ModelNo
                    Me.LabelDeviceSerialNumber.Content = "Unit ID:"
                    Me.lblDeviceSerialNumber.Content = objDeviceInfo.SerialNo
                    Me.LabelDeviceApplicationRev.Content = "Firmware Revision:"
                    Me.lblDeviceApplicationRev.Content = objDeviceInfo.FirmwareRev
                    Me.LabelDeviceHardwareRev.Content = "Firmware Date:"
                    Me.lblDeviceHardwareRev.Content = objDeviceInfo.FirmwareDate
                    Me.lblDeviceVerifyStatus.Content = "Updated on " & objDeviceInfo.UpdateTS

                Case "M250"

                    Me.cboDevices.SelectedIndex = 2
                    Me.cboDevicePort.SelectedItem = objDeviceInfo.ComPort
                    Me.lblDeviceModelNumber.Content = objDeviceInfo.ModelNo
                    Me.LabelDeviceSerialNumber.Content = "Serial Number:"
                    Me.lblDeviceSerialNumber.Content = objDeviceInfo.SerialNo
                    Me.LabelDeviceApplicationRev.Content = "Application Revision:"
                    Me.lblDeviceApplicationRev.Content = objDeviceInfo.FirmwareRev
                    Me.LabelDeviceHardwareRev.Content = "Hardware Revision:"
                    Me.lblDeviceHardwareRev.Content = objDeviceInfo.HardwareRev
                    Me.lblDeviceVerifyStatus.Content = "Updated on " & objDeviceInfo.UpdateTS

                Case "M260"

                    Me.cboDevices.SelectedIndex = 3
                    Me.cboDevicePort.SelectedItem = objDeviceInfo.ComPort
                    Me.lblDeviceModelNumber.Content = objDeviceInfo.ModelNo
                    Me.LabelDeviceSerialNumber.Content = "Serial Number:"
                    Me.lblDeviceSerialNumber.Content = objDeviceInfo.SerialNo
                    Me.LabelDeviceApplicationRev.Content = "Application Revision:"
                    Me.lblDeviceApplicationRev.Content = objDeviceInfo.FirmwareRev
                    Me.LabelDeviceHardwareRev.Content = "Hardware Revision:"
                    Me.lblDeviceHardwareRev.Content = objDeviceInfo.HardwareRev
                    Me.lblDeviceVerifyStatus.Content = "Updated on " & objDeviceInfo.UpdateTS

                Case "M280"

                    Me.cboDevices.SelectedIndex = 4
                    Me.cboDevicePort.SelectedItem = objDeviceInfo.ComPort
                    Me.lblDeviceModelNumber.Content = objDeviceInfo.ModelNo
                    Me.LabelDeviceSerialNumber.Content = "Serial Number:"
                    Me.lblDeviceSerialNumber.Content = objDeviceInfo.SerialNo
                    Me.LabelDeviceApplicationRev.Content = "Application Revision:"
                    Me.lblDeviceApplicationRev.Content = objDeviceInfo.FirmwareRev
                    Me.LabelDeviceHardwareRev.Content = "Hardware Revision:"
                    Me.lblDeviceHardwareRev.Content = objDeviceInfo.HardwareRev
                    Me.lblDeviceVerifyStatus.Content = "Updated on " & objDeviceInfo.UpdateTS

                Case "MAGTEK"

                    Me.cboDevices.SelectedIndex = 5
                    Me.cboDevicePort.SelectedItem = "USB"
                    Me.lblDeviceModelNumber.Content = Nothing
                    Me.LabelDeviceSerialNumber.Content = "Serial Number:"
                    Me.lblDeviceSerialNumber.Content = Nothing
                    Me.LabelDeviceApplicationRev.Content = "Application Revision:"
                    Me.lblDeviceApplicationRev.Content = Nothing
                    Me.LabelDeviceHardwareRev.Content = "Hardware Revision:"
                    Me.lblDeviceHardwareRev.Content = Nothing
                    Me.lblDeviceVerifyStatus.Content = "Updated on " & objDeviceInfo.UpdateTS

                Case "HW3310"

                    Me.cboDevices.SelectedIndex = 6
                    Me.cboDevicePort.SelectedItem = objDeviceInfo.ComPort
                    Me.lblDeviceModelNumber.Content = objDeviceInfo.ModelNo
                    Me.LabelDeviceSerialNumber.Content = "Serial Number:"
                    Me.lblDeviceSerialNumber.Content = objDeviceInfo.SerialNo
                    Me.LabelDeviceApplicationRev.Content = "Application Revision:"
                    Me.lblDeviceApplicationRev.Content = objDeviceInfo.FirmwareRev
                    Me.LabelDeviceHardwareRev.Content = "Hardware Revision:"
                    Me.lblDeviceHardwareRev.Content = objDeviceInfo.HardwareRev
                    Me.lblDeviceVerifyStatus.Content = "Updated on " & objDeviceInfo.UpdateTS

            End Select

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdCancel_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCancel.Click
        Try

            If objESeekDevice IsNot Nothing Then
                objESeekDevice.SerialPortClose()
            End If

            If objHoneywellDevice IsNot Nothing Then
                objHoneywellDevice.SerialPortClose()
            End If

            Me.DialogResult = False
            Me.Close()

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdSave_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdSave.Click

        Try

            If objESeekDevice IsNot Nothing Then
                objESeekDevice.SerialPortClose()
            End If

            If objHoneywellDevice IsNot Nothing Then
                objHoneywellDevice.SerialPortClose()
            End If

            Me.DialogResult = True
            Me.Close()

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#Region "Devices"

    Private Sub cboDevices_Load()

        Try
            Me.cboDevices.Items.Add("CTS LS40 USB")
            Me.cboDevices.Items.Add("CTS LS150 USB")
            Me.cboDevices.Items.Add("ESEEK M250")
            Me.cboDevices.Items.Add("ESEEK M260")
            Me.cboDevices.Items.Add("ESEEK M280")
            Me.cboDevices.Items.Add("MagTek")
            Me.cboDevices.Items.Add("Honeywell 3310")
            Me.cboDevices.Items.Add("Integrated Text Input")
            Me.cboDevices.Items.Add("No Reader")
            Me.cboDevices.SelectedIndex = 7

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
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

            Me.cboDevicePort.Items.Add("USB")
            Me.cboDevicePort.SelectedIndex = 0

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdDeviceVerify_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdDeviceVerify.Click

        Dim strDeviceType As String

        Try
            strDeviceType = Me.cboDevices.SelectedItem
            strDevicePort = Me.cboDevicePort.SelectedItem

            Select Case strDeviceType

                Case "ESEEK M250"

                    Me.lblDeviceVerifyStatus.Content = "Connecting to ESeek Device..."

                    If strDevicePort = "USB" Then
                        Me.lblDeviceVerifyStatus.Content = "Select a COM port to verify the ESeek Device"
                    Else

                        objESeekDevice = New ESeekApi(strDevicePort, WinMain.objClientSettings.SleepMilliSeconds)
                        objESeekDevice.SerialPortOpen()
                        objESeekDevice.VerifyESeek()

                    End If

                Case "ESEEK M260"

                    Me.lblDeviceVerifyStatus.Content = "Connecting to ESeek Device..."

                    If strDevicePort = "USB" Then
                        Me.lblDeviceVerifyStatus.Content = "Select a COM port to verify the ESeek Device"
                    Else

                        objESeekDevice = New ESeekApi(strDevicePort, WinMain.objClientSettings.SleepMilliSeconds)
                        objESeekDevice.SerialPortOpen()
                        objESeekDevice.VerifyESeek()

                    End If

                Case "ESEEK M280"

                    Me.lblDeviceVerifyStatus.Content = "Connecting to ESeek Device..."

                    If strDevicePort = "USB" Then
                        Me.lblDeviceVerifyStatus.Content = "Select a COM port to verify the ESeek Device"
                    Else

                        objESeekDevice = New ESeekApi(strDevicePort, WinMain.objClientSettings.SleepMilliSeconds)
                        objESeekDevice.SerialPortOpen()
                        objESeekDevice.VerifyESeek()

                    End If

                Case "CTS LS40 USB"

                    Me.lblDeviceVerifyStatus.Content = "Connecting to CTS Device..."

                    Dim MyCTSObject As New CTSApi()

                    AddHandler MyCTSObject.BWIdentifyCompleted, AddressOf BWCTSIdentify_RunWorkerCompleted
                    MyCTSObject.BWCTSIdentify_Start(LsFamily.LsDefines.LsUnitType.LS_40_USB)

                Case "CTS LS150 USB"

                    Me.lblDeviceVerifyStatus.Content = "Connecting to CTS Device..."

                    Dim MyCTSObject As New CTSApi()

                    AddHandler MyCTSObject.BWIdentifyCompleted, AddressOf BWCTSIdentify_RunWorkerCompleted
                    MyCTSObject.BWCTSIdentify_Start(LsFamily.LsDefines.LsUnitType.LS_150_USB)

                Case "MagTek"

                    Me.lblDeviceVerifyStatus.Content = "Connecting to MagTek Device..."
                    DeviceUpdate_MagTek()
                    Me.lblDeviceVerifyStatus.Content = "Successful connection to MagTek Device"

                Case "Honeywell 3310"

                    Me.lblDeviceVerifyStatus.Content = "Connecting to Honeywell Device..."

                    If strDevicePort = "USB" Then
                        Me.lblDeviceVerifyStatus.Content = "Select a COM port to verify the Honeywell Device"
                    Else

                        objHoneywellDevice = New HoneywellApi(strDevicePort)
                        objHoneywellDevice.SerialPortOpen()
                        objHoneywellDevice.VerifyDevice()

                    End If

                Case "Integrated Text Input"

                    DeviceUpdate_TextInput()
                    Me.lblDeviceVerifyStatus.Content = "Updated to integrated text input device"

                Case "No Reader"

                    DeviceUpdate_NoReader()
                    Me.lblDeviceVerifyStatus.Content = "Updated to no input device"

            End Select

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

#Region "CTS"

    Private Sub BWCTSIdentify_RunWorkerCompleted(ByVal Result As CTSDeviceInfo)

        Me.lblDeviceModelNumber.Content = Result.ModelNo
        Me.lblDeviceApplicationRev.Content = Result.FirmwareRev
        Me.lblDeviceHardwareRev.Content = Result.FirmwareDate
        Me.lblDeviceSerialNumber.Content = Result.SerialNo

        If Result.ModelNo IsNot Nothing Then

            Me.lblDeviceVerifyStatus.Content = "Successful connection to CTS Device"

            Dim objDeviceInfo As New DeviceInfo

            objDeviceInfo.ClientID = WinMain.objClientSettings.ClientID
            objDeviceInfo.DeviceID = WinMain.objClientSettings.DeviceID
            objDeviceInfo.ModelNo = Result.ModelNo
            objDeviceInfo.FirmwareRev = Result.FirmwareRev
            objDeviceInfo.FirmwareDate = Result.FirmwareDate
            objDeviceInfo.HardwareRev = "NULL"
            objDeviceInfo.ComPort = "USB"
            objDeviceInfo.SerialNo = Result.SerialNo
            objDeviceInfo.DeviceType = Result.DeviceType

            DataAccess.UpdateDevice(objDeviceInfo)

        Else

            Me.lblDeviceVerifyStatus.Content = "Unable to connect to CTS Device"

        End If

    End Sub

#End Region

#Region "ESeek"

    Private Sub objESeekDevice_DataReceived(DataReceived As String) Handles objESeekDevice.OnDataReceived, objHoneywellDevice.OnDataReceived

        Try

            Me.Dispatcher.BeginInvoke(New StartTheInvoke(AddressOf NowStartTheInvoke), DataReceived)

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Delegate Sub StartTheInvoke(ByVal DataReceived As String)

    Friend Sub NowStartTheInvoke(ByVal DataReceived As String)

        Try
            Me.BWSerialPort.RunWorkerAsync(DataReceived)

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWSerialPort_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles BWSerialPort.DoWork

        Dim bw As BackgroundWorker

        Try

            bw = CType(sender, BackgroundWorker)
            e.Result = TimeConsumingOperation_ESeekIdentify(e.Argument, bw)

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWSerialPort_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles BWSerialPort.RunWorkerCompleted

        If (e.Error IsNot Nothing) Then

            WinMain.MyAppLog.WriteToLog("IVS", e.Error.ToString, EventLogEntryType.Error)

        Else

            Try
                If e.Result.ModelNo.Contains("<FINGER") = True Then

                    Me.lblDeviceVerifyStatus.Content = "Unable to connect to ESeek Device"

                ElseIf e.Result.ModelNo.Contains("M2") Then

                    Me.lblDeviceModelNumber.Content = e.Result.ModelNo
                    Me.lblDeviceSerialNumber.Content = e.Result.SerialNo
                    Me.lblDeviceApplicationRev.Content = e.Result.FirmwareRev
                    Me.lblDeviceHardwareRev.Content = e.Result.HardwareRev

                    Me.lblDeviceVerifyStatus.Content = "Successful connection to ESeek Device"

                ElseIf e.Result.ModelNo.Contains("3310") Then

                    Me.lblDeviceModelNumber.Content = e.Result.ModelNo
                    Me.lblDeviceSerialNumber.Content = e.Result.SerialNo
                    Me.lblDeviceApplicationRev.Content = e.Result.FirmwareRev
                    Me.lblDeviceHardwareRev.Content = e.Result.HardwareRev

                    Me.lblDeviceVerifyStatus.Content = "Successful connection to Honeywell Device"

                End If

                If objESeekDevice IsNot Nothing Then
                    objESeekDevice.SerialPortClose()
                End If

                If objHoneywellDevice IsNot Nothing Then
                    objHoneywellDevice.SerialPortClose()
                End If

            Catch ex As Exception
                WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
            End Try

        End If

    End Sub

    Private Function TimeConsumingOperation_ESeekIdentify(ByVal DataReceived As String, ByVal bw As BackgroundWorker) As DeviceInfo

        Dim strDeviceIdentity As String()
        Dim objDeviceInfo As DeviceInfo
        objDeviceInfo = New DeviceInfo

        Try

            If DataReceived.Contains(":") Then
                'Honeywell

                DataReceived = Replace(DataReceived, Encoding.ASCII.GetChars({10}), ":")
                DataReceived = Replace(DataReceived, Encoding.ASCII.GetChars({13}), "")
                DataReceived = Trim(DataReceived)

                strDeviceIdentity = Split(DataReceived, ":")

                objDeviceInfo.ClientID = WinMain.objClientSettings.ClientID
                objDeviceInfo.DeviceID = WinMain.objClientSettings.DeviceID
                objDeviceInfo.ModelNo = Trim(strDeviceIdentity(1).Replace("Area-Imaging Scanner", ""))
                objDeviceInfo.SerialNo = strDeviceIdentity(11).ToString
                objDeviceInfo.FirmwareRev = strDeviceIdentity(9).ToString
                objDeviceInfo.HardwareRev = strDeviceIdentity(4).ToString
                objDeviceInfo.ComPort = strDevicePort
                objDeviceInfo.FirmwareDate = strDeviceIdentity(6).ToString
                objDeviceInfo.DeviceType = "HW" & Trim(strDeviceIdentity(1).Replace("Area-Imaging Scanner", "").Replace("Vuquest", ""))
                objHoneywellDevice.SerialPortClose()

            Else
                'ESeek

                DataReceived = Replace(DataReceived, Encoding.ASCII.GetChars({6}), "")
                DataReceived = Trim(DataReceived)
                DataReceived = Replace(DataReceived, vbCrLf, "<>")

                strDeviceIdentity = Split(DataReceived, "<>")

                objDeviceInfo.ClientID = WinMain.objClientSettings.ClientID
                objDeviceInfo.DeviceID = WinMain.objClientSettings.DeviceID
                objDeviceInfo.ModelNo = strDeviceIdentity(0).ToString
                objDeviceInfo.SerialNo = strDeviceIdentity(1).ToString
                objDeviceInfo.FirmwareRev = strDeviceIdentity(2).ToString
                objDeviceInfo.HardwareRev = strDeviceIdentity(3).ToString
                objDeviceInfo.ComPort = strDevicePort
                objDeviceInfo.FirmwareDate = "NULL"
                objDeviceInfo.DeviceType = String.Format("M{0}", strDeviceIdentity(1).ToString.Substring(0, 3))
                objESeekDevice.SerialPortClose()

            End If


            DataAccess.UpdateDevice(objDeviceInfo)

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        Return objDeviceInfo

    End Function

#End Region

#Region "Other Devices"

    Private Sub DeviceUpdate_MagTek()

        Dim objDeviceInfo As DeviceInfo

        Try

            objDeviceInfo = New DeviceInfo

            objDeviceInfo.ClientID = WinMain.objClientSettings.ClientID
            objDeviceInfo.DeviceID = WinMain.objClientSettings.DeviceID
            objDeviceInfo.ModelNo = 0
            objDeviceInfo.SerialNo = 0
            objDeviceInfo.FirmwareRev = 0
            objDeviceInfo.HardwareRev = 0
            objDeviceInfo.ComPort = "USB"
            objDeviceInfo.FirmwareDate = "NULL"
            objDeviceInfo.DeviceType = "MAGTEK"

            DataAccess.UpdateDevice(objDeviceInfo)

            Me.lblDeviceApplicationRev.Content = Nothing
            Me.lblDeviceHardwareRev.Content = Nothing
            Me.lblDeviceModelNumber.Content = Nothing
            Me.lblDeviceSerialNumber.Content = Nothing

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub DeviceUpdate_NoReader()

        Dim objDeviceInfo As DeviceInfo

        Try

            objDeviceInfo = New DeviceInfo

            objDeviceInfo.ClientID = WinMain.objClientSettings.ClientID
            objDeviceInfo.DeviceID = WinMain.objClientSettings.DeviceID
            objDeviceInfo.ModelNo = 0
            objDeviceInfo.SerialNo = 0
            objDeviceInfo.FirmwareRev = 0
            objDeviceInfo.HardwareRev = 0
            objDeviceInfo.ComPort = "USB"
            objDeviceInfo.FirmwareDate = "NULL"
            objDeviceInfo.DeviceType = "NONE"

            DataAccess.UpdateDevice(objDeviceInfo)

            Me.lblDeviceApplicationRev.Content = Nothing
            Me.lblDeviceHardwareRev.Content = Nothing
            Me.lblDeviceModelNumber.Content = Nothing
            Me.lblDeviceSerialNumber.Content = Nothing

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub DeviceUpdate_TextInput()

        Dim objDeviceInfo As DeviceInfo

        Try

            objDeviceInfo = New DeviceInfo

            objDeviceInfo.ClientID = WinMain.objClientSettings.ClientID
            objDeviceInfo.DeviceID = WinMain.objClientSettings.DeviceID
            objDeviceInfo.ModelNo = 0
            objDeviceInfo.SerialNo = 0
            objDeviceInfo.FirmwareRev = 0
            objDeviceInfo.HardwareRev = 0
            objDeviceInfo.ComPort = "USB"
            objDeviceInfo.FirmwareDate = "NULL"
            objDeviceInfo.DeviceType = "TEXT"

            DataAccess.UpdateDevice(objDeviceInfo)

            Me.lblDeviceApplicationRev.Content = Nothing
            Me.lblDeviceHardwareRev.Content = Nothing
            Me.lblDeviceModelNumber.Content = Nothing
            Me.lblDeviceSerialNumber.Content = Nothing

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

End Class
