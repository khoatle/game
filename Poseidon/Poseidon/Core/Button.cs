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
        public enum InteractionState { HOVER, PRESSED, OUT };
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
            switch (state)
            {
                case InteractionState.OUT:
                    if (onButtonArea)
                    {
                        state = InteractionState.HOVER;
                        sourceRect = new Rectangle(width, buttonIndex * height, width, height);
                    }
                    else
                    {
                        sourceRect = new Rectangle(0, buttonIndex * height, width, height);
                    }
                    break;
                case InteractionState.HOVER:
                    if (onButtonArea && mouseState.LeftButton == ButtonState.Pressed)
                    {
                        state = InteractionState.PRESSED;
                        sourceRect = new Rectangle(2 * width, buttonIndex * height, width, height);
                        PoseidonGame.audio.MenuSelect.Play();
                    }
                    else if (onButtonArea)
                    {
                        // state remains as HOVER
                        sourceRect = new Rectangle(width, buttonIndex * height, width, height);
                    }
                    else
                    {
                        state = InteractionState.OUT;
                        sourceRect = new Rectangle(0, buttonIndex * height, width, height);
                    }
                    break;
                case InteractionState.PRESSED:
                    if (onButtonArea && mouseState.LeftButton == ButtonState.Released)
                    {
                        state = InteractionState.HOVER;
                        sourceRect = new Rectangle(width, buttonIndex * height, width, height);
                        anchored = true;
                    } 
                    else if (onButtonArea && mouseState.LeftButton == ButtonState.Pressed) 
                    {
                        // state remains as pressed
                        sourceRect = new Rectangle(2 * width, buttonIndex * height, width, height);
                    } 
                    else
                    {
                        state = InteractionState.OUT;
                        sourceRect = new Rectangle(0, buttonIndex * height, width, height);
                    }
                    break;
            }

            // Store anchor destination rectangle. This will be used only if anchor is on
            anchorDestinationRect = new Rectangle((int)cursorPosition.X, (int)cursorPosition.Y, width, height);

            // Certain criteria of releasing anchor
            if (mouseState.LeftButton == ButtonState.Pressed && !onButtonArea && mouseOnPanelArea)
            {
                // if mouse is still on panel area, it means another button is selected. So, un-anchor this button
                anchored = false;
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
