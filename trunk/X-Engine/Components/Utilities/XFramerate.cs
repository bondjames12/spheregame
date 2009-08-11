using Microsoft.Xna.Framework;

namespace XEngine
{
    public class XFrameRate : XComponent, XIUpdateable, XIDrawable
    {
        float fps;
        public float updateInterval = 1.0f;
        float timeSinceLastUpdate = 0.0f;
        float frameCount = 0;

        string Write = "";
        public bool DisplayFrameRate = false;

        public XFrameRate(XMain X)
            : base(ref X)
        { 
            DrawOrder = 100000;
        }

        //Update rate
        public override void Update(ref GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedRealTime.TotalSeconds;
            
            timeSinceLastUpdate += elapsed;

            if (timeSinceLastUpdate > updateInterval)
            {
                fps = frameCount / timeSinceLastUpdate;

                Write = "ElapsedRealTime sec:" + gameTime.ElapsedRealTime.TotalSeconds.ToString(".###") + " - " + "FPS:" +
                            fps.ToString("###");

                frameCount = 0;
                timeSinceLastUpdate =0;
            }

            if (DisplayFrameRate)
            {
                X.Debug.Write(Write, false);
            }
        }

        //Draw rate
        public override void Draw(ref GameTime gameTime, ref XCamera Camera)
        {
            frameCount++;
            
        }
    }
}
