using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Poseidon.Core
{
    public class Radar
    {
        private Texture2D PlayerDotImage;
        private Texture2D EnemyDotImage;
        private Texture2D BigBossDotImage;
        private Texture2D FishDotImage;
        private Texture2D RadarImage;
        private Texture2D ShipwreckDotImage;

        // Local coords of the radar image's center, used to offset image when being drawn
        private Vector2 RadarImageCenter;

        // Distance that the radar can "see"
        private const float RadarRange = GameConstants.RadarRange;
        private const float RadarRangeSquared = RadarRange * RadarRange;

        // Radius of radar circle on the screen
        public static float RadarScreenRadius = GameConstants.RadarScreenRadius;

        // This is the center position of the radar hud on the screen. 
        private Vector2 RadarCenterPos;// = new Vector2(500, 175);

        public Radar(ContentManager Content, string playerDotPath, string enemyDotPath, string fishDotPath, string radarImagePath, string bigBossDotPath, Vector2 radarCenter)
        {
            PlayerDotImage = Content.Load<Texture2D>(playerDotPath);
            EnemyDotImage = Content.Load<Texture2D>(enemyDotPath);
            BigBossDotImage = Content.Load<Texture2D>(bigBossDotPath);
            FishDotImage = Content.Load<Texture2D>(fishDotPath);
            RadarImage = Content.Load<Texture2D>(radarImagePath);
            ShipwreckDotImage = Content.Load<Texture2D>("Image/RadarTextures/shipwreckDot");
            this.RadarCenterPos = radarCenter;
            RadarImageCenter = new Vector2(RadarImage.Width * 0.5f, RadarImage.Height * 0.5f);
        }

        public void Draw(SpriteBatch spriteBatch, Vector3 playerPos, BaseEnemy[] enemies, int enemyAmount, Fish[] fishes, int fishAmount, List<ShipWreck> shipWrecks)
        {
            // The last parameter of the color determines how transparent the radar circle will be
            spriteBatch.Draw(RadarImage, RadarCenterPos, null, Color.White, 0.0f, RadarImageCenter, RadarScreenRadius / ((RadarImage.Height) * 0.5f), SpriteEffects.None, 0.0f);
            //new Color(100, 100, 100, 150)
            // display shipwreck on the map
            if (shipWrecks != null)
            {
                foreach (ShipWreck shipWreck in shipWrecks)
                {
                    if (shipWreck.seen)
                    {
                        Vector2 diffVect = new Vector2(shipWreck.Position.X - playerPos.X, shipWreck.Position.Z - playerPos.Z);
                        float distance = diffVect.LengthSquared();

                        // Check if enemy is within RadarRange
                        //if (distance < RadarRangeSquared)
                        //{
                        if (distance > RadarRangeSquared)
                            diffVect *= RadarRange / diffVect.Length();
                        // Scale the distance from world coords to radar coords
                        diffVect *= RadarScreenRadius / RadarRange;

                        // We rotate each point on the radar so that the player is always facing UP on the radar
                        //diffVect = Vector2.Transform(diffVect, Matrix.CreateRotationZ(playerForwardRadians));

                        // Offset coords from radar's center
                        diffVect = -diffVect;
                        diffVect += RadarCenterPos;

                        // We scale each dot so that enemies that are at higher elevations have bigger dots, and enemies
                        // at lower elevations have smaller dots.
                        float scaleHeight = 1.0f + ((shipWreck.Position.Y - playerPos.Y) / 200.0f);

                        // Draw enemy dot on radar
                        spriteBatch.Draw(ShipwreckDotImage, diffVect, null, Color.White, 0.0f, new Vector2(ShipwreckDotImage.Width / 2, ShipwreckDotImage.Height / 2), scaleHeight * 0.8f, SpriteEffects.None, 0.0f);
                        //}
                    }
                }
            }
            // If enemy is in range
            for (int i = 0; i < enemyAmount; i++)
            {
                Vector2 diffVect = new Vector2(enemies[i].Position.X - playerPos.X, enemies[i].Position.Z - playerPos.Z);
                float distance = diffVect.LengthSquared();

                // Check if enemy is within RadarRange
                if (distance < RadarRangeSquared)
                {
                    // Scale the distance from world coords to radar coords
                    diffVect *= RadarScreenRadius / RadarRange;

                    // We rotate each point on the radar so that the player is always facing UP on the radar
                    //diffVect = Vector2.Transform(diffVect, Matrix.CreateRotationZ(playerForwardRadians));

                    // Offset coords from radar's center
                    diffVect = -diffVect;
                    diffVect += RadarCenterPos;

                    // We scale each dot so that enemies that are at higher elevations have bigger dots, and enemies
                    // at lower elevations have smaller dots.
                    float scaleHeight = 1.0f + ((enemies[i].Position.Y - playerPos.Y) / 200.0f);

                    // Draw enemy dot on radar
                    if (enemies[i].isBigBoss)
                        spriteBatch.Draw(BigBossDotImage, diffVect, null, Color.White, 0.0f, new Vector2(BigBossDotImage.Width / 2, BigBossDotImage.Height / 2), scaleHeight, SpriteEffects.None, 0.0f);
                    else spriteBatch.Draw(EnemyDotImage, diffVect, null, Color.White, 0.0f, new Vector2(EnemyDotImage.Width / 2, EnemyDotImage.Height / 2), scaleHeight, SpriteEffects.None, 0.0f);
                }
            }
            // If fish is in range
            for (int i = 0; i < fishAmount; i++)
            {
                Vector2 diffVect = new Vector2(fishes[i].Position.X - playerPos.X, fishes[i].Position.Z - playerPos.Z);
                float distance = diffVect.LengthSquared();

                // Check if enemy is within RadarRange
                if (distance < RadarRangeSquared)
                {
                    // Scale the distance from world coords to radar coords
                    diffVect *= RadarScreenRadius / RadarRange;

                    // We rotate each point on the radar so that the player is always facing UP on the radar
                    //diffVect = Vector2.Transform(diffVect, Matrix.CreateRotationZ(playerForwardRadians));

                    // Offset coords from radar's center
                    diffVect = -diffVect;
                    diffVect += RadarCenterPos;

                    // We scale each dot so that enemies that are at higher elevations have bigger dots, and enemies
                    // at lower elevations have smaller dots.
                    float scaleHeight = 1.0f + ((fishes[i].Position.Y - playerPos.Y) / 200.0f);

                    // Draw enemy dot on radar
                    spriteBatch.Draw(FishDotImage, diffVect, null, Color.White, 0.0f, new Vector2(FishDotImage.Width / 2, FishDotImage.Height / 2), scaleHeight, SpriteEffects.None, 0.0f);
                }
            }
            

            // Draw player's dot last
            //spriteBatch.Draw(PlayerDotImage, RadarCenterPos, Color.White);
            spriteBatch.Draw(PlayerDotImage, RadarCenterPos, null, Color.White, 0.0f, new Vector2(PlayerDotImage.Width / 2, PlayerDotImage.Height / 2), 1.0f, SpriteEffects.None, 0.0f); 
        }
    }
}
