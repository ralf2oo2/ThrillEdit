using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThrillEdit.BusinessLayer.Extentions
{
    public static class ObservableCollectionExtentions
    {
        public static T NextItem<T>(this ObservableCollection<T> collection, T currentItem)
        {
            var currentIndex = collection.IndexOf(currentItem);
            if (currentIndex < collection.Count - 1)
            {
                return collection[currentIndex + 1];
            }
            return collection[0];
        }
    }
}
