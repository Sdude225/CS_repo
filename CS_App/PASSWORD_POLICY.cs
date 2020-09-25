using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_App
{
    public class PASSWORD_POLICY : ICustom_Item
    {
        string type;
        string description;
        string value_data;
        string password_policy;

        public PASSWORD_POLICY(List<string> password_policy)
        {
            this.type = "PASSWORD_POLICY";
            this.description = setDescription(password_policy.FirstOrDefault(s => s.Contains("description")));
            this.value_data = setValue_Data(password_policy.FirstOrDefault(s => s.Contains("value_data")));
            Console.WriteLine(description);
            Console.WriteLine(value_data);
            
        }

        public string setValue_Data(string value_data)
        {
            value_data = value_data.Substring(value_data.IndexOf(':') + 3, value_data.Length - 1 - value_data.LastIndexOf(']') - 1);
            return value_data;
        }

        public string setDescription(string description)
        {
            description = description.Substring(description.IndexOf(':') + 3, description.Length - 1 - description.LastIndexOf('"'));
            return description;
        }
    }
}
