Imports IVS.Data

Public Class WinAlerts

    Private intAlertID As Integer

    Public Sub New(ByVal IDAccount As String, Optional ByVal NameLast As String = Nothing)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Try

            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner
            Me.dgAlerts.ItemsSource = DataAccess.GetSwipeScanAlerts(IDAccount, NameLast, WinMain.objClientSettings.Location, WinMain.intClientID)

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgAlerts_CmdView_Click()

        Dim WinAlert As WinAlert
        Dim objAlertDetail As New AlertDetail

        Try
            objAlertDetail = DataAccess.GetAlertDetail(intAlertID)

            WinAlert = New WinAlert(objAlertDetail)
            WinAlert.Owner = Me

            WinAlert.ShowDialog()

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

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

    Private Sub cmdCancel_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCancel.Click

        Try
            Me.Close()
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

End Class
