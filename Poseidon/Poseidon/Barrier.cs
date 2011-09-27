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
    public class Barrier : GameObject
    {
        public string BarrierType { get; set; }
        public float ForwardDirection { get; set; }
        public int MaxRange { get; set; }
        public int health = GameConstants.EnemyHP;
        //is the object stucked and needs to change direction?
        public bool stucked = false;
        public Barrier()
            : base()
        {
            BarrierType = null;
            ForwardDirection = 0.0f;
            MaxRange = GameConstants.MaxRange;

        }

        public void LoadContent(ContentManager content, string modelName)
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

        public void Draw(Matrix view, Matrix projection)
        {
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

                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }
                mesh.Draw();
            }
        }

        public void Update(Barrier[] barriers, int size, int ChangeDirection, Tank tank)
        {
            Vector3 futurePosition = Position;
            Random random = new Random();
            //int barrier_move;
            float turnAmount = 0;
            //also try to change direction if we are stuck
            if (ChangeDirection >= 95 || stucked == true)
            {
                int rightLeft = random.Next(2);
                if (rightLeft == 0)
                    turnAmount = 20;
                else turnAmount = -20;
            }
            
            Matrix orientationMatrix;
            Vector3 speed;
            Vector3 movement = Vector3.Zero;

            movement.Z = 1;
            float prevForwardDir = ForwardDirection;
            // try upto 10 times to change direction is there is collision
            for (int i=0; i<4; i++)
            {
                ForwardDirection += turnAmount * GameConstants.TurnSpeed;
                orientationMatrix = Matrix.CreateRotationY(ForwardDirection);
                speed = Vector3.Transform(movement, orientationMatrix);
                speed *= GameConstants.BarrierVelocity;
                futurePosition = Position + speed;

                if (Collision.isBarrierValidMove(this, futurePosition, barriers, size, tank))
                {
                    Position = futurePosition;

                    BoundingSphere updatedSphere;
                    updatedSphere = BoundingSphere;

                    updatedSphere.Center.X = Position.X;
                    updatedSphere.Center.Z = Position.Z;
                    BoundingSphere = new BoundingSphere(updatedSphere.Center,
                        updatedSphere.Radius);
                    stucked = false;
                    break;
                }
                else stucked = true;
                //ForwardDirection = prevForwardDir;
                //turnAmount = -turnAmount;
            }
            
        }
    }
}
