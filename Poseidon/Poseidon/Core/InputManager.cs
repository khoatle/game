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
        //for skill combo: to know whether we are waiting for the input of 2nd skill or 1st skill
        public static bool firstSkillEntered = false;
        public static void ChangeSkillBulletWithKeyBoard(KeyboardState lastKeyboardState, KeyboardState currentKeyboardState, HydroBot tank, GameMode gameMode)
        {
            bool skill1Entered, skill2Entered, skill3Entered, skill4Entered, skill5Entered;
            skill1Entered = skill2Entered = skill3Entered = skill4Entered = skill5Entered = false;
            skill1Entered = currentKeyboardState.IsKeyUp(Keys.D1) && lastKeyboardState.IsKeyDown(Keys.D1) && HydroBot.skills[0];
            skill2Entered = currentKeyboardState.IsKeyUp(Keys.D2) && lastKeyboardState.IsKeyDown(Keys.D2) && HydroBot.skills[1];
            skill3Entered = currentKeyboardState.IsKeyUp(Keys.D3) && lastKeyboardState.IsKeyDown(Keys.D3) && HydroBot.skills[2];
            skill4Entered = currentKeyboardState.IsKeyUp(Keys.D4) && lastKeyboardState.IsKeyDown(Keys.D4) && HydroBot.skills[3];
            skill5Entered = currentKeyboardState.IsKeyUp(Keys.D5) && lastKeyboardState.IsKeyDown(Keys.D5) && HydroBot.skills[4];
            //change active skill
            if (firstSkillEntered == false)
            {
                if (skill1Entered)
                {
                    HydroBot.activeSkillID = 0;
                    if (HydroBot.skillComboActivated) firstSkillEntered = true;
                }
                if (skill2Entered)
                {
                    HydroBot.activeSkillID = 1;
                    if (HydroBot.skillComboActivated) firstSkillEntered = true;
                }
                if (skill3Entered)
                { 
                    HydroBot.activeSkillID = 2;
                    if (HydroBot.skillComboActivated) firstSkillEntered = true;
                }
                if (skill4Entered)
                { 
                    HydroBot.activeSkillID = 3;
                    if (HydroBot.skillComboActivated) firstSkillEntered = true;
                }
                if (skill5Entered)
                {
                    HydroBot.activeSkillID = 4;
                    if (HydroBot.skillComboActivated) firstSkillEntered = true;
                }
            }
            //input for 2nd skill if skill combo activated
            else if (HydroBot.skillComboActivated && firstSkillEntered == true)
            {
                if (skill1Entered)
                {
                    HydroBot.secondSkillID = 0;
                    if (HydroBot.skillComboActivated) firstSkillEntered = false;
                }
                if (skill2Entered)
                {
                    HydroBot.secondSkillID = 1;
                    if (HydroBot.skillComboActivated) firstSkillEntered = false;
                }
                if (skill3Entered)
                {
                    HydroBot.secondSkillID = 2;
                    if (HydroBot.skillComboActivated) firstSkillEntered = false;
                }
                if (skill4Entered)
                {
                    HydroBot.secondSkillID = 3;
                    if (HydroBot.skillComboActivated) firstSkillEntered = false;
                }
                if (skill5Entered)
                {
                    HydroBot.secondSkillID = 4;
                    if (HydroBot.skillComboActivated) firstSkillEntered = false;
                }
            }
            //change bullet type
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
