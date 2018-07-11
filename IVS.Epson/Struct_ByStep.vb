
Public Class StructByStep
    ' Step 2
    Public nRadio_ScanningMedia As Integer
    Public nRadio_ImageChannel As Integer
    Public nRadio_RGBColorDepth As Integer
    Public nRadio_IRColorDepth As Integer
    Public bCheck_Scan_SaveFile As Boolean
    Public nCombo_Resolution As Integer

    ' Step 3
    Public nRadio_MICRFont As Integer
    Public bCheck_MICR_ClearSpace As Boolean
    Public bCheck_MICR_SaveFile As Boolean

    ' Step 4
    Public nRadio_PrintType As Integer
    Public nRadio_PrintMode As Integer
    Public bCheck_Endorsement_PrintText As Boolean
    Public bCheck_Endorsement_PrintImage As Boolean
    Public bCheck_Endorsement_PrnDataUnreceiveCancel As Boolean

    Public Sub New()
        ' Step 2
        nRadio_ScanningMedia = ConstComVal.VAL_CONFIGDLG_RADIO_SM_CHECKPAPER
        nRadio_ImageChannel = ConstComVal.VAL_CONFIGDLG_RADIO_IC_RGB
        nRadio_RGBColorDepth = ConstComVal.VAL_CONFIGDLG_RADIO_RGBCD_COLOR
        nRadio_IRColorDepth = ConstComVal.VAL_CONFIGDLG_RADIO_IRCD_GRAY
        bCheck_Scan_SaveFile = ConstComVal.VAL_CONFIGDLG_CHECK_SC_SAVEFILE_ON
        nCombo_Resolution = ConstComVal.VAL_CONFIGDLG_COMBO_RE_200200

        ' Step 3
        nRadio_MICRFont = ConstComVal.VAL_CONFIGDLG_RADIO_MF_E13B
        bCheck_MICR_ClearSpace = ConstComVal.VAL_CONFIGDLG_CHECK_MI_CLEARSPACE_OFF
        bCheck_MICR_SaveFile = ConstComVal.VAL_CONFIGDLG_CHECK_MI_SAVEFILE_OFF

        ' Step 4
        nRadio_PrintType = ConstComVal.VAL_CONFIGDLG_RADIO_PT_PHYSICAL
        nRadio_PrintMode = ConstComVal.VAL_CONFIGDLG_RADIO_PM_HIGHSPEEDMODE
        bCheck_Endorsement_PrintText = ConstComVal.VAL_CONFIGDLG_CHECK_EN_PRINTTEXT_OFF
        bCheck_Endorsement_PrintImage = ConstComVal.VAL_CONFIGDLG_CHECK_EN_PRINTIMAGE_OFF
        bCheck_Endorsement_PrnDataUnreceiveCancel = ConstComVal.VAL_CONFIGDLG_CHECK_EN_PRNDATAUNRECEIVECANCEL_ON
    End Sub

    Public Sub Copy(sData As StructByStep)
        ' Step 2
        nRadio_ScanningMedia = sData.nRadio_ScanningMedia
        nRadio_ImageChannel = sData.nRadio_ImageChannel
        nRadio_RGBColorDepth = sData.nRadio_RGBColorDepth
        nRadio_IRColorDepth = sData.nRadio_IRColorDepth
        bCheck_Scan_SaveFile = sData.bCheck_Scan_SaveFile
        nCombo_Resolution = sData.nCombo_Resolution

        ' Step 3
        nRadio_MICRFont = sData.nRadio_MICRFont
        bCheck_MICR_ClearSpace = sData.bCheck_MICR_ClearSpace
        bCheck_MICR_SaveFile = sData.bCheck_MICR_SaveFile

        ' Step 4
        nRadio_PrintType = sData.nRadio_PrintType
        nRadio_PrintMode = sData.nRadio_PrintMode
        bCheck_Endorsement_PrintText = sData.bCheck_Endorsement_PrintText
        bCheck_Endorsement_PrintImage = sData.bCheck_Endorsement_PrintImage
        bCheck_Endorsement_PrnDataUnreceiveCancel = sData.bCheck_Endorsement_PrnDataUnreceiveCancel
    End Sub
End Class
