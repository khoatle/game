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
    public class TipScene : GameScene
    {
        // Audio
        protected AudioLibrary audio;
        // Spritebatch
        protected SpriteBatch spriteBatch = null;
        ContentManager Content;
        Game game;
        SpriteFont tipFont;
        Texture2D tipBox;
        Rectangle tipBoxRect;
        Vector2 tipStringPosition;
        Random random = new Random();
        /// <summary>
        /// Default Constructor
        public TipScene(Game game, Texture2D background, ContentManager Content)
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
            //cursor = new Cursor(game, spriteBatch);
            //Components.Add(cursor);

            tipFont = IngamePresentation.statisticFont;//Content.Load<SpriteFont>("Fonts/tip");
            tipBox = Content.Load<Texture2D>("Image/Miscellaneous/tipBox");
            
            this.game = game;
        }


        /// <summary>
        /// Show the start scene
        /// </summary>
        public override void Show()
        {
            int tipBoxWidth = (int)(GraphicsDevice.Viewport.TitleSafeArea.Width * 0.78);
            int tipBoxHeight = (int)(GraphicsDevice.Viewport.TitleSafeArea.Height * 0.75);
            tipBoxRect = new Rectangle(GraphicsDevice.Viewport.TitleSafeArea.Center.X - tipBoxWidth / 2, game.Window.ClientBounds.Center.Y - tipBoxHeight / 2, tipBoxWidth, tipBoxHeight);

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
            if (PlayGameScene.currentLevel == 3 || PlayGameScene.currentLevel == 11)
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
            base.Update(gameTime);
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            int currentLevel = PlayGameScene.currentLevel;
            string title = "tip";
            string text = "";
            spriteBatch.Begin();
            base.Draw(gameTime);

            spriteBatch.Draw(tipBox, tipBoxRect, Color.White);

            foreach (TipItem tipItem in PoseidonGame.liveTipManager.allTips[currentLevel])
            {
                text += tipItem.tipItemStr + "\n";
            }

            spriteBatch.DrawString(tipFont, title, new Vector2(game.Window.ClientBounds.Center.X - tipFont.MeasureString(title).X, 2), Color.Red, 0, new Vector2(0, 0), 2f, SpriteEffects.None, 0);

            text = IngamePresentation.wrapLine(text, tipBoxRect.Width - 100, tipFont);

            tipStringPosition = new Vector2(tipBoxRect.Center.X - tipFont.MeasureString(text).X / 2, tipBoxRect.Center.Y - tipFont.MeasureString(text).Y/2);

            spriteBatch.DrawString(tipFont, text, tipStringPosition, Color.Black);

            string nextText = "Press Enter/Esc to continue";
            Vector2 nextTextPosition = new Vector2(game.Window.ClientBounds.Right - tipFont.MeasureString(nextText).X, game.Window.ClientBounds.Bottom - tipFont.MeasureString(nextText).Y);
            spriteBatch.DrawString(tipFont, nextText, nextTextPosition, Color.Black);

            spriteBatch.End();
        }
    }
}


