using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_App
{
    public interface ICustom_Item
    {
        string Type { get; set; }

        string Description { get; set; }

        string Value_Data { get; set; }

        string Value_Type { get; set; }

    }
}
