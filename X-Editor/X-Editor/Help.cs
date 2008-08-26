using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace X_Editor
{
    public partial class Help : Form
    {
        public Help()
        {
            InitializeComponent();

            richTextBox1.Text = "Using the X-Engine Editor is a simple way to create games and applications for the X-Engine." +
                                "\n\nTo add a component to the scene, simply drag it from the list on the right under the \"Components\" tab to the viewport, the large window you will see on the left side of the screen. Once the component has been added to the scene, you can edit its properties by right clicking on it from the list of currently added components on the bottom right part of the screen, and clicking \"Properties\"." +
                                "\n\nTo remove a component from the scene, right click on it and choose \"Remove\". In the properties window of the currently selected component, you can adjust many values that will change how the component functions. Some components, such as the Height Map, require other classes to function properly. To create them, simply drag the component, in the case of terrain, Environmental Parameters, from the list of components onto the viewport. Then you can select the component that uses the class, choose properties, and drag the new class right into the properties window." +
                                "\n\nTo add content to the game, click on the content tab and choose manage content. Here you will see a list of all the content that has been added to the game. You can add content using one of the buttons at the bottom of the content manager. Build Content will let you add content that needs to be built, such as models or textures. Add Content will let you add content that does not need to be built, such as XML files. In the build and add content windows, the \"Output\" textbox should contain the full path to the file, including it's name without the file extension, the same way a filename would be entered into a \"Content.Load\" call in standard XNA. For example \"Content\\Textures\\Heightmap\"." +
                                "\n\nOnce content has been built or added, it can be used by the components in the scene by typing the same filename you just used to build the content into the corresponding file path in the component's properties.";
        }
    }
}
