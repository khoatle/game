using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Poseidon
{
    public class Projectiles : GameObject
    {
        public Vector3 unitDirection;
        public float projectionSpeed;
        
        private bool isActive;

        //float forwardDir;
        public Projectiles()
            : base() {
            isActive = true;
        }

        public virtual void initialize(Vector3 position, Vector3 headingDirection, float speed) {
            this.Position = position;

            projectionSpeed = speed;
            unitDirection = headingDirection;
            unitDirection.Normalize();
            isActive = true;
        }

        //private void calculateUnitDirection()
        //{
        //    Matrix orientationMatrix = Matrix.CreateRotationY(forwardDirection);
        //    Vector3 movement = Vector3.Zero;
        //    movement.Z = 1;// GameConstants.BulletSpeed;
        //    unitDirection = Vector3.Transform(movement, orientationMatrix);
        //    unitDirection.Normalize();
        //}

        protected Vector3 calculateFuturePosition() {
            return Position + unitDirection * projectionSpeed;
        }

        public virtual void update()
        {
            Vector3 tmp = calculateFuturePosition();

            Position = tmp;
            BoundingSphere = new BoundingSphere(tmp, BoundingSphere.Radius);
        }

        public virtual void loadContent(ContentManager content, string modelName)
        {
            this.Model = content.Load<Model>(modelName);
            BoundingSphere = CalculateBoundingSphere();
        }

        public bool getStatus() { return isActive; }

        public virtual void draw(Matrix view, Matrix projection)
        {
            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix translationMatrix = Matrix.CreateTranslation(Position);
            Matrix worldMatrix = translationMatrix;
            //forwardDir += MathHelper.PiOver4 / 4;
            //Matrix rotationMatrix = Matrix.CreateRotationY(forwardDir);
            //Matrix worldMatrix = rotationMatrix * translationMatrix;
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = worldMatrix * transforms[mesh.ParentBone.Index];
                    effect.View = view;
                    effect.DiffuseColor = Color.White.ToVector3();
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;

                    //effect.Alpha = 0.7f;
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