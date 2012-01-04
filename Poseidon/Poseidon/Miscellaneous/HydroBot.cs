﻿#region File Description
//-----------------------------------------------------------------------------
// Tank.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using SkinnedModel;
using Poseidon.Core;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
#endregion

namespace Poseidon
{
    /// <summary>
    /// Helper class for drawing a tank model with animated wheels and turret.
    /// </summary>
    [Serializable()]    
    public class HydroBot : GameObject, ISerializable
    {
        public float ForwardDirection { get; set; }

        //For the animation
        Model botModel;
        Matrix[] bones;
        SkinningData skd;
        public ClipPlayer clipPlayer;
        Matrix charMatrix;
        Quaternion qRotation = Quaternion.Identity;

        //Attributes of our main character ls=LevelStart
        public static float strength, lsStrength;
        public static float speed, lsSpeed;
        public static float shootingRate, lsShootingRate;
        public static float maxHitPoint, lsMaxHitPoint;
        public static float currentHitPoint;//, lsCurrentHitPoint;
        public static int currentEnvPoint, lsCurrentEnvPoint;
        public static int maxEnvPoint;
        // 2 types of bullet
        // 0: killing
        // 1: healing
        public static int bulletType;

        //Skills/Spells of our main character
        //true = enabled/found
        public static bool[] skills, lsSkills;
        //skill combo activated?
        public static bool skillComboActivated;
        //which skill is being selected
        public static int activeSkillID, lsActiveSkillID, secondSkillID;
        //time that skills were previously casted
        //for managing skills' cool down time
        public static double[] skillPrevUsed;
        //invincible mode when Achilles' Armor is used
        public static bool invincibleMode;
        //supersonic mode when using Hermes' winged sandal
        public static bool supersonicMode;
        //auto-hipnotize mode when using skill combo: armor - belt
        public static bool autoHipnotizeMode;
        //if it is the 1st time the user use it
        //let him use it
        public static bool[] firstUse;

        //Cool down time for plant
        public static double prevPlantTime;
        public static bool firstPlant;

        //Sphere for interacting with trashs and fruits
        public BoundingSphere Trash_Fruit_BoundingSphere;
        //SoundEffect RetrievedSound;
        //temporary power-up for the cyborg
        //int tempPower;
        public static float speedUp;
        public static float strengthUp;
        public static float fireRateUp;
        public static double strengthUpStartTime;
        public static double speedUpStartTime;
        public static double fireRateUpStartTime;

        public float desiredAngle;
        public Vector3 pointToMoveTo;
        public bool reachDestination = true;

        public float floatHeight;

        public static int currentExperiencePts, lsCurrentExperiencePts;
        public static int nextLevelExperience, lsNextLevelExperience;
        //private static int increaseBy, lsIncreaseBy;
        public static int level, lsLevel; // experience level
        public static int unassignedPts, lsUnassignedPts;

        // Tank moving bound
        public int MaxRangeX;
        public int MaxRangeZ;

        public static bool isPoissoned;
        public static float accumulatedHealthLossFromPoisson;
        public static float maxHPLossFromPoisson;
        public static float poissonInterval;

        // which game mode is this hydrobot in?
        public GameMode gameMode;

        public static int gamePlusLevel = 0; //Every time you beat the game, gameplus level increases

        // input management
        KeyboardState lastKeyboardState = new KeyboardState();
        KeyboardState currentKeyboardState = new KeyboardState();
        MouseState currentMouseState = new MouseState();
        MouseState lastMouseState = new MouseState();

        // For mouse inputs
        bool doubleClicked = false;
        bool clicked = false;
        double clickTimer = 0;

        private TimeSpan prevFireTime;

        public HydroBot(int MaxRangeX, int MaxRangeZ, float floatHeight, GameMode gameMode)
        {
            // Original attribute
            strength = lsStrength = GameConstants.MainCharStrength;
            speed = lsSpeed = GameConstants.BasicStartSpeed;
            shootingRate = lsShootingRate = GameConstants.MainCharShootingSpeed;
            bulletType = 1;
            maxHitPoint = lsMaxHitPoint = GameConstants.PlayerStartingHP;
            currentHitPoint = GameConstants.PlayerStartingHP;
            maxEnvPoint = GameConstants.MaxEnv;
            currentEnvPoint = lsCurrentEnvPoint = GameConstants.PlayerStartingEnv;
            pointToMoveTo = Vector3.Zero;

            // No buff up at the beginning
            speedUp = 1.0f;
            strengthUp = 1.0f;
            fireRateUp = 1.0f;

            skills = new bool[GameConstants.numberOfSkills];
            lsSkills = new bool[GameConstants.numberOfSkills];
            skillPrevUsed = new double[GameConstants.numberOfSkills];
            firstUse = new bool[GameConstants.numberOfSkills];
            for (int index = 0; index < GameConstants.numberOfSkills; index++)
            {
                skills[index] = false;
            }
            skillComboActivated = false;

            //if(PlayGameScene.currentLevel == 0 && gameMode == GameMode.MainGame) //to take care of reload
            //    skills[index] = false;
            this.floatHeight = floatHeight;
            Position.Y = floatHeight;

            this.MaxRangeX = MaxRangeX;
            this.MaxRangeZ = MaxRangeZ;

            currentExperiencePts = lsCurrentExperiencePts = 0;
            nextLevelExperience = lsNextLevelExperience = GameConstants.MainCharLevelOneExp;
            level = lsLevel = 1;
            unassignedPts = lsUnassignedPts = 0;

            activeSkillID = lsActiveSkillID = secondSkillID = -1;
            invincibleMode = false;
            supersonicMode = false;
            autoHipnotizeMode = false;

            isPoissoned = false;
            poissonInterval = 0;
            maxHPLossFromPoisson = 50;
            accumulatedHealthLossFromPoisson = 0;

            this.gameMode = gameMode;
        }

        /// <summary>
        /// Deserialize the saved data from file. Overrides the function in Gameobject.
        /// This function is invoked from ObjectsToSerialize which is invoked by Deserialize objects.
        /// </summary>
        public HydroBot(SerializationInfo info, StreamingContext context)
        {
            strength = lsStrength = (float)info.GetValue("strength", typeof(float));
            speed = lsSpeed = (float)info.GetValue("speed",typeof(float));
            shootingRate = lsShootingRate = (float)info.GetValue("shootingRate", typeof(float));
            bulletType = 1;
            maxHitPoint = lsMaxHitPoint = (float)info.GetValue("maxHitPoint",typeof(float));
            currentHitPoint = maxHitPoint;
            currentEnvPoint = lsCurrentEnvPoint = (int)info.GetValue("currentEnvPoint",typeof(int));
            maxEnvPoint = GameConstants.MaxEnv;

            //deduct the amount of trash dropped
            if (gameMode != GameMode.SurvivalMode)
            {
                currentEnvPoint -= (GameConstants.NumberTrash[PlayGameScene.currentLevel] * GameConstants.envLossPerTrashAdd);
                if (currentEnvPoint < GameConstants.EachLevelMinEnv) currentEnvPoint = GameConstants.EachLevelMinEnv;
            }

            skills = new bool[GameConstants.numberOfSkills];
            lsSkills = new bool[GameConstants.numberOfSkills];
            skillPrevUsed = new double[GameConstants.numberOfSkills];
            firstUse = new bool[GameConstants.numberOfSkills];
            for (int i = 0; i < GameConstants.numberOfSkills; i++)
            {
                if (PlayGameScene.currentLevel == 0) // No skill in level 0 (gamePLus)
                {
                    lsSkills[i] = skills[i] = false;
                }
                else
                {
                    string Skillname = "skills" + i.ToString();
                    lsSkills[i] = skills[i] = (bool)info.GetValue(Skillname, typeof(bool));
                }
            }
            if (PlayGameScene.currentLevel == 0) // No skill in level 0 (gamePLus)
                activeSkillID = lsActiveSkillID = -1;
            else
                activeSkillID = lsActiveSkillID = (int)info.GetValue("activeSkillID", typeof(int));
            pointToMoveTo = Vector3.Zero;

            // No buff up at the beginning
            speedUp = 1.0f;
            strengthUp = 1.0f;
            fireRateUp = 1.0f;

            floatHeight = (float)info.GetValue("floatHeight", typeof(float));
            Position.Y = floatHeight;

            MaxRangeX = (int)info.GetValue("MaxRangeX", typeof(int));
            MaxRangeZ = (int)info.GetValue("MaxRangeZ", typeof(int));


            currentExperiencePts = lsCurrentExperiencePts = (int)info.GetValue("currentExperiencePts",typeof(int));
            nextLevelExperience = lsNextLevelExperience = (int)info.GetValue("nextLevelExperience",typeof(int));
            //increaseBy = lsIncreaseBy = (int)info.GetValue("increaseBy",typeof(int));
            level = lsLevel = (int)info.GetValue("level",typeof(int));
            unassignedPts = lsUnassignedPts = (int)info.GetValue("unassignedPts",typeof(int));

            invincibleMode = false;
            supersonicMode = false;
            autoHipnotizeMode = false;

            isPoissoned = false;
            poissonInterval = 0;
            maxHPLossFromPoisson = 50;
            accumulatedHealthLossFromPoisson = 0;

            gamePlusLevel = (int)info.GetValue("gamePlusLevel", typeof(int));

        }

        /// <summary>
        /// Serialize the data to save game to file. Invoked by ObjectsToSerialize.GetObjectData
        /// which is invoked by Serialiser.SerializeObjects
        /// </summary>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("strength", strength);
            info.AddValue("speed", speed);
            info.AddValue("shootingRate", shootingRate);
            info.AddValue("maxHitPoint", maxHitPoint);
            info.AddValue("currentEnvPoint", currentEnvPoint);

            for (int i = 0; i < GameConstants.numberOfSkills; i++)
            {
                string Skillname = "skills" + i.ToString();
                info.AddValue(Skillname, skills[i]);
            }
            info.AddValue("activeSkillID", activeSkillID);

            info.AddValue("floatHeight", floatHeight);
            info.AddValue("MaxRangeX", MaxRangeX);
            info.AddValue("MaxRangeZ", MaxRangeZ);

            info.AddValue("currentExperiencePts", currentExperiencePts);
            info.AddValue("nextLevelExperience", nextLevelExperience);
            info.AddValue("level", level);
            info.AddValue("unassignedPts", unassignedPts);
            info.AddValue("gamePlusLevel", gamePlusLevel);
        }

        /// <summary>
        /// Loads the tank model.
        /// </summary>
        public void Load(ContentManager content)
        {
            // Load the tank model from the ContentManager.
            botModel = content.Load<Model>("Models/MainCharacter/bot");
            Model = botModel;

            ForwardDirection = 0.0f;
            //MaxRange = GameConstants.MaxRange;
            BoundingSphere = CalculateBoundingSphere();

            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;
            scaledSphere.Center.Y = floatHeight;
            scaledSphere.Radius *=
                GameConstants.TankBoundingSphereFactor;
            BoundingSphere =
                new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);

            //no skill yet used
            for (int index = 0; index < GameConstants.numberOfSkills; index++)
            {
                
                firstUse[index] = true;
                skillPrevUsed[index] = 0;
            }

            invincibleMode = false;
            supersonicMode = false;
            autoHipnotizeMode = false;
            //just for testing
            //should be removed
            skillComboActivated = true;
            activeSkillID = 4;
            secondSkillID = -1;
            skills[0] = true;
            skills[1] = true;
            skills[2] = true;
            skills[3] = true;
            skills[4] = true;

            firstPlant = true;
            prevPlantTime = 0;

            LoadAnimation(1, 30, 24);
        }

        public void LoadAnimation(int clipStart, int clipEnd, int fpsRate)
        {
            //for the animation
            skd = Model.Tag as SkinningData;
            clipPlayer = new ClipPlayer(skd, fpsRate);//ClipPlayer running at 24 frames/sec
            AnimationClip clip = skd.AnimationClips["Take 001"]; //Take name from the dude.fbx file
            clipPlayer.play(clip, clipStart, clipEnd, true);
            charMatrix = Matrix.CreateScale(0.1f) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                               Matrix.CreateTranslation(Position);
        }

        public void ResetToLevelStart()
        {
            strength = lsStrength;
            speed = lsSpeed;
            shootingRate = lsShootingRate;
            bulletType = 1;
            maxHitPoint = lsMaxHitPoint;
            currentEnvPoint = lsCurrentEnvPoint;
            lsSkills.CopyTo(skills, 0);
            activeSkillID = lsActiveSkillID;

            currentExperiencePts = lsCurrentExperiencePts;
            nextLevelExperience = lsNextLevelExperience;
            //increaseBy = lsIncreaseBy;
            level = lsLevel;
            unassignedPts = lsUnassignedPts;

        }

        public void SetLevelStartValues()
        {
            // Original attribute
            lsStrength = strength;
            lsSpeed = speed;
            lsShootingRate = shootingRate;
            lsMaxHitPoint = maxHitPoint;
            //lsCurrentHitPoint = currentHitPoint;

            skills.CopyTo(lsSkills, 0);

            lsActiveSkillID = activeSkillID;

            lsCurrentExperiencePts = currentExperiencePts;
            lsNextLevelExperience = nextLevelExperience;
            //lsIncreaseBy = increaseBy;
            lsLevel = level;
            lsUnassignedPts = unassignedPts;
            lsCurrentEnvPoint = currentEnvPoint;
        }

        internal void Reset()
        {
            Position = Vector3.Zero;
            Position.Y = floatHeight;
            ForwardDirection = 0f;
            pointToMoveTo = Vector3.Zero;
            reachDestination = true;
            for (int index = 0; index < GameConstants.numberOfSkills; index++)
            {
                firstUse[index] = true;
                skillPrevUsed[index] = 0;
            }
            invincibleMode = false;
            supersonicMode = false;
            autoHipnotizeMode = false;
            // No buff up at the beginning
            speedUp = 1.0f;
            strengthUp = 1.0f;
            fireRateUp = 1.0f;
            strengthUpStartTime = 0;
            speedUpStartTime = 0;
            fireRateUpStartTime = 0;
            currentHitPoint = maxHitPoint;
            isPoissoned = false;
            firstPlant = true;
            prevPlantTime = 0;
            accumulatedHealthLossFromPoisson = 0;
            PlayGameScene.points.Clear();
            ShipWreckScene.points.Clear();
            if(PlayGameScene.currentLevel > 0 && gameMode != GameMode.SurvivalMode)
                currentEnvPoint -= (GameConstants.NumberTrash[PlayGameScene.currentLevel] * GameConstants.envLossPerTrashAdd);
            if (currentEnvPoint < GameConstants.EachLevelMinEnv) currentEnvPoint = GameConstants.EachLevelMinEnv;
        }

        public void UpdateAction(GameTime gameTime, Cursor cursor, Camera gameCamera, BaseEnemy[] enemies, int enemiesAmount, Fish[] fish, int fishAmount, ContentManager Content,
            SpriteBatch spriteBatch, List<DamageBullet> myBullet, GameScene gameScene, HeightMapInfo heightMapInfo, List<HealthBullet> healthBullet, List<Fruit> fruits,
            List<Trash> trashes, List<ShipWreck> shipWrecks, List<Plant> plants, List<StaticObject> staticObjects)
        {
            lastKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            CursorManager.CheckClick(ref lastMouseState, ref currentMouseState, gameTime, ref clickTimer, ref clicked, ref doubleClicked);
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
                    //at level 0, player is only able to heal
                    if (!(gameMode == GameMode.MainGame && PlayGameScene.currentLevel == 0))
                    {
                        HydroBot.bulletType++;
                        if (HydroBot.bulletType == GameConstants.numBulletTypes) HydroBot.bulletType = 0;
                        PoseidonGame.audio.ChangeBullet.Play();
                    }

                }
                // changing active skill
                //if ((lastKeyboardState.IsKeyDown(Keys.LeftShift) || lastKeyboardState.IsKeyDown(Keys.RightShift)) && ((lastKeyboardState.IsKeyDown(Keys.K)
                //        && currentKeyboardState.IsKeyUp(Keys.K)) || (lastMouseState.RightButton == ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Released)))
                //{
                //    if (HydroBot.activeSkillID != -1)
                //    {
                //        HydroBot.activeSkillID++;
                //        if (HydroBot.activeSkillID == GameConstants.numberOfSkills) HydroBot.activeSkillID = 0;
                //        while (HydroBot.skills[HydroBot.activeSkillID] == false)
                //        {
                //            HydroBot.activeSkillID++;
                //            if (HydroBot.activeSkillID == GameConstants.numberOfSkills) HydroBot.activeSkillID = 0;
                //        }
                //    }
                //}
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
                CastSkill.UseSkill(mouseOnLivingObject, pointIntersect, cursor, gameCamera, gameMode, this, gameScene, Content, spriteBatch, gameTime, myBullet, enemies, ref enemiesAmount, fish, ref fishAmount);
            }

            //if the user holds down Ctrl button
            //just shoot at wherever the mouse is pointing w/o moving
            else if (currentKeyboardState.IsKeyDown(Keys.RightControl) || currentKeyboardState.IsKeyDown(Keys.LeftControl))
            {
                if (currentMouseState.LeftButton == ButtonState.Pressed)
                {
                    pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);
                    ForwardDirection = CursorManager.CalculateAngle(pointIntersect, Position);
                    if (PoseidonGame.playTime.TotalSeconds - prevFireTime.TotalSeconds > GameConstants.MainCharBasicTimeBetweenFire.TotalSeconds / (HydroBot.shootingRate * HydroBot.fireRateUp))
                    {
                        prevFireTime = PoseidonGame.playTime;
                        //audio.Shooting.Play();
                        if (HydroBot.bulletType == 0) { AddingObjects.placeBotDamageBullet(this, Content, myBullet); }
                        else if (HydroBot.bulletType == 1) { AddingObjects.placeHealingBullet(this, Content, healthBullet); }
                        if (!clipPlayer.inRange(61, 90))
                            clipPlayer.switchRange(61, 90);
                    }
                    //hydroBot.reachDestination = true;
                }
                pointIntersect = Vector3.Zero;
                reachDestination = true;
            }
            //if the user clicks or holds mouse's left button
            else if (currentMouseState.LeftButton == ButtonState.Pressed && !mouseOnLivingObject)
            {
                pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);
                if (!clipPlayer.inRange(1, 30))
                    clipPlayer.switchRange(1, 30);
            }

            else if (lastMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released)
            {
                pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);
                //if it is out of shooting range then just move there
                if (!CursorManager.InShootingRange(this, cursor, gameCamera, GameConstants.MainGameFloatHeight))
                {
                    if (!clipPlayer.inRange(1, 30))
                        clipPlayer.switchRange(1, 30);
                }
                else
                {
                    //if the enemy is in the shooting range then shoot it w/o moving to it
                    if (mouseOnLivingObject && PoseidonGame.playTime.TotalSeconds - prevFireTime.TotalSeconds > GameConstants.MainCharBasicTimeBetweenFire.TotalSeconds / (HydroBot.shootingRate * HydroBot.fireRateUp))
                    {
                        ForwardDirection = CursorManager.CalculateAngle(pointIntersect, Position);
                        prevFireTime = PoseidonGame.playTime;
                        //audio.Shooting.Play();
                        if (HydroBot.bulletType == 0) { AddingObjects.placeBotDamageBullet(this, Content, myBullet); }
                        else if (HydroBot.bulletType == 1) { AddingObjects.placeHealingBullet(this, Content, healthBullet); }
                        //so the bot will not move
                        pointIntersect = Vector3.Zero;
                        reachDestination = true;
                        if (!clipPlayer.inRange(61, 90))
                            clipPlayer.switchRange(61, 90);
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
                //if (cursor.targetToLock != null)
                //{
                //    pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);
                //    hydroBot.ForwardDirection = CursorManager.CalculateAngle(pointIntersect, hydroBot.Position);
                //    if (CursorManager.InShootingRange(hydroBot, cursor, gameCamera, GameConstants.MainGameFloatHeight))
                //    {
                //        if (currentMouseState.LeftButton == ButtonState.Pressed)
                //        {
                //            pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);
                //            hydroBot.ForwardDirection = CursorManager.CalculateAngle(pointIntersect, hydroBot.Position);
                //            if (gameTime.TotalGameTime.TotalSeconds - prevFireTime.TotalSeconds > fireTime.TotalSeconds / (HydroBot.shootingRate * HydroBot.fireRateUp))
                //            {
                //                prevFireTime = gameTime.TotalGameTime;
                //                //audio.Shooting.Play();
                //                if (HydroBot.bulletType == 0) { AddingObjects.placeBotDamageBullet(hydroBot, Content, myBullet); }
                //                else if (HydroBot.bulletType == 1) { AddingObjects.placeHealingBullet(hydroBot, Content, healthBullet); }
                //                if (!hydroBot.clipPlayer.inRange(61, 90))
                //                    hydroBot.clipPlayer.switchRange(61, 90);
                //            }
                //            //hydroBot.reachDestination = true;
                //        }
                //        pointIntersect = Vector3.Zero;
                //        hydroBot.reachDestination = true;
                //    }
                //    else
                //    {
                //        if (!hydroBot.clipPlayer.inRange(1, 30))
                //            hydroBot.clipPlayer.switchRange(1, 30);
                //    }
                //}
            }
            // if the user releases Caps Lock
            // disable locking
            //else if (currentKeyboardState.IsKeyUp(Keys.CapsLock) && lastKeyboardState.IsKeyDown(Keys.CapsLock))
            //{
            //    cursor.targetToLock = null;
            //    hydroBot.reachDestination = true;
            //}

            //let the user change active skill/bullet too when he presses on number
            //this is better for fast action
            InputManager.ChangeSkillBulletWithKeyBoard(lastKeyboardState, currentKeyboardState, gameMode);

            if (HydroBot.supersonicMode == true)
            {
                pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, GameConstants.MainGameFloatHeight);
                CastSkill.KnockOutEnemies(BoundingSphere, Position, MaxRangeX, MaxRangeZ, enemies, ref enemiesAmount, fish, fishAmount, PoseidonGame.audio, GameMode.MainGame);
            }
            if (heightMapInfo != null)
                if (!heightMapInfo.IsOnHeightmap(pointIntersect)) pointIntersect = Vector3.Zero;
            this.Update(currentKeyboardState, enemies, enemiesAmount, fish, fishAmount, gameTime, pointIntersect);

            //planting trees in main game
            if (gameMode != GameMode.ShipWreck && lastKeyboardState.IsKeyDown(Keys.X) && currentKeyboardState.IsKeyUp(Keys.X))
            {
                if (AddingObjects.placePlant(this, heightMapInfo, Content, plants, shipWrecks, staticObjects, gameTime))
                {
                    int envPoint;
                    if (PoseidonGame.gamePlus)
                    {
                        if (PlayGameScene.currentLevel > 0)
                            envPoint = GameConstants.envGainForDropSeed + 5 * HydroBot.gamePlusLevel;
                        else
                            envPoint = GameConstants.envGainForDropSeed - 5;
                    }
                    else
                        envPoint = GameConstants.envGainForDropSeed;
                    PoseidonGame.audio.plantSound.Play();
                    HydroBot.currentExperiencePts += Plant.experienceReward;
                    HydroBot.currentEnvPoint += envPoint;

                    Point point = new Point();
                    String point_string = "+" + envPoint.ToString() + "ENV\n+" + Plant.experienceReward + "EXP";
                    point.LoadContent(PoseidonGame.contentManager, point_string, Position, Color.LawnGreen);
                    if (gameMode == GameMode.ShipWreck)
                        ShipWreckScene.points.Add(point);
                    else if (gameMode == GameMode.MainGame)
                        PlayGameScene.points.Add(point);
                    else if (gameMode == GameMode.SurvivalMode)
                        SurvivalGameScene.points.Add(point);
                }
            }

            //Interacting with trashs and fruits
            if (currentKeyboardState.IsKeyDown(Keys.Z))
            {
                Interact_with_trash_and_fruit(fruits, trashes, gameTime);
            }
        }
        public void Update(KeyboardState keyboardState, SwimmingObject[] enemies,int enemyAmount, SwimmingObject[] fishes, int fishAmount, GameTime gameTime, Vector3 pointMoveTo)
        {
            if (isPoissoned == true) {
                if (accumulatedHealthLossFromPoisson < maxHPLossFromPoisson) {
                    if (!invincibleMode) {
                        currentHitPoint -= 0.1f;
                    }
                    accumulatedHealthLossFromPoisson += 0.1f;
                    
                    ////display HP loss
                    //if (accumulatedHealthLossFromPoisson > 10)
                    //{
                    //    Point point = new Point();
                    //    String point_string = "-10HP";
                    //    point.LoadContent(PlayGameScene.Content, point_string, Position, Color.White);
                    //    if (scene == 2)
                    //        ShipWreckScene.points.Add(point);
                    //    else
                    //        PlayGameScene.points.Add(point);
                    //}
                }
                else {
                    isPoissoned = false;
                    accumulatedHealthLossFromPoisson = 0;

                    Point point = new Point();
                    String point_string = "Poison Free";
                    point.LoadContent(PoseidonGame.contentManager, point_string, Position, Color.White);
                    if (gameMode == GameMode.ShipWreck)
                        ShipWreckScene.points.Add(point);
                    else if (gameMode == GameMode.MainGame)
                        PlayGameScene.points.Add(point);
                    else if (gameMode == GameMode.SurvivalMode)
                        SurvivalGameScene.points.Add(point);
                }
            }

            if (currentExperiencePts >= nextLevelExperience) {
                //increaseBy = (int)(increaseBy * 1.25);
                //nextLevelExperience += increaseBy;
                currentExperiencePts -= nextLevelExperience;
                nextLevelExperience += (int)(nextLevelExperience/5);
                //strength *= 1.15f;
                //maxHitPoint = (int)(maxHitPoint * 1.10f);
                //currentHitPoint = maxHitPoint;
                unassignedPts += 5;
                level++;
                PoseidonGame.audio.levelUpSound.Play();
            }

            Vector3 futurePosition = Position;

            //worn out effect of power-ups
            if (speedUp != 1.0f)
            {
                if (PoseidonGame.playTime.TotalSeconds - speedUpStartTime >= GameConstants.EffectExpired)
                {
                    speedUp = 1.0f;
                }
            }
            if (strengthUp != 1.0f)
            {
                if (PoseidonGame.playTime.TotalSeconds - strengthUpStartTime >= GameConstants.EffectExpired)
                {
                    strengthUp = 1.0f;
                }
            }
            if (fireRateUp != 1.0f)
            {
                if (PoseidonGame.playTime.TotalSeconds - fireRateUpStartTime >= GameConstants.EffectExpired)
                {
                    fireRateUp = 1.0f;
                }
            }
            //worn out effect of certain skills
            //worn out effect for invicible mode
            //and related skill combos
            if (invincibleMode == true)
            {
                float buffFactor = shootingRate * fireRateUp / 1.5f;
                buffFactor = MathHelper.Clamp(buffFactor, 1.0f, 1.6f);
                if (PoseidonGame.playTime.TotalSeconds - skillPrevUsed[2] >= GameConstants.timeArmorLast * buffFactor)
                {
                    invincibleMode = false;
                    if (autoHipnotizeMode == true) autoHipnotizeMode = false;
                }
                
            }
            //worn out effect of supersonic
            if (supersonicMode == true)
            {
                if (PoseidonGame.playTime.TotalMilliseconds - skillPrevUsed[3]*1000 >= GameConstants.timeSuperSonicLast)
                {
                    // To prevent bot landing on an enemy after using the sandal
                    if ( !Collision.isBotVsBarrierCollision(this.BoundingSphere, enemies, enemyAmount))
                    {
                        supersonicMode = false;
                    }
                }
            }
            //float turnAmount = 0;
            //if (keyboardState.IsKeyDown(Keys.A))
            //{
            //    turnAmount = 1;
            //}
            //else if (keyboardState.IsKeyDown(Keys.D))
            //{
            //    turnAmount = -1;
            //}
            //// Player has speed buff from both temporary powerups and his speed attritubte
            
            //ForwardDirection += turnAmount * GameConstants.TurnSpeed * speedUp * speed;
            //ForwardDirection = WrapAngle(ForwardDirection);
            Vector3 movement = Vector3.Zero;

            if (pointMoveTo != Vector3.Zero && Math.Abs(pointMoveTo.X)<MaxRangeX && Math.Abs(pointMoveTo.Z) < MaxRangeZ)
            {
                //desiredAngle = angle;
                this.pointToMoveTo = pointMoveTo;
                desiredAngle = CalculateAngle(this.pointToMoveTo, Position);
                reachDestination = false;
            }
            //if (desiredAngle != 0)

            if (reachDestination == false)// && pointMoveTo != Vector3.Zero)
            {
                //float difference = WrapAngle(desiredAngle - ForwardDirection);
                Vector3 posDif = this.pointToMoveTo - Position;
                float distanceToDest = posDif.Length();
                //// clamp that between -turnSpeed and turnSpeed.
                
                //if (distanceToDest <= 10f * speedUp * this.speed && ForwardDirection != desiredAngle)
                //{
                //    difference = MathHelper.Clamp(difference, -GameConstants.TurnSpeed , GameConstants.TurnSpeed);
                //}
                //else difference = MathHelper.Clamp(difference, -GameConstants.TurnSpeed * speedUp * this.speed / 2, GameConstants.TurnSpeed * speedUp * this.speed / 2);
                //if (Math.Abs(difference) >= 0.0025f)
                // so, the closest we can get to our target is currentAngle + difference.
                // return that, using WrapAngle again.
                //ForwardDirection = WrapAngle(ForwardDirection + difference);
                //if (ForwardDirection == desiredAngle) desiredAngle = 0;
                
                //if (Position == this.pointToMoveTo) reachDestination = true;


                //if (ForwardDirection == desiredAngle) movement.Z = 1 * speedUp * this.speed;
                //else movement.Z = 0.3f;
                ForwardDirection = desiredAngle;
                movement.Z = 1;
                if (distanceToDest <= 1f)
                {
                    movement.Z = 0;
                    reachDestination = true;
                }
                //if (distanceToDest <= 10f && ForwardDirection != desiredAngle)
                //{
                //    movement.Z = 0;
                //}
                
            }
            //ForwardDirection = WrapAngle(angle);
            //ForwardDirection = WrapAngle(ForwardDirection);
            
            Matrix orientationMatrix = Matrix.CreateRotationY(ForwardDirection);

            //if (keyboardState.IsKeyDown(Keys.W))
            //{
            //    movement.Z = 1;
            //}
            //else if (keyboardState.IsKeyDown(Keys.S))
            //{
            //    movement.Z = -1;
            //}
            //if (desiredAngle != 0) movement.Z = 1;
            Vector3 speedl = Vector3.Transform(movement, orientationMatrix);
            speedl *= GameConstants.MainCharVelocity * speedUp * speed;
            if (supersonicMode == true) speedl *= 5;
            futurePosition = Position + speedl;
            if (Collision.isBotValidMove(this, futurePosition, enemies, enemyAmount, fishes, fishAmount))
            {
                Position = futurePosition;

                BoundingSphere updatedSphere;
                updatedSphere = BoundingSphere;

                updatedSphere.Center.X = Position.X;
                updatedSphere.Center.Z = Position.Z;
                BoundingSphere = new BoundingSphere(updatedSphere.Center,
                    updatedSphere.Radius);
                //Trash_Fruit_BoundingSphere = new BoundingSphere(updatedSphere.Center,
                //    20);
            }

            
            //Position = Vector3.Zero;
            // if clip player has been initialized, update it
            if (clipPlayer != null)
            {
                qRotation = Quaternion.CreateFromAxisAngle(
                                Vector3.Up,
                                ForwardDirection);

                charMatrix = Matrix.CreateScale(0.1f) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                                    Matrix.CreateFromQuaternion(qRotation) *
                                    Matrix.CreateTranslation(Position);
                clipPlayer.update(gameTime.ElapsedGameTime, true, charMatrix);
            }
        }

        private void Interact_with_trash_and_fruit(List<Fruit> fruits, List<Trash> trashes, GameTime gameTime)
        {
            Trash_Fruit_BoundingSphere = new BoundingSphere(BoundingSphere.Center,
                    20);
            if (fruits != null)
            {
                for (int curCell = 0; curCell < fruits.Count; curCell++)
                {
                    if (fruits[curCell].Retrieved == false && Trash_Fruit_BoundingSphere.Intersects(
                        fruits[curCell].BoundingSphere))
                    {
                        fruits[curCell].Retrieved = true;
                        if (fruits[curCell].powerType == 1)
                        {
                            speedUpStartTime = PoseidonGame.playTime.TotalSeconds;
                            speedUp = 2.0f;

                            Point point = new Point();
                            String point_string = "TEMP-SPEED X 2";
                            point.LoadContent(PoseidonGame.contentManager, point_string, fruits[curCell].Position, Color.LawnGreen);
                            if (gameMode == GameMode.ShipWreck)
                                ShipWreckScene.points.Add(point);
                            else if (gameMode == GameMode.MainGame)
                                PlayGameScene.points.Add(point);
                            else if (gameMode == GameMode.SurvivalMode)
                                SurvivalGameScene.points.Add(point);
                        }
                        else if (fruits[curCell].powerType == 2)
                        {
                            strengthUpStartTime = PoseidonGame.playTime.TotalSeconds;
                            strengthUp = 2.0f;

                            Point point = new Point();
                            String point_string = "\nTEMP-STRENGTH X 2";
                            point.LoadContent(PoseidonGame.contentManager, point_string, fruits[curCell].Position, Color.LawnGreen);
                            if (gameMode == GameMode.ShipWreck)
                                ShipWreckScene.points.Add(point);
                            else if (gameMode == GameMode.MainGame)
                                PlayGameScene.points.Add(point);
                            else if (gameMode == GameMode.SurvivalMode)
                                SurvivalGameScene.points.Add(point);
                        }
                        else if (fruits[curCell].powerType == 3)
                        {
                            fireRateUpStartTime = PoseidonGame.playTime.TotalSeconds;
                            fireRateUp = 2.0f;

                            Point point = new Point();
                            String point_string = "\n\nTEMP-SHOOTING RATE X 2";
                            point.LoadContent(PoseidonGame.contentManager, point_string, fruits[curCell].Position, Color.LawnGreen);
                            if (gameMode == GameMode.ShipWreck)
                                ShipWreckScene.points.Add(point);
                            else if (gameMode == GameMode.MainGame)
                                PlayGameScene.points.Add(point);
                            else if (gameMode == GameMode.SurvivalMode)
                                SurvivalGameScene.points.Add(point);
                        }
                        else if (fruits[curCell].powerType == 4)
                        {
                            float hitpointAdded = maxHitPoint - currentHitPoint;
                            currentHitPoint += 100;
                            if (currentHitPoint > maxHitPoint)
                            {
                                currentHitPoint = maxHitPoint;
                                Point point = new Point();
                                String point_string = "\n\n\n+"+ (int)hitpointAdded +" HEALTH";
                                point.LoadContent(PoseidonGame.contentManager, point_string, fruits[curCell].Position, Color.LawnGreen);
                                if (gameMode == GameMode.ShipWreck)
                                    ShipWreckScene.points.Add(point);
                                else if (gameMode == GameMode.MainGame)
                                    PlayGameScene.points.Add(point);
                                else if (gameMode == GameMode.SurvivalMode)
                                    SurvivalGameScene.points.Add(point);
                            }
                            else
                            {
                                Point point = new Point();
                                String point_string = "\n\n\n+100 HEALTH";
                                point.LoadContent(PoseidonGame.contentManager, point_string, fruits[curCell].Position, Color.LawnGreen);
                                if (gameMode == GameMode.ShipWreck)
                                    ShipWreckScene.points.Add(point);
                                else if (gameMode == GameMode.MainGame)
                                    PlayGameScene.points.Add(point);
                                else if (gameMode == GameMode.SurvivalMode)
                                    SurvivalGameScene.points.Add(point);
                            }
                        }
                        //RetrievedSound.Play();
                        PoseidonGame.audio.retrieveSound.Play();
                    }
                }
            }
            if (trashes != null)
            {
                foreach (Trash trash in trashes)
                {
                    if (trash.Retrieved == false && Trash_Fruit_BoundingSphere.Intersects(trash.BoundingSphere))
                    {
                        int envPoints, expPoints;
                        if (PoseidonGame.gamePlus)
                        {
                            if (PlayGameScene.currentLevel > 0)
                                envPoints = GameConstants.envGainForTrashClean + HydroBot.gamePlusLevel * 5;
                            else
                                envPoints = GameConstants.envGainForTrashClean - 5;
                        }
                        else
                            envPoints = GameConstants.envGainForTrashClean;
                        expPoints = trash.experienceReward + HydroBot.gamePlusLevel*5;
                        trash.Retrieved = true;
                        currentExperiencePts += expPoints;
                        currentEnvPoint += envPoints;
                        PoseidonGame.audio.retrieveSound.Play();
                        //RetrievedSound.Play();

                        Point point = new Point();
                        String point_string = "+" + envPoints.ToString() + "ENV\n+" + expPoints.ToString() + "EXP";
                        point.LoadContent(PoseidonGame.contentManager, point_string, trash.Position, Color.LawnGreen);
                        if (gameMode == GameMode.ShipWreck)
                            ShipWreckScene.points.Add(point);
                        else if (gameMode == GameMode.MainGame)
                            PlayGameScene.points.Add(point);
                        else if (gameMode == GameMode.SurvivalMode)
                            SurvivalGameScene.points.Add(point);
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Draws the tank model, using the current animation settings.
        /// </summary>
        public void Draw(Matrix view, Matrix projection)
        {
            
            bones = clipPlayer.GetSkinTransforms();

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(bones);
                    effect.View = view;
                    effect.Projection = projection;

                    if (invincibleMode == true)
                    {
                        effect.DiffuseColor = Color.Gold.ToVector3();
                    }
                    else if (isPoissoned == true) {
                        effect.DiffuseColor = Color.Green.ToVector3();
                    }
                    else {
                        effect.DiffuseColor = Vector3.One;
                    }

                    effect.FogEnabled = true;
                    effect.FogStart = GameConstants.FogStart;
                    effect.FogEnd = GameConstants.FogEnd;
                    effect.FogColor = GameConstants.FogColor.ToVector3();
                }
                mesh.Draw();
            }
        }
        internal void DrawTrashFruitSphere(Matrix view, Matrix projection,
            GameObject boundingSphereModel)
        {
            Matrix scaleMatrix = Matrix.CreateScale(20);
            Matrix translateMatrix =
                Matrix.CreateTranslation(Trash_Fruit_BoundingSphere.Center);
            Matrix worldMatrix = scaleMatrix * translateMatrix;

            foreach (ModelMesh mesh in boundingSphereModel.Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = worldMatrix;
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }
        }

        public static float CalculateAngle(Vector3 point2, Vector3 point1)
        {
            return (float)Math.Atan2(point2.X - point1.X, point2.Z - point1.Z);
        }
    }
}
