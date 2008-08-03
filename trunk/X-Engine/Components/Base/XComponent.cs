using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
namespace XEngine
{
    public class XComponent
    {
        public XMain X;

        bool loaded = false;
        bool autoDraw = true;
        public bool NoCull = false;
        int drawOrder = 20;

        /// <summary>
        /// Gets whether the component's "Load()" method has been called.
        /// </summary>
        public bool Loaded
        {
            get { return loaded; }
        }

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

        public XComponent(XMain X)
        {
            this.X = X;

            X.Components.Add(this);

            this.Name = this.ToString();
        }

        public virtual void Load(ContentManager Content)
        {
            loaded = true;
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(GameTime gameTime, XCamera Camera, XEnvironmentParameters environment)
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
