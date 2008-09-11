using System.Windows.Forms;
using XEngine;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace X_Editor
{
    public class XHeightMap_Plugin : ComponentPlugin
    {
        public XHeightMap_Plugin(XMain X) : base(X)
        {
            type = typeof(XHeightMap);
            group = "Environment";
        }

        public override ListViewItem SetupListViewItem(ListViewItem item, XComponent component)
        {
            XHeightMap heightmap = new XHeightMap(ref X, null, null, null, null, null, null);
            return base.SetupListViewItem(item,heightmap);
        }

        public override void UpdateObjectProperties(object Input, PropertyGrid Properties, ListView Scene)
        {
            XHeightMap heightmap = (XHeightMap)Input;

            if (!string.IsNullOrEmpty(heightmap.HeightMap) && !string.IsNullOrEmpty(heightmap.TextureMap) && heightmap.Params != null)
            {
                try
                {
                    heightmap.Load(X.Content);
                }
                catch
                {
                    MessageBox.Show("There was a problem loading one of the heightmap's files. Check that the file exists and has been built with the Texture importer and processor.", "File Error");
                }
            }

            base.UpdateObjectProperties(Input, Properties, Scene);
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
            
            UpdateObjectProperties(heightmap, Properties, Scene);
        }

        public override void WriteToXML(System.Xml.XmlWriter writer, object obj)
        {
            XHeightMap heightmap = (XHeightMap)obj;

            writer.WriteStartElement("sceneitem");
            writer.WriteAttributeString("Type", this.type.ToString());
            writer.WriteAttributeString("AutoDraw", heightmap.AutoDraw.ToString());
            writer.WriteAttributeString("BTexture", heightmap.BTexture);
            writer.WriteAttributeString("ComponentID", heightmap.ComponentID.ToString());
            writer.WriteAttributeString("DrawOrder", heightmap.DrawOrder.ToString());
            writer.WriteAttributeString("GTexture", heightmap.GTexture);
            writer.WriteAttributeString("HeightMap", heightmap.HeightMap);
            writer.WriteAttributeString("Name", heightmap.Name);
            writer.WriteAttributeString("Params", heightmap.Params.ComponentID.ToString());
            writer.WriteAttributeString("RTexture", heightmap.RTexture);
            writer.WriteAttributeString("TextureMap", heightmap.TextureMap);
            
            writer.WriteEndElement();
        }

        public override void LoadFromXML(System.Xml.XmlNode node, ListView scene,ref Dictionary<uint, List<uint>> Depends)
        {
            XHeightMap heightmap = new XHeightMap(ref X, node.Attributes["HeightMap"].InnerText, null, node.Attributes["RTexture"].InnerText, node.Attributes["GTexture"].InnerText, node.Attributes["BTexture"].InnerText, node.Attributes["TextureMap"].InnerText);
            heightmap.AutoDraw = bool.Parse(node.Attributes["AutoDraw"].InnerText);
            heightmap.DrawOrder = int.Parse(node.Attributes["DrawOrder"].InnerText);
            heightmap.ComponentID = uint.Parse(node.Attributes["ComponentID"].InnerText);
            heightmap.Name = node.Attributes["Name"].InnerText;
            
            List<uint> dep = new List<uint>();
            dep.Add(uint.Parse(node.Attributes["Params"].InnerText));
            Depends.Add(heightmap.ComponentID, dep);

            if (!string.IsNullOrEmpty(heightmap.TextureMap) && !string.IsNullOrEmpty(heightmap.HeightMap) && heightmap.Params != null)
                 heightmap.Load(X.Content);

             X_Editor.Tools.AddXComponentToSceneList(scene, heightmap, group);
        }

        public override void AssignChildComponents(XComponent parent, ref List<uint> children)
        {
            foreach (uint childID in children)
            {
                XComponent child = X.Tools.GetXComponentByID(childID);
                if (child.GetType() == typeof(XEnvironmentParameters))
                {
                    ((XHeightMap)parent).Params = (XEnvironmentParameters)child;
                    parent.Load(X.Content);
                }
            }
        }
    }
}
