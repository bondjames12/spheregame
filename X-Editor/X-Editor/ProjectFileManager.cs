using System.Text;
using System.Xml;

namespace X_Editor
{
    public class ProjectFileManager
    {
        readonly EditorForm editor;

        public ProjectFileManager(EditorForm editor)
        {
            this.editor = editor;
        }

        public void SaveProjectFile(string Filename, ComponentPluginManager plugins)
        {
            XmlTextWriter writer = new XmlTextWriter(Filename, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;

            writer.WriteStartDocument();
            writer.WriteStartElement("Project");

            editor.content.WriteToXML(writer);
            editor.scenes.WriteToXML(writer);

            writer.WriteEndElement();
            writer.WriteEndDocument();

            writer.Flush();
            writer.Close();
        }

        public void LoadProjectFile(string Filename, ComponentPluginManager plugins)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Filename);

            editor.content.LoadFromXML(doc);
            editor.scenes.LoadFromXML(doc.DocumentElement.ChildNodes[1].ChildNodes);
        }
    }
}
