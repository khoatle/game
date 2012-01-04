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

        //Points gained
        public static List<Point> points;

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
            hydroBot = new HydroBot(GameConstants.ShipWreckMaxRangeX, GameConstants.ShipWreckMaxRangeZ, GameConstants.ShipWreckFloatHeight, GameMode.ShipWreck);
            //fireTime = TimeSpan.FromSeconds(0.3f);
            if (PlayGameScene.currentLevel >= 4)
                enemiesAmount = GameConstants.ShipHighNumberShootingEnemies + GameConstants.ShipHighNumberCombatEnemies;
            else enemiesAmount = GameConstants.ShipLowNumberShootingEnemies + GameConstants.ShipLowNumberCombatEnemies;
            enemies = new BaseEnemy[enemiesAmount];
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
            points = new List<Point>();

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

            hydroBot.Load(Content);

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
            hydroBot.Position = Vector3.Zero;
            hydroBot.Position.Y = GameConstants.ShipWreckFloatHeight;
            hydroBot.ForwardDirection = 0f;
            //MediaPlayer.Play(audio.BackMusic);
            showNoKey = false;
            showPainting = false;
            cursor.targetToLock = null;
            InitializeShipField(Content);
            base.Show();
        }

        private void ResetGame(GameTime gameTime, float aspectRatio)
        {
            
            hydroBot.Reset();
            gameCamera.Update(hydroBot.ForwardDirection,
                hydroBot.Position, aspectRatio, gameTime);
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
                if (index == 0 && skillID != -1) treasureChests[index].LoadContent(Content, randomType, skillID);
                else treasureChests[index].LoadContent(Content, randomType, -1);
                randomType = random.Next(3);
            }
            if (PlayGameScene.currentLevel >= 4)
                enemiesAmount = GameConstants.ShipHighNumberShootingEnemies + GameConstants.ShipHighNumberCombatEnemies;
            else enemiesAmount = GameConstants.ShipLowNumberShootingEnemies + GameConstants.ShipLowNumberCombatEnemies;
            enemies = new BaseEnemy[enemiesAmount];
            AddingObjects.placeEnemies(ref enemiesAmount, enemies, Content, random, fishAmount, fish, null,
                GameConstants.ShipWreckMinRangeX,GameConstants.ShipWreckMaxRangeX, GameConstants.ShipWreckMinRangeZ, GameConstants.ShipWreckMaxRangeZ, 0 , GameMode.ShipWreck, GameConstants.ShipWreckFloatHeight);
            AddingObjects.placeFish(ref fishAmount, fish, Content, random, enemiesAmount, enemies, null,
                GameConstants.ShipWreckMinRangeX, GameConstants.ShipWreckMaxRangeX, GameConstants.ShipWreckMinRangeZ, GameConstants.ShipWreckMaxRangeZ, 0, GameMode.ShipWreck, GameConstants.ShipWreckFloatHeight);
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
            if (foundRelic)
            {
                // return to game if enter pressed
                if (chestExitPressed)
                {
                    foundRelic = false;
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
                
                //hydrobot update
                hydroBot.UpdateAction(gameTime, cursor, gameCamera, enemies, enemiesAmount, fish, fishAmount, Content, spriteBatch, myBullet, this, heightMapInfo, healthBullet, null, null, null, null, null);

                //add 1 bubble over tank and each enemy
                timeNextBubble -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (timeNextBubble <= 0)
                {
                    Bubble bubble = new Bubble();
                    bubble.LoadContent(Content, hydroBot.Position, false, 0.025f);
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
                    myBullet[i].update();
                }

                for (int i = 0; i < healthBullet.Count; i++) {
                    healthBullet[i].update();
                }

                for (int i = 0; i < enemyBullet.Count; i++) {
                    enemyBullet[i].update();
                }
                Collision.updateBulletOutOfBound(hydroBot.MaxRangeX, hydroBot.MaxRangeZ, healthBullet, myBullet, enemyBullet, alliesBullets, frustum);
                Collision.updateDamageBulletVsBarriersCollision(myBullet, enemies, ref enemiesAmount, frustum, GameMode.ShipWreck, gameTime, hydroBot,
                    enemies, enemiesAmount, fish, fishAmount, gameCamera);
                Collision.updateHealingBulletVsBarrierCollision(healthBullet, fish, fishAmount, frustum, GameMode.ShipWreck);
                Collision.updateDamageBulletVsBarriersCollision(enemyBullet, fish, ref fishAmount, frustum, GameMode.ShipWreck, gameTime, hydroBot,
                    enemies, enemiesAmount, fish, fishAmount, gameCamera);
                Collision.updateProjectileHitBot(hydroBot, enemyBullet, GameMode.ShipWreck, enemies, enemiesAmount);
                Collision.updateDamageBulletVsBarriersCollision(alliesBullets, enemies, ref enemiesAmount, frustum, GameMode.ShipWreck, gameTime, hydroBot,
                    enemies, enemiesAmount, fish, fishAmount, gameCamera);

                Collision.deleteSmallerThanZero(enemies, ref enemiesAmount, frustum, GameMode.ShipWreck, cursor);
                Collision.deleteSmallerThanZero(fish, ref fishAmount, frustum, GameMode.ShipWreck, cursor);

                for (int i = 0; i < enemiesAmount; i++)
                {
                    if (enemies[i].stunned)
                    {
                        if (PoseidonGame.playTime.TotalSeconds - enemies[i].stunnedStartTime > GameConstants.timeStunLast)
                            enemies[i].stunned = false;
                    }
                    enemies[i].Update(enemies, enemiesAmount, fish, fishAmount, random.Next(100), hydroBot, enemyBullet, alliesBullets, frustum, gameTime, GameMode.ShipWreck);
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
                                    HydroBot.activeSkillID = chest.skillID;
                                    foundRelic = true;
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
                        }
                        doubleClicked = false;
                    }
                }

                for (int i = 0; i < fishAmount; i++) {
                    fish[i].Update(gameTime, enemies, enemiesAmount, fish, fishAmount, random.Next(100) ,hydroBot, enemyBullet);
                }

                //for the shader
                m_Timer += (float)gameTime.ElapsedGameTime.Milliseconds / 1000;

                //cursor update
                cursor.Update(GraphicDevice, gameCamera, gameTime, frustum);

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

            graphics.GraphicsDevice.SetRenderTarget(renderTarget);
            graphics.GraphicsDevice.Clear(Color.Black);
            
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
            hydroBot.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
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

            //Draw points gained / lost
            foreach (Point point in points)
            {
                point.Draw(spriteBatch);
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
            if (HydroBot.activeSkillID != -1) DrawActiveSkill();
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
            Fish fishPontedAt = CursorManager.MouseOnWhichFish(cursor, gameCamera, fish, fishAmount);
            if (fishPontedAt != null)
                IngamePresentation.DrawHealthBar(HealthBar, game, spriteBatch, statsFont, (int)fishPontedAt.health, (int)fishPontedAt.maxHealth, 5, fishPontedAt.Name, Color.BlueViolet);

            //Display Enemy Health
            BaseEnemy enemyPointedAt = CursorManager.MouseOnWhichEnemy(cursor, gameCamera, enemies, enemiesAmount);
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
