using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon
{
    public class Collision {

        /// <summary>
        /// GENERAL FUNCTIONS
        /// </summary>
        public static bool isOutOfView(Vector3 futurePosition, Viewport view) {
            return Math.Abs(futurePosition.X) > view.Width/2 ||
                Math.Abs(futurePosition.Z) > view.Height/2;        
        }
        
        public static bool isOutOfMap(Vector3 futurePosition) {
            return Math.Abs(futurePosition.X) > GameConstants.MaxRange ||
                Math.Abs(futurePosition.Z) > GameConstants.MaxRange;
        }
        // End-----------------------------------------------------

        /// <summary>
        /// BARRIERS FUNCTIONS
        /// </summary>
        public static bool isBarrierValidMove(Barrier barrier, Vector3 futurePosition, Barrier[] barriers, Tank tank) {
            BoundingSphere futureBoundingSphere = barrier.BoundingSphere;
            futureBoundingSphere.Center.X = futurePosition.X;
            futureBoundingSphere.Center.Z = futurePosition.Z;

            if (isOutOfMap(futurePosition)) {
                return false;
            }
            if (isBarrierVsBarrierCollision(barrier, futureBoundingSphere, barriers)) {
                return false;
            }
            if (isBarrierVsTankCollision(futureBoundingSphere, tank)) {
                return false;
            }
            return true; 
        }

        // Helper
        private static bool isBarrierVsBarrierCollision(Barrier barrier, BoundingSphere vehicleBoundingSphere, Barrier[] barriers)
        {
            for (int curBarrier = 0; curBarrier < barriers.Length; curBarrier++)
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
        public static bool isTankValidMove(Tank tank, Vector3 futurePosition, Barrier[] barriers) {
            BoundingSphere futureBoundingSphere = tank.BoundingSphere;
            futureBoundingSphere.Center.X = futurePosition.X;
            futureBoundingSphere.Center.Z = futurePosition.Z;

            //Don't allow off-terrain driving
            if (isOutOfMap(futurePosition)) {
                return false;
            }
            //Don't allow driving through a barrier
            if (isTankVsBarrierCollision(futureBoundingSphere, barriers)) {
                return false;
            }

            return true;
        }

        // Helper
        private static bool isTankVsBarrierCollision(BoundingSphere boundingSphere, Barrier[] barrier) {
            for (int i = 0; i < GameConstants.NumBarriers; i++) {
                if (boundingSphere.Intersects(barrier[i].BoundingSphere)) {
                    return true;
                }
            }
            return false;
        }
        // End----------------------------------------------------------

        /// <summary>
        /// PROJECTILES FUNCTION
        /// </summary>
        public static void updateBulletVsBarriersCollision(List<Projectiles> projectiles, Barrier[] barriers) {
            for (int i = 0; i < projectiles.Count; i++) { 
                for (int j = 0; j < GameConstants.NumBarriers; j++) {
                    if (projectiles[i].BoundingSphere.Intersects(barriers[j].BoundingSphere)) {
                        projectiles.RemoveAt(i--);
                        break;
                    }
                }
            }
        }

        public static void updateBulletOutOfBound(List<Projectiles> projectiles, Viewport view) {
            for (int i = 0; i < projectiles.Count; ) {
                if (isOutOfMap(projectiles[i].Position) || isOutOfView(projectiles[i].Position, view)) {
                    projectiles.RemoveAt(i);
                }
                else { 
                    i++; 
                }
            }
        }

        public static void updateBulletHitMortals(List<DamageBullet> projectiles, Enemy[] enemies, Fish[] fish) {
            for (int i = 0; i < projectiles.Count; ) {
                int index = isBulletHitEnemy(projectiles[i], enemies);

                if (index == -1) {
                    index = isBulletHitFish(projectiles[i], fish);
                    if (index != -1) {
                        fish[index].health -= projectiles[i].damage;
                    }
                }
                else {
                    enemies[index].health -= projectiles[i].damage;
                }
            }
        }

        public static void updateMortals(Enemy[] enemies, Fish[] fish) {
            for (int i = 0; i < GameConstants.NumberFish; i++) {
                if (fish[i].health <= 0) {
                    fish[i] = null;
                }
            }

            for (int i = 0; i < GameConstants.NumberEnemies; i++) {
                if (enemies[i].health <= 0) {
                    enemies[i] = null;
                }
            }
        }

        // Helper
        private static int isBulletHitEnemy(Projectiles projectile, Enemy[] enemies) {
            for (int i = 0; i < GameConstants.NumberEnemies; i++) {
                if (projectile.BoundingSphere.Intersects(enemies[i].BoundingSphere)) {
                    return i;
                }
            }
            return -1;
        }

        // Helper
        private static int isBulletHitFish(Projectiles projectile, Fish[] fish)
        {
            for (int i = 0; i < GameConstants.NumberFish; i++) {
                if (projectile.BoundingSphere.Intersects(fish[i].BoundingSphere)) {
                    return i;
                }
            }
            return -1;
        }
        // End---------------------------------------------------------------
    }
}