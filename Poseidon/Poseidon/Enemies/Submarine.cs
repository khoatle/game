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
    class Submarine : ShootingEnemy
    {
        double timePrevPowerUsed = 0;
        Random random;
        int powerupsType;

        int numHunterGenerated = 0;
        int numHunterGeneratedAtOnce = 2;
        GameMode gameMode;
        bool releasingHunter = false;
        double timeBetweenPowerUse = 15;

        public Submarine(GameMode gameMode)
            : base()
        {
            speed = (float)(GameConstants.EnemySpeed * 1.5);
            damage = GameConstants.TerminatorShootingDamage;
            timeBetweenFire = 2.0f;
            isBigBoss = true;
            random = new Random();
            health = 6000;
            //health = 1;
    
            //if (PlayGameScene.currentLevel > 10)
            //    perceptionRadius = GameConstants.BossPerceptionRadius * (HydroBot.gamePlusLevel + 1);
            //else
            perceptionRadius = GameConstants.BossPerceptionRadius;
            basicExperienceReward = 2000;

            if (PoseidonGame.gamePlus)
            {
                speed *= (1.0f + (float)HydroBot.gamePlusLevel / 4);
                damage *= (HydroBot.gamePlusLevel + 1);
                timeBetweenFire /= (1 + HydroBot.gamePlusLevel * 0.25f);
                health += (HydroBot.gamePlusLevel * 3000);
                basicExperienceReward *= (HydroBot.gamePlusLevel + 1);
                timeBetweenPowerUse /= (1 + (float)HydroBot.gamePlusLevel * 0.25f);
            }
            maxHealth = health;
            //basicExperienceReward = 1;
            this.gameMode = gameMode;
        }

        public override void Load(int clipStart, int clipEnd, int fps)
        {
            skd = Model.Tag as SkinningData;
            clipPlayer = new ClipPlayer(skd, fps);//ClipPlayer running at 24 frames/sec
            AnimationClip clip = skd.AnimationClips["Take 001"]; //Take name from the dude.fbx file
            clipPlayer.play(clip, clipStart, clipEnd, false);
            enemyMatrix = Matrix.CreateScale(0.4f) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                               Matrix.CreateTranslation(Position);
            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;
            scaledSphere.Radius *= 0.25f;
            BoundingSphere =
                new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);
            // Set up the parameters
            SetupShaderParameters(PoseidonGame.contentManager, Model);
        }

        public void Load()
        {
            Load(21, 30, 24);
        }

        public override void Update(SwimmingObject[] enemyList, ref int enemySize, SwimmingObject[] fishList, int fishSize, int changeDirection, HydroBot hydroBot, List<DamageBullet> enemyBullets, List<DamageBullet> alliesBullets, BoundingFrustum cameraFrustum, GameTime gameTime, GameMode gameMode)
        {
            EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);
            if (BoundingSphere.Intersects(cameraFrustum))
            {
                qRotation = Quaternion.CreateFromAxisAngle(
                                Vector3.Up,
                                ForwardDirection);
                enemyMatrix = Matrix.CreateScale(0.4f) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                                    Matrix.CreateFromQuaternion(qRotation) *
                                    Matrix.CreateTranslation(Position);
                clipPlayer.update(gameTime.ElapsedGameTime, true, enemyMatrix);
            }
            // do not delete this
            if (stunned) return;

            // Wear out slow
            if (speedFactor != 1)
                if (PoseidonGame.playTime.TotalSeconds - slowStart.TotalSeconds > slowDuration.TotalSeconds * HydroBot.turtlePower) 
                    speedFactor = 1;

            float buffFactor = HydroBot.maxHitPoint / GameConstants.PlayerStartingHP / 2.0f;
            buffFactor = MathHelper.Clamp(buffFactor, 1.0f, 2.0f);
            if (isHypnotise && PoseidonGame.playTime.TotalSeconds - startHypnotiseTime.TotalSeconds > GameConstants.timeHypnotiseLast * buffFactor * HydroBot.beltPower)
            {
                wearOutHypnotise();
            }

            if (!isHypnotise)
            {
                int perceptionID = perceptAndLock(hydroBot, fishList, fishSize);
                configAction(hydroBot, perceptionID, gameTime);
                makeAction(changeDirection, enemyList, ref enemySize, fishList, fishSize, enemyBullets, hydroBot, cameraFrustum, gameTime);
            }
            else
            {
                int perceptionID = perceptAndLock(hydroBot, enemyList, enemySize);
                configAction(hydroBot, perceptionID, gameTime);
                makeAction(changeDirection, enemyList, ref enemySize, fishList, fishSize, alliesBullets, hydroBot, cameraFrustum, gameTime);
            }
        }


        public void ReleaseHunter(BoundingFrustum cameraFrustum, SwimmingObject[] enemies, ref int enemyAmount, SwimmingObject[] fishes, int fishAmount, HydroBot hydroBot)
        {
            if (!clipPlayer.inRange(31, 40))
                clipPlayer.switchRange(31, 40);
            if (clipPlayer.donePlayingAnimation)
            {
                bool releaseOnRightSide;
                for (int i = 0; i < numHunterGeneratedAtOnce; i++)
                {
                    enemies[enemyAmount] = new ShootingEnemy();
                    enemies[enemyAmount].Name = "Shooting Enemy";
                    enemies[enemyAmount].LoadContent(PoseidonGame.contentManager, "Models/EnemyModels/diver_green_ly");
                    enemies[enemyAmount].releasedFromSubmarine = true;
                    ((BaseEnemy)enemies[enemyAmount]).Load(1, 25, 24);
                    if (i % 2 == 0) releaseOnRightSide = true;
                    else releaseOnRightSide = false;
                    if (PlaceHunter(enemies[enemyAmount], hydroBot, (BaseEnemy[])enemies, enemyAmount, (Fish[])fishes, fishAmount, releaseOnRightSide))
                    {
                        enemyAmount++;
                        numHunterGenerated++;
                    }
                    else
                    {
                        enemies[enemyAmount] = null;
                        break;
                    }
                }
                if (!clipPlayer.inRange(11, 20))
                    clipPlayer.switchRange(11, 20);
                releasingHunter = false;
            }

        }
        public bool PlaceHunter(SwimmingObject newHunter, HydroBot hydroBot, BaseEnemy[] enemies, int enemyAmount, Fish[] fishes, int fishAmount, bool releaseOnRightSide)
        {
            Random random = new Random();
            Vector3 movement = Vector3.Zero;
            movement.Z = 1;
            Matrix orientationMatrix = Matrix.CreateRotationY(ForwardDirection);
            Vector3 shootingDirection = Vector3.Transform(movement, orientationMatrix);
            Vector3 hunterPos;
            if (releaseOnRightSide)
                hunterPos = this.Position + AddingObjects.PerpendicularVector(shootingDirection) * 20;
            else 
                hunterPos = this.Position - AddingObjects.PerpendicularVector(shootingDirection) * 20;

            if (!(AddingObjects.IsSurfaceOccupied(new BoundingSphere(hunterPos, newHunter.BoundingSphere.Radius), enemyAmount, fishAmount, enemies, fishes)))
            {
                newHunter.Position = hunterPos;
                newHunter.ForwardDirection = this.ForwardDirection;
                ((BaseEnemy)newHunter).currentHuntingTarget = hydroBot;
                ((BaseEnemy)newHunter).startChasingTime = PoseidonGame.playTime;
                ((BaseEnemy)newHunter).giveupTime = new TimeSpan(0, 0, 3);
                newHunter.BoundingSphere =
                    new BoundingSphere(newHunter.Position, newHunter.BoundingSphere.Radius);
                return true;
            }
            else return false;
        }

        public void ShootTorpedos(List<DamageBullet> bullets, BoundingFrustum cameraFrustum)
        {
            AddingObjects.placeTorpedo(this, currentHuntingTarget, bullets, cameraFrustum, gameMode);
        }

        public void ShootLaser(List<DamageBullet> bullets, BoundingFrustum cameraFrustum)
        {
            AddingObjects.placeLaser(this, currentHuntingTarget, bullets, cameraFrustum, gameMode);
        }

        public void SpecialMove3(List<DamageBullet> bullets, BoundingFrustum cameraFrustum, GameTime gameTime)
        {
            if (currentHuntingTarget != null && PoseidonGame.playTime.TotalSeconds - prevFire.TotalSeconds > timeBetweenFire * 3)
            {
                AddingObjects.placeChasingBullet(this, currentHuntingTarget, bullets, cameraFrustum);
                prevFire = PoseidonGame.playTime;
            }
        }

        protected override void makeAction(int changeDirection, SwimmingObject[] enemies, ref int enemiesAmount, SwimmingObject[] fishes, int fishAmount, List<DamageBullet> bullets, HydroBot hydroBot, BoundingFrustum cameraFrustum, GameTime gameTime)
        {
            if (releasingHunter) ReleaseHunter(cameraFrustum, enemies, ref enemiesAmount, fishes, fishAmount, hydroBot);

            if (configBits[0] == true)
            {
                randomWalk(changeDirection, enemies, enemiesAmount, fishes, fishAmount, hydroBot, speedFactor);
                return;
            }
            if (currentHuntingTarget != null)
            {
                calculateHeadingDirection();
            }
            if (configBits[2] == true)
            {
                goStraight(enemies, enemiesAmount, fishes, fishAmount, hydroBot);
            }
            if (configBits[3] == true)
            {
                startChasingTime = PoseidonGame.playTime;

                if (currentHuntingTarget is BaseEnemy)
                {
                    BaseEnemy tmp = (BaseEnemy)currentHuntingTarget;
                    if (tmp.health <= 0)
                    {
                        currentHuntingTarget = null;
                        justBeingShot = false;
                        return;
                    }
                }

                if (currentHuntingTarget is Fish)
                {
                    Fish tmp = (Fish)currentHuntingTarget;
                    if (tmp.health <= 0)
                    {
                        currentHuntingTarget = null;
                        justBeingShot = false;
                    }
                }


                if (PoseidonGame.playTime.TotalSeconds - timePrevPowerUsed > timeBetweenPowerUse)
                {
                    bool powerUsed = false;
                    powerupsType = random.Next(2);
                    //only generate hunters while hunting hydrobot
                    if (powerupsType == 0 && numHunterGenerated < GameConstants.NumEnemiesInSubmarine
                        && currentHuntingTarget is HydroBot
                        && this.BoundingSphere.Intersects(cameraFrustum))
                    {
                        ReleaseHunter(cameraFrustum, enemies, ref enemiesAmount, fishes, fishAmount, hydroBot);
                        releasingHunter = true;
                        powerUsed = true;
                    }
                    else if (powerupsType == 1 && currentHuntingTarget is HydroBot)
                    {
                        ShootTorpedos(bullets, cameraFrustum);
                        powerUsed = true;
                    }

                    if (powerUsed) timePrevPowerUsed = PoseidonGame.playTime.TotalSeconds;
                }
                else if (PoseidonGame.playTime.TotalSeconds - prevFire.TotalSeconds > timeBetweenFire && (Position - currentHuntingTarget.Position).Length() < GameConstants.TerminatorShootingRange)
                {
                    //ChasingBullet(bullets, cameraFrustum, gameTime);
                    // AddingObjects.placeChasingBullet(this, currentHuntingTarget, bullets, cameraFrustum);
                    //AddingObjects.placeEnemyBullet(this, damage, bullets, 1, cameraFrustum, 20);
                    ShootLaser(bullets, cameraFrustum);
                    prevFire = PoseidonGame.playTime;
                }
            }
        }
    }
}
