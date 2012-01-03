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

        public static void deleteSmallerThanZero(SwimmingObject[] objs, ref int size, BoundingFrustum cameraFrustum, GameMode gameMode, Cursor cursor)
        {
            for (int i = 0; i < size; i++) {
                if (objs[i].health <= 0 && objs[i].gaveExp == false) {
                    if (objs[i].isBigBoss == true)
                    {
                        if (gameMode == GameMode.MainGame)
                            PlayGameScene.isBossKilled = true;
                        else if (gameMode == GameMode.SurvivalMode && objs[i] is Fish)
                            SurvivalGameScene.isAncientKilled = true;
                    }
                    if (objs[i] is BaseEnemy) {
                        HydroBot.currentExperiencePts += objs[i].basicExperienceReward;
                        objs[i].gaveExp = true;
                        if (gameMode == GameMode.SurvivalMode)
                            SurvivalGameScene.score += objs[i].basicExperienceReward / 2;
                        if (objs[i].BoundingSphere.Intersects(cameraFrustum))
                        {
                            Point point = new Point();
                            String point_string = "+" + objs[i].basicExperienceReward.ToString() + "EXP";
                            point.LoadContent(PoseidonGame.contentManager, point_string, objs[i].Position, Color.LawnGreen);
                            if (gameMode == GameMode.ShipWreck)
                                ShipWreckScene.points.Add(point);
                            else if (gameMode == GameMode.MainGame)
                                PlayGameScene.points.Add(point);
                            else if (gameMode == GameMode.SurvivalMode)
                                SurvivalGameScene.points.Add(point);
                        }

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

                    if (objs[i] is Fish)
                    {
                        int envLoss;
                        envLoss = GameConstants.envLossForFishDeath + 5*HydroBot.gamePlusLevel;
                        HydroBot.currentEnvPoint -= envLoss;
                        if (objs[i].BoundingSphere.Intersects(cameraFrustum))
                        {
                            Point point = new Point();
                            String point_string = "-" + envLoss.ToString() + "ENV";
                            point.LoadContent(PoseidonGame.contentManager, point_string, objs[i].Position, Color.Red);
                            if (gameMode == GameMode.ShipWreck)
                                ShipWreckScene.points.Add(point);
                            else if (gameMode == GameMode.MainGame)
                                PlayGameScene.points.Add(point);
                            else if (gameMode == GameMode.SurvivalMode)
                                SurvivalGameScene.points.Add(point);
                        }
                    }
                    if (objs[i] == cursor.targetToLock)
                    {
                        cursor.targetToLock = null;
                        //objs[i] = null;
                    }

                    //if we are playing the survival mode
                    //revive the dead enemy instead
                    if (gameMode != GameMode.SurvivalMode)
                    {
                        objs[i] = null;
                        for (int k = i; k < size - 1; k++)
                        {
                            objs[k] = objs[k + 1];
                        }
                        objs[--size] = null;
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
            if (staticObjects != null)
            {
                for (int i = 0; i < staticObjects.Count; i++)
                {
                    if (plantBoundingSphere.Intersects(
                        staticObjects[i].BoundingSphere))
                        return true;
                }
            }
            return false;
        }
        //helper
        private static bool isPlantvsShipwreckCollision(BoundingSphere plantBoundingSphere, List<ShipWreck> shipwrecks)
        {
            if (shipwrecks != null)
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
            }
            return false;
        }

        /// <summary>
        /// BARRIERS FUNCTIONS
        /// </summary>
        public static bool isBarriersValidMove(SwimmingObject obj, Vector3 futurePosition, SwimmingObject[] objects, int size, HydroBot hydroBot) {
            BoundingSphere futureBoundingSphere = obj.BoundingSphere;
            futureBoundingSphere.Center.X = futurePosition.X;
            futureBoundingSphere.Center.Z = futurePosition.Z;

            if (isOutOfMap(futurePosition, hydroBot.MaxRangeX, hydroBot.MaxRangeZ)) {
                return false;
            }

            if (isBarrierVsBarrierCollision(obj, futureBoundingSphere, objects, size)) {
                return false;
            }

            if (isBarrierVsBotCollision(futureBoundingSphere, hydroBot)) {
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
                {
                    //System.Diagnostics.Debug.WriteLine("I am "+enemy.Name+" stuck with "+ objs[curBarrier].Name);
                    return true;
                }
            }
            return false;
        }

        // Helper
        private static bool isBarrierVsBotCollision(BoundingSphere vehicleBoundingSphere, HydroBot hydroBot)
        {
            if (vehicleBoundingSphere.Intersects(hydroBot.BoundingSphere))
                return true;
            return false;
        }
        // End--------------------------------------------------------------

        /// <summary>
        /// BOT COLLISION
        /// </summary>
        public static bool isBotValidMove(HydroBot hydroBot, Vector3 futurePosition, SwimmingObject[] enemies,int enemiesAmount, SwimmingObject[] fish, int fishAmount)
        {
            BoundingSphere futureBoundingSphere = hydroBot.BoundingSphere;
            futureBoundingSphere.Center.X = futurePosition.X;
            futureBoundingSphere.Center.Z = futurePosition.Z;

            //Don't allow off-terrain driving
            if (isOutOfMap(futurePosition, hydroBot.MaxRangeX, hydroBot.MaxRangeZ))
            {
                return false;
            }
            //in supersonice mode, you knock and you stun the enemies
            if (HydroBot.supersonicMode == true)
            {
                if (isBotVsBarrierCollision(futureBoundingSphere, fish, fishAmount))
                    return false;
                return true;
            }
            //else don't allow driving through an enemy
            if (isBotVsBarrierCollision(futureBoundingSphere, enemies, enemiesAmount))
            {
                return false;
            }
            if (isBotVsBarrierCollision(futureBoundingSphere, fish, fishAmount)) {
                return false;
            }
            return true;
        }

        // Helper
        public static bool isBotVsBarrierCollision(BoundingSphere boundingSphere, SwimmingObject[] barrier, int size)
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
        /* scene --> 1-playgamescene, 2-shipwreckscene */
        /* switch to use GameMode instead, look at the beginning of PoseidonGame for more details */
        public static void updateDamageBulletVsBarriersCollision(List<DamageBullet> bullets, SwimmingObject[] barriers, ref int size, BoundingFrustum cameraFrustum, GameMode gameMode, GameTime gameTime, HydroBot hydroBot, BaseEnemy[] enemies, int enemiesAmount, Fish[] fishes, int fishAmount, Camera gameCamera) {
            BoundingSphere sphere;
            for (int i = 0; i < bullets.Count; i++) {
                for (int j = 0; j < size; j++) {
                    sphere = barriers[j].BoundingSphere;
                    sphere.Radius *= GameConstants.EasyHitScale;
                    //because Mutant Shark's easy hit sphere is too large
                    if (barriers[j] is MutantShark) sphere.Radius *= 0.7f;
                    if (bullets[i].BoundingSphere.Intersects(sphere))
                    {
                        if (barriers[j] is Fish && barriers[j].BoundingSphere.Intersects(cameraFrustum))
                            PoseidonGame.audio.animalYell.Play();
                        if (barriers[j] is BaseEnemy)
                        {
                            if (((BaseEnemy)barriers[j]).isHypnotise)
                            {
                                continue;
                            }
                            else {
                                ((BaseEnemy)barriers[j]).justBeingShot = true;
                                ((BaseEnemy)barriers[j]).startChasingTime = PoseidonGame.playTime;
                            }
                            //special handling for the skill combo FlyingHammer
                            if (bullets[i] is FlyingHammer)
                            {
                                PoseidonGame.audio.bodyHit.Play();
                                Vector3 oldPosition = barriers[j].Position;
                                Vector3 pushVector = barriers[j].Position - bullets[i].Position;
                                pushVector.Normalize();
                                ((BaseEnemy)barriers[j]).stunned = true;
                                ((BaseEnemy)barriers[j]).stunnedStartTime = PoseidonGame.playTime.TotalSeconds;
                                ((BaseEnemy)barriers[j]).Position += (pushVector * GameConstants.ThorPushFactor);
                                barriers[j].Position.X = MathHelper.Clamp(barriers[j].Position.X, -hydroBot.MaxRangeX, hydroBot.MaxRangeX);
                                barriers[j].Position.Z = MathHelper.Clamp(barriers[j].Position.Z, -hydroBot.MaxRangeZ, hydroBot.MaxRangeZ);
                                barriers[j].BoundingSphere.Center = barriers[j].Position;
                                if (Collision.isBarrierVsBarrierCollision(barriers[j], barriers[j].BoundingSphere, fishes, fishAmount)
                                    || Collision.isBarrierVsBarrierCollision(barriers[j], barriers[j].BoundingSphere, enemies, enemiesAmount))
                                {
                                    barriers[j].Position = oldPosition;
                                    barriers[j].BoundingSphere.Center = oldPosition;
                                }
                                if (PoseidonGame.playTime.TotalSeconds - ((FlyingHammer)bullets[i]).timeShot > 0.75)
                                {
                                    ((FlyingHammer)bullets[i]).explodeNow = true;
                                    PoseidonGame.audio.Explo1.Play();
                                    gameCamera.Shake(25f, .4f);
                                    CastSkill.UseThorHammer(bullets[i].Position, hydroBot.MaxRangeX, hydroBot.MaxRangeZ, enemies, ref enemiesAmount, fishes, fishAmount, hydroBot.gameMode);
                                }
                            }
                        }

                        barriers[j].health -= bullets[i].damage;

                        if (barriers[j].BoundingSphere.Intersects(cameraFrustum))
                        {
                            Point point = new Point();
                            String point_string = "-" + bullets[i].damage.ToString() + "HP";
                            point.LoadContent(PoseidonGame.contentManager, point_string, barriers[j].Position, Color.DarkRed);
                            if (gameMode == GameMode.ShipWreck)
                                ShipWreckScene.points.Add(point);
                            else if (gameMode == GameMode.MainGame)
                                PlayGameScene.points.Add(point);
                            else if (gameMode == GameMode.SurvivalMode)
                                SurvivalGameScene.points.Add(point);
                        }
                        if (bullets[i] is FlyingHammer)
                        {
                            if (((FlyingHammer)bullets[i]).explodeNow) bullets.RemoveAt(i--); 
                        }
                        else bullets.RemoveAt(i--);
                        break;
                    }
                }
            }
        }

        public static void updateHealingBulletVsBarrierCollision(List<HealthBullet> bullets, SwimmingObject[] barriers, int size, BoundingFrustum cameraFrustum, GameMode gameMode) {
            BoundingSphere sphere;
            for (int i = 0; i < bullets.Count; i++) {
                for (int j = 0; j < size; j++) {
                    sphere = barriers[j].BoundingSphere;
                    sphere.Radius *= GameConstants.EasyHitScale;
                    if (bullets[i].BoundingSphere.Intersects(sphere))
                    {
                        if (barriers[j].BoundingSphere.Intersects(cameraFrustum))
                            PoseidonGame.audio.animalHappy.Play();

                        if (barriers[j].health < barriers[j].maxHealth ) {
                            barriers[j].health += bullets[i].healthAmount;
                            if (barriers[j].health > barriers[j].maxHealth) barriers[j].health = barriers[j].maxHealth;

                            int expReward = (int) (((double)bullets[i].healthAmount / (double)GameConstants.HealingAmount) * barriers[j].basicExperienceReward);
                            int envReward = (int) (((double)bullets[i].healthAmount / (double)GameConstants.HealingAmount) * GameConstants.BasicEnvGainForHealingFish);

                            HydroBot.currentExperiencePts += expReward;
                            HydroBot.currentEnvPoint += envReward;

                            Point point = new Point();
                            String point_string = "+" + envReward.ToString() + "ENV\n+"+expReward.ToString()+"EXP";
                            point.LoadContent(PoseidonGame.contentManager, point_string, barriers[j].Position, Color.LawnGreen);
                            if (gameMode == GameMode.ShipWreck)
                                ShipWreckScene.points.Add(point);
                            else if (gameMode == GameMode.MainGame)
                                PlayGameScene.points.Add(point);
                            else if (gameMode == GameMode.SurvivalMode)
                                SurvivalGameScene.points.Add(point); 
                        }
                        bullets.RemoveAt(i--);
                        break;
                    }
                }
            }
        }


        public static void updateProjectileHitBot(HydroBot hydroBot, List<DamageBullet> enemyBullets, GameMode gameMode) {
            for (int i = 0; i < enemyBullets.Count; ) {
                if (enemyBullets[i].BoundingSphere.Intersects(hydroBot.BoundingSphere)) {
                    if (!HydroBot.invincibleMode)
                    {
                        HydroBot.currentHitPoint -= enemyBullets[i].damage;
                        PoseidonGame.audio.botYell.Play();

                        Point point = new Point();
                        String point_string = "-" + enemyBullets[i].damage.ToString() + "HP";
                        point.LoadContent(PoseidonGame.contentManager, point_string, hydroBot.Position, Color.Black);
                        if (gameMode == GameMode.ShipWreck)
                            ShipWreckScene.points.Add(point);
                        else if (gameMode == GameMode.MainGame)
                            PlayGameScene.points.Add(point);
                        else if (gameMode == GameMode.SurvivalMode)
                            SurvivalGameScene.points.Add(point);
                    }
                    enemyBullets.RemoveAt(i);
                    
                }
                else { i++;  }
            }
        }

        public static void updateBulletOutOfBound(int MaxRangeX, int MaxRangeZ, List<HealthBullet> healBullets, List<DamageBullet> botBullets, List<DamageBullet> enemyBullets, List<DamageBullet> alliesBulleys, BoundingFrustum frustum)
        {
            for (int i = 0; i < healBullets.Count; ) {
                if (isOutOfMap(healBullets[i].Position, MaxRangeX, MaxRangeZ) || isOutOfView(healBullets[i].BoundingSphere, frustum)) {
                    healBullets.RemoveAt(i);
                }
                else {
                    i++;
                }
            }

            for (int i = 0; i < botBullets.Count; ) {
                if (isOutOfMap(botBullets[i].Position, MaxRangeX, MaxRangeZ) || isOutOfView(botBullets[i].BoundingSphere, frustum))
                {
                    botBullets.RemoveAt(i);
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

    }
}