Imports com.epson.bank.driver

Public Class COptionParam
    Private m_bOnePiece As Boolean
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

    Public Sub New()
        m_bOnePiece = False
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
    End Sub

    Public Sub SetConfirmationType(bOnePiece As Boolean)
        m_bOnePiece = bOnePiece
    End Sub

    Public Function GetConfirmationType() As Boolean
        Return m_bOnePiece
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
