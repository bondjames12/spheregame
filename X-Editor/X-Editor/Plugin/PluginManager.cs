using System.Collections.Generic;
using System.Windows.Forms;
using XEngine;
using System.Xml;

namespace X_Editor
{
    public class ComponentPluginManager
    {
        public List<ComponentPlugin> Plugins = new List<ComponentPlugin>();
        public Dictionary<uint, List<uint>> Depends = new Dictionary<uint, List<uint>>();


        public ListViewItem SetUpListViewItem(string Type, ListView Scene)
        {
            ListViewItem item = new ListViewItem();
            foreach (ComponentPlugin plugin in Plugins)
                if (plugin.type.ToString() == Type)
                {
                    item = plugin.SetupListViewItem(new ListViewItem(), null);
                    item.Group = Scene.Groups[plugin.group];
                }
            return item;
        }

        public void UpdateObjectProperties(object obj, PropertyGrid Properties, ListView Scene)
        {
            foreach (ComponentPlugin plugin in Plugins)
                if (plugin.type == obj.GetType())
                    plugin.UpdateObjectProperties(obj, Properties, Scene);
        }

        public void AcceptDragDrop(object obj, object DraggedItem, PropertyGrid Properties, ListView Scene)
        {
            foreach (ComponentPlugin plugin in Plugins)
                if (plugin.type == obj.GetType())
                    plugin.AcceptDragDrop(obj, DraggedItem, Properties, Scene);
        }

        public void WriteToXML(XmlWriter writer, ListView scene)
        {
            foreach(ListViewItem item in scene.Items)
                foreach (ComponentPlugin plugin in Plugins)
                    if (plugin.type == X.Tools.GetXComponentByID(item.SubItems["colID"].Text).GetType())
                        plugin.WriteToXML(writer, X.Tools.GetXComponentByID(item.SubItems["colID"].Text));
        }

        public void LoadFromXML(XmlNodeList scenenode, ListView scene)
        {
            foreach(XmlNode node in scenenode)
                foreach (ComponentPlugin plugin in Plugins)
                    if (plugin.type.ToString() == node.Attributes["Type"].InnerText)
                        plugin.LoadFromXML(node, scene, ref Depends);

            //we need to keep a list of objects that are depend on each other
            //an object may have 1 or more child objects which are not linked by the above load routines
            //here we link these objects that recorded there dependencies during load
            foreach (uint keyID in Depends.Keys)
            {
                List<uint> children;
                if (Depends.TryGetValue(keyID,out children))
                {
                    //get parent object and match type
                    XComponent parent = X.Tools.GetXComponentByID(keyID);
                    
                    foreach (ComponentPlugin plugin in Plugins)
                        if (plugin.type == parent.GetType())
                            plugin.AssignChildComponents(parent,ref children);
                }
            }
        }

        XMain x;
        public XMain X
        {
            get { return x; }
            set { x = value; foreach (ComponentPlugin plugin in Plugins) { plugin.X = value; } }
        }

        public ComponentPluginManager(XMain X)
        {
            this.X = X;

            Plugins.Add(new XDynamicSky_Plugin(X));
            Plugins.Add(new XEnvironmentParameters_Plugin(X));
            Plugins.Add(new XHeightMap_Plugin(X));
            Plugins.Add(new XWater_Plugin(X));
            Plugins.Add(new XModel_Plugin(X));
            Plugins.Add(new XActor_Plugin(X));
            Plugins.Add(new XProp_Plugin(X));
            Plugins.Add(new XCamera_Plugin(X));
            Plugins.Add(new XTreeSystem_Plugin(X));
            Plugins.Add(new XSkyBox_Plugin(X));

            // Add any custom plugins for custom components here
        }
    }
}
