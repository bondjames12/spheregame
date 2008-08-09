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
        private float Blending = 1.0f;


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
                    if (Blending < 1.0f)
                    {
                        Animations[OldAnimationIndex].PlayBack(TimeSpan.Parse("0"), 1.0f);
                        Blending += 0.1f;

                        if (Blending > 1.0f)
                            Blending = 1.0f;
                    }
                    else
                    {
                        OldAnimationIndex = AnimationIndex;
                    }
                    Animations[AnimationIndex].PlayBack(gameTime.ElapsedGameTime, Blending);
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
            if (model != null && model.Loaded)
            {
            Matrix World = PhysicsBody.GetWorldMatrix(model.Model, modeloffset);

            Matrix[] transforms = new Matrix[model.Model.Bones.Count];
            model.Model.CopyAbsoluteBoneTransformsTo(transforms);

            
                if (AlphaBlendable)
                {
                    X.GraphicsDevice.RenderState.AlphaBlendEnable = true;
                    X.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha; // source rgb * source alpha
                    X.GraphicsDevice.RenderState.AlphaSourceBlend = Blend.One; // don't modify source alpha
                    X.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha; // dest rgb * (255 - source alpha)
                    X.GraphicsDevice.RenderState.AlphaDestinationBlend = Blend.InverseSourceAlpha; // dest alpha * (255 - source alpha)
                    X.GraphicsDevice.RenderState.BlendFunction = BlendFunction.Add; // add source and dest results
                }
                else
                    X.GraphicsDevice.RenderState.AlphaBlendEnable = false;

                //X.GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;

                //Set camera params, compute matrices
                model.SASData.Camera.NearFarClipping.X = Camera.NearPlane;
                model.SASData.Camera.NearFarClipping.Y = Camera.FarPlane;
                model.SASData.Camera.Position.X = Camera.Position.X;
                model.SASData.Camera.Position.Y = Camera.Position.Y;
                model.SASData.Camera.Position.Z = Camera.Position.Z;
                model.SASData.Projection = Camera.Projection;
                model.SASData.View = Camera.View;
    //apply transforms to this model in this order
      //transforms[mesh.ParentBone.Index] *
      //Matrix.CreateFromYawPitchRoll(myObject.Rotation.Yaw, myObject.Rotation.Pitch, myObject.Rotation.Roll) *
      //    Matrix.CreateScale(myObject.Scaling) * Matrix.CreateTranslation(myObject.Position));
                //model.SASData.Model = World;
                model.SASData.ComputeViewAndProjection();
                model.SASData.ComputeModel();

            if (ShowBoundingBox)
                X.DebugDrawer.DrawCube(boundingBox.Min, boundingBox.Max, Color.White, Matrix.Identity, Camera);

            //process animation
            XSIAnimationData l_Animations = model.Model.Tag as XSIAnimationData;
            bool isSkinned = false;
            Matrix[] bones = null;
            if (l_Animations != null)
            {
                l_Animations.ComputeBoneTransforms(transforms);
                bones = l_Animations.BoneTransforms;
                if (bones != null)
                {
                    if (bones.Length > 0)
                        isSkinned = true;
                }
            }

            foreach (ModelMesh mesh in model.Model.Meshes)
            {
                //model.SASData.ComputeModel();

                foreach (Effect effect in mesh.Effects)
                {
                    if (effect.GetType() == typeof(BasicEffect))
                    {
                        BasicEffect basiceffect = (BasicEffect)effect;
                        basiceffect.EnableDefaultLighting();
                        basiceffect.PreferPerPixelLighting = true;
                        basiceffect.Alpha = 0.5f;

                        basiceffect.View = model.SASData.View;
                        basiceffect.Projection = model.SASData.Projection;
                        //apply world matrix after mult by parent mesh(bone) transform
                        //the order of the multiplication is important!
                        basiceffect.World = model.SASData.Model * mesh.ParentBone.Transform * World;
                        mesh.Draw();
                    }
                    else
                    {
                        // set the shader technique to skinned or Static or the first one, try in that order
                        if (isSkinned && (effect.Techniques["Skinned"] != null))
                        {
                            effect.CurrentTechnique = effect.Techniques["Skinned"];
                        }
                        else
                        {
                            if (effect.Techniques["Static"] != null)
                                effect.CurrentTechnique = effect.Techniques["Static"];
                            else
                                effect.CurrentTechnique = effect.Techniques[0];
                        }

                        // bind bones to shader
                        if (isSkinned)
                        {
                            if ((effect.Parameters["Bones"] != null) && isSkinned)
                                effect.Parameters["Bones"].SetValue(bones);
                        }

                        // bind all other parameters
                        foreach (EffectParameter Parameter in effect.Parameters)
                        {
                            model.SASData.SetEffectParameterValue(Parameter);
                        }
                    }

                    /*
                    effect.Parameters["World"].SetValue(World[0]);
                    //effect.Parameters["Bones"].SetValue(bones);

                    //if this throws and exception DID YOU FORGET TO SET THE MODEL PROCESSOR???????
                    //TO OUR CUSTOM ONE?? X-Model????
                    effect.CurrentTechnique = effect.Techniques["Model"];
                    
                    effect.Parameters["View"].SetValue(Camera.View);
                    effect.Parameters["Projection"].SetValue(Camera.Projection);
                    
                    effect.Parameters["vecEye"].SetValue(new Vector4(Camera.Position, 1));

                    material.SetupEffect(effect);

                    Vector4[] LightDir = { -X.Environment.LightDirection, new Vector4(0.719f, 0.342f, 0.604f, .5f) };

                    Vector4[] LightColor = { X.Environment.LightColor, X.Environment.LightColorAmbient };

                    effect.Parameters["vecLightDir"].SetValue(LightDir);
                    effect.Parameters["LightColor"].SetValue(LightColor);
                    effect.Parameters["NumLights"].SetValue(LightDir.Length);
                     */
                }
                mesh.Draw();
            }
        }
    }


    }
}