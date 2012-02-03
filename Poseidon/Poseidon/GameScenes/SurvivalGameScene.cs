using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Poseidon.Core;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Poseidon.FishSchool;
using System.IO;


namespace Poseidon
{
    /// <summary>
    /// This is a game component that implements the Action Scene.
    /// </summary>
    public partial class SurvivalGameScene : GameScene
    {
        public static GraphicsDeviceManager graphics;
        public static GraphicsDevice GraphicDevice;
        public static ContentManager Content;
        //public static GameTime timming;

        private Texture2D HealthBar;
        private Texture2D EnvironmentBar;

        Game game;
        KeyboardState lastKeyboardState = new KeyboardState();
        KeyboardState currentKeyboardState = new KeyboardState();
        MouseState currentMouseState = new MouseState();
        MouseState lastMouseState = new MouseState();

        public static AudioLibrary audio;

        public TimeSpan startTime, roundTimer, roundTime;
        Random random;
        SpriteBatch spriteBatch;
        SpriteFont statsFont;
        SpriteFont fishTalkFont;

        SpriteFont menuSmall;
        
        public static Camera gameCamera;
        public GameState currentGameState = GameState.Running;
        // In order to know we are resetting the level winning or losing
        // winning: keep the current bot
        // losing: reset our bot to the bot at the beginning of the level
        GameState prevGameState;
        GameObject boundingSphere;

        Terrain terrain;

        public List<DamageBullet> myBullet;
        public List<DamageBullet> alliesBullets;
        public List<DamageBullet> enemyBullet;
        public List<HealthBullet> healthBullet;

        List<Powerpack> powerpacks;
        List<Resource> resources;
        List<Trash> trashes;
        List<Factory> factories;
        ResearchFacility researchFacility;

        public BaseEnemy[] enemies;
        public Fish[] fish;
        public int enemiesAmount = 0;
        public int fishAmount = 0;

        // The main character for this level
        public HydroBot hydroBot;


        // Game is paused?
        protected bool paused;
        protected Vector2 pausePosition;
        protected Rectangle pauseRect = new Rectangle(1, 120, 200, 44);
        protected Texture2D actionTexture;

        // For drawing the currently selected skill
        protected Texture2D[] skillTextures;
        // For drawing the currently selected bullet type
        protected Texture2D[] bulletTypeTextures;


        Radar radar;

        // Frustum of the camera
        public BoundingFrustum frustum;


        private Texture2D stunnedTexture;

        // shader for underwater effect
        // Our Post Process effect object, this is where our shader will be loaded and compiled
        Effect underWaterEffect;
        float m_Timer = 0;
        RenderTarget2D renderTarget;
        Texture2D SceneTexture;

        // Bubbles over characters
        List<Bubble> bubbles;
        float timeNextBubble = 200.0f;
        float timeNextSeaBedBubble = 3000.0f;

        //Points gained over trash, plant, enemies, fish
        public static List<Point> points;

        //for drawing winning or losing scenes
        Texture2D winningTexture, losingTexture;

        //score: using hydrobot gained exp as score
        public static float score = 0;

        //is ancient fish killed
        public static bool isAncientKilled;

        // For mouse inputs
        bool doubleClicked = false;
        bool clicked = false;
        double clickTimer = 0;

        private bool openFactoryConfigurationScene = false;
        private bool openResearchFacilityConfigScene = false;
        private Factory factoryToConfigure;

        public SurvivalGameScene(Game game, GraphicsDeviceManager graphic, ContentManager content, GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch, Vector2 pausePosition, Rectangle pauseRect, Texture2D actionTexture, CutSceneDialog cutSceneDialog, Radar radar, Texture2D stunnedTexture)
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
            this.radar = radar;
            this.stunnedTexture = stunnedTexture;
            roundTime = TimeSpan.FromSeconds(2592000);
            random = new Random();
            
            gameCamera = new Camera(GameConstants.MainCamHeight);
            boundingSphere = new GameObject();
            hydroBot = new HydroBot(GameConstants.MainGameMaxRangeX, GameConstants.MainGameMaxRangeZ, GameConstants.MainGameFloatHeight, GameMode.SurvivalMode);

            if (File.Exists("SurvivalMode"))
            {
                ObjectsToSerialize objectsToSerialize = new ObjectsToSerialize();
                Serializer serializer = new Serializer();
                string SavedFile = "SurvivalMode";
                objectsToSerialize = serializer.DeSerializeObjects(SavedFile);
                hydroBot = objectsToSerialize.hydrobot;
            }

            //stop spinning the bar
            IngamePresentation.StopSpinning();

            HydroBot.gamePlusLevel = 0;
            hydroBot.gameMode = GameMode.SurvivalMode;

            skillTextures = new Texture2D[GameConstants.numberOfSkills];
            bulletTypeTextures = new Texture2D[GameConstants.numBulletTypes];

            // for the mouse or touch
            cursor = new Cursor(game, spriteBatch);
            //Components.Add(cursor);

            myBullet = new List<DamageBullet>();
            healthBullet = new List<HealthBullet>();
            enemyBullet = new List<DamageBullet>();
            alliesBullets = new List<DamageBullet>();

            bubbles = new List<Bubble>();
            points = new List<Point>();


            //loading winning, losing textures
            winningTexture = Content.Load<Texture2D>("Image/SceneTextures/LevelWin");
            losingTexture = Content.Load<Texture2D>("Image/SceneTextures/GameOver");

            isAncientKilled = false;

            this.Load();

        }

        public void Load()
        {
            statsFont = Content.Load<SpriteFont>("Fonts/StatsFont");
            menuSmall = Content.Load<SpriteFont>("Fonts/menuSmall");
            fishTalkFont = Content.Load<SpriteFont>("Fonts/fishTalk");

            // Get the audio library
            audio = (AudioLibrary)
                Game.Services.GetService(typeof(AudioLibrary));

            terrain = new Terrain(Content);

            // Loading main character skill icon textures
            for (int index = 0; index < GameConstants.numberOfSkills; index++)
            {
                skillTextures[index] = Content.Load<Texture2D>(GameConstants.iconNames[index]);
            }

            // Loading main character bullet icon textures
            for (int index = 0; index < GameConstants.numBulletTypes; index++)
            {
                bulletTypeTextures[index] = Content.Load<Texture2D>(GameConstants.bulletNames[index]);
            }

            //Initialize the game field
            InitializeGameField(Content);

            powerpacks = new List<Powerpack>();
            resources = new List<Resource>();

            powerpacks = new List<Powerpack>();
            resources = new List<Resource>();

            hydroBot.Load(Content);

            //prevTank.Load(Content);
            roundTimer = roundTime;

            //Load healthbar
            HealthBar = Content.Load<Texture2D>("Image/Miscellaneous/HealthBar");
            EnvironmentBar = Content.Load<Texture2D>("Image/Miscellaneous/EnvironmentBar");

            // Load and compile our Shader into our Effect instance.
            underWaterEffect = Content.Load<Effect>("Shaders/UnderWater");
            PresentationParameters pp = graphics.GraphicsDevice.PresentationParameters;
            renderTarget = new RenderTarget2D(graphics.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight,
                false, graphics.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24Stencil8);
        }

        /// <summary>
        /// Show the action scene
        /// </summary>
        public override void Show()
        {
            paused = false;
            base.Show();
        }

        private void ResetGame(GameTime gameTime, float aspectRatio)
        {
            cursor.targetToLock = null;
            MediaPlayer.Stop();
            roundTime = TimeSpan.FromSeconds(2592000);
            roundTimer = roundTime;

            terrain = new Terrain(Content);
            // If we are resetting the level losing the game
            // Reset our bot to the one at the beginning of the lost level


            hydroBot.Reset();

            gameCamera.Update(hydroBot.ForwardDirection,
                hydroBot.Position, aspectRatio, gameTime);


            //Clean all fruits and resources
            powerpacks.Clear();
            resources.Clear();

            startTime = gameTime.TotalGameTime;

            currentGameState = GameState.Running;

            InitializeGameField(Content);
        }

        private void InitializeGameField(ContentManager Content)
        {
            enemyBullet = new List<DamageBullet>();
            healthBullet = new List<HealthBullet>();
            myBullet = new List<DamageBullet>();
            alliesBullets = new List<DamageBullet>();

            float orientation = random.Next(100);
          
            enemiesAmount = 0;
            fishAmount = 0;
            enemies = new BaseEnemy[GameConstants.SurvivalModeMaxShootingEnemy + GameConstants.SurvivalModeMaxCombatEnemy
                + GameConstants.SurvivalModeMaxMutantShark + GameConstants.SurvivalModeMaxTerminator];
            fish = new Fish[1];

            AddingObjects.placeEnemies(ref enemiesAmount, enemies, Content, random, fishAmount, fish, null,
                GameConstants.MainGameMinRangeX, GameConstants.MainGameMaxRangeX, GameConstants.MainGameMinRangeZ, GameConstants.MainGameMaxRangeZ, -1, GameMode.SurvivalMode, GameConstants.MainGameFloatHeight);

            AddingObjects.placeFish(ref fishAmount, fish, Content, random, enemiesAmount, enemies, null,
                GameConstants.MainGameMinRangeX, GameConstants.MainGameMaxRangeX, GameConstants.MainGameMinRangeZ, GameConstants.MainGameMaxRangeZ, -1, GameMode.SurvivalMode, GameConstants.MainGameFloatHeight);

            //Initialize trash
            //int random_model;
            //int numberTrash = GameConstants.NumberBioTrash[currentLevel] + GameConstants.NumberNuclearTrash[currentLevel] + GameConstants.NumberPlasticTrash[currentLevel];
            int numberTrash = 100; // 45, 45, 10
            trashes = new List<Trash>(numberTrash);
            int bioIndex, plasticIndex, nuclearIndex;
            for (bioIndex = 0; bioIndex < 45; bioIndex++)
            {
                orientation = random.Next(100);
                trashes.Add(new Trash(TrashType.plastic));
                trashes[bioIndex].LoadContent(Content, "Models/TrashModels/trashModel1", orientation); //bio model
            }
            for (plasticIndex = bioIndex; plasticIndex < bioIndex + 45; plasticIndex++)
            {
                orientation = random.Next(100);
                trashes.Add(new Trash(TrashType.radioactive));
                trashes[plasticIndex].LoadContent(Content, "Models/TrashModels/trashModel2", orientation); //plastic model
            }
            for (nuclearIndex = plasticIndex; nuclearIndex < plasticIndex + 10; nuclearIndex++)
            {
                orientation = random.Next(100);
                trashes.Add(new Trash(TrashType.biodegradable));
                trashes[nuclearIndex].LoadContent(Content, "Models/TrashModels/trashModel3", orientation); //nuclear model
            }

            AddingObjects.placeTrash(ref trashes, Content, random, null, null,
                GameConstants.MainGameMinRangeX, GameConstants.MainGameMaxRangeX, GameConstants.MainGameMinRangeZ,
                GameConstants.MainGameMaxRangeZ, GameMode.MainGame, GameConstants.MainGameFloatHeight, terrain.heightMapInfo); 

            //Create 3 trash processing factories at the beginning
            //JUST FOR TESTING .. REMOVE WHEN THE FACTORY CREATION MENU IS AVAILABLE (SUSHIL)
            Vector3 position;
            factories = new List<Factory>();

            //create research facility
            researchFacility = new ResearchFacility(); //There can be only 1 research facility.
            position = new Vector3(0, 0, -100);
            position.Y = terrain.heightMapInfo.GetHeight(new Vector3(position.X, 0, position.Z));
            orientation = random.Next(100);
            researchFacility.LoadContent(Content, game, "Models/FactoryModels/ResearchFacility", position, orientation);
            HydroBot.numResources -= GameConstants.numResourcesForEachFactory;

            factories.Add(new Factory(FactoryType.biodegradable));
            position = new Vector3(100, 0, 0);
            position.Y = terrain.heightMapInfo.GetHeight(new Vector3(position.X, 0, position.Z));
            orientation = random.Next(100);
            factories[0].LoadContent(Content, game, "Models/FactoryModels/BiodegradableFactory", position, orientation);
            HydroBot.numResources -= GameConstants.numResourcesForEachFactory;

            factories.Add(new Factory(FactoryType.plastic));
            position = new Vector3(0, 0, 0);
            position.Y = terrain.heightMapInfo.GetHeight(new Vector3(position.X, 0, position.Z));
            orientation = random.Next(100);
            factories[1].LoadContent(Content, game, "Models/FactoryModels/PlasticFactory", position, orientation);
            HydroBot.numResources -= GameConstants.numResourcesForEachFactory;

            factories.Add(new Factory(FactoryType.radioactive));
            position = new Vector3(-100, 0, 0);
            position.Y = terrain.heightMapInfo.GetHeight(new Vector3(position.X, 0, position.Z));
            orientation = random.Next(100);
            factories[2].LoadContent(Content, game, "Models/FactoryModels/NuclearFactory", position, orientation);
            HydroBot.numResources -= GameConstants.numResourcesForEachFactory;
        }

        /// <summary>
        /// Hide the scene
        /// </summary>
        public override void Hide()
        {
            // Stop the background music
            //MediaPlayer.Stop();
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
            if (MediaPlayer.State.Equals(MediaState.Stopped))
            {
                MediaPlayer.Play(audio.backgroundMusics[random.Next(GameConstants.NumNormalBackgroundMusics)]);
            }
            if (!paused)
            {
                //timming = gameTime;
                float aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
                lastKeyboardState = currentKeyboardState;
                currentKeyboardState = Keyboard.GetState();

                //currentMouseState = Mouse.GetState();
                // Allows the game to exit
                //if ((currentKeyboardState.IsKeyDown(Keys.Escape)) ||
                //    (currentGamePadState.Buttons.Back == ButtonState.Pressed))
                //    //this.Exit();

                if ((currentGameState == GameState.Running))
                {
                    MouseState currentMouseState;
                    currentMouseState = Mouse.GetState();
                    if (currentMouseState.RightButton == ButtonState.Pressed) //Also need to check for position
                    {
                        foreach (Factory factory in factories)
                        {
                            if (CursorManager.MouseOnObject(cursor, factory.BoundingSphere, factory.Position, gameCamera))
                            {
                                openFactoryConfigurationScene = true;
                                factoryToConfigure = factory;
                                break;
                            }
                        }
                        if (CursorManager.MouseOnObject(cursor, researchFacility.BoundingSphere, researchFacility.Position, gameCamera))
                        {
                            openResearchFacilityConfigScene = true;
                        }
                    }
                    if (openFactoryConfigurationScene || openResearchFacilityConfigScene)
                    {
                        bool exitFactConfPressed;
                        exitFactConfPressed = (lastKeyboardState.IsKeyDown(Keys.Enter) && (currentKeyboardState.IsKeyUp(Keys.Enter)));
                        if (exitFactConfPressed)
                        {
                            openFactoryConfigurationScene = false;
                            openResearchFacilityConfigScene = false;
                        }
                        else
                        {
                            //cursor update
                            cursor.Update(GraphicDevice, gameCamera, gameTime, frustum);
                            CursorManager.CheckClick(ref this.lastMouseState, ref this.currentMouseState, gameTime, ref clickTimer, ref clicked, ref doubleClicked);
                            if (clicked)
                            {
                                if (openFactoryConfigurationScene)
                                {
                                    if (factoryToConfigure.produceRect.Intersects(new Rectangle(lastMouseState.X, lastMouseState.Y, 10, 10)))
                                        factoryToConfigure.SwitchProductionItem();
                                }
                                else
                                {
                                    if (researchFacility.bioUpgrade && researchFacility.bioUpgradeRect.Intersects(new Rectangle(lastMouseState.X, lastMouseState.Y, 10, 10)))
                                        researchFacility.UpgradeBioFactory(factories);
                                    if (researchFacility.plasticUpgrade && researchFacility.plasticUpgradeRect.Intersects(new Rectangle(lastMouseState.X, lastMouseState.Y, 10, 10)))
                                        researchFacility.UpgradePlasticFactory(factories);
                                }
                                clicked = false;
                            }
                            return;
                        }
                    }

                    //hydrobot update
                    hydroBot.UpdateAction(gameTime, cursor, gameCamera, enemies, enemiesAmount, fish, fishAmount, Content, spriteBatch, myBullet, this, terrain.heightMapInfo, healthBullet, powerpacks, resources, null, null,null);

                    //add 1 bubble over bot and each enemy
                    timeNextBubble -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (timeNextBubble <= 0)
                    {
                        Bubble bubble = new Bubble();
                        bubble.LoadContent(Content, hydroBot.Position, false, 0.025f);
                        bubbles.Add(bubble);
                        for (int i = 0; i < enemiesAmount; i++)
                        {
                            if (enemies[i].BoundingSphere.Intersects(frustum) && !(enemies[i] is MutantShark))
                            {
                                Bubble aBubble = new Bubble();
                                aBubble.LoadContent(Content, enemies[i].Position, false, 0.025f);
                                bubbles.Add(aBubble);
                            }
                        }

                        timeNextBubble = 200.0f;
                    }

                    //randomly generate few bubbles from sea bed
                    timeNextSeaBedBubble -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (timeNextSeaBedBubble <= 0)
                    {
                        Vector3 bubblePos = hydroBot.Position;
                        bubblePos.X += random.Next(-50, 50);
                        //bubblePos.Y = 0;
                        bubblePos.Z += random.Next(-50, 50);
                        for (int i = 0; i < 7; i++)
                        {
                            Bubble aBubble = new Bubble();
                            bubblePos.X += random.Next(-2, 2);
                            bubblePos.Z += random.Next(-2, 2);
                            aBubble.LoadContent(Content, bubblePos, true, (float)random.NextDouble() / 80);
                            bubbles.Add(aBubble);
                        }
                        //audio.Bubble.Play();
                        timeNextSeaBedBubble = (random.Next(3) + 3) * 1000.0f;
                    }
                    for (int i = 0; i < bubbles.Count; i++)
                    {
                        if (bubbles[i].timeLast <= 0)
                            bubbles.RemoveAt(i--);
                        else if (bubbles[i].scale >= 0.06)
                            bubbles.RemoveAt(i--);
                    }

                    foreach (Bubble aBubble in bubbles)
                    {
                        if (random.Next(100) >= 95) aBubble.bubble3DPos.X += 0.5f;
                        else if (random.Next(100) >= 95) aBubble.bubble3DPos.X -= 0.5f;
                        if (random.Next(100) >= 95) aBubble.bubble3DPos.Z += 0.5f;
                        else if (random.Next(100) >= 95) aBubble.bubble3DPos.Z -= 0.5f;
                        aBubble.Update(GraphicDevice, gameCamera, gameTime);
                    }

                    //update points
                    for (int i = 0; i < points.Count; i++)
                    {
                        Point point = points[i];
                        if (point.toBeRemoved)
                            points.Remove(point);
                    }
                    foreach (Point point in points)
                    {
                        point.Update(GraphicDevice, gameCamera, gameTime);
                    }

                    gameCamera.Update(hydroBot.ForwardDirection, hydroBot.Position, aspectRatio, gameTime);
                    // Updating camera's frustum
                    frustum = new BoundingFrustum(gameCamera.ViewMatrix * gameCamera.ProjectionMatrix);

                    foreach (Trash trash in trashes)
                    {
                        trash.Update(gameTime);
                    }

                    CursorManager.CheckClick(ref this.lastMouseState, ref this.currentMouseState, gameTime, ref clickTimer, ref clicked, ref doubleClicked);
                    foreach (Factory factory in factories)
                    {
                        factory.Update(gameTime, ref powerpacks, ref resources);
                        if (doubleClicked && CursorManager.MouseOnObject(cursor, factory.BoundingSphere, factory.Position, gameCamera))
                        {
                            //Dump Trash
                            switch (factory.factoryType)
                            {
                                case FactoryType.biodegradable:
                                    dumpTrashInFactory(factory, HydroBot.bioTrash, factory.Position);
                                    HydroBot.bioTrash = 0;
                                    break;
                                case FactoryType.plastic:
                                    dumpTrashInFactory(factory, HydroBot.plasticTrash, factory.Position);
                                    HydroBot.plasticTrash = 0;
                                    break;
                                case FactoryType.radioactive:
                                    dumpTrashInFactory(factory, HydroBot.nuclearTrash, factory.Position);
                                    HydroBot.nuclearTrash = 0;
                                    break;
                            }
                            doubleClicked = false;
                        }
                    }

                    if (researchFacility != null)
                    {
                        researchFacility.Update(gameTime);
                    }

                    for (int i = 0; i < myBullet.Count; i++)
                    {
                        myBullet[i].update();
                    }

                    for (int i = 0; i < healthBullet.Count; i++)
                    {
                        healthBullet[i].update();
                    }

                    for (int i = 0; i < enemyBullet.Count; i++)
                    {
                        enemyBullet[i].update();
                    }
                    for (int i = 0; i < alliesBullets.Count; i++)
                    {
                        alliesBullets[i].update();
                    }
                    Collision.updateBulletOutOfBound(hydroBot.MaxRangeX, hydroBot.MaxRangeZ, healthBullet, myBullet, enemyBullet, alliesBullets, frustum);
                    Collision.updateDamageBulletVsBarriersCollision(myBullet, enemies, ref enemiesAmount, frustum, GameMode.SurvivalMode, gameTime, hydroBot,
                        enemies, enemiesAmount, fish, fishAmount, gameCamera);
                    Collision.updateHealingBulletVsBarrierCollision(healthBullet, fish, fishAmount, frustum, GameMode.SurvivalMode);
                    Collision.updateDamageBulletVsBarriersCollision(enemyBullet, fish, ref fishAmount, frustum, GameMode.SurvivalMode, gameTime, hydroBot,
                        enemies, enemiesAmount, fish, fishAmount, gameCamera);
                    Collision.updateProjectileHitBot(hydroBot, enemyBullet, GameMode.SurvivalMode, enemies, enemiesAmount, null);
                    Collision.updateDamageBulletVsBarriersCollision(alliesBullets, enemies, ref enemiesAmount, frustum, GameMode.SurvivalMode, gameTime, hydroBot,
                        enemies, enemiesAmount, fish, fishAmount, gameCamera);

                    Collision.deleteSmallerThanZero(enemies, ref enemiesAmount, frustum, GameMode.SurvivalMode, cursor);
                    Collision.deleteSmallerThanZero(fish, ref fishAmount, frustum, GameMode.SurvivalMode, cursor);
                    
                    //revive the dead enemies to maintain their number
                    AddingObjects.ReviveDeadEnemy(enemies, enemiesAmount, fish, fishAmount, hydroBot);

                    for (int i = 0; i < enemiesAmount; i++)
                    {
                        //disable stun if stun effect times out
                        if (enemies[i].stunned)
                        {
                            if (PoseidonGame.playTime.TotalSeconds - enemies[i].stunnedStartTime > GameConstants.timeStunLast)
                                enemies[i].stunned = false;
                        }
                        enemies[i].Update(enemies, ref enemiesAmount, fish, fishAmount, random.Next(100), hydroBot, enemyBullet, alliesBullets, frustum, gameTime, GameMode.SurvivalMode);
                    }

                    for (int i = 0; i < fishAmount; i++)
                    {
                        fish[i].Update(gameTime, enemies, enemiesAmount, fish, fishAmount, random.Next(100), hydroBot, enemyBullet);
                    }
                    //Checking win/lost condition for this level
                    if (HydroBot.currentHitPoint <= 0 || isAncientKilled)
                    {
                        currentGameState = GameState.Lost;
                        audio.gameOver.Play();
                    }

                    roundTimer -= gameTime.ElapsedGameTime;
                    PoseidonGame.playTime += gameTime.ElapsedGameTime;

                    //for the shader
                    m_Timer += (float)gameTime.ElapsedGameTime.Milliseconds / 1000;

                    //cursor update
                    cursor.Update(GraphicDevice, gameCamera, gameTime, frustum);

                    //update the good will bar
                    //if (HydroBot.goodWillPoint >= HydroBot.maxGoodWillPoint)
                    //{
                    //    IngamePresentation.SpinNow();
                    //    HydroBot.goodWillPoint = 0;
                    //}
                    //IngamePresentation.UpdateGoodWillBar();

                }

                prevGameState = currentGameState;
                if (currentGameState == GameState.Lost)
                {
                    // Return to main menu
                    if (lastKeyboardState.IsKeyDown(Keys.Enter) &&
                        currentKeyboardState.IsKeyUp(Keys.Enter))
                    {
                        currentGameState = GameState.ToMainMenu;
                    }
                }

                base.Update(gameTime);
            }
        }
               

        public override void Draw(GameTime gameTime)
        {

            switch (currentGameState)
            {
                case GameState.Running:
                    RestoreGraphicConfig();
                    DrawGameplayScreen(gameTime);
                    break;
                case GameState.Won:
                    DrawWinOrLossScreen();
                    break;
                case GameState.Lost:
                    DrawWinOrLossScreen();
                    break;
            }
            base.Draw(gameTime);
            if (paused)
            {
                // Draw the "pause" text
                spriteBatch.Begin();
                spriteBatch.Draw(actionTexture, pausePosition, pauseRect,
                    Color.White);
                spriteBatch.End();
            }

        }

        private void DrawWinOrLossScreen()
        {
            spriteBatch.Begin();
            if (currentGameState == GameState.Won)
                spriteBatch.Draw(winningTexture, GraphicDevice.Viewport.TitleSafeArea, Color.White);
            else spriteBatch.Draw(losingTexture, GraphicDevice.Viewport.TitleSafeArea, Color.White);
            spriteBatch.End();
        }

        private void DrawGameplayScreen(GameTime gameTime)
        {
            graphics.GraphicsDevice.SetRenderTarget(renderTarget);
            graphics.GraphicsDevice.Clear(Color.Black);

            terrain.Draw(gameCamera);

            // Updating camera's frustum
            frustum = new BoundingFrustum(gameCamera.ViewMatrix * gameCamera.ProjectionMatrix);
            foreach (Powerpack f in powerpacks)
            {
                if (!f.Retrieved && f.BoundingSphere.Intersects(frustum))
                {
                    f.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
                }
            }
            foreach (Resource r in resources)
            {
                if (!r.Retrieved && r.BoundingSphere.Intersects(frustum))
                {
                    r.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
                }
            }

            for (int i = 0; i < enemiesAmount; i++)
            {
                if (enemies[i].BoundingSphere.Intersects(frustum))
                {
                    enemies[i].Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalShading");
                    if (enemies[i].stunned == true)
                    {
                        Vector3 placeToDraw = game.GraphicsDevice.Viewport.Project(enemies[i].Position, gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity);
                        Vector2 drawPos = new Vector2(placeToDraw.X, placeToDraw.Y);
                        spriteBatch.Begin();
                        spriteBatch.Draw(stunnedTexture, drawPos, Color.White);
                        spriteBatch.End();
                        RestoreGraphicConfig();
                    }
                }
            }

            for (int i = 0; i < fishAmount; i++)
            {
                if (fish[i].BoundingSphere.Intersects(frustum))
                {
                    fish[i].Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalShading");
                    //RasterizerState rs = new RasterizerState();
                    //rs.FillMode = FillMode.WireFrame;
                    //GraphicDevice.RasterizerState = rs;
                    //fish[i].DrawBoundingSphere(gameCamera.ViewMatrix,
                    //    gameCamera.ProjectionMatrix, boundingSphere);

                    //rs = new RasterizerState();
                    //rs.FillMode = FillMode.Solid;
                    //GraphicDevice.RasterizerState = rs;
                }
            }

            // Drawing trash
            BoundingSphere trashRealSphere;
            foreach (Trash trash in trashes)
            {
                trashRealSphere = trash.BoundingSphere;
                trashRealSphere.Center.Y = trash.Position.Y;
                if (!trash.Retrieved && trashRealSphere.Intersects(frustum))
                {
                    trash.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalShading");
                }
            }

            // Drawing Factories
            BoundingSphere factoryRealSphere;
            foreach (Factory factory in factories)
            {
                factoryRealSphere = factory.BoundingSphere;
                factoryRealSphere.Center.Y = factory.Position.Y;
                if (factoryRealSphere.Intersects(frustum))
                {
                    factory.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
                }
            }
            if (researchFacility != null)
            {
                factoryRealSphere = researchFacility.BoundingSphere;
                factoryRealSphere.Center.Y = researchFacility.Position.Y;
                if (factoryRealSphere.Intersects(frustum))
                    researchFacility.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
            }

            hydroBot.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "Normal Shading");
            for (int i = 0; i < myBullet.Count; i++)
            {
                if (myBullet[i].BoundingSphere.Intersects(frustum))
                {
                    myBullet[i].draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
                    //RasterizerState rs = new RasterizerState();
                    //rs.FillMode = FillMode.WireFrame;
                    //GraphicDevice.RasterizerState = rs;
                    //myBullet[i].DrawBoundingSphere(gameCamera.ViewMatrix,
                    //    gameCamera.ProjectionMatrix, boundingSphere);

                    //rs = new RasterizerState();
                    //rs.FillMode = FillMode.Solid;
                    //GraphicDevice.RasterizerState = rs;
                }
            }

            for (int i = 0; i < healthBullet.Count; i++)
            {
                if (healthBullet[i].BoundingSphere.Intersects(frustum))
                {
                    healthBullet[i].draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
                }
            }

            for (int i = 0; i < enemyBullet.Count; i++)
            {
                if (enemyBullet[i].BoundingSphere.Intersects(frustum))
                {
                    enemyBullet[i].draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
                }
            }

            for (int i = 0; i < alliesBullets.Count; i++)
            {
                if (alliesBullets[i].BoundingSphere.Intersects(frustum))
                {
                    alliesBullets[i].draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
                }
            }
            // draw bubbles
            foreach (Bubble bubble in bubbles)
            {
                bubble.Draw(spriteBatch, 1.0f);
            }

            //Draw points gained / lost
            foreach (Point point in points)
            {
                point.Draw(spriteBatch);
            }


            graphics.GraphicsDevice.SetRenderTarget(null);
            SceneTexture = renderTarget;
            // Render the scene with Edge Detection, using the render target from last frame.
            graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            {
                // Apply the post process shader
                underWaterEffect.CurrentTechnique.Passes[0].Apply();
                {
                    underWaterEffect.Parameters["fTimer"].SetValue(m_Timer);
                    spriteBatch.Draw(SceneTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
                }
            }
            spriteBatch.End();
            spriteBatch.Begin();
            DrawStats();
            DrawBulletType();
            //DrawHeight();
            DrawRadar();
            if (HydroBot.activeSkillID != -1) DrawActiveSkill();
            cursor.Draw(gameTime);
            spriteBatch.End();
        }

        private void DrawRadar()
        {
            radar.Draw(spriteBatch, hydroBot.Position, enemies, enemiesAmount, fish, fishAmount, null, factories, researchFacility);
        }


        private void DrawStats()
        {
            float xOffsetText, yOffsetText;
            
            string str1 = GameConstants.ScoreAchieved;
            string str2 = "";// = GameConstants.StrCellsFound + retrievedFruits.ToString() +
            //" of " + fruits.Count;
            Rectangle rectSafeArea;
            str1 += ((int)score).ToString();
            //if (roundTimer.Minutes < 10)
            //    str1 += "0";
            //str1 += roundTimer.Minutes + ":";
            //if (roundTimer.Seconds < 10)
            //    str1+= "0";
            //str1 += roundTimer.Seconds;
            //str1 += "\n Active skill " + Tank.activeSkillID;
            //str1 += "\n Experience " + Tank.currentExperiencePts + "/" + Tank.nextLevelExperience;
            //str1 += "\n Level: " + Tank.level;
            //str2 += "Player's health: " + tank.currentHitPoint + "/" + tank.maxHitPoint; 
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
            //str2 += "\nFish prevTurnAmount " + fish[0].prevTurnAmount + "Fish pos " +  fish[0].Position + "Stuck " + fish[0].stucked;
            //str2 += "\nPrevFIre " + enemies[0].prevFire;
            //str2 += "Health: " + ((Fish)(fish[0])).health + "\n Size "+ fishAmount;
            //str2 += "Type " + tank.GetType().Name.ToString();
            //if (bubbles.Count > 0)
            //{
            //    str2 += "Bubbles " + bubbles.Count + " Scale " + bubbles[0].scale + " Time last " + bubbles[0].timeLast;
            //    //str2 += "\nBub pos " + bubbles[0].bubblePos;
            //}
            //str2 += "School1 " + schoolOfFish1.flock.flock.Count + " School2 " + schoolOfFish2.flock.flock.Count;
            //str2 += "School1 " + schoolOfFish1.flock.flock.Count + " School2 " + schoolOfFish2.flock.flock.Count;
            //str2 += "\n" + schoolOfFish1.flock.flock[0].Location + "\n" + schoolOfFish2.flock.flock[0].Location;
            //str2 += "\n" + schoolOfFish1.flock.flock[1].texture.Name.Length + "\n" + schoolOfFish2.flock.flock[1].texture.Name;
            //str2 += "\n" + trashes[0].fogEndValue;
            //Display Fish Health
            Fish fishPointedAt = CursorManager.MouseOnWhichFish(cursor, gameCamera, fish, fishAmount);
            if (fishPointedAt != null)
            {
                IngamePresentation.DrawHealthBar(HealthBar, game, spriteBatch, statsFont, (int)fishPointedAt.health, (int)fishPointedAt.maxHealth, 5, fishPointedAt.Name, Color.Red);
                string line;
                line = "'";
                if (fishPointedAt.health < 20)
                {
                    line += "SAVE ME!!!";
                }
                else if (fishPointedAt.health < 60)
                {
                    line += IngamePresentation.wrapLine(fishPointedAt.sad_talk, HealthBar.Width + 20, fishTalkFont);
                }
                else
                {
                    line += IngamePresentation.wrapLine(fishPointedAt.happy_talk, HealthBar.Width + 20, fishTalkFont);
                }
                line += "'";
                spriteBatch.DrawString(fishTalkFont, line, new Vector2(game.Window.ClientBounds.Width / 2 - HealthBar.Width / 2, 32), Color.Yellow);
            }

            //Display Enemy Health
            BaseEnemy enemyPointedAt = CursorManager.MouseOnWhichEnemy(cursor, gameCamera, enemies, enemiesAmount);
            if (enemyPointedAt != null)
                IngamePresentation.DrawHealthBar(HealthBar, game, spriteBatch, statsFont, (int)enemyPointedAt.health, (int)enemyPointedAt.maxHealth, 5, enemyPointedAt.Name, Color.IndianRed);

            //Display Cyborg health
            IngamePresentation.DrawHealthBar(HealthBar, game, spriteBatch, statsFont, (int)HydroBot.currentHitPoint, (int)HydroBot.maxHitPoint, game.Window.ClientBounds.Height - 60, "HEALTH", Color.Brown);

            //Display Environment Bar
            if (HydroBot.currentEnvPoint > HydroBot.maxEnvPoint) HydroBot.currentEnvPoint = HydroBot.maxEnvPoint;
            IngamePresentation.DrawEnvironmentBar(EnvironmentBar, game, spriteBatch, statsFont, HydroBot.currentEnvPoint, HydroBot.maxEnvPoint);

            //Display Level/Experience Bar
            IngamePresentation.DrawLevelBar(HealthBar, game, spriteBatch, statsFont, HydroBot.currentExperiencePts, HydroBot.nextLevelExperience, HydroBot.level, game.Window.ClientBounds.Height - 30, "EXPERIENCE LEVEL", Color.Brown);

            //Display Good will bar
            IngamePresentation.DrawGoodWillBar(game, spriteBatch, statsFont);

            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            xOffsetText = rectSafeArea.X;
            yOffsetText = rectSafeArea.Y;

            Vector2 strSize = statsFont.MeasureString(str1);
            Vector2 strPosition =
                new Vector2((int)xOffsetText + 10, (int)yOffsetText);

            spriteBatch.DrawString(menuSmall, str1, strPosition, Color.DarkRed);
            strPosition.Y += strSize.Y;
            spriteBatch.DrawString(statsFont, str2, strPosition, Color.White);
        }

        // Draw the currently selected bullet type
        private void DrawBulletType()
        {
            int xOffsetText, yOffsetText;
            Rectangle rectSafeArea;

            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            //xOffsetText = rectSafeArea.Left + 325;
            xOffsetText = rectSafeArea.Center.X - 150 - 64;
            yOffsetText = rectSafeArea.Bottom - 80;

            //Vector2 bulletIconPosition =
            //    new Vector2((int)xOffsetText, (int)yOffsetText);
            Rectangle destRectangle = new Rectangle(xOffsetText, yOffsetText, 64, 64);
            //spriteBatch.Draw(bulletTypeTextures[tank.bulletType], bulletIconPosition, Color.White);
            spriteBatch.Draw(bulletTypeTextures[HydroBot.bulletType], destRectangle, Color.White);
        }

        // Draw the currently selected skill/spell
        private void DrawActiveSkill()
        {
            IngamePresentation.DrawActiveSkill(GraphicDevice, skillTextures, spriteBatch);
        }


        public static void RestoreGraphicConfig()
        {
            // Change back the config changed by spriteBatch
            GraphicDevice.BlendState = BlendState.Opaque;
            GraphicDevice.DepthStencilState = DepthStencilState.Default;
            GraphicDevice.SamplerStates[0] = SamplerState.LinearWrap;
            return;
        }

        public void dumpTrashInFactory(Factory factory, int amount, Vector3 position)
        {
            string point_string = "";
            switch (factory.factoryType)
            {
                case FactoryType.biodegradable:
                    point_string = HydroBot.bioTrash + " Biodegradable Trash Dumped.\n";
                    factory.numTrashWaiting += HydroBot.bioTrash;
                    break;
                case FactoryType.plastic:
                    point_string = HydroBot.plasticTrash + " Plastic Trash Dumped.\n";
                    factory.numTrashWaiting += HydroBot.plasticTrash;
                    break;
                case FactoryType.radioactive:
                    point_string = HydroBot.nuclearTrash + " Radioactive Trash Dumped.\n";
                    factory.numTrashWaiting += HydroBot.nuclearTrash;
                    break;
            }
            Point point = new Point();
            point.LoadContent(PoseidonGame.contentManager, point_string, position, Color.LawnGreen);
            points.Add(point);
        }
    }
}
