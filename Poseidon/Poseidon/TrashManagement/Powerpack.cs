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
    public class Powerpack : GameObject
    {
        public bool Retrieved { get; set; }
        //SoundEffect RetrievedSound;
        //Temporary power-up types
        //1: speed
        //2: power
        //3: fire rate
        //4: health point
        //5: Strange Rock
        public int powerType;
        private float orientation; //rotation in radians

        public Powerpack(int powerType)
            : base()
        {
            Retrieved = false;
            orientation = 0f;
            this.powerType = powerType;
        }

        public void LoadContent(ContentManager content, Vector3 powerpackPosition)
        {
            if (this.powerType == 1)
                Model = content.Load<Model>("Models/PlantAndFruitModels/green-fruit");
            else if (this.powerType == 2)
                Model = content.Load<Model>("Models/PlantAndFruitModels/red-fruit");
            else if (this.powerType == 3)
                Model = content.Load<Model>("Models/PlantAndFruitModels/blue-fruit");
            else if (this.powerType == 4)
                Model = content.Load<Model>("Models/PlantAndFruitModels/white-fruit");
            else if (this.powerType == 5) //Strange Rock
                Model = content.Load<Model>("Models/BulletModels/bossBullet");
            
            Position = powerpackPosition;
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
            Update(); //since update is only changing orientation, it is better to put here than in playgamescene & survival scene
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
                    //effect.EmissiveColor = Color.White.ToVector3();
                    effect.FogEnabled = true;
                    effect.FogStart = GameConstants.FogStart;
                    effect.FogEnd = GameConstants.FogEnd;
                    effect.FogColor = GameConstants.FogColor.ToVector3();
                }
                mesh.Draw();
            }
        }

        public void Update()
        {
            orientation += GameConstants.powerpackResourceRotationSpeed;
        }

    }

}
