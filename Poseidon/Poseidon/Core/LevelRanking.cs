using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Poseidon.Core;
using Microsoft.Xna.Framework.Graphics;
namespace Poseidon
{
    public partial class PlayGameScene : GameScene
    {
        public void LevelRanking(ref string bossDefeatRank, ref string enemyDefeatRank, ref string healthLostRank, ref string fishSaveRank, ref string trashCollectRank, ref Texture2D overallTexture, ref string comment)
        {
            string lowestRank = "Beginner";
            string lowerRank = "Apprentice";
            string averageRank = "Adept";
            string higherRank = "Expert";
            string highestRank = "Exceptional";
            //boss defeat ranking
            int totalNumberOfBosses = 0;
            if (currentLevel == 11)
                totalNumberOfBosses = GameConstants.NumberMutantShark[currentLevel] + GameConstants.NumberSubmarine[currentLevel] + GameConstants.NumberTerminator[currentLevel];
            else totalNumberOfBosses = GameConstants.NumberMutantShark[currentLevel] + GameConstants.NumberSubmarine[currentLevel];
            int bossRankPoint = 0;
            if (totalNumberOfBosses > 0)
            {
                float percentBossKill = (float)numBossKills / (float)totalNumberOfBosses;
                if (percentBossKill <= 0.20f)
                {
                    bossDefeatRank = lowestRank;
                    bossRankPoint = 1;
                }
                else if (percentBossKill <= 0.40f)
                {
                    bossDefeatRank = lowerRank;
                    bossRankPoint = 2;
                }
                else if (percentBossKill <= 0.60f)
                {
                    bossDefeatRank = averageRank;
                    bossRankPoint = 3;
                }
                else if (percentBossKill <= 0.80f)
                {
                    bossDefeatRank = higherRank;
                    bossRankPoint = 4;
                }
                else
                {
                    bossDefeatRank = highestRank;
                    bossRankPoint = 5;
                }
            }

            //normal enemy defeat ranking
            int totalNumberOfEnemies = GameConstants.NumberShootingEnemies[currentLevel] + GameConstants.NumberCombatEnemies[currentLevel]
                + GameConstants.NumberGhostPirate[currentLevel] + GameConstants.ShipNumberGhostPirate[currentLevel];
            int enemyRankPoint = 0;
            if (totalNumberOfEnemies > 0)
            {
                float percentEnemyKilled = (float)numNormalKills / (float)totalNumberOfEnemies;
                if (percentEnemyKilled <= 0.30f)
                {
                    enemyDefeatRank = lowestRank;
                    enemyRankPoint = 1;
                }
                else if (percentEnemyKilled <= 0.60f)
                {
                    enemyDefeatRank = lowerRank;
                    enemyRankPoint = 2;
                }
                else if (percentEnemyKilled <= 0.80f)
                {
                    enemyDefeatRank = averageRank;
                    enemyRankPoint = 3;
                }
                else if (percentEnemyKilled <= 0.95f)
                {
                    enemyDefeatRank = higherRank;
                    enemyRankPoint = 4;
                }
                else
                {
                    enemyDefeatRank = highestRank;
                    enemyRankPoint = 5;
                }
            }

            //health lost rank
            int healthLostPoint = 0;
            int healthLostNormal = currentLevel * (int)GameConstants.PlayerStartingHP;
            if (healthLostNormal > 0)
            {
                if (healthLost >= 2 * healthLostNormal)
                {
                    healthLostRank = lowestRank;
                    healthLostPoint = 1;
                }
                else if (healthLost >= 1.5 * healthLostNormal)
                {
                    healthLostRank = lowerRank;
                    healthLostPoint = 2;
                }
                else if (healthLost >= healthLostNormal)
                {
                    healthLostRank = averageRank;
                    healthLostPoint = 3;
                }
                else if (healthLost >= 0.5 * healthLostNormal)
                {
                    healthLostRank = higherRank;
                    healthLostPoint = 4;
                }
                else
                {
                    healthLostRank = highestRank;
                    healthLostPoint = 5;
                }

            }
            //fish save ranking
            int fishSavePoint = 0;
            int realNumFish = fishAmount;
            if (HydroBot.hasDolphin) realNumFish -= 1;
            if (HydroBot.hasSeaCow) realNumFish -= 1;
            if (HydroBot.hasTurtle) realNumFish -= 1;

            if (GameConstants.NumberFish[currentLevel] > 0 && totalNumberOfEnemies > 0)
            {
                float fishSavePercent = (float)realNumFish / (float)GameConstants.NumberFish[currentLevel];
                if (fishSavePercent <= 0.30f)
                {
                    fishSaveRank = lowestRank;
                    fishSavePoint = 1;
                }
                else if (fishSavePercent <= 0.60f)
                {
                    fishSaveRank = lowerRank;
                    fishSavePoint = 2;
                }
                else if (fishSavePercent <= 0.80f)
                {
                    fishSaveRank = averageRank;
                    fishSavePoint = 3;
                }
                else if (fishSavePercent <= 0.95f)
                {
                    fishSaveRank = higherRank;
                    fishSavePoint = 4;
                }
                else
                {
                    fishSaveRank = highestRank;
                    fishSavePoint = 5;
                }
            }

            //trash collect rank
            int trashRankPoint = 0;
            int trashNormalThreshold = 12;
            if (currentLevel == 0) trashNormalThreshold *= 2;
            if (numTrashCollected <= 0.5 * trashNormalThreshold)
            {
                trashCollectRank = lowestRank;
                trashRankPoint = 1;
            }
            else if (numTrashCollected <= trashNormalThreshold)
            {
                trashCollectRank = lowerRank;
                trashRankPoint = 2;
            }
            else if (numTrashCollected <= 1.5 * trashNormalThreshold)
            {
                trashCollectRank = averageRank;
                trashRankPoint = 3;
            }
            else if (numTrashCollected <= 2 * trashNormalThreshold)
            {
                trashCollectRank = higherRank;
                trashRankPoint = 4;
            }
            else
            {
                trashCollectRank = highestRank;
                trashRankPoint = 5;
            }

            float overallRankingPoint = 0;
            int numCatergory = 0;
            if (bossRankPoint > 0)
            {
                overallRankingPoint += bossRankPoint;
                numCatergory += 1;
            }
            if (enemyRankPoint > 0)
            {
                overallRankingPoint += enemyRankPoint;
                numCatergory += 1;
            }
            if (healthLostPoint > 0)
            {
                overallRankingPoint += healthLostPoint;
                numCatergory += 1;
            }
            if (fishSavePoint > 0)
            {
                overallRankingPoint += fishSavePoint;
                numCatergory += 1;
            }
            if (trashRankPoint > 0)
            {
                overallRankingPoint += trashRankPoint;
                numCatergory += 1;
            }

            overallRankingPoint /= numCatergory;

            if (overallRankingPoint >= 5)
            {
                overallTexture = rankTextures[4];
                comment = "Speechless ...";
            }
            else if (overallRankingPoint >= 4)
            {
                overallTexture = rankTextures[3];
                comment = "The sea environment really needs someone like you!";
            }
            else if (overallRankingPoint >= 3)
            {
                overallTexture = rankTextures[2];
                comment = "You are good enough to save the sea environment.";
            }
            else if (overallRankingPoint >= 2)
            {
                overallTexture = rankTextures[1];
                comment = "Try harder and you will play better.";
            }
            else if (overallRankingPoint >= 1)
            {
                overallTexture = rankTextures[0];
                comment = "Your skill needs to be improved.";
            }
        }
    }
}
