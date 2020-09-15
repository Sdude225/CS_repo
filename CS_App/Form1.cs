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
        public String globalFilePath = String.Empty;

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
            if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var fileStream = file.OpenFile();
                textBox1.Text = file.SafeFileName;
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    fileContent = reader.ReadToEnd();
                }


                if (fileContent[0] == '#')
                {
                    Parser();
                }
                else
                {
                    string[] tmp = fileContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    fText = new List<string>(tmp);
                }

                var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SBT");
                System.IO.Directory.CreateDirectory(filePath);
                filePath = Path.Combine(filePath, textBox1.Text);
                globalFilePath = filePath;

                System.IO.File.WriteAllLines(filePath, fText);

                treeView1.Nodes.Clear();
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);
                treeView1.Nodes.Add(new TreeNode(doc.DocumentElement.Name));
                TreeNode tr = new TreeNode();
                tr = treeView1.Nodes[0];
                AddNode(doc.DocumentElement, tr);
                if(doc.GetElementsByTagName("check_type")[0].Attributes["type"] != null)
                {
                    label1.Text = doc.GetElementsByTagName("check_type")[0].Attributes["type"].Value;
                    label2.Text = doc.GetElementsByTagName("group_policy")[0].Attributes["group"].Value;
                }
                
                button3.Visible = true;
                button4.Visible = true;
            }

        }

        private bool isComment(string line)
        {
            return line.StartsWith("#");
        }

        private bool removeAnd(string line)
        {
            return line.Contains("&");
        }

        private string replaceFirst(string line, string text, string replace)
        {
            int index = line.IndexOf(text);
            if (index < 0)
            {
                return line;
            }

            return line.Substring(0, index) + replace + line.Substring(index, text.Length);
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

            for (m = k + 1; m < j; m++)
            {
                if (lines[m].Contains("check_type"))
                {
                    lines[m] = lines[m].Replace("check_type", "<check_type>");
                    lines[m] = lines[m].Replace(":", "");
                    lines[m] = lines[m].Insert(lines[m].Length - 2, "</check_type>");
                    continue;
                }
                else if (lines[m].Contains("value_type"))
                {
                    lines[m] = lines[m].Replace("value_type", "<value_type>");
                    lines[m] = lines[m].Replace(":", "");
                    lines[m] = lines[m].Insert(lines[m].Length - 2, "</value_type>");
                    continue;
                }
                else if (lines[m].Contains("value_data"))
                {
                    lines[m] = lines[m].Replace("value_data", "<value_data>");
                    lines[m] = lines[m].Replace(":", "");
                    lines[m] = lines[m].Replace('"', ' ');
                    lines[m] = lines[m].Insert(lines[m].Length - 2, "</value_data>");
                    continue;
                }
                else if (lines[m].Contains("  description"))
                {
                    lines[m] = lines[m].Replace("description", "<description>");
                    lines[m] = lines[m].Replace(":", "");
                    lines[m] = lines[m].Replace('"', ' ');
                    lines[m] = lines[m].Insert(lines[m].Length - 2, "</description>");
                    continue;
                }
                else if (lines[m].Contains("reg_key"))
                {
                    lines[m] = lines[m].Replace("reg_key", "<reg_key>");
                    lines[m] = lines[m].Replace(":", "");
                    lines[m] = lines[m].Replace('"', ' ');
                    lines[m] = lines[m].Insert(lines[m].Length - 2, "</reg_key>");
                    continue;
                }
                else if (lines[m].Contains("reg_item"))
                {
                    lines[m] = lines[m].Replace("reg_item", "<reg_item>");
                    lines[m] = lines[m].Replace(":", "");
                    lines[m] = lines[m].Replace('"', ' ');
                    lines[m] = lines[m].Insert(lines[m].Length - 2, "</reg_item>");
                    continue;
                }
                else if (lines[m].Contains("  type"))
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
                else if (lines[m].Contains("  solution"))
                {
                    lines[m] = lines[m].Replace("solution", "<solution>");
                    lines[m] = lines[m].Replace(":", "");
                    lines[m] = lines[m].Replace('"', ' ');
                    m++;
                    while (char.IsLetterOrDigit(lines[m][0]))
                    {
                        m++;
                    }
                    lines[m - 1] = lines[m - 1].Replace('"', ' ');
                    lines[m - 1] = lines[m - 1].Insert(lines[m - 1].Length - 2, "</solution>");
                    m--;
                    continue;
                }
                else if (lines[m].Contains("   info"))
                {
                    lines[m] = replaceFirst(lines[m], "info", "<info>");
                    lines[m] = lines[m].Replace(":", "");
                    lines[m] = lines[m].Replace('"', ' ');
                    m++;
                    while (char.IsLetterOrDigit(lines[m][0]))
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


        private void Parser()
        {
            string[] tmp = fileContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var lines = new List<string>(tmp);
            lines.RemoveAll(isComment);
            lines.RemoveAll(removeAnd);

            for (int i = 0; i < lines.Count; i++)
            {
                lines[i] += "\r\n";
            }

            int k = 0;
            for (int i = 2; i < lines.Count - 2; i++)
            {
                if (lines[i].Contains("<custom_item"))
                {
                    string ciId = "<custom_item id=" + '"' + k.ToString() + '"' + ">";
                    k++;
                    lines[i] = lines[i].Replace("<custom_item>", ciId);
                    i++;
                    while (!lines[i].Contains("</custom_item>"))
                        i++;
                    continue;
                }
                else
                {
                    lines.RemoveAt(i);
                    i--;
                }

            }

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Contains(":") && lines[i].Contains("<"))
                {
                    lines[i] = lines[i].Replace(":", "=");
                }
                else if (lines[i].Contains("<custom_item"))
                {
                    custom_item(ref lines, ref i);
                }
            }

            int index = lines[0].IndexOf("version");
            lines[0] = lines[0].Remove(22, (lines[0].Length - 3 - index)).Insert(11, " type");

            lines[1] = lines[1].Insert(13, " group");
            textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 5, 5);
            textBox1.Text += ".audit";
            fText = lines;
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
            List<int> index = new List<int>();
            getCheckedNodes(treeView1.Nodes[0].Nodes[0].Nodes, ref index);

            XmlDocument doc = new XmlDocument();
            doc.Load(globalFilePath);
            XmlNode root = doc.DocumentElement;
            List<XmlNode> nodeList = new List<XmlNode>();

            foreach (int i in index)
            {
                string xpath = "/check_type/group_policy/custom_item[@id=\'" + i + "\']";
                XmlNode node = root.SelectSingleNode(xpath);
                nodeList.Add(node);
            }

            XmlDocument newDoc = new XmlDocument();
            newDoc.LoadXml("<root> </root>");

            foreach(XmlNode xn in nodeList)
            {
                XmlNode importNode = newDoc.ImportNode(xn, true);
                newDoc.DocumentElement.AppendChild(importNode);
            }

            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SBT");
            System.IO.Directory.CreateDirectory(filePath);
            filePath = Path.Combine(filePath, textBox1.Text);

            newDoc.Save(filePath);


        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (button3.Text == "Select All")
            {
                CallRecursive(treeView1, true);
                button3.Text = "Deselect All";
            }
            else
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

        private void button4_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Insert desired word", "Search");

            if (input.Length == 0)
                button2_Click(sender, e);

            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SBT");
            System.IO.Directory.CreateDirectory(filePath);
            filePath = Path.Combine(filePath, textBox1.Text);
            System.IO.File.WriteAllLines(filePath, fText);

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
            XmlNodeList nodelist;
            XmlNode root = doc.DocumentElement;
            string desiredNodes = "//custom_item[contains(description, '" + input + "')]";
            nodelist = root.SelectNodes(desiredNodes);

            if (nodelist.Count == 0)
            {
                MessageBox.Show("Policies with such description do not exist in current file");
                button2_Click(sender, e);
            }

            treeView1.Nodes.Clear();
            TreeNode treenode;
            treenode = treeView1.Nodes.Add("Search results");

            foreach (XmlNode xn in nodelist)
            {
                string text = xn.Name;
                string text1 = xn.Value;

                treenode = treeView1.Nodes.Add(text1, text);

                addChildNodes(xn, treenode);
            }
        }
        private void addChildNodes(XmlNode oldXn, TreeNode OldTreenode)
        {
            TreeNode treeNode = null;
            XmlNodeList nodeList = oldXn.ChildNodes;
            string text = null;

            foreach (XmlNode xn in nodeList)
            {
                if (xn.HasChildNodes)
                    text = xn.Name;
                else
                    text = xn.Value;
                string text1 = xn.Value;
                treeNode = OldTreenode.Nodes.Add(text1, text);
                addChildNodes(xn, treeNode);
            }
        }

        private void getCheckedNodes(TreeNodeCollection nodes, ref List<int> index)
        {
            foreach(TreeNode node in nodes)
            {
                if (!node.Checked)
                    continue;

                index.Add(node.Index);

                if (node.Nodes.Count != 0)
                    getCheckedNodes(node.Nodes, ref index);
            }
        }

        
    }

    }
