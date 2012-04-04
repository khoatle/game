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

        Texture2D bubbleTexture;
        int currentFrame = 0;
        double lastScaleChange;
        float scale = 1.0f;
        
        bool increase = true;

        public void LoadContent(ContentManager Content, String text, Vector3 position, Color color)
        {
            pointsFont = Content.Load<SpriteFont>("Fonts/fishTalk");
            bubbleTexture = Content.Load<Texture2D>("Image/Miscellaneous/bubble-new");
            this.text = text;
            random = new Random();
            point3DPos = position;
            this.color = color;
            lastScaleChange = PoseidonGame.playTime.TotalMilliseconds;
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
            //Rectangle sourceRectangle;
            //sourceRectangle = new Rectangle(currentFrame * 256, 0, 256, 256);
            //if (PoseidonGame.playTime.TotalMilliseconds - timeLastFrame >= 10)
            //{
            //    currentFrame++;
            //    timeLastFrame = PoseidonGame.playTime.TotalMilliseconds;
            //    if (currentFrame == 8) currentFrame = 0;
            //}
            float elapsedTime = (float)((PoseidonGame.playTime.TotalMilliseconds - lastScaleChange) / 1000);
            if (increase)
                scale += 0.5f * elapsedTime;
            else scale -= 0.5f * elapsedTime;
            if (scale >= 1.3f)
                increase = false;
            else if (scale <= 1.0f) increase = true;
            scale = MathHelper.Clamp(scale, 1.0f, 1.3f);
            lastScaleChange = PoseidonGame.playTime.TotalMilliseconds;

            spriteBatch.Begin();
            spriteBatch.Draw(bubbleTexture, point2DPos, null, Color.White * fadeFactor * (1 - pointsFont.MeasureString(this.text).X / bubbleTexture.Width) * 0.8f, 0, new Vector2(bubbleTexture.Width / 2, bubbleTexture.Height / 2), pointsFont.MeasureString(this.text).X / bubbleTexture.Width * scale, SpriteEffects.None, 0);
            spriteBatch.DrawString(pointsFont, this.text, point2DPos, color * fadeFactor * 1.2f, 0, new Vector2(pointsFont.MeasureString(this.text).X/2, pointsFont.MeasureString(this.text).Y/2), scale - 0.3f, SpriteEffects.None, 1);
            //spriteBatch.Draw(bubbleTexture, point2DPos, sourceRectangle, Color.White * fadeFactor, 0, new Vector2(sourceRectangle.Width / 2, sourceRectangle.Height / 2), pointsFont.MeasureString(this.text).X / sourceRectangle.Width * 1.2f, SpriteEffects.None, 0);      
            spriteBatch.End();
        }
    }
}
