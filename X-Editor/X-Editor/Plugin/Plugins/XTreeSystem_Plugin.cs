using System.Windows.Forms;
using XEngine;
using System.Collections.Generic;

namespace X_Editor
{
    public class XTreeSystem_Plugin : ComponentPlugin
    {
        public XTreeSystem_Plugin(XMain X)
            : base(X)
        {
            type = typeof(XTreeSystem);
            group = "Environment";
        }

        public override ListViewItem SetupListViewItem(ListViewItem item, XComponent component)
        {
            XTreeSystem treeSystem = new XTreeSystem(ref X, null, null);
            return base.SetupListViewItem(item, treeSystem);
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

            writer.WriteStartElement("sceneitem");
            writer.WriteAttributeString("Type", treeSystem.GetType().ToString());
            writer.WriteAttributeString("ComponentID", treeSystem.ComponentID.ToString());
            writer.WriteAttributeString("AutoDraw", treeSystem.AutoDraw.ToString());
            writer.WriteAttributeString("DrawOrder", treeSystem.DrawOrder.ToString());
            writer.WriteAttributeString("HeightMap", treeSystem.HeightMap.ComponentID.ToString());
            writer.WriteAttributeString("Name", treeSystem.Name);
            writer.WriteAttributeString("TreeMapFile", treeSystem.TreeMapFile);
            
            writer.WriteEndElement();
        }

        public override void LoadFromXML(System.Xml.XmlNode node, ListView scene, ref Dictionary<uint, List<uint>> Depends)
        {
            XTreeSystem treeSystem = new XTreeSystem(ref X, node.Attributes["TreeMapFile"].InnerText, null);
            treeSystem.AutoDraw = bool.Parse(node.Attributes["AutoDraw"].InnerText);
            treeSystem.DrawOrder = int.Parse(node.Attributes["DrawOrder"].InnerText);
            treeSystem.ComponentID = uint.Parse(node.Attributes["ComponentID"].InnerText);
            treeSystem.Name = node.Attributes["Name"].InnerText;

            List<uint> dep = new List<uint>();
            dep.Add(uint.Parse(node.Attributes["HeightMap"].InnerText));
            Depends.Add(treeSystem.ComponentID, dep);

            X_Editor.Tools.AddXComponentToSceneList(scene, treeSystem, "Environment");
        }

        public override void AssignChildComponents(XComponent parent, ref List<uint> children)
        {
            foreach (uint childID in children)
            {
                XComponent child = X.Tools.GetXComponentByID(childID);
                if (child.GetType() == typeof(XHeightMap))
                {
                    ((XTreeSystem)parent).HeightMap = (XHeightMap)child;
                    ((XTreeSystem)parent).Load(X.Content);
                    ((XTreeSystem)parent).GenerateTrees(X.DefaultCamera);
                }
            }
        }
    }
}
