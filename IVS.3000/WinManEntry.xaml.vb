Imports IVS.Data
Imports IVS.Eseek.M280
Imports System.Drawing

Public Class WinManEntry

    Private isContentChanged As Boolean = False
    Private intSwipeScanID As Integer

    Private objDymoLabel As DYMO.Label.Framework.ILabel

    'M280
    Private WithEvents MyM280 As IVS.Eseek.M280.ESeekM280Api
    Private WithEvents timCheckStatus As New Timers.Timer
    Dim M280Status As Byte() = New Byte(M280DEF.STATUS_SIZE) {}
    Dim bInitM280 As Boolean
    Dim bSysBusy As Boolean
    Private MyImage As Bitmap

#Region "Control Bound Subs"

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Try
            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner

            If WinMain.objClientSettings.DeviceType = "M280" Then

                MyM280 = New IVS.ESeek.M280.ESeekM280Api

                Me.timCheckStatus.Interval = 100
                Me.timCheckStatus.Start()

            Else

                Me.cmdPrintBadge.Margin = New Thickness(180, 200, 0, 0)
                Me.cmdSave.Margin = New Thickness(280, 200, 0, 0)
                Me.cmdClose.Margin = New Thickness(380, 200, 0, 0)
                Me.imgScannedDocument.Visibility = Windows.Visibility.Hidden

                Me.Height = 280

            End If

            Me.cboIDType.Items.Add("-- Select an ID Type --")
            Me.cboIDType.Items.Add("Drivers License Or State ID")
            Me.cboIDType.Items.Add("Military ID Card")
            Me.cboIDType.Items.Add("Other")

            Me.cboIDType.SelectedIndex = 1
            'Me.cboIDType.IsEnabled = False

            Me.cboVisiting.ItemsSource = DataAccess.GetVisitingList(WinMain.objClientSettings.Location, WinMain.intClientID)
            Me.cboVisiting.DisplayMemberPath = ("VisitingName")
            Me.cboVisiting.SelectedValuePath = ("VisitingID")
            Me.cboVisiting.SelectedIndex = 0

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub WinManEntry_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        Try

            AddHandler Me.txtIDNumber.TextChanged, AddressOf TextChanged
            AddHandler Me.txtNameFirst.TextChanged, AddressOf TextChanged
            AddHandler Me.txtNameLast.TextChanged, AddressOf TextChanged

            Me.cmdSave.IsEnabled = False

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdSave_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdSave.Click

        Dim objSwipeScanDetail As New SwipeScanDetail
        Dim objVisitorInfo As VisitorInfo
        Dim objVisiting_SelectedRow As Visiting

        Try
            If cboIDType.SelectedIndex > 0 Then

                objSwipeScanDetail.CardType = Me.cboIDType.SelectedItem
                objSwipeScanDetail.IDAccountNumber = Me.txtIDNumber.Text.ToUpper
                objSwipeScanDetail.NameFirst = Me.txtNameFirst.Text.ToUpper
                objSwipeScanDetail.NameLast = Me.txtNameLast.Text.ToUpper
                objSwipeScanDetail.NameMiddle = ""
                objSwipeScanDetail.DateOfBirth = ""
                objSwipeScanDetail.Sex = "?"
                objSwipeScanDetail.Height = ""
                objSwipeScanDetail.Weight = ""
                objSwipeScanDetail.Eyes = ""
                objSwipeScanDetail.Hair = ""
                objSwipeScanDetail.DateOfIssue = ""
                objSwipeScanDetail.DateOfExpiration = ""
                objSwipeScanDetail.AddressStreet = ""
                objSwipeScanDetail.AddressCity = ""
                objSwipeScanDetail.AddressState = ""
                objSwipeScanDetail.AddressZip = ""
                objSwipeScanDetail.SwipeRawData = "Manual Entry"
                objSwipeScanDetail.UserID = WinMain.intIVSUserId
                objSwipeScanDetail.ClientID = WinMain.intClientID

                intSwipeScanID = DataAccess.NewDataSwipeScanManual(objSwipeScanDetail)

                objVisitorInfo = New VisitorInfo

                objVisitorInfo.SwipeScanID = intSwipeScanID
                objVisitorInfo.AnonymousFlag = Me.cbAnonymous.IsChecked

                If cboVisiting.SelectedIndex > 0 Then
                    objVisiting_SelectedRow = Me.cboVisiting.SelectedItem
                    objVisitorInfo.Visiting = objVisiting_SelectedRow.VisitingName.ToString
                End If

                DataAccess.UpdateSwipeScanVisiting(objVisitorInfo)

                If WinMain.objClientSettings.DeviceType = "M280" Then

                    SaveImage()
                    Me.timCheckStatus.Stop()
                    Me.timCheckStatus.Dispose()
                    Me.MyM280 = Nothing

                End If

                Me.DialogResult = True
                Me.Close()

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdClose_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdClose.Click
        Try

            If WinMain.objClientSettings.DeviceType = "M210/260" Then

                Me.timCheckStatus.Stop()
                Me.timCheckStatus.Dispose()
                Me.MyM280 = Nothing

            End If

            Me.DialogResult = False
            Me.Close()

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try
    End Sub

    Private Sub cmdPrintBadge_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdPrintBadge.Click

        Dim objSwipeScanDetail As New SwipeScanDetail
        Dim objVisitorInfo As VisitorInfo
        Dim objVisiting_SelectedRow As Visiting
        ' Dim intSwipeScanID As Integer
        Dim objDymoPrinter As DYMO.Label.Framework.IPrinter
        Dim objDymoLabelWriterPrinter As DYMO.Label.Framework.ILabelWriterPrinter
        Dim strDymoLabelObjectName As String
        Dim strLocationDescription As String
        ' Dim objVisiting_SelectedRow As Visiting



        Try
            If cboIDType.SelectedIndex > 0 Then

                objSwipeScanDetail.CardType = Me.cboIDType.SelectedItem
                objSwipeScanDetail.IDAccountNumber = Me.txtIDNumber.Text.ToUpper
                objSwipeScanDetail.NameFirst = Me.txtNameFirst.Text.ToUpper
                objSwipeScanDetail.NameLast = Me.txtNameLast.Text.ToUpper
                objSwipeScanDetail.NameMiddle = ""
                objSwipeScanDetail.DateOfBirth = ""
                objSwipeScanDetail.Sex = "?"
                objSwipeScanDetail.Height = ""
                objSwipeScanDetail.Weight = ""
                objSwipeScanDetail.Eyes = ""
                objSwipeScanDetail.Hair = ""
                objSwipeScanDetail.DateOfIssue = ""
                objSwipeScanDetail.DateOfExpiration = ""
                objSwipeScanDetail.AddressStreet = ""
                objSwipeScanDetail.AddressCity = ""
                objSwipeScanDetail.AddressState = ""
                objSwipeScanDetail.AddressZip = ""
                objSwipeScanDetail.SwipeRawData = "Manual Entry"
                objSwipeScanDetail.UserID = WinMain.intIVSUserId
                objSwipeScanDetail.ClientID = WinMain.intClientID

                intSwipeScanID = DataAccess.NewDataSwipeScanManual(objSwipeScanDetail)

                objVisitorInfo = New VisitorInfo

                objVisitorInfo.SwipeScanID = intSwipeScanID
                objVisitorInfo.AnonymousFlag = Me.cbAnonymous.IsChecked

                If cboVisiting.SelectedIndex > 0 Then
                    objVisiting_SelectedRow = Me.cboVisiting.SelectedItem
                    objVisitorInfo.Visiting = objVisiting_SelectedRow.VisitingName.ToString
                End If

                DataAccess.UpdateSwipeScanVisiting(objVisitorInfo)

                If WinMain.objClientSettings.DeviceType = "M210/260" Then

                    SaveImage()
                    Me.timCheckStatus.Stop()
                    Me.timCheckStatus.Dispose()
                    Me.MyM280 = Nothing

                End If

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

                                    objDymoLabel.SetObjectText("VISITORNAME", Me.txtNameFirst.Text.ToUpper & " " & Me.txtNameLast.Text.ToUpper)
                                End If

                            Case "VISITORLAST"

                                If Me.cbAnonymous.IsChecked = True Then

                                    objDymoLabel.SetObjectText("VISITORLAST", "Visitor")
                                Else

                                    objDymoLabel.SetObjectText("VISITORLAST", Me.txtNameLast.Text.ToUpper)
                                End If

                            Case "VISITORFIRST"

                                If Me.cbAnonymous.IsChecked = True Then

                                    objDymoLabel.SetObjectText("VISITORFIRST", "Guest")
                                Else

                                    objDymoLabel.SetObjectText("VISITORFIRST", Me.txtNameFirst.Text.ToUpper)
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

                            Case "QRCODE"

                                objDymoLabel.SetObjectText("QRCODE", "IVS" & intSwipeScanID)

                        End Select

                    End If

                Next

                If (TypeOf objDymoPrinter Is DYMO.Label.Framework.ILabelWriterPrinter) Then

                    objDymoLabelWriterPrinter = objDymoPrinter
                    objDymoLabel.Print(objDymoPrinter)

                End If

                Me.DialogResult = True
                Me.Close()

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

    Private Sub TextChanged()

        Try
            isContentChanged = True

            If Me.txtIDNumber.Text <> "" And Me.txtNameFirst.Text <> "" And Me.txtNameLast.Text <> "" Then
                Me.cmdSave.IsEnabled = True
                Me.cmdPrintBadge.IsEnabled = True
            Else
                Me.cmdSave.IsEnabled = False
                Me.cmdPrintBadge.IsEnabled = False
            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#Region "M280"

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
            'SaveImage()
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

#End Region

End Class