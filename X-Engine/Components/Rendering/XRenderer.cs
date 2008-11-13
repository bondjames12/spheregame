using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace XEngine
{
    public class XRenderer : XComponent, XIDrawable
    {
        //public List<XActor> ActorsInView = new List<XActor>();

        public Color ClearColor = Color.CornflowerBlue;
        //This No Draw list is used to tell what Drawable components are for debug
        //it is used to skip drawing these in render passes other then the final pass the user sees (aka depthmap, shadow mapping, etc)
        public List<XComponent> DebugNoDraw;
        //This is another no draw list but this time is excludes depending on class type. All classes with type in this list will not be drawn!
        public List<Type> DebugNoDrawTypes;

        public XRenderer(XMain X) : base(ref X) 
        {
            DebugNoDraw = new List<XComponent>();
            DebugNoDraw.Add(X.Debug);
            DebugNoDraw.Add(X.DebugDrawer);
            DebugNoDraw.Add(X.Console);
            DebugNoDraw.Add(X.FrameRate);
            //The menumanager is also added to this list elsewhere in the code, no access from here

            //Add particle systems to the DebugNoDrawTypes list
            DebugNoDrawTypes = new List<Type>();
            DebugNoDrawTypes.Add(typeof(XParticleSystem));
            DebugNoDrawTypes.Add(typeof(XDynamicSky));

        }

        public override void Draw(ref GameTime gameTime, ref  XCamera Camera)
        {
            
            //X.DepthMap.StartRenderToDepthMap();
            //make a camera to look at the players camera, center and target!
            //this is for testing (very slow to remake every frame)
            //XCamera sunCam = new XCamera(X, 100f, 300f);
            //sunCam.Position = new Vector3(-X.Environment.LightDirection.X * 200f,-X.Environment.LightDirection.Y * 200f,-X.Environment.LightDirection.Z * 200f);
            //sunCam.Target = Vector3.Zero;
            //sunCam.Up = Vector3.Up;
            //sunCam.Update(ref gameTime);
            //Camera.ShadowView = sunCam.View;
            //Camera.ShadowProjection = sunCam.Projection;
            //sunCam.RenderType = RenderTypes.Depth;
            //Camera.RenderType = RenderTypes.Depth;
            //DrawScene(ref gameTime,ref Camera, DebugNoDraw, DebugNoDrawTypes);
            //sunCam.Disable();
            //X.DepthMap.EndRenderToDepthMap();

            //Camera.ShadowProjection = Camera.Projection;
            //Camera.ShadowView = Camera.View;
            Camera.RenderType = RenderTypes.Normal;
            Camera.DebugMode = true;
            X.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, ClearColor, 1.0f, 0);
            DrawScene(ref gameTime,ref Camera, null , null);

            
           //using (SpriteBatch sprite = new SpriteBatch(X.GraphicsDevice))
            //{
            //    sprite.Begin(SpriteBlendMode.None, SpriteSortMode.Texture, SaveStateMode.SaveState);
            //    sprite.Draw(X.DepthMap.depthMap, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 0.2f, SpriteEffects.None, 1);
            //    sprite.End();
            //}
             

        }

        public void DrawScene(ref GameTime gameTime,ref XCamera Camera, List<XComponent> NoDrawComponents, List<Type> NoDrawTypes)
        {
            X.GraphicsDevice.RenderState.DepthBufferEnable = true;
            X.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            X.GraphicsDevice.RenderState.AlphaBlendEnable = false;

            List<XComponent> Alpha = new List<XComponent>();

            //ActorsInView.Clear();

            //Begin 2D Sprite Batch
            X.spriteBatch.Begin();

            if (DebugMode)
            {
                //draw axis for debug info, so I can get an idea of where i am in the game
                X.DebugDrawer.DrawLine(new Vector3(-1000, 0, 0), new Vector3(1000, 0, 0), Color.Red);
                X.DebugDrawer.DrawLine(new Vector3(0, -1000, 0), new Vector3(0, 1000, 0), Color.Green);
                X.DebugDrawer.DrawLine(new Vector3(0, 0, -1000), new Vector3(0, 0, 1000), Color.Blue);

                /*X
                for (int x = -100; x < 100; x++)
                {
                    X.DebugDrawer.DrawLine(new Vector3(x, -1000, 0), new Vector3(x, 1000, 0), Color.Red);
                    for (int z = -100; z < 100; z++)
                    {
                        X.DebugDrawer.DrawLine(new Vector3(x, -1000, z), new Vector3(x, 1000, z), Color.Red);
                    }

                }

                //Y
                for (int y = -100; y < 100; y++)
                {
                    X.DebugDrawer.DrawLine(new Vector3(0, y, -1000), new Vector3(0, y, 1000), Color.Green);
                    for (int z = -100; z < 100; z++)
                    {
                        X.DebugDrawer.DrawLine(new Vector3(-1000, y, z), new Vector3(1000, y, z), Color.Blue);
                        X.DebugDrawer.DrawLine(new Vector3(z, y, -1000), new Vector3(z, y, 1000), Color.Green);
                    }
                }

                //Z
                for (int z = -100; z < 100; z++)
                {
                    X.DebugDrawer.DrawLine(new Vector3(-1000, 0, z), new Vector3(1000, 0, z), Color.Blue);
                }*/
            }

            for(int i=0;i<X.Components.Count;i++)
            {
                XComponent component = X.Components[i];
            
                //if debug rendering is on draw the XTriggerVolume(s)
                if(Camera.DebugMode)
                {
                    if(component is XTriggerVolume)
                    {
                        component.Draw(ref gameTime, ref Camera);
                    }
                }
                //is this XComponent Drawable? and not the renderer?
                if(!(component is XIDrawable) || (component is XRenderer))
                    continue;

                //Is this XComponent on the NoDraw types list??
                if (NoDrawTypes != null)
                    if (NoDrawTypes.Contains(component.GetType()))
                        continue;

                //Is this XComponent on the NoDraw list?
                if (NoDrawComponents != null)
                    if (NoDrawComponents.Contains(component))
                        continue;

                //Add these components to another list to be draw at the end since they are alphablendable
                //after adding it to the list continue to the next component in this loop!
                if (component.AlphaBlendable)
                {
                    Alpha.Add(component);
                    continue;
                }

                if (component is XActor)
                {
                    //Does XActor, XHeightMap,XWater culling, add other types as create them
                    //Only enter this if the XActor is within the view or its NoCull is set
                    //if (Camera.Frustrum.Contains(((XActor)component).boundingBox) != ContainmentType.Disjoint || component.NoCull)
                    if (component.NoCull || (component is XTree) || Camera.Frustrum.Contains(((XActor)component).PhysicsObject.PhysicsBody.CollisionSkin.WorldBoundingBox) != ContainmentType.Disjoint)
                    {
                        component.Draw(ref gameTime, ref  Camera);
                    }
                }
                else if (component is XHeightMap)
                {
                    if (Camera.Frustrum.Contains(((XHeightMap)component).boundingBox) != ContainmentType.Disjoint || component.NoCull)
                        component.Draw(ref gameTime, ref  Camera);
                }
                else if (component is XWater)
                {
                    if (Camera.Frustrum.Contains(((XWater)component).boundingBox) != ContainmentType.Disjoint || component.NoCull)
                        component.Draw(ref gameTime, ref  Camera);
                }
                else
                {
                    //Frustum testing????? Dynamicsky console debug debug drawer, menu manager
                    component.Draw(ref gameTime, ref  Camera);
                }
            }//end xcomponent foreach loop

            //This list of XActors are in view and have Alphablending turned on, RENDER THEM LAST SO THEY BLEND WITH EVERYTHING!
            for (int j = 0; j < Alpha.Count; j++)
                Alpha[j].Draw(ref gameTime, ref  Camera);

            //End Sprite Batch
            X.spriteBatch.End();
        }

        /// <summary>
        /// This method only support drawing static models. If you attempt to draw an animated (skinned) model it won't look right
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="Camera"></param>
        public virtual void DrawModel(ref XModel Model,ref XCamera Camera)
        {
            for (int i = 0; i < Model.Model.Meshes.Count; i++)
            {
                ModelMesh mesh = Model.Model.Meshes[i];

                //calc new boundingsphere with new position from World/Model matrix
                BoundingSphere bs = mesh.BoundingSphere.Transform(Matrix.CreateTranslation(Model.SASData.Model.Translation));
                
                //Is it in view?
                if (Camera.Frustrum.Contains(bs) == ContainmentType.Disjoint)
                    continue;

                //render boundingspheres for each mesh!
                if (DebugMode)
                {
                    BoundingVolumeRenderer.RenderBoundingSphere(bs, ref Camera.View, ref Camera.Projection);
                }

                Model.SASData.ComputeModel();

                for(int j = 0; j < mesh.Effects.Count; j++)
                {
                    Effect effect = mesh.Effects[j];
                    // bind SAS shader parameters
                    for(int k=0;k<effect.Parameters.Count;k++)
                    {
                        if (effect.Parameters[k].ParameterType == EffectParameterType.String)
                        {
                            if (effect.Parameters[k].Name.Contains("AnimationFileName"))
                            {
                                string animfilename = effect.Parameters[k].GetValueString();
                                if (!string.IsNullOrEmpty(animfilename))
                                {
                                    //there is an animated texture here
                                    foreach (EffectAnnotation anno in effect.Parameters[k].Annotations)
                                    {
                                        if (anno.Name.Contains("AnimatedMap"))
                                        {
                                            effect.Parameters[anno.GetValueString()].SetValue(
                                                Model.Giftextures[j].GetTexture());
                                        }
                                    }
                                }
                            }
                        }
                        Model.SASData.SetEffectParameterValue(effect.Parameters[k]);
                    }

                    //if rendering a depthmap
                    if (Camera.RenderType == RenderTypes.Depth)
                    {
                        //override any techniques with DepthMap technique shader
                        if (effect.Techniques["DepthMapStatic"] != null)
                            effect.CurrentTechnique = effect.Techniques["DepthMapStatic"];
                        else
                        {//if we get there there is no DepthMap shader so we can't render this into our depth MAP!!
                            break;
                        }
                        continue;
                    }

                    if (effect.Techniques["Static"] != null)
                        effect.CurrentTechnique = effect.Techniques["Static"];
                    else
                        effect.CurrentTechnique = effect.Techniques[0];
                }
                mesh.Draw();
            }
        }
    }
}
