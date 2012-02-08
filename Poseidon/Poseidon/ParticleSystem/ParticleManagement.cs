using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Poseidon
{
    class ParticleManagement
    {
        public ParticleSystem explosionParticles;
        public ParticleSystem sandParticles;

        public ParticleManagement(Game game, GraphicsDevice graphicsDevice)
        {
            // Construct our particle system components.
            explosionParticles = new ParticleSystem(game, PoseidonGame.contentManager, "ExplosionSettings", graphicsDevice);
            sandParticles = new ParticleSystem(game, PoseidonGame.contentManager, "SandSettings", graphicsDevice);
            sandParticles.DrawOrder = 400;
            explosionParticles.DrawOrder = 400;
            explosionParticles.Load();
            sandParticles.Load();
        }

        public void Update(GameTime gameTime)
        {
            //update particle systems
            explosionParticles.Update(gameTime);
            sandParticles.Update(gameTime);
        }

        public void Draw(Matrix view, Matrix projection, GameTime gameTime)
        {
            //draw particle effects
            // Pass camera matrices through to the particle system components.
            explosionParticles.SetCamera(view, projection);
            sandParticles.SetCamera(view, projection);
            explosionParticles.Draw(gameTime);
            sandParticles.Draw(gameTime);
        }
    }
}
