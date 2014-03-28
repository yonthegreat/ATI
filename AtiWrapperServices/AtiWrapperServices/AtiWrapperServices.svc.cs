using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Diagnostics;
using AtiWrapperServicesORM.OpenAccess;
namespace AtiWrapperServices
{
    /// <summary>
    /// AtiWrapperServices AtiWrapperServices class implements the methods defined in the IWrapperService interface
    /// </summary>
    public class AtiWrapperServices : IAtiWrapperServices
    {
        /// <summary>
        /// This method is used to add services to a running version of the AtiWrapperServices
        /// WARNING THIS METHOD IS NOT TESTED!!!!
        /// </summary>
        /// <param name="customerId">AtiWrapperServices service customerId</param>
        /// <param name="serviceMethodName">AtiWrapperServices service Name</param>
        public void AddNewServiceProxy(int customerId, string serviceMethodName)
        {
            ServiceProxies.AddNewServiceProxy(customerId, serviceMethodName);
        }

        /// <summary>
        /// This method is used to test the database connection string by querying the customer table.
        /// It returns either the customer Id or name depending on which parameter is Empty or 0.
        /// </summary>
        /// <param name="name">AtiWrapperServices Wrapper customer Name</param>
        /// <param name="id">AtiWrapperServices Wrapper custmoerId</param>
        /// <returns>string value of either customerName or CustomerId</returns>
        protected string TestConnectionString(string name, int id)
        {
            int customerId = 0;
            string customerName = string.Empty;
            using (AtiWrapperServicesModel dbContext = new AtiWrapperServicesModel())
            {
                try
                {
                    if (name != string.Empty && id == 0)
                    {
                        customerId = (from c in dbContext.Customers where c.Name == name select c.Id).FirstOrDefault();
                        return customerId.ToString();
                    }
                    else if (name == string.Empty && id > 0)
                    {
                        customerName = (from c in dbContext.Customers where c.Id == id select c.Name).FirstOrDefault();
                        return customerName;
                    }
                }
                catch (Exception ce)
                {
                    ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Critical, 101, string.Format("Error Database connection: {0} Exception: {1}", ServiceProxies.DBServer, ce.Message));
                    throw new Exception("Bad ConnectionString");
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// This method uses the wrapper specified in the serviceName argument to be invoked and returns the result in a WrapperResult object
        /// </summary>
        /// <param name="customerName">AtiWrapperServices wrapper customer Name</param>
        /// <param name="serviceName">AtiWrapperServices wrapper Name</param>
        /// <param name="serviceParameters">AtiWrapperServices wrapper string of parameters</param>
        /// <returns>WrapperResult</returns>
        public WrapperResult WrapperServiceByCustomerName(string customerName, string serviceName, string serviceParameters)
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation(" WrapperService Proxy Call Method");
            ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Information, 100, string.Format("Calling service method: {0}", serviceName));
            //test the connection string before anything else happens
            int customerId = Convert.ToInt32(TestConnectionString(customerName, 0));

            WrapperOperations wo = new WrapperOperations();
            WrapperResult retval = new WrapperResult();
            retval.FailureReason = string.Format("Customer Id for Name: {0} is invalid", customerName);
            retval.ResultStatus = WrapperResult.ATIWrapperServiceStatusEnum.Other;
            retval = wo.MakeWrapperCallWithParameters(GetConfigMode(), customerId, serviceName, serviceParameters);
            if (retval.ResultStatus == WrapperResult.ATIWrapperServiceStatusEnum.Success)
            {
                ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Information, 200, string.Format("Successful service method: {0} Returned: {1}", serviceName, retval.Result));
            }
            else
            {
                ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Information, -2000, string.Format("Failed service method: {0} Reason: {1}", serviceName, retval.FailureReason));
            }
            
            ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Stop, 7000, "WrapperService Proxy Method returned");
            Trace.CorrelationManager.StopLogicalOperation();
            return retval;
            
        }

        /// <summary>
        /// This method returns False if the application is in Production
        /// or true for all other modes. It is used internally to determine when to used Test or Production Wrappers and Services
        /// </summary>
        /// <returns>boolean</returns>
        public AtiWrapperServicesEnums.ServiceMode GetConfigMode()
        {
            AtiWrapperServicesEnums.ServiceMode mode;
            if (! Enum.TryParse(ServiceProxies.AppSettings["WrapperServiceMode"], out mode))
            {
                mode = AtiWrapperServicesEnums.ServiceMode.Test;
                ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Error, 130, "WrapperServiceMode is not set: using Test mode");
            }
            return mode;
        }

        /// <summary>
        /// This method uses the wrapper specified in the serviceName argument to be invoked and returns the result in a WrapperResult object
        /// </summary>
        /// <param name="customerNumber">AtiWrapperServices wrapper customerId</param>
        /// <param name="serviceName">AtiWrapperServices wrapper Name</param>
        /// <param name="serviceParameters">AtiWrapperServices wrapper string of parameters</param>
        /// <returns>WrapperResult</returns>
        public WrapperResult WrapperServiceByCustomerNumber(int customerNumber, string serviceName, string serviceParameters)
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation(" WrapperService Proxy Call Method");
            ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Information, 100, string.Format("Calling service method: {0}", serviceName));
            //test the connection string before anything else happens
            TestConnectionString(string.Empty, customerNumber);

            WrapperOperations wo = new WrapperOperations();
            WrapperResult retval = wo.MakeWrapperCallWithParameters(GetConfigMode(), customerNumber, serviceName, serviceParameters);
            if (retval != null && retval.ResultStatus == WrapperResult.ATIWrapperServiceStatusEnum.Success)
            {
                ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Information, 200, string.Format("Successful service method: {0} Returned: {1}", serviceName, retval.Result));
            }
            else
            {
                ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Information, -2000, string.Format("Failed service method: {0} Reason: {1}", serviceName, retval.FailureReason));
            }
            ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Stop, 7000, "WrapperService Proxy Method returned");
            Trace.CorrelationManager.StopLogicalOperation();
            return retval;
        }


        /// <summary>
        /// This method uses the wrapper specified in the serviceName argument to be invoked and returns the result in a WrapperResult object
        /// </summary>
        /// <param name="customerName">AtiWrapperServices wrapper customer Name</param>
        /// <param name="serviceMode">AtiWrapperServices ServiceMode (Test or Production)</param>
        /// <param name="serviceName">AtiWrapperServices wrapper Name</param>
        /// <param name="serviceParameters">AtiWrapperServices wrapper string of parameters</param>
        /// <returns>WrapperResult</returns>
        public WrapperResult WrapperServiceByCustomerNameAndServiceMode(string customerName, AtiWrapperServicesEnums.ServiceMode serviceMode, string serviceName, string serviceParameters)
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation(" WrapperService Proxy Call Method");
            ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Information, 100, string.Format("Calling service method: {0}", serviceName));

            //test the connection string before anything else happens
            int customerId = Convert.ToInt32(TestConnectionString(customerName, 0));

            WrapperOperations wo = new WrapperOperations();          
            WrapperResult retval = null;
            retval = wo.MakeWrapperCallWithParameters(GetConfigMode(), customerId, serviceName, serviceParameters);
            
            if (retval != null && retval.ResultStatus == WrapperResult.ATIWrapperServiceStatusEnum.Success)
            {
                ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Information, 200, string.Format("Successful service method: {0} Returned: {1}", serviceName, retval.Result));
            }
            else
            {
                ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Information, -2000, string.Format("Failed service method: {0} Reason: {1}", serviceName, retval.FailureReason));
            }

            ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Stop, 7000, "WrapperService Proxy Method returned");
            Trace.CorrelationManager.StopLogicalOperation();
            return retval;
        }
    }
}
