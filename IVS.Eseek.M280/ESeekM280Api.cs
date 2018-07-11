using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using CyUSB;
using System.Windows.Media.Imaging;
using System.Windows;

namespace IVS.Eseek.M280
{
    public class ESeekM280Api
    {

        USBDeviceList usbDevices;
        CyUSBDevice MyDevice;
        CyBulkEndPoint inEndpoint = null;
        CyControlEndPoint ep0 = null;

        int BufNum = 0;
        int BufSz;
        int QueueSz;
        int Successes;
        int Failures;
        int TotalPixSize;
        double XferBytes;
        bool bRunning;
        bool bInitM280;
        bool bSysBusy;
        byte[][] DataBuf;
        Bitmap CroppedImage;

        Thread tStartCap;

        delegate void ExceptionCallback();
        ExceptionCallback handleException;

        delegate void Image_View();
        Image_View Img_View;

        public event OnImageReceivedEventHandler OnImageReceived;
        public delegate void OnImageReceivedEventHandler(Bitmap ImageReceived);

        public ESeekM280Api()

    {

        usbDevices = new USBDeviceList(CyConst.DEVICES_CYUSB);

        handleException = new ExceptionCallback(ThreadException);

        Img_View = new Image_View(ImageView);

        SetDevice();

        if (M280DEF.PixFormat == PixelFormat.Format16bppRgb565)
        {
            TotalPixSize = M280DEF.Image_Xsize * M280DEF.Image_Ysize * 2;
        }
        else if (M280DEF.PixFormat == PixelFormat.Format24bppRgb)
        {
            TotalPixSize = M280DEF.Image_Xsize * M280DEF.Image_Ysize * 3;
        }

        this.BufNum = TotalPixSize / (512 * M280DEF.Packet_Xfer);
        
        this.bInitM280 = false;
        this.bSysBusy = false;
            
      }

         void SetDevice()
        {
            MyDevice = usbDevices[M280DEF.USB_VID, M280DEF.USB_PID] as CyUSBDevice;

            if (MyDevice != null)
            {
                if (inEndpoint == null)
                {
     
                    inEndpoint = MyDevice.EndPointOf(0x86) as CyBulkEndPoint;
                    ep0 = MyDevice.ControlEndPt;
                    inEndpoint.TimeOut = 500;

                    SendCommand(M280DEF.CMD_INIT_CAMERA, 0, 0);

                }
            }
            else
            {
                inEndpoint = null;
                ep0 = null;
                this.bInitM280 = false;
                this.bSysBusy = false;
            }
        }
     
        public unsafe void XferThread()
        {

            byte[][] cmdBufs = new byte[QueueSz][];
            byte[][] xferBufs = new byte[QueueSz][];
            byte[][] ovLaps = new byte[QueueSz][];
            ISO_PKT_INFO[][] pktsInfo = new ISO_PKT_INFO[QueueSz][];
            int xStart = 0;

            try
            {
                LockNLoad(ref xStart, cmdBufs, xferBufs, ovLaps, pktsInfo);
            }
            catch (NullReferenceException e)
            {
                // This exception gets thrown if the device is unplugged 
                // while we're streaming data
                e.GetBaseException();
                //this.Invoke(handleException);
            }
        }

        public unsafe void LockNLoad(ref int j, byte[][] cBufs, byte[][] xBufs, byte[][] oLaps, ISO_PKT_INFO[][] pktsInfo)
        {

            cBufs[j] = new byte[CyConst.SINGLE_XFER_LEN];
            xBufs[j] = new byte[BufSz];
            oLaps[j] = new byte[20];
            pktsInfo[j] = new ISO_PKT_INFO[M280DEF.Packet_Xfer];

            fixed (byte* tL0 = oLaps[j], tc0 = cBufs[j], tb0 = xBufs[j])
            {
                OVERLAPPED* ovLapStatus = (OVERLAPPED*)tL0;
                ovLapStatus->hEvent = (IntPtr)PInvoke.CreateEvent(0, 0, 0, 0);

                int len = BufSz;
                inEndpoint.BeginDataXfer(ref cBufs[j], ref xBufs[j], ref len, ref oLaps[j]);

                j++;

                if (j < QueueSz)
                    LockNLoad(ref j, cBufs, xBufs, oLaps, pktsInfo); 
                else
                {
                    XferData(cBufs, xBufs, oLaps, pktsInfo);         
                }
            }
        }

        public unsafe void XferData(byte[][] cBufs, byte[][] xBufs, byte[][] oLaps, ISO_PKT_INFO[][] pktsInfo)
        {

            int k = 0;
            int len = 0;
            int pDataBF = 0;

            Successes = 0;
            Failures = 0;
            XferBytes = 0;

            for (; bRunning; )
            {
                fixed (byte* tmpOvlap = oLaps[k])
                {
                    OVERLAPPED* ovLapStatus = (OVERLAPPED*)tmpOvlap;
                    if (!inEndpoint.WaitForXfer(ovLapStatus->hEvent, 500))
                    {
                        inEndpoint.Abort();
                        PInvoke.WaitForSingleObject(ovLapStatus->hEvent, 500);
                    }
                }
          
                if (inEndpoint.FinishDataXfer(ref cBufs[k], ref xBufs[k], ref len, ref oLaps[k]))
                {
                    XferBytes += len;
                    Successes++;
                    Array.Copy(xBufs[k], 0, DataBuf[pDataBF], 0, len);
                    pDataBF++;
                }
                else
                {
                    Failures++;
                }
                k++;
                if (k == QueueSz) 
                {
                    k = 0;
                    Thread.Sleep(1);
                    if (Failures == 0)
                    {
                        Img_View();
                    }
                    else
                    {
                        // Scaner busy
                    }
                    bRunning = false;
                }
            }
        }
   
        public void SendCommand(byte Cmd, ushort val, ushort index)
        {
            if (MyDevice == null || ep0 == null) return;
            
            ep0.Target = CyConst.TGT_DEVICE;
            ep0.ReqType = CyConst.REQ_VENDOR;
            ep0.Direction = CyConst.DIR_TO_DEVICE;
            ep0.ReqCode = Cmd;
            ep0.Value = val;
            ep0.Index = index;
            int len = 0;
            byte[] buf = new byte[1];
            ep0.XferData(ref buf, ref len);
        }
     
        public void ReadCommand(byte Cmd, ref byte[] Read, ref int Len, ushort val, ushort index)
        {

            if (MyDevice == null || ep0 == null) return;
            ep0.Target = CyConst.TGT_DEVICE;
            ep0.ReqType = CyConst.REQ_VENDOR;
            ep0.Value = val;
            ep0.Index = index;
            ep0.ReqCode = Cmd;
            ep0.Read(ref Read, ref Len);
         
        }

        public void ThreadException()
        {                     
            bRunning = false;
            tStartCap = null;
        }

        public void ImageView()
        {
            Bitmap SourceImage = CreateBitmap(M280DEF.Image_Xsize, M280DEF.Image_Ysize, M280DEF.PixFormat);
            Rectangle CropRect = new Rectangle(M280DEF.CropUpleftX, M280DEF.CropUpLeftY, M280DEF.CropSizeX, M280DEF.CropSizeY);
            this.CroppedImage = SourceImage.Clone(CropRect, M280DEF.PixFormat);
            SourceImage.Dispose();

            if (OnImageReceived != null)
            {
                OnImageReceived(this.CroppedImage);
            }

        }

        public Bitmap CreateBitmap(int Width, int Height, PixelFormat Pfmt)
        {
            if (Pfmt == PixelFormat.Format24bppRgb)
            {
                if ((Width * Height * 3) != (TotalPixSize))
                    return null;
            }
            else if (Pfmt == PixelFormat.Format16bppRgb565)
            {
                if ((Width * Height * 2) != (TotalPixSize))
                    return null;
            }
            else
            {
                return null;
            }

            try
            {
                Bitmap Canvas = new Bitmap(Width, Height, Pfmt);
                BitmapData CanvasData = Canvas.LockBits(new Rectangle(0, 0, Width, Height),
                                    ImageLockMode.WriteOnly, Pfmt);

                unsafe
                {
                    byte* Ptr = (byte*)CanvasData.Scan0.ToPointer();

                    for (int Y = 0; Y < TotalPixSize / BufSz; Y++)
                    {

                        for (int X = 0; X < BufSz; X++)
                        {
                            // Swap Data
                            if (X % 2 == 1)
                            {
                                *Ptr = DataBuf[Y][X];
                                Ptr++;
                                *Ptr = DataBuf[Y][X - 1];
                                Ptr++;
                            }
                        }
                    }
                }
                Canvas.UnlockBits(CanvasData);
                return Canvas;

            }
            catch (Exception)
            {
                return null;
            }

        }
  
        public void btnCapture_Click()
        {
            if (MyDevice == null || ep0 == null || inEndpoint == null) return;

            bRunning = true;
          
            BufSz = inEndpoint.MaxPktSize * M280DEF.Packet_Xfer;        
            QueueSz = BufNum;

            inEndpoint.XferSize = BufSz;

            // Initializing Data Buffer
            DataBuf = new byte[TotalPixSize / BufSz][];
            for (int i = 0; i < (TotalPixSize / BufSz); i++)
            {
                DataBuf[i] = new byte[BufSz];
            }

            tStartCap = new Thread(new ThreadStart(XferThread));
            tStartCap.IsBackground = true;
            tStartCap.Priority = ThreadPriority.Highest;
            tStartCap.Start();
            // Send Start capture command to M280
            SendCommand(M280DEF.CMD_ST_CAP, 0, 0);
        }

        public BitmapSource GetBitmapSource(System.Drawing.Bitmap source)
        {

            IntPtr ip = source.GetHbitmap();
            BitmapSource bs = null;

            try
            {
                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            }
            catch (Exception ex)
            {
               // lblStatus.Content = ex.ToString();

            }
            finally
            {
                DeleteObject(ip);

            }

            return bs;

        }

        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        public static extern void DeleteObject(IntPtr hObject);

        public static void SaveImage(ImageDetail ImageDetail)
        {
            Bitmap objBMP = default(Bitmap);

            try
            {

                if (DoesFileLocationExist(ImageDetail.FileName))
                {
                    objBMP = new Bitmap(ImageDetail.Image);
                    objBMP.Save(ImageDetail.FileName, ImageFormat.Jpeg);
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                //MyAppLog.WriteToLog("IVS", ex.ToString(), EventLogEntryType.Error);
            }
            finally
            {
                if (objBMP != null)
                {
                    objBMP.Dispose();
                }
            }

        }

        private static bool DoesFileLocationExist(string FileFolder)
        {

            try
            {

                if (System.IO.Directory.Exists(GetPathOfFile(FileFolder)) == true)
                {
                    return true;

                }
                else if (System.IO.Directory.Exists(GetPathOfFile(FileFolder)) == false)
                {
                    System.IO.Directory.CreateDirectory(GetPathOfFile(FileFolder));
                    return true;
                }

            }
            catch (Exception ex)
            {
                //MyAppLog.WriteToLog("IVS", ex.ToString(), EventLogEntryType.Error);
            }

            return true;
        }

        private static string GetPathOfFile(string FullFileName)
        {
            //int intPosition = 0;
            string strFilePath = null;

            try
            {
                strFilePath = new System.IO.FileInfo(FullFileName).Directory.Name;

            }
            catch (Exception ex)
            {
                //MyAppLog.WriteToLog("IVS", ex.ToString(), EventLogEntryType.Error);
            }

            return strFilePath;

        }
  }

    public class M280DEF
    {
        // PID, VID
        //Prototype Demo Unit
        //public const ushort USB_PID = 0x1003;
        //public const ushort USB_VID = 0x04b4;
        //Production Unit
        public const ushort USB_PID = 0x0280;
        public const ushort USB_VID = 0x28A6;
        
        // Software Version
        public const string SWver = "1.00";
        // Define Command
        public const byte CMD_ST_CAP = 0xDA;
        public const byte CMD_SET_ILLUMIN = 0xE0;
        public const byte CMD_INIT_CAMERA = 0xE1;
        public const byte CMD_GET_STATE = 0xEA;
        public const byte CMD_GET_VERSION = 0xEB;

        // F/W version length
        public const byte MAX_CNUM_APP1 = 10;

        // Image download 
        //public const PixelFormat PixFormat = PixelFormat.Format24bppRgb;
        public const PixelFormat PixFormat = PixelFormat.Format16bppRgb565;
        public const int QUEUE_SIZE = 16;    // 1024 * 768
        public const int Packet_Xfer = 12;      // 512 * 48 = 24576
        public const int Image_Xsize = 1024;
        public const int Image_Ysize = 768;

        // Crop Image
        public const int CropUpleftX = 78;
        public const int CropUpLeftY = 100;
        public const int CropSizeX = 880;
        public const int CropSizeY = 560;

        // Status 
        public const int STATUS_SIZE = 2;
        public const ushort stat_ReadyLED = 1 << 0;
        public const ushort stat_BusyLED = 1 << 1;
        public const ushort stat_CardDet = 1 << 2;
        public const ushort stat_CapDet = 1 << 3;
        public const ushort stat_CamInit = 1 << 4;
        public const ushort stat_SysBusy = 1 << 5;

        public const ushort stat_EngineEr = 1 << 7;
        public const ushort stat_EepromEr = 1 << 8;
        public const ushort stat_FPGAEr = 1 << 9;
        public const ushort StatusGetInterval = 100;  // 100mS


        // Others...
        public const ushort ON = 1;
        public const ushort OFF = 0;
    }
}

public class ImageDetail
{

    private Bitmap _Image;

    private string _FileName;
    public Bitmap Image
    {
        get { return _Image; }
        set { _Image = value; }
    }

    public string FileName
    {
        get { return _FileName; }
        set { _FileName = value; }
    }
}