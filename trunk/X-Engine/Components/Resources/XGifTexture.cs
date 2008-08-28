using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace XEngine
{
    /// <summary>
    /// A class which renders and updates an Animated strip texture
    /// </summary>
    public class XGifTexture : XComponent, XUpdateable
    {
        string Filename;
        XTimer Timer;
        XTextureSequence FrameSequence;
        
        int CurrentFrame;
        int FrameCount;

        private bool IsStopped = false;
        //Amount of time between frames
        private float FrameTime;

        float framerate = 15f;

        /// <summary>
        /// Set/Get the Framerate/second the animated texture will run at
        /// </summary>
        public float FrameRate
        {
            get { return framerate; }
            set { framerate = value; FrameTime = ((FrameCount / framerate) / FrameCount) * 1000; }
        }

        public XGifTexture(ref XMain X, string Filename)
            : base(ref X)
        {
            this.Filename = Filename;
            this.Timer = new XTimer(ref X);
        }

        public override void Load(ContentManager Content)
        {
            this.FrameSequence = Content.Load<XTextureSequence>(Filename);
            FrameCount = FrameSequence.Frames.Length;
            this.FrameTime = ((FrameCount / framerate) / FrameCount) * 1000;
            base.Load(Content);
        }

        #region Playback Control Functions
        public void Start()
        {
            IsStopped = false;
            CurrentFrame = 0;
            Timer.Start();
        }

        public void Stop()
        {
            IsStopped = true;
            CurrentFrame = 0;
            Timer.Stop();
        }

        public void Pause()
        {
            IsStopped = true;
            Timer.Pause();
        }

        public void Resume()
        {
            IsStopped = false;
            Timer.Resume();
        }

        public void Next()
        {
            if (CurrentFrame < FrameCount - 1)
                CurrentFrame++;
            else
                CurrentFrame = 0;
        }

        public void Previous()
        {
            if (CurrentFrame > 0)
                CurrentFrame--;
            else
                CurrentFrame = FrameCount - 1;
        }

        #endregion

        public override void Update(ref GameTime gameTime)
        {
            if (loaded)
            {
                //System.Diagnostics.Debug.Write(Timer.PassedTime.ToString());
                //did enough time pass for another frame?
                //this limited the speed of the animation so if we are drawing the game faster then
                // the desired framerate we skip and wait a bit
                //Also if the game is drawing slower then the framerate of the animation we should
                //skip some frames of animation to keepup!
                if (Timer.PassedTime >= FrameTime && !IsStopped)
                {
                    int framedelta = (int)(Timer.PassedTime / FrameTime);
                    if (framedelta > 1)
                    {//we are running to slow frameskip by the number of frame we are behind!
                        CurrentFrame += framedelta;
                        if (CurrentFrame >= FrameCount) CurrentFrame = 0;
                    }
                    else
                    {
                        if (CurrentFrame < FrameCount - 1)
                            CurrentFrame++;
                        else
                            CurrentFrame = 0;
                    }

                    Timer.Reset();
                    Timer.Start();
                }
            }
        }//end update method

        /// <summary>
        /// Returns the animations current texture frame as incremented through Update
        /// Call this before passing the texture into a shader.
        /// </summary>
        /// <returns></returns>
        public Texture2D GetTexture()
        {
            return FrameSequence.Frames[CurrentFrame];
        }

    }//end  class block
}//end namespace block