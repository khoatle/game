using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon
{
    public class Camera
    {
        public Vector3 AvatarHeadOffset { get; set; }
        public Vector3 TargetOffset { get; set; }
        public Matrix ViewMatrix { get; set; }
        public Matrix ProjectionMatrix { get; set; }
        // Are we shaking?
        public bool shaking;

        // The maximum magnitude of our shake offset
        private float shakeMagnitude;

        // The total duration of the current shake
        private float shakeDuration;

        // A timer that determines how far into our shake we are
        private float shakeTimer;

        // The shake offset vector
        private Vector3 shakeOffset;

        // We only need one Random object no matter how many Cameras we have
        private static readonly Random random = new Random();

        private GameMode gameMode;
        private float camHeight = 0;
        private float gameFloatHeight = 0;
        private int MaxRangeX = 0;
        private int MaxRangeZ = 0;
        private float HalfScreenX = 0;
        private float HalfScreenZ = 0;
        private bool cameraRestrictionCanculated = false;

        public Camera(GameMode gameMode)
        {
            
            if (gameMode == GameMode.MainGame || gameMode == GameMode.SurvivalMode)
            {
                camHeight = GameConstants.MainCamHeight;
                gameFloatHeight = GameConstants.MainGameFloatHeight;
                MaxRangeX = GameConstants.MainGameMaxRangeX + 8;
                MaxRangeZ = GameConstants.MainGameMaxRangeZ + 5;
            }
            else if (gameMode == GameMode.ShipWreck)
            {
                camHeight = GameConstants.ShipCamHeight;
                gameFloatHeight = GameConstants.ShipWreckFloatHeight;
                MaxRangeX = GameConstants.ShipWreckMaxRangeX + 8;
                MaxRangeZ = GameConstants.ShipWreckMaxRangeZ + 5;
            }
            this.gameMode = gameMode;

            AvatarHeadOffset = new Vector3(0, camHeight, 0);
            TargetOffset = new Vector3(0, 0, 0);
            ViewMatrix = Matrix.Identity;
            ProjectionMatrix = Matrix.Identity;
        }

        /// <summary>
        /// Shakes the camera with a specific magnitude and duration.
        /// </summary>
        /// <param name="magnitude">The largest magnitude to apply to the shake.</param>
        /// <param name="duration">The length of time (in seconds) for which the shake should occur.</param>
        public void Shake(float magnitude, float duration)
        {
            // We're now shaking
            shaking = true;

            // Store our magnitude and duration
            shakeMagnitude = magnitude;
            shakeDuration = duration;

            // Reset our timer
            shakeTimer = 0f;
        }

        public void CalculateCameraRestriction(Cursor cursor)
        {
            Vector2 oldPosition = cursor.Position;
            cursor.SetPosition(new Vector2(PoseidonGame.graphics.PreferredBackBufferWidth, PoseidonGame.graphics.PreferredBackBufferHeight));
            Vector3 pointIntersect = CursorManager.IntersectPointWithPlane(cursor, this, gameFloatHeight);
            HalfScreenX = Math.Abs(pointIntersect.X);
            HalfScreenZ =  Math.Abs(pointIntersect.Z);
            cameraRestrictionCanculated = true;
            cursor.SetPosition(oldPosition);
        }
        public void Update(float avatarYaw, Vector3 position, float aspectRatio, GameTime gameTime, Cursor cursor)
        {
            if (gameMode != GameMode.ShipWreck)
            {
                if (position.X >= MaxRangeX - HalfScreenX)
                    position.X = MaxRangeX - HalfScreenX;
                else if (position.X <= -MaxRangeX + HalfScreenX)
                    position.X = -MaxRangeX + HalfScreenX;
                if (position.Z >= MaxRangeZ - HalfScreenZ)
                    position.Z = MaxRangeZ - HalfScreenZ;
                else if (position.Z <= -MaxRangeZ + HalfScreenZ)
                    position.Z = -MaxRangeZ + HalfScreenZ;
            }

            // If we're shaking...
            if (shaking)
            {
                // Move our timer ahead based on the elapsed time
                shakeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                // If we're at the max duration, we're not going to be shaking anymore
                if (shakeTimer >= shakeDuration)
                {
                    shaking = false;
                    shakeTimer = shakeDuration;
                }

                // Compute our progress in a [0, 1] range
                float progress = shakeTimer / shakeDuration;

                // Compute our magnitude based on our maximum value and our progress. This causes
                // the shake to reduce in magnitude as time moves on, giving us a smooth transition
                // back to being stationary. We use progress * progress to have a non-linear fall 
                // off of our magnitude. We could switch that with just progress if we want a linear 
                // fall off.
                float magnitude = shakeMagnitude * (1f - (progress * progress));

                // Generate a new offset vector with three random values and our magnitude
                shakeOffset = new Vector3(NextFloat(), NextFloat(), NextFloat()) * magnitude;
            }

            Matrix rotationMatrix = Matrix.CreateRotationY(avatarYaw);

            Vector3 transformedheadOffset =
                Vector3.Transform(AvatarHeadOffset, rotationMatrix);
            Vector3 transformedReference =
                Vector3.Transform(TargetOffset, rotationMatrix);

            Vector3 cameraPosition = position + transformedheadOffset;

            Vector3 cameraTarget = position + transformedReference;
            Vector3 camRot = Vector3.Transform(Vector3.UnitZ, rotationMatrix);
            // If we're shaking, add our offset to our position and target
            if (shaking)
            {
                cameraPosition += shakeOffset;
                cameraTarget += shakeOffset;
            }
            //Calculate the camera's view and projection
            // matrices based on current values.
            ViewMatrix =
                Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.UnitZ);
            ProjectionMatrix =
                Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(GameConstants.ViewAngle), aspectRatio,
                    GameConstants.NearClip, GameConstants.FarClip);

            if (!cameraRestrictionCanculated) CalculateCameraRestriction(cursor);

        }
        /// <summary>
        /// Helper to generate a random float in the range of [-1, 1].
        /// </summary>
        private float NextFloat()
        {
            return (float)random.NextDouble() * 2f - 1f;
        }
    }
}
