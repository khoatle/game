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
    public class SeaCow : Fish
    {
        public static float AfterX = HydroBot.controlRadius / 4;
        public static float AfterZ = HydroBot.controlRadius * 1.73f / 4;
        public static float roarRadius = 50f;

        public static float cowDamage = 20f;
        public TimeSpan lastAttack;
        public TimeSpan timeBetweenAttack;

        public TimeSpan startCasting;
        public TimeSpan standingTime;

        public TimeSpan lastCast;
        public TimeSpan coolDown;

        public static float seaCowDamage = 20f;

        Camera gameCamera;

        public SeaCow() : base() {
            lastAttack = PoseidonGame.playTime;
            lastCast = PoseidonGame.playTime;
            timeBetweenAttack = new TimeSpan(0, 0, 1);
            // Cool down 20s
            coolDown = new TimeSpan(0, 0, 5);

            standingTime = new TimeSpan(0, 0, 1);
            isBigBoss = false;

            maxHealth = GameConstants.SeaCowStartingHealth;
            health = maxHealth;

            if (HydroBot.gameMode == GameMode.ShipWreck)
                gameCamera = ShipWreckScene.gameCamera;
            else if (HydroBot.gameMode == GameMode.MainGame)
                gameCamera = PlayGameScene.gameCamera;
            else if (HydroBot.gameMode == GameMode.SurvivalMode)
                gameCamera = SurvivalGameScene.gameCamera;
            speedFactor = 1 + (HydroBot.seaCowPower - 1) / 4;
        }

        public override void attack()
        {
            float damage = seaCowDamage * HydroBot.seaCowPower;
            if (PoseidonGame.playTime.TotalSeconds - lastAttack.TotalSeconds > timeBetweenAttack.TotalSeconds)
            {
                currentTarget.health -= damage;

                lastAttack = PoseidonGame.playTime;

                Vector3 facingDirection = currentTarget.Position - Position;
                ForwardDirection = (float)Math.Atan2(facingDirection.X, facingDirection.Z);

                Point point = new Point();
                String point_string = "-" + (int)damage + "HP";
                point.LoadContent(PoseidonGame.contentManager, point_string, currentTarget.Position, Color.Red);
                if (HydroBot.gameMode == GameMode.ShipWreck)
                    ShipWreckScene.points.Add(point);
                else if (HydroBot.gameMode == GameMode.MainGame)
                    PlayGameScene.points.Add(point);
                else if (HydroBot.gameMode == GameMode.SurvivalMode)
                    SurvivalGameScene.points.Add(point);


                //if (this.BoundingSphere.Intersects(cameraFrustum))
                    PoseidonGame.audio.slashSound.Play();
            }
            base.attack();
        }

        public void giveBuff() {
            Random rnd = new Random();
            int type = rnd.Next(3);
            // strength up
            if (type == 0)
            {
                HydroBot.strengthUpStartTime = PoseidonGame.playTime.TotalSeconds;
                HydroBot.strengthUp = 2f;

                Point point = new Point();
                String point_string = "Strength X 2\ntemporarily";
                point.LoadContent(PoseidonGame.contentManager, point_string, Position, Color.LawnGreen);
                if (HydroBot.gameMode == GameMode.ShipWreck)
                    ShipWreckScene.points.Add(point);
                else if (HydroBot.gameMode == GameMode.MainGame)
                    PlayGameScene.points.Add(point);
                else if (HydroBot.gameMode == GameMode.SurvivalMode)
                    SurvivalGameScene.points.Add(point);

            } // Speed up
            else if (type == 1)
            {
                HydroBot.speedUpStartTime = PoseidonGame.playTime.TotalSeconds;
                HydroBot.speedUp = 2f;

                Point point = new Point();
                String point_string = "Speed X 2\ntemporarily";
                point.LoadContent(PoseidonGame.contentManager, point_string, Position, Color.LawnGreen);
                if (HydroBot.gameMode == GameMode.ShipWreck)
                    ShipWreckScene.points.Add(point);
                else if (HydroBot.gameMode == GameMode.MainGame)
                    PlayGameScene.points.Add(point);
                else if (HydroBot.gameMode == GameMode.SurvivalMode)
                    SurvivalGameScene.points.Add(point);
            } // Rate of fire up
            else
            {
                HydroBot.fireRateUpStartTime = PoseidonGame.playTime.TotalSeconds;
                HydroBot.fireRateUp = 2f;

                Point point = new Point();
                String point_string = "Fire rate X 2\ntemporarily"; ;
                point.LoadContent(PoseidonGame.contentManager, point_string, Position, Color.LawnGreen);
                if (HydroBot.gameMode == GameMode.ShipWreck)
                    ShipWreckScene.points.Add(point);
                else if (HydroBot.gameMode == GameMode.MainGame)
                    PlayGameScene.points.Add(point);
                else if (HydroBot.gameMode == GameMode.SurvivalMode)
                    SurvivalGameScene.points.Add(point);
            }
        }

        float lastPower = 1;
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, BoundingFrustum cameraFrustum, SwimmingObject[] enemies, int enemiesSize, SwimmingObject[] fish, int fishSize, int changeDirection, HydroBot tank, List<DamageBullet> enemyBullet)
        {
            EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);

            if (lastPower != HydroBot.seaCowPower)
            {
                float lastMaxHealth = maxHealth;
                maxHealth = GameConstants.SeaCowStartingHealth * HydroBot.seaCowPower;
                health += (maxHealth - lastMaxHealth);
                speedFactor = 1 + (HydroBot.seaCowPower - 1) / 4;
                lastPower = HydroBot.seaCowPower;
            }

            BaseEnemy potentialEnemy = lookForEnemy(enemies, enemiesSize);

            if (!isReturnBot && !isCasting && potentialEnemy != null) { 
                // It is OK to cast?
                if (PoseidonGame.playTime.TotalSeconds - lastCast.TotalSeconds > coolDown.TotalSeconds)// && HydroBot.currentHitPoint < HydroBot.maxHitPoint)
                {
                    isCasting = true;
                    isReturnBot = false;
                    isWandering = false;
                    isChasing = false;
                    isFighting = false;

                    startCasting = PoseidonGame.playTime;

                    //tell graphic effects to play ripple effect
                    HydroBot.ripplingScreen = true;
                    Viewport viewPort = new Viewport();
                    if (HydroBot.gameMode == GameMode.ShipWreck)
                        viewPort = ShipWreckScene.GraphicDevice.Viewport;
                    else if (HydroBot.gameMode == GameMode.MainGame)
                        viewPort = PlayGameScene.GraphicDevice.Viewport;
                    else if (HydroBot.gameMode == GameMode.SurvivalMode)
                        viewPort = SurvivalGameScene.GraphicDevice.Viewport;
                    Vector3 screenPos = viewPort.Project(Position, gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity);
                    HydroBot.rippleCenter = new Vector2(screenPos.X / viewPort.Width, screenPos.Y / viewPort.Height);
                    PoseidonGame.audio.maneteeRoar.Play();
                }
            }

            if (isCasting == true) { 
                // Casting timeout
                if (PoseidonGame.playTime.TotalSeconds - startCasting.TotalSeconds > standingTime.TotalSeconds)
                {
                    isCasting = false; // Done casting
                    isWandering = true; // Let the wander state do the wandering task

                    lastCast = PoseidonGame.playTime;

                    // All enemies within a radius flee
                    for (int i = 0; i < enemiesSize; i++)
                    {
                        BaseEnemy tmp = (BaseEnemy)enemies[i];
                        if (Vector3.Distance(tmp.Position, Position) < GameConstants.SideKick_Look_Radius && !tmp.isFleeing)
                        {
                            //can't scare a boss if its hp is more than 1/3 its max health
                            //can't scare a submarine too
                            if (!(tmp is Submarine || (tmp.isBigBoss && tmp.health > 0.33f * tmp.maxHealth)))
                                tmp.isFleeing = true;
                            tmp.fleeingStart = PoseidonGame.playTime;

                            // Find the fleeing direction for this guy
                            tmp.fleeingDirection = tmp.Position - Position; // Flee from sea cow
                        }
                    } // END for()
                } // End if (PoseidonGame.playTime.TotalSeconds... (timeout), more effect will be added by including an "else"
                RestoreNormalAnimation();
            } // End if (isCasting == true)

            // Moving, and changing state Logic
            Vector3 destination = tank.Position + new Vector3(AfterX, 0, AfterZ);
            if (isWandering == true)
            {
                // If the fish is far from the point after the bot's back or is the bot moving
                if (Vector3.Distance(tank.Position, Position) > HydroBot.controlRadius || (tank.isMoving() && Vector3.Distance(tank.Position, Position) > 30f))
                {
                    isWandering = false;
                    isReturnBot = true;
                    isChasing = false;
                    isFighting = false;

                    seekDestination(destination, enemies, enemiesSize, fish, fishSize, tank, speedFactor);
                }
                else {
                    currentTarget = potentialEnemy;
                    if (currentTarget == null) // See no enemy
                        randomWalk(changeDirection, enemies, enemiesSize, fish, fishSize, tank);
                    else { // Hunt him down
                        isWandering = false;
                        isReturnBot = false;
                        isChasing = true;
                        isFighting = false;
                        
                        seekDestination(currentTarget.Position, enemies, enemiesSize, fish, fishSize, tank, speedFactor);
                    }
                }
            }// End wandering

            // If return is the current state
            else if (isReturnBot == true)
            {
                // If the fish is near the point after the bot's back, wander
                if (Vector3.Distance(destination, Position) < HydroBot.controlRadius * 0.5 &&
                    Vector3.Distance(tank.Position, Position) < HydroBot.controlRadius)
                {
                    isWandering = true;
                    isReturnBot = false;
                    isChasing = false;
                    isFighting = false;

                    randomWalk(changeDirection, enemies, enemiesSize, fish, fishSize, tank);
                }
                else
                    seekDestination(destination, enemies, enemiesSize, fish, fishSize, tank, speedFactor);
            }
            // If the fish is chasing some enemy
            else if (isChasing == true) {
                if (Vector3.Distance(tank.Position, Position) > HydroBot.controlRadius)
                {  // If too far, return to bot
                    isWandering = false;
                    isReturnBot = true;
                    isChasing = false;
                    isFighting = false;

                    currentTarget = null;
                    
                    seekDestination(destination, enemies, enemiesSize, fish, fishSize, tank, speedFactor);
                }
                else if (Vector3.Distance(Position, currentTarget.Position) <
                    5f + BoundingSphere.Radius + currentTarget.BoundingSphere.Radius) // Too close then attack
                {
                    isWandering = false;
                    isReturnBot = false;
                    isChasing = false;
                    isFighting = true;

                    attack();
                }
                else
                    seekDestination(currentTarget.Position, enemies, enemiesSize, fish, fishSize, tank, speedFactor);
            }
            // If fighting some enemy
            else if (isFighting == true) {
                // Enemy is down, return to bot
                if (currentTarget == null || currentTarget.health <= 0) {
                    isWandering = false;
                    isReturnBot = true;
                    isChasing = false;
                    isFighting = false;

                    currentTarget = null;

                    seekDestination(destination, enemies, enemiesSize, fish, fishSize, tank, speedFactor);
                }
                else {
                    // If the enemy ran away and the fish is not too far from bot

                    if (Vector3.Distance(tank.Position, Position) >= HydroBot.controlRadius)
                    {
                        isWandering = false;
                        isReturnBot = true;
                        isChasing = false;
                        isFighting = false;

                        currentTarget = null;

                        seekDestination(destination, enemies, enemiesSize, fish, fishSize, tank, speedFactor);
                    }
                    else if (Vector3.Distance(Position, currentTarget.Position) >=
                      10f + BoundingSphere.Radius + currentTarget.BoundingSphere.Radius)
                    {
                        isWandering = false;
                        isReturnBot = false;
                        isChasing = true;
                        isFighting = false;

                        seekDestination(currentTarget.Position, enemies, enemiesSize, fish, fishSize, tank, speedFactor);
                    }
                    else
                        attack();
                }
            }

            // if clip player has been initialized, update it
            if (clipPlayer != null || BoundingSphere.Intersects(cameraFrustum))
            {
                qRotation = Quaternion.CreateFromAxisAngle(
                                Vector3.Up,
                                ForwardDirection);
                float scale = 2.0f;
                if (isBigBoss) scale *= 2.0f;
                fishMatrix = Matrix.CreateScale(scale) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                                    Matrix.CreateFromQuaternion(qRotation) *
                                    Matrix.CreateTranslation(Position);
                clipPlayer.update(gameTime.ElapsedGameTime, true, fishMatrix);
            }
        }
    }
}
