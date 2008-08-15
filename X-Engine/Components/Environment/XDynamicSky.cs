using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace XEngine
{
    public class XDynamicSky : XComponent, XUpdateable, XDrawable
    {
        private Model domeModel;
        public Effect domeEffect;

        public float Theta;
        public float Phi;

        public bool RealTime;

        public int environmentalParametersNumber;
        XEnvironmentParameters parameters;
        public XEnvironmentParameters Params
        {
            get { return parameters; }
            set { parameters = value; environmentalParametersNumber = value.number; }
        }

        public XDynamicSky(XMain X, XEnvironmentParameters Parameters) : base(X)
        {
            if (Parameters != null)
                this.Params = Parameters;

            RealTime = false;
            Theta = 4.0f;
            Phi = 0.0f;
        }

        public override void Load(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            this.domeModel = X.Content.Load<Model>(@"Content\XEngine\Models\SkyDome");
            domeEffect = X.Content.Load<Effect>(@"Content\XEngine\Effects\Sky");

            X.Tools.RemapModel(domeModel, domeEffect);

            foreach (ModelMesh mesh in domeModel.Meshes)
            {
                Matrix[] boneTransforms = new Matrix[domeModel.Bones.Count];
                domeModel.CopyAbsoluteBoneTransformsTo(boneTransforms);

                foreach (Effect effect in mesh.Effects)
                {
                    effect.Parameters["SkyTextureNight"].SetValue(Params.Night);
                    effect.Parameters["SkyTextureSunset"].SetValue(Params.Sunset);
                    effect.Parameters["SkyTextureDay"].SetValue(Params.Day);

                    effect.Parameters["isSkydome"].SetValue(true);

                    effect.Parameters["LightDirection"].SetValue(Params.LightDirection);
                    effect.Parameters["LightColor"].SetValue(Params.LightColor);
                    effect.Parameters["LightColorAmbient"].SetValue(Params.LightColorAmbient);
                    effect.Parameters["FogColor"].SetValue(Params.FogColor);
                    effect.Parameters["fDensity"].SetValue(Params.FogDensity);
                    effect.Parameters["SunLightness"].SetValue(Params.SunLightness);
                    effect.Parameters["sunRadiusAttenuation"].SetValue(Params.SunRadiusAttenuation);
                    effect.Parameters["largeSunLightness"].SetValue(Params.LargeSunLightness);
                    effect.Parameters["largeSunRadiusAttenuation"].SetValue(Params.LargeSunRadiusAttenuation);
                    effect.Parameters["dayToSunsetSharpness"].SetValue(Params.DayToSunsetSharpness);
                    effect.Parameters["hazeTopAltitude"].SetValue(Params.HazeTopAltitude);
                }
            }

            base.Load(Content);
        }

        public override void Update(ref GameTime gameTime)
        {
            if (loaded)
            {
                if (RealTime)
                {
                    int minutes = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
                    this.Theta = (float)minutes * (float)(Math.PI) / 12.0f / 60.0f;
                }

                Params.LightDirection = this.GetDirection();
                Params.LightDirection.Normalize();
            }
        }

        public override void Draw(ref GameTime gameTime, ref  XCamera camera)
        {
            if (loaded)
            {
                Matrix[] boneTransforms = new Matrix[domeModel.Bones.Count];
                domeModel.CopyAbsoluteBoneTransformsTo(boneTransforms);

                foreach (ModelMesh mesh in domeModel.Meshes)
                {
                    Matrix World = Matrix.CreateScale(10) * boneTransforms[mesh.ParentBone.Index] *
                        Matrix.CreateTranslation(camera.Position.X, camera.Position.Y, camera.Position.Z);

                    Matrix WorldIT = Matrix.Invert(World);
                    WorldIT = Matrix.Transpose(WorldIT);

                    foreach (Effect effect in mesh.Effects)
                    {
                        effect.Parameters["WorldIT"].SetValue(WorldIT);
                        effect.Parameters["World"].SetValue(World);
                        effect.Parameters["WorldViewProj"].SetValue(World * camera.View * camera.Projection);
                        effect.Parameters["ViewInv"].SetValue(Matrix.Invert(camera.View));
                        effect.Parameters["LightDirection"].SetValue(Params.LightDirection);
                    }
                    mesh.Draw();
                }
            }
        }

        public Vector4 GetDirection()
        {

            float y = (float)Math.Cos((double)this.Theta);
            float x = (float)(Math.Sin((double)this.Theta) * Math.Cos(this.Phi));
            float z = (float)(Math.Sin((double)this.Theta) * Math.Sin(this.Phi));
            float w = 1.0f;

            return new Vector4(x, y, z, w);
        }
    }
}