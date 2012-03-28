// Copyright (C) Microsoft Corporation. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Poseidon.Core;

namespace Poseidon {
    class GameConstants
    {
        //camera constants
        public const float NearClip = 1.0f;
        public const float FarClip = 1000.0f;
        public const float ViewAngle = 45.0f;

        //main character constants
        public const float MainCharVelocity = 0.6f;
        public const float BasicStartSpeed = 1f;
        public const float BarrierVelocity = 0.5f;
        public const float TurnSpeed = 0.025f;
        public const float MainCharShootingSpeed = 1.0f;
        public const int MainCharLevelOneExp = 250;
        public const float MainCharStrength = 1.0f;
        public const float PlayerStartingHP = 250f;
        public const float BotShootingRange = 70.0f;
        public static TimeSpan MainCharBasicTimeBetweenFire = TimeSpan.FromSeconds(0.6f);

        //experience reward
        public const int ExpPainting = 35;
        public const int BasicExpHealingFish = 8;
        //game scenes constants
        public const int MainGameMinRangeX = 100;
        public const int MainGameMinRangeZ = 100;
        public const int TrashMinRangeX = 0;
        public const int TrashMinRangeZ = 0;
        public const int ShipWreckMinRangeX = 20;
        public const int ShipWreckMinRangeZ = 20;
        public const int MainGameMaxRangeX = 500;
        public const int MainGameMaxRangeZ = 500;
        public const int ShipWreckMaxRangeX = 600;
        public const int ShipWreckMaxRangeZ = 600;
        // 5 seconds for power-ups' effects
        public const int EffectExpired = 10;
        public const int jigsawGameMaxTime = 180; //sec
        public const int boneCountForDolphinJigsaw = 8;
        public const int boneCountForSeaCowJigsaw = 16;
        public const int boneCountForTurtleJigsaw = 24;

        // bullet consts
        public const float BulletSpeed = 2.0f;
        public const int HealingAmount = 10;
                         
        // HP const
        public const int DefaultEnemyHP = 100;
        public const int DefaultFishHP = 100;
        //public const int FishHP = 50;

        //Trash
        public const int numTrashForUpgrade = 25;
        public const int numDaysForUpgrade = 3;//15;
        public const int numResourcesAtStart = 20;
        public const int numResourcesForEachFactory = 5;
        public const int maxTrashCarryingCapacity = 15;
        public const float powerpackResourceRotationSpeed = 0.08f;

        //Environment Const
        public const int PlayerStartingEnv = 300;
        public const int MaxEnv = 1000;
        public const int EachLevelMinEnv = 200;
        public const int envLossForFishDeath = 15;
        public const int envLossPerTrashAdd = 10;
        public const int envGainForBioTrashClean = 10;
        public const int envGainForPlasticTrashClean = 30;
        public const int envGainForNuclearTrashClean = 100;
        public const int expGainForTrash = 10;
        public const int BasicEnvGainForHealingFish = 5;
        public const int envGainForCorrectQuizAnswer = 50;
        //Health update of fish based on environment
        public const double maxHealthChangeInterval = 6; // Must be  greater than 5 seconds
        public const int healthChangeValue = 1; // health point change per interval
        //environment threshold for having key to treasure chest
        public const float EnvThresholdForKey = 0.75f;

        // Enemy damage
        public const int DefaultBulletDamage = 10;
        public const int DefaultEnemyDamage = 10;
        public const int EnemyShootingDamage = 7;
        public const int CombatEnemyDamage = 15;
        public const int MutantSharkBitingDamage = 25;
        public const int TerminatorShootingDamage = 25;
        public const int ChasingBulletDamage = 80;
        public const int StopBulletChasing = 3;
        public const int TorpedoDamage = 2;
        public const int LaserBeamDamage = 2;

        // Enemy configuration
        public const float EnemyShootingRate = 0.6f;
        public const float EnemySpeed = 0.3f;
        public const float EnemyShootingDistance = 15f;
        public const float EnemyPerceptionRadius = 40f;
        public const float BossPerceptionRadius = 100f;
        public const float FishSpeed = 0.45f;
        public const float EnemeyShootingRange = 50f;
        public const float TerminatorShootingRange = 110f;

        public const int maxLevel = 12;

        //general
        //number of trash, enemy and fish per level for main game
        public static int maxShipPerLevel = 9;
        public static int[] NumberTrash =           {  50,  50,  50,   50,  50,  50,  50,  50,  50,   50,   50,   50  };
        public static int[] NumberBioTrash =        {  24,  24,  24,   24,  24,  24,  24,  24,  24,   24,   24,   24  };
        public static int[] NumberPlasticTrash =    {  24,  24,  24,   24,  24,  24,  24,  24,  24,   24,   24,   24  };
        public static int[] NumberNuclearTrash =    {   2,   2,   2,    2,   2,   2,   2,   2,   2,    2,    2,    2  };
        public static int[] NumberShipWreck = { 0, 0, maxShipPerLevel, 0, 0, maxShipPerLevel, maxShipPerLevel, maxShipPerLevel, maxShipPerLevel, 0, 0, 0 };
        public static int[] FishInSchool =          {  50,  50,  50,   0,  50,  50,  50,  50,  50,   0,   0,   0  };
        public static int[] NumberShootingEnemies = new int[maxLevel];
        public static int[] NumberCombatEnemies = new int[maxLevel];
        public static int[] NumberGhostPirate = new int[maxLevel];
        public static int[] NumberFish = new int[maxLevel];
        public static int[] NumberMutantShark = new int[maxLevel];
        public static int[] NumberTerminator = new int[maxLevel];
        public static int[] NumberSubmarine = new int[maxLevel];
        public static int NumEnemiesInSubmarine = 20;
        public static double[] LevelObjective = new double[maxLevel];

        //number of enemy and fish for ship wreck
        public static int[] ShipNumberGhostPirate = { 0, 0, 1, 0, 0, 2, 3, 4, 5, 0, 0, 0 };
        public const int ShipNumberFish = 0;
        public const int NumberChests = 10;
        public const int NumStaticObjectsMain = 0;
        public const int NumStaticObjectsShip = 10;
        public const int NumTypeShipScence = 6;

        public const int MaxRangeTerrain = 98;
        public const int NumBarriers = 40;
        public const int NumFuelCells = 12;
        public const int MinDistance = 10;
        public const int MaxDistance = 90;
        public static readonly TimeSpan[] RoundTime = {TimeSpan.FromSeconds(180), TimeSpan.FromSeconds(180), TimeSpan.FromSeconds(360), 
                                                       TimeSpan.FromSeconds(180), TimeSpan.FromSeconds(120), TimeSpan.FromSeconds(360),
                                                       TimeSpan.FromSeconds(360), TimeSpan.FromSeconds(360), TimeSpan.FromSeconds(360),
                                                       TimeSpan.FromSeconds(360), TimeSpan.FromSeconds(120), TimeSpan.FromSeconds(180)}; 
        public const string StrTimeRemaining = "Days Remaining: ";
        public const string ScoreAchieved = "Score: ";
        public const int DaysPerSecond = 4; // 120 = 30 days
        public const string StrCellsFound = "Fuel Cells Retrieved: ";
        public const string StrGameWon = "Game Won !";
        public const string StrGameLost = "Game Lost !";
        public const string StrPlayAgain =
            "Press Enter/Start to play again or Esc/Back to quit";
        public const string StrInstructions1 =
            "Retrieve all Fuel Cells before time runs out.";
        public const string StrInstructions2 =
            "Control ship using keyboard (A, D, W, S) or the left thumbstick.";

        // float height for main game
        public const int MainGameFloatHeight = 20;
        public const int TrashFloatHeight = 30;
        // camera height for main game
        public const float MainCamHeight = 200;
        // float height for shipwreck
        public const int ShipWreckFloatHeight = 20;
        public const float ShipCamHeight = 150;
        
        
        //bounding sphere scaling factors
        public const float FuelCarrierBoundingSphereFactor = .8f;
        public const float FuelCellBoundingSphereFactor = .5f;
        public const float BarrierBoundingSphereFactor = 0.7f;
        public const float TankBoundingSphereFactor = 0.06f;
        public const float ShipWreckBoundingSphereFactor = 1.0f;
        public const float FruitBoundingSphereFactor = 0.9f;
        public const float TreasureChestSphereFactor = 1.0f;
        public const float TrashBoundingSphereFactor = 0.2f; //0.9f
        public const float FactoryBoundingSphereFactor = 0.9f;
        public const float FruitGrowth = 3.5f;

        //skills
        public const int numberOfSkills = 5;
        public const int numBulletTypes = 2;

        //radar
        public const float RadarScreenRadius = 85.0f;
        public const float RadarRange = 300.0f;

        //milisecond delay for double click
        public const double clickTimerDelay = 250;


        //skills specifications
        public const float coolDownForHerculesBow = 10;
        public const float coolDownForArchillesArmor = 10;
        public const float coolDownForThorHammer = 10;
        public const float coolDownForHermesSandal = 10;
        public const float coolDownForHypnotise = 1;//10;
        public const float timeArmorLast = 5;
        public const float timeStunLast = 5;
        public const float timeHypnotiseLast = 10;
        public const float ThorDamage = 40;
        public const float ThorRange = 80;
        public const float ThorPushFactor = 10;
        public const float HermesDamage = 30;
        public const float timeSuperSonicLast = 500;
        public static string[] iconNames = { "Image/SkillIcons/BowVer2", "Image/SkillIcons/HammerVer2", "Image/SkillIcons/armorVer2", 
                                               "Image/SkillIcons/sandalVer2", "Image/SkillIcons/AphroBeltVer2" };
        public static string[] bulletNames = { "Image/BulletIcons/damageBulletIcon", "Image/BulletIcons/healthBulletIcon" };
        public static int skillHealthLoss = 10;

        public const float coolDownForPlant = 5;

        //const for the fog effect
        public const float FogStart = 10;
        public const float FogEnd = 450;
        //red sea: darkred (red if lerp) - darkred - IndianRed - IndianRed
        //dead sea: black - black - gray(black if lerp) - gray
        //polar sea: dodgerblue - dodgerblue - cyan - cyan
        //seagrass meadow: all gray
        public static Color[] FogColor = {  Color.Blue, Color.Gray, Color.Blue, Color.Blue,
                                            Color.Blue, Color.DodgerBlue, Color.Black, Color.Blue,
                                            Color.Red, Color.Blue, Color.Blue, Color.Blue };
        public static Color[] AmbientColor = {  Color.DeepSkyBlue, Color.Gray, Color.DeepSkyBlue, Color.DeepSkyBlue,
                                                Color.DeepSkyBlue, Color.DodgerBlue, Color.Black, Color.DeepSkyBlue,
                                                Color.Red, Color.DeepSkyBlue, Color.DeepSkyBlue, Color.DeepSkyBlue };
        public static Color[] DiffuseColor = {  Color.Cyan, Color.Gray, Color.Cyan, Color.Cyan,
                                                Color.Cyan, Color.Cyan, Color.Black, Color.Cyan,
                                                Color.IndianRed, Color.Cyan, Color.Cyan, Color.Cyan };
        public static Color[] SpecularColor = {   Color.LightSkyBlue, Color.Gray, Color.LightSkyBlue, Color.LightSkyBlue,
                                                  Color.LightSkyBlue, Color.Cyan, Color.Black, Color.LightSkyBlue, 
                                                  Color.IndianRed, Color.LightSkyBlue, Color.LightSkyBlue, Color.LightSkyBlue };

        public static bool[] PreferBlueColor = { false, false, false, false,
                                                 false, false, true, false,
                                                 true, false, false, false };
        //so that it is easier to aim and hit
        public const float EasyAimScale = 2.0f;
        public const float EasyHitScale = 1.5f;
        
        //for playing background musics
        public const int NumNormalBackgroundMusics = 4;
        public const int NumBossBackgroundMusics = 2;
        public const int NumMinigameBackgroundMusics = 1;
        public const int NumJigsawBackgroundMusics = 2;

        //attributes
        public static int gainAttributeCost = 1;
        public static float gainSpeed = 0.06f;
        public static float gainShootingRate = 0.06f;
        public static float gainStrength = 0.06f;
        public static int gainHitPoint = 20;

        //consts for the survival mode
        public const int SurvivalModeMaxShootingEnemy = 35;
        public const int SurvivalModeMaxCombatEnemy = 35;
        public const int SurvivalModeMaxGhostPirate = 30;
        public const int SurvivalModeMaxMutantShark = 15;
        public const int SurvivalModeMaxTerminator = 2;
        public const int SurvivalModeMaxSubmarine = 2;

        public const int NumGoodWillBarIcons = 13;
        public const int MaxGoodWillPoint = 1000;
        public const int GoodWillPointGainForPlanting = 25;
        public const int GoodWillPointGainForCleaning = 50;
        public const int GoodWillPointGainForHealing = 25;

        //const for particle systems
        public static int numExplosionParticles = (int) (20 * GameSettings.NumParticleLevel) ;
        public static int numSandParticles = (int)(30 * GameSettings.NumParticleLevel);
        public static int numSandParticlesForFactory = (int)(80 * GameSettings.NumParticleLevel);
        public static float trailParticlesPerSecond = (int)(100 * GameSettings.NumParticleLevel);
        public static int numFrozenBreathParticlesPerUpdate = (int)(3 * GameSettings.NumParticleLevel);

        // constants for Factory Buildings
        public const int FactoryPanelMaxButtons = 4;

        public const float SideKick_Look_Radius = 60f;
        //constants for certain effects
        public const float distortionDuration = 1000;

        // part id for factory part animation while it's in processing state
        public const int biofactoryPartId = 6;
        public const int biofactoryLevelPartId = 3;
        public const int nuclearfactoryPartId = 2;
        public const int plasticfactoryPartId = 0;
        public const int plasticfactoryLevelPartId = 5;

        //constants for animal health
        public const float DolphinStartingHealth = 500;
        public const float SeaCowStartingHealth = 1000;
        public const float TurtleStartingHealth = 2000;

        public const int MaxNumTries = 200;
    }
}
