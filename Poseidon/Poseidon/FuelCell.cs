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
    public class FuelCell : GameObject
    {
        public bool Retrieved { get; set; }
        //SoundEffect RetrievedSound;
        //Temporary power-up types
        //1: speed
        //2: power
        //3: fire rate
        public int powerType;

        public FuelCell(int powerType)
            : base()
        {
            Retrieved = false;
            this.powerType = powerType;
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
            //RetrievedSound = content.Load<SoundEffect>("sound/laserFire");
            
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
                    //if (powerType == 1)
                    //{
                    //    effect.DiffuseColor = Color.Gold.ToVector3();
                    //}
                    //else if (powerType == 2)
                    //{
                    //    effect.DiffuseColor = Color.Red.ToVector3();
                    //}
                    //else if (powerType == 3)
                    //{
                    //    effect.DiffuseColor = Color.Blue.ToVector3();
                    //}
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
