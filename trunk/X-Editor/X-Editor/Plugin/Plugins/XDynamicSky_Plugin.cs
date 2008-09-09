using System.Windows.Forms;
using XEngine;

namespace X_Editor
{
    public class XDynamicSky_Plugin : ComponentPlugin
    {
        public XDynamicSky_Plugin(XMain X)
            : base(X)
        {
            type = typeof(XDynamicSky);
        }

        public override ListViewItem SetupListViewItem(XComponent component)
        {
            XDynamicSky sky = new XDynamicSky(ref X, null);

            return base.SetupListViewItem(sky);
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
           // writer.WriteAttributeString("Type", Name);
            writer.WriteAttributeString("ComponentID", sky.ComponentID.ToString());
            writer.WriteAttributeString("AutoDraw", sky.AutoDraw.ToString());
            writer.WriteAttributeString("DrawOrder", sky.DrawOrder.ToString());
            writer.WriteAttributeString("EnvParamsNum", sky.environmentalParametersNumber.ToString());
            writer.WriteAttributeString("Phi", sky.Phi.ToString());
            writer.WriteAttributeString("Theta", sky.Theta.ToString());
            writer.WriteAttributeString("RealTime", sky.RealTime.ToString());
            writer.WriteEndElement();
        }

        public override void LoadFromXML(System.Xml.XmlNode node, ListView scene)
        {
            XDynamicSky sky = new XDynamicSky(ref X, null);

            sky.AutoDraw = bool.Parse(node.Attributes["AutoDraw"].InnerText);
            sky.DrawOrder = int.Parse(node.Attributes["DrawOrder"].InnerText);
            sky.Phi = float.Parse(node.Attributes["Phi"].InnerText);
            sky.Theta = float.Parse(node.Attributes["Theta"].InnerText);
            sky.RealTime = bool.Parse(node.Attributes["RealTime"].InnerText);
            //sky.ComponentID = int.Parse(node.Attributes["ComponentID"].InnerText);

            int paramsNum = int.Parse(node.Attributes["EnvParamsNum"].InnerText);
            sky.environmentalParametersNumber = paramsNum;

            foreach (ListViewItem item in scene.Items)
                if (item.Tag is XEnvironmentParameters)
                    if (((XEnvironmentParameters)item.Tag).number == paramsNum)
                        sky.Params = (XEnvironmentParameters)item.Tag;

            if (sky.Params != null)
                sky.Load(X.Content);

            ListViewItem sceneitem = new ListViewItem();
            //sceneitem.Text = Name;
            sceneitem.Tag = sky;

            sceneitem.Group = scene.Groups["Environment"];

            scene.Items.Add(sceneitem);
        }
    }
}