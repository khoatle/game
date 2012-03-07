using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Poseidon.Core;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Poseidon.FishSchool;
using Poseidon.GraphicEffects;

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
        //public static GameTime timming;
        
        private Texture2D HealthBar;
        private Texture2D EnvironmentBar;
        private Texture2D foundKeyScreen;

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
        SpriteFont keyFoundFont;
        SpriteFont menuSmall;

        public static Camera gameCamera;
        public static GameState currentGameState = GameState.PlayingCutScene;
     
        // In order to know we are resetting the level winning or losing
        // winning: keep the current bot
        // losing: reset our bot to the bot at the beginning of the level
        GameState prevGameState;
        GameObject boundingSphere;

        public int type = 0;

        public Terrain terrain;
        public List<ShipWreck> shipWrecks;

        public List<DamageBullet> myBullet;
        public List<DamageBullet> alliesBullets;
        public List<DamageBullet> enemyBullet;
        public List<HealthBullet> healthBullet;

        List<Powerpack> powerpacks;
        List<Resource> resources;
        List<Trash> trashes;
        List<Factory> factories;
        Factory factoryAnchor;
        public ResearchFacility researchFacility;
        ResearchFacility researchAnchor;

        List<StaticObject> staticObjects;

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

        protected Texture2D levelObjectiveIconTexture;
        Rectangle levelObjectiveIconRectangle;

        protected Texture2D tipIconTexture;
        Rectangle tipIconRectangle;

        // Current game level
        public static int currentLevel;

        Radar radar;

        // Frustum of the camera
        public BoundingFrustum frustum;

        private Texture2D stunnedIconTexture, scaredIconTexture;

        // to know whether the big boss has been terminated
        // and the level is won
        public static bool isBossKilled = false;

        //stuffs for applying special effects
        RenderTarget2D renderTarget, afterEffectsAppliedRenderTarget;
        public RenderTarget2D renderTarget2, cutSceneImmediateRenderTarget, cutSceneFinalRenderTarget;
        public Texture2D Scene2Texture;
        RenderTarget2D renderTarget3;
        public bool screenTransitNow = false;

        // Bubbles over characters
        List<Bubble> bubbles;
        float timeNextBubble = 200.0f;
        float timeNextSeaBedBubble = 3000.0f;

        //Points gained over trash, plant, enemies, fish
        public static List<Point> points;

        // School of fish
        SchoolOfFish schoolOfFish1;
        SchoolOfFish schoolOfFish2;
        SchoolOfFish schoolOfFish3;

        // the fishes have helped the player to find the treasure chest key!
        // only show once
        bool showFoundKey = false;
        bool firstShow = true;
        public static bool hadkey = false;

        //for drawing winning or losing scenes
        Texture2D winningTexture, losingTexture;

        //for drawing cutscenes
        Texture2D botFace;
        Texture2D otherPersonFace;
        Texture2D talkingBox;
        // Cutscene
        CutSceneDialog cutSceneDialog;
        // Which sentence in the dialog is being printed
        int currentSentence = 0;

        // For mouse inputs
        bool doubleClicked = false;
        bool clicked = false;
        double clickTimer = 0;

        private bool openFactoryConfigurationScene = false;
        private bool openResearchFacilityConfigScene = false;
        private Factory factoryToConfigure;

        // Texture and font for property window of a factory
        private SpriteFont factoryFont;
        private Texture2D factoryBackground;
        private Texture2D factoryProduceButton;
        private Texture2D dummyTexture;

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

        // Models for Factories and buildings
        private Model researchBuildingModel;
        private Model plasticFactoryModel;
        private List<Model> plasticFactoryModelStates;
        private Model biodegradableFactoryModel;
        private Model radioactiveFactoryModel;

        //Models for Trash
        private Model biodegradableTrash, plasticTrash, radioactiveTrash;

        //Models for powerpacks, strangeRock and resource
        private Model[] powerpackModels;
        private Model[] strangeRockModels;
        private Model resourceModel;

        // For applying graphic effects
        public GraphicEffect graphicEffect;
        //for particle systems
        public static ParticleManagement particleManager;

        //for edge detection effect
        RenderTarget2D normalDepthRenderTargetLow, normalDepthRenderTargetHigh, edgeDetectionRenderTarget;

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
            this.stunnedIconTexture = stunnedTexture;
            roundTime = GameConstants.RoundTime[currentLevel];
            random = new Random();

            gameCamera = new Camera(GameMode.MainGame);
            boundingSphere = new GameObject();
            hydroBot = new HydroBot(GameConstants.MainGameMaxRangeX, GameConstants.MainGameMaxRangeZ, GameConstants.MainGameFloatHeight, GameMode.MainGame);

            if (PoseidonGame.gamePlus)
            {
                ObjectsToSerialize objectsToSerialize = new ObjectsToSerialize();
                Serializer serializer = new Serializer();
                string SavedFile;
                if (PlayGameScene.currentLevel == 0)
                    SavedFile = "SurvivalMode";
                else
                    SavedFile = "GamePlusLevel" + PlayGameScene.currentLevel.ToString();
                objectsToSerialize = serializer.DeSerializeObjects(SavedFile);
                hydroBot = objectsToSerialize.hydrobot;
                //if (PlayGameScene.currentLevel == 0)
                //{
                //    for (int index = 0; index < GameConstants.numberOfSkills; index++)
                //    {
                //        HydroBot.skills[index] = false;
                //    }
                //}
                currentGameState = GameState.PlayingCutScene;
            }
            else if (PlayGameScene.currentLevel > 0)
            {
                ObjectsToSerialize objectsToSerialize = new ObjectsToSerialize();
                Serializer serializer = new Serializer();
                string SavedFile = "GameLevel" + PlayGameScene.currentLevel.ToString();
                objectsToSerialize = serializer.DeSerializeObjects(SavedFile);
                hydroBot = objectsToSerialize.hydrobot;
                currentGameState = GameState.PlayingCutScene;
            }
            //stop spinning the bar
            IngamePresentation.StopSpinning();

            if (PoseidonGame.gamePlus)
            {
                int[] numShootingEnemies = { HydroBot.gamePlusLevel * 5, 5 + (HydroBot.gamePlusLevel * 5), 10 + (HydroBot.gamePlusLevel * 5), 15, 15, 30, 30, 30, 30, 75, 15, 15 };
                GameConstants.NumberShootingEnemies = numShootingEnemies;
                int[] numCombatEnemies =   { HydroBot.gamePlusLevel * 5, 5 + (HydroBot.gamePlusLevel * 5), 10 + (HydroBot.gamePlusLevel * 5), 15, 15, 30, 30, 30, 30, 75, 15, 15 };
                GameConstants.NumberCombatEnemies = numCombatEnemies;
                int[] numGhostPirates = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                GameConstants.NumberGhostPirate = numGhostPirates;
                int[] numFish = { 50, 50, 50, 0, 50, 50, 50, 50, 50, 0, 0, 0 };
                GameConstants.NumberFish = numFish;
                int[] numMutantShark = { 0, 0, 0, 1, 1, 2 + HydroBot.gamePlusLevel, 3 + HydroBot.gamePlusLevel, 4 + HydroBot.gamePlusLevel, 5 + HydroBot.gamePlusLevel, 10 + HydroBot.gamePlusLevel, 0, HydroBot.gamePlusLevel };
                GameConstants.NumberMutantShark = numMutantShark;
                int[] numTerminator = { 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 1 };
                GameConstants.NumberTerminator = numTerminator;
                int[] numSubmarine = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                GameConstants.NumberSubmarine = numSubmarine;
            } 
            else {
                int[] numShootingEnemies = { 0, 5, 10, 0, 15, 20, 20, 20, 20, 50, 10, 10 };
                GameConstants.NumberShootingEnemies = numShootingEnemies;
                int[] numCombatEnemies = { 0, 5, 10, 0, 15, 20, 20, 20, 20, 50, 10, 10 };
                GameConstants.NumberCombatEnemies = numCombatEnemies;
                int[] numGhostPirates = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                GameConstants.NumberGhostPirate = numGhostPirates;
                int[] numFish = { 50, 50, 50, 0, 50, 50, 50, 50, 50, 0, 0, 0 };
                GameConstants.NumberFish = numFish;
                int[] numMutantShark = { 0, 0, 0, 1, 1, 2, 3, 4, 5, 10, 0, 0 };
                GameConstants.NumberMutantShark = numMutantShark;
                int[] numTerminator = { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 1 };
                GameConstants.NumberTerminator = numTerminator;
                int[] numSubmarine = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                GameConstants.NumberSubmarine = numSubmarine;
            }

            skillTextures = new Texture2D[GameConstants.numberOfSkills];
            bulletTypeTextures = new Texture2D[GameConstants.numBulletTypes];

            // for the mouse or touch
            cursor = new Cursor(game, spriteBatch);
            //Components.Add(cursor);

            bubbles = new List<Bubble>();
            points = new List<Point>();

            schoolOfFish1 = new SchoolOfFish(Content, "Image/FishSchoolTextures/smallfish1", 100, GameConstants.MainGameMaxRangeX - 250,
                100, GameConstants.MainGameMaxRangeZ - 250);
            schoolOfFish2 = new SchoolOfFish(Content, "Image/FishSchoolTextures/smallfish2-1", -GameConstants.MainGameMaxRangeX + 250, -100,
                -GameConstants.MainGameMaxRangeZ + 250, -100);
            schoolOfFish3 = new SchoolOfFish(Content, "Image/FishSchoolTextures/smallfish3", -GameConstants.MainGameMaxRangeX + 250, -100,
                100, GameConstants.MainGameMaxRangeZ - 250);

            //loading winning, losing textures
            winningTexture = Content.Load<Texture2D>("Image/SceneTextures/LevelWin");
            losingTexture = Content.Load<Texture2D>("Image/SceneTextures/GameOver");
            scaredIconTexture = Content.Load<Texture2D>("Image/Miscellaneous/scared-icon");

            // Instantiate the factory Button
            float buttonScale = 1.0f;
            if (game.Window.ClientBounds.Width <= 900) {
                buttonScale = 0.8f; // scale the factory panel icons a bit smaller in small window mode
            }
            factoryButtonPanel = new ButtonPanel(4, buttonScale);

            this.Load();
            //System.Diagnostics.Debug.WriteLine("In playgamescene init CurrentExp:" + HydroBot.currentExperiencePts);
            //System.Diagnostics.Debug.WriteLine("In playgamescene init NextExp:" + HydroBot.nextLevelExperience);
        }


        public void Load()
        {
            statsFont = Content.Load<SpriteFont>("Fonts/StatsFont");
            menuSmall = Content.Load<SpriteFont>("Fonts/menuSmall");
            fishTalkFont = Content.Load<SpriteFont>("Fonts/fishTalk");
            keyFoundFont = Content.Load<SpriteFont>("Fonts/painting");
            // Get the audio library
            audio = (AudioLibrary)
                Game.Services.GetService(typeof(AudioLibrary));

            //Uncomment below line to use LEVELS
            //string terrain_name = "Image/terrain" + currentLevel;

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

            levelObjectiveIconTexture = Content.Load<Texture2D>("Image/Miscellaneous/LevelObjectiveIcon");
            tipIconTexture = Content.Load<Texture2D>("Image/Miscellaneous/tipIcon");

            foundKeyScreen = Content.Load<Texture2D>("Image/SceneTextures/keyfound");

            powerpacks = new List<Powerpack>();
            resources = new List<Resource>();

            hydroBot.Load(Content);

            //prevTank.Load(Content);
            roundTimer = roundTime;

            //Load healthbar
            HealthBar = Content.Load<Texture2D>("Image/Miscellaneous/HealthBar");
            EnvironmentBar = Content.Load<Texture2D>("Image/Miscellaneous/EnvironmentBar");

            // initialize render targets
            PresentationParameters pp = graphics.GraphicsDevice.PresentationParameters;
            renderTarget = new RenderTarget2D(graphics.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, 
                false, graphics.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24Stencil8);
            renderTarget2 = new RenderTarget2D(graphics.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight,
                false, graphics.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24Stencil8);
            renderTarget3 = new RenderTarget2D(graphics.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight,
                false, graphics.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24Stencil8);
            afterEffectsAppliedRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight,
                false, graphics.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24Stencil8);
            cutSceneImmediateRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight,
                false, graphics.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24Stencil8);
            cutSceneFinalRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight,
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
            factoryPanelTexture = Content.Load<Texture2D>("Image/ButtonTextures/factory_button");
            // Load Font for displaying extra information on factory panel
            factoryPanelFont = Content.Load<SpriteFont>("Fonts/panelInfoText");

            // Load Textures and fonts for factory property dialog
            factoryFont = Content.Load<SpriteFont>("Fonts/factoryConfig");
            factoryBackground = Content.Load<Texture2D>("Image/TrashManagement/factory_config_background");
            factoryProduceButton = Content.Load<Texture2D>("Image/TrashManagement/ChangeFactoryProduceBox");

            // Load Textures and fonts for research facility property dialog
            facilityFont = Content.Load<SpriteFont>("Fonts/researchFacilityConfig");
            facilityFont2 = Content.Load<SpriteFont>("Fonts/researchFacilityConfig2");
            facilityBackground = Content.Load<Texture2D>("Image/TrashManagement/ResearchFacilityBackground");
            facilityUpgradeButton = Content.Load<Texture2D>("Image/TrashManagement/upgradeButton");
            playJigsawButton = Content.Load<Texture2D>("Image/TrashManagement/upgradeButton");
            increaseAttributeButton = Content.Load<Texture2D>("Image/TrashManagement/increaseAttributeButton");
        
            // Load textures for partid animation for factories
            biofactoryAnimationTextures = new List<Texture2D>();
            nuclearFactoryAnimationTextures = new List<Texture2D>();
            for (int i=0; i<6; i++) {
                biofactoryAnimationTextures.Add(Content.Load<Texture2D>("Image/TrashManagement/conveyor_bench" + i));
            }
            nuclearFactoryAnimationTextures.Add(Content.Load<Texture2D>("Image/TrashManagement/orange"));
            nuclearFactoryAnimationTextures.Add(Content.Load<Texture2D>("Image/TrashManagement/yellow"));

            // Load factory and research lab models
            researchBuildingModel = Content.Load<Model>("Models/FactoryModels/ResearchFacility");
            biodegradableFactoryModel = Content.Load<Model>("Models/FactoryModels/BiodegradableFactory");
            plasticFactoryModel = Content.Load<Model>("Models/FactoryModels/PlasticFactory");
            plasticFactoryModelStates = new List<Model>();
            for (int i = 0; i < 4; i++)
            {
                plasticFactoryModelStates.Add(Content.Load<Model>("Models/FactoryModels/PlasticFactory_stage" + i));
            }
            radioactiveFactoryModel = Content.Load<Model>("Models/FactoryModels/NuclearFactory");
            dummyTexture = new Texture2D(game.GraphicsDevice, 2, 2); // create a dummy 2x2 texture
            dummyTexture.SetData(new int[4]);

            // Load Trash
            biodegradableTrash = Content.Load<Model>("Models/TrashModels/biodegradableTrashVer2");
            plasticTrash = Content.Load<Model>("Models/TrashModels/plasticTrashVer2");
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

            //Initialize the game field
            InitializeGameField(Content);

        }

        //Below stuff uses GraphicsDevice which can not be called from Load, as it gives runtime error "GraphicDevice not initialized"
        protected override void LoadContent()
        {
            // Initialie the button panel
            factoryButtonPanel.Initialize(ref factoryPanelTexture, ref factoryPanelFont, new Vector2(10, GraphicsDevice.Viewport.Height - 70));
        }

        /// <summary>
        /// Show the action scene
        /// </summary>
        public override void Show()
        {
            paused = false;
            HydroBot.gameMode = GameMode.MainGame;
            base.Show();
        }

        private void ResetGame(GameTime gameTime, float aspectRatio)
        {
            string filenamePrefix;
            if (PoseidonGame.gamePlus)
                filenamePrefix = "GamePlusLevel";
            else
                filenamePrefix = "GameLevel";

            if (prevGameState == GameState.Lost)
            {
                // player always lose in lv10
                if (currentLevel == 10)
                {
                    currentLevel++;
                    ObjectsToSerialize objectsToSerialize = new ObjectsToSerialize();
                    objectsToSerialize.hydrobot = hydroBot;
                    Serializer serializer = new Serializer();
                    serializer.SerializeObjects(filenamePrefix + currentLevel.ToString(), objectsToSerialize);
                    hydroBot.SetLevelStartValues();
                }
                else hydroBot.ResetToLevelStart();
            }
            else // Level won
            {
                ObjectsToSerialize objectsToSerialize = new ObjectsToSerialize();
                objectsToSerialize.hydrobot = hydroBot;

                Serializer serializer = new Serializer();
                serializer.SerializeObjects(filenamePrefix + currentLevel.ToString(), objectsToSerialize);

                hydroBot.SetLevelStartValues();
            }

            hydroBot.Reset();
            if (currentLevel == 11) HydroBot.bulletType = 0;

            cursor.targetToLock = null;
            MediaPlayer.Stop();
            roundTime = GameConstants.RoundTime[currentLevel];
            roundTimer = roundTime;
            isBossKilled = false;
            
            //User must find the key at every level
            firstShow = true;
            showFoundKey = false;
            hadkey = false;
            screenTransitNow = false;
            graphicEffect.resetTransitTimer();

            terrain = new Terrain(Content);

            // If we are resetting the level losing the game
            // Reset our bot to the one at the beginning of the lost level
            //if (prevGameState == GameState.Lost) tank.CopyAttribute(prevTank);
            //else prevTank.CopyAttribute(tank);

            gameCamera.Update(hydroBot.ForwardDirection,
                hydroBot.Position, aspectRatio, gameTime, cursor);

            //Clean all powerpacks and resources
            powerpacks.Clear();
            resources.Clear();

            startTime = gameTime.TotalGameTime;
            
            currentSentence = 0;
            currentGameState = GameState.PlayingCutScene;
            schoolOfFish1 = new SchoolOfFish(Content, "Image/FishSchoolTextures/smallfish1", 100, GameConstants.MainGameMaxRangeX - 250,
                100, GameConstants.MainGameMaxRangeZ - 250);
            schoolOfFish2 = new SchoolOfFish(Content, "Image/FishSchoolTextures/smallfish2-1", -GameConstants.MainGameMaxRangeX + 250, -100,
                -GameConstants.MainGameMaxRangeZ + 250, -100);
            schoolOfFish3 = new SchoolOfFish(Content, "Image/FishSchoolTextures/smallfish3", -GameConstants.MainGameMaxRangeX + 250, -100,
                100, GameConstants.MainGameMaxRangeZ - 250);

            //reset the shipwreck content too
            ShipWreckScene.resetShipWreckNow = true;

            InitializeGameField(Content);
        }

        private void InitializeGameField(ContentManager Content)
        {
            enemyBullet = new List<DamageBullet>();
            healthBullet = new List<HealthBullet>();
            myBullet = new List<DamageBullet>();
            alliesBullets = new List<DamageBullet>();

            //Initialize the ship wrecks
            shipWrecks = new List<ShipWreck>(GameConstants.NumberShipWreck[currentLevel]);
            int randomType = random.Next(3);
            // example of how to put skill into a ship wreck at a certain level
            // just put the skill into the 1st ship wrect in the list
            int relicType = -1;
            float orientation = random.Next(100);
            switch (currentLevel)
            {
                // learn 1st skill in level 2 and so on
                case 2:
                    relicType = 3;
                    break;
                case 5:
                    relicType = 0;
                    break;
                case 6:
                    relicType = 1;
                    break;
                case 7:
                    relicType = 2;
                    break;
                case 8:
                    relicType = 4;
                    break;
            }
            for (int index = 0; index < GameConstants.NumberShipWreck[currentLevel]; index++)
            {
                shipWrecks.Add(new ShipWreck());
                if (index == 0 && relicType != -1) shipWrecks[index].LoadContent(Content, randomType, relicType, orientation);
                else shipWrecks[index].LoadContent(Content, randomType, -1, orientation);
                //shipWrecks[index].LoadContent(Content, randomType, 1);
                randomType = random.Next(3);
                orientation = random.Next(100);
            }
            enemiesAmount = 0;
            fishAmount = 0;
            enemies = new BaseEnemy[GameConstants.NumberShootingEnemies[currentLevel] + GameConstants.NumberCombatEnemies[currentLevel] + GameConstants.NumberGhostPirate[currentLevel]
                + GameConstants.NumberMutantShark[currentLevel] + GameConstants.NumberTerminator[currentLevel] + GameConstants.NumberSubmarine[currentLevel]*(1 + GameConstants.NumEnemiesInSubmarine)];
            fish = new Fish[GameConstants.NumberFish[currentLevel] + 50]; // Possible 10 sidekicks
            AddingObjects.placeEnemies(ref enemiesAmount, enemies, Content, random, fishAmount, fish, shipWrecks,
                GameConstants.MainGameMinRangeX, GameConstants.MainGameMaxRangeX, GameConstants.MainGameMinRangeZ, GameConstants.MainGameMaxRangeZ, currentLevel, GameMode.MainGame, GameConstants.MainGameFloatHeight);
            AddingObjects.placeFish(ref fishAmount, fish, Content, random, enemiesAmount, enemies, shipWrecks,
                GameConstants.MainGameMinRangeX, GameConstants.MainGameMaxRangeX, GameConstants.MainGameMinRangeZ, GameConstants.MainGameMaxRangeZ, currentLevel, GameMode.MainGame, GameConstants.MainGameFloatHeight);
            //placeFuelCells();
            AddingObjects.placeShipWreck(shipWrecks, staticObjects, random, terrain.heightMapInfo,
                GameConstants.MainGameMinRangeX, GameConstants.MainGameMaxRangeX, GameConstants.MainGameMinRangeZ, GameConstants.MainGameMaxRangeZ);
            
            //Initialize trash
            //int random_model;
            int numberTrash = GameConstants.NumberBioTrash[currentLevel] + GameConstants.NumberNuclearTrash[currentLevel] + GameConstants.NumberPlasticTrash[currentLevel];
            trashes = new List<Trash>(numberTrash);
            int bioIndex, plasticIndex, nuclearIndex;
            for (bioIndex = 0; bioIndex < GameConstants.NumberBioTrash[currentLevel]; bioIndex++)
            {
                orientation = random.Next(100);
                trashes.Add(new Trash(TrashType.biodegradable));
                trashes[bioIndex].Load(Content,ref biodegradableTrash, orientation);
            }
            for (plasticIndex = bioIndex; plasticIndex < bioIndex+GameConstants.NumberPlasticTrash[currentLevel]; plasticIndex++)
            {
                orientation = random.Next(100);
                trashes.Add(new Trash(TrashType.plastic));
                trashes[plasticIndex].Load(Content,ref plasticTrash, orientation); //nuclear model
            }
            for (nuclearIndex = plasticIndex; nuclearIndex< plasticIndex + GameConstants.NumberNuclearTrash[currentLevel]; nuclearIndex++)
            {
                orientation = random.Next(100);
                trashes.Add(new Trash(TrashType.radioactive));
                trashes[nuclearIndex].Load(Content,ref radioactiveTrash, orientation); //organic model
            }

            AddingObjects.placeTrash(ref trashes, Content, random, shipWrecks, staticObjects,
                GameConstants.TrashMinRangeX, GameConstants.MainGameMaxRangeX - 80, GameConstants.TrashMinRangeZ,
                GameConstants.MainGameMaxRangeZ - 60, GameMode.MainGame, GameConstants.MainGameFloatHeight, terrain.heightMapInfo); 

            // Initialize a list of factories
            factories = new List<Factory>();
            
            // Set research facility to null
            researchFacility = null;

            //Initialize the static objects.
            staticObjects = new List<StaticObject>(GameConstants.NumStaticObjectsMain);
            for (int index = 0; index < GameConstants.NumStaticObjectsMain; index++)
            {
                staticObjects.Add(new StaticObject());
                int randomObject = random.Next(3);
                switch (randomObject)
                {
                    case 0:
                        staticObjects[index].LoadContent(Content, "Models/chest");
                        break;
                    case 1:
                        staticObjects[index].LoadContent(Content, "Models/plant");
                        break;
                    case 2:
                        staticObjects[index].LoadContent(Content, "Models/plant2");
                        break;
                }
                staticObjects[index].LoadContent(Content, "Models/barrelstack");
            }
            AddingObjects.PlaceStaticObjects(staticObjects, shipWrecks, random, terrain.heightMapInfo, GameConstants.MainGameMinRangeX,
                GameConstants.MainGameMaxRangeX, GameConstants.MainGameMinRangeZ, GameConstants.MainGameMaxRangeZ);

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
            if ((Keyboard.GetState()).IsKeyDown(Keys.Insert) && type < 3) {
                HydroBot.turtlePower = HydroBot.seaCowPower = HydroBot.dolphinPower = 1.0f;
                AddingObjects.placeMinion(Content, type, enemies, enemiesAmount, fish, ref fishAmount, hydroBot);
                type++;
            }

            // play the boss fight music for certain levels
            if (currentLevel == 3 || currentLevel == 11)
            {

                if (MediaPlayer.State.Equals(MediaState.Stopped))
                {
                    MediaPlayer.Play(audio.bossMusics[random.Next(GameConstants.NumBossBackgroundMusics)]);
                }
            }
            else if (MediaPlayer.State.Equals(MediaState.Stopped))
            {
                MediaPlayer.Play(audio.backgroundMusics[random.Next(GameConstants.NumNormalBackgroundMusics)]);
            }
            if (!paused)
            {
                //timming = gameTime;
                float aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
                lastKeyboardState = currentKeyboardState;
                currentKeyboardState = Keyboard.GetState();
                // Allows the game to exit
                //if ((currentKeyboardState.IsKeyDown(Keys.Escape)) ||
                //    (currentGamePadState.Buttons.Back == ButtonState.Pressed))
                //    //this.Exit();
               

                if (currentGameState == GameState.PlayingCutScene)
                {
                    // Next sentence when the user press Enter
                    if (lastKeyboardState.IsKeyDown(Keys.Enter) &&
                        currentKeyboardState.IsKeyUp(Keys.Enter))
                    {
                        currentSentence++;
                        // End of cutscene for this level
                        if (currentSentence == cutSceneDialog.cutScenes[currentLevel].Count)
                        {
                            currentGameState = GameState.Running;
                            if (currentLevel == 12)
                            {
                                currentGameState = GameState.GameComplete;
                                HydroBot.gamePlusLevel++;
                                //the save file used for Survival mode
                                ObjectsToSerialize objectsToSerialize = new ObjectsToSerialize();
                                objectsToSerialize.hydrobot = hydroBot;
                                Serializer serializer = new Serializer();
                                serializer.SerializeObjects("SurvivalMode", objectsToSerialize);
                            }
                            else
                            {
                               // screenTransitNow = true;
                                
                            }
                            
                        }
                        graphicEffect.resetTransitTimer();
                        screenTransitNow = true;
                    }
                }
                if (currentGameState == GameState.ToNextLevel)
                {
                    ResetGame(gameTime, aspectRatio);
                }
                if ((currentGameState == GameState.Running))
                {
                    MouseState currentMouseState;
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
                    if (currentMouseState.RightButton == ButtonState.Pressed)
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
                        if (researchFacility != null && CursorManager.MouseOnObject(cursor, researchFacility.BoundingSphere, researchFacility.Position, gameCamera))
                        {
                            openResearchFacilityConfigScene = true;
                            ResearchFacility.dolphinLost = ResearchFacility.seaCowLost = ResearchFacility.turtleLost = false;
                            ResearchFacility.dolphinWon = ResearchFacility.seaCowWon = ResearchFacility.turtleWon = false;
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
                            clicked = false;
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
                                    if (ResearchFacility.playSeaCowJigsaw && researchFacility.playSeaCowJigsawRect.Intersects(new Rectangle(lastMouseState.X, lastMouseState.Y, 10, 10)))
                                    {
                                        PoseidonGame.playJigsaw = true;
                                        PoseidonGame.jigsawType = 0; //seacow
                                    }
                                    if (ResearchFacility.playTurtleJigsaw && researchFacility.playTurtleJigsawRect.Intersects(new Rectangle(lastMouseState.X, lastMouseState.Y, 10, 10)))
                                    {
                                        PoseidonGame.playJigsaw = true;
                                        PoseidonGame.jigsawType = 1; //turtle
                                    }
                                    if (ResearchFacility.playDolphinJigsaw && researchFacility.playDolphinJigsawRect.Intersects(new Rectangle(lastMouseState.X, lastMouseState.Y, 10, 10)))
                                    {
                                        PoseidonGame.playJigsaw = true;
                                        PoseidonGame.jigsawType = 2; //dolphin
                                    }
                                    if (HydroBot.unassignedPts>0 && researchFacility.increaseAttributeRect.Intersects(new Rectangle(lastMouseState.X, lastMouseState.Y, 10, 10)))
                                    {
                                        PoseidonGame.AttributeButtonPressed = true;
                                    }
                                }
                                clicked = false;
                            }
                            else if (openResearchFacilityConfigScene)
                            {
                                if (researchFacility.increaseAttributeRect.Contains(lastMouseState.X, lastMouseState.Y))
                                    researchFacility.mouseOnIncreaseAttributeIcon = true;
                                else
                                    researchFacility.mouseOnIncreaseAttributeIcon = false;
                            }
                            return;
                        }
                    }
                    if (currentLevel == 2 || currentLevel == 5 || currentLevel == 6 || currentLevel == 7 || currentLevel == 8)
                    {
                        if ((double)HydroBot.currentEnvPoint / (double)HydroBot.maxEnvPoint > GameConstants.EnvThresholdForKey)
                        {
                            showFoundKey = true;
                            hadkey = true;
                        }
                    }
                    if (showFoundKey && firstShow)
                    {
                        // return to game if enter pressed
                        if (lastKeyboardState.IsKeyDown(Keys.Enter) &&
                            (currentKeyboardState.IsKeyUp(Keys.Enter)))
                        {
                            showFoundKey = false;
                            firstShow = false;
                        }
                        return;
                    }

                    bool mouseOnInteractiveIcons = mouseOnLevelObjectiveIcon(currentMouseState) || mouseOnTipIcon(currentMouseState)
                        || (!factoryButtonPanel.cursorOutsidePanelArea) || factoryButtonPanel.hasAnyAnchor() || factoryButtonPanel.clickToBuildDetected;
                    //hydrobot update
                    hydroBot.UpdateAction(gameTime, cursor, gameCamera, enemies, enemiesAmount, fish, fishAmount, Content, spriteBatch, myBullet,
                        this, terrain.heightMapInfo, healthBullet, powerpacks, resources, trashes, shipWrecks, staticObjects, mouseOnInteractiveIcons);

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
                            aBubble.LoadContent(Content, bubblePos, true, (float) random.NextDouble() / 80);
                            bubbles.Add(aBubble);
                        }
                        //audio.Bubble.Play();
                        timeNextSeaBedBubble =  (random.Next(3) + 3) * 1000.0f;
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
                    for ( int i=0; i< points.Count; i++)
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

                    if (trashes!=null && trashes.Count < GameConstants.NumberTrash[currentLevel])
                    {
                        Vector3 pos = AddingObjects.createSinkingTrash(ref trashes, Content, random, shipWrecks, staticObjects, factories, researchFacility,
                                GameConstants.TrashMinRangeX, GameConstants.MainGameMaxRangeX - 80, GameConstants.TrashMinRangeZ,
                                GameConstants.MainGameMaxRangeZ - 60, GameConstants.MainGameFloatHeight, terrain.heightMapInfo,ref biodegradableTrash,ref plasticTrash,ref radioactiveTrash);
                        //Point point = new Point();
                        //point.LoadContent(PoseidonGame.contentManager, "New Trash Dropped", pos, Color.LawnGreen);
                        //points.Add(point);
                    }
                    foreach (Trash trash in trashes)
                    {
                        trash.Update(gameTime);
                        if (trash.sinking == false && trash.particleAnimationPlayed == false)
                        {
                            for (int k = 0; k < GameConstants.numSandParticles; k++)
                                particleManager.sandParticles.AddParticle(trash.Position, Vector3.Zero);
                            trash.particleAnimationPlayed = true;
                        }
                    }

                    CursorManager.CheckClick(ref this.lastMouseState, ref this.currentMouseState, gameTime, ref clickTimer, ref clicked, ref doubleClicked);
                    foreach (Factory factory in factories)
                    {
                        factory.Update(gameTime,ref powerpacks, ref resources, ref powerpackModels, ref resourceModel, ref strangeRockModels);
                        if(doubleClicked && hydroBot.BoundingSphere.Intersects(factory.BoundingSphere) && CursorManager.MouseOnObject(cursor, factory.BoundingSphere, factory.Position, gameCamera))
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
                        if (doubleClicked && hydroBot.BoundingSphere.Intersects(researchFacility.BoundingSphere) && CursorManager.MouseOnObject(cursor, researchFacility.BoundingSphere, researchFacility.Position, gameCamera) )
                        {
                            if (HydroBot.numStrangeObjCollected > 0)
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
                    }
                    doubleClicked = false;

                    foreach (ShipWreck shipWreck in shipWrecks)
                    {
                        if (shipWreck.BoundingSphere.Intersects(frustum) && shipWreck.seen == false)
                            shipWreck.seen = true;
                    }

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
                    Collision.updateDamageBulletVsBarriersCollision(myBullet, enemies, ref enemiesAmount, frustum, GameMode.MainGame, gameTime, hydroBot,
                        enemies, enemiesAmount, fish, fishAmount, gameCamera);
                    Collision.updateHealingBulletVsBarrierCollision(healthBullet, fish, fishAmount, frustum, GameMode.MainGame);
                    Collision.updateDamageBulletVsBarriersCollision(enemyBullet, fish, ref fishAmount, frustum, GameMode.MainGame, gameTime, hydroBot,
                        enemies, enemiesAmount, fish, fishAmount, gameCamera);
                    Collision.updateProjectileHitBot(hydroBot, enemyBullet, GameMode.MainGame, enemies, enemiesAmount, particleManager.explosionParticles, gameCamera, fish, fishAmount);
                    Collision.updateDamageBulletVsBarriersCollision(alliesBullets, enemies, ref enemiesAmount, frustum, GameMode.MainGame, gameTime, hydroBot,
                        enemies, enemiesAmount, fish, fishAmount, gameCamera);

                    Collision.deleteSmallerThanZero(enemies, ref enemiesAmount, frustum, GameMode.MainGame, cursor);
                    Collision.deleteSmallerThanZero(fish, ref fishAmount, frustum, GameMode.MainGame, cursor);

                    for (int i = 0; i < enemiesAmount; i++)
                    {
                        //disable stun if stun effect times out
                        if (enemies[i].stunned)
                        {
                            if (PoseidonGame.playTime.TotalSeconds - enemies[i].stunnedStartTime > GameConstants.timeStunLast)
                                enemies[i].stunned = false;
                        }
                        enemies[i].Update(enemies, ref enemiesAmount, fish, fishAmount, random.Next(100), hydroBot, enemyBullet, alliesBullets, frustum, gameTime, GameMode.MainGame);
                    }

                    for (int i = 0; i < fishAmount; i++)
                    {
                        fish[i].Update(gameTime, enemies, enemiesAmount, fish, fishAmount, random.Next(100), hydroBot, enemyBullet);
                    }

                    //Checking win/lost condition for this level
                    if (HydroBot.currentHitPoint <= 0) 
                    { 
                        currentGameState = GameState.Lost;
                        audio.gameOver.Play();
                    }

                    roundTimer -= gameTime.ElapsedGameTime;
                    PoseidonGame.playTime += gameTime.ElapsedGameTime;

                    if (CheckWinCondition())
                    {
                        currentGameState = GameState.Won;
                        audio.gameWon.Play();
                    }
                    if (CheckLoseCondition())
                    {
                        currentGameState = GameState.Lost;
                        audio.gameOver.Play();
                    }
                   
                    //cursor update
                    cursor.Update(GraphicDevice, gameCamera, gameTime, frustum);

                    //update the school of fish
                    schoolOfFish1.Update(gameTime, hydroBot, enemies, enemiesAmount, fish, fishAmount);
                    schoolOfFish2.Update(gameTime, hydroBot, enemies, enemiesAmount, fish, fishAmount);
                    schoolOfFish3.Update(gameTime, hydroBot, enemies, enemiesAmount, fish, fishAmount);

                    //update graphic effects
                    graphicEffect.UpdateInput(gameTime);
                    //update particle systems
                    particleManager.Update(gameTime);
                }

                prevGameState = currentGameState;
                if (currentGameState == GameState.Lost)
                {
                    // Reset the world for a new game
                    if (lastKeyboardState.IsKeyDown(Keys.Enter) &&
                        currentKeyboardState.IsKeyUp(Keys.Enter))
                    {
                        //the player should always lose in level 10
                        //if (currentLevel == 10)
                        //{
                        //    currentLevel++;
                        //}
                        //always reset the level when losing
                        ResetGame(gameTime, aspectRatio);
                    }
                }
                if (currentGameState == GameState.Won)
                {
                    if (lastKeyboardState.IsKeyDown(Keys.Enter) &&
                        currentKeyboardState.IsKeyUp(Keys.Enter))
                    {
                        currentLevel++;
                        if (currentLevel < 11)
                            currentGameState = GameState.ToMiniGame;
                        //play the last cutscene if the game has been completed
                        else
                        {
                            currentGameState = GameState.PlayingCutScene;
                            currentSentence = 0;
                        }
                        // no minigame for 1st level
                        if (currentLevel == 1)
                            currentGameState = GameState.ToNextLevel;
                        //ResetGame(gameTime, aspectRatio);
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

            // Check if hydrobot has sufficient resources for building a factory
            if (HydroBot.numResources < GameConstants.numResourcesForEachFactory)
            {
                // Play some sound hinting no sufficient resource
                audio.MenuScroll.Play();
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
                return false;
            }

            //Verify that current location is available for adding the building
            
            //int heightValue = (int)terrain.heightMapInfo.GetHeight(new Vector3(position.X, 0, position.Z));
            if (AddingObjects.IsSeaBedPlaceOccupied((int)position.X, 0, (int)position.Z, radius, shipWrecks, staticObjects, trashes, factories, researchFacility))
            {
                // Play some sound hinting seabed place is occupied
                audio.MenuScroll.Play();
                return false;
            }

            switch (buildingType)
            {
                case BuildingType.researchlab:
                    if (researchFacility != null)
                    {
                        // do not allow addition of more than one research facility
                        audio.MenuScroll.Play();
                        status = false;
                    }
                    else
                    {
                        //create research facility.. Only one is allowed, hence using a separate variable for this purpose.
                        researchFacility = new ResearchFacility();
                        position.Y = terrain.heightMapInfo.GetHeight(new Vector3(position.X, 0, position.Z));
                        //orientation = (float)(Math.PI/2) * random.Next(4);
                        orientation = researchAnchor.orientation;
                        researchFacility.Model = researchBuildingModel;
                        researchFacility.LoadContent(game, position, orientation,ref facilityFont,ref facilityFont2,ref facilityBackground,ref facilityUpgradeButton,ref playJigsawButton,ref increaseAttributeButton);
                        HydroBot.numResources -= GameConstants.numResourcesForEachFactory;
                        status = true;
                    }
                    break;

                case BuildingType.biodegradable:
                    oneFactory = new Factory(FactoryType.biodegradable);
                    position.Y = terrain.heightMapInfo.GetHeight(new Vector3(position.X, 0, position.Z));
                    orientation = factoryAnchor.orientation;
                    oneFactory.Model = biodegradableFactoryModel;
                    oneFactory.LoadContent(game, position, orientation,ref factoryFont,ref factoryBackground,ref factoryProduceButton, biofactoryAnimationTextures);
                    HydroBot.numResources -= GameConstants.numResourcesForEachFactory;
                    factories.Add(oneFactory);
                    status = true;
                    break;

                case BuildingType.plastic:
                    oneFactory = new Factory(FactoryType.plastic);
                    position.Y = terrain.heightMapInfo.GetHeight(new Vector3(position.X, 0, position.Z));
                    orientation = factoryAnchor.orientation;
                    oneFactory.Model = plasticFactoryModel;                 // set the model so that bounding sphere calculation happens based on fully blown model
                    oneFactory.ModelStates = plasticFactoryModelStates;     // set different model states so that under construction states are handled
                    oneFactory.LoadContent(game, position, orientation, ref factoryFont, ref factoryBackground, ref factoryProduceButton, nuclearFactoryAnimationTextures); // for time being reuse nuclear factory animation texture
                    HydroBot.numResources -= GameConstants.numResourcesForEachFactory;
                    factories.Add(oneFactory);
                    status = true;
                    break;
                case BuildingType.radioactive:
                    oneFactory = new Factory(FactoryType.radioactive);
                    position.Y = terrain.heightMapInfo.GetHeight(new Vector3(position.X, 0, position.Z));
                    orientation = factoryAnchor.orientation;
                    oneFactory.Model = radioactiveFactoryModel;
                    oneFactory.LoadContent(game, position, orientation,ref factoryFont,ref factoryBackground,ref factoryProduceButton, nuclearFactoryAnimationTextures);
                    HydroBot.numResources -= GameConstants.numResourcesForEachFactory;
                    factories.Add(oneFactory);
                    status = true;
                    break;
            }
            if (status)
            {
                // Play sound for successful addition of a building
                audio.OpenChest.Play();
            }

            return status;
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
                    researchAnchor = new ResearchFacility();
                    anchorPosition.Y = terrain.heightMapInfo.GetHeight(new Vector3(anchorPosition.X, 0, anchorPosition.Z));
                    orientation = (float)(Math.PI / 2) * random.Next(4);
                    researchAnchor.Model = researchBuildingModel;
                    researchAnchor.LoadContent(game, anchorPosition, orientation, ref facilityFont, ref facilityFont2, ref dummyTexture, ref dummyTexture, ref dummyTexture, ref dummyTexture);
                }
                else
                {
                    FactoryType typeOfFactory = factoryButtonPanel.anchorIndexToFactoryType(factoryButtonPanel.AnchoredIndex);
                    factoryAnchor = new Factory(typeOfFactory);
                    anchorPosition.Y = terrain.heightMapInfo.GetHeight(new Vector3(anchorPosition.X, 0, anchorPosition.Z));
                    orientation = (float)(Math.PI / 2) * random.Next(4);
                    switch(typeOfFactory) {
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
                    factoryAnchor.LoadContent(game, anchorPosition, orientation, ref factoryFont, ref dummyTexture, ref dummyTexture, null);
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

        public override void Draw(GameTime gameTime)
        {

            if (showFoundKey && firstShow)
            {
                DrawFoundKey();
                return;
            }
            
            switch (currentGameState)
            {
                case GameState.PlayingCutScene:
                    DrawCutScene();
                    break;
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
        private void DrawFoundKey()
        {
            string message = "The fishes have helped you to find the hidden key to treasure chests in return for your help!!";
            message = IngamePresentation.wrapLine(message, 800, keyFoundFont);
            spriteBatch.Begin();
            spriteBatch.Draw(foundKeyScreen, new Rectangle(GraphicDevice.Viewport.TitleSafeArea.Center.X - foundKeyScreen.Width/2, GraphicDevice.Viewport.TitleSafeArea.Center.Y-foundKeyScreen.Height/2, foundKeyScreen.Width, foundKeyScreen.Height), Color.White);
            spriteBatch.DrawString(keyFoundFont, message, new Vector2(GraphicDevice.Viewport.TitleSafeArea.Center.X-400, 20), Color.DarkRed);

            string nextText = "Press Enter to continue";
            Vector2 nextTextPosition = new Vector2(GraphicDevice.Viewport.TitleSafeArea.Right - menuSmall.MeasureString(nextText).X, GraphicDevice.Viewport.TitleSafeArea.Bottom - menuSmall.MeasureString(nextText).Y);
            spriteBatch.DrawString(menuSmall, nextText, nextTextPosition, Color.Blue);

            spriteBatch.End();
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
            // Updating camera's frustum
            frustum = new BoundingFrustum(gameCamera.ViewMatrix * gameCamera.ProjectionMatrix);

            //preparing edge detecting for the object being pointed at
            graphicEffect.PrepareEdgeDetect(hydroBot, cursor, gameCamera, fish, fishAmount, enemies, enemiesAmount, trashes, shipWrecks, factories, researchFacility, null,
                powerpacks, resources, graphics.GraphicsDevice, normalDepthRenderTargetLow, normalDepthRenderTargetHigh);

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
            
            hydroBot.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalShading");

            DrawProjectTiles();

            // draw bubbles
            foreach (Bubble bubble in bubbles)
            {
                bubble.Draw(spriteBatch, 1.0f);
            }

            //draw schools of fish
            spriteBatch.Begin();
            schoolOfFish1.Draw(gameTime, spriteBatch);
            schoolOfFish2.Draw(gameTime, spriteBatch);
            schoolOfFish3.Draw(gameTime, spriteBatch);
            spriteBatch.End();

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
                point.Draw(spriteBatch);
            }
            spriteBatch.Begin();
            DrawStats();
            DrawBulletType();
            DrawHeight();
            DrawRadar();

            // Draw the factory panel
            factoryButtonPanel.Draw(spriteBatch);

            if (HydroBot.activeSkillID != -1) DrawActiveSkill();
            DrawLevelObjectiveIcon();
            if (PoseidonGame.gamePlus)
                DrawGamePlusLevel();
            else
                DrawTipIcon();

            if (openFactoryConfigurationScene)
                factoryToConfigure.DrawFactoryConfigurationScene(spriteBatch, menuSmall);
            if (openResearchFacilityConfigScene)
                researchFacility.DrawResearchFacilityConfigurationScene(spriteBatch, menuSmall);

            // spriteBatch.DrawString(statsFont, "Is bot moving: " + hydroBot.isMoving() + "\n", new Vector2(50, 50), Color.Black);

            cursor.Draw(gameTime);
            spriteBatch.End();

            if (screenTransitNow)
            {
                bool doneTransit = graphicEffect.TransitTwoSceens(Scene2Texture, afterEffectsAppliedRenderTarget, graphics, cutSceneImmediateRenderTarget);
                if (doneTransit) screenTransitNow = false;
            }
            else
            {
                graphics.GraphicsDevice.SetRenderTarget(cutSceneImmediateRenderTarget);
                spriteBatch.Begin();
                spriteBatch.Draw(afterEffectsAppliedRenderTarget, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
                spriteBatch.End();
            }
            graphics.GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin();
            spriteBatch.Draw(cutSceneImmediateRenderTarget, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
            spriteBatch.End();
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

        private void DrawRadar()
        {
            radar.Draw(spriteBatch, hydroBot.Position, enemies, enemiesAmount, fish, fishAmount, shipWrecks, factories, researchFacility);
        }

        public bool CharacterNearShipWreck(BoundingSphere shipSphere)
        {
            if (hydroBot.BoundingSphere.Intersects(shipSphere))
                return true;
            else
                return false;
        }

        private void DrawHeight()
        {
            //float xOffsetText, yOffsetText;
            //string str1 = " Height: " + heightMapInfo.GetHeight(hydroBot.Position);
            //Rectangle rectSafeArea;

            ////Calculate str1 position
            //rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            //xOffsetText = rectSafeArea.X;
            //yOffsetText = rectSafeArea.Y;

            //Vector2 strSize = statsFont.MeasureString(str1);
            //Vector2 strPosition =
            //    new Vector2((int)xOffsetText + 200, (int)yOffsetText);

            ////spriteBatch.Begin();
            //spriteBatch.DrawString(statsFont, str1, strPosition, Color.White);
        }

        private void DrawStats()
        {
            float xOffsetText, yOffsetText;
            int days;
            string str1 = GameConstants.StrTimeRemaining;
            string str2 = "";
            //str2 += hydroBot.Position;
            Rectangle rectSafeArea;
            days = ((roundTimer.Minutes * 60) + roundTimer.Seconds)/GameConstants.DaysPerSecond;
            str1 += days.ToString();

            IngamePresentation.DrawObjectPointedAtStatus(cursor, gameCamera, this.game, spriteBatch, fish, fishAmount, enemies, enemiesAmount, trashes, shipWrecks, factories, researchFacility, null, powerpacks, resources);

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
            //strPosition.Y += strSize.Y;
            //spriteBatch.DrawString(statsFont, str2, strPosition, Color.White);
        }

        // Draw the currently selected bullet type
        private void DrawBulletType()
        {
            int xOffsetText, yOffsetText;
            Rectangle rectSafeArea;

            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            //xOffsetText = rectSafeArea.Left + 325;
            xOffsetText = rectSafeArea.Center.X - 150 -64;
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

        //Draw level objective icon
        private void DrawLevelObjectiveIcon()
        {
            int xOffsetText, yOffsetText;
            Rectangle rectSafeArea;

            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            xOffsetText = rectSafeArea.Right - 100;
            yOffsetText = rectSafeArea.Top;

            levelObjectiveIconRectangle = new Rectangle(xOffsetText, yOffsetText, 96, 96);

            spriteBatch.Draw(levelObjectiveIconTexture, levelObjectiveIconRectangle, Color.White);

        }

        //Draw level tip icon
        private void DrawTipIcon()
        {
            int xOffsetText, yOffsetText;

            xOffsetText = levelObjectiveIconRectangle.Center.X - 25;
            yOffsetText = levelObjectiveIconRectangle.Bottom + 10;

            tipIconRectangle = new Rectangle(xOffsetText, yOffsetText, 50, 50);

            spriteBatch.Draw(tipIconTexture, tipIconRectangle, Color.White);

        }

        //Draw GamePlus level
        private void DrawGamePlusLevel()
        {
            int xOffsetText, yOffsetText;
            string text = "GAMEPLUS(" + HydroBot.gamePlusLevel+")";

            xOffsetText = levelObjectiveIconRectangle.Right - (int)fishTalkFont.MeasureString(text).X;
            yOffsetText = levelObjectiveIconRectangle.Bottom + 5;

            spriteBatch.DrawString(fishTalkFont, text, new Vector2(xOffsetText, yOffsetText), Color.Red);
        }

        public bool mouseOnLevelObjectiveIcon(MouseState lmouseState)
        {
            if(levelObjectiveIconRectangle.Intersects(new Rectangle(lmouseState.X, lmouseState.Y, 10, 10)))
                return true;
            else
                return false;
        }

        public bool mouseOnTipIcon(MouseState lmouseState)
        {
            if ( tipIconRectangle.Intersects(new Rectangle(lmouseState.X, lmouseState.Y, 10, 10)))
                return true;
            else
                return false;
        }

        private void DrawCutScene()
        {
            graphics.GraphicsDevice.SetRenderTarget(renderTarget2);
            graphics.GraphicsDevice.Clear(Color.Black);
            //draw the background 1st
            spriteBatch.Begin();
            spriteBatch.Draw(Content.Load<Texture2D>(cutSceneDialog.cutScenes[currentLevel][currentSentence].backgroundName),
                new Rectangle(0, 0, GraphicDevice.Viewport.TitleSafeArea.Width, GraphicDevice.Viewport.TitleSafeArea.Height), Color.White);

            //draw face of the last speaker
            if (currentSentence > 0 && cutSceneDialog.cutScenes[currentLevel][currentSentence - 1].speakerID != 3 && cutSceneDialog.cutScenes[currentLevel][currentSentence].speakerID != 3)
            {

                if (cutSceneDialog.cutScenes[currentLevel][currentSentence - 1].speakerID == 0)
                    spriteBatch.Draw(botFace,
                        new Rectangle(0, 0, GraphicDevice.Viewport.TitleSafeArea.Width, GraphicDevice.Viewport.TitleSafeArea.Height), Color.White);
                else
                    spriteBatch.Draw(otherPersonFace,
                        new Rectangle(0, 0, GraphicDevice.Viewport.TitleSafeArea.Width, GraphicDevice.Viewport.TitleSafeArea.Height), Color.White);
            }

            //draw face of the current speaker
            //bot speaking
            if (cutSceneDialog.cutScenes[currentLevel][currentSentence].speakerID == 0)
            {
                // load texture for each type of emotion
                if (cutSceneDialog.cutScenes[currentLevel][currentSentence].emotionType == 0)
                {
                    botFace = Content.Load<Texture2D>("Image/Cutscenes/botNormal");
                }
                // other ifs here

                //draw face
                //spriteBatch.Draw(botFace,
                //    new Rectangle(GraphicDevice.Viewport.TitleSafeArea.Width - botFace.Width,
                //        GraphicDevice.Viewport.TitleSafeArea.Height - botFace.Height,
                //        botFace.Width, botFace.Height), Color.White);
                spriteBatch.Draw(botFace,
                    new Rectangle(0, 0, GraphicDevice.Viewport.TitleSafeArea.Width, GraphicDevice.Viewport.TitleSafeArea.Height), Color.White);
                //draw talking box
                talkingBox = Content.Load<Texture2D>("Image/Cutscenes/botBox");
                Rectangle botRectangle = new Rectangle(0, GraphicDevice.Viewport.TitleSafeArea.Height - talkingBox.Height, GraphicDevice.Viewport.TitleSafeArea.Width, talkingBox.Height);
                spriteBatch.Draw(talkingBox, botRectangle, Color.White);
                //draw what is said
                string text = IngamePresentation.wrapLine(cutSceneDialog.cutScenes[currentLevel][currentSentence].sentence, GraphicDevice.Viewport.TitleSafeArea.Width - 100, menuSmall);
                spriteBatch.DrawString(menuSmall, text, new Vector2(botRectangle.Left + 50, botRectangle.Top + 60), Color.Blue);
            }
            //Poseidon speaking
            if (cutSceneDialog.cutScenes[currentLevel][currentSentence].speakerID == 1)
            {
                // load texture for each type of emotion
                if (cutSceneDialog.cutScenes[currentLevel][currentSentence].emotionType == 0)
                {
                    otherPersonFace = Content.Load<Texture2D>("Image/Cutscenes/poseidonNormal");
                }
                // other ifs here

                //draw face
                spriteBatch.Draw(otherPersonFace,
                    new Rectangle(0, 0, GraphicDevice.Viewport.TitleSafeArea.Width, GraphicDevice.Viewport.TitleSafeArea.Height), Color.White);
                //draw talking box
                talkingBox = Content.Load<Texture2D>("Image/Cutscenes/otherPersonBox");
                Rectangle PoseidonRectangle = new Rectangle(0, GraphicDevice.Viewport.TitleSafeArea.Height - talkingBox.Height, GraphicDevice.Viewport.TitleSafeArea.Width, talkingBox.Height);
                spriteBatch.Draw(talkingBox, PoseidonRectangle, Color.White);
                //draw what is said
                string text = IngamePresentation.wrapLine(cutSceneDialog.cutScenes[currentLevel][currentSentence].sentence, GraphicDevice.Viewport.TitleSafeArea.Width - 100, menuSmall);
                spriteBatch.DrawString(menuSmall, text, new Vector2(PoseidonRectangle.Left + 50, PoseidonRectangle.Top + 65), Color.Blue);
            }
            //Terminator speaking
            if (cutSceneDialog.cutScenes[currentLevel][currentSentence].speakerID == 2)
            {
                // load texture for each type of emotion
                if (cutSceneDialog.cutScenes[currentLevel][currentSentence].emotionType == 0)
                {
                    otherPersonFace = Content.Load<Texture2D>("Image/Cutscenes/terminatorFace");
                }
                // other ifs here

                //draw face
                spriteBatch.Draw(otherPersonFace,
                    new Rectangle(0, 0, GraphicDevice.Viewport.TitleSafeArea.Width, GraphicDevice.Viewport.TitleSafeArea.Height), Color.White);
                //draw talking box
                talkingBox = Content.Load<Texture2D>("Image/Cutscenes/otherPersonBox");
                Rectangle terminatorRectangle = new Rectangle(0, GraphicDevice.Viewport.TitleSafeArea.Height - talkingBox.Height, GraphicDevice.Viewport.TitleSafeArea.Width, talkingBox.Height);
                spriteBatch.Draw(talkingBox, terminatorRectangle, Color.White);
                //draw what is said
                string text = IngamePresentation.wrapLine(cutSceneDialog.cutScenes[currentLevel][currentSentence].sentence, GraphicDevice.Viewport.TitleSafeArea.Width - 100, menuSmall);
                spriteBatch.DrawString(menuSmall, text, new Vector2(terminatorRectangle.Left + 50, terminatorRectangle.Top + 60), Color.Blue);
            }
            //Narrator speaking
            if (cutSceneDialog.cutScenes[currentLevel][currentSentence].speakerID == 3)
            {
                talkingBox = Content.Load<Texture2D>("Image/Cutscenes/narratorBox");
                Rectangle narratorRectangle = new Rectangle(0, GraphicDevice.Viewport.TitleSafeArea.Height - talkingBox.Height, GraphicDevice.Viewport.TitleSafeArea.Width, talkingBox.Height);
                spriteBatch.Draw(talkingBox, narratorRectangle, Color.White);
                //draw what is said
                string text = IngamePresentation.wrapLine(cutSceneDialog.cutScenes[currentLevel][currentSentence].sentence, GraphicDevice.Viewport.TitleSafeArea.Width - 100, menuSmall);
                spriteBatch.DrawString(menuSmall, text, new Vector2(narratorRectangle.Left + 50, narratorRectangle.Top + 30), Color.Black);
            }
            spriteBatch.End();
            if (screenTransitNow)
            {
                bool doneTransit = graphicEffect.TransitTwoSceens(Scene2Texture, renderTarget2, graphics, cutSceneImmediateRenderTarget);
                if (doneTransit) screenTransitNow = false;
            }
            else
            {
                graphics.GraphicsDevice.SetRenderTarget(cutSceneImmediateRenderTarget);
                spriteBatch.Begin();
                spriteBatch.Draw(renderTarget2, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
                spriteBatch.End();
            }
            graphics.GraphicsDevice.SetRenderTarget(cutSceneFinalRenderTarget);
            graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            {
                spriteBatch.Draw(cutSceneImmediateRenderTarget, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);

            }
            spriteBatch.End();
            Scene2Texture = cutSceneFinalRenderTarget;
            graphics.GraphicsDevice.SetRenderTarget(null);
            graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            {
                spriteBatch.Draw(cutSceneFinalRenderTarget, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);

            }
            spriteBatch.End();
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
            BoundingSphere shipSphere;
            // Drawing ship wrecks
            foreach (ShipWreck shipWreck in shipWrecks)
            {
                shipSphere = shipWreck.BoundingSphere;
                shipSphere.Center = shipWreck.Position;
                if (shipSphere.Intersects(frustum))
                {
                    shipWreck.Draw(gameCamera.ViewMatrix,
                        gameCamera.ProjectionMatrix, gameCamera, "NormalShading");
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

            // Drawing trash
            BoundingSphere trashRealSphere;
            foreach (Trash trash in trashes)
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
            //Draw each static object
            foreach (StaticObject staticObject in staticObjects)
            {
                if (staticObject.BoundingSphere.Intersects(frustum))
                {
                    staticObject.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
                    //RasterizerState rs = new RasterizerState();
                    //rs.FillMode = FillMode.WireFrame;
                    //GraphicDevice.RasterizerState = rs;
                    //staticObject.DrawBoundingSphere(gameCamera.ViewMatrix,
                    //    gameCamera.ProjectionMatrix, boundingSphere);

                    //rs = new RasterizerState();
                    //rs.FillMode = FillMode.Solid;
                    //GraphicDevice.RasterizerState = rs;
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
        }
    }
}
