using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Poseidon.Core;
namespace Poseidon
{
    public class Bubble
    {
        public Texture2D bubbleTexture;
        public Vector2 bubble2DPos;
        public Vector3 bubble3DPos;
        public float scale = 0.025f;
        public float timeLast = 2000.0f;
        public float startingScale;
        public float floatingSpeed;
        public float fluctutatingSpeed;
        public bool fromSeaBed;
        Random random;

        public Color fogColor;
        public Color ambientColor;
        public Color diffuseColor;
        public Color specularColor;

        int minX, maxX;
        double lastUpdate = 0;
        float camHeightScale;

        public void LoadContent(ContentManager Content, Vector3 position, bool fromSeaBed, float startingScale)
        {
            random = new Random();
            bubbleTexture = Content.Load<Texture2D>("Image/Miscellaneous/bubbleBlue");
            bubble3DPos = position;
            int randomFactor = random.Next(3);
            if (randomFactor == 2) bubble3DPos.X += 2;
            else if (randomFactor == 1) bubble3DPos.X -= 2;
            if (randomFactor == 2) bubble3DPos.Z += 2;
            else if (randomFactor == 1) bubble3DPos.Z -= 2;
            this.startingScale = startingScale;
        }
        public void LoadContentBubbleSmall(ContentManager Content, Vector2 position, int minX, int maxX)
        {
            random = new Random();
            startingScale = (float)random.Next(5, 10) / 10.0f * IngamePresentation.textScaleFactor;
            floatingSpeed = random.Next(20, 40);
            fluctutatingSpeed = 40;// random.Next(40, 60);
            bubbleTexture = Content.Load<Texture2D>("Image/Miscellaneous/bubble-small2");
            bubble2DPos = position;
            int randomFactor = random.Next(3);
            if (randomFactor == 2) bubble2DPos.X += 2;
            else if (randomFactor == 1) bubble2DPos.X -= 2;
            this.minX = minX;
            this.maxX = maxX;
            lastUpdate = PoseidonGame.playTime.TotalMilliseconds;
        }
        public void Update(GraphicsDevice graphicDevice, Camera gameCamera, GameTime gameTime)
        {
            EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);

            Vector3 screenPos = graphicDevice.Viewport.Project(bubble3DPos, gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity);
            bubble2DPos.X = screenPos.X;
            bubble2DPos.Y = screenPos.Y;
            timeLast -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timeLast >= 0)
                scale = startingScale * (2000.0f / timeLast);
            scale = MathHelper.Clamp(scale, startingScale, startingScale * 5);
            camHeightScale = (float)GameConstants.StandardCamHeight / (float)gameCamera.camHeight;
        }
        public void UpdateBubbleSmall()
        {
            int randomFactor = random.Next(3);
            float fluctuateDistance = fluctutatingSpeed * (float)(PoseidonGame.playTime.TotalMilliseconds - lastUpdate) / 1000;
            if (randomFactor == 2 && bubble2DPos.X + bubbleTexture.Width / 2 * startingScale + fluctuateDistance <= maxX)
            {
                bubble2DPos.X += fluctuateDistance;
            }
            else if (randomFactor == 1 && bubble2DPos.X - bubbleTexture.Width / 2 * startingScale - fluctuateDistance >= minX)
                bubble2DPos.X -= fluctuateDistance;
            bubble2DPos.Y -= floatingSpeed * (float)(PoseidonGame.playTime.TotalMilliseconds - lastUpdate) / 1000;
            lastUpdate = PoseidonGame.playTime.TotalMilliseconds;
        }
        public void Draw(SpriteBatch spriteBatch, float scaleUp)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(bubbleTexture, bubble2DPos, null, specularColor * camHeightScale, 0, new Vector2(bubbleTexture.Height / 2, bubbleTexture.Width / 2), scale * IngamePresentation.textScaleFactor * camHeightScale, SpriteEffects.None, 0);
            spriteBatch.End();
        }
        public void DrawBubbleSmall(SpriteBatch spriteBatch)
        {
            //spriteBatch.Begin();
            spriteBatch.Draw(bubbleTexture, bubble2DPos, null, Color.White, 0, new Vector2(bubbleTexture.Height / 2, bubbleTexture.Width / 2), startingScale, SpriteEffects.None, 0);
            //spriteBatch.End();
        }
    }
}
