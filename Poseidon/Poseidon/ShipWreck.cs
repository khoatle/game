﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon
{
    public class ShipWreck : GameObject
    {
        // different types of ship wreck
        // or maybe just its orientation
        int type;
        float orientation;
        // no re-access
        public bool accessed;
        // special skill's ID that this ship wreck will have in one of its chests
        public int skillID = 0;
        public void LoadContent(ContentManager content, int type, int skillID, float orientation)
        {
            
            this.type = type;
            switch (type)
            {
                case 0:
                    Model = content.Load<Model>("Models/ShipWreckModels/shipwreck2");
                    break;
                case 1:
                    Model = content.Load<Model>("Models/ShipWreckModels/shipwreck3");
                    break;
                case 2:
                    Model = content.Load<Model>("Models/ShipWreckModels/shipwreck4");
                    break;
            }
            
            Position = Vector3.Down;
            BoundingSphere = CalculateBoundingSphere();

            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;
            scaledSphere.Radius *= GameConstants.ShipWreckBoundingSphereFactor;
            BoundingSphere =
                new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);
            accessed = false;
            this.skillID = skillID;
            this.orientation = orientation;
        }

        public void Draw(Matrix view, Matrix projection)
        {
            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix translateMatrix = Matrix.CreateTranslation(Position);
            Matrix rotationYMatrix = Matrix.CreateRotationY(orientation);
            Matrix worldMatrix = rotationYMatrix * translateMatrix;

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World =
                        worldMatrix * transforms[mesh.ParentBone.Index];
                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;

                    effect.FogEnabled = true;
                    effect.FogStart = GameConstants.FogStart;
                    effect.FogEnd = GameConstants.FogEnd;
                    effect.FogColor = GameConstants.FogColor.ToVector3();
                }
                mesh.Draw();
            }
        }
    }
}
