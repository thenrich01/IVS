Imports IVS.Data
Imports IVS.Eseek.M280
Imports System.Drawing

Public Class WinVisitorImage
    Private intSwipeScanID As Integer
    Private intAlertID As Integer
    Private strAlertType As String
    Private objDymoLabel As DYMO.Label.Framework.ILabel

    Private WithEvents MyM280 As IVS.ESeek.M280.ESeekM280Api
    Private WithEvents timCheckStatus As New Timers.Timer
    Dim M280Status As Byte() = New Byte(M280DEF.STATUS_SIZE) {}
    Dim bInitM280 As Boolean
    Dim bSysBusy As Boolean
    Private MyImage As Bitmap

    Public Sub New(ByVal objVisitorInfo As VisitorInfo)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Try
            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner

            MyM280 = New IVS.Eseek.M280.ESeekM280Api
            'MyM280 = WinVisitorLog.MyM280

            Me.timCheckStatus.Interval = 100
            Me.timCheckStatus.Start()

            Me.txtNameFirst.Content = objVisitorInfo.NameFirst
            Me.txtNameLast.Content = objVisitorInfo.NameLast

            intSwipeScanID = objVisitorInfo.SwipeScanID

            Me.dgAlerts.ItemsSource = DataAccess.GetSwipeScanAlerts_Anoka(objVisitorInfo.IDAccountNumber, objVisitorInfo.NameLast, objVisitorInfo.NameFirst, objVisitorInfo.DateOfBirth, WinMain.objClientSettings.Location, WinMain.intClientID, WinMain.objClientSettings.InternalLoc)

            Me.cboVisiting.ItemsSource = DataAccess.GetVisitingList_Anoka(WinMain.objClientSettings.Location, WinMain.intClientID)
            Me.cboVisiting.DisplayMemberPath = ("VisitingName")
            Me.cboVisiting.SelectedIndex = 0

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

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
                                objDymoLabel.SetObjectText("VISITING", objVisiting_SelectedRow.VisitingName.ToString)

                            Else
                                objDymoLabel.SetObjectText("VISITING", "")
                            End If

                        Case "LOCATION"

                            objDymoLabel.SetObjectText("LOCATION", strLocationDescription)

                        Case "STATION"

                            objDymoLabel.SetObjectText("STATION", WinMain.objClientSettings.Station)

                        Case "IVSCODE"

                            objDymoLabel.SetObjectText("IVSCODE", "+IVS" & intSwipeScanID & "+")

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
                objVisitorInfo.Visiting = objVisiting_SelectedRow.VisitingName
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
                strAlertType = dgAlerts_SelectedRow(0).AlertType

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub WinVisitor_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Me.Closing

        Try

            If WinMain.objClientSettings.DeviceType = "M210/260" Then

                Me.timCheckStatus.Stop()
                Me.timCheckStatus.Dispose()
                Me.MyM280 = Nothing

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub imgScannedDocument_MouseDown(sender As Object, e As System.Windows.Input.MouseButtonEventArgs) Handles imgScannedDocument.MouseDown

        Try
            If Me.bSysBusy = False Then
                MyM280.btnCapture_Click()
            End If
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub CheckState_Tick(sender As Object, e As EventArgs) Handles timCheckStatus.Elapsed

        Dim len As Integer = M280DEF.STATUS_SIZE
        Dim StatusValue As UInt16

        Try

            Me.timCheckStatus.Stop()

            If Me.bSysBusy = False Then
                MyM280.ReadCommand(M280DEF.CMD_GET_STATE, M280Status, len, 0, 0)

                StatusValue = BitConverter.ToUInt16(M280Status, 0)

                ' Check Push button
                If (StatusValue And M280DEF.stat_CapDet) = M280DEF.stat_CapDet Then
                    MyM280.btnCapture_Click()
                End If

                ' Check System Ready
                If (StatusValue And M280DEF.stat_CamInit) = M280DEF.stat_CamInit Then
                    Me.bInitM280 = True
                Else
                    Me.bInitM280 = False
                End If

                ' Check System Busy
                If (StatusValue And M280DEF.stat_SysBusy) = M280DEF.stat_SysBusy Then
                    Me.bSysBusy = True
                Else
                    Me.bSysBusy = False
                End If

            End If

            Me.timCheckStatus.Start()
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub objSerialPort_OnImageReceived(Image As Bitmap) Handles MyM280.OnImageReceived

        Try
            Me.Dispatcher.BeginInvoke(New StartTheInvoke_OnImageReceived(AddressOf NowStartTheInvoke_OnImageReceived), Image)
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Delegate Sub StartTheInvoke_OnImageReceived(ByVal Image As Bitmap)

    Friend Sub NowStartTheInvoke_OnImageReceived(ByVal Image As Bitmap)

        Try
            Me.imgScannedDocument.Source = MyM280.GetBitmapSource(Image)
            MyImage = Image
            SaveImage()
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub SaveImage()

        Dim objImageDetail As New ImageDetail

        Try

            If My.Computer.FileSystem.DirectoryExists(WinMain.objClientSettings.ImageLocation) = False Then

                My.Computer.FileSystem.CreateDirectory(WinMain.objClientSettings.ImageLocation)

            End If

            objImageDetail.Image = MyImage
            objImageDetail.FileName = WinMain.objClientSettings.ImageLocation & "\" & intSwipeScanID & "f.jpg"
            ESeekM280Api.SaveImage(objImageDetail)
            DataAccess.UpdateImageAvailable(intSwipeScanID)
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdDeny_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdDeny.Click

        Try
           
            DataAccess.DenyEntry(intSwipeScanID)

            Me.DialogResult = False
            Me.Close()

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

End Class
