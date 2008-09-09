using System.Collections.Generic;
using System.Windows.Forms;
using XEngine;
using System.Xml;

namespace X_Editor
{
    public class ComponentPluginManager
    {
        public List<ComponentPlugin> Plugins = new List<ComponentPlugin>();

        public ListViewItem SetUpListViewItem(string Type)
        {
            ListViewItem item = new ListViewItem();
            foreach (ComponentPlugin plugin in Plugins)
                if (plugin.type.ToString() == Type)
                    item = plugin.SetupListViewItem(null);

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
            //foreach(XmlNode node in scenenode)
            //    foreach (ComponentPlugin plugin in Plugins)
            //        if (plugin.type.ToString() == node.Attributes["Type"].InnerText)
            //            plugin.LoadFromXML(node, scene);
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

            // Add any custom plugins for custom components here
        }
    }
}
