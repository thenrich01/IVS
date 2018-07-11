Imports System.Text
Imports System.Drawing
Imports com.epson.bank.driver

Class CEndorse
    Private Const TRANS_NO_SPECIFIED As UInteger = &H40000000
    Private m_objProperty As CProperty_1 = Nothing
    Private bmLogoImage As Bitmap = Nothing

    Public Sub New()
        m_objProperty = New CProperty_1()
    End Sub

    Public Function Init(objProperty As CProperty_1) As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS

        m_objProperty = objProperty

        Return errResult
    End Function

    Public Function LoadEndorseFile() As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS

        ' Loading printing region setting file for template printing
        Dim nLoadNum As Integer = 0
        Dim sbPath As New StringBuilder()
        Dim szPath As String = Nothing

        sbPath.Append(System.IO.Directory.GetCurrentDirectory())
        sbPath.Append("\FormatSettingStep4.ini")
        szPath = sbPath.ToString()

        ' Set print information for physical endorsement
        errResult = m_objProperty.GetMfDevice().SetPrintStation(PrintingStation.MF_ST_PHYSICAL_ENDORSEMENT)
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        errResult = m_objProperty.GetMfDevice().LoadTemplatePrintArea(szPath, nLoadNum)
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        ' Set print information for electronic endorsement
        errResult = m_objProperty.GetMfDevice().SetPrintStation(PrintingStation.MF_ST_E_ENDORSEMENT)
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        errResult = m_objProperty.GetMfDevice().LoadTemplatePrintArea(szPath, nLoadNum)
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        Try
            ' Loading image file for printing
            bmLogoImage = New Bitmap("Logo.bmp")
        Catch
            Return ErrorCode.ERR_IMAGE_FILEREAD
        End Try

        Return errResult
    End Function

    Public Function SetEndorseStation() As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS

        ' Select print station
        errResult = m_objProperty.GetMfDevice().SetPrintStation(m_objProperty.GetEndorseType())

        Return errResult
    End Function

    Public Function PhysicalEndorseFunction(unTransactionNumber As UInteger) As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS

        ' Start buffering of print data for format printing
        errResult = m_objProperty.GetMfDevice().TemplatePrint(TemplatePrintMode.TEMPLATEPRINT_BUFFERING)
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        ' Set image data for printing
        If m_objProperty.GetPrintImage() Then
            errResult = m_objProperty.GetMfDevice().SetTemplatePrintArea(AreaSelectMode.SELECTPRINTAREA_AREANAME, "PhysicalEndorseImage", Nothing, unTransactionNumber)
            If errResult <> ErrorCode.SUCCESS Then
                Return errResult
            End If

            errResult = m_objProperty.GetMfDevice().SCNPrintMemoryImage(unTransactionNumber, bmLogoImage)
            If errResult <> ErrorCode.SUCCESS Then
                Return errResult
            End If
        End If

        ' set strings for printing
        If m_objProperty.GetPrintText() Then
            Dim stDateTime As DateTime
            Dim mfDecorate As New MFTrueType()
            Dim objFont As System.Drawing.Font = Nothing
            Dim szPhysicalEndorse As String = Nothing
            Dim sbPhysicalEndorse As New StringBuilder()
            Dim stMonth As String() = New String() {"", "Jan", "Feb", "Mar", "Apr", "May", _
             "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", _
             "Dec"}

            'PhysicalEndorseText1

            errResult = m_objProperty.GetMfDevice().SetTemplatePrintArea(AreaSelectMode.SELECTPRINTAREA_AREANAME, "PhysicalEndorseText1", Nothing, unTransactionNumber)
            If errResult <> ErrorCode.SUCCESS Then
                Return errResult
            End If

            objFont = New System.Drawing.Font("Arial", 9)
            mfDecorate.Font = objFont

            stDateTime = DateTime.Now
            sbPhysicalEndorse.Append("  " & stMonth(stDateTime.Month) & " " & stDateTime.Day.ToString("00") & " " & stDateTime.Year.ToString("0000") & vbCr & vbLf)
            sbPhysicalEndorse.Append("  " & stDateTime.Hour.ToString("00") & ":" & stDateTime.Minute.ToString("00") & ":" & stDateTime.Second.ToString("00") & ":" & stDateTime.Millisecond.ToString() & vbCr & vbLf & vbCr & vbLf)
            sbPhysicalEndorse.Append("  &>1234567890&<")
            szPhysicalEndorse = sbPhysicalEndorse.ToString()

            errResult = m_objProperty.GetMfDevice().SCNPrintText(unTransactionNumber, szPhysicalEndorse, mfDecorate)
            If errResult <> ErrorCode.SUCCESS Then
                Return errResult
            End If

            'PhysicalEndorseText2

            errResult = m_objProperty.GetMfDevice().SetTemplatePrintArea(AreaSelectMode.SELECTPRINTAREA_AREANAME, "PhysicalEndorseText2", Nothing, unTransactionNumber)
            If errResult <> ErrorCode.SUCCESS Then
                Return errResult
            End If

            szPhysicalEndorse = "  &>1234567890&<"

            errResult = m_objProperty.GetMfDevice().SCNPrintText(unTransactionNumber, szPhysicalEndorse, mfDecorate)
            If errResult <> ErrorCode.SUCCESS Then
                Return errResult
            End If
        End If

        ' Print buffered data
        errResult = m_objProperty.GetMfDevice().TemplatePrint(TemplatePrintMode.TEMPLATEPRINT_EXEC)
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        Return errResult
    End Function

    Public Function ElectronicEndorseFunction() As ErrorCode
        Dim errResult As ErrorCode = ErrorCode.SUCCESS

        ' Start buffering of print data for format printing
        errResult = m_objProperty.GetMfDevice().TemplatePrint(TemplatePrintMode.TEMPLATEPRINT_BUFFERING)
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        ' Set image data for printing
        If m_objProperty.GetPrintImage() Then
            errResult = m_objProperty.GetMfDevice().SetTemplatePrintArea(AreaSelectMode.SELECTPRINTAREA_AREANAME, "ElectronicEndorseImage", Nothing, TRANS_NO_SPECIFIED)
            If errResult <> ErrorCode.SUCCESS Then
                Return errResult
            End If

            errResult = m_objProperty.GetMfDevice().PrintImage("Logo.bmp")
            If errResult <> ErrorCode.SUCCESS Then
                Return errResult
            End If
        End If

        ' set strings for printing
        If m_objProperty.GetPrintText() Then
            Dim stDateTime As DateTime
            Dim mfDecorate As New MFTrueType()
            Dim objFont As System.Drawing.Font = Nothing
            Dim szElectronicEndorse As String = Nothing
            Dim sbElectronicEndorse As New StringBuilder()
            Dim stMonth As String() = New String() {"", "Jan", "Feb", "Mar", "Apr", "May", _
             "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", _
             "Dec"}

            'ElectronicEndorseText1

            errResult = m_objProperty.GetMfDevice().SetTemplatePrintArea(AreaSelectMode.SELECTPRINTAREA_AREANAME, "ElectronicEndorseText1", Nothing, TRANS_NO_SPECIFIED)
            If errResult <> ErrorCode.SUCCESS Then
                Return errResult
            End If

            objFont = New System.Drawing.Font("Arial", 9)
            mfDecorate.Font = objFont

            stDateTime = DateTime.Now
            sbElectronicEndorse.Append("  " & stMonth(stDateTime.Month) & " " & stDateTime.Day.ToString("00") & " " & stDateTime.Year.ToString("0000") & vbCr & vbLf)
            sbElectronicEndorse.Append("  " & stDateTime.Hour.ToString("00") & ":" & stDateTime.Minute.ToString("00") & ":" & stDateTime.Second.ToString("00") & ":" & stDateTime.Millisecond.ToString() & vbCr & vbLf & vbCr & vbLf)
            sbElectronicEndorse.Append("  &>1234567890&<")
            szElectronicEndorse = sbElectronicEndorse.ToString()

            errResult = m_objProperty.GetMfDevice().PrintText(szElectronicEndorse, mfDecorate)
            If errResult <> ErrorCode.SUCCESS Then
                Return errResult
            End If

            'ElectronicEndorseText2

            errResult = m_objProperty.GetMfDevice().SetTemplatePrintArea(AreaSelectMode.SELECTPRINTAREA_AREANAME, "ElectronicEndorseText2", Nothing, TRANS_NO_SPECIFIED)
            If errResult <> ErrorCode.SUCCESS Then
                Return errResult
            End If

            szElectronicEndorse = "  &>1234567890&<"

            errResult = m_objProperty.GetMfDevice().PrintText(szElectronicEndorse, mfDecorate)
            If errResult <> ErrorCode.SUCCESS Then
                Return errResult
            End If
        End If

        ' Print buffered data
        errResult = m_objProperty.GetMfDevice().TemplatePrint(TemplatePrintMode.TEMPLATEPRINT_EXEC)
        If errResult <> ErrorCode.SUCCESS Then
            Return errResult
        End If

        Return errResult
    End Function
End Class
