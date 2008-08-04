using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace XEngine
{
    public class XRenderer : XComponent, XDrawable
    {
        public List<XActor> ActorsInView = new List<XActor>();

        public Color ClearColor = Color.CornflowerBlue;

        //Effect shadowalphaEffect;
        //Texture2D shadowalphaTex;

        public XRenderer(XMain X) : base(X) 
        {
            //load shadow resources
            //shadowalphaEffect = X.Content.Load<Effect>("Content/XEngine/Effects/shadowalphaQuad");
            //shadowalphaTex = X.Content.Load<Texture2D>("Content/XEngine/Textures/shadowalphaTex");
        }

        public override void Draw(GameTime gameTime, XCamera Camera, XEnvironmentParameters environment)
        {
            X.GraphicsDevice.Clear(ClearColor);
            X.spriteBatch.Begin();
            DrawScene(gameTime, Camera, null, environment);
            X.spriteBatch.End();
        }

        public void DrawScene(GameTime gameTime, XCamera Camera, List<XComponent>NoDraw, XEnvironmentParameters environment)
        {
            X.GraphicsDevice.RenderState.DepthBufferEnable = true;
            X.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            X.GraphicsDevice.RenderState.AlphaBlendEnable = false;

            List<XActor> Alpha = new List<XActor>();

            ActorsInView.Clear();
            
            //sort X.Components list according to Draworder
            X.Components.Sort();

            foreach (XComponent component in X.Components)
            {
                //is this XComponent Drawable? and not the renderer?
                if(!(component is XDrawable) || (component is XRenderer))
                    continue;

                //Is this XComponent on the NoDraw list?
                if (NoDraw != null)
                    if (NoDraw.Contains(component))
                        continue;

                    if (component is XActor)
                    {
                        if (Camera.Frustrum.Contains(((XActor)component).boundingBox) != ContainmentType.Disjoint || component.NoCull)
                        {
                            ActorsInView.Add(((XActor)component));

                            //Add these xactors to another list to be draw at the end since they are alphablendable
                            if (((XActor)component).Material.AlphaBlendable)
                                Alpha.Add(((XActor)component));
                            else //draw these xactors now
                                component.Draw(gameTime, Camera, environment);
                        }
                    }
                    else if (component is XHeightMap)
                    {
                        if (Camera.Frustrum.Contains(((XHeightMap)component).boundingBox) != ContainmentType.Disjoint || component.NoCull)
                            component.Draw(gameTime, Camera, environment);
                    }
                    else if (component is XWater)
                    {
                        if (Camera.Frustrum.Contains(((XWater)component).boundingBox) != ContainmentType.Disjoint || component.NoCull)
                            component.Draw(gameTime, Camera, environment);
                    }
                    else
                    {

                        component.Draw(gameTime, Camera, environment);
                    }

                foreach (XActor actor in Alpha)
                    actor.Draw(gameTime, Camera, environment);
            }
        }

        public virtual void DrawModel(XModel Model, XCamera Camera, Matrix[] World, XMaterial material, XEnvironmentParameters environment)
        {
            foreach (ModelMesh mesh in Model.Model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.Parameters["World"].SetValue(World[0]);
                    //effect.Parameters["Bones"].SetValue(bones);

                    //if this throws and exception DID YOU FORGET TO SET THE MODEL PROCESSOR???????
                    //TO OUR CUSTOM ONE?? X-Model????
                    effect.CurrentTechnique = effect.Techniques["Model"];
                    
                    effect.Parameters["View"].SetValue(Camera.View);
                    effect.Parameters["Projection"].SetValue(Camera.Projection);
                    
                    effect.Parameters["vecEye"].SetValue(new Vector4(Camera.Position, 1));

                    material.SetupEffect(effect);

                    SetupLighting(effect, material, environment);
                }
                mesh.Draw();
            }
        }

        public void SetupLighting(Effect effect, XMaterial Material,XEnvironmentParameters environment)
        {
            /*Vector4[] LightDir = {
                        new Vector4(-0.526f, 0.573f, -0.627f, 1),
                        new Vector4(0.719f, 0.342f, 0.604f, 1),
                        new Vector4(0.454f, 0.766f, 0.454f, 1)
                    };

            Vector4[] LightColor = {
                        new Vector4(.8f, .8f, .8f, 10000000f),
                        new Vector4(.8f, .8f, .8f, Material.Specularity),
                        new Vector4(.8f, .8f, .8f, 10000000f)
                    };
            */
            Vector4[] LightDir = { -environment.LightDirection, new Vector4(0.719f, 0.342f, 0.604f, .5f) };

            Vector4[] LightColor = {environment.LightColor,environment.LightColorAmbient};

            effect.Parameters["vecLightDir"].SetValue(LightDir);
            effect.Parameters["LightColor"].SetValue(LightColor);
            effect.Parameters["NumLights"].SetValue(LightDir.Length);
        }
    }
}
