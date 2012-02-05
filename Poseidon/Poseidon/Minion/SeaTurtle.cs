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
    public class SeaTurtle : Fish
    {
        public static float AfterX = 30;
        public static float AfterZ = 0;

        public SeaTurtle() : base() {
            isBigBoss = true;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, SwimmingObject[] enemies, int enemiesSize, SwimmingObject[] fish, int fishSize, int changeDirection, HydroBot tank, List<DamageBullet> enemyBullet)
        {
            if (isWandering == false)
            {
                Vector3 destination = tank.Position + new Vector3(AfterX, 0, AfterZ);
                seekDestination(destination, enemies, enemiesSize, fish, fishSize, tank);

                // If the fish is near the point after the bot's back
                if (Vector3.Distance(destination, Position) < 60f)
                {
                    isWandering = true;
                }
            }
            else
            {
                randomWalk(changeDirection, enemies, enemiesSize, fish, fishSize, tank);
                // If too far from leader
                if (Vector3.Distance(Position, tank.Position) > tank.controlRadius)
                {
                    isWandering = false;
                }
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

        public void wander() { 
            
        
        }
    }
}
