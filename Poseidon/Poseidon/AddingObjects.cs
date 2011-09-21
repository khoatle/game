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
    public partial class PlayGameScene {
        private void loadContentEnemies() {
            for (int i = 0; i < GameConstants.NumberEnemies; i++) {
                enemies[i] = new Enemy();
                enemies[i].LoadContent(Content, "Models/tank");
            }
        }

        private void loadContentFish() {
            for (int i = 0; i < GameConstants.NumberEnemies; i++)
            {
                fish[i] = new Fish();
                fish[i].LoadContent(Content, "Models/cube10uR");
            }
        }

        private void placeEnemies() {
        }

        private void placeFish() {
        }


        // Helper
        private void PlaceFuelCellsAndBarriers()
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

            //place barriers
            foreach (Barrier barrier in barriers)
            {
                barrier.Position = GenerateRandomPosition(min, max);
                barrier.Position.Y = GameConstants.FloatHeight;
                tempCenter = barrier.BoundingSphere.Center;
                tempCenter.X = barrier.Position.X;
                tempCenter.Y = GameConstants.FloatHeight;
                tempCenter.Z = barrier.Position.Z;
                barrier.BoundingSphere = new BoundingSphere(tempCenter,
                    barrier.BoundingSphere.Radius);
            }
        }

        // Helper
        private void placeBullet()
        {
            Projectiles p = new Projectiles();
            p.initialize(GraphicDevice.Viewport, tank.Position, GameConstants.BulletSpeed, tank.ForwardDirection);
            p.loadContent(Content, "Models/sphere1uR");
            projectiles.Add(p);
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

            foreach (GameObject currentObj in barriers)
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
