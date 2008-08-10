using XEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
namespace Sphere
{
    public class Fire
    {
        XParticleEmitter smoke;
        XParticleEmitter fire;

        XParticleSystem smokeSYS;
        XParticleSystem fireSYS;

        XResourceGroup content;

        public Fire(XMain X)
        {
            smokeSYS = new XParticleSystem(X, "Content\\Particles\\FireSettings");
            fireSYS = new XParticleSystem(X, "Content\\Particles\\SmokePlumeSettings");

            smoke = new XParticleEmitter(X, smokeSYS, 150);
            fire = new XParticleEmitter(X, fireSYS, 150);

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
}