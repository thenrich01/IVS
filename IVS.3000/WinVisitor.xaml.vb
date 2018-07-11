Imports IVS.Data

Public Class WinVisitor

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

            WinMain.MyAppLog.WriteToLog("IVS", "PrintBadge_Click()", EventLogEntryType.Information, 1)
            objDymoPrinter = DYMO.Label.Framework.Framework.GetPrinters()(WinMain.objClientSettings.DymoPrinter)
            objDymoLabel = DYMO.Label.Framework.Framework.Open(WinMain.objClientSettings.DymoLabel)
            strLocationDescription = DataAccess.GetLocation(WinMain.objClientSettings.Location)

            WinMain.MyAppLog.WriteToLog("IVS", "Setting Dymo Tokens.", EventLogEntryType.Information, 1)

            For Each strDymoLabelObjectName In objDymoLabel.ObjectNames

                If Not String.IsNullOrEmpty(strDymoLabelObjectName) Then

                    Select Case strDymoLabelObjectName

                        Case "VISITORNAME"

                            If Me.cbAnonymous.IsChecked = True Then

                                objDymoLabel.SetObjectText("VISITORNAME", "Guest Visitor")
                            Else

                                objDymoLabel.SetObjectText("VISITORNAME", Me.txtNameFirst.Content & " " & Me.txtNameLast.Content)
                            End If

                            WinMain.MyAppLog.WriteToLog("IVS", "VISITORNAME:" & Me.txtNameFirst.Content & " " & Me.txtNameLast.Content, EventLogEntryType.Information, 1)

                        Case "VISITORLAST"

                            If Me.cbAnonymous.IsChecked = True Then

                                objDymoLabel.SetObjectText("VISITORLAST", "Visitor")
                            Else

                                objDymoLabel.SetObjectText("VISITORLAST", Me.txtNameLast.Content)
                            End If

                            WinMain.MyAppLog.WriteToLog("IVS", "VISITORLAST:" & Me.txtNameLast.Content, EventLogEntryType.Information, 1)

                        Case "VISITORFIRST"

                            If Me.cbAnonymous.IsChecked = True Then

                                objDymoLabel.SetObjectText("VISITORFIRST", "Guest")
                            Else

                                objDymoLabel.SetObjectText("VISITORFIRST", Me.txtNameFirst.Content)
                            End If

                            WinMain.MyAppLog.WriteToLog("IVS", "VISITORFIRST:" & Me.txtNameLast.Content, EventLogEntryType.Information, 1)

                        Case "VISITING"

                            If cboVisiting.SelectedIndex > 0 Then

                                objVisiting_SelectedRow = Me.cboVisiting.SelectedItem
                                objDymoLabel.SetObjectText("VISITING", "Visiting: " & objVisiting_SelectedRow.VisitingName.ToString)
                                WinMain.MyAppLog.WriteToLog("IVS", "VISITING:" & objVisiting_SelectedRow.VisitingName.ToString, EventLogEntryType.Information, 1)

                            Else
                                objDymoLabel.SetObjectText("VISITING", "")
                            End If

                        Case "LOCATION"

                            objDymoLabel.SetObjectText("LOCATION", strLocationDescription)
                            WinMain.MyAppLog.WriteToLog("IVS", "LOCATION:" & strLocationDescription, EventLogEntryType.Information, 1)

                        Case "STATION"

                            objDymoLabel.SetObjectText("STATION", WinMain.objClientSettings.Station)
                            WinMain.MyAppLog.WriteToLog("IVS", "STATION:" & WinMain.objClientSettings.Station, EventLogEntryType.Information, 1)

                        Case "IVSCODE"

                            objDymoLabel.SetObjectText("IVSCODE", "+IVS" & intSwipeScanID & "+")
                            WinMain.MyAppLog.WriteToLog("IVS", "IVSCODE:" & "+IVS" & intSwipeScanID & "+", EventLogEntryType.Information, 1)

                    End Select

                End If

            Next

            If (TypeOf objDymoPrinter Is DYMO.Label.Framework.ILabelWriterPrinter) Then

                Dim TimeProcessStart As Date
                Dim TimeProcessEnd As Date
                Dim timeProcessDuration As TimeSpan

                WinMain.MyAppLog.WriteToLog("IVS", "Printing via Dymo framework.", EventLogEntryType.Information, 1)
                TimeProcessStart = DateTime.Now

                objDymoLabelWriterPrinter = objDymoPrinter
                objDymoLabel.Print(objDymoPrinter)


                TimeProcessEnd = DateTime.Now
                timeProcessDuration = TimeProcessEnd.Subtract(TimeProcessStart)

                WinMain.MyAppLog.WriteToLog("IVS", String.Format("Done printing - Duration: {0} MS", timeProcessDuration.Milliseconds), EventLogEntryType.Information, 1)

            End If

            objVisitorInfo = New VisitorInfo

            objVisitorInfo.SwipeScanID = intSwipeScanID
            objVisitorInfo.AnonymousFlag = Me.cbAnonymous.IsChecked

            If cboVisiting.SelectedIndex > 0 Then
                objVisiting_SelectedRow = Me.cboVisiting.SelectedItem
                objVisitorInfo.Visiting = objVisiting_SelectedRow.VisitingName
            Else
                objVisitorInfo.Visiting = ""
            End If


            WinMain.MyAppLog.WriteToLog("IVS", "Begin updating SQL Server with updated visitor info", EventLogEntryType.Information, 1)
            DataAccess.UpdateSwipeScanVisiting(objVisitorInfo)

            WinMain.MyAppLog.WriteToLog("IVS", "End Updating SQL Server with updated visitor info", EventLogEntryType.Information, 1)
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

            WinMain.MyAppLog.WriteToLog("IVS", "Begin updating SQL Server with updated visitor info", EventLogEntryType.Information, 1)
            DataAccess.UpdateSwipeScanVisiting(objVisitorInfo)
            WinMain.MyAppLog.WriteToLog("IVS", "End Updating SQL Server with updated visitor info", EventLogEntryType.Information, 1)

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

    Private Sub cmdDeny_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdDeny.Click

        Try
            WinMain.MyAppLog.WriteToLog("IVS", "Begin updating SQL Server with updated visitor info", EventLogEntryType.Information, 1)
            DataAccess.DenyEntry(intSwipeScanID)
            WinMain.MyAppLog.WriteToLog("IVS", "End Updating SQL Server with updated visitor info", EventLogEntryType.Information, 1)

            Me.DialogResult = False
            Me.Close()

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

End Class
