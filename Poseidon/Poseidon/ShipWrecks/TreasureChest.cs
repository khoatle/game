using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon
{
    public class TreasureChest : GameObject
    {
        // different types of treasure chest
        // or maybe just its orientation
        int type;
        public float orientation;
        // no re-open
        public bool opened;
        // special skill's ID that this chest will contain
        public int skillID = -1;
        public void LoadContent(ContentManager content, int type, int skillID)
        {

            this.type = type;
            Model = content.Load<Model>("Models/ShipWreckModels/chest-closed");
            Position = Vector3.Down;
            BoundingSphere = CalculateBoundingSphere();

            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;
            scaledSphere.Radius *= GameConstants.TreasureChestSphereFactor;
            BoundingSphere =
                new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);
            opened = false;
            this.skillID = skillID;

            // Set up the parameters
            SetupShaderParameters(PoseidonGame.contentManager, Model);
            EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);
        }

        // our custom shader
        Effect newBasicEffect;

        public void SetupShaderParameters(ContentManager content, Model model)
        {
            newBasicEffect = content.Load<Effect>("Shaders/NewBasicEffect");
            EffectHelpers.ChangeEffectUsedByModelToCustomBasicEffect(model, newBasicEffect);
        }

        public void Draw(Matrix view, Matrix projection, Camera gameCamera, string techniqueName)
        {
            EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);

            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix translateMatrix = Matrix.CreateTranslation(Position);
            Matrix rotationYMatrix = Matrix.CreateRotationY(orientation);
            Matrix worldMatrix = rotationYMatrix * translateMatrix;

            foreach (ModelMesh mesh in Model.Meshes)
            {
                //foreach (BasicEffect effect in mesh.Effects)
                foreach (Effect effect in mesh.Effects)
                {
                    //effect.World =
                    //    worldMatrix * transforms[mesh.ParentBone.Index];
                    //effect.View = view;
                    //effect.Projection = projection;

                    //effect.EnableDefaultLighting();
                    //effect.PreferPerPixelLighting = true;

                    //effect.FogEnabled = true;
                    //effect.FogStart = GameConstants.FogStart;
                    //effect.FogEnd = GameConstants.FogEnd;
                    //effect.FogColor = GameConstants.FogColor.ToVector3();

                    //for our custom BasicEffect
                    effect.Parameters["AmbientColor"].SetValue(ambientColor.ToVector4());
                    effect.Parameters["DiffuseColor"].SetValue(diffuseColor.ToVector4());
                    effect.Parameters["SpecularColor"].SetValue(specularColor.ToVector4());
                    effect.CurrentTechnique = effect.Techniques[techniqueName];
                    Matrix readlWorldMatrix = worldMatrix * transforms[mesh.ParentBone.Index];
                    effect.Parameters["World"].SetValue(readlWorldMatrix);
                    effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Invert(readlWorldMatrix));
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                    effect.Parameters["EyePosition"].SetValue(new Vector4(gameCamera.AvatarHeadOffset, 0));
                    Matrix WorldView = readlWorldMatrix * view;
                    EffectHelpers.SetFogVector(ref WorldView, GameConstants.FogStart, GameConstants.FogEnd, effect.Parameters["FogVector"]);
                    effect.Parameters["FogColor"].SetValue(fogColor.ToVector3());
                    effect.Parameters["AmbientIntensity"].SetValue(1.0f);
                    effect.Parameters["DiffuseIntensity"].SetValue(0.2f);
                }
                mesh.Draw();
            }
        }
    }
}
