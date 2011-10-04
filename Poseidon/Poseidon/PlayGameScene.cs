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
        public static GraphicsDeviceManager graphics;
        public static GraphicsDevice GraphicDevice;
        public static ContentManager Content;
        public static GameTime timming;

        Game game;
        KeyboardState lastKeyboardState = new KeyboardState();
        KeyboardState currentKeyboardState = new KeyboardState();
        GamePadState lastGamePadState = new GamePadState();
        GamePadState currentGamePadState = new GamePadState();
        MouseState currentMouseState = new MouseState();
        MouseState lastMouseState = new MouseState();

        public static AudioLibrary audio;
        int retrievedFruits;
        public TimeSpan startTime, roundTimer, roundTime;
        Random random;
        SpriteBatch spriteBatch;
        SpriteFont statsFont;
        GameObject ground;
        public Camera gameCamera;
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

        List<StaticObject> staticObjects;

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
        public string[] iconNames = { "Image/skill0Icon", "Image/skill1Icon", "Image/skill2Icon", "Image/skill3Icon" };
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

        // Frustum of the camera
        public static BoundingFrustum frustum;

        // For mouse inputs
        bool doubleClicked = false;
        bool clicked = false;
        double clickTimer = 0;

        private Texture2D stunnedTexture;

        public PlayGameScene(Game game, GraphicsDeviceManager graphic, ContentManager content, GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch, Vector2 pausePosition, Rectangle pauseRect, Texture2D actionTexture, CutSceneDialog cutSceneDialog, Radar radar, Texture2D stunnedTexture)
            : base(game)
        {
            graphics = graphic;
            Content = content;
            GraphicDevice = GraphicsDevice;
            this.spriteBatch = spriteBatch;
            this.pausePosition = pausePosition;
            this.pauseRect = pauseRect;
            this.actionTexture = actionTexture;
            this.game = game;
            this.cutSceneDialog = cutSceneDialog;
            this.radar = radar;
            this.stunnedTexture = stunnedTexture;
            roundTime = GameConstants.RoundTime;
            random = new Random();
            ground = new GameObject();
            gameCamera = new Camera();
            boundingSphere = new GameObject();
            tank = new Tank(GameConstants.MainGameMaxRangeX, GameConstants.MainGameMaxRangeZ, GameConstants.MainGameFloatHeight);
            prevTank = new Tank(GameConstants.MainGameMaxRangeX, GameConstants.MainGameMaxRangeZ, GameConstants.MainGameFloatHeight);
            fireTime = TimeSpan.FromSeconds(0.3f);

            enemies = new Enemy[GameConstants.NumberEnemies[currentLevel]];
            fish = new Fish[GameConstants.NumberFish[currentLevel]];

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

            //Uncomment below line to use LEVELS
            //string terrain_name = "Image/terrain" + currentLevel;
            
            //temporary code for testing
            Random random = new Random();
            int random_level = random.Next(20);
            string terrain_name = "Image/terrain" + random_level;
            System.Diagnostics.Debug.WriteLine(terrain_name);
            //end temporary testing code

            ground.Model = Content.Load<Model>(terrain_name);
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
            //Uncomment below line to use LEVELS
            //string terrain_name = "Image/terrain" + currentLevel;

            //temporary code for testing
            Random random = new Random();
            int random_level = random.Next(20);
            string terrain_name = "Image/terrain" + random_level;
            System.Diagnostics.Debug.WriteLine(terrain_name);
            //end temporary testing code

            ground.Model = Content.Load<Model>(terrain_name);

            // If we are resetting the level losing the game
            // Reset our tank to the one at the beginning of the lost level
            if (prevGameState == GameState.Lost) tank.CopyAttribute(prevTank);
            else prevTank.CopyAttribute(tank);

            tank.Reset();
            gameCamera.Update(tank.ForwardDirection,
                tank.Position, aspectRatio);

            

            //Clean all trees
            plants.Clear();

            //Clean all fruits
            fruits.Clear();


            retrievedFruits = 0;
            startTime = gameTime.TotalGameTime;
            roundTimer = roundTime;
            currentSentence = 0;
            currentGameState = GameState.PlayingCutScene;

            InitializeGameField(Content);
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
                else shipWrecks[index].LoadContent(Content, randomType, -1);
                randomType = random.Next(3);
            }
            enemiesAmount = 0;
            fishAmount = 0;
            enemies = new Enemy[GameConstants.NumberEnemies[currentLevel]];
            fish = new Fish[GameConstants.NumberFish[currentLevel]];
            AddingObjects.placeEnemies(ref enemiesAmount, enemies, Content, random, fishAmount, fish, shipWrecks,
                GameConstants.MainGameMinRangeX, GameConstants.MainGameMaxRangeX, GameConstants.MainGameMinRangeZ, GameConstants.MainGameMaxRangeZ, currentLevel, true, GameConstants.MainGameFloatHeight);
            AddingObjects.placeFish(ref fishAmount, fish, Content, random, enemiesAmount, enemies, shipWrecks,
                GameConstants.MainGameMinRangeX, GameConstants.MainGameMaxRangeX, GameConstants.MainGameMinRangeZ, GameConstants.MainGameMaxRangeZ, currentLevel, true, GameConstants.MainGameFloatHeight);
            //placeFuelCells();
            AddingObjects.placeShipWreck(shipWrecks, staticObjects, random, heightMapInfo,
                GameConstants.MainGameMinRangeX, GameConstants.MainGameMaxRangeX, GameConstants.MainGameMinRangeZ, GameConstants.MainGameMaxRangeZ);
            //Initialize the star fishes
            staticObjects = new List<StaticObject>(GameConstants.NumStaticObjects);
            for (int index = 0; index < GameConstants.NumStaticObjects; index++)
            {
                staticObjects.Add(new StaticObject());
                staticObjects[index].LoadContent(Content);
            }
            AddingObjects.PlaceStaticObjects(staticObjects, shipWrecks, random, heightMapInfo, GameConstants.MainGameMinRangeX, 
                GameConstants.MainGameMaxRangeX, GameConstants.MainGameMinRangeZ, GameConstants.MainGameMaxRangeZ);
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
                timming = gameTime;
                float aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
                lastKeyboardState = currentKeyboardState;
                //lastMouseState = currentMouseState;
                currentKeyboardState = Keyboard.GetState();
                lastGamePadState = currentGamePadState;
                currentGamePadState = GamePad.GetState(PlayerIndex.One);
                //currentMouseState = Mouse.GetState();
                // Allows the game to exit
                //if ((currentKeyboardState.IsKeyDown(Keys.Escape)) ||
                //    (currentGamePadState.Buttons.Back == ButtonState.Pressed))
                //    //this.Exit();
                CursorManager.CheckClick(ref lastMouseState, ref currentMouseState, gameTime, ref clickTimer, ref clicked, ref doubleClicked);

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
                    Vector3 pointIntersect = Vector3.Zero;
                    bool mouseOnLivingObject = CursorManager.MouseOnEnemy(cursor, gameCamera, enemies, enemiesAmount) || CursorManager.MouseOnFish(cursor, gameCamera, fish, fishAmount);
                    //if the user holds down Shift button
                    //let him change current bullet or skill type w/o moving
                    if (currentKeyboardState.IsKeyDown(Keys.RightShift) || currentKeyboardState.IsKeyDown(Keys.LeftShift))
                    {
                        // changing bullet type
                        if ((lastKeyboardState.IsKeyDown(Keys.LeftShift) || lastKeyboardState.IsKeyDown(Keys.RightShift)) && ((lastKeyboardState.IsKeyDown(Keys.L)
                                && currentKeyboardState.IsKeyUp(Keys.L)) || (lastMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released)))
                        {
                            tank.bulletType++;
                            if (tank.bulletType == GameConstants.numBulletTypes) tank.bulletType = 0;

                        }
                        // changing active skill
                        if ((lastKeyboardState.IsKeyDown(Keys.LeftShift) || lastKeyboardState.IsKeyDown(Keys.RightShift)) && ((lastKeyboardState.IsKeyDown(Keys.K)
                                && currentKeyboardState.IsKeyUp(Keys.K)) || (lastMouseState.RightButton == ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Released)))
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
                        //if the user wants to move when changing skill or bullet, let him
                        //because this is better for fast action game
                        if (currentMouseState.LeftButton == ButtonState.Pressed)
                        {
                            pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);
                        }
                    }
                    //if the user click on right mouse button
                    //cast the current selected skill
                    //else if (lastMouseState.RightButton == ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Released)
                    else if (currentMouseState.RightButton == ButtonState.Pressed)
                    {

                        // Hercules' Bow!!!
                        if (tank.activeSkillID == 0 && mouseOnLivingObject)
                        {
                            pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);
                            tank.ForwardDirection = CursorManager.CalculateAngle(pointIntersect, tank.Position);
                            //if the skill has cooled down
                            //or this is the 1st time the user uses it
                            if ((gameTime.TotalGameTime.TotalSeconds - tank.skillPrevUsed[0] > GameConstants.coolDownForHerculesBow) || tank.firstUse[0] == true)
                            {
                                tank.firstUse[0] = false;
                                tank.skillPrevUsed[0] = gameTime.TotalGameTime.TotalSeconds;
                                audio.Explosion.Play();
                                CastSkill.UseHerculesBow(tank, Content, myBullet);
                            }

                        }
                        //Thor's Hammer!!!
                        if (tank.activeSkillID == 1)
                        {
                            if ((gameTime.TotalGameTime.TotalSeconds - tank.skillPrevUsed[1] > GameConstants.coolDownForArchillesArmor) || tank.firstUse[1] == true)
                            {
                                tank.firstUse[1] = false;
                                tank.skillPrevUsed[1] = gameTime.TotalGameTime.TotalSeconds;
                                audio.Explosion.Play();
                                CastSkill.UseThorHammer(gameTime, tank, enemies, ref enemiesAmount);
                            }
                        }
                        // Achilles' Armor!!!
                        if (tank.activeSkillID == 2)
                        {
                            if ((gameTime.TotalGameTime.TotalSeconds - tank.skillPrevUsed[2] > GameConstants.coolDownForThorHammer) || tank.firstUse[2] == true)
                            {
                                tank.firstUse[2] = false;
                                tank.invincibleMode = true;
                                audio.NewMeteor.Play();
                                tank.skillPrevUsed[2] = gameTime.TotalGameTime.TotalSeconds;
                            }
                        }

                        //Hermes' Winged Sandal!!!
                        if (tank.activeSkillID == 3)
                        {
                            if ((gameTime.TotalGameTime.TotalSeconds - tank.skillPrevUsed[3] > GameConstants.coolDownForHermesSandle) || tank.firstUse[3] == true)
                            {
                                tank.firstUse[3] = false;
                                audio.NewMeteor.Play();
                                tank.skillPrevUsed[3] = gameTime.TotalGameTime.TotalSeconds;
                                tank.supersonicMode = true;
                            }
                        }
                        pointIntersect = Vector3.Zero;
                    }
                    //if the user holds down Ctrl button
                    //just shoot at wherever the mouse is pointing w/o moving
                    else if (currentKeyboardState.IsKeyDown(Keys.RightControl) || currentKeyboardState.IsKeyDown(Keys.LeftControl))
                    {
                        if (currentMouseState.LeftButton == ButtonState.Pressed)
                        {
                            pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);
                            tank.ForwardDirection = CursorManager.CalculateAngle(pointIntersect, tank.Position);
                            if (gameTime.TotalGameTime.TotalSeconds - prevFireTime.TotalSeconds > fireTime.TotalSeconds / (tank.shootingRate * tank.fireRateUp))
                            {
                                prevFireTime = gameTime.TotalGameTime;
                                audio.Shooting.Play();
                                if (tank.bulletType == 0) { AddingObjects.placeDamageBullet(tank, Content, myBullet); }
                                else if (tank.bulletType == 1) { AddingObjects.placeHealingBullet(tank, Content, healthBullet); }
                            }
                        }
                        pointIntersect = Vector3.Zero;
                    }

                    //if the user clicks or holds mouse's left button
                    else if (currentMouseState.LeftButton == ButtonState.Pressed && !mouseOnLivingObject)
                    {
                        pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);
                    }
                    else if (lastMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released)
                    {
                        pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);
                        //if it is out of shooting range then just move there
                        if (!CursorManager.InShootingRange(tank, cursor, gameCamera, GameConstants.MainGameFloatHeight))
                        {

                        }
                        else
                        {
                            //if the enemy is in the shooting range then shoot it w/o moving to it
                            if (mouseOnLivingObject && gameTime.TotalGameTime.TotalSeconds - prevFireTime.TotalSeconds > fireTime.TotalSeconds / (tank.shootingRate * tank.fireRateUp))
                            {
                                tank.ForwardDirection = CursorManager.CalculateAngle(pointIntersect, tank.Position);
                                prevFireTime = gameTime.TotalGameTime;
                                audio.Shooting.Play();
                                if (tank.bulletType == 0) { AddingObjects.placeDamageBullet(tank, Content, myBullet); }
                                else if (tank.bulletType == 1) { AddingObjects.placeHealingBullet(tank, Content, healthBullet); }
                                //so the tank will not move
                                pointIntersect = Vector3.Zero;
                            }
                            if (doubleClicked == true) pointIntersect = Vector3.Zero;
                        }
                    }
                    //let the user change active skill/bullet too when he presses on number
                    //this is better for fast action
                    InputManager.ChangeSkillBulletWithKeyBoard(lastKeyboardState, currentKeyboardState, tank);

                    if (tank.supersonicMode == true)
                    {
                        pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);
                        CastSkill.KnockOutEnemies(gameTime, tank, enemies, ref enemiesAmount, audio);
                    }
                    if (!heightMapInfo.IsOnHeightmap(pointIntersect)) pointIntersect = Vector3.Zero;
                    tank.Update(currentKeyboardState, enemies, enemiesAmount, fish, fishAmount, fruits, gameTime, pointIntersect);
                    

                    // Are we shooting?
                    if (!(lastKeyboardState.IsKeyDown(Keys.LeftShift) || lastKeyboardState.IsKeyDown(Keys.RightShift))
                        && currentKeyboardState.IsKeyDown(Keys.L)
                        && gameTime.TotalGameTime.TotalSeconds - prevFireTime.TotalSeconds > fireTime.TotalSeconds / (tank.shootingRate * tank.fireRateUp))
                        //||
                        //( (MouseOnEnemy()||MouseOnFish()) && lastMouseState.LeftButton==ButtonState.Pressed && currentMouseState.LeftButton==ButtonState.Released && InShootingRange())
                    {
                        prevFireTime = gameTime.TotalGameTime;
                        audio.Shooting.Play();
                        if (tank.bulletType == 0) { AddingObjects.placeDamageBullet(tank, Content, myBullet); }
                        else if (tank.bulletType == 1) { AddingObjects.placeHealingBullet(tank, Content, healthBullet); }
                    }

                    
                    //Are we planting trees?
                    if ((lastKeyboardState.IsKeyDown(Keys.O) && (currentKeyboardState.IsKeyUp(Keys.O))))
                    {
                        audio.Shooting.Play();
                        AddingObjects.placePlant(tank, heightMapInfo, Content, roundTimer, plants, shipWrecks, staticObjects);
                    }

                    //Are the trees ready for fruit?
                    foreach (Plant plant in plants)
                    {
                        if (plant.timeForFruit == true )
                        {
                            int powerType = random.Next(3) + 1;
                            Fruit fruit = new Fruit(powerType);
                            fruits.Add(fruit);
                            fruit.LoadContent(Content, "Models/fuelcell", plant.Position);
                            plant.timeForFruit = false;
                            plant.fruitCreated++;
                        }
                    }

                    gameCamera.Update(tank.ForwardDirection, tank.Position, aspectRatio);
                    // Updating camera's frustum
                    frustum = new BoundingFrustum(gameCamera.ViewMatrix * gameCamera.ProjectionMatrix);

                    retrievedFruits = 0;
                    foreach (Fruit fruit in fruits)
                    {
                        fruit.Update(currentKeyboardState, tank.BoundingSphere, tank.Trash_Fruit_BoundingSphere);
                        if (fruit.Retrieved) {
                            retrievedFruits++;
                        }
                    }

                    for (int i = 0; i < myBullet.Count; i++)
                    {
                        myBullet[i].update();
                    }

                    for (int i = 0; i < healthBullet.Count; i++)
                    {
                        healthBullet[i].update();
                    }

                    for (int i = 0; i < enemyBullet.Count; i++) {
                        enemyBullet[i].update();
                    }
                    Collision.updateBulletOutOfBound(tank.MaxRangeX, tank.MaxRangeZ, healthBullet, myBullet, frustum);
                    Collision.updateDamageBulletVsBarriersCollision(myBullet, enemies, ref enemiesAmount);
                    Collision.updateHealingBulletVsBarrierCollision(healthBullet, fish, fishAmount);
                    Collision.updateDamageBulletVsBarriersCollision(enemyBullet, fish, ref fishAmount);
                    Collision.updateProjectileHitTank(tank, enemyBullet);

                    for (int i = 0; i < enemiesAmount; i++) {
                        if (!enemies[i].stunned)
                            enemies[i].Update(enemies, enemiesAmount, fish, fishAmount, random.Next(100), tank, enemyBullet);
                        //disable stun if stun effect times out
                        else
                        {
                            if (gameTime.TotalGameTime.TotalSeconds - enemies[i].stunnedStartTime > GameConstants.timeStunLast)
                                enemies[i].stunned = false;
                        }
                    }

                    for (int i = 0; i < fishAmount; i++) {
                        fish[i].Update(enemies, enemiesAmount, fish, fishAmount, random.Next(100), tank, enemyBullet);
                    }

                    //Checking win/lost condition for this level
                    if (tank.currentHitPoint <= 0) { currentGameState = GameState.Lost; }

                    roundTimer -= gameTime.ElapsedGameTime;
                    if (CheckWinCondition()) currentGameState = GameState.Won;
                    if (CheckLoseCondition()) currentGameState = GameState.Lost;
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
            // Updating camera's frustum
            frustum = new BoundingFrustum(gameCamera.ViewMatrix * gameCamera.ProjectionMatrix);
            foreach (Fruit f in fruits)
            {
                if (!f.Retrieved && f.BoundingSphere.Intersects(frustum))
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
                if (enemies[i].BoundingSphere.Intersects(frustum))
                {
                    enemies[i].Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
                    if (enemies[i].stunned == true)
                    {
                        Vector3 placeToDraw = game.GraphicsDevice.Viewport.Project(enemies[i].Position, gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity);
                        Vector2 drawPos = new Vector2(placeToDraw.X, placeToDraw.Y);
                        spriteBatch.Draw(stunnedTexture, drawPos, Color.White);
                    }
                }
            }

            for (int i = 0; i < fishAmount; i++)
            {
                if (fish[i].BoundingSphere.Intersects(frustum))
                    fish[i].Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
            }

            for (int i = 0; i < myBullet.Count; i++)
            {
                myBullet[i].draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
            }

            for (int i = 0; i < healthBullet.Count; i++)
            {
                healthBullet[i].draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
            }

            for (int i = 0; i < enemyBullet.Count; i++) {
                enemyBullet[i].draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
            }

            BoundingSphere shipSphere;
            // Drawing ship wrecks
            foreach (ShipWreck shipWreck in shipWrecks)
            {
                shipSphere = shipWreck.BoundingSphere;
                shipSphere.Center = shipWreck.Position;
                if (shipSphere.Intersects(frustum))
                {
                    shipWreck.Draw(gameCamera.ViewMatrix,
                        gameCamera.ProjectionMatrix);
                    //RasterizerState rs = new RasterizerState();
                    //rs.FillMode = FillMode.WireFrame;
                    //GraphicDevice.RasterizerState = rs;
                    //shipWreck.DrawBoundingSphere(gameCamera.ViewMatrix,
                    //    gameCamera.ProjectionMatrix, boundingSphere);

                    //rs = new RasterizerState();
                    //rs.FillMode = FillMode.Solid;
                    //GraphicDevice.RasterizerState = rs;
                }
            }

            // Draw each plant
            foreach (Plant p in plants) {
                if (p.BoundingSphere.Intersects(frustum))
                {
                    p.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, (float)((p.creationTime - roundTimer.TotalSeconds) / 10.0));
                    RasterizerState rs = new RasterizerState();
                    rs.FillMode = FillMode.WireFrame;
                    GraphicDevice.RasterizerState = rs;
                    p.DrawBoundingSphere(gameCamera.ViewMatrix,
                        gameCamera.ProjectionMatrix, boundingSphere);

                    rs = new RasterizerState();
                    rs.FillMode = FillMode.Solid;
                    GraphicDevice.RasterizerState = rs;
                }
            }
            //Draw each starfish
            foreach (StaticObject staticObject in staticObjects)
            {
                if (staticObject.BoundingSphere.Intersects(frustum))
                {
                    staticObject.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
                    RasterizerState rs = new RasterizerState();
                    rs.FillMode = FillMode.WireFrame;
                    GraphicDevice.RasterizerState = rs;
                    staticObject.DrawBoundingSphere(gameCamera.ViewMatrix,
                        gameCamera.ProjectionMatrix, boundingSphere);

                    rs = new RasterizerState();
                    rs.FillMode = FillMode.Solid;
                    GraphicDevice.RasterizerState = rs;
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
            DrawHeight();
            DrawRadar();
            if (tank.activeSkillID != -1) DrawActiveSkill();
        }

        private void DrawRadar()
        {
            radar.Draw(spriteBatch, tank.Position, enemies, enemiesAmount, staticObjects);
        }

        public bool CharacterNearShipWreck(BoundingSphere shipSphere)
        {
            if (tank.BoundingSphere.Intersects(shipSphere))
                return true;
            else
                return false;
        }

        private void DrawHeight()
        {
            float xOffsetText, yOffsetText;
            string str1 = "Height: " + heightMapInfo.GetHeight(tank.Position);
            Rectangle rectSafeArea;
            Ray cursorRay = cursor.CalculateCursorRay(gameCamera.ProjectionMatrix, gameCamera.ViewMatrix);
            BoundingSphere boundingSphere;
            foreach (ShipWreck shipWreck in shipWrecks)
            {
                boundingSphere = shipWreck.BoundingSphere;
                boundingSphere.Center = shipWreck.Position;
                if (CursorManager.RayIntersectsBoundingSphere(cursorRay, boundingSphere))
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
            if (roundTimer.Minutes < 10)
                str1 += "0";
            str1 += roundTimer.Minutes + ":";
            if (roundTimer.Seconds < 10)
                str1+= "0";
            str1 += roundTimer.Seconds;
            str2 += "Player's health: " + tank.currentHitPoint + "/" + tank.maxHitPoint; 
            //Vector3 pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.FloatHeight);
            //Vector3 mouseDif = pointIntersect - tank.Position;
            //float distanceFomTank = mouseDif.Length();
            //str2 += "Xm= " + pointIntersect.X + " Ym= " + pointIntersect.Y + " Zm= " + pointIntersect.Z + " Distance from tank= " + distanceFomTank;
            //str2 += "\nXt= " + tank.pointToMoveTo.X + " Yt= " + tank.pointToMoveTo.Y + " Zt= " + tank.pointToMoveTo.Z;
            //float angle = CursorManager.CalculateAngle(pointIntersect, tank.Position);
            //str2 += "\nAngle= " + tank.desiredAngle + "Tank FW= " + tank.ForwardDirection;
            //Vector3 posDif = tank.pointToMoveTo - tank.Position;
            //float distanceToDest = posDif.Length();
            //str2 += "\nDistance= " + distanceToDest;
            //str2 += "\nTank Position " + tank.Position;
            //str2 += "\nEnemy Position " + enemies[0].Position;
            //str2 += "\nTank Forward Direction " + tank.ForwardDirection;
            //str2 += "\nEnemy FW " + enemies[0].ForwardDirection;
            //str2 += "\nPrevFIre " + enemies[0].prevFire;
            str2 += "\n Type " + tank.GetType().Name.ToString();
            
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

    }
}
