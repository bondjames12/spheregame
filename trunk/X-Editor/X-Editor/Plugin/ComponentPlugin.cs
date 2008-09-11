using System;
using XEngine;
using System.Windows.Forms;
using System.Xml;
using System.Collections.Generic;

namespace X_Editor
{
    public class ComponentPlugin
    {
        // This is the name that appears in the list of components
        //public string Name;

        // This is the type of component this is an importer for. Use "type = typof(COMPONENT);"
        public Type type;

        public string group;

        internal XMain X;

        public ComponentPlugin(XMain X)
        {
            this.X = X;
        }

        // This is called when an item is dragged from the list of components to the viewport. The list view
        // item text should be the components name ("Name" from above), and its tag should be a new instance
        // of the component.
        public virtual ListViewItem SetupListViewItem(ListViewItem item, XComponent component)
        {
            //custom name
            ListViewItem.ListViewSubItem lvtype = new ListViewItem.ListViewSubItem();
            lvtype.Name = "colName";
            lvtype.Text = component.Name;

            //id
            ListViewItem.ListViewSubItem lvid = new ListViewItem.ListViewSubItem();
            lvid.Name = "colID";
            lvid.Text = component.ComponentID.ToString();


            item.Text = component.ToString();
            item.Name = component.ToString();
            item.SubItems.Add(lvtype);
            item.SubItems.Add(lvid);

            return item;
        }

        // This is called when the values of properties are changed for a component of this type. Use this for
        // re-creating vertex buffers, loading new textures, etc.
        public virtual void UpdateObjectProperties(object Input, PropertyGrid Properties, ListView Scene)
        {
            //update scene list component name
            foreach (ListViewItem item in Scene.Items)
            {//search for item
                if (item.SubItems["colID"].Text == ((XComponent)Input).ComponentID.ToString())
                    item.SubItems["colName"].Text = ((XComponent)Input).Name;
            }

            //forces the properties list to update to display the changes
            if (Properties != null && Properties.SelectedObject == Input)
                Properties.SelectedObject = Input;
        
        }

        // This is called when an item is dragged from the list of components in the scene to the properties
        // window. This is useful for setting properties that want an instance of a class, because one can
        // be dragged from the list of currently created classes into the properties window.
        public virtual void AcceptDragDrop(object Input, object DraggedItem, PropertyGrid Properties, ListView Scene)
        { }

        // This is called when the object should save itself to the project XML file
        public virtual void WriteToXML(XmlWriter writer, object obj)
        { }

        // This is called when the object should load itself from the project XML file
        public virtual void LoadFromXML(XmlNode node, ListView scene,ref Dictionary<uint, List<uint>> Depends)
        { }

        // This is called to hookup any child objects once loading is done
        public virtual void AssignChildComponents(XComponent parent, ref List<uint> children)
        { 
        }
    }
}
