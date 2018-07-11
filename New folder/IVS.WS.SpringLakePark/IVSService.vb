' NOTE: You can use the "Rename" command on the context menu to change the class name "Service1" in both code and config file together.
Public Class IVSService
    Implements IIVS

    Public Function GetData(ByVal value As Integer) As String Implements IIVS.GetData
        Return String.Format("You entered: {0}", value)
    End Function

    Public Function GetDataUsingDataContract(ByVal composite As CompositeType) As CompositeType Implements IIVS.GetDataUsingDataContract
        If composite Is Nothing Then
            Throw New ArgumentNullException("composite")
        End If
        If composite.BoolValue Then
            composite.StringValue &= "Suffix"
        End If
        Return composite
    End Function

End Class
