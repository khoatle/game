﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SkinnedModel;
using Poseidon.Core;


namespace Poseidon
{
    public class BaseEnemy : SwimmingObject
    {
        // For the animation
        protected Matrix[] bones;
        protected SkinningData skd;
        protected ClipPlayer clipPlayer;
        protected Matrix enemyMatrix;
        protected Quaternion qRotation = Quaternion.Identity;

        // Percept ID:
        // 0 = nothing detected
        // 1 = tank detected - 1st priory
        // 2 = last target is still in range - 2nd priory
        // 3 = a fish is detected - 3rd priory
        public int[] perceptID;
        public bool[] configBits;

        // What direction I'm facing. This will
        // be calculated using ForwardDirection
        public Vector3 headingDirection;

        // Stunned bit and the timestamp since the last stun
        public bool stunned;
        public double stunnedStartTime;

        // Time stampt since the robot starts chasing
        protected TimeSpan startChasingTime;
        public TimeSpan prevFire;
        protected float timeBetweenFire;

        // Give up chasing
        public TimeSpan giveupTime;

        // Detection range of the enemy
        public float perceptionRadius;

        protected GameObject currentHuntingTarget;

        protected float speed;
        public int damage;

        public bool isHypnotise;
        protected TimeSpan startHypnotiseTime;

        public virtual void Load(int clipStart, int clipEnd, int fps)
        {
            skd = Model.Tag as SkinningData;
            clipPlayer = new ClipPlayer(skd, fps);//ClipPlayer running at 24 frames/sec
            AnimationClip clip = skd.AnimationClips["Take 001"]; //Take name from the dude.fbx file
            clipPlayer.play(clip, clipStart, clipEnd, true);
            enemyMatrix = Matrix.CreateScale(0.1f) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                               Matrix.CreateTranslation(Position);
            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;
            scaledSphere.Radius *= 0.1f;
            BoundingSphere =
                new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);
            //isBigBoss = true;
            //random = new Random();
            //health = 1000;
            //maxHealth = 1000;
            //perceptionRadius = GameConstants.BossPerceptionRadius;
            //experienceReward = 400; //3000
        }

        public BaseEnemy()
            : base()
        {
            giveupTime = new TimeSpan(0, 0, 3);
            perceptionRadius = GameConstants.EnemyPerceptionRadius;
            timeBetweenFire = 0.5f;
            stunned = false;
            prevFire = new TimeSpan();
            health = GameConstants.DefaultEnemyHP;
            speed = GameConstants.EnemySpeed;
            damage = GameConstants.DefaultEnemyDamage;
            experienceReward = 20;
        }

        public void setHypnotise() {
            isHypnotise = true;
            currentHuntingTarget = null;
            startHypnotiseTime = PlayGameScene.timming.TotalGameTime;
        }

        public void wearOutHypnotise() {
            isHypnotise = false;
            currentHuntingTarget = null;
        }

        // Is it the time to forget about old target?
        protected bool clearMind()
        {
            if (startChasingTime.TotalSeconds == 0 ||
                PlayGameScene.timming.TotalGameTime.TotalSeconds - startChasingTime.TotalSeconds > giveupTime.TotalSeconds)
            {
                currentHuntingTarget = null;
                startChasingTime = PlayGameScene.timming.TotalGameTime;
                return true;
            }
            return false;
        }

        // Calculate the facing vector
        protected void calculateHeadingDirection()
        {
            ForwardDirection = Tank.CalculateAngle(currentHuntingTarget.Position, Position);
            headingDirection = currentHuntingTarget.Position - Position;
            headingDirection.Normalize();
        }

        // Go straight
        protected virtual void goStraight(SwimmingObject[] enemies, int enemiesAmount, SwimmingObject[] fishes, int fishAmount, Tank tank)
        {
            Vector3 futurePosition = Position + speed * headingDirection;
            if (Collision.isBarriersValidMove(this, futurePosition, enemies, enemiesAmount, tank)
                    && Collision.isBarriersValidMove(this, futurePosition, fishes, fishAmount, tank))
            {
                Position = futurePosition;
                BoundingSphere.Center = Position;
            }
        }

        public virtual void Update(SwimmingObject[] enemyList, int enemySize, SwimmingObject[] fishList, int fishSize, int changeDirection, Tank tank, List<DamageBullet> enemyBullets, List<DamageBullet> alliesBullets) {
        }

        public override void Draw(Matrix view, Matrix projection)
        {
            bones = clipPlayer.GetSkinTransforms();

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {

                    effect.SetBoneTransforms(bones);
                    effect.View = view;
                    effect.Projection = projection;
                    if (isHypnotise)
                    {
                        effect.DiffuseColor = Color.Red.ToVector3();
                    }
                    else
                        effect.DiffuseColor = Color.White.ToVector3();
                    effect.FogEnabled = true;
                    effect.FogStart = GameConstants.FogStart;
                    effect.FogEnd = GameConstants.FogEnd;
                    effect.FogColor = GameConstants.FogColor.ToVector3();
                }
                mesh.Draw();
            }
            //Matrix[] transforms = new Matrix[Model.Bones.Count];
            //Model.CopyAbsoluteBoneTransformsTo(transforms);
            ////Matrix translateMatrix = Matrix.CreateTranslation(Position);
            ////Matrix worldMatrix = translateMatrix;
            //Matrix worldMatrix = Matrix.Identity;
            //Matrix rotationYMatrix = Matrix.CreateRotationY(ForwardDirection);
            //Matrix translateMatrix = Matrix.CreateTranslation(Position);
            //worldMatrix = rotationYMatrix * translateMatrix;

            //foreach (ModelMesh mesh in Model.Meshes)
            //{
            //    foreach (BasicEffect effect in mesh.Effects)
            //    {
            //        effect.World =
            //            worldMatrix * transforms[mesh.ParentBone.Index];
            //        effect.View = view;
            //        effect.Projection = projection;
            //        if (isHypnotise)
            //        {
            //            effect.DiffuseColor = Color.Black.ToVector3();
            //        }
            //        else
            //            effect.DiffuseColor = Color.White.ToVector3();

            //        effect.EnableDefaultLighting();
            //        effect.PreferPerPixelLighting = true;

            //        effect.FogEnabled = true;
            //        effect.FogStart = GameConstants.FogStart;
            //        effect.FogEnd = GameConstants.FogEnd;
            //        effect.FogColor = GameConstants.FogColor.ToVector3();
            //    }
            //    mesh.Draw();
            //}
        }

        // Go randomly is default move
        protected void randomWalk(int changeDirection, SwimmingObject[] enemies, int enemiesAmount, SwimmingObject[] fishes, int fishAmount, Tank tank)
        {
            Vector3 futurePosition = Position;
            //int barrier_move
            Random random = new Random();
            float turnAmount = 0;
            //also try to change direction if we are stuck
            if (stucked == true)
            {
                ForwardDirection += MathHelper.PiOver4;
            }
            else if (changeDirection >= 95)
            {
                int rightLeft = random.Next(2);
                if (rightLeft == 0)
                    turnAmount = 20;
                else turnAmount = -20;
            }

            Matrix orientationMatrix;
            // Vector3 speed;
            Vector3 movement = Vector3.Zero;

            movement.Z = 1;
            float prevForwardDir = ForwardDirection;
            Vector3 prevFuturePosition = futurePosition;
            // try upto 10 times to change direction is there is collision
            for (int i = 0; i < 4; i++) {
                ForwardDirection += turnAmount * GameConstants.TurnSpeed;
                orientationMatrix = Matrix.CreateRotationY(ForwardDirection);
                headingDirection = Vector3.Transform(movement, orientationMatrix);
                headingDirection *= GameConstants.EnemySpeed;
                futurePosition = Position + headingDirection;

                if (Collision.isBarriersValidMove(this, futurePosition, enemies, enemiesAmount, tank)
                    && Collision.isBarriersValidMove(this, futurePosition, fishes, fishAmount, tank))
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
                } else {
                    stucked = true;
                    futurePosition = prevFuturePosition;
                }
            }
        }
    }
}