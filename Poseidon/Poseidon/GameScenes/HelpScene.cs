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
    public class HelpScene : GameScene
    {
        Random random = new Random();
        SpriteBatch spriteBatch;
        // Audio
        protected AudioLibrary audio;
        public HelpScene(Game game, Texture2D textureBack, Texture2D textureFront, SpriteBatch spriteBatch)
            : base(game)
        {
            Components.Add(new ImageComponent(game, textureBack,
                ImageComponent.DrawMode.Stretch));
            Components.Add(new ImageComponent(game, textureFront,
                ImageComponent.DrawMode.Center));
            this.spriteBatch = spriteBatch;
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
            spriteBatch.Begin();
            base.Draw(gameTime);
            spriteBatch.End();
        }
    }
}