Imports System.IO.Ports
Imports System.Text
Imports System.IO

Public Class ESeekApi

    Private MyAppLog As New IVS.AppLog.ApplicationLog(IVS.AppLog.ApplicationLog.DateEncodingType.YYYYMMDD, True)
    Private WithEvents objSerialPort As New SerialPort
    Private strComPort As String
    Private intSleepMilliSeconds As Integer
    Public Event OnDataReceived(ByVal DataReceived As String)

    Public Sub New(ByVal ComPort As String, ByVal SleepMilliSeconds As Integer)

        Try
            strComPort = ComPort
            intSleepMilliSeconds = SleepMilliSeconds

            'MyAppLog.WriteToLog("ESeekApi.New() Port: " & ComPort)
            'MyAppLog.WriteToLog("ESeekApi.New() SleepMilliSeconds: " & SleepMilliSeconds)
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Public Function SerialPortOpen() As Boolean

        Dim isSerialPortOpen As Boolean = False

        Try
            objSerialPort.BaudRate = 9600
            objSerialPort.DataBits = 8
            objSerialPort.StopBits = StopBits.One
            objSerialPort.Parity = Parity.None
            objSerialPort.PortName = strComPort

            If objSerialPort.IsOpen = True Then

                objSerialPort.Close()
                'MyAppLog.WriteToLog("ESeekApi.SerialPortOpen() Serial port already open - closing serial port")

            End If

            objSerialPort.Open()
            'MyAppLog.WriteToLog("ESeekApi.SerialPortOpen() Opening serial port")
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
                'MyAppLog.WriteToLog("ESeekApi.SerialPortClose() Closing serial port")
            Else
                'MyAppLog.WriteToLog("ESeekApi.SerialPortClose() Serial port already closed")

            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Public Sub VerifyESeek()

        Dim sb As New System.Text.StringBuilder

        Try
            Dim MySOH As String = Encoding.ASCII.GetChars({1})
            Dim myEOT As String = Encoding.ASCII.GetChars({4})

            sb.Append(MySOH & "<FINGER:>" & myEOT)
            objSerialPort.Write(sb.ToString)

            sb.Clear()

            sb.Append(MySOH & "<SERIAL:>" & myEOT)
            objSerialPort.Write(sb.ToString)

            sb.Clear()

            sb.Append(MySOH & "<APP1RV:>" & myEOT)
            objSerialPort.Write(sb.ToString)

            sb.Clear()

            sb.Append(MySOH & "<HARDRV:>" & myEOT)
            objSerialPort.Write(sb.ToString)

            sb.Clear()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub objSerialPort_DataReceived(sender As Object, e As System.IO.Ports.SerialDataReceivedEventArgs) Handles objSerialPort.DataReceived

        Dim sb As New System.Text.StringBuilder

        Try

            Do

                If objSerialPort.BytesToRead > 0 Then

                    sb.Append(objSerialPort.ReadExisting)

                Else
                    'Old Snare used 80MS,200ms for Symbol P304 Readers
                    'New Snare=20MS
                    'Raised to 80MS for Virtual environments
                    System.Threading.Thread.Sleep(intSleepMilliSeconds)

                    If objSerialPort.BytesToRead <= 0 Then

                        Exit Do

                    End If

                End If
            Loop

            RaiseEvent OnDataReceived(sb.ToString)

            sb.Clear()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub objSerialPort_ErrorReceived(sender As Object, e As System.IO.Ports.SerialErrorReceivedEventArgs) Handles objSerialPort.ErrorReceived

        'NEED to reconnect here
        MyAppLog.WriteToLog("ESeekApi.objSerialPort_ErrorReceived()")
        MyAppLog.WriteToLog(objSerialPort.ReadExisting)

    End Sub

End Class