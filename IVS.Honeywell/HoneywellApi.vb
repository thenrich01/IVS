Imports System.IO.Ports
Imports System.Text
Imports System.IO

Public Class HoneywellApi

    Private MyAppLog As New IVS.AppLog.ApplicationLog(IVS.AppLog.ApplicationLog.DateEncodingType.YYYYMMDD, True)
    Private WithEvents objSerialPort As New SerialPort
    Private strComPort As String
    Public Event OnDataReceived(ByVal DataReceived As String)

    Public Sub New(ByVal ComPort As String)

        Try
            strComPort = ComPort

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Public Function SerialPortOpen() As Boolean

        Dim isSerialPortOpen As Boolean = False

        Try
            objSerialPort.BaudRate = 115200
            objSerialPort.DataBits = 8
            objSerialPort.StopBits = StopBits.One
            objSerialPort.Parity = Parity.None
            objSerialPort.PortName = strComPort

            If objSerialPort.IsOpen = True Then

                objSerialPort.Close()

            End If

            objSerialPort.Open()
            isSerialPortOpen = True

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        Return isSerialPortOpen

    End Function

    Public Sub SerialPortClose()

        Try
            If objSerialPort.IsOpen Then

                objSerialPort.Close()

            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Public Sub VerifyDevice()

        Dim sb As New System.Text.StringBuilder

        Try
            Dim MySOH1 As String = Encoding.ASCII.GetChars({22})
            Dim MySOH2 As String = Encoding.ASCII.GetChars({77})
            Dim MySOH3 As String = Encoding.ASCII.GetChars({13})
            Dim myEOT As String = "."

            sb.Append(MySOH1)
            sb.Append(MySOH2)
            sb.Append(MySOH3)
            sb.Append("REVINF")
            sb.Append(myEOT)

            objSerialPort.Write(sb.ToString)

            sb.Clear()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub objSerialPort_DataReceived(sender As Object, e As System.IO.Ports.SerialDataReceivedEventArgs) Handles objSerialPort.DataReceived

        Try

            RaiseEvent OnDataReceived(objSerialPort.ReadExisting)

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub objSerialPort_ErrorReceived(sender As Object, e As System.IO.Ports.SerialErrorReceivedEventArgs) Handles objSerialPort.ErrorReceived

        MyAppLog.WriteToLog("ESeekApi.objSerialPort_ErrorReceived()")
        MyAppLog.WriteToLog(objSerialPort.ReadExisting)

    End Sub

End Class
