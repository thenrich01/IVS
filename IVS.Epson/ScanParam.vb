Imports com.epson.bank.driver

Public Class CScanParam
    Private m_eImageChannel As ImageTypeOption
    Private m_eRGBColorDepth As ColorDepth
    Private m_eIRColorDepth As ColorDepth
    Private m_eResolution As MfScanDpi
    Private m_eScanMedia As ScanUnit

    Public Sub New()
        m_eImageChannel = 0
        m_eRGBColorDepth = 0
        m_eIRColorDepth = 0
        m_eResolution = 0
        m_eScanMedia = 0
    End Sub

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

    Public Sub SetScanMedia(eScanMedia As ScanUnit)
        m_eScanMedia = eScanMedia
    End Sub

    Public Function GetScanMedia() As ScanUnit
        Return m_eScanMedia
    End Function

End Class
