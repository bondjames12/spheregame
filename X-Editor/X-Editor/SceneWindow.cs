using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace X_Editor
{
    public partial class SceneWindow : Form
    {
        public EditorForm editor;

        public SceneWindow(EditorForm editor)
        {
            InitializeComponent();
            this.editor = editor;
        }

        int NumScenes = 0;

        public void AddScene(object sender, EventArgs e, bool CreateFile)
        {
            CheckSceneFile();
            Scene scene = new Scene("NewScene" + NumScenes.ToString(), this, CreateFile);
            scene.Item.BeginEdit();
            scene.Edit(false);
        }

        private void CheckSceneFile()
        {
            //check for a scene with this name
            if (System.IO.File.Exists(this.editor.ProjectDirectory + @"\Game\Scenes\NewScene" + NumScenes.ToString() + ".xml"))
            {
                NumScenes++;
                CheckSceneFile();
            }
        }

        private void SceneWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void listView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Label) || e.Label == " ")
                e.CancelEdit = true;
            else
                ((Scene)listView1.SelectedItems[0].Tag).Name = e.Label;
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
                listView1.SelectedItems[0].BeginEdit();
        }

        private void addSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddScene(sender, e, true);
        }

        private void deleteSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
                ((Scene)listView1.SelectedItems[0].Tag).Delete();
        }

        private void editSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
                ((Scene)listView1.SelectedItems[0].Tag).Edit(true);
        }

        public void LoadFromXML(XmlNodeList Scenes)
        {
            foreach (XmlNode node in Scenes)
                new Scene(node.Attributes["Name"].InnerText, this, false);

            if (listView1.Items.Count > 0)
                ((Scene)listView1.Items[0].Tag).Edit(true);
        }

        public void WriteToXML(XmlTextWriter Writer)
        {
            Writer.WriteStartElement("Scenes");
            foreach (ListViewItem scene in listView1.Items)
                ((Scene)scene.Tag).SaveToXML(Writer, false);
            Writer.WriteEndElement();
        }
    }

    public class Scene
    {
        string name;
        public string Name
        {
            get { return name; }
            set 
            { 
                if (Item != null)
                {
                    Item.Text = value;
                    System.IO.File.Move(window.editor.ProjectDirectory + @"\Game\Scenes\" + name + ".xml", window.editor.ProjectDirectory + @"\Game\Scenes\" + value + ".xml");
                }
                name = value;
            }
        }

        public ListViewItem Item;

        public bool Loaded = false;

        SceneWindow window;

        public Scene(string Name, SceneWindow window, bool CreateFile)
        {
            this.Name = Name;
            this.window = window;

            Item = new ListViewItem(Name);
            Item.Tag = this;
            Item.ImageIndex = 0;
            Item.StateImageIndex = 0;
            window.listView1.Items.Add(Item);

            if (CreateFile)
                SaveToXML(null, true);
        }

        public void Delete()
        {
            window.listView1.Items.Remove(Item);
            System.IO.File.Delete(window.editor.ProjectDirectory + @"\Game\Scenes\" + Name + ".xml");

            if (window.editor.CurrentScene == this)
            {
                window.editor.scene.Clear();
                window.editor.CurrentScene = null;
            }
        }

        public void LoadFromXML()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(window.editor.ProjectDirectory + "\\Game\\Scenes\\" + Name + ".xml");

            window.editor.ResetEditor(false);
            window.editor.plugins.LoadFromXML(doc.DocumentElement.ChildNodes, window.editor.scene);

            Loaded = true;
        }

        public void Edit(bool Load)
        {
            if (window.editor.CurrentScene != null)
                window.editor.CurrentScene.SaveToXML(null, true);

            window.editor.ResetEditor(false);

            if (!Loaded && Load)
                LoadFromXML();

            window.editor.CurrentScene = this;
        }

        public void SaveToXML(XmlTextWriter projectWriter, bool ForceSave)
        {
            if (projectWriter != null)
            {
                projectWriter.WriteStartElement("Scene");
                projectWriter.WriteAttributeString("Name", Name);
                projectWriter.WriteEndElement();
            }

            if (window.editor.CurrentScene == this || ForceSave)
            {
                XmlTextWriter writer = new XmlTextWriter(window.editor.ProjectDirectory + "\\Game\\Scenes\\" + Name + ".xml", Encoding.UTF8);
                writer.Formatting = Formatting.Indented;

                writer.WriteStartDocument();
                writer.WriteStartElement("Scene");

                window.editor.plugins.WriteToXML(writer, window.editor.scene);

                writer.WriteEndElement();
                writer.WriteEndDocument();

                writer.Flush();
                writer.Close();
            }

            Loaded = false;
        }
    }
}
