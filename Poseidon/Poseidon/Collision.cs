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

        public static bool isOutOfMap(Vector3 futurePosition)
        {
            return Math.Abs(futurePosition.X) > GameConstants.MaxRange ||
                Math.Abs(futurePosition.Z) > GameConstants.MaxRange;
        }
        // End-----------------------------------------------------

        ///PLANT FUNCTIONS
        public static bool isPlantPositionValid(Plant plant, List<Plant> plants, List<ShipWreck> shipwrecks)
        {
            if (isPlantvsPlantCollision(plant.BoundingSphere, plants))
            {
                return false;
            }
            if (isPlantvsShipwreckCollision(plant.BoundingSphere, shipwrecks))
            {
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
        public static bool isBarriersValidMove(SwimmingObject objs, Vector3 futurePosition, SwimmingObject[] objects, int size, Tank tank) {
            BoundingSphere futureBoundingSphere = objs.BoundingSphere;
            futureBoundingSphere.Center.X = futurePosition.X;
            futureBoundingSphere.Center.Z = futurePosition.Z;

            if (isOutOfMap(futurePosition)) {
                return false;
            }

            if (isBarrierVsBarrierCollision(objs, futureBoundingSphere, objects, size)) {
                return false;
            }

            if (isBarrierVsTankCollision(objs.BoundingSphere, tank)) {
                return false;
            }
            return true;
        }

        // Helper
        public static bool isBarrierVsBarrierCollision(SwimmingObject enemy, BoundingSphere vehicleBoundingSphere, SwimmingObject[] objs, int size)
        {
            for (int curBarrier = 0; curBarrier < size; curBarrier++)
            {
                if (enemy.Equals(objs[curBarrier]))
                    continue;
                if (vehicleBoundingSphere.Intersects(
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
            if (isOutOfMap(futurePosition))
            {
                return false;
            }
            //in supersonice mode, you knock and you stun the enemies
            if (tank.supersonicMode == true)
            {
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
        public static void updateDamageBulletVsBarriersCollision(List<DamageBullet> bullets, SwimmingObject[] barriers, ref int size) {
            for (int i = 0; i < bullets.Count; i++) {
                for (int j = 0; j < size; j++) {
                    if (bullets[i].BoundingSphere.Intersects(barriers[j].BoundingSphere)) {
                        barriers[j].health -= bullets[i].damage;
                        if (barriers[j].health <= 0) {
                            for (int k = j + 1; k < size; k++) {
                                barriers[k - 1] = barriers[k];
                            }
                            barriers[--size] = null;
                        }
                        bullets.RemoveAt(i--);
                        break;
                    }
                }
            }
        }

        // It has "BUG" at "EnemyHP", I know it.
        public static void updateHealingBulletVsBarrierCollision(List<HealthBullet> bullets, SwimmingObject[] barriers, int size) {
            for (int i = 0; i < bullets.Count; i++) {
                for (int j = 0; j < size; j++) {
                    if (bullets[i].BoundingSphere.Intersects(barriers[j].BoundingSphere)) {
                        if (barriers[j].health < GameConstants.EnemyHP) {
                            barriers[j].health += GameConstants.HealingAmount;
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
                    if (!tank.invincibleMode) tank.hitPoint -= enemyBullets[i].damage;
                    enemyBullets.RemoveAt(i);
                }
                else { i++;  }
            }
        }

        public static void updateBulletOutOfBound(List<HealthBullet> heals, List<DamageBullet> dams, BoundingFrustum frustum)
        {
            for (int i = 0; i < heals.Count; ) {
                if (isOutOfMap(heals[i].Position) || isOutOfView(heals[i].BoundingSphere, frustum)) {
                    heals.RemoveAt(i);
                }
                else {
                    i++;
                }
            }

            for (int i = 0; i < dams.Count; ) {
                if (isOutOfMap(dams[i].Position) || isOutOfView(dams[i].BoundingSphere, frustum))
                {
                    dams.RemoveAt(i);
                }
                else {
                    i++;
                }
            }
        }
        // End---------------------------------------------------------------
    }
}