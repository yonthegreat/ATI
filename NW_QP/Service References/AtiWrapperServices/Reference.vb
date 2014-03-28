﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.18444
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System
Imports System.Runtime.Serialization

Namespace AtiWrapperServices
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0"),  _
     System.Runtime.Serialization.DataContractAttribute(Name:="WrapperResult", [Namespace]:="http://schemas.datacontract.org/2004/07/AtiWrapperServices"),  _
     System.SerializableAttribute(),  _
     System.Runtime.Serialization.KnownTypeAttribute(GetType(AtiWrapperServices.ServiceMode)),  _
     System.Runtime.Serialization.KnownTypeAttribute(GetType(AtiWrapperServices.WrapperResult.ATIWrapperServiceStatusEnum))>  _
    Partial Public Class WrapperResult
        Inherits Object
        Implements System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
        
        <System.NonSerializedAttribute()>  _
        Private extensionDataField As System.Runtime.Serialization.ExtensionDataObject
        
        <System.Runtime.Serialization.OptionalFieldAttribute()>  _
        Private FailureReasonField As Object
        
        <System.Runtime.Serialization.OptionalFieldAttribute()>  _
        Private ResultField As Object
        
        <System.Runtime.Serialization.OptionalFieldAttribute()>  _
        Private ResultStatusField As AtiWrapperServices.WrapperResult.ATIWrapperServiceStatusEnum
        
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
        Public Property FailureReason() As Object
            Get
                Return Me.FailureReasonField
            End Get
            Set
                If (Object.ReferenceEquals(Me.FailureReasonField, value) <> true) Then
                    Me.FailureReasonField = value
                    Me.RaisePropertyChanged("FailureReason")
                End If
            End Set
        End Property
        
        <System.Runtime.Serialization.DataMemberAttribute()>  _
        Public Property Result() As Object
            Get
                Return Me.ResultField
            End Get
            Set
                If (Object.ReferenceEquals(Me.ResultField, value) <> true) Then
                    Me.ResultField = value
                    Me.RaisePropertyChanged("Result")
                End If
            End Set
        End Property
        
        <System.Runtime.Serialization.DataMemberAttribute()>  _
        Public Property ResultStatus() As AtiWrapperServices.WrapperResult.ATIWrapperServiceStatusEnum
            Get
                Return Me.ResultStatusField
            End Get
            Set
                If (Me.ResultStatusField.Equals(value) <> true) Then
                    Me.ResultStatusField = value
                    Me.RaisePropertyChanged("ResultStatus")
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
        
        <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0"),  _
         System.Runtime.Serialization.DataContractAttribute(Name:="WrapperResult.ATIWrapperServiceStatusEnum", [Namespace]:="http://schemas.datacontract.org/2004/07/AtiWrapperServices")>  _
        Public Enum ATIWrapperServiceStatusEnum As Integer
            
            <System.Runtime.Serialization.EnumMemberAttribute()>  _
            Success = 0
            
            <System.Runtime.Serialization.EnumMemberAttribute()>  _
            MissingPararmeter = 1
            
            <System.Runtime.Serialization.EnumMemberAttribute()>  _
            WrapperNotFound = 2
            
            <System.Runtime.Serialization.EnumMemberAttribute()>  _
            Other = 3
        End Enum
    End Class
    
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0"),  _
     System.Runtime.Serialization.DataContractAttribute(Name:="ServiceMode", [Namespace]:="http://schemas.datacontract.org/2004/07/AtiWrapperServices.AtiWrapperServicesEnum"& _ 
        "s")>  _
    Public Enum ServiceMode As Integer
        
        <System.Runtime.Serialization.EnumMemberAttribute()>  _
        Production = 0
        
        <System.Runtime.Serialization.EnumMemberAttribute()>  _
        Test = 1
        
        <System.Runtime.Serialization.EnumMemberAttribute()>  _
        Development = 2
    End Enum
    
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ServiceModel.ServiceContractAttribute(ConfigurationName:="AtiWrapperServices.IAtiWrapperServices")>  _
    Public Interface IAtiWrapperServices
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IAtiWrapperServices/WrapperServiceByCustomerName", ReplyAction:="http://tempuri.org/IAtiWrapperServices/WrapperServiceByCustomerNameResponse")>  _
        Function WrapperServiceByCustomerName(ByVal customerName As String, ByVal serviceName As String, ByVal serviceParameters As String) As AtiWrapperServices.WrapperResult
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IAtiWrapperServices/WrapperServiceByCustomerNumber", ReplyAction:="http://tempuri.org/IAtiWrapperServices/WrapperServiceByCustomerNumberResponse")>  _
        Function WrapperServiceByCustomerNumber(ByVal customerNumber As Integer, ByVal serviceName As String, ByVal serviceParameters As String) As AtiWrapperServices.WrapperResult
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IAtiWrapperServices/WrapperServiceByCustomerNameAndServiceMode"& _ 
            "", ReplyAction:="http://tempuri.org/IAtiWrapperServices/WrapperServiceByCustomerNameAndServiceMode"& _ 
            "Response")>  _
        Function WrapperServiceByCustomerNameAndServiceMode(ByVal customerName As String, ByVal serviceMode As AtiWrapperServices.ServiceMode, ByVal serviceName As String, ByVal serviceParameters As String) As AtiWrapperServices.WrapperResult
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IAtiWrapperServices/AddNewServiceProxy", ReplyAction:="http://tempuri.org/IAtiWrapperServices/AddNewServiceProxyResponse")>  _
        Sub AddNewServiceProxy(ByVal customerId As Integer, ByVal serviceMethodName As String)
    End Interface
    
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")>  _
    Public Interface IAtiWrapperServicesChannel
        Inherits AtiWrapperServices.IAtiWrapperServices, System.ServiceModel.IClientChannel
    End Interface
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")>  _
    Partial Public Class AtiWrapperServicesClient
        Inherits System.ServiceModel.ClientBase(Of AtiWrapperServices.IAtiWrapperServices)
        Implements AtiWrapperServices.IAtiWrapperServices
        
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
        
        Public Function WrapperServiceByCustomerName(ByVal customerName As String, ByVal serviceName As String, ByVal serviceParameters As String) As AtiWrapperServices.WrapperResult Implements AtiWrapperServices.IAtiWrapperServices.WrapperServiceByCustomerName
            Return MyBase.Channel.WrapperServiceByCustomerName(customerName, serviceName, serviceParameters)
        End Function
        
        Public Function WrapperServiceByCustomerNumber(ByVal customerNumber As Integer, ByVal serviceName As String, ByVal serviceParameters As String) As AtiWrapperServices.WrapperResult Implements AtiWrapperServices.IAtiWrapperServices.WrapperServiceByCustomerNumber
            Return MyBase.Channel.WrapperServiceByCustomerNumber(customerNumber, serviceName, serviceParameters)
        End Function
        
        Public Function WrapperServiceByCustomerNameAndServiceMode(ByVal customerName As String, ByVal serviceMode As AtiWrapperServices.ServiceMode, ByVal serviceName As String, ByVal serviceParameters As String) As AtiWrapperServices.WrapperResult Implements AtiWrapperServices.IAtiWrapperServices.WrapperServiceByCustomerNameAndServiceMode
            Return MyBase.Channel.WrapperServiceByCustomerNameAndServiceMode(customerName, serviceMode, serviceName, serviceParameters)
        End Function
        
        Public Sub AddNewServiceProxy(ByVal customerId As Integer, ByVal serviceMethodName As String) Implements AtiWrapperServices.IAtiWrapperServices.AddNewServiceProxy
            MyBase.Channel.AddNewServiceProxy(customerId, serviceMethodName)
        End Sub
    End Class
End Namespace
