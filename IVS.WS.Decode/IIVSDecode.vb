Imports IVS.Data.Decode

<ServiceContract()>
Public Interface IIVSDecode

    <OperationContract()>
    Function DecodeData(ByVal LicenseGuid As Guid, ByVal RawData As String) As DecodedData

    <OperationContract()>
    Function KeepAlive() As DateTime

End Interface
