using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Poseidon.MiniGames
{
    class WritingBox : RectangleBox {
        private string text;
        private SpriteFont font;

        Keys[] keysToCheck = new Keys[] { 
            Keys.A, Keys.B, Keys.C, Keys.D, Keys.E,
            Keys.F, Keys.G, Keys.H, Keys.I, Keys.J,
            Keys.K, Keys.L, Keys.M, Keys.N, Keys.O,
            Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T,
            Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y,
            Keys.Z, Keys.Back, Keys.Space, Keys.OemComma,
            Keys.OemPeriod, Keys.OemQuestion, Keys.OemSemicolon,
            Keys.OemQuotes
        };

        private KeyboardState currentKeyboardState;
        private KeyboardState lastKeyboardState;

        public WritingBox(int X, int Y, int recWidth, int recHeight)
            : base(X, Y, recWidth, recHeight) {
                text = "";
        }

        public void loadContent(Texture2D background, SpriteFont font)
        {
            loadContent(background);
            this.font = font;
        }

        public void draw(SpriteBatch spriteBatch, Color color)
        {
            base.draw(spriteBatch);
            spriteBatch.Begin();

            spriteBatch.DrawString(font, text, new Vector2(posX, posY), color);

            spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);
            currentKeyboardState = Keyboard.GetState();

            foreach (Keys key in keysToCheck) { 
                if (CheckKey(key)) {
                    addKeyToText(key);
                    break;
                }
            }
            lastKeyboardState = currentKeyboardState;
        }

        public string getText() {
            return text;
        }

        public void setText(string newText) {
            text = newText;
        }

        private void addKeyToText(Keys key) {
            string newChar = "";

            if (text.Length >= 50 && key != Keys.Back)
                return;

            switch (key) {
                case Keys.A:
                    newChar += "a";
                    break;
                case Keys.B:
                    newChar += "b";
                    break;
                case Keys.C:
                    newChar += "c";
                    break;
                case Keys.D:
                    newChar += "d";
                    break;
                case Keys.E:
                    newChar += "e";
                    break;
                case Keys.F:
                    newChar += "f";
                    break;
                case Keys.G:
                    newChar += "g";
                    break;
                case Keys.H:
                    newChar += "h";
                    break;
                case Keys.I:
                    newChar += "i";
                    break;
                case Keys.J:
                    newChar += "j";
                    break;
                case Keys.K:
                    newChar += "k";
                    break;
                case Keys.L:
                    newChar += "l";
                    break;
                case Keys.M:
                    newChar += "m";
                    break;
                case Keys.N:
                    newChar += "n";
                    break;
                case Keys.O:
                    newChar += "o";
                    break;
                case Keys.P:
                    newChar += "p";
                    break;
                case Keys.Q:
                    newChar += "q";
                    break;
                case Keys.R:
                    newChar += "r";
                    break;
                case Keys.S:
                    newChar += "s";
                    break;
                case Keys.T:
                    newChar += "t";
                    break;
                case Keys.U:
                    newChar += "u";
                    break;
                case Keys.V:
                    newChar += "v";
                    break;
                case Keys.W:
                    newChar += "w";
                    break;
                case Keys.X:
                    newChar += "x";
                    break;
                case Keys.Y:
                    newChar += "y";
                    break;
                case Keys.Z:
                    newChar += "z";
                    break;
                case Keys.Space:
                    newChar += " ";
                    break;
                case Keys.OemComma:
                    newChar += ',';
                    break;
                case Keys.OemPeriod:
                    newChar += '.';
                    break;
                case Keys.OemQuestion:
                    newChar += '/';
                    break;
                case Keys.OemSemicolon:
                    newChar += ';';
                    break;
                case Keys.OemQuotes:
                    newChar += '\'';
                    break;
                case Keys.Back:
                    if (text.Length != 0)
                        text = text.Remove(text.Length - 1);
                    return;
            }
            if (currentKeyboardState.IsKeyDown(Keys.RightShift) ||
                currentKeyboardState.IsKeyDown(Keys.LeftShift)){
                    if (Char.IsLetter(newChar[0])) {
                        newChar = newChar.ToUpper();
                    }
                    else if (newChar.Equals("'") ) {
                        newChar = "\"";
                    }
                    else if (newChar == ";") {
                        newChar = ":";
                    }
                    else if (newChar == "/") {
                        newChar = "?";
                    }
            }
            text += newChar;
        }

        private bool CheckKey(Keys key) {
            return lastKeyboardState.IsKeyDown(key) && currentKeyboardState.IsKeyUp(key);
        }
    }
}
