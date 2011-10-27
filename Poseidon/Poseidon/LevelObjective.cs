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
                // kill all enemies to win this level
                if (enemiesAmount == 0) return true;
                if (isBossKilled) return true;
            }
            if (currentLevel == 1)
            {
                // save atleast 10 fish to win this level
                if (roundTimer <= TimeSpan.Zero && fishAmount >= 10)
                {
                    return true;
                }
            }

            return false;
        }
        public bool CheckLoseCondition()
        {
            if (currentLevel == 0)
            {
                if (roundTimer < TimeSpan.Zero) return true;
            }
            if (currentLevel == 1)
            {
                if (fishAmount < 10) return true;
            }
            return false;
        }
    }
}
