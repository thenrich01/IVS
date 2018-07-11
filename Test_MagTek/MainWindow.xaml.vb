Imports IVS.MagTek
Imports System.ComponentModel
Imports System.Runtime.InteropServices

Class MainWindow

    Private DelegateHandlerCardDataStateChange As MagTekApi.CallBackCardDataStateChanged
    Private DelegateHandlerDeviceStateChange As MagTekApi.CallBackDeviceStateChanged

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        AddHandler Me.StateChanged, AddressOf Window_StateChanged

    End Sub

    Private Sub Window_StateChanged(sender As Object, e As EventArgs)

        Dim intResult As Integer

        Select Case Me.WindowState
            Case WindowState.Maximized

                Me.TextBox1.Text += "WindowState.Maximized()" & vbCrLf

                MagTekApi.MTUSCRAGetDeviceState(intResult)
                Me.TextBox1.Text += "MTUSCRAGetDeviceState()" & intResult & vbCrLf

                If intResult = 1 Then
                    Me.TextBox1.Text += "Device Already Connected" & vbCrLf

                Else
                    intResult = MagTekApi.MTUSCRAOpenDevice("")
                    Me.TextBox1.Text += "MTUSCRAGetDeviceState()" & intResult & vbCrLf

                    Me.TextBox1.Text += "DelegateHandlerCardDataStateChange()" & vbCrLf

                    DelegateHandlerCardDataStateChange = New MagTekApi.CallBackCardDataStateChanged(AddressOf objUSBMagTek_OnDataReceived)
                    MagTekApi.MTUSCRACardDataStateChangedNotify(DelegateHandlerCardDataStateChange)

                    DelegateHandlerDeviceStateChange = New MagTekApi.CallBackDeviceStateChanged(AddressOf objUSBMagTek_OnDeviceStateChanged)
                    MagTekApi.MTUSCRADeviceStateChangedNotify(DelegateHandlerDeviceStateChange)

                    MagTekApi.MTUSCRAGetDeviceState(intResult)
                    Me.TextBox1.Text += "MTUSCRAGetDeviceState()" & intResult & vbCrLf

                End If

                Exit Select
            Case WindowState.Minimized
                Me.TextBox1.Text += "WindowState.Minimized()" & vbCrLf

                'In order to Disconnect, DROP the delegate(s)
                MagTekApi.MTUSCRAGetDeviceState(intResult)
                Me.TextBox1.Text += "MTUSCRAGetDeviceState()" & intResult & vbCrLf

                MagTekApi.MTUSCRAClearBuffer()
                Me.TextBox1.Text += "MTUSCRAClearBuffer()" & vbCrLf

                Dim DelegateDataState As [Delegate] = TryCast(DelegateHandlerCardDataStateChange, [Delegate])
                DelegateHandlerCardDataStateChange = TryCast([Delegate].RemoveAll(DelegateDataState, DelegateDataState), MagTekApi.CallBackCardDataStateChanged)

                Dim DelegateDeviceState As [Delegate] = TryCast(DelegateHandlerDeviceStateChange, [Delegate])
                DelegateHandlerDeviceStateChange = TryCast([Delegate].RemoveAll(DelegateDeviceState, DelegateDeviceState), MagTekApi.CallBackDeviceStateChanged)

                MagTekApi.MTUSCRAGetDeviceState(intResult)
                Me.TextBox1.Text += "MTUSCRAGetDeviceState()" & intResult & vbCrLf

                intResult = MagTekApi.MTUSCRACloseDevice()
                Me.TextBox1.Text += "MTUSCRACloseDevice()" & intResult & vbCrLf

                MagTekApi.MTUSCRAGetDeviceState(intResult)
                Me.TextBox1.Text += "MTUSCRAGetDeviceState()" & intResult & vbCrLf

                Exit Select
            Case WindowState.Normal
                Me.TextBox1.Text += "WindowState.Normal()" & vbCrLf


                MagTekApi.MTUSCRAGetDeviceState(intResult)
                Me.TextBox1.Text += "MTUSCRAGetDeviceState()" & intResult & vbCrLf

                If intResult = 1 Then
                    Me.TextBox1.Text += "Device Already Connected" & vbCrLf

                Else
                    intResult = MagTekApi.MTUSCRAOpenDevice("")
                    Me.TextBox1.Text += "MTUSCRAGetDeviceState()" & intResult & vbCrLf

                    Me.TextBox1.Text += "DelegateHandlerCardDataStateChange()" & vbCrLf

                    DelegateHandlerCardDataStateChange = New MagTekApi.CallBackCardDataStateChanged(AddressOf objUSBMagTek_OnDataReceived)
                    MagTekApi.MTUSCRACardDataStateChangedNotify(DelegateHandlerCardDataStateChange)

                    DelegateHandlerDeviceStateChange = New MagTekApi.CallBackDeviceStateChanged(AddressOf objUSBMagTek_OnDeviceStateChanged)
                    MagTekApi.MTUSCRADeviceStateChangedNotify(DelegateHandlerDeviceStateChange)

                    MagTekApi.MTUSCRAGetDeviceState(intResult)
                    Me.TextBox1.Text += "MTUSCRAGetDeviceState()" & intResult & vbCrLf

                End If

                Exit Select
        End Select

    End Sub

    Private Sub objUSBMagTek_OnDataReceived(CardDataState As Integer)

        Dim lResult As Integer
        Dim strData As String

        Try
            If CardDataState = MagTekApi.MTSCRA_DATA_READY Then

                Dim structTEST As New MagTekApi.MTMSRDATA

                lResult = MagTekApi.MTUSCRAGetCardData(structTEST)


                'strData = Space(4096)

                'lResult = MagTekApi.MTUSCRAGetCardDataStr(strData, "|")
                MagTekApi.MTUSCRAClearBuffer()

                Me.Dispatcher.BeginInvoke(New StartTheInvoke_OnDataReceived(AddressOf NowStartTheInvoke_OnDataReceived), structTEST)
            End If

        Catch ex As Exception
            Me.TextBox1.Text += "cmdClose_Click()" & ex.ToString
        End Try

    End Sub

    Delegate Sub StartTheInvoke_OnDataReceived(ByVal StrRawUSBPortData As MagTekApi.MTMSRDATA)

    Friend Sub NowStartTheInvoke_OnDataReceived(ByVal StrRawUSBPortData As MagTekApi.MTMSRDATA)

        Try
            'Me.TextBox1.Text += "NowStartTheInvoke_OnDataReceived()" & StrRawUSBPortData & vbCrLf
            ' Me.TextBox1.Text += "m_dwReaderID:" & StrRawUSBPortData.m_szDeviceSerialNumbe & vbCrLf
            Me.TextBox1.Text += "DATA:" & StrRawUSBPortData.m_szCardData & vbCrLf

        Catch ex As Exception
            Me.TextBox1.Text += "NowStartTheInvoke_OnDataReceived()" & ex.ToString
        End Try

    End Sub

    Delegate Sub StartTheInvoke_OnDeviceStateChanged(ByVal DeviceState As Integer)

    Friend Sub NowStartTheInvoke_OnDeviceStateChanged(ByVal DeviceState As Integer)

        Dim intResult As Integer

        Try
            Select Case DeviceState

                Case MagTekApi.MTSCRA_STATE_CONNECTED

                    Me.TextBox1.Text += "MTSCRA_STATE_CONNECTED" & vbCrLf

                Case MagTekApi.MTSCRA_STATE_DISCONNECTED

                    Me.TextBox1.Text += "MTSCRA_STATE_DISCONNECTED" & vbCrLf

                    Dim DelegateDataState As [Delegate] = TryCast(DelegateHandlerCardDataStateChange, [Delegate])
                    DelegateHandlerCardDataStateChange = TryCast([Delegate].RemoveAll(DelegateDataState, DelegateDataState), MagTekApi.CallBackCardDataStateChanged)

                    Dim DelegateDeviceState As [Delegate] = TryCast(DelegateHandlerDeviceStateChange, [Delegate])
                    DelegateHandlerDeviceStateChange = TryCast([Delegate].RemoveAll(DelegateDeviceState, DelegateDeviceState), MagTekApi.CallBackDeviceStateChanged)

                Case MagTekApi.MTSCRA_STATE_ERROR

                    Me.TextBox1.Text += "MTSCRA_STATE_ERROR" & vbCrLf

                    MagTekApi.MTUSCRAClearBuffer()
                    Me.TextBox1.Text += "MTUSCRAClearBuffer()" & vbCrLf

                    Dim DelegateDataState As [Delegate] = TryCast(DelegateHandlerCardDataStateChange, [Delegate])
                    DelegateHandlerCardDataStateChange = TryCast([Delegate].RemoveAll(DelegateDataState, DelegateDataState), MagTekApi.CallBackCardDataStateChanged)

                    Dim DelegateDeviceState As [Delegate] = TryCast(DelegateHandlerDeviceStateChange, [Delegate])
                    DelegateHandlerDeviceStateChange = TryCast([Delegate].RemoveAll(DelegateDeviceState, DelegateDeviceState), MagTekApi.CallBackDeviceStateChanged)

                    intResult = MagTekApi.MTUSCRACloseDevice()
                    Me.TextBox1.Text += "MTUSCRACloseDevice()" & intResult & vbCrLf

            End Select

        Catch ex As Exception
            Me.TextBox1.Text += "NowStartTheInvoke_OnStateChanged()" & ex.ToString
        End Try

    End Sub

    Private Sub objUSBMagTek_OnDeviceStateChanged(ByVal DeviceState As Integer)

        Try

            Me.Dispatcher.BeginInvoke(New StartTheInvoke_OnDeviceStateChanged(AddressOf NowStartTheInvoke_OnDeviceStateChanged), DeviceState)

        Catch ex As Exception
            Me.TextBox1.Text += "objUSBMagTek_OnDeviceStateChanged()" & ex.ToString
        End Try

    End Sub

    Private Sub cmdConnect_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdConnect.Click

        Dim intResult As Integer

        Try

            intResult = MagTekApi.MTUSCRAOpenDevice("")

            If intResult <> MagTekApi.MTSCRA_ST_OK Then
                MessageBox.Show("Unable to connect to MagTek USB reader.  Ensure reader is properly connected and open window again.", "IVS", MessageBoxButton.OK)

            Else
                DelegateHandlerCardDataStateChange = New MagTekApi.CallBackCardDataStateChanged(AddressOf objUSBMagTek_OnDataReceived)
                MagTekApi.MTUSCRACardDataStateChangedNotify(DelegateHandlerCardDataStateChange)

                DelegateHandlerDeviceStateChange = New MagTekApi.CallBackDeviceStateChanged(AddressOf objUSBMagTek_OnDeviceStateChanged)
                MagTekApi.MTUSCRADeviceStateChangedNotify(DelegateHandlerDeviceStateChange)

            End If

            Me.TextBox1.Text += "MTUSCRAOpenDevice()" & intResult & vbCrLf

        Catch ex As Exception
            Me.TextBox1.Text += "cmdConnect_Click()" & ex.ToString
        End Try

    End Sub

    Private Sub cmdDisconnect_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdDisconnect.Click

        Dim intResult As Integer

        Try
            MagTekApi.MTUSCRAGetDeviceState(intResult)
            Me.TextBox1.Text += "MTUSCRAGetDeviceState()" & intResult & vbCrLf

            MagTekApi.MTUSCRAClearBuffer()
            Me.TextBox1.Text += "MTUSCRAClearBuffer()" & vbCrLf

            Dim DelegateDataState As [Delegate] = TryCast(DelegateHandlerCardDataStateChange, [Delegate])
            DelegateHandlerCardDataStateChange = TryCast([Delegate].RemoveAll(DelegateDataState, DelegateDataState), MagTekApi.CallBackCardDataStateChanged)

            Dim DelegateDeviceState As [Delegate] = TryCast(DelegateHandlerDeviceStateChange, [Delegate])
            DelegateHandlerDeviceStateChange = TryCast([Delegate].RemoveAll(DelegateDeviceState, DelegateDeviceState), MagTekApi.CallBackDeviceStateChanged)

            MagTekApi.MTUSCRAGetDeviceState(intResult)
            Me.TextBox1.Text += "MTUSCRAGetDeviceState()" & intResult & vbCrLf

            intResult = MagTekApi.MTUSCRACloseDevice()
            Me.TextBox1.Text += "MTUSCRACloseDevice()" & intResult & vbCrLf

            MagTekApi.MTUSCRAGetDeviceState(intResult)
            Me.TextBox1.Text += "MTUSCRAGetDeviceState()" & intResult & vbCrLf

        Catch ex As Exception
            Me.TextBox1.Text += "cmdDisconnect_Click()" & ex.ToString
        End Try

    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles Button1.Click
        'Dim strCommand As String = Me.TextBox2.Text
        'Dim strResult As String = Nothing
        'Dim intResult As String = Nothing

        '' MagTekApi.MTUSCRASendCommand(Convert.ToInt32("00", 16), Convert.ToInt32("01", 16), strResult, intResult)
        'MagTekApi.MTUSCRASendCommand("00 01 00", Conversion.Hex("01"), strResult, intResult)
        'Me.TextBox1.Text += intResult & vbCrLf
        'Me.TextBox1.Text += strResult & vbCrLf

        Try
            Dim Uid As UInteger

            MagTekApi.MTUSCRAGetPID(Uid)
            Me.TextBox1.Text += Uid & vbCrLf
        Catch ex As Exception
            Me.TextBox1.Text += "Button1_Click()" & ex.ToString
        End Try


    End Sub

End Class
