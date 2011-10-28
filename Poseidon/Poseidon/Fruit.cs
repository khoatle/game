using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Audio;

namespace Poseidon
{
    public class Fruit : GameObject
    {
        public bool Retrieved { get; set; }
        //SoundEffect RetrievedSound;
        //Temporary power-up types
        //1: speed
        //2: power
        //3: fire rate
        public int powerType;

        public Fruit(int powerType)
            : base()
        {
            Retrieved = false;
            this.powerType = powerType;
        }

        public void LoadContent(ContentManager content, Vector3 plantPosition)
        {
            if (this.powerType == 1)
                Model = content.Load<Model>("Models/PlantAndFruitModels/green-fruit");
            else if (this.powerType == 2)
                Model = content.Load<Model>("Models/PlantAndFruitModels/red-fruit");
            else if (this.powerType == 3)
                Model = content.Load<Model>("Models/PlantAndFruitModels/blue-fruit");
            Position = plantPosition;
            Position.Y = GameConstants.MainGameFloatHeight;
            BoundingSphere = CalculateBoundingSphere();
            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;
            scaledSphere.Center = Position;
            scaledSphere.Radius *=
                GameConstants.FruitBoundingSphereFactor;
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
                    //effect.EmissiveColor = Color.White.ToVector3();
                    effect.FogEnabled = true;
                    effect.FogStart = GameConstants.FogStart;
                    effect.FogEnd = GameConstants.FogEnd;
                    effect.FogColor = GameConstants.FogColor.ToVector3();
                }
                mesh.Draw();
            }
        }

        internal void Update(KeyboardState keyboardState, BoundingSphere vehicleBoundingSphere, BoundingSphere vehicleTrashFruitBoundingSphere)
        {
            //if (vehicleBoundingSphere.Intersects(this.BoundingSphere))
            //    this.Retrieved = true;
            //if (keyboardState.IsKeyDown(Keys.Z))
            //{
            //    if (vehicleTrashFruitBoundingSphere.Intersects(this.BoundingSphere))
            //    {
            //        RetrievedSound.Play();
            //        this.Retrieved = true;
            //    }
                
            //}
            
        }

    }

}
