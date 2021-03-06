﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Poseidon.Core;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Poseidon.FishSchool;
using System.IO;
using Poseidon.GraphicEffects;


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

        int numTrash = 100; //45 - 45 - 10

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
        public static GameState currentGameState = GameState.Running;
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


        private Texture2D stunnedIconTexture, scaredIconTexture;

        float m_Timer = 0;
        RenderTarget2D renderTarget, afterEffectsAppliedRenderTarget;

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
        public static float highestScore;

        //is ancient fish killed
        public static bool isAncientKilled;

        // For mouse inputs
        bool doubleClicked = false;
        bool clicked = false;
        bool notYetReleased = false;
        double clickTimer = 0;

        private bool openFactoryConfigurationScene = false;
        private bool openResearchFacilityConfigScene = false;
        private Factory factoryToConfigure;

        // For applying graphic effects
        GraphicEffect graphicEffect;
        //for particle systems
        public static ParticleManagement particleManager;

        // Texture and font for property window of a factory
        private SpriteFont factoryFont;
        private Texture2D factoryBackground;
        private Texture2D factoryProduceButton;

        // Textures for animating the processing state of factories.
        // Plastic factory will use nuclear factory textures
        private List<Texture2D> biofactoryAnimationTextures;
        private List<Texture2D> nuclearFactoryAnimationTextures;

        // Texture and font for property window of a research facility
        SpriteFont facilityFont;
        SpriteFont facilityFont2;
        Texture2D facilityBackground;
        Texture2D facilityUpgradeButton;
        Texture2D playJigsawButton;
        Texture2D increaseAttributeButton;

        // Texture/Font for Mouse Interaction panel for factories
        Texture2D factoryPanelTexture;
        SpriteFont factoryPanelFont;
        ButtonPanel factoryButtonPanel;

        // Models and textures for Factories and buildings
        private Model researchBuildingModel;
        private List<Model> researchBuildingModelStates;
        private Model plasticFactoryModel;
        private List<Model> plasticFactoryModelStates;
        private List<Texture2D> plasticFactoryLevelTextures;
        private Model biodegradableFactoryModel;
        private List<Model> biodegradableFactoryModelStates;
        private List<Texture2D> biodegradableFactoryLevelTextures;
        private Model radioactiveFactoryModel;
        private List<Model> radioactiveFactoryModelStates;

        //Models for Trash
        private Model biodegradableTrash, plasticTrash, radioactiveTrash;

        //Models for powerpacks, strangeRock and resource
        private Model[] powerpackModels;
        private Model[] strangeRockModels;
        private Model resourceModel;

        //for edge detection effect
        RenderTarget2D normalDepthRenderTargetLow, normalDepthRenderTargetHigh, edgeDetectionRenderTarget;

        //for drawing a game boundary
        GameBoundary gameBoundary;

        protected bool levelObjHover = false;
        protected bool tipHover = false;

        Factory factoryAnchor;
        ResearchFacility researchAnchor;

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
            this.stunnedIconTexture = stunnedTexture;
            roundTime = TimeSpan.FromSeconds(2592000);
            random = new Random();
            
            gameCamera = new Camera(GameMode.SurvivalMode);
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
            HydroBot.gameMode = GameMode.SurvivalMode;
            for (int index = 0; index < GameConstants.numberOfSkills; index++)
            {
                HydroBot.skills[index] = true;
            }

            skillTextures = new Texture2D[GameConstants.numberOfSkills];
            bulletTypeTextures = new Texture2D[GameConstants.numBulletTypes];

            // for the mouse or touch
            cursor = new Cursor(game, spriteBatch);
            //Components.Add(cursor);

            bubbles = new List<Bubble>();
            points = new List<Point>();


            //loading winning, losing textures
            winningTexture = IngamePresentation.winningTexture;
            losingTexture = IngamePresentation.losingTexture;
            scaredIconTexture = IngamePresentation.scaredIconTexture;

            isAncientKilled = false;

            // Instantiate the factory Button
            float buttonScale = 1.0f;
            if (game.Window.ClientBounds.Width <= 900)
            {
                buttonScale = 0.8f; // scale the factory panel icons a bit smaller in small window mode
            }
            factoryButtonPanel = new ButtonPanel(4, buttonScale);

            this.Load();

            gameBoundary = new GameBoundary();
            gameBoundary.LoadGraphicsContent(GraphicDevice);

        }

        public void Load()
        {
            statsFont = IngamePresentation.statsFont;
            menuSmall = IngamePresentation.menuSmall;
            fishTalkFont = IngamePresentation.fishTalkFont;

            // Get the audio library
            audio = (AudioLibrary)
                Game.Services.GetService(typeof(AudioLibrary));

            terrain = new Terrain(Content);

            skillTextures = IngamePresentation.skillTextures;
            bulletTypeTextures = IngamePresentation.bulletTypeTextures;

            powerpacks = new List<Powerpack>();
            resources = new List<Resource>();

            powerpacks = new List<Powerpack>();
            resources = new List<Resource>();

            hydroBot.Load(Content);

            //prevTank.Load(Content);
            roundTimer = roundTime;

            PresentationParameters pp = graphics.GraphicsDevice.PresentationParameters;
            renderTarget = new RenderTarget2D(graphics.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight,
                false, graphics.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24Stencil8);
            afterEffectsAppliedRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight,
                false, graphics.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24Stencil8);

            //for edge detection effect
            normalDepthRenderTargetLow = new RenderTarget2D(graphics.GraphicsDevice,
                                                         pp.BackBufferWidth, pp.BackBufferHeight, false,
                                                         pp.BackBufferFormat, pp.DepthStencilFormat);
            normalDepthRenderTargetHigh = new RenderTarget2D(graphics.GraphicsDevice,
                                             pp.BackBufferWidth, pp.BackBufferHeight, false,
                                             pp.BackBufferFormat, pp.DepthStencilFormat);
            edgeDetectionRenderTarget = new RenderTarget2D(graphics.GraphicsDevice,
                                                         pp.BackBufferWidth, pp.BackBufferHeight, false,
                                                         pp.BackBufferFormat, pp.DepthStencilFormat);

            graphicEffect = new GraphicEffect(this, this.spriteBatch, fishTalkFont);
            // Construct our particle system components.
            particleManager = new ParticleManagement(this.game, GraphicDevice);

            // Load lower left pannel button
            factoryPanelTexture = IngamePresentation.factoryPanelTexture;
            // Load Font for displaying extra information on factory panel
            factoryPanelFont = IngamePresentation.factoryPanelFont;

            // Load textures for partid animation for factories
            biofactoryAnimationTextures = IngamePresentation.biofactoryAnimationTextures;
            nuclearFactoryAnimationTextures = IngamePresentation.nuclearFactoryAnimationTextures;

            // Load factory and research lab models
            plasticFactoryModelStates = new List<Model>();
            biodegradableFactoryModelStates = new List<Model>();
            radioactiveFactoryModelStates = new List<Model>();
            researchBuildingModelStates = new List<Model>();
            int totalStates = 4;
            for (int i = 0; i < totalStates; i++)
            {
                plasticFactoryModelStates.Add(Content.Load<Model>("Models/FactoryModels/PlasticFactory_stage" + i));
                biodegradableFactoryModelStates.Add(Content.Load<Model>("Models/FactoryModels/BiodegradableFactory_stage" + i));
                radioactiveFactoryModelStates.Add(Content.Load<Model>("Models/FactoryModels/NuclearFactory_stage" + i));
                researchBuildingModelStates.Add(Content.Load<Model>("Models/FactoryModels/ResearchFacility_stage" + i));
            }
            radioactiveFactoryModel = radioactiveFactoryModelStates[radioactiveFactoryModelStates.Count - 1];
            researchBuildingModel = researchBuildingModelStates[researchBuildingModelStates.Count - 1];
            biodegradableFactoryModel = biodegradableFactoryModelStates[biodegradableFactoryModelStates.Count - 1];
            plasticFactoryModel = plasticFactoryModelStates[plasticFactoryModelStates.Count - 1];


            // Factory level textures
            plasticFactoryLevelTextures = IngamePresentation.plasticFactoryLevelTextures;
            biodegradableFactoryLevelTextures = IngamePresentation.biodegradableFactoryLevelTextures;

            // Load Trash
            biodegradableTrash = Content.Load<Model>("Models/TrashModels/biodegradableTrashVer4");
            plasticTrash = Content.Load<Model>("Models/TrashModels/plasticTrashVer3");
            radioactiveTrash = Content.Load<Model>("Models/TrashModels/radioactiveTrash");

            //Load Powerpacks
            powerpackModels = new Model[5];
            powerpackModels[1] = Content.Load<Model>("Models/PowerpackResource/speedPowerPack");
            powerpackModels[2] = Content.Load<Model>("Models/PowerpackResource/strengthPowerPack");
            powerpackModels[3] = Content.Load<Model>("Models/PowerpackResource/shootratePowerPack");
            powerpackModels[4] = Content.Load<Model>("Models/PowerpackResource/healthPowerPack");

            //Load Resource
            resourceModel = Content.Load<Model>("Models/PowerpackResource/resource");

            //Load Strange Rock
            strangeRockModels = new Model[2];
            strangeRockModels[0] = Content.Load<Model>("Models/Miscellaneous/strangeRock1Ver2");
            strangeRockModels[1] = Content.Load<Model>("Models/Miscellaneous/strangeRock2Ver2");

            //golden key
            //goldenKey = Content.Load<Model>("Models/Miscellaneous/Goldenkey");

            //Initialize the game field
            InitializeGameField(Content);
        }

        /// <summary>
        /// Show the action scene
        /// </summary>
        public override void Show()
        {
            IngamePresentation.ResetObjPointedAtMsgs();
            paused = false;
            //Factory.buildingSoundInstance.Resume();
            //ResearchFacility.buildingSoundInstance.Resume();
            HydroBot.gameMode = GameMode.SurvivalMode;
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
                hydroBot.Position, aspectRatio, gameTime, cursor);


            //Clean all fruits and resources
            powerpacks.Clear();
            resources.Clear();

            startTime = gameTime.TotalGameTime;

            currentGameState = GameState.Running;

            InitializeGameField(Content);

        }

        //Below stuff uses GraphicsDevice which can not be called from Load, as it gives runtime error "GraphicDevice not initialized"
        protected override void LoadContent()
        {
            // Initialie the button panel
            factoryButtonPanel.Initialize(ref factoryPanelTexture, ref factoryPanelFont, new Vector2(10, GraphicsDevice.Viewport.Height - 70));
        }

        private void InitializeGameField(ContentManager Content)
        {
            currentGameState = GameState.Running;
            HydroBot.numResources += GameConstants.numResourcesAtStart;

            enemyBullet = new List<DamageBullet>();
            healthBullet = new List<HealthBullet>();
            myBullet = new List<DamageBullet>();
            alliesBullets = new List<DamageBullet>();

            float orientation = random.Next(100);
          
            enemiesAmount = 0;
            fishAmount = 0;
            enemies = new BaseEnemy[GameConstants.SurvivalModeMaxShootingEnemy + GameConstants.SurvivalModeMaxCombatEnemy + GameConstants.SurvivalModeMaxGhostPirate
                + GameConstants.SurvivalModeMaxMutantShark + GameConstants.SurvivalModeMaxTerminator + GameConstants.SurvivalModeMaxSubmarine * (1 + GameConstants.NumEnemiesInSubmarine)];
            fish = new Fish[4];

            AddingObjects.placeFish(ref fishAmount, fish, Content, random, enemiesAmount, enemies, null,
                GameConstants.MainGameMinRangeX, GameConstants.MainGameMaxRangeX, GameConstants.MainGameMinRangeZ, GameConstants.MainGameMaxRangeZ, -1, GameMode.SurvivalMode, GameConstants.MainGameFloatHeight);

            AddingObjects.placeEnemies(ref enemiesAmount, enemies, Content, random, fishAmount, fish, null,
                GameConstants.MainGameMinRangeX, GameConstants.MainGameMaxRangeX, GameConstants.MainGameMinRangeZ, GameConstants.MainGameMaxRangeZ, -1, GameMode.SurvivalMode, GameConstants.MainGameFloatHeight);



            //Initialize trash
            //int random_model;
            //int numberTrash = GameConstants.NumberBioTrash[currentLevel] + GameConstants.NumberNuclearTrash[currentLevel] + GameConstants.NumberPlasticTrash[currentLevel];
            trashes = new List<Trash>(GameConstants.SurvivalModeNumBioTrash + GameConstants.SurvivalModeNumPlasTrash + GameConstants.SurvivalModeNumRadioTrash);
            int bioIndex, plasticIndex, nuclearIndex;
            for (bioIndex = 0; bioIndex < GameConstants.SurvivalModeNumBioTrash; bioIndex++)
            {
                orientation = random.Next(100);
                trashes.Add(new Trash(TrashType.biodegradable, particleManager));
                trashes[bioIndex].Load(Content,ref biodegradableTrash, orientation); //bio model
            }
            for (plasticIndex = bioIndex; plasticIndex < bioIndex + GameConstants.SurvivalModeNumPlasTrash; plasticIndex++)
            {
                orientation = random.Next(100);
                trashes.Add(new Trash(TrashType.plastic, particleManager));
                trashes[plasticIndex].Load(Content,ref plasticTrash, orientation); //plastic model
            }
            for (nuclearIndex = plasticIndex; nuclearIndex < plasticIndex + GameConstants.SurvivalModeNumRadioTrash; nuclearIndex++)
            {
                orientation = random.Next(100);
                trashes.Add(new Trash(TrashType.radioactive, particleManager));
                trashes[nuclearIndex].Load(Content,ref radioactiveTrash, orientation); //nuclear model
            }

            AddingObjects.placeTrash(ref trashes, Content, random, null, null,
                GameConstants.TrashMinRangeX, GameConstants.MainGameMaxRangeX - 80, GameConstants.TrashMinRangeZ,
                GameConstants.MainGameMaxRangeZ - 60, GameMode.MainGame, GameConstants.MainGameFloatHeight, terrain.heightMapInfo); 

           //Initialize a list of factories
            factories = new List<Factory>();

            //create research facility
            researchFacility = null;

            if (HydroBot.hasDolphin)
            {
                AddingObjects.placeMinion(Content, 2, enemies, enemiesAmount, fish, ref fishAmount, hydroBot);
            }
            if (HydroBot.hasSeaCow)
            {
                AddingObjects.placeMinion(Content, 0, enemies, enemiesAmount, fish, ref fishAmount, hydroBot);
            }
            if (HydroBot.hasTurtle)
            {
                AddingObjects.placeMinion(Content, 1, enemies, enemiesAmount, fish, ref fishAmount, hydroBot);
            }
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
            //skill combo unlock
            if (score >= 500 && !HydroBot.skillComboActivated)
            {
                HydroBot.skillComboActivated = true;
                Point point = new Point();
                point.LoadContent(PoseidonGame.contentManager, "CONGRATULATIONS\nSKILL COMBO UNLOCKED", hydroBot.Position, Color.Gold);
                points.Add(point);
                PoseidonGame.audio.levelUpSound.Play();
            }
            if (score > highestScore) highestScore = score;
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
                    MouseState currentMouseState = new MouseState();
                    currentMouseState = Mouse.GetState();
                    // Update Factory Button Panel
                    factoryButtonPanel.Update(gameTime, currentMouseState);
                    UpdateAnchor();
                    // if mouse click happened, check for the click position and add new factory
                    if (factoryButtonPanel.hasAnyAnchor() && factoryButtonPanel.cursorOutsidePanelArea && factoryButtonPanel.clickToBuildDetected)
                    {
                        // adjust the position to build factory depending on current camera position
                        Vector3 newBuildingPosition = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);

                        // identify which factory depending on anchor index
                        BuildingType newBuildingType = factoryButtonPanel.anchorIndexToBuildingType();

                        // if addition is successful, play successful sound, otherwise play unsuccessful sound
                        if (addNewBuilding(newBuildingType, newBuildingPosition))
                        {
                            factoryButtonPanel.removeAnchor();
                        }
                    }
                    if ((lastKeyboardState.IsKeyDown(Keys.LeftShift) || lastKeyboardState.IsKeyDown(Keys.RightShift)) && (lastMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released)
                        && !openResearchFacilityConfigScene && !openFactoryConfigurationScene)
                    {
                        foreach (Factory factory in factories)
                        {
                            if (!factory.UnderConstruction && CursorManager.MouseOnObject(cursor, factory.BoundingSphere, factory.Position, gameCamera))
                            {
                                openFactoryConfigurationScene = true;
                                factoryToConfigure = factory;
                                break;
                            }
                        }
                        if (researchFacility != null && !researchFacility.UnderConstruction && CursorManager.MouseOnObject(cursor, researchFacility.BoundingSphere, researchFacility.Position, gameCamera))
                        {
                            openResearchFacilityConfigScene = true;
                            ResearchFacility.dolphinLost = ResearchFacility.seaCowLost = ResearchFacility.turtleLost = false;
                            ResearchFacility.dolphinWon = ResearchFacility.seaCowWon = ResearchFacility.turtleWon = false;
                        }
                    }
                    if (openFactoryConfigurationScene || openResearchFacilityConfigScene)
                    {
                        Factory.buildingSoundInstance.Pause();
                        ResearchFacility.buildingSoundInstance.Pause();
                        bool exitFactConfPressed;
                        exitFactConfPressed = (lastKeyboardState.IsKeyDown(Keys.Enter) && (currentKeyboardState.IsKeyUp(Keys.Enter)));
                        if (exitFactConfPressed)
                        {
                            openFactoryConfigurationScene = false;
                            openResearchFacilityConfigScene = false;
                            PoseidonGame.justCloseControlPanel = true;
                        }
                        else
                        {
                            //cursor update
                            cursor.Update(GraphicDevice, gameCamera, gameTime, frustum);
                            CursorManager.MouseInteractWithControlPanel(ref clicked, ref doubleClicked, ref notYetReleased, ref this.lastMouseState, ref this.currentMouseState, gameTime,
                                ref clickTimer, openFactoryConfigurationScene, factoryToConfigure, researchFacility, factories);
                            return;
                        }
                    }

                    IngamePresentation.levelObjHover = IngamePresentation.mouseOnLevelObjectiveIcon(currentMouseState);
                    bool mouseOnInteractiveIcons = IngamePresentation.levelObjHover || (!factoryButtonPanel.cursorOutsidePanelArea) || factoryButtonPanel.hasAnyAnchor()
                         || factoryButtonPanel.clickToBuildDetected || factoryButtonPanel.clickToRemoveAnchorActive || factoryButtonPanel.rightClickToRemoveAnchor;
                    //hydrobot update
                    hydroBot.UpdateAction(gameTime, cursor, gameCamera, enemies, enemiesAmount, fish, fishAmount, Content, spriteBatch, myBullet,
                        this, terrain.heightMapInfo, healthBullet, powerpacks, resources, trashes, null,null, mouseOnInteractiveIcons);

                    //add 1 bubble over bot and each enemy
                    timeNextBubble -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (timeNextBubble <= 0)
                    {
                        Bubble bubble = new Bubble();
                        bubble.LoadContent(Content, hydroBot.Position, false, 0.025f);
                        bubbles.Add(bubble);
                        for (int i = 0; i < enemiesAmount; i++)
                        {
                            if (enemies[i].BoundingSphere.Intersects(frustum) && !(enemies[i] is MutantShark) && !(enemies[i] is Submarine) && !(enemies[i] is GhostPirate))
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

                    gameCamera.Update(hydroBot.ForwardDirection, hydroBot.Position, aspectRatio, gameTime, cursor);
                    // Updating camera's frustum
                    frustum = new BoundingFrustum(gameCamera.ViewMatrix * gameCamera.ProjectionMatrix);

                    if (trashes != null)// && trashes.Count < numTrash)
                    {
                        Vector3 pos = AddingObjects.createSinkingTrash(ref trashes, Content, random, null, null, factories, researchFacility,
                                GameConstants.TrashMinRangeX, GameConstants.MainGameMaxRangeX - 100, GameConstants.TrashMinRangeZ,
                                GameConstants.MainGameMaxRangeZ - 60, GameConstants.MainGameFloatHeight, terrain.heightMapInfo,ref biodegradableTrash,ref plasticTrash,ref radioactiveTrash, particleManager);
                        //Point point = new Point();
                        //point.LoadContent(PoseidonGame.contentManager, "New Trash Dropped", pos, Color.LawnGreen);
                        //points.Add(point);
                    }

                    foreach (Trash trash in trashes)
                    {
                        trash.Update(gameTime);
                    }

                    CursorManager.CheckClick(ref this.lastMouseState, ref this.currentMouseState, gameTime, ref clickTimer, ref clicked, ref doubleClicked, ref notYetReleased);
                    foreach (Factory factory in factories)
                    {
                        factory.Update(gameTime, ref powerpacks, ref resources, ref powerpackModels, ref resourceModel, ref strangeRockModels);
                        if (doubleClicked && !factory.UnderConstruction && hydroBot.BoundingSphere.Intersects(factory.BoundingSphere) && CursorManager.MouseOnObject(cursor, factory.BoundingSphere, factory.Position, gameCamera))
                        {
                            //Dump Trash
                            switch (factory.factoryType)
                            {
                                case FactoryType.biodegradable:
                                    if (HydroBot.bioTrash > 0)
                                    {
                                        dumpTrashInFactory(factory, factory.Position);
                                        HydroBot.bioTrash = 0;
                                    }
                                    break;
                                case FactoryType.plastic:
                                    if (HydroBot.plasticTrash > 0)
                                    {
                                        dumpTrashInFactory(factory, factory.Position);
                                        HydroBot.plasticTrash = 0;
                                    }
                                    break;
                                case FactoryType.radioactive:
                                    if (HydroBot.nuclearTrash > 0)
                                    {
                                        dumpTrashInFactory(factory, factory.Position);
                                        HydroBot.nuclearTrash = 0;
                                    }
                                    break;
                            }
                        }
                    }


                    if (researchFacility != null)
                    {
                        researchFacility.Update(gameTime, hydroBot.Position, ref points);
                        if (doubleClicked && !researchFacility.UnderConstruction && hydroBot.BoundingSphere.Intersects(researchFacility.BoundingSphere) && CursorManager.MouseOnObject(cursor, researchFacility.BoundingSphere, researchFacility.Position, gameCamera))
                        {
                            string point_string = HydroBot.numStrangeObjCollected + " strange rocks\ndeposited";
                            for (int i = 0; i < HydroBot.numStrangeObjCollected; i++)
                                researchFacility.listTimeRockProcessing.Add(PoseidonGame.playTime.TotalSeconds + (i * GameConstants.DaysPerSecond));
                            Point point = new Point();
                            point.LoadContent(PoseidonGame.contentManager, point_string, researchFacility.Position, Color.LawnGreen);
                            points.Add(point);
                            HydroBot.numStrangeObjCollected = 0;
                        }
                    }
                    doubleClicked = false;

                    for (int i = 0; i < myBullet.Count; i++)
                    {
                        myBullet[i].update(gameTime);
                    }

                    for (int i = 0; i < healthBullet.Count; i++)
                    {
                        healthBullet[i].update(gameTime);
                    }

                    for (int i = 0; i < enemyBullet.Count; i++)
                    {
                        enemyBullet[i].update(gameTime);
                    }
                    for (int i = 0; i < alliesBullets.Count; i++)
                    {
                        alliesBullets[i].update(gameTime);
                    }
                    Collision.updateBulletOutOfBound(hydroBot.MaxRangeX, hydroBot.MaxRangeZ, healthBullet, myBullet, enemyBullet, alliesBullets, frustum);
                    Collision.updateDamageBulletVsBarriersCollision(myBullet, enemies, ref enemiesAmount, frustum, GameMode.SurvivalMode, gameTime, hydroBot,
                        enemies, enemiesAmount, fish, fishAmount, gameCamera, particleManager.explosionParticles);
                    Collision.updateHealingBulletVsBarrierCollision(healthBullet, fish, fishAmount, frustum, GameMode.SurvivalMode);
                    Collision.updateDamageBulletVsBarriersCollision(enemyBullet, fish, ref fishAmount, frustum, GameMode.SurvivalMode, gameTime, hydroBot,
                        enemies, enemiesAmount, fish, fishAmount, gameCamera, particleManager.explosionParticles);
                    Collision.updateProjectileHitBot(hydroBot, enemyBullet, GameMode.SurvivalMode, enemies, enemiesAmount, particleManager.explosionParticles, gameCamera, fish, fishAmount);
                    Collision.updateDamageBulletVsBarriersCollision(alliesBullets, enemies, ref enemiesAmount, frustum, GameMode.SurvivalMode, gameTime, hydroBot,
                        enemies, enemiesAmount, fish, fishAmount, gameCamera, particleManager.explosionParticles);

                    Collision.deleteSmallerThanZero(enemies, ref enemiesAmount, frustum, GameMode.SurvivalMode, cursor, particleManager.explosionLargeParticles);
                    Collision.deleteSmallerThanZero(fish, ref fishAmount, frustum, GameMode.SurvivalMode, cursor, particleManager.explosionLargeParticles);
                    
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
                        fish[i].Update(gameTime, frustum, enemies, enemiesAmount, fish, fishAmount, random.Next(100), hydroBot, enemyBullet);
                    }
                    //Checking win/lost condition for this level
                    if (HydroBot.currentHitPoint <= 0 || isAncientKilled)
                    {
                        currentGameState = GameState.Lost;
                        audio.gameOver.Play();
                    }

                    foreach (Powerpack powPack in powerpacks)
                        powPack.Update();
                    foreach (Resource res in resources)
                        res.Update();

                    roundTimer -= gameTime.ElapsedGameTime;
                    PoseidonGame.playTime += gameTime.ElapsedGameTime;

                    //for the shader
                    m_Timer += (float)gameTime.ElapsedGameTime.Milliseconds / 1000;

                    //cursor update
                    cursor.Update(GraphicDevice, gameCamera, gameTime, frustum);

                    //update graphic effects
                    graphicEffect.UpdateInput(gameTime);
                    //update particle systems
                    particleManager.Update(gameTime);

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


        // Add new factory in the game arena if conditions for adding them satisty
        // Research Facility: Only one
        // Uses class variables factories, researchFacility, HydroBot
        private bool addNewBuilding(BuildingType buildingType, Vector3 position)
        {
            bool status = false;
            float orientation; // 0, PI/2, PI, 3*PI/2
            Factory oneFactory;

            bool notEnoughResource = false;
            switch (buildingType)
            {
                case BuildingType.researchlab:
                    if (HydroBot.numResources < GameConstants.numResourcesForResearchCenter)
                        notEnoughResource = true;
                    break;
                case BuildingType.biodegradable:
                    if (HydroBot.numResources < GameConstants.numResourcesForBioFactory)
                        notEnoughResource = true;
                    break;
                case BuildingType.plastic:
                    if (HydroBot.numResources < GameConstants.numResourcesForPlasticFactory)
                        notEnoughResource = true;
                    break;
                case BuildingType.radioactive:
                    if (HydroBot.numResources < GameConstants.numResourcesForRadioFactory)
                        notEnoughResource = true;
                    break;
            }
            // Check if hydrobot has sufficient resources for building a factory
            if (notEnoughResource)
            {
                // Play some sound hinting no sufficient resource
                audio.MenuScroll.Play();
                Point point = new Point();
                String point_string = "Not enough\nresources";
                point.LoadContent(PoseidonGame.contentManager, point_string, position, Color.Red);
                PlayGameScene.points.Add(point);
                return false;
            }
            if (HydroBot.currentEnergy < GameConstants.EnergyLostPerBuild)
            {
                audio.MenuScroll.Play();
                Point point = new Point();
                String point_string = "Not enough\nenergy";
                point.LoadContent(PoseidonGame.contentManager, point_string, position, Color.Red);
                PlayGameScene.points.Add(point);
                return false;
            }
            int radius = 60; // need to revise this based on the model for each building
            BoundingSphere buildingBoundingSphere;
            if (factoryAnchor != null)
            {
                buildingBoundingSphere = factoryAnchor.BoundingSphere;
                radius = (int)buildingBoundingSphere.Radius;
            }
            else if (researchAnchor != null)
            {
                buildingBoundingSphere = researchAnchor.BoundingSphere;
                radius = (int)buildingBoundingSphere.Radius;
            }

            // Check if position selected for building is within game arena.. The game area is within -MaxRange to +MaxRange for both X and Z axis
            // Give a lax of 40 units so that if a click happened at the edge of the arena, building is not allowed. This is to prevent the case
            // that power packs might appear above the end of the factory whose edge is just beyond the game arena.
            if (Math.Abs(position.X) > (float)(GameConstants.MainGameMaxRangeX - 40) || Math.Abs(position.Z) > (float)(GameConstants.MainGameMaxRangeZ - 40))
            {
                // Play some sound hinting position selected is outside game arena
                audio.MenuScroll.Play();
                Point point = new Point();
                String point_string = "Can not\nbuild here";
                point.LoadContent(PoseidonGame.contentManager, point_string, position, Color.Red);
                points.Add(point);
                return false;
            }

            //Verify that current location is available for adding the building

            int heightValue = GameConstants.MainGameFloatHeight;//(int)terrain.heightMapInfo.GetHeight(new Vector3(position.X, 0, position.Z));
            if (AddingObjects.IsSeaBedPlaceOccupied((int)position.X, heightValue, (int)position.Z, radius, null, null, trashes, factories, researchFacility))
            {
                // Play some sound hinting seabed place is occupied
                audio.MenuScroll.Play();
                Point point = new Point();
                String point_string = "Can not\nbuild here";
                point.LoadContent(PoseidonGame.contentManager, point_string, position, Color.Red);
                points.Add(point);
                return false;
            }

            switch (buildingType)
            {
                case BuildingType.researchlab:
                    if (researchFacility != null)
                    {
                        // do not allow addition of more than one research facility
                        Point point = new Point();
                        String point_string = "Can only build\n1 research center";
                        point.LoadContent(PoseidonGame.contentManager, point_string, position, Color.Red);
                        points.Add(point);
                        audio.MenuScroll.Play();
                        status = false;
                    }
                    else
                    {
                        //create research facility.. Only one is allowed, hence using a separate variable for this purpose.
                        researchFacility = new ResearchFacility(particleManager);
                        position.Y = terrain.heightMapInfo.GetHeight(new Vector3(position.X, 0, position.Z));
                        //orientation = (float)(Math.PI/2) * random.Next(4);
                        orientation = researchAnchor.orientation;
                        researchFacility.Model = researchBuildingModel;
                        researchFacility.ModelStates = researchBuildingModelStates;
                        researchFacility.LoadContent(game, position, orientation);
                        HydroBot.numResources -= GameConstants.numResourcesForResearchCenter;
                        status = true;
                    }
                    break;

                case BuildingType.biodegradable:
                    oneFactory = new Factory(FactoryType.biodegradable, particleManager, GraphicsDevice);
                    position.Y = terrain.heightMapInfo.GetHeight(new Vector3(position.X, 0, position.Z));
                    orientation = factoryAnchor.orientation;
                    oneFactory.Model = biodegradableFactoryModel;
                    oneFactory.ModelStates = biodegradableFactoryModelStates;
                    oneFactory.LevelTextures = biodegradableFactoryLevelTextures;
                    oneFactory.LoadContent(game, position, orientation, ref IngamePresentation.factoryFont, ref IngamePresentation.factoryBackground, biofactoryAnimationTextures);
                    HydroBot.numResources -= GameConstants.numResourcesForBioFactory;
                    factories.Add(oneFactory);
                    status = true;

                    break;

                case BuildingType.plastic:
                    oneFactory = new Factory(FactoryType.plastic, particleManager, GraphicDevice);
                    position.Y = terrain.heightMapInfo.GetHeight(new Vector3(position.X, 0, position.Z));
                    orientation = factoryAnchor.orientation;
                    oneFactory.Model = plasticFactoryModel;                 // set the model so that bounding sphere calculation happens based on fully blown model
                    oneFactory.ModelStates = plasticFactoryModelStates;     // set different model states so that under construction states are handled
                    oneFactory.LevelTextures = plasticFactoryLevelTextures;
                    oneFactory.LoadContent(game, position, orientation, ref IngamePresentation.factoryFont, ref IngamePresentation.factoryBackground, nuclearFactoryAnimationTextures); // for time being reuse nuclear factory animation texture
                    HydroBot.numResources -= GameConstants.numResourcesForPlasticFactory;
                    factories.Add(oneFactory);
                    status = true;

                    break;
                case BuildingType.radioactive:
                    oneFactory = new Factory(FactoryType.radioactive, particleManager, GraphicDevice);
                    position.Y = terrain.heightMapInfo.GetHeight(new Vector3(position.X, 0, position.Z));
                    orientation = factoryAnchor.orientation;
                    oneFactory.Model = radioactiveFactoryModel;
                    oneFactory.ModelStates = radioactiveFactoryModelStates;
                    oneFactory.LoadContent(game, position, orientation, ref IngamePresentation.factoryFont, ref IngamePresentation.factoryBackground, nuclearFactoryAnimationTextures);
                    HydroBot.numResources -= GameConstants.numResourcesForRadioFactory;
                    factories.Add(oneFactory);
                    status = true;

                    break;
            }
            if (status)
            {
                // Play sound for successful addition of a building
                audio.OpenChest.Play();
                HydroBot.currentEnergy -= GameConstants.EnergyLostPerBuild;
                //env loss for building factory
                if (GameConstants.envLossPerFactoryBuilt > 0)
                {
                    HydroBot.currentEnvPoint -= GameConstants.envLossPerFactoryBuilt;
                    Point lossPoint = new Point();
                    lossPoint.LoadContent(Content, "-" + GameConstants.envLossPerFactoryBuilt + "ENV", position, Color.Red);
                    points.Add(lossPoint);
                }
            }

            return status;
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
            Rectangle rectSafeArea;
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;
            spriteBatch.Begin();
            if (currentGameState == GameState.Won)
                spriteBatch.Draw(winningTexture, GraphicDevice.Viewport.TitleSafeArea, Color.White);
            else
            {
                spriteBatch.Draw(losingTexture, GraphicDevice.Viewport.TitleSafeArea, Color.White);
                string losingText = "Sorry everyone, I could not do it...";
                Vector2 losingTextPos = new Vector2(game.Window.ClientBounds.Width / 2, 10 + IngamePresentation.statisticFont.MeasureString(losingText).Y / 2);
                spriteBatch.DrawString(IngamePresentation.statisticFont, losingText, losingTextPos, Color.Red, 0, new Vector2(IngamePresentation.statisticFont.MeasureString(losingText).X / 2, IngamePresentation.statisticFont.MeasureString(losingText).Y / 2), 1.5f, SpriteEffects.None, 0);      
            }
            string nextText = "Press Enter to continue";
            Vector2 nextTextPosition = new Vector2(rectSafeArea.Right - menuSmall.MeasureString(nextText).X - 70, rectSafeArea.Bottom - menuSmall.MeasureString(nextText).Y - 50);
            spriteBatch.DrawString(menuSmall, nextText, nextTextPosition, Color.White);
            spriteBatch.End();
        }

        private void DrawGameplayScreen(GameTime gameTime)
        {
            // Updating camera's frustum
            frustum = new BoundingFrustum(gameCamera.ViewMatrix * gameCamera.ProjectionMatrix);

            //preparing edge detecting for the object being pointed at
            graphicEffect.PrepareEdgeDetect(hydroBot, cursor, gameCamera, fish, fishAmount, enemies, enemiesAmount, trashes, null, factories, researchFacility, null,
                powerpacks, resources, graphics.GraphicsDevice, normalDepthRenderTargetLow, normalDepthRenderTargetHigh);
            //normal drawing of the game scene
            graphics.GraphicsDevice.SetRenderTarget(renderTarget);
            graphics.GraphicsDevice.Clear(Color.Black);

            terrain.Draw(gameCamera);

            //applying edge detection for objects on low layer of the game
            graphicEffect.ApplyEdgeDetection(renderTarget, normalDepthRenderTargetLow, graphics.GraphicsDevice, edgeDetectionRenderTarget);
            RestoreGraphicConfig();
            DrawObjectsOnLowLayer();

            DrawLowPriorityObjectsOnHighLayer();

            //applying edge detection for objects on high layer of the game
            graphicEffect.ApplyEdgeDetection(edgeDetectionRenderTarget, normalDepthRenderTargetHigh, graphics.GraphicsDevice, renderTarget);
            RestoreGraphicConfig();
            DrawObjectsOnHighLayer();

            // Draw anchor if any. The anchor should appear below static interaction button, below the cursor, and below hydrobot
            DrawAnchor();

            DrawProjectTiles();
            hydroBot.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalShading");

            // draw bubbles
            foreach (Bubble bubble in bubbles)
            {
                bubble.Draw(spriteBatch, 1.0f);
            }

            //draw particle effects
            particleManager.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameTime);

            afterEffectsAppliedRenderTarget = graphicEffect.DrawWithEffects(gameTime, renderTarget, graphics);
            //graphicEffect.DrawWithEffects(gameTime, SceneTexture, graphics);
            graphics.GraphicsDevice.SetRenderTarget(afterEffectsAppliedRenderTarget);
            for (int i = 0; i < enemiesAmount; i++)
            {
                if (enemies[i].BoundingSphere.Intersects(frustum))
                {
                    if (enemies[i].stunned == true)
                    {
                        Vector3 placeToDraw = GraphicDevice.Viewport.Project(enemies[i].Position, gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity);
                        Vector2 drawPos = new Vector2(placeToDraw.X, placeToDraw.Y);
                        spriteBatch.Begin();
                        spriteBatch.Draw(stunnedIconTexture, drawPos, Color.White);
                        spriteBatch.End();
                        RestoreGraphicConfig();
                    }
                    if (enemies[i].isFleeing == true)
                    {
                        Vector3 placeToDraw = GraphicDevice.Viewport.Project(enemies[i].Position, gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity);
                        Vector2 drawPos = new Vector2(placeToDraw.X, placeToDraw.Y);
                        spriteBatch.Begin();
                        spriteBatch.Draw(scaredIconTexture, drawPos, Color.White);
                        spriteBatch.End();
                        RestoreGraphicConfig();
                    }
                }
            }

            //draw boundary of the game scene
            //gameBoundary.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);

            //Draw points gained / lost
            foreach (Point point in points)
            {
                point.Draw(spriteBatch, GraphicDevice, gameCamera);
            }
            spriteBatch.Begin();
            if (!openFactoryConfigurationScene && !openResearchFacilityConfigScene)
            {
                DrawStats();
                DrawBulletType();
                //DrawHeight();
                DrawRadar();
                // Draw the factory panel
                factoryButtonPanel.Draw(spriteBatch);
                //factoryButtonPanel.DrawAnchor(spriteBatch);

                if (HydroBot.activeSkillID != -1) DrawActiveSkill();
                IngamePresentation.DrawCollectionStatus(GraphicDevice, spriteBatch);
                IngamePresentation.DrawHydroBotStatus(GraphicDevice, spriteBatch);
                IngamePresentation.DrawLevelObjectiveIcon(GraphicDevice, spriteBatch);
            }
            if (openFactoryConfigurationScene)
                factoryToConfigure.DrawFactoryConfigurationScene(spriteBatch, menuSmall);
            if (openResearchFacilityConfigScene)
                researchFacility.DrawResearchFacilityConfigurationScene(spriteBatch, menuSmall);

            cursor.Draw(gameTime);
            spriteBatch.End();
            graphics.GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin();
            spriteBatch.Draw(afterEffectsAppliedRenderTarget, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
            spriteBatch.End();
        }

        private void DrawRadar()
        {
            radar.Draw(spriteBatch, hydroBot.Position, enemies, enemiesAmount, fish, fishAmount, null, factories, researchFacility, powerpacks, null);
        }


        private void DrawStats()
        {
            float xOffsetText, yOffsetText;
            
            string str1 = GameConstants.ScoreAchieved;
            string str2 = "";// = GameConstants.StrCellsFound + retrievedFruits.ToString() +
            //" of " + fruits.Count;
            Rectangle rectSafeArea;
            str1 += ((int)score).ToString() + " High Score: " + ((int)highestScore).ToString();

            //too much texts on screen 
            IngamePresentation.DrawObjectPointedAtStatus(GraphicDevice, cursor, gameCamera, this.game, spriteBatch, fish, fishAmount, enemies, enemiesAmount, trashes, null, factories, researchFacility, null, powerpacks, resources);

            //Display Cyborg health
            //IngamePresentation.DrawHealthBar(game, spriteBatch, statsFont, (int)HydroBot.currentHitPoint, (int)HydroBot.maxHitPoint, game.Window.ClientBounds.Height - 5 - IngamePresentation.experienceBarHeight - 10 - IngamePresentation.healthBarHeight, "HEALTH", 1.0f);

            //Display Environment Bar
            if (HydroBot.currentEnvPoint > HydroBot.maxEnvPoint) HydroBot.currentEnvPoint = HydroBot.maxEnvPoint;
            IngamePresentation.DrawEnvironmentBar(game, spriteBatch, statsFont, HydroBot.currentEnvPoint, HydroBot.maxEnvPoint);

            //Display Level/Experience Bar
            //IngamePresentation.DrawLevelBar(game, spriteBatch, HydroBot.currentExperiencePts, HydroBot.nextLevelExperience, HydroBot.level, game.Window.ClientBounds.Height - 5, "EXPERIENCE LEVEL", Color.Brown);

            //Display Good will bar
            IngamePresentation.DrawGoodWillBar(game, spriteBatch, statsFont);

            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            xOffsetText = rectSafeArea.X;
            yOffsetText = rectSafeArea.Y;

            Vector2 strSize = statsFont.MeasureString(str1);
            Vector2 strPosition =
                new Vector2((int)xOffsetText + 10, (int)yOffsetText);

            spriteBatch.DrawString(menuSmall, str1, strPosition, Color.Red);
            strPosition.Y += strSize.Y;
            spriteBatch.DrawString(statsFont, str2, strPosition, Color.White);
        }

        // Draw the currently selected bullet type
        private void DrawBulletType()
        {
            //int xOffsetText, yOffsetText;
            //Rectangle rectSafeArea;

            ////Calculate str1 position
            //rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            ////xOffsetText = rectSafeArea.Left + 325;
            //xOffsetText = rectSafeArea.Center.X - 150 - 64;
            //yOffsetText = rectSafeArea.Bottom - 80;

            ////Vector2 bulletIconPosition =
            ////    new Vector2((int)xOffsetText, (int)yOffsetText);
            //Rectangle destRectangle = new Rectangle(xOffsetText, yOffsetText, 64, 64);
            ////spriteBatch.Draw(bulletTypeTextures[tank.bulletType], bulletIconPosition, Color.White);
            //spriteBatch.Draw(bulletTypeTextures[HydroBot.bulletType], destRectangle, Color.White);
            IngamePresentation.DrawBulletType(GraphicDevice, spriteBatch);
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

        public void dumpTrashInFactory(Factory factory, Vector3 position)
        {
            string point_string = "";
            switch (factory.factoryType)
            {
                case FactoryType.biodegradable:
                    point_string = HydroBot.bioTrash + " biodegradable\ntrash dumped";
                    factory.numTrashWaiting += HydroBot.bioTrash;
                    break;
                case FactoryType.plastic:
                    point_string = HydroBot.plasticTrash + " plastic\ntrash dumped";
                    factory.numTrashWaiting += HydroBot.plasticTrash;
                    break;
                case FactoryType.radioactive:
                    point_string = HydroBot.nuclearTrash + " radioactive\ntrash dumped";
                    factory.numTrashWaiting += HydroBot.nuclearTrash;
                    break;
            }
            Point point = new Point();
            point.LoadContent(PoseidonGame.contentManager, point_string, position, Color.LawnGreen);
            points.Add(point);
        }
        public void DrawObjectsOnLowLayer()
        {
            // Drawing sinked trash
            BoundingSphere trashRealSphere;
            foreach (Trash trash in trashes)
            {
                if (!trash.sinking)
                {
                    trashRealSphere = trash.BoundingSphere;
                    trashRealSphere.Center.Y = trash.Position.Y;
                    if (trashRealSphere.Intersects(frustum))
                    {
                        trash.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalShading");
                        //RasterizerState rs = new RasterizerState();
                        //rs.FillMode = FillMode.WireFrame;
                        //GraphicDevice.RasterizerState = rs;
                        //trash.DrawBoundingSphere(gameCamera.ViewMatrix,
                        //    gameCamera.ProjectionMatrix, boundingSphere);

                        //rs = new RasterizerState();
                        //rs.FillMode = FillMode.Solid;
                        //GraphicDevice.RasterizerState = rs;
                    }
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
                    factory.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalShading");
                }
            }
            if (researchFacility != null)
            {
                factoryRealSphere = researchFacility.BoundingSphere;
                factoryRealSphere.Center.Y = researchFacility.Position.Y;
                if (factoryRealSphere.Intersects(frustum))
                {
                    researchFacility.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalShading");
                    //researchFacility.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "BalloonShading");
                }
            }
        }

        public void DrawObjectsOnHighLayer()
        {
            for (int i = 0; i < enemiesAmount; i++)
            {
                if (enemies[i].BoundingSphere.Intersects(frustum))
                {
                    enemies[i].Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalShading");
                    //RasterizerState rs = new RasterizerState();
                    //rs.FillMode = FillMode.WireFrame;
                    //GraphicDevice.RasterizerState = rs;
                    //enemies[i].DrawBoundingSphere(gameCamera.ViewMatrix,
                    //    gameCamera.ProjectionMatrix, boundingSphere);

                    //rs = new RasterizerState();
                    //rs.FillMode = FillMode.Solid;
                    //GraphicDevice.RasterizerState = rs;
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
            // Drawing sinking trash
            BoundingSphere trashRealSphere;
            foreach (Trash trash in trashes)
            {
                if (trash.sinking)
                {
                    trashRealSphere = trash.BoundingSphere;
                    trashRealSphere.Center.Y = trash.Position.Y;
                    if (trashRealSphere.Intersects(frustum))
                    {
                        trash.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalShading");
                        //RasterizerState rs = new RasterizerState();
                        //rs.FillMode = FillMode.WireFrame;
                        //GraphicDevice.RasterizerState = rs;
                        //trash.DrawBoundingSphere(gameCamera.ViewMatrix,
                        //    gameCamera.ProjectionMatrix, boundingSphere);

                        //rs = new RasterizerState();
                        //rs.FillMode = FillMode.Solid;
                        //GraphicDevice.RasterizerState = rs;
                    }
                }
            }
        }

        public void DrawLowPriorityObjectsOnHighLayer()
        {
            foreach (Powerpack f in powerpacks)
            {
                if (f.BoundingSphere.Intersects(frustum))
                {
                    f.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalShading");
                    //RasterizerState rs = new RasterizerState();
                    //rs.FillMode = FillMode.WireFrame;
                    //GraphicDevice.RasterizerState = rs;
                    //f.DrawBoundingSphere(gameCamera.ViewMatrix,
                    //    gameCamera.ProjectionMatrix, boundingSphere);

                    //rs = new RasterizerState();
                    //rs.FillMode = FillMode.Solid;
                    //GraphicDevice.RasterizerState = rs;
                }
            }
            foreach (Resource r in resources)
            {
                if (!r.Retrieved && r.BoundingSphere.Intersects(frustum))
                {
                    r.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalShading");
                }
            }
        }
        public void DrawProjectTiles()
        {
            //GraphicDevice.RasterizerState = rs;
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
                    if (enemyBullet[i] is Torpedo)
                        enemyBullet[i].draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalShading");
                    else enemyBullet[i].draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
                }
            }

            for (int i = 0; i < alliesBullets.Count; i++)
            {
                if (alliesBullets[i].BoundingSphere.Intersects(frustum))
                {
                    alliesBullets[i].draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
                }
            }
        }
        private void UpdateAnchor()
        {
            // check if anchor need to be instantiated or updated
            if (factoryButtonPanel.AnchoredIndex != factoryButtonPanel.PreviousAnchoredIndex)
            {
                factoryAnchor = null;
                researchAnchor = null;

                Vector3 anchorPosition = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);
                float orientation;
                if (factoryButtonPanel.anchorIndexToBuildingType() == BuildingType.researchlab)
                {
                    researchAnchor = new ResearchFacility(particleManager);
                    anchorPosition.Y = terrain.heightMapInfo.GetHeight(new Vector3(anchorPosition.X, 0, anchorPosition.Z));
                    orientation = (float)(Math.PI / 2) * random.Next(4);
                    researchAnchor.Model = researchBuildingModel;
                    researchAnchor.LoadContent(game, anchorPosition, orientation);
                }
                else
                {
                    FactoryType typeOfFactory = factoryButtonPanel.anchorIndexToFactoryType(factoryButtonPanel.AnchoredIndex);
                    factoryAnchor = new Factory(typeOfFactory, particleManager, GraphicDevice);
                    anchorPosition.Y = terrain.heightMapInfo.GetHeight(new Vector3(anchorPosition.X, 0, anchorPosition.Z));
                    orientation = (float)(Math.PI / 2) * random.Next(4);
                    switch (typeOfFactory)
                    {
                        case FactoryType.biodegradable:
                            factoryAnchor.Model = biodegradableFactoryModel; // need to make it different for each type
                            break;
                        case FactoryType.plastic:
                            factoryAnchor.Model = plasticFactoryModel;
                            break;
                        case FactoryType.radioactive:
                            factoryAnchor.Model = radioactiveFactoryModel;
                            break;
                    }
                    factoryAnchor.LoadContent(game, anchorPosition, orientation, ref IngamePresentation.factoryFont, ref IngamePresentation.dummyTexture, null);
                }
            }
            else
            {
                if (factoryAnchor != null)
                {
                    factoryAnchor.Position = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);
                    factoryAnchor.Position.Y = terrain.heightMapInfo.GetHeight(new Vector3(factoryAnchor.Position.X, 0, factoryAnchor.Position.Z));
                }
                else if (researchAnchor != null)
                {
                    researchAnchor.Position = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);
                    researchAnchor.Position.Y = terrain.heightMapInfo.GetHeight(new Vector3(researchAnchor.Position.X, 0, researchAnchor.Position.Z));
                }
            }
        }
        private void DrawAnchor()
        {
            //factoryButtonPanel.DrawAnchor(spriteBatch);
            //return;

            if (factoryButtonPanel.hasAnyAnchor() && factoryButtonPanel.cursorOutsidePanelArea)
            {
                if (factoryButtonPanel.anchorIndexToBuildingType() == BuildingType.researchlab && researchAnchor != null)
                {
                    // Static object gameCamera inside PlayGameScene object
                    //researchAnchor.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalShading");
                    researchAnchor.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "CustomAlphaShading");
                }
                else if (factoryAnchor != null)
                {
                    //factoryAnchor.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalShading");
                    factoryAnchor.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "CustomAlphaShading");
                }
            }
        }
    }
}
