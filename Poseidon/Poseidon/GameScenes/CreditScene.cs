#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Poseidon.Core;
using System;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Poseidon
{
    /// <summary>
    /// This is a game component thats represents the Instrucions Scene
    /// </summary>
    public class CreditScene : GameScene
    {
        Random random = new Random();
        Texture2D backgroundTexture, textureNextButton;
        Texture2D[] creditForgroundTextures;
        Rectangle textureFrontRectangle, nextRectangle;
        SpriteBatch spriteBatch;
        GraphicsDevice graphicsDevice;
        public bool nextPressed;
        MouseState currentMouseState = new MouseState();
        MouseState lastMouseState = new MouseState();
        // Audio
        protected AudioLibrary audio;
        int textureIndex = 0;
        public CreditScene(Game game, Texture2D textureBack, Texture2D[] creditForgroundTextures, Texture2D textureNextButton, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
            : base(game)
        {
            this.spriteBatch = spriteBatch;
            backgroundTexture = textureBack;
            this.textureNextButton = textureNextButton;
            this.graphicsDevice = graphicsDevice;
            // Get the audio library
            audio = (AudioLibrary)
                Game.Services.GetService(typeof(AudioLibrary));
            cursor = new Cursor(game, spriteBatch);
            this.creditForgroundTextures = creditForgroundTextures;

            int creditSceneWidth = (int)(graphicsDevice.Viewport.TitleSafeArea.Width * 0.65);
            int creditSceneHeight = (int)(graphicsDevice.Viewport.TitleSafeArea.Height * 0.75);
            textureFrontRectangle = new Rectangle(graphicsDevice.Viewport.TitleSafeArea.Center.X - creditSceneWidth/2, graphicsDevice.Viewport.TitleSafeArea.Center.Y - creditSceneHeight/2, creditSceneWidth, creditSceneHeight);
         
            int nextRectangleWidth = (int)(graphicsDevice.Viewport.TitleSafeArea.Width * 0.05);
            int nextRectangleHeight = (int)(graphicsDevice.Viewport.TitleSafeArea.Height * 0.0875);
            nextRectangle = new Rectangle(textureFrontRectangle.Center.X - nextRectangleWidth/2, textureFrontRectangle.Bottom - nextRectangleHeight, nextRectangleWidth, nextRectangleHeight);
            nextPressed = false;
        }


        public override void Show()
        {
            cursor.SetMenuCursorImage();
            base.Show();
        }

        public override void Update(GameTime gameTime)
        {
            if (MediaPlayer.State.Equals(MediaState.Stopped))
            {
                MediaPlayer.Play(audio.mainMenuMusic);
            }
            cursor.Update(graphicsDevice, PlayGameScene.gameCamera, gameTime, null);
            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            if (!nextPressed && lastMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released)
            {
                if (nextRectangle.Intersects(new Rectangle(lastMouseState.X, lastMouseState.Y, 5, 5)))
                    nextPressed = true;
            }
            if (nextPressed)
            {
                textureIndex += 1;
                if (textureIndex == creditForgroundTextures.Length) textureIndex = 0;
                nextPressed = false;
            }
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            base.Draw(gameTime);
            spriteBatch.Draw(backgroundTexture, new Rectangle(graphicsDevice.Viewport.TitleSafeArea.Top, graphicsDevice.Viewport.TitleSafeArea.Left, graphicsDevice.Viewport.TitleSafeArea.Width, graphicsDevice.Viewport.TitleSafeArea.Height), Color.White);

            spriteBatch.Draw(creditForgroundTextures[textureIndex], textureFrontRectangle, Color.White);
            spriteBatch.Draw(textureNextButton, nextRectangle, Color.White);
            //else
            //{
            //    spriteBatch.Draw(foregroundTexture1, textureFrontRectangle, Color.White);
            //    spriteBatch.Draw(textureNextButton, nextRectangle, Color.White);
            //}
            cursor.Draw(gameTime);
            string nextText = "Press Enter/Esc to return";
            Vector2 nextTextPosition = new Vector2(graphicsDevice.Viewport.TitleSafeArea.Right - IngamePresentation.menuSmall.MeasureString(nextText).X * GameConstants.generalTextScaleFactor, graphicsDevice.Viewport.TitleSafeArea.Bottom - IngamePresentation.menuSmall.MeasureString(nextText).Y * GameConstants.generalTextScaleFactor);
            spriteBatch.DrawString(IngamePresentation.menuSmall, nextText, nextTextPosition, Color.White, 0f, new Vector2(0, 0), GameConstants.generalTextScaleFactor, SpriteEffects.None, 0f);
            spriteBatch.End();

        }
    }
}