using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CS_App
{
    public partial class Form1 : Form
    {
        public String fileContent = String.Empty;
            
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var file = new OpenFileDialog();
            file.Filter = "Audit files (*.audit)|*.audit";
            if(file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var fileStream = file.OpenFile();

                using(StreamReader reader = new StreamReader(fileStream))
                {
                    fileContent = reader.ReadToEnd();
                }
            }

            Parser();
        }

        private bool isComment(string line)
        {
            return line.StartsWith("#");
        }

        private bool removeAnd(string line)
        {
            return line.Contains("&");
        }


        private void custom_item(ref List<string> lines, ref int i)
        {
            int k = i;
            int j, m;

            for (j = i; j < lines.Count; j++)
            {
                if (!lines[j].Contains("</custom_item"))
                {
                    continue;
                }
                else
                {
                    i = j - 1;
                    break;
                }
            }

            for(m = k + 1; m < j; m++)
            {
                if(lines[m].Contains("check_type"))
                {
                    lines[m] = lines[m].Replace("check_type", "<check_type>");
                    lines[m] = lines[m].Replace(":", "");
                    lines[m] = lines[m].Insert(lines[m].Length - 2, "</check_type>");
                    continue;
                }
                else if(lines[m].Contains("value_type"))
                {
                    lines[m] = lines[m].Replace("value_type", "<value_type>");
                    lines[m] = lines[m].Replace(":", "");
                    lines[m] = lines[m].Insert(lines[m].Length - 2, "</value_type>");
                    continue;
                }
                else if(lines[m].Contains("value_data"))
                {
                    lines[m] = lines[m].Replace("value_data", "<value_data>");
                    lines[m] = lines[m].Replace(":", "");
                    lines[m] = lines[m].Replace('"', ' ');
                    lines[m] = lines[m].Insert(lines[m].Length - 2, "</value_data>");
                    continue;
                }
                else if(lines[m].Contains("  description"))
                {
                    lines[m] = lines[m].Replace("description", "<description>");
                    lines[m] = lines[m].Replace(":", "");
                    lines[m] = lines[m].Replace('"', ' ');
                    lines[m] = lines[m].Insert(lines[m].Length - 2, "</description>");
                    continue;
                }
                else if(lines[m].Contains("reg_key"))
                {
                    lines[m] = lines[m].Replace("reg_key", "<reg_key>");
                    lines[m] = lines[m].Replace(":", "");
                    lines[m] = lines[m].Replace('"', ' ');
                    lines[m] = lines[m].Insert(lines[m].Length - 2, "</reg_key>");
                    continue;
                }
                else if(lines[m].Contains("reg_item"))
                {
                    lines[m] = lines[m].Replace("reg_item", "<reg_item>");
                    lines[m] = lines[m].Replace(":", "");
                    lines[m] = lines[m].Replace('"', ' ');
                    lines[m] = lines[m].Insert(lines[m].Length - 2, "</reg_item>");
                    continue;
                }
                else if(lines[m].Contains("  type"))
                {
                    lines[m] = lines[m].Replace("type", "<type>");
                    lines[m] = lines[m].Replace(":", "");
                    lines[m] = lines[m].Insert(lines[m].Length - 2, "</type>");
                    continue;
                }
                else if (lines[m].Contains("  reference"))
                {
                    lines[m] = lines[m].Replace("reference", "<reference>");
                    lines[m] = lines[m].Replace(":", "");
                    lines[m] = lines[m].Replace('"', ' ');
                    lines[m] = lines[m].Insert(lines[m].Length - 2, "</reference>");
                    continue;
                }
                else if (lines[m].Contains("reg_option"))
                {
                    lines[m] = lines[m].Replace("reg_option", "<reg_option>");
                    lines[m] = lines[m].Replace(":", "");
                    lines[m] = lines[m].Insert(lines[m].Length - 2, "</reg_option>");
                    continue;
                }
                else if (lines[m].Contains("see_also"))
                {
                    lines[m] = lines[m].Replace("see_also", "<see_also>");
                    lines[m] = lines[m].Replace(":", "");
                    lines[m] = lines[m].Replace('"', ' ');
                    lines[m] = lines[m].Insert(lines[m].Length - 2, "</see_also>");
                    continue;
                }
                else if(lines[m].Contains("  solution"))
                {
                    lines[m] = lines[m].Replace("solution", "<solution>");
                    lines[m] = lines[m].Replace(":", "");
                    lines[m] = lines[m].Replace('"', ' ');
                    m++;
                    while(char.IsLetterOrDigit(lines[m][0]))
                    {
                        m++;
                    }
                    lines[m - 1] = lines[m - 1].Replace('"', ' ');
                    lines[m - 1] = lines[m - 1].Insert(lines[m - 1].Length - 2, "</solution>");
                    m--;
                    continue;
                }
                else if(lines[m].Contains("  info"))
                {
                    lines[m] = lines[m].Replace("info", "<info>");
                    lines[m] = lines[m].Replace(":", "");
                    lines[m] = lines[m].Replace('"', ' ');
                    m++;
                    while(char.IsLetterOrDigit(lines[m][0]))
                    {
                        m++;
                    }
                    lines[m - 1] = lines[m - 1].Replace('"', ' ');
                    lines[m - 1] = lines[m - 1].Insert(lines[m - 1].Length - 2, "</info>");
                    m--;
                    continue;
                }
            }
        }

        private void report(ref List<string> lines, ref int i)
        {
            int k = i;
            int j, m;

            for(j = i; j < lines.Count; j++)
            {
                if(!lines[j].Contains("</report"))
                {
                    continue;
                }
                else
                {
                    i = j - 1;
                    break;
                }
            }

            for(m = k + 1; m < j; m++)
            {
                if(lines[m].Contains("  description"))
                {
                    lines[m] = lines[m].Replace("description", "<description>");
                    lines[m] = lines[m].Replace(":", "");
                    lines[m] = lines[m].Replace('"', ' ');
                    lines[m] = lines[m].Insert(lines[m].Length - 2, "</description>");
                    continue;
                }
                else if(lines[m].Contains("  info"))
                {
                    lines[m] = lines[m].Replace("info", "<info>");
                    lines[m] = lines[m].Replace(":", "");
                    lines[m] = lines[m].Replace('"', ' ');
                    lines[m] = lines[m].Insert(lines[m].Length - 2, "</info>");
                }
                else if (lines[m].Contains("see_also"))
                {
                    lines[m] = lines[m].Replace("see_also", "<see_also>");
                    lines[m] = lines[m].Replace(":", "");
                    lines[m] = lines[m].Replace('"', ' ');
                    lines[m] = lines[m].Insert(lines[m].Length - 2, "</see_also>");
                    continue;
                }
            }
        }
        private void Parser()
        {
            string[] tmp = fileContent.Split(new[] { "\r\n", "\r", "\n"}, StringSplitOptions.RemoveEmptyEntries);
            var lines = new List<string>(tmp);
            lines.RemoveAll(isComment);
            lines.RemoveAll(removeAnd);

            for(int i = 0; i < lines.Count; i++)
            {
                lines[i] += "\r\n";
            }

            for(int i = 0; i < lines.Count; i++)
            {
                if(lines[i].Contains(":") && lines[i].Contains("<"))
                {
                    lines[i] = lines[i].Replace(":", "=");
                }
                else if(lines[i].Contains("<custom_item"))
                {
                    custom_item(ref lines, ref i);
                }
                
                if(lines[i].Contains("<report"))
                {
                    report(ref lines, ref i);
                }
            }

            int index = lines[0].IndexOf("version");
            lines[0] = lines[0].Remove(22, (lines[0].Length - 3 - index)).Insert(11, " type");

            lines[1] = lines[1].Insert(13, " group");
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SBT");
            System.IO.Directory.CreateDirectory(filePath);

            filePath = Path.Combine(filePath, "policy.xml");

            System.IO.File.WriteAllLines(filePath, lines);

        }

    }
}
