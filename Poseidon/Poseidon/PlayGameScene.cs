using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Poseidon.Core;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace Poseidon
{
    /// <summary>
    /// This is a game component that implements the Action Scene.
    /// </summary>
    public partial class PlayGameScene : GameScene
    {
        GraphicsDeviceManager graphics;
        GraphicsDevice GraphicDevice;
        ContentManager Content;
        KeyboardState lastKeyboardState = new KeyboardState();
        KeyboardState currentKeyboardState = new KeyboardState();
        GamePadState lastGamePadState = new GamePadState();
        GamePadState currentGamePadState = new GamePadState();
        private AudioLibrary audio;
        int retrievedFuelCells;
        TimeSpan startTime, roundTimer, roundTime;
        Random random;
        SpriteBatch spriteBatch;
        SpriteFont statsFont;
        GameObject ground;
        Camera gameCamera;
        GameState currentGameState = GameState.Running;
        GameObject boundingSphere;

        FuelCarrier fuelCarrier;
        List<FuelCell> fuelCells;
        List<Barrier> barriers;
        List<Projectiles> projectiles;

        Enemy[] enemies;
        Fish[] fish;
        

        //A tank
        public Tank tank;

        private TimeSpan fireTime;
        private TimeSpan prevFireTime;

        // Game is paused?
        protected bool paused;
        protected Vector2 pausePosition;
        protected Rectangle pauseRect = new Rectangle(1, 120, 200, 44);
        protected Texture2D actionTexture;

        Game game;
        public PlayGameScene(Game game, GraphicsDeviceManager graphics, ContentManager Content, GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch, Vector2 pausePosition, Rectangle pauseRect, Texture2D actionTexture):base(game)
        {
            this.graphics = graphics;
            this.Content = Content;
            this.GraphicDevice = GraphicsDevice;
            this.spriteBatch = spriteBatch;
            this.pausePosition = pausePosition;
            this.pauseRect = pauseRect;
            this.actionTexture = actionTexture;
            this.game = game;
            roundTime = GameConstants.RoundTime;
            random = new Random();
            ground = new GameObject();
            gameCamera = new Camera();
            boundingSphere = new GameObject();
            tank = new Tank();
            fireTime = TimeSpan.FromSeconds(0.3f);
            enemies = new Enemy[GameConstants.NumberEnemies];
            fish = new Fish[GameConstants.NumberFish];
            
            this.Load();

        }

        public void Load()
        {
            statsFont = Content.Load<SpriteFont>("Fonts/StatsFont");
            // Get the audio library
            audio = (AudioLibrary)
                Game.Services.GetService(typeof(AudioLibrary));

            ground.Model = Content.Load<Model>("Models/ground");
            boundingSphere.Model = Content.Load<Model>("Models/sphere1uR");

            
            //Initialize fuel cells
            fuelCells = new List<FuelCell> (GameConstants.NumFuelCells);
            int powerType = random.Next(3) + 1;
            for (int index = 0; index < fuelCells.Count; index++)
            {
                fuelCells[index] = new FuelCell(powerType);
                fuelCells[index].LoadContent(Content, "Models/fuelcell");
                powerType = random.Next(3) + 1;
            }

            //Initialize barriers
            barriers = new List<Barrier> (GameConstants.NumBarriers);
            int randomBarrier = random.Next(3);
            string barrierName = null;

            for (int index = 0; index < GameConstants.NumBarriers; index++)
            {
                switch (randomBarrier)
                {
                    case 0:
                        barrierName = "Models/cube10uR";
                        //barrierName = "Models/sphere1uR";
                        break;
                    case 1:
                        barrierName = "Models/cylinder10uR";
                        break;
                    case 2:
                        barrierName = "Models/pyramid10uR";
                        break;
                }
                barriers.Add(new Barrier());
                barriers[index].LoadContent(Content, barrierName);
                randomBarrier = random.Next(3);
            }
            PlaceFuelCellsAndBarriers();

            //Initialize fuel carrier
            fuelCarrier = new FuelCarrier();
            fuelCarrier.LoadContent(Content, "Models/fuelcarrier");

            projectiles = new List<Projectiles>();

            tank.Load(Content);
            roundTimer = roundTime;
        }

        /// <summary>
        /// Show the action scene
        /// </summary>
        public override void Show()
        {
            paused = false;
            MediaPlayer.Play(audio.BackMusic);
            //PlaceFuelCellsAndBarriers();
            base.Show();
        }
        private void ResetGame(GameTime gameTime, float aspectRatio)
        {
            tank.Reset();
            gameCamera.Update(tank.ForwardDirection,
                tank.Position, aspectRatio);
            InitializeGameField(Content);

            retrievedFuelCells = 0;
            startTime = gameTime.TotalGameTime;
            roundTimer = roundTime;
            currentGameState = GameState.Running;
        }

        private void InitializeGameField(ContentManager Content)
        {
            //Initialize barriers
            barriers = new List<Barrier> (GameConstants.NumBarriers);
            int randomBarrier = random.Next(3);
            string barrierName = null;

            for (int index = 0; index < GameConstants.NumBarriers; index++)
            {
                switch (randomBarrier)
                {
                    case 0:
                        barrierName = "Models/cube10uR";
                        //barrierName = "Models/sphere1uR";
                        break;
                    case 1:
                        barrierName = "Models/cylinder10uR";
                        break;
                    case 2:
                        barrierName = "Models/pyramid10uR";
                        break;
                }
                barriers.Add(new Barrier());
                barriers[index].LoadContent(Content, barrierName);
                randomBarrier = random.Next(3);
            }
            PlaceFuelCellsAndBarriers();
        }
        /// <summary>
        /// Hide the scene
        /// </summary>
        public override void Hide()
        {
            // Stop the background music
            MediaPlayer.Stop();
            // Stop the rumble
            //rumblePad.Stop(PlayerIndex.One);
            //rumblePad.Stop(PlayerIndex.Two);

            base.Hide();
        }
        /// <summary>
        /// Paused mode
        /// </summary>
        public bool Paused
        {
            get { return paused; }
            set
            {
                paused = value;
                if (paused)
                {
                    MediaPlayer.Pause();
                }
                else
                {
                    MediaPlayer.Resume();
                }
            }
        }
        public override void Update(GameTime gameTime)
        {
            if (!paused)
            {
                float aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
                lastKeyboardState = currentKeyboardState;
                currentKeyboardState = Keyboard.GetState();
                lastGamePadState = currentGamePadState;
                currentGamePadState = GamePad.GetState(PlayerIndex.One);

                // Allows the game to exit
                //if ((currentKeyboardState.IsKeyDown(Keys.Escape)) ||
                //    (currentGamePadState.Buttons.Back == ButtonState.Pressed))
                //    //this.Exit();



                if ((currentGameState == GameState.Running))
                {
                    //fuelCarrier.Update(currentGamePadState, 
                    //    currentKeyboardState, barriers);

                    // Are we shooting?
                    if (currentKeyboardState.IsKeyDown(Keys.L)
                        && gameTime.TotalGameTime.TotalSeconds - prevFireTime.TotalSeconds > fireTime.TotalSeconds / (tank.shootingRate * tank.fireRateUp))
                    {
                        prevFireTime = gameTime.TotalGameTime;
                        audio.Shooting.Play();
                        placeBullet();
                    }

                    tank.Update(currentKeyboardState, barriers, fuelCells, gameTime);
                    gameCamera.Update(tank.ForwardDirection,
                        tank.Position, aspectRatio);
<<<<<<< HEAD
                    
                    // Update barrier (enemies)
                    for (int i = 0; i < barriers.Count; i++) {
                        barriers[i].Update(barriers, random.Next(100), tank);
                    }
=======
                    for (int barrier_index = 0; barrier_index < GameConstants.NumBarriers; barrier_index++)
                    {
                        barrier_previous_movement[barrier_index] =
                            barriers[barrier_index].Update(barriers, random.Next(100), barrier_previous_movement[barrier_index], tank);
                    }
                    retrievedFuelCells = 0;
                    foreach (FuelCell fuelCell in fuelCells)
                    {
                        fuelCell.Update(currentKeyboardState, tank.BoundingSphere, tank.Trash_Fruit_BoundingSphere);
                        if (fuelCell.Retrieved)
                        {
                            retrievedFuelCells++;
                        }
                    }

                    for (int i = 0; i < projectiles.Count; i++)
                    {
                        projectiles[i].update(barriers);
                    }
                    Collision.updateBulletOutOfBound(projectiles, GraphicDevice.Viewport);
                    Collision.updateBulletVsBarriersCollision(projectiles, barriers);
>>>>>>> c3422d0c4c4a9526fb20ee89b61d04f5372c4cdb

                    retrievedFuelCells = 0;
                    foreach (FuelCell fuelCell in fuelCells)
                    {
                        fuelCell.Update(currentKeyboardState, tank.BoundingSphere, tank.Trash_Fruit_BoundingSphere);
                        if (fuelCell.Retrieved)
                        {
                            retrievedFuelCells++;
                        }
                    }

<<<<<<< HEAD
                    for (int i = 0; i < projectiles.Count; i++)
                    {
                        projectiles[i].update(barriers);
                    }
                    Collision.updateBulletOutOfBound(projectiles, GraphicDevice.Viewport);
                    Collision.updateBulletVsBarriersCollision(projectiles, barriers);


=======
>>>>>>> c3422d0c4c4a9526fb20ee89b61d04f5372c4cdb
                    if (retrievedFuelCells == GameConstants.NumFuelCells)
                    {
                        currentGameState = GameState.Won;
                    }
                    roundTimer -= gameTime.ElapsedGameTime;
                    if ((roundTimer < TimeSpan.Zero) &&
                        (retrievedFuelCells != GameConstants.NumFuelCells))
                    {
                        currentGameState = GameState.Lost;
                    }
                }

                if ((currentGameState == GameState.Won) ||
                    (currentGameState == GameState.Lost))
                {
                    // Reset the world for a new game
                    if ((lastKeyboardState.IsKeyDown(Keys.Enter) &&
                        (currentKeyboardState.IsKeyUp(Keys.Enter))) ||
                        currentGamePadState.Buttons.Start == ButtonState.Pressed)
                    {
                        ResetGame(gameTime, aspectRatio);

                    }
                }
                base.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicDevice.BlendState = BlendState.Opaque;
            GraphicDevice.DepthStencilState = DepthStencilState.Default;
            //GraphicDevice.SamplerStates[0] = SamplerState.LinearWrap;
 
            base.Draw(gameTime);
            if (paused)
            {
                // Draw the "pause" text
                spriteBatch.Draw(actionTexture, pausePosition, pauseRect,
                    Color.White);
            }
            switch (currentGameState)
            {

                case GameState.Running:
                    DrawGameplayScreen();
                    break;
                case GameState.Won:
                    DrawWinOrLossScreen(GameConstants.StrGameWon);
                    break;
                case GameState.Lost:
                    DrawWinOrLossScreen(GameConstants.StrGameLost);
                    break;
            };
            
        }
        /// <summary>
        /// Draws the game terrain, a simple blue grid.
        /// </summary>
        /// <param name="model">Model representing the game playing field.</param>
        private void DrawTerrain(Model model)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = Matrix.Identity;

                    // Use the matrices provided by the game camera
                    effect.View = gameCamera.ViewMatrix;
                    effect.Projection = gameCamera.ProjectionMatrix;
                }
                mesh.Draw();
            }
        }

       

        private void DrawWinOrLossScreen(string gameResult)
        {
            float xOffsetText, yOffsetText;
            Vector2 viewportSize = new Vector2(GraphicDevice.Viewport.Width,
                GraphicDevice.Viewport.Height);
            Vector2 strCenter;

            xOffsetText = yOffsetText = 0;
            Vector2 strResult = statsFont.MeasureString(gameResult);
            Vector2 strPlayAgainSize =
                statsFont.MeasureString(GameConstants.StrPlayAgain);
            Vector2 strPosition;
            strCenter = new Vector2(strResult.X / 2, strResult.Y / 2);

            yOffsetText = (viewportSize.Y / 2 - strCenter.Y);
            xOffsetText = (viewportSize.X / 2 - strCenter.X);
            strPosition = new Vector2((int)xOffsetText, (int)yOffsetText);

            //spriteBatch.Begin();
            spriteBatch.DrawString(statsFont, gameResult,
                strPosition, Color.Red);

            strCenter =
                new Vector2(strPlayAgainSize.X / 2, strPlayAgainSize.Y / 2);
            yOffsetText = (viewportSize.Y / 2 - strCenter.Y) +
                (float)statsFont.LineSpacing;
            xOffsetText = (viewportSize.X / 2 - strCenter.X);
            strPosition = new Vector2((int)xOffsetText, (int)yOffsetText);
            spriteBatch.DrawString(statsFont, GameConstants.StrPlayAgain,
                strPosition, Color.AntiqueWhite);

            //spriteBatch.End();

            //re-enable depth buffer after sprite batch disablement

            //GraphicsDevice.DepthStencilState.DepthBufferEnable = true;
            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            GraphicDevice.DepthStencilState = dss;

            //GraphicsDevice.RenderState.AlphaBlendEnable = false;
            //GraphicsDevice.RenderState.AlphaTestEnable = false;

            //GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
            //GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;
        }

        private void DrawGameplayScreen()
        {
            DrawTerrain(ground.Model);
            foreach (FuelCell fuelCell in fuelCells)
            {
                if (!fuelCell.Retrieved)
                {
                    fuelCell.Draw(gameCamera.ViewMatrix,
                        gameCamera.ProjectionMatrix);
                    //RasterizerState rs = new RasterizerState();
                    //rs.FillMode = FillMode.WireFrame;
                    //GraphicDevice.RasterizerState = rs;
                    //fuelCell.DrawBoundingSphere(gameCamera.ViewMatrix,
                    //    gameCamera.ProjectionMatrix, boundingSphere);

                    //rs = new RasterizerState();
                    //rs.FillMode = FillMode.Solid;
                    //GraphicDevice.RasterizerState = rs;
                }
            }
            foreach (Barrier barrier in barriers)
            {
                barrier.Draw(gameCamera.ViewMatrix,
                    gameCamera.ProjectionMatrix);
                //RasterizerState rs = new RasterizerState();
                //rs.FillMode = FillMode.WireFrame;
                //GraphicsDevice.RasterizerState = rs;
                //barrier.DrawBoundingSphere(gameCamera.ViewMatrix,
                //    gameCamera.ProjectionMatrix, boundingSphere);

                //rs = new RasterizerState();
                //rs.FillMode = FillMode.Solid;
                //GraphicsDevice.RasterizerState = rs;
            }

            // Update bullets
            foreach (Projectiles p in projectiles)
            {
                if (p.getStatus())
                {
                    p.draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
                }
            }

            //fuelCarrier.Draw(gameCamera.ViewMatrix, 
            //    gameCamera.ProjectionMatrix);
            tank.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
            //RasterizerState rs = new RasterizerState();
            //rs.FillMode = FillMode.WireFrame;
            //GraphicsDevice.RasterizerState = rs;
            //tank.DrawBoundingSphere(gameCamera.ViewMatrix,
            //    gameCamera.ProjectionMatrix, boundingSphere);

            //rs = new RasterizerState();
            //rs.FillMode = FillMode.Solid;
            //GraphicsDevice.RasterizerState = rs;
            DrawStats();
        }

        private void DrawStats()
        {
            float xOffsetText, yOffsetText;
            string str1 = GameConstants.StrTimeRemaining;
            string str2 = "" + barriers.Count;
            Rectangle rectSafeArea;

            str1 += (roundTimer.Seconds).ToString();

            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            xOffsetText = rectSafeArea.X;
            yOffsetText = rectSafeArea.Y;

            Vector2 strSize = statsFont.MeasureString(str1);
            Vector2 strPosition =
                new Vector2((int)xOffsetText + 10, (int)yOffsetText);

            //spriteBatch.Begin();
            spriteBatch.DrawString(statsFont, str1, strPosition, Color.White);
            strPosition.Y += strSize.Y;
            spriteBatch.DrawString(statsFont, str2, strPosition, Color.White);
            //spriteBatch.End();

            //re-enable depth buffer after sprite batch disablement

            //GraphicsDevice.DepthStencilState.DepthBufferEnable = true;
            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            GraphicDevice.DepthStencilState = dss;

            //GraphicsDevice.RenderState.AlphaBlendEnable = false;
            //GraphicsDevice.RenderState.AlphaTestEnable = false;

            //GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
            //GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;
        }
    }
}
