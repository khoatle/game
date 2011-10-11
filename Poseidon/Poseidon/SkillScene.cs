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
        private Texture2D ExperienceBar;
        Game game;
        SpriteFont statsFont;
        private int accumulated_experience_points;
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
            Components.Add(cursor);

            ExperienceBar = Content.Load<Texture2D>("Image/ExperienceBar");
            statsFont = Content.Load<SpriteFont>("Fonts/StatsFont");
            this.game = game;            
        }


        /// <summary>
        /// Show the start scene
        /// </summary>
        public override void Show()
        {
            audio.NewMeteor.Play();
            accumulated_experience_points = Tank.currentExperiencePts;
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
            base.Update(gameTime);
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            AddingObjects.DrawExperienceBar(ExperienceBar, game, spriteBatch, statsFont, Tank.currentExperiencePts, (game.Window.ClientBounds.Height/2)-50, "EXPERIENCE", Color.Yellow);
        }
    }
}