using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_App
{
    class Item
    {
        private string type;
        private string description;
        private string value_data;
        private string value_type;
        private string optionalInfo;

        Item(string type, string description, string value_data, string value_type, string optionalInfo)
        {
            this.type = type;
            this.description = description;
            this.value_data = value_data;
            this.value_type = value_type;
            this.optionalInfo = optionalInfo;
        }
        Item() { }
    }
}
