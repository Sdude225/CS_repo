using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

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


        //CIS_MS_Windows_10_Enterprise_Next_Generation_Windows_Security_v1.6.1.audit
        private void button1_Click(object sender, EventArgs e)
        {
            var file = new OpenFileDialog();
            file.Filter = "Audit files (*.audit)|*.audit";
            if(file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Parser parser = new Parser(file.FileName);
                foreach (List<string> s in parser.getParsedText())
                    foreach (string str in s)
                        if (str.Trim().StartsWith("description"))
                            checkedListBox1.Items.Add(str.Replace("description", ""));
            }
        }

        


        private void button2_Click(object sender, EventArgs e)
        {
            
        }


        private void button3_Click(object sender, EventArgs e)
        {
/*            if (button3.Text == "Select All")
            {
                CallRecursive(treeView1, true);
                button3.Text = "Deselect All";
            }else
            {
                CallRecursive(treeView1, false);
                button3.Text = "Select All";
            }*/
        }
    }
}
