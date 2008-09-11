using System.Windows.Forms;
using XEngine;
using System.Collections.Generic;

namespace X_Editor
{
    public class XWater_Plugin : ComponentPlugin
    {
        public XWater_Plugin(XMain X)
            : base(X)
        {
            type = typeof(XWater);
            group = "Environment";
        }

        public override ListViewItem SetupListViewItem(ListViewItem item, XComponent component)
        {
            XWater water = new XWater(ref X);
            water.Load(X.Content);
            return base.SetupListViewItem(item, water);
        }

        public override void UpdateObjectProperties(object Input, PropertyGrid Properties, ListView Scene)
        {
            XWater water = (XWater)Input;
            if (!water.loaded) water.Load(X.Content);

            base.UpdateObjectProperties(Input, Properties, Scene);
        }

        public override void AcceptDragDrop(object Input, object DraggedItem, PropertyGrid Properties, ListView Scene)
        {
        }

        public override void WriteToXML(System.Xml.XmlWriter writer, object obj)
        {
            XWater water = (XWater)obj;

            writer.WriteStartElement("sceneitem");
            writer.WriteAttributeString("Type", water.GetType().ToString());
            writer.WriteAttributeString("AutoDraw", water.AutoDraw.ToString());
            writer.WriteAttributeString("ComponentID", water.ComponentID.ToString());
            //writer.WriteAttributeString("PointOne", water.PointOne.ToString());
            //writer.WriteAttributeString("PointTwo", water.PointTwo.ToString());
            writer.WriteAttributeString("DoesReflect", water.DoesReflect.ToString());
            writer.WriteAttributeString("DoesRefract", water.DoesRefract.ToString());
            writer.WriteAttributeString("DrawOrder", water.DrawOrder.ToString());
            writer.WriteAttributeString("Height", water.Height.ToString());
            writer.WriteAttributeString("Name", water.Name.ToString());
            writer.WriteAttributeString("Rotation", water.Rotation.ToString());
            writer.WriteAttributeString("Scale", water.Scale.ToString());
            writer.WriteAttributeString("Translation", water.Translation.ToString());
            writer.WriteAttributeString("WaveHeight", water.WaveHeight.ToString());
            writer.WriteAttributeString("WaveLength", water.WaveLength.ToString());
            writer.WriteAttributeString("WindDirection", water.WindDirection.ToString());
            writer.WriteAttributeString("WindForce", water.WindForce.ToString());
            writer.WriteEndElement();
        }

        public override void LoadFromXML(System.Xml.XmlNode node, ListView scene, ref Dictionary<uint, List<uint>> Depends)
        {
            XTools tools = new XTools(X);

            //XWater water = new XWater(ref X, tools.ParseXMLVector2(node.Attributes["PointOne"].InnerText), tools.ParseXMLVector2(node.Attributes["PointTwo"].InnerText), float.Parse(node.Attributes["Height"].InnerText));
            XWater water = new XWater(ref X);
            water.Load(X.Content);

            water.AutoDraw = bool.Parse(node.Attributes["AutoDraw"].InnerText);
            water.ComponentID = uint.Parse(node.Attributes["ComponentID"].InnerText);
            water.DoesReflect = bool.Parse(node.Attributes["DoesReflect"].InnerText);
            water.DoesRefract = bool.Parse(node.Attributes["DoesRefract"].InnerText);
            water.DrawOrder = int.Parse(node.Attributes["DrawOrder"].InnerText);
            water.Height = float.Parse(node.Attributes["Height"].InnerText);
            water.Name = node.Attributes["Name"].InnerText;
            water.Rotation = X.Tools.ParseXMLQuaternion(node.Attributes["Rotation"].InnerText);
            water.Scale = X.Tools.ParseXMLVector3(node.Attributes["Scale"].InnerText);
            water.Translation = X.Tools.ParseXMLVector3(node.Attributes["Translation"].InnerText);
            water.WaveHeight = float.Parse(node.Attributes["WaveHeight"].InnerText);
            water.WaveLength = float.Parse(node.Attributes["WaveLength"].InnerText);
            water.WindDirection = float.Parse(node.Attributes["WindDirection"].InnerText);
            water.WindForce = float.Parse(node.Attributes["WindForce"].InnerText);

            X_Editor.Tools.AddXComponentToSceneList(scene, water, group);
        }
    }
}
