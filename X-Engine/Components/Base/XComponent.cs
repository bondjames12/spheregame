using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

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
        /// Base Drawing Order and Updating order!!!!!!
        /// Use this base ordering scheme
        /// Base is 20
        /// Game Console = 1000
        /// 2D Menu 500-600
        /// 2D Debug Text 300-400 (xframerate=301) 
        /// 2D Debug 200 - 300
        /// Particle sytems 180-200
        /// Game objects 100-180
        /// Terrain(21), water(22) 20-50
        /// Far Away objects (sky=19) 0-20
        /// Cameras 0-10
        /// Game Input 0
        /// 
        /// </summary>
        int drawOrder = 20;

        string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        uint componentID;
        public uint ComponentID
        {
            get { return componentID; }
            set
            {
                X.Tools.SetIDGenerator(value);
                if (componentID == value) return;
                //try to set the componentID, first check for existing object with same number and reassign!
                XComponent obj = X.Tools.GetXComponentByID(value);
                if (obj != null)
                {
                    System.Diagnostics.Debugger.Break();
                }
                else
                    componentID = value;
            }
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

        //Used by editor manipulators
        //public virtual Vector3 Translation { get { return Vector3.Zero; } set { } }
        //public virtual Quaternion Rotation { get { return Quaternion.Identity; } set { } }
        //public virtual Vector3 Scale { get { return Vector3.One; } set { } }

        /// <summary>
        /// Compare operator for sorting linked list
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(XComponent obj)
        {
            return this.drawOrder.CompareTo(obj.drawOrder);
        }

        public XComponent(ref XMain X)
        {
            this.componentID = X.Tools.GeneratorNewID();
            this.X = X;
            X.Components.Add(this);
            this.Name = this.ToString().Replace("XEngine.","") + this.componentID.ToString();
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
#if XBOX == FALSE
        /// <summary>
        /// Draws the model attached to the model into the pick buffer
        /// </summary>
        public virtual void DrawPick(XPickBuffer pick_buf, XICamera camera)
        {
        }
#endif
        public virtual void SetProjection(Matrix Projection)
        {
        }

        public virtual void Disable()
        {
            X.Components.Remove(this);
        }
    }
}
