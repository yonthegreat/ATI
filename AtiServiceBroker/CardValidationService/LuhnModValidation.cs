using Ati.ServiceHost;
using Ati.ServiceHost.Endpoints;
using System;
using System.Diagnostics;
using Ati.ServiceHost.Web;
using CardServices;

using System.Text.RegularExpressions;

namespace CardServices
{

    /// <summary>
    /// defines the Luhn's Mod10 validation service
    /// </summary>
    [ExportService("LuhnModValidation", typeof(LuhnModValidation))]
    class LuhnModValidation : ICardValidationService
    {
        #region Methods
        /// <summary>
        /// validates card usings Luhn's Mod10 method
        /// </summary>
        /// <param name="cardNumber">the string representing the card Number</param>
        /// <returns>Result of validation check</returns>
        public bool ValidateCard(string cardNumber)
        {

            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("LuhnModValidation");
            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Start, 21000, "Start LuhnModValidation.ValidateCard");

            if (WebServiceHostFactory.cardTrace == 1)
            {
                Trace.CorrelationManager.ActivityId = Guid.NewGuid();
                Trace.CorrelationManager.StartLogicalOperation("Start Card Trace For luhn validation");
                WebServiceHostFactory._cardTrace.TraceEvent(TraceEventType.Start, 205000, "Start Card Trace For luhn validation");
                WebServiceHostFactory._cardTrace.TraceEvent(TraceEventType.Information, 205500, string.Format("CardTrace: Number: {0}", cardNumber));
            }

            bool functionReturnValue = false;
            string strCC = null;
            // Trimmed string
            int intIndex = 0;
            // String position
            bool intDouble = false;
            // Doubling flag
            int intCheckSum = 0;
            // Running sum
            byte[] byteDigit = { 0 };
            // Extracted intDigit
            int intDigit = 0;

            functionReturnValue = false;


            try
            {
                //intDouble = 0;
                // Start with a non-doubling
                intCheckSum = 0;
                // Start with 0 intCheckSum
                strCC = cardNumber.Trim();
                // Trim extra blanks
                functionReturnValue = false;
                // Assume invalid card

                // Working backwards
                for (intIndex = strCC.Length - 1; intIndex >= 0; intIndex--)
                {
                    byteDigit = System.Text.Encoding.ASCII.GetBytes(strCC.Substring(intIndex, 1));
                    intDigit = Convert.ToInt32(byteDigit[0]);
                    // Isolate character
                    // Skip if not a intDigit
                    if (intDigit > 47 & intDigit < 58)
                    {
                        intDigit = intDigit - 48;
                        // Remove ASCII bias
                        // If in the "double-add" phase
                        if (intDouble)
                        {
                            intDigit = intDigit + intDigit;
                            // then double first
                            if (intDigit > 9)
                            {
                                intDigit = intDigit - 9;
                                // Cast nines
                            }
                        }
                        intDouble = !intDouble;
                        // Flip doubling flag
                        intCheckSum = intCheckSum + intDigit;
                        // Add to running sum
                        // Cast tens
                        if (intCheckSum > 9)
                        {
                            intCheckSum = intCheckSum - 10;
                            // (same as MOD 10 but faster)
                        }
                    }
                }
                // check to see the card number is 16 digits exactly
                Regex r = new Regex(@"\d{16}");
                Match m = r.Match(cardNumber);

                int startLast4Index = cardNumber.Length - 4;
                string last4 = string.Empty;
                if (startLast4Index >= 0)
                {
                    last4 = cardNumber.Substring(startLast4Index, 4);
                }
                if (m.Success && intCheckSum == 0)
                {

                    WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 2100, string.Format("LuhnModValidation.ValidateCard Card Last 4 = {0} Valid", last4));
                    functionReturnValue = true;
                }
                else
                {
                    WebServiceHostFactory._trace.TraceEvent(TraceEventType.Error, 2300, string.Format("LuhnModValidation.ValidateCard Card Last 4 = {0} Failure", last4));
                    functionReturnValue = false;
                }
            }
            catch (Exception ex)
            {
                WebServiceHostFactory._trace.TraceEvent(TraceEventType.Error, 2500, string.Format("LuhnModValidation.ValidateCard Error {0}", ex.Message));
            }
            finally
            {
                WebServiceHostFactory._trace.TraceEvent(TraceEventType.Stop, 2999, "LuhnModValidation.ValidateCard Complete");
                Trace.CorrelationManager.StopLogicalOperation();

            }
            if (WebServiceHostFactory.cardTrace == 1)
            {
                WebServiceHostFactory._cardTrace.TraceEvent(TraceEventType.Information, 205550, string.Format("CardTrace: Number: {0} Valid: {1}", cardNumber, functionReturnValue));
                WebServiceHostFactory._cardTrace.TraceEvent(TraceEventType.Stop, 299999, "Stop Card Trace For luhn validation");
            }
            return functionReturnValue;
        }
    }
        #endregion
}
        

