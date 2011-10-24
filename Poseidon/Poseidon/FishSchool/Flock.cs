#region File Description
//-----------------------------------------------------------------------------
// Flock.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace Poseidon.FishSchool
{
    /// <summary>
    /// This class manages all the fishes in the flock and handles 
    /// their update and draw
    /// </summary>
    public class Flock
    {
        #region Constants
        //Number of FLock members
        int flockSize = GameConstants.FishInSchool[PlayGameScene.currentLevel];
        #endregion

        #region Fields

        //fishes that swim out of the boundry(screen) will wrap around to 
        //the other side
        int boundryWidth;
        int boundryHeight;

        /// <summary>
        /// Tecture used to draw the Flock
        /// </summary>
        Texture2D fishTexture;

        /// <summary>
        /// List of Flock Members
        /// </summary>
        public List<Fish> flock;

        /// <summary>
        /// Parameters flock members use to move and think
        /// </summary>
        public AIParameters FlockParams
        {
            get
            {
                return FlockParams;
            }

            set
            {
                flockParams = value;
            }
        }
        protected AIParameters flockParams;


        #endregion

        #region Initialization

        /// <summary>
        /// Setup the flock boundaries and generate individual members of the flock
        /// </summary>
        /// <param name="tex"> The texture to be used by the fishes</param>
        /// <param name="screenWidth">Width of the screen</param>
        /// <param name="screenHeight">Height of the screen</param>
        /// <param name="flockParameters">Behavior of the flock</param>
        public Flock(Texture2D tex, int gameMaxX, int gameMaxZ,
            AIParameters flockParameters, ContentManager content, int minX, int maxX, int minZ, int maxZ)
        {
            boundryWidth = gameMaxX;
            boundryHeight = gameMaxZ;

            fishTexture = tex;

            flock = new List<Fish>();
            flockParams = flockParameters;

            ResetFlock(content, minX, maxX, minZ, maxZ);
        }
        #endregion

        #region Update and Draw
        /// <summary>
        /// Update each flock member, Each fish want to swim with or flee from everything
        /// it sees depending on what type it is
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="cat"></param>
        public void Update(GameTime gameTime, Tank tank, SwimmingObject[] enemies, int enemyAmount, SwimmingObject[] fishes, int fishAmount)//, Cat cat)
        {
            foreach (Fish thisFish in flock)
            {
                thisFish.ResetThink();

                foreach (Fish otherFish in flock)
                {
                    //this check is so we don't try to fly to ourself!
                    if (thisFish != otherFish)
                    {
                        thisFish.ReactTo(otherFish, ref flockParams);
                    }
                }

                //Look for the main character
                thisFish.ReactToMainCharacter(tank, ref flockParams);
                //React to enemies and other big fishes
                for (int i = 0; i < enemyAmount; i++)
                {
                    thisFish.ReactToSwimmingObject(enemies[i], ref flockParams);
                }
                for (int i = 0; i < fishAmount; i++)
                {
                    thisFish.ReactToSwimmingObject(fishes[i], ref flockParams);
                }
                thisFish.Update(gameTime, ref flockParams);
            }
        }

        /// <summary>
        /// Calls Draw on every member of the Flock
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //BoundingSphere boundingSphere;
            foreach (Fish theFish in flock)
            {
                //boundingSphere = new BoundingSphere(theBird.Location, 1.0f);
                //if (PlayGameScene.frustum.Intersects(boundingSphere))
                    theFish.Draw(spriteBatch, gameTime);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clear the current flock if it exists and randomly generate a new one
        /// </summary>
        public void ResetFlock(ContentManager content, int minX, int maxX, int minZ, int maxZ)
        {
            flock.Clear();
            flock.Capacity = flockSize;

            Fish tempFish;
            Vector3 tempDir;
            Vector3 tempLoc;

            Random random = new Random();
            int xValue, zValue;

            for (int i = 0; i < flockSize; i++)
            {
                //xValue = random.Next(80, boundryWidth - 100);
                //zValue = random.Next(80, boundryHeight - 100);
                xValue = random.Next(minX, maxX);
                zValue = random.Next(minZ, maxZ);
                //if (random.Next(100) % 2 == 0)
                //    xValue *= -1;
                //if (random.Next(100) % 2 == 0)
                //    zValue *= -1;
                //give the fishes some chance to get close to eachother from the beginning
                //tempLoc = new Vector3((float)
                //    random.Next(-boundryWidth + 200, boundryWidth - 200), 0, (float)random.Next(-boundryHeight + 200, boundryHeight - 200));
                tempLoc = new Vector3((float)xValue, 0, (float)zValue);
                tempDir = new Vector3((float)random.NextDouble() - 0.5f, 0, (float)random.NextDouble() - 0.5f);
                //tempDir = new Vector3(0, 0, 0);
                tempDir.Normalize();

                //if (random.Next(2) == 0) fishTexture = content.Load<Texture2D>("Image/smallfish1");
                //else fishTexture = content.Load<Texture2D>("Image/smallfish2-1");
                tempFish = new Fish(fishTexture, tempDir, tempLoc,
                    boundryWidth, boundryHeight);
                flock.Add(tempFish);
            }
        }
        #endregion
    }
}
