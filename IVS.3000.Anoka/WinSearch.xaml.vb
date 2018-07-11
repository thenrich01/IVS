Imports System.Text
Imports IVS.Data
Imports System.Windows.Forms

Public Class WinSearch

    Private sList As List(Of SwipeScanSearch)
    Private intSwipeScanID As Integer
    Private strSwipeScanType As String
    Private strIDAccountNumber As String
    Private strNameLast As String
    Private strNameFirst As String
    Private strDateOfBirth As String

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Try
            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner
         
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#Region "Control Bound Subs"

    Private Sub WinSearch_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        Try
            sList = DataAccess.GetSwipeScanSearch
            Me.dgSearchResults.ItemsSource = sList

            Me.cboUsers.ItemsSource = DataAccess.GetUserNames(0, 0)
            Me.cboUsers.DisplayMemberPath = ("UserName")
            Me.cboUsers.SelectedValuePath = ("UserID")

            Me.cboStations.ItemsSource = DataAccess.GetStations
            Me.cboStations.DisplayMemberPath = ("Station")
            Me.cboStations.SelectedValuePath = ("ClientID")

            Me.cboLocations.ItemsSource = DataAccess.GetLocations
            Me.cboLocations.DisplayMemberPath = ("LocationDescription")
            Me.cboLocations.SelectedValuePath = ("LocationID")

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdClearIDAccount_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdClearIDAccount.Click

        Try
            Me.txtIDAccount.Text = Nothing
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdClearcboUser_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdClearcboUser.Click

        Try
            Me.cboUsers.SelectedIndex = -1
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdClearcboStation_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdClearcboStation.Click

        Try
            Me.cboStations.SelectedIndex = -1
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdClearcboLocation_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdClearcboLocation.Click

        Try
            Me.cboLocations.SelectedIndex = -1
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdClearNameFirst_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdClearNameFirst.Click

        Try
            Me.txtNameFirst.Text = Nothing
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdClearNameLast_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdClearNameLast.Click

        Try
            Me.txtNameLast.Text = Nothing
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdClearSwipeScanDate_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdClearSwipeScanDate.Click

        Try
            Me.DateSwipeScanDate.SelectedDate = Nothing
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdSearch_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdSearch.Click

        Dim objLocationSelected As Locations
        Dim objStationSelected As Clients
        Dim objUserSelected As UserDetail
        Dim strSelectedDateMax As Date

        Dim FilteredList As List(Of SwipeScanSearch)

        Try
            FilteredList = sList

            If txtIDAccount.Text <> "" Then

                FilteredList = FilteredList.FindAll(Function(s As SwipeScanSearch) s.IDAccountNumber.ToUpper.Contains(Me.txtIDAccount.Text.Trim(" ").ToUpper))

            End If

            If txtNameFirst.Text <> "" Then

                FilteredList = FilteredList.FindAll(Function(s As SwipeScanSearch) s.NameFirst.ToUpper.Contains(Me.txtNameFirst.Text.Trim(" ").ToUpper))

            End If

            If txtNameLast.Text <> "" Then

                FilteredList = FilteredList.FindAll(Function(s As SwipeScanSearch) s.NameLast.ToUpper.Contains(Me.txtNameLast.Text.Trim(" ").ToUpper))

            End If

            If DateSwipeScanDate.SelectedDate.HasValue Then

                strSelectedDateMax = DateAdd(DateInterval.Day, 1, DateSwipeScanDate.SelectedDate.Value)
                FilteredList = FilteredList.FindAll(Function(s As SwipeScanSearch) s.SwipeScanTS >= DateSwipeScanDate.SelectedDate.Value AndAlso s.SwipeScanTS < strSelectedDateMax)

            End If

            If Me.cboStations.SelectedIndex >= 0 Then

                objStationSelected = Me.cboStations.SelectedItem
                FilteredList = FilteredList.FindAll(Function(s As SwipeScanSearch) s.ClientID = objStationSelected.ClientID)

            End If

            If Me.cboLocations.SelectedIndex >= 0 Then

                objLocationSelected = Me.cboLocations.SelectedItem
                FilteredList = FilteredList.FindAll(Function(s As SwipeScanSearch) s.Location = objLocationSelected.LocationDescription)

            End If

            If Me.cboUsers.SelectedIndex >= 0 Then

                objUserSelected = Me.cboUsers.SelectedItem
                FilteredList = FilteredList.FindAll(Function(s As SwipeScanSearch) s.UserName = objUserSelected.UserName)

            End If

            Me.dgSearchResults.ItemsSource = FilteredList

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgSearchResults_cmdViewImage_Click()

        Dim WinViewImage As WinView

        Try

            WinViewImage = New WinView(intSwipeScanID)
            WinViewImage.Owner = Me
            WinViewImage.ShowDialog()

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgSearchResults_CmdDelete_Click()

        Try
            If System.Windows.MessageBox.Show("Are you sure you want to delete the Swipe data?", "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

                DataAccess.DeleteSwipeScan(intSwipeScanID)

                sList = sList.FindAll(Function(s As SwipeScanSearch) s.SwipeScanID <> intSwipeScanID)
                Me.dgSearchResults.ItemsSource = sList

                CTS.CTSApi.DeleteImage(intSwipeScanID, WinMain.objClientSettings.ImageLocation)

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgSearchResults_CmdNewAlert_Click()

        Dim WinAlert As WinAlert
        Dim DialogResult As Boolean
        Dim objAlertDetail As New AlertDetail

        Try

            objAlertDetail.AlertID = 0
            objAlertDetail.IDNumber = strIDAccountNumber
            objAlertDetail.NameLast = strNameLast
            objAlertDetail.NameFirst = strNameFirst
            objAlertDetail.DateOfBirth = strDateOfBirth
            objAlertDetail.AlertContactName = WinMain.strIVSUserName
            objAlertDetail.AlertContactNumber = WinMain.objClientSettings.Phone
            objAlertDetail.UserID = WinMain.intIVSUserId

            WinAlert = New WinAlert(objAlertDetail)
            WinAlert.Owner = Me

            DialogResult = WinAlert.ShowDialog

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgSearchResults_SelectionChanged(sender As Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgSearchResults.SelectionChanged

        Dim dgSwipeScanHistory_SelectedRow As System.Collections.IList

        Try

            dgSwipeScanHistory_SelectedRow = e.AddedItems

            If dgSwipeScanHistory_SelectedRow.Count > 0 Then

                intSwipeScanID = dgSwipeScanHistory_SelectedRow(0).SwipeScanID
                strSwipeScanType = dgSwipeScanHistory_SelectedRow(0).SwipeScanType
                strIDAccountNumber = dgSwipeScanHistory_SelectedRow(0).IDAccountNumber
                strNameLast = dgSwipeScanHistory_SelectedRow(0).NameLast
                strNameFirst = dgSwipeScanHistory_SelectedRow(0).NameFirst
                strDateOfBirth = dgSwipeScanHistory_SelectedRow(0).DateOfBirth

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try
    End Sub

    Private Sub cmdClose_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdClose.Click
        Me.Close()
    End Sub

    Private Sub cmdNewAlert_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdNewAlert.Click

        Dim WinAlert As WinAlert
        Dim DialogResult As Boolean
        Dim objAlertDetail As New AlertDetail

        Try

            objAlertDetail.AlertID = 0
            objAlertDetail.AlertContactName = WinMain.strIVSUserName
            objAlertDetail.AlertContactNumber = WinMain.objClientSettings.Phone
            objAlertDetail.UserID = WinMain.intIVSUserId

            WinAlert = New WinAlert(objAlertDetail)
            WinAlert.Owner = Me

            DialogResult = WinAlert.ShowDialog

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

End Class