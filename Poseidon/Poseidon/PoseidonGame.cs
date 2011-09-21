// Copyright (C) Microsoft Corporation. All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
//using Poseidon.Core;
namespace Poseidon
{
    public enum GameState { Loading, Running, Won, Lost }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public partial class PoseidonGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        KeyboardState lastKeyboardState = new KeyboardState();
        KeyboardState currentKeyboardState = new KeyboardState();
        GamePadState lastGamePadState = new GamePadState();
        GamePadState currentGamePadState = new GamePadState();
        int retrievedFuelCells;
        TimeSpan startTime, roundTimer, roundTime;
        Random random;
        SpriteBatch spriteBatch;
        SpriteFont statsFont;
        GameState currentGameState = GameState.Loading;

        GameObject ground;
        Camera gameCamera;

        GameObject boundingSphere;

        FuelCarrier fuelCarrier;
        FuelCell[] fuelCells;
        Barrier[] barriers;
        Enemy[] enemies;
        Fish[] fish;
        Vector3[] barrier_previous_movement;

        List<Projectiles> projectiles;

        //A tank
        Tank tank;

        private TimeSpan fireTime;
        private TimeSpan prevFireTime;
        // Textures
        protected Texture2D helpBackgroundTexture, helpForegroundTexture;
        //HelpScene helpScene;
        //protected GameScene activeScene;
        public PoseidonGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 853;
            graphics.PreferredBackBufferHeight = 700;

            Content.RootDirectory = "Content";

            roundTime = GameConstants.RoundTime;
            random = new Random();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            ground = new GameObject();
            gameCamera = new Camera();
            boundingSphere = new GameObject();
            tank = new Tank();

            fireTime = TimeSpan.FromSeconds(0.3f);

            enemies = new Enemy[GameConstants.NumberEnemies];
            fish = new Fish[GameConstants.NumberFish];

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            ground.Model = Content.Load<Model>("Models/ground");
            boundingSphere.Model = Content.Load<Model>("Models/sphere1uR");

            spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), spriteBatch);
            statsFont = Content.Load<SpriteFont>("Fonts/StatsFont");
            //For the Help scene
            helpBackgroundTexture = Content.Load<Texture2D>("Image/helpbackground");
            helpForegroundTexture = Content.Load<Texture2D>("Image/helpForeground");
            //helpScene = new HelpScene(this, helpBackgroundTexture,
            //helpForegroundTexture);
            //Components.Add(helpScene);
            //Initialize fuel cells
            fuelCells = new FuelCell[GameConstants.NumFuelCells];
            int powerType = random.Next(3) + 1;
            for (int index = 0; index < fuelCells.Length; index++)
            {
                fuelCells[index] = new FuelCell(powerType);     
                fuelCells[index].LoadContent(Content, "Models/fuelcell");
                powerType = random.Next(3) + 1;
            }

            //Initialize barriers
            barriers = new Barrier[GameConstants.NumBarriers];
            barrier_previous_movement = new Vector3[GameConstants.NumBarriers];
            int randomBarrier = random.Next(3);
            string barrierName = null;

            for (int index = 0; index < barriers.Length; index++)
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
                barriers[index] = new Barrier();
                barriers[index].LoadContent(Content, barrierName);
                randomBarrier = random.Next(3);
                barrier_previous_movement[index] = Vector3.Zero;
            }
            PlaceFuelCellsAndBarriers();

            //Initialize fuel carrier
            fuelCarrier = new FuelCarrier();
            fuelCarrier.LoadContent(Content, "Models/fuelcarrier");

            projectiles = new List<Projectiles>();

            tank.Load(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        private bool CheckEnterA()
        {
            // Get the Keyboard and GamePad state
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            bool result = (lastKeyboardState.IsKeyDown(Keys.Enter) &&
                (keyboardState.IsKeyUp(Keys.Enter)));
            result |= (lastGamePadState.Buttons.A == ButtonState.Pressed) &&
                      (gamepadState.Buttons.A == ButtonState.Released);

            lastKeyboardState = keyboardState;
            lastGamePadState = gamepadState;

            return result;
        }

        //private void HandleScenesInput()
        //{

        //    // Handle Help Scene input
        //    if (activeScene == helpScene)
        //    {
        //        if (CheckEnterA())
        //        {
        //            helpScene.Hide();
        //            currentGameState = GameState.Running;
        //        }
        //    }
            
        //}
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //HandleScenesInput();
            float aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
            lastKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            lastGamePadState = currentGamePadState;
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            // Allows the game to exit
            if ((currentKeyboardState.IsKeyDown(Keys.Escape)) ||
                (currentGamePadState.Buttons.Back == ButtonState.Pressed))
                this.Exit();
            if (currentGameState == GameState.Loading)
            {
                if ((lastKeyboardState.IsKeyDown(Keys.Enter) &&
                    (currentKeyboardState.IsKeyUp(Keys.Enter))) ||
                    currentGamePadState.Buttons.Start == ButtonState.Pressed)
                {
                    roundTimer = roundTime;
                    currentGameState = GameState.Running;
                }
            }

            if ((currentGameState == GameState.Running))
            {
                //fuelCarrier.Update(currentGamePadState, 
                //    currentKeyboardState, barriers);

                // Are we shooting?
                if (currentKeyboardState.IsKeyDown(Keys.L)
                    && gameTime.TotalGameTime.TotalSeconds - prevFireTime.TotalSeconds > fireTime.TotalSeconds / tank.fireRateUp)
                {
                    prevFireTime = gameTime.TotalGameTime;
                    placeBullet();
                }

                tank.Update(currentKeyboardState, barriers, fuelCells, gameTime);
                gameCamera.Update(tank.ForwardDirection,
                    tank.Position, aspectRatio);
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

                // BUG BUG BUG
                for (int i = 0; i < projectiles.Count; i++) {
                    projectiles[i].update(barriers);
                }
                Collision.updateBulletOutOfBound(projectiles, GraphicsDevice.Viewport);
                Collision.updateBulletVsBarriersCollision(projectiles, barriers);


                    //Collision.updateBulletVsBarriersCollision(projectiles, barriers);

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
                    ResetGame(gameTime, aspectRatio);
            }

            base.Update(gameTime);
        }

        private void ResetGame(GameTime gameTime, float aspectRatio)
        {
            fuelCarrier.Reset();
            gameCamera.Update(fuelCarrier.ForwardDirection,
                fuelCarrier.Position, aspectRatio);
            InitializeGameField();

            retrievedFuelCells = 0;
            startTime = gameTime.TotalGameTime;
            roundTimer = roundTime;
            currentGameState = GameState.Running;
        }

        private void InitializeGameField()
        {
            //Initialize barriers
            barriers = new Barrier[GameConstants.NumBarriers];
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
                barriers[index] = new Barrier();
                barriers[index].LoadContent(Content, barrierName);
                randomBarrier = random.Next(3);
            }
            PlaceFuelCellsAndBarriers();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            switch (currentGameState)
            {
                case GameState.Loading:
                    //helpScene.Show();
                    //activeScene = helpScene;
                    DrawSplashScreen();
                    break;
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

            base.Draw(gameTime);
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

        private void DrawSplashScreen()
        {
            float xOffsetText, yOffsetText;
            Vector2 viewportSize = new Vector2(GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height);
            Vector2 strCenter;

            graphics.GraphicsDevice.Clear(Color.SteelBlue);

            xOffsetText = yOffsetText = 0;
            Vector2 strInstructionsSize =
                statsFont.MeasureString(GameConstants.StrInstructions1);
            Vector2 strPosition;
            strCenter = new Vector2(strInstructionsSize.X / 2,
                strInstructionsSize.Y / 2);

            yOffsetText = (viewportSize.Y / 2 - strCenter.Y);
            xOffsetText = (viewportSize.X / 2 - strCenter.X);
            strPosition = new Vector2((int)xOffsetText, (int)yOffsetText);

            spriteBatch.Begin();
            spriteBatch.DrawString(statsFont, GameConstants.StrInstructions1,
                strPosition, Color.White);

            strInstructionsSize =
                statsFont.MeasureString(GameConstants.StrInstructions2);
            strCenter = new Vector2(strInstructionsSize.X / 2,
                strInstructionsSize.Y / 2);
            yOffsetText =
                (viewportSize.Y / 2 - strCenter.Y) + statsFont.LineSpacing;
            xOffsetText = (viewportSize.X / 2 - strCenter.X);
            strPosition = new Vector2((int)xOffsetText, (int)yOffsetText);

            spriteBatch.DrawString(statsFont, GameConstants.StrInstructions2,
                strPosition, Color.LightGray);
            spriteBatch.End();

            //re-enable depth buffer after sprite batch disablement

            //GraphicsDevice.DepthStencilState.DepthBufferEnable = true;
            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            GraphicsDevice.DepthStencilState = dss;

            //GraphicsDevice.RenderState.AlphaBlendEnable = false;
            //GraphicsDevice.RenderState.AlphaTestEnable = false;

            //GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
            //GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;
        }

        private void DrawWinOrLossScreen(string gameResult)
        {
            float xOffsetText, yOffsetText;
            Vector2 viewportSize = new Vector2(GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height);
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

            spriteBatch.Begin();
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

            spriteBatch.End();

            //re-enable depth buffer after sprite batch disablement

            //GraphicsDevice.DepthStencilState.DepthBufferEnable = true;
            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            GraphicsDevice.DepthStencilState = dss;

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
                    RasterizerState rs = new RasterizerState();
                    rs.FillMode = FillMode.WireFrame;
                    GraphicsDevice.RasterizerState = rs;
                    fuelCell.DrawBoundingSphere(gameCamera.ViewMatrix,
                        gameCamera.ProjectionMatrix, boundingSphere);

                    rs = new RasterizerState();
                    rs.FillMode = FillMode.Solid;
                    GraphicsDevice.RasterizerState = rs;
                }
            }
            foreach (Barrier barrier in barriers)
            {
                barrier.Draw(gameCamera.ViewMatrix,
                    gameCamera.ProjectionMatrix);
                RasterizerState rs = new RasterizerState();
                rs.FillMode = FillMode.WireFrame;
                GraphicsDevice.RasterizerState = rs;
                barrier.DrawBoundingSphere(gameCamera.ViewMatrix,
                    gameCamera.ProjectionMatrix, boundingSphere);

                rs = new RasterizerState();
                rs.FillMode = FillMode.Solid;
                GraphicsDevice.RasterizerState = rs;
            }

            // Update bullets
            foreach (Projectiles p in projectiles) {
                if (p.getStatus()) {
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
            string str2 =
                GameConstants.StrCellsFound + retrievedFuelCells.ToString() +
                " of " + GameConstants.NumFuelCells.ToString();
            Rectangle rectSafeArea;

            str1 += (roundTimer.Seconds).ToString();

            //Calculate str1 position
            rectSafeArea = GraphicsDevice.Viewport.TitleSafeArea;

            xOffsetText = rectSafeArea.X;
            yOffsetText = rectSafeArea.Y;

            Vector2 strSize = statsFont.MeasureString(str1);
            Vector2 strPosition =
                new Vector2((int)xOffsetText + 10, (int)yOffsetText);

            spriteBatch.Begin();
            spriteBatch.DrawString(statsFont, str1, strPosition, Color.White);
            strPosition.Y += strSize.Y;
            spriteBatch.DrawString(statsFont, str2, strPosition, Color.White);
            spriteBatch.End();

            //re-enable depth buffer after sprite batch disablement

            //GraphicsDevice.DepthStencilState.DepthBufferEnable = true;
            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            GraphicsDevice.DepthStencilState = dss;

            //GraphicsDevice.RenderState.AlphaBlendEnable = false;
            //GraphicsDevice.RenderState.AlphaTestEnable = false;

            //GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
            //GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;
        }
    }
}
