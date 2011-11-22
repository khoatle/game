﻿#region Using Statements

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Poseidon.Core
{
    /// <summary>
    /// This is a game component that implements a menu with text elements.
    /// </summary>
    public class TextMenuComponent : DrawableGameComponent
    {
        // Spritebatch
        protected SpriteBatch spriteBatch = null;
        // Fonts
        protected readonly SpriteFont regularFont, selectedFont;
        // Colors
        protected Color regularColor = Color.Orange, selectedColor = Color.Red;
        // Menu Position
        protected Vector2 position = new Vector2();
        // Items
        protected int selectedIndex = 0;
        private readonly List<string> menuItems;
        // Used for handle input
        protected KeyboardState oldKeyboardState;
        protected GamePadState oldGamePadState;
        // Size of menu in pixels
        protected int width, height;
        // For audio effects
        protected AudioLibrary audio;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="game">the main game object</param>
        /// <param name="normalFont">Font to regular items</param>
        /// <param name="selectedFont">Font to selected item</param>
        public TextMenuComponent(Game game, SpriteFont normalFont,
            SpriteFont selectedFont)
            : base(game)
        {
            regularFont = normalFont;
            this.selectedFont = selectedFont;
            menuItems = new List<string>();

            // Get the current spritebatch
            spriteBatch = (SpriteBatch)
                Game.Services.GetService(typeof(SpriteBatch));

            // Get the audio library
            audio = (AudioLibrary)
                Game.Services.GetService(typeof(AudioLibrary));

            // Used for input handling
            oldKeyboardState = Keyboard.GetState();
            oldGamePadState = GamePad.GetState(PlayerIndex.One);
        }

        /// <summary>
        /// Set the Menu Options
        /// </summary>
        /// <param name="items"></param>
        public void SetMenuItems(string[] items)
        {
            menuItems.Clear();
            menuItems.AddRange(items);
            CalculateBounds();
        }

        /// <summary>
        /// Width of menu in pixels
        /// </summary>
        public int Width
        {
            get { return width; }
        }

        /// <summary>
        /// Height of menu in pixels
        /// </summary>
        public int Height
        {
            get { return height; }
        }

        /// <summary>
        /// Selected menu item index
        /// </summary>
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; }
        }

        /// <summary>
        /// Regular item color
        /// </summary>
        public Color RegularColor
        {
            get { return regularColor; }
            set { regularColor = value; }
        }

        /// <summary>
        /// Selected item color
        /// </summary>
        public Color SelectedColor
        {
            get { return selectedColor; }
            set { selectedColor = value; }
        }

        /// <summary>
        /// Position of component in screen
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Get the menu bounds
        /// </summary>
        protected void CalculateBounds()
        {
            width = 0;
            height = 0;
            foreach (string item in menuItems)
            {
                Vector2 size = selectedFont.MeasureString(item);
                if (size.X > width)
                {
                    width = (int)size.X;
                }
                height += selectedFont.LineSpacing;
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            bool down, up;
            // Handle the keyboard
            down = (oldKeyboardState.IsKeyDown(Keys.Down) &&
                (keyboardState.IsKeyUp(Keys.Down)));
            up = (oldKeyboardState.IsKeyDown(Keys.Up) &&
                (keyboardState.IsKeyUp(Keys.Up)));
            // Handle the D-Pad
            down |= (oldGamePadState.DPad.Down == ButtonState.Pressed) &&
                    (gamepadState.DPad.Down == ButtonState.Released);
            up |= (oldGamePadState.DPad.Up == ButtonState.Pressed) &&
                (gamepadState.DPad.Up == ButtonState.Released);

            if (down || up)
            {
                audio.MenuScroll.Play();
            }

            if (down)
            {
                selectedIndex++;
                if (selectedIndex == menuItems.Count)
                {
                    selectedIndex = 0;
                }
            }
            if (up)
            {
                selectedIndex--;
                if (selectedIndex == -1)
                {
                    selectedIndex = menuItems.Count - 1;
                }
            }

            oldKeyboardState = keyboardState;
            oldGamePadState = gamepadState;

            base.Update(gameTime);
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            float y = position.Y;
            float x;
            if (((menuItems.Count * regularFont.LineSpacing)+position.Y) < Game.Window.ClientBounds.Height)
            {
                for (int i = 0; i < menuItems.Count; i++)
                {
                    SpriteFont font;
                    Color theColor;
                    //x = position.X - ((menuItems[i].Length*45)/2);
                    if (i == SelectedIndex)
                    {
                        font = selectedFont;
                        theColor = selectedColor;
                    }
                    else
                    {
                        font = regularFont;
                        theColor = regularColor;
                    }
                    x = position.X - (font.MeasureString(menuItems[i]).X / 2);
                    // Draw the text shadow
                    spriteBatch.DrawString(font, menuItems[i],
                        new Vector2(x + 1, y + 1), Color.Black);
                    // Draw the text item
                    spriteBatch.DrawString(font, menuItems[i],
                        new Vector2(x, y), theColor);
                    y += font.LineSpacing;
                }
            }
            else // Need to draw 2 columns
            {
                for (int i = 0; i < menuItems.Count; i++)
                {
                    SpriteFont font;
                    Color theColor;
                    //x = position.X - ((menuItems[i].Length*45)/2);
                    if (i == SelectedIndex)
                    {
                        font = selectedFont;
                        theColor = selectedColor;
                    }
                    else
                    {
                        font = regularFont;
                        theColor = regularColor;
                    }
                    if (i <= menuItems.Count / 2)
                        x = position.X - position.X/2 - (font.MeasureString(menuItems[i]).X / 2);
                    else
                        x = position.X + position.X/2 - (font.MeasureString(menuItems[i]).X / 2);
                    // Draw the text shadow
                    spriteBatch.DrawString(font, menuItems[i],
                        new Vector2(x + 1, y + 1), Color.Black);
                    // Draw the text item
                    spriteBatch.DrawString(font, menuItems[i],
                        new Vector2(x, y), theColor);
                    if (i == (int)menuItems.Count / 2)
                        y = position.Y;
                    else
                        y += font.LineSpacing;
                }
            }

            base.Draw(gameTime);
        }
    }
}