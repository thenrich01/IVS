Imports LsFamily
Imports System.ComponentModel
Imports System.IO
Imports System.Drawing
Imports System.Windows.Threading
Imports System.Windows.Forms
Imports System.Printing
Imports System.Threading
Imports System.Text
Imports System.Security.Principal
Imports IVS.Data
Imports IVS.CTS
Imports IVS.AppLog
Imports IVS.Eseek
Imports IVS.Eseek.M280
Imports IVS.Data.WS.TEP.IVSService
Imports IVS.Data.WS.TEP

Public Class WinScanImage

    Private MyAppLog As New ApplicationLog(ApplicationLog.DateEncodingType.YYYYMMDD, True)
    Private WithEvents timerViewTime As DispatcherTimer = New DispatcherTimer()
    Private objClientSettings As New ClientSettings
    Private intIVSUserID As Integer
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

    Public Sub New(ByVal IVSUserID As Integer, Optional ByVal SwipeScanID As Integer = 0)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Dim intClientID As Integer

        Try
            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner

            intClientID = DataAccess.GetClientID()
            objClientSettings = DataAccess.GetClientSettings(intClientID)

            EnableCTSCommands(False)

            System.Windows.MessageBox.Show(objClientSettings.DeviceType, "New", MessageBoxButton.OK)

            Select Case objClientSettings.DeviceType

                Case "LS_40_USB"

                    CTSDeviceType = LsDefines.LsUnitType.LS_40_USB

                    Dim MyCTSObject As New CTSApi()
                    AddHandler MyCTSObject.BWIdentifyCompleted, AddressOf BWCTSIdentify_RunWorkerCompleted
                    MyCTSObject.BWCTSIdentify_Start(CTSDeviceType)

                Case "LS_150_USB"

                    CTSDeviceType = LsDefines.LsUnitType.LS_150_USB

                    Dim MyCTSObject As New CTSApi()
                    AddHandler MyCTSObject.BWIdentifyCompleted, AddressOf BWCTSIdentify_RunWorkerCompleted
                    MyCTSObject.BWCTSIdentify_Start(CTSDeviceType)

                Case "M210/260"

                    MyM280 = New IVS.Eseek.M280.ESeekM280Api
                    Me.timCheckStatus.Interval = 100
                    Me.timCheckStatus.Start()

                    Dim isSerialPortOpen As Boolean = False

                    objSerialPort = New ESeekApi(objClientSettings.ComPort, objClientSettings.SleepMilliSeconds)
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

                    Me.cmdCapture.Visibility = Windows.Visibility.Visible

            End Select

            intIVSUserID = IVSUserID

            intSwipeScanToDisplay = SwipeScanID

            If intSwipeScanToDisplay > 0 Then

                DataSetSwipeScans_Navigate("Position", intSwipeScanToDisplay)
            Else

                If objClientSettings.DisableDBSave = False Then
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

            Me.lblLocation.Content = objClientSettings.Location
            Me.lblStation.Content = objClientSettings.Station
            Me.lblUserName.Content = DataAccess.GetUserName(intIVSUserID)

            If WinMain.isUserAdmin = False Then
                Me.cmdRawData.IsEnabled = False
            End If

            If WinMain.isUserAlertAble = False Then
                Me.cmdNewAlert.IsEnabled = False
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
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
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub WinCTS_KeyDown() Handles Me.KeyDown, Me.MouseDown
        intViewTimeCountDown = objClientSettings.ViewingTime
    End Sub

    Private Sub WinCTS_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Me.Closing

        Try
            timerViewTime.Stop()

            Select Case objClientSettings.DeviceType

                Case "M200/250"

                    If objSerialPort IsNot Nothing Then
                        objSerialPort.SerialPortClose()
                    End If


            End Select





        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
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
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgSwipeScanHistory_UnloadingRow(sender As Object, e As System.Windows.Controls.DataGridRowEventArgs) Handles dgSwipeScanHistory.UnloadingRow

        Try
            e.Row.Background = New SolidColorBrush(Colors.White)
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgSwipeScanHistory_SelectionChanged(sender As Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgSwipeScanHistory.SelectionChanged

        Dim dgSwipeScanHistory_SelectedRow As System.Collections.IList

        Try
            dgSwipeScanHistory_SelectedRow = e.AddedItems

            If dgSwipeScanHistory_SelectedRow.Count > 0 Then

                intSwipeScanToDisplay = dgSwipeScanHistory_SelectedRow(0).SwipeScanID

            End If

            intViewTimeCountDown = objClientSettings.ViewingTime
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgSwipeScanHistory_CmdView_Click()

        Try
            DataSetSwipeScans_Navigate("Position", intSwipeScanToDisplay)

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub dgAlerts_SelectionChanged(sender As Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgAlerts.SelectionChanged

        Dim dgAlerts_SelectedRow As System.Collections.IList

        Try
            dgAlerts_SelectedRow = e.AddedItems

            If dgAlerts_SelectedRow.Count > 0 Then
                intAlertID = dgAlerts_SelectedRow(0).AlertID
            End If

            intViewTimeCountDown = objClientSettings.ViewingTime

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
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

            intViewTimeCountDown = objClientSettings.ViewingTime
            isViewTimeCountingDown = True

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdNavLast_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdNavLast.Click

        Try
            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save updates before scanning?", "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

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
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdNavNext_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdNavNext.Click

        Try
            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save updates before scanning?", "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

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
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdNavPrevious_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdNavPrevious.Click

        Try
            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save updates before scanning?", "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

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
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdCTSScanID_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCTSScanID.Click

        Dim objCTSApi As New CTSApi

        Try
            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save updates before scanning?", "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

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
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdCTSReadMagStripe_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCTSReadMagStripe.Click

        Dim objCTSApi As New CTSApi

        Try
            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save updates before scanning?", "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

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
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdCTSScanCheck_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCTSScanCheck.Click

        Dim objCTSApi As New CTSApi

        Try
            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save updates before scanning?", "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

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
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdCTSResetHW_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCTSResetHW.Click

        Dim objCTSApi As New CTSApi

        Try

            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save updates before reset?", "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

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
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
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

                    WinViewImage = New WinView(intSwipeScanID, strSwipeScanType, objClientSettings.ImageLocation, objClientSettings.ViewingTime)
                    WinViewImage.Owner = Me
                    WinViewImage.ShowDialog()

                    isViewTimeCountingDown = True

                Catch ex As Exception
                    MyAppLog.WriteToLog("WinCTS.cmdViewDocument_Click()" & ex.ToString)
                End Try

            End If

            If e.ChangedButton = MouseButton.Right Then
                Dim imgDocToDisplay As New BitmapImage

                Try
                    imgDocToDisplay.BeginInit()

                    Select Case strDocDisplayedSide

                        Case "F"

                            If File.Exists(objClientSettings.ImageLocation & "\" & intSwipeScanID & "b.jpg") = True Then

                                imgEditedImage = New Bitmap(objClientSettings.ImageLocation & "\" & intSwipeScanID & "b.jpg")
                                imgDocToDisplay.UriSource = New Uri(objClientSettings.ImageLocation & "\" & intSwipeScanID & "b.jpg", UriKind.Absolute)

                            Else

                                imgDocToDisplay.UriSource = New Uri("./Resources/NoImage.jpg", UriKind.Relative)

                            End If

                            strDocDisplayedSide = "B"

                        Case "B"

                            If File.Exists(objClientSettings.ImageLocation & "\" & intSwipeScanID & "f.jpg") = True Then

                                imgEditedImage = New Bitmap(objClientSettings.ImageLocation & "\" & intSwipeScanID & "f.jpg")
                                imgDocToDisplay.UriSource = New Uri(objClientSettings.ImageLocation & "\" & intSwipeScanID & "f.jpg", UriKind.Absolute)

                            Else

                                imgDocToDisplay.UriSource = New Uri("./Resources/NoImage.jpg", UriKind.Relative)

                            End If

                            strDocDisplayedSide = "F"

                    End Select

                    imgDocToDisplay.EndInit()

                    Me.imgScannedDocument.Source = imgDocToDisplay
                    isViewTimeCountingDown = False

                Catch ex As Exception
                    MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
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
            intViewTimeCountDown = objClientSettings.ViewingTime

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdBrightnessSubtract_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdBrightnessSubtract.Click

        Try
            imgEditedImage = CTSApi.AdjustBitmapBrightness(imgEditedImage, -10)
            objNewBitmapSource = CTSApi.GetBitmapSource(imgEditedImage)
            Me.imgScannedDocument.Source = objNewBitmapSource

            isImageEdited = True
            Me.cmdSave.IsEnabled = True
            intViewTimeCountDown = objClientSettings.ViewingTime

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdRotate_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdRotate.Click

        Try
            imgEditedImage = CTSApi.RotateBitmap(imgEditedImage)
            objNewBitmapSource = CTSApi.GetBitmapSource(imgEditedImage)
            Me.imgScannedDocument.Source = objNewBitmapSource

            isImageEdited = True
            Me.cmdSave.IsEnabled = True
            intViewTimeCountDown = objClientSettings.ViewingTime

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
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

            intViewTimeCountDown = objClientSettings.ViewingTime

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdCompare_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCompare.Click

        Dim strSwipeScanType As String
        Dim winCompareScans As WinCompare

        Try
            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save updates before comparing the last 2 scans?", "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

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

            winCompareScans = New WinCompare(intSwipeScanID, strSwipeScanType, intIVSUserID, objClientSettings.ClientID, objClientSettings.ViewingTime)
            winCompareScans.Owner = Me
            winCompareScans.ShowDialog()

            isViewTimeCountingDown = True

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdRawData_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdRawData.Click

        Dim winViewRawData As WinRawData

        Try
            isViewTimeCountingDown = False

            winViewRawData = New WinRawData(strSwipeRawData)
            winViewRawData.Owner = Me
            winViewRawData.ShowDialog()

            intViewTimeCountDown = objClientSettings.ViewingTime
            isViewTimeCountingDown = True

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
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

                If System.Windows.MessageBox.Show("Save updates before entering an alert?", "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

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
            objAlertDetail.AlertContactName = DataAccess.GetUserName(intIVSUserID)
            objAlertDetail.AlertContactNumber = DataAccess.GetUserPhone(intIVSUserID)
            objAlertDetail.UserID = intIVSUserID

            isViewTimeCountingDown = False


            WinNewAlert = New WinAlert(objAlertDetail, intIVSUserID)
            WinNewAlert.Owner = Me

            DialogResult = WinNewAlert.ShowDialog

            If DialogResult = True Then

                SwipeScanAlerts_Load(strDocumentID, strNameFirst, strNameLast)

            End If
            isViewTimeCountingDown = True

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdClose_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdClose.Click

        Try
            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save updates before closing?", "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

                    If isImageEdited = True Then
                        SaveImage()
                    End If

                    'If isRecordEdited = True Then
                    '    SaveData()
                    'End If

                End If

            End If

            Me.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub timerViewTime_Tick(sender As Object, e As System.EventArgs) Handles timerViewTime.Tick

        Try

            If isViewTimeCountingDown = True Then

                If intViewTimeCountDown > 0 Then

                    intViewTimeCountDown -= 1

                Else
                    ClearData()
                End If

            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdClear_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdClear.Click

        Try
            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save updates before clearing the data?", "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

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
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdManualEntry_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdManualEntry.Click

        Dim winManualEntry As WinManEntry

        Try
            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save updates before manual data entry?", "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

                    If isImageEdited = True Then
                        SaveImage()
                    End If

                    'If isRecordEdited = True Then
                    '    SaveData()
                    'End If

                End If

            End If

            isViewTimeCountingDown = False

            winManualEntry = New WinManEntry(intIVSUserID, objClientSettings.ClientID, "CTS")
            winManualEntry.Owner = Me
            winManualEntry.ShowDialog()

            DataSetSwipeScans_Navigate("Last")

            intViewTimeCountDown = objClientSettings.ViewingTime
            isViewTimeCountingDown = True

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdPrint_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdPrint.Click

        'Try
        '    Dim dlgPrint As System.Windows.Controls.PrintDialog = New System.Windows.Controls.PrintDialog

        '    If dlgPrint.ShowDialog().GetValueOrDefault(True) Then

        '        dlgPrint.PrintQueue = LocalPrintServer.GetDefaultPrintQueue
        '        dlgPrint.PrintTicket.CopyCount = 1
        '        dlgPrint.PrintTicket.PageOrientation = PageOrientation.Landscape
        '        dlgPrint.PrintVisual(Me, Me.Title)

        '    End If

        'Catch ex As Exception
        '    MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        'End Try

    End Sub

    Private Sub txtCaseID_TextChanged(sender As Object, e As System.Windows.Controls.TextChangedEventArgs) Handles txtCaseID.TextChanged
        Try
            Me.cmdSave.IsEnabled = True
            isRecordEdited = True
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try
    End Sub

    Private Sub txtDateOfExpiration_TextChanged(sender As Object, e As System.Windows.Controls.TextChangedEventArgs) Handles txtDateOfExpiration.TextChanged
        Try
            Me.cmdSave.IsEnabled = True
            isRecordEdited = True
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try
    End Sub

    Private Sub cbIsUCCScan_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cbIsUCCScan.Click
        Try
            Me.cmdSave.IsEnabled = True
            isRecordEdited = True
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try
    End Sub

    Private Sub cmdCapture_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCapture.Click

        Try
            isViewTimeCountingDown = False

            MyM280.btnCapture_Click()

            isViewTimeCountingDown = True

        Catch ex As Exception
            MyAppLog.WriteToLog("WinCTS.cmdViewDocument_Click()" & ex.ToString)
        End Try

    End Sub

#End Region

#Region "Custom Subs/Functions"

    Private Sub SaveImage()

        Dim objImageDetail As New IVS.CTS.ImageDetail

        Try

            objImageDetail.Image = imgEditedImage
            objImageDetail.FileName = objClientSettings.ImageLocation & "\" & intSwipeScanID & strDocDisplayedSide & ".jpg"
            CTSApi.SaveImage(objImageDetail)

            isImageEdited = False
            Me.cmdSave.IsEnabled = False
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub SaveData()

        'Dim strDateOfExpiration As String

        'Try
        '    If Me.txtDateOfExpiration.Text <> "" Then
        '        strDateOfExpiration = Me.txtDateOfExpiration.Text
        '    ElseIf Me.lblDateOfExpiration.Content <> "" Then
        '        strDateOfExpiration = Me.lblDateOfExpiration.Content
        '    End If
        '    DataAccess.UpdateUCCID(intSwipeScanID, Me.txtCaseID.Text, Me.cbIsUCCScan.IsChecked, strDateOfExpiration, Me.LblIDType.Content)
        '    Me.lblDateOfExpiration.Content = Me.txtDateOfExpiration.Text
        '    Me.lblDateOfExpiration.Visibility = Windows.Visibility.Visible
        '    Me.txtDateOfExpiration.Visibility = Windows.Visibility.Hidden
        '    Me.cmdSave.IsEnabled = False

        'Catch ex As Exception
        '    MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        'End Try

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
                If IsDate(SwipeScanDetailObject.DateOfExpiration) = True Then

                    If SwipeScanDetailObject.DateOfExpiration <> "unknown" And SwipeScanDetailObject.DateOfExpiration <> "" Then

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

                    ElseIf SwipeScanDetailObject.DateOfExpiration = "unknown" Or SwipeScanDetailObject.DateOfExpiration = "" Then

                        Me.lblDateOfExpiration.Visibility = Windows.Visibility.Hidden
                        Me.txtDateOfExpiration.Visibility = Windows.Visibility.Visible

                    End If

                End If

            End If

            If String.IsNullOrEmpty(SwipeScanDetailObject.Age) = False Then
                If IsNumeric(SwipeScanDetailObject.Age) = True Then

                    If SwipeScanDetailObject.Age < objClientSettings.Age And SwipeScanDetailObject.Age > 0 Then

                        If objClientSettings.AgeHighlight = True Then
                            Me.lblAge.Foreground = System.Windows.Media.Brushes.Red
                            Me.lblAge.Background = System.Windows.Media.Brushes.Yellow
                            Me.lblAge.FontWeight = FontWeights.Bold
                            Me.lblAge.FontSize = 19

                        End If

                        If objClientSettings.AgePopup = True Then

                            Dim WinUnderAge As WinUnderAge

                            WinUnderAge = New WinUnderAge(SwipeScanDetailObject.NameFirst & " " & SwipeScanDetailObject.NameLast, SwipeScanDetailObject.Age, objClientSettings.Age)
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

            If FrontImage IsNot Nothing Or File.Exists(objClientSettings.ImageLocation & "\" & SwipeScanDetailObject.SwipeScanID & "f.jpg") Then

                If FrontImage IsNot Nothing Then

                    Me.imgEditedImage = FrontImage

                ElseIf File.Exists(objClientSettings.ImageLocation & "\" & SwipeScanDetailObject.SwipeScanID & "f.jpg") Then

                    imgEditedImage = New Bitmap(objClientSettings.ImageLocation & "\" & SwipeScanDetailObject.SwipeScanID & "f.jpg")

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

            End If

            If SwipeScanDetailObject.CardType = "Drivers License Or State ID" Or SwipeScanDetailObject.CardType = "Military ID Card" Then

                'Me.cbIsUCCScan.IsEnabled = True
                'Me.cbIsUCCScan.IsChecked = SwipeScanDetailObject.IsUCCScan

            End If

            Me.txtCaseID.Text = SwipeScanDetailObject.CaseID

            Me.lblSwipeScanUserName.Content = SwipeScanDetailObject.UserName
            Me.lblSwipeScanLocation.Content = SwipeScanDetailObject.Location

            SwipeScanHistory_Load(SwipeScanDetailObject.IDAccountNumber, SwipeScanDetailObject.CardType)
            SwipeScanAlerts_Load(SwipeScanDetailObject.IDAccountNumber, SwipeScanDetailObject.NameFirst, SwipeScanDetailObject.NameLast)

            If objClientSettings.ImageSave = True Then

                Dim sb As New StringBuilder
                Dim objImageDetail As New IVS.CTS.ImageDetail

                sb.Append(objClientSettings.ImageLocation)
                sb.Append("\")
                sb.Append(intSwipeScanID)
                sb.Append("f.jpg")

                objImageDetail.Image = FrontImage
                objImageDetail.FileName = sb.ToString

                CTSApi.SaveImage(objImageDetail)
                DataAccess.UpdateImageAvailable(intSwipeScanID)

                sb.Clear()
                objImageDetail = New IVS.CTS.ImageDetail

                sb.Append(objClientSettings.ImageLocation)
                sb.Append("\")
                sb.Append(intSwipeScanID)
                sb.Append("b.jpg")

                objImageDetail = New IVS.CTS.ImageDetail
                objImageDetail.Image = BackImage
                objImageDetail.FileName = sb.ToString

                ThreadPool.QueueUserWorkItem(AddressOf CTSApi.SaveImage, objImageDetail)

                If File.Exists(objClientSettings.ImageLocation & "\" & intSwipeScanID & "f.jpg") = True Then
                    imgEditedImage = New Bitmap(objClientSettings.ImageLocation & "\" & intSwipeScanID & "f.jpg")
                End If

            End If

            AddHandler txtDateOfExpiration.TextChanged, AddressOf txtDateOfExpiration_TextChanged
            AddHandler cbIsUCCScan.Click, AddressOf cbIsUCCScan_Click
            AddHandler txtCaseID.TextChanged, AddressOf txtCaseID_TextChanged

            intViewTimeCountDown = objClientSettings.ViewingTime

            Me.cmdSave.IsEnabled = False

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub SwipeScanHistory_Load(ByVal IDAccountNumber As String, ByVal SwipeScanType As String)

        Try
            Me.dgSwipeScanHistory.ItemsSource = DataAccess.GetSwipeScanHistory(IDAccountNumber, SwipeScanType)
            Me.lblSwipeCount.Content = dgSwipeScanHistory.Items.Count
            intViewTimeCountDown = objClientSettings.ViewingTime
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub SwipeScanAlerts_Load(ByVal IDAccountNumber As String, ByVal NameFirst As String, ByVal NameLast As String)

        Try
            Me.dgAlerts.ItemsSource = DataAccess.GetSwipeScanAlerts(IDAccountNumber, NameFirst, NameLast)
            Me.lblAlertCount.Content = dgAlerts.Items.Count
            intViewTimeCountDown = objClientSettings.ViewingTime
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
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

                    objSwipeScanNavigateInfo = DataAccess.SwipeScanNavigateLast(intIVSUserID, objClientSettings.ClientID)

                Case "Next"

                    objSwipeScanNavigateInfo = DataAccess.SwipeScanNavigateNext(intIVSUserID, objClientSettings.ClientID, intSwipeScanID)

                Case "Previous"

                    objSwipeScanNavigateInfo = DataAccess.SwipeScanNavigatePrevious(intIVSUserID, objClientSettings.ClientID, intSwipeScanID)

                Case "First"

                    objSwipeScanNavigateInfo = DataAccess.SwipeScanNavigateFirst(intIVSUserID, objClientSettings.ClientID)

                Case "Position"

                    objSwipeScanNavigateInfo = DataAccess.SwipeScanNavigatePosition(Position)

            End Select

            intSwipeScanID = objSwipeScanNavigateInfo.SwipeScanID
            MyAppLog.WriteToLog("WinCTS.DataSetSwipeScans_Navigate() SwipeScanID:" & intSwipeScanID)

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

            intViewTimeCountDown = objClientSettings.ViewingTime

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub EnableCTSCommands(ByVal IsEnabled As Boolean)

        Try
            Me.cmdCTSReadMagStripe.IsEnabled = IsEnabled
            Me.cmdCTSResetHW.IsEnabled = IsEnabled
            Me.cmdCTSScanCheck.IsEnabled = IsEnabled
            Me.cmdCTSScanID.IsEnabled = IsEnabled

            intViewTimeCountDown = objClientSettings.ViewingTime

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
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
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

#Region "CTS"

    Delegate Sub StartTheInvoke_ReportStatus(ByVal Status As String)

    Friend Sub NowStartTheInvoke_ReportStatus(ByVal Status As String)

        Try
            Me.lblStatusCTS.Content = Status

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
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
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWScanDLIDImage_ProgressChanged(ByVal PercentChange As Integer)

        Try
            MyAppLog.WriteToLog("WinCTS.BWScanDLIDImage_ProgressChanged() ProgressPercentage:" & PercentChange)
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
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWScanDLIDImage_RunWorkerCompleted(ByVal Result As CTSSwipeScanInfo)

        MyAppLog.WriteToLog("WinCTS.BWScanDLIDImage_RunWorkerCompleted()")

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

                If System.Windows.MessageBox.Show("Save scanned image and read magnetic data?", "Unable to read ID data", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

                    Dim winMagEntry As WinMagEntry
                    winMagEntry = New WinMagEntry(intIVSUserID, objClientSettings)
                    winMagEntry.Owner = Me
                    winMagEntry.ShowDialog()

                    If winMagEntry.DialogResult = True Then

                        Me.lblStatusCTS.Content = "Succesful scan - Ready to scan again"

                        If objClientSettings.ImageSave = True Then

                            Dim sb As New StringBuilder
                            Dim objImageDetail As New IVS.CTS.ImageDetail

                            sb.Append(objClientSettings.ImageLocation)
                            sb.Append("\")
                            sb.Append(ManualEntrySwipeScanID)
                            sb.Append("f.jpg")

                            objImageDetail.Image = Result.ImageFront
                            objImageDetail.FileName = sb.ToString

                            CTSApi.SaveImage(objImageDetail)
                            DataAccess.UpdateImageAvailable(ManualEntrySwipeScanID)

                            sb.Clear()
                            objImageDetail = New IVS.CTS.ImageDetail

                            sb.Append(objClientSettings.ImageLocation)
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

                        If System.Windows.MessageBox.Show("Save scanned image and enter data manually?", "Unable to read ID data", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

                            Dim winManEntry As WinManEntry
                            winManEntry = New WinManEntry(intIVSUserID, objClientSettings.ClientID, "CTS")
                            winManEntry.Owner = Me
                            winManEntry.ShowDialog()

                            If winManEntry.DialogResult = True Then

                                Me.lblStatusCTS.Content = "Succesful scan - Ready to scan again"

                                If objClientSettings.ImageSave = True Then

                                    Dim sb As New StringBuilder
                                    Dim objImageDetail As New IVS.CTS.ImageDetail

                                    sb.Append(objClientSettings.ImageLocation)
                                    sb.Append("\")
                                    sb.Append(ManualEntrySwipeScanID)
                                    sb.Append("f.jpg")

                                    objImageDetail.Image = Result.ImageFront
                                    objImageDetail.FileName = sb.ToString

                                    CTSApi.SaveImage(objImageDetail)
                                    DataAccess.UpdateImageAvailable(ManualEntrySwipeScanID)

                                    sb.Clear()
                                    objImageDetail = New IVS.CTS.ImageDetail

                                    sb.Append(objClientSettings.ImageLocation)
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

                objSwipeScanInfo.ClientID = objClientSettings.ClientID
                objSwipeScanInfo.UserID = objClientSettings.DefaultUser
                objSwipeScanInfo.DisableDBSave = objClientSettings.DisableDBSave
                ' objSwipeScanInfo.IDChecker = Application.objMyIDChecker
                objSwipeScanInfo.SwipeScanRawData = Result.SwipeRawData

                objSwipeScanDetail = DataAccess.NewDataSwipeScan(objSwipeScanInfo)

                'SwipeScanDetail_LoadByObject(objSwipeScanDetail, Result.ImageFront, Result.ImageBack)
                SwipeScanDetailLoad(objSwipeScanDetail, Result.ImageFront, Result.ImageBack)

                intViewTimeCountDown = objClientSettings.ViewingTime
                isViewTimeCountingDown = True

            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWScanCheckImage_ProgressChanged(ByVal PercentChange As Integer)

        Try
            MyAppLog.WriteToLog("WinCTS.BWScanCheckImage_ProgressChanged() ProgressPercentage:" & PercentChange)
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
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWScanCheckImage_RunWorkerCompleted(ByVal Result As CTSSwipeScanInfo)

        MyAppLog.WriteToLog("WinCTS.BWScanCheckImage_RunWorkerCompleted()")

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

                objSwipeScanInfo.ClientID = objClientSettings.ClientID
                objSwipeScanInfo.UserID = objClientSettings.DefaultUser
                objSwipeScanInfo.DisableDBSave = objClientSettings.DisableDBSave
                'objSwipeScanInfo.IDChecker = Application.objMyIDChecker
                objSwipeScanInfo.SwipeScanRawData = Result.SwipeRawData
                objSwipeScanInfo.ScanType = "C"

                objSwipeScanDetail = DataAccess.NewDataSwipeScan(objSwipeScanInfo)

                'SwipeScanDetail_LoadByObject(objSwipeScanDetail, Result.ImageFront, Result.ImageBack)
                SwipeScanDetailLoad(objSwipeScanDetail, Result.ImageFront, Result.ImageBack)

                intViewTimeCountDown = objClientSettings.ViewingTime
                isViewTimeCountingDown = True
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWReadMagData_ProgressChanged(ByVal PercentChange As Integer)

        Try
            MyAppLog.WriteToLog("WinCTS.BWReadMagData_ProgressChanged() ProgressPercentage:" & PercentChange)
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
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWReadMagData_RunWorkerCompleted(ByVal Result As CTSSwipeScanInfo)

        MyAppLog.WriteToLog("WinCTS.BWReadMagData_RunWorkerCompleted()")

        Dim objSwipeScanInfo As New SwipeScanInfo
        Dim objSwipeScanDetail As New SwipeScanDetail

        Try
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow

            Me.ProgressBarCTS.Visibility = Windows.Visibility.Hidden

            If Result.LSReadBadgeWithTimeout <> 0 Then
                Me.lblStatusCTS.Content = "Error occurred while scanning"
            Else

                Me.lblStatusCTS.Content = "Succesful scan - Ready to scan again"

                objSwipeScanInfo.ClientID = objClientSettings.ClientID
                objSwipeScanInfo.UserID = intIVSUserID
                objSwipeScanInfo.DisableDBSave = objClientSettings.DisableDBSave
                ' objSwipeScanInfo.IDChecker = Application.objMyIDChecker
                objSwipeScanInfo.SwipeScanRawData = Result.SwipeRawData

                objSwipeScanDetail = DataAccess.NewDataSwipeScan(objSwipeScanInfo)

                'SwipeScanDetail_LoadByObject(objSwipeScanDetail, Result.ImageFront, Result.ImageBack)
                SwipeScanDetailLoad(objSwipeScanDetail, Result.ImageFront, Result.ImageBack)

                intViewTimeCountDown = objClientSettings.ViewingTime
                isViewTimeCountingDown = True
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWCTSReset_ProgressChanged(ByVal PercentChange As Integer)

        Try
            MyAppLog.WriteToLog("WinCTS.BWCTSReset_ProgressChanged() ProgressPercentage:" & PercentChange)
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
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWCTSReset_RunWorkerCompleted(ByVal Result As Boolean)

        MyAppLog.WriteToLog("WinCTS.BWCTSReset_RunWorkerCompleted()")

        Try
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow

            Me.ProgressBarCTS.Visibility = Windows.Visibility.Hidden

            If Result = False Then
                Me.lblStatusCTS.Content = "Error occurred while resetting device"

            Else
                Me.lblStatusCTS.Content = "Succesful reset - Ready to scan again"

            End If

            intViewTimeCountDown = objClientSettings.ViewingTime
            isViewTimeCountingDown = True

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWCTSIdentify_RunWorkerCompleted(ByVal Result As CTSDeviceInfo)

        MyAppLog.WriteToLog("WinCTS.BWCTSIdentify_RunWorkerCompleted()")

        Try

            If Result.ModelNo IsNot Nothing Then

                Dim objDeviceInfo As New DeviceInfo

                objDeviceInfo.ClientID = objClientSettings.ClientID
                objDeviceInfo.DeviceID = objClientSettings.DeviceID
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
                MyAppLog.WriteToLog("WinCTS.BWCTSIdentify_RunWorkerCompleted() Unable to connect to CTS device")

            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
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
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWSerialPort_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles BWSerialPort.RunWorkerCompleted

        If (e.Error IsNot Nothing) Then
            MyAppLog.WriteToLog("IVS", e.Error.ToString, EventLogEntryType.Error)

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
            objSwipeScanInfo.ClientID = objClientSettings.ClientID
            objSwipeScanInfo.UserID = intIVSUserID
            objSwipeScanInfo.SwipeScanRawData = RawSerialPortData
            objSwipeScanInfo.DisableCCSave = objClientSettings.DisableCCSave
            objSwipeScanInfo.DisableDBSave = objClientSettings.DisableDBSave
            objSwipeScanInfo.CCDigits = objClientSettings.CCDigits
            'objSwipeScanInfo.IDChecker = Application.objMyIDChecker

            objSwipeScanDetail = DataAccess.NewDataSwipeScan(objSwipeScanInfo)

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        Return objSwipeScanDetail

    End Function

    Private Sub objSerialPort_OnDataReceived(data As String) Handles objSerialPort.OnDataReceived

        Try
            Me.Dispatcher.BeginInvoke(New StartTheInvoke_OnDataReceived(AddressOf NowStartTheInvoke_OnDataReceived), data)
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Delegate Sub StartTheInvoke_OnDataReceived(ByVal StrRawSerialPortData As String)

    Friend Sub NowStartTheInvoke_OnDataReceived(ByVal StrRawSerialPortData As String)

        Try
            BWSerialPort.RunWorkerAsync(StrRawSerialPortData)

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
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
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub objSerialPort_OnImageReceived(Image As Bitmap) Handles MyM280.OnImageReceived

        Try
            Me.Dispatcher.BeginInvoke(New StartTheInvoke_OnImageReceived(AddressOf NowStartTheInvoke_OnImageReceived), Image)
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Delegate Sub StartTheInvoke_OnImageReceived(ByVal Image As Bitmap)

    Friend Sub NowStartTheInvoke_OnImageReceived(ByVal Image As Bitmap)

        Try
            Me.imgScannedDocument.Source = MyM280.GetBitmapSource(Image)
            imgEditedImage = Image
            strDocDisplayedSide = "F"
            SaveImage()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

End Class