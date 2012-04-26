using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Poseidon.Core;
namespace Poseidon
{
    public partial class PlayGameScene : GameScene
    {
        public static bool newLevelObjAvailable = true;
        public bool CheckWinCondition()
        {
            //if (enemiesAmount <= 10000) return true;
            if (currentLevel == 0)
            {
                // this is just for instantly testing minigames
                //if (enemiesAmount == 0) return true;
                //if (isBossKilled) return true;

                if (levelObjectiveState == 0 && HydroBot.bioTrash >= 5)
                {
                    levelObjectiveState = 1;
                    newLevelObjAvailable = true;
                }
                else if (levelObjectiveState == 1 && HydroBot.plasticTrash >= 5)
                {
                    levelObjectiveState = 2;
                    newLevelObjAvailable = true;
                }
                else if (levelObjectiveState == 2)
                {
                    foreach (Factory factory in factories)
                        if (factory.factoryType == FactoryType.biodegradable)
                        {
                            levelObjectiveState = 3;
                            newLevelObjAvailable = true;
                        }
                }
                else if (levelObjectiveState == 3)
                {
                    if (HydroBot.bioTrash == 0)
                    {
                        levelObjectiveState = 4;
                        newLevelObjAvailable = true;
                    }
                }
                else if (levelObjectiveState == 4)
                {
                    if (HydroBot.plasticTrash == 0)
                    {
                        levelObjectiveState = 5;
                        newLevelObjAvailable = true;
                    }
                }
                else if (levelObjectiveState == 5)
                {
                    //this check will be done in playgamescene.cs
                }
                else if (levelObjectiveState == 6)
                {
                    //this check will be done in collision.cs
                }
                else if (levelObjectiveState == 7)
                {
                    if (HydroBot.level >= 2)
                    {
                        levelObjectiveState = 8;
                        newLevelObjAvailable = true;
                    }
                }
                else if (levelObjectiveState == 8)
                {
                    if (researchFacility != null && (HydroBot.strength > GameConstants.MainCharStrength || HydroBot.speed > GameConstants.BasicStartSpeed || HydroBot.shootingRate > GameConstants.MainCharShootingSpeed || HydroBot.maxHitPoint > GameConstants.PlayerStartingHP))
                    {
                        levelObjectiveState = 9;
                        newLevelObjAvailable = true;
                    }

                }
                //Level Obj: you need increase the env bar to 80% within 4 min ( 30 days).
                else if (levelObjectiveState == 9 && ((double)HydroBot.currentEnvPoint / (double)HydroBot.maxEnvPoint >= GameConstants.LevelObjective[currentLevel]))
                    //real obj above, below is just for easier testing
                    //if ((double)Tank.currentEnvPoint / (double)Tank.maxEnvPoint >= 0.6)
                    return true;

            }
            else if (currentLevel == 1)
            {
                if (levelObjectiveState == 0)
                {
                    //this check is done in Inputmanager.cs
                }
                else if (levelObjectiveState == 1 && PlayGameScene.numNormalKills >= 3)
                {
                    levelObjectiveState = 2;
                }
                //Level Obj: Save at least 50% of fish during 4 min ( 30 days ).
                else if (levelObjectiveState == 2 && roundTimer <= TimeSpan.Zero && ((double)fishAmount / (double)GameConstants.NumberFish[currentLevel] >= GameConstants.LevelObjective[currentLevel]))
                {
                    return true;
                }
            }
            else if (currentLevel == 2)
            {
                //Level Obj: Find the relic in 3 months.
                if (HydroBot.skills[3] == true)
                {
                    return true;
                }
            }
            else if (currentLevel == 3)
            {
                //Level Obj: Defeat the mutant shark
                if (isBossKilled)
                {
                    return true;
                }
            }
            else if (currentLevel == 4)
            {
                //Level Obj: save at least 50% of sharks in 3 mins
                //Level 4 is full of sharks so only need to check total number of fishes
                if (roundTimer <= TimeSpan.Zero && ((double)fishAmount / (double)GameConstants.NumberFish[currentLevel] >= GameConstants.LevelObjective[currentLevel]))
                {
                    return true;
                }
            }
            else if (currentLevel == 5)
            {
                //Level Obj: Find the relic in 3 months.
                if (HydroBot.skills[0] == true)
                {
                    return true;
                }
            }
            else if (currentLevel == 6)
            {
                //Level Obj: Find the relic in 3 months.
                if (HydroBot.skills[1] == true)
                {
                    return true;
                }
            }
            else if (currentLevel == 7)
            {
                //Level Obj: Find the relic in 3 months.
                if (HydroBot.skills[2] == true)
                {
                    return true;
                }
            }
            else if (currentLevel == 8)
            {
                //Level Obj: Find the relic in 3 months.
                if (HydroBot.skills[4] == true)
                {
                    return true;
                }
            }
            else if (currentLevel == 9)
            {
                //Level Obj: Survive the level
                if (roundTimer <= TimeSpan.Zero || enemiesAmount == 0)
                {
                    return true;
                }
            }
            else if (currentLevel == 10)
            {
                //Level Obj: Defeat the Terminator
                //well, if you really can
                if (isBossKilled)
                {
                    return true;
                }
            }
            else if (currentLevel == 11)
            {
                //Level Obj: Defeat the Terminator
                //after gaining the Trident power from Poseidon
                if (isBossKilled)
                {
                    return true;
                }
            }
            return false;
        }
        public bool CheckLoseCondition()
        {
            //if (HydroBot.currentEnvPoint <= 10000) return true;
            //Always lose when the environment is completely destroyed
            if (HydroBot.currentEnvPoint <= 0)   return true;

            if (currentLevel == 0)
            {
                if (roundTimer <= TimeSpan.Zero && (levelObjectiveState < 9 || ((double)HydroBot.currentEnvPoint / (double)HydroBot.maxEnvPoint < GameConstants.LevelObjective[currentLevel])))
                    return true;
            }
            if (currentLevel == 1)
            {
                if ((levelObjectiveState == 2 && ((double)fishAmount / (double)GameConstants.NumberFish[currentLevel] < GameConstants.LevelObjective[currentLevel]))
                    || (levelObjectiveState < 2 && roundTimer <= TimeSpan.Zero))
                    return true;
            }
            if (currentLevel == 2)
            {
                if (roundTimer <= TimeSpan.Zero && HydroBot.skills[3] == false)
                {
                    return true;
                }
            }
            if (currentLevel == 3)
            {
                if (roundTimer <= TimeSpan.Zero)
                {
                    return true;
                }
            }
            if (currentLevel == 4)
            {
                if ((double)fishAmount / (double)GameConstants.NumberFish[currentLevel] < 0.5) return true;
            }
            if (currentLevel == 5)
            {
                if (roundTimer <= TimeSpan.Zero && HydroBot.skills[0] == false)
                {
                    return true;
                }
            }
            if (currentLevel == 6)
            {
                if (roundTimer <= TimeSpan.Zero && HydroBot.skills[1] == false)
                {
                    return true;
                }
            }
            if (currentLevel == 7)
            {
                if (roundTimer <= TimeSpan.Zero && HydroBot.skills[2] == false)
                {
                    return true;
                }
            }
            if (currentLevel == 8)
            {
                if (roundTimer <= TimeSpan.Zero && HydroBot.skills[4] == false)
                {
                    return true;
                }
            }
            if (currentLevel == 9)
            {
                //Level Obj: Survive the level
                
            }
            if (currentLevel == 10)
            {
                if (roundTimer <= TimeSpan.Zero)
                {
                    return true;
                }
            }
            if (currentLevel == 11)
            {
                if (roundTimer <= TimeSpan.Zero)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
