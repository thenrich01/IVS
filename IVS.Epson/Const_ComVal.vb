Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Public NotInheritable Class ConstComVal
    Private Sub New()
    End Sub
    ' DialogMessage

    Public Enum DialogResult
        OK = 0
        Cancel = 1
    End Enum


    ' Main

    ' Step1
    Public Const VAL_MAINDLG_ERR_PAPER_JAM As Integer = 4

    Public Const ERR_LOAD_MODULE As Integer = -1

    ' Config

    ' Step2
    Public Const VAL_CONFIGDLG_RADIO_SM_CHECKPAPER As Integer = 0
    Public Const VAL_CONFIGDLG_RADIO_SM_CARD As Integer = 1
    Public Const VAL_CONFIGDLG_RADIO_IC_RGB As Integer = 0
    Public Const VAL_CONFIGDLG_RADIO_IC_IR As Integer = 1
    Public Const VAL_CONFIGDLG_RADIO_IC_RGBIR As Integer = 2
    Public Const VAL_CONFIGDLG_RADIO_RGBCD_COLOR As Integer = 0
    Public Const VAL_CONFIGDLG_RADIO_RGBCD_GRAY As Integer = 1
    Public Const VAL_CONFIGDLG_RADIO_RGBCD_BW As Integer = 2
    Public Const VAL_CONFIGDLG_RADIO_IRCD_GRAY As Integer = 0
    Public Const VAL_CONFIGDLG_RADIO_IRCD_BW As Integer = 1
    Public Const VAL_CONFIGDLG_CHECK_SC_SAVEFILE_OFF As Boolean = False
    Public Const VAL_CONFIGDLG_CHECK_SC_SAVEFILE_ON As Boolean = True
    Public Const VAL_CONFIGDLG_COMBO_RE_600600 As Integer = 0
    Public Const VAL_CONFIGDLG_COMBO_RE_300300 As Integer = 1
    Public Const VAL_CONFIGDLG_COMBO_RE_240240 As Integer = 2
    Public Const VAL_CONFIGDLG_COMBO_RE_200200 As Integer = 3
    Public Const VAL_CONFIGDLG_COMBO_RE_120120 As Integer = 4
    Public Const VAL_CONFIGDLG_COMBO_RE_100100 As Integer = 5

    ' Step3
    Public Const VAL_CONFIGDLG_RADIO_MF_E13B As Integer = 0
    Public Const VAL_CONFIGDLG_RADIO_MF_CMC7 As Integer = 1
    Public Const VAL_CONFIGDLG_CHECK_MI_CLEARSPACE_OFF As Boolean = False
    Public Const VAL_CONFIGDLG_CHECK_MI_CLEARSPACE_ON As Boolean = True
    Public Const VAL_CONFIGDLG_CHECK_MI_SAVEFILE_OFF As Boolean = False
    Public Const VAL_CONFIGDLG_CHECK_MI_SAVEFILE_ON As Boolean = True

    'Step4
    Public Const VAL_CONFIGDLG_RADIO_PT_PHYSICAL As Integer = 0
    Public Const VAL_CONFIGDLG_RADIO_PT_ELECTRONIC As Integer = 1
    Public Const VAL_CONFIGDLG_RADIO_PM_HIGHSPEEDMODE As Integer = 0
    Public Const VAL_CONFIGDLG_RADIO_PM_DATAWAITINGMODE As Integer = 1
    Public Const VAL_CONFIGDLG_CHECK_EN_PRINTTEXT_OFF As Boolean = False
    Public Const VAL_CONFIGDLG_CHECK_EN_PRINTTEXT_ON As Boolean = True
    Public Const VAL_CONFIGDLG_CHECK_EN_PRINTIMAGE_OFF As Boolean = False
    Public Const VAL_CONFIGDLG_CHECK_EN_PRINTIMAGE_ON As Boolean = True
    Public Const VAL_CONFIGDLG_CHECK_EN_PRNDATAUNRECEIVECANCEL_OFF As Boolean = False
    Public Const VAL_CONFIGDLG_CHECK_EN_PRNDATAUNRECEIVECANCEL_ON As Boolean = True

End Class
