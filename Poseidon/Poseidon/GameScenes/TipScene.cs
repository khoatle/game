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
    public class TipScene : GameScene
    {
        // Audio
        protected AudioLibrary audio;
        // Spritebatch
        protected SpriteBatch spriteBatch = null;
        ContentManager Content;
        Game game;
        SpriteFont tipFont;
        Texture2D tipBox;
        Rectangle tipBoxRect;
        Vector2 tipStringPosition;
        Random random = new Random();
        /// <summary>
        /// Default Constructor
        public TipScene(Game game, Texture2D background, ContentManager Content)
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

            tipFont = Content.Load<SpriteFont>("Fonts/tip");
            tipBox = Content.Load<Texture2D>("Image/Miscellaneous/tipBox");
            tipBoxRect = new Rectangle(game.Window.ClientBounds.Center.X - 500, game.Window.ClientBounds.Center.Y - 300, 1000, 600);
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
            string title = "tip";
            string text = "";
            spriteBatch.Begin();
            base.Draw(gameTime);

            spriteBatch.Draw(tipBox, tipBoxRect, Color.White);

            if (currentLevel == 0)
            {
                text = "Press 'z' to clean trash & 'X' to drop seeds. Click on fish to heal them.";
                text += "\nPoint on a fish and press 'CapsLock' to lock the curson on it.";
                text += "\nEnvironement at the end of a level effects the next level.";
            }
            else if (currentLevel == 1)
            {
                text = "Use 'space bar' to switch between bullets.";
                text += "\nWhen the environment is polluted, the sea creatures die easily.";
                text += "\nEvery time your experience levels up, you gain 5 points to increase attributes. Press 'I'.";
            }
            else if (currentLevel == 2)
            {
                text = "Double click on a ship wreck to get into it. Press 'esc' to get out.";
                text += "\nCleaning the environment and healing make the fish happy.";
                text += "\nDrop seeds near the shipwreck so that you can eat health fruits when you come out of it.";
            }
            else if (currentLevel == 3)
            {
                text = "Hermes's Winged sandal is really useful when you want to get away. Don't just flee with the sandal. Make sure you hurt the mutant shark with it.";
                text += "\nThe mutant shark is poisonous. Keep distance.";
                text += "\nDon't forget to use your experience points. Press 'I'.";
            }
            else if (currentLevel == 4)
            {
                text = "Go for the mutant shark first.";
                text += "\nEvery time a fish dies, the environment goes down.";
                text += "\nYou lose health every time you use the sandal.";
                text += "\nThe weaker you are, the weaker is your sandal.";
            }
            else if (currentLevel == 5)
            {
                text = "Remember, there are 3 shipwrecks. You need to search fast.";
                text += "\nRead the writing on the paintings. They help you in the quiz.";
                text += "\nIt is much easier to aim and shoot while you hold 'ctrl'.";
            }
            else if (currentLevel == 6)
            {
                text = "Hercules's bow hurts a single enemy a lot.";
                text += "\nRead the writing on the paintings. They help you in the quiz.";
                text += "\nFruits can help a lot in battle.";
            }
            else if (currentLevel == 7)
            {
                text = "Thor's hammer stuns and pushes enemies away. Fight smart, do not let the enemy touch you.";
                text += "\nIt is much easier to aim and shoot while you hold 'ctrl'.";
            }
            else if (currentLevel == 8)
            {
                text = "Achilles' armor makes you temporarily invincible.";
                text += "\nRemember what the fish says, for these would be used in the minigames.";
            }
            else if (currentLevel == 9)
            {
                text = "Aphrodite's belt makes enemies turn against each other.";
                text += "\nUse all your skills. Press 1-5.";
                text += "\nShift + RightClick can also be used to switch skills.";
                text += "\nYou are invincible near your \"base of plants\".";
            }
            else if (currentLevel == 10)
            {
                text = "Plant enough trees so that you get the fruits when you need it.";
                text += "\nRunning in a circular motion can help evade the deadly bullets.";
            }
            else if (currentLevel == 11)
            {
                text = "There is less chance to get hit by a bullet when you run in a spiral motion.";
                text += "\nUse all your skills. Press 1-5.";
                text += "\nRemember to use your experience points.";
                text += "\nDrop seeds. You need the fruits.";
            }

            spriteBatch.DrawString(tipFont, title, new Vector2(game.Window.ClientBounds.Center.X - tipFont.MeasureString(title).X, 10), Color.Red, 0, new Vector2(0, 0), 2f, SpriteEffects.None, 0);

            text = AddingObjects.wrapLine(text, tipBoxRect.Width - 100, tipFont);

            tipStringPosition = new Vector2(tipBoxRect.Center.X - tipFont.MeasureString(text).X / 2, tipBoxRect.Center.Y - tipFont.MeasureString(text).Y/2);

            spriteBatch.DrawString(tipFont, text, tipStringPosition, Color.Black);

            string nextText = "Press Enter/esc to continue";
            Vector2 nextTextPosition = new Vector2(game.Window.ClientBounds.Right - tipFont.MeasureString(nextText).X, game.Window.ClientBounds.Bottom - tipFont.MeasureString(nextText).Y);
            spriteBatch.DrawString(tipFont, nextText, nextTextPosition, Color.Black);

            spriteBatch.End();
        }
    }
}


