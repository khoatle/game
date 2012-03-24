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
            if (enemies == null) return null;
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
            if (fish == null) return null;
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

        public static void MouseOnWhichTrash(Cursor cursor, Camera gameCamera, List<Trash> trashes,ref Trash cursorOnTrash,ref Trash botOnTrash, HydroBot hydroBot)
        {
            bool foundBotOnTrash = false, foundCursorOnTrash = false;
            if (hydroBot == null) foundBotOnTrash = true;
            BoundingSphere botTrashBoundingSphere = new BoundingSphere();
            if (!foundBotOnTrash)
                botTrashBoundingSphere = new BoundingSphere(hydroBot.BoundingSphere.Center, 20);
            if (trashes == null) return;
            BoundingSphere trashRealSphere;
            Ray cursorRay = cursor.CalculateCursorRay(gameCamera.ProjectionMatrix, gameCamera.ViewMatrix);
            foreach (Trash trash in trashes)
            {
                trashRealSphere = trash.BoundingSphere;
                trashRealSphere.Center.Y = trash.Position.Y;
                trashRealSphere.Radius *= 5;
                if (!foundBotOnTrash)
                {
                    if (trash.BoundingSphere.Intersects(botTrashBoundingSphere))
                    {
                        foundBotOnTrash = true;
                        botOnTrash = trash;
                    }
                }
                if (!foundCursorOnTrash)
                {
                    if (RayIntersectsBoundingSphere(cursorRay, trashRealSphere))
                    {
                        foundCursorOnTrash = true;
                        cursorOnTrash = trash;
                    }
                }
                if (foundBotOnTrash && foundCursorOnTrash) return;
            }
        }

        public static void MouseOnWhichPowerPack(Cursor cursor, Camera gameCamera, List<Powerpack> powerPacks, ref Powerpack cursorOnPowerPack, ref Powerpack botOnPowerPack, HydroBot hydroBot)
        {
            bool foundBotOnPowerpack = false, foundCursorOnPowerpack = false;
            if (hydroBot == null) foundBotOnPowerpack = true;
            BoundingSphere botPowerPackBoundingSphere = new BoundingSphere();
            if (!foundBotOnPowerpack)
                botPowerPackBoundingSphere = new BoundingSphere(hydroBot.BoundingSphere.Center, 5);
            if (powerPacks == null) return;
            Ray cursorRay = cursor.CalculateCursorRay(gameCamera.ProjectionMatrix, gameCamera.ViewMatrix);
            foreach (Powerpack powerPack in powerPacks)
            {
                if (!foundBotOnPowerpack)
                {
                    if (powerPack.BoundingSphere.Intersects(botPowerPackBoundingSphere))
                    {
                        foundBotOnPowerpack = true;
                        botOnPowerPack = powerPack;
                    }
                }
                if (!foundCursorOnPowerpack)
                {
                    if (RayIntersectsBoundingSphere(cursorRay, powerPack.BoundingSphere))
                    {
                        foundCursorOnPowerpack = true;
                        cursorOnPowerPack = powerPack;
                    }
                }
                if (foundBotOnPowerpack && foundCursorOnPowerpack) return;
            }
        }

        public static void MouseOnWhichResource(Cursor cursor, Camera gameCamera, List<Resource> resources, ref Resource cursorOnResource, ref Resource botOnResource, HydroBot hydroBot)
        {
            bool foundBotOnResource = false, foundCursorOnResource = false;
            if (hydroBot == null) foundBotOnResource = true;
            BoundingSphere botPowerPackBoundingSphere = new BoundingSphere();
            if (!foundBotOnResource)
                botPowerPackBoundingSphere = new BoundingSphere(hydroBot.BoundingSphere.Center, 5);
            if (resources == null) return;
            Ray cursorRay = cursor.CalculateCursorRay(gameCamera.ProjectionMatrix, gameCamera.ViewMatrix);
            foreach (Resource resource in resources)
            {
                if (!foundBotOnResource)
                {
                    if (resource.BoundingSphere.Intersects(botPowerPackBoundingSphere))
                    {
                        foundBotOnResource = true;
                        botOnResource = resource;
                    }
                }
                if (!foundCursorOnResource)
                {
                    if (RayIntersectsBoundingSphere(cursorRay, resource.BoundingSphere))
                    {
                        foundCursorOnResource = true;
                        cursorOnResource = resource;
                    }
                }
                if (foundBotOnResource && foundCursorOnResource) return;
            }
        }


        public static ShipWreck MouseOnWhichShipWreck(Cursor cursor, Camera gameCamera, List<ShipWreck> shipWrecks)
        {
            if (shipWrecks == null) return null;
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
            if (factories == null) return null;
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

        public static TreasureChest MouseOnWhichChest(Cursor cursor, Camera gameCamera, List<TreasureChest> treasureChests)
        {
            if (treasureChests == null) return null;
            BoundingSphere treasureChestRealSphere;
            Ray cursorRay = cursor.CalculateCursorRay(gameCamera.ProjectionMatrix, gameCamera.ViewMatrix);
            foreach (TreasureChest treasureChest in treasureChests)
            {
                treasureChestRealSphere = treasureChest.BoundingSphere;
                treasureChestRealSphere.Center.Y = treasureChest.Position.Y;
                treasureChestRealSphere.Radius *= 1;
                if (RayIntersectsBoundingSphere(cursorRay, treasureChestRealSphere))
                {
                    return treasureChest;
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

        public static void CheckClick(ref MouseState lastMouseState, ref MouseState currentMouseState, GameTime gameTime, ref double clickTimer, ref bool clicked, ref bool doubleClicked, ref bool notYetReleased)
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
            else if (currentMouseState.LeftButton == ButtonState.Pressed)
                notYetReleased = true;
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

        public static void MouseInteractWithControlPanel(ref bool clicked, ref bool doubleClicked, ref bool notYetRealeased, ref MouseState lastMouseState, ref MouseState currentMouseState,
            GameTime gameTime, ref double clickTimer, bool openFactoryConfigurationScene, Factory factoryToConfigure, ResearchFacility researchFacility, List<Factory> factories)
        {
            clicked = false;
            notYetRealeased = false;
            CheckClick(ref lastMouseState, ref currentMouseState, gameTime, ref clickTimer, ref clicked, ref doubleClicked, ref notYetRealeased);
            if (openFactoryConfigurationScene)
            {
                factoryToConfigure.produceButtonHover = factoryToConfigure.produceRect.Contains(lastMouseState.X, lastMouseState.Y);

                if (notYetRealeased && factoryToConfigure.produceButtonHover)
                {
                    factoryToConfigure.produceButtonPress = true;
                }
                else factoryToConfigure.produceButtonPress = false;
            }
            else
            {
                researchFacility.upgradeBotButtonHover = (HydroBot.unassignedPts > 0 && researchFacility.increaseAttributeRect.Contains(lastMouseState.X, lastMouseState.Y));
                if (notYetRealeased && researchFacility.upgradeBotButtonHover)
                {
                    researchFacility.upgradeBotButtonPressed = true;
                }
                else  researchFacility.upgradeBotButtonPressed = false;
                researchFacility.upgradeBioFacButtonHover = (researchFacility.bioUpgrade && researchFacility.bioUpgradeRect.Contains(lastMouseState.X, lastMouseState.Y));
                if (notYetRealeased && researchFacility.upgradeBioFacButtonHover)
                {
                    researchFacility.upgradeBioFacButtonPressed = true;
                }
                else researchFacility.upgradeBioFacButtonPressed = false;
                researchFacility.upgradePlasFacButtonHover = (researchFacility.plasticUpgrade && researchFacility.plasticUpgradeRect.Contains(lastMouseState.X, lastMouseState.Y));
                if (notYetRealeased && researchFacility.upgradePlasFacButtonHover)
                {
                    researchFacility.upgradePlasFacButtonPressed = true;
                }
                else researchFacility.upgradePlasFacButtonPressed = false;
                researchFacility.seacowButtonHover = (ResearchFacility.playSeaCowJigsaw && researchFacility.playSeaCowJigsawRect.Contains(lastMouseState.X, lastMouseState.Y));
                if (notYetRealeased && researchFacility.seacowButtonHover)
                {
                    researchFacility.seacowBotButtonPressed = true;
                }
                else researchFacility.seacowBotButtonPressed = false;
                researchFacility.dolphinButtonHover = (ResearchFacility.playSeaCowJigsaw && researchFacility.playDolphinJigsawRect.Contains(lastMouseState.X, lastMouseState.Y));
                if (notYetRealeased && researchFacility.dolphinButtonHover)
                {
                    researchFacility.dolphinButtonPressed = true;
                }
                else researchFacility.dolphinButtonPressed = false;
                researchFacility.turtleButtonHover = (ResearchFacility.playSeaCowJigsaw && researchFacility.playTurtleJigsawRect.Contains(lastMouseState.X, lastMouseState.Y));
                if (notYetRealeased && researchFacility.turtleButtonHover)
                {
                    researchFacility.turtleButtonPressed = true;
                }
                else researchFacility.turtleButtonPressed = false;
            }
            if (clicked)
            {
                if (openFactoryConfigurationScene)
                {
                    if (factoryToConfigure.produceButtonHover)
                    {
                        factoryToConfigure.SwitchProductionItem();
                        PoseidonGame.audio.MenuScroll.Play();
                    }
                }
                else
                {
                    if (researchFacility.bioUpgrade && researchFacility.bioUpgradeRect.Contains(lastMouseState.X, lastMouseState.Y))
                    {
                        researchFacility.UpgradeBioFactory(factories);
                        PoseidonGame.audio.MenuScroll.Play();
                    }
                    if (researchFacility.plasticUpgrade && researchFacility.plasticUpgradeRect.Contains(lastMouseState.X, lastMouseState.Y))
                    {
                        researchFacility.UpgradePlasticFactory(factories);
                        PoseidonGame.audio.MenuScroll.Play();
                    }
                    if (ResearchFacility.playSeaCowJigsaw && researchFacility.playSeaCowJigsawRect.Contains(lastMouseState.X, lastMouseState.Y))
                    {
                        PoseidonGame.playJigsaw = true;
                        PoseidonGame.jigsawType = 0; //seacow
                        PoseidonGame.audio.MenuScroll.Play();
                    }
                    if (ResearchFacility.playTurtleJigsaw && researchFacility.playTurtleJigsawRect.Contains(lastMouseState.X, lastMouseState.Y))
                    {
                        PoseidonGame.playJigsaw = true;
                        PoseidonGame.jigsawType = 1; //turtle
                        PoseidonGame.audio.MenuScroll.Play();
                    }
                    if (ResearchFacility.playDolphinJigsaw && researchFacility.playDolphinJigsawRect.Contains(lastMouseState.X, lastMouseState.Y))
                    {
                        PoseidonGame.playJigsaw = true;
                        PoseidonGame.jigsawType = 2; //dolphin
                        PoseidonGame.audio.MenuScroll.Play();
                    }
                    if (HydroBot.unassignedPts > 0 && researchFacility.increaseAttributeRect.Contains(lastMouseState.X, lastMouseState.Y))
                    {
                        PoseidonGame.AttributeButtonPressed = true;
                        PoseidonGame.audio.MenuScroll.Play();
                    }
                }
                clicked = false;
            }
        }
    }
}
