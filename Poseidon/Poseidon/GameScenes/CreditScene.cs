#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Poseidon.Core;
using System;
using Microsoft.Xna.Framework.Media;

#endregion

namespace Poseidon
{
    /// <summary>
    /// This is a game component thats represents the Instrucions Scene
    /// </summary>
    public class CreditScene : GameScene
    {
        Random random = new Random();
        Texture2D backgroundTexture, foregroundTexture;
        SpriteBatch spriteBatch;
        GraphicsDevice graphicsDevice;
        // Audio
        protected AudioLibrary audio;
        public CreditScene(Game game, Texture2D textureBack, Texture2D textureFront, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
            : base(game)
        {
            this.spriteBatch = spriteBatch;
            backgroundTexture = textureBack;
            foregroundTexture = textureFront;
            this.graphicsDevice = graphicsDevice;
            // Get the audio library
            audio = (AudioLibrary)
                Game.Services.GetService(typeof(AudioLibrary));
        }
        public override void Update(GameTime gameTime)
        {
            if (MediaPlayer.State.Equals(MediaState.Stopped))
            {
                MediaPlayer.Play(audio.backgroundMusics[random.Next(GameConstants.NumNormalBackgroundMusics)]);
            }
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            System.Diagnostics.Debug.WriteLine("Inside draw function in credits");
            spriteBatch.Begin();
            base.Draw(gameTime);
            spriteBatch.Draw(backgroundTexture, new Rectangle(graphicsDevice.Viewport.TitleSafeArea.Top, graphicsDevice.Viewport.TitleSafeArea.Left, graphicsDevice.Viewport.TitleSafeArea.Width, graphicsDevice.Viewport.TitleSafeArea.Height), Color.White);
            spriteBatch.Draw(foregroundTexture, new Rectangle(graphicsDevice.Viewport.TitleSafeArea.Center.X - 400, graphicsDevice.Viewport.TitleSafeArea.Center.Y -300, 800, 600), Color.White);
            spriteBatch.End();
        }
    }
}