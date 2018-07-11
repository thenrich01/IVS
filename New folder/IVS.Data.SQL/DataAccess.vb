Imports System.Data.SqlClient
Imports System.Text
Imports System.Configuration
Imports System.Net

Public Class DataAccess

    Public Shared MyAppLog As New IVS.AppLog.ApplicationLog(IVS.AppLog.ApplicationLog.DateEncodingType.YYYYMMDD, True)

    Public Shared Function OpenConnection() As SqlConnection

        Dim cnnString As String
        Dim cnn As SQLConnection
        Dim strName As String

        Try
            strName = String.Format("{0}.MySettings.IVSConnectionString", My.Application.Info.CompanyName)
            cnnString = ConfigurationManager.ConnectionStrings(strName).ConnectionString
            cnn = New SqlConnection(cnnString)

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        Return cnn

    End Function

#Region "tblClients"

    Public Shared Function NewClient() As Integer

        Dim cnn As New SqlConnection
        Dim sb As New StringBuilder
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim intClientID As Integer

        Try
            cnn = OpenConnection()

            sb.Append(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData))
            sb.Append("\")
            sb.Append(Trim(My.Application.Info.CompanyName))
            sb.Append("\")
            sb.Append(My.Application.Info.ProductName)
            sb.Append("\Images")

            strSQLCommand = "qryNewClient"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("ImageLocation", sb.ToString)
            cmd.Parameters.AddWithValue("ClientHostName", My.Computer.Name)
            cmd.Parameters.AddWithValue("ClientIPAddress", ReturnMyIPAddress)

            cnn.Open()
            cmd.ExecuteNonQuery()

            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT @@IDENTITY"
            intClientID = cmd.ExecuteScalar

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If
            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return intClientID

    End Function

    Public Shared Function GetClientID() As Integer

        Dim intClientID As Integer = 0
        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetClientID"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("ClientHostName", My.Computer.Name)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                intClientID = dr(0).ToString

            End While

            dr.Close()
            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return intClientID

    End Function

    Public Shared Function GetClients() As List(Of Clients)

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader
        Dim ListOfClients As New List(Of Clients)
        Dim objClient As Clients

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetClients"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cnn.Open()

            dr = cmd.ExecuteReader

            MyAppLog.WriteToLog("IVS.Data.GetClients() HasRows:" & dr.HasRows)

            While dr.Read

                objClient = New Clients
                objClient.ClientID = dr(0)
                objClient.Location = dr(1)
                objClient.Station = dr(2)
                ListOfClients.Add(objClient)

            End While

            dr.Close()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If dr IsNot Nothing Then
                dr.Dispose()
            End If

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return ListOfClients

    End Function

    Public Shared Function GetLocations() As List(Of Locations)

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader
        Dim ListOfLocations As New List(Of Locations)
        Dim objLocation As Locations

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetLocations"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cnn.Open()

            dr = cmd.ExecuteReader
            MyAppLog.WriteToLog("IVS.Data.GetLocations() HasRows:" & dr.HasRows)

            While dr.Read

                objLocation = New Locations
                objLocation.ClientID = dr(0)
                objLocation.Location = dr(1)
                ListOfLocations.Add(objLocation)

            End While

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If dr IsNot Nothing Then
                dr.Dispose()
            End If

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return ListOfLocations

    End Function

    Public Shared Sub UpdateClientIPAddress(ByVal ClientID As Integer)

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand

        Try

            cnn = OpenConnection()

            strSQLCommand = "qryUpdateClientIPAddress"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("ClientIPAddress", ReturnMyIPAddress)
            cmd.Parameters.AddWithValue("ClientID", ClientID)

            cnn.Open()

            cmd.ExecuteNonQuery()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

    End Sub

    Public Shared Function GetClientSettings(ByVal ClientID As Integer) As ClientSettings

        Dim objClientSettings As New ClientSettings
        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetClientSettings"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("ClientID", ClientID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objClientSettings = New ClientSettings
                objClientSettings.ClientID = ClientID
                objClientSettings.DeviceType = dr(0).ToString
                objClientSettings.DeviceID = dr(1).ToString
                objClientSettings.ComPort = dr(2).ToString
                objClientSettings.SleepMilliSeconds = dr(3).ToString
                objClientSettings.IDecodeLicense = dr(4).ToString
                objClientSettings.IDecodeTrackFormat = dr(5).ToString
                objClientSettings.IDecodeCardTypes = dr(6).ToString
                objClientSettings.Location = dr(7).ToString
                objClientSettings.Station = dr(8).ToString
                objClientSettings.Phone = dr(9).ToString
                objClientSettings.Email = dr(10).ToString
                objClientSettings.SkipLogon = dr(11).ToString
                objClientSettings.DisplayAdmin = dr(12).ToString
                objClientSettings.DefaultUser = dr(13).ToString
                objClientSettings.AgeHighlight = dr(14).ToString
                objClientSettings.AgePopup = dr(15).ToString
                objClientSettings.Age = dr(16).ToString
                objClientSettings.ImageSave = dr(17).ToString
                objClientSettings.ImageLocation = dr(18).ToString
                objClientSettings.ViewingTime = dr(19).ToString
                objClientSettings.CCDigits = dr(20).ToString
                objClientSettings.DisableCCSave = dr(21).ToString
                objClientSettings.DisableDBSave = dr(22).ToString
                objClientSettings.LogRetention = dr(23).ToString

            End While

            dr.Close()
            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return objClientSettings

    End Function

    Public Shared Sub SaveClientSettings(ByVal NewClientSettings As ClientSettings)

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryUpdateClientSettings"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("Location", NewClientSettings.Location)
            cmd.Parameters.AddWithValue("Station", NewClientSettings.Station)
            cmd.Parameters.AddWithValue("Phone", NewClientSettings.Phone)
            cmd.Parameters.AddWithValue("Email", NewClientSettings.Email)
            cmd.Parameters.AddWithValue("SkipLogon", NewClientSettings.SkipLogon)
            cmd.Parameters.AddWithValue("DisplayAdmin", NewClientSettings.DisplayAdmin)
            cmd.Parameters.AddWithValue("DefaultUser", NewClientSettings.DefaultUser)
            cmd.Parameters.AddWithValue("AgeHighlight", NewClientSettings.AgeHighlight)
            cmd.Parameters.AddWithValue("AgePopup", NewClientSettings.AgePopup)
            cmd.Parameters.AddWithValue("Age", NewClientSettings.Age)
            cmd.Parameters.AddWithValue("ImageSave", NewClientSettings.ImageSave)
            cmd.Parameters.AddWithValue("ImageLocation", NewClientSettings.ImageLocation)
            cmd.Parameters.AddWithValue("ViewingTime", NewClientSettings.ViewingTime)
            cmd.Parameters.AddWithValue("CCDigits", NewClientSettings.CCDigits)
            cmd.Parameters.AddWithValue("DisableCCSave", NewClientSettings.DisableCCSave)
            cmd.Parameters.AddWithValue("DisableDBSave", NewClientSettings.DisableDBSave)
            cmd.Parameters.AddWithValue("ClientID", NewClientSettings.ClientID)

            cnn.Open()

            cmd.ExecuteNonQuery()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

    End Sub

    Public Shared Sub UpdateIDecodeLicense(ByVal IDecodeLicenseInfo As IDecodeLicenseInfo)

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryUpdateIDecodeLicense"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("IDecodeLicense", IDecodeLicenseInfo.IDecodeLicense)
            cmd.Parameters.AddWithValue("IDecodeVersion", IDecodeLicenseInfo.Version)
            cmd.Parameters.AddWithValue("ClientID", IDecodeLicenseInfo.ClientID)

            cnn.Open()

            cmd.ExecuteNonQuery()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

    End Sub

    Public Shared Function GetStationName(ByVal ClientID As Integer) As String

        Dim strStationName As String
        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetStationName"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("ClientID", ClientID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                strStationName = dr(0).ToString

            End While

            dr.Close()
            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return strStationName

    End Function

#End Region

#Region "tblDevices"

    Public Shared Function GetDeviceInfo(ByVal ClientID As Integer) As DeviceInfo

        Dim objDeviceInfo As New DeviceInfo
        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetDeviceInfo"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("ClientID", ClientID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objDeviceInfo.DeviceID = dr(0).ToString
                objDeviceInfo.DeviceType = dr(1).ToString
                objDeviceInfo.ModelNo = dr(2).ToString
                objDeviceInfo.SerialNo = dr(3).ToString
                objDeviceInfo.FirmwareRev = dr(4).ToString
                objDeviceInfo.FirmwareDate = dr(5).ToString
                objDeviceInfo.HardwareRev = dr(6).ToString
                objDeviceInfo.ComPort = dr(7).ToString
                objDeviceInfo.UpdateTS = dr(8).ToString

            End While

            dr.Close()
            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return objDeviceInfo

    End Function

    Public Shared Sub UpdateDevice(ByVal DeviceInfo As DeviceInfo)

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand

        Try

            MyAppLog.WriteToLog("IVS.Data.UpdateDevice() DeviceType: " & DeviceInfo.DeviceType)
            MyAppLog.WriteToLog("IVS.Data.UpdateDevice() Model: " & DeviceInfo.ModelNo)
            MyAppLog.WriteToLog("IVS.Data.UpdateDevice() FirmwareVersion: " & DeviceInfo.FirmwareRev)
            MyAppLog.WriteToLog("IVS.Data.UpdateDevice() FirmwareDate: " & DeviceInfo.FirmwareDate)
            MyAppLog.WriteToLog("IVS.Data.UpdateDevice() SerialNo: " & DeviceInfo.SerialNo)
            MyAppLog.WriteToLog("IVS.Data.UpdateDevice() DeviceID: " & DeviceInfo.DeviceID)
            MyAppLog.WriteToLog("IVS.Data.UpdateDevice() ClientID: " & DeviceInfo.ClientID)
            MyAppLog.WriteToLog("IVS.Data.UpdateDevice() ComPort: " & DeviceInfo.ComPort)

            cnn = OpenConnection()

            If DeviceInfo.DeviceID > 0 Then

                strSQLCommand = "qryUpdateDevice"

                cmd = New SqlCommand(strSQLCommand, cnn)
                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.AddWithValue("DeviceType", DeviceInfo.DeviceType)
                cmd.Parameters.AddWithValue("ModelNo", DeviceInfo.ModelNo)
                cmd.Parameters.AddWithValue("SerialNo", DeviceInfo.SerialNo)
                cmd.Parameters.AddWithValue("FirmwareRev", DeviceInfo.FirmwareRev)
                cmd.Parameters.AddWithValue("FirmwareDate", DeviceInfo.FirmwareDate)
                cmd.Parameters.AddWithValue("HardwareRev", DeviceInfo.HardwareRev)
                cmd.Parameters.AddWithValue("DeviceID", DeviceInfo.DeviceID)

            Else
                strSQLCommand = "qryNewDevice"

                cmd = New SqlCommand(strSQLCommand, cnn)
                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.AddWithValue("DeviceType", DeviceInfo.DeviceType)
                cmd.Parameters.AddWithValue("ModelNo", DeviceInfo.ModelNo)
                cmd.Parameters.AddWithValue("SerialNo", DeviceInfo.SerialNo)
                cmd.Parameters.AddWithValue("FirmwareRev", DeviceInfo.FirmwareRev)
                cmd.Parameters.AddWithValue("FirmwareDate", DeviceInfo.FirmwareDate)
                cmd.Parameters.AddWithValue("HardwareRev", DeviceInfo.HardwareRev)
                cmd.Parameters.AddWithValue("ClientID", DeviceInfo.ClientID)

            End If

            cnn.Open()

            cmd.ExecuteNonQuery()

            If DeviceInfo.DeviceID < 1 Then

                cmd.CommandType = CommandType.Text
                cmd.CommandText = "SELECT @@IDENTITY"
                DeviceInfo.DeviceID = cmd.ExecuteScalar

            End If

            strSQLCommand = "qryUpdateClientDevice"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("DeviceType", DeviceInfo.DeviceType)
            cmd.Parameters.AddWithValue("DeviceID", DeviceInfo.DeviceID)
            cmd.Parameters.AddWithValue("ComPort", DeviceInfo.ComPort)
            cmd.Parameters.AddWithValue("ClientID", DeviceInfo.ClientID)

            cmd.ExecuteNonQuery()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

    End Sub

#End Region

#Region "tblUsers"

    Public Shared Function GetUsers() As List(Of UserDetail)

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader
        Dim ListOfUsers As New List(Of UserDetail)
        Dim objUsers As UserDetail

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetUsers"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cnn.Open()

            dr = cmd.ExecuteReader

            MyAppLog.WriteToLog("IVS.Data.GetUsers() HasRows:" & dr.HasRows)

            While dr.Read

                objUsers = New UserDetail
                objUsers.UserID = dr(0).ToString
                objUsers.UserName = dr(1).ToString
                objUsers.UserNameFirst = dr(2).ToString
                objUsers.UserNameLast = dr(3).ToString
                objUsers.ActiveFlag = dr(4).ToString
                ListOfUsers.Add(objUsers)

            End While

            dr.Close()
            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If dr IsNot Nothing Then
                dr.Dispose()
            End If

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return ListOfUsers

    End Function

    Public Shared Function GetUserNames() As List(Of UserDetail)

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader
        Dim ListOfUsers As New List(Of UserDetail)
        Dim objUsers As UserDetail

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetUserNames"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cnn.Open()

            dr = cmd.ExecuteReader
            MyAppLog.WriteToLog("IVS.Data.GetUserNames() HasRows:" & dr.HasRows)

            While dr.Read

                objUsers = New UserDetail
                objUsers.UserID = dr(0).ToString
                objUsers.UserName = dr(1).ToString
                objUsers.UserNameFirst = dr(2).ToString
                ListOfUsers.Add(objUsers)

            End While

            dr.Close()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If dr IsNot Nothing Then
                dr.Dispose()
            End If

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return ListOfUsers

    End Function

    Public Shared Function GetUserName(ByVal UserID As Integer) As String

        Dim strUserName As String
        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetUserName"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("UserID", UserID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                strUserName = dr(0).ToString

            End While

            dr.Close()
            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return strUserName

    End Function

    Public Shared Function GetUserPhone(ByVal UserID As Integer) As String

        Dim strUserPhone As String
        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetUserPhone"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("UserID", UserID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                strUserPhone = dr(0).ToString

            End While

            dr.Close()
            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return strUserPhone

    End Function

    Public Shared Function GetUserDetail(ByVal UserID As Integer) As UserDetail

        Dim objUserDetail As New UserDetail
        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetUserDetail"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("UserID", UserID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objUserDetail.UserID = UserID
                objUserDetail.UserName = dr(0).ToString
                objUserDetail.Password = dr(1).ToString
                objUserDetail.UserNameFirst = dr(2).ToString
                objUserDetail.UserNameLast = dr(3).ToString
                objUserDetail.UserEmail = dr(4).ToString
                objUserDetail.UserPhone = dr(5).ToString
                objUserDetail.AdminFlag = dr(6).ToString
                objUserDetail.AlertFlag = dr(7).ToString
                objUserDetail.SearchFlag = dr(8).ToString
                objUserDetail.ActiveFlag = dr(9).ToString
                objUserDetail.UpdateTS = dr(10).ToString

            End While

            dr.Close()
            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return objUserDetail

    End Function

    Public Shared Sub NewUser(ByVal UserDetail As UserDetail)

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryNewUser"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("UserName", UserDetail.UserName)
            cmd.Parameters.AddWithValue("Password", UserDetail.Password)
            cmd.Parameters.AddWithValue("UserNameFirst", UserDetail.UserNameFirst)
            cmd.Parameters.AddWithValue("UserNameLast", UserDetail.UserNameLast)
            cmd.Parameters.AddWithValue("UserEmail", UserDetail.UserEmail)
            cmd.Parameters.AddWithValue("UserPhone", UserDetail.UserPhone)
            cmd.Parameters.AddWithValue("AdminFlag", UserDetail.AdminFlag)
            cmd.Parameters.AddWithValue("AlertFlag", UserDetail.AlertFlag)
            cmd.Parameters.AddWithValue("SearchFlag", UserDetail.SearchFlag)
            cmd.Parameters.AddWithValue("ActiveFlag", UserDetail.ActiveFlag)

            cnn.Open()

            cmd.ExecuteNonQuery()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

    End Sub

    Public Shared Sub UpdateUser(ByVal UserDetail As UserDetail)

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryUpdateUser"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("UserName", UserDetail.UserName)
            cmd.Parameters.AddWithValue("Password", UserDetail.Password)
            cmd.Parameters.AddWithValue("UserNameFirst", UserDetail.UserNameFirst)
            cmd.Parameters.AddWithValue("UserNameLast", UserDetail.UserNameLast)
            cmd.Parameters.AddWithValue("UserEmail", UserDetail.UserEmail)
            cmd.Parameters.AddWithValue("UserPhone", UserDetail.UserPhone)
            cmd.Parameters.AddWithValue("AdminFlag", UserDetail.AdminFlag)
            cmd.Parameters.AddWithValue("AlertFlag", UserDetail.AlertFlag)
            cmd.Parameters.AddWithValue("SearchFlag", UserDetail.SearchFlag)
            cmd.Parameters.AddWithValue("ActiveFlag", UserDetail.ActiveFlag)
            cmd.Parameters.AddWithValue("UserID", UserDetail.UserID)

            cnn.Open()

            cmd.ExecuteNonQuery()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

    End Sub

    Public Shared Sub DeleteUser(ByVal UserID As Integer)

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand

        Try

            cnn = OpenConnection()

            strSQLCommand = "qryDeleteUser"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("UserID", UserID)

            cnn.Open()

            cmd.ExecuteNonQuery()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

    End Sub

    Public Shared Sub EnableUser(ByVal UserID As Integer, ByVal IsActive As Boolean)

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand

        Try

            cnn = OpenConnection()

            strSQLCommand = "qryEnableUser"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("ActiveFlag", IsActive)
            cmd.Parameters.AddWithValue("UserID", UserID)

            cnn.Open()

            cmd.ExecuteNonQuery()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

    End Sub

    Public Shared Function IsUserNameAvailable(ByVal UserName As String) As Boolean

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader
        Dim intRecordCount As Integer
        Dim IsAvailable As Boolean = False

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryIsUserNameAvailable"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("UserName", UserName)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                intRecordCount = dr(0).ToString

            End While

            dr.Close()
            cnn.Close()

            If intRecordCount = 0 Then
                IsAvailable = True
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return IsAvailable

    End Function

    Public Shared Function IsUserAuthenticated(ByVal UserName As String, ByVal Password As String) As Integer

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader
        Dim intUserID As Integer = 0

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryIsUserAuthenticated"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("UserName", UserName)
            cmd.Parameters.AddWithValue("Password", Password)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                intUserID = dr(0).ToString

            End While

            dr.Close()
            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return intUserID

    End Function

#End Region

#Region "tblAlerts"

    Public Shared Function GetAlerts() As List(Of AlertDetail)

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader
        Dim ListOfAlertDetail As New List(Of AlertDetail)
        Dim objAlertDetail As AlertDetail

        Try

            cnn = OpenConnection()

            strSQLCommand = "qryGetAlerts"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cnn.Open()

            dr = cmd.ExecuteReader

            MyAppLog.WriteToLog("IVS.Data.GetAlerts() HasRows:" & dr.HasRows)

            While dr.Read

                objAlertDetail = New AlertDetail
                objAlertDetail.AlertID = dr(0).ToString
                objAlertDetail.AlertType = dr(1).ToString
                objAlertDetail.IDNumber = dr(2).ToString
                objAlertDetail.NameLast = dr(3).ToString
                objAlertDetail.DateOfBirth = dr(4).ToString
                objAlertDetail.ActiveFlag = dr(5).ToString
                objAlertDetail.UpdateTS = dr(6).ToString
                ListOfAlertDetail.Add(objAlertDetail)

            End While

            dr.Close()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If dr IsNot Nothing Then
                dr.Dispose()
            End If

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return ListOfAlertDetail

    End Function

    Public Shared Function GetAlertDetail(ByVal AlertID As Integer) As AlertDetail

        Dim objAlertDetail As New AlertDetail
        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetAlertDetail"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("AlertID", AlertID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objAlertDetail.AlertID = AlertID
                objAlertDetail.AlertType = dr(0).ToString
                objAlertDetail.IDNumber = dr(1).ToString
                objAlertDetail.NameFirst = dr(2).ToString
                objAlertDetail.NameLast = dr(3).ToString
                objAlertDetail.DateOfBirth = dr(4).ToString
                objAlertDetail.AlertContactName = dr(5).ToString
                objAlertDetail.AlertContactNumber = dr(6).ToString
                objAlertDetail.AlertNotes = dr(7).ToString
                objAlertDetail.ActiveFlag = dr(8).ToString
                objAlertDetail.UserID = dr(9).ToString
                objAlertDetail.UserName = dr(10).ToString
                objAlertDetail.UpdateTS = dr(11).ToString

            End While

            dr.Close()
            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return objAlertDetail

    End Function

    Public Shared Function GetSwipeScanAlerts(ByVal IDAccountNumber As String, ByVal NameFirst As String, ByVal NameLast As String) As List(Of AlertDetail)

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader
        Dim ListOfAlertDetail As New List(Of AlertDetail)
        Dim objAlertDetail As AlertDetail

        Try
            If IDAccountNumber.Contains(":") Then
                NameFirst = DBNull.Value.ToString
                NameLast = DBNull.Value.ToString
            End If

            cnn = OpenConnection()

            strSQLCommand = "qryGetSwipeScanAlerts"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("IDNumber", IDAccountNumber)
            cmd.Parameters.AddWithValue("NameFirst", NameFirst)
            cmd.Parameters.AddWithValue("NameLast", NameLast)

            cnn.Open()

            dr = cmd.ExecuteReader

            MyAppLog.WriteToLog("IVS.Data.GetSwipeScanAlerts() HasRows:" & dr.HasRows)

            While dr.Read

                objAlertDetail = New AlertDetail
                objAlertDetail.AlertID = dr(0).ToString
                objAlertDetail.AlertType = dr(1).ToString
                objAlertDetail.UpdateTS = dr(2).ToString
                objAlertDetail.NameFirst = dr(3).ToString
                objAlertDetail.NameLast = dr(4).ToString
                objAlertDetail.IDNumber = dr(5).ToString
                objAlertDetail.MatchLast = dr(6).ToString
                objAlertDetail.MatchID = dr(7).ToString
                ListOfAlertDetail.Add(objAlertDetail)

            End While

            dr.Close()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If dr IsNot Nothing Then
                dr.Dispose()
            End If

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return ListOfAlertDetail

    End Function

    Public Shared Sub NewAlert(ByVal AlertDetail As AlertDetail)

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryNewAlert"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("AlertType", AlertDetail.AlertType)
            cmd.Parameters.AddWithValue("IDNumber", AlertDetail.IDNumber)
            cmd.Parameters.AddWithValue("NameFirst", AlertDetail.NameFirst)
            cmd.Parameters.AddWithValue("NameLast", AlertDetail.NameLast)
            cmd.Parameters.AddWithValue("DateOfBirth", AlertDetail.DateOfBirth)
            cmd.Parameters.AddWithValue("AlertContactName", AlertDetail.AlertContactName)
            cmd.Parameters.AddWithValue("AlertContactNumber", AlertDetail.AlertContactNumber)
            cmd.Parameters.AddWithValue("AlertNotes", AlertDetail.AlertNotes)
            cmd.Parameters.AddWithValue("ActiveFlag", AlertDetail.ActiveFlag)
            cmd.Parameters.AddWithValue("UserID", AlertDetail.UserID)

            cnn.Open()

            cmd.ExecuteNonQuery()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

    End Sub

    Public Shared Sub UpdateAlert(ByVal AlertDetail As AlertDetail)

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryUpdateAlert"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("IDNumber", AlertDetail.IDNumber)
            cmd.Parameters.AddWithValue("NameFirst", AlertDetail.NameFirst)
            cmd.Parameters.AddWithValue("NameLast", AlertDetail.NameLast)
            cmd.Parameters.AddWithValue("DateOfBirth", AlertDetail.DateOfBirth)
            cmd.Parameters.AddWithValue("AlertContactName", AlertDetail.AlertContactName)
            cmd.Parameters.AddWithValue("AlertContactNumber", AlertDetail.AlertContactNumber)
            cmd.Parameters.AddWithValue("AlertNotes", AlertDetail.AlertNotes)
            cmd.Parameters.AddWithValue("ActiveFlag", AlertDetail.ActiveFlag)
            cmd.Parameters.AddWithValue("UserID", AlertDetail.UserID)
            cmd.Parameters.AddWithValue("AlertID", AlertDetail.AlertID)

            cnn.Open()

            cmd.ExecuteNonQuery()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

    End Sub

    Public Shared Sub DeleteAlert(ByVal AlertID As Integer)

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryDeleteAlert"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("AlertID", AlertID)

            cnn.Open()

            cmd.ExecuteNonQuery()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

    End Sub

    Public Shared Sub EnableAlert(ByVal AlertID As Integer, ByVal IsActive As Boolean)

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand

        Try

            cnn = OpenConnection()

            strSQLCommand = "qryEnableAlert"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("ActiveFlag", IsActive)
            cmd.Parameters.AddWithValue("AlertID", AlertID)

            cnn.Open()

            cmd.ExecuteNonQuery()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

    End Sub

#End Region

#Region "SwipeScans"

    Public Shared Sub UpdateImageAvailable(ByVal SwipeScanID As Integer)

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand

        Try

            cnn = OpenConnection()

            strSQLCommand = "qryUpdateImageAvailable"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanID)

            cnn.Open()

            cmd.ExecuteNonQuery()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

    End Sub

    Public Shared Function NewDataSwipeScan(ByVal SwipeScanInfo As SwipeScanInfo) As SwipeScanDetail

        Dim objSwipeScanDetail As New SwipeScanDetail
        Dim intDataToProcessStatus As Integer
        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim objProcessedData As New IDecode.Net.PC.stAllData
        Dim intCardType As Integer = 9
        Dim strCardType As String
        Dim intSwipeScanID As Integer
        Dim sbRawData As New StringBuilder
        Dim strDataSource As String

        Try
            'MyAppLog.WriteToLog("NewDataSwipeScan(): " & SwipeScanInfo.SwipeScanRawData)

            If SwipeScanInfo.ScanType <> "C" Then

                If Left(SwipeScanInfo.SwipeScanRawData, 1) = "t" Then

                    strDataSource = "Magnetic"

                    Dim strSwipeTrackData() As String

                    strSwipeTrackData = SwipeScanInfo.SwipeScanRawData.Split("t")

                    sbRawData.Append("%")
                    sbRawData.Append(strSwipeTrackData(1))
                    sbRawData.Append("?;")
                    sbRawData.Append(strSwipeTrackData(2))
                    sbRawData.Append("?%")
                    sbRawData.Append(strSwipeTrackData(3))

                    If strSwipeTrackData(2).Contains("?") = False Then
                        sbRawData.Append("?;")
                    End If

                ElseIf Left(SwipeScanInfo.SwipeScanRawData, 1) = "@" Then

                    strDataSource = "Barcode"
                    sbRawData.Append(SwipeScanInfo.SwipeScanRawData)

                ElseIf Left(SwipeScanInfo.SwipeScanRawData, 1) = "%" Then

                    strDataSource = "Magnetic"
                    sbRawData.Append(SwipeScanInfo.SwipeScanRawData)

                End If

                'Not a check

                intDataToProcessStatus = SwipeScanInfo.IDChecker.ProcessData(sbRawData.ToString)
                MyAppLog.WriteToLog("ProcessData(): " & intDataToProcessStatus)

                intCardType = SwipeScanInfo.IDChecker.GetCardType
                'ProcessData Status: 0=returned,1=NULL, Negative=ERROR
            Else

                strDataSource = "MICR"

            End If

            Select Case intCardType
                Case 0
                    strCardType = "Drivers License Or State ID"

                Case 1
                    strCardType = "Credit Card"

                Case 2
                    strCardType = "Military ID Card"

                Case 3
                    strCardType = "INS Employee Authorization Card"

                Case 9
                    strCardType = "Check"

            End Select

            MyAppLog.WriteToLog("intCardType(): " & intCardType & " : " & strCardType)

            If SwipeScanInfo.DisableDBSave = False Then

                cnn = OpenConnection()

                strSQLCommand = "qryNewDataSwipeScan"

                cmd = New SqlCommand(strSQLCommand, cnn)
                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.AddWithValue("SwipeScanType", strCardType)
                cmd.Parameters.AddWithValue("ClientID", SwipeScanInfo.ClientID)
                cmd.Parameters.AddWithValue("UserID", SwipeScanInfo.UserID)
                cmd.Parameters.AddWithValue("DataSource", strDataSource)

                cnn.Open()

                cmd.ExecuteNonQuery()

                cmd.CommandText = "SELECT @@IDENTITY"
                cmd.CommandType = CommandType.Text
                intSwipeScanID = cmd.ExecuteScalar

                cnn.Close()
            Else
                intSwipeScanID = 0
            End If



            MyAppLog.WriteToLog("IVS.Data.NewDataSwipeScan()intSwipeScanID:" & intSwipeScanID)

            Select Case intCardType
                Case 0
                    objSwipeScanDetail = NewDataSwipeScan_DLID(intSwipeScanID, sbRawData.ToString, strDataSource, SwipeScanInfo)
                Case 1
                    objSwipeScanDetail = NewDataSwipeScan_CC(intSwipeScanID, sbRawData.ToString, strDataSource, SwipeScanInfo)
                Case 2
                    objSwipeScanDetail = NewDataSwipeScan_MID(intSwipeScanID, sbRawData.ToString, strDataSource, SwipeScanInfo)
                Case 3
                    objSwipeScanDetail = NewDataSwipeScan_INS(intSwipeScanID, sbRawData.ToString, strDataSource, SwipeScanInfo)
                Case 9
                    objSwipeScanDetail = NewDataSwipeScan_CK(intSwipeScanID, SwipeScanInfo.SwipeScanRawData, strDataSource, SwipeScanInfo)
            End Select

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return objSwipeScanDetail

    End Function

    Private Shared Function NewDataSwipeScan_DLID(ByVal SwipeScanID As Integer, ByVal SwipeScanRawData As String, ByVal DataSource As String, ByVal SwipeScanInfo As SwipeScanInfo) As SwipeScanDetail

        Dim objSwipeScanDetail As New SwipeScanDetail
        Dim objProcessedData As New IDecode.Net.PC.stAllData
        Dim strIssueDate As String
        Dim strExpirationDate As String
        Dim strBirthDate As String
        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand

        Try
            objProcessedData = SwipeScanInfo.IDChecker.AllData

            strIssueDate = objProcessedData.IssueDate.Text

            If Len(strIssueDate) = 8 Then
                strIssueDate = strIssueDate.Substring(0, 2) & "/" & strIssueDate.Substring(2, 2) & "/" & strIssueDate.Substring(4, 4)
            End If

            strExpirationDate = objProcessedData.ExpiryDate.Text

            If Len(strExpirationDate) = 8 Then
                strExpirationDate = strExpirationDate.Substring(0, 2) & "/" & strExpirationDate.Substring(2, 2) & "/" & strExpirationDate.Substring(4, 4)
            End If

            strBirthDate = objProcessedData.BirthDate.Text

            If Len(strBirthDate) = 8 Then
                strBirthDate = strBirthDate.Substring(0, 2) & "/" & strBirthDate.Substring(2, 2) & "/" & strBirthDate.Substring(4, 4)
            End If

            If SwipeScanInfo.DisableDBSave = False Then

                cnn = OpenConnection()

                strSQLCommand = "qryNewDataSwipeScan_DLID"

                cmd = New SqlCommand(strSQLCommand, cnn)
                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanID)
                cmd.Parameters.AddWithValue("IDNumber", objProcessedData.Id.Text)
                cmd.Parameters.AddWithValue("NameFirst", objProcessedData.Name.First.Text)
                cmd.Parameters.AddWithValue("NameLast", objProcessedData.Name.Last.Text)
                cmd.Parameters.AddWithValue("NameMiddle", objProcessedData.Name.Middle.Text)
                cmd.Parameters.AddWithValue("DateOfBirth", strBirthDate)
                cmd.Parameters.AddWithValue("Age", objProcessedData.Age)
                cmd.Parameters.AddWithValue("Sex", objProcessedData.Sex.Text)
                cmd.Parameters.AddWithValue("Height", objProcessedData.Height.FeetInches)
                cmd.Parameters.AddWithValue("Weight", objProcessedData.Weight.Pounds)
                cmd.Parameters.AddWithValue("Eyes", objProcessedData.Eye.Color)
                cmd.Parameters.AddWithValue("Hair", objProcessedData.Hair.Color)
                cmd.Parameters.AddWithValue("DateOfIssue", strIssueDate)
                cmd.Parameters.AddWithValue("DateOfExpiration", strExpirationDate)
                cmd.Parameters.AddWithValue("AddressStreet", objProcessedData.MailAddress.Line1)
                cmd.Parameters.AddWithValue("AddressCity", objProcessedData.MailAddress.City.Text)
                cmd.Parameters.AddWithValue("AddressState", objProcessedData.MailAddress.State.Text)
                cmd.Parameters.AddWithValue("AddressZip", objProcessedData.MailAddress.Zip.Text)
                cmd.Parameters.AddWithValue("SwipeRawData", SwipeScanRawData)

                cnn.Open()

                cmd.ExecuteNonQuery()

                cnn.Close()

            End If

            objSwipeScanDetail.SwipeScanID = SwipeScanID
            objSwipeScanDetail.IDAccountNumber = objProcessedData.Id.Text
            objSwipeScanDetail.NameFirst = objProcessedData.Name.First.Text
            objSwipeScanDetail.NameLast = objProcessedData.Name.Last.Text
            objSwipeScanDetail.NameMiddle = objProcessedData.Name.Middle.Text
            objSwipeScanDetail.DateOfBirth = strBirthDate
            objSwipeScanDetail.Age = objProcessedData.Age
            objSwipeScanDetail.Sex = objProcessedData.Sex.Text
            objSwipeScanDetail.Height = objProcessedData.Height.FeetInches
            objSwipeScanDetail.Weight = objProcessedData.Weight.Pounds
            objSwipeScanDetail.Eyes = objProcessedData.Eye.Color
            objSwipeScanDetail.Hair = objProcessedData.Hair.Color
            objSwipeScanDetail.DateOfIssue = strIssueDate
            objSwipeScanDetail.DateOfExpiration = strExpirationDate
            objSwipeScanDetail.AddressStreet = objProcessedData.MailAddress.Line1
            objSwipeScanDetail.AddressCity = objProcessedData.MailAddress.City.Text
            objSwipeScanDetail.AddressState = objProcessedData.MailAddress.State.Text
            objSwipeScanDetail.AddressZip = objProcessedData.MailAddress.Zip.Text
            objSwipeScanDetail.SwipeRawData = SwipeScanRawData
            objSwipeScanDetail.CardType = "Drivers License Or State ID"
            objSwipeScanDetail.DataSource = DataSource
            objSwipeScanDetail.UpdateTS = Now

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return objSwipeScanDetail

    End Function

    Private Shared Function NewDataSwipeScan_CC(ByVal SwipeScanID As Integer, ByVal SwipeScanRawData As String, ByVal DataSource As String, SwipeScanInfo As SwipeScanInfo) As SwipeScanDetail

        Dim objSwipeScanDetail As New SwipeScanDetail
        Dim objProcessedData As New IDecode.Net.PC.stCCItems
        Dim strExpirationDate As String
        Dim strCCNumber As String
        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand

        Try
            objProcessedData = SwipeScanInfo.IDChecker.CreditCardItems

            strExpirationDate = objProcessedData.Expiration.Date

            If Len(strExpirationDate) = 8 Then
                strExpirationDate = strExpirationDate.Substring(0, 2) & "/" & strExpirationDate.Substring(2, 2) & "/" & strExpirationDate.Substring(4, 4)
            End If

            If strExpirationDate = "unknown" Then
                strExpirationDate = Nothing
            End If

            strCCNumber = Trim(objProcessedData.Account.Text)

            If SwipeScanInfo.CCDigits > 0 Then

                Dim sb As New StringBuilder

                For i = 1 To strCCNumber.Length - SwipeScanInfo.CCDigits

                    sb.Append("X")

                Next

                sb.Append(strCCNumber.Substring(strCCNumber.Length - SwipeScanInfo.CCDigits, SwipeScanInfo.CCDigits))

                strCCNumber = sb.ToString

            End If

            If SwipeScanInfo.DisableCCSave = True Then

                SwipeScanInfo.SwipeScanRawData = Nothing

            End If

            If SwipeScanInfo.DisableDBSave = False Then

                cnn = OpenConnection()

                strSQLCommand = "qryNewDataSwipeScan_CC"

                cmd = New SqlCommand(strSQLCommand, cnn)
                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanID)
                cmd.Parameters.AddWithValue("CCNumber", strCCNumber)
                cmd.Parameters.AddWithValue("CCIssuer", objProcessedData.Issuer.Text)
                cmd.Parameters.AddWithValue("NameFirst", objProcessedData.Name.First)
                cmd.Parameters.AddWithValue("NameLast", objProcessedData.Name.Last)
                cmd.Parameters.AddWithValue("NameMiddle", objProcessedData.Name.Middle)
                cmd.Parameters.AddWithValue("DateOfExpiration", strExpirationDate)
                cmd.Parameters.AddWithValue("SwipeRawData", SwipeScanRawData)

                cnn.Open()

                cmd.ExecuteNonQuery()

                cnn.Close()

            End If

            objSwipeScanDetail.SwipeScanID = SwipeScanID
            objSwipeScanDetail.IDAccountNumber = strCCNumber
            objSwipeScanDetail.NameFirst = objProcessedData.Name.First
            objSwipeScanDetail.NameLast = objProcessedData.Name.Last
            objSwipeScanDetail.NameMiddle = objProcessedData.Name.Middle
            objSwipeScanDetail.DateOfExpiration = strExpirationDate
            objSwipeScanDetail.SwipeRawData = SwipeScanRawData
            objSwipeScanDetail.CardType = "Credit Card"
            objSwipeScanDetail.DataSource = DataSource
            objSwipeScanDetail.UpdateTS = Now

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return objSwipeScanDetail

    End Function

    Private Shared Function NewDataSwipeScan_MID(ByVal SwipeScanID As Integer, ByVal SwipeScanRawData As String, ByVal DataSource As String, ByVal SwipeScanInfo As SwipeScanInfo) As SwipeScanDetail

        Dim objSwipeScanDetail As New SwipeScanDetail
        Dim objProcessedData As New IDecode.Net.PC.stDODItems
        Dim strIssueDate As String
        Dim strExpirationDate As String
        Dim strBirthDate As String
        Dim intAge As Integer = 0
        Dim cnn As New SqlConnection
        Dim sb As New StringBuilder
        Dim strSQLCommand As String
        Dim cmd As SqlCommand

        Try
            objProcessedData = SwipeScanInfo.IDChecker.MilitaryCardItems

            strIssueDate = objProcessedData.CardInfo.IssueDate.Text

            If Len(strIssueDate) = 8 Then
                strIssueDate = strIssueDate.Substring(0, 2) & "/" & strIssueDate.Substring(2, 2) & "/" & strIssueDate.Substring(4, 4)
            End If

            strExpirationDate = objProcessedData.CardInfo.ExpirationDate.Text

            If Len(strExpirationDate) = 8 Then
                strExpirationDate = strExpirationDate.Substring(0, 2) & "/" & strExpirationDate.Substring(2, 2) & "/" & strExpirationDate.Substring(4, 4)
            End If

            If strExpirationDate = "unknown" Then
                strExpirationDate = Nothing
            End If

            strBirthDate = objProcessedData.PersonalInfo.DOB.Text

            If Len(strBirthDate) = 8 Then
                strBirthDate = strBirthDate.Substring(0, 2) & "/" & strBirthDate.Substring(2, 2) & "/" & strBirthDate.Substring(4, 4)

                intAge = Math.Floor(DateDiff(DateInterval.Day, CDate(strBirthDate), Today) / 365.25)

            End If

            If SwipeScanInfo.DisableDBSave = False Then

                cnn = OpenConnection()

                strSQLCommand = "qryNewDataSwipeScan_MID"

                cmd = New SqlCommand(strSQLCommand, cnn)
                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanID)
                cmd.Parameters.AddWithValue("IDNumber", objProcessedData.CardInfo.FormNumber.Text)
                cmd.Parameters.AddWithValue("NameFirst", objProcessedData.PersonalInfo.Name.First)
                cmd.Parameters.AddWithValue("NameLast", objProcessedData.PersonalInfo.Name.Last)
                cmd.Parameters.AddWithValue("NameMiddle", objProcessedData.PersonalInfo.Name.Middle)
                cmd.Parameters.AddWithValue("DateOfBirth", strBirthDate)
                cmd.Parameters.AddWithValue("Age", intAge)
                cmd.Parameters.AddWithValue("Height", objProcessedData.PersonalInfo.Height.FeetInches)
                cmd.Parameters.AddWithValue("Weight", objProcessedData.PersonalInfo.Weight.Pounds)
                cmd.Parameters.AddWithValue("Eyes", objProcessedData.PersonalInfo.Eye.Color)
                cmd.Parameters.AddWithValue("Hair", objProcessedData.PersonalInfo.Hair.Color)
                cmd.Parameters.AddWithValue("DateOfIssue", strIssueDate)
                cmd.Parameters.AddWithValue("DateOfExpiration", strExpirationDate)
                cmd.Parameters.AddWithValue("SwipeRawData", SwipeScanRawData)

                cnn.Open()

                cmd.ExecuteNonQuery()

                cnn.Close()

            End If

            objSwipeScanDetail.SwipeScanID = SwipeScanID
            objSwipeScanDetail.IDAccountNumber = objProcessedData.CardInfo.FormNumber.Text
            objSwipeScanDetail.NameFirst = objProcessedData.PersonalInfo.Name.First
            objSwipeScanDetail.NameLast = objProcessedData.PersonalInfo.Name.Last
            objSwipeScanDetail.NameMiddle = objProcessedData.PersonalInfo.Name.Middle
            objSwipeScanDetail.DateOfBirth = strBirthDate
            objSwipeScanDetail.Age = intAge
            objSwipeScanDetail.Height = objProcessedData.PersonalInfo.Height.FeetInches
            objSwipeScanDetail.Weight = objProcessedData.PersonalInfo.Weight.Pounds
            objSwipeScanDetail.Eyes = objProcessedData.PersonalInfo.Eye.Color
            objSwipeScanDetail.Hair = objProcessedData.PersonalInfo.Hair.Color
            objSwipeScanDetail.DateOfIssue = strIssueDate
            objSwipeScanDetail.DateOfExpiration = strExpirationDate
            objSwipeScanDetail.SwipeRawData = SwipeScanRawData
            objSwipeScanDetail.CardType = "Military ID Card"
            objSwipeScanDetail.DataSource = DataSource
            objSwipeScanDetail.UpdateTS = Now

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return objSwipeScanDetail

    End Function

    Private Shared Function NewDataSwipeScan_INS(ByVal SwipeScanID As Integer, ByVal SwipeScanRawData As String, ByVal DataSource As String, ByVal SwipeScanInfo As SwipeScanInfo) As SwipeScanDetail

        Dim objSwipeScanDetail As New SwipeScanDetail
        Dim objProcessedData As New IDecode.Net.PC.stINSEmpAuthItems
        Dim strIssueDate As String
        Dim strExpirationDate As String
        Dim strBirthDate As String
        Dim intAge As Integer = 0
        Dim cnn As New SqlConnection
        Dim sb As New StringBuilder
        Dim strSQLCommand As String
        Dim cmd As SqlCommand

        Try
            SwipeScanInfo = SwipeScanInfo

            objProcessedData = SwipeScanInfo.IDChecker.INSEmployeeAuthItems

            strIssueDate = objProcessedData.StartDate.Text

            If Len(strIssueDate) = 8 Then
                strIssueDate = strIssueDate.Substring(0, 2) & "/" & strIssueDate.Substring(2, 2) & "/" & strIssueDate.Substring(4, 4)
            End If

            strExpirationDate = objProcessedData.ExpiryDate.Text

            If Len(strExpirationDate) = 8 Then
                strExpirationDate = strExpirationDate.Substring(0, 2) & "/" & strExpirationDate.Substring(2, 2) & "/" & strExpirationDate.Substring(4, 4)
            End If

            If strExpirationDate = "unknown" Then
                strExpirationDate = Nothing
            End If

            strBirthDate = objProcessedData.BirthDate.Text

            If Len(strBirthDate) = 8 Then
                strBirthDate = strBirthDate.Substring(0, 2) & "/" & strBirthDate.Substring(2, 2) & "/" & strBirthDate.Substring(4, 4)
                intAge = Math.Floor(DateDiff(DateInterval.Day, CDate(strBirthDate), Today) / 365.25)
            End If

            If SwipeScanInfo.DisableDBSave = False Then

                cnn = OpenConnection()

                strSQLCommand = "qryNewDataSwipeScan_INS"

                cmd = New SqlCommand(strSQLCommand, cnn)
                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanID)
                cmd.Parameters.AddWithValue("IDNumber", objProcessedData.CardNumber.Text)
                cmd.Parameters.AddWithValue("NameFirst", objProcessedData.Name.First)
                cmd.Parameters.AddWithValue("NameLast", objProcessedData.Name.Last)
                cmd.Parameters.AddWithValue("NameMiddle", objProcessedData.Name.Middle)
                cmd.Parameters.AddWithValue("DateOfBirth", strBirthDate)
                cmd.Parameters.AddWithValue("Age", intAge)
                cmd.Parameters.AddWithValue("Sex", objProcessedData.Sex.Text)
                cmd.Parameters.AddWithValue("DateOfIssue", strIssueDate)
                cmd.Parameters.AddWithValue("DateOfExpiration", strExpirationDate)
                cmd.Parameters.AddWithValue("SwipeRawData", SwipeScanRawData)

                cnn.Open()

                cmd.ExecuteNonQuery()

                cnn.Close()

            End If

            objSwipeScanDetail.SwipeScanID = SwipeScanID
            objSwipeScanDetail.IDAccountNumber = objProcessedData.CardNumber.Text
            objSwipeScanDetail.NameFirst = objProcessedData.Name.First
            objSwipeScanDetail.NameLast = objProcessedData.Name.Last
            objSwipeScanDetail.NameMiddle = objProcessedData.Name.Middle
            objSwipeScanDetail.DateOfBirth = strBirthDate
            objSwipeScanDetail.Age = intAge
            objSwipeScanDetail.Sex = objProcessedData.Sex.Text
            objSwipeScanDetail.DateOfIssue = strIssueDate
            objSwipeScanDetail.DateOfExpiration = strExpirationDate
            objSwipeScanDetail.SwipeRawData = SwipeScanRawData
            objSwipeScanDetail.CardType = "INS Employee Authorization Card"
            objSwipeScanDetail.DataSource = DataSource
            objSwipeScanDetail.UpdateTS = Now

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return objSwipeScanDetail

    End Function

    Private Shared Function NewDataSwipeScan_CK(ByVal SwipeScanID As Integer, ByVal MicrNumber As String, ByVal DataSource As String, ByVal SwipeScanInfo As SwipeScanInfo) As SwipeScanDetail

        Dim objSwipeScanDetail As New SwipeScanDetail
        Dim strMICRNumber As String
        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim strRoutingNumber As String
        Dim strAccountNumber As String
        Dim strCheckNumber As String
        Dim strCheckAmount As String

        Try
            'If MICR contains !=ERROR Unreadable char?
            strMICRNumber = GetMICRNumber(MicrNumber)
            strRoutingNumber = GetRoutingNumber(strMICRNumber)
            strAccountNumber = GetAccountNumber(strMICRNumber)
            strCheckNumber = GetCheckNumber(strMICRNumber)
            strCheckAmount = GetCheckAmount(MicrNumber)

            If SwipeScanInfo.DisableDBSave = False Then

                cnn = OpenConnection()

                strSQLCommand = "qryNewDataSwipeScan_CK"

                cmd = New SqlCommand(strSQLCommand, cnn)
                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanID)
                cmd.Parameters.AddWithValue("RoutingNumber", strRoutingNumber)
                cmd.Parameters.AddWithValue("AccountNumber", strAccountNumber)
                cmd.Parameters.AddWithValue("CheckNumber", strCheckNumber)
                cmd.Parameters.AddWithValue("CheckAmount", strCheckAmount)
                cmd.Parameters.AddWithValue("BatchID", SwipeScanInfo.BatchID)
                cmd.Parameters.AddWithValue("SwipeRawData", MicrNumber)

                cnn.Open()

                cmd.ExecuteNonQuery()

                cnn.Close()

            End If

            objSwipeScanDetail.SwipeScanID = SwipeScanID
            objSwipeScanDetail.IDAccountNumber = strRoutingNumber & ":" & strAccountNumber
            objSwipeScanDetail.SwipeRawData = MicrNumber
            objSwipeScanDetail.CheckNumber = strCheckNumber
            objSwipeScanDetail.CardType = "Check"
            objSwipeScanDetail.DataSource = DataSource
            objSwipeScanDetail.UpdateTS = Now

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return objSwipeScanDetail

    End Function

    Public Shared Function NewDataSwipeScanManual(ByVal SwipeScanDetail As SwipeScanDetail) As Integer

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim intSwipeScanID As Integer

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryNewDataSwipeScan"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("SwipeScanType", SwipeScanDetail.CardType)
            cmd.Parameters.AddWithValue("ClientID", SwipeScanDetail.ClientID)
            cmd.Parameters.AddWithValue("UserID", SwipeScanDetail.UserID)
            cmd.Parameters.AddWithValue("DataSource", "Manual")

            cnn.Open()

            cmd.ExecuteNonQuery()

            cmd.CommandText = "SELECT @@IDENTITY"
            cmd.CommandType = CommandType.Text
            intSwipeScanID = cmd.ExecuteScalar

            cnn.Close()

            MyAppLog.WriteToLog("clsData.NewDataSwipeScanManual()intSwipeScanID:" & intSwipeScanID)

            SwipeScanDetail.SwipeScanID = intSwipeScanID

            Select Case SwipeScanDetail.CardType

                Case "Drivers License Or State ID"
                    NewDataSwipeScanManual_DLID(SwipeScanDetail)
                Case "Credit Card"
                    NewDataSwipeScanManual_CC(SwipeScanDetail)
                Case "Military ID Card"
                    NewDataSwipeScanManual_MID(SwipeScanDetail)
                Case "INS Employee Authorization Card"
                    NewDataSwipeScanManual_INS(SwipeScanDetail)
                Case "Check"
                    NewDataSwipeScanManual_CK(SwipeScanDetail)

            End Select

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return intSwipeScanID

    End Function

    Private Shared Sub NewDataSwipeScanManual_CC(ByVal SwipeScanDetail As SwipeScanDetail)

        Dim strExpirationDate As String
        Dim strCCNumber As String
        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand

        Try
            strExpirationDate = SwipeScanDetail.DateOfExpiration

            If Len(strExpirationDate) = 8 Then
                strExpirationDate = strExpirationDate.Substring(0, 2) & "/" & strExpirationDate.Substring(2, 2) & "/" & strExpirationDate.Substring(4, 4)
            End If

            If strExpirationDate = "unknown" Then
                strExpirationDate = Nothing
            End If

            strCCNumber = Trim(SwipeScanDetail.IDAccountNumber)

            If SwipeScanDetail.CCDigits > 0 Then

                Dim sb As New StringBuilder

                For i = 1 To strCCNumber.Length - SwipeScanDetail.CCDigits

                    sb.Append("X")

                Next

                sb.Append(strCCNumber.Substring(strCCNumber.Length - SwipeScanDetail.CCDigits, SwipeScanDetail.CCDigits))

                strCCNumber = sb.ToString

            End If


            If SwipeScanDetail.DisableCCSave = True Then

                SwipeScanDetail.SwipeRawData = Nothing
            End If

            cnn = OpenConnection()

            strSQLCommand = "qryNewDataSwipeScan_CC"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanDetail.SwipeScanID)
            cmd.Parameters.AddWithValue("CCNumber", strCCNumber)
            cmd.Parameters.AddWithValue("CCIssuer", SwipeScanDetail.CCIssuer)
            cmd.Parameters.AddWithValue("NameFirst", SwipeScanDetail.NameFirst)
            cmd.Parameters.AddWithValue("NameLast", SwipeScanDetail.NameLast)
            cmd.Parameters.AddWithValue("NameMiddle", SwipeScanDetail.NameMiddle)
            cmd.Parameters.AddWithValue("DateOfExpiration", strExpirationDate)
            cmd.Parameters.AddWithValue("SwipeRawData", "Manual Entry")

            cnn.Open()

            cmd.ExecuteNonQuery()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

    End Sub

    Private Shared Sub NewDataSwipeScanManual_DLID(ByVal SwipeScanDetail As SwipeScanDetail)

        Dim strIssueDate As String
        Dim strExpirationDate As String
        Dim strBirthDate As String
        Dim cnn As New SqlConnection
        Dim sb As New StringBuilder
        Dim strSQLCommand As String
        Dim cmd As SqlCommand

        Try
            strIssueDate = SwipeScanDetail.DateOfIssue

            If Len(strIssueDate) = 8 Then
                strIssueDate = strIssueDate.Substring(0, 2) & "/" & strIssueDate.Substring(2, 2) & "/" & strIssueDate.Substring(4, 4)
            End If

            strExpirationDate = SwipeScanDetail.DateOfExpiration

            If Len(strExpirationDate) = 8 Then
                strExpirationDate = strExpirationDate.Substring(0, 2) & "/" & strExpirationDate.Substring(2, 2) & "/" & strExpirationDate.Substring(4, 4)
            End If

            strBirthDate = SwipeScanDetail.DateOfBirth

            If Len(strBirthDate) = 8 Then
                strBirthDate = strBirthDate.Substring(0, 2) & "/" & strBirthDate.Substring(2, 2) & "/" & strBirthDate.Substring(4, 4)
            End If

            cnn = OpenConnection()

            strSQLCommand = "qryNewDataSwipeScan_DLID"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanDetail.SwipeScanID)
            cmd.Parameters.AddWithValue("IDNumber", SwipeScanDetail.IDAccountNumber)
            cmd.Parameters.AddWithValue("NameFirst", SwipeScanDetail.NameFirst)
            cmd.Parameters.AddWithValue("NameLast", SwipeScanDetail.NameLast)
            cmd.Parameters.AddWithValue("NameMiddle", SwipeScanDetail.NameMiddle)
            cmd.Parameters.AddWithValue("DateOfBirth", strBirthDate)
            cmd.Parameters.AddWithValue("Age", SwipeScanDetail.Age)
            cmd.Parameters.AddWithValue("Sex", SwipeScanDetail.Sex)
            cmd.Parameters.AddWithValue("Height", SwipeScanDetail.Height)
            cmd.Parameters.AddWithValue("Weight", SwipeScanDetail.Weight)
            cmd.Parameters.AddWithValue("Eyes", SwipeScanDetail.Eyes)
            cmd.Parameters.AddWithValue("Hair", SwipeScanDetail.Hair)
            cmd.Parameters.AddWithValue("DateOfIssue", strIssueDate)
            cmd.Parameters.AddWithValue("DateOfExpiration", strExpirationDate)
            cmd.Parameters.AddWithValue("AddressStreet", SwipeScanDetail.AddressStreet)
            cmd.Parameters.AddWithValue("AddressCity", SwipeScanDetail.AddressCity)
            cmd.Parameters.AddWithValue("AddressState", SwipeScanDetail.AddressState)
            cmd.Parameters.AddWithValue("AddressZip", SwipeScanDetail.AddressZip)
            cmd.Parameters.AddWithValue("SwipeRawData", "Manual Entry")

            cnn.Open()

            cmd.ExecuteNonQuery()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

    End Sub

    Private Shared Sub NewDataSwipeScanManual_MID(ByVal SwipeScanDetail As SwipeScanDetail)

        Dim strIssueDate As String
        Dim strExpirationDate As String
        Dim strBirthDate As String
        Dim intAge As Integer = 0
        Dim cnn As New SqlConnection
        Dim sb As New StringBuilder
        Dim strSQLCommand As String
        Dim cmd As SqlCommand

        Try
            strIssueDate = SwipeScanDetail.DateOfIssue

            If Len(strIssueDate) = 8 Then
                strIssueDate = strIssueDate.Substring(0, 2) & "/" & strIssueDate.Substring(2, 2) & "/" & strIssueDate.Substring(4, 4)
            End If

            strExpirationDate = SwipeScanDetail.DateOfExpiration

            If Len(strExpirationDate) = 8 Then
                strExpirationDate = strExpirationDate.Substring(0, 2) & "/" & strExpirationDate.Substring(2, 2) & "/" & strExpirationDate.Substring(4, 4)
            End If

            If strExpirationDate = "unknown" Then
                strExpirationDate = Nothing
            End If

            strBirthDate = SwipeScanDetail.DateOfBirth

            If Len(strBirthDate) = 8 Then
                strBirthDate = strBirthDate.Substring(0, 2) & "/" & strBirthDate.Substring(2, 2) & "/" & strBirthDate.Substring(4, 4)

                intAge = DateDiff(DateInterval.Year, CDate(strBirthDate), Today)

            End If

            cnn = OpenConnection()

            strSQLCommand = "qryNewDataSwipeScan_MID"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanDetail.SwipeScanID)
            cmd.Parameters.AddWithValue("IDNumber", SwipeScanDetail.IDAccountNumber)
            cmd.Parameters.AddWithValue("NameFirst", SwipeScanDetail.NameFirst)
            cmd.Parameters.AddWithValue("NameLast", SwipeScanDetail.NameLast)
            cmd.Parameters.AddWithValue("NameMiddle", SwipeScanDetail.NameMiddle)
            cmd.Parameters.AddWithValue("DateOfBirth", strBirthDate)
            cmd.Parameters.AddWithValue("Age", intAge)
            cmd.Parameters.AddWithValue("Height", SwipeScanDetail.Height)
            cmd.Parameters.AddWithValue("Weight", SwipeScanDetail.Weight)
            cmd.Parameters.AddWithValue("Eyes", SwipeScanDetail.Eyes)
            cmd.Parameters.AddWithValue("Hair", SwipeScanDetail.Hair)
            cmd.Parameters.AddWithValue("DateOfIssue", strIssueDate)
            cmd.Parameters.AddWithValue("DateOfExpiration", strExpirationDate)
            cmd.Parameters.AddWithValue("SwipeRawData", "Manual Entry")

            cnn.Open()

            cmd.ExecuteNonQuery()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

    End Sub

    Private Shared Sub NewDataSwipeScanManual_INS(ByVal SwipeScanDetail As SwipeScanDetail)

        Dim strIssueDate As String
        Dim strExpirationDate As String
        Dim strBirthDate As String
        Dim intAge As Integer = 0
        Dim cnn As New SqlConnection
        Dim sb As New StringBuilder
        Dim strSQLCommand As String
        Dim cmd As SqlCommand

        Try
            strIssueDate = SwipeScanDetail.DateOfIssue

            If Len(strIssueDate) = 8 Then
                strIssueDate = strIssueDate.Substring(0, 2) & "/" & strIssueDate.Substring(2, 2) & "/" & strIssueDate.Substring(4, 4)
            End If

            strExpirationDate = SwipeScanDetail.DateOfExpiration

            If Len(strExpirationDate) = 8 Then
                strExpirationDate = strExpirationDate.Substring(0, 2) & "/" & strExpirationDate.Substring(2, 2) & "/" & strExpirationDate.Substring(4, 4)
            End If

            If strExpirationDate = "unknown" Then
                strExpirationDate = Nothing
            End If

            strBirthDate = SwipeScanDetail.DateOfBirth

            If Len(strBirthDate) = 8 Then
                strBirthDate = strBirthDate.Substring(0, 2) & "/" & strBirthDate.Substring(2, 2) & "/" & strBirthDate.Substring(4, 4)
                intAge = DateDiff(DateInterval.Year, CDate(strBirthDate), Today)
            End If

            cnn = OpenConnection()

            strSQLCommand = "qryNewDataSwipeScan_INS"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanDetail.SwipeScanID)
            cmd.Parameters.AddWithValue("IDNumber", SwipeScanDetail.IDAccountNumber)
            cmd.Parameters.AddWithValue("NameFirst", SwipeScanDetail.NameFirst)
            cmd.Parameters.AddWithValue("NameLast", SwipeScanDetail.NameLast)
            cmd.Parameters.AddWithValue("NameMiddle", SwipeScanDetail.NameMiddle)
            cmd.Parameters.AddWithValue("DateOfBirth", strBirthDate)
            cmd.Parameters.AddWithValue("Age", intAge)
            cmd.Parameters.AddWithValue("Sex", SwipeScanDetail.Sex)
            cmd.Parameters.AddWithValue("DateOfIssue", strIssueDate)
            cmd.Parameters.AddWithValue("DateOfExpiration", strExpirationDate)
            cmd.Parameters.AddWithValue("SwipeRawData", "Manual Entry")

            cnn.Open()

            cmd.ExecuteNonQuery()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

    End Sub

    Private Shared Sub NewDataSwipeScanManual_CK(ByVal SwipeScanDetail As SwipeScanDetail)

        Dim strArray As String()
        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand

        Try
            strArray = Split(SwipeScanDetail.IDAccountNumber, ":")

            cnn = OpenConnection()

            strSQLCommand = "qryNewDataSwipeScan_CK"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanDetail.SwipeScanID)
            cmd.Parameters.AddWithValue("RoutingNumber", strArray(0).ToString)
            cmd.Parameters.AddWithValue("AccountNumber", strArray(1).ToString)
            cmd.Parameters.AddWithValue("CheckNumber", SwipeScanDetail.CheckNumber)
            cmd.Parameters.AddWithValue("CheckAmount", 0)
            cmd.Parameters.AddWithValue("BatchID", Nothing)
            cmd.Parameters.AddWithValue("SwipeRawData", "Manual Entry")

            cnn.Open()

            cmd.ExecuteNonQuery()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

    End Sub

    Private Shared Function GetMICRNumber(ByVal MICRNumber As String) As String

        Dim strMICRNumber As String
        Dim intAmountPositionStart As Integer
        Dim intAmountPositionEnd As Integer
        Dim intAmountLength As Integer

        Try

            intAmountPositionStart = InStr(MICRNumber, ";")
            intAmountPositionEnd = InStr(intAmountPositionStart + 1, MICRNumber, ";")

            If intAmountPositionStart > 0 And intAmountPositionEnd > 0 Then

                intAmountLength = intAmountPositionEnd - intAmountPositionStart + 1
                strMICRNumber = Trim(MICRNumber.Remove(intAmountPositionStart - 1, intAmountLength))

            Else
                strMICRNumber = MICRNumber
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        Return strMICRNumber

    End Function

    Private Shared Function GetRoutingNumber(ByVal MICRNumber As String) As String

        Dim strRoutingNumber As String
        Dim intPositionStart As Integer
        Dim intPositionEnd As Integer
        Dim intRoutingLength As Integer

        Try
            If MICRNumber.Contains("!") Then
                strRoutingNumber = "Error"
            Else
                intPositionStart = InStr(MICRNumber, ":")

                intPositionEnd = InStr(intPositionStart + 1, MICRNumber, ":")

                intRoutingLength = intPositionEnd - intPositionStart - 1

                strRoutingNumber = Trim(MICRNumber.Substring(intPositionStart, intRoutingLength))

            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        Return strRoutingNumber

    End Function

    Private Shared Function GetAccountNumber(ByVal MICRNumber As String) As String

        Dim strAccountNumber As String
        Dim intRoutingPositionStart As Integer
        Dim intRoutingPositionEnd As Integer
        Dim intAccountPositionStart As Integer
        Dim intAccountPositionEnd As Integer
        Dim intAccountLength As Integer

        Try
            If MICRNumber.Contains("!") Then

                strAccountNumber = "Error"

            Else

                intRoutingPositionStart = InStr(MICRNumber, ":")

                intRoutingPositionEnd = InStr(intRoutingPositionStart + 1, MICRNumber, ":")

                intAccountPositionEnd = InStr(intRoutingPositionEnd, MICRNumber, "<")

                intAccountLength = intAccountPositionEnd - intRoutingPositionEnd - 1

                strAccountNumber = Trim(MICRNumber.Substring(intRoutingPositionEnd, intAccountLength))

                If (strAccountNumber.Contains(" ") And Len(strAccountNumber) > 13) Or intAccountPositionEnd = 0 Then
                    'Money Order:             :091900533:2037 38315946< 90
                    Dim strAccountSplit As String()
                    Dim strAccountPartA As String
                    Dim strAccountPartB As String

                    strAccountSplit = strAccountNumber.Split(" ")
                    strAccountPartA = strAccountSplit(0).ToString
                    strAccountPartB = strAccountSplit(1).ToString

                    If strAccountPartB < strAccountPartA Then

                        strAccountNumber = strAccountPartA

                    Else

                        strAccountNumber = strAccountPartB

                    End If

                End If

                If intAccountLength = 1 Then

                    ':091300036: <6310890744< 1031
                    intAccountPositionStart = InStr(MICRNumber, "<")
                    intAccountPositionEnd = InStr(intAccountPositionStart + 1, MICRNumber, "<")
                    intAccountLength = intAccountPositionEnd - intAccountPositionStart - 1
                    strAccountNumber = Trim(MICRNumber.Substring(intAccountPositionStart, intAccountLength))
                End If

                strAccountNumber = strAccountNumber.Replace("=", "")
                strAccountNumber = strAccountNumber.Replace(" ", "")
                strAccountNumber = strAccountNumber.TrimStart("0")

            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        Return strAccountNumber

    End Function

    Private Shared Function GetCheckNumber(ByVal MICRNumber As String) As String

        Dim strCheckNumber As String
        Dim intCheckPositionStart As Integer
        Dim intCheckPositionEnd As Integer
        Dim intCheckNumberLength As Integer

        Try
            If MICRNumber.Contains("!") Then

                strCheckNumber = 0

            Else

                intCheckPositionStart = InStr(MICRNumber, "<")

                intCheckPositionEnd = InStr(intCheckPositionStart + 1, MICRNumber, "<")

                If intCheckPositionEnd = 0 Then
                    intCheckNumberLength = Len(MICRNumber) - intCheckPositionStart
                Else
                    intCheckNumberLength = intCheckPositionEnd - intCheckPositionStart - 1
                End If

                strCheckNumber = Trim(MICRNumber.Substring(intCheckPositionStart, intCheckNumberLength))

                If intCheckNumberLength > 6 Then
                    ':091300036: <6310890744< 1031
                    intCheckPositionStart = InStr(intCheckPositionStart + 1, MICRNumber, "<")

                    intCheckNumberLength = Len(MICRNumber) - intCheckPositionStart

                    strCheckNumber = Trim(MICRNumber.Substring(intCheckPositionStart, intCheckNumberLength))

                ElseIf intCheckNumberLength = 0 Then

                    Dim strMICRNumber As String = MICRNumber
                    Dim strAccountNumber As String = GetAccountNumber(MICRNumber)
                    Dim strRoutingNumber As String = GetRoutingNumber(MICRNumber)
                    strCheckNumber = strMICRNumber

                    strCheckNumber = strCheckNumber.Replace(strRoutingNumber, "")
                    strCheckNumber = strCheckNumber.Replace(strAccountNumber, "")
                    strCheckNumber = strCheckNumber.Replace(":", "")
                    strCheckNumber = strCheckNumber.Replace("<", "")

                End If

                strCheckNumber = strCheckNumber.TrimStart("0")

            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        Return strCheckNumber

    End Function

    Private Shared Function GetCheckAmount(ByVal MICRNumber As String) As String
        'TODO: check if isnumeric
        Dim strCheckAmount As String
        Dim intPositionStart As Integer
        Dim intPositionEnd As Integer
        Dim intCheckAmountLength As Integer

        ':075906236: 0408778< 1756;0000004542;
        ':124084672:2018 4902263922<
        Try
            If MICRNumber.Contains("!") = True Or MICRNumber.Contains(";") = False Then

                strCheckAmount = 0

            Else
                intPositionStart = InStr(MICRNumber, ";")

                intPositionEnd = InStr(intPositionStart + 1, MICRNumber, ";")

                intCheckAmountLength = intPositionEnd - intPositionStart - 1

                strCheckAmount = Trim(MICRNumber.Substring(intPositionStart, intCheckAmountLength))

                strCheckAmount = strCheckAmount.Substring(0, intCheckAmountLength - 2) & "." & strCheckAmount.Substring(intCheckAmountLength - 2, 2)
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        Return strCheckAmount

    End Function

    Public Shared Function GetSwipeScanDetail(ByVal SwipeScanID As Integer, ByVal SwipeScanType As String) As SwipeScanDetail

        Dim objSwipeScanDetail As New SwipeScanDetail
        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader

        Try
            cnn = OpenConnection()

            Select Case SwipeScanType

                Case "Credit Card"

                    strSQLCommand = "qryGetSwipeScanDetail_CC"

                    cmd = New SqlCommand(strSQLCommand, cnn)
                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanID)

                    cnn.Open()

                    dr = cmd.ExecuteReader

                    While dr.Read

                        objSwipeScanDetail.SwipeScanID = SwipeScanID
                        objSwipeScanDetail.IDAccountNumber = dr(0).ToString
                        objSwipeScanDetail.CCIssuer = dr(1).ToString
                        objSwipeScanDetail.NameFirst = dr(2).ToString
                        objSwipeScanDetail.NameLast = dr(3).ToString
                        objSwipeScanDetail.NameMiddle = dr(4).ToString
                        objSwipeScanDetail.DateOfExpiration = dr(5).ToString
                        objSwipeScanDetail.UserName = dr(6).ToString
                        objSwipeScanDetail.Location = dr(7).ToString
                        objSwipeScanDetail.SwipeRawData = dr(8).ToString
                        objSwipeScanDetail.DataSource = dr(9).ToString
                        objSwipeScanDetail.CaseID = dr(10).ToString

                    End While

                Case "Drivers License Or State ID"

                    strSQLCommand = "qryGetSwipeScanDetail_DLID"

                    cmd = New SqlCommand(strSQLCommand, cnn)
                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanID)

                    cnn.Open()

                    dr = cmd.ExecuteReader

                    While dr.Read

                        objSwipeScanDetail.SwipeScanID = SwipeScanID
                        objSwipeScanDetail.IDAccountNumber = dr(0).ToString
                        objSwipeScanDetail.NameFirst = dr(1).ToString
                        objSwipeScanDetail.NameLast = dr(2).ToString
                        objSwipeScanDetail.NameMiddle = dr(3).ToString
                        objSwipeScanDetail.DateOfBirth = dr(4).ToString
                        objSwipeScanDetail.Age = dr(5).ToString
                        objSwipeScanDetail.Sex = dr(6).ToString
                        objSwipeScanDetail.Height = dr(7).ToString
                        objSwipeScanDetail.Weight = dr(8).ToString
                        objSwipeScanDetail.Eyes = dr(9).ToString
                        objSwipeScanDetail.Hair = dr(10).ToString
                        objSwipeScanDetail.DateOfIssue = dr(11).ToString
                        objSwipeScanDetail.DateOfExpiration = dr(12).ToString
                        objSwipeScanDetail.AddressStreet = dr(13).ToString
                        objSwipeScanDetail.AddressCity = dr(14).ToString
                        objSwipeScanDetail.AddressState = dr(15).ToString
                        objSwipeScanDetail.AddressZip = dr(16).ToString
                        objSwipeScanDetail.UserName = dr(17).ToString
                        objSwipeScanDetail.Location = dr(18).ToString
                        objSwipeScanDetail.SwipeRawData = dr(19).ToString
                        objSwipeScanDetail.DataSource = dr(20).ToString
                        objSwipeScanDetail.CaseID = dr(21).ToString

                    End While

                Case "Check"

                    strSQLCommand = "qryGetSwipeScanDetail_CK"

                    cmd = New SqlCommand(strSQLCommand, cnn)
                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanID)

                    cnn.Open()

                    dr = cmd.ExecuteReader

                    While dr.Read

                        objSwipeScanDetail.SwipeScanID = SwipeScanID
                        objSwipeScanDetail.IDAccountNumber = dr(0).ToString
                        objSwipeScanDetail.CheckNumber = dr(1).ToString
                        objSwipeScanDetail.UserName = dr(2).ToString
                        objSwipeScanDetail.Location = dr(3).ToString
                        objSwipeScanDetail.SwipeRawData = dr(4).ToString
                        objSwipeScanDetail.DataSource = dr(5).ToString
                        objSwipeScanDetail.CaseID = dr(6).ToString

                    End While

                Case "Military ID Card"

                    strSQLCommand = "qryGetSwipeScanDetail_MID"

                    cmd = New SqlCommand(strSQLCommand, cnn)
                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanID)

                    cnn.Open()

                    dr = cmd.ExecuteReader

                    While dr.Read

                        objSwipeScanDetail.SwipeScanID = SwipeScanID
                        objSwipeScanDetail.IDAccountNumber = dr(0).ToString
                        objSwipeScanDetail.NameFirst = dr(1).ToString
                        objSwipeScanDetail.NameLast = dr(2).ToString
                        objSwipeScanDetail.NameMiddle = dr(3).ToString
                        objSwipeScanDetail.DateOfBirth = dr(4).ToString
                        objSwipeScanDetail.Age = dr(5).ToString
                        objSwipeScanDetail.Height = dr(6).ToString
                        objSwipeScanDetail.Weight = dr(7).ToString
                        objSwipeScanDetail.Eyes = dr(8).ToString
                        objSwipeScanDetail.Hair = dr(9).ToString
                        objSwipeScanDetail.DateOfIssue = dr(10).ToString
                        objSwipeScanDetail.DateOfExpiration = dr(11).ToString
                        objSwipeScanDetail.UserName = dr(12).ToString
                        objSwipeScanDetail.Location = dr(13).ToString
                        objSwipeScanDetail.SwipeRawData = dr(14).ToString
                        objSwipeScanDetail.DataSource = dr(15).ToString
                        objSwipeScanDetail.CaseID = dr(16).ToString

                    End While

                Case "INS Employee Authorization Card"

                    strSQLCommand = "qryGetSwipeScanDetail_INS"

                    cmd = New SqlCommand(strSQLCommand, cnn)
                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanID)

                    cnn.Open()

                    dr = cmd.ExecuteReader

                    While dr.Read

                        objSwipeScanDetail.SwipeScanID = SwipeScanID
                        objSwipeScanDetail.IDAccountNumber = dr(0).ToString
                        objSwipeScanDetail.NameFirst = dr(1).ToString
                        objSwipeScanDetail.NameLast = dr(2).ToString
                        objSwipeScanDetail.NameMiddle = dr(3).ToString
                        objSwipeScanDetail.DateOfBirth = dr(4).ToString
                        objSwipeScanDetail.Age = dr(5).ToString
                        objSwipeScanDetail.Sex = dr(6).ToString
                        objSwipeScanDetail.DateOfIssue = dr(7).ToString
                        objSwipeScanDetail.DateOfExpiration = dr(8).ToString
                        objSwipeScanDetail.UserName = dr(9).ToString
                        objSwipeScanDetail.Location = dr(10).ToString
                        objSwipeScanDetail.SwipeRawData = dr(11).ToString
                        objSwipeScanDetail.DataSource = dr(12).ToString
                        objSwipeScanDetail.CaseID = dr(13).ToString

                    End While

            End Select

            If dr IsNot Nothing Then
                dr.Close()
            End If

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return objSwipeScanDetail

    End Function

    Public Shared Function GetSwipeScanHistory(ByVal IDAccountNumber As String, ByVal SwipeScanType As String) As List(Of SwipeScanHistory)

        Dim strArray As String()
        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader
        Dim ListOfSwipeScanHistory As New List(Of SwipeScanHistory)
        Dim objSwipeScanHistory As SwipeScanHistory

        Try
            cnn = OpenConnection()

            Select Case SwipeScanType

                Case "Credit Card"

                    strSQLCommand = "qryGetSwipeScanHistory_CC"

                Case "Drivers License Or State ID"

                    strSQLCommand = "qryGetSwipeScanHistory_DLID"

                Case "Check"

                    strSQLCommand = "qryGetSwipeScanHistory_CK"
                    strArray = Split(IDAccountNumber, ":")

                Case "Military ID Card"

                    strSQLCommand = "qryGetSwipeScanHistory_MID"

                Case "INS Employee Authorization Card"

                    strSQLCommand = "qryGetSwipeScanHistory_INS"

            End Select

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            If SwipeScanType = "Check" Then

                cmd.Parameters.AddWithValue("RoutingNumber", strArray(0).ToString)
                cmd.Parameters.AddWithValue("AccountNumber", strArray(1).ToString)

            Else
                cmd.Parameters.AddWithValue("IDAccountNumber", IDAccountNumber)

            End If

            cnn.Open()

            dr = cmd.ExecuteReader

            MyAppLog.WriteToLog("IVS.Data.GetSwipeScanHistory() HasRows:" & dr.HasRows)

            While dr.Read

                objSwipeScanHistory = New SwipeScanHistory
                objSwipeScanHistory.SwipeScanID = dr(0).ToString
                objSwipeScanHistory.Location = dr(1).ToString
                objSwipeScanHistory.Station = dr(2).ToString
                objSwipeScanHistory.Phone = dr(3).ToString
                objSwipeScanHistory.UserName = dr(4).ToString
                objSwipeScanHistory.SwipeScanTS = dr(5).ToString
                ListOfSwipeScanHistory.Add(objSwipeScanHistory)

            End While

            dr.Close()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally
            If dr IsNot Nothing Then
                dr.Dispose()
            End If

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If
        End Try

        Return ListOfSwipeScanHistory

    End Function

    Public Shared Function GetSwipeScanType(ByVal SwipeScanID As Integer) As String

        Dim strSwipeScanType As String
        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetSwipeScanType"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                strSwipeScanType = dr(0).ToString

            End While

            dr.Close()
            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return strSwipeScanType

    End Function

    Public Shared Function SwipeScanNavigateFirst(ByVal UserID As Integer, ByVal ClientID As Integer) As SwipeScanNavigateInfo

        Dim objSwipeScanNavigateInfo As New SwipeScanNavigateInfo
        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader

        Try

            cnn = OpenConnection()

            strSQLCommand = "qrySwipeScansNavigateFirst"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("UserID", UserID)
            cmd.Parameters.AddWithValue("ClientID", ClientID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objSwipeScanNavigateInfo.SwipeScanID = dr(0).ToString
                objSwipeScanNavigateInfo.SwipeScanType = dr(1).ToString
                objSwipeScanNavigateInfo.SwipeScanTS = dr(2).ToString

            End While

            dr.Close()
            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return objSwipeScanNavigateInfo

    End Function

    Public Shared Function SwipeScanNavigatePrevious(ByVal UserID As Integer, ByVal ClientID As Integer, ByVal SwipeScanID As Integer) As SwipeScanNavigateInfo

        Dim objSwipeScanNavigateInfo As New SwipeScanNavigateInfo
        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader

        Try

            cnn = OpenConnection()

            strSQLCommand = "qrySwipeScansNavigatePrevious"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("UserID", UserID)
            cmd.Parameters.AddWithValue("ClientID", ClientID)
            cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objSwipeScanNavigateInfo.SwipeScanID = dr(0).ToString
                objSwipeScanNavigateInfo.SwipeScanType = dr(1).ToString
                objSwipeScanNavigateInfo.SwipeScanTS = dr(2).ToString

            End While

            dr.Close()
            cnn.Close()

            If objSwipeScanNavigateInfo.SwipeScanID = 0 Then

                objSwipeScanNavigateInfo = SwipeScanNavigateLast(UserID, ClientID)

            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return objSwipeScanNavigateInfo

    End Function

    Public Shared Function SwipeScanNavigatePosition(ByVal SwipeScanID As Integer) As SwipeScanNavigateInfo

        Dim objSwipeScanNavigateInfo As New SwipeScanNavigateInfo
        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader

        Try

            cnn = OpenConnection()

            strSQLCommand = "qrySwipeScansNavigatePosition"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objSwipeScanNavigateInfo.SwipeScanID = dr(0).ToString
                objSwipeScanNavigateInfo.SwipeScanType = dr(1).ToString
                objSwipeScanNavigateInfo.SwipeScanTS = dr(2).ToString

            End While

            dr.Close()
            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return objSwipeScanNavigateInfo

    End Function

    Public Shared Function SwipeScanNavigateNext(ByVal UserID As Integer, ByVal ClientID As Integer, ByVal SwipeScanID As Integer) As SwipeScanNavigateInfo

        Dim objSwipeScanNavigateInfo As New SwipeScanNavigateInfo
        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader

        Try

            cnn = OpenConnection()

            strSQLCommand = "qrySwipeScansNavigateNext"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("UserID", UserID)
            cmd.Parameters.AddWithValue("ClientID", ClientID)
            cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objSwipeScanNavigateInfo.SwipeScanID = dr(0).ToString
                objSwipeScanNavigateInfo.SwipeScanType = dr(1).ToString
                objSwipeScanNavigateInfo.SwipeScanTS = dr(2).ToString

            End While

            dr.Close()
            cnn.Close()

            If objSwipeScanNavigateInfo.SwipeScanID = 0 Then

                objSwipeScanNavigateInfo = SwipeScanNavigateFirst(UserID, ClientID)

            End If

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return objSwipeScanNavigateInfo

    End Function

    Public Shared Function SwipeScanNavigateLast(ByVal UserID As Integer, ByVal ClientID As Integer) As SwipeScanNavigateInfo

        Dim objSwipeScanNavigateInfo As New SwipeScanNavigateInfo
        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader

        Try

            cnn = OpenConnection()

            strSQLCommand = "qrySwipeScansNavigateLast"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("UserID", UserID)
            cmd.Parameters.AddWithValue("ClientID", ClientID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objSwipeScanNavigateInfo.SwipeScanID = dr(0).ToString
                objSwipeScanNavigateInfo.SwipeScanType = dr(1).ToString
                objSwipeScanNavigateInfo.SwipeScanTS = dr(2).ToString

            End While

            dr.Close()
            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return objSwipeScanNavigateInfo

    End Function

    Public Shared Sub DeleteSwipeScan(ByVal SwipeScanID As Integer)

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim strSwipeScanType As String

        Try
            strSwipeScanType = GetSwipeScanType(SwipeScanID)

            cnn = OpenConnection()

            strSQLCommand = "qryDeleteSwipeScan"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanID)

            cnn.Open()

            cmd.ExecuteNonQuery()

            Select Case strSwipeScanType

                Case "Credit Card"

                    strSQLCommand = "qryDeleteSwipeScan_CC"

                Case "Drivers License Or State ID"

                    strSQLCommand = "qryDeleteSwipeScan_DLID"

                Case "Check"

                    strSQLCommand = "qryDeleteSwipeScan_CK"

                Case "Military ID Card"

                    strSQLCommand = "qryDeleteSwipeScan_MID"

                Case "INS Employee Authorization Card"

                    strSQLCommand = "qryDeleteSwipeScan_INS"

            End Select

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanID)

            cmd.ExecuteNonQuery()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

    End Sub

    Public Shared Sub UpdateCaseID(ByVal SwipeScanID As Integer, ByVal CaseID As String)

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryUpdateCaseID"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanID)
            cmd.Parameters.AddWithValue("CaseID", CaseID)

            cnn.Open()

            cmd.ExecuteNonQuery()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

    End Sub

    Public Shared Function GetSwipeScanSearch() As List(Of SwipeScanSearch)

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader
        Dim ListOfSwipeScanSearch As New List(Of SwipeScanSearch)
        Dim objSwipeScanSearch As SwipeScanSearch

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetSwipeScanSearch"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cnn.Open()

            dr = cmd.ExecuteReader

            MyAppLog.WriteToLog("IVS.Data.GetSwipeScanSearch() HasRows:" & dr.HasRows)

            While dr.Read

                objSwipeScanSearch = New SwipeScanSearch
                objSwipeScanSearch.SwipeScanID = dr(0).ToString
                objSwipeScanSearch.CaseID = dr(1).ToString
                objSwipeScanSearch.SwipeScanType = dr(2).ToString
                objSwipeScanSearch.Location = dr(3).ToString
                objSwipeScanSearch.Station = dr(4).ToString
                objSwipeScanSearch.UserName = dr(5).ToString
                objSwipeScanSearch.SwipeScanTS = dr(6).ToString
                objSwipeScanSearch.IDAccountNumber = dr(7).ToString
                objSwipeScanSearch.NameFirst = dr(8).ToString
                objSwipeScanSearch.NameLast = dr(9).ToString
                objSwipeScanSearch.DateOfBirth = dr(10).ToString
                objSwipeScanSearch.ImageAvailable = dr(11).ToString
                objSwipeScanSearch.ClientID = dr(12).ToString
                ListOfSwipeScanSearch.Add(objSwipeScanSearch)

            End While

            dr.Close()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally
            If dr IsNot Nothing Then
                dr.Dispose()
            End If

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If
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
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        Return strMyIPAddress

    End Function

    Public Shared Function ReturnHostName(ByVal IPAddress As String) As String
        Dim strHostName As String = IPAddress

        Try
            Dim host As IPHostEntry = Dns.GetHostEntry(IPAddress)
            strHostName = host.HostName.ToString

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        Return strHostName

    End Function

End Class

Public Class Clients

    Private _ClientID As Integer
    Private _Location As String
    Private _Station As String

    Property ClientID() As Integer
        Get
            Return _ClientID
        End Get
        Set(ByVal Value As Integer)
            _ClientID = Value
        End Set
    End Property

    Property Location() As String
        Get
            Return _Location
        End Get
        Set(ByVal Value As String)
            _Location = Value
        End Set
    End Property

    Property Station() As String
        Get
            Return _Station
        End Get
        Set(ByVal Value As String)
            _Station = Value
        End Set
    End Property

End Class

Public Class Locations

    Private _ClientID As Integer
    Private _Location As String

    Property ClientID() As Integer
        Get
            Return _ClientID
        End Get
        Set(ByVal Value As Integer)
            _ClientID = Value
        End Set
    End Property

    Property Location() As String
        Get
            Return _Location
        End Get
        Set(ByVal Value As String)
            _Location = Value
        End Set
    End Property

End Class

Public Class ClientSettings

    Private _ClientID As Integer
    Private _DeviceType As String
    Private _DeviceID As Integer
    Private _ComPort As String
    Private _SleepMilliSeconds As Integer
    Private _IDecodeLicense As String
    Private _IDecodeTrackFormat As String
    Private _IDecodeCardTypes As String
    Private _Location As String
    Private _Station As String
    Private _Phone As String
    Private _Email As String
    Private _SkipLogon As Boolean
    Private _DisplayAdmin As Boolean
    Private _DefaultUser As Integer
    Private _AgeHighlight As Boolean
    Private _AgePopup As Boolean
    Private _Age As Integer
    Private _ImageSave As Boolean
    Private _ImageLocation As String
    Private _ViewingTime As Integer
    Private _CCDigits As Integer
    Private _DisableCCSave As Boolean
    Private _DisableDBSave As Boolean
    Private _LogRetention As Integer

    Property ClientID() As Integer
        Get
            Return _ClientID
        End Get
        Set(ByVal Value As Integer)
            _ClientID = Value
        End Set
    End Property

    Property DeviceType() As String
        Get
            Return _DeviceType
        End Get
        Set(ByVal Value As String)
            _DeviceType = Value
        End Set
    End Property

    Property DeviceID() As Integer
        Get
            Return _DeviceID
        End Get
        Set(ByVal Value As Integer)
            _DeviceID = Value
        End Set
    End Property

    Property ComPort() As String
        Get
            Return _ComPort
        End Get
        Set(ByVal Value As String)
            _ComPort = Value
        End Set
    End Property

    Property SleepMilliSeconds() As Integer
        Get
            Return _SleepMilliSeconds
        End Get
        Set(ByVal Value As Integer)
            _SleepMilliSeconds = Value
        End Set
    End Property

    Property IDecodeLicense() As String
        Get
            Return _IDecodeLicense
        End Get
        Set(ByVal Value As String)
            _IDecodeLicense = Value
        End Set
    End Property

    Property IDecodeTrackFormat() As String
        Get
            Return _IDecodeTrackFormat
        End Get
        Set(ByVal Value As String)
            _IDecodeTrackFormat = Value
        End Set
    End Property

    Property IDecodeCardTypes() As String
        Get
            Return _IDecodeCardTypes
        End Get
        Set(ByVal Value As String)
            _IDecodeCardTypes = Value
        End Set
    End Property

    Property Location() As String
        Get
            Return _Location
        End Get
        Set(ByVal Value As String)
            _Location = Value
        End Set
    End Property

    Property Station() As String
        Get
            Return _Station
        End Get
        Set(ByVal Value As String)
            _Station = Value
        End Set
    End Property

    Property Phone() As String
        Get
            Return _Phone
        End Get
        Set(ByVal Value As String)
            _Phone = Value
        End Set
    End Property

    Property Email() As String
        Get
            Return _Email
        End Get
        Set(ByVal Value As String)
            _Email = Value
        End Set
    End Property

    Property SkipLogon() As Boolean
        Get
            Return _SkipLogon
        End Get
        Set(ByVal Value As Boolean)
            _SkipLogon = Value
        End Set
    End Property

    Property DisplayAdmin() As Boolean
        Get
            Return _DisplayAdmin
        End Get
        Set(ByVal Value As Boolean)
            _DisplayAdmin = Value
        End Set
    End Property

    Property DefaultUser() As Integer
        Get
            Return _DefaultUser
        End Get
        Set(ByVal Value As Integer)
            _DefaultUser = Value
        End Set
    End Property

    Property AgeHighlight() As Boolean
        Get
            Return _AgeHighlight
        End Get
        Set(ByVal Value As Boolean)
            _AgeHighlight = Value
        End Set
    End Property

    Property AgePopup() As Boolean
        Get
            Return _AgePopup
        End Get
        Set(ByVal Value As Boolean)
            _AgePopup = Value
        End Set
    End Property

    Property Age() As Integer
        Get
            Return _Age
        End Get
        Set(ByVal Value As Integer)
            _Age = Value
        End Set
    End Property

    Property ImageSave() As Boolean
        Get
            Return _ImageSave
        End Get
        Set(ByVal Value As Boolean)
            _ImageSave = Value
        End Set
    End Property

    Property ImageLocation() As String
        Get
            Return _ImageLocation
        End Get
        Set(ByVal Value As String)
            _ImageLocation = Value
        End Set
    End Property

    Property ViewingTime() As Integer
        Get
            Return _ViewingTime
        End Get
        Set(ByVal Value As Integer)
            _ViewingTime = Value
        End Set
    End Property

    Property CCDigits() As Integer
        Get
            Return _CCDigits
        End Get
        Set(ByVal Value As Integer)
            _CCDigits = Value
        End Set
    End Property

    Property DisableCCSave() As Boolean
        Get
            Return _DisableCCSave
        End Get
        Set(ByVal Value As Boolean)
            _DisableCCSave = Value
        End Set
    End Property

    Property DisableDBSave() As Boolean
        Get
            Return _DisableDBSave
        End Get
        Set(ByVal Value As Boolean)
            _DisableDBSave = Value
        End Set
    End Property

    Property LogRetention() As Integer
        Get
            Return _LogRetention
        End Get
        Set(ByVal Value As Integer)
            _LogRetention = Value
        End Set
    End Property

End Class

Public Class IDecodeLicenseInfo

    Private _ClientID As Integer
    Private _IDecodeLicense As String
    Private _Version As String

    Property ClientID() As Integer
        Get
            Return _ClientID
        End Get
        Set(ByVal Value As Integer)
            _ClientID = Value
        End Set
    End Property

    Property IDecodeLicense() As String
        Get
            Return _IDecodeLicense
        End Get
        Set(ByVal Value As String)
            _IDecodeLicense = Value
        End Set
    End Property

    Property Version() As String
        Get
            Return _Version
        End Get
        Set(ByVal Value As String)
            _Version = Value
        End Set
    End Property

End Class

Public Class DeviceInfo

    Private _ClientID As Integer
    Private _DeviceID As Integer
    Private _DeviceType As String
    Private _ModelNo As String
    Private _SerialNo As String
    Private _FirmwareRev As String
    Private _FirmwareDate As String
    Private _HardwareRev As String
    Private _ComPort As String
    Private _UpdateTS As Date

    Property ClientID() As Integer
        Get
            Return _ClientID
        End Get
        Set(ByVal Value As Integer)
            _ClientID = Value
        End Set
    End Property

    Property DeviceID() As Integer
        Get
            Return _DeviceID
        End Get
        Set(ByVal Value As Integer)
            _DeviceID = Value
        End Set
    End Property

    Property DeviceType() As String
        Get
            Return _DeviceType
        End Get
        Set(ByVal Value As String)
            _DeviceType = Value
        End Set
    End Property

    Property ModelNo() As String
        Get
            Return _ModelNo
        End Get
        Set(ByVal Value As String)
            _ModelNo = Value
        End Set
    End Property

    Property SerialNo() As String
        Get
            Return _SerialNo
        End Get
        Set(ByVal Value As String)
            _SerialNo = Value
        End Set
    End Property

    Property FirmwareRev() As String
        Get
            Return _FirmwareRev
        End Get
        Set(ByVal Value As String)
            _FirmwareRev = Value
        End Set
    End Property

    Property FirmwareDate() As String
        Get
            Return _FirmwareDate
        End Get
        Set(ByVal Value As String)
            _FirmwareDate = Value
        End Set
    End Property

    Property HardwareRev() As String
        Get
            Return _HardwareRev
        End Get
        Set(ByVal Value As String)
            _HardwareRev = Value
        End Set
    End Property

    Property ComPort() As String
        Get
            Return _ComPort
        End Get
        Set(ByVal Value As String)
            _ComPort = Value
        End Set
    End Property

    Property UpdateTS() As Date
        Get
            Return _UpdateTS
        End Get
        Set(ByVal Value As Date)
            _UpdateTS = Value
        End Set
    End Property

End Class

Public Class UserDetail

    Private _UserID As Integer
    Private _UserName As String
    Private _Password As String
    Private _UserNameFirst As String
    Private _UserNameLast As String
    Private _UserEmail As String
    Private _UserPhone As String
    Private _AdminFlag As Boolean
    Private _AlertFlag As Boolean
    Private _SearchFlag As Boolean
    Private _ActiveFlag As Boolean
    Private _UpdateTS As Date

    Property UserID() As Integer
        Get
            Return _UserID
        End Get
        Set(ByVal Value As Integer)
            _UserID = Value
        End Set
    End Property

    Property UserName() As String
        Get
            Return _UserName
        End Get
        Set(ByVal Value As String)
            _UserName = Value
        End Set
    End Property

    Property Password() As String
        Get
            Return _Password
        End Get
        Set(ByVal Value As String)
            _Password = Value
        End Set
    End Property

    Property UserNameFirst() As String
        Get
            Return _UserNameFirst
        End Get
        Set(ByVal Value As String)
            _UserNameFirst = Value
        End Set
    End Property

    Property UserNameLast() As String
        Get
            Return _UserNameLast
        End Get
        Set(ByVal Value As String)
            _UserNameLast = Value
        End Set
    End Property

    Property UserEmail() As String
        Get
            Return _UserEmail
        End Get
        Set(ByVal Value As String)
            _UserEmail = Value
        End Set
    End Property

    Property UserPhone() As String
        Get
            Return _UserPhone
        End Get
        Set(ByVal Value As String)
            _UserPhone = Value
        End Set
    End Property

    Property AdminFlag() As Boolean
        Get
            Return _AdminFlag
        End Get
        Set(ByVal Value As Boolean)
            _AdminFlag = Value
        End Set
    End Property

    Property AlertFlag() As Boolean
        Get
            Return _AlertFlag
        End Get
        Set(ByVal Value As Boolean)
            _AlertFlag = Value
        End Set
    End Property

    Property SearchFlag() As Boolean
        Get
            Return _SearchFlag
        End Get
        Set(ByVal Value As Boolean)
            _SearchFlag = Value
        End Set
    End Property

    Property ActiveFlag() As Boolean
        Get
            Return _ActiveFlag
        End Get
        Set(ByVal Value As Boolean)
            _ActiveFlag = Value
        End Set
    End Property

    Property UpdateTS() As Date
        Get
            Return _UpdateTS
        End Get
        Set(ByVal Value As Date)
            _UpdateTS = Value
        End Set
    End Property

End Class

Public Class AlertDetail

    Private _AlertID As Integer
    Private _AlertType As String
    Private _IDNumber As String
    Private _NameFirst As String
    Private _NameLast As String
    Private _DateOfBirth As String
    Private _AlertContactName As String
    Private _AlertContactNumber As String
    Private _AlertNotes As String
    Private _ActiveFlag As Boolean
    Private _UserID As Integer
    Private _UserName As String
    Private _MatchLast As String
    Private _MatchID As String
    Private _UpdateTS As Date

    Property AlertID() As Integer
        Get
            Return _AlertID
        End Get
        Set(ByVal Value As Integer)
            _AlertID = Value
        End Set
    End Property

    Property AlertType() As String
        Get
            Return _AlertType
        End Get
        Set(ByVal Value As String)
            _AlertType = Value
        End Set
    End Property

    Property IDNumber() As String
        Get
            Return _IDNumber
        End Get
        Set(ByVal Value As String)
            _IDNumber = Value
        End Set
    End Property

    Property NameFirst() As String
        Get
            Return _NameFirst
        End Get
        Set(ByVal Value As String)
            _NameFirst = Value
        End Set
    End Property

    Property NameLast() As String
        Get
            Return _NameLast
        End Get
        Set(ByVal Value As String)
            _NameLast = Value
        End Set
    End Property

    Property DateOfBirth() As String
        Get
            Return _DateOfBirth
        End Get
        Set(ByVal Value As String)
            _DateOfBirth = Value
        End Set
    End Property

    Property AlertContactName() As String
        Get
            Return _AlertContactName
        End Get
        Set(ByVal Value As String)
            _AlertContactName = Value
        End Set
    End Property

    Property AlertContactNumber() As String
        Get
            Return _AlertContactNumber
        End Get
        Set(ByVal Value As String)
            _AlertContactNumber = Value
        End Set
    End Property

    Property AlertNotes() As String
        Get
            Return _AlertNotes
        End Get
        Set(ByVal Value As String)
            _AlertNotes = Value
        End Set
    End Property

    Property ActiveFlag() As Boolean
        Get
            Return _ActiveFlag
        End Get
        Set(ByVal Value As Boolean)
            _ActiveFlag = Value
        End Set
    End Property

    Property UserID() As Integer
        Get
            Return _UserID
        End Get
        Set(ByVal Value As Integer)
            _UserID = Value
        End Set
    End Property

    Property UserName() As String
        Get
            Return _UserName
        End Get
        Set(ByVal Value As String)
            _UserName = Value
        End Set
    End Property

    Property MatchLast() As String
        Get
            Return _MatchLast
        End Get
        Set(ByVal Value As String)
            _MatchLast = Value
        End Set
    End Property

    Property MatchID() As String
        Get
            Return _MatchID
        End Get
        Set(ByVal Value As String)
            _MatchID = Value
        End Set
    End Property

    Property UpdateTS() As Date
        Get
            Return _UpdateTS
        End Get
        Set(ByVal Value As Date)
            _UpdateTS = Value
        End Set
    End Property

End Class

Public Class SwipeScanDetail

    Private _SwipeScanID As Integer
    Private _CaseID As String
    Private _CheckNumber As Integer
    Private _IDAccountNumber As String
    Private _CardType As String
    Private _CCIssuer As String
    Private _NameFirst As String
    Private _NameLast As String
    Private _NameMiddle As String
    Private _DateOfBirth As String
    Private _Age As Integer
    Private _Sex As String
    Private _Height As String
    Private _Weight As String
    Private _Eyes As String
    Private _Hair As String
    Private _DateOfIssue As String
    Private _DateOfExpiration As String
    Private _AddressStreet As String
    Private _AddressCity As String
    Private _AddressState As String
    Private _AddressZip As String
    Private _SwipeRawData As String
    Private _UserName As String
    Private _Location As String
    Private _ClientID As Integer
    Private _UserID As Integer
    Private _CCDigits As Integer
    Private _DisableCCSave As Boolean
    Private _DisableDBSave As Boolean
    Private _UpdateTS As Date
    Private _DataSource As String

    Property SwipeScanID() As Integer
        Get
            Return _SwipeScanID
        End Get
        Set(ByVal Value As Integer)
            _SwipeScanID = Value
        End Set
    End Property

    Property CaseID() As String
        Get
            Return _CaseID
        End Get
        Set(ByVal Value As String)
            _CaseID = Value
        End Set
    End Property

    Property CheckNumber() As Integer
        Get
            Return _CheckNumber
        End Get
        Set(ByVal Value As Integer)
            _CheckNumber = Value
        End Set
    End Property

    Property IDAccountNumber() As String
        Get
            Return _IDAccountNumber
        End Get
        Set(ByVal Value As String)
            _IDAccountNumber = Value
        End Set
    End Property

    Property CardType() As String
        Get
            Return _CardType
        End Get
        Set(ByVal Value As String)
            _CardType = Value
        End Set
    End Property

    Property CCIssuer() As String
        Get
            Return _CCIssuer
        End Get
        Set(ByVal Value As String)
            _CCIssuer = Value
        End Set
    End Property

    Property NameFirst() As String
        Get
            Return _NameFirst
        End Get
        Set(ByVal Value As String)
            _NameFirst = Value
        End Set
    End Property

    Property NameLast() As String
        Get
            Return _NameLast
        End Get
        Set(ByVal Value As String)
            _NameLast = Value
        End Set
    End Property

    Property NameMiddle() As String
        Get
            Return _NameMiddle
        End Get
        Set(ByVal Value As String)
            _NameMiddle = Value
        End Set
    End Property

    Property DateOfBirth() As String
        Get
            Return _DateOfBirth
        End Get
        Set(ByVal Value As String)
            _DateOfBirth = Value
        End Set
    End Property

    Property Age() As Integer
        Get
            Return _Age
        End Get
        Set(ByVal Value As Integer)
            _Age = Value
        End Set
    End Property

    Property Sex() As String
        Get
            Return _Sex
        End Get
        Set(ByVal Value As String)
            _Sex = Value
        End Set
    End Property

    Property Height() As String
        Get
            Return _Height
        End Get
        Set(ByVal Value As String)
            _Height = Value
        End Set
    End Property

    Property Weight() As String
        Get
            Return _Weight
        End Get
        Set(ByVal Value As String)
            _Weight = Value
        End Set
    End Property

    Property Eyes() As String
        Get
            Return _Eyes
        End Get
        Set(ByVal Value As String)
            _Eyes = Value
        End Set
    End Property

    Property Hair() As String
        Get
            Return _Hair
        End Get
        Set(ByVal Value As String)
            _Hair = Value
        End Set
    End Property

    Property DateOfIssue() As String
        Get
            Return _DateOfIssue
        End Get
        Set(ByVal Value As String)
            _DateOfIssue = Value
        End Set
    End Property

    Property DateOfExpiration() As String
        Get
            Return _DateOfExpiration
        End Get
        Set(ByVal Value As String)
            _DateOfExpiration = Value
        End Set
    End Property

    Property AddressStreet() As String
        Get
            Return _AddressStreet
        End Get
        Set(ByVal Value As String)
            _AddressStreet = Value
        End Set
    End Property

    Property AddressCity() As String
        Get
            Return _AddressCity
        End Get
        Set(ByVal Value As String)
            _AddressCity = Value
        End Set
    End Property

    Property AddressState() As String
        Get
            Return _AddressState
        End Get
        Set(ByVal Value As String)
            _AddressState = Value
        End Set
    End Property

    Property AddressZip() As String
        Get
            Return _AddressZip
        End Get
        Set(ByVal Value As String)
            _AddressZip = Value
        End Set
    End Property

    Property SwipeRawData() As String
        Get
            Return _SwipeRawData
        End Get
        Set(ByVal Value As String)
            _SwipeRawData = Value
        End Set
    End Property

    Property UserName() As String
        Get
            Return _UserName
        End Get
        Set(ByVal Value As String)
            _UserName = Value
        End Set
    End Property

    Property Location() As String
        Get
            Return _Location
        End Get
        Set(ByVal Value As String)
            _Location = Value
        End Set
    End Property

    Property ClientID() As Integer
        Get
            Return _ClientID
        End Get
        Set(ByVal Value As Integer)
            _ClientID = Value
        End Set
    End Property

    Property UserID() As Integer
        Get
            Return _UserID
        End Get
        Set(ByVal Value As Integer)
            _UserID = Value
        End Set
    End Property

    Property CCDigits() As Integer
        Get
            Return _CCDigits
        End Get
        Set(ByVal Value As Integer)
            _CCDigits = Value
        End Set
    End Property

    Property DisableCCSave() As Boolean
        Get
            Return _DisableCCSave
        End Get
        Set(ByVal Value As Boolean)
            _DisableCCSave = Value
        End Set
    End Property

    Property DisableDBSave() As Boolean
        Get
            Return _DisableDBSave
        End Get
        Set(ByVal Value As Boolean)
            _DisableDBSave = Value
        End Set
    End Property

    Property UpdateTS() As Date
        Get
            Return _UpdateTS
        End Get
        Set(ByVal Value As Date)
            _UpdateTS = Value
        End Set
    End Property

    Property DataSource() As String
        Get
            Return _DataSource
        End Get
        Set(ByVal Value As String)
            _DataSource = Value
        End Set
    End Property

End Class

Public Class SwipeScanSearch

    Private _SwipeScanID As Integer
    Private _CaseID As String
    Private _SwipeScanType As String
    Private _Location As String
    Private _Station As String
    Private _UserName As String
    Private _IDAccountNumber As String
    Private _NameFirst As String
    Private _NameLast As String
    Private _DateOfBirth As String
    Private _ImageAvailable As Boolean
    Private _ClientID As Integer
    Private _SwipeScanTS As Date

    Property SwipeScanID() As Integer
        Get
            Return _SwipeScanID
        End Get
        Set(ByVal Value As Integer)
            _SwipeScanID = Value
        End Set
    End Property

    Property CaseID() As String
        Get
            Return _CaseID
        End Get
        Set(ByVal Value As String)
            _CaseID = Value
        End Set
    End Property

    Property SwipeScanType() As String
        Get
            Return _SwipeScanType
        End Get
        Set(ByVal Value As String)
            _SwipeScanType = Value
        End Set
    End Property

    Property Location() As String
        Get
            Return _Location
        End Get
        Set(ByVal Value As String)
            _Location = Value
        End Set
    End Property

    Property Station() As String
        Get
            Return _Station
        End Get
        Set(ByVal Value As String)
            _Station = Value
        End Set
    End Property

    Property UserName() As String
        Get
            Return _UserName
        End Get
        Set(ByVal Value As String)
            _UserName = Value
        End Set
    End Property

    Property IDAccountNumber() As String
        Get
            Return _IDAccountNumber
        End Get
        Set(ByVal Value As String)
            _IDAccountNumber = Value
        End Set
    End Property

    Property NameFirst() As String
        Get
            Return _NameFirst
        End Get
        Set(ByVal Value As String)
            _NameFirst = Value
        End Set
    End Property

    Property NameLast() As String
        Get
            Return _NameLast
        End Get
        Set(ByVal Value As String)
            _NameLast = Value
        End Set
    End Property

    Property DateOfBirth() As String
        Get
            Return _DateOfBirth
        End Get
        Set(ByVal Value As String)
            _DateOfBirth = Value
        End Set
    End Property

    Property ImageAvailable() As Boolean
        Get
            Return _ImageAvailable
        End Get
        Set(ByVal Value As Boolean)
            _ImageAvailable = Value
        End Set
    End Property

    Property ClientID() As Integer
        Get
            Return _ClientID
        End Get
        Set(ByVal Value As Integer)
            _ClientID = Value
        End Set
    End Property

    Property SwipeScanTS() As Date
        Get
            Return _SwipeScanTS
        End Get
        Set(ByVal Value As Date)
            _SwipeScanTS = Value
        End Set
    End Property

End Class

Public Class SwipeScanInfo

    Private _SwipeScanRawData As String
    Private _ScanType As String
    Private _ClientID As Integer
    Private _UserID As Integer
    Private _IDChecker As IDecode.Net.PC.Checker
    Private _CCDigits As Integer
    Private _DisableCCSave As Boolean
    Private _DisableDBSave As Boolean
    Private _BatchID As String

    Property SwipeScanRawData() As String
        Get
            Return _SwipeScanRawData
        End Get
        Set(ByVal Value As String)
            _SwipeScanRawData = Value
        End Set
    End Property

    Property ScanType() As String
        Get
            Return _ScanType
        End Get
        Set(ByVal Value As String)
            _ScanType = Value
        End Set
    End Property

    Property ClientID() As Integer
        Get
            Return _ClientID
        End Get
        Set(ByVal Value As Integer)
            _ClientID = Value
        End Set
    End Property

    Property UserID() As Integer
        Get
            Return _UserID
        End Get
        Set(ByVal Value As Integer)
            _UserID = Value
        End Set
    End Property

    Property IDChecker() As IDecode.Net.PC.Checker
        Get
            Return _IDChecker
        End Get
        Set(ByVal Value As IDecode.Net.PC.Checker)
            _IDChecker = Value
        End Set
    End Property

    Property CCDigits() As Integer
        Get
            Return _CCDigits
        End Get
        Set(ByVal Value As Integer)
            _CCDigits = Value
        End Set
    End Property

    Property DisableCCSave() As Boolean
        Get
            Return _DisableCCSave
        End Get
        Set(ByVal Value As Boolean)
            _DisableCCSave = Value
        End Set
    End Property

    Property DisableDBSave() As Boolean
        Get
            Return _DisableDBSave
        End Get
        Set(ByVal Value As Boolean)
            _DisableDBSave = Value
        End Set
    End Property

    Property BatchID() As String
        Get
            Return _BatchID
        End Get
        Set(ByVal Value As String)
            _BatchID = Value
        End Set
    End Property

End Class

Public Class SwipeScanHistory

    Private _SwipeScanID As Integer
    Private _Location As String
    Private _Station As String
    Private _Phone As String
    Private _UserName As String
    Private _SwipeScanTS As Date

    Property SwipeScanID() As Integer
        Get
            Return _SwipeScanID
        End Get
        Set(ByVal Value As Integer)
            _SwipeScanID = Value
        End Set
    End Property

    Property Location() As String
        Get
            Return _Location
        End Get
        Set(ByVal Value As String)
            _Location = Value
        End Set
    End Property

    Property Station() As String
        Get
            Return _Station
        End Get
        Set(ByVal Value As String)
            _Station = Value
        End Set
    End Property

    Property Phone() As String
        Get
            Return _Phone
        End Get
        Set(ByVal Value As String)
            _Phone = Value
        End Set
    End Property

    Property UserName() As String
        Get
            Return _UserName
        End Get
        Set(ByVal Value As String)
            _UserName = Value
        End Set
    End Property

    Property SwipeScanTS() As Date
        Get
            Return _SwipeScanTS
        End Get
        Set(ByVal Value As Date)
            _SwipeScanTS = Value
        End Set
    End Property

End Class

Public Class SwipeScanNavigateInfo

    Private _SwipeScanID As Integer
    Private _SwipeScanType As String
    Private _SwipeScanTS As Date

    Property SwipeScanID() As Integer
        Get
            Return _SwipeScanID
        End Get
        Set(ByVal Value As Integer)
            _SwipeScanID = Value
        End Set
    End Property

    Property SwipeScanType() As String
        Get
            Return _SwipeScanType
        End Get
        Set(ByVal Value As String)
            _SwipeScanType = Value
        End Set
    End Property

    Property SwipeScanTS() As Date
        Get
            Return _SwipeScanTS
        End Get
        Set(ByVal Value As Date)
            _SwipeScanTS = Value
        End Set
    End Property

End Class
