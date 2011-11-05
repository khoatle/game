using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon
{
    public class Collision
    {
        /// <summary>
        /// GENERAL FUNCTIONS
        /// </summary>
        public static bool isOutOfView(BoundingSphere boundingSphere, BoundingFrustum frustum)
        {
            if (boundingSphere.Intersects(frustum))
                return false;
            return true;
        }

        public static bool isOutOfMap(Vector3 futurePosition, int MaxRangeX, int MaxRangeZ)
        {
            return Math.Abs(futurePosition.X) > MaxRangeX ||
                Math.Abs(futurePosition.Z) > MaxRangeZ;
        }

        public static void deleteSmallerThanZero(SwimmingObject[] objs, ref int size, BoundingFrustum cameraFrustum) {
            for (int i = 0; i < size; i++) {
                if (objs[i].health <= 0) {
                    if (objs[i].isBigBoss == true) PlayGameScene.isBossKilled = true;

                    if (objs[i] is BaseEnemy) {
                        Tank.currentExperiencePts += objs[i].experienceReward;
                        if (!objs[i].isBigBoss)
                        {
                            if (objs[i].BoundingSphere.Intersects(cameraFrustum))
                                PoseidonGame.audio.hunterYell.Play();
                        }
                        else
                        {
                            if (objs[i] is MutantShark)
                                PoseidonGame.audio.mutantSharkYell.Play();
                            else if (objs[i] is Terminator)
                                PoseidonGame.audio.terminatorYell.Play();
                        }
                    }

                    for (int k = i; k < size-1; k++) {
                        objs[k] = objs[k+1];
                    }
                    objs[--size] = null;

                    if (objs[i] is Fish)
                    {
                        Tank.currentEnvPoint -= GameConstants.envLossForFishDeath;
                    }
                }
            }
        }
        // End-----------------------------------------------------

        ///PLANT FUNCTIONS
        public static bool isPlantPositionValid(Plant plant, List<Plant> plants, List<ShipWreck> shipwrecks, List<StaticObject> staticObjects)
        {
            if (isPlantvsPlantCollision(plant.BoundingSphere, plants))
            {
                return false;
            }
            if (isPlantvsShipwreckCollision(plant.BoundingSphere, shipwrecks))
            {
                return false;
            }
            if (isPlantvsStaticObjectCollision(plant.BoundingSphere, staticObjects)) {
                return false;
            }

            return true;
        }

        // Helper
        private static bool isPlantvsPlantCollision(BoundingSphere plantBoundingSphere, List<Plant> plants)
        {
            for (int i = 0; i < plants.Count; i++)
            {
                if (plantBoundingSphere.Intersects(
                    plants[i].BoundingSphere))
                    return true;
            }
            return false;
        }
        // Helper
        private static bool isPlantvsStaticObjectCollision(BoundingSphere plantBoundingSphere, List<StaticObject> staticObjects)
        {
            for (int i = 0; i < staticObjects.Count; i++)
            {
                if (plantBoundingSphere.Intersects(
                    staticObjects[i].BoundingSphere))
                    return true;
            }
            return false;
        }
        //helper
        private static bool isPlantvsShipwreckCollision(BoundingSphere plantBoundingSphere, List<ShipWreck> shipwrecks)
        {
            BoundingSphere shipSphere;
            for (int i = 0; i < shipwrecks.Count; i++)
            {
                shipSphere = shipwrecks[i].BoundingSphere;
                shipSphere.Center = shipwrecks[i].Position;
                if (plantBoundingSphere.Intersects(
                    shipSphere))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// BARRIERS FUNCTIONS
        /// </summary>
        public static bool isBarriersValidMove(SwimmingObject obj, Vector3 futurePosition, SwimmingObject[] objects, int size, Tank tank) {
            BoundingSphere futureBoundingSphere = obj.BoundingSphere;
            futureBoundingSphere.Center.X = futurePosition.X;
            futureBoundingSphere.Center.Z = futurePosition.Z;

            if (isOutOfMap(futurePosition, tank.MaxRangeX, tank.MaxRangeZ)) {
                return false;
            }

            if (isBarrierVsBarrierCollision(obj, futureBoundingSphere, objects, size)) {
                return false;
            }

            if (isBarrierVsTankCollision(futureBoundingSphere, tank)) {
                return false;
            }
            return true;
        }

        // Helper
        public static bool isBarrierVsBarrierCollision(SwimmingObject enemy, BoundingSphere futureBoundingSphere, SwimmingObject[] objs, int size)
        {
            for (int curBarrier = 0; curBarrier < size; curBarrier++)
            {
                if (enemy.Equals(objs[curBarrier]))
                    continue;
                if (futureBoundingSphere.Intersects(
                    objs[curBarrier].BoundingSphere))
                    return true;
            }
            return false;
        }

        // Helper
        private static bool isBarrierVsTankCollision(BoundingSphere vehicleBoundingSphere, Tank tank)
        {
            if (vehicleBoundingSphere.Intersects(tank.BoundingSphere))
                return true;
            return false;
        }
        // End--------------------------------------------------------------

        /// <summary>
        /// TANK COLLISION
        /// </summary>
        public static bool isTankValidMove(Tank tank, Vector3 futurePosition, SwimmingObject[] enemies,int enemiesAmount, SwimmingObject[] fish, int fishAmount)
        {
            BoundingSphere futureBoundingSphere = tank.BoundingSphere;
            futureBoundingSphere.Center.X = futurePosition.X;
            futureBoundingSphere.Center.Z = futurePosition.Z;

            //Don't allow off-terrain driving
            if (isOutOfMap(futurePosition, tank.MaxRangeX, tank.MaxRangeZ))
            {
                return false;
            }
            //in supersonice mode, you knock and you stun the enemies
            if (Tank.supersonicMode == true)
            {
                if (isTankVsBarrierCollision(futureBoundingSphere, fish, fishAmount))
                    return false;
                return true;
            }
            //else don't allow driving through an enemy
            if (isTankVsBarrierCollision(futureBoundingSphere, enemies, enemiesAmount))
            {
                return false;
            }
            if (isTankVsBarrierCollision(futureBoundingSphere, fish, fishAmount)) {
                return false;
            }
            return true;
        }

        // Helper
        private static bool isTankVsBarrierCollision(BoundingSphere boundingSphere, SwimmingObject[] barrier, int size)
        {
            for (int i = 0; i < size; i++)
            {
                if (boundingSphere.Intersects(barrier[i].BoundingSphere))
                {
                    return true;
                }
            }
            return false;
        }
        // End----------------------------------------------------------

        /// <summary>
        /// PROJECTILES FUNCTION
        /// </summary>
        public static void updateDamageBulletVsBarriersCollision(List<DamageBullet> bullets, SwimmingObject[] barriers, ref int size, bool soundWhenHit, BoundingFrustum cameraFrustum) {
            BoundingSphere sphere;
            for (int i = 0; i < bullets.Count; i++) {
                for (int j = 0; j < size; j++) {
                    sphere = barriers[j].BoundingSphere;
                    sphere.Radius *= GameConstants.EasyHitScale;
                    //because Mutant Shark's easy hit sphere is too large
                    if (barriers[j] is MutantShark) sphere.Radius *= 0.7f;
                    if (bullets[i].BoundingSphere.Intersects(sphere))
                    {
                        if (soundWhenHit && barriers[j].BoundingSphere.Intersects(cameraFrustum))
                            PoseidonGame.audio.animalYell.Play();
                        //if (barriers[j] is BaseEnemy) {
                        //    if (((BaseEnemy)barriers[j]).isHypnotise) {
                        //        return;
                        //    }
                        //}

                        barriers[j].health -= bullets[i].damage;
                        bullets.RemoveAt(i--);
                        break;
                    }
                }
            }
        }

        // It has "BUG" at "EnemyHP", I know it.
        public static void updateHealingBulletVsBarrierCollision(List<HealthBullet> bullets, SwimmingObject[] barriers, int size, BoundingFrustum cameraFrustum) {
            BoundingSphere sphere;
            for (int i = 0; i < bullets.Count; i++) {
                for (int j = 0; j < size; j++) {
                    sphere = barriers[j].BoundingSphere;
                    sphere.Radius *= GameConstants.EasyHitScale;
                    if (bullets[i].BoundingSphere.Intersects(sphere))
                    {
                        if (barriers[j].BoundingSphere.Intersects(cameraFrustum))
                            PoseidonGame.audio.animalHappy.Play();
                        if (barriers[j].health < GameConstants.DefaultEnemyHP ) {
                            barriers[j].health += GameConstants.HealingAmount;
                            if (barriers[j].health > barriers[j].maxHealth) barriers[j].health = barriers[j].maxHealth;
                            Tank.currentExperiencePts += barriers[j].experienceReward;
                            Tank.currentEnvPoint += GameConstants.envGainForHealingFish;
                        }
                        bullets.RemoveAt(i--);
                        break;
                    }
                }
            }
        }

        public static void updateProjectileHitTank(Tank tank, List<DamageBullet> enemyBullets) {
            for (int i = 0; i < enemyBullets.Count; ) {
                if (enemyBullets[i].BoundingSphere.Intersects(tank.BoundingSphere)) {
                    if (!Tank.invincibleMode) Tank.currentHitPoint -= enemyBullets[i].damage;
                    enemyBullets.RemoveAt(i);
                    PoseidonGame.audio.botYell.Play();
                }
                else { i++;  }
            }
        }

        public static void updateBulletOutOfBound(int MaxRangeX, int MaxRangeZ, List<HealthBullet> healBullets, List<DamageBullet> tankBullets, List<DamageBullet> enemyBullets, List<DamageBullet> alliesBulleys, BoundingFrustum frustum)
        {
            for (int i = 0; i < healBullets.Count; ) {
                if (isOutOfMap(healBullets[i].Position, MaxRangeX, MaxRangeZ) || isOutOfView(healBullets[i].BoundingSphere, frustum)) {
                    healBullets.RemoveAt(i);
                }
                else {
                    i++;
                }
            }

            for (int i = 0; i < tankBullets.Count; ) {
                if (isOutOfMap(tankBullets[i].Position, MaxRangeX, MaxRangeZ) || isOutOfView(tankBullets[i].BoundingSphere, frustum))
                {
                    tankBullets.RemoveAt(i);
                }
                else {
                    i++;
                }
            }

            for (int i = 0; i < enemyBullets.Count; ) {
                if (isOutOfMap(enemyBullets[i].Position, MaxRangeX, MaxRangeZ) || isOutOfView(enemyBullets[i].BoundingSphere, frustum)) {
                    enemyBullets.RemoveAt(i);
                } else {
                    i++;
                }
            }

            for (int i = 0; i < alliesBulleys.Count; ) {
                if (isOutOfMap(alliesBulleys[i].Position, MaxRangeX, MaxRangeZ) || isOutOfView(alliesBulleys[i].BoundingSphere, frustum)) {
                    alliesBulleys.RemoveAt(i);
                } else {
                    i++;
                }
            }
        }
        // End---------------------------------------------------------------
    }
}