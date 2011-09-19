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
    class Barrier : GameObject
    {
        public string BarrierType { get; set; }
        public float ForwardDirection { get; set; }
        public int MaxRange { get; set; }

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

        public Vector3 Update(Barrier[] barriers, int ChangeDirection, Vector3 PreviousMovement, Tank tank)
        {
            Vector3 futurePosition = Position;
            Random random = new Random();
            int barrier_move;
            float turnAmount = 1;

            ForwardDirection += turnAmount * GameConstants.TurnSpeed;
            Matrix orientationMatrix = Matrix.CreateRotationY(ForwardDirection);

            Vector3 movement = Vector3.Zero;

            if (ChangeDirection >= 95)
            {
                barrier_move = random.Next(4);
                switch (barrier_move)
                {
                    case 0:
                        movement.X = -1;
                        break;
                    case 1:
                        movement.X = 1;
                        break;
                    case 2:
                        movement.Z = -1;
                        break;
                    case 3:
                        movement.Z = 1;
                        break;
                }
            }
            else
            {
                movement = PreviousMovement;

            }

            Vector3 speed = Vector3.Transform(movement, orientationMatrix);
            speed *= GameConstants.BarrierVelocity;
            futurePosition = Position + speed;

            if (ValidateMovement(futurePosition, barriers, tank))
            {
                Position = futurePosition;

                BoundingSphere updatedSphere;
                updatedSphere = BoundingSphere;

                updatedSphere.Center.X = Position.X;
                updatedSphere.Center.Z = Position.Z;
                BoundingSphere = new BoundingSphere(updatedSphere.Center,
                    updatedSphere.Radius);
            }
            return movement;
        }

        private bool ValidateMovement(Vector3 futurePosition,
            Barrier[] barriers, Tank tank)
        {
            BoundingSphere futureBoundingSphere = BoundingSphere;
            futureBoundingSphere.Center.X = futurePosition.X;
            futureBoundingSphere.Center.Z = futurePosition.Z;

            //Don't allow off-terrain driving
            if ((Math.Abs(futurePosition.X) > MaxRange) ||
                (Math.Abs(futurePosition.Z) > MaxRange))
                return false;
            //Don't allow driving through a barrier
            if (CheckForBarrierCollision(futureBoundingSphere, barriers))
                return false;
            //Don't allow driving through fuel carrier
            if (CheckForFuelCarrierCollision(futureBoundingSphere, tank))
                return false;

            return true;
        }

        private bool CheckForBarrierCollision(
            BoundingSphere vehicleBoundingSphere, Barrier[] barriers)
        {
            for (int curBarrier = 0; curBarrier < barriers.Length; curBarrier++)
            {
                if (this.Equals(barriers[curBarrier]))
                    continue;
                if (vehicleBoundingSphere.Intersects(
                    barriers[curBarrier].BoundingSphere))
                    return true;
            }
            return false;
        }

        private bool CheckForFuelCarrierCollision(
            BoundingSphere vehicleBoundingSphere, Tank tank)
        {
            if (vehicleBoundingSphere.Intersects(tank.BoundingSphere))
                return true;
            return false;
        }
    }


}
