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

namespace Poseidon
{
    public enum GameState { PlayingCutScene, Loading, Running, Won, Lost, ToMiniGame, ToNextLevel }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PoseidonGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        KeyboardState lastKeyboardState = new KeyboardState();
        //KeyboardState currentKeyboardState = new KeyboardState();
        GamePadState lastGamePadState = new GamePadState();
        //GamePadState currentGamePadState = new GamePadState();
        MouseState currentMouseState = new MouseState();
        MouseState lastMouseState = new MouseState();

        SpriteBatch spriteBatch;
        SpriteFont statsFont;
        CutSceneDialog cutSceneDialog;
        // Textures for help scene
        protected Texture2D helpBackgroundTexture, helpForegroundTexture;
        HelpScene helpScene;
        protected GameScene activeScene;
        protected GameScene prevScene;
        // For the Start scene
        private SpriteFont smallFont, largeFont, startSceneSmall, startSceneLarge, typeFont;
        protected Texture2D startBackgroundTexture, startElementsTexture;
        StartScene startScene;
        // For the Attribute board
        AttributeBoard AttributeScene;
        protected Texture2D AttributeBackgroundTexture;
        // For the Level Objective
        LevelObjectiveScene levelObjectiveScene;
        protected Texture2D LevelObjectiveBackgroundTexture;
        // For the mini games
        QuizzGameScene quizzGameScene;
        protected Texture2D quizzGameBackgroundTexture;
        TypingGameScene typeGameScene;
        protected Texture2D typeGameBackgroundTexture;
        protected Texture2D boxBackground;
        // Audio Stuff
        public static AudioLibrary audio;
        PlayGameScene playGameScene;
        ShipWreckScene shipWreckScene;
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
        bool EscPressed;
        bool doubleClicked = false;
        bool clicked=false;
        double clickTimer = 0;

        // Texture to show that enemy is stunned
        protected Texture2D stunnedTexture;

        // Radar for the game
        Radar radar;

        //for continously playing random background musics
        Random rand = new Random();

        public PoseidonGame()
        {
            graphics = new GraphicsDeviceManager(this);


            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;//850;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;//700;

            //graphics.IsFullScreen = true;

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
            startSceneSmall = Content.Load<SpriteFont>("Fonts/startScreenSmall");
            startSceneLarge = Content.Load<SpriteFont>("Fonts/startScreenLarge");
            smallFont = Content.Load<SpriteFont>("Fonts/menuSmall");
            largeFont = Content.Load<SpriteFont>("Fonts/menuLarge");
            typeFont = Content.Load<SpriteFont>("Fonts/font");
            startBackgroundTexture = Content.Load<Texture2D>("Image/SceneTextures/startbackground");
            startElementsTexture = Content.Load<Texture2D>("Image/SceneTextures/startSceneElements");
            startScene = new StartScene(this, startSceneSmall, startSceneLarge,
                startBackgroundTexture, startElementsTexture);
            Components.Add(startScene);
            //SkillBackgroundTexture = Content.Load<Texture2D>("Image/skill_background");

            AttributeBackgroundTexture = Content.Load<Texture2D>("Image/AttributeBoardTextures/AttributeBackground");
            LevelObjectiveBackgroundTexture = Content.Load<Texture2D>("Image/SceneTextures/LevelObjectiveBackground");
            quizzGameBackgroundTexture = Content.Load<Texture2D>("Image/MinigameTextures/classroom1");
            typeGameBackgroundTexture = Content.Load<Texture2D>("Image/MinigameTextures/classroom2");
            boxBackground = Content.Load<Texture2D>("Image/MinigameTextures/whiteskin");

            // Loading the cutscenes
            cutSceneDialog = new CutSceneDialog();

            // Create the main game play scene
            playGameScene = new PlayGameScene(this, graphics, Content, GraphicsDevice, spriteBatch, pausePosition, pauseRect, actionTexture, cutSceneDialog, radar, stunnedTexture);
            Components.Add(playGameScene);

            // Create the main game play scene
            shipWreckScene = new ShipWreckScene(this, graphics, Content, GraphicsDevice, spriteBatch, pausePosition, pauseRect, actionTexture, cutSceneDialog, stunnedTexture);
            Components.Add(shipWreckScene);

            // Create the Attribute board
            AttributeScene = new AttributeBoard(this, smallFont, largeFont,
                AttributeBackgroundTexture, Content);
            Components.Add(AttributeScene);

            // Create level objective scene
            levelObjectiveScene = new LevelObjectiveScene(this, smallFont,
                largeFont, LevelObjectiveBackgroundTexture, Content, playGameScene);
            Components.Add(levelObjectiveScene);

            // Create minigame scenes
            quizzGameScene = new QuizzGameScene(this, smallFont,
                largeFont, quizzGameBackgroundTexture, Content);
            Components.Add(quizzGameScene);
            typeGameScene = new TypingGameScene(this, typeFont, boxBackground, typeGameBackgroundTexture, Content);
            Components.Add(typeGameScene);
            // Start the game in the start Scene
            startScene.Show();
            activeScene = startScene;

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
            else if (activeScene == quizzGameScene)
            {
                HandleQuizzGameInput();
            }
            else if (activeScene == typeGameScene)
            {
                HandleTypeGameInput();
            }
        }

        public void HandleQuizzGameInput()
        {
            if (quizzGameScene.questionAnswered >= 4)// || enterPressed)
            {
                //each right answer give 5% environment boost
                Tank.currentEnvPoint += quizzGameScene.numRightAnswer * GameConstants.envGainForCorrectQuizAnswer;
                if (Tank.currentEnvPoint >= Tank.maxEnvPoint) Tank.currentEnvPoint = Tank.maxEnvPoint;
                playGameScene.currentGameState = GameState.ToNextLevel;
                ShowScene(playGameScene);
            }
        }
        public void HandleTypeGameInput()
        {
            if (typeGameScene.isOver && enterPressed)
            {
                playGameScene.currentGameState = GameState.ToNextLevel;
                ShowScene(playGameScene);
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
                playGameScene.Paused = !playGameScene.Paused;
            }
            if (backPressed)
            {
                //playGameScene.tank.CopyAttribute(shipWreckScene.tank);
                playGameScene.roundTimer = shipWreckScene.roundTimer;
                ShipWreckScene.gameCamera.shaking = false;
                ShowScene(playGameScene);
                doubleClicked = false;
            }
            if (AttributePressed)
            {
                prevScene = shipWreckScene;
                ShowScene(AttributeScene);
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
                MediaPlayer.Stop();
                ShowScene(startScene);
            }
            if (AttributePressed)
            {
                prevScene = playGameScene;
                ShowScene(AttributeScene);
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
            else
            {
                doubleClicked = false;
            }
            if (playGameScene.currentGameState == GameState.ToMiniGame)
            {
                Random rand = new Random();
                //if (rand.Next(2) == 0)
                //    ShowScene(quizzGameScene);
                //else
                    ShowScene(typeGameScene);
            }
        }
        public bool GetInShipWreck()
        {

            for (int curWreck = 0; curWreck < playGameScene.shipWrecks.Count; curWreck++)
            {
                if (!playGameScene.shipWrecks[curWreck].accessed
                    && CursorManager.MouseOnShipWreck(playGameScene.cursor,playGameScene.shipWrecks[curWreck].BoundingSphere, playGameScene.shipWrecks[curWreck].Position, PlayGameScene.gameCamera)
                    && playGameScene.CharacterNearShipWreck(playGameScene.shipWrecks[curWreck].BoundingSphere)
                    )
                {            
                    // no re-explore a ship wreck
                    // no, let the user re-explore now because he would miss a relic -> lose
                    //playGameScene.shipWrecks[curWreck].accessed = true;
                    // put the skill into one of the chest if skillID != 0
                    shipWreckScene.skillID = playGameScene.shipWrecks[curWreck].skillID;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Handle buttons and keyboard in StartScene
        /// </summary>
        private void HandleStartSceneInput()
        {
            if (enterPressed)
            {
                audio.MenuSelect.Play();
                switch (startScene.SelectedMenuIndex)
                {
                    case 0:
                        MediaPlayer.Stop();
                        ShowScene(playGameScene);
                        break;
                    case 1:
                        ShowScene(helpScene);
                        break;
                    case 2:
                        Exit();
                        break;
                }
            }
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
                if (Tank.unassignedPts >= GameConstants.gainAttributeCost)
                {
                    audio.MenuSelect.Play();
                    Tank.speed += 0.1f;
                    Tank.unassignedPts -= GameConstants.gainAttributeCost;
                }
            }
            if (AttributeScene.hitpointIconRectangle.Intersects(new Rectangle(lastMouseState.X, lastMouseState.Y, 1, 1))
                && lastMouseState.LeftButton == ButtonState.Pressed
                && currentMouseState.LeftButton == ButtonState.Released)
            {
                if (Tank.unassignedPts >= GameConstants.gainAttributeCost)
                {
                    audio.MenuSelect.Play();
                    Tank.currentHitPoint += 10;
                    if (Tank.currentHitPoint > Tank.maxHitPoint)
                        Tank.maxHitPoint = Tank.currentHitPoint;
                    Tank.unassignedPts -= GameConstants.gainAttributeCost;
                }
            }
            if (AttributeScene.shootrateIconRectangle.Intersects(new Rectangle(lastMouseState.X, lastMouseState.Y, 1, 1))
                && lastMouseState.LeftButton == ButtonState.Pressed
                && currentMouseState.LeftButton == ButtonState.Released)
            {
                if (Tank.unassignedPts >= GameConstants.gainAttributeCost)
                {
                    audio.MenuSelect.Play();
                    Tank.shootingRate += 0.1f;
                    Tank.unassignedPts -= GameConstants.gainAttributeCost;
                }
            }
            if (AttributeScene.bulletStrengthIconRectangle.Intersects(new Rectangle(lastMouseState.X, lastMouseState.Y, 1, 1))
                && lastMouseState.LeftButton == ButtonState.Pressed
                && currentMouseState.LeftButton == ButtonState.Released)
            {

                if (Tank.unassignedPts >= GameConstants.gainAttributeCost)
                {
                    audio.MenuSelect.Play();
                    Tank.strength += 0.1f;
                    Tank.unassignedPts -= GameConstants.gainAttributeCost;
                }
            }
            if ((AttributeScene.doneIconRectangle.Intersects(new Rectangle(lastMouseState.X, lastMouseState.Y, 1,1))
                && lastMouseState.LeftButton == ButtonState.Pressed
                && currentMouseState.LeftButton == ButtonState.Released)
                || EscPressed )
            {
                ShowScene(prevScene);
            }

        }

        private void HandleLevelObjectiveInput()
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

