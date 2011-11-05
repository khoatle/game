using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon
{
    public class CombatEnemy : BaseEnemy
    {
        private BoundingSphere futureBoundingSphere;

        // Percept ID:
        // 0 = nothing detected
        // 1 = tank detected - 1st priory
        // 2 = last target is still in range - 2nd priory
        // 3 = a fish is detected - 3rd priory

        // Move bits:
        // 1st bit: randomWalk
        // 2nd bit: standstill
        // 3rd bit: chasing
        // 4th bit: is striking
        public CombatEnemy()
            : base()
        {
            perceptID = new int[] { 0, 1, 2, 3 };
            configBits = new bool[] { false, false, false, false };
            damage = GameConstants.DefaultEnemyDamage * 3;
            perceptionRadius *= 2;
            isHypnotise = false;
        }

        public override void Update(SwimmingObject[] enemyList, int enemySize, SwimmingObject[] fishList, int fishSize, int changeDirection, Tank tank, List<DamageBullet> enemyBullets, List<DamageBullet> alliesBullets, BoundingFrustum cameraFrustum, GameTime gameTime)
        {
            qRotation = Quaternion.CreateFromAxisAngle(
                            Vector3.Up,
                            ForwardDirection);
            enemyMatrix = Matrix.CreateScale(0.1f) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                                Matrix.CreateFromQuaternion(qRotation) *
                                Matrix.CreateTranslation(Position);
            clipPlayer.update(gameTime.ElapsedGameTime, true, enemyMatrix);

            if (stunned)
            {
                if (!clipPlayer.inRange(1, 30))
                    clipPlayer.switchRange(1, 30);
                return;
            }

            if (isHypnotise && gameTime.TotalGameTime.TotalSeconds - startHypnotiseTime.TotalSeconds > GameConstants.timeHypnotiseLast)
            {
                wearOutHypnotise();
            }
            if (!isHypnotise)
            {
                int perceptionID = perceptAndLock(tank, fishList, fishSize);
                configAction(perceptionID, gameTime);
                makeAction(changeDirection, enemyList, enemySize, fishList, fishSize, enemyBullets, tank, cameraFrustum, gameTime);
            }
            else
            {
                int perceptionID = perceptAndLock(tank, enemyList, enemySize);
                configAction(perceptionID, gameTime);
                makeAction(changeDirection, enemyList, enemySize, fishList, fishSize, enemyBullets, tank, cameraFrustum, gameTime);
            }
        }

        protected int perceptAndLock(Tank tank, SwimmingObject[] enemyList, int enemySize)
        {
            if (!isHypnotise && Vector3.Distance(Position, tank.Position) < perceptionRadius)
            {
                currentHuntingTarget = tank;
                return perceptID[1];
            }
            else
            {
                if (currentHuntingTarget != null
                        && Vector3.Distance(Position, currentHuntingTarget.Position) < perceptionRadius)
                {
                    return perceptID[2];
                }

                for (int i = 0; i < enemySize; i++)
                {
                    if (this != enemyList[i] && Vector3.Distance(Position, enemyList[i].Position) < perceptionRadius)
                    {
                        currentHuntingTarget = enemyList[i];
                        return perceptID[3];
                    }
                }
                return perceptID[0];
            }
        }

        protected void configAction(int perception, GameTime gameTime)
        {
            if (perception == perceptID[0])
            {
                if (currentHuntingTarget != null && clearMind(gameTime) == false)
                {
                    configBits[0] = false;
                    configBits[1] = false;
                    configBits[2] = true;
                    configBits[3] = false;
                }
                else
                {
                    configBits[0] = true;
                    configBits[1] = false;
                    configBits[2] = false;
                    configBits[3] = false;
                }
            }
            else if (perception == perceptID[1] || perception == perceptID[2] || perception == perceptID[3])
            {
                configBits[0] = false;
                configBits[2] = true;

                calculateFutureBoundingSphere();

                configBits[3] = (Vector3.Distance(Position, currentHuntingTarget.Position) < BoundingSphere.Radius + currentHuntingTarget.BoundingSphere.Radius + 3f) ? true : false;
                configBits[1] = configBits[3];
            }
        }

        protected void calculateFutureBoundingSphere()
        {
            Vector3 futurePosition = Position + speed * headingDirection;
            futureBoundingSphere = new BoundingSphere(futurePosition, BoundingSphere.Radius);
        }

        // Execute the actions
        protected virtual void makeAction(int changeDirection, SwimmingObject[] enemies, int enemiesAmount, SwimmingObject[] fishes, int fishAmount, List<DamageBullet> bullets, Tank tank, BoundingFrustum cameraFrustum, GameTime gameTime)
        {
            if (configBits[0] == true)
            {
                // swimming w/o attacking
                if (!clipPlayer.inRange(1, 30) && !configBits[3])
                    clipPlayer.switchRange(1, 30);
                randomWalk(changeDirection, enemies, enemiesAmount, fishes, fishAmount, tank);
                return;
            }
            if (currentHuntingTarget != null)
            {
                calculateHeadingDirection();
                calculateFutureBoundingSphere();
            }
            if (configBits[2] == true)
            {
                // swimming w/o attacking
                if (!clipPlayer.inRange(1, 30) && !configBits[3])
                    clipPlayer.switchRange(1, 30);
                goStraight(enemies, enemiesAmount, fishes, fishAmount, tank);
            }
            if (configBits[3] == true)
            {
                startChasingTime = gameTime.TotalGameTime;

                if (currentHuntingTarget.GetType().Name.Equals("Fish"))
                {
                    Fish tmp = (Fish)currentHuntingTarget;
                    if (tmp.health <= 0)
                    {
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

                if (gameTime.TotalGameTime.TotalSeconds - prevFire.TotalSeconds > timeBetweenFire)
                {
                    if (!clipPlayer.inRange(31, 60))
                        clipPlayer.switchRange(31, 60);
                    if (currentHuntingTarget.GetType().Name.Equals("Tank"))
                    {
                        if (!Tank.invincibleMode)
                        {
                            Tank.currentHitPoint -= damage;
                            PoseidonGame.audio.botYell.Play();
                        }
                    }
                    if (currentHuntingTarget is SwimmingObject)
                    {
                        ((SwimmingObject)currentHuntingTarget).health -= damage;
                        if (currentHuntingTarget.BoundingSphere.Intersects(cameraFrustum))
                        {
                            PoseidonGame.audio.animalYell.Play();
                        }
                    }
                    prevFire = gameTime.TotalGameTime;

                    if (this.BoundingSphere.Intersects(cameraFrustum))
                        PoseidonGame.audio.slashSound.Play();
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