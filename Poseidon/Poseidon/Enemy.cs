using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon {
    public class Enemy : SwimmingObject {
        public Vector3 shootingDirection;
        public Vector3 headingDirection;
        public Vector3 futurePosition; 

        private bool hasPrevTarget;
        private GameObject lastTarget;

        private bool chasing;
        //stunned and cannot move
        public bool stunned;

        // Time stampt since the robot starts chasing
        private TimeSpan startChasingTime;
        public TimeSpan prevFire;
        private float timeBetweenFire;

        // Give up after 30 seconds
        private TimeSpan giveUpTime;

        public float perceptionRadius;

        public Enemy()
            : base()
        {
            giveUpTime = new TimeSpan(0, 0, 5);
            perceptionRadius = 30f;
            timeBetweenFire = 0.5f;

            chasing = false;
            stunned = false;

            prevFire = new TimeSpan();
            hasPrevTarget = false;
        }

        private void lockAtributes(GameObject obj) {
            hasPrevTarget = true;
            lastTarget = obj;
            shootingDirection = obj.Position - Position;
            shootingDirection.Normalize();
            startChasingTime = PlayGameScene.timming.TotalGameTime;
            headingDirection = shootingDirection;
            headingDirection.Normalize();
            ForwardDirection = Tank.CalculateAngle(obj.Position, Position);
        }

        private void shoot(List<DamageBullet> enemyBullet) {
            if (PlayGameScene.timming.TotalGameTime.TotalSeconds - prevFire.TotalSeconds > timeBetweenFire) {
                PlayGameScene.placeEnemyBullet(Position, shootingDirection, GameConstants.DefaultEnemyDamage, enemyBullet);
                prevFire = PlayGameScene.timming.TotalGameTime;
            }
        }

        public void Update(SwimmingObject[] enemies, int enemiesAmount, SwimmingObject[] fishes, int fishAmount, int changeDirection, Tank tank, List<DamageBullet> enemyBullet) {
            if (hasPrevTarget) {
                if (Vector3.Distance(tank.Position, Position) < perceptionRadius) {
                    lockAtributes(tank);
                    shoot(enemyBullet);
                    return;
                }
                    
                if (Vector3.Distance(Position, lastTarget.Position) < perceptionRadius) {
                    lockAtributes(lastTarget);
                    shoot(enemyBullet);

                    if (lastTarget.GetType().Name.Equals("Tank")) {
                        if (((Tank)lastTarget).hitPoint <= 0) {
                            hasPrevTarget = false;
                            lastTarget = null;
                        }
                    } else if (lastTarget.GetType().Name.Equals("Fish")) {
                        if (((Fish)lastTarget).health <= 0) {
                            hasPrevTarget = false;
                            lastTarget = null;
                        }
                    }

                    return;
                }
                    
                for (int i = 0; i < fishAmount; i++) {
                    if (Vector3.Distance(fishes[i].Position, Position) < perceptionRadius) { 
                        lockAtributes(fishes[i]);
                        shoot(enemyBullet);
                        return;
                    }
                }
                // Hunting phase
                if (PlayGameScene.timming.TotalGameTime.TotalSeconds - startChasingTime.TotalSeconds < giveUpTime.TotalSeconds) {
                    // Tank just get out of bound, chase towards its last direction
                    go(enemies, enemiesAmount, fishes, fishAmount, tank, changeDirection);
                    return;
                } else {
                    hasPrevTarget = false;
                    return;
                }
            }

            if (Vector3.Distance(tank.Position, Position) < perceptionRadius) {
                lockAtributes(tank);
                shoot(enemyBullet);
                return;
            }

            for (int i = 0; i < fishAmount; i++) {
                if (Vector3.Distance(fishes[i].Position, Position) < perceptionRadius)
                {
                    lockAtributes(fishes[i]);
                    shoot(enemyBullet);
                    return;
                }
            }
            randomWalk(changeDirection, enemies, enemiesAmount, fishes, fishAmount, tank);
            
            //if (Vector3.Distance(tank.Position, Position) < perceptionRadius)
            //{
            //    shootingDirection = tank.Position - Position;
            //    shootingDirection.Normalize();
            //    startChasingTime = PlayGameScene.timming.TotalGameTime;
            //    headingDirection = shootingDirection;
            //    headingDirection.Normalize();
            //    ForwardDirection = Tank.CalculateAngle(tank.Position, Position);
            //    lastTarget = tank;

            //    if (PlayGameScene.timming.TotalGameTime.TotalSeconds - prevFire.TotalSeconds > timeBetweenFire)
            //    {
            //        PlayGameScene.placeEnemyBullet(Position, shootingDirection, GameConstants.DefaultEnemyDamage, enemyBullet);
            //        prevFire = PlayGameScene.timming.TotalGameTime;
            //    }
            //    return;
            //} else {
            //    for (int i = 0; i < enemiesAmount; i++) {
            //        if (Vector3.Distance(Position, fishes[i].Position) < perceptionRadius) {
            //            shootingDirection = tank.Position - Position;
            //            shootingDirection.Normalize();
            //            startChasingTime = PlayGameScene.timming.TotalGameTime;
            //            headingDirection = shootingDirection;
            //            headingDirection.Normalize();
            //            ForwardDirection = Tank.CalculateAngle(tank.Position, Position);

            //            if (PlayGameScene.timming.TotalGameTime.TotalSeconds - prevFire.TotalSeconds > timeBetweenFire)
            //            {
            //                PlayGameScene.placeEnemyBullet(Position, shootingDirection, GameConstants.DefaultEnemyDamage, enemyBullet);
            //                prevFire = PlayGameScene.timming.TotalGameTime;
            //            }
            //            return;
            //        }
            //    }
            //}




            
            
            
            // if (huntingTankStatus(tank.Position) && chasing) {
            //    if (!chasing) {
            //        randomWalk(changeDirection, enemies, enemiesAmount, fishes, fishAmount, tank);
            //    } else {
            //        go(enemies, enemiesAmount, fishes, fishAmount, tank, changeDirection);
            //    }
            //} else if (huntingFishStatus(fishes, fishAmount)) {
            //    if (!chasing) {
            //        randomWalk(changeDirection, enemies, enemiesAmount, fishes, fishAmount, tank);
            //    } else {
            //    }
            //}
            //else {
            //    if (PlayGameScene.timming.TotalGameTime.TotalSeconds - prevFire.TotalSeconds > timeBetweenFire) {
            //        PlayGameScene.placeEnemyBullet(Position, shootingDirection, GameConstants.DefaultEnemyDamage, enemyBullet);
            //        prevFire = PlayGameScene.timming.TotalGameTime;
            //    }
            //}

            //if (huntTank(tank.Position)) {
            //    if (PlayGameScene.timming.TotalGameTime.TotalSeconds - prevFire.TotalSeconds > timeBetweenFire) {
            //        PlayGameScene.placeEnemyBullet(Position, shootingDirection, GameConstants.DefaultEnemyDamage, enemyBullet);
            //        prevFire = PlayGameScene.timming.TotalGameTime;
            //    }
            //}
            //else if (huntFish(fishes, fishAmount)) {
            //    if (PlayGameScene.timming.TotalGameTime.TotalSeconds - prevFire.TotalSeconds > timeBetweenFire)
            //    {
            //        PlayGameScene.placeEnemyBullet(Position, shootingDirection, GameConstants.DefaultEnemyDamage, enemyBullet);
            //        prevFire = PlayGameScene.timming.TotalGameTime;
            //    }            
            //}
        }

        private void go(SwimmingObject[] enemy, int enemiesAmount, SwimmingObject[] fish, int fishAmount, Tank tank, int changeDirection) {
            Vector3 futurePosition = Position + headingDirection * GameConstants.EnemySpeed;

            if (Collision.isBarriersValidMove(this, futurePosition, enemy, enemiesAmount, tank)
                && Collision.isBarriersValidMove(this, futurePosition, fish, fishAmount, tank)) {
                Position = futurePosition;
                BoundingSphere = new BoundingSphere(futurePosition, BoundingSphere.Radius);
            }
        }

        private void randomWalk(int changeDirection, SwimmingObject[] enemies, int enemiesAmount, SwimmingObject[] fishes, int fishAmount, Tank tank) {
            futurePosition = Position;
            //int barrier_move
            Random random = new Random();
            float turnAmount = 0;
            //also try to change direction if we are stuck
            if (changeDirection >= 95 || stucked == true) {
                int rightLeft = random.Next(2);
                if (rightLeft == 0)
                    turnAmount = 20;
                else turnAmount = -20;
            }

            Matrix orientationMatrix;
            // Vector3 speed;
            Vector3 movement = Vector3.Zero;

            movement.Z = 1;
            float prevForwardDir = ForwardDirection;
            // try upto 10 times to change direction is there is collision
            for (int i = 0; i < 4; i++)
            {
                ForwardDirection += turnAmount * GameConstants.TurnSpeed;
                orientationMatrix = Matrix.CreateRotationY(ForwardDirection);
                headingDirection = Vector3.Transform(movement, orientationMatrix);
                headingDirection *= GameConstants.BarrierVelocity;
                futurePosition = Position + headingDirection;

                if (Collision.isBarriersValidMove(this, futurePosition, enemies, enemiesAmount, tank)
                    && Collision.isBarriersValidMove(this, futurePosition, fishes, fishAmount, tank)) {
                    Position = futurePosition;

                    BoundingSphere updatedSphere;
                    updatedSphere = BoundingSphere;

                    updatedSphere.Center.X = Position.X;
                    updatedSphere.Center.Z = Position.Z;
                    BoundingSphere = new BoundingSphere(updatedSphere.Center,
                        updatedSphere.Radius);

                    stucked = false;
                    break;
                }
                else stucked = true;
            }
        }

        // true: Running, false: Standby
        //private bool huntingTankStatus(Vector3 obj) {
        //    if (Vector3.Distance(obj, Position) < perceptionRadius) {
        //        shootingDirection = obj - Position;
        //        shootingDirection.Normalize();
        //        startChasingTime = PlayGameScene.timming.TotalGameTime;
        //        headingDirection = shootingDirection;
        //        headingDirection.Normalize();
        //        tankLastPosition = obj;
        //        chasing = true;

        //        ForwardDirection = Tank.CalculateAngle(obj, Position);

        //        // Standby and shoot
        //        return false;
        //    }

        //    if (chasing) {
        //        if (PlayGameScene.timming.TotalGameTime.TotalSeconds - startChasingTime.TotalSeconds < giveUpTime.TotalSeconds) {
        //            // Tank just get out of bound, chase towards its last direction
        //            return true;
        //        } else {
        //            // Stop chasing, move like normal
        //            chasing = false;
        //        }
        //    }
        //    return true;
        //}

        //private bool huntingFishStatus(SwimmingObject[] fishes, int fishAmount) {
        //    for (int i = 0; i < fishAmount; i++) {
        //        if (fishes[i].BoundingSphere.Intersects(this.BoundingSphere)) {
        //            fishLastPosition = fishes[i].Position;
        //            return true;
        //        }                        
        //    }
        //    return false;
        //}
    }
}