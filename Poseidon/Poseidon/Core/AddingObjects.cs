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
        public static void loadContentEnemies(ref int enemiesAmount, BaseEnemy[] enemies, ContentManager Content, int currentLevel, GameMode gameMode)
        {

            if (gameMode == GameMode.SurvivalMode)
            {
                enemiesAmount = GameConstants.SurvivalModeMaxShootingEnemy + GameConstants.SurvivalModeMaxCombatEnemy + GameConstants.SurvivalModeMaxGhostPirate
                + GameConstants.SurvivalModeMaxMutantShark + GameConstants.SurvivalModeMaxTerminator + GameConstants.SurvivalModeMaxSubmarine;
            }
            else if (gameMode == GameMode.MainGame)
                enemiesAmount = GameConstants.NumberShootingEnemies[currentLevel] + GameConstants.NumberCombatEnemies[currentLevel] + GameConstants.NumberGhostPirate[currentLevel]
                    + GameConstants.NumberMutantShark[currentLevel] + GameConstants.NumberTerminator[currentLevel] + GameConstants.NumberSubmarine[currentLevel];
            else if (gameMode == GameMode.ShipWreck)
            {
                enemiesAmount = GameConstants.ShipNumberGhostPirate[PlayGameScene.currentLevel];
            }
            int numShootingEnemies = 0;
            int numCombatEnemies = 0;
            int numGhostPirates = 0;
            int numMutantShark = 0;
            int numTerminator = 0;
            int numSubmarine = 0;
        
            if (gameMode == GameMode.SurvivalMode)
            {
                numShootingEnemies = GameConstants.SurvivalModeMaxShootingEnemy;
                numCombatEnemies = GameConstants.SurvivalModeMaxCombatEnemy;
                numGhostPirates = GameConstants.SurvivalModeMaxGhostPirate;
                numMutantShark = GameConstants.SurvivalModeMaxMutantShark;
                numTerminator = GameConstants.SurvivalModeMaxTerminator;
                numSubmarine = GameConstants.SurvivalModeMaxSubmarine;
            }
            else if (gameMode == GameMode.MainGame)
            {
                numShootingEnemies = GameConstants.NumberShootingEnemies[currentLevel];
                numCombatEnemies = GameConstants.NumberCombatEnemies[currentLevel];
                numGhostPirates = GameConstants.NumberGhostPirate[currentLevel];
                numMutantShark = GameConstants.NumberMutantShark[currentLevel];
                numTerminator = GameConstants.NumberTerminator[currentLevel];
                numSubmarine = GameConstants.NumberSubmarine[currentLevel];
            }
            else if (gameMode == GameMode.ShipWreck)
            {
                numGhostPirates = GameConstants.ShipNumberGhostPirate[PlayGameScene.currentLevel];
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
                else if (i < numShootingEnemies + numCombatEnemies + numGhostPirates)
                {
                    enemies[i] = new GhostPirate();
                    enemies[i].Name = "Ghost Pirate";
                    enemies[i].LoadContent(Content, "Models/EnemyModels/skeletonrigged"); 
                    enemies[i].Load(10, 40, 24);
                }
                else if (i < numShootingEnemies + numCombatEnemies + numGhostPirates + numMutantShark)
                {
                    MutantShark mutantShark = new MutantShark();
                    mutantShark.LoadContent(Content, "Models/EnemyModels/mutantSharkVer2");
                    mutantShark.Name = "mutant shark";
                    enemies[i] = mutantShark;
                }
                else if (i < numShootingEnemies + numCombatEnemies + numGhostPirates + numMutantShark + numTerminator)
                {
                    Terminator terminator = new Terminator(gameMode);
                    terminator.LoadContent(Content, "Models/EnemyModels/terminator");
                    if (currentLevel == 4 && gameMode == GameMode.MainGame) terminator.Name = "???";
                    else terminator.Name = "terminator";
                    terminator.Load(31, 60, 24);
                    enemies[i] = terminator;
                }
                else if (i < numShootingEnemies + numCombatEnemies + numGhostPirates + numMutantShark + numTerminator + numSubmarine)
                {
                    Submarine submarine = new Submarine(gameMode);
                    submarine.LoadContent(Content, "Models/EnemyModels/submarine");
                    submarine.Name = "Shark Submarine";
                    submarine.Load(21, 30, 24);
                    enemies[i] = submarine;
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
                //type = 2;
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
                    fish[i].sad_talk = "I need to go to the beach to lay eggs. Can you ask the humans not to hunt me?";  
                }
                else if (type == 1)
                {
                    fish[i].LoadContent(Content, "Models/SeaAnimalModels/dolphinVer3");
                    fish[i].Name += "dolphin";
                    fish[i].Load(1, 24, 24);
                    fish[i].happy_talk = "We remind you to play, play, play, for you will find great power in play.";
                    fish[i].sad_talk = "Though we try to be friends with humans, they always hurt us with their pollution, propellers and what not!";
                }
                else if (type == 2)
                {
                    fish[i].LoadContent(Content, "Models/SeaAnimalModels/maneteeVer2");
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
                    fish[i].LoadContent(Content, "Models/SeaAnimalModels/orcaVer2");
                    fish[i].Name += "orca";
                    fish[i].Load(1, 24, 24);
                    fish[i].happy_talk = "Move away, you little bot, here comes the killer whale.";
                    fish[i].sad_talk = "I lost my way. I can't hear my friends due to the noise from the oil-rig.";
                }
                else if (type == 5)
                {

                    fish[i].LoadContent(Content, "Models/SeaAnimalModels/sealVer2");
                    fish[i].Name += "seal";
                    fish[i].Load(1, 24, 24);
                    fish[i].happy_talk = "See how I swim, with a swerve and a twist, a flip of the flipper, a flick of the wrist!";
                    fish[i].sad_talk = "We need the arctic ice. Stop global warming.";
                }
                else if (type == 6)
                {
                    fish[i].LoadContent(Content, "Models/SeaAnimalModels/normalsharkVer3");
                    fish[i].Name += "shark";
                    fish[i].Load(1, 24, 24);
                    fish[i].happy_talk = "You stink like a rusty metal. I can smell it. I also hear a prey far away. I'll go 15mph this time.";
                    fish[i].sad_talk = "Humans kill over 30 million sharks every year. We are the oldest fish, spare us.";
                }
                else if (type == 7)
                {
                    fish[i].LoadContent(Content, "Models/SeaAnimalModels/leopardsharkVer3");
                    fish[i].Name += "leopard shark";
                    fish[i].Load(1, 24, 24);
                    fish[i].happy_talk = "No, I am not racist and I date all kinds of shark, not you, dear bot.";
                    fish[i].sad_talk = "We never eat humans. Why do they hurt us?";
                }
                else if (type == 8)
                {
                    fish[i].LoadContent(Content, "Models/SeaAnimalModels/hammersharkVer2");
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

        public static void placeMinion(ContentManager Content, int type, BaseEnemy[] enemies, int enemiesAmount, Fish[] fish, ref int fishAmount, HydroBot bot) {
            
            
            Fish newFish;
            if (type == 0)
            {
                newFish = new SeaCow();
                newFish.Name = "Steller's Sea Cow";
                newFish.LoadContent(Content, "Models/SeaAnimalModels/stellarSeaCow");
                newFish.Load(1, 24, 24);
                newFish.happy_talk = "I am much larger than manetee or dugong.";
                newFish.sad_talk = "I was too slow to escape the hunters.";
            }
            else if (type == 1)
            {
                newFish = new SeaTurtle();
                newFish.Name = "Meiolania";
                newFish.LoadContent(Content, "Models/SeaAnimalModels/MeiolaniaWithAnim");
                newFish.Load(1, 24, 24);
                newFish.happy_talk = "Huge, hard shell, armored head and spiked tail ... anything else about me?";
                newFish.sad_talk = "I actually ... have never swum before ^^!";
            }
            else {
                newFish = new SeaDolphin();
                newFish.Name = "Maui's Dolphin";
                newFish.LoadContent(Content, "Models/SeaAnimalModels/mauiDolphin");
                newFish.Load(1, 24, 24);
                newFish.happy_talk = "I am the world's rarest and smallest known species of dolphin!";
                newFish.sad_talk = "If only human did not fish us."; 
            }
            newFish.Position = newFish.BoundingSphere.Center = calculatePlacingPosition(newFish.BoundingSphere.Radius, bot, enemies, enemiesAmount, fish, fishAmount);
            fish[fishAmount] = newFish;
            fishAmount++;
        }

        public static Vector3 calculatePlacingPosition(float radius, HydroBot bot, BaseEnemy[] enemies, int enemiesAmount, Fish[] fish, int fishAmount) {
            Random random = new Random();
            BoundingSphere newSphere = new BoundingSphere(new Vector3(), radius);
            float X, Y = bot.Position.Y, Z;
            do {
                X = (float)random.NextDouble() * (2 * bot.Position.X + 50) - bot.Position.X - 25f;
                //X = bot.Position.X + (float)random.NextDouble() * 50f;
                Z = (float)random.NextDouble() * (2 * bot.Position.Z + 50) - bot.Position.Z - 25f;
                newSphere.Center = new Vector3(X, Y, Z);
            } while (IsSurfaceOccupied(newSphere, enemiesAmount, fishAmount, enemies, fish) || newSphere.Intersects(bot.BoundingSphere));

            return new Vector3(X, Y, Z);
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
                enemies[i].Position = GenerateSurfaceRandomPosition(minX, maxX, minZ, maxZ, floatHeight, enemies[i].BoundingSphere.Radius, random, enemiesAmount, fishAmount, enemies, fish, shipWrecks);
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
                    fish[i].Position = GenerateSurfaceRandomPosition(0, minX, 0, minZ, floatHeight, fish[i].BoundingSphere.Radius, random, enemiesAmount, fishAmount, enemies, fish, shipWrecks);
                else fish[i].Position = GenerateSurfaceRandomPosition(minX, maxX, minZ, maxZ, floatHeight, fish[i].BoundingSphere.Radius, random, enemiesAmount, fishAmount, enemies, fish, shipWrecks);
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

        public static void placeTreasureChests(List<TreasureChest> treasureChests, List<StaticObject> staticObjects, Random random, int minX, int maxX, int minZ, int maxZ)
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
        public static void placeHealingBullet(HydroBot hydroBot, ContentManager Content, List<HealthBullet> healthBullet, GameMode gameMode) {
            HealthBullet h = new HealthBullet();
            Matrix orientationMatrix = Matrix.CreateRotationY(hydroBot.ForwardDirection);
            Vector3 movement = Vector3.Zero;
            movement.Z = 1;
            Vector3 shootingDirection = Vector3.Transform(movement, orientationMatrix);

            h.initialize(hydroBot.Position + shootingDirection * 7, shootingDirection, GameConstants.BulletSpeed, HydroBot.strength, HydroBot.strengthUp, gameMode);
            h.loadContent(Content, "Models/BulletModels/healBullet");
            PoseidonGame.audio.botNormalShot.Play();
            healthBullet.Add(h);
        }

        public static void placeBotDamageBullet(HydroBot hydroBot, ContentManager Content, List<DamageBullet> myBullet, GameMode gameMode) {
            DamageBullet d = new DamageBullet();

            Matrix orientationMatrix = Matrix.CreateRotationY(hydroBot.ForwardDirection);
            Vector3 movement = Vector3.Zero;
            movement.Z = 1;
            Vector3 shootingDirection = Vector3.Transform(movement, orientationMatrix);

            d.initialize(hydroBot.Position + shootingDirection * 7, shootingDirection, GameConstants.BulletSpeed, HydroBot.strength, HydroBot.strengthUp, gameMode);
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
            newBullet.initialize(shooter.Position, shootingDirection, GameConstants.BulletSpeed, GameConstants.ChasingBulletDamage, target, (Terminator)shooter);
            newBullet.loadContent(PoseidonGame.contentManager, "Models/BulletModels/chasingBullet");
            bullets.Add(newBullet);
            if (shooter.BoundingSphere.Intersects(cameraFrustum)) {
                PoseidonGame.audio.chasingBulletSound.Play();
            }
        }

        public static void placeTorpedo(GameObject shooter, GameObject target, List<DamageBullet> bullets, BoundingFrustum cameraFrustum, GameMode gameMode)
        {

            Matrix orientationMatrix = Matrix.CreateRotationY(((Submarine)shooter).ForwardDirection);
            Vector3 movement = Vector3.Zero;
            movement.Z = 1;
            Vector3 shootingDirection = Vector3.Transform(movement, orientationMatrix);

            //one topedo on the left and one on the right
            Torpedo newBullet = new Torpedo();
            newBullet.initialize(shooter.Position - PerpendicularVector(shootingDirection) * 10 + shootingDirection * 0, shootingDirection, GameConstants.BulletSpeed, GameConstants.TorpedoDamage, target, (Submarine)shooter, gameMode);
            newBullet.loadContent(PoseidonGame.contentManager, "Models/BulletModels/torpedo");
            bullets.Add(newBullet);

            Torpedo newBullet1 = new Torpedo();
            newBullet1.initialize(shooter.Position + PerpendicularVector(shootingDirection) * 10 + shootingDirection * 0, shootingDirection, GameConstants.BulletSpeed, GameConstants.TorpedoDamage, target, (Submarine)shooter, gameMode);
            newBullet1.loadContent(PoseidonGame.contentManager, "Models/BulletModels/torpedo");
            bullets.Add(newBullet1);
            if (shooter.BoundingSphere.Intersects(cameraFrustum))
            {
                PoseidonGame.audio.bossShot.Play();
            }
        }

        public static void placeLaser(GameObject shooter, GameObject target, List<DamageBullet> bullets, BoundingFrustum cameraFrustum, GameMode gameMode)
        {

            Matrix orientationMatrix = Matrix.CreateRotationY(((Submarine)shooter).ForwardDirection);
            Vector3 movement = Vector3.Zero;
            movement.Z = 1;
            Vector3 shootingDirection = Vector3.Transform(movement, orientationMatrix);

            //one topedo on the left and one on the right
            LaserBeam newBullet = new LaserBeam();
            newBullet.initialize(shooter.Position + shootingDirection * 5, shootingDirection, GameConstants.BulletSpeed, GameConstants.LaserBeamDamage, target, (Submarine)shooter, gameMode);
            newBullet.loadContent(PoseidonGame.contentManager, "Models/BulletModels/torpedo");
            bullets.Add(newBullet);

            if (shooter.BoundingSphere.Intersects(cameraFrustum))
            {
                PoseidonGame.audio.bossShot.Play();
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
            newBullet.initialize(obj.Position + shootingDirection * offsetFactor, shootingDirection, GameConstants.BulletSpeed, damage, (BaseEnemy)obj);
            if (type == 1)
            {
                newBullet.loadContent(PoseidonGame.contentManager, "Models/BulletModels/bossBullet");
                if (obj.BoundingSphere.Intersects(cameraFrustum))
                    PoseidonGame.audio.bossShot.Play();
            }
            else if (type == 0)
            {
                newBullet.loadContent(PoseidonGame.contentManager, "Models/BulletModels/normalbullet");
                if (obj.BoundingSphere.Intersects(cameraFrustum))
                    PoseidonGame.audio.enemyShot.Play();
            }
            bullets.Add(newBullet);
        }

        public static void placeTrash(
            ref List<Trash> trashes,  ContentManager Content, Random random, List<ShipWreck> shipWrecks, List<StaticObject> staticObjects, int minX, int maxX, int minZ, int maxZ, GameMode gameMode, float floatHeight, HeightMapInfo heightMapInfo)
        {
            Vector3 tempCenter;
            int xVal, zVal, heightValue; //positionSign
            foreach (Trash trash in trashes)
            {
                //trash.Position = GenerateSurfaceRandomPosition(minX, maxX, minZ, maxZ, random, enemiesAmount, fishAmount, enemies,
                //    fish, shipWrecks);
                do
                {
                    xVal = random.Next(minX, maxX);
                    zVal = random.Next(minZ, maxZ);
                    if (random.Next(100) % 2 == 0)
                        xVal *= -1;
                    if (random.Next(100) % 2 == 0)
                        zVal *= -1;
                    heightValue = (int)heightMapInfo.GetHeight(new Vector3(xVal, 0, zVal));
                    //positionSign = random.Next(4);
                    
                    //switch (positionSign)
                    //{
                    //    case 0:
                    //        xVal *= -1;
                    //        break;
                    //    case 1:
                    //        zVal *= -1;
                    //        break;
                    //    case 2:
                    //        xVal *= -1;
                    //        zVal *= -1;
                    //        break;
                    //}
                } while (IsSeaBedPlaceOccupied(xVal, 0, zVal, 30, shipWrecks, staticObjects, trashes, null, null) ); //no need to check with factories as this funciton is called only at the start of the game when factories are not present.

                trash.Position.X = xVal;
                trash.Position.Z = zVal;
                trash.Position.Y = trash.seaFloorHeight = heightValue;
                tempCenter = trash.BoundingSphere.Center;
                tempCenter.X = trash.Position.X;
                tempCenter.Y = floatHeight;
                tempCenter.Z = trash.Position.Z;
                trash.BoundingSphere = new BoundingSphere(tempCenter,
                    trash.BoundingSphere.Radius);
            }
        }

        public static Vector3 createSinkingTrash(
            ref List<Trash> trashes, ContentManager Content, Random random, List<ShipWreck> shipWrecks, List<StaticObject> staticObjects, List<Factory> factories, ResearchFacility researchFacility, int minX, int maxX, int minZ, int maxZ, float floatHeight, HeightMapInfo heightMapInfo,ref Model bioTrash,ref Model plasticTrash,ref Model nukeTrash, ParticleManagement particleManager)
        {
            Vector3 tempCenter;
            int positionSign, xVal, zVal, heightValue;
            float orientation = random.Next(100);
            int trash_type = random.Next(100);
            Trash sinkingTrash;
            if (trash_type < 48)
            {
                sinkingTrash = new Trash(TrashType.biodegradable, particleManager);
                sinkingTrash.Load(Content,ref bioTrash, orientation);
                sinkingTrash.sinkingRate = 0.25f;
                sinkingTrash.sinkingRotationRate = 0.015f;
            }
            else if (trash_type < 96)
            {
                sinkingTrash = new Trash(TrashType.plastic, particleManager);
                sinkingTrash.Load(Content,ref plasticTrash, orientation); //nuclear model
                sinkingTrash.sinkingRate = 0.35f;
                sinkingTrash.sinkingRotationRate = -0.015f;
            }
            else
            {
                sinkingTrash = new Trash(TrashType.radioactive, particleManager);
                sinkingTrash.Load(Content,ref nukeTrash, orientation); //nuclear model
                sinkingTrash.sinkingRate = 0.6f;
                sinkingTrash.sinkingRotationRate = 0.025f;
            }
            sinkingTrash.sinking = true;
            sinkingTrash.sinkableTrash = true;
            do
            {
                positionSign = random.Next(4);
                xVal = random.Next(minX, maxX);
                zVal = random.Next(minZ, maxZ);
                switch (positionSign)
                {
                    case 0:
                        xVal *= -1;
                        break;
                    case 1:
                        zVal *= -1;
                        break;
                    case 2:
                        xVal *= -1;
                        zVal *= -1;
                        break;
                }
                heightValue = (int)heightMapInfo.GetHeight(new Vector3(xVal, 0, zVal));
            } while (IsSeaBedPlaceOccupied(xVal, 0, zVal, 30, shipWrecks, staticObjects, trashes, factories, researchFacility));

            sinkingTrash.Position.X = xVal;
            sinkingTrash.Position.Z = zVal;
            sinkingTrash.Position.Y = floatHeight+100;
            sinkingTrash.seaFloorHeight = heightMapInfo.GetHeight(new Vector3(sinkingTrash.Position.X, 0, sinkingTrash.Position.Z));//GameConstants.TrashFloatHeight;
            tempCenter = sinkingTrash.BoundingSphere.Center;
            tempCenter.X = sinkingTrash.Position.X;
            tempCenter.Y = floatHeight;
            tempCenter.Z = sinkingTrash.Position.Z;
            sinkingTrash.BoundingSphere = new BoundingSphere(tempCenter,sinkingTrash.BoundingSphere.Radius);
            trashes.Add(sinkingTrash);
            return sinkingTrash.Position;
        }

        // Helper
        public static Vector3 GenerateSurfaceRandomPosition(int minX, int maxX, int minZ, int maxZ, float floatHeight, float boundingSphereRadius, Random random, int enemiesAmount, int fishAmount, BaseEnemy[] enemies, Fish[] fish, List<ShipWreck> shipWrecks)
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

            } while (IsSurfaceOccupied(new BoundingSphere(new Vector3(xValue, floatHeight, zValue), boundingSphereRadius), enemiesAmount, fishAmount, enemies, fish));

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
                
                
            } while (IsSeaBedPlaceOccupied(xValue,0, zValue, 30, shipWrecks, staticObjects, null, null, null));
            
            return new Vector3(xValue, 0, zValue);
        }
        // Helper
        public static bool IsSurfaceOccupied(BoundingSphere prospectiveBoundingSphere, int enemiesAmount, int fishAmount, BaseEnemy[] enemies, Fish[] fish)
        {
            //int optimalDistance;
            for (int i = 0; i < enemiesAmount; i++)
            {
                ////give more space for big guys
                //if (enemies[i] is MutantShark) optimalDistance = 70;
                //else optimalDistance = 30;
                //if (((int)(MathHelper.Distance(
                //    xValue, enemies[i].Position.X)) < optimalDistance) &&
                //    ((int)(MathHelper.Distance(
                //    zValue, enemies[i].Position.Z)) < optimalDistance))
                //    return true;
                
                if (prospectiveBoundingSphere.Intersects(enemies[i].BoundingSphere)) return true;
            }

            for (int i = 0; i < fishAmount; i++)
            {
                //if (fish[i].isBigBoss) optimalDistance = 70;
                //else optimalDistance = 30;
                //if (((int)(MathHelper.Distance(
                //    xValue, fish[i].Position.X)) < optimalDistance) &&
                //    ((int)(MathHelper.Distance(
                //    zValue, fish[i].Position.Z)) < optimalDistance))
                //    return true;
                if (prospectiveBoundingSphere.Intersects(fish[i].BoundingSphere)) return true;
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
        //public static bool IsSeaBedPlaceOccupied(int xValue, int zValue, List<ShipWreck> shipWrecks, List<StaticObject> staticObjects, List<Trash> trashes, List<Factory> factories, ResearchFacility researchFacility)
        //{

        //    if (shipWrecks != null)
        //    {
        //        //not so close to the ship wreck
        //        foreach (GameObject currentObj in shipWrecks)
        //        {
        //            if (((int)(MathHelper.Distance(
        //                xValue, currentObj.Position.X)) < 200) &&
        //                ((int)(MathHelper.Distance(
        //                zValue, currentObj.Position.Z)) < 200))
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    if (staticObjects != null)
        //    {
        //        foreach (GameObject currentObj in staticObjects)
        //        {
        //            if (((int)(MathHelper.Distance(
        //                xValue, currentObj.Position.X)) < 15) &&
        //                ((int)(MathHelper.Distance(
        //                zValue, currentObj.Position.Z)) < 15))
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    if (trashes != null)
        //    {
        //        foreach (Trash trash in trashes)
        //        {
        //            if (((int)(MathHelper.Distance(
        //                xValue, trash.Position.X)) < 20) &&
        //                ((int)(MathHelper.Distance(
        //                zValue, trash.Position.Z)) < 20))
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    if (factories != null)
        //    {
        //        foreach (Factory factory in factories)
        //        {
        //            if (((int)(MathHelper.Distance(
        //                xValue, factory.Position.X)) < 100) &&
        //                ((int)(MathHelper.Distance(
        //                zValue, factory.Position.Z)) < 100))
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    if (researchFacility != null)
        //    {
        //        if (((int)(MathHelper.Distance(
        //                xValue, researchFacility.Position.X)) < 100) &&
        //                ((int)(MathHelper.Distance(
        //                zValue, researchFacility.Position.Z)) < 100))
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        // Finds if seabed place is free within the radius from point xValue, yValue, zValue
        public static bool IsSeaBedPlaceOccupied(int xValue, int yValue, int zValue, int radius, List<ShipWreck> shipWrecks, List<StaticObject> staticObjects, List<Trash> trashes, List<Factory> factories, ResearchFacility researchFacility)
        {
            //BoundingSphere objectBoundingSphere = new BoundingSphere(new Vector3(xValue, 0, zValue), radius);
            BoundingSphere objectBoundingSphere = new BoundingSphere(new Vector3(xValue, yValue, zValue), radius);
            if (shipWrecks != null)
            {
                //not so close to the ship wreck
                foreach (GameObject currentObj in shipWrecks)
                {
                    if (objectBoundingSphere.Intersects(currentObj.BoundingSphere))
                    {
                        return true;
                    }
                }
            }
            if (staticObjects != null)
            {
                foreach (GameObject currentObj in staticObjects)
                {
                    if (objectBoundingSphere.Intersects(currentObj.BoundingSphere))
                    {
                        return true;
                    }
                }
            }
            if (trashes != null)
            {
                foreach (Trash trash in trashes)
                {
                    if (objectBoundingSphere.Intersects(trash.BoundingSphere))
                    {
                        return true;
                    }
                }
            }
            if (factories != null)
            {
                foreach (Factory factory in factories)
                {
                    if (objectBoundingSphere.Intersects(factory.BoundingSphere))
                    {
                        return true;
                    }
                }
            }
            if (researchFacility != null)
            {
                if (objectBoundingSphere.Intersects(researchFacility.BoundingSphere))
                {
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

        public static void PlaceStaticObjectsOnShipFloor(List<StaticObject> staticObjects, List<TreasureChest> treasureChests, Random random, int minX, int maxX, int minZ, int maxZ)
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

        public static Vector3 PerpendicularVector(Vector3 directionVector)
        {
            return new Vector3(-directionVector.Z, directionVector.Y, directionVector.X);
        }

        public static void ReviveDeadEnemy(BaseEnemy[] enemies, int enemyAmount, Fish[] fishes, int fishAmount, HydroBot hydroBot)
        {
            //we can't afford trying forever
            int numTries = 0;
            Random random = new Random();
            for (int i = 0; i < enemyAmount; i++)
            {
                //we do not revive enemy release by submarine cuz the sub will get revived
                if (enemies[i].health <= 0 && !enemies[i].releasedFromSubmarine)
                {
                    int xValue, zValue;
                    //do
                    //{
                    //    xValue = random.Next(0, hydroBot.MaxRangeX);
                    //    zValue = random.Next(0, hydroBot.MaxRangeZ);
                    //    numTries++;
                    //} while (numTries < 20 && (IsSurfaceOccupied(xValue, zValue, enemyAmount, fishAmount, enemies, fishes) ||
                    //     (((int)(MathHelper.Distance(xValue, hydroBot.Position.X)) < 200) &&
                    //     ((int)(MathHelper.Distance(zValue, hydroBot.Position.Z)) < 200))));
                    for (numTries = 0; numTries < 20; numTries++)
                    {
                        xValue = random.Next(0, hydroBot.MaxRangeX);
                        zValue = random.Next(0, hydroBot.MaxRangeZ);
                        if (random.Next(100) % 2 == 0)
                            xValue *= -1;
                        if (random.Next(100) % 2 == 0)
                            zValue *= -1;
                        if (!(IsSurfaceOccupied(new BoundingSphere(new Vector3(xValue, hydroBot.floatHeight, zValue), enemies[i].BoundingSphere.Radius), enemyAmount, fishAmount, enemies, fishes) ||
                         (((int)(MathHelper.Distance(xValue, hydroBot.Position.X)) < 200) &&
                         ((int)(MathHelper.Distance(zValue, hydroBot.Position.Z)) < 200))))
                        {
                            enemies[i].health = enemies[i].maxHealth;
                            enemies[i].stunned = false;
                            enemies[i].isHypnotise = false;
                            enemies[i].gaveExp = false;

                            enemies[i].Position.X = xValue;
                            enemies[i].Position.Z = zValue;
                            enemies[i].Position.Y = hydroBot.floatHeight;
                            enemies[i].BoundingSphere =
                                new BoundingSphere(enemies[i].Position, enemies[i].BoundingSphere.Radius);
                            break;
                        }
                        else
                        {
                            //if we can not find a spare space to put the revived enemy
                            //temporarily put it somewhere far
                            enemies[i].Position.X = -5000;
                            enemies[i].Position.Z = -5000;
                        }
                    }             
                }
            }
        }
    }
}
