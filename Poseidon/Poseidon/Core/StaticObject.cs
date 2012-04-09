using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SkinnedModel;
using Poseidon.Core;

namespace Poseidon
{
    public class StaticObject : GameObject
    {
        public float orientation = 0;
        public static Random random = new Random();
        public float scale;
        bool isAnimated = false;
        Matrix[] bones;
        SkinningData skd;
        protected ClipPlayer clipPlayer;
        protected Matrix fishMatrix;
        protected Quaternion qRotation = Quaternion.Identity;
        //public BoundingBox boundingBox;

        public void LoadContent(ContentManager content, string modelname, bool isAnimated, int clipStart, int clipEnd, int fpsRate)
        {
            if (HydroBot.gameMode == GameMode.MainGame)
            {
                orientation = (float)random.Next(0, 629) / 100;
                if (!isAnimated)
                    scale = (float)random.Next(5, 11) / 10;
                else scale = random.Next(4, 7);
            }
            else if (HydroBot.gameMode == GameMode.ShipWreck)
            {
                scale = 1.0f;
            }
        
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

            this.isAnimated = isAnimated;

            if (isAnimated)
            {
                skd = Model.Tag as SkinningData;
                clipPlayer = new ClipPlayer(skd, fpsRate);//ClipPlayer running at 24 frames/sec
                AnimationClip clip = skd.AnimationClips["Take 001"]; //Take name from the dude.fbx file
                clipPlayer.play(clip, clipStart, clipEnd, true);
                fishMatrix = Matrix.CreateScale(1.0f) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                                   Matrix.CreateTranslation(Position);
            }

            // Set up the parameters
            //SetupShaderParameters(content, Model);
             
        }



        public void Update(BoundingFrustum cameraFrustum, GameTime gameTime)
        {
            if (isAnimated)
            {
                // if clip player has been initialized, update it
                if (clipPlayer != null && BoundingSphere.Intersects(cameraFrustum))
                {
                    qRotation = Quaternion.CreateFromAxisAngle(
                                    Vector3.Up,
                                    orientation);
                    float scale = this.scale;

                    fishMatrix = Matrix.CreateScale(scale) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                                        Matrix.CreateFromQuaternion(qRotation) *
                                        Matrix.CreateTranslation(Position);
                    //fishMatrix = Matrix.CreateScale(scale) * Matrix.CreateTranslation(Position);
                    clipPlayer.update(gameTime.ElapsedGameTime, true, fishMatrix);
                }
            }
        }

        public void Draw(Matrix view, Matrix projection)
        {
            EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);

            if (isAnimated)
            {
                  bones = clipPlayer.GetSkinTransforms();

                  foreach (ModelMesh mesh in Model.Meshes)
                  {
                      foreach (SkinnedEffect effect in mesh.Effects)
                      //foreach (Effect effect in mesh.Effects)
                      {
                          //for standard Skinned Effect
                          effect.DirectionalLight1.Enabled = false;
                          effect.DirectionalLight2.Enabled = false;
                          effect.DirectionalLight0.Enabled = true;
                          effect.DirectionalLight0.Direction = new Vector3(0, 1, 0);
                          effect.DirectionalLight0.DiffuseColor = diffuseColor.ToVector3();
                          //effect.DirectionalLight0.SpecularColor = new Vector3(135.0f / 255.0f, 206.0f / 255.0f, 250.0f / 255.0f);
                          effect.PreferPerPixelLighting = true;
                          effect.SetBoneTransforms(bones);
                          effect.View = view;
                          effect.Projection = projection;

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
                  return;
            }
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
