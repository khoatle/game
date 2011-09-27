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
        MouseState currentMouseState = new MouseState();
        MouseState lastMouseState = new MouseState();
        private AudioLibrary audio;


        Random random;
        SpriteBatch spriteBatch;
        SpriteFont statsFont;
        GameObject ground;
        Camera gameCamera;
        
        GameObject boundingSphere;


        //List<FuelCell> fuelCells;
        List<Fruit> fruits;

        List<Projectiles> projectiles;

        Enemy[] enemies;
        Fish[] fish;

        int enemiesAmount = 0;
        int fishAmount = 0;

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
            // for the mouse or touch
            cursor = new Cursor(game, spriteBatch);
            Components.Add(cursor);
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
            //fuelCells = new List<FuelCell>(GameConstants.NumFuelCells);
            //int powerType = random.Next(3) + 1;
            //for (int index = 0; index < GameConstants.NumFuelCells; index++)
            //{
            //    fuelCells.Add(new FuelCell(powerType));
            //    fuelCells[index].LoadContent(Content, "Models/fuelcell");
            //    powerType = random.Next(3) + 1;
            //}

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
            placeEnemy();
            placeFish();

        }

        private void loadContentEnemies()
        {
            for (int i = 0; i < GameConstants.NumberEnemies; i++)
            {
                enemies[i] = new Enemy();
                enemies[i].LoadContent(Content, "Models/pyramid10uR");
            }
        }

        private void loadContentFish()
        {
            for (int i = 0; i < GameConstants.NumberFish; i++)
            {
                fish[i] = new Fish();
                fish[i].LoadContent(Content, "Models/cube10uR");
            }
        }

        private void placeEnemy()
        {
            loadContentEnemies();

            int min = GameConstants.MinDistance;
            int max = GameConstants.MaxDistance;
            Vector3 tempCenter;

            //place enemies
            for (int i = 0; i < enemiesAmount; i++)
            {
                enemies[i].Position = GenerateRandomPosition(min, max);
                enemies[i].Position.Y = GameConstants.FloatHeight;
                tempCenter = enemies[i].BoundingSphere.Center;
                tempCenter.X = enemies[i].Position.X;
                tempCenter.Y = GameConstants.FloatHeight;
                tempCenter.Z = enemies[i].Position.Z;
                enemies[i].BoundingSphere =
                    new BoundingSphere(tempCenter, enemies[i].BoundingSphere.Radius);
            }
        }

        private void placeFish()
        {
            loadContentFish();

            int min = GameConstants.MinDistance;
            int max = GameConstants.MaxDistance;
            Vector3 tempCenter;

            //place fish
            for (int i = 0; i < fishAmount; i++)
            {
                fish[i].Position = GenerateRandomPosition(min, max);
                fish[i].Position.Y = GameConstants.FloatHeight;
                tempCenter = fish[i].BoundingSphere.Center;
                tempCenter.X = fish[i].Position.X;
                tempCenter.Y = GameConstants.FloatHeight;
                tempCenter.Z = fish[i].Position.Z;
                fish[i].BoundingSphere =
                    new BoundingSphere(tempCenter, fish[i].BoundingSphere.Radius);
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
            //foreach (GameObject currentObj in fuelCells)
            //{
            //    if (((int)(MathHelper.Distance(
            //        xValue, currentObj.Position.X)) < 15) &&
            //        ((int)(MathHelper.Distance(
            //        zValue, currentObj.Position.Z)) < 15))
            //        return true;
            //}

            for (int i = 0; i < enemiesAmount; i++)
            {
                if (((int)(MathHelper.Distance(
                    xValue, enemies[i].Position.X)) < 15) &&
                    ((int)(MathHelper.Distance(
                    zValue, enemies[i].Position.Z)) < 15))
                    return true;
            }

            for (int i = 0; i < fishAmount; i++)
            {
                if (((int)(MathHelper.Distance(
                    xValue, fish[i].Position.X)) < 15) &&
                    ((int)(MathHelper.Distance(
                    zValue, fish[i].Position.Z)) < 15))
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
                lastMouseState = currentMouseState;
                currentKeyboardState = Keyboard.GetState();
                lastGamePadState = currentGamePadState;
                currentGamePadState = GamePad.GetState(PlayerIndex.One);
                currentMouseState = Mouse.GetState();


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


                Vector3 pointIntersect;
                //float angle;
                //if (lastMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released)
                if (currentMouseState.LeftButton == ButtonState.Pressed)
                {
                    pointIntersect = IntersectPointWithPlane(GameConstants.FloatHeight);
                    //angle = CalculateAngle(pointIntersect, tank.Position);
                }
                else pointIntersect = Vector3.Zero;
                tank.Update(currentKeyboardState, enemies, enemiesAmount, fruits, gameTime, pointIntersect);
                gameCamera.Update(tank.ForwardDirection,
                    tank.Position, aspectRatio);

                for (int i = 0; i < projectiles.Count; i++)
                {
                    projectiles[i].update();
                }
                Collision.updateBulletOutOfBound(projectiles, GraphicDevice.Viewport);
                Collision.updateBulletVsBarriersCollision(projectiles, enemies, ref enemiesAmount);

                for (int i = 0; i < enemiesAmount; i++)
                {
                    enemies[i].Update(enemies, enemiesAmount, random.Next(100), tank);
                }

                for (int i = 0; i < fishAmount; i++)
                {
                    fish[i].Update(enemies, fishAmount, random.Next(100), tank);
                }

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


            base.Draw(gameTime);
            if (paused)
            {
                // Draw the "pause" text
                spriteBatch.Draw(actionTexture, pausePosition, pauseRect,
                    Color.White);
            }

            // Change back the config changed by spriteBatch
            GraphicDevice.BlendState = BlendState.Opaque;
            GraphicDevice.DepthStencilState = DepthStencilState.Default;
            GraphicDevice.SamplerStates[0] = SamplerState.LinearWrap;

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
            //foreach (FuelCell fuelCell in fuelCells)
            //{
            //    if (!fuelCell.Retrieved)
            //    {
            //        fuelCell.Draw(gameCamera.ViewMatrix,
            //            gameCamera.ProjectionMatrix);
            //    }
            //}

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

            xOffsetText = rectSafeArea.Right - 50;
            yOffsetText = rectSafeArea.Top;

            Vector2 bulletIconPosition =
                new Vector2((int)xOffsetText, (int)yOffsetText);

            spriteBatch.Draw(bulletTypeTextures[tank.bulletType], bulletIconPosition, Color.White);
        }
        // Draw the currently selected skill/spell
        private void DrawActiveSkill()
        {
            float xOffsetText, yOffsetText;
            Rectangle rectSafeArea;

            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            xOffsetText = rectSafeArea.Right - 100;
            yOffsetText = rectSafeArea.Top + 50;

            Vector2 skillIconPosition =
                new Vector2((int)xOffsetText, (int)yOffsetText);

            spriteBatch.Draw(skillTextures[tank.activeSkillID], skillIconPosition, Color.White);
        }
        private void DrawStats()
        {
            float xOffsetText, yOffsetText;
            string str1 = GameConstants.StrTimeRemaining;

            Rectangle rectSafeArea;


            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            xOffsetText = rectSafeArea.X;
            yOffsetText = rectSafeArea.Y;

            Vector2 strSize = statsFont.MeasureString(str1);
            Vector2 strPosition =
                new Vector2((int)xOffsetText + 10, (int)yOffsetText);

            spriteBatch.DrawString(statsFont, "This is the ship wreck scene " + str1, strPosition, Color.White);

        }
        public Vector3 IntersectPointWithPlane(float planeHeight)
        {
            Ray cursorRay = cursor.CalculateCursorRay(gameCamera.ProjectionMatrix, gameCamera.ViewMatrix);
            float t = (planeHeight - cursorRay.Position.Y) / cursorRay.Direction.Y;
            float x = cursorRay.Position.X + cursorRay.Direction.X * t;
            float z = cursorRay.Position.Z + cursorRay.Direction.Z * t;
            return new Vector3(x, planeHeight, z);
        }
    }
}
