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
    public class SeaTurtle : Fish
    {
        public static float AfterX = HydroBot.controlRadius / 4;
        public static float AfterZ = - HydroBot.controlRadius * 1.73f / 4 ;

        public static float turtleDamage = 5f;

        // Frozen breathe attack stuff
        public TimeSpan lastCast;
        public TimeSpan coolDown;

        public TimeSpan lastAttack;
        public TimeSpan timeBetweenAttack; 

        public SeaTurtle() : base() {
            lastAttack = PoseidonGame.playTime;
            // Cool down 10s
            timeBetweenAttack = new TimeSpan(0, 0, 1);

            // Cool down 5s for the skill
            coolDown = new TimeSpan(0, 0, 5);
            lastCast = PoseidonGame.playTime;

            isBigBoss = false;
        }

        public override void attack()
        {
            if (PoseidonGame.playTime.TotalSeconds - lastAttack.TotalSeconds > timeBetweenAttack.TotalSeconds)
            {
                currentTarget.health -= turtleDamage;

                lastAttack = PoseidonGame.playTime;

                Point point = new Point();
                String point_string = "Enemy Got Biten, health - " + turtleDamage;
                point.LoadContent(PoseidonGame.contentManager, point_string, currentTarget.Position, Color.Red);
                if (HydroBot.gameMode == GameMode.ShipWreck)
                    ShipWreckScene.points.Add(point);
                else if (HydroBot.gameMode == GameMode.MainGame)
                    PlayGameScene.points.Add(point);
                else if (HydroBot.gameMode == GameMode.SurvivalMode)
                    SurvivalGameScene.points.Add(point);
            }
        }

        public void roar(SwimmingObject[] enemies, int enemiesSize) {
            Matrix orientationMatrix = Matrix.CreateRotationY(ForwardDirection);
            Vector3 movement = Vector3.Zero;
            movement.Z = 1;
            Vector3 observerDirection = Vector3.Transform(movement, orientationMatrix);
            
            for (int i = 0; i < enemiesSize; i++) {
                Vector3 otherDirection = enemies[i].Position - Position;
                if (Behavior.isInSight(observerDirection, Position, otherDirection, enemies[i].Position, (float)Math.PI * 4/3, 60f)) {
                //if (Vector3.Distance(enemies[i].Position, Position) < 60f) {
                    enemies[i].health -= turtleDamage;
                    enemies[i].speedFactor = 0.5f;
                    enemies[i].slowStart = PoseidonGame.playTime;
                }
            }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, SwimmingObject[] enemies, int enemiesSize, SwimmingObject[] fish, int fishSize, int changeDirection, HydroBot tank, List<DamageBullet> enemyBullet)
        {
            // Frozen breathe
            if (PoseidonGame.playTime.TotalSeconds - lastCast.TotalSeconds > coolDown.TotalSeconds) {
                roar(enemies, enemiesSize);

                lastCast = PoseidonGame.playTime;

                Point point = new Point();
                String point_string = "Just roar";
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

                    seekDestination(destination, enemies, enemiesSize, fish, fishSize, tank, speedFactor);
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

                        seekDestination(currentTarget.Position, enemies, enemiesSize, fish, fishSize, tank, speedFactor);
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
                    float test = Vector3.Distance(destination, Position);

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
            else if (isChasing == true)
            {
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

                    seekDestination(destination, enemies, enemiesSize, fish, fishSize, tank, speedFactor);
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

                        seekDestination(destination, enemies, enemiesSize, fish, fishSize, tank, speedFactor);
                    } // Or enemy ran away
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
