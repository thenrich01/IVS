Imports com.epson.bank.driver

Public Class CEndorseParam
    Private m_eEndorseType As PrintingStation
    Private m_bPrintText As Boolean
    Private m_bPrintImage As Boolean

    Public Sub New()
        m_eEndorseType = PrintingStation.MF_ST_PHYSICAL_ENDORSEMENT
        m_bPrintText = False
        m_bPrintImage = False
    End Sub

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
End Class