using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon
{
    public class Trash : GameObject
    {
        public int experienceReward;
        public int environmentReward;

        public bool Retrieved { get; set; }

        public float orientation;

        //public float heightChange = 0.5f;
        //public float currentChange = 0.0f;
        //public bool floatUp;
        public Trash()
            : base()
        {
            Retrieved = false;
            experienceReward = 10;
            environmentReward = 10;
        }

        public void LoadContent(ContentManager content, string modelname, float orientation)
        {
            Model = content.Load<Model>(modelname);
            Position = Vector3.Down;
            BoundingSphere = CalculateBoundingSphere();

            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;
            scaledSphere.Radius *= GameConstants.TrashBoundingSphereFactor;
            BoundingSphere =
                new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);
            this.orientation = orientation;
            //if (orientation > 50) floatUp = true;
            //else floatUp = false;
        }
        public void Update(GameTime gameTime)
        {
            //for floating trash
            //if (currentChange >= heightChange)
            //{
            //    currentChange = 0.0f;
            //    floatUp = !floatUp;
            //}
            //currentChange += 0.025f;
            //if (floatUp) Position.Y += currentChange;
            //else Position.Y -= currentChange;
        }
        public void Draw(Matrix view, Matrix projection)
        {
            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix translateMatrix = Matrix.CreateTranslation(Position);
            Matrix rotationYMatrix = Matrix.CreateRotationY(orientation);
            Matrix worldMatrix = rotationYMatrix * translateMatrix;

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
