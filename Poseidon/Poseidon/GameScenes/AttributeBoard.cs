#region Using Statements

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Poseidon.Core;
using Microsoft.Xna.Framework.Content;

#endregion

namespace Poseidon
{
    /// <summary>
    /// This is a game component that implements the Game Start Scene.
    /// </summary>
    public class AttributeBoard : GameScene
    {
        // Misc
        //protected SkillMenuComponent menu;
        // Audio
        protected AudioLibrary audio;
        // Spritebatch
        protected SpriteBatch spriteBatch = null;
        ContentManager Content;
        private Texture2D UnassignedPtsBar;
        Game game;
        SpriteFont statsFont;
        SpriteFont menuLarge;
        private Texture2D speedTexture;
        private Texture2D hitpointTexture;
        private Texture2D shootrateTexture;
        private Texture2D bulletStrengthTexture;
        private Texture2D doneTexture;
        public Rectangle speedIconRectangle;
        public Rectangle hitpointIconRectangle;
        public Rectangle shootrateIconRectangle;
        public Rectangle bulletStrengthIconRectangle;
        public Rectangle doneIconRectangle;
        private int centerX = PlayGameScene.GraphicDevice.Viewport.TitleSafeArea.Center.X;
        private int centerY = PlayGameScene.GraphicDevice.Viewport.TitleSafeArea.Center.Y + 50;
        Random random = new Random();
        /// <summary>
        /// Default Constructor
         public AttributeBoard(Game game, Texture2D background, ContentManager Content)
            : base(game)
        {
            this.Content = Content;
            Components.Add(new ImageComponent(game, background,
                                            ImageComponent.DrawMode.Center));

            // Get the current spritebatch
            spriteBatch = (SpriteBatch)Game.Services.GetService(
                                            typeof(SpriteBatch));

            // Get the audio library
            audio = (AudioLibrary)
                Game.Services.GetService(typeof(AudioLibrary));

            // for the mouse or touch
            cursor = new Cursor(game, spriteBatch);
            //Components.Add(cursor);

            UnassignedPtsBar = Content.Load<Texture2D>("Image/AttributeBoardTextures/UnassignedPtsBar");
            statsFont = Content.Load<SpriteFont>("Fonts/StatsFont");
            menuLarge = Content.Load<SpriteFont>("Fonts/menuLarge");
            speedTexture = Content.Load<Texture2D>("Image/AttributeBoardTextures/speed");
            hitpointTexture = Content.Load<Texture2D>("Image/AttributeBoardTextures/hit_point");
            shootrateTexture = Content.Load<Texture2D>("Image/AttributeBoardTextures/shooting_rate");
            bulletStrengthTexture = Content.Load<Texture2D>("Image/AttributeBoardTextures/strength");
            doneTexture = Content.Load<Texture2D>("Image/AttributeBoardTextures/done");
            this.game = game;            
        }


        /// <summary>
        /// Show the start scene
        /// </summary>
        public override void Show()
        {
            audio.NewMeteor.Play();
            base.Show();
        }

        /// <summary>
        /// Hide the start scene
        /// </summary>
        public override void Hide()
        {
            //MediaPlayer.Stop();
            base.Hide();
        }



      /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // play the boss fight music for certain levels
            if (PlayGameScene.currentLevel == 3 || PlayGameScene.currentLevel == 10)
            {
                if (MediaPlayer.State.Equals(MediaState.Stopped))
                {
                    MediaPlayer.Play(audio.bossMusics[random.Next(GameConstants.NumBossBackgroundMusics)]);
                }
            }
            else if (MediaPlayer.State.Equals(MediaState.Stopped))
            {
                MediaPlayer.Play(audio.backgroundMusics[random.Next(GameConstants.NumNormalBackgroundMusics)]);
            }
            cursor.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            base.Draw(gameTime);
            DrawSpeedIcon();
            DrawHitPointIcon();
            DrawShootRateIcon();
            DrawBulletStrengthIcon();
            DrawDoneIcon();
            DrawUnassignedPtsBar(HydroBot.unassignedPts, centerY-15, "UNASSIGNED POINTS", Color.DarkBlue);
            int print_speed = (int)Math.Round((HydroBot.speed - GameConstants.BasicStartSpeed) / GameConstants.gainSpeed);
            int print_hitpoint = (int)Math.Round((HydroBot.maxHitPoint - GameConstants.PlayerStartingHP) / (double)GameConstants.gainHitPoint);
            int print_shootingRate = (int)Math.Round((HydroBot.shootingRate - GameConstants.MainCharShootingSpeed) / GameConstants.gainShootingRate);
            int print_strength = (int)Math.Round((HydroBot.strength - GameConstants.MainCharStrength) / GameConstants.gainStrength);
            spriteBatch.DrawString(menuLarge, print_speed.ToString("F0"), new Vector2(speedIconRectangle.Center.X- menuLarge.MeasureString(print_speed.ToString()).X/2 ,speedIconRectangle.Center.Y), Color.Black);
            spriteBatch.DrawString(menuLarge, print_hitpoint.ToString("F0"), new Vector2(hitpointIconRectangle.Center.X - menuLarge.MeasureString(print_hitpoint.ToString()).X / 2, hitpointIconRectangle.Center.Y), Color.Black);
            spriteBatch.DrawString(menuLarge, print_shootingRate.ToString("F0"), new Vector2(shootrateIconRectangle.Center.X - menuLarge.MeasureString(print_shootingRate.ToString()).X / 2, shootrateIconRectangle.Center.Y), Color.Black);
            spriteBatch.DrawString(menuLarge, print_strength.ToString("F0"), new Vector2(bulletStrengthIconRectangle.Center.X - menuLarge.MeasureString(print_strength.ToString()).X / 2, bulletStrengthIconRectangle.Center.Y), Color.Black);
            cursor.Draw(gameTime);
            spriteBatch.End();
        }

        private void DrawSpeedIcon()
        {
            int xOffsetText, yOffsetText;

            //xOffsetText = game.Window.ClientBounds.Left + 100;
            xOffsetText = centerX - 370;
            //yOffsetText = game.Window.ClientBounds.Top + 200;
            yOffsetText = centerY - 230;

            speedIconRectangle = new Rectangle(xOffsetText, yOffsetText, 270, 200);

            spriteBatch.Draw(speedTexture, speedIconRectangle, Color.White);
        }

        private void DrawHitPointIcon()
        {
            int xOffsetText, yOffsetText;

            //xOffsetText = game.Window.ClientBounds.Right - 370;
            //yOffsetText = game.Window.ClientBounds.Top + 200;
            xOffsetText = centerX + 100;
            yOffsetText = centerY - 230;

            hitpointIconRectangle = new Rectangle(xOffsetText, yOffsetText, 270, 200);

            spriteBatch.Draw(hitpointTexture, hitpointIconRectangle, Color.White);
        }

        private void DrawShootRateIcon()
        {
            int xOffsetText, yOffsetText;

            //xOffsetText = game.Window.ClientBounds.Left + 100;
            xOffsetText = centerX - 370;
            //yOffsetText = game.Window.ClientBounds.Bottom - 300;
            yOffsetText =  centerY + 50;

            shootrateIconRectangle = new Rectangle(xOffsetText, yOffsetText, 270, 200);

            spriteBatch.Draw(shootrateTexture, shootrateIconRectangle, Color.White);
        }

        private void DrawBulletStrengthIcon()
        {
            int xOffsetText, yOffsetText;

            //xOffsetText = game.Window.ClientBounds.Right - 370;
            //yOffsetText = game.Window.ClientBounds.Bottom - 300;
            xOffsetText = centerX + 100;
            yOffsetText = centerY + 50;

            bulletStrengthIconRectangle = new Rectangle(xOffsetText, yOffsetText, 270, 200);

            spriteBatch.Draw(bulletStrengthTexture, bulletStrengthIconRectangle, Color.White);
        }

        private void DrawDoneIcon()
        {
            int xOffsetText, yOffsetText;

            //xOffsetText = game.Window.ClientBounds.Center.X - 100;
            //yOffsetText = game.Window.ClientBounds.Bottom - 100;
            xOffsetText = centerX - 100;
            yOffsetText = centerY + 250;

            doneIconRectangle = new Rectangle(xOffsetText, yOffsetText, 200, 100);

            spriteBatch.Draw(doneTexture, doneIconRectangle, Color.White);
            
        }

        public void DrawUnassignedPtsBar(int currentUnassignedPts, int heightFromTop, string type, Color typeColor)
        {
            int barX = game.Window.ClientBounds.Width / 2 - UnassignedPtsBar.Width / 2;
            int barY = heightFromTop;
            int barHeight = 44;
            double UnassignedPtsiness;
            int maxUnassignedPts = 5;
            int level;
            Color UnassignedPtsColor, BackupColor;
            level = currentUnassignedPts / maxUnassignedPts;
            currentUnassignedPts = currentUnassignedPts % maxUnassignedPts;
            if (currentUnassignedPts == 0 && level > 0)
            {
                level--;
                currentUnassignedPts = maxUnassignedPts;
            }
            if (level > 0)
                type += " + " + level.ToString();
            UnassignedPtsiness = (double)currentUnassignedPts / maxUnassignedPts;
            switch (level)
            {
                case 0:
                    UnassignedPtsColor = Color.Tan;
                    BackupColor = Color.Transparent;
                    break;
                case 1:
                    UnassignedPtsColor = Color.Khaki;
                    BackupColor = Color.Tan;
                    break;
                case 2:
                    UnassignedPtsColor = Color.Goldenrod;
                    BackupColor = Color.Khaki;
                    break;
                case 3:
                    UnassignedPtsColor = Color.DarkGoldenrod;
                    BackupColor = Color.Goldenrod;
                    break;
                case 4:
                    UnassignedPtsColor = Color.DarkOrange;
                    BackupColor = Color.DarkGoldenrod;
                    break;
                default:
                    UnassignedPtsColor = Color.DarkRed;
                    BackupColor = Color.DarkOrange;
                    break;
            }
            //Draw the negative space for the UnassignedPts bar
            spriteBatch.Draw(UnassignedPtsBar,
                new Rectangle(barX, barY, UnassignedPtsBar.Width, barHeight),
                new Rectangle(0, barHeight + 1, UnassignedPtsBar.Width, barHeight),
                BackupColor);
            //Draw the current UnassignedPts level based on the current UnassignedPts
            spriteBatch.Draw(UnassignedPtsBar,
                new Rectangle(barX, barY, (int)(UnassignedPtsBar.Width * UnassignedPtsiness), barHeight),
                new Rectangle(0, barHeight + 1, UnassignedPtsBar.Width, barHeight),
                UnassignedPtsColor);
            //Draw the box around the UnassignedPts bar
            spriteBatch.Draw(UnassignedPtsBar,
                new Rectangle(barX, barY, UnassignedPtsBar.Width, barHeight),
                new Rectangle(0, 0, UnassignedPtsBar.Width, barHeight),
                Color.White);
            spriteBatch.DrawString(statsFont, type.ToUpper(), new Vector2(game.Window.ClientBounds.Width / 2 - ((type.Length / 2) * 14), heightFromTop + 10), typeColor);
        }


    }
}


