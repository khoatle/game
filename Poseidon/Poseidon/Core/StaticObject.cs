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
        float orientation = 0;
        public static Random random = new Random();
        float scale;

        public void LoadContent(ContentManager content, string modelname)
        {
            if (HydroBot.gameMode == GameMode.MainGame)
            {
                orientation = (float)random.Next(0, 629) / 100;
                scale = (float)random.Next(5, 11)/ 10;
            }
            
            //Model = content.Load<Model>("Models/shark2");
            Model = content.Load<Model>(modelname);
            Position = Vector3.Down;
            BoundingSphere = CalculateBoundingSphere();

            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;
            if (HydroBot.gameMode == GameMode.ShipWreck)
                scaledSphere.Radius *= GameConstants.ShipWreckBoundingSphereFactor;
            else scaledSphere.Radius *= scale;
            BoundingSphere =
                new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);



            // Set up the parameters
            //SetupShaderParameters(content, Model);
             
        }

        public void Update()
        {
            
        }

        public void Draw(Matrix view, Matrix projection)
        {
            EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);

            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix translateMatrix = Matrix.CreateScale(scale) * Matrix.CreateTranslation(Position);
            Matrix rotationYMatrix = Matrix.CreateRotationY(orientation);
            //Matrix scaleMatrix = Matrix.CreateScale(scale);
            Matrix worldMatrix = rotationYMatrix * translateMatrix;

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World =
                        worldMatrix * transforms[mesh.ParentBone.Index];
                    effect.View = view;
                    effect.Projection = projection;

                    //effect.EnableDefaultLighting();
                    effect.DirectionalLight1.Enabled = false;
                    effect.DirectionalLight2.Enabled = false;
                    effect.DirectionalLight0.Enabled = true;
                    effect.DirectionalLight0.Direction = new Vector3(0, 1, 0);
                    effect.DirectionalLight0.DiffuseColor = diffuseColor.ToVector3();
                    //effect.DirectionalLight0.SpecularColor = new Vector3(135.0f / 255.0f, 206.0f / 255.0f, 250.0f / 255.0f);
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
