Imports System.Runtime.InteropServices
'http://geekswithblogs.net/rakker/archive/2012/07/11/reading-credit-card-data-from-a-magtek-card-swipe-device.aspx

Public Class MagTekApi
    Public Const MTSCRA_ST_OK = 0
    Public Const MTSCRA_ST_FAILED = 1
    Public Const MTSCRA_ST_OPEN = 2
    Public Const MTSCRA_ST_INVALID_PARAM = 3

    Public Const MTSCRA_STATE_DISCONNECTED = 0
    Public Const MTSCRA_STATE_CONNECTED = 1
    Public Const MTSCRA_STATE_ERROR = 2

    Public Const MTSCRA_CARDREAD_OK = 0
    Public Const MTSCRA_CARDREAD_ERROR = 1

    Public Const MTSCRA_DATA_NOTREADY = 0
    Public Const MTSCRA_DATA_READY = 1
    Public Const MTSCRA_DATA_ERROR = 2

    Declare Function MTUSCRAOpenDevice Lib "MTUSCRA.dll" (ByVal DeviceName As String) As UInteger
    Declare Function MTUSCRACloseDevice Lib "MTUSCRA.dll" () As UInteger
    Declare Function MTUSCRASendCommand Lib "MTUSCRA.dll" (command As String, commandLength As UInteger, ByRef result As String, ByRef resultLength As UInteger) As UInteger
    Declare Function MTUSCRAGetCardData Lib "MTUSCRA.dll" (ByRef cardData As MTMSRDATA) As UInteger
    Declare Function MTUSCRAGetCardDataStr Lib "MTUSCRA.dll" (ByVal Data As String, ByVal Delimiter As String) As UInteger

    Declare Sub MTUSCRAClearBuffer Lib "MTUSCRA.dll" ()
    Declare Sub MTUSCRAGetDeviceState Lib "MTUSCRA.dll" (ByRef State As UInteger)
    Declare Sub MTUSCRAGetCardDataState Lib "MTUSCRA.dll" (ByRef State As UInteger)
    Declare Sub MTUSCRAGetPID Lib "MTUSCRA.dll" (ByRef ID As UInteger)
    Declare Sub MTUSCRADeviceStateChangedNotify Lib "MTUSCRA.dll" (ByVal lpFunc As CallBackDeviceStateChanged)
    Declare Sub MTUSCRACardDataStateChangedNotify Lib "MTUSCRA.dll" (ByVal lpFunc As CallBackCardDataStateChanged)

    Public Delegate Sub CallBackCardDataStateChanged(ByVal Parm As UInteger)
    Public Delegate Sub CallBackDeviceStateChanged(ByVal Parm As UInteger)

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure MTMSRDATA
        ''' <summary>The default data size for tracks.</summary>
        Private Const DEF_MSR_DATA_LEN As Integer = 256

        ''' <summary>The card data.</summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=DEF_MSR_DATA_LEN * 3)> _
        Public m_szCardData As String

        ''' <summary>masked card data.</summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=DEF_MSR_DATA_LEN * 3)> _
        Public m_szCardDataMasked As String

        ''' <summary>Track 1 Data.</summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=DEF_MSR_DATA_LEN)> _
        Public m_szTrack1Data As String

        ''' <summary>Track 2 data.</summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=DEF_MSR_DATA_LEN)> _
        Public m_szTrack2Data As String

        ''' <summary>Track 3 data.</summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=DEF_MSR_DATA_LEN)> _
        Public m_szTrack3Data As String

        ''' <summary>Masked track 1 data.</summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=DEF_MSR_DATA_LEN)> _
        Public m_szTrack1DataMasked As String

        ''' <summary>masked track 2 data.</summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=DEF_MSR_DATA_LEN)> _
        Public m_szTrack2DataMasked As String

        ''' <summary>masked track 3 data.</summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=DEF_MSR_DATA_LEN)> _
        Public m_szTrack3DataMasked As String

        ''' <summary>MagnePrint data.</summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=DEF_MSR_DATA_LEN)> _
        Public m_szMagnePrintData As String

        ''' <summary>Card encode type.</summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=DEF_MSR_DATA_LEN)> _
        Public m_szCardEncodeType As String

        ''' <summary>MagnePrint Status.</summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=DEF_MSR_DATA_LEN)> _
        Public m_szMagnePrintStatus As String

        ''' <summary>DUKPT Session ID.</summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=DEF_MSR_DATA_LEN)> _
        Public m_szDUKPTSessionID As String

        ''' <summary>Device Serial Number.</summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=DEF_MSR_DATA_LEN)> _
        Public m_szDeviceSerialNumber As String

        ''' <summary>DUKPT Key Serial Number.</summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=DEF_MSR_DATA_LEN)> _
        Public m_szDUKPTKSN As String

        ''' <summary>First Name from Track 1.</summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=DEF_MSR_DATA_LEN)> _
        Public m_szFirstName As String

        ''' <summary>Last Name from Track 1.</summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=DEF_MSR_DATA_LEN)> _
        Public m_szLastName As String

        ''' <summary>PAN from Track 2.</summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=DEF_MSR_DATA_LEN)> _
        Public m_szPAN As String

        ''' <summary>The Expiration Month.</summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=DEF_MSR_DATA_LEN)> _
        Public m_szMonth As String

        ''' <summary>The Expiration Year.</summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=DEF_MSR_DATA_LEN)> _
        Public m_szYear As String

        ''' <summary>Reader product ID.</summary>
        Public m_dwReaderID As UInteger

        ''' <summary>MagnePrint length.</summary>
        Public m_dwMagnePrintLength As UInteger

        ''' <summary>MagnePrint Status.</summary>
        Public m_dwMagnePrintStatus As UInteger

        ''' <summary>Track 1 data length.</summary>
        Public m_dwTrack1Length As UInteger

        ''' <summary>Track 2 length.</summary>
        Public m_dwTrack2Length As UInteger

        ''' <summary>Track 3 length.</summary>
        Public m_dwTrack3Length As UInteger

        ''' <summary>Track 1 length masked.</summary>
        Public m_dwTrack1LengthMasked As UInteger

        ''' <summary>Track 2 length masked.</summary>
        Public m_dwTrack2LengthMasked As UInteger

        ''' <summary>Track 3 length masked.</summary>
        Public m_dwTrack3LengthMasked As UInteger

        ''' <summary>Card encode type.</summary>
        Public m_dwCardEncodeType As UInteger

        ''' <summary>Track 1 decode status.</summary>
        Public m_dwTrack1DcdStatus As UInteger

        ''' <summary>Track 2 decode status.</summary>
        Public m_dwTrack2DcdStatus As UInteger

        ''' <summary>The m_dw track 3 dcd status.</summary>
        Public m_dwTrack3DcdStatus As UInteger

        ''' <summary>Card swipe status.</summary>
        Public m_dwCardSwipeStatus As UInteger
    End Structure

End Class