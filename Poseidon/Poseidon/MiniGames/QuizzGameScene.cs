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

namespace Poseidon.MiniGames
{
    /// <summary>
    /// This is a game component that implements the Game Start Scene.
    /// </summary>
    public class QuizzGameScene : GameScene
    {
        // For mouse inputs
        MouseState currentMouseState = new MouseState();
        MouseState lastMouseState = new MouseState();
        KeyboardState lastKeyboardState = new KeyboardState();
        KeyboardState currentKeyboardState = new KeyboardState();
        bool doubleClicked = false;
        bool clicked = false;
        double clickTimer = 0;
        //Cursor cursor;
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

        Vector2 positionA, positionB, positionC, positionD;
        Rectangle rectA, rectB, rectC, rectD;
        Vector2 clickPosition;

        /// <summary>
        /// Default Constructor
        public QuizzGameScene(Game game, SpriteFont smallFont, SpriteFont largeFont,
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

            positionA = new Vector2(100, 400);
            positionB = new Vector2(100, 500);
            positionC = new Vector2(100, 600);
            positionD = new Vector2(100, 700);
            rectA = new Rectangle((int)positionA.X, (int)positionA.Y, 300, 50);
            rectB = new Rectangle((int)positionB.X, (int)positionB.Y, 300, 50);
            rectC = new Rectangle((int)positionC.X, (int)positionC.Y, 300, 50);
            rectD = new Rectangle((int)positionD.X, (int)positionD.Y, 300, 50);

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
                    clickPosition.X = lastMouseState.X;
                    clickPosition.Y = lastMouseState.Y;
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
            CheckClick(gameTime);
            if (displayRightWrongAnswer)
            {
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
                if ((lastKeyboardState.IsKeyDown(Keys.A) &&
                    currentKeyboardState.IsKeyUp(Keys.A))
                    ||
                    (clicked && rectA.Intersects(new Rectangle((int)clickPosition.X, (int)clickPosition.Y, 1, 1)))
                    )
                {
                    selectedChoice = 0;
                    clicked = false;
                }
                if ((lastKeyboardState.IsKeyDown(Keys.B) &&
                    currentKeyboardState.IsKeyUp(Keys.B))
                    ||
                    (clicked && rectB.Intersects(new Rectangle((int)clickPosition.X, (int)clickPosition.Y, 1, 1)))
                    )
                {
                    selectedChoice = 1;
                    clicked = false;
                }
                if ((lastKeyboardState.IsKeyDown(Keys.B) &&
                    currentKeyboardState.IsKeyUp(Keys.B))
                    ||
                    (clicked && rectC.Intersects(new Rectangle((int)clickPosition.X, (int)clickPosition.Y, 1, 1)))
                    )
                {
                    selectedChoice = 2;
                    clicked = false;
                }
                if ((lastKeyboardState.IsKeyDown(Keys.B) &&
                    currentKeyboardState.IsKeyUp(Keys.B))
                    ||
                    (clicked && rectD.Intersects(new Rectangle((int)clickPosition.X, (int)clickPosition.Y, 1, 1)))
                    )
                {
                    selectedChoice = 3;
                    clicked = false;
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
            Color color = Color.DarkGoldenrod;
            spriteBatch.DrawString(menuLarge, quizzesLibrary.quizzesList[questionID].question, new Vector2(100, 200), color);
            // draw 4 answers
            spriteBatch.DrawString(menuLarge, "A. " + quizzesLibrary.quizzesList[questionID].options[0], positionA, color);
            spriteBatch.DrawString(menuLarge, "B. " + quizzesLibrary.quizzesList[questionID].options[1], positionB , color);
            spriteBatch.DrawString(menuLarge, "C. " + quizzesLibrary.quizzesList[questionID].options[2], positionC , color);
            spriteBatch.DrawString(menuLarge, "D. " + quizzesLibrary.quizzesList[questionID].options[3], positionD, color);
            if (displayRightWrongAnswer)
            {
                if (selectedChoice != quizzesLibrary.quizzesList[questionID].answerID)
                {
                    spriteBatch.DrawString(menuLarge, "Wrong, stupid motherfucker!", new Vector2(400, 400), color);
                }
                else
                {
                    spriteBatch.DrawString(menuLarge, "Right, smart motherfucker!", new Vector2(400, 400), color);
                }
            }
            spriteBatch.End();
        }
    }
}


