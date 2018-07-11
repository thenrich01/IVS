Imports IVS.AppLog

Public Class Installer1

    Public Shared MyAppLog As New ApplicationLog(ApplicationLog.DateEncodingType.YYYYMMDD, True)

    Public Sub New()
        MyBase.New()

        'This call is required by the Component Designer.
        InitializeComponent()

        'Add initialization code after the call to InitializeComponent

    End Sub

    Private Sub Installer1_AfterInstall(sender As Object, e As System.Configuration.Install.InstallEventArgs) Handles Me.AfterInstall

        Dim strTargetDirectory As String
        Dim strDevices As String
        Dim strProcess As String
        Dim p As Process

        Try
            'Get setup parameters
            strTargetDirectory = Context.Parameters("targetdir")
            strDevices = Context.Parameters("BTNDEVICE")

            MyAppLog.WriteToLog("IVS.Setup() Target Directory:" & strTargetDirectory)
            MyAppLog.WriteToLog("IVS.Setup() Device Driver Install Option:" & strDevices)

            Select Case strDevices
                Case 1
                    MyAppLog.WriteToLog("IVS.Setup() Device Driver Install: ESeek")
                Case 2
                    MyAppLog.WriteToLog("IVS.Setup() Device Driver Install: CTS")
                Case 3
                    MyAppLog.WriteToLog("IVS.Setup() Device Driver Install: Magtek USB")
                Case 4
                    MyAppLog.WriteToLog("IVS.Setup() Device Driver Install: Nothing")
            End Select

            MyAppLog.WriteToLog("IVS.Setup() Checking Target Directory parameter")

            If Right(Trim(strTargetDirectory), 2) = "\\" Then
                MyAppLog.WriteToLog("IVS.Setup() Removing last character from strTargetDirectory")
                strTargetDirectory = Left(strTargetDirectory, Len(strTargetDirectory) - 1)
            End If

            If Right(Trim(strTargetDirectory), 1) <> "\" Then
                MyAppLog.WriteToLog("IVS.Setup() Adding last character \ to strTargetDirectory")
                strTargetDirectory += "\"
            End If

            Select Case strDevices
                Case 1
                    'ESeek
                    strProcess = strTargetDirectory & "Drivers\ESeek\CDM20824_Setup.exe"
                    MyAppLog.WriteToLog("IVS.Setup() Starting process: " & strProcess)
                    p = System.Diagnostics.Process.Start(strProcess)
                    p.WaitForExit()
                    MyAppLog.WriteToLog("IVS.Setup() Finished process: " & strProcess)
                Case 2
                    'CTS LS40
                    strProcess = strTargetDirectory & "Drivers\CTS\Driver_installer.exe"
                    MyAppLog.WriteToLog("IVS.Setup() Starting process: " & strProcess)
                    p = System.Diagnostics.Process.Start(strProcess)
                    p.WaitForExit()
                    MyAppLog.WriteToLog("IVS.Setup() Finished process: " & strProcess)
                Case 3
                    'MagTek
                Case 4
                    'No drivers to install
            End Select

        Catch ex As Exception
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

End Class
