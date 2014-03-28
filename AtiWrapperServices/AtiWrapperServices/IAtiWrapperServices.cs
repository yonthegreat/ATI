using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace AtiWrapperServices
{
    
    /// <summary>
    /// AtiWrapperServices Service contract and Interface definition
    /// </summary>
    [ServiceContract]
    public interface IAtiWrapperServices
    {
        /// <summary>
        /// Contract for calling a Service Wrapper using the Customer Name
        /// </summary>
        /// <param name="customerName">AtiWrapperServices customerName of the Wrapper Service</param>
        /// <param name="name">AtiWrapperServices Name of the Wrapper method</param>
        /// <param name="parameters">AtiWrapperServices Wrapper parameters</param>
        /// <returns></returns>
        [OperationContract]
        WrapperResult WrapperServiceByCustomerName(string customerName, string serviceName, string serviceParameters);

        /// <summary>
        /// Contract for calling a Service Wrapper using the Customer Id
        /// </summary>
        /// <param name="customerNumber">AtiWrapperServices customerNumber of the Wrapper Service</param>
        /// <param name="name">AtiWrapperServices Name of the Wrapper method</param>
        /// <param name="parameters">AtiWrapperServices Wrapper parameters</param>
        /// <returns>WrapperResult</returns>
        [OperationContract]
        WrapperResult WrapperServiceByCustomerNumber(int customerNumber, string serviceName, string serviceParameters);

        /// <summary>
        /// Contract for calling a Service Wrapper using Customer Id and specifying the Test or production modes
        /// </summary>
        /// <param name="customerName">AtiWrapperServices customerName of the Wrapper Service</param>
        /// <param name="serviceMode"></param>
        /// <param name="name">AtiWrapperServices Name of the Wrapper method</param>
        /// <param name="parameters">AtiWrapperServices Wrapper parameters</param>
        /// <returns>WrapperResult</returns>
        [OperationContract]
        WrapperResult WrapperServiceByCustomerNameAndServiceMode(string customerName, AtiWrapperServicesEnums.ServiceMode serviceMode, string serviceName, string serviceParameters);

        /// <summary>
        /// Contract for adding a new service proxy to the AtiWrapperServices while it is running
        /// </summary>
        /// <param name="customerId">AtiWrapperServices customerId of the Wrapper Service</param>
        /// <param name="serviceMethodName">AtiWrapperServices serviceMethodName of the Wrapper method</param>
        [OperationContract]
        void AddNewServiceProxy(int customerId, string serviceMethodName);
        
    }
}
