Imports System.ComponentModel
Imports System.Windows.Threading
Imports IVS.MagTek
Imports IVS.AppLog
Imports IVS.Data
Imports IVS.Data.WS.TEP.IVSService
Imports IVS.Data.WS.TEP
Imports System.Windows.Forms

Public Class WinMagTek

    Private MyAppLog As New ApplicationLog(ApplicationLog.DateEncodingType.YYYYMMDD, True)
    Private objClientSettings As New ClientSettings
    Private intIVSUserID As Integer
    Private WithEvents BWMagTek As New BackgroundWorker
    Private WithEvents timerViewTime As DispatcherTimer = New DispatcherTimer()
    Private intSwipeScanID As Integer
    Private intAlertID As Integer
    Private intViewTimeCountDown As Integer
    Private isViewTimeCountingDown As Boolean = False
    Private intSwipeScanToDisplay As Integer = 0

    Private DelegateHandlerCardDataStateChange As MagTekApi.CallBackCardDataStateChanged

    'TEP - GRAND RAPIDS
    Private isTEPChanged As Boolean = False
    Private objTEPClientSettings As TEPClientSettings

    Public Sub New(ByVal IVSUserID As Integer, Optional ByVal SwipeScanID As Integer = 0)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Dim intResult As Integer
        Dim intClientID As Integer

        Try
            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner
            intClientID = DataAccess.GetClientID

            MyAppLog.WriteToLog("Client ID: " & intClientID)

            objClientSettings = DataAccess.GetClientSettings(intClientID)
            intIVSUserID = IVSUserID
            intSwipeScanToDisplay = SwipeScanID

            'TEP - GRAND RAPIDS
            objTEPClientSettings = DataAccess.GetTEPClientSettings()
            Me.cmdTEP.IsEnabled = False
            Me.cmdState.IsEnabled = False
            Me.cmdWarning.IsEnabled = False

            intResult = MagTekApi.MTUSCRAOpenDevice("")

            If intResult <> MagTekApi.MTSCRA_ST_OK Then
                System.Windows.MessageBox.Show("Unable to connect to MagTek USB reader.  Ensure reader is properly connected and open window again.", "IVS", MessageBoxButton.OK)
            End If

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

#Region "Control Bound Subs"

    Private Sub WinMagTek_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Me.Closing

        Try
            MagTekApi.MTUSCRAClearBuffer()

            Dim DelegateDataState As [Delegate] = TryCast(DelegateHandlerCardDataStateChange, [Delegate])
            DelegateHandlerCardDataStateChange = TryCast([Delegate].RemoveAll(DelegateDataState, DelegateDataState), MagTekApi.CallBackCardDataStateChanged)

            MagTekApi.MTUSCRACloseDevice()

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try
    End Sub

    Private Sub WinMagTek_KeyDown() Handles Me.KeyDown, Me.MouseDown
        intViewTimeCountDown = objClientSettings.ViewingTime
    End Sub

    Private Sub Window_Loaded(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded

        Try
            If intSwipeScanToDisplay > 0 Then

                DataSetSwipeScans_Navigate("Position", intSwipeScanToDisplay)
            Else

                If objClientSettings.DisableDBSave = False Then
                    DataSetSwipeScans_Navigate("Last")
                Else
                    Me.cmdManualEntry.Visibility = Windows.Visibility.Hidden
                    Me.lblRecordNavigation.Visibility = Windows.Visibility.Hidden
                    Me.cmdNavPrevious.Visibility = Windows.Visibility.Hidden
                    Me.cmdNavNext.Visibility = Windows.Visibility.Hidden
                    Me.cmdNavLast.Visibility = Windows.Visibility.Hidden
                    Me.lblSwipeHistory.IsEnabled = False
                    Me.labelSwipeCount.IsEnabled = False
                    Me.lblSwipeCount.IsEnabled = False
                    Me.dgSwipeScanHistory.IsEnabled = False
                End If

            End If

            Me.lblUserName.Content = DataAccess.GetUserName(intIVSUserID)

            If WinMain.isUserAlertAble = False Then
                Me.cmdNewAlert.IsEnabled = False
            End If

            DelegateHandlerCardDataStateChange = New MagTekApi.CallBackCardDataStateChanged(AddressOf objUSBMagTek_OnDataReceived)
            MagTekApi.MTUSCRACardDataStateChangedNotify(DelegateHandlerCardDataStateChange)

            AddHandler Me.txtCitation.TextChanged, AddressOf txtTEP_TextChanged
            AddHandler Me.txtICR.TextChanged, AddressOf txtTEP_TextChanged

            intViewTimeCountDown = objClientSettings.ViewingTime

            timerViewTime.IsEnabled = True
            timerViewTime.Interval = TimeSpan.FromSeconds(1)
            timerViewTime.Start()
            isViewTimeCountingDown = True

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub cmdNavLast_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdNavLast.Click

        Try
            DataSetSwipeScans_Navigate("Last")

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub cmdNavNext_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdNavNext.Click

        Try
            DataSetSwipeScans_Navigate("Next")

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub cmdNavPrevious_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdNavPrevious.Click

        Try
            DataSetSwipeScans_Navigate("Previous")

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
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
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub dgAlerts_CmdView_Click()

        Dim WinAlert As WinAlert
        Dim DialogResult As Boolean
        Dim objAlertDetail As New AlertDetail

        Try
            isViewTimeCountingDown = False

            objAlertDetail = DataAccess.GetAlertDetail(intAlertID)
            WinAlert = New WinAlert(objAlertDetail, intIVSUserID, WinMain.isUserAlertAble)
            WinAlert.Owner = Me

            DialogResult = WinAlert.ShowDialog

            intViewTimeCountDown = objClientSettings.ViewingTime
            isViewTimeCountingDown = True

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub dgSwipeScanHistory_CmdView_Click()

        Try
            DataSetSwipeScans_Navigate("Position", intSwipeScanToDisplay)

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
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
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub dgSwipeScanHistory_UnloadingRow(sender As Object, e As System.Windows.Controls.DataGridRowEventArgs) Handles dgSwipeScanHistory.UnloadingRow

        Try
            e.Row.Background = New SolidColorBrush(Colors.White)
        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
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
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub cmdClose_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdClose.Click

        Try
            timerViewTime.Stop()
            Me.Close()
        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub cmdnewAlert_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdNewAlert.Click

        Dim WinNewAlert As WinAlert
        Dim DialogResult As Boolean
        Dim strDocumentID As String
        Dim strNameFirst As String
        Dim strNameLast As String
        Dim objAlertDetail As New AlertDetail

        Try
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

            intViewTimeCountDown = objClientSettings.ViewingTime
            isViewTimeCountingDown = True

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
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
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub cmdClear_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdClear.Click

        Try
            ClearData()

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub cmdManualEntry_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdManualEntry.Click

        Dim winManualEntry As WinManEntry

        Try
            isViewTimeCountingDown = False

            winManualEntry = New WinManEntry(intIVSUserID, objClientSettings.ClientID, 1)
            winManualEntry.Owner = Me
            winManualEntry.ShowDialog()

            DataSetSwipeScans_Navigate("Last")

            intViewTimeCountDown = objClientSettings.ViewingTime
            isViewTimeCountingDown = True

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

#End Region

#Region "Custom Subs/Functions"

    Private Sub SwipeScanDetail_Load(ByVal SwipeScanID As Integer, ByVal SwipeScanType As String)

        Dim objSwipeScanDetail As New SwipeScanDetail

        Try
            objSwipeScanDetail = DataAccess.GetSwipeScanDetail(SwipeScanID, SwipeScanType)

            Me.lblIDNumber.Content = objSwipeScanDetail.IDAccountNumber
            Me.lblNameFirst.Content = objSwipeScanDetail.NameFirst
            Me.lblNameLast.Content = objSwipeScanDetail.NameLast
            Me.lblNameMiddle.Content = objSwipeScanDetail.NameMiddle
            Me.lblDateOfBirth.Content = objSwipeScanDetail.DateOfBirth

            If objSwipeScanDetail.Age > 0 Then
                Me.lblAge.Content = objSwipeScanDetail.Age
            Else
                Me.lblAge.Content = Nothing
            End If

            Me.lblSex.Content = objSwipeScanDetail.Sex
            Me.lblHeight.Content = objSwipeScanDetail.Height
            Me.lblWeight.Content = objSwipeScanDetail.Weight
            Me.lblEyes.Content = objSwipeScanDetail.Height
            Me.lblHair.Content = objSwipeScanDetail.Hair
            Me.lblDateOfIssue.Content = objSwipeScanDetail.DateOfIssue
            Me.lblDateOfExpiration.Content = objSwipeScanDetail.DateOfExpiration

            If String.IsNullOrEmpty(objSwipeScanDetail.DateOfExpiration) = False Then
                If IsDate(objSwipeScanDetail.DateOfExpiration) = True Then

                    If objSwipeScanDetail.DateOfExpiration <> "unknown" Then

                        If objSwipeScanDetail.DateOfExpiration < Today Then
                            Me.lblDateOfExpiration.Foreground = System.Windows.Media.Brushes.Red
                            Me.lblDateOfExpiration.Background = System.Windows.Media.Brushes.Yellow
                            Me.lblDateOfExpiration.FontWeight = FontWeights.Bold
                        Else

                            Me.lblDateOfExpiration.Foreground = System.Windows.Media.Brushes.Black
                            Me.lblDateOfExpiration.Background = System.Windows.Media.Brushes.White
                            Me.lblDateOfExpiration.FontWeight = FontWeights.Normal
                        End If

                    End If

                End If
            End If

            If String.IsNullOrEmpty(objSwipeScanDetail.Age) = False Then
                If IsNumeric(objSwipeScanDetail.Age) = True Then

                    If objSwipeScanDetail.Age < objClientSettings.Age And objSwipeScanDetail.Age > 0 Then

                        If objClientSettings.AgeHighlight = True Then
                            Me.lblAge.Foreground = System.Windows.Media.Brushes.Red
                            Me.lblAge.Background = System.Windows.Media.Brushes.Yellow
                            Me.lblAge.FontWeight = FontWeights.Bold
                        End If

                    Else

                        Me.lblAge.Foreground = System.Windows.Media.Brushes.Black
                        Me.lblAge.Background = System.Windows.Media.Brushes.White
                        Me.lblAge.FontWeight = FontWeights.Normal
                    End If

                End If
            End If

            Me.lblAddressStreet.Content = objSwipeScanDetail.AddressStreet
            Me.lblAddressCity.Content = objSwipeScanDetail.AddressCity
            Me.lblAddressState.Content = objSwipeScanDetail.AddressState
            Me.lblAddressZip.Content = objSwipeScanDetail.AddressZip

            'TEP - GRAND RAPIDS
            LoadTEPDetail(SwipeScanID)

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub SwipeScanHistory_Load(ByVal IDAccountNumber As String, ByVal SwipeScanType As String)

        Try
            Me.dgSwipeScanHistory.ItemsSource = DataAccess.GetSwipeScanHistory(IDAccountNumber, SwipeScanType)
            Me.lblSwipeCount.Content = dgSwipeScanHistory.Items.Count

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub SwipeScanAlerts_Load(ByVal IDAccountNumber As String, ByVal NameFirst As String, ByVal NameLast As String)

        Try
            Me.dgAlerts.ItemsSource = DataAccess.GetSwipeScanAlerts(IDAccountNumber, NameFirst, NameLast)
            Me.lblAlertCount.Content = dgAlerts.Items.Count

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub DataSetSwipeScans_Navigate(ByVal Direction As String, Optional ByVal Position As Integer = 0)

        Dim objSwipeScanNavigateInfo As New SwipeScanNavigateInfo
        Dim strNavigationDirection As String
        Dim strCurrentSwipeScanType As String
        Dim strDocumentID As String
        Dim strNameFirst As String
        Dim strNameLast As String

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

            If intSwipeScanID > 0 Then

                strCurrentSwipeScanType = objSwipeScanNavigateInfo.SwipeScanType

                Me.lblIDType.Content = strCurrentSwipeScanType
                Me.lblTimeStamp.Content = objSwipeScanNavigateInfo.SwipeScanTS

                SwipeScanDetail_Load(intSwipeScanID, strCurrentSwipeScanType)

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
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub BWMagTek_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles BWMagTek.DoWork

        Dim bw As BackgroundWorker

        Try
            bw = CType(sender, BackgroundWorker)
            e.Result = TimeConsumingOperation_NewSwipeIDDL(e.Argument, bw)

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub BWMagTek_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles BWMagTek.RunWorkerCompleted

        If (e.Error IsNot Nothing) Then

            MyAppLog.WriteToLog("IVS", e.Error.ToString, EventLogEntryType.Error)

        Else

            If Me.WindowState = Windows.WindowState.Minimized Then
                Me.WindowState = Windows.WindowState.Normal
            End If

            Me.lblIDNumber.Content = e.Result.IDAccountNumber
            Me.lblNameFirst.Content = e.Result.NameFirst
            Me.lblNameLast.Content = e.Result.NameLast
            Me.lblNameMiddle.Content = e.Result.NameMiddle
            Me.lblDateOfBirth.Content = e.Result.DateOfBirth

            If e.Result.Age > 0 Then
                Me.lblAge.Content = e.Result.Age
            Else
                Me.lblAge.Content = Nothing
            End If

            Me.lblSex.Content = e.Result.Sex
            Me.lblHeight.Content = e.Result.Height
            Me.lblWeight.Content = e.Result.Weight
            Me.lblEyes.Content = e.Result.Height
            Me.lblHair.Content = e.Result.Hair
            Me.lblDateOfIssue.Content = e.Result.DateOfIssue
            Me.lblDateOfExpiration.Content = e.Result.DateOfExpiration

            If String.IsNullOrEmpty(Trim(e.Result.DateOfExpiration)) = False Then
                If IsDate(e.Result.DateOfExpiration) = True Then

                    If e.Result.DateOfExpiration <> "unknown" Then

                        If e.Result.DateOfExpiration < Today Then
                            Me.lblDateOfExpiration.Foreground = System.Windows.Media.Brushes.Red
                            Me.lblDateOfExpiration.Background = System.Windows.Media.Brushes.Yellow
                            Me.lblDateOfExpiration.FontWeight = FontWeights.Bold
                        Else

                            Me.lblDateOfExpiration.Foreground = System.Windows.Media.Brushes.Black
                            Me.lblDateOfExpiration.Background = System.Windows.Media.Brushes.White
                            Me.lblDateOfExpiration.FontWeight = FontWeights.Normal
                        End If

                    End If

                End If
            End If

            If String.IsNullOrEmpty(e.Result.Age) = False Then
                If IsNumeric(e.Result.Age) = True Then


                    If e.Result.Age < objClientSettings.Age And e.Result.Age > 0 Then

                        If objClientSettings.AgeHighlight = True Then
                            Me.lblAge.Foreground = System.Windows.Media.Brushes.Red
                            Me.lblAge.Background = System.Windows.Media.Brushes.Yellow
                            Me.lblAge.FontWeight = FontWeights.Bold
                        End If

                        If objClientSettings.AgePopup = True Then

                            Dim WinUnderAge As WinUnderAge

                            WinUnderAge = New WinUnderAge(e.Result.NameFirst & " " & e.Result.NameLast, e.Result.Age, objClientSettings.Age)
                            WinUnderAge.Owner = Me
                            WinUnderAge.ShowDialog()

                        End If

                    Else

                        Me.lblAge.Foreground = System.Windows.Media.Brushes.Black
                        Me.lblAge.Background = System.Windows.Media.Brushes.White
                        Me.lblAge.FontWeight = FontWeights.Normal
                    End If

                End If

            End If

            Me.lblAddressStreet.Content = e.Result.AddressStreet
            Me.lblAddressCity.Content = e.Result.AddressCity
            Me.lblAddressState.Content = e.Result.AddressState
            Me.lblAddressZip.Content = e.Result.AddressZip

            intSwipeScanID = e.Result.SwipeScanID

            Me.lblIDType.Content = e.Result.CardType
            Me.lblTimeStamp.Content = e.Result.UpdateTS

            Me.lblIDNumber.Content = e.Result.IDAccountNumber
            Me.lblNameFirst.Content = e.Result.NameFirst
            Me.lblNameLast.Content = e.Result.NameLast

            SwipeScanAlerts_Load(e.Result.IDAccountNumber, e.Result.NameFirst, e.Result.NameLast)
            SwipeScanHistory_Load(e.Result.IDAccountNumber, e.Result.CardType)

            'TEP - GRAND RAPIDS
            LoadTEPDetail(e.Result.SwipeScanID)

            intViewTimeCountDown = objClientSettings.ViewingTime

        End If

    End Sub

    Private Function TimeConsumingOperation_NewSwipeIDDL(ByVal RawUSBPortData As String, ByVal bw As BackgroundWorker) As SwipeScanDetail

        Dim objSwipeScanDetail As New SwipeScanDetail
        Dim strArray As String()
        Dim objSwipeScanInfo As New SwipeScanInfo

        Try
            strArray = Split(RawUSBPortData, "|")

            objSwipeScanInfo.ClientID = objClientSettings.ClientID
            objSwipeScanInfo.UserID = intIVSUserID
            objSwipeScanInfo.SwipeScanRawData = strArray(15).ToString
            objSwipeScanInfo.DisableCCSave = objClientSettings.DisableCCSave
            objSwipeScanInfo.DisableDBSave = objClientSettings.DisableDBSave
            objSwipeScanInfo.CCDigits = objClientSettings.CCDigits
            objSwipeScanDetail = DataAccess.NewDataSwipeScan(objSwipeScanInfo)

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

        Return objSwipeScanDetail

    End Function

    Private Sub objUSBMagTek_OnDataReceived(CardDataState As Integer)

        Dim lResult As Integer
        Dim strData As String

        Try
            If CardDataState = MagTekApi.MTSCRA_DATA_READY Then

                strData = Space(4096)

                lResult = MagTekApi.MTUSCRAGetCardDataStr(strData, "|")
                MagTekApi.MTUSCRAClearBuffer()

                Me.Dispatcher.BeginInvoke(New StartTheInvoke_OnDataReceived(AddressOf NowStartTheInvoke_OnDataReceived), strData)
            End If

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Delegate Sub StartTheInvoke_OnDataReceived(ByVal StrRawUSBPortData As String)

    Friend Sub NowStartTheInvoke_OnDataReceived(ByVal StrRawUSBPortData As String)

        Try
            BWMagTek.RunWorkerAsync(StrRawUSBPortData)

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub ClearData()

        Try
            Me.lblIDType.Content = Nothing
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

            'TEP - GRAND RAPIDS
            Me.txtCitation.Text = Nothing
            Me.txtICR.Text = Nothing
            Me.lblCitation.Content = Nothing
            Me.lblICR.Content = Nothing
            Me.lblDisposition.Content = Nothing
            Me.lblViolation.Content = Nothing
            Me.cmdAddViolation.Visibility = Windows.Visibility.Hidden
            Me.cmdState.Visibility = Windows.Visibility.Hidden
            Me.cmdTEP.Visibility = Windows.Visibility.Hidden
            Me.cmdWarning.Visibility = Windows.Visibility.Hidden
            Me.txtCitation.Visibility = Windows.Visibility.Hidden
            Me.txtICR.Visibility = Windows.Visibility.Hidden
            Me.imgCitationRequired.Visibility = Windows.Visibility.Hidden
            Me.imgICRRequired.Visibility = Windows.Visibility.Hidden

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

#End Region

#Region "TEP - GRAND RAPIDS"

    Private Sub cmdAddViolation_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdAddViolation.Click

        Dim winViolation As WinViolations
        Dim DialogResult As Boolean
        Dim ListOfTEPSwipeScanViolations As New List(Of TEPSwipeScanViolations)
        Dim uriSource As System.Uri

        Try
            isViewTimeCountingDown = False

            winViolation = New WinViolations(intSwipeScanID)
            winViolation.Owner = Me
            DialogResult = winViolation.ShowDialog()

            If DialogResult = True Then

                ListOfTEPSwipeScanViolations = New List(Of TEPSwipeScanViolations)(DataAccess.GetSwipeScanDetail_Violations(intSwipeScanID))

                For Each objTEPSwipeScanViolation As TEPSwipeScanViolations In ListOfTEPSwipeScanViolations

                    If objTEPSwipeScanViolation.Sequence = 1 Then

                        Me.lblViolation.Content = objTEPSwipeScanViolation.Offense
                        Me.cmdAddViolation.IsEnabled = False
                        Me.cmdAddViolation.Visibility = Windows.Visibility.Hidden

                    End If

                    If objTEPSwipeScanViolation.Sequence = 2 Then

                        Me.cmdAddViolation.IsEnabled = True
                        Me.cmdAddViolation.Visibility = Windows.Visibility.Visible

                        uriSource = New Uri("Resources/MagnifyingGlass.png", UriKind.Relative)
                        Me.imgCmdViolation.Source = New BitmapImage(uriSource)

                    End If

                Next

                txtTEP_TextChanged()

            End If

            intViewTimeCountDown = objClientSettings.ViewingTime
            isViewTimeCountingDown = True

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub cmdTEP_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdTEP.Click

        Dim objTEPSwipeScanDetail As New TEPSwipeScanDetail
        Dim isCitationusedAlready As Boolean

        Try
            isCitationusedAlready = DataAccess.IsCitationUsedAlready(Me.txtCitation.Text)

            If isCitationusedAlready = True Then

                System.Windows.MessageBox.Show("Citation # has already been used", "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)

            Else
                objTEPSwipeScanDetail.SwipeScanID = intSwipeScanID
                objTEPSwipeScanDetail.ClientID = 1
                objTEPSwipeScanDetail.CitationID = Me.txtCitation.Text
                objTEPSwipeScanDetail.ICRID = Me.txtICR.Text
                objTEPSwipeScanDetail.Disposition = "TEP"

                DataAccess.NewDataSwipeScan_TEP(objTEPSwipeScanDetail)

                Me.lblDisposition.Content = "Referred to Traffic Education Program"
                Me.lblCitation.Content += Trim(Me.txtCitation.Text)
                Me.lblICR.Content += Me.txtICR.Text
                TEP_CreateTEPAlert("TEP", "Referred to Traffic Education Program")
                TEP_DisableEntry()

            End If

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub cmdState_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdState.Click

        Dim objTEPSwipeScanDetail As New TEPSwipeScanDetail
        Dim isCitationusedAlready As Boolean

        Try
            isCitationusedAlready = DataAccess.IsCitationUsedAlready(Me.txtCitation.Text)

            If isCitationusedAlready = True Then

                System.Windows.MessageBox.Show("Citation # has already been used", "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)

            Else

                objTEPSwipeScanDetail.SwipeScanID = intSwipeScanID
                objTEPSwipeScanDetail.ClientID = 1
                objTEPSwipeScanDetail.CitationID = Me.txtCitation.Text
                objTEPSwipeScanDetail.ICRID = Me.txtICR.Text
                objTEPSwipeScanDetail.Disposition = "MN"

                DataAccess.NewDataSwipeScan_TEP(objTEPSwipeScanDetail)

                Me.lblDisposition.Content = "Referred to State of Minnesota"
                Me.lblCitation.Content += Me.txtCitation.Text
                Me.lblICR.Content += Me.txtICR.Text

                TEP_CreateTEPAlert("MN", "Referred to State of Minnesota")
                TEP_DisableEntry()

            End If

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub cmdWarning_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmdWarning.Click

        Dim objTEPSwipeScanDetail As New TEPSwipeScanDetail

        Try

            objTEPSwipeScanDetail.SwipeScanID = intSwipeScanID
            objTEPSwipeScanDetail.ClientID = 1
            objTEPSwipeScanDetail.CitationID = Me.txtCitation.Text
            objTEPSwipeScanDetail.ICRID = Me.txtICR.Text
            objTEPSwipeScanDetail.Disposition = "WARN"

            DataAccess.NewDataSwipeScan_TEP(objTEPSwipeScanDetail)

            Me.lblDisposition.Content = "Warning Issued"
            Me.lblCitation.Visibility = Windows.Visibility.Hidden
            Me.lblICR.Content += Me.txtICR.Text
            TEP_CreateTEPAlert("WARN", "Warning Issued")
            TEP_DisableEntry()

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub LoadTEPDetail(ByVal SwipeScanID As Integer)

        Dim objTEPSwipeScanDetail As New TEPSwipeScanDetail
        Dim ListOfTEPSwipeScanViolations As New List(Of TEPSwipeScanViolations)
        Dim uriSource As System.Uri

        Try
            Me.txtCitation.Text = Nothing
            Me.lblViolation.Content = Nothing
            Me.txtICR.Text = Nothing
            Me.lblICR.Content = Nothing
            Me.txtCitation.IsEnabled = True
            Me.txtICR.IsEnabled = True
            Me.lblCitation.Visibility = Windows.Visibility.Visible

            objTEPSwipeScanDetail = DataAccess.GetSwipeScanDetail_TEP(SwipeScanID)

            Me.lblCitation.Content = Trim(objTEPClientSettings.CitationPrefix)

            uriSource = New Uri("Resources/red-plus-md.png", UriKind.Relative)
            Me.imgCmdViolation.Source = New BitmapImage(uriSource)

            If objTEPSwipeScanDetail.Disposition <> "" Then

                Me.lblCitation.Content += objTEPSwipeScanDetail.CitationID
                Me.lblICR.Content += objTEPSwipeScanDetail.ICRID

                Me.imgCitationRequired.Visibility = Windows.Visibility.Hidden
                Me.imgICRRequired.Visibility = Windows.Visibility.Hidden

                Select Case objTEPSwipeScanDetail.Disposition

                    Case "TEP"
                        Me.lblDisposition.Content = "Traffic Education Program"
                    Case "MN"
                        Me.lblDisposition.Content = "Sent to State of Minnesota"
                    Case "WARN"
                        Me.lblDisposition.Content = "Warning Issued"
                        Me.lblCitation.Visibility = Windows.Visibility.Hidden
                End Select

                Me.cmdAddViolation.Visibility = Windows.Visibility.Hidden
                Me.cmdState.Visibility = Windows.Visibility.Hidden
                Me.cmdTEP.Visibility = Windows.Visibility.Hidden
                Me.cmdWarning.Visibility = Windows.Visibility.Hidden
                Me.txtCitation.Visibility = Windows.Visibility.Hidden
                Me.txtICR.Visibility = Windows.Visibility.Hidden

                Me.LabelDisposition.Visibility = Windows.Visibility.Visible
                Me.lblDisposition.Visibility = Windows.Visibility.Visible

            Else
                Me.imgCitationRequired.Visibility = Windows.Visibility.Visible
                Me.imgICRRequired.Visibility = Windows.Visibility.Visible
                Me.cmdAddViolation.IsEnabled = True
                Me.cmdWarning.IsEnabled = False
                Me.cmdState.IsEnabled = False
                Me.cmdTEP.IsEnabled = False
                Me.cmdAddViolation.Visibility = Windows.Visibility.Visible
                Me.cmdState.Visibility = Windows.Visibility.Visible
                Me.cmdTEP.Visibility = Windows.Visibility.Visible
                Me.cmdWarning.Visibility = Windows.Visibility.Visible
                Me.txtCitation.Visibility = Windows.Visibility.Visible
                Me.txtICR.Visibility = Windows.Visibility.Visible

                Me.LabelDisposition.Visibility = Windows.Visibility.Hidden
                Me.lblDisposition.Visibility = Windows.Visibility.Hidden

            End If

            ListOfTEPSwipeScanViolations = New List(Of TEPSwipeScanViolations)(DataAccess.GetSwipeScanDetail_Violations(SwipeScanID))

            For Each objTEPSwipeScanViolation As TEPSwipeScanViolations In ListOfTEPSwipeScanViolations

                If objTEPSwipeScanViolation.Sequence = 1 Then

                    Me.lblViolation.Content = objTEPSwipeScanViolation.Offense
                    Me.cmdAddViolation.IsEnabled = False

                End If

                If objTEPSwipeScanViolation.Sequence = 2 Then

                    Me.cmdAddViolation.IsEnabled = True
                    Me.cmdAddViolation.Visibility = Windows.Visibility.Visible

                    uriSource = New Uri("Resources/MagnifyingGlass.png", UriKind.Relative)
                    Me.imgCmdViolation.Source = New BitmapImage(uriSource)

                End If
            Next

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub txtTEP_TextChanged()

        Try
            isTEPChanged = True

            If Me.lblViolation.Content <> "" Then

                If txtICR.Text <> "" Then
                    Me.cmdWarning.IsEnabled = True

                    If txtCitation.Text <> "" Then

                        Me.cmdTEP.IsEnabled = True
                        Me.cmdState.IsEnabled = True

                    End If
                End If
            End If

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub TEP_DisableEntry()

        Me.txtCitation.IsEnabled = False
        Me.txtICR.IsEnabled = False
        Me.cmdTEP.IsEnabled = False
        Me.cmdState.IsEnabled = False
        Me.cmdWarning.IsEnabled = False
        Me.cmdTEP.IsEnabled = True
        Me.cmdState.IsEnabled = True
        Me.cmdWarning.IsEnabled = True

        Me.cmdState.Visibility = Windows.Visibility.Hidden
        Me.cmdTEP.Visibility = Windows.Visibility.Hidden
        Me.cmdWarning.Visibility = Windows.Visibility.Hidden
        Me.txtCitation.Visibility = Windows.Visibility.Hidden
        Me.txtICR.Visibility = Windows.Visibility.Hidden

        Me.LabelDisposition.Visibility = Windows.Visibility.Visible
        Me.lblDisposition.Visibility = Windows.Visibility.Visible

        Me.imgCitationRequired.Visibility = Windows.Visibility.Hidden
        Me.imgICRRequired.Visibility = Windows.Visibility.Hidden

    End Sub

    Private Sub TEP_CreateTEPAlert(ByVal AlertType As String, ByVal Disposition As String)

        Dim strDocumentID As String
        Dim strNameFirst As String
        Dim strNameLast As String
        Dim strAlertNotes As String
        Dim objAlertDetail As New AlertDetail

        Try
            strDocumentID = Me.lblIDNumber.Content
            strNameFirst = Me.lblNameFirst.Content
            strNameLast = Me.lblNameLast.Content

            strAlertNotes = "Disposition:" & Disposition & ", Citation:" & Me.lblCitation.Content & ", ICR:" & Me.lblICR.Content & ", Primary Violation:" & Me.lblViolation.Content & "."

            objAlertDetail.AlertID = 0
            objAlertDetail.AlertType = AlertType
            objAlertDetail.IDNumber = strDocumentID
            objAlertDetail.NameFirst = strNameFirst
            objAlertDetail.NameLast = strNameLast
            objAlertDetail.DateOfBirth = Me.lblDateOfBirth.Content
            objAlertDetail.AlertContactName = DataAccess.GetUserName(intIVSUserID)
            objAlertDetail.AlertContactNumber = DataAccess.GetUserPhone(intIVSUserID)
            objAlertDetail.ActiveFlag = True
            objAlertDetail.AlertNotes = strAlertNotes
            objAlertDetail.UserID = intIVSUserID

            DataAccess.NewAlert(objAlertDetail)

            SwipeScanAlerts_Load(strDocumentID, strNameFirst, strNameLast)

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

#End Region

End Class