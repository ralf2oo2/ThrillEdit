using ByteSizeLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ThrillEdit.ApplicationLayer.Converters
{
    public class MegaByteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ByteSize size = ByteSize.FromBytes((long)value);
            return $"{Math.Round(size.MegaBytes, 1)}{ByteSize.MegaByteSymbol}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
