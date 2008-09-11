using System.Windows.Forms;
using XEngine;
using System.Collections.Generic;

namespace X_Editor
{
    public class XDynamicSky_Plugin : ComponentPlugin
    {
        public XDynamicSky_Plugin(XMain X)
            : base(X)
        {
            type = typeof(XDynamicSky);
            group = "Environment";
        }

        public override ListViewItem SetupListViewItem(ListViewItem item, XComponent component)
        {
            XDynamicSky sky = new XDynamicSky(ref X, null);
            return base.SetupListViewItem(item, sky);
        }

        public override void UpdateObjectProperties(object Input, PropertyGrid Properties, ListView Scene)
        {
            XDynamicSky sky = (XDynamicSky)Input;

            if (sky.Params != null)
            {
                sky.Load(X.Content);
            }

            base.UpdateObjectProperties(Input, Properties, Scene);
        }

        public override void AcceptDragDrop(object Input, object DraggedItem, PropertyGrid Properties, ListView Scene)
        {
            XDynamicSky sky = (XDynamicSky)Input;

            if (Tools.GetPropertyAtPoint(Properties.PointToClient(Cursor.Position), Properties).Label == "Params" && DraggedItem is XEnvironmentParameters)
                sky.Params = (XEnvironmentParameters)DraggedItem;

            Properties.SelectedObject = sky;
            UpdateObjectProperties(sky, Properties, Scene);
        }

        public override void WriteToXML(System.Xml.XmlWriter writer, object obj)
        {
            XDynamicSky sky = (XDynamicSky)obj;

            writer.WriteStartElement("sceneitem");
            writer.WriteAttributeString("Type", sky.GetType().ToString());
            writer.WriteAttributeString("ComponentID", sky.ComponentID.ToString());
            writer.WriteAttributeString("AutoDraw", sky.AutoDraw.ToString());
            writer.WriteAttributeString("DrawOrder", sky.DrawOrder.ToString());
            writer.WriteAttributeString("Params", sky.Params.ComponentID.ToString());
            writer.WriteAttributeString("Phi", sky.Phi.ToString());
            writer.WriteAttributeString("Theta", sky.Theta.ToString());
            writer.WriteAttributeString("RealTime", sky.RealTime.ToString());
            writer.WriteEndElement();
        }

        public override void LoadFromXML(System.Xml.XmlNode node, ListView scene, ref Dictionary<uint, List<uint>> Depends)
        {
            XDynamicSky sky = new XDynamicSky(ref X, null);

            sky.AutoDraw = bool.Parse(node.Attributes["AutoDraw"].InnerText);
            sky.DrawOrder = int.Parse(node.Attributes["DrawOrder"].InnerText);
            sky.Phi = float.Parse(node.Attributes["Phi"].InnerText);
            sky.Theta = float.Parse(node.Attributes["Theta"].InnerText);
            sky.RealTime = bool.Parse(node.Attributes["RealTime"].InnerText);
            sky.ComponentID = uint.Parse(node.Attributes["ComponentID"].InnerText);

            List<uint> dep = new List<uint>();
            dep.Add(uint.Parse(node.Attributes["Params"].InnerText));
            Depends.Add(sky.ComponentID, dep);

            X_Editor.Tools.AddXComponentToSceneList(scene, sky, group);
        }

        public override void AssignChildComponents(XComponent parent, ref List<uint> children)
        {
            foreach (uint childID in children)
            {
                XComponent child = X.Tools.GetXComponentByID(childID);
                if (child.GetType() == typeof(XEnvironmentParameters))
                {
                    ((XDynamicSky)parent).Params = (XEnvironmentParameters)child;
                    parent.Load(X.Content);
                }
            }
        }
    }
}