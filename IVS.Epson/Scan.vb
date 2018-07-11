Imports com.epson.bank.driver

Class CScan
    Private m_objProperty As CProperty_1 = Nothing

    Public Sub New()
        m_objProperty = New CProperty_1()
    End Sub

    Public Function Init(objProperty As CProperty_1) As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS

        m_objProperty = objProperty


        Return errResult
    End Function

    ' Start scanning
    Public Function ScanFunction() As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS

        errResult = m_objProperty.GetMfDevice().SCNMICRFunctionContinuously(Nothing, FunctionType.MF_EXEC)

        Return errResult
    End Function

    ' Set MF_BASE01 structure
    Public Function SetBaseSetting() As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS

        errResult = m_objProperty.GetMfDevice().SCNSelectScanUnit(m_objProperty.GetScanMedia())
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        errResult = m_objProperty.GetMfDevice().SCNMICRFunctionContinuously(m_objProperty.GetMfBase(), FunctionType.MF_GET_BASE_DEFAULT)
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        m_objProperty.GetMfBase().ErrorEject = MfEjectType.MF_EXIT_ERROR_DISCHARGE

        ' Set buzzer settings
        If m_objProperty.GetBuzzerSuccess() = True Then
            m_objProperty.GetMfBase().BuzzerCount(MfBuzzerType.MF_BUZZER_TYPE_SUCCESS) = MfBuzzerCount.MF_BUZZER_COUNT_ONE
            m_objProperty.GetMfBase().BuzzerHz(MfBuzzerType.MF_BUZZER_TYPE_SUCCESS) = MfBuzzerHz.MF_BUZZER_HZ_4000
        Else
            m_objProperty.GetMfBase().BuzzerCount(MfBuzzerType.MF_BUZZER_TYPE_SUCCESS) = MfBuzzerCount.MF_BUZZER_DISABLE
        End If

        If m_objProperty.GetBuzzerError() = True Then
            m_objProperty.GetMfBase().BuzzerCount(MfBuzzerType.MF_BUZZER_TYPE_ERROR) = MfBuzzerCount.MF_BUZZER_COUNT_TWO
            m_objProperty.GetMfBase().BuzzerHz(MfBuzzerType.MF_BUZZER_TYPE_ERROR) = MfBuzzerHz.MF_BUZZER_HZ_880
        Else
            m_objProperty.GetMfBase().BuzzerCount(MfBuzzerType.MF_BUZZER_TYPE_ERROR) = MfBuzzerCount.MF_BUZZER_DISABLE
        End If

        If m_objProperty.GetBuzzerDoubleFeed() = True Then
            m_objProperty.GetMfBase().BuzzerCount(MfBuzzerType.MF_BUZZER_TYPE_WFEED) = MfBuzzerCount.MF_BUZZER_COUNT_MAX
            m_objProperty.GetMfBase().BuzzerHz(MfBuzzerType.MF_BUZZER_TYPE_WFEED) = MfBuzzerHz.MF_BUZZER_HZ_440
        Else
            m_objProperty.GetMfBase().BuzzerCount(MfBuzzerType.MF_BUZZER_TYPE_WFEED) = MfBuzzerCount.MF_BUZZER_DISABLE
        End If

        errResult = m_objProperty.GetMfDevice().SCNMICRFunctionContinuously(m_objProperty.GetMfBase(), FunctionType.MF_SET_BASE_PARAM)

        Return errResult
    End Function

    ' Set scanning conditions
    Public Function SetScanSetting() As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS

        ' For front side image
        errResult = m_objProperty.GetMfDevice().SCNMICRFunctionContinuously(m_objProperty.GetMfScanFront(), FunctionType.MF_GET_SCAN_FRONT_DEFAULT)
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        m_objProperty.GetMfScanFront().Resolution = m_objProperty.GetResolution()

        errResult = m_objProperty.GetMfDevice().SCNMICRFunctionContinuously(m_objProperty.GetMfScanFront(), FunctionType.MF_SET_SCAN_FRONT_PARAM)
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        errResult = m_objProperty.GetMfDevice().SCNSelectScanFace(ScanSide.MF_SCAN_FACE_FRONT)
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        errResult = m_objProperty.GetMfDevice().SCNSetImageTypeOption(m_objProperty.GetImageChannel())
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        ' For back side image
        errResult = m_objProperty.GetMfDevice().SCNMICRFunctionContinuously(m_objProperty.GetMfScanBack(), FunctionType.MF_GET_SCAN_BACK_DEFAULT)
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        m_objProperty.GetMfScanBack().Resolution = m_objProperty.GetResolution()

        errResult = m_objProperty.GetMfDevice().SCNMICRFunctionContinuously(m_objProperty.GetMfScanBack(), FunctionType.MF_SET_SCAN_BACK_PARAM)
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        errResult = m_objProperty.GetMfDevice().SCNSelectScanFace(ScanSide.MF_SCAN_FACE_BACK)
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        errResult = m_objProperty.GetMfDevice().SCNSetImageTypeOption(m_objProperty.GetImageChannel())
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        Return errResult
    End Function

    ' Scanning cancellation
    Public Function CancelScan() As ErrorCode
        Return m_objProperty.GetMfDevice().SCNMICRCancelFunction(MfEjectType.MF_EJECT_DISCHARGE)
    End Function
End Class
