using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace MoodleBot.Common
{
    public static class MoodleBotCommon
    {
        public static IConfigurationRoot Configuration { get; }
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value?.Trim());
        }

        public static bool IsNotNullOrEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value?.Trim());
        }

        public static string GetWhatsNumber(this string value)
        {
            return Regex.Replace(value, @"[^0-9]+", "");
        }

        public static string Description(this Enum value)
        {
            // get attributes  
            var field = value.GetType().GetField(value.ToString());
            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

            // return description
            return attributes.Any() ? ((DescriptionAttribute)attributes.ElementAt(0)).Description : "Description Not Found";
        }

        public static TEnum GetEnumFromValue<TEnum>(this string value) where TEnum : Enum
        {
            return (TEnum)Enum.Parse(typeof(TEnum), value);
        }

        public static bool IsEnumContainsValue<TEnum>(this string value) where TEnum : Enum
        {
            return Enum.IsDefined(typeof(TEnum), value);
        }

        public static bool IsEnumContainsValue<TEnum>(this int value) where TEnum : Enum
        {
            return Enum.IsDefined(typeof(TEnum), value);
        }

        public static TEnum GetEnumFromDescription<TEnum>(this string description) where TEnum : Enum
        {
            foreach (var field in typeof(TEnum).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (TEnum)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (TEnum)field.GetValue(null);
                }
            }

            throw new ArgumentException("Not found.", nameof(description));
        }

        public static long GetTimeStampFromDateTime(this DateTime dateTime)
        {
            return (Int32)(DateTime.Now.Subtract(dateTime)).TotalSeconds;
        }

        public static long GetTimeStampFromDateTime(this string dateTime)
        {
            var dateTimePart = dateTime.Split('-');
            return GetTimeStampFromDateTime(new DateTime(Convert.ToInt32(dateTimePart[0]), Convert.ToInt32(dateTimePart[1]), Convert.ToInt32(dateTimePart[2])));
        }
    }
}
