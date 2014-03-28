using System;
using System.Text.RegularExpressions;

namespace AtsAPCC.DataAnnotations
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    sealed public class PaymentCardNumberAttribute : RegexAttribute
    {
        public const string Pattern = @"^\d{16}$";
        public const string FailedErrorMessage = "Card Number must be 16 digits";
        public PaymentCardNumberAttribute()
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