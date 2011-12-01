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
        GamePadState lastGamePadState = new GamePadState();
        GamePadState currentGamePadState = new GamePadState();
        MouseState currentMouseState = new MouseState();
        MouseState lastMouseState = new MouseState();

        public static AudioLibrary audio;

        public TimeSpan startTime, roundTimer, roundTime;
        Random random;
        SpriteBatch spriteBatch;
        SpriteFont statsFont;
        SpriteFont fishTalkFont;

        SpriteFont menuSmall;
        GameObject ground;
        public static Camera gameCamera;
        public GameState currentGameState = GameState.Running;
        // In order to know we are resetting the level winning or losing
        // winning: keep the current bot
        // losing: reset our bot to the bot at the beginning of the level
        GameState prevGameState;
        GameObject boundingSphere;

        public List<DamageBullet> myBullet;
        public List<DamageBullet> alliesBullets;
        public List<DamageBullet> enemyBullet;
        public List<HealthBullet> healthBullet;

        List<Plant> plants;
        List<Fruit> fruits;

        public BaseEnemy[] enemies;
        public Fish[] fish;
        public int enemiesAmount = 0;
        public int fishAmount = 0;

        // The main character for this level
        public HydroBot hydroBot;
        // The main character at the beginning of this level
        // Used for restarting the level
        //bot prevbot;
        //private TimeSpan fireTime;
        private TimeSpan prevFireTime;

        // Game is paused?
        protected bool paused;
        protected Vector2 pausePosition;
        protected Rectangle pauseRect = new Rectangle(1, 120, 200, 44);
        protected Texture2D actionTexture;

        // For drawing the currently selected skill
        protected Texture2D[] skillTextures;
        // For drawing the currently selected bullet type
        protected Texture2D[] bulletTypeTextures;

        HeightMapInfo heightMapInfo;

        Radar radar;

        // Frustum of the camera
        public BoundingFrustum frustum;

        // For mouse inputs
        bool doubleClicked = false;
        bool clicked = false;
        double clickTimer = 0;

        private Texture2D stunnedTexture;

        // shader for underwater effect
        // Our Post Process effect object, this is where our shader will be loaded and compiled
        Effect effectPost;
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
            ground = new GameObject();
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

            //Uncomment below line to use LEVELS
            //string terrain_name = "Image/terrain" + currentLevel;

            //temporary code for testing
            Random random = new Random();
            int random_level = random.Next(20);
            string terrain_name = "Image/TerrainHeightMaps/terrain" + random_level;
            //end temporary testing code

            ground.Model = Content.Load<Model>(terrain_name);
            boundingSphere.Model = Content.Load<Model>("Models/Miscellaneous/sphere1uR");

            heightMapInfo = ground.Model.Tag as HeightMapInfo;
            if (heightMapInfo == null)
            {
                string message = "The terrain model did not have a HeightMapInfo " +
                    "object attached. Are you sure you are using the " +
                    "TerrainProcessor?";
                throw new InvalidOperationException(message);
            }

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



            plants = new List<Plant>();
            fruits = new List<Fruit>();

            hydroBot.Load(Content);

            //prevTank.Load(Content);
            roundTimer = roundTime;

            //Load healthbar
            HealthBar = Content.Load<Texture2D>("Image/Miscellaneous/HealthBar");
            EnvironmentBar = Content.Load<Texture2D>("Image/Miscellaneous/EnvironmentBar");

            // Load and compile our Shader into our Effect instance.
            effectPost = Content.Load<Effect>("Shaders/PostProcess");
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

            //Uncomment below line to use LEVELS
            //string terrain_name = "Image/terrain" + currentLevel;

            //temporary code for testing
            Random random = new Random();
            int random_level = random.Next(20);
            string terrain_name = "Image/TerrainHeightMaps/terrain" + random_level;
            //end temporary testing code

            ground.Model = Content.Load<Model>(terrain_name);

            // If we are resetting the level losing the game
            // Reset our bot to the one at the beginning of the lost level


            hydroBot.Reset();

            gameCamera.Update(hydroBot.ForwardDirection,
                hydroBot.Position, aspectRatio, gameTime);



            //Clean all trees
            plants.Clear();

            //Clean all fruits
            fruits.Clear();

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

          
            enemiesAmount = 0;
            fishAmount = 0;
            enemies = new BaseEnemy[GameConstants.SurvivalModeMaxShootingEnemy + GameConstants.SurvivalModeMaxCombatEnemy
                + GameConstants.SurvivalModeMaxMutantShark + GameConstants.SurvivalModeMaxTerminator];
            fish = new Fish[1];

            AddingObjects.placeEnemies(ref enemiesAmount, enemies, Content, random, fishAmount, fish, null,
                GameConstants.MainGameMinRangeX, GameConstants.MainGameMaxRangeX, GameConstants.MainGameMinRangeZ, GameConstants.MainGameMaxRangeZ, -1, GameMode.SurvivalMode, GameConstants.MainGameFloatHeight);

            AddingObjects.placeFish(ref fishAmount, fish, Content, random, enemiesAmount, enemies, null,
                GameConstants.MainGameMinRangeX, GameConstants.MainGameMaxRangeX, GameConstants.MainGameMinRangeZ, GameConstants.MainGameMaxRangeZ, -1, GameMode.SurvivalMode, GameConstants.MainGameFloatHeight);
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
                //lastMouseState = currentMouseState;
                currentKeyboardState = Keyboard.GetState();
                lastGamePadState = currentGamePadState;
                currentGamePadState = GamePad.GetState(PlayerIndex.One);
                //currentMouseState = Mouse.GetState();
                // Allows the game to exit
                //if ((currentKeyboardState.IsKeyDown(Keys.Escape)) ||
                //    (currentGamePadState.Buttons.Back == ButtonState.Pressed))
                //    //this.Exit();
                CursorManager.CheckClick(ref lastMouseState, ref currentMouseState, gameTime, ref clickTimer, ref clicked, ref doubleClicked);


                if ((currentGameState == GameState.Running))
                {

                    Vector3 pointIntersect = Vector3.Zero;
                    bool mouseOnLivingObject = CursorManager.MouseOnEnemy(cursor, gameCamera, enemies, enemiesAmount) || CursorManager.MouseOnFish(cursor, gameCamera, fish, fishAmount);
                    //if the user holds down Shift button
                    //let him change current bullet or skill type w/o moving
                    if (currentKeyboardState.IsKeyDown(Keys.RightShift) || currentKeyboardState.IsKeyDown(Keys.LeftShift))
                    {
                        // changing bullet type
                        if ((lastKeyboardState.IsKeyDown(Keys.LeftShift) || lastKeyboardState.IsKeyDown(Keys.RightShift)) && ((lastKeyboardState.IsKeyDown(Keys.L)
                                && currentKeyboardState.IsKeyUp(Keys.L)) || (lastMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released)))
                        {

                            HydroBot.bulletType++;
                            if (HydroBot.bulletType == GameConstants.numBulletTypes) HydroBot.bulletType = 0;
                            audio.ChangeBullet.Play();


                        }
                        // changing active skill
                        if ((lastKeyboardState.IsKeyDown(Keys.LeftShift) || lastKeyboardState.IsKeyDown(Keys.RightShift)) && ((lastKeyboardState.IsKeyDown(Keys.K)
                                && currentKeyboardState.IsKeyUp(Keys.K)) || (lastMouseState.RightButton == ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Released)))
                        {
                            if (HydroBot.activeSkillID != -1)
                            {
                                HydroBot.activeSkillID++;
                                if (HydroBot.activeSkillID == GameConstants.numberOfSkills) HydroBot.activeSkillID = 0;
                                while (HydroBot.skills[HydroBot.activeSkillID] == false)
                                {
                                    HydroBot.activeSkillID++;
                                    if (HydroBot.activeSkillID == GameConstants.numberOfSkills) HydroBot.activeSkillID = 0;
                                }
                            }
                        }
                        //if the user wants to move when changing skill or bullet, let him
                        //because this is better for fast action game
                        if (currentMouseState.LeftButton == ButtonState.Pressed)
                        {
                            pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);
                        }
                    }
                    //if the user click on right mouse button
                    //cast the current selected skill
                    //else if (lastMouseState.RightButton == ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Released)
                    else if (currentMouseState.RightButton == ButtonState.Pressed)
                    {

                        // Hercules' Bow!!!
                        if (HydroBot.activeSkillID == 0 && mouseOnLivingObject)
                        {
                            pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);
                            hydroBot.ForwardDirection = CursorManager.CalculateAngle(pointIntersect, hydroBot.Position);
                            //if the skill has cooled down
                            //or this is the 1st time the user uses it
                            if ((PoseidonGame.playTime.TotalSeconds - HydroBot.skillPrevUsed[0] > GameConstants.coolDownForHerculesBow) || HydroBot.firstUse[0] == true)
                            {
                                HydroBot.firstUse[0] = false;
                                HydroBot.skillPrevUsed[0] = PoseidonGame.playTime.TotalSeconds;
                                //audio.Explosion.Play();
                                CastSkill.UseHerculesBow(hydroBot, Content, spriteBatch, myBullet, this);
                                HydroBot.currentHitPoint -= GameConstants.skillHealthLoss; // Lose health after useing this

                                //display HP loss
                                Point point = new Point();
                                String point_string = "-" + GameConstants.skillHealthLoss.ToString() + "HP";
                                point.LoadContent(Content, point_string, hydroBot.Position, Color.Black);
                                points.Add(point);

                                hydroBot.reachDestination = true;
                                if (!hydroBot.clipPlayer.inRange(61, 90))
                                    hydroBot.clipPlayer.switchRange(61, 90);
                            }

                        }
                        //Thor's Hammer!!!
                        if (HydroBot.activeSkillID == 1)
                        {
                            if ((PoseidonGame.playTime.TotalSeconds - HydroBot.skillPrevUsed[1] > GameConstants.coolDownForArchillesArmor) || HydroBot.firstUse[1] == true)
                            {
                                HydroBot.firstUse[1] = false;
                                HydroBot.skillPrevUsed[1] = PoseidonGame.playTime.TotalSeconds;
                                audio.Explo1.Play();
                                gameCamera.Shake(25f, .4f);
                                CastSkill.UseThorHammer(gameTime, hydroBot, enemies, ref enemiesAmount, fish, fishAmount, GameMode.SurvivalMode);
                                HydroBot.currentHitPoint -= GameConstants.skillHealthLoss; // Lose health after useing this

                                //display HP loss
                                Point point = new Point();
                                String point_string = "-" + GameConstants.skillHealthLoss.ToString() + "HP";
                                point.LoadContent(Content, point_string, hydroBot.Position, Color.Black);
                                points.Add(point);

                                if (!hydroBot.clipPlayer.inRange(61, 90))
                                    hydroBot.clipPlayer.switchRange(61, 90);
                            }
                        }
                        // Achilles' Armor!!!
                        if (HydroBot.activeSkillID == 2)
                        {
                            if ((PoseidonGame.playTime.TotalSeconds - HydroBot.skillPrevUsed[2] > GameConstants.coolDownForThorHammer) || HydroBot.firstUse[2] == true)
                            {
                                HydroBot.firstUse[2] = false;
                                HydroBot.invincibleMode = true;
                                audio.armorSound.Play();
                                HydroBot.skillPrevUsed[2] = PoseidonGame.playTime.TotalSeconds;
                                HydroBot.currentHitPoint -= GameConstants.skillHealthLoss; // Lose health after useing this

                                //display HP loss
                                Point point = new Point();
                                String point_string = "-" + GameConstants.skillHealthLoss.ToString() + "HP";
                                point.LoadContent(Content, point_string, hydroBot.Position, Color.Black);
                                points.Add(point);

                                if (!hydroBot.clipPlayer.inRange(61, 90))
                                    hydroBot.clipPlayer.switchRange(61, 90);
                            }
                        }

                        //Hermes' Winged Sandal!!!
                        if (HydroBot.activeSkillID == 3)
                        {
                            if ((PoseidonGame.playTime.TotalSeconds - HydroBot.skillPrevUsed[3] > GameConstants.coolDownForHermesSandle) || HydroBot.firstUse[3] == true)
                            {
                                HydroBot.firstUse[3] = false;
                                audio.hermesSound.Play();
                                HydroBot.skillPrevUsed[3] = PoseidonGame.playTime.TotalSeconds;
                                HydroBot.supersonicMode = true;
                                HydroBot.currentHitPoint -= GameConstants.skillHealthLoss; // Lose health after useing this

                                //display HP loss
                                Point point = new Point();
                                String point_string = "-" + GameConstants.skillHealthLoss.ToString() + "HP";
                                point.LoadContent(Content, point_string, hydroBot.Position, Color.Black);
                                points.Add(point);

                                if (!hydroBot.clipPlayer.inRange(61, 90))
                                    hydroBot.clipPlayer.switchRange(61, 90);
                            }
                        }

                        // Hypnotise skill
                        if (HydroBot.activeSkillID == 4)
                        {
                            BaseEnemy enemy = CursorManager.MouseOnWhichEnemy(cursor, gameCamera, enemies, enemiesAmount);

                            if (enemy != null && (HydroBot.firstUse[4] == true || PoseidonGame.playTime.TotalSeconds - HydroBot.skillPrevUsed[4] > GameConstants.coolDownForHypnotise))
                            {
                                HydroBot.firstUse[4] = false;

                                enemy.setHypnotise(gameTime);

                                HydroBot.skillPrevUsed[4] = PoseidonGame.playTime.TotalSeconds;
                                HydroBot.currentHitPoint -= GameConstants.skillHealthLoss;

                                //display HP loss
                                Point point = new Point();
                                String point_string = "-" + GameConstants.skillHealthLoss.ToString() + "HP";
                                point.LoadContent(Content, point_string, hydroBot.Position, Color.Black);
                                points.Add(point);

                                audio.hipnotizeSound.Play();
                                if (!hydroBot.clipPlayer.inRange(61, 90))
                                    hydroBot.clipPlayer.switchRange(61, 90);
                            }
                        }

                        pointIntersect = Vector3.Zero;
                    }

                    //if the user holds down Ctrl button
                    //just shoot at wherever the mouse is pointing w/o moving
                    else if (currentKeyboardState.IsKeyDown(Keys.RightControl) || currentKeyboardState.IsKeyDown(Keys.LeftControl))
                    {
                        if (currentMouseState.LeftButton == ButtonState.Pressed)
                        {
                            pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);
                            hydroBot.ForwardDirection = CursorManager.CalculateAngle(pointIntersect, hydroBot.Position);
                            if (PoseidonGame.playTime.TotalSeconds - prevFireTime.TotalSeconds > GameConstants.MainCharBasicTimeBetweenFire.TotalSeconds / (HydroBot.shootingRate * HydroBot.fireRateUp))
                            {
                                prevFireTime = PoseidonGame.playTime;
                                //audio.Shooting.Play();
                                if (HydroBot.bulletType == 0) { AddingObjects.placeBotDamageBullet(hydroBot, Content, myBullet); }
                                else if (HydroBot.bulletType == 1) { AddingObjects.placeHealingBullet(hydroBot, Content, healthBullet); }
                                if (!hydroBot.clipPlayer.inRange(61, 90))
                                    hydroBot.clipPlayer.switchRange(61, 90);
                            }
                            //hydroBot.reachDestination = true;
                        }
                        pointIntersect = Vector3.Zero;
                        hydroBot.reachDestination = true;
                    }
                    //if the user clicks or holds mouse's left button
                    else if (currentMouseState.LeftButton == ButtonState.Pressed && !mouseOnLivingObject)
                    {
                        pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);
                        if (!hydroBot.clipPlayer.inRange(1, 30))
                            hydroBot.clipPlayer.switchRange(1, 30);
                    }

                    else if (lastMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released)
                    {
                        pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);
                        //if it is out of shooting range then just move there
                        if (!CursorManager.InShootingRange(hydroBot, cursor, gameCamera, GameConstants.MainGameFloatHeight))
                        {
                            if (!hydroBot.clipPlayer.inRange(1, 30))
                                hydroBot.clipPlayer.switchRange(1, 30);
                        }
                        else
                        {
                            //if the enemy is in the shooting range then shoot it w/o moving to it
                            if (mouseOnLivingObject && PoseidonGame.playTime.TotalSeconds - prevFireTime.TotalSeconds > GameConstants.MainCharBasicTimeBetweenFire.TotalSeconds / (HydroBot.shootingRate * HydroBot.fireRateUp))
                            {
                                hydroBot.ForwardDirection = CursorManager.CalculateAngle(pointIntersect, hydroBot.Position);
                                prevFireTime = PoseidonGame.playTime;
                                //audio.Shooting.Play();
                                if (HydroBot.bulletType == 0) { AddingObjects.placeBotDamageBullet(hydroBot, Content, myBullet); }
                                else if (HydroBot.bulletType == 1) { AddingObjects.placeHealingBullet(hydroBot, Content, healthBullet); }
                                //so the bot will not move
                                pointIntersect = Vector3.Zero;
                                hydroBot.reachDestination = true;
                                if (!hydroBot.clipPlayer.inRange(61, 90))
                                    hydroBot.clipPlayer.switchRange(61, 90);
                            }
                            if (doubleClicked == true) pointIntersect = Vector3.Zero;
                        }
                    }

                    //if the user holds down Caps Lock button
                    //lock the target inside shooting range
                    if (currentKeyboardState.IsKeyUp(Keys.CapsLock) && lastKeyboardState.IsKeyDown(Keys.CapsLock))
                    {
                        if (cursor.targetToLock == null)
                        {

                            Fish fishPointedAt = CursorManager.MouseOnWhichFish(cursor, gameCamera, fish, fishAmount);
                            if (fishPointedAt != null && cursor.targetToLock == null)
                            {
                                cursor.targetToLock = fishPointedAt;
                            }
                            else
                            {
                                BaseEnemy enemyPointedAt = CursorManager.MouseOnWhichEnemy(cursor, gameCamera, enemies, enemiesAmount);
                                if (enemyPointedAt != null && cursor.targetToLock == null)
                                    cursor.targetToLock = enemyPointedAt;
                            }
                        }
                        else cursor.targetToLock = null;
                    }

                    //let the user change active skill/bullet too when he presses on number
                    //this is better for fast action
                    InputManager.ChangeSkillBulletWithKeyBoard(lastKeyboardState, currentKeyboardState, hydroBot, GameMode.SurvivalMode);

                    if (HydroBot.supersonicMode == true)
                    {
                        pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);
                        CastSkill.KnockOutEnemies(gameTime, hydroBot, enemies, ref enemiesAmount, fish, fishAmount, audio, GameMode.SurvivalMode);
                    }
                    if (!heightMapInfo.IsOnHeightmap(pointIntersect)) pointIntersect = Vector3.Zero;
                    hydroBot.Update(currentKeyboardState, enemies, enemiesAmount, fish, fishAmount, fruits, null, gameTime, pointIntersect, GameMode.SurvivalMode);
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


                    //Are we planting trees?
                    if ((lastKeyboardState.IsKeyDown(Keys.X) && (currentKeyboardState.IsKeyUp(Keys.X))))
                    {
                        if (AddingObjects.placePlant(hydroBot, heightMapInfo, Content, plants, null, null, gameTime))
                        {
                            audio.plantSound.Play();
                            HydroBot.currentExperiencePts += Plant.experienceReward;
                            HydroBot.currentEnvPoint += GameConstants.envGainForDropSeed;

                            Point point = new Point();
                            String point_string = "+" + GameConstants.envGainForDropSeed.ToString() + "ENV\n+" + Plant.experienceReward + "EXP";
                            point.LoadContent(Content, point_string, hydroBot.Position, Color.LawnGreen);
                            points.Add(point);
                        }
                    }

                    //Are the trees ready for fruit?
                    foreach (Plant plant in plants)
                    {
                        if (plant.timeForFruit == true)
                        {
                            int powerType = random.Next(4) + 1;
                            Fruit fruit = new Fruit(powerType);
                            fruits.Add(fruit);
                            fruit.LoadContent(Content, plant.Position);
                            plant.timeForFruit = false;
                            plant.fruitCreated++;
                        }
                    }

                    gameCamera.Update(hydroBot.ForwardDirection, hydroBot.Position, aspectRatio, gameTime);
                    // Updating camera's frustum
                    frustum = new BoundingFrustum(gameCamera.ViewMatrix * gameCamera.ProjectionMatrix);

                    foreach (Fruit fruit in fruits)
                    {
                        fruit.Update(currentKeyboardState, hydroBot.BoundingSphere, hydroBot.Trash_Fruit_BoundingSphere);
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
                    Collision.updateDamageBulletVsBarriersCollision(myBullet, enemies, ref enemiesAmount, frustum, GameMode.SurvivalMode, gameTime);
                    Collision.updateHealingBulletVsBarrierCollision(healthBullet, fish, fishAmount, frustum, GameMode.SurvivalMode);
                    Collision.updateDamageBulletVsBarriersCollision(enemyBullet, fish, ref fishAmount, frustum, GameMode.SurvivalMode, gameTime);
                    Collision.updateProjectileHitBot(hydroBot, enemyBullet, GameMode.SurvivalMode);
                    Collision.updateDamageBulletVsBarriersCollision(alliesBullets, enemies, ref enemiesAmount, frustum, GameMode.SurvivalMode, gameTime);

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
                        enemies[i].Update(enemies, enemiesAmount, fish, fishAmount, random.Next(100), hydroBot, enemyBullet, alliesBullets, frustum, gameTime, GameMode.SurvivalMode);
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

                }

                prevGameState = currentGameState;
                if (currentGameState == GameState.Lost)
                {
                    // Return to main menu
                    if ((lastKeyboardState.IsKeyDown(Keys.Enter) &&
                        (currentKeyboardState.IsKeyUp(Keys.Enter))) ||
                        currentGamePadState.Buttons.Start == ButtonState.Pressed)
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

        /// <summary>
        /// Draws the game terrain, a simple blue grid.
        /// </summary>
        /// <param name="model">Model representing the game playing field.</param>
        private void DrawTerrain(Model model)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = Matrix.Identity;

                    // Use the matrices provided by the game camera
                    effect.View = gameCamera.ViewMatrix;
                    effect.Projection = gameCamera.ProjectionMatrix;

                    effect.FogEnabled = true;
                    effect.FogStart = GameConstants.FogStart;
                    effect.FogEnd = GameConstants.FogEnd;
                    effect.FogColor = GameConstants.FogColor.ToVector3();
                }
                mesh.Draw();
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
            DrawTerrain(ground.Model);
            // Updating camera's frustum
            frustum = new BoundingFrustum(gameCamera.ViewMatrix * gameCamera.ProjectionMatrix);
            foreach (Fruit f in fruits)
            {
                if (!f.Retrieved && f.BoundingSphere.Intersects(frustum))
                {
                    f.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
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

            for (int i = 0; i < enemiesAmount; i++)
            {
                if (enemies[i].BoundingSphere.Intersects(frustum))
                {
                    enemies[i].Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
                    if (enemies[i].stunned == true)
                    {
                        Vector3 placeToDraw = game.GraphicsDevice.Viewport.Project(enemies[i].Position, gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity);
                        Vector2 drawPos = new Vector2(placeToDraw.X, placeToDraw.Y);
                        spriteBatch.Begin();
                        spriteBatch.Draw(stunnedTexture, drawPos, Color.White);
                        spriteBatch.End();
                        RestoreGraphicConfig();
                    }
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
                    fish[i].Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
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

            for (int i = 0; i < myBullet.Count; i++)
            {
                myBullet[i].draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
            }

            for (int i = 0; i < healthBullet.Count; i++)
            {
                healthBullet[i].draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
            }

            for (int i = 0; i < enemyBullet.Count; i++)
            {
                enemyBullet[i].draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
            }

            for (int i = 0; i < alliesBullets.Count; i++)
            {
                alliesBullets[i].draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
            }


            // Draw each plant
            foreach (Plant p in plants)
            {
                if (p.BoundingSphere.Intersects(frustum))
                {
                    p.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, (float)((p.creationTime - roundTimer.TotalSeconds) / 10.0));
                    //RasterizerState rs = new RasterizerState();
                    //rs.FillMode = FillMode.WireFrame;
                    //GraphicDevice.RasterizerState = rs;
                    //p.DrawBoundingSphere(gameCamera.ViewMatrix,
                    //    gameCamera.ProjectionMatrix, boundingSphere);

                    //rs = new RasterizerState();
                    //rs.FillMode = FillMode.Solid;
                    //GraphicDevice.RasterizerState = rs;
                }
            }


            //fuelCarrier.Draw(gameCamera.ViewMatrix, 
            //    gameCamera.ProjectionMatrix);
            hydroBot.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
            //RasterizerState rs = new RasterizerState();
            //rs.FillMode = FillMode.WireFrame;
            //GraphicDevice.RasterizerState = rs;
            //hydroBot.DrawBoundingSphere(gameCamera.ViewMatrix,
            //    gameCamera.ProjectionMatrix, boundingSphere);

            //rs = new RasterizerState();
            //rs.FillMode = FillMode.Solid;
            //GraphicDevice.RasterizerState = rs;
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
                effectPost.CurrentTechnique.Passes[0].Apply();
                {
                    effectPost.Parameters["fTimer"].SetValue(m_Timer);
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
            radar.Draw(spriteBatch, hydroBot.Position, enemies, enemiesAmount, fish, fishAmount, null);
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
                AddingObjects.DrawHealthBar(HealthBar, game, spriteBatch, statsFont, (int)fishPointedAt.health, (int)fishPointedAt.maxHealth, 5, fishPointedAt.Name, Color.Red);
                string line;
                line = "'";
                if (fishPointedAt.health < 20)
                {
                    line += "SAVE ME!!!";
                }
                else if (fishPointedAt.health < 60)
                {
                    line += AddingObjects.wrapLine(fishPointedAt.sad_talk, HealthBar.Width + 20, fishTalkFont);
                }
                else
                {
                    line += AddingObjects.wrapLine(fishPointedAt.happy_talk, HealthBar.Width + 20, fishTalkFont);
                }
                line += "'";
                spriteBatch.DrawString(fishTalkFont, line, new Vector2(game.Window.ClientBounds.Width / 2 - HealthBar.Width / 2, 32), Color.Yellow);
            }

            //Display Enemy Health
            BaseEnemy enemyPointedAt = CursorManager.MouseOnWhichEnemy(cursor, gameCamera, enemies, enemiesAmount);
            if (enemyPointedAt != null)
                AddingObjects.DrawHealthBar(HealthBar, game, spriteBatch, statsFont, (int)enemyPointedAt.health, (int)enemyPointedAt.maxHealth, 5, enemyPointedAt.Name, Color.IndianRed);

            //Display Cyborg health
            AddingObjects.DrawHealthBar(HealthBar, game, spriteBatch, statsFont, (int)HydroBot.currentHitPoint, (int)HydroBot.maxHitPoint, game.Window.ClientBounds.Height - 60, "HEALTH", Color.Brown);

            //Display Environment Bar
            if (HydroBot.currentEnvPoint > HydroBot.maxEnvPoint) HydroBot.currentEnvPoint = HydroBot.maxEnvPoint;
            AddingObjects.DrawEnvironmentBar(EnvironmentBar, game, spriteBatch, statsFont, HydroBot.currentEnvPoint, HydroBot.maxEnvPoint);

            //Display Level/Experience Bar
            AddingObjects.DrawLevelBar(HealthBar, game, spriteBatch, statsFont, HydroBot.currentExperiencePts, HydroBot.nextLevelExperience, HydroBot.level, game.Window.ClientBounds.Height - 30, "EXPERIENCE LEVEL", Color.Brown);

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
            int xOffsetText, yOffsetText;
            Rectangle rectSafeArea;

            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            //xOffsetText = rectSafeArea.Right - 400;
            xOffsetText = rectSafeArea.Center.X + 150;
            yOffsetText = rectSafeArea.Bottom - 100;

            //Vector2 skillIconPosition =
            //    new Vector2((int)xOffsetText, (int)yOffsetText);
            Rectangle destRectangle = new Rectangle(xOffsetText, yOffsetText, 96, 96);

            //spriteBatch.Draw(skillTextures[tank.activeSkillID], skillIconPosition, Color.White);
            spriteBatch.Draw(skillTextures[HydroBot.activeSkillID], destRectangle, Color.White);

        }


        public static void RestoreGraphicConfig()
        {
            // Change back the config changed by spriteBatch
            GraphicDevice.BlendState = BlendState.Opaque;
            GraphicDevice.DepthStencilState = DepthStencilState.Default;
            GraphicDevice.SamplerStates[0] = SamplerState.LinearWrap;
            return;
        }
    }
}
