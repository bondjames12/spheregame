using System.Windows.Forms;
using Microsoft.Xna.Framework;
using XEngine;
using System.Collections.Generic;

namespace X_Editor
{
    public class XEnvironmentParameters_Plugin : ComponentPlugin
    {
        public XEnvironmentParameters_Plugin(XMain X)
            : base(X)
        {
            type = typeof(XEnvironmentParameters);
            group = "Environment";
        }

        public override ListViewItem SetupListViewItem(ListViewItem item, XComponent component)
        {
            XEnvironmentParameters paramaters = new XEnvironmentParameters(X);
            paramaters.Load(X.Content);
            return base.SetupListViewItem(item, paramaters);
        }

        public override void UpdateObjectProperties(object Input, PropertyGrid Properties, ListView Scene)
        {
            XEnvironmentParameters parameters = (XEnvironmentParameters)Input;

            if (!string.IsNullOrEmpty(parameters.NightFile) && !string.IsNullOrEmpty(parameters.DayFile) && !string.IsNullOrEmpty(parameters.SunsetFile))
            {
                try
                {
                    parameters.Load(X.Content);
                }
                catch
                {
                    MessageBox.Show("There was a problem loading one of the files. Check that the file exists and has been built with the Texture importer and processor.", "File Error");
                }
            }

            //search for an reload any XHeightMap, Sky components
            foreach (ListViewItem item in Scene.Items)
                if ((X.Tools.GetXComponentByID(item.SubItems["colID"].Text).GetType() == typeof(XHeightMap)) || ((X.Tools.GetXComponentByID(item.SubItems["colID"].Text)).GetType() == typeof(XDynamicSky)))
                    X.Tools.GetXComponentByID(item.SubItems["colID"].Text).Load(X.Content);

            base.UpdateObjectProperties(Input, Properties, Scene);
        }

        public override void AcceptDragDrop(object Input, object DraggedItem, PropertyGrid Properties, ListView Scene)
        {
            XEnvironmentParameters param = (XEnvironmentParameters)Input;

            if (Tools.GetPropertyAtPoint(Properties.PointToClient(Cursor.Position), Properties).Label == "DayFile" && DraggedItem is ContentItem)
                param.DayFile = ((ContentItem)DraggedItem).GenerateFilename().Replace("\\Content", "Content");
            else if (Tools.GetPropertyAtPoint(Properties.PointToClient(Cursor.Position), Properties).Label == "NightFile" && DraggedItem is ContentItem)
                param.NightFile = ((ContentItem)DraggedItem).GenerateFilename().Replace("\\Content", "Content");
            else if (Tools.GetPropertyAtPoint(Properties.PointToClient(Cursor.Position), Properties).Label == "SunsetFile" && DraggedItem is ContentItem)
                param.SunsetFile = ((ContentItem)DraggedItem).GenerateFilename().Replace("\\Content", "Content");

            UpdateObjectProperties(Input, Properties, Scene);
        }

        public override void WriteToXML(System.Xml.XmlWriter writer, object obj)
        {
            XEnvironmentParameters param = (XEnvironmentParameters)obj;

            writer.WriteStartElement("sceneitem");
            writer.WriteAttributeString("Type", this.type.ToString());
            writer.WriteAttributeString("AutoDraw", param.AutoDraw.ToString());
            writer.WriteAttributeString("ComponentID", param.ComponentID.ToString());
            writer.WriteAttributeString("dayFile", param.dayFile);
            writer.WriteAttributeString("dayToSunsetSharpness", param.dayToSunsetSharpness.ToString());
            writer.WriteAttributeString("DrawOrder", param.DrawOrder.ToString());
            writer.WriteAttributeString("fogColor", param.fogColor.ToString());
            writer.WriteAttributeString("fogDensity", param.fogDensity.ToString());
            writer.WriteAttributeString("hazeTopAltitude", param.hazeTopAltitude.ToString());
            writer.WriteAttributeString("largeSunLightness", param.largeSunLightness.ToString());
            writer.WriteAttributeString("largeSunRadiusAttentuation", param.largeSunRadiusAttenuation.ToString());
            writer.WriteAttributeString("lightColor", param.lightColor.ToString());
            writer.WriteAttributeString("lightColorAmbient", param.lightColorAmbient.ToString());
            writer.WriteAttributeString("lightDirection", param.lightDirection.ToString());
            writer.WriteAttributeString("Name", param.Name.ToString());
            writer.WriteAttributeString("nightFile", param.nightFile);
            writer.WriteAttributeString("shadows", param.shadows.ToString());
            writer.WriteAttributeString("sunLightness", param.sunLightness.ToString());
            writer.WriteAttributeString("sunRadiusAttentuation", param.sunRadiusAttenuation.ToString());
            writer.WriteAttributeString("sunsetFile", param.sunsetFile);

            writer.WriteEndElement();
        }

        public override void LoadFromXML(System.Xml.XmlNode node, ListView scene, ref Dictionary<uint, List<uint>> Depends)
        {
            XTools tools = new XTools(X);

            XEnvironmentParameters param = new XEnvironmentParameters(X);
            param.AutoDraw = bool.Parse(node.Attributes["AutoDraw"].InnerText);
            param.ComponentID = uint.Parse(node.Attributes["ComponentID"].InnerText);
            param.dayFile = node.Attributes["dayFile"].InnerText;
            param.dayToSunsetSharpness = float.Parse(node.Attributes["dayToSunsetSharpness"].InnerText);
            param.DrawOrder = int.Parse(node.Attributes["DrawOrder"].InnerText);
            param.fogColor = tools.ConvertVector3ToVector4(tools.ParseXMLVector3(node.Attributes["fogColor"].InnerText));
            param.fogDensity = float.Parse(node.Attributes["fogDensity"].InnerText);
            param.hazeTopAltitude = float.Parse(node.Attributes["hazeTopAltitude"].InnerText);
            param.largeSunLightness = float.Parse(node.Attributes["largeSunLightness"].InnerText);
            param.largeSunRadiusAttenuation = float.Parse(node.Attributes["largeSunRadiusAttentuation"].InnerText);
            param.lightColor = new Vector4(tools.ParseXMLVector3(node.Attributes["lightColor"].InnerText), 1);
            param.lightColorAmbient = new Vector4(tools.ParseXMLVector3(node.Attributes["lightColorAmbient"].InnerText), 1);
            param.lightDirection = new Vector4(tools.ParseXMLVector3(node.Attributes["lightDirection"].InnerText), 1);
            param.Name = node.Attributes["Name"].InnerText;
            param.nightFile = node.Attributes["nightFile"].InnerText;
            param.shadows = bool.Parse(node.Attributes["shadows"].InnerText);
            param.sunLightness = float.Parse(node.Attributes["sunLightness"].InnerText);
            param.sunRadiusAttenuation = float.Parse(node.Attributes["sunRadiusAttentuation"].InnerText);
            param.sunsetFile = node.Attributes["sunsetFile"].InnerText;
            
            ListViewItem sceneitem = new ListViewItem();

            if (!string.IsNullOrEmpty(param.DayFile) && !string.IsNullOrEmpty(param.NightFile) && !string.IsNullOrEmpty(param.SunsetFile))
                param.Load(X.Content);

            X_Editor.Tools.AddXComponentToSceneList(scene, param, group);
        }
    }
}
