using System.Windows.Forms;
using XEngine;

namespace X_Editor
{
    public class XHeightMap_Plugin : ComponentPlugin
    {
        public XHeightMap_Plugin(XMain X) : base(X)
        {
            type = typeof(XHeightMap);
            //Name = "Height Map Terrain";
        }

        public override ListViewItem SetupListViewItem()
        {
            ListViewItem item = new ListViewItem();
            //item.Text = Name;

            XHeightMap heightmap = new XHeightMap(ref X, null, null, null, null, null, null);
            item.Tag = heightmap;

            return item;
        }

        public override void UpdateObjectProperties(object Input, PropertyGrid Properties, ListView Scene)
        {
            XHeightMap heightmap = (XHeightMap)Input;

            if (!string.IsNullOrEmpty(heightmap.HeightMap) && !string.IsNullOrEmpty(heightmap.TextureMap) && heightmap.Params != null)
            {
                try
                {
                    heightmap.Load(X.Content);

                    if (Properties != null)
                        Properties.SelectedObject = heightmap;
                }
                catch
                {
                    MessageBox.Show("There was a problem loading one of the heightmap's files. Check that the file exists and has been built with the Texture importer and processor.", "File Error");
                }
            }
        }

        public override void AcceptDragDrop(object Input, object DraggedItem, PropertyGrid Properties, ListView Scene)
        {
            XHeightMap heightmap = (XHeightMap)Input;

            if (Tools.GetPropertyAtPoint(Properties.PointToClient(Cursor.Position), Properties).Label == "Params" && DraggedItem is XEnvironmentParameters)
                heightmap.Params = (XEnvironmentParameters)DraggedItem;
            else if (Tools.GetPropertyAtPoint(Properties.PointToClient(Cursor.Position), Properties).Label == "HeightMap" && DraggedItem is ContentItem)
                heightmap.HeightMap = ((ContentItem)DraggedItem).GenerateFilename().Replace("\\Content", "Content");
            else if (Tools.GetPropertyAtPoint(Properties.PointToClient(Cursor.Position), Properties).Label == "RTexture" && DraggedItem is ContentItem)
                heightmap.RTexture = ((ContentItem)DraggedItem).GenerateFilename().Replace("\\Content", "Content");
            else if (Tools.GetPropertyAtPoint(Properties.PointToClient(Cursor.Position), Properties).Label == "GTexture" && DraggedItem is ContentItem)
                heightmap.GTexture = ((ContentItem)DraggedItem).GenerateFilename().Replace("\\Content", "Content");
            else if (Tools.GetPropertyAtPoint(Properties.PointToClient(Cursor.Position), Properties).Label == "BTexture" && DraggedItem is ContentItem)
                heightmap.BTexture = ((ContentItem)DraggedItem).GenerateFilename().Replace("\\Content", "Content");
            else if (Tools.GetPropertyAtPoint(Properties.PointToClient(Cursor.Position), Properties).Label == "TextureMap" && DraggedItem is ContentItem)
                heightmap.TextureMap = ((ContentItem)DraggedItem).GenerateFilename().Replace("\\Content", "Content");
            
            Properties.SelectedObject = heightmap;
            UpdateObjectProperties(heightmap, Properties, Scene);
        }

        public override void WriteToXML(System.Xml.XmlWriter writer, object obj)
        {
            XHeightMap heightmap = (XHeightMap)obj;

            writer.WriteStartElement("sceneitem");
            //writer.WriteAttributeString("Type", Name);
            writer.WriteAttributeString("ComponentID", heightmap.ComponentID.ToString());
            writer.WriteAttributeString("HeightMap", heightmap.HeightMap);
            writer.WriteAttributeString("TextureMap", heightmap.TextureMap);
            writer.WriteAttributeString("RTexture", heightmap.RTexture);
            writer.WriteAttributeString("GTexture", heightmap.GTexture);
            writer.WriteAttributeString("BTexture", heightmap.BTexture);
            writer.WriteAttributeString("AutoDraw", heightmap.AutoDraw.ToString());
            writer.WriteAttributeString("DrawOrder", heightmap.DrawOrder.ToString());
            writer.WriteAttributeString("EnvParamsNum", heightmap.environmentalParametersNumber.ToString());
            writer.WriteEndElement();
        }

        public override void LoadFromXML(System.Xml.XmlNode node, ListView scene)
        {
            ListViewItem sceneitem = SetupListViewItem();

            XHeightMap heightmap = new XHeightMap(ref X, node.Attributes["HeightMap"].InnerText, null, node.Attributes["RTexture"].InnerText, node.Attributes["GTexture"].InnerText, node.Attributes["BTexture"].InnerText, node.Attributes["TextureMap"].InnerText);
            heightmap.AutoDraw = bool.Parse(node.Attributes["AutoDraw"].InnerText);
            heightmap.DrawOrder = int.Parse(node.Attributes["DrawOrder"].InnerText);

            int paramsNum = int.Parse(node.Attributes["EnvParamsNum"].InnerText);
            heightmap.environmentalParametersNumber = paramsNum;

           // heightmap.ComponentID = int.Parse(node.Attributes["ComponentID"].InnerText);

            foreach (ListViewItem item in scene.Items)
                if (item.Tag is XEnvironmentParameters)
                    if (((XEnvironmentParameters)item.Tag).number == paramsNum)
                        heightmap.Params = (XEnvironmentParameters)item.Tag;

            if (!string.IsNullOrEmpty(heightmap.TextureMap) && !string.IsNullOrEmpty(heightmap.HeightMap) && heightmap.Params != null)
                 heightmap.Load(X.Content);

            //sceneitem.Text = Name;
            sceneitem.Tag = heightmap;
            sceneitem.Group = scene.Groups["Environment"];

            scene.Items.Add(sceneitem);
        }
    }
}
