using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Poseidon
{

    public class Plant : GameObject
    {
        //public int MaxRange { get; set; }

        public double creationTime;
        public bool timeForFruit;
        public int fruitCreated;
        
        public Plant()
            : base()
        {
            //MaxRange = GameConstants.MaxRange;
            timeForFruit = false;
            fruitCreated = 0;
        }

        public void LoadContent(ContentManager content, Vector3 cyborgPosition, double loadTime)
        {
            creationTime = loadTime;
            Model = content.Load<Model>("Models/plant");
            Position = cyborgPosition;
            BoundingSphere = CalculateBoundingSphere();
            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;
            scaledSphere.Center = Position;
            scaledSphere.Radius *=
                GameConstants.PlantBoundingSphereFactor;
            BoundingSphere =
                new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);
        }

        public void Draw(Matrix view, Matrix projection, float growth)
        {
            if (growth > GameConstants.FruitGrowth) // stop plant growth
            {
                if (growth > (GameConstants.FruitGrowth * (fruitCreated + 1)))
                    timeForFruit = true;
                growth = GameConstants.FruitGrowth;
            }
            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix translateMatrix = Matrix.CreateScale(1.0f,growth,1.0f) * Matrix.CreateTranslation(Position);
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

                
    }

}
