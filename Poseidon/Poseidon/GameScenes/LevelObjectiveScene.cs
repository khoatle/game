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
    public class LevelObjectiveScene : GameScene
    {
        // Audio
        protected AudioLibrary audio;
        // Spritebatch
        protected SpriteBatch spriteBatch = null;
        ContentManager Content;
        Game game;
        SpriteFont statsFont;
        SpriteFont menuLarge;
        private PlayGameScene playgamescene;
        Random random = new Random();
        /// <summary>
        /// Default Constructor
        public LevelObjectiveScene(Game game, SpriteFont smallFont, SpriteFont largeFont,
                           Texture2D background, ContentManager Content, PlayGameScene playgamescene)
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

            statsFont = Content.Load<SpriteFont>("Fonts/StatsFont");
            menuLarge = Content.Load<SpriteFont>("Fonts/menuLarge");
            this.playgamescene = playgamescene;
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
            //AddingObjects.DrawUnassignedPtsBar(UnassignedPtsBar, game, spriteBatch, statsFont, Tank.unassignedPts, (game.Window.ClientBounds.Height / 2) - 50, "UNASSIGNED POINTS", Color.DarkBlue);
            spriteBatch.DrawString(menuLarge, "LEVEL1:\nYou Need to kill all enemies or the big boss", new Vector2(100, 200), Color.Black);
            spriteBatch.DrawString(menuLarge, playgamescene.enemiesAmount.ToString()+" enemies left", new Vector2(100, 400), Color.Black);
            //spriteBatch.DrawString(menuLarge, Tank.shootingRate.ToString("F1"), new Vector2(210, game.Window.ClientBounds.Height - 320), Color.Black);
            //spriteBatch.DrawString(menuLarge, Tank.strength.ToString("F1"), new Vector2(game.Window.ClientBounds.Width - 270, game.Window.ClientBounds.Height - 260), Color.Black);
            spriteBatch.End();
        }
    }
}


