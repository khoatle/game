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
        public static bool isOutOfView(Vector3 futurePosition, Viewport view)
        {
            return Math.Abs(futurePosition.X) > view.Width / 2 ||
                Math.Abs(futurePosition.Z) > view.Height / 2;
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
        public static bool isBarrierValidMove(Barrier barrier, Vector3 futurePosition, Barrier[] barriers, int size, Tank tank)
        {
            BoundingSphere futureBoundingSphere = barrier.BoundingSphere;
            futureBoundingSphere.Center.X = futurePosition.X;
            futureBoundingSphere.Center.Z = futurePosition.Z;

            if (isOutOfMap(futurePosition))
            {
                return false;
            }
            if (isBarrierVsBarrierCollision(barrier, futureBoundingSphere, barriers, size))
            {
                return false;
            }
            if (isBarrierVsTankCollision(futureBoundingSphere, tank))
            {
                return false;
            }
            return true;
        }

        // Helper
        private static bool isBarrierVsBarrierCollision(Barrier barrier, BoundingSphere vehicleBoundingSphere, Barrier[] barriers, int size)
        {
            for (int curBarrier = 0; curBarrier < size; curBarrier++)
            {
                if (barrier.Equals(barriers[curBarrier]))
                    continue;
                if (vehicleBoundingSphere.Intersects(
                    barriers[curBarrier].BoundingSphere))
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
        public static bool isTankValidMove(Tank tank, Vector3 futurePosition, Barrier[] barriers, int size)
        {
            BoundingSphere futureBoundingSphere = tank.BoundingSphere;
            futureBoundingSphere.Center.X = futurePosition.X;
            futureBoundingSphere.Center.Z = futurePosition.Z;

            //Don't allow off-terrain driving
            if (isOutOfMap(futurePosition))
            {
                return false;
            }
            //Don't allow driving through a barrier
            if (isTankVsBarrierCollision(futureBoundingSphere, barriers, size))
            {
                return false;
            }

            return true;
        }

        // Helper
        private static bool isTankVsBarrierCollision(BoundingSphere boundingSphere, Barrier[] barrier, int size)
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
        public static void updateBulletVsBarriersCollision(List<Projectiles> projectiles, Barrier[] barriers, ref int size)
        {
            for (int i = 0; i < projectiles.Count; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (projectiles[i].BoundingSphere.Intersects(barriers[j].BoundingSphere))
                    {
                        barriers[j].health -= projectiles[i].bulletDamage;
                        if (barriers[j].health <= 0)
                        {
                            barriers[j] = null;
                            for (int k = j + 1; k < size; k++)
                            {
                                barriers[k - 1] = barriers[k];
                            }
                            size--;
                        }

                        projectiles.RemoveAt(i--);
                        break;
                    }
                }
            }
        }

        public static void updateBulletOutOfBound(List<Projectiles> projectiles, Viewport view)
        {
            for (int i = 0; i < projectiles.Count; )
            {
                if (isOutOfMap(projectiles[i].Position) || isOutOfView(projectiles[i].Position, view))
                {
                    projectiles.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        //public static void updateBulletHitMortals(List<DamageBullet> projectiles, Enemy[] enemies, Fish[] fish) {
        //    for (int i = 0; i < projectiles.Count; ) {
        //        int index = isBulletHitEnemy(projectiles[i], enemies);

        //        if (index == -1) {
        //            index = isBulletHitFish(projectiles[i], fish);
        //            if (index != -1) {
        //                fish[index].health -= projectiles[i].damage;
        //            }
        //        }
        //        else {
        //            enemies[index].health -= projectiles[i].damage;
        //        }
        //    }
        //}

        //public static void updateMortals(Enemy[] enemies, Fish[] fish) {
        //    for (int i = 0; i < GameConstants.NumberFish; i++) {
        //        if (fish[i].health <= 0) {
        //            fish[i] = null;
        //        }
        //    }

        //    for (int i = 0; i < GameConstants.NumberEnemies; i++) {
        //        if (enemies[i].health <= 0) {
        //            enemies[i] = null;
        //        }
        //    }
        //}

        //// Helper
        //private static int isBulletHitEnemy(Projectiles projectile, Enemy[] enemies) {
        //    for (int i = 0; i < GameConstants.NumberEnemies; i++) {
        //        if (projectile.BoundingSphere.Intersects(enemies[i].BoundingSphere)) {
        //            return i;
        //        }
        //    }
        //    return -1;
        //}

        //// Helper
        //private static int isBulletHitFish(Projectiles projectile, Fish[] fish)
        //{
        //    for (int i = 0; i < GameConstants.NumberFish; i++) {
        //        if (projectile.BoundingSphere.Intersects(fish[i].BoundingSphere)) {
        //            return i;
        //        }
        //    }
        //    return -1;
        //}
        // End---------------------------------------------------------------
    }
}