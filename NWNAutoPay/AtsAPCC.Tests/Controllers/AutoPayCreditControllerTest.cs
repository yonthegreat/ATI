using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using AtsATPCC.AutoPayCreditCardService;
using AtsAPCC.Controllers;
using AtsAPCC.Models;
using AtsAPCC.Tests.MvcFakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StructureMap;

namespace AtsAPCC.Tests.Controllers
{
    [TestClass]
    public class AutoPayCreditControllerTest
    {
        private AutoPayCreditController _controller;
        private Mock<AutoPayCreditCardService> _mockAutoPayService;

        [TestInitialize]
        public void TestInit()
        {
            _mockAutoPayService = new Mock<AutoPayCreditCardService>();
            ObjectFactory.Initialize(init => init.For<AutoPayCreditCardService>().Use(_mockAutoPayService.Object));
            _controller = new AutoPayCreditController(_mockAutoPayService.Object);
            _controller.ControllerContext = new FakeControllerContext(_controller, new SessionStateItemCollection());
        }

        [TestCleanup]
        public void Cleanup()
        {
            ObjectFactory.ResetDefaults();
        }

        [TestMethod]
        public void landing_action_stores_account_info_in_session_and_redirects()
        {
            var expected = GetTestModel("", "", 0, 0);

            // Act
            var result = _controller.Signup(expected.AccountNumber, expected.FirstName, expected.LastName, expected.PhoneNumber, expected.EmailAddress, expected.BillingZipCode) as RedirectToRouteResult;
            var actual = _controller.ControllerContext.HttpContext.Session["model"] as AutoPayEditModel;
            // Assert

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.AccountNumber, actual.AccountNumber);
            Assert.AreEqual(expected.FirstName, actual.FirstName);
            Assert.AreEqual(expected.LastName, actual.LastName);
            Assert.AreEqual(expected.PhoneNumber, actual.PhoneNumber);
            Assert.AreEqual(expected.EmailAddress, actual.EmailAddress);
            Assert.AreEqual(expected.BillingZipCode, actual.BillingZipCode);

            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void index_returns_view()
        {
            var expected = GetTestModel("", "", 0, 0);
            _controller.ControllerContext.HttpContext.Session["model"] = expected;

            var result = _controller.Index() as ViewResult;
            var actual = result.Model as AutoPayEditModel;

            Assert.AreEqual("", result.ViewName);
            Assert.IsNotNull(result.Model);
            Assert.AreEqual(expected.AccountNumber, actual.AccountNumber);
            Assert.AreEqual(expected.FirstName, actual.FirstName);
            Assert.AreEqual(expected.LastName, actual.LastName);
            Assert.AreEqual(expected.PhoneNumber, actual.PhoneNumber);
            Assert.AreEqual(expected.EmailAddress, actual.EmailAddress);
        }

        [TestMethod]
        public void eligible_index_submit_adds_card_info_to_session()
        {
            _mockAutoPayService.Setup(x => x.IsEligibleForAutoPayCC(It.IsAny<string>()))
                               .Returns(new AutoPayEligibilityResponse
                                   {
                                       IsEligible = true,
                                       Response = AutoPayServiceResponse.OK
                                   });
            var expected = GetTestModel("4111111111110000", "100", 1, 2014);
            // Act
            var result = _controller.Index(expected) as RedirectToRouteResult;
            var actual = (AutoPayEditModel) _controller.ControllerContext.HttpContext.Session["model"];
            // Assert
            Assert.AreEqual("Verify", result.RouteValues["action"]);
            Assert.AreEqual(expected.AccountNumber, actual.AccountNumber);
            Assert.AreEqual(expected.FirstName, actual.FirstName);
            Assert.AreEqual(expected.LastName, actual.LastName);
            Assert.AreEqual(expected.PhoneNumber, actual.PhoneNumber);
            Assert.AreEqual(expected.EmailAddress, actual.EmailAddress);
            Assert.AreEqual(expected.PaymentCardNumber, actual.PaymentCardNumber);
            Assert.AreEqual(expected.PaymentCardSecurityCode, actual.PaymentCardSecurityCode);
            Assert.AreEqual(expected.ExpirationMonth, actual.ExpirationMonth);
            Assert.AreEqual(expected.ExpirationYear, actual.ExpirationYear);
        }

        [TestMethod]
        public void invalid_index_submit_returns_to_view()
        {
            _mockAutoPayService.Setup(x => x.IsEligibleForAutoPayCC(It.IsAny<string>()))
                               .Returns(new AutoPayEligibilityResponse
                               {
                                   IsEligible = true,
                                   Response = AutoPayServiceResponse.OK
                               });

            var expected = GetTestModel("41111111111100", "100", 1, 2014);
            // Act
            _controller.ModelState.AddModelError("", "Test Error");
            var result = _controller.Index(expected) as ViewResult;
            var actual = (AutoPayEditModel)result.Model;
            // Assert
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(expected.AccountNumber, actual.AccountNumber);
            Assert.AreEqual(expected.FirstName, actual.FirstName);
            Assert.AreEqual(expected.LastName, actual.LastName);
            Assert.AreEqual(expected.PhoneNumber, actual.PhoneNumber);
            Assert.AreEqual(expected.EmailAddress, actual.EmailAddress);
            Assert.AreEqual(string.Empty, actual.PaymentCardNumber);
            Assert.AreEqual(string.Empty, actual.PaymentCardSecurityCode);
            Assert.AreEqual(1, actual.ExpirationMonth);
            Assert.AreEqual(2014, actual.ExpirationYear);
        }

        [TestMethod]
        public void invalid_card_index_submit_returns_to_view()
        {
            _mockAutoPayService.Setup(x => x.IsEligibleForAutoPayCC(It.IsAny<string>()))
                               .Returns(new AutoPayEligibilityResponse
                               {
                                   IsEligible = true,
                                   Response = AutoPayServiceResponse.OK
                               });

            var expected = GetTestModel("11111111111100", "100", 1, 2014);
            // Act

            var result = _controller.Index(expected) as ViewResult;
            var actual = (AutoPayEditModel)result.Model;
            // Assert
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(expected.AccountNumber, actual.AccountNumber);
            Assert.AreEqual(expected.FirstName, actual.FirstName);
            Assert.AreEqual(expected.LastName, actual.LastName);
            Assert.AreEqual(expected.PhoneNumber, actual.PhoneNumber);
            Assert.AreEqual(expected.EmailAddress, actual.EmailAddress);
            Assert.AreEqual(string.Empty, actual.PaymentCardNumber);
            Assert.AreEqual(string.Empty, actual.PaymentCardSecurityCode);
            Assert.AreEqual(1, actual.ExpirationMonth);
            Assert.AreEqual(2014, actual.ExpirationYear);
        }

        [TestMethod]
        public void ineligibile_index_submit_redirects_to_ineligible_view()
        {
            _mockAutoPayService.Setup(x => x.IsEligibleForAutoPayCC(It.IsAny<string>()))
                               .Returns(new AutoPayEligibilityResponse
                               {
                                   IsEligible = false,
                                   Response = AutoPayServiceResponse.OK
                               });
            var model = GetTestModel("", "", 0, 0);
            // Act
            _controller.ControllerContext.HttpContext.Session["model"] = model;

            var expected = GetTestModel("4111111111110000", "100", 1, 2014);
            // Act
            var result = _controller.Index(expected) as RedirectToRouteResult;
            var actual = (AutoPayEditModel)_controller.ControllerContext.HttpContext.Session["model"];
            // Assert
            Assert.AreEqual("NotEligible", result.RouteValues["action"]);
            Assert.AreEqual(expected.AccountNumber, actual.AccountNumber);
            Assert.AreEqual(expected.FirstName, actual.FirstName);
            Assert.AreEqual(expected.LastName, actual.LastName);
            Assert.AreEqual(expected.PhoneNumber, actual.PhoneNumber);
            Assert.AreEqual(expected.EmailAddress, actual.EmailAddress);
            Assert.AreEqual(string.Empty, actual.PaymentCardNumber);
            Assert.AreEqual(string.Empty, actual.PaymentCardSecurityCode);
            Assert.AreEqual(0, actual.ExpirationMonth);
            Assert.AreEqual(0, actual.ExpirationYear);
        }

        [TestMethod]
        public void service_error_index_submit_returns_error_message()
        {
            _mockAutoPayService.Setup(x => x.IsEligibleForAutoPayCC(It.IsAny<string>()))
                               .Returns(new AutoPayEligibilityResponse
                               {
                                   IsEligible = false,
                                   Response = AutoPayServiceResponse.Error
                               });

            var expected = GetTestModel("41111111111100", "100", 1, 2014);
            // Act

            var result = _controller.Index(expected) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("NotAvailable", result.RouteValues["action"]);
        }

        [TestMethod]
        public void verify_returns_view()
        {
            var expected = GetTestModel("4111111111111111", "100", 1, 2014);
            _controller.ControllerContext.HttpContext.Session["model"] = expected;

            var result = _controller.Verify() as ViewResult;
            var actual = result.Model as AutoPayEditModel;

            Assert.AreEqual("", result.ViewName);
            Assert.IsNotNull(result.Model);
            Assert.AreEqual(expected.AccountNumber, actual.AccountNumber);
            Assert.AreEqual(expected.FirstName, actual.FirstName);
            Assert.AreEqual(expected.LastName, actual.LastName);
            Assert.AreEqual(expected.PhoneNumber, actual.PhoneNumber);
            Assert.AreEqual(expected.EmailAddress, actual.EmailAddress);
            Assert.AreEqual(expected.PaymentCardNumber, actual.PaymentCardNumber);
            Assert.AreEqual(expected.PaymentCardSecurityCode, actual.PaymentCardSecurityCode);
            Assert.AreEqual(expected.ExpirationMonth, actual.ExpirationMonth);
            Assert.AreEqual(expected.ExpirationYear, actual.ExpirationYear);
        }

        [TestMethod]
        public void submit_verify_success_redirects_to_receipt()
        {
            _mockAutoPayService.Setup(x => x.EnrollAutoPayCC(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                                                             It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                                                             It.IsAny<DateTime>(), It.IsAny<string>(),
                                                             It.IsAny<CreditCardTypes>(),
                                                             It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>() ) )
                               .Returns(new AutoPayEnrollCreditCardResponse
                                   {
                                       Confirmation = 123456,
                                       Response = AutoPayServiceResponse.OK
                                   });

            var expected = GetTestModel("4111111111111111", "100", 1, 2014);
            _controller.ControllerContext.HttpContext.Session["model"] = expected;

            var result = _controller.Verify(expected) as RedirectToRouteResult;
            var actual = (AutoPayEditModel) _controller.ControllerContext.HttpContext.Session["model"];

            Assert.AreEqual("Receipt", result.RouteValues["action"]);
            Assert.AreEqual(expected.AccountNumber, actual.AccountNumber);
            Assert.AreEqual(expected.FirstName, actual.FirstName);
            Assert.AreEqual(expected.LastName, actual.LastName);
            Assert.AreEqual(expected.PhoneNumber, actual.PhoneNumber);
            Assert.AreEqual(expected.EmailAddress, actual.EmailAddress);
            Assert.AreEqual(expected.PaymentCardNumber, actual.PaymentCardNumber);
            Assert.AreEqual(expected.PaymentCardSecurityCode, actual.PaymentCardSecurityCode);
            Assert.AreEqual(expected.ExpirationMonth, actual.ExpirationMonth);
            Assert.AreEqual(expected.ExpirationYear, actual.ExpirationYear);
            Assert.AreEqual("123456", actual.ConfirmationNumber);
            Assert.AreEqual(true, actual.EnrollmentWasSuccessful);

        }

        [TestMethod]
        public void submit_verify_error_redirects_to_receipt()
        {
            _mockAutoPayService.Setup(x => x.EnrollAutoPayCC(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<CreditCardTypes>(),
                It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>() ) )
                   .Returns(new AutoPayEnrollCreditCardResponse
                   {
                       Confirmation = 0,
                       Response = AutoPayServiceResponse.Error
                   });

            var expected = GetTestModel("4111111111111111", "100", 1, 2014);
            _controller.ControllerContext.HttpContext.Session["model"] = expected;

            var result = _controller.Verify(expected) as RedirectToRouteResult;
            var actual = (AutoPayEditModel)_controller.ControllerContext.HttpContext.Session["model"];

            Assert.AreEqual("Error", result.RouteValues["action"]);
            Assert.AreEqual(expected.AccountNumber, actual.AccountNumber);
            Assert.AreEqual(expected.FirstName, actual.FirstName);
            Assert.AreEqual(expected.LastName, actual.LastName);
            Assert.AreEqual(expected.PhoneNumber, actual.PhoneNumber);
            Assert.AreEqual(expected.EmailAddress, actual.EmailAddress);
            Assert.AreEqual(expected.PaymentCardNumber, actual.PaymentCardNumber);
            Assert.AreEqual(expected.PaymentCardSecurityCode, actual.PaymentCardSecurityCode);
            Assert.AreEqual(expected.ExpirationMonth, actual.ExpirationMonth);
            Assert.AreEqual(expected.ExpirationYear, actual.ExpirationYear);
            Assert.AreEqual("0", actual.ConfirmationNumber);
            Assert.AreEqual(false, actual.EnrollmentWasSuccessful);
        }

        [TestMethod]
        public void receipt_returns_view()
        {
            var expected = GetTestModel("4111111111111111", "100", 1, 2014);
            expected.ConfirmationNumber = "1234567";
            expected.EnrollmentWasSuccessful = true;
            _controller.ControllerContext.HttpContext.Session["model"] = expected;

            var result = _controller.Receipt() as ViewResult;
            var actual = result.Model as AutoPayEditModel;

            Assert.AreEqual("", result.ViewName);
            Assert.IsNotNull(result.Model);
            Assert.AreEqual(expected.AccountNumber, actual.AccountNumber);
            Assert.AreEqual(expected.FirstName, actual.FirstName);
            Assert.AreEqual(expected.LastName, actual.LastName);
            Assert.AreEqual(expected.PhoneNumber, actual.PhoneNumber);
            Assert.AreEqual(expected.EmailAddress, actual.EmailAddress);
            Assert.AreEqual(expected.PaymentCardNumber, actual.PaymentCardNumber);
            Assert.AreEqual(expected.PaymentCardSecurityCode, actual.PaymentCardSecurityCode);
            Assert.AreEqual(expected.ExpirationMonth, actual.ExpirationMonth);
            Assert.AreEqual(expected.ExpirationYear, actual.ExpirationYear);
            Assert.AreEqual(expected.ConfirmationNumber, actual.ConfirmationNumber);
            Assert.AreEqual(expected.EnrollmentWasSuccessful, actual.EnrollmentWasSuccessful);
        }

        [TestMethod]
        public void not_eligible_returns_view()
        {
            var expected = GetTestModel("", "", 0, 0);
            var result = _controller.NotEligible() as ViewResult;

            Assert.AreEqual("", result.ViewName);
            Assert.IsNull(result.Model);
        }

        [TestMethod]
        public void redirect_two_level_url_returns_route()
        {
            var actual = _controller.Redirect("Foo", "Bar") as RedirectResult;

            var expected = "http://www.test.com/Foo/Bar";

            Assert.AreEqual(expected, actual.Url);
        }

        [TestMethod]
        public void redirect_three_level_url_returns_route()
        {
            var actual = _controller.RedirectArea("You", "Foo", "Bar") as RedirectResult;

            var expected = "http://www.test.com/You/Foo/Bar";

            Assert.AreEqual(expected, actual.Url);
        }

        [TestMethod]
        public void redirect_four_level_url_returns_route()
        {
            var actual = _controller.RedirectSubArea("Hey", "You", "Foo", "Bar") as RedirectResult;

            var expected = "http://www.test.com/Hey/You/Foo/Bar";

            Assert.AreEqual(expected, actual.Url);
        }

        [TestMethod]
        public void month_dropdown_populated()
        {
            var pt = new PrivateType(typeof(AutoPayCreditController));

            var actual = (IEnumerable<SelectListItem>)pt.InvokeStatic("GetExpirationMonths");

            Assert.AreEqual(12, actual.Count());
            Assert.AreEqual("1", actual.First().Value);
            Assert.AreEqual("12", actual.Last().Value);
        }

        [TestMethod]
        public void year_dropdown_populated()
        {
            var pt = new PrivateType(typeof(AutoPayCreditController));

            var actual = (IEnumerable<SelectListItem>)pt.InvokeStatic("GetExpirationYears");

            Assert.AreEqual(15, actual.Count());
            Assert.AreEqual(DateTime.Now.Year.ToString(), actual.First().Value);
            Assert.AreEqual(DateTime.Now.AddYears(14).Year.ToString(), actual.Last().Value);
        }

        [TestMethod]
        public void zero_level_url_return_mapped_correctly()
        {
            var pt = new PrivateType(typeof(AutoPayCreditController));

            var expected = "http://www.test.com";

            var actual = (string)pt.InvokeStatic("GetFormattedUrl", new object[] { string.Empty, string.Empty, string.Empty, string.Empty });

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void one_level_url_return_mapped_correctly()
        {
            var pt = new PrivateType(typeof(AutoPayCreditController));

            var expected = "http://www.test.com/Bar";

            var actual = (string)pt.InvokeStatic("GetFormattedUrl", new object[] { string.Empty, string.Empty, string.Empty, "Bar" });

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void two_level_url_return_mapped_correctly()
        {
            var pt = new PrivateType(typeof (AutoPayCreditController));

            var expected = "http://www.test.com/Foo/Bar";

            var actual = (string) pt.InvokeStatic("GetFormattedUrl", new object[] { string.Empty, string.Empty, "Foo", "Bar"});

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void three_level_url_return_mapped_correctly()
        {
            var pt = new PrivateType(typeof(AutoPayCreditController));

            var expected = "http://www.test.com/You/Foo/Bar";

            var actual = (string)pt.InvokeStatic("GetFormattedUrl", new object[] { string.Empty, "You", "Foo", "Bar" });

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void four_level_url_return_mapped_correctly()
        {
            var pt = new PrivateType(typeof(AutoPayCreditController));

            var expected = "http://www.test.com/Hey/You/Foo/Bar";

            var actual = (string)pt.InvokeStatic("GetFormattedUrl", new object[] { "Hey", "You", "Foo", "Bar" });

            Assert.AreEqual(expected, actual);
        }

        public AutoPayEditModel GetTestModel(string paymentCardNumber, string paymentCardSecurityCode, int expirationMonth, int expirationYear)
        {
            return new AutoPayEditModel
                {
                    FirstName = "John",
                    LastName = "Public",
                    AccountNumber = "1231231",
                    PhoneNumber = "5035551111",
                    EmailAddress = "johnqpublic@t.com",
                    PaymentCardNumber = paymentCardNumber,
                    PaymentCardSecurityCode = paymentCardSecurityCode,
                    ExpirationMonth = expirationMonth,
                    ExpirationYear = expirationYear,
                    BillingZipCode = "97201"
                };
        }
    }
}
