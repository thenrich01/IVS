Imports System.IO.Ports
Imports System.ComponentModel
Imports System.Text
Imports System.Windows.Forms
Imports IVS.Eseek
Imports IVS.CTS
Imports IVS.Data
Imports IVS.Honeywell

Public Class WinAdmin

    Private WithEvents objESeekDevice As ESeekApi
    Private WithEvents objHoneywellDevice As HoneywellApi
    Private strDevicePort As String
    Private isContentChanged As Boolean = False
    Private WithEvents BWSerialPort As New BackgroundWorker
    Private intAlertID As Integer
    Private intUserID As Integer
    Private intVisitingID As Integer
    Private intLocationID As Integer
    Private strLocation As String

    Public Sub New()

        ' This call is required by the designer.
        Try
            InitializeComponent()
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        ' Add any initialization after the InitializeComponent() call.

        Try
            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner

            ClearLabelsOnLoad()
            IDecodeTab_Load()
            cboDevicePort_Load()
            cboClientDefaultUser_Load()
            cboDevices_Load()
            DymoPrinters_Load()

            Me.dgAlerts.ItemsSource = DataAccess.GetAlerts
            Me.dgUsers.ItemsSource = DataAccess.GetUsers
            Me.dgVisiting.ItemsSource = DataAccess.GetVisiting
            Me.dgLocations.ItemsSource = DataAccess.GetLocations

            Me.cboClientLocation.ItemsSource = DataAccess.GetLocations
            Me.cboClientLocation.DisplayMemberPath = ("LocationDescription")
            Me.cboClientLocation.SelectedValuePath = ("LocationID")

            Me.cboClientMirror.ItemsSource = DataAccess.GetOtherStations(WinMain.objClientSettings.ClientID)
            Me.cboClientMirror.DisplayMemberPath = ("Station")
            Me.cboClientMirror.SelectedValuePath = ("ClientID")

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
            Me.cboClientLocation.SelectedValue = WinMain.objClientSettings.Location
            Me.txtClientStation.Text = WinMain.objClientSettings.Station
            Me.txtClientPhone.Text = WinMain.objClientSettings.Phone
            Me.txtClientEmail.Text = WinMain.objClientSettings.Email
            Me.cbClientSkipLogon.IsChecked = WinMain.objClientSettings.SkipLogon
            Me.txtIDecodeLicenseKey.Text = WinMain.objClientSettings.IDecodeLicense
            Me.cbAgePopup.IsChecked = WinMain.objClientSettings.AgePopup
            Me.txtAge.Text = WinMain.objClientSettings.Age
            Me.cbImageSave.IsChecked = WinMain.objClientSettings.ImageSave
            Me.txtImageLocation.Text = WinMain.objClientSettings.ImageLocation

            Me.txtDymoLabel.Text = WinMain.objClientSettings.DymoLabel
            Me.cboDymoPrinters.SelectedItem = WinMain.objClientSettings.DymoPrinter

            If WinMain.objClientSettings.MirroredClient > 0 Then

                Me.lblMirrorClient.Visibility = Windows.Visibility.Visible
                Me.cboClientMirror.Visibility = Windows.Visibility.Hidden

                Me.lblMirrorClient.Content = "Mirrored by " & DataAccess.GetStationName(WinMain.objClientSettings.MirroredClient)

                Me.cbClientMirror.IsChecked = False
                Me.cbClientMirror.IsEnabled = False
                Me.cboClientMirror.IsEnabled = False
                Me.cboClientMirror.SelectedIndex = -1

            Else

                Me.lblMirrorClient.Visibility = Windows.Visibility.Hidden
                Me.cboClientMirror.Visibility = Windows.Visibility.Visible

                If WinMain.objClientSettings.MirrorClientID > 0 Then

                    Me.cbClientMirror.IsChecked = True
                    Me.cboClientMirror.IsEnabled = True
                    Me.cboClientMirror.SelectedValue = WinMain.objClientSettings.MirrorClientID
                Else
                    Me.cbClientMirror.IsChecked = False
                    Me.cboClientMirror.IsEnabled = False
                    Me.cboClientMirror.SelectedIndex = -1
                End If

            End If

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

            AddHandler cboClientLocation.SelectionChanged, AddressOf TextChanged
            AddHandler txtClientStation.TextChanged, AddressOf TextChanged
            AddHandler txtClientPhone.TextChanged, AddressOf TextChanged
            AddHandler txtClientEmail.TextChanged, AddressOf TextChanged
            AddHandler cbClientSkipLogon.Click, AddressOf TextChanged
            AddHandler cbClientSkipMain.Click, AddressOf TextChanged
            AddHandler cboClientDefaultUser.SelectionChanged, AddressOf TextChanged
            AddHandler cbAgePopup.Click, AddressOf TextChanged
            AddHandler txtAge.TextChanged, AddressOf TextChanged
            AddHandler cbImageSave.Click, AddressOf TextChanged
            AddHandler txtImageLocation.TextChanged, AddressOf TextChanged
            AddHandler txtIDecodeLicenseKey.TextChanged, AddressOf txtIDecodeLicenseKey_TextChanged
            AddHandler cboDymoPrinters.SelectionChanged, AddressOf TextChanged
            AddHandler txtDymoLabel.TextChanged, AddressOf TextChanged

            Me.cmdSave.IsEnabled = False

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdSave_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdSave.Click

        Dim objNewClientSettings As New ClientSettings
        Dim objDefaultUser_SelectedRow As UserDetail
        Dim objClientLocation_SelectedRow As Locations
        Dim objClientMirror_SelectedRow As Clients

        Try
            objNewClientSettings.ClientID = WinMain.objClientSettings.ClientID

            objClientLocation_SelectedRow = Me.cboClientLocation.SelectedItem
            objNewClientSettings.Location = objClientLocation_SelectedRow.LocationID

            objNewClientSettings.Station = Me.txtClientStation.Text
            objNewClientSettings.Phone = Me.txtClientPhone.Text
            objNewClientSettings.Email = Me.txtClientEmail.Text
            objNewClientSettings.SkipLogon = Me.cbClientSkipLogon.IsChecked

            objDefaultUser_SelectedRow = Me.cboClientDefaultUser.SelectedItem
            objNewClientSettings.DefaultUser = objDefaultUser_SelectedRow.UserID

            objNewClientSettings.AgePopup = Me.cbAgePopup.IsChecked
            objNewClientSettings.Age = Me.txtAge.Text
            objNewClientSettings.ImageSave = Me.cbImageSave.IsChecked
            objNewClientSettings.ImageLocation = Me.txtImageLocation.Text
            objNewClientSettings.DymoPrinter = Me.cboDymoPrinters.SelectedItem
            objNewClientSettings.DymoLabel = Me.txtDymoLabel.Text

            If Me.cbClientMirror.IsChecked = True Then
                objClientMirror_SelectedRow = Me.cboClientMirror.SelectedItem
                objNewClientSettings.MirrorClientID = objClientMirror_SelectedRow.ClientID
            Else
                objNewClientSettings.MirrorClientID = 0
            End If

            DataAccess.SaveClientSettings(objNewClientSettings)

            isContentChanged = False
            Me.cmdSave.IsEnabled = False

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cboClientLocation_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles cboClientLocation.SelectionChanged

        Try
            Me.cmdSave.IsEnabled = True

        Catch ex As Exception
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
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgLocations_CmdEdit_Click()

        Try
            Me.txtLocation.Text = strLocation
            Me.cmdNewLocation.Visibility = Windows.Visibility.Hidden
            Me.cmdEditLocation.Visibility = Windows.Visibility.Visible

        Catch ex As Exception
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
            Me.cboClientLocation.ItemsSource = DataAccess.GetLocations
            Me.cboClientLocation.DisplayMemberPath = ("LocationDescription")
            Me.cboClientLocation.SelectedValuePath = ("LocationID")
            Me.cboClientLocation.SelectedValue = WinMain.objClientSettings.Location
        Catch ex As Exception
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
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub txtLocation_TextChanged(sender As Object, e As System.Windows.Controls.TextChangedEventArgs) Handles txtLocation.TextChanged

        Try

            If Me.txtLocation.Text <> "" Then

                Me.cmdEditLocation.IsEnabled = True
                Me.cmdNewLocation.IsEnabled = True
            Else

                Me.cmdEditLocation.IsEnabled = False
                Me.cmdNewLocation.IsEnabled = False
            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdCancel_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCancel.Click

        Try
            If isContentChanged = True Then

                If System.Windows.MessageBox.Show("Exit without saving changes?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

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

    Private Sub dgVisiting_SelectionChanged(sender As Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgVisiting.SelectionChanged

        Dim dgVisiting_SelectedRow As System.Collections.IList

        Try
            dgVisiting_SelectedRow = e.AddedItems

            If dgVisiting_SelectedRow.Count > 0 Then

                intVisitingID = dgVisiting_SelectedRow(0).VisitingID

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgVisiting_CmdDelete_Click()

        Dim sb As New StringBuilder

        Try
            If System.Windows.MessageBox.Show("Are you sure you want to delete this record?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then
                DataAccess.DeleteVisiting(intVisitingID)
            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        Me.dgVisiting.ItemsSource = DataAccess.GetVisiting

    End Sub

    Private Sub cmdNewVisiting_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdNewVisiting.Click

        Dim WinVisiting As winVisiting
        Dim DialogResult As Boolean
        Dim objVisitingDetail As New Visiting

        Try
            objVisitingDetail.VisitingID = 0

            WinVisiting = New winVisiting(objVisitingDetail)
            WinVisiting.Owner = Me

            DialogResult = WinVisiting.ShowDialog

            If DialogResult = True Then
                Me.dgVisiting.ItemsSource = DataAccess.GetVisiting
            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgVisiting_CmdView_Click()

        Dim WinVisiting As winVisiting
        Dim DialogResult As Boolean
        Dim objVisitingDetail As New Visiting

        Try
            objVisitingDetail = DataAccess.GetVisitingDetail(intVisitingID)

            WinVisiting = New winVisiting(objVisitingDetail)
            WinVisiting.Owner = Me

            DialogResult = WinVisiting.ShowDialog

            If DialogResult = True Then
                Me.dgVisiting.ItemsSource = DataAccess.GetVisiting
            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgVisiting_CmdDisable_Click()

        Try
            DataAccess.EnableVisiting(intVisitingID, False)
            Me.dgVisiting.ItemsSource = DataAccess.GetVisiting

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgVisiting_CmdEnable_Click()

        Try
            DataAccess.EnableVisiting(intVisitingID, True)
            Me.dgVisiting.ItemsSource = DataAccess.GetVisiting

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cbClientMirror_Checked(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cbClientMirror.Checked

        Try
            isContentChanged = True
            Me.cmdSave.IsEnabled = True
            Me.cboClientMirror.IsEnabled = True
            If WinMain.objClientSettings.MirrorClientID > 0 Then
                Me.cboClientMirror.SelectedValue = WinMain.objClientSettings.MirrorClientID
            End If
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cbClientMirror_Unchecked(sender As Object, e As System.Windows.RoutedEventArgs) Handles cbClientMirror.Unchecked

        Try
            isContentChanged = True
            Me.cmdSave.IsEnabled = True
            Me.cboClientMirror.IsEnabled = False
            Me.cboClientMirror.SelectedIndex = -1
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cboClientMirror_SelectionChanged(sender As Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles cboClientMirror.SelectionChanged

        Try
            Me.cmdSave.IsEnabled = True
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try
    End Sub

#End Region

#Region "Custom Subs/Functions"

    Private Sub ClearLabelsOnLoad()

        Try

            Me.lblIDecodeLicenseStatus.Content = Nothing
            Me.lblIDecodeRegistrationStatus.Content = Nothing
            Me.lblIDecodeVersion.Content = Nothing
            Me.lblIDecodeTrialDaysRemaining.Content = Nothing

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

            Me.cboClientDefaultUser.ItemsSource = DataAccess.GetUserNames(WinMain.objClientSettings.Location, WinMain.objClientSettings.ClientID)
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
            objAlertDetail.AlertContactName = DataAccess.GetUserName(WinMain.intIVSUserId)
            objAlertDetail.AlertContactNumber = WinMain.objUserDetail.UserPhone
            objAlertDetail.UserID = WinMain.intIVSUserId

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

#Region "IDecode"

    Private Sub IDecodeTab_Load()

        Dim intIDecodeLicenseStatus As Integer

        Try

            intIDecodeLicenseStatus = Application.objMyIDChecker.LicenseStatus

            Me.cmdIDecodeRegister.IsEnabled = False

            Select Case intIDecodeLicenseStatus

                Case 0
                    Me.lblIDecodeLicenseStatus.Content = "License status OK"

                Case 1
                    Me.lblIDecodeLicenseStatus.Content = "Trial version within evaluation period"
                    Me.labelIDecodeTrialDaysRemaining.Visibility = Windows.Visibility.Visible
                    Me.lblIDecodeTrialDaysRemaining.Visibility = Windows.Visibility.Visible
                    Me.lblIDecodeTrialDaysRemaining.Content = Application.objMyIDChecker.TrialDaysRemaining

                Case 2
                    Me.lblIDecodeLicenseStatus.Content = "Trial version beyond evaluation"
                    Me.cmdIDecodeRegister.IsEnabled = True
                Case 3
                    Me.lblIDecodeLicenseStatus.Content = "Unregistered copy"
                    Me.cmdIDecodeRegister.IsEnabled = True

                Case Else
                    Me.lblIDecodeLicenseStatus.Content = "Error retrieving license status"
                    Me.cmdIDecodeRegister.IsEnabled = True
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
        Me.cmdSave.IsEnabled = True
        TextChanged()
    End Sub

#End Region

#Region "Labels"

    Private Sub DymoPrinters_Load()

        Try

            For Each printer In DYMO.Label.Framework.Framework.GetPrinters

                cboDymoPrinters.Items.Add(printer.Name)

            Next

            If cboDymoPrinters.Items.Count > 0 Then

                If WinMain.objClientSettings.DymoPrinter IsNot Nothing Then

                    cboDymoPrinters.SelectedItem = WinMain.objClientSettings.DymoPrinter

                End If

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub txtDymoLabel_GotFocus(sender As Object, e As System.Windows.RoutedEventArgs) Handles txtDymoLabel.GotFocus

        Dim MyFileBrowserDialog As New OpenFileDialog
        Dim strDymoLabel As String = Me.txtDymoLabel.Text

        Try
            MyFileBrowserDialog.Multiselect = False

            TextChanged()

            Dim result As DialogResult = MyFileBrowserDialog.ShowDialog()

            If (result = Forms.DialogResult.OK) Then

                strDymoLabel = MyFileBrowserDialog.FileName

                Me.txtDymoLabel.Text = MyFileBrowserDialog.FileName

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdDymoLabel_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdDymoLabel.Click

        Dim MyFileBrowserDialog As New OpenFileDialog
        Dim strDymoLabel As String = Me.txtDymoLabel.Text

        Try
            MyFileBrowserDialog.Multiselect = False

            TextChanged()

            Dim result As DialogResult = MyFileBrowserDialog.ShowDialog()

            If (result = Forms.DialogResult.OK) Then

                strDymoLabel = MyFileBrowserDialog.FileName

                Me.txtDymoLabel.Text = MyFileBrowserDialog.FileName

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

End Class