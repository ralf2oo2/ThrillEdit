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
    class ItemIndexMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var itemsControl = values[0] as ItemsControl;
            var item = values[1];
            var itemContainer = itemsControl.ItemContainerGenerator.ContainerFromItem(item);

            if (itemContainer == null)
            {
                return Binding.DoNothing;
            }

            var itemIndex = itemsControl.ItemContainerGenerator.IndexFromContainer(itemContainer);
            return itemIndex + 1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
