using System.ServiceModel;
using Ati.ServiceHost;


namespace CardServices
{
    /// <summary>
    /// Contract for implementing a card validation service.
    /// </summary>
    /// 
    [ServiceContract]
    public interface ICardValidationService : IHostedService
    {
        #region Methods
        /// <summary>
        /// performs validation of card number
        /// </summary>
        /// <param name="cardNumber"></param>
        /// CardValidation
        /// <returns>result of validation check</returns>
        [OperationContract]
        bool ValidateCard(string cardNumber);
        #endregion
    }

    /// <summary>
    /// Contract for the CardType Service
    /// </summary>
     [ServiceContract]
    public interface ICardType : IHostedService
     {
         #region Methods
        /// <summary>
        /// performs validation of card number
        /// </summary>
        /// <param name="cardNumber"></param>
        /// CardValidation
        /// <returns>result of validation check</returns>
        [OperationContract]
        string CardType(string cardNumber);
        #endregion
     }
    
}
