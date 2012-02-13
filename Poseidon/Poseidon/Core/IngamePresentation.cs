using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Poseidon.Core
{
    public static class IngamePresentation
    {
        //static TimeSpan playTime = TimeSpan.Zero;
        static TimeSpan lastChangeTime = TimeSpan.Zero;
        static TimeSpan accelerationTime = TimeSpan.FromSeconds(3);
        static TimeSpan startAccelerationTime = TimeSpan.Zero;
        static float partialDraw = 0.0f;
        static float spinningSpeed = 0.1f;
        static bool goingToStopSpinning = false;
        static bool stoppedSpinning = true;
        static Random random = new Random();
        //textures for good will bar
        static Texture2D[] iconTextures;
        static Texture2D GoodWillBar;

        public static int poseidonFace = 0, strengthIcon = 1, speedIcon = 2, shootRateIcon = 3, healthIcon = 4, bowIcon = 5, hammerIcon = 6,
            armorIcon = 7, sandalIcon = 8, beltIcon = 9, dolphinIcon = 10, seaCowIcon = 11, turtleIcon = 12;

        public static void InitiateGoodWillBarGraphic(ContentManager Content)
        {
            //load icons for good will bar
            iconTextures = new Texture2D[GameConstants.NumGoodWillBarIcons];
            iconTextures[poseidonFace] = Content.Load<Texture2D>("Image/SpinningReel/poseidonFace");
            iconTextures[strengthIcon] = Content.Load<Texture2D>("Image/SpinningReel/strengthIcon");
            iconTextures[speedIcon] = Content.Load<Texture2D>("Image/SpinningReel/speedIcon");
            iconTextures[shootRateIcon] = Content.Load<Texture2D>("Image/SpinningReel/shootRateIcon");
            iconTextures[bowIcon] = Content.Load<Texture2D>("Image/SpinningReel/bowIcon");
            iconTextures[hammerIcon] = Content.Load<Texture2D>("Image/SpinningReel/hammerIcon");
            iconTextures[armorIcon] = Content.Load<Texture2D>("Image/SpinningReel/armorIcon");
            iconTextures[sandalIcon] = Content.Load<Texture2D>("Image/SpinningReel/sandalIcon");
            iconTextures[beltIcon] = Content.Load<Texture2D>("Image/SpinningReel/beltIcon");
            iconTextures[healthIcon] = Content.Load<Texture2D>("Image/SpinningReel/healthIcon");
            iconTextures[dolphinIcon] = Content.Load<Texture2D>("Image/SpinningReel/dolphinIcon");
            iconTextures[seaCowIcon] = Content.Load<Texture2D>("Image/SpinningReel/seaCowIcon");
            iconTextures[turtleIcon] = Content.Load<Texture2D>("Image/SpinningReel/turtleIcon");
            GoodWillBar = Content.Load<Texture2D>("Image/Miscellaneous/EnvironmentBar");
        }
        public static void DrawActiveSkill(GraphicsDevice GraphicDevice, Texture2D[] skillTextures, SpriteBatch spriteBatch)
        {
            int xOffsetText, yOffsetText;
            Rectangle rectSafeArea;

            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            //xOffsetText = rectSafeArea.Right - 400;
            xOffsetText = rectSafeArea.Center.X + 150;
            yOffsetText = rectSafeArea.Bottom - 100;

            //Vector2 skillIconPosition =
            //    new Vector2((int)xOffsetText, (int)yOffsetText);
            Rectangle destRectangle = new Rectangle(xOffsetText, yOffsetText, 96, 96);

            //spriteBatch.Draw(skillTextures[tank.activeSkillID], skillIconPosition, Color.White);
            spriteBatch.Draw(skillTextures[HydroBot.activeSkillID], destRectangle, Color.White);

            //draw the 2nd skill icon if skill combo activated
            if (HydroBot.skillComboActivated && HydroBot.secondSkillID != -1)
            {
                destRectangle = new Rectangle(xOffsetText + 100, yOffsetText, 96, 96);
                spriteBatch.Draw(skillTextures[HydroBot.secondSkillID], destRectangle, Color.White);
            }
        }
        public static void DrawHealthBar(Texture2D HealthBar, Game game, SpriteBatch spriteBatch, SpriteFont statsFont, int currentHealth, int maxHealth, int heightFromTop, string type, Color typeColor)
        {
            int barX = game.Window.ClientBounds.Width / 2 - HealthBar.Width / 2;
            int barY = heightFromTop;
            int barHeight = 22;
            double healthiness = (double)currentHealth / maxHealth;

            //Draw the negative space for the health bar
            spriteBatch.Draw(HealthBar,
                new Rectangle(barX, barY, HealthBar.Width, barHeight),
                new Rectangle(0, barHeight + 1, HealthBar.Width, barHeight),
                Color.Transparent);
            //Draw the current health level based on the current Health
            Color healthColor = Color.LawnGreen;
            if (healthiness < 0.2)
                healthColor = Color.DarkRed;
            else if (healthiness < 0.5)
                healthColor = Color.Orange;
            spriteBatch.Draw(HealthBar,
                new Rectangle(barX, barY, (int)(HealthBar.Width * healthiness), barHeight),
                new Rectangle(0, barHeight + 1, HealthBar.Width, barHeight),
                healthColor);
            //Draw the box around the health bar
            spriteBatch.Draw(HealthBar,
                new Rectangle(barX, barY, HealthBar.Width, barHeight),
                new Rectangle(0, 0, HealthBar.Width, barHeight),
                Color.White);
            spriteBatch.DrawString(statsFont, type.ToUpper(), new Vector2(game.Window.ClientBounds.Width / 2 - ((type.Length / 2) * 14), heightFromTop - 1), typeColor);
        }

        public static void DrawEnvironmentBar(Texture2D Bar, Game game, SpriteBatch spriteBatch, SpriteFont statsFont, int currentEnvironment, int maxEnvironemnt)
        {
            int barX = game.Window.ClientBounds.Width - 50;
            int barY = game.Window.ClientBounds.Height / 2 - Bar.Height / 2;
            string type = "ENVIRONMENT";
            Color typeColor = Color.Black;
            int barWidth = Bar.Width / 2;
            double healthiness = (double)currentEnvironment / maxEnvironemnt;
            //Draw the negative space for the health bar
            spriteBatch.Draw(Bar,
                new Rectangle(barX, barY, barWidth, Bar.Height),
                new Rectangle(barWidth + 1, 0, barWidth, Bar.Height),
                Color.Transparent);
            //Draw the current health level based on the current Health
            Color healthColor = Color.Gold;
            if (healthiness < 0.2)
                healthColor = Color.DarkRed;
            else if (healthiness < 0.5)
                healthColor = Color.Red;
            else if (healthiness < 0.8)
                healthColor = Color.LawnGreen;
            spriteBatch.Draw(Bar,
                new Rectangle(barX, barY + (Bar.Height - (int)(Bar.Height * healthiness)), barWidth, (int)(Bar.Height * healthiness)),
                new Rectangle(barWidth + 1, 0, barWidth, Bar.Height),
                healthColor);
            //Draw the box around the health bar
            spriteBatch.Draw(Bar,
                new Rectangle(barX, barY, barWidth, Bar.Height),
                new Rectangle(0, 0, barWidth, Bar.Height),
                Color.White);
            //spriteBatch.DrawString(statsFont, type.ToUpper(), new Vector2(game.Window.ClientBounds.Width / 2 - ((type.Length / 2) * 14), heightFromTop - 1), typeColor);
            //spriteBatch.DrawString(statsFont, type.ToUpper(), new Vector2(barX + 10, barY + 20), typeColor, 90.0f, new Vector2(barX + 10, barY + 20), 1, SpriteEffects.FlipVertically, 0);
            spriteBatch.DrawString(statsFont, type.ToUpper(), new Vector2(barX + 35, barY + 70), typeColor, 3.14f / 2, new Vector2(0, 0), 1, SpriteEffects.None, 0);
        }

        public static void UpdateGoodWillBar()
        {
            int faceBeingShown;
            if (!HydroBot.goodWillBarActivated) return;
            if (PoseidonGame.playTime - lastChangeTime >= TimeSpan.FromMilliseconds(20) && !stoppedSpinning)
            {
                if (PoseidonGame.playTime - startAccelerationTime <= accelerationTime)
                    spinningSpeed += 0.003f;
                else
                {
                    if (spinningSpeed <= 0.05 && partialDraw < 1.0f)
                    {
                        goingToStopSpinning = true;
                    }
                    else
                    {
                        if (!goingToStopSpinning)
                            spinningSpeed -= 0.0005f;
                    }
                }

                partialDraw += spinningSpeed;
                if (partialDraw >= 1.0f)
                {
                    if (!goingToStopSpinning)
                    {
                        HydroBot.faceToDraw++;
                        partialDraw = partialDraw - 1.0f;
                        PoseidonGame.audio.reelHit.Play();
  
                    }
                    else
                    {
                        partialDraw = 1.0f;
                        stoppedSpinning = true;
                        PoseidonGame.audio.reelHit.Play();

                        //miracles happen here
                        //the face drawn on the screen is actually faceToDraw + 1
                        faceBeingShown = HydroBot.faceToDraw + 1;
                        if (faceBeingShown == GameConstants.NumGoodWillBarIcons) faceBeingShown = 0;
                        if (HydroBot.iconActivated[faceBeingShown]){
                            if (faceBeingShown == poseidonFace)
                            {
                                //fill up the hitpoint
                                HydroBot.currentHitPoint = HydroBot.lsMaxHitPoint;
                                //reset all skills' cooldown
                                for (int i = 0; i < GameConstants.numberOfSkills; i++)
                                {
                                    HydroBot.firstUse[i] = false;
                                }
                            }
                            else if (faceBeingShown == strengthIcon)
                            {
                                HydroBot.strength += GameConstants.gainStrength;
                            }
                            else if (faceBeingShown == speedIcon)
                            {
                                HydroBot.speed += GameConstants.gainSpeed;
                            }
                            else if (faceBeingShown == shootRateIcon)
                            {
                                HydroBot.shootingRate += GameConstants.gainShootingRate;
                            }
                            else if (faceBeingShown == healthIcon)
                            {
                                HydroBot.currentHitPoint += GameConstants.gainHitPoint;
                                HydroBot.maxHitPoint += GameConstants.gainHitPoint;
                            }
                            else if (faceBeingShown == bowIcon)
                            {
                                HydroBot.bowPower += 0.05f;
                            }
                            else if (faceBeingShown == hammerIcon)
                            {
                                HydroBot.hammerPower += 0.05f;
                            }
                            else if (faceBeingShown == armorIcon)
                            {
                                HydroBot.armorPower += 0.05f;
                            }
                            else if (faceBeingShown == sandalIcon)
                            {
                                HydroBot.sandalPower += 0.05f;
                            }
                            else if (faceBeingShown == beltIcon)
                            {
                                HydroBot.beltPower += 0.05f;
                            }
                            else if (faceBeingShown == dolphinIcon)
                            {
                                HydroBot.dolphinPower += 0.05f;
                            }
                            else if (faceBeingShown == seaCowIcon)
                            {
                                HydroBot.seaCowPower += 0.05f;
                            }
                            else if (faceBeingShown == turtleIcon)
                            {
                                HydroBot.turtlePower += 0.05f;
                            }

                        }
                        
                    }

                }
                if (HydroBot.faceToDraw == iconTextures.Length) HydroBot.faceToDraw = 0;
                lastChangeTime = PoseidonGame.playTime;
            }
        }
        public static void StopSpinning()
        {
            stoppedSpinning = true;
            partialDraw = 1.0f;
        }
        public static void SpinNow()
        {
            if (stoppedSpinning == true)
            {
                startAccelerationTime = PoseidonGame.playTime;
                spinningSpeed = 0.1f;
                goingToStopSpinning = stoppedSpinning = false;
                accelerationTime = TimeSpan.FromSeconds(random.Next(3) + 1);
            }
        }
        public static void DrawGoodWillBar(Game game, SpriteBatch spriteBatch, SpriteFont statsFont)
        {
            if (!HydroBot.goodWillBarActivated) return;
            // draw the bar
            int barX = game.Window.ClientBounds.Left + 50;
            int barY = game.Window.ClientBounds.Center.Y - GoodWillBar.Height / 2;
            string type = "GOOD WILL";
            Color typeColor = Color.Gold;
            int barWidth = GoodWillBar.Width / 2;
            double healthiness = (double)HydroBot.goodWillPoint / HydroBot.maxGoodWillPoint;
            //Draw the negative space for the health bar
            spriteBatch.Draw(GoodWillBar,
                new Rectangle(barX, barY, barWidth, GoodWillBar.Height),
                new Rectangle(barWidth + 1, 0, barWidth, GoodWillBar.Height),
                Color.Transparent);
            //Draw the current health level based on the current Health
            Color healthColor = Color.Gold;
            if (healthiness < 0.2)
                healthColor = Color.DarkRed;
            else if (healthiness < 0.5)
                healthColor = Color.Red;
            else if (healthiness < 0.8)
                healthColor = Color.LawnGreen;
            spriteBatch.Draw(GoodWillBar,
                new Rectangle(barX, barY + (GoodWillBar.Height - (int)(GoodWillBar.Height * healthiness)), barWidth, (int)(GoodWillBar.Height * healthiness)),
                new Rectangle(barWidth + 1, 0, barWidth, GoodWillBar.Height),
                healthColor);
            //Draw the box around the health bar
            spriteBatch.Draw(GoodWillBar,
                new Rectangle(barX, barY, barWidth, GoodWillBar.Height),
                new Rectangle(0, 0, barWidth, GoodWillBar.Height),
                Color.White);
            //spriteBatch.DrawString(statsFont, type.ToUpper(), new Vector2(game.Window.ClientBounds.Width / 2 - ((type.Length / 2) * 14), heightFromTop - 1), typeColor);
            //spriteBatch.DrawString(statsFont, type.ToUpper(), new Vector2(barX + 10, barY + 20), typeColor, 90.0f, new Vector2(barX + 10, barY + 20), 1, SpriteEffects.FlipVertically, 0);
            spriteBatch.DrawString(statsFont, type.ToUpper(), new Vector2(barX + 10, barY + 200), typeColor, -3.14f / 2, new Vector2(0, 0), 1, SpriteEffects.None, 0);

            //draw the spinning reel on top of the bar
            Color colorToDraw;
            int faceDrawNext = HydroBot.faceToDraw + 1;
            if (faceDrawNext == iconTextures.Length) faceDrawNext = 0;
            if (HydroBot.iconActivated[faceDrawNext]) colorToDraw = Color.White;
            else colorToDraw = Color.Black;
            spriteBatch.Draw(iconTextures[faceDrawNext], new Vector2(barX - 32, barY - iconTextures[faceDrawNext].Height - 20), new Rectangle(0, (int)(iconTextures[faceDrawNext].Height * (1.0f - partialDraw)), iconTextures[faceDrawNext].Width, (int)(iconTextures[faceDrawNext].Height * partialDraw)), colorToDraw);
            if (HydroBot.iconActivated[HydroBot.faceToDraw]) colorToDraw = Color.White;
            else colorToDraw = Color.Black;
            spriteBatch.Draw(iconTextures[HydroBot.faceToDraw], new Vector2(barX - 32, barY - iconTextures[faceDrawNext].Height - 20 + iconTextures[HydroBot.faceToDraw].Height * partialDraw), new Rectangle(0, 0, iconTextures[HydroBot.faceToDraw].Width, (int)(iconTextures[HydroBot.faceToDraw].Height * (1.0f - partialDraw))), colorToDraw);
            
        }

        public static void DrawLevelBar(Texture2D LevelBar, Game game, SpriteBatch spriteBatch, SpriteFont statsFont, int currentExperience, int nextLevelExp, int level, int heightFromTop, string type, Color typeColor)
        {
            int barX = game.Window.ClientBounds.Width / 2 - LevelBar.Width / 2;
            int barY = heightFromTop;
            int barHeight = 22;
            double experience = (double)currentExperience / nextLevelExp;
            type += " " + level.ToString();
            //Draw the negative space for the health bar
            spriteBatch.Draw(LevelBar,
                new Rectangle(barX, barY, LevelBar.Width, barHeight),
                new Rectangle(0, barHeight + 1, LevelBar.Width, barHeight),
                Color.Transparent);
            //Draw the current health level based on the current Health
            spriteBatch.Draw(LevelBar,
                new Rectangle(barX, barY, (int)(LevelBar.Width * experience), barHeight),
                new Rectangle(0, barHeight + 1, LevelBar.Width, barHeight),
                Color.Aqua);
            //Draw the box around the health bar
            spriteBatch.Draw(LevelBar,
                new Rectangle(barX, barY, LevelBar.Width, barHeight),
                new Rectangle(0, 0, LevelBar.Width, barHeight),
                Color.White);
            spriteBatch.DrawString(statsFont, type.ToUpper(), new Vector2(game.Window.ClientBounds.Width / 2 - ((type.Length / 2) * 11), heightFromTop - 1), typeColor);
        }

        public static string wrapLine(string input_line, int width, SpriteFont font)
        {
            String line = String.Empty;
            String returnString = String.Empty;
            String[] wordArray = input_line.Split(' ');

            foreach (String word in wordArray)
            {
                if (font.MeasureString(line + word).Length() > width)
                {
                    returnString = returnString + line + '\n';
                    line = String.Empty;
                }
                line = line + word + ' ';
            }
            return returnString + line;
        }

        public static string wrapLine(string input_line, int width, SpriteFont font, float scale)
        {
            String line = String.Empty;
            String returnString = String.Empty;
            String[] wordArray = input_line.Split(' ');

            foreach (String word in wordArray)
            {
                if (font.MeasureString(line + word).Length()*scale > width)
                {
                    returnString = returnString + line + '\n';
                    line = String.Empty;
                }
                line = line + word + ' ';
            }
            return returnString + line;
        }
    }
}
