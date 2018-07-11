Imports LsFamily
Imports IVS.CTS
Imports IVS.AppLog
Imports IVS.Data
Imports IVS.Data.IVSService

Public Class WinMagEntry

    Private MyAppLog As New ApplicationLog(ApplicationLog.DateEncodingType.YYYYMMDD, True)
    Private objClientSettings As ClientSettings
    Private intIVSUserID As Integer

    Public Sub New(ByVal IVSUserID As Integer, ByVal ClientSettings As ClientSettings)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Dim objCTSApi As New CTSApi
        Dim CTSDeviceType As LsDefines.LsUnitType

        Try
            objClientSettings = ClientSettings
            intIVSUserID = IVSUserID

            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner

            Select Case objClientSettings.DeviceType

                Case "LS_40_USB"
                    CTSDeviceType = LsDefines.LsUnitType.LS_40_USB
                Case "LS_150_USB"
                    CTSDeviceType = LsDefines.LsUnitType.LS_150_USB

            End Select

            AddHandler objCTSApi.BWProgressChanged, AddressOf BWReadMagData_ProgressChanged
            AddHandler objCTSApi.BWCompleted, AddressOf BWReadMagData_RunWorkerCompleted

            objCTSApi.BWReadMagStripe_Start(CTSDeviceType)

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

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

                Me.DialogResult = False
                Me.Close()

            Else

                Me.lblStatusCTS.Content = "Succesful scan"

                objSwipeScanInfo.ClientID = objClientSettings.ClientID
                objSwipeScanInfo.UserID = intIVSUserID
                objSwipeScanInfo.CCDigits = objClientSettings.CCDigits
                objSwipeScanInfo.DisableCCSave = objClientSettings.DisableCCSave
                objSwipeScanInfo.DisableDBSave = objClientSettings.DisableDBSave
                objSwipeScanInfo.SwipeScanRawData = Result.SwipeRawData

                objSwipeScanDetail = DataAccess.NewDataSwipeScan(objSwipeScanInfo)

                WinCTS.ManualEntrySwipeScanID = objSwipeScanDetail.SwipeScanID
                Me.DialogResult = True
                Me.Close()

            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

End Class