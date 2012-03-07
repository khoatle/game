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
        public ParticleSystem sandParticles, sandParticlesForFactory;
        public ParticleSystem projectileTrailParticles;
        public ParticleSystem frozenBreathParticles;


        public ParticleManagement(Game game, GraphicsDevice graphicsDevice)
        {
            // Construct our particle system components.
            explosionParticles = new ParticleSystem(game, PoseidonGame.contentManager, "ExplosionSettings", graphicsDevice);
            sandParticles = new ParticleSystem(game, PoseidonGame.contentManager, "SandSettings", graphicsDevice);
            sandParticlesForFactory = new ParticleSystem(game, PoseidonGame.contentManager, "SandSettingsForFactory", graphicsDevice);
            projectileTrailParticles = new ParticleSystem(game, PoseidonGame.contentManager, "ProjectileTrailSettings", graphicsDevice);
            frozenBreathParticles = new ParticleSystem(game, PoseidonGame.contentManager, "FrozenBreathSettings", graphicsDevice);

            sandParticles.DrawOrder = 200;
            sandParticlesForFactory.DrawOrder = 200;
            explosionParticles.DrawOrder = 400;
            projectileTrailParticles.DrawOrder = 300;
            frozenBreathParticles.DrawOrder = 400;

            explosionParticles.Load();
            sandParticles.Load();
            sandParticlesForFactory.Load();
            projectileTrailParticles.Load();
            frozenBreathParticles.Load();
       
        }

        public void Update(GameTime gameTime)
        {
            //update particle systems
            explosionParticles.Update(gameTime);

            sandParticles.Update(gameTime);
            sandParticlesForFactory.Update(gameTime);
            //tint the sand particle to the color of the environment
            //i.e: dirty sea, red sea, black sea etc.
            EffectHelpers.GetEffectConfiguration(ref sandParticles.fogColor,ref sandParticles.ambientColor,ref sandParticles.diffuseColor,ref sandParticles.specularColor);
            sandParticles.TintParticleColor();
            EffectHelpers.GetEffectConfiguration(ref sandParticlesForFactory.fogColor, ref sandParticlesForFactory.ambientColor, ref sandParticlesForFactory.diffuseColor, ref sandParticlesForFactory.specularColor);
            sandParticlesForFactory.TintParticleColor();

            projectileTrailParticles.Update(gameTime);
            frozenBreathParticles.Update(gameTime);
        }

        public void Draw(Matrix view, Matrix projection, GameTime gameTime)
        {
            //draw particle effects
            // Pass camera matrices through to the particle system components.
            explosionParticles.SetCamera(view, projection);
            sandParticles.SetCamera(view, projection);
            sandParticlesForFactory.SetCamera(view, projection);
            projectileTrailParticles.SetCamera(view, projection);
            frozenBreathParticles.SetCamera(view, projection);

            explosionParticles.Draw(gameTime);
            sandParticles.Draw(gameTime);
            sandParticlesForFactory.Draw(gameTime);
            projectileTrailParticles.Draw(gameTime);
            frozenBreathParticles.Draw(gameTime);
        }
    }
}
