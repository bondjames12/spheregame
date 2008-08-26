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

        public Fire(ref XMain X)
        {
            //smokeSYS = new XParticleSystem(X, "Content\\Particles\\ExplosionSmokeSettings");
            //fireSYS = new XParticleSystem(X, "Content\\Particles\\ExplosionSettings");

            //smoke = new XParticleEmitter(X, smokeSYS,4);
            //fire = new XParticleEmitter(X, fireSYS, 1);

            //content = new XResourceGroup(X);
            //content.AddComponent(smokeSYS);
            //content.AddComponent(fireSYS);
            //content.Load();
        }

        public void Update(GameTime gameTime)
        {
            //smoke.Update(ref gameTime);
            //fire.Update(ref gameTime);
        }
    }
}