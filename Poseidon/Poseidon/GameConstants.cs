﻿// Copyright (C) Microsoft Corporation. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon {
    class GameConstants
    {
        //camera constants
        public const float NearClip = 1.0f;
        public const float FarClip = 1000.0f;
        public const float ViewAngle = 45.0f;

        //ship constants
        public const float Velocity = 0.75f;
        public const float BarrierVelocity = 0.5f;
        public const float TurnSpeed = 0.025f;
        public const int MainGameMinRangeX = 20;
        public const int MainGameMinRangeZ = 20;
        public const int ShipWreckMinRangeX = 20;
        public const int ShipWreckMinRangeZ = 20;
        public const int MainGameMaxRangeX = 500;
        public const int MainGameMaxRangeZ = 500;
        public const int ShipWreckMaxRangeX = 30; //changing this will mess up the shiwreck width.
        public const int ShipWreckMaxRangeZ = 800;
        // 5 seconds for power-ups' effects
        public const int EffectExpired = 10;

        // bullet consts
        public const float BulletSpeed = 2.0f;
        public const int HealingAmount = 5;

        // HP const
        public const int DefaultEnemyHP = 100;
        public const int FishHP = 50;
        public const int PlayerStartingHP = 1000;

        //Environment Const
        public const int PlayerStartingEnv = 500;
        public const int MaxEnv = 1000;

        // Bullet const
        public const int DefaultBulletDamage = 10;
        public const int DefaultEnemyDamage = 5;

        public const float EnemySpeed = 0.5f;
        public const float EnemyShootingDistance = 15f;
        public const float EnemyPerceptionRadius = 30f;
        public const float BossPerceptionRadius = 100f;
        public const float FishSpeed = 0.5f;

        //general
        //number of trash, enemy and fish per level for main game
        //public static int[] NumberTrash =           { 100,  50,  50,   0,  50,  50,  50, 50,  50,  0  };
        public static int[] NumberTrash =           { 0, 50, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] NumberShootingEnemies = {   0,  15,  20,   0,  25,  30,  35,  40,  45,  10  };
        public static int[] NumberCombatEnemies =   {   0,  15,  20,   0,  25,  30,  35,  50,  45,  10  };
        public static int[] NumberFish =            {  50,  50,  50,   0,  50,  50,  50,  50,  50,   0  };
        public static int[] NumberMutantShark =     {   0,   0,   0,   1,   2,   3,   4,   5,   6,   0  };
        public static int[] NumberTerminator =      {   0,   0,   0,   0,   1,   0,   0,   0,   0,   1  };
        public static int[] NumberShipWreck =       {   0,   5,   5,   0,   0,   5,   5,   5,   5,   0  };
        public static int[] FishInSchool =          {  50,  50,  50,   0,  50,  50,  50,  50,  50,   0  };
        //number of enemy and fish for ship wreck
        public const int ShipNumberShootingEnemies = 5;
        public const int ShipNumberCombatEnemies = 5;
        public const int ShipNumberFish = 0;
        public const int NumberChests = 10;
        public const int NumStaticObjectsMain = 0;
        public const int NumStaticObjectsShip = 10;

        public const int MaxRangeTerrain = 98;
        public const int NumBarriers = 40;
        public const int NumFuelCells = 12;
        public const int MinDistance = 10;
        public const int MaxDistance = 90;
        public static readonly TimeSpan RoundTime = TimeSpan.FromSeconds(120);
        public const string StrTimeRemaining = "Time Remaining: ";
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
        public const float ShipCamHeight = 120;
        
        
        //bounding sphere scaling factors
        public const float FuelCarrierBoundingSphereFactor = .8f;
        public const float FuelCellBoundingSphereFactor = .5f;
        public const float BarrierBoundingSphereFactor = 0.7f;
        public const float TankBoundingSphereFactor = .9f;
        public const float ShipWreckBoundingSphereFactor = 1.0f;
        public const float PlantBoundingSphereFactor = 0.2f;
        public const float FruitBoundingSphereFactor = 0.9f;
        public const float TreasureChestSphereFactor = 1.0f;
        public const float TrashBoundingSphereFactor = 0.9f;
        public const float FruitGrowth = 5.0f;

        //skills
        public const int numberOfSkills = 5;
        public const int numBulletTypes = 2;

        //radar
        public const float RadarScreenRadius = 85.0f;
        public const float RadarRange = 300.0f;

        //milisecond delay for double click
        public const double clickTimerDelay = 250;

        //shooting range
        public const float shootingRange = 70.0f;

        //skills specifications
        public const float coolDownForHerculesBow = 5;
        public const float coolDownForArchillesArmor = 5;
        public const float coolDownForThorHammer = 5;
        public const float coolDownForHermesSandle = 5;
        public const float coolDownForHypnotise = 5;
        public const float timeArmorLast = 5;
        public const float timeStunLast = 5;
        public const float timeHypnotiseLast = 10;
        public const float ThorDamage = 20;
        public const float ThorRange = 40;
        public const float ThorPushFactor = 10;
        public const float HermesDamage = 30;
        public const float timeSuperSonicLast = 500;
        public static string[] iconNames = { "Image/Bow", "Image/Hammer", "Image/armor", "Image/sandal", "Image/AphroBelt" };
        public static string[] bulletNames = { "Image/damageBullet", "Image/healthBullet" };
        public static int gainSkillCost = 1;
        public static int skillHealthLoss = 10;

        public const float coolDownForPlant = 5;

        //const for the fog effect
        public const float FogStart = 10;
        public const float FogEnd = 430;//350;
        public static Color FogColor = Color.Blue;//Color.CornflowerBlue;
    }
}
