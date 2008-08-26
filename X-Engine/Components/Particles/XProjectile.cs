#region File Description
//-----------------------------------------------------------------------------
// Projectile.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
#endregion

namespace XEngine
{
    /// <summary>
    /// This class demonstrates how to combine several different particle systems
    /// to build up a more sophisticated composite effect. It implements a rocket
    /// projectile, which arcs up into the sky using a ParticleEmitter to leave a
    /// steady stream of trail particles behind it. After a while it explodes,
    /// creating a sudden burst of explosion and smoke particles.
    /// </summary>
    public class XProjectile : XComponent, XUpdateable
    {
        #region Constants

        const float trailParticlesPerSecond = 200;
        const int numExplosionParticles = 30;
        const int numExplosionSmokeParticles = 50;
        const float projectileLifespan = 1.5f;
        const float sidewaysVelocityRange = 60;
        const float verticalVelocityRange = 40;
        const float gravity = 15;

        #endregion

        #region Fields

        XParticleSystem explosionParticles;
        XParticleSystem explosionSmokeParticles;
        XParticleEmitter trailEmitter;

        Vector3 position;
        Vector3 velocity;
        float age;
        //if this gets to be true we should stop rendering this and delete it!
        bool dead = false;

        static Random random = new Random();

        #endregion


        /// <summary>
        /// Constructs a new projectile.
        /// </summary>
        public XProjectile(ref XMain X, XParticleSystem explosionParticles,
                          XParticleSystem explosionSmokeParticles,
                          XParticleSystem projectileTrailParticles) : base(ref X)
        {
            this.explosionParticles = explosionParticles;
            this.explosionSmokeParticles = explosionSmokeParticles;

            // Start at the origin, firing in a random (but roughly upward) direction.
            position = Vector3.Zero;

            velocity.X = (float)(random.NextDouble() - 0.5) * sidewaysVelocityRange;
            velocity.Y = (float)(random.NextDouble() + 0.5) * verticalVelocityRange;
            velocity.Z = (float)(random.NextDouble() - 0.5) * sidewaysVelocityRange;

            // Use the particle emitter helper to output our trail particles.
            trailEmitter = new XParticleEmitter(ref X,projectileTrailParticles,
                                               trailParticlesPerSecond, position);
        }

        /// <summary>
        /// Updates the projectile.
        /// </summary>
        public override void Update(ref GameTime gameTime)
        {
            //if dead then disable all XComponents related to this projectile
            //but we can't do it right away because the explosion is not finished yet!
            if (dead)
            {
                //set flag trailemitter that its dead (it will disable itself and its particle system automatically)
                trailEmitter.dead = true;
                //check if our explosion and explosion smoke particle systems are dead if so disable them and then disable ourself!
                if (explosionParticles.dead && explosionSmokeParticles.dead)
                {
                    explosionParticles.Disable();
                    explosionSmokeParticles.Disable();
                    this.Disable();
                }
                return;
            }

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Simple projectile physics.
            position += velocity * elapsedTime;
            velocity.Y -= elapsedTime * gravity;
            age += elapsedTime;

            // Update the particle emitter, which will create our particle trail.
            trailEmitter.newPosition = position;
            //trailEmitter.Update(gameTime, position); //changed XRenderer will call update on this class automatically

            // If enough time has passed, explode! Note how we pass our velocity
            // in to the AddParticle method: this lets the explosion be influenced
            // by the speed and direction of the projectile which created it.
            if (age > projectileLifespan)
            {
                for (int i = 0; i < numExplosionParticles; i++)
                    explosionParticles.AddParticle(position, velocity);

                for (int i = 0; i < numExplosionSmokeParticles; i++)
                    explosionSmokeParticles.AddParticle(position, velocity);
                //flag as dead
                dead = true;                
            }
        }
    }
}
