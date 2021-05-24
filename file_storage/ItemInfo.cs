using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace file_storage
{
    public class ItemInfo
    {
        public string Type { get; set; }
        public string Name { get; set; }

        public ItemInfo(string type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}
