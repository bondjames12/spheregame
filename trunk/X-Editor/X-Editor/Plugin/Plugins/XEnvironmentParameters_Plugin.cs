using System.Windows.Forms;
using Microsoft.Xna.Framework;
using XEngine;

namespace X_Editor
{
    public class XEnvironmentParameters_Plugin : ComponentPlugin
    {
        public XEnvironmentParameters_Plugin(XMain X)
            : base(X)
        {
            type = typeof(XEnvironmentParameters);
            Name = "Environment Parameters";
        }

        public override ListViewItem SetupListViewItem()
        {
            ListViewItem item = new ListViewItem();
            item.Text = Name;

            XEnvironmentParameters paramaters = new XEnvironmentParameters(X);
            paramaters.Load(X.Content);
            item.Tag = paramaters;

            return item;
        }

        public override void UpdateObjectProperties(object Input, PropertyGrid Properties, ListView Scene)
        {
            XEnvironmentParameters parameters = (XEnvironmentParameters)Input;

            if (!string.IsNullOrEmpty(parameters.NightFile) && !string.IsNullOrEmpty(parameters.DayFile) && !string.IsNullOrEmpty(parameters.SunsetFile))
            {
                try
                {
                    parameters.Load(X.Content);

                    if (Properties != null && Properties.SelectedObject == parameters)
                        Properties.SelectedObject = parameters;
                }
                catch
                {
                    MessageBox.Show("There was a problem loading one of the files. Check that the file exists and has been built with the Texture importer and processor.", "File Error");
                }
            }

            foreach (ListViewItem component in Scene.Items)
                if (((XComponent)(component.Tag)).GetType() == typeof(XHeightMap) || ((XComponent)(component.Tag)).GetType() == typeof(XDynamicSky))
                    ((XComponent)(component.Tag)).Load(X.Content);
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
            writer.WriteAttributeString("Type", Name);
            writer.WriteAttributeString("ComponentID", param.ComponentID.ToString());
            writer.WriteAttributeString("DayFile", param.DayFile);
            writer.WriteAttributeString("NightFile", param.NightFile);
            writer.WriteAttributeString("SunsetFile", param.SunsetFile);
            writer.WriteAttributeString("SunsetSharpness", param.DayToSunsetSharpness.ToString());
            writer.WriteAttributeString("FogColor", param.FogColor.ToString());
            writer.WriteAttributeString("FogDensity", param.FogDensity.ToString());
            writer.WriteAttributeString("HazeTopAltitude", param.HazeTopAltitude.ToString());
            writer.WriteAttributeString("LargeSunLightness", param.LargeSunLightness.ToString());
            writer.WriteAttributeString("LargeSunRadiusAttentuation", param.LargeSunRadiusAttenuation.ToString());
            writer.WriteAttributeString("LightColor", param.LightColor.ToString());
            writer.WriteAttributeString("LightAmbient", param.LightColorAmbient.ToString());
            writer.WriteAttributeString("SunLightness", param.SunLightness.ToString());
            writer.WriteAttributeString("SunRadiusAttentuation", param.SunRadiusAttenuation.ToString());
            writer.WriteAttributeString("EnvParamsNum", param.number.ToString());

            writer.WriteEndElement();
        }

        public override void LoadFromXML(System.Xml.XmlNode node, ListView scene)
        {
            XTools tools = new XTools(X);

            XEnvironmentParameters param = new XEnvironmentParameters(X);
            param.DayFile = node.Attributes["DayFile"].InnerText;
            param.NightFile = node.Attributes["NightFile"].InnerText;
            param.SunsetFile = node.Attributes["SunsetFile"].InnerText;
            param.DayToSunsetSharpness = float.Parse(node.Attributes["SunsetSharpness"].InnerText);
            param.FogColor = tools.ConvertVector3ToVector4(tools.ParseXMLVector3(node.Attributes["FogColor"].InnerText));
            param.FogDensity = float.Parse(node.Attributes["FogDensity"].InnerText);
            param.HazeTopAltitude = float.Parse(node.Attributes["HazeTopAltitude"].InnerText);
            param.LargeSunLightness = float.Parse(node.Attributes["LargeSunLightness"].InnerText);
            param.LargeSunRadiusAttenuation = float.Parse(node.Attributes["LargeSunRadiusAttentuation"].InnerText);
            param.LightColor = new Vector4(tools.ParseXMLVector3(node.Attributes["LightColor"].InnerText), 1);
            param.LightColorAmbient = tools.ConvertVector3ToVector4(tools.ParseXMLVector3(node.Attributes["LightAmbient"].InnerText));
            param.SunLightness = float.Parse(node.Attributes["SunLightness"].InnerText);
            param.SunRadiusAttenuation = float.Parse(node.Attributes["SunRadiusAttentuation"].InnerText);
            //param.ComponentID = int.Parse(node.Attributes["ComponentID"].InnerText);

            ListViewItem sceneitem = new ListViewItem();

            int paramsNum = int.Parse(node.Attributes["EnvParamsNum"].InnerText);
            param.number = paramsNum;

            if (XEnvironmentParameters.count < paramsNum)
                XEnvironmentParameters.count = paramsNum;

            if (!string.IsNullOrEmpty(param.DayFile) && !string.IsNullOrEmpty(param.NightFile) && !string.IsNullOrEmpty(param.SunsetFile))
                param.Load(X.Content);

            foreach (ListViewItem item in scene.Items)
            {
                if (item.Tag is XHeightMap)
                {
                    if (((XHeightMap)item.Tag).environmentalParametersNumber == paramsNum)
                    {
                        ((XHeightMap)item.Tag).Params = param;

                        if (!string.IsNullOrEmpty(((XHeightMap)item.Tag).TextureMap) && !string.IsNullOrEmpty(((XHeightMap)item.Tag).HeightMap) && ((XHeightMap)item.Tag).Params != null)
                            ((XHeightMap)item.Tag).Load(X.Content);
                    }
                }
                else if (item.Tag is XDynamicSky)
                {
                    if (((XDynamicSky)item.Tag).environmentalParametersNumber == paramsNum)
                    {
                        ((XDynamicSky)item.Tag).Params = param;

                        if (((XDynamicSky)item.Tag).Params != null)
                            ((XDynamicSky)item.Tag).Load(X.Content);
                    }
                }
            }

            sceneitem.Text = Name;
            sceneitem.Tag = param;
            sceneitem.Group = scene.Groups["Environment"];

            scene.Items.Add(sceneitem);
        }
    }
}
