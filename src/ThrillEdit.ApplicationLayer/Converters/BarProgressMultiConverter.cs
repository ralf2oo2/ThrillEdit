using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace ThrillEdit.ApplicationLayer.Converters
{
    internal class BarProgressMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan progress = (TimeSpan)values[0];
            TimeSpan Duration = (TimeSpan)values[1];
            return Divide(progress, Duration) * 10;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            double sliderValue = (double)value;
            TimeSpan duration = TimeSpan.FromTicks(long.Parse(((TextBlock)parameter).Text));
            return new object[] { Divide(sliderValue, duration), null };
        }

        public static double Divide(TimeSpan dividend, TimeSpan divisor)
        {
            return (double)dividend.Ticks / (double)divisor.Ticks;
        }

        public static TimeSpan Divide(double sliderValue, TimeSpan divisor)
        {
            return TimeSpan.FromTicks((long)((double)divisor.Ticks * (sliderValue / 10)));
        }
    }
}
