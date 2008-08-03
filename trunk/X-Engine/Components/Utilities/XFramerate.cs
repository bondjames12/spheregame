using Microsoft.Xna.Framework;

namespace XEngine
{
    public class XFrameRate : XComponent, XUpdateable
    {
        float fps;
        public float updateInterval = 1.0f;
        float timeSinceLastUpdate = 0.0f;
        float frameCount = 0;

        string Write = "";
        public bool DisplayFrameRate = false;

        public XFrameRate(XMain X) : base(X) { }

        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedRealTime.TotalSeconds;
            frameCount++;
            timeSinceLastUpdate += elapsed;

            if (timeSinceLastUpdate > updateInterval)
            {
                fps = frameCount / timeSinceLastUpdate;

                Write = "FPS: " + fps.ToString("###") + " - " + "ElapsedRealTime s:" +
                            gameTime.ElapsedRealTime.TotalSeconds.ToString(".###");

                frameCount = 0;
                timeSinceLastUpdate -= updateInterval;
            }

            if (DisplayFrameRate)
            {
                X.Debug.Write(Write, false);
            }
        }
    }
}
