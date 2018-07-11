Imports IVS.Data

Public Class WinAlerts

    Private intAlertID As Integer
    Private strAlertType As String

    Public Sub New(ByVal IDAccount As String, Optional ByVal NameLast As String = Nothing, Optional ByVal NameFirst As String = Nothing, Optional ByVal DateOfBirth As String = Nothing)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Try

            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner
            Me.dgAlerts.ItemsSource = DataAccess.GetSwipeScanAlerts_Anoka(IDAccount, NameLast, NameFirst, DateOfBirth, WinMain.objClientSettings.Location, WinMain.intClientID, WinMain.objClientSettings.InternalLoc)

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgAlerts_CmdView_Click()

        Try
            If strAlertType = "IVS" Then

                Dim WinAlert As WinAlert
                Dim objAlertDetail As New AlertDetail

                objAlertDetail = DataAccess.GetAlertDetail(intAlertID)

                WinAlert = New WinAlert(objAlertDetail)
                WinAlert.Owner = Me

                WinAlert.ShowDialog()

            ElseIf strAlertType = "SYNERGY" Then

                Dim WinAlertAnoka As WinAlertAnoka
                Dim objAlertDetailAnoka As AlertDetailAnoka

                objAlertDetailAnoka = DataAccess.GetAlertDetail_Anoka(intAlertID)

                WinAlertAnoka = New WinAlertAnoka(objAlertDetailAnoka)
                WinAlertAnoka.Owner = Me

                WinAlertAnoka.ShowDialog()

            End If

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
                strAlertType = dgAlerts_SelectedRow(0).AlertType

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
