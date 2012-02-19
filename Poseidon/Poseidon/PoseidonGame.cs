// Copyright (C) Microsoft Corporation. All rights reserved.

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
    public enum GameState { PlayingCutScene, Loading, Running, Won, Lost, ToMiniGame, ToNextLevel, GameComplete, ToMainMenu }
    public enum GameMode { MainGame, ShipWreck, SurvivalMode };
    public enum TrashType { biodegradable, plastic, radioactive };
    public enum FactoryType { biodegradable, plastic, radioactive};
    public enum BuildingType { biodegradable, plastic, radioactive, researchlab }; // as a super type for factory-type (including researchlab)

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PoseidonGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

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
        protected Texture2D helpBackgroundTexture, helpForegroundTexture;
        HelpScene helpScene;
        protected GameScene activeScene;
        protected GameScene prevScene;
        // For the Start scene
        private SpriteFont smallFont, largeFont, startSceneSmall, startSceneLarge, typeFont;
        protected Texture2D startBackgroundTexture, startElementsTexture, teamLogo;
        StartScene startScene;
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
        public static bool AttributeButtonPressed;
        bool EscPressed;
        bool doubleClicked = false;
        bool clicked=false;
        double clickTimer = 0;

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

        public PoseidonGame()
        {
            graphics = new GraphicsDeviceManager(this);


            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;//850;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;//700;
            
            graphics.IsFullScreen = false;

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
            statsFont = Content.Load<SpriteFont>("Fonts/StatsFont");

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
            actionTexture = Content.Load<Texture2D>("Image/Miscellaneous/actionTextures");
            stunnedTexture = Content.Load<Texture2D>("Image/Miscellaneous/dizzy-icon");

            // Loading the radar
            Vector2 radarCenter = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Right - GameConstants.RadarScreenRadius, GraphicsDevice.Viewport.TitleSafeArea.Bottom - GameConstants.RadarScreenRadius);
            radar = new Radar(Content, "Image/RadarTextures/playerDot", "Image/RadarTextures/enemyDot", "Image/RadarTextures/fishDot", "Image/RadarTextures/compass", "Image/RadarTextures/bossDot", radarCenter);

            //For the Help scene
            helpBackgroundTexture = Content.Load<Texture2D>("Image/SceneTextures/helpbackground");
            helpForegroundTexture = Content.Load<Texture2D>("Image/SceneTextures/helpForeground");
            helpScene = new HelpScene(this, helpBackgroundTexture, helpForegroundTexture, spriteBatch);
            Components.Add(helpScene);

            // Create the Start Scene
            startSceneSmall = Content.Load<SpriteFont>("Fonts/startScreenLarge");
            startSceneLarge = Content.Load<SpriteFont>("Fonts/startScreenLarge");
            smallFont = Content.Load<SpriteFont>("Fonts/menuSmall");
            largeFont = Content.Load<SpriteFont>("Fonts/menuLarge");
            typeFont = Content.Load<SpriteFont>("Fonts/font");
            startBackgroundTexture = Content.Load<Texture2D>("Image/SceneTextures/startbackground");
            startElementsTexture = Content.Load<Texture2D>("Image/SceneTextures/startSceneElements");
            teamLogo = Content.Load<Texture2D>("Image/Miscellaneous/TeamLogo");
            startScene = new StartScene(this, startSceneSmall, startSceneLarge,
                startBackgroundTexture, startElementsTexture, teamLogo);
            Components.Add(startScene);
            //SkillBackgroundTexture = Content.Load<Texture2D>("Image/skill_background");

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
            selectLoadingLevelScene = new SelectLoadingLevelScene(this, startSceneLarge, startBackgroundTexture, teamLogo);
            Components.Add(selectLoadingLevelScene);

            // Create the shipwreck game play scene -- MUST be created before play game scene, as it overwrites the static attibutes of hydrobot
            shipWreckScene = new ShipWreckScene(this, graphics, Content, GraphicsDevice, spriteBatch, pausePosition, pauseRect, actionTexture, cutSceneDialog, stunnedTexture);
            Components.Add(shipWreckScene);

            // Start the game in the start Scene
            startScene.Show();
            activeScene = startScene;

            //initiate graphic for good will bar
            IngamePresentation.InitiateGoodWillBarGraphic(Content);
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
            else if (activeScene == selectLoadingLevelScene)
            {
                HandleSelectLoadingLevelSceneInput();
            }
            else if (activeScene == loadingScene)
            {
                if (loadingScene.loadingSceneStarted)
                {
                    //CreateLevelDependentScenes(loadingScene.loadingLevel);
                    CreateLevelDependentScenes();
                    startScene.gameStarted = true;
                    startScene.Hide();
                    loadingScene.loadingSceneStarted = false;
                    ShowScene(playGameScene);
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
            if (enterPressed)
            {
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
            if (quizzGameScene.questionAnswered >= 4)// || enterPressed)
            {
                //each right answer give 5% environment boost
                HydroBot.currentEnvPoint += quizzGameScene.numRightAnswer * GameConstants.envGainForCorrectQuizAnswer;
                if (HydroBot.currentEnvPoint >= HydroBot.maxEnvPoint) HydroBot.currentEnvPoint = HydroBot.maxEnvPoint;
                HydroBot.currentExperiencePts += quizzGameScene.numRightAnswer * 50;
                PlayGameScene.currentGameState = GameState.ToNextLevel;
                ShowScene(playGameScene);
            }
        }
        public void HandleTypeGameInput()
        {
            if (typeGameScene.isOver && enterPressed)
            {
                PlayGameScene.currentGameState = GameState.ToNextLevel;
                ShowScene(playGameScene);
            }
        }
        public void HandleJigsawInput()
        {
            if (jigsawGameScene.inOrder)
            {
                AddingObjects.placeMinion(Content, jigsawType, playGameScene.enemies, playGameScene.enemiesAmount, playGameScene.fish, ref playGameScene.fishAmount, playGameScene.hydroBot);
                MediaPlayer.Stop();
                ShowScene(playGameScene);
                switch (jigsawType)
                {
                    case 0:
                        HydroBot.hasSeaCow = true;
                        HydroBot.iconActivated[IngamePresentation.seaCowIcon] = true;
                        HydroBot.seaCowPower += 1.0f;
                        HydroBot.numSeaCowPieces -= GameConstants.boneCountForJigsaw;
                        ResearchFacility.playSeaCowJigsaw = false;
                        ResearchFacility.seaCowWon = true;
                        break;
                    case 1:
                        HydroBot.hasTurtle = true;
                        HydroBot.iconActivated[IngamePresentation.turtleIcon] = true;
                        HydroBot.turtlePower += 1.0f;
                        HydroBot.numTurtlePieces -= GameConstants.boneCountForJigsaw;
                        ResearchFacility.playTurtleJigsaw = false;
                        ResearchFacility.turtleWon = true;
                        break;
                    case 2:
                        HydroBot.hasDolphin = true;
                        HydroBot.iconActivated[IngamePresentation.dolphinIcon] = true;
                        HydroBot.dolphinPower += 1.0f;
                        HydroBot.numDolphinPieces -= GameConstants.boneCountForJigsaw;
                        ResearchFacility.playDolphinJigsaw = false;
                        ResearchFacility.dolphinWon = true;
                        break;
                }
            }
            if (jigsawGameScene.timeUp)
            {
                MediaPlayer.Stop();
                ShowScene(playGameScene);
                switch (jigsawType)
                {
                    case 0:
                        HydroBot.numSeaCowPieces -= GameConstants.boneCountForJigsaw;
                        ResearchFacility.playSeaCowJigsaw = false;
                        ResearchFacility.seaCowLost = true;
                        break;
                    case 1:
                        HydroBot.numTurtlePieces -= GameConstants.boneCountForJigsaw;
                        ResearchFacility.playTurtleJigsaw = false;
                        ResearchFacility.turtleLost = true;
                        break;
                    case 2:
                        HydroBot.numDolphinPieces -= GameConstants.boneCountForJigsaw;
                        ResearchFacility.playDolphinJigsaw = false;
                        ResearchFacility.dolphinLost = true;
                        break;
                }
            }
            if (EscPressed)
            {
                MediaPlayer.Stop();
                ShowScene(playGameScene);
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
            if (backPressed)
            {
                //playGameScene.tank.CopyAttribute(shipWreckScene.tank);
                playGameScene.roundTimer = shipWreckScene.roundTimer;
                playGameScene.Scene2Texture = shipWreckScene.cutSceneImmediateRenderTarget;
                playGameScene.screenTransitNow = true;
                playGameScene.graphicEffect.resetTransitTimer();
                ShipWreckScene.gameCamera.shaking = false;
                ShowScene(playGameScene);
                doubleClicked = false;
            }
            //do not let the player to open the attribute board in shipwreck now
            //because it will reset the shipwreck
            if (AttributePressed || AttributeButtonPressed)
            {
                prevScene = shipWreckScene;
                ShowScene(AttributeScene);
                AttributeButtonPressed = false;
            }
        }
        /// <summary>
        /// Handle update for the main game
        /// </summary>
        private void HandleActionInput()
        {

            // User pauses the game
            if (pPressed)
            {
                audio.MenuBack.Play();
                playGameScene.Paused = !playGameScene.Paused;
            }
            if (backPressed)
            {
                if (PlayGameScene.currentGameState == GameState.PlayingCutScene)
                {
                    PlayGameScene.currentGameState = GameState.Running;
                }
                else
                {
                    MediaPlayer.Stop();
                    ShowScene(startScene);
                }
            }
            if (AttributePressed || AttributeButtonPressed)
            {
                prevScene = playGameScene;
                ShowScene(AttributeScene);
                AttributeButtonPressed = false;
            }
            if (doubleClicked 
                && !CursorManager.MouseOnEnemy(playGameScene.cursor, PlayGameScene.gameCamera, playGameScene.enemies, playGameScene.enemiesAmount)
                && !CursorManager.MouseOnFish(playGameScene.cursor, PlayGameScene.gameCamera, playGameScene.fish, playGameScene.fishAmount)
                && GetInShipWreck())
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
            if (lastMouseState.LeftButton == ButtonState.Pressed
                && currentMouseState.LeftButton == ButtonState.Released
                && playGameScene.mouseOnLevelObjectiveIcon(currentMouseState))
            {
                prevScene = playGameScene;
                ShowScene(levelObjectiveScene);
            }
            if (lastMouseState.LeftButton == ButtonState.Pressed
                && currentMouseState.LeftButton == ButtonState.Released
                && playGameScene.mouseOnTipIcon(currentMouseState))
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
                if (!playGameScene.shipWrecks[curWreck].accessed
                    && CursorManager.MouseOnObject(playGameScene.cursor,playGameScene.shipWrecks[curWreck].BoundingSphere, playGameScene.shipWrecks[curWreck].Position, PlayGameScene.gameCamera)
                    && playGameScene.CharacterNearShipWreck(playGameScene.shipWrecks[curWreck].BoundingSphere)
                    )
                {            
                    // no re-explore a ship wreck
                    // no, let the user re-explore now because he would miss a relic -> lose
                    //playGameScene.shipWrecks[curWreck].accessed = true;
                    // put the skill into one of the chest if skillID != 0
                    shipWreckScene.currentShipWreckID = curWreck;
                    shipWreckScene.skillID = playGameScene.shipWrecks[curWreck].skillID;
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
            }
            if (backPressed)
            {
                MediaPlayer.Stop();
                ShowScene(startScene);
            }
            if (AttributePressed || AttributeButtonPressed)
            {
                prevScene = survivalGameScene;
                ShowScene(AttributeScene);
                AttributeButtonPressed = false;
            }
            if (survivalGameScene.currentGameState == GameState.ToMainMenu)
                ShowScene(startScene);
        }
        /// <summary>
        /// Handle buttons and keyboard in StartScene
        /// </summary>
        private void HandleStartSceneInput()
        {
            if (enterPressed)
            {
                audio.MenuSelect.Play();

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
                        ShowScene(playGameScene);
                        break;
                    case "Load Saved Level":
                        MediaPlayer.Stop();
                        ShowScene(selectLoadingLevelScene);
                        break;
                    case "Survival Mode":
                        MediaPlayer.Stop();
                        gamePlus = false;
                        CreateSurvivalDependentScenes();
                        SurvivalGameScene.score = 0;
                        ShowScene(survivalGameScene);
                        break;
                    case "Help":
                        ShowScene(helpScene);
                        break;
                    case "Quit":
                        Exit();
                        break;
                        
                }
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
            cutSceneDialog = new CutSceneDialog();

            // Create the main game play scene
            playGameScene = new PlayGameScene(this, graphics, Content, GraphicsDevice, spriteBatch, pausePosition, pauseRect, actionTexture, cutSceneDialog, radar, stunnedTexture);
            Components.Add(playGameScene);
                      
            // Create the Attribute board
            AttributeScene = new AttributeBoard(this, AttributeBackgroundTexture, Content);
            Components.Add(AttributeScene);

            // Create level objective scene
            levelObjectiveScene = new LevelObjectiveScene(this, LevelObjectiveBackgroundTexture, Content, playGameScene);
            Components.Add(levelObjectiveScene);

            // Create tip scene
            tipScene = new TipScene(this, tipBackgroundTexture, Content);
            Components.Add(tipScene);

            // Create minigame scenes
            quizzGameScene = new QuizzGameScene(this, quizzGameBackgroundTexture, Content, GraphicsDevice);
            Components.Add(quizzGameScene);
            typeGameScene = new TypingGameScene(this, typeFont, boxBackground, typeGameBackgroundTexture, Content);
            Components.Add(typeGameScene);
            jigsawGameScene = new JigsawGameScene(this, Content, graphics, GraphicsDevice);
            Components.Add(jigsawGameScene);

        }
        private void CreateSurvivalDependentScenes()
        {
            // Create the survival game play scene
            survivalGameScene = new SurvivalGameScene(this, graphics, Content, GraphicsDevice, spriteBatch, pausePosition, pauseRect, actionTexture, cutSceneDialog, radar, stunnedTexture);
            Components.Add(survivalGameScene);

            // Create the Attribute board
            AttributeScene = new AttributeBoard(this, AttributeBackgroundTexture, Content);
            Components.Add(AttributeScene);

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
            if ((AttributeScene.doneIconRectangle.Intersects(new Rectangle(lastMouseState.X, lastMouseState.Y, 1,1))
                && lastMouseState.LeftButton == ButtonState.Pressed
                && currentMouseState.LeftButton == ButtonState.Released)
                || EscPressed || enterPressed)
            {
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

        }

        private void HandleLevelObjectiveInput()
        {
            if (EscPressed || enterPressed)
                ShowScene(prevScene);
        }

        private void HandleTipInput()
        {
            if (EscPressed || enterPressed)
                ShowScene(prevScene);
        }

        protected void ShowScene(GameScene scene)
        {
            activeScene.Hide();
            activeScene = scene;
            scene.Show();
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Get the Keyboard and GamePad state
            CheckKeyEntered();
            CheckClick(gameTime);
            HandleScenesInput(gameTime);
            base.Update(gameTime);
            
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            
            //graphics.GraphicsDevice.Clear(Color.Black);
            //spriteBatch.Begin();
            base.Draw(gameTime);
            //spriteBatch.End();
        }

    }
}

