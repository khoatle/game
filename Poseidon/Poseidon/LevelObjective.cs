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
            }
            return false;
        }
    }
}
