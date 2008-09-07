using System.Windows.Forms;
using XEngine;

namespace X_Editor
{
    public class XWater_Plugin : ComponentPlugin
    {
        public XWater_Plugin(XMain X)
            : base(X)
        {
            type = typeof(XWater);
            //Name = "Water";
        }

        public override ListViewItem SetupListViewItem()
        {
            ListViewItem item = new ListViewItem();
            //item.Text = Name;

            XWater water = new XWater(ref X);
            water.Load(X.Content);
            item.Tag = water;
            
            return item;
        }

        public override void UpdateObjectProperties(object Input, PropertyGrid Properties, ListView Scene)
        {
            XWater water = (XWater)Input;
            water.Load(X.Content);
        }

        public override void AcceptDragDrop(object Input, object DraggedItem, PropertyGrid Properties, ListView Scene)
        {
        }

        public override void LoadFromXML(System.Xml.XmlNode node, ListView scene)
        {
            XTools tools = new XTools(X);

            XWater water = new XWater(ref X, tools.ParseXMLVector2(node.Attributes["PointOne"].InnerText), tools.ParseXMLVector2(node.Attributes["PointTwo"].InnerText), float.Parse(node.Attributes["Height"].InnerText));
            water.Load(X.Content);

            water.AutoDraw = bool.Parse(node.Attributes["AutoDraw"].InnerText);
            water.DrawOrder = int.Parse(node.Attributes["DrawOrder"].InnerText);
            water.WaveLenth = float.Parse(node.Attributes["WaveLength"].InnerText);
            water.WaveHeight = float.Parse(node.Attributes["WaveHeight"].InnerText);
            water.WindDirection = float.Parse(node.Attributes["WindDirection"].InnerText);
            water.WindForce = float.Parse(node.Attributes["WindForce"].InnerText);
            //water.ComponentID = int.Parse(node.Attributes["ComponentID"].InnerText);

            //ListViewItem item = new ListViewItem(Name);
            //item.Tag = water;
            //item.Group = scene.Groups["Environment"];

            //scene.Items.Add(item);
        }

        public override void WriteToXML(System.Xml.XmlWriter writer, object obj)
        {
            XWater water = (XWater)obj;

            writer.WriteStartElement("sceneitem");
            //writer.WriteAttributeString("Type", Name);
            writer.WriteAttributeString("ComponentID", water.ComponentID.ToString());
            writer.WriteAttributeString("PointOne", water.PointOne.ToString());
            writer.WriteAttributeString("PointTwo", water.PointTwo.ToString());
            writer.WriteAttributeString("Height", water.Height.ToString());
            writer.WriteAttributeString("AutoDraw", water.AutoDraw.ToString());
            writer.WriteAttributeString("DrawOrder", water.DrawOrder.ToString());
            writer.WriteAttributeString("WaveLength", water.WaveLenth.ToString());
            writer.WriteAttributeString("WaveHeight", water.WaveHeight.ToString());
            writer.WriteAttributeString("WindDirection", water.WindDirection.ToString());
            writer.WriteAttributeString("WindForce", water.WindForce.ToString());
            writer.WriteEndElement();
        }
    }
}
