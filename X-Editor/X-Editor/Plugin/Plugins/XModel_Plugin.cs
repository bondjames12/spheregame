using System.Windows.Forms;
using XEngine;

namespace X_Editor
{
    public class XModel_Plugin : ComponentPlugin
    {
        public XModel_Plugin(XMain X) : base(X)
        {
            this.type = typeof(XModel);
        }

        public override void AcceptDragDrop(object Input, object DraggedItem, System.Windows.Forms.PropertyGrid Properties, System.Windows.Forms.ListView Scene)
        {
            if (Tools.GetPropertyAtPoint(Properties.PointToClient(Cursor.Position), Properties).Label == "Filename_editor" && DraggedItem is ContentItem)
                ((XModel)Input).Filename = ((ContentItem)DraggedItem).GenerateFilename().Replace("\\Content", "Content");

            UpdateObjectProperties(Input, Properties, Scene);
        }

        public override System.Windows.Forms.ListViewItem SetupListViewItem(XComponent component)
        {
            XModel model = new XModel(ref X, null);

            return base.SetupListViewItem(model);
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
            writer.WriteStartElement("sceneitem");
            //writer.WriteAttributeString("Type", Name);
            writer.WriteAttributeString("ComponentID", ((XComponent)obj).ComponentID.ToString());
            writer.WriteAttributeString("Filename", ((XModel)obj).Filename);
            writer.WriteAttributeString("Number", ((XModel)obj).Number.ToString());
            writer.WriteEndElement();
        }

        public override void LoadFromXML(System.Xml.XmlNode node, System.Windows.Forms.ListView scene)
        {
            XModel model = new XModel(ref X, node.Attributes["Filename"].InnerText);
            model.Number = int.Parse(node.Attributes["Number"].InnerText);

            if (XModel.Count < model.Number)
                XModel.Count = model.Number;

             if (!string.IsNullOrEmpty(model.Filename))
               model.Load(X.Content);

            ListViewItem item = new ListViewItem();
            item.Text = model.ToString();
            item.Tag = model;
            item.Group = scene.Groups["Models"];

            //model.ComponentID = int.Parse(node.Attributes["ComponentID"].InnerText);

            foreach (ListViewItem sceneitem in scene.Items)
                if (sceneitem.Tag is XActor)
                    if (((XActor)sceneitem.Tag).modelNumber == model.Number)
                        ((XActor)sceneitem.Tag).model = model;

            scene.Items.Add(item);
        }
    }
}
