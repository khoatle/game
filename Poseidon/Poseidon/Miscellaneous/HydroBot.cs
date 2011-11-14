#region File Description
//-----------------------------------------------------------------------------
// Tank.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using SkinnedModel;
using Poseidon.Core;
#endregion

namespace Poseidon
{
    /// <summary>
    /// Helper class for drawing a tank model with animated wheels and turret.
    /// </summary>
    public class HydroBot : GameObject
    {
        public float ForwardDirection { get; set; }

        //For the animation
        Model botModel;
        Matrix[] bones;
        SkinningData skd;
        public ClipPlayer clipPlayer;
        Matrix charMatrix;
        Quaternion qRotation = Quaternion.Identity;

        //Attributes of our main character ls=LevelStart
        public static float strength, lsStrength;
        public static float speed, lsSpeed;
        public static float shootingRate, lsShootingRate;
        public static float maxHitPoint, lsMaxHitPoint;
        public static float currentHitPoint;//, lsCurrentHitPoint;
        public static int currentEnvPoint, lsCurrentEnvPoint;
        public static int maxEnvPoint;
        // 2 types of bullet
        // 0: killing
        // 1: healing
        public static int bulletType;

        //Skills/Spells of our main character
        //true = enabled/found
        public static bool[] skills, lsSkills;
        //which skill is being selected
        public static int activeSkillID, lsActiveSkillID;
        //time that skills were previously casted
        //for managing skills' cool down time
        public static double[] skillPrevUsed;
        //invincible mode when Achilles' Armor is used
        public static bool invincibleMode;
        //supersonic mode when using Hermes' winged sandal
        public static bool supersonicMode;
        //if it is the 1st time the user use it
        //let him use it
        public static bool[] firstUse;

        //Cool down time for plant
        public static double prevPlantTime;
        public static bool firstPlant;

        //Sphere for interacting with trashs and fruits
        public BoundingSphere Trash_Fruit_BoundingSphere;
        //SoundEffect RetrievedSound;
        //temporary power-up for the cyborg
        //int tempPower;
        public static float speedUp;
        public static float strengthUp;
        public static float fireRateUp;
        public static double strengthUpStartTime;
        public static double speedUpStartTime;
        public static double fireRateUpStartTime;

        public float desiredAngle;
        public Vector3 pointToMoveTo;
        public bool reachDestination = true;

        public float floatHeight;

        public static int currentExperiencePts, lsCurrentExperiencePts;
        public static int nextLevelExperience, lsNextLevelExperience;
        private static int increaseBy, lsIncreaseBy;
        public static int level, lsLevel; // experience level
        public static int unassignedPts, lsUnassignedPts;

        // Tank moving bound
        public int MaxRangeX;
        public int MaxRangeZ;

        public static bool isPoissoned;
        public static float accumulatedHealthLossFromPoisson;
        public static float maxHPLossFromPoisson;
        public static float poissonInterval;

        public HydroBot(int MaxRangeX, int MaxRangeZ, float floatHeight)
        {
            // Original attribute
            strength = lsStrength = GameConstants.MainCharStrength;
            speed = lsSpeed = GameConstants.BasicStartSpeed;
            shootingRate = lsShootingRate = GameConstants.MainCharShootingSpeed;
            bulletType = 1;
            maxHitPoint = lsMaxHitPoint = GameConstants.PlayerStartingHP;
            currentHitPoint = GameConstants.PlayerStartingHP;
            maxEnvPoint = GameConstants.MaxEnv;
            currentEnvPoint = lsCurrentEnvPoint = GameConstants.PlayerStartingEnv;
            pointToMoveTo = Vector3.Zero;

            // No buff up at the beginning
            speedUp = 1.0f;
            strengthUp = 1.0f;
            fireRateUp = 1.0f;

            skills = new bool[GameConstants.numberOfSkills];
            lsSkills = new bool[GameConstants.numberOfSkills];
            skillPrevUsed = new double[GameConstants.numberOfSkills];
            firstUse = new bool[GameConstants.numberOfSkills];

            this.floatHeight = floatHeight;
            Position.Y = floatHeight;

            this.MaxRangeX = MaxRangeX;
            this.MaxRangeZ = MaxRangeZ;

            currentExperiencePts = lsCurrentExperiencePts = 0;
            nextLevelExperience = lsNextLevelExperience = GameConstants.MainCharLevelOneExp;
            level = lsLevel = 1;
            unassignedPts = lsUnassignedPts = 0;

            activeSkillID = lsActiveSkillID = -1;
            invincibleMode = false;
            supersonicMode = false;
            //just for testing
            //should be removed
            //activeSkillID = lsActiveSkillID = 0;

            isPoissoned = false;
            poissonInterval = 0;
            maxHPLossFromPoisson = 50;
            accumulatedHealthLossFromPoisson = 0;
        }

        /// <summary>
        /// Loads the tank model.
        /// </summary>
        public void Load(ContentManager content)
        {
            // Load the tank model from the ContentManager.
            botModel = content.Load<Model>("Models/MainCharacter/bot");
            Model = botModel;

            ForwardDirection = 0.0f;
            //MaxRange = GameConstants.MaxRange;
            BoundingSphere = CalculateBoundingSphere();

            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;
            scaledSphere.Center.Y = floatHeight;
            scaledSphere.Radius *=
                GameConstants.TankBoundingSphereFactor;
            BoundingSphere =
                new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);

            //no skill yet activated
            for (int index = 0; index < GameConstants.numberOfSkills; index++)
            {
                skills[index] = false;
                firstUse[index] = true;
                skillPrevUsed[index] = 0;
            }

            invincibleMode = false;
            supersonicMode = false;
            //just for testing
            //should be removed
            activeSkillID = 4;
            skills[0] = true;
            skills[1] = true;
            skills[2] = true;
            skills[3] = true;
            skills[4] = true;

            firstPlant = true;
            prevPlantTime = 0;

            LoadAnimation(1, 30, 24);
        }

        public void LoadAnimation(int clipStart, int clipEnd, int fpsRate)
        {
            //for the animation
            skd = Model.Tag as SkinningData;
            clipPlayer = new ClipPlayer(skd, fpsRate);//ClipPlayer running at 24 frames/sec
            AnimationClip clip = skd.AnimationClips["Take 001"]; //Take name from the dude.fbx file
            clipPlayer.play(clip, clipStart, clipEnd, true);
            charMatrix = Matrix.CreateScale(0.1f) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                               Matrix.CreateTranslation(Position);
        }

        public void ResetToLevelStart()
        {
            strength = lsStrength;
            speed = lsSpeed;
            shootingRate = lsShootingRate;
            bulletType = 1;
            maxHitPoint = lsMaxHitPoint;
            //currentHitPoint = lsCurrentHitPoint;
            currentEnvPoint = lsCurrentEnvPoint;
            lsSkills.CopyTo(skills, 0);
            activeSkillID = lsActiveSkillID;

            currentExperiencePts = lsCurrentExperiencePts;
            nextLevelExperience = lsNextLevelExperience;
            increaseBy = lsIncreaseBy;
            level = lsLevel;
            unassignedPts = lsUnassignedPts;

        }

        public void SetLevelStartValues()
        {
            // Original attribute
            lsStrength = strength;
            lsSpeed = speed;
            lsShootingRate = shootingRate;
            lsMaxHitPoint = maxHitPoint;
            //lsCurrentHitPoint = currentHitPoint;

            skills.CopyTo(lsSkills, 0);

            lsActiveSkillID = activeSkillID;

            lsCurrentExperiencePts = currentExperiencePts;
            lsNextLevelExperience = nextLevelExperience;
            lsIncreaseBy = increaseBy;
            lsLevel = level;
            lsUnassignedPts = unassignedPts;
            lsCurrentEnvPoint = currentEnvPoint;
        }

        internal void Reset()
        {
            Position = Vector3.Zero;
            Position.Y = floatHeight;
            ForwardDirection = 0f;
            pointToMoveTo = Vector3.Zero;
            reachDestination = true;
            for (int index = 0; index < GameConstants.numberOfSkills; index++)
            {
                firstUse[index] = true;
                skillPrevUsed[index] = 0;
            }
            invincibleMode = false;
            // No buff up at the beginning
            speedUp = 1.0f;
            strengthUp = 1.0f;
            fireRateUp = 1.0f;
            strengthUpStartTime = 0;
            speedUpStartTime = 0;
            fireRateUpStartTime = 0;
            currentHitPoint = maxHitPoint;
            isPoissoned = false;
            firstPlant = true;
            prevPlantTime = 0;
            accumulatedHealthLossFromPoisson = 0;
            PlayGameScene.points.Clear();
            ShipWreckScene.points.Clear();
            if(PlayGameScene.currentLevel>0)
                currentEnvPoint -= (GameConstants.NumberTrash[PlayGameScene.currentLevel] * GameConstants.envLossPerTrashAdd);
            if (currentEnvPoint < GameConstants.EachLevelMinEnv) currentEnvPoint = GameConstants.EachLevelMinEnv;
        }


        public void Update(KeyboardState keyboardState, SwimmingObject[] enemies,int enemyAmount, SwimmingObject[] fishes, int fishAmount, List<Fruit> fruits, List<Trash> trashes, GameTime gameTime, Vector3 pointMoveTo, int scene) //scene- 1playgame 2shipwreck
        {
            if (isPoissoned == true) {
                if (accumulatedHealthLossFromPoisson < maxHPLossFromPoisson) {
                    currentHitPoint -= 0.1f;
                    accumulatedHealthLossFromPoisson += 0.1f;

                    ////display HP loss
                    //if (accumulatedHealthLossFromPoisson > 10)
                    //{
                    //    Point point = new Point();
                    //    String point_string = "-10HP";
                    //    point.LoadContent(PlayGameScene.Content, point_string, Position, Color.White);
                    //    if (scene == 2)
                    //        ShipWreckScene.points.Add(point);
                    //    else
                    //        PlayGameScene.points.Add(point);
                    //}
                }
                else {
                    isPoissoned = false;
                    accumulatedHealthLossFromPoisson = 0;

                    Point point = new Point();
                    String point_string = "Poison Free";
                    point.LoadContent(PlayGameScene.Content, point_string, Position, Color.White);
                    if (scene == 2)
                        ShipWreckScene.points.Add(point);
                    else
                        PlayGameScene.points.Add(point);
                }
            }

            if (currentExperiencePts >= nextLevelExperience) {
                //increaseBy = (int)(increaseBy * 1.25);
                //nextLevelExperience += increaseBy;
                currentExperiencePts -= nextLevelExperience;
                nextLevelExperience += (int)(nextLevelExperience/5);
                //strength *= 1.15f;
                //maxHitPoint = (int)(maxHitPoint * 1.10f);
                //currentHitPoint = maxHitPoint;
                unassignedPts += 5;
                level++;
                PoseidonGame.audio.levelUpSound.Play();
            }

            Vector3 futurePosition = Position;

            //worn out effect of power-ups
            if (speedUp != 1.0f)
            {
                if (gameTime.TotalGameTime.TotalSeconds - speedUpStartTime >= GameConstants.EffectExpired)
                {
                    speedUp = 1.0f;
                }
            }
            if (strengthUp != 1.0f)
            {
                if (gameTime.TotalGameTime.TotalSeconds - strengthUpStartTime >= GameConstants.EffectExpired)
                {
                    strengthUp = 1.0f;
                }
            }
            if (fireRateUp != 1.0f)
            {
                if (gameTime.TotalGameTime.TotalSeconds - fireRateUpStartTime >= GameConstants.EffectExpired)
                {
                    fireRateUp = 1.0f;
                }
            }
            //worn out effect of certain skills
            if (invincibleMode == true)
            {
                float buffFactor = shootingRate * fireRateUp / 1.5f;
                buffFactor = MathHelper.Clamp(buffFactor, 1.0f, 1.6f);
                if (gameTime.TotalGameTime.TotalSeconds - skillPrevUsed[2] >= GameConstants.timeArmorLast * buffFactor)
                {
                    invincibleMode = false;
                }
            }
            //worn out effect of supersonic
            if (supersonicMode == true)
            {
                if (gameTime.TotalGameTime.TotalMilliseconds - skillPrevUsed[3]*1000 >= GameConstants.timeSuperSonicLast)
                {
                    // To prevent bot landing on an enemy after using the sandal
                    if ( !Collision.isBotVsBarrierCollision(this.BoundingSphere, enemies, enemyAmount))
                    {
                        supersonicMode = false;
                    }
                }
            }
            //float turnAmount = 0;
            //if (keyboardState.IsKeyDown(Keys.A))
            //{
            //    turnAmount = 1;
            //}
            //else if (keyboardState.IsKeyDown(Keys.D))
            //{
            //    turnAmount = -1;
            //}
            //// Player has speed buff from both temporary powerups and his speed attritubte
            
            //ForwardDirection += turnAmount * GameConstants.TurnSpeed * speedUp * speed;
            //ForwardDirection = WrapAngle(ForwardDirection);
            Vector3 movement = Vector3.Zero;

            if (pointMoveTo != Vector3.Zero && Math.Abs(pointMoveTo.X)<MaxRangeX && Math.Abs(pointMoveTo.Z) < MaxRangeZ)
            {
                //desiredAngle = angle;
                this.pointToMoveTo = pointMoveTo;
                desiredAngle = CalculateAngle(this.pointToMoveTo, Position);
                reachDestination = false;
            }
            //if (desiredAngle != 0)

            if (reachDestination == false)// && pointMoveTo != Vector3.Zero)
            {
                //float difference = WrapAngle(desiredAngle - ForwardDirection);
                Vector3 posDif = this.pointToMoveTo - Position;
                float distanceToDest = posDif.Length();
                //// clamp that between -turnSpeed and turnSpeed.
                
                //if (distanceToDest <= 10f * speedUp * this.speed && ForwardDirection != desiredAngle)
                //{
                //    difference = MathHelper.Clamp(difference, -GameConstants.TurnSpeed , GameConstants.TurnSpeed);
                //}
                //else difference = MathHelper.Clamp(difference, -GameConstants.TurnSpeed * speedUp * this.speed / 2, GameConstants.TurnSpeed * speedUp * this.speed / 2);
                //if (Math.Abs(difference) >= 0.0025f)
                // so, the closest we can get to our target is currentAngle + difference.
                // return that, using WrapAngle again.
                //ForwardDirection = WrapAngle(ForwardDirection + difference);
                //if (ForwardDirection == desiredAngle) desiredAngle = 0;
                
                //if (Position == this.pointToMoveTo) reachDestination = true;


                //if (ForwardDirection == desiredAngle) movement.Z = 1 * speedUp * this.speed;
                //else movement.Z = 0.3f;
                ForwardDirection = desiredAngle;
                movement.Z = 1;
                if (distanceToDest <= 1f)
                {
                    movement.Z = 0;
                    reachDestination = true;
                }
                //if (distanceToDest <= 10f && ForwardDirection != desiredAngle)
                //{
                //    movement.Z = 0;
                //}
                
            }
            //ForwardDirection = WrapAngle(angle);
            //ForwardDirection = WrapAngle(ForwardDirection);
            
            Matrix orientationMatrix = Matrix.CreateRotationY(ForwardDirection);

            //if (keyboardState.IsKeyDown(Keys.W))
            //{
            //    movement.Z = 1;
            //}
            //else if (keyboardState.IsKeyDown(Keys.S))
            //{
            //    movement.Z = -1;
            //}
            //if (desiredAngle != 0) movement.Z = 1;
            Vector3 speedl = Vector3.Transform(movement, orientationMatrix);
            speedl *= GameConstants.MainCharVelocity * speedUp * speed;
            if (supersonicMode == true) speedl *= 5;
            futurePosition = Position + speedl;
            if (Collision.isBotValidMove(this, futurePosition, enemies, enemyAmount, fishes, fishAmount))
            {
                Position = futurePosition;

                BoundingSphere updatedSphere;
                updatedSphere = BoundingSphere;

                updatedSphere.Center.X = Position.X;
                updatedSphere.Center.Z = Position.Z;
                BoundingSphere = new BoundingSphere(updatedSphere.Center,
                    updatedSphere.Radius);
                //Trash_Fruit_BoundingSphere = new BoundingSphere(updatedSphere.Center,
                //    20);
            }

            //Interacting with trashs and fruits
            if (keyboardState.IsKeyDown(Keys.Z))
            {
                Interact_with_trash_and_fruit(fruits, trashes, gameTime);
            }
            //Position = Vector3.Zero;
            // if clip player has been initialized, update it
            if (clipPlayer != null)
            {
                qRotation = Quaternion.CreateFromAxisAngle(
                                Vector3.Up,
                                ForwardDirection);

                charMatrix = Matrix.CreateScale(0.1f) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                                    Matrix.CreateFromQuaternion(qRotation) *
                                    Matrix.CreateTranslation(Position);
                clipPlayer.update(gameTime.ElapsedGameTime, true, charMatrix);
            }
        }

        private void Interact_with_trash_and_fruit(List<Fruit> fruits, List<Trash> trashes, GameTime gameTime)
        {
            Trash_Fruit_BoundingSphere = new BoundingSphere(BoundingSphere.Center,
                    20);
            if (fruits != null)
            {
                for (int curCell = 0; curCell < fruits.Count; curCell++)
                {
                    if (fruits[curCell].Retrieved == false && Trash_Fruit_BoundingSphere.Intersects(
                        fruits[curCell].BoundingSphere))
                    {
                        fruits[curCell].Retrieved = true;
                        if (fruits[curCell].powerType == 1)
                        {
                            speedUpStartTime = gameTime.TotalGameTime.TotalSeconds;
                            speedUp = 2.0f;

                            Point point = new Point();
                            String point_string = "TEMP-SPEED X 2";
                            point.LoadContent(PlayGameScene.Content, point_string, fruits[curCell].Position, Color.LawnGreen);
                            PlayGameScene.points.Add(point);
                        }
                        else if (fruits[curCell].powerType == 2)
                        {
                            strengthUpStartTime = gameTime.TotalGameTime.TotalSeconds;
                            strengthUp = 2.0f;

                            Point point = new Point();
                            String point_string = "\nTEMP-STRENGTH X 2";
                            point.LoadContent(PlayGameScene.Content, point_string, fruits[curCell].Position, Color.LawnGreen);
                            PlayGameScene.points.Add(point);
                        }
                        else if (fruits[curCell].powerType == 3)
                        {
                            fireRateUpStartTime = gameTime.TotalGameTime.TotalSeconds;
                            fireRateUp = 2.0f;

                            Point point = new Point();
                            String point_string = "\n\nTEMP-SHOOTING RATE X 2";
                            point.LoadContent(PlayGameScene.Content, point_string, fruits[curCell].Position, Color.LawnGreen);
                            PlayGameScene.points.Add(point);
                        }
                        else if (fruits[curCell].powerType == 4)
                        {
                            float hitpointAdded = maxHitPoint - currentHitPoint;
                            currentHitPoint += 100;
                            if (currentHitPoint > maxHitPoint)
                            {
                                currentHitPoint = maxHitPoint;
                                Point point = new Point();
                                String point_string = "\n\n\n+"+ (int)hitpointAdded +" HEALTH";
                                point.LoadContent(PlayGameScene.Content, point_string, fruits[curCell].Position, Color.LawnGreen);
                                PlayGameScene.points.Add(point);
                            }
                            else
                            {
                                Point point = new Point();
                                String point_string = "\n\n\n+100 HEALTH";
                                point.LoadContent(PlayGameScene.Content, point_string, fruits[curCell].Position, Color.LawnGreen);
                                PlayGameScene.points.Add(point);
                            }
                        }
                        //RetrievedSound.Play();
                        PlayGameScene.audio.retrieveSound.Play();
                    }
                }
            }
            if (trashes != null)
            {
                foreach (Trash trash in trashes)
                {
                    if (trash.Retrieved == false && Trash_Fruit_BoundingSphere.Intersects(trash.BoundingSphere))
                    {
                        trash.Retrieved = true;
                        currentExperiencePts += trash.experienceReward;
                        currentEnvPoint += GameConstants.envGainForTrashClean;
                        PlayGameScene.audio.retrieveSound.Play();
                        //RetrievedSound.Play();

                        Point point = new Point();
                        String point_string = "+" + GameConstants.envGainForTrashClean.ToString() + "ENV\n+" + trash.experienceReward + "EXP";
                        point.LoadContent(PlayGameScene.Content, point_string, trash.Position, Color.LawnGreen);
                        PlayGameScene.points.Add(point);
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Draws the tank model, using the current animation settings.
        /// </summary>
        public void Draw(Matrix view, Matrix projection)
        {
            
            bones = clipPlayer.GetSkinTransforms();

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(bones);
                    effect.View = view;
                    effect.Projection = projection;

                    if (invincibleMode == true)
                    {
                        effect.DiffuseColor = Color.Gold.ToVector3();
                    }
                    else if (isPoissoned == true) {
                        effect.DiffuseColor = Color.Green.ToVector3();
                    }
                    else {
                        effect.DiffuseColor = Vector3.One;
                    }

                    effect.FogEnabled = true;
                    effect.FogStart = GameConstants.FogStart;
                    effect.FogEnd = GameConstants.FogEnd;
                    effect.FogColor = GameConstants.FogColor.ToVector3();
                }
                mesh.Draw();
            }
        }
        internal void DrawTrashFruitSphere(Matrix view, Matrix projection,
            GameObject boundingSphereModel)
        {
            Matrix scaleMatrix = Matrix.CreateScale(20);
            Matrix translateMatrix =
                Matrix.CreateTranslation(Trash_Fruit_BoundingSphere.Center);
            Matrix worldMatrix = scaleMatrix * translateMatrix;

            foreach (ModelMesh mesh in boundingSphereModel.Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = worldMatrix;
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }
        }

        public static float CalculateAngle(Vector3 point2, Vector3 point1)
        {
            return (float)Math.Atan2(point2.X - point1.X, point2.Z - point1.Z);
        }
    }
}
