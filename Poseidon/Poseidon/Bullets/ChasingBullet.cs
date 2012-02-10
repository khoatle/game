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
    public class ChasingBullet : DamageBullet {
        public GameObject target;
        public bool stopChasing;
        public float forwardDir;
        public float stopChasingTime;

        public ChasingBullet() : base() { }

        public void initialize(Vector3 position, Vector3 headingDirection, float speed, 
            int damage, GameObject target, BaseEnemy shooter) {
                base.initialize(position, headingDirection, speed, damage, shooter);
                this.target = target;
                this.unitDirection = target.Position - position;
                this.unitDirection.Normalize();
                stopChasing = false;
                stopChasingTime = GameConstants.StopBulletChasing;
        }

        public override void update(GameTime gameTime)
        {
            if (target != null && target.GetType().Name.Equals("HydroBot") && !stopChasing) {
                if (Vector3.Distance(target.Position, Position) > target.BoundingSphere.Radius * 6) {
                    Matrix orientationMatrix = Matrix.CreateRotationY(((HydroBot)target).ForwardDirection);
                    Vector3 movement = Vector3.Zero;
                    movement.Z = 1;
                    Vector3 shootingDirection = Vector3.Transform(movement, orientationMatrix);
                    shootingDirection.Normalize();

                    stopChasingTime -= 1 / 60;

                    unitDirection = (target.Position - Position) + 20*shootingDirection;
                    unitDirection.Normalize();
        
                }
                else {
                    if (stopChasingTime <= 0) {
                        stopChasing = true;
                        stopChasingTime = GameConstants.StopBulletChasing;
                    }
                }
            }
            Position += unitDirection * projectionSpeed;
            BoundingSphere = new BoundingSphere(Position, BoundingSphere.Radius);
        }

        public override void draw(Matrix view, Matrix projection)
        {
            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix translationMatrix = Matrix.CreateTranslation(Position);
            forwardDir += MathHelper.PiOver4 / 4;
            Matrix rotationMatrix = Matrix.CreateRotationY(forwardDir);
            Matrix worldMatrix = rotationMatrix * translationMatrix;

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = worldMatrix * transforms[mesh.ParentBone.Index];
                    effect.DiffuseColor = Color.Gold.ToVector3();
                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;

                    effect.FogEnabled = true;
                    effect.FogStart = GameConstants.FogStart;
                    effect.FogEnd = GameConstants.FogEnd;
                    effect.FogColor = GameConstants.FogColor.ToVector3();
                }
                mesh.Draw();
            }
        }
    }
}
