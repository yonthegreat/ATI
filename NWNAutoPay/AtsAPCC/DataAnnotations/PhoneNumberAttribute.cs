using System;
using System.Text.RegularExpressions;

namespace AtsAPCC.DataAnnotations
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    sealed public class PhoneNumberAttribute : RegexAttribute
    {
        public const string Pattern = @"^\d{10}$";
        public const string FailedErrorMessage = "Phone Number must be 10 digits, such as 5035551212";

        public PhoneNumberAttribute()
            : base(Pattern, RegexOptions.IgnoreCase)
        { }

        public override string FormatErrorMessage(string name)
        {
            return FailedErrorMessage;
        }

        public override bool IsValid(object value)
        {
            if (!string.IsNullOrWhiteSpace(value as string))
            {
                return base.IsValid(value);
            }
            else
            {
                return true;
            }
        }
    }
}