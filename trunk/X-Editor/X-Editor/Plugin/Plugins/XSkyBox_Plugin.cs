using System.Windows.Forms;
using Microsoft.Xna.Framework;
using XEngine;
using System.Collections.Generic;

namespace X_Editor
{
    public class XSkyBox_Plugin : ComponentPlugin
    {
        public XSkyBox_Plugin(XMain X)
            : base(X)
        {
            type = typeof(XSkyBox);
            group = "Environment";
        }

        public override ListViewItem SetupListViewItem(ListViewItem item, XComponent component)
        {
            XSkyBox sky = new XSkyBox(ref X, null);
            return base.SetupListViewItem(item, sky);
        }

        public override void UpdateObjectProperties(object Input, PropertyGrid Properties, ListView Scene)
        {
            XSkyBox sky = (XSkyBox)Input;

            if (!string.IsNullOrEmpty(sky.SkyCubeMap))
            {
                sky.Load(X.Content);
            }

            base.UpdateObjectProperties(Input, Properties, Scene);
        }

        public override void AcceptDragDrop(object Input, object DraggedItem, PropertyGrid Properties, ListView Scene)
        {
            XSkyBox sky = (XSkyBox)Input;

            if (Tools.GetPropertyAtPoint(Properties.PointToClient(Cursor.Position), Properties).Label == "SkyCubeMap" && DraggedItem is ContentItem)
                sky.SkyCubeMap = ((ContentItem)DraggedItem).GenerateFilename().Replace("\\Content", "Content");


            UpdateObjectProperties(sky, Properties, Scene);
        }

        public override void WriteToXML(System.Xml.XmlWriter writer, object obj)
        {
            XSkyBox sky = (XSkyBox)obj;

            writer.WriteStartElement("sceneitem");
            writer.WriteAttributeString("Type", sky.GetType().ToString());
            writer.WriteAttributeString("ComponentID", sky.ComponentID.ToString());
            writer.WriteAttributeString("AutoDraw", sky.AutoDraw.ToString());
            writer.WriteAttributeString("DrawOrder", sky.DrawOrder.ToString());
            writer.WriteAttributeString("Name", sky.Name);
            writer.WriteAttributeString("SkyCubeMap", sky.SkyCubeMap);
            writer.WriteEndElement();
        }

        public override void LoadFromXML(System.Xml.XmlNode node, ListView scene, ref Dictionary<uint, List<uint>> Depends)
        {
            XSkyBox sky = new XSkyBox(ref X, node.Attributes["SkyCubeMap"].InnerText);

            sky.AutoDraw = bool.Parse(node.Attributes["AutoDraw"].InnerText);
            sky.DrawOrder = int.Parse(node.Attributes["DrawOrder"].InnerText);
            sky.ComponentID = uint.Parse(node.Attributes["ComponentID"].InnerText);
            sky.Name = node.Attributes["Name"].InnerText;

            sky.Load(X.Content);

            X_Editor.Tools.AddXComponentToSceneList(scene, sky, group);
        }
    }
}
