using System.Windows.Forms;
using System.Drawing;
using Microsoft.Xna.Framework.Input;

namespace X_Editor
{
    public partial class OpenPopup : Form
    {
        EditorForm editor;
        public OpenPopup(EditorForm editor)
        {
            this.editor = editor;

            InitializeComponent();

            editor.Enabled = false;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            editor.openLeveToolStripMenuItem_Click(sender, e);
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            editor.newProjectToolStripMenuItem_Click(sender, e);
        }
    }
}
