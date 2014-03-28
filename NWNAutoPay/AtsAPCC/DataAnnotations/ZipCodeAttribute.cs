using System;
using System.Text.RegularExpressions;

namespace AtsAPCC.DataAnnotations
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    sealed public class ZipCodeAttribute : RegexAttribute
    {
        public const string Pattern = @"^\d{5}$";
        public const string FailedErrorMessage = "Card Billing ZIP Code must be 5 digits, such as 97209";
        public ZipCodeAttribute()
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