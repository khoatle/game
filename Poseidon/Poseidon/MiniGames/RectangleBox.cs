using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Poseidon.MiniGames
{
    class RectangleBox {
        private Rectangle box;
        protected int posX;
        protected int posY;
        protected int width;
        protected int height;
        private Texture2D background;

        public RectangleBox(int X, int Y, int recWidth, int recHeight) {
            posX = X;
            posY = Y;
            width = recWidth;
            height = recHeight;

            box = new Rectangle(posX, posY, width, height);
        }

        public virtual void initialize() { 
            
        }

        public virtual void loadContent(Texture2D background) {
            this.background = background;

        }

        public virtual void update(GameTime gameTime) { 
            
        }

        public virtual void draw(SpriteBatch spriteBatch) {
            spriteBatch.Begin();

            spriteBatch.Draw(background, box, Color.White);

            spriteBatch.End();
        }
    }
}
