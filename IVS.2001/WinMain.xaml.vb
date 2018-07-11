Imports System.Threading
Imports IVS.Data
Imports IVS.AppLog
Imports IVS.Data.IVSService

Class WinMain

    Private MyAppLog As New ApplicationLog(ApplicationLog.DateEncodingType.YYYYMMDD, True)
    Private objClientSettings As New ClientSettings
    Private intIVSUserID As Integer
    Private strIVSUserName As String
    Public Shared isUserAdmin As Boolean
    Public Shared isUserSearchAble As Boolean
    Public Shared isUserAlertAble As Boolean

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterScreen

    End Sub

#Region "Control Bound Subs"

    Private Sub WinMain_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        Try
            ClientSettingsLoad()

        Catch ex As Exception
            MyAppLog.WriteToLog("WinMain.WinMain_Loaded()" & ex.ToString)
        End Try

    End Sub

    Private Sub cmdAdmin_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdAdmin.Click

        Dim winAdministration As WinAdmin
        Dim dialogResult As Boolean

        Try
            winAdministration = New WinAdmin(intIVSUserID)

            winAdministration.Owner = Me
            dialogResult = winAdministration.ShowDialog

            If dialogResult = True Then
                ClientSettingsLoad()
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdSearch_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdSearch.Click

        Dim WinSearchScans As WinSearch

        Try
            WinSearchScans = New WinSearch(intIVSUserID, objClientSettings.DeviceType, objClientSettings.ImageLocation, objClientSettings.ViewingTime)
            WinSearchScans.Owner = Me
            WinSearchScans.ShowDialog()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdScan_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdScan.Click

        Try
            If objClientSettings.DeviceType = "LS_40_USB" Or objClientSettings.DeviceType = "LS_150_USB" Then

                Dim winCTSScan As WinCTS
                winCTSScan = New WinCTS(intIVSUserID)
                winCTSScan.Owner = Me
                winCTSScan.ShowDialog()

            ElseIf objClientSettings.DeviceType = "ESEEK" Then

                Dim winESeekScan As WinESeek
                winESeekScan = New WinESeek(intIVSUserID)
                winESeekScan.Owner = Me
                winESeekScan.ShowDialog()

            ElseIf objClientSettings.DeviceType = "MAGTEK" Then

                Dim winMagTekScan As WinMagTek
                winMagTekScan = New WinMagTek(intIVSUserID)
                winMagTekScan.Owner = Me
                winMagTekScan.ShowDialog()

            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdLogon_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdLogon.Click

        Dim strUserName As String
        Dim strPassword As String
        Dim objUserDetail As New UserDetail

        Try
            strUserName = Me.txtUserName.Text
            strPassword = Me.txtPassword.Password

            intIVSUserID = DataAccess.IsUserAuthenticated(strUserName, strPassword)

            If intIVSUserID > 0 Then

                Me.lblStatus.Content = "Successfull logon"

                If objClientSettings.DeviceType = "No Reader" Then
                    Me.cmdScan.IsEnabled = False
                Else
                    Me.cmdScan.IsEnabled = True
                End If

                Me.cmdLogon.IsEnabled = False
                Me.txtUserName.Text = Nothing
                Me.txtPassword.Password = Nothing

                Me.recLogon.Visibility = Windows.Visibility.Hidden
                Me.lblUserName.Visibility = Windows.Visibility.Hidden
                Me.txtUserName.Visibility = Windows.Visibility.Hidden
                Me.lblPassword.Visibility = Windows.Visibility.Hidden
                Me.txtPassword.Visibility = Windows.Visibility.Hidden
                Me.cmdLogon.Visibility = Windows.Visibility.Hidden

                objUserDetail = DataAccess.GetUserDetail(intIVSUserID)

                isUserAdmin = objUserDetail.AdminFlag
                isUserAlertAble = objUserDetail.AlertFlag
                isUserSearchAble = objUserDetail.SearchFlag

                MyAppLog.WriteToLog("Logged on User: " & intIVSUserID)
                MyAppLog.WriteToLog("User.isUserAdmin: " & isUserAdmin)
                MyAppLog.WriteToLog("User.isUserAlertAble: " & isUserAlertAble)
                MyAppLog.WriteToLog("User.isUserSearchAble: " & isUserSearchAble)

                If isUserAdmin = True Then
                    Me.cmdAdmin.IsEnabled = True
                Else
                    Me.cmdAdmin.IsEnabled = False
                End If

                If isUserSearchAble = True Then
                    Me.cmdSearch.IsEnabled = True
                Else
                    Me.cmdSearch.IsEnabled = False
                End If

                If intIVSUserID < 3 Then
                    Me.cmdEditUserAccount.Visibility = Windows.Visibility.Hidden
                Else
                    Me.cmdEditUserAccount.Visibility = Windows.Visibility.Visible
                End If

            Else
                Me.lblStatus.Content = "Username or password incorrect."
                Me.txtPassword.Password = Nothing
            End If

            Me.lblStatus.Visibility = Windows.Visibility.Visible

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdEditUserAccount_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdEditUserAccount.Click

        Dim WinUser As WinUser
        Dim DialogResult As Boolean

        Try

            WinUser = New WinUser(intIVSUserID, True)
            WinUser.Owner = Me

            DialogResult = WinUser.ShowDialog

            If DialogResult = True Then
                If MessageBox.Show("User account has been updated.  Log onto IVS with the updated user account.", "IVS - User Account Update", MessageBoxButton.OK, MessageBoxImage.Information) = MessageBoxResult.OK Then

                    Me.Close()

                End If
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdCancel_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCancel.Click

        Try
            Application.Current.Shutdown()
            Me.Close()
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

#Region "Custom Subs/Functions"

    Private Sub ClientSettingsLoad()

        Dim intClientID As Integer
        Dim objUserDetail As New UserDetail

        Try
            intClientID = DataAccess.GetClientID

            If intClientID > 0 Then

                Me.lblStatus.Visibility = Windows.Visibility.Hidden
                DataAccess.UpdateClientIPAddress(intClientID)

            Else

                intClientID = DataAccess.NewClient

                Me.lblStatus.Content = "IVS client options need to be entered"
                Me.lblStatus.Visibility = Windows.Visibility.Visible
                Me.cmdSearch.IsEnabled = False
                Me.cmdScan.IsEnabled = False

            End If

            MyAppLog.WriteToLog("Client ID: " & intClientID)

            objClientSettings = DataAccess.GetClientSettings(intClientID)

            MyAppLog.WriteToLog("Client Device Type: " & objClientSettings.DeviceType)
            MyAppLog.WriteToLog("Client DeviceID: " & objClientSettings.DeviceID)
            MyAppLog.WriteToLog("Client ComPort: " & objClientSettings.ComPort)
            MyAppLog.WriteToLog("Client Location: " & objClientSettings.Location)
            MyAppLog.WriteToLog("Client Station: " & objClientSettings.Station)
            MyAppLog.WriteToLog("Client Skip Logon: " & objClientSettings.SkipLogon)
            MyAppLog.WriteToLog("Client Default User: " & objClientSettings.DefaultUser)
            MyAppLog.WriteToLog("Client Save Images: " & objClientSettings.ImageSave)
            MyAppLog.WriteToLog("Client Image Location: " & objClientSettings.ImageLocation)
            MyAppLog.WriteToLog("Client Age HighLight: " & objClientSettings.AgeHighlight)
            MyAppLog.WriteToLog("Client Age PopUp: " & objClientSettings.AgePopup)
            MyAppLog.WriteToLog("Client Age: " & objClientSettings.Age)
            MyAppLog.WriteToLog("Client Viewing Time: " & objClientSettings.ViewingTime)
            MyAppLog.WriteToLog("Client CC Digits: " & objClientSettings.CCDigits)
            MyAppLog.WriteToLog("Client CC Save Disabled: " & objClientSettings.DisableCCSave)
            MyAppLog.WriteToLog("Client DB Save Disabled: " & objClientSettings.DisableDBSave)
            MyAppLog.WriteToLog("Client Log Retention: " & objClientSettings.LogRetention)

            If objClientSettings.SkipLogon = True Then

                intIVSUserID = objClientSettings.DefaultUser
                Me.cmdEditUserAccount.Visibility = Windows.Visibility.Hidden

                objUserDetail = DataAccess.GetUserDetail(intIVSUserID)

                isUserAdmin = objUserDetail.AdminFlag
                isUserSearchAble = objUserDetail.SearchFlag
                isUserAlertAble = objUserDetail.AlertFlag
                strIVSUserName = objUserDetail.UserNameFirst & " " & objUserDetail.UserNameLast

                MyAppLog.WriteToLog("User.isUserAdmin: " & isUserAdmin)
                MyAppLog.WriteToLog("User.isUserSearchAble: " & isUserSearchAble)
                MyAppLog.WriteToLog("User.isUserAlertAble: " & isUserAlertAble)

                If isUserAdmin = True Then
                    cmdAdmin.IsEnabled = True

                    Me.recLogon.Visibility = Windows.Visibility.Hidden
                    Me.lblUserName.Visibility = Windows.Visibility.Hidden
                    Me.txtUserName.Visibility = Windows.Visibility.Hidden
                    Me.lblPassword.Visibility = Windows.Visibility.Hidden
                    Me.txtPassword.Visibility = Windows.Visibility.Hidden
                    Me.cmdLogon.Visibility = Windows.Visibility.Hidden

                Else
                    Me.cmdAdmin.IsEnabled = False

                    Me.recLogon.Visibility = Windows.Visibility.Visible
                    Me.lblUserName.Visibility = Windows.Visibility.Visible
                    Me.txtUserName.Visibility = Windows.Visibility.Visible
                    Me.lblPassword.Visibility = Windows.Visibility.Visible
                    Me.txtPassword.Visibility = Windows.Visibility.Visible
                    Me.cmdLogon.Visibility = Windows.Visibility.Visible
                    Me.cmdLogon.IsEnabled = True

                End If

                If objClientSettings.DeviceType = "No Reader" Then
                    Me.cmdScan.IsEnabled = False
                Else
                    Me.cmdScan.IsEnabled = True
                End If

                If isUserSearchAble = True Then
                    Me.cmdSearch.IsEnabled = True
                Else
                    Me.cmdSearch.IsEnabled = False
                End If

            Else

                Me.cmdAdmin.IsEnabled = False
                Me.cmdSearch.IsEnabled = False
                Me.cmdScan.IsEnabled = False

                Me.recLogon.Visibility = Windows.Visibility.Visible
                Me.lblUserName.Visibility = Windows.Visibility.Visible
                Me.txtUserName.Visibility = Windows.Visibility.Visible
                Me.lblPassword.Visibility = Windows.Visibility.Visible
                Me.txtPassword.Visibility = Windows.Visibility.Visible
                Me.cmdLogon.Visibility = Windows.Visibility.Visible
                Me.cmdEditUserAccount.Visibility = Windows.Visibility.Hidden

            End If

            'ThreadPool.QueueUserWorkItem(AddressOf MyAppLog.DeleteExpiredLogFiles, objClientSettings.LogRetention)

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

End Class

