﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CS_App
{
    public class PASSWORD_POLICY : ICustom_Item
    {
        string type;

        public string Type
        {
            get { return type; }

            set { this.type = value; }
        }
        string description;

        public string Description
        {
            get { return description; }

            set { this.description = value; }
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

        string password_policy;

        public string PasswordType
        {
            get { return password_policy; }

            set { this.password_policy = value; }
        }

        public PASSWORD_POLICY(List<string> password_policy)
        {
            Type = "PASSWORD_POLICY";

            setDescription(password_policy.FirstOrDefault(s => s.Contains("description")));
            setValueData(password_policy.FirstOrDefault(s => s.Contains("value_data")));
            setValueType(password_policy.FirstOrDefault(s => s.Contains("value_type")));
            setPasswordPolicy(password_policy.FirstOrDefault(s => s.Contains("password_policy")));
        }

        public void setPasswordPolicy(string password)
        {
            password = Regex.Replace(password, @"\s+", "");
            password = password.Replace("password_policy:", "");
            PasswordType = password;
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
