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
        Game game;
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
            this.game = game;

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
            Vector2 textPositon = new Vector2(Game.Window.ClientBounds.Center.X - (font.MeasureString(text).X / 2) * GameConstants.generalTextScaleFactor, Game.Window.ClientBounds.Center.Y + 200 * GameConstants.generalTextScaleFactor);

            spriteBatch.Begin();
            base.Draw(gameTime);

            int logoWidth = (int)(256 * GameConstants.generalTextScaleFactor); //300
            int logoHeight = (int)(256 * GameConstants.generalTextScaleFactor); //300
            Rectangle teamLogoRectangle = new Rectangle(this.game.Window.ClientBounds.Right - (logoWidth - (logoWidth / 10)), this.game.Window.ClientBounds.Bottom - (logoHeight - (logoHeight / 10)), logoWidth, logoHeight);
            spriteBatch.Draw(teamLogo, teamLogoRectangle, Color.White);
            spriteBatch.DrawString(font, text, textPositon, Color.Black, 0, Vector2.Zero, GameConstants.generalTextScaleFactor, SpriteEffects.None, 0);
            spriteBatch.End();
            //this.loadingLevel = PlayGameScene.currentLevel;
            loadingSceneStarted = true;
        }

    }
}