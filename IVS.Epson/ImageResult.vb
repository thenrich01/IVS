Imports System.Drawing
Imports System.IO
Imports com.epson.bank.driver

Public Class CImageResult
    Private m_nTransactionNumber As Integer
    Private m_cImage As Bitmap
    Private m_cData As Stream
    Private m_nImageSize As Integer
    Private m_eGradation As ImageType
    Private m_eFace As ScanSide
    Private m_eFormat As Format

    Public Sub New()
        m_nTransactionNumber = 0
        m_cData = Nothing
        m_cImage = Nothing
        m_nImageSize = 0
        m_eGradation = 0
        m_eFace = 0
        m_eFormat = 0
    End Sub

    Public Sub SetTransactionNumber(nTransactionNumber As Integer)
        m_nTransactionNumber = nTransactionNumber
    End Sub

    Public Function GetTransactionNumber() As Integer
        Return m_nTransactionNumber
    End Function

    Public Sub SetBitmapImage(cImage As Bitmap)
        m_cImage = cImage
    End Sub

    Public Function GetBitmapImage() As Bitmap
        Return m_cImage
    End Function

    Public Sub SetImageData(cData As Stream)
        m_cData = cData
    End Sub

    Public Function GetImageData() As Stream
        Return m_cData
    End Function

    Public Sub SetImageSize(nImageSize As Integer)
        m_nImageSize = nImageSize
    End Sub

    Public Function GetImageSize() As Integer
        Return m_nImageSize
    End Function

    Public Sub SetGradation(eGradetion As ImageType)
        m_eGradation = eGradetion
    End Sub

    Public Function GetGradation() As ImageType
        Return m_eGradation
    End Function

    Public Sub SetFace(eFace As ScanSide)
        m_eFace = eFace
    End Sub

    Public Function GetFace() As ScanSide
        Return m_eFace
    End Function

    Public Sub SetFormat(eFormat As Format)
        m_eFormat = eFormat
    End Sub

    Public Function GetFormat() As Format
        Return m_eFormat
    End Function
    Public Sub FreeImage()
        If m_cData IsNot Nothing AndAlso m_cImage IsNot Nothing Then
            m_cData.Dispose()
            m_cImage.Dispose()
            m_nImageSize = 0
            m_nTransactionNumber = 0
        End If
    End Sub
End Class
