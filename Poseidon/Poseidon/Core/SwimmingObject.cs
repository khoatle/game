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

        public Vector3 velocity;

        // Slow down stuff
        public float speedFactor;
        public TimeSpan slowStart;
        public TimeSpan slowDuration;

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
            speedFactor = 1f;

            velocity = new Vector3();
            velocity.Y = GameConstants.MainGameFloatHeight;

            // Slow duration is init as 5
            slowDuration = new TimeSpan(0, 0, 5);
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

        // Go straight
        protected virtual void seekDestination(Vector3 destination, SwimmingObject[] enemies, int enemiesAmount, SwimmingObject[] fishes, int fishAmount, HydroBot hydroBot, float speedFactor)
        {
            //Vector3 futurePosition = Position + speed * headingDirection;
            //if (Collision.isBarriersValidMove(this, futurePosition, enemies, enemiesAmount, hydroBot)
            //        && Collision.isBarriersValidMove(this, futurePosition, fishes, fishAmount, hydroBot))
            //{
            //    Position = futurePosition;
            //    //BoundingSphere.Center = Position;
            //    BoundingSphere.Center.X += speed * headingDirection.X;
            //    BoundingSphere.Center.Z += speed * headingDirection.Z;
            //}
            float pullDistance = Vector3.Distance(destination, Position);
            // float timeFactor = (currentHuntingTarget.GetType().Name.Equals("CombatEnemy")) ? 1.25f : 1f;
            Vector3 futurePosition;

            if (pullDistance > (BoundingSphere.Radius + BoundingSphere.Radius))
            {
                Vector3 pull = (destination - Position) * (1 / pullDistance);
                Vector3 totalPush = Vector3.Zero;

                int contenders = 0;
                for (int i = 0; i < enemiesAmount; i++)
                {
                    if (enemies[i] != this)
                    {
                        Vector3 push = Position - enemies[i].Position;

                        float distance = (Vector3.Distance(Position, enemies[i].Position)) - enemies[i].BoundingSphere.Radius;
                        if (distance < BoundingSphere.Radius * 5)
                        {
                            contenders++;
                            if (distance < 0.0001f) // prevent divide by 0 
                            {
                                distance = 0.0001f;
                            }
                            float weight = 1 / distance;
                            totalPush += push * weight;
                        }
                    }
                }
                Vector3 hydrobotPush = Position - hydroBot.Position;
                float hydroDistance = (Vector3.Distance(Position, hydroBot.Position)) - hydroBot.BoundingSphere.Radius;
                if (hydroDistance < BoundingSphere.Radius * 5) {
                    contenders++;
                    if (hydroDistance < 0.0001f) // prevent divide by 0 
                    {
                        hydroDistance = 0.0001f;
                    }
                    float weight = 1 / hydroDistance;
                    totalPush += hydrobotPush * weight;
                }

                for (int i = 0; i < fishAmount; i++)
                {
                    if (fishes[i] != this)
                    {
                        Vector3 push = Position - fishes[i].Position;

                        float distance = (Vector3.Distance(Position, fishes[i].Position) - fishes[i].BoundingSphere.Radius) - BoundingSphere.Radius;
                        if (distance < BoundingSphere.Radius * 5)
                        {
                            contenders++;
                            if (distance < 0.0001f) // prevent divide by 0 
                            {
                                distance = 0.0001f;
                            }
                            float weight = 1 / distance;
                            totalPush += push * weight;
                        }
                    }
                }

                pull *= Math.Max(1, 4 * contenders);
                pull += totalPush;
                pull.Normalize();

                // Speed factor stuff
                futurePosition = Position + (pull * GameConstants.FishSpeed * speedFactor);

                if (Collision.isBarriersValidMove(this, futurePosition, enemies, enemiesAmount, hydroBot)
                        && Collision.isBarriersValidMove(this, futurePosition, fishes, fishAmount, hydroBot))
                {
                    Position = futurePosition;
                    BoundingSphere.Center.X += (pull * GameConstants.FishSpeed * speedFactor).X;
                    BoundingSphere.Center.Z += (pull * GameConstants.FishSpeed * speedFactor).Z;
                    float lastForwardDir = ForwardDirection;
                    ForwardDirection = (float)Math.Atan2(pull.X, pull.Z);
                    PlaySteeringAnimation(lastForwardDir, ForwardDirection);
                }
            }
        }

        public virtual void PlaySteeringAnimation(float lastForwardDir, float curForwardDir)
        {
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
                    effect.FogColor = fogColor.ToVector3();
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
