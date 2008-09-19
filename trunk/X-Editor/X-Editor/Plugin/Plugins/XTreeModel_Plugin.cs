using System.Windows.Forms;
using XEngine;
using System.Collections.Generic;

namespace X_Editor
{
    public class XTreeModel_Plugin : ComponentPlugin
    {
        public XTreeModel_Plugin(XMain X)
            : base(X)
        {
            this.type = typeof(XTreeModel);
            group = "Models";
        }

        public override void AcceptDragDrop(object Input, object DraggedItem, System.Windows.Forms.PropertyGrid Properties, System.Windows.Forms.ListView Scene)
        {
            if (Tools.GetPropertyAtPoint(Properties.PointToClient(Cursor.Position), Properties).Label == "XMLProfile" && DraggedItem is ContentItem)
                ((XTreeModel)Input).XMLProfile = ((ContentItem)DraggedItem).GenerateFullPathFilename();
            else if (Tools.GetPropertyAtPoint(Properties.PointToClient(Cursor.Position), Properties).Label == "barkTexture" && DraggedItem is ContentItem)
                ((XTreeModel)Input).barkTexture = ((ContentItem)DraggedItem).GenerateFilename().Replace("\\Content", "Content");
            else if (Tools.GetPropertyAtPoint(Properties.PointToClient(Cursor.Position), Properties).Label == "leafTexture" && DraggedItem is ContentItem)
                ((XTreeModel)Input).leafTexture = ((ContentItem)DraggedItem).GenerateFilename().Replace("\\Content", "Content");

            UpdateObjectProperties(Input, Properties, Scene);
        }

        public override System.Windows.Forms.ListViewItem SetupListViewItem(ListViewItem item, XComponent component)
        {
            XTreeModel model = new XTreeModel(ref X, null,null,null);

            return base.SetupListViewItem(item, model);
        }

        public override void UpdateObjectProperties(object Input, PropertyGrid Properties, ListView Scene)
        {
            XTreeModel model = (XTreeModel)Input;

            //load the model file is we have a filenames now
            if (!string.IsNullOrEmpty(model.XMLProfile) && !string.IsNullOrEmpty(model.leafTexture) && !string.IsNullOrEmpty(model.barkTexture))
                model.Load(X.Content);

            base.UpdateObjectProperties(Input, Properties, Scene);
        }

        public override void WriteToXML(System.Xml.XmlWriter writer, object obj)
        {
            XTreeModel model = (XTreeModel)obj;

            writer.WriteStartElement("sceneitem");
            writer.WriteAttributeString("Type", model.GetType().ToString());
            writer.WriteAttributeString("AutoDraw", model.AutoDraw.ToString());
            writer.WriteAttributeString("ComponentID", model.ComponentID.ToString());
            writer.WriteAttributeString("DrawOrder", model.DrawOrder.ToString());
            writer.WriteAttributeString("XMLProfile", model.XMLProfile);
            writer.WriteAttributeString("leafTexture", model.leafTexture);
            writer.WriteAttributeString("barkTexture", model.barkTexture);
            writer.WriteAttributeString("Name", model.Name);
            writer.WriteAttributeString("seed", model.seed.ToString());
            writer.WriteAttributeString("cuttoffLevel", model.cuttoffLevel.ToString());
            writer.WriteAttributeString("radialSegments", model.radialSegments.ToString());
            writer.WriteAttributeString("addLeaves", model.addLeaves.ToString());
            writer.WriteEndElement();
        }

        public override void LoadFromXML(System.Xml.XmlNode node, System.Windows.Forms.ListView scene,ref Dictionary<uint, List<uint>> Depends)
        {
            XTreeModel model = new XTreeModel(ref X, node.Attributes["XMLProfile"].InnerText, node.Attributes["barkTexture"].InnerText, node.Attributes["leafTexture"].InnerText);
            model.AutoDraw = bool.Parse(node.Attributes["AutoDraw"].InnerText);
            model.ComponentID = uint.Parse(node.Attributes["ComponentID"].InnerText);
            model.DrawOrder = int.Parse(node.Attributes["DrawOrder"].InnerText);
            model.Name = node.Attributes["Name"].InnerText;
            model.seed = int.Parse(node.Attributes["seed"].InnerText);
            model.cuttoffLevel = int.Parse(node.Attributes["cuttoffLevel"].InnerText);
            model.radialSegments = int.Parse(node.Attributes["radialSegments"].InnerText);
            model.addLeaves = bool.Parse(node.Attributes["addLeaves"].InnerText);

            //load the model file is we have a filenames now
            if (!string.IsNullOrEmpty(model.XMLProfile) && !string.IsNullOrEmpty(model.leafTexture) && !string.IsNullOrEmpty(model.barkTexture))
                model.Load(X.Content);

            X_Editor.Tools.AddXComponentToSceneList(scene, model, group);
        }
    }
}
