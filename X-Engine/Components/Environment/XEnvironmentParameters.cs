using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XEngine
{
    public class XEnvironmentParameters : XComponent, XLoadable
    {
        public static int count = 0;
        public int number;

        public XEnvironmentParameters(XMain X) : base(X)
        {
            LightDirection = new Vector4(100.0f, -100.0f, 100.0f, 1.0f);
            LightColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            LightColorAmbient = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);
            FogColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            FogDensity = .0045f;
            SunLightness = 0.2f;
            SunRadiusAttenuation = 256.0f;
            LargeSunLightness = 0.2f;
            LargeSunRadiusAttenuation = 1.0f;
            DayToSunsetSharpness = 1.15f;
            HazeTopAltitude = 100.0f;

            SunsetFile = @"Content\XEngine\Textures\SkySunset";
            DayFile = @"Content\XEngine\Textures\SkyDay";
            NightFile = @"Content\XEngine\Textures\SkyNight";

            count++;
            number = count;
        }

        public Vector4 LightDirection;
        public Vector4 LightColor;
        public Vector4 LightColorAmbient;
        public Vector4 FogColor;
        public float FogDensity;
        public float SunLightness;
        public float SunRadiusAttenuation;
        public float LargeSunLightness;
        public float LargeSunRadiusAttenuation;
        public float DayToSunsetSharpness;
        public float HazeTopAltitude;

        public Texture2D Night;
        public Texture2D Day;
        public Texture2D Sunset;

        public string NightFile;
        public string DayFile;
        public string SunsetFile;

        public override void Load(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            Day = Content.Load<Texture2D>(DayFile);
            Night = Content.Load<Texture2D>(NightFile);
            Sunset = Content.Load<Texture2D>(SunsetFile);

            base.Load(Content);
        }
    }
}
