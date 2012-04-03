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
        public bool loadingSurvivalScene = false;
        //public int loadingLevel;

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
            string text;
            if (PoseidonGame.gamePlus)
                text = "LOADING GAME +";
            else
                text = "LOADING";
            Vector2 textPositon = new Vector2(Game.Window.ClientBounds.Center.X - (font.MeasureString(text).X / 2), Game.Window.ClientBounds.Center.Y + 200);

            spriteBatch.Begin();
            base.Draw(gameTime);

            Rectangle logoRect = new Rectangle(Game.Window.ClientBounds.Right - teamLogo.Width, Game.Window.ClientBounds.Bottom - teamLogo.Height, teamLogo.Width, teamLogo.Height);
            spriteBatch.Draw(teamLogo, logoRect , Color.White);
            spriteBatch.DrawString(font, text, textPositon, Color.Black);
            spriteBatch.End();
            //this.loadingLevel = PlayGameScene.currentLevel;
            loadingSceneStarted = true;
        }

    }
}