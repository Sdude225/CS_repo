using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CS_App
{
    class CHECK_ACCOUNT : ICustom_Item
    {
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

        string description;

        public string Description
        {
            get { return description; }

            set { this.description = value; }
        }

        string check_type;

        public string Check_Type
        {
            get { return check_type; }

            set { this.check_type = value; }
        }

        string account_type;

        public string Account_Type
        {
            get { return account_type; }

            set { this.account_type = value; }
        }

        public CHECK_ACCOUNT(List<string> checkAccountPolicy)
        {
            Type = "CHECK_ACCOUNT";

            setValueType(checkAccountPolicy.FirstOrDefault(s => s.Contains("value_type")));
            setValueData(checkAccountPolicy.FirstOrDefault(s => s.Contains("value_data")));
            setDescription(checkAccountPolicy.FirstOrDefault(s => s.Contains("description")));
            setAccountType(checkAccountPolicy.FirstOrDefault(s => s.Contains("account_type")));
        }

        public void setAccountType(string account_type)
        {
            account_type = Regex.Replace(account_type, @"\s+", "");
            account_type = account_type.Replace("account_type:", "");
            Account_Type = account_type;
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
            switch(Value_Type)
            {
                case "POLICY_SET":
                    if (value_data == "\"Enabled\"")
                        Value_Data = "1";
                    else
                        Value_Data = "0";
                    break;
                default:
                    Value_Data = value_data;
                    break;
            }
        }

        public void setDescription(string description)
        {
            description = description.Substring(description.IndexOf(':') + 3, description.LastIndexOf('"') - description.IndexOf(':') - 3);
            Description = description;
        }
    }
}
