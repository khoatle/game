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
            for (int i = 0; i < enemiesAmount; i++) {
                enemies[i] = new Enemy();
                enemies[i].LoadContent(Content, "Models/fuelcarrier");
            }
        }

        public static void loadContentFish(ref int fishAmount, Fish[] fish, ContentManager Content, int currentLevel, bool mainGame)
        {
            if (mainGame)
                fishAmount = GameConstants.NumberFish[currentLevel];
            else fishAmount = GameConstants.ShipNumberFish;
            for (int i = 0; i < fishAmount; i++) {
                fish[i] = new Fish();
                fish[i].LoadContent(Content, "Models/cube10uR");
            }
        }

        public static void placeEnemies(ref int enemiesAmount, Enemy[] enemies, ContentManager Content, Random random, int fishAmount, Fish[] fish, List<ShipWreck> shipWrecks, int minX, int maxX, int minZ, int maxZ, int currentLevel, bool mainGame)
        {
            loadContentEnemies(ref enemiesAmount, enemies, Content, currentLevel, mainGame);

            //int min = GameConstants.MinDistance;
            //int max = GameConstants.MaxDistance;
            Vector3 tempCenter;

            //place enemies
            for (int i = 0; i < enemiesAmount; i++)
            {
                enemies[i].Position = GenerateRandomPosition(minX, maxX, minZ, maxZ, random, enemiesAmount, fishAmount, enemies, fish, shipWrecks);
                enemies[i].Position.Y = GameConstants.FloatHeight;
                tempCenter = enemies[i].BoundingSphere.Center;
                tempCenter.X = enemies[i].Position.X;
                tempCenter.Y = GameConstants.FloatHeight;
                tempCenter.Z = enemies[i].Position.Z;
                enemies[i].BoundingSphere =
                    new BoundingSphere(tempCenter, enemies[i].BoundingSphere.Radius);
            }
        }

        public static void placeFish(ref int fishAmount, Fish[] fish, ContentManager Content, Random random, int enemiesAmount, Enemy[] enemies, List<ShipWreck> shipWrecks, int minX, int maxX, int minZ, int maxZ, int currentLevel, bool mainGame)
        {
            loadContentFish(ref fishAmount, fish, Content, currentLevel, mainGame);

            //int min = GameConstants.MinDistance;
            //int max = GameConstants.MaxDistance;
            Vector3 tempCenter;

            //place fish
            for (int i = 0; i < fishAmount; i++)
            {
                fish[i].Position = GenerateRandomPosition(minX, maxX, minZ, maxZ, random, enemiesAmount, fishAmount, enemies, fish, shipWrecks);
                fish[i].Position.Y = GameConstants.FloatHeight;
                tempCenter = fish[i].BoundingSphere.Center;
                tempCenter.X = fish[i].Position.X;
                tempCenter.Y = GameConstants.FloatHeight;
                tempCenter.Z = fish[i].Position.Z;
                fish[i].BoundingSphere =
                    new BoundingSphere(tempCenter, fish[i].BoundingSphere.Radius);
            }
        }


        public static void placeShipWreck(List<ShipWreck> shipWrecks, Random random, int enemiesAmount, int fishAmount, Enemy[] enemies, Fish[] fish, HeightMapInfo heightMapInfo, int minX, int maxX, int minZ, int maxZ)
        {
            //int min = GameConstants.MinDistance;
            //int max = GameConstants.MaxDistance;
            Vector3 tempCenter;

            //place ship wrecks
            foreach (ShipWreck shipWreck in shipWrecks)
            {
                shipWreck.Position = GenerateRandomPosition(minX, maxX, minZ, maxZ, random, enemiesAmount, fishAmount, enemies, fish, shipWrecks);
                //ship wreck should not be floating
                shipWreck.Position.Y = heightMapInfo.GetHeight(shipWreck.Position);
                tempCenter = shipWreck.BoundingSphere.Center;
                tempCenter.X = shipWreck.Position.X;
                tempCenter.Y = GameConstants.FloatHeight;
                tempCenter.Z = shipWreck.Position.Z;
                shipWreck.BoundingSphere = new BoundingSphere(tempCenter,
                    shipWreck.BoundingSphere.Radius);
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

        public static void placeDamageBullet(Tank tank, ContentManager Content, List<DamageBullet> myBullet) {
            DamageBullet d = new DamageBullet();

            Matrix orientationMatrix = Matrix.CreateRotationY(tank.ForwardDirection);
            Vector3 movement = Vector3.Zero;
            movement.Z = 1;
            Vector3 shootingDirection = Vector3.Transform(movement, orientationMatrix);
            
            d.initialize(tank.Position, shootingDirection, GameConstants.BulletSpeed, tank.strength, tank.strengthUp);
            d.loadContent(Content, "Models/fuelcell");
            myBullet.Add(d);
        }

        public static void placeEnemyBullet(Vector3 bulletPosition, Vector3 shootingDirection, int damage, List<DamageBullet> bullets, AudioLibrary audio) {
            DamageBullet newBullet = new DamageBullet();

            newBullet.initialize(bulletPosition, shootingDirection, GameConstants.BulletSpeed, damage);
            newBullet.loadContent(PlayGameScene.Content, "Models/sphere1uR");
            audio.Shooting.Play();
            bullets.Add(newBullet);
        }

        // Helper
        public static void placePlant(Tank tank, HeightMapInfo heightMapInfo, ContentManager Content, TimeSpan roundTimer, List<Plant> plants, List<ShipWreck> shipWrecks)
        {
            Plant p = new Plant();
            Vector3 possiblePosition = tank.Position;
            possiblePosition.Y = heightMapInfo.GetHeight(tank.Position);
            p.LoadContent(Content, possiblePosition, roundTimer.TotalSeconds);
            if (Collision.isPlantPositionValid(p, plants, shipWrecks)) {
                plants.Add(p);
            }
        }

        // Helper
        public static Vector3 GenerateRandomPosition(int minX, int maxX, int minZ, int maxZ, Random random, int enemiesAmount, int fishAmount, Enemy[] enemies, Fish[] fish, List<ShipWreck> shipWrecks)
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

            } while (IsOccupied(xValue, zValue, enemiesAmount, fishAmount, enemies, fish, shipWrecks));

            return new Vector3(xValue, 0, zValue);
        }

        // Helper
        public static bool IsOccupied(int xValue, int zValue, int enemiesAmount, int fishAmount, Enemy[] enemies, Fish[] fish, List<ShipWreck> shipWrecks)
        {
            for (int i = 0; i < enemiesAmount; i++)
            {
                if (((int)(MathHelper.Distance(
                    xValue, enemies[i].Position.X)) < 15) &&
                    ((int)(MathHelper.Distance(
                    zValue, enemies[i].Position.Z)) < 15))
                    return true;
            }

            for (int i = 0; i < fishAmount; i++)
            {
                if (((int)(MathHelper.Distance(
                    xValue, fish[i].Position.X)) < 15) &&
                    ((int)(MathHelper.Distance(
                    zValue, fish[i].Position.Z)) < 15))
                    return true;
            }
            if (shipWrecks != null)
            {
                foreach (GameObject currentObj in shipWrecks)
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

        public static void placeStarFish(List<StaticObjects> starfishes, Random random, HeightMapInfo heightMapInfo, int minX, int maxX, int minZ, int maxZ)
        {
            int xValue, zValue;
            //place star fish
            foreach (StaticObjects starfish in starfishes)
            {
                xValue = random.Next(minX, maxX);
                zValue = random.Next(minZ, maxZ);
                if (random.Next(100) % 2 == 0)
                    xValue *= -1;
                if (random.Next(100) % 2 == 0)
                    zValue *= -1;

                //starfish.Position = new Vector3(xValue, 0, zValue);
                starfish.Position = new Vector3(xValue, zValue, 0);
                //starfish.Position.Y = heightMapInfo.GetHeight(starfish.Position);
                starfish.Position.Z = heightMapInfo.GetHeight(starfish.Position);
            }
        }

    }
}
