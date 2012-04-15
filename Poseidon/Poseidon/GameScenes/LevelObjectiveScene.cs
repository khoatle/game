﻿#region Using Statements

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Poseidon.Core;
using Microsoft.Xna.Framework.Content;

#endregion

namespace Poseidon
{
    /// <summary>
    /// This is a game component that implements the Game Start Scene.
    /// </summary>
    public class LevelObjectiveScene : GameScene
    {
        // Audio
        protected AudioLibrary audio;
        // Spritebatch
        protected SpriteBatch spriteBatch = null;
        ContentManager Content;
        Game game;
        SpriteFont levelObjFont;
        Texture2D objectiveBox, achievedBox;
        Rectangle objectiveBoxRect, achievedBoxRect;
        Vector2 objectiveStringPosition, achievedStringPostion;
        private PlayGameScene playgamescene;
        Random random = new Random();
        /// <summary>
        /// Default Constructor
        public LevelObjectiveScene(Game game, Texture2D background, ContentManager Content, PlayGameScene playgamescene)
            : base(game)
        {
            this.Content = Content;
            Components.Add(new ImageComponent(game, background,
                                            ImageComponent.DrawMode.Stretch));

            // Get the current spritebatch
            spriteBatch = (SpriteBatch)Game.Services.GetService(
                                            typeof(SpriteBatch));

            // Get the audio library
            audio = (AudioLibrary)
                Game.Services.GetService(typeof(AudioLibrary));

            // for the mouse or touch
            //cursor = new Cursor(game, spriteBatch);
            //Components.Add(cursor);

            levelObjFont = Content.Load<SpriteFont>("Fonts/levelObj");
            objectiveBox = Content.Load<Texture2D>("Image/Miscellaneous/levelObjectiveBox");
            achievedBox = Content.Load<Texture2D>("Image/Miscellaneous/achievedObjectiveBox");
            this.playgamescene = playgamescene;
            this.game = game;
        }


        /// <summary>
        /// Show the start scene
        /// </summary>
        public override void Show()
        {

            int objectiveBoxWidth = (int)(game.GraphicsDevice.Viewport.TitleSafeArea.Width * 0.55);
            int objectiveBoxHeight = (int)(game.GraphicsDevice.Viewport.TitleSafeArea.Height * 0.3);
            objectiveBoxRect = new Rectangle(game.GraphicsDevice.Viewport.TitleSafeArea.Center.X - objectiveBoxWidth / 2, game.GraphicsDevice.Viewport.TitleSafeArea.Center.Y - (int)(objectiveBoxHeight * 1.2), objectiveBoxWidth, objectiveBoxHeight);
            int achievedBoxWidth = (int)(game.GraphicsDevice.Viewport.TitleSafeArea.Width * 0.55);
            int achievedBoxHeight = (int)(game.GraphicsDevice.Viewport.TitleSafeArea.Height * 0.3);
            achievedBoxRect = new Rectangle(game.GraphicsDevice.Viewport.TitleSafeArea.Center.X - achievedBoxWidth / 2, game.GraphicsDevice.Viewport.TitleSafeArea.Center.Y + (int)(achievedBoxHeight * 0.2), achievedBoxWidth, achievedBoxHeight);
            

            audio.NewMeteor.Play();
            base.Show();
        }

        /// <summary>
        /// Hide the start scene
        /// </summary>
        public override void Hide()
        {
            //MediaPlayer.Stop();
            base.Hide();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // play the boss fight music for certain levels
            if (PlayGameScene.currentLevel == 3 || PlayGameScene.currentLevel == 11)
            {
                if (MediaPlayer.State.Equals(MediaState.Stopped))
                {
                    MediaPlayer.Play(audio.bossMusics[random.Next(GameConstants.NumBossBackgroundMusics)]);
                }
            }
            else if (MediaPlayer.State.Equals(MediaState.Stopped))
            {
                MediaPlayer.Play(audio.backgroundMusics[random.Next(GameConstants.NumNormalBackgroundMusics)]);
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            int currentLevel = PlayGameScene.currentLevel;
            string level_description;
            string level_objective="";
            string achieved_status="";

            if (HydroBot.gameMode == GameMode.SurvivalMode)
                level_description = "SURVIVAL MODE";
            if (PoseidonGame.gamePlus)
            {
                level_description = "GAME + " + HydroBot.gamePlusLevel + "\n" + " LEVEL " + (currentLevel + 1).ToString();
            }
            else
            {
                level_description = "LEVEL " + (currentLevel + 1).ToString();
            }

            spriteBatch.Begin();
            base.Draw(gameTime);

            spriteBatch.Draw(objectiveBox, objectiveBoxRect, Color.White);
            spriteBatch.Draw(achievedBox, achievedBoxRect, Color.White);

            int realFishAmount = playgamescene.fishAmount;
            if (HydroBot.hasDolphin) realFishAmount -= 1;
            if (HydroBot.hasSeaCow) realFishAmount -= 1;
            if (HydroBot.hasTurtle) realFishAmount -= 1;

            if (HydroBot.gameMode == GameMode.SurvivalMode)
            {
                level_objective = "Protect the ancient sea animal against hunters";
            }
            else if (currentLevel == 0)
            {
                double env_percent = Math.Min((double)HydroBot.currentEnvPoint / (double)HydroBot.maxEnvPoint * 100, 100);
                double target_percent = GameConstants.LevelObjective[currentLevel]*100;
                level_objective = "Increase the environment bar to " + target_percent.ToString() + "% within " + HydroBot.levelDuration + " days.";
                if (env_percent < target_percent)
                    achieved_status = "Now the environment bar is " + env_percent.ToString() + "%.";
                else
                    achieved_status = "You achieved the target. Keep cleaning the environment for next level.";
            }
            else if (currentLevel == 1)
            {
                double fish_percent = Math.Min(((double)realFishAmount/(double)GameConstants.NumberFish[currentLevel]) * 100, 100);
                double target_percent = GameConstants.LevelObjective[currentLevel] * 100;
                level_objective = "Save at least " + target_percent.ToString() + "% of the sea creatures within " + HydroBot.levelDuration + " days.";
                achieved_status = "There are " + (int)fish_percent + "% sea creatures remaining.";
            }
            else if (currentLevel == 2)
            {
                level_objective = "Find the relic in " + HydroBot.levelDuration + " days.";
                if (HydroBot.skills[3] == false)
                {
                    achieved_status = "Relic not found.";
                }
                else
                {
                    achieved_status = "You found Hermes' Winged sandal.";
                }
            }
            else if (currentLevel == 3)
            {
                level_objective = "Defeat the mutant shark in " + HydroBot.levelDuration + " days.";
                if (PlayGameScene.isBossKilled)
                    achieved_status = "Mutant shark has been defeated.";
                else achieved_status = "Mutant shark is still lurking around.";
            }
            else if (currentLevel == 4)
            {
                double shark_percent = Math.Min(((double)realFishAmount / (double)GameConstants.NumberFish[currentLevel]) * 100, 100);
                double target_percent = GameConstants.LevelObjective[currentLevel] * 100;
                level_objective = "Save at least " + target_percent.ToString() + "% of the sharks within " + HydroBot.levelDuration + " days.";
                achieved_status = "There are " + (int)shark_percent + "% sharks remaining.";
            }
            else if (currentLevel == 5)
            {
                level_objective = "Find the relic in " + HydroBot.levelDuration + " days.";
                if (HydroBot.skills[0] == false)
                {
                    achieved_status = "Relic not found.";
                }
                else
                {
                    achieved_status = "You found the Hercules' bow. Aim well and use it wisely.";
                }
            }
            else if (currentLevel == 6)
            {
                level_objective = "Find the relic in " + HydroBot.levelDuration + " days.";
                if (HydroBot.skills[1] == false)
                {
                    achieved_status = "Relic not found.";
                }
                else
                {
                    achieved_status = "You found Thor's hammer. Use it when surrounded by enemies.";
                }
            }
            else if (currentLevel == 7)
            {
                level_objective = "Find the relic in " + HydroBot.levelDuration + " days.";
                if (HydroBot.skills[2] == false)
                {
                    achieved_status = "Relic not found.";
                }
                else
                {
                    achieved_status = "You found Achilles' armor. Use it when you feel helpless.";
                }
            }
            else if (currentLevel == 8)
            {
                level_objective = "Find the relic in " + HydroBot.levelDuration +" days.";
                if (HydroBot.skills[4] == false)
                {
                    achieved_status = "Relic not found.";
                }
                else
                {
                    achieved_status = "You found Aphrodite's belt. Use it when there are too many enemies around.";
                }
            }
            else if (currentLevel == 9)
            {
                level_objective = "Break through enemy defense alive.";
                achieved_status = "You have "+ Math.Min((HydroBot.currentHitPoint/HydroBot.maxHitPoint*100), 100) + "% health remaining.";
            }
            else if (currentLevel == 10)
            {
                level_objective = "Defeat the Terminator within " + HydroBot.levelDuration + " days.";
                achieved_status = "Terminator is as strong as ever. You did not even dent his armour.";
            }
            else if (currentLevel == 11)
            {
                level_objective = "Defeat the Terminator within " + HydroBot.levelDuration + " days.";
                if (PlayGameScene.isBossKilled)
                    achieved_status = "Terminator has been defeated.";
                else achieved_status = "Terminator is still alive.";
            }

            spriteBatch.DrawString(levelObjFont, level_description, new Vector2(game.Window.ClientBounds.Center.X - levelObjFont.MeasureString(level_description).X, 10), Color.Red, 0, new Vector2(0, 0), 2f, SpriteEffects.None, 0);

            level_objective = IngamePresentation.wrapLine(level_objective, objectiveBoxRect.Width - 100, levelObjFont);
            achieved_status = IngamePresentation.wrapLine(achieved_status, achievedBoxRect.Width - 100, levelObjFont);

           // objectiveStringPosition = new Vector2(objectiveBoxRect.Left + 60, objectiveBoxRect.Top+100);
           // achievedStringPostion = new Vector2(achievedBoxRect.Left + 60, achievedBoxRect.Top+100);

            objectiveStringPosition = new Vector2(objectiveBoxRect.Center.X - levelObjFont.MeasureString(level_objective).X/2, objectiveBoxRect.Center.Y - levelObjFont.MeasureString(level_objective).Y/2);
            achievedStringPostion = new Vector2(achievedBoxRect.Center.X - levelObjFont.MeasureString(achieved_status).X / 2, achievedBoxRect.Center.Y - levelObjFont.MeasureString(achieved_status).Y/2);

            spriteBatch.DrawString(levelObjFont, level_objective, objectiveStringPosition, Color.Blue);

            spriteBatch.DrawString(levelObjFont, achieved_status, achievedStringPostion, Color.Blue);

            string nextText = "Press Enter/esc to continue";
            Vector2 nextTextPosition = new Vector2(game.Window.ClientBounds.Right - levelObjFont.MeasureString(nextText).X, game.Window.ClientBounds.Bottom - levelObjFont.MeasureString(nextText).Y);
            spriteBatch.DrawString(levelObjFont, nextText, nextTextPosition, Color.Black);
            
            spriteBatch.End();
        }
    }
}


