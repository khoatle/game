#region Using Statements

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Poseidon.Core;

#endregion

namespace Poseidon
{
    /// <summary>
    /// This is a game component that implements the Game Start Scene.
    /// </summary>
    public class LoadingScene : GameScene
    {
        protected readonly Texture2D teamLogo;
        protected SpriteBatch spriteBatch = null;
        private SpriteFont font;
        public bool loadingSceneStarted = false;
        public int loadingLevel;

        public LoadingScene(Game game, SpriteFont font,
                            Texture2D background, Texture2D teamLogo)
            : base(game)
        {
            this.teamLogo = teamLogo;
            this.font = font;
            Components.Add(new ImageComponent(game, background,
                                            ImageComponent.DrawMode.Stretch));

            // Get the current spritebatch
            spriteBatch = (SpriteBatch)Game.Services.GetService(
                                            typeof(SpriteBatch));

        }

        /// <summary>
        /// Show the start scene
        /// </summary>
        public override void Show()
        {
            base.Show();
        }

        /// <summary>
        /// Hide the start scene
        /// </summary>
        public override void Hide()
        {
            base.Hide();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            Rectangle logoRect = new Rectangle(Game.Window.ClientBounds.Center.X - teamLogo.Width/2, Game.Window.ClientBounds.Center.Y - teamLogo.Height, teamLogo.Width, teamLogo.Height);
            string text = "LOADING...";
            Vector2 textPositon = new Vector2(logoRect.Center.X - (font.MeasureString("LOADING").X/2), logoRect.Bottom+100);

            spriteBatch.Begin();
            base.Draw(gameTime);

            spriteBatch.Draw(teamLogo, logoRect , Color.White);
            spriteBatch.DrawString(font, text, textPositon, Color.Black);
            spriteBatch.End();
            this.loadingLevel = PlayGameScene.currentLevel;
            loadingSceneStarted = true;
        }

    }
}