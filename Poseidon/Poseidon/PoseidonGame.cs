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
namespace Poseidon
{
    public enum GameState { PlayingCutScene, Loading, Running, Won, Lost }

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

        SpriteBatch spriteBatch;
        SpriteFont statsFont;
        CutSceneDialog cutSceneDialog;
        // Textures for help scene
        protected Texture2D helpBackgroundTexture, helpForegroundTexture;
        HelpScene helpScene;
        protected GameScene activeScene;
        protected GameScene prevScene;
        // For the Start scene
        private SpriteFont smallFont, largeFont;
        protected Texture2D startBackgroundTexture, startElementsTexture;
        StartScene startScene;
        // For the Skill board
        SkillScene skillScene;
        // Audio Stuff
        private AudioLibrary audio;
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
        bool skillPressed;
        public PoseidonGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 853;
            graphics.PreferredBackBufferHeight = 700;

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
            actionTexture = Content.Load<Texture2D>("Image/rockrainenhanced");

            //For the Help scene
            helpBackgroundTexture = Content.Load<Texture2D>("Image/helpbackground");
            helpForegroundTexture = Content.Load<Texture2D>("Image/helpForeground");
            helpScene = new HelpScene(this, helpBackgroundTexture,
            helpForegroundTexture);
            Components.Add(helpScene);

            // Create the Start Scene
            smallFont = Content.Load<SpriteFont>("Fonts/menuSmall");
            largeFont = Content.Load<SpriteFont>("Fonts/menuLarge");
            startBackgroundTexture = Content.Load<Texture2D>("Image/startbackground");
            startElementsTexture = Content.Load<Texture2D>("Image/startSceneElements");
            startScene = new StartScene(this, smallFont, largeFont,
                startBackgroundTexture, startElementsTexture);
            Components.Add(startScene);

            // Loading the cutscenes
            cutSceneDialog = new CutSceneDialog();

            // Create the main game play scene
            playGameScene = new PlayGameScene(this, graphics, Content, GraphicsDevice, spriteBatch, pausePosition, pauseRect, actionTexture, cutSceneDialog);
            Components.Add(playGameScene);

            // Create the main game play scene
            shipWreckScene = new ShipWreckScene(this, graphics, Content, GraphicsDevice, spriteBatch, pausePosition, pauseRect, actionTexture, cutSceneDialog);
            Components.Add(shipWreckScene);

            // Create the Skill board
            skillScene = new SkillScene(this, smallFont, largeFont,
                startBackgroundTexture, startElementsTexture, Content);
            Components.Add(skillScene);

            

            // Start the game in the start Scene :)
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

        private void CheckKeyEntered()
        {
            // Get the Keyboard and GamePad state
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            enterPressed = (lastKeyboardState.IsKeyDown(Keys.Enter) &&
                (keyboardState.IsKeyUp(Keys.Enter)));
            enterPressed |= (lastGamePadState.Buttons.A == ButtonState.Pressed) &&
                      (gamepadState.Buttons.A == ButtonState.Released);
            pPressed = (lastKeyboardState.IsKeyDown(Keys.P) &&
                (keyboardState.IsKeyUp(Keys.P)));
            zPressed = (lastKeyboardState.IsKeyDown(Keys.Z) &&
                (keyboardState.IsKeyUp(Keys.Z)));
            backPressed = (lastKeyboardState.IsKeyDown(Keys.Escape) &&
                (keyboardState.IsKeyUp(Keys.Escape)));
            skillPressed = (lastKeyboardState.IsKeyDown(Keys.I) &&
                (keyboardState.IsKeyUp(Keys.I)));
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
                if (enterPressed)
                {
                    ShowScene(startScene);
                }
            }
            // Handle Action Scene Input
            else if (activeScene == playGameScene)
            {
                HandleActionInput();
            }
            // Handle Skill scene input
            else if (activeScene == skillScene)
            {
                HandleSkillSceneInput();
            }
            // Handle ship wreck scene input
            else if (activeScene == shipWreckScene)
            {
                HandleShipWreckSceneInput();
            }
        }

        /// <summary>
        /// Handle update for the ship wreck scene
        /// </summary>
        public void HandleShipWreckSceneInput()
        {
            // User pauses the game
            if (pPressed)
            {
                audio.MenuBack.Play();
                playGameScene.Paused = !playGameScene.Paused;
            }
            if (backPressed)
            {
                playGameScene.tank.CopyAttribute(shipWreckScene.tank); 
                ShowScene(playGameScene);
            }
            if (skillPressed)
            {
                prevScene = shipWreckScene;
                ShowScene(skillScene);
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
                ShowScene(startScene);
            }
            if (skillPressed)
            {
                prevScene = playGameScene;
                ShowScene(skillScene);
            }
            if (zPressed && GetInShipWreck())
            {
                shipWreckScene.tank.CopyAttribute(playGameScene.tank);
                ShowScene(shipWreckScene);
            }
        }
        public bool GetInShipWreck()
        {
          
            BoundingSphere shipWreckBoundingSphere = new BoundingSphere(playGameScene.tank.BoundingSphere.Center,
                    10);
            for (int curWreck = 0; curWreck < playGameScene.shipWrecks.Count; curWreck++)
            {
                if (!playGameScene.shipWrecks[curWreck].accessed && shipWreckBoundingSphere.Intersects(
                    playGameScene.shipWrecks[curWreck].BoundingSphere))
                {
                    // no re-explore a ship wreck
                    playGameScene.shipWrecks[curWreck].accessed = true;
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
        /// Handle buttons and keyboard in SkillScene
        /// </summary>
        private void HandleSkillSceneInput()
        {
            if (enterPressed)
            {
                audio.MenuSelect.Play();
                switch (skillScene.SelectedMenuIndex)
                {
                    case 0:
                        if (prevScene == playGameScene)
                            playGameScene.tank.strength += 0.25f;
                        else shipWreckScene.tank.strength += 0.25f;
                        break;
                    case 1:
                        if (prevScene == playGameScene)
                            playGameScene.tank.speed += 0.25f;
                        else shipWreckScene.tank.speed += 0.25f;
                        break;
                    case 2:
                        if (prevScene == playGameScene)
                            playGameScene.tank.shootingRate += 0.25f;
                        else shipWreckScene.tank.shootingRate += 0.25f;
                        break;
                    case 3:
                        if (prevScene == playGameScene)
                            playGameScene.tank.hitPoint += 30;
                        else shipWreckScene.tank.hitPoint += 30;
                        break;
                    case 4:
                        ShowScene(prevScene);
                        break;
                }
            }
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
            spriteBatch.Begin();
            base.Draw(gameTime);
            //if (paused)
            //{
                // Draw the "pause" text
            //    spriteBatch.Draw(actionTexture, pausePosition, pauseRect,
            //        Color.White);
            //}
            //else base.Draw(gameTime);
            spriteBatch.End();
        }

    }
}
