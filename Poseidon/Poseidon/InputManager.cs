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
    public static class InputManager
    {
        public static void ChangeSkillBulletWithKeyBoard(KeyboardState lastKeyboardState, KeyboardState currentKeyboardState, Tank tank)
        {
            if (currentKeyboardState.IsKeyDown(Keys.D1)) tank.activeSkillID = 0;
            if (currentKeyboardState.IsKeyDown(Keys.D2)) tank.activeSkillID = 1;
            if (currentKeyboardState.IsKeyDown(Keys.D3)) tank.activeSkillID = 2;
            if (currentKeyboardState.IsKeyDown(Keys.D4)) tank.activeSkillID = 3;
            if (lastKeyboardState.IsKeyDown(Keys.Space) && currentKeyboardState.IsKeyUp(Keys.Space))
            {
                tank.bulletType++;
                if (tank.bulletType == GameConstants.numBulletTypes) tank.bulletType = 0;
            }
        }
    }
}
