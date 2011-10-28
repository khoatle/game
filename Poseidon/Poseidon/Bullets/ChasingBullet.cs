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
        public float forwardDir;

        public ChasingBullet() : base() { 
        }

        public void initialize(Vector3 position, Vector3 headingDirection, float speed, int damage, GameObject target) {
            base.initialize(position, headingDirection, speed);
            this.damage = damage;
            this.target = target;
        }

        public override void update() {
            if (target != null)
            {
                unitDirection = target.Position - Position;
                unitDirection.Normalize();
            }

            Position = calculateFuturePosition();
            BoundingSphere = new BoundingSphere(Position, BoundingSphere.Radius);
        }

        public override void draw(Matrix view, Matrix projection)
        {
            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix translationMatrix = Matrix.CreateTranslation(Position);
            forwardDir += MathHelper.PiOver4/4;
            Matrix rotationMatrix = Matrix.CreateRotationY(forwardDir);
            Matrix worldMatrix =  rotationMatrix * translationMatrix;

            foreach (ModelMesh mesh in Model.Meshes) {
                foreach (BasicEffect effect in mesh.Effects){
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
