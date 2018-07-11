Imports IVS.Data

Public Class winVisiting

   Private objVisitingDetail As Visiting
    Private isContentChanged As Boolean
    Private vList As List(Of Clients)

#Region "Control Bound Subs"

    Public Sub New(ByVal VisitingDetail As Visiting)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner

        Try
            objVisitingDetail = VisitingDetail
            vList = DataAccess.GetStations

            Me.cboStations.ItemsSource = vList
            Me.cboStations.DisplayMemberPath = ("Station")
            Me.cboStations.SelectedValuePath = ("ClientID")

            Me.cboLocations.ItemsSource = DataAccess.GetLocations
            Me.cboLocations.DisplayMemberPath = ("LocationDescription")
            Me.cboLocations.SelectedValuePath = ("LocationID")

            Me.cbLocationGlobal.IsChecked = True
            Me.cboLocations.IsEnabled = False
            Me.cboStations.IsEnabled = False
            Me.cbClientGlobal.IsChecked = True
            Me.cbIsActive.IsChecked = True

            If objVisitingDetail.VisitingID > 0 Then

                Me.txtVisitingName.Text = objVisitingDetail.VisitingName
                Me.cbIsActive.IsChecked = objVisitingDetail.ActiveFlag

                If objVisitingDetail.LocationID = 0 Then
                    'global
                    Me.cbLocationGlobal.IsChecked = True
                    Me.cboLocations.IsEnabled = False

                    Me.cbClientGlobal.IsChecked = False
                    Me.cbClientGlobal.IsEnabled = False

                    Me.cboStations.IsEnabled = False

                Else

                    Me.cbLocationGlobal.IsChecked = False
                    Me.cboLocations.IsEnabled = True
                    'set locationid

                    Me.cboLocations.SelectedValue = objVisitingDetail.LocationID

                    If objVisitingDetail.ClientID = 0 Then

                        Me.cbClientGlobal.IsChecked = True
                        Me.cboStations.IsEnabled = False

                    Else

                        Me.cbClientGlobal.IsChecked = False
                        Me.cbClientGlobal.IsEnabled = True

                        'set clientid 
                        Dim FilteredList As List(Of Clients)

                        FilteredList = vList
                        FilteredList = FilteredList.FindAll(Function(s As Clients) s.Location = objVisitingDetail.LocationID)

                        Me.cboStations.ItemsSource = FilteredList
                        Me.cboStations.DisplayMemberPath = ("Station")
                        Me.cboStations.SelectedValuePath = ("ClientID")

                        Me.cboStations.SelectedValue = objVisitingDetail.ClientID

                    End If
                End If

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub winVisiting_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        Try
            AddHandler cbIsActive.Click, AddressOf TextChanged
            AddHandler txtVisitingName.TextChanged, AddressOf TextChanged
            AddHandler cbLocationGlobal.Checked, AddressOf TextChanged
            AddHandler cbClientGlobal.Checked, AddressOf TextChanged
            AddHandler cboLocations.SelectionChanged, AddressOf TextChanged
            AddHandler cboStations.SelectionChanged, AddressOf TextChanged

            Me.cmdSave.IsEnabled = False

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdDelete_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdDelete.Click

        Try

            If MessageBox.Show("Are you sure you want to delete this Visiting Name?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

                DataAccess.DeleteVisiting(objVisitingDetail.VisitingID)

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdSave_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdSave.Click

        Dim objNewVisitingDetail As New Visiting
        Dim objLocationSelected As Locations
        Dim objStationSelected As Clients

        Try
            objNewVisitingDetail.VisitingName = Me.txtVisitingName.Text
            objNewVisitingDetail.ActiveFlag = Me.cbIsActive.IsChecked

            If cbLocationGlobal.IsChecked = True Then
                objNewVisitingDetail.LocationID = 0
                objNewVisitingDetail.ClientID = 0
            Else
                objLocationSelected = Me.cboLocations.SelectedItem

                objNewVisitingDetail.LocationID = objLocationSelected.LocationID

                If cbClientGlobal.IsChecked = True Then
                    objNewVisitingDetail.ClientID = 0
                Else
                    objStationSelected = Me.cboStations.SelectedItem
                    objNewVisitingDetail.ClientID = objStationSelected.ClientID
                End If
            End If

            If objVisitingDetail.VisitingID = 0 Then

                DataAccess.NewVisiting(objNewVisitingDetail)

            Else

                objNewVisitingDetail.VisitingID = objVisitingDetail.VisitingID

                DataAccess.UpdateVisiting(objNewVisitingDetail)

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        Me.DialogResult = True
        Me.Close()

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
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cbLocationGlobal_Checked(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cbLocationGlobal.Checked

        Try
            Me.cbClientGlobal.IsChecked = True
            Me.cboLocations.IsEnabled = False
            Me.cboStations.IsEnabled = False
            Me.cbClientGlobal.IsEnabled = False

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cbLocationGlobal_Unchecked(sender As Object, e As System.Windows.RoutedEventArgs) Handles cbLocationGlobal.Unchecked

        Dim FilteredList As List(Of Clients)

        Try
            Me.cboLocations.IsEnabled = True
            Me.cbClientGlobal.IsEnabled = True

            If objVisitingDetail.ClientID = 0 Then

                Me.cbClientGlobal.IsChecked = True
                Me.cboStations.IsEnabled = False

            Else

                Me.cbClientGlobal.IsChecked = False
                Me.cbClientGlobal.IsEnabled = True

                'set clientid       
                FilteredList = vList
                FilteredList = FilteredList.FindAll(Function(s As Clients) s.Location = Me.cboLocations.SelectedValue)

                Me.cboStations.ItemsSource = FilteredList
                Me.cboStations.DisplayMemberPath = ("Station")
                Me.cboStations.SelectedValuePath = ("ClientID")

                Me.cboStations.SelectedValue = objVisitingDetail.ClientID

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cbClientGlobal_Checked(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cbClientGlobal.Checked

        Try
            Me.cboStations.IsEnabled = False

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cbClientGlobal_Unchecked(sender As Object, e As System.Windows.RoutedEventArgs) Handles cbClientGlobal.Unchecked

        Dim FilteredList As List(Of Clients)

        Try
            Me.cboStations.IsEnabled = True

            FilteredList = vList
            FilteredList = FilteredList.FindAll(Function(s As Clients) s.Location = Me.cboLocations.SelectedValue)

            Me.cboStations.ItemsSource = FilteredList
            Me.cboStations.DisplayMemberPath = ("Station")
            Me.cboStations.SelectedValuePath = ("ClientID")

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cboLocations_SelectionChanged(sender As Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles cboLocations.SelectionChanged

        Dim FilteredList As List(Of Clients)

        Try
            FilteredList = vList
            FilteredList = FilteredList.FindAll(Function(s As Clients) s.Location = Me.cboLocations.SelectedValue)

            Me.cboStations.ItemsSource = FilteredList
            Me.cboStations.DisplayMemberPath = ("Station")
            Me.cboStations.SelectedValuePath = ("ClientID")

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

#Region "Custom Subs/Functions"

    Private Sub TextChanged()

        Try
            isContentChanged = True

            If Me.txtVisitingName.Text <> "" Then

                Me.cmdSave.IsEnabled = True

            Else
                Me.cmdSave.IsEnabled = False
            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

#End Region

End Class
