Imports System.Text
Imports IVS.Data

Public Class WinAlert

    Private objAlertDetail As AlertDetail
    Private isContentChanged As Boolean = False
    Private vList As List(Of Clients)

#Region "Control Bound Subs"

    Public Sub New(ByVal AlertDetail As AlertDetail, Optional ByVal EditMode As Boolean = False)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Dim sb As New StringBuilder

        Try
            objAlertDetail = AlertDetail

            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner

            vList = DataAccess.GetStations

            Me.cboStations.ItemsSource = vList
            Me.cboStations.DisplayMemberPath = ("Station")
            Me.cboStations.SelectedValuePath = ("ClientID")

            Me.cboLocations.ItemsSource = DataAccess.GetLocations
            Me.cboLocations.DisplayMemberPath = ("LocationDescription")
            Me.cboLocations.SelectedValuePath = ("LocationID")

            If objAlertDetail.AlertID = 0 Then

                If objAlertDetail.IDNumber IsNot Nothing Then
                    Me.txtAlertIDAccountNumber.Text = objAlertDetail.IDNumber
                End If

                If objAlertDetail.NameFirst IsNot Nothing Then
                    Me.txtAlertNameFirst.Text = objAlertDetail.NameFirst
                End If

                If objAlertDetail.NameLast IsNot Nothing Then
                    Me.txtAlertNameLast.Text = objAlertDetail.NameLast
                End If

                If IsDate(objAlertDetail.DateOfBirth) And objAlertDetail.DateOfBirth <> DateTime.MinValue Then
                    Me.txtDateOfBirth.Text = objAlertDetail.DateOfBirth
                Else
                    Me.txtDateOfBirth.Text = Nothing
                End If

                Me.txtAlertContactName.Text = objAlertDetail.AlertContactName
                Me.txtAlertContactNumber.Text = objAlertDetail.AlertContactNumber

                If objAlertDetail.ActiveFlag = False Then
                    Me.cbIsActive.IsChecked = False
                Else
                    Me.cbIsActive.IsChecked = True
                End If

                Me.cbIsActive.IsChecked = True

                Me.lblLastUpdate.Visibility = Windows.Visibility.Hidden
                Me.cmdDelete.Visibility = Windows.Visibility.Hidden


                Me.cbLocationGlobal.IsChecked = True
                Me.cboLocations.IsEnabled = False
                Me.cboStations.IsEnabled = False
                Me.cbClientGlobal.IsChecked = True

            End If

            If objAlertDetail.AlertID > 0 Then

                Me.txtAlertIDAccountNumber.Text = objAlertDetail.IDNumber
                Me.txtAlertNameFirst.Text = objAlertDetail.NameFirst
                Me.txtAlertNameLast.Text = objAlertDetail.NameLast

                If IsDate(objAlertDetail.DateOfBirth) And objAlertDetail.DateOfBirth <> DateTime.MinValue Then
                    Me.txtDateOfBirth.Text = objAlertDetail.DateOfBirth
                Else
                    Me.txtDateOfBirth.Text = Nothing
                End If

                Me.txtAlertContactName.Text = objAlertDetail.AlertContactName
                Me.txtAlertContactNumber.Text = objAlertDetail.AlertContactNumber

                Me.cbIsActive.IsChecked = objAlertDetail.ActiveFlag
                Me.txtAlertNotes.Text = objAlertDetail.AlertNotes

                If objAlertDetail.LocationID = 0 Then
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

                    Me.cboLocations.SelectedValue = objAlertDetail.LocationID

                    If objAlertDetail.ClientID = 0 Then

                        Me.cbClientGlobal.IsChecked = True
                        Me.cboStations.IsEnabled = False

                    Else

                        Me.cbClientGlobal.IsChecked = False
                        Me.cbClientGlobal.IsEnabled = True

                        'set clientid
                        Dim FilteredList As List(Of Clients)

                        FilteredList = vList
                        FilteredList = FilteredList.FindAll(Function(s As Clients) s.Location = objAlertDetail.LocationID)

                        Me.cboStations.ItemsSource = FilteredList
                        Me.cboStations.DisplayMemberPath = ("Station")
                        Me.cboStations.SelectedValuePath = ("ClientID")

                        Me.cboStations.SelectedValue = objAlertDetail.ClientID

                    End If
                End If

                sb.Append("Updated on ")
                sb.Append(objAlertDetail.UpdateTS)
                sb.Append(" by ")
                sb.Append(objAlertDetail.UserName)

                Me.lblLastUpdate.Content = sb.ToString

                Me.txtAlertContactName.Focus()

                If EditMode = False Then

                    Me.txtAlertIDAccountNumber.IsEnabled = False
                    Me.txtAlertNameFirst.IsEnabled = False
                    Me.txtAlertNameLast.IsEnabled = False
                    Me.txtDateOfBirth.IsEnabled = False
                    Me.txtAlertContactName.IsEnabled = False
                    Me.txtAlertContactNumber.IsEnabled = False
                    Me.cbIsActive.IsEnabled = False
                    Me.txtAlertNotes.IsEnabled = False
                    Me.imgIDRequired.Visibility = Windows.Visibility.Hidden
                    Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                    Me.cmdSave.Visibility = Windows.Visibility.Hidden
                    Me.cmdDelete.Visibility = Windows.Visibility.Hidden
                    Me.cmdCancel.Content = "Close"

                    Me.cbLocationGlobal.IsEnabled = False
                    Me.cbClientGlobal.IsEnabled = False
                    Me.cboLocations.IsEnabled = False
                    Me.cboStations.IsEnabled = False

                End If
            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub WinAlert_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        Try
            AddHandler cbIsActive.Click, AddressOf TextChanged
            AddHandler txtAlertNameFirst.TextChanged, AddressOf TextChanged
            AddHandler txtAlertNameLast.TextChanged, AddressOf TextChanged
            AddHandler txtDateOfBirth.TextChanged, AddressOf TextChanged
            AddHandler txtAlertContactName.TextChanged, AddressOf TextChanged
            AddHandler txtAlertContactNumber.TextChanged, AddressOf TextChanged
            AddHandler txtAlertIDAccountNumber.TextChanged, AddressOf TextChanged
            AddHandler txtAlertNotes.TextChanged, AddressOf TextChanged
            AddHandler cbLocationGlobal.Checked, AddressOf TextChanged
            AddHandler cbClientGlobal.Checked, AddressOf TextChanged
            AddHandler cboLocations.SelectionChanged, AddressOf TextChanged
            AddHandler cboStations.SelectionChanged, AddressOf TextChanged

            Me.cmdSave.IsEnabled = False

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdDelete_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdDelete.Click

        Try

            If MessageBox.Show("Are you sure you want to delete this alert?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

                DataAccess.DeleteAlert(objAlertDetail.AlertID)
                Me.DialogResult = True
                Me.Close()

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdSave_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdSave.Click

        Dim objNewAlertDetail As New AlertDetail
        Dim objLocationSelected As Locations
        Dim objStationSelected As Clients

        Try

            Dim parseDateOfBirth As DateTime
            Dim TryParseDateResult As Boolean = DateTime.TryParse(Me.txtDateOfBirth.Text, parseDateOfBirth)

            If Me.txtDateOfBirth.Text <> "" Then
                If TryParseDateResult = True Then
                    objNewAlertDetail.DateOfBirth = parseDateOfBirth.ToShortDateString
                Else

                    If MessageBox.Show("The date of birth entered is not a valid date.  Press Cancel to update the date of birth before proceeding.  Press OK to proceed without saving the date of birth. ", "Invalid Date of Birth", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.Cancel Then

                        Exit Sub

                    End If

                End If

            Else
                objNewAlertDetail.DateOfBirth = DateTime.MinValue.ToShortDateString
            End If

            objNewAlertDetail.AlertType = "ALERT"
            objNewAlertDetail.IDNumber = Me.txtAlertIDAccountNumber.Text
            objNewAlertDetail.NameFirst = Me.txtAlertNameFirst.Text
            objNewAlertDetail.NameLast = Me.txtAlertNameLast.Text
            'objNewAlertDetail.DateOfBirth = Me.txtDateOfBirth.Text
            objNewAlertDetail.AlertContactName = Me.txtAlertContactName.Text
            objNewAlertDetail.AlertContactNumber = Me.txtAlertContactNumber.Text
            objNewAlertDetail.ActiveFlag = Me.cbIsActive.IsChecked
            objNewAlertDetail.AlertNotes = Me.txtAlertNotes.Text
            objNewAlertDetail.UserID = objAlertDetail.UserID

            If cbLocationGlobal.IsChecked = True Then
                objNewAlertDetail.LocationID = 0
                objNewAlertDetail.ClientID = 0
            Else
                objLocationSelected = Me.cboLocations.SelectedItem

                objNewAlertDetail.LocationID = objLocationSelected.LocationID

                If cbClientGlobal.IsChecked = True Then
                    objNewAlertDetail.ClientID = 0
                Else
                    objStationSelected = Me.cboStations.SelectedItem
                    objNewAlertDetail.ClientID = objStationSelected.ClientID
                End If
            End If

            If objAlertDetail.AlertID = 0 Then

                DataAccess.NewAlert(objNewAlertDetail)

            Else

                objNewAlertDetail.AlertID = objAlertDetail.AlertID
                DataAccess.UpdateAlert(objNewAlertDetail)

            End If

            Me.DialogResult = True
            Me.Close()

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

#End Region

#Region "Custom Subs/Functions"

    Private Sub TextChanged()

        Try
            isContentChanged = True

            If Me.txtAlertIDAccountNumber.Text <> "" Or Me.txtAlertNameLast.Text <> "" Then

                Me.cmdSave.IsEnabled = True

            Else
                Me.cmdSave.IsEnabled = False
            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

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

            If objAlertDetail.ClientID = 0 Then

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

                Me.cboStations.SelectedValue = objAlertDetail.ClientID

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

End Class