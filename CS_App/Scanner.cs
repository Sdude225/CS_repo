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
using System.Xml;
using System.Xml.Schema;

namespace CS_App
{
    class Scanner : Form
    {
        List<ICustom_Item> custom_Items = new List<ICustom_Item>();
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
            p.WaitForExit();
            
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
                        PASSWORD_POLICY pass_pol = new PASSWORD_POLICY(s);
                        custom_Items.Add(pass_pol);
                        passwordPolicyCheck(pass_pol, lines, index);
                        break;
                    case "LOCKOUT_POLICY":
                        LOCKOUT_POLICY lock_pol = new LOCKOUT_POLICY(s);
                        custom_Items.Add(lock_pol);
                        lockoutPolicyCheck(lock_pol, lines, index);
                        break;
                    case "REGISTRY_SETTING":
                        REGISTRY_SETTING reg_pol = new REGISTRY_SETTING(s);
                        custom_Items.Add(reg_pol);
                        registryPolicyCheck(reg_pol, lines, index);
                        break;
                    case "CHECK_ACCOUNT":
                        CHECK_ACCOUNT chk_pol = new CHECK_ACCOUNT(s);
                        custom_Items.Add(chk_pol);
                        checkAccount(chk_pol, lines, index);
                        break;
                    case "BANNER_CHECK":
                        BANNER_CHECK ban_pol = new BANNER_CHECK(s);
                        custom_Items.Add(ban_pol);
                        bannerCheck(ban_pol, lines, index);
                        break;
                    default:
                        listView1.Items[index].BackColor = Color.Gray;
                        break;
                }
                i++;
            }
        }

        public void passwordPolicyCheck(PASSWORD_POLICY passwordPolicy, IEnumerable<string> localPolicies, int index)
        {
            List<string> minMaxList = new List<string>();
            string localValue;
            string tmp;
            string minValue;
            switch(passwordPolicy.PasswordType)
            {
                case "ENFORCE_PASSWORD_HISTORY":
                    localValue = localPolicies.FirstOrDefault(s => s.Contains("PasswordHistorySize"));
                    tmp = Regex.Match(localValue, @"-?\d+").Value;
                    minValue = Regex.Match(passwordPolicy.Value_Data, @"-?\d+").Value;
                    if (Int32.Parse(tmp) >= Int32.Parse(minValue))
                        listView1.Items[index].BackColor = Color.Green;
                    else
                        listView1.Items[index].BackColor = Color.Red;
                    break;
                case "MAXIMUM_PASSWORD_AGE":
                    localValue = localPolicies.FirstOrDefault(s => s.Contains("MaximumPasswordAge"));
                    tmp = Regex.Match(localValue, @"-?\d+").Value;
                    foreach(Match match in Regex.Matches(passwordPolicy.Value_Data, @"-?\d+"))
                        minMaxList.Add(match.Value);
                    if (Int32.Parse(tmp) >= Int32.Parse(minMaxList[0]) && Int32.Parse(tmp) <= Int32.Parse(minMaxList[1]))
                        listView1.Items[index].BackColor = Color.Green;
                    else
                        listView1.Items[index].BackColor = Color.Red;
                    break;
                case "MINIMUM_PASSWORD_AGE":
                    localValue = localPolicies.FirstOrDefault(s => s.Contains("MinimumPasswordAge"));
                    tmp = Regex.Match(localValue, @"-?\d+").Value;
                    minValue = Regex.Match(passwordPolicy.Value_Data, @"-?\d+").Value;
                    if (Int32.Parse(tmp) >= Int32.Parse(minValue))
                        listView1.Items[index].BackColor = Color.Green;
                    else
                        listView1.Items[index].BackColor = Color.Red;
                    break;
                case "MINIMUM_PASSWORD_LENGTH":
                    localValue = localPolicies.FirstOrDefault(s => s.Contains("MinimumPasswordLength"));
                    tmp = Regex.Match(localValue, @"-?\d+").Value;
                    minValue = Regex.Match(passwordPolicy.Value_Data, @"-?\d+").Value;
                    if (Int32.Parse(tmp) >= Int32.Parse(minValue))
                        listView1.Items[index].BackColor = Color.Green;
                    else
                        listView1.Items[index].BackColor = Color.Red;
                    break;
                case "COMPLEXITY_REQUIREMENTS":
                    localValue = localPolicies.FirstOrDefault(s => s.Contains("PasswordComplexity"));
                    tmp = Regex.Match(localValue, @"-?\d+").Value;
                    if (Int32.Parse(tmp) == Int32.Parse(passwordPolicy.Value_Data))
                        listView1.Items[index].BackColor = Color.Green;
                    else
                        listView1.Items[index].BackColor = Color.Red;
                    break;
                case "REVERSIBLE_ENCRYPTION":
                    listView1.Items[index].BackColor = Color.LightGray;
                    break;
                case "FORCE_LOGOFF":
                    localValue = localPolicies.FirstOrDefault(s => s.Contains("ForceLogoffWhenHourExpire"));
                    tmp = Regex.Match(localValue, @"-?\d+").Value;
                    if (Int32.Parse(tmp) == Int32.Parse(passwordPolicy.Value_Data))
                        listView1.Items[index].BackColor = Color.Green;
                    else
                        listView1.Items[index].BackColor = Color.Red;
                    break;
            }
        }

        public void lockoutPolicyCheck(LOCKOUT_POLICY lockoutPolicy, IEnumerable<string> localPolicies, int index)
        {
            switch(lockoutPolicy.LockoutType)
            {
                case "LOCKOUT_DURATION":
                    listView1.Items[index].BackColor = Color.Gray;
                    break;
                case "LOCKOUT_THRESHOLD":
                    string localValue = localPolicies.FirstOrDefault(s => s.Contains("LockoutBadCount"));
                    string tmp = Regex.Match(localValue, @"-?\d+").Value;
                    List<string> minMaxList = new List<string>();
                    foreach (Match match in Regex.Matches(lockoutPolicy.Value_Data, @"-?\d+"))
                        minMaxList.Add(match.Value);
                    if (Int32.Parse(tmp) >= Int32.Parse(minMaxList[0]) && Int32.Parse(tmp) <= Int32.Parse(minMaxList[1]))
                        listView1.Items[index].BackColor = Color.Green;
                    else
                        listView1.Items[index].BackColor = Color.Red;
                    break;
                case "LOCKOUT_RESET":
                    listView1.Items[index].BackColor = Color.Gray;
                    break;
            }

        }

        public void registryPolicyCheck(REGISTRY_SETTING registryPolicy, IEnumerable<string> localPolicies, int index)
        {
            string output = null;
            string err = null;
            registryPolicy.checkQuery(ref output, ref err);

            if (err.Length != 0)
                listView1.Items[index].BackColor = Color.LightGray;
            else
            {
                if (!registryPolicy.compareRegister(output))
                    listView1.Items[index].BackColor = Color.Red;
                else
                {
                    listView1.Items[index].BackColor = Color.Green;
                }
            }
        }

        public void checkAccount(CHECK_ACCOUNT chkAccPolicy, IEnumerable<string> localPolicies, int index)
        {
            string localValue;
            string val;
            switch(chkAccPolicy.Account_Type)
            {
                case "ADMINISTRATOR_ACCOUNT":
                    if (chkAccPolicy.Value_Type == "POLICY_SET")
                    {
                        localValue = localPolicies.FirstOrDefault(s => s.Contains("EnableAdminAccount"));
                        val = Regex.Match(localValue, @"\d+").Value;
                        if (Int32.Parse(chkAccPolicy.Value_Data) == Int32.Parse(val))
                            listView1.Items[index].BackColor = Color.Green;
                        else
                            listView1.Items[index].BackColor = Color.Red;
                    }
                    else
                        listView1.Items[index].BackColor = Color.Green;
                    break;
                case "GUEST_ACCOUNT":
                    if (chkAccPolicy.Value_Type == "POLICY_SET")
                    {
                        localValue = localPolicies.FirstOrDefault(s => s.Contains("EnableGuestAccount"));
                        val = Regex.Match(localValue, @"\d+").Value;
                        if (Int32.Parse(chkAccPolicy.Value_Data) == Int32.Parse(val))
                            listView1.Items[index].BackColor = Color.Green;
                        else
                            listView1.Items[index].BackColor = Color.Red;
                    }
                    else
                        listView1.Items[index].BackColor = Color.Green;
                    break;
            }
        }

        public void bannerCheck(BANNER_CHECK bannerPolicy, IEnumerable<string> localPolicies, int index)
        {
            string output = null;
            string err = null;

            bannerPolicy.checkQuery(ref output, ref err);

            if (err.Length != 0)
                listView1.Items[index].BackColor = Color.LightGray;
            else
            {
                if (!bannerPolicy.compareReg(output))
                    listView1.Items[index].BackColor = Color.Red;
                else
                    listView1.Items[index].BackColor = Color.Green;
            }
        }
    }
}
