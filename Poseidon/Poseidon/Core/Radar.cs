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
        private Texture2D OrganicFactoryDotImage;
        private Texture2D PlasticFactoryDotImage;
        private Texture2D NuclearFactoryDotImage;
        private Texture2D ResearchFacilityDotImage;
        private Texture2D SideKickDotImage;
        private Texture2D GoldenKeyDot;

        // Local coords of the radar image's center, used to offset image when being drawn
        private Vector2 RadarImageCenter;

        // Distance that the radar can "see"
        private const float RadarRange = GameConstants.RadarRange;
        private const float RadarRangeSquared = RadarRange * RadarRange;

        // Radius of radar circle on the screen
        public static float RadarScreenRadius = GameConstants.RadarScreenRadius * GameConstants.generalTextScaleFactor;

        // This is the center position of the radar hud on the screen. 
        private Vector2 RadarCenterPos;// = new Vector2(500, 175);

        public Radar(ContentManager Content, string playerDotPath, string enemyDotPath, string fishDotPath, string radarImagePath, string bigBossDotPath, Game game)
        {
            PlayerDotImage = Content.Load<Texture2D>(playerDotPath);
            EnemyDotImage = Content.Load<Texture2D>(enemyDotPath);
            BigBossDotImage = Content.Load<Texture2D>(bigBossDotPath);
            FishDotImage = Content.Load<Texture2D>(fishDotPath);
            RadarImage = Content.Load<Texture2D>(radarImagePath);
            ShipwreckDotImage = Content.Load<Texture2D>("Image/RadarTextures/shipwreckDot");
            OrganicFactoryDotImage = Content.Load<Texture2D>("Image/RadarTextures/bioFactoryDot");
            PlasticFactoryDotImage = Content.Load<Texture2D>("Image/RadarTextures/plasticFactoryDot");
            NuclearFactoryDotImage = Content.Load<Texture2D>("Image/RadarTextures/nuclearFactoryDot");
            ResearchFacilityDotImage = Content.Load<Texture2D>("Image/RadarTextures/researchCenterDot");
            SideKickDotImage = Content.Load<Texture2D>("Image/RadarTextures/sidekickDot");
            GoldenKeyDot = Content.Load<Texture2D>("Image/RadarTextures/keydot");

            this.RadarCenterPos = new Vector2(game.GraphicsDevice.Viewport.TitleSafeArea.Right - RadarScreenRadius, game.GraphicsDevice.Viewport.TitleSafeArea.Bottom - RadarScreenRadius);
            RadarImageCenter = new Vector2(RadarImage.Width * 0.5f, RadarImage.Height * 0.5f);
        }

        public void Draw(SpriteBatch spriteBatch, Vector3 playerPos, BaseEnemy[] enemies, int enemyAmount, Fish[] fishes, int fishAmount, List<ShipWreck> shipWrecks, List<Factory> factories, ResearchFacility researchFacility, List<Powerpack> powerPacks)
        {
            // The last parameter of the color determines how transparent the radar circle will be
            spriteBatch.Draw(RadarImage, RadarCenterPos, null, Color.White, 0.0f, RadarImageCenter, RadarScreenRadius / (RadarImage.Height * 0.5f), SpriteEffects.None, 0.0f);
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
                        spriteBatch.Draw(ShipwreckDotImage, diffVect, null, Color.White, 0.0f, new Vector2(ShipwreckDotImage.Width / 2, ShipwreckDotImage.Height / 2), scaleHeight * 0.8f * GameConstants.generalTextScaleFactor, SpriteEffects.None, 0.0f);
                        //}
                    }
                }
            }

            // display factories on the map
            if (factories != null)
            {
                foreach (Factory factory in factories)
                {
                    Vector2 diffVect = new Vector2(factory.Position.X - playerPos.X, factory.Position.Z - playerPos.Z);
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
                        float scaleHeight = 1.0f + ((factory.Position.Y - playerPos.Y) / 200.0f);

                        // Draw factory dot on radar
                        Texture2D thisImage;
                        switch (factory.factoryType)
                        {
                            case FactoryType.biodegradable:
                                thisImage = OrganicFactoryDotImage;
                                break;
                            case FactoryType.plastic:
                                thisImage = PlasticFactoryDotImage;
                                break;
                            case FactoryType.radioactive:
                                thisImage = NuclearFactoryDotImage;
                                break;
                            default:
                                thisImage = OrganicFactoryDotImage;
                                break;
                        }
                        spriteBatch.Draw(thisImage, diffVect, null, Color.White, 0.0f, new Vector2(thisImage.Width / 2, thisImage.Height / 2), scaleHeight * 0.8f * GameConstants.generalTextScaleFactor, SpriteEffects.None, 0.0f);
                    //}
                }
            }
            if (researchFacility != null)
            {
                Vector2 diffVect = new Vector2(researchFacility.Position.X - playerPos.X, researchFacility.Position.Z - playerPos.Z);
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
                    float scaleHeight = 1.0f + ((researchFacility.Position.Y - playerPos.Y) / 200.0f);

                    // Draw enemy dot on radar
                    spriteBatch.Draw(ResearchFacilityDotImage, diffVect, null, Color.White, 0.0f, new Vector2(ResearchFacilityDotImage.Width / 2, ResearchFacilityDotImage.Height / 2), scaleHeight * 0.8f * GameConstants.generalTextScaleFactor, SpriteEffects.None, 0.0f);
                //}
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
                        spriteBatch.Draw(BigBossDotImage, diffVect, null, Color.White, 0.0f, new Vector2(BigBossDotImage.Width / 2, BigBossDotImage.Height / 2), scaleHeight * GameConstants.generalTextScaleFactor, SpriteEffects.None, 0.0f);
                    else spriteBatch.Draw(EnemyDotImage, diffVect, null, Color.White, 0.0f, new Vector2(EnemyDotImage.Width / 2, EnemyDotImage.Height / 2), scaleHeight * GameConstants.generalTextScaleFactor, SpriteEffects.None, 0.0f);
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

                    if (fishes[i].isBigBoss) scaleHeight *= 1.5f;
                    // Draw fish dot on radar
                    if (fishes[i] is SeaCow || fishes[i] is SeaDolphin || fishes[i] is SeaTurtle)
                        spriteBatch.Draw(SideKickDotImage, diffVect, null, Color.White, 0.0f, new Vector2(FishDotImage.Width / 2, FishDotImage.Height / 2), scaleHeight * GameConstants.generalTextScaleFactor, SpriteEffects.None, 0.0f);
                    else spriteBatch.Draw(FishDotImage, diffVect, null, Color.White, 0.0f, new Vector2(FishDotImage.Width / 2, FishDotImage.Height / 2), scaleHeight * GameConstants.generalTextScaleFactor, SpriteEffects.None, 0.0f);
                }
            }

            // display golden key on the map
            if (powerPacks != null)
            {
                foreach (Powerpack powerPack in powerPacks)
                {
                    if (powerPack.powerType == PowerPackType.GoldenKey)
                    {
                        Vector2 diffVect = new Vector2(powerPack.Position.X - playerPos.X, powerPack.Position.Z - playerPos.Z);
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
                        float scaleHeight = 1.0f;

                        spriteBatch.Draw(GoldenKeyDot, diffVect, null, Color.White, 0.0f, new Vector2(GoldenKeyDot.Width / 2, GoldenKeyDot.Height / 2), scaleHeight * 0.4f * GameConstants.generalTextScaleFactor, SpriteEffects.None, 0.0f);
                        //}
                    }
                }
            }
            // Draw player's dot last
            //spriteBatch.Draw(PlayerDotImage, RadarCenterPos, Color.White);
            spriteBatch.Draw(PlayerDotImage, RadarCenterPos, null, Color.White, 0.0f, new Vector2(PlayerDotImage.Width / 2, PlayerDotImage.Height / 2), 1.0f * GameConstants.generalTextScaleFactor, SpriteEffects.None, 0.0f); 
        }
    }
}
