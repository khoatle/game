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
    public class HelpScene : GameScene
    {
        Random random = new Random();
        SpriteBatch spriteBatch;
        GraphicsDevice graphicsDevice;
        Rectangle textureFrontRectangle, nextRectangle, backRectangle;
        int sceneCount;
        Texture2D textureFront1, textureFront2, textureFront3, textureFront4, textureFront5, textureNextButton, textureBackButton;
        // Audio
        protected AudioLibrary audio;
        MouseState currentMouseState = new MouseState();
        MouseState lastMouseState = new MouseState();
        SpriteFont menuSmall;

        public HelpScene(Game game, Texture2D textureBack, Texture2D textureFront1, Texture2D textureFront2, Texture2D textureFront3, Texture2D textureFront4, Texture2D textureFront5, Texture2D nextButton, SpriteBatch spriteBatch, GraphicsDevice GraphicDevice, SpriteFont font)
            : base(game)
        {
            Components.Add(new ImageComponent(game, textureBack,
                ImageComponent.DrawMode.Stretch));
            this.spriteBatch = spriteBatch;
            this.textureFront1 = textureFront1;
            this.textureFront2 = textureFront2;
            this.textureFront3 = textureFront3;
            this.textureFront4 = textureFront4;
            this.textureFront5 = textureFront5;
            this.textureNextButton = nextButton;
            textureBackButton = PoseidonGame.contentManager.Load<Texture2D>("Image/ButtonTextures/backHelpButton");
            // Get the audio library
            audio = (AudioLibrary)
                Game.Services.GetService(typeof(AudioLibrary));
            cursor = new Cursor(game, spriteBatch);
            cursor.targetToLock = null;
            this.graphicsDevice = GraphicDevice;
            //System.Diagnostics.Debug.WriteLine("Viewport Width: " + graphicsDevice.Viewport.TitleSafeArea.Width + " Viewport Height: " + graphicsDevice.Viewport.TitleSafeArea.Height + " center " + graphicsDevice.Viewport.TitleSafeArea.Center);
            //System.Diagnostics.Debug.WriteLine("GameWindow Width: " + game.Window.ClientBounds.Width + " Viewport Height: " + game.Window.ClientBounds.Height + " Center "+game.Window.ClientBounds.Center.X);
            int textureWidth = (int)(graphicsDevice.Viewport.TitleSafeArea.Width * 0.625);
            int textureHeight = (int)(graphicsDevice.Viewport.TitleSafeArea.Height * 0.75);
            textureFrontRectangle = new Rectangle(graphicsDevice.Viewport.TitleSafeArea.Center.X - textureWidth / 2, graphicsDevice.Viewport.TitleSafeArea.Center.Y - textureHeight / 2, textureWidth, textureHeight);
                        
            int nextRectangleWidth = (int)(graphicsDevice.Viewport.TitleSafeArea.Width * 0.05);
            int nextRectangleHeight = (int)(graphicsDevice.Viewport.TitleSafeArea.Height * 0.125);
            nextRectangle = new Rectangle(textureFrontRectangle.Right - (nextRectangleWidth + 30), textureFrontRectangle.Center.Y - nextRectangleHeight/2, nextRectangleWidth, nextRectangleHeight);
            backRectangle = new Rectangle(textureFrontRectangle.Left + 30, textureFrontRectangle.Center.Y - nextRectangleHeight / 2, nextRectangleWidth, nextRectangleHeight);
            sceneCount = 1;
            menuSmall = font;
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
            //Check Click on next button
            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            if (lastMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released)
            {
                if (nextRectangle.Intersects(new Rectangle(lastMouseState.X, lastMouseState.Y, 5, 5)))
                    sceneCount++;
                if (backRectangle.Intersects(new Rectangle(lastMouseState.X, lastMouseState.Y, 5, 5)))
                    sceneCount--;
                if (sceneCount > 5) sceneCount = 1;
                else if (sceneCount < 1) sceneCount = 5;
            }
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            //Update(gameTime);
            spriteBatch.Begin();
            base.Draw(gameTime);
            Texture2D textureFront;
            switch (sceneCount)
            {
                case 1:
                    textureFront = textureFront1;
                    break;
                case 2:
                    textureFront = textureFront2;
                    break;
                case 3:
                    textureFront = textureFront3;
                    break;
                case 4:
                    textureFront = textureFront4;
                    break;
                case 5:
                    textureFront = textureFront5;
                    break;
                default:
                    sceneCount = 1;
                    textureFront = textureFront1;
                    break;
            }
            spriteBatch.Draw(textureFront, textureFrontRectangle, Color.White);
            spriteBatch.Draw(textureNextButton, nextRectangle, Color.White);
            spriteBatch.Draw(textureBackButton, backRectangle, Color.White);

            string nextText = "Press Enter/Esc to return";
            Vector2 nextTextPosition = new Vector2(graphicsDevice.Viewport.TitleSafeArea.Right - menuSmall.MeasureString(nextText).X*GameConstants.generalTextScaleFactor, graphicsDevice.Viewport.TitleSafeArea.Bottom - menuSmall.MeasureString(nextText).Y*GameConstants.generalTextScaleFactor);
            spriteBatch.DrawString(menuSmall, nextText, nextTextPosition, Color.White, 0f, new Vector2(0,0), GameConstants.generalTextScaleFactor, SpriteEffects.None, 0f);

            cursor.Draw(gameTime);
            spriteBatch.End();
        }
    }
}