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
        public static void ChangeSkillBulletWithKeyBoard(KeyboardState lastKeyboardState, KeyboardState currentKeyboardState, HydroBot tank, GameMode gameMode)
        {
            if (currentKeyboardState.IsKeyDown(Keys.D1) && HydroBot.skills[0]) HydroBot.activeSkillID = 0;
            if (currentKeyboardState.IsKeyDown(Keys.D2) && HydroBot.skills[1]) HydroBot.activeSkillID = 1;
            if (currentKeyboardState.IsKeyDown(Keys.D3) && HydroBot.skills[2]) HydroBot.activeSkillID = 2;
            if (currentKeyboardState.IsKeyDown(Keys.D4) && HydroBot.skills[3]) HydroBot.activeSkillID = 3;
            if (currentKeyboardState.IsKeyDown(Keys.D5) && HydroBot.skills[4]) HydroBot.activeSkillID = 4;
            if (lastKeyboardState.IsKeyDown(Keys.Space) && currentKeyboardState.IsKeyUp(Keys.Space))
            {
                // level 0 of main game, can only heal
                if (PlayGameScene.currentLevel != 0 || gameMode == GameMode.SurvivalMode)
                {
                    HydroBot.bulletType++;
                    if (HydroBot.bulletType == GameConstants.numBulletTypes) HydroBot.bulletType = 0;
                    PoseidonGame.audio.ChangeBullet.Play();
                }
            }
        }
    }
}
