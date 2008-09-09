using System.Windows.Forms;
using XEngine;

namespace X_Editor
{
    public class XTreeSystem_Plugin : ComponentPlugin
    {
        public XTreeSystem_Plugin(XMain X)
            : base(X)
        {
            type = typeof(XTreeSystem);
        }

        public override ListViewItem SetupListViewItem(XComponent component)
        {
            XTreeSystem treeSystem = new XTreeSystem(ref X, null, null);

            return base.SetupListViewItem(treeSystem);
        }

        public override void UpdateObjectProperties(object Input, PropertyGrid Properties, ListView Scene)
        {
            XTreeSystem treeSystem = (XTreeSystem)Input;

            if (!string.IsNullOrEmpty(treeSystem.TreeMapFile) && treeSystem.HeightMap != null)
            {
                try
                {
                    treeSystem.Load(X.Content);
                    treeSystem.GenerateTrees(X.DefaultCamera);
                }
                catch
                {
                    MessageBox.Show("There was a problem loading one of the TreeSystem's files.", "File Error");
                }
            }

            base.UpdateObjectProperties(Input, Properties, Scene);
        }

        public override void AcceptDragDrop(object Input, object DraggedItem, PropertyGrid Properties, ListView Scene)
        {
            XTreeSystem treeSystem = (XTreeSystem)Input;

            if (Tools.GetPropertyAtPoint(Properties.PointToClient(Cursor.Position), Properties).Label == "HeightMap" && DraggedItem is XHeightMap)
                treeSystem.HeightMap = (XHeightMap)DraggedItem;
            else if (Tools.GetPropertyAtPoint(Properties.PointToClient(Cursor.Position), Properties).Label == "TreeMapFile" && DraggedItem is ContentItem)
                treeSystem.TreeMapFile = ((ContentItem)DraggedItem).GenerateFilename().Replace("\\Content", "Content");

            UpdateObjectProperties(treeSystem, Properties, Scene);
        }

        public override void WriteToXML(System.Xml.XmlWriter writer, object obj)
        {
            XTreeSystem treeSystem = (XTreeSystem)obj;

            /*writer.WriteStartElement("sceneitem");
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
            writer.WriteEndElement();*/
        }

        public override void LoadFromXML(System.Xml.XmlNode node, ListView scene)
        {
            /*ListViewItem sceneitem = SetupListViewItem(null);

            XTreeSystem treeSystem = new XTreeSystem(ref X, node.Attributes["HeightMap"].InnerText, null, node.Attributes["RTexture"].InnerText, node.Attributes["GTexture"].InnerText, node.Attributes["BTexture"].InnerText, node.Attributes["TextureMap"].InnerText);
            treeSystem.AutoDraw = bool.Parse(node.Attributes["AutoDraw"].InnerText);
            treeSystem.DrawOrder = int.Parse(node.Attributes["DrawOrder"].InnerText);

            int paramsNum = int.Parse(node.Attributes["EnvParamsNum"].InnerText);
            treeSystem.environmentalParametersNumber = paramsNum;

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

            scene.Items.Add(sceneitem);*/
        }
    }
}
