using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace XEngine
{
    public class XTriggerVolume : XComponent, XIUpdateable, XITransform
    {
        protected XPhysicsObject obj;
        public bool continuous;
        public bool triggered;
        public Vector3 translation;
        public Quaternion rotation;
        public Vector3 scale;

        public event TriggerVolumeEventHandler Triggered;

        public XPhysicsObject TriggerKey
        {
            get { return obj; }
            set { obj = value; }
        }

        public Vector3 Translation
        {
            get { return translation; }
            set { translation = value; }
        }

        public Quaternion Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public Vector3 Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public XTriggerVolume(ref XMain X, XPhysicsObject obj, bool Continuous, Vector3 translation, Quaternion rotation, Vector3 scale) : base(ref X)
        {
            this.obj = obj;
            continuous = Continuous;
            triggered = false;
            this.translation = translation;
            this.rotation = rotation;
            this.scale = scale;
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
            X.Debug.Write("Trigger Fired: " + Name,false);
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
    }//end class XTriggerVolume

    public class TriggerVolumeEventArgs : EventArgs { }

    public delegate void TriggerVolumeEventHandler(object sender, TriggerVolumeEventArgs e);
}
