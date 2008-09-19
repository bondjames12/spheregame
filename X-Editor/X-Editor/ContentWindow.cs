using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Input;
using System.Xml;

namespace X_Editor
{
    public partial class ContentWindow : Form
    {
        public EditorForm editor;

        public ContentWindow(EditorForm editor)
        {
            this.editor = editor;

            InitializeComponent();

            ContentFolder Root = new ContentFolder("Content", null, this);
            treeView1.Nodes.Add(Root.Node);
        }

        private void renameToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Text != "Content")
                treeView1.SelectedNode.BeginEdit();
        }

        int NewFolderNumber = 1;

        private void addFolderToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Tag is ContentFolder)
            {
                ((ContentFolder)treeView1.SelectedNode.Tag).AddChild("NewFolder" + NewFolderNumber);
                NewFolderNumber++;
                treeView1.SelectedNode.Expand();
                ((ContentFolder)treeView1.SelectedNode.Tag).Children[((ContentFolder)treeView1.SelectedNode.Tag).Children.Count - 1].Node.BeginEdit();
                treeView1.SelectedNode = ((ContentFolder)treeView1.SelectedNode.Tag).Children[((ContentFolder)treeView1.SelectedNode.Tag).Children.Count - 1].Node;
            }
        }

        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Label) && e.Label != " ")
            {
                if (e.Node.Tag is ContentFolder)
                {
                    if (System.IO.Directory.Exists(editor.ProjectDirectory + "\\Game" + ((ContentFolder)e.Node.Tag).GenerateFilename()))
                        if (!System.IO.Directory.Exists(editor.ProjectDirectory + "\\Game" + ((ContentFolder)e.Node.Tag).Parent.GenerateFilename() + "\\" + e.Label))
                            System.IO.Directory.Move(editor.ProjectDirectory + "\\Game" + ((ContentFolder)e.Node.Tag).GenerateFilename(), editor.ProjectDirectory + "\\Game" + ((ContentFolder)e.Node.Tag).Parent.GenerateFilename() + "\\" + e.Label);

                    ((ContentFolder)e.Node.Tag).Name = e.Label;
                    NewFolderNumber--;
                }
                else if (e.Node.Tag is ContentItem && ((ContentItem)e.Node.Tag).Parent is ContentFolder)
                {
                    ContentItem item = ((ContentItem)e.Node.Tag);

                    if (item.Built)
                        System.IO.File.Move(editor.ProjectDirectory + "\\Game" + item.GenerateFilename() + ".xnb", editor.ProjectDirectory + "\\Game" + ((ContentFolder)item.Parent).GenerateFilename() + "\\" + e.Label + ".xnb");
                    else
                        System.IO.File.Move(editor.ProjectDirectory + "\\Game" + item.GenerateFilename(), editor.ProjectDirectory + "\\Game" + ((ContentFolder)item.Parent).GenerateFilename() + "\\" + e.Label);

                    item.Name = e.Label;
                }
                else
                    e.CancelEdit = true;
            }
            else
                e.CancelEdit = true;
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView1.SelectedNode = e.Node;
        }

        private void deleteToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Text != "Content")
            {
                if (treeView1.SelectedNode.Tag is ContentFolder)
                    ((ContentFolder)treeView1.SelectedNode.Tag).Parent.RemoveChild(((ContentFolder)treeView1.SelectedNode.Tag), true, true);
                else if (treeView1.SelectedNode.Tag is ContentItem && ((ContentItem)treeView1.SelectedNode.Tag).Parent is ContentFolder)
                    ((ContentFolder)((ContentItem)treeView1.SelectedNode.Tag).Parent).RemoveContent((ContentItem)treeView1.SelectedNode.Tag);
            }
        }

        TreeNode DraggedItem;

        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (((TreeNode)e.Item).Text != "Content")
            {
                if (((TreeNode)e.Item).Tag is ContentItem)
                {
                    if (!(((ContentItem)((TreeNode)e.Item).Tag).Parent is ContentItem))
                    {
                        DraggedItem = (TreeNode)e.Item;
                        editor.draggedItem = ((TreeNode)e.Item).Tag;
                        DoDragDrop(e.Item.ToString(), DragDropEffects.Move);
                    }
                }
                else if (((TreeNode)e.Item).Tag is ContentFolder)
                {
                    DraggedItem = (TreeNode)e.Item;
                    DoDragDrop(e.Item.ToString(), DragDropEffects.Move);
                }
            }
        }

        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            if (DraggedItem.Tag is ContentFolder && treeView1.SelectedNode.Tag is ContentFolder)
                ((ContentFolder)DraggedItem.Tag).MoveTo((ContentFolder)treeView1.SelectedNode.Tag);
            else if (DraggedItem.Tag is ContentItem && treeView1.SelectedNode.Tag is ContentFolder)
                ((ContentItem)DraggedItem.Tag).MoveTo((ContentFolder)treeView1.SelectedNode.Tag);
        }

        private void treeView1_DragOver(object sender, DragEventArgs e)
        {
            treeView1.SelectedNode = treeView1.GetNodeAt(treeView1.PointToClient(Cursor.Position));
        }

        public void AddContentItem(string Filename, string[] Dependencies, bool Built, bool[] DependenciesBuilt)
        {
            ((ContentFolder)treeView1.SelectedNode.Tag).AddContent(Filename, Built);

            treeView1.SelectedNode.Expand();
            treeView1.SelectedNode = treeView1.SelectedNode.Nodes[treeView1.SelectedNode.Nodes.Count - 1];

            if (Dependencies != null)
            {
                for (int i = 0; i < Dependencies.Length; i++)
                    ((ContentItem)treeView1.SelectedNode.Tag).AddDependency(Dependencies[i], DependenciesBuilt[i]);
            }
        }

        private void addContentToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Tag is ContentFolder)
            {
                AddContentWindow content = new AddContentWindow(this, (ContentFolder)treeView1.SelectedNode.Tag);
                content.Show();
            }
        }

        public void WriteToXML(XmlTextWriter writer)
        {
            writer.WriteStartElement("Content");

            if (treeView1.Nodes.Count > 0)
            {
                foreach (TreeNode node in treeView1.Nodes[0].Nodes)
                {
                    if (node.Tag is ContentFolder)
                        ((ContentFolder)node.Tag).WriteToXML(writer);
                    else if (node.Tag is ContentItem)
                        ((ContentItem)node.Tag).WriteToXML(writer);
                }
            }

            writer.WriteEndElement();
        }

        public void LoadFromXML(XmlDocument doc)
        {
            ProcessNode(doc.DocumentElement.ChildNodes[0], treeView1.Nodes[0]);
            treeView1.Nodes[0].Expand();
        }

        void ProcessNode(XmlNode Node, TreeNode Current)
        {
            if (Node.Name != "Content")
            {
                if (Node.Attributes["Type"].InnerText == "Folder")
                    ((ContentFolder)Current.Tag).AddChild(Node.Attributes["Name"].InnerText);
                else if (Node.Attributes["Type"].InnerText == "Item" && Current.Tag is ContentFolder)
                    ((ContentFolder)Current.Tag).AddContent(Node.Attributes["Name"].InnerText, bool.Parse(Node.Attributes["Built"].InnerText));
                else if (Node.Attributes["Type"].InnerText == "Item" && Current.Tag is ContentItem)
                    ((ContentItem)Current.Tag).AddDependency(Node.Attributes["Name"].InnerText, bool.Parse(Node.Attributes["Built"].InnerText));

                foreach (XmlNode child in Node.ChildNodes)
                    ProcessNode(child, Current.Nodes[Current.Nodes.Count - 1]);
            }
            else
            {
                foreach (XmlNode child in Node.ChildNodes)
                    ProcessNode(child, Current);
            }
        }

        private void ContentWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public void Reset()
        {
            treeView1.Nodes.Clear();
            ContentFolder Root = new ContentFolder("Content", null, this);
            treeView1.Nodes.Add(Root.Node);
        }
    }

    public class ContentItem
    {
        public string Name;

        public List<ContentItem> Dependencies = new List<ContentItem>();
        public object Parent;

        public TreeNode Node;
        public bool Built;

        ContentWindow window;

        public ContentItem(object Parent, string Name, ContentWindow window, bool Built)
        {
            this.Parent = Parent;
            this.Name = Name;

            this.window = window;
            this.Built = Built;

            Node = new TreeNode(Name);
            Node.Tag = this;

            if (!Built)
            {
                Node.ImageIndex = 0;
                Node.SelectedImageIndex = 0;
            }
            else
            {
                Node.ImageIndex = 2;
                Node.SelectedImageIndex = 2;
            }
        }

        public void AddDependency(string Name, bool Built)
        {
            ContentItem item = new ContentItem(this, Name, window, Built);
            Dependencies.Add(item);
            Node.Nodes.Add(item.Node);
        }

        public void MoveTo(ContentFolder New)
        {
            if (Built)
                System.IO.File.Move(window.editor.ProjectDirectory + "\\Game" + GenerateFilename() + ".xnb", window.editor.ProjectDirectory + "\\Game" + New.GenerateFilename() + "\\" + Name + ".xnb");
            else
                System.IO.File.Move(window.editor.ProjectDirectory + "\\Game" + GenerateFilename(), window.editor.ProjectDirectory + "\\Game" + New.GenerateFilename() + "\\" + Name);

            foreach (ContentItem item in Dependencies)
            {
                if (item.Built)
                    System.IO.File.Move(window.editor.ProjectDirectory + "\\Game" + item.GenerateFilename() + ".xnb", window.editor.ProjectDirectory + "\\Game" + New.GenerateFilename() + "\\" + item.Name + ".xnb");
                else
                    System.IO.File.Move(window.editor.ProjectDirectory + "\\Game" + item.GenerateFilename(), window.editor.ProjectDirectory + "\\Game" + New.GenerateFilename() + "\\" + item.Name);
            }

            ((ContentFolder)Parent).RemoveContent(this);
            New.AcceptContent(this);
        }

        public string GenerateFilename()
        {
            string Filename = "\\" + Name;
            if (Parent is ContentFolder)
                Filename = ((ContentFolder)Parent).GenerateFilename() + Filename;
            else
                Filename = ((ContentFolder)((ContentItem)Parent).Parent).GenerateFilename() + Filename;

            return Filename;
        }

        public string GenerateFullPathFilename()
        {
            string Filename = "\\" + Name;
            if (Parent is ContentFolder)
                Filename = ((ContentFolder)Parent).GenerateFilename() + Filename;
            else
                Filename = ((ContentFolder)((ContentItem)Parent).Parent).GenerateFilename() + Filename;

            return window.editor.ProjectDirectory + "\\Game" + Filename;
        }

        public void Delete()
        {
            foreach (ContentItem item in Dependencies)
            {
                if (item.Built)
                    System.IO.File.Delete(window.editor.ProjectDirectory + "\\Game" + item.GenerateFilename() + ".xnb");
                else
                    System.IO.File.Delete(window.editor.ProjectDirectory + "\\Game" + item.GenerateFilename());
            }

            if (Built)
                System.IO.File.Delete(window.editor.ProjectDirectory + "\\Game" + GenerateFilename() + ".xnb");
            else
                System.IO.File.Delete(window.editor.ProjectDirectory + "\\Game" + GenerateFilename());
        }

        public void WriteToXML(XmlTextWriter writer)
        {
            writer.WriteStartElement("Node");
            writer.WriteAttributeString("Type", "Item");
            writer.WriteAttributeString("Name", Name);
            writer.WriteAttributeString("Built", Built.ToString());

            foreach (ContentItem item in Dependencies)
                item.WriteToXML(writer);

            writer.WriteEndElement();
        }
    }

    public class ContentFolder
    {
        public string Name;
        public List<ContentItem> Content = new List<ContentItem>();
        public List<ContentFolder> Children = new List<ContentFolder>();
        public ContentFolder Parent;

        public TreeNode Node;

        ContentWindow window;

        public ContentFolder(string Name, ContentFolder Parent, ContentWindow window)
        {
            this.Parent = Parent;
            this.Name = Name;

            this.window = window;

            Node = new TreeNode(Name);
            Node.Tag = this;
            Node.ImageIndex = 1;
            Node.SelectedImageIndex = 1;
        }

        public void AddChild(string Name)
        {
            ContentFolder Child = new ContentFolder(Name, this, window);
            Children.Add(Child);
            Node.Nodes.Add(Child.Node);

            if (!System.IO.Directory.Exists(window.editor.ProjectDirectory + "\\Game" + Children[Children.Count - 1].GenerateFilename()))
                System.IO.Directory.CreateDirectory(window.editor.ProjectDirectory + "\\Game" + Children[Children.Count - 1].GenerateFilename());
        }

        public void AddContent(string Name, bool Built)
        {
            ContentItem Item = new ContentItem(this, Name, window, Built);
            Content.Add(Item);
            Node.Nodes.Add(Item.Node);
        }

        public void RemoveChild(ContentFolder Child, bool Delete, bool RemoveChildren)
        {
            Node.Nodes.Remove(Child.Node);

            if (RemoveChildren)
                Child.RemoveAllChildren(Delete);

            //foreach (ContentItem item in Child.Content)
            //    item.Delete();

            Children.Remove(Child);

            if (Delete)
            {
                if (System.IO.Directory.Exists(window.editor.ProjectDirectory + "\\Game" + Child.GenerateFilename()))
                    System.IO.Directory.Delete(window.editor.ProjectDirectory + "\\Game" + Child.GenerateFilename());
            }
        }

        public void RemoveAllChildren(bool Delete)
        {
            List<ContentFolder> RemoveChildren = new List<ContentFolder>();

            foreach (ContentFolder child in Children)
            {
                child.RemoveAllChildren(Delete);
                RemoveChildren.Add(child);
            }

            foreach (ContentFolder child in RemoveChildren)
                RemoveChild(child, Delete, true);

            List<ContentItem> RemoveItems = new List<ContentItem>();

            foreach (ContentItem item in Content)
                RemoveItems.Add(item);

            foreach (ContentItem item in RemoveItems)
                RemoveContent(item);
        }

        public void RemoveContent(ContentItem Item)
        {
            Item.Delete();

            Node.Nodes.Remove(Item.Node);
            Content.Remove(Item);
        }

        public void AcceptChild(ContentFolder Child)
        {
            Node.Nodes.Add(Child.Node);
            Node.Expand();
            Children.Add(Child);
            Child.Parent = this;
        }

        public void AcceptContent(ContentItem Item)
        {
            Node.Nodes.Add(Item.Node);
            Node.Expand();
            Content.Add(Item);
            Item.Parent = this;
        }


        public void MoveTo(ContentFolder New)
        {
            if (System.IO.Directory.Exists(window.editor.ProjectDirectory + "\\Game" + GenerateFilename()))
                if (System.IO.Directory.Exists(window.editor.ProjectDirectory + "\\Game" + New.GenerateFilename()))
                    System.IO.Directory.Move(window.editor.ProjectDirectory + "\\Game" + GenerateFilename(), window.editor.ProjectDirectory + "\\Game" + New.GenerateFilename() + "\\" + Name);

            Parent.RemoveChild(this, false, false);
            New.AcceptChild(this);
        }

        public string GenerateFilename()
        {
            string Filename = "\\" + Name;

            if (Parent != null)
                Filename = Parent.GenerateFilename() + Filename;

            return Filename;
        }

        public void WriteToXML(XmlTextWriter writer)
        {
            writer.WriteStartElement("Node");
            writer.WriteAttributeString("Type", "Folder");
            writer.WriteAttributeString("Name", Name);

            foreach (ContentItem item in Content)
                item.WriteToXML(writer);

            foreach (ContentFolder folder in Children)
                folder.WriteToXML(writer);

            writer.WriteEndElement();
        }
    }
}
