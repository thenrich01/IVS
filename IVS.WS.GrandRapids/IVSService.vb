Imports System.ServiceModel.Channels
Imports IVS.Data.GrandRapids

<ServiceBehavior(InstanceContextMode:=InstanceContextMode.[Single])> _
Public Class IVSService
    Implements IIVS

    Private MyAppLog As New IVS.AppLog.ApplicationLog(IVS.AppLog.ApplicationLog.DateEncodingType.YYYYMMDD, True, "IVS.WS.GrandRapids", DateFormat.GeneralDate)

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
        MyAppLog.WriteToLog("IVSService.Finalize()")
    End Sub

#Region "tblClients"

    Private Function NewClient(ByVal LicenseGuid As Guid, ByVal HostName As String, ByVal IPAddress As String, ByVal ImagePath As String) As Integer Implements IIVS.NewClient

        Dim intClientID As Integer
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                intClientID = DataAccess.NewClient(HostName, IPAddress, ImagePath)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "NewClient")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.NewClient()" & ex.ToString)
        End Try

        Return intClientID

    End Function

    Private Function GetClientID(ByVal LicenseGuid As Guid, ByVal HostName As String) As String Implements IIVS.GetClientID

        Dim intClientID As Integer = 0
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                intClientID = DataAccess.GetClientID(objThisEndPointProperty.Address, HostName)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "GetClientID")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.GetClientID()" & ex.ToString)
        End Try

        Return intClientID

    End Function

    Private Function GetClients(ByVal LicenseGuid As Guid) As List(Of Clients) Implements IIVS.GetClients

        Dim ListOfClients As New List(Of Clients)
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                ListOfClients = DataAccess.GetClients(objThisEndPointProperty.Address)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "GetClients")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.GetClients()" & ex.ToString)
        End Try

        Return ListOfClients

    End Function

    Private Function GetStations(ByVal LicenseGuid As Guid) As List(Of Locations) Implements IIVS.GetStations

        Dim ListOfLocations As New List(Of Locations)
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                ListOfLocations = DataAccess.GetStations(objThisEndPointProperty.Address)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "GetStations")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.GetStations()" & ex.ToString)
        End Try

        Return ListOfLocations

    End Function

    Private Function GetLocations(ByVal LicenseGuid As Guid) As List(Of Locations) Implements IIVS.GetLocations

        Dim ListOfLocations As New List(Of Locations)
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                ListOfLocations = DataAccess.GetLocations(objThisEndPointProperty.Address)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "GetLocations")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.GetLocations()" & ex.ToString)
        End Try

        Return ListOfLocations

    End Function

    Private Function GetLocation(ByVal LicenseGuid As Guid, ByVal LocationID As Integer) As Locations Implements IIVS.GetLocation

        Dim objLocations As New Locations
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                objLocations = DataAccess.GetLocation(objThisEndPointProperty.Address, LocationID)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "GetLocation")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.GetLocation()" & ex.ToString)
        End Try

        Return objLocations

    End Function

    Private Sub NewLocation(ByVal LicenseGuid As Guid, ByVal LocationDescription As String) Implements IIVS.NewLocation

        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                DataAccess.NewLocation(objThisEndPointProperty.Address, LocationDescription)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "NewLocation")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.NewLocation()" & ex.ToString)
        End Try

    End Sub

    Private Sub UpdateLocation(ByVal LicenseGuid As Guid, ByVal NewLocation As Locations) Implements IIVS.UpdateLocation

        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                DataAccess.UpdateLocation(objThisEndPointProperty.Address, NewLocation)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "UpdateLocation")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.UpdateLocation()" & ex.ToString)
        End Try

    End Sub

    Private Sub DeleteLocation(ByVal LicenseGuid As Guid, ByVal LocationID As Integer) Implements IIVS.DeleteLocation

        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                DataAccess.DeleteLocation(objThisEndPointProperty.Address, LocationID)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "DeleteLocation")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.DeleteLocation()" & ex.ToString)
        End Try

    End Sub

    Private Function UpdateClientIPAddress(ByVal LicenseGuid As Guid, ByVal ClientID As Integer, ByVal IPAddress As String) As Boolean Implements IIVS.UpdateClientIPAddress

        Dim boolResult As Boolean = False
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                DataAccess.UpdateClientIPAddress(ClientID, IPAddress)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "UpdateClientIPAddress")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.UpdateClientIPAddress()" & ex.ToString)
        End Try

        Return boolResult

    End Function

    Private Function GetClientSettings(ByVal LicenseGuid As Guid, ByVal ClientID As Integer) As ClientSettings Implements IIVS.GetClientSettings

        Dim objClientSettings As New ClientSettings
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                objClientSettings = DataAccess.GetClientSettings(objThisEndPointProperty.Address, ClientID)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "GetClientSettings")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.GetClientSettings()" & ex.ToString)
        End Try

        Return objClientSettings

    End Function

    Private Sub SaveClientSettings(ByVal LicenseGuid As Guid, ByVal NewClientSettings As ClientSettings) Implements IIVS.SaveClientSettings

        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                DataAccess.SaveClientSettings(objThisEndPointProperty.Address, NewClientSettings)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "SaveClientSettings")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.SaveClientSettings()" & ex.ToString)
        End Try

    End Sub

    Private Function GetStationName(ByVal LicenseGuid As Guid, ByVal ClientID As Integer) As String Implements IIVS.GetStationName

        Dim strStationName As String = Nothing
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                strStationName = DataAccess.GetStationName(objThisEndPointProperty.Address, ClientID)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "GetStationName")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.GetStationName()" & ex.ToString)
        End Try

        Return strStationName

    End Function

#End Region

#Region "tblDevices"

    Private Function GetDeviceInfo(ByVal LicenseGuid As Guid, ByVal ClientID As Integer) As DeviceInfo Implements IIVS.GetDeviceInfo

        Dim objDeviceInfo As New DeviceInfo
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                objDeviceInfo = DataAccess.GetDeviceInfo(objThisEndPointProperty.Address, ClientID)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "GetDeviceInfo")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.GetDeviceInfo()" & ex.ToString)
        End Try

        Return objDeviceInfo

    End Function

    Private Sub UpdateDevice(ByVal LicenseGuid As Guid, ByVal DeviceInfo As DeviceInfo) Implements IIVS.UpdateDevice

        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                DataAccess.UpdateDevice(objThisEndPointProperty.Address, DeviceInfo)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "UpdateDevice")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.UpdateDevice()" & ex.ToString)
        End Try

    End Sub

#End Region

#Region "tblUsers"

    Private Function GetUsers(ByVal LicenseGuid As Guid) As List(Of UserDetail) Implements IIVS.GetUsers

        Dim ListOfUsers As New List(Of UserDetail)
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                ListOfUsers = DataAccess.GetUsers(objThisEndPointProperty.Address)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "GetUsers")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.GetUsers()" & ex.ToString)
        End Try

        Return ListOfUsers

    End Function

    Private Function GetUserNames(ByVal LicenseGuid As Guid) As List(Of UserDetail) Implements IIVS.GetUserNames

        Dim ListOfUsers As New List(Of UserDetail)
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                ListOfUsers = DataAccess.GetUserNames(objThisEndPointProperty.Address)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "GetUserNames")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.GetUserNames()" & ex.ToString)
        End Try

        Return ListOfUsers

    End Function

    Private Function GetUserName(ByVal LicenseGuid As Guid, ByVal UserID As Integer) As String Implements IIVS.GetUserName

        Dim UserName As String = Nothing
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                UserName = DataAccess.GetUserName(objThisEndPointProperty.Address, UserID)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "GetUserName")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.GetUserName()" & ex.ToString)
        End Try

        Return UserName

    End Function

    Private Function GetUserPhone(ByVal LicenseGuid As Guid, ByVal UserID As Integer) As String Implements IIVS.GetUserPhone

        Dim UserPhone As String = Nothing
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                UserPhone = DataAccess.GetUserPhone(objThisEndPointProperty.Address, UserID)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "GetUserPhone")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.GetUserPhone()" & ex.ToString)
        End Try

        Return UserPhone

    End Function

    Private Function GetUserDetail(ByVal LicenseGuid As Guid, ByVal UserID As Integer) As UserDetail Implements IIVS.GetUserDetail

        Dim objUserDetail As New UserDetail
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                objUserDetail = DataAccess.GetUserDetail(objThisEndPointProperty.Address, UserID)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "GetUserDetail")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.GetUserDetail()" & ex.ToString)
        End Try

        Return objUserDetail

    End Function

    Private Sub NewUser(ByVal LicenseGuid As Guid, ByVal UserDetail As UserDetail) Implements IIVS.NewUser

        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                DataAccess.NewUser(objThisEndPointProperty.Address, UserDetail)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "NewUser")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.NewUser()" & ex.ToString)
        End Try

    End Sub

    Private Sub UpdateUser(ByVal LicenseGuid As Guid, ByVal UserDetail As UserDetail) Implements IIVS.UpdateUser

        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                DataAccess.UpdateUser(objThisEndPointProperty.Address, UserDetail)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "UpdateUser")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.UpdateUser()" & ex.ToString)
        End Try

    End Sub

    Private Sub DeleteUser(ByVal LicenseGuid As Guid, ByVal UserID As Integer) Implements IIVS.DeleteUser

        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                DataAccess.DeleteUser(objThisEndPointProperty.Address, UserID)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "DeleteUser")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.DeleteUser()" & ex.ToString)
        End Try

    End Sub

    Private Sub EnableUser(ByVal LicenseGuid As Guid, ByVal UserID As Integer, ByVal IsActive As Boolean) Implements IIVS.EnableUser

        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                DataAccess.EnableUser(objThisEndPointProperty.Address, UserID, IsActive)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "EnableUser")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.EnableUser()" & ex.ToString)
        End Try

    End Sub

    Private Function IsUserNameAvailable(ByVal LicenseGuid As Guid, ByVal UserName As String) As Boolean Implements IIVS.IsUserNameAvailable

        Dim IsAvailable As Boolean = False
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                IsAvailable = DataAccess.IsUserNameAvailable(objThisEndPointProperty.Address, UserName)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "IsUserNameAvailable")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.IsUserNameAvailable()" & ex.ToString)
        End Try

        Return IsAvailable

    End Function

    Private Function IsUserAuthenticated(ByVal LicenseGuid As Guid, ByVal UserName As String, ByVal Password As String) As Integer Implements IIVS.IsUserAuthenticated

        Dim intUserID As Integer
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                intUserID = DataAccess.IsUserAuthenticated(objThisEndPointProperty.Address, UserName, Password)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "IsUserAuthenticated")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.IsUserAuthenticated()" & ex.ToString)
        End Try

        Return intUserID

    End Function

#End Region

#Region "tblAlerts"

    Private Function GetAlerts(ByVal LicenseGuid As Guid) As List(Of AlertDetail) Implements IIVS.GetAlerts

        Dim ListOfAlerts As New List(Of AlertDetail)
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                ListOfAlerts = DataAccess.GetAlerts(objThisEndPointProperty.Address)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "GetAlerts")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.GetAlerts()" & ex.ToString)
        End Try

        Return ListOfAlerts

    End Function

    Private Function GetAlertDetail(ByVal LicenseGuid As Guid, ByVal AlertID As Integer) As AlertDetail Implements IIVS.GetAlertDetail

        Dim objAlertDetail As New AlertDetail
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                objAlertDetail = DataAccess.GetAlertDetail(objThisEndPointProperty.Address, AlertID)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "GetAlertDetail")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.GetAlertDetail()" & ex.ToString)
        End Try

        Return objAlertDetail

    End Function

    Private Function GetSwipeScanAlerts(ByVal LicenseGuid As Guid, ByVal IDAccountNumber As String, ByVal NameFirst As String, ByVal NameLast As String) As List(Of AlertDetail) Implements IIVS.GetSwipeScanAlerts

        Dim ListOfAlerts As New List(Of AlertDetail)
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                ListOfAlerts = DataAccess.GetSwipeScanAlerts(objThisEndPointProperty.Address, IDAccountNumber, NameFirst, NameLast)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "GetSwipeScanAlerts")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.GetSwipeScanAlerts()" & ex.ToString)
        End Try

        Return ListOfAlerts

    End Function

    Private Sub NewAlert(ByVal LicenseGuid As Guid, ByVal AlertDetail As AlertDetail) Implements IIVS.NewAlert

        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                DataAccess.NewAlert(objThisEndPointProperty.Address, AlertDetail)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "NewAlert")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.NewAlert()" & ex.ToString)
        End Try

    End Sub

    Private Sub UpdateAlert(ByVal LicenseGuid As Guid, ByVal AlertDetail As AlertDetail) Implements IIVS.UpdateAlert

        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                DataAccess.UpdateAlert(objThisEndPointProperty.Address, AlertDetail)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "UpdateAlert")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.UpdateAlert()" & ex.ToString)
        End Try

    End Sub

    Private Sub DeleteAlert(ByVal LicenseGuid As Guid, ByVal AlertID As Integer) Implements IIVS.DeleteAlert

        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                DataAccess.DeleteAlert(objThisEndPointProperty.Address, AlertID)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "DeleteAlert")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.DeleteAlert()" & ex.ToString)
        End Try

    End Sub

    Private Sub EnableAlert(ByVal LicenseGuid As Guid, ByVal AlertID As Integer, ByVal IsActive As Boolean) Implements IIVS.EnableAlert

        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                DataAccess.EnableAlert(objThisEndPointProperty.Address, AlertID, IsActive)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "EnableAlert")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.EnableAlert()" & ex.ToString)
        End Try

    End Sub

#End Region

#Region "SwipeScans"

    Private Sub UpdateImageAvailable(ByVal LicenseGuid As Guid, ByVal SwipeScanID As Integer) Implements IIVS.UpdateImageAvailable

        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                DataAccess.UpdateImageAvailable(objThisEndPointProperty.Address, SwipeScanID)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "UpdateImageAvailable")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.UpdateImageAvailable()" & ex.ToString)
        End Try

    End Sub

    Private Function NewDataSwipeScan(ByVal LicenseGuid As Guid, ByVal SwipeScanInfo As SwipeScanInfo) As SwipeScanDetail Implements IIVS.NewDataSwipeScan

        Dim objSwipeScanDetail As New SwipeScanDetail
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                objSwipeScanDetail = DataAccess.NewDataSwipeScan(LicenseGuid, objThisEndPointProperty.Address, SwipeScanInfo)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "NewDataSwipeScan")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.NewDataSwipeScan()" & ex.ToString)
        End Try

        Return objSwipeScanDetail

    End Function

    Private Function NewDataSwipeScanManual(ByVal LicenseGuid As Guid, ByVal SwipeScanDetail As SwipeScanDetail) As Integer Implements IIVS.NewDataSwipeScanManual

        Dim intSwipeScanID As Integer
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                intSwipeScanID = DataAccess.NewDataSwipeScanManual(objThisEndPointProperty.Address, SwipeScanDetail)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "NewDataSwipeScanManual")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.NewDataSwipeScanManual()" & ex.ToString)
        End Try

        Return intSwipeScanID

    End Function

    Private Function GetSwipeScanDetail(ByVal LicenseGuid As Guid, ByVal SwipeScanID As Integer, ByVal SwipeScanType As String) As SwipeScanDetail Implements IIVS.GetSwipeScanDetail

        Dim objSwipeScanDetail As New SwipeScanDetail
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                objSwipeScanDetail = DataAccess.GetSwipeScanDetail(objThisEndPointProperty.Address, SwipeScanID, SwipeScanType)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "GetSwipeScanDetail")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.GetSwipeScanDetail()" & ex.ToString)
        End Try

        Return objSwipeScanDetail

    End Function

    Private Function GetSwipeScanHistory(ByVal LicenseGuid As Guid, ByVal IDAccountNumber As String, ByVal SwipeScanType As String) As List(Of SwipeScanHistory) Implements IIVS.GetSwipeScanHistory

        Dim ListOfSwipeScanHistory As New List(Of SwipeScanHistory)
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                ListOfSwipeScanHistory = DataAccess.GetSwipeScanHistory(objThisEndPointProperty.Address, IDAccountNumber, SwipeScanType)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "GetSwipeScanHistory")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.GetSwipeScanHistory()" & ex.ToString)
        End Try

        Return ListOfSwipeScanHistory

    End Function

    Private Function GetSwipeScanType(ByVal LicenseGuid As Guid, ByVal SwipeScanID As Integer) As String Implements IIVS.GetSwipeScanType

        Dim strSwipeScanType As String = Nothing
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                strSwipeScanType = DataAccess.GetSwipeScanType(objThisEndPointProperty.Address, SwipeScanID)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "GetSwipeScanType")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.GetSwipeScanType()" & ex.ToString)
        End Try

        Return strSwipeScanType

    End Function

    Private Function SwipeScanNavigateFirst(ByVal LicenseGuid As Guid, ByVal UserID As Integer, ByVal ClientID As Integer) As SwipeScanNavigateInfo Implements IIVS.SwipeScanNavigateFirst

        Dim objSwipeScanNavigateInfo As New SwipeScanNavigateInfo
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                objSwipeScanNavigateInfo = DataAccess.SwipeScanNavigateFirst(objThisEndPointProperty.Address, UserID, ClientID)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "SwipeScanNavigateFirst")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.SwipeScanNavigateFirst()" & ex.ToString)
        End Try

        Return objSwipeScanNavigateInfo

    End Function

    Private Function SwipeScanNavigatePrevious(ByVal LicenseGuid As Guid, ByVal UserID As Integer, ByVal ClientID As Integer, ByVal SwipeScanID As Integer) As SwipeScanNavigateInfo Implements IIVS.SwipeScanNavigatePrevious

        Dim objSwipeScanNavigateInfo As New SwipeScanNavigateInfo
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                objSwipeScanNavigateInfo = DataAccess.SwipeScanNavigatePrevious(objThisEndPointProperty.Address, UserID, ClientID, SwipeScanID)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "SwipeScanNavigatePrevious")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.SwipeScanNavigatePrevious()" & ex.ToString)
        End Try

        Return objSwipeScanNavigateInfo

    End Function

    Private Function SwipeScanNavigatePosition(ByVal LicenseGuid As Guid, ByVal SwipeScanID As Integer) As SwipeScanNavigateInfo Implements IIVS.SwipeScanNavigatePosition

        Dim objSwipeScanNavigateInfo As New SwipeScanNavigateInfo
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                objSwipeScanNavigateInfo = DataAccess.SwipeScanNavigatePosition(objThisEndPointProperty.Address, SwipeScanID)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "SwipeScanNavigatePosition")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.SwipeScanNavigatePosition()" & ex.ToString)
        End Try

        Return objSwipeScanNavigateInfo

    End Function

    Private Function SwipeScanNavigateNext(ByVal LicenseGuid As Guid, ByVal UserID As Integer, ByVal ClientID As Integer, ByVal SwipeScanID As Integer) As SwipeScanNavigateInfo Implements IIVS.SwipeScanNavigateNext

        Dim objSwipeScanNavigateInfo As New SwipeScanNavigateInfo
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                objSwipeScanNavigateInfo = DataAccess.SwipeScanNavigateNext(objThisEndPointProperty.Address, UserID, ClientID, SwipeScanID)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "SwipeScanNavigateNext")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.SwipeScanNavigateNext()" & ex.ToString)
        End Try

        Return objSwipeScanNavigateInfo

    End Function

    Private Function SwipeScanNavigateLast(ByVal LicenseGuid As Guid, ByVal UserID As Integer, ByVal ClientID As Integer) As SwipeScanNavigateInfo Implements IIVS.SwipeScanNavigateLast

        Dim objSwipeScanNavigateInfo As New SwipeScanNavigateInfo
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                objSwipeScanNavigateInfo = DataAccess.SwipeScanNavigateLast(objThisEndPointProperty.Address, UserID, ClientID)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "SwipeScanNavigateLast")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.SwipeScanNavigateLast()" & ex.ToString)
        End Try

        Return objSwipeScanNavigateInfo

    End Function

    Private Sub DeleteSwipeScan(ByVal LicenseGuid As Guid, ByVal SwipeScanID As Integer) Implements IIVS.DeleteSwipeScan

        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                DataAccess.DeleteSwipeScan(objThisEndPointProperty.Address, SwipeScanID)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "DeleteSwipeScan")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.DeleteSwipeScan()" & ex.ToString)
        End Try

    End Sub

    Private Sub UpdateCaseID(ByVal LicenseGuid As Guid, ByVal SwipeScanID As Integer, ByVal CaseID As String) Implements IIVS.UpdateCaseID

        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                DataAccess.UpdateCaseID(objThisEndPointProperty.Address, SwipeScanID, CaseID)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "UpdateCaseID")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.UpdateCaseID()" & ex.ToString)
        End Try

    End Sub

    Private Function GetSwipeScanSearch(ByVal LicenseGuid As Guid) As List(Of SwipeScanSearch) Implements IIVS.GetSwipeScanSearch

        Dim ListOfSwipeScanSearch As New List(Of SwipeScanSearch)
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                ListOfSwipeScanSearch = DataAccess.GetSwipeScanSearch(objThisEndPointProperty.Address)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "GetSwipeScanSearch")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.GetSwipeScanSearch()" & ex.ToString)
        End Try

        Return ListOfSwipeScanSearch

    End Function

#End Region

#Region "Custom"

    Private Function GetTEPViolations(ByVal LicenseGuid As Guid) As List(Of IVS.Data.GrandRapids.TEPViolations) Implements IIVS.GetTEPViolations

        Dim ListOfTEPViolations As New List(Of IVS.Data.GrandRapids.TEPViolations)
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                ListOfTEPViolations = IVS.Data.GrandRapids.DataAccess.GetTEPViolations(objThisEndPointProperty.Address)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "GetTEPViolations")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.GetTEPViolations()" & ex.ToString)
        End Try

        Return ListOfTEPViolations

    End Function

    Private Function GetTEPClientSettings(ByVal LicenseGuid As Guid) As IVS.Data.GrandRapids.TEPClientSettings Implements IIVS.GetTEPClientSettings

        Dim objTEPClientSettings As New IVS.Data.GrandRapids.TEPClientSettings
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)
       
            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                objTEPClientSettings = IVS.Data.GrandRapids.DataAccess.GetTEPClientSettings(objThisEndPointProperty.Address)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "GetTEPClientSettings")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.GetTEPClientSettings()" & ex.ToString)
        End Try

        Return objTEPClientSettings

    End Function

    Private Sub NewDataSwipeScan_TEP(ByVal LicenseGuid As Guid, ByVal TEPSwipeScanDetail As IVS.Data.GrandRapids.TEPSwipeScanDetail) Implements IIVS.NewDataSwipeScan_TEP

        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)
          
            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                IVS.Data.GrandRapids.DataAccess.NewDataSwipeScan_TEP(objThisEndPointProperty.Address, TEPSwipeScanDetail)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "NewDataSwipeScan_TEP")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.NewDataSwipeScan_TEP()" & ex.ToString)
        End Try

    End Sub

    Private Sub NewDataSwipeScan_Violation(ByVal LicenseGuid As Guid, ByVal TEPSwipeScanViolation As IVS.Data.GrandRapids.TEPSwipeScanViolations) Implements IIVS.NewDataSwipeScan_Violation

        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                IVS.Data.GrandRapids.DataAccess.NewDataSwipeScan_Violation(objThisEndPointProperty.Address, TEPSwipeScanViolation)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "NewDataSwipeScan_Violation")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.NewDataSwipeScan_Violation()" & ex.ToString)
        End Try

    End Sub

    Private Function GetSwipeScanDetail_TEP(ByVal LicenseGuid As Guid, ByVal SwipeScanID As Integer) As IVS.Data.GrandRapids.TEPSwipeScanDetail Implements IIVS.GetSwipeScanDetail_TEP

        Dim objTEPSwipeScanDetail As New IVS.Data.GrandRapids.TEPSwipeScanDetail
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                objTEPSwipeScanDetail = IVS.Data.GrandRapids.DataAccess.GetSwipeScanDetail_TEP(objThisEndPointProperty.Address, SwipeScanID)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "GetSwipeScanDetail_TEP")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.GetSwipeScanDetail_TEP()" & ex.ToString)
        End Try

        Return objTEPSwipeScanDetail

    End Function

    Private Function GetSwipeScanDetail_Violations(ByVal LicenseGuid As Guid, ByVal SwipeScanID As Integer) As List(Of IVS.Data.GrandRapids.TEPSwipeScanViolations) Implements IIVS.GetSwipeScanDetail_Violations

        Dim ListOfTEPSwipeScanViolations As New List(Of IVS.Data.GrandRapids.TEPSwipeScanViolations)
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                ListOfTEPSwipeScanViolations = IVS.Data.GrandRapids.DataAccess.GetSwipeScanDetail_Violations(objThisEndPointProperty.Address, SwipeScanID)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "GetSwipeScanDetail_Violations")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.GetSwipeScanDetail_Violations()" & ex.ToString)
        End Try

        Return ListOfTEPSwipeScanViolations

    End Function

    Private Function IsCitationUsedAlready(ByVal LicenseGuid As Guid, ByVal CitationID As String) As Boolean Implements IIVS.IsCitationUsedAlready

        Dim IsAvailable As Boolean = False
        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid) Then
                IsAvailable = DataAccess.IsCitationUsedAlready(objThisEndPointProperty.Address, CitationID)
            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "IsCitationUsedAlready")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVSService.IsCitationUsedAlready()" & ex.ToString)
        End Try

        Return IsAvailable

    End Function

#End Region

End Class
