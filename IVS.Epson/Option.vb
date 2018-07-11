Imports com.epson.bank.driver

Class COption
    Private m_objProperty As CProperty_1 = Nothing

    Public Sub New()
        m_objProperty = New CProperty_1()
    End Sub

    Public Function Init(objProperty As CProperty_1) As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS

        m_objProperty = objProperty

        Return errResult
    End Function

    ' Set MF_PROCESS01 structure
    Public Function SetProcessSetting() As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS

        errResult = m_objProperty.GetMfDevice().SCNMICRFunctionContinuously(m_objProperty.GetMfProcess(), FunctionType.MF_GET_PROCESS_DEFAULT)
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        m_objProperty.GetMfProcess().ActivationMode = m_objProperty.GetActivation()

        ' Mis-Insertion error
        m_objProperty.GetMfProcess().PaperMisInsertionErrorSelect = m_objProperty.GetPaperMisInsertionDetect()
        If m_objProperty.GetPaperMisInsertionDetect() = MfErrorSelect.MF_ERROR_SELECT_DETECT Then
            m_objProperty.GetMfProcess().PaperMisInsertionCancel = m_objProperty.GetPaperMisInsertionCancel()
            m_objProperty.GetMfProcess().PaperMisInsertionErrorEject = m_objProperty.GetPaperMisInsertionEject()
        End If

        ' Noise error
        m_objProperty.GetMfProcess().NoiseErrorSelect = m_objProperty.GetNoiseDetect()
        If m_objProperty.GetNoiseDetect() = MfErrorSelect.MF_ERROR_SELECT_DETECT Then
            m_objProperty.GetMfProcess().NoiseCancel = m_objProperty.GetNoiseCancel()
            m_objProperty.GetMfProcess().NoiseErrorEject = m_objProperty.GetNoiseEject()
        End If

        ' Double feed error
        m_objProperty.GetMfProcess().DoubleFeedErrorSelect = m_objProperty.GetDoubleFeedDetect()
        If m_objProperty.GetDoubleFeedDetect() = MfErrorSelect.MF_ERROR_SELECT_DETECT Then
            m_objProperty.GetMfProcess().DoubleFeedCancel = m_objProperty.GetDoubleFeedCancel()
            m_objProperty.GetMfProcess().DoubleFeedErrorEject = m_objProperty.GetDoubleFeedEject()
        End If

        ' Bad data in MICR error
        m_objProperty.GetMfProcess().BaddataErrorSelect = m_objProperty.GetBaddataDetect()
        If m_objProperty.GetBaddataDetect() = MfErrorSelect.MF_ERROR_SELECT_DETECT Then
            m_objProperty.GetMfProcess().BaddataCancel = m_objProperty.GetBaddataCancel()
            m_objProperty.GetMfProcess().BaddataErrorEject = m_objProperty.GetBaddataEject()
            m_objProperty.GetMfProcess().BaddataCount = 0
        End If

        ' No data in MICR magnetic waveform error
        m_objProperty.GetMfProcess().NodataErrorSelect = m_objProperty.GetNodataDetect()
        If m_objProperty.GetNodataDetect() = MfErrorSelect.MF_ERROR_SELECT_DETECT Then
            m_objProperty.GetMfProcess().NodataCancel = m_objProperty.GetNodataCancel()
            m_objProperty.GetMfProcess().NodataErrorEject = m_objProperty.GetNodataEject()
        End If

        ' Set endorsement printing mode
        m_objProperty.GetMfProcess().EndorsePrintMode = m_objProperty.GetEndorsePrintMode()

        ' Print data unreceived error
        m_objProperty.GetMfProcess().PrnDataUnreceiveCancel = m_objProperty.GetPrnDataUnreceiveCancel()

        errResult = m_objProperty.GetMfDevice().SCNMICRFunctionContinuously(m_objProperty.GetMfProcess(), FunctionType.MF_SET_PROCESS_PARAM)

        Return errResult
    End Function

    ' Set Waterfall Mode
    Public Function SetWaterfallMode() As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS

        errResult = m_objProperty.GetMfDevice().SetWaterfallMode(m_objProperty.GetWaterfall())

        Return errResult
    End Function

    ' Perform scanning using confirmation mode
    Public Function ConfirmationFunction() As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS

        errResult = m_objProperty.GetMfDevice().SetBehaviorToScnResult(m_objProperty.GetConfirmationEject(), MfStamp.MF_STAMP_DISABLE, m_objProperty.GetConfirmationCancel())

        Return errResult
    End Function
End Class
