using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon
{
    class FuelCell : GameObject
    {
        public bool Retrieved { get; set; }

        public FuelCell()
            : base()
        {
            Retrieved = false;
        }

        public void LoadContent(ContentManager content, string modelName)
        {
            Model = content.Load<Model>(modelName);
            Position = Vector3.Down;

            BoundingSphere = CalculateBoundingSphere();

            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;
            scaledSphere.Radius *= GameConstants.FuelCellBoundingSphereFactor;
            BoundingSphere =
                new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);
        }

        public void Draw(Matrix view, Matrix projection)
        {
            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix translateMatrix = Matrix.CreateTranslation(Position);
            Matrix worldMatrix = translateMatrix;

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World =
                        worldMatrix * transforms[mesh.ParentBone.Index];
                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }
                mesh.Draw();
            }
        }

        internal void Update(BoundingSphere vehicleBoundingSphere)
        {
            if (vehicleBoundingSphere.Intersects(this.BoundingSphere))
                this.Retrieved = true;
        }
    }

}
