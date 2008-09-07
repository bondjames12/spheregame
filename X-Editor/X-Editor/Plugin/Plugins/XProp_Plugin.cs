using System.Windows.Forms;
using Microsoft.Xna.Framework;
using XEngine;

namespace X_Editor
{
    public class XProp_Plugin : ComponentPlugin
    {
        public XProp_Plugin(XMain X)
            : base(X)
        {
            type = typeof(XProp);
            //Name = "XProp";
        }

        public override void AcceptDragDrop(object Input, object DraggedItem, PropertyGrid Properties, ListView Scene)
        {
            if (DraggedItem is XModel)
            {
                ((XProp)Input).model = (XModel)DraggedItem;
                //((XProp)Input).Size = ((XProp)Input).Size;

                UpdateObjectProperties(Input, Properties, Scene);
            }
        }

        public override ListViewItem SetupListViewItem()
        {
            XProp prop = new XProp(ref X, new XModel(ref X, null), Vector3.Zero, Vector3.Zero, Matrix.Identity, Vector3.One);
            prop.NoCull = true;

            ListViewItem item = new ListViewItem();

            //custom name
            ListViewItem.ListViewSubItem lvtype = new ListViewItem.ListViewSubItem();
            lvtype.Name = "colName";
            lvtype.Text = prop.Name;

            //id
            ListViewItem.ListViewSubItem lvid = new ListViewItem.ListViewSubItem();
            lvid.Name = "colID";
            lvid.Text = prop.ComponentID.ToString();


            item.Text = prop.ToString();
            item.Name = prop.ToString();
            item.SubItems.Add(lvtype);
            item.SubItems.Add(lvid);

            return item;
        }

        public override void UpdateObjectProperties(object Input, PropertyGrid Properties, ListView Scene)
        {
            //we changed a property do something?

            //update scene list component name
            foreach (ListViewItem item in Scene.Items)
            {//search for item
                if (item.SubItems["colID"].Text == ((XProp)Input).ComponentID.ToString())
                    item.SubItems["colName"].Text = ((XProp)Input).Name;
            }

            //forces the properties list to update to display the changes
            if (Properties != null && Properties.SelectedObject == Input)
                Properties.SelectedObject = Input;
        }

        public override void WriteToXML(System.Xml.XmlWriter writer, object obj)
        {
            //missing some here
            XProp actor = (XProp)obj;
            writer.WriteStartElement("sceneitem");
            //writer.WriteAttributeString("Type", Name);
            writer.WriteAttributeString("ComponentID", actor.ComponentID.ToString());
            writer.WriteAttributeString("AutoDraw", actor.AutoDraw.ToString());
            writer.WriteAttributeString("DrawOrder", actor.DrawOrder.ToString());
            writer.WriteAttributeString("ModelNumber", actor.modelNumber.ToString());
            writer.WriteAttributeString("ModelOffset", actor.modeloffset.ToString());
            writer.WriteAttributeString("Position", actor.position.ToString());
            writer.WriteAttributeString("ModelScale", actor.scale.ToString());
            writer.WriteEndElement();
        }

        public override void LoadFromXML(System.Xml.XmlNode node, ListView scene)
        {
            XTools tools = new XTools(X);

            XProp actor = new XProp(ref X,new XModel(ref X, null),
                tools.ParseXMLVector3(node.Attributes["Position"].InnerText),
                tools.ParseXMLVector3(node.Attributes["ModelOffset"].InnerText),
                tools.ParseMatrix(node.Attributes["Orientation"].InnerText),
                tools.ParseXMLVector3(node.Attributes["Scale"].InnerText));

            actor.AutoDraw = bool.Parse(node.Attributes["AutoDraw"].InnerText);
            actor.DrawOrder = int.Parse(node.Attributes["DrawOrder"].InnerText);
            actor.modelNumber = int.Parse(node.Attributes["ModelNumber"].InnerText);
            //actor.ComponentID = int.Parse(node.Attributes["ComponentID"].InnerText);

            foreach(ListViewItem item in scene.Items)
                if (item.Tag is XModel)
                    if (((XModel)item.Tag).Number == actor.modelNumber)
                    {
                        actor.model = ((XModel)item.Tag);
                        if (!string.IsNullOrEmpty(((XModel)item.Tag).Filename))
                            ((XModel)item.Tag).Load(X.Content);
                    }

            //ListViewItem sceneitem = new ListViewItem(Name);
            //sceneitem.Tag = actor;
            //sceneitem.Group = scene.Groups["Actors"];

            //scene.Items.Add(sceneitem);
             
        }
    }
}
