﻿using System;
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
        private Texture2D RadarImage;

        // Local coords of the radar image's center, used to offset image when being drawn
        private Vector2 RadarImageCenter;

        // Distance that the radar can "see"
        private const float RadarRange = GameConstants.RadarRange;
        private const float RadarRangeSquared = RadarRange * RadarRange;

        // Radius of radar circle on the screen
        public static float RadarScreenRadius = GameConstants.RadarScreenRadius;

        // This is the center position of the radar hud on the screen. 
        private Vector2 RadarCenterPos;// = new Vector2(500, 175);

        public Radar(ContentManager Content, string playerDotPath, string enemyDotPath, string radarImagePath, Vector2 radarCenter)
        {
            PlayerDotImage = Content.Load<Texture2D>(playerDotPath);
            EnemyDotImage = Content.Load<Texture2D>(enemyDotPath);
            RadarImage = Content.Load<Texture2D>(radarImagePath);
            this.RadarCenterPos = radarCenter;
            RadarImageCenter = new Vector2(RadarImage.Width * 0.5f, RadarImage.Height * 0.5f);
        }

        public void Draw(SpriteBatch spriteBatch, Vector3 playerPos, Enemy[] enemies, int enemyAmount)
        {
            // The last parameter of the color determines how transparent the radar circle will be
            spriteBatch.Draw(RadarImage, RadarCenterPos, null, new Color(100, 100, 100, 150), 0.0f, RadarImageCenter, RadarScreenRadius / (RadarImage.Height * 0.5f), SpriteEffects.None, 0.0f);

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
                    spriteBatch.Draw(EnemyDotImage, diffVect, null, Color.White, 0.0f, new Vector2(0.0f, 0.0f), scaleHeight, SpriteEffects.None, 0.0f);
                }
            }

            // Draw player's dot last
            spriteBatch.Draw(PlayerDotImage, RadarCenterPos, Color.White);
        }
    }
}
