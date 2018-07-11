Imports System.Text
Imports System.Net
Imports IVS.AppLog
Imports IVS.Data.WS.TEP.IVSService

Public Class DataAccess

    Public Shared MyAppLog As New ApplicationLog(ApplicationLog.DateEncodingType.YYYYMMDD, True)

    Public Shared Sub NewException(ByVal exc As Exception)

        'Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        'Dim objIVSLicense As New IVS.License.IVSLicense

        'Try
        '    objIVSWebClient.NewException(objIVSLicense.IVSGUID, exc, ReturnMyIPAddress)

        'Catch ex As Exception
        '    MyAppLog.WriteToLog(ex)
        'Finally
        '    objIVSWebClient.Close()
        'End Try

    End Sub

    Public Shared Sub NewException(ByVal ExSource As String, ByVal ExTargetSite As String, ByVal ExType As String, ByVal ExMessage As String, ByVal ExData As String, ByVal ExStackTrace As String)

        'Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        'Dim objIVSLicense As New IVS.License.IVSLicense

        'Try
        '    'objIVSWebClient.NewException(objIVSLicense.IVSGUID, ReturnMyIPAddress, ExSource, ExTargetSite, ExType, ExMessage, ExData, ExStackTrace)

        'Catch ex As Exception
        '    MyAppLog.WriteToLog(ex)
        'Finally
        '    objIVSWebClient.Close()
        'End Try

    End Sub

#Region "tblClients"

    Public Shared Function NewClient() As Integer

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim intClientID As Integer
        Dim strIPAddress As String = Nothing
        Dim sb As New StringBuilder
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            strIPAddress = ReturnMyIPAddress()

            sb.Append(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData))
            sb.Append("\")
            sb.Append(Trim(My.Application.Info.CompanyName))
            sb.Append("\")
            sb.Append(My.Application.Info.ProductName)
            sb.Append("\Images")

            intClientID = objIVSWebClient.NewClient(objIVSLicense.IVSGUID, My.Computer.Name, ReturnMyIPAddress, sb.ToString)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return intClientID

    End Function

    Public Shared Function GetClientID() As Integer

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim intClientID As Integer
        Dim strHostname As String = Nothing
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try

            intClientID = objIVSWebClient.GetClientID(objIVSLicense.IVSGUID, My.Computer.Name)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return intClientID

    End Function

    Public Shared Function GetClients() As List(Of Clients)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim ListOfClients As New List(Of Clients)
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            ListOfClients = New List(Of Clients)(objIVSWebClient.GetClients(objIVSLicense.IVSGUID))

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return ListOfClients

    End Function

    Public Shared Function GetStations() As List(Of Locations)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim ListOfLocations As New List(Of Locations)
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            ListOfLocations = New List(Of Locations)(objIVSWebClient.GetStations(objIVSLicense.IVSGUID))

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return ListOfLocations

    End Function

    Public Shared Function GetLocations() As List(Of Locations)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim ListOfLocations As New List(Of Locations)
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            ListOfLocations = New List(Of Locations)(objIVSWebClient.GetLocations(objIVSLicense.IVSGUID))

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return ListOfLocations

    End Function

    Public Shared Function GetLocation(ByVal LocationID As Integer) As Locations

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objLocations As New Locations
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objLocations = objIVSWebClient.GetLocation(objIVSLicense.IVSGUID, LocationID)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return objLocations

    End Function

    Public Shared Sub NewLocation(ByVal LocationDescription As String)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objIVSWebClient.NewLocation(objIVSLicense.IVSGUID, LocationDescription)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

    End Sub

    Public Shared Sub UpdateLocation(ByVal NewLocation As Locations)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objIVSWebClient.UpdateLocation(objIVSLicense.IVSGUID, NewLocation)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

    End Sub

    Public Shared Sub DeleteLocation(ByVal LocationID As Integer)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objIVSWebClient.DeleteLocation(objIVSLicense.IVSGUID, LocationID)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

    End Sub

    Public Shared Sub UpdateClientIPAddress(ByVal ClientID As Integer)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objIVSWebClient.UpdateClientIPAddress(objIVSLicense.IVSGUID, ClientID, ReturnMyIPAddress)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

    End Sub

    Public Shared Function GetClientSettings(ByVal ClientID As Integer) As ClientSettings

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objClientSettings As New ClientSettings
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objClientSettings = objIVSWebClient.GetClientSettings(objIVSLicense.IVSGUID, ClientID)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return objClientSettings

    End Function

    Public Shared Sub SaveClientSettings(ByVal NewClientSettings As ClientSettings)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objIVSWebClient.SaveClientSettings(objIVSLicense.IVSGUID, NewClientSettings)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

    End Sub

    Public Shared Function GetStationName(ByVal ClientID As Integer) As String

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim strStationName As String
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            strStationName = objIVSWebClient.GetStationName(objIVSLicense.IVSGUID, ClientID)
        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return strStationName

    End Function

#End Region

#Region "tblDevices"

    Public Shared Function GetDeviceInfo(ByVal ClientID As Integer) As DeviceInfo

        Dim objDeviceInfo As New DeviceInfo
        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objDeviceInfo = objIVSWebClient.GetDeviceInfo(objIVSLicense.IVSGUID, ClientID)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return objDeviceInfo

    End Function

    Public Shared Sub UpdateDevice(ByVal DeviceInfo As DeviceInfo)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objIVSWebClient.UpdateDevice(objIVSLicense.IVSGUID, DeviceInfo)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

    End Sub

#End Region

#Region "tblUsers"

    Public Shared Function GetUsers() As List(Of UserDetail)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim ListOfUsers As New List(Of UserDetail)
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            ListOfUsers = New List(Of UserDetail)(objIVSWebClient.GetUsers(objIVSLicense.IVSGUID))

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return ListOfUsers

    End Function

    Public Shared Function GetUserNames() As List(Of UserDetail)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim ListOfUsers As New List(Of UserDetail)
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            ListOfUsers = New List(Of UserDetail)(objIVSWebClient.GetUserNames(objIVSLicense.IVSGUID))

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return ListOfUsers

    End Function

    Public Shared Function GetUserName(ByVal UserID As Integer) As String

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim strUserName As String
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            strUserName = objIVSWebClient.GetUserName(objIVSLicense.IVSGUID, UserID)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return strUserName

    End Function

    Public Shared Function GetUserPhone(ByVal UserID As Integer) As String

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim strUserPhone As String
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            strUserPhone = objIVSWebClient.GetUserPhone(objIVSLicense.IVSGUID, UserID)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return strUserPhone

    End Function

    Public Shared Function GetUserDetail(ByVal UserID As Integer) As UserDetail

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objUserDetail As New UserDetail
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objUserDetail = objIVSWebClient.GetUserDetail(objIVSLicense.IVSGUID, UserID)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return objUserDetail

    End Function

    Public Shared Sub NewUser(ByVal UserDetail As UserDetail)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objIVSWebClient.NewUser(objIVSLicense.IVSGUID, UserDetail)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

    End Sub

    Public Shared Sub UpdateUser(ByVal UserDetail As UserDetail)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objIVSWebClient.UpdateUser(objIVSLicense.IVSGUID, UserDetail)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

    End Sub

    Public Shared Sub DeleteUser(ByVal UserID As Integer)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objIVSWebClient.DeleteUser(objIVSLicense.IVSGUID, UserID)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

    End Sub

    Public Shared Sub EnableUser(ByVal UserID As Integer, ByVal IsActive As Boolean)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objIVSWebClient.EnableUser(objIVSLicense.IVSGUID, UserID, IsActive)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

    End Sub

    Public Shared Function IsUserNameAvailable(ByVal UserName As String) As Boolean

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim IsAvailable As Boolean = False
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            IsAvailable = objIVSWebClient.IsUserNameAvailable(objIVSLicense.IVSGUID, UserName)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return IsAvailable

    End Function

    Public Shared Function IsUserAuthenticated(ByVal UserName As String, ByVal Password As String) As Integer

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim intUserID As Integer = 0
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            intUserID = objIVSWebClient.IsUserAuthenticated(objIVSLicense.IVSGUID, UserName, Password)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return intUserID

    End Function

#End Region

#Region "tblAlerts"

    Public Shared Function GetAlerts() As List(Of AlertDetail)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim ListOfAlerts As New List(Of AlertDetail)
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            ListOfAlerts = New List(Of AlertDetail)(objIVSWebClient.GetAlerts(objIVSLicense.IVSGUID))

        Catch ex As Exception
            'objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return ListOfAlerts

    End Function

    Public Shared Function GetAlertDetail(ByVal AlertID As Integer) As AlertDetail

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objAlertDetail As New AlertDetail
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objAlertDetail = objIVSWebClient.GetAlertDetail(objIVSLicense.IVSGUID, AlertID)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return objAlertDetail

    End Function

    Public Shared Function GetSwipeScanAlerts(ByVal IDAccountNumber As String, ByVal NameFirst As String, ByVal NameLast As String) As List(Of AlertDetail)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim ListOfAlerts As New List(Of AlertDetail)
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            ListOfAlerts = New List(Of AlertDetail)(objIVSWebClient.GetSwipeScanAlerts(objIVSLicense.IVSGUID, IDAccountNumber, NameFirst, NameLast))

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return ListOfAlerts

    End Function

    Public Shared Sub NewAlert(ByVal AlertDetail As AlertDetail)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objIVSWebClient.NewAlert(objIVSLicense.IVSGUID, AlertDetail)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

    End Sub

    Public Shared Sub UpdateAlert(ByVal AlertDetail As AlertDetail)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objIVSWebClient.UpdateAlert(objIVSLicense.IVSGUID, AlertDetail)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

    End Sub

    Public Shared Sub DeleteAlert(ByVal AlertID As Integer)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objIVSWebClient.DeleteAlert(objIVSLicense.IVSGUID, AlertID)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

    End Sub

    Public Shared Sub EnableAlert(ByVal AlertID As Integer, ByVal IsActive As Boolean)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objIVSWebClient.EnableAlert(objIVSLicense.IVSGUID, AlertID, IsActive)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

    End Sub

#End Region

#Region "SwipeScans"

    Public Shared Sub UpdateImageAvailable(ByVal SwipeScanID As Integer)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objIVSWebClient.UpdateImageAvailable(objIVSLicense.IVSGUID, SwipeScanID)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

    End Sub

    Public Shared Function NewDataSwipeScan(ByVal SwipeScanInfo As SwipeScanInfo) As SwipeScanDetail

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objSwipeScanDetail As New SwipeScanDetail
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objSwipeScanDetail = objIVSWebClient.NewDataSwipeScan(objIVSLicense.IVSGUID, SwipeScanInfo)
        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return objSwipeScanDetail

    End Function

    Public Shared Function NewDataSwipeScanManual(ByVal SwipeScanDetail As SwipeScanDetail) As Integer

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim intSwipeScanID As Integer
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            intSwipeScanID = objIVSWebClient.NewDataSwipeScanManual(objIVSLicense.IVSGUID, SwipeScanDetail)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return intSwipeScanID

    End Function

    Public Shared Function GetSwipeScanDetail(ByVal SwipeScanID As Integer, ByVal SwipeScanType As String) As SwipeScanDetail

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objSwipeScanDetail As New SwipeScanDetail
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objSwipeScanDetail = objIVSWebClient.GetSwipeScanDetail(objIVSLicense.IVSGUID, SwipeScanID, SwipeScanType)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return objSwipeScanDetail

    End Function

    Public Shared Function GetSwipeScanHistory(ByVal IDAccountNumber As String, ByVal SwipeScanType As String) As List(Of SwipeScanHistory)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim ListOfSwipeScanHistory As New List(Of SwipeScanHistory)
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            ListOfSwipeScanHistory = New List(Of SwipeScanHistory)(objIVSWebClient.GetSwipeScanHistory(objIVSLicense.IVSGUID, IDAccountNumber, SwipeScanType))

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return ListOfSwipeScanHistory

    End Function

    Public Shared Function GetSwipeScanType(ByVal SwipeScanID As Integer) As String

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim strSwipeScanType As String
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            strSwipeScanType = objIVSWebClient.GetSwipeScanType(objIVSLicense.IVSGUID, SwipeScanID)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return strSwipeScanType

    End Function

    Public Shared Function SwipeScanNavigateFirst(ByVal UserID As Integer, ByVal ClientID As Integer) As SwipeScanNavigateInfo

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objSwipeScanNavigateInfo As New SwipeScanNavigateInfo
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objSwipeScanNavigateInfo = objIVSWebClient.SwipeScanNavigateFirst(objIVSLicense.IVSGUID, UserID, ClientID)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return objSwipeScanNavigateInfo

    End Function

    Public Shared Function SwipeScanNavigatePrevious(ByVal UserID As Integer, ByVal ClientID As Integer, ByVal SwipeScanID As Integer) As SwipeScanNavigateInfo

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objSwipeScanNavigateInfo As New SwipeScanNavigateInfo
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objSwipeScanNavigateInfo = objIVSWebClient.SwipeScanNavigatePrevious(objIVSLicense.IVSGUID, UserID, ClientID, SwipeScanID)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return objSwipeScanNavigateInfo

    End Function

    Public Shared Function SwipeScanNavigatePosition(ByVal SwipeScanID As Integer) As SwipeScanNavigateInfo

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objSwipeScanNavigateInfo As New SwipeScanNavigateInfo
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objSwipeScanNavigateInfo = objIVSWebClient.SwipeScanNavigatePosition(objIVSLicense.IVSGUID, SwipeScanID)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return objSwipeScanNavigateInfo

    End Function

    Public Shared Function SwipeScanNavigateNext(ByVal UserID As Integer, ByVal ClientID As Integer, ByVal SwipeScanID As Integer) As SwipeScanNavigateInfo

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objSwipeScanNavigateInfo As New SwipeScanNavigateInfo
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objSwipeScanNavigateInfo = objIVSWebClient.SwipeScanNavigateNext(objIVSLicense.IVSGUID, UserID, ClientID, SwipeScanID)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return objSwipeScanNavigateInfo

    End Function

    Public Shared Function SwipeScanNavigateLast(ByVal UserID As Integer, ByVal ClientID As Integer) As SwipeScanNavigateInfo

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objSwipeScanNavigateInfo As New SwipeScanNavigateInfo
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objSwipeScanNavigateInfo = objIVSWebClient.SwipeScanNavigateLast(objIVSLicense.IVSGUID, UserID, ClientID)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return objSwipeScanNavigateInfo

    End Function

    Public Shared Function SwipeScanNavigateFirstDetail(ByVal UserID As Integer, ByVal ClientID As Integer) As SwipeScanDetail

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objSwipeScanDetail As New SwipeScanDetail
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objSwipeScanDetail = objIVSWebClient.SwipeScanNavigateFirstDetail(objIVSLicense.IVSGUID, UserID, ClientID)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return objSwipeScanDetail

    End Function

    Public Shared Function SwipeScanNavigatePreviousDetail(ByVal UserID As Integer, ByVal ClientID As Integer, ByVal SwipeScanID As Integer) As SwipeScanDetail

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objSwipeScanDetail As New SwipeScanDetail
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objSwipeScanDetail = objIVSWebClient.SwipeScanNavigatePreviousDetail(objIVSLicense.IVSGUID, UserID, ClientID, SwipeScanID)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return objSwipeScanDetail

    End Function

    Public Shared Function SwipeScanNavigatePositionDetail(ByVal SwipeScanID As Integer) As SwipeScanDetail

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objSwipeScanDetail As New SwipeScanDetail
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objSwipeScanDetail = objIVSWebClient.SwipeScanNavigatePositionDetail(objIVSLicense.IVSGUID, SwipeScanID)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return objSwipeScanDetail

    End Function

    Public Shared Function SwipeScanNavigateNextDetail(ByVal UserID As Integer, ByVal ClientID As Integer, ByVal SwipeScanID As Integer) As SwipeScanDetail

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objSwipeScanDetail As New SwipeScanDetail
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objSwipeScanDetail = objIVSWebClient.SwipeScanNavigateNextDetail(objIVSLicense.IVSGUID, UserID, ClientID, SwipeScanID)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return objSwipeScanDetail

    End Function

    Public Shared Function SwipeScanNavigateLastDetail(ByVal UserID As Integer, ByVal ClientID As Integer) As SwipeScanDetail

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objSwipeScanDetail As New SwipeScanDetail
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objSwipeScanDetail = objIVSWebClient.SwipeScanNavigateLastDetail(objIVSLicense.IVSGUID, UserID, ClientID)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return objSwipeScanDetail

    End Function

    Public Shared Sub DeleteSwipeScan(ByVal SwipeScanID As Integer)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objIVSWebClient.DeleteSwipeScan(objIVSLicense.IVSGUID, SwipeScanID)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

    End Sub

    Public Shared Function GetSwipeScanSearch() As List(Of SwipeScanSearch)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim ListOfSwipeScanSearch As New List(Of SwipeScanSearch)
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            ListOfSwipeScanSearch = New List(Of SwipeScanSearch)(objIVSWebClient.GetSwipeScanSearch(objIVSLicense.IVSGUID))

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return ListOfSwipeScanSearch

    End Function

#End Region

    Public Shared Function ReturnMyIPAddress() As String
        Dim strMyHostName As String
        Dim strMyIPAddress As String = "IPAddress"
        Dim ipHostEntry As IPHostEntry

        Try
            strMyHostName = Dns.GetHostName()
            ipHostEntry = Dns.GetHostEntry(strMyHostName)

            ' Grab the first IP addresses that is not a loopback
            For Each ipaddress As IPAddress In ipHostEntry.AddressList

                If ipaddress.IsLoopback(ipaddress) = False Then

                    strMyIPAddress = ipaddress.ToString()

                End If

            Next
        Catch ex As Exception
            MyAppLog.WriteToLog(ex)
        End Try

        Return strMyIPAddress

    End Function

    Public Shared Function ReturnHostName(ByVal IPAddress As String) As String
        Dim strHostName As String = IPAddress

        Try
            Dim host As IPHostEntry = Dns.GetHostEntry(IPAddress)
            strHostName = host.HostName.ToString

        Catch ex As Exception
            MyAppLog.WriteToLog(ex)
        End Try

        Return strHostName

    End Function

#Region "TEP"

    Public Shared Function GetTEPViolations() As List(Of TEPViolations)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim ListOfTEPViolations As New List(Of TEPViolations)
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            ListOfTEPViolations = New List(Of TEPViolations)(objIVSWebClient.GetTEPViolations(objIVSLicense.IVSGUID))

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return ListOfTEPViolations

    End Function

    Public Shared Function GetTEPClientSettings() As TEPClientSettings

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objTEPClientSettings As New TEPClientSettings
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objTEPClientSettings = objIVSWebClient.GetTEPClientSettings(objIVSLicense.IVSGUID)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return objTEPClientSettings

    End Function

    Public Shared Sub NewDataSwipeScan_TEP(ByVal TEPSwipeScanDetail As TEPSwipeScanDetail)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objIVSWebClient.NewDataSwipeScan_TEP(objIVSLicense.IVSGUID, TEPSwipeScanDetail)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

    End Sub

    Public Shared Sub NewDataSwipeScan_Violation(ByVal TEPSwipeScanViolation As TEPSwipeScanViolations)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objIVSWebClient.NewDataSwipeScan_Violation(objIVSLicense.IVSGUID, TEPSwipeScanViolation)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

    End Sub

    Public Shared Function GetSwipeScanDetail_TEP(ByVal SwipeScanID As Integer) As TEPSwipeScanDetail

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim objTEPSwipeScanDetail As New TEPSwipeScanDetail
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            objTEPSwipeScanDetail = objIVSWebClient.GetSwipeScanDetail_TEP(objIVSLicense.IVSGUID, SwipeScanID)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return objTEPSwipeScanDetail

    End Function

    Public Shared Function GetSwipeScanDetail_Violations(ByVal SwipeScanID As Integer) As List(Of TEPSwipeScanViolations)

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim ListOfTEPSwipeScanViolations As New List(Of TEPSwipeScanViolations)
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            ListOfTEPSwipeScanViolations = New List(Of TEPSwipeScanViolations)(objIVSWebClient.GetSwipeScanDetail_Violations(objIVSLicense.IVSGUID, SwipeScanID))

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return ListOfTEPSwipeScanViolations

    End Function

    Public Shared Function IsCitationUsedAlready(ByVal CitationID As String) As Boolean

        Dim objIVSWebClient As IVSClient = New IVSClient("SSL")
        Dim IsAvailable As Boolean = False
        Dim objIVSLicense As New IVS.License.IVSLicense

        Try
            IsAvailable = objIVSWebClient.IsCitationUsedAlready(objIVSLicense.IVSGUID, CitationID)

        Catch ex As Exception
            objIVSWebClient.NewException(objIVSLicense.IVSGUID, ex, ReturnMyIPAddress)
            MyAppLog.WriteToLog(ex)
        Finally
            objIVSWebClient.Close()
        End Try

        Return IsAvailable

    End Function

#End Region

End Class
