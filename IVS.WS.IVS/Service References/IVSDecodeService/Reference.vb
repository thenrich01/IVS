﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.296
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Namespace IVSDecodeService
    
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ServiceModel.ServiceContractAttribute(ConfigurationName:="IVSDecodeService.IIVSDecode")>  _
    Public Interface IIVSDecode
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IIVSDecode/DecodeData", ReplyAction:="http://tempuri.org/IIVSDecode/DecodeDataResponse")> _
        Function DecodeData(ByVal LicenseGuid As System.Guid, ByVal RawData As String) As Data.IVS.IVSDecodeService.DecodedData
    End Interface
    
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")>  _
    Public Interface IIVSDecodeChannel
        Inherits IVSDecodeService.IIVSDecode, System.ServiceModel.IClientChannel
    End Interface
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")>  _
    Partial Public Class IVSDecodeClient
        Inherits System.ServiceModel.ClientBase(Of IVSDecodeService.IIVSDecode)
        Implements IVSDecodeService.IIVSDecode
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal endpointConfigurationName As String)
            MyBase.New(endpointConfigurationName)
        End Sub
        
        Public Sub New(ByVal endpointConfigurationName As String, ByVal remoteAddress As String)
            MyBase.New(endpointConfigurationName, remoteAddress)
        End Sub
        
        Public Sub New(ByVal endpointConfigurationName As String, ByVal remoteAddress As System.ServiceModel.EndpointAddress)
            MyBase.New(endpointConfigurationName, remoteAddress)
        End Sub
        
        Public Sub New(ByVal binding As System.ServiceModel.Channels.Binding, ByVal remoteAddress As System.ServiceModel.EndpointAddress)
            MyBase.New(binding, remoteAddress)
        End Sub
        
        Public Function DecodeData(ByVal LicenseGuid As System.Guid, ByVal RawData As String) As Data.IVS.IVSDecodeService.DecodedData Implements IVSDecodeService.IIVSDecode.DecodeData
            Return MyBase.Channel.DecodeData(LicenseGuid, RawData)
        End Function
    End Class
End Namespace