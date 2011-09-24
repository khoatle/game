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
    public partial class ShipWreckScene : GameScene
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
      
        
        Random random;
        SpriteBatch spriteBatch;
        SpriteFont statsFont;
        GameObject ground;
        Camera gameCamera;
        
        GameObject boundingSphere;

        
        List<FuelCell> fuelCells;
        List<Barrier> barriers;
        
        List<Projectiles> projectiles;

        Enemy[] enemies;
        Fish[] fish;


        //A tank
        public Tank tank;

        private TimeSpan fireTime;
        private TimeSpan prevFireTime;

        // For drawing the currently selected skill
        protected Texture2D[] skillTextures;
        public string[] iconNames = { "Image/skill0Icon", "Image/skill1Icon", "Image/skill2Icon" };
        // For drawing the currently selected bullet type
        protected Texture2D[] bulletTypeTextures;
        public string[] bulletNames = { "Image/skill0Icon", "Image/skill1Icon" };
        // Game is paused?
        protected bool paused;
        protected Vector2 pausePosition;
        protected Rectangle pauseRect = new Rectangle(1, 120, 200, 44);
        protected Texture2D actionTexture;

        // He died inside the ship wreck?
        public bool dead;
        // has artifact?
        public int skillID = 0;

        public ShipWreckScene(Game game, GraphicsDeviceManager graphics, ContentManager Content, GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch, Vector2 pausePosition, Rectangle pauseRect, Texture2D actionTexture, CutSceneDialog cutSceneDialog)
            : base(game)
        {
            this.graphics = graphics;
            this.Content = Content;
            this.GraphicDevice = GraphicsDevice;
            this.spriteBatch = spriteBatch;
            this.pausePosition = pausePosition;
            this.pauseRect = pauseRect;
            this.actionTexture = actionTexture;
            this.game = game;

            random = new Random();
            ground = new GameObject();
            gameCamera = new Camera();
            boundingSphere = new GameObject();
            tank = new Tank();
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
            fuelCells = new List<FuelCell>(GameConstants.NumFuelCells);
            int powerType = random.Next(3) + 1;
            for (int index = 0; index < GameConstants.NumFuelCells; index++)
            {
                fuelCells.Add(new FuelCell(powerType));
                fuelCells[index].LoadContent(Content, "Models/fuelcell");
                powerType = random.Next(3) + 1;
            }

            //Initialize the game field
            InitializeShipField(Content);



            projectiles = new List<Projectiles>();

            tank.Load(Content);

        }

        /// <summary>
        /// Show the action scene
        /// </summary>
        
        public override void Show()
        {
            paused = false;
            tank.Reset();
            MediaPlayer.Play(audio.BackMusic);
            InitializeShipField(Content);
            base.Show();
        }
        private void ResetGame(GameTime gameTime, float aspectRatio)
        {
            tank.Reset();
            gameCamera.Update(tank.ForwardDirection,
                tank.Position, aspectRatio);
            InitializeShipField(Content);

        }

        private void InitializeShipField(ContentManager Content)
        {
            dead = false;
            // Initialize the chests here
            // Put the skill in one of it if this.skillID != 0

            //Initialize barriers
            barriers = new List<Barrier>(GameConstants.NumBarriers);
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
        // Helper
        private void PlaceFuelCellsAndBarriers()
        {
            int min = GameConstants.MinDistance;
            int max = GameConstants.MaxDistance;
            Vector3 tempCenter;

            //place fuel cells
            foreach (FuelCell cell in fuelCells)
            {
                cell.Position = GenerateRandomPosition(min, max);
                cell.Position.Y = GameConstants.FloatHeight;
                tempCenter = cell.BoundingSphere.Center;
                tempCenter.X = cell.Position.X;
                tempCenter.Y = GameConstants.FloatHeight;
                tempCenter.Z = cell.Position.Z;
                cell.BoundingSphere =
                    new BoundingSphere(tempCenter, cell.BoundingSphere.Radius);
                cell.Retrieved = false;
            }

            //place barriers
            foreach (Barrier barrier in barriers)
            {
                barrier.Position = GenerateRandomPosition(min, max);
                barrier.Position.Y = GameConstants.FloatHeight;
                tempCenter = barrier.BoundingSphere.Center;
                tempCenter.X = barrier.Position.X;
                tempCenter.Y = GameConstants.FloatHeight;
                tempCenter.Z = barrier.Position.Z;
                barrier.BoundingSphere = new BoundingSphere(tempCenter,
                    barrier.BoundingSphere.Radius);
            }

        }

        // Helper
        private void placeBullet()
        {
            Projectiles p = new Projectiles();
            p.initialize(GraphicDevice.Viewport, tank.Position, GameConstants.BulletSpeed, tank.ForwardDirection, tank.strengthUp);
            p.loadContent(Content, "Models/sphere1uR");
            projectiles.Add(p);
        }
        // Helper
        private Vector3 GenerateRandomPosition(int min, int max)
        {
            int xValue, zValue;
            do
            {
                xValue = random.Next(min, max);
                zValue = random.Next(min, max);
                if (random.Next(100) % 2 == 0)
                    xValue *= -1;
                if (random.Next(100) % 2 == 0)
                    zValue *= -1;

            } while (IsOccupied(xValue, zValue));

            return new Vector3(xValue, 0, zValue);
        }

        // Helper
        private bool IsOccupied(int xValue, int zValue)
        {
            foreach (GameObject currentObj in fuelCells)
            {
                if (((int)(MathHelper.Distance(
                    xValue, currentObj.Position.X)) < 15) &&
                    ((int)(MathHelper.Distance(
                    zValue, currentObj.Position.Z)) < 15))
                    return true;
            }

            foreach (GameObject currentObj in barriers)
            {
                if (((int)(MathHelper.Distance(
                    xValue, currentObj.Position.X)) < 15) &&
                    ((int)(MathHelper.Distance(
                    zValue, currentObj.Position.Z)) < 15))
                    return true;
            }
            return false;
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
            if (!paused && !dead)
            {
                float aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
                lastKeyboardState = currentKeyboardState;
                currentKeyboardState = Keyboard.GetState();
                lastGamePadState = currentGamePadState;
                currentGamePadState = GamePad.GetState(PlayerIndex.One);


                // changing active skill
                if ((lastKeyboardState.IsKeyDown(Keys.LeftShift) || lastKeyboardState.IsKeyDown(Keys.RightShift)) && lastKeyboardState.IsKeyDown(Keys.K)
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
                if ((lastKeyboardState.IsKeyDown(Keys.LeftShift) || lastKeyboardState.IsKeyDown(Keys.RightShift)) && lastKeyboardState.IsKeyDown(Keys.L)
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


                tank.Update(currentKeyboardState, barriers, fuelCells, gameTime);
                gameCamera.Update(tank.ForwardDirection,
                    tank.Position, aspectRatio);


                // Update barrier (enemies)
                for (int i = 0; i < barriers.Count; i++)
                {
                    barriers[i].Update(barriers, random.Next(100), tank);
                }


                for (int i = 0; i < projectiles.Count; i++)
                {
                    projectiles[i].update(barriers);
                }
                Collision.updateBulletOutOfBound(projectiles, GraphicDevice.Viewport);
                Collision.updateBulletVsBarriersCollision(projectiles, barriers);

                // Just for death simulation
                // should be removed
                if (lastKeyboardState.IsKeyDown(Keys.O) &&
                        currentKeyboardState.IsKeyUp(Keys.O))
                {
                    dead = true;
                }
                
                base.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (dead) return;
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
 
              
            DrawGameplayScreen();

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
            DrawBulletType();
            if (tank.activeSkillID != -1) DrawActiveSkill();
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
        private void DrawStats()
        {
            float xOffsetText, yOffsetText;
            string str1 = GameConstants.StrTimeRemaining;
            string str2 = GameConstants.StrCellsFound +
                " of " + fuelCells.Count;
            Rectangle rectSafeArea;


            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            xOffsetText = rectSafeArea.X;
            yOffsetText = rectSafeArea.Y;

            Vector2 strSize = statsFont.MeasureString(str1);
            Vector2 strPosition =
                new Vector2((int)xOffsetText + 10, (int)yOffsetText);

            //spriteBatch.Begin();
            spriteBatch.DrawString(statsFont, "This is the ship wreck scene " + str1, strPosition, Color.White);
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
