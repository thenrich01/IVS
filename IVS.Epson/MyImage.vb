Imports System.Windows.Media.Imaging
Imports System.Windows

Public Class MyImage
    <System.Runtime.InteropServices.DllImport("gdi32.dll")> _
    Public Shared Function DeleteObject(hObject As IntPtr) As Boolean
    End Function

    Public Shared Function GetBitmapSource(source As System.Drawing.Bitmap) As BitmapSource

        Dim ip As IntPtr = source.GetHbitmap()
        Dim bs As BitmapSource = Nothing

        Try
            bs = Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())

        Catch ex As Exception
            'MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            DeleteObject(ip)

        End Try

        Return bs

    End Function

End Class
