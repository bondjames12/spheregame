using System.Windows.Forms;
using Microsoft.Xna.Framework;
using XEngine;
using System.Collections.Generic;

namespace X_Editor
{
    public class XActor_Plugin : ComponentPlugin
    {
        public XActor_Plugin(XMain X) : base(X)
        {
            type = typeof(XActor);
            group = "Actors";
        }

        public override void AcceptDragDrop(object Input, object DraggedItem, PropertyGrid Properties, ListView Scene)
        {
            if (DraggedItem is XModel)
            {
                ((XActor)Input).model = (XModel)DraggedItem;
                ((XActor)Input).RebuildCollisionSkin(((XActor)Input).Translation);

                UpdateObjectProperties(Input, Properties, Scene);
                ((XActor)Input).Immovable = true;
            }
        }

        public override ListViewItem SetupListViewItem(ListViewItem item, XComponent component)
        {
            XActor actor = new XActor(ref X, null, Vector3.Zero,Vector3.Zero, 1);
            actor.NoCull = true;
            actor.Immovable = true;

            return base.SetupListViewItem(item, actor);
        }

        public override void UpdateObjectProperties(object Input, PropertyGrid Properties, ListView Scene)
        {
            XActor actor = (XActor)Input;

            if ((actor.PhysicsObject == null) || (actor.PhysicsObject.PhysicsSkin == null) || actor.PhysicsObject.PhysicsSkin.NumPrimitives <= 0) //this XActor we created or model we loaded does not have any collision primitives we can't continue to render this~
            {
                //X.Components.Remove(actor);
                MessageBox.Show("The XActor " + actor.Name + " ID: " + actor.ComponentID.ToString() + " Does not have a Collision Skin defined. Cannot load into XEngine at this time!", "Missing Component");
                return;
            }

            base.UpdateObjectProperties(Input, Properties, Scene);
        }

        public override void WriteToXML(System.Xml.XmlWriter writer, object obj)
        {
            XActor actor = (XActor)obj;
            writer.WriteStartElement("sceneitem");
            writer.WriteAttributeString("Type", actor.GetType().ToString());
            writer.WriteAttributeString("ComponentID", actor.ComponentID.ToString());
            writer.WriteAttributeString("AutoDraw", actor.AutoDraw.ToString());
            writer.WriteAttributeString("Name", actor.Name);
            writer.WriteAttributeString("DrawOrder", actor.DrawOrder.ToString());
            writer.WriteAttributeString("Immovable", actor.Immovable.ToString());
            writer.WriteAttributeString("Mass_editor", actor.Mass_editor.ToString());
            writer.WriteAttributeString("model_editor", actor.model_editor.ComponentID.ToString());
            writer.WriteAttributeString("Rotation", actor.Rotation.ToString());
            writer.WriteAttributeString("Translation", actor.Translation.ToString());
            writer.WriteAttributeString("Scale", actor.Scale.ToString());
            writer.WriteAttributeString("Velocity", actor.Velocity.ToString());
            writer.WriteAttributeString("CollisionEnabled", actor.CollisionEnabled.ToString());
            writer.WriteEndElement();
        }

        public override void LoadFromXML(System.Xml.XmlNode node, ListView scene, ref Dictionary<uint, List<uint>> Depends)
        {
            XTools tools = new XTools(X);
            
            XActor actor = new XActor(ref X, null,
                tools.ParseXMLVector3(node.Attributes["Translation"].InnerText),
                tools.ParseXMLVector3(node.Attributes["Velocity"].InnerText),
                float.Parse(node.Attributes["Mass_editor"].InnerText));
            
            actor.AutoDraw = bool.Parse(node.Attributes["AutoDraw"].InnerText);
            actor.DrawOrder = int.Parse(node.Attributes["DrawOrder"].InnerText);
            actor.Immovable = bool.Parse(node.Attributes["Immovable"].InnerText);
            actor.Rotation = tools.ParseXMLQuaternion(node.Attributes["Rotation"].InnerText);
            actor.ComponentID = uint.Parse(node.Attributes["ComponentID"].InnerText);
            actor.Name = node.Attributes["Name"].InnerText;
            actor.Scale = tools.ParseXMLVector3(node.Attributes["Scale"].InnerText);
            actor.CollisionEnabled = bool.Parse(node.Attributes["CollisionEnabled"].InnerText);
            
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
                    ((XActor)parent).model_editor = (XModel)child;
                    ((XActor)parent).model_editor.Parent = ((XActor)parent);
                    ((XActor)parent).RebuildCollisionSkin(((XActor)parent).Translation);
                }
            }
        }

    }
}
