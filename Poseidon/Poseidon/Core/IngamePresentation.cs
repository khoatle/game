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
        static bool displayResult = false;
        static double timeStartDisplay;
        static float startingDisplayScale = 0.55f;
        static float resultDisplayScale = startingDisplayScale;
        static bool displayMusicPlayed = false, buzzNow = false;
        static Random random = new Random();
        //textures for good will bar
        static Texture2D[] iconTextures, resultTextures;
        static Texture2D GoodWillBar, EnvironmentBar, lobsterTexture, crabTexture, iconFrame;
        public static Texture2D buttonNormalTexture, buttonHoverTexture, buttonPressedTexture;
        static Texture2D HealthBar;

        public static SpriteFont statsFont, statisticFont, fishTalkFont;

        public static List<Bubble> bubbles;
        public static double lastBubbleCreated = 0;

        public static int poseidonFace = 0, strengthIcon = 1, speedIcon = 2, shootRateIcon = 3, healthIcon = 4, bowIcon = 5, hammerIcon = 6,
            armorIcon = 7, sandalIcon = 8, beltIcon = 9, dolphinIcon = 10, seaCowIcon = 11, turtleIcon = 12;

        // Texture and font for property window of a factory
        public static SpriteFont factoryFont;
        public static Texture2D factoryBackground;
        public static Texture2D factoryProduceButtonNormalTexture, factoryProduceButtonHoverTexture, factoryProduceButtonPressedTexture;
        public static Texture2D dummyTexture;

        // Texture and font for property window of a research facility
        public static SpriteFont facilityFont;
        public static SpriteFont facilityFont2;
        public static Texture2D facilityBackground;
        public static Texture2D facilityUpgradeButton;
        public static Texture2D playJigsawButton;
        public static Texture2D increaseAttributeButtonNormalTexture, increaseAttributeButtonHoverTexture, increaseAttributeButtonPressedTexture;

        //level winning/losing screen
        public static Texture2D winningTexture, losingTexture;

        //for displaying tip on screen
        public static int currentTipID = 0;
        public static double lastTipChange = 0;
        public static int lastCurrentLevel = 0;
        public static float fadeFactorBeginValue = 4.0f;
        public static float fadeFactorReduceStep = 0.01f;
        public static float fadeFactor = fadeFactorBeginValue;

        //to next level button
        public static Texture2D toNextLevelNormalTexture, toNextLevelHoverTexture;

        public static void Initiate2DGraphics(ContentManager Content)
        {
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
            resultTextures = new Texture2D[GameConstants.NumGoodWillBarIcons];
            resultTextures[poseidonFace] = Content.Load<Texture2D>("Image/SpinningReel/poseiResult");
            resultTextures[strengthIcon] = Content.Load<Texture2D>("Image/SpinningReel/attributeIncreased");
            resultTextures[speedIcon] = Content.Load<Texture2D>("Image/SpinningReel/attributeIncreased");
            resultTextures[shootRateIcon] = Content.Load<Texture2D>("Image/SpinningReel/attributeIncreased");
            resultTextures[bowIcon] = Content.Load<Texture2D>("Image/SpinningReel/skillIncreased");
            resultTextures[hammerIcon] = Content.Load<Texture2D>("Image/SpinningReel/skillIncreased");
            resultTextures[armorIcon] = Content.Load<Texture2D>("Image/SpinningReel/skillIncreased");
            resultTextures[sandalIcon] = Content.Load<Texture2D>("Image/SpinningReel/skillIncreased");
            resultTextures[beltIcon] = Content.Load<Texture2D>("Image/SpinningReel/skillIncreased");
            resultTextures[healthIcon] = Content.Load<Texture2D>("Image/SpinningReel/attributeIncreased");
            resultTextures[dolphinIcon] = Content.Load<Texture2D>("Image/SpinningReel/friendshipIncreased");
            resultTextures[seaCowIcon] = Content.Load<Texture2D>("Image/SpinningReel/friendshipIncreased");
            resultTextures[turtleIcon] = Content.Load<Texture2D>("Image/SpinningReel/friendshipIncreased");

            buttonNormalTexture = Content.Load<Texture2D>("Image/ButtonTextures/buttonFrame");
            buttonHoverTexture = Content.Load<Texture2D>("Image/ButtonTextures/buttonFrameHover");
            buttonPressedTexture = Content.Load<Texture2D>("Image/ButtonTextures/buttonFramePressed");

            // Load Textures and fonts for factory control panel
            factoryFont = Content.Load<SpriteFont>("Fonts/factoryConfig");
            factoryBackground = Content.Load<Texture2D>("Image/TrashManagement/futuristicControlPanel");
            factoryProduceButtonNormalTexture = Content.Load<Texture2D>("Image/ButtonTextures/buttonLargeFrame"); //("Image/TrashManagement/ChangeFactoryProduceBox");
            factoryProduceButtonHoverTexture = Content.Load<Texture2D>("Image/ButtonTextures/buttonFrameHover");
            factoryProduceButtonPressedTexture = Content.Load<Texture2D>("Image/ButtonTextures/buttonFramePressed");
            dummyTexture = new Texture2D(PoseidonGame.graphics.GraphicsDevice, 2, 2); // create a dummy 2x2 texture
            dummyTexture.SetData(new int[4]);


            // Load Textures and fonts for research facility property dialog
            facilityFont = Content.Load<SpriteFont>("Fonts/factoryConfig");
            facilityFont2 = Content.Load<SpriteFont>("Fonts/factoryConfig");
            facilityBackground = Content.Load<Texture2D>("Image/TrashManagement/futuristicControlPanel2");
            facilityUpgradeButton = Content.Load<Texture2D>("Image/TrashManagement/increaseAttributeButton");
            playJigsawButton = Content.Load<Texture2D>("Image/TrashManagement/upgradeButton");
            increaseAttributeButtonNormalTexture = Content.Load<Texture2D>("Image/TrashManagement/increaseAttributeButton");
            increaseAttributeButtonHoverTexture = Content.Load<Texture2D>("Image/TrashManagement/increaseAttributeButtonHover");
            increaseAttributeButtonPressedTexture = Content.Load<Texture2D>("Image/TrashManagement/increaseAttributeButtonPressed");

            winningTexture = Content.Load<Texture2D>("Image/SceneTextures/LevelWinNew");
            losingTexture = Content.Load<Texture2D>("Image/SceneTextures/GameOverNew");

            iconFrame = Content.Load<Texture2D>("Image/SpinningReel/transparent_frame");
            GoodWillBar = Content.Load<Texture2D>("Image/Miscellaneous/goodWillBar");
            EnvironmentBar = Content.Load<Texture2D>("Image/Miscellaneous/EnvironmentBarNew");
            HealthBar = Content.Load<Texture2D>("Image/Miscellaneous/HealthBarNew");
            lobsterTexture = Content.Load<Texture2D>("Image/Miscellaneous/lobster");
            crabTexture = Content.Load<Texture2D>("Image/Miscellaneous/crab");
            statsFont = Content.Load<SpriteFont>("Fonts/StatsFont");
            fishTalkFont = Content.Load<SpriteFont>("Fonts/fishTalk");
            bubbles = new List<Bubble>();

            statisticFont = Content.Load<SpriteFont>("Fonts/statisticsfont");

            toNextLevelHoverTexture = Content.Load<Texture2D>("Image/Miscellaneous/tonextlevelhover");
            toNextLevelNormalTexture = Content.Load<Texture2D>("Image/Miscellaneous/tonextlevel");
        }
        
        public static void DrawLiveTip(GraphicsDevice GraphicDevice,SpriteBatch spriteBatch)
        {
            Rectangle rectSafeArea;
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;
            float xOffsetText, yOffsetText;
            xOffsetText = rectSafeArea.X + 10;
            yOffsetText = rectSafeArea.Y + 50;

            if (lastCurrentLevel != PlayGameScene.currentLevel)
            {
                currentTipID = 0;
                lastCurrentLevel = PlayGameScene.currentLevel;
                lastTipChange = PoseidonGame.playTime.TotalSeconds;
                fadeFactor = fadeFactorBeginValue;
            }
            string tipStr = "Tip: " + PoseidonGame.liveTipManager.allTips[PlayGameScene.currentLevel][currentTipID].tipItemStr;
            tipStr = wrapLine(tipStr, rectSafeArea.Width / 4, statsFont);
            spriteBatch.DrawString(statsFont, tipStr, new Vector2(xOffsetText, yOffsetText), Color.White * fadeFactor);
            fadeFactor -= fadeFactorReduceStep;

            if (PoseidonGame.playTime.TotalSeconds - lastTipChange >= 15)
            {
                currentTipID++;
                if (currentTipID == PoseidonGame.liveTipManager.allTips[PlayGameScene.currentLevel].Count) currentTipID = 0;
                lastTipChange = PoseidonGame.playTime.TotalSeconds;
                fadeFactor = fadeFactorBeginValue;
                PoseidonGame.audio.PowerGet.Play();
            }
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
        public static void DrawHealthBar(Game game, SpriteBatch spriteBatch, SpriteFont statsFont, int currentHealth, int maxHealth, int heightFromTop, string type, Color typeColor)
        {
            type = type.ToUpper();
            int barLength = (int)(statsFont.MeasureString(type).X * 1.5f);
            if (barLength < HealthBar.Width) barLength = HealthBar.Width;
            int barHeight = 22;
            int barX = game.Window.ClientBounds.Width / 2 - barLength / 2;
            int barY = heightFromTop;

            double healthiness = (double)currentHealth / maxHealth;

            //Draw the negative space for the health bar
            //spriteBatch.Draw(HealthBar,
            //    new Rectangle(barX, barY, barLength, barHeight),
            //    new Rectangle(0, barHeight + 1, HealthBar.Width, barHeight),
            //    Color.Transparent);
            //Draw the current health level based on the current Health
            Color healthColor = Color.LimeGreen;
            if (healthiness < 0.2)
                healthColor = Color.DarkRed;
            else if (healthiness < 0.5)
                healthColor = Color.Orange;
            spriteBatch.Draw(HealthBar,
                new Rectangle(barX, barY, (int)(barLength * healthiness), barHeight),
                new Rectangle(0, barHeight + 1, (int)(HealthBar.Width * healthiness), barHeight),
                healthColor);
            //Draw the box around the health bar
            spriteBatch.Draw(HealthBar,
                new Rectangle(barX, barY, barLength, barHeight),
                new Rectangle(0, 0, HealthBar.Width, barHeight),
                Color.White);
            spriteBatch.DrawString(statsFont, type, new Vector2(game.Window.ClientBounds.Width / 2 - statsFont.MeasureString(type).X / 2, heightFromTop - 1), Color.MediumVioletRed);
        }

        public static void DrawEnvironmentBar(Game game, SpriteBatch spriteBatch, SpriteFont statsFont, int currentEnvironment, int maxEnvironemnt)
        {
            int barX = game.Window.ClientBounds.Width - 50;
            int barY = game.Window.ClientBounds.Height / 2 - EnvironmentBar.Height / 2;
            string type = "ENVIRONMENT";
            Color typeColor = Color.IndianRed;
            int barWidth = 41;// EnvironmentBar.Width / 2;
            double healthiness = (double)currentEnvironment / maxEnvironemnt;
            //Draw the negative space for the health bar
            //spriteBatch.Draw(EnvironmentBar,
            //    new Rectangle(barX, barY, barWidth, EnvironmentBar.Height),
            //    new Rectangle(barWidth + 1, 0, barWidth, EnvironmentBar.Height),
            //    Color.Transparent);
            //Draw the current health level based on the current Health
            Color healthColor = Color.Gold;
            if (healthiness < 0.2)
                healthColor = Color.DarkRed;
            else if (healthiness < 0.5)
                healthColor = Color.Red;
            else if (healthiness < 0.8)
                healthColor = Color.LawnGreen;
            spriteBatch.Draw(EnvironmentBar,
                new Rectangle(barX, barY + (EnvironmentBar.Height - (int)(EnvironmentBar.Height * healthiness)), barWidth, (int)(EnvironmentBar.Height * healthiness)),
                new Rectangle(45, EnvironmentBar.Height - (int)(EnvironmentBar.Height * healthiness), 43, (int)(EnvironmentBar.Height * healthiness)),
                healthColor);
            //Draw the box around the health bar
            spriteBatch.Draw(EnvironmentBar,
                new Rectangle(barX, barY, barWidth, EnvironmentBar.Height),
                new Rectangle(0, 0, barWidth, EnvironmentBar.Height),
                Color.White);
            //spriteBatch.DrawString(statsFont, type.ToUpper(), new Vector2(game.Window.ClientBounds.Width / 2 - ((type.Length / 2) * 14), heightFromTop - 1), typeColor);
            //spriteBatch.DrawString(statsFont, type.ToUpper(), new Vector2(barX + 10, barY + 20), typeColor, 90.0f, new Vector2(barX + 10, barY + 20), 1, SpriteEffects.FlipVertically, 0);
            type = type.ToUpper();
            //spriteBatch.DrawString(statsFont, type.ToUpper(), new Vector2(barX + 35, barY + 70), typeColor, 3.14f / 2, new Vector2(0, 0), 1, SpriteEffects.None, 0);
            spriteBatch.DrawString(statsFont, type, new Vector2(barX + barWidth / 2 + statsFont.MeasureString(type).Y / 2, game.Window.ClientBounds.Height / 2 - statsFont.MeasureString(type).X / 2), Color.Gold, 3.14f / 2, new Vector2(0, 0), 1, SpriteEffects.None, 0);

            //draw the lobster on bottom
            //spriteBatch.Draw(lobsterTexture, new Vector2(barX + barWidth / 2, game.Window.ClientBounds.Height / 2 + EnvironmentBar.Height / 2 - 30), null, Color.White, 0, new Vector2(lobsterTexture.Width / 2, lobsterTexture.Height / 2), 1, SpriteEffects.None, 0);

            //draw floating bubbles inside tube
            if (PoseidonGame.playTime.TotalMilliseconds - lastBubbleCreated >= 250)
            {
                Bubble bubble = new Bubble();
                bubble.LoadContentBubbleSmall(PoseidonGame.contentManager, new Vector2(barX + barWidth / 2, barY + EnvironmentBar.Height - 7), barX, barX + barWidth - 3);
                bubbles.Add(bubble);
                lastBubbleCreated = PoseidonGame.playTime.TotalMilliseconds;
            }
            for (int i = 0; i < bubbles.Count; i++)
            {
                if (bubbles[i].bubble2DPos.Y - bubbles[i].bubbleTexture.Height/2 * bubbles[i].startingScale <= barY + (EnvironmentBar.Height - (int)(EnvironmentBar.Height * healthiness)) ||
                    bubbles[i].bubble2DPos.Y - bubbles[i].bubbleTexture.Height/2 * bubbles[i].startingScale <= barY + 4)
                    bubbles.RemoveAt(i--);

            }
            foreach (Bubble aBubble in bubbles)
            {
                aBubble.UpdateBubbleSmall();
                aBubble.DrawBubbleSmall(spriteBatch);
            }
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

                            //display result of the spin
                            displayResult = true;
                            timeStartDisplay = PoseidonGame.playTime.TotalMilliseconds;

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
            if (displayResult)
            {
                if (PoseidonGame.playTime.TotalMilliseconds - timeStartDisplay <= 500)
                {
                    resultDisplayScale -= 0.01f;
                }
                else if (PoseidonGame.playTime.TotalMilliseconds - timeStartDisplay <= 1000)
                {
                    if (resultDisplayScale > 0.26f) resultDisplayScale = 0.26f;
                    if (!displayMusicPlayed)
                    {
                        PoseidonGame.audio.superPunch.Play();
                        displayMusicPlayed = true;
                    }
                    buzzNow = true;
                }
                else if (PoseidonGame.playTime.TotalMilliseconds - timeStartDisplay <= 2500)
                {
                    buzzNow = false;
                }
                else
                {
                    displayResult = false;
                    resultDisplayScale = startingDisplayScale;
                    displayMusicPlayed = false;
                }
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

            int barX = iconFrame.Width/2 - GoodWillBar.Width/4;
            int barY = game.Window.ClientBounds.Height / 2 - GoodWillBar.Height / 2;
            string type = "GOOD WILL";
            Color typeColor = Color.Gold;
            int barWidth = 42;// EnvironmentBar.Width / 2;
            double healthiness = (double)HydroBot.goodWillPoint / HydroBot.maxGoodWillPoint;

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
                new Rectangle(45, GoodWillBar.Height - (int)(GoodWillBar.Height * healthiness), 43, (int)(GoodWillBar.Height * healthiness)),
                healthColor);

            //Draw the box around the health bar
            spriteBatch.Draw(GoodWillBar,
                new Rectangle(barX, barY, barWidth, EnvironmentBar.Height),
                new Rectangle(0, 0, barWidth, EnvironmentBar.Height),
                Color.White);

            //spriteBatch.DrawString(statsFont, type.ToUpper(), new Vector2(game.Window.ClientBounds.Width / 2 - ((type.Length / 2) * 14), heightFromTop - 1), typeColor);
            //spriteBatch.DrawString(statsFont, type.ToUpper(), new Vector2(barX + 10, barY + 20), typeColor, 90.0f, new Vector2(barX + 10, barY + 20), 1, SpriteEffects.FlipVertically, 0);
            type = type.ToUpper();
            //spriteBatch.DrawString(statsFont, type, new Vector2(barX + 10, barY + 200), typeColor, -3.14f / 2, new Vector2(0, 0), 1, SpriteEffects.None, 0);
            spriteBatch.DrawString(statsFont, type, new Vector2(barX + barWidth / 2 - statsFont.MeasureString(type).Y / 2, game.Window.ClientBounds.Height / 2 + statsFont.MeasureString(type).X / 2), Color.Gold, -3.14f / 2, new Vector2(0, 0), 1, SpriteEffects.None, 0);

            //draw the spinning reel on top of the bar
            Color colorToDraw;
            int faceDrawNext = HydroBot.faceToDraw + 1;
            if (faceDrawNext == iconTextures.Length) faceDrawNext = 0;
            if (HydroBot.iconActivated[faceDrawNext]) colorToDraw = Color.White;
            else colorToDraw = Color.Black;
            spriteBatch.Draw(iconTextures[faceDrawNext], new Vector2(0, barY - iconTextures[faceDrawNext].Height - 20), new Rectangle(0, (int)(iconTextures[faceDrawNext].Height * (1.0f - partialDraw)), iconTextures[faceDrawNext].Width, (int)(iconTextures[faceDrawNext].Height * partialDraw)), colorToDraw);
            if (HydroBot.iconActivated[HydroBot.faceToDraw]) colorToDraw = Color.White;
            else colorToDraw = Color.Black;
            spriteBatch.Draw(iconTextures[HydroBot.faceToDraw], new Vector2(0, barY - iconTextures[faceDrawNext].Height - 20 + iconTextures[HydroBot.faceToDraw].Height * partialDraw), new Rectangle(0, 0, iconTextures[HydroBot.faceToDraw].Width, (int)(iconTextures[HydroBot.faceToDraw].Height * (1.0f - partialDraw))), colorToDraw);

            //draw the frame
            spriteBatch.Draw(iconFrame, new Vector2(0, barY - iconFrame.Height - 20), null, Color.White);

            //draw the crab on bottom
            //spriteBatch.Draw(crabTexture, new Vector2(barX + barWidth / 2, game.Window.ClientBounds.Height / 2 + GoodWillBar.Height / 2 - 30), null, Color.White, 0, new Vector2(crabTexture.Width / 2, crabTexture.Height / 2), 1, SpriteEffects.None, 0);

            //display the result of the spin
            if (displayResult)
            {
                Vector2 posToDraw = new Vector2(iconFrame.Width / 2, barY - iconFrame.Height - 20);
                if (buzzNow) posToDraw += new Vector2(random.Next(-5, 5), random.Next(-5, 5));
                spriteBatch.Draw(resultTextures[faceDrawNext], posToDraw, null, Color.White, 0,
                    new Vector2(resultTextures[faceDrawNext].Width / 2, resultTextures[faceDrawNext].Height / 2), resultDisplayScale, SpriteEffects.None, 0);
            }
        }

        public static void DrawLevelBar(Game game, SpriteBatch spriteBatch, int currentExperience, int nextLevelExp, int level, int heightFromTop, string type, Color typeColor)
        {
            int barX = game.Window.ClientBounds.Width / 2 - HealthBar.Width / 2;
            int barY = heightFromTop;
            int barHeight = 22;
            double experience = (double)currentExperience / nextLevelExp;
            type += " " + level.ToString();
            //Draw the negative space for the health bar
            //spriteBatch.Draw(HealthBar,
            //    new Rectangle(barX, barY, HealthBar.Width, barHeight),
            //    new Rectangle(0, barHeight + 1, HealthBar.Width, barHeight),
            //    Color.Transparent);
            //Draw the current health level based on the current Health
            spriteBatch.Draw(HealthBar,
                new Rectangle(barX, barY, (int)(HealthBar.Width * experience), barHeight),
                new Rectangle(0, barHeight + 1, (int)(HealthBar.Width * experience), barHeight),
                Color.CornflowerBlue);
            //Draw the box around the health bar
            spriteBatch.Draw(HealthBar,
                new Rectangle(barX, barY, HealthBar.Width, barHeight),
                new Rectangle(0, 0, HealthBar.Width, barHeight),
                Color.White);
            //type = type.ToUpper();
            spriteBatch.DrawString(statsFont, type, new Vector2(game.Window.ClientBounds.Width / 2 - statsFont.MeasureString(type).X/2, heightFromTop - 1), Color.Gold);
        }

        public static void DrawObjectPointedAtStatus(Cursor cursor, Camera gameCamera, Game game, SpriteBatch spriteBatch, Fish[] fish, int fishAmount, BaseEnemy[] enemies, int enemiesAmount, List<Trash> trashes, List<ShipWreck> shipWrecks, List<Factory> factories, ResearchFacility researchFacility, List<TreasureChest> treasureChests, List<Powerpack> powerPacks, List<Resource> resources)
        {
            int commentMaxLength = game.Window.ClientBounds.Width / 4;

            //Display Fish Health
            Fish fishPointedAt = CursorManager.MouseOnWhichFish(cursor, gameCamera, fish, fishAmount);
            if (fishPointedAt != null)
            {
                IngamePresentation.DrawHealthBar(game, spriteBatch, statsFont, (int)fishPointedAt.health, (int)fishPointedAt.maxHealth, 5, fishPointedAt.Name, Color.Red);
                string line;
                line = "'";
                if (fishPointedAt.health < 20)
                {
                    line += "SAVE ME!!!";
                }
                else if (fishPointedAt.health < 60)
                {
                    line += IngamePresentation.wrapLine(fishPointedAt.sad_talk, commentMaxLength, fishTalkFont);
                }
                else
                {
                    line += IngamePresentation.wrapLine(fishPointedAt.happy_talk, commentMaxLength, fishTalkFont);
                }
                line += "'";
                spriteBatch.DrawString(fishTalkFont, line, new Vector2(game.Window.ClientBounds.Width / 2 - fishTalkFont.MeasureString(line).X / 2, 32), Color.Yellow);
            }
            else
            {
                //Display Enemy Health
                BaseEnemy enemyPointedAt = CursorManager.MouseOnWhichEnemy(cursor, gameCamera, enemies, enemiesAmount);
                if (enemyPointedAt != null)
                    IngamePresentation.DrawHealthBar(game, spriteBatch, statsFont, (int)enemyPointedAt.health, (int)enemyPointedAt.maxHealth, 5, enemyPointedAt.Name, Color.IndianRed);
                else
                {
                    Powerpack powerPackPointedAt = null, botOnPowerPack = null;
                    CursorManager.MouseOnWhichPowerPack(cursor, gameCamera, powerPacks, ref powerPackPointedAt, ref botOnPowerPack, null);
                    if (powerPackPointedAt != null)
                    {
                        string line, comment;
                        line = "";
                        comment = "";
                        if (powerPackPointedAt.powerType == PowerPackType.Speed)
                        {
                            line += "SPEED BOOST POWERPACK";
                            comment = "Temporarily doubles Hydrobot's movement speed.";
                        }
                        else if (powerPackPointedAt.powerType == PowerPackType.Strength)
                        {
                            line += "STRENGTH BOOST POWERPACK";
                            comment = "Temporarily doubles Hydrobot's power.";
                        }
                        else if (powerPackPointedAt.powerType == PowerPackType.FireRate)
                        {
                            line += "FIRE RATEBOOST POWERPACK";
                            comment = "Temporarily doubles Hydrobot's shooting speed.";
                        }
                        else if (powerPackPointedAt.powerType == PowerPackType.Health)
                        {
                            line += "HEALTH BOOST POWERPACK";
                            comment = "Replenishes Hydrobot's health.";
                        }
                        else if (powerPackPointedAt.powerType == PowerPackType.StrangeRock)
                        {
                            line += "STRANGE ROCK";
                            comment = "A rock that exhibits abnormal characteristics. Can be dropped at Research Center for analysing.";
                        }
                        else if (powerPackPointedAt.powerType == PowerPackType.GoldenKey)
                        {
                            line += "GOLDEN KEY";
                            comment = "Can open any treasure chest.";
                        }
                        spriteBatch.DrawString(statsFont, line, new Vector2(game.Window.ClientBounds.Width / 2 - statsFont.MeasureString(line).X / 2, 4), Color.Yellow);
                        comment = wrapLine(comment, commentMaxLength, statsFont);
                        spriteBatch.DrawString(statsFont, comment, new Vector2(game.Window.ClientBounds.Width / 2 - statsFont.MeasureString(comment).X / 2 , statsFont.MeasureString(line).Y + 2), Color.Red);
                    }
                    else
                    {
                        Resource resourcePointedAt = null, botOnResource = null;
                        CursorManager.MouseOnWhichResource(cursor, gameCamera, resources, ref resourcePointedAt, ref botOnResource, null);
                        if (resourcePointedAt != null)
                        {
                            string line, comment;
                            line = "RECYCLED RESOURCE BOX";
                            comment = "A box contains recycled resource produced by the processing plant. Recycled resources can be used to construct new facilities.";
                            spriteBatch.DrawString(statsFont, line, new Vector2(game.Window.ClientBounds.Width / 2 - statsFont.MeasureString(line).X / 2, 4), Color.Yellow);
                            comment = wrapLine(comment, commentMaxLength, statsFont);
                            spriteBatch.DrawString(statsFont, comment, new Vector2(game.Window.ClientBounds.Width / 2 - statsFont.MeasureString(comment).X / 2, statsFont.MeasureString(line).Y + 2), Color.Red);
                        }
                        else
                        {
                            TreasureChest chestPointedAt = CursorManager.MouseOnWhichChest(cursor, gameCamera, treasureChests);
                            if (chestPointedAt != null)
                            {
                                string line, comment;
                                line = "TREASURE CHEST";
                                comment = "Contains valuables sunk with the ship hundreds years ago.";
                                spriteBatch.DrawString(statsFont, line, new Vector2(game.Window.ClientBounds.Width / 2 - statsFont.MeasureString(line).X / 2, 4), Color.Yellow);
                                comment = wrapLine(comment, commentMaxLength, statsFont);
                                spriteBatch.DrawString(statsFont, comment, new Vector2(game.Window.ClientBounds.Width / 2 - statsFont.MeasureString(comment).X / 2, statsFont.MeasureString(line).Y + 2), Color.Red);
                            }
                            Trash trashPointedAt = null, botOnTrash = null;
                            CursorManager.MouseOnWhichTrash(cursor, gameCamera, trashes, ref trashPointedAt, ref botOnTrash, null);
                            if (trashPointedAt != null)
                            {
                                string line, comment;
                                line = "";
                                comment = "";
                                if (trashPointedAt.trashType == TrashType.biodegradable)
                                {
                                    line += "BIODEGRADABLE TRASH";
                                    comment = "Organic, will emit greenhouse gases unless processed in a processing plant.";
                                }
                                else if (trashPointedAt.trashType == TrashType.plastic)
                                {
                                    line += "PLASTIC TRASH";
                                    comment = "May take more than 500 years to decompose.";
                                }
                                else
                                {
                                    line += "RADIOACTIVE TRASH";
                                    comment = "An invisible speck can cause cancer.";
                                }
                                spriteBatch.DrawString(statsFont, line, new Vector2(game.Window.ClientBounds.Width / 2 - statsFont.MeasureString(line).X / 2, 4), Color.Yellow);
                                comment = wrapLine(comment, commentMaxLength, statsFont);
                                spriteBatch.DrawString(statsFont, comment, new Vector2(game.Window.ClientBounds.Width / 2 - statsFont.MeasureString(comment).X / 2, statsFont.MeasureString(line).Y + 2), Color.Red);
                            }
                            else
                            {
                                ShipWreck shipPointedAt = CursorManager.MouseOnWhichShipWreck(cursor, gameCamera, shipWrecks);
                                if (shipPointedAt != null)
                                {
                                    string line, comment;
                                    line = "";
                                    comment = "";
                                    line = "OLD SHIPWRECK";
                                    comment = "Sunk hundreds years ago.";
                                    spriteBatch.DrawString(statsFont, line, new Vector2(game.Window.ClientBounds.Width / 2 - statsFont.MeasureString(line).X / 2, 4), Color.Yellow);
                                    comment = wrapLine(comment, commentMaxLength, statsFont);
                                    spriteBatch.DrawString(statsFont, comment, new Vector2(game.Window.ClientBounds.Width / 2 - statsFont.MeasureString(comment).X / 2, statsFont.MeasureString(line).Y + 2), Color.Red);
                                }
                                else
                                {
                                    Factory factoryPointedAt = CursorManager.MouseOnWhichFactory(cursor, gameCamera, factories);
                                    if (factoryPointedAt != null)
                                    {
                                        string line, comment;
                                        line = "";
                                        comment = "";
                                        if (factoryPointedAt.factoryType == FactoryType.biodegradable)
                                        {
                                            line += "BIODEGRADABLE TRASH PROCESSING PLANT";
                                            comment = "Organic trashes can be dropped here for processing.";
                                        }
                                        else if (factoryPointedAt.factoryType == FactoryType.plastic)
                                        {
                                            line += "PLASTIC TRASH PROCESSING PLANT";
                                            comment = "Plastic trashes can be dropped here for processing.";
                                        }
                                        else
                                        {
                                            line += "RADIOACTIVE TRASH PROCESSING PLANT";
                                            comment = "Radioactive trashes can be dropped here for processing.";
                                        }
                                        spriteBatch.DrawString(statsFont, line, new Vector2(game.Window.ClientBounds.Width / 2 - statsFont.MeasureString(line).X / 2, 4), Color.Yellow);
                                        comment = wrapLine(comment, commentMaxLength, statsFont);
                                        spriteBatch.DrawString(statsFont, comment, new Vector2(game.Window.ClientBounds.Width / 2 - statsFont.MeasureString(comment).X / 2, statsFont.MeasureString(line).Y + 2), Color.Red);
                                    }
                                    else
                                    {
                                        if (CursorManager.MouseOnResearchFacility(cursor, gameCamera, researchFacility))
                                        {
                                            string line, comment;
                                            line = "RESEARCH FACILITY";
                                            comment = "Researches on upgrading factories and Hydrobot, analysing abnormal objects and resurrecting extinct animals from DNA.";
                                            spriteBatch.DrawString(statsFont, line, new Vector2(game.Window.ClientBounds.Width / 2 - statsFont.MeasureString(line).X / 2, 4), Color.Yellow);
                                            comment = wrapLine(comment, commentMaxLength, statsFont);
                                            spriteBatch.DrawString(statsFont, comment, new Vector2(game.Window.ClientBounds.Width / 2, statsFont.MeasureString(line).Y + 2), Color.Red, 0, new Vector2(statsFont.MeasureString(comment).X/2, 0), 1.0f, SpriteEffects.None, 0);

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
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
