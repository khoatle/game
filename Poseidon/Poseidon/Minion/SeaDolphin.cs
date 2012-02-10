﻿using System;
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

        public static TimeSpan lastCast;
        public static TimeSpan coolDown;

        public static TimeSpan lastAttack;
        public static TimeSpan timeBetweenAttack; 

        public static float healAmount = 50f;

        public static float dolphinDamage = 30f;

        public SeaDolphin() : base() {
            lastCast = PoseidonGame.playTime;
            lastAttack = PoseidonGame.playTime;
            // Cool down 1s
            timeBetweenAttack = new TimeSpan(0,0,1);
            // Cool down 5s
            coolDown = new TimeSpan(0, 0, 5);
            isBigBoss = true;

        }

        public bool isTargeting() {
            return currentTarget == null || currentTarget.health <= 0;
        }

        public override void attack()
        {
            if (PoseidonGame.playTime.TotalSeconds - lastAttack.TotalSeconds > timeBetweenAttack.TotalSeconds)
            {
                currentTarget.health -= dolphinDamage;

                lastAttack = PoseidonGame.playTime;

                Point point = new Point();
                String point_string = "Enemy Got Biten, health - " + dolphinDamage;
                point.LoadContent(PoseidonGame.contentManager, point_string, currentTarget.Position, Color.Red);
                if (HydroBot.gameMode == GameMode.ShipWreck)
                    ShipWreckScene.points.Add(point);
                else if (HydroBot.gameMode == GameMode.MainGame)
                    PlayGameScene.points.Add(point);
                else if (HydroBot.gameMode == GameMode.SurvivalMode)
                    SurvivalGameScene.points.Add(point);
            }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, SwimmingObject[] enemies, int enemiesSize, SwimmingObject[] fish, int fishSize, int changeDirection, HydroBot tank, List<DamageBullet> enemyBullet)
        {
            // Heal
            if (PoseidonGame.playTime.TotalSeconds - lastCast.TotalSeconds > coolDown.TotalSeconds && HydroBot.currentHitPoint < HydroBot.maxHitPoint) {
                HydroBot.currentHitPoint += healAmount;
                // Got capped by Max HP
                HydroBot.currentHitPoint = Math.Min(HydroBot.maxHitPoint, HydroBot.currentHitPoint);

                lastCast = PoseidonGame.playTime;

                Point point = new Point();
                String point_string = "Health + " + healAmount;
                point.LoadContent(PoseidonGame.contentManager, point_string, Position, Color.LawnGreen);
                if (HydroBot.gameMode == GameMode.ShipWreck)
                    ShipWreckScene.points.Add(point);
                else if (HydroBot.gameMode == GameMode.MainGame)
                    PlayGameScene.points.Add(point);
                else if (HydroBot.gameMode == GameMode.SurvivalMode)
                    SurvivalGameScene.points.Add(point);
            }

            Vector3 destination = tank.Position + new Vector3(AfterX, 0, AfterZ);
            if (isWandering == true)
            {
                // If the fish is far from the point after the bot's back or is the bot moving
                if (Vector3.Distance(tank.Position, Position) > HydroBot.controlRadius || tank.isMoving())
                {
                    isWandering = false;
                    isReturnBot = true;
                    isChasing = false;
                    isFighting = false;

                    seekDestination(destination, enemies, enemiesSize, fish, fishSize, tank);
                }
                else
                {
                    currentTarget = lookForEnemy(enemies, enemiesSize);
                    if (currentTarget == null) // See no enemy
                        randomWalk(changeDirection, enemies, enemiesSize, fish, fishSize, tank);
                    else
                    { // Hunt him down
                        isWandering = false;
                        isReturnBot = false;
                        isChasing = true;
                        isFighting = false;

                        seekDestination(currentTarget.Position, enemies, enemiesSize, fish, fishSize, tank);
                    }
                }
            }// End wandering

            // If return is the current state
            else if (isReturnBot == true)
            {
                // If the fish is near the point after the bot's back, wander
                if (Vector3.Distance(destination, Position) < HydroBot.controlRadius * 0.75 &&
                    Vector3.Distance(tank.Position, Position) < HydroBot.controlRadius)
                {
                    isWandering = true;
                    isReturnBot = false;
                    isChasing = false;
                    isFighting = false;

                    randomWalk(changeDirection, enemies, enemiesSize, fish, fishSize, tank);
                }
                else
                    seekDestination(destination, enemies, enemiesSize, fish, fishSize, tank);
            }
            // If the fish is chasing some enemy
            else if (isChasing == true)
            {
                if (Vector3.Distance(tank.Position, Position) > HydroBot.controlRadius)
                {  // If too far, return to bot
                    isWandering = false;
                    isReturnBot = true;
                    isChasing = false;
                    isFighting = false;

                    currentTarget = null;

                    seekDestination(destination, enemies, enemiesSize, fish, fishSize, tank);
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
                    seekDestination(currentTarget.Position, enemies, enemiesSize, fish, fishSize, tank);
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

                    seekDestination(destination, enemies, enemiesSize, fish, fishSize, tank);
                }
                else
                {
                    // If the enemy ran away and the fish is not too far from bot

                    if (Vector3.Distance(tank.Position, Position) >= HydroBot.controlRadius)
                    {
                        isWandering = false;
                        isReturnBot = true;
                        isChasing = false;
                        isFighting = false;

                        currentTarget = null;

                        seekDestination(destination, enemies, enemiesSize, fish, fishSize, tank);
                    }
                    else if (Vector3.Distance(Position, currentTarget.Position) >=
                      5f + BoundingSphere.Radius + currentTarget.BoundingSphere.Radius)
                    {
                        isWandering = false;
                        isReturnBot = false;
                        isChasing = true;
                        isFighting = false;

                        seekDestination(currentTarget.Position, enemies, enemiesSize, fish, fishSize, tank);
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
