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
    class CursorManager
    {
        public static bool InShootingRange(HydroBot hydroBot, Cursor cursor, Camera gameCamera, float planeHeight)
        {
            Vector3 pointIntersect = IntersectPointWithPlane(cursor, gameCamera, planeHeight);
            Vector3 mouseDif = pointIntersect - hydroBot.Position;
            float distanceFromTank = mouseDif.Length();
            if (distanceFromTank < GameConstants.BotShootingRange)
                return true;
            else
                return false;
        }

        public static bool RayIntersectsBoundingSphere(Ray ray, BoundingSphere boundingSphere)
        {
            if (boundingSphere.Intersects(ray) != null)
            {
                return true;
            }
            return false;
        }

        public static bool MouseOnEnemy(Cursor cursor, Camera gameCamera, BaseEnemy[] enemies, int enemiesAmount)
        {
            Ray cursorRay = cursor.CalculateCursorRay(gameCamera.ProjectionMatrix, gameCamera.ViewMatrix);
            BoundingSphere sphere;
            for (int i = 0; i < enemiesAmount; i++)
            {
                //making it easier to aim
                sphere = enemies[i].BoundingSphere;
                sphere.Radius *= GameConstants.EasyAimScale;
                if (RayIntersectsBoundingSphere(cursorRay, sphere))
                {
                    cursor.SetShootingMouseImage();
                    return true;
                }
            }
            cursor.SetNormalMouseImage();
            return false;
        }

        public static BaseEnemy MouseOnWhichEnemy(Cursor cursor, Camera gameCamera, BaseEnemy[] enemies, int enemiesAmount)
        {
            Ray cursorRay = cursor.CalculateCursorRay(gameCamera.ProjectionMatrix, gameCamera.ViewMatrix);
            BoundingSphere sphere;
            for (int i = 0; i < enemiesAmount; i++)
            {
                //making it easier to aim
                sphere = enemies[i].BoundingSphere;
                sphere.Radius *= GameConstants.EasyAimScale;
                if (RayIntersectsBoundingSphere(cursorRay, sphere))
                {
                    //cursor.SetShootingMouseImage();
                    return enemies[i];
                }
            }
            //cursor.SetNormalMouseImage();
            return null;
        }


        public static bool MouseOnFish(Cursor cursor, Camera gameCamera, Fish[] fish, int fishAmount)
        {
            BoundingSphere sphere;
            Ray cursorRay = cursor.CalculateCursorRay(gameCamera.ProjectionMatrix, gameCamera.ViewMatrix);
            for (int i = 0; i < fishAmount; i++)
            {
                sphere = fish[i].BoundingSphere;
                sphere.Radius *= GameConstants.EasyAimScale;
                if (RayIntersectsBoundingSphere(cursorRay, sphere))
                {
                    cursor.SetOnFishMouseImage();
                    return true;
                }
            }
            cursor.SetNormalMouseImage();
            return false;
        }

        public static Fish MouseOnWhichFish(Cursor cursor, Camera gameCamera, Fish[] fish, int fishAmount)
        {
            BoundingSphere sphere;
            Ray cursorRay = cursor.CalculateCursorRay(gameCamera.ProjectionMatrix, gameCamera.ViewMatrix);
            for (int i = 0; i < fishAmount; i++)
            {
                sphere = fish[i].BoundingSphere;
                sphere.Radius *= GameConstants.EasyAimScale;
                if (RayIntersectsBoundingSphere(cursorRay, sphere))
                {
                    return fish[i];
                }
            }
            //cursor.SetNormalMouseImage();
            return null;
        }

        public static Trash MouseOnWhichTrash(Cursor cursor, Camera gameCamera, List<Trash> trashes)
        {
            BoundingSphere trashRealSphere;
            Ray cursorRay = cursor.CalculateCursorRay(gameCamera.ProjectionMatrix, gameCamera.ViewMatrix);
            foreach (Trash trash in trashes)
            {
                trashRealSphere = trash.BoundingSphere;
                trashRealSphere.Center.Y = trash.Position.Y;
                trashRealSphere.Radius *= 5;
                if (RayIntersectsBoundingSphere(cursorRay, trashRealSphere))
                {
                    return trash;
                }
            }
            //cursor.SetNormalMouseImage();
            return null;
        }

        public static ShipWreck MouseOnWhichShipWreck(Cursor cursor, Camera gameCamera, List<ShipWreck> shipWrecks)
        {
            BoundingSphere shipRealSphere;
            Ray cursorRay = cursor.CalculateCursorRay(gameCamera.ProjectionMatrix, gameCamera.ViewMatrix);
            foreach (ShipWreck shipWreck in shipWrecks)
            {
                shipRealSphere = shipWreck.BoundingSphere;
                shipRealSphere.Center.Y = shipWreck.Position.Y;
                shipRealSphere.Radius *= 1;
                if (RayIntersectsBoundingSphere(cursorRay, shipRealSphere))
                {
                    return shipWreck;
                }
            }
            //cursor.SetNormalMouseImage();
            return null;
        }

        public static Factory MouseOnWhichFactory(Cursor cursor, Camera gameCamera, List<Factory> factories)
        {
            BoundingSphere factoryRealSphere;
            Ray cursorRay = cursor.CalculateCursorRay(gameCamera.ProjectionMatrix, gameCamera.ViewMatrix);
            foreach (Factory factory in factories)
            {
                factoryRealSphere = factory.BoundingSphere;
                factoryRealSphere.Center.Y = factory.Position.Y;
                factoryRealSphere.Radius *= 1;
                if (RayIntersectsBoundingSphere(cursorRay, factoryRealSphere))
                {
                    return factory;
                }
            }
            return null;
        }

        public static bool MouseOnResearchFacility(Cursor cursor, Camera gameCamera, ResearchFacility researchFacility)
        {
            if (researchFacility == null) return false;
            BoundingSphere researchFacilityRealSphere;
            Ray cursorRay = cursor.CalculateCursorRay(gameCamera.ProjectionMatrix, gameCamera.ViewMatrix);
            researchFacilityRealSphere = researchFacility.BoundingSphere;
            researchFacilityRealSphere.Center.Y = researchFacility.Position.Y;
            researchFacilityRealSphere.Radius *= 1;
            if (RayIntersectsBoundingSphere(cursorRay, researchFacilityRealSphere))
            {
                return true;
            }
            else return false;
        }

        public static void CheckClick(ref MouseState lastMouseState, ref MouseState currentMouseState, GameTime gameTime, ref double clickTimer, ref bool clicked, ref bool doubleClicked)
        {
            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            clickTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (lastMouseState.LeftButton == ButtonState.Pressed
                && currentMouseState.LeftButton == ButtonState.Released)
            {

                if (clicked && (clickTimer < GameConstants.clickTimerDelay))
                {
                    doubleClicked = true;
                    clicked = false;
                }
                else
                {
                    doubleClicked = false;
                    clicked = true;
                }
                clickTimer = 0;
            }
        }
        public static float CalculateAngle(Vector3 point2, Vector3 point1)
        {
            return (float)Math.Atan2(point2.X - point1.X, point2.Z - point1.Z);
        }
        public static Vector3 IntersectPointWithPlane(Cursor cursor, Camera gameCamera, float planeHeight)
        {
            Ray cursorRay = cursor.CalculateCursorRay(gameCamera.ProjectionMatrix, gameCamera.ViewMatrix);
            float t = (planeHeight - cursorRay.Position.Y) / cursorRay.Direction.Y;
            float x = cursorRay.Position.X + cursorRay.Direction.X * t;
            float z = cursorRay.Position.Z + cursorRay.Direction.Z * t;
            return new Vector3(x, planeHeight, z);
        }
        public static bool MouseOnObject(Cursor cursor, BoundingSphere boundingSphere, Vector3 center, Camera gameCamera)
        {

            Ray cursorRay = cursor.CalculateCursorRay(gameCamera.ProjectionMatrix, gameCamera.ViewMatrix);
            BoundingSphere boundingSphiro;
            boundingSphiro = boundingSphere;
            boundingSphiro.Center = center;
            if (CursorManager.RayIntersectsBoundingSphere(cursorRay, boundingSphiro))
                return true;
            return false;
        }

    }
}
