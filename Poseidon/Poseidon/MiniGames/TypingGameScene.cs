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
        Game game;
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
        public bool isOver = false;
        public float maxTime = 60;
        public float elapsedSeconds;
        public float timeInterval;
        public float timeBetweenUpdates;
        KeyboardState lastKeyboardState = new KeyboardState();
        KeyboardState currentKeyboardState = new KeyboardState();
        //introducing game rule and stuff
        bool introducing = false;
        Texture2D introductionTexture;
        // Audio
        protected AudioLibrary audio;
        Random random = new Random();
        //for displaying game prize
        int expAwarded = 0;
        /// <summary>
        /// Default Constructor
        public TypingGameScene(Game game, SpriteFont font, Texture2D boxBackground, Texture2D theme, ContentManager Content)
            : base(game) {
            this.game = game;
            content = Content;
            timeBetweenUpdates = (float)game.TargetElapsedTime.TotalSeconds;
            timeInterval = 10f;
            elapsedSeconds = 0;
            int topLeftX = PlayGameScene.GraphicDevice.Viewport.TitleSafeArea.Left,
                downLeftY = PlayGameScene.GraphicDevice.Viewport.TitleSafeArea.Bottom,
                width = PlayGameScene.GraphicDevice.Viewport.TitleSafeArea.Width,
                height = PlayGameScene.GraphicDevice.Viewport.TitleSafeArea.Height;
            ParagraphLibrary paragraphLib = new ParagraphLibrary();
            displayBox = new Textbox(topLeftX + 10, height - 240, width - 20, 200, paragraphLib.paragraphLib[random.Next(paragraphLib.paragraphLib.Count)].content);
            typingBox = new WritingBox(topLeftX + 10, height - 80, width - 20, 20);

            isMatching = true;
            this.boxBackground = boxBackground;
            Components.Add(new ImageComponent(game, theme,
                            ImageComponent.DrawMode.Stretch));
            this.font = font;
            // Get the current spritebatch
            spriteBatch = (SpriteBatch)Game.Services.GetService(
                                            typeof(SpriteBatch));
            // Get the audio library
            audio = (AudioLibrary)
                Game.Services.GetService(typeof(AudioLibrary));
            LoadContent();
        }

        /// <summary>
        /// Show the start scene
        /// </summary>
        public override void Show()
        {
            introducing = true;
            isOver = false;
            MediaPlayer.Stop();
            base.Show();
        }

        /// <summary>
        /// Hide the start scene
        /// </summary>
        public override void Hide()
        {
            MediaPlayer.Stop();
            base.Hide();
        }

        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
            trafficLightRed = content.Load<Texture2D>("Image/MinigameTextures/redlight");
            trafficLightYellow = content.Load<Texture2D>("Image/MinigameTextures/yellowlight");
            trafficLightGreen = content.Load<Texture2D>("Image/MinigameTextures/greenlight");
            introductionTexture = content.Load<Texture2D>("Image/MinigameTextures/TypeGameIntro");
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
            if (MediaPlayer.State.Equals(MediaState.Stopped))
            {
                MediaPlayer.Play(audio.minigameMusics[random.Next(GameConstants.NumMinigameBackgroundMusics)]);
            }
            if (isOver) return;
            if (introducing)
            {
                lastKeyboardState = currentKeyboardState;
                currentKeyboardState = Keyboard.GetState();
                if (lastKeyboardState.IsKeyDown(Keys.Enter) &&
                        currentKeyboardState.IsKeyUp(Keys.Enter))
                {
                    introducing = false;
                }
                return;
            }
            List<string> words = ((Textbox)displayBox).getWords();
            elapsedSeconds += timeBetweenUpdates;
            if (elapsedSeconds < timeInterval)
                return;

            // TODO: Add your update logic here 
            WritingBox typingBar = (WritingBox)typingBox;
            Textbox display = (Textbox)displayBox;
            if (display.getMarkupIndex() >= words.Count) {
                isOver = true;

                if (elapsedSeconds > 30) {
                    expAwarded = 100;
                }
                else if (elapsedSeconds > 10)
                {
                    expAwarded += 300;
                }
                else {
                    expAwarded += 500;
                }
                Tank.currentExperiencePts += expAwarded;
                return;
            }

            if (elapsedSeconds > maxTime) {

                isOver = true;
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
            if (introducing)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(introductionTexture, game.GraphicsDevice.Viewport.TitleSafeArea, Color.White);
                spriteBatch.End();
                return;
            }
            if (isOver)
            {
                spriteBatch.Begin();
                base.Draw(gameTime);
                spriteBatch.DrawString(font, "You gain " + expAwarded + " experience from the written test!", new Vector2(trafficLightRed.Width + 10, 10), Color.Red);
                spriteBatch.End();

                return;
            }
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            // TODO: Add your drawing code here
            spriteBatch.Begin();
            base.Draw(gameTime);


            if (elapsedSeconds <= timeInterval)
            {
                string drawThis = "" + (int)(timeInterval - elapsedSeconds + 0.5);
                //spriteBatch.Draw(boxBackground, new Vector2(startBox.posX, startBox.posY), Color.White);
 
                if (timeInterval - elapsedSeconds >= 6)
                {
                    drawThis = "Wait: " + drawThis;
                    spriteBatch.DrawString(font, "" + drawThis, new Vector2(game.GraphicsDevice.Viewport.TitleSafeArea.Center.X - font.MeasureString(drawThis).X, 10), Color.DarkRed);

                    //spriteBatch.Draw(trafficLightRed, new Vector2(10, 10), Color.White);
                    spriteBatch.Draw(trafficLightRed, new Vector2(game.GraphicsDevice.Viewport.TitleSafeArea.Center.X + 50, 10), Color.White);
                }
                else {
                    drawThis = "Ready: " + drawThis;
                    spriteBatch.DrawString(font, "" + drawThis, new Vector2(game.GraphicsDevice.Viewport.TitleSafeArea.Center.X - font.MeasureString(drawThis).X, 10), Color.DarkRed);

                    spriteBatch.Draw(trafficLightYellow, new Vector2(game.GraphicsDevice.Viewport.TitleSafeArea.Center.X + 50, 10), Color.White);
                }
            }
            else {
                string str = "Remaining time: ";
                //spriteBatch.Draw(trafficLightGreen, new Vector2(10, 10), Color.White);
                spriteBatch.Draw(trafficLightGreen, new Vector2(game.GraphicsDevice.Viewport.TitleSafeArea.Center.X+50, 10), Color.White);
                spriteBatch.DrawString(font, str + (int)(maxTime - elapsedSeconds), new Vector2(game.GraphicsDevice.Viewport.TitleSafeArea.Center.X - font.MeasureString(str).X, 10), Color.Red);
            }

            if (isOver) {
                spriteBatch.End();
                return;
            }

            spriteBatch.End();
            displayBox.draw(spriteBatch);
            if (isMatching)
            {
                ((WritingBox)typingBox).draw(spriteBatch, Color.Black);
            }
            else
            {
                ((WritingBox)typingBox).draw(spriteBatch, Color.Red);
            }
        }
    }
}


