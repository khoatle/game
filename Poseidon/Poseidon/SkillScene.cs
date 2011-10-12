﻿#region Using Statements

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

            UnassignedPtsBar = Content.Load<Texture2D>("Image/UnassignedPtsBar");
            statsFont = Content.Load<SpriteFont>("Fonts/StatsFont");
            menuLarge = Content.Load<SpriteFont>("Fonts/menuLarge");
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
            base.Update(gameTime);
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            AddingObjects.DrawUnassignedPtsBar(UnassignedPtsBar, game, spriteBatch, statsFont, Tank.unassignedPts, (game.Window.ClientBounds.Height/2)-50, "UNASSIGNED POINTS", Color.DarkBlue);
            spriteBatch.DrawString(menuLarge, Tank.speed.ToString("F1"), new Vector2(210, 225), Color.Black);
            spriteBatch.DrawString(menuLarge, Tank.maxHitPoint.ToString(), new Vector2(game.Window.ClientBounds.Width-295, 180), Color.Black);
            spriteBatch.DrawString(menuLarge, Tank.shootingRate.ToString("F1"), new Vector2(210, game.Window.ClientBounds.Height-320), Color.Black);
            spriteBatch.DrawString(menuLarge, Tank.strength.ToString("F1"), new Vector2(game.Window.ClientBounds.Width - 270, game.Window.ClientBounds.Height-260), Color.Black);
        }
    }
}


