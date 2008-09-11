using System.Windows.Forms;
using Microsoft.Xna.Framework;
using XEngine;
using System.Collections.Generic;

namespace X_Editor
{
    public class XProp_Plugin : ComponentPlugin
    {
        public XProp_Plugin(XMain X)
            : base(X)
        {
            type = typeof(XProp);
            group = "Actors";
        }

        public override void AcceptDragDrop(object Input, object DraggedItem, PropertyGrid Properties, ListView Scene)
        {
            if (DraggedItem is XModel)
            {
                ((XProp)Input).model = (XModel)DraggedItem;
                UpdateObjectProperties(Input, Properties, Scene);
            }
        }

        public override ListViewItem SetupListViewItem(ListViewItem item, XComponent component)
        {
            XProp prop = new XProp(ref X, null, Vector3.Zero, Vector3.Zero, Matrix.Identity, Vector3.One);
            prop.NoCull = true;
            return base.SetupListViewItem(item, prop);
        }

        public override void UpdateObjectProperties(object Input, PropertyGrid Properties, ListView Scene)
        {
            //we changed a property do something?

            base.UpdateObjectProperties(Input, Properties, Scene);
        }

        public override void WriteToXML(System.Xml.XmlWriter writer, object obj)
        {
            //missing some here
            XProp prop = (XProp)obj;
            writer.WriteStartElement("sceneitem");

            writer.WriteAttributeString("Type", prop.GetType().ToString());
            writer.WriteAttributeString("AutoDraw", prop.AutoDraw.ToString());
            writer.WriteAttributeString("ComponentID", prop.ComponentID.ToString());
            writer.WriteAttributeString("DrawOrder", prop.DrawOrder.ToString());
            writer.WriteAttributeString("model_editor", prop.model_editor.ComponentID.ToString());
            writer.WriteAttributeString("ModelOffset_editor", prop.ModelOffset_editor.ToString());
            writer.WriteAttributeString("Name", prop.Name);
            writer.WriteAttributeString("Rotation", prop.Rotation.ToString());
            writer.WriteAttributeString("Scale", prop.Scale.ToString());
            writer.WriteAttributeString("Translation", prop.Translation.ToString());

            writer.WriteEndElement();
        }

        public override void LoadFromXML(System.Xml.XmlNode node, ListView scene, ref Dictionary<uint, List<uint>> Depends)
        {
            XTools tools = new XTools(X);

            XProp actor = new XProp(ref X,null,
                tools.ParseXMLVector3(node.Attributes["Translation"].InnerText),
                tools.ParseXMLVector3(node.Attributes["ModelOffset_editor"].InnerText),
                Matrix.CreateFromQuaternion(tools.ParseXMLQuaternion(node.Attributes["Rotation"].InnerText)),
                tools.ParseXMLVector3(node.Attributes["Scale"].InnerText));
            actor.NoCull = true;

            actor.AutoDraw = bool.Parse(node.Attributes["AutoDraw"].InnerText);
            actor.DrawOrder = int.Parse(node.Attributes["DrawOrder"].InnerText);
            actor.ComponentID = uint.Parse(node.Attributes["ComponentID"].InnerText);
            actor.Name = node.Attributes["Name"].InnerText;

            List<uint> dep = new List<uint>();
            dep.Add(uint.Parse(node.Attributes["model_editor"].InnerText));
            Depends.Add(actor.ComponentID, dep);

            X_Editor.Tools.AddXComponentToSceneList(scene, actor, group);
        }

        public override void AssignChildComponents(XComponent parent, ref List<uint> children)
        {
            foreach (uint childID in children)
            {
                XComponent child = X.Tools.GetXComponentByID(childID);
                if (child.GetType() == typeof(XModel))
                {
                    ((XProp)parent).model_editor = (XModel)child;
                }
            }
            //base.AssignChildComponents(parent, ref children);
        }
    }
}
