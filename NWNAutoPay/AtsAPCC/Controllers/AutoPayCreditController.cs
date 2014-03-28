using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using AtsAPCC.Filters;
using AtsAPCC.Models;
using System.Xml.Linq;
using System.Diagnostics;
using AtsATPCC.AutoPayCreditCardService;
using System.ServiceModel.Configuration;
using System.Collections.Specialized;
using System.Web;
using Microsoft.Practices.Unity;
using Unity.Mvc4;


namespace AtsAPCC.Controllers
{
    [NoCache]
    public class AutoPayCreditController : Controller
    {

        static public TraceSource _trace = new TraceSource("NWNAutoPaySite");
        static public TraceSource _errorTrace = new TraceSource("NWNAutoPaySite_Failures");
        static public TraceSource _maintenanceTrace = new TraceSource("NWNAutoPaySite_Maintenance");
        static public string dBServer;
        static public System.Collections.Specialized.NameValueCollection appSettings;
        static public string error = string.Empty;
      
        private readonly AutoPayCreditCardService _autoPayService;

        private const string PaymentCardIsInvalid = "The card information is invalid. Please re-enter the Card Number and Security Code.";

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="autoPayService"></param>
        public AutoPayCreditController(AutoPayCreditCardService autoPayService)
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("starting NWN AutoPay Error Page");
            _maintenanceTrace.TraceEvent(TraceEventType.Start, 400000);
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("NWN AutoPay Controller");
            _trace.TraceEvent(TraceEventType.Start, 100, "stating NWNAutoPay website controller");
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("starting NWN AutoPay Error Page");
            _errorTrace.TraceEvent(TraceEventType.Start, 300000);

            appSettings = ConfigurationManager.AppSettings;

            
            ClientSection clientSection = (ClientSection)ConfigurationManager.GetSection("system.serviceModel/client");
            string address;
            for (int i = 0; i < clientSection.Endpoints.Count; i++)
            {
                address = clientSection.Endpoints[i].Address.ToString();
                _trace.TraceEvent(TraceEventType.Information, 110, string.Format("AtiWrapperServices Endpoints: {0}", address));
            }

            dBServer = "NA";
            _trace.TraceEvent(TraceEventType.Information, 150, string.Format("database server: {0}", dBServer));
            _trace.TraceEvent(TraceEventType.Information, 155, string.Format("NWNAutoPaySiteMode is: {0}", appSettings["NWNAutoPaySiteMode"]));

            _autoPayService = autoPayService;
        }

        public void CheckTestErrorPageMode()
        {
            if (appSettings["TestErrorPage"].Equals("1"))
            {
                throw new HttpException(999998, "Go Directly to the Error Page");
            }

        }
       
        public void CheckMaintenaceMode(string screenName)
        {
            App_In_Maintenance.UtilitySoapClient maintenanceClient = new App_In_Maintenance.UtilitySoapClient();

            string appName = string.Empty;
            try
            {
                appName = appSettings["AppName"];
            }
            catch (Exception we)
            {
                _trace.TraceEvent(TraceEventType.Error, 157, string.Format("Web.Config AppName is not defined: {0}", we.Message));
                throw new Exception("Web.Config AppName is not defined");
            }

            bool result = maintenanceClient.App_In_Maintenance(appName);
            if (result)
            {
                MaintenancePage(screenName);
            }
        }

        public void MaintenancePage(string screen)
        {
            _maintenanceTrace.TraceEvent(TraceEventType.Error, 400100, string.Format("NNWAutoPay Maintenance from screen: {0}", screen));
            _maintenanceTrace.TraceEvent(TraceEventType.Stop, 400200);
            throw new HttpException(999999, "Maintenance Mode");
        }

        public void ErrorPage(string screen)
        {
            _maintenanceTrace.TraceEvent(TraceEventType.Error, 400300, string.Format("NNWAutoPay Error from screen: {0}", screen));
            _maintenanceTrace.TraceEvent(TraceEventType.Stop, 400400);
            throw new HttpException(999998, "Maintenance Mode");
        }


        public ActionResult Signup()
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("NWN AutoPay Sign up Get");
            CheckTestErrorPageMode();
            CheckMaintenaceMode("Signup");
#if DEBUG
            _trace.TraceEvent(TraceEventType.Information, 9000, "Debug version so Sigup is allowed");
            _trace.TraceEvent(TraceEventType.Stop, 9999, "stopping NWNAutoPay website Sign Up");

            //TODO Remove after testing
            //throw new HttpException(404, "Not Found");
            return View();
#else
            _trace.TraceEvent(TraceEventType.Information, 9001, "Release version so Sigup is Not allowed");
             _trace.TraceEvent(TraceEventType.Stop, 9999, "stopping NWNAutoPay website Sign Up");
            return View("NotAvailable");
#endif
        }


        [HttpPost]
        public ActionResult Signup(string accountNumber, string firstName, string lastName, string phoneNumber,
                                   string emailAddress, string billingZipCode)
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("NWN AutoPay Sign up Post");
            _trace.TraceEvent(TraceEventType.Start, 200, "stating NWNAutoPay website Sign Up");
            CheckMaintenaceMode("Signup post");
            var model = new AutoPayEditModel
                {
                    AccountNumber = accountNumber,
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = phoneNumber,
                    EmailAddress = emailAddress,
                    ExpirationMonth = DateTime.Now.Month,
                    BillingZipCode = billingZipCode
                };
            Session["model"] = model;
            _trace.TraceEvent(TraceEventType.Stop, 299, "stopping NWNAutoPay website Sign Up");
            return RedirectToAction("Index");
            
        }

        public ActionResult Index()
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("NWN AutoPay Controller Index Get");
            _trace.TraceEvent(TraceEventType.Start, 1000, "Start NWN AutoPay Controller Index GetAction");
           // CheckMaintenaceMode("Index");
            CheckTestErrorPageMode();
            ViewBag.Message = "Index";

            var model = (AutoPayEditModel) Session["model"];

            SetViewDropDownItems();
            _trace.TraceEvent(TraceEventType.Stop, 1999, "NWN AutoPay Controller Index Get Complete");
            Trace.CorrelationManager.StopLogicalOperation();
            return View(model);
            
        }

        [HttpPost]
        public ActionResult Index(AutoPayEditModel model)
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("NWN AutoPay Controller Index Put");
            _trace.TraceEvent(TraceEventType.Start, 2000, "Start NWN AutoPay Controller Index Put Action");
            CheckTestErrorPageMode();
            CheckMaintenaceMode("Index post");
            var isEligible = IsAutoPayEligible(model);

            if (isEligible.Item1)
                return RedirectToAction("NotAvailable");

            if (!isEligible.Item2)
                return RedirectToAction("NotEligible");

            var isCardInfoValid = IsPaymentCardValid(model);

            if (!isCardInfoValid)
            {
                if(!string.IsNullOrEmpty( model.PaymentCardNumber) )
                    ModelState.AddModelError("PaymentCardNumber", PaymentCardIsInvalid);


                ModelState.AddModelError("PaymentCardSecurityCode", "");
            }

            if (ModelState.IsValid)
            {
                Session["model"] = model;
                return RedirectToAction("Verify");
            }

            // Required to clear the attempted value from the model state.
            ModelState.SetModelValue("PaymentCardNumber", new ValueProviderResult(string.Empty, string.Empty, CultureInfo.InvariantCulture));
            ModelState.SetModelValue("PaymentCardSecurityCode", new ValueProviderResult(string.Empty, string.Empty, CultureInfo.InvariantCulture));

            // Clear out account and security code info in the model before returning to the view.
            model.ClearPaymentCardInformation();

            SetViewDropDownItems();
            _trace.TraceEvent(TraceEventType.Stop, 2999, "NWN AutoPay Controller Index Put Complete");
            Trace.CorrelationManager.StopLogicalOperation();
            return View(model);
        }

        [HttpGet]
        public ActionResult Verify()
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("NWN AutoPay Controller Verify Get");
            _trace.TraceEvent(TraceEventType.Start, 3000, "Start NWN AutoPay Controller Verify Get Action");
            CheckMaintenaceMode("Verify");
            var model = ( AutoPayEditModel )Session[ "model" ];

            //if card number and security code cleared, redirect to page 1
            var isComplete = Session[ "complete" ];

            if( ( string )isComplete == "true" )
            {
                Session[ "complete" ] = null;
                model.ClearPaymentCardInformation();

                Session[ "model" ] = model;
                return RedirectToAction( "Index" );
            }

            _trace.TraceEvent(TraceEventType.Stop, 3999, "NWN AutoPay Controller Verify Get Complete");
            Trace.CorrelationManager.StopLogicalOperation();
            return View(model);
        }

        [HttpPost]
        public ActionResult Verify(AutoPayEditModel nullModel)
        {

            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("NWN AutoPay Controller Verify Put");
            _trace.TraceEvent(TraceEventType.Start, 4000, "Start NWN AutoPay Controller Verify Put Action");
            CheckMaintenaceMode("Verify post");
            // Since the model doesn't contain the card info, use the model in session.
            var signup = (AutoPayEditModel) Session["model"];

            if (signup == null)
                return RedirectToAction("NotAvailable");

            //if card number and security code cleared, redirect to page 1
            var isComplete = Session[ "complete" ];

            if ((string) isComplete == "true")
            {
                Session[ "complete" ] = null;
                signup.ClearPaymentCardInformation();
                Session[ "model" ] = signup;

                return RedirectToAction( "Index" );
            }

            string paymentReference = SubmitPaymentInformation(signup);
            if (paymentReference == string.Empty)
            {
                return RedirectToAction("Error");
            }
            AutoPayEnrollCreditCardResponse response = SubmitAutoPayEnrollment(signup, paymentReference);
            if (response == null)
            {
                _trace.TraceEvent(TraceEventType.Error, 4501, string.Format("NWN AutoPay Controller SubmitAutoPayEnrollment for account: "));
                return RedirectToAction("Error");
            }
            
            _trace.TraceEvent(TraceEventType.Information, 4500, string.Format("NWN AutoPay Controller Verify Put Complete response: {0}", response.Response));
            if (response.Confirmation == 0)
            {
                _trace.TraceEvent(TraceEventType.Error, 4502, string.Format("NWN AutoPay Controller SubmitAutoPayEnrollment response status is Error"));
                return RedirectToAction("Error");
            }

            AddConfirmationToSession( response.Confirmation.ToString(
                   CultureInfo.InvariantCulture ), response.Response == AutoPayServiceResponse.OK );

            _trace.TraceEvent(TraceEventType.Stop, 4999, "NWN AutoPay Controller Verify Put Complete");
            Trace.CorrelationManager.StopLogicalOperation();

            return RedirectToAction("Receipt");
        }

        public ActionResult Error()
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("NWN AutoPay Controller Error");
            _trace.TraceEvent(TraceEventType.Start, 5000, "Start NWN AutoPay Controller Error Action");
            CheckMaintenaceMode("Error");

            _trace.TraceEvent(TraceEventType.Stop, 5999, "NWN AutoPay Controller Verify Put Complete");
            Trace.CorrelationManager.StopLogicalOperation();
            return View("NotAvailable");
        }

        public ActionResult Receipt()
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("NWN AutoPay Controller Receipt");
            _trace.TraceEvent(TraceEventType.Start, 6000, "Start NWN AutoPay Controller Receipt Action");
            CheckMaintenaceMode("Receipt");
            var model = (AutoPayEditModel)Session["model"];

            Session[ "complete" ] = "true";

            _trace.TraceEvent(TraceEventType.Stop, 6999, "NWN AutoPay Controller Verify Receipt Complete");
            Trace.CorrelationManager.StopLogicalOperation();
            return View(model);
        }

        public ActionResult NotEligible()
        {
            return View("NotEligible");
        }

        public ActionResult NotAvailable()
        {
            //MaintenancePage("AutoPayCrediNotAvailable");
            return View("NotAvailable");
        }
        

        public ActionResult Redirect(string originalController, string originalAction)
        {
            return Redirect(GetFormattedUrl(string.Empty, string.Empty, originalController, originalAction));
        }

        public ActionResult RedirectArea(string originalArea, string originalController, string originalAction)
        {
            return Redirect(GetFormattedUrl(originalArea, string.Empty, originalController, originalAction));
        }

        public ActionResult RedirectSubArea(string originalArea, string originalSubArea, string originalController, string originalAction)
        {
            return Redirect(GetFormattedUrl(originalArea, originalSubArea, originalController, originalAction));
        }

        protected Tuple<bool, bool> IsAutoPayEligible(AutoPayEditModel model)
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("NWN AutoPay Controller IsAutoPayEligible");
            _trace.TraceEvent(TraceEventType.Start, 7000, "Start NWN AutoPay Controller IsAutoPayEligible Action");
            // NWN Code
            //var response = _autoPayService.IsEligibleForAutoPayCC(model.AccountNumber);

            //ATI code Please do not Change this section
            #region ATI
            AtiWrapperServices.AtiWrapperServicesClient atiWrapperServicesClient = new AtiWrapperServices.AtiWrapperServicesClient();
            AtiWrapperServices.WrapperResult wrapperResult = atiWrapperServicesClient.WrapperServiceByCustomerName(appSettings["EnrollCustomerName"], appSettings["EligibilityCallName"], string.Format("<accountNumber>{0}</accountNumber>", model.AccountNumber));
            AutoPayEligibilityResponse response = new AutoPayEligibilityResponse();
            if (appSettings["NWNAutoPaySiteMode"] == "Production" || appSettings["NWNAutoPaySiteMode"] == "Test")
            {
                if (wrapperResult.ResultStatus.Equals(AtiWrapperServices.WrapperResult.ATIWrapperServiceStatusEnum.Success))
                {
                    XDocument doc = XDocument.Parse(wrapperResult.Result.ToString());
                    response.AccountNumber = doc.Root.Elements("AccountNumber").FirstOrDefault().Value;
                    response.IsEligible = bool.Parse(doc.Root.Elements("IsEligible").FirstOrDefault().Value);
                    response.Response = (AutoPayServiceResponse)Enum.Parse(typeof(AutoPayServiceResponse), doc.Root.Elements("Response").FirstOrDefault().Value);
                }
                else
                {
                    response.AccountNumber = model.AccountNumber;
                    response.IsEligible = false;
                    response.Response = AutoPayServiceResponse.Error;
                }
            }
            else if (appSettings["NWNAutoPaySiteMode"] == "Development")
            {
                _trace.TraceEvent(TraceEventType.Information, 7500, string.Format("AutoPay IsEligable Development Mode"));
                response.AccountNumber = model.AccountNumber;
                response.IsEligible = true;
                response.Response = AutoPayServiceResponse.OK;
            }
            // End of ATI section
            #endregion ATI

            _trace.TraceEvent(TraceEventType.Information, 7500, string.Format("AutoPay IsEligable Response: {0} Is Eligable: {1}", response.Response, response.IsEligible));
            
            if (response.Response == AutoPayServiceResponse.Error && response.IsEligible)
            {
                _trace.TraceEvent(TraceEventType.Error, 7600, string.Format("AutoPay IsEligable Response: {0} Is Eligable: {1}", response.Response, response.IsEligible));
                response.Response = AutoPayServiceResponse.OK;
            }
            _trace.TraceEvent(TraceEventType.Stop, 7999, "NWN AutoPay Controller Verify Receipt Complete");
            Trace.CorrelationManager.StopLogicalOperation();
            return
                new Tuple<bool, bool>(
                    response.Response == AutoPayServiceResponse.Error,
                    response.Response == AutoPayServiceResponse.OK && response.IsEligible
                    );
        }

        protected bool IsPaymentCardValid(AutoPayEditModel model)
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("NWN AutoPay Controller IsPaymentCardValid");
            _trace.TraceEvent(TraceEventType.Start, 8000, "Start NWN AutoPay Controller IsPaymentCardValid Action");
            // Call to ATS Service for validation here
            //ATI Section Please Do Not Change
            #region ATI
            CardValidation.CardValidationServiceClient cardValidationClient = new CardValidation.CardValidationServiceClient();

            bool result = cardValidationClient.ValidateCard(model.PaymentCardNumber);
            _trace.TraceEvent(TraceEventType.Start, 8500, string.Format("AutoPay IsPaymentCardValid Response: {0} ", result));
            _trace.TraceEvent(TraceEventType.Stop, 8999, "NWN AutoPay Controller IsPaymentCardValid Complete");
            Trace.CorrelationManager.StopLogicalOperation();
            return result;
            //End ATI Section
            #endregion ATI
        }
        
        protected string SubmitPaymentInformation(AutoPayEditModel model)
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("NWN AutoPay Controller SubmitPaymentInformation");
            _trace.TraceEvent(TraceEventType.Start, 9000, "Start NWN AutoPay Controller SubmitPaymentInformation Action");
            // Call to ATS Service for payment submit here
            //ATI Section please do not change
            #region ATI
            int twoDigitYear = model.ExpirationYear - 2000;
            EnrollCard.EnrollCardServiceClient enrollCardClient = new EnrollCard.EnrollCardServiceClient();
            _trace.TraceEvent(TraceEventType.Start, 9200, string.Format("AutoPay SubmitPaymentInformation act: {0} last4: {1} exp M: {2} exp Y: {3} Fname: {4} LName: {5} FullName: {6} zip: {7} phone: {8} email {9}",
                model.AccountNumber, model.ObscuredPaymentCardNumber, model.ExpirationMonth, twoDigitYear, model.FirstName, model.LastName, model.NameOnCard, model.BillingZipCode, model.PhoneNumber, model.EmailAddress));

            EnrollCard.EnrollmentResult result = enrollCardClient.EnrollCard(appSettings["EnrollCustomerName"], model.AccountNumber, model.PaymentCardNumber, model.ObscuredPaymentCardNumber,
                model.ExpirationMonth, twoDigitYear, model.PaymentCardSecurityCode, model.FirstName, model.LastName, string.Empty, model.NameOnCard,
                string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, model.BillingZipCode, model.PhoneNumber, model.EmailAddress);

            _trace.TraceEvent(TraceEventType.Start, 9400, string.Format("AutoPay SubmitPaymentInformation result: {0} ", result.ResultStatus.GetHashCode().ToString()));
            _trace.TraceEvent(TraceEventType.Start, 9410, string.Format("AutoPay SubmitPaymentInformation result: {0} ", result.Result));
            if (!result.ResultStatus.Equals(EnrollCard.EnrollmentResult.ATIServiceBrokerStatusEnum.Success))
            {
                model.EnrollmentWasSuccessful = false;
                model.ConfirmationNumber = String.Empty;    
            }
            else
            {
                model.ConfirmationNumber = result.Result;
                model.EnrollmentWasSuccessful = true;
            }
            // End ATI Section
            #endregion ATI

            _trace.TraceEvent(TraceEventType.Start, 9500, string.Format("AutoPay SubmitPaymentInformation Response: {0} ", result.ResultStatus));
            _trace.TraceEvent(TraceEventType.Stop, 9999, "NWN AutoPay Controller SubmitPaymentInformation Complete");
            Trace.CorrelationManager.StopLogicalOperation();
            return result.transactionId;
        }

        protected AutoPayEnrollCreditCardResponse SubmitAutoPayEnrollment(AutoPayEditModel model, string paymentReference)
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("NWN AutoPay Controller SubmitAutoPayEnrollment");
            _trace.TraceEvent(TraceEventType.Start, 10000, "Start NWN AutoPay Controller SubmitAutoPayEnrollment Action");
            
            //NWN Code
            //var response = _autoPayService.EnrollAutoPayCC(model.AccountNumber, model.FirstName, model.LastName,
            //                                               model.PhoneNumber, model.EmailAddress,
            //                                               model.ObscuredPaymentCardNumber, DateTime.Now, "Web",
            //                                               model.PaymentCardType.GetValueOrDefault(), model.ExpirationDate, paymentReference, model.NameOnCard);

            ////ATI section Please do not Change
            #region ATI
            AtiWrapperServices.AtiWrapperServicesClient warpperServiceClient = new AtiWrapperServices.AtiWrapperServicesClient();
            AtiWrapperServices.WrapperResult wrapperResult = null;
            _trace.TraceEvent(TraceEventType.Information, 10200, string.Format("AutoPay SubmitAutoPayEnrollment  " + string.Format("<accountNumber>{0}</accountNumber><firstName>{1}</firstName><lastName>{2}</lastName><phoneNumber>{3}</phoneNumber><emailAddress>{4}</emailAddress><creditCardLast4>{5}</creditCardLast4><enrollDate>{6}</enrollDate><signature>{7}</signature><creditCardType>{8}</creditCardType><referenceId>{9}</referenceId><creditCardExpDate>{10}</creditCardExpDate><nameOnCard>{11}</nameOnCard>",
                    model.AccountNumber, model.FirstName, model.LastName, model.PhoneNumber, model.EmailAddress, model.ObscuredPaymentCardNumber, DateTime.Now, "Web", model.PaymentCardType.GetValueOrDefault(), paymentReference, model.ExpirationDate, model.NameOnCard)));
            AutoPayEnrollCreditCardResponse response = new AutoPayEnrollCreditCardResponse();


            if (appSettings["NWNAutoPaySiteMode"] == "Production" || appSettings["NWNAutoPaySiteMode"] == "Test")
            {

                wrapperResult = warpperServiceClient.WrapperServiceByCustomerName(appSettings["ConfirmCustomerName"], appSettings["ConfirmCallName"], string.Format("<accountNumber>{0}</accountNumber><firstName>{1}</firstName><lastName>{2}</lastName><phoneNumber>{3}</phoneNumber><emailAddress>{4}</emailAddress><creditCardLast4>{5}</creditCardLast4><enrollDate>{6}</enrollDate><signature>{7}</signature><creditCardType>{8}</creditCardType><referenceId>{9}</referenceId><creditCardExpDate>{10}</creditCardExpDate><nameOnCard>{11}</nameOnCard>",
                        model.AccountNumber, model.FirstName, model.LastName, model.PhoneNumber, model.EmailAddress, model.ObscuredPaymentCardNumber, DateTime.Now, "Web", model.PaymentCardType.GetValueOrDefault(), paymentReference, model.ExpirationDate, model.NameOnCard));
                
                if (wrapperResult.ResultStatus.Equals(AtiWrapperServices.WrapperResult.ATIWrapperServiceStatusEnum.Success))
                {
                    _trace.TraceEvent(TraceEventType.Information, 10300, string.Format("AutoPay SubmitAutoPayEnrollment Response: {0} ", wrapperResult.ResultStatus));
                    _trace.TraceEvent(TraceEventType.Information, 10310, string.Format("AutoPay SubmitAutoPayEnrollment Response: {0} ", wrapperResult.Result.ToString()));
                    XDocument doc = XDocument.Parse(wrapperResult.Result.ToString());
                    response.AccountNumber = doc.Root.Elements("AccountNumber").FirstOrDefault().Value;
                    response.Confirmation = long.Parse(doc.Root.Elements("Confirmation").FirstOrDefault().Value);

                    response.Response = (AutoPayServiceResponse)Enum.Parse(typeof(AutoPayServiceResponse), doc.Root.Elements("Response").FirstOrDefault().Value);
                    _trace.TraceEvent(TraceEventType.Information, 10399, string.Format("AutoPay SubmitAutoPayEnrollment Response: {0} ", response.Response));
                }
                else
                {

                    response.AccountNumber = model.AccountNumber;
                    response.Response = AutoPayServiceResponse.Error;
                    _trace.TraceEvent(TraceEventType.Error, 10400, string.Format("AutoPay SubmitAutoPayEnrollment Response: {0} ", response.Response));
                    return response;

                }
                //End ATI Section
            }
            else if (appSettings["NWNAutoPaySiteMode"] == "Development")
            {
                _trace.TraceEvent(TraceEventType.Information, 10299, string.Format("AutoPay SubmitAutoPayEnrollment Development Mode"));
                //This section is used for internal ATI development so that NWN staging or production is not hit.
                response.Confirmation = 12345;
                response.Response = AutoPayServiceResponse.OK;
            }
            #endregion ATI
            _trace.TraceEvent(TraceEventType.Information, 10500, string.Format("AutoPay SubmitAutoPayEnrollment Response: {0} ", response.Response));
            _trace.TraceEvent(TraceEventType.Stop, 10999, "NWN AutoPay Controller SubmitAutoPayEnrollment Complete");
            Trace.CorrelationManager.StopLogicalOperation();
            return response;
        }

        protected void AddConfirmationToSession(string confirmationNumber, bool enrollmentWasSuccessful)
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("NWN AutoPay Controller AddConfirmationToSession");
            _trace.TraceEvent(TraceEventType.Start, 11000, "Start NWN AutoPay Controller AddConfirmationToSession Action");
            var model = (AutoPayEditModel)Session["model"];

            // This is the production code that passes the real values
            model.ConfirmationNumber = confirmationNumber;
            model.EnrollmentWasSuccessful = enrollmentWasSuccessful;

            Session["model"] = model;
            _trace.TraceEvent(TraceEventType.Information, 11500, string.Format("AutoPay AddConfirmationToSession Response: {0} ConfirmationNumber: {1}", enrollmentWasSuccessful, confirmationNumber));
            _trace.TraceEvent(TraceEventType.Stop, 11999, "NWN AutoPay Controller AddConfirmationToSession Complete");
            Trace.CorrelationManager.StopLogicalOperation();
        }

        private void SetViewDropDownItems()
        {
            ViewBag.ExpirationMonthList = GetExpirationMonths();
            ViewBag.ExpirationYearList = GetExpirationYears();
        }

        private static IEnumerable<SelectListItem> GetExpirationMonths()
        {
            var months = Enumerable.Range(1, 12).Select(x => new SelectListItem
                {
                    Value = x.ToString(CultureInfo.InvariantCulture), Text = x.ToString(CultureInfo.InvariantCulture)
                });

            return months;
        }

        private static IEnumerable<SelectListItem> GetExpirationYears()
        {
            var months = Enumerable.Range(DateTime.Now.Year, 15).Select(x => new SelectListItem
            {
                Value = x.ToString(CultureInfo.InvariantCulture),
                Text = x.ToString(CultureInfo.InvariantCulture)
            });

            return months;
        }

        private static string GetFormattedUrl(string area, string subArea, string controller, string action)
        {
            var url = string.Format("{0}{1}{2}{3}{4}", ConfigurationManager.AppSettings["MainSiteUrl"],
                !string.IsNullOrWhiteSpace(area) ? "/" + area : "",
                !string.IsNullOrWhiteSpace(subArea) ? "/" + subArea : "", 
                !string.IsNullOrWhiteSpace(controller) ? "/" + controller : "", 
                !string.IsNullOrWhiteSpace(action) ? "/" + action : "");

            return url;
        }
    }
}
