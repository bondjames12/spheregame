using System.Windows.Forms;
using Microsoft.Xna.Framework;
using XEngine;
using System.Collections.Generic;

namespace X_Editor
{
    public class XCubeTriggerVolume_Plugin : ComponentPlugin
    {
        public XCubeTriggerVolume_Plugin(XMain X)
            : base(X)
        {
            type = typeof(XCubeTriggerVolume);
            group = "Actors";
        }

        public override void AcceptDragDrop(object Input, object DraggedItem, PropertyGrid Properties, ListView Scene)
        {
            
        }

        public override ListViewItem SetupListViewItem(ListViewItem item, XComponent component)
        {


            return base.SetupListViewItem(item, component);
        }

        public override void UpdateObjectProperties(object Input, PropertyGrid Properties, ListView Scene)
        {
            

            base.UpdateObjectProperties(Input, Properties, Scene);
        }

        public override void WriteToXML(System.Xml.XmlWriter writer, object obj)
        {
            
        }

        public override void LoadFromXML(System.Xml.XmlNode node, ListView scene, ref Dictionary<uint, List<uint>> Depends)
        {
            
             
        }

    }
}//end namespace
