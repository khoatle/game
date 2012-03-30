#region Using Statements

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using Poseidon.Core;
using System.IO;
using System.Collections.Generic;

#endregion

namespace Poseidon
{
    /// <summary>
    /// This is a game component that implements the Config Scene.
    /// </summary>
    public class ConfigScene : GameScene
    {
        // Textures and their rectangles
        protected readonly Texture2D configTitle, slideBar, uncheckedBox, checkedBox, okButton;

        protected Rectangle titleRect;
        protected List<Rectangle> itemRectList;
        protected List<Rectangle> iconRectList;
        protected Rectangle musicVolumeRect;
        protected Rectangle soundVolumeRect;
        protected Rectangle specialEffectRect;
        protected Rectangle numParticleRect;
        protected Rectangle showLiveTipRect;
        protected Rectangle schoolFishRect;
        protected Rectangle okBox;
        private string[] menuItems;

        //Fonts
        SpriteFont regularFont;
        SpriteFont selectedFont;

        //Check mouse position and click
        int selectedIndex = -1;
        bool mouseOnOK = false;

        MouseState currentMouseState = new MouseState();
        MouseState lastMouseState = new MouseState();
        private bool clicked = false;
        public static bool okClicked = false;


        //Color
        protected Color regularColor = Color.Khaki, selectedColor = Color.FloralWhite;

        // Audio
        protected AudioLibrary audio;
        // Spritebatch
        protected SpriteBatch spriteBatch = null;

        Game game;

        GraphicsDevice graphicsDevice;

        public bool gameStarted = false;

        //for continously playing random background musics
        Random rand = new Random();


        public ConfigScene(Game game, SpriteFont smallFont, SpriteFont largeFont,
                            Texture2D background, Texture2D configTitle, Texture2D unselectedCheckbox, Texture2D selectedCheckBox, Texture2D okButton,  GraphicsDevice graphicDevice)
            : base(game)
        {
            this.game = game;
            this.graphicsDevice = graphicDevice;
            regularFont = smallFont;
            selectedFont = largeFont;
            uncheckedBox = unselectedCheckbox;
            checkedBox = selectedCheckBox;
            this.okButton = okButton;

            Components.Add(new ImageComponent(game, background,
                                            ImageComponent.DrawMode.Stretch));

            this.configTitle = configTitle;
            int titleWidth = (int)(game.Window.ClientBounds.Width * 0.32); //409 if width=1280
            int titleHeight = (int)(game.Window.ClientBounds.Height * 0.125); //100 if height=800
            titleRect = new Rectangle(game.Window.ClientBounds.Center.X - titleWidth / 2, game.Window.ClientBounds.Top + titleHeight/3, titleWidth, titleHeight);

            string[] items = { "Music Volume", "Sound Volume", "Show Live Tips", "Special Effect", "Particle Level", "Fish School Size" };
            menuItems = items;
            itemRectList = new List<Rectangle>(menuItems.Length);
            iconRectList = new List<Rectangle>(menuItems.Length);

            int x, y, width, height;
            x = game.Window.ClientBounds.Center.X - (int)( findMaxWidth() * 0.6);
            y = titleRect.Bottom+titleHeight/4;
            height = (int)(regularFont.MeasureString(menuItems[0]).Y);
            foreach (string menu in menuItems)
            {
                width = (int)(regularFont.MeasureString(menu).X);
                Rectangle itemRectangle = new Rectangle(x, y, width, height);
                itemRectList.Add(itemRectangle);

                y += (int)(regularFont.LineSpacing*1.2);
            }

            iconRectList.Add(new Rectangle());
            iconRectList.Add(new Rectangle());
            showLiveTipRect = new Rectangle(game.Window.ClientBounds.Center.X+titleWidth/2, itemRectList[2].Top, height, height);
            iconRectList.Add(showLiveTipRect);
            specialEffectRect = new Rectangle(game.Window.ClientBounds.Center.X + titleWidth / 2, itemRectList[3].Top, height, height);
            iconRectList.Add(specialEffectRect);

            width = titleWidth/2;
            height = (int)(titleHeight*0.75);
            okBox = new Rectangle(game.Window.ClientBounds.Center.X - width / 2, game.Window.ClientBounds.Bottom - (int)(height * 1.5), width, height);

            // Get the current spritebatch
            spriteBatch = (SpriteBatch)Game.Services.GetService(
                                            typeof(SpriteBatch));

            // Get the audio library
            audio = (AudioLibrary)
                Game.Services.GetService(typeof(AudioLibrary));
            
            cursor = new Cursor(game, spriteBatch);

            selectedIndex = -1;
        }

        private int findMaxWidth()
        {
            int maxWidth = 0;
            int width;
            foreach (string menu in menuItems)
            {
                width = (int)(regularFont.MeasureString(menu).X);
                if (width > maxWidth)
                {
                    maxWidth = width;
                }
            }
            return maxWidth;
        }
        /// <summary>
        /// Show the start scene
        /// </summary>
        public override void Show()
        {
            
            audio.NewMeteor.Play();

            cursor.SetMenuCursorImage();
                        
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
        //public int SelectedMenuIndex
        //{
            
        //}

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            int prevselectedIndex = selectedIndex;
            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            for (int i = 0; i < iconRectList.Count; i++)
            {
                if (iconRectList[i].Intersects(new Rectangle(currentMouseState.X, currentMouseState.Y, 10, 10)))
                {
                    selectedIndex = i;
                    break;
                }
                selectedIndex = -1;
            }

            if (okBox.Intersects(new Rectangle(currentMouseState.X, currentMouseState.Y, 10, 10)))
            {
                selectedIndex = -2; 
            }

            if (prevselectedIndex != selectedIndex && selectedIndex!= -1)
            {
                audio.MenuScroll.Play();
            }

            if (selectedIndex >= 0)
            {   
                if (iconRectList[selectedIndex].Intersects(new Rectangle(currentMouseState.X, currentMouseState.Y, 10, 10)) && lastMouseState.LeftButton.Equals(ButtonState.Pressed) && currentMouseState.LeftButton.Equals(ButtonState.Released))
                {
                    clicked = true;
                }
            }

            if (clicked)
            {
                switch (selectedIndex)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        if (GameSettings.ShowLiveTip)
                            GameSettings.ShowLiveTip = false;
                        else
                            GameSettings.ShowLiveTip = true;
                        break;
                    case 3:
                        if (GameSettings.SpecialEffectsEnabled)
                            GameSettings.SpecialEffectsEnabled = false;
                        else
                            GameSettings.SpecialEffectsEnabled = true;
                        break;
                    case 4:
                        break;
                    case 5:
                        break;
                    default:
                        break;
                }
                clicked = false;
            }

            if (okBox.Intersects(new Rectangle(currentMouseState.X, currentMouseState.Y, 10, 10)) && lastMouseState.LeftButton.Equals(ButtonState.Pressed) && currentMouseState.LeftButton.Equals(ButtonState.Released))
            {
                okClicked = true;
            }

            if (MediaPlayer.State.Equals(MediaState.Stopped))
            {

                MediaPlayer.Play(audio.mainMenuMusic);
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
            
            //Draw Title
            spriteBatch.Draw(configTitle, titleRect, Color.White);
            
            //Draw the item text
            for(int i=0; i<menuItems.Length; i++ )
            {
                if (i == selectedIndex)
                {
                    spriteBatch.DrawString(selectedFont, menuItems[i], new Vector2(itemRectList[i].Left + 1, itemRectList[i].Top + 1), selectedColor); //shadow
                    spriteBatch.DrawString(selectedFont, menuItems[i], new Vector2(itemRectList[i].Left, itemRectList[i].Top), selectedColor);
                }
                else
                {
                    spriteBatch.DrawString(regularFont, menuItems[i], new Vector2(itemRectList[i].Left + 1, itemRectList[i].Top + 1), regularColor); //shadow
                    spriteBatch.DrawString(regularFont, menuItems[i], new Vector2(itemRectList[i].Left, itemRectList[i].Top), regularColor);
                }
            }
            
            //Draw the checkbox and bars
            if(GameSettings.ShowLiveTip)
                spriteBatch.Draw(checkedBox, showLiveTipRect, Color.White);
            else
                spriteBatch.Draw(uncheckedBox, showLiveTipRect, Color.White);
            if (GameSettings.SpecialEffectsEnabled)
                spriteBatch.Draw(checkedBox, specialEffectRect, Color.White);
            else
                spriteBatch.Draw(uncheckedBox, specialEffectRect, Color.White);
 
            //draw OK button
            if(selectedIndex==-2) //mouse on Ok
                spriteBatch.Draw(okButton, okBox, Color.Orange);
            else
                spriteBatch.Draw(okButton, okBox, Color.White);

            cursor.Draw(gameTime);
            spriteBatch.End();
        }
    }
}