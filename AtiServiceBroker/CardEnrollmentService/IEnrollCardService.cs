using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Ati.ServiceHost;

namespace CardEnrollmentService
{
    /// <summary>
    /// Defines the requried contract for implementing a calculator service.
    /// </summary>
    /// 
    [ServiceContract]
    public interface IEnrollCardService : IHostedService
    {
        #region Methods
        /// <summary>
        /// performs validation of card number
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns>result of validation check</returns>
        [OperationContract]
        EnrollmentResult EnrollCard(string CustomerName, string AccountAccountNumber, string CardFullNumber, string CardLast4,
                            int CardExpiresMonth, int CardExpriresYear, string CardSecurityCode, string CardHolderFirstName,
                            string CardHolderLastName, string CardHolderInitial, string CardHolderAppearsOnCard, string AddressAddress1,
                            string AddressAddress2, string AddressAddress3, string AddressCity, string AddressState, string AddressZip,
                            string ContactTelephone, string ContactEmail);

        //[OperationContract]
        //EnrollmentResult ConfirmCardEnrollment(string AccountAccountNumber, string CardLast4, int CardExpiresMonth, int CardExpriresYear);

        //[OperationContract]
        //EnrollmentResult DeactivateCardEnrollment(string AccountAccountNumber, string CardLast4, int CardExpiresMonth, int CardExpriresYear);
        #endregion
    }
}
