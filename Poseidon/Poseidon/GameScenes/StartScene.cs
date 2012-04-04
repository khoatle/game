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

        protected Rectangle titleLine1SrcRect;
        protected Rectangle titleLine1DestRect;
        protected Rectangle titleLine2SrcRect;
        protected Rectangle titleLine2DestRect;

        protected float widthScale, heightScale;

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

            widthScale = (float)game.Window.ClientBounds.Width / 1440;
            heightScale = (float)game.Window.ClientBounds.Height / 900;

            //textScale = widthScale * heightScale;
            //if (textScale > 1) textScale = 1;

            GameConstants.generalTextScaleFactor = (float)Math.Sqrt((double)widthScale * (double)heightScale);

            if (GameConstants.generalTextScaleFactor > 1)
                GameConstants.generalTextScaleFactor = 1;

            titleLine1SrcRect = new Rectangle(0, 0, 588, 126);//Hydrobot (0,0, 588, 126)
            titleLine2SrcRect = new Rectangle(90, 169, 620, 126); //Adventure (90, 169, 620, 126)
            
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
            resetMenuStartPosition();

            menu.Position = new Vector2((game.Window.ClientBounds.Width / 2) , titleLine2DestRect.Bottom);

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

        private void resetMenuStartPosition()
        {
            int width1 = (int)(titleLine1SrcRect.Width * widthScale);
            int height1 = (int)(titleLine1SrcRect.Height * heightScale);
            int width2 = (int)(titleLine2SrcRect.Width * widthScale);
            int height2 = (int)(titleLine2SrcRect.Height * heightScale);
            int line1posX = (-1 * width1);
            int line2posX = game.Window.ClientBounds.Width;
            int line1posY = height1 / 3;
            int line2posY = (int)(height1 * 1.5);
            titleLine1DestRect = new Rectangle(line1posX, line1posY, width1, height1);
            titleLine2DestRect = new Rectangle(line2posX, line2posY, width2, height2);
        }

        /// <summary>
        /// Show the start scene
        /// </summary>
        public override void Show()
        {
            
            titleLine1DestRect.X = (-1 * titleLine1DestRect.Width);
            titleLine2DestRect.X = game.Window.ClientBounds.Width;

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
                menu.Position = new Vector2((game.Window.ClientBounds.Width / 2), titleLine2DestRect.Bottom);
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
                if (titleLine2DestRect.X >= (game.Window.ClientBounds.Center.X - titleLine2DestRect.Width / 2))
                {
                    titleLine2DestRect.X -= (int)(15*widthScale);
                }

                if (titleLine1DestRect.X <= (game.Window.ClientBounds.Center.X - titleLine1DestRect.Width / 2))
                {
                    titleLine1DestRect.X += (int)(15*widthScale);
                }
                else
                {
                    menu.Visible = true;
                    menu.Enabled = true;
                }
            }
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

            int logoWidth = (int)(300 * widthScale); //300
            int logoHeight = (int)(300 * heightScale); //300
            Rectangle teamLogoRectangle = new Rectangle(game.Window.ClientBounds.Right - (logoWidth-(logoWidth/10)), game.Window.ClientBounds.Bottom - (logoHeight-(logoHeight/10)), logoWidth, logoHeight);
            spriteBatch.Draw(teamLogo, teamLogoRectangle, Color.White);
            //System.Diagnostics.Debug.WriteLine(titleLine1Position + "Rect" + titleLine1Rect);
            spriteBatch.Draw(elements, titleLine1DestRect, titleLine1SrcRect, Color.White);
            spriteBatch.Draw(elements, titleLine2DestRect, titleLine2SrcRect, Color.White);
            
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