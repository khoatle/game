using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon
{
    public class ShootingEnemy : BaseEnemy  {

        // shortDistance is the distance the enemy will try to keep from the victim
        protected bool closeEnough;
        protected float shortDistance;

        public ShootingEnemy() : base() { 
            perceptID = new int[] {0,1,2,3};
            configBits = new bool[] {false, false, false, false};
            shortDistance = GameConstants.EnemyShootingDistance;
        }

        // Return the perceptID correspondingly
        protected int perceptAndLock(Tank tank, SwimmingObject[] enemyList, int enemySize) {
            if (Vector3.Distance(Position, tank.Position) < perceptionRadius) {
                closeEnough = (Vector3.Distance(Position, tank.Position) > shortDistance) ? false : true;
                currentHuntingTarget = tank;
                return perceptID[1];
            } else {
                if (currentHuntingTarget != null 
                        && Vector3.Distance(Position, currentHuntingTarget.Position) < perceptionRadius) {
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

        protected void configAction(int perception) {
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
                } else {
                    configBits[0] = false;
                    configBits[1] = false;
                    configBits[2] = true;
                    configBits[3] = true;
                }
            }
        }

        // Execute the actions
        protected virtual void makeAction(int changeDirection, SwimmingObject[] enemies, int enemiesAmount, SwimmingObject[] fishes, int fishAmount, List<DamageBullet> bullets, Tank tank) {
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
                    AddingObjects.placeEnemyBullet(this, GameConstants.DefaultEnemyDamage, bullets, 0);
                    prevFire = PlayGameScene.timming.TotalGameTime;
                }
            }
        }

        public override void Update(SwimmingObject[] enemyList, int enemySize, SwimmingObject[] fishList, int fishSize, int changeDirection, Tank tank, List<DamageBullet> bullets) {
            // do not delete this
            if (stunned) return;
            int perceptionID = perceptAndLock(tank, fishList, fishSize);
            configAction(perceptionID);
            makeAction(changeDirection, enemyList, enemySize, fishList, fishSize, bullets, tank);
        }
    }
}
