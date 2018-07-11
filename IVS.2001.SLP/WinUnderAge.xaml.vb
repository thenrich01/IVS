Imports IVS.AppLog
Imports IVS.Data.WS.TEP

Public Class WinUnderAge

    Public Shared MyAppLog As New ApplicationLog(ApplicationLog.DateEncodingType.YYYYMMDD, True)

    Public Sub New(ByVal Name As String, ByVal Age As Integer, ByVal MinimumAge As Integer)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Try
            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner
            Me.lblName.Content = Name
            Me.lblAge.Content = "is " & Age & " years old"
            Me.lbMinimumlAge.Content = "and is under the minimum age of " & MinimumAge

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdOK_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdOK.Click
        Me.Close()
    End Sub

End Class