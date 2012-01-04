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
        public bool CheckWinCondition()
        {
            if (currentLevel == 0)
            {
                // this is just for instantly testing minigames
                //if (enemiesAmount == 0) return true;
                //if (isBossKilled) return true;


                //Level Obj: you need increase the env bar to 80% within 4 min ( 30 days).
                if (roundTimer <= TimeSpan.Zero && ((double)HydroBot.currentEnvPoint / (double)HydroBot.maxEnvPoint >= GameConstants.LevelObjective[currentLevel]))
                //real obj above, below is just for easier testing
                //if ((double)Tank.currentEnvPoint / (double)Tank.maxEnvPoint >= 0.6)
                    return true;

            }
            else if (currentLevel == 1)
            {
                //Level Obj: Save at least 50% of fish during 4 min ( 30 days ).
                if (roundTimer <= TimeSpan.Zero && ((double)fishAmount / (double)GameConstants.NumberFish[currentLevel] >= GameConstants.LevelObjective[currentLevel]))
                {
                    return true;
                }
            }
            else if (currentLevel == 2)
            {
                //Level Obj: Find the relic in 3 months.
                if (roundTimer <= TimeSpan.Zero && HydroBot.skills[3] == true)
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
                if (roundTimer <= TimeSpan.Zero && HydroBot.skills[0] == true)
                {
                    return true;
                }
            }
            else if (currentLevel == 6)
            {
                //Level Obj: Find the relic in 3 months.
                if (roundTimer <= TimeSpan.Zero && HydroBot.skills[1] == true)
                {
                    return true;
                }
            }
            else if (currentLevel == 7)
            {
                //Level Obj: Find the relic in 3 months.
                if (roundTimer <= TimeSpan.Zero && HydroBot.skills[2] == true)
                {
                    return true;
                }
            }
            else if (currentLevel == 8)
            {
                //Level Obj: Find the relic in 3 months.
                if (roundTimer <= TimeSpan.Zero && HydroBot.skills[4] == true)
                {
                    return true;
                }
            }
            else if (currentLevel == 9)
            {
                //Level Obj: Survive the level
                if (roundTimer <= TimeSpan.Zero)
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
            //Always lose when the environment is completely destroyed
            if (HydroBot.currentEnvPoint <= 0)   return true;

            if (currentLevel == 0)
            {
                if (roundTimer <= TimeSpan.Zero && ((double)HydroBot.currentEnvPoint / (double)HydroBot.maxEnvPoint < GameConstants.LevelObjective[currentLevel]))
                    return true;
            }
            if (currentLevel == 1)
            {
                if ((double)fishAmount / (double)GameConstants.NumberFish[currentLevel] < GameConstants.LevelObjective[currentLevel]) return true;
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
