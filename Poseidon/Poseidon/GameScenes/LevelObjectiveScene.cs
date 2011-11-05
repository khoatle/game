#region Using Statements

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
        Vector2 objectiveStringPosition, achievedStringPostion, tipPosition;
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
            objectiveBoxRect = new Rectangle(game.Window.ClientBounds.Center.X-350, game.Window.ClientBounds.Center.Y-300 , 700, 250);
            achievedBoxRect = new Rectangle(game.Window.ClientBounds.Center.X - 350, game.Window.ClientBounds.Center.Y + 50, 700, 250);
            this.playgamescene = playgamescene;
            this.game = game;
        }


        /// <summary>
        /// Show the start scene
        /// </summary>
        public override void Show()
        {
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
            if (PlayGameScene.currentLevel == 3 || PlayGameScene.currentLevel == 10)
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
            string level_description = "LEVEL "+ (currentLevel+1).ToString();
            string level_objective="";
            string achieved_status="";
            string tip = "TIP --> ";
            spriteBatch.Begin();
            base.Draw(gameTime);

            spriteBatch.Draw(objectiveBox, objectiveBoxRect, Color.White);
            spriteBatch.Draw(achievedBox, achievedBoxRect, Color.White);

            if (currentLevel == 0)
            {
                double env_percent = (double)Tank.currentEnvPoint / (double)Tank.maxEnvPoint * 100;
                double target_percent = GameConstants.LevelObjective[currentLevel]*100;
                level_objective = "Increase the environment bar to " + target_percent.ToString() + "% within " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.";
                if (env_percent < target_percent)
                    achieved_status = "Now the environment bar is " + env_percent.ToString() + "%.";
                else
                    achieved_status = "You achieved the target. Keep cleaning the environment for next level.";
                tip += "The environment status at the end of a level will affect the next level"; 
            }
            else if (currentLevel == 1)
            {
                double fish_percent = ((double)playgamescene.fishAmount/(double)GameConstants.NumberFish[currentLevel]) * 100;
                double target_percent = GameConstants.LevelObjective[currentLevel] * 100;
                level_objective = "Save at least " + target_percent.ToString() + "% of the sea creatures within " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.";
                achieved_status = "There are " + fish_percent.ToString() + "% sea creatures remaining.";
                tip += "It is much easier to aim and shoot while you hold 'ctrl'.";
            }
            else if (currentLevel == 2)
            {
                level_objective = "Find the relic in " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.";
                if (Tank.skills[3] == false)
                {
                    achieved_status = "Relic not found.";
                    tip += "Cleaning the environment and helping the fish make the fish happy.";
                }
                else
                {
                    achieved_status = "You found Hermes' Winged sandal.";
                    tip += "Hermes's Winged sandal is really useful when you want to get away.";
                }
            }
            else if (currentLevel == 3)
            {
                level_objective = "Destroy the mutant shark in " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.";
                achieved_status = "Mutant shark is still lurking around.";
                tip += "Don't just flee with the sandal. Make sure you hurt the mutant shark with it.";
            }
            else if (currentLevel == 4)
            {
                double shark_percent = ((double)playgamescene.fishAmount / (double)GameConstants.NumberFish[currentLevel]) * 100;
                double target_percent = GameConstants.LevelObjective[currentLevel] * 100;
                level_objective = "Save at least " + target_percent.ToString() + "% of the sharks within " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.";
                achieved_status = "There are " + shark_percent.ToString() + "% sharks remaining.";
                tip += "Use your experience points (press I)."; 
            }
            else if (currentLevel == 5)
            {
                level_objective = "Find the relic in " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.";
                if (Tank.skills[0] == false)
                {
                    achieved_status = "Relic not found.";
                    tip += "Fruits can help a lot in battle.";
                }
                else
                {
                    achieved_status = "You found the Hercules' bow. Aim well and use it wisely.";
                    tip += "Hercules's bow hurts a single enemy a lot.";
                }
            }
            else if (currentLevel == 6)
            {
                level_objective = "Find the relic in " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.";
                if (Tank.skills[1] == false)
                {
                    achieved_status = "Relic not found.";
                    tip += "Read the writing on the paintings. They help you in the quiz.";
                }
                else
                {
                    achieved_status = "You found Thor's hammer. Use it when surrounded by enemies.";
                    tip += "Thor's hammer stuns and pushes enemies away.";
                }
            }
            else if (currentLevel == 7)
            {
                level_objective = "Find the relic in " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.";
                if (Tank.skills[2] == false)
                {
                    achieved_status = "Relic not found.";
                    tip += "Fight smart, do not let the enemy touch you.";
                }
                else
                {
                    achieved_status = "You found Achilles' armor. Use it when you feel helpless.";
                    tip += "Achilles' armor makes you invincible for 5 seconds.";
                }
            }
            else if (currentLevel == 8)
            {
                level_objective = "Find the relic in " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.";
                if (Tank.skills[4] == false)
                {
                    achieved_status = "Relic not found.";
                    tip += "Remember what the fish says, for these would be used in the minigames.";
                }
                else
                {
                    achieved_status = "You found Aphrodite's belt. Use it when there are too many enemies around.";
                    tip += "Aphrodite's belt makes enemies turn against each other.";
                }
            }
            else if (currentLevel == 9)
            {
                level_objective = "Defeat the Terminator within " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.";
                achieved_status = "Terminator is as strong as ever. You did not even dent his armour.";
                tip += "Use your skills effeciently since they reduce your health.";
            }
            else if (currentLevel == 10)
            {
                level_objective = "Defeat the Terminator within " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.";
                achieved_status = "Terminator is still alive.";
                tip += "Good Luck. You are almost there.";
            }

            spriteBatch.DrawString(levelObjFont, level_description, new Vector2(game.Window.ClientBounds.Center.X - levelObjFont.MeasureString(level_description).X, 10), Color.Red, 0, new Vector2(0, 0), 2f, SpriteEffects.None, 0);

            level_objective = AddingObjects.wrapLine(level_objective, objectiveBoxRect.Width-100, levelObjFont);
            achieved_status = AddingObjects.wrapLine(achieved_status, achievedBoxRect.Width-100, levelObjFont);

            objectiveStringPosition = new Vector2(objectiveBoxRect.Left + 60, objectiveBoxRect.Top+100);
            achievedStringPostion = new Vector2(achievedBoxRect.Left + 60, achievedBoxRect.Top+100);
            tipPosition = new Vector2(game.Window.ClientBounds.Center.X - levelObjFont.MeasureString(tip).X/2 ,  game.Window.ClientBounds.Bottom - 50);

            spriteBatch.DrawString(levelObjFont, level_objective, objectiveStringPosition, Color.Blue);

            spriteBatch.DrawString(levelObjFont, achieved_status, achievedStringPostion, Color.Blue);

            spriteBatch.DrawString(levelObjFont, tip, tipPosition, Color.Black);



            spriteBatch.End();
        }
    }
}


