Imports IVS.Data

Class Application

    Private MyAppLog As New IVS.AppLog.ApplicationLog(IVS.AppLog.ApplicationLog.DateEncodingType.YYYYMMDD, True)

    Private Sub Application_Startup(sender As Object, e As System.Windows.StartupEventArgs) Handles Me.Startup

        Try
            MyAppLog.WriteToLog("*********************** Starting IVS **********************")
            MyAppLog.WriteToLog("IVS.Application_Startup() Version " & My.Application.Info.Version.ToString)
            MyAppLog.WriteToLog("IVS.Application_Startup() Build Date " & DateAdd(DateInterval.Day, My.Application.Info.Version.Build, DateValue("01/01/2000")))
            MyAppLog.WriteToLog("IVS.Application_Startup() Build Time " & Date.FromOADate(My.Application.Info.Version.Revision / 1800 / 24))
            MyAppLog.WriteToLog("IVS.Application_Startup() Client Hostname: " & My.Computer.Name)
            MyAppLog.WriteToLog("IVS.Application_Startup() Client IPAddress: " & DataAccess.ReturnMyIPAddress)
            MyAppLog.WriteToLog("IVS.Application_Startup() AssemblyName " & My.Application.Info.AssemblyName)
            MyAppLog.WriteToLog("IVS.Application_Startup() ProductName " & My.Application.Info.ProductName)

            For Each item In My.Application.Info.LoadedAssemblies
                MyAppLog.WriteToLog("IVS.Application_Startup() Loaded Assembly " & item.FullName)
            Next

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

End Class