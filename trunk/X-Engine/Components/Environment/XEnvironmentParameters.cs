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

        public Vector4 LightDirection { get; set; }
        public Vector4 LightColor { get; set; }
        public Vector4 LightColorAmbient { get; set; }
        public Vector4 FogColor { get; set; }
        public float FogDensity { get; set; }
        public float SunLightness { get; set; }
        public float SunRadiusAttenuation { get; set; }
        public float LargeSunLightness { get; set; }
        public float LargeSunRadiusAttenuation { get; set; }
        public float DayToSunsetSharpness { get; set; }
        public float HazeTopAltitude { get; set; }

        public Texture2D Night;
        public Texture2D Day;
        public Texture2D Sunset;

        public string NightFile { get; set; }
        public string DayFile { get; set; }
        public string SunsetFile { get; set; }

        public override void Load(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            Day = Content.Load<Texture2D>(DayFile);
            Night = Content.Load<Texture2D>(NightFile);
            Sunset = Content.Load<Texture2D>(SunsetFile);

            base.Load(Content);
        }
    }
}
