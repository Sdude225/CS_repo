using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CS_App
{
    class Enforcer : Form
    {
        ListView listView1 = Application.OpenForms["Form1"].Controls["listView1"] as ListView;
        List<string> tobeEnforced = new List<string>();
        public Enforcer(List<ICustom_Item> custom_Items, List<string> localPolicies)
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SBT");
            System.IO.Directory.CreateDirectory(filePath);

            filePath = Path.Combine(filePath, "security_policy.cfg");

            tobeEnforced = localPolicies;

            int i = 0;
            foreach(ICustom_Item item in custom_Items)
            {
                switch(item.Type)
                {
                    case "PASSWORD_POLICY":
                        enforcePassword((PASSWORD_POLICY)item);
                        break;
                    case "LOCKOUT_POLICY":
                        LOCKOUT_POLICY tmp = (LOCKOUT_POLICY)item;
                        if (tmp.LockoutType == "LOCKOUT_THRESHOLD")
                            enforceUtility("LockoutBadCount", tmp);
                        break;
                    case "BANNER_CHECK":
                        banEnforce((BANNER_CHECK)item);
                        break;
                    case "REGISTRY_SETTING":
                        regEnforce((REGISTRY_SETTING)item);
                        break;
                }
                i++;
            }

            Apply(filePath);
        }

        public void regEnforce(REGISTRY_SETTING regSetting)
        {
            Console.WriteLine("reg add " + regSetting.Reg_Key + " /v " + regSetting.Reg_Item + " /t REG_DWORD /d " + regSetting.Value_Data + " /f");
            Process p = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "cmd.exe";
            info.RedirectStandardInput = true;
            info.UseShellExecute = false;
            info.Verb = "runas";
            p.StartInfo = info;
            p.Start();

            using (StreamWriter sw = p.StandardInput)
            {
                sw.WriteLine("reg add " + regSetting.Reg_Key + " /v " + regSetting.Reg_Item + " /t REG_DWORD /d " + regSetting.Value_Data + " /f");
            }
            p.WaitForExit();
        }

        public void banEnforce(BANNER_CHECK bannerPolicy)
        {
            Process p = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "cmd.exe";
            info.RedirectStandardInput = true;
            info.UseShellExecute = false;
            info.Verb = "runas";
            p.StartInfo = info;
            p.Start();

            using (StreamWriter sw = p.StandardInput)
            {
                sw.WriteLine("reg add " + bannerPolicy.Reg_Key + " /v " + bannerPolicy.Reg_Item + " /t REG_SZ /d " + bannerPolicy.Value_Data + " /f");
            }
            p.WaitForExit();
        }

        public void Apply(string filePath)
        {
            int index = tobeEnforced.FindIndex(s => s.Contains("NewAdministratorName"));
            string line = tobeEnforced[index].Substring(tobeEnforced[index].IndexOf('"'), tobeEnforced[index].LastIndexOf('"') - tobeEnforced[index].IndexOf('"') + 1);
            tobeEnforced[index] = tobeEnforced[index].Replace(line, "\"admin\"");
            index = tobeEnforced.FindIndex(s => s.Contains("NewGuestName"));
            line = tobeEnforced[index].Substring(tobeEnforced[index].IndexOf('"'), tobeEnforced[index].LastIndexOf('"') - tobeEnforced[index].IndexOf('"') + 1);
            tobeEnforced[index] = tobeEnforced[index].Replace(line, "\"Guest\"");
            System.IO.File.WriteAllLines(filePath, tobeEnforced);

            Process p = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "cmd.exe";
            info.RedirectStandardInput = true;
            info.UseShellExecute = false;
            info.Verb = "runas";
            p.StartInfo = info;
            p.Start();

            using (StreamWriter sw = p.StandardInput)
            {
                sw.WriteLine("secedit.exe /configure /db %windir%\\securitynew.sdb /cfg " + filePath + " /areas SECURITYPOLICY");
            }
            p.WaitForExit();

        }

        public void enforceUtility(string value, ICustom_Item passwordPolicy)
        {
            string pattern = @"-?\d+";
            int index = tobeEnforced.FindIndex(s => s.Contains(value));
            tobeEnforced[index] = tobeEnforced[index].Replace(Regex.Match(tobeEnforced[index], pattern).Value, Regex.Match(passwordPolicy.Value_Data, pattern).Value);
        }

        public void enforcePassword(PASSWORD_POLICY passwordPolicy)
        {
            switch(passwordPolicy.PasswordType)
            {
                case "ENFORCE_PASSWORD_HISTORY":
                    enforceUtility("PasswordHistorySize", passwordPolicy);
                    break;
                case "MAXIMUM_PASSWORD_AGE":
                    string pattern = @"-?\d+";
                    int index = tobeEnforced.FindIndex(s => s.Contains("MaximumPasswordAge"));
                    tobeEnforced[index] = tobeEnforced[index].Replace(Regex.Match(tobeEnforced[index], pattern).Value, Regex.Match(passwordPolicy.Value_Data, pattern).Value + "0");
                    break;
                case "MINIMUM_PASSWORD_AGE":
                    enforceUtility("MinimumPasswordAge", passwordPolicy);
                    break;
                case "MINIMUM_PASSWORD_LENGTH":
                    enforceUtility("MinimumPasswordLength", passwordPolicy);
                    break;
                case "COMPLEXITY_REQUIREMENTS":
                    enforceUtility("PasswordComplexity", passwordPolicy);
                    break;
                case "FORCE_LOGOFF":
                    enforceUtility("ForceLogoffWhenHourExpire", passwordPolicy);
                    break;
            }
        }
    }
}
