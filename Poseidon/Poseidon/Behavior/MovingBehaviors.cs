using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon.Behavior
{
    public class MovingBehaviors
    {
        public static void updateWandering(ref Vector2 currentPosition, ref Vector2 currentVelocity, float mass, float maxSpeed, 
            float maxSteeringForce, float wanderCircleDistance, float circleRadius, BoundingSphere boundingSphere, 
            float wanderingMaxAngle, List<GameObject> obstacles, float minX, float maxX, float minZ, float maxZ) {

            Vector2 curPos = currentPosition;
            Vector2 curVel = currentVelocity;

            // Find the location of the wandering circle
            Vector2 circlePosition = currentVelocity;
            circlePosition.Normalize();
            circlePosition *= wanderCircleDistance;
            circlePosition += currentPosition;

            Random rnd = new Random();

            // Pick an angle from [-wanderingMaxAngle, +wanderingMaxAngle]
            double wanderAngle = rnd.NextDouble()*2*wanderingMaxAngle - wanderingMaxAngle;

            // Find the new destination to steer
            Vector2 circleOffset = new Vector2(circleRadius * (float)Math.Cos(wanderAngle), circleRadius * (float)Math.Sin(wanderAngle));
            Vector2 newDestination = circlePosition + circleOffset;

            // If encounter an obstacle, simply steer away (flee) from it
            float obstacleX, obstacleZ;
            if (!updateSeeking(ref currentPosition, ref currentVelocity, newDestination, mass, maxSpeed,
                maxSteeringForce, boundingSphere.Radius, obstacles, out obstacleX, out obstacleZ, minX, maxX, minZ, maxZ)) {

               updateFleeing(ref currentPosition, ref currentVelocity, new Vector2(obstacleX, obstacleZ), mass, maxSpeed, maxSteeringForce,
                        boundingSphere.Radius, obstacles, out obstacleX, out obstacleZ, minX, maxX, minZ, maxZ);
            }
        }

        public static void updateLeaderFollowing(HydroBot bot, List<GameObject> obstacles, ref Vector2 currentPosition, 
            ref Vector2 currentVelocity, float mass, float maxSpeed, float maxSteeringForce, float wanderCircleDistance, 
            float circleRadius, BoundingSphere boundingSphere, float wanderingMaxAngle, float minX, float maxX, float minZ, float maxZ) { 
            

        }

        private static bool isInSight(Vector2 currentPosition, Vector2 currentVelocity, GameObject other) { 
            //Vector2 boxHorzDirection = currentVelocity
            return false;
        }

        // Return true if successful
        public static bool updateSeeking(ref Vector2 currentPosition, ref Vector2 currentVelocity, Vector2 destination, 
            float mass, float maxSpeed, float maxSteeringForce, float boundingRadius, List<GameObject> obstacles, 
            out float obstacleX, out float obstacleZ, float minX, float maxX, float minZ, float maxZ) {

            // Make backup
            Vector2 curPos = currentPosition;
            Vector2 curVel = currentVelocity;
            // If not close to the destination
            if (Vector2.Distance(destination, curPos) > 0.001) {
                Vector2 desiredVector = destination - curPos;
                Vector2 steeringForce = desiredVector - curVel;
                truncate(ref steeringForce, maxSteeringForce);

                Vector2 dv = steeringForce / mass;

                curVel += dv;
                truncate(ref curVel, maxSpeed);
            }
            curPos += curVel;

            // Check collision 
            for (int i = 0; i < obstacles.Count; i++) {
                if (Vector2.Distance(new Vector2(obstacles[i].Position.X, obstacles[i].Position.Z), curPos) < obstacles[i].BoundingSphere.Radius + boundingRadius
                    && curPos.X >= minX && curPos.Y >= minZ && curPos.X <= maxX && curPos.Y <= maxZ) {

                    obstacleX = obstacles[i].Position.X;
                    obstacleZ = obstacles[i].Position.Z;
                    return false;
                }
            }
            currentPosition = curPos;
            currentVelocity = curVel;
            obstacleX = -1;
            obstacleZ = -1;

            return true;
        }

        public static bool updateFleeing(ref Vector2 currentPosition, ref Vector2 currentVelocity, Vector2 nonDestination,
            float mass, float maxSpeed, float maxSteeringForce, float boundingRadius, List<GameObject> obstacles,
            out float obstacleX, out float obstacleZ, float minX, float maxX, float minZ, float maxZ) {

            // Make backup
            Vector2 curPos = currentPosition;
            Vector2 curVel = currentVelocity;

            Vector2 desiredVector = curPos - nonDestination;
            Vector2 steeringForce = desiredVector - curVel;
            truncate(ref steeringForce, maxSteeringForce);

            Vector2 dv = steeringForce / mass;

            curVel += dv;
            truncate(ref curVel, maxSpeed);

            curPos += curVel;

            // Check collision 
            for (int i = 0; i < obstacles.Count; i++) {
                if (Vector2.Distance(new Vector2(obstacles[i].Position.X, obstacles[i].Position.Z), curPos) < obstacles[i].BoundingSphere.Radius + boundingRadius
                    && curPos.X >= minX && curPos.Y >= minZ && curPos.X <= maxX && curPos.Y <= maxZ) {
                    
                    obstacleX = obstacles[i].Position.X;
                    obstacleZ = obstacles[i].Position.Z;

                    return false;
                }
            }
            currentPosition = curPos;
            currentVelocity = curVel;
            obstacleX = -1;
            obstacleZ = -1;          
  
            return true;
        }

        public static bool updateArriving(ref Vector2 currentPosition, ref Vector2 currentVelocity, Vector2 destination,
            float mass, float maxSpeed, float maxSteeringForce, float boundingRadius, float brakingRadius,
            List<GameObject> obstacles, float minX, float maxX, float minZ, float maxZ) { 
        
            // Back up
            Vector2 curPos = currentPosition;
            Vector2 curVel = currentVelocity;
            float distance = Vector2.Distance(curPos, destination);
            float rampedSpeed = maxSpeed * (distance / brakingRadius);
            float speed = Math.Min(rampedSpeed, maxSpeed);

            // Add 0.001 to avoid NaN
            Vector2 desiredVector = (float)(speed / (distance + 0.00001)) * (destination - curPos);

            Vector2 steeringForce = desiredVector - curVel;
            truncate(ref steeringForce, maxSteeringForce);
            Vector2 dv = steeringForce / mass;
            curVel += dv;

            if (distance < 0.00001) {
                curVel = new Vector2();
            }

            curPos += curVel;

            // Check collision 
            for (int i = 0; i < obstacles.Count; i++) {
                if (Vector2.Distance(new Vector2(obstacles[i].Position.X, obstacles[i].Position.Z), curPos) < obstacles[i].BoundingSphere.Radius + boundingRadius
                    && curPos.X >= minX && curPos.Y >= minZ && curPos.X <= maxX && curPos.Y <= maxZ) {
                    return false;
                }
            }
            currentPosition = curPos;
            currentVelocity = curVel;

            return true;
        }

        private static void truncate(ref Vector2 vector, float maxMagnitude) {
            float magnitude = vector.Length();
            magnitude = Math.Min(magnitude, maxMagnitude);

            vector.Normalize();
            vector *= magnitude;
        }
    }
}
