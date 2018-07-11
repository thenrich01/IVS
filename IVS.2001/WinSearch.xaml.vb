Imports System.Data
Imports System.Text
Imports System.Windows.Forms
Imports IVS.Data
Imports IVS.AppLog
Imports IVS.Data.IVSService

Public Class WinSearch

    Private MyAppLog As New ApplicationLog(ApplicationLog.DateEncodingType.YYYYMMDD, True)
    Private sList As List(Of SwipeScanSearch)
    Private intSwipeScanID As Integer
    Private strSwipeScanType As String
    Private strDeviceType As String
    Private strImageLocation As String
    Private intViewingTime As Integer
    Private intIVSUserID As Integer

    Public Sub New(ByVal IVSUserID As Integer, ByVal DeviceType As String, ByVal ImageLocation As String, ByVal ViewingTime As Integer)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Try
            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner
            strImageLocation = ImageLocation
            intViewingTime = ViewingTime
            intIVSUserID = IVSUserID
            strDeviceType = DeviceType
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#Region "Control Bound Subs"

    Private Sub WinSearch_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        Try
            sList = DataAccess.GetSwipeScanSearch
            Me.dgSearchResults.ItemsSource = sList

            Me.cboUsers.ItemsSource = DataAccess.GetUserNames
            Me.cboUsers.DisplayMemberPath = ("UserName")
            Me.cboUsers.SelectedValuePath = ("UserID")

            Me.cboStations.ItemsSource = DataAccess.GetLocations
            Me.cboStations.DisplayMemberPath = ("Location")
            Me.cboStations.SelectedValuePath = ("ClientID")

            Me.cboSwipeScanType.Items.Add("Drivers License Or State ID")
            Me.cboSwipeScanType.Items.Add("Credit Card")
            Me.cboSwipeScanType.Items.Add("Check")
            Me.cboSwipeScanType.Items.Add("Military ID Card")
            Me.cboSwipeScanType.Items.Add("INS Employee Authorization Card")

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdClearCaseID_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdClearCaseID.Click

        Try
            Me.txtCaseID.Text = Nothing
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdClearIDAccount_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdClearIDAccount.Click

        Try
            Me.txtIDAccount.Text = Nothing
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdClearcboUser_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdClearcboUser.Click

        Try
            Me.cboUsers.SelectedIndex = -1
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdClearcboStation_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdClearcboStation.Click

        Try
            Me.cboStations.SelectedIndex = -1
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdClearNameFirst_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdClearNameFirst.Click

        Try
            Me.txtNameFirst.Text = Nothing
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdClearNameLast_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdClearNameLast.Click

        Try
            Me.txtNameLast.Text = Nothing
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdClearSwipeScanDate_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdClearSwipeScanDate.Click

        Try
            Me.DateSwipeScanDate.SelectedDate = Nothing
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdClearSwipeScanType_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdClearSwipeScanType.Click

        Try
            Me.cboSwipeScanType.SelectedIndex = -1
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdSearch_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdSearch.Click

        Dim objStationSelected As Locations
        Dim objUserSelected As UserDetail
        Dim strSelectedDateMax As Date
        Dim sb As New StringBuilder

        Dim FilteredList As List(Of SwipeScanSearch)

        Try
            FilteredList = sList

            If txtCaseID.Text <> "" Then

                FilteredList = FilteredList.FindAll(Function(s As SwipeScanSearch) s.CaseID.ToUpper.Contains(Me.txtCaseID.Text.Trim(" ").ToUpper))

            End If

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
                FilteredList = FilteredList.FindAll(Function(s As SwipeScanSearch) s.ClientID = objStationSelected.LocationID)

            End If

            If Me.cboUsers.SelectedIndex >= 0 Then

                objUserSelected = Me.cboUsers.SelectedItem
                FilteredList = FilteredList.FindAll(Function(s As SwipeScanSearch) s.UserName = objUserSelected.UserName)

            End If

            If Me.cboSwipeScanType.SelectedIndex >= 0 Then

                FilteredList = FilteredList.FindAll(Function(s As SwipeScanSearch) s.SwipeScanType = Me.cboSwipeScanType.SelectedItem)

            End If

            Me.dgSearchResults.ItemsSource = FilteredList

        Catch exICE As InvalidCastException
            MyAppLog.WriteToLog("IVS", exICE.ToString, EventLogEntryType.Error)
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgSearchResults_CmdDelete_Click()

        Try
            If System.Windows.MessageBox.Show("Are you sure you want to delete the Swipe data?", "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

                DataAccess.DeleteSwipeScan(intSwipeScanID)

                sList = sList.FindAll(Function(s As SwipeScanSearch) s.SwipeScanID <> intSwipeScanID)
                Me.dgSearchResults.ItemsSource = sList

                CTS.CTSApi.DeleteImage(intSwipeScanID, strImageLocation)

            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdClose_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdClose.Click
        Me.Close()
    End Sub

#End Region

    Private Sub dgSearchResults_cmdViewImage_Click()

        Dim WinViewImage As WinView

        Try

            WinViewImage = New WinView(intSwipeScanID, strSwipeScanType, strImageLocation, intViewingTime)
            WinViewImage.Owner = Me
            WinViewImage.ShowDialog()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgSearchResults_CmdView_Click()
        MyAppLog.WriteToLog("WinSearch.dgSearchResults_CmdView_Click()" & intIVSUserID)
        Try
            If strDeviceType = "LS_40_USB" Or strDeviceType = "LS_150_USB" Then

                Dim winCTSScan As WinCTS
                winCTSScan = New WinCTS(intIVSUserID, intSwipeScanID)
                winCTSScan.Owner = Me
                winCTSScan.ShowDialog()

            ElseIf strDeviceType = "ESEEK" Then

                Dim winESeekScan As WinESeek
                winESeekScan = New WinESeek(intIVSUserID, intSwipeScanID)
                winESeekScan.Owner = Me
                winESeekScan.ShowDialog()

            ElseIf strDeviceType = "MAGTEK" Then

                Dim winMagTekScan As WinMagTek
                winMagTekScan = New WinMagTek(intIVSUserID, intSwipeScanID)
                winMagTekScan.Owner = Me
                winMagTekScan.ShowDialog()

            ElseIf strDeviceType = "No Reader" Then

                Dim winCTSScan As WinCTS
                winCTSScan = New WinCTS(intIVSUserID, intSwipeScanID)
                winCTSScan.Owner = Me
                winCTSScan.ShowDialog()

            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgSearchResults_SelectionChanged(sender As Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgSearchResults.SelectionChanged

        Dim dgSwipeScanHistory_SelectedRow As System.Collections.IList

        Try

            dgSwipeScanHistory_SelectedRow = e.AddedItems

            If dgSwipeScanHistory_SelectedRow.Count > 0 Then

                intSwipeScanID = dgSwipeScanHistory_SelectedRow(0).SwipeScanID
                strSwipeScanType = dgSwipeScanHistory_SelectedRow(0).SwipeScanType
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try
    End Sub

    Private Sub cmdNewAlert_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdNewAlert.Click

        Dim WinAlert As WinAlert
        Dim DialogResult As Boolean
        Dim objAlertDetail As New AlertDetail

        Try

            objAlertDetail.AlertID = 0
            objAlertDetail.AlertContactName = DataAccess.GetUserName(intIVSUserID)
            objAlertDetail.AlertContactNumber = DataAccess.GetUserPhone(intIVSUserID)
            objAlertDetail.UserID = intIVSUserID

            WinAlert = New WinAlert(objAlertDetail)
            WinAlert.Owner = Me

            DialogResult = WinAlert.ShowDialog

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

End Class