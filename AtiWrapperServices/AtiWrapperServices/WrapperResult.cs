using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace AtiWrapperServices
{
    /// <summary>
    /// AtiWrapperServices WrapperResult Class
    /// </summary>
    [DataContract]
    public class WrapperResult
    {
        /// <summary>
        /// Enumeration for result
        /// </summary>
        public enum ATIWrapperServiceStatusEnum {
            [EnumMember]
            Success, 
            [EnumMember]
            MissingPararmeter,
            [EnumMember]
            WrapperNotFound, 
            [EnumMember]
            Other 
        }
        /// <summary>
        /// Enumeration for wrapper return type
        /// </summary>
        public enum ReturnEncodingEnum { Xml = 0, Obj }

        /// <summary>
        /// AtiWrapperServices WrapperResult Fields
        /// </summary>
        [DataMember]
        public ATIWrapperServiceStatusEnum ResultStatus;
        [DataMember]
        public object FailureReason;
        [DataMember]
        public object Result;
    }
}