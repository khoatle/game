﻿// Copyright (C) Microsoft Corporation. All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Media;
using Poseidon.Core;
using Poseidon.MiniGames;
using System.IO;

namespace Poseidon
{
    public enum GameState { GameStart, PlayingPresentScene, DisplayMenu, NewGameStarted, PlayingOpeningCinematic, PlayingCutScene, IntroducingGame, Loading, Running, Won, Lost, WonButStaying, ToMiniGame, ToNextLevel, GameComplete, ToMainMenu }
    public enum GameMode { MainGame, ShipWreck, SurvivalMode };
    public enum TrashType { biodegradable, plastic, radioactive };
    public enum PowerPackType { Speed, Strength, FireRate, Health, StrangeRock, GoldenKey };
    public enum FactoryType { biodegradable, plastic, radioactive};
    public enum BuildingType { biodegradable, plastic, radioactive, researchlab }; // as a super type for factory-type (including researchlab)

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PoseidonGame : Microsoft.Xna.Framework.Game
    {
        public static GraphicsDeviceManager graphics;

        public static TimeSpan playTime = TimeSpan.Zero;

        KeyboardState lastKeyboardState = new KeyboardState();
        //KeyboardState currentKeyboardState = new KeyboardState();
        GamePadState lastGamePadState = new GamePadState();
        //GamePadState currentGamePadState = new GamePadState();
        MouseState currentMouseState = new MouseState();
        MouseState lastMouseState = new MouseState();

        public static SpriteBatch spriteBatch;
        SpriteFont statsFont;
        CutSceneDialog cutSceneDialog;
        // Textures for help scene
        protected Texture2D helpBackgroundTexture, helpForegroundTexture1, helpForegroundTexture2, helpForegroundTexture3, helpForegroundTexture4, helpForegroundTexture5, nextHelpButton;
        HelpScene helpScene;
        //Textures for the credit scene
        protected Texture2D creditBackgroundTexture, nextCreditButton;
        protected Texture2D[] creditForgroundTextures;
        CreditScene creditScene;
        protected GameScene activeScene;
        protected GameScene prevScene;
        // For the Start scene
        private SpriteFont smallFont, largeFont, startSceneSmall, startSceneLarge, typeFont;
        protected Texture2D startBackgroundTexture, startElementsTexture, teamLogo;
        StartScene startScene;
        ConfigScene configScene;
        LoadingScene loadingScene;
        SelectLoadingLevelScene selectLoadingLevelScene;
        // For the Attribute board
        AttributeBoard AttributeScene;
        protected Texture2D AttributeBackgroundTexture;
        // For the Level Objective
        LevelObjectiveScene levelObjectiveScene;
        protected Texture2D LevelObjectiveBackgroundTexture;
        //For the tip Scene
        TipScene tipScene;
        protected Texture2D tipBackgroundTexture;
        // For the mini games
        QuizzGameScene quizzGameScene;
        protected Texture2D quizzGameBackgroundTexture;
        TypingGameScene typeGameScene;
        JigsawGameScene jigsawGameScene;
        protected Texture2D typeGameBackgroundTexture;
        protected Texture2D boxBackground;
        // Audio Stuff
        public static AudioLibrary audio;
        PlayGameScene playGameScene;
        ShipWreckScene shipWreckScene;
        SurvivalGameScene survivalGameScene;
        // Game is paused?
        protected bool paused;
        protected Vector2 pausePosition;
        protected Rectangle pauseRect = new Rectangle(1, 120, 200, 44);
        protected Texture2D actionTexture;
        // The user pressed Enter or P?
        bool enterPressed;
        bool pPressed;
        bool backPressed;
        bool zPressed;
        bool AttributePressed;
        public static bool AttributeButtonPressed = false;
        bool EscPressed, TabPressed;
        bool doubleClicked = false;
        bool clicked=false;
        double clickTimer = 0;

        public string perfString = "";

        public static bool gamePlus = false;

        //jigsaw
        public static bool playJigsaw = false;
        public static int jigsawType; // 0-seacow, 1-turtle 2-dolphin

        // Texture to show that enemy is stunned
        protected Texture2D stunnedTexture;

        // Radar for the game
        Radar radar;

        //for continously playing random background musics
        Random rand = new Random();

        public static ContentManager contentManager;

        public static Video presentScene, cinematic;
        public static VideoPlayer videoPlayer;
        GameState gameState;

        public static int currentShipWreckID;

        //for displaying tips
        public static LiveTipManager liveTipManager;

        public static bool justCloseControlPanel = false;

        public bool threeDgraphicInitiated = false;

        public static bool DrawBoundingSphere = false, capturingCinematic = false, capturingGameTrailer = false;
        public PoseidonGame()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;//850;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;//700;

            if (capturingCinematic || capturingGameTrailer)
                graphics.IsFullScreen = false;
            else graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            videoPlayer = new VideoPlayer();
            gameState = GameState.GameStart;
            // Performance stuff
            //PerformanceHelper.InitializeWithGame(this);
            //graphics.SynchronizeWithVerticalRetrace = false;
            //this.IsFixedTimeStep = false;


            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            contentManager = Content;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), spriteBatch);

            loadGlobalData();

            setDifficulty(GameSettings.DifficultyLevel);
            
            //initiate all 2D graphics and fonts for the game
            IngamePresentation.Initiate2DGraphics(Content, this);

            statsFont = IngamePresentation.statsFont;

            //For pausing the game
            paused = false;
            pausePosition.X = (this.Window.ClientBounds.Width -
                pauseRect.Width) / 2;
            pausePosition.Y = (this.Window.ClientBounds.Height -
                pauseRect.Height) / 2;

            // Load Audio Elements
            audio = new AudioLibrary();
            audio.LoadContent(Content);
            Services.AddService(typeof(AudioLibrary), audio);

            //For general game control
            actionTexture = IngamePresentation.actionTexture;
            stunnedTexture = IngamePresentation.stunnedTexture;

            // Loading the radar
            //Vector2 radarCenter = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Right - GameConstants.RadarScreenRadius * GameConstants.generalTextScaleFactor, GraphicsDevice.Viewport.TitleSafeArea.Bottom - GameConstants.RadarScreenRadius * GameConstants.generalTextScaleFactor);
            radar = new Radar(Content, "Image/RadarTextures/playerDot", "Image/RadarTextures/enemyDot", "Image/RadarTextures/fishDot", "Image/RadarTextures/compass", "Image/RadarTextures/bossDot", this);

            //load the tips
            liveTipManager = new LiveTipManager();

            //For the Help scene
            helpBackgroundTexture = Content.Load<Texture2D>("Image/SceneTextures/startbackgroundNew");
            helpForegroundTexture1 = Content.Load<Texture2D>("Image/SceneTextures/helpforeground_move_1");
            helpForegroundTexture2 = Content.Load<Texture2D>("Image/SceneTextures/helpforeground_shoot_2");
            helpForegroundTexture3 = Content.Load<Texture2D>("Image/SceneTextures/helpforeground_trash_3");
            helpForegroundTexture4 = Content.Load<Texture2D>("Image/SceneTextures/helpforeground_skills_4");
            helpForegroundTexture5 = Content.Load<Texture2D>("Image/SceneTextures/helpforeground_otherKeys_5");
            nextHelpButton = Content.Load<Texture2D>("Image/ButtonTextures/nextHelpButton");
            SpriteFont menuSmall = IngamePresentation.menuSmall;
            helpScene = new HelpScene(this, helpBackgroundTexture, helpForegroundTexture1, helpForegroundTexture2, helpForegroundTexture3, helpForegroundTexture4, helpForegroundTexture5, nextHelpButton, spriteBatch, GraphicsDevice, menuSmall);
            Components.Add(helpScene);

            
            //For Credit scene
            creditBackgroundTexture = Content.Load<Texture2D>("Image/SceneTextures/startbackgroundNew");
            creditForgroundTextures = new Texture2D[3];
            creditForgroundTextures[0] = Content.Load<Texture2D>("Image/SceneTextures/Credits1");
            creditForgroundTextures[1] = Content.Load<Texture2D>("Image/SceneTextures/Credits2");
            creditForgroundTextures[2] = Content.Load<Texture2D>("Image/SceneTextures/Credits3");
            nextCreditButton = Content.Load<Texture2D>("Image/ButtonTextures/nextCreditButton");
            creditScene = new CreditScene(this, creditBackgroundTexture, creditForgroundTextures, nextCreditButton, spriteBatch, GraphicsDevice);
            Components.Add(creditScene);

            // Create the Start Scene
            startSceneSmall = Content.Load<SpriteFont>("Fonts/startScreenLarge");
            startSceneLarge = Content.Load<SpriteFont>("Fonts/startScreenLarge");
            smallFont = IngamePresentation.menuSmall;
            largeFont = IngamePresentation.largeFont;
            typeFont = IngamePresentation.typeFont;
            startBackgroundTexture = Content.Load<Texture2D>("Image/SceneTextures/startbackgroundNew");
            startElementsTexture = Content.Load<Texture2D>("Image/SceneTextures/startSceneElements");
            teamLogo = Content.Load<Texture2D>("Image/Miscellaneous/TeamLogo");
            startScene = new StartScene(this, startSceneSmall, startSceneLarge,
                startBackgroundTexture, startElementsTexture, teamLogo, GraphicsDevice);
            Components.Add(startScene);
            //SkillBackgroundTexture = Content.Load<Texture2D>("Image/skill_background");

            //Create the config screen
            Texture2D configTitle = Content.Load<Texture2D>("Image/SceneTextures/configTitle");
            Texture2D unselectedCheckBox = Content.Load<Texture2D>("Image/ButtonTextures/configUnselectedCheckBox");
            Texture2D selectedCheckBox = Content.Load<Texture2D>("Image/ButtonTextures/configSelectedCheckBox");
            Texture2D configslideBar = Content.Load<Texture2D>("Image/ButtonTextures/configBar");
            Texture2D configslideButton = Content.Load<Texture2D>("Image/ButtonTextures/configDragButton");
            Texture2D configOkButton = Content.Load<Texture2D>("Image/ButtonTextures/configOkButton");
            SpriteFont normalFont = Content.Load<SpriteFont>("Fonts/fishTalk");
            configScene = new ConfigScene(this, startSceneSmall, startSceneLarge, normalFont, startBackgroundTexture, configTitle, unselectedCheckBox, selectedCheckBox, configslideBar, configslideButton, configOkButton, GraphicsDevice);
            Components.Add(configScene);

            AttributeBackgroundTexture = Content.Load<Texture2D>("Image/AttributeBoardTextures/AttributeBackground");
            LevelObjectiveBackgroundTexture = Content.Load<Texture2D>("Image/SceneTextures/LevelObjectiveBackground");
            tipBackgroundTexture = LevelObjectiveBackgroundTexture;
            quizzGameBackgroundTexture = Content.Load<Texture2D>("Image/MinigameTextures/classroom1");
            typeGameBackgroundTexture = Content.Load<Texture2D>("Image/MinigameTextures/classroom2");
            boxBackground = Content.Load<Texture2D>("Image/MinigameTextures/whiteskin");

            //Loading the LOADING scene
            loadingScene = new LoadingScene(this, largeFont, startBackgroundTexture, teamLogo);
            Components.Add(loadingScene);

            //Loading the select loading level scene
            selectLoadingLevelScene = new SelectLoadingLevelScene(this, startSceneLarge, startBackgroundTexture, teamLogo, GraphicsDevice);
            Components.Add(selectLoadingLevelScene);

            // Create the shipwreck game play scene -- MUST be created before play game scene, as it overwrites the static attibutes of hydrobot
            shipWreckScene = new ShipWreckScene(this, graphics, Content, GraphicsDevice, spriteBatch, pausePosition, pauseRect, actionTexture, cutSceneDialog);
            Components.Add(shipWreckScene);

            // Create the Attribute board
            AttributeScene = new AttributeBoard(this, AttributeBackgroundTexture, Content);
            Components.Add(AttributeScene);

            presentScene = Content.Load<Video>("Videos/presentScene");
            cinematic = Content.Load<Video>("Videos/cinematicVer2");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        public void CheckClick(GameTime gameTime)
        {
            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            clickTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if(lastMouseState.LeftButton==ButtonState.Pressed 
                && currentMouseState.LeftButton == ButtonState.Released)
            {
                    
                    if (clicked && (clickTimer < GameConstants.clickTimerDelay))
                    {
                        doubleClicked = true;
                        clicked = false;
                    }
                    else
                    {
                        doubleClicked = false;
                        clicked = true;
                    }
                    clickTimer = 0;
             }
        }


        private void CheckKeyEntered()
        {
            // Get the Keyboard and GamePad state
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            enterPressed = (lastKeyboardState.IsKeyDown(Keys.Enter) &&
                (keyboardState.IsKeyUp(Keys.Enter)));
            //enterPressed |= (lastGamePadState.Buttons.A == ButtonState.Pressed) &&
            //          (gamepadState.Buttons.A == ButtonState.Released);
            pPressed = (lastKeyboardState.IsKeyDown(Keys.P) &&
                (keyboardState.IsKeyUp(Keys.P)));
            zPressed = (lastKeyboardState.IsKeyDown(Keys.Z) &&
                (keyboardState.IsKeyUp(Keys.Z)));
            backPressed = (lastKeyboardState.IsKeyDown(Keys.Escape) &&
                (keyboardState.IsKeyUp(Keys.Escape)));
            AttributePressed = (lastKeyboardState.IsKeyDown(Keys.I) &&
                (keyboardState.IsKeyUp(Keys.I)));
            EscPressed = (lastKeyboardState.IsKeyDown(Keys.Escape) &&
                (keyboardState.IsKeyUp(Keys.Escape)));
            TabPressed = (lastKeyboardState.IsKeyDown(Keys.Tab) &&
                (keyboardState.IsKeyUp(Keys.Tab)));
            lastKeyboardState = keyboardState;
            lastGamePadState = gamepadState;
            
        }
        /// <summary>
        /// Handle input of all game scenes
        /// </summary>
        private void HandleScenesInput(GameTime gameTime)
        {
            // Handle Start Scene Input
            if (activeScene == startScene)
            {
                HandleStartSceneInput();
            }
            else if (activeScene == configScene)
            {
                if (enterPressed || EscPressed || ConfigScene.okClicked)
                {
                    ConfigScene.okClicked = false;
                    ShowScene(startScene);
                }
            }
            else if (activeScene == selectLoadingLevelScene)
            {
                HandleSelectLoadingLevelSceneInput();
            }
            else if (activeScene == loadingScene)
            {
                if (loadingScene.loadingSceneStarted)
                {
                    loadingScene.loadingSceneStarted = false;
                    if (loadingScene.loadingSurvivalScene)
                    {
                        loadingScene.loadingSurvivalScene = false;
                        startScene.gameStarted = true;
                        CreateSurvivalDependentScenes();
                        SurvivalGameScene.score = 0;
                        ShowScene(survivalGameScene);
                    }
                    else
                    {
                        CreateLevelDependentScenes();
                        startScene.gameStarted = true;
                        startScene.Hide();
                        ShowScene(playGameScene);
                        if (PlayGameScene.currentLevel == 0)
                            PlayGameScene.currentGameState = GameState.NewGameStarted;
                    }
                }
            }
            // Handle Help Scene input
            else if (activeScene == helpScene)
            {
                if (enterPressed || EscPressed)
                {
                    ShowScene(startScene);
                }
            }
            else if (activeScene == creditScene)
            {
                if (enterPressed || EscPressed)
                {
                    creditScene.nextPressed = false;
                    ShowScene(startScene);
                }
            }
            // Handle Action Scene Input
            else if (activeScene == playGameScene)
            {
                HandleActionInput();
            }
            // Handle Attribute scene input
            else if (activeScene == AttributeScene)
            {
                HandleAttributeSceneInput();
            }
            // Handle ship wreck scene input
            else if (activeScene == shipWreckScene)
            {
                HandleShipWreckSceneInput();
            }
            else if (activeScene == levelObjectiveScene)
            {
                HandleLevelObjectiveInput();
            }
            else if (activeScene == tipScene)
            {
                HandleTipInput();
            }
            else if (activeScene == quizzGameScene)
            {
                HandleQuizzGameInput();
            }
            else if (activeScene == typeGameScene)
            {
                HandleTypeGameInput();
            }
            else if (activeScene == survivalGameScene)
            {
                HandleSurvivalInput();
            }
            else if (activeScene == jigsawGameScene)
            {
                HandleJigsawInput();
            }
        }

        /// <summary>
        /// Handle which level to load
        /// </summary>
        public void HandleSelectLoadingLevelSceneInput()
        {
            int i=0, lvl;
            if (TextMenuComponent.clicked)
            {
                TextMenuComponent.clicked = false;
                foreach( int level in selectLoadingLevelScene.savedlevels)
                {
                    if (selectLoadingLevelScene.SelectedMenuIndex == i)
                    {
                        if (level >= 100)
                        {
                            PoseidonGame.gamePlus = true;
                            lvl = level - 100;
                        }
                        else
                        {
                            PoseidonGame.gamePlus = false;
                            lvl = level;
                        }
                        PlayGameScene.currentLevel = lvl;
                        MediaPlayer.Stop();
                        ShowScene(loadingScene);
                    }
                    i++;
                }

                //go Back selected
                if (selectLoadingLevelScene.SelectedMenuIndex == i)
                    ShowScene(startScene);
            }
            else if (EscPressed)
                ShowScene(startScene);
        }

        public void HandleQuizzGameInput()
        {
            if (quizzGameScene.questionAnswered >= 4 || EscPressed)// || enterPressed)
            {
                //each right answer give 5% environment boost
                HydroBot.currentEnvPoint += quizzGameScene.numRightAnswer * GameConstants.envGainForCorrectQuizAnswer;
                if (HydroBot.currentEnvPoint >= HydroBot.maxEnvPoint) HydroBot.currentEnvPoint = HydroBot.maxEnvPoint;
                HydroBot.currentExperiencePts += (quizzGameScene.numRightAnswer + HydroBot.gamePlusLevel) * 50 ;
                HydroBot.IncreaseGoodWillPoint(GameConstants.BasicGoodWillGainForPlayingMiniGame * quizzGameScene.numRightAnswer);
                PlayGameScene.currentGameState = GameState.ToNextLevel;
                ShowScene(playGameScene);
            }
        }
        public void HandleTypeGameInput()
        {
            if (typeGameScene.isOver && enterPressed || EscPressed)
            {
                PlayGameScene.currentGameState = GameState.ToNextLevel;
                ShowScene(playGameScene);
            }
        }
        public void HandleJigsawInput()
        {
            if (jigsawGameScene.inOrder || jigsawGameScene.letAIHandle)
            {
                float efficiency = 1.0f;
                if (jigsawGameScene.letAIHandle) efficiency = 0.6f;
                MediaPlayer.Stop();
                jigsawGameScene.StopVideoPlayers();
                ShowScene(prevScene);
                switch (jigsawType)
                {
                    case 0:
                        if (!HydroBot.hasSeaCow)
                        {
                            HydroBot.hasSeaCow = true;
                            AddingObjects.placeMinion(Content, jigsawType, playGameScene.enemies, playGameScene.enemiesAmount, playGameScene.fish, ref playGameScene.fishAmount, playGameScene.hydroBot);
                        }
                        HydroBot.iconActivated[IngamePresentation.seaCowIcon] = true;
                        HydroBot.seaCowPower += efficiency;
                        HydroBot.numSeaCowPieces -= GameConstants.boneCountForSeaCowJigsaw;
                        ResearchFacility.playSeaCowJigsaw = false;
                        ResearchFacility.seaCowWon = true;
                        break;
                    case 1:
                        if (!HydroBot.hasTurtle)
                        {
                            HydroBot.hasTurtle = true;
                            AddingObjects.placeMinion(Content, jigsawType, playGameScene.enemies, playGameScene.enemiesAmount, playGameScene.fish, ref playGameScene.fishAmount, playGameScene.hydroBot);
                        }
                        HydroBot.iconActivated[IngamePresentation.turtleIcon] = true;
                        HydroBot.turtlePower += efficiency;
                        HydroBot.numTurtlePieces -= GameConstants.boneCountForTurtleJigsaw;
                        ResearchFacility.playTurtleJigsaw = false;
                        ResearchFacility.turtleWon = true;
                        break;
                    case 2:
                        if (!HydroBot.hasDolphin)
                        {
                            HydroBot.hasDolphin = true;
                            AddingObjects.placeMinion(Content, jigsawType, playGameScene.enemies, playGameScene.enemiesAmount, playGameScene.fish, ref playGameScene.fishAmount, playGameScene.hydroBot);
                        }
                        HydroBot.iconActivated[IngamePresentation.dolphinIcon] = true;
                        HydroBot.dolphinPower += efficiency;
                        HydroBot.numDolphinPieces -= GameConstants.boneCountForDolphinJigsaw;
                        ResearchFacility.playDolphinJigsaw = false;
                        ResearchFacility.dolphinWon = true;
                        break;
                }
            }
            if (jigsawGameScene.timeUp || (EscPressed && jigsawGameScene.gamePlayed))
            {
                MediaPlayer.Stop();
                jigsawGameScene.StopVideoPlayers();
                ShowScene(prevScene);
                switch (jigsawType)
                {
                    case 0:
                        HydroBot.numSeaCowPieces -= GameConstants.boneCountForSeaCowJigsaw;
                        ResearchFacility.playSeaCowJigsaw = false;
                        ResearchFacility.seaCowLost = true;
                        break;
                    case 1:
                        HydroBot.numTurtlePieces -= GameConstants.boneCountForTurtleJigsaw;
                        ResearchFacility.playTurtleJigsaw = false;
                        ResearchFacility.turtleLost = true;
                        break;
                    case 2:
                        HydroBot.numDolphinPieces -= GameConstants.boneCountForDolphinJigsaw;
                        ResearchFacility.playDolphinJigsaw = false;
                        ResearchFacility.dolphinLost = true;
                        break;
                }
            }
            if (EscPressed)
            {
                MediaPlayer.Stop();
                jigsawGameScene.StopVideoPlayers();
                ShowScene(prevScene);
                EscPressed = false;
            }
        }
        /// <summary>
        /// Handle update for the ship wreck scene
        /// </summary>
        public void HandleShipWreckSceneInput()
        {
            // if dead or timeout inside the ship wreck
            // let the main game dictate about win/lose
            if (shipWreckScene.returnToMain)
            {
                //playGameScene.tank.CopyAttribute(shipWreckScene.tank);
                playGameScene.roundTimer = shipWreckScene.roundTimer;
                ShipWreckScene.gameCamera.shaking = false;
                ShowScene(playGameScene);
            }

            // User pauses the game
            if (pPressed)
            {
                audio.MenuBack.Play();
                shipWreckScene.Paused = !shipWreckScene.Paused;
            }
            if (backPressed && !shipWreckScene.foundRelic[currentShipWreckID])
            {
                //playGameScene.tank.CopyAttribute(shipWreckScene.tank);
                playGameScene.roundTimer = shipWreckScene.roundTimer;
                playGameScene.Scene2Texture = shipWreckScene.cutSceneImmediateRenderTarget;
                playGameScene.screenTransitNow = true;
                playGameScene.graphicEffect.resetTransitTimer();
                ShipWreckScene.gameCamera.shaking = false;
                ShowScene(playGameScene);
                HydroBot.isCastingSkill = false;
                doubleClicked = false;
            }
            if ((lastMouseState.LeftButton == ButtonState.Pressed
                && currentMouseState.LeftButton == ButtonState.Released
                && IngamePresentation.mouseOnLevelObjectiveIcon(currentMouseState)) ||
                TabPressed)
            {
                prevScene = shipWreckScene;
                PlayGameScene.newLevelObjAvailable = false;
                ShowScene(levelObjectiveScene);
            }
            if (lastMouseState.LeftButton == ButtonState.Pressed
                && currentMouseState.LeftButton == ButtonState.Released
                && IngamePresentation.mouseOnTipIcon(currentMouseState))
            {
                prevScene = shipWreckScene;
                ShowScene(tipScene);
            }
            else
            {
                doubleClicked = false;
            }
 
            //if (AttributeButtonPressed)// || AttributePressed)
            //{
            //    prevScene = shipWreckScene;
            //    ShowScene(AttributeScene);
            //    AttributeButtonPressed = false;
            //}
        }
        /// <summary>
        /// Handle update for the main game
        /// </summary>
        private void HandleActionInput()
        {

            // User pauses the game
            if (pPressed && !(PlayGameScene.currentGameState == GameState.PlayingOpeningCinematic))
            {
                audio.MenuBack.Play();
                playGameScene.Paused = !playGameScene.Paused;
                ResearchFacility.buildingSoundInstance.Pause();
                Factory.buildingSoundInstance.Pause();
            }
            if (backPressed)
            {
                if (PlayGameScene.currentGameState == GameState.PlayingOpeningCinematic)
                {
                    videoPlayer.Stop();
                    PlayGameScene.currentGameState = GameState.PlayingCutScene;
                }
                else if (PlayGameScene.currentGameState == GameState.PlayingCutScene)
                {
                    if (PlayGameScene.currentLevel == 0)
                        PlayGameScene.currentGameState = GameState.IntroducingGame;
                    else if (PlayGameScene.currentLevel == 12)
                    {
                        PlayGameScene.currentGameState = GameState.GameComplete;
                        HydroBot.gamePlusLevel++;
                        //the save file used for Survival mode
                        ObjectsToSerialize objectsToSerialize = new ObjectsToSerialize();
                        objectsToSerialize.hydrobot = playGameScene.hydroBot;
                        Serializer serializer = new Serializer();
                        serializer.SerializeObjects("SurvivalMode", objectsToSerialize);
                    }
                    else PlayGameScene.currentGameState = GameState.Running;
                }
                else
                {
                    MediaPlayer.Stop();
                    ShowScene(startScene);
                }
            }
            if (AttributeButtonPressed)// || AttributePressed)
            {
                prevScene = playGameScene;
                ShowScene(AttributeScene);
                AttributeButtonPressed = false;
            }
            if (doubleClicked 
                && !CursorManager.MouseOnEnemy(playGameScene.cursor, PlayGameScene.gameCamera, playGameScene.enemies, playGameScene.enemiesAmount)
                && !CursorManager.MouseOnFish(playGameScene.cursor, PlayGameScene.gameCamera, playGameScene.fish, playGameScene.fishAmount)
                && GetInShipWreck() && !playGameScene.factoryButtonPanel.hasAnyAnchor() && !playGameScene.openFactoryConfigurationScene && !playGameScene.openResearchFacilityConfigScene
                && !HydroBot.isCastingSkill)
            {
                //disable camera shaking or else we will get a shake 
                //right after getting out of a shipwreck
                PlayGameScene.gameCamera.shaking = false;
                //shipWreckScene.tank.CopyAttribute(playGameScene.tank);
                shipWreckScene.roundTimer = playGameScene.roundTimer;
                shipWreckScene.Scene2Texture = playGameScene.cutSceneImmediateRenderTarget;
                
                //shipWreckScene.Load();
                ShowScene(shipWreckScene);
                doubleClicked = false;
            }
            if ((lastMouseState.LeftButton == ButtonState.Pressed
                && currentMouseState.LeftButton == ButtonState.Released
                && IngamePresentation.mouseOnLevelObjectiveIcon(currentMouseState)) ||
                TabPressed)
            {
                prevScene = playGameScene;
                PlayGameScene.newLevelObjAvailable = false;
                IngamePresentation.newTextScale = IngamePresentation.newTextStandardScale;
                ShowScene(levelObjectiveScene);
            }
            if (lastMouseState.LeftButton == ButtonState.Pressed
                && currentMouseState.LeftButton == ButtonState.Released
                && IngamePresentation.mouseOnTipIcon(currentMouseState))
            {
                prevScene = playGameScene;
                ShowScene(tipScene);
            }
            else
            {
                doubleClicked = false;
            }
            if (PlayGameScene.currentGameState == GameState.ToMiniGame)
            {
                Random rand = new Random();
                if (rand.Next(2) == 0)
                    ShowScene(quizzGameScene);
                else
                    ShowScene(typeGameScene);
            }
            if (PlayGameScene.currentGameState == GameState.GameComplete)
            {
                MediaPlayer.Stop();
                ShowScene(startScene);
            }
            if (playJigsaw)
            {
                prevScene = playGameScene;
                playJigsaw = false;
                jigsawGameScene.setImageType(jigsawType);
                ShowScene(jigsawGameScene);
            }
        }
        public bool GetInShipWreck()
        {

            for (int curWreck = 0; curWreck < playGameScene.shipWrecks.Count; curWreck++)
            {
                if (CursorManager.MouseOnObject(playGameScene.cursor, playGameScene.shipWrecks[curWreck].BoundingSphere, playGameScene.shipWrecks[curWreck].Position, PlayGameScene.gameCamera)
                    && playGameScene.CharacterNearShipWreck(playGameScene.shipWrecks[curWreck].BoundingSphere)
                    )
                {            
                    // put the skill into one of the chest if skillID != 0
                    currentShipWreckID = shipWreckScene.currentShipWreckID = curWreck;
                    shipWreckScene.skillID = playGameScene.shipWrecks[curWreck].skillID;
                    //currentShipWreckID = curWreck;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Handle update for the survival mode game
        /// </summary>
        private void HandleSurvivalInput()
        {

            // User pauses the game
            if (pPressed)
            {
                audio.MenuBack.Play();
                survivalGameScene.Paused = !survivalGameScene.Paused;
                ResearchFacility.buildingSoundInstance.Pause();
                Factory.buildingSoundInstance.Pause();
            }
            if (backPressed)
            {
                MediaPlayer.Stop();
                ShowScene(startScene);
            }
            if (AttributeButtonPressed)// || AttributePressed)
            {
                prevScene = survivalGameScene;
                ShowScene(AttributeScene);
                AttributeButtonPressed = false;
            }
            if ((lastMouseState.LeftButton == ButtonState.Pressed
                && currentMouseState.LeftButton == ButtonState.Released
                && IngamePresentation.mouseOnLevelObjectiveIcon(currentMouseState)) ||
                TabPressed)
            {
                prevScene = survivalGameScene;
                ShowScene(levelObjectiveScene);
            }
            else
            {
                doubleClicked = false;
            }
            if (SurvivalGameScene.currentGameState == GameState.ToMainMenu)
            {
                MediaPlayer.Stop();
                ShowScene(startScene);
            }
            if (playJigsaw)
            {
                prevScene = survivalGameScene;
                playJigsaw = false;
                jigsawGameScene.setImageType(jigsawType);
                ShowScene(jigsawGameScene);
            }
        }
        /// <summary>
        /// Handle buttons and keyboard in StartScene
        /// </summary>
        private void HandleStartSceneInput()
        {
            if (TextMenuComponent.clicked)
            {
                TextMenuComponent.clicked = false;
                audio.MenuSelect.Play();

                int selectedMenuIndex = startScene.SelectedMenuIndex;

                if (selectedMenuIndex != -1)
                {
                    switch (startScene.menuItems[startScene.SelectedMenuIndex])
                    {
                        case "New Game":
                            MediaPlayer.Stop();
                            gamePlus = false;
                            HydroBot.gamePlusLevel = 0;
                            PlayGameScene.currentLevel = 0;
                            PlayGameScene.currentGameState = GameState.PlayingCutScene;
                            ShowScene(loadingScene);
                            break;
                        case "New Game Plus":
                            MediaPlayer.Stop();
                            gamePlus = true;
                            PlayGameScene.currentLevel = 0;
                            PlayGameScene.currentGameState = GameState.PlayingCutScene;
                            ShowScene(loadingScene);
                            break;
                        case "Resume Game":
                            MediaPlayer.Stop();
                            if (HydroBot.gameMode == GameMode.MainGame)
                                ShowScene(playGameScene);
                            else if (HydroBot.gameMode == GameMode.SurvivalMode)
                                ShowScene(survivalGameScene);
                            break;
                        case "Load Saved Level":
                            //MediaPlayer.Stop();
                            ShowScene(selectLoadingLevelScene);
                            break;
                        case "Guardian Mode":
                            MediaPlayer.Stop();
                            gamePlus = false;
                            HydroBot.gamePlusLevel = 0;
                            loadingScene.loadingSurvivalScene = true;
                            ShowScene(loadingScene);
                            break;
                        case "Config":
                            ShowScene(configScene);
                            break;
                        case "Help":
                            ShowScene(helpScene);
                            break;
                        case "Credits":
                            ShowScene(creditScene);
                            break;
                        case "Quit":
                            MediaPlayer.Stop();
                            saveGlobalData();
                            Exit();
                            break;

                    }
                }
            }
        }


        private void saveGlobalData()
        {
            BinaryWriter bw = new BinaryWriter(File.Open("globalData", FileMode.Create));
            bw.Write((double)GameSettings.MusicVolume);
            bw.Write((double)GameSettings.SoundVolume);
            bw.Write(GameSettings.ShowLiveTip);
            bw.Write(GameSettings.SpecialEffectsEnabled);
            bw.Write((double)GameSettings.NumParticleLevel);
            bw.Write((double)GameSettings.SchoolOfFishDetail);
            bw.Write(HydroBot.skillComboActivated);
            bw.Write((double)SurvivalGameScene.highestScore);
            bw.Write((Int16)GameSettings.DifficultyLevel);
            bw.Close();
        }


        private void loadGlobalData()
        {
            if (File.Exists("globalData"))
            {
                try
                {
                    BinaryReader br = new BinaryReader(File.Open("globalData", FileMode.Open));
                    GameSettings.MusicVolume = (float)br.ReadDouble();
                    MediaPlayer.Volume = GameSettings.MusicVolume;
                    GameSettings.SoundVolume = (float)br.ReadDouble();
                    SoundEffect.MasterVolume = GameSettings.SoundVolume;
                    videoPlayer.Volume = GameSettings.SoundVolume;
                    GameSettings.ShowLiveTip = br.ReadBoolean();
                    GameSettings.SpecialEffectsEnabled = br.ReadBoolean();
                    GameSettings.NumParticleLevel = (float)br.ReadDouble();
                    GameConstants.numExplosionParticles = (int) (GameConstants.DefaultNumExplosionParticles * GameSettings.NumParticleLevel) ;
                    GameConstants.numExplosionSmallParticles = (int)(GameConstants.DefaultNumExplosionSmallParticles * GameSettings.NumParticleLevel);
                    GameConstants.numSandParticles = (int)(GameConstants.DefaultNumSandParticles * GameSettings.NumParticleLevel);
                    GameConstants.numSandParticlesForFactory = (int)(GameConstants.DefaultNumSandParticlesForFactory * GameSettings.NumParticleLevel);
                    GameConstants.trailParticlesPerSecond = (int)(GameConstants.DefaultTrailParticlesPerSecond * GameSettings.NumParticleLevel);
                    GameConstants.numFrozenBreathParticlesPerUpdate = (int)(GameConstants.DefaultNumFrozenBreathParticlesPerUpdate * GameSettings.NumParticleLevel);
                    GameSettings.SchoolOfFishDetail = (float)br.ReadDouble();
                    HydroBot.skillComboActivated = HydroBot.lsSkillComboActivated = br.ReadBoolean();
                    SurvivalGameScene.highestScore = (float)br.ReadDouble();
                    GameSettings.DifficultyLevel = br.ReadInt16();
                    br.Close();
                }
                catch
                {
                    setDefaultGlobalData();
                }
            }
            else
            {
                setDefaultGlobalData();
            }
        }

        private void setDefaultGlobalData()
        {
            GameSettings.MusicVolume = 1f;
            GameSettings.SoundVolume = 1f;
            GameSettings.ShowLiveTip = true;
            GameSettings.SpecialEffectsEnabled = true;
            GameSettings.NumParticleLevel = 1f;
            GameSettings.SchoolOfFishDetail = 1f;
            HydroBot.skillComboActivated = HydroBot.lsSkillComboActivated = false;
            SurvivalGameScene.highestScore = 0f;
            GameSettings.DifficultyLevel = 2;
        }

        public static void setDifficulty(int difficultyLevel)
        {
           
            switch (difficultyLevel)
            {
                case 1:
                    TimeSpan[] roundTimeEasy = {TimeSpan.FromSeconds(530 + 120), TimeSpan.FromSeconds(240), TimeSpan.FromSeconds(490 + 120), 
                                                       TimeSpan.FromSeconds(220 + 120), TimeSpan.FromSeconds(120), TimeSpan.FromSeconds(600 + 180),
                                                       TimeSpan.FromSeconds(570 + 180), TimeSpan.FromSeconds(570 + 180), TimeSpan.FromSeconds(570 + 180),
                                                       TimeSpan.FromSeconds(530 + 180), TimeSpan.FromSeconds(220 + 180), TimeSpan.FromSeconds(220 + 180)};
                    GameConstants.RoundTime = roundTimeEasy;
                    break;
                case 2:
                    TimeSpan[] roundTimeMedium = {TimeSpan.FromSeconds(530 + 120), TimeSpan.FromSeconds(240 + 120), TimeSpan.FromSeconds(450 + 120), 
                                                       TimeSpan.FromSeconds(180 + 120), TimeSpan.FromSeconds(140), TimeSpan.FromSeconds(530 + 180),
                                                       TimeSpan.FromSeconds(530 + 180), TimeSpan.FromSeconds(530 + 180), TimeSpan.FromSeconds(530 + 180),
                                                       TimeSpan.FromSeconds(570 + 180), TimeSpan.FromSeconds(180 + 180), TimeSpan.FromSeconds(180 + 180)};
                     GameConstants.RoundTime = roundTimeMedium;
                    break;
                case 3:
                    TimeSpan[] roundTimeHard = {TimeSpan.FromSeconds(530), TimeSpan.FromSeconds(280), TimeSpan.FromSeconds(410 + 120), 
                                                       TimeSpan.FromSeconds(140 + 120), TimeSpan.FromSeconds(160), TimeSpan.FromSeconds(500 + 180),
                                                       TimeSpan.FromSeconds(500 + 180), TimeSpan.FromSeconds(500 + 180), TimeSpan.FromSeconds(500 + 180),
                                                       TimeSpan.FromSeconds(600 + 180), TimeSpan.FromSeconds(150 + 180), TimeSpan.FromSeconds(150 + 180)};
                     GameConstants.RoundTime = roundTimeHard;
                    break;
                default:
                    TimeSpan[] roundTimeDefault = {TimeSpan.FromSeconds(530), TimeSpan.FromSeconds(180), TimeSpan.FromSeconds(450), 
                                                       TimeSpan.FromSeconds(180), TimeSpan.FromSeconds(120), TimeSpan.FromSeconds(530),
                                                       TimeSpan.FromSeconds(530), TimeSpan.FromSeconds(530), TimeSpan.FromSeconds(530),
                                                       TimeSpan.FromSeconds(570), TimeSpan.FromSeconds(180), TimeSpan.FromSeconds(180)};
                     GameConstants.RoundTime = roundTimeDefault; //same as medium
                    break;
            }
        }

        private void CreateLevelDependentScenes()
        {

            //Set Level Objective. To make it scale with GamePlusLevel, we must put it in playgamescene after the hydrobot is loaded,
            //However, The cutscene uses the levelobjective values, and the cut scene must be created before playgamescene as it is 
            //used in playgamescene. Hence levelObj is initialized here , & can not use the gameplusLevel.
            if (gamePlus)
            {
                double[] levelObjective = { 0.95, 0.65, 0, 0, 0.3, 0, 0, 0, 0, 0, 0, 0 };
                GameConstants.LevelObjective = levelObjective;
            }
            else
            {
                double[] levelObjective = { 0.8, 0.75, 0, 0, 0.5, 0, 0, 0, 0, 0, 0, 0 };
                GameConstants.LevelObjective = levelObjective;
            }

            // Loading the cutscenes
            if (cutSceneDialog == null)
                cutSceneDialog = new CutSceneDialog();

            // Create the main game play scene
            playGameScene = new PlayGameScene(this, graphics, Content, GraphicsDevice, spriteBatch, pausePosition, pauseRect, actionTexture, cutSceneDialog, radar);
            Components.Add(playGameScene);

            // Create level objective scene
            //if (levelObjectiveScene == null)
            //{
                levelObjectiveScene = new LevelObjectiveScene(this, LevelObjectiveBackgroundTexture, Content, playGameScene);
                Components.Add(levelObjectiveScene);
            //}

            // Create tip scene
            if (tipScene == null)
            {
                tipScene = new TipScene(this, tipBackgroundTexture, Content);
                Components.Add(tipScene);
            }

            // Create minigame scenes
            if (quizzGameScene == null)
            {
                quizzGameScene = new QuizzGameScene(this, quizzGameBackgroundTexture, Content, GraphicsDevice);
                Components.Add(quizzGameScene);
            }
            if (typeGameScene == null)
            {
                typeGameScene = new TypingGameScene(this, typeFont, boxBackground, typeGameBackgroundTexture, Content);
                Components.Add(typeGameScene);
            }
            if (jigsawGameScene == null)
            {
                jigsawGameScene = new JigsawGameScene(this, Content, graphics, GraphicsDevice);
                Components.Add(jigsawGameScene);
            }

        }
        private void CreateSurvivalDependentScenes()
        {
            // Create level objective scene
            //if (levelObjectiveScene == null)
            //{
                levelObjectiveScene = new LevelObjectiveScene(this, LevelObjectiveBackgroundTexture, Content, playGameScene);
                Components.Add(levelObjectiveScene);
            //}

            //if (jigsawGameScene == null)
            //{
                jigsawGameScene = new JigsawGameScene(this, Content, graphics, GraphicsDevice);
                Components.Add(jigsawGameScene);
            //}

            // Create the survival game play scene
            survivalGameScene = new SurvivalGameScene(this, graphics, Content, GraphicsDevice, spriteBatch, pausePosition, pauseRect, actionTexture, cutSceneDialog, radar, stunnedTexture);
            Components.Add(survivalGameScene);

        }
        /// <summary>
        /// Handle buttons and keyboard in Attribute Scene
        /// </summary>
        private void HandleAttributeSceneInput()
        {
            if (AttributeScene.speedIconRectangle.Intersects(new Rectangle(lastMouseState.X, lastMouseState.Y, 1, 1))
                && lastMouseState.LeftButton == ButtonState.Pressed
                && currentMouseState.LeftButton == ButtonState.Released)
            {
                if (HydroBot.unassignedPts >= GameConstants.gainAttributeCost)
                {
                    audio.MenuSelect.Play();
                    HydroBot.speed += GameConstants.gainSpeed;
                    HydroBot.unassignedPts -= GameConstants.gainAttributeCost;
                }
            }
            if (AttributeScene.hitpointIconRectangle.Intersects(new Rectangle(lastMouseState.X, lastMouseState.Y, 1, 1))
                && lastMouseState.LeftButton == ButtonState.Pressed
                && currentMouseState.LeftButton == ButtonState.Released)
            {
                if (HydroBot.unassignedPts >= GameConstants.gainAttributeCost)
                {
                    audio.MenuSelect.Play();
                    HydroBot.currentHitPoint += GameConstants.gainHitPoint;
                    HydroBot.maxHitPoint += GameConstants.gainHitPoint;
                    HydroBot.currentEnergy += GameConstants.EnergyGainPerUpgrade;
                    HydroBot.maxEnergy += GameConstants.EnergyGainPerUpgrade;
                    HydroBot.unassignedPts -= GameConstants.gainAttributeCost;
                }
            }
            if (AttributeScene.shootrateIconRectangle.Intersects(new Rectangle(lastMouseState.X, lastMouseState.Y, 1, 1))
                && lastMouseState.LeftButton == ButtonState.Pressed
                && currentMouseState.LeftButton == ButtonState.Released)
            {
                if (HydroBot.unassignedPts >= GameConstants.gainAttributeCost)
                {
                    audio.MenuSelect.Play();
                    HydroBot.shootingRate += GameConstants.gainShootingRate;
                    HydroBot.unassignedPts -= GameConstants.gainAttributeCost;
                }
            }
            if (AttributeScene.bulletStrengthIconRectangle.Intersects(new Rectangle(lastMouseState.X, lastMouseState.Y, 1, 1))
                && lastMouseState.LeftButton == ButtonState.Pressed
                && currentMouseState.LeftButton == ButtonState.Released)
            {

                if (HydroBot.unassignedPts >= GameConstants.gainAttributeCost)
                {
                    audio.MenuSelect.Play();
                    HydroBot.strength += GameConstants.gainStrength;
                    HydroBot.unassignedPts -= GameConstants.gainAttributeCost;
                }
            }

            bool mouseOnDoneButton = AttributeScene.doneIconRectangle.Intersects(new Rectangle(lastMouseState.X, lastMouseState.Y, 1, 1));
            AttributeBoard.doneButtonHover = mouseOnDoneButton;

            if ((mouseOnDoneButton
                && lastMouseState.LeftButton == ButtonState.Pressed
                && currentMouseState.LeftButton == ButtonState.Released)
                || EscPressed || enterPressed)
            {
                AttributeBoard.doneButtonPressed = true;
                audio.MenuScroll.Play();
                if (prevScene is ShipWreckScene)
                {
                    shipWreckScene.backFromAttributeBoard = true;
                }
                ShowScene(prevScene);
                if (prevScene is ShipWreckScene)
                {
                    shipWreckScene.screenTransitNow = false;
                }
            }
            else AttributeBoard.doneButtonPressed = false;

        }

        private void HandleLevelObjectiveInput()
        {
            if (EscPressed || enterPressed || TabPressed)
            {
                if (prevScene is ShipWreckScene) shipWreckScene.backFromTipOrLevelObjective = true;
                ShowScene(prevScene);
            }
        }

        private void HandleTipInput()
        {
            if (EscPressed || enterPressed)
            {
                if (prevScene is ShipWreckScene) shipWreckScene.backFromTipOrLevelObjective = true;
                ShowScene(prevScene);
            }
        }

        protected void ShowScene(GameScene scene)
        {
            activeScene.Hide();
            activeScene = scene;
            Factory.buildingSoundInstance.Pause();
            ResearchFacility.buildingSoundInstance.Pause();
            scene.Show();
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //PerformanceHelper.StartFrame();
            //using (new TimeRulerHelper("Update", Color.Yellow))
            //{
                // System.Threading.Thread.Sleep(5);

                // Get the Keyboard and GamePad state
                CheckKeyEntered();
                if (gameState != GameState.DisplayMenu && gameState != GameState.ToMainMenu && EscPressed) gameState = GameState.DisplayMenu;
                if (gameState == GameState.GameStart)
                {
                    videoPlayer.Play(presentScene);
                    gameState = GameState.PlayingPresentScene;
                }
                if (gameState == GameState.PlayingPresentScene)
                {
                    return;
                }
                if (gameState == GameState.DisplayMenu)
                {
                    videoPlayer.Stop();
                    // Start the game in the start Scene
                    startScene.Show();
                    activeScene = startScene;
                    //just changing to a random state
                    gameState = GameState.ToMainMenu;
                }

                CheckClick(gameTime);
                HandleScenesInput(gameTime);
                base.Update(gameTime);
            //}
            //perfString = "FPS: " + PerformanceHelper.FpsCounter.Fps;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //using (new TimeRulerHelper("Draw", Color.Blue))
            //{

               GraphicsDevice.DepthStencilState = DepthStencilState.Default;
               if (gameState == GameState.PlayingPresentScene)
               {
                   graphics.GraphicsDevice.Clear(Color.Black);
                   Texture2D playingTexture;
                   if (videoPlayer.State == MediaState.Playing)
                   {
                       playingTexture = videoPlayer.GetTexture();
                       spriteBatch.Begin();
                       //spriteBatch.Draw(playingTexture, new Rectangle((int)(graphics.PreferredBackBufferWidth / 4), (int)(graphics.PreferredBackBufferHeight/4), (int)(graphics.PreferredBackBufferWidth/2), (int)(graphics.PreferredBackBufferHeight/2)), Color.White);
                       spriteBatch.Draw(playingTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
                       spriteBatch.End();
                   }
                   if (videoPlayer.State == MediaState.Stopped)
                       gameState = GameState.DisplayMenu;
               }
               //Draw loading scene to mask long loading time
               if (PlayGameScene.currentGameState == GameState.ToNextLevel) loadingScene.Draw(gameTime);
               base.Draw(gameTime);
            //}
            //spriteBatch.Begin();
            //spriteBatch.DrawString(smallFont, perfString + "\n" + "Avg draw: " + PerformanceHelper.TimeRuler.GetAverageTime(0, "Draw")
            //    + "\nAvg Update: " + PerformanceHelper.TimeRuler.GetAverageTime(0, "Update"), new Vector2(500, 500), Color.White);
            //spriteBatch.End();
        }

    }
}

