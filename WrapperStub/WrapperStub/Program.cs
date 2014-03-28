using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrapperStub
{
    class Program
    {
        private static WrapperService.AtiWrapperServicesClient wrapperClient = new WrapperService.AtiWrapperServicesClient();
        
        static void Main(string[] args)
        {

//            409315
//3/12/2014 2:23:13 PM -07:00 [13] Information WrapperService 3101: John
//3/12/2014 2:23:13 PM -07:00 [13] Information WrapperService 3101: Public
//3/12/2014 2:23:13 PM -07:00 [13] Information WrapperService 3101: 5035551234
//3/12/2014 2:23:13 PM -07:00 [13] Information WrapperService 3101: jp@a.com
//3/12/2014 2:23:13 PM -07:00 [13] Information WrapperService 3101: 1111
//3/12/2014 2:23:13 PM -07:00 [13] Information WrapperService 3101: 3/12/2014 2:23:06 PM
//3/12/2014 2:23:13 PM -07:00 [13] Information WrapperService 3101: Web
//3/12/2014 2:23:13 PM -07:00 [13] Information WrapperService 3101: Visa
//3/12/2014 2:23:13 PM -07:00 [13] Information WrapperService 3101: 3/31/2014 12:00:00 AM
//3/12/2014 2:23:13 PM -07:00 [13] Information WrapperService 3101: 3067
//3/12/2014 2:23:13 PM -07:00 [13] Information WrapperService 3101: John Q Public

            //PassThruIsEligableForAutoPayCC
            WrapperService.WrapperResult tempusResult = new WrapperService.WrapperResult();

           tempusResult = wrapperClient.WrapperServiceByCustomerName("NWNAutoPay", "PassThruIsEligableForAutoPayCC", string.Format("<accountNumber>2040</accountNumber>"));
            Console.WriteLine(string.Format("Test Result Status: {0} Result value: {1}", tempusResult.ResultStatus.ToString(), tempusResult.Result.ToString()));
            Console.ReadKey();
            

            tempusResult = wrapperClient.WrapperServiceByCustomerName("NWNAutoPay", "PassThruIsEligableForAutoPayCC", string.Format("<accountNumber>2750019</accountNumber>"));
            Console.WriteLine(string.Format("Test Result Status: {0} Result value: {1}", tempusResult.ResultStatus.ToString(), tempusResult.Result.ToString()));
            Console.ReadKey();

            tempusResult = wrapperClient.WrapperServiceByCustomerName("NewTest6", "NewTest62", string.Format("<accountNumber>2750019</accountNumber>"));
            Console.WriteLine(string.Format("Test Result Status: {0} Result value: {1}", tempusResult.ResultStatus.ToString(), tempusResult.Result.ToString()));
            Console.ReadKey();

            
            
            tempusResult = wrapperClient.WrapperServiceByCustomerName("NWNAutoPay2", "PassThruEnrollAutoPayCC", string.Format("<accountNumber>2750019</accountNumber><firstName>John</firstName><lastName>Public</lastName><phoneNumber>5035551234</phoneNumber><emailAddress>jp@a.com</emailAddress><creditCardLast4>1111</creditCardLast4><enrollDate>3/12/2014 2:23:06 PM</enrollDate><signature>Web</signature><creditCardType>Visa</creditCardType><referenceId>3067</referenceId><creditCardExpDate>3/31/2014 12:00:00 AM</creditCardExpDate><nameOnCard>John Q Public</nameOnCard>"));
            Console.WriteLine(string.Format("Test Result Status: {0} Result value: {1}", tempusResult.ResultStatus.ToString(), tempusResult.Result.ToString()));
            Console.ReadKey();


            //GenericTempusCCAuthAvsOnly
            tempusResult = wrapperClient.WrapperServiceByCustomerNumber(5, "GenericTempusCCAuthAvsOnly",
                "<TRANSACTION.RNID>8883</TRANSACTION.RNID><TRANSACTION.RNCERT>1D9189E8CBB101A7A0804FBF8289323CE016F8D8</TRANSACTION.RNCERT><TRANSACTION.CCACCOUNT>4111111111111111</TRANSACTION.CCACCOUNT><TRANSACTION.CCEXP>02/15</TRANSACTION.CCEXP><TRANSACTION.CCAVS>97222</TRANSACTION.CCAVS>");
            Console.WriteLine(string.Format("Test Result Status: {0} Result value: {1}", tempusResult.ResultStatus.ToString(), tempusResult.Result.ToString()));
            Console.ReadKey();

            //GenericTempusCCAuthOnly
            tempusResult = wrapperClient.WrapperServiceByCustomerNumber(5, "GenericTempusCCAuthOnly",
                "<TRANSACTION.RNID>8883</TRANSACTION.RNID><TRANSACTION.RNCERT>1D9189E8CBB101A7A0804FBF8289323CE016F8D8</TRANSACTION.RNCERT><TRANSACTION.CCACCOUNT>4111111111111111</TRANSACTION.CCACCOUNT><TRANSACTION.CCEXP>02/15</TRANSACTION.CCEXP><TRANSACTION.CCAMT>11.00</TRANSACTION.CCAMT>");
            Console.WriteLine(string.Format("Test Result Status: {0} Result value: {1}", tempusResult.ResultStatus.ToString(), tempusResult.Result.ToString()));
            Console.ReadKey();

            //GenericTempusCCAuthAcctExpAmt
            tempusResult = wrapperClient.WrapperServiceByCustomerNumber(5, "GenericTempusCCAuthAcctExpAmt",
                "<TRANSACTION.RNID>8883</TRANSACTION.RNID><TRANSACTION.RNCERT>1D9189E8CBB101A7A0804FBF8289323CE016F8D8</TRANSACTION.RNCERT><TRANSACTION.CCACCOUNT>4111111111111111</TRANSACTION.CCACCOUNT><TRANSACTION.CCEXP>02/15</TRANSACTION.CCEXP><TRANSACTION.CCAMT>11.00</TRANSACTION.CCAMT>");
            Console.WriteLine(string.Format("Test Result Status: {0} Result value: {1}", tempusResult.ResultStatus.ToString(), tempusResult.Result.ToString()));
            Console.ReadKey();

            //GenrericTempusCCAuthSaleWAvs
            tempusResult = wrapperClient.WrapperServiceByCustomerNumber(5, "GenericTempusCCAuthSaleWAvs",
                "<TRANSACTION.RNID>8883</TRANSACTION.RNID><TRANSACTION.RNCERT>1D9189E8CBB101A7A0804FBF8289323CE016F8D8</TRANSACTION.RNCERT><TRANSACTION.CCACCOUNT>4111111111111111</TRANSACTION.CCACCOUNT><TRANSACTION.CCEXP>02/15</TRANSACTION.CCEXP><TRANSACTION.CCAMT>11.00</TRANSACTION.CCAMT><TRANSACTION.CCCVV>123</TRANSACTION.CCCVV><TRANSACTION.CCAVS>97222</TRANSACTION.CCAVS>");
            Console.WriteLine(string.Format("Test Result Status: {0} Result value: {1}", tempusResult.ResultStatus.ToString(), tempusResult.Result.ToString()));
            Console.ReadKey();

            //GenericTempusCCAuthSale
            tempusResult = wrapperClient.WrapperServiceByCustomerNumber(5, "GenericTempusCCAuthSale",
                "<TRANSACTION.RNID>8883</TRANSACTION.RNID><TRANSACTION.RNCERT>1D9189E8CBB101A7A0804FBF8289323CE016F8D8</TRANSACTION.RNCERT><TRANSACTION.CCACCOUNT>4111111111111111</TRANSACTION.CCACCOUNT><TRANSACTION.CCEXP>02/15</TRANSACTION.CCEXP><TRANSACTION.CCAMT>11.00</TRANSACTION.CCAMT><TRANSACTION.CCCVV>123</TRANSACTION.CCCVV>");
            Console.WriteLine(string.Format("Test Result Status: {0} Result value: {1}", tempusResult.ResultStatus.ToString(), tempusResult.Result.ToString()));
            Console.ReadKey();

            //GenericTempusCCCreditBySysCode
            tempusResult = wrapperClient.WrapperServiceByCustomerNumber(5, "GenericTempusCCCreditBySysCode",
                "<TRANSACTION.RNID>8883</TRANSACTION.RNID><TRANSACTION.RNCERT>1D9189E8CBB101A7A0804FBF8289323CE016F8D8</TRANSACTION.RNCERT><TRANSACTION.CCLOOKUPSYSTEMCODE>174211</TRANSACTION.CCLOOKUPSYSTEMCODE><TRANSACTION.CCAMT>1.00</TRANSACTION.CCAMT>");
            Console.WriteLine(string.Format("Test Result Status: {0} Result value: {1}", tempusResult.ResultStatus.ToString(), tempusResult.Result.ToString()));
            Console.ReadKey();

            //GenericTempusCCVoidBySysCode
            tempusResult = wrapperClient.WrapperServiceByCustomerNumber(5, "GenericTempusCCVoidBySysCode",
                "<TRANSACTION.RNID>8883</TRANSACTION.RNID><TRANSACTION.RNCERT>1D9189E8CBB101A7A0804FBF8289323CE016F8D8</TRANSACTION.RNCERT><TRANSACTION.CCSYSTEMCODE>174210</TRANSACTION.CCSYSTEMCODE>");
            Console.WriteLine(string.Format("Test Result Status: {0} Result value: {1}", tempusResult.ResultStatus.ToString(), tempusResult.Result.ToString()));
            Console.ReadKey();
        }
    }
}
