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
    class GhostPirate : CombatEnemy
    {
        public GhostPirate()
            : base()
        {
            speed *= 1.5f;
            damage = (int)(GameConstants.CombatEnemyDamage * 1.5);
            health = maxHealth = GameConstants.DefaultEnemyHP * 2;
            basicExperienceReward = 120;
            if (PoseidonGame.gamePlus)
            {
                health *= (HydroBot.gamePlusLevel + 1);
                damage *= (HydroBot.gamePlusLevel + 1);
                basicExperienceReward *= (HydroBot.gamePlusLevel + 1);
            }
            maxHealth = health;
            
        }

        public override void Load(int clipStart, int clipEnd, int fpsRate)
        {
            modelScale = boundingSphereScale = 0.25f;
            skd = Model.Tag as SkinningData;
            clipPlayer = new ClipPlayer(skd, fpsRate);//ClipPlayer running at 24 frames/sec
            AnimationClip clip = skd.AnimationClips["Take 001"]; //Take name from the dude.fbx file
            clipPlayer.play(clip, clipStart, clipEnd, true);
            enemyMatrix = Matrix.CreateScale(modelScale) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                               Matrix.CreateTranslation(Position);
            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;
            scaledSphere.Radius *= boundingSphereScale;
            BoundingSphere =
                new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);

            // Set up the parameters
            SetupShaderParameters(PoseidonGame.contentManager, Model);
        }

        public override void LoadContent(ContentManager content, string modelName)
        {
            Model = content.Load<Model>(modelName);
            BarrierType = modelName;
            Position = Vector3.Down;
            BoundingSphere = CalculateBoundingSphere();
            this.Load(10, 40, 24);
        }

        public override void ChangeBoundingSphere()
        {
            //BoundingSphere.Center += new Vector3(20,0,0);
        }

        /* scene: 1-playgamescene 2-shipwreckscene */
        public override void Update(SwimmingObject[] enemyList, ref int enemySize, SwimmingObject[] fishList, int fishSize, int changeDirection, HydroBot hydroBot, List<DamageBullet> enemyBullets, List<DamageBullet> alliesBullets, BoundingFrustum cameraFrustum, GameTime gameTime, GameMode gameMode)
        {
            EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);

            // if clip player has been initialized, update it
            if (BoundingSphere.Intersects(cameraFrustum))
            {
                qRotation = Quaternion.CreateFromAxisAngle(
                                Vector3.Up,
                                ForwardDirection);
                //float scale = 0.25f;

                enemyMatrix = Matrix.CreateScale(modelScale) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                                    Matrix.CreateFromQuaternion(qRotation) *
                                    Matrix.CreateTranslation(Position);
                clipPlayer.update(gameTime.ElapsedGameTime, true, enemyMatrix);
            }
            //if stunned, switch to idle anim
            //for mutant shark, idle = swimming normally
            if (stunned)
            {
                if (!clipPlayer.inRange(10, 40))
                    clipPlayer.switchRange(10, 40);
                return;
            }

            // Fleeing stuff
            if (isFleeing == true)
            {
                if (PoseidonGame.playTime.TotalSeconds - fleeingStart.TotalSeconds < fleeingDuration.TotalSeconds * HydroBot.seaCowPower)
                {
                    flee(enemyList, enemySize, fishList, fishSize, hydroBot);
                    return;
                }
                else
                {
                    isFleeing = false;
                }
                if (!clipPlayer.inRange(10, 40))
                    clipPlayer.switchRange(10, 40);
            }

            // Wear out slow
            if (speedFactor != 1)
                if (PoseidonGame.playTime.TotalSeconds - slowStart.TotalSeconds > slowDuration.TotalSeconds * HydroBot.turtlePower)
                    speedFactor = 1;

            float buffFactor = HydroBot.currentEnergy / GameConstants.PlayerStartingEnergy / 2.0f * HydroBot.beltPower;
            buffFactor = MathHelper.Clamp(buffFactor, 1.0f, 1.6f);
            if (isHypnotise && PoseidonGame.playTime.TotalSeconds - startHypnotiseTime.TotalSeconds > GameConstants.timeHypnotiseLast * buffFactor)
            {
                wearOutHypnotise();
            }

            if (!isHypnotise && !isFleeing && !stunned)
            {
                int perceptionID = perceptAndLock(hydroBot, fishList, fishSize);
                configAction(hydroBot, perceptionID, gameTime);
                makeAction(changeDirection, enemyList, enemySize, fishList, fishSize, enemyBullets, hydroBot, cameraFrustum, gameTime, gameMode);
            }
            else
            {
                int perceptionID = perceptAndLock(hydroBot, enemyList, enemySize);
                configAction(hydroBot, perceptionID, gameTime);
                makeAction(changeDirection, enemyList, enemySize, fishList, fishSize, enemyBullets, hydroBot, cameraFrustum, gameTime, gameMode);
            }


        }
        // Execute the actions. ( scene: 1-playgamescene, 2-shipwreckscene)
        protected override void makeAction(int changeDirection, SwimmingObject[] enemies, int enemiesAmount, SwimmingObject[] fishes, int fishAmount, List<DamageBullet> bullets, HydroBot hydroBot, BoundingFrustum cameraFrustum, GameTime gameTime, GameMode gameMode)
        {
            if (configBits[0] == true)
            {
                // swimming w/o attacking
                if (!clipPlayer.inRange(10, 40) && !configBits[3])
                    clipPlayer.switchRange(10, 40);
                randomWalk(changeDirection, enemies, enemiesAmount, fishes, fishAmount, hydroBot, speedFactor);
                return;
            }
            if (currentHuntingTarget != null)
            {
                calculateHeadingDirection();
                calculateFutureBoundingSphere();
            }
            if (configBits[2] == true)
            {
                // swimming w/o attacking
                if (!clipPlayer.inRange(10, 40) && !configBits[3])
                    clipPlayer.switchRange(10, 40);
                goStraight(enemies, enemiesAmount, fishes, fishAmount, hydroBot);
            }

            if (configBits[3] == true)
            {
                startChasingTime = PoseidonGame.playTime;

                if (currentHuntingTarget is Fish)
                {
                    Fish tmp = (Fish)currentHuntingTarget;
                    if (tmp.health <= 0)
                    {
                        currentHuntingTarget = null;
                        return;
                    }
                }

                if (currentHuntingTarget is BaseEnemy)
                {
                    BaseEnemy tmp = (BaseEnemy)currentHuntingTarget;
                    if (tmp.health <= 0)
                    {
                        currentHuntingTarget = null;
                        return;
                    }
                }

                if (PoseidonGame.playTime.TotalSeconds - prevFire.TotalSeconds > timeBetweenFire / speedFactor)
                {
                    if (!clipPlayer.inRange(50, 65))
                        clipPlayer.switchRange(50, 65);
                    if (currentHuntingTarget.GetType().Name.Equals("HydroBot"))
                    {
                        if (!(HydroBot.invincibleMode || HydroBot.supersonicMode))
                        {
                            HydroBot.currentHitPoint -= damage;

                            Point point = new Point();
                            String point_string = "-" + damage.ToString() + "HP";
                            point.LoadContent(PoseidonGame.contentManager, point_string, hydroBot.Position, Color.Red);
                            if (gameMode == GameMode.ShipWreck)
                                ShipWreckScene.points.Add(point);
                            else if (gameMode == GameMode.MainGame)
                                PlayGameScene.points.Add(point);
                            else if (gameMode == GameMode.SurvivalMode)
                                SurvivalGameScene.points.Add(point);

                            PoseidonGame.audio.botYell.Play();
                            PlayGameScene.healthLost += damage;
                        }
                        if (HydroBot.autoHipnotizeMode)
                        {
                            setHypnotise();
                        }
                        if (HydroBot.autoExplodeMode)
                        {
                            PoseidonGame.audio.Explo1.Play();
                            if (gameMode == GameMode.MainGame)
                                PlayGameScene.gameCamera.Shake(12.5f, .2f);
                            else if (gameMode == GameMode.ShipWreck)
                                ShipWreckScene.gameCamera.Shake(12.5f, .2f);
                            else if (gameMode == GameMode.SurvivalMode)
                                SurvivalGameScene.gameCamera.Shake(12.5f, .2f);

                            CastSkill.UseThorHammer(hydroBot.Position, hydroBot.MaxRangeX, hydroBot.MaxRangeZ, (BaseEnemy[])enemies, ref enemiesAmount, fishes, fishAmount, HydroBot.gameMode, true);
                        }
                    }
                    if (currentHuntingTarget is SwimmingObject)
                    {
                        ((SwimmingObject)currentHuntingTarget).health -= damage;
                        if (currentHuntingTarget.BoundingSphere.Intersects(cameraFrustum))
                        {
                            if (currentHuntingTarget is Fish)
                                PoseidonGame.audio.animalYell.Play();
                            Point point = new Point();
                            String point_string = "-" + damage.ToString() + "HP";
                            point.LoadContent(PoseidonGame.contentManager, point_string, currentHuntingTarget.Position, Color.Red);
                            if (gameMode == GameMode.ShipWreck)
                                ShipWreckScene.points.Add(point);
                            else if (gameMode == GameMode.MainGame)
                                PlayGameScene.points.Add(point);
                            else if (gameMode == GameMode.SurvivalMode)
                                SurvivalGameScene.points.Add(point);
                        }
                    }
                    prevFire = PoseidonGame.playTime;

                    if (this.BoundingSphere.Intersects(cameraFrustum))
                        PoseidonGame.audio.slashSound.Play();
                }
            }
        }
    }
}