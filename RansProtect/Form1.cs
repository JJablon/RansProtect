using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RansProtect
{
    public partial class Form1 : Form
    {
        public bool ran = false;
        public static long iteration = 0;

        public Form1()
        {
            InitializeComponent();
            /*try
            {
                System.Diagnostics.EventLog.CreateEventSource("RansProtect", "RansProtect");
            }
            catch (Exception) { }
            if (System.Diagnostics.EventLog.SourceExists("RansProtect"))
            {
                System.Diagnostics.EventLog.WriteEntry("RansProtect", "event1", System.Diagnostics.EventLogEntryType.Information);
                //eventLog1.WriteEntry("event1", System.Diagnostics.EventLogEntryType.SuccessAudit, 6666);
                eventLog1.WriteEvent(new System.Diagnostics.EventInstance(6666, 6666), "event1");
            }
            else { MessageBox.Show("aaa"); }*/
        }
        TreeNode structure = new TreeNode("ROOT", null);
        System.Windows.Forms.TreeNode root;
        private void fileSystemWatcher1_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            iteration++;
            if (!ran) {
                Stack<String> nodes = new Stack<String>();
                //this.listBox1.Items.Add("Changed:" + e.FullPath);
                FileInfo fi = new FileInfo(e.FullPath);
                DirectoryInfo parent = fi.Directory;
                do
                {


                    nodes.Push(parent.Name);


                    parent = Directory.GetParent(parent.FullName);
                }

                while (Path.GetPathRoot(e.FullPath) != parent.FullName);
                nodes.Push(parent.Name);
                var reversed = nodes.ToArray();
                //Array.Reverse(reversed);
                var currentNode = structure;
                foreach (string name in nodes)
                {
                    //listBox2.Items.Add(name);
                    if (currentNode != null)
                    {
                        if (!currentNode.Children.Contains(new TreeNode(name, null))) currentNode.Children.Add(new TreeNode(name, null));
                        else
                        {
                            currentNode.GetChildNode(name).Changed++;
                        }
                        currentNode = currentNode.GetChildNode(name);

                    }
                    else
                    {
                        listBox2.Items.Add("fatal error");
                    }
                }
                if(this.treeView1.Nodes == null || this.treeView1.Nodes.Count == 0)
                 root = this.treeView1.Nodes.Add("ROOT");

                //ran = true;
            }
        }
        private void Refresh()
        {

            TreeWalker.RefreshExpanded(structure);
            string selected = "";
            if(treeView1.SelectedNode != null)
            selected = treeView1.SelectedNode.FullPath;
            List<string> ExpandedNodes = new List<string>();
            if (treeView1 != null && treeView1.Nodes != null && treeView1.Nodes.Count > 0 && treeView1.Nodes[0].Nodes != null && treeView1.Nodes[0].Nodes.Count > 0)
            {


                System.Windows.Forms.TreeNode Nodes1 = treeView1.Nodes[0];
                structure.Expanded = true;
                TreeWalker.collectExpandedNodes(Nodes1.Nodes[0], structure);
            }

            treeView1.Nodes.Clear();
            if (this.treeView1.Nodes == null || this.treeView1.Nodes.Count == 0||!this.treeView1.Nodes.Contains(new System.Windows.Forms.TreeNode("ROOT")))
                root = this.treeView1.Nodes.Add("ROOT");
            treeView1.BeginUpdate();
            TreeWalker.RefreshAdded(structure);
            TreeWalker.Walk(structure, root);

            treeView1.EndUpdate();
            treeView1.Update();
            treeView1.Refresh();

            System.Windows.Forms.TreeNode Nodes2 = treeView1.Nodes[0];
            treeView1.Nodes[0].Expand();
            TreeWalker.ExpandNodes(Nodes2.Nodes[0], structure);
            if(selected != "")
             treeView1.SelectedNode =  TreeWalker.GetNodeFromPath(treeView1.Nodes, selected, treeView1) ;
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            Refresh();
            timer1.Interval = 1000;

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            TreeWalker.RefreshStats(structure);
            Refresh();

        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addToWhitelistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.TreeNode tn = this.treeView1.SelectedNode;

        }
    }

    static class TreeWalker
    {
        public static System.Windows.Forms.TreeNode GetNodeFromPath(TreeNodeCollection nodes, string path, TreeView tvPD2)
        {
            System.Windows.Forms.TreeNode foundNode = null;
            foreach (System.Windows.Forms.TreeNode tn in nodes)
            {
                if (tn.FullPath == path)
                {
                    tvPD2.SelectedNode = tn;
                    tvPD2.SelectedNode.EnsureVisible();
                    tvPD2.Focus();
                    return tn;
                }
                else if (tn.Nodes.Count > 0)
                {
                    foundNode = GetNodeFromPath(tn.Nodes, path,tvPD2);
                }
                if (foundNode != null)
                    return foundNode;
            }
            return null;
        }


        public static void collectExpandedNodes(System.Windows.Forms.TreeNode Nodes, TreeNode tn)
        {
            if(Nodes != null && Nodes.Nodes != null && Nodes.Nodes.Count > 0)
            { 
                
                //tn.Expanded = true;
                for (int i = 0; i < Nodes.Nodes.Count; i++)
                {
                    System.Windows.Forms.TreeNode checknode = Nodes.Nodes[i];
                    if (tn != null && tn.GetChildNode(checknode.Text) != null
                      )
                    {
                        if (checknode.IsExpanded)
                        {
                            tn.GetChildNode(checknode.Text).Expanded = true;
                            collectExpandedNodes(checknode, tn.GetChildNode(checknode.Text));
                        }
                    }
                }
                
            }
            
        }


        public static void ExpandNodes(System.Windows.Forms.TreeNode Nodes, TreeNode tn)
        {
            if (Nodes != null && Nodes.Nodes != null && Nodes.Nodes.Count > 0)
            {
                //Nodes = Nodes.Nodes[0];
                for (int i = 0; i < Nodes.Nodes.Count; i++)
                {
                    System.Windows.Forms.TreeNode checknode = Nodes.Nodes[i];
                    
                        if (tn!=null&& tn.Expanded == true) Nodes.Expand(); //checknode
                        //tn.GetChildNode(checknode.Text).Expanded = true;
                    if (tn != null &&  tn.GetChildNode(checknode.Text) != null   )
                    {
                        ExpandNodes(checknode, tn.GetChildNode(checknode.Text));
                    }
                }

            }

        }


        
        public static void Walk ( TreeNode tn, System.Windows.Forms.TreeNode tnc )
        {
            int dummy = 0;
            if (tnc != null)
            {
                System.Windows.Forms.TreeNode temp = null;
                //if (tnc.Nodes != null && !tnc.Nodes.Contains(new System.Windows.Forms.TreeNode(tn.Name))) temp = tnc.Nodes.Add(tn.Name);

                if (tn.AddedToTree == false) { tn.AddedToTree = true; temp = tnc.Nodes.Add(tn.Name);// tnc.TreeView.BeginUpdate();
                }
                else
                {
                    foreach (System.Windows.Forms.TreeNode iterating in tnc.Nodes)
                    {
                        if (iterating.Name == tn.Name) temp = iterating;
                    }
                    
                }

                if (tn.Changed > 10) temp.ForeColor = System.Drawing.Color.Green;
                if (tn.Changed > 50) temp.ForeColor = System.Drawing.Color.Yellow;
                if (tn.Changed > 100) temp.ForeColor = System.Drawing.Color.Orange;
                if (tn.Changed > 200) temp.ForeColor = System.Drawing.Color.DarkOrange;
                if (tn.Changed > 1000) temp.ForeColor = System.Drawing.Color.OrangeRed;
                if (tn.Changed > 2000) temp.ForeColor = System.Drawing.Color.Red;
                if (tn.Children != null && tn.Children.Count != 0)
                    foreach (TreeNode node in tn.Children)
                    {
                        TreeWalker.Walk(node, temp);
                        if (Form1.iteration > 100)
                            {
                             dummy++;
                            }
                    }
                 

            }

        }




        public static void RefreshAdded(TreeNode tn)
        {
                if (tn.Children != null && tn.Children.Count != 0)
                    foreach (TreeNode node in tn.Children)
                    {
                        node.AddedToTree = false;
                        TreeWalker.RefreshAdded(node);
                    }
                tn.AddedToTree = false;
            }
        public static void RefreshStats(TreeNode tn)
        {
            if (tn.Children != null && tn.Children.Count != 0)
                foreach (TreeNode node in tn.Children)
                {
                    node.Changed = 0;
                    TreeWalker.RefreshStats(node);
                }
            tn.AddedToTree = false;
        }

        public static void RefreshExpanded(TreeNode tn)
        {
            if (tn.Children != null && tn.Children.Count != 0)
                foreach (TreeNode node in tn.Children)
                {
                    node.Expanded = false;
                    TreeWalker.RefreshExpanded(node);
                }
            tn.AddedToTree = false;
        }






    }


    class TreeNode : IEquatable<TreeNode>
    {
        public int id;
        public bool AddedToTree { get; set; }
        public string Name { get; set; }
        public int Changed { get; set; }
        public bool Expanded { get; set; }
        //public TreeNode FirstChild { get;  set; }
        public List<TreeNode> Children { get; set; }
        //public TreeNode NextSibling { get;  set; }
        public TreeNode(string data, TreeNode firstChild//, TreeNode nextSibling
            )
        {
            this.AddedToTree = false;
            this.Name = data;
            //this.FirstChild = firstChild;
            Children = new List<TreeNode>();
            //this.NextSibling = nextSibling;
            Changed = 1;
            Expanded = false;
        }



        public TreeNode GetChildNode (string name)
        {
            if(this.Children.Contains(new TreeNode(name, null)))
            {
                int cnt = 0;
                    foreach (var o in this.Children)
                {
                    if (o.Name == name) return this.Children[cnt];
                    cnt++;
                }
                return null;

            }
            else
            {
                return null;
            }
            

        }



        public override string ToString()
        {
            return Name;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            TreeNode objAsPart = obj as TreeNode;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }
        public override int GetHashCode()
        {
            return id;
        }
        public bool Equals(TreeNode other)
        {
            if (other == null) return false;
            return (this.Name.Equals(other.Name));
        }




    }
}
