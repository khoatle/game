#region File Description
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
using Microsoft.Xna.Framework.Media;
#endregion

namespace Poseidon
{
    /// <summary>
    /// Main character of the game: the HydroBot.
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

        public static float controlRadius = 80f;

        // 2 types of bullet
        // 0: killing
        // 1: healing
        public static int bulletType;

        //Skills/Spells of our main character
        //true = enabled/found
        public static bool[] skills, lsSkills;
        //skill combo activated?
        public static bool skillComboActivated, goodWillBarActivated, lsSkillComboActivated, lsGoodWillBarActivated;
        //which skill is being selected
        public static int activeSkillID, lsActiveSkillID, secondSkillID, lsSecondSkillID;
        //time that skills were previously casted
        //for managing skills' cool down time
        public static double[] skillPrevUsed;
        //invincible mode when Achilles' Armor is used
        public static bool invincibleMode;
        //supersonic mode when using Hermes' winged sandal
        public static bool supersonicMode;
        //auto-hipnotize mode when using skill combo: armor - belt
        public static bool autoHipnotizeMode;
        //auto-explode mode when using skill combo: armor - hammer
        public static bool autoExplodeMode;
        //hit-hipnotise mode when using skill combo: sandal - belt
        public static bool sonicHipnotiseMode;
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
        public static bool isBeingHealed = false;
        //for adjusting the intensity of the diffuse light when bot being healed by sea dolphin
        public static float diffuseIntensity = 10.0f;

        // which game mode is this hydrobot in?
        public static GameMode gameMode;

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

        //monitoring time between fire
        private TimeSpan prevFireTime;

        //good will bar related stuff
        public static int faceToDraw = 0, lsFaceToDraw;
        public static bool[] iconActivated, lsIconActivated;
        public static int goodWillPoint, lsGoodWillPoint;
        public static int maxGoodWillPoint;

        //Trash Collection
        public static int bioTrash = 0;
        public static int plasticTrash = 0;
        public static int nuclearTrash = 0;

        //Total trash processed across levels
        public static int totalBioTrashProcessed, lsTotalBioTrashProcessed;
        public static int totalPlasticTrashProcessed, lsTotalPlasticTrashProcessed;
        public static int totalNuclearTrashProcessed, lsTotalNuclearTrashProcessed;

        //Resources for building factories and research facility.
        public static int numResources, lsNumResources;

        //Factory levels
        public static int bioPlantLevel, lsBioPlantLevel;
        public static int plasticPlantLevel, lsPlasticPlantLevel;

        //resurrected sidekicks related stuff
        public static int numStrangeObjCollected, lsNumStrangeObjCollected;
        public static int numDolphinPieces, numSeaCowPieces, numTurtlePieces, lsNumDolphinPieces, lsNumSeaCowPieces, lsNumTurtlePieces;
        public static bool hasDolphin, hasSeaCow, hasTurtle, lsHasDolphin, lsHasSeaCow, lsHasTurtle;
        public static float dolphinPower, seaCowPower, turtlePower, lsDolphinPower, lsSeaCowPower, lsTurtlePower;

        //screen should be in distorted mode or not
        //do not need to be added to save file
        public static bool distortingScreen = false;
        public static double distortionStart = 0;
        public static bool ripplingScreen = false;
        public static Vector2 rippleCenter;

        //new stuff related to power of skills
        //they are upgraded thru the good will bar
        //need to be added to save file
        public static float bowPower, hammerPower, armorPower, sandalPower, beltPower,
            lsBowPower, lsHammerPower, lsArmorPower, lsSandalPower, lsBeltPower;

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

            //stuff related to good will bar and skill combo
            iconActivated = new bool[GameConstants.NumGoodWillBarIcons];
            lsIconActivated = new bool[GameConstants.NumGoodWillBarIcons];
            for (int index = 0; index < GameConstants.NumGoodWillBarIcons; index++)
            {
                //some basic icons are activated right from the beginning
                if (index == IngamePresentation.poseidonFace || index == IngamePresentation.strengthIcon || index == IngamePresentation.speedIcon
                    || index == IngamePresentation.shootRateIcon || index == IngamePresentation.healthIcon)
                    iconActivated[index] = true;
                else iconActivated[index] = false;
            }
            skillComboActivated = false;
            goodWillBarActivated = false;
            maxGoodWillPoint = GameConstants.MaxGoodWillPoint;
            goodWillPoint = lsGoodWillPoint = 0;

            //stuff related to resurrected sidekicks

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
            autoExplodeMode = false;
            sonicHipnotiseMode = false;

            isPoissoned = false;
            poissonInterval = 0;
            maxHPLossFromPoisson = 50;
            accumulatedHealthLossFromPoisson = 0;

            //resurrected sidekicks stuff
            numStrangeObjCollected = lsNumStrangeObjCollected = 0;
            hasDolphin = hasSeaCow = hasTurtle = lsHasDolphin = lsHasSeaCow = lsHasTurtle = false;
            dolphinPower = seaCowPower = turtlePower = lsDolphinPower = lsSeaCowPower = lsTurtlePower = 0;
            numDolphinPieces = lsNumDolphinPieces = numSeaCowPieces = lsNumSeaCowPieces = numTurtlePieces = lsNumTurtlePieces = 0;

            //power of the skills
            bowPower = hammerPower = armorPower = sandalPower = beltPower = 
                lsBowPower = lsHammerPower = lsArmorPower = lsSandalPower = lsBeltPower = 1.0f;

            bioTrash = plasticTrash = nuclearTrash = 0;
            totalBioTrashProcessed = totalPlasticTrashProcessed = totalNuclearTrashProcessed = 0;
            bioPlantLevel = plasticPlantLevel = lsBioPlantLevel = lsPlasticPlantLevel = 1;
            numResources  = lsNumResources = GameConstants.numResourcesAtStart;

            HydroBot.gameMode = gameMode;
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
            {
                activeSkillID = lsActiveSkillID = -1;
                secondSkillID = lsSecondSkillID = -1;
            }
            else
            {
                activeSkillID = lsActiveSkillID = (int)info.GetValue("activeSkillID", typeof(int));
                secondSkillID = lsSecondSkillID = (int)info.GetValue("secondSkillID", typeof(int));
            }
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
            autoExplodeMode = false;
            sonicHipnotiseMode = false;

            isPoissoned = false;
            poissonInterval = 0;
            maxHPLossFromPoisson = 50;
            accumulatedHealthLossFromPoisson = 0;

            gamePlusLevel = (int)info.GetValue("gamePlusLevel", typeof(int));

            goodWillBarActivated = lsGoodWillBarActivated = (bool)info.GetValue("goodWillBarActivated", typeof(bool));
            goodWillPoint = lsGoodWillPoint = (int)info.GetValue("goodWillPoint", typeof(int));
            faceToDraw = lsFaceToDraw = (int)info.GetValue("faceToDraw", typeof(int));
            for (int j = 0; j < GameConstants.NumGoodWillBarIcons; j++)
            {
                string iconName = "iconActivated"+j.ToString();
                iconActivated[j] = lsIconActivated[j] = (bool)info.GetValue(iconName, typeof(bool));
            }
            
            skillComboActivated = lsSkillComboActivated = (bool)info.GetValue("skillComboActivated", typeof(bool));

            bioTrash = plasticTrash = nuclearTrash = 0;
            totalBioTrashProcessed = lsTotalBioTrashProcessed = (int)info.GetValue("totalBioTrashProcessed", typeof(int));
            totalPlasticTrashProcessed = lsTotalPlasticTrashProcessed = (int)info.GetValue("totalPlasticTrashProcessed", typeof(int));
            totalNuclearTrashProcessed = lsTotalNuclearTrashProcessed = (int)info.GetValue("totalNuclearTrashProcessed", typeof(int));
            bioPlantLevel = lsBioPlantLevel = (int)info.GetValue("bioPlantLevel", typeof(int));
            plasticPlantLevel = lsPlasticPlantLevel = (int)info.GetValue("plasticPlantLevel", typeof(int));
            numResources = lsNumResources = (int)info.GetValue("numResources", typeof(int));
            numResources += GameConstants.numResourcesAtStart;

            //resurrected sidekicks stuff
            numStrangeObjCollected = lsNumStrangeObjCollected = (int)info.GetValue("numStrangeObjCollected", typeof(int));
            hasDolphin = lsHasDolphin = (bool)info.GetValue("hasDolphin", typeof(bool));
            hasSeaCow = lsHasSeaCow = (bool)info.GetValue("hasSeaCow", typeof(bool));
            hasTurtle = lsHasTurtle = (bool)info.GetValue("hasTurtle", typeof(bool));
            numDolphinPieces = lsNumDolphinPieces = (int)info.GetValue("numDolphinPieces", typeof(int));
            numSeaCowPieces = lsNumSeaCowPieces = (int)info.GetValue("numSeaCowPieces", typeof(int));
            numTurtlePieces = lsNumTurtlePieces = (int)info.GetValue("numTurtlePieces", typeof(int));
            dolphinPower = lsDolphinPower = (float)info.GetValue("dolphinPower", typeof(float));
            seaCowPower = lsSeaCowPower = (float)info.GetValue("seaCowPower", typeof(float));
            turtlePower = lsTurtlePower = (float)info.GetValue("turtlePower", typeof(float));

            bowPower = lsBowPower = (float)info.GetValue("bowPower", typeof(float));
            hammerPower = lsHammerPower = (float)info.GetValue("hammerPower", typeof(float));
            sandalPower = lsSandalPower = (float)info.GetValue("sandalPower", typeof(float));
            armorPower = lsArmorPower = (float)info.GetValue("armorPower", typeof(float));
            beltPower = lsBeltPower = (float)info.GetValue("beltPower", typeof(float));
            
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
            info.AddValue("secondSkillID", secondSkillID);

            info.AddValue("floatHeight", floatHeight);
            info.AddValue("MaxRangeX", MaxRangeX);
            info.AddValue("MaxRangeZ", MaxRangeZ);

            info.AddValue("currentExperiencePts", currentExperiencePts);
            info.AddValue("nextLevelExperience", nextLevelExperience);
            info.AddValue("level", level);
            info.AddValue("unassignedPts", unassignedPts);
            info.AddValue("gamePlusLevel", gamePlusLevel);
            info.AddValue("goodWillBarActivated", goodWillBarActivated);
            info.AddValue("goodWillPoint", goodWillPoint);
            info.AddValue("faceToDraw", faceToDraw);
            for( int j=0; j<GameConstants.NumGoodWillBarIcons; j++)
            {
                string iconName = "iconActivated"+ j.ToString();
                info.AddValue(iconName, iconActivated[j]);
            }
            info.AddValue("skillComboActivated", skillComboActivated);
            info.AddValue("bioPlantLevel", bioPlantLevel);
            info.AddValue("plasticPlantLevel", plasticPlantLevel);
            info.AddValue("numResources", numResources);
            info.AddValue("totalBioTrashProcessed", totalBioTrashProcessed);
            info.AddValue("totalPlasticTrashProcessed", totalPlasticTrashProcessed);
            info.AddValue("totalNuclearTrashProcessed", totalNuclearTrashProcessed);

            //resurrected sidekicks stuff
            info.AddValue("numStrangeObjCollected", numStrangeObjCollected);
            info.AddValue("hasDolphin", hasDolphin);
            info.AddValue("hasSeaCow", hasSeaCow);
            info.AddValue("hasTurtle", hasTurtle);
            info.AddValue("numDolphinPieces", numDolphinPieces);
            info.AddValue("numSeaCowPieces", numSeaCowPieces);
            info.AddValue("numTurtlePieces", numTurtlePieces);
            info.AddValue("dolphinPower", dolphinPower);
            info.AddValue("seaCowPower", seaCowPower);
            info.AddValue("turtlePower", turtlePower);

            info.AddValue("bowPower", bowPower);
            info.AddValue("hammerPower", hammerPower);
            info.AddValue("sandalPower", sandalPower);
            info.AddValue("armorPower", armorPower);
            info.AddValue("beltPower", beltPower);
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
            autoExplodeMode = false;
            sonicHipnotiseMode = false;

            //just for testing
            //should be removed
            //skillComboActivated = true;
            //activeSkillID = 4;
            //secondSkillID = -1;
            //skills[0] = true;
            //skills[1] = true;
            //skills[2] = true;
            //skills[3] = true;
            //skills[4] = true;

            //for testing survival mode
            //currentHitPoint = maxHitPoint = 300;
            //strength = 2;
            //speed = 1;

            //goodWillBarActivated = true;
            //for (int index = 0; index < GameConstants.NumGoodWillBarIcons; index++)
            //{
            //    iconActivated[index] = true;
            //}

            //hasTurtle = true;
            //turtlePower = 1.0f;
            //hasDolphin = true;
            //dolphinPower = 1.0f;
            //hasSeaCow = true;
            //seaCowPower = 1.0f;

            firstPlant = true;
            prevPlantTime = 0;

            LoadAnimation(1, 30, 24);

            

            // Set up the parameters
            SetupShaderParameters(content, Model);
            //EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);
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

            //good will bar and skill combo
            secondSkillID = lsSecondSkillID;
            faceToDraw = lsFaceToDraw;
            lsIconActivated.CopyTo(iconActivated, 0);
            skillComboActivated = lsSkillComboActivated;
            goodWillBarActivated = lsGoodWillBarActivated;
            goodWillPoint = lsGoodWillPoint;

            //resurrected sidekicks
            numStrangeObjCollected = lsNumStrangeObjCollected;
            hasDolphin = lsHasDolphin;
            hasSeaCow = lsHasSeaCow;
            hasTurtle = lsHasTurtle;
            dolphinPower = lsDolphinPower;
            seaCowPower = lsSeaCowPower;
            turtlePower = lsTurtlePower;
            numDolphinPieces = lsNumDolphinPieces;
            numSeaCowPieces = lsNumSeaCowPieces;
            numTurtlePieces = lsNumTurtlePieces;

            //power of skills
            bowPower = lsBowPower;
            hammerPower = lsHammerPower;
            armorPower = lsArmorPower;
            sandalPower = lsSandalPower;
            beltPower = lsBeltPower;


            currentExperiencePts = lsCurrentExperiencePts;
            nextLevelExperience = lsNextLevelExperience;
            //increaseBy = lsIncreaseBy;
            level = lsLevel;
            unassignedPts = lsUnassignedPts;
            //stop good will bar from spinning
            IngamePresentation.StopSpinning();

            //factory levels
            bioPlantLevel = lsBioPlantLevel;
            plasticPlantLevel = lsPlasticPlantLevel;
            numResources = lsNumResources;

            totalBioTrashProcessed = lsTotalBioTrashProcessed;
            totalPlasticTrashProcessed = lsTotalPlasticTrashProcessed;
            totalNuclearTrashProcessed = lsTotalNuclearTrashProcessed;
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

            //good will bar and skill combo
            lsSecondSkillID = secondSkillID;
            lsSkillComboActivated = skillComboActivated;
            lsGoodWillBarActivated = goodWillBarActivated;
            iconActivated.CopyTo(lsIconActivated, 0);
            lsFaceToDraw = faceToDraw;
            lsGoodWillPoint = goodWillPoint;

            //resurrected sidekicks
            lsNumStrangeObjCollected = numStrangeObjCollected;
            lsHasSeaCow = hasSeaCow;
            lsHasDolphin = hasDolphin;
            lsHasTurtle = hasTurtle;
            lsSeaCowPower = seaCowPower;
            lsDolphinPower = dolphinPower;
            lsTurtlePower = turtlePower;
            lsNumDolphinPieces = numDolphinPieces;
            lsNumSeaCowPieces = numSeaCowPieces;
            lsNumTurtlePieces = numTurtlePieces;

            //skill powers
            lsBowPower = bowPower;
            lsHammerPower = hammerPower;
            lsArmorPower = armorPower;
            lsSandalPower = sandalPower;
            lsBeltPower = beltPower;

            lsCurrentExperiencePts = currentExperiencePts;
            lsNextLevelExperience = nextLevelExperience;
            //lsIncreaseBy = increaseBy;
            lsLevel = level;
            lsUnassignedPts = unassignedPts;
            lsCurrentEnvPoint = currentEnvPoint;

            //factories
            lsBioPlantLevel = bioPlantLevel;
            lsPlasticPlantLevel = plasticPlantLevel;

            numResources += GameConstants.numResourcesAtStart;
            lsNumResources = numResources;

            lsTotalBioTrashProcessed = totalBioTrashProcessed;
            lsTotalPlasticTrashProcessed = totalPlasticTrashProcessed;
            lsTotalNuclearTrashProcessed = totalNuclearTrashProcessed;
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
            autoExplodeMode = false;
            sonicHipnotiseMode = false;
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
            bioTrash = plasticTrash = nuclearTrash = 0;
        }

        public void UpdateAction(GameTime gameTime, Cursor cursor, Camera gameCamera, BaseEnemy[] enemies, int enemiesAmount, Fish[] fish, int fishAmount, ContentManager Content,
            SpriteBatch spriteBatch, List<DamageBullet> myBullet, GameScene gameScene, HeightMapInfo heightMapInfo, List<HealthBullet> healthBullet, List<Powerpack> powerpacks, List<Resource> resources,
            List<Trash> trashes, List<ShipWreck> shipWrecks, List<StaticObject> staticObjects, bool mouseOnInteractiveIcons)
        {
            lastKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            CursorManager.CheckClick(ref lastMouseState, ref currentMouseState, gameTime, ref clickTimer, ref clicked, ref doubleClicked);
            Vector3 pointIntersect = Vector3.Zero;
            int floatHeight;
            if (gameMode == GameMode.ShipWreck) floatHeight = GameConstants.ShipWreckFloatHeight;
            else floatHeight = GameConstants.MainGameFloatHeight;
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
                    pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, floatHeight);
                }
            }
            //if the user click on right mouse button
            //cast the current selected skill
            //else if (lastMouseState.RightButton == ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Released)
            else if (lastMouseState.RightButton == ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Released && !mouseOnInteractiveIcons)
            {
                //ForwardDirection = CursorManager.CalculateAngle(pointIntersect, Position);
                CastSkill.UseSkill(mouseOnLivingObject, pointIntersect, cursor, gameCamera, gameMode, this, gameScene, Content, spriteBatch, gameTime, myBullet, enemies, ref enemiesAmount, fish, ref fishAmount);
            }

            //if the user holds down Ctrl button
            //just shoot at wherever the mouse is pointing w/o moving
            else if (currentKeyboardState.IsKeyDown(Keys.RightControl) || currentKeyboardState.IsKeyDown(Keys.LeftControl))
            {
                if (currentMouseState.LeftButton == ButtonState.Pressed)
                {
                    pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, floatHeight);
                    ForwardDirection = CursorManager.CalculateAngle(pointIntersect, Position);
                    if (PoseidonGame.playTime.TotalSeconds - prevFireTime.TotalSeconds > GameConstants.MainCharBasicTimeBetweenFire.TotalSeconds / (HydroBot.shootingRate * HydroBot.fireRateUp))
                    {
                        prevFireTime = PoseidonGame.playTime;
                        //audio.Shooting.Play();
                        if (HydroBot.bulletType == 0) { AddingObjects.placeBotDamageBullet(this, Content, myBullet, gameMode); }
                        else if (HydroBot.bulletType == 1) { AddingObjects.placeHealingBullet(this, Content, healthBullet, gameMode); }
                        if (!clipPlayer.inRange(61, 90))
                            clipPlayer.switchRange(61, 90);
                    }
                    //hydroBot.reachDestination = true;
                }
                pointIntersect = Vector3.Zero;
                reachDestination = true;
            }
            //if the user clicks or holds mouse's left button
            else if (currentMouseState.LeftButton == ButtonState.Pressed && !mouseOnLivingObject && !mouseOnInteractiveIcons)
            {
                pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, floatHeight);
                if (!clipPlayer.inRange(1, 30))
                    clipPlayer.switchRange(1, 30);
            }

            else if (lastMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released && !mouseOnInteractiveIcons)
            {
                pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, floatHeight);
                //if it is out of shooting range then just move there
                if (!CursorManager.InShootingRange(this, cursor, gameCamera, floatHeight))
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
                        if (HydroBot.bulletType == 0) { AddingObjects.placeBotDamageBullet(this, Content, myBullet, gameMode); }
                        else if (HydroBot.bulletType == 1) { AddingObjects.placeHealingBullet(this, Content, healthBullet, gameMode); }
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
                pointIntersect = CursorManager.IntersectPointWithPlane(cursor, gameCamera, floatHeight);
                CastSkill.KnockOutEnemies(BoundingSphere, Position, MaxRangeX, MaxRangeZ, enemies, ref enemiesAmount, fish, fishAmount, PoseidonGame.audio, gameMode);
            }
            if (heightMapInfo != null)
                if (!heightMapInfo.IsOnHeightmap(pointIntersect)) pointIntersect = Vector3.Zero;
            this.Update(currentKeyboardState, enemies, enemiesAmount, fish, fishAmount, gameTime, pointIntersect, heightMapInfo);

            //Interacting with trash
            if (currentKeyboardState.IsKeyUp(Keys.Z) && lastKeyboardState.IsKeyDown(Keys.Z))
            {
                //Collect powerpacks and resources
                Collect_Powerpacks_and_Resources(powerpacks, resources, gameTime);

                if (bioTrash >= GameConstants.maxTrashCarryingCapacity)
                {
                    Point point = new Point();
                    point.LoadContent(PoseidonGame.contentManager, "Organic trash\ncontainer nis full", Position, Color.LawnGreen);
                    if (gameMode == GameMode.ShipWreck)
                        ShipWreckScene.points.Add(point);
                    else if (gameMode == GameMode.MainGame)
                        PlayGameScene.points.Add(point);
                    else if (gameMode == GameMode.SurvivalMode)
                        SurvivalGameScene.points.Add(point);
                }
                else
                {
                    Trash_Fruit_BoundingSphere = new BoundingSphere(BoundingSphere.Center, 20);
                    if (trashes != null)
                    {
                        //foreach (Trash trash in trashes)
                        for(int i=0; i<trashes.Count; i++)
                        {
                            Trash trash = trashes[i];
                            if (Trash_Fruit_BoundingSphere.Intersects(trash.BoundingSphere))
                            {
                                string display_str;
                                if (trash.trashType == TrashType.biodegradable)
                                {
                                    bioTrash++;
                                    display_str = "Organic trash\ncollected " + bioTrash;

                                    if (gameMode == GameMode.MainGame) PlayGameScene.numTrashCollected += 1;

                                    IncreaseGoodWillPoint(GameConstants.GoodWillPointGainForCleaning);

                                    //Increase Environment and experience points
                                    int envPoints = GameConstants.envGainForBioTrashClean;
                                    int expPoints;
                                    if (PoseidonGame.gamePlus)
                                    {
                                        if (PlayGameScene.currentLevel > 0)
                                            envPoints = (envPoints + HydroBot.gamePlusLevel * 5);
                                        else
                                            envPoints = envPoints - 5;
                                    }
                                    expPoints = GameConstants.expGainForTrash + (HydroBot.gamePlusLevel * 5);
                                    HydroBot.currentExperiencePts += expPoints;
                                    HydroBot.currentEnvPoint += envPoints;
                                    display_str += "\n+" + envPoints.ToString() + "ENV\n+" + expPoints.ToString() + "EXP";
                                }
                                else if (trash.trashType == TrashType.plastic)
                                {
                                    display_str = "Wrong type:\nPlastic";
                                }
                                else //radioactive
                                {
                                    display_str = "Wrong type:\nRadioactive";
                                }
                                PoseidonGame.audio.retrieveSound.Play();

                                Point point = new Point();
                                point.LoadContent(PoseidonGame.contentManager, display_str, trash.Position, Color.LawnGreen);
                                if (gameMode == GameMode.ShipWreck)
                                    ShipWreckScene.points.Add(point);
                                else if (gameMode == GameMode.MainGame)
                                    PlayGameScene.points.Add(point);
                                else if (gameMode == GameMode.SurvivalMode)
                                    SurvivalGameScene.points.Add(point);
                                trashes.Remove(trash);
                                i -= 1;
                            }
                        }
                    }
                }
            }
            if (lastKeyboardState.IsKeyDown(Keys.X) && currentKeyboardState.IsKeyUp(Keys.X)) // Collect Plastic Trash
            {
                if (plasticTrash >= GameConstants.maxTrashCarryingCapacity)
                {
                    Point point = new Point();
                    point.LoadContent(PoseidonGame.contentManager, "Plastic trash\ncontainer is full", Position, Color.LawnGreen);
                    if (gameMode == GameMode.ShipWreck)
                        ShipWreckScene.points.Add(point);
                    else if (gameMode == GameMode.MainGame)
                        PlayGameScene.points.Add(point);
                    else if (gameMode == GameMode.SurvivalMode)
                        SurvivalGameScene.points.Add(point);
                }
                else
                {
                    Trash_Fruit_BoundingSphere = new BoundingSphere(BoundingSphere.Center, 20);
                    if (trashes != null)
                    {
                        //foreach (Trash trash in trashes)
                        for(int i=0; i<trashes.Count; i++)
                        {
                            Trash trash = trashes[i];
                            if (Trash_Fruit_BoundingSphere.Intersects(trash.BoundingSphere))
                            {
                                string display_str;
                                if (trash.trashType == TrashType.biodegradable)
                                {
                                    display_str = "Wrong type:\nOrganic";
                                }
                                else if (trash.trashType == TrashType.plastic)
                                {
                                    plasticTrash++;
                                    display_str = "Plastic trash\ncollected " + plasticTrash;

                                    if (gameMode == GameMode.MainGame) PlayGameScene.numTrashCollected += 1;

                                    //update good will point
                                    IncreaseGoodWillPoint(GameConstants.GoodWillPointGainForCleaning);

                                    //Increase Environment and experience points
                                    int envPoints = GameConstants.envGainForPlasticTrashClean;
                                    int expPoints;
                                    if (PoseidonGame.gamePlus)
                                    {
                                        if (PlayGameScene.currentLevel > 0)
                                            envPoints = (envPoints + HydroBot.gamePlusLevel * 5);
                                        else
                                            envPoints = envPoints - 5;
                                    }
                                    expPoints = GameConstants.expGainForTrash + (HydroBot.gamePlusLevel * 5);
                                    HydroBot.currentExperiencePts += expPoints;
                                    HydroBot.currentEnvPoint += envPoints;
                                    display_str += "\n+" + envPoints.ToString() + "ENV\n+" + expPoints.ToString() + "EXP";
                                }
                                else //radioactive
                                {
                                    display_str = "Wrong type:\nRadioactive";
                                }
                                PoseidonGame.audio.retrieveSound.Play();
                                Point point = new Point();
                                point.LoadContent(PoseidonGame.contentManager, display_str, trash.Position, Color.LawnGreen);
                                if (gameMode == GameMode.ShipWreck)
                                    ShipWreckScene.points.Add(point);
                                else if (gameMode == GameMode.MainGame)
                                    PlayGameScene.points.Add(point);
                                else if (gameMode == GameMode.SurvivalMode)
                                    SurvivalGameScene.points.Add(point);
                                trashes.Remove(trash);
                                i -= 1;
                            }
                        }
                    }
                }
            }
            if (lastKeyboardState.IsKeyDown(Keys.C) && currentKeyboardState.IsKeyUp(Keys.C)) // Collect Nuclear Trash
            {
                if (nuclearTrash >= GameConstants.maxTrashCarryingCapacity)
                {
                    Point point = new Point();
                    point.LoadContent(PoseidonGame.contentManager, "Nuclear trash\ncontainer is full", Position, Color.LawnGreen);
                    if (gameMode == GameMode.ShipWreck)
                        ShipWreckScene.points.Add(point);
                    else if (gameMode == GameMode.MainGame)
                        PlayGameScene.points.Add(point);
                    else if (gameMode == GameMode.SurvivalMode)
                        SurvivalGameScene.points.Add(point);
                }
                else
                {
                    Trash_Fruit_BoundingSphere = new BoundingSphere(BoundingSphere.Center, 20);
                    if (trashes != null)
                    {
                        //foreach (Trash trash in trashes)
                        for (int i = 0; i < trashes.Count; i++)
                        {
                            Trash trash = trashes[i];
                            if (Trash_Fruit_BoundingSphere.Intersects(trash.BoundingSphere))
                            {
                                string display_str;
                                if (trash.trashType == TrashType.biodegradable)
                                {
                                    display_str = "Wrong type:\nOrganic";
                                }
                                else if (trash.trashType == TrashType.plastic)
                                {
                                    display_str = "Wrong type:\nPlastic";
                                }
                                else //radioactive
                                {
                                    nuclearTrash++;
                                    display_str = "Radioactive trash\ncollected " + nuclearTrash;

                                    if (gameMode == GameMode.MainGame) PlayGameScene.numTrashCollected += 1;

                                    //update good will point
                                    IncreaseGoodWillPoint(GameConstants.GoodWillPointGainForCleaning);

                                    //Increase Environment and experience points
                                    int envPoints = GameConstants.envGainForNuclearTrashClean;
                                    int expPoints;
                                    if (PoseidonGame.gamePlus)
                                    {
                                        if (PlayGameScene.currentLevel > 0)
                                            envPoints = (envPoints + HydroBot.gamePlusLevel * 5);
                                        else
                                            envPoints = envPoints - 5;
                                    }
                                    expPoints = GameConstants.expGainForTrash + (HydroBot.gamePlusLevel * 5);
                                    HydroBot.currentExperiencePts += expPoints;
                                    HydroBot.currentEnvPoint += envPoints;
                                    display_str += "\n+" + envPoints.ToString() + "ENV\n+" + expPoints.ToString() + "EXP";
                                }
                                PoseidonGame.audio.retrieveSound.Play();
                                Point point = new Point();
                                point.LoadContent(PoseidonGame.contentManager, display_str, trash.Position, Color.LawnGreen);
                                if (gameMode == GameMode.ShipWreck)
                                    ShipWreckScene.points.Add(point);
                                else if (gameMode == GameMode.MainGame)
                                    PlayGameScene.points.Add(point);
                                else if (gameMode == GameMode.SurvivalMode)
                                    SurvivalGameScene.points.Add(point);
                                trashes.Remove(trash);
                                i -= 1;
                            }
                        }
                    }
                }
            }

            //if (lastKeyboardState.IsKeyDown(Keys.R) && currentKeyboardState.IsKeyUp(Keys.R))
            //{
            //    IngamePresentation.SpinNow();
            //}
            //if (lastKeyboardState.IsKeyDown(Keys.T) && currentKeyboardState.IsKeyUp(Keys.T))
            //{
            //    IngamePresentation.StopSpinning();
            //}

            //update the good will bar
            if (HydroBot.goodWillPoint >= HydroBot.maxGoodWillPoint)
            {
                IngamePresentation.SpinNow();
                HydroBot.goodWillPoint = 0;
            }
            IngamePresentation.UpdateGoodWillBar();
            
        }
        public void Update(KeyboardState keyboardState, SwimmingObject[] enemies,int enemyAmount, SwimmingObject[] fishes, int fishAmount, GameTime gameTime, Vector3 pointMoveTo, HeightMapInfo heightMapInfo)
        {
            EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);

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
                    String point_string = "Corrosion free";
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
            if (invincibleMode == true && autoHipnotizeMode == false && autoExplodeMode == false)
            {
                float buffFactor = shootingRate * fireRateUp / 1.5f;
                buffFactor = MathHelper.Clamp(buffFactor, 1.0f, 1.6f);
                if (PoseidonGame.playTime.TotalSeconds - skillPrevUsed[2] >= GameConstants.timeArmorLast * buffFactor * HydroBot.armorPower)
                {
                    invincibleMode = false;
                    if (autoHipnotizeMode == true) autoHipnotizeMode = false;
                }
                
            }
            if (autoHipnotizeMode == true)
            {
                float buffFactor = shootingRate * fireRateUp / 1.5f;
                buffFactor = MathHelper.Clamp(buffFactor, 1.0f, 1.6f);
                if (PoseidonGame.playTime.TotalSeconds - skillPrevUsed[2] >= GameConstants.timeArmorLast * buffFactor * HydroBot.armorPower * HydroBot.beltPower)
                {
                    autoHipnotizeMode = false;
                    invincibleMode = false;
                }
            }
            if (autoExplodeMode == true)
            {
                float buffFactor = shootingRate * fireRateUp / 1.5f;
                buffFactor = MathHelper.Clamp(buffFactor, 1.0f, 1.6f);
                if (PoseidonGame.playTime.TotalSeconds - skillPrevUsed[2] >= GameConstants.timeArmorLast * buffFactor * HydroBot.armorPower * HydroBot.hammerPower)
                {
                    autoExplodeMode = false;
                    invincibleMode = false;
                }
            }
            //worn out effect of supersonic
            if (supersonicMode == true && !sonicHipnotiseMode)
            {
                if (PoseidonGame.playTime.TotalMilliseconds - skillPrevUsed[3]*1000 >= GameConstants.timeSuperSonicLast * speed * speedUp * HydroBot.sandalPower/ GameConstants.BasicStartSpeed)
                {
                    // To prevent bot landing on an enemy after using the sandal
                    if ( !Collision.isBotVsBarrierCollision(this.BoundingSphere, enemies, enemyAmount))
                    {
                        supersonicMode = false;
                    }
                }
            }
            if (sonicHipnotiseMode == true)
            {
                if (PoseidonGame.playTime.TotalMilliseconds - skillPrevUsed[3] * 1000 >= GameConstants.timeSuperSonicLast * speed * speedUp * HydroBot.sandalPower * HydroBot.beltPower/ GameConstants.BasicStartSpeed)
                {
                    // To prevent bot landing on an enemy after using the sandal
                    if (!Collision.isBotVsBarrierCollision(this.BoundingSphere, enemies, enemyAmount))
                    {
                        sonicHipnotiseMode = false;
                        supersonicMode = false;
                    }
                }
            }
            if (distortingScreen == true)
            {
                if (PoseidonGame.playTime.TotalMilliseconds - distortionStart * 1000 >= GameConstants.distortionDuration)
                {
                    distortingScreen = false;
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
            if (Collision.isBotValidMove(this, futurePosition, enemies, enemyAmount, fishes, fishAmount, heightMapInfo))
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

        private void Collect_Powerpacks_and_Resources(List<Powerpack> powerpacks, List<Resource> resources, GameTime gameTime)
        {
            string point_string = "";
            bool incrSpeed, incrStrength, incrShootRate;
            int numHealth, numResourceCollected, numRocks;
            incrShootRate = incrSpeed = incrStrength = false;
            numHealth = numResourceCollected = numRocks = 0;

            if (powerpacks != null)
            {
                Trash_Fruit_BoundingSphere = new BoundingSphere(BoundingSphere.Center, 5);
                for (int curCell = 0; curCell < powerpacks.Count; curCell++)
                {
                    if (Trash_Fruit_BoundingSphere.Intersects(
                        powerpacks[curCell].BoundingSphere))
                    {
                        if (powerpacks[curCell].powerType == PowerPackType.Speed)
                        {
                            speedUpStartTime = PoseidonGame.playTime.TotalSeconds;
                            speedUp = 2.0f;
                            incrSpeed = true;
                        }
                        else if (powerpacks[curCell].powerType == PowerPackType.Strength)
                        {
                            strengthUpStartTime = PoseidonGame.playTime.TotalSeconds;
                            strengthUp = 2.0f;
                            incrStrength = true;
                        }
                        else if (powerpacks[curCell].powerType == PowerPackType.FireRate)
                        {
                            fireRateUpStartTime = PoseidonGame.playTime.TotalSeconds;
                            fireRateUp = 2.0f;
                            incrShootRate = true;
                        }
                        else if (powerpacks[curCell].powerType == PowerPackType.Health)
                        {
                            float hitpointAdded = maxHitPoint - currentHitPoint;
                            currentHitPoint += 100;
                            if (currentHitPoint > maxHitPoint)
                            {
                                currentHitPoint = maxHitPoint;
                                numHealth += (int)hitpointAdded;
                            }
                            else
                            {
                                numHealth += 100;
                            }
                        }
                        else if (powerpacks[curCell].powerType == PowerPackType.StrangeRock)
                        {
                            numStrangeObjCollected++;
                            numRocks += 1;
                        }
                        //RetrievedSound.Play();
                        PoseidonGame.audio.retrieveSound.Play();
                        powerpacks.RemoveAt(curCell);
                        curCell -= 1;
                    }
                }
            }
            if (resources != null)
            {
                Trash_Fruit_BoundingSphere = new BoundingSphere(BoundingSphere.Center,
                    25);
                //foreach (Resource resource in resources)
                for (int curCell = 0; curCell < resources.Count; curCell++)
                {
                    if (Trash_Fruit_BoundingSphere.Intersects(resources[curCell].BoundingSphere))
                    {
                        numResources++;
                        numResourceCollected++;
                        PoseidonGame.audio.retrieveSound.Play();
                        resources.RemoveAt(curCell);
                        curCell -= 1;
                    }       
                }
            }

            if (numHealth > 0)
                point_string += numHealth + " health\n";
            if (incrSpeed)
                point_string += "Speed X 2\n";
            if (incrShootRate)
                point_string += "Fire rate X 2\n";
            if (incrStrength)
                point_string += "Strength X 2\n";
            if (numResourceCollected > 0)
                point_string += numResourceCollected + " resource\n";
            if (numRocks > 0)
                point_string += numRocks + " strange rocks\n";
            if (numResourceCollected > 0 || numRocks > 0)
                point_string += "collected";

            if (point_string != "")
            {
                Point point = new Point();
                point.LoadContent(PoseidonGame.contentManager, point_string, Position, Color.LawnGreen);
                if (gameMode == GameMode.ShipWreck)
                    ShipWreckScene.points.Add(point);
                else if (gameMode == GameMode.MainGame)
                    PlayGameScene.points.Add(point);
                else if (gameMode == GameMode.SurvivalMode)
                    SurvivalGameScene.points.Add(point);
            }
            return;
        }


        // our custom shader
        Effect newSkinnedeffect;
  
        public void SetupShaderParameters(ContentManager content, Model model)
        {
            newSkinnedeffect = content.Load<Effect>("Shaders/NewSkinnedEffect");
            EffectHelpers.ChangeEffectUsedByModelToCustomSkinnedEffect(model, newSkinnedeffect);
        }

        /// <summary>
        /// Draws the tank model, using the current animation settings.
        /// </summary>
        public void Draw(Matrix view, Matrix projection, Camera gameCamera, string techniqueName)
        {

            bones = clipPlayer.GetSkinTransforms();

            foreach (ModelMesh mesh in Model.Meshes)
            {
                //foreach (SkinnedEffect effect in mesh.Effects)
                foreach (Effect effect in mesh.Effects)
                {
                    //for standard SkinnedEffect
                    //effect.SetBoneTransforms(bones);
                    //effect.View = view;
                    //effect.Projection = projection;

                    //if (invincibleMode == true)
                    //{
                    //    effect.DiffuseColor = Color.Gold.ToVector3();
                    //}
                    //else if (isPoissoned == true)
                    //{
                    //    effect.DiffuseColor = Color.Green.ToVector3();
                    //}
                    //else
                    //{
                    //    effect.DiffuseColor = Vector3.One;
                    //}
                    ////effect.EmissiveColor = Color.White.ToVector3();
                    ////effect.SpecularColor = Color.Red.ToVector3();
                    ////effect.SpecularPower = 1000.0f;
                    ////effect.DirectionalLight0.Enabled = false;
                    ////effect.DirectionalLight1.Enabled = false;
                    ////effect.DirectionalLight2.Enabled = false;
                    //effect.EnableDefaultLighting();
                    ////effect.SpecularColor = Vector3.One;

                    //effect.PreferPerPixelLighting = true;
                    //effect.FogEnabled = true;
                    //effect.FogStart = GameConstants.FogStart;
                    //effect.FogEnd = GameConstants.FogEnd;
                    //effect.FogColor = GameConstants.FogColor.ToVector3();

                    //for our custom SkinnedEffect
                    //NewSkinnedEffect.fx
                    effect.CurrentTechnique = effect.Techniques[techniqueName];

                    effect.Parameters["World"].SetValue(Matrix.Identity);

                    effect.Parameters["Bones"].SetValue(bones);
                    effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Invert(Matrix.Identity));
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                    effect.Parameters["EyePosition"].SetValue(new Vector4(gameCamera.AvatarHeadOffset, 0));
                    Matrix WorldView = Matrix.Identity * view;
                    EffectHelpers.SetFogVector(ref WorldView, GameConstants.FogStart, GameConstants.FogEnd, effect.Parameters["FogVector"]);

                    effect.Parameters["FogColor"].SetValue(fogColor.ToVector3());
                    effect.Parameters["AmbientColor"].SetValue(ambientColor.ToVector4());
                    effect.Parameters["DiffuseColor"].SetValue(diffuseColor.ToVector4());
                    effect.Parameters["SpecularColor"].SetValue(specularColor.ToVector4());
  
                    if (isPoissoned == true)
                    {
                        effect.Parameters["DiffuseColor"].SetValue(new Vector4(Color.Green.ToVector3(), 1));
                    }
                    else
                    {
                        effect.Parameters["DiffuseColor"].SetValue(new Vector4(Vector3.One, 1));
                    }
                    effect.Parameters["Shininess"].SetValue(30);
                    if (HydroBot.gameMode == GameMode.ShipWreck)
                    {
                        effect.Parameters["DiffuseIntensity"].SetValue(0.65f);
                        effect.Parameters["AmbientIntensity"].SetValue(0.65f);
                        effect.Parameters["Shininess"].SetValue(1.0f);
                    }
                    else effect.Parameters["DiffuseIntensity"].SetValue(1.0f);
                    if (invincibleMode == true)
                    {
                        effect.Parameters["DiffuseIntensity"].SetValue(3.0f);
                        //effect.Parameters["DiffuseColor"].SetValue(new Vector4(0, 1, 0, 1));
                    }
                    else if (isBeingHealed == true)
                    {
                        effect.Parameters["DiffuseIntensity"].SetValue(diffuseIntensity);
                        diffuseIntensity -= 0.015f;
                        if (diffuseIntensity <= 0)
                        {
                            isBeingHealed = false;
                            diffuseIntensity = 10.0f;
                        }
                        effect.Parameters["DiffuseColor"].SetValue(new Vector4(0, 1, 0, 1));
                    }
    
                    //SkinnedEffect.fx
                    //effect.Parameters["ShaderIndex"].SetValue(17);
                    //effect.Parameters["WorldViewProj"].SetValue(view * projection);
                    //// The direction of the diffuse light 
                    //Vector3 DiffuseLightDirection = new Vector3(0.0f, 0.5f, 0.5f);
                    //effect.Parameters["DirLight0Direction"].SetValue(DiffuseLightDirection);
                    //// The color of the diffuse light 
                    //Vector4 DiffuseColor = new Vector4(1, 1, 1, 1);
                    //effect.Parameters["DiffuseColor"].SetValue(DiffuseColor);

                }
                mesh.Draw();
                if (autoHipnotizeMode)
                {
                    foreach (Effect effect in mesh.Effects)
                    {
                        effect.CurrentTechnique = effect.Techniques["BalloonShading"];
                        effect.Parameters["gWorldXf"].SetValue(Matrix.Identity);
                        effect.Parameters["gWorldITXf"].SetValue(Matrix.Invert(Matrix.Identity));
                        effect.Parameters["Bones"].SetValue(bones);
                        effect.Parameters["gWvpXf"].SetValue(Matrix.Identity * view * projection);
                        effect.Parameters["gViewIXf"].SetValue(Matrix.Invert(view));
                        //effect.Parameters["gInflate"].SetValue(0.07f);
                        effect.Parameters["gGlowColor"].SetValue(Color.Violet.ToVector3());
                        //effect.Parameters["gGlowExpon"].SetValue(1.5f);
                    }
                }
                else if (autoExplodeMode)
                {
                    foreach (Effect effect in mesh.Effects)
                    {
                        effect.CurrentTechnique = effect.Techniques["BalloonShading"];
                        effect.Parameters["gWorldXf"].SetValue(Matrix.Identity);
                        effect.Parameters["gWorldITXf"].SetValue(Matrix.Invert(Matrix.Identity));
                        effect.Parameters["Bones"].SetValue(bones);
                        effect.Parameters["gWvpXf"].SetValue(Matrix.Identity * view * projection);
                        effect.Parameters["gViewIXf"].SetValue(Matrix.Invert(view));
                        //effect.Parameters["gInflate"].SetValue(0.07f);
                        effect.Parameters["gGlowColor"].SetValue(Color.Red.ToVector3());
                        //effect.Parameters["gGlowExpon"].SetValue(1.5f);
                    }
                }
                else if (invincibleMode == true)
                {
                    foreach (Effect effect in mesh.Effects)
                    {
                        effect.CurrentTechnique = effect.Techniques["BalloonShading"];
                        effect.Parameters["gWorldXf"].SetValue(Matrix.Identity);
                        effect.Parameters["gWorldITXf"].SetValue(Matrix.Invert(Matrix.Identity));
                        effect.Parameters["Bones"].SetValue(bones);
                        effect.Parameters["gWvpXf"].SetValue(Matrix.Identity * view * projection);
                        effect.Parameters["gViewIXf"].SetValue(Matrix.Invert(view));
                        //effect.Parameters["gInflate"].SetValue(0.07f);
                        //effect.Parameters["gGlowColor"].SetValue(new Vector3(0.0f, 1.0f, 0.0f));
                        //effect.Parameters["gGlowExpon"].SetValue(1.5f);
                    }
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
        public static void IncreaseGoodWillPoint(int point)
        {
            if (goodWillBarActivated) goodWillPoint += point;
        }

        public bool isMoving() { return  !reachDestination; }
    }
}
