using System;
using Windows.UI.Xaml.Data;

namespace MatchStats.Common
{
    public class BooleanToWinLoseImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value is bool && (bool)value) ? "&#xE071;" : "&#xE071;";
            //return (value is bool && (bool) value) ? "&#xE170;" : "&#xE11D;";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}