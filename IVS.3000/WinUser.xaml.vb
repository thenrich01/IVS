Imports System.Text
Imports IVS.Data

Public Class WinUser

    Private objUserDetail As UserDetail
    Private intUserID As Integer
    Private vList As List(Of Clients)
    Private isContentChanged As Boolean = False

#Region "Control Bound Subs"

    Public Sub New(Optional ByVal UserID As Integer = 0, Optional ByVal EditMode As Boolean = False)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Dim sb As New StringBuilder

        Try
            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner

            vList = DataAccess.GetStations

            Me.cboStations.ItemsSource = vList
            Me.cboStations.DisplayMemberPath = ("Station")
            Me.cboStations.SelectedValuePath = ("ClientID")

            Me.cboLocations.ItemsSource = DataAccess.GetLocations
            Me.cboLocations.DisplayMemberPath = ("LocationDescription")
            Me.cboLocations.SelectedValuePath = ("LocationID")

            Me.cbLocationGlobal.IsChecked = True
            Me.cboLocations.IsEnabled = False
            Me.cboStations.IsEnabled = False
            Me.cbClientGlobal.IsChecked = True
            Me.cbIsActive.IsChecked = True

            If UserID > 0 Then

                intUserID = UserID

                Dim strUserName As String

                objUserDetail = DataAccess.GetUserDetail(intUserID)
                strUserName = DataAccess.GetUserName(objUserDetail.UserID)

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

                If objUserDetail.LocationID = 0 Then
                    'global
                    Me.cbLocationGlobal.IsChecked = True
                    Me.cboLocations.IsEnabled = False

                    Me.cbClientGlobal.IsChecked = False
                    Me.cbClientGlobal.IsEnabled = False

                    Me.cboStations.IsEnabled = False

                Else

                    Me.cbLocationGlobal.IsChecked = False
                    Me.cboLocations.IsEnabled = True
                    'set locationid

                    Me.cboLocations.SelectedValue = objUserDetail.LocationID

                    If objUserDetail.ClientID = 0 Then

                        Me.cbClientGlobal.IsChecked = True
                        Me.cboStations.IsEnabled = False

                    Else

                        Me.cbClientGlobal.IsChecked = False
                        Me.cbClientGlobal.IsEnabled = True

                        'set clientid 
                        Dim FilteredList As List(Of Clients)

                        FilteredList = vList
                        FilteredList = FilteredList.FindAll(Function(s As Clients) s.Location = objUserDetail.LocationID)

                        Me.cboStations.ItemsSource = FilteredList
                        Me.cboStations.DisplayMemberPath = ("Station")
                        Me.cboStations.SelectedValuePath = ("ClientID")

                        Me.cboStations.SelectedValue = objUserDetail.ClientID

                    End If
                End If

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
            AddHandler cbLocationGlobal.Checked, AddressOf TextChanged
            AddHandler cbClientGlobal.Checked, AddressOf TextChanged
            AddHandler cboLocations.SelectionChanged, AddressOf TextChanged
            AddHandler cboStations.SelectionChanged, AddressOf TextChanged

            Me.cmdSave.IsEnabled = False

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdSave_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdSave.Click

        Dim objNewUserDetail As New UserDetail
        Dim CanCloseWindow As Boolean = True
        Dim objLocationSelected As Locations
        Dim objStationSelected As Clients

        Try
            objNewUserDetail.UserName = Me.txtUserName.Text
            objNewUserDetail.UserNameFirst = Me.txtUserNameFirst.Text
            objNewUserDetail.UserNameLast = Me.txtUserNameLast.Text
            objNewUserDetail.UserEmail = Me.txtUserEmail.Text
            objNewUserDetail.UserPhone = Me.txtUserPhone.Text
            objNewUserDetail.Password = Me.txtPassword.Text
            objNewUserDetail.AdminFlag = Me.cbIsAdmin.IsChecked
            objNewUserDetail.AlertFlag = Me.cbIsAlert.IsChecked
            objNewUserDetail.SearchFlag = Me.cbIsSearch.IsChecked
            objNewUserDetail.ActiveFlag = Me.cbIsActive.IsChecked

            If cbLocationGlobal.IsChecked = True Then
                objNewUserDetail.LocationID = 0
                objNewUserDetail.ClientID = 0
            Else
                objLocationSelected = Me.cboLocations.SelectedItem

                objNewUserDetail.LocationID = objLocationSelected.LocationID

                If cbClientGlobal.IsChecked = True Then
                    objNewUserDetail.ClientID = 0
                Else
                    objStationSelected = Me.cboStations.SelectedItem
                    objNewUserDetail.ClientID = objStationSelected.ClientID
                End If
            End If

            If intUserID = 0 Then

                Dim IsUserNameAvailable As Boolean

                IsUserNameAvailable = DataAccess.IsUserNameAvailable(Me.txtUserName.Text)

                If IsUserNameAvailable = True Then
                    DataAccess.NewUser(objNewUserDetail)
                Else
                    Me.lblLastUpdate.Content = "UserName unavailable"
                    Me.lblLastUpdate.Visibility = Windows.Visibility.Visible
                    CanCloseWindow = False
                End If

            Else
               
                objNewUserDetail.UserID = intUserID
                DataAccess.UpdateUser(objNewUserDetail)

            End If

            If CanCloseWindow = True Then
                Me.DialogResult = True
                Me.Close()
            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdDelete_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdDelete.Click

        Try
            If MessageBox.Show("Are you sure you want to delete this user?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

                DataAccess.DeleteUser(intUserID)
                Me.DialogResult = True
                Me.Close()

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
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

    Private Sub cbLocationGlobal_Checked(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cbLocationGlobal.Checked

        Try
            Me.cbClientGlobal.IsChecked = True
            Me.cboLocations.IsEnabled = False
            Me.cboStations.IsEnabled = False
            Me.cbClientGlobal.IsEnabled = False

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cbLocationGlobal_Unchecked(sender As Object, e As System.Windows.RoutedEventArgs) Handles cbLocationGlobal.Unchecked

        Dim FilteredList As List(Of Clients)

        Try
            Me.cboLocations.IsEnabled = True
            Me.cbClientGlobal.IsEnabled = True

            If objUserDetail.ClientID = 0 Then

                Me.cbClientGlobal.IsChecked = True
                Me.cboStations.IsEnabled = False

            Else

                Me.cbClientGlobal.IsChecked = False
                Me.cbClientGlobal.IsEnabled = True

                'set clientid       
                FilteredList = vList
                FilteredList = FilteredList.FindAll(Function(s As Clients) s.Location = Me.cboLocations.SelectedValue)

                Me.cboStations.ItemsSource = FilteredList
                Me.cboStations.DisplayMemberPath = ("Station")
                Me.cboStations.SelectedValuePath = ("ClientID")

                Me.cboStations.SelectedValue = objUserDetail.ClientID

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cbClientGlobal_Checked(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cbClientGlobal.Checked

        Try
            Me.cboStations.IsEnabled = False

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cbClientGlobal_Unchecked(sender As Object, e As System.Windows.RoutedEventArgs) Handles cbClientGlobal.Unchecked

        Dim FilteredList As List(Of Clients)

        Try
            Me.cboStations.IsEnabled = True

            FilteredList = vList
            FilteredList = FilteredList.FindAll(Function(s As Clients) s.Location = Me.cboLocations.SelectedValue)

            Me.cboStations.ItemsSource = FilteredList
            Me.cboStations.DisplayMemberPath = ("Station")
            Me.cboStations.SelectedValuePath = ("ClientID")

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cboLocations_SelectionChanged(sender As Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles cboLocations.SelectionChanged

        Dim FilteredList As List(Of Clients)

        Try
            FilteredList = vList
            FilteredList = FilteredList.FindAll(Function(s As Clients) s.Location = Me.cboLocations.SelectedValue)

            Me.cboStations.ItemsSource = FilteredList
            Me.cboStations.DisplayMemberPath = ("Station")
            Me.cboStations.SelectedValuePath = ("ClientID")

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