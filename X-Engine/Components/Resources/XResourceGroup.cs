using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace XEngine
{
    public class XResourceGroup : XComponent
    {
        ContentManager content;
        List<XComponent> components;

        public XResourceGroup(ref XMain X)
            : base(ref X)
        {
            content = new ContentManager(X.Services);
            components = new List<XComponent>();
        }

        public void AddComponent(XComponent Component)
        {
            components.Add(Component);
        }

        public void Load()
        {
            foreach (XComponent component in components)
                component.Load(content);
        }
    }
}
