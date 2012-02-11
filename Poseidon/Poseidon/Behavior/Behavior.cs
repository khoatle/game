using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon
{
    public class Behavior {

        public static List<GameObject> getAllObstacle(GameObject observer, Fish[] fishes, int fishAmount, BaseEnemy[] enemies, 
            int enemyAmount, HydroBot bot, List<GameObject> exclude)
        {
            List<GameObject> obstacles = new List<GameObject>();
            if (observer != bot) {
                obstacles.Add(bot);
            }
            for (int i = 0; i < enemyAmount; i++) {
                if (observer != enemies[i]) {
                    obstacles.Add(enemies[i]);
                }
            }
            for (int i = 0; i < fishAmount; i++) {
                if (observer != fishes[i]) {
                    obstacles.Add(fishes[i]);
                }
            }

            for (int i = 0; i < obstacles.Count; i++) {
                for (int j = 0; j < exclude.Count; j++) {
                    if (obstacles[i] == exclude[j]) {
                        obstacles.RemoveAt(i);
                    }
                }
            }

            return obstacles;
        }

        public static bool isInSight(Vector3 observerDirection, Vector3 observerPosition, Vector3 otherDirection, Vector3 otherPosition, float inSightAngle, float threshold) {
            Vector2 observerDir2 = new Vector2(observerDirection.X, observerDirection.Z);
            Vector2 otherDir2 = new Vector2(otherDirection.X, otherDirection.Z);

            float dot = Vector2.Dot(observerDir2, otherDir2);
            float angle = dot / (observerDir2.Length() * otherDir2.Length());
            angle = (float)Math.Acos(angle);

            float distance = Vector3.Distance(otherPosition, observerPosition);

            if (angle > inSightAngle / 2)
                return false;
            else {
                return distance <= threshold;
            }
        }

        //public static Vector3 followLeader(Vector3 leaderPosition, Vector3 leaderVelocity, Vector3 currentPosition, Vector3 currentVelocity,
        //    float maxSpeed, float maxSteeringForce)
        //{
        //    if (isInSight(leaderPosition, currentPosition, 50f)) {
        //        Vector3 fleeFrom = leaderVelocity;
        //        fleeFrom.Normalize();
        //        fleeFrom = leaderPosition + fleeFrom * 40f;

        //        Vector3 futurePosition = flee(currentPosition, fleeFrom, currentVelocity, maxSpeed, maxSteeringForce);

        //        return futurePosition;
        //    }
        //    return new Vector3();
        //}

        //public static Vector3 separate(Vector3 currentPosition, Vector3 currentVelocity, out float nextSpeed,
        //    float maxSpeed, float maxSteeringForce, List<GameObject> obstacles) {

        //    float desiredSeparation = 20f;
        //    Vector2 currentPos2 = new Vector2(currentPosition.X, currentPosition.Z);
        //    Vector2 desiredDirection = new Vector2();
        //    Vector2 currentVel2 = new Vector2(currentVelocity.X, currentVelocity.Z);
        //    Vector2 steeringForce = new Vector2();

        //    int count = 0;
        //    foreach (GameObject other in obstacles) {
        //        Vector2 otherPos2 = new Vector2(other.Position.X, other.Position.Z);

        //        float distance = Vector2.Distance(otherPos2, currentPos2);
        //        if (distance > 0 && distance < desiredSeparation) {
        //            Vector2 diff = currentPos2 - otherPos2;
        //            diff.Normalize();
        //            diff /= distance;
        //            desiredDirection += diff;

        //            count++;
        //        }
        //    }
            
        //    if (count > 0) {
        //        desiredDirection /= count;
        //    }
        //    // If there is an obstacle nearby
        //    if (desiredDirection.Length() > 0)
        //    {
        //        steeringForce = desiredDirection;
        //        steeringForce.Normalize();
        //        steeringForce *= maxSpeed;

        //        steeringForce -= currentVel2;

        //        if (steeringForce.Length() > maxSteeringForce)
        //        {
        //            steeringForce.Normalize();
        //            steeringForce *= maxSteeringForce;
        //        }

        //        currentVel2 += steeringForce;
        //        currentPos2 += currentVel2;
        //    }
        //    nextSpeed = currentVel2.Length();

        //    return new Vector3(currentPos2.X, currentPosition.Y, currentPos2.Y);
        //}

        //public static Vector3 arrive(Vector3 currentPosition, Vector3 target, Vector3 currentVelocity, out float nextSpeed, 
        //    float maxSpeed, float slowingDistance, float maxSteeringForce) {

        //    Vector2 currentPos2 = new Vector2(currentPosition.X, currentPosition.Z);
        //    Vector2 targetPos2 = new Vector2(target.X, target.Z);
        //    Vector2 currentVel2 = new Vector2(currentVelocity.X, currentVelocity.Z);

        //    Vector2 targetOffset = targetPos2 - currentPos2;
        //    float distance = targetOffset.Length();
        //    float rampedSpeed = maxSpeed * ((distance + 0.01f) / slowingDistance); // +0.01 to avoid NaN
        //    float clippedSpeed = Math.Min(rampedSpeed, maxSpeed);

        //    Vector2 desiredVelocity = targetOffset * (clippedSpeed / (distance + 0.01f)); // +0.01 to avoid NaN
        //    Vector2 steeringForce = desiredVelocity - currentVel2;
        //    if (steeringForce.Length() > maxSteeringForce) {
        //        steeringForce.Normalize();
        //        steeringForce *= maxSteeringForce;
        //    }
        //    currentVel2 += steeringForce;
        //    if (currentVel2.Length() > maxSpeed) {
        //        currentVel2.Normalize();
        //        currentVel2 *= maxSpeed;
        //    }
        //    currentPos2 += currentVel2;
        //    nextSpeed = currentVel2.Length();

        //    return new Vector3(currentPos2.X, currentPosition.Y, currentPos2.Y);
        //}

        //public static Vector3 flee(Vector3 currentPosition, Vector3 target, Vector3 currentVelocity, float maxSpeed,
        //    float maxSteeringForce) { 

        //    Vector2 currentPos2 = new Vector2(currentPosition.X, currentPosition.Z);
        //    Vector2 targetPos2 = new Vector2(target.X, target.Z);
        //    Vector2 currentVel2 = new Vector2(currentVelocity.X, currentVelocity.Z);

        //    Vector2 desiredDirection = currentPos2 - targetPos2;
            
        //    desiredDirection.Normalize();
        //    desiredDirection *= maxSpeed;

        //    Vector2 steerForce = desiredDirection - currentVel2;
        //    if (steerForce.Length() > maxSteeringForce) {
        //        steerForce.Normalize();
        //        steerForce *= maxSteeringForce;
        //    }
        //    currentVel2 += steerForce;

        //    if (currentVel2.Length() > maxSpeed) {
        //        currentVel2.Normalize();
        //        currentVel2 *= maxSpeed;
        //    }
        //    currentPos2 += currentVel2;

        //    return new Vector3(currentPos2.X, currentPosition.Y, currentPos2.Y);
        //}
    }
}
