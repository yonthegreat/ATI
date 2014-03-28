using System;
using System.Text.RegularExpressions;

namespace AtsAPCC.DataAnnotations
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    sealed public class PaymentCardSecurityCodeAttribute : RegexAttribute
    {
        public const string Pattern = @"^\d{3}$";
        public const string FailedErrorMessage = "Security Code must be 3 digits";
        public PaymentCardSecurityCodeAttribute()
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