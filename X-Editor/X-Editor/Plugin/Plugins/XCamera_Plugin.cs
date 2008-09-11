using System.Windows.Forms;
using XEngine;
using System.Collections.Generic;

namespace X_Editor
{
    public class XCamera_Plugin : ComponentPlugin
    {
        public XCamera_Plugin(XMain X)
            : base(X)
        {
            type = typeof(XCamera);
            group = "Camera";
        }

        public override ListViewItem SetupListViewItem(ListViewItem item, XComponent component)
        {
            XCamera camera = new XCamera(ref X,0.1f,1000f);
            return base.SetupListViewItem(item, camera);
        }

        public override void UpdateObjectProperties(object Input, PropertyGrid Properties, ListView Scene)
        {
            base.UpdateObjectProperties(Input, Properties, Scene);
        }

        public override void AcceptDragDrop(object Input, object DraggedItem, PropertyGrid Properties, ListView Scene)
        {
        }

        public override void LoadFromXML(System.Xml.XmlNode node, ListView scene, ref Dictionary<uint, List<uint>> Depends)
        {
            XTools tools = new XTools(X);

            //XCamera camera = new XCamera(ref X, tools.ParseXMLVector2(node.Attributes["PointOne"].InnerText), tools.ParseXMLVector2(node.Attributes["PointTwo"].InnerText), float.Parse(node.Attributes["Height"].InnerText));
            //camera.Load(X.Content);

            //camera.AutoDraw = bool.Parse(node.Attributes["AutoDraw"].InnerText);
            //camera.DrawOrder = int.Parse(node.Attributes["DrawOrder"].InnerText);
            //camera.WaveLenth = float.Parse(node.Attributes["WaveLength"].InnerText);
            //camera.WaveHeight = float.Parse(node.Attributes["WaveHeight"].InnerText);
            //camera.WindDirection = float.Parse(node.Attributes["WindDirection"].InnerText);
            //camera.WindForce = float.Parse(node.Attributes["WindForce"].InnerText);
            //camera.ComponentID = int.Parse(node.Attributes["ComponentID"].InnerText);

            //X_Editor.Tools.AddXComponentToSceneList(scene, sky, "Environment");
        }

        public override void WriteToXML(System.Xml.XmlWriter writer, object obj)
        {
            XCamera camera = (XCamera)obj;

            /*writer.WriteStartElement("sceneitem");
            writer.WriteAttributeString("Type", Name);
            writer.WriteAttributeString("ComponentID", camera.ComponentID.ToString());
            writer.WriteAttributeString("PointOne", camera.PointOne.ToString());
            writer.WriteAttributeString("PointTwo", camera.PointTwo.ToString());
            writer.WriteAttributeString("Height", camera.Height.ToString());
            writer.WriteAttributeString("AutoDraw", camera.AutoDraw.ToString());
            writer.WriteAttributeString("DrawOrder", camera.DrawOrder.ToString());
            writer.WriteAttributeString("WaveLength", camera.WaveLenth.ToString());
            writer.WriteAttributeString("WaveHeight", camera.WaveHeight.ToString());
            writer.WriteAttributeString("WindDirection", camera.WindDirection.ToString());
            writer.WriteAttributeString("WindForce", camera.WindForce.ToString());
            writer.WriteEndElement();*/
        }
    }
}
