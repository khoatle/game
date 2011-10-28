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
#endregion

namespace Poseidon
{
    /// <summary>
    /// Helper class for drawing a tank model with animated wheels and turret.
    /// </summary>
    public class Tank : GameObject
    {
        #region Fields


        // The XNA framework Model object that we are going to display.
        Model tankModel;


        // Shortcut references to the bones that we are going to animate.
        // We could just look these up inside the Draw method, but it is more
        // efficient to do the lookups while loading and cache the results.
        ModelBone leftBackWheelBone;
        ModelBone rightBackWheelBone;
        ModelBone leftFrontWheelBone;
        ModelBone rightFrontWheelBone;
        ModelBone leftSteerBone;
        ModelBone rightSteerBone;
        ModelBone turretBone;
        ModelBone cannonBone;
        ModelBone hatchBone;


        // Store the original transform matrix for each animating bone.
        Matrix leftBackWheelTransform;
        Matrix rightBackWheelTransform;
        Matrix leftFrontWheelTransform;
        Matrix rightFrontWheelTransform;
        Matrix leftSteerTransform;
        Matrix rightSteerTransform;
        Matrix turretTransform;
        Matrix cannonTransform;
        Matrix hatchTransform;


        // Array holding all the bone transform matrices for the entire model.
        // We could just allocate this locally inside the Draw method, but it
        // is more efficient to reuse a single array, as this avoids creating
        // unnecessary garbage.
        Matrix[] boneTransforms;


        // Current animation positions.
        float wheelRotationValue;
        float steerRotationValue;
        float turretRotationValue;
        float cannonRotationValue;
        float hatchRotationValue;

        public float ForwardDirection { get; set; }
        //public int MaxRange { get; set; }
        #endregion


        //Attributes of our main character ls=LevelStart
        public static float strength, lsStrength;
        public static float speed, lsSpeed;
        public static float shootingRate, lsShootingRate;
        public static int maxHitPoint, lsMaxHitPoint;
        public static int currentHitPoint, lsCurrentHitPoint;
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
        SoundEffect RetrievedSound;
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
        #region Properties


        /// <summary>
        /// Gets or sets the wheel rotation amount.
        /// </summary>
        public float WheelRotation
        {
            get { return wheelRotationValue; }
            set { wheelRotationValue = value; }
        }


        /// <summary>
        /// Gets or sets the steering rotation amount.
        /// </summary>
        public float SteerRotation
        {
            get { return steerRotationValue; }
            set { steerRotationValue = value; }
        }


        /// <summary>
        /// Gets or sets the turret rotation amount.
        /// </summary>
        public float TurretRotation
        {
            get { return turretRotationValue; }
            set { turretRotationValue = value; }
        }


        /// <summary>
        /// Gets or sets the cannon rotation amount.
        /// </summary>
        public float CannonRotation
        {
            get { return cannonRotationValue; }
            set { cannonRotationValue = value; }
        }


        /// <summary>
        /// Gets or sets the entry hatch rotation amount.
        /// </summary>
        public float HatchRotation
        {
            get { return hatchRotationValue; }
            set { hatchRotationValue = value; }
        }


        #endregion

        public Tank(int MaxRangeX, int MaxRangeZ, float floatHeight)
        {
            // Original attribute
            strength = lsStrength = 1.0f;
            speed = lsSpeed = 1.0f;
            shootingRate = lsShootingRate = 1.0f;
            bulletType = 0;
            maxHitPoint = lsMaxHitPoint = GameConstants.PlayerStartingHP;
            currentHitPoint = lsCurrentHitPoint = GameConstants.PlayerStartingHP;
            maxEnvPoint = GameConstants.MaxEnv;
            currentEnvPoint = lsCurrentEnvPoint = GameConstants.PlayerStartingEnv;
            pointToMoveTo = Vector3.Zero;

            // No buff up at the beginning
            speedUp = 1.0f;
            strengthUp = 1.0f;
            fireRateUp = 1.0f;

            skills = lsSkills = new bool[GameConstants.numberOfSkills];
            skillPrevUsed = new double[GameConstants.numberOfSkills];
            firstUse = new bool[GameConstants.numberOfSkills];

            this.floatHeight = floatHeight;
            Position.Y = floatHeight;

            this.MaxRangeX = MaxRangeX;
            this.MaxRangeZ = MaxRangeZ;

            currentExperiencePts = lsCurrentExperiencePts = 0;
            nextLevelExperience = lsNextLevelExperience = 100;
            increaseBy = lsIncreaseBy = 100;
            level = lsLevel = 1;
            unassignedPts = lsUnassignedPts = 0;

            activeSkillID = lsActiveSkillID = -1;
            invincibleMode = false;
            supersonicMode = false;
            //just for testing
            //should be removed
            activeSkillID = lsActiveSkillID = 0;
            skills[0] = true;
            skills[1] = true;
            skills[2] = true;
            skills[3] = true;
            skills[4] = true;
            
            lsSkills = skills;
        }

        /// <summary>
        /// Loads the tank model.
        /// </summary>
        public void Load(ContentManager content)
        {
            // Load the tank model from the ContentManager.
            tankModel = content.Load<Model>("Models/MainCharacter/tank");
            Model = tankModel;
            // Look up shortcut references to the bones we are going to animate.
            leftBackWheelBone = tankModel.Bones["l_back_wheel_geo"];
            rightBackWheelBone = tankModel.Bones["r_back_wheel_geo"];
            leftFrontWheelBone = tankModel.Bones["l_front_wheel_geo"];
            rightFrontWheelBone = tankModel.Bones["r_front_wheel_geo"];
            leftSteerBone = tankModel.Bones["l_steer_geo"];
            rightSteerBone = tankModel.Bones["r_steer_geo"];
            turretBone = tankModel.Bones["turret_geo"];
            cannonBone = tankModel.Bones["canon_geo"];
            hatchBone = tankModel.Bones["hatch_geo"];

            // Store the original transform matrix for each animating bone.
            leftBackWheelTransform = leftBackWheelBone.Transform;
            rightBackWheelTransform = rightBackWheelBone.Transform;
            leftFrontWheelTransform = leftFrontWheelBone.Transform;
            rightFrontWheelTransform = rightFrontWheelBone.Transform;
            leftSteerTransform = leftSteerBone.Transform;
            rightSteerTransform = rightSteerBone.Transform;
            turretTransform = turretBone.Transform;
            cannonTransform = cannonBone.Transform;
            hatchTransform = hatchBone.Transform;

            // Allocate the transform matrix array.
            boneTransforms = new Matrix[tankModel.Bones.Count];

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

            //Trash_Fruit_BoundingSphere =
            //    new BoundingSphere(scaledSphere.Center, 10);
            RetrievedSound = content.Load<SoundEffect>("sound/laserFire");

            //no skill yet activated
            for (int index = 0; index < GameConstants.numberOfSkills; index++)
            {
                skills[index] = false;
                firstUse[index] = true;
                skillPrevUsed[index] = 0;
            }
            activeSkillID = -1;
            invincibleMode = false;
            supersonicMode = false;
            //just for testing
            //should be removed
            activeSkillID = 0;
            skills[0] = true;
            skills[1] = true;
            skills[2] = true;
            skills[3] = true;
            skills[4] = true;

            firstPlant = true;
            prevPlantTime = 0;
        }


        public void ResetToLevelStart()
        {
            strength = lsStrength;
            speed = lsSpeed;
            shootingRate = lsShootingRate;
            bulletType = 0;
            maxHitPoint = lsMaxHitPoint;
            currentHitPoint = lsCurrentExperiencePts;
            currentEnvPoint = lsCurrentEnvPoint;
            skills = lsSkills;
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
            lsCurrentHitPoint = currentHitPoint;

            lsSkills = skills;
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
            firstPlant = true;
            prevPlantTime = 0;
            if(PlayGameScene.currentLevel>0)
                currentEnvPoint -= (GameConstants.NumberTrash[PlayGameScene.currentLevel] * GameConstants.envLossPerTrashAdd);
            if (currentEnvPoint < 0) currentEnvPoint = 0;
        }

 
        // Copy every attributes but the position and direction
        //public void CopyAttribute(Tank tank)
        //{
        //    strength = tank.strength;
        //    //speed = tank.speed;
        //    shootingRate = tank.shootingRate;
        //    maxHitPoint = tank.maxHitPoint;
        //    currentHitPoint = tank.currentHitPoint;
        //    speedUp = tank.speedUp;
        //    strengthUp = tank.strengthUp;
        //    fireRateUp = tank.fireRateUp;
        //    strengthUpStartTime = tank.strengthUpStartTime;
        //    speedUpStartTime = tank.speedUpStartTime;
        //    fireRateUpStartTime = tank.fireRateUpStartTime;
        //    skills = tank.skills;
        //    skillPrevUsed = tank.skillPrevUsed;
        //    activeSkillID = tank.activeSkillID;
        //    invincibleMode = tank.invincibleMode;
        //    supersonicMode = tank.supersonicMode;
        //    firstUse = tank.firstUse;
        //}

        public void Update(KeyboardState keyboardState, SwimmingObject[] enemies,int enemyAmount, SwimmingObject[] fishes, int fishAmount, List<Fruit> fruits, List<Trash> trashes, GameTime gameTime, Vector3 pointMoveTo)
        {
            if (currentExperiencePts >= nextLevelExperience) {
                increaseBy = (int)(increaseBy * 1.5);
                //nextLevelExperience += increaseBy;
                currentExperiencePts -= nextLevelExperience;
                nextLevelExperience = increaseBy;
                //strength *= 1.15f;
                //maxHitPoint = (int)(maxHitPoint * 1.10f);
                //currentHitPoint = maxHitPoint;
                unassignedPts += 5;
                level++;
            }

            Vector3 futurePosition = Position;
            //if (steerRotationValue != 0) steerRotationValue = 0;
            //if (wheelRotationValue != 0) wheelRotationValue = 0;

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
                if (gameTime.TotalGameTime.TotalSeconds - skillPrevUsed[2] >= GameConstants.timeArmorLast)
                {
                    invincibleMode = false;
                }
            }
            //worn out effect of supersonic
            if (supersonicMode == true)
            {
                if (gameTime.TotalGameTime.TotalMilliseconds - skillPrevUsed[3]*1000 >= GameConstants.timeSuperSonicLast)
                {
                    supersonicMode = false;
                }
            }
            float turnAmount = 0;
            if (keyboardState.IsKeyDown(Keys.A))
            {
                turnAmount = 1;
            }
            else if (keyboardState.IsKeyDown(Keys.D))
            {
                turnAmount = -1;
            }
            else steerRotationValue = 0;
            // Player has speed buff from both temporary powerups and his speed attritubte
            
            ForwardDirection += turnAmount * GameConstants.TurnSpeed * speedUp * speed;
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

            
            if (keyboardState.IsKeyDown(Keys.W))
            {
                movement.Z = 1;
            }
            else if (keyboardState.IsKeyDown(Keys.S))
            {
                movement.Z = -1;
            }
            else wheelRotationValue = 0;
            //if (desiredAngle != 0) movement.Z = 1;
            Vector3 speedl = Vector3.Transform(movement, orientationMatrix);
            speedl *= GameConstants.Velocity * speedUp * speed;
            if (supersonicMode == true) speedl *= 5;
            futurePosition = Position + speedl;
            steerRotationValue = turnAmount;
            wheelRotationValue += movement.Z * 20;
            if (Collision.isTankValidMove(this, futurePosition, enemies, enemyAmount, fishes, fishAmount))
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
                        }
                        else if (fruits[curCell].powerType == 2)
                        {
                            strengthUpStartTime = gameTime.TotalGameTime.TotalSeconds;
                            strengthUp = 2.0f;
                        }
                        else if (fruits[curCell].powerType == 3)
                        {
                            fireRateUpStartTime = gameTime.TotalGameTime.TotalSeconds;
                            fireRateUp = 2.0f;
                        }
                        RetrievedSound.Play();
                    }
                }
            }
            foreach (Trash trash in trashes)
            {
                if (trash.Retrieved == false && Trash_Fruit_BoundingSphere.Intersects(trash.BoundingSphere))
                {
                    trash.Retrieved = true;
                    currentExperiencePts += trash.experienceReward;
                    currentEnvPoint += GameConstants.envGainForTrashClean;
                    RetrievedSound.Play();
                }
            }
            return;
        }

        /// <summary>
        /// Draws the tank model, using the current animation settings.
        /// </summary>
        public void Draw(Matrix view, Matrix projection)
        {
            
            Matrix worldMatrix = Matrix.Identity;
            Matrix rotationYMatrix = Matrix.CreateRotationY(ForwardDirection);
            Matrix translateMatrix = Matrix.CreateTranslation(Position);
            
            worldMatrix = rotationYMatrix * translateMatrix;
            // Set the world matrix as the root transform of the model.
            tankModel.Root.Transform = worldMatrix;

            // Calculate matrices based on the current animation position.
            Matrix wheelRotation = Matrix.CreateRotationX(wheelRotationValue);
            Matrix steerRotation = Matrix.CreateRotationY(steerRotationValue);
            Matrix turretRotation = Matrix.CreateRotationY(turretRotationValue);
            Matrix cannonRotation = Matrix.CreateRotationX(cannonRotationValue);
            Matrix hatchRotation = Matrix.CreateRotationX(hatchRotationValue);

            // Apply matrices to the relevant bones.
            leftBackWheelBone.Transform = wheelRotation * leftBackWheelTransform;
            rightBackWheelBone.Transform = wheelRotation * rightBackWheelTransform;
            leftFrontWheelBone.Transform = wheelRotation * leftFrontWheelTransform;
            rightFrontWheelBone.Transform = wheelRotation * rightFrontWheelTransform;
            leftSteerBone.Transform = steerRotation * leftSteerTransform;
            rightSteerBone.Transform = steerRotation * rightSteerTransform;
            turretBone.Transform = turretRotation * turretTransform;
            cannonBone.Transform = cannonRotation * cannonTransform;
            hatchBone.Transform = hatchRotation * hatchTransform;

            // Look up combined bone matrices for the entire model.
            tankModel.CopyAbsoluteBoneTransformsTo(boneTransforms);

            // Draw the model.
            foreach (ModelMesh mesh in tankModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index];
                    effect.View = view;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                    if (invincibleMode == true)
                    {
                        effect.DiffuseColor = Color.Gold.ToVector3();
                    }
                    else effect.DiffuseColor = Vector3.One;

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
