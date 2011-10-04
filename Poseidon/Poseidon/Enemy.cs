using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon {
    public class Enemy : SwimmingObject {

        // Percept ID:
        // 0 = nothing detected
        // 1 = tank detected - 1st priory
        // 2 = last target is still in range - 2nd priory
        // 3 = a fish is detected - 3rd priory
        public static int[] perceptID = {0,1,2,3};

        private bool closeEnough;

        // Move bits:
        // 1st bit: randomWalk
        // 2nd bit: standstill
        // 3rd bit: move straight
        // 4th bit: shooting
        public static bool[] configBits = {false,false,false,false};

        public Vector3 headingDirection;
        private Vector3 futurePosition; 

        private GameObject currentHuntingTarget;

        //stunned and cannot move
        public bool stunned;
        public double stunnedStartTime;

        // Time stampt since the robot starts chasing
        private TimeSpan startChasingTime;
        public TimeSpan prevFire;
        private float timeBetweenFire;

        // Give up after 3 seconds
        private TimeSpan giveUpTime;

        public float perceptionRadius;

        private bool isCombat;
        private float shortDistance;

        public Enemy()
            : base()
        {
            giveUpTime = new TimeSpan(0, 0, 3);
            perceptionRadius = GameConstants.EnemyPerceptionRadius;
            timeBetweenFire = 0.3f;
            stunned = false;
            prevFire = new TimeSpan();

            isCombat = false;
            shortDistance = GameConstants.EnemyShootingDistance;

            currentHuntingTarget = null;
        }

        public void Update(SwimmingObject[] enemyList, int enemySize, SwimmingObject[] fishList, int fishSize, int changeDirection, Tank tank, List<DamageBullet> bullets) {
            int perceptionID = perceptAndLock(tank, fishList, fishSize);
            configAction(perceptionID);
            makeAction(changeDirection, enemyList, enemySize, fishList, fishSize, bullets, tank);
        }

        private bool clearMind() {
            if (startChasingTime.TotalSeconds == 0 || PlayGameScene.timming.TotalGameTime.TotalSeconds - startChasingTime.TotalSeconds > giveUpTime.TotalSeconds) {
                currentHuntingTarget = null;
                startChasingTime = PlayGameScene.timming.TotalGameTime;
                return true;
            }
            return false;
        }

        private void calculateHeadingDirection() {
            ForwardDirection = Tank.CalculateAngle(currentHuntingTarget.Position, Position);
            headingDirection = currentHuntingTarget.Position - Position;
            headingDirection.Normalize();
        }

        // Return the perceptID correspondingly
        private int perceptAndLock(Tank tank, SwimmingObject[] enemyList, int enemySize) {
            if (Vector3.Distance(Position, tank.Position) < perceptionRadius) {
                closeEnough = (Vector3.Distance(Position, tank.Position) > shortDistance)? false : true;
                currentHuntingTarget = tank;
                return perceptID[1];
            }
            else {
                if (currentHuntingTarget != null && Vector3.Distance(Position, currentHuntingTarget.Position) < perceptionRadius) {
                    closeEnough = (Vector3.Distance(Position, currentHuntingTarget.Position) > shortDistance) ? false : true;
                    return perceptID[2];
                }

                for (int i = 0; i < enemySize; i++) {
                    if (Vector3.Distance(Position, enemyList[i].Position) < perceptionRadius) {
                        closeEnough = (Vector3.Distance(Position, enemyList[i].Position) > shortDistance) ? false : true;
                        currentHuntingTarget = enemyList[i];
                        return perceptID[3];
                    }
                }
                return perceptID[0];
            }
        }

        // Config what action to take
        private void configAction(int perception) {
            if (perception == perceptID[0]) {
                if (currentHuntingTarget != null && clearMind() == false) {
                    configBits[0] = false;
                    configBits[1] = false;
                    configBits[2] = true;
                    configBits[3] = true;
                } else {
                    configBits[0] = true;
                    configBits[1] = false;
                    configBits[2] = false;
                    configBits[3] = false;
                }
            } else if (perception == perceptID[1] || perception == perceptID[2] || perception == perceptID[3]) {
                if (closeEnough == true) {
                    configBits[0] = false;
                    configBits[1] = true;
                    configBits[2] = false;
                    configBits[3] = true;
                }
                else {
                    configBits[0] = false;
                    configBits[1] = false;
                    configBits[2] = true;
                    configBits[3] = true;
                }
            }
        }

        // Execute the actions
        private void makeAction(int changeDirection, SwimmingObject[] enemies, int enemiesAmount, SwimmingObject[] fishes, int fishAmount, List<DamageBullet> bullets, Tank tank) {
            if (configBits[0] == true) {
                randomWalk(changeDirection, enemies, enemiesAmount, fishes, fishAmount, tank);
                return;
            }
            if (currentHuntingTarget != null) {
                calculateHeadingDirection();
            }
            if (configBits[2] == true) { 
                goStraight(enemies, enemiesAmount, fishes, fishAmount, tank);
            }
            if (configBits[3] == true) {
                startChasingTime = PlayGameScene.timming.TotalGameTime;

                if (currentHuntingTarget.GetType().Name.Equals("Fish")) {
                    Fish tmp = (Fish)currentHuntingTarget;
                    if (tmp.health <= 0) {
                        currentHuntingTarget = null;
                    }
                }

                if (PlayGameScene.timming.TotalGameTime.TotalSeconds - prevFire.TotalSeconds > timeBetweenFire) {
                    AddingObjects.placeEnemyBullet(this, GameConstants.DefaultEnemyDamage, bullets);
                    prevFire = PlayGameScene.timming.TotalGameTime;
                }
            }
        } 

        // Go straight
        private void goStraight(SwimmingObject[] enemies, int enemiesAmount, SwimmingObject[] fishes, int fishAmount, Tank tank) {
            Vector3 futurePosition = Position + GameConstants.EnemySpeed*headingDirection;
            if (Collision.isBarriersValidMove(this, futurePosition, enemies, enemiesAmount, tank)
                    && Collision.isBarriersValidMove(this, futurePosition, fishes, fishAmount, tank)) {
                Position = futurePosition;
                BoundingSphere.Center = Position;
            }
        }

        // Go randomly
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
                headingDirection *= GameConstants.EnemySpeed;
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
    }
}