using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CS_App
{
    class REGISTRY_SETTING : ICustom_Item
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

        string reg_key;

        public string Reg_Key
        {
            get { return reg_key; }

            set { this.reg_key = value; }
        }

        string reg_item;

        public string Reg_Item
        {
            get { return reg_item; }

            set { this.reg_item = value; }
        }

        string localRegValue;

        public string LOCALREGVALUE
        {
            get { return localRegValue; }

            set { this.localRegValue = value; }
        }

        public REGISTRY_SETTING(List<string> registryPolicy)
        {
            Type = "REGISTRY_SETTING";

            setDescription(registryPolicy.FirstOrDefault(s => s.Contains("description")));
            setValueData(registryPolicy.FirstOrDefault(s => s.Contains("value_data")));
            setValueType(registryPolicy.FirstOrDefault(s => s.Contains("value_type")));
            setRegItem(registryPolicy.FirstOrDefault(s => s.Contains("reg_item")));
            setRegKey(registryPolicy.FirstOrDefault(s => s.Contains("reg_key")));
        }

        public bool compareRegister(string output)
        {
            List<string> minMaxValues = new List<string>();
            string[] outputLines = output.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if(outputLines.Length != 2)
                return false;
            else
            {
                string[] regSet = outputLines[1].Split(new[] { "    " }, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    switch(Value_Type)
                    {
                        case "POLICY_DWORD":
                            foreach (Match match in Regex.Matches(Value_Data, @"-?\d+"))
                                minMaxValues.Add(match.Value);
                            LOCALREGVALUE = regSet[2];
                            if(minMaxValues.Count == 1)
                            {
                                if (Int32.Parse(minMaxValues[0]) == Convert.ToInt32(regSet[2], 16))
                                    return true;
                                else
                                    return false;
                            }
                            else
                            {
                                if (Convert.ToInt32(regSet[2], 16) >= Int32.Parse(minMaxValues[0]) && Convert.ToInt32(regSet[2], 16) <= Int32.Parse(minMaxValues[1]))
                                    return true;
                                else
                                    return false;
                            }
                        case "POLICY_MULTI_TEXT":
                            string[] value = Value_Data.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                            string[] regValue = regSet[2].Split(new[] { @"\0" }, StringSplitOptions.RemoveEmptyEntries);

                            for(int i = 0; i < value.Length; i++)
                            {
                                value[i] = Regex.Replace(value[i], "[\"^()$]", "");
                                value[i] = value[i].Replace(@"\\", @"\");
                            }

                            for(int i = 0; i < regValue.Length; i++)
                                regValue[i] = Regex.Replace(regValue[i], @"\s+", "");

                            bool flag = true;

                            foreach(string s in regValue)
                            {
                                if (!value.Contains(s))
                                    flag = false;
                            }

                            return flag;
                            
                        default:
                            return false;
                    }
                }
                catch (Exception e)
                {
                    return true;
                }
            }
        }

        public void checkQuery(ref string output, ref string err)
        {
            string cmdQuery = "reg query " + Reg_Key + " /v " + Reg_Item;
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c " + cmdQuery;
            process.StartInfo.Verb = "runas";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            output = process.StandardOutput.ReadToEnd();
            err = process.StandardError.ReadToEnd();
            process.WaitForExit();
            
        }

        public void setRegKey(string regKey)
        {
            regKey = regKey.Substring(regKey.IndexOf('"'), regKey.LastIndexOf('"') - regKey.IndexOf('"') + 1);
            Reg_Key = regKey;
        }

        public void setRegItem(string regItem)
        {
            regItem = regItem.Substring(regItem.IndexOf('"'), regItem.LastIndexOf('"') - regItem.IndexOf('"') + 1);
            Reg_Item = regItem;
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
            Value_Data = value_data;
        }

        public void setDescription(string description)
        {
            description = description.Substring(description.IndexOf(':') + 3, description.LastIndexOf('"') - description.IndexOf(':') - 3);
            Description = description;
        }
    }
}
