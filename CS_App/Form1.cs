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
using System.Xml;

namespace CS_App
{
    public partial class Form1 : Form
    {
        public List<List<string>> fileContent = null;
            
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
            checkedListBox1.Items.Clear();

            var file = new OpenFileDialog();
            file.Filter = "Audit files (*.audit)|*.audit";
            if(file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Parser parser = new Parser(file.FileName);
                System.IO.StreamReader tmp = new System.IO.StreamReader(file.FileName);

                foreach (List<string> s in parser.getParsedText())
                    foreach (string str in s)
                        if (str.Trim().StartsWith("description"))
                            checkedListBox1.Items.Add(str.Replace("description", ""));
                checkedListBox1.SetSelected(checkedListBox1.SelectedIndex, false);
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
            

            foreach (int index in checkedListBox1.CheckedIndices)
                System.IO.File.AppendAllLines(filePath, fileContent[index]);
            
        }

        private void CallRecursive(bool value)
        {
            for(int i = 0; i < checkedListBox1.Items.Count; i++)
                checkedListBox1.SetItemChecked(i, value);
        }


        private void button3_Click(object sender, EventArgs e)
        {
            if(button3.Text == "Select All")
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

        private void button4_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Insert desired word", "Search");

            if (input.Length == 0)
            {
                checkedListBox1.Items.Clear();
                foreach (List<string> s in fileContent)
                    foreach (string line in s)
                        if (line.Trim().StartsWith("description"))
                            checkedListBox1.Items.Add(line.Replace("description", ""));
            }

            List<string> searchedItems = new List<string>();

            foreach(string item in checkedListBox1.Items)
            {
                if (item.IndexOf(input, StringComparison.OrdinalIgnoreCase) >= 0)
                    searchedItems.Add(item);
            }

            if(searchedItems.Count == 0)
            {
                MessageBox.Show("Search querry yeilded no result, try again");
                button4_Click(sender, e);
            }

            checkedListBox1.Items.Clear();

            foreach (string s in searchedItems)
                checkedListBox1.Items.Add(s);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            List<List<string>> selectedPolicies = new List<List<string>>();

            Scanner scanner = new Scanner(selectedPolicies);
        }
    }
}
