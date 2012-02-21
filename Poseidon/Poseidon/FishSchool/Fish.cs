#region File Description
//-----------------------------------------------------------------------------
// Fish.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace Poseidon.FishSchool
{
    public class Fish : Animal
    {
        #region Fields

        public Color fogColor;
        public Color ambientColor;
        public Color diffuseColor;
        public Color specularColor;

        protected Random random;
        Vector3 aiNewDir;
        int aiNumSeen;
        
        //location and direction in 3D world
        //Vector3 loc3D;
        //Vector3 dir3D;
        #endregion
        FleeBehavior fleeReaction;
        #region Initialization
        /// <summary>
        /// Bird constructor
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="dir">movement direction</param>
        /// <param name="loc">spawn location</param>
        /// <param name="screenSize">screen size</param>
        public Fish(Texture2D tex, Vector3 dir, Vector3 loc,
            int screenWidth, int screenHeight)
            : base(tex, screenWidth, screenHeight)
        {
            direction = dir;
            direction.Normalize();
            //dir3D = new Vector3(direction.X, 0, direction.Y);

            location = loc;
            //loc3D = new Vector3(loc.X, 0, loc.Y);

            moveSpeed = 30.0f;// 125.0f;
            fleeing = false;
            random = new Random((int)loc.X + (int)loc.Z);
            animaltype = AnimalType.Fish;
            BuildBehaviors();
        }
        #endregion

        #region Update and Draw

        /// <summary>
        /// update fish position, wrapping around the screen edges
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime, ref AIParameters aiParams)
        {
            EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector3 randomDir = Vector3.Zero;

            randomDir.X = (float)random.NextDouble() - 0.5f;
            randomDir.Y = 0;
            randomDir.Z = (float)random.NextDouble() - 0.5f;
            Vector3.Normalize(ref randomDir, out randomDir);

            if (aiNumSeen > 0)
            {
                aiNewDir = (direction * aiParams.MoveInOldDirectionInfluence) +
                    (aiNewDir * (aiParams.MoveInFlockDirectionInfluence /
                    (float)aiNumSeen));
            }
            else
            {
                aiNewDir = direction * aiParams.MoveInOldDirectionInfluence;
            }

            aiNewDir += (randomDir * aiParams.MoveInRandomDirectionInfluence);
            Vector3.Normalize(ref aiNewDir, out aiNewDir);
            aiNewDir = ChangeDirection(direction, aiNewDir,
                aiParams.MaxTurnRadians * elapsedTime);
            direction = aiNewDir;

            if (direction.LengthSquared() > .01f)
            {
                Vector3 moveAmount = direction * moveSpeed * elapsedTime;
                location = location + moveAmount;

                if (Math.Abs(location.X) > boundryWidth + 200) location.X = -location.X;
                if (Math.Abs(location.Z) > boundryHeight + 200) location.Z = -location.Z;
            }
        }

        /// <summary>
        /// Draw the fish, tinting it if it's currently fleeing
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Color tintColor = color;
            float rotation = 0.0f;
            rotation = (float)Math.Atan2(direction.Z, direction.X);

            // if the entity is highlighted, we want to make it pulse with a red tint.
            if (fleeing)
            {
                // to do this, we'll first generate a value t, which we'll use to
                // determine how much tint to have.
                float t = (float)Math.Sin(10 * PoseidonGame.playTime.TotalSeconds);

                // Sin varies from -1 to 1, and we want t to go from 0 to 1, so we'll 
                // scale it now.
                t = .5f + .5f * t;

                // finally, we'll calculate our tint color by using Lerp to generate
                // a color in between Red and White.
                tintColor = new Color(Vector4.Lerp(
                    Color.Red.ToVector4(), Color.White.ToVector4(), t));
            }
            Vector3 screenPos = PlayGameScene.GraphicDevice.Viewport.Project(location, PlayGameScene.gameCamera.ProjectionMatrix, PlayGameScene.gameCamera.ViewMatrix, Matrix.Identity);
            Vector2 loc2D = new Vector2(screenPos.X, screenPos.Y);
            // Draw the animal, centered around its position, and using the 
            //orientation and tint color
            //spriteBatch.Draw(texture, location, null, tintColor,
            //    rotation, textureCenter, 1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(texture, loc2D, null, specularColor,
                rotation, textureCenter, 1.0f, SpriteEffects.None, 0.0f);
        }
        #endregion

        #region Methods

        /// <summary>
        /// Instantiates all the behaviors that this fish knows about
        /// </summary>
        public void BuildBehaviors()
        {
            fleeReaction = new FleeBehavior(this);

            Behaviors fishReactions = new Behaviors();
            fishReactions.Add(new AlignBehavior(this));
            fishReactions.Add(new CohesionBehavior(this));
            fishReactions.Add(new SeparationBehavior(this));
            behaviors.Add(AnimalType.Fish, fishReactions);
        }
        /// <summary>
        /// Setup the fish to figure out it's new movement direction
        /// </summary>
        /// <param name="AIparams">flock AI parameters</param>
        public void ResetThink()
        {
            Fleeing = false;
            aiNewDir = Vector3.Zero;
            aiNumSeen = 0;
            reactionDistance = 0f;
            reactionLocation = Vector3.Zero;
        }

        /// <summary>
        /// Since we're wrapping movement around the screen, two point at extreme 
        /// sides of the screen are actually very close together, this function 
        /// figures out if destLocation is closer the srcLocation if you wrap around
        /// the screen
        /// </summary>
        /// <param name="srcLocation">screen location of src</param>
        /// <param name="destLocation">screen location of dest</param>
        /// <param name="outVector">relative location of dest to src</param>
        private void ClosestLocation(ref Vector3 srcLocation,
            ref Vector3 destLocation, out Vector3 outLocation)
        {
            outLocation = new Vector3();
            float x = destLocation.X;
            float z = destLocation.Z;
            float dX = Math.Abs(destLocation.X - srcLocation.X);
            float dZ = Math.Abs(destLocation.Z - srcLocation.Z);

            // now see if the distance between fishes is closer if going off one
            // side of the map and onto the other.
            if (Math.Abs(boundryWidth - destLocation.X + srcLocation.X) < dX)
            {
                dX = boundryWidth - destLocation.X + srcLocation.X;
                x = destLocation.X - boundryWidth;
            }
            if (Math.Abs(boundryWidth - srcLocation.X + destLocation.X) < dX)
            {
                dX = boundryWidth - srcLocation.X + destLocation.X;
                x = destLocation.X + boundryWidth;
            }

            if (Math.Abs(boundryHeight - destLocation.Z + srcLocation.Z) < dZ)
            {
                dZ = boundryHeight - destLocation.Y + srcLocation.Y;
                z = destLocation.Y - boundryHeight;
            }
            if (Math.Abs(boundryHeight - srcLocation.Z + destLocation.Z) < dZ)
            {
                dZ = boundryHeight - srcLocation.Y + destLocation.Y;
                z = destLocation.Y + boundryHeight;
            }
            outLocation.X = x;
            outLocation.Z = z;
        }

        /// <summary>
        /// React to an Animal based on it's type
        /// </summary>
        /// <param name="animal"></param>
        public void ReactTo(Animal animal, ref AIParameters AIparams)
        {
            if (animal != null)
            {
                //setting the the reactionLocation and reactionDistance here is
                //an optimization, many of the possible reactions use the distance
                //and location of theAnimal, so we might as well figure them out
                //only once !
                Vector3 otherLocation = animal.Location;
                ClosestLocation(ref location, ref otherLocation,
                    out reactionLocation);
                reactionDistance = Vector3.Distance(location, reactionLocation);

                //we only react if theAnimal is close enough that we can see it
                if (reactionDistance < AIparams.DetectionDistance)
                {
                    Behaviors reactions = behaviors[animal.AnimalType];
                    foreach (Behavior reaction in reactions)
                    {
                        reaction.Update(animal, AIparams);
                        if (reaction.Reacted)
                        {
                            aiNewDir += reaction.Reaction;
                            aiNumSeen++;
                        }
                    }
                }
            }
        }
        public void ReactToMainCharacter(HydroBot tank, ref AIParameters AIparams)
        {
            if (tank != null)
            {
                //setting the the reactionLocation and reactionDistance here is
                //an optimization, many of the possible reactions use the distance
                //and location of theAnimal, so we might as well figure them out
                //only once !
                Vector3 otherLocation = tank.Position;
                ClosestLocation(ref location, ref otherLocation,
                    out reactionLocation);
                reactionDistance = Vector3.Distance(location, reactionLocation);

                //we only react if theAnimal is close enough that we can see it
                if (reactionDistance < AIparams.DetectionDistance)
                {
                    fleeReaction.Update(tank, AIparams);

                    if (fleeReaction.Reacted)
                    {
                        aiNewDir += fleeReaction.Reaction;
                        aiNumSeen++;
                    }

                }
            }
        }
        public void ReactToSwimmingObject(SwimmingObject swimmingObject, ref AIParameters AIparams)
        {
            if (swimmingObject != null)
            {
                //setting the the reactionLocation and reactionDistance here is
                //an optimization, many of the possible reactions use the distance
                //and location of theAnimal, so we might as well figure them out
                //only once !
                Vector3 otherLocation = swimmingObject.Position;
                ClosestLocation(ref location, ref otherLocation,
                    out reactionLocation);
                reactionDistance = Vector3.Distance(location, reactionLocation);

                //we only react if theAnimal is close enough that we can see it
                if (reactionDistance < AIparams.DetectionDistance)
                {
                    fleeReaction.Update(swimmingObject, AIparams);

                    if (fleeReaction.Reacted)
                    {
                        aiNewDir += fleeReaction.Reaction;
                        aiNumSeen++;
                    }

                }
            }
        }

        /// <summary>
        /// This function clamps turn rates to no more than maxTurnRadians
        /// </summary>
        /// <param name="oldDir">current movement direction</param>
        /// <param name="newDir">desired movement direction</param>
        /// <param name="maxTurnRadians">max turn in radians</param>
        /// <returns></returns>
        private static Vector3 ChangeDirection(
            Vector3 oldDir, Vector3 newDir, float maxTurnRadians)
        {
            float oldAngle = (float)Math.Atan2(oldDir.Z, oldDir.X);
            float desiredAngle = (float)Math.Atan2(newDir.Z, newDir.X);
            float newAngle = MathHelper.Clamp(desiredAngle, WrapAngle(
                    oldAngle - maxTurnRadians), WrapAngle(oldAngle + maxTurnRadians));
            return new Vector3((float)Math.Cos(newAngle), 0, (float)Math.Sin(newAngle));
        }
        /// <summary>
        /// clamps the angle in radians between -Pi and Pi.
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        private static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }
        #endregion
    }
}