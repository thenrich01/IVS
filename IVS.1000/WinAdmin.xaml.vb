'Imports System.Data
Imports System.IO.Ports
Imports System.ComponentModel
Imports System.Text
Imports System.Windows.Forms
Imports IVS.Eseek
Imports IVS.CTS
Imports IVS.Data
Imports IVS.AppLog
Imports System.Runtime.InteropServices
Imports System.Threading
Imports IVS.Epson
Imports com.epson.bank.driver

Public Class WinAdmin

    'Private MyAppLog As New ApplicationLog(ApplicationLog.DateEncodingType.YYYYMMDD, True)
    'Private WinMain.objClientSettings As ClientSettings
    'Private intIVSUserID As Integer
    Private WithEvents objESeekDevice As ESeekApi
    Private strDevicePort As String
    Private isContentChanged As Boolean = False
    Private WithEvents BWSerialPort As New BackgroundWorker
    Private intAlertID As Integer
    Private intUserID As Integer
    Private WithEvents BWDeleteScans As BackgroundWorker

    Private m_objDriverControl As CApp = Nothing
    Private m_objConfigData As StructByStep = Nothing

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        'Dim intClientID As Integer

        Try
            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner

            'intClientID = DataAccess.GetClientID()
            'intIVSUserID = IVSUserID

            'WinMain.MyAppLog.WriteToLog("Client ID: " & intClientID)

            'WinMain.objClientSettings = DataAccess.GetClientSettings(intClientID)

            ClearLabelsOnLoad()
            cboDevicePort_Load()
            cboClientDefaultUser_Load()
            cboDevices_Load()
            IDecodeTab_Load()

            Me.dgAlerts.ItemsSource = DataAccess.GetAlerts
            Me.dgUsers.ItemsSource = DataAccess.GetUsers
            Me.dgUCCReport.ItemsSource = DataAccess.GetUCCReport(Today)

            Me.lblEdition.Content = My.Application.Info.Description
            Me.lblVersion.Content = My.Application.Info.ProductName
            Me.lblBuild.Content = My.Application.Info.Version.ToString

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#Region "Control Bound Subs"

    Private Sub WinAdmin_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Me.Closing

        Try
            If objESeekDevice IsNot Nothing Then
                objESeekDevice.SerialPortClose()
            End If
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub WinAdmin_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        Try
            Me.txtClientLocation.Text = WinMain.objClientSettings.Location
            Me.txtClientStation.Text = WinMain.objClientSettings.Station
            Me.txtClientPhone.Text = WinMain.objClientSettings.Phone
            Me.txtClientEmail.Text = WinMain.objClientSettings.Email
            Me.cbClientSkipLogon.IsChecked = WinMain.objClientSettings.SkipLogon

            Me.txtViewingTime.Text = WinMain.objClientSettings.ViewingTime
            Me.cbDisableCCSave.IsChecked = WinMain.objClientSettings.DisableCCSave
            Me.cbDisableDBSave.IsChecked = WinMain.objClientSettings.DisableDBSave
            Me.txtCCDigits.Text = WinMain.objClientSettings.CCDigits
            Me.cbAgeHighlight.IsChecked = WinMain.objClientSettings.AgeHighlight
            Me.cbAgePopup.IsChecked = WinMain.objClientSettings.AgePopup
            Me.txtAge.Text = WinMain.objClientSettings.Age
            Me.cbImageSave.IsChecked = WinMain.objClientSettings.ImageSave
            Me.txtImageLocation.Text = WinMain.objClientSettings.ImageLocation

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

                Case "TM-S9000"

                    Me.cboDevices.SelectedIndex = 2
                    Me.cboDevicePort.SelectedItem = "USB"
                    Me.lblDeviceModelNumber.Content = "TM-S9000"
                    Me.LabelDeviceSerialNumber.Content = "Serial Number:"
                    Me.lblDeviceSerialNumber.Content = Nothing
                    Me.LabelDeviceApplicationRev.Content = "Application Revision:"
                    Me.lblDeviceApplicationRev.Content = Nothing
                    Me.LabelDeviceHardwareRev.Content = "Hardware Revision:"
                    Me.lblDeviceHardwareRev.Content = Nothing
                    Me.lblDeviceVerifyStatus.Content = "Updated on " & objDeviceInfo.UpdateTS

                Case "M200/250"

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

                Case "M210/260"

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

                Case "TEXT"

                    Me.cboDevices.SelectedIndex = 6
                    Me.cboDevicePort.SelectedItem = "USB"
                    Me.lblDeviceModelNumber.Content = Nothing
                    Me.LabelDeviceSerialNumber.Content = "Serial Number:"
                    Me.lblDeviceSerialNumber.Content = Nothing
                    Me.LabelDeviceApplicationRev.Content = "Application Revision:"
                    Me.lblDeviceApplicationRev.Content = Nothing
                    Me.LabelDeviceHardwareRev.Content = "Hardware Revision:"
                    Me.lblDeviceHardwareRev.Content = Nothing
                    Me.lblDeviceVerifyStatus.Content = "Updated on " & objDeviceInfo.UpdateTS

                Case "NONE"

                    Me.cboDevices.SelectedIndex = 7
                    Me.cboDevicePort.SelectedItem = "USB"
                    Me.lblDeviceModelNumber.Content = Nothing
                    Me.LabelDeviceSerialNumber.Content = "Serial Number:"
                    Me.lblDeviceSerialNumber.Content = Nothing
                    Me.LabelDeviceApplicationRev.Content = "Application Revision:"
                    Me.lblDeviceApplicationRev.Content = Nothing
                    Me.LabelDeviceHardwareRev.Content = "Hardware Revision:"
                    Me.lblDeviceHardwareRev.Content = Nothing
                    Me.lblDeviceVerifyStatus.Content = "Updated on " & objDeviceInfo.UpdateTS

            End Select

            AddHandler txtClientLocation.TextChanged, AddressOf TextChanged
            AddHandler txtClientStation.TextChanged, AddressOf TextChanged
            AddHandler txtClientPhone.TextChanged, AddressOf TextChanged
            AddHandler txtClientEmail.TextChanged, AddressOf TextChanged
            AddHandler cbClientSkipLogon.Click, AddressOf TextChanged
            AddHandler cboClientDefaultUser.SelectionChanged, AddressOf TextChanged
            AddHandler txtViewingTime.TextChanged, AddressOf TextChanged
            AddHandler cbDisableCCSave.Click, AddressOf TextChanged
            AddHandler cbDisableDBSave.Click, AddressOf TextChanged
            AddHandler txtCCDigits.TextChanged, AddressOf TextChanged
            AddHandler cbAgeHighlight.Click, AddressOf TextChanged
            AddHandler cbAgePopup.Click, AddressOf TextChanged
            AddHandler txtAge.TextChanged, AddressOf TextChanged
            AddHandler cbImageSave.Click, AddressOf TextChanged
            AddHandler txtImageLocation.TextChanged, AddressOf TextChanged

            Me.cmdSave.IsEnabled = False

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdSave_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdSave.Click

        Dim objNewClientSettings As New ClientSettings
        Dim objDefaultUser_SelectedRow As UserDetail

        Try
            objNewClientSettings.ClientID = WinMain.objClientSettings.ClientID
            objNewClientSettings.Location = Me.txtClientLocation.Text
            objNewClientSettings.Station = Me.txtClientStation.Text
            objNewClientSettings.Phone = Me.txtClientPhone.Text
            objNewClientSettings.Email = Me.txtClientEmail.Text
            objNewClientSettings.SkipLogon = Me.cbClientSkipLogon.IsChecked

            objDefaultUser_SelectedRow = Me.cboClientDefaultUser.SelectedItem
            intUserID = objDefaultUser_SelectedRow.UserID

            objNewClientSettings.DefaultUser = objDefaultUser_SelectedRow.UserID
            objNewClientSettings.ViewingTime = Me.txtViewingTime.Text
            objNewClientSettings.DisableCCSave = Me.cbDisableCCSave.IsChecked
            objNewClientSettings.DisableDBSave = Me.cbDisableDBSave.IsChecked
            objNewClientSettings.CCDigits = Me.txtCCDigits.Text
            objNewClientSettings.AgeHighlight = Me.cbAgeHighlight.IsChecked
            objNewClientSettings.AgePopup = Me.cbAgePopup.IsChecked
            objNewClientSettings.Age = Me.txtAge.Text
            objNewClientSettings.ImageSave = Me.cbImageSave.IsChecked
            objNewClientSettings.ImageLocation = Me.txtImageLocation.Text

            DataAccess.SaveClientSettings(objNewClientSettings)

            isContentChanged = False
            Me.cmdSave.IsEnabled = False

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdCancel_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCancel.Click

        Try
            If isContentChanged = True Then

                If MessageBox.Show("Exit without saving changes?", "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

                    Me.DialogResult = True
                    Me.Close()

                End If
            Else
                Me.DialogResult = True
                Me.Close()
            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub txtImageLocation_GotFocus(sender As Object, e As System.Windows.RoutedEventArgs) Handles txtImageLocation.GotFocus

        Dim MyFolderBrowserDialog As New System.Windows.Forms.FolderBrowserDialog
        Dim strImageLocation As String = Me.txtImageLocation.Text

        Try
            Dim result As DialogResult = MyFolderBrowserDialog.ShowDialog()

            If (result = Forms.DialogResult.OK) Then

                strImageLocation = MyFolderBrowserDialog.SelectedPath

                Me.txtImageLocation.Text = MyFolderBrowserDialog.SelectedPath

            End If
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try
    End Sub

    Private Sub cmdBrowseSourceFolder_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdBrowseSourceFolder.Click
        Dim MyFolderBrowserDialog As New System.Windows.Forms.FolderBrowserDialog
        Dim strImageLocation As String = Me.txtImageLocation.Text

        Try
            Dim result As DialogResult = MyFolderBrowserDialog.ShowDialog()

            If (result = Forms.DialogResult.OK) Then

                strImageLocation = MyFolderBrowserDialog.SelectedPath

                Me.txtImageLocation.Text = MyFolderBrowserDialog.SelectedPath

            End If
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try
    End Sub

#End Region

#Region "Custom Subs/Functions"

    Private Sub ClearLabelsOnLoad()

        Try

            Me.lblDeviceModelNumber.Content = Nothing
            Me.lblDeviceSerialNumber.Content = Nothing
            Me.lblDeviceApplicationRev.Content = Nothing
            Me.lblDeviceHardwareRev.Content = Nothing
            Me.lblDeviceVerifyStatus.Content = Nothing

            Me.lblDeviceVerifyStatus.Content = Nothing

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cboClientDefaultUser_Load()

        Try

            Me.cboClientDefaultUser.ItemsSource = DataAccess.GetUserNames
            Me.cboClientDefaultUser.DisplayMemberPath = ("UserName")
            Me.cboClientDefaultUser.SelectedValuePath = ("UserID")
            Me.cboClientDefaultUser.SelectedValue = WinMain.objClientSettings.DefaultUser

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub TextChanged()

        isContentChanged = True
        Me.cmdSave.IsEnabled = True

    End Sub

#End Region

#Region "Users"

    Private Sub dgUsers_SelectionChanged(sender As Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgUsers.SelectionChanged

        Dim dgUsers_SelectedRow As System.Collections.IList

        Try
            dgUsers_SelectedRow = e.AddedItems

            If dgUsers_SelectedRow.Count > 0 Then

                intUserID = dgUsers_SelectedRow(0).UserID

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgUsers_CmdView_Click()

        Dim WinUser As WinUser
        Dim DialogResult As Boolean

        Try

            WinUser = New WinUser(intUserID, True)
            WinUser.Owner = Me

            DialogResult = WinUser.ShowDialog

            If DialogResult = True Then
                Me.dgUsers.ItemsSource = DataAccess.GetUsers
                cboClientDefaultUser_Load()
            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgUsers_CmdEnable_Click()

        Try
            DataAccess.EnableUser(intUserID, True)
            Me.dgUsers.ItemsSource = DataAccess.GetUsers
            cboClientDefaultUser_Load()
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgUsers_CmdDisable_Click()

        Try
            DataAccess.EnableUser(intUserID, False)
            Me.dgUsers.ItemsSource = DataAccess.GetUsers
            cboClientDefaultUser_Load()
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdNewUser_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdNewUser.Click

        Dim WinUser As WinUser
        Dim DialogResult As Boolean

        Try

            WinUser = New WinUser()
            WinUser.Owner = Me

            DialogResult = WinUser.ShowDialog

            If DialogResult = True Then
                Me.dgUsers.ItemsSource = DataAccess.GetUsers
                cboClientDefaultUser_Load()
            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

#Region "Alerts"

    Private Sub dgAlerts_SelectionChanged(sender As Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgAlerts.SelectionChanged

        Dim dgAlerts_SelectedRow As System.Collections.IList

        Try
            dgAlerts_SelectedRow = e.AddedItems

            If dgAlerts_SelectedRow.Count > 0 Then

                intAlertID = dgAlerts_SelectedRow(0).AlertID

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgAlerts_CmdView_Click()

        Dim WinAlert As WinAlert
        Dim DialogResult As Boolean
        Dim objAlertDetail As New AlertDetail

        Try
            objAlertDetail = DataAccess.GetAlertDetail(intAlertID)
            WinAlert = New WinAlert(objAlertDetail, True)
            WinAlert.Owner = Me

            DialogResult = WinAlert.ShowDialog

            If DialogResult = True Then
                Me.dgAlerts.ItemsSource = DataAccess.GetAlerts
            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgAlerts_CmdEnable_Click()

        Try
            DataAccess.EnableAlert(intAlertID, True)
            Me.dgAlerts.ItemsSource = DataAccess.GetAlerts

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgAlerts_CmdDisable_Click()

        Try
            DataAccess.EnableAlert(intAlertID, False)
            Me.dgAlerts.ItemsSource = DataAccess.GetAlerts

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdNewAlert_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdNewAlert.Click

        Dim WinAlert As WinAlert
        Dim DialogResult As Boolean
        Dim objAlertDetail As New AlertDetail

        Try
            objAlertDetail.AlertID = 0
            objAlertDetail.AlertContactName = WinMain.strIVSUserName
            objAlertDetail.AlertContactNumber = WinMain.objClientSettings.Phone
            objAlertDetail.UserID = WinMain.intIVSUserID

            WinAlert = New WinAlert(objAlertDetail)
            WinAlert.Owner = Me

            DialogResult = WinAlert.ShowDialog

            If DialogResult = True Then
                Me.dgAlerts.ItemsSource = DataAccess.GetAlerts
            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

#Region "Devices"

    Private Sub cboDevices_Load()

        Try
            Me.cboDevices.Items.Add("CTS LS40 USB")
            Me.cboDevices.Items.Add("CTS LS150 USB")
            Me.cboDevices.Items.Add("EPSON TM-S9000")
            Me.cboDevices.Items.Add("ESEEK M250")
            Me.cboDevices.Items.Add("ESEEK M280")
            Me.cboDevices.Items.Add("MagTek")
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

            WinMain.MyAppLog.WriteToLog(intActiveComPorts & " active serial port(s) were found:")

            If intActiveComPorts > 0 Then

                For Each strActivePort As String In ActiveComPorts

                    Me.cboDevicePort.Items.Add(strActivePort)
                    WinMain.MyAppLog.WriteToLog("Active serial port: " & strActivePort)

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

                Case "Integrated Text Input"

                    DeviceUpdate_TextInput()
                    Me.lblDeviceVerifyStatus.Content = "Updated to integrated text input device"

                Case "No Reader"

                    DeviceUpdate_NoReader()
                    Me.lblDeviceVerifyStatus.Content = "Updated to no input device"

                Case "EPSON TM-S9000"
                    'TODO Test here
                    m_objDriverControl = New CApp()
                    m_objConfigData = New StructByStep()

                    If InitializeDriver() <> True Then
                        Me.lblDeviceVerifyStatus.Content = "Failed to connect to Epson TM-S9000"
                    Else
                        Me.lblDeviceVerifyStatus.Content = "Successful connection to Epson TM-S9000"
                        DeviceUpdate_Epson()
                        Me.lblDeviceModelNumber.Content = "TM-S9000"
                    End If
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

    Private Sub objESeekDevice_DataReceived(DataReceived As String) Handles objESeekDevice.OnDataReceived

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

                Else
                    Me.lblDeviceModelNumber.Content = e.Result.ModelNo
                    Me.lblDeviceSerialNumber.Content = e.Result.SerialNo
                    Me.lblDeviceApplicationRev.Content = e.Result.FirmwareRev
                    Me.lblDeviceHardwareRev.Content = e.Result.HardwareRev

                    Me.lblDeviceVerifyStatus.Content = "Successful connection to ESeek Device"

                End If

            Catch ex As Exception
                WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
            End Try

        End If

    End Sub

    Private Function TimeConsumingOperation_ESeekIdentify(ByVal DataReceived As String, ByVal bw As BackgroundWorker) As DeviceInfo

        Dim strESeekIdentity As String()
        Dim objDeviceInfo As DeviceInfo

        Try

            objDeviceInfo = New DeviceInfo

            DataReceived = Replace(DataReceived, Encoding.ASCII.GetChars({6}), "")
            DataReceived = Trim(DataReceived)
            DataReceived = Replace(DataReceived, vbCrLf, "<>")

            strESeekIdentity = Split(DataReceived, "<>")

            objDeviceInfo.ClientID = WinMain.objClientSettings.ClientID
            objDeviceInfo.DeviceID = WinMain.objClientSettings.DeviceID
            objDeviceInfo.ModelNo = strESeekIdentity(0).ToString
            objDeviceInfo.SerialNo = strESeekIdentity(1).ToString
            objDeviceInfo.FirmwareRev = strESeekIdentity(2).ToString
            objDeviceInfo.HardwareRev = strESeekIdentity(3).ToString
            objDeviceInfo.ComPort = strDevicePort
            objDeviceInfo.FirmwareDate = "NULL"
            objDeviceInfo.DeviceType = strESeekIdentity(0).ToString
            objESeekDevice.SerialPortClose()

            DataAccess.UpdateDevice(objDeviceInfo)

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        Return objDeviceInfo

    End Function

#End Region

#Region "Epson"

    Private Function InitializeDriver() As String
        Dim errResult As ErrorCode = ErrorCode.SUCCESS
        Dim drRet As DialogResult = 0
        Dim strResult As String = Nothing

        If m_objDriverControl Is Nothing Then
            Return False
        End If

        ' Open device and register callback functions
        errResult = m_objDriverControl.InitDevice()
        While errResult <> ErrorCode.SUCCESS
            If errResult = ErrorCode.ERR_UNKNOWN Then
                drRet = MessageBox.Show(ConstComStr.MSG_06_000, ConstComStr.CAPTION_06_000, MessageBoxButtons.OKCancel, MessageBoxIcon.[Error])
            Else
                strResult = IVS.Epson.Epson.GetErrorString(errResult)
                drRet = MessageBox.Show(ConstComStr.MSG_00_000, strResult, MessageBoxButtons.OKCancel, MessageBoxIcon.[Error])

            End If

            If drRet = System.Windows.Forms.DialogResult.OK Then
                errResult = m_objDriverControl.InitDevice()
            Else
                Return False
            End If
        End While

        Return True

    End Function

    Private Sub DeviceUpdate_Epson()

        Dim objDeviceInfo As DeviceInfo

        Try

            objDeviceInfo = New DeviceInfo

            objDeviceInfo.ClientID = WinMain.objClientSettings.ClientID
            objDeviceInfo.DeviceID = WinMain.objClientSettings.DeviceID
            objDeviceInfo.ModelNo = "TM-S9000"
            objDeviceInfo.SerialNo = 0
            objDeviceInfo.FirmwareRev = 0
            objDeviceInfo.HardwareRev = 0
            objDeviceInfo.ComPort = "USB"
            objDeviceInfo.FirmwareDate = "NULL"
            objDeviceInfo.DeviceType = "TM-S9000"

            DataAccess.UpdateDevice(objDeviceInfo)

            Me.lblDeviceApplicationRev.Content = Nothing
            Me.lblDeviceHardwareRev.Content = Nothing
            Me.lblDeviceModelNumber.Content = Nothing
            Me.lblDeviceSerialNumber.Content = Nothing

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub CloseDevice()

        Dim errRet As ErrorCode = ErrorCode.SUCCESS

        errRet = m_objDriverControl.ExitDevice()

        'If errRet <> ErrorCode.SUCCESS Then

        'End If

    End Sub

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

#Region "IDecode"

    Private Sub IDecodeTab_Load()

        Dim intIDecodeLicenseStatus As Integer

        Try

            intIDecodeLicenseStatus = Application.objMyIDChecker.LicenseStatus

            Me.cmdIDecodeRegister.IsEnabled = False

            Select Case intIDecodeLicenseStatus

                Case 0
                    Me.lblIDecodeLicenseStatus.Content = "License status OK"
                    Me.txtIDecodeLicenseKey.Text = WinMain.objClientSettings.IDecodeLicense
                    Me.lblIDecodeTrialDaysRemaining.Visibility = Windows.Visibility.Hidden
                    Me.labelIDecodeTrialDaysRemaining.Visibility = Windows.Visibility.Hidden

                Case 1
                    Me.lblIDecodeLicenseStatus.Content = "Trial version within evaluation period"
                    Me.labelIDecodeTrialDaysRemaining.Visibility = Windows.Visibility.Visible
                    Me.lblIDecodeTrialDaysRemaining.Visibility = Windows.Visibility.Visible
                    Me.lblIDecodeTrialDaysRemaining.Content = Application.objMyIDChecker.TrialDaysRemaining
                    Me.lblIDecodeTrialDaysRemaining.Visibility = Windows.Visibility.Visible
                    Me.labelIDecodeTrialDaysRemaining.Visibility = Windows.Visibility.Visible

                Case 2
                    Me.lblIDecodeLicenseStatus.Content = "Trial version beyond evaluation"
                    Me.cmdIDecodeRegister.IsEnabled = True
                    Me.lblIDecodeTrialDaysRemaining.Visibility = Windows.Visibility.Hidden
                    Me.labelIDecodeTrialDaysRemaining.Visibility = Windows.Visibility.Hidden

                Case 3
                    Me.lblIDecodeLicenseStatus.Content = "Unregistered copy"
                    Me.cmdIDecodeRegister.IsEnabled = True
                    Me.lblIDecodeTrialDaysRemaining.Visibility = Windows.Visibility.Hidden
                    Me.labelIDecodeTrialDaysRemaining.Visibility = Windows.Visibility.Hidden

                Case Else
                    Me.lblIDecodeLicenseStatus.Content = "Error retrieving license status"
                    Me.cmdIDecodeRegister.IsEnabled = True
                    Me.lblIDecodeTrialDaysRemaining.Visibility = Windows.Visibility.Hidden
                    Me.labelIDecodeTrialDaysRemaining.Visibility = Windows.Visibility.Hidden

            End Select

            Me.lblIDecodeVersion.Content = Application.objMyIDChecker.Version

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdIDecodeRegister_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdIDecodeRegister.Click

        Dim strIDecodeLicenseKey As String
        Dim intIDecodeRegisterStatus As Integer
        Dim strIDecodeVersion As String

        Dim objNewIDecodeLicenseInfo As New IDecodeLicenseInfo

        Try
            strIDecodeLicenseKey = Me.txtIDecodeLicenseKey.Text

            'THIS ONLY NEEDS TO BE DONE ONCE FOR AN APPLICATION

            intIDecodeRegisterStatus = Application.objMyIDChecker.RegisterIDecode(strIDecodeLicenseKey)

            Me.lblIDecodeRegistrationStatus.Content = IDecodeRegistrationResult(intIDecodeRegisterStatus)

            If intIDecodeRegisterStatus = 0 Then

                strIDecodeVersion = Application.objMyIDChecker.Version

                objNewIDecodeLicenseInfo.ClientID = WinMain.objClientSettings.ClientID
                objNewIDecodeLicenseInfo.IDecodeLicense = strIDecodeLicenseKey
                objNewIDecodeLicenseInfo.Version = strIDecodeVersion

                DataAccess.UpdateIDecodeLicense(objNewIDecodeLicenseInfo)

                IDecodeTab_Load()

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try
    End Sub

    Private Function IDecodeRegistrationResult(ByVal Returned As Integer) As String

        Dim strIDecodeRegisterStatus As String

        Select Case Returned

            Case 0
                strIDecodeRegisterStatus = "OK - Registration key accepted"
            Case 1
                strIDecodeRegisterStatus = "Error in PC field"
            Case 2
                strIDecodeRegisterStatus = "Error in PSN field"
            Case 3
                strIDecodeRegisterStatus = "Error in RANGE field"
            Case 4
                strIDecodeRegisterStatus = "Error in USN field"
            Case 5
                strIDecodeRegisterStatus = "Error in F1CRC field"
            Case 6
                strIDecodeRegisterStatus = "Error in F2CRC field"
            Case 9
                strIDecodeRegisterStatus = "Duplicate Registration Key"
            Case 10
                strIDecodeRegisterStatus = "Cannot renew this trial period"
            Case Else
                strIDecodeRegisterStatus = "Unknown error with registration key"
        End Select

        Return strIDecodeRegisterStatus

    End Function

    Private Sub txtIDecodeLicenseKey_TextChanged()
        Me.cmdIDecodeRegister.IsEnabled = True
        isContentChanged = True
        Me.cmdSave.IsEnabled = True
    End Sub

#End Region

#Region "UCC"

    Private Sub cboUCCReportSelector_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles cboUCCReportSelector.SelectionChanged

        Dim strDateOfExpiration As Date = Today

        Try

            Select Case cboUCCReportSelector.SelectedIndex
                Case 0
                    strDateOfExpiration = DateAdd(DateInterval.Day, 90, Today)

                Case 1
                    strDateOfExpiration = DateAdd(DateInterval.Day, 60, Today)

                Case 2
                    strDateOfExpiration = DateAdd(DateInterval.Day, 30, Today)

                Case 3
                    strDateOfExpiration = Today
            End Select

            Me.dgUCCReport.ItemsSource = DataAccess.GetUCCReport(strDateOfExpiration)

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdUCCReportPrint_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdUCCReportPrint.Click

        Dim strDateOfExpiration As Date
        Dim WinReportUCCExpired As WinReportUCCExpired

        Try

            Select Case cboUCCReportSelector.SelectedIndex
                Case 0
                    strDateOfExpiration = DateAdd(DateInterval.Day, 90, Today)

                Case 1
                    strDateOfExpiration = DateAdd(DateInterval.Day, 60, Today)

                Case 2
                    strDateOfExpiration = DateAdd(DateInterval.Day, 30, Today)

                Case 3
                    strDateOfExpiration = Today

            End Select

            WinReportUCCExpired = New WinReportUCCExpired(strDateOfExpiration)
            WinReportUCCExpired.Owner = Me

            WinReportUCCExpired.ShowDialog()

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub


#End Region

#Region "Delete"

    Private Sub DateBeginDate_SelectedDateChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles DateBeginDate.SelectedDateChanged

        If DateBeginDate.SelectedDate >= DateEndDate.SelectedDate Then
            MessageBox.Show("Start date must be before end date.")
            DateBeginDate.SelectedDate = Nothing
            Me.cmdDelete.IsEnabled = False
        Else
            Me.cmdDelete.IsEnabled = True
        End If

    End Sub

    Private Sub DateEndDate_SelectedDateChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles DateEndDate.SelectedDateChanged

        If DateBeginDate.SelectedDate >= DateEndDate.SelectedDate Then
            MessageBox.Show("End date must be after start date.")
            DateEndDate.SelectedDate = Nothing
            Me.cmdDelete.IsEnabled = False
        Else
            Me.cmdDelete.IsEnabled = True
        End If

    End Sub

    Private Sub cmdDelete_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdDelete.Click

        Dim objListOfRecordsToDelete As New List(Of Integer)
        Dim intListOfRecordsToDeleteCount As Integer
        Dim strMessage As String

        Try

            objListOfRecordsToDelete = DataAccess.GetRecordsToDelete(Me.DateBeginDate.SelectedDate, Me.DateEndDate.SelectedDate)
            intListOfRecordsToDeleteCount = objListOfRecordsToDelete.Count

            strMessage = "Are you sure you want to delete " & intListOfRecordsToDeleteCount & " records between " & Me.DateBeginDate.SelectedDate & " and " & Me.DateEndDate.SelectedDate & "?"

            If System.Windows.MessageBox.Show(strMessage, "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

                Me.cmdDelete.IsEnabled = False
                Me.lblDeleteStatus.Visibility = Windows.Visibility.Visible
                Me.ProgressBarDelete.Visibility = True
                Me.BWDeleteScans = New BackgroundWorker
                Me.BWDeleteScans.WorkerReportsProgress = True
                Me.BWDeleteScans.RunWorkerAsync(objListOfRecordsToDelete)

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWDeleteScans_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles BWDeleteScans.DoWork

        Dim bw As BackgroundWorker

        Try

            bw = CType(sender, BackgroundWorker)
            BWDeleteScans_ProgressChanged(0)
            e.Result = TimeConsumingOperation_DeleteScans(e.Argument, bw)

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWDeleteScans_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles BWDeleteScans.RunWorkerCompleted

        If (e.Error IsNot Nothing) Then
            WinMain.MyAppLog.WriteToLog("IVS", e.Error.ToString, EventLogEntryType.Error)

        Else

            Try
                Me.lblDeleteStatus.Content = "Delete action completed"
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow

                DateBeginDate.SelectedDate = Nothing
                DateEndDate.SelectedDate = Nothing

                Me.ProgressBarDelete.Visibility = Windows.Visibility.Hidden

            Catch ex As Exception
                WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
            End Try

        End If

    End Sub

    Private Function TimeConsumingOperation_DeleteScans(ByVal objListOfRecordsToDelete As List(Of Integer), ByVal bw As BackgroundWorker) As Boolean

        Dim intListOfRecordsToDeleteCount As Integer
        Dim intCurrentRecordCount As Integer = 0
        Dim intCurrentProgress As Integer

        Try
            intListOfRecordsToDeleteCount = objListOfRecordsToDelete.Count

            For Each intDeleteSwipeScanID As Integer In objListOfRecordsToDelete
                intCurrentRecordCount += 1

                intCurrentProgress = (intCurrentRecordCount / intListOfRecordsToDeleteCount) * 100
                BWDeleteScans_ProgressChanged(intCurrentProgress)

                CTSApi.DeleteImage(intDeleteSwipeScanID, WinMain.objClientSettings.ImageLocation)
                DataAccess.DeleteSwipeScan(intDeleteSwipeScanID)

            Next

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        Return True

    End Function

    Private Sub BWDeleteScans_ProgressChanged(ByVal PercentChange As Integer)

        Try
            ProgressBarDelete.Dispatcher.BeginInvoke(New StartTheInvoke_UpdateProgressBar(AddressOf NowStartTheInvoke_UpdateProgressBar), PercentChange)
            lblDeleteStatus.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), PercentChange.ToString & " % Completed")

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Delegate Sub StartTheInvoke_ReportStatus(ByVal Status As String)

    Friend Sub NowStartTheInvoke_ReportStatus(ByVal Status As String)

        Try
            Me.lblDeleteStatus.Content = Status

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Delegate Sub StartTheInvoke_UpdateProgressBar(ByVal Progress As Integer)

    Friend Sub NowStartTheInvoke_UpdateProgressBar(ByVal Progress As Integer)

        Try
            Me.ProgressBarDelete.Visibility = Windows.Visibility.Visible
            Me.ProgressBarDelete.Value = Progress

            If Progress = 0 Then

                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

End Class