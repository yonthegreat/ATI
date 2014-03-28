using System;
using AtsAPCC.AutoPayCreditCardServiceReference;
using AtsAPCC.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AtsAPCC.Tests.Models
{
    [TestClass]
    public class AutoPayEditModelTests
    {
        [TestMethod]
        public void model_returns_obscured_card_number()
        {
            var model = new AutoPayEditModel
                {
                    PaymentCardNumber = "1111111111114321"
                };

            var expected = "4321";

            Assert.AreEqual(expected, model.ObscuredPaymentCardNumber);
        }

        [TestMethod]
        public void model_missing_card_number_returns_wihtout_error()
        {
            var model = new AutoPayEditModel
            {
                PaymentCardNumber = ""
            };

            var expected = "";

            Assert.AreEqual(expected, model.ObscuredPaymentCardNumber);
        }

        [TestMethod]
        public void model_expiration_date_returned_as_datetime_thirtyone_day_month()
        {
            var model = new AutoPayEditModel
            {
                ExpirationMonth = 5,
                ExpirationYear = 2014
            };

            var expected = new DateTime(2014, 5, 31);

            Assert.AreEqual(expected, model.ExpirationDate);
        }

        [TestMethod]
        public void model_expiration_date_returned_as_datetime_thirty_day_month()
        {
            var model = new AutoPayEditModel
            {
                ExpirationMonth = 4,
                ExpirationYear = 2014
            };

            var expected = new DateTime(2014, 4, 30);

            Assert.AreEqual(expected, model.ExpirationDate);
        }

        [TestMethod]
        public void model_expiration_date_returned_as_datetime_february()
        {
            var model = new AutoPayEditModel
            {
                ExpirationMonth = 2,
                ExpirationYear = 2015
            };

            var expected = new DateTime(2015, 2, 28);

            Assert.AreEqual(expected, model.ExpirationDate);
        }

        [TestMethod]
        public void model_expiration_date_returned_as_datetime_leap_year()
        {
            var model = new AutoPayEditModel
            {
                ExpirationMonth = 2,
                ExpirationYear = 2016
            };

            var expected = new DateTime(2016, 2, 29);

            Assert.AreEqual(expected, model.ExpirationDate);
        }

        [TestMethod]
        public void model_card_data_is_cleared()
        {
            var model = new AutoPayEditModel
            {
                PaymentCardNumber = "1111111111114321",
                PaymentCardSecurityCode = "111"
            };

            model.ClearPaymentCardInformation();

            Assert.AreEqual(string.Empty, model.PaymentCardNumber);
            Assert.AreEqual(string.Empty, model.PaymentCardSecurityCode);
        }

        [TestMethod]
        public void model_card_type_is_returned_visa()
        {
            var model = new AutoPayEditModel
            {
                PaymentCardNumber = "4111111111114321",
                PaymentCardSecurityCode = "111"
            };

            Assert.AreEqual(CreditCardTypes.Visa, model.PaymentCardType);
        }

        [TestMethod]
        public void model_card_type_is_returned_mastercard()
        {
            var model = new AutoPayEditModel
            {
                PaymentCardNumber = "5111111111114321",
                PaymentCardSecurityCode = "111"
            };

            Assert.AreEqual(CreditCardTypes.MasterCard, model.PaymentCardType);
        }

        [TestMethod]
        public void model_card_type_is_returned_discover()
        {
            var model = new AutoPayEditModel
            {
                PaymentCardNumber = "6011111111114321",
                PaymentCardSecurityCode = "111"
            };

            Assert.AreEqual(CreditCardTypes.Discover, model.PaymentCardType);
        }

        [TestMethod]
        public void model_card_type_is_returned_unknown()
        {
            var model = new AutoPayEditModel
            {
                PaymentCardNumber = "1111111111114321",
                PaymentCardSecurityCode = "111"
            };

            Assert.IsNull(model.PaymentCardType);
        }
    }
}
