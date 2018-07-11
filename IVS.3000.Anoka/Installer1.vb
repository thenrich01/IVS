Imports System.Configuration
Imports System.Text
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
        Dim strSqlServername As String
        Dim strSqlServerDatabase As String
        Dim strSqlServerUserName As String
        Dim strSqlServerPassword As String
        'Dim strAccessDatabase As String
        'Dim strDevices As String
        Dim strDatabase As String
        Dim strDatabaseType As String
        Dim strConnectionString As String = Nothing
        Dim strProviderName As String = Nothing
        Dim strExepath As String
        Dim myConfig As Configuration
        'Dim strProcess As String
        'Dim p As Process

        Try
            'Get setup parameters
            strTargetDirectory = Context.Parameters("targetdir")
            strSqlServername = Context.Parameters("SQLSRVR")
            strSqlServerDatabase = Context.Parameters("SQLDB")
            strSqlServerUserName = Context.Parameters("SQLUSR")
            strSqlServerPassword = Context.Parameters("SQLPW")
            'strAccessDatabase = Context.Parameters("ACCDB")
            'strDevices = Context.Parameters("BTNDEVICE")
            strDatabase = Context.Parameters("BTNDB")
            strDatabaseType = Context.Parameters("BTNDBTYPE")

            If Trim(strSqlServername) = "" Then
                strSqlServername = Context.Parameters("SRVR")
            End If

            If Trim(strSqlServerDatabase) = "" Then
                strSqlServerDatabase = Context.Parameters("DB")
            End If

            If Trim(strSqlServerUserName) = "" Then
                strSqlServerUserName = Context.Parameters("USR")
            End If

            If Trim(strSqlServerPassword) = "" Then
                strSqlServerPassword = Context.Parameters("PW")
            End If

            'If Trim(strAccessDatabase) = "" Then
            '    strAccessDatabase = Context.Parameters("DB")
            'End If

            MyAppLog.WriteToLog("IVS.Setup() Target Directory:" & strTargetDirectory)
            MyAppLog.WriteToLog("IVS.Setup() SQL Server Name:" & strSqlServername)
            MyAppLog.WriteToLog("IVS.Setup() SQL Server DB:" & strSqlServerDatabase)
            MyAppLog.WriteToLog("IVS.Setup() SQL Server User:" & strSqlServerUserName)
            ' MyAppLog.WriteToLog("IVS.Setup() SQL Server PW:" & strSqlServerPassword)
            'MyAppLog.WriteToLog("IVS.Setup() Access DB Location:" & strAccessDatabase)
            'MyAppLog.WriteToLog("IVS.Setup() Device Driver Install Option:" & strDevices)

            'Select Case strDevices
            '    Case 1
            '        MyAppLog.WriteToLog("IVS.Setup() Device Driver Install: ESeek")
            '    Case 2
            '        MyAppLog.WriteToLog("IVS.Setup() Device Driver Install: CTS")
            '    Case 3
            '        MyAppLog.WriteToLog("IVS.Setup() Device Driver Install: Magtek USB")
            '    Case 4
            '        MyAppLog.WriteToLog("IVS.Setup() Device Driver Install: Nothing")
            'End Select

            'MyAppLog.WriteToLog("IVS.Setup() Database Option:" & strDatabase)

            'Select Case strDatabase
            '    Case 1
            '        MyAppLog.WriteToLog("IVS.Setup() Database Option: Existing Database")
            '    Case 2
            '        MyAppLog.WriteToLog("IVS.Setup() Database Option: New Database")
            'End Select

            'MyAppLog.WriteToLog("IVS.Setup() Database Type Option:" & strDatabaseType)

            Select Case strDatabaseType
                Case 1
                    MyAppLog.WriteToLog("IVS.Setup() Database Type: Access")
                Case 2
                    MyAppLog.WriteToLog("IVS.Setup() Database Type: SQl Server")
                Case 3
                    MyAppLog.WriteToLog("IVS.Setup() Database Type: Manual Install")
            End Select

            MyAppLog.WriteToLog("IVS.Setup() Checking Target Directory parameter")

            If Right(Trim(strTargetDirectory), 2) = "\\" Then
                strTargetDirectory = Left(strTargetDirectory, Len(strTargetDirectory) - 1)
            End If

            If Right(Trim(strTargetDirectory), 1) <> "\" Then
                strTargetDirectory += "\"
            End If

            'Set Connection String in App.config file
            MyAppLog.WriteToLog("IVS.Setup() Building connection string")

            strExepath = String.Format("{0}IVS.exe", strTargetDirectory)
            myConfig = ConfigurationManager.OpenExeConfiguration(strExepath)

            Select Case strDatabaseType

                Case 1
                    'Access
                    'strConnectionString = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}\IVS.3000.accdb", strAccessDatabase)
                    'strProviderName = "System.Data.OleDb"

                Case 2
                    'SQL
                    strConnectionString = String.Format("Provider=SQLOLEDB;Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}", strSqlServername, strSqlServerDatabase, strSqlServerUserName, strSqlServerPassword)
                    strProviderName = "System.Data.SqlClient"

                Case 3
                    'Manual
                    strConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\IVS.3000.accdb"
                    strProviderName = "System.Data.OleDb"
            End Select

            MyAppLog.WriteToLog("IVS.Setup() Connection ProviderName: " & strProviderName)

            MyAppLog.WriteToLog("IVS.Setup() Writing connection string to app.config")
            myConfig.ConnectionStrings.ConnectionStrings("IVS.MySettings.IVSConnectionString").ConnectionString = strConnectionString
            myConfig.ConnectionStrings.ConnectionStrings("IVS.MySettings.IVSConnectionString").ProviderName = strProviderName

            MyAppLog.WriteToLog("IVS.Setup() Saving updated app.config")
            myConfig.Save()

            'Install Access DB Engine

            'strProcess = strTargetDirectory & "Drivers\AccessDatabaseEngine.exe"
            'MyAppLog.WriteToLog("IVS.Setup() Starting process: " & strProcess)
            'p = System.Diagnostics.Process.Start(strProcess, "/passive")
            'p.WaitForExit()
            'MyAppLog.WriteToLog("IVS.Setup() Finished process: " & strProcess)

            'Select Case strDevices
            '    Case 1
            '        'ESeek
            '        strProcess = strTargetDirectory & "Drivers\ESeek\CDM20830_Setup.exe"
            '        MyAppLog.WriteToLog("IVS.Setup() Starting process: " & strProcess)
            '        p = System.Diagnostics.Process.Start(strProcess)
            '        p.WaitForExit()
            '        MyAppLog.WriteToLog("IVS.Setup() Finished process: " & strProcess)
            '    Case 2
            '        'CTS LS40
            '        strProcess = strTargetDirectory & "Drivers\CTS\Driver_installer.exe"
            '        MyAppLog.WriteToLog("IVS.Setup() Starting process: " & strProcess)
            '        p = System.Diagnostics.Process.Start(strProcess)
            '        p.WaitForExit()
            '        MyAppLog.WriteToLog("IVS.Setup() Finished process: " & strProcess)
            '    Case 3
            '        'MagTek
            '    Case 4
            '        'No drivers to install
            'End Select

            'Copy Database file/Create Database

            'If strDatabase = 2 Then

            '    'Copy access DB
            '    MyAppLog.WriteToLog("IVS.Setup() Copying Access database to target location")
            '    Dim strSourceFile As String = String.Format("{0}IVS.3000.accdb", strTargetDirectory)
            '    Dim strDestinationDirectory As String = strAccessDatabase
            '    Dim strDestinationFile As String = String.Format("{0}\IVS.3000.accdb", strAccessDatabase)

            '    If My.Computer.FileSystem.DirectoryExists(strDestinationDirectory) = False Then

            '        MyAppLog.WriteToLog("IVS.Setup() Creating destination directory")
            '        My.Computer.FileSystem.CreateDirectory(strDestinationDirectory)

            '    End If

            '    MyAppLog.WriteToLog("IVS.Setup() Destination directory exists - Copying file")
            '    My.Computer.FileSystem.CopyFile(strSourceFile, strDestinationFile, True)

            'End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

End Class