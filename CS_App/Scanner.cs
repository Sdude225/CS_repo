using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Schema;

namespace CS_App
{
    class Scanner : Form
    {
        List<string> localSecurityPolicies = new List<string>();
        ListView listView1 = Application.OpenForms["Form1"].Controls["listView1"] as ListView;

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

            int index;
            int i = 0;
            foreach (List<string> s in policies)
            {
                string type = s.FirstOrDefault(str => str.Contains("type"));
                type = Regex.Replace(type, @"\s+", "");
                type = type.Replace("type:", "");
                index = listView1.CheckedIndices[i];

                switch (type)
                {
                    case "PASSWORD_POLICY":
                        passwordPolicyCheck(s, lines, index);
                        break;
                    case "LOCKOUT_POLICY":
                        lockoutPolicyCheck(s, lines, index);
                        break;
                }
                i++;
            }
        }

        public void passwordPolicyCheck(List<string> passwordPolicy, IEnumerable<string> localPolicies, int index)
        {
            string pass = passwordPolicy.FirstOrDefault(str => str.Contains("password_policy"));
            pass = Regex.Replace(pass, @"\s+", "");
            pass = pass.Replace("password_policy:", "");
            string localValue;
            string tmp;
            int val;
            string value_data;
            int min_value;
            int max_value;


            switch (pass)
            {
                case "ENFORCE_PASSWORD_HISTORY":
                    localValue = localPolicies.FirstOrDefault(s => s.Contains("PasswordHistorySize"));
                    tmp = localValue.Substring(localValue.LastIndexOf('=') + 1, localValue.Length - 1 - localValue.LastIndexOf('='));
                    val = Int32.Parse(tmp);
                    if (val == 1)
                        listView1.Items[index].BackColor = Color.Green;
                    else
                        listView1.Items[index].BackColor = Color.Red;
                    break;
                case "MAXIMUM_PASSWORD_AGE":
                    localValue = localPolicies.FirstOrDefault(s => s.Contains("MaximumPasswordAge"));
                    tmp = localValue.Substring(localValue.LastIndexOf('=') + 1, localValue.Length - 1 - localValue.LastIndexOf('='));
                    value_data = passwordPolicy.FirstOrDefault(str => str.Contains("value_data"));
                    value_data = Regex.Replace(value_data, @"\s+", "");
                    value_data = value_data.Replace("value_data:", "");
                    val = int.Parse(tmp, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign);
                    min_value = Int32.Parse(value_data.Substring(1, value_data.IndexOf('.') - 1));
                    max_value = int.Parse(value_data.Substring(value_data.LastIndexOf('.') + 1, value_data.Length - 1 - value_data.LastIndexOf('.') - 1));
                    if (val >= min_value && val <= max_value)
                        listView1.Items[index].BackColor = Color.Green;
                    else
                        listView1.Items[index].BackColor = Color.Red;
                    break;
                case "MINIMUM_PASSWORD_AGE":
                    localValue = localPolicies.FirstOrDefault(s => s.Contains("MinimumPasswordAge"));
                    val = (int)localValue[localValue.Count() - 1] - 48;
                    value_data = passwordPolicy.FirstOrDefault(str => str.Contains("value_data"));
                    value_data = Regex.Replace(value_data, @"\s+", "");
                    value_data = value_data.Replace("value_data:", "");
                    min_value = Int32.Parse(value_data.Substring(1, value_data.IndexOf('.') - 1));
                    if (val >= min_value)
                        listView1.Items[index].BackColor = Color.Green;
                    else
                        listView1.Items[index].BackColor = Color.Red;
                    break;
                case "MINIMUM_PASSWORD_LENGTH":
                    localValue = localPolicies.FirstOrDefault(s => s.Contains("MinimumPasswordLength"));
                    tmp = localValue.Substring(localValue.LastIndexOf('=') + 1, localValue.Length - 1 - localValue.LastIndexOf('='));
                    val = Int32.Parse(tmp);
                    value_data = passwordPolicy.FirstOrDefault(str => str.Contains("value_data"));
                    value_data = Regex.Replace(value_data, @"\s+", "");
                    value_data = value_data.Replace("value_data:", "");
                    min_value = Int32.Parse(value_data.Substring(1, value_data.IndexOf('.') - 1));
                    if (val >= min_value)
                        listView1.Items[index].BackColor = Color.Green;
                    else
                        listView1.Items[index].BackColor = Color.Red;
                    break;
                case "COMPLEXITY_REQUIREMENTS":
                    localValue = localPolicies.FirstOrDefault(s => s.Contains("PasswordComplexity"));
                    tmp = localValue.Substring(localValue.LastIndexOf('=') + 1, localValue.Length - 1 - localValue.LastIndexOf('='));
                    val = Int32.Parse(tmp);
                    if (val == 1)
                        listView1.Items[index].BackColor = Color.Green;
                    else
                        listView1.Items[index].BackColor = Color.Red;
                    break;
                case "REVERSIBLE_ENCRYPTION":
                    listView1.Items[index].BackColor = Color.LightGray;
                    break;
                case "FORCE_LOGOFF":
                    localValue = localPolicies.FirstOrDefault(s => s.Contains("ForceLogoffWhenHourExpire"));
                    tmp = localValue.Substring(localValue.LastIndexOf('=') + 1, localValue.Length - 1 - localValue.LastIndexOf('='));
                    val = Int32.Parse(tmp);
                    if (val == 1)
                        listView1.Items[index].BackColor = Color.Green;
                    else
                        listView1.Items[index].BackColor = Color.Red;
                    break;
            }
        }

        public void lockoutPolicyCheck(List<string> lockoutPolicy, IEnumerable<string> localPolicies, int index)
        {
            string lockout = lockoutPolicy.FirstOrDefault(str => str.Contains("lockout_policy"));
            lockout = Regex.Replace(lockout, @"\s+", "");
            lockout = lockout.Replace("lockout_policy:", "");
            string localValue;
            string tmp;
            int val;
            string value_data;
            int min_value;
            int max_value;
            
            switch(lockout)
            {
                case "LOCKOUT_DURATION":
                    listView1.Items[index].BackColor = Color.Gray;
                    break;
                case "LOCKOUT_THRESHOLD":
                    localValue = localPolicies.FirstOrDefault(s => s.Contains("LockoutBadCount"));
                    tmp = localValue.Substring(localValue.LastIndexOf('=') + 1, localValue.Length - 1 - localValue.LastIndexOf('='));
                    value_data = lockoutPolicy.FirstOrDefault(str => str.Contains("value_data"));
                    value_data = Regex.Replace(value_data, @"\s+", "");
                    value_data = value_data.Replace("value_data:", "");
                    val = int.Parse(tmp, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign);
                    min_value = Int32.Parse(value_data.Substring(1, value_data.IndexOf('.') - 1));
                    max_value = int.Parse(value_data.Substring(value_data.LastIndexOf('.') + 1, value_data.Length - 1 - value_data.LastIndexOf('.') - 1));
                    if (val >= min_value && val <= max_value)
                        listView1.Items[index].BackColor = Color.Green;
                    else
                        listView1.Items[index].BackColor = Color.Red;
                    break;
                case "LOCKOUT_RESET":
                    listView1.Items[index].BackColor = Color.Gray;
                    break;
            }

        }
    }
}
