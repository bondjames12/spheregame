using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace XEngine
{
    public class XTriggerVolume : XComponent, XIUpdateable
    {
        protected XActor actor;
        bool continuous;
        public bool triggered;

        public event TriggerVolumeEventHandler Triggered;

        public XTriggerVolume(ref XMain X, XActor Actor, bool Continuous) : base(ref X)
        {
            this.actor = Actor;
            this.continuous = Continuous;
            triggered = false;
        }



        public override void Update(ref GameTime gameTime)
        {
            if ((continuous) || (!continuous && !triggered))
                if (CheckForIntersection())
                    Fire();
        }

        void Fire()
        {
            if (!continuous)
                triggered = true;

            OnTriggered(new TriggerVolumeEventArgs());
        }

        protected virtual void OnTriggered(TriggerVolumeEventArgs e)
        {
            if (Triggered != null)
                Triggered(this, e);
        }

        protected virtual bool CheckForIntersection()
        {
            return false;
        }
    }

    public class TriggerVolumeEventArgs : EventArgs { }

    public delegate void TriggerVolumeEventHandler(object sender, TriggerVolumeEventArgs e);
}
