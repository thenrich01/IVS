Imports System.ServiceModel.Security
Imports System.Text
'Imports Test_WebService.IVSService
Imports IVS.AppLog
Imports IVS.Data.IVSService
Imports IVS.Data

Class MainWindow

    Private MyAppLog As New ApplicationLog(ApplicationLog.DateEncodingType.YYYYMMDD, True)

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
        Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
        Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
        Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
        Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
        Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
        Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
        Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
        Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
        Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
        Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
        Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

        Me.cboFunctions.Items.Add("- Select a Function -")
        Me.cboFunctions.Items.Add("NewClient")
        Me.cboFunctions.Items.Add("GetClientID")
        Me.cboFunctions.Items.Add("GetClients")
        Me.cboFunctions.Items.Add("GetLocations")
        Me.cboFunctions.Items.Add("UpdateClientIPAddress")
        Me.cboFunctions.Items.Add("GetClientSettings")
        Me.cboFunctions.Items.Add("SaveClientSettings")
        Me.cboFunctions.Items.Add("UpdateIDecodeLicense")
        Me.cboFunctions.Items.Add("GetStationName")
        Me.cboFunctions.Items.Add("GetDeviceInfo")
        Me.cboFunctions.Items.Add("GetUsers")
        Me.cboFunctions.Items.Add("GetUserNames")
        Me.cboFunctions.Items.Add("GetUserName")
        Me.cboFunctions.Items.Add("GetUserPhone")
        Me.cboFunctions.Items.Add("GetUserDetail")
        Me.cboFunctions.Items.Add("NewUser")
        Me.cboFunctions.Items.Add("UpdateUser")
        Me.cboFunctions.Items.Add("DeleteUser")
        Me.cboFunctions.Items.Add("EnableUser")
        Me.cboFunctions.Items.Add("IsUserNameAvailable")
        Me.cboFunctions.Items.Add("IsUserAuthenticated")
        Me.cboFunctions.Items.Add("GetAlerts")
        Me.cboFunctions.Items.Add("GetAlertDetail")
        Me.cboFunctions.Items.Add("GetSwipeScanAlerts")
        Me.cboFunctions.Items.Add("NewAlert")
        Me.cboFunctions.Items.Add("UpdateAlert")
        Me.cboFunctions.Items.Add("DeleteAlert")
        Me.cboFunctions.Items.Add("EnableAlert")
        Me.cboFunctions.Items.Add("UpdateImageAvailable")
        Me.cboFunctions.Items.Add("NewDataSwipeScan")
        Me.cboFunctions.Items.Add("NewDataSwipeScanManual")
        Me.cboFunctions.Items.Add("GetSwipeScanDetail")
        Me.cboFunctions.Items.Add("GetSwipeScanHistory")
        Me.cboFunctions.Items.Add("GetSwipeScanType")
        Me.cboFunctions.Items.Add("SwipeScanNavigateFirst")
        Me.cboFunctions.Items.Add("SwipeScanNavigatePrevious")
        Me.cboFunctions.Items.Add("SwipeScanNavigatePosition")
        Me.cboFunctions.Items.Add("SwipeScanNavigateNext")
        Me.cboFunctions.Items.Add("SwipeScanNavigateLast")
        Me.cboFunctions.Items.Add("DeleteSwipeScan")
        Me.cboFunctions.SelectedIndex = 0

        Me.dgLists.Visibility = Windows.Visibility.Hidden
        'Me.txtResults.Visibility = Windows.Visibility.Hidden

        Me.cboSwipeScanType.Items.Add("- Swipe Scan Type -")
        Me.cboSwipeScanType.Items.Add("Credit Card")
        Me.cboSwipeScanType.Items.Add("Check")
        Me.cboSwipeScanType.Items.Add("Drivers License Or State ID")
        Me.cboSwipeScanType.Items.Add("INS Employee Authorization Card")
        Me.cboSwipeScanType.Items.Add("Military ID Card")
        Me.cboSwipeScanType.SelectedIndex = 0

        Me.cbSSL.IsChecked = True
        Me.txtIPAddress.Text = IVS.Data.DataAccess.ReturnMyIPAddress
        Me.txtHostName.Text = IVS.Data.DataAccess.ReturnHostName(IVS.Data.DataAccess.ReturnMyIPAddress)


    End Sub

    Private Sub cmdExecuteFunction_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmdExecuteFunction.Click

        Dim timeStart As Date
        Dim timeEnd As Date
        Dim strHostname As String = Nothing
        Dim strIPAddress As String = Nothing
        Dim intClientID As Integer = 0
        Dim strImagePath As String = Nothing
        Dim intUserID As Integer = 0
        Dim isActive As Boolean
        Dim strUserName As String = Nothing
        Dim strPassword As String = Nothing
        Dim intAlertID As Integer = 0
        Dim strNameLast As String = Nothing
        Dim strIDNumber As String = Nothing
        Dim intSwipeScanID As Integer = 0
        Dim strSwipeScanType As String = Nothing
        Dim strEndpoint As String = Nothing
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            timeStart = Now

            Me.lblExecuteFunctionStatus.Content = "Executing.."

            Select Case cbSSL.IsChecked

                Case True
                    strEndpoint = "SSL"
                Case False
                    strEndpoint = "Dev"
            End Select

            Dim objIVSWebClient As IVSClient = New IVSClient(strEndpoint)

            Select Case Me.cboFunctions.SelectedItem.ToString

                Case "NewClient"
                    strHostname = Me.txtHostName.Text
                    strIPAddress = Me.txtIPAddress.Text
                    strImagePath = GetImagePath()
                    If cbDAL.IsChecked Then
                        Me.txtClientID.Text = IVS.Data.DataAccess.NewClient()
                    Else
                        Me.txtClientID.Text = objIVSWebClient.NewClient(objIVSLicense.IVSGUID, strHostname, strIPAddress, strImagePath)
                    End If

                    Me.lblExecuteFunctionStatus.Content = "NewClient Completed"

                Case "GetClientID"
                    strHostname = Me.txtHostName.Text
                    Me.txtClientID.Text = objIVSWebClient.GetClientID(objIVSLicense.IVSGUID, strHostname)
                    Me.lblExecuteFunctionStatus.Content = "GetClientID Completed"

                Case "GetClients"

                    If cbDAL.IsChecked Then
                        Me.dgLists.ItemsSource = IVS.Data.DataAccess.GetClients
                    Else
                        Me.dgLists.ItemsSource = objIVSWebClient.GetClients(objIVSLicense.IVSGUID)
                    End If
                    Dim intCount As Integer = objIVSWebClient.GetClients(objIVSLicense.IVSGUID).Count

                    Me.dgLists.Visibility = Windows.Visibility.Visible
                    Me.txtResults.Visibility = Windows.Visibility.Hidden
                    Me.lblExecuteFunctionStatus.Content = "GetClients Completed:" & intCount

                Case "GetLocations"
                    Me.dgLists.ItemsSource = objIVSWebClient.GetLocations(objIVSLicense.IVSGUID)
                    Me.dgLists.Visibility = Windows.Visibility.Visible
                    Me.txtResults.Visibility = Windows.Visibility.Hidden
                    Me.lblExecuteFunctionStatus.Content = "GetLocations Completed"

                Case "UpdateClientIPAddress"
                    intClientID = Me.txtClientID.Text
                    strIPAddress = Me.txtIPAddress.Text
                    objIVSWebClient.UpdateClientIPAddress(objIVSLicense.IVSGUID, intClientID, strIPAddress)
                    Me.lblExecuteFunctionStatus.Content = "UpdateClientIPAddress Completed"

                Case "GetClientSettings"
                    Dim ObjClientSettings As New ClientSettings
                    intClientID = Me.txtClientID.Text
                    ObjClientSettings = objIVSWebClient.GetClientSettings(objIVSLicense.IVSGUID, intClientID)
                    'Me.dgLists.ItemsSource = GetListOfResults_ClientSettings(ObjClientSettings)

                    Me.txtResults.Text += "ClientID:" & ObjClientSettings.ClientID & vbCrLf
                    Me.txtResults.Text += "DeviceID:" & ObjClientSettings.DeviceID & vbCrLf
                    Me.txtResults.Visibility = Windows.Visibility.Visible

                    Me.dgLists.Visibility = Windows.Visibility.Visible
                    'Me.txtResults.Visibility = Windows.Visibility.Hidden
                    Me.lblExecuteFunctionStatus.Content = "GetClientSettings Completed"

                Case "SaveClientSettings"
                    Me.lblExecuteFunctionStatus.Content = "SaveClientSettings Completed"

                Case "UpdateIDecodeLicense"
                    Me.lblExecuteFunctionStatus.Content = "UpdateIDecodeLicense Completed"

                Case "GetStationName"
                    intClientID = Me.txtClientID.Text
                    Dim strStationName As String = objIVSWebClient.GetStationName(objIVSLicense.IVSGUID, intClientID)

                    Dim ListOfResults As New List(Of Results)
                    Dim objResults As New Results

                    objResults.ResultsItem = "StationName"
                    objResults.ResultsValue = strStationName
                    ListOfResults.Add(objResults)

                    Me.dgLists.ItemsSource = ListOfResults

                    Me.dgLists.Visibility = Windows.Visibility.Visible
                    Me.txtResults.Visibility = Windows.Visibility.Hidden
                    Me.lblExecuteFunctionStatus.Content = "GetStationName Completed"

                Case "GetDeviceInfo"
                    intClientID = Me.txtClientID.Text
                    Dim objDeviceInfo As New DeviceInfo
                    objDeviceInfo = objIVSWebClient.GetDeviceInfo(objIVSLicense.IVSGUID, intClientID)

                    Me.dgLists.ItemsSource = GetListOfResults_DeviceInfo(objDeviceInfo)
                    Me.dgLists.Visibility = Windows.Visibility.Visible
                    'Me.txtResults.Visibility = Windows.Visibility.Hidden
                    Me.lblExecuteFunctionStatus.Content = "GetDeviceInfo Completed"

                Case "GetUsers"
                    Me.dgLists.ItemsSource = objIVSWebClient.GetUsers(objIVSLicense.IVSGUID)
                    Me.dgLists.Visibility = Windows.Visibility.Visible
                    Me.txtResults.Visibility = Windows.Visibility.Hidden
                    Me.lblExecuteFunctionStatus.Content = "GetUsers Completed"

                Case "GetUserNames"
                    Me.dgLists.ItemsSource = objIVSWebClient.GetUserNames(objIVSLicense.IVSGUID)
                    Me.dgLists.Visibility = Windows.Visibility.Visible
                    Me.txtResults.Visibility = Windows.Visibility.Hidden
                    Me.lblExecuteFunctionStatus.Content = "GetUserNames Completed"

                Case "GetUserName"
                    intUserID = Me.txtUserID.Text
                    strUserName = objIVSWebClient.GetUserName(objIVSLicense.IVSGUID, intUserID)

                    Dim ListOfResults As New List(Of Results)
                    Dim objResults As New Results

                    objResults.ResultsItem = "UserName"
                    objResults.ResultsValue = strUserName
                    ListOfResults.Add(objResults)

                    Me.dgLists.ItemsSource = ListOfResults

                    Me.dgLists.Visibility = Windows.Visibility.Visible
                    Me.txtResults.Visibility = Windows.Visibility.Hidden
                    Me.lblExecuteFunctionStatus.Content = "GetUserName Completed"

                Case "GetUserPhone"
                    intUserID = Me.txtUserID.Text
                    Dim strUserPhone As String = objIVSWebClient.GetUserPhone(objIVSLicense.IVSGUID, intUserID)

                    Dim ListOfResults As New List(Of Results)
                    Dim objResults As New Results

                    objResults.ResultsItem = "UserPhone"
                    objResults.ResultsValue = strUserPhone
                    ListOfResults.Add(objResults)

                    Me.dgLists.ItemsSource = ListOfResults

                    Me.dgLists.Visibility = Windows.Visibility.Visible
                    Me.txtResults.Visibility = Windows.Visibility.Hidden
                    Me.lblExecuteFunctionStatus.Content = "GetUserPhone Completed"

                Case "GetUserDetail"
                    intUserID = Me.txtUserID.Text
                    Dim objUserDetail As New UserDetail
                    objUserDetail = objIVSWebClient.GetUserDetail(objIVSLicense.IVSGUID, intUserID)

                    Me.dgLists.ItemsSource = GetListOfResults_UserDetail(objUserDetail)

                    Me.dgLists.Visibility = Windows.Visibility.Visible
                    Me.txtResults.Visibility = Windows.Visibility.Hidden
                    Me.lblExecuteFunctionStatus.Content = "GetUserDetail Completed"

                Case "NewUser"
                    Me.lblExecuteFunctionStatus.Content = "NewUser Completed"

                Case "UpdateUser"
                    Me.lblExecuteFunctionStatus.Content = "UpdateUser Completed"

                Case "DeleteUser"
                    intUserID = Me.txtUserID.Text

                    If MessageBox.Show("Are you sure you want to delete UserID: " & intUserID & " ?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

                        objIVSWebClient.DeleteUser(objIVSLicense.IVSGUID, intUserID)

                    End If

                    Me.lblExecuteFunctionStatus.Content = "DeleteUser Completed"

                Case "EnableUser"
                    intUserID = Me.txtUserID.Text
                    isActive = Me.cbIsActive.IsChecked

                    objIVSWebClient.EnableUser(objIVSLicense.IVSGUID, intUserID, isActive)
                    Me.lblExecuteFunctionStatus.Content = "EnableUser Completed"

                Case "IsUserNameAvailable"
                    strUserName = Me.txtUserName.Text
                    Dim isUserNameAvailable As Boolean = objIVSWebClient.IsUserNameAvailable(objIVSLicense.IVSGUID, strUserName)

                    Dim ListOfResults As New List(Of Results)
                    Dim objResults As New Results

                    objResults.ResultsItem = "isUserNameAvailable"
                    objResults.ResultsValue = isUserNameAvailable
                    ListOfResults.Add(objResults)

                    Me.dgLists.ItemsSource = ListOfResults

                    Me.dgLists.Visibility = Windows.Visibility.Visible
                    Me.txtResults.Visibility = Windows.Visibility.Hidden
                    Me.lblExecuteFunctionStatus.Content = "IsUserNameAvailable Completed"

                Case "IsUserAuthenticated"
                    strUserName = Me.txtUserName.Text
                    strPassword = Me.txtPassword.Text

                    Dim IsUserAuthenticated As Boolean = objIVSWebClient.IsUserAuthenticated(objIVSLicense.IVSGUID, strUserName, strPassword)

                    Dim ListOfResults As New List(Of Results)
                    Dim objResults As New Results

                    objResults.ResultsItem = "IsUserAuthenticated"
                    objResults.ResultsValue = IsUserAuthenticated
                    ListOfResults.Add(objResults)

                    Me.dgLists.ItemsSource = ListOfResults

                    Me.dgLists.Visibility = Windows.Visibility.Visible
                    Me.txtResults.Visibility = Windows.Visibility.Hidden
                    Me.lblExecuteFunctionStatus.Content = "IsUserAuthenticated Completed"

                Case "GetAlerts"
                    Me.dgLists.ItemsSource = objIVSWebClient.GetAlerts(objIVSLicense.IVSGUID)
                    Me.dgLists.Visibility = Windows.Visibility.Visible
                    Me.txtResults.Visibility = Windows.Visibility.Hidden
                    Me.lblExecuteFunctionStatus.Content = "GetAlerts Completed"

                Case "GetAlertDetail"
                    intAlertID = Me.txtAlertID.Text
                    Dim objAlertDetail As New AlertDetail
                    objAlertDetail = objIVSWebClient.GetAlertDetail(objIVSLicense.IVSGUID, intAlertID)

                    Me.dgLists.ItemsSource = GetListOfResults_AlertDetail(objAlertDetail)

                    Me.dgLists.Visibility = Windows.Visibility.Visible
                    Me.txtResults.Visibility = Windows.Visibility.Hidden
                    Me.lblExecuteFunctionStatus.Content = "GetAlertDetail Completed"

                Case "GetSwipeScanAlerts"
                    strNameLast = Me.txtNameLast.Text
                    strIDNumber = Me.txtIDNumber.Text

                    Me.dgLists.ItemsSource = objIVSWebClient.GetSwipeScanAlerts(objIVSLicense.IVSGUID, strIDNumber, "", strNameLast)
                    Me.dgLists.Visibility = Windows.Visibility.Visible
                    Me.txtResults.Visibility = Windows.Visibility.Hidden
                    Me.lblExecuteFunctionStatus.Content = "GetSwipeScanAlerts Completed"

                Case "NewAlert"
                    Dim objAlertDetail As New AlertDetail
                    objAlertDetail.IDNumber = Me.txtIDNumber.Text
                    objAlertDetail.NameFirst = "TESTY"
                    objAlertDetail.NameLast = Me.txtNameLast.Text
                    objAlertDetail.DateOfBirth = "01/01/1900"
                    objAlertDetail.AlertContactName = "Mr. Contact"
                    objAlertDetail.AlertContactNumber = "555-1234"
                    objAlertDetail.ActiveFlag = True
                    objAlertDetail.AlertNotes = "This is a test alert generated from the webservice tester"
                    objAlertDetail.UserID = Me.txtUserID.Text

                    objIVSWebClient.NewAlert(objIVSLicense.IVSGUID, objAlertDetail)

                    Me.lblExecuteFunctionStatus.Content = "NewAlert Completed"

                Case "UpdateAlert"
                    Me.lblExecuteFunctionStatus.Content = "UpdateAlert Completed"

                Case "DeleteAlert"
                    intAlertID = Me.txtAlertID.Text

                    If MessageBox.Show("Are you sure you want to delete AlertID: " & intAlertID & " ?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

                        objIVSWebClient.DeleteAlert(objIVSLicense.IVSGUID, intAlertID)

                    End If
                    Me.lblExecuteFunctionStatus.Content = "DeleteAlert Completed"

                Case "EnableAlert"
                    intAlertID = Me.txtAlertID.Text
                    isActive = Me.cbIsActive.IsChecked

                    objIVSWebClient.EnableAlert(objIVSLicense.IVSGUID, intAlertID, isActive)
                    Me.lblExecuteFunctionStatus.Content = "EnableAlert Completed"

                Case "UpdateImageAvailable"
                    intSwipeScanID = Me.txtSwipeScanID.Text
                    objIVSWebClient.UpdateImageAvailable(objIVSLicense.IVSGUID, intSwipeScanID)
                    Me.lblExecuteFunctionStatus.Content = "UpdateImageAvailable Completed"

                Case "NewDataSwipeScan"
                    Dim objSwipeScanInfo As New SwipeScanInfo
                    Dim objSwipeScanDetail As New SwipeScanDetail
                    objSwipeScanInfo.ClientID = Me.txtClientID.Text
                    objSwipeScanInfo.UserID = Me.txtUserID.Text
                    objSwipeScanInfo.CCDigits = 4
                    objSwipeScanInfo.DisableCCSave = False
                    objSwipeScanInfo.DisableDBSave = False
                    objSwipeScanInfo.ScanType = "Drivers License Or State ID"
                    objSwipeScanInfo.SwipeScanRawData = Me.txtIDNumber.Text
                    ' objSwipeScanInfo.SwipeScanRawData = "@  ANSI 6360380101DL00290054DLDAQA000023407310 DAAJOSEPH SMITH SAMPLE DBB19870522"
                    objSwipeScanDetail = objIVSWebClient.NewDataSwipeScan(objIVSLicense.IVSGUID, objSwipeScanInfo)

                    Me.dgLists.ItemsSource = GetListOfResults_SwipeScanDetail(objSwipeScanDetail)

                    Me.dgLists.Visibility = Windows.Visibility.Visible
                    Me.txtResults.Visibility = Windows.Visibility.Hidden
                    Me.lblExecuteFunctionStatus.Content = "NewDataSwipeScan Completed"

                Case "NewDataSwipeScanManual"
                    Me.lblExecuteFunctionStatus.Content = "NewDataSwipeScanManual Completed"

                Case "GetSwipeScanDetail"
                    intSwipeScanID = Me.txtSwipeScanID.Text
                    strSwipeScanType = Me.cboSwipeScanType.SelectedItem.ToString

                    Dim objSwipeScanDetail As New SwipeScanDetail
                    objSwipeScanDetail = objIVSWebClient.GetSwipeScanDetail(objIVSLicense.IVSGUID, intSwipeScanID, strSwipeScanType)

                    Me.dgLists.ItemsSource = GetListOfResults_SwipeScanDetail(objSwipeScanDetail)

                    Me.dgLists.Visibility = Windows.Visibility.Visible
                    Me.txtResults.Visibility = Windows.Visibility.Hidden
                    Me.lblExecuteFunctionStatus.Content = "GetSwipeScanDetail Completed"

                Case "GetSwipeScanHistory"
                    strSwipeScanType = Me.cboSwipeScanType.SelectedItem.ToString
                    strIDNumber = Me.txtIDNumber.Text

                    Me.dgLists.ItemsSource = objIVSWebClient.GetSwipeScanHistory(objIVSLicense.IVSGUID, strIDNumber, strSwipeScanType)
                    Me.dgLists.Visibility = Windows.Visibility.Visible
                    Me.txtResults.Visibility = Windows.Visibility.Hidden
                    Me.lblExecuteFunctionStatus.Content = "GetSwipeScanHistory Completed"

                Case "GetSwipeScanType"
                    intSwipeScanID = Me.txtSwipeScanID.Text

                    strSwipeScanType = objIVSWebClient.GetSwipeScanType(objIVSLicense.IVSGUID, intSwipeScanID)

                    Dim ListOfResults As New List(Of Results)
                    Dim objResults As New Results
                    objResults.ResultsItem = "SwipeScanID"
                    objResults.ResultsValue = intSwipeScanID
                    ListOfResults.Add(objResults)
                    objResults = New Results
                    objResults.ResultsItem = "SwipeScanType"
                    objResults.ResultsValue = strSwipeScanType
                    ListOfResults.Add(objResults)

                    Me.cboSwipeScanType.SelectedItem = strSwipeScanType

                    Me.dgLists.ItemsSource = ListOfResults

                    Me.dgLists.Visibility = Windows.Visibility.Visible
                    Me.txtResults.Visibility = Windows.Visibility.Hidden
                    Me.lblExecuteFunctionStatus.Content = "GetSwipeScanType Completed"

                Case "SwipeScanNavigateFirst"
                    intUserID = Me.txtUserID.Text
                    intClientID = Me.txtClientID.Text

                    Dim objSwipeScanNavigateInfo As New SwipeScanNavigateInfo
                    objSwipeScanNavigateInfo = objIVSWebClient.SwipeScanNavigateFirst(objIVSLicense.IVSGUID, intUserID, intClientID)

                    Me.dgLists.ItemsSource = GetListOfResults_SwipeScanNavigateInfo(objSwipeScanNavigateInfo)

                    Me.dgLists.Visibility = Windows.Visibility.Visible
                    Me.txtResults.Visibility = Windows.Visibility.Hidden
                    Me.lblExecuteFunctionStatus.Content = "SwipeScanNavigateFirst Completed"

                Case "SwipeScanNavigatePrevious"
                    intSwipeScanID = Me.txtSwipeScanID.Text
                    intUserID = Me.txtUserID.Text
                    intClientID = Me.txtClientID.Text

                    Dim objSwipeScanNavigateInfo As New SwipeScanNavigateInfo
                    objSwipeScanNavigateInfo = objIVSWebClient.SwipeScanNavigatePrevious(objIVSLicense.IVSGUID, intUserID, intClientID, intSwipeScanID)

                    Me.dgLists.ItemsSource = GetListOfResults_SwipeScanNavigateInfo(objSwipeScanNavigateInfo)

                    Me.dgLists.Visibility = Windows.Visibility.Visible
                    Me.txtResults.Visibility = Windows.Visibility.Hidden
                    Me.lblExecuteFunctionStatus.Content = "SwipeScanNavigatePrevious Completed"

                Case "SwipeScanNavigatePosition"
                    intSwipeScanID = Me.txtSwipeScanID.Text

                    Dim objSwipeScanNavigateInfo As New SwipeScanNavigateInfo
                    objSwipeScanNavigateInfo = objIVSWebClient.SwipeScanNavigatePosition(objIVSLicense.IVSGUID, intSwipeScanID)

                    Me.dgLists.ItemsSource = GetListOfResults_SwipeScanNavigateInfo(objSwipeScanNavigateInfo)

                    Me.dgLists.Visibility = Windows.Visibility.Visible
                    Me.txtResults.Visibility = Windows.Visibility.Hidden
                    Me.lblExecuteFunctionStatus.Content = "SwipeScanNavigatePosition Completed"

                Case "SwipeScanNavigateNext"
                    intSwipeScanID = Me.txtSwipeScanID.Text
                    intUserID = Me.txtUserID.Text
                    intClientID = Me.txtClientID.Text

                    Dim objSwipeScanNavigateInfo As New SwipeScanNavigateInfo
                    objSwipeScanNavigateInfo = objIVSWebClient.SwipeScanNavigateNext(objIVSLicense.IVSGUID, intUserID, intClientID, intSwipeScanID)

                    Me.dgLists.ItemsSource = GetListOfResults_SwipeScanNavigateInfo(objSwipeScanNavigateInfo)

                    Me.dgLists.Visibility = Windows.Visibility.Visible
                    Me.txtResults.Visibility = Windows.Visibility.Hidden
                    Me.lblExecuteFunctionStatus.Content = "SwipeScanNavigateNext Completed"

                Case "SwipeScanNavigateLast"
                    intUserID = Me.txtUserID.Text
                    intClientID = Me.txtClientID.Text

                    Dim objSwipeScanNavigateInfo As New SwipeScanNavigateInfo
                    objSwipeScanNavigateInfo = objIVSWebClient.SwipeScanNavigateLast(objIVSLicense.IVSGUID, intUserID, intClientID)

                    Me.dgLists.ItemsSource = GetListOfResults_SwipeScanNavigateInfo(objSwipeScanNavigateInfo)

                    Me.dgLists.Visibility = Windows.Visibility.Visible
                    Me.txtResults.Visibility = Windows.Visibility.Hidden
                    Me.lblExecuteFunctionStatus.Content = "SwipeScanNavigateLast Completed"

                Case "DeleteSwipeScan"
                    intSwipeScanID = Me.txtSwipeScanID.Text

                    If MessageBox.Show("Are you sure you want to delete SwipeScanID: " & intSwipeScanID & " ?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) = MessageBoxResult.OK Then

                        objIVSWebClient.DeleteSwipeScan(objIVSLicense.IVSGUID, intSwipeScanID)

                    End If
                    Me.lblExecuteFunctionStatus.Content = "DeleteSwipeScan Completed"

            End Select

            objIVSWebClient.Close()

            timeEnd = Now

            Dim timeElapsed As TimeSpan = timeEnd - timeStart
            'Me.lblExecuteFunctionStatus.Content += timeElapsed.Seconds.ToString

            Me.lblTimeSpan.Content = "Elapsed Time: "
            Me.lblTimeSpan.Content += timeElapsed.Hours.ToString
            Me.lblTimeSpan.Content += "H "
            Me.lblTimeSpan.Content += timeElapsed.Minutes.ToString
            Me.lblTimeSpan.Content += "M "
            Me.lblTimeSpan.Content += timeElapsed.Seconds.ToString
            Me.lblTimeSpan.Content += "S "
            Me.lblTimeSpan.Content += timeElapsed.Milliseconds.ToString
            Me.lblTimeSpan.Content += "MS "


        Catch ex As Exception
            Me.txtResults.Text = ex.ToString
            Me.txtResults.Visibility = Windows.Visibility.Visible
            Me.dgLists.Visibility = Windows.Visibility.Hidden
        End Try

    End Sub

    Private Sub cboFunctions_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles cboFunctions.SelectionChanged

        Select Case Me.cboFunctions.SelectedItem

            Case "NewClient"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Visible
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Visible
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "GetClientID"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Visible
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "GetClients"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "GetLocations"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "UpdateClientIPAddress"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Visible
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "GetClientSettings"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "SaveClientSettings"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "UpdateIDecodeLicense"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "GetStationName"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "GetDeviceInfo"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "GetUsers"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "GetUserNames"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "GetUserName"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "GetUserPhone"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "GetUserDetail"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "NewUser"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "UpdateUser"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "DeleteUser"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "EnableUser"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Visible
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "IsUserNameAvailable"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Visible
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "IsUserAuthenticated"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Visible
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Visible
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "GetAlerts"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "GetAlertDetail"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "GetSwipeScanAlerts"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Visible
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Visible
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "NewAlert"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Visible
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Visible
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "UpdateAlert"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "DeleteAlert"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "EnableAlert"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Visible
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "UpdateImageAvailable"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "NewDataSwipeScan"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "NewDataSwipeScanManual"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "GetSwipeScanDetail"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Visible

            Case "GetSwipeScanHistory"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Visible
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Visible

            Case "GetSwipeScanType"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "SwipeScanNavigateFirst"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "SwipeScanNavigatePrevious"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "SwipeScanNavigatePosition"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "SwipeScanNavigateNext"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "SwipeScanNavigateLast"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

            Case "DeleteSwipeScan"
                Me.imgHostNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIPAddressRequired.Visibility = Windows.Visibility.Hidden
                Me.imgClientIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIsActiveRequired.Visibility = Windows.Visibility.Hidden
                Me.imgUserNameRequired.Visibility = Windows.Visibility.Hidden
                Me.imgPasswordRequired.Visibility = Windows.Visibility.Hidden
                Me.imgAlertIDRequired.Visibility = Windows.Visibility.Hidden
                Me.imgNameLastRequired.Visibility = Windows.Visibility.Hidden
                Me.imgIDNumberRequired.Visibility = Windows.Visibility.Hidden
                Me.imgSwipeScanIDRequired.Visibility = Windows.Visibility.Visible
                Me.imgSwipeScanTypeRequired.Visibility = Windows.Visibility.Hidden

        End Select

    End Sub

    Private Function GetImagePath() As String
        Dim sb As New StringBuilder

        Try
            sb.Append(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData))
            sb.Append("\")
            sb.Append(Trim(My.Application.Info.CompanyName))
            sb.Append("\")
            'This needs to be set to SNARE/PROTECT/PATRIOT
            sb.Append(My.Application.Info.ProductName)
            sb.Append("\Images")

        Catch ex As Exception
            Me.txtResults.Text = ex.ToString
            Me.txtResults.Visibility = Windows.Visibility.Visible
            Me.dgLists.Visibility = Windows.Visibility.Hidden
        End Try

        Return sb.ToString

    End Function

    Private Function GetListOfResults_ClientSettings(ByVal objClientSettings As ClientSettings) As List(Of Results)

        Dim ListOfResults As New List(Of Results)
        Dim objResults As New Results

        Me.txtResults.Text += "ClientID:" & objClientSettings.ClientID & vbCrLf
        Me.txtResults.Text += "DeviceID:" & objClientSettings.DeviceID & vbCrLf
        Me.txtResults.Visibility = Windows.Visibility.Visible


        objResults.ResultsItem = "ClientID"
        objResults.ResultsValue = objClientSettings.ClientID
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "DeviceID"
        objResults.ResultsValue = objClientSettings.DeviceID
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "ComPort"
        objResults.ResultsValue = objClientSettings.ComPort
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "IDecodeLicense"
        objResults.ResultsValue = objClientSettings.IDecodeLicense
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "IDecodeTrackFormat"
        objResults.ResultsValue = objClientSettings.IDecodeTrackFormat
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "IDecodeCardTypes"
        objResults.ResultsValue = objClientSettings.IDecodeCardTypes
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "Location"
        objResults.ResultsValue = objClientSettings.Location
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "Station"
        objResults.ResultsValue = objClientSettings.Station
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "Phone"
        objResults.ResultsValue = objClientSettings.Phone
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "Email"
        objResults.ResultsValue = objClientSettings.Email
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "SkipLogon"
        objResults.ResultsValue = objClientSettings.SkipLogon
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "DisplayAdmin"
        objResults.ResultsValue = objClientSettings.DisplayAdmin
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "DefaultUser"
        objResults.ResultsValue = objClientSettings.DefaultUser
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "AgeHighlight"
        objResults.ResultsValue = objClientSettings.AgeHighlight
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "AgePopup"
        objResults.ResultsValue = objClientSettings.AgePopup
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "Age"
        objResults.ResultsValue = objClientSettings.Age
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "ImageSave"
        objResults.ResultsValue = objClientSettings.ImageSave
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "ImageLocation"
        objResults.ResultsValue = objClientSettings.ImageLocation
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "ViewingTime"
        objResults.ResultsValue = objClientSettings.ViewingTime
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "CCDigits"
        objResults.ResultsValue = objClientSettings.CCDigits
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "DisableCCSave"
        objResults.ResultsValue = objClientSettings.DisableCCSave
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "DisableDBSave"
        objResults.ResultsValue = objClientSettings.DisableDBSave
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "LogRetention"
        objResults.ResultsValue = objClientSettings.LogRetention
        ListOfResults.Add(objResults)

        Return ListOfResults

    End Function

    Private Function GetListOfResults_DeviceInfo(ByVal objDeviceInfo As DeviceInfo) As List(Of Results)

        Try
            Me.txtResults.Visibility = Windows.Visibility.Visible
            Me.txtResults.Text += "DeviceType:" & objDeviceInfo.DeviceType & vbCrLf
            Me.txtResults.Text += "ComPort:" & objDeviceInfo.ComPort & vbCrLf
            'Me.txtResults.Text += "DeviceType:" & objDeviceInfo.DeviceType & vbCrLf
            'Me.txtResults.Text += "DeviceType:" & objDeviceInfo.DeviceType & vbCrLf

            Dim ListOfResults As New List(Of Results)
            Dim objResults As New Results

            objResults.ResultsItem = "DeviceID"
            objResults.ResultsValue = objDeviceInfo.DeviceID
            ListOfResults.Add(objResults)
            objResults = New Results
            objResults.ResultsItem = "DeviceType"
            objResults.ResultsValue = objDeviceInfo.DeviceType
            ListOfResults.Add(objResults)
            'objResults = New Results
            'objResults.ResultsItem = "ModelNo"
            'objResults.ResultsValue = objDeviceInfo.ModelNo
            'ListOfResults.Add(objResults)
            'objResults = New Results
            'objResults.ResultsItem = "SerialNo"
            'objResults.ResultsValue = objDeviceInfo.SerialNo
            'ListOfResults.Add(objResults)
            'objResults = New Results
            'objResults.ResultsItem = "FirmwareRev"
            'objResults.ResultsValue = objDeviceInfo.FirmwareRev
            'ListOfResults.Add(objResults)
            'objResults = New Results
            'objResults.ResultsItem = "FirmwareDate"
            'objResults.ResultsValue = objDeviceInfo.FirmwareDate
            'ListOfResults.Add(objResults)
            'objResults = New Results
            'objResults.ResultsItem = "HardwareRev"
            'objResults.ResultsValue = objDeviceInfo.HardwareRev
            'ListOfResults.Add(objResults)
            ''objResults = New Results
            ''objResults.ResultsItem = "ComPort"
            ''objResults.ResultsValue = objDeviceInfo.ComPort
            ''ListOfResults.Add(objResults)
            'objResults = New Results
            'objResults.ResultsItem = "UpdateTS"
            'objResults.ResultsValue = objDeviceInfo.UpdateTS
            'ListOfResults.Add(objResults)

            Return ListOfResults
        Catch ex As Exception
            Me.txtResults.Text += ex.ToString
        End Try


    End Function

    Private Function GetListOfResults_UserDetail(ByVal objUserDetail As UserDetail) As List(Of Results)

        Dim ListOfResults As New List(Of Results)
        Dim objResults As New Results

        objResults.ResultsItem = "UserID"
        objResults.ResultsValue = objUserDetail.UserID
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "UserName"
        objResults.ResultsValue = objUserDetail.UserName
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "Password"
        objResults.ResultsValue = objUserDetail.Password
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "UserNameFirst"
        objResults.ResultsValue = objUserDetail.UserNameFirst
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "UserNameLast"
        objResults.ResultsValue = objUserDetail.UserNameLast
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "UserEmail"
        objResults.ResultsValue = objUserDetail.UserEmail
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "UserPhone"
        objResults.ResultsValue = objUserDetail.UserPhone
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "AdminFlag"
        objResults.ResultsValue = objUserDetail.AdminFlag
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "AlertFlag"
        objResults.ResultsValue = objUserDetail.AlertFlag
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "SearchFlag"
        objResults.ResultsValue = objUserDetail.SearchFlag
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "ActiveFlag"
        objResults.ResultsValue = objUserDetail.ActiveFlag
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "UpdateTS"
        objResults.ResultsValue = objUserDetail.UpdateTS
        ListOfResults.Add(objResults)

        Return ListOfResults

    End Function

    Private Function GetListOfResults_AlertDetail(ByVal objAlertDetail As AlertDetail) As List(Of Results)

        Dim ListOfResults As New List(Of Results)
        Dim objResults As New Results

        objResults.ResultsItem = "AlertID"
        objResults.ResultsValue = objAlertDetail.AlertID
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "IDNumber"
        objResults.ResultsValue = objAlertDetail.IDNumber
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "NameFirst"
        objResults.ResultsValue = objAlertDetail.NameFirst
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "NameLast"
        objResults.ResultsValue = objAlertDetail.NameLast
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "DateOfBirth"
        objResults.ResultsValue = objAlertDetail.DateOfBirth
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "AlertContactName"
        objResults.ResultsValue = objAlertDetail.AlertContactName
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "AlertContactNumber"
        objResults.ResultsValue = objAlertDetail.AlertContactNumber
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "AlertNotes"
        objResults.ResultsValue = objAlertDetail.AlertNotes
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "ActiveFlag"
        objResults.ResultsValue = objAlertDetail.ActiveFlag
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "UserID"
        objResults.ResultsValue = objAlertDetail.UserID
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "UserName"
        objResults.ResultsValue = objAlertDetail.UserName
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "MatchLast"
        objResults.ResultsValue = objAlertDetail.MatchLast
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "MatchID"
        objResults.ResultsValue = objAlertDetail.MatchID
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "UpdateTS"
        objResults.ResultsValue = objAlertDetail.UpdateTS
        ListOfResults.Add(objResults)

        Return ListOfResults

    End Function

    Private Function GetListOfResults_SwipeScanDetail(ByVal objSwipeScanDetail As SwipeScanDetail) As List(Of Results)

        Dim ListOfResults As New List(Of Results)
        Dim objResults As New Results

        objResults.ResultsItem = "SwipeScanID"
        objResults.ResultsValue = objSwipeScanDetail.SwipeScanID
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "CheckNumber"
        objResults.ResultsValue = objSwipeScanDetail.CheckNumber
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "IDAccountNumber"
        objResults.ResultsValue = objSwipeScanDetail.IDAccountNumber
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "CardType"
        objResults.ResultsValue = objSwipeScanDetail.CardType
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "CCIssuer"
        objResults.ResultsValue = objSwipeScanDetail.CCIssuer
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "NameFirst"
        objResults.ResultsValue = objSwipeScanDetail.NameFirst
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "NameLast"
        objResults.ResultsValue = objSwipeScanDetail.NameLast
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "NameMiddle"
        objResults.ResultsValue = objSwipeScanDetail.NameMiddle
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "DateOfBirth"
        objResults.ResultsValue = objSwipeScanDetail.DateOfBirth
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "Age"
        objResults.ResultsValue = objSwipeScanDetail.Age
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "Sex"
        objResults.ResultsValue = objSwipeScanDetail.Sex
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "Height"
        objResults.ResultsValue = objSwipeScanDetail.Height
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "Weight"
        objResults.ResultsValue = objSwipeScanDetail.Weight
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "Eyes"
        objResults.ResultsValue = objSwipeScanDetail.Eyes
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "Hair"
        objResults.ResultsValue = objSwipeScanDetail.Hair
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "DateOfIssue"
        objResults.ResultsValue = objSwipeScanDetail.DateOfIssue
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "DateOfExpiration"
        objResults.ResultsValue = objSwipeScanDetail.DateOfExpiration
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "AddressStreet"
        objResults.ResultsValue = objSwipeScanDetail.AddressStreet
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "AddressCity"
        objResults.ResultsValue = objSwipeScanDetail.AddressCity
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "AddressState"
        objResults.ResultsValue = objSwipeScanDetail.AddressState
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "AddressZip"
        objResults.ResultsValue = objSwipeScanDetail.AddressZip
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "SwipeRawData"
        objResults.ResultsValue = objSwipeScanDetail.SwipeRawData
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "UserName"
        objResults.ResultsValue = objSwipeScanDetail.UserName
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "Location"
        objResults.ResultsValue = objSwipeScanDetail.Location
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "ClientID"
        objResults.ResultsValue = objSwipeScanDetail.ClientID
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "UserID"
        objResults.ResultsValue = objSwipeScanDetail.UserID
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "CCDigits"
        objResults.ResultsValue = objSwipeScanDetail.CCDigits
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "DisableCCSave"
        objResults.ResultsValue = objSwipeScanDetail.DisableCCSave
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "DisableDBSave"
        objResults.ResultsValue = objSwipeScanDetail.DisableDBSave
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "DataSource"
        objResults.ResultsValue = objSwipeScanDetail.DataSource
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "UpdateTS"
        objResults.ResultsValue = objSwipeScanDetail.UpdateTS
        ListOfResults.Add(objResults)

        Return ListOfResults

    End Function

    Private Function GetListOfResults_SwipeScanNavigateInfo(ByVal objSwipeScanNavigateInfo As SwipeScanNavigateInfo) As List(Of Results)

        Dim ListOfResults As New List(Of Results)
        Dim objResults As New Results

        objResults.ResultsItem = "SwipeScanID"
        objResults.ResultsValue = objSwipeScanNavigateInfo.SwipeScanID
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "SwipeScanType"
        objResults.ResultsValue = objSwipeScanNavigateInfo.SwipeScanType
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "SwipeScanTS"
        objResults.ResultsValue = objSwipeScanNavigateInfo.SwipeScanTS
        ListOfResults.Add(objResults)

        Return ListOfResults

    End Function

End Class

Public Class Results

    Private _ResultsItem As String
    Private _ResultsValue As String

    Property ResultsItem() As String
        Get
            Return _ResultsItem
        End Get
        Set(ByVal Value As String)
            _ResultsItem = Value
        End Set
    End Property

    Property ResultsValue() As String
        Get
            Return _ResultsValue
        End Get
        Set(ByVal Value As String)
            _ResultsValue = Value
        End Set
    End Property

End Class
