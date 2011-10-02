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
    public partial class PlayGameScene
    {
        public void UseHerculesBow()
        {
            DamageBullet d = new DamageBullet();

            Matrix orientationMatrix = Matrix.CreateRotationY(tank.ForwardDirection);
            Vector3 movement = Vector3.Zero;
            movement.Z = 1;
            Vector3 shootingDirection = Vector3.Transform(movement, orientationMatrix);

            d.initialize(tank.Position, shootingDirection, GameConstants.BulletSpeed, tank.strength * 10, tank.strengthUp);
            d.loadContent(Content, "Models/fuelcarrier");
            myBullet.Add(d);
        }
        public void UseThorHammer()
        {
            for (int i = 0; i < enemiesAmount; i++)
            {
                if (InThorRange(enemies[i].Position)){
                    enemies[i].stunned = true;
                    enemies[i].health -= (int) GameConstants.ThorDamage;
                    PushEnemy(enemies[i]);
                    if (enemies[i].health <= 0)
                    {
                        for (int k = i + 1; k < enemiesAmount; k++) {
                            enemies[k - 1] = enemies[k];
                        }
                        enemies[--enemiesAmount] = null;
                    }
                }
                
            }
        }
        // enemy is inside the stun area of Thor's Hammer
        public bool InThorRange(Vector3 enemyPosition)
        {
            float distance = (enemyPosition - tank.Position).Length();
            if (distance < GameConstants.ThorRange) return true;
            else return false;
        }
        // push enemy away
        public void PushEnemy(Enemy enemy)
        {
            Vector3 pushVector = enemy.Position - tank.Position;
            pushVector.Normalize();
            enemy.Position += (pushVector * GameConstants.ThorPushFactor);
            enemy.BoundingSphere.Center = enemy.Position;
        }
    }
}
