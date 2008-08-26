using System;
using XEngine;
using System.Windows.Forms;
using System.Xml;

namespace X_Editor
{
    public abstract class ComponentPlugin
    {
        // This is the name that appears in the list of components
        public string Name;

        // This is the type of component this is an importer for. Use "type = typof(COMPONENT);"
        public Type type;

        internal XMain X;

        public ComponentPlugin(XMain X)
        {
            this.X = X;
        }

        // This is called when an item is dragged from the list of components to the viewport. The list view
        // item text should be the components name ("Name" from above), and its tag should be a new instance
        // of the component.
        public abstract ListViewItem SetupListViewItem();

        // This is called when the values of properties are changed for a component of this type. Use this for
        // re-creating vertex buffers, loading new textures, etc.
        public abstract void UpdateObjectProperties(object Input, PropertyGrid Properties, ListView Scene);

        // This is called when an item is dragged from the list of components in the scene to the properties
        // window. This is useful for setting properties that want an instance of a class, because one can
        // be dragged from the list of currently created classes into the properties window.
        public abstract void AcceptDragDrop(object Input, object DraggedItem, PropertyGrid Properties, ListView Scene);

        // This is called when the object should save itself to the project XML file
        public abstract void WriteToXML(XmlWriter writer, object obj);

        // This is called when the object should load itself from the project XML file
        public abstract void LoadFromXML(XmlNode node, ListView scene);
    }
}
