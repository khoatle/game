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
            if (currentKeyboardState.IsKeyDown(Keys.D1)) Tank.activeSkillID = 0;
            if (currentKeyboardState.IsKeyDown(Keys.D2)) Tank.activeSkillID = 1;
            if (currentKeyboardState.IsKeyDown(Keys.D3)) Tank.activeSkillID = 2;
            if (currentKeyboardState.IsKeyDown(Keys.D4)) Tank.activeSkillID = 3;
            if (currentKeyboardState.IsKeyDown(Keys.D5)) Tank.activeSkillID = 4;
            if (lastKeyboardState.IsKeyDown(Keys.Space) && currentKeyboardState.IsKeyUp(Keys.Space))
            {
                // level 0, can only heal
                if (PlayGameScene.currentLevel != 0)
                {
                    Tank.bulletType++;
                    if (Tank.bulletType == GameConstants.numBulletTypes) Tank.bulletType = 0;
                    PlayGameScene.audio.ChangeBullet.Play();
                }
            }
        }
    }
}
