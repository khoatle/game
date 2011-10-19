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
        public float scale = 1.0f;
        public float timeLast = 3000.0f;
        Random random;

        public void LoadContent(ContentManager Content, Vector3 position)
        {
            random = new Random();
            bubbleTexture = Content.Load<Texture2D>("Image/bubbleBlue");
            bubble3DPos = position;
            if (random.Next(2) < 1) bubble3DPos.X += 2;
            else bubble3DPos.X -= 2;
            if (random.Next(2) < 1) bubble3DPos.Z += 2;
            else bubble3DPos.Z -= 2;
        }
        public void Update()
        {
            Vector3 screenPos = PlayGameScene.GraphicDevice.Viewport.Project(bubble3DPos, PlayGameScene.gameCamera.ProjectionMatrix, PlayGameScene.gameCamera.ViewMatrix, Matrix.Identity);
            bubble2DPos.X = screenPos.X;
            bubble2DPos.Y = screenPos.Y;
            timeLast -= (float)PlayGameScene.timming.ElapsedGameTime.TotalMilliseconds;
            scale = 0.025f * (timeLast / 3000.0f);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            //spriteBatch.Draw(bubbleTexture, bubblePos, null, Color.White, 0, new Vector2(bubbleTexture.Width/2, bubbleTexture.Height/2), SpriteEffects.None, 0);
            spriteBatch.Draw(bubbleTexture, bubble2DPos, null, Color.White, 0, new Vector2(bubbleTexture.Height / 2, bubbleTexture.Width / 2), scale, SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }
}
