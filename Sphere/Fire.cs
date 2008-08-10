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
            smokeSYS = new XParticleSystem(X, "Content\\Particles\\FireSettings");
            fireSYS = new XParticleSystem(X, "Content\\Particles\\SmokePlumeSettings");

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