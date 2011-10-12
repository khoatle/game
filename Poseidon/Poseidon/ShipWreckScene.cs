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
        GraphicsDevice GraphicDevice;
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
        SpriteFont menuSmall;
        GameObject ground;
        Camera gameCamera;
        
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

        public ShipWreckScene(Game game, GraphicsDeviceManager graphics, ContentManager Content, GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch, Vector2 pausePosition, Rectangle pauseRect, Texture2D actionTexture, CutSceneDialog cutSceneDialog, Texture2D stunnedTexture)
            : base(game)
        {
            this.graphics = graphics;
            this.Content = Content;
            this.GraphicDevice = GraphicsDevice;
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
            enemies = new BaseEnemy[GameConstants.ShipNumberEnemies];
            fish = new Fish[GameConstants.ShipNumberFish];
            skillTextures = new Texture2D[GameConstants.numberOfSkills];
            bulletTypeTextures = new Texture2D[GameConstants.numBulletTypes];

            //enemies = new Enemy[GameConstants.NumberEnemies];
            //fish = new Fish[GameConstants.NumberFish];
            
            // for the mouse or touch
            cursor = new Cursor(game, spriteBatch);
            Components.Add(cursor);

            myBullet = new List<DamageBullet>();
            healthBullet = new List<HealthBullet>();
            enemyBullet = new List<DamageBullet>();
            alliesBullets = new List<DamageBullet>();
            
            this.Load();
        }

        public void Load()
        {
            statsFont = Content.Load<SpriteFont>("Fonts/StatsFont");
            menuSmall = Content.Load<SpriteFont>("Fonts/menuSmall");
            // Get the audio library
            audio = (AudioLibrary)
                Game.Services.GetService(typeof(AudioLibrary));

            //Random random = new Random();
            //int random_terrain = random.Next(5);
            //string wood_terrain_name = "Image/wood-terrain" + random_terrain;
            //System.Diagnostics.Debug.WriteLine(wood_terrain_name);

            //ground.Model = Content.Load<Model>(wood_terrain_name);
            ground.Model = Content.Load<Model>("Models/shipwreckscene");
            boundingSphere.Model = Content.Load<Model>("Models/sphere1uR");
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
            HealthBar = Content.Load<Texture2D>("Image/HealthBar");
        }

        /// <summary>
        /// Show the action scene
        /// </summary>

        public override void Show()
        {
            paused = false;
            //initialize random shipwreck terrain
            Random random = new Random();
            int random_terrain = random.Next(5);
            //string wood_terrain_name = "Image/wood-terrain" + random_terrain;
            //System.Diagnostics.Debug.WriteLine(wood_terrain_name);

            //ground.Model = Content.Load<Model>(wood_terrain_name);
            //reset position for the tank
            tank.Position = Vector3.Zero;
            tank.Position.Y = GameConstants.ShipWreckFloatHeight;
            tank.ForwardDirection = 0f;
            MediaPlayer.Play(audio.BackMusic);
            InitializeShipField(Content);
            base.Show();
        }

        private void ResetGame(GameTime gameTime, float aspectRatio)
        {
            
            tank.Reset();
            gameCamera.Update(tank.ForwardDirection,
                tank.Position, aspectRatio);
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
                        staticObjects[index].LoadContent(Content, "Models/barrel");
                        break;
                    case 1:
                        staticObjects[index].LoadContent(Content, "Models/barrelstack");
                        break;
                    case 2:
                        staticObjects[index].LoadContent(Content, "Models/boxstack");
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
            float aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
            lastKeyboardState = currentKeyboardState;
            //lastMouseState = currentMouseState;
            currentKeyboardState = Keyboard.GetState();
            lastGamePadState = currentGamePadState;
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
            if (foundRelic)
            {
                // Next sentence when the user press Enter
                if ((lastKeyboardState.IsKeyDown(Keys.Enter) &&
                    (currentKeyboardState.IsKeyUp(Keys.Enter))) ||
                    currentGamePadState.Buttons.Start == ButtonState.Pressed)
                {
                    foundRelic = false;
                }
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
                            audio.Explosion.Play();
                            CastSkill.UseHerculesBow(tank, Content, myBullet);
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
                            CastSkill.UseThorHammer(gameTime, tank, enemies, ref enemiesAmount);
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
                            audio.Shooting.Play();
                            if (Tank.bulletType == 0) { AddingObjects.placeTankDamageBullet(tank, Content, myBullet); }
                            else if (Tank.bulletType == 1) { AddingObjects.placeHealingBullet(tank, Content, healthBullet); }
                            //so the tank will not move
                            pointIntersect = Vector3.Zero;
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
                    CastSkill.KnockOutEnemies(gameTime, tank, enemies, ref enemiesAmount, audio);
                }
                //if (!heightMapInfo.IsOnHeightmap(pointIntersect)) pointIntersect = Vector3.Zero;
                tank.Update(currentKeyboardState, enemies, enemiesAmount, fish, fishAmount, null, null, gameTime, pointIntersect);
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
                    audio.Shooting.Play();
                    if (Tank.bulletType == 0) { AddingObjects.placeTankDamageBullet(tank, Content, myBullet); }
                    else if (Tank.bulletType == 1) { AddingObjects.placeHealingBullet(tank, Content, healthBullet); }
                }

                gameCamera.Update(tank.ForwardDirection,
                    tank.Position, aspectRatio);
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
                        chest.opened = true;
                        audio.OpenChest.Play();
                        chest.Model = Content.Load<Model>("Models/chest");
                        if (chest.skillID == -1)
                        {
                            // give the player some experience
                            Tank.currentHitPoint += 30;
                        }
                        else 
                        {
                            // player found a God's relic
                            // unlock a skill
                            Tank.skills[chest.skillID] = true;
                            Tank.activeSkillID = chest.skillID;
                            foundRelic = true;
                        }
                    }
                }

                for (int i = 0; i < fishAmount; i++) {
                    fish[i].Update(gameTime, enemies, enemiesAmount, fish, fishAmount, random.Next(100) ,tank, enemyBullet);
                }

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
                spriteBatch.Draw(actionTexture, pausePosition, pauseRect,
                    Color.White);
            }

            // Change back the config changed by spriteBatch
            GraphicDevice.BlendState = BlendState.Opaque;
            GraphicDevice.DepthStencilState = DepthStencilState.Default;
            GraphicDevice.SamplerStates[0] = SamplerState.LinearWrap;

            DrawGameplayScreen();

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

        private void DrawGameplayScreen()
        {
            graphics.GraphicsDevice.Clear(Color.Black);
            if (foundRelic)
            {
                DrawFoundRelicScene(skillID);
                return;
            }
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
                        spriteBatch.Draw(stunnedTexture, drawPos, Color.White);
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

            //rs = new RasterizerState();
            //rs.FillMode = FillMode.Solid;
            //GraphicsDevice.RasterizerState = rs;
            DrawStats();
            DrawBulletType();
            if (Tank.activeSkillID != -1) DrawActiveSkill();
            
        }
        private void DrawFoundRelicScene(int skillID)
        {
            float xOffsetText, yOffsetText;
            string str1 = "You have found relic " + skillID;
            Rectangle rectSafeArea;
            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            xOffsetText = rectSafeArea.X;
            yOffsetText = rectSafeArea.Y;

            Vector2 strSize = statsFont.MeasureString(str1);
            Vector2 strPosition =
                new Vector2((int)xOffsetText + 10, (int)yOffsetText);
            spriteBatch.DrawString(statsFont, str1, strPosition, Color.White);
            xOffsetText = rectSafeArea.Right - 100;
            yOffsetText = rectSafeArea.Top + 50;

            Vector2 skillIconPosition =
                new Vector2((int)xOffsetText, (int)yOffsetText);

            spriteBatch.Draw(skillTextures[Tank.activeSkillID], skillIconPosition, Color.White);
        }
        // Draw the currently selected bullet type
        private void DrawBulletType()
        {
            int xOffsetText, yOffsetText;
            Rectangle rectSafeArea;

            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            xOffsetText = rectSafeArea.Right - 100;
            yOffsetText = rectSafeArea.Top;

            Vector2 bulletIconPosition =
                new Vector2((int)xOffsetText, (int)yOffsetText);
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

            xOffsetText = rectSafeArea.Right - 100;
            yOffsetText = rectSafeArea.Top + 50;

            Vector2 skillIconPosition =
                new Vector2((int)xOffsetText, (int)yOffsetText);
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
            AddingObjects.DrawLevelBar(HealthBar, game, spriteBatch, statsFont, Tank.currentExperiencePts, Tank.nextLevelExperience, Tank.level, game.Window.ClientBounds.Height - 30, "LEVEL", Color.GreenYellow);


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
    }
}
