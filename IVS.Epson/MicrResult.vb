Imports com.epson.bank.driver

Public Class CMicrResult
    Private m_nTransactionNumber As Integer
    Private m_errRet As ErrorCode
    Private m_byStatus As Byte
    Private m_byDetail As Byte
    Private m_szMicrStr As String
    Private m_szAccountNumber As String
    Private m_szAmount As String
    Private m_szBankNumber As String
    Private m_szSerialNumber As String
    Private m_szEPC As String
    Private m_szTransitNumber As String
    Private m_szOnUSField As String
    Private m_szAuxiliatyOnUSField As String
    Private m_eCheckType As Check
    Private m_eCountryCode As Country

    Public Sub New()
        m_nTransactionNumber = 0
        m_errRet = 0
        m_byStatus = 0
        m_byDetail = 0
        m_szMicrStr = Nothing
        m_szAccountNumber = Nothing
        m_szAmount = Nothing
        m_szBankNumber = Nothing
        m_szSerialNumber = Nothing
        m_szEPC = Nothing
        m_szTransitNumber = Nothing
        m_szOnUSField = Nothing
        m_szAuxiliatyOnUSField = Nothing
    End Sub

    Public Sub SetTransactionNumber(nTransactionNumber As Integer)
        m_nTransactionNumber = nTransactionNumber
    End Sub

    Public Function GetTransactionNumber() As Integer
        Return m_nTransactionNumber
    End Function

    Public Sub SetResult(errRet As ErrorCode)
        m_errRet = errRet
    End Sub

    Public Function GetResult() As ErrorCode
        Return m_errRet
    End Function

    Public Sub SetStatus(byStatus As Byte)
        m_byStatus = byStatus
    End Sub

    Public Function GetStatus() As Byte
        Return m_byStatus
    End Function

    Public Sub SetDetail(byDetail As Byte)
        m_byDetail = byDetail
    End Sub

    Public Function GetDetail() As Byte
        Return m_byDetail
    End Function

    Public Sub SetMicrStr(szMicrStr As String)
        m_szMicrStr = szMicrStr
    End Sub

    Public Function GetMicrStr() As String
        Return m_szMicrStr
    End Function

    Public Sub SetAccountNumber(szAccountNumber As String)
        m_szAccountNumber = szAccountNumber
    End Sub

    Public Function GetAccountNumber() As String
        Return m_szAccountNumber
    End Function

    Public Sub SetAmount(szAmount As String)
        m_szAmount = szAmount
    End Sub

    Public Function GetAmount() As String
        Return m_szAmount
    End Function

    Public Sub SetBankNumber(szBankNumber As String)
        m_szBankNumber = szBankNumber
    End Sub

    Public Function GetBankNumber() As String
        Return m_szBankNumber
    End Function

    Public Sub SetSerialNumber(szSerialNumber As String)
        m_szSerialNumber = szSerialNumber
    End Sub

    Public Function GetSerialNumber() As String
        Return m_szSerialNumber
    End Function

    Public Sub SetEPC(szEPC As String)
        m_szEPC = szEPC
    End Sub

    Public Function GetEPC() As String
        Return m_szEPC
    End Function

    Public Sub SetTransitNumber(szTransitNumber As String)
        m_szTransitNumber = szTransitNumber
    End Sub

    Public Function GetTransitNumber() As String
        Return m_szTransitNumber
    End Function

    Public Sub SetCheckType(eCheckType As Check)
        m_eCheckType = eCheckType
    End Sub

    Public Function GetCheckType() As Check
        Return m_eCheckType
    End Function

    Public Sub SetCountryCode(eCountryCode As Country)
        m_eCountryCode = eCountryCode
    End Sub

    Public Function GetCountryCode() As Country
        Return m_eCountryCode
    End Function

    Public Sub SetOnUSField(szOnUSField As String)
        m_szOnUSField = szOnUSField
    End Sub

    Public Function GetOnUSField() As String
        Return m_szOnUSField
    End Function

    Public Sub SetAuxiliatyOnUSField(szAuxiliatyOnUSField As String)
        m_szAuxiliatyOnUSField = szAuxiliatyOnUSField
    End Sub

    Public Function GetAuxiliatyOnUSField() As String
        Return m_szAuxiliatyOnUSField
    End Function

End Class
