using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using XSIXNARuntime;
using System;

namespace XEngine
{
    public class XAnimatedActor : XActor, XUpdateable
    {
        public int AnimationIndex = 0;
        public int OldAnimationIndex = 0;
        XSIAnimationData l_Animations;
        public List<XSIAnimationContent> Animations;
        public bool Playing = true;
        private float Blend = 1.0f;


        public XAnimatedActor(XMain X, PhysicsObject Object, XModel model, Vector3 ModelScale, Vector3 ModelOffset, 
            Vector3 Velocity, float Mass) :
                base(X, Object, model, ModelScale, ModelOffset, Velocity, Mass)
        {
            model.ParentActor = this;
        }

        public override void Load(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            base.Load(Content);
            
            Animations = new List<XSIAnimationContent>();
            // post process animation
            l_Animations = model.Model.Tag as XSIAnimationData;

            if (l_Animations != null)
            {
                foreach (KeyValuePair<String, XSIAnimationContent> AnimationClip in l_Animations.RuntimeAnimationContentDictionary)
                {
                    AnimationClip.Value.BindModelBones(model.Model);
                    Animations.Add(AnimationClip.Value);
                }

                l_Animations.ResolveBones(model.Model);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if ((Animations != null) && (Animations.Count > 0))
            {
                if (Playing)
                {
                    if (Blend < 1.0f)
                    {
                        Animations[OldAnimationIndex].PlayBack(TimeSpan.Parse("0"), 1.0f);
                        Blend += 0.1f;

                        if (Blend > 1.0f)
                            Blend = 1.0f;
                    }
                    else
                    {
                        OldAnimationIndex = AnimationIndex;
                    }
                    Animations[AnimationIndex].PlayBack(gameTime.ElapsedGameTime, Blend);
                }
                else
                {
                    Animations[AnimationIndex].PlayBack(TimeSpan.Parse("00:00:00"), 1.0f);
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, XCamera Camera)
        {
            base.Draw(gameTime, Camera);
        }
    }
}