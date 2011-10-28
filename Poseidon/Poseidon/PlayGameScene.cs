using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Poseidon.Core;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Poseidon.FishSchool;


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
        public static GameTime timming;
        
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
        int retrievedFruits;
        public TimeSpan startTime, roundTimer, roundTime;
        Random random;
        SpriteBatch spriteBatch;
        SpriteFont statsFont;
        SpriteFont menuSmall;
        GameObject ground;
        public static Camera gameCamera;
        public GameState currentGameState = GameState.PlayingCutScene;
        // In order to know we are resetting the level winning or losing
        // winning: keep the current tank
        // losing: reset our tank to the tank at the beginning of the level
        GameState prevGameState;
        GameObject boundingSphere;

        public List<ShipWreck> shipWrecks;

        public List<DamageBullet> myBullet;
        public List<DamageBullet> alliesBullets;
        public List<DamageBullet> enemyBullet;
        public List<HealthBullet> healthBullet;

        List<Plant> plants;
        List<Fruit> fruits;
        List<Trash> trashes;

        List<StaticObject> staticObjects;

        public BaseEnemy[] enemies;
        public Fish[] fish;
        public int enemiesAmount = 0;
        public int fishAmount = 0;

        // The main character for this level
        public Tank tank;
        // The main character at the beginning of this level
        // Used for restarting the level
        //Tank prevTank;
        private TimeSpan fireTime;
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

        protected Texture2D levelObjectiveIconTexture;
        Rectangle levelObjectiveIconRectangle;

        // Current game level
        public static int currentLevel = 0;

        // Cutscene
        CutSceneDialog cutSceneDialog;
        // Which sentence in the dialog is being printed
        int currentSentence = 0;

        HeightMapInfo heightMapInfo;

        Radar radar;

        // Frustum of the camera
        public static BoundingFrustum frustum;

        // For mouse inputs
        bool doubleClicked = false;
        bool clicked = false;
        double clickTimer = 0;

        private Texture2D stunnedTexture;

        // to know whether the big boss has been terminated
        // and the level is won
        public static bool isBossKilled = false;

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

        // School of fish
        SchoolOfFish schoolOfFish1;
        SchoolOfFish schoolOfFish2;
        SchoolOfFish schoolOfFish3;

        // the fishes have helped the player to find the treasure chest key!
        // only show once
        bool showFoundKey = false;
        bool firstShow = true;

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
            this.stunnedTexture = stunnedTexture;
            roundTime = GameConstants.RoundTime;
            random = new Random();
            ground = new GameObject();
            gameCamera = new Camera(GameConstants.MainCamHeight);
            boundingSphere = new GameObject();
            tank = new Tank(GameConstants.MainGameMaxRangeX, GameConstants.MainGameMaxRangeZ, GameConstants.MainGameFloatHeight);
            //prevTank = new Tank(GameConstants.MainGameMaxRangeX, GameConstants.MainGameMaxRangeZ, GameConstants.MainGameFloatHeight);
            fireTime = TimeSpan.FromSeconds(0.3f);

            enemies = new BaseEnemy[GameConstants.NumberShootingEnemies[currentLevel] + GameConstants.NumberCombatEnemies[currentLevel]];
            fish = new Fish[GameConstants.NumberFish[currentLevel]];

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

            schoolOfFish1 = new SchoolOfFish(Content, "Image/FishSchoolTextures/smallfish1", 100, GameConstants.MainGameMaxRangeX - 250,
                100, GameConstants.MainGameMaxRangeZ - 250);
            schoolOfFish2 = new SchoolOfFish(Content, "Image/FishSchoolTextures/smallfish2-1", -GameConstants.MainGameMaxRangeX + 250, -100,
                -GameConstants.MainGameMaxRangeZ + 250, -100);
            schoolOfFish3 = new SchoolOfFish(Content, "Image/FishSchoolTextures/smallfish3", -GameConstants.MainGameMaxRangeX + 250, -100,
                100, GameConstants.MainGameMaxRangeZ - 250);
            this.Load();
        }

        public void Load()
        {
            statsFont = Content.Load<SpriteFont>("Fonts/StatsFont");
            menuSmall = Content.Load<SpriteFont>("Fonts/menuSmall");
            // Get the audio library
            audio = (AudioLibrary)
                Game.Services.GetService(typeof(AudioLibrary));

            //Uncomment below line to use LEVELS
            //string terrain_name = "Image/terrain" + currentLevel;
            
            //temporary code for testing
            Random random = new Random();
            int random_level = random.Next(20);
            string terrain_name = "Image/TerrainHeightMaps/terrain" + random_level;
            //System.Diagnostics.Debug.WriteLine(terrain_name);
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

            levelObjectiveIconTexture = Content.Load<Texture2D>("Image/Miscellaneous/LevelObjectiveIcon");

            //Initialize the game field
            InitializeGameField(Content);

            

            plants = new List<Plant>();
            fruits = new List<Fruit>();

            tank.Load(Content);

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
            MediaPlayer.Play(audio.BackMusic);
            //PlaceFuelCellsAndBarriers();
            base.Show();
        }

        private void ResetGame(GameTime gameTime, float aspectRatio)
        {
            isBossKilled = false;

            //Uncomment below line to use LEVELS
            //string terrain_name = "Image/terrain" + currentLevel;

            //temporary code for testing
            Random random = new Random();
            int random_level = random.Next(20);
            string terrain_name = "Image/TerrainHeightMaps/terrain" + random_level;
            //System.Diagnostics.Debug.WriteLine(terrain_name);
            //end temporary testing code

            ground.Model = Content.Load<Model>(terrain_name);

            // If we are resetting the level losing the game
            // Reset our tank to the one at the beginning of the lost level
            //if (prevGameState == GameState.Lost) tank.CopyAttribute(prevTank);
            //else prevTank.CopyAttribute(tank);

            if (prevGameState == GameState.Lost)
                tank.ResetToLevelStart();
            else
                tank.SetLevelStartValues();

            tank.Reset();
            gameCamera.Update(tank.ForwardDirection,
                tank.Position, aspectRatio, gameTime);

            

            //Clean all trees
            plants.Clear();

            //Clean all fruits
            fruits.Clear();


            retrievedFruits = 0;
            startTime = gameTime.TotalGameTime;
            roundTimer = roundTime;
            currentSentence = 0;
            currentGameState = GameState.PlayingCutScene;
            schoolOfFish1 = new SchoolOfFish(Content, "Image/FishSchoolTextures/smallfish1", 100, GameConstants.MainGameMaxRangeX - 250,
                100, GameConstants.MainGameMaxRangeZ - 250);
            schoolOfFish2 = new SchoolOfFish(Content, "Image/FishSchoolTextures/smallfish2-1", -GameConstants.MainGameMaxRangeX + 250, -100,
                -GameConstants.MainGameMaxRangeZ + 250, -100);
            schoolOfFish3 = new SchoolOfFish(Content, "Image/FishSchoolTextures/smallfish3", -GameConstants.MainGameMaxRangeX + 250, -100,
                100, GameConstants.MainGameMaxRangeZ - 250);
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
            enemies = new BaseEnemy[GameConstants.NumberShootingEnemies[currentLevel] + GameConstants.NumberCombatEnemies[currentLevel]
                + GameConstants.NumberMutantShark[currentLevel] + GameConstants.NumberTerminator[currentLevel]];
            fish = new Fish[GameConstants.NumberFish[currentLevel]];
            AddingObjects.placeEnemies(ref enemiesAmount, enemies, Content, random, fishAmount, fish, shipWrecks,
                GameConstants.MainGameMinRangeX, GameConstants.MainGameMaxRangeX, GameConstants.MainGameMinRangeZ, GameConstants.MainGameMaxRangeZ, currentLevel, true, GameConstants.MainGameFloatHeight);
            AddingObjects.placeFish(ref fishAmount, fish, Content, random, enemiesAmount, enemies, shipWrecks,
                GameConstants.MainGameMinRangeX, GameConstants.MainGameMaxRangeX, GameConstants.MainGameMinRangeZ, GameConstants.MainGameMaxRangeZ, currentLevel, true, GameConstants.MainGameFloatHeight);
            //placeFuelCells();
            AddingObjects.placeShipWreck(shipWrecks, staticObjects, random, heightMapInfo,
                GameConstants.MainGameMinRangeX, GameConstants.MainGameMaxRangeX, GameConstants.MainGameMinRangeZ, GameConstants.MainGameMaxRangeZ);
            
            //Initialize trash
            int random_model;
            trashes = new List<Trash>(GameConstants.NumberTrash[currentLevel]);
            for (int index = 0; index < GameConstants.NumberTrash[currentLevel]; index++)
            {
                random_model = random.Next(6);
                orientation = random.Next(100);
                trashes.Add(new Trash());
                switch (random_model)
                {
                    case 0:
                        trashes[index].LoadContent(Content, "Models/TrashModels/trashModel1", orientation);
                        break;
                    case 1:
                        trashes[index].LoadContent(Content, "Models/TrashModels/trashModel2", orientation);
                        break;
                    case 2:
                        trashes[index].LoadContent(Content, "Models/TrashModels/trashModel3", orientation);
                        break;
                    case 3:
                        trashes[index].LoadContent(Content, "Models/TrashModels/trashModel2", orientation);
                        break;
                    case 4:
                        trashes[index].LoadContent(Content, "Models/TrashModels/trashModel2", orientation);
                        break;
                    case 5:
                        trashes[index].LoadContent(Content, "Models/TrashModels/trashModel2", orientation);
                        break;
                }
                //trashes[index].LoadContent(Content, "Models/TrashModels/trashModel3", orientation);
            }
            AddingObjects.placeTrash(trashes, Content, random, shipWrecks,
                GameConstants.MainGameMinRangeX, GameConstants.MainGameMaxRangeX, GameConstants.MainGameMinRangeZ, 
                GameConstants.MainGameMaxRangeZ, currentLevel, true, GameConstants.MainGameFloatHeight, heightMapInfo); 

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
            AddingObjects.PlaceStaticObjects(staticObjects, shipWrecks, random, heightMapInfo, GameConstants.MainGameMinRangeX,
                GameConstants.MainGameMaxRangeX, GameConstants.MainGameMinRangeZ, GameConstants.MainGameMaxRangeZ);
        }

        /// <summary>
        /// Hide the scene
        /// </summary>
        public override void Hide()
        {
            // Stop the background music
            MediaPlayer.Stop();
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
            if (!paused)
            {
                timming = gameTime;
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

                
                if (currentGameState == GameState.PlayingCutScene)
                {
                    // Next sentence when the user press Enter
                    if ((lastKeyboardState.IsKeyDown(Keys.Enter) &&
                        (currentKeyboardState.IsKeyUp(Keys.Enter))) ||
                        currentGamePadState.Buttons.Start == ButtonState.Pressed)
                    {
                        currentSentence++;
                        // End of cutscene for this level
                        if (currentSentence == cutSceneDialog.cutScenes[currentLevel].Count)
                            currentGameState = GameState.Running;
                    }
                }
                if (currentGameState == GameState.ToNextLevel)
                {
                    ResetGame(gameTime, aspectRatio);
                }
                if ((currentGameState == GameState.Running))
                {
                    //if (currentLevel == 2 || currentLevel == 5 || currentLevel == 6 || currentLevel == 7 || currentLevel == 8)
                    //real one above, below is just for testing
                    if (currentLevel == 0 || currentLevel == 5 || currentLevel == 6 || currentLevel == 7 || currentLevel == 8)
                    {
                        if ((double)Tank.currentEnvPoint / (double)Tank.maxEnvPoint > GameConstants.EnvThresholdForKey)
                            showFoundKey = true;
                    }
                    if (showFoundKey && firstShow)
                    {
                        // return to game if enter pressed
                        if ((lastKeyboardState.IsKeyDown(Keys.Enter) &&
                            (currentKeyboardState.IsKeyUp(Keys.Enter))) ||
                            currentGamePadState.Buttons.Start == ButtonState.Pressed)
                        {
                            showFoundKey = false;
                            firstShow = false;
                        }
                        return;
                    }
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
                            Tank.bulletType++;
                            if (Tank.bulletType == GameConstants.numBulletTypes) Tank.bulletType = 0;
                            audio.ChangeBullet.Play();
                        }
                        // changing active skill
                        if ((lastKeyboardState.IsKeyDown(Keys.LeftShift) || lastKeyboardState.IsKeyDown(Keys.RightShift)) && ((lastKeyboardState.IsKeyDown(Keys.K)
                                && currentKeyboardState.IsKeyUp(Keys.K)) || (lastMouseState.RightButton == ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Released)))
                        {
                            if (Tank.activeSkillID != -1)
                            {
                                Tank.activeSkillID++;
                                if (Tank.activeSkillID == GameConstants.numberOfSkills) Tank.activeSkillID = 0;
                                while (Tank.skills[Tank.activeSkillID] == false)
                                {
                                    Tank.activeSkillID++;
                                    if (Tank.activeSkillID == GameConstants.numberOfSkills) Tank.activeSkillID = 0;
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
                        if (Tank.activeSkillID == 0 && mouseOnLivingObject)
                        {
                            pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);
                            tank.ForwardDirection = CursorManager.CalculateAngle(pointIntersect, tank.Position);
                            //if the skill has cooled down
                            //or this is the 1st time the user uses it
                            if ((gameTime.TotalGameTime.TotalSeconds - Tank.skillPrevUsed[0] > GameConstants.coolDownForHerculesBow) || Tank.firstUse[0] == true)
                            {
                                Tank.firstUse[0] = false;
                                Tank.skillPrevUsed[0] = gameTime.TotalGameTime.TotalSeconds;
                                audio.Explosion.Play();
                                CastSkill.UseHerculesBow(tank, Content, spriteBatch, myBullet, this);
                                Tank.currentHitPoint -= GameConstants.skillHealthLoss; // Lose health after useing this
                                tank.reachDestination = true;
                            }

                        }
                        //Thor's Hammer!!!
                        if (Tank.activeSkillID == 1)
                        {
                            if ((gameTime.TotalGameTime.TotalSeconds - Tank.skillPrevUsed[1] > GameConstants.coolDownForArchillesArmor) || Tank.firstUse[1] == true)
                            {
                                Tank.firstUse[1] = false;
                                Tank.skillPrevUsed[1] = gameTime.TotalGameTime.TotalSeconds;
                                audio.Explo1.Play();
                                gameCamera.Shake(25f, .4f);
                                CastSkill.UseThorHammer(gameTime, tank, enemies, ref enemiesAmount, fish, fishAmount);
                                Tank.currentHitPoint -= GameConstants.skillHealthLoss; // Lose health after useing this
                            }
                        }
                        // Achilles' Armor!!!
                        if (Tank.activeSkillID == 2)
                        {
                            if ((gameTime.TotalGameTime.TotalSeconds - Tank.skillPrevUsed[2] > GameConstants.coolDownForThorHammer) || Tank.firstUse[2] == true)
                            {
                                Tank.firstUse[2] = false;
                                Tank.invincibleMode = true;
                                audio.NewMeteor.Play();
                                Tank.skillPrevUsed[2] = gameTime.TotalGameTime.TotalSeconds;
                                Tank.currentHitPoint -= GameConstants.skillHealthLoss; // Lose health after useing this
                            }
                        }

                        //Hermes' Winged Sandal!!!
                        if (Tank.activeSkillID == 3)
                        {
                            if ((gameTime.TotalGameTime.TotalSeconds - Tank.skillPrevUsed[3] > GameConstants.coolDownForHermesSandle) || Tank.firstUse[3] == true)
                            {
                                Tank.firstUse[3] = false;
                                audio.NewMeteor.Play();
                                Tank.skillPrevUsed[3] = gameTime.TotalGameTime.TotalSeconds;
                                Tank.supersonicMode = true;
                                Tank.currentHitPoint -= GameConstants.skillHealthLoss; // Lose health after useing this
                            }
                        }

                        // Hypnotise skill
                        if (Tank.activeSkillID == 4)
                        {
                            BaseEnemy enemy = CursorManager.MouseOnWhichEnemy(cursor, gameCamera, enemies, enemiesAmount);

                            if (enemy == null)
                            {
                                return;
                            }

                            if (Tank.firstUse[3] == true || gameTime.TotalGameTime.TotalSeconds - Tank.skillPrevUsed[4] > GameConstants.coolDownForHypnotise)
                            {
                                Tank.firstUse[4] = false;

                                enemy.setHypnotise();

                                Tank.skillPrevUsed[4] = gameTime.TotalGameTime.TotalSeconds;
                                Tank.currentHitPoint -= GameConstants.skillHealthLoss;
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
                            tank.ForwardDirection = CursorManager.CalculateAngle(pointIntersect, tank.Position);
                            if (gameTime.TotalGameTime.TotalSeconds - prevFireTime.TotalSeconds > fireTime.TotalSeconds / (Tank.shootingRate * Tank.fireRateUp))
                            {
                                prevFireTime = gameTime.TotalGameTime;
                                audio.Shooting.Play();
                                if (Tank.bulletType == 0) { AddingObjects.placeTankDamageBullet(tank, Content, myBullet); }
                                else if (Tank.bulletType == 1) { AddingObjects.placeHealingBullet(tank, Content, healthBullet); }
                            }
                        }
                        pointIntersect = Vector3.Zero;
                    }

                    //if the user clicks or holds mouse's left button
                    else if (currentMouseState.LeftButton == ButtonState.Pressed && !mouseOnLivingObject)
                    {
                        pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);
                    }
                    else if (lastMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released)
                    {
                        pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);
                        //if it is out of shooting range then just move there
                        if (!CursorManager.InShootingRange(tank, cursor, gameCamera, GameConstants.MainGameFloatHeight))
                        {

                        }
                        else
                        {
                            //if the enemy is in the shooting range then shoot it w/o moving to it
                            if (mouseOnLivingObject && gameTime.TotalGameTime.TotalSeconds - prevFireTime.TotalSeconds > fireTime.TotalSeconds / (Tank.shootingRate * Tank.fireRateUp))
                            {
                                tank.ForwardDirection = CursorManager.CalculateAngle(pointIntersect, tank.Position);
                                prevFireTime = gameTime.TotalGameTime;
                                audio.Shooting.Play();
                                if (Tank.bulletType == 0) { AddingObjects.placeTankDamageBullet(tank, Content, myBullet); }
                                else if (Tank.bulletType == 1) { AddingObjects.placeHealingBullet(tank, Content, healthBullet); }
                                //so the tank will not move
                                pointIntersect = Vector3.Zero;
                                tank.reachDestination = true;
                            }
                            if (doubleClicked == true) pointIntersect = Vector3.Zero;
                        }
                    }
                    //let the user change active skill/bullet too when he presses on number
                    //this is better for fast action
                    InputManager.ChangeSkillBulletWithKeyBoard(lastKeyboardState, currentKeyboardState, tank);

                    if (Tank.supersonicMode == true)
                    {
                        pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);
                        CastSkill.KnockOutEnemies(gameTime, tank, enemies, ref enemiesAmount, fish, fishAmount, audio);
                    }
                    if (!heightMapInfo.IsOnHeightmap(pointIntersect)) pointIntersect = Vector3.Zero;
                    tank.Update(currentKeyboardState, enemies, enemiesAmount, fish, fishAmount, fruits, trashes, gameTime, pointIntersect);
                    //add 1 bubble over tank and each enemy
                    timeNextBubble -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (timeNextBubble <= 0)
                    {
                        Bubble bubble = new Bubble();
                        bubble.LoadContent(Content, tank.Position, false, 0.025f);
                        bubbles.Add(bubble);
                        for (int i = 0; i < enemiesAmount; i++)
                        {
                            if (enemies[i].BoundingSphere.Intersects(frustum))
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
                        Vector3 bubblePos = tank.Position;
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
                        aBubble.Update(GraphicDevice, gameCamera);
                    }
                    // Are we shooting?
                    if (!(lastKeyboardState.IsKeyDown(Keys.LeftShift) || lastKeyboardState.IsKeyDown(Keys.RightShift))
                        && currentKeyboardState.IsKeyDown(Keys.L)
                        && gameTime.TotalGameTime.TotalSeconds - prevFireTime.TotalSeconds > fireTime.TotalSeconds / (Tank.shootingRate * Tank.fireRateUp))
                    //||
                    //( (MouseOnEnemy()||MouseOnFish()) && lastMouseState.LeftButton==ButtonState.Pressed && currentMouseState.LeftButton==ButtonState.Released && InShootingRange())
                    {
                        prevFireTime = gameTime.TotalGameTime;
                        audio.Shooting.Play();
                        if (Tank.bulletType == 0) { AddingObjects.placeTankDamageBullet(tank, Content, myBullet); }
                        else if (Tank.bulletType == 1) { AddingObjects.placeHealingBullet(tank, Content, healthBullet); }
                    }


                    //Are we planting trees?
                    if ((lastKeyboardState.IsKeyDown(Keys.X) && (currentKeyboardState.IsKeyUp(Keys.X))))
                    {
                        if (AddingObjects.placePlant(tank, heightMapInfo, Content, roundTimer, plants, shipWrecks, staticObjects, gameTime))
                        {
                            audio.PowerShow.Play();
                            Tank.currentExperiencePts += Plant.experienceReward;
                            Tank.currentEnvPoint += GameConstants.envGainForDropSeed;
                        }
                    }

                    //Are the trees ready for fruit?
                    foreach (Plant plant in plants)
                    {
                        if (plant.timeForFruit == true)
                        {
                            int powerType = random.Next(3) + 1;
                            Fruit fruit = new Fruit(powerType);
                            fruits.Add(fruit);
                            fruit.LoadContent(Content, plant.Position);
                            plant.timeForFruit = false;
                            plant.fruitCreated++;
                        }
                    }

                    gameCamera.Update(tank.ForwardDirection, tank.Position, aspectRatio, gameTime);
                    // Updating camera's frustum
                    frustum = new BoundingFrustum(gameCamera.ViewMatrix * gameCamera.ProjectionMatrix);

                    retrievedFruits = 0;
                    foreach (Fruit fruit in fruits)
                    {
                        fruit.Update(currentKeyboardState, tank.BoundingSphere, tank.Trash_Fruit_BoundingSphere);
                        if (fruit.Retrieved)
                        {
                            retrievedFruits++;
                        }
                    }

                    foreach (Trash trash in trashes)
                    {
                        trash.Update(gameTime);
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
                    Collision.updateBulletOutOfBound(tank.MaxRangeX, tank.MaxRangeZ, healthBullet, myBullet, enemyBullet, alliesBullets, frustum);
                    Collision.updateDamageBulletVsBarriersCollision(myBullet, enemies, ref enemiesAmount);
                    Collision.updateHealingBulletVsBarrierCollision(healthBullet, fish, fishAmount);
                    Collision.updateDamageBulletVsBarriersCollision(enemyBullet, fish, ref fishAmount);
                    Collision.updateProjectileHitTank(tank, enemyBullet);
                    Collision.updateDamageBulletVsBarriersCollision(alliesBullets, enemies, ref enemiesAmount);

                    Collision.deleteSmallerThanZero(enemies, ref enemiesAmount);
                    Collision.deleteSmallerThanZero(fish, ref fishAmount);

                    for (int i = 0; i < enemiesAmount; i++)
                    {
                        //disable stun if stun effect times out
                        if (enemies[i].stunned)
                        {
                            if (gameTime.TotalGameTime.TotalSeconds - enemies[i].stunnedStartTime > GameConstants.timeStunLast)
                                enemies[i].stunned = false;
                        }
                        enemies[i].Update(enemies, enemiesAmount, fish, fishAmount, random.Next(100), tank, enemyBullet, alliesBullets);
                    }

                    for (int i = 0; i < fishAmount; i++)
                    {
                        fish[i].Update(gameTime, enemies, enemiesAmount, fish, fishAmount, random.Next(100), tank, enemyBullet);
                    }

                    //Checking win/lost condition for this level
                    if (Tank.currentHitPoint <= 0) { currentGameState = GameState.Lost; }

                    roundTimer -= gameTime.ElapsedGameTime;
                    if (CheckWinCondition()) currentGameState = GameState.Won;
                    if (CheckLoseCondition()) currentGameState = GameState.Lost;

                    //for the shader
                    m_Timer += (float)gameTime.ElapsedGameTime.Milliseconds / 1000;

                    //cursor update
                    cursor.Update(gameTime);

                    //update the school of fish
                    schoolOfFish1.Update(gameTime, tank, enemies, enemiesAmount, fish, fishAmount);
                    schoolOfFish2.Update(gameTime, tank, enemies, enemiesAmount, fish, fishAmount);
                    schoolOfFish3.Update(gameTime, tank, enemies, enemiesAmount, fish, fishAmount);
                }

                prevGameState = currentGameState;
                if (currentGameState == GameState.Lost)
                {
                    // Reset the world for a new game
                    if ((lastKeyboardState.IsKeyDown(Keys.Enter) &&
                        (currentKeyboardState.IsKeyUp(Keys.Enter))) ||
                        currentGamePadState.Buttons.Start == ButtonState.Pressed)
                    {
                        //the player should always lose in level 9
                        if (currentLevel == 9)
                        {
                            currentLevel++;
                        }
                        //always reset the level when losing
                        ResetGame(gameTime, aspectRatio);
                    }
                }
                if (currentGameState == GameState.Won)
                {
                    if ((lastKeyboardState.IsKeyDown(Keys.Enter) &&
                        (currentKeyboardState.IsKeyUp(Keys.Enter))) ||
                        currentGamePadState.Buttons.Start == ButtonState.Pressed)
                    {
                        currentLevel++;
                        currentGameState = GameState.ToMiniGame;
                        //ResetGame(gameTime, aspectRatio);
                    }
                }
                base.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {

            if (showFoundKey && firstShow)
            {
                DrawFoundKey();
                return;
            }
            if (paused)
            {
                // Draw the "pause" text
                spriteBatch.Begin();
                spriteBatch.Draw(actionTexture, pausePosition, pauseRect,
                    Color.White);
                spriteBatch.End();
            }
            switch (currentGameState)
            {
                case GameState.PlayingCutScene:
                    spriteBatch.Begin();
                    DrawCutScene();
                    spriteBatch.End();
                    break;
                case GameState.Running:
                    RestoreGraphicConfig();
                    DrawGameplayScreen();
                    break;
                case GameState.Won:
                    DrawWinOrLossScreen(GameConstants.StrGameWon);
                    break;
                case GameState.Lost:
                    DrawWinOrLossScreen(GameConstants.StrGameLost);
                    break;
            }
            base.Draw(gameTime);

        }
        private void DrawFoundKey()
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(statsFont, "The fishes have helped you to find the hidden key to treasure chests in return for your help", new Vector2(0, 0), Color.White);
            spriteBatch.End();
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

        private void DrawWinOrLossScreen(string gameResult)
        {
            spriteBatch.Begin();
            float xOffsetText, yOffsetText;
            Vector2 viewportSize = new Vector2(GraphicDevice.Viewport.Width,
                GraphicDevice.Viewport.Height);
            Vector2 strCenter;

            xOffsetText = yOffsetText = 0;
            Vector2 strResult = statsFont.MeasureString(gameResult);
            Vector2 strPlayAgainSize =
                statsFont.MeasureString(GameConstants.StrPlayAgain);
            Vector2 strPosition;
            strCenter = new Vector2(strResult.X / 2, strResult.Y / 2);

            yOffsetText = (viewportSize.Y / 2 - strCenter.Y);
            xOffsetText = (viewportSize.X / 2 - strCenter.X);
            strPosition = new Vector2((int)xOffsetText, (int)yOffsetText);

            spriteBatch.DrawString(statsFont, gameResult,
                strPosition, Color.Red);

            strCenter =
                new Vector2(strPlayAgainSize.X / 2, strPlayAgainSize.Y / 2);
            yOffsetText = (viewportSize.Y / 2 - strCenter.Y) +
                (float)statsFont.LineSpacing;
            xOffsetText = (viewportSize.X / 2 - strCenter.X);
            strPosition = new Vector2((int)xOffsetText, (int)yOffsetText);
            spriteBatch.DrawString(statsFont, GameConstants.StrPlayAgain,
                strPosition, Color.AntiqueWhite);
            spriteBatch.End();
        }

        private void DrawGameplayScreen()
        {
            graphics.GraphicsDevice.SetRenderTarget(renderTarget);
            graphics.GraphicsDevice.Clear(Color.DarkSlateBlue);
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

            for (int i = 0; i < enemyBullet.Count; i++) {
                enemyBullet[i].draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
            }

            for (int i = 0; i < alliesBullets.Count; i++) {
                alliesBullets[i].draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
            }
            BoundingSphere shipSphere;
            // Drawing ship wrecks
            foreach (ShipWreck shipWreck in shipWrecks)
            {
                shipSphere = shipWreck.BoundingSphere;
                shipSphere.Center = shipWreck.Position;
                if (shipSphere.Intersects(frustum))
                {
                    shipWreck.Draw(gameCamera.ViewMatrix,
                        gameCamera.ProjectionMatrix);
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

            // Draw each plant
            foreach (Plant p in plants) {
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
            // Drawing trash
            BoundingSphere trashRealSphere;
            foreach (Trash trash in trashes)
            {
                trashRealSphere = trash.BoundingSphere;
                trashRealSphere.Center.Y = trash.Position.Y;
                if (!trash.Retrieved && trashRealSphere.Intersects(frustum))
                {
                    trash.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
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
            //fuelCarrier.Draw(gameCamera.ViewMatrix, 
            //    gameCamera.ProjectionMatrix);
            tank.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
            //RasterizerState rs = new RasterizerState();
            //rs.FillMode = FillMode.WireFrame;
            //GraphicsDevice.RasterizerState = rs;
            //tank.DrawBoundingSphere(gameCamera.ViewMatrix,
            //    gameCamera.ProjectionMatrix, boundingSphere);

            // draw bubbles
            foreach (Bubble bubble in bubbles)
            {
                bubble.Draw(spriteBatch, 1.0f);
            }

            //draw schools of fish
            spriteBatch.Begin();
            schoolOfFish1.Draw(timming, spriteBatch);
            schoolOfFish2.Draw(timming, spriteBatch);
            schoolOfFish3.Draw(timming, spriteBatch);
            spriteBatch.End();

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
            if (Tank.activeSkillID != -1) DrawActiveSkill();
            DrawLevelObjectiveIcon();
            cursor.Draw(timming);
            spriteBatch.End();
        }

        private void DrawRadar()
        {
            radar.Draw(spriteBatch, tank.Position, enemies, enemiesAmount, fish, fishAmount, staticObjects);
        }

        public bool CharacterNearShipWreck(BoundingSphere shipSphere)
        {
            if (tank.BoundingSphere.Intersects(shipSphere))
                return true;
            else
                return false;
        }

        private void DrawHeight()
        {
            //float xOffsetText, yOffsetText;
            //string str1 = " Height: " + heightMapInfo.GetHeight(tank.Position);
            //Rectangle rectSafeArea;
            //Ray cursorRay = cursor.CalculateCursorRay(gameCamera.ProjectionMatrix, gameCamera.ViewMatrix);
            //BoundingSphere boundingSphere;
            //foreach (ShipWreck shipWreck in shipWrecks)
            //{
            //    boundingSphere = shipWreck.BoundingSphere;
            //    boundingSphere.Center = shipWreck.Position;
            //    if (CursorManager.RayIntersectsBoundingSphere(cursorRay, boundingSphere))
            //        str1 += " Pointing to ship wreck ";

            //}

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
            string str1 = GameConstants.StrTimeRemaining;
            string str2 = "";// = GameConstants.StrCellsFound + retrievedFruits.ToString() +
            //" of " + fruits.Count;
            Rectangle rectSafeArea;
            if (roundTimer.Minutes < 10)
                str1 += "0";
            str1 += roundTimer.Minutes + ":";
            if (roundTimer.Seconds < 10)
                str1+= "0";
            str1 += roundTimer.Seconds;
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
            str2 += "School1 " + schoolOfFish1.flock.flock.Count + " School2 " + schoolOfFish2.flock.flock.Count;
            str2 += "\n" + schoolOfFish1.flock.flock[0].Location + "\n" + schoolOfFish2.flock.flock[0].Location;
            //str2 += "\n" + schoolOfFish1.flock.flock[1].texture.Name.Length + "\n" + schoolOfFish2.flock.flock[1].texture.Name;
            //Display Fish Health
            Fish fishPointedAt = CursorManager.MouseOnWhichFish(cursor, gameCamera, fish, fishAmount);
            if (fishPointedAt != null)
                AddingObjects.DrawHealthBar(HealthBar, game, spriteBatch, statsFont, fishPointedAt.health, fishPointedAt.maxHealth, 5, fishPointedAt.Name, Color.Red);

            //Display Enemy Health
            BaseEnemy enemyPointedAt = CursorManager.MouseOnWhichEnemy(cursor, gameCamera, enemies, enemiesAmount);
            if (enemyPointedAt != null)
                AddingObjects.DrawHealthBar(HealthBar, game, spriteBatch, statsFont, enemyPointedAt.health, enemyPointedAt.maxHealth, 5, enemyPointedAt.Name, Color.IndianRed);

            //Display Cyborg health
            AddingObjects.DrawHealthBar(HealthBar, game, spriteBatch, statsFont, Tank.currentHitPoint, Tank.maxHitPoint, game.Window.ClientBounds.Height-60, "HEALTH", Color.Brown);

            //Display Environment Bar
            if (Tank.currentEnvPoint > Tank.maxEnvPoint) Tank.currentEnvPoint = Tank.maxEnvPoint;
            AddingObjects.DrawEnvironmentBar(EnvironmentBar, game, spriteBatch, statsFont, Tank.currentEnvPoint, Tank.maxEnvPoint);

            //Display Level/Experience Bar
            AddingObjects.DrawLevelBar(HealthBar, game, spriteBatch, statsFont, Tank.currentExperiencePts, Tank.nextLevelExperience, Tank.level, game.Window.ClientBounds.Height-30, "EXPERIENCE LEVEL", Color.GreenYellow);

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
            xOffsetText = rectSafeArea.Center.X - 150 -64;
            yOffsetText = rectSafeArea.Bottom - 80;

            //Vector2 bulletIconPosition =
            //    new Vector2((int)xOffsetText, (int)yOffsetText);
            Rectangle destRectangle = new Rectangle(xOffsetText, yOffsetText, 64, 64);
            //spriteBatch.Draw(bulletTypeTextures[tank.bulletType], bulletIconPosition, Color.White);
            spriteBatch.Draw(bulletTypeTextures[Tank.bulletType], destRectangle, Color.White);
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
            spriteBatch.Draw(skillTextures[Tank.activeSkillID], destRectangle, Color.White);
            
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

        public bool mouseOnLevelObjectiveIcon(MouseState lmouseState)
        {
            if(levelObjectiveIconRectangle.Intersects(new Rectangle(lmouseState.X, lmouseState.Y, 10, 10)))
                return true;
            else
                return false;
        }

        private void DrawCutScene()
        {
            float xOffsetText, yOffsetText;
            string str1 = cutSceneDialog.cutScenes[currentLevel][currentSentence].sentence;
            Rectangle rectSafeArea;
            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            xOffsetText = rectSafeArea.X;
            yOffsetText = rectSafeArea.Y;

            Vector2 strSize = statsFont.MeasureString(str1);
            Vector2 strPosition =
                new Vector2((int)xOffsetText + 10, (int)yOffsetText);
            spriteBatch.DrawString(statsFont, str1, strPosition, Color.White);
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
