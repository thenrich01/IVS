Imports LsFamily
Imports System.Reflection
Imports System.Drawing
Imports System.ComponentModel
Imports System.Drawing.Imaging
Imports System.Windows.Media.Imaging
Imports System.Windows.Media
Imports System.Windows
Imports System.Text
Imports IVS.data

Public Class CTSApi

    Private Shared MyAppLog As New IVS.AppLog.ApplicationLog(IVS.AppLog.ApplicationLog.DateEncodingType.YYYYMMDD, True)
    Private LSRetValues As New Dictionary(Of Integer, String)

    Private WithEvents BWScanDLIDImage As BackgroundWorker
    Private WithEvents BWReadMagStripe As BackgroundWorker
    Private WithEvents BWScanCheckImage As BackgroundWorker
    Private WithEvents BWBatchCheckImage As BackgroundWorker
    Private WithEvents BWCTSIdentify As BackgroundWorker
    Private WithEvents BWCTSReset As BackgroundWorker
    Private WithEvents BWCheckFeeder As BackgroundWorker

    Public Event BWProgressChanged(ByVal PercentChange As Integer)
    Public Event BWCompleted(ByVal objCTSSwipeScanInfo As CTSSwipeScanInfo)
    Public Event BWIdentifyCompleted(ByVal objCTSDeviceInfo As CTSDeviceInfo)
    Public Event BWCTSResetCompleted(ByVal Result As Boolean)
    Public Event BWCheckFeederCompleted(ByVal Result As Boolean)

    Public Sub New()

        Try
            LSLoadLSRetValues()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub LSLoadLSRetValues()

        Dim constants As New ArrayList()
        Dim TypeLSReply As System.Type

        Try

            TypeLSReply = GetType(LsReply)

            Dim fieldInfos As FieldInfo() = TypeLSReply.GetFields()

            For Each fi As FieldInfo In fieldInfos

                constants.Add(fi)

            Next

            For Each fi As FieldInfo In DirectCast(constants.ToArray(GetType(FieldInfo)), FieldInfo())

                LSRetValues.Add(fi.GetValue(Nothing), fi.Name)

            Next

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Function LSGetLSReplyValue(ByVal LSReply As Integer) As String

        Dim LSReplyValue As String = Nothing

        Try
            If LSRetValues.ContainsKey(LSReply) Then
                LSReplyValue = LSReply & " : " & LSRetValues.Item(LSReply)
            Else
                LSReplyValue = "LSReply:" & LSReply
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        Return LSReplyValue

    End Function

    Public Sub BWScanDLIDImage_Start(ByVal UnitType As LsFamily.LsDefines.LsUnitType)

        Me.BWScanDLIDImage = New BackgroundWorker
        Me.BWScanDLIDImage.WorkerReportsProgress = True
        Me.BWScanDLIDImage.WorkerSupportsCancellation = True
        Me.BWScanDLIDImage.RunWorkerAsync(UnitType)

    End Sub

    Public Sub BWScanDLIDImage_Cancel()

        Try
            Me.BWScanDLIDImage.CancelAsync()
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWScanDLIDImage_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles BWScanDLIDImage.DoWork

        Dim bw As BackgroundWorker

        Try
            bw = CType(sender, BackgroundWorker)
            RaiseEvent BWProgressChanged(0)
            e.Result = BWScanDLIDImage_TimeConsumingOperation(bw, e.Argument)

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub BWScanDLIDImage_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles BWScanDLIDImage.RunWorkerCompleted

        If (e.Error IsNot Nothing) Then

            MyAppLog.WriteToLog("IVS", e.Error.ToString, EventLogEntryType.Error)

        Else

            RaiseEvent BWCompleted(e.Result)
            Me.BWScanDLIDImage.Dispose()

        End If

    End Sub

    Private Function BWScanDLIDImage_TimeConsumingOperation(ByVal bw As BackgroundWorker, ByVal UnitType As LsFamily.LsDefines.LsUnitType) As CTSSwipeScanInfo

        Dim objLsApi As New LsApi
        Dim objCTSSwipeScanInfo As New CTSSwipeScanInfo
        Dim CTSUnitStatus As New LsUnitStatus
        Dim lsResult As Integer = Nothing
        Dim bmpDLIDImageBack As Bitmap = Nothing
        Dim bmpDLIDImageFront As Bitmap = Nothing
        Dim strDLIDBarcode2D As String = String.Empty
        Dim strDLIDBarcode1D As String = String.Empty
        Dim hWnd As Integer
        Dim hConnect As Integer
        Dim NrDoc As UInteger

        Try
            RaiseEvent BWProgressChanged(5)

            lsResult = objLsApi.LSConnect(hWnd, UnitType, hConnect, True)

            objCTSSwipeScanInfo.LSConnect = lsResult

            'MyAppLog.WriteToLog("CTSApi.BWScanDLIDImage_TimeConsumingOperation() UnitType:" & UnitType)
            'MyAppLog.WriteToLog("IVS", "CTSApi.BWScanDLIDImage_TimeConsumingOperation() UnitType:" & UnitType, EventLogEntryType.Error)

            If lsResult = LsReply.LS_OKAY OrElse lsResult = LsReply.LS_ALREADY_OPEN Then

                lsResult = objLsApi.LSReset(hConnect, hWnd, LsDefines.Reset.RESET_ERROR)

                RaiseEvent BWProgressChanged(10)
                'MyAppLog.WriteToLog("CTSApi.BWScanDLIDImage_TimeConsumingOperation() LSReset:" & LSGetLSReplyValue(lsResult))

                If (lsResult = LsReply.LS_OKAY) Then

                    objCTSSwipeScanInfo.LSPhotoFeeder = True

                    RaiseEvent BWProgressChanged(15)

                    'Process the document in the feeder
                    If UnitType = LsDefines.LsUnitType.LS_40_USB Then

                        lsResult = objLsApi.LSDocHandle(hConnect, hWnd, LsDefines.Stamp.STAMP_NO, LsDefines.PrintValidate.PRINT_VALIDATE_NO, LsDefines.CodelineToRead.READ_CODELINE_HW_NO, _
                                LsDefines.Side.SIDE_ALL_IMAGE, LsFamily.LsDefines.ScanMode.SCAN_MODE_256_GRAY_300, LsDefines.Feeder.FEED_AUTO, LsDefines.Sorter.SORTER_POCKET_1, _
                                LsDefines.Wait.WAIT_NO, LsDefines.Beep.BEEP_YES, NrDoc, LsDefines.ScanDocType.SCAN_CARD)

                    ElseIf UnitType = LsDefines.LsUnitType.LS_150_USB Then

                        'MyAppLog.WriteToLog("CTSApi.BWScanDLIDImage_TimeConsumingOperation() ScanMode: SCAN_MODE_256_GRAY_300")

                        lsResult = objLsApi.LSDocHandle(hConnect, hWnd, LsDefines.Stamp.STAMP_NO, LsDefines.PrintValidate.PRINT_VALIDATE_NO, LsDefines.CodelineToRead.READ_CODELINE_HW_NO, _
                                LsDefines.Side.SIDE_ALL_IMAGE, LsFamily.LsDefines.ScanMode.SCAN_MODE_COLOR_300, LsDefines.Feeder.FEED_AUTO, LsDefines.Sorter.SORTER_POCKET_1, _
                                LsDefines.Wait.WAIT_NO, LsDefines.Beep.BEEP_YES, NrDoc, LsDefines.ScanDocType.SCAN_CARD)

                    End If

                    RaiseEvent BWProgressChanged(20)

                    objCTSSwipeScanInfo.LSDocHandle = lsResult

                    'MyAppLog.WriteToLog("CTSApi.BWScanDLIDImage_TimeConsumingOperation() LSDocHandle:" & LSGetLSReplyValue(lsResult))

                    If (lsResult = LsReply.LS_OKAY) Then

                        'Successful ID Scan - Retrieve ID card images
                        lsResult = objLsApi.LSReadImage(hConnect, hWnd, LsDefines.ClearBlack.CLEAR_BLACK_YES, LsDefines.Side.SIDE_ALL_IMAGE, NrDoc, bmpDLIDImageFront, bmpDLIDImageBack, Nothing, Nothing)

                        RaiseEvent BWProgressChanged(30)

                        objCTSSwipeScanInfo.LSReadImage = lsResult

                        objCTSSwipeScanInfo.ImageFront = bmpDLIDImageFront
                        objCTSSwipeScanInfo.ImageBack = bmpDLIDImageBack

                        'MyAppLog.WriteToLog("CTSApi.TEST_BWScanDLIDImage_TimeConsumingOperation() LSReadImage:" & LSGetLSReplyValue(lsResult))
                        MyAppLog.WriteToLog("IVS", "CTSApi.TEST_BWScanDLIDImage_TimeConsumingOperation() LSReadImage:" & LSGetLSReplyValue(lsResult), EventLogEntryType.Error)

                        If (lsResult = LsReply.LS_OKAY) Then

                            'Successful Image retrieval - Retrieve barcode data from back of card

                            lsResult = objLsApi.LSReadBarcodesDriverLicense(hWnd, bmpDLIDImageBack, LsDefines.encodeBase.ENCODE_NO, strDLIDBarcode2D, LsDefines.CodelineToRead.READ_BARCODE_CODE39, strDLIDBarcode1D)

                            objCTSSwipeScanInfo.LSReadBarcodeBack = lsResult

                            MyAppLog.WriteToLog("CTSApi.BWScanDLIDImage_TimeConsumingOperation() LSReadBarcodesDriverLicense Back:" & LSGetLSReplyValue(lsResult))

                            If lsResult <> LsReply.LS_OKAY Then

                                RaiseEvent BWProgressChanged(50)

                                'Try Front of card
                                lsResult = objLsApi.LSReadBarcodesDriverLicense(hWnd, bmpDLIDImageFront, LsDefines.encodeBase.ENCODE_NO, strDLIDBarcode2D, LsDefines.CodelineToRead.READ_BARCODE_CODE39, strDLIDBarcode1D)

                                objCTSSwipeScanInfo.LSReadBarcodeFront = lsResult

                                MyAppLog.WriteToLog("CTSApi.BWScanDLIDImage_TimeConsumingOperation() LSReadBarcodesDriverLicense Front:" & LSGetLSReplyValue(lsResult))

                            End If 'LSReadBarcodesDriverLicense Front

                            If (lsResult = LsReply.LS_OKAY) Then

                                'Successful Barcode read from Image - Display barcode data

                                RaiseEvent BWProgressChanged(70)

                                'MyAppLog.WriteToLog("CTSApi.BWScanDLIDImage_TimeConsumingOperation() strDLIDBarcode1D:" & strDLIDBarcode1D)
                                'MyAppLog.WriteToLog("CTSApi.BWScanDLIDImage_TimeConsumingOperation() strDLIDBarcode2D:" & strDLIDBarcode2D)

                                objCTSSwipeScanInfo.SwipeRawData = strDLIDBarcode2D

                            End If 'LSReadBarcodesDriverLicense Front
                        End If 'LSReadImage

                    Else
                        'Feeder empty
                        objCTSSwipeScanInfo.LSPhotoFeeder = False
                        RaiseEvent BWProgressChanged(11)

                        Me.BWScanDLIDImage.CancelAsync()
                    End If 'LSDocHandle
                End If
            End If

            RaiseEvent BWProgressChanged(100)

            lsResult = objLsApi.LSDisconnect(hConnect, hWnd)
            'MyAppLog.WriteToLog("CTSApi.BWScanDLIDImage_TimeConsumingOperation() LSDisconnect:" & LSGetLSReplyValue(lsResult))

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        Return objCTSSwipeScanInfo

    End Function

    Private Sub BWScanDLIDImage_Disposed(sender As Object, e As System.EventArgs) Handles BWScanDLIDImage.Disposed
        Try
            'MyAppLog.WriteToLog("CTSApi.BWScanDLIDImage_Disposed()")
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try
    End Sub

    Public Sub BWReadMagStripe_Start(ByVal UnitType As LsFamily.LsDefines.LsUnitType)

        Me.BWReadMagStripe = New BackgroundWorker
        Me.BWReadMagStripe.WorkerReportsProgress = True
        Me.BWReadMagStripe.RunWorkerAsync(UnitType)

    End Sub

    Private Sub BWReadMagStripe_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles BWReadMagStripe.DoWork

        Dim bw As BackgroundWorker

        Try
            bw = CType(sender, BackgroundWorker)
            RaiseEvent BWProgressChanged(0)
            e.Result = BWReadMagStripe_TimeConsumingOperation(bw, e.Argument)

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Function BWReadMagStripe_TimeConsumingOperation(ByVal bw As BackgroundWorker, ByVal UnitType As LsFamily.LsDefines.LsUnitType) As CTSSwipeScanInfo

        Dim objLsApi As New LsApi
        Dim objCTSSwipeScanInfo As New CTSSwipeScanInfo
        Dim lsResult As Integer
        Dim strMagStripeData As String = Nothing
        Dim hWnd As Integer
        Dim hConnect As Integer

        Try
            RaiseEvent BWProgressChanged(10)
            lsResult = objLsApi.LSConnect(hWnd, UnitType, hConnect, True)

            objCTSSwipeScanInfo.LSConnect = lsResult

            If lsResult = LsReply.LS_OKAY OrElse lsResult = LsReply.LS_ALREADY_OPEN Then

                lsResult = objLsApi.LSReadBadgeWithTimeout(hConnect, hWnd, LsDefines.Badge.BADGE_READ_TRACKS_1_2_3, 20000, strMagStripeData)

                RaiseEvent BWProgressChanged(60)

                objCTSSwipeScanInfo.LSReadBadgeWithTimeout = lsResult

                MyAppLog.WriteToLog("CTSApi.BWReadMagStripe_TimeConsumingOperation() LSReadBadgeWithTimeout:" & LSGetLSReplyValue(lsResult))

                If (lsResult = LsReply.LS_OKAY) = True Then

                    If strMagStripeData = "ttt" Then
                        'timeout
                        objCTSSwipeScanInfo.LSReadBadgeWithTimeout = 999
                    Else

                        objCTSSwipeScanInfo.SwipeRawData = strMagStripeData

                        RaiseEvent BWProgressChanged(80)

                    End If

                End If

                'MyAppLog.WriteToLog("CTSApi.BWReadMagStripe_TimeConsumingOperation()" & strMagStripeData)
                RaiseEvent BWProgressChanged(100)

            End If

            lsResult = objLsApi.LSDisconnect(hConnect, hWnd)
            'MyAppLog.WriteToLog("CTSApi.BWReadMagStripe_TimeConsumingOperation() LSDisconnect:" & LSGetLSReplyValue(lsResult))

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        Return objCTSSwipeScanInfo

    End Function

    Private Sub BWReadMagStripe_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles BWReadMagStripe.RunWorkerCompleted

        If (e.Error IsNot Nothing) Then

            MyAppLog.WriteToLog("IVS", e.Error.ToString, EventLogEntryType.Error)

        Else

            'MyAppLog.WriteToLog("CTSApi.BWReadMagStripe_RunWorkerCompleted()")

            RaiseEvent BWCompleted(e.Result)
            Me.BWReadMagStripe.Dispose()

        End If

    End Sub

    Private Sub BWReadMagStripe_Disposed(sender As Object, e As System.EventArgs) Handles BWReadMagStripe.Disposed
        Try
            'MyAppLog.WriteToLog("CTSApi.BWReadMagStripe_Disposed()")
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try
    End Sub

    Public Sub BWScanCheckImage_Start(ByVal UnitType As LsFamily.LsDefines.LsUnitType)

        Me.BWScanCheckImage = New BackgroundWorker
        Me.BWScanCheckImage.WorkerReportsProgress = True
        Me.BWScanCheckImage.RunWorkerAsync(UnitType)

    End Sub

    Private Sub BWScanCheckImage_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles BWScanCheckImage.DoWork

        Dim bw As BackgroundWorker

        Try
            bw = CType(sender, BackgroundWorker)
            RaiseEvent BWProgressChanged(0)
            e.Result = BWScanCheckImage_TimeConsumingOperation(bw, e.Argument)

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Function BWScanCheckImage_TimeConsumingOperation(ByVal bw As BackgroundWorker, ByVal UnitType As LsFamily.LsDefines.LsUnitType) As CTSSwipeScanInfo

        Dim objLsApi As New LsApi
        Dim objCTSSwipeScanInfo As New CTSSwipeScanInfo
        Dim CTSUnitStatus As New LsUnitStatus
        Dim lsResult As Integer = Nothing
        Dim bmpCheckImageFront As Bitmap = Nothing
        Dim bmpCheckImageBack As Bitmap = Nothing
        Dim strMICRNumber As String = " "
        Dim hWnd As Integer
        Dim hConnect As Integer
        Dim NrDoc As UInteger

        Try
            RaiseEvent BWProgressChanged(5)

            lsResult = objLsApi.LSConnect(hWnd, UnitType, hConnect, True)

            objCTSSwipeScanInfo.LSConnect = lsResult
            'MyAppLog.WriteToLog("CTSApi.BWScanCheckImage_TimeConsumingOperation() LSConnect:" & LSGetLSReplyValue(lsResult))

            If lsResult = LsReply.LS_OKAY OrElse lsResult = LsReply.LS_ALREADY_OPEN Then

                RaiseEvent BWProgressChanged(10)

                lsResult = objLsApi.LSReset(hConnect, hWnd, LsDefines.Reset.RESET_ERROR)

                'MyAppLog.WriteToLog("CTSApi.BWScanCheckImage_TimeConsumingOperation() LSReset:" & LSGetLSReplyValue(lsResult))

                If (lsResult = LsReply.LS_OKAY) Then

                    RaiseEvent BWProgressChanged(15)

                    objCTSSwipeScanInfo.LSPhotoFeeder = True

                    RaiseEvent BWProgressChanged(20)

                    lsResult = objLsApi.LSDisableWaitDocument(hConnect, hWnd, True)

                    objCTSSwipeScanInfo.LSDisableWaitDocument = lsResult
                    'MyAppLog.WriteToLog("CTSApi.BWScanCheckImage_TimeConsumingOperation() LSDisableWaitDocument:" & LSGetLSReplyValue(lsResult))
                    'MyAppLog.WriteToLog("CTSApi.BWScanCheckImage_TimeConsumingOperation() UnitType:" & UnitType)

                    'If UnitType = LsDefines.LsUnitType.LS_40_USB Then
                    'READ_CODELINE_HW_NO
                    lsResult = objLsApi.LSDocHandle(hConnect, hWnd, LsDefines.Stamp.STAMP_NO, LsDefines.PrintValidate.PRINT_VALIDATE_NO, LsDefines.CodelineToRead.READ_CODELINE_HW_MICR, _
                            LsDefines.Side.SIDE_ALL_IMAGE, LsFamily.LsDefines.ScanMode.SCAN_MODE_256_GRAY_300, LsDefines.Feeder.FEED_AUTO, LsDefines.Sorter.SORTER_POCKET_1, _
                            LsDefines.Wait.WAIT_NO, LsDefines.Beep.BEEP_YES, NrDoc, LsDefines.ScanDocType.SCAN_PAPER_DOCUMENT)

                    'ElseIf UnitType = LsDefines.LsUnitType.LS_150_USB Then
                    'READ_CODELINE_HW_MICR
                    '    lsResult = objLsApi.LSDocHandle(hConnect, hWnd, LsDefines.Stamp.STAMP_NO, LsDefines.PrintValidate.PRINT_VALIDATE_NO, LsDefines.CodelineToRead.READ_CODELINE_HW_MICR, _
                    '           LsDefines.Side.SIDE_ALL_IMAGE, LsFamily.LsDefines.ScanMode.SCAN_MODE_COLOR_300, LsDefines.Feeder.FEED_AUTO, LsDefines.Sorter.SORTER_POCKET_1, _
                    '           LsDefines.Wait.WAIT_NO, LsDefines.Beep.BEEP_YES, NrDoc, LsDefines.ScanDocType.SCAN_PAPER_DOCUMENT)

                    'End If

                    RaiseEvent BWProgressChanged(30)

                    objCTSSwipeScanInfo.LSDocHandle = lsResult

                    'MyAppLog.WriteToLog("CTSApi.BWScanCheckImage_TimeConsumingOperation() LSDocHandle:" & LSGetLSReplyValue(lsResult))

                    If (lsResult = LsReply.LS_OKAY) Then
                        Dim CodelineHw As String = Nothing
                        CodelineHw = New String(" ")
                        lsResult = objLsApi.LSReadCodeline(hConnect, 0, strMICRNumber)
                        'MyAppLog.WriteToLog("CTSApi.BWScanCheckImage_TimeConsumingOperation() CodelineHw:" & CodelineHw)

                        RaiseEvent BWProgressChanged(50)

                        lsResult = objLsApi.LSReadImage(hConnect, hWnd, LsDefines.ClearBlack.CLEAR_BLACK_YES, LsDefines.Side.SIDE_ALL_IMAGE, NrDoc, bmpCheckImageFront, bmpCheckImageBack, Nothing, Nothing)

                        objCTSSwipeScanInfo.LSReadImage = lsResult
                        MyAppLog.WriteToLog("CTSApi.BWScanCheckImage_TimeConsumingOperation() LSReadImage:" & LSGetLSReplyValue(lsResult))

                        If (lsResult = LsReply.LS_OKAY) Then

                            RaiseEvent BWProgressChanged(70)

                            objCTSSwipeScanInfo.LSReadCodeline = lsResult
                            MyAppLog.WriteToLog("CTSApi.BWScanCheckImage_TimeConsumingOperation() LSReadCodeline:" & LSGetLSReplyValue(lsResult))

                            If (lsResult = LsReply.LS_OKAY) Then

                                'MyAppLog.WriteToLog("CTSApi.BWScanCheckImage_TimeConsumingOperation() LSReadCodeline:" & strMICRNumber)
                                RaiseEvent BWProgressChanged(80)

                                objCTSSwipeScanInfo.SwipeRawData = strMICRNumber
                                objCTSSwipeScanInfo.ImageFront = bmpCheckImageFront
                                objCTSSwipeScanInfo.ImageBack = bmpCheckImageBack

                            End If 'LSReadCodeLine

                        End If 'LSReadImage

                    Else
                        'Feeder empty
                        objCTSSwipeScanInfo.LSPhotoFeeder = False
                        RaiseEvent BWProgressChanged(11)

                        Me.BWScanDLIDImage.CancelAsync()
                    End If 'LSDocHandle

                    'Else
                    'Feeder empty
                    'RaiseEvent BWProgressChanged(11)

                    'Me.BWScanCheckImage.CancelAsync()

                    'End If 'IsDocPresent

                End If ' LSReset

            End If 'LSConnect

            RaiseEvent BWProgressChanged(90)

            lsResult = objLsApi.LSDisconnect(hConnect, hWnd)
            'MyAppLog.WriteToLog("CTSApi.BWScanCheckImage_TimeConsumingOperation() LSDisconnect:" & LSGetLSReplyValue(lsResult))

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        RaiseEvent BWProgressChanged(100)

        Return objCTSSwipeScanInfo

    End Function

    Private Sub BWScanCheckImage_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles BWScanCheckImage.RunWorkerCompleted

        If (e.Error IsNot Nothing) Then

            MyAppLog.WriteToLog("IVS", e.Error.ToString, EventLogEntryType.Error)

        Else

            'MyAppLog.WriteToLog("CTSApi.BWScanCheckImage_RunWorkerCompleted()")

            RaiseEvent BWCompleted(e.Result)
            Me.BWScanCheckImage.Dispose()

        End If

    End Sub

    Private Sub BWScanCheckImage_Disposed(sender As Object, e As System.EventArgs) Handles BWScanCheckImage.Disposed
        Try
            'MyAppLog.WriteToLog("CTSApi.BWScanCheckImage_Disposed()")
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try
    End Sub

    Public Sub BWBatchCheckImage_Start(ByVal BatchInfo As BatchInfo)

        Me.BWBatchCheckImage = New BackgroundWorker
        Me.BWBatchCheckImage.WorkerReportsProgress = True
        Me.BWBatchCheckImage.RunWorkerAsync(BatchInfo)

    End Sub

    Private Sub BWBatchCheckImage_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles BWBatchCheckImage.DoWork

        Dim bw As BackgroundWorker

        Try
            bw = CType(sender, BackgroundWorker)
            RaiseEvent BWProgressChanged(0)
            e.Result = BWBatchCheckImage_TimeConsumingOperation(bw, e.Argument)

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Function BWBatchCheckImage_TimeConsumingOperation(ByVal bw As BackgroundWorker, ByVal BatchInfo As BatchInfo) As CTSSwipeScanInfo

        Dim objLsApi As New LsApi
        Dim objCTSSwipeScanInfo As New CTSSwipeScanInfo
        Dim CTSUnitStatus As New LsUnitStatus
        Dim lsResult As Integer = Nothing
        Dim bmpCheckImageFront As Bitmap = Nothing
        Dim bmpCheckImageBack As Bitmap = Nothing
        Dim strMICRNumber As String = " "
        Dim hWnd As Integer
        Dim hConnect As Integer
        Dim NrDoc As UInteger

        Dim intNumDocument As Integer = 0
        Dim strDirectoryFile As String = BatchInfo.ImageLocation & "\BATCHID-" & BatchInfo.BatchID
        Dim strBaseFileName As String = "B-" & BatchInfo.BatchID & "-"
        Dim intDocID As UInt32 = 0
        Dim NomeFileImmagineMerge As String = New String(" ", 256)
        Dim pos_x As Double = 0
        Dim pos_y As Double = 0
        Dim pos_w As Double = 0
        Dim pos_h As Double = 0
        Dim intQuality As Integer = 100
        Dim intPageNumber As Integer
        Dim cb As LS515OnCodelineCallBack
        Dim fileFront As String = New String(" ", 256)
        Dim fileRear As String = New String(" ", 256)
        Dim fileFront2 As String = New String(" ", 256)
        Dim CodelineSw As String = New String(" ")
        Dim CodelineHw As String = New String(" ")
        Dim FrontImage As Bitmap = Nothing
        Dim BackImage As Bitmap = Nothing
        Dim FrontImage2 As Bitmap = Nothing
        Dim ImageMerged As Bitmap = Nothing
        Dim NrDocToProcess As Integer = 0
        Dim objSwipeScanInfo As New SwipeScanInfo
        '  Dim strMICRNumber As String
        'Dim Lock As New Object


        Try
            RaiseEvent BWProgressChanged(5)

            lsResult = objLsApi.LSConnect(hWnd, BatchInfo.CTSDeviceType, hConnect, True)

            objCTSSwipeScanInfo.LSConnect = lsResult
            'MyAppLog.WriteToLog("CTSApi.BWBatchCheckImage_TimeConsumingOperation() LSConnect:" & LSGetLSReplyValue(lsResult))

            If lsResult = LsReply.LS_OKAY OrElse lsResult = LsReply.LS_ALREADY_OPEN Then

                RaiseEvent BWProgressChanged(10)

                lsResult = objLsApi.LSReset(hConnect, hWnd, LsDefines.Reset.RESET_ERROR)

                'MyAppLog.WriteToLog("CTSApi.BWBatchCheckImage_TimeConsumingOperation() LSReset:" & LSGetLSReplyValue(lsResult))

                'lsResult = objLsApi.LSConfigDoubleLeafingAndDocLength(hConnect, hWnd, LsDefines.DoubleLeafing.DOUBLE_LEAFING_DISABLE, 1, 1, 5)

                'MyAppLog.WriteToLog("CTSApi.BWScanCheckImageMulti_TimeConsumingOperation() LSConfigDoubleLeafingAndDocLength:" & LSGetLSReplyValue(lsResult))

                If (lsResult = LsReply.LS_OKAY) Then

                    RaiseEvent BWProgressChanged(15)

                    objCTSSwipeScanInfo.LSPhotoFeeder = True

                    RaiseEvent BWProgressChanged(20)

                    lsResult = objLsApi.LSDisableWaitDocument(hConnect, hWnd, True)

                    objCTSSwipeScanInfo.LSDisableWaitDocument = lsResult
                    'MyAppLog.WriteToLog("CTSApi.BWBatchCheckImage_TimeConsumingOperation() LSDisableWaitDocument:" & LSGetLSReplyValue(lsResult))
                    'MyAppLog.WriteToLog("CTSApi.BWBatchCheckImage_TimeConsumingOperation() UnitType:" & BatchInfo.CTSDeviceType)

                    'If UnitType = LsDefines.LsUnitType.LS_40_USB Then

                    'lsResult = objLsApi.LSDocHandle(hConnect, hWnd, LsDefines.Stamp.STAMP_NO, LsDefines.PrintValidate.PRINT_VALIDATE_NO, LsDefines.CodelineToRead.READ_CODELINE_HW_NO, _
                    '        LsDefines.Side.SIDE_ALL_IMAGE, LsFamily.LsDefines.ScanMode.SCAN_MODE_256_GRAY_300, LsDefines.Feeder.FEED_FROM_PATH, LsDefines.Sorter.SORTER_POCKET_1, _
                    '        LsDefines.Wait.WAIT_YES, LsDefines.Beep.BEEP_NO, NrDoc, LsDefines.ScanDocType.SCAN_PAPER_DOCUMENT)

                    'ElseIf UnitType = LsDefines.LsUnitType.LS_150_USB Then

                    cb = AddressOf Ls515CodelineRead

                    lsResult = objLsApi.LSAutoDocHandle(hConnect, hWnd, LsDefines.Stamp.STAMP_NO, LsDefines.PrintValidate.PRINT_VALIDATE_NO, LsDefines.CodelineToRead.READ_CODELINE_HW_MICR, _
                            LsDefines.Side.SIDE_ALL_IMAGE, LsFamily.LsDefines.ScanMode.SCAN_MODE_256_GRAY_300, LsDefines.Sorter.SORTER_POCKET_1, intNumDocument, _
                        LsDefines.ClearBlack.CLEAR_BLACK_YES, LsDefines.ImageSave.IMAGE_SAVE_BOTH, strDirectoryFile, strBaseFileName, LsDefines.Unit.UNIT_INCH, pos_x, pos_y, pos_w, pos_h, _
                        LsDefines.FileType.FILE_JPEG, intQuality, LsDefines.FileAttribute.SAVE_REPLACE, intPageNumber, LsDefines.Wait.WAIT_YES, LsDefines.Beep.BEEP_NO, _
                              cb, LsDefines.ScanDocType.SCAN_PAPER_DOCUMENT)

                    'End If

                    RaiseEvent BWProgressChanged(30)
                    'RaiseEvent BWProgressChanged(900)
                    objCTSSwipeScanInfo.LSDocHandle = lsResult

                    'MyAppLog.WriteToLog("CTSApi.BWBatchCheckImage_TimeConsumingOperation() LSDocHandle:" & LSGetLSReplyValue(lsResult))

                    If (lsResult = LsReply.LS_OKAY) Then

                        'TEST
                        'Create batch Folder

                        If My.Computer.FileSystem.DirectoryExists(strDirectoryFile) = False Then

                            'MyAppLog.WriteToLog("CTSApi.BWBatchCheckImage_TimeConsumingOperation() Creating destination directory")
                            My.Computer.FileSystem.CreateDirectory(strDirectoryFile)

                        End If

                        'Check to see if the file exists
                        If Not My.Computer.FileSystem.FileExists(strDirectoryFile & "\Batch-" & BatchInfo.BatchID & ".txt") Then
                            My.Computer.FileSystem.WriteAllText(strDirectoryFile & "\Batch-" & BatchInfo.BatchID & ".txt", String.Empty, False)
                        End If

                        Dim sb1 As New StringBuilder
                        sb1.Append("ROUTING NUMBER")
                        sb1.Append(vbTab)
                        sb1.Append("ACCOUNT NUMBER")
                        sb1.Append(vbTab)
                        sb1.Append("CHECK NUMBER")
                        sb1.Append(vbTab)
                        sb1.Append("CHECK AMOUNT")
                        sb1.Append(vbTab)
                        sb1.Append(vbTab)
                        sb1.Append("MICR NUMBER")
                        sb1.Append(vbCrLf)

                        My.Computer.FileSystem.WriteAllText(strDirectoryFile & "\Batch-" & BatchInfo.BatchID & ".txt", sb1.ToString, True)
                        Dim intTotalAmount As Decimal = 0
                        Do
                            ' NomeFileImmagineMerge = BatchInfo.ImageLocation & "\B" & BatchInfo.BatchID & "-" & (intDocID + 1).ToString("0000") & ".jpg"
                            ' NomeFileImmagineMerge = BatchInfo.ImageLocation + "\Image_" + intDocID.ToString("0000") + "GUV" + ".jpg"

                            'Dim strTest As String = BatchInfo.ImageLocation & "\B_" & BatchInfo.BatchID & "-" & (intDocID + 1).ToString("0000") & "F.jpg"

                            lsResult = objLsApi.LSGetDocData(hConnect, 0, NrDoc, fileFront, fileRear, fileFront2, NomeFileImmagineMerge, FrontImage, BackImage, FrontImage2, Nothing, ImageMerged, CodelineSw, CodelineHw, False, False, LsDefines.FileType.FILE_JPEG)
                            ' lsResult = objLsApi.LSGetDocData(hConnect, 0, NrDoc, strTest, fileRear, fileFront2, NomeFileImmagineMerge, FrontImage, BackImage, FrontImage2, Nothing, ImageMerged, CodelineSw, CodelineHw, False, False, LsDefines.FileType.FILE_JPEG)

                            If (lsResult = LsFamily.LsReply.LS_OKAY Or lsResult = LsFamily.LsReply.LS_DOUBLE_LEAFING_WARNING Or _
                                lsResult = LsFamily.LsReply.LS_SORTER1_FULL Or lsResult = LsFamily.LsReply.LS_SORTER2_FULL Or _
                                ((lsResult >= LsFamily.LsReply.LS_SORTER_1_POCKET_1_FULL) And (lsResult <= LsFamily.LsReply.LS_SORTER_7_POCKET_3_FULL))) Then

                                'objSwipeScanInfo.ClientID = BatchInfo.ClientID
                                'objSwipeScanInfo.UserID = BatchInfo.UserID
                                'objSwipeScanInfo.DisableDBSave = False
                                'objSwipeScanInfo.SwipeScanRawData = CodelineHw
                                'objSwipeScanInfo.BatchID = BatchInfo.BatchID
                                'objSwipeScanInfo.ScanType = "C"

                                '  DataAccess.NewDataSwipeScan(objSwipeScanInfo)


                                'TEST
                                strMICRNumber = DataAccess.GetMICRNumber(CodelineHw)

                                Dim sb As New StringBuilder
                                sb.Append(DataAccess.GetRoutingNumber(strMICRNumber))
                                sb.Append(vbTab)
                                sb.Append(DataAccess.GetAccountNumber(strMICRNumber))
                                sb.Append(vbTab)
                                sb.Append(DataAccess.GetCheckNumber(strMICRNumber))
                                sb.Append(vbTab)
                                sb.Append(vbTab)
                                sb.Append(DataAccess.GetCheckAmount(CodelineHw))
                                sb.Append(vbTab)
                                sb.Append(vbTab)
                                sb.Append(strMICRNumber)
                                sb.Append(vbCrLf)

                                intTotalAmount += DataAccess.GetCheckAmount(CodelineHw)
                                ' SyncLock Lock()
                                My.Computer.FileSystem.WriteAllText(strDirectoryFile & "\Batch-" & BatchInfo.BatchID & ".txt", sb.ToString, True)
                                '  End SyncLock

                                RaiseEvent BWProgressChanged(intDocID + 9001)

                                intDocID = intDocID + 1

                                ' force Ok for repeat the GetDocData()
                                lsResult = LsFamily.LsReply.LS_OKAY


                            Else
                                'If (fFeederEmpty And lsResult = LsFamily.LsReply.LS_FEEDER_EMPTY) Then
                                '    'MessageBox.Show(cApplFunc.CheckReply(rc, "LSGetDocData"), TITLE_POPUP, MessageBoxButtons.OK, MessageBoxIcon.Information)
                                'End If

                                If (lsResult <> LsFamily.LsReply.LS_FEEDER_EMPTY) Then
                                    ' MessageBox.Show(cApplFunc.CheckReply(rc, "LSGetDocData"), TITLE_POPUP, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                End If
                            End If

                        Loop While ((lsResult = LsFamily.LsReply.LS_OKAY) And (NrDocToProcess = 0))

                        Dim sb2 As New StringBuilder
                        sb2.Append(intDocID)
                        sb2.Append(" documents processed")
                        sb2.Append(vbTab)
                        sb2.Append(" $")
                        sb2.Append(intTotalAmount)
                        sb2.Append(" total amount")

                        My.Computer.FileSystem.WriteAllText(strDirectoryFile & "\Batch-" & BatchInfo.BatchID & ".txt", sb2.ToString, True)

                    Else
                        'Feeder empty
                        objCTSSwipeScanInfo.LSPhotoFeeder = False
                        RaiseEvent BWProgressChanged(11)

                        Me.BWScanDLIDImage.CancelAsync()
                    End If 'LSDocHandle

                End If ' LSReset

            End If 'LSConnect

            RaiseEvent BWProgressChanged(90)

            lsResult = objLsApi.LSDisconnect(hConnect, hWnd)
            'MyAppLog.WriteToLog("CTSApi.BWBatchCheckImage_TimeConsumingOperation() LSDisconnect:" & LSGetLSReplyValue(lsResult))

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        RaiseEvent BWProgressChanged(100)

        Return objCTSSwipeScanInfo

    End Function

    Public Shared Function Ls515CodelineRead(ByVal CodelineReadHW As String, ByVal NrDoc As Int32, ByRef Pocket As Int32, ByRef Font As LsFamily.LsDefines.PrintFont, ByRef StringToPrint As String)

        ' Pocket selection
        'Pocket = CurrPocket
        'If CurrPocket = LsFamily.LsDefines.Sorter.SORTER_POCKET_2 Then
        '    CurrPocket = LsFamily.LsDefines.Sorter.SORTER_POCKET_1
        'Else
        '    CurrPocket = LsFamily.LsDefines.Sorter.SORTER_POCKET_2
        'End If

        '' Font and print selection
        'Select Case cApplFunc.PrintValidate
        '    Case 0
        '        Font = 0
        '        StringToPrint = Nothing

        '    Case 1
        '        If (cApplFunc.Print_High) Then
        '            Font = LsFamily.LsDefines.PrintFont.PRINT_UP_FORMAT_NORMAL
        '        Else
        '            Font = LsFamily.LsDefines.PrintFont.PRINT_FORMAT_NORMAL
        '        End If
        '        StringToPrint = String.Copy(cApplFunc.Endorse_str)

        '    Case 2
        '        If (cApplFunc.Print_High) Then
        '            Font = LsFamily.LsDefines.PrintFont.PRINT_UP_FORMAT_BOLD
        '        Else
        '            Font = LsFamily.LsDefines.PrintFont.PRINT_UP_FORMAT_BOLD
        '        End If
        '        StringToPrint = String.Copy(cApplFunc.Endorse_str)

        '    Case 3
        '        If (cApplFunc.Print_High) Then
        '            Font = LsFamily.LsDefines.PrintFont.PRINT_UP_FORMAT_NORMAL_15_CHAR
        '        Else
        '            Font = LsFamily.LsDefines.PrintFont.PRINT_FORMAT_NORMAL_15
        '        End If
        '        StringToPrint = String.Copy(cApplFunc.Endorse_str)

        '    Case Else
        '        Font = 0
        '        StringToPrint = Nothing
        'End Select

        Return True
    End Function

    Private Sub BWBatchCheckImage_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles BWBatchCheckImage.RunWorkerCompleted

        If (e.Error IsNot Nothing) Then

            MyAppLog.WriteToLog("IVS", e.Error.ToString, EventLogEntryType.Error)

        Else

            'MyAppLog.WriteToLog("CTSApi.BWBatchCheckImage_RunWorkerCompleted()")

            RaiseEvent BWCompleted(e.Result)
            Me.BWBatchCheckImage.Dispose()

        End If

    End Sub

    Private Sub BWBatchCheckImage_Disposed(sender As Object, e As System.EventArgs) Handles BWBatchCheckImage.Disposed
        Try
            'MyAppLog.WriteToLog("CTSApi.BWBatchCheckImage_Disposed()")
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try
    End Sub

    Public Sub BWCTSIdentify_Start(ByVal UnitType As LsFamily.LsDefines.LsUnitType)

        Me.BWCTSIdentify = New BackgroundWorker
        Me.BWCTSIdentify.WorkerReportsProgress = True
        Me.BWCTSIdentify.RunWorkerAsync(UnitType)

    End Sub

    Private Sub BWCTSIdentify_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles BWCTSIdentify.DoWork

        Dim bw As BackgroundWorker

        Try
            bw = CType(sender, BackgroundWorker)
            RaiseEvent BWProgressChanged(0)
            e.Result = BWCTSIdentify_TimeConsumingOperation(bw, e.Argument)

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Function BWCTSIdentify_TimeConsumingOperation(ByVal bw As BackgroundWorker, ByVal UnitType As LsFamily.LsDefines.LsUnitType) As CTSDeviceInfo

        Dim objLsApi As New LsApi
        Dim objCTSDeviceInfo As New CTSDeviceInfo
        Dim lsResult As Integer = 0
        Dim lsConfig As New LsConfiguration
        Dim lsModel As String = New String(" ", 20)
        Dim lsFirmwareVersion As String = New String(" ", 20)
        Dim lsFirmwareDate As String = New String(" ", 20)
        Dim lsUnitId As String = New String(" ", 20)
        Dim hWnd As Integer
        Dim hConnect As Integer

        Try
            'MyAppLog.WriteToLog("CTSApi.BWCTSIdentify_TimeConsumingOperation() LsUnitType:" & UnitType.ToString)
            lsResult = objLsApi.LSConnect(hWnd, UnitType, hConnect, True)

            objCTSDeviceInfo.LsConnect = lsResult
            'MyAppLog.WriteToLog("CTSApi.BWCTSIdentify_TimeConsumingOperation() LsConnect:" & LSGetLSReplyValue(lsResult))

            If lsResult = LsReply.LS_OKAY OrElse lsResult = LsReply.LS_ALREADY_OPEN Then

                lsResult = objLsApi.LSIdentify(hConnect, hWnd, lsConfig, lsModel, lsFirmwareVersion, lsFirmwareDate, lsUnitId)

                'MyAppLog.WriteToLog("CTSApi.BWCTSIdentify_TimeConsumingOperation() LSIdentify:" & LSGetLSReplyValue(lsResult))

                objCTSDeviceInfo.LSIdentify = lsResult
                objCTSDeviceInfo.ModelNo = lsModel
                objCTSDeviceInfo.FirmwareRev = lsFirmwareVersion
                objCTSDeviceInfo.FirmwareDate = lsFirmwareDate
                objCTSDeviceInfo.SerialNo = lsUnitId
                objCTSDeviceInfo.DeviceType = UnitType.ToString

                'Select Case UnitType
                '    Case LsDefines.LsUnitType.LS_40_USB
                '        objCTSDeviceInfo.DeviceType = "LS_40_USB"
                '    Case LsDefines.LsUnitType.LS_150_USB
                '        objCTSDeviceInfo.DeviceType = "LS_150_USB"
                'End Select

            End If

            lsResult = objLsApi.LSDisconnect(hConnect, hWnd)
            'MyAppLog.WriteToLog("CTSApi.BWCTSIdentify_TimeConsumingOperation() LSDisconnect:" & LSGetLSReplyValue(lsResult))

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        Return objCTSDeviceInfo

    End Function

    Private Sub BWCTSIdentify_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles BWCTSIdentify.RunWorkerCompleted

        If (e.Error IsNot Nothing) Then

            MyAppLog.WriteToLog("IVS", e.Error.ToString, EventLogEntryType.Error)

        Else

            'MyAppLog.WriteToLog("CTSApi.BWCTSIdentify_RunWorkerCompleted()")

            RaiseEvent BWIdentifyCompleted(e.Result)
            Me.BWCTSIdentify.Dispose()

        End If

    End Sub

    Private Sub BWCTSIdentify_Disposed(sender As Object, e As System.EventArgs) Handles BWCTSIdentify.Disposed
        Try
            'MyAppLog.WriteToLog("CTSApi.BWCTSIdentify_Disposed()")
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try
    End Sub

    Public Sub BWCTSReset_Start(ByVal UnitType As LsFamily.LsDefines.LsUnitType)

        Me.BWCTSReset = New BackgroundWorker
        Me.BWCTSReset.WorkerReportsProgress = True
        Me.BWCTSReset.RunWorkerAsync(UnitType)

    End Sub

    Private Sub BWCTSReset_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles BWCTSReset.DoWork

        Dim bw As BackgroundWorker

        Try
            bw = CType(sender, BackgroundWorker)
            RaiseEvent BWProgressChanged(0)
            e.Result = BWCTSReset_TimeConsumingOperation(bw, e.Argument)

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Function BWCTSReset_TimeConsumingOperation(ByVal bw As BackgroundWorker, ByVal UnitType As LsFamily.LsDefines.LsUnitType) As Boolean

        Dim objLsApi As New LsApi
        Dim lsResult As Integer = 0
        Dim hWnd As Integer
        Dim hConnect As Integer

        Try

            lsResult = objLsApi.LSConnect(hWnd, UnitType, hConnect, True)
            RaiseEvent BWProgressChanged(25)

            'MyAppLog.WriteToLog("CTSApi.ResetSW() LSConnect: " & LSGetLSReplyValue(lsResult))

            If lsResult = LsReply.LS_OKAY OrElse lsResult = LsReply.LS_ALREADY_OPEN Then

                lsResult = objLsApi.LSReset(hConnect, hWnd, LsDefines.Reset.RESET_ERROR)
                RaiseEvent BWProgressChanged(50)
                'MyAppLog.WriteToLog("CTSApi.ResetSW() LSReset: " & LSGetLSReplyValue(lsResult))

            End If

            'MyAppLog.WriteToLog("CTSApi.ResetHW() LSConnect: " & LSGetLSReplyValue(lsResult))

            If lsResult = LsReply.LS_OKAY OrElse lsResult = LsReply.LS_ALREADY_OPEN Then

                lsResult = objLsApi.LSReset(hConnect, hWnd, LsDefines.Reset.RESET_PATH)
                RaiseEvent BWProgressChanged(75)
                'MyAppLog.WriteToLog("CTSApi.ResetHW() LSReset: " & LSGetLSReplyValue(lsResult))

            End If

            lsResult = objLsApi.LSDisconnect(hConnect, hWnd)
            'MyAppLog.WriteToLog("CTSApi.ResetSW() LSDisconnect:" & LSGetLSReplyValue(lsResult))
            RaiseEvent BWProgressChanged(100)

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        Return True

    End Function

    Private Sub BWCTSReset_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles BWCTSReset.RunWorkerCompleted

        If (e.Error IsNot Nothing) Then

            MyAppLog.WriteToLog("IVS", e.Error.ToString, EventLogEntryType.Error)

        Else

            'MyAppLog.WriteToLog("CTSApi.BWCTSReset_RunWorkerCompleted()")

            RaiseEvent BWCTSResetCompleted(e.Result)
            Me.BWCTSReset.Dispose()

        End If

    End Sub

    Private Sub BWCTSReset_Disposed(sender As Object, e As System.EventArgs) Handles BWCTSReset.Disposed
        Try
            'MyAppLog.WriteToLog("CTSApi.BWCTSReset_Disposed()")
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try
    End Sub

    <System.Runtime.InteropServices.DllImport("gdi32.dll")> _
    Public Shared Function DeleteObject(hObject As IntPtr) As Boolean
    End Function

    Public Shared Sub SaveImage(ByVal ImageDetail As ImageDetail)

        Dim objBMP As Bitmap

        Try
            If DoesFileLocationExist(ImageDetail.FileName) Then

                objBMP = New Bitmap(ImageDetail.Image)

                objBMP.Save(ImageDetail.FileName, System.Drawing.Imaging.ImageFormat.Jpeg)
                'MyAppLog.WriteToLog("CTSApi.SaveImage()" & ImageDetail.FileName)

            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally
            If objBMP IsNot Nothing Then
                objBMP.Dispose()
            End If
        End Try

    End Sub

    Public Shared Sub DeleteImage(ByVal SwipeScanID As Integer, ByVal FileLocation As String)

        Dim strFileName As String

        Try
            strFileName = FileLocation & "\" & SwipeScanID & "f.jpg"

            If My.Computer.FileSystem.FileExists(strFileName) = True Then

                System.IO.File.Delete(strFileName)
                'MyAppLog.WriteToLog("CTSApi.DeleteImage()" & strFileName)

            End If

            strFileName = FileLocation & "\" & SwipeScanID & "b.jpg"

            If My.Computer.FileSystem.FileExists(strFileName) = True Then

                System.IO.File.Delete(strFileName)
                'MyAppLog.WriteToLog("CTSApi.DeleteImage()" & strFileName)

            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Public Shared Function AdjustBitmapBrightness(Image As Bitmap, Value As Integer) As Bitmap

        Dim FinalValue As Single
        Dim NewBitmap As Bitmap = Nothing
        Dim NewGraphics As Graphics
        Dim FloatColorMatrix As Single()()
        Dim NewColorMatrix As ColorMatrix
        Dim Attributes As ImageAttributes

        Try
            FinalValue = CSng(Value) / 255.0F
            NewBitmap = New Bitmap(Image.Width, Image.Height)
            NewGraphics = Graphics.FromImage(NewBitmap)
            FloatColorMatrix = {New Single() {1, 0, 0, 0, 0}, New Single() {0, 1, 0, 0, 0}, New Single() {0, 0, 1, 0, 0}, New Single() {0, 0, 0, 1, 0}, New Single() {FinalValue, FinalValue, FinalValue, 1, 1}}
            NewColorMatrix = New ColorMatrix(FloatColorMatrix)
            Attributes = New ImageAttributes()
            Attributes.SetColorMatrix(NewColorMatrix)

            NewGraphics.DrawImage(Image, New Rectangle(0, 0, Image.Width, Image.Height), 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel, Attributes)

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            Attributes.Dispose()
            NewGraphics.Dispose()
            Image.Dispose()

        End Try

        Return NewBitmap

    End Function

    Public Shared Function RotateBitmap(ByVal Image As Bitmap) As Bitmap

        Dim tb As New TransformedBitmap()

        Try
            tb.BeginInit()
            tb.Source = GetBitmapSource(Image)
            Dim transform As New RotateTransform(180)
            tb.Transform = transform
            tb.EndInit()

            Return GetBitmap(tb)

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally
            Image.Dispose()
        End Try

    End Function

    Public Shared Function GetBitmapSource(source As System.Drawing.Bitmap) As BitmapSource

        Dim ip As IntPtr = source.GetHbitmap()
        Dim bs As BitmapSource = Nothing

        Try
            bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions())

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            DeleteObject(ip)

        End Try

        Return bs

    End Function

    Public Shared Function GetBitmap(source As BitmapSource) As Bitmap

        Dim bmp As Bitmap
        Dim data As BitmapData

        Try
            bmp = New Bitmap(source.PixelWidth, source.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb)

            data = bmp.LockBits(New Rectangle(System.Drawing.Point.Empty, bmp.Size), ImageLockMode.[WriteOnly], System.Drawing.Imaging.PixelFormat.Format32bppPArgb)
            source.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride)
            bmp.UnlockBits(data)

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        Return bmp

    End Function

    Private Shared Function DoesFileLocationExist(ByVal FileFolder As String) As Boolean

        Try
            If My.Computer.FileSystem.DirectoryExists(GetPathOfFile(FileFolder)) = True Then

                Return True

            ElseIf My.Computer.FileSystem.DirectoryExists(GetPathOfFile(FileFolder)) = False Then

                'MyAppLog.WriteToLog("CTSApi.DoesFileLocationExist() Creating Image directory")
                My.Computer.FileSystem.CreateDirectory(GetPathOfFile(FileFolder))

                Return True
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Function

    Private Shared Function GetPathOfFile(FullFileName As String) As String
        Dim intPosition As Integer
        Dim strFilePath As String = Nothing

        Try
            intPosition = InStrRev(FullFileName, "\")
            If intPosition > 0 Then
                strFilePath = Left$(FullFileName, intPosition)
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        Return strFilePath

    End Function

End Class

Public Class CTSDeviceInfo

    Private _DeviceType As String
    Private _ModelNo As String
    Private _SerialNo As String
    Private _FirmwareRev As String
    Private _FirmwareDate As String
    Private _LsConnect As Integer
    Private _LSIdentify As Integer

    Property DeviceType() As String
        Get
            Return _DeviceType
        End Get
        Set(ByVal Value As String)
            _DeviceType = Value
        End Set
    End Property

    Property ModelNo() As String
        Get
            Return _ModelNo
        End Get
        Set(ByVal Value As String)
            _ModelNo = Value
        End Set
    End Property

    Property SerialNo() As String
        Get
            Return _SerialNo
        End Get
        Set(ByVal Value As String)
            _SerialNo = Value
        End Set
    End Property

    Property FirmwareRev() As String
        Get
            Return _FirmwareRev
        End Get
        Set(ByVal Value As String)
            _FirmwareRev = Value
        End Set
    End Property

    Property FirmwareDate() As String
        Get
            Return _FirmwareDate
        End Get
        Set(ByVal Value As String)
            _FirmwareDate = Value
        End Set
    End Property

    Property LsConnect() As Integer
        Get
            Return _LsConnect
        End Get
        Set(ByVal Value As Integer)
            _LsConnect = Value
        End Set
    End Property

    Property LSIdentify() As Integer
        Get
            Return _LSIdentify
        End Get
        Set(ByVal Value As Integer)
            _LSIdentify = Value
        End Set
    End Property

End Class

Public Class ImageDetail

    Private _Image As Bitmap
    Private _FileName As String

    Property Image() As Bitmap
        Get
            Return _Image
        End Get
        Set(ByVal Value As Bitmap)
            _Image = Value
        End Set
    End Property

    Property FileName() As String
        Get
            Return _FileName
        End Get
        Set(ByVal Value As String)
            _FileName = Value
        End Set
    End Property
End Class

Public Class CTSSwipeScanInfo

    Private _SwipeRawData As String
    Private _ImageFront As Bitmap
    Private _ImageBack As Bitmap
    Private _LSConnect As Integer
    Private _LSPhotoFeeder As Boolean
    Private _LSDocHandle As Integer
    Private _LSReadImage As Integer
    Private _LSReadBarcodeFront As Integer
    Private _LSReadBarcodeBack As Integer
    Private _LSReadBadgeWithTimeout As Integer
    Private _LSDisableWaitDocument As Integer
    Private _LSReadCodeline As Integer

    Property SwipeRawData() As String
        Get
            Return _SwipeRawData
        End Get
        Set(ByVal Value As String)
            _SwipeRawData = Value
        End Set
    End Property

    Property ImageFront() As Bitmap
        Get
            Return _ImageFront
        End Get
        Set(ByVal Value As Bitmap)
            _ImageFront = Value
        End Set
    End Property

    Property ImageBack() As Bitmap
        Get
            Return _ImageBack
        End Get
        Set(ByVal Value As Bitmap)
            _ImageBack = Value
        End Set
    End Property

    Property LSConnect() As Integer
        Get
            Return _LSConnect
        End Get
        Set(ByVal Value As Integer)
            _LSConnect = Value
        End Set
    End Property

    Property LSPhotoFeeder() As Boolean
        Get
            Return _LSPhotoFeeder
        End Get
        Set(ByVal Value As Boolean)
            _LSPhotoFeeder = Value
        End Set
    End Property

    Property LSDocHandle() As Integer
        Get
            Return _LSDocHandle
        End Get
        Set(ByVal Value As Integer)
            _LSDocHandle = Value
        End Set
    End Property

    Property LSReadImage() As Integer
        Get
            Return _LSReadImage
        End Get
        Set(ByVal Value As Integer)
            _LSReadImage = Value
        End Set
    End Property

    Property LSReadBarcodeFront() As Integer
        Get
            Return _LSReadBarcodeFront
        End Get
        Set(ByVal Value As Integer)
            _LSReadBarcodeFront = Value
        End Set
    End Property

    Property LSReadBarcodeBack() As Integer
        Get
            Return _LSReadBarcodeBack
        End Get
        Set(ByVal Value As Integer)
            _LSReadBarcodeBack = Value
        End Set
    End Property

    Property LSReadBadgeWithTimeout() As Integer
        Get
            Return _LSReadBadgeWithTimeout
        End Get
        Set(ByVal Value As Integer)
            _LSReadBadgeWithTimeout = Value
        End Set
    End Property

    Property LSDisableWaitDocument() As Integer
        Get
            Return _LSDisableWaitDocument
        End Get
        Set(ByVal Value As Integer)
            _LSDisableWaitDocument = Value
        End Set
    End Property

    Property LSReadCodeline() As Integer
        Get
            Return _LSReadCodeline
        End Get
        Set(ByVal Value As Integer)
            _LSReadCodeline = Value
        End Set
    End Property

End Class

Public Class BatchInfo

    Private _CTSDeviceType As LsDefines.LsUnitType
    Private _BatchID As String
    Private _ClientID As Integer
    Private _UserID As Integer
    Private _ImageLocation As String

    Property CTSDeviceType() As LsDefines.LsUnitType
        Get
            Return _CTSDeviceType
        End Get
        Set(ByVal Value As LsDefines.LsUnitType)
            _CTSDeviceType = Value
        End Set
    End Property

    Property BatchID() As String
        Get
            Return _BatchID
        End Get
        Set(ByVal Value As String)
            _BatchID = Value
        End Set
    End Property

    Property ClientID() As Integer
        Get
            Return _ClientID
        End Get
        Set(ByVal Value As Integer)
            _ClientID = Value
        End Set
    End Property

    Property UserID() As Integer
        Get
            Return _UserID
        End Get
        Set(ByVal Value As Integer)
            _UserID = Value
        End Set
    End Property

    Property ImageLocation() As String
        Get
            Return _ImageLocation
        End Get
        Set(ByVal Value As String)
            _ImageLocation = Value
        End Set
    End Property

End Class
