﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WrapperStub.WrapperService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="WrapperResult", Namespace="http://schemas.datacontract.org/2004/07/AtiWrapperServices")]
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(WrapperStub.WrapperService.ServiceMode))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(WrapperStub.WrapperService.WrapperResult.ATIWrapperServiceStatusEnum))]
    public partial class WrapperResult : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private object FailureReasonField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private object ResultField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private WrapperStub.WrapperService.WrapperResult.ATIWrapperServiceStatusEnum ResultStatusField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public object FailureReason {
            get {
                return this.FailureReasonField;
            }
            set {
                if ((object.ReferenceEquals(this.FailureReasonField, value) != true)) {
                    this.FailureReasonField = value;
                    this.RaisePropertyChanged("FailureReason");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public object Result {
            get {
                return this.ResultField;
            }
            set {
                if ((object.ReferenceEquals(this.ResultField, value) != true)) {
                    this.ResultField = value;
                    this.RaisePropertyChanged("Result");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public WrapperStub.WrapperService.WrapperResult.ATIWrapperServiceStatusEnum ResultStatus {
            get {
                return this.ResultStatusField;
            }
            set {
                if ((this.ResultStatusField.Equals(value) != true)) {
                    this.ResultStatusField = value;
                    this.RaisePropertyChanged("ResultStatus");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
        [System.Runtime.Serialization.DataContractAttribute(Name="WrapperResult.ATIWrapperServiceStatusEnum", Namespace="http://schemas.datacontract.org/2004/07/AtiWrapperServices")]
        public enum ATIWrapperServiceStatusEnum : int {
            
            [System.Runtime.Serialization.EnumMemberAttribute()]
            Success = 0,
            
            [System.Runtime.Serialization.EnumMemberAttribute()]
            MissingPararmeter = 1,
            
            [System.Runtime.Serialization.EnumMemberAttribute()]
            WrapperNotFound = 2,
            
            [System.Runtime.Serialization.EnumMemberAttribute()]
            Other = 3,
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ServiceMode", Namespace="http://schemas.datacontract.org/2004/07/AtiWrapperServices.AtiWrapperServicesEnum" +
        "s")]
    public enum ServiceMode : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Production = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Test = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Development = 2,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="WrapperService.IAtiWrapperServices")]
    public interface IAtiWrapperServices {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAtiWrapperServices/WrapperServiceByCustomerName", ReplyAction="http://tempuri.org/IAtiWrapperServices/WrapperServiceByCustomerNameResponse")]
        WrapperStub.WrapperService.WrapperResult WrapperServiceByCustomerName(string customerName, string serviceName, string serviceParameters);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAtiWrapperServices/WrapperServiceByCustomerName", ReplyAction="http://tempuri.org/IAtiWrapperServices/WrapperServiceByCustomerNameResponse")]
        System.Threading.Tasks.Task<WrapperStub.WrapperService.WrapperResult> WrapperServiceByCustomerNameAsync(string customerName, string serviceName, string serviceParameters);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAtiWrapperServices/WrapperServiceByCustomerNumber", ReplyAction="http://tempuri.org/IAtiWrapperServices/WrapperServiceByCustomerNumberResponse")]
        WrapperStub.WrapperService.WrapperResult WrapperServiceByCustomerNumber(int customerNumber, string serviceName, string serviceParameters);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAtiWrapperServices/WrapperServiceByCustomerNumber", ReplyAction="http://tempuri.org/IAtiWrapperServices/WrapperServiceByCustomerNumberResponse")]
        System.Threading.Tasks.Task<WrapperStub.WrapperService.WrapperResult> WrapperServiceByCustomerNumberAsync(int customerNumber, string serviceName, string serviceParameters);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAtiWrapperServices/WrapperServiceByCustomerNameAndServiceMode" +
            "", ReplyAction="http://tempuri.org/IAtiWrapperServices/WrapperServiceByCustomerNameAndServiceMode" +
            "Response")]
        WrapperStub.WrapperService.WrapperResult WrapperServiceByCustomerNameAndServiceMode(string CustomerName, WrapperStub.WrapperService.ServiceMode serviceMode, string serviceName, string serviceParameters);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAtiWrapperServices/WrapperServiceByCustomerNameAndServiceMode" +
            "", ReplyAction="http://tempuri.org/IAtiWrapperServices/WrapperServiceByCustomerNameAndServiceMode" +
            "Response")]
        System.Threading.Tasks.Task<WrapperStub.WrapperService.WrapperResult> WrapperServiceByCustomerNameAndServiceModeAsync(string CustomerName, WrapperStub.WrapperService.ServiceMode serviceMode, string serviceName, string serviceParameters);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAtiWrapperServices/AddNewServiceProxy", ReplyAction="http://tempuri.org/IAtiWrapperServices/AddNewServiceProxyResponse")]
        void AddNewServiceProxy(int customerId, string serviceMethodName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAtiWrapperServices/AddNewServiceProxy", ReplyAction="http://tempuri.org/IAtiWrapperServices/AddNewServiceProxyResponse")]
        System.Threading.Tasks.Task AddNewServiceProxyAsync(int customerId, string serviceMethodName);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IAtiWrapperServicesChannel : WrapperStub.WrapperService.IAtiWrapperServices, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AtiWrapperServicesClient : System.ServiceModel.ClientBase<WrapperStub.WrapperService.IAtiWrapperServices>, WrapperStub.WrapperService.IAtiWrapperServices {
        
        public AtiWrapperServicesClient() {
        }
        
        public AtiWrapperServicesClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public AtiWrapperServicesClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AtiWrapperServicesClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AtiWrapperServicesClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public WrapperStub.WrapperService.WrapperResult WrapperServiceByCustomerName(string customerName, string serviceName, string serviceParameters) {
            return base.Channel.WrapperServiceByCustomerName(customerName, serviceName, serviceParameters);
        }
        
        public System.Threading.Tasks.Task<WrapperStub.WrapperService.WrapperResult> WrapperServiceByCustomerNameAsync(string customerName, string serviceName, string serviceParameters) {
            return base.Channel.WrapperServiceByCustomerNameAsync(customerName, serviceName, serviceParameters);
        }
        
        public WrapperStub.WrapperService.WrapperResult WrapperServiceByCustomerNumber(int customerNumber, string serviceName, string serviceParameters) {
            return base.Channel.WrapperServiceByCustomerNumber(customerNumber, serviceName, serviceParameters);
        }
        
        public System.Threading.Tasks.Task<WrapperStub.WrapperService.WrapperResult> WrapperServiceByCustomerNumberAsync(int customerNumber, string serviceName, string serviceParameters) {
            return base.Channel.WrapperServiceByCustomerNumberAsync(customerNumber, serviceName, serviceParameters);
        }
        
        public WrapperStub.WrapperService.WrapperResult WrapperServiceByCustomerNameAndServiceMode(string CustomerName, WrapperStub.WrapperService.ServiceMode serviceMode, string serviceName, string serviceParameters) {
            return base.Channel.WrapperServiceByCustomerNameAndServiceMode(CustomerName, serviceMode, serviceName, serviceParameters);
        }
        
        public System.Threading.Tasks.Task<WrapperStub.WrapperService.WrapperResult> WrapperServiceByCustomerNameAndServiceModeAsync(string CustomerName, WrapperStub.WrapperService.ServiceMode serviceMode, string serviceName, string serviceParameters) {
            return base.Channel.WrapperServiceByCustomerNameAndServiceModeAsync(CustomerName, serviceMode, serviceName, serviceParameters);
        }
        
        public void AddNewServiceProxy(int customerId, string serviceMethodName) {
            base.Channel.AddNewServiceProxy(customerId, serviceMethodName);
        }
        
        public System.Threading.Tasks.Task AddNewServiceProxyAsync(int customerId, string serviceMethodName) {
            return base.Channel.AddNewServiceProxyAsync(customerId, serviceMethodName);
        }
    }
}
