using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AtsAPCC.DataAnnotations
{
    /// <summary>
    /// This sealed abstract (must inherit) class allows for RegEx options to be passed in
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public abstract class RegexAttribute : ValidationAttribute
    {
        public string RegExPattern { get; set; }
        public RegexOptions Options { get; set; }

        protected RegexAttribute(string pattern, RegexOptions options = RegexOptions.None)
        {
            RegExPattern = pattern;
            Options = options;
        }

        public override bool IsValid(object value)
        {
            var stringValue = value as string;
            return !string.IsNullOrWhiteSpace(stringValue) && IsValid(stringValue);
        }

        private bool IsValid(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && new Regex(RegExPattern, Options).IsMatch(value);
        }
    }
}