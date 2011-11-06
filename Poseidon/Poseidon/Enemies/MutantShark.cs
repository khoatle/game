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
        protected double timeLastRoar = 0; 
        Random rand = new Random();
        public MutantShark() : base() {
            speed = (float)(GameConstants.EnemySpeed * 1.5);
            damage = GameConstants.DefaultEnemyDamage * 5;
            isBigBoss = true;
            health = 4000;
            maxHealth = 4000;
            perceptionRadius = GameConstants.BossPerceptionRadius;
            experienceReward = 600;
        }

        public override void Load(int clipStart, int clipEnd, int fpsRate)
        {
            skd = Model.Tag as SkinningData;
            clipPlayer = new ClipPlayer(skd, fpsRate);//ClipPlayer running at 24 frames/sec
            AnimationClip clip = skd.AnimationClips["Take 001"]; //Take name from the dude.fbx file
            clipPlayer.play(clip, clipStart, clipEnd, true);
            enemyMatrix = Matrix.CreateScale(1.0f) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                               Matrix.CreateTranslation(Position);
            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;
            scaledSphere.Radius *= 0.6f;
            BoundingSphere =
                new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);
        }

        public override void LoadContent(ContentManager content, string modelName)
        {
            Model = content.Load<Model>(modelName);
            BarrierType = modelName;
            Position = Vector3.Down;
            BoundingSphere = CalculateBoundingSphere();
            this.Load(1, 24, 24);
        }

        public override void ChangeBoundingSphere()
        {
            //BoundingSphere.Center += new Vector3(20,0,0);
        }
        public override void Update(SwimmingObject[] enemyList, int enemySize, SwimmingObject[] fishList, int fishSize, int changeDirection, HydroBot hydroBot, List<DamageBullet> enemyBullets, List<DamageBullet> alliesBullets, BoundingFrustum cameraFrustum, GameTime gameTime)
        {
            // if clip player has been initialized, update it
            if (clipPlayer != null)
            {
                qRotation = Quaternion.CreateFromAxisAngle(
                                Vector3.Up,
                                ForwardDirection);
                float scale = 1.0f;

                enemyMatrix = Matrix.CreateScale(scale) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                                    Matrix.CreateFromQuaternion(qRotation) *
                                    Matrix.CreateTranslation(Position);
                clipPlayer.update(gameTime.ElapsedGameTime, true, enemyMatrix);
            }
            //if stunned, switch to idle anim
            //for mutant shark, idle = swimming normally
            if (stunned)
            {
                if (!clipPlayer.inRange(1, 24))
                    clipPlayer.switchRange(1, 24);
                return;
            }
            if (isHypnotise && gameTime.TotalGameTime.TotalSeconds - startHypnotiseTime.TotalSeconds > GameConstants.timeHypnotiseLast)
            {
                wearOutHypnotise();
            }

            if (!isHypnotise)
            {
                int perceptionID = perceptAndLock(hydroBot, fishList, fishSize);
                configAction(perceptionID, gameTime);
                makeAction(changeDirection, enemyList, enemySize, fishList, fishSize, enemyBullets, hydroBot, cameraFrustum, gameTime);
            }
            else
            {
                int perceptionID = perceptAndLock(hydroBot, enemyList, enemySize);
                configAction(perceptionID, gameTime);
                makeAction(changeDirection, enemyList, enemySize, fishList, fishSize, enemyBullets, hydroBot, cameraFrustum, gameTime);
            }
        }
        // Execute the actions
        protected override void makeAction(int changeDirection, SwimmingObject[] enemies, int enemiesAmount, SwimmingObject[] fishes, int fishAmount, List<DamageBullet> bullets, HydroBot hydroBot, BoundingFrustum cameraFrustum, GameTime gameTime)
        {
            if (configBits[0] == true)
            {
                // swimming w/o attacking
                if (!clipPlayer.inRange(1, 24) && !configBits[3])
                    clipPlayer.switchRange(1, 24);
                randomWalk(changeDirection, enemies, enemiesAmount, fishes, fishAmount, hydroBot);
                return;
            }
            if (currentHuntingTarget != null)
            {
                calculateHeadingDirection();
                calculateFutureBoundingSphere();
            }
            if (configBits[2] == true)
            {
                // swimming w/o attacking
                if (!clipPlayer.inRange(1, 24) && !configBits[3])
                    clipPlayer.switchRange(1, 24);
                goStraight(enemies, enemiesAmount, fishes, fishAmount, hydroBot);
            }
            if (!configBits[3] && this.BoundingSphere.Intersects(cameraFrustum))
            {
                if (gameTime.TotalGameTime.TotalSeconds - timeLastRoar > 10)
                    if (rand.Next(100) >= 95)
                    {
                        timeLastRoar = gameTime.TotalGameTime.TotalSeconds;
                        Roar();
                    }
            }
            if (configBits[3] == true)
            {
                startChasingTime = gameTime.TotalGameTime;

                if (currentHuntingTarget.GetType().Name.Equals("Fish"))
                {
                    Fish tmp = (Fish)currentHuntingTarget;
                    if (tmp.health <= 0)
                    {
                        currentHuntingTarget = null;
                        return;
                    }
                }

                if (gameTime.TotalGameTime.TotalSeconds - prevFire.TotalSeconds > timeBetweenFire)
                {
                    //if attack and swim both at the same time or not
                    //just use attacking anim
                    if (!clipPlayer.inRange(30, 53))
                        clipPlayer.switchRange(30, 53);
                    //if (!clipPlayer.inRange(60, 83))
                    //    clipPlayer.switchRange(60, 83);

                    if (currentHuntingTarget.GetType().Name.Equals("HydroBot"))
                    {
                        if (!HydroBot.invincibleMode)
                        {
                            HydroBot.currentHitPoint -= damage;
                            PoseidonGame.audio.botYell.Play();
                        }
                    }
                    if (currentHuntingTarget.GetType().Name.Equals("Fish"))
                    {
                        ((Fish)currentHuntingTarget).health -= damage;
                        if (currentHuntingTarget.BoundingSphere.Intersects(cameraFrustum))
                        {
                            PoseidonGame.audio.animalYell.Play();
                        }
                    }
                    prevFire = gameTime.TotalGameTime;

                    if (this.BoundingSphere.Intersects(cameraFrustum))
                        PoseidonGame.audio.biteSound.Play();
                }
            }
        }

        protected void Roar()
        {
            PoseidonGame.audio.roarSound.Play();
            PlayGameScene.gameCamera.Shake(10f, 1.9f);
        }
    }
}
