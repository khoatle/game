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
        public static void UseHerculesBow(Tank tank, ContentManager Content, List<DamageBullet> myBullets)
        {
            DamageBullet d = new DamageBullet();

            Matrix orientationMatrix = Matrix.CreateRotationY(tank.ForwardDirection);
            Vector3 movement = Vector3.Zero;
            movement.Z = 1;
            Vector3 shootingDirection = Vector3.Transform(movement, orientationMatrix);

            d.initialize(tank.Position, shootingDirection, GameConstants.BulletSpeed, tank.strength * 10, tank.strengthUp);
            d.loadContent(Content, "Models/fuelcarrier");
            myBullets.Add(d);
        }
        public static void UseThorHammer(GameTime gameTime, Tank tank, Enemy[] enemies, ref int enemiesAmount)
        {
            for (int i = 0; i < enemiesAmount; i++)
            {
                if (InThorRange(tank, enemies[i].Position)){
                    enemies[i].stunned = true;
                    enemies[i].stunnedStartTime = gameTime.TotalGameTime.TotalSeconds;
                    enemies[i].health -= (int) GameConstants.ThorDamage;
                    PushEnemy(tank, enemies[i]);
                    if (enemies[i].health <= 0)
                    {
                        if (enemies[i].isBigBoss == true) PlayGameScene.isBossKilled = true;
                        for (int k = i + 1; k < enemiesAmount; k++) {
                            enemies[k - 1] = enemies[k];
                        }
                        enemies[--enemiesAmount] = null;
                        i--;
                    }
                }
                
            }
        }
        // enemy is inside the stun area of Thor's Hammer
        public static bool InThorRange(Tank tank, Vector3 enemyPosition)
        {
            float distance = (enemyPosition - tank.Position).Length();
            if (distance < GameConstants.ThorRange) return true;
            else return false;
        }
        // push enemy away
        public static void PushEnemy(Tank tank, Enemy enemy)
        {
            Vector3 pushVector = enemy.Position - tank.Position;
            pushVector.Normalize();
            enemy.Position += (pushVector * GameConstants.ThorPushFactor);
            MathHelper.Clamp(enemy.Position.X, -tank.MaxRangeX, tank.MaxRangeX);
            MathHelper.Clamp(enemy.Position.Z, -tank.MaxRangeZ, tank.MaxRangeZ);
            enemy.BoundingSphere.Center = enemy.Position;
        }
        //Knock out any enemy that you crash into
        public static void KnockOutEnemies(GameTime gameTime, Tank tank, Enemy[] enemies, ref int enemiesAmount, AudioLibrary audio)
        {
            for (int i = 0; i < enemiesAmount; i++)
            {
                if (tank.BoundingSphere.Intersects(enemies[i].BoundingSphere))
                {
                    Vector3 pushVector = enemies[i].Position - tank.Position;
                    pushVector.Normalize();
                    enemies[i].stunned = true;
                    enemies[i].stunnedStartTime = gameTime.TotalGameTime.TotalSeconds;
                    enemies[i].Position += (pushVector * GameConstants.ThorPushFactor);
                    MathHelper.Clamp(enemies[i].Position.X, -tank.MaxRangeX, tank.MaxRangeX);
                    MathHelper.Clamp(enemies[i].Position.Z, -tank.MaxRangeZ, tank.MaxRangeZ);
                    enemies[i].BoundingSphere.Center = enemies[i].Position;
                    enemies[i].health -= (int)GameConstants.HermesDamage;
                    audio.Shooting.Play();
                    if (enemies[i].health <= 0)
                    {
                        if (enemies[i].isBigBoss == true) PlayGameScene.isBossKilled = true;
                        for (int k = i + 1; k < enemiesAmount; k++)
                        {
                            enemies[k - 1] = enemies[k];
                        }
                        enemies[--enemiesAmount] = null;
                        i--;
                    }
                }
            }
        }
    }
}
