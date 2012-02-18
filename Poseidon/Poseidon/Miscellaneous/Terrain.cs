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
    public class Terrain
    {
        public Model terrainModel;
        Random random = new Random();
        public HeightMapInfo heightMapInfo;

        public Terrain(ContentManager Content)
        {
            //Uncomment below line to use LEVELS
            //string terrain_name = "Image/terrain" + currentLevel;
            int random_level = random.Next(20);
            string terrain_name = "Image/TerrainHeightMaps/terrain" + random_level;
            terrainModel = Content.Load<Model>(terrain_name);
            heightMapInfo = terrainModel.Tag as HeightMapInfo;
            if (heightMapInfo == null)
            {
                string message = "The terrain model did not have a HeightMapInfo " +
                    "object attached. Are you sure you are using the " +
                    "TerrainProcessor?";
                throw new InvalidOperationException(message);
            }
            // Set up the parameters
            //SetupShaderParameters(PoseidonGame.contentManager, terrainModel);
        }

        public void Update()
        {

        }

        // our custom shader
        Effect newBasicEffect;

        public void SetupShaderParameters(ContentManager content, Model model)
        {
            newBasicEffect = content.Load<Effect>("Shaders/NewBasicEffect");
            EffectHelpers.ChangeEffectUsedByModelToCustomBasicEffect(model, newBasicEffect);
        }

        public void Draw(Camera gameCamera)
        {
            foreach (ModelMesh mesh in terrainModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                //foreach (Effect effect in mesh.Effects)
                {
                    //effect.AmbientLightColor = new Vector3(0, 191.0f / 255.0f, 1);
                    //effect.Alpha = 1.0f;
                    //effect.EnableDefaultLighting();
                    //effect.SpecularColor = Vector3.One;
                    //effect.DirectionalLight1.Enabled = false;
                    //effect.DirectionalLight2.Enabled = false;
                    //effect.DirectionalLight0.Enabled = false;
                    //effect.DirectionalLight0.Direction = new Vector3(0, 1, 0);
                    //effect.DirectionalLight0.DiffuseColor = new Vector3(0, 1, 1);
                    //effect.DirectionalLight0.SpecularColor = new Vector3(135.0f / 255.0f, 206.0f / 255.0f, 250.0f / 255.0f);
                    //effect.SpecularPower = 1.0f;
                    //effect.PreferPerPixelLighting = true;
                    effect.World = Matrix.Identity;

                    // Use the matrices provided by the game camera
                    effect.View = gameCamera.ViewMatrix;
                    effect.Projection = gameCamera.ProjectionMatrix;

                    effect.FogEnabled = true;
                    effect.FogStart = GameConstants.FogStart;
                    effect.FogEnd = GameConstants.FogEnd;
                    effect.FogColor = GameConstants.FogColor.ToVector3();

                    //for our custom BasicEffect
                    //effect.CurrentTechnique = effect.Techniques[techniqueName];
                    //effect.Parameters["World"].SetValue(Matrix.Identity);
                    //effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Invert(Matrix.Identity));
                    //effect.Parameters["View"].SetValue(gameCamera.ViewMatrix);
                    //effect.Parameters["Projection"].SetValue(gameCamera.ProjectionMatrix);
                    //effect.Parameters["EyePosition"].SetValue(new Vector4(gameCamera.AvatarHeadOffset, 0));
                    //Matrix WorldView = Matrix.Identity * gameCamera.ViewMatrix;
                    //EffectHelpers.SetFogVector(ref WorldView, GameConstants.FogStart, GameConstants.FogEnd, effect.Parameters["FogVector"]);
                    //effect.Parameters["FogColor"].SetValue(GameConstants.FogColor.ToVector3());
                }
                mesh.Draw();
            }
        }
    }
}
