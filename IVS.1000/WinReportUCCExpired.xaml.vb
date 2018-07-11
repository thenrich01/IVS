Imports IVS.Data
Imports IVS.AppLog
Imports Microsoft.Reporting.WinForms

Public Class WinReportUCCExpired

    Private strDateOfExpiration As Date
    Private MyAppLog As New ApplicationLog(ApplicationLog.DateEncodingType.YYYYMMDD, True)

    Public Sub New(ByVal DateOfExpiration As Date)

        Try
            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            strDateOfExpiration = DateOfExpiration
        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub _reportViewer_Load(sender As Object, e As System.EventArgs) Handles _reportViewer.Load

        Dim objReportDataSource As New Microsoft.Reporting.WinForms.ReportDataSource()
        Dim dataset As New SwipeScanDetail
        Dim objReportParameter As ReportParameter

        Try
            objReportDataSource.Name = "DataSet1"
            objReportDataSource.Value = DataAccess.GetUCCReport(strDateOfExpiration)
            Me._reportViewer.LocalReport.DataSources.Add(objReportDataSource)
            Me._reportViewer.LocalReport.ReportPath = "ReportUCCExpired.rdlc"

            objReportParameter = New ReportParameter("DateOfExpiration", strDateOfExpiration)
            Me._reportViewer.LocalReport.SetParameters(objReportParameter)
            _reportViewer.RefreshReport()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

End Class
