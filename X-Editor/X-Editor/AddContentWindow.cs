using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace X_Editor
{
    public partial class AddContentWindow : Form
    {
        ContentBuilder builder;
        ContentFolder folder;
        ContentWindow window;

        public AddContentWindow(ContentWindow window, ContentFolder folder)
        {
            InitializeComponent();

            builder = new ContentBuilder();

            this.window = window;
            this.folder = folder;
        }

        private void browse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                filename.Text = openFileDialog1.FileName;
        }

        private void build_Click(object sender, EventArgs e)
        {
            if (importer.Text != "None")
            {
                builder.Clear();

                //System.IO.Directory.CreateDirectory(window.editor.ProjectDirectory + "\\TEMPBUILD");
                //builder.Output = window.editor.ProjectDirectory + "\\TEMPBUILD";

                builder.Add(filename.Text, System.IO.Path.GetFileNameWithoutExtension(filename.Text), importer.Text, processor.Text);

                Cursor = Cursors.WaitCursor;
                string error = builder.Build();
                Cursor = Cursors.Default;

                if (string.IsNullOrEmpty(error))
                {
                    List<string> dependencies = new List<string>();
                    List<bool> dependenciesbuilt = new List<bool>();
                    string Name = "";

                    foreach (string File in System.IO.Directory.GetFiles(builder.OutputDirectory))
                    {
                        string Filename = System.IO.Path.GetFileName(File);

                        if (Filename == System.IO.Path.GetFileNameWithoutExtension(filename.Text) + ".xnb")
                            Name = Filename;
                        else
                        {
                            dependencies.Add(System.IO.Path.GetFileNameWithoutExtension(Filename));
                            if (System.IO.Path.GetExtension(File) == ".xnb")
                                dependenciesbuilt.Add(true);
                            else
                                dependenciesbuilt.Add(false);
                        }
                        try
                        {
                            System.IO.File.Move(builder.OutputDirectory + "\\" + Filename, window.editor.ProjectDirectory + "\\Game" + folder.GenerateFilename() + "\\" + Filename);
                        }
                        catch (System.IO.IOException IOe)
                        { }
                    }

                    string[] depend = new string[dependencies.Count];
                    for (int i = 0; i < dependencies.Count; i++)
                        depend[i] = dependencies[i];

                    bool[] dependbuilt = new bool[dependenciesbuilt.Count];
                    for (int i = 0; i < dependenciesbuilt.Count; i++)
                        dependbuilt[i] = dependenciesbuilt[i];

                    window.AddContentItem(System.IO.Path.GetFileNameWithoutExtension(Name), depend, true, dependbuilt);

                    Close();
                }
                else
                    MessageBox.Show(error, "Build Error");

                //try
                //{
                //    System.IO.Directory.Delete(window.editor.ProjectDirectory + "\\TEMPBUILD\\Content");
                //    System.IO.Directory.Delete(window.editor.ProjectDirectory + "\\TEMPBUILD");
                //}
                //catch (System.IO.IOException IOe)
                //{ }
            }
            else
            {
                System.IO.File.Copy(filename.Text, window.editor.ProjectDirectory + "\\Game" + folder.GenerateFilename() + "\\" + System.IO.Path.GetFileName(filename.Text));
                window.AddContentItem(System.IO.Path.GetFileName(filename.Text), null, false, null);
                Close();
            }
        }
    }
}
