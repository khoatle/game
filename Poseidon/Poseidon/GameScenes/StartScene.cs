#region Using Statements

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Poseidon.Core;
using System.IO;
using System.Collections.Generic;

#endregion

namespace Poseidon
{
    /// <summary>
    /// This is a game component that implements the Game Start Scene.
    /// </summary>
    public class StartScene : GameScene
    {
        // Misc
        protected TextMenuComponent menu;
        protected readonly Texture2D elements;
        protected readonly Texture2D teamLogo;
        // Audio
        protected AudioLibrary audio;
        // Spritebatch
        protected SpriteBatch spriteBatch = null;

        Game game;

        protected Rectangle titleLine1Rect;
        protected Vector2 titleLine1Position;
        protected Rectangle titleLine2Rect;
        protected Vector2 titleLine2Position;

        GraphicsDevice graphicsDevice;

        public bool gameStarted = false;
        
        //for continously playing random background musics
        Random rand= new Random();
        public string[] menuItems;

        public StartScene(Game game, SpriteFont smallFont, SpriteFont largeFont,
                            Texture2D background, Texture2D elements, Texture2D teamLogo, GraphicsDevice graphicDevice)
            : base(game)
        {
            this.elements = elements;
            this.teamLogo = teamLogo;
            this.game = game;
            this.graphicsDevice = graphicDevice;

            titleLine1Rect = new Rectangle(0, 0, 588, 126);//Hydrobot (0,0, 588, 126)
            titleLine2Rect = new Rectangle(90, 169, 620, 126); //Adventure (90, 169, 620, 126)
            
            Components.Add(new ImageComponent(game, background,
                                            ImageComponent.DrawMode.Stretch));

            
            // Create the Menu
            if (File.Exists("SurvivalMode"))
            {
                string[] items = { "New Game", "New Game Plus", "Load Saved Level", "Survival Mode", "Config", "Help", "Credits", "Quit" };
                menuItems = items;
            }
            else
            {
                string[] items = { "New Game", "Load Saved Level", "Config", "Help", "Credits", "Quit" };
                menuItems = items;
            }
           
            menu = new TextMenuComponent(game, smallFont, largeFont);

            //starting values
            titleLine1Position.X = -1 * titleLine1Rect.Width;
            titleLine1Position.Y = titleLine1Rect.Height / 3;
            titleLine2Position.X = game.Window.ClientBounds.Width;
            titleLine2Position.Y = (int)(titleLine1Rect.Height * 1.5);

            menu.Position = new Vector2((game.Window.ClientBounds.Width / 2) , (titleLine2Position.Y + titleLine2Rect.Height));

            menu.SetMenuItems(menuItems);
            Components.Add(menu);

            // Get the current spritebatch
            spriteBatch = (SpriteBatch)Game.Services.GetService(
                                            typeof(SpriteBatch));

            // Get the audio library
            audio = (AudioLibrary)
                Game.Services.GetService(typeof(AudioLibrary));

            cursor = new Cursor(game, spriteBatch);
        }

        /// <summary>
        /// Show the start scene
        /// </summary>
        public override void Show()
        {
            titleLine1Position.X = -1 * titleLine1Rect.Width;
            titleLine2Position.X = game.Window.ClientBounds.Width;

            audio.NewMeteor.Play();

            cursor.SetMenuCursorImage();

            
            if (gameStarted)
            {
                // Create the Menu
                if (PlayGameScene.currentGameState != GameState.GameComplete)
                {
                    if (File.Exists("SurvivalMode"))
                    {
                        string[] items = { "Resume Game", "New Game", "New Game Plus", "Load Saved Level", "Survival Mode", "Config", "Help", "Credits", "Quit" };
                        menuItems = items;
                    }
                    else
                    {
                        string[] items = { "Resume Game", "New Game", "Load Saved Level", "Config", "Help", "Credits", "Quit" };
                        menuItems = items;
                    }
                }
                else {
                    if (File.Exists("SurvivalMode"))
                    {
                        string[] items = { "New Game", "New Game Plus", "Load Saved Level", "Survival Mode", "Config", "Help", "Credits", "Quit" };
                        menuItems = items;
                    }
                    else
                    {
                        string[] items = { "New Game", "Load Saved Level", "Config", "Help", "Credits", "Quit" };
                        menuItems = items;
                    }
                }
                menu.Position = new Vector2(game.Window.ClientBounds.Width / 2
                                          , titleLine2Rect.Bottom + 5);
                menu.SetMenuItems(menuItems);
            }
            

            // These elements will be visible when the 'Rock Rain' title
            // is done.
            menu.Visible = false;
            menu.Enabled = false;
            //showEnhanced = false;

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
            if (MediaPlayer.State.Equals(MediaState.Stopped))
            {
                
                MediaPlayer.Play(audio.mainMenuMusic);
            }
            if (!menu.Visible)
            {
                if (titleLine2Position.X >= (game.Window.ClientBounds.Center.X - titleLine2Rect.Width/2))
                {
                    titleLine2Position.X -= (game.Window.ClientBounds.Width*0.0117f);
                }

                if (titleLine1Position.X <= (game.Window.ClientBounds.Center.X - titleLine1Rect.Width/2))
                {
                    titleLine1Position.X += (game.Window.ClientBounds.Width * 0.0117f);
                }
                else
                {
                    menu.Visible = true;
                    menu.Enabled = true;
                    //Random rand = new Random();
                    //MediaPlayer.Play(audio.backgroundMusics[rand.Next(GameConstants.NumNormalBackgroundMusics)]);
//#if XBOX360
//                    enhancedPosition = new Vector2((rainPosition.X + 
//                    rainRect.Width - enhancedRect.Width / 2), rainPosition.Y);
//#else
//                    enhancedPosition =
//                        new Vector2((rainPosition.X + rainRect.Width -
//                        enhancedRect.Width / 2) - 80, rainPosition.Y);
//#endif
//                    showEnhanced = true;
                }
            }
            //else
            //{
            //    elapsedTime += gameTime.ElapsedGameTime;

            //    if (elapsedTime > TimeSpan.FromSeconds(1))
            //    {
            //        elapsedTime -= TimeSpan.FromSeconds(1);
            //        showEnhanced = !showEnhanced;
            //    }
            //}
            cursor.Update(graphicsDevice, PlayGameScene.gameCamera, gameTime, null);
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

            int logoWidth = (int)(game.Window.ClientBounds.Width * 0.234); //300
            int logoHeight = (int)(game.Window.ClientBounds.Height * 0.375); //300
            Rectangle teamLogoRectangle = new Rectangle(game.Window.ClientBounds.Right - (logoWidth-(logoWidth/10)), game.Window.ClientBounds.Bottom - (logoHeight-(logoHeight/10)), logoWidth, logoHeight);
            spriteBatch.Draw(teamLogo, teamLogoRectangle, Color.White);
            //System.Diagnostics.Debug.WriteLine(titleLine1Position + "Rect" + titleLine1Rect);
            spriteBatch.Draw(elements, titleLine1Position, titleLine1Rect, Color.White);
            spriteBatch.Draw(elements, titleLine2Position, titleLine2Rect, Color.White);
            
            //if (showEnhanced)
            //{
            //    spriteBatch.Draw(elements, enhancedPosition, enhancedRect,
            //                     Color.White);
            //}

            cursor.Draw(gameTime);
            spriteBatch.End();
        }
    }
}