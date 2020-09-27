using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CS_App
{
    class LOCKOUT_POLICY : ICustom_Item
    {
        string description;

        public string Description
        {
            get { return description; }

            set { this.description = value; }
        }

        string type;

        public string Type
        {
            get { return type; }

            set { this.type = value; }
        }

        string value_data;

        public string Value_Data
        {
            get { return value_data; }

            set { this.value_data = value; }
        }

        string value_type;

        public string Value_Type
        {
            get { return value_type; }

            set { this.value_type = value; }
        }

        string lockoutPolicyType;

        public string LockoutType
        {
            get { return lockoutPolicyType; }

            set { lockoutPolicyType = value; }
        }

        public LOCKOUT_POLICY(List<string> lockoutPolicy)
        {
            Type = "LOCKOUT_POLICY";

            setDescription(lockoutPolicy.FirstOrDefault(s => s.Contains("description")));
            setValueData(lockoutPolicy.FirstOrDefault(s => s.Contains("value_data")));
            setValueType(lockoutPolicy.FirstOrDefault(s => s.Contains("value_type")));
            setLockoutType(lockoutPolicy.FirstOrDefault(s => s.Contains("lockout_policy")));
        }

        public void setLockoutType(string lockout)
        {
            lockout = Regex.Replace(lockout, @"\s+", "");
            lockout = lockout.Replace("lockout_policy:", "");
            LockoutType = lockout;
        }

        public void setValueType(string value_type)
        {
            value_type = Regex.Replace(value_type, @"\s+", "");
            value_type = value_type.Replace("value_type:", "");
            Value_Type = value_type;
        }

        public void setValueData(string value_data)
        {
            value_data = Regex.Replace(value_data, @"\s+", "");
            value_data = value_data.Replace("value_data:", "");
            if (value_data == "\"Enabled\"")
                value_data = "1";
            else if (value_data == "\"Disabled\"")
                value_data = "0";
            Value_Data = value_data;
        }

        public void setDescription(string description)
        {
            description = description.Substring(description.IndexOf(':') + 3, description.LastIndexOf('"') - description.IndexOf(':') - 3);
            Description = description;
        }
    }
}
