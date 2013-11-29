using System;
using System.ComponentModel.DataAnnotations;
using Windows.UI.Xaml.Data;

namespace MatchStats.Common
{
    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is Enum)) return null;
            return ((Enum) value).GetAttribute<DisplayAttribute>().Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}