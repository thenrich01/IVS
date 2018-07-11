Imports LsFamily
Imports System.ComponentModel
Imports System.IO
Imports System.Drawing
Imports System.Windows.Threading
'Imports System.Windows.Forms
Imports System.Printing
Imports System.Threading
Imports System.Text
'Imports System.Security.Principal
Imports IVS.Data
Imports IVS.CTS
'Imports IVS.AppLog
Imports IVS.Eseek
Imports IVS.Eseek.M280
Imports IVS.Epson
Imports com.epson.bank.driver
Imports System.Windows.Forms

Public Class WinScanImage

    'Private MyAppLog As New ApplicationLog(ApplicationLog.DateEncodingType.YYYYMMDD, True)
    Private WithEvents timerViewTime As DispatcherTimer = New DispatcherTimer()
    'Private objClientSettings As New ClientSettings
    'Private intIVSUserID As Integer
    Private intSwipeScanID As Integer
    Private strSwipeRawData As String
    Private strDocDisplayedSide As String
    Private intAlertID As Integer
    Private imgEditedImage As Bitmap
    Private objNewBitmapSource As BitmapSource
    Private intViewTimeCountDown As Integer
    Private isViewTimeCountingDown As Boolean = False
    Private intSwipeScanToDisplay As Integer = 0
    Private isImageEdited As Boolean = False
    Private isRecordEdited As Boolean = False
    Public Shared ManualEntrySwipeScanID As Integer
    Private CTSDeviceType As LsDefines.LsUnitType

    'ESeek
    Private WithEvents objSerialPort As ESeekApi
    Private WithEvents BWSerialPort As New BackgroundWorker
    'M280
    Private WithEvents MyM280 As IVS.Eseek.M280.ESeekM280Api
    Private WithEvents timCheckStatus As New Timers.Timer
    Dim M280Status As Byte() = New Byte(M280DEF.STATUS_SIZE) {}
    Dim bInitM280 As Boolean
    Dim bSysBusy As Boolean
    Private MyImage As Bitmap

    'Epson
    Private m_objDriverControl As CApp = Nothing
    Private m_objConfigData As StructByStep = Nothing

    Private Delegate Sub CallbackProcDisplayMicrText(errResult As ErrorCode, objMicrResult As CMicrResult)
    Private Delegate Sub CallbackProcSaveMicrText(errResult As ErrorCode, objMicrResult As CMicrResult)
    Private Delegate Sub CallbackProcDisplayImage(errResult As ErrorCode, objImageResult As CImageResult, bFourSheetScan As Boolean)
    Private Delegate Sub CallbackProcSaveImageData(errResult As ErrorCode, objImageResult As CImageResult)

    Private CallbackFuncDisplayMicrText As CallbackProcDisplayMicrText = Nothing
    Private CallbackFuncSaveMicrText As CallbackProcSaveMicrText = Nothing
    Private CallbackFuncDisplayImageData As CallbackProcDisplayImage = Nothing
    Private CallbackFuncSaveImageData As CallbackProcSaveImageData = Nothing

    Private CallbackFuncProcessError As CApp.CallbackProcProcessError = Nothing
    Private CallbackFuncImage As CApp.CallbackProcImage = Nothing
    Private CallbackFuncMicr As CApp.CallbackProcMicr = Nothing

    Private m_bScanCancelError As Boolean = False
    Private m_biImage2Front As Bitmap = Nothing
    Private m_biImage2Back As Bitmap = Nothing

    ' Private m_test As Integer

    Public Sub New(Optional ByVal SwipeScanID As Integer = 0)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        'Dim intClientID As Integer

        Try
            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner

            'intClientID = DataAccess.GetClientID()
            'objClientSettings = DataAccess.GetClientSettings(intClientID)

            EnableCTSCommands(False)

            ' System.Windows.MessageBox.Show(objClientSettings.DeviceType, "New", MessageBoxButton.OK)

            Select Case WinMain.objClientSettings.DeviceType

                Case "LS_40_USB"

                    Me.cmdCapture.Visibility = Windows.Visibility.Hidden
                    CTSDeviceType = LsDefines.LsUnitType.LS_40_USB

                    Dim MyCTSObject As New CTSApi()
                    AddHandler MyCTSObject.BWIdentifyCompleted, AddressOf BWCTSIdentify_RunWorkerCompleted
                    MyCTSObject.BWCTSIdentify_Start(CTSDeviceType)

                Case "LS_150_USB"

                    Me.cmdCapture.Visibility = Windows.Visibility.Hidden

                    CTSDeviceType = LsDefines.LsUnitType.LS_150_USB

                    Dim MyCTSObject As New CTSApi()
                    AddHandler MyCTSObject.BWIdentifyCompleted, AddressOf BWCTSIdentify_RunWorkerCompleted
                    MyCTSObject.BWCTSIdentify_Start(CTSDeviceType)

                Case "M210/260"

                    Me.cmdCapture.Visibility = Windows.Visibility.Visible

                    MyM280 = New IVS.Eseek.M280.ESeekM280Api
                    Me.timCheckStatus.Interval = 100
                    Me.timCheckStatus.Start()

                    Dim isSerialPortOpen As Boolean = False

                    objSerialPort = New ESeekApi(WinMain.objClientSettings.ComPort, WinMain.objClientSettings.SleepMilliSeconds)
                    isSerialPortOpen = objSerialPort.SerialPortOpen()

                    If isSerialPortOpen = False Then
                        System.Windows.MessageBox.Show("Unable to connect to ESeek reader.  Ensure reader is properly connected and open window again.", "IVS", MessageBoxButton.OK)
                    End If

                    'Hide CTS commands
                    Me.cmdCTSReadMagStripe.Visibility = Windows.Visibility.Hidden
                    Me.cmdCTSResetHW.Visibility = Windows.Visibility.Hidden
                    Me.cmdCTSScanCheck.Visibility = Windows.Visibility.Hidden
                    Me.cmdCTSScanID.Visibility = Windows.Visibility.Hidden
                    Me.RecCheck.Visibility = Windows.Visibility.Hidden
                    Me.lblCheckNumber.Visibility = Windows.Visibility.Hidden
                    Me.labelCheckNumber.Visibility = Windows.Visibility.Hidden
                    Me.lblStatusCTS.Visibility = Windows.Visibility.Hidden
                    Me.ProgressBarCTS.Visibility = Windows.Visibility.Hidden

                    'Reposition Controls
                    Me.RecUCC.Margin = New Thickness(105, 545, 0, 0)
                    Me.labelCaseID.Margin = New Thickness(110, 545, 0, 0)
                    Me.txtCaseID.Margin = New Thickness(215, 545, 0, 0)
                    Me.cbIsUCCScan.Margin = New Thickness(470, 550, 0, 0)

                    Me.cmdManualEntry.Margin = New Thickness(10, 120, 0, 0)
                    Me.cmdPrint.Margin = New Thickness(10, 170, 0, 0)
                    Me.cmdNewAlert.Margin = New Thickness(10, 220, 0, 0)
                    Me.cmdCompare.Margin = New Thickness(10, 270, 0, 0)
                    Me.cmdClear.Margin = New Thickness(10, 320, 0, 0)
                    Me.cmdRawData.Margin = New Thickness(10, 370, 0, 0)
                    Me.cmdClose.Margin = New Thickness(10, 420, 0, 0)

                    ' Me.cmdCapture.Visibility = Windows.Visibility.Visible
                    Me.imgScannedDocument.Width = 290

                Case "TM-S9000"

                    Me.cmdCapture.Visibility = Windows.Visibility.Visible
                    'Hide CTS commands
                    Me.cmdCTSReadMagStripe.Visibility = Windows.Visibility.Visible
                    Me.cmdCTSResetHW.Visibility = Windows.Visibility.Hidden
                    Me.cmdCTSScanCheck.Visibility = Windows.Visibility.Hidden
                    Me.cmdCTSScanID.Visibility = Windows.Visibility.Hidden
                    Me.RecCheck.Visibility = Windows.Visibility.Hidden
                    Me.lblCheckNumber.Visibility = Windows.Visibility.Hidden
                    Me.labelCheckNumber.Visibility = Windows.Visibility.Hidden
                    Me.lblStatusCTS.Visibility = Windows.Visibility.Visible
                    Me.ProgressBarCTS.Visibility = Windows.Visibility.Hidden

                    'Reposition Controls
                    Me.RecUCC.Margin = New Thickness(105, 545, 0, 0)
                    Me.labelCaseID.Margin = New Thickness(110, 545, 0, 0)
                    Me.txtCaseID.Margin = New Thickness(215, 545, 0, 0)
                    Me.cbIsUCCScan.Margin = New Thickness(470, 550, 0, 0)

                    Me.cmdCTSReadMagStripe.Margin = New Thickness(10, 120, 0, 0)

                    Me.cmdManualEntry.Margin = New Thickness(10, 170, 0, 0)
                    Me.cmdPrint.Margin = New Thickness(10, 220, 0, 0)
                    Me.cmdNewAlert.Margin = New Thickness(10, 270, 0, 0)
                    Me.cmdCompare.Margin = New Thickness(10, 320, 0, 0)
                    Me.cmdClear.Margin = New Thickness(10, 370, 0, 0)
                    Me.cmdRawData.Margin = New Thickness(10, 420, 0, 0)
                    Me.cmdClose.Margin = New Thickness(10, 470, 0, 0)

                    Me.imgScannedDocument.Width = 290

                    Epson_Connect()

            End Select

            'intIVSUserID = IVSUserID

            intSwipeScanToDisplay = SwipeScanID

            If intSwipeScanToDisplay > 0 Then

                DataSetSwipeScans_Navigate("Position", intSwipeScanToDisplay)
            Else

                If WinMain.objClientSettings.DisableDBSave = False Then
                    DataSetSwipeScans_Navigate("Last")
                Else
                    Me.cmdManualEntry.IsEnabled = False
                    Me.lblRecordNavigation.Visibility = Windows.Visibility.Hidden
                    Me.cmdNavPrevious.Visibility = Windows.Visibility.Hidden
                    Me.cmdNavNext.Visibility = Windows.Visibility.Hidden
                    Me.cmdNavLast.Visibility = Windows.Visibility.Hidden
                    Me.cmdBrightnessAdd.Visibility = Windows.Visibility.Hidden
                    Me.cmdBrightnessSubtract.Visibility = Windows.Visibility.Hidden
                    Me.cmdRotate.Visibility = Windows.Visibility.Hidden
                    Me.cmdSave.Visibility = Windows.Visibility.Hidden
                    Me.lblSwipeHistory.IsEnabled = False
                    Me.labelSwipeCount.IsEnabled = False
                    Me.lblSwipeCount.IsEnabled = False
                    Me.dgSwipeScanHistory.IsEnabled = False
                    Me.cmdCompare.IsEnabled = False
                End If

            End If

            Me.lblLocation.Content = WinMain.objClientSettings.Location
            Me.lblStation.Content = WinMain.objClientSettings.Station
            Me.lblUserName.Content = WinMain.strIVSUserName

            If WinMain.isUserAdmin = False Then
                Me.cmdRawData.IsEnabled = False
            End If

            If WinMain.isUserAlertAble = False Then
                Me.cmdNewAlert.IsEnabled = False
            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#Region "Control Bound Subs"

    Private Sub WinCTS_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded

        Try
            timerViewTime.IsEnabled = True
            timerViewTime.Interval = TimeSpan.FromSeconds(1)
            timerViewTime.Start()
            isViewTimeCountingDown = True

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub WinScanImage_KeyDown() Handles Me.KeyDown, Me.MouseDown
        intViewTimeCountDown = WinMain.objClientSettings.ViewingTime
    End Sub

    Private Sub WinScanImage_StateChanged(sender As Object, e As System.EventArgs) Handles Me.StateChanged

        Try

            Select Case Me.WindowState

                Case WindowState.Maximized
                    'On Maximize, Reconnect if not connected
                    Epson_Connect()

                    Exit Select
                Case WindowState.Minimized
                    'On Minimize, drop delegates and then close connection
                    Epson_Disconnect()

                    Exit Select
                Case WindowState.Normal
                    'On Normal, Reconnect if not connected
                    Epson_Connect()

                    Exit Select
            End Select

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try
    End Sub

    Private Sub WinScanImage_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Me.Closing

        Try
            timerViewTime.Stop()

            Select Case WinMain.objClientSettings.DeviceType

                Case "M200/250"

                    If objSerialPort IsNot Nothing Then
                        objSerialPort.SerialPortClose()
                    End If

                Case "TM-S9000"

                    Epson_Disconnect()

            End Select

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgSwipeScanHistory_LoadingRow(sender As Object, e As System.Windows.Controls.DataGridRowEventArgs) Handles dgSwipeScanHistory.LoadingRow

        Dim dgUsers_SelectedRow As SwipeScanHistory

        Try
            dgUsers_SelectedRow = e.Row.Item

            If dgUsers_SelectedRow.SwipeScanID = intSwipeScanID Then

                e.Row.Background = New SolidColorBrush(Colors.Yellow)
            Else
                e.Row.Background = New SolidColorBrush(Colors.White)
            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgSwipeScanHistory_UnloadingRow(sender As Object, e As System.Windows.Controls.DataGridRowEventArgs) Handles dgSwipeScanHistory.UnloadingRow

        Try
            e.Row.Background = New SolidColorBrush(Colors.White)
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgSwipeScanHistory_SelectionChanged(sender As Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgSwipeScanHistory.SelectionChanged

        Dim dgSwipeScanHistory_SelectedRow As System.Collections.IList

        Try
            dgSwipeScanHistory_SelectedRow = e.AddedItems

            If dgSwipeScanHistory_SelectedRow.Count > 0 Then

                intSwipeScanToDisplay = dgSwipeScanHistory_SelectedRow(0).SwipeScanID

            End If

            intViewTimeCountDown = WinMain.objClientSettings.ViewingTime
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgSwipeScanHistory_CmdView_Click()

        Try
            DataSetSwipeScans_Navigate("Position", intSwipeScanToDisplay)

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

            intViewTimeCountDown = WinMain.objClientSettings.ViewingTime

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgAlerts_CmdView_Click()

        Dim WinAlert As WinAlert
        Dim DialogResult As Boolean
        Dim objAlertDetail As New AlertDetail

        Try
            isViewTimeCountingDown = False

            objAlertDetail = DataAccess.GetAlertDetail(intAlertID)

            WinAlert = New WinAlert(objAlertDetail, WinMain.isUserAlertAble)
            WinAlert.Owner = Me

            DialogResult = WinAlert.ShowDialog

            intViewTimeCountDown = WinMain.objClientSettings.ViewingTime
            isViewTimeCountingDown = True

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdNavLast_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdNavLast.Click

        Try
            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save updates before scanning?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

                    If isImageEdited = True Then
                        SaveImage()
                    End If

                    'If isRecordEdited = True Then
                    '    SaveData()
                    'End If

                End If

            End If

            DataSetSwipeScans_Navigate("Last")

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdNavNext_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdNavNext.Click

        Try
            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save updates before scanning?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

                    If isImageEdited = True Then
                        SaveImage()
                    End If

                    'If isRecordEdited = True Then
                    '    SaveData()
                    'End If

                End If

            End If
            DataSetSwipeScans_Navigate("Next")

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdNavPrevious_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdNavPrevious.Click

        Try
            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save updates before scanning?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

                    If isImageEdited = True Then
                        SaveImage()
                    End If

                    'If isRecordEdited = True Then
                    '    SaveData()
                    'End If

                End If

            End If

            DataSetSwipeScans_Navigate("Previous")

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdCTSScanID_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCTSScanID.Click

        'If WinMain.objClientSettings.DeviceType = "LS_40_USB" Or WinMain.objClientSettings.DeviceType = "LS_150_USB" Then

        Dim objCTSApi As New CTSApi

        Try
            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save updates before scanning?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

                    If isImageEdited = True Then
                        SaveImage()
                    End If

                    'If isRecordEdited = True Then
                    '    SaveData()
                    'End If

                End If

            End If

            AddHandler objCTSApi.BWProgressChanged, AddressOf BWScanDLIDImage_ProgressChanged
            AddHandler objCTSApi.BWCompleted, AddressOf BWScanDLIDImage_RunWorkerCompleted

            objCTSApi.BWScanDLIDImage_Start(CTSDeviceType)

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        'ElseIf WinMain.objClientSettings.DeviceType = "TM-S9000" Then

        '    If m_objDriverControl Is Nothing Then
        '        Return
        '    End If

        '    ClearData()

        '    Dim objCARDScanParam As New CScanParam
        '    objCARDScanParam.SetScanMedia(ScanUnit.EPS_BI_SCN_UNIT_CARD)
        '    objCARDScanParam.SetRGBColorDepth(com.epson.bank.driver.ColorDepth.EPS_BI_SCN_24BIT)
        '    objCARDScanParam.SetResolution(MfScanDpi.MF_SCAN_DPI_600)
        '    objCARDScanParam.SetIRColorDepth(com.epson.bank.driver.ColorDepth.EPS_BI_SCN_24BIT)
        '    objCARDScanParam.SetImageChannel(ImageTypeOption.EPS_BI_SCN_OPTION_COLOR)

        '    m_objDriverControl.SetScanParam(objCARDScanParam)

        '    Me.imgScannedDocument.Width = 290

        '    Dim errRet As ErrorCode = m_objDriverControl.ScanCard()
        '    If errRet <> ErrorCode.SUCCESS Then
        '        ShowErrorMessage(errRet)
        '        m_objDriverControl.CancelError()
        '        Return
        '    End If

        'End If

    End Sub

    Private Sub cmdCTSReadMagStripe_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCTSReadMagStripe.Click

        If WinMain.objClientSettings.DeviceType = "LS_40_USB" Or WinMain.objClientSettings.DeviceType = "LS_150_USB" Then

            Dim objCTSApi As New CTSApi

            Try
                If isImageEdited = True Then

                    If System.Windows.MessageBox.Show("Save updates before scanning?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

                        If isImageEdited = True Then
                            SaveImage()
                        End If

                        'If isRecordEdited = True Then
                        '    SaveData()
                        'End If

                    End If

                End If

                AddHandler objCTSApi.BWProgressChanged, AddressOf BWReadMagData_ProgressChanged
                AddHandler objCTSApi.BWCompleted, AddressOf BWReadMagData_RunWorkerCompleted

                objCTSApi.BWReadMagStripe_Start(CTSDeviceType)

            Catch ex As Exception
                WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
            End Try

        ElseIf WinMain.objClientSettings.DeviceType = "TM-S9000" Then

            'isViewTimeCountingDown = False

            Me.txtReaderInput.Text = Nothing
            Me.txtReaderInput.Focus()

            'lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Waiting for card scan")

        End If

    End Sub

    Private Sub cmdCTSScanCheck_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCTSScanCheck.Click

        Dim objCTSApi As New CTSApi

        Try
            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save updates before scanning?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

                    If isImageEdited = True Then
                        SaveImage()
                    End If

                    'If isRecordEdited = True Then
                    '    SaveData()
                    'End If

                End If

            End If

            AddHandler objCTSApi.BWProgressChanged, AddressOf BWScanCheckImage_ProgressChanged
            AddHandler objCTSApi.BWCompleted, AddressOf BWScanCheckImage_RunWorkerCompleted

            objCTSApi.BWScanCheckImage_Start(CTSDeviceType)

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdCTSResetHW_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCTSResetHW.Click

        Dim objCTSApi As New CTSApi

        Try

            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save updates before reset?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

                    If isImageEdited = True Then
                        SaveImage()
                    End If

                    'If isRecordEdited = True Then
                    '    SaveData()
                    'End If

                End If

            End If

            AddHandler objCTSApi.BWProgressChanged, AddressOf BWCTSReset_ProgressChanged
            AddHandler objCTSApi.BWCTSResetCompleted, AddressOf BWCTSReset_RunWorkerCompleted

            objCTSApi.BWCTSReset_Start(CTSDeviceType)

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub imgScannedDocument_MouseDown(sender As Object, e As System.Windows.Input.MouseButtonEventArgs) Handles imgScannedDocument.MouseDown

        If strDocDisplayedSide <> "X" Then

            If e.ChangedButton = MouseButton.Left And e.ClickCount = 2 Then

                Dim WinViewImage As WinView
                Dim strSwipeScanType As String

                Try
                    isViewTimeCountingDown = False

                    strSwipeScanType = Me.LblIDType.Content

                    WinViewImage = New WinView(intSwipeScanID, strSwipeScanType)
                    WinViewImage.Owner = Me
                    WinViewImage.ShowDialog()

                    isViewTimeCountingDown = True

                Catch ex As Exception
                    WinMain.MyAppLog.WriteToLog("WinCTS.cmdViewDocument_Click()" & ex.ToString)
                End Try

            End If

            If e.ChangedButton = MouseButton.Right Then
                Dim imgDocToDisplay As New BitmapImage

                Try
                    imgDocToDisplay.BeginInit()

                    Select Case strDocDisplayedSide

                        Case "F"

                            If File.Exists(WinMain.objClientSettings.ImageLocation & "\" & intSwipeScanID & "b.jpg") = True Then

                                imgEditedImage = New Bitmap(WinMain.objClientSettings.ImageLocation & "\" & intSwipeScanID & "b.jpg")
                                imgDocToDisplay.UriSource = New Uri(WinMain.objClientSettings.ImageLocation & "\" & intSwipeScanID & "b.jpg", UriKind.Absolute)

                            Else

                                imgDocToDisplay.UriSource = New Uri("./Resources/NoImage.jpg", UriKind.Relative)

                            End If

                            strDocDisplayedSide = "B"

                        Case "B"

                            If File.Exists(WinMain.objClientSettings.ImageLocation & "\" & intSwipeScanID & "f.jpg") = True Then

                                imgEditedImage = New Bitmap(WinMain.objClientSettings.ImageLocation & "\" & intSwipeScanID & "f.jpg")
                                imgDocToDisplay.UriSource = New Uri(WinMain.objClientSettings.ImageLocation & "\" & intSwipeScanID & "f.jpg", UriKind.Absolute)

                            Else

                                imgDocToDisplay.UriSource = New Uri("./Resources/NoImage.jpg", UriKind.Relative)

                            End If

                            strDocDisplayedSide = "F"

                    End Select

                    imgDocToDisplay.EndInit()

                    Me.imgScannedDocument.Source = imgDocToDisplay
                    isViewTimeCountingDown = False

                Catch ex As Exception
                    WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
                End Try

            End If
        End If
    End Sub

    Private Sub cmdBrightnessAdd_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdBrightnessAdd.Click

        Try
            imgEditedImage = CTSApi.AdjustBitmapBrightness(imgEditedImage, 10)
            objNewBitmapSource = CTSApi.GetBitmapSource(imgEditedImage)
            Me.imgScannedDocument.Source = objNewBitmapSource

            isImageEdited = True
            Me.cmdSave.IsEnabled = True
            intViewTimeCountDown = WinMain.objClientSettings.ViewingTime

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdBrightnessSubtract_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdBrightnessSubtract.Click

        Try
            imgEditedImage = CTSApi.AdjustBitmapBrightness(imgEditedImage, -10)
            objNewBitmapSource = CTSApi.GetBitmapSource(imgEditedImage)
            Me.imgScannedDocument.Source = objNewBitmapSource

            isImageEdited = True
            Me.cmdSave.IsEnabled = True
            intViewTimeCountDown = WinMain.objClientSettings.ViewingTime

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdRotate_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdRotate.Click

        Try
            imgEditedImage = CTSApi.RotateBitmap(imgEditedImage)
            objNewBitmapSource = CTSApi.GetBitmapSource(imgEditedImage)
            Me.imgScannedDocument.Source = objNewBitmapSource

            isImageEdited = True
            Me.cmdSave.IsEnabled = True
            intViewTimeCountDown = WinMain.objClientSettings.ViewingTime

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdSave_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdSave.Click

        Try
            If isImageEdited = True Then
                SaveImage()
            End If

            'If isRecordEdited = True Then
            SaveData()
            'End If

            intViewTimeCountDown = WinMain.objClientSettings.ViewingTime

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdCompare_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCompare.Click

        Dim strSwipeScanType As String
        Dim winCompareScans As WinCompare

        Try
            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save updates before comparing the last 2 scans?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

                    If isImageEdited = True Then
                        SaveImage()
                    End If

                    'If isRecordEdited = True Then
                    '    SaveData()
                    'End If

                End If

            End If

            isViewTimeCountingDown = False

            strSwipeScanType = Me.LblIDType.Content

            winCompareScans = New WinCompare(intSwipeScanID, strSwipeScanType)
            winCompareScans.Owner = Me
            winCompareScans.ShowDialog()

            isViewTimeCountingDown = True

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdRawData_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdRawData.Click

        Dim winViewRawData As WinRawData

        Try
            isViewTimeCountingDown = False

            winViewRawData = New WinRawData(strSwipeRawData)
            winViewRawData.Owner = Me
            winViewRawData.ShowDialog()

            intViewTimeCountDown = WinMain.objClientSettings.ViewingTime
            isViewTimeCountingDown = True

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdNewAlert_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdNewAlert.Click

        Dim WinNewAlert As WinAlert
        Dim DialogResult As Boolean
        Dim strDocumentID As String
        Dim strNameFirst As String
        Dim strNameLast As String

        Dim objAlertDetail As New AlertDetail

        Try
            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save updates before entering an alert?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

                    If isImageEdited = True Then
                        SaveImage()
                    End If

                    'If isRecordEdited = True Then
                    '    SaveData()
                    'End If

                End If

            End If

            strDocumentID = Me.lblIDNumber.Content
            strNameFirst = Me.lblNameFirst.Content
            strNameLast = Me.lblNameLast.Content

            objAlertDetail.AlertID = 0
            objAlertDetail.IDNumber = strDocumentID
            objAlertDetail.NameFirst = strNameFirst
            objAlertDetail.NameLast = strNameLast
            objAlertDetail.DateOfBirth = Me.lblDateOfBirth.Content
            objAlertDetail.AlertContactName = WinMain.strIVSUserName
            objAlertDetail.AlertContactNumber = WinMain.objClientSettings.Phone
            objAlertDetail.UserID = WinMain.intIVSUserID

            isViewTimeCountingDown = False


            WinNewAlert = New WinAlert(objAlertDetail)
            WinNewAlert.Owner = Me

            DialogResult = WinNewAlert.ShowDialog

            If DialogResult = True Then

                SwipeScanAlerts_Load(strDocumentID, strNameFirst, strNameLast)

            End If
            isViewTimeCountingDown = True

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdClose_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdClose.Click

        Try
            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save updates before closing?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

                    If isImageEdited = True Then
                        SaveImage()
                    End If

                    'If isRecordEdited = True Then
                    '    SaveData()
                    'End If

                End If

            End If

            timerViewTime.Stop()

            Select Case WinMain.objClientSettings.DeviceType

                Case "M200/250"

                    If objSerialPort IsNot Nothing Then
                        objSerialPort.SerialPortClose()
                    End If

                Case "TM-S9000"
                   Epson_Disconnect
            End Select

            Me.Close()

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub timerViewTime_Tick(sender As Object, e As System.EventArgs) Handles timerViewTime.Tick

        Try

            If isViewTimeCountingDown = True Then

                Me.txtReaderInput.Text = Nothing
                Me.txtReaderInput.Focus()

                If intViewTimeCountDown > 0 Then

                    intViewTimeCountDown -= 1

                Else
                    ClearData()
                End If

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdClear_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdClear.Click

        Try
            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save updates before clearing the data?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

                    If isImageEdited = True Then
                        SaveImage()
                    End If

                    'If isRecordEdited = True Then
                    '    SaveData()
                    'End If

                End If

            End If

            ClearData()

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdManualEntry_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdManualEntry.Click

        Dim winManualEntry As WinManEntry

        Try
            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save updates before manual data entry?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

                    If isImageEdited = True Then
                        SaveImage()
                    End If

                    'If isRecordEdited = True Then
                    '    SaveData()
                    'End If

                End If

            End If

            isViewTimeCountingDown = False

            winManualEntry = New WinManEntry("CTS")
            winManualEntry.Owner = Me
            winManualEntry.ShowDialog()

            DataSetSwipeScans_Navigate("Last")

            intViewTimeCountDown = WinMain.objClientSettings.ViewingTime
            isViewTimeCountingDown = True

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdPrint_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdPrint.Click

        Try
            Dim dlgPrint As System.Windows.Controls.PrintDialog = New System.Windows.Controls.PrintDialog

            If dlgPrint.ShowDialog().GetValueOrDefault(True) Then

                dlgPrint.PrintQueue = LocalPrintServer.GetDefaultPrintQueue
                dlgPrint.PrintTicket.CopyCount = 1
                dlgPrint.PrintTicket.PageOrientation = PageOrientation.Landscape
                dlgPrint.PrintVisual(Me, Me.Title)

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub txtCaseID_TextChanged(sender As Object, e As System.Windows.Controls.TextChangedEventArgs) Handles txtCaseID.TextChanged
        Try
            Me.cmdSave.IsEnabled = True
            isRecordEdited = True
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try
    End Sub

    Private Sub txtDateOfExpiration_TextChanged(sender As Object, e As System.Windows.Controls.TextChangedEventArgs) Handles txtDateOfExpiration.TextChanged
        Try
            Me.cmdSave.IsEnabled = True
            isRecordEdited = True
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try
    End Sub

    Private Sub cbIsUCCScan_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cbIsUCCScan.Click
        Try
            Me.cmdSave.IsEnabled = True
            isRecordEdited = True
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try
    End Sub

#End Region

#Region "Custom Subs/Functions"

    Private Sub SaveImage()

        Dim objImageDetail As New IVS.CTS.ImageDetail

        Try

            objImageDetail.Image = imgEditedImage
            objImageDetail.FileName = WinMain.objClientSettings.ImageLocation & "\" & intSwipeScanID & strDocDisplayedSide & ".jpg"
            CTSApi.SaveImage(objImageDetail)

            isImageEdited = False
            Me.cmdSave.IsEnabled = False
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub SaveData()

        Dim strDateOfExpiration As String

        Try
            If Me.txtDateOfExpiration.Text <> "" Then
                strDateOfExpiration = Me.txtDateOfExpiration.Text
            ElseIf Me.lblDateOfExpiration.Content <> "" Then
                strDateOfExpiration = Me.lblDateOfExpiration.Content
            End If
            DataAccess.UpdateUCCID(intSwipeScanID, Me.txtCaseID.Text, Me.cbIsUCCScan.IsChecked, strDateOfExpiration, Me.LblIDType.Content)
            Me.lblDateOfExpiration.Content = Me.txtDateOfExpiration.Text
            Me.lblDateOfExpiration.Visibility = Windows.Visibility.Visible
            Me.txtDateOfExpiration.Visibility = Windows.Visibility.Hidden
            Me.cmdSave.IsEnabled = False

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub SwipeScanDetailLoad(ByVal SwipeScanDetailObject As SwipeScanDetail, Optional ByVal FrontImage As Bitmap = Nothing, Optional ByVal BackImage As Bitmap = Nothing)

        Try
            RemoveHandler txtDateOfExpiration.TextChanged, AddressOf txtDateOfExpiration_TextChanged
            RemoveHandler cbIsUCCScan.Click, AddressOf cbIsUCCScan_Click
            RemoveHandler txtCaseID.TextChanged, AddressOf txtCaseID_TextChanged

            Me.LblIDType.Content = SwipeScanDetailObject.CardType
            Me.lblTimeStamp.Content = SwipeScanDetailObject.UpdateTS

            If SwipeScanDetailObject.CardType = "Credit Card" Then
                Select Case SwipeScanDetailObject.CCIssuer

                    Case "AX"
                        Me.lblIDNumber.Content = "American Express " & SwipeScanDetailObject.IDAccountNumber
                    Case "MC"
                        Me.lblIDNumber.Content = "Mastercard " & SwipeScanDetailObject.IDAccountNumber
                    Case "VI"
                        Me.lblIDNumber.Content = "Visa " & SwipeScanDetailObject.IDAccountNumber
                    Case Else
                        Me.lblIDNumber.Content = SwipeScanDetailObject.IDAccountNumber
                End Select
            Else
                Me.lblIDNumber.Content = SwipeScanDetailObject.IDAccountNumber
            End If

            Me.lblNameFirst.Content = SwipeScanDetailObject.NameFirst
            Me.lblNameLast.Content = SwipeScanDetailObject.NameLast
            Me.lblNameMiddle.Content = SwipeScanDetailObject.NameMiddle
            Me.lblDateOfBirth.Content = SwipeScanDetailObject.DateOfBirth

            If SwipeScanDetailObject.Age > 0 Then
                Me.lblAge.Content = SwipeScanDetailObject.Age
            Else
                Me.lblAge.Content = Nothing
            End If

            Me.lblSex.Content = SwipeScanDetailObject.Sex
            Me.lblHeight.Content = SwipeScanDetailObject.Height
            Me.lblWeight.Content = SwipeScanDetailObject.Weight
            Me.lblEyes.Content = SwipeScanDetailObject.Eyes
            Me.lblHair.Content = SwipeScanDetailObject.Hair
            Me.lblDateOfIssue.Content = SwipeScanDetailObject.DateOfIssue
            Me.lblDateOfExpiration.Content = SwipeScanDetailObject.DateOfExpiration
            Me.txtDateOfExpiration.Text = SwipeScanDetailObject.DateOfExpiration

            If String.IsNullOrEmpty(Trim(SwipeScanDetailObject.DateOfExpiration)) = False Then

                If SwipeScanDetailObject.DateOfExpiration <> "unknown" And SwipeScanDetailObject.DateOfExpiration <> "" Then

                    If IsDate(SwipeScanDetailObject.DateOfExpiration) = True Then
                        If SwipeScanDetailObject.DateOfExpiration < Today Then
                            Me.lblDateOfExpiration.Foreground = System.Windows.Media.Brushes.Red
                            Me.lblDateOfExpiration.Background = System.Windows.Media.Brushes.Yellow
                            Me.lblDateOfExpiration.FontWeight = FontWeights.Bold
                        Else

                            Me.lblDateOfExpiration.Foreground = System.Windows.Media.Brushes.Black
                            Me.lblDateOfExpiration.Background = System.Windows.Media.Brushes.White
                            Me.lblDateOfExpiration.FontWeight = FontWeights.Normal
                        End If

                        Me.lblDateOfExpiration.Visibility = Windows.Visibility.Visible
                        Me.txtDateOfExpiration.Visibility = Windows.Visibility.Hidden
                    End If

                ElseIf SwipeScanDetailObject.DateOfExpiration = "unknown" Or SwipeScanDetailObject.DateOfExpiration = "" Then

                    Me.lblDateOfExpiration.Visibility = Windows.Visibility.Hidden
                    Me.txtDateOfExpiration.Visibility = Windows.Visibility.Visible

                End If

            End If

            If String.IsNullOrEmpty(SwipeScanDetailObject.Age) = False Then

                If IsNumeric(SwipeScanDetailObject.Age) = True Then

                    If SwipeScanDetailObject.Age < WinMain.objClientSettings.Age And SwipeScanDetailObject.Age > 0 Then

                        If WinMain.objClientSettings.AgeHighlight = True Then
                            Me.lblAge.Foreground = System.Windows.Media.Brushes.Red
                            Me.lblAge.Background = System.Windows.Media.Brushes.Yellow
                            Me.lblAge.FontWeight = FontWeights.Bold
                            Me.lblAge.FontSize = 19

                        End If

                        If WinMain.objClientSettings.AgePopup = True Then

                            Dim WinUnderAge As WinUnderAge

                            WinUnderAge = New WinUnderAge(SwipeScanDetailObject.NameFirst & " " & SwipeScanDetailObject.NameLast, SwipeScanDetailObject.Age, WinMain.objClientSettings.Age)
                            WinUnderAge.Owner = Me
                            WinUnderAge.ShowDialog()

                        End If

                    Else

                        Me.lblAge.Foreground = System.Windows.Media.Brushes.Black
                        Me.lblAge.Background = System.Windows.Media.Brushes.White
                        Me.lblAge.FontWeight = FontWeights.Normal
                        Me.lblAge.FontSize = 16
                    End If

                End If

            End If

            Me.lblAddressStreet.Content = SwipeScanDetailObject.AddressStreet
            Me.lblAddressCity.Content = SwipeScanDetailObject.AddressCity
            Me.lblAddressState.Content = SwipeScanDetailObject.AddressState
            Me.lblAddressZip.Content = SwipeScanDetailObject.AddressZip
            strSwipeRawData = "Data Source: " & SwipeScanDetailObject.DataSource & vbCrLf & SwipeScanDetailObject.SwipeRawData
            Me.lblCheckNumber.Content = SwipeScanDetailObject.CheckNumber

            intSwipeScanID = SwipeScanDetailObject.SwipeScanID

            If FrontImage IsNot Nothing Or File.Exists(WinMain.objClientSettings.ImageLocation & "\" & SwipeScanDetailObject.SwipeScanID & "f.jpg") Then

                If FrontImage IsNot Nothing Then

                    Me.imgEditedImage = FrontImage

                ElseIf File.Exists(WinMain.objClientSettings.ImageLocation & "\" & SwipeScanDetailObject.SwipeScanID & "f.jpg") Then

                    imgEditedImage = New Bitmap(WinMain.objClientSettings.ImageLocation & "\" & SwipeScanDetailObject.SwipeScanID & "f.jpg")

                End If

                If SwipeScanDetailObject.CardType = "Check" Then

                    Me.imgScannedDocument.Width = 390

                Else

                    Me.imgScannedDocument.Width = 290

                End If

                objNewBitmapSource = CTSApi.GetBitmapSource(imgEditedImage)

                Me.imgScannedDocument.Source = objNewBitmapSource
                Me.cmdBrightnessAdd.IsEnabled = True
                Me.cmdBrightnessSubtract.IsEnabled = True
                Me.cmdRotate.IsEnabled = True

                strDocDisplayedSide = "F"

                Me.cmdCapture.Visibility = Windows.Visibility.Hidden
                Me.cmdSaveScanFront.Visibility = Windows.Visibility.Hidden
                Me.cmdSaveScanBack.Visibility = Windows.Visibility.Hidden

            Else

                Me.imgScannedDocument.Width = 290

                Dim imgDocToDisplay As New BitmapImage
                imgDocToDisplay.BeginInit()
                imgDocToDisplay.UriSource = New Uri("./Resources/NoImage.jpg", UriKind.Relative)
                imgDocToDisplay.EndInit()

                Me.imgScannedDocument.Source = imgDocToDisplay

                Me.cmdBrightnessAdd.IsEnabled = False
                Me.cmdBrightnessSubtract.IsEnabled = False
                Me.cmdRotate.IsEnabled = False
                Me.cmdSave.IsEnabled = False

                strDocDisplayedSide = "X"

                If WinMain.objClientSettings.DeviceType = "M210/260" Then
                    Me.cmdCapture.Visibility = Windows.Visibility.Visible
                End If

            End If

            If SwipeScanDetailObject.CardType = "Drivers License Or State ID" Or SwipeScanDetailObject.CardType = "Military ID Card" Then

                Me.cbIsUCCScan.IsEnabled = True
                Me.cbIsUCCScan.IsChecked = SwipeScanDetailObject.IsUCCScan

            End If

            Me.txtCaseID.Text = SwipeScanDetailObject.CaseID

            Me.lblSwipeScanUserName.Content = SwipeScanDetailObject.UserName
            Me.lblSwipeScanLocation.Content = SwipeScanDetailObject.Location

            SwipeScanHistory_Load(SwipeScanDetailObject.IDAccountNumber, SwipeScanDetailObject.CardType)
            SwipeScanAlerts_Load(SwipeScanDetailObject.IDAccountNumber, SwipeScanDetailObject.NameFirst, SwipeScanDetailObject.NameLast)

            If WinMain.objClientSettings.ImageSave = True Then

                Dim sb As New StringBuilder
                Dim objImageDetail As New IVS.CTS.ImageDetail

                sb.Append(WinMain.objClientSettings.ImageLocation)
                sb.Append("\")
                sb.Append(intSwipeScanID)
                sb.Append("f.jpg")

                objImageDetail.Image = FrontImage
                objImageDetail.FileName = sb.ToString

                CTSApi.SaveImage(objImageDetail)
                DataAccess.UpdateImageAvailable(intSwipeScanID)

                sb.Clear()
                objImageDetail = New IVS.CTS.ImageDetail

                sb.Append(WinMain.objClientSettings.ImageLocation)
                sb.Append("\")
                sb.Append(intSwipeScanID)
                sb.Append("b.jpg")

                objImageDetail = New IVS.CTS.ImageDetail
                objImageDetail.Image = BackImage
                objImageDetail.FileName = sb.ToString

                ThreadPool.QueueUserWorkItem(AddressOf CTSApi.SaveImage, objImageDetail)

                If File.Exists(WinMain.objClientSettings.ImageLocation & "\" & intSwipeScanID & "f.jpg") = True Then
                    imgEditedImage = New Bitmap(WinMain.objClientSettings.ImageLocation & "\" & intSwipeScanID & "f.jpg")
                End If

            End If

            AddHandler txtDateOfExpiration.TextChanged, AddressOf txtDateOfExpiration_TextChanged
            AddHandler cbIsUCCScan.Click, AddressOf cbIsUCCScan_Click
            AddHandler txtCaseID.TextChanged, AddressOf txtCaseID_TextChanged

            intViewTimeCountDown = WinMain.objClientSettings.ViewingTime

            Me.cmdSave.IsEnabled = False

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub SwipeScanHistory_Load(ByVal IDAccountNumber As String, ByVal SwipeScanType As String)

        Try
            Me.dgSwipeScanHistory.ItemsSource = DataAccess.GetSwipeScanHistory(IDAccountNumber, SwipeScanType)
            Me.lblSwipeCount.Content = dgSwipeScanHistory.Items.Count
            intViewTimeCountDown = WinMain.objClientSettings.ViewingTime
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub SwipeScanAlerts_Load(ByVal IDAccountNumber As String, ByVal NameFirst As String, ByVal NameLast As String)

        Try
            Me.dgAlerts.ItemsSource = DataAccess.GetSwipeScanAlerts(IDAccountNumber, NameFirst, NameLast)
            Me.lblAlertCount.Content = dgAlerts.Items.Count
            intViewTimeCountDown = WinMain.objClientSettings.ViewingTime
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub DataSetSwipeScans_Navigate(ByVal Direction As String, Optional ByVal Position As Integer = 0)

        Dim objSwipeScanNavigateInfo As New SwipeScanNavigateInfo
        Dim strNavigationDirection As String
        Dim strCurrentSwipeScanType As String
        Dim strDocumentID As String
        Dim strNameFirst As String
        Dim strNameLast As String
        Dim objSwipeScanDetail As New SwipeScanDetail

        Try

            strNavigationDirection = Direction

            Select Case strNavigationDirection

                Case "Last"

                    objSwipeScanNavigateInfo = DataAccess.SwipeScanNavigateLast(WinMain.intIVSUserID, WinMain.objClientSettings.ClientID)

                Case "Next"

                    objSwipeScanNavigateInfo = DataAccess.SwipeScanNavigateNext(WinMain.intIVSUserID, WinMain.objClientSettings.ClientID, intSwipeScanID)

                Case "Previous"

                    objSwipeScanNavigateInfo = DataAccess.SwipeScanNavigatePrevious(WinMain.intIVSUserID, WinMain.objClientSettings.ClientID, intSwipeScanID)

                Case "First"

                    objSwipeScanNavigateInfo = DataAccess.SwipeScanNavigateFirst(WinMain.intIVSUserID, WinMain.objClientSettings.ClientID)

                Case "Position"

                    objSwipeScanNavigateInfo = DataAccess.SwipeScanNavigatePosition(Position)

            End Select

            intSwipeScanID = objSwipeScanNavigateInfo.SwipeScanID
            'MyAppLog.WriteToLog("WinCTS.DataSetSwipeScans_Navigate() SwipeScanID:" & intSwipeScanID)

            If intSwipeScanID > 0 Then

                strCurrentSwipeScanType = objSwipeScanNavigateInfo.SwipeScanType

                Me.LblIDType.Content = strCurrentSwipeScanType
                Me.lblTimeStamp.Content = objSwipeScanNavigateInfo.SwipeScanTS

                objSwipeScanDetail = DataAccess.GetSwipeScanDetail(intSwipeScanID, strCurrentSwipeScanType)

                SwipeScanDetailLoad(objSwipeScanDetail)

                strDocumentID = Me.lblIDNumber.Content
                strNameFirst = Me.lblNameFirst.Content
                strNameLast = Me.lblNameLast.Content

                If strDocumentID <> "" Then

                    SwipeScanHistory_Load(strDocumentID, strCurrentSwipeScanType)
                    SwipeScanAlerts_Load(strDocumentID, strNameFirst, strNameLast)

                End If

            End If

            intViewTimeCountDown = WinMain.objClientSettings.ViewingTime

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub EnableCTSCommands(ByVal IsEnabled As Boolean)

        Try
            Me.cmdCTSReadMagStripe.IsEnabled = IsEnabled
            Me.cmdCTSResetHW.IsEnabled = IsEnabled
            Me.cmdCTSScanCheck.IsEnabled = IsEnabled
            Me.cmdCTSScanID.IsEnabled = IsEnabled

            intViewTimeCountDown = WinMain.objClientSettings.ViewingTime

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub ClearData()

        Try
            RemoveHandler txtDateOfExpiration.TextChanged, AddressOf txtDateOfExpiration_TextChanged
            RemoveHandler cbIsUCCScan.Click, AddressOf cbIsUCCScan_Click
            RemoveHandler txtCaseID.TextChanged, AddressOf txtCaseID_TextChanged

            Me.LblIDType.Content = Nothing
            Me.lblTimeStamp.Content = Nothing
            Me.lblAlertCount.Content = Nothing
            Me.lblSwipeCount.Content = Nothing
            Me.dgAlerts.ItemsSource = Nothing
            Me.dgSwipeScanHistory.ItemsSource = Nothing
            Me.lblIDNumber.Content = Nothing
            Me.lblNameLast.Content = Nothing
            Me.lblNameFirst.Content = Nothing
            Me.lblNameMiddle.Content = Nothing
            Me.lblAddressStreet.Content = Nothing
            Me.lblAddressCity.Content = Nothing
            Me.lblAddressState.Content = Nothing
            Me.lblAddressZip.Content = Nothing
            Me.lblDateOfBirth.Content = Nothing
            Me.lblAge.Content = Nothing
            Me.lblAge.Foreground = System.Windows.Media.Brushes.Black
            Me.lblAge.Background = System.Windows.Media.Brushes.White
            Me.lblAge.FontWeight = FontWeights.Normal
            Me.lblSex.Content = Nothing
            Me.lblHeight.Content = Nothing
            Me.lblWeight.Content = Nothing
            Me.lblEyes.Content = Nothing
            Me.lblHair.Content = Nothing
            Me.lblDateOfExpiration.Content = Nothing
            Me.lblDateOfExpiration.Foreground = System.Windows.Media.Brushes.Black
            Me.lblDateOfExpiration.Background = System.Windows.Media.Brushes.White
            Me.lblDateOfExpiration.FontWeight = FontWeights.Normal
            Me.lblDateOfIssue.Content = Nothing
            Me.lblSwipeScanUserName.Content = Nothing
            Me.lblSwipeScanLocation.Content = Nothing
            Me.lblCheckNumber.Content = Nothing
            Me.txtDateOfExpiration.Text = Nothing
            Me.txtDateOfExpiration.Visibility = Windows.Visibility.Hidden
            Me.lblDateOfExpiration.Visibility = Windows.Visibility.Visible
            Me.txtCaseID.Text = Nothing
            Me.cbIsUCCScan.IsChecked = False

            Dim imgDocToDisplay As New BitmapImage
            imgDocToDisplay.BeginInit()
            Me.imgScannedDocument.Width = 290
            imgDocToDisplay.UriSource = New Uri("./Resources/NoImage.jpg", UriKind.Relative)
            imgDocToDisplay.EndInit()
            Me.imgScannedDocument.Source = imgDocToDisplay

            Me.cmdBrightnessAdd.IsEnabled = False
            Me.cmdBrightnessSubtract.IsEnabled = False
            Me.cmdRotate.IsEnabled = False

            strDocDisplayedSide = "X"

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

#Region "CTS"

    Delegate Sub StartTheInvoke_ReportStatus(ByVal Status As String)

    Friend Sub NowStartTheInvoke_ReportStatus(ByVal Status As String)

        Try
            Me.lblStatusCTS.Content = Status

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Delegate Sub StartTheInvoke_UpdateProgressBar(ByVal Progress As Integer)

    Friend Sub NowStartTheInvoke_UpdateProgressBar(ByVal Progress As Integer)

        Try
            Me.ProgressBarCTS.Visibility = Windows.Visibility.Visible
            Me.ProgressBarCTS.Value = Progress

            If Progress = 0 Then

                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait
                ClearData()

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWScanDLIDImage_ProgressChanged(ByVal PercentChange As Integer)

        Try
            ProgressBarCTS.Dispatcher.BeginInvoke(New StartTheInvoke_UpdateProgressBar(AddressOf NowStartTheInvoke_UpdateProgressBar), PercentChange)

            Select Case PercentChange
                Case 0
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Connecting to CTS device")
                Case 5
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Checking CTS device document feeder")
                Case 10
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Document found in feeder")
                Case 11
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "No document detected in Feeder")
                Case 15
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Scanning identification card")
                Case 20
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Retrieving scanned ID images")
                Case 30
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Reading barcode on front of ID")
                Case 50
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Reading barcode on back of ID")
                Case 60
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Decoding barcode")
                Case 70
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Recording information in database")
                Case 80
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Saving front ID image")
                Case 90
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Saving back ID image")
                Case 100
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Finished")
            End Select

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWScanDLIDImage_RunWorkerCompleted(ByVal Result As CTSSwipeScanInfo)

        Dim objSwipeScanInfo As New SwipeScanInfo
        Dim objSwipeScanDetail As New SwipeScanDetail

        Try

            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow
            Me.ProgressBarCTS.Visibility = Windows.Visibility.Hidden

            If Result.LSPhotoFeeder = False Then

                Me.lblStatusCTS.Content = "No document found in feeder"

            End If

            If Result.LSReadBarcodeBack <> 0 And Result.LSReadBarcodeFront <> 0 Then

                Me.lblStatusCTS.Content = "Error occurred while scanning"

                If System.Windows.MessageBox.Show("Save scanned image and read magnetic data?", "Unable to read ID data", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

                    Dim winMagEntry As WinMagEntry
                    winMagEntry = New WinMagEntry()
                    winMagEntry.Owner = Me
                    winMagEntry.ShowDialog()

                    If winMagEntry.DialogResult = True Then

                        Me.lblStatusCTS.Content = "Succesful scan - Ready to scan again"

                        If WinMain.objClientSettings.ImageSave = True Then

                            Dim sb As New StringBuilder
                            Dim objImageDetail As New IVS.CTS.ImageDetail

                            sb.Append(WinMain.objClientSettings.ImageLocation)
                            sb.Append("\")
                            sb.Append(ManualEntrySwipeScanID)
                            sb.Append("f.jpg")

                            objImageDetail.Image = Result.ImageFront
                            objImageDetail.FileName = sb.ToString

                            CTSApi.SaveImage(objImageDetail)
                            DataAccess.UpdateImageAvailable(ManualEntrySwipeScanID)

                            sb.Clear()
                            objImageDetail = New IVS.CTS.ImageDetail

                            sb.Append(WinMain.objClientSettings.ImageLocation)
                            sb.Append("\")
                            sb.Append(ManualEntrySwipeScanID)
                            sb.Append("b.jpg")

                            objImageDetail = New IVS.CTS.ImageDetail
                            objImageDetail.Image = Result.ImageBack
                            objImageDetail.FileName = sb.ToString

                            ThreadPool.QueueUserWorkItem(AddressOf CTSApi.SaveImage, objImageDetail)
                        End If

                        DataSetSwipeScans_Navigate("Last")

                    Else

                        If System.Windows.MessageBox.Show("Save scanned image and enter data manually?", "Unable to read ID data", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

                            Dim winManEntry As WinManEntry
                            winManEntry = New WinManEntry("CTS")
                            winManEntry.Owner = Me
                            winManEntry.ShowDialog()

                            If winManEntry.DialogResult = True Then

                                Me.lblStatusCTS.Content = "Succesful scan - Ready to scan again"

                                If WinMain.objClientSettings.ImageSave = True Then

                                    Dim sb As New StringBuilder
                                    Dim objImageDetail As New IVS.CTS.ImageDetail

                                    sb.Append(WinMain.objClientSettings.ImageLocation)
                                    sb.Append("\")
                                    sb.Append(ManualEntrySwipeScanID)
                                    sb.Append("f.jpg")

                                    objImageDetail.Image = Result.ImageFront
                                    objImageDetail.FileName = sb.ToString

                                    CTSApi.SaveImage(objImageDetail)
                                    DataAccess.UpdateImageAvailable(ManualEntrySwipeScanID)

                                    sb.Clear()
                                    objImageDetail = New IVS.CTS.ImageDetail

                                    sb.Append(WinMain.objClientSettings.ImageLocation)
                                    sb.Append("\")
                                    sb.Append(ManualEntrySwipeScanID)
                                    sb.Append("b.jpg")

                                    objImageDetail = New IVS.CTS.ImageDetail
                                    objImageDetail.Image = Result.ImageBack
                                    objImageDetail.FileName = sb.ToString

                                    ThreadPool.QueueUserWorkItem(AddressOf CTSApi.SaveImage, objImageDetail)
                                End If

                                DataSetSwipeScans_Navigate("Last")

                            End If

                        End If

                    End If

                Else
                    objNewBitmapSource = CTSApi.GetBitmapSource(Result.ImageFront)

                    Me.imgScannedDocument.Width = 290
                    Me.imgScannedDocument.Source = objNewBitmapSource

                    Me.cmdBrightnessAdd.IsEnabled = True
                    Me.cmdBrightnessSubtract.IsEnabled = True
                    Me.cmdRotate.IsEnabled = True
                    Me.cmdSave.IsEnabled = True

                    strDocDisplayedSide = "F"

                End If

            End If

            If Result.LSPhotoFeeder = True And Result.LSConnect = 0 And Result.LSDisableWaitDocument = 0 And Result.LSDocHandle = 0 And Result.LSReadBadgeWithTimeout = 0 _
                And Result.LSReadBarcodeFront = 0 And Result.LSReadCodeline = 0 And Result.LSReadImage = 0 Then

                Me.lblStatusCTS.Content = "Succesful scan - Ready to scan again"

                objSwipeScanInfo.ClientID = WinMain.objClientSettings.ClientID
                objSwipeScanInfo.UserID = WinMain.intIVSUserID
                objSwipeScanInfo.DisableDBSave = WinMain.objClientSettings.DisableDBSave
                objSwipeScanInfo.IDChecker = Application.objMyIDChecker
                objSwipeScanInfo.SwipeScanRawData = Result.SwipeRawData

                objSwipeScanDetail = DataAccess.NewDataSwipeScan(objSwipeScanInfo)

                'SwipeScanDetail_LoadByObject(objSwipeScanDetail, Result.ImageFront, Result.ImageBack)
                SwipeScanDetailLoad(objSwipeScanDetail, Result.ImageFront, Result.ImageBack)

                intViewTimeCountDown = WinMain.objClientSettings.ViewingTime
                isViewTimeCountingDown = True

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWScanCheckImage_ProgressChanged(ByVal PercentChange As Integer)

        Try
            ProgressBarCTS.Dispatcher.BeginInvoke(New StartTheInvoke_UpdateProgressBar(AddressOf NowStartTheInvoke_UpdateProgressBar), PercentChange)

            Select Case PercentChange
                Case 0
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Starting Scan")
                Case 5
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Connecting to CTS device")
                Case 10
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Getting status from CTS device")
                Case 11
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "No document detected in Feeder")
                Case 15
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Checking CTS document feeder")
                Case 20
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Scanning document in feeder")
                Case 30
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Scanning document in feeder")
                Case 50
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Retrieving scanned check images")
                Case 70
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Reading MICR number on check")
                Case 80
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "MICR number retrieved")
                Case 90
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Disconnecting from CTS device")
                Case 100
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Scan completed")

            End Select

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWScanCheckImage_RunWorkerCompleted(ByVal Result As CTSSwipeScanInfo)

        Dim objSwipeScanInfo As New SwipeScanInfo
        Dim objSwipeScanDetail As New SwipeScanDetail

        Try
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow

            Me.ProgressBarCTS.IsEnabled = False
            Me.ProgressBarCTS.Visibility = Windows.Visibility.Hidden

            If Result.LSReadCodeline <> 0 Then

                Me.lblStatusCTS.Content = "Error occurred while scanning"

                objNewBitmapSource = CTSApi.GetBitmapSource(Result.ImageFront)

                Me.imgScannedDocument.Width = 390
                Me.imgScannedDocument.Source = objNewBitmapSource

                Me.cmdBrightnessAdd.IsEnabled = True
                Me.cmdBrightnessSubtract.IsEnabled = True
                Me.cmdRotate.IsEnabled = True
                Me.cmdSave.IsEnabled = True

                strDocDisplayedSide = "F"

            Else

                Me.lblStatusCTS.Content = "Succesful scan - Ready to scan again"

                objSwipeScanInfo.ClientID = WinMain.objClientSettings.ClientID
                objSwipeScanInfo.UserID = WinMain.intIVSUserID
                objSwipeScanInfo.DisableDBSave = WinMain.objClientSettings.DisableDBSave
                'objSwipeScanInfo.IDChecker = Application.objMyIDChecker
                objSwipeScanInfo.SwipeScanRawData = Result.SwipeRawData
                objSwipeScanInfo.ScanType = "C"

                objSwipeScanDetail = DataAccess.NewDataSwipeScan(objSwipeScanInfo)

                'SwipeScanDetail_LoadByObject(objSwipeScanDetail, Result.ImageFront, Result.ImageBack)
                SwipeScanDetailLoad(objSwipeScanDetail, Result.ImageFront, Result.ImageBack)

                intViewTimeCountDown = WinMain.objClientSettings.ViewingTime
                isViewTimeCountingDown = True
            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWReadMagData_ProgressChanged(ByVal PercentChange As Integer)

        Try
            ProgressBarCTS.Dispatcher.BeginInvoke(New StartTheInvoke_UpdateProgressBar(AddressOf NowStartTheInvoke_UpdateProgressBar), PercentChange)

            Select Case PercentChange
                Case 0
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Waiting for card swipe")
                Case 10
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Waiting for card swipe")
                Case 60
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Reading magnetic data")
                Case 80
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Recording swipe information in database")
                Case 100
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Finished")

            End Select

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWReadMagData_RunWorkerCompleted(ByVal Result As CTSSwipeScanInfo)

        Dim objSwipeScanInfo As New SwipeScanInfo
        Dim objSwipeScanDetail As New SwipeScanDetail

        Try
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow

            Me.ProgressBarCTS.Visibility = Windows.Visibility.Hidden

            If Result.LSReadBadgeWithTimeout <> 0 Then
                Me.lblStatusCTS.Content = "Error occurred while scanning"
            Else

                Me.lblStatusCTS.Content = "Succesful scan - Ready to scan again"

                objSwipeScanInfo.ClientID = WinMain.objClientSettings.ClientID
                objSwipeScanInfo.UserID = WinMain.intIVSUserID
                objSwipeScanInfo.DisableDBSave = WinMain.objClientSettings.DisableDBSave
                objSwipeScanInfo.IDChecker = Application.objMyIDChecker
                objSwipeScanInfo.SwipeScanRawData = Result.SwipeRawData

                objSwipeScanDetail = DataAccess.NewDataSwipeScan(objSwipeScanInfo)

                'SwipeScanDetail_LoadByObject(objSwipeScanDetail, Result.ImageFront, Result.ImageBack)
                SwipeScanDetailLoad(objSwipeScanDetail, Result.ImageFront, Result.ImageBack)

                intViewTimeCountDown = WinMain.objClientSettings.ViewingTime
                isViewTimeCountingDown = True
            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWCTSReset_ProgressChanged(ByVal PercentChange As Integer)

        Try
            ProgressBarCTS.Dispatcher.BeginInvoke(New StartTheInvoke_UpdateProgressBar(AddressOf NowStartTheInvoke_UpdateProgressBar), PercentChange)

            Select Case PercentChange

                Case 25
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Resetting software errors")
                Case 50
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Resetting hardware errors")
                Case 75
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Disconnecting from device")
                Case 100
                    lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Finished")

            End Select

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWCTSReset_RunWorkerCompleted(ByVal Result As Boolean)

        Try
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow

            Me.ProgressBarCTS.Visibility = Windows.Visibility.Hidden

            If Result = False Then
                Me.lblStatusCTS.Content = "Error occurred while resetting device"

            Else
                Me.lblStatusCTS.Content = "Succesful reset - Ready to scan again"

            End If

            intViewTimeCountDown = WinMain.objClientSettings.ViewingTime
            isViewTimeCountingDown = True

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWCTSIdentify_RunWorkerCompleted(ByVal Result As CTSDeviceInfo)

        Try

            If Result.ModelNo IsNot Nothing Then

                Dim objDeviceInfo As New DeviceInfo

                objDeviceInfo.ClientID = WinMain.objClientSettings.ClientID
                objDeviceInfo.DeviceID = WinMain.objClientSettings.DeviceID
                objDeviceInfo.ModelNo = Result.ModelNo
                objDeviceInfo.FirmwareRev = Result.FirmwareRev
                objDeviceInfo.FirmwareDate = Result.FirmwareDate
                objDeviceInfo.HardwareRev = "NULL"
                objDeviceInfo.ComPort = "USB"
                objDeviceInfo.SerialNo = Result.SerialNo
                objDeviceInfo.DeviceType = Result.DeviceType

                DataAccess.UpdateDevice(objDeviceInfo)
                Me.lblStatusCTS.Content = "Successful connection to CTS device - Ready to scan"
                EnableCTSCommands(True)

            Else
                Me.lblStatusCTS.Content = "Unable to connect to CTS device"
                WinMain.MyAppLog.WriteToLog("WinCTS.BWCTSIdentify_RunWorkerCompleted() Unable to connect to CTS device")

            End If

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
            WinMain.MyAppLog.WriteToLog("IVS", e.Error.ToString, EventLogEntryType.Error)

        Else

            If Me.WindowState = Windows.WindowState.Minimized Then
                Me.WindowState = Windows.WindowState.Normal
            End If

            SwipeScanDetailLoad(e.Result)

        End If

    End Sub

    Private Function TimeConsumingOperation_NewSwipeIDDL(ByVal RawSerialPortData As String, ByVal bw As BackgroundWorker) As SwipeScanDetail

        Dim objSwipeScanDetail As New SwipeScanDetail
        Dim objSwipeScanInfo As New SwipeScanInfo

        Try
            objSwipeScanInfo.ClientID = WinMain.objClientSettings.ClientID
            objSwipeScanInfo.UserID = WinMain.intIVSUserID
            objSwipeScanInfo.SwipeScanRawData = RawSerialPortData
            objSwipeScanInfo.DisableCCSave = WinMain.objClientSettings.DisableCCSave
            objSwipeScanInfo.DisableDBSave = WinMain.objClientSettings.DisableDBSave
            objSwipeScanInfo.CCDigits = WinMain.objClientSettings.CCDigits
            objSwipeScanInfo.IDChecker = Application.objMyIDChecker

            objSwipeScanDetail = DataAccess.NewDataSwipeScan(objSwipeScanInfo)

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        Return objSwipeScanDetail

    End Function

    Private Sub objSerialPort_OnDataReceived(data As String) Handles objSerialPort.OnDataReceived

        Try
            Me.Dispatcher.BeginInvoke(New StartTheInvoke_OnDataReceived(AddressOf NowStartTheInvoke_OnDataReceived), data)
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Delegate Sub StartTheInvoke_OnDataReceived(ByVal StrRawSerialPortData As String)

    Friend Sub NowStartTheInvoke_OnDataReceived(ByVal StrRawSerialPortData As String)

        Try
            BWSerialPort.RunWorkerAsync(StrRawSerialPortData)

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub CheckState_Tick(sender As Object, e As EventArgs) Handles timCheckStatus.Elapsed

        Dim len As Integer = M280DEF.STATUS_SIZE
        Dim StatusValue As UInt16

        Try
            Me.timCheckStatus.Stop()

            MyM280.ReadCommand(M280DEF.CMD_GET_STATE, M280Status, len, 0, 0)

            StatusValue = BitConverter.ToUInt16(M280Status, 0)

            ' Check Push button
            If (StatusValue And M280DEF.stat_CapDet) = M280DEF.stat_CapDet Then
                MyM280.btnCapture_Click()
            End If

            ' Check Busy LED
            If (StatusValue And M280DEF.stat_BusyLED) = M280DEF.stat_BusyLED Then
                'I AM BUSY
            End If

            ' Check Ready LED
            If (StatusValue And M280DEF.stat_ReadyLED) = M280DEF.stat_ReadyLED Then
                'I AM READY
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
            imgEditedImage = Image

            Me.cmdSaveScanFront.Visibility = Windows.Visibility.Visible
            Me.cmdSaveScanBack.Visibility = Windows.Visibility.Visible

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

#Region "EPSON"

    Private Sub Epson_Connect()

      m_objDriverControl = New CApp()
        m_objConfigData = New StructByStep()

        If InitializeDriver() <> True Then
            'Me.Close()
            Return
        Else
            Me.cmdCTSScanID.IsEnabled = True
            Me.cmdCTSReadMagStripe.IsEnabled = True
            lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Connected to Epson TM-S9000")
        End If

        'Me.txtReaderInput.Focus()

    End Sub

    Private Sub Epson_Disconnect()

        Dim errRet As ErrorCode = ErrorCode.SUCCESS

        errRet = m_objDriverControl.ExitDevice()
        If errRet <> ErrorCode.SUCCESS Then
            ShowErrorMessage(errRet)
        Else

            Me.cmdCTSScanID.IsEnabled = False
            Me.cmdCTSReadMagStripe.IsEnabled = False
            lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Disconnected from Epson TM-S9000")

        End If

    End Sub

    Private Function InitializeDriver() As Boolean

        Dim errResult As ErrorCode = ErrorCode.SUCCESS
        Dim drRet As DialogResult = 0
        Dim strResult As String = Nothing

        If m_objDriverControl Is Nothing Then
            Return False
        End If

        ' Open device and register callback functions
        errResult = m_objDriverControl.InitDevice()
        While errResult <> ErrorCode.SUCCESS
            If errResult = ErrorCode.ERR_UNKNOWN Then
                drRet = MessageBox.Show(ConstComStr.MSG_06_000, ConstComStr.CAPTION_06_000, MessageBoxButtons.OKCancel, MessageBoxIcon.[Error])
            Else
                strResult = IVS.Epson.Epson.GetErrorString(errResult)
                drRet = MessageBox.Show(ConstComStr.MSG_00_000, strResult, MessageBoxButtons.OKCancel, MessageBoxIcon.[Error])
            End If

            If drRet = System.Windows.Forms.DialogResult.OK Then
                errResult = m_objDriverControl.InitDevice()
            Else
                Return False
            End If
        End While

        'CallbackFuncDisplayMicrText = New CallbackProcDisplayMicrText(AddressOf CallbackDisplayMicrText)
        'CallbackFuncSaveMicrText = New CallbackProcSaveMicrText(AddressOf CallbackSaveMicrText)
        CallbackFuncDisplayImageData = New CallbackProcDisplayImage(AddressOf CallbackDisplayImageData)
        'CallbackFuncSaveImageData = New CallbackProcSaveImageData(AddressOf CallbackSaveImageData)

        CallbackFuncProcessError = New CApp.CallbackProcProcessError(AddressOf CallbackProcProcessError)
        CallbackFuncImage = New CApp.CallbackProcImage(AddressOf CallbackProcImage)
        'CallbackFuncMicr = New CApp.CallbackProcMicr(AddressOf CallbackProcMicr)

        ' Specify callback function from DriverControl
        m_objDriverControl.SetProcessErrorCallback(CallbackFuncProcessError)
        m_objDriverControl.SetImageCallback(CallbackFuncImage)
        m_objDriverControl.SetMicrCallback(CallbackFuncMicr)

        Return True

    End Function

    Private Sub txtReaderInput_GotFocus(sender As Object, e As System.Windows.RoutedEventArgs) Handles txtReaderInput.GotFocus
        lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Waiting for card scan")
    End Sub

    Private Sub txtReaderInput_LostFocus(sender As Object, e As System.Windows.RoutedEventArgs) Handles txtReaderInput.LostFocus

        lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "")

    End Sub

    'MAG
    Private Sub txtReaderInput_TextChanged(sender As Object, e As System.Windows.Controls.TextChangedEventArgs) Handles txtReaderInput.TextChanged

        Dim CharCount As Integer = 0
        'Dim intTotalCharCount As Integer = 0
        Dim objSwipeScanInfo As New SwipeScanInfo
        Dim objSwipeScanDetail As New SwipeScanDetail
        Dim strTrackData As String = txtReaderInput.Text

        Try

            For Each c As Char In strTrackData

                If c = "?" Then CharCount += 1
                'If c = "%" Then Me.edtMicrText.Text = "Reading Card Data"

            Next


            If CharCount > 1 Then

                'strTrackData = txtReaderInput.Text
                Me.Dispatcher.BeginInvoke(New StartTheInvoke_OnDataReceived(AddressOf NowStartTheInvoke_OnDataReceived), strTrackData)
                Me.cmdCapture.Visibility = Windows.Visibility.Visible
                'WinMain.MyAppLog.WriteToLog("IVS", strTrackData, EventLogEntryType.Information)
                Me.txtReaderInput.Text = Nothing
                'Else
                'WinMain.MyAppLog.WriteToLog("strTrackData:" & strTrackData)
                'WinMain.MyAppLog.WriteToLog("CharCount:" & CharCount)

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    'ERRORS
    Private Sub CallbackProcProcessError(errResult As ErrorCode)
        If m_objDriverControl Is Nothing Then
            Return
        End If

        Dim eStatus As New ASB()
        ' Display message box correspondent to error
        Select Case errResult
            Case ErrorCode.ERR_COVER_OPEN
                Do
                    MessageBox.Show(ConstComStr.MSG_01_001, ConstComStr.CAPTION_01_001, MessageBoxButtons.OK, MessageBoxIcon.[Error])

                    m_objDriverControl.GetDeviceStatus(eStatus)
                Loop While (eStatus And ASB.ASB_COVER_OPEN) = ASB.ASB_COVER_OPEN
                m_objDriverControl.CancelError()
                Exit Select

            Case ErrorCode.ERR_PAPER_JAM
                If m_objConfigData.nRadio_ScanningMedia = ConstComVal.VAL_CONFIGDLG_RADIO_SM_CHECKPAPER Then
                    Do
                        MessageBox.Show(ConstComStr.MSG_02_000, ConstComStr.CAPTION_02_000, MessageBoxButtons.OK, MessageBoxIcon.[Error])

                        m_objDriverControl.GetDeviceStatus(eStatus)
                    Loop While ((eStatus And ASB.ASB_EJECT_SENSOR_NO_PAPER) <> ASB.ASB_EJECT_SENSOR_NO_PAPER) OrElse ((eStatus And ASB.ASB_SLIP_PAPER_SIZE) <> ASB.ASB_SLIP_PAPER_SIZE) OrElse ((eStatus And ASB.ASB_PAPER_INTERMEDIATE) <> ASB.ASB_PAPER_INTERMEDIATE)
                    m_objDriverControl.CancelError()
                Else
                    MessageBox.Show(ConstComStr.MSG_02_000, ConstComStr.CAPTION_02_000, MessageBoxButtons.OK, MessageBoxIcon.[Error])
                    m_objDriverControl.CancelError()
                End If
                Exit Select

            Case ErrorCode.ERR_MICR_BADDATA, ErrorCode.ERR_MICR_NODATA
                Exit Select
            Case Else

                ShowErrorMessage(errResult)
                Exit Select
        End Select
    End Sub

    'IMAGES
    Private Sub UpdateImageData(eFace As ScanSide, eImage As ImageType, bFourSheetScan As Boolean)
        If m_objDriverControl Is Nothing Then
            Return
        End If

        Dim cImageResult As New CImageResult()

        ' Obtain image data
        cImageResult.SetFace(eFace)
        cImageResult.SetGradation(eImage)
        cImageResult.SetFormat(Format.EPS_BI_SCN_BITMAP)
        Dim errResult As ErrorCode = ErrorCode.SUCCESS

        errResult = m_objDriverControl.GetScanImage(cImageResult)

        CallbackDisplayImageData(errResult, cImageResult, bFourSheetScan)
    End Sub

    Private Sub CallbackProcImage()
        If m_objDriverControl Is Nothing Then
            Return
        End If
        ' Configure display data for each scanning side and light source
        If m_objConfigData.nRadio_ImageChannel = ConstComVal.VAL_CONFIGDLG_RADIO_IC_RGB Then
            ' Configure 2 plane data for RGB image
            UpdateImageData(ScanSide.MF_SCAN_FACE_FRONT, ImageType.MF_SCAN_IMAGE_VISIBLE, False)
            UpdateImageData(ScanSide.MF_SCAN_FACE_BACK, ImageType.MF_SCAN_IMAGE_VISIBLE, False)

            ' Save image if necessary
            'TextBox1.Dispatcher.BeginInvoke(New StartTheInvoke_TextBox1(AddressOf NowStartTheInvoke_TextBox1), m_objConfigData.bCheck_Scan_SaveFile.ToString)

            'If m_objConfigData.bCheck_Scan_SaveFile = ConstComVal.VAL_CONFIGDLG_CHECK_MI_SAVEFILE_ON Then
            '    SaveImageData(ScanSide.MF_SCAN_FACE_FRONT, ImageType.MF_SCAN_IMAGE_VISIBLE)
            '    SaveImageData(ScanSide.MF_SCAN_FACE_BACK, ImageType.MF_SCAN_IMAGE_VISIBLE)
            'End If
        ElseIf m_objConfigData.nRadio_ImageChannel = ConstComVal.VAL_CONFIGDLG_RADIO_IC_IR Then
            ' Configure 2 plane data for IR image
            UpdateImageData(ScanSide.MF_SCAN_FACE_FRONT, ImageType.MF_SCAN_IMAGE_INFRARED, False)
            UpdateImageData(ScanSide.MF_SCAN_FACE_BACK, ImageType.MF_SCAN_IMAGE_INFRARED, False)

            ' Save image if necessary
            'If m_objConfigData.bCheck_Scan_SaveFile = ConstComVal.VAL_CONFIGDLG_CHECK_MI_SAVEFILE_ON Then
            '    SaveImageData(ScanSide.MF_SCAN_FACE_FRONT, ImageType.MF_SCAN_IMAGE_INFRARED)
            '    SaveImageData(ScanSide.MF_SCAN_FACE_BACK, ImageType.MF_SCAN_IMAGE_INFRARED)
            'End If
        ElseIf m_objConfigData.nRadio_ImageChannel = ConstComVal.VAL_CONFIGDLG_RADIO_IC_RGBIR Then
            ' Configure 4 plane image
            UpdateImageData(ScanSide.MF_SCAN_FACE_FRONT, ImageType.MF_SCAN_IMAGE_VISIBLE, True)
            UpdateImageData(ScanSide.MF_SCAN_FACE_BACK, ImageType.MF_SCAN_IMAGE_VISIBLE, True)
            UpdateImageData(ScanSide.MF_SCAN_FACE_FRONT, ImageType.MF_SCAN_IMAGE_INFRARED, True)
            UpdateImageData(ScanSide.MF_SCAN_FACE_BACK, ImageType.MF_SCAN_IMAGE_INFRARED, True)

            ' Save image if necessary
            'If m_objConfigData.bCheck_Scan_SaveFile = ConstComVal.VAL_CONFIGDLG_CHECK_MI_SAVEFILE_ON Then
            '    SaveImageData(ScanSide.MF_SCAN_FACE_FRONT, ImageType.MF_SCAN_IMAGE_VISIBLE)
            '    SaveImageData(ScanSide.MF_SCAN_FACE_BACK, ImageType.MF_SCAN_IMAGE_VISIBLE)
            '    SaveImageData(ScanSide.MF_SCAN_FACE_FRONT, ImageType.MF_SCAN_IMAGE_INFRARED)
            '    SaveImageData(ScanSide.MF_SCAN_FACE_BACK, ImageType.MF_SCAN_IMAGE_INFRARED)
            'End If
        End If
    End Sub

    Private Sub CallbackDisplayImageData(errResult As ErrorCode, cImageResult As CImageResult, bFourSheetScan As Boolean)

        If (Me.Dispatcher.CheckAccess()) Then
            If errResult <> ErrorCode.SUCCESS Then
                ShowErrorMessage(errResult)
                Return
            End If

            Dim cBitmap As New Bitmap(cImageResult.GetBitmapImage())
            If cBitmap IsNot Nothing Then
                cImageResult.FreeImage()
            End If

            If cImageResult.GetFace() = ScanSide.MF_SCAN_FACE_FRONT Then

                m_biImage2Front = New Bitmap(cBitmap)
                'System.Windows.MessageBox.Show("m_biImage2Front")
                Me.imgScannedDocument.Source = IVS.Epson.MyImage.GetBitmapSource(m_biImage2Front)
                ' m_test += 1
                Dim sb As New StringBuilder
                Dim objImageDetail As New IVS.CTS.ImageDetail

                sb.Append(WinMain.objClientSettings.ImageLocation)
                sb.Append("\")
                sb.Append(intSwipeScanID)
                sb.Append("f.jpg")

                objImageDetail.Image = m_biImage2Front
                objImageDetail.FileName = sb.ToString

                ThreadPool.QueueUserWorkItem(AddressOf CTSApi.SaveImage, objImageDetail)
                DataAccess.UpdateImageAvailable(intSwipeScanID)

                Me.cmdBrightnessAdd.IsEnabled = True
                Me.cmdBrightnessSubtract.IsEnabled = True
                Me.cmdRotate.IsEnabled = True
                'Me.cmdSave.IsEnabled = True

                strDocDisplayedSide = "F"

            ElseIf cImageResult.GetFace = ScanSide.MF_SCAN_FACE_BACK Then

                m_biImage2Back = New Bitmap(cBitmap)
                'm_test += 1

                Dim sb As New StringBuilder
                Dim objImageDetail As New IVS.CTS.ImageDetail

                sb.Append(WinMain.objClientSettings.ImageLocation)
                sb.Append("\")
                sb.Append(intSwipeScanID)
                sb.Append("b.jpg")

                objImageDetail.Image = m_biImage2Back
                objImageDetail.FileName = sb.ToString

                ThreadPool.QueueUserWorkItem(AddressOf CTSApi.SaveImage, objImageDetail)

            End If

            cBitmap.Dispose()
            isViewTimeCountingDown = True
          
        Else

            Me.Dispatcher.BeginInvoke(New StartTheInvoke_stImage2Front(AddressOf NowStartTheInvoke_stImage2Front), errResult, cImageResult, bFourSheetScan)

        End If
    End Sub

    Delegate Sub StartTheInvoke_stImage2Front(ByVal errResult As ErrorCode, ByVal cImageResult As CImageResult, ByVal bFourSheetScan As Boolean)

    Friend Sub NowStartTheInvoke_stImage2Front(ByVal errResult As ErrorCode, ByVal cImageResult As CImageResult, ByVal bFourSheetScan As Boolean)

        Try
            CallbackDisplayImageData(errResult, cImageResult, bFourSheetScan)
        Catch ex As Exception
            'WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub ClearImage()

        Me.imgScannedDocument.Source = Nothing

    End Sub

    Private Sub ShowErrorMessage(errResult As ErrorCode)
        Select Case errResult
            Case ErrorCode.ERR_TYPE
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_000, ConstComStr.CAPTION_03_000, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_OPENED
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_001, ConstComStr.CAPTION_03_001, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_NO_PRINTER
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_002, ConstComStr.CAPTION_03_002, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_NO_TARGET
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_003, ConstComStr.CAPTION_03_003, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_NO_MEMORY
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_004, ConstComStr.CAPTION_03_004, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_HANDLE
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_005, ConstComStr.CAPTION_03_005, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_TIMEOUT
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_006, ConstComStr.CAPTION_03_006, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_ACCESS
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_007, ConstComStr.CAPTION_03_007, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_PARAM
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_008, ConstComStr.CAPTION_03_008, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_NOT_SUPPORT
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_009, ConstComStr.CAPTION_03_009, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_OFFLINE
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_010, ConstComStr.CAPTION_03_010, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_NOT_EPSON
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_011, ConstComStr.CAPTION_03_011, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_WITHOUT_CB
                If m_bScanCancelError = True Then
                    System.Windows.MessageBox.Show(ConstComStr.MSG_03_012_01, ConstComStr.CAPTION_03_012, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    m_bScanCancelError = False
                Else
                    System.Windows.MessageBox.Show(ConstComStr.MSG_03_012_00, ConstComStr.CAPTION_03_012, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                Exit Select
            Case ErrorCode.ERR_BUFFER_OVER_FLOW
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_013, ConstComStr.CAPTION_03_013, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_REGISTRY
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_014, ConstComStr.CAPTION_03_014, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_PAPERINSERT_TIMEOUT
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_015, ConstComStr.CAPTION_03_015, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_EXEC_FUNCTION
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_016, ConstComStr.CAPTION_03_016, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_EXEC_MICR
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_017, ConstComStr.CAPTION_03_017, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_EXEC_SCAN
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_018, ConstComStr.CAPTION_03_018, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_RESET
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_019, ConstComStr.CAPTION_03_019, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_ABORT
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_020, ConstComStr.CAPTION_03_020, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_MICR
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_021, ConstComStr.CAPTION_03_021, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_SCAN
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_022, ConstComStr.CAPTION_03_022, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_LINE_OVERFLOW
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_023, ConstComStr.CAPTION_03_023, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_PAPER_PILED
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_024, ConstComStr.CAPTION_03_024, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_PAPER_JAM
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_025, ConstComStr.CAPTION_03_025, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_COVER_OPEN
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_026, ConstComStr.CAPTION_03_026, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_MICR_NODATA
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_027, ConstComStr.CAPTION_03_027, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_MICR_BADDATA
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_028, ConstComStr.CAPTION_03_028, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_MICR_NOISE
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_029, ConstComStr.CAPTION_03_029, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_SCN_COMPRESS
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_030, ConstComStr.CAPTION_03_030, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_PAPER_EXIST
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_031, ConstComStr.CAPTION_03_031, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_PAPER_INSERT
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_032, ConstComStr.CAPTION_03_032, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_EXEC_SCAN_CHECK_CONTINUOUS
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_033, ConstComStr.CAPTION_03_033, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_EXEC_SCAN_CHECK_ONEBYONE
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_034, ConstComStr.CAPTION_03_034, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_EXEC_SCAN_IDCARD
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_035, ConstComStr.CAPTION_03_035, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_EXEC_PRINT_ROLLPAPER
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_036, ConstComStr.CAPTION_03_036, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_EXEC_PRINT_VALIDATION
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_037, ConstComStr.CAPTION_03_037, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_THREAD
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_038, ConstComStr.CAPTION_03_038, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_IMAGE_FILEOPEN
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_039, ConstComStr.CAPTION_03_039, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_IMAGE_UNKNOWNFORMAT
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_040, ConstComStr.CAPTION_03_040, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_SIZE
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_041, ConstComStr.CAPTION_03_041, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_NOT_FOUND
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_042, ConstComStr.CAPTION_03_042, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_NOT_EXEC
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_043, ConstComStr.CAPTION_03_043, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_BARCODE_NODATA
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_044, ConstComStr.CAPTION_03_044, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_MICR_PARSE
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_045, ConstComStr.CAPTION_03_045, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_SCN_IQA
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_046, ConstComStr.CAPTION_03_046, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_PRINT_DATA_LENGTH_EXCEED
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_047, ConstComStr.CAPTION_03_047, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_PRINT_DATA_UNRECEIVE
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_048, ConstComStr.CAPTION_03_048, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_IMAGE_FILEREAD
                System.Windows.MessageBox.Show(ConstComStr.MSG_03_049, ConstComStr.CAPTION_03_049, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case Else
                System.Windows.MessageBox.Show(errResult.ToString(), "ERR", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
        End Select
    End Sub

#End Region

    Private Sub cmdCapture_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCapture.Click

        'Dim objImageDetail As New IVS.CTS.ImageDetail

        Try
            isViewTimeCountingDown = False

            If WinMain.objClientSettings.DeviceType = "M210/260" Then

                MyM280.btnCapture_Click()
                isViewTimeCountingDown = True

            ElseIf WinMain.objClientSettings.DeviceType = "TM-S9000" Then

                If m_objDriverControl Is Nothing Then
                    Return
                End If

                ' ClearData()

                Dim objCARDScanParam As New CScanParam
                objCARDScanParam.SetScanMedia(ScanUnit.EPS_BI_SCN_UNIT_CARD)
                objCARDScanParam.SetRGBColorDepth(com.epson.bank.driver.ColorDepth.EPS_BI_SCN_24BIT)
                objCARDScanParam.SetResolution(MfScanDpi.MF_SCAN_DPI_600)
                objCARDScanParam.SetIRColorDepth(com.epson.bank.driver.ColorDepth.EPS_BI_SCN_24BIT)
                objCARDScanParam.SetImageChannel(ImageTypeOption.EPS_BI_SCN_OPTION_COLOR)

                m_objDriverControl.SetScanParam(objCARDScanParam)

                Me.imgScannedDocument.Width = 290

                Dim errRet As ErrorCode = m_objDriverControl.ScanCard()
                If errRet <> ErrorCode.SUCCESS Then
                    ShowErrorMessage(errRet)
                    m_objDriverControl.CancelError()
                    Return
                End If

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("WinCTS.cmdViewDocument_Click()" & ex.ToString)
        End Try

    End Sub

    Private Sub cmdSaveScanFront_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdSaveScanFront.Click

        Dim objImageDetail As New IVS.CTS.ImageDetail

        Try
            isViewTimeCountingDown = False

            Me.cmdSaveScanFront.Visibility = Windows.Visibility.Hidden
            Me.cmdSaveScanBack.Visibility = Windows.Visibility.Hidden

            objImageDetail.Image = imgEditedImage
            objImageDetail.FileName = WinMain.objClientSettings.ImageLocation & "\" & intSwipeScanID & "F.jpg"
            CTSApi.SaveImage(objImageDetail)

            isImageEdited = False
            strDocDisplayedSide = "F"

            Me.cmdBrightnessAdd.IsEnabled = True
            Me.cmdBrightnessSubtract.IsEnabled = True
            Me.cmdRotate.IsEnabled = True

            isViewTimeCountingDown = True

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("WinCTS.cmdViewDocument_Click()" & ex.ToString)
        End Try

    End Sub

    Private Sub cmdSaveScanBack_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdSaveScanBack.Click

        Dim objImageDetail As New IVS.CTS.ImageDetail

        Try
            isViewTimeCountingDown = False

            Me.cmdSaveScanFront.Visibility = Windows.Visibility.Hidden
            Me.cmdSaveScanBack.Visibility = Windows.Visibility.Hidden

            objImageDetail.Image = imgEditedImage
            objImageDetail.FileName = WinMain.objClientSettings.ImageLocation & "\" & intSwipeScanID & "B.jpg"
            CTSApi.SaveImage(objImageDetail)

            isImageEdited = False
            strDocDisplayedSide = "B"

            Me.cmdBrightnessAdd.IsEnabled = True
            Me.cmdBrightnessSubtract.IsEnabled = True
            Me.cmdRotate.IsEnabled = True

            isViewTimeCountingDown = True

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("WinCTS.cmdViewDocument_Click()" & ex.ToString)
        End Try

    End Sub

End Class