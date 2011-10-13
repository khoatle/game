using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SkinnedModel;
using Poseidon.Core;
namespace Poseidon
{
    class MutantShark : CombatEnemy
    {
        private TimeSpan shootingStartTime;
        private float shootingSeconds;

        public MutantShark() : base() {
            shootingStartTime = new TimeSpan();
            shootingSeconds = 4f;
        }

        public override void LoadContent(ContentManager content, string modelName)
        {
            Model = content.Load<Model>(modelName);
            BarrierType = modelName;
            Position = Vector3.Down;
            BoundingSphere = CalculateBoundingSphere();

            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;
            scaledSphere.Radius *= 0.3f;
            BoundingSphere =
                new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);
            isBigBoss = true;
            health = 1000;
            maxHealth = 1000;
            perceptionRadius = GameConstants.BossPerceptionRadius;
            experienceReward = 200; //1000
        }

        public override void Update(SwimmingObject[] enemyList, int enemySize, SwimmingObject[] fishList, int fishSize, int changeDirection, Tank tank, List<DamageBullet> enemyBullets, List<DamageBullet> alliesBullets) {
            if (stunned) return;

            if (currentHuntingTarget != null && PlayGameScene.timming.TotalGameTime.TotalSeconds - shootingStartTime.TotalSeconds >= shootingSeconds) {
                AddingObjects.placeChasingBullet(this, currentHuntingTarget, damage, enemyBullets); 
                shootingStartTime = PlayGameScene.timming.TotalGameTime;
                return;
            }
            
            int perceptionID = perceptAndLock(tank, fishList, fishSize);
            configAction(perceptionID);
            makeAction(changeDirection, enemyList, enemySize, fishList, fishSize, enemyBullets, tank);
        }
    }
}
