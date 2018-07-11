'Imports IVS.Data
'Imports IVS.AppLog
Imports IVS.Data.WS.TEP.IVSService
Imports IVS.Data.WS.TEP

Public Class WinManEntry

    'Private MyAppLog As New ApplicationLog(ApplicationLog.DateEncodingType.YYYYMMDD, True)
    'Private objClientSettings As New ClientSettings
    'rivate intIVSUserID As Integer
    Private MyCallingWindow As String
    Private isContentChanged As Boolean = False

#Region "Control Bound Subs"

    Public Sub New(ByVal CallingWindow As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Try
            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner

            'objClientSettings = DataAccess.GetClientSettings(ClientID)

            'intIVSUserID = IVSUserID

            MyCallingWindow = CallingWindow

            Me.cboIDType.Items.Add("-- Select an ID Type --")
            Me.cboIDType.Items.Add("Drivers License Or State ID")


            If WinMain.objClientSettings.DisableCCSave = False Then
                Me.cboIDType.Items.Add("Credit Card")
            End If

            Me.cboIDType.Items.Add("Military ID Card")
            Me.cboIDType.Items.Add("INS Employee Authorization Card")

            If WinMain.objClientSettings.DeviceType = "CTS" Then
                Me.cboIDType.Items.Add("Check")
            End If

            Me.cboIDType.SelectedIndex = 0

            If WinMain.objClientSettings.DisableCCSave = False Then
                Me.cboCCIssuer.Items.Add("-- Select a Credit Card Type --")
                Me.cboCCIssuer.Items.Add("American Express")
                Me.cboCCIssuer.Items.Add("Discover")
                Me.cboCCIssuer.Items.Add("Mastercard")
                Me.cboCCIssuer.Items.Add("Visa")
                Me.cboCCIssuer.Items.Add("Other")
                Me.cboCCIssuer.SelectedIndex = 0
            End If

            Me.cboCCIssuer.IsEnabled = False

        Catch ex As Exception
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub WinManEntry_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        Try

            AddHandler Me.txtCheckNumber.TextChanged, AddressOf TextChanged
            AddHandler Me.txtIDNumber.TextChanged, AddressOf TextChanged
            AddHandler Me.txtNameFirst.TextChanged, AddressOf TextChanged
            AddHandler Me.txtNameLast.TextChanged, AddressOf TextChanged
            AddHandler Me.txtNameMiddle.TextChanged, AddressOf TextChanged
            AddHandler Me.txtDateOfBirth.TextChanged, AddressOf TextChanged
            AddHandler Me.txtSex.TextChanged, AddressOf TextChanged
            AddHandler Me.txtHeight.TextChanged, AddressOf TextChanged
            AddHandler Me.txtWeight.TextChanged, AddressOf TextChanged
            AddHandler Me.txtEyes.TextChanged, AddressOf TextChanged
            AddHandler Me.txtHair.TextChanged, AddressOf TextChanged
            AddHandler Me.txtDateOfIssue.TextChanged, AddressOf TextChanged
            AddHandler Me.txtDateOfExpiration.TextChanged, AddressOf TextChanged
            AddHandler Me.txtAddressStreet.TextChanged, AddressOf TextChanged
            AddHandler Me.txtAddressCity.TextChanged, AddressOf TextChanged
            AddHandler Me.txtAddressState.TextChanged, AddressOf TextChanged
            AddHandler Me.txtAddressZip.TextChanged, AddressOf TextChanged

            Me.cmdSave.IsEnabled = False

        Catch ex As Exception
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cboIDType_SelectionChanged(sender As Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles cboIDType.SelectionChanged
        Dim strIDType As String

        Try
            strIDType = Me.cboIDType.SelectedItem

            Select Case strIDType

                Case "Drivers License Or State ID"

                    Me.txtNameFirst.IsEnabled = True
                    Me.txtNameLast.IsEnabled = True
                    Me.txtNameMiddle.IsEnabled = True
                    Me.txtAddressStreet.IsEnabled = True
                    Me.txtAddressCity.IsEnabled = True
                    Me.txtAddressState.IsEnabled = True
                    Me.txtAddressZip.IsEnabled = True
                    Me.txtDateOfBirth.IsEnabled = True
                    Me.txtSex.IsEnabled = True
                    Me.txtHeight.IsEnabled = True
                    Me.txtWeight.IsEnabled = True
                    Me.txtHair.IsEnabled = True
                    Me.txtEyes.IsEnabled = True
                    Me.txtDateOfExpiration.IsEnabled = True
                    Me.txtDateOfIssue.IsEnabled = True
                    Me.txtCheckNumber.IsEnabled = False
                    Me.txtCheckNumber.Text = Nothing
                    Me.cboCCIssuer.IsEnabled = False
                    Me.imgCCIssuerRequired.Visibility = Windows.Visibility.Hidden
                    Me.imgNameFirstRequired.Visibility = Windows.Visibility.Visible
                    Me.imgNameLastRequired.Visibility = Windows.Visibility.Visible
                    Me.cboCCIssuer.SelectedIndex = 0

                Case "Credit Card"

                    Me.txtNameFirst.IsEnabled = True
                    Me.txtNameLast.IsEnabled = True
                    Me.txtNameMiddle.IsEnabled = True
                    Me.txtAddressStreet.IsEnabled = False
                    Me.txtAddressStreet.Text = Nothing
                    Me.txtAddressCity.IsEnabled = False
                    Me.txtAddressCity.Text = Nothing
                    Me.txtAddressState.IsEnabled = False
                    Me.txtAddressState.Text = Nothing
                    Me.txtAddressZip.IsEnabled = False
                    Me.txtAddressZip.Text = Nothing
                    Me.txtDateOfBirth.IsEnabled = False
                    Me.txtDateOfBirth.Text = Nothing
                    Me.txtSex.IsEnabled = False
                    Me.txtSex.Text = Nothing
                    Me.txtHeight.IsEnabled = False
                    Me.txtHeight.Text = Nothing
                    Me.txtWeight.IsEnabled = False
                    Me.txtHair.IsEnabled = False
                    Me.txtHair.Text = Nothing
                    Me.txtEyes.IsEnabled = False
                    Me.txtEyes.Text = Nothing
                    Me.txtDateOfExpiration.IsEnabled = True
                    Me.txtDateOfIssue.IsEnabled = False
                    Me.txtDateOfIssue.Text = Nothing
                    Me.txtCheckNumber.IsEnabled = False
                    Me.txtCheckNumber.Text = Nothing
                    Me.cboCCIssuer.IsEnabled = True
                    Me.imgCCIssuerRequired.Visibility = Windows.Visibility.Visible
                    Me.imgNameFirstRequired.Visibility = Windows.Visibility.Visible
                    Me.imgNameLastRequired.Visibility = Windows.Visibility.Visible
                    Me.cboCCIssuer.SelectedIndex = 0

                Case "Military ID Card"

                    Me.txtNameFirst.IsEnabled = True
                    Me.txtNameLast.IsEnabled = True
                    Me.txtNameMiddle.IsEnabled = True
                    Me.txtAddressStreet.IsEnabled = False
                    Me.txtAddressStreet.Text = Nothing
                    Me.txtAddressCity.IsEnabled = False
                    Me.txtAddressCity.Text = Nothing
                    Me.txtAddressState.IsEnabled = False
                    Me.txtAddressState.Text = Nothing
                    Me.txtAddressZip.IsEnabled = False
                    Me.txtAddressZip.Text = Nothing
                    Me.txtDateOfBirth.IsEnabled = True
                    Me.txtSex.IsEnabled = False
                    Me.txtSex.Text = Nothing
                    Me.txtHeight.IsEnabled = True
                    Me.txtWeight.IsEnabled = True
                    Me.txtHair.IsEnabled = True
                    Me.txtEyes.IsEnabled = True
                    Me.txtDateOfExpiration.IsEnabled = True
                    Me.txtDateOfIssue.IsEnabled = True
                    Me.txtCheckNumber.IsEnabled = False
                    Me.txtCheckNumber.Text = Nothing
                    Me.cboCCIssuer.IsEnabled = False
                    Me.imgCCIssuerRequired.Visibility = Windows.Visibility.Hidden
                    Me.imgNameFirstRequired.Visibility = Windows.Visibility.Visible
                    Me.imgNameLastRequired.Visibility = Windows.Visibility.Visible
                    Me.cboCCIssuer.SelectedIndex = 0

                Case "INS Employee Authorization Card"

                    Me.txtNameFirst.IsEnabled = True
                    Me.txtNameLast.IsEnabled = True
                    Me.txtNameMiddle.IsEnabled = True
                    Me.txtAddressStreet.Text = Nothing
                    Me.txtAddressCity.IsEnabled = False
                    Me.txtAddressCity.Text = Nothing
                    Me.txtAddressState.IsEnabled = False
                    Me.txtAddressState.Text = Nothing
                    Me.txtAddressZip.IsEnabled = False
                    Me.txtAddressZip.Text = Nothing
                    Me.txtDateOfBirth.IsEnabled = True
                    Me.txtSex.IsEnabled = True
                    Me.txtHeight.IsEnabled = False
                    Me.txtHeight.Text = Nothing
                    Me.txtWeight.IsEnabled = False
                    Me.txtWeight.Text = Nothing
                    Me.txtHair.IsEnabled = False
                    Me.txtHair.Text = Nothing
                    Me.txtEyes.IsEnabled = False
                    Me.txtEyes.Text = Nothing
                    Me.txtDateOfExpiration.IsEnabled = True
                    Me.txtDateOfIssue.IsEnabled = True
                    Me.txtCheckNumber.IsEnabled = False
                    Me.txtCheckNumber.Text = Nothing
                    Me.cboCCIssuer.IsEnabled = False
                    Me.imgCCIssuerRequired.Visibility = Windows.Visibility.Hidden
                    Me.imgNameFirstRequired.Visibility = Windows.Visibility.Visible
                    Me.imgNameLastRequired.Visibility = Windows.Visibility.Visible
                    Me.cboCCIssuer.SelectedIndex = 0

                Case "Check"

                    Me.txtNameFirst.IsEnabled = False
                    Me.txtNameLast.IsEnabled = False
                    Me.txtNameMiddle.IsEnabled = False
                    Me.txtAddressStreet.IsEnabled = False
                    Me.txtAddressStreet.Text = Nothing
                    Me.txtAddressCity.IsEnabled = False
                    Me.txtAddressCity.Text = Nothing
                    Me.txtAddressState.IsEnabled = False
                    Me.txtAddressState.Text = Nothing
                    Me.txtAddressZip.IsEnabled = False
                    Me.txtAddressZip.Text = Nothing
                    Me.txtDateOfBirth.IsEnabled = False
                    Me.txtDateOfBirth.Text = Nothing
                    Me.txtSex.IsEnabled = False
                    Me.txtSex.Text = Nothing
                    Me.txtHeight.IsEnabled = False
                    Me.txtHeight.Text = Nothing
                    Me.txtWeight.IsEnabled = False
                    Me.txtWeight.Text = Nothing
                    Me.txtHair.IsEnabled = False
                    Me.txtHair.Text = Nothing
                    Me.txtEyes.IsEnabled = False
                    Me.txtEyes.Text = Nothing
                    Me.txtDateOfExpiration.IsEnabled = False
                    Me.txtDateOfExpiration.Text = Nothing
                    Me.txtDateOfIssue.IsEnabled = False
                    Me.txtDateOfIssue.Text = Nothing
                    Me.txtCheckNumber.IsEnabled = True
                    Me.cboCCIssuer.IsEnabled = False
                    Me.imgCCIssuerRequired.Visibility = Windows.Visibility.Hidden
                    Me.imgNameFirstRequired.Visibility = Windows.Visibility.Hidden
                    Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                    Me.cboCCIssuer.SelectedIndex = 0

            End Select

        Catch ex As Exception
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdSave_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdSave.Click

        Dim objSwipeScanDetail As New SwipeScanDetail
        Dim strCCIssuer As String

        Try
            If cboIDType.SelectedIndex > 0 Then

                If cboCCIssuer.SelectedIndex > 0 Then

                    Select Case cboCCIssuer.SelectedItem

                        Case "American Express"
                            strCCIssuer = "AX"
                        Case "Discover"
                            strCCIssuer = "DI"
                        Case "Mastercard"
                            strCCIssuer = "MC"
                        Case "Visa"
                            strCCIssuer = "VI"
                        Case "Other"
                            strCCIssuer = "O"

                    End Select

                End If

                objSwipeScanDetail.CardType = Me.cboIDType.SelectedItem

                If Me.cboIDType.SelectedItem <> "Check" Then
                    objSwipeScanDetail.CheckNumber = 0
                Else
                    objSwipeScanDetail.CheckNumber = Me.txtCheckNumber.Text
                End If

                objSwipeScanDetail.IDAccountNumber = Me.txtIDNumber.Text
                objSwipeScanDetail.NameFirst = Me.txtNameFirst.Text
                objSwipeScanDetail.NameLast = Me.txtNameLast.Text
                objSwipeScanDetail.NameMiddle = Me.txtNameMiddle.Text
                objSwipeScanDetail.DateOfBirth = Me.txtDateOfBirth.Text
                objSwipeScanDetail.Age = 0
                objSwipeScanDetail.Sex = Me.txtSex.Text
                objSwipeScanDetail.Height = Me.txtHeight.Text
                objSwipeScanDetail.Weight = Me.txtWeight.Text
                objSwipeScanDetail.Eyes = Me.txtEyes.Text
                objSwipeScanDetail.Hair = Me.txtHair.Text
                objSwipeScanDetail.DateOfIssue = Me.txtDateOfIssue.Text
                objSwipeScanDetail.DateOfExpiration = Me.txtDateOfExpiration.Text
                objSwipeScanDetail.AddressStreet = Me.txtAddressStreet.Text
                objSwipeScanDetail.AddressCity = Me.txtAddressCity.Text
                objSwipeScanDetail.AddressState = Me.txtAddressState.Text
                objSwipeScanDetail.AddressZip = Me.txtAddressZip.Text
                objSwipeScanDetail.SwipeRawData = "Manual Entry"
                objSwipeScanDetail.UserID = WinMain.intIVSUserID
                objSwipeScanDetail.ClientID = WinMain.objClientSettings.ClientID
                objSwipeScanDetail.CCIssuer = strCCIssuer

                If MyCallingWindow = "CTS" Then
                    WinScanImage.ManualEntrySwipeScanID = DataAccess.NewDataSwipeScanManual(objSwipeScanDetail)
                    Me.DialogResult = True
                    Me.Close()

                Else
                    DataAccess.NewDataSwipeScanManual(objSwipeScanDetail)
                    Me.DialogResult = False
                    Me.Close()
                End If

            End If

        Catch ex As Exception
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub cmdClose_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdClose.Click
        Try
            Me.Close()
        Catch ex As Exception
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try
    End Sub

#End Region

    Private Sub TextChanged()

        Try
            isContentChanged = True

            If Me.cboIDType.SelectedItem = "Check" Then

                If Me.txtIDNumber.Text.Length > 2 And Me.txtIDNumber.Text.Contains(":") And Me.txtCheckNumber.Text <> "" Then

                    Me.cmdSave.IsEnabled = True

                Else
                    Me.cmdSave.IsEnabled = False
                End If

            Else

                If Me.txtIDNumber.Text <> "" And Me.txtNameFirst.Text <> "" And Me.txtNameLast.Text <> "" Then

                    Me.cmdSave.IsEnabled = True

                Else
                    Me.cmdSave.IsEnabled = False
                End If

            End If

        Catch ex As Exception
            DataAccess.NewException(ex)
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

End Class