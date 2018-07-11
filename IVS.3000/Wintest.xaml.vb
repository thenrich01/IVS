Imports IVS.Data

Public Class Wintest

    Private intSwipeScanID As Integer
    Private intAlertID As Integer
    Private objDymoLabel As DYMO.Label.Framework.ILabel

    Public Sub New(ByVal objVisitorInfo As VisitorInfo)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Try
            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner

            Me.txtNameFirst.Content = objVisitorInfo.NameFirst
            Me.txtNameLast.Content = objVisitorInfo.NameLast

            intSwipeScanID = objVisitorInfo.SwipeScanID

            Me.dgAlerts.ItemsSource = DataAccess.GetSwipeScanAlerts(objVisitorInfo.IDAccountNumber, objVisitorInfo.NameLast, WinMain.objClientSettings.Location, WinMain.intClientID)

            Me.cboVisiting.ItemsSource = DataAccess.GetVisitingList(WinMain.objClientSettings.Location, WinMain.intClientID)
            Me.cboVisiting.DisplayMemberPath = ("VisitingName")
            Me.cboVisiting.SelectedIndex = 0

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

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

    Private Sub cmdPrintBadge_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdPrintBadge.Click

        Dim objVisitorInfo As VisitorInfo
        Dim objDymoPrinter As DYMO.Label.Framework.IPrinter
        Dim objDymoLabelWriterPrinter As DYMO.Label.Framework.ILabelWriterPrinter
        Dim strDymoLabelObjectName As String
        Dim strLocationDescription As String
        Dim objVisiting_SelectedRow As Visiting

        Try
            objDymoPrinter = DYMO.Label.Framework.Framework.GetPrinters()(WinMain.objClientSettings.DymoPrinter)
            objDymoLabel = DYMO.Label.Framework.Framework.Open(WinMain.objClientSettings.DymoLabel)
            strLocationDescription = DataAccess.GetLocation(WinMain.objClientSettings.Location)

            For Each strDymoLabelObjectName In objDymoLabel.ObjectNames

                If Not String.IsNullOrEmpty(strDymoLabelObjectName) Then

                    Select Case strDymoLabelObjectName

                        Case "VISITORNAME"

                            If Me.cbAnonymous.IsChecked = True Then

                                objDymoLabel.SetObjectText("VISITORNAME", "Guest Visitor")
                            Else

                                objDymoLabel.SetObjectText("VISITORNAME", Me.txtNameFirst.Content & " " & Me.txtNameLast.Content)
                            End If

                        Case "VISITORLAST"

                            If Me.cbAnonymous.IsChecked = True Then

                                objDymoLabel.SetObjectText("VISITORLAST", "Visitor")
                            Else

                                objDymoLabel.SetObjectText("VISITORLAST", Me.txtNameLast.Content)
                            End If

                        Case "VISITORFIRST"

                            If Me.cbAnonymous.IsChecked = True Then

                                objDymoLabel.SetObjectText("VISITORFIRST", "Guest")
                            Else

                                objDymoLabel.SetObjectText("VISITORFIRST", Me.txtNameFirst.Content)
                            End If

                        Case "VISITING"

                            If cboVisiting.SelectedIndex > 0 Then

                                objVisiting_SelectedRow = Me.cboVisiting.SelectedItem
                                objDymoLabel.SetObjectText("VISITING", "Visiting: " & objVisiting_SelectedRow.VisitingName.ToString)

                            Else
                                objDymoLabel.SetObjectText("VISITING", "")
                            End If

                        Case "LOCATION"

                            objDymoLabel.SetObjectText("LOCATION", strLocationDescription)

                        Case "STATION"

                            objDymoLabel.SetObjectText("STATION", WinMain.objClientSettings.Station)

                    End Select

                End If

            Next

            If (TypeOf objDymoPrinter Is DYMO.Label.Framework.ILabelWriterPrinter) Then

                objDymoLabelWriterPrinter = objDymoPrinter
                objDymoLabel.Print(objDymoPrinter)

            End If

            objVisitorInfo = New VisitorInfo

            objVisitorInfo.SwipeScanID = intSwipeScanID
            objVisitorInfo.AnonymousFlag = Me.cbAnonymous.IsChecked

            If cboVisiting.SelectedIndex > 0 Then
                objVisiting_SelectedRow = Me.cboVisiting.SelectedItem
                objDymoLabel.SetObjectText("VISITING", objVisiting_SelectedRow.VisitingName.ToString)
            Else
                objVisitorInfo.Visiting = ""
            End If

            DataAccess.UpdateSwipeScanVisiting(objVisitorInfo)

            Me.DialogResult = True
            Me.Close()

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdSave_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdSave.Click

        Dim objVisitorInfo As VisitorInfo
        Dim objVisiting_SelectedRow As Visiting

        Try
            objVisitorInfo = New VisitorInfo

            objVisitorInfo.SwipeScanID = intSwipeScanID
            objVisitorInfo.AnonymousFlag = Me.cbAnonymous.IsChecked

            If cboVisiting.SelectedIndex > 0 Then
                objVisiting_SelectedRow = Me.cboVisiting.SelectedItem
                objVisitorInfo.Visiting = objVisiting_SelectedRow.VisitingName
            Else
                objVisitorInfo.Visiting = ""
            End If

            DataAccess.UpdateSwipeScanVisiting(objVisitorInfo)

            Me.DialogResult = False
            Me.Close()

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

    Private Sub WinVisitor_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Me.Closing

        Dim objVisitorInfo As VisitorInfo
        Dim objVisiting_SelectedRow As Visiting

        Try
            objVisitorInfo = New VisitorInfo

            objVisitorInfo.SwipeScanID = intSwipeScanID
            objVisitorInfo.AnonymousFlag = Me.cbAnonymous.IsChecked

            If cboVisiting.SelectedIndex > 0 Then
                objVisiting_SelectedRow = Me.cboVisiting.SelectedItem
                objVisitorInfo.Visiting = objVisiting_SelectedRow.VisitingName
            Else
                objVisitorInfo.Visiting = ""
            End If

            DataAccess.UpdateSwipeScanVisiting(objVisitorInfo)

            Me.DialogResult = False

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

End Class
