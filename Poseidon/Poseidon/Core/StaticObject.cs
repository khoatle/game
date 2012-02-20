using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon
{
    public class StaticObject : GameObject
    {
            public void LoadContent(ContentManager content, string modelname)
            {

                //Model = content.Load<Model>("Models/shark2");
                Model = content.Load<Model>(modelname);
                Position = Vector3.Down;
                BoundingSphere = CalculateBoundingSphere();

                BoundingSphere scaledSphere;
                scaledSphere = BoundingSphere;
                scaledSphere.Radius *= GameConstants.ShipWreckBoundingSphereFactor;
                BoundingSphere =
                    new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);

                // Set up the parameters
                //SetupShaderParameters(content, Model);
                EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);
            }

            public void Draw(Matrix view, Matrix projection)
            {
                Matrix[] transforms = new Matrix[Model.Bones.Count];
                Model.CopyAbsoluteBoneTransformsTo(transforms);
                Matrix translateMatrix = Matrix.CreateTranslation(Position);
                Matrix worldMatrix = translateMatrix;

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

                        effect.AmbientLightColor = ambientColor.ToVector3();
                        effect.DiffuseColor = diffuseColor.ToVector3();
                        effect.SpecularColor = specularColor.ToVector3();

                        effect.FogEnabled = true;
                        effect.FogStart = GameConstants.FogStart;
                        effect.FogEnd = GameConstants.FogEnd;
                        effect.FogColor = fogColor.ToVector3();
                    }
                    mesh.Draw();
                }
            }
    }
}
