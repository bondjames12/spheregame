using XEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
namespace Sphere
{
    public class Fire
    {
        SmokeEmitter smoke;
        FireEmitter fire;

        XParticleSystem smokeSYS;
        XParticleSystem fireSYS;

        XResourceGroup content;

        public Fire(XMain X)
        {
            smokeSYS = new XParticleSystem(X, SetupSmoke());
            fireSYS = new XParticleSystem(X, SetupFire());

            smokeSYS.DrawOrder = 50;
            fireSYS.DrawOrder = 40;

            smoke = new SmokeEmitter(X, smokeSYS, 150);
            fire = new FireEmitter(X, fireSYS, 150);

            content = new XResourceGroup(X);
            content.AddComponent(smokeSYS);
            content.AddComponent(fireSYS);
            content.Load();
        }

        public void Update(GameTime gameTime)
        {
            smoke.Update(gameTime);
            fire.Update(gameTime);
        }

        public XParticleSystemSettings SetupFire()
        {
            XParticleSystemSettings settings = new XParticleSystemSettings();

            settings.TextureName = @"Content\Textures\Fire";

            settings.MaxParticles = 375;

            settings.Duration = TimeSpan.FromSeconds(2.5f);

            settings.DurationRandomness = 1;

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 2;

            settings.MinVerticalVelocity = -2;
            settings.MaxVerticalVelocity = 2;

            // Set gravity upside down, so the flames will 'fall' upward.
            settings.Gravity = new Vector3(2, 8, 0);

            settings.MinColor = new Color(255, 255, 255, 100);
            settings.MaxColor = new Color(255, 255, 255, 150);

            settings.MinStartSize = 7;
            settings.MaxStartSize = 9;

            settings.MinEndSize = 10;
            settings.MaxEndSize = 12;

            // Use additive blending.
            settings.SourceBlend = Blend.SourceAlpha;
            settings.DestinationBlend = Blend.One;

            return settings;
        }

        public XParticleSystemSettings SetupSmoke()
        {
            XParticleSystemSettings settings = new XParticleSystemSettings();

            settings.TextureName = @"Content\Textures\Smoke";

            settings.MaxParticles = 525;

            settings.Duration = TimeSpan.FromSeconds(3.5f);

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 6;

            settings.MinVerticalVelocity = 4.5f;
            settings.MaxVerticalVelocity = 10;

            // Create a wind effect by tilting the gravity vector sideways.
            settings.Gravity = new Vector3(16, 8, 0);

            settings.EndVelocity = 0.75f;

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 9;
            settings.MaxStartSize = 11;

            settings.MinColor = new Color(155, 155, 155, 200);
            settings.MaxColor = new Color(155, 155, 155, 220);

            settings.MinEndSize = 20;
            settings.MaxEndSize = 22;

            return settings;
        }
    }

    public class FireEmitter : XParticleEmitter
    {
        public FireEmitter(XMain X, XParticleSystem particleSystem, float particlesPerSecond)
            : base(X, particleSystem, particlesPerSecond)
        {
        }

        public override void Emit()
        {
            Vector2 pos = X.Tools.GetRandomVector2(new Vector2(-31, -31), new Vector2(31, 31));

            particleSystem.AddParticle(new Vector3(pos.X, 1.5f, pos.Y), Vector3.Zero);
        }
    }

    public class SmokeEmitter : XParticleEmitter
    {
        public SmokeEmitter(XMain X, XParticleSystem particleSystem, float particlesPerSecond)
            : base(X, particleSystem, particlesPerSecond)
        {
        }

        public override void Emit()
        {
            Vector2 pos = X.Tools.GetRandomVector2(new Vector2(-31, -31), new Vector2(31, 31));

            particleSystem.AddParticle(new Vector3(pos.X, 1.5f, pos.Y), Vector3.Zero);
        }
    }
}