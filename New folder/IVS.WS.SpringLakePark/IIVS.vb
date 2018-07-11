' NOTE: You can use the "Rename" command on the context menu to change the interface name "IService1" in both code and config file together.
<ServiceContract()> _
Public Interface IIVS

    <OperationContract()> _
    Function GetData(ByVal value As Integer) As String

    <OperationContract()> _
    Function GetDataUsingDataContract(ByVal composite As CompositeType) As CompositeType

    ' TODO: Add your service operations here

End Interface

' Use a data contract as illustrated in the sample below to add composite types to service operations

<DataContract()> _
Public Class CompositeType

    Private boolValueField As Boolean
    Private stringValueField As String

    <DataMember()> _
    Public Property BoolValue() As Boolean
        Get
            Return Me.boolValueField
        End Get
        Set(ByVal value As Boolean)
            Me.boolValueField = value
        End Set
    End Property

    <DataMember()> _
    Public Property StringValue() As String
        Get
            Return Me.stringValueField
        End Get
        Set(ByVal value As String)
            Me.stringValueField = value
        End Set
    End Property

End Class
