// Copyright (C) Microsoft Corporation. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon
{
    class GameConstants
    {
        //camera constants
        public const float NearClip = 1.0f;
        public const float FarClip = 1000.0f;
        public const float ViewAngle = 45.0f;

        //ship constants
        public const float Velocity = 0.75f;
        public const float BarrierVelocity = 0.1f;
        public const float TurnSpeed = 0.025f;
        public const int MainGameMaxRangeX = 500;
        public const int MainGameMaxRangeZ = 500;
        public const int ShipWreckMaxRangeX = 500;
        public const int ShipWreckMaxRangeZ = 500;
        // 5 seconds for power-ups' effects
        public const int EffectExpired = 10;

        // bullet consts
        public const float BulletSpeed = 1.0f;
        public const int HealingAmount = 5;

        // HP const
        public const int EnemyHP = 100;
        public const int FishHP = 50;
        public const int PlayerHP = 1000;

        // Bullet const
        public const int DefaultBulletDamage = 10;
        public const int DefaultEnemyDamage = 5;

        public const float EnemySpeed = 0.5f;

        //general
        public const int NumberEnemies = 20;
        public const int NumberFish = 30;
        public const int NumberShipWrecks = 10;
        public const int MaxRangeTerrain = 98;
        public const int NumBarriers = 40;
        public const int NumFuelCells = 12;
        public const int MinDistance = 10;
        public const int MaxDistance = 90;
        public static readonly TimeSpan RoundTime = TimeSpan.FromSeconds(150.25);
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
        public const int FloatHeight = 20;

        //bounding sphere scaling factors
        public const float FuelCarrierBoundingSphereFactor = .8f;
        public const float FuelCellBoundingSphereFactor = .5f;
        public const float BarrierBoundingSphereFactor = 0.7f;
        public const float TankBoundingSphereFactor = .9f;
        public const float ShipWreckBoundingSphereFactor = 2.5f;
        public const float PlantBoundingSphereFactor = 0.6f;
        public const float FruitBoundingSphereFactor = 0.9f;
        public const float FruitGrowth = 2.5f;

        //skills
        public const int numberOfSkills = 4;
        public const int numBulletTypes = 2;

        //radar
        public const float RadarScreenRadius = 75.0f;
        public const float RadarRange = 500.0f;

        //milisecond delay for double click
        public const double clickTimerDelay = 250;

        //shooting range
        public const float shootingRange = 70.0f;

        //cool down time for each skill
        public const float coolDownForHerculesBow = 5;
        public const float coolDownForArchillesArmor = 5;
        public const float coolDownForThorHammer = 5;
        public const float coolDownForHermesSandle = 5;
        public const float timeArmorLast = 5;
        public const float timeStunLast = 5;
        public const float ThorDamage = 20;
        public const float ThorRange = 40;
        public const float ThorPushFactor = 10;
        public const float HermesDamage = 30;
        public const float timeSuperSonicLast = 500;
        
    }
}