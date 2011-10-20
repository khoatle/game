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
            damage = GameConstants.DefaultEnemyDamage * 3;
            perceptionRadius *= 2;
            isHypnotise = false;
        }

        public override void Update(SwimmingObject[] enemyList, int enemySize, SwimmingObject[] fishList, int fishSize, int changeDirection, Tank tank, List<DamageBullet> enemyBullets, List<DamageBullet> alliesBullets) {
            qRotation = Quaternion.CreateFromAxisAngle(
                            Vector3.Up,
                            ForwardDirection);
            enemyMatrix = Matrix.CreateScale(0.1f) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                                Matrix.CreateFromQuaternion(qRotation) *
                                Matrix.CreateTranslation(Position);
            clipPlayer.update(PlayGameScene.timming.ElapsedGameTime, true, enemyMatrix);

            if (stunned)
            {
                if (!clipPlayer.inRange(1, 30))
                    clipPlayer.switchRange(1, 30);
                return;
            }

            if (isHypnotise && PlayGameScene.timming.TotalGameTime.TotalSeconds - startHypnotiseTime.TotalSeconds > GameConstants.timeHypnotiseLast) {
                wearOutHypnotise();
            }
            if (!isHypnotise) {
                int perceptionID = perceptAndLock(tank, fishList, fishSize);
                configAction(perceptionID);
                makeAction(changeDirection, enemyList, enemySize, fishList, fishSize, enemyBullets, tank);
            }
            else {
                int perceptionID = perceptAndLock(tank, enemyList, enemySize);
                configAction(perceptionID);
                makeAction(changeDirection, enemyList, enemySize, fishList, fishSize, enemyBullets, tank);
            }
        }

        protected int perceptAndLock(Tank tank, SwimmingObject[] enemyList, int enemySize) {
            if (!isHypnotise && Vector3.Distance(Position, tank.Position) < perceptionRadius) {
                currentHuntingTarget = tank;
                return perceptID[1];
            } else {
                if (currentHuntingTarget != null 
                        && Vector3.Distance(Position, currentHuntingTarget.Position) < perceptionRadius) {
                    return perceptID[2];      
                }

                for (int i = 0; i < enemySize; i++) {
                    if (this != enemyList[i] && Vector3.Distance(Position, enemyList[i].Position) < perceptionRadius) { 
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

        protected void calculateFutureBoundingSphere() {
            Vector3 futurePosition = Position + speed * headingDirection;
            futureBoundingSphere = new BoundingSphere(futurePosition, BoundingSphere.Radius);
        }

        // Execute the actions
        protected virtual void makeAction(int changeDirection, SwimmingObject[] enemies, int enemiesAmount, SwimmingObject[] fishes, int fishAmount, List<DamageBullet> bullets, Tank tank) {
            if (configBits[0] == true) {
                // swimming w/o attacking
                if (!clipPlayer.inRange(1, 30) && !configBits[3])
                    clipPlayer.switchRange(1, 30);
                randomWalk(changeDirection, enemies, enemiesAmount, fishes, fishAmount, tank);
                return;
            }
            if (currentHuntingTarget != null) { 
                calculateHeadingDirection();
                calculateFutureBoundingSphere();
            }
            if (configBits[2] == true) {
                // swimming w/o attacking
                if (!clipPlayer.inRange(1, 30) && !configBits[3])
                    clipPlayer.switchRange(1, 30);
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

                if (currentHuntingTarget is BaseEnemy)
                {
                    BaseEnemy tmp = (BaseEnemy)currentHuntingTarget;
                    if (tmp.health <= 0)
                    {
                        currentHuntingTarget = null;
                        return;
                    }
                }

                if (PlayGameScene.timming.TotalGameTime.TotalSeconds - prevFire.TotalSeconds > timeBetweenFire) {
                    if (!clipPlayer.inRange(31, 60))
                        clipPlayer.switchRange(31, 60);
                    if (currentHuntingTarget.GetType().Name.Equals("Tank")) {
                        //((Tank)currentHuntingTarget).currentHitPoint -= damage;
                        Tank.currentHitPoint -= damage;
                    }
                    if (currentHuntingTarget is SwimmingObject) {
                        ((SwimmingObject)currentHuntingTarget).health -= damage;
                    }
                    prevFire = PlayGameScene.timming.TotalGameTime;
                }
            }
        }

        //public override void Draw(Matrix view, Matrix projection)
        //{
        //    Matrix[] transforms = new Matrix[Model.Bones.Count];
        //    Model.CopyAbsoluteBoneTransformsTo(transforms);
        //    //Matrix translateMatrix = Matrix.CreateTranslation(Position);
        //    //Matrix worldMatrix = translateMatrix;
        //    Matrix worldMatrix = Matrix.Identity;
        //    Matrix rotationYMatrix = Matrix.CreateRotationY(ForwardDirection);
        //    Matrix translateMatrix = Matrix.CreateTranslation(Position);
        //    worldMatrix = rotationYMatrix * translateMatrix;

        //    foreach (ModelMesh mesh in Model.Meshes)
        //    {
        //        foreach (BasicEffect effect in mesh.Effects)
        //        {
        //            effect.World =
        //                worldMatrix * transforms[mesh.ParentBone.Index];
        //            effect.View = view;
        //            effect.Projection = projection;
        //            if (isHypnotise) {
        //                effect.DiffuseColor = Color.Black.ToVector3();
        //            } else 
        //                effect.DiffuseColor = Color.Green.ToVector3();

        //            effect.EnableDefaultLighting();
        //            effect.PreferPerPixelLighting = true;

        //            effect.FogEnabled = true;
        //            effect.FogStart = GameConstants.FogStart;
        //            effect.FogEnd = GameConstants.FogEnd;
        //            effect.FogColor = GameConstants.FogColor.ToVector3();
        //        }
        //        mesh.Draw();
        //    }
        //}
    }
}