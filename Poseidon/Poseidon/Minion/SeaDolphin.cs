using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon
{
    public class SeaDolphin : Fish
    {
        public static float AfterX = 0;
        public static float AfterZ = -30;
        public static float SeaDolphinSeeRadius = 30f;

        public SeaDolphin() : base() {
            isBigBoss = true;
        }

        public BaseEnemy spotEnemy(SwimmingObject[] enemies, int enemiesSize) {
            for (int i = 0; i < enemiesSize; i++) { 
                if (Vector3.Distance(enemies[i].Position, Position) < SeaDolphinSeeRadius) {
                    currentTarget = (BaseEnemy)enemies[i];
                }
            }
            return null;
        }

        public bool isTargeting() {
            return currentTarget == null || currentTarget.health <= 0;
        }

        public void huntTarget() { 
        
        }

        public void returnToLeader(HydroBot bot, SwimmingObject[] enemies, int enemiesSize, SwimmingObject[] fish, int fishSize)
        {
            Vector3 destination = bot.Position + new Vector3(AfterX, 0, AfterZ);
            seekDestination(destination, enemies, enemiesSize, fish, fishSize, bot);

            isFollowing = true;
            isWandering = false;
            isFighting = false;
        
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, SwimmingObject[] enemies, int enemiesSize, SwimmingObject[] fish, int fishSize, int changeDirection, HydroBot tank, List<DamageBullet> enemyBullet)
        {
            if (isWandering == false)
            {
                Vector3 destination = tank.Position + new Vector3(AfterX, 0, AfterZ);
                seekDestination(destination, enemies, enemiesSize, fish, fishSize, tank);

                // If the fish is near the point after the bot's back
                if (Vector3.Distance(destination, Position) < 60f)
                    isWandering = true;
            }
            else
            {
                randomWalk(changeDirection, enemies, enemiesSize, fish, fishSize, tank);
                // If too far from leader
                if (Vector3.Distance(Position, tank.Position) > tank.controlRadius)
                    isWandering = false;
            }

            // if clip player has been initialized, update it
            if (clipPlayer != null)
            {
                qRotation = Quaternion.CreateFromAxisAngle(
                                Vector3.Up,
                                ForwardDirection);
                float scale = 0.75f;
                if (isBigBoss) scale *= 2.0f;
                fishMatrix = Matrix.CreateScale(scale) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                                    Matrix.CreateFromQuaternion(qRotation) *
                                    Matrix.CreateTranslation(Position);
                clipPlayer.update(gameTime.ElapsedGameTime, true, fishMatrix);
            }
        }
    }
}
