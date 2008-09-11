using System.Windows.Forms;
using XEngine;
using System.Collections.Generic;

namespace X_Editor
{
    public class XModel_Plugin : ComponentPlugin
    {
        public XModel_Plugin(XMain X) : base(X)
        {
            this.type = typeof(XModel);
            group = "Models";
        }

        public override void AcceptDragDrop(object Input, object DraggedItem, System.Windows.Forms.PropertyGrid Properties, System.Windows.Forms.ListView Scene)
        {
            if (Tools.GetPropertyAtPoint(Properties.PointToClient(Cursor.Position), Properties).Label == "Filename_editor" && DraggedItem is ContentItem)
                ((XModel)Input).Filename = ((ContentItem)DraggedItem).GenerateFilename().Replace("\\Content", "Content");

            UpdateObjectProperties(Input, Properties, Scene);
        }

        public override System.Windows.Forms.ListViewItem SetupListViewItem(ListViewItem item, XComponent component)
        {
            XModel model = new XModel(ref X, null);
            return base.SetupListViewItem(item, model);
        }

        public override void UpdateObjectProperties(object Input, PropertyGrid Properties, ListView Scene)
        {
            XModel model = (XModel)Input;

            //load the model file is we have a filename now
             if (!string.IsNullOrEmpty(model.Filename))
                model.Load(X.Content);

            base.UpdateObjectProperties(Input, Properties, Scene);
        }

        public override void WriteToXML(System.Xml.XmlWriter writer, object obj)
        {
            XModel model = (XModel)obj;

            writer.WriteStartElement("sceneitem");
            writer.WriteAttributeString("Type", model.GetType().ToString());
            writer.WriteAttributeString("AutoDraw", model.AutoDraw.ToString());
            writer.WriteAttributeString("ComponentID", model.ComponentID.ToString());
            writer.WriteAttributeString("DrawOrder", model.DrawOrder.ToString());
            writer.WriteAttributeString("Filename_editor", model.Filename_editor);
            writer.WriteAttributeString("Name", model.Name);
            writer.WriteEndElement();
        }

        public override void LoadFromXML(System.Xml.XmlNode node, System.Windows.Forms.ListView scene,ref Dictionary<uint, List<uint>> Depends)
        {
            XModel model = new XModel(ref X, node.Attributes["Filename_editor"].InnerText);
            model.AutoDraw = bool.Parse(node.Attributes["AutoDraw"].InnerText);
            model.ComponentID = uint.Parse(node.Attributes["ComponentID"].InnerText);
            model.DrawOrder = int.Parse(node.Attributes["DrawOrder"].InnerText);
            model.Name = node.Attributes["Name"].InnerText;

            if (!string.IsNullOrEmpty(model.Filename))
                model.Load(X.Content);

            X_Editor.Tools.AddXComponentToSceneList(scene, model, group);
        }
    }
}
