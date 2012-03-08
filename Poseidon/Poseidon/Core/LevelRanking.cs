using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Poseidon.Core;
namespace Poseidon
{
    public partial class PlayGameScene : GameScene
    {
        public void LevelRanking(ref string bossDefeatRank, ref string enemyDefeatRank, ref string healthLostRank, ref string fishSaveRank, ref string trashCollectRank, ref string overallRank, ref string comment)
        {
            //boss defeat ranking
            int totalNumberOfBosses = GameConstants.NumberMutantShark[currentLevel] + GameConstants.NumberSubmarine[currentLevel] + GameConstants.NumberTerminator[currentLevel];
            int bossRankPoint = 0;
            if (totalNumberOfBosses > 0)
            {
                float percentBossKill = (float)numBossKills / (float)totalNumberOfBosses;
                if (percentBossKill <= 0.20f)
                {
                    bossDefeatRank = "Novice";
                    bossRankPoint = 1;
                }
                else if (percentBossKill <= 0.40f)
                {
                    bossDefeatRank = "Apprentice";
                    bossRankPoint = 2;
                }
                else if (percentBossKill <= 0.60f)
                {
                    bossDefeatRank = "Adept";
                    bossRankPoint = 3;
                }
                else if (percentBossKill <= 0.80f)
                {
                    bossDefeatRank = "Expert";
                    bossRankPoint = 4;
                }
                else
                {
                    bossDefeatRank = "Exceptional";
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
                    enemyDefeatRank = "Novice";
                    enemyRankPoint = 1;
                }
                else if (percentEnemyKilled <= 0.60f)
                {
                    enemyDefeatRank = "Apprentice";
                    enemyRankPoint = 2;
                }
                else if (percentEnemyKilled <= 0.80f)
                {
                    enemyDefeatRank = "Adept";
                    enemyRankPoint = 3;
                }
                else if (percentEnemyKilled <= 0.95f)
                {
                    enemyDefeatRank = "Expert";
                    enemyRankPoint = 4;
                }
                else
                {
                    enemyDefeatRank = "Exceptional";
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
                    healthLostRank = "Novice";
                    healthLostPoint = 1;
                }
                else if (healthLost >= 1.5 * healthLostNormal)
                {
                    healthLostRank = "Apprentice";
                    healthLostPoint = 2;
                }
                else if (healthLost >= healthLostNormal)
                {
                    healthLostRank = "Adept";
                    healthLostPoint = 3;
                }
                else if (healthLost >= 0.5 * healthLostNormal)
                {
                    healthLostRank = "Expert";
                    healthLostPoint = 4;
                }
                else
                {
                    healthLostRank = "Exceptional";
                    healthLostPoint = 5;
                }

            }
            //fish save ranking
            int fishSavePoint = 0;
            int realNumFish = fishAmount;
            if (HydroBot.hasDolphin) realNumFish -= 1;
            if (HydroBot.hasSeaCow) realNumFish -= 1;
            if (HydroBot.hasTurtle) realNumFish -= 1;

            if (GameConstants.NumberFish[currentLevel] > 0)
            {
                float fishSavePercent = (float)realNumFish / (float)GameConstants.NumberFish[currentLevel];
                if (fishSavePercent <= 0.30f)
                {
                    fishSaveRank = "Novice";
                    fishSavePoint = 1;
                }
                else if (fishSavePercent <= 0.60f)
                {
                    fishSaveRank = "Apprentice";
                    fishSavePoint = 2;
                }
                else if (fishSavePercent <= 0.80f)
                {
                    fishSaveRank = "Adept";
                    fishSavePoint = 3;
                }
                else if (fishSavePercent <= 0.95f)
                {
                    fishSaveRank = "Expert";
                    fishSavePoint = 4;
                }
                else
                {
                    fishSaveRank = "Exceptional";
                    fishSavePoint = 5;
                }
            }

            //trash collect rank
            int trashRankPoint = 0;
            int trashNormalThreshold = 15;
            if (numTrashCollected <= 0.5 * trashNormalThreshold)
            {
                trashCollectRank = "Novice";
                trashRankPoint = 1;
            }
            else if (numTrashCollected <= trashNormalThreshold)
            {
                trashCollectRank = "Apprentice";
                trashRankPoint = 2;
            }
            else if (numTrashCollected <= 1.5 * trashNormalThreshold)
            {
                trashCollectRank = "Adept";
                trashRankPoint = 3;
            }
            else if (numTrashCollected <= 2 * trashNormalThreshold)
            {
                trashCollectRank = "Expert";
                trashRankPoint = 4;
            }
            else
            {
                trashCollectRank = "Exceptional";
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
                overallRank = "Exceptional";
                comment = "Speechless ...";
            }
            else if (overallRankingPoint >= 4)
            {
                overallRank = "Expert";
                comment = "The sea environment really needs someone like you!";
            }
            else if (overallRankingPoint >= 3)
            {
                overallRank = "Adept";
                comment = "You are good enough to save the sea environment.";
            }
            else if (overallRankingPoint >= 2)
            {
                overallRank = "Apprentice";
                comment = "Try harder and you will play better.";
            }
            else if (overallRankingPoint >= 1)
            {
                overallRank = "Novice";
                comment = "Your skill needs to be improved.";
            }
        }
    }
}
