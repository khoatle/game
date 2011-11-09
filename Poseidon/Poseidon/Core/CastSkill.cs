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
        public static void UseHerculesBow(HydroBot tank, ContentManager Content, SpriteBatch spriteBatch, List<DamageBullet> myBullets, GameScene inGameScene)
        {
            float healthiness = (float)HydroBot.currentHitPoint / (float)GameConstants.PlayerStartingHP;
            HerculesBullet d = new HerculesBullet(Content, spriteBatch, inGameScene);

            Matrix orientationMatrix = Matrix.CreateRotationY(tank.ForwardDirection);
            Vector3 movement = Vector3.Zero;
            movement.Z = 1;
            Vector3 shootingDirection = Vector3.Transform(movement, orientationMatrix);

            d.initialize(tank.Position, shootingDirection, GameConstants.BulletSpeed, HydroBot.strength * 20 * healthiness, HydroBot.strengthUp);
            d.loadContent(Content, "Models/BulletModels/herculesBullet");
            PlayGameScene.audio.herculesShot.Play();
            myBullets.Add(d);
        }
        public static void UseThorHammer(GameTime gameTime, HydroBot tank, BaseEnemy[] enemies, ref int enemiesAmount, SwimmingObject[] fishes, int fishAmount, int scene) // scene 1:playgame, 2:shipwreck
        {
            for (int i = 0; i < enemiesAmount; i++)
            {
                if (InThorRange(tank, enemies[i].Position)){
                    float healthiness = (float) HydroBot.currentHitPoint / (float)GameConstants.PlayerStartingHP;
                    enemies[i].stunned = true;
                    enemies[i].stunnedStartTime = gameTime.TotalGameTime.TotalSeconds;
                    int healthloss = (int) (GameConstants.ThorDamage * healthiness * HydroBot.strength);
                    enemies[i].health -= healthloss;
                    PushEnemy(tank, enemies[i], enemies, enemiesAmount, fishes, fishAmount);
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
                    point.LoadContent(PlayGameScene.Content, point_string, enemies[i].Position, Color.DarkRed);
                    if (scene == 2)
                        ShipWreckScene.points.Add(point);
                    else
                        PlayGameScene.points.Add(point);
                }       
            }
        }

        public static void useHypnotise(BaseEnemy enemy, GameTime gameTime) {
            enemy.setHypnotise(gameTime);
        } 

        // enemy is inside the stun area of Thor's Hammer
        public static bool InThorRange(HydroBot tank, Vector3 enemyPosition)
        {
            float distance = (enemyPosition - tank.Position).Length();
            if (distance < GameConstants.ThorRange) return true;
            else return false;
        }
        // push enemy away
        public static void PushEnemy(HydroBot tank, BaseEnemy enemy, SwimmingObject[] enemies, int enemiesAmount, SwimmingObject[] fishes, int fishAmount )
        {
            Vector3 oldPosition = enemy.Position;
            Vector3 pushVector = enemy.Position - tank.Position;
            pushVector.Normalize();
            enemy.Position += (pushVector * GameConstants.ThorPushFactor);
            enemy.Position.X = MathHelper.Clamp(enemy.Position.X, -tank.MaxRangeX, tank.MaxRangeX);
            enemy.Position.Z = MathHelper.Clamp(enemy.Position.Z, -tank.MaxRangeZ, tank.MaxRangeZ);
            enemy.BoundingSphere.Center = enemy.Position;
            if (Collision.isBarrierVsBarrierCollision(enemy, enemy.BoundingSphere, fishes, fishAmount)
                || Collision.isBarrierVsBarrierCollision(enemy, enemy.BoundingSphere, enemies, enemiesAmount))
            {
                enemy.Position = oldPosition;
                enemy.BoundingSphere.Center = oldPosition;
            }
        }
        //Knock out any enemy that you crash into
        public static void KnockOutEnemies(GameTime gameTime, HydroBot tank, BaseEnemy[] enemies, ref int enemiesAmount, SwimmingObject[] fishes, int fishAmount, AudioLibrary audio, int scene) //scene 1:playgame, 2:shipwreck
        {
            for (int i = 0; i < enemiesAmount; i++)
            {
                if (tank.BoundingSphere.Intersects(enemies[i].BoundingSphere))
                {
                    PlayGameScene.audio.bodyHit.Play();
                    float healthiness = (float)HydroBot.currentHitPoint / (float)GameConstants.PlayerStartingHP;
                    Vector3 oldPosition = enemies[i].Position;
                    Vector3 pushVector = enemies[i].Position - tank.Position;
                    pushVector.Normalize();
                    enemies[i].stunned = true;
                    enemies[i].stunnedStartTime = gameTime.TotalGameTime.TotalSeconds;
                    enemies[i].Position += (pushVector * GameConstants.ThorPushFactor);
                    enemies[i].Position.X = MathHelper.Clamp(enemies[i].Position.X, -tank.MaxRangeX, tank.MaxRangeX);
                    enemies[i].Position.Z = MathHelper.Clamp(enemies[i].Position.Z, -tank.MaxRangeZ, tank.MaxRangeZ);
                    enemies[i].BoundingSphere.Center = enemies[i].Position;
                    if (Collision.isBarrierVsBarrierCollision(enemies[i], enemies[i].BoundingSphere, fishes, fishAmount)
                        || Collision.isBarrierVsBarrierCollision(enemies[i], enemies[i].BoundingSphere, enemies, enemiesAmount))
                    {
                        enemies[i].Position = oldPosition;
                        enemies[i].BoundingSphere.Center = oldPosition;
                    }
                    int healthloss = (int)(GameConstants.HermesDamage * healthiness * HydroBot.strength);
                    enemies[i].health -= healthloss;
                    audio.Shooting.Play();
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
                    point.LoadContent(PlayGameScene.Content, point_string, enemies[i].Position, Color.DarkRed);
                    if (scene == 2)
                        ShipWreckScene.points.Add(point);
                    else
                        PlayGameScene.points.Add(point);
                }
            }
        }
    }
}
