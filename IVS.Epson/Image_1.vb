Imports com.epson.bank.driver

Class CImage_1
    Private m_objProperty As CProperty_1 = Nothing

    Public Sub New()
        m_objProperty = New CProperty_1()
    End Sub

    Public Function Init(objProperty As CProperty_1) As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS

        m_objProperty = objProperty

        Return errResult
    End Function

    ' Specify image data format
    Public Function SetImageSetting(eFace As ScanSide, eGradation As ImageType, eFormat As Format) As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS
        Dim eColorDepth As ColorDepth = ColorDepth.EPS_BI_SCN_24BIT
        Dim eColor As Color = Color.EPS_BI_SCN_COLOR
        Dim eExOption As ExOption = ExOption.EPS_BI_SCN_MANUAL

        errResult = m_objProperty.GetMfDevice().SCNSelectScanFace(eFace)
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        errResult = m_objProperty.GetMfDevice().SCNSelectScanImage(eGradation)
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        If eGradation = ImageType.MF_SCAN_IMAGE_VISIBLE Then
            eColorDepth = m_objProperty.GetRGBColorDepth()
        Else
            eColorDepth = m_objProperty.GetIRColorDepth()
        End If

        If eColorDepth = ColorDepth.EPS_BI_SCN_24BIT Then
            eColor = Color.EPS_BI_SCN_COLOR
            eExOption = ExOption.EPS_BI_SCN_MANUAL
        Else
            eColor = Color.EPS_BI_SCN_MONOCHROME
            eExOption = ExOption.EPS_BI_SCN_SHARP
        End If

        errResult = m_objProperty.GetMfDevice().SCNSetImageQuality(eColorDepth, 0, eColor, eExOption)
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        errResult = m_objProperty.GetMfDevice().SCNSetImageFormat(eFormat)

        Return errResult
    End Function

    ' Get check image
    Public Function GetScanImage(ByRef objImageResult As CImageResult) As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS
        Dim mfScan As MFScan = Nothing

        errResult = SetImageSetting(objImageResult.GetFace(), objImageResult.GetGradation(), objImageResult.GetFormat())
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        If objImageResult.GetFace() = ScanSide.MF_SCAN_FACE_FRONT Then
            mfScan = m_objProperty.GetMfScanFront()
        Else
            mfScan = m_objProperty.GetMfScanBack()
        End If

        errResult = m_objProperty.GetMfDevice().GetScanImage(m_objProperty.GetTransactionNumber(), mfScan)
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        objImageResult.SetTransactionNumber(m_objProperty.GetTransactionNumber())
        objImageResult.SetImageSize(CInt(mfScan.Data.Length))
        objImageResult.SetBitmapImage(mfScan.Image)
        objImageResult.SetImageData(mfScan.Data)

        Return errResult
    End Function

    ' Get cashier's check image
    Public Function GetCashersCheckImage(ByRef objImageResult As CImageResult) As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS
        Dim mfScan As MFScan = Nothing

        errResult = m_objProperty.GetMfDevice().SCNSelectScanFace(objImageResult.GetFace())
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        errResult = m_objProperty.GetMfDevice().SCNSelectScanImage(ImageType.MF_SCAN_IMAGE_VISIBLE)
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        errResult = m_objProperty.GetMfDevice().SCNSetImageFormat(Format.EPS_BI_SCN_BITMAP)
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        errResult = m_objProperty.GetMfDevice().SCNSetImageQuality(ColorDepth.EPS_BI_SCN_24BIT, 0, Color.EPS_BI_SCN_COLOR, ExOption.EPS_BI_SCN_MANUAL)
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        If objImageResult.GetFace() = ScanSide.MF_SCAN_FACE_FRONT Then
            mfScan = m_objProperty.GetMfScanFront()
        Else
            mfScan = m_objProperty.GetMfScanBack()
        End If

        errResult = m_objProperty.GetMfDevice().GetScanImage(m_objProperty.GetTransactionNumber(), mfScan)
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        objImageResult.SetTransactionNumber(m_objProperty.GetTransactionNumber())
        objImageResult.SetImageSize(CInt(mfScan.Data.Length))
        objImageResult.SetBitmapImage(mfScan.Image)
        objImageResult.SetImageData(mfScan.Data)

        Return errResult
    End Function
End Class
