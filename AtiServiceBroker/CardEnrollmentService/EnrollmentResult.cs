using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace CardEnrollmentService
{
    [DataContract]
    public class EnrollmentResult
    {
        public EnrollmentResult()
        {
            this.FailureReason = string.Empty;
            this.Result = string.Empty;
            this.ResultStatus = ATIServiceBrokerStatusEnum.Other;
            this.transactionId = string.Empty;
        }
        public enum ATIServiceBrokerStatusEnum {
            [EnumMember]
            Success,
            [EnumMember]
            UpdateTokenFailed,
            [EnumMember]
            InvalidToken,
            [EnumMember]
            BadEnrollStatus,
            [EnumMember]
            BadTransactionData,
            [EnumMember]
            TempusBadStatus,
            [EnumMember]
            BadTempusCall,
            [EnumMember]
            BadAccountOrCard,
            [EnumMember]
            Other,
            [EnumMember]
            DeactivateCards
        }
       
        [DataMember]
        public ATIServiceBrokerStatusEnum ResultStatus;
        [DataMember]
        public string FailureReason;
        [DataMember]
        public string Result;
        [DataMember]
        public string transactionId;

        public enum CardEnrollmentStatus
        {
            [EnumMember]
            InActive,
            [EnumMember]
            Active,
            [EnumMember]
            Confirmed
        }
    }
}

