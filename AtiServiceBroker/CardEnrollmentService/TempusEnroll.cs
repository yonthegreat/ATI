using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ati.ServiceHost.Web;
using System.Diagnostics;
using System.Configuration;

namespace CardEnrollmentService
{
    class TempusEnroll
    {
        private AtiWrapperServices.AtiWrapperServicesClient wrapperClient = new AtiWrapperServices.AtiWrapperServicesClient();
        /// <summary>
        /// this Method performs a Tempus card enrollment. It generates a GUID for the card that ATI will use and Keeps the Tempus Token so that other card services can be used.
        /// </summary>
        /// <param name="fullCardNumber"></param>
        /// <param name="expiresMonth"></param>
        /// <param name="expiresYear"></param>
        /// <param name="zip"></param>
        /// <param name="tempusCardID"></param>
        /// <returns></returns>
        public EnrollmentResult TempusEnrollment(string fullCardNumber, int expiresMonth, int expiresYear, string zip, string tempusCardID)
        {
            int tempusId = 0;
            string tempusName = string.Empty;
            string tempusCallName = string.Empty;
            try
            {
                tempusId = Convert.ToInt32(WebServiceHostFactory.appSettings["TempusCustomerId"]);
                tempusName = WebServiceHostFactory.appSettings["TempusCustomerName"];
                tempusCallName = WebServiceHostFactory.appSettings["TempusEnrollCallName"];
            }
            catch (Exception we)
            {
                WebServiceHostFactory._trace.TraceEvent(TraceEventType.Error, 3405, "Tempus Web.Config not defines for TempusCustomerId, or TempusCustomerName or TempusEnrollCallName");
                throw new Exception("Tempus Web.Config not defines for TempusCustomerId, or TempusCustomerName or TempusEnrollCallName");
            }

            if (WebServiceHostFactory.cardTrace == 1)
            {
                Trace.CorrelationManager.ActivityId = Guid.NewGuid();
                Trace.CorrelationManager.StartLogicalOperation("Start Card Trace For Enrollment");
                WebServiceHostFactory._cardTrace.TraceEvent(TraceEventType.Start, 202000, "Start Card Trace For Enrollment");
                
            }
            EnrollmentResult result = new EnrollmentResult();
            result.ResultStatus = EnrollmentResult.ATIServiceBrokerStatusEnum.Other;
            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 3410, string.Format("TempusEnroll.TempusEnrollment: Tempus Enroll: {0}", tempusCardID));
            AtiWrapperServices.WrapperResult tempusResult = new AtiWrapperServices.WrapperResult();
            try
            {
                string EnrollmentMode = ConfigurationManager.AppSettings["EnrollmentMode"];
                if (WebServiceHostFactory.cardTrace == 1)
                {
                    WebServiceHostFactory._cardTrace.TraceEvent(TraceEventType.Information, 210000, string.Format("CardTrace: Number: {0} expMonth: {1} expYear: {2} zip {3}", fullCardNumber, String.Format("{0:00}", expiresMonth), String.Format("{0:00}", expiresYear), zip));
                }
                
                tempusResult = wrapperClient.WrapperServiceByCustomerNumber(tempusId, tempusCallName, string.Format("<TRANSACTION.REPACCOUNTNUMBER>{0}</TRANSACTION.REPACCOUNTNUMBER><TRANSACTION.REPACCOUNTEXP>{1}/{2}</TRANSACTION.REPACCOUNTEXP><TRANSACTION.REPACCOUNTAVS>{3}</TRANSACTION.REPACCOUNTAVS><TRANSACTION.REPCUSTIDENT>{4}</TRANSACTION.REPCUSTIDENT>",
                fullCardNumber, String.Format("{0:00}", expiresMonth), String.Format("{0:00}", expiresYear), zip, tempusCardID));

                if (tempusResult.ResultStatus == AtiWrapperServices.WrapperResult.ATIWrapperServiceStatusEnum.Success)
                {
                    WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 3420, string.Format("TempusEnroll.TempusEnrollment: Tempus Enroll: {0} Status: {1}", tempusCardID, "Success"));
                    result.ResultStatus = EnrollmentResult.ATIServiceBrokerStatusEnum.Success;
                    result.Result = tempusResult.Result.ToString();
                }
                else
                {
                    WebServiceHostFactory._trace.TraceEvent(TraceEventType.Warning, 3430, string.Format("TempusEnroll.TempusEnrollment: Tempus Enroll: {0} Status: {1}", tempusCardID, tempusResult.ResultStatus.ToString()));
                    result.ResultStatus = EnrollmentResult.ATIServiceBrokerStatusEnum.TempusBadStatus;
                    result.FailureReason = tempusResult.FailureReason.ToString();
                }
            }
            catch(Exception ex)
            {
                WebServiceHostFactory._trace.TraceEvent(TraceEventType.Error, 3450, string.Format("TempusEnroll.TempusEnrollment: Tempus Enroll: {0} Error: {1}", tempusCardID, ex.Message));
                result.ResultStatus = EnrollmentResult.ATIServiceBrokerStatusEnum.BadTempusCall;
                result.FailureReason = ex.Message;
                

            }

            if (WebServiceHostFactory.cardTrace == 1)
            {
                WebServiceHostFactory._cardTrace.TraceEvent(TraceEventType.Stop, 299999, "Stop Card Trace For Enrollment");
            }
      
            return result;

        }

        
    }


}
