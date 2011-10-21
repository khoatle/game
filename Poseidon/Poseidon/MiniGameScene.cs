#region Using Statements

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Poseidon.Core;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Poseidon
{
    /// <summary>
    /// This is a game component that implements the Game Start Scene.
    /// </summary>
    public class MiniGameScene : GameScene
    {
        // For mouse inputs
        MouseState currentMouseState = new MouseState();
        MouseState lastMouseState = new MouseState();
        KeyboardState lastKeyboardState = new KeyboardState();
        KeyboardState currentKeyboardState = new KeyboardState();
        bool doubleClicked = false;
        bool clicked = false;
        double clickTimer = 0;
        Cursor cursor;
        // Audio
        protected AudioLibrary audio;
        // Spritebatch
        protected SpriteBatch spriteBatch = null;
        ContentManager Content;
        Game game;
        SpriteFont statsFont;
        SpriteFont menuLarge;

        Random random = new Random();
        QuizzesLibrary quizzesLibrary;
        int questionID;
        public int questionAnswered = 0;
        int selectedChoice;

        bool displayRightWrongAnswer = false;
        /// <summary>
        /// Default Constructor
        public MiniGameScene(Game game, SpriteFont smallFont, SpriteFont largeFont,
                           Texture2D background, ContentManager Content)
            : base(game)
        {
            
            
            this.Content = Content;
            Components.Add(new ImageComponent(game, background,
                                            ImageComponent.DrawMode.Stretch));

            // Get the current spritebatch
            spriteBatch = (SpriteBatch)Game.Services.GetService(
                                            typeof(SpriteBatch));

            // Get the audio library
            audio = (AudioLibrary)
                Game.Services.GetService(typeof(AudioLibrary));

            statsFont = Content.Load<SpriteFont>("Fonts/StatsFont");
            menuLarge = Content.Load<SpriteFont>("Fonts/menuLarge");

            this.game = game;

            quizzesLibrary = new QuizzesLibrary();
            questionID = random.Next(quizzesLibrary.quizzesList.Count);

            cursor = new Cursor(game, spriteBatch);
            Components.Add(cursor);
        }


        /// <summary>
        /// Show the start scene
        /// </summary>
        public override void Show()
        {
            audio.NewMeteor.Play();
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
        public void CheckClick(GameTime gameTime)
        {
            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            clickTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (lastMouseState.LeftButton == ButtonState.Pressed
                && currentMouseState.LeftButton == ButtonState.Released)
            {

                if (clicked && (clickTimer < GameConstants.clickTimerDelay))
                {
                    doubleClicked = true;
                    clicked = false;
                }
                else
                {
                    doubleClicked = false;
                    clicked = true;
                }
                clickTimer = 0;
            }
        }
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {

            // dictates right/wrong answer
            // move to the next question
            // Reset the world for a new game
            if (displayRightWrongAnswer)
            {
                CheckClick(gameTime);
                if (clicked)
                {
                    questionID = random.Next(quizzesLibrary.quizzesList.Count);
                    clicked = false;
                    if (questionAnswered < 4) questionAnswered++;
                    displayRightWrongAnswer = false;
                }
            }
            else
            {
                selectedChoice = -1;
                lastKeyboardState = currentKeyboardState;
                currentKeyboardState = Keyboard.GetState();
                if (lastKeyboardState.IsKeyDown(Keys.A) &&
                    currentKeyboardState.IsKeyUp(Keys.A))
                {
                    selectedChoice = 0;
                }
                if (lastKeyboardState.IsKeyDown(Keys.B) &&
                    currentKeyboardState.IsKeyUp(Keys.B))
                {
                    selectedChoice = 1;
                }
                if (lastKeyboardState.IsKeyDown(Keys.B) &&
                    currentKeyboardState.IsKeyUp(Keys.B))
                {
                    selectedChoice = 2;
                }
                if (lastKeyboardState.IsKeyDown(Keys.B) &&
                    currentKeyboardState.IsKeyUp(Keys.B))
                {
                    selectedChoice = 3;
                }
                if (selectedChoice != -1) displayRightWrongAnswer = true;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            // Draw question and choices
            spriteBatch.Begin();
            base.Draw(gameTime);
            // draw the question
            spriteBatch.DrawString(menuLarge, quizzesLibrary.quizzesList[questionID].question, new Vector2(100, 200), Color.Yellow);
            // draw 4 answers
            spriteBatch.DrawString(menuLarge, "A. " + quizzesLibrary.quizzesList[questionID].options[0], new Vector2(100, 400), Color.Yellow);
            spriteBatch.DrawString(menuLarge, "B. " + quizzesLibrary.quizzesList[questionID].options[1], new Vector2(100, 500), Color.Yellow);
            spriteBatch.DrawString(menuLarge, "C. " + quizzesLibrary.quizzesList[questionID].options[2], new Vector2(100, 600), Color.Yellow);
            spriteBatch.DrawString(menuLarge, "D. " + quizzesLibrary.quizzesList[questionID].options[3], new Vector2(100, 700), Color.Yellow);
            if (displayRightWrongAnswer)
            {
                if (selectedChoice != quizzesLibrary.quizzesList[questionID].answerID)
                {
                    spriteBatch.DrawString(menuLarge, "Wrong, stupid motherfucker!", new Vector2(400, 400), Color.Yellow);
                }
                else
                {
                    spriteBatch.DrawString(menuLarge, "Right, smart motherfucker!", new Vector2(400, 400), Color.Yellow);
                }
            }
            spriteBatch.End();
        }
    }
}


