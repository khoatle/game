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
using Poseidon.Core;
namespace Poseidon
{
    public enum GameState { Loading, Running, Won, Lost }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public partial class PoseidonGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        KeyboardState lastKeyboardState = new KeyboardState();
        KeyboardState currentKeyboardState = new KeyboardState();
        GamePadState lastGamePadState = new GamePadState();
        GamePadState currentGamePadState = new GamePadState();

        SpriteBatch spriteBatch;
        SpriteFont statsFont;

        // Textures for help scene
        protected Texture2D helpBackgroundTexture, helpForegroundTexture;

        HelpScene helpScene;
        protected GameScene activeScene;
        // For the Start scene
        private SpriteFont smallFont, largeFont;
        protected Texture2D startBackgroundTexture, startElementsTexture;
        StartScene startScene;
        // Audio Stuff
        private AudioLibrary audio;
        PlayGameScene playGameScene;

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

            // Load Audio Elements
            audio = new AudioLibrary();
            audio.LoadContent(Content);
            Services.AddService(typeof(AudioLibrary), audio);

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

            // Create the main game play scene
            playGameScene = new PlayGameScene(this, graphics, Content, GraphicsDevice, spriteBatch);
            Components.Add(playGameScene);
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
        private bool CheckEnterA()
        {
            // Get the Keyboard and GamePad state
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            bool result = (lastKeyboardState.IsKeyDown(Keys.Enter) &&
                (keyboardState.IsKeyUp(Keys.Enter)));
            result |= (lastGamePadState.Buttons.A == ButtonState.Pressed) &&
                      (gamepadState.Buttons.A == ButtonState.Released);

            lastKeyboardState = keyboardState;
            lastGamePadState = gamepadState;

            return result;
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
                if (CheckEnterA())
                {
                    ShowScene(startScene);
                }
            }
            // Handle Action Scene Input
            else if (activeScene == playGameScene)
            {
                HandleActionInput();
            }
        }

        /// <summary>
        /// Handle update for the main game
        /// </summary>
        private void HandleActionInput()
        {

            lastKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            lastGamePadState = currentGamePadState;
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
            // Allows the game to exit
            if ((currentKeyboardState.IsKeyDown(Keys.Escape)) ||
                (currentGamePadState.Buttons.Back == ButtonState.Pressed))
                this.Exit();

        }


        /// <summary>
        /// Handle buttons and keyboard in StartScene
        /// </summary>
        private void HandleStartSceneInput()
        {
            if (CheckEnterA())
            {
                audio.MenuSelect.Play();
                switch (startScene.SelectedMenuIndex)
                {
                    case 0:
                        //activeScene.Hide();
                        //activeScene = null;
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

            HandleScenesInput(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            base.Draw(gameTime);
            spriteBatch.End();
        }

    }
}
