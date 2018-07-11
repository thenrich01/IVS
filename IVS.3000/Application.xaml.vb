Imports System.Security.Principal
Imports IVS.Data

Class Application

    Private MyAppLog As New IVS.AppLog.ApplicationLog(IVS.AppLog.ApplicationLog.DateEncodingType.YYYYMMDD, True)
    Public Shared objMyIDChecker As IDecode.Net.PC.Checker
    Private objClientSettings As New ClientSettings

    Private Sub Application_Startup(sender As Object, e As System.Windows.StartupEventArgs) Handles Me.Startup

        Dim intClientID As Integer
        Dim intIDecodeResponse As Integer

        Try
            intClientID = DataAccess.GetClientID
            objClientSettings = DataAccess.GetClientSettings(intClientID)

            Dim UserIdentity As WindowsIdentity
            UserIdentity = WindowsIdentity.GetCurrent
            Dim UserPrincipal As WindowsPrincipal
            UserPrincipal = New WindowsPrincipal(UserIdentity)

            objMyIDChecker = New IDecode.Net.PC.Checker

            If UserPrincipal.IsInRole(WindowsBuiltInRole.Administrator) Then

                intIDecodeResponse = Application.objMyIDChecker.Setup(objClientSettings.IDecodeTrackFormat, objClientSettings.IDecodeCardTypes)

            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

End Class