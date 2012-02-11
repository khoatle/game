using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon.Core
{
    class Button
    {
        public enum InteractionState { HOVER, SELECTED, OUT };
        public InteractionState state;
        private int width;
        private int height;
        private Vector2 topLeft;
        private Vector2 cursorPosition;
        private Texture2D buttonTexture;
        private bool anchored;
        private int buttonIndex;
        private float scale = 1.0f;
        private string name;
        private ButtonState previousRightButtonState;
        private bool selectSoundPlayed;

        public bool Anchored
        {
            get { return anchored; }
            set { anchored = value; }
        }

        public Vector2 TopLeft
        {
            get { return topLeft; }
        }

        public string Name
        {
            get { return name; }
        }

        // Rectangle of image strip we want to display
        Rectangle sourceRect;

        // Rectangle where we want to display the image strip
        Rectangle destinationRect;

        // source/Destination Rectangle for anchor
        Rectangle anchorSourceRect;
        Rectangle anchorDestinationRect;

        public void Initialize(int idx, string name, Texture2D buttonTexture, Vector2 topLeft, int width, int height, float scaleFactor)
        {
            this.buttonTexture = buttonTexture;
            this.topLeft = topLeft;
            this.width = width;
            this.height = height;
            this.name = name;
            scale = scaleFactor;
            buttonIndex = idx;
            anchored = false;
            state = InteractionState.OUT;
            cursorPosition = new Vector2();
            previousRightButtonState = ButtonState.Released;
            selectSoundPlayed = false;

            sourceRect = new Rectangle(0, idx * height, width, height);
            destinationRect = new Rectangle((int)topLeft.X, (int)topLeft.Y, (int)(width * scale), (int)(height * scale));
            anchorSourceRect = new Rectangle(idx * width, 4 * height, width, height);
        }

        public void Update(GameTime gameTime, MouseState mouseState, bool mouseOnPanelArea)
        {
            // check different mouse states and update source rectangle for button accordingly
            cursorPosition.X = mouseState.X;
            cursorPosition.Y = mouseState.Y;
            bool onButtonArea = destinationRect.Contains(mouseState.X, mouseState.Y);
            if (mouseState.LeftButton != ButtonState.Pressed && onButtonArea)
            {
                // HOVER
                state = InteractionState.HOVER;
                sourceRect = new Rectangle(width, buttonIndex * height, width, height);
            }
            else if (mouseState.LeftButton == ButtonState.Pressed && onButtonArea)
            {
                // SELECTED
                state = InteractionState.SELECTED;
                sourceRect = new Rectangle(2 * width, buttonIndex * height, width, height);
                anchored = true;

                if (!selectSoundPlayed)
                {
                    PoseidonGame.audio.MenuSelect.Play();
                    selectSoundPlayed = true;
                }
            }
            else if (mouseState.LeftButton == ButtonState.Pressed && !onButtonArea)
            {
                // OUT
                state = InteractionState.OUT;
                if (mouseOnPanelArea)
                {
                    // if mouse is still on panel area, it means another button is selected. So, un-anchor this button
                    anchored = false;
                }
            }
            else if (!onButtonArea)
            {
                // OUT
                state = InteractionState.OUT;
                sourceRect = new Rectangle(0, buttonIndex * height, width, height);
                anchorDestinationRect = new Rectangle((int)cursorPosition.X, (int)cursorPosition.Y, width, height);
            }

            if (mouseState.LeftButton == ButtonState.Released)
            {
                selectSoundPlayed = false; // reset boolean when leftbutton is released
            }

            if (previousRightButtonState == ButtonState.Pressed && mouseState.RightButton == ButtonState.Released)
            {
                // Anchor released
                anchored = false;
            }
            previousRightButtonState = mouseState.RightButton;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(buttonTexture, destinationRect, sourceRect, Color.White);
        }

        public void DrawAnchor(SpriteBatch spriteBatch)
        {
            if (anchored && state == InteractionState.OUT)
            {
                spriteBatch.Draw(buttonTexture, anchorDestinationRect, anchorSourceRect, Color.White);
            }
        }
    }
}
