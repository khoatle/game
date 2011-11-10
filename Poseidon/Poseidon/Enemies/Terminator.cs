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
    class Terminator : ShootingEnemy
    {
        //Matrix[] bones;
        //SkinningData skd;
        //ClipPlayer clipPlayer;
        //Matrix fishMatrix;
        //Quaternion qRotation = Quaternion.Identity;
        double timePrevPowerUsed = 0;
        //this boss will fire 3 bullets at once for 3 seconds
        bool enragedMode = false;
        bool crazyMode = false;
        bool chasingBulletMode = false;
        double timeChasingBulletLast = 1;

        double timeEnrageLast = 3;
        protected double timeLastLaugh = 0;
        Random random;
        int powerupsType;

        public Terminator()
            : base()
        {
            speed = (float)(GameConstants.EnemySpeed * 1.2);
            damage = GameConstants.DefaultEnemyDamage * 5;
            timeBetweenFire = 0.3f;
            isBigBoss = true;
            random = new Random();
            //Terminator is undefeatable before the last level
            if (PlayGameScene.currentLevel == 10)
            {
                health = 10000;
                maxHealth = 10000;
            }
            else
            {
                health = 1000000;
                maxHealth = 1000000;
            }
            perceptionRadius = GameConstants.BossPerceptionRadius;
            experienceReward = 3000;
        }

        public override void Load(int clipStart, int clipEnd, int fps)
        {
            skd = Model.Tag as SkinningData;
            clipPlayer = new ClipPlayer(skd, fps);//ClipPlayer running at 24 frames/sec
            AnimationClip clip = skd.AnimationClips["Take 001"]; //Take name from the dude.fbx file
            clipPlayer.play(clip, clipStart, clipEnd, true);
            enemyMatrix = Matrix.CreateScale(0.2f) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                               Matrix.CreateTranslation(Position);
            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;
            scaledSphere.Radius *= 0.08f;
            BoundingSphere =
                new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);
        }


        public override void Update(SwimmingObject[] enemyList, int enemySize, SwimmingObject[] fishList, int fishSize, int changeDirection, HydroBot hydroBot, List<DamageBullet> enemyBullets, List<DamageBullet> alliesBullets, BoundingFrustum cameraFrustum, GameTime gameTime, int scene) //scene 1:playgame 2:shipwreck
        {
            qRotation = Quaternion.CreateFromAxisAngle(
                            Vector3.Up,
                            ForwardDirection);
            enemyMatrix = Matrix.CreateScale(0.2f) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                                Matrix.CreateFromQuaternion(qRotation) *
                                Matrix.CreateTranslation(Position);
            clipPlayer.update(gameTime.ElapsedGameTime, true, enemyMatrix);
            // do not delete this
            if (stunned) return;

            if (isHypnotise && gameTime.TotalGameTime.TotalSeconds - startHypnotiseTime.TotalSeconds > GameConstants.timeHypnotiseLast)
            {
                wearOutHypnotise();
            }

            if (!isHypnotise)
            {
                int perceptionID = perceptAndLock(hydroBot, fishList, fishSize);
                configAction(perceptionID, gameTime);
                makeAction(changeDirection, enemyList, enemySize, fishList, fishSize, enemyBullets, hydroBot, cameraFrustum, gameTime);
            }
            else
            {
                int perceptionID = perceptAndLock(hydroBot, enemyList, enemySize);
                configAction(perceptionID, gameTime);
                makeAction(changeDirection, enemyList, enemySize, fishList, fishSize, alliesBullets, hydroBot, cameraFrustum, gameTime);
            }
        }

        //public override void Draw(Matrix view, Matrix projection)
        //{
        //    bones = clipPlayer.GetSkinTransforms();

        //    foreach (ModelMesh mesh in Model.Meshes)
        //    {
        //        foreach (SkinnedEffect effect in mesh.Effects)
        //        {

        //            effect.SetBoneTransforms(bones);
        //            effect.View = view;
        //            effect.Projection = projection;
        //        }
        //        mesh.Draw();
        //    }

        //}

        public void RapidFire(List<DamageBullet> bullets, BoundingFrustum cameraFrustum, GameTime gameTime)
        {
            if (this.BoundingSphere.Intersects(cameraFrustum) && gameTime.TotalGameTime.TotalSeconds - timeLastLaugh > 10)
            {
                PoseidonGame.audio.bossLaugh.Play();
                timeLastLaugh = gameTime.TotalGameTime.TotalSeconds;
            }
            if (gameTime.TotalGameTime.TotalSeconds - prevFire.TotalSeconds > timeBetweenFire)
            {
                float originalForwardDir = ForwardDirection;
                ForwardDirection += MathHelper.PiOver4 / 4;
                AddingObjects.placeEnemyBullet(this, damage, bullets, 1, cameraFrustum, 20);
                ForwardDirection -= MathHelper.PiOver4 / 2;
                AddingObjects.placeEnemyBullet(this, damage, bullets, 1, cameraFrustum, 20);
                ForwardDirection = originalForwardDir;
                AddingObjects.placeEnemyBullet(this, damage, bullets, 1, cameraFrustum, 20);
                prevFire = gameTime.TotalGameTime;
            }
        }

        public void RapidFire2(List<DamageBullet> bullets, BoundingFrustum cameraFrustum, GameTime gameTime)
        {
            if (this.BoundingSphere.Intersects(cameraFrustum) && gameTime.TotalGameTime.TotalSeconds - timeLastLaugh > 10)
            {
                PoseidonGame.audio.bossLaugh.Play();
                timeLastLaugh = gameTime.TotalGameTime.TotalSeconds;
            }
            if (gameTime.TotalGameTime.TotalSeconds - prevFire.TotalSeconds > timeBetweenFire / 3)
            {
                AddingObjects.placeEnemyBullet(this, damage, bullets, 1, cameraFrustum, 20);
                prevFire = gameTime.TotalGameTime;
            }
        }

        public void ChasingBullet(List<DamageBullet> bullets, BoundingFrustum cameraFrustum, GameTime gameTime) { 
            if (this.BoundingSphere.Intersects(cameraFrustum) && gameTime.TotalGameTime.TotalSeconds - timeLastLaugh > 10) {
                PoseidonGame.audio.bossLaugh.Play();
                timeLastLaugh = gameTime.TotalGameTime.TotalSeconds;
            }

            if (gameTime.TotalGameTime.TotalSeconds - prevFire.TotalSeconds > timeBetweenFire * 3)
            {
                AddingObjects.placeChasingBullet(this, currentHuntingTarget, bullets, cameraFrustum);
                prevFire = gameTime.TotalGameTime;
            }
        }

        protected override void makeAction(int changeDirection, SwimmingObject[] enemies, int enemiesAmount, SwimmingObject[] fishes, int fishAmount, List<DamageBullet> bullets, HydroBot hydroBot, BoundingFrustum cameraFrustum, GameTime gameTime)
        {
            if (configBits[0] == true)
            {
                randomWalk(changeDirection, enemies, enemiesAmount, fishes, fishAmount, hydroBot);
                return;
            }
            if (currentHuntingTarget != null)
            {
                calculateHeadingDirection();
            }
            if (configBits[2] == true)
            {
                goStraight(enemies, enemiesAmount, fishes, fishAmount, hydroBot);
            }
            if (configBits[3] == true)
            {
                startChasingTime = gameTime.TotalGameTime;

                if (currentHuntingTarget.GetType().Name.Equals("Fish"))
                {
                    Fish tmp = (Fish)currentHuntingTarget;
                    if (tmp.health <= 0)
                    {
                        currentHuntingTarget = null;
                    }
                }
                if (enragedMode == true)
                {
                    RapidFire(bullets, cameraFrustum, gameTime);

                    if (gameTime.TotalGameTime.TotalSeconds - timePrevPowerUsed > timeEnrageLast)
                        enragedMode = false;
                }
                else if (crazyMode == true)
                {
                    RapidFire2(bullets, cameraFrustum, gameTime);
                    if (gameTime.TotalGameTime.TotalSeconds - timePrevPowerUsed > timeEnrageLast)
                        crazyMode = false;
                }
                else if (chasingBulletMode == true) {
                    ChasingBullet(bullets, cameraFrustum, gameTime);
                    if (gameTime.TotalGameTime.TotalSeconds - timePrevPowerUsed > timeChasingBulletLast)
                        chasingBulletMode = false;
                }
                else if (gameTime.TotalGameTime.TotalSeconds - timePrevPowerUsed > 5)
                {
                    powerupsType = random.Next(3);
                    if (powerupsType == 0)
                        enragedMode = true;
                    else if (powerupsType == 1)
                        crazyMode = true;
                    else if (powerupsType == 2)
                    {
                        chasingBulletMode = true;
                    }
                    //PlayGameScene.audio.MinigunWindUp.Play();
                    timePrevPowerUsed = gameTime.TotalGameTime.TotalSeconds;
                }
                else if (gameTime.TotalGameTime.TotalSeconds - prevFire.TotalSeconds > timeBetweenFire)
                {
                    //ChasingBullet(bullets, cameraFrustum, gameTime);
                    //AddingObjects.placeChasingBullet(this, currentHuntingTarget, bullets, cameraFrustum);
                    AddingObjects.placeEnemyBullet(this, damage, bullets, 1, cameraFrustum, 20);
                    prevFire = gameTime.TotalGameTime;
                }
            }
        }
    }
}
