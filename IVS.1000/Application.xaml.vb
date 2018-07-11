Imports IVS.Data
Imports System.Configuration
Imports System.Security.Principal

Class Application

    Private MyAppLog As New IVS.AppLog.ApplicationLog(IVS.AppLog.ApplicationLog.DateEncodingType.YYYYMMDD, True)
    Public Shared objMyIDChecker As IDecode.Net.PC.Checker
    Private objClientSettings As New ClientSettings

    Private Sub Application_Startup(sender As Object, e As System.Windows.StartupEventArgs) Handles Me.Startup

        Dim intClientID As Integer
        Dim intIDecodeResponse As Integer

        Try
            'MyAppLog.WriteToLog("*********************** Starting IVS **********************")
            'MyAppLog.WriteToLog("IVS.Application_Startup() Version " & My.Application.Info.Version.ToString)
            'MyAppLog.WriteToLog("IVS.Application_Startup() Build Date " & DateAdd(DateInterval.Day, My.Application.Info.Version.Build, DateValue("01/01/2000")))
            'MyAppLog.WriteToLog("IVS.Application_Startup() Build Time " & Date.FromOADate(My.Application.Info.Version.Revision / 1800 / 24))
            'MyAppLog.WriteToLog("IVS.Application_Startup() Client Hostname: " & My.Computer.Name)
            'MyAppLog.WriteToLog("IVS.Application_Startup() Client IPAddress: " & DataAccess.ReturnMyIPAddress)
            'MyAppLog.WriteToLog("IVS.Application_Startup() AssemblyName " & My.Application.Info.AssemblyName)
            'MyAppLog.WriteToLog("IVS.Application_Startup() ProductName " & My.Application.Info.ProductName)

            'For Each item In My.Application.Info.LoadedAssemblies
            '    MyAppLog.WriteToLog("IVS.Application_Startup() Loaded Assembly " & item.FullName)
            'Next

            'MyAppLog.WriteToLog("IVS.Application_Startup() Data provider " & ConfigurationManager.ConnectionStrings("IVS.MySettings.IVSConnectionString").ProviderName)
            'MyAppLog.WriteToLog("IVS.Application_Startup() Connection String " & ConfigurationManager.ConnectionStrings("IVS.MySettings.IVSConnectionString").ConnectionString)

            intClientID = DataAccess.GetClientID
            objClientSettings = DataAccess.GetClientSettings(intClientID)

            Dim UserIdentity As WindowsIdentity
            UserIdentity = WindowsIdentity.GetCurrent
            Dim UserPrincipal As WindowsPrincipal
            UserPrincipal = New WindowsPrincipal(UserIdentity)

            objMyIDChecker = New IDecode.Net.PC.Checker

            MyAppLog.WriteToLog("IVS.Application_Startup() IDecode LicenseStatus: " & objMyIDChecker.LicenseStatus)
            'MyAppLog.WriteToLog("IVS.Application_Startup() IDecode Version: " & objMyIDChecker.Version)
            'MyAppLog.WriteToLog("IVS.Application_Startup() IDecode TrialDaysRemaining: " & objMyIDChecker.TrialDaysRemaining)

            If UserPrincipal.IsInRole(WindowsBuiltInRole.Administrator) Then

                'MyAppLog.WriteToLog("IVS.Application_Startup() Application user is administrator")

                'MyAppLog.WriteToLog("IVS.Application_Startup() objMyIDChecker.TrackFormat()" & objClientSettings.IDecodeTrackFormat)
                'MyAppLog.WriteToLog("IVS.Application_Startup() objMyIDChecker.CardTypes()" & objClientSettings.IDecodeCardTypes)

                intIDecodeResponse = Application.objMyIDChecker.Setup(objClientSettings.IDecodeTrackFormat, objClientSettings.IDecodeCardTypes)

                MyAppLog.WriteToLog("IVS.Application_Startup() objMyIDChecker.Setup()" & intIDecodeResponse)

            Else
                MyAppLog.WriteToLog("IVS.Application_Startup() Application user is NOT administrator")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

End Class