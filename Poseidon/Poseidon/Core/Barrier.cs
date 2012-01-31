// Copyright (C) Microsoft Corporation. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon
{
    public class SwimmingObject : GameObject
    {
        public string BarrierType { get; set; }
        public float ForwardDirection { get; set; }
        public int MaxRange { get; set; }
        public float health = GameConstants.DefaultEnemyHP;
        public float maxHealth = GameConstants.DefaultEnemyHP;
        public string Name = "Swimming Object";
        // Is the object stucked and needs to change direction?
        public bool stucked = false;
        public float accumulatedHealthLossFromPoison;
        public float maxHPLossFromPoisson;
        public float poissonInterval;
        public int basicExperienceReward;

        // is this enemy a big boss
        // in order to know whether the big boss is killed
        // and the level is won
        public bool isBigBoss = false;

        public bool isPoissoned;

        //has this enemy given us exp points already?
        public bool gaveExp;

        public SwimmingObject()
            : base()
        {
            BarrierType = null;
            ForwardDirection = 0.0f;
            //MaxRange = GameConstants.MaxRange;
            basicExperienceReward = 20;
            isPoissoned = false;
            poissonInterval = 0;
            maxHPLossFromPoisson = 50;
            accumulatedHealthLossFromPoison = 0;
            gaveExp = false;
        }

        public virtual void LoadContent(ContentManager content, string modelName)
        {
            Model = content.Load<Model>(modelName);
            BarrierType = modelName;
            Position = Vector3.Down;
            BoundingSphere = CalculateBoundingSphere();

            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;        
            scaledSphere.Radius *= GameConstants.BarrierBoundingSphereFactor;
            BoundingSphere =
                new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);
        }

        public virtual void Draw(Matrix view, Matrix projection) {
            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);
            //Matrix translateMatrix = Matrix.CreateTranslation(Position);
            //Matrix worldMatrix = translateMatrix;
            Matrix worldMatrix = Matrix.Identity;
            Matrix rotationYMatrix = Matrix.CreateRotationY(ForwardDirection);
            Matrix translateMatrix = Matrix.CreateTranslation(Position);
            worldMatrix = rotationYMatrix * translateMatrix;

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World =
                        worldMatrix * transforms[mesh.ParentBone.Index];
                    effect.View = view;
                    effect.Projection = projection;
                    effect.DiffuseColor = Color.White.ToVector3();

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
        public virtual void Draw(Matrix view, Matrix projection, Camera gameCamera, string techniqueName)
        {
        }
        //public virtual void Update(SwimmingObject[] swimmingObjects, int size, int changeDirection, Tank tank)
        //{
            //Vector3 futurePosition = Position;
            ////int barrier_move
            //Random random = new Random();
            //float turnAmount = 0;
            ////also try to change direction if we are stuck
            //if (changeDirection >= 95 || stucked == true)
            //{
            //    int rightLeft = random.Next(2);
            //    if (rightLeft == 0)
            //        turnAmount = 20;
            //    else turnAmount = -20;
            //}
            
            //Matrix orientationMatrix;
            //Vector3 speed;
            //Vector3 movement = Vector3.Zero;

            //movement.Z = 1;
            //float prevForwardDir = ForwardDirection;
            //// try upto 10 times to change direction is there is collision
            //for (int i=0; i<4; i++) {
            //    ForwardDirection += turnAmount * GameConstants.TurnSpeed;
            //    orientationMatrix = Matrix.CreateRotationY(ForwardDirection);
            //    speed = Vector3.Transform(movement, orientationMatrix);
            //    speed *= GameConstants.BarrierVelocity;
            //    futurePosition = Position + speed;

            //    if (Collision.isEnemyValidMove(this, futurePosition, swimmingObjects, size, tank)) {
            //        Position = futurePosition;

            //        BoundingSphere updatedSphere;
            //        updatedSphere = BoundingSphere;

            //        updatedSphere.Center.X = Position.X;
            //        updatedSphere.Center.Z = Position.Z;
            //        BoundingSphere = new BoundingSphere(updatedSphere.Center,
            //            updatedSphere.Radius);
                   
            //        stucked = false;
            //        break;
            //    }
            //    else stucked = true;
            //    //ForwardDirection = prevForwardDir;
            //    //turnAmount = -turnAmount;
            //}
        //}
    }
}
