using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Poseidon.Core;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

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
        GamePadState lastGamePadState = new GamePadState();
        GamePadState currentGamePadState = new GamePadState();
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

        BaseEnemy[] enemies;
        Fish[] fish;

        int enemiesAmount = 0;
        int fishAmount = 0;

        List<DamageBullet> myBullet;
        List<DamageBullet> enemyBullet;
        List<HealthBullet> healthBullet;
        List<DamageBullet> alliesBullets;
        List<TreasureChest> treasureChests;
        List<StaticObject> staticObjects;

        //A tank
        public Tank tank;

        private TimeSpan fireTime;
        private TimeSpan prevFireTime;

        // For drawing the currently selected skill
        protected Texture2D[] skillTextures;
        // For drawing the currently selected bullet type
        protected Texture2D[] bulletTypeTextures;
        // Game is paused?
        protected bool paused;
        protected Vector2 pausePosition;
        protected Rectangle pauseRect = new Rectangle(1, 120, 200, 44);
        protected Texture2D actionTexture;
        protected Texture2D stunnedTexture;
        protected Texture2D gameObjectiveIconTexture;
        protected Texture2D noKeyScreen;
        protected Texture2D skillFoundScreen;
        // He died inside the ship wreck?
        public bool returnToMain;
        // has artifact?
        public int skillID = -1;

        // Frustum of the camera
        BoundingFrustum frustum;

        // For mouse inputs
        bool doubleClicked = false;
        bool clicked = false;
        double clickTimer = 0;

        HeightMapInfo heightMapInfo;

        // draw a cutscene when finding a god's relic
        bool foundRelic = false;

        // shader for underwater effect
        // Our Post Process effect object, this is where our shader will be loaded and compiled
        Effect effectPost;
        float m_Timer = 0;
        RenderTarget2D renderTarget;
        Texture2D SceneTexture;

        // showing paintings when openning treasure chests
        OceanPaintings oceanPaintings;
        bool showPainting = false;
        int paintingToShow = 0;
        bool showNoKey = false;
        // Bubbles over characters
        List<Bubble> bubbles;
        float timeNextBubble = 200.0f;
        //float timeNextSeaBedBubble = 3000.0f;

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
            this.stunnedTexture = stunnedTexture;
            random = new Random();
            ground = new GameObject();
            gameCamera = new Camera(GameConstants.ShipCamHeight);
            boundingSphere = new GameObject();
            tank = new Tank(GameConstants.ShipWreckMaxRangeX, GameConstants.ShipWreckMaxRangeZ, GameConstants.ShipWreckFloatHeight);
            fireTime = TimeSpan.FromSeconds(0.3f);
            enemies = new BaseEnemy[GameConstants.ShipNumberShootingEnemies + GameConstants.ShipNumberCombatEnemies];
            fish = new Fish[GameConstants.ShipNumberFish];
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
            InitializeShipField(Content);

            tank.Load(Content);

            //Load healthbar
            HealthBar = Content.Load<Texture2D>("Image/Miscellaneous/HealthBar");

            noKeyScreen = Content.Load<Texture2D>("Image/SceneTextures/no_key");

            skillFoundScreen = Content.Load<Texture2D>("Image/SceneTextures/skillFoundBackground");

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
            //initialize random shipwreck terrain
            //Random random = new Random();
            //int random_terrain = random.Next(5);
            //string wood_terrain_name = "Image/wood-terrain" + random_terrain;

            //ground.Model = Content.Load<Model>(wood_terrain_name);
            //reset position for the tank
            tank.Position = Vector3.Zero;
            tank.Position.Y = GameConstants.ShipWreckFloatHeight;
            tank.ForwardDirection = 0f;
            //MediaPlayer.Play(audio.BackMusic);
            showNoKey = false;
            showPainting = false;
            InitializeShipField(Content);
            base.Show();
        }

        private void ResetGame(GameTime gameTime, float aspectRatio)
        {
            
            tank.Reset();
            gameCamera.Update(tank.ForwardDirection,
                tank.Position, aspectRatio, gameTime);
            //enemiesAmount = GameConstants.NumberEnemies;
            //fishAmount = GameConstants.NumberFish;
            InitializeShipField(Content);
        }

        private void InitializeShipField(ContentManager Content)
        {
            returnToMain = false;
            enemyBullet = new List<DamageBullet>();
            healthBullet = new List<HealthBullet>();
            myBullet = new List<DamageBullet>();
            alliesBullets = new List<DamageBullet>();
            // Initialize the chests here
            // Put the skill in one of it if this.skillID != -1
            treasureChests = new List<TreasureChest>(GameConstants.NumberChests);
            int randomType = random.Next(3);
            for (int index = 0; index < GameConstants.NumberChests; index++)
            {
                treasureChests.Add(new TreasureChest());
                //put a God's relic inside a chest!
                if (index == 0 && skillID != 0) treasureChests[index].LoadContent(Content, randomType, skillID);
                else treasureChests[index].LoadContent(Content, randomType, -1);
                randomType = random.Next(3);
            }
            AddingObjects.placeEnemies(ref enemiesAmount, enemies, Content, random, fishAmount, fish, null,
                GameConstants.ShipWreckMinRangeX,GameConstants.ShipWreckMaxRangeX, GameConstants.ShipWreckMinRangeZ, GameConstants.ShipWreckMaxRangeZ, 0 ,false, GameConstants.ShipWreckFloatHeight);
            AddingObjects.placeFish(ref fishAmount, fish, Content, random, enemiesAmount, enemies, null,
                GameConstants.ShipWreckMinRangeX, GameConstants.ShipWreckMaxRangeX, GameConstants.ShipWreckMinRangeZ, GameConstants.ShipWreckMaxRangeZ, 0, false, GameConstants.ShipWreckFloatHeight);
            AddingObjects.placeTreasureChests(treasureChests, staticObjects, random, heightMapInfo,
                GameConstants.ShipWreckMinRangeX, GameConstants.ShipWreckMaxRangeX, GameConstants.ShipWreckMinRangeZ, GameConstants.ShipWreckMaxRangeZ);
            
            //Initialize the static objects.
            staticObjects = new List<StaticObject>(GameConstants.NumStaticObjectsShip);
            for (int index = 0; index < GameConstants.NumStaticObjectsShip; index++)
            {
                staticObjects.Add(new StaticObject());
                int randomObject = random.Next(3);
                switch (randomObject)
                {
                    case 0:
                        staticObjects[index].LoadContent(Content, "Models/ShipWreckModels/barrel");
                        break;
                    case 1:
                        staticObjects[index].LoadContent(Content, "Models/ShipWreckModels/barrelstack");
                        break;
                    case 2:
                        staticObjects[index].LoadContent(Content, "Models/ShipWreckModels/boxstack");
                        break;
                }
                //staticObjects[index].LoadContent(Content, "Models/boxstack");
            }
            AddingObjects.PlaceStaticObjectsOnShipFloor(staticObjects, treasureChests, random, heightMapInfo, GameConstants.ShipWreckMinRangeX,
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
            if (MediaPlayer.State.Equals(MediaState.Stopped))
            {
                MediaPlayer.Play(audio.backgroundMusics[random.Next(GameConstants.NumNormalBackgroundMusics)]);
            }
            float aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
            lastKeyboardState = currentKeyboardState;
            //lastMouseState = currentMouseState;
            currentKeyboardState = Keyboard.GetState();
            lastGamePadState = currentGamePadState;
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
            if (foundRelic)
            {
                // return to game if enter pressed
                if ((lastKeyboardState.IsKeyDown(Keys.Enter) &&
                    (currentKeyboardState.IsKeyUp(Keys.Enter))) ||
                    currentGamePadState.Buttons.Start == ButtonState.Pressed)
                {
                    foundRelic = false;
                }
                return;
            }
            if (showPainting)
            {
                // return to game if enter pressed
                if ((lastKeyboardState.IsKeyDown(Keys.Enter) &&
                    (currentKeyboardState.IsKeyUp(Keys.Enter))) ||
                    currentGamePadState.Buttons.Start == ButtonState.Pressed)
                {
                    showPainting = false;
                }
                return;
            }
            if (showNoKey)
            {
                // return to game if enter pressed
                if ((lastKeyboardState.IsKeyDown(Keys.Enter) &&
                    (currentKeyboardState.IsKeyUp(Keys.Enter))) ||
                    currentGamePadState.Buttons.Start == ButtonState.Pressed)
                {
                    showNoKey = false;
                }
                return;
            }
            if (!paused && !returnToMain)
            {   
                //currentMouseState = Mouse.GetState();
                CursorManager.CheckClick(ref lastMouseState,ref currentMouseState, gameTime, ref clickTimer, ref clicked, ref doubleClicked);
                //if the user clicks or holds mouse's left button
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
                        pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.ShipWreckFloatHeight);
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
                        pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.ShipWreckFloatHeight);
                        tank.ForwardDirection = CursorManager.CalculateAngle(pointIntersect, tank.Position);
                        //if the skill has cooled down
                        //or this is the 1st time the user uses it
                        if ((gameTime.TotalGameTime.TotalSeconds - Tank.skillPrevUsed[0] > GameConstants.coolDownForHerculesBow) || Tank.firstUse[0] == true)
                        {
                            Tank.firstUse[0] = false;
                            Tank.skillPrevUsed[0] = gameTime.TotalGameTime.TotalSeconds;
                            //audio.Explosion.Play();
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
                            audio.Explosion.Play();
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
                            audio.armorSound.Play();
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
                            audio.hermesSound.Play();
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
                            audio.hipnotizeSound.Play();
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
                        pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.ShipWreckFloatHeight);
                        tank.ForwardDirection = CursorManager.CalculateAngle(pointIntersect, tank.Position);
                        if (gameTime.TotalGameTime.TotalSeconds - prevFireTime.TotalSeconds > fireTime.TotalSeconds / (Tank.shootingRate * Tank.fireRateUp))
                        {
                            prevFireTime = gameTime.TotalGameTime;
                            //audio.Shooting.Play();
                            if (Tank.bulletType == 0) { AddingObjects.placeTankDamageBullet(tank, Content, myBullet); }
                            else if (Tank.bulletType == 1) { AddingObjects.placeHealingBullet(tank, Content, healthBullet); }
                        }
                    }
                    pointIntersect = Vector3.Zero;
                }

                //if the user clicks or holds mouse's left button
                else if (currentMouseState.LeftButton == ButtonState.Pressed && !mouseOnLivingObject)
                {
                    pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.ShipWreckFloatHeight);
                }
                else if (lastMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released)
                {
                    pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.ShipWreckFloatHeight);
                    //if it is out of shooting range then just move there
                    if (!CursorManager.InShootingRange(tank, cursor, gameCamera, GameConstants.ShipWreckFloatHeight))
                    {

                    }
                    else
                    {
                        //if the enemy is in the shooting range then shoot it w/o moving to it
                        if (mouseOnLivingObject && gameTime.TotalGameTime.TotalSeconds - prevFireTime.TotalSeconds > fireTime.TotalSeconds / (Tank.shootingRate * Tank.fireRateUp))
                        {
                            tank.ForwardDirection = CursorManager.CalculateAngle(pointIntersect, tank.Position);
                            prevFireTime = gameTime.TotalGameTime;
                            //audio.Shooting.Play();
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
                    pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.ShipWreckFloatHeight);
                    CastSkill.KnockOutEnemies(gameTime, tank, enemies, ref enemiesAmount, fish, fishAmount, audio);
                }
                //if (!heightMapInfo.IsOnHeightmap(pointIntersect)) pointIntersect = Vector3.Zero;
                tank.Update(currentKeyboardState, enemies, enemiesAmount, fish, fishAmount, null, null, gameTime, pointIntersect);
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
                if ((!(lastKeyboardState.IsKeyDown(Keys.LeftShift) || lastKeyboardState.IsKeyDown(Keys.RightShift))
                        && currentKeyboardState.IsKeyDown(Keys.L)
                        && gameTime.TotalGameTime.TotalSeconds - prevFireTime.TotalSeconds > fireTime.TotalSeconds / (Tank.shootingRate * Tank.fireRateUp)
                        )
                        //||
                        //( (MouseOnEnemy()||MouseOnFish()) && lastMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released && InShootingRange())
                        )
                {
                    prevFireTime = gameTime.TotalGameTime;
                    //audio.Shooting.Play();
                    if (Tank.bulletType == 0) { AddingObjects.placeTankDamageBullet(tank, Content, myBullet); }
                    else if (Tank.bulletType == 1) { AddingObjects.placeHealingBullet(tank, Content, healthBullet); }
                }

                gameCamera.Update(tank.ForwardDirection,
                    tank.Position, aspectRatio, gameTime);
                // Updating camera's frustum
                frustum = new BoundingFrustum(gameCamera.ViewMatrix * gameCamera.ProjectionMatrix);

                for (int i = 0; i < myBullet.Count; i++) {
                    myBullet[i].update();
                }

                for (int i = 0; i < healthBullet.Count; i++) {
                    healthBullet[i].update();
                }

                for (int i = 0; i < enemyBullet.Count; i++) {
                    enemyBullet[i].update();
                }
                Collision.updateBulletOutOfBound(tank.MaxRangeX, tank.MaxRangeZ, healthBullet, myBullet, enemyBullet, alliesBullets, frustum);
                Collision.updateDamageBulletVsBarriersCollision(myBullet, enemies, ref enemiesAmount);
                Collision.updateHealingBulletVsBarrierCollision(healthBullet, fish, fishAmount);
                Collision.updateDamageBulletVsBarriersCollision(enemyBullet, fish, ref fishAmount);
                Collision.updateProjectileHitTank(tank, enemyBullet);

                Collision.deleteSmallerThanZero(enemies, ref enemiesAmount);
                Collision.deleteSmallerThanZero(fish, ref fishAmount);

                for (int i = 0; i < enemiesAmount; i++)
                {
                    if (enemies[i].stunned)
                    {
                        if (gameTime.TotalGameTime.TotalSeconds - enemies[i].stunnedStartTime > GameConstants.timeStunLast)
                            enemies[i].stunned = false;
                    }
                    enemies[i].Update(enemies, enemiesAmount, fish, fishAmount, random.Next(100), tank, enemyBullet, alliesBullets);
                }

                foreach (TreasureChest chest in treasureChests)
                {
                    if (CharacterNearChest(chest.BoundingSphere) && CursorManager.MouseOnChest(cursor, chest.BoundingSphere, chest.Position, gameCamera)
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
                            //this is just for testing
                            //should be removed
                            skillID = 4;
                            chest.skillID = 4;
                            if (chest.skillID == -1)
                            {
                                // give the player some experience as reward
                                Tank.currentExperiencePts += 20;
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
                                if (Tank.skills[chest.skillID] == false)
                                {
                                    Tank.skills[chest.skillID] = true;
                                    Tank.activeSkillID = chest.skillID;
                                    foundRelic = true;
                                }
                                else
                                {
                                    // give the player some experience as reward
                                    Tank.currentExperiencePts += 20;
                                    // show a random painting
                                    paintingToShow = random.Next(oceanPaintings.paintings.Count);
                                    showPainting = true;
                                }
                            }
                        }
                        doubleClicked = false;
                    }
                }

                for (int i = 0; i < fishAmount; i++) {
                    fish[i].Update(gameTime, enemies, enemiesAmount, fish, fishAmount, random.Next(100) ,tank, enemyBullet);
                }

                //for the shader
                m_Timer += (float)gameTime.ElapsedGameTime.Milliseconds / 1000;

                //cursor update
                cursor.Update(gameTime);

                roundTimer -= gameTime.ElapsedGameTime;
                if (roundTimer < TimeSpan.Zero)
                {
                    returnToMain = true;
                }
                // Dead inside a shipwreck
                if (Tank.currentHitPoint <= 0){
                    returnToMain = true;
                }

                base.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime) {
            if (returnToMain) return;

            base.Draw(gameTime);
            if (paused)
            {
                // Draw the "pause" text
                spriteBatch.Begin();
                spriteBatch.Draw(actionTexture, pausePosition, pauseRect,
                    Color.White);
                spriteBatch.End();
            }

            // Change back the config changed by spriteBatch
            RestoreGraphicConfig();

            DrawGameplayScreen(gameTime);

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

            graphics.GraphicsDevice.SetRenderTarget(renderTarget);
            graphics.GraphicsDevice.Clear(Color.DarkSlateBlue);
            
            DrawTerrain(ground.Model);
            // Updating camera's frustum
            frustum = new BoundingFrustum(gameCamera.ViewMatrix * gameCamera.ProjectionMatrix);

            BoundingSphere chestSphere;
            // Drawing ship wrecks
            foreach (TreasureChest treasureChest in treasureChests)
            {
                chestSphere = treasureChest.BoundingSphere;
                chestSphere.Center = treasureChest.Position;
                if (chestSphere.Intersects(frustum))
                {
                    treasureChest.Draw(gameCamera.ViewMatrix,
                        gameCamera.ProjectionMatrix);
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
                }
            }
            for (int i = 0; i < fishAmount; i++)
            {
                if (fish[i].BoundingSphere.Intersects(frustum))
                    fish[i].Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
            }

            for (int i = 0; i < myBullet.Count; i++) {
                myBullet[i].draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
            }

            for (int i = 0; i < healthBullet.Count; i++) {
                healthBullet[i].draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
            }

            for (int i = 0; i < enemyBullet.Count; i++) {
                enemyBullet[i].draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
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
                bubble.Draw(spriteBatch, 1.5f);
            }
            //rs = new RasterizerState();
            //rs.FillMode = FillMode.Solid;
            //GraphicsDevice.RasterizerState = rs;
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
            if (Tank.activeSkillID != -1) DrawActiveSkill();
            cursor.Draw(gameTime);
            spriteBatch.End();
            if (foundRelic)
            {
                spriteBatch.Begin();
                DrawFoundRelicScene(skillID);
                spriteBatch.End();
                RestoreGraphicConfig();
                //return;
            }
            
        }
        private void DrawNoKey()
        {
            string message = "You have not had the key to treasure chests yet, try to help the fish first so that they will help you find the key in return";
            message = AddingObjects.wrapLine(message, 800, paintingFont);
            spriteBatch.Draw(noKeyScreen, new Rectangle(GraphicDevice.Viewport.TitleSafeArea.Center.X - noKeyScreen.Width / 2, GraphicDevice.Viewport.TitleSafeArea.Center.Y - noKeyScreen.Height / 2, noKeyScreen.Width, noKeyScreen.Height), Color.SandyBrown);
            spriteBatch.DrawString(paintingFont, message, new Vector2(GraphicDevice.Viewport.TitleSafeArea.Center.X - 400, 20), Color.White);
        }
        private void DrawPainting()
        {
            spriteBatch.Draw(oceanPaintings.paintings[paintingToShow].painting, 
                new Rectangle(0, 0, GraphicDevice.Viewport.TitleSafeArea.Width, GraphicDevice.Viewport.TitleSafeArea.Height), Color.White);
            spriteBatch.DrawString(paintingFont, oceanPaintings.paintings[paintingToShow].caption, new Vector2(0, 0), oceanPaintings.paintings[paintingToShow].color);
            spriteBatch.DrawString(paintingFont, "Do you know:", new Vector2(GraphicDevice.Viewport.TitleSafeArea.Left, GraphicDevice.Viewport.TitleSafeArea.Center.Y),
                oceanPaintings.paintings[paintingToShow].color);

            String line = AddingObjects.wrapLine(oceanPaintings.paintings[paintingToShow].tip, GraphicDevice.Viewport.TitleSafeArea.Width, paintingFont);
            spriteBatch.DrawString(paintingFont, line,
                new Vector2(GraphicDevice.Viewport.TitleSafeArea.Left, GraphicDevice.Viewport.TitleSafeArea.Center.Y + 100), oceanPaintings.paintings[paintingToShow].color);

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
            str1 = AddingObjects.wrapLine(str1, rectSafeArea.Width - 20, paintingFont);

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

        private void DrawStats()
        {
            float xOffsetText, yOffsetText;
            string str1 = GameConstants.StrTimeRemaining;
            if (roundTimer.Minutes < 10)
                str1 += "0";
            str1 += roundTimer.Minutes + ":";
            if (roundTimer.Seconds < 10)
                str1 += "0";
            str1 += roundTimer.Seconds;
            string str2 = "";// = GameConstants.StrCellsFound + retrievedFruits.ToString() +
            //" of " + fruits.Count;
            Rectangle rectSafeArea;

            //str1 += (roundTimer.Seconds).ToString();

            Vector3 pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.ShipWreckFloatHeight);
            Vector3 mouseDif = pointIntersect - tank.Position;
            float distanceFomTank = mouseDif.Length();
            //str2 += "Xm= " + pointIntersect.X + " Ym= " + pointIntersect.Y + " Zm= " + pointIntersect.Z + " Distance from tank= " + distanceFomTank;
            //str2 += "\nXt= " + tank.pointToMoveTo.X + " Yt= " + tank.pointToMoveTo.Y + " Zt= " + tank.pointToMoveTo.Z;
            float angle = CursorManager.CalculateAngle(pointIntersect, tank.Position);
            str2 += "\nAngle= " + tank.desiredAngle + "Tank FW= " + tank.ForwardDirection;
            Vector3 posDif = tank.pointToMoveTo - tank.Position;
            float distanceToDest = posDif.Length();
            //str2 += "\nDistance= " + distanceToDest;
            str2 += "\nTank Position " + tank.Position;
            //str2 += "\nEnemy Position " + enemies[0].Position;
            //str2 += "\nEnemy amount " + enemies.Length;
            //str2 += "\nFish Position " + fish[0].Position;
            //str2 += "\nFish amount " + fish.Length;
            //str2 += "\nTank Forward Direction " + tank.ForwardDirection;
            //str2 += "\nEnemy FW " + enemies[0].ForwardDirection;
            //str2 += "\nPrevFIre " + enemies[0].prevFire;
            str2 += "\n Tank Health " + Tank.currentHitPoint;
            //str2 += "\n" + tank.skillPrevUsed[0] + " " + tank.skillPrevUsed[1] + " " + tank.skillPrevUsed[2];

            //Display Fish Health
            Fish fishPontedAt = CursorManager.MouseOnWhichFish(cursor, gameCamera, fish, fishAmount);
            if (fishPontedAt != null)
                AddingObjects.DrawHealthBar(HealthBar, game, spriteBatch, statsFont, fishPontedAt.health, fishPontedAt.maxHealth, 5, fishPontedAt.Name, Color.BlueViolet);

            //Display Enemy Health
            BaseEnemy enemyPointedAt = CursorManager.MouseOnWhichEnemy(cursor, gameCamera, enemies, enemiesAmount);
            if (enemyPointedAt != null)
                AddingObjects.DrawHealthBar(HealthBar, game, spriteBatch, statsFont, enemyPointedAt.health, enemyPointedAt.maxHealth, 5, enemyPointedAt.Name, Color.IndianRed);

            //Display Cyborg health
            AddingObjects.DrawHealthBar(HealthBar, game, spriteBatch, statsFont, Tank.currentHitPoint, Tank.maxHitPoint, game.Window.ClientBounds.Height - 60, "HEALTH", Color.Brown);

            //Display Level/Experience Bar
            AddingObjects.DrawLevelBar(HealthBar, game, spriteBatch, statsFont, Tank.currentExperiencePts, Tank.nextLevelExperience, Tank.level, game.Window.ClientBounds.Height - 30, "EXPERIENCE LEVEL", Color.Brown);


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
        public bool CharacterNearChest(BoundingSphere chestSphere)
        {
            if (tank.BoundingSphere.Intersects(chestSphere))
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
