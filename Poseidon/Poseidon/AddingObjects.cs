using System;
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
        public static void loadContentEnemies(ref int enemiesAmount, Enemy[] enemies, ContentManager Content, int currentLevel, bool mainGame)
        {
            if (mainGame)
                enemiesAmount = GameConstants.NumberEnemies[currentLevel];
            else enemiesAmount = GameConstants.ShipNumberEnemies;
            for (int i = 0; i < enemiesAmount - 2; i++) {
                enemies[i] = new Enemy();
                enemies[i].LoadContent(Content, "Models/Fuelcarrier");
            }
            MutantShark mutantShark = new MutantShark();
            mutantShark.LoadContent(Content, "Models/mutantShark");
            enemies[enemiesAmount - 2] = mutantShark;
            
            Terminator terminator = new Terminator();
            terminator.LoadContent(Content, "Models/squirrel");
            terminator.Load();
            enemies[enemiesAmount - 1] = terminator;
        }

        public static void loadContentFish(ref int fishAmount, Fish[] fish, ContentManager Content, int currentLevel, bool mainGame)
        {
            if (mainGame)
                fishAmount = GameConstants.NumberFish[currentLevel];
            else fishAmount = GameConstants.ShipNumberFish;
            Random random = new Random();
            int type = random.Next(6);
            for (int i = 0; i < fishAmount; i++) {
                fish[i] = new Fish();
                //fish[i].LoadContent(Content, "Models/orca1");
                //fish[i].Load();
                if (type == 0)
                    fish[i].LoadContent(Content, "Models/fish_fbxascii");
                else if (type == 1)
                    fish[i].LoadContent(Content, "Models/fish2");
                else if (type == 2)
                    fish[i].LoadContent(Content, "Models/shark");
                else if (type == 3)
                    fish[i].LoadContent(Content, "Models/dolphin");
                else if (type == 4)
                    fish[i].LoadContent(Content, "Models/orca1");
                else if (type == 5)
                    fish[i].LoadContent(Content, "Models/shark2");
                type = random.Next(6);
            }
        }

        public static void placeEnemies(ref int enemiesAmount, Enemy[] enemies, ContentManager Content, Random random, int fishAmount, Fish[] fish, List<ShipWreck> shipWrecks, int minX, int maxX, int minZ, int maxZ, int currentLevel, bool mainGame, float floatHeight)
        {
            loadContentEnemies(ref enemiesAmount, enemies, Content, currentLevel, mainGame);

            //int min = GameConstants.MinDistance;
            //int max = GameConstants.MaxDistance;
            Vector3 tempCenter;

            //place enemies
            for (int i = 0; i < enemiesAmount; i++)
            {
                enemies[i].Position = GenerateSurfaceRandomPosition(minX, maxX, minZ, maxZ, random, enemiesAmount, fishAmount, enemies, fish, shipWrecks);
                enemies[i].Position.Y = floatHeight;
                tempCenter = enemies[i].BoundingSphere.Center;
                tempCenter.X = enemies[i].Position.X;
                tempCenter.Y = floatHeight;
                tempCenter.Z = enemies[i].Position.Z;
                enemies[i].BoundingSphere =
                    new BoundingSphere(tempCenter, enemies[i].BoundingSphere.Radius);
            }
        }

        public static void placeFish(ref int fishAmount, Fish[] fish, ContentManager Content, Random random, int enemiesAmount, Enemy[] enemies, List<ShipWreck> shipWrecks, int minX, int maxX, int minZ, int maxZ, int currentLevel, bool mainGame, float floatHeight)
        {
            loadContentFish(ref fishAmount, fish, Content, currentLevel, mainGame);

            //int min = GameConstants.MinDistance;
            //int max = GameConstants.MaxDistance;
            Vector3 tempCenter;

            //place fish
            for (int i = 0; i < fishAmount; i++)
            {
                fish[i].Position = GenerateSurfaceRandomPosition(minX, maxX, minZ, maxZ, random, enemiesAmount, fishAmount, enemies, fish, shipWrecks);
                fish[i].Position.Y = floatHeight;
                tempCenter = fish[i].BoundingSphere.Center;
                tempCenter.X = fish[i].Position.X;
                tempCenter.Y = floatHeight;
                tempCenter.Z = fish[i].Position.Z;
                fish[i].BoundingSphere =
                    new BoundingSphere(tempCenter, fish[i].BoundingSphere.Radius);
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
        public static void placeTreasureChests(List<TreasureChest> treasureChests, Random random, HeightMapInfo heightMapInfo, int minX, int maxX, int minZ, int maxZ)
        {

            Vector3 tempCenter;

            //place treasure chests
            foreach (TreasureChest chest in treasureChests)
            {
                chest.Position = GenerateShipFloorRandomPosition(minX, maxX, minZ, maxZ, random, treasureChests);
                //ship wreck should not be floating
                chest.Position.Y = heightMapInfo.GetHeight(chest.Position);
                tempCenter = chest.BoundingSphere.Center;
                tempCenter.X = chest.Position.X;
                tempCenter.Y = GameConstants.ShipWreckFloatHeight;
                tempCenter.Z = chest.Position.Z;
                chest.BoundingSphere = new BoundingSphere(tempCenter,
                    chest.BoundingSphere.Radius);
            }
        }
        public static void placeHealingBullet(Tank tank, ContentManager Content, List<HealthBullet> healthBullet) {
            HealthBullet h = new HealthBullet();
            Matrix orientationMatrix = Matrix.CreateRotationY(tank.ForwardDirection);
            Vector3 movement = Vector3.Zero;
            movement.Z = 1;
            Vector3 shootingDirection = Vector3.Transform(movement, orientationMatrix);
 
            h.initialize(tank.Position, shootingDirection, GameConstants.BulletSpeed, tank.strength, tank.strengthUp);
            h.loadContent(Content, "Models/sphere1uR");
            healthBullet.Add(h);
        }

        public static void placeTankDamageBullet(Tank tank, ContentManager Content, List<DamageBullet> myBullet) {
            DamageBullet d = new DamageBullet();

            Matrix orientationMatrix = Matrix.CreateRotationY(tank.ForwardDirection);
            Vector3 movement = Vector3.Zero;
            movement.Z = 1;
            Vector3 shootingDirection = Vector3.Transform(movement, orientationMatrix);
            
            d.initialize(tank.Position, shootingDirection, GameConstants.BulletSpeed, tank.strength, tank.strengthUp);
            d.loadContent(Content, "Models/fuelcell");
            myBullet.Add(d);
        }

        public static void placeEnemyBullet(GameObject obj, int damage, List<DamageBullet> bullets, int type) {
            Tank tmp1;
            SwimmingObject tmp2;
            Matrix orientationMatrix;
            if (obj.GetType().Name.Equals("Tank")) {
                tmp1 = (Tank)obj;
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

            newBullet.initialize(obj.Position, shootingDirection, GameConstants.BulletSpeed, damage);
            if (type == 1)
                newBullet.loadContent(PlayGameScene.Content, "Models/bossBullet1");
            else newBullet.loadContent(PlayGameScene.Content, "Models/sphere1uR");
            bullets.Add(newBullet);
        }

        public static bool placePlant(Tank tank, HeightMapInfo heightMapInfo, ContentManager Content, TimeSpan roundTimer, List<Plant> plants, List<ShipWreck> shipWrecks, List<StaticObject> staticObjects)
        {
            Plant p = new Plant();
            Vector3 possiblePosition = tank.Position;
            possiblePosition.Y = heightMapInfo.GetHeight(tank.Position);
            p.LoadContent(Content, possiblePosition, roundTimer.TotalSeconds);
            if (Collision.isPlantPositionValid(p, plants, shipWrecks, staticObjects)) {
                plants.Add(p);
                return true;
            }
            return false;
        }

        public static void placeTrash(
            List<Trash> trashes, int enemiesAmount, Enemy[] enemies, ContentManager Content, Random random, int fishAmount, Fish[] fish, List<ShipWreck> shipWrecks, int minX, int maxX, int minZ, int maxZ, int currentLevel, bool mainGame, float floatHeight)
        {
            Vector3 tempCenter;

            foreach (Trash trash in trashes)
            {
                trash.Position = GenerateSurfaceRandomPosition(minX, maxX, minZ, maxZ, random, enemiesAmount, fishAmount, enemies,
                    fish, shipWrecks);
                trash.Position.Y = floatHeight;
                tempCenter = trash.BoundingSphere.Center;
                tempCenter.X = trash.Position.X;
                tempCenter.Y = floatHeight;
                tempCenter.Z = trash.Position.Z;
                trash.BoundingSphere = new BoundingSphere(tempCenter,
                    trash.BoundingSphere.Radius);
            }
        }

        // Helper
        public static Vector3 GenerateSurfaceRandomPosition(int minX, int maxX, int minZ, int maxZ, Random random, int enemiesAmount, int fishAmount, Enemy[] enemies, Fish[] fish, List<ShipWreck> shipWrecks)
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
        public static Vector3 GenerateShipFloorRandomPosition(int minX, int maxX, int minZ, int maxZ, Random random, List<TreasureChest> treasureChests)
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

            } while (IsShipFloorPlaceOccupied(xValue, zValue, treasureChests));

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
        public static bool IsSurfaceOccupied(int xValue, int zValue, int enemiesAmount, int fishAmount, Enemy[] enemies, Fish[] fish)
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
        public static bool IsShipFloorPlaceOccupied(int xValue, int zValue, List<TreasureChest> treasureChests)
        {

            if (treasureChests != null)
            {
                foreach (GameObject currentObj in treasureChests)
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
        // Helper
        public static bool IsSeaBedPlaceOccupied(int xValue, int zValue, List<ShipWreck> shipWrecks, List<StaticObject> staticObjects)
        {

            if (shipWrecks != null)
            {
                //not so close to the ship wreck
                foreach (GameObject currentObj in shipWrecks)
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
    }
}
