#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Poseidon.Core;

#endregion

namespace Poseidon
{
    /// <summary>
    /// This is a game component thats represents the Instrucions Scene
    /// </summary>
    public class HelpScene : GameScene
    {
        SpriteBatch spriteBatch;
        public HelpScene(Game game, Texture2D textureBack, Texture2D textureFront, SpriteBatch spriteBatch)
            : base(game)
        {
            Components.Add(new ImageComponent(game, textureBack,
                ImageComponent.DrawMode.Stretch));
            Components.Add(new ImageComponent(game, textureFront,
                ImageComponent.DrawMode.Center));
            this.spriteBatch = spriteBatch;
        }
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            base.Draw(gameTime);
            spriteBatch.End();
        }
    }
}