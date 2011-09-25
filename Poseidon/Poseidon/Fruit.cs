using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon
{
    public class Fruit : FuelCell
    {
        public Fruit(int powerType)
            : base(powerType)
        {
        }

        public void LoadContent(ContentManager content, string modelName, Vector3 plantPosition)
        {
            Model = content.Load<Model>(modelName);
            Position = plantPosition;
            Position.Y = GameConstants.FloatHeight;
            BoundingSphere = CalculateBoundingSphere();
            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;
            scaledSphere.Center = Position;
            scaledSphere.Radius *=
                GameConstants.FruitBoundingSphereFactor;
            BoundingSphere =
                new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);
        }
    }
}