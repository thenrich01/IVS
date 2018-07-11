Imports LsFamily
Imports System.ComponentModel
Imports System.IO
Imports System.Drawing
Imports System.Windows.Threading
Imports System.Windows.Forms
Imports System.Threading
Imports System.Text
Imports System.Security.Principal
Imports IVS.CTS
Imports IVS.AppLog
Imports IVS.Data
Imports IVS.Data.WS.TEP.IVSService
Imports IVS.Data.WS.TEP

Public Class WinCTS

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
    Public Shared ManualEntrySwipeScanID As Integer
    Private CTSDeviceType As LsDefines.LsUnitType
    'TEP - GRAND RAPIDS
    Private isTEPChanged As Boolean = False
    Private objTEPClientSettings As TEPClientSettings

    Public Sub New(ByVal IVSUserID As Integer, Optional ByVal SwipeScanID As Integer = 0)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Dim intClientID As Integer

        Try
            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner
            EnableCTSCommands(False)
            intClientID = DataAccess.GetClientID()
            objClientSettings = DataAccess.GetClientSettings(intClientID)

            intIVSUserID = IVSUserID

            intSwipeScanToDisplay = SwipeScanID

            'TEP - GRAND RAPIDS
            objTEPClientSettings = DataAccess.GetTEPClientSettings()
            Me.cmdTEP.IsEnabled = False
            Me.cmdState.IsEnabled = False
            Me.cmdWarning.IsEnabled = False

            Select Case objClientSettings.DeviceType

                Case "LS_40_USB"
                    CTSDeviceType = LsDefines.LsUnitType.LS_40_USB
                Case "LS_150_USB"
                    CTSDeviceType = LsDefines.LsUnitType.LS_150_USB
            End Select

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
                End If

            End If

            Me.lblLocation.Content = objClientSettings.Location
            Me.lblStation.Content = objClientSettings.Station
            Me.lblUserName.Content = DataAccess.GetUserName(intIVSUserID)

            If WinMain.isUserAlertAble = False Then
                Me.cmdNewAlert.IsEnabled = False
            End If

            Dim MyCTSObject As New CTSApi()
            AddHandler MyCTSObject.BWIdentifyCompleted, AddressOf BWCTSIdentify_RunWorkerCompleted
            MyCTSObject.BWCTSIdentify_Start(CTSDeviceType)

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
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
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub WinCTS_KeyDown() Handles Me.KeyDown, Me.MouseDown
        intViewTimeCountDown = objClientSettings.ViewingTime
    End Sub

    Private Sub WinCTS_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Me.Closing

        Try
            timerViewTime.Stop()

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

    Private Sub dgSwipeScanHistory_CmdView_Click()

        Try
            DataSetSwipeScans_Navigate("Position", intSwipeScanToDisplay)

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

    Private Sub cmdCTSScanID_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCTSScanID.Click

        Dim objCTSApi As New CTSApi

        Try
            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save image updates before scanning?", "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

                    SaveImage()

                End If

            End If

            AddHandler objCTSApi.BWProgressChanged, AddressOf BWScanDLIDImage_ProgressChanged
            AddHandler objCTSApi.BWCompleted, AddressOf BWScanDLIDImage_RunWorkerCompleted

            objCTSApi.BWScanDLIDImage_Start(CTSDeviceType)

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub cmdCTSReadMagStripe_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCTSReadMagStripe.Click

        Dim objCTSApi As New CTSApi

        Try
            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save image updates before scanning?", "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

                    SaveImage()

                End If

            End If

            AddHandler objCTSApi.BWProgressChanged, AddressOf BWReadMagData_ProgressChanged
            AddHandler objCTSApi.BWCompleted, AddressOf BWReadMagData_RunWorkerCompleted

            objCTSApi.BWReadMagStripe_Start(CTSDeviceType)

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub cmdCTSScanCheck_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCTSScanCheck.Click

        Dim objCTSApi As New CTSApi

        Try
            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save image updates before scanning?", "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

                    SaveImage()

                End If

            End If

            AddHandler objCTSApi.BWProgressChanged, AddressOf BWScanCheckImage_ProgressChanged
            AddHandler objCTSApi.BWCompleted, AddressOf BWScanCheckImage_RunWorkerCompleted

            objCTSApi.BWScanCheckImage_Start(CTSDeviceType)

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub cmdCTSResetHW_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCTSResetHW.Click

        Dim objCTSApi As New CTSApi

        Try

            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save image updates before reset?", "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

                    SaveImage()

                End If

            End If

            AddHandler objCTSApi.BWProgressChanged, AddressOf BWCTSReset_ProgressChanged
            AddHandler objCTSApi.BWCTSResetCompleted, AddressOf BWCTSReset_RunWorkerCompleted

            objCTSApi.BWCTSReset_Start(CTSDeviceType)

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
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
                    DataAccess.NewException(ex)
                    MyAppLog.WriteToLog(ex)
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
                    DataAccess.NewException(ex)
                    MyAppLog.WriteToLog(ex)
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
            intViewTimeCountDown = objClientSettings.ViewingTime

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub cmdBrightnessSubtract_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdBrightnessSubtract.Click

        Try
            imgEditedImage = CTSApi.AdjustBitmapBrightness(imgEditedImage, -10)
            objNewBitmapSource = CTSApi.GetBitmapSource(imgEditedImage)
            Me.imgScannedDocument.Source = objNewBitmapSource

            isImageEdited = True
            intViewTimeCountDown = objClientSettings.ViewingTime

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub cmdRotate_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdRotate.Click

        Try
            imgEditedImage = CTSApi.RotateBitmap(imgEditedImage)
            objNewBitmapSource = CTSApi.GetBitmapSource(imgEditedImage)
            Me.imgScannedDocument.Source = objNewBitmapSource

            isImageEdited = True
            intViewTimeCountDown = objClientSettings.ViewingTime

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub cmdSave_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdSave.Click

        Try
            SaveImage()

            intViewTimeCountDown = objClientSettings.ViewingTime

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
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

                If System.Windows.MessageBox.Show("Save image updates before entering an alert?", "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

                    SaveImage()

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
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub cmdClose_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdClose.Click

        Try
            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save image updates before closing?", "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

                    SaveImage()

                End If

            End If

            Me.Close()

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
            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save image updates before clearing the data?", "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

                    SaveImage()

                End If

            End If

            ClearData()

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub cmdManualEntry_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdManualEntry.Click

        Dim winManualEntry As WinManEntry

        Try
            If isImageEdited = True Then

                If System.Windows.MessageBox.Show("Save image updates before manual data entry?", "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

                    SaveImage()

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
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

#End Region

#Region "Custom Subs/Functions"

    Private Sub SaveImage()

        Dim objImageDetail As New ImageDetail

        Try
            objImageDetail.Image = imgEditedImage
            objImageDetail.FileName = objClientSettings.ImageLocation & "\" & intSwipeScanID & strDocDisplayedSide & ".jpg"
            CTSApi.SaveImage(objImageDetail)

            isImageEdited = False

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub SwipeScanDetail_Load(ByVal SwipeScanID As Integer, ByVal SwipeScanType As String)

        Dim objSwipeScanDetail As New SwipeScanDetail

        Try
            objSwipeScanDetail = DataAccess.GetSwipeScanDetail(SwipeScanID, SwipeScanType)

            If SwipeScanType = "Credit Card" Then

                Select Case objSwipeScanDetail.CCIssuer

                    Case "AX"
                        Me.lblIDNumber.Content = "American Express - " & objSwipeScanDetail.IDAccountNumber
                    Case "MC"
                        Me.lblIDNumber.Content = "Mastercard - " & objSwipeScanDetail.IDAccountNumber
                    Case "VI"
                        Me.lblIDNumber.Content = "Visa - " & objSwipeScanDetail.IDAccountNumber
                    Case Else
                        Me.lblIDNumber.Content = objSwipeScanDetail.IDAccountNumber
                End Select

            Else
                Me.lblIDNumber.Content = objSwipeScanDetail.IDAccountNumber
            End If

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
                            Me.lblAge.FontSize = 19

                        End If

                    Else

                        Me.lblAge.Foreground = System.Windows.Media.Brushes.Black
                        Me.lblAge.Background = System.Windows.Media.Brushes.White
                        Me.lblAge.FontWeight = FontWeights.Normal
                        Me.lblAge.FontSize = 16
                    End If

                End If
            End If

            Me.lblAddressStreet.Content = objSwipeScanDetail.AddressStreet
            Me.lblAddressCity.Content = objSwipeScanDetail.AddressCity
            Me.lblAddressState.Content = objSwipeScanDetail.AddressState
            Me.lblAddressZip.Content = objSwipeScanDetail.AddressZip
            strSwipeRawData = "Data Source: " & objSwipeScanDetail.DataSource & vbCrLf & objSwipeScanDetail.SwipeRawData
            Me.lblCheckNumber.Content = "#" & objSwipeScanDetail.CheckNumber

            If File.Exists(objClientSettings.ImageLocation & "\" & SwipeScanID & "f.jpg") = True Then

                imgEditedImage = New Bitmap(objClientSettings.ImageLocation & "\" & SwipeScanID & "f.jpg")

                If SwipeScanType = "Check" Then

                    Me.imgScannedDocument.Width = 390

                Else

                    Me.imgScannedDocument.Width = 290

                End If

                objNewBitmapSource = CTSApi.GetBitmapSource(imgEditedImage)

                Me.imgScannedDocument.Source = objNewBitmapSource
                Me.cmdBrightnessAdd.IsEnabled = True
                Me.cmdBrightnessSubtract.IsEnabled = True
                Me.cmdRotate.IsEnabled = True
                Me.cmdSave.IsEnabled = True

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

            'TEP - GRAND RAPIDS
            LoadTEPDetail(SwipeScanID)

            intViewTimeCountDown = objClientSettings.ViewingTime

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub SwipeScanDetail_LoadByObject(ByVal SwipeScanDetailObject As SwipeScanDetail, ByVal FrontImage As Bitmap, ByVal BackImage As Bitmap)

        Try

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
            Me.lblEyes.Content = SwipeScanDetailObject.Height
            Me.lblHair.Content = SwipeScanDetailObject.Hair
            Me.lblDateOfIssue.Content = SwipeScanDetailObject.DateOfIssue
            Me.lblDateOfExpiration.Content = SwipeScanDetailObject.DateOfExpiration

            If String.IsNullOrEmpty(Trim(SwipeScanDetailObject.DateOfExpiration)) = False Then
                If IsDate(SwipeScanDetailObject.DateOfExpiration) = True Then

                    If SwipeScanDetailObject.DateOfExpiration <> "unknown" Then

                        If SwipeScanDetailObject.DateOfExpiration < Today Then
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
            Me.lblCheckNumber.Content = "#" & SwipeScanDetailObject.CheckNumber

            intSwipeScanID = SwipeScanDetailObject.SwipeScanID

            If FrontImage IsNot Nothing Then

                Me.imgEditedImage = FrontImage

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
                Me.cmdSave.IsEnabled = True

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

            SwipeScanHistory_Load(SwipeScanDetailObject.IDAccountNumber, SwipeScanDetailObject.CardType)
            SwipeScanAlerts_Load(SwipeScanDetailObject.IDAccountNumber, SwipeScanDetailObject.NameFirst, SwipeScanDetailObject.NameLast)

            'TEP - GRAND RAPIDS
            LoadTEPDetail(intSwipeScanID)

            If objClientSettings.ImageSave = True Then

                Dim sb As New StringBuilder
                Dim objImageDetail As New ImageDetail

                sb.Append(objClientSettings.ImageLocation)
                sb.Append("\")
                sb.Append(intSwipeScanID)
                sb.Append("f.jpg")

                objImageDetail.Image = FrontImage
                objImageDetail.FileName = sb.ToString

                CTSApi.SaveImage(objImageDetail)
                DataAccess.UpdateImageAvailable(intSwipeScanID)

                sb.Clear()
                objImageDetail = New ImageDetail

                sb.Append(objClientSettings.ImageLocation)
                sb.Append("\")
                sb.Append(intSwipeScanID)
                sb.Append("b.jpg")

                objImageDetail = New ImageDetail
                objImageDetail.Image = BackImage
                objImageDetail.FileName = sb.ToString

                ThreadPool.QueueUserWorkItem(AddressOf CTSApi.SaveImage, objImageDetail)

                If File.Exists(objClientSettings.ImageLocation & "\" & intSwipeScanID & "f.jpg") = True Then
                    imgEditedImage = New Bitmap(objClientSettings.ImageLocation & "\" & intSwipeScanID & "f.jpg")
                End If

            End If
        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub SwipeScanHistory_Load(ByVal IDAccountNumber As String, ByVal SwipeScanType As String)

        Try
            Me.dgSwipeScanHistory.ItemsSource = DataAccess.GetSwipeScanHistory(IDAccountNumber, SwipeScanType)
            Me.lblSwipeCount.Content = dgSwipeScanHistory.Items.Count
            intViewTimeCountDown = objClientSettings.ViewingTime
        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub SwipeScanAlerts_Load(ByVal IDAccountNumber As String, ByVal NameFirst As String, ByVal NameLast As String)

        Try
            Me.dgAlerts.ItemsSource = DataAccess.GetSwipeScanAlerts(IDAccountNumber, NameFirst, NameLast)
            Me.lblAlertCount.Content = dgAlerts.Items.Count
            intViewTimeCountDown = objClientSettings.ViewingTime
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

                Me.LblIDType.Content = strCurrentSwipeScanType
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

    Private Sub EnableCTSCommands(ByVal IsEnabled As Boolean)

        Try
            Me.cmdCTSReadMagStripe.IsEnabled = IsEnabled
            Me.cmdCTSResetHW.IsEnabled = IsEnabled
            Me.cmdCTSScanCheck.IsEnabled = IsEnabled
            Me.cmdCTSScanID.IsEnabled = IsEnabled

            intViewTimeCountDown = objClientSettings.ViewingTime

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub ClearData()

        Try
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

#Region "CTS"

    Delegate Sub StartTheInvoke_ReportStatus(ByVal Status As String)

    Friend Sub NowStartTheInvoke_ReportStatus(ByVal Status As String)

        Try
            Me.lblStatusCTS.Content = Status

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
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
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
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
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
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

                If System.Windows.MessageBox.Show("Save scanned image and read magnetic data?", "Unable to read ID data", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = MessageBoxResult.OK Then

                    Dim winMagEntry As WinMagEntry
                    winMagEntry = New WinMagEntry(intIVSUserID, objClientSettings)
                    winMagEntry.Owner = Me
                    winMagEntry.ShowDialog()

                    If winMagEntry.DialogResult = True Then

                        Me.lblStatusCTS.Content = "Succesful scan - Ready to scan again"

                        If objClientSettings.ImageSave = True Then

                            Dim sb As New StringBuilder
                            Dim objImageDetail As New ImageDetail

                            sb.Append(objClientSettings.ImageLocation)
                            sb.Append("\")
                            sb.Append(ManualEntrySwipeScanID)
                            sb.Append("f.jpg")

                            objImageDetail.Image = Result.ImageFront
                            objImageDetail.FileName = sb.ToString

                            CTSApi.SaveImage(objImageDetail)
                            DataAccess.UpdateImageAvailable(ManualEntrySwipeScanID)

                            sb.Clear()
                            objImageDetail = New ImageDetail

                            sb.Append(objClientSettings.ImageLocation)
                            sb.Append("\")
                            sb.Append(ManualEntrySwipeScanID)
                            sb.Append("b.jpg")

                            objImageDetail = New ImageDetail
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
                                    Dim objImageDetail As New ImageDetail

                                    sb.Append(objClientSettings.ImageLocation)
                                    sb.Append("\")
                                    sb.Append(ManualEntrySwipeScanID)
                                    sb.Append("f.jpg")

                                    objImageDetail.Image = Result.ImageFront
                                    objImageDetail.FileName = sb.ToString

                                    CTSApi.SaveImage(objImageDetail)
                                    DataAccess.UpdateImageAvailable(ManualEntrySwipeScanID)

                                    sb.Clear()
                                    objImageDetail = New ImageDetail

                                    sb.Append(objClientSettings.ImageLocation)
                                    sb.Append("\")
                                    sb.Append(ManualEntrySwipeScanID)
                                    sb.Append("b.jpg")

                                    objImageDetail = New ImageDetail
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
                objSwipeScanInfo.SwipeScanRawData = Result.SwipeRawData

                objSwipeScanDetail = DataAccess.NewDataSwipeScan(objSwipeScanInfo)

                SwipeScanDetail_LoadByObject(objSwipeScanDetail, Result.ImageFront, Result.ImageBack)

                intViewTimeCountDown = objClientSettings.ViewingTime
                isViewTimeCountingDown = True

            End If

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
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
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
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

                objSwipeScanInfo.ClientID = objClientSettings.ClientID
                objSwipeScanInfo.UserID = objClientSettings.DefaultUser
                objSwipeScanInfo.DisableDBSave = objClientSettings.DisableDBSave
                objSwipeScanInfo.SwipeScanRawData = Result.SwipeRawData
                objSwipeScanInfo.ScanType = "C"

                objSwipeScanDetail = DataAccess.NewDataSwipeScan(objSwipeScanInfo)

                SwipeScanDetail_LoadByObject(objSwipeScanDetail, Result.ImageFront, Result.ImageBack)

                intViewTimeCountDown = objClientSettings.ViewingTime
                isViewTimeCountingDown = True
            End If

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
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
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
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

                objSwipeScanInfo.ClientID = objClientSettings.ClientID
                objSwipeScanInfo.UserID = intIVSUserID
                objSwipeScanInfo.DisableDBSave = objClientSettings.DisableDBSave
                objSwipeScanInfo.SwipeScanRawData = Result.SwipeRawData

                objSwipeScanDetail = DataAccess.NewDataSwipeScan(objSwipeScanInfo)

                SwipeScanDetail_LoadByObject(objSwipeScanDetail, Result.ImageFront, Result.ImageBack)

                intViewTimeCountDown = objClientSettings.ViewingTime
                isViewTimeCountingDown = True
            End If

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
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
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
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

            intViewTimeCountDown = objClientSettings.ViewingTime
            isViewTimeCountingDown = True

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub BWCTSIdentify_RunWorkerCompleted(ByVal Result As CTSDeviceInfo)

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
            End If

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

        Try

            objTEPSwipeScanDetail.SwipeScanID = intSwipeScanID
            objTEPSwipeScanDetail.ClientID = 1
            objTEPSwipeScanDetail.CitationID = Me.txtCitation.Text
            objTEPSwipeScanDetail.ICRID = Me.txtICR.Text
            objTEPSwipeScanDetail.Disposition = "TEP"

            DataAccess.NewDataSwipeScan_TEP(objTEPSwipeScanDetail)

            Me.lblDisposition.Content = "Referred to Traffic Education Program"
            Me.lblCitation.Content += Me.txtCitation.Text
            Me.lblICR.Content += Me.txtICR.Text

            TEP_CreateTEPAlert("TEP", "Referred to Traffic Education Program")
            TEP_DisableEntry()

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub cmdState_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdState.Click

        Dim objTEPSwipeScanDetail As New TEPSwipeScanDetail

        Try

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
            Me.lblCitation.Content += Me.txtCitation.Text
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

            objTEPSwipeScanDetail = DataAccess.GetSwipeScanDetail_TEP(SwipeScanID)

            Me.lblCitation.Content = objTEPClientSettings.CitationPrefix

            uriSource = New Uri("Resources/red-plus-md.png", UriKind.Relative)
            Me.imgCmdViolation.Source = New BitmapImage(uriSource)

            If objTEPSwipeScanDetail.CitationID > 0 Then

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

            If txtCitation.Text <> "" Then

                If txtICR.Text <> "" Then

                    If Me.lblViolation.Content <> "" Then

                        Me.cmdTEP.IsEnabled = True
                        Me.cmdState.IsEnabled = True
                        Me.cmdWarning.IsEnabled = True

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

            strAlertNotes = "Disposition:" & Disposition & ", Citation:" & Me.lblCitation.Content & ", ICR:" & Me.lblICR.Content & ", Primary Violation:" & Me.lblViolation.Content

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