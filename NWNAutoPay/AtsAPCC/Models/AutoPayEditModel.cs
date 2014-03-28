using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AtsATPCC.AutoPayCreditCardService;
using AtsAPCC.DataAnnotations;

namespace AtsAPCC.Models
{
    public class AutoPayEditModel
    {
        [DisplayName("Account Number:")]
        public string AccountNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Name on Card is required")]
        [PaymentCardName]
        [DisplayName("Name on Card:")]
        [StringLength( 40, MinimumLength = 2, ErrorMessage = "Name on Card must be between 2 and 40 characters" )]
        public string NameOnCard { get; set; }

        public int PaymentCardTypeId { get; set; }

        [Required(ErrorMessage = "Card Number is required")]
        [PaymentCardNumber]
        [DisplayName("Card Number:")]
        public string PaymentCardNumber { get; set; }

        // Potentially replaced by call to ATS service
        public CreditCardTypes? PaymentCardType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(PaymentCardNumber))
                    return null;

                if (PaymentCardNumber.Length < 4)
                    return null;

                switch (PaymentCardNumber.Substring(0, 1))
                {
                    case "4":
                        return CreditCardTypes.Visa;
                    case "5":
                        return CreditCardTypes.MasterCard;
                    default:
                        if (PaymentCardNumber.Substring(0, 4) == "6011")
                            return CreditCardTypes.Discover;
                        return null;
                }
            }
        }

        [DisplayName("Card Number:")]
        public string ObscuredPaymentCardNumber { 
            get { return PaymentCardNumber.Length < 4 ? PaymentCardNumber : PaymentCardNumber.Substring(PaymentCardNumber.Length - 4, 4); }  
        }

        [Required(ErrorMessage = "Security Code is required")]
        [PaymentCardSecurityCode]
        [DisplayName("Security Code:")]
        public string PaymentCardSecurityCode { get; set; }

        [DisplayName("Card Expiration Date:")]
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }

        public DateTime ExpirationDate
        {
            get
            {
                return new DateTime(ExpirationYear, ExpirationMonth, 
                    new DateTime(ExpirationYear, ExpirationMonth, 1).AddMonths(1).AddDays(-1).Day);
            }
        }

        public string BillingZipCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Phone Number is required")]
        [PhoneNumber]
        [DisplayName("Phone Number:")]
        public string PhoneNumber { get; set; }

        [DisplayName("Confirmation Number:")]
        public string ConfirmationNumber { get; set; }

        public bool EnrollmentWasSuccessful { get; set; }

        public void ClearPaymentCardInformation()
        {
            PaymentCardNumber = string.Empty;
            PaymentCardSecurityCode = string.Empty;
        }
    }
}