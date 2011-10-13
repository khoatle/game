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
    public class SkillScene : GameScene
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
        /// <summary>
        /// Default Constructor
         public SkillScene(Game game, SpriteFont smallFont, SpriteFont largeFont,
                            Texture2D background, ContentManager Content)
            : base(game)
        {
            this.Content = Content;
            Components.Add(new ImageComponent(game, background,
                                            ImageComponent.DrawMode.Stretch));

            // Get the current spritebatch
            spriteBatch = (SpriteBatch)Game.Services.GetService(
                                            typeof(SpriteBatch));

            // Get the audio library
            audio = (AudioLibrary)
                Game.Services.GetService(typeof(AudioLibrary));

            // for the mouse or touch
            cursor = new Cursor(game, spriteBatch);
            //Components.Add(cursor);

            UnassignedPtsBar = Content.Load<Texture2D>("Image/UnassignedPtsBar");
            statsFont = Content.Load<SpriteFont>("Fonts/StatsFont");
            menuLarge = Content.Load<SpriteFont>("Fonts/menuLarge");
            speedTexture = Content.Load<Texture2D>("Image/speed");
            hitpointTexture = Content.Load<Texture2D>("Image/hit_point");
            shootrateTexture = Content.Load<Texture2D>("Image/shooting_rate");
            bulletStrengthTexture = Content.Load<Texture2D>("Image/strength");
            doneTexture = Content.Load<Texture2D>("Image/done");
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
            MediaPlayer.Stop();
            base.Hide();
        }



      /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            cursor.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            DrawSpeedIcon();
            DrawHitPointIcon();
            DrawShootRateIcon();
            DrawBulletStrengthIcon();
            DrawDoneIcon();
            DrawUnassignedPtsBar(Tank.unassignedPts, (game.Window.ClientBounds.Top)+410, "UNASSIGNED POINTS", Color.DarkBlue);
            int stringHeight = 60;
            int stringLength = 30;
            spriteBatch.DrawString(menuLarge, Tank.speed.ToString("F1"), new Vector2(speedIconRectangle.Center.X-stringLength,speedIconRectangle.Bottom-stringHeight), Color.Black);
            spriteBatch.DrawString(menuLarge, Tank.currentHitPoint.ToString()+"/"+Tank.maxHitPoint.ToString(), new Vector2(hitpointIconRectangle.Center.X - stringLength*3, hitpointIconRectangle.Bottom - stringHeight), Color.Black);
            spriteBatch.DrawString(menuLarge, Tank.shootingRate.ToString("F1"), new Vector2(shootrateIconRectangle.Center.X - stringLength, shootrateIconRectangle.Bottom - stringHeight), Color.Black);
            spriteBatch.DrawString(menuLarge, Tank.strength.ToString("F1"), new Vector2(bulletStrengthIconRectangle.Center.X - stringLength, bulletStrengthIconRectangle.Bottom - stringHeight), Color.Black);
            cursor.Draw(gameTime);
        }

        private void DrawSpeedIcon()
        {
            int xOffsetText, yOffsetText;

            xOffsetText = game.Window.ClientBounds.Left + 100;
            yOffsetText = game.Window.ClientBounds.Top + 200;

            speedIconRectangle = new Rectangle(xOffsetText, yOffsetText, 270, 200);

            spriteBatch.Draw(speedTexture, speedIconRectangle, Color.White);
        }

        private void DrawHitPointIcon()
        {
            int xOffsetText, yOffsetText;

            xOffsetText = game.Window.ClientBounds.Right - 370;
            yOffsetText = game.Window.ClientBounds.Top + 200;

            hitpointIconRectangle = new Rectangle(xOffsetText, yOffsetText, 270, 200);

            spriteBatch.Draw(hitpointTexture, hitpointIconRectangle, Color.White);
        }

        private void DrawShootRateIcon()
        {
            int xOffsetText, yOffsetText;

            xOffsetText = game.Window.ClientBounds.Left + 100;
            yOffsetText = game.Window.ClientBounds.Bottom - 300;

            shootrateIconRectangle = new Rectangle(xOffsetText, yOffsetText, 270, 200);

            spriteBatch.Draw(shootrateTexture, shootrateIconRectangle, Color.White);
        }

        private void DrawBulletStrengthIcon()
        {
            int xOffsetText, yOffsetText;

            xOffsetText = game.Window.ClientBounds.Right - 370;
            yOffsetText = game.Window.ClientBounds.Bottom - 300;

            bulletStrengthIconRectangle = new Rectangle(xOffsetText, yOffsetText, 270, 200);

            spriteBatch.Draw(bulletStrengthTexture, bulletStrengthIconRectangle, Color.White);
        }

        private void DrawDoneIcon()
        {
            int xOffsetText, yOffsetText;

            xOffsetText = game.Window.ClientBounds.Center.X - 100;
            yOffsetText = game.Window.ClientBounds.Bottom - 100;

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
            //System.Diagnostics.Debug.WriteLine(currentUnassignedPts+","+maxUnassignedPts);
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


