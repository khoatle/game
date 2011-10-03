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
        GameObject ground;
        Camera gameCamera;
        
        GameObject boundingSphere;


        Enemy[] enemies;
        Fish[] fish;

        int enemiesAmount = 0;
        int fishAmount = 0;

        List<DamageBullet> myBullet;
        List<DamageBullet> enemyBullet;
        List<HealthBullet> healthBullet;

        //A tank
        public Tank tank;

        private TimeSpan fireTime;
        private TimeSpan prevFireTime;

        // For drawing the currently selected skill
        protected Texture2D[] skillTextures;
        public string[] iconNames = { "Image/skill0Icon", "Image/skill1Icon", "Image/skill2Icon", "Image/skill3Icon" };
        // For drawing the currently selected bullet type
        protected Texture2D[] bulletTypeTextures;
        public string[] bulletNames = { "Image/skill0Icon", "Image/skill1Icon" };
        // Game is paused?
        protected bool paused;
        protected Vector2 pausePosition;
        protected Rectangle pauseRect = new Rectangle(1, 120, 200, 44);
        protected Texture2D actionTexture;
        protected Texture2D stunnedTexture;
        // He died inside the ship wreck?
        public bool dead;
        // has artifact?
        public int skillID = 0;

        // Frustum of the camera
        BoundingFrustum frustum;

        // For mouse inputs
        bool doubleClicked = false;
        bool clicked = false;
        double clickTimer = 0;

        HeightMapInfo heightMapInfo;

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
            gameCamera = new Camera();
            boundingSphere = new GameObject();
            tank = new Tank(GameConstants.ShipWreckMaxRangeX, GameConstants.ShipWreckMaxRangeZ);
            fireTime = TimeSpan.FromSeconds(0.3f);
            enemies = new Enemy[GameConstants.NumberEnemies];
            fish = new Fish[GameConstants.NumberFish];
            skillTextures = new Texture2D[GameConstants.numberOfSkills];
            bulletTypeTextures = new Texture2D[GameConstants.numBulletTypes];

            enemies = new Enemy[GameConstants.NumberEnemies];
            fish = new Fish[GameConstants.NumberFish];
            
            // for the mouse or touch
            cursor = new Cursor(game, spriteBatch);
            Components.Add(cursor);

            myBullet = new List<DamageBullet>();
            healthBullet = new List<HealthBullet>();
            enemyBullet = new List<DamageBullet>();

            this.Load();
        }

        public void Load()
        {
            statsFont = Content.Load<SpriteFont>("Fonts/StatsFont");
            // Get the audio library
            audio = (AudioLibrary)
                Game.Services.GetService(typeof(AudioLibrary));

            Random random = new Random();
            int random_terrain = random.Next(5);
            string wood_terrain_name = "Image/wood-terrain" + random_terrain;
            System.Diagnostics.Debug.WriteLine(wood_terrain_name);

            ground.Model = Content.Load<Model>(wood_terrain_name);

            boundingSphere.Model = Content.Load<Model>("Models/sphere1uR");
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
                skillTextures[index] = Content.Load<Texture2D>(iconNames[index]);
            }
            // Loading main character bullet icon textures
            for (int index = 0; index < GameConstants.numBulletTypes; index++)
            {
                bulletTypeTextures[index] = Content.Load<Texture2D>(bulletNames[index]);
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
            string wood_terrain_name = "Image/wood-terrain" + random_terrain;
            System.Diagnostics.Debug.WriteLine(wood_terrain_name);

            ground.Model = Content.Load<Model>(wood_terrain_name);
            //reset position for the tank
            tank.Position = Vector3.Zero;
            tank.Position.Y = GameConstants.FloatHeight;
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
            enemiesAmount = GameConstants.NumberEnemies;
            fishAmount = GameConstants.NumberFish;
            InitializeShipField(Content);
        }

        private void InitializeShipField(ContentManager Content)
        {
            dead = false;
            // Initialize the chests here
            // Put the skill in one of it if this.skillID != 0
            AddingObjects.placeEnemies(ref enemiesAmount, enemies, Content, random, fishAmount, fish, null);
            AddingObjects.placeFish(ref fishAmount, fish, Content, random, enemiesAmount, enemies, null);

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
            if (!paused && !dead)
            {
                float aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
                lastKeyboardState = currentKeyboardState;
                //lastMouseState = currentMouseState;
                currentKeyboardState = Keyboard.GetState();
                lastGamePadState = currentGamePadState;
                currentGamePadState = GamePad.GetState(PlayerIndex.One);
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
                        tank.bulletType++;
                        if (tank.bulletType == GameConstants.numBulletTypes) tank.bulletType = 0;

                    }
                    // changing active skill
                    if ((lastKeyboardState.IsKeyDown(Keys.LeftShift) || lastKeyboardState.IsKeyDown(Keys.RightShift)) && ((lastKeyboardState.IsKeyDown(Keys.K)
                            && currentKeyboardState.IsKeyUp(Keys.K)) || (lastMouseState.RightButton == ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Released)))
                    {
                        if (tank.activeSkillID != -1)
                        {
                            tank.activeSkillID++;
                            if (tank.activeSkillID == GameConstants.numberOfSkills) tank.activeSkillID = 0;
                            while (tank.skills[tank.activeSkillID] == false)
                            {
                                tank.activeSkillID++;
                                if (tank.activeSkillID == GameConstants.numberOfSkills) tank.activeSkillID = 0;
                            }
                        }
                    }
                    //if the user wants to move when changing skill or bullet, let him
                    //because this is better for fast action game
                    if (currentMouseState.LeftButton == ButtonState.Pressed)
                    {
                        pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.FloatHeight);
                    }
                }
                //if the user click on right mouse button
                //cast the current selected skill
                //else if (lastMouseState.RightButton == ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Released)
                else if (currentMouseState.RightButton == ButtonState.Pressed)
                {

                    // Hercules' Bow!!!
                    if (tank.activeSkillID == 0 && mouseOnLivingObject)
                    {
                        pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.FloatHeight);
                        tank.ForwardDirection = CursorManager.CalculateAngle(pointIntersect, tank.Position);
                        //if the skill has cooled down
                        //or this is the 1st time the user uses it
                        if ((gameTime.TotalGameTime.TotalSeconds - tank.skillPrevUsed[0] > GameConstants.coolDownForHerculesBow) || tank.firstUse[0] == true)
                        {
                            tank.firstUse[0] = false;
                            tank.skillPrevUsed[0] = gameTime.TotalGameTime.TotalSeconds;
                            audio.Explosion.Play();
                            CastSkill.UseHerculesBow(tank, Content, myBullet);
                        }

                    }
                    //Thor's Hammer!!!
                    if (tank.activeSkillID == 1)
                    {
                        if ((gameTime.TotalGameTime.TotalSeconds - tank.skillPrevUsed[1] > GameConstants.coolDownForArchillesArmor) || tank.firstUse[1] == true)
                        {
                            tank.firstUse[1] = false;
                            tank.skillPrevUsed[1] = gameTime.TotalGameTime.TotalSeconds;
                            audio.Explosion.Play();
                            CastSkill.UseThorHammer(gameTime, tank, enemies, ref enemiesAmount);
                        }
                    }
                    // Achilles' Armor!!!
                    if (tank.activeSkillID == 2)
                    {
                        if ((gameTime.TotalGameTime.TotalSeconds - tank.skillPrevUsed[2] > GameConstants.coolDownForThorHammer) || tank.firstUse[2] == true)
                        {
                            tank.firstUse[2] = false;
                            tank.invincibleMode = true;
                            audio.NewMeteor.Play();
                            tank.skillPrevUsed[2] = gameTime.TotalGameTime.TotalSeconds;
                        }
                    }

                    //Hermes' Winged Sandal!!!
                    if (tank.activeSkillID == 3)
                    {
                        if ((gameTime.TotalGameTime.TotalSeconds - tank.skillPrevUsed[3] > GameConstants.coolDownForHermesSandle) || tank.firstUse[3] == true)
                        {
                            tank.firstUse[3] = false;
                            audio.NewMeteor.Play();
                            tank.skillPrevUsed[3] = gameTime.TotalGameTime.TotalSeconds;
                            tank.supersonicMode = true;
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
                        pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.FloatHeight);
                        tank.ForwardDirection = CursorManager.CalculateAngle(pointIntersect, tank.Position);
                        if (gameTime.TotalGameTime.TotalSeconds - prevFireTime.TotalSeconds > fireTime.TotalSeconds / (tank.shootingRate * tank.fireRateUp))
                        {
                            prevFireTime = gameTime.TotalGameTime;
                            audio.Shooting.Play();
                            if (tank.bulletType == 0) { AddingObjects.placeDamageBullet(tank, Content, myBullet); }
                            else if (tank.bulletType == 1) { AddingObjects.placeHealingBullet(tank, Content, healthBullet); }
                        }
                    }
                    pointIntersect = Vector3.Zero;
                }

                //if the user clicks or holds mouse's left button
                else if (currentMouseState.LeftButton == ButtonState.Pressed && !mouseOnLivingObject)
                {
                    pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.FloatHeight);
                }
                else if (lastMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released)
                {
                    pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.FloatHeight);
                    //if it is out of shooting range then just move there
                    if (!CursorManager.InShootingRange(tank, cursor, gameCamera))
                    {

                    }
                    else
                    {
                        //if the enemy is in the shooting range then shoot it w/o moving to it
                        if (mouseOnLivingObject && gameTime.TotalGameTime.TotalSeconds - prevFireTime.TotalSeconds > fireTime.TotalSeconds / (tank.shootingRate * tank.fireRateUp))
                        {
                            tank.ForwardDirection = CursorManager.CalculateAngle(pointIntersect, tank.Position);
                            prevFireTime = gameTime.TotalGameTime;
                            audio.Shooting.Play();
                            if (tank.bulletType == 0) { AddingObjects.placeDamageBullet(tank, Content, myBullet); }
                            else if (tank.bulletType == 1) { AddingObjects.placeHealingBullet(tank, Content, healthBullet); }
                            //so the tank will not move
                            pointIntersect = Vector3.Zero;
                        }
                        if (doubleClicked == true) pointIntersect = Vector3.Zero;
                    }
                }
                if (tank.supersonicMode == true)
                {
                    pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.FloatHeight);
                    CastSkill.KnockOutEnemies(gameTime, tank, enemies, ref enemiesAmount, audio);
                }
                if (!heightMapInfo.IsOnHeightmap(pointIntersect)) pointIntersect = Vector3.Zero;
                tank.Update(currentKeyboardState, enemies, enemiesAmount, fish, fishAmount, null, gameTime, pointIntersect);
                // Are we shooting?
                if ((!(lastKeyboardState.IsKeyDown(Keys.LeftShift) || lastKeyboardState.IsKeyDown(Keys.RightShift))
                        && currentKeyboardState.IsKeyDown(Keys.L)
                        && gameTime.TotalGameTime.TotalSeconds - prevFireTime.TotalSeconds > fireTime.TotalSeconds / (tank.shootingRate * tank.fireRateUp)
                        )
                        //||
                        //( (MouseOnEnemy()||MouseOnFish()) && lastMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released && InShootingRange())
                        )
                {
                    prevFireTime = gameTime.TotalGameTime;
                    audio.Shooting.Play();
                    if (tank.bulletType == 0) { AddingObjects.placeDamageBullet(tank, Content, myBullet); }
                    else if (tank.bulletType == 1) { AddingObjects.placeHealingBullet(tank, Content, healthBullet); }
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
                Collision.updateBulletOutOfBound(tank.MaxRangeX, tank.MaxRangeZ, healthBullet, myBullet, frustum);
                Collision.updateDamageBulletVsBarriersCollision(myBullet, enemies, ref enemiesAmount);
                Collision.updateHealingBulletVsBarrierCollision(healthBullet, fish, fishAmount);
                Collision.updateDamageBulletVsBarriersCollision(enemyBullet, fish, ref fishAmount);
                Collision.updateProjectileHitTank(tank, enemyBullet);

                for (int i = 0; i < enemiesAmount; i++)
                {
                    if (!enemies[i].stunned)
                        enemies[i].Update(enemies, enemiesAmount, fish, fishAmount, random.Next(100), tank, enemyBullet);
                    //disable stun if stun effect times out
                    else
                    {
                        if (gameTime.TotalGameTime.TotalSeconds - enemies[i].stunnedStartTime > GameConstants.timeStunLast)
                            enemies[i].stunned = false;
                    }
                }


                for (int i = 0; i < fishAmount; i++) {
                    fish[i].Update(enemies, enemiesAmount, fish, fishAmount, random.Next(100) ,tank, enemyBullet);
                }

                roundTimer -= gameTime.ElapsedGameTime;
                if (roundTimer < TimeSpan.Zero)
                {
                    dead = true;
                }
                // Dead inside a shipwreck
                if (tank.hitPoint <= 0){
                    dead = true;
                }

                base.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime) {
            if (dead) return;


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
                }
                mesh.Draw();
            }
        }

        private void DrawGameplayScreen()
        {
            DrawTerrain(ground.Model);
            // Updating camera's frustum
            frustum = new BoundingFrustum(gameCamera.ViewMatrix * gameCamera.ProjectionMatrix);


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
            if (tank.activeSkillID != -1) DrawActiveSkill();
        }
        // Draw the currently selected bullet type
        private void DrawBulletType()
        {
            float xOffsetText, yOffsetText;
            Rectangle rectSafeArea;

            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            xOffsetText = rectSafeArea.Right - 50;
            yOffsetText = rectSafeArea.Top;

            Vector2 bulletIconPosition =
                new Vector2((int)xOffsetText, (int)yOffsetText);

            spriteBatch.Draw(bulletTypeTextures[tank.bulletType], bulletIconPosition, Color.White);
        }
        // Draw the currently selected skill/spell
        private void DrawActiveSkill()
        {
            float xOffsetText, yOffsetText;
            Rectangle rectSafeArea;

            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            xOffsetText = rectSafeArea.Right - 100;
            yOffsetText = rectSafeArea.Top + 50;

            Vector2 skillIconPosition =
                new Vector2((int)xOffsetText, (int)yOffsetText);

            spriteBatch.Draw(skillTextures[tank.activeSkillID], skillIconPosition, Color.White);
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

            Vector3 pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.FloatHeight);
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
            str2 += "\n Tank Health " + tank.hitPoint;
            //str2 += "\n" + tank.skillPrevUsed[0] + " " + tank.skillPrevUsed[1] + " " + tank.skillPrevUsed[2];

            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            xOffsetText = rectSafeArea.X;
            yOffsetText = rectSafeArea.Y;

            Vector2 strSize = statsFont.MeasureString(str1);
            Vector2 strPosition =
                new Vector2((int)xOffsetText + 10, (int)yOffsetText);

            spriteBatch.DrawString(statsFont, str1, strPosition, Color.White);
            strPosition.Y += strSize.Y;
            spriteBatch.DrawString(statsFont, str2, strPosition, Color.White);

        }    
    }
}
