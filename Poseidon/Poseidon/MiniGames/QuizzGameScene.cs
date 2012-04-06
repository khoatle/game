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
        //introducing game rule and stuff
        bool introducing = false;
        Texture2D introductionTexture;

        GraphicsDevice graphicsDevice;

        /// <summary>
        /// Default Constructor
        public QuizzGameScene(Game game, Texture2D background, ContentManager Content, GraphicsDevice graphicsDevice)
            : base(game)
        {
            this.Content = Content;
            this.graphicsDevice = graphicsDevice;
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
            introductionTexture = Content.Load<Texture2D>("Image/MinigameTextures/QuizGameIntro");

            this.game = game;

            quizzesLibrary = new QuizzesLibrary();
            questionID = random.Next(quizzesLibrary.quizzesList.Count);

            positionQs = new Vector2(graphicsDevice.Viewport.TitleSafeArea.Left + 300, graphicsDevice.Viewport.TitleSafeArea.Top + 105);
            positionA = new Vector2(graphicsDevice.Viewport.TitleSafeArea.Left + 100, graphicsDevice.Viewport.TitleSafeArea.Center.Y);
            positionB = new Vector2(graphicsDevice.Viewport.TitleSafeArea.Left + 100, graphicsDevice.Viewport.TitleSafeArea.Center.Y+100);
            positionC = new Vector2(graphicsDevice.Viewport.TitleSafeArea.Left + 100, graphicsDevice.Viewport.TitleSafeArea.Center.Y+200);
            positionD = new Vector2(graphicsDevice.Viewport.TitleSafeArea.Left + 100, graphicsDevice.Viewport.TitleSafeArea.Center.Y+300);
            positionAns = new Vector2 (graphicsDevice.Viewport.TitleSafeArea.Center.X, graphicsDevice.Viewport.TitleSafeArea.Bottom - 50);
            rectA = new Rectangle((int)positionA.X, (int)positionA.Y, 60, 60);
            rectB = new Rectangle((int)positionB.X, (int)positionB.Y, 60, 60);
            rectC = new Rectangle((int)positionC.X, (int)positionC.Y, 60, 60);
            rectD = new Rectangle((int)positionD.X, (int)positionD.Y, 60, 60);
            nextButtonRect = new Rectangle(graphicsDevice.Viewport.TitleSafeArea.Right - 220, graphicsDevice.Viewport.TitleSafeArea.Bottom - 100, 200, 80);

            cursor = new Cursor(game, spriteBatch);
            cursor.targetToLock = null;
            //Components.Add(cursor);
        }


        /// <summary>
        /// Show the start scene
        /// </summary>
        public override void Show()
        {
            MediaPlayer.Stop();
            questionAnswered = 0;
            numRightAnswer = 0;
            introducing = true;
            //audio.NewMeteor.Play();
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
            if (MediaPlayer.State.Equals(MediaState.Stopped))
            {
                MediaPlayer.Play(audio.minigameMusics[random.Next(GameConstants.NumMinigameBackgroundMusics)]);
            }
            lastKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            if (introducing)
            {
                if (lastKeyboardState.IsKeyDown(Keys.Enter) &&
                        currentKeyboardState.IsKeyUp(Keys.Enter))
                {
                    introducing = false;
                }
            }
            // dictates right/wrong answer
            // move to the next question
            // Reset the world for a new game
            cursor.Update(graphicsDevice, PlayGameScene.gameCamera, gameTime, null);
            CheckClick(gameTime);
            if (displayRightWrongAnswer)
            {
                if ((lastKeyboardState.IsKeyDown(Keys.Enter) &&
                    currentKeyboardState.IsKeyUp(Keys.Enter))
                    ||
                    (clicked && nextButtonRect.Intersects(new Rectangle((int)clickPosition.X, (int)clickPosition.Y, 1, 1)))
                    )
                {
                    questionAnswered++;
                    quizzesLibrary.quizzesList.RemoveAt(questionID); //remove the old question from the list
                    if (questionAnswered < 4)
                    {          
                        questionID = random.Next(quizzesLibrary.quizzesList.Count);
                    }
                    clicked = false;
                    
                    displayRightWrongAnswer = false;
                    rightAnswer = false;
                }
            }
            else
            {
                selectedChoice = -1;
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
                if ((lastKeyboardState.IsKeyDown(Keys.C) &&
                    currentKeyboardState.IsKeyUp(Keys.C))
                    ||
                    (clicked && rectC.Intersects(new Rectangle((int)clickPosition.X, (int)clickPosition.Y, 1, 1)))
                    )
                {
                    audio.ChangeBullet.Play();
                    selectedChoice = 2;
                    clicked = false;
                }
                if ((lastKeyboardState.IsKeyDown(Keys.D) &&
                    currentKeyboardState.IsKeyUp(Keys.D))
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
            if (introducing)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(introductionTexture, game.GraphicsDevice.Viewport.TitleSafeArea, Color.White);
                spriteBatch.End();
                return;
            }
            // Draw question and choices
            spriteBatch.Begin();
            base.Draw(gameTime);
            if (questionAnswered < 4)
            {
                // draw the question
                Color color = Color.Lime;
                string question = IngamePresentation.wrapLine(quizzesLibrary.quizzesList[questionID].question, (int)(0.75f * game.Window.ClientBounds.Width), quizFont, GameConstants.generalTextScaleFactor);
                spriteBatch.DrawString(quizFont, question, new Vector2(game.Window.ClientBounds.Width/2, 200 * GameConstants.generalTextScaleFactor), Color.Red, 0, new Vector2(quizFont.MeasureString(question).X/2, quizFont.MeasureString(question).Y/2), GameConstants.generalTextScaleFactor, SpriteEffects.None, 0);
                //draw 4 buttons
                spriteBatch.Draw(buttonTexture, rectA, Color.White);
                spriteBatch.Draw(buttonTexture, rectB, Color.White);
                spriteBatch.Draw(buttonTexture, rectC, Color.White);
                spriteBatch.Draw(buttonTexture, rectD, Color.White);
                // draw 4 answers
                spriteBatch.DrawString(quizFont, " A  " + quizzesLibrary.quizzesList[questionID].options[0], positionA, color, 0, Vector2.Zero, GameConstants.generalTextScaleFactor, SpriteEffects.None, 0);
                spriteBatch.DrawString(quizFont, " B  " + quizzesLibrary.quizzesList[questionID].options[1], positionB, color, 0, Vector2.Zero, GameConstants.generalTextScaleFactor, SpriteEffects.None, 0);
                spriteBatch.DrawString(quizFont, " C  " + quizzesLibrary.quizzesList[questionID].options[2], positionC, color, 0, Vector2.Zero, GameConstants.generalTextScaleFactor, SpriteEffects.None, 0);
                spriteBatch.DrawString(quizFont, " D  " + quizzesLibrary.quizzesList[questionID].options[3], positionD, color, 0, Vector2.Zero, GameConstants.generalTextScaleFactor, SpriteEffects.None, 0);
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
                string skipText = "Press Esc to skip minigame.";
                Vector2 skipTextPosition = new Vector2(game.GraphicsDevice.Viewport.TitleSafeArea.Right - IngamePresentation.menuSmall.MeasureString(skipText).X * GameConstants.generalTextScaleFactor, 0);
                spriteBatch.DrawString(IngamePresentation.menuSmall, skipText, skipTextPosition, Color.Red, 0, Vector2.Zero, GameConstants.generalTextScaleFactor, SpriteEffects.None, 0);
                
            }
            spriteBatch.End();
        }
    }
}


