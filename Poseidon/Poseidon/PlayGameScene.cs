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
        Game game;
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
        public GameState currentGameState = GameState.PlayingCutScene;
        // In order to know we are resetting the level winning or losing
        // winning: keep the current tank
        // losing: reset our tank to the tank at the beginning of the level
        GameState prevGameState;
        GameObject boundingSphere;

        FuelCarrier fuelCarrier;
        List<FuelCell> fuelCells;
        List<Barrier> barriers;
        public List<ShipWreck> shipWrecks;
        List<Projectiles> projectiles;
        List<Plant> plants;

        Enemy[] enemies;
        Fish[] fish;
        

        // The main character for this level
        public Tank tank;
        // The main character at the beginning of this level
        // Used for restarting the level
        Tank prevTank;
        private TimeSpan fireTime;
        private TimeSpan prevFireTime;

        // Game is paused?
        protected bool paused;
        protected Vector2 pausePosition;
        protected Rectangle pauseRect = new Rectangle(1, 120, 200, 44);
        protected Texture2D actionTexture;

        // For drawing the currently selected skill
        protected Texture2D[] skillTextures;
        public string[] iconNames = { "Image/skill0Icon", "Image/skill1Icon", "Image/skill2Icon" };
        // For drawing the currently selected bullet type
        protected Texture2D[] bulletTypeTextures;
        public string[] bulletNames = { "Image/skill0Icon", "Image/skill1Icon"};

        // Current game level
        public int currentLevel = 0;

        // Cutscene
        CutSceneDialog cutSceneDialog;
        // Which sentence in the dialog is being printed
        int currentSentence = 0;
        
        public PlayGameScene(Game game, GraphicsDeviceManager graphics, ContentManager Content, GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch, Vector2 pausePosition, Rectangle pauseRect, Texture2D actionTexture, CutSceneDialog cutSceneDialog):base(game)
        {
            this.graphics = graphics;
            this.Content = Content;
            this.GraphicDevice = GraphicsDevice;
            this.spriteBatch = spriteBatch;
            this.pausePosition = pausePosition;
            this.pauseRect = pauseRect;
            this.actionTexture = actionTexture;
            this.game = game;
            this.cutSceneDialog = cutSceneDialog;
            roundTime = GameConstants.RoundTime;
            random = new Random();
            ground = new GameObject();
            gameCamera = new Camera();
            boundingSphere = new GameObject();
            tank = new Tank();
            prevTank = new Tank();
            fireTime = TimeSpan.FromSeconds(0.3f);
            enemies = new Enemy[GameConstants.NumberEnemies];
            fish = new Fish[GameConstants.NumberFish];
            skillTextures = new Texture2D[GameConstants.numberOfSkills];
            bulletTypeTextures = new Texture2D[GameConstants.numBulletTypes];
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

            // Loading main character skill icon textures
            for (int index = 0; index < GameConstants.numberOfSkills; index++)
            {
                skillTextures[index] = Content.Load<Texture2D>(iconNames[index]);
            }

            // Loading main character bullet icon textures
            for (int index = 0; index < GameConstants.numBulletTypes; index++)
            {
                bulletTypeTextures[index] = Content.Load<Texture2D>(bulletNames[index]);
            }
            
            //Initialize fuel cells
            fuelCells = new List<FuelCell> (GameConstants.NumFuelCells);
            int powerType = random.Next(3) + 1;
            for (int index = 0; index < GameConstants.NumFuelCells; index++)
            {
                fuelCells.Add(new FuelCell(powerType));
                fuelCells[index].LoadContent(Content, "Models/fuelcell");
                powerType = random.Next(3) + 1;
            }
            
            //Initialize the game field
            InitializeGameField(Content);

            //Initialize fuel carrier
            fuelCarrier = new FuelCarrier();
            fuelCarrier.LoadContent(Content, "Models/fuelcarrier");

            projectiles = new List<Projectiles>();

            plants = new List<Plant>();

            tank.Load(Content);
            
            prevTank.Load(Content);
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
            // If we are resetting the level losing the game
            // Reset our tank to the one at the beginning of the lost level
            if (prevGameState == GameState.Lost) tank.CopyAttribute(prevTank);
            else prevTank.CopyAttribute(tank);

            tank.Reset();
            gameCamera.Update(tank.ForwardDirection,
                tank.Position, aspectRatio);
            InitializeGameField(Content);

            //Cleann all trees
            plants.Clear();

            retrievedFuelCells = 0;
            startTime = gameTime.TotalGameTime;
            roundTimer = roundTime;
            currentSentence = 0;
            currentGameState = GameState.PlayingCutScene;
        }

        private void InitializeGameField(ContentManager Content)
        {
            //Initialize the ship wrecks
            shipWrecks = new List<ShipWreck>(GameConstants.NumberShipWrecks);
            int randomType = random.Next(3);
            // example of how to put skill into a ship wreck at a certain level
            // just put the skill into the 1st ship wrect in the list
            for (int index = 0; index < GameConstants.NumberShipWrecks; index++)
            {
                shipWrecks.Add(new ShipWreck());
                if (index == 0) shipWrecks[index].LoadContent(Content, randomType, 1);
                else shipWrecks[index].LoadContent(Content, randomType, 0);
                randomType = random.Next(3);
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


                if (currentGameState == GameState.PlayingCutScene)
                {
                    // Next sentence when the user press Enter
                    if ((lastKeyboardState.IsKeyDown(Keys.Enter) &&
                        (currentKeyboardState.IsKeyUp(Keys.Enter))) ||
                        currentGamePadState.Buttons.Start == ButtonState.Pressed)
                    {
                        currentSentence++;
                        // End of cutscene for this level
                        if (currentSentence == cutSceneDialog.cutScenes[currentLevel].Count)
                            currentGameState = GameState.Running;
                    }
                }
                if ((currentGameState == GameState.Running))
                {
                    //fuelCarrier.Update(currentGamePadState, 
                    //    currentKeyboardState, barriers);

                    // changing active skill
                    if ((lastKeyboardState.IsKeyDown(Keys.LeftShift) ||  lastKeyboardState.IsKeyDown(Keys.RightShift))&& lastKeyboardState.IsKeyDown(Keys.K)
                            && currentKeyboardState.IsKeyUp(Keys.K))
                    {
                        if (tank.activeSkillID != -1)
                        {
                            tank.activeSkillID++;
                            if (tank.activeSkillID == GameConstants.numberOfSkills) tank.activeSkillID = 0;
                            while (tank.skills[tank.activeSkillID] == false)
                            {
                                tank.activeSkillID++;
                                if (tank.activeSkillID == GameConstants.numberOfSkills) tank.activeSkillID = 0;
                            }
                        }
                    }
                    // changing bullet type
                    if ((lastKeyboardState.IsKeyDown(Keys.LeftShift) ||  lastKeyboardState.IsKeyDown(Keys.RightShift)) && lastKeyboardState.IsKeyDown(Keys.L)
                            && currentKeyboardState.IsKeyUp(Keys.L))
                    {
                            tank.bulletType++;
                            if (tank.bulletType == GameConstants.numBulletTypes) tank.bulletType = 0;

                    }
                    // Are we shooting?
                    if (!(lastKeyboardState.IsKeyDown(Keys.LeftShift) || lastKeyboardState.IsKeyDown(Keys.RightShift)) && currentKeyboardState.IsKeyDown(Keys.L)
                        && gameTime.TotalGameTime.TotalSeconds - prevFireTime.TotalSeconds > fireTime.TotalSeconds / (tank.shootingRate * tank.fireRateUp))
                    {
                        prevFireTime = gameTime.TotalGameTime;
                        audio.Shooting.Play();
                        placeBullet();
                    }

                    //Are we planting trees?
                    if ((lastKeyboardState.IsKeyDown(Keys.O) && (currentKeyboardState.IsKeyUp(Keys.O))))
                    {
                        audio.Shooting.Play();
                        placePlant();
                    }

                    tank.Update(currentKeyboardState, barriers, fuelCells, gameTime);
                    gameCamera.Update(tank.ForwardDirection,
                        tank.Position, aspectRatio);

                    
                    // Update barrier (enemies)
                    for (int i = 0; i < barriers.Count; i++) {
                        barriers[i].Update(barriers, random.Next(100), tank);
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
                
                prevGameState = currentGameState;
                if (currentGameState == GameState.Lost)
                {
                    // Reset the world for a new game
                    if ((lastKeyboardState.IsKeyDown(Keys.Enter) &&
                        (currentKeyboardState.IsKeyUp(Keys.Enter))) ||
                        currentGamePadState.Buttons.Start == ButtonState.Pressed)
                    {
                        
                        ResetGame(gameTime, aspectRatio);

                    }
                }
                if (currentGameState == GameState.Won)
                {
                    if ((lastKeyboardState.IsKeyDown(Keys.Enter) &&
                        (currentKeyboardState.IsKeyUp(Keys.Enter))) ||
                        currentGamePadState.Buttons.Start == ButtonState.Pressed)
                    {
                        currentLevel++;
                        ResetGame(gameTime, aspectRatio);
                        
                    }
                }
                base.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // Change back the config changed by spriteBatch
            GraphicDevice.BlendState = BlendState.Opaque;
            GraphicDevice.DepthStencilState = DepthStencilState.Default;
            GraphicDevice.SamplerStates[0] = SamplerState.LinearWrap;
 
            base.Draw(gameTime);
            if (paused)
            {
                // Draw the "pause" text
                spriteBatch.Draw(actionTexture, pausePosition, pauseRect,
                    Color.White);
            }
            switch (currentGameState)
            {
                case GameState.PlayingCutScene:
                    DrawCutScene();
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
            // Drawing ship wrecks
            foreach (ShipWreck shipWreck in shipWrecks)
            {
                shipWreck.Draw(gameCamera.ViewMatrix,
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

            // Draw each plant
            foreach (Plant p in plants)
            {
                p.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
                //RasterizerState rs = new RasterizerState();
                //rs.FillMode = FillMode.WireFrame;
                //GraphicDevice.RasterizerState = rs;
                //p.DrawBoundingSphere(gameCamera.ViewMatrix,
                //    gameCamera.ProjectionMatrix, boundingSphere);

                //rs = new RasterizerState();
                //rs.FillMode = FillMode.Solid;
                //GraphicDevice.RasterizerState = rs;
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
            DrawBulletType();
            if (tank.activeSkillID != -1) DrawActiveSkill();
        }

        private void DrawStats()
        {
            float xOffsetText, yOffsetText;
            string str1 = GameConstants.StrTimeRemaining;
            string str2 = GameConstants.StrCellsFound + retrievedFuelCells.ToString() +
                " of " + fuelCells.Count;
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
        // Draw the currently selected bullet type
        private void DrawBulletType()
        {
            float xOffsetText, yOffsetText;
            Rectangle rectSafeArea;

            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            xOffsetText = rectSafeArea.X;
            yOffsetText = rectSafeArea.Y;

            Vector2 bulletIconPosition =
                new Vector2((int)xOffsetText + 50, (int)yOffsetText + 100);

            spriteBatch.Draw(bulletTypeTextures[tank.bulletType], bulletIconPosition, Color.White);
        }
        // Draw the currently selected skill/spell
        private void DrawActiveSkill()
        {
            float xOffsetText, yOffsetText;
            Rectangle rectSafeArea;

            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            xOffsetText = rectSafeArea.X;
            yOffsetText = rectSafeArea.Y;
         
            Vector2 skillIconPosition =
                new Vector2((int)xOffsetText + 50, (int)yOffsetText + 50);

            spriteBatch.Draw(skillTextures[tank.activeSkillID], skillIconPosition, Color.White);
        }
        private void DrawCutScene()
        {
            float xOffsetText, yOffsetText;
            string str1 = cutSceneDialog.cutScenes[currentLevel][currentSentence].sentence;
            Rectangle rectSafeArea;
            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            xOffsetText = rectSafeArea.X;
            yOffsetText = rectSafeArea.Y;

            Vector2 strSize = statsFont.MeasureString(str1);
            Vector2 strPosition =
                new Vector2((int)xOffsetText + 10, (int)yOffsetText);
            spriteBatch.DrawString(statsFont, str1, strPosition, Color.White);
        }
    }
}
