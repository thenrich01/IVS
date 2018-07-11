Imports IVS.Data

Public Class WinTextInput

    Private Sub WinTextInput_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        Try

            Me.txtReaderInput.Focus()

        Catch ex As Exception

        End Try

    End Sub

    Private Sub cmdCancel_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCancel.Click
        Me.Close()
    End Sub

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Try

            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner

        Catch ex As Exception

        End Try

    End Sub

    Private Sub txtReaderInput_TextChanged(sender As Object, e As System.Windows.Controls.TextChangedEventArgs) Handles txtReaderInput.TextChanged

        Dim CharCount As Integer = 0
        'Dim intTotalCharCount As Integer = 0
        Dim objSwipeScanInfo As New SwipeScanInfo
        Dim objSwipeScanDetail As New SwipeScanDetail

        Try

            For Each c As Char In txtReaderInput.Text

                If c = "?" Then CharCount += 1
                'If c = "%" Then Me.edtMicrText.Text = "Reading Card Data"

            Next

            If CharCount = 3 Then

                Dim strTrackData As String = txtReaderInput.Text

                'txtReaderInput.Text = Nothing

                objSwipeScanInfo.ClientID = WinMain.objClientSettings.ClientID
                objSwipeScanInfo.UserID = WinMain.intIVSUserID
                objSwipeScanInfo.CCDigits = WinMain.objClientSettings.CCDigits
                objSwipeScanInfo.DisableCCSave = WinMain.objClientSettings.DisableCCSave
                objSwipeScanInfo.DisableDBSave = WinMain.objClientSettings.DisableDBSave
                objSwipeScanInfo.IDChecker = Application.objMyIDChecker
                objSwipeScanInfo.SwipeScanRawData = strTrackData

                objSwipeScanDetail = DataAccess.NewDataSwipeScan(objSwipeScanInfo)

                WinScanImage.ManualEntrySwipeScanID = objSwipeScanDetail.SwipeScanID

                Me.DialogResult = True
                Me.Close()

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub
End Class
