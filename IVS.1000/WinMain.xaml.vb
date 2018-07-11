Imports IVS.Data
Imports IVS.AppLog

Class WinMain

    Public Shared MyAppLog As New ApplicationLog(ApplicationLog.DateEncodingType.YYYYMMDD, True)
    Public Shared objClientSettings As New ClientSettings
    Public Shared intIVSUserID As Integer
    Public Shared strIVSUserName As String
    Public Shared isUserAdmin As Boolean
    Public Shared isUserSearchAble As Boolean
    Public Shared isUserAlertAble As Boolean
    Public Shared objUserDetail As New UserDetail
    Public Shared intClientID As Integer

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
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdAdmin_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdAdmin.Click

        Dim winAdministration As WinAdmin
        Dim dialogResult As Boolean

        Try
            winAdministration = New WinAdmin()

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
            WinSearchScans = New WinSearch()
            WinSearchScans.Owner = Me
            WinSearchScans.ShowDialog()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdScan_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdScan.Click

        Try
            If objClientSettings.DeviceType = "LS_150_USB" Or objClientSettings.DeviceType = "LS_40_USB" Or objClientSettings.DeviceType = "M210/260" Or objClientSettings.DeviceType = "TM-S9000" Then

                Dim WinScanImage As WinScanImage
                WinScanImage = New WinScanImage()
                WinScanImage.Owner = Me
                WinScanImage.ShowDialog()

            ElseIf objClientSettings.DeviceType = "MAGTEK" Or objClientSettings.DeviceType = "M200/250" Or objClientSettings.DeviceType = "TEXT" Then

                Dim WinScan As WinScan
                WinScan = New WinScan()
                WinScan.Owner = Me
                WinScan.ShowDialog()

            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdLogon_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdLogon.Click

        Dim strUserName As String
        Dim strPassword As String

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

    Private Sub cmdBatch_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdBatch.Click

        Dim MyBatchWindow As WinBatch

        Try
            MyBatchWindow = New WinBatch()
            MyBatchWindow.ShowDialog()

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

            If objClientSettings.SkipLogon = True Then

                intIVSUserID = objClientSettings.DefaultUser
                Me.cmdEditUserAccount.Visibility = Windows.Visibility.Hidden

                objUserDetail = DataAccess.GetUserDetail(intIVSUserID)

                isUserAdmin = objUserDetail.AdminFlag
                isUserSearchAble = objUserDetail.SearchFlag
                isUserAlertAble = objUserDetail.AlertFlag
                strIVSUserName = objUserDetail.UserNameFirst & " " & objUserDetail.UserNameLast

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

            If objClientSettings.DeviceType = "LS_150_USB" Then
                Me.cmdBatch.IsEnabled = True
            Else
                Me.cmdBatch.IsEnabled = False
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

End Class

