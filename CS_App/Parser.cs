using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CS_App
{
    class Parser
    {
        private List<List<string>> fnList = new List<List<string>>();
        private List<string> stack = new List<string>();
        public Parser(string filePath)
        {
            string line = null;
            bool flg = false;
            System.IO.StreamReader file = new System.IO.StreamReader(filePath);
            Regex rgx0 = new Regex("^\\s+\\b(type|description|value_type|value_data|reg_key|reg_item|check_type|reg_option|password_policy|audit_policy_subcategory)\\b");
            Regex rgx1 = new Regex("(^\\s*\\<\\b(custom_item)\\b)|(^\\s*\\<\\/\\b(custom_item)\\b)");
            while ((line = file.ReadLine()) != null)
            {
                if (line.StartsWith("#") || line == "") continue;
                if (rgx1.IsMatch(line))
                {
                    flg = !flg;
                    if (stack.Count != 0)
                    {
                        stack.Add(line);
                        fnList.Add(stack);
                        stack = new List<string>();
                    }
                }
                if (flg)
                    if (rgx0.IsMatch(line)) stack.Add(line);
            }
        }
        public List<List<string>> getParsedText() { return fnList; }

        public List<List<string>> getData(StreamReader streamReader)
        {
            List<List<string>> fileContent = new List<List<string>>();
            List<string> stack = new List<string>();
            string line = null;
            while((line = streamReader.ReadLine()) != null)
            {
                if (!line.Contains("custom_item"))
                    stack.Add(line);
                else
                {
                    stack.Add(line);
                    fileContent.Add(stack);
                    stack = new List<string>();
                }
                
            }

            return fileContent;
        }
    }
}
