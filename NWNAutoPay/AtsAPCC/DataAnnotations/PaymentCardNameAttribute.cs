using System;
using System.Text.RegularExpressions;

namespace AtsAPCC.DataAnnotations
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    sealed public class PaymentCardNameAttribute : RegexAttribute
    {
        public const string Pattern = @"^[a-zA-Z\.\-\' ]{0,40}$";
        public const string FailedErrorMessage = "Name on Card contains invalid characters";

        public PaymentCardNameAttribute()
            : base(Pattern, RegexOptions.IgnoreCase)
        { }

        public override string FormatErrorMessage(string name)
        {
            return FailedErrorMessage;
        }

        public override bool IsValid(object value)
        {
            return string.IsNullOrWhiteSpace(value as string) || base.IsValid(value);
        }
    }
}