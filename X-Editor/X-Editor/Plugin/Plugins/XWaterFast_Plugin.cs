using System.Windows.Forms;
using XEngine;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace X_Editor
{
    public class XWaterFast_Plugin : ComponentPlugin
    {
        public XWaterFast_Plugin(XMain X)
            : base(X)
        {
            type = typeof(XWaterFast);
            group = "Environment";
        }

        public override ListViewItem SetupListViewItem(ListViewItem item, XComponent component)
        {
            XWaterFast water = new XWaterFast(ref X, null);
            
            return base.SetupListViewItem(item, water);
        }

        public override void UpdateObjectProperties(object Input, PropertyGrid Properties, ListView Scene)
        {
            XWaterFast water = (XWaterFast)Input;
            if (!string.IsNullOrEmpty(water.EnvironmentMap)) water.Load(X.Content);

            base.UpdateObjectProperties(Input, Properties, Scene);
        }

        public override void AcceptDragDrop(object Input, object DraggedItem, PropertyGrid Properties, ListView Scene)
        {
            if (Tools.GetPropertyAtPoint(Properties.PointToClient(Cursor.Position), Properties).Label == "EnvironmentMap" && DraggedItem is ContentItem)
            {
                ((XWaterFast)Input).EnvironmentMap = ((ContentItem)DraggedItem).GenerateFilename().Replace("\\Content", "Content");
                ((XWaterFast)Input).Load(X.Content);
            }

            UpdateObjectProperties(Input, Properties, Scene);
        }

        public override void WriteToXML(System.Xml.XmlWriter writer, object obj)
        {
            XWaterFast water = (XWaterFast)obj;

            writer.WriteStartElement("sceneitem");
            writer.WriteAttributeString("Type", water.GetType().ToString());
            writer.WriteAttributeString("AutoDraw", water.AutoDraw.ToString());
            writer.WriteAttributeString("ComponentID", water.ComponentID.ToString());     
            writer.WriteAttributeString("DrawOrder", water.DrawOrder.ToString());
            writer.WriteAttributeString("EnvironmentMap", water.EnvironmentMap);
            writer.WriteAttributeString("Name", water.Name.ToString());
            writer.WriteAttributeString("Rotation", water.Rotation.ToString());
            writer.WriteAttributeString("Scale", water.Scale.ToString());
            writer.WriteAttributeString("Translation", water.Translation.ToString());
            writer.WriteAttributeString("BumpHeight", water.BumpHeight.ToString());
            writer.WriteAttributeString("BumpSpeed", water.BumpSpeed.ToString());
            writer.WriteAttributeString("DeepWaterColor", water.DeepWaterColor.ToString());
            writer.WriteAttributeString("FresnelBias", water.FresnelBias.ToString());
            writer.WriteAttributeString("FresnelPower", water.FresnelPower.ToString());
            writer.WriteAttributeString("HDRMultiplier", water.HDRMultiplier.ToString());
            writer.WriteAttributeString("Height", water.Height.ToString());
            writer.WriteAttributeString("ReflectionAmount", water.ReflectionAmount.ToString());
            writer.WriteAttributeString("ReflectionColor", water.ReflectionColor.ToString());
            writer.WriteAttributeString("ShallowWaterColor", water.ShallowWaterColor.ToString());
            writer.WriteAttributeString("TextureScale", water.TextureScale.ToString());
            writer.WriteAttributeString("WaterAmount", water.WaterAmount.ToString());
            writer.WriteAttributeString("WaveAmplitute", water.WaveAmplitude.ToString());
            writer.WriteAttributeString("WaveFrequency", water.WaveFrequency.ToString());
            writer.WriteAttributeString("Width", water.Width.ToString());
            writer.WriteEndElement();
        }

        public override void LoadFromXML(System.Xml.XmlNode node, ListView scene, ref Dictionary<uint, List<uint>> Depends)
        {
            XTools tools = new XTools(X);

            XWaterFast water = new XWaterFast(ref X, node.Attributes["EnvironmentMap"].InnerText);
            
            water.AutoDraw = bool.Parse(node.Attributes["AutoDraw"].InnerText);
            water.ComponentID = uint.Parse(node.Attributes["ComponentID"].InnerText);
            water.DrawOrder = int.Parse(node.Attributes["DrawOrder"].InnerText);
            water.Name = node.Attributes["Name"].InnerText;
            water.Rotation = X.Tools.ParseXMLQuaternion(node.Attributes["Rotation"].InnerText);
            water.Scale = X.Tools.ParseXMLVector3(node.Attributes["Scale"].InnerText);
            water.Translation = X.Tools.ParseXMLVector3(node.Attributes["Translation"].InnerText);
            water.BumpHeight = float.Parse(node.Attributes["BumpHeight"].InnerText);
            water.BumpSpeed = X.Tools.ParseXMLVector2(node.Attributes["BumpSpeed"].InnerText);
            water.DeepWaterColor = X.Tools.ParseXMLColor(node.Attributes["DeepWaterColor"].InnerText);
            water.FresnelBias = float.Parse(node.Attributes["FresnelBias"].InnerText);
            water.FresnelPower = float.Parse(node.Attributes["FresnelPower"].InnerText);
            water.HDRMultiplier = float.Parse(node.Attributes["HDRMultiplier"].InnerText);
            water.Height = int.Parse(node.Attributes["Height"].InnerText);
            water.ReflectionAmount = float.Parse(node.Attributes["ReflectionAmount"].InnerText);
            water.ReflectionColor = X.Tools.ParseXMLColor(node.Attributes["ReflectionColor"].InnerText);
            water.ShallowWaterColor = X.Tools.ParseXMLColor(node.Attributes["ShallowWaterColor"].InnerText);
            water.TextureScale = X.Tools.ParseXMLVector2(node.Attributes["TextureScale"].InnerText);
            water.WaterAmount = float.Parse(node.Attributes["WaterAmount"].InnerText);
            water.WaveAmplitude = float.Parse(node.Attributes["WaveAmplitute"].InnerText);
            water.WaveFrequency = float.Parse(node.Attributes["WaveFrequency"].InnerText);
            water.Width = int.Parse(node.Attributes["Width"].InnerText);
            
            water.Load(X.Content);

            X_Editor.Tools.AddXComponentToSceneList(scene, water, group);
        }
    }
}
