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
    public partial class PlayGameScene
    {
        private void loadContentEnemies()
        {
            enemiesAmount = GameConstants.NumberEnemies;
            for (int i = 0; i < enemiesAmount; i++)
            {
                enemies[i] = new Enemy();
                enemies[i].LoadContent(Content, "Models/pyramid10uR");
            }
        }

        private void loadContentFish()
        {
            fishAmount = GameConstants.NumberFish;
            for (int i = 0; i < fishAmount; i++)
            {
                fish[i] = new Fish();
                fish[i].LoadContent(Content, "Models/cube10uR");
            }
        }

        private void placeEnemies()
        {
            loadContentEnemies();

            int min = GameConstants.MinDistance;
            int max = GameConstants.MaxDistance;
            Vector3 tempCenter;

            //place enemies
            for (int i = 0; i < enemiesAmount; i++)
            {
                enemies[i].Position = GenerateRandomPosition(min, max);
                enemies[i].Position.Y = GameConstants.FloatHeight;
                tempCenter = enemies[i].BoundingSphere.Center;
                tempCenter.X = enemies[i].Position.X;
                tempCenter.Y = GameConstants.FloatHeight;
                tempCenter.Z = enemies[i].Position.Z;
                enemies[i].BoundingSphere =
                    new BoundingSphere(tempCenter, enemies[i].BoundingSphere.Radius);
            }
        }

        private void placeFish()
        {
            loadContentFish();

            int min = GameConstants.MinDistance;
            int max = GameConstants.MaxDistance;
            Vector3 tempCenter;

            //place fish
            for (int i = 0; i < fishAmount; i++)
            {
                fish[i].Position = GenerateRandomPosition(min, max);
                fish[i].Position.Y = GameConstants.FloatHeight;
                tempCenter = fish[i].BoundingSphere.Center;
                tempCenter.X = fish[i].Position.X;
                tempCenter.Y = GameConstants.FloatHeight;
                tempCenter.Z = fish[i].Position.Z;
                fish[i].BoundingSphere =
                    new BoundingSphere(tempCenter, fish[i].BoundingSphere.Radius);
            }
        }

        private void placeFuelCells()
        {
            int min = GameConstants.MinDistance;
            int max = GameConstants.MaxDistance;
            Vector3 tempCenter;

            //place fuel cells
            foreach (FuelCell cell in fuelCells)
            {
                cell.Position = GenerateRandomPosition(min, max);
                cell.Position.Y = GameConstants.FloatHeight;
                tempCenter = cell.BoundingSphere.Center;
                tempCenter.X = cell.Position.X;
                tempCenter.Y = GameConstants.FloatHeight;
                tempCenter.Z = cell.Position.Z;
                cell.BoundingSphere =
                    new BoundingSphere(tempCenter, cell.BoundingSphere.Radius);
                cell.Retrieved = false;
            }
        }

        private void placeShipWreck()
        {
            int min = GameConstants.MinDistance;
            int max = GameConstants.MaxDistance;
            Vector3 tempCenter;

            //place ship wrecks
            foreach (ShipWreck shipWreck in shipWrecks)
            {
                shipWreck.Position = GenerateRandomPosition(min, max);
                //ship wreck should not be floating
                shipWreck.Position.Y = 0;
                tempCenter = shipWreck.BoundingSphere.Center;
                tempCenter.X = shipWreck.Position.X;
                tempCenter.Y = 0;
                tempCenter.Z = shipWreck.Position.Z;
                shipWreck.BoundingSphere = new BoundingSphere(tempCenter,
                    shipWreck.BoundingSphere.Radius);
            }
        }

        // Helper
        private void placeBullet()
        {
            Projectiles p = new Projectiles();
            p.initialize(GraphicDevice.Viewport, tank.Position, GameConstants.BulletSpeed, tank.ForwardDirection, tank.strengthUp);
            p.loadContent(Content, "Models/sphere1uR");
            projectiles.Add(p);
        }

        // Helper
        private void placePlant()
        {
            Plant p = new Plant();
            p.LoadContent(Content, tank.Position);
            if (Collision.isPlantPositionValid(p, plants, shipWrecks))
            {
                plants.Add(p);
            }
        }

        // Helper
        private Vector3 GenerateRandomPosition(int min, int max)
        {
            int xValue, zValue;
            do
            {
                xValue = random.Next(min, max);
                zValue = random.Next(min, max);
                if (random.Next(100) % 2 == 0)
                    xValue *= -1;
                if (random.Next(100) % 2 == 0)
                    zValue *= -1;

            } while (IsOccupied(xValue, zValue));

            return new Vector3(xValue, 0, zValue);
        }

        // Helper
        private bool IsOccupied(int xValue, int zValue)
        {
            foreach (GameObject currentObj in fuelCells)
            {
                if (((int)(MathHelper.Distance(
                    xValue, currentObj.Position.X)) < 15) &&
                    ((int)(MathHelper.Distance(
                    zValue, currentObj.Position.Z)) < 15))
                    return true;
            }

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

            foreach (GameObject currentObj in shipWrecks)
            {
                if (((int)(MathHelper.Distance(
                    xValue, currentObj.Position.X)) < 15) &&
                    ((int)(MathHelper.Distance(
                    zValue, currentObj.Position.Z)) < 15))
                    return true;
            }
            return false;
        }
    }
}
