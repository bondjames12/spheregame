using System;
using System.Windows.Forms;
using XEngine;
using Microsoft.Xna.Framework.Content;
using System.Drawing;

namespace X_Editor
{
    public partial class EditorForm : Form
    {
        ProjectFileManager projectFileManager;
        public ComponentPluginManager plugins;
        public ContentWindow content;
        public SceneWindow scenes;

        Scene currentScene;
        public Scene CurrentScene
        {
            get { return currentScene; }
            set { currentScene = value; if (value != null) { label1.Text = value.Name + " Components"; } }
        }

        public string ProjectDirectory;

        public EditorForm()
        {
            InitializeComponent();

            projectFileManager = new ProjectFileManager(this);
        }

        OpenPopup open;

        private void OnLoad(object sender, EventArgs e)
        {
            treeView1.Nodes["root"].Expand();
            
            open = new OpenPopup(this);
            open.Show();
            open.Select();
            open.Focus();
            open.Location = new Point(Location.X + Width / 2 - open.Width / 2, Location.Y + Height / 2 - open.Height / 2);

            content = new ContentWindow(this);
            scenes = new SceneWindow(this);

            plugins = new ComponentPluginManager(renderControl1.X);

            //add the different component plugins to the component list box!
            foreach (ComponentPlugin plugin in plugins.Plugins)
                treeView1.Nodes[0].Nodes.Add(plugin.type.ToString());
        }
        //shortcut methods
        public XComponent GetXComponent(string ID)
        {
            return renderControl1.X.Tools.GetXComponentByID(ID);
        }

        public XComponent GetXComponent(uint ID)
        {
            return renderControl1.X.Tools.GetXComponentByID(ID);
        }

        //events
        private void renderControl1_MouseEnter(object sender, EventArgs e)
        {
            renderControl1.hasFocus = true;
            renderControl1.Select();
        }

        private void renderControl1_MouseLeave(object sender, EventArgs e)
        {
            renderControl1.hasFocus = false;
        }

        TreeNode draggedComponent;

        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeNode Node = (TreeNode)e.Item;

            if ((string)Node.Tag != "group")
            {
                draggedComponent = Node;
                DoDragDrop(e.Item.ToString(), DragDropEffects.Move);
            }
        }

        private void renderControl1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Move;
        }

        private void renderControl1_DragDrop(object sender, DragEventArgs e)
        {
            ListViewItem item = new ListViewItem();
            renderControl1.dragdroprelease = true;

            item = plugins.SetUpListViewItem(draggedComponent.Text);

            ListViewItem sceneitem = new ListViewItem();
            sceneitem = plugins.SetUpListViewItem(draggedComponent.Text);

            scene.Items.Add(item);
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (scene.SelectedItems.Count > 0)
            {
                properties.SelectedObject = renderControl1.X.Tools.GetXComponentByID(scene.SelectedItems[0].SubItems["colID"].Text);
                tabControl1.SelectTab(1);
                if (properties.SelectedObject == null)
                    MessageBox.Show("The selected item no longer exists. Debug this", "Error");
            }
        }

        public void RefreshPropertiesTab()
        {
            properties.Refresh();
        }

        private void properties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            plugins.UpdateObjectProperties(properties.SelectedObject, properties, scene);
        }

        public object draggedItem;

        private void scene_ItemDrag(object sender, ItemDragEventArgs e)
        {//we are draging and item from the scene list
            ListViewItem item = (ListViewItem)e.Item;
            //find and store the xcomponent of this scene object
            draggedItem = GetXComponent(item.SubItems["colID"].Text);

            DoDragDrop(e.Item.ToString(), DragDropEffects.Move);
        }

        private void scene_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        private void properties_DragDrop(object sender, DragEventArgs e)
        {
            plugins.AcceptDragDrop(properties.SelectedObject, draggedItem, properties, scene);   
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this component from the scene?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                if (scene.SelectedItems.Count > 0)
                {
                    renderControl1.X.Components.Remove(GetXComponent(scene.SelectedItems[0].SubItems["colID"].Text));
                    scene.SelectedItems[0].Remove();
                }
            }
        }

        public void ResetEditor(bool ClearContent)
        {
            renderControl1.Tag = this;
            //Set this ContentRootDir before called Init() This makes sure the content manager is pointing to our project directory
            renderControl1.ContentRootDir = ProjectDirectory + @"\Game";
            renderControl1.Init();
            renderControl1.SetupBaseComponents();
            
            //save this X in the plugins manager as well
            plugins.X = renderControl1.X;

            CurrentScene = null;
            scene.Items.Clear(); ;
            properties.SelectedObject = null;
            draggedComponent = null;
            draggedItem = null;
            foreach (TreeNode node in treeView1.Nodes)
                node.Collapse();
            treeView1.Nodes["root"].Expand();

            if (ClearContent)
                content.Reset();
        }

        public void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                string Folder = System.IO.Path.GetFileNameWithoutExtension(saveFileDialog1.FileName);
                ProjectDirectory = System.IO.Path.GetDirectoryName(saveFileDialog1.FileName) + @"\" + Folder;
                ProjectFilePath = ProjectDirectory + "\\" + System.IO.Path.GetFileName(saveFileDialog1.FileName);
                ResetEditor(true);
                open.Close();

                System.IO.Directory.CreateDirectory(ProjectDirectory + @"\Game");
                System.IO.Directory.CreateDirectory(ProjectDirectory + @"\Game\Content");
                System.IO.Directory.CreateDirectory(ProjectDirectory + @"\Game\Scripts");
                System.IO.Directory.CreateDirectory(ProjectDirectory + @"\Game\Scenes");

                Tools.copyDirectory(GetType().Assembly.CodeBase.Replace("file:///", "").Replace("X-Editor.EXE", "") + @"Content", ProjectDirectory + @"\Game\Content\");

                scenes.AddScene(null, null, true);
                projectFileManager.SaveProjectFile(ProjectFilePath, plugins);
                Cursor = Cursors.Default;

                this.Enabled = true;
                this.Focus();
            }
            else
                open.Focus();
        }

        public string ProjectFilePath;

        private void saveLevelFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            projectFileManager.SaveProjectFile(ProjectFilePath, plugins);
        }

        public void openLeveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ProjectDirectory = System.IO.Path.GetDirectoryName(openFileDialog1.FileName);

                ProjectFilePath = openFileDialog1.FileName;

                ResetEditor(true);

                projectFileManager.LoadProjectFile(openFileDialog1.FileName, plugins);

                open.Close();

                this.Enabled = true;
                this.Focus();
            }
            else
                open.Focus();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help help = new Help();
            help.Show();
        }

        private void EditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;

            DialogResult result = MessageBox.Show("Would you like to save your changes?", "Project Management", MessageBoxButtons.YesNoCancel);

            if (result == DialogResult.Yes)
                projectFileManager.SaveProjectFile(ProjectFilePath, plugins);

            if (result != DialogResult.Cancel)
                e.Cancel = false;
        }

        private void contentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            content.Show();
        }

        private void scenesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scenes.Show();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void btnRotate_Click(object sender, EventArgs e)
        {
            if (btnRotate.Checked)
                renderControl1.mManipulator.EnabledModes |= TransformationMode.Rotation;
            else
                renderControl1.mManipulator.EnabledModes &= ~TransformationMode.Rotation;
        }

        private void btnScale_Click(object sender, EventArgs e)
        {
            if (btnScale.Checked)
                renderControl1.mManipulator.EnabledModes |= TransformationMode.ScaleAxis;
            else
                renderControl1.mManipulator.EnabledModes &= ~TransformationMode.ScaleAxis;
        }

        private void btnTranslate_CheckStateChanged(object sender, EventArgs e)
        {
            if (btnTranslate.Checked)
                renderControl1.mManipulator.EnabledModes |= TransformationMode.TranslationAxis;
            else
                renderControl1.mManipulator.EnabledModes &= ~TransformationMode.TranslationAxis;
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void renderControl1_OnSelectedComponentChange(object sender, XComponent selectedObj)
        {
            properties.SelectedObject = selectedObj;
            tabControl1.SelectTab(1);
        }

       
    }
}
