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
using System.Windows.Forms.VisualStyles;

namespace CS_App
{
    public partial class Form1 : Form
    {
        public List<List<string>> fileContent = null;

        public Form1() { InitializeComponent(); }

        private void Form1_Load(object sender, EventArgs e) { }
        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            var file = new OpenFileDialog();
            file.Filter = "Audit files (*.audit)|*.audit";
            if (file.ShowDialog() == DialogResult.OK)
            {
                Parser parser = new Parser(file.FileName);
                StreamReader tmp = new StreamReader(file.FileName);
                foreach (List<string> s in parser.getParsedText())
                    foreach (string str in s)
                        if (str.Trim().StartsWith("description"))
                            listView1.Items.Add(str.Replace("description", ""));
                fileContent = parser.getParsedText();
            }
            textBox1.Text = Path.GetFileName(file.FileName);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SBT");
            System.IO.Directory.CreateDirectory(filePath);
            filePath = Path.Combine(filePath, textBox1.Text);
            System.IO.File.WriteAllText(filePath, "");
            foreach (int index in listView1.CheckedIndices)
                System.IO.File.AppendAllLines(filePath, fileContent[index]);
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            if (button3.Text == "Select All")
            {
                CallRecursive(true);
                button3.Text = "Deselect All";
            }
            else
            {
                CallRecursive(false);
                button3.Text = "Select All";
            }
        }
        private void CallRecursive(bool value)
        {
            foreach (ListViewItem item in listView1.Items)
                item.Checked = value;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Insert desired word", "Search");
            foreach (ListViewItem item in listView1.Items)
            {
                item.BackColor = Color.White;
                if (item.ToString().IndexOf(input, StringComparison.OrdinalIgnoreCase) >= 0)
                    item.BackColor = Color.DeepSkyBlue;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            List<List<string>> selectedPolicies = new List<List<string>>();
            List<string> policy = new List<string>();

            foreach (int index in listView1.CheckedIndices)
            {
                foreach (string s in fileContent[index])
                    policy.Add(s);
                selectedPolicies.Add(policy);
                policy = new List<string>();
            }

            Scanner scanner = new Scanner(selectedPolicies);
        }
    }
}
