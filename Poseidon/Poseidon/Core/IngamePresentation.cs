using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Poseidon.Core
{
    public static class IngamePresentation
    {
        //static TimeSpan playTime = TimeSpan.Zero;
        static TimeSpan lastChangeTime = TimeSpan.Zero;
        static TimeSpan accelerationTime = TimeSpan.FromSeconds(3);
        static TimeSpan startAccelerationTime = TimeSpan.Zero;
        static float partialDraw = 0.0f;
        static float spinningSpeed = 0.1f;
        static bool goingToStopSpinning = false;
        static bool stoppedSpinning = true;
        static bool displayResult = false;
        static double timeStartDisplay;
        static float startingDisplayScale = 0.55f;
        static float resultDisplayScale = startingDisplayScale;
        static bool displayMusicPlayed = false, buzzNow = false;
        static Random random = new Random();
        //textures for good will bar
        static Texture2D[] iconTextures, resultTextures;
        private static Texture2D whiteTexture;
        static Texture2D GoodWillBar, EnvironmentBar, iconFrame;
        public static Texture2D buttonNormalTexture, buttonHoverTexture, buttonPressedTexture;
        static Texture2D HealthBar;

        public static SpriteFont statsFont, statisticFont, fishTalkFont, tipFont;

        public static List<Bubble> bubbles;
        public static double lastBubbleCreated = 0;

        public static int poseidonFace = 0, strengthIcon = 1, speedIcon = 2, shootRateIcon = 3, healthIcon = 4, bowIcon = 5, hammerIcon = 6,
            armorIcon = 7, sandalIcon = 8, beltIcon = 9, dolphinIcon = 10, seaCowIcon = 11, turtleIcon = 12;

        // Texture and font for property window of a factory
        public static Texture2D factoryPanelTexture;
        public static SpriteFont factoryFont, factoryPanelFont;
        public static Texture2D factoryBackground;
        public static Texture2D factoryProduceButtonNormalTexture, factoryProduceButtonHoverTexture, factoryProduceButtonPressedTexture;
        public static Texture2D dummyTexture;

        // Texture and font for property window of a research facility
        public static SpriteFont facilityFont;
        public static SpriteFont facilityFont2;
        public static Texture2D facilityBackground;
        public static Texture2D facilityUpgradeButton;
        public static Texture2D playJigsawButton;
        public static Texture2D increaseAttributeButtonNormalTexture, increaseAttributeButtonHoverTexture, increaseAttributeButtonPressedTexture;
        public static SpriteFont menuSmall, largeFont, typeFont;

        //level winning/losing screen
        public static Texture2D winningTexture, losingTexture, actionTexture, stunnedTexture, scaredIconTexture, goldenKeyTexture;

        //for displaying tip on screen
        public static int currentTipID = 0;
        public static double lastTipChange = 0;
        public static int lastCurrentLevel = 0;
        public static float fadeFactorBeginValue = 4.0f;
        public static float fadeFactorReduceStep = 0.01f;
        public static float fadeFactor = fadeFactorBeginValue;

        //to next level button
        public static Texture2D toNextLevelNormalTexture, toNextLevelHoverTexture;

        // For drawing the currently selected skill
        public static Texture2D[] skillTextures;
        // For drawing the currently selected bullet type
        public static Texture2D[] bulletTypeTextures;

        //school of fish textures
        public static Texture2D fishTexture1, fishTexture2, fishTexture3;

        public static Texture2D levelObjectiveNormalIconTexture, levelObjectiveHoverIconTexture, tipNormalIconTexture, tipHoverIconTexture;

        // Textures for animating the processing state of factories.
        // Plastic factory will use nuclear factory textures
        public static List<Texture2D> biofactoryAnimationTextures;
        public static List<Texture2D> nuclearFactoryAnimationTextures;
        public static List<Texture2D> plasticFactoryLevelTextures;
        public static List<Texture2D> biodegradableFactoryLevelTextures;

        public static Texture2D laserBeamTexture, healLaserBeamTexture, submarineLaserBeamTexture, terminatorBullet;

        public static Texture2D normalCursorTexture;
        public static Texture2D shootingCursorTexture;
        public static Texture2D onFishCursorTexture;
        public static Texture2D onAttributeCursorTexture;
        public static Texture2D menuCursorTexture;

        public static Texture2D letAIHandleNormalTexture, letAIHandleHoverTexture;

        public static Texture2D exitShipWreckTexture;

        public static Texture2D levelObjectiveIconTexture;
        public static Rectangle levelObjectiveIconRectangle;
        public static Texture2D arrowTexture;

        public static bool levelObjHover = false;
        public static bool tipHover = false;
        public static Texture2D tipIconTexture;
        public static Rectangle tipIconRectangle;

        //for text scaling
        private static int commentMaxLength;
        public static float textScaleFactor;
        public static float lineSpacing;

        //textures showing hydrobot's statuses
        static Texture2D bioIcon, plasticIcon, radioIcon, resourceIcon, strangeRockIcon, experienceIcon, botHealthIcon, energyIcon;

        public static Texture2D introScene, introScene1;

        public static void Initiate2DGraphics(ContentManager Content, Game game)
        {
            commentMaxLength = game.Window.ClientBounds.Width / 4;
            textScaleFactor = (float)game.Window.ClientBounds.Width / 1440 * (float)game.Window.ClientBounds.Height / 900;
            textScaleFactor = (float)Math.Sqrt(textScaleFactor);
            if (textScaleFactor > 1) textScaleFactor = 1;
            GameConstants.generalTextScaleFactor = textScaleFactor;
            lineSpacing = GameConstants.lineSpacing / 2;

            iconTextures = new Texture2D[GameConstants.NumGoodWillBarIcons];
            iconTextures[poseidonFace] = Content.Load<Texture2D>("Image/SpinningReel/poseidonFace");
            iconTextures[strengthIcon] = Content.Load<Texture2D>("Image/SpinningReel/strengthIcon");
            iconTextures[speedIcon] = Content.Load<Texture2D>("Image/SpinningReel/speedIcon");
            iconTextures[shootRateIcon] = Content.Load<Texture2D>("Image/SpinningReel/shootRateIcon");
            iconTextures[bowIcon] = Content.Load<Texture2D>("Image/SpinningReel/bowIcon");
            iconTextures[hammerIcon] = Content.Load<Texture2D>("Image/SpinningReel/hammerIcon");
            iconTextures[armorIcon] = Content.Load<Texture2D>("Image/SpinningReel/armorIcon");
            iconTextures[sandalIcon] = Content.Load<Texture2D>("Image/SpinningReel/sandalIcon");
            iconTextures[beltIcon] = Content.Load<Texture2D>("Image/SpinningReel/beltIcon");
            iconTextures[healthIcon] = Content.Load<Texture2D>("Image/SpinningReel/healthIcon");
            iconTextures[dolphinIcon] = Content.Load<Texture2D>("Image/SpinningReel/dolphinIcon");
            iconTextures[seaCowIcon] = Content.Load<Texture2D>("Image/SpinningReel/seaCowIcon");
            iconTextures[turtleIcon] = Content.Load<Texture2D>("Image/SpinningReel/turtleIcon");
            resultTextures = new Texture2D[GameConstants.NumGoodWillBarIcons];
            resultTextures[poseidonFace] = Content.Load<Texture2D>("Image/SpinningReel/poseiResult");
            resultTextures[strengthIcon] = Content.Load<Texture2D>("Image/SpinningReel/attributeIncreased");
            resultTextures[speedIcon] = Content.Load<Texture2D>("Image/SpinningReel/attributeIncreased");
            resultTextures[shootRateIcon] = Content.Load<Texture2D>("Image/SpinningReel/attributeIncreased");
            resultTextures[bowIcon] = Content.Load<Texture2D>("Image/SpinningReel/skillIncreased");
            resultTextures[hammerIcon] = Content.Load<Texture2D>("Image/SpinningReel/skillIncreased");
            resultTextures[armorIcon] = Content.Load<Texture2D>("Image/SpinningReel/skillIncreased");
            resultTextures[sandalIcon] = Content.Load<Texture2D>("Image/SpinningReel/skillIncreased");
            resultTextures[beltIcon] = Content.Load<Texture2D>("Image/SpinningReel/skillIncreased");
            resultTextures[healthIcon] = Content.Load<Texture2D>("Image/SpinningReel/attributeIncreased");
            resultTextures[dolphinIcon] = Content.Load<Texture2D>("Image/SpinningReel/friendshipIncreased");
            resultTextures[seaCowIcon] = Content.Load<Texture2D>("Image/SpinningReel/friendshipIncreased");
            resultTextures[turtleIcon] = Content.Load<Texture2D>("Image/SpinningReel/friendshipIncreased");
            whiteTexture = Content.Load<Texture2D>("Image/SpinningReel/whiteTexture");

            buttonNormalTexture = Content.Load<Texture2D>("Image/ButtonTextures/buttonFrame");
            buttonHoverTexture = Content.Load<Texture2D>("Image/ButtonTextures/buttonFrameHover");
            buttonPressedTexture = Content.Load<Texture2D>("Image/ButtonTextures/buttonFramePressed");

            // Load Textures and fonts for factory control panel
            factoryBackground = Content.Load<Texture2D>("Image/TrashManagement/futuristicControlPanel");
            factoryProduceButtonNormalTexture = Content.Load<Texture2D>("Image/ButtonTextures/buttonLargeFrame"); //("Image/TrashManagement/ChangeFactoryProduceBox");
            factoryProduceButtonHoverTexture = Content.Load<Texture2D>("Image/ButtonTextures/buttonFrameHover");
            factoryProduceButtonPressedTexture = Content.Load<Texture2D>("Image/ButtonTextures/buttonFramePressed");
            dummyTexture = new Texture2D(PoseidonGame.graphics.GraphicsDevice, 2, 2); // create a dummy 2x2 texture
            dummyTexture.SetData(new int[4]);


            // Load Textures for research facility property dialog
            facilityBackground = Content.Load<Texture2D>("Image/TrashManagement/futuristicControlPanel2");
            facilityUpgradeButton = Content.Load<Texture2D>("Image/TrashManagement/increaseAttributeButton");
            playJigsawButton = Content.Load<Texture2D>("Image/TrashManagement/upgradeButton");
            increaseAttributeButtonNormalTexture = Content.Load<Texture2D>("Image/TrashManagement/increaseAttributeButton");
            increaseAttributeButtonHoverTexture = Content.Load<Texture2D>("Image/TrashManagement/increaseAttributeButtonHover");
            increaseAttributeButtonPressedTexture = Content.Load<Texture2D>("Image/TrashManagement/increaseAttributeButtonPressed");
            

            winningTexture = Content.Load<Texture2D>("Image/SceneTextures/LevelWinNew");
            losingTexture = Content.Load<Texture2D>("Image/SceneTextures/GameOverNew");
            actionTexture = Content.Load<Texture2D>("Image/Miscellaneous/actionTextures");
            stunnedTexture = Content.Load<Texture2D>("Image/Miscellaneous/dizzy-icon");
            scaredIconTexture = Content.Load<Texture2D>("Image/Miscellaneous/scared-icon");
            arrowTexture = Content.Load<Texture2D>("Image/Miscellaneous/arrow");

            iconFrame = Content.Load<Texture2D>("Image/SpinningReel/transparent_frame");
            GoodWillBar = Content.Load<Texture2D>("Image/Miscellaneous/goodWillBar");
            EnvironmentBar = Content.Load<Texture2D>("Image/Miscellaneous/EnvironmentBarNew");
            HealthBar = Content.Load<Texture2D>("Image/Miscellaneous/HealthBarNew");
            bubbles = new List<Bubble>();

            //load all fonts
            statsFont = Content.Load<SpriteFont>("Fonts/StatsFont");
            statisticFont = Content.Load<SpriteFont>("Fonts/statisticsfont");
            menuSmall = Content.Load<SpriteFont>("Fonts/menuSmall");
            facilityFont = Content.Load<SpriteFont>("Fonts/factoryConfig");
            facilityFont2 = Content.Load<SpriteFont>("Fonts/factoryConfig");
            factoryFont = Content.Load<SpriteFont>("Fonts/factoryConfig");
            fishTalkFont = Content.Load<SpriteFont>("Fonts/fishTalk");
            largeFont = Content.Load<SpriteFont>("Fonts/menuLarge");
            typeFont = Content.Load<SpriteFont>("Fonts/font");
            factoryPanelFont = Content.Load<SpriteFont>("Fonts/panelInfoText");
            tipFont = Content.Load<SpriteFont>("Fonts/tip");

            toNextLevelHoverTexture = Content.Load<Texture2D>("Image/Miscellaneous/tonextlevelhover");
            toNextLevelNormalTexture = Content.Load<Texture2D>("Image/Miscellaneous/tonextlevel");

            exitShipWreckTexture = Content.Load<Texture2D>("Image/Miscellaneous/exitShipWreckTip");

            skillTextures = new Texture2D[GameConstants.numberOfSkills];
            bulletTypeTextures = new Texture2D[GameConstants.numBulletTypes];
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

            goldenKeyTexture = Content.Load<Texture2D>("Image/SceneTextures/goldkey");

            fishTexture1 = Content.Load<Texture2D>("Image/FishSchoolTextures/smallfish1");
            fishTexture2 = Content.Load<Texture2D>("Image/FishSchoolTextures/smallfish2-1");
            fishTexture3 = Content.Load<Texture2D>("Image/FishSchoolTextures/smallfish3");

            levelObjectiveNormalIconTexture = Content.Load<Texture2D>("Image/Miscellaneous/LevelObjectiveIcon");
            levelObjectiveHoverIconTexture = Content.Load<Texture2D>("Image/Miscellaneous/LevelObjectiveIconHover");
            tipNormalIconTexture = Content.Load<Texture2D>("Image/Miscellaneous/tipIcon");
            tipHoverIconTexture = Content.Load<Texture2D>("Image/Miscellaneous/tipIconHover");

            // Load lower left pannel button
            factoryPanelTexture = Content.Load<Texture2D>("Image/ButtonTextures/factory_button");

            // Load textures for partid animation for factories
            biofactoryAnimationTextures = new List<Texture2D>();
            nuclearFactoryAnimationTextures = new List<Texture2D>();
            for (int i = 0; i < 6; i++)
            {
                biofactoryAnimationTextures.Add(Content.Load<Texture2D>("Image/TrashManagement/conveyor_bench" + i));
            }
            nuclearFactoryAnimationTextures.Add(Content.Load<Texture2D>("Image/TrashManagement/orange"));
            nuclearFactoryAnimationTextures.Add(Content.Load<Texture2D>("Image/TrashManagement/yellow"));

            // Factory level textures
            plasticFactoryLevelTextures = new List<Texture2D>();
            biodegradableFactoryLevelTextures = new List<Texture2D>();
            for (int i = 0; i < 2; i++)
            {
                // using same level textures for both factories
                Texture2D loadedTexture = Content.Load<Texture2D>("Image/TrashManagement/BiodegradableFactory_level" + i);
                plasticFactoryLevelTextures.Add(loadedTexture);
                biodegradableFactoryLevelTextures.Add(loadedTexture);
            }
            Texture2D lastLevelTexture = Content.Load<Texture2D>("Image/TrashManagement/BiodegradableFactory_level2");
            biodegradableFactoryLevelTextures.Add(lastLevelTexture);
            lastLevelTexture = Content.Load<Texture2D>("Image/TrashManagement/PlasticFactory_level2");
            plasticFactoryLevelTextures.Add(lastLevelTexture);

            laserBeamTexture = PoseidonGame.contentManager.Load<Texture2D>("Image/BulletIcons/redBall");
            healLaserBeamTexture = PoseidonGame.contentManager.Load<Texture2D>("Image/BulletIcons/greenBall");
            submarineLaserBeamTexture = PoseidonGame.contentManager.Load<Texture2D>("Image/BulletIcons/laserBeam");
            terminatorBullet = PoseidonGame.contentManager.Load<Texture2D>("Image/BulletIcons/redBallT");

            normalCursorTexture = Content.Load<Texture2D>("Image/CursorTextures/Starfish-cursor");
            shootingCursorTexture = Content.Load<Texture2D>("Image/CursorTextures/shootcursor");
            onFishCursorTexture = Content.Load<Texture2D>("Image/CursorTextures/fishcursorNew1");
            onAttributeCursorTexture = Content.Load<Texture2D>("Image/CursorTextures/hammerAndWrench");
            menuCursorTexture = Content.Load<Texture2D>("Image/CursorTextures/menuCursor");

            letAIHandleNormalTexture = Content.Load<Texture2D>("Image/MinigameTextures/letAIHandle");
            letAIHandleHoverTexture = Content.Load<Texture2D>("Image/MinigameTextures/letAIHandleHover");

            bioIcon = Content.Load<Texture2D>("Image/HydroBotStatus/bio_icon");
            plasticIcon = Content.Load<Texture2D>("Image/HydroBotStatus/plastic_icon");
            radioIcon = Content.Load<Texture2D>("Image/HydroBotStatus/radioactive_icon");
            resourceIcon = Content.Load<Texture2D>("Image/HydroBotStatus/resources");
            strangeRockIcon = Content.Load<Texture2D>("Image/HydroBotStatus/strange_rock");
            experienceIcon = Content.Load<Texture2D>("Image/HydroBotStatus/experience");
            botHealthIcon = Content.Load<Texture2D>("Image/HydroBotStatus/health");
            energyIcon = Content.Load<Texture2D>("Image/HydroBotStatus/energy_icon");

            introScene = Content.Load<Texture2D>("Image/SceneTextures/introScreen0");
            introScene1 = Content.Load<Texture2D>("Image/SceneTextures/introScreen1");
        }

        public static Model bossBullet, chasingBullet, damageBullet, healBullet, herculesArrow, mjolnir, normalbullet, piercingArrow, torpedo,
            diverGun, diverKnife, mutantShark, ghostPirate, submarine, terminator, bioFactory, bioFactoryStage0, bioFactoryStage1, bioFactoryState2, bioFactoryStage3,
            nuclearFactory, nuclearFactoryStage0, nuclearFactoryStage1, nuclearFactoryStage2, nuclearFactoryStage3, plasticFactory, plasticFactoryStage0, plasticFactoryStage1, plasticFactoryStage2, plasticFactoryStage3,
            researchFacility, researchFacilityStage0, researchFacilityStage1, researchFacilityStage2, researchFacilityStage3, hydrobot, goldenKey, boundingSphere, strangeRock1, strangeRock2,
            healthPowPack, resource, shootratePowPack, speedPowPack, strengthPowPack,
            dolphin, hammershark, leopardshark, manetee, mauiDolphin, meiolania, normalshark, orca, penguin, seal, stellarSeaCow, stingray, turtle,
            barrel, barrelstack, boxstack, chestClosed, chest, shipwreck2, shipwreck3, shipwreck4, shipwreckscene1, shipwreckscene2, shipwreckscene3, shipwreckscene4, shipwreckscene5, shipwreckscene6,
            bioTrash, plasticTrash, radioTrash;
        public static bool toNextLevelHover;
        public static Texture2D toNextLevelTexture;
        public static Rectangle toNextLevelIconRectangle;

        public static void Initiate3DGraphics(ContentManager Content)
        {
            bossBullet = Content.Load<Model>("Models/BulletModels/bossBullet");
            chasingBullet = Content.Load<Model>("Models/BulletModels/chasingBullet");
            damageBullet = Content.Load<Model>("Models/BulletModels/damageBullet");
            healBullet = Content.Load<Model>("Models/BulletModels/healBullet");
            herculesArrow = Content.Load<Model>("Models/BulletModels/herculesArrow");
            mjolnir = Content.Load<Model>("Models/BulletModels/mjolnir");
            normalbullet = Content.Load<Model>("Models/BulletModels/normalbullet");
            piercingArrow = Content.Load<Model>("Models/BulletModels/piercingArrow");
            torpedo = Content.Load<Model>("Models/BulletModels/torpedo");

            diverGun = Content.Load<Model>("Models/EnemyModels/diver_green_ly");
            diverKnife = Content.Load<Model>("Models/EnemyModels/diver_knife_orange_yellow");
            mutantShark = Content.Load<Model>("Models/EnemyModels/mutantSharkVer2");
            ghostPirate = Content.Load<Model>("Models/EnemyModels/skeletonrigged");
            submarine = Content.Load<Model>("Models/EnemyModels/submarine");
            terminator = Content.Load<Model>("Models/EnemyModels/terminator");
            
            bioFactory = Content.Load<Model>("Models/FactoryModels/biodegradablefactory");
            bioFactoryStage0 = Content.Load<Model>("Models/FactoryModels/biodegradablefactory_stage0");
            bioFactoryStage1 = Content.Load<Model>("Models/FactoryModels/biodegradablefactory_stage1");
            bioFactoryState2 = Content.Load<Model>("Models/FactoryModels/biodegradablefactory_stage2");
            bioFactoryStage3 = Content.Load<Model>("Models/FactoryModels/biodegradablefactory_stage3");
            nuclearFactory = Content.Load<Model>("Models/FactoryModels/nuclearfactory");
            nuclearFactoryStage0 = Content.Load<Model>("Models/FactoryModels/nuclearfactory_stage0");
            nuclearFactoryStage1 = Content.Load<Model>("Models/FactoryModels/nuclearfactory_stage1");
            nuclearFactoryStage2 = Content.Load<Model>("Models/FactoryModels/nuclearfactory_stage2");
            nuclearFactoryStage3 = Content.Load<Model>("Models/FactoryModels/nuclearfactory_stage3");
            plasticFactory = Content.Load<Model>("Models/FactoryModels/plasticfactory");
            plasticFactoryStage0 = Content.Load<Model>("Models/FactoryModels/plasticfactory_stage0");
            plasticFactoryStage1 = Content.Load<Model>("Models/FactoryModels/plasticfactory_stage1");
            plasticFactoryStage2 = Content.Load<Model>("Models/FactoryModels/plasticfactory_stage2");
            plasticFactoryStage3 = Content.Load<Model>("Models/FactoryModels/plasticfactory_stage3");
            researchFacility = Content.Load<Model>("Models/FactoryModels/researchfacility");
            researchFacilityStage0 = Content.Load<Model>("Models/FactoryModels/researchfacility_stage0");
            researchFacilityStage1 = Content.Load<Model>("Models/FactoryModels/researchfacility_stage1");
            researchFacilityStage2 = Content.Load<Model>("Models/FactoryModels/researchfacility_stage2");
            researchFacilityStage3 = Content.Load<Model>("Models/FactoryModels/researchfacility_stage3");
            
            hydrobot = Content.Load<Model>("Models/MainCharacter/bot");
            
            goldenKey = Content.Load<Model>("Models/Miscellaneous/goldenkey");
            boundingSphere = Content.Load<Model>("Models/Miscellaneous/sphere1ur");
            strangeRock1 = Content.Load<Model>("Models/Miscellaneous/strangerock1ver2");
            strangeRock2 = Content.Load<Model>("Models/Miscellaneous/strangerock2ver2");

            healthPowPack = Content.Load<Model>("Models/PowerpackResource/healthpowerpack");
            resource = Content.Load<Model>("Models/PowerpackResource/resource");
            shootratePowPack = Content.Load<Model>("Models/PowerpackResource/shootratepowerpack");
            speedPowPack = Content.Load<Model>("Models/PowerpackResource/speedpowerpack");
            strengthPowPack = Content.Load<Model>("Models/PowerpackResource/strengthpowerpack");

            dolphin = Content.Load<Model>("Models/SeaAnimalModels/dolphinVer3");
            hammershark = Content.Load<Model>("Models/SeaAnimalModels/hammersharkver2");
            leopardshark = Content.Load<Model>("Models/SeaAnimalModels/leopardsharkver3");
            manetee = Content.Load<Model>("Models/SeaAnimalModels/maneteever2");
            mauiDolphin = Content.Load<Model>("Models/SeaAnimalModels/mauidolphin");
            meiolania = Content.Load<Model>("Models/SeaAnimalModels/meiolaniawithanim");
            normalshark = Content.Load<Model>("Models/SeaAnimalModels/normalsharkver3");
            orca = Content.Load<Model>("Models/SeaAnimalModels/orcaver2");
            penguin = Content.Load<Model>("Models/SeaAnimalModels/penguin");
            seal = Content.Load<Model>("Models/SeaAnimalModels/sealver2");
            stellarSeaCow = Content.Load<Model>("Models/SeaAnimalModels/stellarseacow");
            stingray = Content.Load<Model>("Models/SeaAnimalModels/stingray");
            turtle = Content.Load<Model>("Models/SeaAnimalModels/turtle");

            barrel = Content.Load<Model>("Models/ShipWreckModels/barrel");
            barrelstack = Content.Load<Model>("Models/ShipWreckModels/barrelstack");
            boxstack = Content.Load<Model>("Models/ShipWreckModels/boxstack");
            chestClosed = Content.Load<Model>("Models/ShipWreckModels/chest-closed");
            chest = Content.Load<Model>("Models/ShipWreckModels/chest");
            shipwreck2 = Content.Load<Model>("Models/ShipWreckModels/shipwreck2");
            shipwreck3 = Content.Load<Model>("Models/ShipWreckModels/shipwreck3");
            shipwreck4 = Content.Load<Model>("Models/ShipWreckModels/shipwreck4");
            shipwreckscene1 = Content.Load<Model>("Models/ShipWreckModels/shipwreckscene1");
            shipwreckscene2 = Content.Load<Model>("Models/ShipWreckModels/shipwreckscene2");
            shipwreckscene3 = Content.Load<Model>("Models/ShipWreckModels/shipwreckscene3");
            shipwreckscene4 = Content.Load<Model>("Models/ShipWreckModels/shipwreckscene4");
            shipwreckscene5 = Content.Load<Model>("Models/ShipWreckModels/shipwreckscene5");
            shipwreckscene6 = Content.Load<Model>("Models/ShipWreckModels/shipwreckscene6");

            bioTrash = Content.Load<Model>("Models/TrashModels/biodegradableTrashVer4");
            plasticTrash = Content.Load<Model>("Models/TrashModels/plastictrashver3");
            radioTrash = Content.Load<Model>("Models/TrashModels/radioactivetrash"); ;
        }

        //public static void DrawDebug(string str, Vector2 position, SpriteBatch spriteBatch) {
        //    spriteBatch.DrawString(menuSmall, str, position, Color.White);
        //}

        private static Vector2 timerPos = Vector2.Zero;
        private static Color timerColor = Color.Red;
        private static double lastTimerColorChange = 0;
        public static void DrawTimeRemaining(TimeSpan roundTimer, GraphicsDevice GraphicDevice, SpriteBatch spriteBatch)
        {
            float xOffsetText, yOffsetText;
            int days;
            string str1 = GameConstants.StrTimeRemaining;
            Rectangle rectSafeArea;
            days = ((roundTimer.Minutes * 60) + roundTimer.Seconds) / GameConstants.DaysPerSecond;
            if (PlayGameScene.currentLevel == 0 && HydroBot.gameMode == GameMode.MainGame)
                str1 += "Unlimited";
            else str1 += days.ToString();

            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            xOffsetText = rectSafeArea.X;
            yOffsetText = rectSafeArea.Y;

            timerPos = new Vector2((int)xOffsetText + 10, (int)yOffsetText + 10);

            if (days >= 10) timerColor = Color.Red;
            else if (!(PlayGameScene.currentLevel == 0) && PoseidonGame.playTime.TotalMilliseconds - lastTimerColorChange >= 500)
            {
                if (timerColor == Color.Red)
                    timerColor = Color.White;
                else timerColor = Color.Red;
                lastTimerColorChange = PoseidonGame.playTime.TotalMilliseconds;
            }
            //else timerColor = Color.Red;

            //str1 += "\n" + (int)HydroBot.currentEnergy + "\\" + (int)HydroBot.maxEnergy; 
            
            spriteBatch.DrawString(menuSmall, str1, timerPos, timerColor, 0, Vector2.Zero, textScaleFactor, SpriteEffects.None, 0);
        }
        public static void DrawLiveTip(GraphicsDevice GraphicDevice,SpriteBatch spriteBatch)
        {
            if (!GameSettings.ShowLiveTip) return;

            Rectangle rectSafeArea;
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            float xOffsetText, yOffsetText;
            xOffsetText = timerPos.X;
            yOffsetText = timerPos.Y + menuSmall.MeasureString(GameConstants.StrTimeRemaining).Y * textScaleFactor + 10 ;

            if (lastCurrentLevel != PlayGameScene.currentLevel)
            {
                currentTipID = 0;
                lastCurrentLevel = PlayGameScene.currentLevel;
                lastTipChange = PoseidonGame.playTime.TotalSeconds;
                fadeFactor = fadeFactorBeginValue;
            }
            string tipStr = "Tip: " + PoseidonGame.liveTipManager.allTips[PlayGameScene.currentLevel][currentTipID].tipItemStr;
            tipStr = wrapLine(tipStr, rectSafeArea.Width / 4, statsFont, textScaleFactor);
            spriteBatch.DrawString(statsFont, tipStr, new Vector2(xOffsetText, yOffsetText), Color.White * fadeFactor, 0, Vector2.Zero, textScaleFactor, SpriteEffects.None, 0);
            fadeFactor -= fadeFactorReduceStep;

            if (PoseidonGame.playTime.TotalSeconds - lastTipChange >= 15)
            {
                currentTipID++;
                if (currentTipID == PoseidonGame.liveTipManager.allTips[PlayGameScene.currentLevel].Count) currentTipID = 0;
                lastTipChange = PoseidonGame.playTime.TotalSeconds;
                fadeFactor = fadeFactorBeginValue;
                PoseidonGame.audio.PowerGet.Play();
            }
        }

        public static void DrawActiveSkill(GraphicsDevice GraphicDevice, Texture2D[] skillTextures, SpriteBatch spriteBatch)
        {
            int xOffsetText, yOffsetText;
            Rectangle rectSafeArea;

            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            //xOffsetText = rectSafeArea.Right - 400;
            //xOffsetText = rectSafeArea.Center.X + experienceBarLength / 2;
            //yOffsetText = rectSafeArea.Height - (int)(96 * GameConstants.generalTextScaleFactor);
            xOffsetText = rectSafeArea.Center.X + (int)(150 * textScaleFactor) + (int)(32 * textScaleFactor) + (int)(fishTalkFont.MeasureString("100").X * textScaleFactor);
            yOffsetText = rectSafeArea.Height - (int)(96 * GameConstants.generalTextScaleFactor);

            //Vector2 skillIconPosition =
            //    new Vector2((int)xOffsetText, (int)yOffsetText);

            Rectangle destRectangle = new Rectangle(xOffsetText, yOffsetText, (int)(96 * GameConstants.generalTextScaleFactor), (int)(96 * GameConstants.generalTextScaleFactor));

            //spriteBatch.Draw(skillTextures[tank.activeSkillID], skillIconPosition, Color.White);
            spriteBatch.Draw(skillTextures[HydroBot.activeSkillID], destRectangle, Color.White);

            //draw the 2nd skill icon if skill combo activated
            if (HydroBot.skillComboActivated && HydroBot.secondSkillID != -1)
            {
                destRectangle = new Rectangle(xOffsetText + (int)(96 * GameConstants.generalTextScaleFactor), yOffsetText, (int)(96 * GameConstants.generalTextScaleFactor), (int)(96 * GameConstants.generalTextScaleFactor));
                spriteBatch.Draw(skillTextures[HydroBot.secondSkillID], destRectangle, Color.White);
            }
        }
        // Draw the currently selected bullet type
        public static void DrawBulletType(GraphicsDevice GraphicDevice, SpriteBatch spriteBatch)
        {

            int xOffsetText, yOffsetText;
            Rectangle rectSafeArea;

            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            //xOffsetText = rectSafeArea.Center.X - experienceBarLength/2 - (int)(64 * GameConstants.generalTextScaleFactor);
            //yOffsetText = rectSafeArea.Height - (int)(64 * GameConstants.generalTextScaleFactor) - 5;
            xOffsetText = rectSafeArea.Center.X - (int)(32 * textScaleFactor) - (int)(150 * textScaleFactor) - (int)(96 * GameConstants.generalTextScaleFactor);
            yOffsetText = rectSafeArea.Height - (int)(80 * GameConstants.generalTextScaleFactor);

            //Vector2 bulletIconPosition =
            //    new Vector2((int)xOffsetText, (int)yOffsetText);
            Rectangle destRectangle = new Rectangle(xOffsetText, yOffsetText, (int)(64 * GameConstants.generalTextScaleFactor), (int)(64 * GameConstants.generalTextScaleFactor));
            //spriteBatch.Draw(bulletTypeTextures[tank.bulletType], bulletIconPosition, Color.White);
            spriteBatch.Draw(bulletTypeTextures[HydroBot.bulletType], destRectangle, Color.White);
        }

        public static int healthBarHeight;
        public static void DrawHealthBar(Game game, SpriteBatch spriteBatch, SpriteFont statsFont, int currentHealth, int maxHealth, int heightFromTop, string type, float opaqueValue)
        {
            type = type.ToUpper();
            int barLength = (int)(statsFont.MeasureString(type).X * 1.5f * textScaleFactor);
            if (barLength < HealthBar.Width * textScaleFactor) barLength = (int)(HealthBar.Width * textScaleFactor);
            //healthBarLength = barLength;
            int barHeight = (int)(22 * textScaleFactor);
            healthBarHeight = barHeight;
            int barX = game.Window.ClientBounds.Width / 2 - barLength / 2;
            int barY = heightFromTop;

            double healthiness = (double)currentHealth / maxHealth;

            //Draw the negative space for the health bar
            //spriteBatch.Draw(HealthBar,
            //    new Rectangle(barX, barY, barLength, barHeight),
            //    new Rectangle(0, barHeight + 1, HealthBar.Width, barHeight),
            //    Color.Transparent);
            //Draw the current health level based on the current Health
            Color healthColor = Color.LimeGreen;
            if (healthiness < 0.2)
                healthColor = Color.DarkRed;
            else if (healthiness < 0.5)
                healthColor = Color.Orange;
            //spriteBatch.Draw(HealthBar,
            //    new Rectangle(barX, barY, (int)(barLength * healthiness), barHeight),
            //    new Rectangle(0, 22, (int)(HealthBar.Width * healthiness), 22),
            //    healthColor);
            spriteBatch.Draw(HealthBar, new Vector2(barX, barY), new Rectangle(0, 22, (int)(HealthBar.Width * healthiness), 22), healthColor * opaqueValue,
                0, Vector2.Zero, new Vector2((float)barLength / HealthBar.Width, textScaleFactor), SpriteEffects.None, 0);
            //Draw the box around the health bar
            spriteBatch.Draw(HealthBar, new Vector2(barX, barY), new Rectangle(0, 0, HealthBar.Width, 22), Color.White * opaqueValue,
                0, Vector2.Zero, new Vector2((float)barLength / HealthBar.Width, textScaleFactor), SpriteEffects.None, 0);
            spriteBatch.DrawString(statsFont, type, new Vector2(game.Window.ClientBounds.Width / 2 - statsFont.MeasureString(type).X / 2 * textScaleFactor, heightFromTop - 1), Color.MediumVioletRed * opaqueValue,
                0, Vector2.Zero, textScaleFactor, SpriteEffects.None, 0);
        }

        public static int experienceBarLength, experienceBarHeight;
        public static void DrawLevelBar(Game game, SpriteBatch spriteBatch, int currentExperience, int nextLevelExp, int level, int heightFromBot, string type, Color typeColor)
        {
            type += " " + level.ToString();
            int barLength = (int)(statsFont.MeasureString(type).X * 1.5f * textScaleFactor);
            if (barLength < HealthBar.Width * textScaleFactor) barLength = (int)(HealthBar.Width * textScaleFactor);
            experienceBarLength = barLength;
            int barHeight = (int)(22 * textScaleFactor);
            experienceBarHeight = barHeight;
            int barX = game.Window.ClientBounds.Width / 2 - barLength / 2;
            int barY = heightFromBot - barHeight;
            double experience = (double)currentExperience / nextLevelExp;       
            //Draw the negative space for the health bar
            //spriteBatch.Draw(HealthBar,
            //    new Rectangle(barX, barY, HealthBar.Width, barHeight),
            //    new Rectangle(0, barHeight + 1, HealthBar.Width, barHeight),
            //    Color.Transparent);
            //Draw the current health level based on the current Health
            spriteBatch.Draw(HealthBar, new Vector2(barX, barY), new Rectangle(0, 22, (int)(HealthBar.Width * experience), 22), Color.CornflowerBlue,
                        0, Vector2.Zero, new Vector2((float)barLength / HealthBar.Width, textScaleFactor), SpriteEffects.None, 0);
            //Draw the box around the health bar
            spriteBatch.Draw(HealthBar, new Vector2(barX, barY), new Rectangle(0, 0, HealthBar.Width, 22), Color.White,
                0, Vector2.Zero, new Vector2((float)barLength / HealthBar.Width, textScaleFactor), SpriteEffects.None, 0);
            spriteBatch.DrawString(statsFont, type, new Vector2(game.Window.ClientBounds.Width / 2 - statsFont.MeasureString(type).X / 2 * textScaleFactor, barY - 1), Color.Gold,
                0, Vector2.Zero, textScaleFactor, SpriteEffects.None, 0);
        }

        public static void DrawEnvironmentBar(Game game, SpriteBatch spriteBatch, SpriteFont statsFont, int currentEnvironment, int maxEnvironemnt)
        {
            int barX = game.Window.ClientBounds.Width - 10 - (int)(41 * textScaleFactor);
            int barY = game.Window.ClientBounds.Height / 2 - (int)(EnvironmentBar.Height / 2 * textScaleFactor);
            string type = "ENVIRONMENT";
            Color typeColor = Color.IndianRed;
            int barWidth = (int)(41 * textScaleFactor);// EnvironmentBar.Width / 2;
            double healthiness = (double)currentEnvironment / maxEnvironemnt;
            //Draw the negative space for the health bar
            //spriteBatch.Draw(EnvironmentBar,
            //    new Rectangle(barX, barY, barWidth, EnvironmentBar.Height),
            //    new Rectangle(barWidth + 1, 0, barWidth, EnvironmentBar.Height),
            //    Color.Transparent);
            //Draw the current health level based on the current Health
            Color healthColor = Color.Gold;
            if (healthiness < 0.2)
                healthColor = Color.DarkRed;
            else if (healthiness < 0.5)
                healthColor = Color.Red;
            else if (healthiness < 0.8)
                healthColor = Color.LawnGreen;
            spriteBatch.Draw(EnvironmentBar,
                new Vector2(barX, barY + (EnvironmentBar.Height - (int)(EnvironmentBar.Height * healthiness)) * textScaleFactor),
                new Rectangle(45, EnvironmentBar.Height - (int)(EnvironmentBar.Height * healthiness), 43, (int)(EnvironmentBar.Height * healthiness)),
                healthColor, 0, Vector2.Zero, textScaleFactor, SpriteEffects.None, 0);
            //Draw the box around the health bar
            spriteBatch.Draw(EnvironmentBar, new Vector2(barX, barY), new Rectangle(0, 0, 41, EnvironmentBar.Height), Color.White,
                0, Vector2.Zero, textScaleFactor, SpriteEffects.None, 0);
            //spriteBatch.DrawString(statsFont, type.ToUpper(), new Vector2(game.Window.ClientBounds.Width / 2 - ((type.Length / 2) * 14), heightFromTop - 1), typeColor);
            //spriteBatch.DrawString(statsFont, type.ToUpper(), new Vector2(barX + 10, barY + 20), typeColor, 90.0f, new Vector2(barX + 10, barY + 20), 1, SpriteEffects.FlipVertically, 0);
            type = type.ToUpper();
            //spriteBatch.DrawString(statsFont, type.ToUpper(), new Vector2(barX + 35, barY + 70), typeColor, 3.14f / 2, new Vector2(0, 0), 1, SpriteEffects.None, 0);
            spriteBatch.DrawString(statsFont, type, new Vector2(barX + barWidth / 2, game.Window.ClientBounds.Height / 2), Color.Gold, 3.14f / 2, new Vector2(statsFont.MeasureString(type).X / 2, statsFont.MeasureString(type).Y / 2), textScaleFactor, SpriteEffects.None, 0);

            //draw the lobster on bottom
            //spriteBatch.Draw(lobsterTexture, new Vector2(barX + barWidth / 2, game.Window.ClientBounds.Height / 2 + EnvironmentBar.Height / 2 - 30), null, Color.White, 0, new Vector2(lobsterTexture.Width / 2, lobsterTexture.Height / 2), 1, SpriteEffects.None, 0);

            //draw floating bubbles inside tube
            if (PoseidonGame.playTime.TotalMilliseconds - lastBubbleCreated >= 250)
            {
                Bubble bubble = new Bubble();
                bubble.LoadContentBubbleSmall(PoseidonGame.contentManager, new Vector2(barX + barWidth / 2, barY + (EnvironmentBar.Height) * textScaleFactor - 8), barX, barX + barWidth - 3);
                bubbles.Add(bubble);
                lastBubbleCreated = PoseidonGame.playTime.TotalMilliseconds;
            }
            for (int i = 0; i < bubbles.Count; i++)
            {
                if (bubbles[i].bubble2DPos.Y - bubbles[i].bubbleTexture.Height/2 * bubbles[i].startingScale <= barY + (EnvironmentBar.Height - (int)(EnvironmentBar.Height * healthiness)) * textScaleFactor ||
                    bubbles[i].bubble2DPos.Y - bubbles[i].bubbleTexture.Height/2 * bubbles[i].startingScale <= barY + 4)
                    bubbles.RemoveAt(i--);

            }
            foreach (Bubble aBubble in bubbles)
            {
                aBubble.UpdateBubbleSmall();
                aBubble.DrawBubbleSmall(spriteBatch);
            }
        }
        public static void DrawGoodWillBar(Game game, SpriteBatch spriteBatch, SpriteFont statsFont)
        {
            if (!HydroBot.goodWillBarActivated) return;

            int barX = (int)((iconFrame.Width / 2 - 21) * textScaleFactor);
            int barY = game.Window.ClientBounds.Height / 2 - (int)(iconFrame.Height / 2 * textScaleFactor) + 15 ;//game.Window.ClientBounds.Height / 2 - (int)(GoodWillBar.Height / 2 * textScaleFactor);
            string type = "GOOD WILL";
            Color typeColor = Color.Gold;
            int barWidth = (int)(42 * textScaleFactor);// EnvironmentBar.Width / 2;
            double healthiness = (double)HydroBot.goodWillPoint / HydroBot.maxGoodWillPoint;

            //Draw the current health level based on the current Health
            Color healthColor = Color.Gold;
            if (healthiness < 0.2)
                healthColor = Color.DarkRed;
            else if (healthiness < 0.5)
                healthColor = Color.Red;
            else if (healthiness < 0.8)
                healthColor = Color.LawnGreen;
            //spriteBatch.Draw(GoodWillBar,
            //    new Vector2(barX, barY + (GoodWillBar.Height - (int)(GoodWillBar.Height * healthiness)) * textScaleFactor),
            //    new Rectangle(45, GoodWillBar.Height - (int)(GoodWillBar.Height * healthiness), 43, (int)(GoodWillBar.Height * healthiness)),
            //    healthColor, 0, Vector2.Zero, textScaleFactor, SpriteEffects.None, 0);
            //Draw the box around the health bar
            //spriteBatch.Draw(GoodWillBar, new Vector2(barX, barY), new Rectangle(0, 0, 42, GoodWillBar.Height), Color.White,
            //    0, Vector2.Zero, textScaleFactor, SpriteEffects.None, 0);
            //spriteBatch.DrawString(statsFont, type.ToUpper(), new Vector2(game.Window.ClientBounds.Width / 2 - ((type.Length / 2) * 14), heightFromTop - 1), typeColor);
            //spriteBatch.DrawString(statsFont, type.ToUpper(), new Vector2(barX + 10, barY + 20), typeColor, 90.0f, new Vector2(barX + 10, barY + 20), 1, SpriteEffects.FlipVertically, 0);
           
            //draw the spinning reel on top of the bar
            Color colorToDraw;
            int faceDrawNext = HydroBot.faceToDraw + 1;
            if (faceDrawNext == iconTextures.Length) faceDrawNext = 0;
            if (HydroBot.iconActivated[faceDrawNext]) colorToDraw = Color.White;
            else colorToDraw = Color.Black;
            spriteBatch.Draw(iconTextures[faceDrawNext], new Vector2(0, barY - iconTextures[faceDrawNext].Height * textScaleFactor - 15), new Rectangle(0, (int)(iconTextures[faceDrawNext].Height * (1.0f - partialDraw)), iconTextures[faceDrawNext].Width, (int)(iconTextures[faceDrawNext].Height * partialDraw)), colorToDraw, 
                0, Vector2.Zero, textScaleFactor, SpriteEffects.None, 0);
            if (HydroBot.iconActivated[HydroBot.faceToDraw]) colorToDraw = Color.White;
            else colorToDraw = Color.Black;
            spriteBatch.Draw(iconTextures[HydroBot.faceToDraw], new Vector2(0, barY - iconTextures[faceDrawNext].Height * textScaleFactor - 15 + iconTextures[HydroBot.faceToDraw].Height * textScaleFactor * partialDraw), new Rectangle(0, 0, iconTextures[HydroBot.faceToDraw].Width, (int)(iconTextures[HydroBot.faceToDraw].Height * (1.0f - partialDraw))), colorToDraw,
                0, Vector2.Zero, textScaleFactor, SpriteEffects.None, 0);

            //draw the bar indicator on top of the image
            //healthColor.A = 1;
            spriteBatch.Draw(whiteTexture, new Vector2(0, barY - whiteTexture.Height * textScaleFactor - 15 + (whiteTexture.Height - (int)(whiteTexture.Height * healthiness)) * textScaleFactor), new Rectangle(0, 0, whiteTexture.Width, (int)(whiteTexture.Height * healthiness)), healthColor,
                0, Vector2.Zero, textScaleFactor, SpriteEffects.None, 0);
            //draw the frame
            spriteBatch.Draw(iconFrame, new Vector2(0, barY - iconFrame.Height * textScaleFactor - 15), null, Color.White, 0, Vector2.Zero, textScaleFactor, SpriteEffects.None, 0);

            //draw the crab on bottom
            //spriteBatch.Draw(crabTexture, new Vector2(barX + barWidth / 2, game.Window.ClientBounds.Height / 2 + GoodWillBar.Height / 2 - 30), null, Color.White, 0, new Vector2(crabTexture.Width / 2, crabTexture.Height / 2), 1, SpriteEffects.None, 0);

            //draw the bar title
            type = type.ToUpper();
            //spriteBatch.DrawString(statsFont, type, new Vector2(barX + 10, barY + 200), typeColor, -3.14f / 2, new Vector2(0, 0), 1, SpriteEffects.None, 0);
            //spriteBatch.DrawString(statsFont, type, new Vector2(barX + barWidth / 2, game.Window.ClientBounds.Height / 2), Color.Gold, -3.14f / 2, new Vector2(statsFont.MeasureString(type).X / 2, statsFont.MeasureString(type).Y / 2), textScaleFactor, SpriteEffects.None, 0);
            spriteBatch.DrawString(statsFont, type, new Vector2(statsFont.MeasureString(type).X / 2 * 0.8f * textScaleFactor, barY - iconFrame.Height * textScaleFactor - 15 - statsFont.MeasureString(type).Y / 2 * 0.8f * textScaleFactor - 5), Color.Gold, 0, new Vector2(statsFont.MeasureString(type).X / 2, statsFont.MeasureString(type).Y / 2), 0.8f * textScaleFactor, SpriteEffects.None, 0);

            //display the result of the spin
            if (displayResult)
            {
                Vector2 posToDraw = new Vector2(iconFrame.Width / 2 * textScaleFactor, barY - (iconFrame.Height - 20) * textScaleFactor);
                if (buzzNow) posToDraw += new Vector2(random.Next(-5, 5), random.Next(-5, 5)) * textScaleFactor;
                spriteBatch.Draw(resultTextures[faceDrawNext], posToDraw, null, Color.White, 0,
                    new Vector2(resultTextures[faceDrawNext].Width / 2, resultTextures[faceDrawNext].Height / 2), resultDisplayScale * textScaleFactor, SpriteEffects.None, 0);
            }
        }
        public static void UpdateGoodWillBar()
        {
            int faceBeingShown;
            if (!HydroBot.goodWillBarActivated) return;
            if (PoseidonGame.playTime - lastChangeTime >= TimeSpan.FromMilliseconds(20) && !stoppedSpinning)
            {
                if (PoseidonGame.playTime - startAccelerationTime <= accelerationTime)
                    spinningSpeed += 0.003f;
                else
                {
                    if (spinningSpeed <= 0.05 && partialDraw < 1.0f)
                    {
                        goingToStopSpinning = true;
                    }
                    else
                    {
                        if (!goingToStopSpinning)
                            spinningSpeed -= 0.0005f;
                    }
                }

                partialDraw += spinningSpeed;
                if (partialDraw >= 1.0f)
                {
                    if (!goingToStopSpinning)
                    {
                        HydroBot.faceToDraw++;
                        partialDraw = partialDraw - 1.0f;
                        PoseidonGame.audio.reelHit.Play();
  
                    }
                    else
                    {
                        partialDraw = 1.0f;
                        stoppedSpinning = true;
                        PoseidonGame.audio.reelHit.Play();


                        //miracles happen here
                        //the face drawn on the screen is actually faceToDraw + 1
                        faceBeingShown = HydroBot.faceToDraw + 1;
                        if (faceBeingShown == GameConstants.NumGoodWillBarIcons) faceBeingShown = 0;
                        if (HydroBot.iconActivated[faceBeingShown]){

                            //display result of the spin
                            displayResult = true;
                            timeStartDisplay = PoseidonGame.playTime.TotalMilliseconds;

                            if (faceBeingShown == poseidonFace)
                            {
                                //fill up the hitpoint
                                HydroBot.currentHitPoint = HydroBot.maxHitPoint;
                                HydroBot.currentEnergy = HydroBot.maxEnergy;
                                //reset all skills' cooldown
                                for (int i = 0; i < GameConstants.numberOfSkills; i++)
                                {
                                    HydroBot.firstUse[i] = false;
                                }
                            }
                            else if (faceBeingShown == strengthIcon)
                            {
                                HydroBot.strength += GameConstants.gainStrength;
                            }
                            else if (faceBeingShown == speedIcon)
                            {
                                HydroBot.speed += GameConstants.gainSpeed;
                            }
                            else if (faceBeingShown == shootRateIcon)
                            {
                                HydroBot.shootingRate += GameConstants.gainShootingRate;
                            }
                            else if (faceBeingShown == healthIcon)
                            {
                                HydroBot.currentHitPoint += GameConstants.gainHitPoint;
                                HydroBot.maxHitPoint += GameConstants.gainHitPoint;
                                HydroBot.currentEnergy += GameConstants.EnergyGainPerUpgrade;
                                HydroBot.maxEnergy += GameConstants.EnergyGainPerUpgrade;
                            }
                            else if (faceBeingShown == bowIcon)
                            {
                                HydroBot.bowPower += 0.05f;
                            }
                            else if (faceBeingShown == hammerIcon)
                            {
                                HydroBot.hammerPower += 0.05f;
                            }
                            else if (faceBeingShown == armorIcon)
                            {
                                HydroBot.armorPower += 0.05f;
                            }
                            else if (faceBeingShown == sandalIcon)
                            {
                                HydroBot.sandalPower += 0.05f;
                            }
                            else if (faceBeingShown == beltIcon)
                            {
                                HydroBot.beltPower += 0.05f;
                            }
                            else if (faceBeingShown == dolphinIcon)
                            {
                                HydroBot.dolphinPower += 0.05f;
                            }
                            else if (faceBeingShown == seaCowIcon)
                            {
                                HydroBot.seaCowPower += 0.05f;
                            }
                            else if (faceBeingShown == turtleIcon)
                            {
                                HydroBot.turtlePower += 0.05f;
                            }

                        }
                        
                    }

                }
                if (HydroBot.faceToDraw == iconTextures.Length) HydroBot.faceToDraw = 0;
                lastChangeTime = PoseidonGame.playTime;
            }
            if (displayResult)
            {
                if (PoseidonGame.playTime.TotalMilliseconds - timeStartDisplay <= 500)
                {
                    resultDisplayScale -= 0.01f;
                }
                else if (PoseidonGame.playTime.TotalMilliseconds - timeStartDisplay <= 1000)
                {
                    if (resultDisplayScale > 0.26f) resultDisplayScale = 0.26f;
                    if (!displayMusicPlayed)
                    {
                        PoseidonGame.audio.superPunch.Play();
                        displayMusicPlayed = true;
                    }
                    buzzNow = true;
                }
                else if (PoseidonGame.playTime.TotalMilliseconds - timeStartDisplay <= 2500)
                {
                    buzzNow = false;
                }
                else
                {
                    displayResult = false;
                    resultDisplayScale = startingDisplayScale;
                    displayMusicPlayed = false;
                }
            }
        }
        public static void StopSpinning()
        {
            stoppedSpinning = true;
            partialDraw = 1.0f;
        }
        public static void SpinNow()
        {
            if (stoppedSpinning == true)
            {
                startAccelerationTime = PoseidonGame.playTime;
                spinningSpeed = 0.1f;
                goingToStopSpinning = stoppedSpinning = false;
                accelerationTime = TimeSpan.FromSeconds(random.Next(3) + 1);
            }
        }

        static bool fishWasPointedAt = false, enemyWasPointedAt = false, nonLivingObjWasPointedAt = false;
        static string fishTalk, swimmingObjName;
        static int swimmingObjHealth, swimmingObjMaxHealth;
        static string line = "", comment = "", tip = "", tip2 = "";
        static string prevLine = "", prevComment = "", prevTip = "", prevTip2 = "";
        static float opaqueValue = 1.0f, startingOpaqueValue = 1.0f;
        static float fadeStep = 0.01f;
        static double lastFadeChange = 0;
        public static void ResetObjPointedAtMsgs()
        {
            fishWasPointedAt = enemyWasPointedAt = nonLivingObjWasPointedAt = false;
        }
        public static void DrawObjectPointedAtStatus(GraphicsDevice graphicsDevice, Cursor cursor, Camera gameCamera, Game game, SpriteBatch spriteBatch, Fish[] fish, int fishAmount, BaseEnemy[] enemies, int enemiesAmount, List<Trash> trashes, List<ShipWreck> shipWrecks, List<Factory> factories, ResearchFacility researchFacility, List<TreasureChest> treasureChests, List<Powerpack> powerPacks, List<Resource> resources)
        {
            bool somethingPointedAt = false;
            string name = "";
            Vector3 position = Vector3.Zero;
            //Display Fish Health
            Fish fishPointedAt = CursorManager.MouseOnWhichFish(cursor, gameCamera, fish, fishAmount);
            if (fishPointedAt != null)
            {
                swimmingObjName = fishPointedAt.Name;
                swimmingObjHealth = (int)fishPointedAt.health;
                swimmingObjMaxHealth = (int)fishPointedAt.maxHealth;
                IngamePresentation.DrawHealthBar(game, spriteBatch, statsFont, swimmingObjHealth, swimmingObjMaxHealth, 5, swimmingObjName, 1.0f);
                //string fishTalk;
                fishTalk = "'";
                if (fishPointedAt.health < 20)
                {
                    fishTalk += "SAVE ME!!!";
                }
                else if (fishPointedAt.health < 60)
                {
                    fishTalk += IngamePresentation.wrapLine(fishPointedAt.sad_talk, commentMaxLength, fishTalkFont, textScaleFactor);
                }
                else
                {
                    fishTalk += IngamePresentation.wrapLine(fishPointedAt.happy_talk, commentMaxLength, fishTalkFont, textScaleFactor);
                }
                fishTalk += "'";
                spriteBatch.DrawString(fishTalkFont, fishTalk, new Vector2(game.Window.ClientBounds.Width / 2, 4 + (fishTalkFont.MeasureString(swimmingObjName).Y + fishTalkFont.MeasureString(fishTalk).Y / 2 + lineSpacing) * textScaleFactor), Color.Yellow, 0, new Vector2(fishTalkFont.MeasureString(fishTalk).X / 2, fishTalkFont.MeasureString(fishTalk).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                somethingPointedAt = true;
                fishWasPointedAt = true;
                enemyWasPointedAt = nonLivingObjWasPointedAt = false;
            }
            else
            {
                //fishWasPointedAt = false;
                //Display Enemy Health
                BaseEnemy enemyPointedAt = CursorManager.MouseOnWhichEnemy(cursor, gameCamera, enemies, enemiesAmount);
                if (enemyPointedAt != null)
                {
                    swimmingObjName = enemyPointedAt.Name;
                    swimmingObjHealth = (int)enemyPointedAt.health;
                    swimmingObjMaxHealth = (int)enemyPointedAt.maxHealth;
                    IngamePresentation.DrawHealthBar(game, spriteBatch, statsFont, swimmingObjHealth, swimmingObjMaxHealth, 5, swimmingObjName, 1.0f);
                    somethingPointedAt = true;
                    enemyWasPointedAt = true;
                    fishWasPointedAt = nonLivingObjWasPointedAt = false;
                }
                else
                {
                    //enemyWasPointedAt = false;
                    line = comment = tip = tip2 = "";
                    Powerpack powerPackPointedAt = null, botOnPowerPack = null;
                    CursorManager.MouseOnWhichPowerPack(cursor, gameCamera, powerPacks, ref powerPackPointedAt, ref botOnPowerPack, null);
                    if (powerPackPointedAt != null)
                    {
                        line = "";
                        comment = "";
                        if (powerPackPointedAt.powerType == PowerPackType.Speed)
                        {
                            line = "SPEED BOOST POWERPACK";
                            name = "Speed Boost Powerpack";
                            comment = "Temporarily doubles Hydrobot's movement speed.";
                        }
                        else if (powerPackPointedAt.powerType == PowerPackType.Strength)
                        {
                            line = "STRENGTH BOOST POWERPACK";
                            name = "Strength Boost Powerpack";
                            comment = "Temporarily doubles Hydrobot's power.";
                        }
                        else if (powerPackPointedAt.powerType == PowerPackType.FireRate)
                        {
                            line = "SHOOT RATE BOOST POWERPACK";
                            name = "Shoot Rate Boost Powerpack";
                            comment = "Temporarily doubles Hydrobot's shooting speed.";
                        }
                        else if (powerPackPointedAt.powerType == PowerPackType.Health)
                        {
                            line = "HEALTH BOOST POWERPACK";
                            name = "Health Boost Powerpack";
                            comment = "Replenishes Hydrobot's health.";
                        }
                        else if (powerPackPointedAt.powerType == PowerPackType.StrangeRock)
                        {
                            line = "STRANGE ROCK";
                            name = "Strange Rock";
                            comment = "A rock that exhibits abnormal characteristics. Can be dropped at Research Center for analysing.";
                        }
                        else if (powerPackPointedAt.powerType == PowerPackType.GoldenKey)
                        {
                            line = "GOLDEN KEY";
                            name = "Golden Key";
                            comment = "Can open any treasure chest.";
                        }
                        position = powerPackPointedAt.Position;
                        tip = "Press Z to collect";
                    }
                    else
                    {
                        Resource resourcePointedAt = null, botOnResource = null;
                        CursorManager.MouseOnWhichResource(cursor, gameCamera, resources, ref resourcePointedAt, ref botOnResource, null);
                        if (resourcePointedAt != null)
                        {
                            line = "RECYCLED RESOURCE BOX";
                            name = "Recycled Resource Box";
                            comment = "A box contains recycled resource produced by the processing plant. Recycled resources can be used to construct new facilities.";
                            tip = "Press Z to collect";
                            position = resourcePointedAt.Position;
                        }
                        else
                        {
                            TreasureChest chestPointedAt = CursorManager.MouseOnWhichChest(cursor, gameCamera, treasureChests);
                            if (chestPointedAt != null)
                            {
                                line = "TREASURE CHEST";
                                name = "Treasure Chest";
                                comment = "Contains valuables sunk with the ship hundreds years ago.";
                                tip = "Double click to open";
                                position = chestPointedAt.Position;
                            }
                            Trash trashPointedAt = null, botOnTrash = null;
                            CursorManager.MouseOnWhichTrash(cursor, gameCamera, trashes, ref trashPointedAt, ref botOnTrash, null);
                            if (trashPointedAt != null)
                            {
                                line = "";
                                comment = "";
                                if (trashPointedAt.trashType == TrashType.biodegradable)
                                {
                                    line += "BIODEGRADABLE WASTE";
                                    name = "Biodegradable Waste";
                                    comment = "Great source of renewable energy.";
                                    tip = "Press Z to collect";
                                }
                                else if (trashPointedAt.trashType == TrashType.plastic)
                                {
                                    line += "PLASTIC WASTE";
                                    name = "Plastic Waste";
                                    comment = "May take more than 500 years to decompose.";
                                    tip = "Press X to collect";
                                }
                                else
                                {
                                    line += "RADIOACTIVE WASTE";
                                    name = "Radioactive Waste";
                                    comment = "An invisible speck can cause cancer.";
                                    tip = "Press C to collect";
                                }
                                position = trashPointedAt.Position;
                            }
                            else
                            {
                                ShipWreck shipPointedAt = CursorManager.MouseOnWhichShipWreck(cursor, gameCamera, shipWrecks);
                                if (shipPointedAt != null)
                                {
                                    line = "";
                                    comment = "";
                                    line = "OLD SHIPWRECK";
                                    name = "Old Shipwreck";
                                    comment = "Sunk hundreds years ago.";
                                    tip = "Double click to enter";
                                    position = shipPointedAt.Position;
                                }
                                else
                                {
                                    Factory factoryPointedAt = CursorManager.MouseOnWhichFactory(cursor, gameCamera, factories);
                                    if (factoryPointedAt != null)
                                    {
                                        line = "";
                                        comment = "";
                                        if (factoryPointedAt.factoryType == FactoryType.biodegradable)
                                        {
                                            line += "BIODEGRADABLE WASTE PROCESSING PLANT";
                                            name = "Biodegradable Waste Processing Plant";
                                            comment = "Organic wastes can be dropped here for processing.";
                                        }
                                        else if (factoryPointedAt.factoryType == FactoryType.plastic)
                                        {
                                            line += "PLASTIC WASTE PROCESSING PLANT";
                                            name = "Plastic Waste Processing Plant";
                                            comment = "Plastic wastes can be dropped here for processing.";
                                        }
                                        else
                                        {
                                            line += "RADIOACTIVE WASTE PROCESSING PLANT";
                                            name = "Radioactive Waste Processing Plant";
                                            comment = "Radioactive wastes can be dropped here for processing.";
                                        }
                             
                                        position = factoryPointedAt.Position;
                                        tip = "Double click to drop collected wastes";
                                        tip2 = "Shift + Click to open control panel";
                                    }
                                    else
                                    {
                                        if (CursorManager.MouseOnResearchFacility(cursor, gameCamera, researchFacility))
                                        {
                                            line = "RESEARCH FACILITY";
                                            name = "Research Facility";
                                            position = researchFacility.Position;
                                            comment = "Researches on upgrading plants and Hydrobot, analysing abnormal objects and resurrecting extinct animals from DNA.";
                                            tip = "Double click to drop collected objects";
                                            tip2 = "Shift + Click to open control panel";
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //draw name right over obj pointed at
                    if (GameSettings.ShowLiveTip && name != "")
                    {
                        Vector3 screenPos = graphicsDevice.Viewport.Project(position - new Vector3(0, 0, 20), gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity);
                        Vector2 twoDPos;
                        twoDPos.X = screenPos.X;
                        twoDPos.Y = screenPos.Y;
                        spriteBatch.DrawString(IngamePresentation.fishTalkFont, name, twoDPos, Color.Gold, 0,
                            new Vector2(IngamePresentation.fishTalkFont.MeasureString(name).X / 2, IngamePresentation.fishTalkFont.MeasureString(name).Y / 2), IngamePresentation.textScaleFactor, SpriteEffects.None, 0);
                        if (tip != "")
                            spriteBatch.DrawString(IngamePresentation.fishTalkFont, tip, twoDPos + new Vector2(0, IngamePresentation.fishTalkFont.MeasureString(name).Y / 2 + 5 + IngamePresentation.fishTalkFont.MeasureString(tip).Y / 2) * IngamePresentation.textScaleFactor, Color.White, 0,
                                new Vector2(IngamePresentation.fishTalkFont.MeasureString(tip).X / 2, IngamePresentation.fishTalkFont.MeasureString(tip).Y / 2), IngamePresentation.textScaleFactor, SpriteEffects.None, 0);
                        if (tip2 != "")
                            spriteBatch.DrawString(IngamePresentation.fishTalkFont, tip2, twoDPos + new Vector2(0, IngamePresentation.fishTalkFont.MeasureString(name).Y / 2 + 5 + IngamePresentation.fishTalkFont.MeasureString(tip).Y + 5 + IngamePresentation.fishTalkFont.MeasureString(tip2).Y / 2) * IngamePresentation.textScaleFactor, Color.White, 0,
                                new Vector2(IngamePresentation.fishTalkFont.MeasureString(tip2).X / 2, IngamePresentation.fishTalkFont.MeasureString(tip2).Y / 2), IngamePresentation.textScaleFactor, SpriteEffects.None, 0);
                    }
                    if (line != "" && comment != "")
                    {
                        nonLivingObjWasPointedAt = true;
                        somethingPointedAt = true;
                        enemyWasPointedAt = fishWasPointedAt = false;
                    }
                    //else nonLivingObjWasPointedAt = false;
                    if (somethingPointedAt)
                    {
                        spriteBatch.DrawString(statsFont, line, new Vector2(game.Window.ClientBounds.Width / 2, 4 + statsFont.MeasureString(line).Y / 2 * textScaleFactor), Color.Yellow, 0, new Vector2(statsFont.MeasureString(line).X / 2, statsFont.MeasureString(line).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                        comment = wrapLine(comment, commentMaxLength, statsFont, textScaleFactor);
                        tip = wrapLine(tip, commentMaxLength, statsFont, textScaleFactor);
                        Vector2 commentPos = new Vector2(game.Window.ClientBounds.Width / 2, 4 + (statsFont.MeasureString(line).Y + lineSpacing + statsFont.MeasureString(comment).Y / 2) * textScaleFactor);
                        spriteBatch.DrawString(statsFont, comment, commentPos, Color.Red, 0, new Vector2(statsFont.MeasureString(comment).X / 2, statsFont.MeasureString(comment).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                        //Vector2 tipPos = commentPos + new Vector2(0, statsFont.MeasureString(comment).Y / 2 + lineSpacing + statsFont.MeasureString(tip).Y / 2) * textScaleFactor;
                        //spriteBatch.DrawString(statsFont, tip, tipPos, Color.LightCyan, 0, new Vector2(statsFont.MeasureString(tip).X / 2, statsFont.MeasureString(tip).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                        //if (tip2 != "")
                        //{
                        //    Vector2 tip2Pos = tipPos + new Vector2(0, statsFont.MeasureString(tip).Y / 2 + lineSpacing + statsFont.MeasureString(tip2).Y / 2) * textScaleFactor;
                        //    spriteBatch.DrawString(statsFont, tip2, tip2Pos, Color.LightCyan, 0, new Vector2(statsFont.MeasureString(tip2).X / 2, statsFont.MeasureString(tip2).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                        //}
                        prevLine = line;
                        prevComment = comment;
                        prevTip = tip;
                        prevTip2 = tip2;
                    }
                }
            }

            //if nothing is pointed at now, draw the old obj-pointed messages which fade through time
            if (!somethingPointedAt)
            {
                opaqueValue -= (float)(fadeStep * 20 * (PoseidonGame.playTime.TotalMilliseconds - lastFadeChange) / 1000);
                lastFadeChange = PoseidonGame.playTime.TotalMilliseconds;
                if (opaqueValue <= 0) opaqueValue = 0;
                if (nonLivingObjWasPointedAt)
                {
                    spriteBatch.DrawString(statsFont, prevLine, new Vector2(game.Window.ClientBounds.Width / 2, 4 + statsFont.MeasureString(prevLine).Y / 2 * textScaleFactor), Color.Yellow * opaqueValue, 0, new Vector2(statsFont.MeasureString(prevLine).X / 2, statsFont.MeasureString(prevLine).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                    //comment = wrapLine(comment, commentMaxLength, statsFont, textScaleFactor);
                    //tip = wrapLine(tip, commentMaxLength, statsFont, textScaleFactor);
                    Vector2 commentPos = new Vector2(game.Window.ClientBounds.Width / 2, 4 + (statsFont.MeasureString(prevLine).Y + lineSpacing + statsFont.MeasureString(prevComment).Y / 2) * textScaleFactor);
                    spriteBatch.DrawString(statsFont, prevComment, commentPos, Color.Red * opaqueValue, 0, new Vector2(statsFont.MeasureString(prevComment).X / 2, statsFont.MeasureString(prevComment).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                    //Vector2 tipPos = commentPos + new Vector2(0, statsFont.MeasureString(prevComment).Y / 2 + lineSpacing + statsFont.MeasureString(prevTip).Y / 2) * textScaleFactor;
                    //spriteBatch.DrawString(statsFont, prevTip, tipPos, Color.LightCyan * opaqueValue, 0, new Vector2(statsFont.MeasureString(prevTip).X / 2, statsFont.MeasureString(prevTip).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                    //if (prevTip2 != "")
                    //{
                    //    Vector2 tip2Pos = tipPos + new Vector2(0, statsFont.MeasureString(prevTip).Y / 2 + lineSpacing + statsFont.MeasureString(prevTip2).Y / 2) * textScaleFactor;
                    //    spriteBatch.DrawString(statsFont, prevTip2, tip2Pos, Color.LightCyan * opaqueValue, 0, new Vector2(statsFont.MeasureString(prevTip2).X / 2, statsFont.MeasureString(prevTip2).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                    //}
                }
                if (fishWasPointedAt)
                {
                    //IngamePresentation.DrawHealthBar(game, spriteBatch, statsFont, swimmingObjHealth, swimmingObjMaxHealth, 5, swimmingObjName, opaqueValue);
                    swimmingObjName = swimmingObjName.ToUpper();
                    spriteBatch.DrawString(statsFont, swimmingObjName, new Vector2(game.Window.ClientBounds.Width / 2 - statsFont.MeasureString(swimmingObjName).X / 2 * textScaleFactor, 5 - 1), Color.MediumVioletRed * opaqueValue,
                        0, Vector2.Zero, textScaleFactor, SpriteEffects.None, 0);
                    spriteBatch.DrawString(fishTalkFont, fishTalk, new Vector2(game.Window.ClientBounds.Width / 2, 4 + (fishTalkFont.MeasureString(swimmingObjName).Y + fishTalkFont.MeasureString(fishTalk).Y / 2 + lineSpacing) * textScaleFactor), Color.Yellow * opaqueValue, 0, new Vector2(fishTalkFont.MeasureString(fishTalk).X / 2, fishTalkFont.MeasureString(fishTalk).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                }
                //if (enemyWasPointedAt)
                //{
                //    IngamePresentation.DrawHealthBar(game, spriteBatch, statsFont, swimmingObjHealth, swimmingObjMaxHealth, 5, swimmingObjName, opaqueValue);
                //}
            }
            else
            {
                opaqueValue = startingOpaqueValue;
            }
        }

        public static void DrawObjectUnderStatus(SpriteBatch spriteBatch, Camera gameCamera, HydroBot hydroBot, GraphicsDevice graphicsDevice, List<Powerpack> powerPacks, List<Resource> resources, List<Trash> trashes, List<TreasureChest> chests, List<ShipWreck> shipWrecks, List<Factory> factories, ResearchFacility researchFacility)
        {
            if (!GameSettings.ShowLiveTip) return;
            //for highlighting obj under bot
            Powerpack powerPackPointedAt1 = null, botOnPowerPack1 = null;
            CursorManager.MouseOnWhichPowerPack(null, gameCamera, powerPacks, ref powerPackPointedAt1, ref botOnPowerPack1, hydroBot);
            string name = "", interaction = "", interaction2 = "";
            Vector2 twoDPos = Vector2.Zero;


            if (botOnPowerPack1 != null)
            {
                Vector3 screenPos = graphicsDevice.Viewport.Project(botOnPowerPack1.Position, gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity);
                twoDPos.X = screenPos.X;
                twoDPos.Y = screenPos.Y;
                if (botOnPowerPack1.powerType != PowerPackType.GoldenKey)
                    name = "Power Pack";
                else name = "Golden Key";
                interaction = "Press Z to collect";
                spriteBatch.DrawString(IngamePresentation.fishTalkFont, name, twoDPos, Color.Gold, 0,
                     new Vector2(IngamePresentation.fishTalkFont.MeasureString(name).X / 2, IngamePresentation.fishTalkFont.MeasureString(name).Y / 2), IngamePresentation.textScaleFactor, SpriteEffects.None, 0);
                spriteBatch.DrawString(IngamePresentation.fishTalkFont, interaction, twoDPos + new Vector2(0, IngamePresentation.fishTalkFont.MeasureString(name).Y / 2 + 5 + IngamePresentation.fishTalkFont.MeasureString(interaction).Y / 2) * IngamePresentation.textScaleFactor, Color.White, 0,
                        new Vector2(IngamePresentation.fishTalkFont.MeasureString(interaction).X / 2, IngamePresentation.fishTalkFont.MeasureString(interaction).Y / 2), IngamePresentation.textScaleFactor, SpriteEffects.None, 0);

            }
            else
            {
                Resource resourcePackPointedAt1 = null, botOnResource1 = null;
                CursorManager.MouseOnWhichResource(null, gameCamera, resources, ref resourcePackPointedAt1, ref botOnResource1, hydroBot);
                if (botOnResource1 != null)
                {
                    Vector3 screenPos = graphicsDevice.Viewport.Project(botOnResource1.Position, gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity);
                    twoDPos.X = screenPos.X;
                    twoDPos.Y = screenPos.Y;
                    name = "Recycled Resource Box";
                    interaction = "Press Z to collect";
                    spriteBatch.DrawString(IngamePresentation.fishTalkFont, name, twoDPos, Color.Gold, 0,
                         new Vector2(IngamePresentation.fishTalkFont.MeasureString(name).X / 2, IngamePresentation.fishTalkFont.MeasureString(name).Y / 2), IngamePresentation.textScaleFactor, SpriteEffects.None, 0);
                    spriteBatch.DrawString(IngamePresentation.fishTalkFont, interaction, twoDPos + new Vector2(0, IngamePresentation.fishTalkFont.MeasureString(name).Y / 2 + 5 + IngamePresentation.fishTalkFont.MeasureString(interaction).Y / 2) * IngamePresentation.textScaleFactor, Color.White, 0,
                            new Vector2(IngamePresentation.fishTalkFont.MeasureString(interaction).X / 2, IngamePresentation.fishTalkFont.MeasureString(interaction).Y / 2), IngamePresentation.textScaleFactor, SpriteEffects.None, 0);
                }
                else
                {
                    TreasureChest botOverChest = CursorManager.BotOverWhichChest(hydroBot, chests);
                    if (botOverChest != null)
                    {
                        Vector3 screenPos = graphicsDevice.Viewport.Project(botOverChest.Position, gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity);
                        twoDPos.X = screenPos.X;
                        twoDPos.Y = screenPos.Y;
                        name = "Treasure Chest";
                        interaction = "Double click to open";

                        spriteBatch.DrawString(IngamePresentation.fishTalkFont, name, twoDPos, Color.Gold, 0,
                            new Vector2(IngamePresentation.fishTalkFont.MeasureString(name).X / 2, IngamePresentation.fishTalkFont.MeasureString(name).Y / 2), IngamePresentation.textScaleFactor, SpriteEffects.None, 0);
                        spriteBatch.DrawString(IngamePresentation.fishTalkFont, interaction, twoDPos + new Vector2(0, IngamePresentation.fishTalkFont.MeasureString(name).Y / 2 + 5 + IngamePresentation.fishTalkFont.MeasureString(interaction).Y / 2) * IngamePresentation.textScaleFactor, Color.White, 0,
                                new Vector2(IngamePresentation.fishTalkFont.MeasureString(interaction).X / 2, IngamePresentation.fishTalkFont.MeasureString(interaction).Y / 2), IngamePresentation.textScaleFactor, SpriteEffects.None, 0);
                    }
                    Trash trashPointedAt1 = null, botOnTrash1 = null;
                    CursorManager.MouseOnWhichTrash(null, gameCamera, trashes, ref trashPointedAt1, ref botOnTrash1, hydroBot);
                    if (botOnTrash1 != null)
                    {
                        Vector3 screenPos = graphicsDevice.Viewport.Project(botOnTrash1.Position, gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity);
                        twoDPos.X = screenPos.X;
                        twoDPos.Y = screenPos.Y;
                        if (botOnTrash1.trashType == TrashType.biodegradable)
                        {
                            name = "Biodegradable Waste";
                            interaction = "Press Z to collect";
                        }
                        else if (botOnTrash1.trashType == TrashType.plastic)
                        {
                            name = "Plastic Waste";
                            interaction = "Press X to collect";
                        }
                        else if (botOnTrash1.trashType == TrashType.radioactive)
                        {
                            name = "Radioactive Waste";
                            interaction = "Press C to collect";
                        }
                        spriteBatch.DrawString(IngamePresentation.fishTalkFont, name, twoDPos, Color.Gold, 0,
                            new Vector2(IngamePresentation.fishTalkFont.MeasureString(name).X / 2, IngamePresentation.fishTalkFont.MeasureString(name).Y / 2), IngamePresentation.textScaleFactor, SpriteEffects.None, 0);
                        spriteBatch.DrawString(IngamePresentation.fishTalkFont, interaction, twoDPos + new Vector2(0, IngamePresentation.fishTalkFont.MeasureString(name).Y / 2 + 5 + IngamePresentation.fishTalkFont.MeasureString(interaction).Y / 2) * IngamePresentation.textScaleFactor, Color.White, 0,
                                new Vector2(IngamePresentation.fishTalkFont.MeasureString(interaction).X / 2, IngamePresentation.fishTalkFont.MeasureString(interaction).Y / 2), IngamePresentation.textScaleFactor, SpriteEffects.None, 0);
                    }
                    else
                    {
                        ShipWreck botOverShipWreck = CursorManager.BotOverWhichShipWreck(hydroBot, shipWrecks);
                        if (botOverShipWreck != null)
                        {
                            Vector3 screenPos = graphicsDevice.Viewport.Project(botOverShipWreck.Position, gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity);
                            twoDPos.X = screenPos.X;
                            twoDPos.Y = screenPos.Y;
                            name = "Old Shipwreck";
                            interaction = "Double click to enter";

                            spriteBatch.DrawString(IngamePresentation.fishTalkFont, name, twoDPos, Color.Gold, 0,
                                new Vector2(IngamePresentation.fishTalkFont.MeasureString(name).X / 2, IngamePresentation.fishTalkFont.MeasureString(name).Y / 2), IngamePresentation.textScaleFactor, SpriteEffects.None, 0);
                            spriteBatch.DrawString(IngamePresentation.fishTalkFont, interaction, twoDPos + new Vector2(0, IngamePresentation.fishTalkFont.MeasureString(name).Y / 2 + 5 + IngamePresentation.fishTalkFont.MeasureString(interaction).Y / 2) * IngamePresentation.textScaleFactor, Color.White, 0,
                                    new Vector2(IngamePresentation.fishTalkFont.MeasureString(interaction).X / 2, IngamePresentation.fishTalkFont.MeasureString(interaction).Y / 2), IngamePresentation.textScaleFactor, SpriteEffects.None, 0);
                        }
                        else
                        {
                            Factory botOverFactory = CursorManager.BotOverWhichFactory(hydroBot, factories);
                            if (botOverFactory != null)
                            {
                                if (botOverFactory.factoryType == FactoryType.biodegradable)
                                {
                                    name = "Biodegradable Waste Processing Plant";
                                }
                                else if (botOverFactory.factoryType == FactoryType.plastic)
                                {
                                    name = "Plastic Waste Processing Plant";
                                }
                                else
                                {
                                    name = "Radioactive Waste Processing Plant";
                                }
                                interaction = "Double click to drop collected wastes";
                                interaction2 = "Shift + Click to open control panel";
                                Vector3 screenPos = graphicsDevice.Viewport.Project(botOverFactory.Position - new Vector3(0, 0, 20), gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity);
                                twoDPos.X = screenPos.X;
                                twoDPos.Y = screenPos.Y;

                            }
                            if (CursorManager.BotOverResearchFacility(hydroBot, researchFacility))
                            {
                                name = "Research Facility";
                                interaction = "Double click to drop collected objects";
                                interaction2 = "Shift + Click to open control panel";
                                Vector3 screenPos = graphicsDevice.Viewport.Project(researchFacility.Position - new Vector3(0, 0, 20), gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity);
                                twoDPos.X = screenPos.X;
                                twoDPos.Y = screenPos.Y;
                            }
                            spriteBatch.DrawString(IngamePresentation.fishTalkFont, name, twoDPos, Color.Gold, 0,
                             new Vector2(IngamePresentation.fishTalkFont.MeasureString(name).X / 2, IngamePresentation.fishTalkFont.MeasureString(name).Y / 2), IngamePresentation.textScaleFactor, SpriteEffects.None, 0);
                            spriteBatch.DrawString(IngamePresentation.fishTalkFont, interaction, twoDPos + new Vector2(0, IngamePresentation.fishTalkFont.MeasureString(name).Y / 2 + 5 + IngamePresentation.fishTalkFont.MeasureString(interaction).Y / 2) * IngamePresentation.textScaleFactor, Color.White, 0,
                                    new Vector2(IngamePresentation.fishTalkFont.MeasureString(interaction).X / 2, IngamePresentation.fishTalkFont.MeasureString(interaction).Y / 2), IngamePresentation.textScaleFactor, SpriteEffects.None, 0);
                            spriteBatch.DrawString(IngamePresentation.fishTalkFont, interaction2, twoDPos + new Vector2(0, IngamePresentation.fishTalkFont.MeasureString(name).Y / 2 + 5 + IngamePresentation.fishTalkFont.MeasureString(interaction).Y + 5 + IngamePresentation.fishTalkFont.MeasureString(interaction2).Y/2) * IngamePresentation.textScaleFactor, Color.White, 0,
                                   new Vector2(IngamePresentation.fishTalkFont.MeasureString(interaction2).X / 2, IngamePresentation.fishTalkFont.MeasureString(interaction2).Y / 2), IngamePresentation.textScaleFactor, SpriteEffects.None, 0);
                        }
                    }
                }
            }
        }

        static Color levelObjectiveColor = Color.White;
        static double timeLastColorChange = 0;
        static float rotatingAngle = 0;
        public static float newTextScale = 5.0f, newTextStandardScale = 5.0f;
        //Draw level objective icon
        public static void DrawLevelObjectiveIcon(GraphicsDevice GraphicDevice, SpriteBatch spriteBatch)
        {
            if (levelObjHover) levelObjectiveIconTexture = levelObjectiveHoverIconTexture;
            else levelObjectiveIconTexture = levelObjectiveNormalIconTexture;

            int xOffsetText, yOffsetText;
            Rectangle rectSafeArea;

            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            xOffsetText = rectSafeArea.Right;
            yOffsetText = rectSafeArea.Top;
            int width = (int)(levelObjectiveIconTexture.Width * textScaleFactor * 0.8f);
            int height = (int)(levelObjectiveIconTexture.Height * textScaleFactor * 0.8f);

            levelObjectiveIconRectangle = new Rectangle(xOffsetText - width, yOffsetText, width, height);

            if ((HydroBot.gameMode == GameMode.MainGame || HydroBot.gameMode == GameMode.ShipWreck) && PlayGameScene.newLevelObjAvailable && PlayGameScene.timeElapsedFromLevelStart >= 3000 && !PoseidonGame.capturingGameTrailer)
            {
                if (PoseidonGame.playTime.TotalMilliseconds - timeLastColorChange >= 500)
                {
                    if (levelObjectiveColor == Color.White)
                        levelObjectiveColor = Color.Transparent;
                    else levelObjectiveColor = Color.White;
                    timeLastColorChange = PoseidonGame.playTime.TotalMilliseconds;
                    PoseidonGame.audio.loudBeep.Play();
                    //rotatingAngle += 0.1f;
                }
            }
            else
            {
                levelObjectiveColor = Color.White;
                rotatingAngle = 0;
            }
            //levelObjectiveColor = EffectHelpers.LerpColor(Color.White, Color.LawnGreen, (float)Math.Abs(Math.Sin(PoseidonGame.playTime.TotalSeconds * 2)));
            //if (PlayGameScene.newLevelObjAvailable && PlayGameScene.timeElapsedFromLevelStart >= 3000)
            //{
            //    newTextScale -= 0.1f;
            //    if (newTextScale <= 1.0f) newTextScale = 1.0f;      
            //}
            spriteBatch.Draw(levelObjectiveIconTexture, levelObjectiveIconRectangle, null, levelObjectiveColor, rotatingAngle, Vector2.Zero, SpriteEffects.None, 0);
            //if (PlayGameScene.newLevelObjAvailable)
            //{
            //    newTextScale -= 0.1f;
            //    if (newTextScale <= 1.0f) newTextScale = 1.0f;
            //    spriteBatch.DrawString(fishTalkFont, "NEW", new Vector2(levelObjectiveIconRectangle.Center.X, levelObjectiveIconRectangle.Center.Y), Color.LawnGreen, rotatingAngle, new Vector2(facilityFont.MeasureString("NEW").X / 2, facilityFont.MeasureString("NEW").Y / 2), newTextScale, SpriteEffects.None, 0);
            //}
            //if (PlayGameScene.newLevelObjAvailable && GameSettings.ShowLiveTip && PlayGameScene.timeElapsedFromLevelStart >= 3000)
            //{
            //    xOffsetText = levelObjectiveIconRectangle.X - arrowTexture.Width;
            //    yOffsetText = levelObjectiveIconRectangle.Center.Y - arrowTexture.Height / 2;

            //    Rectangle arrowRectangle = new Rectangle(xOffsetText, yOffsetText, arrowTexture.Width, arrowTexture.Height);

            //    spriteBatch.Draw(arrowTexture, arrowRectangle, Color.White);
            //}
        }

        //Draw level tip icon
        public static void DrawTipIcon(GraphicsDevice GraphicDevice, SpriteBatch spriteBatch)
        {
            if (tipHover) tipIconTexture = tipHoverIconTexture;
            else tipIconTexture = tipNormalIconTexture;
            int xOffsetText, yOffsetText;

            xOffsetText = (int)(levelObjectiveIconRectangle.Center.X - 75 * textScaleFactor / 2);
            yOffsetText = (int)(levelObjectiveIconRectangle.Bottom + 15);

            tipIconRectangle = new Rectangle(xOffsetText, yOffsetText, (int)(75 * textScaleFactor), (int)(75 * textScaleFactor));

            spriteBatch.Draw(tipIconTexture, tipIconRectangle, Color.White);

        }
        //Draw GamePlus level
        public static void DrawGamePlusLevel(SpriteBatch spriteBatch)
        {
            int xOffsetText, yOffsetText;
            string text = "Game + " + HydroBot.gamePlusLevel;

            xOffsetText = levelObjectiveIconRectangle.Right - (int)facilityFont.MeasureString(text).X - 20;
            yOffsetText = levelObjectiveIconRectangle.Bottom + 5;

            spriteBatch.DrawString(facilityFont, text, new Vector2(xOffsetText, yOffsetText), Color.Gold);
        }
        public static void DrawToNextLevelButton(SpriteBatch spriteBatch)
        {
            if (toNextLevelHover) toNextLevelTexture = IngamePresentation.toNextLevelHoverTexture;
            else toNextLevelTexture = IngamePresentation.toNextLevelNormalTexture;
            int xOffsetText, yOffsetText;

            xOffsetText = levelObjectiveIconRectangle.X - toNextLevelTexture.Width - 20;
            yOffsetText = levelObjectiveIconRectangle.Center.Y - toNextLevelTexture.Height / 2;

            toNextLevelIconRectangle = new Rectangle(xOffsetText, yOffsetText, toNextLevelTexture.Width, toNextLevelTexture.Height);

            spriteBatch.Draw(toNextLevelTexture, toNextLevelIconRectangle, Color.White);
        }

        //draw number of collected waste and resources
        public static void DrawCollectionStatus(GraphicsDevice GraphicDevice, SpriteBatch spriteBatch)
        {
            int iconSpacing = (int)(75 * textScaleFactor);
            int betweenTextAndIcon = (int)(10 * textScaleFactor);
            int iconWidth = (int)(32 * textScaleFactor);
            int iconHeight = (int)(32 * textScaleFactor);

            int middleScreenX = GraphicDevice.Viewport.TitleSafeArea.Center.X;
            int fromBottom = (int)(GraphicDevice.Viewport.TitleSafeArea.Bottom - iconWidth - 10 * textScaleFactor);
            string text = "";

            Rectangle radioRectangle = new Rectangle(middleScreenX - iconWidth, fromBottom, iconWidth, iconHeight);
            spriteBatch.Draw(radioIcon, radioRectangle, Color.White);
            text = HydroBot.nuclearTrash.ToString();
            spriteBatch.DrawString(fishTalkFont, text, new Vector2(radioRectangle.Right + betweenTextAndIcon + fishTalkFont.MeasureString(text).X / 2, radioRectangle.Center.Y), Color.White,
                0, new Vector2(fishTalkFont.MeasureString(text).X / 2, fishTalkFont.MeasureString(text).Y / 2), textScaleFactor, SpriteEffects.None, 0);

            Rectangle plasticRectangle = new Rectangle(radioRectangle.X - iconSpacing, fromBottom, iconWidth, iconHeight);
            spriteBatch.Draw(plasticIcon, plasticRectangle, Color.White);
            text = HydroBot.plasticTrash.ToString();
            spriteBatch.DrawString(fishTalkFont, text, new Vector2(plasticRectangle.Right + betweenTextAndIcon + fishTalkFont.MeasureString(text).X / 2, plasticRectangle.Center.Y), Color.White,
                0, new Vector2(fishTalkFont.MeasureString(text).X / 2, fishTalkFont.MeasureString(text).Y / 2), textScaleFactor, SpriteEffects.None, 0);

            Rectangle bioRectangle = new Rectangle(plasticRectangle.X - iconSpacing, fromBottom, iconWidth, iconHeight);
            spriteBatch.Draw(bioIcon, bioRectangle, Color.White);
            text = HydroBot.bioTrash.ToString();
            spriteBatch.DrawString(fishTalkFont, text, new Vector2(bioRectangle.Right + betweenTextAndIcon + fishTalkFont.MeasureString(text).X / 2, bioRectangle.Center.Y), Color.White,
                0, new Vector2(fishTalkFont.MeasureString(text).X / 2, fishTalkFont.MeasureString(text).Y / 2), textScaleFactor, SpriteEffects.None, 0);

            Rectangle resourceRectangle = new Rectangle(radioRectangle.X + iconSpacing, fromBottom, iconWidth, iconHeight);
            spriteBatch.Draw(resourceIcon, resourceRectangle, Color.White);
            text = HydroBot.numResources.ToString();
            spriteBatch.DrawString(fishTalkFont, text, new Vector2(resourceRectangle.Right + betweenTextAndIcon + fishTalkFont.MeasureString(text).X / 2, resourceRectangle.Center.Y), Color.White,
                0, new Vector2(fishTalkFont.MeasureString(text).X / 2, fishTalkFont.MeasureString(text).Y / 2), textScaleFactor, SpriteEffects.None, 0);

            Rectangle rockRectangle = new Rectangle(resourceRectangle.X + iconSpacing, fromBottom, iconWidth, iconHeight);
            spriteBatch.Draw(strangeRockIcon, rockRectangle, Color.White);
            text = HydroBot.numStrangeObjCollected.ToString();
            spriteBatch.DrawString(fishTalkFont, text, new Vector2(rockRectangle.Right + betweenTextAndIcon + fishTalkFont.MeasureString(text).X / 2, rockRectangle.Center.Y), Color.White,
                0, new Vector2(fishTalkFont.MeasureString(text).X / 2, fishTalkFont.MeasureString(text).Y / 2), textScaleFactor, SpriteEffects.None, 0);

        }

        public static void DrawHydroBotStatus(GraphicsDevice GraphicDevice, SpriteBatch spriteBatch)
        {
            int iconSpacing = (int)(150 * textScaleFactor);
            int betweenTextAndIcon = (int)(10 * textScaleFactor);
            int iconWidth = (int)(32 * textScaleFactor);
            int iconHeight = (int)(32 * textScaleFactor);

            int middleScreenX = GraphicDevice.Viewport.TitleSafeArea.Center.X;
            int fromBottom = (int)(GraphicDevice.Viewport.TitleSafeArea.Bottom - 10 * textScaleFactor - 2 * iconWidth - 10);
            string text = "";

            Rectangle energyRectangle = new Rectangle(middleScreenX - iconWidth, fromBottom, iconWidth, iconHeight);
            spriteBatch.Draw(energyIcon, energyRectangle, Color.White);
            double healthiness = HydroBot.currentEnergy / HydroBot.maxEnergy;
            Color healthColor = Color.LimeGreen;
            if (healthiness < 0.2)
                healthColor = Color.DarkRed;
            else if (healthiness < 0.5)
                healthColor = Color.Orange;
            text = ((int)((HydroBot.currentEnergy / HydroBot.maxEnergy) * 100)).ToString() + "%";
            spriteBatch.DrawString(fishTalkFont, text, new Vector2(energyRectangle.Right + betweenTextAndIcon + fishTalkFont.MeasureString(text).X / 2, energyRectangle.Center.Y), healthColor,
                0, new Vector2(fishTalkFont.MeasureString(text).X / 2, fishTalkFont.MeasureString(text).Y / 2), textScaleFactor, SpriteEffects.None, 0);

            Rectangle healthRectangle = new Rectangle(energyRectangle.X - iconSpacing, fromBottom, iconWidth, iconHeight);
            spriteBatch.Draw(botHealthIcon, healthRectangle, Color.White);
            healthiness = HydroBot.currentHitPoint / HydroBot.maxHitPoint;
            healthColor = Color.LimeGreen;
            if (healthiness < 0.2)
                healthColor = Color.DarkRed;
            else if (healthiness < 0.5)
                healthColor = Color.Orange;
            text = ((int)((HydroBot.currentHitPoint / HydroBot.maxHitPoint) * 100)).ToString() + "%";
            spriteBatch.DrawString(fishTalkFont, text, new Vector2(healthRectangle.Right + betweenTextAndIcon + fishTalkFont.MeasureString(text).X / 2, healthRectangle.Center.Y), healthColor,
                0, new Vector2(fishTalkFont.MeasureString(text).X / 2, fishTalkFont.MeasureString(text).Y / 2), textScaleFactor, SpriteEffects.None, 0);

            Rectangle experienceRectangle = new Rectangle(energyRectangle.X + iconSpacing, fromBottom, iconWidth, iconHeight);
            spriteBatch.Draw(experienceIcon, experienceRectangle, Color.White);
            text = ((int)(((float)HydroBot.currentExperiencePts / (float)HydroBot.nextLevelExperience) * 100)).ToString() + "%";
            spriteBatch.DrawString(fishTalkFont, text, new Vector2(experienceRectangle.Right + betweenTextAndIcon + fishTalkFont.MeasureString(text).X / 2, experienceRectangle.Center.Y), Color.White,
                0, new Vector2(fishTalkFont.MeasureString(text).X / 2, fishTalkFont.MeasureString(text).Y / 2), textScaleFactor, SpriteEffects.None, 0);
        }

        public static bool mouseOnLevelObjectiveIcon(MouseState lmouseState)
        {
            if (levelObjectiveIconRectangle.Contains(lmouseState.X, lmouseState.Y))
                return true;
            else
                return false;
        }

        public static bool mouseOnTipIcon(MouseState lmouseState)
        {
            if (tipIconRectangle.Contains(lmouseState.X, lmouseState.Y))
                return true;
            else
                return false;
        }
        public static bool mouseOnNextLevelIcon(MouseState lmouseState)
        {
            if (toNextLevelIconRectangle.Contains(lmouseState.X, lmouseState.Y))
                return true;
            else
                return false;
        }

        public static string wrapLine(string input_line, int width, SpriteFont font)
        {
            String line = String.Empty;
            String returnString = String.Empty;
            String[] wordArray = input_line.Split(' ');

            foreach (String word in wordArray)
            {
                if (font.MeasureString(line + word).Length() > width)
                {
                    returnString = returnString + line + '\n';
                    line = String.Empty;
                    line = line + word + ' ';
                }
                else if (word.Contains("\n"))
                {
                    returnString = returnString + line + word;
                    line = string.Empty;
                }
                else line = line + word + ' ';
            }
            return returnString + line;
        }

        public static string wrapLine(string input_line, int width, SpriteFont font, float scale)
        {
            String line = String.Empty;
            String returnString = String.Empty;
            String[] wordArray = input_line.Split(' ');

            foreach (String word in wordArray)
            {
                if (font.MeasureString(line + word).Length() * scale > width)
                {
                    returnString = returnString + line + '\n';
                    line = String.Empty;
                    line = line + word + ' ';
                }
                else if (word.Contains("\n"))
                {
                    returnString = returnString + line + word;
                    line = string.Empty;
                }
                else line = line + word + ' ';
            }
            return returnString + line;
        }
    }
}
