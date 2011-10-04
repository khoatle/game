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
        public static bool InShootingRange(Tank tank, Cursor cursor, Camera gameCamera, float planeHeight)
        {
            Vector3 pointIntersect = IntersectPointWithPlane(cursor, gameCamera, planeHeight);
            Vector3 mouseDif = pointIntersect - tank.Position;
            float distanceFromTank = mouseDif.Length();
            if (distanceFromTank < GameConstants.shootingRange)
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

        public static bool MouseOnEnemy(Cursor cursor, Camera gameCamera, Enemy[] enemies, int enemiesAmount)
        {
            Ray cursorRay = cursor.CalculateCursorRay(gameCamera.ProjectionMatrix, gameCamera.ViewMatrix);
            BoundingSphere sphere;
            for (int i = 0; i < enemiesAmount; i++)
            {
                //making it easier to aim
                sphere = enemies[i].BoundingSphere;
                sphere.Radius *= 2.0f;
                if (RayIntersectsBoundingSphere(cursorRay, sphere))
                {
                    cursor.SetShootingMouseImage();
                    return true;
                }
            }
            cursor.SetNormalMouseImage();
            return false;
        }


        public static bool MouseOnFish(Cursor cursor, Camera gameCamera, Fish[] fish, int fishAmount)
        {
            BoundingSphere sphere;
            Ray cursorRay = cursor.CalculateCursorRay(gameCamera.ProjectionMatrix, gameCamera.ViewMatrix);
            for (int i = 0; i < fishAmount; i++)
            {
                sphere = fish[i].BoundingSphere;
                sphere.Radius *= 2.0f;
                if (RayIntersectsBoundingSphere(cursorRay, fish[i].BoundingSphere))
                {
                    cursor.SetShootingMouseImage();
                    return true;
                }
            }
            cursor.SetNormalMouseImage();
            return false;
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
        public static bool MouseOnShipWreck(Cursor cursor, BoundingSphere boundingSphere, Vector3 center, Camera gameCamera)
        {

            Ray cursorRay = cursor.CalculateCursorRay(gameCamera.ProjectionMatrix, gameCamera.ViewMatrix);
            BoundingSphere boundingSphiro;
            boundingSphiro = boundingSphere;
            boundingSphiro.Center = center;
            if (CursorManager.RayIntersectsBoundingSphere(cursorRay, boundingSphiro))
                return true;
            return false;
        }
        public static bool MouseOnChest(Cursor cursor, BoundingSphere boundingSphere, Vector3 center, Camera gameCamera)
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
