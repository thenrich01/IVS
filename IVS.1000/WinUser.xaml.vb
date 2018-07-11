Imports System.Text
'Imports IVS.AppLog
Imports IVS.Data

Public Class WinUser

    'Private MyAppLog As New ApplicationLog(ApplicationLog.DateEncodingType.YYYYMMDD, True)
    'Private intIVSUserID As Integer
    Private intUserID As Integer
    Private isContentChanged As Boolean = False

#Region "Control Bound Subs"

    Public Sub New(Optional ByVal UserID As Integer = 0, Optional ByVal EditMode As Boolean = False)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Dim sb As New StringBuilder

        Try
            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner

            'intIVSUserID = IVSUserID

            If UserID > 0 Then
                intUserID = UserID

                Dim objUserDetail As New UserDetail
                'Dim strUserName As String

                objUserDetail = DataAccess.GetUserDetail(intUserID)
                'strUserName = DataAccess.GetUserName(objUserDetail.UserID)

                sb.Append("Updated on ")
                sb.Append(objUserDetail.UpdateTS)

                Me.lblLastUpdate.Content = sb.ToString

                Me.txtUserName.Text = objUserDetail.UserName
                Me.txtUserNameFirst.Text = objUserDetail.UserNameFirst
                Me.txtUserNameLast.Text = objUserDetail.UserNameLast
                Me.txtUserEmail.Text = objUserDetail.UserEmail
                Me.txtUserPhone.Text = objUserDetail.UserPhone
                Me.txtPassword.Text = objUserDetail.Password
                Me.cbIsAdmin.IsChecked = objUserDetail.AdminFlag
                Me.cbIsAlert.IsChecked = objUserDetail.AlertFlag
                Me.cbIsSearch.IsChecked = objUserDetail.SearchFlag
                Me.cbIsActive.IsChecked = objUserDetail.ActiveFlag

                If EditMode = False Then

                    Me.txtUserName.IsEnabled = False
                    Me.txtUserNameFirst.IsEnabled = False
                    Me.txtUserNameLast.IsEnabled = False
                    Me.txtUserEmail.IsEnabled = False
                    Me.txtUserPhone.IsEnabled = False
                    Me.txtPassword.IsEnabled = False
                    Me.cbIsActive.IsEnabled = False
                    Me.cbIsAdmin.IsEnabled = False
                    Me.cbIsAlert.IsEnabled = False
                    Me.imgNameFirstRequired.Visibility = Windows.Visibility.Hidden
                    Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                    Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                    Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                    Me.cmdDelete.Visibility = Windows.Visibility.Hidden
                    Me.cmdSave.Visibility = Windows.Visibility.Hidden
                    Me.cmdCancel.Content = "Close"

                End If

                If WinMain.isUserAdmin = False Then
                    Me.cbIsActive.IsEnabled = False
                    Me.cbIsAdmin.IsEnabled = False
                    Me.cbIsAlert.IsEnabled = False
                    Me.cbIsSearch.IsEnabled = False
                End If

            ElseIf UserID = 0 Then

                Me.lblLastUpdate.Content = ""
                Me.cbIsActive.IsChecked = True
                Me.cbIsAdmin.IsChecked = False
                Me.lblLastUpdate.Visibility = Windows.Visibility.Hidden
                Me.cmdDelete.Visibility = Windows.Visibility.Hidden

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub WinUser_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        Try
            AddHandler cbIsActive.Click, AddressOf TextChanged
            AddHandler cbIsAdmin.Click, AddressOf TextChanged
            AddHandler cbIsSearch.Click, AddressOf TextChanged
            AddHandler cbIsAlert.Click, AddressOf TextChanged
            AddHandler txtPassword.TextChanged, AddressOf TextChanged
            AddHandler txtUserEmail.TextChanged, AddressOf TextChanged
            AddHandler txtUserName.TextChanged, AddressOf TextChanged
            AddHandler txtUserNameFirst.TextChanged, AddressOf TextChanged
            AddHandler txtUserNameLast.TextChanged, AddressOf TextChanged
            AddHandler txtUserPhone.TextChanged, AddressOf TextChanged

            Me.cmdSave.IsEnabled = False

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdSave_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdSave.Click

        Dim objUserDetail As New UserDetail
        Dim CanCloseWindow As Boolean = True

        Try

            objUserDetail.UserName = Me.txtUserName.Text
            objUserDetail.UserNameFirst = Me.txtUserNameFirst.Text
            objUserDetail.UserNameLast = Me.txtUserNameLast.Text
            objUserDetail.UserEmail = Me.txtUserEmail.Text
            objUserDetail.UserPhone = Me.txtUserPhone.Text
            objUserDetail.Password = Me.txtPassword.Text
            objUserDetail.AdminFlag = Me.cbIsAdmin.IsChecked
            objUserDetail.AlertFlag = Me.cbIsAlert.IsChecked
            objUserDetail.SearchFlag = Me.cbIsSearch.IsChecked
            objUserDetail.ActiveFlag = Me.cbIsActive.IsChecked

            If intUserID = 0 Then

                Dim IsUserNameAvailable As Boolean

                IsUserNameAvailable = DataAccess.IsUserNameAvailable(Me.txtUserName.Text)

                If IsUserNameAvailable = True Then
                    DataAccess.NewUser(objUserDetail)
                Else
                    Me.lblLastUpdate.Content = "UserName unavailable"
                    Me.lblLastUpdate.Visibility = Windows.Visibility.Visible
                    CanCloseWindow = False
                End If

            Else
                objUserDetail.UserID = intUserID

                DataAccess.UpdateUser(objUserDetail)

            End If
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If CanCloseWindow = True Then
                Me.DialogResult = True
                Me.Close()
            Else

            End If

        End Try

    End Sub

    Private Sub cmdDelete_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdDelete.Click

        Try
            If MessageBox.Show("Are you sure you want to delete this user?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

                DataAccess.DeleteUser(intUserID)
            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally
            Me.DialogResult = True
            Me.Close()

        End Try

    End Sub

    Private Sub cmdCancel_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCancel.Click

        Try
            If isContentChanged = True Then

                If MessageBox.Show("Exit without saving changes?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

                    Me.DialogResult = False
                    Me.Close()

                End If
            Else

                Me.DialogResult = False
                Me.Close()

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

#Region "Custom Subs/Functions"

    Private Sub TextChanged()

        Try
            isContentChanged = True

            If Me.txtUserNameFirst.Text <> "" And Me.txtUserNameLast.Text <> "" And txtUserName.Text <> "" And txtPassword.Text <> "" Then

                Me.cmdSave.IsEnabled = True

            Else
                Me.cmdSave.IsEnabled = False
            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

End Class