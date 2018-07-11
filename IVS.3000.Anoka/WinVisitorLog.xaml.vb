Imports IVS.MagTek
Imports IVS.Eseek
Imports IVS.Data
Imports System.Threading
Imports System.ComponentModel
Imports System.Windows.Threading
Imports IVS.CTS
Imports System.Text
Imports LsFamily
Imports IVS.Eseek.M280
Imports System.Drawing
Imports IVS.Honeywell

Public Class WinVisitorLog

    'FTP
    Private WithEvents BWFTP As New BackgroundWorker

    'ESeek
    Private WithEvents objSerialPort As ESeekApi
    Private WithEvents BWSerialPort As New BackgroundWorker

    'Honeywell
    Private WithEvents objHWSerialPort As HoneywellApi
    'Public Shared WithEvents MyM280 As IVS.ESeek.M280.ESeekM280Api
    'Private WithEvents timCheckStatus As New Timers.Timer
    'Dim M280Status As Byte() = New Byte(M280DEF.STATUS_SIZE) {}
    'Dim bInitM280 As Boolean
    'Dim bSysBusy As Boolean
    'Public MyImage As Bitmap
    'Public IsImageReady As Boolean

    'MagTek
    Private DelegateHandlerCardDataStateChange As MagTekApi.CallBackCardDataStateChanged
    'CTS
    Private CTSDeviceType As LsDefines.LsUnitType

    Private WithEvents timerDateTime As DispatcherTimer = New DispatcherTimer()
    Private intSwipeScanID As Integer
    Private strIDAccount As String
    Private strNameLast As String
    Private strNameFirst As String
    Private strDateOfBirth As String
    Private isAnonymous As Boolean
    Private strVisiting As String
    Private objDymoLabel As DYMO.Label.Framework.ILabel


    Dim winNewVisitor

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

    Private Sub WinVisitorLog_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Me.Closing

        Try

            Select Case WinMain.objClientSettings.DeviceType

                Case "M200/250"

                    If objSerialPort IsNot Nothing Then
                        objSerialPort.SerialPortClose()
                    End If

                Case "M210/260"

                    If objSerialPort IsNot Nothing Then
                        objSerialPort.SerialPortClose()
                    End If

                Case "MAGTEK"

                    Dim intResult As Integer

                    MagTekApi.MTUSCRAClearBuffer()

                    Dim DelegateDataState As [Delegate] = TryCast(DelegateHandlerCardDataStateChange, [Delegate])
                    DelegateHandlerCardDataStateChange = TryCast([Delegate].RemoveAll(DelegateDataState, DelegateDataState), MagTekApi.CallBackCardDataStateChanged)

                    intResult = MagTekApi.MTUSCRACloseDevice()


                Case "HW3310"

                    If objSerialPort IsNot Nothing Then
                        objHWSerialPort.SerialPortClose()
                    End If

                Case "CTS"

            End Select

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub WinVisitorLog_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        Try

            Dim objDeviceInfo As New DeviceInfo
            objDeviceInfo = DataAccess.GetDeviceInfo(WinMain.objClientSettings.ClientID)

            Select Case objDeviceInfo.DeviceType

                Case "M250"

                    Dim isSerialPortOpen As Boolean = False

                    objSerialPort = New ESeekApi(WinMain.objClientSettings.ComPort, WinMain.objClientSettings.SleepMilliSeconds)
                    isSerialPortOpen = objSerialPort.SerialPortOpen()

                    If isSerialPortOpen = False Then
                        MessageBox.Show("Unable to connect to ESeek reader.  Ensure reader is properly connected and open window again.", "IVS", MessageBoxButton.OK)
                    End If

                Case "M260"

                    Dim isSerialPortOpen As Boolean = False

                    objSerialPort = New ESeekApi(WinMain.objClientSettings.ComPort, WinMain.objClientSettings.SleepMilliSeconds)
                    isSerialPortOpen = objSerialPort.SerialPortOpen()

                    If isSerialPortOpen = False Then
                        MessageBox.Show("Unable to connect to ESeek reader.  Ensure reader is properly connected and open window again.", "IVS", MessageBoxButton.OK)
                    End If

                Case "M280"

                    Dim isSerialPortOpen As Boolean = False

                    objSerialPort = New ESeekApi(WinMain.objClientSettings.ComPort, WinMain.objClientSettings.SleepMilliSeconds)
                    isSerialPortOpen = objSerialPort.SerialPortOpen()

                    If isSerialPortOpen = False Then
                        MessageBox.Show("Unable to connect to ESeek reader.  Ensure reader is properly connected and open window again.", "IVS", MessageBoxButton.OK)
                    End If

                Case "MAGTEK"

                    Dim intResult As Integer

                    intResult = MagTekApi.MTUSCRAOpenDevice("")

                    If intResult <> MagTekApi.MTSCRA_ST_OK Then
                        MessageBox.Show("Unable to connect to MagTek USB reader.  Ensure reader is properly connected and open window again.", "IVS", MessageBoxButton.OK)
                    End If

                    DelegateHandlerCardDataStateChange = New MagTekApi.CallBackCardDataStateChanged(AddressOf objUSBMagTek_OnDataReceived)
                    MagTekApi.MTUSCRACardDataStateChangedNotify(DelegateHandlerCardDataStateChange)

                Case "HW3310"

                    Dim isSerialPortOpen As Boolean = False

                    objHWSerialPort = New HoneywellApi(WinMain.objClientSettings.ComPort)
                    isSerialPortOpen = objHWSerialPort.SerialPortOpen()

                    If isSerialPortOpen = False Then
                        MessageBox.Show("Unable to connect to Honeywell reader.  Ensure reader is properly connected and open window again.", "IVS", MessageBoxButton.OK)
                    End If

                Case "LS_40_USB"

                    CTSDeviceType = LsDefines.LsUnitType.LS_40_USB

                    Me.cmdCTSScanID.Visibility = Windows.Visibility.Visible
                    Me.cmdCTSReadMagStripe.Visibility = Windows.Visibility.Visible

                Case "LS_150_USB"

                    CTSDeviceType = LsDefines.LsUnitType.LS_150_USB

                    Me.cmdCTSScanID.Visibility = Windows.Visibility.Visible
                    Me.cmdCTSReadMagStripe.Visibility = Windows.Visibility.Visible

            End Select


            Me.lblToday.Content = Today.ToLongDateString & " " & Now.ToShortTimeString

            timerDateTime.IsEnabled = True
            timerDateTime.Interval = TimeSpan.FromSeconds(1)
            timerDateTime.Start()

            Visitors_Load()

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdManualEntry_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdManualEntry.Click

        Dim winManualEntry As WinManEntry

        Try

            winManualEntry = New WinManEntry()
            winManualEntry.Owner = Me
            winManualEntry.ShowDialog()

            Visitors_Load()

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try
    End Sub

    Private Sub cmdVisitorsToday_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdVisitorsToday.Click

        Dim WinReportVisitorsToday As WinReportVisitorsToday

        Try

            WinReportVisitorsToday = New WinReportVisitorsToday()
            WinReportVisitorsToday.Owner = Me

            WinReportVisitorsToday.ShowDialog()

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdCurrentVisitors_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCurrentVisitors.Click

        Dim WinReportCurrentVisitors As WinReportCurrentVisitors

        Try

            WinReportCurrentVisitors = New WinReportCurrentVisitors()
            WinReportCurrentVisitors.Owner = Me

            WinReportCurrentVisitors.ShowDialog()

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdNewAlert_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdNewAlert.Click

        Dim WinAlert As WinAlert
        Dim DialogResult As Boolean
        Dim objAlertDetail As New AlertDetail

        Try

            objAlertDetail.AlertID = 0
            objAlertDetail.AlertContactName = WinMain.strIVSUserName
            objAlertDetail.AlertContactNumber = WinMain.objUserDetail.UserPhone
            objAlertDetail.UserID = WinMain.intIVSUserId

            WinAlert = New WinAlert(objAlertDetail)
            WinAlert.Owner = Me

            DialogResult = WinAlert.ShowDialog

            If DialogResult = True Then
                Visitors_Load()
            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdCancel_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCancel.Click
        Me.Close()
    End Sub

    Private Sub dgVisitorLog_SelectionChanged(sender As Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgVisitorLog.SelectionChanged

        Dim dgvVisitorLog_SelectedRow As System.Collections.IList

        Try
            strIDAccount = Nothing
            strNameFirst = Nothing
            strNameLast = Nothing
            strDateOfBirth = Nothing

            dgvVisitorLog_SelectedRow = e.AddedItems

            If dgvVisitorLog_SelectedRow.Count > 0 Then

                intSwipeScanID = dgvVisitorLog_SelectedRow(0).SwipeScanID
                strIDAccount = dgvVisitorLog_SelectedRow(0).IDAccount
                strNameLast = dgvVisitorLog_SelectedRow(0).NameLast
                strNameFirst = dgvVisitorLog_SelectedRow(0).NameFirst
                strDateOfBirth = dgvVisitorLog_SelectedRow(0).DateOfBirth
                If IsDBNull(dgvVisitorLog_SelectedRow(0).Visiting) = False Then
                    strVisiting = dgvVisitorLog_SelectedRow(0).Visiting
                Else
                    strVisiting = Nothing
                End If

                isAnonymous = dgvVisitorLog_SelectedRow(0).AnonymousFlag
            End If

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

    Private Sub dgVisitorLog_CmdViewAlerts_Click()

        Dim winAlerts As WinAlerts
        Dim DialogResult As Boolean

        Try
            winAlerts = New WinAlerts(strIDAccount, strNameLast, strNameFirst, strDateOfBirth)
            winAlerts.Owner = Me

            DialogResult = winAlerts.ShowDialog

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgVisitorLog_CmdVisitorCheckOut_Click()

        Try
            DataAccess.SwipeScanCheckOut(intSwipeScanID)
            Visitors_Load()

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgVisitorLog_CmdVisitorPrintBadge_Click()

        Dim objDymoPrinter As DYMO.Label.Framework.IPrinter
        Dim objDymoLabelWriterPrinter As DYMO.Label.Framework.ILabelWriterPrinter
        Dim strDymoLabelObjectName As String
        Dim strLocationDescription As String

        Try
            WinMain.MyAppLog.WriteToLog("IVS", "dgVisitorLog_CmdVisitorPrintBadge_Click()", EventLogEntryType.Information, 1)
            objDymoPrinter = DYMO.Label.Framework.Framework.GetPrinters()(WinMain.objClientSettings.DymoPrinter)
            objDymoLabel = DYMO.Label.Framework.Framework.Open(WinMain.objClientSettings.DymoLabel)
            strLocationDescription = DataAccess.GetLocation(WinMain.objClientSettings.Location)

            WinMain.MyAppLog.WriteToLog("IVS", "Setting Dymo Tokens.", EventLogEntryType.Information, 1)
            For Each strDymoLabelObjectName In objDymoLabel.ObjectNames

                If Not String.IsNullOrEmpty(strDymoLabelObjectName) Then

                    Select Case strDymoLabelObjectName

                        Case "VISITORNAME"

                            If isAnonymous = True Then

                                objDymoLabel.SetObjectText("VISITORNAME", "Guest Visitor")
                            Else

                                objDymoLabel.SetObjectText("VISITORNAME", strNameFirst & " " & strNameLast)
                            End If

                            WinMain.MyAppLog.WriteToLog("IVS", "VISITORNAME:" & strNameFirst & " " & strNameLast, EventLogEntryType.Information, 1)

                        Case "VISITORLAST"

                            If isAnonymous = True Then

                                objDymoLabel.SetObjectText("VISITORLAST", "Visitor")
                            Else

                                objDymoLabel.SetObjectText("VISITORLAST", strNameLast)
                            End If

                            WinMain.MyAppLog.WriteToLog("IVS", "VISITORLAST:" & strNameLast, EventLogEntryType.Information, 1)

                        Case "VISITORFIRST"

                            If isAnonymous = True Then

                                objDymoLabel.SetObjectText("VISITORFIRST", "Guest")
                            Else

                                objDymoLabel.SetObjectText("VISITORFIRST", strNameFirst)
                            End If

                            WinMain.MyAppLog.WriteToLog("IVS", "VISITORFIRST:" & strNameFirst, EventLogEntryType.Information, 1)

                        Case "VISITING"

                            If strVisiting = Nothing Then

                                objDymoLabel.SetObjectText("VISITING", "")

                            End If
                            If strVisiting <> Nothing Then

                                objDymoLabel.SetObjectText("VISITING", "Visiting: " & strVisiting)
                                WinMain.MyAppLog.WriteToLog("IVS", "VISITING:" & strVisiting, EventLogEntryType.Information, 1)

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

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgVisitorLog_CmdVisitorNewAlert_Click()

        Dim WinNewAlert As WinAlert
        Dim DialogResult As Boolean
        Dim objAlertDetail As New AlertDetail

        Try

            objAlertDetail.AlertID = 0
            objAlertDetail.IDNumber = strIDAccount
            objAlertDetail.NameFirst = strNameFirst
            objAlertDetail.NameLast = strNameLast
            objAlertDetail.DateOfBirth = strDateOfBirth
            objAlertDetail.AlertContactName = WinMain.strIVSUserName
            objAlertDetail.AlertContactNumber = WinMain.objUserDetail.UserPhone
            objAlertDetail.UserID = WinMain.intIVSUserId

            WinNewAlert = New WinAlert(objAlertDetail)
            WinNewAlert.Owner = Me

            DialogResult = WinNewAlert.ShowDialog

            If DialogResult = True Then

                Visitors_Load()

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgVisitorLog_CmdVisitorEditVisiting_Click()

        Dim WinEditVisiting As WinEditVisiting
        Dim DialogResult As Boolean
        Dim objVisitorInfo As New VisitorInfo

        Try

            objVisitorInfo.SwipeScanID = intSwipeScanID
            objVisitorInfo.NameFirst = strNameFirst
            objVisitorInfo.NameLast = strNameLast
            objVisitorInfo.AnonymousFlag = isAnonymous
            objVisitorInfo.Visiting = strVisiting

            WinEditVisiting = New WinEditVisiting(objVisitorInfo)
            WinEditVisiting.Owner = Me

            DialogResult = WinEditVisiting.ShowDialog

            If DialogResult = True Then

                Visitors_Load()

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub timerDateTime_Tick(sender As Object, e As System.EventArgs) Handles timerDateTime.Tick

        Try
            If DateTime.Now.Second = 0 Then
                Me.lblToday.Content = Today.ToLongDateString & " " & Now.ToShortTimeString
                Visitors_Load()
                If DateTime.Now.Minute = 30 Then
                    'Upload log files

                    If My.Settings.ExportToExternalServer = 1 Then
                        BWFTP.RunWorkerAsync()
                    End If

                End If
            End If
            Me.txtReaderInput.Focus()

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cbDisplayAllVisitors_Checked(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cbDisplayAllVisitors.Checked
        Visitors_Load()
    End Sub

    Private Sub cbDisplayAllVisitors_Unchecked(sender As Object, e As System.Windows.RoutedEventArgs) Handles cbDisplayAllVisitors.Unchecked
        Visitors_Load()
    End Sub

    Private Sub txtReaderInput_TextChanged(sender As Object, e As System.Windows.Controls.TextChangedEventArgs) Handles txtReaderInput.TextChanged

        Dim CharCount As Integer = 0
        Dim strTrackData As String = txtReaderInput.Text

        Try

            For Each c As Char In strTrackData

                If c = "+" Then CharCount += 1

            Next

            If CharCount > 1 Then

                DataAccess.SwipeScanCheckOut(strTrackData.Replace("IVS", "").Replace("+", ""))
                Visitors_Load()
                Me.txtReaderInput.Text = Nothing

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#Region "Custom Subs/Functions"

    Private Sub Visitors_Load()

        Dim sList As List(Of VisitorLog)

        Try
            sList = DataAccess.GetVisitorLog(WinMain.objClientSettings.Location, WinMain.intClientID)

            If cbDisplayAllVisitors.IsChecked = True Then

                ' sList = sList.FindAll
            Else
                sList = sList.FindAll(Function(s As VisitorLog) s.CanCheckOut.Equals(True))
            End If

            Me.dgVisitorLog.ItemsSource = sList
            Me.lblVisitorsToday.Content = DataAccess.GetVisitorsTodayCount(WinMain.intClientID)
            Me.lblVisitorsCurrent.Content = DataAccess.GetVisitorsCurrentCount(WinMain.intClientID)

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

#Region "ESeek"

    Private Sub BWSerialPort_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles BWSerialPort.DoWork

        Dim bw As BackgroundWorker

        Try
            bw = CType(sender, BackgroundWorker)
            e.Result = TimeConsumingOperation_NewSwipeIDDL(e.Argument, bw)

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWSerialPort_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles BWSerialPort.RunWorkerCompleted

        If (e.Error IsNot Nothing) Then

            WinMain.MyAppLog.WriteToLog("WinVisitorLog.BWSerialPort_RunWorkerCompleted()" & e.Error.ToString)

        Else

            Try
                If e.Result.isCheckingIn = True Then

                    'Dim winNewVisitor

                    Select Case WinMain.objClientSettings.DeviceType

                        Case "M280"
                            winNewVisitor = New WinVisitorImage(e.Result)
                        Case Else
                            winNewVisitor = New WinVisitor(e.Result)
                            'winNewVisitor = New Wintest(e.Result)
                    End Select

                    winNewVisitor.Owner = Me
                    winNewVisitor.ShowDialog()

                End If

                Visitors_Load()

            Catch ex As Exception
                WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
            End Try

        End If

    End Sub

    Private Function TimeConsumingOperation_NewSwipeIDDL(ByVal RawSerialPortData As String, ByVal bw As BackgroundWorker) As VisitorInfo

        Dim objVisitorInfo As New VisitorInfo
        Dim objSwipeScanInfo As New SwipeScanInfo

        Try
            objSwipeScanInfo.ClientID = WinMain.intClientID
            objSwipeScanInfo.UserID = WinMain.intIVSUserId
            objSwipeScanInfo.SwipeScanRawData = RawSerialPortData
            objSwipeScanInfo.DisableDBSave = WinMain.objClientSettings.DisableDBSave
            objSwipeScanInfo.IDChecker = Application.objMyIDChecker

            objVisitorInfo = DataAccess.NewDataSwipeScan(objSwipeScanInfo)

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        Return objVisitorInfo

    End Function

    Private Sub objSerialPort_OnDataReceived(data As String) Handles objSerialPort.OnDataReceived, objHWSerialPort.OnDataReceived

        Try
            Me.Dispatcher.BeginInvoke(New StartTheInvoke_OnDataReceived(AddressOf NowStartTheInvoke_OnDataReceived), data)
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Delegate Sub StartTheInvoke_OnDataReceived(ByVal StrRawSerialPortData As String)

    Friend Sub NowStartTheInvoke_OnDataReceived(ByVal StrRawSerialPortData As String)

        Dim strSwipeTrackData() As String

        Try

            If StrRawSerialPortData.Substring(0, 1) = "@" Or StrRawSerialPortData.Substring(0, 1) = "t" Or StrRawSerialPortData.Substring(0, 1) = "%" Then

                BWSerialPort.RunWorkerAsync(StrRawSerialPortData)

            ElseIf StrRawSerialPortData.Substring(0, 4) = "+IVS" Then

                DataAccess.SwipeScanCheckOut(StrRawSerialPortData.Replace("IVS", "").Replace("+", ""))
                Visitors_Load()

            Else

                strSwipeTrackData = StrRawSerialPortData.Split("?")

                If strSwipeTrackData.Count < 4 Then
                    System.Windows.MessageBox.Show("Unable to read all data on the card.", "Unable to Read Card Data", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Else
                    BWSerialPort.RunWorkerAsync(StrRawSerialPortData)
                End If

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

#Region "MagTek"

    Private Sub objUSBMagTek_OnDataReceived(CardDataState As Integer)

        Dim lResult As UInteger
        Dim structMTMSRDATA As New MagTekApi.MTMSRDATA

        Try
            If CardDataState = MagTekApi.MTSCRA_DATA_READY Then

                lResult = MagTekApi.MTUSCRAGetCardData(structMTMSRDATA)
                MagTekApi.MTUSCRAClearBuffer()

                Me.Dispatcher.BeginInvoke(New StartTheInvoke_OnDataReceived(AddressOf NowStartTheInvoke_OnDataReceived), structMTMSRDATA.m_szCardData)

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

#Region "CTS"

    Private Sub cmdCTSScanID_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCTSScanID.Click

        Dim objCTSApi As New CTSApi

        Try
            AddHandler objCTSApi.BWProgressChanged, AddressOf BWScanDLIDImage_ProgressChanged
            AddHandler objCTSApi.BWCompleted, AddressOf BWScanDLIDImage_RunWorkerCompleted

            objCTSApi.BWScanDLIDImage_Start(CTSDeviceType)

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdCTSReadMagStripe_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCTSReadMagStripe.Click

        Dim objCTSApi As New CTSApi

        Try
            AddHandler objCTSApi.BWProgressChanged, AddressOf BWReadMagData_ProgressChanged
            AddHandler objCTSApi.BWCompleted, AddressOf BWReadMagData_RunWorkerCompleted

            objCTSApi.BWReadMagStripe_Start(CTSDeviceType)

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Delegate Sub StartTheInvoke_ReportStatus(ByVal Status As String)

    Friend Sub NowStartTheInvoke_ReportStatus(ByVal Status As String)

        Try
            ' Me.lblStatusCTS.Content = Status

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Delegate Sub StartTheInvoke_UpdateProgressBar(ByVal Progress As Integer)

    Friend Sub NowStartTheInvoke_UpdateProgressBar(ByVal Progress As Integer)

        Try
            'Me.ProgressBarCTS.Visibility = Windows.Visibility.Visible
            'Me.ProgressBarCTS.Value = Progress

            If Progress = 0 Then

                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait
                'ClearData()

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWScanDLIDImage_ProgressChanged(ByVal PercentChange As Integer)

        Try
            ' WinMain.MyAppLog.WriteToLog("WinVisitorLog.BWScanDLIDImage_ProgressChanged() ProgressPercentage:" & PercentChange)
            'ProgressBarCTS.Dispatcher.BeginInvoke(New StartTheInvoke_UpdateProgressBar(AddressOf NowStartTheInvoke_UpdateProgressBar), PercentChange)

            Select Case PercentChange
                'Case 0
                '    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Connecting to CTS device")
                'Case 5
                '    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Checking CTS device document feeder")
                'Case 10
                '    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Document found in feeder")
                'Case 11
                '    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "No document detected in Feeder")
                'Case 15
                '    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Scanning identification card")
                'Case 20
                '    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Retrieving scanned ID images")
                'Case 30
                '    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Reading barcode on front of ID")
                'Case 50
                '    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Reading barcode on back of ID")
                'Case 60
                '    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Decoding barcode")
                'Case 70
                '    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Recording information in database")
                'Case 80
                '    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Saving front ID image")
                'Case 90
                '    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Saving back ID image")
                'Case 100
                '    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Finished")
            End Select

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWScanDLIDImage_RunWorkerCompleted(ByVal Result As CTSSwipeScanInfo)

        'WinMain.MyAppLog.WriteToLog("WinVisitorLog.BWScanDLIDImage_RunWorkerCompleted()")

        Dim objSwipeScanInfo As New SwipeScanInfo
        Dim objSwipeScanDetail As New SwipeScanDetail

        Try


        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWReadMagData_ProgressChanged(ByVal PercentChange As Integer)

        Try
            ' MyAppLog.WriteToLog("WinVisitorLog.BWReadMagData_ProgressChanged() ProgressPercentage:" & PercentChange)
            'ProgressBarCTS.Dispatcher.BeginInvoke(New StartTheInvoke_UpdateProgressBar(AddressOf NowStartTheInvoke_UpdateProgressBar), PercentChange)

            'Select Case PercentChange
            '    Case 0
            '        lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Waiting for card swipe")
            '    Case 10
            '        lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Waiting for card swipe")
            '    Case 60
            '        lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Reading magnetic data")
            '    Case 80
            '        lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Recording swipe information in database")
            '    Case 100
            '        lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Finished")

            'End Select

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWReadMagData_RunWorkerCompleted(ByVal Result As CTSSwipeScanInfo)

        'MyAppLog.WriteToLog("WinVisitorLog.BWReadMagData_RunWorkerCompleted()")

        Dim objSwipeScanInfo As New SwipeScanInfo
        Dim objSwipeScanDetail As New SwipeScanDetail

        Try
            'Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow

            'Me.ProgressBarCTS.Visibility = Windows.Visibility.Hidden

            'If Result.LSReadBadgeWithTimeout <> 0 Then
            '    Me.lblStatusCTS.Content = "Error occurred while scanning"
            'Else

            '    Me.lblStatusCTS.Content = "Succesful scan - Ready to scan again"

            '    objSwipeScanInfo.ClientID = objClientSettings.ClientID
            '    objSwipeScanInfo.UserID = intIVSUserID
            '    objSwipeScanInfo.DisableDBSave = objClientSettings.DisableDBSave
            '    objSwipeScanInfo.IDChecker = Application.objMyIDChecker
            '    objSwipeScanInfo.SwipeScanRawData = Result.SwipeRawData

            '    objSwipeScanDetail = DataAccess.NewDataSwipeScan(objSwipeScanInfo)

            '    SwipeScanDetail_LoadByObject(objSwipeScanDetail, Result.ImageFront, Result.ImageBack)

            '    intViewTimeCountDown = objClientSettings.ViewingTime
            '    isViewTimeCountingDown = True
            'End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

#Region "FTP"

    Private Sub BWFTP_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BWFTP.DoWork

        Try
            'Upload error files to external server
            DataAccess.ExportLogFilesToExternalServer(My.Settings.ExternalURL)
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWFTP_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BWFTP.RunWorkerCompleted

        If e.Error IsNot Nothing Then
            WinMain.MyAppLog.WriteToLog("IVS", "BWFTP_RunWorkerCompleted()", EventLogEntryType.Information, 0)
        Else
            WinMain.MyAppLog.WriteToLog("IVS", e.Error.ToString, EventLogEntryType.Error)
        End If

    End Sub

#End Region

#Region "DEVICES"

    Private Sub cmdDevices_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdDevices.Click

        Dim WinDevices As WinDevices
        Dim DialogResult As Boolean

        Try

            Dim objDeviceInfo As New DeviceInfo
            objDeviceInfo = DataAccess.GetDeviceInfo(WinMain.objClientSettings.ClientID)
            MessageBox.Show(objDeviceInfo.DeviceType)

            Select Case objDeviceInfo.DeviceType

                Case "M200/250"

                    If objSerialPort IsNot Nothing Then
                        objSerialPort.SerialPortClose()
                    End If

                Case "M210/260"

                    If objSerialPort IsNot Nothing Then
                        objSerialPort.SerialPortClose()
                    End If

                Case "MAGTEK"

                    Dim intResult As Integer

                    MagTekApi.MTUSCRAClearBuffer()

                    Dim DelegateDataState As [Delegate] = TryCast(DelegateHandlerCardDataStateChange, [Delegate])
                    DelegateHandlerCardDataStateChange = TryCast([Delegate].RemoveAll(DelegateDataState, DelegateDataState), MagTekApi.CallBackCardDataStateChanged)

                    intResult = MagTekApi.MTUSCRACloseDevice()
                    MessageBox.Show(intResult)
                Case "HW3310"

                    If objSerialPort IsNot Nothing Then
                        objHWSerialPort.SerialPortClose()
                    End If

                Case "CTS"

            End Select

            WinDevices = New WinDevices()
            WinDevices.Owner = Me

            DialogResult = WinDevices.ShowDialog

            MessageBox.Show("HI")
            ' If DialogResult = True Then

            ' Dim objDeviceInfo As New DeviceInfo
            'objDeviceInfo = DataAccess.GetDeviceInfo(WinMain.objClientSettings.ClientID)
            WinMain.objClientSettings = DataAccess.GetClientSettings(WinMain.objClientSettings.ClientID)


            MessageBox.Show(WinMain.objClientSettings.DeviceType)

            Select Case WinMain.objClientSettings.DeviceType

                Case "M250"

                    Dim isSerialPortOpen As Boolean = False

                    objSerialPort = New ESeekApi(WinMain.objClientSettings.ComPort, WinMain.objClientSettings.SleepMilliSeconds)
                    isSerialPortOpen = objSerialPort.SerialPortOpen()

                    If isSerialPortOpen = False Then
                        MessageBox.Show("Unable to connect to ESeek reader.  Ensure reader is properly connected and open window again.", "IVS", MessageBoxButton.OK)
                    End If

                Case "M260"

                    Dim isSerialPortOpen As Boolean = False

                    objSerialPort = New ESeekApi(WinMain.objClientSettings.ComPort, WinMain.objClientSettings.SleepMilliSeconds)
                    isSerialPortOpen = objSerialPort.SerialPortOpen()

                    If isSerialPortOpen = False Then
                        MessageBox.Show("Unable to connect to ESeek reader.  Ensure reader is properly connected and open window again.", "IVS", MessageBoxButton.OK)
                    End If

                Case "M280"

                    Dim isSerialPortOpen As Boolean = False

                    objSerialPort = New ESeekApi(WinMain.objClientSettings.ComPort, WinMain.objClientSettings.SleepMilliSeconds)
                    isSerialPortOpen = objSerialPort.SerialPortOpen()

                    If isSerialPortOpen = False Then
                        MessageBox.Show("Unable to connect to ESeek reader.  Ensure reader is properly connected and open window again.", "IVS", MessageBoxButton.OK)
                    End If

                Case "MAGTEK"

                    Dim intResult As Integer

                    intResult = MagTekApi.MTUSCRAOpenDevice("")

                    If intResult <> MagTekApi.MTSCRA_ST_OK Then
                        MessageBox.Show("Unable to connect to MagTek USB reader.  Ensure reader is properly connected and open window again.", "IVS", MessageBoxButton.OK)
                    End If

                    DelegateHandlerCardDataStateChange = New MagTekApi.CallBackCardDataStateChanged(AddressOf objUSBMagTek_OnDataReceived)
                    MagTekApi.MTUSCRACardDataStateChangedNotify(DelegateHandlerCardDataStateChange)

                Case "HW3310"

                    Dim isSerialPortOpen As Boolean = False

                    objHWSerialPort = New HoneywellApi(WinMain.objClientSettings.ComPort)
                    isSerialPortOpen = objHWSerialPort.SerialPortOpen()

                    If isSerialPortOpen = False Then
                        MessageBox.Show("Unable to connect to Honeywell reader.  Ensure reader is properly connected and open window again.", "IVS", MessageBoxButton.OK)
                    End If

                Case "LS_40_USB"

                    CTSDeviceType = LsDefines.LsUnitType.LS_40_USB

                    Me.cmdCTSScanID.Visibility = Windows.Visibility.Visible
                    Me.cmdCTSReadMagStripe.Visibility = Windows.Visibility.Visible

                Case "LS_150_USB"

                    CTSDeviceType = LsDefines.LsUnitType.LS_150_USB

                    Me.cmdCTSScanID.Visibility = Windows.Visibility.Visible
                    Me.cmdCTSReadMagStripe.Visibility = Windows.Visibility.Visible

            End Select

            'End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

End Class