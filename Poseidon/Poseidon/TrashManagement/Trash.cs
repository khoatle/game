using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon
{
    public class Trash : GameObject
    {
        public TrashType trashType;

        public bool sinking;
        public float sinkingRate;
        public float sinkingRotationRate;
        public float seaFloorHeight;

        public float orientation;


        public float fogEndValue = GameConstants.FogEnd;
        public float fogEndMaxVal = 1000.0f;
        public bool increaseFog = true;

        public bool particleAnimationPlayed = false;

        //public float heightChange = 0.5f;
        //public float currentChange = 0.0f;
        //public bool floatUp;
        public Trash( TrashType trashtype)
            : base()
        {
            sinking = false;
            trashType = trashtype;
        }

        public void Load(ContentManager content,ref Model model, float orientation)
        {
            Model = model;
            Position = Vector3.Down;
            BoundingSphere = CalculateBoundingSphere();

            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;
            scaledSphere.Radius *= GameConstants.TrashBoundingSphereFactor;
            BoundingSphere =
                new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);
            this.orientation = orientation;
            //if (orientation > 50) floatUp = true;
            //else floatUp = false;
            // Set up the parameters
            SetupShaderParameters(PoseidonGame.contentManager, Model);
        }
        public void Update(GameTime gameTime)
        {
            EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);
            //for floating trash
            //if (currentChange >= heightChange)
            //{
            //    currentChange = 0.0f;
            //    floatUp = !floatUp;
            //}
            //currentChange += 0.025f;
            //if (floatUp) Position.Y += currentChange;
            //else Position.Y -= currentChange;
            if (sinking)
            {
                Position.Y -= sinkingRate;
                orientation += sinkingRotationRate;
                if (Position.Y <= seaFloorHeight)
                {
                    Position.Y = seaFloorHeight;
                    sinking = false;
                }
            }
            if (increaseFog)
                fogEndValue += 2.5f;
            else fogEndValue -= 2.5f;
            if (fogEndValue > fogEndMaxVal || fogEndValue < GameConstants.FogEnd)
                increaseFog = !increaseFog;
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

                    ////effect.DiffuseColor = Color.Green.ToVector3();

                    //effect.FogEnabled = true;
                    //effect.FogStart = GameConstants.FogStart;
                    //effect.FogEnd = fogEndValue;// GameConstants.FogEnd;
                    //effect.FogColor = GameConstants.FogColor.ToVector3();

                    //for our custom BasicEffect
                    effect.Parameters["AmbientColor"].SetValue(ambientColor.ToVector4());
                    effect.Parameters["DiffuseColor"].SetValue(diffuseColor.ToVector4());
                    effect.Parameters["SpecularColor"].SetValue(specularColor.ToVector4());
                    Matrix readlWorldMatrix = worldMatrix * transforms[mesh.ParentBone.Index];
                    effect.CurrentTechnique = effect.Techniques[techniqueName];
                    effect.Parameters["World"].SetValue(readlWorldMatrix);
                    effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Invert(readlWorldMatrix));
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                    effect.Parameters["EyePosition"].SetValue(new Vector4(gameCamera.AvatarHeadOffset, 0));
                    Matrix WorldView = readlWorldMatrix * view;
                    EffectHelpers.SetFogVector(ref WorldView, GameConstants.FogStart, fogEndValue, effect.Parameters["FogVector"]);
                    effect.Parameters["FogColor"].SetValue(fogColor.ToVector3());
                    effect.Parameters["Shininess"].SetValue(1);
                }
                mesh.Draw();
            }
        }   
    }
}
