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
        RectangleBox startBox;
        private bool isMatching;
        private Texture2D boxBackground;
        private Texture2D trafficLightRed;
        private Texture2D trafficLightYellow;
        private Texture2D trafficLightGreen;
        private ContentManager content;
        private SpriteFont font;
        public bool isWin = false;
        public float elapsedSeconds;
        public float timeInterval;
        public float timeBetweenUpdates;
        public float rewardingTime;

        /// <summary>
        /// Default Constructor
        public TypingGameScene(Game game, SpriteFont font, Texture2D boxBackground, Texture2D theme, ContentManager Content)
            : base(game) {
                content = Content;
                timeBetweenUpdates = (float)game.TargetElapsedTime.TotalSeconds;
                timeInterval = 10f;
                elapsedSeconds = 0;
                rewardingTime = 20;
                int topLeftX = PlayGameScene.GraphicDevice.Viewport.TitleSafeArea.Left,
                    downLeftY = PlayGameScene.GraphicDevice.Viewport.TitleSafeArea.Bottom,
                    width = PlayGameScene.GraphicDevice.Viewport.TitleSafeArea.Width,
                    height = PlayGameScene.GraphicDevice.Viewport.TitleSafeArea.Height;

                displayBox = new Textbox(topLeftX + 10, height/3, 1400, 350,
                    "Dream of the Red Chamber, composed by Cao Xueqin, " +
                    "is one of China's Four Great Classical Novels.");
                typingBox = new WritingBox(topLeftX + 10, downLeftY - 200, 1400, 40);

                isMatching = true;
                this.boxBackground = boxBackground;
                Components.Add(new ImageComponent(game, theme,
                                ImageComponent.DrawMode.Stretch));
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
            trafficLightRed = content.Load<Texture2D>("Image/traffic_red");
            trafficLightYellow = content.Load<Texture2D>("Image/traffic_yellow");
            trafficLightGreen = content.Load<Texture2D>("Image/traffic_green");
            ((WritingBox)typingBox).loadContent(boxBackground, font);
            ((Textbox)displayBox).loadContent(boxBackground, font);
            startBox = new RectangleBox(trafficLightGreen.Width + 10, 10, 100, 200);
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            List<string> words = ((Textbox)displayBox).getWords();
            elapsedSeconds += timeBetweenUpdates;
            if (elapsedSeconds < timeInterval)
                return;

            // TODO: Add your update logic here 
            rewardingTime -= timeBetweenUpdates;
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
            if (i < 0 || i >= words[display.getMarkupIndex()].Length) {
                isMatching = false;
                typingBox.update(gameTime);
            } else {
                isMatching = true;
                KeyboardState currentState = Keyboard.GetState();
                if (currentState.IsKeyDown(Keys.Space) || currentState.IsKeyDown(Keys.Enter)) {
                    if (words[display.getMarkupIndex()].Equals(typingBar.getText().TrimEnd())) {
                        typingBar.setText("");
                        ((Textbox)displayBox).incrementMarkUpIndex();
                    }
                } else {
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
            spriteBatch.Begin();
            base.Draw(gameTime);

            if (elapsedSeconds <= timeInterval)
            {
                string drawThis = "" + (int)(timeInterval - elapsedSeconds + 0.5);
                spriteBatch.Draw(boxBackground, new Vector2(startBox.posX, startBox.posY), Color.White);
 
                if (timeInterval - elapsedSeconds >= 6)
                {
                    drawThis = "Wait: " + drawThis; 
                    spriteBatch.DrawString(font, "" + drawThis, new Vector2(trafficLightRed.Width + 10, 10), Color.Yellow);

                    spriteBatch.Draw(trafficLightRed, new Vector2(10, 10), Color.White);
                }
                else {
                    drawThis = "Ready: " + drawThis;
                    spriteBatch.DrawString(font, "" + drawThis, new Vector2(trafficLightRed.Width + 10, 10), Color.Yellow);

                    spriteBatch.Draw(trafficLightYellow, new Vector2(10, 10), Color.White);
                }
            }
            else {
                spriteBatch.Draw(trafficLightGreen, new Vector2(10, 10), Color.White);
            }
            spriteBatch.End();
            
            if (isMatching)
            {
                ((WritingBox)typingBox).draw(spriteBatch, Color.Black);
            }
            else
            {
                ((WritingBox)typingBox).draw(spriteBatch, Color.Red);
            }
            displayBox.draw(spriteBatch);
            
        }
    }
}


