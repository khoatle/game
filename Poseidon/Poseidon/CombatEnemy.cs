using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon
{
    public class CombatEnemy : BaseEnemy {
        private BoundingSphere futureBoundingSphere;

        // Percept ID:
        // 0 = nothing detected
        // 1 = tank detected - 1st priory
        // 2 = last target is still in range - 2nd priory
        // 3 = a fish is detected - 3rd priory

        // Move bits:
        // 1st bit: randomWalk
        // 2nd bit: standstill
        // 3rd bit: move straight
        // 4th bit: is striking
        public CombatEnemy() : base() {
            perceptID = new int[] {0,1,2,3};
            configBits = new bool[] {false, false, false, false};
            speed = (float)(GameConstants.EnemySpeed*1.5);
            damage = GameConstants.DefaultEnemyDamage * 2;
        }

        public override void Update(SwimmingObject[] enemyList, int enemySize, SwimmingObject[] fishList, int fishSize, int changeDirection, Tank tank, List<DamageBullet> enemyBullets, List<DamageBullet> alliesBullets) {
            if (stunned) return;
            int perceptionID = perceptAndLock(tank, fishList, fishSize);
            configAction(perceptionID);
            makeAction(changeDirection, enemyList, enemySize, fishList, fishSize, enemyBullets, tank);
        }

        protected int perceptAndLock(Tank tank, SwimmingObject[] enemyList, int enemySize) {
            if (Vector3.Distance(Position, tank.Position) < perceptionRadius) {
                currentHuntingTarget = tank;
                return perceptID[1];
            } else {
                if (currentHuntingTarget != null 
                        && Vector3.Distance(Position, currentHuntingTarget.Position) < perceptionRadius) {
                    return perceptID[2];      
                }

                for (int i = 0; i < enemySize; i++) {
                    if (Vector3.Distance(Position, enemyList[i].Position) < perceptionRadius) { 
                        currentHuntingTarget = enemyList[i];
                        return perceptID[3];
                    }
                }
                return perceptID[0];
            }
        }

        protected void configAction(int perception) {
            if (perception == perceptID[0]) {
                if (currentHuntingTarget != null && clearMind() == false) {
                    configBits[0] = false;
                    configBits[1] = false;
                    configBits[2] = true;
                    configBits[3] = false;
                } else {
                    configBits[0] = true;
                    configBits[1] = false;
                    configBits[2] = false;
                    configBits[3] = false;
                }
            } else if (perception == perceptID[1] || perception == perceptID[2] || perception == perceptID[3]) {
                configBits[0] = false;
                configBits[2] = true;

                calculateFutureBoundingSphere();

                configBits[3] = (Vector3.Distance(Position, currentHuntingTarget.Position) < BoundingSphere.Radius + currentHuntingTarget.BoundingSphere.Radius + 3f) ? true : false;
                configBits[1] = configBits[3];
            }
        }

        private void calculateFutureBoundingSphere() {
            Vector3 futurePosition = Position + speed * headingDirection;
            futureBoundingSphere = new BoundingSphere(futurePosition, BoundingSphere.Radius);
        }

        // Execute the actions
        protected virtual void makeAction(int changeDirection, SwimmingObject[] enemies, int enemiesAmount, SwimmingObject[] fishes, int fishAmount, List<DamageBullet> bullets, Tank tank) {
            if (configBits[0] == true) { 
                randomWalk(changeDirection, enemies, enemiesAmount, fishes, fishAmount, tank);
                return;
            }
            if (currentHuntingTarget != null) { 
                calculateHeadingDirection();
                calculateFutureBoundingSphere();
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
                        return;
                    }
                }

                if (PlayGameScene.timming.TotalGameTime.TotalSeconds - prevFire.TotalSeconds > timeBetweenFire) {
                    if (currentHuntingTarget.GetType().Name.Equals("Tank")) {
                        //((Tank)currentHuntingTarget).currentHitPoint -= damage;
                        Tank.currentHitPoint -= damage;
                    }
                    if (currentHuntingTarget.GetType().Name.Equals("SwimmingObject")) {
                        ((SwimmingObject)currentHuntingTarget).health -= damage;
                    }
                    prevFire = PlayGameScene.timming.TotalGameTime;
                }
            }
        }
    }
}