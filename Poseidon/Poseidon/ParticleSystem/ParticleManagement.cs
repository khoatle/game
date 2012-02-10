using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Poseidon
{
    public class ParticleManagement
    {
        public ParticleSystem explosionParticles;
        public ParticleSystem sandParticles;
        public ParticleSystem projectileTrailParticles;


        public ParticleManagement(Game game, GraphicsDevice graphicsDevice)
        {
            // Construct our particle system components.
            explosionParticles = new ParticleSystem(game, PoseidonGame.contentManager, "ExplosionSettings", graphicsDevice);
            sandParticles = new ParticleSystem(game, PoseidonGame.contentManager, "SandSettings", graphicsDevice);
            projectileTrailParticles = new ParticleSystem(game, PoseidonGame.contentManager, "ProjectileTrailSettings", graphicsDevice);

            sandParticles.DrawOrder = 200;
            explosionParticles.DrawOrder = 400;
            projectileTrailParticles.DrawOrder = 300;
            explosionParticles.Load();
            sandParticles.Load();
            projectileTrailParticles.Load();
            
        }

        public void CreateTrailEmitter(Vector3 position)
        {

        }
        public void UpdateTrailEmitter(Vector3 position, GameTime gameTime)
        {

        }
        public void Update(GameTime gameTime)
        {
            //update particle systems
            explosionParticles.Update(gameTime);
            sandParticles.Update(gameTime);
            projectileTrailParticles.Update(gameTime);
        }

        public void Draw(Matrix view, Matrix projection, GameTime gameTime)
        {
            //draw particle effects
            // Pass camera matrices through to the particle system components.
            explosionParticles.SetCamera(view, projection);
            sandParticles.SetCamera(view, projection);
            projectileTrailParticles.SetCamera(view, projection);
            explosionParticles.Draw(gameTime);
            sandParticles.Draw(gameTime);
            projectileTrailParticles.Draw(gameTime);
        }
    }
}
