﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Audio;

namespace Poseidon
{
    public class Powerpack : GameObject
    {
        public bool Retrieved { get; set; }
        //SoundEffect RetrievedSound;
        //Temporary power-up types
        //1: speed
        //2: power
        //3: fire rate
        //4: health point
        //5: Strange Rock
        public int powerType;
        private float orientation; //rotation in radians

        public Powerpack(int powerType)
            : base()
        {
            Retrieved = false;
            orientation = 0f;
            this.powerType = powerType;
        }

        public void LoadContent(Vector3 powerpackPosition)
        {
            Position = powerpackPosition;
            BoundingSphere = CalculateBoundingSphere();
            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;
            scaledSphere.Center = Position;
            scaledSphere.Radius *=
                GameConstants.FruitBoundingSphereFactor;
            BoundingSphere =
                new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);

            SetupShaderParameters(PoseidonGame.contentManager, Model);
            
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
            Update(); //since update is only changing orientation, it is better to put here than in playgamescene & survival scene
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
                    ////effect.EmissiveColor = Color.White.ToVector3();

                    //effect.AmbientLightColor = ambientColor.ToVector3();
                    //effect.DiffuseColor = diffuseColor.ToVector3();
                    //effect.SpecularColor = specularColor.ToVector3();

                    //effect.FogEnabled = true;
                    //effect.FogStart = GameConstants.FogStart;
                    //effect.FogEnd = GameConstants.FogEnd;
                    //effect.FogColor = fogColor.ToVector3();

                    //for our custom BasicEffect
                    effect.Parameters["FogEnabled"].SetValue(false);
                    if (powerType == 5)
                    {
                        effect.Parameters["AmbientColor"].SetValue(Color.White.ToVector4() * 0.6f); //(ambientColor.ToVector4());
                        effect.Parameters["DiffuseColor"].SetValue(Color.White.ToVector4() * 0.6f); //(diffuseColor.ToVector4());
                        effect.Parameters["SpecularColor"].SetValue(Color.White.ToVector4() * 0.6f);//specularColor.ToVector4());
                        //effect.Parameters["FogColor"].SetValue(Color.White.ToVector3());//(fogColor.ToVector3());
                    }
                    else
                    {
                        effect.Parameters["AmbientColor"].SetValue(Color.White.ToVector4() * 0.6f); //(ambientColor.ToVector4());
                        effect.Parameters["DiffuseColor"].SetValue(Color.White.ToVector4() * 0.6f); //(diffuseColor.ToVector4());
                        effect.Parameters["SpecularColor"].SetValue(Color.White.ToVector4() * 0.6f);//specularColor.ToVector4());
                        //effect.Parameters["AmbientColor"].SetValue(ambientColor.ToVector4());
                        //effect.Parameters["DiffuseColor"].SetValue(diffuseColor.ToVector4());
                        //effect.Parameters["SpecularColor"].SetValue(specularColor.ToVector4());
                        //effect.Parameters["FogColor"].SetValue(fogColor.ToVector3());
                    }
                    effect.CurrentTechnique = effect.Techniques[techniqueName];
                    Matrix readlWorldMatrix = worldMatrix * transforms[mesh.ParentBone.Index];
                    effect.Parameters["World"].SetValue(readlWorldMatrix);
                    effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Invert(readlWorldMatrix));
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                    effect.Parameters["EyePosition"].SetValue(new Vector4(gameCamera.AvatarHeadOffset, 0));
                    Matrix WorldView = readlWorldMatrix * view;
                    EffectHelpers.SetFogVector(ref WorldView, GameConstants.FogStart, GameConstants.FogEnd, effect.Parameters["FogVector"]);         
                    effect.Parameters["AmbientIntensity"].SetValue(1.0f);
                    effect.Parameters["DiffuseIntensity"].SetValue(0.2f);
                    effect.Parameters["Shininess"].SetValue(1.0f);
                }
                mesh.Draw();
            }
        }

        public void Update()
        {
            EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);
            orientation += GameConstants.powerpackResourceRotationSpeed;
        }

    }

}
