using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon
{
    public class ShootingEnemy : BaseEnemy
    {

        // shortDistance is the distance the enemy will try to keep from the victim
        protected bool closeEnough;
        protected float shortDistance;

        public ShootingEnemy()
            : base()
        {
            perceptID = new int[] { 0, 1, 2, 3 };
            configBits = new bool[] { false, false, false, false };
            shortDistance = GameConstants.EnemyShootingDistance;// *(HydroBot.gamePlusLevel + 1);
            isHypnotise = false;
            timeBetweenFire = GameConstants.EnemyShootingRate;
            damage = GameConstants.EnemyShootingDamage;
            if (PoseidonGame.gamePlus)
            {
                timeBetweenFire /= (1 + HydroBot.gamePlusLevel * 0.25f);
                damage *= (HydroBot.gamePlusLevel + 1);
            }

        }

        // Return the perceptID correspondingly
        protected int perceptAndLock(HydroBot hydroBot, SwimmingObject[] enemyList, int enemySize)
        {
            if (!isHypnotise && Vector3.Distance(Position, hydroBot.Position) < perceptionRadius)
            {
                closeEnough = (Vector3.Distance(Position, hydroBot.Position) > shortDistance) ? false : true;
                currentHuntingTarget = hydroBot;
                return perceptID[1];
            }
            else
            {
                if (currentHuntingTarget != null
                        && Vector3.Distance(Position, currentHuntingTarget.Position) < perceptionRadius)
                {
                    closeEnough = (Vector3.Distance(Position, currentHuntingTarget.Position) > shortDistance) ? false : true;
                    return perceptID[2];
                }

                for (int i = 0; i < enemySize; i++)
                {
                    if (this != enemyList[i] && Vector3.Distance(Position, enemyList[i].Position) < perceptionRadius)
                    {
                        closeEnough = (Vector3.Distance(Position, enemyList[i].Position) > shortDistance) ? false : true;
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
                    configBits[3] = false;// true;
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
                if (closeEnough == true)
                {
                    configBits[0] = false;
                    configBits[1] = true;
                    configBits[2] = false;
                    configBits[3] = true;
                }
                else
                {
                    configBits[0] = false;
                    configBits[1] = false;
                    configBits[2] = true;
                    configBits[3] = true;
                }
            }
        }

        // Execute the actions
        protected virtual void makeAction(int changeDirection, SwimmingObject[] enemies, ref int enemiesAmount, SwimmingObject[] fishes, int fishAmount, List<DamageBullet> bullets, HydroBot hydroBot, BoundingFrustum cameraFrustum, GameTime gameTime)
        {
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

                if (currentHuntingTarget is Fish)
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

                // Divide speed factor make enemy attacks slower
                if (PoseidonGame.playTime.TotalSeconds - prevFire.TotalSeconds > timeBetweenFire / speedFactor && (Position - currentHuntingTarget.Position).Length() < GameConstants.EnemeyShootingRange)
                {
                    AddingObjects.placeEnemyBullet(this, damage, bullets, 0, cameraFrustum, 0);
                    prevFire = PoseidonGame.playTime;
                }
            }
        }

        public override void Update(SwimmingObject[] enemyList, ref int enemySize, SwimmingObject[] fishList, int fishSize, int changeDirection, HydroBot hydroBot, List<DamageBullet> enemyBullets, List<DamageBullet> alliesBullets, BoundingFrustum cameraFrustum, GameTime gameTime, GameMode gameMode)
        {
            EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);

            if (BoundingSphere.Intersects(cameraFrustum)) {
                qRotation = Quaternion.CreateFromAxisAngle(
                                Vector3.Up,
                                ForwardDirection);
                enemyMatrix = Matrix.CreateScale(0.1f) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                                    Matrix.CreateFromQuaternion(qRotation) *
                                    Matrix.CreateTranslation(Position);
                clipPlayer.update(gameTime.ElapsedGameTime, true, enemyMatrix);
            }
            // do not delete this
            if (stunned) return;

            // Fleeing stuff
            if (isFleeing == true)
            {
                if (PoseidonGame.playTime.TotalSeconds - fleeingStart.TotalSeconds < fleeingDuration.TotalSeconds * HydroBot.seaCowPower)
                {
                    flee(enemyList, enemySize, fishList, fishSize, hydroBot);
                    return;
                }
                else
                    isFleeing = false;
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
    }
}
