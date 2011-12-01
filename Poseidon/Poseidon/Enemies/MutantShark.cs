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
            damage = GameConstants.MutantSharkBitingDamage;
            isBigBoss = true;
            if (PlayGameScene.currentLevel == 3)
            {
                health = 6000;
                maxHealth = 6000;
            }
            else
            {
                health = 5000;
                maxHealth = 5000;
            }

            perceptionRadius = GameConstants.BossPerceptionRadius;
            basicExperienceReward = 750;
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

        /* scene: 1-playgamescene 2-shipwreckscene */
        public override void Update(SwimmingObject[] enemyList, int enemySize, SwimmingObject[] fishList, int fishSize, int changeDirection, HydroBot hydroBot, List<DamageBullet> enemyBullets, List<DamageBullet> alliesBullets, BoundingFrustum cameraFrustum, GameTime gameTime, GameMode gameMode)
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
            float buffFactor = HydroBot.maxHitPoint / GameConstants.PlayerStartingHP / 2.0f;
            buffFactor = MathHelper.Clamp(buffFactor, 1.0f, 2.0f);
            if (isHypnotise && PoseidonGame.playTime.TotalSeconds - startHypnotiseTime.TotalSeconds > GameConstants.timeHypnotiseLast * buffFactor)
            {
                wearOutHypnotise();
            }

            if (!isHypnotise)
            {
                int perceptionID = perceptAndLock(hydroBot, fishList, fishSize);
                configAction(hydroBot, perceptionID, gameTime);
                makeAction(changeDirection, enemyList, enemySize, fishList, fishSize, enemyBullets, hydroBot, cameraFrustum, gameTime, gameMode);
            }
            else
            {
                int perceptionID = perceptAndLock(hydroBot, enemyList, enemySize);
                configAction(hydroBot, perceptionID, gameTime);
                makeAction(changeDirection, enemyList, enemySize, fishList, fishSize, enemyBullets, hydroBot, cameraFrustum, gameTime, gameMode);
            }
        }
        // Execute the actions. ( scene: 1-playgamescene, 2-shipwreckscene)
        protected override void makeAction(int changeDirection, SwimmingObject[] enemies, int enemiesAmount, SwimmingObject[] fishes, int fishAmount, List<DamageBullet> bullets, HydroBot hydroBot, BoundingFrustum cameraFrustum, GameTime gameTime, GameMode gameMode)
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
                if (PoseidonGame.playTime.TotalSeconds - timeLastRoar > 10)
                    if (rand.Next(100) >= 95)
                    {
                        timeLastRoar = PoseidonGame.playTime.TotalSeconds;
                        Roar(gameMode);
                    }
            }
            if (configBits[3] == true)
            {
                startChasingTime = PoseidonGame.playTime;

                if (currentHuntingTarget.GetType().Name.Equals("Fish"))
                {
                    Fish tmp = (Fish)currentHuntingTarget;
                    if (tmp.health <= 0)
                    {
                        currentHuntingTarget = null;
                        return;
                    }
                }

                if (currentHuntingTarget is BaseEnemy)
                {
                    BaseEnemy tmp = (BaseEnemy)currentHuntingTarget;
                    if (tmp.health <= 0)
                    {
                        currentHuntingTarget = null;
                        return;
                    }
                }

                if (PoseidonGame.playTime.TotalSeconds - prevFire.TotalSeconds > timeBetweenFire)
                {
                    //if attack and swim both at the same time or not
                    //just use attacking anim
                    if (!clipPlayer.inRange(30, 53))
                        clipPlayer.switchRange(30, 53);
                    //if (!clipPlayer.inRange(60, 83))
                    //    clipPlayer.switchRange(60, 83);

                    if (currentHuntingTarget.GetType().Name.Equals("HydroBot"))
                    {
                        if (!(HydroBot.invincibleMode || HydroBot.supersonicMode) ){
                            HydroBot.currentHitPoint -= damage;

                            PoseidonGame.audio.botYell.Play();

                            HydroBot.isPoissoned = true;
                            HydroBot.accumulatedHealthLossFromPoisson = 0;

                            PoseidonGame.audio.botYell.Play();

                            Point point = new Point();
                            String point_string = "-" + damage.ToString() + "HP (POISONED)";
                            point.LoadContent(PoseidonGame.contentManager, point_string, hydroBot.Position, Color.Black);
                            if (gameMode == GameMode.ShipWreck)
                                ShipWreckScene.points.Add(point);
                            else if (gameMode == GameMode.MainGame)
                                PlayGameScene.points.Add(point);
                            else if (gameMode == GameMode.SurvivalMode)
                                SurvivalGameScene.points.Add(point);
                        }
                    }
                    //if (currentHuntingTarget.GetType().Name.Equals("Fish"))
                    //{
                    //    ((Fish)currentHuntingTarget).health -= damage;
                    //    if (currentHuntingTarget.BoundingSphere.Intersects(cameraFrustum))
                    //    {
                    //        PoseidonGame.audio.animalYell.Play();
                    //    }
                    //}
                    if (currentHuntingTarget is SwimmingObject)
                    {
                        ((SwimmingObject)currentHuntingTarget).health -= damage;
                        ((SwimmingObject)currentHuntingTarget).accumulatedHealthLossFromPoison = 0;
                        ((SwimmingObject)currentHuntingTarget).isPoissoned = true;
                        
                        if (currentHuntingTarget.BoundingSphere.Intersects(cameraFrustum))
                        {
                            Point point = new Point();
                            String point_string = "-" + damage.ToString() + "HP (POISONED)";
                            point.LoadContent(PoseidonGame.contentManager, point_string, currentHuntingTarget.Position, Color.Red);
                            if (gameMode == GameMode.ShipWreck)
                                ShipWreckScene.points.Add(point);
                            else if (gameMode == GameMode.MainGame)
                                PlayGameScene.points.Add(point);
                            else if (gameMode == GameMode.SurvivalMode)
                                SurvivalGameScene.points.Add(point);
                            if (currentHuntingTarget is Fish)
                                PoseidonGame.audio.animalYell.Play();
                        }
                    }
                    prevFire = PoseidonGame.playTime;

                    if (this.BoundingSphere.Intersects(cameraFrustum))
                        PoseidonGame.audio.biteSound.Play();
                }
            }
        }

        protected void Roar(GameMode gameMode)
        {
            PoseidonGame.audio.roarSound.Play();
            if (gameMode == GameMode.MainGame)
                PlayGameScene.gameCamera.Shake(10f, 1.9f);
            else if (gameMode == GameMode.SurvivalMode)
                SurvivalGameScene.gameCamera.Shake(10f, 1.9f);
        }
    }
}
