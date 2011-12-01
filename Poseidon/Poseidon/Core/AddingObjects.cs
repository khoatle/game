﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Poseidon.Core;

namespace Poseidon
{
    public static class AddingObjects
    {
        public static void loadContentEnemies(ref int enemiesAmount, BaseEnemy[] enemies, ContentManager Content, int currentLevel, GameMode gameMode)
        {
         
            if (gameMode == GameMode.SurvivalMode)
            {
                enemiesAmount = GameConstants.SurvivalModeMaxShootingEnemy + GameConstants.SurvivalModeMaxCombatEnemy
                + GameConstants.SurvivalModeMaxMutantShark + GameConstants.SurvivalModeMaxTerminator;
            }
            else if (gameMode == GameMode.MainGame)
                enemiesAmount = GameConstants.NumberShootingEnemies[currentLevel] + GameConstants.NumberCombatEnemies[currentLevel]
                    + GameConstants.NumberMutantShark[currentLevel] + GameConstants.NumberTerminator[currentLevel];
            else if (gameMode == GameMode.ShipWreck)
            {
                if (PlayGameScene.currentLevel >= 4)
                    enemiesAmount = GameConstants.ShipHighNumberShootingEnemies + GameConstants.ShipHighNumberCombatEnemies;
                else enemiesAmount = GameConstants.ShipLowNumberShootingEnemies + GameConstants.ShipLowNumberCombatEnemies;
            }
            int numShootingEnemies = 0;
            int numCombatEnemies= 0;
            int numMutantShark = 0;
            int numTerminator = 0;
        
            if (gameMode == GameMode.SurvivalMode)
            {
                numShootingEnemies = GameConstants.SurvivalModeMaxShootingEnemy;
                numCombatEnemies = GameConstants.SurvivalModeMaxCombatEnemy;
                numMutantShark = GameConstants.SurvivalModeMaxMutantShark;
                numTerminator = GameConstants.SurvivalModeMaxTerminator;
            }
            else if (gameMode == GameMode.MainGame)
            {
                numShootingEnemies = GameConstants.NumberShootingEnemies[currentLevel];
                numCombatEnemies = GameConstants.NumberCombatEnemies[currentLevel];
                numMutantShark = GameConstants.NumberMutantShark[currentLevel];
                numTerminator = GameConstants.NumberTerminator[currentLevel];
            }
            else if (gameMode == GameMode.ShipWreck)
            {
                if (PlayGameScene.currentLevel >= 4)
                {
                    numShootingEnemies = GameConstants.ShipHighNumberShootingEnemies;
                    numCombatEnemies = GameConstants.ShipHighNumberCombatEnemies;
                }
                else
                {
                    numShootingEnemies = GameConstants.ShipLowNumberShootingEnemies;
                    numCombatEnemies = GameConstants.ShipLowNumberCombatEnemies;
                }
            }
            Random rnd = new Random();
            for (int i = 0; i < enemiesAmount; i++) {
                if (i < numShootingEnemies)
                {
                    enemies[i] = new ShootingEnemy();
                    enemies[i].Name = "Shooting Enemy";
                    enemies[i].LoadContent(Content, "Models/EnemyModels/diver_green_ly");
                    enemies[i].Load(1, 25, 24);
                }
                else if (i < numShootingEnemies + numCombatEnemies){
                    enemies[i] = new CombatEnemy();
                    enemies[i].Name = "Combat Enemy";
                    enemies[i].LoadContent(Content, "Models/EnemyModels/diver_knife_orange_yellow");
                    enemies[i].Load(1, 30, 24);// 31 60 for attack
                }
                else if (i < numShootingEnemies + numCombatEnemies + numMutantShark)
                {
                    MutantShark mutantShark = new MutantShark();
                    mutantShark.LoadContent(Content, "Models/EnemyModels/mutantSharkVer2");
                    mutantShark.Name = "mutant shark";
                    enemies[i] = mutantShark;
                }
                else if (i < numShootingEnemies + numCombatEnemies + numMutantShark + numTerminator)
                {
                    Terminator terminator = new Terminator(gameMode);
                    terminator.LoadContent(Content, "Models/EnemyModels/terminator");
                    if (currentLevel == 4 && gameMode == GameMode.MainGame) terminator.Name = "???";
                    else terminator.Name = "terminator";
                    terminator.Load(31, 60, 24);
                    enemies[i] = terminator;
                }
            }
            
        }

        public static void loadContentFish(ref int fishAmount, Fish[] fish, ContentManager Content, int currentLevel, GameMode gameMode)
        {
            if (gameMode == GameMode.MainGame)
                fishAmount = GameConstants.NumberFish[currentLevel];
            else if (gameMode == GameMode.ShipWreck) fishAmount = GameConstants.ShipNumberFish;
            //there is only 1 fish to protect in the survival mode
            else if (gameMode == GameMode.SurvivalMode)
            {
                fishAmount = 1;
            }
            else fishAmount = 0;
            Random random = new Random();
            int type;
            //Level 4 is shark only
            if (currentLevel == 4)
                type = random.Next(6, 9);
            else type = random.Next(9);

            for (int i = 0; i < fishAmount; i++) {
                fish[i] = new Fish();
                if (gameMode == GameMode.SurvivalMode)
                {
                    fish[i].Name = "Ancient ";
                    fish[i].isBigBoss = true;
                }
                else fish[i].Name = "";
                if (type == 0)
                {
                    fish[i].LoadContent(Content, "Models/SeaAnimalModels/turtle");
                    //name must be initialized before Load()
                    fish[i].Name += "turtle";
                    fish[i].Load(1, 24, 24);
                    fish[i].happy_talk = "We are reptiles from much before the Jurassic age. Oh, I cry not for sorrow, just to get the salt out.";
                    fish[i].sad_talk = "I need to go to the beach to lay eggs. Can you ask the humans not to kill me?";  
                }
                else if (type == 1)
                {
                    fish[i].LoadContent(Content, "Models/SeaAnimalModels/dolphin");
                    fish[i].Name += "dolphin";
                    fish[i].Load(1, 24, 24);
                    fish[i].happy_talk = "We remind you to play, play, play, for you will find great power in play.";
                    fish[i].sad_talk = "Though we try to be friends with humans, they always hurt us with their pollution, propellers and what not!";
                }
                else if (type == 2)
                {
                    fish[i].LoadContent(Content, "Models/SeaAnimalModels/manetee");
                    fish[i].Name += "manetee";
                    fish[i].Load(1, 24, 24);
                    fish[i].happy_talk = "Do not call me sea-cow. Do I look that fat?";
                    fish[i].sad_talk = "I am a vegeterian. Why are they killing me?";
                }
                else if (type == 3)
                {
                    fish[i].LoadContent(Content, "Models/SeaAnimalModels/stingray");
                    fish[i].Name += "sting ray";
                    fish[i].Load(1, 24, 24);
                    fish[i].happy_talk = "I can't see you as my eyes are on top. But I can sense a bot with my electro-receptors.";
                    fish[i].sad_talk = "I will teach you to sting, if you promise to sting everyone who eat bbq sting-ray.";
                }
                else if (type == 4)
                {
                    fish[i].LoadContent(Content, "Models/SeaAnimalModels/orca");
                    fish[i].Name += "orca";
                    fish[i].Load(1, 24, 24);
                    fish[i].happy_talk = "Move away, you little bot, here comes the killer whale.";
                    fish[i].sad_talk = "I lost my way. I can't hear my friends due to the noise from the oil-rig.";
                }
                else if (type == 5)
                {

                    fish[i].LoadContent(Content, "Models/SeaAnimalModels/seal");
                    fish[i].Name += "seal";
                    fish[i].Load(1, 24, 24);
                    fish[i].happy_talk = "See how I swim, with a swerve and a twist, a flip of the flipper, a flick of the wrist!";
                    fish[i].sad_talk = "We need the arctic ice. Stop global warming.";
                }
                else if (type == 6)
                {
                    fish[i].LoadContent(Content, "Models/SeaAnimalModels/normalshark");
                    fish[i].Name += "shark";
                    fish[i].Load(1, 24, 24);
                    fish[i].happy_talk = "You stink like a rusty metal. I can smell it. I also hear a prey far away. I'll go 15mph this time.";
                    fish[i].sad_talk = "Humans kill over 30 million sharks every year. We are the oldest fish, spare us.";
                }
                else if (type == 7)
                {
                    fish[i].LoadContent(Content, "Models/SeaAnimalModels/leopardshark");
                    fish[i].Name += "leopard shark";
                    fish[i].Load(1, 24, 24);
                    fish[i].happy_talk = "No, I am not racist and I date all kinds of shark, not you, dear bot.";
                    fish[i].sad_talk = "We never eat humans. Why do they hurt us?";
                }
                else if (type == 8)
                {
                    fish[i].LoadContent(Content, "Models/SeaAnimalModels/hammershark");
                    fish[i].Name += "hammer shark";
                    fish[i].Load(1, 24, 24);
                    fish[i].happy_talk = "I have 360 degree binocular vision. I can detect an electrical signal of half a billionth of a volt. What superpower you brag about?";
                    fish[i].sad_talk = "Why do humans like our fins so much. Does 'delicacy' mean genocide?";
                }
                //fish[i].LoadContent(Content, "Models/orca");
                //fish[i].Load(1, 24, 24);
                //fish[i].Name = "hammer shark";
                if (currentLevel == 4)
                    type = random.Next(6, 9);
                else type = random.Next(9);
            }
        }

        public static void placeEnemies(ref int enemiesAmount, BaseEnemy[] enemies, ContentManager Content, Random random, int fishAmount, Fish[] fish, List<ShipWreck> shipWrecks, int minX, int maxX, int minZ, int maxZ, int currentLevel, GameMode gameMode, float floatHeight)
        {
            loadContentEnemies(ref enemiesAmount, enemies, Content, currentLevel, gameMode);

            //int min = GameConstants.MinDistance;
            //int max = GameConstants.MaxDistance;
            //Vector3 tempCenter;

            //place enemies
            for (int i = 0; i < enemiesAmount; i++)
            {
                enemies[i].Position = GenerateSurfaceRandomPosition(minX, maxX, minZ, maxZ, random, enemiesAmount, fishAmount, enemies, fish, shipWrecks);
                enemies[i].Position.Y = floatHeight;
                //tempCenter = enemies[i].BoundingSphere.Center;
                //tempCenter.X = enemies[i].Position.X;
                //tempCenter.Y = floatHeight;
                //tempCenter.Z = enemies[i].Position.Z;
                enemies[i].BoundingSphere =
                    new BoundingSphere(enemies[i].Position, enemies[i].BoundingSphere.Radius);
                //enemies[i].ChangeBoundingSphere();
            }
        }

        public static void placeFish(ref int fishAmount, Fish[] fish, ContentManager Content, Random random, int enemiesAmount, BaseEnemy[] enemies, List<ShipWreck> shipWrecks, int minX, int maxX, int minZ, int maxZ, int currentLevel, GameMode gameMode, float floatHeight)
        {
            loadContentFish(ref fishAmount, fish, Content, currentLevel, gameMode);

            //int min = GameConstants.MinDistance;
            //int max = GameConstants.MaxDistance;
            //Vector3 tempCenter;

            //place fish
            for (int i = 0; i < fishAmount; i++)
            {
                //in survival mode, try to place the ancient fish near you
                if (gameMode == GameMode.SurvivalMode)
                    fish[i].Position = GenerateSurfaceRandomPosition(0, minX, 0, minZ, random, enemiesAmount, fishAmount, enemies, fish, shipWrecks);
                else fish[i].Position = GenerateSurfaceRandomPosition(minX, maxX, minZ, maxZ, random, enemiesAmount, fishAmount, enemies, fish, shipWrecks);
                fish[i].Position.Y = floatHeight;
                //tempCenter = fish[i].BoundingSphere.Center;
                //tempCenter.X = fish[i].Position.X;
                //tempCenter.Y = floatHeight;
                //tempCenter.Z = fish[i].Position.Z;
                fish[i].BoundingSphere =
                    new BoundingSphere(fish[i].Position, fish[i].BoundingSphere.Radius);
            }
        }


        public static void placeShipWreck(List<ShipWreck> shipWrecks, List<StaticObject> staticObjects, Random random, HeightMapInfo heightMapInfo, int minX, int maxX, int minZ, int maxZ)
        {
            //int min = GameConstants.MinDistance;
            //int max = GameConstants.MaxDistance;
            Vector3 tempCenter;

            //place ship wrecks
            foreach (ShipWreck shipWreck in shipWrecks)
            {
                shipWreck.Position = GenerateSeaBedRandomPosition(minX, maxX, minZ, maxZ, random, shipWrecks, staticObjects);
                //ship wreck should not be floating
                shipWreck.Position.Y = heightMapInfo.GetHeight(shipWreck.Position);
                tempCenter = shipWreck.BoundingSphere.Center;
                tempCenter.X = shipWreck.Position.X;
                tempCenter.Y = GameConstants.MainGameFloatHeight;
                tempCenter.Z = shipWreck.Position.Z;
                shipWreck.BoundingSphere = new BoundingSphere(tempCenter,
                    shipWreck.BoundingSphere.Radius);
            }
        }
        public static void placeTreasureChests(List<TreasureChest> treasureChests, List<StaticObject> staticObjects, Random random, HeightMapInfo heightMapInfo, int minX, int maxX, int minZ, int maxZ)
        {

            Vector3 tempCenter;

            //place treasure chests
            foreach (TreasureChest chest in treasureChests)
            {
                chest.Position = GenerateShipFloorRandomPosition(minX, maxX, minZ, maxZ, random, treasureChests, staticObjects);
                //ship wreck should not be floating
                chest.Position.Y = 0;// heightMapInfo.GetHeight(chest.Position);
                tempCenter = chest.BoundingSphere.Center;
                tempCenter.X = chest.Position.X;
                tempCenter.Y = GameConstants.ShipWreckFloatHeight;
                tempCenter.Z = chest.Position.Z;
                chest.BoundingSphere = new BoundingSphere(tempCenter,
                    chest.BoundingSphere.Radius);
                if (chest.Position.X > 0) chest.orientation = -MathHelper.PiOver2;
                else chest.orientation = MathHelper.PiOver2;
            }
        }
        public static void placeHealingBullet(HydroBot hydroBot, ContentManager Content, List<HealthBullet> healthBullet) {
            HealthBullet h = new HealthBullet();
            Matrix orientationMatrix = Matrix.CreateRotationY(hydroBot.ForwardDirection);
            Vector3 movement = Vector3.Zero;
            movement.Z = 1;
            Vector3 shootingDirection = Vector3.Transform(movement, orientationMatrix);
 
            h.initialize(hydroBot.Position, shootingDirection, GameConstants.BulletSpeed, HydroBot.strength, HydroBot.strengthUp);
            h.loadContent(Content, "Models/BulletModels/healBullet");
            PoseidonGame.audio.botNormalShot.Play();
            healthBullet.Add(h);
        }

        public static void placeBotDamageBullet(HydroBot hydroBot, ContentManager Content, List<DamageBullet> myBullet) {
            DamageBullet d = new DamageBullet();

            Matrix orientationMatrix = Matrix.CreateRotationY(hydroBot.ForwardDirection);
            Vector3 movement = Vector3.Zero;
            movement.Z = 1;
            Vector3 shootingDirection = Vector3.Transform(movement, orientationMatrix);
            
            d.initialize(hydroBot.Position, shootingDirection, GameConstants.BulletSpeed, HydroBot.strength, HydroBot.strengthUp);
            d.loadContent(Content, "Models/BulletModels/damageBullet");
            PoseidonGame.audio.botNormalShot.Play();
            myBullet.Add(d);
        }

        public static void placeChasingBullet(GameObject shooter, GameObject target, List<DamageBullet> bullets, BoundingFrustum cameraFrustum) {
            if (!target.GetType().Name.Equals("HydroBot")) {
                return;
            }
            
            Matrix orientationMatrix = Matrix.CreateRotationY(((Terminator)shooter).ForwardDirection);
            Vector3 movement = Vector3.Zero;
            movement.Z = 1;
            Vector3 shootingDirection = Vector3.Transform(movement, orientationMatrix);
            
            ChasingBullet newBullet = new ChasingBullet();
            newBullet.initialize(shooter.Position, shootingDirection, GameConstants.BulletSpeed, GameConstants.ChasingBulletDamage, target);
            newBullet.loadContent(PoseidonGame.contentManager, "Models/BulletModels/chasingBullet");
            bullets.Add(newBullet);
            if (shooter.BoundingSphere.Intersects(cameraFrustum)) {
                PoseidonGame.audio.chasingBulletSound.Play();
            }
        }


        public static void placeEnemyBullet(GameObject obj, int damage, List<DamageBullet> bullets, int type, BoundingFrustum cameraFrustum, float offsetFactor) {
            HydroBot tmp1;
            SwimmingObject tmp2;
            Matrix orientationMatrix;
            if (obj.GetType().Name.Equals("HydroBot")) {
                tmp1 = (HydroBot)obj;
                orientationMatrix = Matrix.CreateRotationY(tmp1.ForwardDirection);
            }
            else {
                tmp2 = (SwimmingObject)obj;
                orientationMatrix = Matrix.CreateRotationY(tmp2.ForwardDirection);
            }
            
            DamageBullet newBullet = new DamageBullet();

            Vector3 movement = Vector3.Zero;
            movement.Z = 1;
            Vector3 shootingDirection = Vector3.Transform(movement, orientationMatrix);
            newBullet.initialize(obj.Position + shootingDirection * offsetFactor, shootingDirection, GameConstants.BulletSpeed, damage);
            if (type == 1)
            {
                newBullet.loadContent(PoseidonGame.contentManager, "Models/BulletModels/bossBullet");
                if (obj.BoundingSphere.Intersects(cameraFrustum))
                    PoseidonGame.audio.bossShot.Play();
            }
            else
            {
                newBullet.loadContent(PoseidonGame.contentManager, "Models/BulletModels/normalbullet");
                if (obj.BoundingSphere.Intersects(cameraFrustum))
                    PoseidonGame.audio.enemyShot.Play();
            }
            bullets.Add(newBullet);
        }

        public static bool placePlant(HydroBot hydroBot, HeightMapInfo heightMapInfo, ContentManager Content, List<Plant> plants, List<ShipWreck> shipWrecks, List<StaticObject> staticObjects, GameTime gameTime)
        {
            if ((PoseidonGame.playTime.TotalSeconds - HydroBot.prevPlantTime > GameConstants.coolDownForPlant) || HydroBot.firstPlant == true)
            {
                Plant p = new Plant();
                Vector3 possiblePosition = hydroBot.Position;
                possiblePosition.Y = heightMapInfo.GetHeight(hydroBot.Position);
                p.LoadContent(Content, possiblePosition, PoseidonGame.playTime.TotalSeconds);
                if (Collision.isPlantPositionValid(p, plants, shipWrecks, staticObjects))
                {
                    plants.Add(p);
                    HydroBot.firstPlant = false;
                    HydroBot.prevPlantTime = PoseidonGame.playTime.TotalSeconds;
                    return true;
                }
            }
            return false;
        }

        public static void placeTrash(
            List<Trash> trashes,  ContentManager Content, Random random, List<ShipWreck> shipWrecks, int minX, int maxX, int minZ, int maxZ, int currentLevel, GameMode gameMode, float floatHeight, HeightMapInfo heightMapInfo)
        {
            Vector3 tempCenter;
            int positionSign;
            foreach (Trash trash in trashes)
            {
                //trash.Position = GenerateSurfaceRandomPosition(minX, maxX, minZ, maxZ, random, enemiesAmount, fishAmount, enemies,
                //    fish, shipWrecks);
                positionSign = random.Next(4);
                trash.Position.X= random.Next(minX, maxX);
                trash.Position.Z = random.Next(minZ, maxZ);
                switch(positionSign)
                {
                    case 0:
                        trash.Position.X *= -1;
                        break;
                    case 1:
                        trash.Position.Z *= -1;
                        break;
                    case 2:
                        trash.Position.X *= -1;
                        trash.Position.Z *= -1;
                        break;
                }
                trash.Position.Y = heightMapInfo.GetHeight(new Vector3(trash.Position.X, 0, trash.Position.Z));//GameConstants.TrashFloatHeight;
                tempCenter = trash.BoundingSphere.Center;
                tempCenter.X = trash.Position.X;
                tempCenter.Y = floatHeight;
                tempCenter.Z = trash.Position.Z;
                trash.BoundingSphere = new BoundingSphere(tempCenter,
                    trash.BoundingSphere.Radius);
            }
        }

        // Helper
        public static Vector3 GenerateSurfaceRandomPosition(int minX, int maxX, int minZ, int maxZ, Random random, int enemiesAmount, int fishAmount, BaseEnemy[] enemies, Fish[] fish, List<ShipWreck> shipWrecks)
        {
            int xValue, zValue;
            do
            {
                xValue = random.Next(minX, maxX);
                zValue = random.Next(minZ, maxZ);
                if (random.Next(100) % 2 == 0)
                    xValue *= -1;
                if (random.Next(100) % 2 == 0)
                    zValue *= -1;

            } while (IsSurfaceOccupied(xValue, zValue, enemiesAmount, fishAmount, enemies, fish));

            return new Vector3(xValue, 0, zValue);
        }
        // Helper
        public static Vector3 GenerateShipFloorRandomPosition(int minX, int maxX, int minZ, int maxZ, Random random, List<TreasureChest> treasureChests, List<StaticObject> staticObjects)
        {
            int xValue, zValue;
            do
            {
                xValue = random.Next(minX, maxX);
                zValue = random.Next(minZ, maxZ);
                if (random.Next(100) % 2 == 0)
                    xValue *= -1;
                if (random.Next(100) % 2 == 0)
                    zValue *= -1;

            } while (IsShipFloorPlaceOccupied(xValue, zValue, treasureChests, staticObjects));
            if (xValue > 0) xValue = maxX - 8;
            else xValue = -maxX + 8;
            return new Vector3(xValue, 0, zValue);
        }
        // Helper
        public static Vector3 GenerateSeaBedRandomPosition(int minX, int maxX, int minZ, int maxZ, Random random, List<ShipWreck> shipWrecks, List<StaticObject> staticObjects)
        {
            int xValue, zValue;
            do
            {
                xValue = random.Next(minX, maxX);
                zValue = random.Next(minZ, maxZ);
                if (random.Next(100) % 2 == 0)
                    xValue *= -1;
                if (random.Next(100) % 2 == 0)
                    zValue *= -1;
                
            } while (IsSeaBedPlaceOccupied(xValue, zValue, shipWrecks, staticObjects));
            
            return new Vector3(xValue, 0, zValue);
        }
        // Helper
        public static bool IsSurfaceOccupied(int xValue, int zValue, int enemiesAmount, int fishAmount, BaseEnemy[] enemies, Fish[] fish)
        {
            for (int i = 0; i < enemiesAmount; i++)
            {
                if (((int)(MathHelper.Distance(
                    xValue, enemies[i].Position.X)) < 50) &&
                    ((int)(MathHelper.Distance(
                    zValue, enemies[i].Position.Z)) < 50))
                    return true;
            }

            for (int i = 0; i < fishAmount; i++)
            {
                if (((int)(MathHelper.Distance(
                    xValue, fish[i].Position.X)) < 50) &&
                    ((int)(MathHelper.Distance(
                    zValue, fish[i].Position.Z)) < 50))
                    return true;
            }
           
            return false;
        }
        // Helper
        public static bool IsShipFloorPlaceOccupied(int xValue, int zValue, List<TreasureChest> treasureChests, List<StaticObject> staticObjects)
        {

            if (treasureChests != null)
            {
                foreach (GameObject currentObj in treasureChests)
                {
                    if (((int)(MathHelper.Distance(
                        xValue, currentObj.Position.X)) < 50) &&
                        ((int)(MathHelper.Distance(
                        zValue, currentObj.Position.Z)) < 50))
                        return true;
                }
            }
            if (staticObjects != null)
            {
                foreach (GameObject currentObj in staticObjects)
                {
                    if (((int)(MathHelper.Distance(
                        xValue, currentObj.Position.X)) < 50) &&
                        ((int)(MathHelper.Distance(
                        zValue, currentObj.Position.Z)) < 50))
                        return true;
                }
            }
            return false;
        }
        // Helper
        public static bool IsSeaBedPlaceOccupied(int xValue, int zValue, List<ShipWreck> shipWrecks, List<StaticObject> staticObjects)
        {

            if (shipWrecks != null)
            {
                //not so close to the ship wreck
                foreach (GameObject currentObj in shipWrecks)
                {
                    if (((int)(MathHelper.Distance(
                        xValue, currentObj.Position.X)) < 200) &&
                        ((int)(MathHelper.Distance(
                        zValue, currentObj.Position.Z)) < 200))
                        return true;
                }
            }
            if (staticObjects != null)
            {
                foreach (GameObject currentObj in staticObjects)
                {
                    if (((int)(MathHelper.Distance(
                        xValue, currentObj.Position.X)) < 15) &&
                        ((int)(MathHelper.Distance(
                        zValue, currentObj.Position.Z)) < 15))
                        return true;
                }
            }
            return false;
        }

        public static void PlaceStaticObjects(List<StaticObject> staticObjects, List<ShipWreck> shipWrecks, Random random, HeightMapInfo heightMapInfo, int minX, int maxX, int minZ, int maxZ)
        {
            Vector3 tempCenter;

            //place ship wrecks
            foreach (StaticObject staticObject in staticObjects)
            {
                staticObject.Position = GenerateSeaBedRandomPosition(minX, maxX, minZ, maxZ, random, shipWrecks, staticObjects);
                //ship wreck should not be floating
                staticObject.Position.Y = heightMapInfo.GetHeight(staticObject.Position);
                tempCenter = staticObject.BoundingSphere.Center;
                tempCenter.X = staticObject.Position.X;
                tempCenter.Y = staticObject.Position.Y;
                tempCenter.Z = staticObject.Position.Z;
                staticObject.BoundingSphere = new BoundingSphere(tempCenter,
                    staticObject.BoundingSphere.Radius);
            }
        }

        public static void PlaceStaticObjectsOnShipFloor(List<StaticObject> staticObjects, List<TreasureChest> treasureChests, Random random, HeightMapInfo heightMapInfo, int minX, int maxX, int minZ, int maxZ)
        {
            Vector3 tempCenter;

            //place ship wrecks
            foreach (StaticObject staticObject in staticObjects)
            {
                staticObject.Position = GenerateShipFloorRandomPosition(minX, maxX, minZ, maxZ, random, treasureChests, staticObjects);
                //ship wreck should not be floating
                staticObject.Position.Y = 0;// heightMapInfo.GetHeight(staticObject.Position);
                tempCenter = staticObject.BoundingSphere.Center;
                tempCenter.X = staticObject.Position.X;
                tempCenter.Y = staticObject.Position.Y;
                tempCenter.Z = staticObject.Position.Z;
                staticObject.BoundingSphere = new BoundingSphere(tempCenter,
                    staticObject.BoundingSphere.Radius);
            }
        }

        public static void DrawHealthBar(Texture2D HealthBar, Game game, SpriteBatch spriteBatch, SpriteFont statsFont, int currentHealth, int maxHealth, int heightFromTop, string type, Color typeColor)
        {
            int barX = game.Window.ClientBounds.Width / 2 - HealthBar.Width / 2;
            int barY = heightFromTop;
            int barHeight = 22;
            double healthiness = (double)currentHealth/maxHealth;

            //Draw the negative space for the health bar
            spriteBatch.Draw(HealthBar,
                new Rectangle(barX, barY, HealthBar.Width, barHeight),
                new Rectangle(0, barHeight + 1, HealthBar.Width, barHeight),
                Color.Transparent);
            //Draw the current health level based on the current Health
            Color healthColor = Color.LawnGreen;
            if (healthiness < 0.2)
                healthColor = Color.DarkRed;
            else if (healthiness < 0.5)
                healthColor = Color.Orange;
            spriteBatch.Draw(HealthBar,
                new Rectangle(barX, barY, (int)(HealthBar.Width * healthiness), barHeight),
                new Rectangle(0, barHeight + 1, HealthBar.Width, barHeight),
                healthColor);
            //Draw the box around the health bar
            spriteBatch.Draw(HealthBar,
                new Rectangle(barX, barY, HealthBar.Width, barHeight),
                new Rectangle(0, 0, HealthBar.Width, barHeight),
                Color.White);
            spriteBatch.DrawString(statsFont, type.ToUpper(), new Vector2(game.Window.ClientBounds.Width / 2 - ((type.Length / 2) * 14), heightFromTop - 1), typeColor);
        }

        public static void DrawEnvironmentBar(Texture2D Bar, Game game, SpriteBatch spriteBatch, SpriteFont statsFont, int currentEnvironment, int maxEnvironemnt)
        {
            int barX = game.Window.ClientBounds.Right - 50;
            int barY = game.Window.ClientBounds.Center.Y-Bar.Height/2;
            string type = "ENVIRONMENT";
            Color typeColor = Color.Black;
            int barWidth = Bar.Width/2;
            double healthiness = (double)currentEnvironment / maxEnvironemnt;
            //Draw the negative space for the health bar
            spriteBatch.Draw(Bar,
                new Rectangle(barX, barY, barWidth, Bar.Height),
                new Rectangle(barWidth+1, 0, barWidth, Bar.Height),
                Color.Transparent);
            //Draw the current health level based on the current Health
            Color healthColor = Color.Gold;
            if (healthiness < 0.2)
                healthColor = Color.DarkRed;
            else if (healthiness < 0.5)
                healthColor = Color.Red;
            else if (healthiness < 0.8)
                healthColor = Color.LawnGreen;
            spriteBatch.Draw(Bar,
                new Rectangle(barX, barY + (Bar.Height - (int)(Bar.Height * healthiness)), barWidth, (int)(Bar.Height * healthiness)),
                new Rectangle(barWidth+1, 0, barWidth, Bar.Height),
                healthColor);
            //Draw the box around the health bar
            spriteBatch.Draw(Bar,
                new Rectangle(barX, barY, barWidth, Bar.Height),
                new Rectangle(0, 0, barWidth, Bar.Height),
                Color.White);
            //spriteBatch.DrawString(statsFont, type.ToUpper(), new Vector2(game.Window.ClientBounds.Width / 2 - ((type.Length / 2) * 14), heightFromTop - 1), typeColor);
            //spriteBatch.DrawString(statsFont, type.ToUpper(), new Vector2(barX + 10, barY + 20), typeColor, 90.0f, new Vector2(barX + 10, barY + 20), 1, SpriteEffects.FlipVertically, 0);
            spriteBatch.DrawString(statsFont, type.ToUpper(), new Vector2(barX + 35, barY + 70), typeColor, 3.14f / 2, new Vector2(0,0), 1, SpriteEffects.None, 0);
        }


        public static void DrawLevelBar(Texture2D LevelBar, Game game, SpriteBatch spriteBatch, SpriteFont statsFont, int currentExperience, int nextLevelExp, int level, int heightFromTop, string type, Color typeColor)
        {
            int barX = game.Window.ClientBounds.Width / 2 - LevelBar.Width / 2;
            int barY = heightFromTop;
            int barHeight = 22;
            double experience = (double)currentExperience / nextLevelExp;
            type += " " + level.ToString();
            //Draw the negative space for the health bar
            spriteBatch.Draw(LevelBar,
                new Rectangle(barX, barY, LevelBar.Width, barHeight),
                new Rectangle(0, barHeight + 1, LevelBar.Width, barHeight),
                Color.Transparent);
            //Draw the current health level based on the current Health
            spriteBatch.Draw(LevelBar,
                new Rectangle(barX, barY, (int)(LevelBar.Width * experience), barHeight),
                new Rectangle(0, barHeight + 1, LevelBar.Width, barHeight),
                Color.Aqua);
            //Draw the box around the health bar
            spriteBatch.Draw(LevelBar,
                new Rectangle(barX, barY, LevelBar.Width, barHeight),
                new Rectangle(0, 0, LevelBar.Width, barHeight),
                Color.White);
            spriteBatch.DrawString(statsFont, type.ToUpper(), new Vector2(game.Window.ClientBounds.Width / 2 - ((type.Length / 2) * 11), heightFromTop - 1), typeColor);
        }

        public static string wrapLine(string input_line, int width, SpriteFont font)
        {
            String line = String.Empty;
            String returnString = String.Empty;
            String[] wordArray = input_line.Split(' ');

            foreach (String word in wordArray)
            {
                if (font.MeasureString(line + word).Length() > width)
                {
                    returnString = returnString + line + '\n';
                    line = String.Empty;
                }
                line = line + word + ' ';
            }
            return returnString + line;
        }

        private static Vector3 PerpendicularVector(Vector3 directionVector)
        {
            return new Vector3(-directionVector.Z, directionVector.Y, directionVector.X);
        }

        public static void ReviveDeadEnemy(BaseEnemy[] enemies, int enemyAmount, Fish[] fishes, int fishAmount, HydroBot hydroBot)
        {
            Random random = new Random();
            for (int i = 0; i < enemyAmount; i++)
            {
                if (enemies[i].health <= 0)
                {
                    enemies[i].health = enemies[i].maxHealth;
                    enemies[i].stunned = false;
                    enemies[i].isHypnotise = false;

                    int xValue, zValue;
                    do
                    {
                        xValue = random.Next(0, hydroBot.MaxRangeX);
                        zValue = random.Next(0, hydroBot.MaxRangeZ);

                    } while (IsSurfaceOccupied(xValue, zValue, enemyAmount, fishAmount, enemies, fishes) ||
                         (((int)(MathHelper.Distance(xValue, hydroBot.Position.X)) < 200) &&
                         ((int)(MathHelper.Distance(zValue, hydroBot.Position.Z)) < 200)));

                    enemies[i].Position.X = xValue;
                    enemies[i].Position.Z = zValue;
                    enemies[i].Position.Y = hydroBot.floatHeight;
                    enemies[i].BoundingSphere =
                        new BoundingSphere(enemies[i].Position, enemies[i].BoundingSphere.Radius);
                }
            }
        }
    }
}
