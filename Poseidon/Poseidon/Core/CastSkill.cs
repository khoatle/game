using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Poseidon.Core;


namespace Poseidon
{
    public class CastSkill
    {
        public static void UseSkill(bool mouseOnLivingObject, Vector3 pointIntersect, Cursor cursor, Camera gameCamera, GameMode gameMode, HydroBot hydroBot, GameScene gameScene, ContentManager Content, SpriteBatch spriteBatch,
            GameTime gameTime, List<DamageBullet> myBullet, BaseEnemy[] enemies, ref int enemiesAmount, Fish[] fish, ref int fishAmount )
        {
            bool skillUsed = false;
            int healthToLose = 0;
            // Hercules' Bow!!!
            if (HydroBot.activeSkillID == 0)// && mouseOnLivingObject)
            {
                pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);
                hydroBot.ForwardDirection = CursorManager.CalculateAngle(pointIntersect, hydroBot.Position);
                //use skill combo if activated
                //cooldowns for both skills must be cleared
                if ((HydroBot.skillComboActivated && HydroBot.secondSkillID == 1) &&
                    //cooldowns check
                    ((HydroBot.firstUse[0] == true && HydroBot.firstUse[1] == true)||
                     (PoseidonGame.playTime.TotalSeconds - HydroBot.skillPrevUsed[0] > GameConstants.coolDownForHerculesBow &&
                      PoseidonGame.playTime.TotalSeconds - HydroBot.skillPrevUsed[1] > GameConstants.coolDownForThorHammer)))
                {
                    ShootHammer(hydroBot, Content, myBullet);
                    HydroBot.skillPrevUsed[0] = PoseidonGame.playTime.TotalSeconds;
                    HydroBot.skillPrevUsed[1] = PoseidonGame.playTime.TotalSeconds;
                    HydroBot.firstUse[0] = false;
                    HydroBot.firstUse[1] = false;
                    skillUsed = true;
                }
                //or else use single skill
                //if the skill has cooled down
                //or this is the 1st time the user uses it
                else if ((PoseidonGame.playTime.TotalSeconds - HydroBot.skillPrevUsed[0] > GameConstants.coolDownForHerculesBow) || HydroBot.firstUse[0] == true)
                {
                    HydroBot.firstUse[0] = false;
                    HydroBot.skillPrevUsed[0] = PoseidonGame.playTime.TotalSeconds;
                    //audio.Explosion.Play();
                    CastSkill.UseHerculesBow(hydroBot, Content, spriteBatch, myBullet, gameScene);
                    skillUsed = true;
                }

                // Lose health after using this
                if (skillUsed)
                {
                    
                    if (HydroBot.skillComboActivated && HydroBot.secondSkillID != -1 && HydroBot.secondSkillID != HydroBot.activeSkillID)
                        healthToLose = 2 * GameConstants.skillHealthLoss;
                    else healthToLose = GameConstants.skillHealthLoss;
                    //display HP loss
                    HydroBot.currentHitPoint -= healthToLose;
                    Point point = new Point();
                    String point_string = "-" + healthToLose.ToString() + "HP";
                    point.LoadContent(PoseidonGame.contentManager, point_string, hydroBot.Position, Color.Black);
                    if (gameMode == GameMode.ShipWreck)
                        ShipWreckScene.points.Add(point);
                    else if (gameMode == GameMode.MainGame)
                        PlayGameScene.points.Add(point);
                    else if (gameMode == GameMode.SurvivalMode)
                        SurvivalGameScene.points.Add(point);
       
                    if (!hydroBot.clipPlayer.inRange(61, 90))
                        hydroBot.clipPlayer.switchRange(61, 90);
                }
                //stop moving whether or not the skill has been casted
                hydroBot.reachDestination = true;

            }
            //Thor's Hammer!!!
            if (HydroBot.activeSkillID == 1)
            {
                if ((PoseidonGame.playTime.TotalSeconds - HydroBot.skillPrevUsed[1] > GameConstants.coolDownForThorHammer) || HydroBot.firstUse[1] == true)
                {
                    HydroBot.firstUse[1] = false;
                    HydroBot.skillPrevUsed[1] = PoseidonGame.playTime.TotalSeconds;
                    PoseidonGame.audio.Explo1.Play();
                    gameCamera.Shake(25f, .4f);
                    CastSkill.UseThorHammer(hydroBot.Position, hydroBot.MaxRangeX, hydroBot.MaxRangeZ, enemies, ref enemiesAmount, fish, fishAmount, GameMode.MainGame);
                    HydroBot.currentHitPoint -= GameConstants.skillHealthLoss; // Lose health after useing this

                    //display HP loss
                    Point point = new Point();
                    String point_string = "-" + GameConstants.skillHealthLoss.ToString() + "HP";
                    point.LoadContent(PoseidonGame.contentManager, point_string, hydroBot.Position, Color.Black);
                    if (gameMode == GameMode.ShipWreck)
                        ShipWreckScene.points.Add(point);
                    else if (gameMode == GameMode.MainGame)
                        PlayGameScene.points.Add(point);
                    else if (gameMode == GameMode.SurvivalMode)
                        SurvivalGameScene.points.Add(point);

                    if (!hydroBot.clipPlayer.inRange(61, 90))
                        hydroBot.clipPlayer.switchRange(61, 90);
                }
            }
            // Achilles' Armor!!!
            if (HydroBot.activeSkillID == 2)
            {
                if ((PoseidonGame.playTime.TotalSeconds - HydroBot.skillPrevUsed[2] > GameConstants.coolDownForArchillesArmor) || HydroBot.firstUse[2] == true)
                {
                    HydroBot.firstUse[2] = false;
                    HydroBot.invincibleMode = true;
                    PoseidonGame.audio.armorSound.Play();
                    HydroBot.skillPrevUsed[2] = PoseidonGame.playTime.TotalSeconds;
                    HydroBot.currentHitPoint -= GameConstants.skillHealthLoss; // Lose health after useing this

                    //display HP loss
                    Point point = new Point();
                    String point_string = "-" + GameConstants.skillHealthLoss.ToString() + "HP";
                    point.LoadContent(PoseidonGame.contentManager, point_string, hydroBot.Position, Color.Black);
                    if (gameMode == GameMode.ShipWreck)
                        ShipWreckScene.points.Add(point);
                    else if (gameMode == GameMode.MainGame)
                        PlayGameScene.points.Add(point);
                    else if (gameMode == GameMode.SurvivalMode)
                        SurvivalGameScene.points.Add(point);

                    if (!hydroBot.clipPlayer.inRange(61, 90))
                        hydroBot.clipPlayer.switchRange(61, 90);
                }
            }

            //Hermes' Winged Sandal!!!
            if (HydroBot.activeSkillID == 3)
            {
                if ((PoseidonGame.playTime.TotalSeconds - HydroBot.skillPrevUsed[3] > GameConstants.coolDownForHermesSandle) || HydroBot.firstUse[3] == true)
                {
                    HydroBot.firstUse[3] = false;
                    PoseidonGame.audio.hermesSound.Play();
                    HydroBot.skillPrevUsed[3] = PoseidonGame.playTime.TotalSeconds;
                    HydroBot.supersonicMode = true;
                    HydroBot.currentHitPoint -= GameConstants.skillHealthLoss; // Lose health after useing this

                    //display HP loss
                    Point point = new Point();
                    String point_string = "-" + GameConstants.skillHealthLoss.ToString() + "HP";
                    point.LoadContent(PoseidonGame.contentManager, point_string, hydroBot.Position, Color.Black);
                    if (gameMode == GameMode.ShipWreck)
                        ShipWreckScene.points.Add(point);
                    else if (gameMode == GameMode.MainGame)
                        PlayGameScene.points.Add(point);
                    else if (gameMode == GameMode.SurvivalMode)
                        SurvivalGameScene.points.Add(point);

                    if (!hydroBot.clipPlayer.inRange(61, 90))
                        hydroBot.clipPlayer.switchRange(61, 90);
                }
            }

            // Hypnotise skill
            if (HydroBot.activeSkillID == 4)
            {
                BaseEnemy enemy = CursorManager.MouseOnWhichEnemy(cursor, gameCamera, enemies, enemiesAmount);

                if (enemy != null && (HydroBot.firstUse[4] == true || PoseidonGame.playTime.TotalSeconds - HydroBot.skillPrevUsed[4] > GameConstants.coolDownForHypnotise))
                {
                    HydroBot.firstUse[4] = false;

                    enemy.setHypnotise(gameTime);

                    HydroBot.skillPrevUsed[4] = PoseidonGame.playTime.TotalSeconds;
                    HydroBot.currentHitPoint -= GameConstants.skillHealthLoss;

                    //display HP loss
                    Point point = new Point();
                    String point_string = "-" + GameConstants.skillHealthLoss.ToString() + "HP";
                    point.LoadContent(PoseidonGame.contentManager, point_string, hydroBot.Position, Color.Black);
                    if (gameMode == GameMode.ShipWreck)
                        ShipWreckScene.points.Add(point);
                    else if (gameMode == GameMode.MainGame)
                        PlayGameScene.points.Add(point);
                    else if (gameMode == GameMode.SurvivalMode)
                        SurvivalGameScene.points.Add(point);

                    PoseidonGame.audio.hipnotizeSound.Play();
                    if (!hydroBot.clipPlayer.inRange(61, 90))
                        hydroBot.clipPlayer.switchRange(61, 90);
                }
            }

            pointIntersect = Vector3.Zero;
        }
        //=================//combo skills casting
        public static void ShootHammer(HydroBot hydroBot, ContentManager Content, List<DamageBullet> myBullets)
        {
            float healthiness = (float)HydroBot.currentHitPoint / (float)GameConstants.PlayerStartingHP;
            FlyingHammer f = new FlyingHammer();

            Matrix orientationMatrix = Matrix.CreateRotationY(hydroBot.ForwardDirection);
            Vector3 movement = Vector3.Zero;
            movement.Z = 1;
            Vector3 shootingDirection = Vector3.Transform(movement, orientationMatrix);

            f.initialize(hydroBot.Position, shootingDirection, GameConstants.BulletSpeed, HydroBot.strength * 10 * healthiness, HydroBot.strengthUp);
            f.loadContent(Content, "Models/BulletModels/mjolnir");
            PoseidonGame.audio.herculesShot.Play();
            myBullets.Add(f);
        }
        //=================//single skill casting
        public static void UseHerculesBow(HydroBot hydroBot, ContentManager Content, SpriteBatch spriteBatch, List<DamageBullet> myBullets, GameScene inGameScene)
        {
            float healthiness = (float)HydroBot.currentHitPoint / (float)GameConstants.PlayerStartingHP;
            HerculesBullet d = new HerculesBullet(Content, spriteBatch, inGameScene);

            Matrix orientationMatrix = Matrix.CreateRotationY(hydroBot.ForwardDirection);
            Vector3 movement = Vector3.Zero;
            movement.Z = 1;
            Vector3 shootingDirection = Vector3.Transform(movement, orientationMatrix);

            d.initialize(hydroBot.Position, shootingDirection, GameConstants.BulletSpeed, HydroBot.strength * 20 * healthiness, HydroBot.strengthUp);
            d.loadContent(Content, "Models/BulletModels/herculesBullet");
            //d.loadContent(Content, "Models/BulletModels/mjolnir");
            PoseidonGame.audio.herculesShot.Play();
            myBullets.Add(d);
        }
        public static void UseThorHammer(Vector3 Position, int MaxRangeX, int MaxRangeZ, BaseEnemy[] enemies, ref int enemiesAmount, SwimmingObject[] fishes, int fishAmount, GameMode gameMode)
        {
            for (int i = 0; i < enemiesAmount; i++)
            {
                if (InThorRange(Position, enemies[i].Position)){
                    float healthiness = (float) HydroBot.currentHitPoint / (float)GameConstants.PlayerStartingHP;
                    enemies[i].stunned = true;
                    enemies[i].stunnedStartTime = PoseidonGame.playTime.TotalSeconds;
                    int healthloss = (int) (GameConstants.ThorDamage * healthiness * HydroBot.strength * HydroBot.strengthUp);
                    enemies[i].health -= healthloss;
                    PushEnemy(Position, MaxRangeX, MaxRangeZ, enemies[i], enemies, enemiesAmount, fishes, fishAmount);
                    //if (enemies[i].health <= 0)
                    //{
                    //    if (enemies[i].isBigBoss == true) PlayGameScene.isBossKilled = true;
                    //    for (int k = i + 1; k < enemiesAmount; k++) {
                    //        enemies[k - 1] = enemies[k];
                    //    }
                    //    enemies[--enemiesAmount] = null;
                    //    i--;
                    //}

                    Point point = new Point();
                    String point_string = "-" + healthloss.ToString() + "HP";
                    point.LoadContent(PoseidonGame.contentManager, point_string, enemies[i].Position, Color.DarkRed);
                    if (gameMode == GameMode.ShipWreck)
                        ShipWreckScene.points.Add(point);
                    else if (gameMode == GameMode.MainGame)
                        PlayGameScene.points.Add(point);
                    else if (gameMode == GameMode.SurvivalMode)
                        SurvivalGameScene.points.Add(point);
                }       
            }
        }

        public static void useHypnotise(BaseEnemy enemy, GameTime gameTime) {
            enemy.setHypnotise(gameTime);
        } 

        // enemy is inside the stun area of Thor's Hammer
        public static bool InThorRange(Vector3 Position, Vector3 enemyPosition)
        {
            float distance = (enemyPosition - Position).Length();
            if (distance < GameConstants.ThorRange) return true;
            else return false;
        }
        // push enemy away
        public static void PushEnemy(Vector3 Position, int MaxRangeX, int MaxRangeZ, BaseEnemy enemy, SwimmingObject[] enemies, int enemiesAmount, SwimmingObject[] fishes, int fishAmount )
        {
            Vector3 oldPosition = enemy.Position;
            Vector3 pushVector = enemy.Position - Position;
            pushVector.Normalize();
            enemy.Position += (pushVector * GameConstants.ThorPushFactor);
            enemy.Position.X = MathHelper.Clamp(enemy.Position.X, -MaxRangeX, MaxRangeX);
            enemy.Position.Z = MathHelper.Clamp(enemy.Position.Z, -MaxRangeZ, MaxRangeZ);
            enemy.BoundingSphere.Center = enemy.Position;
            if (Collision.isBarrierVsBarrierCollision(enemy, enemy.BoundingSphere, fishes, fishAmount)
                || Collision.isBarrierVsBarrierCollision(enemy, enemy.BoundingSphere, enemies, enemiesAmount))
            {
                enemy.Position = oldPosition;
                enemy.BoundingSphere.Center = oldPosition;
            }
        }
        //Knock out any enemy that you crash into
        public static void KnockOutEnemies(GameTime gameTime, BoundingSphere boundingSphere, Vector3 Position, int MaxRangeX, int MaxRangeZ, BaseEnemy[] enemies, ref int enemiesAmount, SwimmingObject[] fishes, int fishAmount, AudioLibrary audio, GameMode gameMode)
        {
            for (int i = 0; i < enemiesAmount; i++)
            {
                if (boundingSphere.Intersects(enemies[i].BoundingSphere))
                {
                    PoseidonGame.audio.bodyHit.Play();
                    float healthiness = (float)HydroBot.currentHitPoint / (float)GameConstants.PlayerStartingHP;
                    Vector3 oldPosition = enemies[i].Position;
                    Vector3 pushVector = enemies[i].Position - Position;
                    pushVector.Normalize();
                    enemies[i].stunned = true;
                    enemies[i].stunnedStartTime = PoseidonGame.playTime.TotalSeconds;
                    enemies[i].Position += (pushVector * GameConstants.ThorPushFactor);
                    enemies[i].Position.X = MathHelper.Clamp(enemies[i].Position.X, -MaxRangeX, MaxRangeX);
                    enemies[i].Position.Z = MathHelper.Clamp(enemies[i].Position.Z, -MaxRangeZ, MaxRangeZ);
                    enemies[i].BoundingSphere.Center = enemies[i].Position;
                    if (Collision.isBarrierVsBarrierCollision(enemies[i], enemies[i].BoundingSphere, fishes, fishAmount)
                        || Collision.isBarrierVsBarrierCollision(enemies[i], enemies[i].BoundingSphere, enemies, enemiesAmount))
                    {
                        enemies[i].Position = oldPosition;
                        enemies[i].BoundingSphere.Center = oldPosition;
                    }
                    int healthloss = (int)(GameConstants.HermesDamage * healthiness * HydroBot.speed * HydroBot.speedUp);
                    enemies[i].health -= healthloss;
                    //audio.Shooting.Play();
                    //if (enemies[i].health <= 0)
                    //{
                    //    if (enemies[i].isBigBoss == true) PlayGameScene.isBossKilled = true;
                    //    for (int k = i + 1; k < enemiesAmount; k++)
                    //    {
                    //        enemies[k - 1] = enemies[k];
                    //    }
                    //    enemies[--enemiesAmount] = null;
                    //    i--;
                    //}
                    Point point = new Point();
                    String point_string = "-" + healthloss.ToString() + "HP";
                    point.LoadContent(PoseidonGame.contentManager, point_string, enemies[i].Position, Color.DarkRed);
                    if (gameMode == GameMode.ShipWreck)
                        ShipWreckScene.points.Add(point);
                    else if (gameMode == GameMode.MainGame)
                        PlayGameScene.points.Add(point);
                    else if (gameMode == GameMode.SurvivalMode)
                        SurvivalGameScene.points.Add(point);
                }
            }
        }
    }
}
