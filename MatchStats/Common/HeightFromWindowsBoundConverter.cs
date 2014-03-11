using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MatchStats.Common
{
    public class HeightFromWindowsBoundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var result =  Window.Current.Bounds.Height;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}