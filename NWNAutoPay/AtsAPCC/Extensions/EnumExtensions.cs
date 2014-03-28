using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;

namespace AtsAPCC.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription<T>(this T enumerationValue)
            where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name 
            //for the enum 
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value 
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum 
            return enumerationValue.ToString();
        }

        public static T EnumFromDescription<T>(this String stringValue, T defaultValue)
        {
            foreach (T enumValue in Enum.GetValues(typeof(T)))
            {
                var descriptionAttributes =
                    (DescriptionAttribute[])
                    (typeof(T).GetField(enumValue.ToString()))
                        .GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (descriptionAttributes.Length > 0 && descriptionAttributes[0].Description == stringValue)
                    return enumValue;
            }
            return defaultValue;
        }


        public static T EnumFromString<T>(this String stringValue, T defaultValue)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), stringValue);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static T EnumFromString<T>(this String stringValue, Dictionary<String, T> mapping, T defaultValue)
        {
            try
            {
                if (mapping.ContainsKey(stringValue))
                    return mapping[stringValue];
                return defaultValue;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
    }
}