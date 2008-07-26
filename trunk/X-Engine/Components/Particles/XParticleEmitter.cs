#region Using Statements
using System;
using Microsoft.Xna.Framework;
#endregion

namespace XEngine
{
    public class XParticleEmitter : XComponent, XUpdateable
    {
        #region Fields

        public XParticleSystem particleSystem;
        public float timeBetweenParticles;
        float timeLeftOver;

        #endregion


        /// <summary>
        /// Constructs a new particle emitter object.
        /// </summary>
        public XParticleEmitter(XMain X, XParticleSystem system, float particlesPerSecond) : base(X)
        {
            this.particleSystem = system;

            timeBetweenParticles = 1.0f / particlesPerSecond;
        }


        /// <summary>
        /// Updates the emitter, creating the appropriate number of particles
        /// in the appropriate positions.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if (gameTime == null)
                throw new ArgumentNullException("gameTime");

            // Work out how much time has passed since the previous update.
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (elapsedTime > 0)
            {
                // If we had any time left over that we didn't use during the
                // previous update, add that to the current elapsed time.
                float timeToSpend = timeLeftOver + elapsedTime;

                // Counter for looping over the time interval.
                float currentTime = -timeLeftOver;

                // Create particles as long as we have a big enough time interval.
                while (timeToSpend > timeBetweenParticles)
                {
                    currentTime += timeBetweenParticles;
                    timeToSpend -= timeBetweenParticles;

                    Emit();
                }

                // Store any time we didn't use, so it can be part of the next update.
                timeLeftOver = timeToSpend;
            }
        }

        public virtual void Emit()
        {
            Vector3 position = new Vector3(0, 0, 0);
            Vector3 velocity = Vector3.Zero;
            particleSystem.AddParticle(position, velocity);
        }
    }
}
