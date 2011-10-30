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


        private KeyboardState currentKeyboardState;
        private KeyboardState lastKeyboardState;
        private float timeTillRapidDelete;
        private bool beginRapidDelete;
        private float timeBetweenDelete;
        private TimeSpan prevDelete;

        public WritingBox(int X, int Y, int recWidth, int recHeight)
            : base(X, Y, recWidth, recHeight) {
                text = "";
                timeBetweenDelete = 0.04f;
                timeTillRapidDelete = 0.5f;
                beginRapidDelete = false;
                prevDelete = new TimeSpan();
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

            if (currentKeyboardState.IsKeyDown(Keys.Back))
            {
                handleDelete(gameTime.TotalGameTime);
                return;
            }

            beginRapidDelete = false;
            prevDelete = new TimeSpan();
            string inputString = TryConvertKeyboardInput(currentKeyboardState, lastKeyboardState);
            text += inputString;

            lastKeyboardState = currentKeyboardState;
        }

        private void handleDelete(TimeSpan deleteTime) {
            if (text.Length != 0) {
                if (beginRapidDelete)
                {
                    if (deleteTime.TotalSeconds - prevDelete.TotalSeconds >= timeBetweenDelete)
                    {
                        text = text.Remove(text.Length - 1);
                        prevDelete = deleteTime;
                    }
                }
                else {
                    if (prevDelete.TotalSeconds == 0) {
                        prevDelete = deleteTime;
                        text = text.Remove(text.Length - 1);
                    }
                    else if (deleteTime.TotalSeconds - prevDelete.TotalSeconds >= timeTillRapidDelete) {
                        beginRapidDelete = true;
                    }
                }
            }
        }

        public string getText() {
            return text;
        }

        public void setText(string newText) {
            text = newText;
        }

        private static string TryConvertKeyboardInput(KeyboardState keyboard, KeyboardState oldKeyboard) {
            Keys[] keys = keyboard.GetPressedKeys();
            string returnString = "";
            bool shift = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);

            for (int i = 0; i < keys.Length; i++) {
                if (!oldKeyboard.IsKeyDown(keys[i])) {
                    switch (keys[i])
                    {
                        //Alphabet keys
                        case Keys.A: if (shift) { returnString += 'A'; } else { returnString += 'a'; } break;
                        case Keys.B: if (shift) { returnString += 'B'; } else { returnString += 'b'; } break;
                        case Keys.C: if (shift) { returnString += 'C'; } else { returnString += 'c'; } break;
                        case Keys.D: if (shift) { returnString += 'D'; } else { returnString += 'd'; } break;
                        case Keys.E: if (shift) { returnString += 'E'; } else { returnString += 'e'; } break;
                        case Keys.F: if (shift) { returnString += 'F'; } else { returnString += 'f'; } break;
                        case Keys.G: if (shift) { returnString += 'G'; } else { returnString += 'g'; } break;
                        case Keys.H: if (shift) { returnString += 'H'; } else { returnString += 'h'; } break;
                        case Keys.I: if (shift) { returnString += 'I'; } else { returnString += 'i'; } break;
                        case Keys.J: if (shift) { returnString += 'J'; } else { returnString += 'j'; } break;
                        case Keys.K: if (shift) { returnString += 'K'; } else { returnString += 'k'; } break;
                        case Keys.L: if (shift) { returnString += 'L'; } else { returnString += 'l'; } break;
                        case Keys.M: if (shift) { returnString += 'M'; } else { returnString += 'm'; } break;
                        case Keys.N: if (shift) { returnString += 'N'; } else { returnString += 'n'; } break;
                        case Keys.O: if (shift) { returnString += 'O'; } else { returnString += 'o'; } break;
                        case Keys.P: if (shift) { returnString += 'P'; } else { returnString += 'p'; } break;
                        case Keys.Q: if (shift) { returnString += 'Q'; } else { returnString += 'q'; } break;
                        case Keys.R: if (shift) { returnString += 'R'; } else { returnString += 'r'; } break;
                        case Keys.S: if (shift) { returnString += 'S'; } else { returnString += 's'; } break;
                        case Keys.T: if (shift) { returnString += 'T'; } else { returnString += 't'; } break;
                        case Keys.U: if (shift) { returnString += 'U'; } else { returnString += 'u'; } break;
                        case Keys.V: if (shift) { returnString += 'V'; } else { returnString += 'v'; } break;
                        case Keys.W: if (shift) { returnString += 'W'; } else { returnString += 'w'; } break;
                        case Keys.X: if (shift) { returnString += 'X'; } else { returnString += 'x'; } break;
                        case Keys.Y: if (shift) { returnString += 'Y'; } else { returnString += 'y'; } break;
                        case Keys.Z: if (shift) { returnString += 'Z'; } else { returnString += 'z'; } break;
 
                        //Decimal keys
                        case Keys.D0: if (shift) { returnString += ')'; } else { returnString += '0'; } break;
                        case Keys.D1: if (shift) { returnString += '!'; } else { returnString += '1'; } break;
                        case Keys.D2: if (shift) { returnString += '@'; } else { returnString += '2'; } break;
                        case Keys.D3: if (shift) { returnString += '#'; } else { returnString += '3'; } break;
                        case Keys.D4: if (shift) { returnString += '$'; } else { returnString += '4'; } break;
                        case Keys.D5: if (shift) { returnString += '%'; } else { returnString += '5'; } break;
                        case Keys.D6: if (shift) { returnString += '^'; } else { returnString += '6'; } break;
                        case Keys.D7: if (shift) { returnString += '&'; } else { returnString += '7'; } break;
                        case Keys.D8: if (shift) { returnString += '*'; } else { returnString += '8'; } break;
                        case Keys.D9: if (shift) { returnString += '('; } else { returnString += '9'; } break;

                        //Decimal numpad keys
                        case Keys.NumPad0: returnString += '0'; break;
                        case Keys.NumPad1: returnString += '1'; break;
                        case Keys.NumPad2: returnString += '2'; break;
                        case Keys.NumPad3: returnString += '3'; break;
                        case Keys.NumPad4: returnString += '4'; break;
                        case Keys.NumPad5: returnString += '5'; break;
                        case Keys.NumPad6: returnString += '6'; break;
                        case Keys.NumPad7: returnString += '7'; break;
                        case Keys.NumPad8: returnString += '8'; break;
                        case Keys.NumPad9: returnString += '9'; break;

                        //Special keys
                        case Keys.OemTilde: if (shift) { returnString += '~'; } else { returnString += '`'; } break;
                        case Keys.OemSemicolon: if (shift) { returnString += ':'; } else { returnString += ';'; } break;
                        case Keys.OemQuotes: if (shift) { returnString += '"'; } else { returnString += '\''; } break;
                        case Keys.OemQuestion: if (shift) { returnString += '?'; } else { returnString += '/'; } break;
                        case Keys.OemPlus: if (shift) { returnString += '+'; } else { returnString += '='; } break;
                        case Keys.OemPipe: if (shift) { returnString += '|'; } else { returnString += '\\'; } break;
                        case Keys.OemPeriod: if (shift) { returnString += '>'; } else { returnString += '.'; } break;
                        case Keys.OemOpenBrackets: if (shift) { returnString += '{'; } else { returnString += '['; } break;
                        case Keys.OemCloseBrackets: if (shift) { returnString += '}'; } else { returnString += ']'; } break;
                        case Keys.OemMinus: if (shift) { returnString += '_'; } else { returnString += '-'; } break;
                        case Keys.OemComma: if (shift) { returnString += '<'; } else { returnString += ','; } break;
                        case Keys.Space: returnString += ' '; break;
                    }
                }
            }

            return returnString;
        }

        private bool CheckKey(Keys key) {
            return lastKeyboardState.IsKeyDown(key) && currentKeyboardState.IsKeyUp(key);
        }
    }
}
