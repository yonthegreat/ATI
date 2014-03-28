using System;

using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using Ati.ServiceHost;
using System.Diagnostics;
using Ati.ServiceHost.Web;
using System.Xml.Linq;


namespace CardEnrollmentService
{
    
    [ExportService("TempusEnrollCardService", typeof(TempusEnrollCardService))]
    public class TempusEnrollCardService : IEnrollCardService
    {
        


        protected string TestConnectionString(string connString)
        {
            using (var conn = new SqlConnection(connString))
            {
                
                try
                {

                    conn.Open();
                    if (conn.ServerVersion == null || conn.ServerVersion == string.Empty)
                    {
                        throw new Exception("Bad Database connection string");
                    }
                    
                }
                catch (Exception ce)
                {
                    WebServiceHostFactory._trace.TraceEvent(TraceEventType.Critical, 101, string.Format("Error Database connection: {0} Exception: {1}", WebServiceHostFactory.dBServer, ce.Message));
                    throw new Exception("Bad ConnectionString");
                }
                return string.Empty;
            }
        }

        #region methods
        /// <summary>
        /// This method is the exposed interface for the Tempus enrollment Service. It is used by other services and websites to enroll a card in the Tempus system
        /// </summary>
        /// <param name="CustomerName"></param>
        /// <param name="AccountAccountNumber"></param>
        /// <param name="CardFullNumber"></param>
        /// <param name="CardLast4"></param>
        /// <param name="CardExpiresMonth"></param>
        /// <param name="CardExpriresYear"></param>
        /// <param name="CardSecurityCode"></param>
        /// <param name="CardHolderFirstName"></param>
        /// <param name="CardHolderLastName"></param>
        /// <param name="CardHolderInitial"></param>
        /// <param name="CardHolderAppearsOnCard"></param>
        /// <param name="AddressAddress1"></param>
        /// <param name="AddressAddress2"></param>
        /// <param name="AddressAddress3"></param>
        /// <param name="AddressCity"></param>
        /// <param name="AddressState"></param>
        /// <param name="AddressZip"></param>
        /// <param name="ContactTelephone"></param>
        /// <param name="ContactEmail"></param>
        /// <returns></returns>
        public EnrollmentResult EnrollCard(string CustomerName, string AccountAccountNumber, string CardFullNumber, string CardLast4,
                            int CardExpiresMonth, int CardExpriresYear, string CardSecurityCode, string CardHolderFirstName,
                            string CardHolderLastName, string CardHolderInitial, string CardHolderAppearsOnCard, string AddressAddress1,
                            string AddressAddress2, string AddressAddress3, string AddressCity, string AddressState, string AddressZip,
                            string ContactTelephone, string ContactEmail)
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("TempusEnrollCardService");
            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Start, 3000, "Start TempusEnrollCardService.EnrollCard");
            
            int accountId;
            EnrollmentResult result = new EnrollmentResult();
            
            string connString;
            try
            {
                connString = ConfigurationManager.ConnectionStrings["CardEnrollmentService.Properties.Settings.ServiceBrokerDBConnectionString"].ConnectionString;
            }
            catch (Exception e2)
            {
                WebServiceHostFactory._trace.TraceEvent(TraceEventType.Error, 3100, string.Format("TempusEnrollCardService.EnrollCard: database connection string not found: {0}", e2.Message));
                result.ResultStatus = EnrollmentResult.ATIServiceBrokerStatusEnum.Other;
                result.FailureReason = "database connection string not found: " + e2.Message;
                return result;
            }

            // check to see if the database server is alive
            TestConnectionString(connString);


            int intCardId = 0;
            Guid cardId = Guid.NewGuid(); 
            string validGUID = String.Empty;
            int count = 0;
            while (validGUID == String.Empty)
            {
                validGUID = EncodeGUIDToTempusFormat(cardId);
                if (validGUID == string.Empty)
                {
                    cardId = Guid.NewGuid(); 
                    WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 3160, string.Format("TempusGuid: {0} Count{1}", cardId, count));
                    count++;
                }
            }
            String tempusCardId = validGUID;
            
            EnrollmentResult tempusResult = new EnrollmentResult();
            int transactionId = 0;
            string token = string.Empty;
            bool enrollStatus = false;
            string transactionStep = String.Empty;
            int cardActivated;
            try
            {
                var cardActiveReturn = ConfigurationManager.AppSettings["CardActiveValue"];
                cardActivated = Int32.Parse(cardActiveReturn.ToString());
            }
            catch (Exception e)
            {
                WebServiceHostFactory._trace.TraceEvent(TraceEventType.Error, 3200, string.Format("TempusEnrollCardService.EnrollCard: Card Active Value not set: {0}", e.Message));
                result.FailureReason = "Card Active Value not set: " + e.Message;
                result.ResultStatus = EnrollmentResult.ATIServiceBrokerStatusEnum.Other;
                return result;
            }
            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 3250, string.Format("TempusEnrollCardService.EnrollCard: Card Active Value: {0}", cardActivated.ToString()));
           

            using (var conn = new SqlConnection(connString))
            {
                using (TransactionScope beforeTempusScope = new TransactionScope())
                {
                    try
                    {

                        conn.Open();
                        #region AddAccount
                        using (var accountCommand = new SqlCommand("dbo.FindOrCreateAccount", conn))
                        {
                            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 3300, string.Format("TempusEnrollCardService.EnrollCard: FindOrCreateAccount: {0} CustomerName {1}", AccountAccountNumber, CustomerName));
                            
                            accountCommand.CommandType = CommandType.StoredProcedure;
                            accountCommand.Parameters.Add(new SqlParameter("@AccountNumber", SqlDbType.NVarChar, 50)).Value = AccountAccountNumber;
                            accountCommand.Parameters.Add(new SqlParameter("@CustomerName", SqlDbType.NVarChar, 50)).Value = CustomerName;
                            
                            transactionStep = "Account: " + AccountAccountNumber + " Name: " + CustomerName + " Cmd: " + accountCommand.ToString();
                            var a = accountCommand.ExecuteScalar();
                            accountId = Int32.Parse(a.ToString());
                            
                            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 3302, string.Format("TempusEnrollCardService.EnrollCard: FindOrCreateAccount: {0}", AccountAccountNumber));
                           
                        }
                        
                        #endregion
                       
                        #region AddCard
                        using (var cardCommand = new SqlCommand("dbo.FindOrCreateCard", conn))
                        {

                            try
                            {
                                int expMonth = Int32.Parse(CardExpiresMonth.ToString());
                                int expYear = Int32.Parse(CardExpriresYear.ToString());
                            }
                            catch
                            {
                                WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 3310, string.Format("TempusEnrollCardService.EnrollCard: FindOrCreateCard: Bad numeric value ExpMonth: {0} ExpYear: {1}", CardExpiresMonth, CardExpriresYear));
                                result.ResultStatus = EnrollmentResult.ATIServiceBrokerStatusEnum.Other;
                                return result;
                            }
                            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 3320, string.Format("TempusEnrollCardService.EnrollCard: FindOrCreateCard: Guid:{0} Last4:{1} TempusId {2}", cardId, CardLast4, tempusCardId));
                            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 3325, string.Format("TempusEnrollCardService.EnrollCard: FindOrCreateCard:Acct: {0} Last4:{1} ExpMonth: {2} ExpYear: {3} Sec: {4} Zip: {5} FN: {6} LN: {7} Name: {8} Ph: {9} EM: {10}",
                                accountId, CardLast4, CardExpiresMonth, CardExpriresYear, CardSecurityCode, AddressZip, CardHolderFirstName, CardHolderLastName, CardHolderAppearsOnCard, ContactTelephone, ContactEmail));
                            cardCommand.CommandType = CommandType.StoredProcedure;
                            cardCommand.Parameters.Add(new SqlParameter("@AccountId", SqlDbType.NVarChar));
                            cardCommand.Parameters.Add(new SqlParameter("@Last4", SqlDbType.NVarChar));
                            cardCommand.Parameters.Add(new SqlParameter("@ExpMonth", SqlDbType.Int));
                            cardCommand.Parameters.Add(new SqlParameter("@ExpYear", SqlDbType.Int));
                            cardCommand.Parameters.Add(new SqlParameter("@BillingZip", SqlDbType.NVarChar));
                            cardCommand.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.NVarChar));
                            cardCommand.Parameters.Add(new SqlParameter("@LastName", SqlDbType.NVarChar));
                            cardCommand.Parameters.Add(new SqlParameter("@NameOnCard", SqlDbType.NVarChar));
                            cardCommand.Parameters.Add(new SqlParameter("@Telephone", SqlDbType.NVarChar));
                            cardCommand.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar));
                            cardCommand.Parameters["@AccountId"].Value = accountId;
                            cardCommand.Parameters["@Last4"].Value = CardLast4;
                            cardCommand.Parameters["@ExpMonth"].Value = CardExpiresMonth;
                            cardCommand.Parameters["@ExpYear"].Value = CardExpriresYear;
                            cardCommand.Parameters["@FirstName"].Value = CardHolderFirstName;
                            cardCommand.Parameters["@LastName"].Value = CardHolderLastName;
                            cardCommand.Parameters["@NameOnCard"].Value = CardHolderAppearsOnCard;
                            cardCommand.Parameters["@BillingZip"].Value = AddressZip;
                            cardCommand.Parameters["@Telephone"].Value = ContactTelephone;
                            cardCommand.Parameters["@Email"].Value = ContactEmail;


                            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 3326, string.Format("TempusEnrollCardService.EnrollCard: FindOrCreateCard:Acct: {0} Last4:{1} ExpMonth: {2} ExpYear: {3} Sec: {4} Zip: {5} FN: {6} LN: {7} Name: {8} Ph: {9} EM: {10}",
                                accountId, CardLast4, CardExpiresMonth, CardExpriresYear, CardSecurityCode, AddressZip, CardHolderFirstName, CardHolderLastName, CardHolderAppearsOnCard, ContactTelephone, ContactEmail));
                            cardCommand.CommandType = CommandType.StoredProcedure;
                            var z = cardCommand.ExecuteScalar();

                            intCardId = Convert.ToInt32(z.ToString());
                            
                            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 3350, string.Format("TempusEnrollCardService.EnrollCard: FindOrCreateCard: {0}", CardLast4));
                        }
                        #endregion
                    }
                    catch (Exception sqlEx)
                    {
                        WebServiceHostFactory._trace.TraceEvent(TraceEventType.Error, 3390, string.Format("TempusEnrollCardService.EnrollCard: FINDCARDACCOUNT: {0}", sqlEx.Message));
                        result.ResultStatus = EnrollmentResult.ATIServiceBrokerStatusEnum.BadAccountOrCard;
                        result.FailureReason = string.Format("Transaction Step: {0} Error: {1}", transactionStep, sqlEx.Message);
                        return result;
                    }
                    
                    
                    try
                    {
                        transactionStep = "Tempus Enroll";
                        TempusEnroll te = new TempusEnroll();
 
                        tempusResult = te.TempusEnrollment(CardFullNumber, CardExpiresMonth, CardExpriresYear, AddressZip, tempusCardId);
                       
                        WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 3400, string.Format("TempusEnrollCardService.EnrollCard: Tempus Enroll: {0}", cardId));
                    }
                    catch (Exception teEx)
                    {
                        WebServiceHostFactory._trace.TraceEvent(TraceEventType.Error, 3450, string.Format("TempusEnrollCardService.EnrollCard: Tempus call failed: {0}", teEx.Message));
                        result.ResultStatus = EnrollmentResult.ATIServiceBrokerStatusEnum.BadTempusCall;
                        result.FailureReason = string.Format("Transaction Step: {0} Error: {1}", transactionStep, teEx.Message);
                        
                        return result;
                    }
                    if (tempusResult.ResultStatus != EnrollmentResult.ATIServiceBrokerStatusEnum.Success)
                    {
                        WebServiceHostFactory._trace.TraceEvent(TraceEventType.Warning, 3470, string.Format("TempusEnrollCardService.EnrollCard: Tempus Enroll Failure: {0}", tempusResult.FailureReason));
                        result.ResultStatus = EnrollmentResult.ATIServiceBrokerStatusEnum.TempusBadStatus;
                        result.FailureReason = string.Format("Transaction Step: {0} Status: {1} Error: {2}", transactionStep, tempusResult.ResultStatus.ToString(), tempusResult.FailureReason);
                        
                        return result;
                    }
                    using (TransactionScope afterTempusScope = new TransactionScope())
                    {
                        try
                        {
                            using (var transactionCommand = new SqlCommand("dbo.CreateTempusXmlCardTransaction", conn))
                            {
                                WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 3500, string.Format("TempusEnrollCardService.EnrollCard: CreateTempusXmlCardTransaction: {0}", cardId));
                                transactionStep = "Enrollment Transaction";
                                transactionCommand.CommandType = CommandType.StoredProcedure;
                                transactionCommand.Parameters.Add(new SqlParameter("@CardId", SqlDbType.Int));
                                transactionCommand.Parameters.Add(new SqlParameter("@XmlData", SqlDbType.Xml));

                                transactionCommand.Parameters["@CardId"].Value = intCardId;
                                transactionCommand.Parameters["@XmlData"].Value = tempusResult.Result;

                                var transIdReturn = transactionCommand.ExecuteScalar();
                                tempusResult.transactionId = transIdReturn.ToString();
                                WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 3520, string.Format("TempusEnrollCardService.EnrollCard: CreateTempusXmlCardTransaction: {0}", transactionId.ToString()));
                                WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 3530, string.Format("TempusEnrollCardService.EnrollCard: CreateTempusXmlCardTransaction: {0}", tempusResult.Result));
                            }


                        }
                        catch (Exception sqlEx)
                        {
                            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Error, 3550, string.Format("TempusEnrollCardService.EnrollCard: Tempus Data Bad: {0}", sqlEx.Message));
                            result.ResultStatus = EnrollmentResult.ATIServiceBrokerStatusEnum.BadTransactionData;
                            result.Result = string.Format("Transaction Step: {0} Error: {1}", transactionStep, sqlEx.Message);
                            return result;
                        }

                        afterTempusScope.Complete();
                    }
                    beforeTempusScope.Complete();
                }

                #region updateCardScope
                using (TransactionScope updateCardScope = new TransactionScope())
                {
                    try
                    {
                        transactionStep = "Enroll Status";
                        WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 3600, string.Format("TempusEnrollCardService.EnrollCard: GetTempusResulttoXmlDocument: {0}", transactionId.ToString()));
                        XDocument doc = XDocument.Parse(tempusResult.Result);
                      
                        if (WebServiceHostFactory.appSettings["EnrollmentMode"] == "Production")
                        {
                            var tempusProctutionTokenValue = doc.Root.Elements(WebServiceHostFactory.appSettings["TempusEnrollTokens"]).FirstOrDefault().Value;
                            if (String.IsNullOrEmpty(tempusProctutionTokenValue.ToString()))
                            {
                                
                                WebServiceHostFactory._trace.TraceEvent(TraceEventType.Error, 3610, string.Format("TempusEnrollCardService.EnrollCard: GetTempusProductionToken: Empty"));
                                result.ResultStatus = EnrollmentResult.ATIServiceBrokerStatusEnum.BadEnrollStatus;
                                result.FailureReason = "Enrollment Status Failed";
                                return result;
                            }
                        }
                        var tempusStatus = doc.Root.Elements("TRANSUCCESS").FirstOrDefault().Value;
                        if (String.IsNullOrEmpty(tempusStatus.ToString()))
                        {
                            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 3620, string.Format("TempusEnrollCardService.EnrollCard: GetTempusEnrollStatus: Empty"));
                        }
                        else 
                        {
                            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 3620, string.Format("TempusEnrollCardService.EnrollCard: GetTempusEnrollStatus: {0}", tempusStatus.ToString()));
                            enrollStatus = bool.Parse(tempusStatus.ToString().ToLower());
                            if (!enrollStatus)
                            {
                                WebServiceHostFactory._trace.TraceEvent(TraceEventType.Error, 3650, string.Format("TempusEnrollCardService.EnrollCard: Enrollment Status Failed: {0}", tempusStatus.ToString()));
                                result.ResultStatus = EnrollmentResult.ATIServiceBrokerStatusEnum.BadEnrollStatus;
                                result.FailureReason = "Enrollment Status Failed";
                                return result;
                            }
                        }

                        WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 3700, string.Format("TempusEnrollCardService.EnrollCard: TransactionId: {0}", transactionId));
                        transactionStep = "Enroll Token";
                        var tempusToken = doc.Root.Elements("REPTOKEN").FirstOrDefault().Value;
                        if (String.IsNullOrEmpty(tempusStatus.ToString()))
                        {
                            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 3720, string.Format("TempusEnrollCardService.EnrollCard: GetTempusEnrollToken: Empty"));
                            result.ResultStatus = EnrollmentResult.ATIServiceBrokerStatusEnum.InvalidToken;
                            result.FailureReason = "Enrollment Token Invalid";
                            return result;
                        }
                        else
                        {
                            token = tempusToken.ToString();
                            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 3730, string.Format("TempusEnrollCardService.EnrollCard: GetTempusEnrollToken: {0}", tempusToken.ToString()));
                            // TODO: Need to get info from Tempus on how to validate tokens

                        }
                        using (var updateCardCommand = new SqlCommand("dbo.UpdateCardTokenAndActivate", conn))
                        {
                            transactionStep = "Update Token";
                            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 3800, string.Format("TempusEnrollCardService.EnrollCard: UpdateCardTokenAndActivate: {0}", intCardId));
                            updateCardCommand.CommandType = CommandType.StoredProcedure;
                            updateCardCommand.Parameters.Add(new SqlParameter("@CardId", SqlDbType.Int));
                            updateCardCommand.Parameters.Add(new SqlParameter("@Token", SqlDbType.NVarChar));
                            updateCardCommand.Parameters.Add(new SqlParameter("@Activate", SqlDbType.Int));
                            updateCardCommand.Parameters["@CardId"].Value = intCardId;
                            updateCardCommand.Parameters["@Token"].Value = token;
                            updateCardCommand.Parameters["@Activate"].Value = cardActivated;
                            var updateReturn = updateCardCommand.ExecuteNonQuery();
                            int rowsUpdated = Int32.Parse(updateReturn.ToString());
                            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 3810, string.Format("TempusEnrollCardService.EnrollCard: UpdateCardTokenAndActivate: {0} Token: {1} Act: {2}", cardId, token, cardActivated));
                            if (rowsUpdated != 1)
                            {
                                WebServiceHostFactory._trace.TraceEvent(TraceEventType.Error, 3850, string.Format("TempusEnrollCardService.EnrollCard: Card Not Active: {0}", cardId));
                                result.ResultStatus = EnrollmentResult.ATIServiceBrokerStatusEnum.UpdateTokenFailed;
                                result.FailureReason = "Card not activated";
                                return result;
                            }

                            else
                            {
                                updateCardScope.Complete();
                            }
                        }
                    }

                    
                    catch (Exception sqlEx)
                    {
                        WebServiceHostFactory._trace.TraceEvent(TraceEventType.Error, 3995, string.Format("TempusEnrollCardService.EnrollCard: Card Transaction and Activation: {0}", cardId));
                        result.ResultStatus = EnrollmentResult.ATIServiceBrokerStatusEnum.Other;
                        result.Result = string.Format("Transaction Step: {0} Error: {1}", transactionStep, sqlEx.Message);
                        return result;
                    }
                    
                }
                #endregion

                conn.Close();
            }
            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Stop, 3999, "TempusEnrollCardService.EnrollCard Complete");
            Trace.CorrelationManager.StopLogicalOperation();
            return tempusResult;
            
        }
        #endregion

        //this section of code is stubbed out until we need to deactivate cards
        #region FutureCard confirmation and deactivation
        //public EnrollmentResult ConfirmCardEnrollment(string AccountAccountNumber, string CardLast4, int CardExpiresMonth, int CardExpriresYear)
        //{
            
        //    EnrollmentResult result = new EnrollmentResult();

        //    //string connString;
        //    //try
        //    //{
        //    //    connString = ConfigurationManager.ConnectionStrings["CardEnrollmentService.Properties.Settings.ServiceBrokerDBConnectionString"].ConnectionString;
        //    //}
        //    //catch (Exception e2)
        //    //{
        //    //    WebServiceHostFactory._trace.TraceEvent(TraceEventType.Error, 30100, string.Format("TempusEnrollCardService.ConfirmCardEnrollment: database connection string not found: {0}", e2.Message));
        //    //    result.ResultStatus = EnrollmentResult.StatusEnum.Other;
        //    //    result.FailureReason = "database connection string not found: " + e2.Message;
        //    //    return result;
        //    //}
        //    //WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 30150, string.Format("TempusEnrollCardService.ConfirmCardEnrollment: database connection string: {0}", connString));

        //    //using (var conn = new SqlConnection(connString))
        //    //{
        //    //    try
        //    //    {
        //    //        conn.Open();
        //    //        using (var confirmedCardsCommand = new SqlCommand("dbo.FindConfirmedCardsForAccount", conn))
        //    //        {
        //    //            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 30300, string.Format("TempusEnrollCardService.ConfirmCardEnrollment: FindConfirmedCardsForAccount: {0}", AccountAccountNumber));

        //    //            confirmedCardsCommand.CommandType = CommandType.StoredProcedure;
        //    //            confirmedCardsCommand.Parameters.Add(new SqlParameter("@AccountNumber", SqlDbType.NVarChar, 50)).Value = AccountAccountNumber;
                        
        //    //            SqlDataReader reader = confirmedCardsCommand.ExecuteReader();
        //    //            while (reader.Read())
        //    //            {
        //    //                try
        //    //                {
        //    //                    Guid cardId = (Guid)reader[0];
        //    //                    WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 30310, string.Format("TempusEnrollCardService.ConfirmCardEnrollment: FindConfirmedCardsForAccount {0} CardId: {1}", AccountAccountNumber, cardId));

        //    //                    using (var updateCardStatusCommand = new SqlCommand("dbo.UpdateCardStatusByUniqueId", conn))
        //    //                    {
        //    //                        WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 30320, string.Format("TempusEnrollCardService.ConfirmCardEnrollment: UpdateCardStatusByUniqueId CardId: {1}", cardId));
        //    //                        updateCardStatusCommand.CommandType = CommandType.StoredProcedure;
        //    //                        updateCardStatusCommand.Parameters.Add(new SqlParameter(@CardUniqueId, SqlDbType.UniqueIdentifier)).Value = cardId;
        //    //                        updateCardStatusCommand.Parameters.Add(new SqlParameter(@updateCardStatusCommand, SqlDbType.Int)).Value = EnrollmentResult.CardEnrollmentStatus.InActive;
        //    //                        var updateReturn = updateCardStatusCommand.ExecuteNonQuery();
                                    
        //    //                        int rowsUpdated = Int32.Parse(updateReturn.ToString());
        //    //                        WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 30330, string.Format("TempusEnrollCardService.ConfirmCardEnrollment: FindConfirmedCardsForAccount CardId: {0} rowCount: {1}", cardId, rowsUpdated));
        //    //                    }
        //    //                }
        //    //                catch
        //    //                {

        //    //                }
        //    //            }
                        

        //    //            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 30302, string.Format("TempusEnrollCardService.ConfirmCardEnrollment: FindOrCreateAccount: {0}", AccountAccountNumber));

        //    //        }
        //    //    }
        //    //    catch
        //    //    {

        //    //    }
        //    //    finally
        //    //    {
        //    //        conn.Close();
        //    //    }
        //    //}
        //    //WebServiceHostFactory._trace.TraceEvent(TraceEventType.Stop, 30999, "TempusEnrollCardService.ConfirmCardEnrollment Complete");
        //    //Trace.CorrelationManager.StopLogicalOperation();
        //    return result;
            

        //}

        //public EnrollmentResult DeactivateCardEnrollment(string AccountAccountNumber, string CardLast4, int CardExpiresMonth, int CardExpriresYear)
        //{
        //    Trace.CorrelationManager.ActivityId = Guid.NewGuid();
        //    Trace.CorrelationManager.StartLogicalOperation("DeactivateCardEnrollment");
        //    WebServiceHostFactory._trace.TraceEvent(TraceEventType.Start, 40000, "Start TempusEnrollCardService.DeactivateCardEnrollment");

        //    int accountId;
        //    EnrollmentResult result = new EnrollmentResult();

        //    WebServiceHostFactory._trace.TraceEvent(TraceEventType.Stop, 40999, "TempusEnrollCardService.ConfirmCardEnrollment Complete");
        //    Trace.CorrelationManager.StopLogicalOperation();
        //    return result;
        //}
        #endregion Future

        /// <summary>
        /// This method allows us to transform a 32 character Guid into a 22 character string to fit in the tempus CUSTIDENT field.
        /// Additionally characters that Tempus doe not allow are transformed.
        /// </summary>
        /// <param name="g">our Guid card token</param>
        /// <returns></returns>
        private string EncodeGUIDToTempusFormat(Guid g)
        {
            string b64 = Convert.ToBase64String(g.ToByteArray()).Substring(0, 22);
            bool isValid = true;
            foreach (byte b in b64)
            {
                if ( b >= 48 && b <= 57)
                {
                    //0-9
                    continue;
                }
                if (b >= 65 && b <= 90)
                {
                    //A-Z
                    continue;
                }
                if ( b >= 97 && b <= 122)
                {
                    //a-z
                    continue;
                }
                if ( b == 46 || b == 45 || b == 95 || b == 58 || b ==47 || b == 38 )
                {
                    //.-_:/&
                    continue;
                }
                else
                {
                    isValid = false;
                    break;
                }
            }

            if ( !isValid )
                return string.Empty;
            else
                return b64;
        }
    }
}
