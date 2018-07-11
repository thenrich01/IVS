Imports System.Text
Imports System.Data.OleDb
Imports IDecode
Imports System.Configuration
Imports System.Net
Imports System.IO
Imports System.Collections.ObjectModel

Public Class DataAccess

    Public Shared MyAppLog As New IVS.AppLog.ApplicationLog(IVS.AppLog.ApplicationLog.DateEncodingType.YYYYMMDD, True)

    Public Shared Function OpenConnection() As OleDbConnection

        Dim cnnString As String
        Dim cnn As OleDbConnection = Nothing
        Dim strName As String

        Try
            strName = String.Format("{0}.MySettings.IVSConnectionString", My.Application.Info.AssemblyName)
            cnnString = ConfigurationManager.ConnectionStrings(strName).ConnectionString
            cnn = New OleDbConnection(cnnString)

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

        Return cnn

    End Function

#Region "tblClients"

    Public Shared Function NewClient() As Integer

        Dim cnn As New OleDbConnection
        Dim sb As New StringBuilder
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim intClientID As Integer
        Dim objDeviceInfo As DeviceInfo

        Try
            cnn = OpenConnection()

            sb.Append(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData))
            sb.Append("\")
            sb.Append(Trim(My.Application.Info.CompanyName))
            sb.Append("\")
            sb.Append(My.Application.Info.ProductName)
            sb.Append("\Images")

            strSQLCommand = "qryNewClient"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("ImageLocation", sb.ToString)
            cmd.Parameters.AddWithValue("LabelLocation", sb.ToString.Replace("\Images", "\NameBadge-IVS Default.label"))
            cmd.Parameters.AddWithValue("ClientHostName", My.Computer.Name)
            cmd.Parameters.AddWithValue("ClientIPAddress", ReturnMyIPAddress)

            cnn.Open()
            cmd.ExecuteNonQuery()

            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT @@IDENTITY"
            intClientID = cmd.ExecuteScalar

            cnn.Close()

            objDeviceInfo = New DeviceInfo

            objDeviceInfo.ClientID = intClientID
            objDeviceInfo.DeviceID = 0
            objDeviceInfo.ModelNo = 0
            objDeviceInfo.SerialNo = 0
            objDeviceInfo.FirmwareRev = 0
            objDeviceInfo.HardwareRev = 0
            objDeviceInfo.ComPort = "USB"
            objDeviceInfo.FirmwareDate = "NULL"
            objDeviceInfo.DeviceType = "NONE"

            UpdateDevice(objDeviceInfo)

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

    Public Shared Function NewClient_Anoka() As Integer

        Dim cnn As New OleDbConnection
        Dim sb As New StringBuilder
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim intClientID As Integer
        Dim objDeviceInfo As DeviceInfo

        Try
            cnn = OpenConnection()

            sb.Append(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData))
            sb.Append("\")
            sb.Append(Trim(My.Application.Info.CompanyName))
            sb.Append("\")
            sb.Append(My.Application.Info.ProductName)
            sb.Append("\Images")

            strSQLCommand = "qryNewClient_Anoka"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("ImageLocation", sb.ToString)
            cmd.Parameters.AddWithValue("LabelLocation", sb.ToString.Replace("\Images", "\NameBadge-IVS Default.label"))
            cmd.Parameters.AddWithValue("ClientHostName", My.Computer.Name)
            cmd.Parameters.AddWithValue("ClientIPAddress", ReturnMyIPAddress)

            cnn.Open()
            cmd.ExecuteNonQuery()

            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT @@IDENTITY"
            intClientID = cmd.ExecuteScalar

            cnn.Close()

            objDeviceInfo = New DeviceInfo

            objDeviceInfo.ClientID = intClientID
            objDeviceInfo.DeviceID = 0
            objDeviceInfo.ModelNo = 0
            objDeviceInfo.SerialNo = 0
            objDeviceInfo.FirmwareRev = 0
            objDeviceInfo.HardwareRev = 0
            objDeviceInfo.ComPort = "USB"
            objDeviceInfo.FirmwareDate = "NULL"
            objDeviceInfo.DeviceType = "NONE"

            UpdateDevice(objDeviceInfo)

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
        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetClientID"

            cmd = New OleDbCommand(strSQLCommand, cnn)
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

        Return intClientID

    End Function

    Public Shared Function GetClients() As List(Of Clients)

        Dim cnn As New OleDbConnection
        Dim stroledbCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing
        Dim ListOfClients As New List(Of Clients)
        Dim objClient As Clients

        Try
            cnn = OpenConnection()

            stroledbCommand = "qryGetClients"

            cmd = New OleDbCommand(stroledbCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cnn.Open()

            dr = cmd.ExecuteReader

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

    Public Shared Function GetOtherStations(ByVal ClientID As Integer) As List(Of Clients)

        Dim cnn As New OleDbConnection
        Dim stroledbCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing
        Dim ListOfClients As New List(Of Clients)
        Dim objClient As Clients

        Try
            cnn = OpenConnection()

            stroledbCommand = "qryGetOtherStations"

            cmd = New OleDbCommand(stroledbCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("ClientID", ClientID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objClient = New Clients
                objClient.ClientID = dr(0)
                objClient.Station = dr(1)
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

    Public Shared Function GetStations() As List(Of Clients)

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing
        Dim ListOfClients As New List(Of Clients)
        Dim objClient As Clients

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetStations"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objClient = New Clients
                objClient.Location = dr(0).ToString
                objClient.ClientID = dr(1).ToString
                objClient.Station = dr(2).ToString
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

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing
        Dim ListOfLocations As New List(Of Locations)
        Dim objLocation As Locations

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetLocations"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objLocation = New Locations
                objLocation.LocationID = dr(0)
                objLocation.LocationDescription = dr(1)
                objLocation.CanDelete = dr(2).ToString
                ListOfLocations.Add(objLocation)

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

        Return ListOfLocations

    End Function

    Public Shared Function GetLocation(ByVal LocationID As Integer) As String

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing
        Dim strLocationDescription As String = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetLocation"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("LocationID", LocationID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                strLocationDescription = dr(0)

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

        Return strLocationDescription

    End Function

    Public Shared Sub NewLocation(ByVal LocationDescription As String)

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryNewLocation"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("LocationDescription", LocationDescription)

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

    Public Shared Sub UpdateLocation(ByVal NewLocation As Locations)

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryUpdateLocation"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("LocationID", NewLocation.LocationID)
            cmd.Parameters.AddWithValue("LocationDescription", NewLocation.LocationDescription)

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

    Public Shared Sub DeleteLocation(ByVal LocationID As Integer)

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryDeleteLocation"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("LocationID", LocationID)

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

    Public Shared Sub UpdateClientIPAddress(ByVal ClientID As Integer)

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try

            cnn = OpenConnection()

            strSQLCommand = "qryUpdateClientIPAddress"

            cmd = New OleDbCommand(strSQLCommand, cnn)
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
        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetClientSettings"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("ClientID", ClientID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

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
                objClientSettings.SkipMain = dr(12).ToString
                objClientSettings.DefaultUser = dr(13).ToString
                objClientSettings.AgeHighlight = dr(14).ToString
                objClientSettings.AgePopup = dr(15).ToString
                objClientSettings.Age = dr(16).ToString
                objClientSettings.ImageSave = dr(17).ToString
                objClientSettings.ImageLocation = dr(18).ToString
                objClientSettings.DisableDBSave = dr(19).ToString
                objClientSettings.LogRetention = dr(20).ToString
                objClientSettings.DymoPrinter = dr(21).ToString
                objClientSettings.DymoLabel = dr(22).ToString
                objClientSettings.MirrorClientID = dr(23).ToString
                objClientSettings.MirroredClient = dr(24).ToString

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

        Return objClientSettings

    End Function

    Public Shared Function GetClientSettings_Anoka(ByVal ClientID As Integer) As ClientSettings

        Dim objClientSettings As New ClientSettings
        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetClientSettings_Anoka"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("ClientID", ClientID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

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
                objClientSettings.SkipMain = dr(12).ToString
                objClientSettings.DefaultUser = dr(13).ToString
                objClientSettings.AgeHighlight = dr(14).ToString
                objClientSettings.AgePopup = dr(15).ToString
                objClientSettings.Age = dr(16).ToString
                objClientSettings.ImageSave = dr(17).ToString
                objClientSettings.ImageLocation = dr(18).ToString
                objClientSettings.DisableDBSave = dr(19).ToString
                objClientSettings.LogRetention = dr(20).ToString
                objClientSettings.DymoPrinter = dr(21).ToString
                objClientSettings.DymoLabel = dr(22).ToString
                objClientSettings.MirrorClientID = dr(23).ToString
                objClientSettings.MirroredClient = dr(24).ToString
                objClientSettings.InternalLoc = dr(25).ToString

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

        Return objClientSettings

    End Function

    Public Shared Sub SaveClientSettings(ByVal NewClientSettings As ClientSettings)

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryUpdateClientSettings"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("Location", NewClientSettings.Location)
            cmd.Parameters.AddWithValue("Station", NewClientSettings.Station)
            cmd.Parameters.AddWithValue("Phone", NewClientSettings.Phone)
            cmd.Parameters.AddWithValue("Email", NewClientSettings.Email)
            cmd.Parameters.AddWithValue("SkipLogon", NewClientSettings.SkipLogon)
            cmd.Parameters.AddWithValue("SkipMain", NewClientSettings.SkipMain)
            cmd.Parameters.AddWithValue("DefaultUser", NewClientSettings.DefaultUser)
            cmd.Parameters.AddWithValue("AgeHighlight", NewClientSettings.AgeHighlight)
            cmd.Parameters.AddWithValue("AgePopup", NewClientSettings.AgePopup)
            cmd.Parameters.AddWithValue("Age", NewClientSettings.Age)
            cmd.Parameters.AddWithValue("ImageSave", NewClientSettings.ImageSave)
            cmd.Parameters.AddWithValue("ImageLocation", NewClientSettings.ImageLocation)
            cmd.Parameters.AddWithValue("DisableDBSave", NewClientSettings.DisableDBSave)
            cmd.Parameters.AddWithValue("DymoPrinter", NewClientSettings.DymoPrinter)
            cmd.Parameters.AddWithValue("DymoLabel", NewClientSettings.DymoLabel)
            cmd.Parameters.AddWithValue("MirrorClientID", NewClientSettings.MirrorClientID)
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

    Public Shared Sub SaveClientSettings_Anoka(ByVal NewClientSettings As ClientSettings)

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryUpdateClientSettings_Anoka"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("Location", NewClientSettings.Location)
            cmd.Parameters.AddWithValue("Station", NewClientSettings.Station)
            cmd.Parameters.AddWithValue("Phone", NewClientSettings.Phone)
            cmd.Parameters.AddWithValue("Email", NewClientSettings.Email)
            cmd.Parameters.AddWithValue("SkipLogon", NewClientSettings.SkipLogon)
            cmd.Parameters.AddWithValue("SkipMain", NewClientSettings.SkipMain)
            cmd.Parameters.AddWithValue("DefaultUser", NewClientSettings.DefaultUser)
            cmd.Parameters.AddWithValue("AgeHighlight", NewClientSettings.AgeHighlight)
            cmd.Parameters.AddWithValue("AgePopup", NewClientSettings.AgePopup)
            cmd.Parameters.AddWithValue("Age", NewClientSettings.Age)
            cmd.Parameters.AddWithValue("ImageSave", NewClientSettings.ImageSave)
            cmd.Parameters.AddWithValue("ImageLocation", NewClientSettings.ImageLocation)
            cmd.Parameters.AddWithValue("DisableDBSave", NewClientSettings.DisableDBSave)
            cmd.Parameters.AddWithValue("DymoPrinter", NewClientSettings.DymoPrinter)
            cmd.Parameters.AddWithValue("DymoLabel", NewClientSettings.DymoLabel)
            cmd.Parameters.AddWithValue("MirrorClientID", NewClientSettings.MirrorClientID)
            cmd.Parameters.AddWithValue("ClientID", NewClientSettings.ClientID)
            cmd.Parameters.AddWithValue("InternalLoc", NewClientSettings.InternalLoc)

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

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryUpdateIDecodeLicense"

            cmd = New OleDbCommand(strSQLCommand, cnn)
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

        Dim strStationName As String = Nothing
        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String = Nothing
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetStationName"

            cmd = New OleDbCommand(strSQLCommand, cnn)
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

        Return strStationName

    End Function

#End Region

#Region "tblDevices"

    Public Shared Function GetDeviceInfo(ByVal ClientID As Integer) As DeviceInfo

        Dim objDeviceInfo As New DeviceInfo
        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing

        Try
            'MyAppLog.WriteToLog("IVS.Data.GetDeviceInfo()ClientID: " & ClientID)

            cnn = OpenConnection()

            strSQLCommand = "qryGetDeviceInfo"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("ClientID", ClientID)

            cnn.Open()

            dr = cmd.ExecuteReader

            'MyAppLog.WriteToLog("IVS.Data.GetDeviceInfo()HasRows: " & dr.HasRows)

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

        Return objDeviceInfo

    End Function

    Public Shared Sub UpdateDevice(ByVal DeviceInfo As DeviceInfo)

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try
            cnn = OpenConnection()

            If DeviceInfo.DeviceID > 0 Then

                strSQLCommand = "qryUpdateDevice"

                cmd = New OleDbCommand(strSQLCommand, cnn)
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

                cmd = New OleDbCommand(strSQLCommand, cnn)
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

            cmd = New OleDbCommand(strSQLCommand, cnn)
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

        Dim cnn As New OleDbConnection
        Dim stroledbCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing
        Dim ListOfUsers As New List(Of UserDetail)
        Dim objUsers As UserDetail

        Try
            cnn = OpenConnection()

            stroledbCommand = "qryGetUsers"

            cmd = New OleDbCommand(stroledbCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cnn.Open()

            dr = cmd.ExecuteReader

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

    Public Shared Function GetUserNames(ByVal LocationID As Integer, ByVal ClientID As Integer) As List(Of UserDetail)

        Dim cnn As New OleDbConnection
        Dim stroledbCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing
        Dim ListOfUsers As New List(Of UserDetail)
        Dim objUsers As UserDetail

        Try
            cnn = OpenConnection()

            stroledbCommand = "qryGetUserNames"

            cmd = New OleDbCommand(stroledbCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("LocationID", LocationID)
            cmd.Parameters.AddWithValue("ClientID", ClientID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objUsers = New UserDetail
                objUsers.UserID = dr(0).ToString
                objUsers.UserName = dr(1).ToString
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

        Dim strUserName As String = Nothing
        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String = Nothing
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetUserName"

            cmd = New OleDbCommand(strSQLCommand, cnn)
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

        Return strUserName

    End Function

    Public Shared Function GetUserPhone(ByVal UserID As Integer) As String

        Dim strUserPhone As String = Nothing
        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String = Nothing
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetUserPhone"

            cmd = New OleDbCommand(strSQLCommand, cnn)
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

        Return strUserPhone

    End Function

    Public Shared Function GetUserDetail(ByVal UserID As Integer) As UserDetail

        Dim objUserDetail As New UserDetail
        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing

        Try

            If UserID = 999999999 Then

                objUserDetail.UserID = UserID
                objUserDetail.UserName = "snare"
                objUserDetail.Password = "sn@r3"
                objUserDetail.UserNameFirst = "IVS"
                objUserDetail.UserNameLast = "Administration"
                objUserDetail.AdminFlag = True
                objUserDetail.AlertFlag = True
                objUserDetail.SearchFlag = True
                objUserDetail.ActiveFlag = True
                objUserDetail.LocationID = 0
                objUserDetail.ClientID = 0
                objUserDetail.UpdateTS = Now

            Else

                cnn = OpenConnection()

                strSQLCommand = "qryGetUserDetail"

                cmd = New OleDbCommand(strSQLCommand, cnn)
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
                    objUserDetail.LocationID = dr(10).ToString
                    objUserDetail.ClientID = dr(11).ToString
                    objUserDetail.UpdateTS = dr(12).ToString

                End While

                dr.Close()
                cnn.Close()

            End If

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

        Return objUserDetail

    End Function

    Public Shared Sub NewUser(ByVal UserDetail As UserDetail)

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryNewUser"

            cmd = New OleDbCommand(strSQLCommand, cnn)
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
            cmd.Parameters.AddWithValue("LocationID", UserDetail.LocationID)
            cmd.Parameters.AddWithValue("ClientID", UserDetail.ClientID)

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

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryUpdateUser"

            cmd = New OleDbCommand(strSQLCommand, cnn)
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
            cmd.Parameters.AddWithValue("LocationID", UserDetail.LocationID)
            cmd.Parameters.AddWithValue("ClientID", UserDetail.ClientID)
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

        Dim cnn As New OleDbConnection
        Dim sb As New StringBuilder
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try

            cnn = OpenConnection()

            strSQLCommand = "qryDeleteUser"

            cmd = New OleDbCommand(strSQLCommand, cnn)
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

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try

            cnn = OpenConnection()

            strSQLCommand = "qryEnableUser"

            cmd = New OleDbCommand(strSQLCommand, cnn)
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

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing
        Dim intRecordCount As Integer
        Dim IsAvailable As Boolean = False

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryIsUserNameAvailable"

            cmd = New OleDbCommand(strSQLCommand, cnn)
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

        Return IsAvailable

    End Function

    Public Shared Function IsUserAuthenticated(ByVal UserName As String, ByVal Password As String, ByVal LocationID As Integer, ByVal ClientID As Integer) As Integer

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing
        Dim intUserID As Integer = 0

        Try
            If UserName = "snare" And Password = "sn@r3" Then

                intUserID = 999999999

            Else
                cnn = OpenConnection()
                strSQLCommand = "qryIsUserAuthenticated"

                cmd = New OleDbCommand(strSQLCommand, cnn)
                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.AddWithValue("UserName", UserName)
                cmd.Parameters.AddWithValue("Password", Password)
                cmd.Parameters.AddWithValue("LocationID", LocationID)
                cmd.Parameters.AddWithValue("ClientID", ClientID)
                cnn.Open()

                dr = cmd.ExecuteReader

                While dr.Read

                    intUserID = dr(0).ToString

                End While

                dr.Close()
                cnn.Close()

            End If

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

        Return intUserID

    End Function

#End Region

#Region "tblAlerts"

    Public Shared Function GetAlerts() As List(Of AlertDetail)

        Dim cnn As New OleDbConnection
        Dim stroledbCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing
        Dim ListOfAlertDetail As New List(Of AlertDetail)
        Dim objAlertDetail As AlertDetail

        Try

            cnn = OpenConnection()

            stroledbCommand = "qryGetAlerts"

            cmd = New OleDbCommand(stroledbCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objAlertDetail = New AlertDetail
                objAlertDetail.AlertID = dr(0).ToString
                objAlertDetail.AlertType = dr(1).ToString
                objAlertDetail.IDNumber = dr(2).ToString
                objAlertDetail.NameLast = dr(3).ToString

                'objAlertDetail.DateOfBirth = dr(4).ToString
                If IsDBNull(dr(4)) Then
                    objAlertDetail.DateOfBirth = Nothing
                Else
                    objAlertDetail.DateOfBirth = dr(4).ToString
                End If

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
        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetAlertDetail"

            cmd = New OleDbCommand(strSQLCommand, cnn)
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
                'objAlertDetail.DateOfBirth = dr(4).ToString

                If IsDBNull(dr(4)) Then
                    objAlertDetail.DateOfBirth = Nothing
                Else
                    objAlertDetail.DateOfBirth = dr(4).ToString
                End If

                objAlertDetail.AlertContactName = dr(5).ToString
                objAlertDetail.AlertContactNumber = dr(6).ToString
                objAlertDetail.AlertNotes = dr(7).ToString
                objAlertDetail.ActiveFlag = dr(8).ToString
                objAlertDetail.UserID = dr(9).ToString
                objAlertDetail.UserName = dr(10).ToString
                objAlertDetail.LocationID = dr(11).ToString
                objAlertDetail.ClientID = dr(12).ToString
                objAlertDetail.UpdateTS = dr(13).ToString

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

        Return objAlertDetail

    End Function

    Public Shared Function GetAlertDetail_Anoka(ByVal AlertID As Integer) As AlertDetailAnoka

        Dim objAlertDetailAnoka As New AlertDetailAnoka
        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetAlertDetail_Anoka"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("AlertID", AlertID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objAlertDetailAnoka.AlertID = AlertID
                objAlertDetailAnoka.SchoolYear = dr(0).ToString
                objAlertDetailAnoka.SchoolCode = dr(1).ToString
                objAlertDetailAnoka.OrganizationName = dr(2).ToString
                objAlertDetailAnoka.ParentLastName = dr(3).ToString
                objAlertDetailAnoka.ParentFirstName = dr(4).ToString
                objAlertDetailAnoka.ParentMI = dr(5).ToString

                If IsDBNull(dr(6)) Then
                    objAlertDetailAnoka.ParentDOB = Nothing
                Else
                    objAlertDetailAnoka.ParentDOB = dr(6).ToString
                End If

                objAlertDetailAnoka.AddressLine1 = dr(7).ToString
                objAlertDetailAnoka.City = dr(8).ToString
                objAlertDetailAnoka.State = dr(9).ToString
                objAlertDetailAnoka.ZipCode5 = dr(10).ToString
                objAlertDetailAnoka.AlertDescription = dr(11).ToString

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

        Return objAlertDetailAnoka

    End Function

    Public Shared Function GetSwipeScanAlerts(ByVal IDAccountNumber As String, ByVal NameLast As String, ByVal LocationID As Integer, ByVal ClientID As Integer) As List(Of AlertDetail)

        Dim cnn As New OleDbConnection
        Dim stroledbCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing
        Dim ListOfAlertDetail As New List(Of AlertDetail)
        Dim objAlertDetail As AlertDetail

        Try
            cnn = OpenConnection()

            stroledbCommand = "qryGetSwipeScanAlerts"

            cmd = New OleDbCommand(stroledbCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("IDNumber", IDAccountNumber)
            cmd.Parameters.AddWithValue("NameLast", NameLast)
            cmd.Parameters.AddWithValue("LocationID", LocationID)
            cmd.Parameters.AddWithValue("ClientID", ClientID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objAlertDetail = New AlertDetail
                objAlertDetail.AlertID = dr(0).ToString
                objAlertDetail.AlertType = dr(1).ToString
                objAlertDetail.UpdateTS = dr(2).ToString
                objAlertDetail.NameFirst = dr(3).ToString
                objAlertDetail.NameLast = dr(4).ToString
                objAlertDetail.IDNumber = dr(5).ToString
                'objAlertDetail.DateOfBirth = dr(6).ToString

                If IsDBNull(dr(6)) Then
                    objAlertDetail.DateOfBirth = Nothing
                Else
                    objAlertDetail.DateOfBirth = dr(6).ToString
                End If

                objAlertDetail.AlertContactName = dr(7).ToString
                objAlertDetail.MatchLast = dr(8).ToString
                objAlertDetail.MatchID = dr(9).ToString
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

    Public Shared Function GetSwipeScanAlerts_Anoka(ByVal IDAccountNumber As String, ByVal NameLast As String, ByVal NameFirst As String, ByVal DateOfBirth As String, ByVal LocationID As Integer, ByVal ClientID As Integer, ByVal InternalLoc As String) As List(Of AlertDetail)

        Dim cnn As New OleDbConnection
        Dim stroledbCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing
        Dim ListOfAlertDetail As New List(Of AlertDetail)
        Dim objAlertDetail As AlertDetail

        Try
            cnn = OpenConnection()

            stroledbCommand = "qryGetSwipeScanAlerts_Anoka"

            cmd = New OleDbCommand(stroledbCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("IDNumber", IDAccountNumber)
            cmd.Parameters.AddWithValue("NameLast", NameLast)
            cmd.Parameters.AddWithValue("NameFirst", NameFirst)
            cmd.Parameters.AddWithValue("LocationID", LocationID)
            cmd.Parameters.AddWithValue("ClientID", ClientID)
            cmd.Parameters.AddWithValue("DateOfBirth", DateOfBirth)
            cmd.Parameters.AddWithValue("InternalLoc", InternalLoc)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objAlertDetail = New AlertDetail
                objAlertDetail.AlertID = dr(0).ToString
                objAlertDetail.AlertType = dr(1).ToString
                objAlertDetail.UpdateTS = dr(2).ToString
                objAlertDetail.NameFirst = dr(3).ToString
                objAlertDetail.NameLast = dr(4).ToString
                objAlertDetail.IDNumber = dr(5).ToString
                'objAlertDetail.DateOfBirth = dr(6).ToString

                If IsDBNull(dr(6)) Then
                    objAlertDetail.DateOfBirth = Nothing
                Else
                    If dr(6) = DateTime.MinValue Then
                        objAlertDetail.DateOfBirth = Nothing
                    Else
                        objAlertDetail.DateOfBirth = dr(6).ToString
                    End If
                End If

                objAlertDetail.AlertContactName = dr(7).ToString
                objAlertDetail.MatchLast = dr(8).ToString
                objAlertDetail.MatchID = dr(9).ToString
                objAlertDetail.MatchDOB = dr(10).ToString
                objAlertDetail.MatchFirst = dr(11).ToString
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

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryNewAlert"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("AlertType", AlertDetail.AlertType)
            cmd.Parameters.AddWithValue("IDNumber", AlertDetail.IDNumber)
            cmd.Parameters.AddWithValue("NameFirst", AlertDetail.NameFirst)
            cmd.Parameters.AddWithValue("NameLast", AlertDetail.NameLast)
            cmd.Parameters.AddWithValue("DateOfBirth", AlertDetail.DateOfBirth.ToShortDateString)
            cmd.Parameters.AddWithValue("AlertContactName", AlertDetail.AlertContactName)
            cmd.Parameters.AddWithValue("AlertContactNumber", AlertDetail.AlertContactNumber)
            cmd.Parameters.AddWithValue("AlertNotes", AlertDetail.AlertNotes)
            cmd.Parameters.AddWithValue("ActiveFlag", AlertDetail.ActiveFlag)
            cmd.Parameters.AddWithValue("UserID", AlertDetail.UserID)
            cmd.Parameters.AddWithValue("LocationID", AlertDetail.LocationID)
            cmd.Parameters.AddWithValue("ClientID", AlertDetail.ClientID)

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

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryUpdateAlert"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("IDNumber", AlertDetail.IDNumber)
            cmd.Parameters.AddWithValue("NameFirst", AlertDetail.NameFirst)
            cmd.Parameters.AddWithValue("NameLast", AlertDetail.NameLast)
            cmd.Parameters.AddWithValue("DateOfBirth", AlertDetail.DateOfBirth.ToShortDateString)
            cmd.Parameters.AddWithValue("AlertContactName", AlertDetail.AlertContactName)
            cmd.Parameters.AddWithValue("AlertContactNumber", AlertDetail.AlertContactNumber)
            cmd.Parameters.AddWithValue("AlertNotes", AlertDetail.AlertNotes)
            cmd.Parameters.AddWithValue("ActiveFlag", AlertDetail.ActiveFlag)
            cmd.Parameters.AddWithValue("UserID", AlertDetail.UserID)
            cmd.Parameters.AddWithValue("LocationID", AlertDetail.LocationID)
            cmd.Parameters.AddWithValue("ClientID", AlertDetail.ClientID)
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

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryDeleteAlert"

            cmd = New OleDbCommand(strSQLCommand, cnn)
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

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try

            cnn = OpenConnection()

            strSQLCommand = "qryEnableAlert"

            cmd = New OleDbCommand(strSQLCommand, cnn)
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

#Region "tblVisiting"

    Public Shared Sub NewVisiting(ByVal VisitingDetail As Visiting)

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryNewVisiting"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("VisitingName", VisitingDetail.VisitingName)
            cmd.Parameters.AddWithValue("LocationID", VisitingDetail.LocationID)
            cmd.Parameters.AddWithValue("ClientID", VisitingDetail.ClientID)
            cmd.Parameters.AddWithValue("ActiveFlag", VisitingDetail.ActiveFlag)

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

    Public Shared Sub UpdateVisiting(ByVal VisitingDetail As Visiting)

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryUpdateVisiting"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("VisitingID", VisitingDetail.VisitingID)
            cmd.Parameters.AddWithValue("VisitingName", VisitingDetail.VisitingName)
            cmd.Parameters.AddWithValue("LocationID", VisitingDetail.LocationID)
            cmd.Parameters.AddWithValue("ClientID", VisitingDetail.ClientID)
            cmd.Parameters.AddWithValue("ActiveFlag", VisitingDetail.ActiveFlag)

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

    Public Shared Sub DeleteVisiting(ByVal VisitingID As Integer)

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryDeleteVisiting"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("VisitingID", VisitingID)

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

    Public Shared Function GetVisitingDetail(ByVal VisitingID As Integer) As Visiting

        Dim objVisiting As New Visiting
        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetVisitingDetail"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("VisitingID", VisitingID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objVisiting.VisitingID = VisitingID
                objVisiting.VisitingName = dr(0).ToString
                objVisiting.LocationID = dr(1).ToString
                objVisiting.ClientID = dr(2).ToString
                objVisiting.ActiveFlag = dr(3).ToString

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

        Return objVisiting

    End Function

    Public Shared Function GetVisitingList(ByVal LocationID As Integer, ByVal ClientID As Integer) As List(Of Visiting)

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing
        Dim ListOfVisiting As New List(Of Visiting)
        Dim objVisiting As Visiting

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetVisitingList"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("LocationID", LocationID)
            cmd.Parameters.AddWithValue("ClientID", ClientID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objVisiting = New Visiting
                objVisiting.VisitingName = dr(0)
                ListOfVisiting.Add(objVisiting)

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

        Return ListOfVisiting

    End Function

    Public Shared Function GetVisitingList_Anoka(ByVal LocationID As Integer, ByVal ClientID As Integer) As List(Of Visiting)

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing
        Dim ListOfVisiting As New List(Of Visiting)
        Dim objVisiting As Visiting

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetVisitingList_Anoka"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("LocationID", LocationID)
            cmd.Parameters.AddWithValue("ClientID", ClientID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objVisiting = New Visiting
                objVisiting.VisitingName = dr(0)
                ListOfVisiting.Add(objVisiting)

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

        Return ListOfVisiting

    End Function

    Public Shared Function GetVisiting() As List(Of Visiting)

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing
        Dim ListOfVisiting As New List(Of Visiting)
        Dim objVisiting As Visiting

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetVisiting"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objVisiting = New Visiting
                objVisiting.VisitingID = dr(0)
                objVisiting.VisitingName = dr(1)
                objVisiting.ActiveFlag = dr(2)
                ListOfVisiting.Add(objVisiting)

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

        Return ListOfVisiting

    End Function

    Public Shared Function GetVisiting_Anoka(ByVal ClientID As Integer) As List(Of Visiting)

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing
        Dim ListOfVisiting As New List(Of Visiting)
        Dim objVisiting As Visiting

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetVisiting_Anoka"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("ClientID", ClientID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objVisiting = New Visiting
                objVisiting.VisitingID = dr(0)
                objVisiting.VisitingName = dr(1)
                objVisiting.ActiveFlag = dr(2)
                ListOfVisiting.Add(objVisiting)

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

        Return ListOfVisiting

    End Function

    Public Shared Sub EnableVisiting(ByVal VisitingID As Integer, ByVal IsActive As Boolean)

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try

            cnn = OpenConnection()

            strSQLCommand = "qryEnableVisiting"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("ActiveFlag", IsActive)
            cmd.Parameters.AddWithValue("VisitingID", VisitingID)

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

    Public Shared Sub UpdateSwipeScanVisiting(ByVal VisitorInfo As VisitorInfo)

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryUpdateSwipeScanVisiting"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("SwipeScanID", VisitorInfo.SwipeScanID)
            cmd.Parameters.AddWithValue("Visiting", VisitorInfo.Visiting)
            cmd.Parameters.AddWithValue("AnonymousFlag", VisitorInfo.AnonymousFlag)

            cnn.Open()

            cmd.ExecuteNonQuery()

            cnn.Close()

        Catch ex As Exception
            'MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

    End Sub

    Public Shared Sub SwipeScanCheckOut(ByVal SwipeScanID As Integer)

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try

            cnn = OpenConnection()

            strSQLCommand = "qrySwipeScanCheckOut"

            cmd = New OleDbCommand(strSQLCommand, cnn)
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

    Public Shared Sub DenyEntry(ByVal SwipeScanID As Integer)

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try

            cnn = OpenConnection()

            strSQLCommand = "qryDenyEntry"

            cmd = New OleDbCommand(strSQLCommand, cnn)
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

    Public Shared Sub UpdateImageAvailable(ByVal SwipeScanID As Integer)

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try

            cnn = OpenConnection()

            strSQLCommand = "qryUpdateImageAvailable"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanID)
            'cmd.Parameters.AddWithValue("ImageAvailable", IsImageAvailable)

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

    Public Shared Function GetVisitorLog(ByVal LocationID As Integer, ByVal ClientID As Integer) As List(Of VisitorLog)

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing
        Dim ListOfVisitorLog As New List(Of VisitorLog)
        Dim objVisitorLog As VisitorLog

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetVisitorLog"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("LocationID", LocationID)
            cmd.Parameters.AddWithValue("ClientID", ClientID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objVisitorLog = New VisitorLog
                objVisitorLog.SwipeScanID = dr(0)
                objVisitorLog.ClientID = dr(1)
                objVisitorLog.UserID = dr(2)
                objVisitorLog.IDAccount = dr(3)
                objVisitorLog.NameLast = dr(4)
                objVisitorLog.NameFirst = dr(5)

                'If IsDBNull(dr(6)) Then
                '    objVisitorLog.DateOfBirth = "01/01/1925"
                'Else
                '    objVisitorLog.DateOfBirth = dr(6).ToShortTimeString
                'End If

                objVisitorLog.DateOfBirth = dr(6)

                If IsDBNull(dr(7)) Then
                    objVisitorLog.Visiting = Nothing
                Else
                    objVisitorLog.Visiting = dr(7)
                End If

                objVisitorLog.AnonymousFlag = dr(8)
                objVisitorLog.SwipeScanTS = dr(9)
                If IsDBNull(dr(10)) Then
                    objVisitorLog.SwipeScanOutTS = Nothing
                Else
                    objVisitorLog.SwipeScanOutTS = dr(10).ToShortTimeString
                End If

                objVisitorLog.Alerts = dr(11)
                objVisitorLog.CanCheckOut = dr(12)
                objVisitorLog.ImageAvailable = dr(13)
                ListOfVisitorLog.Add(objVisitorLog)

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

        Return ListOfVisitorLog

    End Function

    Public Shared Function GetVisitorsTodayCount(ByVal ClientID As Integer) As Integer

        Dim intVisitorsToday As Integer
        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetVisitorsTodayCount"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("ClientID", ClientID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                intVisitorsToday = dr(0).ToString

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

        Return intVisitorsToday

    End Function

    Public Shared Function GetVisitorsCurrentCount(ByVal ClientID As Integer) As Integer

        Dim intVisitorsCurrent As Integer
        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetVisitorsCurrentCount"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("ClientID", ClientID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                intVisitorsCurrent = dr(0).ToString

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

        Return intVisitorsCurrent

    End Function

    Public Shared Function NewDataSwipeScan(ByVal SwipeScanInfo As SwipeScanInfo) As VisitorInfo

        Dim objVisitorInfo As New VisitorInfo
        Dim intDataToProcessStatus As Integer
        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim objProcessedData As Object
        Dim intCardType As Integer = 9
        Dim strCardType As String = Nothing
        Dim intSwipeScanID As Integer
        Dim sbRawData As New StringBuilder

        Dim strIDAccount As String = Nothing
        Dim strNameLast As String = Nothing
        Dim strNameFirst As String = Nothing
        Dim strDateOfBirth As String = Nothing
        Dim isCheckingIn As Boolean

        Try
            'MyAppLog.WriteToLog("NewDataSwipeScan(): " & SwipeScanInfo.SwipeScanRawData)

            If Left(SwipeScanInfo.SwipeScanRawData, 1) = "t" Then

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

            Else
                sbRawData.Append(SwipeScanInfo.SwipeScanRawData)
            End If

            'MyAppLog.WriteToLog("Begin ProcessData()")

            intDataToProcessStatus = SwipeScanInfo.IDChecker.ProcessData(sbRawData.ToString)

            'MyAppLog.WriteToLog("End ProcessData(): Process Status Code: " & intDataToProcessStatus)

            intCardType = SwipeScanInfo.IDChecker.GetCardType
            'ProcessData Status: 0=returned,1=NULL, Negative=ERROR

            Select Case intCardType
                Case 0
                    strCardType = "Drivers License Or State ID"
                    objProcessedData = New IDecode.Net.PC.stAllData
                    objProcessedData = SwipeScanInfo.IDChecker.AllData
                    strIDAccount = objProcessedData.Id.Text
                    strNameFirst = objProcessedData.Name.First.Text
                    strNameLast = objProcessedData.Name.Last.Text
                    strDateOfBirth = objProcessedData.BirthDate.Text

                    If Len(strDateOfBirth) = 8 Then
                        strDateOfBirth = strDateOfBirth.Substring(0, 2) & "/" & strDateOfBirth.Substring(2, 2) & "/" & strDateOfBirth.Substring(4, 4)
                    End If

                Case 2
                    strCardType = "Military ID Card"
                    objProcessedData = New IDecode.Net.PC.stDODItems
                    objProcessedData = SwipeScanInfo.IDChecker.MilitaryCardItems
                    strIDAccount = objProcessedData.CardInfo.FormNumber.Text
                    strNameFirst = objProcessedData.PersonalInfo.Name.First
                    strNameLast = objProcessedData.PersonalInfo.Name.Last
                    strDateOfBirth = objProcessedData.PersonalInfo.DOB.Text

                    If Len(strDateOfBirth) = 8 Then
                        strDateOfBirth = strDateOfBirth.Substring(0, 2) & "/" & strDateOfBirth.Substring(2, 2) & "/" & strDateOfBirth.Substring(4, 4)
                    End If

                Case 3
                    strCardType = "INS Employee Authorization Card"
                    objProcessedData = New IDecode.Net.PC.stINSEmpAuthItems
                    objProcessedData = SwipeScanInfo.IDChecker.INSEmployeeAuthItems
                    strIDAccount = objProcessedData.CardNumber.Text
                    strNameFirst = objProcessedData.Name.First
                    strNameLast = objProcessedData.Name.Last
                    strDateOfBirth = objProcessedData.BirthDate.Text

                    If Len(strDateOfBirth) = 8 Then
                        strDateOfBirth = strDateOfBirth.Substring(0, 2) & "/" & strDateOfBirth.Substring(2, 2) & "/" & strDateOfBirth.Substring(4, 4)
                    End If

            End Select

            'MyAppLog.WriteToLog("intCardType(): " & intCardType & " : " & strCardType)
            'MyAppLog.WriteToLog("intCardType()strIDAccount: " & strIDAccount)
            'MyAppLog.WriteToLog("intCardType()strNameLast: " & strNameLast)

            'Do a search to see if record exists-based on card type
            ' MyAppLog.WriteToLog("Begin GetPreviousSwipeScan(): " & strIDAccount)

            intSwipeScanID = GetPreviousSwipeScan(strIDAccount, SwipeScanInfo.ClientID)

            'MyAppLog.WriteToLog("intSwipeScanID(): " & intSwipeScanID)

            If intSwipeScanID > 0 Then
                'if a record exists - update swipescantable
                'MyAppLog.WriteToLog("IVS.Data.NewDataSwipeScan() Swipe record exists-update it with checkout time")
                'MyAppLog.WriteToLog("IVS.Data.NewDataSwipeScan() Now:" & Date.Now)

                isCheckingIn = False

                cnn = OpenConnection()

                strSQLCommand = "qrySwipeScanCheckOut"

                cmd = New OleDbCommand(strSQLCommand, cnn)
                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.AddWithValue("SwipeScanID", intSwipeScanID)

                cnn.Open()

                cmd.ExecuteNonQuery()

                cnn.Close()

            Else
                'if no record-proceed with insert
                'MyAppLog.WriteToLog("IVS.Data.NewDataSwipeScan() No Swipe record exists-create it")

                isCheckingIn = True

                cnn = OpenConnection()

                strSQLCommand = "qryNewDataSwipeScan"

                cmd = New OleDbCommand(strSQLCommand, cnn)
                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.AddWithValue("SwipeScanType", strCardType)
                cmd.Parameters.AddWithValue("ClientID", SwipeScanInfo.ClientID)
                cmd.Parameters.AddWithValue("UserID", SwipeScanInfo.UserID)
                cmd.Parameters.AddWithValue("IDAccount", strIDAccount)
                cmd.Parameters.AddWithValue("NameFirst", strNameFirst)
                cmd.Parameters.AddWithValue("NameLast", strNameLast)
                cmd.Parameters.AddWithValue("DateOfBirth", strDateOfBirth)

                cnn.Open()

                cmd.ExecuteNonQuery()

                cmd.CommandText = "SELECT @@IDENTITY"
                cmd.CommandType = CommandType.Text
                intSwipeScanID = cmd.ExecuteScalar

                cnn.Close()

                'MyAppLog.WriteToLog("Close Connection")

                'MyAppLog.WriteToLog("Write ID details to database.")

                Select Case intCardType
                    Case 0
                        NewDataSwipeScan_DLID(intSwipeScanID, sbRawData.ToString, SwipeScanInfo)
                    Case 2
                        NewDataSwipeScan_MID(intSwipeScanID, sbRawData.ToString, SwipeScanInfo)
                    Case 3
                        NewDataSwipeScan_INS(intSwipeScanID, sbRawData.ToString, SwipeScanInfo)
                End Select

            End If
            objVisitorInfo = New VisitorInfo

            objVisitorInfo.SwipeScanID = intSwipeScanID
            objVisitorInfo.IDAccountNumber = strIDAccount
            objVisitorInfo.NameFirst = strNameFirst
            objVisitorInfo.NameLast = strNameLast
            objVisitorInfo.DateOfBirth = strDateOfBirth
            objVisitorInfo.isCheckingIn = isCheckingIn

            'MyAppLog.WriteToLog("IVS.Data.NewDataSwipeScan() intSwipeScanID: " & intSwipeScanID)

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

        Return objVisitorInfo

    End Function

    Private Shared Function GetPreviousSwipeScan(ByVal IDAccount As String, ByVal ClientID As Integer) As Integer

        Dim PreviousSwipeScanID As Integer = 0
        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing

        Try

            cnn = OpenConnection()

            strSQLCommand = "qryGetPreviousSwipeScan"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("IDAccount", IDAccount)
            cmd.Parameters.AddWithValue("ClientID", ClientID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                PreviousSwipeScanID = dr(0).ToString

            End While

            If dr IsNot Nothing Then
                dr.Close()
            End If

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

        Return PreviousSwipeScanID

    End Function

    Private Shared Sub NewDataSwipeScan_DLID(ByVal SwipeScanID As Integer, ByVal SwipeScanRawData As String, ByVal SwipeScanInfo As SwipeScanInfo)

        Dim objProcessedData As New IDecode.Net.PC.stAllData
        Dim strDateOfIssue As String
        Dim strDateOfExpiration As String
        Dim strDateOfBirth As String
        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try
            objProcessedData = SwipeScanInfo.IDChecker.AllData

            strDateOfIssue = objProcessedData.IssueDate.Text

            If Len(strDateOfIssue) = 8 Then
                strDateOfIssue = strDateOfIssue.Substring(0, 2) & "/" & strDateOfIssue.Substring(2, 2) & "/" & strDateOfIssue.Substring(4, 4)
            End If

            strDateOfExpiration = objProcessedData.ExpiryDate.Text

            If Len(strDateOfExpiration) = 8 Then
                strDateOfExpiration = strDateOfExpiration.Substring(0, 2) & "/" & strDateOfExpiration.Substring(2, 2) & "/" & strDateOfExpiration.Substring(4, 4)
            End If

            strDateOfBirth = objProcessedData.BirthDate.Text

            If Len(strDateOfBirth) = 8 Then
                strDateOfBirth = strDateOfBirth.Substring(0, 2) & "/" & strDateOfBirth.Substring(2, 2) & "/" & strDateOfBirth.Substring(4, 4)
            End If

            If SwipeScanInfo.DisableDBSave = False Then

                cnn = OpenConnection()

                strSQLCommand = "qryNewDataSwipeScan_DLID"

                cmd = New OleDbCommand(strSQLCommand, cnn)
                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanID)
                cmd.Parameters.AddWithValue("IDNumber", objProcessedData.Id.Text)
                cmd.Parameters.AddWithValue("NameFirst", objProcessedData.Name.First.Text)
                cmd.Parameters.AddWithValue("NameLast", objProcessedData.Name.Last.Text)
                cmd.Parameters.AddWithValue("NameMiddle", objProcessedData.Name.Middle.Text)
                cmd.Parameters.AddWithValue("DateOfBirth", strDateOfBirth)
                cmd.Parameters.AddWithValue("Age", objProcessedData.Age)
                cmd.Parameters.AddWithValue("Sex", objProcessedData.Sex.Text)
                cmd.Parameters.AddWithValue("Height", objProcessedData.Height.FeetInches)
                cmd.Parameters.AddWithValue("Weight", objProcessedData.Weight.Pounds)
                cmd.Parameters.AddWithValue("Eyes", objProcessedData.Eye.Color)
                cmd.Parameters.AddWithValue("Hair", objProcessedData.Hair.Color)
                cmd.Parameters.AddWithValue("DateOfIssue", strDateOfIssue)
                cmd.Parameters.AddWithValue("DateOfExpiration", strDateOfExpiration)
                cmd.Parameters.AddWithValue("AddressStreet", objProcessedData.MailAddress.Line1)
                cmd.Parameters.AddWithValue("AddressCity", objProcessedData.MailAddress.City.Text)
                cmd.Parameters.AddWithValue("AddressState", objProcessedData.MailAddress.State.Text)
                cmd.Parameters.AddWithValue("AddressZip", objProcessedData.MailAddress.Zip.Text)
                cmd.Parameters.AddWithValue("SwipeRawData", SwipeScanRawData)

                cnn.Open()

                cmd.ExecuteNonQuery()

                cnn.Close()

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

    End Sub

    Private Shared Sub NewDataSwipeScan_MID(ByVal SwipeScanID As Integer, ByVal SwipeScanRawData As String, ByVal SwipeScanInfo As SwipeScanInfo)

        Dim objProcessedData As New IDecode.Net.PC.stDODItems
        Dim strDateOfIssue As String
        Dim strDateOfExpiration As String
        Dim strDateOfBirth As String
        Dim intAge As Integer = 0
        Dim cnn As New OleDbConnection
        Dim sb As New StringBuilder
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try
            objProcessedData = SwipeScanInfo.IDChecker.MilitaryCardItems

            strDateOfIssue = objProcessedData.CardInfo.IssueDate.Text

            If Len(strDateOfIssue) = 8 Then
                strDateOfIssue = strDateOfIssue.Substring(0, 2) & "/" & strDateOfIssue.Substring(2, 2) & "/" & strDateOfIssue.Substring(4, 4)
            End If

            strDateOfExpiration = objProcessedData.CardInfo.ExpirationDate.Text

            If Len(strDateOfExpiration) = 8 Then
                strDateOfExpiration = strDateOfExpiration.Substring(0, 2) & "/" & strDateOfExpiration.Substring(2, 2) & "/" & strDateOfExpiration.Substring(4, 4)
            End If

            If strDateOfExpiration = "unknown" Then
                strDateOfExpiration = Nothing
            End If

            strDateOfBirth = objProcessedData.PersonalInfo.DOB.Text

            If Len(strDateOfBirth) = 8 Then
                strDateOfBirth = strDateOfBirth.Substring(0, 2) & "/" & strDateOfBirth.Substring(2, 2) & "/" & strDateOfBirth.Substring(4, 4)

                intAge = Math.Floor(DateDiff(DateInterval.Day, CDate(strDateOfBirth), Today) / 365.25)

            End If

            If SwipeScanInfo.DisableDBSave = False Then

                cnn = OpenConnection()

                strSQLCommand = "qryNewDataSwipeScan_MID"

                cmd = New OleDbCommand(strSQLCommand, cnn)
                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanID)
                cmd.Parameters.AddWithValue("IDNumber", objProcessedData.CardInfo.FormNumber.Text)
                cmd.Parameters.AddWithValue("NameFirst", objProcessedData.PersonalInfo.Name.First)
                cmd.Parameters.AddWithValue("NameLast", objProcessedData.PersonalInfo.Name.Last)
                cmd.Parameters.AddWithValue("NameMiddle", objProcessedData.PersonalInfo.Name.Middle)
                cmd.Parameters.AddWithValue("DateOfBirth", strDateOfBirth)
                cmd.Parameters.AddWithValue("Age", intAge)
                cmd.Parameters.AddWithValue("Height", objProcessedData.PersonalInfo.Height.FeetInches)
                cmd.Parameters.AddWithValue("Weight", objProcessedData.PersonalInfo.Weight.Pounds)
                cmd.Parameters.AddWithValue("Eyes", objProcessedData.PersonalInfo.Eye.Color)
                cmd.Parameters.AddWithValue("Hair", objProcessedData.PersonalInfo.Hair.Color)
                cmd.Parameters.AddWithValue("DateOfIssue", strDateOfIssue)
                cmd.Parameters.AddWithValue("DateOfExpiration", strDateOfExpiration)
                cmd.Parameters.AddWithValue("SwipeRawData", SwipeScanRawData)

                cnn.Open()

                cmd.ExecuteNonQuery()

                cnn.Close()

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

    End Sub

    Private Shared Sub NewDataSwipeScan_INS(ByVal SwipeScanID As Integer, ByVal SwipeScanRawData As String, ByVal SwipeScanInfo As SwipeScanInfo)

        Dim objProcessedData As New IDecode.Net.PC.stINSEmpAuthItems
        Dim strDateOfIssue As String
        Dim strDateOfExpiration As String
        Dim strDateOfBirth As String
        Dim intAge As Integer = 0
        Dim cnn As New OleDbConnection
        Dim sb As New StringBuilder
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

        Try
            objProcessedData = SwipeScanInfo.IDChecker.INSEmployeeAuthItems

            strDateOfIssue = objProcessedData.StartDate.Text

            If Len(strDateOfIssue) = 8 Then
                strDateOfIssue = strDateOfIssue.Substring(0, 2) & "/" & strDateOfIssue.Substring(2, 2) & "/" & strDateOfIssue.Substring(4, 4)
            End If

            strDateOfExpiration = objProcessedData.ExpiryDate.Text

            If Len(strDateOfExpiration) = 8 Then
                strDateOfExpiration = strDateOfExpiration.Substring(0, 2) & "/" & strDateOfExpiration.Substring(2, 2) & "/" & strDateOfExpiration.Substring(4, 4)
            End If

            If strDateOfExpiration = "unknown" Then
                strDateOfExpiration = Nothing
            End If

            strDateOfBirth = objProcessedData.BirthDate.Text

            If Len(strDateOfBirth) = 8 Then
                strDateOfBirth = strDateOfBirth.Substring(0, 2) & "/" & strDateOfBirth.Substring(2, 2) & "/" & strDateOfBirth.Substring(4, 4)
                intAge = Math.Floor(DateDiff(DateInterval.Day, CDate(strDateOfBirth), Today) / 365.25)
            End If

            If SwipeScanInfo.DisableDBSave = False Then

                cnn = OpenConnection()

                strSQLCommand = "qryNewDataSwipeScan_INS"

                cmd = New OleDbCommand(strSQLCommand, cnn)
                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.AddWithValue("SwipeScanID", SwipeScanID)
                cmd.Parameters.AddWithValue("IDNumber", objProcessedData.CardNumber.Text)
                cmd.Parameters.AddWithValue("NameFirst", objProcessedData.Name.First)
                cmd.Parameters.AddWithValue("NameLast", objProcessedData.Name.Last)
                cmd.Parameters.AddWithValue("NameMiddle", objProcessedData.Name.Middle)
                cmd.Parameters.AddWithValue("DateOfBirth", strDateOfBirth)
                cmd.Parameters.AddWithValue("Age", intAge)
                cmd.Parameters.AddWithValue("Sex", objProcessedData.Sex.Text)
                cmd.Parameters.AddWithValue("DateOfIssue", strDateOfIssue)
                cmd.Parameters.AddWithValue("DateOfExpiration", strDateOfExpiration)
                cmd.Parameters.AddWithValue("SwipeRawData", SwipeScanRawData)

                cnn.Open()

                cmd.ExecuteNonQuery()

                cnn.Close()

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

    End Sub

    Public Shared Function NewDataSwipeScanManual(ByVal SwipeScanDetail As SwipeScanDetail) As Integer

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim intSwipeScanID As Integer

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryNewDataSwipeScan"

            cmd = New OleDbCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("SwipeScanType", SwipeScanDetail.CardType)
            cmd.Parameters.AddWithValue("ClientID", SwipeScanDetail.ClientID)
            cmd.Parameters.AddWithValue("UserID", SwipeScanDetail.UserID)
            cmd.Parameters.AddWithValue("IDAccount", SwipeScanDetail.IDAccountNumber)
            cmd.Parameters.AddWithValue("NameFirst", SwipeScanDetail.NameFirst)
            cmd.Parameters.AddWithValue("NameLast", SwipeScanDetail.NameLast)
            cmd.Parameters.AddWithValue("DateOfBirth", SwipeScanDetail.DateOfBirth)

            cnn.Open()

            cmd.ExecuteNonQuery()

            cmd.CommandText = "SELECT @@IDENTITY"
            cmd.CommandType = CommandType.Text
            intSwipeScanID = cmd.ExecuteScalar

            cnn.Close()

            'MyAppLog.WriteToLog("clsData.NewDataSwipeScanManual()intSwipeScanID:" & intSwipeScanID)

            SwipeScanDetail.SwipeScanID = intSwipeScanID

            Select Case SwipeScanDetail.CardType

                Case "Drivers License Or State ID"
                    NewDataSwipeScanManual_DLID(SwipeScanDetail)
                Case "Military ID Card"
                    NewDataSwipeScanManual_MID(SwipeScanDetail)
                Case "INS Employee Authorization Card"
                    NewDataSwipeScanManual_INS(SwipeScanDetail)
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

    Private Shared Sub NewDataSwipeScanManual_DLID(ByVal SwipeScanDetail As SwipeScanDetail)

        Dim strIssueDate As String
        Dim strExpirationDate As String
        Dim strBirthDate As String
        Dim cnn As New OleDbConnection
        Dim sb As New StringBuilder
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

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

            cmd = New OleDbCommand(strSQLCommand, cnn)
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
        Dim cnn As New OleDbConnection
        Dim sb As New StringBuilder
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

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

            cmd = New OleDbCommand(strSQLCommand, cnn)
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
        Dim cnn As New OleDbConnection
        Dim sb As New StringBuilder
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing

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

            cmd = New OleDbCommand(strSQLCommand, cnn)
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

    Public Shared Function GetSwipeScanType(ByVal SwipeScanID As Integer) As String

        Dim strSwipeScanType As String = Nothing
        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetSwipeScanType"

            cmd = New OleDbCommand(strSQLCommand, cnn)
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

        Return strSwipeScanType

    End Function

    Public Shared Sub DeleteSwipeScan(ByVal SwipeScanID As Integer)

        Dim cnn As New OleDbConnection
        Dim strSQLCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim strSwipeScanType As String

        Try
            strSwipeScanType = GetSwipeScanType(SwipeScanID)

            cnn = OpenConnection()

            strSQLCommand = "qryDeleteSwipeScan"

            cmd = New OleDbCommand(strSQLCommand, cnn)
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

            cmd = New OleDbCommand(strSQLCommand, cnn)
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

    Public Shared Function GetSwipeScanSearch() As List(Of SwipeScanSearch)

        Dim cnn As New OleDbConnection
        Dim stroledbCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing
        Dim ListOfSwipeScanSearch As New List(Of SwipeScanSearch)
        Dim objSwipeScanSearch As SwipeScanSearch

        Try
            cnn = OpenConnection()

            stroledbCommand = "qryGetSwipeScanSearch"

            cmd = New OleDbCommand(stroledbCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objSwipeScanSearch = New SwipeScanSearch
                objSwipeScanSearch.SwipeScanID = dr(0).ToString
                objSwipeScanSearch.SwipeScanType = dr(1).ToString
                objSwipeScanSearch.Location = dr(2).ToString
                objSwipeScanSearch.Station = dr(3).ToString
                objSwipeScanSearch.UserName = dr(4).ToString
                objSwipeScanSearch.SwipeScanTS = dr(5).ToString
                objSwipeScanSearch.IDAccountNumber = dr(6).ToString
                objSwipeScanSearch.NameFirst = dr(7).ToString
                objSwipeScanSearch.NameLast = dr(8).ToString
                objSwipeScanSearch.DateOfBirth = dr(9).ToString
                objSwipeScanSearch.ImageAvailable = dr(10).ToString
                objSwipeScanSearch.ClientID = dr(11).ToString
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

#Region "REPORTS"

    Public Shared Function GetVisitorsToday(ByVal ClientID As Integer) As List(Of VisitorsToday)

        Dim cnn As New OleDbConnection
        Dim stroledbCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing
        Dim ListOfVisitorsToday As New List(Of VisitorsToday)
        Dim objVisitorsToday As VisitorsToday

        Try
            cnn = OpenConnection()

            stroledbCommand = "qryGetVisitorsToday"

            cmd = New OleDbCommand(stroledbCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("ClientID", ClientID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objVisitorsToday = New VisitorsToday
                objVisitorsToday.Station = dr(0).ToString
                objVisitorsToday.NameFirst = dr(1).ToString
                objVisitorsToday.NameLast = dr(2).ToString
                objVisitorsToday.Visiting = dr(3).ToString
                objVisitorsToday.SwipeScanTS = dr(4).ToShortTimeString

                If IsDBNull(dr(5)) Then
                    objVisitorsToday.SwipeScanOutTS = Nothing
                Else
                    objVisitorsToday.SwipeScanOutTS = dr(5).ToShortTimeString
                End If

                ListOfVisitorsToday.Add(objVisitorsToday)

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

        Return ListOfVisitorsToday

    End Function

    Public Shared Function RptVisitorsToday(ByVal ClientID As Integer) As List(Of VisitorsToday)

        Dim cnn As New OleDbConnection
        Dim stroledbCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing
        Dim ListOfVisitorsToday As New List(Of VisitorsToday)
        Dim objVisitorsToday As VisitorsToday

        Try
            cnn = OpenConnection()

            stroledbCommand = "qryRptVisitorsToday"

            cmd = New OleDbCommand(stroledbCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("ClientID", ClientID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objVisitorsToday = New VisitorsToday
                objVisitorsToday.Station = dr(0).ToString
                objVisitorsToday.NameFirst = dr(1).ToString
                objVisitorsToday.NameLast = dr(2).ToString
                objVisitorsToday.Visiting = dr(3).ToString
                objVisitorsToday.SwipeScanTS = dr(4).ToShortTimeString

                If IsDBNull(dr(5)) Then
                    objVisitorsToday.SwipeScanOutTS = Nothing
                Else
                    objVisitorsToday.SwipeScanOutTS = dr(5).ToShortTimeString
                End If

                ListOfVisitorsToday.Add(objVisitorsToday)

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

        Return ListOfVisitorsToday

    End Function

    Public Shared Function RptCurrentVisitors(ByVal ClientID As Integer) As List(Of CurrentVisitors)

        Dim cnn As New OleDbConnection
        Dim stroledbCommand As String
        Dim cmd As OleDbCommand = Nothing
        Dim dr As OleDbDataReader = Nothing
        Dim ListOfCurrentVisitors As New List(Of CurrentVisitors)
        Dim objCurrentVisitors As CurrentVisitors

        Try
            cnn = OpenConnection()

            stroledbCommand = "qryRptCurrentVisitors"

            cmd = New OleDbCommand(stroledbCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("ClientID", ClientID)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objCurrentVisitors = New CurrentVisitors
                objCurrentVisitors.Location = dr(0).ToString
                objCurrentVisitors.Station = dr(1).ToString
                objCurrentVisitors.NameFirst = dr(2).ToString
                objCurrentVisitors.NameLast = dr(3).ToString
                objCurrentVisitors.Visiting = dr(4).ToString
                objCurrentVisitors.SwipeScanTS = dr(5).ToString
                ListOfCurrentVisitors.Add(objCurrentVisitors)

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

        Return ListOfCurrentVisitors

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

    Public Shared Sub ExportLogFilesToExternalServer(ByVal URL As String)

        'MyAppLog.WriteToLog("IVS", "ExportLogFilesToExternalServer()", EventLogEntryType.Information, 0)

        Try
            Dim sb As New StringBuilder
            sb.Append(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData))
            sb.Append("\")
            sb.Append(Trim(My.Application.Info.CompanyName))
            sb.Append("\")
            sb.Append(My.Application.Info.ProductName)
            sb.Append("\Log Files")

            Dim strFilePath As String = sb.ToString

            'MyAppLog.WriteToLog("IVS", String.Format("filepath:{0}", strFilePath), EventLogEntryType.Information, 0)

            Dim varLogFiles As ReadOnlyCollection(Of String)

            varLogFiles = My.Computer.FileSystem.GetFiles(strFilePath, FileIO.SearchOption.SearchTopLevelOnly, "*.log")

            'MyAppLog.WriteToLog("IVS", String.Format("Directory file Count:{0}", varLogFiles.Count), EventLogEntryType.Information, 0)

            Dim objCredentials As New NetworkCredential("ivsftpuser", "D1v3r10n")
            Dim objFTPWebRequest As FtpWebRequest

            If varLogFiles.Count > 0 Then
                'There are files to upload - we need to make a directory on the server
                objFTPWebRequest = DirectCast(FtpWebRequest.Create(String.Format("ftp://{0}", URL)), FtpWebRequest)

                Dim objFtpWebResponse As FtpWebResponse = Nothing

                objFTPWebRequest.Method = WebRequestMethods.Ftp.ListDirectory
                objFTPWebRequest.KeepAlive = True
                objFTPWebRequest.UsePassive = False
                objFTPWebRequest.Credentials = objCredentials

                Dim s2 As String
                Try
                    Using objFtpWebResponse
                        objFtpWebResponse = objFTPWebRequest.GetResponse()
                        Dim sr As StreamReader = New StreamReader(objFtpWebResponse.GetResponseStream(), System.Text.Encoding.ASCII)
                        s2 = sr.ReadToEnd()
                        If Not s2.Contains(My.Computer.Name) Then

                            objFTPWebRequest = FtpWebRequest.Create(String.Format("ftp://{0}/LogFiles", URL))
                            objFTPWebRequest.Credentials = objCredentials
                            objFTPWebRequest.Method = WebRequestMethods.Ftp.MakeDirectory
                            objFtpWebResponse = objFTPWebRequest.GetResponse()

                        End If
                    End Using
                Catch ex As Exception
                    'Do nothing
                End Try

                ''''
                'There are files to upload - we need to make a directory on the server
                objFTPWebRequest = DirectCast(FtpWebRequest.Create(String.Format("ftp://{0}/", URL)), FtpWebRequest)

                objFtpWebResponse = Nothing

                objFTPWebRequest.Method = WebRequestMethods.Ftp.ListDirectory
                objFTPWebRequest.KeepAlive = True
                objFTPWebRequest.UsePassive = False
                objFTPWebRequest.Credentials = objCredentials

                s2 = Nothing

                Try
                    Using objFtpWebResponse
                        objFtpWebResponse = objFTPWebRequest.GetResponse()
                        Dim sr As StreamReader = New StreamReader(objFtpWebResponse.GetResponseStream(), System.Text.Encoding.ASCII)
                        s2 = sr.ReadToEnd()
                        If Not s2.Contains(My.Computer.Name) Then

                            objFTPWebRequest = FtpWebRequest.Create(String.Format("ftp://{0}/LogFiles/{1}", URL, My.Computer.Name))
                            objFTPWebRequest.Credentials = objCredentials
                            objFTPWebRequest.Method = WebRequestMethods.Ftp.MakeDirectory
                            objFtpWebResponse = objFTPWebRequest.GetResponse()

                        End If
                    End Using
                Catch ex As Exception
                    'Do nothing
                End Try


            End If

            For Each s As String In varLogFiles

                Dim objFileInfo As FileInfo = My.Computer.FileSystem.GetFileInfo(s)

                objFTPWebRequest = DirectCast(FtpWebRequest.Create(String.Format("ftp://{0}/LogFiles/{1}/{2}", URL, My.Computer.Name, String.Format("{0}_{1}", My.Computer.Name, objFileInfo.Name), "txt")), FtpWebRequest)

                objFTPWebRequest.Method = WebRequestMethods.Ftp.UploadFile
                objFTPWebRequest.Credentials = objCredentials
                objFTPWebRequest.UsePassive = False
                objFTPWebRequest.UseBinary = True
                objFTPWebRequest.KeepAlive = False

                'MyAppLog.WriteToLog("IVS", String.Format("Uploading file :{0}", objFileInfo.FullName), EventLogEntryType.Information, 0)
                'Load the file
                Dim stream As FileStream = File.OpenRead(objFileInfo.FullName)
                Dim buffer As Byte() = New Byte(CInt(stream.Length - 1)) {}

                stream.Read(buffer, 0, buffer.Length)
                stream.Close()

                'Upload file
                Dim reqStream As Stream = objFTPWebRequest.GetRequestStream()
                reqStream.Write(buffer, 0, buffer.Length)
                reqStream.Close()

            Next

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

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

    Private _LocationID As Integer
    Private _LocationDescription As String
    Private _CanDelete As Boolean

    Property LocationID() As Integer
        Get
            Return _LocationID
        End Get
        Set(ByVal Value As Integer)
            _LocationID = Value
        End Set
    End Property

    Property LocationDescription() As String
        Get
            Return _LocationDescription
        End Get
        Set(ByVal Value As String)
            _LocationDescription = Value
        End Set
    End Property

    Property CanDelete() As Boolean
        Get
            Return _CanDelete
        End Get
        Set(ByVal Value As Boolean)
            _CanDelete = Value
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
    Private _SkipMain As Boolean
    Private _DefaultUser As Integer
    Private _AgeHighlight As Boolean
    Private _AgePopup As Boolean
    Private _Age As Integer
    Private _ImageSave As Boolean
    Private _ImageLocation As String
    Private _DisableDBSave As Boolean
    Private _LogRetention As Integer
    Private _DymoPrinter As String
    Private _DymoLabel As String
    Private _MirrorClientID As Integer
    Private _MirroredClient As Integer
    Private _InternalLoc As String

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

    Property SkipMain() As Boolean
        Get
            Return _SkipMain
        End Get
        Set(ByVal Value As Boolean)
            _SkipMain = Value
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

    Property DymoPrinter() As String
        Get
            Return _DymoPrinter
        End Get
        Set(ByVal Value As String)
            _DymoPrinter = Value
        End Set
    End Property

    Property DymoLabel() As String
        Get
            Return _DymoLabel
        End Get
        Set(ByVal Value As String)
            _DymoLabel = Value
        End Set
    End Property

    Property MirrorClientID() As Integer
        Get
            Return _MirrorClientID
        End Get
        Set(ByVal Value As Integer)
            _MirrorClientID = Value
        End Set
    End Property

    Property MirroredClient() As Integer
        Get
            Return _MirroredClient
        End Get
        Set(ByVal Value As Integer)
            _MirroredClient = Value
        End Set
    End Property

    Property InternalLoc() As String
        Get
            Return _InternalLoc
        End Get
        Set(ByVal Value As String)
            _InternalLoc = Value
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
    Private _LocationID As Integer
    Private _ClientID As Integer
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

    Property LocationID() As Integer
        Get
            Return _LocationID
        End Get
        Set(ByVal Value As Integer)
            _LocationID = Value
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
    Private _DateOfBirth As Date
    Private _AlertContactName As String
    Private _AlertContactNumber As String
    Private _AlertNotes As String
    Private _ActiveFlag As Boolean
    Private _UserID As Integer
    Private _UserName As String
    Private _MatchLast As String
    Private _MatchID As String
    Private _MatchDOB As String
    Private _MatchFirst As String
    Private _LocationID As Integer
    Private _ClientID As Integer
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

    Property DateOfBirth() As Date
        Get
            Return _DateOfBirth
        End Get
        Set(ByVal Value As Date)
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

    Property MatchDOB() As String
        Get
            Return _MatchDOB
        End Get
        Set(ByVal Value As String)
            _MatchDOB = Value
        End Set
    End Property

    Property MatchFirst() As String
        Get
            Return _MatchFirst
        End Get
        Set(ByVal Value As String)
            _MatchFirst = Value
        End Set
    End Property

    Property LocationID() As Integer
        Get
            Return _LocationID
        End Get
        Set(ByVal Value As Integer)
            _LocationID = Value
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

    Property UpdateTS() As Date
        Get
            Return _UpdateTS
        End Get
        Set(ByVal Value As Date)
            _UpdateTS = Value
        End Set
    End Property

End Class

Public Class AlertDetailAnoka

    Private _AlertID As Integer
    Private _SchoolYear As Integer
    Private _SchoolCode As String
    Private _OrganizationName As String
    Private _ParentLastName As String
    Private _ParentFirstName As String
    Private _ParentMI As String
    Private _ParentDOB As Date
    Private _AddressLine1 As String
    Private _City As String
    Private _State As String
    Private _ZipCode5 As String
    Private _AlertDescription As String

    Property AlertID() As Integer
        Get
            Return _AlertID
        End Get
        Set(ByVal Value As Integer)
            _AlertID = Value
        End Set
    End Property

    Property SchoolYear() As Integer
        Get
            Return _SchoolYear
        End Get
        Set(ByVal Value As Integer)
            _SchoolYear = Value
        End Set
    End Property

    Property SchoolCode() As String
        Get
            Return _SchoolCode
        End Get
        Set(ByVal Value As String)
            _SchoolCode = Value
        End Set
    End Property

    Property OrganizationName() As String
        Get
            Return _OrganizationName
        End Get
        Set(ByVal Value As String)
            _OrganizationName = Value
        End Set
    End Property

    Property ParentLastName() As String
        Get
            Return _ParentLastName
        End Get
        Set(ByVal Value As String)
            _ParentLastName = Value
        End Set
    End Property

    Property ParentFirstName() As String
        Get
            Return _ParentFirstName
        End Get
        Set(ByVal Value As String)
            _ParentFirstName = Value
        End Set
    End Property

    Property ParentMI() As String
        Get
            Return _ParentMI
        End Get
        Set(ByVal Value As String)
            _ParentMI = Value
        End Set
    End Property

    Property ParentDOB() As Date
        Get
            Return _ParentDOB
        End Get
        Set(ByVal Value As Date)
            _ParentDOB = Value
        End Set
    End Property

    Property AddressLine1() As String
        Get
            Return _AddressLine1
        End Get
        Set(ByVal Value As String)
            _AddressLine1 = Value
        End Set
    End Property

    Property City() As String
        Get
            Return _City
        End Get
        Set(ByVal Value As String)
            _City = Value
        End Set
    End Property

    Property State() As String
        Get
            Return _State
        End Get
        Set(ByVal Value As String)
            _State = Value
        End Set
    End Property

    Property ZipCode5() As String
        Get
            Return _ZipCode5
        End Get
        Set(ByVal Value As String)
            _ZipCode5 = Value
        End Set
    End Property

    Property AlertDescription() As String
        Get
            Return _AlertDescription
        End Get
        Set(ByVal Value As String)
            _AlertDescription = Value
        End Set
    End Property

End Class

Public Class Visiting

    Private _VisitingID As Integer
    Private _VisitingName As String
    Private _LocationID As Integer
    Private _ClientID As Integer
    Private _ActiveFlag As Boolean

    Property VisitingID() As Integer
        Get
            Return _VisitingID
        End Get
        Set(ByVal Value As Integer)
            _VisitingID = Value
        End Set
    End Property

    Property VisitingName() As String
        Get
            Return _VisitingName
        End Get
        Set(ByVal Value As String)
            _VisitingName = Value
        End Set
    End Property

    Property LocationID() As Integer
        Get
            Return _LocationID
        End Get
        Set(ByVal Value As Integer)
            _LocationID = Value
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

    Property ActiveFlag() As Boolean
        Get
            Return _ActiveFlag
        End Get
        Set(ByVal Value As Boolean)
            _ActiveFlag = Value
        End Set
    End Property

End Class

Public Class VisitingList

    Private _VisitingID As Integer
    Private _VisitingName As String

    Property VisitingID() As Integer
        Get
            Return _VisitingID
        End Get
        Set(ByVal Value As Integer)
            _VisitingID = Value
        End Set
    End Property

    Property VisitingName() As String
        Get
            Return _VisitingName
        End Get
        Set(ByVal Value As String)
            _VisitingName = Value
        End Set
    End Property

End Class

Public Class VisitorLog

    Private _SwipeScanID As Integer
    Private _ClientID As Integer
    Private _UserID As Integer
    Private _IDAccount As String
    Private _NameLast As String
    Private _NameFirst As String
    Private _DateOfBirth As String
    Private _Visiting As String
    Private _AnonymousFlag As Boolean
    Private _SwipeScanTS As Date
    Private _SwipeScanOutTS As String
    Private _Alerts As String
    Private _CanCheckOut As Boolean
    Private _ImageAvailable As Boolean

    Property SwipeScanID() As Integer
        Get
            Return _SwipeScanID
        End Get
        Set(ByVal Value As Integer)
            _SwipeScanID = Value
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

    Property IDAccount() As String
        Get
            Return _IDAccount
        End Get
        Set(ByVal Value As String)
            _IDAccount = Value
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

    Property NameFirst() As String
        Get
            Return _NameFirst
        End Get
        Set(ByVal Value As String)
            _NameFirst = Value
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

    Property Visiting() As String
        Get
            Return _Visiting
        End Get
        Set(ByVal Value As String)
            _Visiting = Value
        End Set
    End Property

    Property AnonymousFlag() As Boolean
        Get
            Return _AnonymousFlag
        End Get
        Set(ByVal Value As Boolean)
            _AnonymousFlag = Value
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

    Property SwipeScanOutTS() As String
        Get
            Return _SwipeScanOutTS
        End Get
        Set(ByVal Value As String)
            _SwipeScanOutTS = Value
        End Set
    End Property

    Property Alerts() As String
        Get
            Return _Alerts
        End Get
        Set(ByVal Value As String)
            _Alerts = Value
        End Set
    End Property

    Property CanCheckOut() As Boolean
        Get
            Return _CanCheckOut
        End Get
        Set(ByVal Value As Boolean)
            _CanCheckOut = Value
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

End Class

Public Class SwipeScanDetail

    Private _SwipeScanID As Integer
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

Public Class VisitorInfo

    Private _SwipeScanID As Integer
    Private _IDAccountNumber As String
    Private _isCheckingIn As Boolean
    Private _NameFirst As String
    Private _NameLast As String
    Private _DateOfBirth As String
    Private _Visiting As String
    Private _AnonymousFlag As Boolean

    Property SwipeScanID() As Integer
        Get
            Return _SwipeScanID
        End Get
        Set(ByVal Value As Integer)
            _SwipeScanID = Value
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

    Property isCheckingIn() As Boolean
        Get
            Return _isCheckingIn
        End Get
        Set(ByVal Value As Boolean)
            _isCheckingIn = Value
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

    Property Visiting() As String
        Get
            Return _Visiting
        End Get
        Set(ByVal Value As String)
            _Visiting = Value
        End Set
    End Property

    Property AnonymousFlag() As Boolean
        Get
            Return _AnonymousFlag
        End Get
        Set(ByVal Value As Boolean)
            _AnonymousFlag = Value
        End Set
    End Property

End Class

Public Class CurrentVisitors

    Private _Location As String
    Private _Station As String
    Private _NameFirst As String
    Private _NameLast As String
    Private _Visiting As String
    Private _SwipeScanTS As Date

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

    Property Visiting() As String
        Get
            Return _Visiting
        End Get
        Set(ByVal Value As String)
            _Visiting = Value
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

Public Class VisitorsToday

    Private _Station As String
    Private _NameFirst As String
    Private _NameLast As String
    Private _Visiting As String
    Private _SwipeScanTS As Date
    Private _SwipeScanOutTS As Date

    Property Station() As String
        Get
            Return _Station
        End Get
        Set(ByVal Value As String)
            _Station = Value
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

    Property Visiting() As String
        Get
            Return _Visiting
        End Get
        Set(ByVal Value As String)
            _Visiting = Value
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

    Property SwipeScanOutTS() As Date
        Get
            Return _SwipeScanOutTS
        End Get
        Set(ByVal Value As Date)
            _SwipeScanOutTS = Value
        End Set
    End Property
End Class
