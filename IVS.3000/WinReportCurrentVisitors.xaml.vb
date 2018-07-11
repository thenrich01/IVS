Imports IVS.Data

Public Class WinReportCurrentVisitors

    Public Sub New()

        Try
            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub _reportViewer_Load(sender As Object, e As System.EventArgs) Handles _reportViewer.Load

        Dim objReportDataSource As New Microsoft.Reporting.WinForms.ReportDataSource()
        Dim dataset As New CurrentVisitors

        Try
            objReportDataSource.Name = "DataSet1"
            objReportDataSource.Value = DataAccess.RptCurrentVisitors(WinMain.intClientID)
            Me._reportViewer.LocalReport.DataSources.Add(objReportDataSource)
            Me._reportViewer.LocalReport.ReportPath = "ReportCurrentVisitors.rdlc"
            _reportViewer.RefreshReport()

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

End Class
