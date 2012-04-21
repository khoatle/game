using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon
{
    public class CombatEnemy : BaseEnemy
    {   
        private BoundingSphere futureBoundingSphere;

        // Percept ID:
        // 0 = nothing detected
        // 1 = hydrobot detected - 1st priory
        // 2 = last target is still in range - 2nd priory
        // 3 = a fish is detected - 3rd priory

        // Move bits:
        // 1st bit: randomWalk
        // 2nd bit: standstill
        // 3rd bit: chasing
        // 4th bit: is striking
        public CombatEnemy()
            : base()
        {
            perceptID = new int[] { 0, 1, 2, 3 };
            configBits = new bool[] { false, false, false, false };

            timeBetweenFire = GameConstants.EnemyShootingRate;
            damage = GameConstants.CombatEnemyDamage;
            if (PoseidonGame.gamePlus)
            {
                timeBetweenFire /= (1 + (float)HydroBot.gamePlusLevel * 0.25f);
                damage *= (HydroBot.gamePlusLevel + 1);
            }

            //maxHealth *= (HydroBot.gamePlusLevel + 1);
            //health = maxHealth;
            perceptionRadius *= 2;
            isHypnotise = false;
        }
   
        public override void Update(SwimmingObject[] enemyList, ref int enemySize, SwimmingObject[] fishList, int fishSize, int changeDirection, HydroBot hydroBot, List<DamageBullet> enemyBullets, List<DamageBullet> alliesBullets, BoundingFrustum cameraFrustum, GameTime gameTime, GameMode gameMode)
        {
            EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);
            if (BoundingSphere.Intersects(cameraFrustum))
                qRotation = Quaternion.CreateFromAxisAngle(
                                Vector3.Up,
                                ForwardDirection);
                enemyMatrix = Matrix.CreateScale(0.1f) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                                    Matrix.CreateFromQuaternion(qRotation) *
                                    Matrix.CreateTranslation(Position);
                clipPlayer.update(gameTime.ElapsedGameTime, true, enemyMatrix);


            // Fleeing stuff
            if (isFleeing == true) {
                if (PoseidonGame.playTime.TotalSeconds - fleeingStart.TotalSeconds < fleeingDuration.TotalSeconds * HydroBot.seaCowPower)
                {
                    flee(enemyList, enemySize, fishList, fishSize, hydroBot);
                    return;
                }
                else {
                    isFleeing = false;
                }
                if (!clipPlayer.inRange(1, 30))
                    clipPlayer.switchRange(1, 30);
            }
            if (isPoissoned == true)
            {
                if (accumulatedHealthLossFromPoison < maxHPLossFromPoisson)
                {
                    health -= 0.1f;
                    accumulatedHealthLossFromPoison += 0.1f;
                }
                else
                {
                    isPoissoned = false;
                    accumulatedHealthLossFromPoison = 0;
                }
            }
            // Wear out slow
            if (speedFactor != 1)
                if (PoseidonGame.playTime.TotalSeconds - slowStart.TotalSeconds > slowDuration.TotalSeconds * HydroBot.turtlePower)
                    speedFactor = 1;
            
            // Stun stuff
            if (stunned)
            {
                if (!clipPlayer.inRange(1, 30))
                    clipPlayer.switchRange(1, 30);
                return;
            }

            float buffFactor = HydroBot.maxHitPoint / GameConstants.PlayerStartingHP / 2.0f * HydroBot.beltPower;
            buffFactor = MathHelper.Clamp(buffFactor, 1.0f, 1.6f);
            if (isHypnotise && PoseidonGame.playTime.TotalSeconds - startHypnotiseTime.TotalSeconds > GameConstants.timeHypnotiseLast * buffFactor)
            {
                wearOutHypnotise();
            }

            if (!isHypnotise && !isFleeing && !stunned)
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

        protected int perceptAndLock(HydroBot hydroBot, SwimmingObject[] enemyList, int enemySize)
        {
            if (!isHypnotise && Vector3.Distance(Position, hydroBot.Position) < perceptionRadius)
            {
                currentHuntingTarget = hydroBot;
                return perceptID[1];
            }
            else
            {
                if (currentHuntingTarget != null
                        && Vector3.Distance(Position, currentHuntingTarget.Position) < perceptionRadius) {
                    return perceptID[2];
                }

                for (int i = 0; i < enemySize; i++)
                {
                    if (this != enemyList[i] && Vector3.Distance(Position, enemyList[i].Position) < perceptionRadius) {
                        currentHuntingTarget = enemyList[i];
                        return perceptID[3];
                    }
                }
                return perceptID[0];
            }
        }

        protected void configAction(HydroBot bot, int perception, GameTime gameTime)
        {
            if (justBeingShot == true) {
                configBits[0] = false;
                configBits[1] = false;
                configBits[2] = true;
                configBits[3] = false;
                currentHuntingTarget = bot;
            }

            if (perception == perceptID[0])
            {
                if (currentHuntingTarget != null && clearMind(gameTime) == false)
                {
                    configBits[0] = false;
                    configBits[1] = false;
                    configBits[2] = true;
                    configBits[3] = false;
                }
                else
                {
                    configBits[0] = true;
                    configBits[1] = false;
                    configBits[2] = false;
                    configBits[3] = false;
                }
            }
            else if (perception == perceptID[1] || perception == perceptID[2] || perception == perceptID[3])
            {
                configBits[0] = false;
                configBits[2] = true;

                calculateFutureBoundingSphere();

                configBits[3] = (Vector3.Distance(Position, currentHuntingTarget.Position) < BoundingSphere.Radius + currentHuntingTarget.BoundingSphere.Radius + 3f) ? true : false;
                configBits[1] = configBits[3];
            }
        }

        protected void calculateFutureBoundingSphere()
        {
            Vector3 futurePosition = Position + speed * headingDirection * speedFactor;
            futureBoundingSphere = new BoundingSphere(futurePosition, BoundingSphere.Radius);
        }

        // Execute the actions .. scene -> 1=playgamescene, 2=shipwreckscene
        protected virtual void makeAction(int changeDirection, SwimmingObject[] enemies, int enemiesAmount, SwimmingObject[] fishes, int fishAmount, List<DamageBullet> bullets, HydroBot hydroBot, BoundingFrustum cameraFrustum, GameTime gameTime, GameMode gameMode)
        {
            if (configBits[0] == true)
            {
                // swimming w/o attacking
                if (!clipPlayer.inRange(1, 30) && !configBits[3])
                    clipPlayer.switchRange(1, 30);
                randomWalk(changeDirection, enemies, enemiesAmount, fishes, fishAmount, hydroBot, speedFactor);
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
                if (!clipPlayer.inRange(1, 30) && !configBits[3])
                    clipPlayer.switchRange(1, 30);
                goStraight(enemies, enemiesAmount, fishes, fishAmount, hydroBot);
            }
            if (configBits[3] == true)
            {
                startChasingTime = PoseidonGame.playTime;

                if (currentHuntingTarget is Fish)
                {
                    Fish tmp = (Fish)currentHuntingTarget;
                    if (tmp.health <= 0)
                    {
                        currentHuntingTarget = null;
                        justBeingShot = false;
                        return;
                    }
                }

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

                if (PoseidonGame.playTime.TotalSeconds - prevFire.TotalSeconds > timeBetweenFire / speedFactor)
                {
                    if (!clipPlayer.inRange(31, 60))
                        clipPlayer.switchRange(31, 60);
                    if (currentHuntingTarget.GetType().Name.Equals("HydroBot"))
                    {
                        if (!(HydroBot.invincibleMode || HydroBot.supersonicMode))
                        {
                            HydroBot.currentHitPoint -= damage;
                            
                            Point point = new Point();
                            String point_string = "-" + damage.ToString() + "HP";
                            point.LoadContent(PoseidonGame.contentManager, point_string, hydroBot.Position, Color.Red);
                            if (gameMode == GameMode.ShipWreck)
                                ShipWreckScene.points.Add(point);
                            else if (gameMode == GameMode.MainGame)
                                PlayGameScene.points.Add(point);
                            else if (gameMode == GameMode.SurvivalMode)
                                SurvivalGameScene.points.Add(point);

                            PoseidonGame.audio.botYell.Play();
                            PlayGameScene.healthLost += damage;
                        }
                        if (HydroBot.autoHipnotizeMode)
                        {
                            setHypnotise();
                        }
                        if (HydroBot.autoExplodeMode)
                        {
                            PoseidonGame.audio.Explo1.Play();
                            if (gameMode == GameMode.MainGame)
                                PlayGameScene.gameCamera.Shake(12.5f, .2f);
                            else if (gameMode == GameMode.ShipWreck)
                                ShipWreckScene.gameCamera.Shake(12.5f, .2f);
                            else if (gameMode == GameMode.SurvivalMode)
                                SurvivalGameScene.gameCamera.Shake(12.5f, .2f);

                            CastSkill.UseThorHammer(hydroBot.Position, hydroBot.MaxRangeX, hydroBot.MaxRangeZ, (BaseEnemy[])enemies, ref enemiesAmount, fishes, fishAmount, HydroBot.gameMode, true);
                        }
                    }
                    if (currentHuntingTarget is SwimmingObject)
                    {
                        ((SwimmingObject)currentHuntingTarget).health -= damage;
                        if (currentHuntingTarget.BoundingSphere.Intersects(cameraFrustum))
                        {
                            if (currentHuntingTarget is Fish)
                                PoseidonGame.audio.animalYell.Play();
                            Point point = new Point();
                            String point_string = "-" + damage.ToString() + "HP";
                            point.LoadContent(PoseidonGame.contentManager, point_string, currentHuntingTarget.Position, Color.Red);
                            if (gameMode == GameMode.ShipWreck)
                                ShipWreckScene.points.Add(point);
                            else if (gameMode == GameMode.MainGame)
                                PlayGameScene.points.Add(point);
                            else if (gameMode == GameMode.SurvivalMode)
                                SurvivalGameScene.points.Add(point);
                        }
                    }
                    prevFire = PoseidonGame.playTime;

                    if (this.BoundingSphere.Intersects(cameraFrustum))
                        PoseidonGame.audio.slashSound.Play();
                }
            }
        }
    }
}