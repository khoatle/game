﻿#region File Description
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
    public class Tank: GameObject
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
        public int MaxRange { get; set; }
        #endregion

        //Attributes of our main character
        public float strength;
        public float speed;
        public float shootingRate;
        public int hitPoint;

        //Sphere for interacting with trashs and fruits
        public BoundingSphere Trash_Fruit_BoundingSphere;
        SoundEffect RetrievedSound;
        //temporary power-up for the cyborg
        //int tempPower;
        public float speedUp;
        public float strengthUp;
        public float fireRateUp;
        double strengthUpStartTime;
        double speedUpStartTime;
        double fireRateUpStartTime;
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

        public Tank()
        {
            // Original attribute
            strength = 1.0f;
            speed = 1.0f;
            shootingRate = 1.0f;
            hitPoint = 100;

            // No buff up at the beginning
            speedUp = 1.0f;
            strengthUp = 1.0f;
            fireRateUp = 1.0f;
            Position.Y = GameConstants.FloatHeight;
        }
        /// <summary>
        /// Loads the tank model.
        /// </summary>
        public void Load(ContentManager content)
        {
            // Load the tank model from the ContentManager.
            tankModel = content.Load<Model>("Models/tank");
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
            MaxRange = GameConstants.MaxRange;
            BoundingSphere = CalculateBoundingSphere();

            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;
            scaledSphere.Center.Y = GameConstants.FloatHeight;
            scaledSphere.Radius *=
                GameConstants.TankBoundingSphereFactor;
            BoundingSphere =
                new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);

            //Trash_Fruit_BoundingSphere =
            //    new BoundingSphere(scaledSphere.Center, 10);
            RetrievedSound = content.Load<SoundEffect>("sound/laserFire");
            
        }
        internal void Reset()
        {
            Position = Vector3.Zero;
            ForwardDirection = 0f;
        }
        public void Update(KeyboardState keyboardState, List<Barrier> barriers, List<FuelCell> fuelCells, GameTime gameTime)
        {
            Vector3 futurePosition = Position;
            //if (steerRotationValue != 0) steerRotationValue = 0;
            //if (wheelRotationValue != 0) wheelRotationValue = 0;

            //worn out effect of power-ups
            if (speedUp != 1.0f)
            {
                if (gameTime.TotalGameTime.TotalSeconds - speedUpStartTime >= 5)
                {
                    speedUp = 1.0f;
                }
            }
            if (strengthUp != 1.0f)
            {
                if (gameTime.TotalGameTime.TotalSeconds - strengthUpStartTime >= 5)
                {
                    strengthUp = 1.0f;
                }
            }
            if (fireRateUp != 1.0f)
            {
                if (gameTime.TotalGameTime.TotalSeconds - fireRateUpStartTime >= 5)
                {
                    fireRateUp = 1.0f;
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
            ForwardDirection += turnAmount * GameConstants.TurnSpeed * speedUp * this.speed;
            Matrix orientationMatrix = Matrix.CreateRotationY(ForwardDirection);

            Vector3 movement = Vector3.Zero;
            if (keyboardState.IsKeyDown(Keys.W))
            {
                movement.Z = 1;
            }
            else if (keyboardState.IsKeyDown(Keys.S))
            {
                movement.Z = -1;
            }
            else wheelRotationValue = 0;
            Vector3 speed = Vector3.Transform(movement, orientationMatrix);
            speed *= GameConstants.Velocity * speedUp * this.speed;
            futurePosition = Position + speed;
            steerRotationValue = turnAmount;
            wheelRotationValue += movement.Z * 20;
            if (Collision.isTankValidMove(this, futurePosition, barriers))
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
                Interact_with_trash_and_fruit(fuelCells, gameTime);
            }
        }
        private void Interact_with_trash_and_fruit(List<FuelCell> fuelCells, GameTime gameTime)
        {
            Trash_Fruit_BoundingSphere = new BoundingSphere(BoundingSphere.Center,
                    20);
            for (int curCell = 0; curCell < fuelCells.Count; curCell++)
            {
                if (fuelCells[curCell].Retrieved == false && Trash_Fruit_BoundingSphere.Intersects(
                    fuelCells[curCell].BoundingSphere))
                {
                    fuelCells[curCell].Retrieved = true;
                    if (fuelCells[curCell].powerType == 1)
                    {
                        speedUpStartTime = gameTime.TotalGameTime.TotalSeconds;
                        speedUp = 2.0f;
                    }
                    else if (fuelCells[curCell].powerType == 2)
                    {
                        strengthUpStartTime = gameTime.TotalGameTime.TotalSeconds;
                        strengthUp = 2.0f;
                    }
                    else if (fuelCells[curCell].powerType == 3)
                    {
                        fireRateUpStartTime = gameTime.TotalGameTime.TotalSeconds;
                        fireRateUp = 2.0f;
                    }
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
    }
}
