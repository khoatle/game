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
        SpriteFont statsFont, statisticFont;
        SpriteFont fishTalkFont;
        SpriteFont keyFoundFont;
        static SpriteFont menuSmall;

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
        public static CutSceneDialog cutSceneDialog;
        // Which sentence in the dialog is being printed
        int currentSentence = 0;

        // For mouse inputs
        bool doubleClicked = false;
        bool clicked = false;
        bool notYetReleased = false;
        double clickTimer = 0;

        public bool openFactoryConfigurationScene = false;
        public bool openResearchFacilityConfigScene = false;
        private Factory factoryToConfigure;



        // Textures for animating the processing state of factories.
        // Plastic factory will use nuclear factory textures
        private List<Texture2D> biofactoryAnimationTextures;
        private List<Texture2D> nuclearFactoryAnimationTextures;

        // Texture/Font for Mouse Interaction panel for factories
        Texture2D factoryPanelTexture;
        SpriteFont factoryPanelFont;
        public ButtonPanel factoryButtonPanel;

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
        private Model goldenKey;
        private Model resourceModel;

        // For applying graphic effects
        public GraphicEffect graphicEffect;
        //for particle systems
        public static ParticleManagement particleManager;

        //for edge detection effect
        RenderTarget2D normalDepthRenderTargetLow, normalDepthRenderTargetHigh, edgeDetectionRenderTarget;

        //for level statistics
        public static int numNormalKills = 0;
        public static int numBossKills = 0;
        public static int healthLost = 0;
        public static int numTrashCollected = 0;
        public bool displayStatisticsNow = false;

        //textures for level statistics display
        private Texture2D statisticLogoTexture;
        private Texture2D[] rankTextures;

        public static int levelObjectiveState = 0;

        public static double timeElapsedFromLevelStart = 0;

        public PlayGameScene(Game game, GraphicsDeviceManager graphic, ContentManager content, GraphicsDevice GraphicsDevice, SpriteBatch sBatch, Vector2 pausePosition, Rectangle pauseRect, Texture2D actionTexture, CutSceneDialog cutSceneDialogue, Radar radar)
            : base(game)
        {
            graphics = graphic;
            Content = content;
            GraphicDevice = GraphicsDevice;
            spriteBatch = sBatch;
            this.pausePosition = pausePosition;
            this.pauseRect = pauseRect;
            this.actionTexture = actionTexture;
            this.game = game;
            cutSceneDialog = cutSceneDialogue;
            this.radar = radar;
            this.stunnedIconTexture = IngamePresentation.stunnedTexture;
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

            //if (PoseidonGame.gamePlus)
            //{
            //    int[] numShootingEnemies = { 0, 5, 10 + (HydroBot.gamePlusLevel * 5), 15, 15, 30, 30, 30, 30, 75, 15, 15 };
            //    GameConstants.NumberShootingEnemies = numShootingEnemies;
            //    int[] numCombatEnemies =   { 0, 5, 10 + (HydroBot.gamePlusLevel * 5), 15, 15, 30, 30, 30, 30, 75, 15, 15 };
            //    GameConstants.NumberCombatEnemies = numCombatEnemies;
            //    int[] numGhostPirates = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            //    GameConstants.NumberGhostPirate = numGhostPirates;
            //    int[] numFish = { 50, 50, 50, 0, 50, 50, 25, 50, 0, 0, 0, 0 };
            //    GameConstants.NumberFish = numFish;
            //    int[] numMutantShark = { 0, 0, 0, 1, 1, 2 + HydroBot.gamePlusLevel, 3 + HydroBot.gamePlusLevel, 4 + HydroBot.gamePlusLevel, 5 + HydroBot.gamePlusLevel, 10 + HydroBot.gamePlusLevel, 0, HydroBot.gamePlusLevel };
            //    GameConstants.NumberMutantShark = numMutantShark;
            //    int[] numTerminator = { 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 1 };
            //    GameConstants.NumberTerminator = numTerminator;
            //    int[] numSubmarine = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            //    GameConstants.NumberSubmarine = numSubmarine;
            //} 
            //else {
                int[] numShootingEnemies = { 0, 5, 10, 0, 10, 15, 20, 20, 20, 30, 10, 10 };
                GameConstants.NumberShootingEnemies = numShootingEnemies;
                int[] numCombatEnemies = { 0, 5, 10, 0, 10, 15, 20, 20, 20, 30, 10, 10 };
                GameConstants.NumberCombatEnemies = numCombatEnemies;
                int[] numGhostPirates = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 20, 0, 0 };
                GameConstants.NumberGhostPirate = numGhostPirates;
                int[] numFish = { 50, 50, 50, 0, 50, 50, 25, 50, 0, 0, 0, 0 };
                GameConstants.NumberFish = numFish;
                int[] numMutantShark = { 0, 0, 0, 1, 1, 2, 1, 2, 2, 6, 0, 0 };
                GameConstants.NumberMutantShark = numMutantShark;
                int[] numTerminator = { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 1 };
                GameConstants.NumberTerminator = numTerminator;
                int[] numSubmarine = { 0, 0, 0, 0, 0, 0, 1, 1, 2, 3, 0, 0 };
                GameConstants.NumberSubmarine = numSubmarine;
            //}

            skillTextures = new Texture2D[GameConstants.numberOfSkills];
            bulletTypeTextures = new Texture2D[GameConstants.numBulletTypes];

            // for the mouse or touch
            cursor = new Cursor(game, spriteBatch);
            //Components.Add(cursor);

            bubbles = new List<Bubble>();
            points = new List<Point>();

            schoolOfFish1 = new SchoolOfFish(Content,IngamePresentation.fishTexture1, 100, GameConstants.MainGameMaxRangeX - 250,
                100, GameConstants.MainGameMaxRangeZ - 250, GameConstants.MainCamHeight);
            schoolOfFish2 = new SchoolOfFish(Content, IngamePresentation.fishTexture2, -GameConstants.MainGameMaxRangeX + 250, -100,
                -GameConstants.MainGameMaxRangeZ + 250, -100, GameConstants.MainCamHeight);
            schoolOfFish3 = new SchoolOfFish(Content, IngamePresentation.fishTexture3, -GameConstants.MainGameMaxRangeX + 250, -100,
                100, GameConstants.MainGameMaxRangeZ - 250, GameConstants.MainCamHeight);

            scaredIconTexture = IngamePresentation.scaredIconTexture;

            winningTexture = IngamePresentation.winningTexture;
            losingTexture = IngamePresentation.losingTexture;

            //load texture for statistics screen
            statisticLogoTexture = Content.Load<Texture2D>("Image/LevelStatistics/levelstatistics");
            rankTextures = new Texture2D[5];
            rankTextures[0] = Content.Load<Texture2D>("Image/LevelStatistics/beginnerRank");
            rankTextures[1] = Content.Load<Texture2D>("Image/LevelStatistics/apprenticeRank");
            rankTextures[2] = Content.Load<Texture2D>("Image/LevelStatistics/adeptRank");
            rankTextures[3] = Content.Load<Texture2D>("Image/LevelStatistics/expertRank");
            rankTextures[4] = Content.Load<Texture2D>("Image/LevelStatistics/exceptionalRank");

            // Instantiate the factory Button
            float buttonScale = 1.0f * IngamePresentation.textScaleFactor;
            //if (game.Window.ClientBounds.Width <= 900) {
            //    buttonScale = 0.8f; // scale the factory panel icons a bit smaller in small window mode
            //}
            factoryButtonPanel = new ButtonPanel(4, buttonScale);

            this.Load();
            //System.Diagnostics.Debug.WriteLine("In playgamescene init CurrentExp:" + HydroBot.currentExperiencePts);
            //System.Diagnostics.Debug.WriteLine("In playgamescene init NextExp:" + HydroBot.nextLevelExperience);
        }


        public void Load()
        {
            statsFont = IngamePresentation.statsFont;
            statisticFont = IngamePresentation.statisticFont;
            menuSmall = IngamePresentation.menuSmall;
            fishTalkFont = IngamePresentation.fishTalkFont;
            keyFoundFont = statisticFont;// Content.Load<SpriteFont>("Fonts/painting");
            // Get the audio library
            audio = PoseidonGame.audio;

            //Uncomment below line to use LEVELS
            //string terrain_name = "Image/terrain" + currentLevel;

            terrain = new Terrain(Content);

            skillTextures = IngamePresentation.skillTextures;
            bulletTypeTextures = IngamePresentation.bulletTypeTextures;

            foundKeyScreen = IngamePresentation.goldenKeyTexture;

            powerpacks = new List<Powerpack>();
            resources = new List<Resource>();

            hydroBot.Load(Content);

            //prevTank.Load(Content);
            roundTimer = roundTime;

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

            graphicEffect = new GraphicEffect(this, spriteBatch, fishTalkFont);
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
            goldenKey = Content.Load<Model>("Models/Miscellaneous/Goldenkey");

            boundingSphere.Model = Content.Load<Model>("Models/Miscellaneous/sphere1uR");

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
            IngamePresentation.ResetObjPointedAtMsgs();
            //Factory.buildingSoundInstance.Resume();
            //ResearchFacility.buildingSoundInstance.Resume();
            base.Show();
        }

        private void ResetGame(GameTime gameTime, float aspectRatio)
        {
            currentSentence = 0;
            currentGameState = GameState.PlayingCutScene;
            gameCamera.shaking = false;

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
                if (PoseidonGame.gamePlus)
                    serializer.SerializeObjects("SurvivalMode", objectsToSerialize);

                hydroBot.SetLevelStartValues();
            }

            hydroBot.Reset();
            if (currentLevel == 11) HydroBot.bulletType = 0;

            cursor.targetToLock = null;
            MediaPlayer.Stop();
            roundTime = GameConstants.RoundTime[currentLevel];
            roundTimer = roundTime;
            isBossKilled = false;
            

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
            
            schoolOfFish1 = new SchoolOfFish(Content, IngamePresentation.fishTexture1, 100, GameConstants.MainGameMaxRangeX - 250,
                100, GameConstants.MainGameMaxRangeZ - 250, GameConstants.MainCamHeight);
            schoolOfFish2 = new SchoolOfFish(Content, IngamePresentation.fishTexture2, -GameConstants.MainGameMaxRangeX + 250, -100,
                -GameConstants.MainGameMaxRangeZ + 250, -100, GameConstants.MainCamHeight);
            schoolOfFish3 = new SchoolOfFish(Content, IngamePresentation.fishTexture3, -GameConstants.MainGameMaxRangeX + 250, -100,
                100, GameConstants.MainGameMaxRangeZ - 250, GameConstants.MainCamHeight);

            //reset the shipwreck content too
            //ShipWreckScene.resetShipWreckNow = true;

            InitializeGameField(Content);

            ////level statistics reset
            //ResetStatisticCounters();

            ////reset particles
            //particleManager.ResetParticles();
        }

        private void InitializeGameField(ContentManager Content)
        {
            timeElapsedFromLevelStart = 0;
            levelObjectiveState = 0;
            newLevelObjAvailable = true;
            HydroBot.numResources += GameConstants.numResourcesAtStart;
            //User must find the key at every level
            firstShow = true;
            showFoundKey = false;
            hadkey = false;
            ResetStatisticCounters();
            //reset particles
            particleManager.ResetParticles();
            //reset the shipwreck content too
            ShipWreckScene.resetShipWreckNow = true;
            isBossKilled = false;

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
                0, GameConstants.MainGameMaxRangeX, 0, GameConstants.MainGameMaxRangeZ);
            
            //Initialize trash
            //int random_model;
            int numberTrash = GameConstants.NumberBioTrash[currentLevel] + GameConstants.NumberNuclearTrash[currentLevel] + GameConstants.NumberPlasticTrash[currentLevel];
            trashes = new List<Trash>(numberTrash);
            int bioIndex, plasticIndex, nuclearIndex;
            for (bioIndex = 0; bioIndex < GameConstants.NumberBioTrash[currentLevel]; bioIndex++)
            {
                orientation = random.Next(100);
                trashes.Add(new Trash(TrashType.biodegradable, particleManager));
                trashes[bioIndex].Load(Content,ref biodegradableTrash, orientation);
            }
            for (plasticIndex = bioIndex; plasticIndex < bioIndex+GameConstants.NumberPlasticTrash[currentLevel]; plasticIndex++)
            {
                orientation = random.Next(100);
                trashes.Add(new Trash(TrashType.plastic, particleManager));
                trashes[plasticIndex].Load(Content,ref plasticTrash, orientation); //nuclear model
            }
            for (nuclearIndex = plasticIndex; nuclearIndex< plasticIndex + GameConstants.NumberNuclearTrash[currentLevel]; nuclearIndex++)
            {
                orientation = random.Next(100);
                trashes.Add(new Trash(TrashType.radioactive, particleManager));
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
            staticObjects = new List<StaticObject>(GameConstants.NumStaticObjectsMain[currentLevel]);
            int randomObject;
            for (int index = 0; index < GameConstants.NumStaticObjectsMain[currentLevel]; index++)
            {
                staticObjects.Add(new StaticObject());
                if (currentLevel == 6)
                {
                    randomObject = random.Next(4);
                    //randomObject = 3;
                    switch (randomObject)
                    {
                        case 0:
                            staticObjects[index].LoadContent(Content, "Models/DecorationObjects/animalBone1", false, 1, 24, 24);
                            break;
                        case 1:
                            staticObjects[index].LoadContent(Content, "Models/DecorationObjects/animalBone2", false, 0, 0, 0);
                            break;
                        case 2:
                            staticObjects[index].LoadContent(Content, "Models/DecorationObjects/animalBone3", false, 0, 0, 0);
                            break;
                        case 3:
                            staticObjects[index].LoadContent(Content, "Models/DecorationObjects/animalBone4", false, 0, 0, 0);
                            break;
                    }
                }
                if (currentLevel == 7)
                {
                    staticObjects[index].LoadContent(Content, "Models/DecorationObjects/kelpPlant", true, 1, 48, 24);
                }
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
            if (currentGameState == GameState.NewGameStarted)
            {
                PoseidonGame.videoPlayer.Play(PoseidonGame.cinematic);
                currentGameState = GameState.PlayingOpeningCinematic;
            }
            //if (currentGameState == GameState.PlayingOpeningCinematic)
            //{
            //    lastKeyboardState = currentKeyboardState;
            //    currentKeyboardState = Keyboard.GetState();
            //}
            //if ((Keyboard.GetState()).IsKeyDown(Keys.Insert) && type < 3)
            //{
            //    HydroBot.turtlePower = HydroBot.seaCowPower = HydroBot.dolphinPower = 1.0f;
            //    AddingObjects.placeMinion(Content, 2 - type, enemies, enemiesAmount, fish, ref fishAmount, hydroBot);
            //    type++;
            //}

            // play the boss fight music for certain levels
            if (currentGameState == GameState.Won)
                MediaPlayer.Stop();
            else if (currentLevel == 3 || currentLevel == 11)
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
                if (currentGameState == GameState.Running || currentGameState == GameState.WonButStaying)
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
                    if (currentLevel == 2 || currentLevel == 5 || currentLevel == 6 || currentLevel == 7 || currentLevel == 8)
                    {
                        if ((double)HydroBot.currentEnvPoint / (double)HydroBot.maxEnvPoint > GameConstants.EnvThresholdForKey)
                        {
                            showFoundKey = true;
                            //hadkey = true;
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
                            //generate the key
                            Vector3 powerpackPosition = Vector3.Zero;
                            PowerPackType powerType = PowerPackType.GoldenKey; //type 5 for strange rock
                            Powerpack powerpack = new Powerpack(powerType);
                            powerpackPosition.Y = GameConstants.MainGameFloatHeight + 10;
                            powerpackPosition.X = random.Next(0, (int)(2 * GameConstants.MainGameMaxRangeX * 0.8f)) - GameConstants.MainGameMaxRangeX * 0.8f;
                            powerpackPosition.Z = random.Next(0, (int)(2 * GameConstants.MainGameMaxRangeZ * 0.8f)) - GameConstants.MainGameMaxRangeZ * 0.8f;
                            //powerpackPosition = hydroBot.Position;
                            powerpack.Model = goldenKey;
                            powerpack.LoadContent(powerpackPosition);
                            powerpacks.Add(powerpack);
                        }
                        return;
                    }

                    IngamePresentation.tipHover = IngamePresentation.mouseOnTipIcon(currentMouseState);
                    IngamePresentation.levelObjHover = IngamePresentation.mouseOnLevelObjectiveIcon(currentMouseState);
                    bool mouseOnInteractiveIcons = IngamePresentation.levelObjHover || IngamePresentation.tipHover || IngamePresentation.toNextLevelHover || (!factoryButtonPanel.cursorOutsidePanelArea) || factoryButtonPanel.hasAnyAnchor() 
                        || factoryButtonPanel.clickToBuildDetected || factoryButtonPanel.clickToRemoveAnchorActive || factoryButtonPanel.rightClickToRemoveAnchor;
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

                    if (trashes!=null)// && trashes.Count < GameConstants.NumberTrash[currentLevel])
                    {
                        Vector3 pos = AddingObjects.createSinkingTrash(ref trashes, Content, random, shipWrecks, staticObjects, factories, researchFacility,
                                GameConstants.TrashMinRangeX, GameConstants.MainGameMaxRangeX - 100, GameConstants.TrashMinRangeZ,
                                GameConstants.MainGameMaxRangeZ - 60, GameConstants.MainGameFloatHeight, terrain.heightMapInfo, ref biodegradableTrash, ref plasticTrash, ref radioactiveTrash, particleManager);
                        //Point point = new Point();
                        //point.LoadContent(PoseidonGame.contentManager, "New Trash Dropped", pos, Color.LawnGreen);
                        //points.Add(point);
                    }
                    foreach (Trash trash in trashes)
                    {
                        trash.Update(gameTime);
                    }

                    foreach (Powerpack powPack in powerpacks)
                        powPack.Update();
                    foreach (Resource res in resources)
                        res.Update();
                   
                    CursorManager.CheckClick(ref this.lastMouseState, ref this.currentMouseState, gameTime, ref clickTimer, ref clicked, ref doubleClicked, ref notYetReleased);
                    foreach (Factory factory in factories)
                    {
                        factory.Update(gameTime,ref powerpacks, ref resources, ref powerpackModels, ref resourceModel, ref strangeRockModels);

                        if(doubleClicked && !factory.UnderConstruction && hydroBot.BoundingSphere.Intersects(factory.BoundingSphere) && CursorManager.MouseOnObject(cursor, factory.BoundingSphere, factory.Position, gameCamera))
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
                        if (doubleClicked && !researchFacility.UnderConstruction && hydroBot.BoundingSphere.Intersects(researchFacility.BoundingSphere) && CursorManager.MouseOnObject(cursor, researchFacility.BoundingSphere, researchFacility.Position, gameCamera) )
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
                    foreach (StaticObject staticObj in staticObjects)
                    {
                        staticObj.Update(frustum, gameTime);
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
                        enemies, enemiesAmount, fish, fishAmount, gameCamera, particleManager.explosionParticles);
                    Collision.updateHealingBulletVsBarrierCollision(healthBullet, fish, fishAmount, frustum, GameMode.MainGame);
                    Collision.updateDamageBulletVsBarriersCollision(enemyBullet, fish, ref fishAmount, frustum, GameMode.MainGame, gameTime, hydroBot,
                        enemies, enemiesAmount, fish, fishAmount, gameCamera, particleManager.explosionParticles);
                    Collision.updateProjectileHitBot(hydroBot, enemyBullet, GameMode.MainGame, enemies, enemiesAmount, particleManager.explosionParticles, gameCamera, fish, fishAmount);
                    Collision.updateDamageBulletVsBarriersCollision(alliesBullets, enemies, ref enemiesAmount, frustum, GameMode.MainGame, gameTime, hydroBot,
                        enemies, enemiesAmount, fish, fishAmount, gameCamera, particleManager.explosionParticles);

                    Collision.deleteSmallerThanZero(enemies, ref enemiesAmount, frustum, GameMode.MainGame, cursor, particleManager.explosionLargeParticles);
                    Collision.deleteSmallerThanZero(fish, ref fishAmount, frustum, GameMode.MainGame, cursor, particleManager.explosionLargeParticles);

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
                        fish[i].Update(gameTime, frustum, enemies, enemiesAmount, fish, fishAmount, random.Next(100), hydroBot, enemyBullet);
                    }

                    roundTimer -= gameTime.ElapsedGameTime;
                    PoseidonGame.playTime += gameTime.ElapsedGameTime;
                    timeElapsedFromLevelStart += gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (!(currentGameState == GameState.WonButStaying))
                    {
                        if (CheckWinCondition())
                        {
                            if (!GameConstants.haveToStayTillEnd[currentLevel])
                                currentGameState = GameState.WonButStaying;
                            else currentGameState = GameState.Won;
                            audio.gameWon.Play();
                            newLevelObjAvailable = false;
                        }
                        if (CheckLoseCondition())
                        {
                            currentGameState = GameState.Lost;
                            audio.gameOver.Play();
                        }
                    }
                    else
                    {
                        //time = 0, move to next level now
                        if (HydroBot.currentHitPoint <= 0) currentGameState = GameState.Lost;
                        if (roundTimer <= TimeSpan.Zero) currentGameState = GameState.Won;
                        IngamePresentation.toNextLevelHover = IngamePresentation.mouseOnNextLevelIcon(lastMouseState);
                        if (IngamePresentation.toNextLevelHover && this.lastMouseState.LeftButton == ButtonState.Pressed && this.currentMouseState.LeftButton == ButtonState.Released)
                        {
                            currentGameState = GameState.Won;
                            IngamePresentation.toNextLevelHover = false;
                        }
                    }


                    //Checking win/lost condition for this level
                    if (HydroBot.currentHitPoint <= 0)
                    {
                        currentGameState = GameState.Lost;
                        audio.gameOver.Play();
                    }
                    //cursor update
                    cursor.Update(GraphicDevice, gameCamera, gameTime, frustum);

                    //update the school of fish
                    schoolOfFish1.Update(gameTime, frustum, hydroBot, enemies, enemiesAmount, fish, fishAmount);
                    schoolOfFish2.Update(gameTime, frustum, hydroBot, enemies, enemiesAmount, fish, fishAmount);
                    schoolOfFish3.Update(gameTime, frustum, hydroBot, enemies, enemiesAmount, fish, fishAmount);

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
                        if (!displayStatisticsNow) displayStatisticsNow = true;
                        else
                        {
                            //return all strange rocks that are not yet processed to bot
                            if (researchFacility != null)
                                HydroBot.numStrangeObjCollected += researchFacility.listTimeRockProcessing.Count;
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

            int radius = 60; // need to revise this based on the model for each building
            BoundingSphere buildingBoundingSphere;
            //BoundingBox buildingBoundingBox = new BoundingBox();
            if (factoryAnchor != null)
            {
                buildingBoundingSphere = factoryAnchor.BoundingSphere;
                radius = (int)buildingBoundingSphere.Radius;
                //factoryAnchor.CalculateBoundingBox(1.0f, factoryAnchor.orientation);
                //buildingBoundingBox = factoryAnchor.boundingBox;
            }
            else if (researchAnchor != null)
            {
                buildingBoundingSphere = researchAnchor.BoundingSphere;
                radius = (int)buildingBoundingSphere.Radius;
                //researchAnchor.CalculateBoundingBox(1.0f, researchAnchor.orientation);
                //buildingBoundingBox = researchAnchor.boundingBox;
            }

            //AddingObjects.ModifyBoundingBox(ref buildingBoundingBox, GameConstants.MainGameFloatHeight);

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
                PlayGameScene.points.Add(point);
                return false;
            }

            //Verify that current location is available for adding the building

            int heightValue = GameConstants.MainGameFloatHeight;//(int)terrain.heightMapInfo.GetHeight(new Vector3(position.X, 0, position.Z));
            if (AddingObjects.IsSeaBedPlaceOccupied((int)position.X, heightValue, (int)position.Z, radius, shipWrecks, staticObjects, trashes, factories, researchFacility))
            //if (AddingObjects.IsSeaBedPlaceOccupied(buildingBoundingBox, shipWrecks, staticObjects, trashes, factories, researchFacility))
            {
                // Play some sound hinting seabed place is occupied
                audio.MenuScroll.Play();
                Point point = new Point();
                String point_string = "Can not\nbuild here";
                point.LoadContent(PoseidonGame.contentManager, point_string, position, Color.Red);
                PlayGameScene.points.Add(point);
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
                        PlayGameScene.points.Add(point);
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
                        //researchFacility.CalculateBoundingBox(1.0f, orientation);
                        //AddingObjects.ModifyBoundingBox(ref researchFacility.boundingBox, GameConstants.MainGameFloatHeight);
                        HydroBot.numResources -= GameConstants.numResourcesForResearchCenter;
                        status = true;
                        //env loss for building factory
                        if (GameConstants.envLossPerFactoryBuilt > 0)
                        {
                            HydroBot.currentEnvPoint -= GameConstants.envLossPerFactoryBuilt;
                            Point lossPoint = new Point();
                            lossPoint.LoadContent(Content, "-" + GameConstants.envLossPerFactoryBuilt + "ENV", position, Color.Red);
                            points.Add(lossPoint);
                        }
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
                    //oneFactory.CalculateBoundingBox(1.0f, orientation);
                    //AddingObjects.ModifyBoundingBox(ref oneFactory.boundingBox, GameConstants.MainGameFloatHeight);
                    HydroBot.numResources -= GameConstants.numResourcesForBioFactory;
                    factories.Add(oneFactory);
                    status = true;

                    //env loss for building factory
                    if (GameConstants.envLossPerFactoryBuilt > 0)
                    {
                        HydroBot.currentEnvPoint -= GameConstants.envLossPerFactoryBuilt;
                        Point lossPoint = new Point();
                        lossPoint.LoadContent(Content, "-" + GameConstants.envLossPerFactoryBuilt + "ENV", position, Color.Red);
                        points.Add(lossPoint);
                    }
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
                    //oneFactory.CalculateBoundingBox(1.0f, orientation);
                    //AddingObjects.ModifyBoundingBox(ref oneFactory.boundingBox, GameConstants.MainGameFloatHeight);
                    factories.Add(oneFactory);
                    status = true;
                    //env loss for building factory
                    if (GameConstants.envLossPerFactoryBuilt > 0)
                    {
                        HydroBot.currentEnvPoint -= GameConstants.envLossPerFactoryBuilt;
                        Point lossPoint = new Point();
                        lossPoint.LoadContent(Content, "-" + GameConstants.envLossPerFactoryBuilt + "ENV", position, Color.Red);
                        points.Add(lossPoint);
                    }
                    break;
                case BuildingType.radioactive:
                    oneFactory = new Factory(FactoryType.radioactive, particleManager, GraphicDevice);
                    position.Y = terrain.heightMapInfo.GetHeight(new Vector3(position.X, 0, position.Z));
                    orientation = factoryAnchor.orientation;
                    oneFactory.Model = radioactiveFactoryModel;
                    oneFactory.ModelStates = radioactiveFactoryModelStates;
                    oneFactory.LoadContent(game, position, orientation, ref IngamePresentation.factoryFont, ref IngamePresentation.factoryBackground, nuclearFactoryAnimationTextures);
                    HydroBot.numResources -= GameConstants.numResourcesForRadioFactory;
                    //oneFactory.CalculateBoundingBox(1.0f, orientation);
                    //AddingObjects.ModifyBoundingBox(ref oneFactory.boundingBox, GameConstants.MainGameFloatHeight);
                    factories.Add(oneFactory);
                    status = true;
                    //env loss for building factory
                    if (GameConstants.envLossPerFactoryBuilt > 0)
                    {
                        HydroBot.currentEnvPoint -= GameConstants.envLossPerFactoryBuilt;
                        Point lossPoint = new Point();
                        lossPoint.LoadContent(Content, "-" + GameConstants.envLossPerFactoryBuilt + "ENV", position, Color.Red);
                        points.Add(lossPoint);
                    }
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

        public override void Draw(GameTime gameTime)
        {
            
            switch (currentGameState)
            {
                case GameState.PlayingOpeningCinematic:
                    DrawOpeningCinematic();
                    break;
                case GameState.PlayingCutScene:
                    DrawCutScene();
                    break;
                case GameState.Running:
                    RestoreGraphicConfig();
                    DrawGameplayScreen(gameTime);
                    break;
                case GameState.WonButStaying:
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
            string message = "The fishes have helped you to find the hidden key to treasure chests in return for your help. Position of they key will be displayed on the radar.";
            message = IngamePresentation.wrapLine(message, GraphicDevice.Viewport.TitleSafeArea.Width - 20, keyFoundFont);
            int foundKeyScreenHeight = (int)(GraphicDevice.Viewport.TitleSafeArea.Height);
            int foundKeyScreenWidth = (int)(GraphicDevice.Viewport.TitleSafeArea.Width);
            spriteBatch.Begin();
            //spriteBatch.Draw(foundKeyScreen, new Rectangle(GraphicDevice.Viewport.TitleSafeArea.Center.X - foundKeyScreenWidth/2, GraphicDevice.Viewport.TitleSafeArea.Center.Y-foundKeyScreenHeight/2, foundKeyScreenWidth, foundKeyScreenHeight), Color.White);
            spriteBatch.Draw(foundKeyScreen, new Rectangle(GraphicDevice.Viewport.TitleSafeArea.Center.X - foundKeyScreen.Width / 2, GraphicDevice.Viewport.TitleSafeArea.Center.Y - foundKeyScreen.Height / 2, foundKeyScreen.Width, foundKeyScreen.Height), Color.White);
            spriteBatch.DrawString(keyFoundFont, message, new Vector2(10, 130), Color.Gold);

            string nextText = "Press Enter to continue";
            Vector2 nextTextPosition = new Vector2(GraphicDevice.Viewport.TitleSafeArea.Right - menuSmall.MeasureString(nextText).X, GraphicDevice.Viewport.TitleSafeArea.Bottom - menuSmall.MeasureString(nextText).Y);
            spriteBatch.DrawString(menuSmall, nextText, nextTextPosition, Color.Blue);

            spriteBatch.End();
        }

        private void ResetStatisticCounters()
        {
            curNumBossDeaf = 0;
            curNumEnemyDeaf = 0;
            curHealthLost = 0;
            curNumFishSaved = 0;
            curTrashCollected = 0;
            numNormalKills = 0;
            numBossKills = 0;
            healthLost = 0;
            numTrashCollected = 0;
            concludeMusicPlayed = false;
            displayStatisticsNow = false;
        }

        private float curNumBossDeaf = 0, curNumEnemyDeaf = 0, curHealthLost = 0, curNumFishSaved = 0, curTrashCollected = 0;
        private bool concludeMusicPlayed = false;
        private void DrawWinOrLossScreen()
        {
            float xOffsetText, yOffsetText;
            Rectangle rectSafeArea;
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            xOffsetText = rectSafeArea.X;
            yOffsetText = rectSafeArea.Y;
            spriteBatch.Begin();
            if (currentGameState == GameState.Won)
            {
                spriteBatch.Draw(winningTexture, GraphicDevice.Viewport.TitleSafeArea, Color.White);
                //draw level statistics to feedback player

                string bossDefeatRank = "";
                string enemyDefeatRank = "";
                string healthLostRank = "";
                string fishSaveRank = "";
                string trashCollectRank = "";
                Texture2D overallRankTexture = null;
                string overallRank = "Overall rank:";
                string comment = "";

                LevelRanking(ref bossDefeatRank, ref enemyDefeatRank, ref healthLostRank, ref fishSaveRank, ref trashCollectRank, ref overallRankTexture, ref comment);

                int realNumFish = fishAmount;
                if (HydroBot.hasDolphin) realNumFish -= 1;
                if (HydroBot.hasSeaCow) realNumFish -= 1;
                if (HydroBot.hasTurtle) realNumFish -= 1;
                bool somethingIncreasing = false;

                string str1 = "";
                string str2 = "";
                string strComment = "";
                //the player wants to skip the counting and see the result rightaway
                if (displayStatisticsNow)
                {
                    curNumBossDeaf = numBossKills;
                    curNumEnemyDeaf = numNormalKills;
                    curHealthLost = healthLost;
                    curNumFishSaved = realNumFish;
                    curTrashCollected = numTrashCollected;
                }
                if (bossDefeatRank != "")
                {
                    str1 += "Number of boss defeated: " + (int)curNumBossDeaf + "\n";
                    curNumBossDeaf += 0.2f;
                    if (curNumBossDeaf > numBossKills) curNumBossDeaf = numBossKills;
                    if (curNumBossDeaf == numBossKills)
                        str2 += bossDefeatRank;
                    else
                        somethingIncreasing = true;
                    str2 += "\n";
                }
                if (enemyDefeatRank != "")
                {
                    str1 += "Number of enemies defeated: " + (int)curNumEnemyDeaf + "\n";
                    curNumEnemyDeaf += 0.2f;
                    if (curNumEnemyDeaf > numNormalKills) curNumEnemyDeaf = numNormalKills;
                    if (curNumEnemyDeaf == numNormalKills)
                        str2 += enemyDefeatRank;
                    else somethingIncreasing = true;
                    str2 += "\n";
                }
                if (healthLostRank != "")
                {
                    str1 += "Health lost: " + (int)curHealthLost + "\n";
                    curHealthLost++;
                    if (curHealthLost > healthLost) curHealthLost = healthLost;
                    if (curHealthLost == healthLost)
                        str2 += healthLostRank;
                    else somethingIncreasing = true;
                    str2 += "\n";
                }
                if (fishSaveRank != "")
                {
                    str1 += "Number of fishes saved: " + (int)curNumFishSaved + "\n";
                    curNumFishSaved += 0.2f;
                    if (curNumFishSaved > realNumFish) curNumFishSaved = realNumFish;
                    if (curNumFishSaved == realNumFish)
                        str2 += fishSaveRank;
                    else somethingIncreasing = true;
                    str2 += "\n";
                }
                if (trashCollectRank != "")
                {
                    str1 += "Number of trash collected: " + (int)curTrashCollected + "\n";
                    curTrashCollected += 0.2f;
                    if (curTrashCollected > numTrashCollected) curTrashCollected = numTrashCollected;
                    if (curTrashCollected == numTrashCollected)
                        str2 += trashCollectRank;
                    else somethingIncreasing = true;
                    str2 += "\n";
                }

                if (!somethingIncreasing)
                {
                    strComment = comment;
                    if (!concludeMusicPlayed)
                    {
                        PoseidonGame.audio.bodyHit.Play();
                        concludeMusicPlayed = true;
                    }
                }
                else PoseidonGame.audio.reelHit.Play();


                string winningText = "JUSTICE ALWAYS WIN!";
                Vector2 winningTextPos = new Vector2(game.Window.ClientBounds.Width / 2, 10 + statisticFont.MeasureString(winningText).Y / 2);
                spriteBatch.DrawString(statisticFont, winningText, winningTextPos, Color.Gold, 0, new Vector2(statisticFont.MeasureString(winningText).X / 2, statisticFont.MeasureString(winningText).Y / 2), 1.5f, SpriteEffects.None, 0);
                string levelStatsString = "Level Statistics";
                Vector2 levelStatsPos = winningTextPos + new Vector2(0, statisticFont.MeasureString(winningText).Y / 2 * 1.5f + 30 + statisticFont.MeasureString(levelStatsString).Y / 2);
                spriteBatch.DrawString(statisticFont, levelStatsString, levelStatsPos, Color.Red, 0, new Vector2(statisticFont.MeasureString(levelStatsString).X / 2, statisticFont.MeasureString(levelStatsString).Y / 2), 1.0f, SpriteEffects.None, 0);
                //spriteBatch.Draw(statisticLogoTexture, new Vector2(game.Window.ClientBounds.Width / 2, game.Window.ClientBounds.Height / 4 + 20), null, Color.White, 0, new Vector2(statisticLogoTexture.Width / 2, statisticLogoTexture.Height / 2), 1.0f, SpriteEffects.None, 0);
                //spriteBatch.DrawString(statsFont, strTitle, new Vector2(game.Window.ClientBounds.Width / 2, game.Window.ClientBounds.Height / 4), Color.Red, 0, new Vector2(statsFont.MeasureString(strTitle).X / 2, statsFont.MeasureString(strTitle).Y / 2), 3.0f, SpriteEffects.None, 0);
                spriteBatch.DrawString(statisticFont, str1, new Vector2(game.Window.ClientBounds.Width / 4, levelStatsPos.Y + statisticFont.MeasureString(levelStatsString).Y / 2 + 20), Color.Yellow, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
                spriteBatch.DrawString(statisticFont, str2, new Vector2(3 * game.Window.ClientBounds.Width / 4, levelStatsPos.Y + statisticFont.MeasureString(levelStatsString).Y / 2 + 20), Color.Red, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
                if (!somethingIncreasing)
                {
                    Vector2 overallRankTextPos = new Vector2(game.Window.ClientBounds.Width / 2 - statisticFont.MeasureString(overallRank).X / 2, levelStatsPos.Y + statisticFont.MeasureString(levelStatsString).Y / 2 + 20 + statisticFont.MeasureString(str1).Y * 0.6f);
                    spriteBatch.DrawString(statisticFont, overallRank, overallRankTextPos, Color.Red, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(overallRankTexture, new Vector2(game.Window.ClientBounds.Width / 2 - overallRankTexture.Width / 2, overallRankTextPos.Y + statisticFont.MeasureString(overallRank).Y), null, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
                }
                spriteBatch.DrawString(statisticFont, strComment, new Vector2(game.Window.ClientBounds.Width / 2, 3 * game.Window.ClientBounds.Height / 4), Color.Red, 0, new Vector2(statisticFont.MeasureString(strComment).X / 2, statisticFont.MeasureString(strComment).Y / 2), 1.0f, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(losingTexture, GraphicDevice.Viewport.TitleSafeArea, Color.White);
                string losingText = "Sorry everyone, I could not do it...";
                Vector2 losingTextPos = new Vector2(game.Window.ClientBounds.Width / 2, 10 + statisticFont.MeasureString(losingText).Y / 2);
                spriteBatch.DrawString(statisticFont, losingText, losingTextPos, Color.Red, 0, new Vector2(statisticFont.MeasureString(losingText).X / 2, statisticFont.MeasureString(losingText).Y / 2), 1.5f, SpriteEffects.None, 0);      
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
            schoolOfFish1.Draw(gameTime, spriteBatch, frustum);
            schoolOfFish2.Draw(gameTime, spriteBatch, frustum);
            schoolOfFish3.Draw(gameTime, spriteBatch, frustum);
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
                        spriteBatch.Draw(stunnedIconTexture, drawPos - new Vector2(stunnedIconTexture.Width, stunnedIconTexture.Height), null, Color.White);
                        spriteBatch.End();
                        //RestoreGraphicConfig();
                    }
                    if (enemies[i].isFleeing == true)
                    {
                        Vector3 placeToDraw = GraphicDevice.Viewport.Project(enemies[i].Position, gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity);
                        Vector2 drawPos = new Vector2(placeToDraw.X, placeToDraw.Y);
                        spriteBatch.Begin();
                        spriteBatch.Draw(scaredIconTexture, drawPos - new Vector2(scaredIconTexture.Width, scaredIconTexture.Height), Color.White);
                        spriteBatch.End();
                        
                    }
                }
            }
            //RestoreGraphicConfig();
            //draw boundary of the game scene
            //gameBoundary.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
            //Draw points gained / lost
            if (!PoseidonGame.capturingCinematic)
            {
                foreach (Point point in points)
                {
                    point.Draw(spriteBatch, GraphicDevice, gameCamera);
                }
            }
            spriteBatch.Begin();
            if (!openFactoryConfigurationScene && !openResearchFacilityConfigScene && !(showFoundKey && firstShow) && !PoseidonGame.capturingCinematic)
            {
                IngamePresentation.DrawTimeRemaining(roundTimer, GraphicDevice, spriteBatch);
                DrawStats();
                IngamePresentation.DrawLiveTip(GraphicDevice, spriteBatch);
                DrawBulletType();
                DrawHeight();
                DrawRadar();
                IngamePresentation.DrawCollectionStatus(GraphicDevice, spriteBatch);
                IngamePresentation.DrawHydroBotStatus(GraphicDevice, spriteBatch);

                // Draw the factory panel
                factoryButtonPanel.Draw(spriteBatch);

                if (HydroBot.activeSkillID != -1) DrawActiveSkill();
                IngamePresentation.DrawLevelObjectiveIcon(GraphicDevice, spriteBatch);
                if (currentGameState == GameState.WonButStaying) IngamePresentation.DrawToNextLevelButton(spriteBatch);
                if (PoseidonGame.gamePlus)
                    IngamePresentation.DrawGamePlusLevel(spriteBatch);
                else
                    IngamePresentation.DrawTipIcon(GraphicDevice, spriteBatch);
            }
            if (openFactoryConfigurationScene)
                factoryToConfigure.DrawFactoryConfigurationScene(spriteBatch, menuSmall);
            if (openResearchFacilityConfigScene)
                researchFacility.DrawResearchFacilityConfigurationScene(spriteBatch, menuSmall);

            // spriteBatch.DrawString(statsFont, "Is bot moving: " + hydroBot.isMoving() + "\n", new Vector2(50, 50), Color.Black);
            if (!PoseidonGame.capturingCinematic)
                cursor.Draw(gameTime);
            spriteBatch.End();
            if (showFoundKey && firstShow)
            {
                DrawFoundKey();
                //return;
            }
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
            //graphics.GraphicsDevice.Clear(Color.Black);
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
                    if (PoseidonGame.DrawBoundingSphere){                 
                        RasterizerState rs = new RasterizerState();
                        rs.FillMode = FillMode.WireFrame;
                        GraphicDevice.RasterizerState = rs;
                        researchAnchor.DrawBoundingSphere(gameCamera.ViewMatrix,
                            gameCamera.ProjectionMatrix, boundingSphere);

                        rs = new RasterizerState();
                        rs.FillMode = FillMode.Solid;
                        GraphicDevice.RasterizerState = rs;
                    }
                }
                else if (factoryAnchor != null)
                {
                    //factoryAnchor.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalShading");
                    factoryAnchor.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "CustomAlphaShading");
                    if (PoseidonGame.DrawBoundingSphere)
                    {
                        RasterizerState rs = new RasterizerState();
                        rs.FillMode = FillMode.WireFrame;
                        GraphicDevice.RasterizerState = rs;
                        factoryAnchor.DrawBoundingSphere(gameCamera.ViewMatrix,
                            gameCamera.ProjectionMatrix, boundingSphere);

                        rs = new RasterizerState();
                        rs.FillMode = FillMode.Solid;
                        GraphicDevice.RasterizerState = rs;
                    }
                }
            }
        }

        private void DrawRadar()
        {
            radar.Draw(spriteBatch, hydroBot.Position, enemies, enemiesAmount, fish, fishAmount, shipWrecks, factories, researchFacility, powerpacks, staticObjects);
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
            //IngamePresentation.DrawDebug("isWandering " + fish[fishAmount - 1].isWandering + "\nisReturnBot " + fish[fishAmount - 1].isReturnBot + "\nisChasing " + fish[fishAmount - 1].isChasing + "\nisFighting " + fish[fishAmount - 1].isFighting + "\nisCasting " + fish[fishAmount - 1].isCasting + "\n", new Vector2(100, 100), spriteBatch);

            //too much texts on screen 

            IngamePresentation.DrawObjectPointedAtStatus(cursor, gameCamera, this.game, spriteBatch, fish, fishAmount, enemies, enemiesAmount, trashes, shipWrecks, factories, researchFacility, null, powerpacks, resources);
            IngamePresentation.DrawObjectUnderStatus(spriteBatch, gameCamera, hydroBot, GraphicDevice, powerpacks, resources, trashes, null, shipWrecks, factories, researchFacility);
            //Display Cyborg health
            //IngamePresentation.DrawHealthBar(game, spriteBatch, statsFont, (int)HydroBot.currentHitPoint, (int)HydroBot.maxHitPoint, game.Window.ClientBounds.Height - 5 - IngamePresentation.experienceBarHeight - 10 - IngamePresentation.healthBarHeight, "HEALTH", 1.0f);

            //Display Environment Bar
            if (HydroBot.currentEnvPoint > HydroBot.maxEnvPoint) HydroBot.currentEnvPoint = HydroBot.maxEnvPoint;
            IngamePresentation.DrawEnvironmentBar(game, spriteBatch, statsFont, HydroBot.currentEnvPoint, HydroBot.maxEnvPoint);

            //Display Level/Experience Bar
            //IngamePresentation.DrawLevelBar(game, spriteBatch, HydroBot.currentExperiencePts, HydroBot.nextLevelExperience, HydroBot.level, game.Window.ClientBounds.Height - 5, "EXPERIENCE LEVEL", Color.Brown);

            //Display Good will bar
            IngamePresentation.DrawGoodWillBar(game, spriteBatch, statsFont);

            //strPosition.Y += strSize.Y;
            //spriteBatch.DrawString(statsFont, str2, strPosition, Color.White);
        }

        // Draw the currently selected bullet type
        private void DrawBulletType()
        {

            IngamePresentation.DrawBulletType(GraphicDevice, spriteBatch);
        }

        // Draw the currently selected skill/spell
        private void DrawActiveSkill()
        {
            IngamePresentation.DrawActiveSkill(GraphicDevice, skillTextures, spriteBatch);
        }

        private void DrawOpeningCinematic()
        {
            if (currentGameState == GameState.PlayingOpeningCinematic)
            {
                graphics.GraphicsDevice.Clear(Color.Black);
                Texture2D playingTexture;
                if (PoseidonGame.videoPlayer.State == MediaState.Playing)
                {
                    playingTexture = PoseidonGame.videoPlayer.GetTexture();
                    spriteBatch.Begin();
                    //spriteBatch.Draw(playingTexture, new Rectangle((int)(graphics.PreferredBackBufferWidth / 4), (int)(graphics.PreferredBackBufferHeight/4), (int)(graphics.PreferredBackBufferWidth/2), (int)(graphics.PreferredBackBufferHeight/2)), Color.White);
                    spriteBatch.Draw(playingTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
                    string nextText = "Esc to skip cinematic";
                    Vector2 nextTextPosition = new Vector2(GraphicDevice.Viewport.TitleSafeArea.Right - menuSmall.MeasureString(nextText).X * GameConstants.generalTextScaleFactor, 0);
                    spriteBatch.DrawString(menuSmall, nextText, nextTextPosition, Color.Red, 0, Vector2.Zero, GameConstants.generalTextScaleFactor, SpriteEffects.None, 0);
                    spriteBatch.End();
                }
                if (PoseidonGame.videoPlayer.State == MediaState.Stopped)
                    currentGameState = GameState.PlayingCutScene;      
            }
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
                string text = IngamePresentation.wrapLine(cutSceneDialog.cutScenes[currentLevel][currentSentence].sentence, GraphicDevice.Viewport.TitleSafeArea.Width - 100, menuSmall, GameConstants.generalTextScaleFactor);
                spriteBatch.DrawString(menuSmall, text, new Vector2(botRectangle.Left + 50, botRectangle.Top + 60), Color.Blue, 0, Vector2.Zero, GameConstants.generalTextScaleFactor, SpriteEffects.None, 0);
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
                string text = IngamePresentation.wrapLine(cutSceneDialog.cutScenes[currentLevel][currentSentence].sentence, GraphicDevice.Viewport.TitleSafeArea.Width - 100, menuSmall, GameConstants.generalTextScaleFactor);
                spriteBatch.DrawString(menuSmall, text, new Vector2(PoseidonRectangle.Left + 50, PoseidonRectangle.Top + 65), Color.Blue, 0, Vector2.Zero, GameConstants.generalTextScaleFactor, SpriteEffects.None, 0);
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
                string text = IngamePresentation.wrapLine(cutSceneDialog.cutScenes[currentLevel][currentSentence].sentence, GraphicDevice.Viewport.TitleSafeArea.Width - 100, menuSmall, GameConstants.generalTextScaleFactor);
                spriteBatch.DrawString(menuSmall, text, new Vector2(terminatorRectangle.Left + 50, terminatorRectangle.Top + 60), Color.Blue, 0, Vector2.Zero, GameConstants.generalTextScaleFactor, SpriteEffects.None, 0);
            }
            //Narrator speaking
            if (cutSceneDialog.cutScenes[currentLevel][currentSentence].speakerID == 3)
            {
                talkingBox = Content.Load<Texture2D>("Image/Cutscenes/narratorBox");
                Rectangle narratorRectangle = new Rectangle(0, GraphicDevice.Viewport.TitleSafeArea.Height - talkingBox.Height, GraphicDevice.Viewport.TitleSafeArea.Width, talkingBox.Height);
                spriteBatch.Draw(talkingBox, narratorRectangle, Color.White);
                //draw what is said
                string text = IngamePresentation.wrapLine(cutSceneDialog.cutScenes[currentLevel][currentSentence].sentence, GraphicDevice.Viewport.TitleSafeArea.Width - 100, menuSmall, GameConstants.generalTextScaleFactor);
                spriteBatch.DrawString(menuSmall, text, new Vector2(narratorRectangle.Left + 50, narratorRectangle.Top + 30), Color.Black, 0, Vector2.Zero, GameConstants.generalTextScaleFactor, SpriteEffects.None, 0);
            }
            string nextText = "Enter to continue. Esc to skip.";
            Vector2 nextTextPosition = new Vector2(GraphicDevice.Viewport.TitleSafeArea.Right - menuSmall.MeasureString(nextText).X * GameConstants.generalTextScaleFactor, 0);
            spriteBatch.DrawString(menuSmall, nextText, nextTextPosition, Color.Red, 0, Vector2.Zero, GameConstants.generalTextScaleFactor, SpriteEffects.None, 0);

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
                    if (PoseidonGame.DrawBoundingSphere)
                    {
                        RasterizerState rs = new RasterizerState();
                        rs.FillMode = FillMode.WireFrame;
                        GraphicDevice.RasterizerState = rs;
                        factory.DrawBoundingSphere(gameCamera.ViewMatrix,
                            gameCamera.ProjectionMatrix, boundingSphere);

                        rs = new RasterizerState();
                        rs.FillMode = FillMode.Solid;
                        GraphicDevice.RasterizerState = rs;
                    }
                }
            }
            if (researchFacility != null)
            {
                factoryRealSphere = researchFacility.BoundingSphere;
                factoryRealSphere.Center.Y = researchFacility.Position.Y;
                if (factoryRealSphere.Intersects(frustum))
                {
                    researchFacility.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalShading");
                    if (PoseidonGame.DrawBoundingSphere)
                    {
                        RasterizerState rs = new RasterizerState();
                        rs.FillMode = FillMode.WireFrame;
                        GraphicDevice.RasterizerState = rs;
                        researchFacility.DrawBoundingSphere(gameCamera.ViewMatrix,
                            gameCamera.ProjectionMatrix, boundingSphere);

                        rs = new RasterizerState();
                        rs.FillMode = FillMode.Solid;
                        GraphicDevice.RasterizerState = rs;
                    }
                }
            }
            //Draw each static object
            //BoundingSphere sphereToCheck;
            foreach (StaticObject staticObject in staticObjects)
            {
                //staticObject.OriginalBoundingSphere.Center = staticObject.BoundingSphere.Center;
                //sphereToCheck = staticObject.BoundingSphere;
                //sphereToCheck.Radius /= staticObject.boundingSphereScale;
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
            //BoundingSphere sphereToCheck;
            for (int i = 0; i < enemiesAmount; i++)
            {
                //sphereToCheck = enemies[i].BoundingSphere;
                //sphereToCheck.Radius /= enemies[i].boundingSphereScale;
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

    }
}
