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

        private Vector3 tankLastPosition;

        private bool chasing;
        
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
            chasing = false;
            giveUpTime = new TimeSpan(0, 0, 5);
            perceptionRadius = 40f;
            timeBetweenFire = 0.3f;
            prevFire = new TimeSpan();
        }

        public void Update(SwimmingObject[] enemies, int enemiesAmount, SwimmingObject[] fishes, int fishAmount, int changeDirection, Tank tank, List<DamageBullet> enemyBullet) {           
            if (updateHuntingObject(tank.Position)) {
                if (!chasing) {
                    randomWalk(changeDirection, enemies, enemiesAmount, tank);
                } else {
                    updatePosition();
                }
            } else {
                if (PlayGameScene.timming.TotalGameTime.TotalSeconds - prevFire.TotalSeconds > timeBetweenFire) {
                    PlayGameScene.placeEnemyBullet(Position, shootingDirection, GameConstants.DefaultEnemyDamage, enemyBullet);
                    prevFire = PlayGameScene.timming.TotalGameTime;
                }
            }
        }

        private void updatePosition() {
            Position += headingDirection * GameConstants.EnemySpeed;
            BoundingSphere = new BoundingSphere(Position, BoundingSphere.Radius);
        }

        private void randomWalk(int changeDirection, SwimmingObject[] objects, int size, Tank tank) {
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

                if (Collision.isBarriersValidMove(this, futurePosition, objects, size, tank)) {
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
        private bool updateHuntingObject(Vector3 obj) {
            if (Vector3.Distance(obj, Position) < perceptionRadius) {
                shootingDirection = obj - Position;
                shootingDirection.Normalize();
                startChasingTime = PlayGameScene.timming.TotalGameTime;
                headingDirection = shootingDirection;
                headingDirection.Normalize();
                tankLastPosition = obj;
                chasing = true;

                ForwardDirection = Tank.CalculateAngle(obj, Position);

                // Standby and shoot
                return false;
            }

            if (chasing) {
                if (PlayGameScene.timming.TotalGameTime.TotalSeconds - startChasingTime.TotalSeconds < giveUpTime.TotalSeconds) {
                    // Tank just get out of bound, chase towards its last direction
                    return true;
                } else {
                    // Stop chasing, move like normal
                    chasing = false;
                }
            }
            return true;
        }
    }
}