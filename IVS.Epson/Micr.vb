Imports com.epson.bank.driver

Class CMicr
    Private m_objProperty As CProperty_1 = Nothing

    Public Sub New()
        m_objProperty = New CProperty_1()
    End Sub

    Public Function Init(objProperty As CProperty_1) As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS
        m_objProperty = objProperty

        Return errResult
    End Function

    ' Set MICR reading conditions
    Public Function SetMicrSetting() As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS
        Dim bParsing As Boolean = False
        Dim eClearSpaces As RemoveSpace = RemoveSpace.CLEAR_SPACE_DISABLE

        errResult = m_objProperty.GetMfDevice().SCNMICRFunctionContinuously(m_objProperty.GetMfMicr(), FunctionType.MF_GET_MICR_DEFAULT)
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        m_objProperty.GetMfMicr().Font = m_objProperty.GetFont()
        If m_objProperty.GetParsing() Then
            bParsing = True
        End If
        m_objProperty.GetMfMicr().Parsing = bParsing

        errResult = m_objProperty.GetMfDevice().SCNMICRFunctionContinuously(m_objProperty.GetMfMicr(), FunctionType.MF_SET_MICR_PARAM)
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        If m_objProperty.GetClearSpace() Then
            eClearSpaces = RemoveSpace.CLEAR_SPACE_ENABLE
        End If
        errResult = m_objProperty.GetMfDevice().MICRClearSpaces(eClearSpaces)

        Return errResult
    End Function

    ' Get MICR data
    Public Function GetMicr(ByRef objMicrResult As CMicrResult) As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS

        errResult = m_objProperty.GetMfDevice().GetMicrText(m_objProperty.GetTransactionNumber(), m_objProperty.GetMfMicr())
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        objMicrResult.SetTransactionNumber(m_objProperty.GetTransactionNumber())
        objMicrResult.SetResult(m_objProperty.GetMfMicr().Ret)
        objMicrResult.SetStatus(m_objProperty.GetMfMicr().Status)
        objMicrResult.SetDetail(m_objProperty.GetMfMicr().Detail)
        objMicrResult.SetMicrStr(m_objProperty.GetMfMicr().MicrStr)
        objMicrResult.SetAccountNumber(m_objProperty.GetMfMicr().AccountNumber)
        objMicrResult.SetAmount(m_objProperty.GetMfMicr().Amount)
        objMicrResult.SetBankNumber(m_objProperty.GetMfMicr().BankNumber)
        objMicrResult.SetSerialNumber(m_objProperty.GetMfMicr().SerialNumber)
        objMicrResult.SetEPC(m_objProperty.GetMfMicr().EPC)
        objMicrResult.SetTransitNumber(m_objProperty.GetMfMicr().TransitNumber)
        objMicrResult.SetCheckType(m_objProperty.GetMfMicr().CheckType)
        objMicrResult.SetCountryCode(m_objProperty.GetMfMicr().CountryCode)
        objMicrResult.SetOnUSField(m_objProperty.GetMfMicr().OnUSField)
        objMicrResult.SetAuxiliatyOnUSField(m_objProperty.GetMfMicr().AuxillatyOnUSField)

        Return errResult
    End Function


    ' MICR cleaning
    Public Function CleaningMicr() As ErrorCode
        Return m_objProperty.GetMfDevice().MICRCleaning()
    End Function

    Public Function ClearMicrSetting() As ErrorCode
        Return m_objProperty.GetMfDevice().SCNMICRFunctionContinuously(m_objProperty.GetMfMicr(), FunctionType.MF_CLEAR_MICR_PARAM)
    End Function

End Class
