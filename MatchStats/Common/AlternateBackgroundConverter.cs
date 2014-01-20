using System;
using Windows.UI;
using Windows.UI.Xaml.Data;

namespace MatchStats.Common
{
    public class AlternateBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is int)) return null;
            var index = (int)value;
            return index % 2 == 0 ? Colors.WhiteSmoke : Colors.LightBlue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}