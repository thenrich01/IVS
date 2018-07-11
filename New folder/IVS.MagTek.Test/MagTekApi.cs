using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace IVS.MagTek.Test
{
    public class MagTekApi
    {
        /// <summary>
        ///   Clears the data buffer
        /// </summary>
        [DllImport("MTUSCRA.dll")]
        private static extern void MTUSCRAClearBuffer();

        /// <summary>
        ///   Opens the MTUSCRA Device
        /// </summary>
        /// <returns> A uint that is an <see cref="ErrorValuesEnum" /> </returns>
        [DllImport("MTUSCRA.dll")]
        private static extern uint MTUSCRACloseDevice();

        /// <summary>Gets the card data from the reader.</summary>
        /// <param name="cardData">The card data structure for holding the card information. </param>
        /// <returns>A uint that is an <see cref="ErrorValuesEnum"/> </returns>
        [DllImport("MTUSCRA.dll")]
        private static extern uint MTUSCRAGetCardData(ref MTMSRDATA cardData);

        //// This has been removed because it requires unsafe code.
        /////// <summary>Gets the card data delimited by the provided string.</summary>
        /////// <param name="data">The string result that will be sent back. </param>
        /////// <param name="delimiter">The delimiter for the string </param>
        /////// <returns>A uint that is an <see cref="ErrorValuesEnum"/> </returns>
        ////[DllImport("MTUSCRA.dll")]
        ////private static extern unsafe uint MTUSCRAGetCardDataStr(byte* data, string delimiter);

        /// <summary>Opens the MTUSCRA Device</summary>
        /// <param name="deviceName">The name of the device to open </param>
        /// <returns>A uint that is an <see cref="ErrorValuesEnum"/> </returns>
        [DllImport("MTUSCRA.dll")]
        private static extern uint MTUSCRAOpenDevice(string deviceName);

        /// <summary>Opens the MTUSCRA Device</summary>
        /// <param name="command">The command to send </param>
        /// <param name="commandLength">The length of the command sent. </param>
        /// <param name="result">The result of the command </param>
        /// <param name="resultLength">The length of the result. </param>
        /// <returns>A uint that is an <see cref="ErrorValuesEnum"/> </returns>
        [DllImport("MTUSCRA.dll")]
        private static extern uint MTUSCRASendCommand(string command, uint commandLength, ref string result, ref uint resultLength);

        /// <summary>
        /// Raised when the card state data changes.
        /// </summary>
        /// <param name="callBack">The call back for card data state changing.</param>
        [DllImport("MTUSCRA.dll")]
        private static extern void MTUSCRACardDataStateChangedNotify(CardDataStateChangedCallBack callBack);

        /// <summary>
        /// Raised when the device state changes
        /// </summary>
        /// <param name="callBack">The call back for card data state changing.</param>
        [DllImport("MTUSCRA.dll")]
        private static extern void MTUSCRADeviceStateChangedNotify(DeviceStateChangedCallBack callBack);

        /// <summary>
        /// Gets the current device state.
        /// </summary>
        /// <param name="deviceState">The device state</param>
        [DllImport("MTUSCRA.dll")]
        private static extern void MTUSCRAGetDeviceState(ref uint deviceState);

        /// <summary>
        /// Gets the current data state.
        /// </summary>
        /// <param name="dataState">The data state</param>
        [DllImport("MTUSCRA.dll")]
        private static extern void MTUSCRAGetCardDataState(ref uint dataState);

        /// <summary>
        /// Gets the cproduct id of the card reader.
        /// </summary>
        /// <param name="productId">The product id</param>
        [DllImport("MTUSCRA.dll")]
        private static extern void MTUSCRAGetPID(ref uint productId);

        /// <summary>
        ///   A delegate for handling the card data state callback.
        /// </summary>
        /// <param name="dataState"> The data state </param>
        private delegate void CardDataStateChangedCallBack(uint dataState);

        /// <summary>
        ///   A delegate for handling the device state callback.
        /// </summary>
        /// <param name="deviceState"> The device state </param>
        private delegate void DeviceStateChangedCallBack(uint deviceState);

        //this.CardDataStateChanged = new CardDataStateChangedCallBack(this.OnCardDataStateChanged);
        //this.DeviceStateChanged = new DeviceStateChangedCallBack(this.OnDeviceStateChanged);

        //MTUSCRACardDataStateChangedNotify(this.CardDataStateChanged);
        //MTUSCRADeviceStateChangedNotify(this.DeviceStateChanged);

        [StructLayout(LayoutKind.Sequential)]
        public struct MTMSRDATA
        {
            /// <summary>The default data size for tracks.</summary>
            private const int DEF_MSR_DATA_LEN = 256;

            /// <summary>The card data.</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DEF_MSR_DATA_LEN * 3)]
            public string m_szCardData;

            /// <summary>masked card data.</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DEF_MSR_DATA_LEN * 3)]
            public string m_szCardDataMasked;

            /// <summary>Track 1 Data.</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DEF_MSR_DATA_LEN)]
            public string m_szTrack1Data;

            /// <summary>Track 2 data.</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DEF_MSR_DATA_LEN)]
            public string m_szTrack2Data;

            /// <summary>Track 3 data.</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DEF_MSR_DATA_LEN)]
            public string m_szTrack3Data;

            /// <summary>Masked track 1 data.</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DEF_MSR_DATA_LEN)]
            public string m_szTrack1DataMasked;

            /// <summary>masked track 2 data.</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DEF_MSR_DATA_LEN)]
            public string m_szTrack2DataMasked;

            /// <summary>masked track 3 data.</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DEF_MSR_DATA_LEN)]
            public string m_szTrack3DataMasked;

            /// <summary>MagnePrint data.</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DEF_MSR_DATA_LEN)]
            public string m_szMagnePrintData;

            /// <summary>Card encode type.</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DEF_MSR_DATA_LEN)]
            public string m_szCardEncodeType;

            /// <summary>MagnePrint Status.</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DEF_MSR_DATA_LEN)]
            public string m_szMagnePrintStatus;

            /// <summary>DUKPT Session ID.</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DEF_MSR_DATA_LEN)]
            public string m_szDUKPTSessionID;

            /// <summary>Device Serial Number.</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DEF_MSR_DATA_LEN)]
            public string m_szDeviceSerialNumber;

            /// <summary>DUKPT Key Serial Number.</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DEF_MSR_DATA_LEN)]
            public string m_szDUKPTKSN;

            /// <summary>First Name from Track 1.</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DEF_MSR_DATA_LEN)]
            public string m_szFirstName;

            /// <summary>Last Name from Track 1.</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DEF_MSR_DATA_LEN)]
            public string m_szLastName;

            /// <summary>PAN from Track 2.</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DEF_MSR_DATA_LEN)]
            public string m_szPAN;

            /// <summary>The Expiration Month.</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DEF_MSR_DATA_LEN)]
            public string m_szMonth;

            /// <summary>The Expiration Year.</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DEF_MSR_DATA_LEN)]
            public string m_szYear;

            /// <summary>Reader product ID.</summary>
            public uint m_dwReaderID;

            /// <summary>MagnePrint length.</summary>
            public uint m_dwMagnePrintLength;

            /// <summary>MagnePrint Status.</summary>
            public uint m_dwMagnePrintStatus;

            /// <summary>Track 1 data length.</summary>
            public uint m_dwTrack1Length;

            /// <summary>Track 2 length.</summary>
            public uint m_dwTrack2Length;

            /// <summary>Track 3 length.</summary>
            public uint m_dwTrack3Length;

            /// <summary>Track 1 length masked.</summary>
            public uint m_dwTrack1LengthMasked;

            /// <summary>Track 2 length masked.</summary>
            public uint m_dwTrack2LengthMasked;

            /// <summary>Track 3 length masked.</summary>
            public uint m_dwTrack3LengthMasked;

            /// <summary>Card encode type.</summary>
            public uint m_dwCardEncodeType;

            /// <summary>Track 1 decode status.</summary>
            public uint m_dwTrack1DcdStatus;

            /// <summary>Track 2 decode status.</summary>
            public uint m_dwTrack2DcdStatus;

            /// <summary>The m_dw track 3 dcd status.</summary>
            public uint m_dwTrack3DcdStatus;

            /// <summary>Card swipe status.</summary>
            public uint m_dwCardSwipeStatus;
        }
    }

}
