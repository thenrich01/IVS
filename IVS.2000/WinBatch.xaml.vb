Imports IVS.CTS
Imports IVS.AppLog
Imports IVS.Data
Imports LsFamily

Public Class WinBatch

    Private MyAppLog As New ApplicationLog(ApplicationLog.DateEncodingType.YYYYMMDD, True)
    Private objClientSettings As New ClientSettings
    Private intIVSUserID As Integer
    Private CTSDeviceType As LsDefines.LsUnitType

    Public Sub New(ByVal IVSUserID As Integer)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Dim intClientID As Integer

        Try
            intClientID = DataAccess.GetClientID
            objClientSettings = DataAccess.GetClientSettings(intClientID)

            intIVSUserID = IVSUserID

            Select Case objClientSettings.DeviceType

                Case "LS_40_USB"
                    CTSDeviceType = LsDefines.LsUnitType.LS_40_USB
                Case "LS_150_USB"
                    CTSDeviceType = LsDefines.LsUnitType.LS_150_USB
            End Select

            Dim MyCTSObject As New CTSApi()
            AddHandler MyCTSObject.BWIdentifyCompleted, AddressOf BWCTSIdentify_RunWorkerCompleted
            MyCTSObject.BWCTSIdentify_Start(CTSDeviceType)

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

            If Progress = 0 Then

                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait

            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWScanCheckImage_ProgressChanged(ByVal PercentChange As Integer)

        Try
            MyAppLog.WriteToLog("WinBatch.BWScanCheckImage_ProgressChanged() ProgressPercentage:" & PercentChange)
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

            If PercentChange > 9000 Then

                Dim intCheckID As Integer = PercentChange - 9000
                lblStatusCTS.Dispatcher.BeginInvoke(New StartTheInvoke_ReportStatus(AddressOf NowStartTheInvoke_ReportStatus), "Processing check #" & intCheckID)

            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWScanCheckImage_RunWorkerCompleted(ByVal Result As CTSSwipeScanInfo)

        MyAppLog.WriteToLog("WinBatch.BWScanCheckImage_RunWorkerCompleted()")

        Dim objCTSSwipeScanInfo As New CTSSwipeScanInfo
        Dim objSwipeScanInfo As New SwipeScanInfo
        Dim objSwipeScanDetail As New SwipeScanDetail

        Try
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow

            Me.ProgressBarCTS.IsEnabled = False
            Me.ProgressBarCTS.Visibility = Windows.Visibility.Hidden

            objCTSSwipeScanInfo = Result

            If objCTSSwipeScanInfo.LSPhotoFeeder = False Then

                Me.lblStatusCTS.Content = "No document found in feeder"

            End If

            If objCTSSwipeScanInfo.LSReadCodeline <> 0 Then

                Me.lblStatusCTS.Content = "Error occurred while scanning"


            End If

            If objCTSSwipeScanInfo.LSPhotoFeeder = True And objCTSSwipeScanInfo.LSConnect = 0 And objCTSSwipeScanInfo.LSDisableWaitDocument = 0 And objCTSSwipeScanInfo.LSDocHandle = 0 And objCTSSwipeScanInfo.LSReadBadgeWithTimeout = 0 _
                And objCTSSwipeScanInfo.LSReadBarcodeBack = 0 And objCTSSwipeScanInfo.LSReadBarcodeFront = 0 And objCTSSwipeScanInfo.LSReadCodeline = 0 And objCTSSwipeScanInfo.LSReadImage = 0 Then

                Me.lblStatusCTS.Content = "Succesful scan - Ready to scan again"

            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWCTSIdentify_RunWorkerCompleted(ByVal Result As CTSDeviceInfo)

        Dim objCTSDeviceInfo As New CTSDeviceInfo

        MyAppLog.WriteToLog("WinBatch.BWCTSIdentify_RunWorkerCompleted()")

        Try

            objCTSDeviceInfo = Result

            If objCTSDeviceInfo.ModelNo IsNot Nothing Then

                Dim objDeviceInfo As New DeviceInfo

                objDeviceInfo.ClientID = objClientSettings.ClientID
                objDeviceInfo.DeviceID = objClientSettings.DeviceID
                objDeviceInfo.ModelNo = objCTSDeviceInfo.ModelNo
                objDeviceInfo.FirmwareRev = objCTSDeviceInfo.FirmwareRev
                objDeviceInfo.FirmwareDate = objCTSDeviceInfo.FirmwareDate
                objDeviceInfo.HardwareRev = "NULL"
                objDeviceInfo.ComPort = "USB"
                objDeviceInfo.SerialNo = objCTSDeviceInfo.SerialNo
                objDeviceInfo.DeviceType = objCTSDeviceInfo.DeviceType

                DataAccess.UpdateDevice(objDeviceInfo)
                Me.lblStatusCTS.Content = "Successful connection to CTS device - Ready to scan"
                EnableCTSCommands(True)

            Else
                Me.lblStatusCTS.Content = "Unable to connect to CTS device"
                MyAppLog.WriteToLog("WinBatch.BWCTSIdentify_RunWorkerCompleted() Unable to connect to CTS device")

            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub EnableCTSCommands(ByVal IsEnabled As Boolean)

        Try
            Me.cmdBatchStart.IsEnabled = IsEnabled

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdBatchStart_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdBatchStart.Click

        Dim objCTSApi As New CTSApi
        Dim objBatchInfo As New BatchInfo


        Try
            objBatchInfo.CTSDeviceType = CTSDeviceType
            objBatchInfo.BatchID = Me.txtBatchID.Text
            objBatchInfo.ClientID = objClientSettings.ClientID
            objBatchInfo.UserID = intIVSUserID
            objBatchInfo.ImageLocation = objClientSettings.ImageLocation

            AddHandler objCTSApi.BWProgressChanged, AddressOf BWScanCheckImage_ProgressChanged
            AddHandler objCTSApi.BWCompleted, AddressOf BWScanCheckImage_RunWorkerCompleted

            objCTSApi.BWBatchCheckImage_Start(objBatchInfo)

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

End Class