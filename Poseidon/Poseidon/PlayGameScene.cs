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
        MouseState currentMouseState = new MouseState();
        MouseState lastMouseState = new MouseState();
        private AudioLibrary audio;
        int retrievedFruits;
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
        public List<ShipWreck> shipWrecks;
        
        public List<DamageBullet> myBullet;
        public List<DamageBullet> enemyBullet;
        public List<HealthBullet> healthBullet;

        List<Plant> plants;
        List<Fruit> fruits;

        Enemy[] enemies;
        Fish[] fish;

        int enemiesAmount = 0;
        int fishAmount = 0;


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
        public string[] bulletNames = { "Image/skill0Icon", "Image/skill1Icon" };

        // Current game level
        public int currentLevel = 0;

        // Cutscene
        CutSceneDialog cutSceneDialog;
        // Which sentence in the dialog is being printed
        int currentSentence = 0;
        
        HeightMapInfo heightMapInfo;

        Radar radar;

        public PlayGameScene(Game game, GraphicsDeviceManager graphics, ContentManager Content, GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch, Vector2 pausePosition, Rectangle pauseRect, Texture2D actionTexture, CutSceneDialog cutSceneDialog, Radar radar)
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
            this.cutSceneDialog = cutSceneDialog;
            this.radar = radar;
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

            // for the mouse or touch
            cursor = new Cursor(game, spriteBatch);
            Components.Add(cursor);

            myBullet = new List<DamageBullet>();
            healthBullet = new List<HealthBullet>();
            enemyBullet = new List<DamageBullet>();

            this.Load();
        }

        public void Load()
        {
            statsFont = Content.Load<SpriteFont>("Fonts/StatsFont");
            // Get the audio library
            audio = (AudioLibrary)
                Game.Services.GetService(typeof(AudioLibrary));

            ground.Model = Content.Load<Model>("Image/terrain");
            boundingSphere.Model = Content.Load<Model>("Models/sphere1uR");

            heightMapInfo = ground.Model.Tag as HeightMapInfo;
            if (heightMapInfo == null)
            {
                string message = "The terrain model did not have a HeightMapInfo " +
                    "object attached. Are you sure you are using the " +
                    "TerrainProcessor?";
                throw new InvalidOperationException(message);
            }

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

            //Initialize the game field
            InitializeGameField(Content);

            //Initialize fuel carrier
            fuelCarrier = new FuelCarrier();
            fuelCarrier.LoadContent(Content, "Models/fuelcarrier");

            plants = new List<Plant>();
            fruits = new List<Fruit>();

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

            enemiesAmount = GameConstants.NumberEnemies;
            fishAmount = GameConstants.NumberFish;
            InitializeGameField(Content);

            //Clean all trees
            plants.Clear();

            //Clean all fruits
            fruits.Clear();

            retrievedFruits = 0;
            startTime = gameTime.TotalGameTime;
            roundTimer = roundTime;
            currentSentence = 0;
            currentGameState = GameState.PlayingCutScene;
        }

        private void InitializeGameField(ContentManager Content)
        {
            enemyBullet = new List<DamageBullet>();
            healthBullet = new List<HealthBullet>();
            myBullet = new List<DamageBullet>();

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
            placeEnemies();
            placeFish();
            //placeFuelCells();
            placeShipWreck();
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
                if (paused) {
                    MediaPlayer.Pause();
                }
                else {
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
                lastMouseState = currentMouseState;
                currentKeyboardState = Keyboard.GetState();
                lastGamePadState = currentGamePadState;
                currentGamePadState = GamePad.GetState(PlayerIndex.One);
                currentMouseState = Mouse.GetState();
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

                    // Are we shooting?
                    if (!(lastKeyboardState.IsKeyDown(Keys.LeftShift) || lastKeyboardState.IsKeyDown(Keys.RightShift)) && currentKeyboardState.IsKeyDown(Keys.L)
                        && gameTime.TotalGameTime.TotalSeconds - prevFireTime.TotalSeconds > fireTime.TotalSeconds / (tank.shootingRate * tank.fireRateUp))
                    {
                        prevFireTime = gameTime.TotalGameTime;
                        audio.Shooting.Play();
                        if (tank.bulletType == 0) { placeDamageBullet(); }
                        else if (tank.bulletType == 1) { placeHealingBullet(); }
                    }


                    // changing bullet type
                    if ((lastKeyboardState.IsKeyDown(Keys.LeftShift) || lastKeyboardState.IsKeyDown(Keys.RightShift)) && lastKeyboardState.IsKeyDown(Keys.L)
                            && currentKeyboardState.IsKeyUp(Keys.L))
                    {
                        tank.bulletType++;
                        if (tank.bulletType == GameConstants.numBulletTypes) tank.bulletType = 0;

                    }

                    //Are we planting trees?
                    if ((lastKeyboardState.IsKeyDown(Keys.O) && (currentKeyboardState.IsKeyUp(Keys.O))))
                    {
                        audio.Shooting.Play();
                        placePlant();
                    }

                    //Are the trees ready for fruit?
                    foreach (Plant plant in plants)
                    {
                        if (plant.timeForFruit == true && plant.fruitCreated == false)
                        {
                            int powerType = random.Next(3) + 1;
                            Fruit fruit = new Fruit(powerType);
                            fruits.Add(fruit);
                            fruit.LoadContent(Content, "Models/fuelcell", plant.Position);
                            plant.fruitCreated = true;
                        }
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

                    retrievedFruits = 0;
                    foreach (Fruit fruit in fruits)
                    {
                        fruit.Update(currentKeyboardState, tank.BoundingSphere, tank.Trash_Fruit_BoundingSphere);
                        if (fruit.Retrieved)
                        {
                            retrievedFruits++;
                        }
                    }

                    for (int i = 0; i < myBullet.Count; i++) {
                        myBullet[i].update();
                    }

                    for (int i = 0; i < healthBullet.Count; i++) {
                        healthBullet[i].update();
                    }
                    Collision.updateBulletOutOfBound(healthBullet, myBullet, GraphicDevice.Viewport);
                    Collision.updateDamageBulletVsBarriersCollision(myBullet, enemies, ref enemiesAmount);
                    Collision.updateHealingBulletVsBarrierCollision(healthBullet, enemies, enemiesAmount);

                    for (int i = 0; i < enemiesAmount; i++) {
                        enemies[i].Update(enemies, enemiesAmount, tank);
                    }

                    for (int i = 0; i < fishAmount; i++) {
                        fish[i].Update(enemies, enemiesAmount, tank);
                    }

                    if (retrievedFruits == GameConstants.NumFuelCells)
                    {
                        currentGameState = GameState.Won;
                    }
                    roundTimer -= gameTime.ElapsedGameTime;
                    if ((roundTimer < TimeSpan.Zero) &&
                        (retrievedFruits != GameConstants.NumFuelCells))
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
                    // Change back the config changed by spriteBatch
                    GraphicDevice.BlendState = BlendState.Opaque;
                    GraphicDevice.DepthStencilState = DepthStencilState.Default;
                    GraphicDevice.SamplerStates[0] = SamplerState.LinearWrap;
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
        }

        private void DrawGameplayScreen()
        {
            DrawTerrain(ground.Model);

            foreach (Fruit f in fruits)
            {
                if (!f.Retrieved)
                {
                    f.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
                    RasterizerState rs = new RasterizerState();
                    rs.FillMode = FillMode.WireFrame;
                    GraphicDevice.RasterizerState = rs;
                    f.DrawBoundingSphere(gameCamera.ViewMatrix,
                        gameCamera.ProjectionMatrix, boundingSphere);

                    rs = new RasterizerState();
                    rs.FillMode = FillMode.Solid;
                    GraphicDevice.RasterizerState = rs;
                }
            }

            for (int i = 0; i < enemiesAmount; i++)
            {
                enemies[i].Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
            }

            for (int i = 0; i < fishAmount; i++) {
                fish[i].Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
            }

            for (int i = 0; i < myBullet.Count; i++) {
                myBullet[i].draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
            }

            for (int i = 0; i < healthBullet.Count; i++) {
                healthBullet[i].draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
            }

                // Drawing ship wrecks
                foreach (ShipWreck shipWreck in shipWrecks)
                {
                    shipWreck.Draw(gameCamera.ViewMatrix,
                        gameCamera.ProjectionMatrix);
                    RasterizerState rs = new RasterizerState();
                    rs.FillMode = FillMode.WireFrame;
                    GraphicDevice.RasterizerState = rs;
                    shipWreck.DrawBoundingSphere(gameCamera.ViewMatrix,
                        gameCamera.ProjectionMatrix, boundingSphere);

                    rs = new RasterizerState();
                    rs.FillMode = FillMode.Solid;
                    GraphicDevice.RasterizerState = rs;
                }


            // Draw each plant
            foreach (Plant p in plants)
            {
                p.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, (float)((p.creationTime-roundTimer.TotalSeconds)/10.0));
                RasterizerState rs = new RasterizerState();
                rs.FillMode = FillMode.WireFrame;
                GraphicDevice.RasterizerState = rs;
                p.DrawBoundingSphere(gameCamera.ViewMatrix,
                    gameCamera.ProjectionMatrix, boundingSphere);

                rs = new RasterizerState();
                rs.FillMode = FillMode.Solid;
                GraphicDevice.RasterizerState = rs;
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
            DrawHeight();
            DrawRadar();
            if (tank.activeSkillID != -1) DrawActiveSkill();
        }

        private void DrawRadar()
        {
            radar.Draw(spriteBatch, tank.Position, enemies, enemiesAmount);
        }

        private void DrawHeight()
        {
            float xOffsetText, yOffsetText;
            string str1 = "Height: " +heightMapInfo.GetHeight(tank.Position);
            Rectangle rectSafeArea;
            Ray cursorRay = cursor.CalculateCursorRay(gameCamera.ProjectionMatrix, gameCamera.ViewMatrix);
            BoundingSphere boundingSphere;
            foreach (ShipWreck shipWreck in shipWrecks)
            {
                boundingSphere = shipWreck.BoundingSphere;
                boundingSphere.Center = shipWreck.Position;
                if (RayIntersectsBoudingSphere(cursorRay, boundingSphere))
                    str1 += " Pointing to ship wreck ";
            }
           
            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            xOffsetText = rectSafeArea.X;
            yOffsetText = rectSafeArea.Y;

            Vector2 strSize = statsFont.MeasureString(str1);
            Vector2 strPosition =
                new Vector2((int)xOffsetText + 200, (int)yOffsetText);

            //spriteBatch.Begin();
            spriteBatch.DrawString(statsFont, str1, strPosition, Color.White);
        }
        private void DrawStats()
        {
            float xOffsetText, yOffsetText;
            string str1 = GameConstants.StrTimeRemaining;
            string str2 = "";// = GameConstants.StrCellsFound + retrievedFruits.ToString() +
                //" of " + fruits.Count;
            Rectangle rectSafeArea;

            str1 += (roundTimer.Seconds).ToString();

            Vector3 pointIntersect = IntersectPointWithPlane(GameConstants.FloatHeight);
            Vector3 mouseDif = pointIntersect - tank.Position;
            float distanceFomTank = mouseDif.Length();
            str2 += "Xm= " + pointIntersect.X + " Ym= " + pointIntersect.Y + " Zm= " + pointIntersect.Z + " Distance from tank= " + distanceFomTank;
            str2 += "\nXt= " + tank.pointToMoveTo.X + " Yt= " + tank.pointToMoveTo.Y + " Zt= " + tank.pointToMoveTo.Z;
            str2 += "\nXc= " + tank.Position.X + " Yc= " + tank.Position.Y + " Zc= " + tank.Position.Z;
            float angle = CalculateAngle(pointIntersect, tank.Position);
            str2 += "\nAngle= " + tank.desiredAngle + "Tank FW= " + tank.ForwardDirection;
            Vector3 posDif = tank.pointToMoveTo - tank.Position;
            float distanceToDest = posDif.Length();
            str2 += "\nDistance= " + distanceToDest;
            str2 += "\n Type: " + tank.bulletType + "\n Strength up " + tank.strength;
            if (healthBullet.Count > 0 && enemiesAmount > 0)
            {
                str2 += "\n 1st Bullet Pos " + healthBullet[0].Position
                    + "\n Barrier pos " + enemies[0].Position;
            }
            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            xOffsetText = rectSafeArea.X;
            yOffsetText = rectSafeArea.Y;

            Vector2 strSize = statsFont.MeasureString(str1);
            Vector2 strPosition =
                new Vector2((int)xOffsetText + 10, (int)yOffsetText);

            spriteBatch.DrawString(statsFont, str1, strPosition, Color.White);
            strPosition.Y += strSize.Y;
            spriteBatch.DrawString(statsFont, str2, strPosition, Color.White);
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
        private static bool RayIntersectsBoudingSphere(Ray ray, BoundingSphere boundingSphere)
        {
            if (boundingSphere.Intersects(ray) != null)
            {
                return true;
            }
            return false;
        }
        public Vector3 IntersectPointWithPlane(float planeHeight)
        {
            Ray cursorRay = cursor.CalculateCursorRay(gameCamera.ProjectionMatrix, gameCamera.ViewMatrix);
            float t = (planeHeight - cursorRay.Position.Y) / cursorRay.Direction.Y;
            float x = cursorRay.Position.X + cursorRay.Direction.X * t;
            float z = cursorRay.Position.Z + cursorRay.Direction.Z * t;
            return new Vector3(x, planeHeight, z);
        }
        public float CalculateAngle(Vector3 point2, Vector3 point1)
        {
            return (float) Math.Atan2(point2.X - point1.X, point2.Z - point1.Z);
        }
    }
}
