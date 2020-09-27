using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CS_App
{
    class BANNER_CHECK : ICustom_Item
    {
        string description;

        public string Description
        {
            get { return description; }

            set { this.description = value; }
        }

        string value_type;

        public string Value_Type
        {
            get { return value_type; }

            set { this.value_type = value; }
        }

        string value_data;

        public string Value_Data
        {
            get { return value_data; }

            set { this.value_data = value; }
        }

        string type;

        public string Type
        {
            get { return type; }

            set { this.type = value; }
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

        public BANNER_CHECK(List<string> bannerPolicy)
        {
            Type = "BANNER_CHECK";

            setDescription(bannerPolicy.FirstOrDefault(s => s.Contains("description")));
            setValueData(bannerPolicy.FirstOrDefault(s => s.Contains("value_data")));
            setValueType(bannerPolicy.FirstOrDefault(s => s.Contains("value_type")));
            setRegItem(bannerPolicy.FirstOrDefault(s => s.Contains("reg_item")));
            setRegKey(bannerPolicy.FirstOrDefault(s => s.Contains("reg_key")));
        }

        public bool compareReg(string output)
        {
            string[] outputLines = output.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            string[] regSet = outputLines[1].Split(new[] { "    " }, StringSplitOptions.RemoveEmptyEntries);

            if (regSet.Length == 2)
                return false;
            else
            {
                Console.WriteLine(regSet[2] + " " + Value_Data);
                if (regSet[2] == Value_Data)
                    return true;
                else
                    return false;
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
