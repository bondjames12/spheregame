using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace XEngine
{
    public class XRenderer : XComponent, XDrawable
    {
        //public List<XActor> ActorsInView = new List<XActor>();

        public Color ClearColor = Color.CornflowerBlue;
        //This No Draw list is used to tell what Drawable components are for debug
        //it is used to skip drawing these in render passes other then the final pass the user sees (aka depthmap, shadow mapping, etc)
        public List<XComponent> DebugNoDraw;
        //This is another no draw list but this time is excludes depending on class type. All classes with type in this list will not be drawn!
        public List<Type> DebugNoDrawTypes;

        public XRenderer(XMain X) : base(X) 
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

        }

        public override void Draw(ref GameTime gameTime, ref  XCamera Camera)
        {
            //X.DepthMap.StartRenderToDepthMap();
            //Camera.RenderType = RenderTypes.Depth;
            //DrawScene(ref gameTime,ref Camera, DebugNoDraw, DebugNoDrawTypes);
            //X.DepthMap.EndRenderToDepthMap();

            Camera.RenderType = RenderTypes.Normal;
            X.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, ClearColor, 1.0f, 0);
            DrawScene(ref gameTime,ref Camera, null , null);

            //using (SpriteBatch sprite = new SpriteBatch(X.GraphicsDevice))
            //{
            //    sprite.Begin(SpriteBlendMode.None, SpriteSortMode.Texture, SaveStateMode.SaveState);
            //    sprite.Draw(X.DepthMap.depthMap, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 0.4f, SpriteEffects.None, 1);
            //    sprite.End();
            //}

        }

        public void DrawScene(ref GameTime gameTime,ref XCamera Camera, List<XComponent> NoDrawComponents, List<Type> NoDrawTypes)
        {
            X.GraphicsDevice.RenderState.DepthBufferEnable = true;
            X.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            X.GraphicsDevice.RenderState.AlphaBlendEnable = false;

            List<XActor> Alpha = new List<XActor>();

            //ActorsInView.Clear();

            //Begin 2D Sprite Batch
            X.spriteBatch.Begin();

            if (DebugMode)
            {
                //draw axis for debug info, so I can get an idea of where i am in the game
                X.DebugDrawer.DrawLine(new Vector3(-1000, 0, 0), new Vector3(1000, 0, 0), Color.Red);
                X.DebugDrawer.DrawLine(new Vector3(0, -1000, 0), new Vector3(0, 1000, 0), Color.Green);
                X.DebugDrawer.DrawLine(new Vector3(0, 0, -1000), new Vector3(0, 0, 1000), Color.Blue);
            }

            foreach (XComponent component in X.Components)
            {
                //is this XComponent Drawable? and not the renderer?
                if(!(component is XDrawable) || (component is XRenderer))
                    continue;

                //Is this XComponent on the NoDraw types list??
                if (NoDrawTypes != null)
                    if (NoDrawTypes.Contains(component.GetType()))
                        continue;

                //Is this XComponent on the NoDraw list?
                if (NoDrawComponents != null)
                    if (NoDrawComponents.Contains(component))
                        continue;

                if (component is XActor)
                {
                    //Does XActor, XHeightMap,XWater culling, add other types as create them
                    //Only enter this if the XActor is within the view or its NoCull is set
                    //if (Camera.Frustrum.Contains(((XActor)component).boundingBox) != ContainmentType.Disjoint || component.NoCull)
                    //{
                        //ActorsInView.Add(((XActor)component));

                        //Add these xactors to another list to be draw at the end since they are alphablendable
                        if (((XActor)component).AlphaBlendable)
                            Alpha.Add(((XActor)component));
                        else //draw this xactor now
                            component.Draw(ref gameTime, ref  Camera);
                    //}
                }
                else if (component is XHeightMap)
                {
                    //if (Camera.Frustrum.Contains(((XHeightMap)component).boundingBox) != ContainmentType.Disjoint || component.NoCull)
                        component.Draw(ref gameTime, ref  Camera);
                }
                else if (component is XWater)
                {
                    //if (Camera.Frustrum.Contains(((XWater)component).boundingBox) != ContainmentType.Disjoint || component.NoCull)
                        component.Draw(ref gameTime, ref  Camera);
                }
                else
                {
                    //Frustum testing????? Dynamicsky console debug debug drawer, menu manager
                    component.Draw(ref gameTime, ref  Camera);
                }
            }//end xcomponent foreach loop

            //This list of XActors are in view and have Alphablending turned on, RENDER THEM LAST SO THEY BLEND WITH EVERYTHING!
            foreach (XActor actor in Alpha)
                actor.Draw(ref gameTime, ref  Camera);

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
            foreach (ModelMesh mesh in Model.Model.Meshes)
            {
                Model.SASData.ComputeModel();

                foreach (Effect effect in mesh.Effects)
                {
                    /*if (effect.GetType() == typeof(BasicEffect))
                    {
                        //if rendering a depthmap
                        if (Camera.RenderType == RenderTypes.Depth)
                        {
                            System.Diagnostics.Debug.WriteLine("Basiceffect object! Please give a shader:" + mesh.Name);
                            return; //we can't render meshes using basiceffect into our depth MAP!!!!!
                        }
                        BasicEffect basiceffect = (BasicEffect)effect;
                        basiceffect.EnableDefaultLighting();
                        basiceffect.PreferPerPixelLighting = true;
                        basiceffect.Alpha = 0.5f;
                        basiceffect.View = Model.SASData.View;
                        basiceffect.Projection = Model.SASData.Projection;
                        basiceffect.World = Model.SASData.Model;
                        continue;
                    }
                    else
                    {*/
                        // bind SAS shader parameters
                        foreach (EffectParameter Parameter in effect.Parameters)
                        {
                            Model.SASData.SetEffectParameterValue(Parameter);
                        }

                        //if rendering a depthmap
                        /*if (Camera.RenderType == RenderTypes.Depth)
                        {
                            //override any techniques with DepthMap technique shader
                            if (effect.Techniques["DepthMapStatic"] != null)
                                effect.CurrentTechnique = effect.Techniques["DepthMapStatic"];
                            else
                            {//if we get there there is no DepthMap shader so we can't render this into our depth MAP!!
                                break;
                            }
                            continue;
                        }*/

                        if (effect.Techniques["Static"] != null)
                            effect.CurrentTechnique = effect.Techniques["Static"];
                        else
                            effect.CurrentTechnique = effect.Techniques[0];
                        
                    //}
                }
                mesh.Draw();
            }
        }
    }
}
