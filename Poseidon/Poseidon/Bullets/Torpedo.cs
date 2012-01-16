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

        public Torpedo() : base() { }

        public void initialize(Vector3 position, Vector3 headingDirection, float speed,
            int damage, GameObject target, BaseEnemy shooter)
        {
            base.initialize(position, headingDirection, speed, damage, shooter);
            this.unitDirection = headingDirection; //target.Position - position;
            this.unitDirection.Normalize();
            this.forwardDir = shooter.ForwardDirection;
        }

        public override void update()
        {
            
            Position += unitDirection * projectionSpeed;
            BoundingSphere = new BoundingSphere(Position, BoundingSphere.Radius);
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
                    effect.View = view;
                    effect.Projection = projection;

                    //effect.Alpha = 0.2f;

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
