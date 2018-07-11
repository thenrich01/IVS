Imports IVS.AppLog
Imports IVS.Data.WS.TEP.IVSService
Imports IVS.Data.WS.TEP

Public Class WinViolations

    Private MyAppLog As New ApplicationLog(ApplicationLog.DateEncodingType.YYYYMMDD, True)
    Private vList As List(Of TEPViolations)
    Private intViolation1 As Integer
    Private intViolation2 As Integer
    Private intViolation3 As Integer
    Private intViolation4 As Integer
    Private intViolation5 As Integer
    Private intSwipeScanID As Integer

    Public Sub New(ByVal SwipeScanID As Integer)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Dim ListOfTEPSwipeScanViolations As New List(Of TEPSwipeScanViolations)

        Try
            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner

            intSwipeScanID = SwipeScanID

            vList = DataAccess.GetTEPViolations

            Me.cboViolation1.ItemsSource = vList
            Me.cboViolation1.DisplayMemberPath = ("StatuteOffense")
            Me.cboViolation1.SelectedValuePath = ("ViolationID")

            Me.cmdSave.IsEnabled = False
            Me.cmdClear.IsEnabled = False

            ListOfTEPSwipeScanViolations = New List(Of TEPSwipeScanViolations)(DataAccess.GetSwipeScanDetail_Violations(SwipeScanID))

            For Each objTEPSwipeScanViolation As TEPSwipeScanViolations In ListOfTEPSwipeScanViolations

                Select Case objTEPSwipeScanViolation.Sequence

                    Case 1
                        Me.lblViolation1.Content = objTEPSwipeScanViolation.Statute & " : " & objTEPSwipeScanViolation.Offense
                        Me.cboViolation1.Visibility = Windows.Visibility.Hidden
                        Me.cboViolation2.Visibility = Windows.Visibility.Hidden
                        Me.cboViolation3.Visibility = Windows.Visibility.Hidden
                        Me.cboViolation4.Visibility = Windows.Visibility.Hidden
                        Me.cboViolation5.Visibility = Windows.Visibility.Hidden
                        Me.cmdClear.Visibility = Windows.Visibility.Hidden
                        Me.cmdSave.Visibility = Windows.Visibility.Hidden
                        Me.cmdCancel.Content = "Close"
                    Case 2
                        Me.lblViolation2.Content = objTEPSwipeScanViolation.Statute & " : " & objTEPSwipeScanViolation.Offense
                    Case 3
                        Me.lblViolation3.Content = objTEPSwipeScanViolation.Statute & " : " & objTEPSwipeScanViolation.Offense
                    Case 4
                        Me.lblViolation4.Content = objTEPSwipeScanViolation.Statute & " : " & objTEPSwipeScanViolation.Offense
                    Case 5
                        Me.lblViolation5.Content = objTEPSwipeScanViolation.Statute & " : " & objTEPSwipeScanViolation.Offense

                End Select
           
            Next

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub WinViolations_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        Try

            Me.cboViolation2.IsEnabled = False
            Me.cboViolation3.IsEnabled = False
            Me.cboViolation4.IsEnabled = False
            Me.cboViolation5.IsEnabled = False

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub cboViolation1_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles cboViolation1.SelectionChanged

        Dim FilteredList As List(Of TEPViolations)

        Try
            Me.cboViolation2.ItemsSource = Nothing
            Me.cboViolation3.ItemsSource = Nothing
            Me.cboViolation4.ItemsSource = Nothing
            Me.cboViolation5.ItemsSource = Nothing

            Me.cboViolation2.IsEnabled = True
            Me.cboViolation3.IsEnabled = False
            Me.cboViolation4.IsEnabled = False
            Me.cboViolation5.IsEnabled = False

            intViolation1 = Me.cboViolation1.SelectedValue

            FilteredList = vList
            FilteredList = FilteredList.FindAll(Function(s As TEPViolations) s.ViolationID <> intViolation1)

            Me.cboViolation2.ItemsSource = FilteredList
            Me.cboViolation2.DisplayMemberPath = ("StatuteOffense")
            Me.cboViolation2.SelectedValuePath = ("ViolationID")

            Me.cmdSave.IsEnabled = True
            Me.cmdClear.IsEnabled = True

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try
    End Sub

    Private Sub cboViolation2_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles cboViolation2.SelectionChanged

        Dim FilteredList As List(Of TEPViolations)

        Try
            Me.cboViolation3.ItemsSource = Nothing
            Me.cboViolation4.ItemsSource = Nothing
            Me.cboViolation5.ItemsSource = Nothing

            Me.cboViolation3.IsEnabled = True
            Me.cboViolation4.IsEnabled = False
            Me.cboViolation5.IsEnabled = False

            intViolation2 = Me.cboViolation2.SelectedValue

            FilteredList = vList
            FilteredList = FilteredList.FindAll(Function(s As TEPViolations) s.ViolationID <> intViolation1)
            FilteredList = FilteredList.FindAll(Function(s As TEPViolations) s.ViolationID <> intViolation2)

            Me.cboViolation3.ItemsSource = FilteredList
            Me.cboViolation3.DisplayMemberPath = ("StatuteOffense")
            Me.cboViolation3.SelectedValuePath = ("ViolationID")

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub cboViolation3_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles cboViolation3.SelectionChanged

        Dim FilteredList As List(Of TEPViolations)

        Try
            Me.cboViolation4.ItemsSource = Nothing
            Me.cboViolation5.ItemsSource = Nothing

            Me.cboViolation4.IsEnabled = True
            Me.cboViolation5.IsEnabled = False

            intViolation3 = Me.cboViolation3.SelectedValue

            FilteredList = vList
            FilteredList = FilteredList.FindAll(Function(s As TEPViolations) s.ViolationID <> intViolation1)
            FilteredList = FilteredList.FindAll(Function(s As TEPViolations) s.ViolationID <> intViolation2)
            FilteredList = FilteredList.FindAll(Function(s As TEPViolations) s.ViolationID <> intViolation3)

            Me.cboViolation4.ItemsSource = FilteredList
            Me.cboViolation4.DisplayMemberPath = ("StatuteOffense")
            Me.cboViolation4.SelectedValuePath = ("ViolationID")

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub cboViolation4_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles cboViolation4.SelectionChanged

        Dim FilteredList As List(Of TEPViolations)

        Try
            Me.cboViolation5.ItemsSource = Nothing
            Me.cboViolation5.IsEnabled = True

            intViolation4 = Me.cboViolation4.SelectedValue

            FilteredList = vList
            FilteredList = FilteredList.FindAll(Function(s As TEPViolations) s.ViolationID <> intViolation1)
            FilteredList = FilteredList.FindAll(Function(s As TEPViolations) s.ViolationID <> intViolation2)
            FilteredList = FilteredList.FindAll(Function(s As TEPViolations) s.ViolationID <> intViolation3)
            FilteredList = FilteredList.FindAll(Function(s As TEPViolations) s.ViolationID <> intViolation4)

            Me.cboViolation5.ItemsSource = FilteredList
            Me.cboViolation5.DisplayMemberPath = ("StatuteOffense")
            Me.cboViolation5.SelectedValuePath = ("ViolationID")

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub cmdCancel_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdCancel.Click

        Try
            Me.DialogResult = False
            Me.Close()

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub cmdClear_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdClear.Click

        Try
            Me.cboViolation1.ItemsSource = Nothing
            Me.cboViolation1.ItemsSource = vList
            Me.cboViolation1.DisplayMemberPath = ("StatuteOffense")
            Me.cboViolation1.SelectedValuePath = ("ViolationID")

            Me.cboViolation2.ItemsSource = Nothing
            Me.cboViolation3.ItemsSource = Nothing
            Me.cboViolation4.ItemsSource = Nothing
            Me.cboViolation5.ItemsSource = Nothing

            Me.cboViolation2.IsEnabled = False
            Me.cboViolation3.IsEnabled = False
            Me.cboViolation4.IsEnabled = False
            Me.cboViolation5.IsEnabled = False

            Me.cmdSave.IsEnabled = False
            Me.cmdClear.IsEnabled = False

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Private Sub cmdSave_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdSave.Click

        Dim objcboViolation_SelectedItem As TEPViolations
        Dim objTEPSwipeScanViolations As TEPSwipeScanViolations

        Try
            If intViolation1 > 0 Then
                objTEPSwipeScanViolations = New TEPSwipeScanViolations
                objcboViolation_SelectedItem = Me.cboViolation1.SelectedItem
                objTEPSwipeScanViolations.SwipeScanID = intSwipeScanID
                objTEPSwipeScanViolations.Sequence = 1
                objTEPSwipeScanViolations.Statute = objcboViolation_SelectedItem.Statute
                objTEPSwipeScanViolations.Offense = objcboViolation_SelectedItem.Offense
                DataAccess.NewDataSwipeScan_Violation(objTEPSwipeScanViolations)
            End If

            If intViolation2 > 0 Then
                objTEPSwipeScanViolations = New TEPSwipeScanViolations
                objcboViolation_SelectedItem = Me.cboViolation2.SelectedItem
                objTEPSwipeScanViolations.SwipeScanID = intSwipeScanID
                objTEPSwipeScanViolations.Sequence = 2
                objTEPSwipeScanViolations.Statute = objcboViolation_SelectedItem.Statute
                objTEPSwipeScanViolations.Offense = objcboViolation_SelectedItem.Offense
                DataAccess.NewDataSwipeScan_Violation(objTEPSwipeScanViolations)
            End If

            If intViolation3 > 0 Then
                objTEPSwipeScanViolations = New TEPSwipeScanViolations
                objcboViolation_SelectedItem = Me.cboViolation3.SelectedItem
                objTEPSwipeScanViolations.SwipeScanID = intSwipeScanID
                objTEPSwipeScanViolations.Sequence = 3
                objTEPSwipeScanViolations.Statute = objcboViolation_SelectedItem.Statute
                objTEPSwipeScanViolations.Offense = objcboViolation_SelectedItem.Offense
                DataAccess.NewDataSwipeScan_Violation(objTEPSwipeScanViolations)
            End If

            If intViolation4 > 0 Then
                objTEPSwipeScanViolations = New TEPSwipeScanViolations
                objcboViolation_SelectedItem = Me.cboViolation4.SelectedItem
                objTEPSwipeScanViolations.SwipeScanID = intSwipeScanID
                objTEPSwipeScanViolations.Sequence = 4
                objTEPSwipeScanViolations.Statute = objcboViolation_SelectedItem.Statute
                objTEPSwipeScanViolations.Offense = objcboViolation_SelectedItem.Offense
                DataAccess.NewDataSwipeScan_Violation(objTEPSwipeScanViolations)
            End If

            If intViolation5 > 0 Then
                objTEPSwipeScanViolations = New TEPSwipeScanViolations
                objcboViolation_SelectedItem = Me.cboViolation5.SelectedItem
                objTEPSwipeScanViolations.SwipeScanID = intSwipeScanID
                objTEPSwipeScanViolations.Sequence = 5
                objTEPSwipeScanViolations.Statute = objcboViolation_SelectedItem.Statute
                objTEPSwipeScanViolations.Offense = objcboViolation_SelectedItem.Offense
                DataAccess.NewDataSwipeScan_Violation(objTEPSwipeScanViolations)
            End If

            Me.DialogResult = True
            Me.Close()

        Catch ex As Exception
            DataAccess.NewException(ex)
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

End Class
