#region Using Statements

using System;
using System.IO;
using System.Collections.Generic;
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
    public class SelectLoadingLevelScene : GameScene
    {
        protected readonly Texture2D teamLogo;
        protected SpriteBatch spriteBatch = null;
        private SpriteFont font;
        protected TextMenuComponent menu;
        public List<int> savedlevels = new List<int>();

        public SelectLoadingLevelScene(Game game, SpriteFont font,
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

            // Create the Menu
            int i;
            List<string> menuStringList = new List<string>();
            for (i = 1; i < GameConstants.RoundTime.Length; i++)
            {
                string savedFileName = "GameLevel" + i.ToString();
                string menuString = "LEVEL " + (i+1).ToString(); // +1 Since the game will start from next level
                if (File.Exists(savedFileName))
                {
                    savedlevels.Add(i);
                    menuStringList.Add(menuString);
                }
            }
            menuStringList.Add("Go Back");
            menu = new TextMenuComponent(game, font, font);
            menu.SetMenuItems(menuStringList.ToArray());
            Components.Add(menu);
        }

        /// <summary>
        /// Show the start scene
        /// </summary>
        public override void Show()
        {
            menu.Position = new Vector2(Game.Window.ClientBounds.Width / 2
                                          , 50);
            menu.Visible = true;
            menu.Enabled = true;
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
        /// Gets the selected menu option
        /// </summary>
        public int SelectedMenuIndex
        {
            get { return menu.SelectedIndex; }
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
            spriteBatch.Begin();
            base.Draw(gameTime);
            if (savedlevels.Count == 0)
            {
                string text = "No Saved Games";
                text = AddingObjects.wrapLine(text, Game.Window.ClientBounds.Width, font);
                Vector2 textPosition = new Vector2(Game.Window.ClientBounds.Center.X - (font.MeasureString(text).X / 2), Game.Window.ClientBounds.Center.Y);
                spriteBatch.DrawString(font, text, textPosition, Color.Black);
            }
            spriteBatch.End();
        }

    }
}