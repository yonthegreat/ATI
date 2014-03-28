using Ati.ServiceHost;
using Ati.ServiceHost.Endpoints;
using System;
using System.Diagnostics;
using Ati.ServiceHost.Web;
using CardServices;

using System.Text.RegularExpressions;

namespace CardServices
{
    [ExportService("CardTypeService", typeof(CardTypeService))]
    public class CardTypeService : ICardType
    {
        /// <summary>
        /// This method is used to determine what type of card is being used for the transaction.
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        public string CardType(string cardNumber)
        {
            string retVal = string.Empty;
            if (cardNumber.Substring(0, 1) == "4")
            {
                retVal = "Visa";
            }
            else
            {
                string first2 = cardNumber.Substring(0, 2);
                switch (first2)
                {
                    case "50":
                    case "51":
                    case "52":
                    case "53":
                    case "54":
                    case "55":
                        retVal = "MC";
                        break;
                    case "36":
                    case "38":
                        retVal = "DC";
                        break;
                    case "60":
                    case "65":
                        retVal = "Disc";
                        break;
                    case "34":
                    case "37":
                        retVal = "Amex";
                        break;
                    default:
                        break;
                }
            }
            return retVal;

        }
    }
}