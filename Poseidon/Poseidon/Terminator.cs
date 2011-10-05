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
    class Terminator : Enemy
    {
        Matrix[] bones;
        SkinningData skd;
        ClipPlayer clipPlayer;
        Matrix fishMatrix;
        Quaternion qRotation = Quaternion.Identity;
        double timePrevPowerUsed = 0;
        //this boss will fire 3 bullets at once for 3 seconds
        bool enragedMode = false;
        double timeEnrageLast = 3;
        public void Load()
        {
            skd = Model.Tag as SkinningData;
            clipPlayer = new ClipPlayer(skd, 60);//ClipPlayer running at 24 frames/sec
            AnimationClip clip = skd.AnimationClips["Take 001"]; //Take name from the dude.fbx file
            clipPlayer.play(clip, 99, 124, true);
            fishMatrix = Matrix.CreateScale(0.1f) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                               Matrix.CreateTranslation(Position);
            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;
            scaledSphere.Radius *= 0.04f;
            BoundingSphere =
                new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);
            isBigBoss = true;
        }

        public override void Update(SwimmingObject[] enemyList, int enemySize, SwimmingObject[] fishList, int fishSize, int changeDirection, Tank tank, List<DamageBullet> bullets)
        {
            if (!stunned)
            {
                int perceptionID = perceptAndLock(tank, fishList, fishSize);
                configAction(perceptionID);
                makeAction(changeDirection, enemyList, enemySize, fishList, fishSize, bullets, tank);
            }
            qRotation = Quaternion.CreateFromAxisAngle(
                            Vector3.Up,
                            ForwardDirection);
            fishMatrix = Matrix.CreateScale(0.1f) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                                Matrix.CreateFromQuaternion(qRotation) *
                                Matrix.CreateTranslation(Position);
            clipPlayer.update(PlayGameScene.timming.ElapsedGameTime, true, fishMatrix);
        }
        public override void Draw(Matrix view, Matrix projection)
        {
            bones = clipPlayer.GetSkinTransforms();

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {

                    effect.SetBoneTransforms(bones);
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }

        }
        public void RapidFire(List<DamageBullet> bullets)
        {
            
            if (PlayGameScene.timming.TotalGameTime.TotalSeconds - prevFire.TotalSeconds > timeBetweenFire)
            {
                float originalForwardDir = ForwardDirection;
                ForwardDirection += MathHelper.PiOver4 / 4;
                AddingObjects.placeEnemyBullet(this, GameConstants.DefaultEnemyDamage, bullets);
                PlayGameScene.audio.Shooting.Play();
                ForwardDirection -= MathHelper.PiOver4 / 2;
                AddingObjects.placeEnemyBullet(this, GameConstants.DefaultEnemyDamage, bullets);
                PlayGameScene.audio.Shooting.Play();
                ForwardDirection = originalForwardDir;
                AddingObjects.placeEnemyBullet(this, GameConstants.DefaultEnemyDamage, bullets);
                PlayGameScene.audio.Shooting.Play();
                prevFire = PlayGameScene.timming.TotalGameTime;

            }
        }
        protected override void makeAction(int changeDirection, SwimmingObject[] enemies, int enemiesAmount, SwimmingObject[] fishes, int fishAmount, List<DamageBullet> bullets, Tank tank)
        {
            if (configBits[0] == true)
            {
                randomWalk(changeDirection, enemies, enemiesAmount, fishes, fishAmount, tank);
                return;
            }
            if (currentHuntingTarget != null)
            {
                calculateHeadingDirection();
            }
            if (configBits[2] == true)
            {
                goStraight(enemies, enemiesAmount, fishes, fishAmount, tank);
            }
            if (configBits[3] == true)
            {
                startChasingTime = PlayGameScene.timming.TotalGameTime;

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
                    RapidFire(bullets);
                    
                    if (PlayGameScene.timming.TotalGameTime.TotalSeconds - timePrevPowerUsed > timeEnrageLast)
                        enragedMode = false;
                }
                else if (PlayGameScene.timming.TotalGameTime.TotalSeconds - timePrevPowerUsed > 10)
                {
                    enragedMode = true;
                    PlayGameScene.audio.MinigunWindUp.Play();
                    timePrevPowerUsed = PlayGameScene.timming.TotalGameTime.TotalSeconds;
                }
                else if (PlayGameScene.timming.TotalGameTime.TotalSeconds - prevFire.TotalSeconds > timeBetweenFire)
                {
                    AddingObjects.placeEnemyBullet(this, GameConstants.DefaultEnemyDamage, bullets);
                    prevFire = PlayGameScene.timming.TotalGameTime;
                }
            }
        } 
    }
}
