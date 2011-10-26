#region Using Statements

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Poseidon.Core;
using Microsoft.Xna.Framework.Media;

#endregion

namespace Poseidon.MiniGames
{
    /// <summary>
    /// This is a game component that implements the Game Start Scene.
    /// </summary>
    public class TypingGameScene : GameScene
    {
        SpriteBatch spriteBatch;
        RectangleBox typingBox;
        RectangleBox displayBox;
        private bool isMatching;
        private Texture2D boxBackground;
        private ContentManager content;
        private SpriteFont font;
        public bool isWin = false;
        /// <summary>
        /// Default Constructor
        public TypingGameScene(Game game, SpriteFont font, Texture2D background, ContentManager Content)
            : base(game) {
                content = Content;
                displayBox = new Textbox(10, 10, 780, 350,
                    "Dream of the Red Chamber, composed by Cao Xueqin, " +
                    "is one of China's Four Great Classical Novels.");
                typingBox = new WritingBox(10, 400, 780, 40);
                isMatching = true;
                boxBackground = background;
                this.font = font;
                // Get the current spritebatch
                spriteBatch = (SpriteBatch)Game.Services.GetService(
                                                typeof(SpriteBatch));
                LoadContent();
        }

        /// <summary>
        /// Show the start scene
        /// </summary>
        public override void Show()
        {
            base.Show();
        }

        /// <summary>
        /// Hide the start scene
        /// </summary>
        public override void Hide()
        {
            base.Hide();
        }

        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
            ((WritingBox)typingBox).loadContent(boxBackground, font);
            ((Textbox)displayBox).loadContent(boxBackground, font);
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            List<string> words = ((Textbox)displayBox).getWords();

            // TODO: Add your update logic here 

            WritingBox typingBar = (WritingBox)typingBox;
            Textbox display = (Textbox)displayBox;
            if (display.getMarkupIndex() >= words.Count) {
                isWin = true;
                return;
            }
            if (words[display.getMarkupIndex()].Equals("\n"))
            {
                display.incrementMarkUpIndex();
                return;
            }
            int i = words[display.getMarkupIndex()].IndexOf(typingBar.getText());
            if (i < 0 || i >= words[display.getMarkupIndex()].Length)
            {
                isMatching = false;
                typingBox.update(gameTime);
            }
            else
            {
                isMatching = true;
                KeyboardState currentState = Keyboard.GetState();
                if (currentState.IsKeyDown(Keys.Space) || currentState.IsKeyDown(Keys.Enter))
                {
                    if (words[display.getMarkupIndex()].Equals(typingBar.getText().TrimEnd()))
                    {
                        typingBar.setText("");
                        ((Textbox)displayBox).incrementMarkUpIndex();
                    }
                }
                else
                {
                    typingBox.update(gameTime);
                }
            }
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            // TODO: Add your drawing code here
            if (isMatching)
            {
                ((WritingBox)typingBox).draw(spriteBatch, Color.White);
            }
            else
            {
                ((WritingBox)typingBox).draw(spriteBatch, Color.Red);
            }
            displayBox.draw(spriteBatch);
            base.Draw(gameTime);
        }
    }
}


