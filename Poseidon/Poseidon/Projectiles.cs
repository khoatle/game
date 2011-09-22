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
    public class Projectiles : GameObject {
        private Vector3 unitDirection;
        private float projectionSpeed;
        private float forwardDirection;
        public int bulletDamage = GameConstants.DefaultBulletDamage;
        private Viewport view;

        private bool isActive;

        public Projectiles() : base() {
            isActive = true;
        }

        public void initialize(Viewport viewport, Vector3 position,float speed, float forwardDirection) {
            this.forwardDirection = forwardDirection;
            this.Position = position;
            this.view = viewport;

            projectionSpeed = speed;
            calculateUnitDirection();
            unitDirection.Normalize();
            isActive = true;
        }

        private void calculateUnitDirection() {
            Matrix orientationMatrix = Matrix.CreateRotationY(forwardDirection);
            Vector3 movement = Vector3.Zero;
            movement.Z = 1;// GameConstants.BulletSpeed;
            unitDirection = Vector3.Transform(movement, orientationMatrix);
            unitDirection.Normalize();
        }

        private Vector3 calculateFuturePosition() {
            return Position + unitDirection * GameConstants.BulletSpeed;
        }
        
        public void update(List<Barrier> barriers) {
            Vector3 tmp = calculateFuturePosition();

            if (isActive && Collision.isOutOfMap(tmp) && Collision.isOutOfView(tmp, view)) {
                isActive = false;
                return;
            }
            Position = tmp;
            BoundingSphere = new BoundingSphere(tmp, BoundingSphere.Radius);
        }

        public void loadContent(ContentManager content, string modelName) {
            this.Model = content.Load<Model>(modelName);
            BoundingSphere = CalculateBoundingSphere();
        }

        public bool getStatus() { return isActive; }

        public void draw(Matrix view, Matrix projection) {
            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix translationMatrix = Matrix.CreateTranslation(Position);
            Matrix worldMatrix = translationMatrix;

            foreach (ModelMesh mesh in Model.Meshes) {
                foreach (BasicEffect effect in mesh.Effects) {
                    effect.World = worldMatrix * transforms[mesh.ParentBone.Index];
                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }
                mesh.Draw();
            }
        }
    }
}