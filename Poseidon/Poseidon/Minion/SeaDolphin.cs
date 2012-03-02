using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon
{
    public class SeaDolphin : Fish
    {
        public static float AfterX = -HydroBot.controlRadius/4;
        public static float AfterZ = 0;

        public TimeSpan lastCast;
        public TimeSpan coolDown;

        public TimeSpan startCasting;
        public TimeSpan standingTime;

        public TimeSpan lastAttack;
        public TimeSpan timeBetweenAttack; 

        public static float healAmount = 50f;

        public static float dolphinDamage = 10f;

        public SeaDolphin() : base() {
            lastCast = PoseidonGame.playTime;
            lastAttack = PoseidonGame.playTime;
            // Cool down 1s
            timeBetweenAttack = new TimeSpan(0,0,1);
            // Cool down 5s
            coolDown = new TimeSpan(0, 0, 5);
            standingTime = new TimeSpan(0, 0, 2);
            isBigBoss = false;

            maxHealth = GameConstants.DolphinStartingHealth;
            health = maxHealth;
        }

        public bool isTargeting() {
            return currentTarget == null || currentTarget.health <= 0;
        }

        public override void attack()
        {
            float damage = dolphinDamage * HydroBot.dolphinPower;
            if (PoseidonGame.playTime.TotalSeconds - lastAttack.TotalSeconds > timeBetweenAttack.TotalSeconds)
            {
                currentTarget.health -= damage;

                lastAttack = PoseidonGame.playTime;

                Vector3 facingDirection = currentTarget.Position - Position;
                ForwardDirection = (float)Math.Atan2(facingDirection.X, facingDirection.Z);

                Point point = new Point();
                String point_string = "Enemy Got Biten, health - " + damage;
                point.LoadContent(PoseidonGame.contentManager, point_string, currentTarget.Position, Color.Red);
                if (HydroBot.gameMode == GameMode.ShipWreck)
                    ShipWreckScene.points.Add(point);
                else if (HydroBot.gameMode == GameMode.MainGame)
                    PlayGameScene.points.Add(point);
                else if (HydroBot.gameMode == GameMode.SurvivalMode)
                    SurvivalGameScene.points.Add(point);
            }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, SwimmingObject[] enemies, int enemiesSize, SwimmingObject[] fish, int fishSize, int changeDirection, HydroBot hydroBot, List<DamageBullet> enemyBullet)
        {
            EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);

            float lastMaxHealth = maxHealth;
            maxHealth = GameConstants.DolphinStartingHealth * HydroBot.dolphinPower;
            health += (maxHealth - lastMaxHealth);

            // Only cast skill when there is an enemy near by
            if (!isReturnBot && !isCasting)
            {
                // It is OK to cast?
                if (PoseidonGame.playTime.TotalSeconds - lastCast.TotalSeconds > coolDown.TotalSeconds && HydroBot.currentHitPoint < HydroBot.maxHitPoint) {
                    isCasting = true;
                    isReturnBot = false;
                    isWandering = false;
                    isChasing = false;
                    isFighting = false;

                    HydroBot.isBeingHealed = true;

                    // Start casting
                    startCasting = PoseidonGame.playTime;

                    healAmount *= HydroBot.dolphinPower;

                    HydroBot.currentHitPoint += healAmount;
                    HydroBot.currentHitPoint = Math.Min(HydroBot.maxHitPoint, HydroBot.currentHitPoint);

                    PoseidonGame.audio.healingSound.Play();

                    Point point = new Point();
                    String point_string = "Health + " + healAmount;
                    point.LoadContent(PoseidonGame.contentManager, point_string, hydroBot.Position, Color.LawnGreen);
                    if (HydroBot.gameMode == GameMode.ShipWreck)
                        ShipWreckScene.points.Add(point);
                    else if (HydroBot.gameMode == GameMode.MainGame)
                        PlayGameScene.points.Add(point);
                    else if (HydroBot.gameMode == GameMode.SurvivalMode)
                        SurvivalGameScene.points.Add(point);
                }
            }

            // OK, I'm casting it
            if (isCasting == true)
            {
                // Casting timeout
                if (PoseidonGame.playTime.TotalSeconds - startCasting.TotalSeconds > standingTime.TotalSeconds)
                {
                    isCasting = false; // Done casting
                    isWandering = true; // Let the wander state do the wandering task

                    //this got reset in HydroBot.cs too
                    //HydroBot.isBeingHealed = false;

                    lastCast = PoseidonGame.playTime;
                } // Else do nothing (all other states are false). Later effect can be added here
                else {
                    Vector3 facingDirection = hydroBot.Position - Position;
                    ForwardDirection = (float)Math.Atan2(facingDirection.X, facingDirection.Z);
                }
            }

            Vector3 destination = hydroBot.Position + new Vector3(AfterX, 0, AfterZ);
            if (isWandering == true)
            {
                // If the fish is far from the point after the bot's back or is the bot moving
                if (Vector3.Distance(hydroBot.Position, Position) > HydroBot.controlRadius || hydroBot.isMoving())
                {
                    isWandering = false;
                    isReturnBot = true;
                    isChasing = false;
                    isFighting = false;

                    seekDestination(destination, enemies, enemiesSize, fish, fishSize, hydroBot, speedFactor);
                }
                else
                {
                    currentTarget = lookForEnemy(enemies, enemiesSize);
                    if (currentTarget == null) // See no enemy
                        randomWalk(changeDirection, enemies, enemiesSize, fish, fishSize, hydroBot);
                    else
                    { // Hunt him down
                        isWandering = false;
                        isReturnBot = false;
                        isChasing = true;
                        isFighting = false;

                        seekDestination(currentTarget.Position, enemies, enemiesSize, fish, fishSize, hydroBot, speedFactor);
                    }
                }
            }// End wandering

            // If return is the current state
            else if (isReturnBot == true)
            {
                // If the fish is near the point after the bot's back, wander
                if (Vector3.Distance(destination, Position) < HydroBot.controlRadius * 0.5 &&
                    Vector3.Distance(hydroBot.Position, Position) < HydroBot.controlRadius)
                {
                    isWandering = true;
                    isReturnBot = false;
                    isChasing = false;
                    isFighting = false;

                    randomWalk(changeDirection, enemies, enemiesSize, fish, fishSize, hydroBot);
                }
                else
                    seekDestination(destination, enemies, enemiesSize, fish, fishSize, hydroBot, speedFactor);
            }
            // If the fish is chasing some enemy
            else if (isChasing == true)
            {
                if (Vector3.Distance(hydroBot.Position, Position) > HydroBot.controlRadius)
                {  // If too far, return to bot
                    isWandering = false;
                    isReturnBot = true;
                    isChasing = false;
                    isFighting = false;

                    currentTarget = null;

                    seekDestination(destination, enemies, enemiesSize, fish, fishSize, hydroBot, speedFactor);
                }
                else if (Vector3.Distance(Position, currentTarget.Position) <
                    10f + BoundingSphere.Radius + currentTarget.BoundingSphere.Radius) // Too close then attack
                {
                    isWandering = false;
                    isReturnBot = false;
                    isChasing = false;
                    isFighting = true;

                    attack();
                }
                else
                    seekDestination(currentTarget.Position, enemies, enemiesSize, fish, fishSize, hydroBot, speedFactor);
            }
            // If fighting some enemy
            else if (isFighting == true)
            {
                // Enemy is down, return to bot
                if (currentTarget == null || currentTarget.health <= 0)
                {
                    isWandering = false;
                    isReturnBot = true;
                    isChasing = false;
                    isFighting = false;

                    currentTarget = null;

                    seekDestination(destination, enemies, enemiesSize, fish, fishSize, hydroBot, speedFactor);
                }
                else
                {
                    // If the enemy ran away and the fish is not too far from bot

                    if (Vector3.Distance(hydroBot.Position, Position) >= HydroBot.controlRadius)
                    {
                        isWandering = false;
                        isReturnBot = true;
                        isChasing = false;
                        isFighting = false;

                        currentTarget = null;

                        seekDestination(destination, enemies, enemiesSize, fish, fishSize, hydroBot, speedFactor);
                    }
                    else if (Vector3.Distance(Position, currentTarget.Position) >=
                      5f + BoundingSphere.Radius + currentTarget.BoundingSphere.Radius)
                    {
                        isWandering = false;
                        isReturnBot = false;
                        isChasing = true;
                        isFighting = false;

                        seekDestination(currentTarget.Position, enemies, enemiesSize, fish, fishSize, hydroBot, speedFactor);
                    }
                    else
                        attack();
                }
            }

            // if clip player has been initialized, update it
            if (clipPlayer != null)
            {
                qRotation = Quaternion.CreateFromAxisAngle(
                                Vector3.Up,
                                ForwardDirection);
                float scale = 1f;
                if (isBigBoss) scale *= 2.0f;
                fishMatrix = Matrix.CreateScale(scale) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                                    Matrix.CreateFromQuaternion(qRotation) *
                                    Matrix.CreateTranslation(Position);
                clipPlayer.update(gameTime.ElapsedGameTime, true, fishMatrix);
            }
        }
    }
}
