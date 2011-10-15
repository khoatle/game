using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SkinnedModel;
using Poseidon.Core;
namespace Poseidon
{
    class MutantShark : CombatEnemy
    {

        Matrix[] bones;
        SkinningData skd;
        ClipPlayer clipPlayer;
        Matrix fishMatrix;
        Quaternion qRotation = Quaternion.Identity;

        public MutantShark() : base() {
        }

        public void Load(int clipStart, int clipEnd, int fpsRate)
        {
            skd = Model.Tag as SkinningData;
            clipPlayer = new ClipPlayer(skd, fpsRate);//ClipPlayer running at 24 frames/sec
            AnimationClip clip = skd.AnimationClips["Take 001"]; //Take name from the dude.fbx file
            clipPlayer.play(clip, clipStart, clipEnd, true);
            fishMatrix = Matrix.CreateScale(1.0f) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                               Matrix.CreateTranslation(Position);
            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;
            //scaledSphere.Radius *= 1.5f;
            BoundingSphere =
                new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);

        }

        public override void LoadContent(ContentManager content, string modelName)
        {
            Model = content.Load<Model>(modelName);
            BarrierType = modelName;
            Position = Vector3.Down;
            BoundingSphere = CalculateBoundingSphere();

            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;
            scaledSphere.Radius *= 0.3f;
            BoundingSphere =
                new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);
            isBigBoss = true;
            health = 1000;
            maxHealth = 1000;
            perceptionRadius = GameConstants.BossPerceptionRadius;
            experienceReward = 200; //1000
            this.Load(1,24,24);
        }
        public override void Update(SwimmingObject[] enemyList, int enemySize, SwimmingObject[] fishList, int fishSize, int changeDirection, Tank tank, List<DamageBullet> enemyBullets, List<DamageBullet> alliesBullets)
        {
            if (stunned) return;

            if (isHypnotise && PlayGameScene.timming.TotalGameTime.TotalSeconds - startHypnotiseTime.TotalSeconds > GameConstants.timeHypnotiseLast)
            {
                wearOutHypnotise();
            }

            if (!isHypnotise)
            {
                int perceptionID = perceptAndLock(tank, fishList, fishSize);
                configAction(perceptionID);
                makeAction(changeDirection, enemyList, enemySize, fishList, fishSize, enemyBullets, tank);
            }
            else
            {
                int perceptionID = perceptAndLock(tank, enemyList, enemySize);
                configAction(perceptionID);
                makeAction(changeDirection, enemyList, enemySize, fishList, fishSize, enemyBullets, tank);
            }


            // if clip player has been initialized, update it
            if (clipPlayer != null)
            {
                qRotation = Quaternion.CreateFromAxisAngle(
                                Vector3.Up,
                                ForwardDirection);
                float scale = 1.0f;

                fishMatrix = Matrix.CreateScale(scale) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                                    Matrix.CreateFromQuaternion(qRotation) *
                                    Matrix.CreateTranslation(Position);
                clipPlayer.update(PlayGameScene.timming.ElapsedGameTime, true, fishMatrix);
            }
        }
        // Execute the actions
        protected override void makeAction(int changeDirection, SwimmingObject[] enemies, int enemiesAmount, SwimmingObject[] fishes, int fishAmount, List<DamageBullet> bullets, Tank tank)
        {
            if (configBits[0] == true)
            {
                if (!clipPlayer.inRange(1, 24) && !configBits[3])
                    clipPlayer.switchRange(1, 24);
                randomWalk(changeDirection, enemies, enemiesAmount, fishes, fishAmount, tank);
                return;
            }
            if (currentHuntingTarget != null)
            {
                calculateHeadingDirection();
                calculateFutureBoundingSphere();
            }
            if (configBits[2] == true)
            {
                if (!clipPlayer.inRange(1, 24) && !configBits[3])
                    clipPlayer.switchRange(1, 24);
                goStraight(enemies, enemiesAmount, fishes, fishAmount, tank);
            }
            if (configBits[3] == true)
            {
                startChasingTime = PlayGameScene.timming.TotalGameTime;

                if (currentHuntingTarget.GetType().Name.Equals("Fish"))
                {
                    Fish tmp = (Fish)currentHuntingTarget;
                    if (tmp.health <= 0)
                    {
                        currentHuntingTarget = null;
                        return;
                    }
                }

                if (PlayGameScene.timming.TotalGameTime.TotalSeconds - prevFire.TotalSeconds > timeBetweenFire)
                {
                    if (!clipPlayer.inRange(25, 48))
                        clipPlayer.switchRange(25, 48);

                    if (currentHuntingTarget.GetType().Name.Equals("Tank"))
                    {
                        //((Tank)currentHuntingTarget).currentHitPoint -= damage;
                        Tank.currentHitPoint -= damage;
                    }
                    if (currentHuntingTarget.GetType().Name.Equals("SwimmingObject"))
                    {
                        ((SwimmingObject)currentHuntingTarget).health -= damage;
                    }
                    prevFire = PlayGameScene.timming.TotalGameTime;

                }
            }
        }
        // Go straight
        protected override void goStraight(SwimmingObject[] enemies, int enemiesAmount, SwimmingObject[] fishes, int fishAmount, Tank tank)
        {
            
            Vector3 futurePosition = Position + speed * headingDirection;
            if (Collision.isBarriersValidMove(this, futurePosition, enemies, enemiesAmount, tank)
                    && Collision.isBarriersValidMove(this, futurePosition, fishes, fishAmount, tank))
            {
                Position = futurePosition;
                BoundingSphere.Center = Position;
            }

        }
        public override void Draw(Matrix view, Matrix projection)
        {
            if (clipPlayer == null)
            {
                // just return for now.. Some of the fishes do not have animation, so clipPlayer won't be initialized for them
                base.Draw(view, projection);
                return;
            }

            bones = clipPlayer.GetSkinTransforms();

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(bones);
                    effect.View = view;
                    effect.Projection = projection;

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
