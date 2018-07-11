Imports System.Text
Imports IVS.Data

Public Class WinAlertAnoka

    Private objAlertDetailAnoka As AlertDetailAnoka

    Public Sub New(ByVal AlertDetailAnoka As AlertDetailAnoka)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Try
            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner

            objAlertDetailAnoka = AlertDetailAnoka

            Me.txtAlertOrganization.Text = objAlertDetailAnoka.OrganizationName
            Me.txtAlertNameLast.Text = objAlertDetailAnoka.ParentLastName
            Me.txtAlertNameFirst.Text = objAlertDetailAnoka.ParentFirstName
            Me.txtDateOfBirth.Text = objAlertDetailAnoka.ParentDOB
            Me.txtAlertAddress.Text = objAlertDetailAnoka.AddressLine1
            Me.txtAlertCityState.Text = String.Format("{0}, {1}", objAlertDetailAnoka.City, objAlertDetailAnoka.State)
            Me.txtAlertNotes.Text = objAlertDetailAnoka.AlertDescription

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdCancel_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCancel.Click

        Try
            Me.DialogResult = False
            Me.Close()

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

End Class
