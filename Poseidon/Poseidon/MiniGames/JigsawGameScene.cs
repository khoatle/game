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
        private Texture2D image, seacowImage, turtleImage, dolphinImage;
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

        private double timeNow;
        public bool timeUp;

        Random random;

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

            topLeftPosition = new Vector2(game.Window.ClientBounds.Width / 2, 0);

            desiredWidthOfImage = game.Window.ClientBounds.Width / 2;
            desiredHeightOfImage = game.Window.ClientBounds.Height;

            isSliding = false;
            distanceTraveled = 0;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            font = content.Load<SpriteFont>("Fonts/JigsawFont");
            timerFont = content.Load<SpriteFont>("Fonts/menuSmall");

            seacowImage = content.Load<Texture2D>("Image/MinigameTextures/seaCow");
            turtleImage = content.Load<Texture2D>("Image/MinigameTextures/seaTurtle");
            dolphinImage = content.Load<Texture2D>("Image/MinigameTextures/seaDolphin");
            
            blackImage = content.Load<Texture2D>("Image/MinigameTextures/BlackBox");

            // Load/Initialize pieces here
            loadPieces();
            // Shuffle pieces
            shufflePieces();
        }

        public override void Show()
        {
            MediaPlayer.Stop();
            timeUp = false;
            timeNow = (double)GameConstants.jigsawGameMaxTime;
            inOrder = false;
            shufflePieces();
            base.Show();
        }

        public void setImageType(int imageType)
        {
            switch (imageType)
            {
                case 0:
                    image = seacowImage;
                    break;
                case 1:
                    image = turtleImage;
                    break;
                case 2:
                    image = dolphinImage;
                    break;
                default:
                    image = seacowImage;
                    break;
            }
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
            timeNow -= gameTime.ElapsedGameTime.TotalSeconds;
            if (timeNow <= 0)
            {
                timeUp = true;
            }
            timerString = "TIME REMAINING: "+ ((int)timeNow).ToString() + " Seconds";
            
            cursor.Update(graphicsDevice, PlayGameScene.gameCamera , gameTime, null);
   
            putInfoInDebuggingString();

            MouseState mouseState = Mouse.GetState();

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

            spriteBatch.Begin();
            // Draw dark background so it has the illusion of empty cell is drawn
            spriteBatch.Draw(blackImage, new Rectangle((int)topLeftPosition.X, (int)topLeftPosition.Y, desiredWidthOfImage, desiredHeightOfImage), Color.Black);
            spriteBatch.End();

            drawAllPieces(new Vector2(), texelSize.X, texelSize.Y);

            spriteBatch.Begin();
            spriteBatch.DrawString(timerFont, timerString, new Vector2(), Color.DarkRed);
            spriteBatch.DrawString(font, debugString, new Vector2(0,200), Color.Black);
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
        private void putInfoInDebuggingString()
        {
            int row, col;
            MouseState mouseState = Mouse.GetState();

            Vector2 desiredSizeOfPiece = new Vector2(desiredWidthOfImage / numberOfCol, desiredHeightOfImage / numberOfRow);
            getRowColOfPiece(mouseState, desiredSizeOfPiece.X, desiredSizeOfPiece.Y, out row, out col);

            debugString = "Coordinate of the piece is (rowFrom, colFrom): (" + row + ", " + col + ")\n";
            debugString += "Corrdinate of empty piece is (rowFrom, colFrom): (" + emptyCellRow + ", " + emptyCellCol + ")\n";
            debugString += Mouse.GetState().ToString();
            debugString += "\nSlidability: " + isSlidable(row, col);

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
            if (inOrder) debugString += "\nWinner\n";
            else debugString += "\nLoser\n";

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
            return pieceRow == emptyCellRow ^ pieceCol == emptyCellCol;
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
    }
}