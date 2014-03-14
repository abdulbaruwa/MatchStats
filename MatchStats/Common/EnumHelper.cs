using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using MatchStats.Model;

namespace MatchStats.Common
{
    public static class EnumHelper
    {
        /// <summary>
        ///     Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        public static T GetAttribute<T>(this Enum enumValue) where T : Attribute
        {
            return enumValue
                .GetType()
                .GetTypeInfo()
                .GetDeclaredField(enumValue.ToString())
                .GetCustomAttribute<T>();
        }

        public static List<T> GetEnumAsList<T>()
        {
            Array enumArray = Enum.GetValues(typeof (T));
            var result = new List<T>();
            foreach (object item in enumArray)
            {
                result.Add((T) item);
            }
            return result;
        }

        public static IEnumerable<NameAndDescription> GetEnumKeyValuePairs<T>()
        {
            IEnumerable<Enum> enumArray = Enum.GetValues(typeof (T)).Cast<Enum>();
            return enumArray.Select(item =>
                new NameAndDescription
                {
                    Name = item.ToString(),
                    Description = item.GetAttribute<DisplayAttribute>().Name
                });
        }
    }
}