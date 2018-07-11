Imports com.epson.bank.driver

Class CProperty_1

    Private m_objmfDevice As MFDevice = Nothing
    Private m_objmfBase As MFBase = Nothing
    Private m_objmfScanFront As MFScan = Nothing
    Private m_objmfScanBack As MFScan = Nothing
    Private m_objmfMicr As MFMicr = Nothing
    Private m_objmfProcess As MFProcess = Nothing

    Private m_eScanMedia As ScanUnit
    Private m_eImageChannel As ImageTypeOption
    Private m_eRGBColorDepth As ColorDepth
    Private m_eIRColorDepth As ColorDepth
    Private m_eResolution As MfScanDpi
    Private m_eFont As MfMicrFont
    Private m_bParsing As Boolean
    Private m_bClearSpace As Boolean
    Private m_nTransactionNumber As Integer

    Private m_bBuzzerSuccess As Boolean
    Private m_bBuzzerDoubleFeed As Boolean
    Private m_bBuzzerError As Boolean

    'Step 4
    Private m_eWaterfall As WaterfallMode
    Private m_eActivation As MfActivateMode
    Private m_eConfirmationEject As MfEject
    Private m_eConfirmationCancel As MfProcessContinue
    Private m_ePaperMisInsertionDetect As MfErrorSelect
    Private m_ePaperMisInsertionEject As MfEject
    Private m_ePaperMisInsertionCancel As MfCancel
    Private m_eNoiseDetect As MfErrorSelect
    Private m_eNoiseEject As MfEject
    Private m_eNoiseCancel As MfCancel
    Private m_eDoubleFeedDetect As MfErrorSelect
    Private m_eDoubleFeedEject As MfEject
    Private m_eDoubleFeedCancel As MfCancel
    Private m_eBaddataDetect As MfErrorSelect
    Private m_eBaddataEject As MfEject
    Private m_eBaddataCancel As MfCancel
    Private m_eNodataDetect As MfErrorSelect
    Private m_eNodataEject As MfEject
    Private m_eNodataCancel As MfCancel
    Private m_eEndorsePrintMode As MfEndorsePrintMode
    Private m_ePrnDataUnreceiveCancel As MfCancel
    Private m_eEndorseType As PrintingStation

    Private m_bPrintText As Boolean
    Private m_bPrintImage As Boolean

    Public Sub New()
        m_objmfDevice = New MFDevice()
        m_objmfBase = New MFBase()
        m_objmfScanFront = New MFScan()
        m_objmfScanBack = New MFScan()
        m_objmfMicr = New MFMicr()
        m_objmfProcess = New MFProcess()

        m_eScanMedia = ScanUnit.EPS_BI_SCN_UNIT_CHECKPAPER
        m_eImageChannel = ImageTypeOption.EPS_BI_SCN_OPTION_COLOR
        m_eRGBColorDepth = ColorDepth.EPS_BI_SCN_24BIT
        m_eIRColorDepth = ColorDepth.EPS_BI_SCN_8BIT
        m_eResolution = MfScanDpi.MF_SCAN_DPI_200
        m_eFont = MfMicrFont.MF_MICR_FONT_E13B
        m_bParsing = False
        m_bClearSpace = False
        m_nTransactionNumber = 0

        m_bBuzzerSuccess = False
        m_bBuzzerDoubleFeed = False
        m_bBuzzerError = False

        'Step 4
        m_eWaterfall = WaterfallMode.WATERFALL_MODE_DISABLE
        m_eActivation = MfActivateMode.MF_ACTIVATE_MODE_HIGH_SPEED
        m_eConfirmationEject = MfEject.MF_EJECT_MAIN_POCKET
        m_eConfirmationCancel = MfProcessContinue.MF_PROCESS_CONTINUE_OVERLAP
        m_ePaperMisInsertionDetect = MfErrorSelect.MF_ERROR_SELECT_DETECT
        m_ePaperMisInsertionEject = MfEject.MF_EJECT_MAIN_POCKET
        m_ePaperMisInsertionCancel = MfCancel.MF_CANCEL_DISABLE
        m_eNoiseDetect = MfErrorSelect.MF_ERROR_SELECT_DETECT
        m_eNoiseEject = MfEject.MF_EJECT_MAIN_POCKET
        m_eNoiseCancel = MfCancel.MF_CANCEL_DISABLE
        m_eDoubleFeedDetect = MfErrorSelect.MF_ERROR_SELECT_DETECT
        m_eDoubleFeedEject = MfEject.MF_EJECT_MAIN_POCKET
        m_eDoubleFeedCancel = MfCancel.MF_CANCEL_DISABLE
        m_eBaddataDetect = MfErrorSelect.MF_ERROR_SELECT_DETECT
        m_eBaddataEject = MfEject.MF_EJECT_MAIN_POCKET
        m_eBaddataCancel = MfCancel.MF_CANCEL_DISABLE
        m_eNodataDetect = MfErrorSelect.MF_ERROR_SELECT_DETECT
        m_eNodataEject = MfEject.MF_EJECT_MAIN_POCKET
        m_eNodataCancel = MfCancel.MF_CANCEL_DISABLE
        m_eEndorsePrintMode = MfEndorsePrintMode.MF_ENDORSEPRINT_MODE_HIGHSPEED
        m_ePrnDataUnreceiveCancel = MfCancel.MF_CANCEL_ENABLE
        m_eEndorseType = PrintingStation.MF_ST_PHYSICAL_ENDORSEMENT
        m_bPrintText = False

        m_bPrintImage = False
    End Sub

    Public Sub SetMfDevice(objmfDevice As MFDevice)
        m_objmfDevice = objmfDevice
    End Sub

    Public Function GetMfDevice() As MFDevice
        Return m_objmfDevice
    End Function

    Public Function GetMfBase() As MFBase
        Return m_objmfBase
    End Function

    Public Function GetMfMicr() As MFMicr
        Return m_objmfMicr
    End Function

    Public Function GetMfScanFront() As MFScan
        Return m_objmfScanFront
    End Function

    Public Function GetMfScanBack() As MFScan
        Return m_objmfScanBack
    End Function

    Public Function GetMfProcess() As MFProcess
        Return m_objmfProcess
    End Function

    Public Sub SetScanMedia(eScanMedia As ScanUnit)
        m_eScanMedia = eScanMedia
    End Sub

    Public Function GetScanMedia() As ScanUnit
        Return m_eScanMedia
    End Function

    Public Sub SetImageChannel(eImageChannel As ImageTypeOption)
        m_eImageChannel = eImageChannel
    End Sub

    Public Function GetImageChannel() As ImageTypeOption
        Return m_eImageChannel
    End Function

    Public Sub SetRGBColorDepth(eColorDepth As ColorDepth)
        m_eRGBColorDepth = eColorDepth
    End Sub

    Public Function GetRGBColorDepth() As ColorDepth
        Return m_eRGBColorDepth
    End Function

    Public Sub SetIRColorDepth(eColorDepth As ColorDepth)
        m_eIRColorDepth = eColorDepth
    End Sub

    Public Function GetIRColorDepth() As ColorDepth
        Return m_eIRColorDepth
    End Function

    Public Sub SetResolution(eResolution As MfScanDpi)
        m_eResolution = eResolution
    End Sub

    Public Function GetResolution() As MfScanDpi
        Return m_eResolution
    End Function

    Public Sub SetFont(eFont As MfMicrFont)
        m_eFont = eFont
    End Sub

    Public Function GetFont() As MfMicrFont
        Return m_eFont
    End Function

    Public Sub SetParsing(bParsing As Boolean)
        m_bParsing = bParsing
    End Sub

    Public Function GetParsing() As Boolean
        Return m_bParsing
    End Function

    Public Sub SetClearSpace(bClearSpace As Boolean)
        m_bClearSpace = bClearSpace
    End Sub

    Public Function GetClearSpace() As Boolean
        Return m_bClearSpace
    End Function

    Public Sub SetTransactionNumber(nTransactionNumber As Integer)
        m_nTransactionNumber = nTransactionNumber
    End Sub

    Public Function GetTransactionNumber() As Integer
        Return m_nTransactionNumber
    End Function

    Public Sub SetBuzzerSuccess(bBuzzerSuccess As Boolean)
        m_bBuzzerSuccess = bBuzzerSuccess
    End Sub

    Public Function GetBuzzerSuccess() As Boolean
        Return m_bBuzzerSuccess
    End Function

    Public Sub SetBuzzerDoubleFeed(bBuzzerDoubleFeed As Boolean)
        m_bBuzzerDoubleFeed = bBuzzerDoubleFeed
    End Sub

    Public Function GetBuzzerDoubleFeed() As Boolean
        Return m_bBuzzerDoubleFeed
    End Function

    Public Sub SetBuzzerError(bBuzzerError As Boolean)
        m_bBuzzerError = bBuzzerError
    End Sub

    Public Function GetBuzzerError() As Boolean
        Return m_bBuzzerError
    End Function

    '''/////////////
    '''/ Step 4
    '''/////////////
    Public Sub SetEndorseType(eEndorseType As PrintingStation)
        m_eEndorseType = eEndorseType
    End Sub

    Public Function GetEndorseType() As PrintingStation
        Return m_eEndorseType
    End Function

    Public Sub SetPrintText(bPrintText As Boolean)
        m_bPrintText = bPrintText
    End Sub

    Public Function GetPrintText() As Boolean
        Return m_bPrintText
    End Function

    Public Sub SetPrintImage(bPrintImage As Boolean)
        m_bPrintImage = bPrintImage
    End Sub

    Public Function GetPrintImage() As Boolean
        Return m_bPrintImage
    End Function

    Public Sub SetWaterfall(eWaterfall As WaterfallMode)
        m_eWaterfall = eWaterfall
    End Sub

    Public Function GetWaterfall() As WaterfallMode
        Return m_eWaterfall
    End Function

    Public Sub SetActivation(eActivation As MfActivateMode)
        m_eActivation = eActivation
    End Sub

    Public Function GetActivation() As MfActivateMode
        Return m_eActivation
    End Function

    Public Sub SetConfirmationEject(eConfirmationEject As MfEject)
        m_eConfirmationEject = eConfirmationEject
    End Sub

    Public Function GetConfirmationEject() As MfEject
        Return m_eConfirmationEject
    End Function

    Public Sub SetConfirmationCancel(eConfirmationCancel As MfProcessContinue)
        m_eConfirmationCancel = eConfirmationCancel
    End Sub

    Public Function GetConfirmationCancel() As MfProcessContinue
        Return m_eConfirmationCancel
    End Function

    Public Sub SetPaperMisInsertionDetect(ePaperMisInsertionDetect As MfErrorSelect)
        m_ePaperMisInsertionDetect = ePaperMisInsertionDetect
    End Sub

    Public Function GetPaperMisInsertionDetect() As MfErrorSelect
        Return m_ePaperMisInsertionDetect
    End Function

    Public Sub SetPaperMisInsertionEject(ePaperMisInsertionEject As MfEject)
        m_ePaperMisInsertionEject = ePaperMisInsertionEject
    End Sub

    Public Function GetPaperMisInsertionEject() As MfEject
        Return m_ePaperMisInsertionEject
    End Function

    Public Sub SetPaperMisInsertionCancel(ePaperMisInsertionCancel As MfCancel)
        m_ePaperMisInsertionCancel = ePaperMisInsertionCancel
    End Sub

    Public Function GetPaperMisInsertionCancel() As MfCancel
        Return m_ePaperMisInsertionCancel
    End Function

    Public Sub SetNoiseDetect(eNoiseDetect As MfErrorSelect)
        m_eNoiseDetect = eNoiseDetect
    End Sub

    Public Function GetNoiseDetect() As MfErrorSelect
        Return m_eNoiseDetect
    End Function

    Public Sub SetNoiseEject(eNoiseEject As MfEject)
        m_eNoiseEject = eNoiseEject
    End Sub

    Public Function GetNoiseEject() As MfEject
        Return m_eNoiseEject
    End Function

    Public Sub SetNoiseCancel(eNoiseCancel As MfCancel)
        m_eNoiseCancel = eNoiseCancel
    End Sub

    Public Function GetNoiseCancel() As MfCancel
        Return m_eNoiseCancel
    End Function

    Public Sub SetDoubleFeedDetect(eDoubleFeedDetect As MfErrorSelect)
        m_eDoubleFeedDetect = eDoubleFeedDetect
    End Sub

    Public Function GetDoubleFeedDetect() As MfErrorSelect
        Return m_eDoubleFeedDetect
    End Function

    Public Sub SetDoubleFeedEject(eDoubleFeedEject As MfEject)
        m_eDoubleFeedEject = eDoubleFeedEject
    End Sub

    Public Function GetDoubleFeedEject() As MfEject
        Return m_eDoubleFeedEject
    End Function

    Public Sub SetDoubleFeedCancel(eDoubleFeedCancel As MfCancel)
        m_eDoubleFeedCancel = eDoubleFeedCancel
    End Sub

    Public Function GetDoubleFeedCancel() As MfCancel
        Return m_eDoubleFeedCancel
    End Function

    Public Sub SetBaddataDetect(eBaddataDetect As MfErrorSelect)
        m_eBaddataDetect = eBaddataDetect
    End Sub

    Public Function GetBaddataDetect() As MfErrorSelect
        Return m_eBaddataDetect
    End Function

    Public Sub SetBaddataEject(eBaddataEject As MfEject)
        m_eBaddataEject = eBaddataEject
    End Sub

    Public Function GetBaddataEject() As MfEject
        Return m_eBaddataEject
    End Function

    Public Sub SetBaddataCancel(eBaddataCancel As MfCancel)
        m_eBaddataCancel = eBaddataCancel
    End Sub

    Public Function GetBaddataCancel() As MfCancel
        Return m_eBaddataCancel
    End Function

    Public Sub SetNodataDetect(eNodataDetect As MfErrorSelect)
        m_eNodataDetect = eNodataDetect
    End Sub

    Public Function GetNodataDetect() As MfErrorSelect
        Return m_eNodataDetect
    End Function

    Public Sub SetNodataEject(eNodataEject As MfEject)
        m_eNodataEject = eNodataEject
    End Sub

    Public Function GetNodataEject() As MfEject
        Return m_eNodataEject
    End Function

    Public Sub SetNodataCancel(eNodataCancel As MfCancel)
        m_eNodataCancel = eNodataCancel
    End Sub

    Public Function GetNodataCancel() As MfCancel
        Return m_eNodataCancel
    End Function

    Public Sub SetEndorsePrintMode(eEndorsePrintMode As MfEndorsePrintMode)
        m_eEndorsePrintMode = eEndorsePrintMode
    End Sub

    Public Function GetEndorsePrintMode() As MfEndorsePrintMode
        Return m_eEndorsePrintMode
    End Function

    Public Sub SetPrnDataUnreceiveCancel(ePrnDataUnreceiveCancel As MfCancel)
        m_ePrnDataUnreceiveCancel = ePrnDataUnreceiveCancel
    End Sub

    Public Function GetPrnDataUnreceiveCancel() As MfCancel
        Return m_ePrnDataUnreceiveCancel
    End Function

End Class
