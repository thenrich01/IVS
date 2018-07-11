Imports IVS.Data.GrandRapids

<ServiceContract()>
Public Interface IIVS

    <OperationContract()> _
    Function NewClient(ByVal LicenseGuid As Guid, ByVal HostName As String, ByVal IPAddress As String, ByVal ImagePath As String) As Integer

    <OperationContract()> _
    Function GetClientID(ByVal LicenseGuid As Guid, ByVal HostName As String) As String

    <OperationContract()> _
    Function GetClients(ByVal LicenseGuid As Guid) As List(Of Clients)

    <OperationContract()> _
    Function GetStations(ByVal LicenseGuid As Guid) As List(Of Locations)

    <OperationContract()> _
    Function GetLocations(ByVal LicenseGuid As Guid) As List(Of Locations)

    <OperationContract()> _
    Function GetLocation(ByVal LicenseGuid As Guid, ByVal LocationID As Integer) As Locations

    <OperationContract()> _
    Sub NewLocation(ByVal LicenseGuid As Guid, ByVal LocationDescription As String)

    <OperationContract()> _
    Sub UpdateLocation(ByVal LicenseGuid As Guid, ByVal NewLocation As Locations)

    <OperationContract()> _
    Sub DeleteLocation(ByVal LicenseGuid As Guid, ByVal LocationID As Integer)

    <OperationContract()> _
    Function UpdateClientIPAddress(ByVal LicenseGuid As Guid, ByVal ClientID As Integer, ByVal IPAddress As String) As Boolean

    <OperationContract()> _
    Function GetClientSettings(ByVal LicenseGuid As Guid, ByVal ClientID As Integer) As ClientSettings

    <OperationContract()> _
    Sub SaveClientSettings(ByVal LicenseGuid As Guid, ByVal NewClientSettings As ClientSettings)

    <OperationContract()> _
    Function GetStationName(ByVal LicenseGuid As Guid, ByVal ClientID As Integer) As String

    <OperationContract()> _
    Function GetDeviceInfo(ByVal LicenseGuid As Guid, ByVal ClientID As Integer) As DeviceInfo

    <OperationContract()> _
    Sub UpdateDevice(ByVal LicenseGuid As Guid, ByVal DeviceInfo As DeviceInfo)

    <OperationContract()> _
    Function GetUsers(ByVal LicenseGuid As Guid) As List(Of UserDetail)

    <OperationContract()> _
    Function GetUserNames(ByVal LicenseGuid As Guid) As List(Of UserDetail)

    <OperationContract()> _
    Function GetUserName(ByVal LicenseGuid As Guid, ByVal UserID As Integer) As String

    <OperationContract()> _
    Function GetUserPhone(ByVal LicenseGuid As Guid, ByVal UserID As Integer) As String

    <OperationContract()> _
    Function GetUserDetail(ByVal LicenseGuid As Guid, ByVal UserID As Integer) As UserDetail

    <OperationContract()> _
    Sub NewUser(ByVal LicenseGuid As Guid, ByVal UserDetail As UserDetail)

    <OperationContract()> _
    Sub UpdateUser(ByVal LicenseGuid As Guid, ByVal UserDetail As UserDetail)

    <OperationContract()> _
    Sub DeleteUser(ByVal LicenseGuid As Guid, ByVal UserID As Integer)

    <OperationContract()> _
    Sub EnableUser(ByVal LicenseGuid As Guid, ByVal UserID As Integer, ByVal IsActive As Boolean)

    <OperationContract()> _
    Function IsUserNameAvailable(ByVal LicenseGuid As Guid, ByVal UserName As String) As Boolean

    <OperationContract()> _
    Function IsUserAuthenticated(ByVal LicenseGuid As Guid, ByVal UserName As String, ByVal Password As String) As Integer

    <OperationContract()> _
    Function GetAlerts(ByVal LicenseGuid As Guid) As List(Of AlertDetail)

    <OperationContract()> _
    Function GetAlertDetail(ByVal LicenseGuid As Guid, ByVal AlertID As Integer) As AlertDetail

    <OperationContract()> _
    Function GetSwipeScanAlerts(ByVal LicenseGuid As Guid, ByVal IDAccountNumber As String, ByVal NameFirst As String, ByVal NameLast As String) As List(Of AlertDetail)

    <OperationContract()> _
    Sub NewAlert(ByVal LicenseGuid As Guid, ByVal AlertDetail As AlertDetail)

    <OperationContract()> _
    Sub UpdateAlert(ByVal LicenseGuid As Guid, ByVal AlertDetail As AlertDetail)

    <OperationContract()> _
    Sub DeleteAlert(ByVal LicenseGuid As Guid, ByVal AlertID As Integer)

    <OperationContract()> _
    Sub EnableAlert(ByVal LicenseGuid As Guid, ByVal AlertID As Integer, ByVal IsActive As Boolean)

    <OperationContract()> _
    Sub UpdateImageAvailable(ByVal LicenseGuid As Guid, ByVal SwipeScanID As Integer)

    <OperationContract()> _
    Function NewDataSwipeScan(ByVal LicenseGuid As Guid, ByVal SwipeScanInfo As SwipeScanInfo) As SwipeScanDetail

    <OperationContract()> _
    Function NewDataSwipeScanManual(ByVal LicenseGuid As Guid, ByVal SwipeScanDetail As SwipeScanDetail) As Integer

    <OperationContract()> _
    Function GetSwipeScanDetail(ByVal LicenseGuid As Guid, ByVal SwipeScanID As Integer, ByVal SwipeScanType As String) As SwipeScanDetail

    <OperationContract()> _
    Function GetSwipeScanHistory(ByVal LicenseGuid As Guid, ByVal IDAccountNumber As String, ByVal SwipeScanType As String) As List(Of SwipeScanHistory)

    <OperationContract()> _
    Function GetSwipeScanType(ByVal LicenseGuid As Guid, ByVal SwipeScanID As Integer) As String

    <OperationContract()> _
    Function SwipeScanNavigateFirst(ByVal LicenseGuid As Guid, ByVal UserID As Integer, ByVal ClientID As Integer) As SwipeScanNavigateInfo

    <OperationContract()> _
    Function SwipeScanNavigatePrevious(ByVal LicenseGuid As Guid, ByVal UserID As Integer, ByVal ClientID As Integer, ByVal SwipeScanID As Integer) As SwipeScanNavigateInfo

    <OperationContract()> _
    Function SwipeScanNavigatePosition(ByVal LicenseGuid As Guid, ByVal SwipeScanID As Integer) As SwipeScanNavigateInfo

    <OperationContract()> _
    Function SwipeScanNavigateNext(ByVal LicenseGuid As Guid, ByVal UserID As Integer, ByVal ClientID As Integer, ByVal SwipeScanID As Integer) As SwipeScanNavigateInfo

    <OperationContract()> _
    Function SwipeScanNavigateLast(ByVal LicenseGuid As Guid, ByVal UserID As Integer, ByVal ClientID As Integer) As SwipeScanNavigateInfo

    <OperationContract()> _
    Sub DeleteSwipeScan(ByVal LicenseGuid As Guid, ByVal SwipeScanID As Integer)

    <OperationContract()> _
    Sub UpdateCaseID(ByVal LicenseGuid As Guid, ByVal SwipeScanID As Integer, ByVal CaseID As String)

    <OperationContract()> _
    Function GetSwipeScanSearch(ByVal LicenseGuid As Guid) As List(Of SwipeScanSearch)

    <OperationContract()> _
    Function GetTEPViolations(ByVal LicenseGuid As Guid) As List(Of IVS.Data.GrandRapids.TEPViolations)

    <OperationContract()> _
    Function GetTEPClientSettings(ByVal LicenseGuid As Guid) As IVS.Data.GrandRapids.TEPClientSettings

    <OperationContract()> _
    Sub NewDataSwipeScan_TEP(ByVal LicenseGuid As Guid, ByVal TEPSwipeScanDetail As IVS.Data.GrandRapids.TEPSwipeScanDetail)

    <OperationContract()> _
    Sub NewDataSwipeScan_Violation(ByVal LicenseGuid As Guid, ByVal TEPSwipeScanViolation As IVS.Data.GrandRapids.TEPSwipeScanViolations)

    <OperationContract()> _
    Function GetSwipeScanDetail_TEP(ByVal LicenseGuid As Guid, ByVal SwipeScanID As Integer) As IVS.Data.GrandRapids.TEPSwipeScanDetail

    <OperationContract()> _
    Function GetSwipeScanDetail_Violations(ByVal LicenseGuid As Guid, ByVal SwipeScanID As Integer) As List(Of IVS.Data.GrandRapids.TEPSwipeScanViolations)

    <OperationContract()> _
    Function IsCitationUsedAlready(ByVal LicenseGuid As Guid, ByVal CitationID As String) As Boolean

End Interface
