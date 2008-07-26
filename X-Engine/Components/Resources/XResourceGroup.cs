﻿using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace XEngine
{
    public class XResourceGroup : XComponent
    {
        ContentManager content;
        List<XComponent> components;

        public XResourceGroup(XMain X)
            : base(X)
        {
            content = new ContentManager(X.Game.Services);
            components = new List<XComponent>();
        }

        public void AddComponent(XComponent Component)
        {
            if (Component is XLoadable)
                components.Add(Component);
        }

        public void Load()
        {
            foreach (XComponent component in components)
                component.Load(content);
        }
    }
}
