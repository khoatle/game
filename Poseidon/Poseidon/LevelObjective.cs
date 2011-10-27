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
                //Level Obj: you need increase the env bar to 80% within 3 min ( 90 days).
                if (roundTimer <= TimeSpan.Zero && ((double)Tank.currentEnvPoint / (double)Tank.maxEnvPoint >= 0.8))
                    return true;
                
            }
            if (currentLevel == 1)
            {
                //Level Obj: Save at least 80% of fish during 3 min ( 90 days ).
                if (roundTimer <= TimeSpan.Zero && (fishAmount/GameConstants.NumberFish[currentLevel] >= 0.8))
                {
                    return true;
                }
            }
            if (currentLevel == 2)
            {
                //Level Obj: Find the relic in 3 months.
                if (roundTimer <= TimeSpan.Zero && Tank.skills[3] == true)
                {
                    return true;
                }
            }
            if (currentLevel == 3)
            {
                //Level Obj: Defeat the mutant shark
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
            if (Tank.currentEnvPoint <= 0)
                return true;
            if (currentLevel == 0)
            {
                if (roundTimer <= TimeSpan.Zero && ((double)Tank.currentEnvPoint / (double)Tank.maxEnvPoint >= 0.8)) 
                    return true;
            }
            if (currentLevel == 1)
            {
                if (fishAmount / GameConstants.NumberFish[currentLevel] < 0.8) return true;
            }
            if (currentLevel == 2)
            {
                if (roundTimer <= TimeSpan.Zero && Tank.skills[3] == false)
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
            return false;
        }
    }
}
