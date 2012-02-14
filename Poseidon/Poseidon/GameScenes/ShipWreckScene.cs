using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Poseidon.Core;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Poseidon.GraphicEffects;

namespace Poseidon
{
    /// <summary>
    /// This is a game component that implements the Action Scene.
    /// </summary>
    public partial class ShipWreckScene : GameScene {
        GraphicsDeviceManager graphics;
        public static GraphicsDevice GraphicDevice;
        ContentManager Content;

        private Texture2D HealthBar;

        Game game;
        KeyboardState lastKeyboardState = new KeyboardState();
        KeyboardState currentKeyboardState = new KeyboardState();
        MouseState currentMouseState = new MouseState();
        MouseState lastMouseState = new MouseState();
        private AudioLibrary audio;

        public TimeSpan roundTimer;

        Random random;
        SpriteBatch spriteBatch;
        SpriteFont statsFont;
        SpriteFont paintingFont;
        SpriteFont menuSmall;
        GameObject ground;
        public static Camera gameCamera;
        
        GameObject boundingSphere;

        //objects that must be remembered for each shipwreck
        //List<bool> shipAccessed;
        //List<BaseEnemy[]> enemies;
        //List<Fish[]> fishes;
        //List<int> enemiesAmount;// = 0;
        //List<int> fishAmount;// = 0;

        //List<List<TreasureChest>> treasureChests;
        //List<List<StaticObject>> staticObjects;
        //List<bool> foundRelic;// = false;

        bool[] shipAccessed;
        BaseEnemy[][] enemies;
        Fish[][] fishes;
        int[] enemiesAmount;
        int[] fishAmount;
        List<TreasureChest>[] treasureChests;
        List<StaticObject>[] staticObjects;
        // draw a cutscene when finding a god's relic
        bool[] foundRelic;// = false;
        // has artifact?
        public int skillID;

        List<DamageBullet> myBullet;
        List<DamageBullet> enemyBullet;
        List<HealthBullet> healthBullet;
        List<DamageBullet> alliesBullets;


        public int currentShipWreckID;
        public static bool resetShipWreckNow = true;
        public bool backFromAttributeBoard = false;

        //the hydrobot
        public HydroBot hydroBot;


        // For drawing the currently selected skill
        protected Texture2D[] skillTextures;
        // For drawing the currently selected bullet type
        protected Texture2D[] bulletTypeTextures;
        // Game is paused?
        protected bool paused;
        protected Vector2 pausePosition;
        protected Rectangle pauseRect = new Rectangle(1, 120, 200, 44);
        protected Texture2D actionTexture;
        protected Texture2D stunnedIconTexture, scaredIconTexture;
        protected Texture2D gameObjectiveIconTexture;
        protected Texture2D noKeyScreen;
        protected Texture2D skillFoundScreen;
        // He died inside the ship wreck?
        public bool returnToMain;

        // Frustum of the camera
        BoundingFrustum frustum;

        // For mouse inputs
        bool doubleClicked = false;
        bool clicked = false;
        double clickTimer = 0;

        float m_Timer = 0;
        public RenderTarget2D renderTarget, afterEffectsAppliedRenderTarget, cutSceneImmediateRenderTarget;
        public Texture2D SceneTexture, Scene2Texture;
        public bool screenTransitNow = false;

        // showing paintings when openning treasure chests
        OceanPaintings oceanPaintings;
        bool showPainting = false;
        int paintingToShow = 0;
        bool showNoKey = false;
        // showing strange objects found scene
        bool showStrangeObjectFound = false;
        Texture2D[] fossilTextures;
        int fossilToDraw;

        // Bubbles over characters
        List<Bubble> bubbles;
        float timeNextBubble = 200.0f;
        //float timeNextSeaBedBubble = 3000.0f;

        //Points gained
        public static List<Point> points;

        // For applying graphic effects
        GraphicEffect graphicEffect;
        //for particle systems
        public static ParticleManagement particleManager;

        //for edge detection effect
        RenderTarget2D normalDepthRenderTarget, edgeDetectionRenderTarget;

        public ShipWreckScene(Game game, GraphicsDeviceManager graphics, ContentManager Content, GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch, Vector2 pausePosition, Rectangle pauseRect, Texture2D actionTexture, CutSceneDialog cutSceneDialog, Texture2D stunnedTexture)
            : base(game)
        {
            this.graphics = graphics;
            this.Content = Content;
            GraphicDevice = GraphicsDevice;
            this.spriteBatch = spriteBatch;
            this.pausePosition = pausePosition;
            this.pauseRect = pauseRect;
            this.actionTexture = actionTexture;
            this.game = game;
            this.stunnedIconTexture = stunnedTexture;
            random = new Random();
            ground = new GameObject();
            gameCamera = new Camera(GameConstants.ShipCamHeight);
            boundingSphere = new GameObject();
            hydroBot = new HydroBot(GameConstants.ShipWreckMaxRangeX, GameConstants.ShipWreckMaxRangeZ, GameConstants.ShipWreckFloatHeight, GameMode.ShipWreck);
            //fireTime = TimeSpan.FromSeconds(0.3f);
            //if (PlayGameScene.currentLevel >= 4)
            //    enemiesAmount = GameConstants.ShipHighNumberShootingEnemies + GameConstants.ShipHighNumberCombatEnemies;
            //else enemiesAmount = GameConstants.ShipLowNumberShootingEnemies + GameConstants.ShipLowNumberCombatEnemies;
            //enemies = new BaseEnemy[enemiesAmount];
            //fish = new Fish[GameConstants.ShipNumberFish];
            skillTextures = new Texture2D[GameConstants.numberOfSkills];
            bulletTypeTextures = new Texture2D[GameConstants.numBulletTypes];

            //enemies = new Enemy[GameConstants.NumberEnemies];
            //fish = new Fish[GameConstants.NumberFish];
            
            // for the mouse or touch
            cursor = new Cursor(game, spriteBatch);
            //Components.Add(cursor);

            myBullet = new List<DamageBullet>();
            healthBullet = new List<HealthBullet>();
            enemyBullet = new List<DamageBullet>();
            alliesBullets = new List<DamageBullet>();
            
            //for paintings inside treasure chests
            oceanPaintings = new OceanPaintings(Content);

            bubbles = new List<Bubble>();
            points = new List<Point>();

            fossilTextures = new Texture2D[4];
            fossilTextures[0] = Content.Load<Texture2D>("Image/Fossils/fossil1");
            fossilTextures[1] = Content.Load<Texture2D>("Image/Fossils/fossil2");
            fossilTextures[2] = Content.Load<Texture2D>("Image/Fossils/fossil3");
            fossilTextures[3] = Content.Load<Texture2D>("Image/Fossils/fossil4");

            

            this.Load();
        }


        public void Load()
        {
            statsFont = Content.Load<SpriteFont>("Fonts/StatsFont");
            menuSmall = Content.Load<SpriteFont>("Fonts/menuSmall");
            paintingFont = Content.Load<SpriteFont>("Fonts/painting");
            // Get the audio library
            audio = (AudioLibrary)
                Game.Services.GetService(typeof(AudioLibrary));

            //Random random = new Random();
            //int random_terrain = random.Next(5);
            //string wood_terrain_name = "Image/wood-terrain" + random_terrain;

            //ground.Model = Content.Load<Model>(wood_terrain_name);
            ground.Model = Content.Load<Model>("Models/ShipWreckModels/shipwreckscene");
            boundingSphere.Model = Content.Load<Model>("Models/Miscellaneous/sphere1uR");
            //heightMapInfo = ground.Model.Tag as HeightMapInfo;
            //if (heightMapInfo == null)
            //{
            //    string message = "The terrain model did not have a HeightMapInfo " +
            //        "object attached. Are you sure you are using the " +
            //        "TerrainProcessor?";
            //    throw new InvalidOperationException(message);
            //}
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

            //Initialize fuel cells
            //fuelCells = new List<FuelCell>(GameConstants.NumFuelCells);
            //int powerType = random.Next(3) + 1;
            //for (int index = 0; index < GameConstants.NumFuelCells; index++)
            //{
            //    fuelCells.Add(new FuelCell(powerType));
            //    fuelCells[index].LoadContent(Content, "Models/fuelcell");
            //    powerType = random.Next(3) + 1;
            //}

            //Initialize the game field
            //InitializeShipField(Content);

            hydroBot.Load(Content);

            //Load healthbar
            HealthBar = Content.Load<Texture2D>("Image/Miscellaneous/HealthBar");

            noKeyScreen = Content.Load<Texture2D>("Image/SceneTextures/no_key");

            skillFoundScreen = Content.Load<Texture2D>("Image/SceneTextures/skillFoundBackground");
            scaredIconTexture = Content.Load<Texture2D>("Image/Miscellaneous/scared-icon");

            PresentationParameters pp = graphics.GraphicsDevice.PresentationParameters;
            renderTarget = new RenderTarget2D(graphics.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight,
                false, graphics.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24Stencil8);

            //for edge detection effect
            normalDepthRenderTarget = new RenderTarget2D(graphics.GraphicsDevice,
                                                         pp.BackBufferWidth, pp.BackBufferHeight, false,
                                                         pp.BackBufferFormat, pp.DepthStencilFormat);
            edgeDetectionRenderTarget = new RenderTarget2D(graphics.GraphicsDevice,
                                                         pp.BackBufferWidth, pp.BackBufferHeight, false,
                                                         pp.BackBufferFormat, pp.DepthStencilFormat);
            cutSceneImmediateRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight,
                false, graphics.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24Stencil8);

            graphicEffect = new GraphicEffect(this, this.spriteBatch, statsFont);
            graphicEffect.fadeTransitAmount = 0.025f;
            // Construct our particle system components.
            particleManager = new ParticleManagement(this.game, GraphicDevice);
        }

        /// <summary>
        /// Show the action scene
        /// </summary>

        public override void Show()
        {
            paused = false;
            //initialize random shipwreck terrain
            //Random random = new Random();
            //int random_terrain = random.Next(5);
            //string wood_terrain_name = "Image/wood-terrain" + random_terrain;

            //ground.Model = Content.Load<Model>(wood_terrain_name);
            //MediaPlayer.Play(audio.BackMusic);
            showNoKey = false;
            showPainting = false;
            cursor.targetToLock = null;
            returnToMain = false;
            HydroBot.gameMode = GameMode.ShipWreck;

            if (resetShipWreckNow == true)
            {
                CreateNewShipWreckVariables();
                resetShipWreckNow = false;
            }
            if (!backFromAttributeBoard)
            {
                ResetShipWreckGeneralVariables();
            }
            if (shipAccessed[currentShipWreckID] == false)
            {
                InitializeShipField(Content);
                shipAccessed[currentShipWreckID] = true;
            }

            graphicEffect.resetTransitTimer();
            screenTransitNow = true;
            if (backFromAttributeBoard == true) backFromAttributeBoard = false;
            base.Show();
        }

        private void CreateNewShipWreckVariables()
        {
            //shipAccessed = new List<bool>(GameConstants.maxShipPerLevel);
            //enemies = new List<BaseEnemy[]>(GameConstants.maxShipPerLevel);
            //fishes = new List<Fish[]>(GameConstants.maxShipPerLevel);
            //enemiesAmount = new List<int>(GameConstants.maxShipPerLevel);
            //fishAmount = new List<int>(GameConstants.maxShipPerLevel);
            //treasureChests = new List<List<TreasureChest>>(GameConstants.maxShipPerLevel);
            //staticObjects = new List<List<StaticObject>>(GameConstants.maxShipPerLevel);
            //foundRelic = new List<bool>(GameConstants.maxShipPerLevel);// = false;
            //for (int index = 0; index < GameConstants.maxShipPerLevel; index++)
            //{
            //    //shipAccessed[index] = new Boolean();// false;
            //    //shipAccessed[index] = false;
            //    shipAccessed.Add(false);
            //    //foundRelic[index] = false;
            //    foundRelic.Add(false);
            //}

            shipAccessed = new bool[GameConstants.maxShipPerLevel];
            enemies = new BaseEnemy[GameConstants.maxShipPerLevel][];
            fishes = new Fish[GameConstants.maxShipPerLevel][];
            enemiesAmount = new int[GameConstants.maxShipPerLevel];
            fishAmount = new int[GameConstants.maxShipPerLevel];
            treasureChests = new List<TreasureChest>[GameConstants.maxShipPerLevel];
            staticObjects = new List<StaticObject>[GameConstants.maxShipPerLevel];
            foundRelic = new bool[GameConstants.maxShipPerLevel];
            for (int index = 0; index < GameConstants.maxShipPerLevel; index++)
            {
                shipAccessed[index] = false;
                foundRelic[index] = false;
            }
        }

        private void ResetShipWreckGeneralVariables()//(GameTime gameTime, float aspectRatio)
        {

            hydroBot.Position = Vector3.Zero;
            hydroBot.Position.Y = GameConstants.ShipWreckFloatHeight;
            hydroBot.ForwardDirection = 0f;
            hydroBot.pointToMoveTo = Vector3.Zero;
            hydroBot.reachDestination = true;
            //gameCamera.Update(hydroBot.ForwardDirection,
            //    hydroBot.Position, aspectRatio, gameTime);
            enemyBullet = new List<DamageBullet>();
            healthBullet = new List<HealthBullet>();
            myBullet = new List<DamageBullet>();
            alliesBullets = new List<DamageBullet>();
            bubbles = new List<Bubble>();
            points = new List<Point>();
            //enemiesAmount = GameConstants.NumberEnemies;
            //fishAmount = GameConstants.NumberFish;
            //InitializeShipField(Content);
        }

        private void InitializeShipField(ContentManager Content)
        {
  
            //enemyBullet = new List<DamageBullet>();
            //healthBullet = new List<HealthBullet>();
            //myBullet = new List<DamageBullet>();
            //alliesBullets = new List<DamageBullet>();
            // Initialize the chests here
            // Put the skill in one of it if this.skillID != -1
            treasureChests[currentShipWreckID] = new List<TreasureChest>(GameConstants.NumberChests);
            int randomType = random.Next(3);
            for (int index = 0; index < GameConstants.NumberChests; index++)
            {
                treasureChests[currentShipWreckID].Add(new TreasureChest());
                //put a God's relic inside a chest!
                if (index == 0 && skillID != -1) treasureChests[currentShipWreckID][index].LoadContent(Content, randomType, skillID);
                else treasureChests[currentShipWreckID][index].LoadContent(Content, randomType, -1);
                randomType = random.Next(3);
            }
            if (PlayGameScene.currentLevel >= 4)
                enemiesAmount[currentShipWreckID] = GameConstants.ShipHighNumberShootingEnemies + GameConstants.ShipHighNumberCombatEnemies;
            else enemiesAmount[currentShipWreckID] = GameConstants.ShipLowNumberShootingEnemies + GameConstants.ShipLowNumberCombatEnemies;
            enemies[currentShipWreckID] = new BaseEnemy[enemiesAmount[currentShipWreckID]];
            int enemyNum = enemiesAmount[currentShipWreckID];
            int fishNum = 0;// fishAmount[currentShipWreckID];
            AddingObjects.placeEnemies(ref enemyNum, enemies[currentShipWreckID], Content, random, fishAmount[currentShipWreckID], fishes[currentShipWreckID], null,
                GameConstants.ShipWreckMinRangeX,GameConstants.ShipWreckMaxRangeX, GameConstants.ShipWreckMinRangeZ, GameConstants.ShipWreckMaxRangeZ, 0 , GameMode.ShipWreck, GameConstants.ShipWreckFloatHeight);
            AddingObjects.placeFish(ref fishNum, fishes[currentShipWreckID], Content, random, enemiesAmount[currentShipWreckID], enemies[currentShipWreckID], null,
                GameConstants.ShipWreckMinRangeX, GameConstants.ShipWreckMaxRangeX, GameConstants.ShipWreckMinRangeZ, GameConstants.ShipWreckMaxRangeZ, 0, GameMode.ShipWreck, GameConstants.ShipWreckFloatHeight);
            AddingObjects.placeTreasureChests(treasureChests[currentShipWreckID], staticObjects[currentShipWreckID], random,
                GameConstants.ShipWreckMinRangeX, GameConstants.ShipWreckMaxRangeX, GameConstants.ShipWreckMinRangeZ, GameConstants.ShipWreckMaxRangeZ);
            
            //Initialize the static objects.
            staticObjects[currentShipWreckID] = new List<StaticObject>(GameConstants.NumStaticObjectsShip);
            for (int index = 0; index < GameConstants.NumStaticObjectsShip; index++)
            {
                staticObjects[currentShipWreckID].Add(new StaticObject());
                int randomObject = random.Next(3);
                switch (randomObject)
                {
                    case 0:
                        staticObjects[currentShipWreckID][index].LoadContent(Content, "Models/ShipWreckModels/barrel");
                        break;
                    case 1:
                        staticObjects[currentShipWreckID][index].LoadContent(Content, "Models/ShipWreckModels/barrelstack");
                        break;
                    case 2:
                        staticObjects[currentShipWreckID][index].LoadContent(Content, "Models/ShipWreckModels/boxstack");
                        break;
                }
                //staticObjects[index].LoadContent(Content, "Models/boxstack");
            }
            AddingObjects.PlaceStaticObjectsOnShipFloor(staticObjects[currentShipWreckID], treasureChests[currentShipWreckID], random, GameConstants.ShipWreckMinRangeX,
                GameConstants.ShipWreckMaxRangeX, GameConstants.ShipWreckMinRangeZ, GameConstants.ShipWreckMaxRangeZ);
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
            bool chestExitPressed; //to close painting, relic, or no-key
            chestExitPressed = (lastKeyboardState.IsKeyDown(Keys.LeftAlt) &&
                    (currentKeyboardState.IsKeyUp(Keys.LeftAlt)));
            chestExitPressed |= (lastKeyboardState.IsKeyDown(Keys.RightAlt) &&
                    (currentKeyboardState.IsKeyUp(Keys.RightAlt)));
            chestExitPressed |= (lastKeyboardState.IsKeyDown(Keys.Enter) &&
                    (currentKeyboardState.IsKeyUp(Keys.Enter)));
            if (MediaPlayer.State.Equals(MediaState.Stopped))
            {
                MediaPlayer.Play(audio.backgroundMusics[random.Next(GameConstants.NumNormalBackgroundMusics)]);
            }
            float aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
            lastKeyboardState = currentKeyboardState;
            //lastMouseState = currentMouseState;
            currentKeyboardState = Keyboard.GetState();
            if (foundRelic[currentShipWreckID])
            {
                // return to game if enter pressed
                if (chestExitPressed)
                {
                    foundRelic[currentShipWreckID] = false;
                }
                return;
            }
            if (showPainting)
            {
                // return to game if enter pressed
                if (chestExitPressed)
                {
                    showPainting = false;
                }
                return;
            }
            if (showStrangeObjectFound)
            {
                // return to game if enter pressed
                if (chestExitPressed)
                {
                    showStrangeObjectFound = false;
                }
                return;
            }
            if (showNoKey)
            {
                // return to game if enter pressed
                if (chestExitPressed)
                {
                    showNoKey = false;
                }
                return;
            }
            if (!paused && !returnToMain)
            {   
             
                CursorManager.CheckClick(ref lastMouseState,ref currentMouseState, gameTime, ref clickTimer, ref clicked, ref doubleClicked);

                bool mouseOnInteractiveIcons = false;
                //hydrobot update
                hydroBot.UpdateAction(gameTime, cursor, gameCamera, enemies[currentShipWreckID], enemiesAmount[currentShipWreckID], fishes[currentShipWreckID], fishAmount[currentShipWreckID],
                    Content, spriteBatch, myBullet, this, null, healthBullet, null, null, null, null, null, mouseOnInteractiveIcons);

                //add 1 bubble over tank and each enemy
                timeNextBubble -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (timeNextBubble <= 0)
                {
                    Bubble bubble = new Bubble();
                    bubble.LoadContent(Content, hydroBot.Position, false, 0.025f);
                    bubbles.Add(bubble);
                    for (int i = 0; i < enemiesAmount[currentShipWreckID]; i++)
                    {
                        if (enemies[currentShipWreckID][i].BoundingSphere.Intersects(frustum))
                        {
                            Bubble aBubble = new Bubble();
                            aBubble.LoadContent(Content, enemies[currentShipWreckID][i].Position, false, 0.025f);
                            bubbles.Add(aBubble);
                        }
                    }

                    timeNextBubble = 200.0f;
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

                gameCamera.Update(hydroBot.ForwardDirection,
                    hydroBot.Position, aspectRatio, gameTime);
                // Updating camera's frustum
                frustum = new BoundingFrustum(gameCamera.ViewMatrix * gameCamera.ProjectionMatrix);

                for (int i = 0; i < myBullet.Count; i++) {
                    myBullet[i].update(gameTime);
                }

                for (int i = 0; i < healthBullet.Count; i++) {
                    healthBullet[i].update(gameTime);
                }

                for (int i = 0; i < enemyBullet.Count; i++) {
                    enemyBullet[i].update(gameTime);
                }
                for (int i = 0; i < alliesBullets.Count; i++)
                {
                    alliesBullets[i].update(gameTime);
                }
                Collision.updateBulletOutOfBound(hydroBot.MaxRangeX, hydroBot.MaxRangeZ, healthBullet, myBullet, enemyBullet, alliesBullets, frustum);
                int refNum = enemiesAmount[currentShipWreckID];
                Collision.updateDamageBulletVsBarriersCollision(myBullet, enemies[currentShipWreckID], ref refNum, frustum, GameMode.ShipWreck, gameTime, hydroBot,
                    enemies[currentShipWreckID], enemiesAmount[currentShipWreckID], fishes[currentShipWreckID], fishAmount[currentShipWreckID], gameCamera);
                enemiesAmount[currentShipWreckID] = refNum;
                Collision.updateHealingBulletVsBarrierCollision(healthBullet, fishes[currentShipWreckID], fishAmount[currentShipWreckID], frustum, GameMode.ShipWreck);
                refNum = fishAmount[currentShipWreckID];
                Collision.updateDamageBulletVsBarriersCollision(enemyBullet, fishes[currentShipWreckID], ref refNum, frustum, GameMode.ShipWreck, gameTime, hydroBot,
                    enemies[currentShipWreckID], enemiesAmount[currentShipWreckID], fishes[currentShipWreckID], fishAmount[currentShipWreckID], gameCamera);
                fishAmount[currentShipWreckID] = refNum;
                Collision.updateProjectileHitBot(hydroBot, enemyBullet, GameMode.ShipWreck, enemies[currentShipWreckID], enemiesAmount[currentShipWreckID], null);
                refNum = enemiesAmount[currentShipWreckID];
                Collision.updateDamageBulletVsBarriersCollision(alliesBullets, enemies[currentShipWreckID], ref refNum, frustum, GameMode.ShipWreck, gameTime, hydroBot,
                    enemies[currentShipWreckID], enemiesAmount[currentShipWreckID], fishes[currentShipWreckID], fishAmount[currentShipWreckID], gameCamera);
                enemiesAmount[currentShipWreckID] = refNum;
                refNum = enemiesAmount[currentShipWreckID];
                Collision.deleteSmallerThanZero(enemies[currentShipWreckID], ref refNum, frustum, GameMode.ShipWreck, cursor);
                enemiesAmount[currentShipWreckID] = refNum;
                refNum = fishAmount[currentShipWreckID];
                Collision.deleteSmallerThanZero(fishes[currentShipWreckID], ref refNum, frustum, GameMode.ShipWreck, cursor);
                fishAmount[currentShipWreckID] = refNum;

                for (int i = 0; i < enemiesAmount[currentShipWreckID]; i++)
                {
                    if (enemies[currentShipWreckID][i].stunned)
                    {
                        if (PoseidonGame.playTime.TotalSeconds - enemies[currentShipWreckID][i].stunnedStartTime > GameConstants.timeStunLast)
                            enemies[currentShipWreckID][i].stunned = false;
                    }
                    refNum = enemiesAmount[currentShipWreckID];
                    enemies[currentShipWreckID][i].Update(enemies[currentShipWreckID], ref refNum, fishes[currentShipWreckID], fishAmount[currentShipWreckID], random.Next(100), hydroBot, enemyBullet, alliesBullets, frustum, gameTime, GameMode.ShipWreck);
                    enemiesAmount[currentShipWreckID] = refNum;
                }

                foreach (TreasureChest chest in treasureChests[currentShipWreckID])
                {
                    if (CharacterNearChest(chest.BoundingSphere) && CursorManager.MouseOnObject(cursor, chest.BoundingSphere, chest.Position, gameCamera)
                        && chest.opened == false && doubleClicked)
                    {
                        //fishes are not going to give u the key for treasure chest
                        //when they are not pleased because of polluted environment
                        if (!PlayGameScene.hadkey)
                        {
                            showNoKey = true;
                        }
                        else
                        {
                            chest.opened = true;
                            audio.OpenChest.Play();
                            chest.Model = Content.Load<Model>("Models/ShipWreckModels/chest");
                            chest.SetupShaderParameters(PoseidonGame.contentManager, chest.Model);
                            //this is just for testing
                            //should be removed
                            //skillID = 4;
                            //chest.skillID = 4;
                            if (chest.skillID == -1)
                            {
                                // give the player some experience as reward
                                HydroBot.currentExperiencePts += GameConstants.ExpPainting;

                                Point point = new Point();
                                String point_string = "+" + GameConstants.ExpPainting + " EXP";
                                point.LoadContent(PlayGameScene.Content, point_string, chest.Position, Color.LawnGreen);
                                points.Add(point);

                                // show a random painting
                                paintingToShow = random.Next(oceanPaintings.paintings.Count);
                                showPainting = true;

                            }
                            else
                            {
                                // player found a God's relic
                                // unlock a skill
                                // do not unlock if the player has already found it before
                                // because re-exploreing ship is enabled now
                                if (HydroBot.skills[chest.skillID] == false)
                                {
                                    HydroBot.skills[chest.skillID] = true;
                                    //activate the goodwill bar icon too
                                    if (chest.skillID == 0) HydroBot.iconActivated[IngamePresentation.bowIcon] = true;
                                    else if (chest.skillID == 1) HydroBot.iconActivated[IngamePresentation.hammerIcon] = true;
                                    else if (chest.skillID == 2) HydroBot.iconActivated[IngamePresentation.armorIcon] = true;
                                    else if (chest.skillID == 3) HydroBot.iconActivated[IngamePresentation.sandalIcon] = true;
                                    else if (chest.skillID == 4) HydroBot.iconActivated[IngamePresentation.beltIcon] = true;
                                    HydroBot.activeSkillID = chest.skillID;
                                    foundRelic[currentShipWreckID] = true;
                                }
                                else
                                {
                                    // give the player some experience as reward
                                    HydroBot.currentExperiencePts += GameConstants.ExpPainting;

                                    Point point = new Point();
                                    String point_string = "+" + GameConstants.ExpPainting + " EXP";
                                    point.LoadContent(Content, point_string, chest.Position, Color.LawnGreen);
                                    points.Add(point);

                                    // show a random painting
                                    paintingToShow = random.Next(oceanPaintings.paintings.Count);
                                    showPainting = true;
                                }
                            }
                            if (showPainting == true)
                            {
                                if (random.Next(100) <= 2)
                                {
                                    showStrangeObjectFound = true;
                                    HydroBot.numStrangeObjCollected++;
                                    fossilToDraw = random.Next(4);
                                }
                            }
                        }
                        doubleClicked = false;
                    }
                }

                for (int i = 0; i < fishAmount[currentShipWreckID]; i++)
                {
                    fishes[currentShipWreckID][i].Update(gameTime, enemies[currentShipWreckID], enemiesAmount[currentShipWreckID], fishes[currentShipWreckID], fishAmount[currentShipWreckID], random.Next(100), hydroBot, enemyBullet);
                }

                //for the shader
                m_Timer += (float)gameTime.ElapsedGameTime.Milliseconds / 1000;

                //cursor update
                cursor.Update(GraphicDevice, gameCamera, gameTime, frustum);

                //update graphic effects
                graphicEffect.UpdateInput(gameTime);
                //update particle systems
                particleManager.Update(gameTime);

                roundTimer -= gameTime.ElapsedGameTime;
                PoseidonGame.playTime += gameTime.ElapsedGameTime;
                if (roundTimer < TimeSpan.Zero)
                {
                    returnToMain = true;
                }
                // Dead inside a shipwreck
                if (HydroBot.currentHitPoint <= 0){
                    returnToMain = true;
                }

                base.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime) {
            if (returnToMain) return;

            base.Draw(gameTime);
            

            // Change back the config changed by spriteBatch
            RestoreGraphicConfig();

            DrawGameplayScreen(gameTime);
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

        private void DrawGameplayScreen(GameTime gameTime)
        {
            
            if (showPainting)
            {
                spriteBatch.Begin();
                DrawPainting();
                spriteBatch.End();
                return;
            }
            if (showNoKey)
            {
                spriteBatch.Begin();
                DrawNoKey();
                spriteBatch.End();
                return;
            }

            //preparingedge detecting for the object being pointed at
            graphicEffect.PrepareEdgeDetect(cursor, gameCamera, fishes[currentShipWreckID], fishAmount[currentShipWreckID], enemies[currentShipWreckID], enemiesAmount[currentShipWreckID],
                null, null, null, null, treasureChests[currentShipWreckID], graphics.GraphicsDevice, normalDepthRenderTarget);

            //normal drawing of the game scene
            graphics.GraphicsDevice.SetRenderTarget(renderTarget);
            graphics.GraphicsDevice.Clear(Color.Black);
            
            DrawTerrain(ground.Model);
            // Updating camera's frustum
            frustum = new BoundingFrustum(gameCamera.ViewMatrix * gameCamera.ProjectionMatrix);

            BoundingSphere chestSphere;
            // Drawing ship wrecks
            foreach (TreasureChest treasureChest in treasureChests[currentShipWreckID])
            {
                chestSphere = treasureChest.BoundingSphere;
                chestSphere.Center = treasureChest.Position;
                if (chestSphere.Intersects(frustum))
                {
                    treasureChest.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalShading");
                    //RasterizerState rs = new RasterizerState();
                    //rs.FillMode = FillMode.WireFrame;
                    //GraphicDevice.RasterizerState = rs;
                    //treasureChest.DrawBoundingSphere(gameCamera.ViewMatrix,
                    //    gameCamera.ProjectionMatrix, boundingSphere);

                    //rs = new RasterizerState();
                    //rs.FillMode = FillMode.Solid;
                    //GraphicDevice.RasterizerState = rs;
                }
            }
            //Draw each static object
            foreach (StaticObject staticObject in staticObjects[currentShipWreckID])
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
            for (int i = 0; i < enemiesAmount[currentShipWreckID]; i++)
            {
                if (enemies[currentShipWreckID][i].BoundingSphere.Intersects(frustum))
                {
                    enemies[currentShipWreckID][i].Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalShading");
                }
            }
            for (int i = 0; i < fishAmount[currentShipWreckID]; i++)
            {
                if (fishes[currentShipWreckID][i].BoundingSphere.Intersects(frustum))
                    fishes[currentShipWreckID][i].Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalShading");
            }

            hydroBot.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalShading");

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
            // draw bubbles
            foreach (Bubble bubble in bubbles)
            {
                bubble.Draw(spriteBatch, 1.5f);
            }

            //draw particle effects
            particleManager.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameTime);
            //applying edge detection
            graphicEffect.ApplyEdgeDetection(renderTarget, normalDepthRenderTarget, graphics.GraphicsDevice, edgeDetectionRenderTarget);

            SceneTexture = edgeDetectionRenderTarget;
            //graphics.GraphicsDevice.SetRenderTarget(null);
            //spriteBatch.Begin();
            //spriteBatch.Draw(SceneTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
            //spriteBatch.End();
            afterEffectsAppliedRenderTarget = graphicEffect.DrawWithEffects(gameTime, SceneTexture, graphics);
            //graphicEffect.DrawWithEffects(gameTime, SceneTexture, graphics);
            graphics.GraphicsDevice.SetRenderTarget(afterEffectsAppliedRenderTarget);
            for (int i = 0; i < enemiesAmount[currentShipWreckID]; i++)
            {
                if (enemies[currentShipWreckID][i].BoundingSphere.Intersects(frustum))
                {
                    if (enemies[currentShipWreckID][i].stunned == true)
                    {
                        Vector3 placeToDraw = GraphicDevice.Viewport.Project(enemies[currentShipWreckID][i].Position, gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity);
                        Vector2 drawPos = new Vector2(placeToDraw.X, placeToDraw.Y);
                        spriteBatch.Begin();
                        spriteBatch.Draw(stunnedIconTexture, drawPos, Color.White);
                        spriteBatch.End();
                        RestoreGraphicConfig();
                    }
                    if (enemies[currentShipWreckID][i].isFleeing == true)
                    {
                        Vector3 placeToDraw = GraphicDevice.Viewport.Project(enemies[currentShipWreckID][i].Position, gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity);
                        Vector2 drawPos = new Vector2(placeToDraw.X, placeToDraw.Y);
                        spriteBatch.Begin();
                        spriteBatch.Draw(scaredIconTexture, drawPos, Color.White);
                        spriteBatch.End();
                        RestoreGraphicConfig();
                    }
                }
            }
            //Draw points gained / lost
            foreach (Point point in points)
            {
                point.Draw(spriteBatch);
            }
            spriteBatch.Begin();
            DrawStats();
            DrawBulletType();
            if (HydroBot.activeSkillID != -1) DrawActiveSkill();
            cursor.Draw(gameTime);
            spriteBatch.End();
            if (foundRelic[currentShipWreckID])
            {
                spriteBatch.Begin();
                DrawFoundRelicScene(skillID);
                spriteBatch.End();
                RestoreGraphicConfig();
                //return;
            }
            if (showStrangeObjectFound)
            {
                spriteBatch.Begin();
                DrawFoundStrangeObjScene();
                spriteBatch.End();
                RestoreGraphicConfig();
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
            spriteBatch.Begin();
            spriteBatch.Draw(cutSceneImmediateRenderTarget, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
            spriteBatch.End();
            
        }
        private void DrawFoundStrangeObjScene()
        {
            float xOffsetText, yOffsetText;
            string str1 = "You have found a strange ... \"rock\"";

            Rectangle rectSafeArea;
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;
            str1 = IngamePresentation.wrapLine(str1, rectSafeArea.Width - 20, paintingFont);

            xOffsetText = rectSafeArea.X;
            yOffsetText = rectSafeArea.Y;

            //Vector2 strSize = statsFont.MeasureString(str1);
            Vector2 strPosition =
                new Vector2((int)xOffsetText + 10, (int)yOffsetText + 20);

            //spriteBatch.Draw(skillFoundScreen, new Rectangle(rectSafeArea.Center.X - skillFoundScreen.Width / 2, rectSafeArea.Center.Y - skillFoundScreen.Height / 2, skillFoundScreen.Width, skillFoundScreen.Height), Color.White);

            spriteBatch.DrawString(paintingFont, str1, strPosition, Color.Silver);
            xOffsetText = rectSafeArea.Center.X - (fossilTextures[fossilToDraw].Width / 2);
            yOffsetText = rectSafeArea.Center.Y - (fossilTextures[fossilToDraw].Height / 2);

            Vector2 skillIconPosition =
                new Vector2((int)xOffsetText, (int)yOffsetText);

            spriteBatch.Draw(fossilTextures[fossilToDraw], skillIconPosition, Color.White);

            string nextText = "Press Alt/Enter to continue";
            Vector2 nextTextPosition = new Vector2(GraphicDevice.Viewport.TitleSafeArea.Right - menuSmall.MeasureString(nextText).X, GraphicDevice.Viewport.TitleSafeArea.Bottom - menuSmall.MeasureString(nextText).Y);
            spriteBatch.DrawString(menuSmall, nextText, nextTextPosition, Color.White);
        }
        private void DrawNoKey()
        {
            string message = "You have not had the key to treasure chests yet, try to help the fish first so that they will help you find the key in return";
            message = IngamePresentation.wrapLine(message, 800, paintingFont);
            spriteBatch.Draw(noKeyScreen, new Rectangle(GraphicDevice.Viewport.TitleSafeArea.Center.X - noKeyScreen.Width / 2, GraphicDevice.Viewport.TitleSafeArea.Center.Y - noKeyScreen.Height / 2, noKeyScreen.Width, noKeyScreen.Height), Color.SandyBrown);
            spriteBatch.DrawString(paintingFont, message, new Vector2(GraphicDevice.Viewport.TitleSafeArea.Center.X - 400, 20), Color.White);
            
            string nextText = "Press Alt/Enter to continue";
            Vector2 nextTextPosition = new Vector2(GraphicDevice.Viewport.TitleSafeArea.Right - menuSmall.MeasureString(nextText).X, GraphicDevice.Viewport.TitleSafeArea.Bottom - menuSmall.MeasureString(nextText).Y);
            spriteBatch.DrawString(menuSmall, nextText, nextTextPosition, Color.White);
        }
        private void DrawPainting()
        {
            spriteBatch.Draw(oceanPaintings.paintings[paintingToShow].painting, 
                new Rectangle(0, 0, GraphicDevice.Viewport.TitleSafeArea.Width, GraphicDevice.Viewport.TitleSafeArea.Height), Color.White);
            spriteBatch.DrawString(paintingFont, oceanPaintings.paintings[paintingToShow].caption, new Vector2(0, 0), oceanPaintings.paintings[paintingToShow].color);
            spriteBatch.DrawString(paintingFont, "Do you know:", new Vector2(GraphicDevice.Viewport.TitleSafeArea.Left, GraphicDevice.Viewport.TitleSafeArea.Center.Y),
                oceanPaintings.paintings[paintingToShow].color);

            String line = IngamePresentation.wrapLine(oceanPaintings.paintings[paintingToShow].tip, GraphicDevice.Viewport.TitleSafeArea.Width, paintingFont);
            spriteBatch.DrawString(paintingFont, line,
                new Vector2(GraphicDevice.Viewport.TitleSafeArea.Left, GraphicDevice.Viewport.TitleSafeArea.Center.Y + 100), oceanPaintings.paintings[paintingToShow].color);

            string nextText = "Press Alt/Enter to continue";
            Vector2 nextTextPosition = new Vector2(GraphicDevice.Viewport.TitleSafeArea.Right - menuSmall.MeasureString(nextText).X, GraphicDevice.Viewport.TitleSafeArea.Bottom - menuSmall.MeasureString(nextText).Y);
            spriteBatch.DrawString(menuSmall, nextText, nextTextPosition, oceanPaintings.paintings[paintingToShow].color);

        }
        private void DrawFoundRelicScene(int skill_id)
        {
            float xOffsetText, yOffsetText;
            string skill_name = " " ;
            switch (skill_id)
            {
                case 0:
                    skill_name = "Hercules' bow. A mighty bow whose arrows are sure dealers of death, for they had been dipped in the blood of the great dragon of Lerna. Aim well, for it eats up your strength too. Press 1 to select and right click to use.";
                    break;
                case 1:
                    skill_name = "Mjolnir, the mighty hammer of Thor. It is one of the most fearsome weapons capable of destroying everyone around you if you are strong. But if you're weak, it is will hardly dent an armour. Press 2 to select it and right click to use it.";
                    break;
                case 2:
                    skill_name = "Achilles' armor. It is enchanted and can not be pierced. Press 3 to select and right click to use.";
                    break;
                case 3:
                    skill_name = "Hermes' Winged sandal. Made of imperishable gold, they fly as swift as any bird and impair anyone in its way. Press 4 to select and right click to use.";
                    break;
                case 4:
                    skill_name = "Aphrodite's belt. It will bewilder and hypnotize the enemy to turn against each other. Press 5 to select and right click to use.";
                    break;
            }
            string str1 = "You have discovered the " + skill_name;

            Rectangle rectSafeArea;
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;
            str1 = IngamePresentation.wrapLine(str1, rectSafeArea.Width - 20, paintingFont);

            xOffsetText = rectSafeArea.X;
            yOffsetText = rectSafeArea.Y;

            //Vector2 strSize = statsFont.MeasureString(str1);
            Vector2 strPosition =
                new Vector2((int)xOffsetText + 10, (int)yOffsetText+20);

            //spriteBatch.Draw(skillFoundScreen, new Rectangle(rectSafeArea.Center.X - skillFoundScreen.Width / 2, rectSafeArea.Center.Y - skillFoundScreen.Height / 2, skillFoundScreen.Width, skillFoundScreen.Height), Color.White);

            spriteBatch.DrawString(paintingFont, str1, strPosition, Color.Silver);
            xOffsetText = rectSafeArea.Center.X - (skillTextures[skill_id].Width/2);
            yOffsetText = rectSafeArea.Center.Y - (skillTextures[skill_id].Height/2);

            Vector2 skillIconPosition =
                new Vector2((int)xOffsetText, (int)yOffsetText);

            spriteBatch.Draw(skillTextures[skill_id], skillIconPosition, Color.White);

            string nextText = "Press Alt/Enter to continue";
            Vector2 nextTextPosition = new Vector2(GraphicDevice.Viewport.TitleSafeArea.Right - menuSmall.MeasureString(nextText).X, GraphicDevice.Viewport.TitleSafeArea.Bottom - menuSmall.MeasureString(nextText).Y);
            spriteBatch.DrawString(menuSmall, nextText, nextTextPosition, Color.White);
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
            IngamePresentation.DrawActiveSkill(GraphicDevice, skillTextures, spriteBatch);
        }

        private void DrawStats()
        {
            float xOffsetText, yOffsetText;
            int days;
            string str1 = GameConstants.StrTimeRemaining;
            days = ((roundTimer.Minutes * 60) + roundTimer.Seconds) / GameConstants.DaysPerSecond;
            str1 += days.ToString();
            //if (roundTimer.Minutes < 10)
            //    str1 += "0";
            //str1 += roundTimer.Minutes + ":";
            //if (roundTimer.Seconds < 10)
            //    str1 += "0";
            //str1 += roundTimer.Seconds;
            //string str2 = "";// = GameConstants.StrCellsFound + retrievedFruits.ToString() +
            //" of " + fruits.Count;
            Rectangle rectSafeArea;

            //str1 += (roundTimer.Seconds).ToString();

            //Vector3 pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.ShipWreckFloatHeight);
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
            //str2 += "\nEnemy amount " + enemies.Length;
            //str2 += "\nFish Position " + fish[0].Position;
            //str2 += "\nFish amount " + fish.Length;
            //str2 += "\nTank Forward Direction " + tank.ForwardDirection;
            //str2 += "\nEnemy FW " + enemies[0].ForwardDirection;
            //str2 += "\nPrevFIre " + enemies[0].prevFire;
            //str2 += "\n Tank Health " + Tank.currentHitPoint;
            //str2 += "\n" + tank.skillPrevUsed[0] + " " + tank.skillPrevUsed[1] + " " + tank.skillPrevUsed[2];

            //Display Fish Health
            Fish fishPontedAt = CursorManager.MouseOnWhichFish(cursor, gameCamera, fishes[currentShipWreckID], fishAmount[currentShipWreckID]);
            if (fishPontedAt != null)
                IngamePresentation.DrawHealthBar(HealthBar, game, spriteBatch, statsFont, (int)fishPontedAt.health, (int)fishPontedAt.maxHealth, 5, fishPontedAt.Name, Color.BlueViolet);

            //Display Enemy Health
            BaseEnemy enemyPointedAt = CursorManager.MouseOnWhichEnemy(cursor, gameCamera, enemies[currentShipWreckID], enemiesAmount[currentShipWreckID]);
            if (enemyPointedAt != null)
                IngamePresentation.DrawHealthBar(HealthBar, game, spriteBatch, statsFont, (int)enemyPointedAt.health, (int)enemyPointedAt.maxHealth, 5, enemyPointedAt.Name, Color.IndianRed);

            //Display Cyborg health
            IngamePresentation.DrawHealthBar(HealthBar, game, spriteBatch, statsFont, (int)HydroBot.currentHitPoint, (int)HydroBot.maxHitPoint, game.Window.ClientBounds.Height - 60, "HEALTH", Color.Brown);

            //Display Level/Experience Bar
            IngamePresentation.DrawLevelBar(HealthBar, game, spriteBatch, statsFont, (int)HydroBot.currentExperiencePts, HydroBot.nextLevelExperience, HydroBot.level, game.Window.ClientBounds.Height - 30, "EXPERIENCE LEVEL", Color.Brown);

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
        public bool CharacterNearChest(BoundingSphere chestSphere)
        {
            if (hydroBot.BoundingSphere.Intersects(chestSphere))
                return true;
            else
                return false;
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
