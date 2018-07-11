Imports System.Text
Imports IVS.Data
Imports IVS.AppLog

Public Class WinAlert

    Private MyAppLog As New ApplicationLog(ApplicationLog.DateEncodingType.YYYYMMDD, True)
    Private objAlertDetail As AlertDetail
    Private isContentChanged As Boolean = False

#Region "Control Bound Subs"

    Public Sub New(ByVal AlertDetail As AlertDetail, Optional ByVal EditMode As Boolean = False)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Dim sb As New StringBuilder

        Try
            objAlertDetail = AlertDetail

            MyAppLog.WriteToLog("WinAlert.New()AlertID:" & objAlertDetail.AlertID)
            MyAppLog.WriteToLog("WinAlert.New()EditMode:" & EditMode)

            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner

            If objAlertDetail.AlertID = 0 Then

                If objAlertDetail.IDNumber IsNot Nothing Then
                    Me.txtAlertIDAccountNumber.Text = objAlertDetail.IDNumber
                End If

                If objAlertDetail.NameFirst IsNot Nothing Then
                    Me.txtAlertNameFirst.Text = objAlertDetail.NameFirst
                End If

                If objAlertDetail.NameLast IsNot Nothing Then
                    Me.txtAlertNameLast.Text = objAlertDetail.NameLast
                End If

                If objAlertDetail.DateOfBirth IsNot Nothing Then
                    Me.txtDateOfBirth.Text = objAlertDetail.DateOfBirth
                End If

                Me.txtAlertContactName.Text = objAlertDetail.AlertContactName
                Me.txtAlertContactNumber.Text = objAlertDetail.AlertContactNumber

                If objAlertDetail.ActiveFlag = False Then
                    Me.cbIsActive.IsChecked = False
                Else
                    Me.cbIsActive.IsChecked = True
                End If

                Me.cbIsActive.IsChecked = True

                Me.lblLastUpdate.Visibility = Windows.Visibility.Hidden
                Me.cmdDelete.Visibility = Windows.Visibility.Hidden

            End If

            If objAlertDetail.AlertID > 0 Then

                Me.txtAlertIDAccountNumber.Text = objAlertDetail.IDNumber
                Me.txtAlertNameFirst.Text = objAlertDetail.NameFirst
                Me.txtAlertNameLast.Text = objAlertDetail.NameLast
                Me.txtDateOfBirth.Text = objAlertDetail.DateOfBirth
                Me.txtAlertContactName.Text = objAlertDetail.AlertContactName
                Me.txtAlertContactNumber.Text = objAlertDetail.AlertContactNumber

                Me.cbIsActive.IsChecked = objAlertDetail.ActiveFlag
                Me.txtAlertNotes.Text = objAlertDetail.AlertNotes

                sb.Append("Updated on ")
                sb.Append(objAlertDetail.UpdateTS)
                sb.Append(" by ")
                sb.Append(objAlertDetail.UserName)

                Me.lblLastUpdate.Content = sb.ToString

                Me.txtAlertContactName.Focus()

                If EditMode = False Then

                    Me.txtAlertIDAccountNumber.IsEnabled = False
                    Me.txtAlertNameFirst.IsEnabled = False
                    Me.txtAlertNameLast.IsEnabled = False
                    Me.txtDateOfBirth.IsEnabled = False
                    Me.txtAlertContactName.IsEnabled = False
                    Me.txtAlertContactNumber.IsEnabled = False
                    Me.cbIsActive.IsEnabled = False
                    Me.txtAlertNotes.IsEnabled = False
                    Me.imgIDRequired.Visibility = Windows.Visibility.Hidden
                    Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                    Me.cmdSave.Visibility = Windows.Visibility.Hidden
                    Me.cmdDelete.Visibility = Windows.Visibility.Hidden
                    Me.cmdCancel.Content = "Close"

                End If
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub WinAlert_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        Try
            AddHandler cbIsActive.Click, AddressOf TextChanged
            AddHandler txtAlertNameFirst.TextChanged, AddressOf TextChanged
            AddHandler txtAlertNameLast.TextChanged, AddressOf TextChanged
            AddHandler txtDateOfBirth.TextChanged, AddressOf TextChanged
            AddHandler txtAlertContactName.TextChanged, AddressOf TextChanged
            AddHandler txtAlertContactNumber.TextChanged, AddressOf TextChanged
            AddHandler txtAlertIDAccountNumber.TextChanged, AddressOf TextChanged
            AddHandler txtAlertNotes.TextChanged, AddressOf TextChanged

            Me.cmdSave.IsEnabled = False

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdDelete_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdDelete.Click

        Try

            If MessageBox.Show("Are you sure you want to delete this alert?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

                DataAccess.DeleteAlert(objAlertDetail.AlertID)

            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            Me.DialogResult = True
            Me.Close()

        End Try

    End Sub

    Private Sub cmdSave_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdSave.Click

        Dim objNewAlertDetail As New AlertDetail

        Try
            objNewAlertDetail.AlertType = "ALERT"
            objNewAlertDetail.IDNumber = Me.txtAlertIDAccountNumber.Text
            objNewAlertDetail.NameFirst = Me.txtAlertNameFirst.Text
            objNewAlertDetail.NameLast = Me.txtAlertNameLast.Text
            objNewAlertDetail.DateOfBirth = Me.txtDateOfBirth.Text
            objNewAlertDetail.AlertContactName = Me.txtAlertContactName.Text
            objNewAlertDetail.AlertContactNumber = Me.txtAlertContactNumber.Text
            objNewAlertDetail.ActiveFlag = Me.cbIsActive.IsChecked
            objNewAlertDetail.AlertNotes = Me.txtAlertNotes.Text
            objNewAlertDetail.UserID = objAlertDetail.UserID

            If objAlertDetail.AlertID = 0 Then

                DataAccess.NewAlert(objNewAlertDetail)

            Else

                objNewAlertDetail.AlertID = objAlertDetail.AlertID
                DataAccess.UpdateAlert(objNewAlertDetail)

            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            Me.DialogResult = True
            Me.Close()

        End Try

    End Sub

    Private Sub cmdCancel_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCancel.Click

        Try
            If isContentChanged = True Then

                If MessageBox.Show("Exit without saving changes?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

                    Me.DialogResult = False
                    Me.Close()

                End If
            Else
                Me.DialogResult = False
                Me.Close()
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

#Region "Custom Subs/Functions"

    Private Sub TextChanged()

        Try
            isContentChanged = True

            If Me.txtAlertIDAccountNumber.Text <> "" Or Me.txtAlertNameLast.Text <> "" Then

                Me.cmdSave.IsEnabled = True

            Else
                Me.cmdSave.IsEnabled = False
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

End Class