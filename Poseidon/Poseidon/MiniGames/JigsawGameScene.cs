using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Poseidon.Core;

namespace Poseidon.MiniGames
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class JigsawGameScene : GameScene
    {
        Game game;
        ContentManager content;
        GraphicsDevice graphicsDevice;

        public const int DEFAULT_PIECES_PER_ROW = 3;
        public const int DEFAULT_PIECES_PER_COL = 3;
        public const int SHUFFLING_STEP = 100;

        // Number of updates needed to slide 1 block or a group of block
        public const int numberOfUpdates = 20;

        // the top left position of the (0,0) piece
        public Vector2 topLeftPosition;
        public Vector2 frameTopLeftPosition;

        // User defined width and height of the whole image
        public int desiredWidthOfImage;
        public int desiredHeightOfImage;

        GraphicsDeviceManager graphics;
        // spriteBatch that does the drawing
        SpriteBatch spriteBatch;

        // The size of the puzzle
        public int numberOfRow;
        public int numberOfCol;

        // The 2D array holds all the pieces
        private Piece[][] jigsawPieces;

        // The whole picture
        private Texture2D image;
        private Texture2D frameImg, smallFrameImg;
        private Texture2D[] seacowImage, turtleImage, dolphinImage;
        // The empty cell pucture
        private Texture2D blackImage;

        // Bookeeping the position of the empty cell
        private int emptyCellRow;
        private int emptyCellCol;

        // Bookeeping the coordinate of the cell that the mouse last clicked
        private int mouseRow;
        private int mouseCol;

        // Is the block being slided
        private bool isSliding;
        // Distance traveled by all the blocks
        private float distanceTraveled;

        // String for debugging
        public string debugString, timerString;
        SpriteFont font, timerFont;

        public bool inOrder = false;
        public bool gamePlayed = false;
        bool gameStarted = false;

        private double timeNow, previewTime;
        public bool timeUp;

        Random random;

        private Video dnaAnimation;
        private Video extractDNAVid;
        private Video reconstructDNAVid;
        private Video fillGapsVid;
        private Video injectAndGrowVid;
        private enum VideoPlayBackState { ExtractingDNA, ReconstructingDNA, FillingGaps, InjectAndGrow };
        private VideoPlayBackState videoPlayBackState;
        private VideoPlayer videoPlayer, dnaAnimationVideoPlayer;
        private int vidIndex = 0;
        private string stepText = "";
        private string generalInfoText = "";
        private string animalName;

        Rectangle videoRectangle, descriptionRectangle;
        int startWidth, tabSpace;

        Rectangle letAIHandleButtonRectangle;
        Texture2D letAIHandleButtonTexture;
        bool letAIHandleButtonHover = false;
        public bool letAIHandle = false;

        public MouseState lastMouseState, currentMouseState;
        public bool clicked = false;
        bool mouseOnPlayButton = false;

        Texture2D startButton, selectedStartButton;//, skipButton;
        Rectangle startButtonRect;//, skipButtonRect;
        //string startText;
        float textScale;
        //Vector2 startTextPosition;
        
        float widthScale, heightScale;

        public JigsawGameScene(Game game, ContentManager Content, GraphicsDeviceManager graphic, GraphicsDevice graphicsDevice)
            : base(game)
        {
            this.game = game;
            this.content = Content;
            graphics = graphic;
            this.graphicsDevice = graphicsDevice;
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            cursor = new Cursor(game, spriteBatch);
            //Content.RootDirectory = "Content";
            random = new Random();

            widthScale = (float)game.Window.ClientBounds.Width / 1280;
            heightScale = (float)game.Window.ClientBounds.Height / 800;

            if (widthScale > 1.5) widthScale = 1.5f;
            if (heightScale > 1.5) heightScale = 1.5f;

            startWidth = (int)(25 * widthScale);
            tabSpace = (int)(50 * widthScale);

            int videoRectWidth = (int)(600*widthScale); 
            int videoRectHeight = (int)(480 * heightScale);
            int topTextHeight = (int)(100 * heightScale);
            videoRectangle = new Rectangle(startWidth, topTextHeight , videoRectWidth, videoRectHeight);

            int descRectWidth = videoRectWidth;
            int descRectHeight = game.Window.ClientBounds.Height - videoRectHeight - topTextHeight;
            descriptionRectangle = new Rectangle(startWidth, videoRectangle.Bottom+10, descRectWidth, descRectHeight);

            letAIHandleButtonRectangle = new Rectangle(50, 0, IngamePresentation.letAIHandleNormalTexture.Width, IngamePresentation.letAIHandleNormalTexture.Height);

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public override void Initialize()
        {
            // Init the spriteBatch
            //spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            // Make mouse visible
            //this.IsMouseVisible = true;

            initializeSizeOfPuzzle(DEFAULT_PIECES_PER_ROW, DEFAULT_PIECES_PER_COL);
            initializeEmptyCell(0, 0);

            // Tweak this
            topLeftPosition = new Vector2((game.Window.ClientBounds.Width / 2) + (30*widthScale), 75*(heightScale));
            frameTopLeftPosition = new Vector2(game.Window.ClientBounds.Width / 2, 20*heightScale);

            // Tweak this
            desiredWidthOfImage = game.Window.ClientBounds.Width / 2 - (int)(78 * widthScale);
            desiredHeightOfImage = game.Window.ClientBounds.Height - (int)(160 * heightScale);

            isSliding = false;
            distanceTraveled = 0;
            mouseOnPlayButton = false;
            videoPlayer = new VideoPlayer();
            dnaAnimationVideoPlayer = new VideoPlayer();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            font = content.Load<SpriteFont>("Fonts/JigsawFont");
            timerFont = IngamePresentation.menuSmall;

            seacowImage = new Texture2D[2];
            turtleImage = new Texture2D[2];
            dolphinImage = new Texture2D[2];
            seacowImage[0] = content.Load<Texture2D>("Image/MinigameTextures/StellersSeaCow2");
            turtleImage[0] = content.Load<Texture2D>("Image/MinigameTextures/Meiolania1");
            dolphinImage[0] = content.Load<Texture2D>("Image/MinigameTextures/mauiDolphin1");
            seacowImage[1] = content.Load<Texture2D>("Image/MinigameTextures/StellersSeaCow3");
            turtleImage[1] = content.Load<Texture2D>("Image/MinigameTextures/Meiolania2");
            dolphinImage[1] = content.Load<Texture2D>("Image/MinigameTextures/mauiDolphin2");
            
            blackImage = content.Load<Texture2D>("Image/MinigameTextures/BlackBox");
            frameImg = content.Load<Texture2D>("Image/MinigameTextures/Frame1");
            smallFrameImg = content.Load<Texture2D>("Image/MinigameTextures/Frame2");

            // Load/Initialize pieces here
            loadPieces();

            dnaAnimation = content.Load<Video>("Videos/dnaAnimation");
            extractDNAVid = content.Load<Video>("Videos/extractDNA");
            reconstructDNAVid = content.Load<Video>("Videos/reconstructDNA");
            fillGapsVid = content.Load<Video>("Videos/fillGaps");
            injectAndGrowVid = content.Load<Video>("Videos/injectAndGrow");

            initializeStartScene();

        }

        private void initializeStartScene()
        {
            textScale = (float)Math.Sqrt((double)(widthScale*heightScale));// GameConstants.generalTextScaleFactor;
            //startText = "WHILE THE EXTINCT ANIMAL IS BEING RESURRECTED IN THE LAB, PLAY THE JIGSAW GAME TO RECREATE THIS IMAGE. YOU CAN ALSO CHOOSE TO SKIP IT. THE AI WILL DO IT FOR YOU, BUT THE ANIMAL WILL HAVE 60% EFFICIENCY";
            //startText = IngamePresentation.wrapLine(startText, desiredWidthOfImage-5, font, textScale);
            //startTextPosition = new Vector2(topLeftPosition.X+5, topLeftPosition.Y+desiredHeightOfImage/2);
            startButton = IngamePresentation.buttonNormalTexture;
            selectedStartButton = IngamePresentation.buttonHoverTexture;
            startButtonRect = new Rectangle( (int)(topLeftPosition.X + desiredWidthOfImage / 2 - startButton.Width / 2), (int)(topLeftPosition.Y + desiredHeightOfImage/2 - startButton.Height/2), startButton.Width, startButton.Height);
            //skipButtonRect = new Rectangle((int)(startTextPosition.X + desiredWidthOfImage*3/4 - startButton.Width / 2), (int)(startTextPosition.Y + font.MeasureString(startText).Y + (20*heightScale)), startButton.Width, startButton.Height);
            previewTime = 0;
        }

        public void StopVideoPlayers()
        {
            videoPlayer.Stop();
            dnaAnimationVideoPlayer.Stop();
        }

        public override void Show()
        {
            MediaPlayer.Stop();
            cursor.SetMenuCursorImage();
            timeUp = false;
            timeNow = (double)GameConstants.jigsawGameMaxTime;
            previewTime = 0;
            inOrder = false;
            letAIHandle = false;
            videoPlayBackState = VideoPlayBackState.ExtractingDNA;
            vidIndex = 0;
            gameStarted = gamePlayed = false;
            unShufflePieces();
            mouseOnPlayButton = false;
            base.Show();
        }

        public void setImageType(int imageType)
        {
            switch (imageType)
            {
                case 0:
                    image = seacowImage[random.Next(2)];
                    animalName = "STELLAR'S SEA COW";
                    generalInfoText = "";
                    generalInfoText += animalName+":\n ";
                    generalInfoText += "Description: Large herbivorous mammal with black thick skin,small head no teeth.\n ";
                    generalInfoText += "Extinct Since: ~1750.\n ";
                    generalInfoText += "Reason: Hunting for food and skin.";
                    break;
                case 1:
                    image = turtleImage[random.Next(2)];
                    animalName = "MEIOLANIA";
                    generalInfoText = "";
                    generalInfoText += animalName + ":\n ";
                    generalInfoText += "Description: Large turle with 2ft wide head and 2 long horns.\n ";
                    generalInfoText += "Extinct: >2,000yrs.\n ";
                    generalInfoText += "Reason: Excessive hunting.";
                    break;
                case 2:
                    image = dolphinImage[random.Next(2)];
                    animalName = "MAUI'S DOLPHIN";
                    generalInfoText = "";
                    generalInfoText += animalName + ":\n ";
                    generalInfoText += "Description: Smallest known species of dolphin. Lived near coasts.\n ";
                    generalInfoText += "Extinct Since: ~2050.\n ";
                    generalInfoText += "Reason of Extinction: Pollution, injury from nets, boats.";
                    break;
                default:
                    image = seacowImage[random.Next(2)];
                    generalInfoText = "";
                    animalName = "Steller's Sea Cow";
                    generalInfoText += animalName + "\n ";
                    generalInfoText += "DESCRIPTION: fat, heavy, and possibly ugly\n ";
                    generalInfoText += "EXTINCT SINCE: 1700 something\n ";
                    generalInfoText += "REASON: Ate too much!";
                    break;
            }
            generalInfoText = Poseidon.Core.IngamePresentation.wrapLine(generalInfoText, descriptionRectangle.Width-(tabSpace*2), font);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (MediaPlayer.State.Equals(MediaState.Stopped))
            {
                MediaPlayer.Play(PoseidonGame.audio.jigsawMusics[random.Next(GameConstants.NumJigsawBackgroundMusics)]);
            }

            if (dnaAnimationVideoPlayer.State == MediaState.Stopped)
                dnaAnimationVideoPlayer.Play(dnaAnimation);

            cursor.Update(graphicsDevice, PlayGameScene.gameCamera, gameTime, null);

            MouseState mouseState = Mouse.GetState();
            letAIHandleButtonHover = mouseOnLetAIHandle(mouseState);
            bool dumbValue = false;
            double dumbDoubleValue = 0;
            clicked = false;
            CursorManager.CheckClick(ref lastMouseState, ref currentMouseState, gameTime, ref dumbDoubleValue, ref clicked, ref dumbValue, ref dumbValue);

            if (!gameStarted)
            {
                if (startButtonRect.Intersects(new Rectangle(mouseState.X, mouseState.Y, 10, 10)))
                {
                    mouseOnPlayButton = true;
                    if (clicked)
                    {
                        gameStarted = true;
                        shufflePieces();
                        clicked = false;
                        PoseidonGame.audio.MenuScroll.Play();
                    }
                }
                else
                {
                    mouseOnPlayButton = false;
                }
                //else if (clicked && skipButtonRect.Intersects(new Rectangle(mouseState.X, mouseState.Y, 10, 10)))
                //{
                //    letAIHandle = true;
                //    clicked = false;
                //    return;
                //}
                previewTime += gameTime.ElapsedGameTime.TotalSeconds;
                if (previewTime < 10)
                {
                    videoPlayBackState = VideoPlayBackState.ExtractingDNA;
                    videoPlayer.Play(extractDNAVid);
                    stepText = "STEP 1: EXTRACT DNA FROM COLLECTED FRAGMENTS";
                }
                else if (previewTime < 20)
                {
                    videoPlayBackState = VideoPlayBackState.ReconstructingDNA;
                    videoPlayer.Play(reconstructDNAVid);
                    stepText = "STEP 2: ATTEMPTING TO RECONSTRUCT DNA SEQUENCE";
                }
                else if (previewTime < 30)
                {
                    videoPlayBackState = VideoPlayBackState.FillingGaps;
                    videoPlayer.Play(fillGapsVid);
                    stepText = "STEP 3: FILLING GAPS IN DNA WITH PREDICTON TECHNIQUES";
                }
                else
                {
                    videoPlayBackState = VideoPlayBackState.InjectAndGrow;
                    videoPlayer.Play(injectAndGrowVid);
                    stepText = "STEP 4: INJECT DNA INTO CELL AND GROW CELL";
                }
                stepText = Poseidon.Core.IngamePresentation.wrapLine(stepText, videoRectangle.Width - (tabSpace * 2), font);
                timerString = "RESURRECTING " + animalName;
            }
            else
            {
                timeNow -= gameTime.ElapsedGameTime.TotalSeconds;
                checkInOrder();
                if (GameConstants.jigsawGameMaxTime - timeNow <= 20)
                {
                    videoPlayBackState = VideoPlayBackState.ExtractingDNA;
                    videoPlayer.Play(extractDNAVid);
                    stepText = "STEP 1: EXTRACT DNA FROM COLLECTED FRAGMENTS";
                }
                else if (GameConstants.jigsawGameMaxTime - timeNow <= 40)
                {
                    videoPlayBackState = VideoPlayBackState.ReconstructingDNA;
                    videoPlayer.Play(reconstructDNAVid);
                    stepText = "STEP 2: ATTEMPTING TO RECONSTRUCT DNA SEQUENCE";
                }
                else if (GameConstants.jigsawGameMaxTime - timeNow <= 60)
                {
                    videoPlayBackState = VideoPlayBackState.FillingGaps;
                    videoPlayer.Play(fillGapsVid);
                    stepText = "STEP 3: FILLING GAPS IN DNA WITH PREDICTON TECHNIQUES";
                }
                else
                {
                    videoPlayBackState = VideoPlayBackState.InjectAndGrow;
                    videoPlayer.Play(injectAndGrowVid);
                    stepText = "STEP 4: INJECT DNA INTO CELL AND GROW CELL";
                }
                stepText = Poseidon.Core.IngamePresentation.wrapLine(stepText, videoRectangle.Width - (tabSpace * 2), font);
                if (timeNow <= 0)
                {
                    timeUp = true;
                }
                timerString = "RESURRECTING " + animalName + "\nTIME REMAINING: " + ((int)timeNow).ToString() + " Seconds";


                if (letAIHandleButtonHover && clicked)
                {
                    letAIHandle = true;
                    clicked = false;
                    PoseidonGame.audio.MenuScroll.Play();
                    return;
                }

                // If not sliding, then the player can control, otherwise he has to wait until the sliding finishes
                if (isSliding == false)
                {
                    float desiredWidthPerPiece;
                    float desiredHeightPerPiece;
                    getDesiredWidthAndHeightPerPiece(out desiredWidthPerPiece, out desiredHeightPerPiece);

                    // Get the cell the mouse is targeting
                    getRowColOfPiece(mouseState, desiredWidthPerPiece, desiredHeightPerPiece, out mouseRow, out mouseCol);

                    // If slidable
                    if (mouseState.LeftButton == ButtonState.Pressed && isSlidable(mouseRow, mouseCol))
                    {
                        isSliding = true;

                        moveBlocks(mouseRow, mouseCol);
                        if (!gamePlayed) gamePlayed = true;
                    }
                } // Slide otherwise 
                else moveBlocks(mouseRow, mouseCol);
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Store trueWidthOfPiece and trueHeightOfPiece in a vector2, Width first, Height the second
            Vector2 texelSize = calculateSizeOfPiece();

            //draw dna animation on the background
            Texture2D playingTexture;
            if (dnaAnimationVideoPlayer.State == MediaState.Playing)
            {
                playingTexture = dnaAnimationVideoPlayer.GetTexture();
                spriteBatch.Begin();
                spriteBatch.Draw(playingTexture, PoseidonGame.graphics.GraphicsDevice.Viewport.TitleSafeArea, Color.White);
                spriteBatch.End();
            }

            //spriteBatch.Begin();
            //// Draw dark background so it has the illusion of empty cell is drawn
            //spriteBatch.Draw(blackImage, new Rectangle((int)topLeftPosition.X, (int)topLeftPosition.Y, desiredWidthOfImage, desiredHeightOfImage), Color.Black);
            //spriteBatch.End();

            // Draw frame
            spriteBatch.Begin();
            spriteBatch.Draw(frameImg, new Rectangle((int)frameTopLeftPosition.X, (int)frameTopLeftPosition.Y, 
                game.Window.ClientBounds.Width / 2 - 20, game.Window.ClientBounds.Height - 2 * (int)frameTopLeftPosition.Y), Color.White);
            spriteBatch.End();
            drawAllPieces(new Vector2(), texelSize.X, texelSize.Y);

            if (!gameStarted)
            {
                //Color textColor;
                //if (image == turtleImage[0])
                //    textColor = Color.Yellow;
                //else
                //    textColor = Color.Black;
                spriteBatch.Begin();

                //spriteBatch.DrawString(font, startText, startTextPosition, textColor, 0f, new Vector2(0,0), textScale, SpriteEffects.None, 0f);
                if(mouseOnPlayButton)
                    spriteBatch.Draw(selectedStartButton, startButtonRect, Color.White);
                else
                    spriteBatch.Draw(startButton, startButtonRect, Color.White);
                spriteBatch.DrawString(timerFont, "PLAY", new Vector2(startButtonRect.Center.X, startButtonRect.Center.Y), Color.Yellow, 0f, new Vector2(timerFont.MeasureString("PLAY").X / 2, timerFont.MeasureString("PLAY").Y / 2), textScale, SpriteEffects.None, 0f);
                //spriteBatch.Draw(skipButton, skipButtonRect, Color.White);
                //spriteBatch.DrawString(timerFont, "SKIP", new Vector2(skipButtonRect.Center.X - timerFont.MeasureString("SKIP").X / 2, skipButtonRect.Center.Y - timerFont.MeasureString("SKIP").Y / 2), Color.Yellow, 0f, new Vector2(0,0), textScale, SpriteEffects.None, 0f );

                spriteBatch.End();
            }

            spriteBatch.Begin();
            spriteBatch.DrawString(timerFont, timerString, new Vector2(50,5), Color.White, 0, Vector2.Zero, GameConstants.generalTextScaleFactor, SpriteEffects.None, 0);
            letAIHandleButtonRectangle.Y = (int)(5 + timerFont.MeasureString(timerString).Y * GameConstants.generalTextScaleFactor );
            if (letAIHandleButtonHover) letAIHandleButtonTexture = IngamePresentation.letAIHandleHoverTexture;
            else letAIHandleButtonTexture = IngamePresentation.letAIHandleNormalTexture;
            spriteBatch.Draw(letAIHandleButtonTexture, letAIHandleButtonRectangle, Color.White);
            cursor.Draw(gameTime);
            spriteBatch.End();

            //draw the small frame containing animated DNA
            //and the step
            spriteBatch.Begin();
            //spriteBatch.Draw(smallFrameImg, new Rectangle(20, 100, 512, 512), Color.White);
            spriteBatch.Draw(smallFrameImg, videoRectangle, Color.White);
            //spriteBatch.DrawString(font, stepText, new Vector2(70, 200), new Color(new Vector3(0, 1 ,0)));
            int topSpace = (int)(game.Window.ClientBounds.Height*0.063); //50
            spriteBatch.DrawString(font, stepText, new Vector2( videoRectangle.Left + tabSpace, videoRectangle.Top + topSpace) , new Color(new Vector3(0, 1, 0)));
            spriteBatch.End();

            //draw the videos
            if (videoPlayer.State == MediaState.Playing)
            {
                playingTexture = videoPlayer.GetTexture();
                spriteBatch.Begin();
                //spriteBatch.Draw(playingTexture, new Vector2(110, 300), Color.White);
                int textSpace = (int)(font.MeasureString(stepText).Y)+topSpace+10;
                spriteBatch.Draw(playingTexture, new Vector2(videoRectangle.Center.X - playingTexture.Width / 2, videoRectangle.Top + textSpace + topSpace), Color.White);
                spriteBatch.End();
            }

            //draw the small frame containing general info
            spriteBatch.Begin();
            //spriteBatch.Draw(smallFrameImg, new Rectangle(20, Game.Window.ClientBounds.Bottom - (int)font.MeasureString(generalInfoText).Y - 100, game.Window.ClientBounds.Width / 2 - 100, (int)font.MeasureString(generalInfoText).Y + 100), Color.White);
            spriteBatch.Draw(smallFrameImg, descriptionRectangle, Color.White);
            spriteBatch.End();

            //draw general info about the animal
            spriteBatch.Begin();
            //spriteBatch.DrawString(font, generalInfoText, new Vector2(70, Game.Window.ClientBounds.Bottom -  font.MeasureString(generalInfoText).Y - 70), Color.Yellow);
            spriteBatch.DrawString(font, generalInfoText, new Vector2(descriptionRectangle.Left+tabSpace, descriptionRectangle.Center.Y - font.MeasureString(generalInfoText).Y / 2), Color.Yellow);
            cursor.Draw(gameTime);
            spriteBatch.End();

            //base.Draw(gameTime);
        }

        // Slide control
        private void moveBlocks(int row, int col)
        {
            int directionX = 0, directionY = 0;
            float distance = 0;
            float desiredWidthPerPiece;
            float desiredHeightPerPiece;
            getDesiredWidthAndHeightPerPiece(out desiredWidthPerPiece, out desiredHeightPerPiece);

            // Assign the direction
            if (row == emptyCellRow)
            {
                if (col > emptyCellCol) directionX = -1;
                else directionX = 1;

                distance = desiredWidthPerPiece;
            }
            if (col == emptyCellCol)
            {
                if (row > emptyCellRow) directionY = -1;
                else directionY = 1;

                distance = desiredHeightPerPiece;
            }
            // If return false (unable to move forward) then reset distanceTraveled and stop moving
            if (moveBlocksOneUnit(distance, row, col, directionX, directionY) == false)
            {
                isSliding = false;
                distanceTraveled = 0;

                // Bring blocks to its correct order
                shiftBlocks(row, col);
            }
        }

        // Shift block to the empty cell
        private void shiftBlocks(int lastPieceRow, int lastPieceCol)
        {
            int dirRow = (emptyCellRow == lastPieceRow) ? 0 : (lastPieceRow - emptyCellRow) / Math.Abs(emptyCellRow - lastPieceRow),
                dirCol = (emptyCellCol == lastPieceCol) ? 0 : (lastPieceCol - emptyCellCol) / Math.Abs(emptyCellCol - lastPieceCol);

            for (int i = emptyCellRow, j = emptyCellCol;
                 i != lastPieceRow || j != lastPieceCol;
                 i += dirRow, j += dirCol)
            {
                swapCell(i, j, i + dirRow, j + dirCol);
            }
        }

        // Move block by an unit distance
        // Return if the pieces can still be moved
        private bool moveBlocksOneUnit(float distance, int clickedRow, int clickedCol, int directionX, int directionY)
        {
            float desiredWidthPerPiece;
            float desiredHeightPerPiece;
            getDesiredWidthAndHeightPerPiece(out desiredWidthPerPiece, out desiredHeightPerPiece);

            if (distanceTraveled < distance)
            {
                // Move the blocks by a distance calculated in 1 Update
                int advanceRow = directionY, advanceCol = directionX;
                // Move all the blocks on the way to the empty cell
                // Note that advanceRow and advanceCol can't be the same value. If one has the value 1/-1, the other has the value 0
                for (int i = clickedRow, j = clickedCol;
                    i < numberOfRow && j < numberOfCol && ( i != emptyCellRow || j != emptyCellCol);
                    i += advanceRow, j += advanceCol)
                {

                    jigsawPieces[i][j].topLeftPosition.X += directionX * (desiredWidthPerPiece / numberOfUpdates);
                    jigsawPieces[i][j].topLeftPosition.Y += directionY * (desiredHeightPerPiece / numberOfUpdates);
                }

                // Advance the distanceTraveled
                // If slide horizontally
                if (directionX == 1 || directionX == -1)
                {
                    distanceTraveled += desiredWidthPerPiece / numberOfUpdates;
                } // If slide vertically
                else if (directionY == 1 || directionY == -1)
                {
                    distanceTraveled += desiredHeightPerPiece / numberOfUpdates;
                }
                return true;
            }
            return false;
        }

        // Swap the empty cell with the cell (rowFrom, colFrom)
        private void swapCell(int rowFrom, int colFrom, int rowTo, int colTo)
        {
            if (rowTo >= numberOfRow || colTo >= numberOfCol)
                return;
            float desiredWidthPerPiece;
            float desiredHeightPerPiece;
            getDesiredWidthAndHeightPerPiece(out desiredWidthPerPiece, out desiredHeightPerPiece);

            // Are we swapping the empty cell
            if (isEmptyCell(rowFrom, colFrom))
            {
                Vector2 tmpPos = getTopLeft_XY_OfPiece(rowFrom, colFrom, desiredWidthPerPiece, desiredHeightPerPiece);

                jigsawPieces[rowFrom][colFrom] = jigsawPieces[rowTo][colTo];
                jigsawPieces[rowFrom][colFrom].topLeftPosition = tmpPos;
                jigsawPieces[rowTo][colTo] = null;

                emptyCellRow = rowTo;
                emptyCellCol = colTo;
            }
            else if (isEmptyCell(rowTo, colTo))
            {
                Vector2 tmpPos = getTopLeft_XY_OfPiece(rowTo, colTo, desiredWidthPerPiece, desiredHeightPerPiece);

                jigsawPieces[rowTo][colTo] = jigsawPieces[rowFrom][colFrom];
                jigsawPieces[rowTo][colTo].topLeftPosition = tmpPos;
                jigsawPieces[rowFrom][colFrom] = null;

                emptyCellRow = rowFrom;
                emptyCellCol = colFrom;
            }
            else
            {
                // Swap the true row, col coordinate of the 2 cells
                int tmpRow = jigsawPieces[rowFrom][colFrom].trueRow, tmpCol = jigsawPieces[rowFrom][colFrom].trueCol;
                jigsawPieces[rowFrom][colFrom].trueRow = jigsawPieces[rowTo][colTo].trueRow;
                jigsawPieces[rowFrom][colFrom].trueCol = jigsawPieces[rowTo][colTo].trueCol;
                jigsawPieces[rowTo][colTo].trueRow = tmpRow;
                jigsawPieces[rowTo][colTo].trueCol = tmpCol;
            }
        }

        // Given the mouseState, determine which piece the player is targeting
        private void getRowColOfPiece(MouseState mouseState, float desiredWidthPerPiece,
            float desiredHeightPerPiece, out int row, out int col)
        {

            float x = mouseState.X, y = mouseState.Y;

            col = (int)((x - topLeftPosition.X) / desiredWidthPerPiece);
            row = (int)((y - topLeftPosition.Y) / desiredHeightPerPiece);
        }

        // Get information
        private void checkInOrder()
        {
            int row, col;
            MouseState mouseState = Mouse.GetState();

            Vector2 desiredSizeOfPiece = new Vector2(desiredWidthOfImage / numberOfCol, desiredHeightOfImage / numberOfRow);
            getRowColOfPiece(mouseState, desiredSizeOfPiece.X, desiredSizeOfPiece.Y, out row, out col);

            //debugString = "Coordinate of the piece is (rowFrom, colFrom): (" + row + ", " + col + ")\n";
            //debugString += "Corrdinate of empty piece is (rowFrom, colFrom): (" + emptyCellRow + ", " + emptyCellCol + ")\n";
            //debugString += Mouse.GetState().ToString();
            //debugString += "\nSlidability: " + isSlidable(row, col);

            inOrder = true;
            for (int i = 0; i < numberOfRow; i++)
            {
                for (int j = 0; j < numberOfCol; j++)
                {
                    if (!isEmptyCell(i, j) && (jigsawPieces[i][j].trueRow != i || jigsawPieces[i][j].trueCol != j))
                    {
                        inOrder = false;
                    }
                }
            }
            //if (inOrder) debugString += "\nWinner\n";
            //else debugString += "\nLoser\n";

        }

        // Display all the pieces by drawing piece by pieces, rowFrom-wise
        private void drawAllPieces(Vector2 originOfTopLeftPiece,
            float trueWidthOfPiece, float trueHeightOfPiece)
        {

            // Get the trueWidthOfPiece and trueHeightOfPiece of each pieces
            float desiredWidthPerPiece;
            float desiredHeightPerPiece;
            getDesiredWidthAndHeightPerPiece(out desiredWidthPerPiece, out desiredHeightPerPiece);

            // Draw each pieces, recalculate the position of the next-to-draw piece
            for (int i = 0; i < numberOfRow; i++)
            {
                for (int j = 0; j < numberOfCol; j++)
                {
                    // Only draw non-null pieces
                    if (!isEmptyCell(i, j))
                    {
                        jigsawPieces[i][j].draw(spriteBatch, image, originOfTopLeftPiece,
                            desiredWidthPerPiece, desiredHeightPerPiece, trueWidthOfPiece, trueHeightOfPiece);
                    }
                }
            }
        }

        // Get the size of pieces
        private Vector2 calculateSizeOfPiece()
        {
            return new Vector2(image.Width / numberOfCol, image.Height / numberOfRow);
        }

        // Scaling the image
        // return the scaling factor for x and y
        private static Vector2 calculateScalingFactor(Texture2D image, float frameWidth, float frameHeight)
        {
            float scaleX = frameWidth / image.Width;
            float scaleY = frameHeight / image.Height;

            return new Vector2(scaleX, scaleY);
        }

        // Assign the size of puzzle
        private void initializeSizeOfPuzzle(int piecesInRow, int piecesInCol)
        {
            numberOfRow = piecesInCol;
            numberOfCol = piecesInRow;
        }

        // Initialize all the pieces
        private void loadPieces()
        {
            jigsawPieces = new Piece[numberOfRow][];

            float desiredWidthPerPiece;
            float desiredHeightPerPiece;
            getDesiredWidthAndHeightPerPiece(out desiredWidthPerPiece, out desiredHeightPerPiece);

            Vector2 position = new Vector2(topLeftPosition.X, topLeftPosition.Y);

            for (int i = 0; i < numberOfRow; i++)
            {
                jigsawPieces[i] = new Piece[numberOfCol];
                for (int j = 0; j < numberOfCol; j++)
                {
                    // Temporarily assign the empty cell to be (0, 0)
                    if (i == 0 && j == 0)
                    {
                        jigsawPieces[i][j] = null;
                    }
                    else
                    {
                        jigsawPieces[i][j] = new Piece();
                        jigsawPieces[i][j].initialize(i, j, position.X, position.Y);
                    }
                    // next piece in a rowFrom
                    position.X += desiredWidthPerPiece;
                }
                // New rowFrom
                position.Y += desiredHeightPerPiece;
                position.X = topLeftPosition.X;
            }
        }

        // Initialize the empty cells
        private void initializeEmptyCell(int row, int col)
        {
            emptyCellRow = row;
            emptyCellCol = col;
        }

        // Check empty cell
        private bool isEmptyCell(int row, int col)
        {
            return row == emptyCellRow && col == emptyCellCol;
        }

        // Shuffle Pieces. Only apply to jigsawPieces, trueOrderOfPieces stays the same
        public void shufflePieces()
        {
            Random rnd = new Random();
            int dirRow, dirCol;
            for (int i = 0; i < SHUFFLING_STEP; i++)
            {
                do
                {
                    dirRow = rnd.Next(-1, 2);
                    dirCol = rnd.Next(-1, 2);
                } while (emptyCellRow + dirRow < 0 || emptyCellRow + dirRow >= numberOfRow || emptyCellCol + dirCol >= numberOfCol || emptyCellCol + dirCol < 0 ||
                    Math.Abs(dirCol) == Math.Abs(dirRow));

                swapCell(emptyCellRow, emptyCellCol, emptyCellRow + dirRow, emptyCellCol + dirCol);
            }
        }

        //Unshuffle Pieces
        public void unShufflePieces()
        {
            int truerow, truecol;
            for (int i = 0; i < numberOfRow; i++)
            {
                for (int j = 0; j < numberOfCol; j++)
                {
                    if(isEmptyCell(i,j))
                        continue;
                    truerow = jigsawPieces[i][j].trueRow;
                    truecol = jigsawPieces[i][j].trueCol;
                    while ( (!isEmptyCell(i, j)) && (i != truerow || j != truecol) )
                    {
                        truerow = jigsawPieces[i][j].trueRow;
                        truecol = jigsawPieces[i][j].trueCol;
                        swapCell(i, j, truerow, truecol);
                    }
                }
            }
        }

        // Re-assign the topLeftPosition order of pieces
        private void reAssignOrder()
        {
            float desiredWidthPerPiece;
            float desiredHeightPerPiece;
            getDesiredWidthAndHeightPerPiece(out desiredWidthPerPiece, out desiredHeightPerPiece);

            // Re-assign the position
            Vector2 position = new Vector2(topLeftPosition.X, topLeftPosition.Y);
            for (int i = 0; i < numberOfRow; i++)
            {
                for (int j = 0; j < numberOfCol; j++)
                {
                    // Temporarily assign the empty cell to be (0, 0)
                    if (!isEmptyCell(i, j))
                    {
                        jigsawPieces[i][j].topLeftPosition = new Vector2(position.X, position.Y);
                    }
                    // next piece in a rowFrom
                    position.X += desiredWidthPerPiece;
                }
                // New rowFrom
                position.Y += desiredHeightPerPiece;
                position.X = topLeftPosition.X;
            }
        }

        // Is slidable?
        // Slidable if rowFrom = emptyRow xor colFrom == emptyCol
        private bool isSlidable(int pieceRow, int pieceCol)
        {
            if (pieceRow < 0 || pieceCol < 0) return false;
            else return pieceRow == emptyCellRow ^ pieceCol == emptyCellCol;
        }

        // Get top left X, Y position of piece
        private Vector2 getTopLeft_XY_OfPiece(int row, int col, float desiredWidthPerPiece, float desiredHeightPerPiece)
        {
            return new Vector2(topLeftPosition.X + col * desiredWidthPerPiece, topLeftPosition.Y + row * desiredHeightPerPiece);
        }

        // Get the trueWidthOfPiece and trueHeightOfPiece of each pieces
        private void getDesiredWidthAndHeightPerPiece(out float desiredWidthPerPiece, out float desiredHeightPerPiece)
        {
            desiredWidthPerPiece = desiredWidthOfImage / numberOfCol;
            desiredHeightPerPiece = desiredHeightOfImage / numberOfRow;
        }

        public bool mouseOnLetAIHandle(MouseState lmouseState)
        {
            if (letAIHandleButtonRectangle.Contains(lmouseState.X, lmouseState.Y))
                return true;
            else
                return false;
        }
    }
}