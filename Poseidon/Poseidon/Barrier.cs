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
        public Vector3 previousDirection;
        public int health = GameConstants.EnemyHP;

        public Barrier()
            : base()
        {
            BarrierType = null;
            ForwardDirection = 0.0f;
            MaxRange = GameConstants.MaxRange;
            previousDirection = Vector3.Zero;
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

        public void Update(List<Barrier> barriers, int ChangeDirection, Tank tank)
        {
            Vector3 futurePosition = Position;
            Random random = new Random();
            int barrier_move;
            float turnAmount = 1;

            ForwardDirection += turnAmount * GameConstants.TurnSpeed;
            Matrix orientationMatrix = Matrix.CreateRotationY(ForwardDirection);

            Vector3 movement = Vector3.Zero;

            if (ChangeDirection >= 95) {
                barrier_move = random.Next(4);
                switch (barrier_move) {
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
            else {
                movement = previousDirection;
            }

            Vector3 speed = Vector3.Transform(movement, orientationMatrix);
            speed *= GameConstants.BarrierVelocity;
            futurePosition = Position + speed;

            if (Collision.isBarrierValidMove(this, futurePosition, barriers, tank))
            {
                Position = futurePosition;

                BoundingSphere updatedSphere;
                updatedSphere = BoundingSphere;

                updatedSphere.Center.X = Position.X;
                updatedSphere.Center.Z = Position.Z;
                BoundingSphere = new BoundingSphere(updatedSphere.Center,
                    updatedSphere.Radius);
            }
            previousDirection = movement;
        }
    }
}
