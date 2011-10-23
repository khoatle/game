﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Poseidon.FishSchool
{
    #region FlockingAIParameters
    public struct AIParameters
    {
        /// <summary>
        /// how far away the animals see each other
        /// </summary>
        public float DetectionDistance;
        /// <summary>
        /// seperate from animals inside this distance
        /// </summary>
        public float SeparationDistance;
        /// <summary>
        /// how much the animal tends to move in it's previous direction
        /// </summary>
        public float MoveInOldDirectionInfluence;
        /// <summary>
        /// how much the animal tends to move with animals in it's detection distance
        /// </summary>
        public float MoveInFlockDirectionInfluence;
        /// <summary>
        /// how much the animal tends to move randomly
        /// </summary>
        public float MoveInRandomDirectionInfluence;
        /// <summary>
        /// how quickly the animal can turn
        /// </summary>
        public float MaxTurnRadians;
        /// <summary>
        /// how much each nearby animal influences it's behavior
        /// </summary>
        public float PerMemberWeight;
        /// <summary>
        /// how much dangerous animals influence it's behavior
        /// </summary>
        public float PerDangerWeight;
    }
    #endregion
    public class SchoolOfFish
    {
        // Default value for the AI parameters
        const float detectionDefault = 70.0f;
        const float separationDefault = 50.0f;
        const float moveInOldDirInfluenceDefault = 1.0f;
        const float moveInFlockDirInfluenceDefault = 1.0f;
        const float moveInRandomDirInfluenceDefault = 0.05f;
        const float maxTurnRadiansDefault = 6.0f;
        const float perMemberWeightDefault = 1.0f;
        const float perDangerWeightDefault = 50.0f;

        // Do we need to update AI parameers this Update
        bool aiParameterUpdate = false;
        Texture2D birdTexture;

        Flock flock;

        AIParameters flockParams;

        public SchoolOfFish(ContentManager Content)
        {
            flock = null;
            //cat = null;

            flockParams = new AIParameters();
            ResetAIParams();
            birdTexture = Content.Load<Texture2D>("Image/smallfish");
        }

        public void Update(GameTime gameTime)
        {
            if (flock != null)
            {
                flock.Update(gameTime);//, cat);
            }
            else
            {
                SpawnFlock();
            }
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (flock != null)
            {
                flock.Draw(spriteBatch, gameTime);
            }
        }
        /// <summary>
        /// Create the bird flock
        /// </summary>
        /// <param name="theNum"></param>
        protected void SpawnFlock()
        {
            if (flock == null)
            {
                flock = new Flock(birdTexture, GameConstants.MainGameMaxRangeX,
                                  GameConstants.MainGameMaxRangeZ, flockParams);
            }
        }
        /// <summary>
        /// Reset flock AI parameters
        /// </summary>
        private void ResetAIParams()
        {
            flockParams.DetectionDistance = detectionDefault;
            flockParams.SeparationDistance = separationDefault;
            flockParams.MoveInOldDirectionInfluence = moveInOldDirInfluenceDefault;
            flockParams.MoveInFlockDirectionInfluence = moveInFlockDirInfluenceDefault;
            flockParams.MoveInRandomDirectionInfluence = moveInRandomDirInfluenceDefault;
            flockParams.MaxTurnRadians = maxTurnRadiansDefault;
            flockParams.PerMemberWeight = perMemberWeightDefault;
            flockParams.PerDangerWeight = perDangerWeightDefault;
        }
    }
}
