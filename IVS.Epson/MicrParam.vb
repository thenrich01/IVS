Imports com.epson.bank.driver

Public Class CMicrParam
    Private m_eFont As MfMicrFont
    Private m_bParsing As Boolean
    Private m_bClearSpace As Boolean

    Public Sub New()
        m_eFont = 0
        m_bParsing = False
        m_bClearSpace = False
    End Sub

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

End Class
