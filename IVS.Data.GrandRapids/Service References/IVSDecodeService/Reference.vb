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

Imports System
Imports System.Runtime.Serialization

Namespace IVSDecodeService
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0"),  _
     System.Runtime.Serialization.DataContractAttribute(Name:="DecodedData", [Namespace]:="http://schemas.datacontract.org/2004/07/IVS.Data.Decode"),  _
     System.SerializableAttribute()>  _
    Partial Public Class DecodedData
        Inherits Object
        Implements System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
        
        <System.NonSerializedAttribute()>  _
        Private extensionDataField As System.Runtime.Serialization.ExtensionDataObject
        
        <System.Runtime.Serialization.OptionalFieldAttribute()>  _
        Private AddressCityField As String
        
        <System.Runtime.Serialization.OptionalFieldAttribute()>  _
        Private AddressStateField As String
        
        <System.Runtime.Serialization.OptionalFieldAttribute()>  _
        Private AddressStreetField As String
        
        <System.Runtime.Serialization.OptionalFieldAttribute()>  _
        Private AddressZipField As String
        
        <System.Runtime.Serialization.OptionalFieldAttribute()>  _
        Private AgeField As Integer
        
        <System.Runtime.Serialization.OptionalFieldAttribute()>  _
        Private CCIssuerField As String
        
        <System.Runtime.Serialization.OptionalFieldAttribute()>  _
        Private CardTypeField As String
        
        <System.Runtime.Serialization.OptionalFieldAttribute()>  _
        Private DateOfBirthField As String
        
        <System.Runtime.Serialization.OptionalFieldAttribute()>  _
        Private DateOfExpirationField As String
        
        <System.Runtime.Serialization.OptionalFieldAttribute()>  _
        Private DateOfIssueField As String
        
        <System.Runtime.Serialization.OptionalFieldAttribute()>  _
        Private EyesField As String
        
        <System.Runtime.Serialization.OptionalFieldAttribute()>  _
        Private HairField As String
        
        <System.Runtime.Serialization.OptionalFieldAttribute()>  _
        Private HeightField As String
        
        <System.Runtime.Serialization.OptionalFieldAttribute()>  _
        Private IDAccountNumberField As String
        
        <System.Runtime.Serialization.OptionalFieldAttribute()>  _
        Private NameFirstField As String
        
        <System.Runtime.Serialization.OptionalFieldAttribute()>  _
        Private NameLastField As String
        
        <System.Runtime.Serialization.OptionalFieldAttribute()>  _
        Private NameMiddleField As String
        
        <System.Runtime.Serialization.OptionalFieldAttribute()>  _
        Private SexField As String
        
        <System.Runtime.Serialization.OptionalFieldAttribute()>  _
        Private WeightField As String
        
        <Global.System.ComponentModel.BrowsableAttribute(false)>  _
        Public Property ExtensionData() As System.Runtime.Serialization.ExtensionDataObject Implements System.Runtime.Serialization.IExtensibleDataObject.ExtensionData
            Get
                Return Me.extensionDataField
            End Get
            Set
                Me.extensionDataField = value
            End Set
        End Property
        
        <System.Runtime.Serialization.DataMemberAttribute()>  _
        Public Property AddressCity() As String
            Get
                Return Me.AddressCityField
            End Get
            Set
                If (Object.ReferenceEquals(Me.AddressCityField, value) <> true) Then
                    Me.AddressCityField = value
                    Me.RaisePropertyChanged("AddressCity")
                End If
            End Set
        End Property
        
        <System.Runtime.Serialization.DataMemberAttribute()>  _
        Public Property AddressState() As String
            Get
                Return Me.AddressStateField
            End Get
            Set
                If (Object.ReferenceEquals(Me.AddressStateField, value) <> true) Then
                    Me.AddressStateField = value
                    Me.RaisePropertyChanged("AddressState")
                End If
            End Set
        End Property
        
        <System.Runtime.Serialization.DataMemberAttribute()>  _
        Public Property AddressStreet() As String
            Get
                Return Me.AddressStreetField
            End Get
            Set
                If (Object.ReferenceEquals(Me.AddressStreetField, value) <> true) Then
                    Me.AddressStreetField = value
                    Me.RaisePropertyChanged("AddressStreet")
                End If
            End Set
        End Property
        
        <System.Runtime.Serialization.DataMemberAttribute()>  _
        Public Property AddressZip() As String
            Get
                Return Me.AddressZipField
            End Get
            Set
                If (Object.ReferenceEquals(Me.AddressZipField, value) <> true) Then
                    Me.AddressZipField = value
                    Me.RaisePropertyChanged("AddressZip")
                End If
            End Set
        End Property
        
        <System.Runtime.Serialization.DataMemberAttribute()>  _
        Public Property Age() As Integer
            Get
                Return Me.AgeField
            End Get
            Set
                If (Me.AgeField.Equals(value) <> true) Then
                    Me.AgeField = value
                    Me.RaisePropertyChanged("Age")
                End If
            End Set
        End Property
        
        <System.Runtime.Serialization.DataMemberAttribute()>  _
        Public Property CCIssuer() As String
            Get
                Return Me.CCIssuerField
            End Get
            Set
                If (Object.ReferenceEquals(Me.CCIssuerField, value) <> true) Then
                    Me.CCIssuerField = value
                    Me.RaisePropertyChanged("CCIssuer")
                End If
            End Set
        End Property
        
        <System.Runtime.Serialization.DataMemberAttribute()>  _
        Public Property CardType() As String
            Get
                Return Me.CardTypeField
            End Get
            Set
                If (Object.ReferenceEquals(Me.CardTypeField, value) <> true) Then
                    Me.CardTypeField = value
                    Me.RaisePropertyChanged("CardType")
                End If
            End Set
        End Property
        
        <System.Runtime.Serialization.DataMemberAttribute()>  _
        Public Property DateOfBirth() As String
            Get
                Return Me.DateOfBirthField
            End Get
            Set
                If (Object.ReferenceEquals(Me.DateOfBirthField, value) <> true) Then
                    Me.DateOfBirthField = value
                    Me.RaisePropertyChanged("DateOfBirth")
                End If
            End Set
        End Property
        
        <System.Runtime.Serialization.DataMemberAttribute()>  _
        Public Property DateOfExpiration() As String
            Get
                Return Me.DateOfExpirationField
            End Get
            Set
                If (Object.ReferenceEquals(Me.DateOfExpirationField, value) <> true) Then
                    Me.DateOfExpirationField = value
                    Me.RaisePropertyChanged("DateOfExpiration")
                End If
            End Set
        End Property
        
        <System.Runtime.Serialization.DataMemberAttribute()>  _
        Public Property DateOfIssue() As String
            Get
                Return Me.DateOfIssueField
            End Get
            Set
                If (Object.ReferenceEquals(Me.DateOfIssueField, value) <> true) Then
                    Me.DateOfIssueField = value
                    Me.RaisePropertyChanged("DateOfIssue")
                End If
            End Set
        End Property
        
        <System.Runtime.Serialization.DataMemberAttribute()>  _
        Public Property Eyes() As String
            Get
                Return Me.EyesField
            End Get
            Set
                If (Object.ReferenceEquals(Me.EyesField, value) <> true) Then
                    Me.EyesField = value
                    Me.RaisePropertyChanged("Eyes")
                End If
            End Set
        End Property
        
        <System.Runtime.Serialization.DataMemberAttribute()>  _
        Public Property Hair() As String
            Get
                Return Me.HairField
            End Get
            Set
                If (Object.ReferenceEquals(Me.HairField, value) <> true) Then
                    Me.HairField = value
                    Me.RaisePropertyChanged("Hair")
                End If
            End Set
        End Property
        
        <System.Runtime.Serialization.DataMemberAttribute()>  _
        Public Property Height() As String
            Get
                Return Me.HeightField
            End Get
            Set
                If (Object.ReferenceEquals(Me.HeightField, value) <> true) Then
                    Me.HeightField = value
                    Me.RaisePropertyChanged("Height")
                End If
            End Set
        End Property
        
        <System.Runtime.Serialization.DataMemberAttribute()>  _
        Public Property IDAccountNumber() As String
            Get
                Return Me.IDAccountNumberField
            End Get
            Set
                If (Object.ReferenceEquals(Me.IDAccountNumberField, value) <> true) Then
                    Me.IDAccountNumberField = value
                    Me.RaisePropertyChanged("IDAccountNumber")
                End If
            End Set
        End Property
        
        <System.Runtime.Serialization.DataMemberAttribute()>  _
        Public Property NameFirst() As String
            Get
                Return Me.NameFirstField
            End Get
            Set
                If (Object.ReferenceEquals(Me.NameFirstField, value) <> true) Then
                    Me.NameFirstField = value
                    Me.RaisePropertyChanged("NameFirst")
                End If
            End Set
        End Property
        
        <System.Runtime.Serialization.DataMemberAttribute()>  _
        Public Property NameLast() As String
            Get
                Return Me.NameLastField
            End Get
            Set
                If (Object.ReferenceEquals(Me.NameLastField, value) <> true) Then
                    Me.NameLastField = value
                    Me.RaisePropertyChanged("NameLast")
                End If
            End Set
        End Property
        
        <System.Runtime.Serialization.DataMemberAttribute()>  _
        Public Property NameMiddle() As String
            Get
                Return Me.NameMiddleField
            End Get
            Set
                If (Object.ReferenceEquals(Me.NameMiddleField, value) <> true) Then
                    Me.NameMiddleField = value
                    Me.RaisePropertyChanged("NameMiddle")
                End If
            End Set
        End Property
        
        <System.Runtime.Serialization.DataMemberAttribute()>  _
        Public Property Sex() As String
            Get
                Return Me.SexField
            End Get
            Set
                If (Object.ReferenceEquals(Me.SexField, value) <> true) Then
                    Me.SexField = value
                    Me.RaisePropertyChanged("Sex")
                End If
            End Set
        End Property
        
        <System.Runtime.Serialization.DataMemberAttribute()>  _
        Public Property Weight() As String
            Get
                Return Me.WeightField
            End Get
            Set
                If (Object.ReferenceEquals(Me.WeightField, value) <> true) Then
                    Me.WeightField = value
                    Me.RaisePropertyChanged("Weight")
                End If
            End Set
        End Property
        
        Public Event PropertyChanged As System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        
        Protected Sub RaisePropertyChanged(ByVal propertyName As String)
            Dim propertyChanged As System.ComponentModel.PropertyChangedEventHandler = Me.PropertyChangedEvent
            If (Not (propertyChanged) Is Nothing) Then
                propertyChanged(Me, New System.ComponentModel.PropertyChangedEventArgs(propertyName))
            End If
        End Sub
    End Class
    
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ServiceModel.ServiceContractAttribute(ConfigurationName:="IVSDecodeService.IIVSDecode")>  _
    Public Interface IIVSDecode
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IIVSDecode/DecodeData", ReplyAction:="http://tempuri.org/IIVSDecode/DecodeDataResponse")>  _
        Function DecodeData(ByVal LicenseGuid As System.Guid, ByVal RawData As String) As IVSDecodeService.DecodedData
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
        
        Public Function DecodeData(ByVal LicenseGuid As System.Guid, ByVal RawData As String) As IVSDecodeService.DecodedData Implements IVSDecodeService.IIVSDecode.DecodeData
            Return MyBase.Channel.DecodeData(LicenseGuid, RawData)
        End Function
    End Class
End Namespace
