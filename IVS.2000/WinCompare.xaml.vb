Imports System.Windows.Threading
Imports IVS.Data
Imports IVS.AppLog

Public Class WinCompare

    Private MyAppLog As New ApplicationLog(ApplicationLog.DateEncodingType.YYYYMMDD, True)
    Private WithEvents timerViewTime As DispatcherTimer = New DispatcherTimer()
    Private intViewingTime As Integer
    Private intViewTimeCountDown As Integer

#Region "Control Bound Subs"

    Public Sub New(ByVal SwipeScanID As Integer, ByVal SwipeScanType As String, ByVal IVSUserID As Integer, ClientID As Integer, ByVal ViewingTime As Integer)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Dim objSwipeScanNavigateInfo As New SwipeScanNavigateInfo

        Try
            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner

            SwipeScanDetail2_Load(SwipeScanID, SwipeScanType)

            objSwipeScanNavigateInfo = DataAccess.SwipeScanNavigatePrevious(IVSUserID, ClientID, SwipeScanID)

            SwipeScanDetail1_Load(objSwipeScanNavigateInfo.SwipeScanID, objSwipeScanNavigateInfo.SwipeScanType)

            SwipeScanDetail_Compare()

            intViewingTime = ViewingTime
            intViewTimeCountDown = intViewingTime

            timerViewTime.IsEnabled = True
            timerViewTime.Interval = TimeSpan.FromSeconds(1)
            timerViewTime.Start()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub WinESeek_KeyDown() Handles Me.KeyDown, Me.MouseDown
        intViewTimeCountDown = intViewingTime
    End Sub

    Private Sub timerViewTime_Tick(sender As Object, e As System.EventArgs) Handles timerViewTime.Tick

        Try

            If intViewTimeCountDown > 0 Then

                intViewTimeCountDown -= 1

            Else
                timerViewTime.Stop()
                Me.Close()

            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdClose_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdClose.Click
        Try
            timerViewTime.Stop()
            Me.Close()
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try
    End Sub

#End Region

#Region "Custom Subs/Functions"

    Private Sub SwipeScanDetail1_Load(ByVal SwipeScanID As Integer, ByVal SwipeScanType As String)

        Dim objSwipeSScanDetail As New SwipeScanDetail

        Try
            objSwipeSScanDetail = DataAccess.GetSwipeScanDetail(SwipeScanID, SwipeScanType)

            Me.lbl1IDType.Content = SwipeScanType
            Me.lbl1IDNumber.Content = objSwipeSScanDetail.IDAccountNumber
            Me.Lbl1NameFirst.Content = objSwipeSScanDetail.NameFirst
            Me.Lbl1NameLast.Content = objSwipeSScanDetail.NameLast
            Me.Lbl1NameMiddle.Content = objSwipeSScanDetail.NameMiddle
            Me.Lbl1DateOfBirth.Content = objSwipeSScanDetail.DateOfBirth
            Me.Lbl1Age.Content = objSwipeSScanDetail.Age
            Me.Lbl1Sex.Content = objSwipeSScanDetail.Sex
            Me.Lbl1Height.Content = objSwipeSScanDetail.Height
            Me.Lbl1Weight.Content = objSwipeSScanDetail.Weight
            Me.Lbl1Eyes.Content = objSwipeSScanDetail.Height
            Me.Lbl1Hair.Content = objSwipeSScanDetail.Hair
            Me.Lbl1DateOfIssue.Content = objSwipeSScanDetail.DateOfIssue
            Me.Lbl1DateOfExpiration.Content = objSwipeSScanDetail.DateOfExpiration
            Me.Lbl1AddressStreet.Content = objSwipeSScanDetail.AddressStreet
            Me.Lbl1AddressCity.Content = objSwipeSScanDetail.AddressCity
            Me.Lbl1AddressState.Content = objSwipeSScanDetail.AddressState
            Me.Lbl1AddressZip.Content = objSwipeSScanDetail.AddressZip

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub SwipeScanDetail2_Load(ByVal SwipeScanID As Integer, ByVal SwipeScanType As String)

        Dim objSwipeSScanDetail As New SwipeScanDetail

        Try
            objSwipeSScanDetail = DataAccess.GetSwipeScanDetail(SwipeScanID, SwipeScanType)

            Me.lbl2IDType.Content = SwipeScanType
            Me.lbl2IDNumber.Content = objSwipeSScanDetail.IDAccountNumber
            Me.Lbl2NameFirst.Content = objSwipeSScanDetail.NameFirst
            Me.Lbl2NameLast.Content = objSwipeSScanDetail.NameLast
            Me.Lbl2NameMiddle.Content = objSwipeSScanDetail.NameMiddle
            Me.Lbl2DateOfBirth.Content = objSwipeSScanDetail.DateOfBirth
            Me.Lbl2Age.Content = objSwipeSScanDetail.Age
            Me.Lbl2Sex.Content = objSwipeSScanDetail.Sex
            Me.Lbl2Height.Content = objSwipeSScanDetail.Height
            Me.Lbl2Weight.Content = objSwipeSScanDetail.Weight
            Me.Lbl2Eyes.Content = objSwipeSScanDetail.Height
            Me.Lbl2Hair.Content = objSwipeSScanDetail.Hair
            Me.Lbl2DateOfIssue.Content = objSwipeSScanDetail.DateOfIssue
            Me.Lbl2DateOfExpiration.Content = objSwipeSScanDetail.DateOfExpiration
            Me.Lbl2AddressStreet.Content = objSwipeSScanDetail.AddressStreet
            Me.Lbl2AddressCity.Content = objSwipeSScanDetail.AddressCity
            Me.Lbl2AddressState.Content = objSwipeSScanDetail.AddressState
            Me.Lbl2AddressZip.Content = objSwipeSScanDetail.AddressZip

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub SwipeScanDetail_Compare()

        Try

            If Me.lbl1IDNumber.Content <> Me.lbl2IDNumber.Content Then

                Me.lbl1IDNumber.Foreground = System.Windows.Media.Brushes.Red
                Me.lbl1IDNumber.FontWeight = FontWeights.Bold

                Me.lbl2IDNumber.Foreground = System.Windows.Media.Brushes.Red
                Me.lbl2IDNumber.FontWeight = FontWeights.Bold

            End If

            If Me.Lbl1NameFirst.Content <> Me.Lbl2NameFirst.Content Then

                Me.Lbl1NameFirst.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl1NameFirst.FontWeight = FontWeights.Bold

                Me.Lbl2NameFirst.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl2NameFirst.FontWeight = FontWeights.Bold

            End If

            If Me.Lbl1NameLast.Content <> Me.Lbl2NameLast.Content Then

                Me.Lbl1NameLast.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl1NameLast.FontWeight = FontWeights.Bold

                Me.Lbl2NameLast.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl2NameLast.FontWeight = FontWeights.Bold

            End If

            If Me.Lbl1NameMiddle.Content <> Me.Lbl2NameMiddle.Content Then

                Me.Lbl1NameMiddle.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl1NameMiddle.FontWeight = FontWeights.Bold

                Me.Lbl2NameMiddle.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl2NameMiddle.FontWeight = FontWeights.Bold

            End If

            If Me.Lbl1DateOfBirth.Content <> Me.Lbl2DateOfBirth.Content Then

                Me.Lbl1DateOfBirth.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl1DateOfBirth.FontWeight = FontWeights.Bold

                Me.Lbl2DateOfBirth.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl2DateOfBirth.FontWeight = FontWeights.Bold

            End If

            If Me.Lbl1Age.Content <> Me.Lbl2Age.Content Then

                Me.Lbl1Age.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl1Age.FontWeight = FontWeights.Bold

                Me.Lbl2Age.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl2Age.FontWeight = FontWeights.Bold

            End If

            If Me.Lbl1Sex.Content <> Me.Lbl2Sex.Content Then

                Me.Lbl1Sex.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl1Sex.FontWeight = FontWeights.Bold

                Me.Lbl2Sex.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl2Sex.FontWeight = FontWeights.Bold

            End If

            If Me.Lbl1Height.Content <> Me.Lbl2Height.Content Then

                Me.Lbl1Height.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl1Height.FontWeight = FontWeights.Bold

                Me.Lbl2Height.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl2Height.FontWeight = FontWeights.Bold

            End If

            If Me.Lbl1Weight.Content <> Me.Lbl2Weight.Content Then

                Me.Lbl1Weight.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl1Weight.FontWeight = FontWeights.Bold

                Me.Lbl2Weight.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl2Weight.FontWeight = FontWeights.Bold

            End If

            If Me.Lbl1Eyes.Content <> Me.Lbl2Eyes.Content Then

                Me.Lbl1Eyes.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl1Eyes.FontWeight = FontWeights.Bold

                Me.Lbl2Eyes.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl2Eyes.FontWeight = FontWeights.Bold

            End If

            If Me.Lbl1Hair.Content <> Me.Lbl2Hair.Content Then

                Me.Lbl1Hair.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl1Hair.FontWeight = FontWeights.Bold

                Me.Lbl2Hair.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl2Hair.FontWeight = FontWeights.Bold

            End If

            If Me.Lbl1DateOfIssue.Content <> Me.Lbl2DateOfIssue.Content Then

                Me.Lbl1DateOfIssue.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl1DateOfIssue.FontWeight = FontWeights.Bold

                Me.Lbl2DateOfIssue.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl2DateOfIssue.FontWeight = FontWeights.Bold

            End If

            If Me.Lbl1DateOfExpiration.Content <> Me.Lbl2DateOfExpiration.Content Then

                Me.Lbl1DateOfExpiration.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl1DateOfExpiration.FontWeight = FontWeights.Bold

                Me.Lbl2DateOfExpiration.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl2DateOfExpiration.FontWeight = FontWeights.Bold

            End If

            If Me.Lbl1AddressStreet.Content <> Me.Lbl2AddressStreet.Content Then

                Me.Lbl1AddressStreet.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl1AddressStreet.FontWeight = FontWeights.Bold

                Me.Lbl2AddressStreet.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl2AddressStreet.FontWeight = FontWeights.Bold

            End If

            If Me.Lbl1AddressCity.Content <> Me.Lbl2AddressCity.Content Then

                Me.Lbl1AddressCity.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl1AddressCity.FontWeight = FontWeights.Bold

                Me.Lbl2AddressCity.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl2AddressCity.FontWeight = FontWeights.Bold

            End If

            If Me.Lbl1AddressState.Content <> Me.Lbl2AddressState.Content Then

                Me.Lbl1AddressState.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl1AddressState.FontWeight = FontWeights.Bold

                Me.Lbl2AddressState.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl2AddressState.FontWeight = FontWeights.Bold

            End If

            If Me.Lbl1AddressZip.Content <> Me.Lbl2AddressZip.Content Then

                Me.Lbl1AddressZip.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl1AddressZip.FontWeight = FontWeights.Bold

                Me.Lbl2AddressZip.Foreground = System.Windows.Media.Brushes.Red
                Me.Lbl2AddressZip.FontWeight = FontWeights.Bold

            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

End Class