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
        public static void loadContentEnemies(ref int enemiesAmount, BaseEnemy[] enemies, ContentManager Content, int currentLevel, bool mainGame)
        {
            if (mainGame)
                enemiesAmount = GameConstants.NumberShootingEnemies[currentLevel] + GameConstants.NumberCombatEnemies[currentLevel]
                    + GameConstants.NumberMutantShark[currentLevel] + GameConstants.NumberTerminator[currentLevel];
            else enemiesAmount = GameConstants.ShipNumberShootingEnemies + GameConstants.ShipNumberCombatEnemies;
            int numShootingEnemies;
            int numCombatEnemies;
            int numMutantShark = 0;
            int numTerminator = 0;
            if (mainGame)
            {
                numShootingEnemies = GameConstants.NumberShootingEnemies[currentLevel];
                numCombatEnemies = GameConstants.NumberCombatEnemies[currentLevel];
                numMutantShark = GameConstants.NumberMutantShark[currentLevel];
                numTerminator = GameConstants.NumberTerminator[currentLevel];
            }
            else
            {
                numShootingEnemies = GameConstants.ShipNumberShootingEnemies;
                numCombatEnemies = GameConstants.ShipNumberCombatEnemies;
            }
            Random rnd = new Random();
            for (int i = 0; i < enemiesAmount; i++) {
                if (i < numShootingEnemies)
                {
                    enemies[i] = new ShootingEnemy();
                    enemies[i].Name = "Shooting Enemy";
                    enemies[i].LoadContent(Content, "Models/diver");
                    enemies[i].Load(1, 25, 24);
                }
                else if (i < numShootingEnemies + numCombatEnemies){
                    enemies[i] = new CombatEnemy();
                    enemies[i].Name = "Combat Enemy";
                    enemies[i].LoadContent(Content, "Models/diver_knife");
                    enemies[i].Load(1, 30, 24);// 31 60 for attack
                }
                else if (i < numShootingEnemies + numCombatEnemies + numMutantShark)
                {
                    MutantShark mutantShark = new MutantShark();
                    mutantShark.LoadContent(Content, "Models/mutantSharkVer2");
                    mutantShark.Name = "mutant shark";
                    enemies[i] = mutantShark;
                }
                else if (i < numShootingEnemies + numCombatEnemies + numMutantShark + numTerminator)
                {
                    Terminator terminator = new Terminator();
                    terminator.LoadContent(Content, "Models/diver");
                    if (currentLevel == 4) terminator.Name = "???";
                    else terminator.Name = "terminator";
                    terminator.Load();
                    enemies[i] = terminator;
                }
            }
            
            //Terminator terminator = new Terminator();
            //terminator.LoadContent(Content, "Models/squirrel");
            //terminator.Name = "terminator";
            //terminator.Load();
            //enemies[enemiesAmount - 2] = terminator;

            //Terminator hunter = new Terminator();
            //hunter.LoadContent(Content, "Models/diver");
            //hunter.Name = "Whale Hunter";
            //hunter.Load(1, 25, 24);
            //enemies[enemiesAmount - 3] = hunter;
        }

        public static void loadContentFish(ref int fishAmount, Fish[] fish, ContentManager Content, int currentLevel, bool mainGame)
        {
            if (mainGame)
                fishAmount = GameConstants.NumberFish[currentLevel];
            else fishAmount = GameConstants.ShipNumberFish;
            Random random = new Random();
            int type = random.Next(9);
            for (int i = 0; i < fishAmount; i++) {
                fish[i] = new Fish();
                if (type == 0)
                {
                    fish[i].LoadContent(Content, "Models/turtle");
                    fish[i].Load(1, 24, 24);
                    fish[i].Name = "turtle";
                }
                else if (type == 1)
                {
                    fish[i].LoadContent(Content, "Models/dolphin");
                    fish[i].Load(1, 24, 24);
                    fish[i].Name = "dolphin";
                }
                else if (type == 2)
                {
                    fish[i].LoadContent(Content, "Models/normalshark");
                    fish[i].Load(1, 24, 24);
                    fish[i].Name = "shark";
                }
                else if (type == 3)
                {
                    fish[i].LoadContent(Content, "Models/stingray");
                    fish[i].Load(1, 24, 24);
                    fish[i].Name = "sting ray";
                }
                else if (type == 4)
                {
                    fish[i].LoadContent(Content, "Models/orca");
                    fish[i].Load(1, 24, 24);
                    fish[i].Name = "orca";
                }
                else if (type == 5)
                {
                    fish[i].LoadContent(Content, "Models/leopardshark");
                    fish[i].Load(1, 24, 24);
                    fish[i].Name = "leopard shark";
                }
                else if (type == 6)
                {
                    fish[i].LoadContent(Content, "Models/manetee");
                    fish[i].Load(1, 24, 24);
                    fish[i].Name = "manetee";
                }
                else if (type == 7)
                {
                    fish[i].LoadContent(Content, "Models/seal");
                    fish[i].Load(1, 24, 24);
                    fish[i].Name = "seal";
                }
                else if (type == 8)
                {
                    fish[i].LoadContent(Content, "Models/hammershark");
                    fish[i].Load(1, 24, 24);
                    fish[i].Name = "hammer shark";
                }
                //fish[i].LoadContent(Content, "Models/orca");
                //fish[i].Load(1, 24, 24);
                //fish[i].Name = "hammer shark";
                type = random.Next(9);
            }
        }

        public static void placeEnemies(ref int enemiesAmount, BaseEnemy[] enemies, ContentManager Content, Random random, int fishAmount, Fish[] fish, List<ShipWreck> shipWrecks, int minX, int maxX, int minZ, int maxZ, int currentLevel, bool mainGame, float floatHeight)
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

        public static void placeFish(ref int fishAmount, Fish[] fish, ContentManager Content, Random random, int enemiesAmount, BaseEnemy[] enemies, List<ShipWreck> shipWrecks, int minX, int maxX, int minZ, int maxZ, int currentLevel, bool mainGame, float floatHeight)
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
        public static void placeHealingBullet(Tank tank, ContentManager Content, List<HealthBullet> healthBullet) {
            HealthBullet h = new HealthBullet();
            Matrix orientationMatrix = Matrix.CreateRotationY(tank.ForwardDirection);
            Vector3 movement = Vector3.Zero;
            movement.Z = 1;
            Vector3 shootingDirection = Vector3.Transform(movement, orientationMatrix);
 
            h.initialize(tank.Position, shootingDirection, GameConstants.BulletSpeed, Tank.strength, Tank.strengthUp);
            h.loadContent(Content, "Models/healBullet");
            healthBullet.Add(h);
        }

        public static void placeTankDamageBullet(Tank tank, ContentManager Content, List<DamageBullet> myBullet) {
            DamageBullet d = new DamageBullet();

            Matrix orientationMatrix = Matrix.CreateRotationY(tank.ForwardDirection);
            Vector3 movement = Vector3.Zero;
            movement.Z = 1;
            Vector3 shootingDirection = Vector3.Transform(movement, orientationMatrix);
            
            d.initialize(tank.Position, shootingDirection, GameConstants.BulletSpeed, Tank.strength, Tank.strengthUp);
            d.loadContent(Content, "Models/damageBullet");
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
            {
                newBullet.loadContent(PlayGameScene.Content, "Models/bossBullet");
            }
            else newBullet.loadContent(PlayGameScene.Content, "Models/normalbullet");
            bullets.Add(newBullet);         
        }

        public static void placeChasingBullet(GameObject shooter, GameObject target, int damage, List<DamageBullet> bullets) {
            Tank tmp1;
            SwimmingObject tmp2;
            Matrix orientationMatrix;
            if (shooter.GetType().Name.Equals("Tank")) {
                tmp1 = (Tank)shooter;
                orientationMatrix = Matrix.CreateRotationY(tmp1.ForwardDirection);
            } else {
                tmp2 = (SwimmingObject)shooter;
                orientationMatrix = Matrix.CreateRotationY(tmp2.ForwardDirection);
            }

            ChasingBullet newBullet = new ChasingBullet();

            Vector3 movement = Vector3.Zero;
            movement.Z = 1;
            Vector3 shootingDirection = Vector3.Transform(movement, orientationMatrix);
            newBullet.initialize(shooter.Position, shootingDirection, GameConstants.BulletSpeed, damage, target);
            newBullet.loadContent(PlayGameScene.Content, "Models/chasingBullet");
            bullets.Add(newBullet);
        }

        public static bool placePlant(Tank tank, HeightMapInfo heightMapInfo, ContentManager Content, TimeSpan roundTimer, List<Plant> plants, List<ShipWreck> shipWrecks, List<StaticObject> staticObjects, GameTime gameTime)
        {
            if ((gameTime.TotalGameTime.TotalSeconds - Tank.prevPlantTime > GameConstants.coolDownForPlant) || Tank.firstPlant == true)
            {
                Plant p = new Plant();
                Vector3 possiblePosition = tank.Position;
                possiblePosition.Y = heightMapInfo.GetHeight(tank.Position);
                p.LoadContent(Content, possiblePosition, roundTimer.TotalSeconds);
                if (Collision.isPlantPositionValid(p, plants, shipWrecks, staticObjects))
                {
                    plants.Add(p);
                    Tank.firstPlant = false;
                    Tank.prevPlantTime = gameTime.TotalGameTime.TotalSeconds;
                    return true;
                }
            }
            return false;
        }

        public static void placeTrash(
            List<Trash> trashes,  ContentManager Content, Random random, List<ShipWreck> shipWrecks, int minX, int maxX, int minZ, int maxZ, int currentLevel, bool mainGame, float floatHeight, HeightMapInfo heightMapInfo)
        {
            Vector3 tempCenter;
            foreach (Trash trash in trashes)
            {
                //trash.Position = GenerateSurfaceRandomPosition(minX, maxX, minZ, maxZ, random, enemiesAmount, fishAmount, enemies,
                //    fish, shipWrecks);
                trash.Position.X= random.Next(minX, maxX);
                trash.Position.Z = random.Next(minZ, maxZ);
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

            //System.Diagnostics.Debug.WriteLine(currentHealth+","+maxHealth);
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
            Color typeColor = Color.Red;
            int barWidth = Bar.Width/2;
            double healthiness = (double)currentEnvironment / maxEnvironemnt;
            //System.Diagnostics.Debug.WriteLine(currentHealth+","+maxHealth);
            //Draw the negative space for the health bar
            spriteBatch.Draw(Bar,
                new Rectangle(barX, barY, barWidth, Bar.Height),
                new Rectangle(barWidth+1, 0, barWidth, Bar.Height),
                Color.Transparent);
            //Draw the current health level based on the current Health
            Color healthColor = Color.LawnGreen;
            if (healthiness < 0.2)
                healthColor = Color.Red;
            else if (healthiness < 0.5)
                healthColor = Color.Green;
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
            //System.Diagnostics.Debug.WriteLine(currentHealth+","+maxHealth);
            //Draw the negative space for the health bar
            spriteBatch.Draw(LevelBar,
                new Rectangle(barX, barY, LevelBar.Width, barHeight),
                new Rectangle(0, barHeight + 1, LevelBar.Width, barHeight),
                Color.Transparent);
            //Draw the current health level based on the current Health
            spriteBatch.Draw(LevelBar,
                new Rectangle(barX, barY, (int)(LevelBar.Width * experience), barHeight),
                new Rectangle(0, barHeight + 1, LevelBar.Width, barHeight),
                Color.DarkSlateBlue);
            //Draw the box around the health bar
            spriteBatch.Draw(LevelBar,
                new Rectangle(barX, barY, LevelBar.Width, barHeight),
                new Rectangle(0, 0, LevelBar.Width, barHeight),
                Color.White);
            spriteBatch.DrawString(statsFont, type.ToUpper(), new Vector2(game.Window.ClientBounds.Width / 2 - ((type.Length / 2) * 11), heightFromTop - 1), typeColor);
        }


    }
}
