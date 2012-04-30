#region Using Statements

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using Poseidon.Core;
using System.IO;
using System.Collections.Generic;

#endregion

namespace Poseidon
{
    /// <summary>
    /// This is a game component that implements the Config Scene.
    /// </summary>
    public class ConfigScene : GameScene
    {
        // Textures and their rectangles
        protected readonly Texture2D configTitle, slideBar, dragButton, uncheckedBox, checkedBox, okButton;

        protected Rectangle titleRect;
        protected List<Rectangle> itemRectList;
        protected List<Rectangle> iconRectList;
        protected Rectangle musicVolumeRect;
        protected Rectangle soundVolumeRect;
        protected Rectangle specialEffectRect;
        protected Rectangle numParticleRect;
        protected Rectangle showLiveTipRect;
        protected Rectangle schoolFishRect;
        protected Rectangle easyRect, mediumRect, hardRect;
        protected Rectangle okBox;
        private string[] menuItems;

        //Scale Factors
        float widthScale = 1f;
        float heightScale = 1f;
        float textScale = 1f;

        //Fonts
        SpriteFont regularFont;
        SpriteFont selectedFont;
        SpriteFont numberFont;
        
        //Check mouse position and click
        int selectedIndex = -1;

        MouseState currentMouseState = new MouseState();
        MouseState lastMouseState = new MouseState();
        private bool clicked = false;
        private int clickedPositionX;
        public static bool okClicked = false;


        //Color
        protected Color regularColor = Color.Khaki, selectedColor = Color.FloralWhite;

        // Audio
        protected AudioLibrary audio;
        // Spritebatch
        protected SpriteBatch spriteBatch = null;

        Game game;

        GraphicsDevice graphicsDevice;

        public bool gameStarted = false;

        //for continously playing random background musics
        Random rand = new Random();


        public ConfigScene(Game game, SpriteFont smallFont, SpriteFont largeFont, SpriteFont numberFont,
                            Texture2D background, Texture2D configTitle, Texture2D unselectedCheckbox, Texture2D selectedCheckBox, Texture2D slidebar, Texture2D dragbutton, Texture2D okButton,  GraphicsDevice graphicDevice)
            : base(game)
        {
            this.game = game;
            this.graphicsDevice = graphicDevice;
            regularFont = smallFont;
            selectedFont = largeFont;
            //widthScale = (float)game.Window.ClientBounds.Width / 1280;
            //heightScale = (float)game.Window.ClientBounds.Height / 800;
            textScale = GameConstants.generalTextScaleFactor;//(float)Math.Sqrt((double)(widthScale * heightScale));
            
            uncheckedBox = unselectedCheckbox;
            checkedBox = selectedCheckBox;
            this.okButton = okButton;
            this.slideBar = slidebar;
            this.dragButton = dragbutton;
            this.numberFont = numberFont;


            Components.Add(new ImageComponent(game, background,
                                            ImageComponent.DrawMode.Stretch));

            this.configTitle = configTitle;
            int titleWidth = (int)(330 * widthScale);
            int titleHeight = (int)(80 * heightScale);
            titleRect = new Rectangle(game.Window.ClientBounds.Center.X - titleWidth / 2, game.Window.ClientBounds.Top + titleHeight/3, titleWidth, titleHeight);

            string[] items = { "Music Volume", "Sound Volume", "Live Tutorial", "Special Effect", "Particle Level", "Fish School Size", "Difficulty" };
            menuItems = items;
            itemRectList = new List<Rectangle>(menuItems.Length);
            iconRectList = new List<Rectangle>(menuItems.Length);

            int x, y, width, height, maxItemWidth;
            maxItemWidth = findMaxWidth();
            x = game.Window.ClientBounds.Center.X - (int)( maxItemWidth * 0.7);
            y = titleRect.Bottom+titleHeight/4;
            height = (int)((regularFont.MeasureString(menuItems[0]).Y) * textScale);
            foreach (string menu in menuItems)
            {
                width = (int)(regularFont.MeasureString(menu).X * textScale);
                Rectangle itemRectangle = new Rectangle(x, y, width, height);
                itemRectList.Add(itemRectangle);

                y += (int)(regularFont.LineSpacing*1.2*textScale);
            }

            int iconPositionX = x + maxItemWidth + (int)(10 * widthScale);
            int barheight = (int)(20*heightScale), barwidth = (int)(200*widthScale);
            int checkBoxheight = (int)(30*heightScale) , checkboxWidth = (int)(30*widthScale);

            musicVolumeRect = new Rectangle(iconPositionX, itemRectList[0].Center.Y - barheight / 2, barwidth, barheight);
            iconRectList.Add(musicVolumeRect);
            soundVolumeRect = new Rectangle(iconPositionX, itemRectList[1].Center.Y - barheight / 2, barwidth, barheight);
            iconRectList.Add(soundVolumeRect);
            showLiveTipRect = new Rectangle(iconPositionX+ barwidth/2, itemRectList[2].Center.Y - checkBoxheight/2, checkboxWidth, checkBoxheight);
            iconRectList.Add(showLiveTipRect);
            specialEffectRect = new Rectangle(iconPositionX + barwidth/2, itemRectList[3].Center.Y - checkBoxheight / 2, checkboxWidth, checkBoxheight);
            iconRectList.Add(specialEffectRect);
            numParticleRect = new Rectangle(iconPositionX, itemRectList[4].Center.Y - barheight / 2, barwidth, barheight);
            iconRectList.Add(numParticleRect);
            schoolFishRect = new Rectangle(iconPositionX, itemRectList[5].Center.Y - barheight / 2, barwidth, barheight);
            iconRectList.Add(schoolFishRect);
            easyRect = new Rectangle(iconPositionX, itemRectList[6].Center.Y - checkBoxheight / 2, checkboxWidth, checkBoxheight);
            iconRectList.Add(easyRect);
            mediumRect = new Rectangle(iconPositionX + checkboxWidth * 3, itemRectList[6].Center.Y - checkBoxheight / 2, checkboxWidth, checkBoxheight);
            iconRectList.Add(mediumRect);
            hardRect = new Rectangle(iconPositionX + checkboxWidth * 6, itemRectList[6].Center.Y - checkBoxheight / 2, checkboxWidth, checkBoxheight);
            iconRectList.Add(hardRect);

            width = titleWidth/2;
            height = (int)(titleHeight*0.75);
            okBox = new Rectangle(game.Window.ClientBounds.Center.X - width / 2, game.Window.ClientBounds.Bottom - (int)(height * 1.5), width, height);

            // Get the current spritebatch
            spriteBatch = (SpriteBatch)Game.Services.GetService(
                                            typeof(SpriteBatch));

            // Get the audio library
            audio = (AudioLibrary)
                Game.Services.GetService(typeof(AudioLibrary));
            
            cursor = new Cursor(game, spriteBatch);

            selectedIndex = -1;
        }

        private int findMaxWidth()
        {
            int maxWidth = 0;
            int width;
            foreach (string menu in menuItems)
            {
                width = (int)(regularFont.MeasureString(menu).X * textScale);
                if (width > maxWidth)
                {
                    maxWidth = width;
                }
            }
            return maxWidth;
        }
        /// <summary>
        /// Show the start scene
        /// </summary>
        public override void Show()
        {
            
            audio.NewMeteor.Play();

            cursor.SetMenuCursorImage();
                        
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
        /// Gets the selected menu option
        /// </summary>
        //public int SelectedMenuIndex
        //{
            
        //}

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            int prevselectedIndex = selectedIndex;
            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            int cursorWidthSensivity = (int)(32*widthScale), cursorHeightSensitivity = (int)(32*heightScale);
            int mouseX = currentMouseState.X - (cursorWidthSensivity / 2);
            int mouseY = currentMouseState.Y - (cursorHeightSensitivity / 2);

            for (int i = 0; i < iconRectList.Count; i++)
            {
                if (iconRectList[i].Intersects(new Rectangle(mouseX, mouseY, cursorWidthSensivity, cursorHeightSensitivity)))
                {
                    selectedIndex = i;
                    break;
                }
                selectedIndex = -1;
            }

            if (okBox.Intersects(new Rectangle(mouseX, mouseY, cursorWidthSensivity, cursorHeightSensitivity)))
            {
                selectedIndex = -2; 
            }

            if (prevselectedIndex != selectedIndex && selectedIndex!= -1)
            {
                audio.MenuScroll.Play();
            }

            if (selectedIndex == 2 || selectedIndex == 3 ||selectedIndex > 5) //checkbox
            {
                if (iconRectList[selectedIndex].Intersects(new Rectangle(mouseX, mouseY, cursorWidthSensivity, cursorHeightSensitivity)) && lastMouseState.LeftButton.Equals(ButtonState.Pressed) && currentMouseState.LeftButton.Equals(ButtonState.Released))
                {
                    clickedPositionX = currentMouseState.X;
                    clicked = true;
                }
            }
            else if (selectedIndex >= 0) //Bars -- just drag
            {
                if (iconRectList[selectedIndex].Intersects(new Rectangle(mouseX, mouseY, cursorWidthSensivity, cursorHeightSensitivity)) && currentMouseState.LeftButton.Equals(ButtonState.Pressed))
                {
                    clickedPositionX = currentMouseState.X;
                    clicked = true;
                }
            }

            if (clicked)
            {
                switch (selectedIndex)
                {
                    case 0:
                        GameSettings.MusicVolume = (float)(clickedPositionX - musicVolumeRect.Left) / (float)musicVolumeRect.Width;
                        if (GameSettings.MusicVolume < 0f) GameSettings.MusicVolume = 0f;
                        if (GameSettings.MusicVolume > 1f) GameSettings.MusicVolume = 1f;
                        MediaPlayer.Volume = GameSettings.MusicVolume;
                        break;
                    case 1:
                        GameSettings.SoundVolume = (float)(clickedPositionX - soundVolumeRect.Left) / (float)soundVolumeRect.Width;
                        if (GameSettings.SoundVolume < 0f) GameSettings.SoundVolume = 0f;
                        if (GameSettings.SoundVolume > 1f) GameSettings.SoundVolume = 1f;
                        SoundEffect.MasterVolume = GameSettings.SoundVolume;
                        PoseidonGame.videoPlayer.Volume = GameSettings.SoundVolume;
                        break;
                    case 2:
                        if (GameSettings.ShowLiveTip)
                            GameSettings.ShowLiveTip = false;
                        else
                            GameSettings.ShowLiveTip = true;
                        break;
                    case 3:
                        if (GameSettings.SpecialEffectsEnabled)
                            GameSettings.SpecialEffectsEnabled = false;
                        else
                            GameSettings.SpecialEffectsEnabled = true;
                        break;
                    case 4:
                        GameSettings.NumParticleLevel = (float)(clickedPositionX - numParticleRect.Left) / (float)numParticleRect.Width;
                        if (GameSettings.NumParticleLevel < 0f) GameSettings.NumParticleLevel = 0f;
                        if (GameSettings.NumParticleLevel > 1f) GameSettings.NumParticleLevel = 1f;
                        GameConstants.numExplosionParticles = (int) (GameConstants.DefaultNumExplosionParticles * GameSettings.NumParticleLevel);
                        GameConstants.numExplosionSmallParticles = (int)(GameConstants.numExplosionSmallParticles * GameSettings.NumParticleLevel);
                        GameConstants.numSandParticles = (int)(GameConstants.DefaultNumSandParticles * GameSettings.NumParticleLevel);
                        GameConstants.numSandParticlesForFactory = (int)(GameConstants.DefaultNumSandParticlesForFactory * GameSettings.NumParticleLevel);
                        GameConstants.trailParticlesPerSecond = (int)(GameConstants.DefaultTrailParticlesPerSecond * GameSettings.NumParticleLevel);
                        GameConstants.numFrozenBreathParticlesPerUpdate = (int)(GameConstants.DefaultNumFrozenBreathParticlesPerUpdate * GameSettings.NumParticleLevel);
                        break;
                    case 5:
                        GameSettings.SchoolOfFishDetail = (float)(clickedPositionX - schoolFishRect.Left) / (float)schoolFishRect.Width;
                        if (GameSettings.SchoolOfFishDetail < 0f) GameSettings.SchoolOfFishDetail = 0f;
                        if (GameSettings.SchoolOfFishDetail > 1f) GameSettings.SchoolOfFishDetail = 1f;
                        break;
                    case 6:
                        GameSettings.DifficultyLevel = 1; //Easy
                        PoseidonGame.setDifficulty(1);
                        PlayGameScene.cutSceneDialog = new CutSceneDialog();
                        break;
                    case 7:
                        GameSettings.DifficultyLevel = 2; //Medium
                        PoseidonGame.setDifficulty(2);
                        PlayGameScene.cutSceneDialog = new CutSceneDialog();
                        break;
                    case 8:
                        GameSettings.DifficultyLevel = 3; //Hard
                        PoseidonGame.setDifficulty(3);
                        PlayGameScene.cutSceneDialog = new CutSceneDialog();
                        break;
                    default:
                        break;
                }
                clicked = false;
            }

            if (okBox.Intersects(new Rectangle(mouseX, mouseY, cursorWidthSensivity, cursorHeightSensitivity)) && lastMouseState.LeftButton.Equals(ButtonState.Pressed) && currentMouseState.LeftButton.Equals(ButtonState.Released))
            {
                okClicked = true;
            }

            if (MediaPlayer.State.Equals(MediaState.Stopped))
            {

                MediaPlayer.Play(audio.mainMenuMusic);
            }

            cursor.Update(graphicsDevice, PlayGameScene.gameCamera, gameTime, null);
            base.Update(gameTime);
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            base.Draw(gameTime);
            
            //Draw Title
            spriteBatch.Draw(configTitle, titleRect, Color.White);
            
            //Draw the item text
            for(int i=0; i<itemRectList.Count; i++ )
            {
                if (i == selectedIndex || (i==6 && selectedIndex>6))
                {
                    spriteBatch.DrawString(selectedFont, menuItems[i], new Vector2(itemRectList[i].Left + 1, itemRectList[i].Top + 1), selectedColor, 0f, new Vector2(0, 0), textScale, SpriteEffects.None, 0f); //shadow
                    spriteBatch.DrawString(selectedFont, menuItems[i], new Vector2(itemRectList[i].Left, itemRectList[i].Top), selectedColor, 0f, new Vector2(0, 0), textScale, SpriteEffects.None, 0f);
                }
                else
                {
                    spriteBatch.DrawString(regularFont, menuItems[i], new Vector2(itemRectList[i].Left + 1, itemRectList[i].Top + 1), regularColor, 0f, new Vector2(0, 0), textScale, SpriteEffects.None, 0f); //shadow
                    spriteBatch.DrawString(regularFont, menuItems[i], new Vector2(itemRectList[i].Left, itemRectList[i].Top), regularColor, 0f, new Vector2(0, 0), textScale, SpriteEffects.None, 0f);
                }
            }
            

            //Draw the checkbox and bars
            //Music Volume
            spriteBatch.Draw(slideBar, musicVolumeRect, Color.White);
            int buttonDiameter = musicVolumeRect.Height;
            float numberScale = 0.8f * textScale;
            int buttonPositionX = musicVolumeRect.Left + (int)(GameSettings.MusicVolume*musicVolumeRect.Width) - (buttonDiameter/2);
            spriteBatch.Draw(dragButton, new Rectangle(buttonPositionX, musicVolumeRect.Top, buttonDiameter, buttonDiameter), Color.White) ;
            spriteBatch.DrawString(numberFont, GameSettings.MusicVolume.ToString("P0"), new Vector2(buttonPositionX, musicVolumeRect.Top-(numberFont.MeasureString("1").Y * numberScale)), selectedColor, 0f, new Vector2(0, 0), numberScale, SpriteEffects.None, 0f);

            //Sound
            spriteBatch.Draw(slideBar, soundVolumeRect, Color.White);
            buttonDiameter = soundVolumeRect.Height;
            buttonPositionX = soundVolumeRect.Left + (int)(GameSettings.SoundVolume * soundVolumeRect.Width) - (buttonDiameter / 2);
            spriteBatch.Draw(dragButton, new Rectangle(buttonPositionX, soundVolumeRect.Top, buttonDiameter, buttonDiameter), Color.White);
            spriteBatch.DrawString(numberFont, GameSettings.SoundVolume.ToString("P0"), new Vector2(buttonPositionX, soundVolumeRect.Top - (numberFont.MeasureString("1").Y * numberScale)), selectedColor, 0f, new Vector2(0, 0), numberScale, SpriteEffects.None, 0f);

            //Show Tip CheckBox
            if(GameSettings.ShowLiveTip)
                spriteBatch.Draw(checkedBox, showLiveTipRect, Color.White);
            else
                spriteBatch.Draw(uncheckedBox, showLiveTipRect, Color.White);

            //Special Effect CheckBox
            if (GameSettings.SpecialEffectsEnabled)
                spriteBatch.Draw(checkedBox, specialEffectRect, Color.White);
            else
                spriteBatch.Draw(uncheckedBox, specialEffectRect, Color.White);
 
            //Num Particle Bar
            spriteBatch.Draw(slideBar, numParticleRect , Color.White);
            buttonDiameter = numParticleRect.Height;
            buttonPositionX = numParticleRect.Left + (int)(GameSettings.NumParticleLevel * numParticleRect.Width) - (buttonDiameter / 2);
            spriteBatch.Draw(dragButton, new Rectangle(buttonPositionX, numParticleRect.Top, buttonDiameter, buttonDiameter), Color.White);
            spriteBatch.DrawString(numberFont, GameSettings.NumParticleLevel.ToString("P0"), new Vector2(buttonPositionX, numParticleRect.Top - (numberFont.MeasureString("1").Y*numberScale)), selectedColor, 0f, new Vector2(0, 0), numberScale, SpriteEffects.None, 0f);

            //School Size
            spriteBatch.Draw(slideBar, schoolFishRect, Color.White);
            buttonDiameter = schoolFishRect.Height;
            buttonPositionX = schoolFishRect.Left + (int)(GameSettings.SchoolOfFishDetail * schoolFishRect.Width) - (buttonDiameter / 2);
            spriteBatch.Draw(dragButton, new Rectangle(buttonPositionX, schoolFishRect.Top, buttonDiameter, buttonDiameter), Color.White);
            spriteBatch.DrawString(numberFont, GameSettings.SchoolOfFishDetail.ToString("P0"), new Vector2(buttonPositionX, schoolFishRect.Top - (numberFont.MeasureString("1").Y*numberScale)), selectedColor, 0f, new Vector2(0, 0), numberScale, SpriteEffects.None, 0f);

            //Easy Difficulty Level
            if (GameSettings.DifficultyLevel == 1)
                spriteBatch.Draw(checkedBox, easyRect, Color.White);
            else
                spriteBatch.Draw(uncheckedBox, easyRect, Color.White);
            spriteBatch.DrawString(numberFont, "EASY", new Vector2(easyRect.Center.X - numberFont.MeasureString("EASY").X/2, easyRect.Bottom + 5), selectedColor, 0f, new Vector2(0, 0), textScale, SpriteEffects.None, 0f);

            //Medium Difficulty Level
            if (GameSettings.DifficultyLevel == 2)
                spriteBatch.Draw(checkedBox, mediumRect, Color.White);
            else
                spriteBatch.Draw(uncheckedBox, mediumRect, Color.White);
            spriteBatch.DrawString(numberFont, "MEDIUM", new Vector2(mediumRect.Center.X - numberFont.MeasureString("MEDIUM").X / 2, mediumRect.Bottom + 5), selectedColor, 0f, new Vector2(0, 0), textScale, SpriteEffects.None, 0f);

            //Hard Difficulty Level
            if (GameSettings.DifficultyLevel == 3)
                spriteBatch.Draw(checkedBox, hardRect, Color.White);
            else
                spriteBatch.Draw(uncheckedBox, hardRect, Color.White);
            spriteBatch.DrawString(numberFont, "HARD", new Vector2(hardRect.Center.X - numberFont.MeasureString("HARD").X / 2, hardRect.Bottom + 5), selectedColor, 0f, new Vector2(0, 0), textScale, SpriteEffects.None, 0f);


            //draw OK button
            if(selectedIndex==-2) //mouse on Ok
                spriteBatch.Draw(okButton, okBox, Color.DarkRed);
            else
                spriteBatch.Draw(okButton, okBox, Color.Khaki);

            cursor.Draw(gameTime);
            spriteBatch.End();
        }
    }
}