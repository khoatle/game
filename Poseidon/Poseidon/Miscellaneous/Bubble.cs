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
    public class Bubble
    {
        Texture2D bubbleTexture;
        public Vector2 bubble2DPos;
        public Vector3 bubble3DPos;
        public float scale = 0.025f;
        public float timeLast = 2000.0f;
        public float startingScale;
        public bool fromSeaBed;
        Random random;

        public void LoadContent(ContentManager Content, Vector3 position, bool fromSeaBed, float startingScale)
        {
            random = new Random();
            bubbleTexture = Content.Load<Texture2D>("Image/Miscellaneous/bubbleBlue");
            bubble3DPos = position;
            if (random.Next(3) == 2) bubble3DPos.X += 2;
            else if (random.Next(3) == 1) bubble3DPos.X -= 2;
            if (random.Next(3) == 2) bubble3DPos.Z += 2;
            else if (random.Next(3) == 1) bubble3DPos.Z -= 2;
            this.startingScale = startingScale;
        }
        public void Update(GraphicsDevice graphicDevice, Camera gameCamera, GameTime gameTime)
        {
            Vector3 screenPos = graphicDevice.Viewport.Project(bubble3DPos, gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity);
            bubble2DPos.X = screenPos.X;
            bubble2DPos.Y = screenPos.Y;
            timeLast -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timeLast >= 0)
                scale = startingScale * (2000.0f / timeLast);
        }
        public void Draw(SpriteBatch spriteBatch, float scaleUp)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(bubbleTexture, bubble2DPos, null, Color.White, 0, new Vector2(bubbleTexture.Height / 2, bubbleTexture.Width / 2), scale * scaleUp, SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }
}
