Imports System.Data.SqlClient
Imports System.Configuration

Public Class DataAccess

    Public Shared MyAppLog As New IVS.AppLog.ApplicationLog(IVS.AppLog.ApplicationLog.DateEncodingType.YYYYMMDD, True, "IVS.WS.Decode", DateFormat.GeneralDate)

    Public Shared Function OpenConnection() As SqlConnection

        Dim cnnString As String
        Dim cnn As SQLConnection

        Try
            cnnString = ConfigurationManager.ConnectionStrings("IVS.WS.Decode.My.MySettings.ConnectionString").ConnectionString
            cnn = New SqlConnection(cnnString)

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS.Data.Decode.OpenConnection()" & ex.ToString)
        End Try

        Return cnn

    End Function

    Public Shared Function GetWebServiceSettings() As WebServiceSettings

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader
        Dim objWebServiceSettings As WebServiceSettings

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryGetWebServiceSettings"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                objWebServiceSettings = New WebServiceSettings
                objWebServiceSettings.IDecodeLicense = dr(0).ToString
                objWebServiceSettings.IDecodeTrackFormat = dr(1).ToString
                objWebServiceSettings.IDecodeCardTypes = dr(2).ToString

            End While

            dr.Close()
            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog("IVS.Data.Decode.GetWebServiceSettings()" & ex.ToString)

        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return objWebServiceSettings

    End Function

    Public Shared Sub UpdateWSIDecode(ByVal Version As String)

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand

        Try

            cnn = OpenConnection()

            strSQLCommand = "qryUpdateWSIDecode"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("IDecodeVersion", Version)

            cnn.Open()

            cmd.ExecuteNonQuery()

            cnn.Close()

        Catch ex As Exception

            MyAppLog.WriteToLog("IVS.Data.Decode.UpdateWSIDecode()" & ex.ToString)

        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

    End Sub

    Public Shared Function IsIVSLicenseValid(ByVal LicenseGuid As Guid, ByVal IPAddress As String) As Boolean

        Dim IsLicenseValid As Boolean
        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader

        Try
            MyAppLog.WriteToLog("IVS.Data.Decode.IsIVSLicenseValid()" & LicenseGuid.ToString)
            cnn = OpenConnection()

            strSQLCommand = "qryIsIVSLicenseValid"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("LicenseGuid", LicenseGuid.ToString)

            cnn.Open()

            dr = cmd.ExecuteReader

            While dr.Read

                IsLicenseValid = dr(0).ToString

            End While

            dr.Close()

            strSQLCommand = "qryWebServiceActivity"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("IPAddress", IPAddress)
            cmd.Parameters.AddWithValue("IVSFunction", "IsIVSLicenseValid")
            cmd.Parameters.AddWithValue("ActivityDetails", IsLicenseValid & " - LicenseGuid: " & LicenseGuid.ToString)

            cmd.ExecuteNonQuery()

            cnn.Close()

        Catch ex As Exception

            MyAppLog.WriteToLog("IVS.Data.Decode.IsIVSLicenseValid()" & ex.ToString)

        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

        Return IsLicenseValid

    End Function

    Public Shared Sub LicenseException(ByVal LicenseGuid As Guid, IPAddress As String, IVSService As String, ByVal IVSFunction As String)

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryLicenseException"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("LicenseGUID", LicenseGuid)
            cmd.Parameters.AddWithValue("IPAddress", IPAddress)
            cmd.Parameters.AddWithValue("IVSService", IVSService)
            cmd.Parameters.AddWithValue("IVSFunction", IVSFunction)

            cnn.Open()

            cmd.ExecuteNonQuery()

            cnn.Close()

        Catch ex As Exception

            MyAppLog.WriteToLog("IVS.Data.Decode.LicenseException()" & ex.ToString)

        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If

        End Try

    End Sub

    Public Shared Sub WebServiceActivity(ByVal IPAddress As String, ByVal IVSFunction As String, ByVal ActivityDetails As String)

        Dim cnn As New SqlConnection
        Dim strSQLCommand As String
        Dim cmd As SqlCommand

        Try
            cnn = OpenConnection()

            strSQLCommand = "qryWebServiceActivity"

            cmd = New SqlCommand(strSQLCommand, cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("IPAddress", IPAddress)
            cmd.Parameters.AddWithValue("IVSFunction", IVSFunction)
            cmd.Parameters.AddWithValue("ActivityDetails", ActivityDetails)

            cnn.Open()

            cmd.ExecuteNonQuery()

            cnn.Close()

        Catch ex As Exception
            MyAppLog.WriteToLog(ex)
        Finally

            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If

            If cnn IsNot Nothing Then
                cnn.Dispose()
            End If
        End Try
    End Sub

End Class

Public Class WebServiceSettings

    Private _IDecodeLicense As String
    Private _IDecodeTrackFormat As String
    Private _IDecodeCardTypes As String

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

End Class

Public Class DecodedData

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

End Class