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

        enum keywords { custom_item, type, description, value_data, value_type };
            
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
            Item tempItem;
            while ((line = file.ReadLine()) != null )
            {
                //if (r0.IsMatch(line)) { flg = !flg; continue; }
                if (line.StartsWith("#") || line == "" || flg) continue;
                /*if (r1.IsMatch(line)) break;
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
                }*/
                parsedText += line + '\n';
                if ()
            }
            file.Close();
            foreach (List<string> l in fList)
                l.RemoveAt(0);
            for (int i = 0; i < fList.Count; i++)
                treeView1.Nodes.Add("custom_item_" + i);
            for (int i = 0; i < fList.Count; i++)
                for (int j = 0; j < fList[i].Count; j++)
                    treeView1.Nodes[i].Nodes.Add(fList[i][j]);
            System.Console.WriteLine("\nSize of list is : " + fList.Count);
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
            List<List<string>> tmp = null;
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SBT");
            System.IO.Directory.CreateDirectory(filePath);
            filePath = Path.Combine(filePath, textBox1.Text);
            /*
            FileStream fs = System.IO.File.OpenWrite(filePath);
            for (int i = 0; i < fList.Count; i++)
                for (int j = 0; j < fList[i].Count; j++)
                    fs.Write(fList[i][j]);*/
            Stream file = File.Open(filePath, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            object obj = bf.Deserialize(file);
            TreeNode[] nodeList = (obj as IEnumerable<TreeNode>).ToArray();
            treeView1.Nodes.AddRange(nodeList);
            file.Close();
            //System.IO.File.WriteAllLines(filePath, treeView1.Nodes.Cast<TreeNode>().ToArray());
            // LoadTree(treeView1, filePath);
            button3.Visible = true;
        }

        public static void SaveTree(TreeView tree, string filename)
        {
            using (Stream file = File.Open(filename, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(file, tree.Nodes.Cast<TreeNode>().ToList());
            }
        }

        public static void LoadTree(TreeView tree, string filename)
        {
            using (Stream file = File.Open(filename, FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                object obj = bf.Deserialize(file);

                TreeNode[] nodeList = (obj as IEnumerable<TreeNode>).ToArray();
                tree.Nodes.AddRange(nodeList);
            }
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
