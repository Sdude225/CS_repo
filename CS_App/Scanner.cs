using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace CS_App
{
    class Scanner
    {
        List<string> localSecurityPolicies = new List<string>();

        public Scanner(List<List<string>> policies)
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SBT");
            System.IO.Directory.CreateDirectory(filePath);

            Process p = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "cmd.exe";
            info.RedirectStandardInput = true;
            info.UseShellExecute = false;
            info.Verb = "runas";
            p.StartInfo = info;
            p.Start();
            using(StreamWriter sw = p.StandardInput)
            {
                sw.WriteLine("cd " + filePath);
                sw.WriteLine("secedit.exe /export /cfg " + filePath + @"\security_policy.inf");
            }
            p.Close();

            filePath = Path.Combine(filePath, "security_policy.inf");
            var lines = File.ReadLines(filePath);

            foreach(List<string> s in policies)
            {
                string type = s.FirstOrDefault(str => str.Contains("type"));
                type = Regex.Replace(type, @"\s+", "");
                type = type.Replace("type:", "");
                
                switch(type)
                {
                    case "PASSWORD_POLICY":
                        passwordPolicyCheck(s, lines);
                        break;
                }

            }
        }

        public void passwordPolicyCheck(List<string> passwordPolicy, IEnumerable<string> localPolicies)
        {
            string pass = passwordPolicy.FirstOrDefault(str => str.Contains("password_policy"));
            pass = Regex.Replace(pass, @"\s+", "");
            pass = pass.Replace("password_policy:", "");


            switch (pass)
            {
                case "ENFORCE_PASSWORD_HISTORY":
                    Console.WriteLine("ENFORCE_PASSWORD_HISTORY");
                    break;
                case "MAXIMUM_PASSWORD_AGE":
                    Console.WriteLine("MAXIMUM_PASSWORD_AGE");
                    break;
                case "MINIMUM_PASSWORD_AGE":
                    string localValue = localPolicies.FirstOrDefault(s => s.Contains("MinimumPasswordAge"));
                    char val = localValue[localValue.Count() - 1];
                    string value_data = passwordPolicy.FirstOrDefault(str => str.Contains("value_data"));
                    value_data = Regex.Replace(value_data, @"\s+", "");
                    value_data = value_data.Replace("value_data:", "");
                    Console.WriteLine(value_data);

                    break;
                case "MINIMUM_PASSWORD_LENGTH":
                    Console.WriteLine("MINIMUM_PASSWORD_LENGTH");
                    break;
                case "COMPLEXITY_REQUIREMENTS":
                    Console.WriteLine("COMPLEXITY_REQUIREMENTS");
                    break;
                case "REVERSIBLE_ENCRYPTION":
                    Console.WriteLine("FORCE_LOGOFF");
                    break;
            }
        }
    }
}
