Imports System.IO.Ports
Imports System.ComponentModel
Imports System.Text
'Imports System.Windows.Forms
Imports IVS.ESeek
Imports IVS.CTS
'Imports IVS.Data
'Imports IVS.AppLog
Imports IVS.Data.WS.TEP.IVSService
Imports IVS.Data.WS.TEP

Public Class WinAdmin

    'Private MyAppLog As New ApplicationLog(ApplicationLog.DateEncodingType.YYYYMMDD, True)
    'Private objClientSettings As ClientSettings
    'Private intIVSUserID As Integer
    Private WithEvents objESeekDevice As ESeekApi
    Private strDevicePort As String
    Private isContentChanged As Boolean = False
    Private WithEvents BWSerialPort As New BackgroundWorker
    Private intAlertID As Integer
    Private intUserID As Integer
    Private intLocationID As Integer
    Private strLocation As String

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        ' Dim intClientID As Integer

        Try
            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner

            'intClientID = DataAccess.GetClientID()
            'intIVSUserID = IVSUserID

            'WinMain.MyAppLog.WriteToLog("Client ID: " & intClientID)

            'objClientSettings = DataAccess.GetClientSettings(intClientID)

            ClearLabelsOnLoad()
            cboDevicePort_Load()
            cboClientDefaultUser_Load()
            cboDevices_Load()

            Me.dgAlerts.ItemsSource = DataAccess.GetAlerts
            Me.dgUsers.ItemsSource = DataAccess.GetUsers
            Me.dgLocations.ItemsSource = DataAccess.GetLocations
            Me.cboClientLocation.ItemsSource = DataAccess.GetLocations
            Me.cboClientLocation.DisplayMemberPath = ("LocationDescription")
            Me.cboClientLocation.SelectedValuePath = ("LocationID")

            Me.lblEdition.Content = My.Application.Info.Description
            Me.lblVersion.Content = My.Application.Info.ProductName
            Me.lblBuild.Content = My.Application.Info.Version.ToString

        Catch ex As Exception
            'DataAccess.NewException(ex)
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
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub WinAdmin_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        Try
            Me.cboClientLocation.SelectedValue = WinMain.objClientSettings.Location
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

                Case "M200/250"

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

                Case "M210/260"

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

                Case "MAGTEK"

                    Me.cboDevices.SelectedIndex = 4
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

            AddHandler cboClientLocation.SelectionChanged, AddressOf TextChanged
            AddHandler txtClientStation.TextChanged, AddressOf TextChanged
            AddHandler txtClientPhone.TextChanged, AddressOf TextChanged
            AddHandler txtClientEmail.TextChanged, AddressOf TextChanged
            AddHandler cboClientDefaultUser.SelectionChanged, AddressOf TextChanged
            AddHandler cbClientSkipLogon.Click, AddressOf TextChanged
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
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdSave_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdSave.Click

        Dim objNewClientSettings As New ClientSettings
        Dim objDefaultUser_SelectedRow As UserDetail
        Dim objClientLocation_SelectedRow As Locations

        Try
            objNewClientSettings.ClientID = WinMain.objClientSettings.ClientID

            objClientLocation_SelectedRow = Me.cboClientLocation.SelectedItem
            objNewClientSettings.Location = objClientLocation_SelectedRow.LocationID

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
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdCancel_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCancel.Click

        Try
            If isContentChanged = True Then

                If MessageBox.Show("Exit without saving changes?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

                    Me.DialogResult = True
                    Me.Close()

                End If
            Else
                Me.DialogResult = True
                Me.Close()
            End If

        Catch ex As Exception
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cboClientLocation_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles cboClientLocation.SelectionChanged

        Try
            Me.cmdSave.IsEnabled = True
            Me.dgLocations.ItemsSource = DataAccess.GetLocations

        Catch ex As Exception
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try
    End Sub

    Private Sub dgLocations_SelectionChanged(sender As Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgLocations.SelectionChanged

        Dim dgLocations_SelectedRow As System.Collections.IList

        Try
            dgLocations_SelectedRow = e.AddedItems

            If dgLocations_SelectedRow.Count > 0 Then

                intLocationID = dgLocations_SelectedRow(0).LocationID
                strLocation = dgLocations_SelectedRow(0).LocationDescription

            End If

        Catch ex As Exception
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgLocations_CmdEdit_Click()

        Dim sb As New StringBuilder

        Try
            Me.txtLocation.Text = strLocation
            Me.cmdNewLocation.Visibility = Windows.Visibility.Hidden
            Me.cmdEditLocation.Visibility = Windows.Visibility.Visible

        Catch ex As Exception
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        Me.dgLocations.ItemsSource = DataAccess.GetLocations

    End Sub

    Private Sub dgLocations_CmdDelete_Click()

        Dim sb As New StringBuilder

        Try
            If System.Windows.MessageBox.Show("Are you sure you want to delete this record?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

                DataAccess.DeleteLocation(intLocationID)

            End If

            Me.dgLocations.ItemsSource = DataAccess.GetLocations

        Catch ex As Exception
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdNewLocation_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdNewLocation.Click

        Try
            DataAccess.NewLocation(Me.txtLocation.Text)
            Me.txtLocation.Text = Nothing

            Me.dgLocations.ItemsSource = DataAccess.GetLocations
            Me.cboClientLocation.ItemsSource = DataAccess.GetLocations
            Me.cboClientLocation.DisplayMemberPath = ("LocationDescription")
            Me.cboClientLocation.SelectedValuePath = ("LocationID")
            Me.cboClientLocation.SelectedValue = WinMain.objClientSettings.Location

        Catch ex As Exception
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdEditLocation_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdEditLocation.Click

        Dim objLocation As New Locations

        Try
            objLocation.LocationID = intLocationID
            objLocation.LocationDescription = Me.txtLocation.Text

            DataAccess.UpdateLocation(objLocation)
            Me.txtLocation.Text = Nothing

            Me.cmdNewLocation.Visibility = Windows.Visibility.Visible
            Me.cmdEditLocation.Visibility = Windows.Visibility.Hidden

            Me.dgLocations.ItemsSource = DataAccess.GetLocations
            Me.cboClientLocation.ItemsSource = DataAccess.GetLocations
            Me.cboClientLocation.DisplayMemberPath = ("LocationDescription")
            Me.cboClientLocation.SelectedValuePath = ("LocationID")
            Me.cboClientLocation.SelectedValue = WinMain.objClientSettings.Location

        Catch ex As Exception
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub txtLocation_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs) Handles txtLocation.TextChanged

        Try

            If Me.txtLocation.Text <> "" Then

                Me.cmdEditLocation.IsEnabled = True
                Me.cmdNewLocation.IsEnabled = True
            Else

                Me.cmdEditLocation.IsEnabled = False
                Me.cmdNewLocation.IsEnabled = False
            End If

        Catch ex As Exception
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub txtImageLocation_GotFocus(sender As Object, e As System.Windows.RoutedEventArgs) Handles txtImageLocation.GotFocus

        Dim MyFolderBrowserDialog As New System.Windows.Forms.FolderBrowserDialog
        Dim strImageLocation As String = Me.txtImageLocation.Text

        Try
            Dim result As Forms.DialogResult = MyFolderBrowserDialog.ShowDialog()

            If (result = Forms.DialogResult.OK) Then

                strImageLocation = MyFolderBrowserDialog.SelectedPath

                Me.txtImageLocation.Text = MyFolderBrowserDialog.SelectedPath

            End If
        Catch ex As Exception
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try
    End Sub

    Private Sub cmdBrowseSourceFolder_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdBrowseSourceFolder.Click
        Dim MyFolderBrowserDialog As New System.Windows.Forms.FolderBrowserDialog
        Dim strImageLocation As String = Me.txtImageLocation.Text

        Try
            Dim result As Forms.DialogResult = MyFolderBrowserDialog.ShowDialog()

            If (result = Forms.DialogResult.OK) Then

                strImageLocation = MyFolderBrowserDialog.SelectedPath

                Me.txtImageLocation.Text = MyFolderBrowserDialog.SelectedPath

            End If
        Catch ex As Exception
            DataAccess.NewException(ex)
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
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cboClientDefaultUser_Load()

        Try

            Me.cboClientDefaultUser.ItemsSource = DataAccess.GetUserNames
            Me.cboClientDefaultUser.DisplayMemberPath = ("UserName")
            Me.cboClientDefaultUser.SelectedValuePath = ("UserID")
            Me.cboClientDefaultUser.SelectedValue = WinMain.objClientSettings.DefaultUser

            Me.cmdSave.IsEnabled = False

        Catch ex As Exception
            DataAccess.NewException(ex)
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
            DataAccess.NewException(ex)
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
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgUsers_CmdEnable_Click()

        Try
            DataAccess.EnableUser(intUserID, True)
            Me.dgUsers.ItemsSource = DataAccess.GetUsers
            cboClientDefaultUser_Load()
        Catch ex As Exception
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgUsers_CmdDisable_Click()

        Try
            DataAccess.EnableUser(intUserID, False)
            Me.dgUsers.ItemsSource = DataAccess.GetUsers
            cboClientDefaultUser_Load()
        Catch ex As Exception
            DataAccess.NewException(ex)
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
            DataAccess.NewException(ex)
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
            DataAccess.NewException(ex)
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
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgAlerts_CmdEnable_Click()

        Try
            DataAccess.EnableAlert(intAlertID, True)
            Me.dgAlerts.ItemsSource = DataAccess.GetAlerts

        Catch ex As Exception
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgAlerts_CmdDisable_Click()

        Try
            DataAccess.EnableAlert(intAlertID, False)
            Me.dgAlerts.ItemsSource = DataAccess.GetAlerts

        Catch ex As Exception
            DataAccess.NewException(ex)
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
            objAlertDetail.AlertContactNumber = WinMain.objUserDetail.UserPhone
            objAlertDetail.UserID = WinMain.intIVSUserID

            WinAlert = New WinAlert(objAlertDetail, WinMain.intIVSUserID)
            WinAlert.Owner = Me

            DialogResult = WinAlert.ShowDialog

            If DialogResult = True Then
                Me.dgAlerts.ItemsSource = DataAccess.GetAlerts
            End If

        Catch ex As Exception
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

#Region "Devices"

    Private Sub cboDevices_Load()

        Try
            Me.cboDevices.Items.Add("CTS LS40 USB")
            Me.cboDevices.Items.Add("CTS LS150 USB")
            Me.cboDevices.Items.Add("ESEEK M250")
            Me.cboDevices.Items.Add("ESEEK M280")
            Me.cboDevices.Items.Add("MagTek")
            Me.cboDevices.Items.Add("Integrated Text Input")
            Me.cboDevices.Items.Add("No Reader")
            Me.cboDevices.SelectedIndex = 6

        Catch ex As Exception
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cboDevicePort_Load()

        Dim ActiveComPorts As String()
        Dim intActiveComPorts As Integer

        Try
            ActiveComPorts = SerialPort.GetPortNames
            intActiveComPorts = ActiveComPorts.Count

            'WinMain.MyAppLog.WriteToLog(intActiveComPorts & " active serial port(s) were found:")

            If intActiveComPorts > 0 Then

                For Each strActivePort As String In ActiveComPorts

                    Me.cboDevicePort.Items.Add(strActivePort)
                    'WinMain.MyAppLog.WriteToLog("Active serial port: " & strActivePort)

                Next

            End If

            Me.cboDevicePort.Items.Add("USB")
            Me.cboDevicePort.SelectedIndex = 0

        Catch ex As Exception
            DataAccess.NewException(ex)
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

            End Select

        Catch ex As Exception
            DataAccess.NewException(ex)
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
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Delegate Sub StartTheInvoke(ByVal DataReceived As String)

    Friend Sub NowStartTheInvoke(ByVal DataReceived As String)

        Try
            Me.BWSerialPort.RunWorkerAsync(DataReceived)

        Catch ex As Exception
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWSerialPort_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles BWSerialPort.DoWork

        Dim bw As BackgroundWorker

        Try

            bw = CType(sender, BackgroundWorker)
            e.Result = TimeConsumingOperation_ESeekIdentify(e.Argument, bw)

        Catch ex As Exception
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWSerialPort_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles BWSerialPort.RunWorkerCompleted

        If (e.Error IsNot Nothing) Then

            'WinMain.MyAppLog.WriteToLog("winAdmin.BWSerialPort_RunWorkerCompleted()" & e.Error.ToString)

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

                    Me.cmdSave.IsEnabled = False

                End If

            Catch ex As Exception
                DataAccess.NewException(ex)
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
            DataAccess.NewException(ex)
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
            DataAccess.NewException(ex)
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
            DataAccess.NewException(ex)
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
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

End Class