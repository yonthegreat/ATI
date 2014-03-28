using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Ati.ServiceHost;
using System.Diagnostics;
using Ati.ServiceHost.Web;


namespace CustomerPassThruService
{
    /// <summary>
    /// defines Simple service pass through to Customer's web service through AtiServiceWrapper
    /// </summary>
    [ExportService("CustomerPassThruService", typeof(SimpleCustomerPassThruService))]
    public class SimpleCustomerPassThruService : ICustomerPassThruService
    {
        //private ServiceProxyDBContext db = new ServiceProxyDBContext();

        private AtiWrapperServices.AtiWrapperServicesClient wrapperClient = new AtiWrapperServices.AtiWrapperServicesClient();

        public object PassThruByName(string customerName, string serviceName, bool isTest, IEnumerable<object> parameters)
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("CustomerPassThruService");
            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Start, 22000, "Start CustomerPassThruService.PassThruByName");
            //var parameterNames = (from c in db.Customers
            //                                join u in db.ServiceURLs on c.Id equals u.Customer.Id
            //                                join n in db.ServiceNames on u.Id equals n.ServiceURL.Id
            //                                join m in db.ServiceMethods on n.Id equals m.ServiceName.Id
            //                                join w in db.WrapperMethods on m.Id equals w.ServiceMethod.Id
            //                                join i in db.WrapperInputParameters on w.Id equals i.WrapperMethod.Id
            //                                join v in db.WrapperValueProperties on i.Id equals v.WrapperInputParameter.Id
            //                                join q in db.QualifiedNames on v.QualifiedName.Id equals q.Id
            //                                where c.Name == customerName && n.Name == serviceName
            //                                select new { Name = q.Name }).ToList();
            //int custId = (from c in db.Customers
            //              where c.Name == customerName
            //              select c.Id).FirstOrDefault();

            //WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 2100, string.Format("CustomerPassThruService.PassThruByName CustName: {0} CustId: {1}", customerName, custId));

            //int paramCount = 0;
            //StringBuilder args = new StringBuilder();
            //object[] paramArray = parameters.ToArray();
            //foreach (var param in parameterNames)
            //{
            //    args.Append(string.Format("<{0}>{1}</{0}>",param.Name, paramArray[paramCount]));
            //    paramCount++;
            //}
            //AtiWrapperServices.WrapperResult result = new AtiWrapperServices.WrapperResult();
            //result.ResultStatus = AtiWrapperServices.WrapperResult.ATIWrapperServiceStatusEnum.Other;
            //WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 2200, string.Format("CustomerPassThruService.PassThruByName CustName: {0} ServiceName: {1}", customerName, serviceName));
            //if (isTest)
            //{
            //    result = wrapperClient.TestService(custId, serviceName, args.ToString());
            //}
            //else
            //{
            //    result = wrapperClient.ProductionService(custId, serviceName, args.ToString());
            //}
            //WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 2700, string.Format("CustomerPassThruService.PassThruByName CustName: {0} ServiceName: {1} Status: {2}", customerName, serviceName, result.ResultStatus.ToString()));
            //WebServiceHostFactory._trace.TraceEvent(TraceEventType.Stop, 2999, "CustomerPassThruService.PassThruByName Complete");
            //Trace.CorrelationManager.StopLogicalOperation();
			
            AtiWrapperServices.WrapperResult result = new AtiWrapperServices.WrapperResult();
            return result.Result;
        }


        public object PassThruByNumber(int customerNumber, string serviceName, bool isTest, IEnumerable<object> parameters)
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("CustomerPassThruService");
            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Start, 23000, "Start CustomerPassThruService.PassThruByNumber");

            //var parameterNames = (from c in db.Customers
            //                      join u in db.ServiceURLs on c.Id equals u.Customer.Id
            //                      join n in db.ServiceNames on u.Id equals n.ServiceURL.Id
            //                      join m in db.ServiceMethods on n.Id equals m.ServiceName.Id
            //                      join w in db.WrapperMethods on m.Id equals w.ServiceMethod.Id
            //                      join i in db.WrapperInputParameters on w.Id equals i.WrapperMethod.Id
            //                      join v in db.WrapperValueProperties on i.Id equals v.WrapperInputParameter.Id
            //                      join q in db.QualifiedNames on v.QualifiedName.Id equals q.Id
            //                      where c.Id == customerNumber && n.Name == serviceName
            //                      select new { Name = q.Name }).ToList();

            //int paramCount = 0;
            //StringBuilder args = new StringBuilder();
            //object[] paramArray = parameters.ToArray();
            //foreach (var param in parameterNames)
            //{
            //    args.Append(string.Format("<{0}>{1}</{0}>", param.Name, paramArray[paramCount]));
            //    paramCount++;
            //}
            //AtiWrapperServices.WrapperResult result = new AtiWrapperServices.WrapperResult();
            //result.ResultStatus = AtiWrapperServices.WrapperResult.ATIWrapperServiceStatusEnum.Other;
            //WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 2200, string.Format("CustomerPassThruService.PassThruByNumber CustName: {0} ServiceName: {1}", customerNumber, serviceName));
            //if (isTest)
            //{
            //    result = wrapperClient.TestService(customerNumber, serviceName, args.ToString());
            //}
            //else
            //{
            //    result = wrapperClient.ProductionService(customerNumber, serviceName, args.ToString());
            //}
            //WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 2700, string.Format("CustomerPassThruService.PassThruByNumber CustNumber: {0} ServiceName: {1} Status: {2}", customerNumber, serviceName, result.ResultStatus.ToString()));
            //WebServiceHostFactory._trace.TraceEvent(TraceEventType.Stop, 2999, "CustomerPassThruService.PassThruByName Complete");
            //Trace.CorrelationManager.StopLogicalOperation();

            AtiWrapperServices.WrapperResult result = new AtiWrapperServices.WrapperResult();
            return result.Result;
        }
    }
}
