Imports IVS.Data
Imports Microsoft.Reporting.WinForms

Public Class CurrentVisitors
    Private Sub _reportViewer_Load(sender As Object, e As System.EventArgs) Handles _reportViewer.Load

        Dim objReportDataSource As New Microsoft.Reporting.WinForms.ReportDataSource()
        Dim dataset As New List(Of SwipeScanDetail)

        Try
            objReportDataSource.Name = "DataSet1"
            objReportDataSource.Value = DataAccess.GetUCCReport("10/24/2013")

            'dataset = DataAccess.GetUCCReport("10/24/2013")
            'MessageBox.Show(dataset.Count, "HI")
            Me._reportViewer.LocalReport.DataSources.Add(objReportDataSource)
            'Me._reportViewer.LocalReport.ReportPath = "Report1.rdlc"
            Me._reportViewer.LocalReport.ReportPath = "ReportUCCExpired.rdlc"

            Dim objReportParameter As ReportParameter = New ReportParameter("DateOfExpiration", Today)
            Me._reportViewer.LocalReport.SetParameters(objReportParameter)
            _reportViewer.RefreshReport()


            'Dim reportDataSource1 As New Microsoft.Reporting.WinForms.ReportDataSource()
            'Dim dataset As New SwipeScanDetail
            'reportDataSource1.Name = "DataSet1"
            'reportDataSource1.Value = DataAccess.GetUCCReport(Today)
            'Me._reportViewer.LocalReport.DataSources.Add(reportDataSource1)
            'Me._reportViewer.LocalReport.ReportPath = "../../ReportUCCExpired.rdlc"
            'Dim rp As ReportParameter = New ReportParameter("DateOfExpiration", "10/11/2012")
            'Me._reportViewer.LocalReport.SetParameters(rp)
            '_reportViewer.RefreshReport()

        Catch ex As Exception
            System.Windows.MessageBox.Show(ex.ToString, "exception")
        End Try

    End Sub
End Class
