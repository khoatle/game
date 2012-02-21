using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon
{
    public class Torpedo : DamageBullet
    {
        public float forwardDir;
        GameMode gameMode;
        public ParticleEmitter trailEmitter;
        ParticleManagement particleManager;

        public Torpedo() : base() { }

        public void initialize(Vector3 position, Vector3 headingDirection, float speed,
            int damage, GameObject target, BaseEnemy shooter, GameMode gameMode)
        {
            base.initialize(position, headingDirection, speed, damage, shooter);
            this.unitDirection = headingDirection; //target.Position - position;
            this.unitDirection.Normalize();
            this.forwardDir = shooter.ForwardDirection;
            this.gameMode = gameMode;
            if (gameMode == GameMode.MainGame)
            {
                this.particleManager = PlayGameScene.particleManager;
            }
            else if (gameMode == GameMode.ShipWreck)
            {
                this.particleManager = ShipWreckScene.particleManager;
            }
            else if (gameMode == GameMode.SurvivalMode)
            {
                this.particleManager = SurvivalGameScene.particleManager;
            }

            // Use the particle emitter helper to output our trail particles.
            trailEmitter = new ParticleEmitter(particleManager.projectileTrailParticles,
                                               GameConstants.trailParticlesPerSecond, position);

        }

        public override void update(GameTime gameTime)
        {
            
            Position += unitDirection * projectionSpeed;
            BoundingSphere = new BoundingSphere(Position, BoundingSphere.Radius);

            // Update the particle emitter, which will create our particle trail.
            trailEmitter.Update(gameTime, Position);

            EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);
        }

        public override void draw(Matrix view, Matrix projection)
        {
            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix translationMatrix = Matrix.CreateTranslation(Position);
          
            Matrix rotationMatrix = Matrix.CreateRotationY(forwardDir);
            Matrix worldMatrix = rotationMatrix * translationMatrix;

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = worldMatrix * transforms[mesh.ParentBone.Index];
                    effect.DiffuseColor = Color.Gold.ToVector3();
                    //effect.AmbientLightColor = Color.Red.ToVector3();
                    //effect.SpecularColor = Color.White.ToVector3();
                    //effect.SpecularPower = 10.0f;
                    effect.View = view;
                    effect.Projection = projection;

                    //effect.Alpha = 0.2f;
                    effect.LightingEnabled = true;
                    //effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;

                    effect.FogEnabled = true;
                    effect.FogStart = GameConstants.FogStart;
                    effect.FogEnd = GameConstants.FogEnd;
                    effect.FogColor = fogColor.ToVector3();
                }
                mesh.Draw();
            }
        }
    }
}
