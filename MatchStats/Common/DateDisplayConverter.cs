using System;
using Windows.UI.Xaml.Data;

namespace MatchStats.Common
{
    public class DateDisplayConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is DateTime)) return null;
            return ((DateTime)value).ToString("dd-MMM-yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}