using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using XSIXNARuntime;
using System;

namespace XEngine
{
    public class XAnimatedActor : XActor, XUpdateable
    {
        private int _AnimationIndex;
        private int _OldAnimationIndex;
        XSIAnimationData l_Animations;
        private List<XSIAnimationContent> Animations;
        public bool Playing = true;
        private float _Blending;

        /// <summary>
        /// Change the index to change which animation is playing
        /// </summary>
        public int AnimationIndex 
        { 
            get 
            {
                return _AnimationIndex;
            }
            set
            {
                if (Animations.Count > 0)
                {
                    _AnimationIndex = value;
                    if (_AnimationIndex < 0)
                        _AnimationIndex = Animations.Count - 1;
                    if (_AnimationIndex >= Animations.Count)
                        _AnimationIndex = 0;
                    _Blending = 0.0f;
                }
            }
        }

        public XAnimatedActor(ref XMain X, XPhysicsObject Object, XModel model, Vector3 ModelScale,
                                    Vector3 ModelOffset, Vector3 Velocity, float Mass) :
                base(ref X, Object, model,ModelScale, ModelOffset, Velocity, Mass)
        {
            model.Parent = this;
            _AnimationIndex = 0;
            _OldAnimationIndex = 0;
            _Blending = 1.0f;
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

        public override void Update(ref GameTime gameTime)
        {
            if ((Animations != null) && (Animations.Count > 0))
            {
                if (Playing)
                {
                    if (_Blending < 1.0f)
                    {
                        Animations[_OldAnimationIndex].PlayBack(TimeSpan.Parse("0"), 1.0f);
                        _Blending += 0.1f;

                        if (_Blending > 1.0f)
                            _Blending = 1.0f;
                    }
                    else
                    {
                        _OldAnimationIndex = _AnimationIndex;
                    }
                    Animations[_AnimationIndex].PlayBack(gameTime.ElapsedGameTime, _Blending);
                }
                else
                {
                    Animations[_AnimationIndex].PlayBack(TimeSpan.Parse("00:00:00"), 1.0f);
                }
            }

            base.Update(ref gameTime);
        }

        public override void Draw(ref GameTime gameTime, ref  XCamera Camera)
        {
            if (model != null && model.loaded)
            {
            Matrix World = PhysicsObject.GetWorldMatrix(model.Model, Vector3.Zero); //modeloffset);

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

                X.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;

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


                //Lighting?????
                //update lighting information for shaders, apply global lighting environment params
                model.SASData.AmbientLights.Clear();
                model.SASData.PointLights.Clear();
                model.SASData.AmbientLights.Add(new XSISASAmbientLight(X.Environment.LightColorAmbient));
                model.SASData.PointLights.Add(new XSISASPointLight(X.Environment.LightColor, -X.Environment.LightDirection*100, 10000));

                //model.InitDefaultSASLighting();


            if (DebugMode)
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
                //apply world matrix after mult by parent mesh(bone) transform
                //the order of the multiplication is important!
                //Relative to world origin, move to position on mesh(mesh.ParentBone.Transform) then to position on world(World)!
                model.SASData.Model = mesh.ParentBone.Transform * World;
                model.SASData.ComputeModel();
                
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
                        basiceffect.World = model.SASData.Model;
                        mesh.Draw();
                    }
                    else
                    {

                        // bind SAS shader parameters
                        foreach (EffectParameter Parameter in effect.Parameters)
                            model.SASData.SetEffectParameterValue(Parameter);
                        
                        // bind bones to shader
                        if (isSkinned)
                        {
                            if ((effect.Parameters["Bones"] != null) && isSkinned)
                                effect.Parameters["Bones"].SetValue(bones);
                        }

                        //if rendering a depthmap
                        if (Camera.RenderType == RenderTypes.Depth)
                        {
                            //override any techniques with DepthMap technique shader
                            if (effect.Techniques["DepthMapSkinned"] != null)
                                effect.CurrentTechnique = effect.Techniques["DepthMapSkinned"];
                            continue;
                        }

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
                    }
                }
                mesh.Draw();
            }
        }
    }


    }
}