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
                //  the drawdetails set-up can be re-used for all items in the scene
                //DrawDetails dd = drawDetails_;
                //dd.dev = X.GraphicsDevice;
                //dd.fogColor = X.Environment.FogColor;
                //dd.fogDistance = X.Environment.FogDensity;
                //dd.lightAmbient = X.Environment.LightColorAmbient;
                //dd.lightDiffuse = X.Environment.LightColor;
                //dd.lightDir = X.Environment.LightDirection;

                //dd.viewInv = viewInv_;
                //dd.viewProj = view_ * projection_;
                //dd.world = Matrix.Identity;

                Matrix World = PhysicsObject.GetWorldMatrix(model.Model, Vector3.Zero); //modeloffset);

                //  draw the loaded model (the only model I have)
                loadedModel_.Draw(ref Camera, World, blender_);
            }
            //  when everything else is drawn, Z sort and draw the transparent parts
            ModelDraw.DrawDeferred();

            /*if (model != null && model.loaded)
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
                //model.SASData.AmbientLights.Clear();
                //model.SASData.PointLights.Clear();
                //model.SASData.AmbientLights.Add(new XSISASAmbientLight(X.Environment.LightColorAmbient));
                //model.SASData.PointLights.Add(new XSISASPointLight(X.Environment.LightColor, -X.Environment.LightDirection*100, 10000));

                model.InitDefaultSASLighting();


            if (DebugMode)
                X.DebugDrawer.DrawCube(PhysicsObject.PhysicsBody.CollisionSkin.WorldBoundingBox.Min, PhysicsObject.PhysicsBody.CollisionSkin.WorldBoundingBox.Max, Color.White, Matrix.Identity, Camera);

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
                for(int j = 0; j < mesh.Effects.Count; j++)
                {
                    Effect effect = mesh.Effects[j];
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
                        {
                            if (Parameter.ParameterType == EffectParameterType.String)
                            {
                                if (Parameter.Name.Contains("AnimationFileName"))
                                {
                                    string animfilename = Parameter.GetValueString();
                                    if (!string.IsNullOrEmpty(animfilename))
                                    {
                                        //there is an animated texture here
                                        foreach (EffectAnnotation anno in Parameter.Annotations)
                                        {
                                            if (anno.Name.Contains("AnimatedMap"))
                                            {
                                                effect.Parameters[anno.GetValueString()].SetValue(
                                                    model.Giftextures[j].GetTexture());
                                            }
                                        }
                                    }
                                }
                            }
                            model.SASData.SetEffectParameterValue(Parameter);
                        }
                            
                        
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

*/
        }
    }
}