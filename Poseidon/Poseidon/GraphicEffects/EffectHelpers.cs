#region File Description
//-----------------------------------------------------------------------------
// EffectHelpers.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
#endregion

namespace Poseidon
{
    /// <summary>
    /// Track which effect parameters need to be recomputed during the next OnApply.
    /// </summary>
    [Flags]
    internal enum EffectDirtyFlags
    {
        WorldViewProj   = 1,
        World           = 2,
        EyePosition     = 4,
        MaterialColor   = 8,
        Fog             = 16,
        FogEnable       = 32,
        AlphaTest       = 64,
        ShaderIndex     = 128,
        All             = -1
    }


    /// <summary>
    /// Helper code shared between the various built-in effects.
    /// </summary>
    internal static class EffectHelpers
    {
        /// <summary>
        /// Sets up the standard key/fill/back lighting rig.
        /// </summary>
        static Vector3 EnableDefaultLighting(DirectionalLight light0, DirectionalLight light1, DirectionalLight light2)
        {
            // Key light.
            light0.Direction = new Vector3(-0.5265408f, -0.5735765f, -0.6275069f);
            light0.DiffuseColor = new Vector3(1, 0.9607844f, 0.8078432f);
            light0.SpecularColor = new Vector3(1, 0.9607844f, 0.8078432f);
            light0.Enabled = true;

            // Fill light.
            light1.Direction = new Vector3(0.7198464f, 0.3420201f, 0.6040227f);
            light1.DiffuseColor = new Vector3(0.9647059f, 0.7607844f, 0.4078432f);
            light1.SpecularColor = Vector3.Zero;
            light1.Enabled = true;

            // Back light.
            light2.Direction = new Vector3(0.4545195f, -0.7660444f, 0.4545195f);
            light2.DiffuseColor = new Vector3(0.3231373f, 0.3607844f, 0.3937255f);
            light2.SpecularColor = new Vector3(0.3231373f, 0.3607844f, 0.3937255f);
            light2.Enabled = true;

            // Ambient light.
            return new Vector3(0.05333332f, 0.09882354f, 0.1819608f);
        }


        /// <summary>
        /// Lazily recomputes the world+view+projection matrix and
        /// fog vector based on the current effect parameter settings.
        /// </summary>
        internal static EffectDirtyFlags SetWorldViewProjAndFog(EffectDirtyFlags dirtyFlags,
                                                                ref Matrix world, ref Matrix view, ref Matrix projection, ref Matrix worldView,
                                                                bool fogEnabled, float fogStart, float fogEnd,
                                                                EffectParameter worldViewProjParam, EffectParameter fogVectorParam)
        {
            // Recompute the world+view+projection matrix?
            if ((dirtyFlags & EffectDirtyFlags.WorldViewProj) != 0)
            {
                Matrix worldViewProj;
                
                Matrix.Multiply(ref world, ref view, out worldView);
                Matrix.Multiply(ref worldView, ref projection, out worldViewProj);
                
                worldViewProjParam.SetValue(worldViewProj);
                
                dirtyFlags &= ~EffectDirtyFlags.WorldViewProj;
            }

            if (fogEnabled)
            {
                // Recompute the fog vector?
                if ((dirtyFlags & (EffectDirtyFlags.Fog | EffectDirtyFlags.FogEnable)) != 0)
                {
                    SetFogVector(ref worldView, fogStart, fogEnd, fogVectorParam);

                    dirtyFlags &= ~(EffectDirtyFlags.Fog | EffectDirtyFlags.FogEnable);
                }
            }
            else
            {
                // When fog is disabled, make sure the fog vector is reset to zero.
                if ((dirtyFlags & EffectDirtyFlags.FogEnable) != 0)
                {
                    fogVectorParam.SetValue(Vector4.Zero);

                    dirtyFlags &= ~EffectDirtyFlags.FogEnable;
                }
            }

            return dirtyFlags;
        }


        /// <summary>
        /// Sets a vector which can be dotted with the object space vertex position to compute fog amount.
        /// </summary>
        public static void SetFogVector(ref Matrix worldView, float fogStart, float fogEnd, EffectParameter fogVectorParam)
        {
            if (fogStart == fogEnd)
            {
                // Degenerate case: force everything to 100% fogged if start and end are the same.
                fogVectorParam.SetValue(new Vector4(0, 0, 0, 1));
            }
            else
            {
                // We want to transform vertex positions into view space, take the resulting
                // Z value, then scale and offset according to the fog start/end distances.
                // Because we only care about the Z component, the shader can do all this
                // with a single dot product, using only the Z row of the world+view matrix.
                
                float scale = 1f / (fogStart - fogEnd);

                Vector4 fogVector = new Vector4();

                fogVector.X = worldView.M13 * scale;
                fogVector.Y = worldView.M23 * scale;
                fogVector.Z = worldView.M33 * scale;
                fogVector.W = (worldView.M43 + fogStart) * scale;

                fogVectorParam.SetValue(fogVector);
            }
        }


        /// <summary>
        /// Lazily recomputes the world inverse transpose matrix and
        /// eye position based on the current effect parameter settings.
        /// </summary>
        internal static EffectDirtyFlags SetLightingMatrices(EffectDirtyFlags dirtyFlags, ref Matrix world, ref Matrix view,
                                                             EffectParameter worldParam, EffectParameter worldInverseTransposeParam, EffectParameter eyePositionParam)
        {
            // Set the world and world inverse transpose matrices.
            if ((dirtyFlags & EffectDirtyFlags.World) != 0)
            {
                Matrix worldTranspose;
                Matrix worldInverseTranspose;
                
                Matrix.Invert(ref world, out worldTranspose);
                Matrix.Transpose(ref worldTranspose, out worldInverseTranspose);
                
                worldParam.SetValue(world);
                worldInverseTransposeParam.SetValue(worldInverseTranspose);
                
                dirtyFlags &= ~EffectDirtyFlags.World;
            }

            // Set the eye position.
            if ((dirtyFlags & EffectDirtyFlags.EyePosition) != 0)
            {
                Matrix viewInverse;
                
                Matrix.Invert(ref view, out viewInverse);

                eyePositionParam.SetValue(viewInverse.Translation);

                dirtyFlags &= ~EffectDirtyFlags.EyePosition;
            }

            return dirtyFlags;
        }


        /// <summary>
        /// Sets the diffuse/emissive/alpha material color parameters.
        /// </summary>
        internal static void SetMaterialColor(bool lightingEnabled, float alpha,
                                              ref Vector3 diffuseColor, ref Vector3 emissiveColor, ref Vector3 ambientLightColor,
                                              EffectParameter diffuseColorParam, EffectParameter emissiveColorParam)
        {
            // Desired lighting model:
            //
            //     ((AmbientLightColor + sum(diffuse directional light)) * DiffuseColor) + EmissiveColor
            //
            // When lighting is disabled, ambient and directional lights are ignored, leaving:
            //
            //     DiffuseColor + EmissiveColor
            //
            // For the lighting disabled case, we can save one shader instruction by precomputing
            // diffuse+emissive on the CPU, after which the shader can use DiffuseColor directly,
            // ignoring its emissive parameter.
            //
            // When lighting is enabled, we can merge the ambient and emissive settings. If we
            // set our emissive parameter to emissive+(ambient*diffuse), the shader no longer
            // needs to bother adding the ambient contribution, simplifying its computation to:
            //
            //     (sum(diffuse directional light) * DiffuseColor) + EmissiveColor
            //
            // For futher optimization goodness, we merge material alpha with the diffuse
            // color parameter, and premultiply all color values by this alpha.
            
            if (lightingEnabled)
            {
                Vector4 diffuse = new Vector4();
                Vector3 emissive = new Vector3();
                
                diffuse.X = diffuseColor.X * alpha;
                diffuse.Y = diffuseColor.Y * alpha;
                diffuse.Z = diffuseColor.Z * alpha;
                diffuse.W = alpha;

                emissive.X = (emissiveColor.X + ambientLightColor.X * diffuseColor.X) * alpha;
                emissive.Y = (emissiveColor.Y + ambientLightColor.Y * diffuseColor.Y) * alpha;
                emissive.Z = (emissiveColor.Z + ambientLightColor.Z * diffuseColor.Z) * alpha;

                diffuseColorParam.SetValue(diffuse);
                emissiveColorParam.SetValue(emissive);
            }
            else
            {
                Vector4 diffuse = new Vector4();
                
                diffuse.X = (diffuseColor.X + emissiveColor.X) * alpha;
                diffuse.Y = (diffuseColor.Y + emissiveColor.Y) * alpha;
                diffuse.Z = (diffuseColor.Z + emissiveColor.Z) * alpha;
                diffuse.W = alpha;

                diffuseColorParam.SetValue(diffuse);
            }
        }
        /// <summary>
        /// Alters a model so it will draw using a custom effect, while preserving
        /// whatever textures were set on it as part of the original effects.
        /// </summary>
        public static void ChangeEffectUsedByModelToCustomSkinnedEffect(Model model, Effect replacementEffect)
        {
            // Table mapping the original effects to our replacement versions.
            Dictionary<Effect, Effect> effectMapping = new Dictionary<Effect, Effect>();

            foreach (ModelMesh mesh in model.Meshes)
            {
                // Scan over all the effects currently on the mesh.
                //foreach (Effect oldEffect in mesh.Effects)
                //{
                //    //if (oldEffect == replacementEffect) continue;
                //    //if (!(oldEffect is SkinnedEffect || oldEffect is BasicEffect))
                //    //    continue;
                //    // If we haven't already seen this effect...
                //    //if (!effectMapping.ContainsKey(oldEffect))
                //    //{
                //    // Make a clone of our replacement effect. We can't just use
                //    // it directly, because the same effect might need to be
                //    // applied several times to different parts of the model using
                //    // a different texture each time, so we need a fresh copy each
                //    // time we want to set a different texture into it.
                //    Effect newEffect = replacementEffect.Clone();

                //    //newEffect.Parameters["Texture"].SetValue();
                //    //oldEffect.Parameters
                //    // Copy across the texture from the original effect.
                //    //if (oldEffect is BasicEffect)
                //    //    newEffect.Parameters["Texture"].SetValue(((BasicEffect)oldEffect).Texture);
                //    //else if (oldEffect is SkinnedEffect)
                //    //    newEffect.Parameters["Texture"].SetValue(((SkinnedEffect)oldEffect).Texture);
                //    //else if (oldEffect is AlphaTestEffect)
                //    //    newEffect.Parameters["Texture"].SetValue(((AlphaTestEffect)oldEffect).Texture);
                //    //else if (oldEffect is DualTextureEffect)
                //    //    newEffect.Parameters["Texture"].SetValue(((DualTextureEffect)oldEffect).Texture);
                //    //else if (oldEffect is EnvironmentMapEffect)
                //    //    newEffect.Parameters["Texture"].SetValue(((EnvironmentMapEffect)oldEffect).Texture);

                //    effectMapping.Add(oldEffect, newEffect);
                //    //}
                //}

                // Now that we've found all the effects in use on this mesh,
                // update it to use our new replacement versions.
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    //if (meshPart.Effect == replacementEffect) continue;
                    if (!(meshPart.Effect is SkinnedEffect || meshPart.Effect is BasicEffect || meshPart.Effect is AlphaTestEffect || meshPart.Effect is DualTextureEffect || meshPart.Effect is EnvironmentMapEffect)) continue;
                    Effect newEffect = replacementEffect.Clone();
                    newEffect.Parameters["Texture"].SetValue(((SkinnedEffect)meshPart.Effect).Texture);
                    meshPart.Effect = newEffect;// effectMapping[meshPart.Effect];
                }
            }
        }
        public static void ChangeEffectUsedByModelToCustomBasicEffect(Model model, Effect replacementEffect)
        {
            // Table mapping the original effects to our replacement versions.
            Dictionary<Effect, Effect> effectMapping = new Dictionary<Effect, Effect>();

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    if (!(meshPart.Effect is SkinnedEffect || meshPart.Effect is BasicEffect || meshPart.Effect is AlphaTestEffect || meshPart.Effect is DualTextureEffect || meshPart.Effect is EnvironmentMapEffect)) continue;
                    Effect newEffect = replacementEffect.Clone();
                    newEffect.Parameters["Texture"].SetValue(((BasicEffect)meshPart.Effect).Texture);
                    meshPart.Effect = newEffect;// effectMapping[meshPart.Effect];
                }
            }
        }

        public static float lastEnv = 1000.0f;
        public static void GetEffectConfiguration(ref Color fogColor, ref Color ambientColor, ref Color diffuseColor, ref Color specularColor)
        {
            if (Math.Abs(HydroBot.currentEnvPoint - lastEnv) >= 150f || Math.Abs(HydroBot.currentEnvPoint - lastEnv) <= 0.3f) lastEnv = HydroBot.currentEnvPoint;
            else if (HydroBot.currentEnvPoint > lastEnv) lastEnv += 0.001f;
            else if (HydroBot.currentEnvPoint < lastEnv) lastEnv -= 0.001f;
            float envFactor = (lastEnv / (float)HydroBot.maxEnvPoint);

            //we don't want the screen to be too dark
            if (envFactor < 0.35f) envFactor = 0.35f;
            //or too bright
            if (envFactor > 0.90f) envFactor = 0.90f;
            //envFactor = 1.0f;
            if (HydroBot.gameMode == GameMode.MainGame)
            {
                fogColor = LerpColor(GameConstants.FogColor[PlayGameScene.currentLevel], Color.Blue, envFactor) * envFactor;
                ambientColor = LerpColor(GameConstants.AmbientColor[PlayGameScene.currentLevel], new Color(0, 191.0f / 255.0f, 1), envFactor) * envFactor;
                diffuseColor = LerpColor(GameConstants.DiffuseColor[PlayGameScene.currentLevel], new Color(0, 255, 255), envFactor) * envFactor;
                specularColor = LerpColor(GameConstants.SpecularColor[PlayGameScene.currentLevel], new Color(135.0f / 255.0f, 206.0f / 255.0f, 250.0f / 255.0f), envFactor) * envFactor;
                //fogColor = GameConstants.FogColor[PlayGameScene.currentLevel] * envFactor;
                //ambientColor = GameConstants.AmbientColor[PlayGameScene.currentLevel] * envFactor;
                //diffuseColor = GameConstants.DiffuseColor[PlayGameScene.currentLevel] * envFactor;
                //specularColor = GameConstants.SpecularColor[PlayGameScene.currentLevel] * envFactor;
            }
            else if (HydroBot.gameMode == GameMode.ShipWreck)
            {
                fogColor = GameConstants.FogColor[PlayGameScene.currentLevel] * envFactor;
                ambientColor = GameConstants.AmbientColor[PlayGameScene.currentLevel] * envFactor;
                diffuseColor = GameConstants.DiffuseColor[PlayGameScene.currentLevel] * envFactor;
                specularColor = GameConstants.SpecularColor[PlayGameScene.currentLevel] * envFactor;
            }
            else if (HydroBot.gameMode == GameMode.SurvivalMode)
            {
                fogColor = GameConstants.FogColor[0] * envFactor;
                ambientColor = GameConstants.AmbientColor[0] * envFactor;
                diffuseColor = GameConstants.DiffuseColor[0] * envFactor;
                specularColor = GameConstants.SpecularColor[0] * envFactor;
            }
        }
        public static Color LerpColor(Color color1, Color color2, float amount)
        {
            //if (amount < 0.5) return color1;
            Color resultColor = new Color();
            resultColor.R = (byte)MathHelper.Lerp(color1.R, color2.R, amount);
            resultColor.G = (byte)MathHelper.Lerp(color1.G, color2.G, amount);
            resultColor.B = (byte)MathHelper.Lerp(color1.B, color2.B, amount);
            //important for specular color
            resultColor.A = (byte)MathHelper.Lerp(color1.A, color2.A, amount);
            return resultColor;
        }
    }
}
