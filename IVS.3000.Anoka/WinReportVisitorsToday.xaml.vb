Imports IVS.Data
Imports Microsoft.Reporting.WinForms

Public Class WinReportVisitorsToday

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
        Dim dataset As New VisitorsToday
        Dim objReportParameter As ReportParameter

        Try
            objReportDataSource.Name = "DataSet1"
            objReportDataSource.Value = DataAccess.RptVisitorsToday(WinMain.intClientID)
            Me._reportViewer.LocalReport.DataSources.Add(objReportDataSource)
            Me._reportViewer.LocalReport.ReportPath = "ReportVisitorsToday.rdlc"
            objReportParameter = New ReportParameter("StationName", DataAccess.GetStationName(WinMain.intClientID))
            Me._reportViewer.LocalReport.SetParameters(objReportParameter)
            _reportViewer.RefreshReport()

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

End Class
