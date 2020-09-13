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

        private Tuple<string, string> splitLine(string line)
        {
            string[] words = line.Split(':');
            string tmp = null;
            for (int i = 1; i < words.Length; i++)
            {
                if (i == words.Length - 1)
                {
                    tmp += words[i];
                    break;
                }
                tmp += words[i] + ":";
            }
            return new Tuple<string, string>(words[0], tmp);
        }

        private void parseFile(string filePath)
        {
            string line;
            string parsedText = null;
            bool flg = true;
            List<string> onelist = new List<string>();
            List<List<string>> fList = new List<List<string>>();
            System.IO.StreamReader file = new System.IO.StreamReader(filePath);
            Regex r0 = new Regex("\\s*\\<\\/\\b(report)\\b", RegexOptions.Compiled);
            Regex r1 = new Regex("\\s*\\<\\/\\b(then)\\b", RegexOptions.Compiled);
            Regex r2 = new Regex("^\\s+\\w+\\s*\\:", RegexOptions.Compiled);
            Regex r3 = new Regex("\\s*\\<\\/\\b(custom_item)\\b", RegexOptions.Compiled);
            while ((line = file.ReadLine()) != null )
            {
                if (r0.IsMatch(line)) { flg = !flg; continue; }
                if (line.StartsWith("#") || line == "" || flg) continue;
                if (r1.IsMatch(line)) break;
                if (r2.IsMatch(line) && parsedText !=null)
                {
                    onelist.Add(parsedText);
                    parsedText = line;
                    continue;
                }
                if (r3.IsMatch(line))
                {
                    onelist.Add(parsedText);
                    fList.Add(onelist);
                    parsedText = null;
                    onelist = new List<string>();
                    continue;
                }
                parsedText += line + '\n';
            }
            file.Close();
            foreach (List<string> l in fList)
                l.RemoveAt(0);
            TreeView tr = new TreeView();
            tr.Nodes.Add(new System.Windows.Forms.TreeNode().Text = "some");
            tr.Nodes.Add(new System.Windows.Forms.TreeNode().Text = "anotherthing");
            foreach (List<string> ls in fList)
                listBox1.Items.Add(tr);
                System.Console.WriteLine("\nSize of list is : " + fList[1][1]);
        }
        //CIS_MS_Windows_10_Enterprise_Next_Generation_Windows_Security_v1.6.1.audit
        private void button1_Click(object sender, EventArgs e)
        {
            var file = new OpenFileDialog();
            file.Filter = "Audit files (*.audit)|*.audit";
            if(file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                parseFile(file.FileName);
            }
        }

        

        private void AddNode(XmlNode inXmlNode, TreeNode inTreeNode)
        {
            XmlNode xNode;
            TreeNode tNode;
            XmlNodeList nodeList;
            int i;
            if (inXmlNode.HasChildNodes)
            {
                nodeList = inXmlNode.ChildNodes;

                for (i = 0; i <= nodeList.Count - 1; i++)
                {
                    xNode = inXmlNode.ChildNodes[i];
                    inTreeNode.Nodes.Add(new TreeNode(xNode.Name));
                    tNode = inTreeNode.Nodes[i];
                    this.AddNode(xNode, tNode);
                }
            }
            else
            {
                inTreeNode.Text = (inXmlNode.OuterXml).Trim();
            }
        }

        List<string> fText = null;
        private void button2_Click(object sender, EventArgs e)
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SBT");
            System.IO.Directory.CreateDirectory(filePath);
            filePath = Path.Combine(filePath, textBox1.Text);

            System.IO.File.WriteAllLines(filePath, fText);

            treeView1.Nodes.Clear();
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
            treeView1.Nodes.Add(new TreeNode(doc.DocumentElement.Name));
            TreeNode tr = new TreeNode();
            tr = treeView1.Nodes[0];
            AddNode(doc.DocumentElement, tr);
            label1.Text = doc.GetElementsByTagName("check_type")[0].Attributes["type"].Value;
            label2.Text = doc.GetElementsByTagName("group_policy")[0].Attributes["group"].Value;
            button3.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (button3.Text == "Select All")
            {
                CallRecursive(treeView1, true);
                button3.Text = "Deselect All";
            }else
            {
                CallRecursive(treeView1, false);
                button3.Text = "Select All";
            }
        }

        private void PrintRecursive(TreeNode treeNode, bool fg)
        {
            treeNode.Checked = fg;
            foreach (TreeNode tn in treeNode.Nodes)
            {
                PrintRecursive(tn, fg);
            }
        }
  
        private void CallRecursive(TreeView treeView, bool fg)
        {
            TreeNodeCollection nodes = treeView.Nodes;
            foreach (TreeNode n in nodes)
            {
                PrintRecursive(n, fg);
            }
        }
    }
}
