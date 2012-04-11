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
            int floatHeight;
            if (gameMode == GameMode.ShipWreck) floatHeight = GameConstants.ShipWreckFloatHeight;
            else floatHeight = GameConstants.MainGameFloatHeight;
            pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, floatHeight);
            hydroBot.ForwardDirection = CursorManager.CalculateAngle(pointIntersect, hydroBot.Position);
            // Hercules' Bow!!!
            if (HydroBot.activeSkillID == 0)// && mouseOnLivingObject)
            {
                //use skill combo if activated
                //cooldowns for both skills must be cleared
                if ((HydroBot.skillComboActivated && HydroBot.secondSkillID == 1) &&
                    //cooldowns check
                    ((HydroBot.firstUse[0] == true && HydroBot.firstUse[1] == true)||
                     (PoseidonGame.playTime.TotalSeconds - HydroBot.skillPrevUsed[0] > GameConstants.coolDownForHerculesBow &&
                      PoseidonGame.playTime.TotalSeconds - HydroBot.skillPrevUsed[1] > GameConstants.coolDownForThorHammer)))
                {
                    ShootHammer(hydroBot, Content, myBullet, gameMode);
                    HydroBot.skillPrevUsed[0] = PoseidonGame.playTime.TotalSeconds;
                    HydroBot.skillPrevUsed[1] = PoseidonGame.playTime.TotalSeconds;
                    HydroBot.firstUse[0] = false;
                    HydroBot.firstUse[1] = false;
                    skillUsed = true;
                }
                else if ((HydroBot.skillComboActivated && HydroBot.secondSkillID == 3) && 
                    ((HydroBot.firstUse[3] == true && HydroBot.firstUse[0] == true) ||
                    (PoseidonGame.playTime.TotalSeconds - HydroBot.skillPrevUsed[0] > GameConstants.coolDownForHerculesBow &&
                    PoseidonGame.playTime.TotalSeconds - HydroBot.skillPrevUsed[3] > GameConstants.coolDownForHermesSandal)))
                {
                    ShootPiercingArrow(hydroBot, Content, spriteBatch, myBullet, gameMode);
                    HydroBot.skillPrevUsed[0] = PoseidonGame.playTime.TotalSeconds;
                    HydroBot.skillPrevUsed[3] = PoseidonGame.playTime.TotalSeconds;
                    HydroBot.firstUse[0] = false;
                    HydroBot.firstUse[3] = false;
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
                    CastSkill.UseHerculesBow(hydroBot, Content, spriteBatch, myBullet, gameMode);
                    skillUsed = true;
                }

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
                    CastSkill.UseThorHammer(hydroBot.Position, hydroBot.MaxRangeX, hydroBot.MaxRangeZ, enemies, ref enemiesAmount, fish, fishAmount, HydroBot.gameMode, false);
                    skillUsed = true;
                }
            }
            // Achilles' Armor!!!
            if (HydroBot.activeSkillID == 2)
            {
                if ((HydroBot.skillComboActivated && HydroBot.secondSkillID == 4) &&
                    //cooldowns check
                    ((HydroBot.firstUse[2] == true && HydroBot.firstUse[4] == true) ||
                     (PoseidonGame.playTime.TotalSeconds - HydroBot.skillPrevUsed[2] > GameConstants.coolDownForArchillesArmor &&
                      PoseidonGame.playTime.TotalSeconds - HydroBot.skillPrevUsed[4] > GameConstants.coolDownForHypnotise)))
                {
                    HydroBot.invincibleMode = true;
                    HydroBot.autoHipnotizeMode = true;
                    HydroBot.skillPrevUsed[2] = PoseidonGame.playTime.TotalSeconds;
                    HydroBot.skillPrevUsed[4] = PoseidonGame.playTime.TotalSeconds;
                    HydroBot.firstUse[2] = false;
                    HydroBot.firstUse[4] = false;
                    skillUsed = true;
                }
                else if ((HydroBot.skillComboActivated && HydroBot.secondSkillID == 1) &&
                    //cooldowns check
                ((HydroBot.firstUse[2] == true && HydroBot.firstUse[1] == true) ||
                 (PoseidonGame.playTime.TotalSeconds - HydroBot.skillPrevUsed[2] > GameConstants.coolDownForArchillesArmor &&
                  PoseidonGame.playTime.TotalSeconds - HydroBot.skillPrevUsed[1] > GameConstants.coolDownForThorHammer)))
                {
                    HydroBot.invincibleMode = true;
                    HydroBot.autoExplodeMode = true;
                    HydroBot.skillPrevUsed[2] = PoseidonGame.playTime.TotalSeconds;
                    HydroBot.skillPrevUsed[1] = PoseidonGame.playTime.TotalSeconds;
                    HydroBot.firstUse[2] = false;
                    HydroBot.firstUse[1] = false;
                    skillUsed = true;
                }
                else if ((PoseidonGame.playTime.TotalSeconds - HydroBot.skillPrevUsed[2] > GameConstants.coolDownForArchillesArmor) || HydroBot.firstUse[2] == true)
                {
                    HydroBot.firstUse[2] = false;
                    HydroBot.invincibleMode = true;
                    HydroBot.skillPrevUsed[2] = PoseidonGame.playTime.TotalSeconds;
                    skillUsed = true;
                }
                if (skillUsed) PoseidonGame.audio.armorSound.Play(); 
            }

            //Hermes' Winged Sandal!!!
            if (HydroBot.activeSkillID == 3)
            {
                 if ((HydroBot.skillComboActivated && HydroBot.secondSkillID == 4) && 
                    ((HydroBot.firstUse[3] == true && HydroBot.firstUse[4] == true) ||
                    (PoseidonGame.playTime.TotalSeconds - HydroBot.skillPrevUsed[4] > GameConstants.coolDownForHypnotise &&
                    PoseidonGame.playTime.TotalSeconds - HydroBot.skillPrevUsed[3] > GameConstants.coolDownForHermesSandal)))
                {
                    HydroBot.sonicHipnotiseMode = true;
                    HydroBot.supersonicMode = true;
                    HydroBot.skillPrevUsed[4] = PoseidonGame.playTime.TotalSeconds;
                    HydroBot.skillPrevUsed[3] = PoseidonGame.playTime.TotalSeconds;
                    HydroBot.firstUse[4] = false;
                    HydroBot.firstUse[3] = false;
                    skillUsed = true;
                }
                else if ((PoseidonGame.playTime.TotalSeconds - HydroBot.skillPrevUsed[3] > GameConstants.coolDownForHermesSandal) || HydroBot.firstUse[3] == true)
                {
                    HydroBot.firstUse[3] = false;
                    HydroBot.skillPrevUsed[3] = PoseidonGame.playTime.TotalSeconds;
                    HydroBot.supersonicMode = true;
                    skillUsed = true;
                }
                 if (skillUsed) PoseidonGame.audio.hermesSound.Play();
            }

            // Hypnotise skill
            if (HydroBot.activeSkillID == 4)
            {
                BaseEnemy enemy = CursorManager.MouseOnWhichEnemy(cursor, gameCamera, enemies, enemiesAmount);
                //can't hipnotize a submarine
                if (enemy != null && !(enemy is Submarine) && (HydroBot.firstUse[4] == true || PoseidonGame.playTime.TotalSeconds - HydroBot.skillPrevUsed[4] > GameConstants.coolDownForHypnotise))
                {
                    HydroBot.firstUse[4] = false;
                    PoseidonGame.audio.hipnotizeSound.Play();
                    useHypnotise(enemy);
                    HydroBot.skillPrevUsed[4] = PoseidonGame.playTime.TotalSeconds;
                    skillUsed = true;
                }
            }

            // Lose health after using skill
            if (skillUsed)
            {
                if (HydroBot.skillComboActivated && HydroBot.secondSkillID != -1 && HydroBot.secondSkillID != HydroBot.activeSkillID)
                    healthToLose = 2 * GameConstants.skillHealthLoss;
                else healthToLose = GameConstants.skillHealthLoss;
                //display HP loss
                HydroBot.currentHitPoint -= healthToLose;
                Point point = new Point();
                String point_string = "-" + healthToLose.ToString() + "HP";
                point.LoadContent(PoseidonGame.contentManager, point_string, hydroBot.Position, Color.Red);
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
            pointIntersect = Vector3.Zero;
        }
        //=================//combo skills casting
        public static void ShootHammer(HydroBot hydroBot, ContentManager Content, List<DamageBullet> myBullets, GameMode gameMode)
        {
            float healthiness = (float)HydroBot.currentHitPoint / (float)GameConstants.PlayerStartingHP;
            FlyingHammer f = new FlyingHammer(gameMode);

            Matrix orientationMatrix = Matrix.CreateRotationY(hydroBot.ForwardDirection);
            Vector3 movement = Vector3.Zero;
            movement.Z = 1;
            Vector3 shootingDirection = Vector3.Transform(movement, orientationMatrix);

            f.initialize(hydroBot.Position, shootingDirection, GameConstants.BulletSpeed, HydroBot.strength * 10 * healthiness, HydroBot.strengthUp * HydroBot.bowPower * HydroBot.hammerPower, gameMode);
            f.loadContent(Content, "Models/BulletModels/mjolnir");
            PoseidonGame.audio.herculesShot.Play();
            myBullets.Add(f);
        }
        public static void ShootPiercingArrow(HydroBot hydroBot, ContentManager Content, SpriteBatch spriteBatch, List<DamageBullet> myBullets, GameMode gameMode)
        {
            float healthiness = (float)HydroBot.currentHitPoint / (float)GameConstants.PlayerStartingHP;
            HerculesBullet d = new HerculesBullet(Content, spriteBatch, gameMode, hydroBot.ForwardDirection, true);

            Matrix orientationMatrix = Matrix.CreateRotationY(hydroBot.ForwardDirection);
            Vector3 movement = Vector3.Zero;
            movement.Z = 1;
            Vector3 shootingDirection = Vector3.Transform(movement, orientationMatrix);

            d.initialize(hydroBot.Position + shootingDirection * 10, shootingDirection, GameConstants.BulletSpeed, HydroBot.strength * 10 * healthiness * HydroBot.bowPower * HydroBot.sandalPower, HydroBot.strengthUp, gameMode);
            d.loadContent(Content, "Models/BulletModels/piercingArrow");
            d.BoundingSphere.Radius *= 0.7f;
            //d.loadContent(Content, "Models/BulletModels/mjolnir");
            PoseidonGame.audio.herculesShot.Play();
            myBullets.Add(d);

        }
        //=================//single skill casting
        public static void UseHerculesBow(HydroBot hydroBot, ContentManager Content, SpriteBatch spriteBatch, List<DamageBullet> myBullets, GameMode gameMode)
        {
            float healthiness = (float)HydroBot.currentHitPoint / (float)GameConstants.PlayerStartingHP;
            HerculesBullet d = new HerculesBullet(Content, spriteBatch, gameMode, hydroBot.ForwardDirection, false);

            Matrix orientationMatrix = Matrix.CreateRotationY(hydroBot.ForwardDirection);
            Vector3 movement = Vector3.Zero;
            movement.Z = 1;
            Vector3 shootingDirection = Vector3.Transform(movement, orientationMatrix);

            d.initialize(hydroBot.Position, shootingDirection, GameConstants.BulletSpeed, HydroBot.strength * 20 * healthiness * HydroBot.bowPower, HydroBot.strengthUp, gameMode);
            d.loadContent(Content, "Models/BulletModels/herculesArrow");
            d.BoundingSphere.Radius *= 0.7f;
            //d.loadContent(Content, "Models/BulletModels/mjolnir");
            PoseidonGame.audio.herculesShot.Play();
            myBullets.Add(d);
        }
        public static void UseThorHammer(Vector3 Position, int MaxRangeX, int MaxRangeZ, BaseEnemy[] enemies, ref int enemiesAmount, SwimmingObject[] fishes, int fishAmount, GameMode gameMode, bool halfStrength)
        {
            HydroBot.distortingScreen = true;
            HydroBot.distortionStart = PoseidonGame.playTime.TotalSeconds;

            for (int i = 0; i < enemiesAmount; i++)
            {
                if (InThorRange(Position, enemies[i].Position)){
                    float healthiness = (float) HydroBot.currentHitPoint / (float)GameConstants.PlayerStartingHP;
                    //you can't stun a submarine
                    if (!(enemies[i] is Submarine))
                    {
                        enemies[i].stunned = true;
                        enemies[i].stunnedStartTime = PoseidonGame.playTime.TotalSeconds;
                    }
                    float healthloss = (GameConstants.ThorDamage * healthiness * HydroBot.strength * HydroBot.strengthUp * HydroBot.hammerPower);
                    if (halfStrength) healthloss /= 2;
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
                    String point_string = "-" + ((int)healthloss).ToString() + "HP";
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

        public static void useHypnotise(BaseEnemy enemy) {
            enemy.setHypnotise();
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
        public static void KnockOutEnemies(BoundingSphere boundingSphere, Vector3 Position, int MaxRangeX, int MaxRangeZ, BaseEnemy[] enemies, ref int enemiesAmount, SwimmingObject[] fishes, int fishAmount, AudioLibrary audio, GameMode gameMode)
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
                    //can't stun a submarine
                    if (!(enemies[i] is Submarine))
                    {
                        enemies[i].stunned = true;
                        enemies[i].stunnedStartTime = PoseidonGame.playTime.TotalSeconds;
                        if (HydroBot.sonicHipnotiseMode)
                        {
                            enemies[i].setHypnotise();
                        }
                    }
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
                    int healthloss = (int)(GameConstants.HermesDamage * healthiness * HydroBot.speed * HydroBot.speedUp * HydroBot.sandalPower);
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
