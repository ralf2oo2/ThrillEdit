using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ThrillEdit.BusinessLayer.Models;

namespace ThrillEdit.ApplicationLayer.Converters
{
    public class CommandParameterMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            VorbisData vorbisData = (VorbisData) values[0];
            int index = (int)values[1];
            Tuple <VorbisData, int> tuple = new Tuple<VorbisData, int>(vorbisData, index);
            return tuple;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
