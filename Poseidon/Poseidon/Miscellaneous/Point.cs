using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace Poseidon
{
    public class Point
    {
        private byte textAlpha = 255;
        private String text;
        private Color color;
        private SpriteFont pointsFont;
        private Vector2 point2DPos;
        private Vector3 point3DPos;
        public float timeLast = 2000.0f;
        public bool toBeRemoved = false;
        Random random;
     

        public void LoadContent(ContentManager Content, String text, Vector3 position, Color color)
        {
            pointsFont = Content.Load<SpriteFont>("Fonts/fishTalk");
            this.text = text;
            random = new Random();
            point3DPos = position;
            this.color = color;
        }
        public void Update(GraphicsDevice graphicDevice, Camera gameCamera, GameTime gameTime)
        {
            textAlpha = (byte)Math.Max(textAlpha - 2, 0); // Adjust to make text fade away faster
            Vector3 screenPos = graphicDevice.Viewport.Project(point3DPos, gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity);
            point2DPos.X = screenPos.X;
            point2DPos.Y = screenPos.Y;
            timeLast -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timeLast <= 0)
                toBeRemoved = true;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Color textcolor = color;
            float fadeFactor = ((float)textAlpha) / 255;
            textcolor = new Color(textcolor.R, textcolor.G, textcolor.B, textAlpha);
            spriteBatch.Begin();
            spriteBatch.DrawString(pointsFont, this.text, point2DPos, color * fadeFactor);
            spriteBatch.End();
        }
    }
}
