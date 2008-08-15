﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System;
namespace XEngine
{
    public class XComponent : IComparable<XComponent>
    {
        public XMain X;

        /// <summary>
        /// The DebugMode flag is used to turn on and off debug features of objects such as 
        /// bounding rendering and on screen status messages.
        /// </summary>
        public bool DebugMode = true;
        //Whether the component's "Load()" method has been called.
        public bool loaded = false;
        bool autoDraw = true;
        public bool AlphaBlendable = false;
        public bool NoCull = false;
        /// <summary>
        /// Base Drawing Order
        /// </summary>
        int drawOrder = 20;

        string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets if the renderer should automatically draw the component in the main draw loop.
        /// </summary>
        public bool AutoDraw
        {
            get { return autoDraw; }
            set { autoDraw = value; }
        }

        /// <summary>
        /// Gets or Sets which draw group this component will draw with. Lower values will be drawn first.
        /// </summary>
        public int DrawOrder
        {
            get { return drawOrder; }
            set { drawOrder = value; }
        }

        /// <summary>
        /// Compare operator for sorting linked list
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(XComponent obj)
        {
            return this.drawOrder.CompareTo(obj.drawOrder);
        }


        public XComponent(XMain X)
        {
            this.X = X;
            X.Components.Add(this);
            this.Name = this.ToString();
            //sort X.Components list according to Draworder
            X.Components.Sort();
        }

        public virtual void Load(ContentManager Content)
        {
            loaded = true;
        }

        public virtual void Update(ref GameTime gameTime)
        {
        }

        public virtual void Draw(ref GameTime gameTime,ref XCamera Camera)
        {
        }

        public virtual void SetProjection(Matrix Projection)
        {
        }

        public virtual void Disable()
        {
            X.Components.Remove(this);
        }
    }
}
