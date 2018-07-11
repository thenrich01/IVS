Imports com.epson.bank.driver

Public Class CApp
    Public Const EXEC_SCAN_CHECK As Byte = 0
    Public Const EXEC_SCAN_CARD As Byte = 1
    Public Const EXEC_PRINT As Byte = 2

    Public Delegate Sub CallbackProcProcessError(errResult As ErrorCode)
    Public Delegate Sub CallbackProcImage()
    Public Delegate Sub CallbackProcMicr()
    Public Delegate Sub CallbackProcPhysicalEndorse(unTransactionNumber As UInteger)
    Public Delegate Sub CallbackProcElectronicEndorse()

    Private m_cbFuncProcessErrorCB As CallbackProcProcessError = Nothing
    Private m_cbFuncImageCB As CallbackProcImage = Nothing
    Private m_cbFuncMicrCB As CallbackProcMicr = Nothing
    Private m_cbFuncPhysicalEndorseCB As CallbackProcPhysicalEndorse = Nothing
    Private m_cbFuncElectronicEndorseCB As CallbackProcElectronicEndorse = Nothing

    Private m_objProperty As CProperty_1 = Nothing
    Private m_objScan As CScan = Nothing
    Private m_objMicr As CMicr = Nothing
    Private m_objImage As CImage_1 = Nothing
    Private m_objEndorse As CEndorse = Nothing
    Private m_objOption As COption = Nothing

    Private m_byExecType As Byte
    Private m_bOpenDevice As Boolean

    Public Sub New()
        m_objProperty = New CProperty_1()
        m_objScan = New CScan()
        m_objMicr = New CMicr()
        m_objImage = New CImage_1()
        m_objEndorse = New CEndorse()
        m_objOption = New COption()

        m_byExecType = EXEC_SCAN_CHECK
    End Sub

    ' Initialize
    Public Function InitDevice() As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS

        m_bOpenDevice = False

        Try
            ' Connect to the device
            m_objProperty.SetMfDevice(New MFDevice())
            errResult = m_objProperty.GetMfDevice().OpenMonPrinter(OpenType.TYPE_PRINTER, "TM-S9000U")
            If errResult <> ErrorCode.SUCCESS Then
                Return errResult
            End If
            m_bOpenDevice = True

            ' Register callback functions for notification of the reading status
            AddHandler m_objProperty.GetMfDevice().SCNMICRStatusCallback, New MFDevice.SCNMICRStatusCallbackHandler(AddressOf ScanStatus)
            errResult = m_objProperty.GetMfDevice().SCNMICRSetStatusBack()
            If errResult <> ErrorCode.SUCCESS Then
                Return errResult
            End If

            ' Register callback functions for notification of the endorsement data acceptance
            AddHandler m_objProperty.GetMfDevice().StartEndorsementStatusCallback, New MFDevice.StartEndorsementStatusCallbackHandler(AddressOf StartEndorsement)
            errResult = m_objProperty.GetMfDevice().StartEndorsementSetStatusBack()
            If errResult <> ErrorCode.SUCCESS Then
                Return errResult
            End If

            ' Register callback functions for notification of the endorsement data close
            AddHandler m_objProperty.GetMfDevice().EndEndorsementStatusCallback, New MFDevice.EndEndorsementStatusCallbackHandler(AddressOf EndEndorsement)
            errResult = m_objProperty.GetMfDevice().EndEndorsementSetStatusBack()
            If errResult <> ErrorCode.SUCCESS Then
                Return errResult
            End If

            m_objScan.Init(m_objProperty)
            m_objMicr.Init(m_objProperty)
            m_objImage.Init(m_objProperty)
            m_objOption.Init(m_objProperty)
            'Step4

            m_objEndorse.Init(m_objProperty)
        Catch
            Return ErrorCode.ERR_UNKNOWN
        End Try

        Return errResult
    End Function
    'TEST

    Public Function GetTest() As String
        Dim errResult As ErrorCode = ErrorCode.SUCCESS
        Dim _eDriverType As DriverType
        Dim _eType As VersionType
        Dim _ptVersion As MFVersion = Nothing

        Dim etypeID As Byte
        Dim eFont As Byte
        Dim eExrom As Byte
        Dim euspecial As Byte

        'errResult = m_objProperty.GetMfDevice().GetType(etypeID, eFont, eExrom, euspecial) '=74
        errResult = m_objProperty.GetMfBase.Version '=257

        'If errResult <> ErrorCode.SUCCESS Then
        '    Return errResult.ToString
        'End If

        Return errResult

    End Function
    ' Post process
    Public Function ExitDevice() As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS

        If m_bOpenDevice = True Then
            ' Cancel the reading status callback
            errResult = m_objProperty.GetMfDevice().SCNMICRCancelStatusBack()
            If errResult <> ErrorCode.SUCCESS Then
                Return errResult
            End If
            RemoveHandler m_objProperty.GetMfDevice().SCNMICRStatusCallback, New MFDevice.SCNMICRStatusCallbackHandler(AddressOf ScanStatus)

            ' Step4
            ' Cancel the endorsement data acceptance callback
            errResult = m_objProperty.GetMfDevice().StartEndorsementCancelStatusBack()
            If errResult <> ErrorCode.SUCCESS Then
                Return errResult
            End If
            RemoveHandler m_objProperty.GetMfDevice().StartEndorsementStatusCallback, New MFDevice.StartEndorsementStatusCallbackHandler(AddressOf StartEndorsement)

            ' Cancel the endorsement data close callback
            errResult = m_objProperty.GetMfDevice().EndEndorsementCancelStatusBack()
            If errResult <> ErrorCode.SUCCESS Then
                Return errResult
            End If
            RemoveHandler m_objProperty.GetMfDevice().EndEndorsementStatusCallback, New MFDevice.EndEndorsementStatusCallbackHandler(AddressOf EndEndorsement)


            ' Disconnect from device
            errResult = m_objProperty.GetMfDevice().CloseMonPrinter()
            If errResult <> ErrorCode.SUCCESS Then
                Return errResult
            End If
            m_bOpenDevice = False
        End If

        Return errResult
    End Function

    ' Register callback function for notification of the MICR data
    Public Sub SetMicrCallback(cbMicrCB As CallbackProcMicr)
        m_cbFuncMicrCB = cbMicrCB
    End Sub

    ' Register callback function for notification of the image data
    Public Sub SetImageCallback(cbImageCB As CallbackProcImage)
        m_cbFuncImageCB = cbImageCB
    End Sub

    ' Register callback function for notification of error during image acquisition
    Public Sub SetProcessErrorCallback(cbProcessErrorCB As CallbackProcProcessError)
        m_cbFuncProcessErrorCB = cbProcessErrorCB
    End Sub

    ' Set MICR settings
    Public Sub SetMicrParam(ByRef objMicrParam As CMicrParam)
        If objMicrParam IsNot Nothing Then
            m_objProperty.SetFont(objMicrParam.GetFont())
            m_objProperty.SetParsing(objMicrParam.GetParsing())
            m_objProperty.SetClearSpace(objMicrParam.GetClearSpace())
        End If
    End Sub

    ' Get MICR settings
    Public Sub GetMicrParam(ByRef objMicrParam As CMicrParam)
        If objMicrParam IsNot Nothing Then
            objMicrParam.SetFont(m_objProperty.GetFont())
            objMicrParam.SetParsing(m_objProperty.GetParsing())
            objMicrParam.SetClearSpace(m_objProperty.GetClearSpace())
        End If
    End Sub

    ' Set scanning parameters
    Public Sub SetScanParam(ByRef objScanParam As CScanParam)
        If objScanParam IsNot Nothing Then
            m_objProperty.SetScanMedia(objScanParam.GetScanMedia())
            m_objProperty.SetImageChannel(objScanParam.GetImageChannel())
            m_objProperty.SetRGBColorDepth(objScanParam.GetRGBColorDepth())
            m_objProperty.SetIRColorDepth(objScanParam.GetIRColorDepth())
            m_objProperty.SetResolution(objScanParam.GetResolution())
        End If
    End Sub

    ' Get scanning parameters
    Public Sub GetScanParam(ByRef objScanParam As CScanParam)
        If objScanParam IsNot Nothing Then
            objScanParam.SetScanMedia(m_objProperty.GetScanMedia())
            objScanParam.SetImageChannel(m_objProperty.GetImageChannel())
            objScanParam.SetRGBColorDepth(m_objProperty.GetRGBColorDepth())
            objScanParam.SetIRColorDepth(m_objProperty.GetIRColorDepth())
            objScanParam.SetResolution(m_objProperty.GetResolution())
        End If
    End Sub

    ' Check scanning process
    Public Function ScanCheck() As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS

        m_byExecType = EXEC_SCAN_CHECK

        ' Set MF_BASE01 structure
        errResult = m_objScan.SetBaseSetting()
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        ' Set scanning parameters for check scanning
        errResult = m_objScan.SetScanSetting()
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        ' Set MICR parameters
        errResult = m_objMicr.SetMicrSetting()
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        ' Set MF_PROCESS01 structure
        errResult = m_objOption.SetProcessSetting()
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        ' Set endorsement type
        errResult = m_objEndorse.SetEndorseStation()
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        ' Start check scanning
        errResult = m_objScan.ScanFunction()
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        Return errResult
    End Function

    ' Card scanning process
    Public Function ScanCard() As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS

        m_byExecType = EXEC_SCAN_CARD

        ' Clear MF_MICR strucuture
        errResult = m_objMicr.ClearMicrSetting()
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        ' Set MF_BASE01 strucuture
        errResult = m_objScan.SetBaseSetting()
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        ' Set card scanning conditions
        errResult = m_objScan.SetScanSetting()
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        ' Start card scanning
        errResult = m_objScan.ScanFunction()
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        Return errResult
    End Function

    ' Get MICR data
    Public Function GetMicr(ByRef objMicrResult As CMicrResult) As ErrorCode
        Return m_objMicr.GetMicr(objMicrResult)
    End Function

    ' Get image data
    Public Function GetScanImage(ByRef objImageResult As CImageResult) As ErrorCode
        Return m_objImage.GetScanImage(objImageResult)
    End Function

    ' Error recovery
    Public Function CancelError() As ErrorCode
        Return m_objProperty.GetMfDevice().CancelError()
    End Function

    ' Get device status
    Public Sub GetDeviceStatus(ByRef eStatus As ASB)
        eStatus = m_objProperty.GetMfDevice().Status
    End Sub

    ' Cancel scanning
    Public Function CancelScan() As ErrorCode
        Return m_objScan.CancelScan()
    End Function

    ' MICR head cleaning
    Public Function CleaningMicr() As ErrorCode
        Return m_objMicr.CleaningMicr()
    End Function

    ' Callback function for notifying scanning status
    Private Sub ScanStatus(nTransactionNumber As Integer, eMainStatus As MainStatus, eSubStatus As ErrorCode, strPortName As String)

        If eSubStatus <> ErrorCode.SUCCESS Then
            m_cbFuncProcessErrorCB(eSubStatus)
        End If

        If eMainStatus = MainStatus.MF_DATARECEIVE_DONE Then
            m_objProperty.SetTransactionNumber(nTransactionNumber)

            If m_objProperty.GetEndorseType() = PrintingStation.MF_ST_E_ENDORSEMENT Then
                If m_objProperty.GetPrintText() OrElse m_objProperty.GetPrintImage() Then
                    ' Notify callback for endorsement
                    m_cbFuncElectronicEndorseCB()
                End If
            End If

            If m_byExecType = EXEC_SCAN_CHECK Then
                ' Notify callback for MICR data
                m_cbFuncMicrCB()
            End If
            ' Notify callback for image data
            m_cbFuncImageCB()
        End If
    End Sub

    '''/////////////
    '''/ Step 4
    '''/////////////

    ' Register callback for Physical Endorsement
    Public Sub SetPhysicalEndorseCallback(cbPhysicalEndorseCB As CallbackProcPhysicalEndorse)
        m_cbFuncPhysicalEndorseCB = cbPhysicalEndorseCB
    End Sub

    ' Register callback for Electronic Endorsement
    Public Sub SetElectronicEndorseCallback(cbElectronicEndorseCB As CallbackProcElectronicEndorse)
        m_cbFuncElectronicEndorseCB = cbElectronicEndorseCB
    End Sub

    ' Set endorsement data
    Public Sub SetEndorseParam(ByRef objEndorseParam As CEndorseParam)
        If objEndorseParam IsNot Nothing Then
            m_objProperty.SetEndorseType(objEndorseParam.GetEndorseType())
            m_objProperty.SetPrintText(objEndorseParam.GetPrintText())
            m_objProperty.SetPrintImage(objEndorseParam.GetPrintImage())
        End If
    End Sub

    ' Get endorsement data
    Public Sub GetEndorseParam(ByRef objEndorseParam As CEndorseParam)
        If objEndorseParam IsNot Nothing Then
            objEndorseParam.SetEndorseType(m_objProperty.GetEndorseType())
            objEndorseParam.SetPrintText(m_objProperty.GetPrintText())
            objEndorseParam.SetPrintImage(m_objProperty.GetPrintImage())
        End If
    End Sub

    ' Set processing mode options
    Public Sub SetOptionParam(ByRef objOptionParam As COptionParam)
        If objOptionParam IsNot Nothing Then
            m_objProperty.SetEndorsePrintMode(objOptionParam.GetEndorsePrintMode())
            m_objProperty.SetPrnDataUnreceiveCancel(objOptionParam.GetPrnDataUnreceiveCancel())
        End If
    End Sub

    ' Get processing mode options
    Public Sub GetOptionParam(ByRef objOptionParam As COptionParam)
        If objOptionParam IsNot Nothing Then
            objOptionParam.SetEndorsePrintMode(m_objProperty.GetEndorsePrintMode())
            objOptionParam.SetPrnDataUnreceiveCancel(m_objProperty.GetPrnDataUnreceiveCancel())
        End If
    End Sub

    ' Reading endorsement data file
    Public Function LoadEndorseFile() As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS

        errResult = m_objEndorse.LoadEndorseFile()

        Return errResult
    End Function

    ' Set Physical Endorsement data
    Public Function PhysicalEndorseFunction(unTransactionNumber As UInteger) As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS

        errResult = m_objEndorse.PhysicalEndorseFunction(unTransactionNumber)

        Return errResult
    End Function

    ' Set Electronic Endorsement data
    Public Function ElectronicEndorseFunction() As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS

        errResult = m_objEndorse.ElectronicEndorseFunction()

        Return errResult
    End Function

    ' Callback function for notifying endorsement data acceptance
    Private Sub StartEndorsement(nTransactionNumber As Integer, strPortName As String)
        If m_objProperty.GetEndorseType() = PrintingStation.MF_ST_PHYSICAL_ENDORSEMENT Then
            If m_objProperty.GetPrintText() OrElse m_objProperty.GetPrintImage() Then
                ' notifying endorsement data acceptance
                m_cbFuncPhysicalEndorseCB(CUInt(nTransactionNumber))
            End If
        End If
    End Sub

    ' Callback function for notifying endorsement data close
    Private Sub EndEndorsement(nTransactionNumber As Integer, strPortName As String)
        ' After this callback is executed, all endorsement data is discarded 

    End Sub
End Class