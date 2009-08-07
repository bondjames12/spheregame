using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using KiloWatt.Base.Animation;

namespace XEngine
{
    public class XAnimatedActor : XActor, XIUpdateable
    {
        ModelDraw loadedModel_;             //  loaded geometry
        AnimationSet animations_;           //  the animations I have to choose from
        AnimationInstance[] instances_;     //  the animation data, as loaded
        IBlendedAnimation[] blended_;       //  state about the different animations (that can change)
        AnimationBlender blender_;          //  object that blends between playing animations
        private int curAnimationInstance_ =  -1;     //  which animation is playing? (-1 for none)
        private int OldAnimationInstance_ = -1;
        //Specific data
        EffectParameter pose_;
        public Vector4[] PoseData;
        public bool HasPose { get { return pose_ != null; } }

        /// <summary>
        /// Change the index to change which animation is playing
        /// </summary>
        public int AnimationIndex
        {
            get
            {
                return curAnimationInstance_;
            }
            set
            {
                OldAnimationInstance_ = curAnimationInstance_;
                //must be loaded first before we can set this
                curAnimationInstance_ = value;
                if (loaded)
                {
                    if (animations_.AnimationDictionary.Count > 0)
                    {
                        if (curAnimationInstance_ < 0)
                            curAnimationInstance_ = animations_.AnimationDictionary.Count - 1;
                        if (curAnimationInstance_ >= animations_.AnimationDictionary.Count)
                            curAnimationInstance_ = 0;
                    }
                }
            }
        }

        public XAnimatedActor(ref XMain X, XModel model, Vector3 Position,
                                    Vector3 Velocity, float Mass) :
            base(ref X, model, Position, Velocity, Mass)
        {
            if (model != null) model.Parent = this;
        }

        public XAnimatedActor(ref XMain X, XPhysicsObject Object, XModel model, Vector3 ModelScale,
                                    Vector3 Velocity, float Mass)
            :
                base(ref X, Object, model, ModelScale, Velocity, Mass)
        {
            if (model != null) model.Parent = this;
        }

        public override void Load(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            base.Load(Content);

            //  load the model from disk, and prepare it for animation
            loadedModel_ = new ModelDraw(ref X, this.model.Model, this.model.Name);

            //  create a blender that can compose the animations for transition
            blender_ = new AnimationBlender(loadedModel_.Model, loadedModel_.Name);
            loadedModel_.AnimationInstance = blender_;

            //  clear current state
            curAnimationInstance_ = 0;
            instances_ = null;
            blended_ = null;

            //  get the list of animations from our dictionary
            Dictionary<string, object> tag = loadedModel_.Model.Tag as Dictionary<string, object>;
            object aobj = null;
            if (tag != null)
                tag.TryGetValue("AnimationSet", out aobj);
            animations_ = aobj as AnimationSet;

            //  set up animations
            if (animations_ != null)
            {
                instances_ = new AnimationInstance[animations_.NumAnimations];
                //  I'll need a BlendedAnimation per animation, so that I can let the 
                //  blender object transition between them.
                blended_ = new IBlendedAnimation[instances_.Length];
                int ix = 0;
                foreach (Animation a in animations_.Animations)
                {
                    instances_[ix] = new AnimationInstance(a);
                    blended_[ix] = AnimationBlender.CreateBlendedAnimation(instances_[ix]);
                    ++ix;
                }
            }
        }

        IBlendedAnimation GetBlended(int ix)
        {
            unchecked
            {
                return (ix < 0) ? null : (ix >= blended_.Length) ? null : blended_[ix];
            }
        }

        public override void Update(ref GameTime gameTime)
        {
            if (curAnimationInstance_ > 0)
                instances_[curAnimationInstance_].Time = instances_[OldAnimationInstance_].Time;
                
            blender_.TransitionAnimations(GetBlended(OldAnimationInstance_), GetBlended(curAnimationInstance_),
                                              1.0f);

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (dt > 0.1f) dt = 0.1f;

            if (blender_ != null)
                blender_.Advance(dt);

            base.Update(ref gameTime);
        }

        public override void Draw(ref GameTime gameTime, ref  XCamera Camera)
        {
            //  if I have a model, set up the scene drawing parameters and draw the model
            if (loadedModel_ != null)
            {
                Matrix World = PhysicsObject.GetWorldMatrix(model.Model, Vector3.Zero); //modeloffset);

                //  draw the loaded model (the only model I have)
                //loadedModel_.Draw(ref Camera, World, blender_);
                loadedModel_.SceneDraw(ref Camera, World);

                //TODO: fix transparent rendering
                //Any shaders with the transparent annotation will draw here
                //should be done at the very end of scene
                loadedModel_.SceneDrawTransparent(ref Camera, World);
            }
        }
    }
}