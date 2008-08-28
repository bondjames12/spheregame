using System.Windows.Forms;
using Microsoft.Xna.Framework;
using XEngine;

namespace X_Editor
{
    public class XActor_Plugin : ComponentPlugin
    {
        public XActor_Plugin(XMain X) : base(X)
        {
            type = typeof(XActor);
            Name = "XActor";
        }

        public override void AcceptDragDrop(object Input, object DraggedItem, PropertyGrid Properties, ListView Scene)
        {
            if (DraggedItem is XModel)
            {
                ((XActor)Input).model = (XModel)DraggedItem;
                //((XActor)Input).Size = ((XActor)Input).Size;

                UpdateObjectProperties(Input, Properties, Scene);
            }
        }

        public override ListViewItem SetupListViewItem()
        {
            ListViewItem item = new ListViewItem();
            item.Text = Name;

            XActor actor = new XActor(ref X, new XModel(ref X, null), Vector3.Zero, Vector3.Zero,Vector3.Zero, 1);
            actor.NoCull = true;
            item.Tag = actor;
            
            return item;
        }

        public override void UpdateObjectProperties(object Input, PropertyGrid Properties, ListView Scene)
        {
            XActor actor = (XActor)Input;

            XActor newAct = new XActor(ref X, actor.model, actor.Position, actor.modeloffset,  actor.Velocity, actor.Mass_editor);
            newAct.Immovable = actor.Immovable;
            newAct.AutoDraw = actor.AutoDraw;
            newAct.DrawOrder = actor.DrawOrder;
            newAct.Rotation_editor = actor.Rotation_editor;
            newAct.DebugMode = actor.DebugMode;
            //newAct.ComponentID = actor.ComponentID;

            if ((newAct.PhysicsObject == null) || (newAct.PhysicsObject.PhysicsSkin == null) || newAct.PhysicsObject.PhysicsSkin.NumPrimitives <= 0) //this XActor we created or model we loaded does not have any collision primitives we can't continue to render this~
            {
                X.Components.Remove(newAct);
                return;
            }

            if (Properties != null)
                Properties.SelectedObject = newAct;

            X.Components.Remove(X.Tools.GetXComponentByID(actor.ComponentID)); 
        }

        public override void WriteToXML(System.Xml.XmlWriter writer, object obj)
        {
            XActor actor = (XActor)obj;
            writer.WriteStartElement("sceneitem");
            writer.WriteAttributeString("Type", Name);
            writer.WriteAttributeString("ComponentID", actor.ComponentID.ToString());
            //REMOVED: writer.WriteAttributeString("ActorType", actor.Type.ToString());
            writer.WriteAttributeString("AutoDraw", actor.AutoDraw.ToString());
            writer.WriteAttributeString("DrawOrder", actor.DrawOrder.ToString());
            writer.WriteAttributeString("Immovable", actor.Immovable.ToString());
            writer.WriteAttributeString("Mass", actor.Mass_editor.ToString());
            writer.WriteAttributeString("ModelNumber", actor.modelNumber.ToString());
            writer.WriteAttributeString("ModelOffset", actor.modeloffset.ToString());
            writer.WriteAttributeString("Rotation", actor.Rotation_editor.ToString());
            writer.WriteAttributeString("Position", actor.Position.ToString());
            writer.WriteAttributeString("ModelScale", actor.Scale.ToString());
            //REMOVED: writer.WriteAttributeString("Size", actor.Size.ToString());
            writer.WriteAttributeString("Velocity", actor.Velocity.ToString());
            writer.WriteEndElement();
        }

        public override void LoadFromXML(System.Xml.XmlNode node, ListView scene)
        {
            XTools tools = new XTools(X);

            XActor actor = new XActor(ref X,new XModel(ref X, null),
                tools.ParseXMLVector3(node.Attributes["Position"].InnerText),
                tools.ParseXMLVector3(node.Attributes["ModelOffset"].InnerText),
                tools.ParseXMLVector3(node.Attributes["Velocity"].InnerText),
                float.Parse(node.Attributes["Mass"].InnerText));

            actor.AutoDraw = bool.Parse(node.Attributes["AutoDraw"].InnerText);
            actor.DrawOrder = int.Parse(node.Attributes["DrawOrder"].InnerText);
            actor.Immovable = bool.Parse(node.Attributes["Immovable"].InnerText);
            actor.modelNumber = int.Parse(node.Attributes["ModelNumber"].InnerText);
            actor.Rotation_editor = tools.ParseXMLVector3(node.Attributes["Rotation"].InnerText);
            //actor.ComponentID = int.Parse(node.Attributes["ComponentID"].InnerText);

            foreach(ListViewItem item in scene.Items)
                if (item.Tag is XModel)
                    if (((XModel)item.Tag).Number == actor.modelNumber)
                    {
                        actor.model = ((XModel)item.Tag);
                        if (!string.IsNullOrEmpty(((XModel)item.Tag).Filename))
                            ((XModel)item.Tag).Load(X.Content);
                    }

            ListViewItem sceneitem = new ListViewItem(Name);
            sceneitem.Tag = actor;
            sceneitem.Group = scene.Groups["Actors"];

            scene.Items.Add(sceneitem);
             
        }

        /*REMOVED: XActor.ActorType GetActorType(string type)
        {
            if (type == "BowlingPin")
                return XActor.ActorType.BowlingPin;
            else if (type == "Box")
                return XActor.ActorType.Box;
            else if (type == "Capsule")
                return XActor.ActorType.Capsule;
            else if (type == "Mesh")
                return XActor.ActorType.Mesh;
            else if (type == "Sphere")
                return XActor.ActorType.Sphere;
            else if (type == "Plane")
                return XActor.ActorType.Plane;

            return XActor.ActorType.Box;
        }*/
    }
}
