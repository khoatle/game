#region Using Statements

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
        private List<Rectangle> rectMenuItems;
        // Size of menu in pixels
        protected int width, height;
        // For audio effects
        protected AudioLibrary audio;
        
        MouseState currentMouseState = new MouseState();
        MouseState lastMouseState = new MouseState();
        public static bool clicked = false;

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
            rectMenuItems = new List<Rectangle>();

            // Get the current spritebatch
            spriteBatch = (SpriteBatch)
                Game.Services.GetService(typeof(SpriteBatch));

            // Get the audio library
            audio = (AudioLibrary)
                Game.Services.GetService(typeof(AudioLibrary));

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

            rectMenuItems.Clear();

            int x,y, width, height;
            y = (int)position.Y;
            if (((menuItems.Count * regularFont.LineSpacing) + position.Y) < Game.Window.ClientBounds.Height) // 1 column
            {

                for (int i = 0; i < menuItems.Count; i++)
                {
                    width = (int)(regularFont.MeasureString(menuItems[i]).X);
                    height = (int)(regularFont.MeasureString(menuItems[i]).Y);
                    x = (int)(position.X - (width / 2));
                    
                    Rectangle itemRectangle = new Rectangle(x,y, width, height);
                    rectMenuItems.Add(itemRectangle);
                    
                    y += regularFont.LineSpacing;
                }
            }
            else if (((menuItems.Count * regularFont.LineSpacing) + position.Y) < Game.Window.ClientBounds.Height * 2)  // Need to draw 2 columns
            {
                for (int i = 0; i < menuItems.Count; i++)
                {
                    width = (int)(regularFont.MeasureString(menuItems[i]).X);
                    height = (int)(regularFont.MeasureString(menuItems[i]).Y);
                    if (i <= menuItems.Count / 2)
                        x = (int)(position.X - (position.X / 2) - (width / 2));
                    else
                        x = (int)(position.X + (position.X / 2) - (width/ 2));

                    Rectangle itemRectangle = new Rectangle(x, y, width, height);
                    rectMenuItems.Add(itemRectangle);

                    if (i == (int)menuItems.Count / 2)
                        y = (int)position.Y;
                    else
                        y += regularFont.LineSpacing;
                }
            }
            else // Need to draw 3 columns
            {
                for (int i = 0; i < menuItems.Count; i++)
                {
                    width = (int)(regularFont.MeasureString(menuItems[i]).X);
                    height = (int)(regularFont.MeasureString(menuItems[i]).Y);
                    if (i <= menuItems.Count / 3)
                        x = (int)(position.X - (Game.Window.ClientBounds.Width / 3) - (width / 2));
                    else if (i <= menuItems.Count * 2 / 3)
                        x = (int)(position.X - (width / 2));
                    else
                        x = (int)(position.X + (Game.Window.ClientBounds.Width / 3) - (width / 2));

                    Rectangle itemRectangle = new Rectangle(x, y, width, height);
                    rectMenuItems.Add(itemRectangle);
                    
                    if ((i == (int)menuItems.Count / 3) || (i == (int)menuItems.Count * 2 / 3))
                        y = (int)position.Y;
                    else
                        y += regularFont.LineSpacing;
                }
            }

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
            int prevselectedIndex = selectedIndex;
            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            for(int i=0; i<menuItems.Count; i++)
            {
                if (rectMenuItems[i].Intersects(new Rectangle(currentMouseState.X, currentMouseState.Y, 10, 10)))
                {
                    selectedIndex = i;
                    break;
                }
            }
            
            if(prevselectedIndex != selectedIndex)
                audio.MenuScroll.Play();

            if(lastMouseState.LeftButton.Equals(ButtonState.Pressed) && currentMouseState.LeftButton.Equals(ButtonState.Released))
                clicked = true;

            base.Update(gameTime);
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < menuItems.Count; i++)
            {
                SpriteFont font;
                Color theColor;
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
                spriteBatch.DrawString(font, menuItems[i], new Vector2(rectMenuItems[i].Left+1, rectMenuItems[i].Top+1), theColor); //shadow
                spriteBatch.DrawString(font, menuItems[i], new Vector2(rectMenuItems[i].Left, rectMenuItems[i].Top), theColor);
            }

            base.Draw(gameTime);
        }
    }
}