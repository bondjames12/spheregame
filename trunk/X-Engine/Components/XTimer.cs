using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace XEngine
{
    public class XTimer : XComponent, XIUpdateable
    {
        /// <summary>
        /// Elapsed time since timer start in Milliseconds
        /// </summary>
        public double PassedTime;
        public double StartTime;
        public double StopTime;
        private GameTime LastUpdate = new GameTime();

        public bool IsRunning;

        double PauseTime;
        double TimePassedWhileStopped;

        public XTimer(ref XMain X) : base(ref X) { }

        public override void Update(ref GameTime gameTime)
        {
            LastUpdate = gameTime;

            if (IsRunning)
            {
                PassedTime = gameTime.TotalRealTime.TotalMilliseconds - StartTime;
                PassedTime -= TimePassedWhileStopped;
            }
        }

        public void Start()
        {
            StartTime = LastUpdate.TotalRealTime.TotalMilliseconds;
            IsRunning = true;
        }

        public void Pause()
        {
            PauseTime = LastUpdate.TotalRealTime.TotalMilliseconds;
            IsRunning = false;
        }

        public void Resume()
        {
            TimePassedWhileStopped += LastUpdate.TotalRealTime.TotalMilliseconds - PauseTime;
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

        public void TogglePaused( )
        {
            if (IsRunning)
                Pause();
            else
                Resume();
        }
    }
}
