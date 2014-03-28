//            //need to do the commit prior to the card deactivation
                    //            updateCardScope.Complete();
                    //            try
                    //            {
                    //                using (var deactivateCards = new SqlCommand("dbo.UpdateCardByAccountToInactive", conn))
                    //                {
                    //                    transactionStep = "Update Token";

                    //                    WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 3860, string.Format("TempusEnrollCardService.EnrollCard: DeactivateCardsForAccount: {0}", AccountAccountNumber));
                    //                    deactivateCards.CommandType = CommandType.StoredProcedure;
                    //                    deactivateCards.Parameters.Add(new SqlParameter("@AccountNumber", SqlDbType.Int));
                    //                    deactivateCards.Parameters["@AccountNumber"].Value = AccountAccountNumber;
                    //                    var deactivateCardCount = deactivateCards.ExecuteNonQuery();
                    //                    int cardsDeactivated = Int32.Parse(deactivateCards.ToString());
                    //                    WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 3870, string.Format("TempusEnrollCardService.EnrollCard: DeactivateCardsForAccount: {0} Cards Deactivated: {1}", AccountAccountNumber, cardsDeactivated));
                    //                }
                    //            }
                    //            catch (Exception dcEx)
                    //            {
                    //                WebServiceHostFactory._trace.TraceEvent(TraceEventType.Error, 3880, string.Format("TempusEnrollCardService.EnrollCard: DeactivateCardsForAccount: {0} Error: {1}", AccountAccountNumber, dcEx.Message));
                    //                result.ResultStatus = EnrollmentResult.ATIServiceBrokerStatusEnum.DeactivateCards;
                    //                result.FailureReason = string.Format("Transaction Step: {0} Error: {1}", transactionStep, dcEx.Message); 
                    //                return result;
                    //            }
                    //            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 3890, string.Format("TempusEnrollCardService.EnrollCard: UpdateCardTokenAndActivate Status: {0}", result.ResultStatus.ToString()));
                    //            result.ResultStatus = EnrollmentResult.ATIServiceBrokerStatusEnum.Success;
                    //            result.Result = transactionId.ToString();
                    //        }
                    //    }
                    //}