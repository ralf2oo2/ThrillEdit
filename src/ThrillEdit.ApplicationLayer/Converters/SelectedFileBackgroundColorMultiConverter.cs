using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using ThrillEdit.BusinessLayer.Models;

namespace ThrillEdit.ApplicationLayer.Converters
{
    public class SelectedFileBackgroundColorMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] != null && values[0] != null && (string)values[0] == ((FileItem)values[1]).Path)
            {
                return (SolidColorBrush)new BrushConverter().ConvertFrom("#FF8F2C");
            }
            return new SolidColorBrush(Colors.LightGreen) { Opacity = 0 };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
