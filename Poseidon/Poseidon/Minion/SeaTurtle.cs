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
        public static float AfterX = HydroBot.controlRadius / 3;
        public static float AfterZ = - HydroBot.controlRadius * 1.73f / 3 ;

        public static float turtleDamage = 30f;
        public static float frozenBreathDamage = 30f;

        // Frozen breathe attack stuff
        public TimeSpan lastCast;
        public TimeSpan coolDown;
        //int numTimeReleaseFrozenBreath = 0;

        public TimeSpan startCasting;
        public TimeSpan standingTime;
        public bool firstCast;

        public TimeSpan lastAttack;
        public TimeSpan timeBetweenAttack;

        ParticleManagement particleManager;

        public SeaTurtle() : base() {
            lastAttack = PoseidonGame.playTime;
            // Cool down 10s
            timeBetweenAttack = new TimeSpan(0, 0, 1);

            // Cool down 5s for the skill
            coolDown = new TimeSpan(0, 0, 5);
            lastCast = PoseidonGame.playTime;

            // Cool down 20s
            standingTime = new TimeSpan(0, 0, 1);

            isBigBoss = false;

            maxHealth = GameConstants.TurtleStartingHealth;
            health = maxHealth;

            if (HydroBot.gameMode == GameMode.ShipWreck)
                particleManager = ShipWreckScene.particleManager;
            else if (HydroBot.gameMode == GameMode.MainGame)
                particleManager = PlayGameScene.particleManager;
            else if (HydroBot.gameMode == GameMode.SurvivalMode)
                particleManager = SurvivalGameScene.particleManager;

            speedFactor = 1.5f + (HydroBot.seaCowPower - 1) / 4;
        }

        public override void attack()
        {
            float damage = HydroBot.turtlePower * turtleDamage;
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

        public void FrozenBreathe(SwimmingObject[] enemies, int enemiesSize, bool firstCast) {
            Matrix orientationMatrix = Matrix.CreateRotationY(ForwardDirection);
            Vector3 movement = Vector3.Zero;
            movement.Z = 1;
            Vector3 observerDirection = Vector3.Transform(movement, orientationMatrix);
            
            for (int i = 0; i < enemiesSize; i++) {
                Vector3 otherDirection = enemies[i].Position - Position;
                if (Behavior.isInSight(observerDirection, Position, otherDirection, enemies[i].Position, MathHelper.PiOver4, 
                    60f * (float)((PoseidonGame.playTime.TotalMilliseconds - startCasting.TotalMilliseconds) / standingTime.TotalMilliseconds)))
                {
                //if (Vector3.Distance(enemies[i].Position, Position) < 60f) {
                    if (firstCast) 
                        enemies[i].health -= frozenBreathDamage * HydroBot.turtlePower;
                    enemies[i].speedFactor = 0.5f;
                    enemies[i].slowStart = PoseidonGame.playTime;
                }
            }

            if (particleManager.frozenBreathParticles != null)
            {
                for (int k = 0; k < GameConstants.numFrozenBreathParticlesPerUpdate; k++)
                    particleManager.frozenBreathParticles.AddParticle(Position + Vector3.Transform(new Vector3(0, 0, 1), orientationMatrix) * 10, Vector3.Zero, ForwardDirection, MathHelper.PiOver4);
            }
            //numTimeReleaseFrozenBreath += 1;
        }

        float lastPower = 1.0f;
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, BoundingFrustum cameraFrustum, SwimmingObject[] enemies, int enemiesSize, 
            SwimmingObject[] fish, int fishSize, int changeDirection, HydroBot tank, List<DamageBullet> enemyBullet)
        {
            EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);

            if (lastPower != HydroBot.turtlePower)
            {
                float lastMaxHealth = maxHealth;
                maxHealth = GameConstants.TurtleStartingHealth * HydroBot.turtlePower;
                health += (maxHealth - lastMaxHealth);
                speedFactor = 1.5f + (HydroBot.turtlePower - 1) / 4;
                lastPower = HydroBot.turtlePower;
            }

            BaseEnemy potentialEnemy = lookForEnemy(enemies, enemiesSize);
            if (!isReturnBot && !isCasting && potentialEnemy != null)
            {
                // It is OK to cast?
                if (PoseidonGame.playTime.TotalSeconds - lastCast.TotalSeconds > coolDown.TotalSeconds)// && HydroBot.currentHitPoint < HydroBot.maxHitPoint)
                {
                    isCasting = true;
                    isReturnBot = false;
                    isWandering = false;
                    isChasing = false;
                    isFighting = false;

                    // For this case, I choose to set the forward direction statically rather than
                    // facing different enemy since potentialEnemy may die, run away, etc
                    Vector3 facingDirection = potentialEnemy.Position - Position;
                    ForwardDirection = (float)Math.Atan2(facingDirection.X, facingDirection.Z);

                    firstCast = true;

                    startCasting = PoseidonGame.playTime;
                    PoseidonGame.audio.frozenBreathe.Play();
                }
            }

            if (isCasting == true)
            {
                // Casting timeout
                if (PoseidonGame.playTime.TotalSeconds - startCasting.TotalSeconds > standingTime.TotalSeconds)
                {
                    isCasting = false; // Done casting
                    isWandering = true; // Let the wander state do the wandering task, (and also lock enemy is potential enemy != null)

                    lastCast = PoseidonGame.playTime;
                } // Effect during cast
                else {
                    FrozenBreathe(enemies, enemiesSize, firstCast);
                    firstCast = false;   
                }
                RestoreNormalAnimation();
            }

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
                else
                {
                    currentTarget = potentialEnemy;
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
                if (Vector3.Distance(destination, Position) < HydroBot.controlRadius * 0.5 &&
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
                if (Vector3.Distance(tank.Position, Position) > HydroBot.controlRadius * 1.25f)
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
            if (clipPlayer != null || BoundingSphere.Intersects(cameraFrustum))
            {
                qRotation = Quaternion.CreateFromAxisAngle(
                                Vector3.Up,
                                ForwardDirection);
                float scale = 0.7f;
                if (isBigBoss) scale *= 2.0f;
                fishMatrix = Matrix.CreateScale(scale) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                                    Matrix.CreateFromQuaternion(qRotation) *
                                    Matrix.CreateTranslation(Position);
                clipPlayer.update(gameTime.ElapsedGameTime, true, fishMatrix);
            }
        }
    }
}
