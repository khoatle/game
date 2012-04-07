using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Poseidon.MiniGames
{
    class Textbox : RectangleBox {
        private int markupIndex;
        private List<string> words;
        private SpriteFont font;

        public Textbox(int X, int Y, int recWidth, int recHeight, string text) 
            : base(X, Y, recWidth, recHeight) {
                markupIndex = 0;
                parseText(text);
        }

        public void loadContent(Texture2D background, SpriteFont font) {
            loadContent(background);
            this.font = font;
            doWordWrapping();
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            base.draw(spriteBatch);
            spriteBatch.Begin();

            Vector2 nextDrawPosition = new Vector2(posX, posY);

            for (int i = 0; i < words.Count; i++) {
                if (words[i].Equals("\n")) {
                    nextDrawPosition.X = posX;
                    nextDrawPosition.Y += font.LineSpacing * GameConstants.generalTextScaleFactor;
                }
                else if (i == markupIndex) {
                    spriteBatch.DrawString(font, words[i] + " ", nextDrawPosition, Color.Crimson, 0, Vector2.Zero, GameConstants.generalTextScaleFactor, SpriteEffects.None, 0);
                    nextDrawPosition.X += font.MeasureString(words[markupIndex] + " ").X * GameConstants.generalTextScaleFactor;
                } else {
                    spriteBatch.DrawString(font, words[i] + " ", nextDrawPosition, Color.Black, 0, Vector2.Zero, GameConstants.generalTextScaleFactor, SpriteEffects.None, 0);
                    nextDrawPosition.X += font.MeasureString(words[i] + " ").X * GameConstants.generalTextScaleFactor;
                }
            }
            spriteBatch.End();
        }

        public void incrementMarkUpIndex() {
            markupIndex++ ;
        }

        public int getMarkupIndex() {
            return markupIndex;
        }

        private void doWordWrapping()
        {
            string line = "";
            for (int i = 0; i < words.Count; i++) {
                if (font.MeasureString(line + words[i]).X * GameConstants.generalTextScaleFactor > width) {
                    if (markupIndex == i) {
                        markupIndex++;
                    }
                    words.Insert(i, "\n");
                    line = "";
                }
                line += words[i] + " ";
            }
        }

        private void parseText(string text)
        {
            string[] array = text.Split(' ');
            words = new List<string>();

            for (int i = 0; i < array.Length; i++) {
                words.Add(array[i]);
            }
        }

        public List<string> getWords() {
            return words;
        }
    }
}
