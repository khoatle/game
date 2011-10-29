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
        //SpriteFont statsFont;
        SpriteFont quizFont;

        Random random = new Random();
        QuizzesLibrary quizzesLibrary;
        int questionID;
        public int questionAnswered = 0;
        int selectedChoice;
        public int numRightAnswer = 0;

        bool displayRightWrongAnswer = false;
        bool rightAnswer = false;

        Vector2 positionA, positionB, positionC, positionD, positionAns, positionQs;
        Rectangle rectA, rectB, rectC, rectD, nextButtonRect;
        Vector2 clickPosition;

        Texture2D buttonTexture;
        Texture2D selectedButtonTexture;
        Texture2D nextButtonTexture;

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

            //statsFont = Content.Load<SpriteFont>("Fonts/StatsFont");
            quizFont = Content.Load<SpriteFont>("Fonts/quiz");


            nextButtonTexture = Content.Load<Texture2D>("Image/MinigameTextures/NextButton");
            buttonTexture = Content.Load<Texture2D>("Image/MinigameTextures/quizButton");
            selectedButtonTexture = Content.Load<Texture2D>("Image/MinigameTextures/quizButtonSelected");

            this.game = game;

            quizzesLibrary = new QuizzesLibrary();
            questionID = random.Next(quizzesLibrary.quizzesList.Count);

            positionQs = new Vector2(PlayGameScene.GraphicDevice.Viewport.TitleSafeArea.Left + 300, PlayGameScene.GraphicDevice.Viewport.TitleSafeArea.Top+105);
            positionA = new Vector2(PlayGameScene.GraphicDevice.Viewport.TitleSafeArea.Left + 100, PlayGameScene.GraphicDevice.Viewport.TitleSafeArea.Center.Y);
            positionB = new Vector2(PlayGameScene.GraphicDevice.Viewport.TitleSafeArea.Left + 100, PlayGameScene.GraphicDevice.Viewport.TitleSafeArea.Center.Y+100);
            positionC = new Vector2(PlayGameScene.GraphicDevice.Viewport.TitleSafeArea.Left + 100, PlayGameScene.GraphicDevice.Viewport.TitleSafeArea.Center.Y+200);
            positionD = new Vector2(PlayGameScene.GraphicDevice.Viewport.TitleSafeArea.Left + 100, PlayGameScene.GraphicDevice.Viewport.TitleSafeArea.Center.Y+300);
            positionAns = new Vector2 (PlayGameScene.GraphicDevice.Viewport.TitleSafeArea.Center.X, PlayGameScene.GraphicDevice.Viewport.TitleSafeArea.Bottom - 50);
            rectA = new Rectangle((int)positionA.X, (int)positionA.Y, 60, 60);
            rectB = new Rectangle((int)positionB.X, (int)positionB.Y, 60, 60);
            rectC = new Rectangle((int)positionC.X, (int)positionC.Y, 60, 60);
            rectD = new Rectangle((int)positionD.X, (int)positionD.Y, 60, 60);
            nextButtonRect = new Rectangle(PlayGameScene.GraphicDevice.Viewport.TitleSafeArea.Right - 220, PlayGameScene.GraphicDevice.Viewport.TitleSafeArea.Bottom - 100, 200, 80);

            cursor = new Cursor(game, spriteBatch);
            //Components.Add(cursor);
        }


        /// <summary>
        /// Show the start scene
        /// </summary>
        public override void Show()
        {
            audio.NewMeteor.Play();
            questionAnswered = 0;
            numRightAnswer = 0;
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
            cursor.Update(gameTime);
            CheckClick(gameTime);
            if (displayRightWrongAnswer)
            {
                if (clicked && nextButtonRect.Intersects(new Rectangle((int)clickPosition.X, (int)clickPosition.Y, 1, 1)))
                {
                    questionID = random.Next(quizzesLibrary.quizzesList.Count);
                    clicked = false;
                    if (questionAnswered < 4) questionAnswered++;
                    displayRightWrongAnswer = false;
                    rightAnswer = false;
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
                    audio.ChangeBullet.Play();
                    selectedChoice = 0;
                    clicked = false;
                }
                if ((lastKeyboardState.IsKeyDown(Keys.B) &&
                    currentKeyboardState.IsKeyUp(Keys.B))
                    ||
                    (clicked && rectB.Intersects(new Rectangle((int)clickPosition.X, (int)clickPosition.Y, 1, 1)))
                    )
                {
                    audio.ChangeBullet.Play();
                    selectedChoice = 1;
                    clicked = false;
                }
                if ((lastKeyboardState.IsKeyDown(Keys.B) &&
                    currentKeyboardState.IsKeyUp(Keys.B))
                    ||
                    (clicked && rectC.Intersects(new Rectangle((int)clickPosition.X, (int)clickPosition.Y, 1, 1)))
                    )
                {
                    audio.ChangeBullet.Play();
                    selectedChoice = 2;
                    clicked = false;
                }
                if ((lastKeyboardState.IsKeyDown(Keys.B) &&
                    currentKeyboardState.IsKeyUp(Keys.B))
                    ||
                    (clicked && rectD.Intersects(new Rectangle((int)clickPosition.X, (int)clickPosition.Y, 1, 1)))
                    )
                {
                    audio.ChangeBullet.Play();
                    selectedChoice = 3;
                    clicked = false;
                }
                if (selectedChoice != -1)
                {
                    displayRightWrongAnswer = true;
                    if (selectedChoice == quizzesLibrary.quizzesList[questionID].answerID)
                    {
                        rightAnswer = true;
                        numRightAnswer++;
                    }
                }
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
            Color color = Color.Lime;
            string question = AddingObjects.wrapLine(quizzesLibrary.quizzesList[questionID].question, 850 , quizFont);
            spriteBatch.DrawString(quizFont, question, positionQs, Color.Red);
            //draw 4 buttons
            spriteBatch.Draw(buttonTexture, rectA, Color.White);
            spriteBatch.Draw(buttonTexture, rectB, Color.White);
            spriteBatch.Draw(buttonTexture, rectC, Color.White);
            spriteBatch.Draw(buttonTexture, rectD, Color.White);
            // draw 4 answers
            spriteBatch.DrawString(quizFont, " A  " + quizzesLibrary.quizzesList[questionID].options[0], positionA, color);
            spriteBatch.DrawString(quizFont, " B  " + quizzesLibrary.quizzesList[questionID].options[1], positionB , color);
            spriteBatch.DrawString(quizFont, " C  " + quizzesLibrary.quizzesList[questionID].options[2], positionC , color);
            spriteBatch.DrawString(quizFont, " D  " + quizzesLibrary.quizzesList[questionID].options[3], positionD, color);
            if (displayRightWrongAnswer)
            {
                Color ansButtonColor;
                if (rightAnswer)
                {
                    spriteBatch.DrawString(quizFont, "CORRECT", positionAns, color);
                    ansButtonColor = Color.Yellow;
                }
                else
                {
                    spriteBatch.DrawString(quizFont, "WRONG", positionAns, color);
                    ansButtonColor = Color.Red;
                }
                switch (selectedChoice)
                {
                    case 0:
                        spriteBatch.Draw(selectedButtonTexture, rectA, ansButtonColor);
                        break;
                    case 1:
                        spriteBatch.Draw(selectedButtonTexture, rectB, ansButtonColor);
                        break;
                    case 2:
                        spriteBatch.Draw(selectedButtonTexture, rectC, ansButtonColor);
                        break;
                    case 3:
                        spriteBatch.Draw(selectedButtonTexture, rectD, ansButtonColor);
                        break;
                }
                spriteBatch.Draw(nextButtonTexture, nextButtonRect, Color.White);
            }
            cursor.Draw(gameTime);
            spriteBatch.End();
        }
    }
}


