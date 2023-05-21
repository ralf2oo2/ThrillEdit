using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThrillEdit.BusinessLayer.Models;

namespace ThrillEdit.BusinessLayer.Providers
{
    public class ItemProvider
    {
        public List<Item> GetItems(string path, string[] allowedFileExtentions)
        {
            var items = new List<Item>();

            var dirInfo = new DirectoryInfo(path);

            foreach (var directory in dirInfo.GetDirectories())
            {
                if(GetItems(directory.FullName, allowedFileExtentions).Count() > 0)
                {
                    var item = new DirectoryItem
                    {
                        Name = directory.Name,
                        Path = directory.FullName,
                        Items = GetItems(directory.FullName, allowedFileExtentions)
                    };

                    items.Add(item);
                }
            }

            foreach (var file in dirInfo.GetFiles())
            {
                if (allowedFileExtentions.Contains(file.Name.Split(".").Last()))
                {
                    var item = new FileItem
                    {
                        Name = file.Name,
                        Path = file.FullName
                    };

                    items.Add(item);
                }
            }

            return items;
        }
    }
}
