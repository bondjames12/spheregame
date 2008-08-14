using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace XEngine
{
    public class XTimer : XComponent, XUpdateable
    {
        public float PassedTime;
        public float StartTime;
        public float StopTime;

        public bool IsRunning;

        float PauseTime;
        float TimePassedWhileStopped;

        public XTimer(XMain X) : base(X) { }

        public override void Update(ref GameTime gameTime)
        {
            if (IsRunning)
            {
                PassedTime = gameTime.TotalGameTime.Seconds - StartTime;
                PassedTime -= TimePassedWhileStopped;
            }
        }

        public void Start(GameTime gameTime)
        {
            StartTime = gameTime.TotalGameTime.Seconds;
            IsRunning = true;
        }

        public void Pause(GameTime gameTime)
        {
            PauseTime = gameTime.TotalGameTime.Seconds;
            IsRunning = false;
        }

        public void Resume(GameTime gameTime)
        {
            TimePassedWhileStopped += gameTime.TotalGameTime.Seconds - PauseTime;
            IsRunning = true;
        }

        public void Stop()
        {
            StopTime = PassedTime + StartTime;
            IsRunning = false;
        }

        public void Reset()
        {
            StopTime = 0;
            PassedTime = 0;
            StartTime = 0;
            TimePassedWhileStopped = 0;
            PauseTime = 0;
            IsRunning = false;
        }

        public void TogglePaused(GameTime gameTime)
        {
            if (IsRunning)
                Pause(gameTime);
            else
                Resume(gameTime);
        }
    }
}
