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

        public void LoadContent(ContentManager content, Vector3 plantPosition)
        {
            if(this.powerType ==1 )
                Model = content.Load<Model>("Models/green-fruit");
            else if (this.powerType == 2)
                Model = content.Load<Model>("Models/red-fruit");
            else if (this.powerType == 3)
                Model = content.Load<Model>("Models/blue-fruit");
            Position = plantPosition;
            Position.Y = GameConstants.MainGameFloatHeight;
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