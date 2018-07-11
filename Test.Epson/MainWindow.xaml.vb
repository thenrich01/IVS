Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports System.IO
Imports com.epson.bank.driver
Imports IVS.Epson

Partial Public Class MainWindow

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

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        m_objDriverControl = New CApp()
        m_objConfigData = New StructByStep()

        If InitializeDriver() <> True Then
            Me.Close()
            Return
        End If

        Me.txtReaderInput.Focus()

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
                strResult = GetErrorString(errResult)
                drRet = MessageBox.Show(ConstComStr.MSG_00_000, strResult, MessageBoxButtons.OKCancel, MessageBoxIcon.[Error])
            End If

            If drRet = System.Windows.Forms.DialogResult.OK Then
                errResult = m_objDriverControl.InitDevice()
            Else
                Return False
            End If
        End While

        CallbackFuncDisplayMicrText = New CallbackProcDisplayMicrText(AddressOf CallbackDisplayMicrText)
        CallbackFuncSaveMicrText = New CallbackProcSaveMicrText(AddressOf CallbackSaveMicrText)
        CallbackFuncDisplayImageData = New CallbackProcDisplayImage(AddressOf CallbackDisplayImageData)
        CallbackFuncSaveImageData = New CallbackProcSaveImageData(AddressOf CallbackSaveImageData)

        CallbackFuncProcessError = New CApp.CallbackProcProcessError(AddressOf CallbackProcProcessError)
        CallbackFuncImage = New CApp.CallbackProcImage(AddressOf CallbackProcImage)
        CallbackFuncMicr = New CApp.CallbackProcMicr(AddressOf CallbackProcMicr)

        ' Specify callback function from DriverControl
        m_objDriverControl.SetProcessErrorCallback(CallbackFuncProcessError)
        m_objDriverControl.SetImageCallback(CallbackFuncImage)
        m_objDriverControl.SetMicrCallback(CallbackFuncMicr)

        Return True

    End Function

    'EVENTS

    Private Sub cmdScanCheck_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdScanCheck.Click
        If m_objDriverControl Is Nothing Then
            Return
        End If

        ClearImage()

        edtMicrText.Text = ""

        Dim objCARDScanParam As New CScanParam
        objCARDScanParam.SetScanMedia(ScanUnit.EPS_BI_SCN_UNIT_CHECKPAPER)
        objCARDScanParam.SetRGBColorDepth(com.epson.bank.driver.ColorDepth.EPS_BI_SCN_24BIT)
        objCARDScanParam.SetResolution(MfScanDpi.MF_SCAN_DPI_200)
        objCARDScanParam.SetIRColorDepth(com.epson.bank.driver.ColorDepth.EPS_BI_SCN_8BIT)
        objCARDScanParam.SetImageChannel(ImageTypeOption.EPS_BI_SCN_OPTION_COLOR)

        m_objDriverControl.SetScanParam(objCARDScanParam)

        Me.stImage2Front.Width = 390
        Me.stImage2Back.Width = 390

        Dim errRet As ErrorCode = m_objDriverControl.ScanCheck()
        If errRet <> ErrorCode.SUCCESS Then
            ShowErrorMessage(errRet)
            m_objDriverControl.CancelError()
            Return
        End If

    End Sub

    Private Sub cmdScanID_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdScanID.Click
        If m_objDriverControl Is Nothing Then
            Return
        End If

        ClearImage()

        edtMicrText.Text = ""

        Dim objCARDScanParam As New CScanParam
        objCARDScanParam.SetScanMedia(ScanUnit.EPS_BI_SCN_UNIT_CARD)
        objCARDScanParam.SetRGBColorDepth(com.epson.bank.driver.ColorDepth.EPS_BI_SCN_24BIT)
        objCARDScanParam.SetResolution(MfScanDpi.MF_SCAN_DPI_600)
        objCARDScanParam.SetIRColorDepth(com.epson.bank.driver.ColorDepth.EPS_BI_SCN_24BIT)
        objCARDScanParam.SetImageChannel(ImageTypeOption.EPS_BI_SCN_OPTION_COLOR)

        m_objDriverControl.SetScanParam(objCARDScanParam)

        Me.stImage2Front.Width = 290
        Me.stImage2Back.Width = 290

        Dim errRet As ErrorCode = m_objDriverControl.ScanCard()
        If errRet <> ErrorCode.SUCCESS Then
            ShowErrorMessage(errRet)
            m_objDriverControl.CancelError()
            Return
        End If

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

    Private Sub ShowErrorMessage(errResult As ErrorCode)
        Select Case errResult
            Case ErrorCode.ERR_TYPE
                MessageBox.Show(ConstComStr.MSG_03_000, ConstComStr.CAPTION_03_000, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_OPENED
                MessageBox.Show(ConstComStr.MSG_03_001, ConstComStr.CAPTION_03_001, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_NO_PRINTER
                MessageBox.Show(ConstComStr.MSG_03_002, ConstComStr.CAPTION_03_002, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_NO_TARGET
                MessageBox.Show(ConstComStr.MSG_03_003, ConstComStr.CAPTION_03_003, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_NO_MEMORY
                MessageBox.Show(ConstComStr.MSG_03_004, ConstComStr.CAPTION_03_004, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_HANDLE
                MessageBox.Show(ConstComStr.MSG_03_005, ConstComStr.CAPTION_03_005, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_TIMEOUT
                MessageBox.Show(ConstComStr.MSG_03_006, ConstComStr.CAPTION_03_006, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_ACCESS
                MessageBox.Show(ConstComStr.MSG_03_007, ConstComStr.CAPTION_03_007, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_PARAM
                MessageBox.Show(ConstComStr.MSG_03_008, ConstComStr.CAPTION_03_008, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_NOT_SUPPORT
                MessageBox.Show(ConstComStr.MSG_03_009, ConstComStr.CAPTION_03_009, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_OFFLINE
                MessageBox.Show(ConstComStr.MSG_03_010, ConstComStr.CAPTION_03_010, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_NOT_EPSON
                MessageBox.Show(ConstComStr.MSG_03_011, ConstComStr.CAPTION_03_011, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_WITHOUT_CB
                If m_bScanCancelError = True Then
                    MessageBox.Show(ConstComStr.MSG_03_012_01, ConstComStr.CAPTION_03_012, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    m_bScanCancelError = False
                Else
                    MessageBox.Show(ConstComStr.MSG_03_012_00, ConstComStr.CAPTION_03_012, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                Exit Select
            Case ErrorCode.ERR_BUFFER_OVER_FLOW
                MessageBox.Show(ConstComStr.MSG_03_013, ConstComStr.CAPTION_03_013, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_REGISTRY
                MessageBox.Show(ConstComStr.MSG_03_014, ConstComStr.CAPTION_03_014, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_PAPERINSERT_TIMEOUT
                MessageBox.Show(ConstComStr.MSG_03_015, ConstComStr.CAPTION_03_015, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_EXEC_FUNCTION
                MessageBox.Show(ConstComStr.MSG_03_016, ConstComStr.CAPTION_03_016, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_EXEC_MICR
                MessageBox.Show(ConstComStr.MSG_03_017, ConstComStr.CAPTION_03_017, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_EXEC_SCAN
                MessageBox.Show(ConstComStr.MSG_03_018, ConstComStr.CAPTION_03_018, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_RESET
                MessageBox.Show(ConstComStr.MSG_03_019, ConstComStr.CAPTION_03_019, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_ABORT
                MessageBox.Show(ConstComStr.MSG_03_020, ConstComStr.CAPTION_03_020, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_MICR
                MessageBox.Show(ConstComStr.MSG_03_021, ConstComStr.CAPTION_03_021, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_SCAN
                MessageBox.Show(ConstComStr.MSG_03_022, ConstComStr.CAPTION_03_022, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_LINE_OVERFLOW
                MessageBox.Show(ConstComStr.MSG_03_023, ConstComStr.CAPTION_03_023, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_PAPER_PILED
                MessageBox.Show(ConstComStr.MSG_03_024, ConstComStr.CAPTION_03_024, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_PAPER_JAM
                MessageBox.Show(ConstComStr.MSG_03_025, ConstComStr.CAPTION_03_025, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_COVER_OPEN
                MessageBox.Show(ConstComStr.MSG_03_026, ConstComStr.CAPTION_03_026, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_MICR_NODATA
                MessageBox.Show(ConstComStr.MSG_03_027, ConstComStr.CAPTION_03_027, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_MICR_BADDATA
                MessageBox.Show(ConstComStr.MSG_03_028, ConstComStr.CAPTION_03_028, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_MICR_NOISE
                MessageBox.Show(ConstComStr.MSG_03_029, ConstComStr.CAPTION_03_029, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_SCN_COMPRESS
                MessageBox.Show(ConstComStr.MSG_03_030, ConstComStr.CAPTION_03_030, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_PAPER_EXIST
                MessageBox.Show(ConstComStr.MSG_03_031, ConstComStr.CAPTION_03_031, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_PAPER_INSERT
                MessageBox.Show(ConstComStr.MSG_03_032, ConstComStr.CAPTION_03_032, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_EXEC_SCAN_CHECK_CONTINUOUS
                MessageBox.Show(ConstComStr.MSG_03_033, ConstComStr.CAPTION_03_033, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_EXEC_SCAN_CHECK_ONEBYONE
                MessageBox.Show(ConstComStr.MSG_03_034, ConstComStr.CAPTION_03_034, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_EXEC_SCAN_IDCARD
                MessageBox.Show(ConstComStr.MSG_03_035, ConstComStr.CAPTION_03_035, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_EXEC_PRINT_ROLLPAPER
                MessageBox.Show(ConstComStr.MSG_03_036, ConstComStr.CAPTION_03_036, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_EXEC_PRINT_VALIDATION
                MessageBox.Show(ConstComStr.MSG_03_037, ConstComStr.CAPTION_03_037, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_THREAD
                MessageBox.Show(ConstComStr.MSG_03_038, ConstComStr.CAPTION_03_038, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_IMAGE_FILEOPEN
                MessageBox.Show(ConstComStr.MSG_03_039, ConstComStr.CAPTION_03_039, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_IMAGE_UNKNOWNFORMAT
                MessageBox.Show(ConstComStr.MSG_03_040, ConstComStr.CAPTION_03_040, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_SIZE
                MessageBox.Show(ConstComStr.MSG_03_041, ConstComStr.CAPTION_03_041, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_NOT_FOUND
                MessageBox.Show(ConstComStr.MSG_03_042, ConstComStr.CAPTION_03_042, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_NOT_EXEC
                MessageBox.Show(ConstComStr.MSG_03_043, ConstComStr.CAPTION_03_043, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_BARCODE_NODATA
                MessageBox.Show(ConstComStr.MSG_03_044, ConstComStr.CAPTION_03_044, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_MICR_PARSE
                MessageBox.Show(ConstComStr.MSG_03_045, ConstComStr.CAPTION_03_045, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_SCN_IQA
                MessageBox.Show(ConstComStr.MSG_03_046, ConstComStr.CAPTION_03_046, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_PRINT_DATA_LENGTH_EXCEED
                MessageBox.Show(ConstComStr.MSG_03_047, ConstComStr.CAPTION_03_047, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_PRINT_DATA_UNRECEIVE
                MessageBox.Show(ConstComStr.MSG_03_048, ConstComStr.CAPTION_03_048, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case ErrorCode.ERR_IMAGE_FILEREAD
                MessageBox.Show(ConstComStr.MSG_03_049, ConstComStr.CAPTION_03_049, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
            Case Else
                MessageBox.Show(errResult.ToString(), "ERR", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Select
        End Select
    End Sub

    Private Function GetErrorString(errResultCode As ErrorCode) As String
        Select Case errResultCode

            Case ErrorCode.SUCCESS
                Return ConstComStr.CAPTION_00_000
            Case ErrorCode.ERR_TYPE
                Return ConstComStr.CAPTION_03_000
            Case ErrorCode.ERR_OPENED
                Return ConstComStr.CAPTION_03_001
            Case ErrorCode.ERR_NO_PRINTER
                Return ConstComStr.CAPTION_03_002
            Case ErrorCode.ERR_NO_TARGET
                Return ConstComStr.CAPTION_03_003
            Case ErrorCode.ERR_NO_MEMORY
                Return ConstComStr.CAPTION_03_004
            Case ErrorCode.ERR_HANDLE
                Return ConstComStr.CAPTION_03_005
            Case ErrorCode.ERR_TIMEOUT
                Return ConstComStr.CAPTION_03_006
            Case ErrorCode.ERR_ACCESS
                Return ConstComStr.CAPTION_03_007
            Case ErrorCode.ERR_PARAM
                Return ConstComStr.CAPTION_03_008
            Case ErrorCode.ERR_NOT_SUPPORT
                Return ConstComStr.CAPTION_03_009
            Case ErrorCode.ERR_OFFLINE
                Return ConstComStr.CAPTION_03_010
            Case ErrorCode.ERR_NOT_EPSON
                Return ConstComStr.CAPTION_03_011
            Case ErrorCode.ERR_WITHOUT_CB
                Return ConstComStr.CAPTION_03_012
            Case ErrorCode.ERR_BUFFER_OVER_FLOW
                Return ConstComStr.CAPTION_03_013
            Case ErrorCode.ERR_REGISTRY
                Return ConstComStr.CAPTION_03_014
            Case ErrorCode.ERR_PAPERINSERT_TIMEOUT
                Return ConstComStr.CAPTION_03_015
            Case ErrorCode.ERR_EXEC_FUNCTION
                Return ConstComStr.CAPTION_03_016
            Case ErrorCode.ERR_EXEC_MICR
                Return ConstComStr.CAPTION_03_017
            Case ErrorCode.ERR_EXEC_SCAN
                Return ConstComStr.CAPTION_03_018
            Case ErrorCode.ERR_RESET
                Return ConstComStr.CAPTION_03_019
            Case ErrorCode.ERR_ABORT
                Return ConstComStr.CAPTION_03_020
            Case ErrorCode.ERR_MICR
                Return ConstComStr.CAPTION_03_021
            Case ErrorCode.ERR_SCAN
                Return ConstComStr.CAPTION_03_022
            Case ErrorCode.ERR_LINE_OVERFLOW
                Return ConstComStr.CAPTION_03_023
            Case ErrorCode.ERR_PAPER_PILED
                Return ConstComStr.CAPTION_03_024
            Case ErrorCode.ERR_PAPER_JAM
                Return ConstComStr.CAPTION_03_025
            Case ErrorCode.ERR_COVER_OPEN
                Return ConstComStr.CAPTION_03_026
            Case ErrorCode.ERR_MICR_NODATA
                Return ConstComStr.CAPTION_03_027
            Case ErrorCode.ERR_MICR_BADDATA
                Return ConstComStr.CAPTION_03_028
            Case ErrorCode.ERR_MICR_NOISE
                Return ConstComStr.CAPTION_03_029
            Case ErrorCode.ERR_SCN_COMPRESS
                Return ConstComStr.CAPTION_03_030
            Case ErrorCode.ERR_PAPER_EXIST
                Return ConstComStr.CAPTION_03_031
            Case ErrorCode.ERR_PAPER_INSERT
                Return ConstComStr.CAPTION_03_032
            Case ErrorCode.ERR_EXEC_SCAN_CHECK_CONTINUOUS
                Return ConstComStr.CAPTION_03_033
            Case ErrorCode.ERR_EXEC_SCAN_CHECK_ONEBYONE
                Return ConstComStr.CAPTION_03_034
            Case ErrorCode.ERR_EXEC_SCAN_IDCARD
                Return ConstComStr.CAPTION_03_035
            Case ErrorCode.ERR_EXEC_PRINT_ROLLPAPER
                Return ConstComStr.CAPTION_03_036
            Case ErrorCode.ERR_EXEC_PRINT_VALIDATION
                Return ConstComStr.CAPTION_03_037
            Case ErrorCode.ERR_THREAD
                Return ConstComStr.CAPTION_03_038
            Case ErrorCode.ERR_IMAGE_FILEOPEN
                Return ConstComStr.CAPTION_03_039
            Case ErrorCode.ERR_IMAGE_UNKNOWNFORMAT
                Return ConstComStr.CAPTION_03_040
            Case ErrorCode.ERR_SIZE
                Return ConstComStr.CAPTION_03_041
            Case ErrorCode.ERR_NOT_FOUND
                Return ConstComStr.CAPTION_03_042
            Case ErrorCode.ERR_NOT_EXEC
                Return ConstComStr.CAPTION_03_043
            Case ErrorCode.ERR_BARCODE_NODATA
                Return ConstComStr.CAPTION_03_044
            Case ErrorCode.ERR_MICR_PARSE
                Return ConstComStr.CAPTION_03_045
            Case ErrorCode.ERR_SCN_IQA
                Return ConstComStr.CAPTION_03_046
            Case ErrorCode.ERR_PRINT_DATA_LENGTH_EXCEED
                Return ConstComStr.CAPTION_03_047
            Case ErrorCode.ERR_PRINT_DATA_UNRECEIVE
                Return ConstComStr.CAPTION_03_048
            Case ErrorCode.ERR_IMAGE_FILEREAD
                Return ConstComStr.CAPTION_03_049
            Case Else
                Return errResultCode.ToString()
        End Select
    End Function

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

    Private Sub SaveImageData(eFace As ScanSide, eImage As ImageType)
        If m_objDriverControl Is Nothing Then
            Return
        End If
        TextBox1.Dispatcher.BeginInvoke(New StartTheInvoke_TextBox1(AddressOf NowStartTheInvoke_TextBox1), "SaveImageData()")
        Dim cImageResult As New CImageResult()

        ' Specify image data format
        Dim eFormat As Format = Format.EPS_BI_SCN_JPEGHIGH

        ' Obtain image data
        cImageResult.SetFace(eFace)
        cImageResult.SetGradation(eImage)
        cImageResult.SetFormat(eFormat)
        Dim errResult As ErrorCode = ErrorCode.SUCCESS

        errResult = m_objDriverControl.GetScanImage(cImageResult)
        TextBox1.Dispatcher.BeginInvoke(New StartTheInvoke_TextBox1(AddressOf NowStartTheInvoke_TextBox1), errResult.ToString)
        CallbackSaveImageData(errResult, cImageResult)

    End Sub

    Private Sub CallbackSaveImageData(errResult As ErrorCode, objImageResult As CImageResult)

        If errResult <> ErrorCode.SUCCESS Then
            ShowErrorMessage(errResult)
            Return
        End If

        Dim cImage As Stream = objImageResult.GetImageData()
        Dim nImageSize As Integer = objImageResult.GetImageSize()
        Dim nTransactionNumber As Integer = objImageResult.GetTransactionNumber()
        Dim strExt As String = "jpg"

        ' Create Folder
        Dim sbPath As New StringBuilder()
        Dim szPath As String = Nothing

        sbPath.Append(System.IO.Directory.GetCurrentDirectory())
        sbPath.Append("\ScanResult")
        szPath = sbPath.ToString()

        Try
            System.IO.Directory.CreateDirectory(szPath)
        Catch
            MessageBox.Show(ConstComStr.MSG_05_000, ConstComStr.CAPTION_05_000, MessageBoxButtons.OK, MessageBoxIcon.[Error])
            Return
        End Try


        ' Save image to file
        sbPath.Append("\" & nTransactionNumber.ToString("000") & ("_Image_") & (If(objImageResult.GetGradation() = ImageType.MF_SCAN_IMAGE_INFRARED, "IR_", "")) & (If(objImageResult.GetFace() = ScanSide.MF_SCAN_FACE_BACK, "Back", "Front")) & (".") & strExt)
        szPath = sbPath.ToString()

        If objImageResult.GetImageData() IsNot Nothing Then
            Dim cFile As System.IO.FileStream
            Try
                cFile = New System.IO.FileStream(szPath, System.IO.FileMode.Create)
            Catch
                MessageBox.Show(ConstComStr.MSG_05_001, ConstComStr.CAPTION_05_000, MessageBoxButtons.OK, MessageBoxIcon.[Error])
                Return
            End Try

            Dim byBuffer As Byte() = New Byte(objImageResult.GetImageSize() - 1) {}

            ' Save in file
            Try
                objImageResult.GetImageData().Read(byBuffer, 0, byBuffer.Length)
                cFile.Write(byBuffer, 0, byBuffer.Length)

                objImageResult.GetImageData().Close()
            Catch
                MessageBox.Show(ConstComStr.MSG_05_001, ConstComStr.CAPTION_05_001, MessageBoxButtons.OK, MessageBoxIcon.[Error])
                Return
            Finally
                cFile.Close()
            End Try
        End If

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
            TextBox1.Dispatcher.BeginInvoke(New StartTheInvoke_TextBox1(AddressOf NowStartTheInvoke_TextBox1), m_objConfigData.bCheck_Scan_SaveFile.ToString)

            If m_objConfigData.bCheck_Scan_SaveFile = ConstComVal.VAL_CONFIGDLG_CHECK_MI_SAVEFILE_ON Then
                SaveImageData(ScanSide.MF_SCAN_FACE_FRONT, ImageType.MF_SCAN_IMAGE_VISIBLE)
                SaveImageData(ScanSide.MF_SCAN_FACE_BACK, ImageType.MF_SCAN_IMAGE_VISIBLE)
            End If
        ElseIf m_objConfigData.nRadio_ImageChannel = ConstComVal.VAL_CONFIGDLG_RADIO_IC_IR Then
            ' Configure 2 plane data for IR image
            UpdateImageData(ScanSide.MF_SCAN_FACE_FRONT, ImageType.MF_SCAN_IMAGE_INFRARED, False)
            UpdateImageData(ScanSide.MF_SCAN_FACE_BACK, ImageType.MF_SCAN_IMAGE_INFRARED, False)

            ' Save image if necessary
            If m_objConfigData.bCheck_Scan_SaveFile = ConstComVal.VAL_CONFIGDLG_CHECK_MI_SAVEFILE_ON Then
                SaveImageData(ScanSide.MF_SCAN_FACE_FRONT, ImageType.MF_SCAN_IMAGE_INFRARED)
                SaveImageData(ScanSide.MF_SCAN_FACE_BACK, ImageType.MF_SCAN_IMAGE_INFRARED)
            End If
        ElseIf m_objConfigData.nRadio_ImageChannel = ConstComVal.VAL_CONFIGDLG_RADIO_IC_RGBIR Then
            ' Configure 4 plane image
            UpdateImageData(ScanSide.MF_SCAN_FACE_FRONT, ImageType.MF_SCAN_IMAGE_VISIBLE, True)
            UpdateImageData(ScanSide.MF_SCAN_FACE_BACK, ImageType.MF_SCAN_IMAGE_VISIBLE, True)
            UpdateImageData(ScanSide.MF_SCAN_FACE_FRONT, ImageType.MF_SCAN_IMAGE_INFRARED, True)
            UpdateImageData(ScanSide.MF_SCAN_FACE_BACK, ImageType.MF_SCAN_IMAGE_INFRARED, True)

            ' Save image if necessary
            If m_objConfigData.bCheck_Scan_SaveFile = ConstComVal.VAL_CONFIGDLG_CHECK_MI_SAVEFILE_ON Then
                SaveImageData(ScanSide.MF_SCAN_FACE_FRONT, ImageType.MF_SCAN_IMAGE_VISIBLE)
                SaveImageData(ScanSide.MF_SCAN_FACE_BACK, ImageType.MF_SCAN_IMAGE_VISIBLE)
                SaveImageData(ScanSide.MF_SCAN_FACE_FRONT, ImageType.MF_SCAN_IMAGE_INFRARED)
                SaveImageData(ScanSide.MF_SCAN_FACE_BACK, ImageType.MF_SCAN_IMAGE_INFRARED)
            End If
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
            Else
                m_biImage2Back = New Bitmap(cBitmap)
            End If

            Me.stImage2Front.Source = IVS.Epson.MyImage.GetBitmapSource(m_biImage2Front)
            Me.stImage2Back.Source = IVS.Epson.MyImage.GetBitmapSource(m_biImage2Back)

            cBitmap.Dispose()

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

        Me.stImage2Front.Source = Nothing
        Me.stImage2Back.Source = Nothing

    End Sub

    'MICR
    Private Sub CallbackDisplayMicrText(errResult As ErrorCode, cMicrResult As CMicrResult)
        'http://blog.somecreativity.com/2008/01/10/wpf-equivalent-of-invokerequired/
        If (Me.edtMicrText.Dispatcher.CheckAccess()) Then

            Dim strMicrRet As String = Nothing

            If errResult = ErrorCode.SUCCESS Then
                edtMicrText.Text = cMicrResult.GetMicrStr()
            Else
                strMicrRet = GetErrorString(errResult)
                edtMicrText.Text = strMicrRet
            End If

        Else
            edtMicrText.Dispatcher.BeginInvoke(New StartTheInvoke_edtMicrText(AddressOf NowStartTheInvoke_edtMicrText), cMicrResult.GetMicrStr())
        End If

    End Sub

    Private Sub CallbackSaveMicrText(errResult As ErrorCode, cMicrResult As CMicrResult)

        ' Obtain MICR data
        Dim nTransactionNumber As Integer = cMicrResult.GetTransactionNumber()

        If errResult <> ErrorCode.SUCCESS Then
            ShowErrorMessage(errResult)
            Return
        End If

        ' Create Folder
        Dim sbPath As New StringBuilder()
        Dim szPath As String = Nothing

        sbPath.Append(System.IO.Directory.GetCurrentDirectory())
        sbPath.Append("\ScanResult")
        szPath = sbPath.ToString()
        Try
            System.IO.Directory.CreateDirectory(szPath)
        Catch
            MessageBox.Show(ConstComStr.MSG_05_000, ConstComStr.CAPTION_05_000, MessageBoxButtons.OK, MessageBoxIcon.[Error])
            Return
        End Try

        ' Save data to file
        sbPath.Append("\" & nTransactionNumber.ToString("000") & ".Micr.txt")
        szPath = sbPath.ToString()

        Dim swFile As System.IO.StreamWriter
        Try
            swFile = New System.IO.StreamWriter(szPath)
        Catch
            MessageBox.Show(ConstComStr.MSG_05_001, ConstComStr.CAPTION_05_001, MessageBoxButtons.OK, MessageBoxIcon.[Error])
            Return
        End Try

        Try
            ' Transaction number
            swFile.WriteLine("# General Transaction Information:")
            swFile.WriteLine("TransactionNumber = " & nTransactionNumber.ToString())

            ' MICR reading status
            swFile.WriteLine("")
            swFile.WriteLine("# Magnetic Character Results:")
            swFile.WriteLine("Status = " & cMicrResult.GetStatus().ToString())

            ' Detail MICR reading status
            swFile.WriteLine("Detail = " & cMicrResult.GetDetail().ToString())

            ' Return value from MICR recognition
            swFile.WriteLine("MicrResult = " & GetErrorString(cMicrResult.GetResult()))

            ' Obtained MICR strings
            swFile.WriteLine("MicrString = " & (If(cMicrResult.GetMicrStr() IsNot Nothing, cMicrResult.GetMicrStr(), "")))

            'MicrAccountNumber
            swFile.WriteLine("MicrAccountNumber = " & (If(cMicrResult.GetAccountNumber() IsNot Nothing, cMicrResult.GetAccountNumber(), "")))

            'MicrAmount
            swFile.WriteLine("MicrAmount        = " & (If(cMicrResult.GetAmount() IsNot Nothing, cMicrResult.GetAmount(), "")))

            'MicrBankNumber
            swFile.WriteLine("MicrBankNumber    = " & (If(cMicrResult.GetBankNumber() IsNot Nothing, cMicrResult.GetBankNumber(), "")))

            'MicrSerialNumber
            swFile.WriteLine("MicrSerialNumber  = " & (If(cMicrResult.GetSerialNumber() IsNot Nothing, cMicrResult.GetSerialNumber(), "")))

            'MicrEPC
            swFile.WriteLine("MicrEPC           = " & (If(cMicrResult.GetEPC() IsNot Nothing, cMicrResult.GetEPC(), "")))

            ' MicrTransitNumber
            swFile.WriteLine("MicrTransitNumber = " & (If(cMicrResult.GetTransitNumber() IsNot Nothing, cMicrResult.GetTransitNumber(), "")))

            ' MicrCheckType
            swFile.WriteLine("MicrCheckType     = " & cMicrResult.GetCheckType().ToString())

            ' MicrCountryCode
            swFile.WriteLine("MicrCountryCode     = " & cMicrResult.GetCountryCode().ToString())

            ' MicrOnUSField
            swFile.WriteLine("MicrOnUSField = " & (If(cMicrResult.GetOnUSField() IsNot Nothing, cMicrResult.GetOnUSField(), "")))

            ' MicrAuxiliatyOnUSField

            swFile.WriteLine("MicrAuxiliatyOnUSField = " & (If(cMicrResult.GetAuxiliatyOnUSField() IsNot Nothing, cMicrResult.GetAuxiliatyOnUSField(), "")))
        Catch
            MessageBox.Show(ConstComStr.MSG_05_001, ConstComStr.CAPTION_05_001, MessageBoxButtons.OK, MessageBoxIcon.[Error])
            Return
        Finally
            swFile.Close()
        End Try

    End Sub

    Private Sub CallbackProcMicr()
        ' Obtain MICR data
        Dim cMicrResult As New CMicrResult()
        Dim errResult As ErrorCode = m_objDriverControl.GetMicr(cMicrResult)
        CallbackFuncDisplayMicrText(errResult, cMicrResult)

        TextBox1.Dispatcher.BeginInvoke(New StartTheInvoke_TextBox1(AddressOf NowStartTheInvoke_TextBox1), "bCheck_MICR_SaveFile:" & m_objConfigData.bCheck_MICR_SaveFile)

        ' Save MICR data if necessary
        ' If m_objConfigData.bCheck_MICR_SaveFile = ConstComVal.VAL_CONFIGDLG_CHECK_MI_SAVEFILE_ON Then
        CallbackSaveMicrText(errResult, cMicrResult)
        'End If
    End Sub

    Delegate Sub StartTheInvoke_edtMicrText(ByVal message As String)

    Friend Sub NowStartTheInvoke_edtMicrText(ByVal micr As String)

        Try
            Me.edtMicrText.Text = micr
        Catch ex As Exception
            'WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    'MAG

    Private Sub MainWindow_GotFocus(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.GotFocus
        Me.txtReaderInput.Focus()
    End Sub

    Private Sub txtReaderInput_TextInput(sender As Object, e As System.Windows.Input.TextCompositionEventArgs) Handles txtReaderInput.TextInput
        Me.TextBox1.Text += Now + " txtReaderInput_TextInput" + vbCrLf
        'Dim CharCount As Integer = 0
        'Dim intTotalCharCount As Integer = 0

        'Try

        '    For Each c As Char In txtReaderInput.Text

        '        If c = "?" Then CharCount += 1
        '        If c = "%" Then Me.edtMicrText.Text = "Reading Card Data"
        '        'If c = "%" Then edtMicrText.Dispatcher.BeginInvoke(New StartTheInvoke_edtMicrText(AddressOf NowStartTheInvoke_edtMicrText), "Reading Card Data")

        '    Next


        '    'Me.TextBox1.Text += "CharCount:" + CharCount + vbCrLf
        '    If CharCount = 3 Then

        '        Dim strTrackData As String = txtReaderInput.Text
        '        edtMicrText.Dispatcher.BeginInvoke(New StartTheInvoke_edtMicrText(AddressOf NowStartTheInvoke_edtMicrText), strTrackData)
        '        TextBox1.Dispatcher.BeginInvoke(New StartTheInvoke_TextBox1(AddressOf NowStartTheInvoke_TextBox1), "CharCount:" + CharCount)
        '        Me.edtMicrText.Text = strTrackData
        '        txtReaderInput.Text = Nothing
        '    Else

        '        edtMicrText.Dispatcher.BeginInvoke(New StartTheInvoke_edtMicrText(AddressOf NowStartTheInvoke_edtMicrText), txtReaderInput.Text)
        '        TextBox1.Dispatcher.BeginInvoke(New StartTheInvoke_TextBox1(AddressOf NowStartTheInvoke_TextBox1), "CharCount:" + CharCount)
        '    End If

        '    'Me.TextBox1.Text += "CharCount:" + CharCount + vbCrLf
        'Catch ex As Exception
        '    'MyAppLog.WriteToLog("WinESeek.txtReaderInput_TextChanged()" & ex.ToString)
        '    TextBox1.Dispatcher.BeginInvoke(New StartTheInvoke_TextBox1(AddressOf NowStartTheInvoke_TextBox1), ex.ToString)
        'End Try

    End Sub
    Private Sub txtReaderInput_TextChanged(sender As Object, e As System.Windows.Controls.TextChangedEventArgs) Handles txtReaderInput.TextChanged
        'Me.TextBox1.Text += Now + " txtReaderInput_TextChanged" + vbCrLf

        Dim CharCount As Integer = 0
        Dim intTotalCharCount As Integer = 0

        Try

            For Each c As Char In txtReaderInput.Text

                If c = "?" Then CharCount += 1
                'If c = "%" Then Me.edtMicrText.Text = "Reading Card Data"
                'If c = "%" Then edtMicrText.Dispatcher.BeginInvoke(New StartTheInvoke_edtMicrText(AddressOf NowStartTheInvoke_edtMicrText), "Reading Card Data")
                'Me.TextBox1.Text += "CharCount:" + CharCount + vbCrLf
            Next


            'Me.TextBox1.Text += "CharCount:" + CharCount + vbCrLf

            If CharCount = 3 Then

                Dim strTrackData As String = txtReaderInput.Text
                'edtMicrText.Dispatcher.BeginInvoke(New StartTheInvoke_edtMicrText(AddressOf NowStartTheInvoke_edtMicrText), strTrackData)
                Me.edtMicrText.Text = strTrackData
                txtReaderInput.Text = Nothing
                Me.TextBox1.Text += "3" + vbCrLf
            Else
                'Me.TextBox1.Text += "CharCount:" + CharCount + vbCrLf
                'TextBox1.Dispatcher.BeginInvoke(New StartTheInvoke_TextBox1(AddressOf NowStartTheInvoke_TextBox1), "CharCount:" + CharCount)
            End If
            'Me.TextBox1.Text += "CharCount:" + CharCount + vbCrLf
        Catch ex As Exception
            'MyAppLog.WriteToLog("WinESeek.txtReaderInput_TextChanged()" & ex.ToString)
        End Try

    End Sub

    'TEST
    Delegate Sub StartTheInvoke_TextBox1(ByVal message As String)

    Friend Sub NowStartTheInvoke_TextBox1(ByVal message As String)

        Try
            Me.TextBox1.Text += message & vbCrLf

        Catch ex As Exception
            'WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdClose_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdClose.Click
        Dim errRet As ErrorCode = ErrorCode.SUCCESS

        errRet = m_objDriverControl.ExitDevice()
        If errRet <> ErrorCode.SUCCESS Then
            ShowErrorMessage(errRet)
        End If

        Close()
    End Sub

    Private Sub cmdScanCancel_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdScanCancel.Click
        If m_objDriverControl Is Nothing Then
            Return
        End If

        Dim errRet As ErrorCode = m_objDriverControl.CancelScan()
        If errRet <> ErrorCode.SUCCESS Then
            m_bScanCancelError = True
            ShowErrorMessage(errRet)
            m_objDriverControl.CancelError()
        End If
    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles Button1.Click
        Dim strTest As String = m_objDriverControl.GetTest

        Me.TextBox1.Text = strTest
    End Sub

End Class
