using System.Windows.Forms;
using Microsoft.Xna.Framework;
using XEngine;
using System.Collections.Generic;

namespace X_Editor
{
    public class XTree_Plugin : ComponentPlugin
    {
        public XTree_Plugin(XMain X)
            : base(X)
        {
            type = typeof(XTree);
            group = "Actors";
        }

        public override void AcceptDragDrop(object Input, object DraggedItem, PropertyGrid Properties, ListView Scene)
        {
            if (DraggedItem is XTreeModel)
            {
                ((XTree)Input).tree = (XTreeModel)DraggedItem;
                ((XTree)Input).Immovable = true;
                ((XTree)Input).Load(X.Content);

                UpdateObjectProperties(Input, Properties, Scene);
            }
        }

        public override ListViewItem SetupListViewItem(ListViewItem item, XComponent component)
        {
            XTree actor = new XTree(ref X, null, Vector3.Zero, Vector3.One);
            actor.NoCull = true;

            return base.SetupListViewItem(item, actor);
        }

        public override void UpdateObjectProperties(object Input, PropertyGrid Properties, ListView Scene)
        {
            XTree actor = (XTree)Input;
            actor.GenerateFrustumBB();


            base.UpdateObjectProperties(Input, Properties, Scene);
        }

        public override void WriteToXML(System.Xml.XmlWriter writer, object obj)
        {
            XTree actor = (XTree)obj;
            writer.WriteStartElement("sceneitem");
            writer.WriteAttributeString("Type", actor.GetType().ToString());
            writer.WriteAttributeString("ComponentID", actor.ComponentID.ToString());
            writer.WriteAttributeString("AutoDraw", actor.AutoDraw.ToString());
            writer.WriteAttributeString("Name", actor.Name);
            writer.WriteAttributeString("DrawOrder", actor.DrawOrder.ToString());
            writer.WriteAttributeString("Immovable", actor.Immovable.ToString());
            writer.WriteAttributeString("Mass", actor.Mass.ToString());
            writer.WriteAttributeString("model_editor", actor.model_editor.ComponentID.ToString());
            writer.WriteAttributeString("Rotation", actor.Rotation.ToString());
            writer.WriteAttributeString("Translation", actor.Translation.ToString());
            writer.WriteAttributeString("Scale", actor.Scale.ToString());
            writer.WriteAttributeString("Velocity", actor.Velocity.ToString());
            writer.WriteAttributeString("CollisionEnabled", actor.CollisionEnabled.ToString());
            writer.WriteAttributeString("RenderLeaves", actor.RenderLeaves.ToString());
            writer.WriteEndElement();
        }

        public override void LoadFromXML(System.Xml.XmlNode node, ListView scene, ref Dictionary<uint, List<uint>> Depends)
        {
            XTools tools = new XTools(X);

            XTree actor = new XTree(ref X, null,
                tools.ParseXMLVector3(node.Attributes["Translation"].InnerText),
                tools.ParseXMLVector3(node.Attributes["Scale"].InnerText)
                );
            
            actor.AutoDraw = bool.Parse(node.Attributes["AutoDraw"].InnerText);
            actor.DrawOrder = int.Parse(node.Attributes["DrawOrder"].InnerText);
            actor.Immovable = bool.Parse(node.Attributes["Immovable"].InnerText);
            actor.Rotation = tools.ParseXMLQuaternion(node.Attributes["Rotation"].InnerText);
            actor.ComponentID = uint.Parse(node.Attributes["ComponentID"].InnerText);
            actor.Mass = float.Parse(node.Attributes["Mass"].InnerText);
            actor.Velocity = tools.ParseVector3(node.Attributes["Velocity"].InnerText);
            actor.Name = node.Attributes["Name"].InnerText;
            actor.Scale = tools.ParseXMLVector3(node.Attributes["Scale"].InnerText);
            actor.CollisionEnabled = bool.Parse(node.Attributes["CollisionEnabled"].InnerText);
            actor.RenderLeaves = bool.Parse(node.Attributes["RenderLeaves"].InnerText);
            
            List<uint> dep = new List<uint>();
            dep.Add(uint.Parse(node.Attributes["model_editor"].InnerText));
            Depends.Add(actor.ComponentID, dep);
            
            X_Editor.Tools.AddXComponentToSceneList(scene, actor, group);

        }

        public override void AssignChildComponents(XComponent parent, ref List<uint> children)
        {
            if(children.Count == 1)
            {
                XTree xparent = (XTree)parent;
                XTreeModel child = (XTreeModel)X.Tools.GetXComponentByID(children[0]);
                if (child.GetType() == typeof(XTreeModel))
                {
                    xparent.model_editor = child;
                    xparent.Load(X.Content);
                }
            }
        }

    }
}
