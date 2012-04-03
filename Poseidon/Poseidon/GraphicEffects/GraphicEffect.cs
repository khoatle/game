using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Poseidon.Core;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Poseidon.GraphicEffects
{
    public class GraphicEffect
    {
        BloomComponent bloom;
        int bloomSettingsIndex = 0;

        KeyboardState lastKeyboardState = new KeyboardState();
        KeyboardState currentKeyboardState = new KeyboardState();

        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        Effect underWaterEffect, screenTransitionEffect, edgeDetectionEffect, customBlurEffect, distortionEffect, rippleEffect;
        bool underWaterEffectEnabled = true;
        bool bloomEffectEnabled = true;
        RenderTarget2D afterUnderWaterTexture, afterBloomTexture, afterEffectsRenderTarget, blurRenderTarget1, blurRenderTarget2, distortionRenderTarget, rippleRenderTarget;
        EffectParameterCollection edgeDetectionParameters;
        EffectParameter waveParam, distortionParam, centerCoordParam;
        float distortion = 1.0f;
        float divisor = 0.001f;
        float wave = MathHelper.Pi;

        PresentationParameters pp;

        float m_Timer = 0;
        Random random;
        float fadeBetweenScenes = 1.0f;
        public float fadeTransitAmount = 0.025f;

        public GraphicEffect(GameScene gameScene, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            random = new Random();
            bloom = new BloomComponent(gameScene.Game);
            // Look up the resolution and format of our main backbuffer.
            pp = gameScene.Game.GraphicsDevice.PresentationParameters;

            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;
            //width /= 2;
            //height /= 2;

            blurRenderTarget1 = new RenderTarget2D(gameScene.Game.GraphicsDevice, width, height, false, pp.BackBufferFormat, DepthFormat.None);
            blurRenderTarget2 = new RenderTarget2D(gameScene.Game.GraphicsDevice, width, height, false, pp.BackBufferFormat, DepthFormat.None);
            distortionRenderTarget = new RenderTarget2D(gameScene.Game.GraphicsDevice, width, height, false, pp.BackBufferFormat, DepthFormat.None);
            rippleRenderTarget = new RenderTarget2D(gameScene.Game.GraphicsDevice, width, height, false, pp.BackBufferFormat, DepthFormat.None);
            afterUnderWaterTexture = new RenderTarget2D(gameScene.Game.GraphicsDevice, width, height, false, pp.BackBufferFormat, DepthFormat.None);
            afterBloomTexture = new RenderTarget2D(gameScene.Game.GraphicsDevice, width, height, false, pp.BackBufferFormat, DepthFormat.None);
            afterEffectsRenderTarget = new RenderTarget2D(gameScene.Game.GraphicsDevice, width, height, false, pp.BackBufferFormat, DepthFormat.None);

            underWaterEffect = gameScene.Game.Content.Load<Effect>("Shaders/UnderWater");
            screenTransitionEffect = gameScene.Game.Content.Load<Effect>("Shaders/ScreenTransition");
            edgeDetectionEffect = gameScene.Game.Content.Load<Effect>("Shaders/EdgeDetectionEffect");
            customBlurEffect = gameScene.Game.Content.Load<Effect>("Shaders/CustomBlur");
            distortionEffect = gameScene.Game.Content.Load<Effect>("Shaders/DistortionEffect");
            rippleEffect = gameScene.Game.Content.Load<Effect>("Shaders/RippleEffect");

            edgeDetectionParameters = edgeDetectionEffect.Parameters;
            edgeDetectionEffect.CurrentTechnique = edgeDetectionEffect.Techniques["EdgeDetect"];

            waveParam = rippleEffect.Parameters["wave"];
            distortionParam = rippleEffect.Parameters["distortion"];
            centerCoordParam = rippleEffect.Parameters["centerCoord"];

            this.spriteBatch = spriteBatch;
            this.spriteFont = spriteFont;
        }

        public bool enableConfig = false;
        public void UpdateInput(GameTime gameTime)
        {
            lastKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            
            if (currentKeyboardState.IsKeyDown(Keys.OemTilde) &&
                 lastKeyboardState.IsKeyUp(Keys.OemTilde))
            {
                enableConfig = !enableConfig;
            }
            if (enableConfig)
            {
                if (currentKeyboardState.IsKeyDown(Keys.Q) &&
                     lastKeyboardState.IsKeyUp(Keys.Q))
                {
                    bloom.Settings.BaseIntensity += 0.1f;
                }
                if (currentKeyboardState.IsKeyDown(Keys.W) &&
                     lastKeyboardState.IsKeyUp(Keys.W))
                {
                    bloom.Settings.BaseSaturation += 0.1f;
                }
                if (currentKeyboardState.IsKeyDown(Keys.E) &&
                     lastKeyboardState.IsKeyUp(Keys.E))
                {
                    bloom.Settings.BloomIntensity += 0.1f;
                }
                if (currentKeyboardState.IsKeyDown(Keys.R) &&
                     lastKeyboardState.IsKeyUp(Keys.R))
                {
                    bloom.Settings.BloomSaturation += 0.1f;
                }
                if (currentKeyboardState.IsKeyDown(Keys.T) &&
                     lastKeyboardState.IsKeyUp(Keys.T))
                {
                    bloom.Settings.BloomThreshold += 0.1f;
                }
                if (currentKeyboardState.IsKeyDown(Keys.Y) &&
                     lastKeyboardState.IsKeyUp(Keys.Y))
                {
                    bloom.Settings.BlurAmount += 0.1f;
                }

                if (currentKeyboardState.IsKeyDown(Keys.D1) &&
                     lastKeyboardState.IsKeyUp(Keys.D1))
                {
                    bloom.Settings.BaseIntensity -= 0.1f;
                }
                if (currentKeyboardState.IsKeyDown(Keys.D2) &&
                     lastKeyboardState.IsKeyUp(Keys.D2))
                {
                    bloom.Settings.BaseSaturation -= 0.1f;
                }
                if (currentKeyboardState.IsKeyDown(Keys.D3) &&
                     lastKeyboardState.IsKeyUp(Keys.D3))
                {
                    bloom.Settings.BloomIntensity -= 0.1f;
                }
                if (currentKeyboardState.IsKeyDown(Keys.D4) &&
                     lastKeyboardState.IsKeyUp(Keys.D4))
                {
                    bloom.Settings.BloomSaturation -= 0.1f;
                }
                if (currentKeyboardState.IsKeyDown(Keys.D5) &&
                     lastKeyboardState.IsKeyUp(Keys.D5))
                {
                    bloom.Settings.BloomThreshold -= 0.1f;
                }
                if (currentKeyboardState.IsKeyDown(Keys.D6) &&
                     lastKeyboardState.IsKeyUp(Keys.D6))
                {
                    bloom.Settings.BlurAmount -= 0.1f;
                }
            }
            // Switch underwater effect on/off
            if (currentKeyboardState.IsKeyDown(Keys.U) &&
                 lastKeyboardState.IsKeyUp(Keys.U))
            {
                underWaterEffectEnabled = !underWaterEffectEnabled;
            }


            //if (currentKeyboardState.IsKeyDown(Keys.R) &&
            //    lastKeyboardState.IsKeyUp(Keys.R))
            //{
            //    divisor = 0.1f;
            //}

            // Switch to the next bloom settings preset?
            if (currentKeyboardState.IsKeyDown(Keys.A) &&
                 lastKeyboardState.IsKeyUp(Keys.A))
            {
                bloomSettingsIndex = (bloomSettingsIndex + 1) %
                                     BloomSettings.PresetSettings.Length;

                bloom.Settings = BloomSettings.PresetSettings[bloomSettingsIndex];
                bloomEffectEnabled = true;
            }

            // Toggle bloom on or off?
            if (currentKeyboardState.IsKeyDown(Keys.B) &&
                 lastKeyboardState.IsKeyUp(Keys.B))
            {
                bloomEffectEnabled = !bloomEffectEnabled;
            }

            // Cycle through the intermediate buffer debug display modes?
            if (currentKeyboardState.IsKeyDown(Keys.D) &&
                 lastKeyboardState.IsKeyUp(Keys.D))
            {
                bloomEffectEnabled = true;
                bloom.ShowBuffer++;

                if (bloom.ShowBuffer > BloomComponent.IntermediateBuffer.FinalResult)
                    bloom.ShowBuffer = 0;
            }

            //timer for certain the shaders
            m_Timer += (float)gameTime.ElapsedGameTime.Milliseconds / 1000;
        }


        public RenderTarget2D DrawWithEffects(GameTime gameTime, RenderTarget2D originalScene, GraphicsDeviceManager graphics)
        {
            if (!GameSettings. SpecialEffectsEnabled) return originalScene;
            //applying blurring effect
            SetBlurEffectParameters(1.0f / (float)blurRenderTarget1.Width, 0);

            DrawFullscreenQuad(originalScene, blurRenderTarget1, customBlurEffect, graphics.GraphicsDevice);

            SetBlurEffectParameters(0, 1.0f / (float)blurRenderTarget1.Height);

            DrawFullscreenQuad(blurRenderTarget1, blurRenderTarget2,
                               customBlurEffect, graphics.GraphicsDevice);

            //applying distortion effect
            if (HydroBot.distortingScreen)
            {
                float distortionFactor = (float)(GameConstants.distortionDuration - (PoseidonGame.playTime.TotalMilliseconds - HydroBot.distortionStart * 1000)) / GameConstants.distortionDuration;
                graphics.GraphicsDevice.SetRenderTarget(distortionRenderTarget);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                {
                    // Apply the post process shader
                    distortionEffect.CurrentTechnique.Passes[0].Apply();
                    {
                        distortionEffect.Parameters["fTimer"].SetValue(m_Timer * 2);
                        distortionEffect.Parameters["iSeed"].SetValue(1337);
                        distortionEffect.Parameters["fNoiseAmount"].SetValue(0.01f * distortionFactor);
                        spriteBatch.Draw(blurRenderTarget2, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
                    }
                }
                spriteBatch.End();
            }

            //applying water ripple effect
            if (HydroBot.ripplingScreen)
            {
                graphics.GraphicsDevice.SetRenderTarget(rippleRenderTarget);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                {
                    rippleEffect.CurrentTechnique.Passes[0].Apply();
                    {

                        //wave = MathHelper.Pi / divisor;
                        wave = MathHelper.Pi / divisor;
                        waveParam.SetValue(wave);
                        distortionParam.SetValue(distortion);
                        centerCoordParam.SetValue(HydroBot.rippleCenter);
                        if (HydroBot.distortingScreen)
                            spriteBatch.Draw(distortionRenderTarget, Vector2.Zero, Color.White);
                        else
                            spriteBatch.Draw(blurRenderTarget2, Vector2.Zero, Color.White);
                    }
                }
                spriteBatch.End();
                // Change the period.
                divisor += 0.01f;
                if (divisor >= 1.0f)
                {
                    divisor = 0.001f;
                    HydroBot.ripplingScreen = false;
                }
            }

            //apply underwater effect
            graphics.GraphicsDevice.SetRenderTarget(afterUnderWaterTexture);
            if (underWaterEffectEnabled)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                {
                    underWaterEffect.Parameters["fTimer"].SetValue(m_Timer * 1.5f);
                    // Apply the underwater effect post process shader
                    underWaterEffect.CurrentTechnique.Passes[0].Apply();
                    {
                        if (!HydroBot.distortingScreen && !HydroBot.ripplingScreen)
                        {
                            spriteBatch.Draw(blurRenderTarget2, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight),
                                //fix the problem of ugly areas at the edges of the screen
                                 new Rectangle(32, 32, originalScene.Width - 64, originalScene.Height - 64), Color.White);
                        }
                        else if (HydroBot.distortingScreen && !HydroBot.ripplingScreen)
                            spriteBatch.Draw(distortionRenderTarget, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight),
                                //fix the problem of ugly areas at the edges of the screen
                                 new Rectangle(32, 32, originalScene.Width - 64, originalScene.Height - 64), Color.White);
                        else if (HydroBot.ripplingScreen)
                            spriteBatch.Draw(rippleRenderTarget, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight),
                                //fix the problem of ugly areas at the edges of the screen
                                 new Rectangle(32, 32, originalScene.Width - 64, originalScene.Height - 64), Color.White);
                    }
                }
                spriteBatch.End();
            }
            else
            {
                spriteBatch.Begin();
                spriteBatch.Draw(originalScene, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
                spriteBatch.End();
            }

            //apply bloom effect
            if (bloomEffectEnabled)
            {
                //afterBloomTexture = new RenderTarget2D(graphics.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, DepthFormat.None);
                bloom.Draw(gameTime, afterUnderWaterTexture, afterBloomTexture);
            }
            else
            {
                graphics.GraphicsDevice.SetRenderTarget(afterBloomTexture);
                spriteBatch.Begin();
                spriteBatch.Draw(afterUnderWaterTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
                spriteBatch.End();
            }
            graphics.GraphicsDevice.SetRenderTarget(afterEffectsRenderTarget);
            graphics.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.Draw(afterBloomTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);// * ((float)HydroBot.currentEnvPoint / (float)HydroBot.maxEnvPoint));
            spriteBatch.End();
            //graphics.GraphicsDevice.SamplerStates[1] = SamplerState.AnisotropicClamp;
            //graphics.GraphicsDevice.Textures[1] = afterBloomTexture;          
            //DrawOverlayText();
            return afterEffectsRenderTarget;
        }

        void DrawOverlayText()
        {
            //string text = "A = settings (" + bloom.Settings.Name + ")\n" +
            //              "B = toggle bloom (" + (bloomEffectEnabled ? "on" : "off") + ")\n" +
            //              "D = show buffer (" + bloom.ShowBuffer.ToString() + ")\n" +
            //              "U = toggle underwater (" + (underWaterEffectEnabled ? "on" : "off") + ")";
            string text = "Enable config (press `): " + enableConfig.ToString() + "\n" +
                          "BaseIntensity: " + bloom.Settings.BaseIntensity + "\n" +
                          "BaseSaturation: " + bloom.Settings.BaseSaturation + "\n" +
                          "BloomIntensity: " + bloom.Settings.BloomIntensity + "\n" +
                          "BloomSaturation: " + bloom.Settings.BloomSaturation + "\n" +
                          "BloomThreshold: " + bloom.Settings.BloomThreshold + "\n" +
                          "BlurAmount: " + bloom.Settings.BlurAmount + "\n";
            //spriteBatch.Begin(0, BlendState.Opaque, null, null, null, null);
            spriteBatch.Begin();
            // Draw the string twice to create a drop shadow, first colored black
            // and offset one pixel to the bottom right, then again in white at the
            // intended position. This makes text easier to read over the background.
            spriteBatch.DrawString(spriteFont, text, new Vector2(65, 65), Color.Black);
            spriteBatch.DrawString(spriteFont, text, new Vector2(64, 64), Color.White);

            spriteBatch.End();
        }

        public void resetTransitTimer()
        {
            fadeBetweenScenes = 1.0f;
            screenTransitionEffect.CurrentTechnique = screenTransitionEffect.Techniques[random.Next(2)];
        }

        public bool TransitTwoSceens(Texture2D sceenFadeOut, Texture2D sceenFadeIn, GraphicsDeviceManager graphics, RenderTarget2D outputRenderTarget)
        {
            graphics.GraphicsDevice.SetRenderTarget(outputRenderTarget);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            {
                screenTransitionEffect.Parameters["fFadeAmount"].SetValue(fadeBetweenScenes);
                screenTransitionEffect.Parameters["fSmoothSize"].SetValue(0.05f);
                screenTransitionEffect.Parameters["ColorMap2"].SetValue(sceenFadeOut);
                screenTransitionEffect.CurrentTechnique.Passes[0].Apply();
                {
                    //float fadeBetweenScenes = ((float)Math.Sin(m_Timer) * 0.5f) + 0.5f;  
                    spriteBatch.Draw(sceenFadeIn, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
                }
            }
            spriteBatch.End();
            fadeBetweenScenes -= fadeTransitAmount;
            if (fadeBetweenScenes <= 0.0f)
            {
                fadeBetweenScenes = 1.0f;
                return true;
            }
            else return false;
        }

        public void PrepareEdgeDetect(HydroBot hydroBot, Cursor cursor, Camera gameCamera, Fish[] fish, int fishAmount, BaseEnemy[] enemies, int enemiesAmount, List<Trash> trashes, List<ShipWreck> shipWrecks, List<Factory> factories, ResearchFacility researchFacility, List<TreasureChest> treasureChests, List<Powerpack> powerPacks, List<Resource> resources, GraphicsDevice graphicsDevice, RenderTarget2D normalDepthRenderTargetLow, RenderTarget2D normalDepthRenderTargetHigh)
        {
            if (!GameSettings.SpecialEffectsEnabled) return;

            graphicsDevice.SetRenderTarget(normalDepthRenderTargetLow);
            graphicsDevice.Clear(Color.Black);
            graphicsDevice.SetRenderTarget(normalDepthRenderTargetHigh);
            graphicsDevice.Clear(Color.Black);
            graphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            Fish fishPointedAt = CursorManager.MouseOnWhichFish(cursor, gameCamera, fish, fishAmount);
            if (fishPointedAt != null)
            {
                graphicsDevice.SetRenderTarget(normalDepthRenderTargetHigh);
                fishPointedAt.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalDepth");
                edgeDetectionParameters["EdgeColor"].SetValue(new Vector4(0, 1, 0, 1));

            }
            else
            {
                BaseEnemy enemyPointedAt = CursorManager.MouseOnWhichEnemy(cursor, gameCamera, enemies, enemiesAmount);
                if (enemyPointedAt != null)
                {
                    graphicsDevice.SetRenderTarget(normalDepthRenderTargetHigh);
                    enemyPointedAt.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalDepth");
                    edgeDetectionParameters["EdgeColor"].SetValue(new Vector4(1, 0, 0, 1));
                }
                else
                {
                    Powerpack powerPackPointedAt = null, botOnPowerPack = null;
                    CursorManager.MouseOnWhichPowerPack(cursor, gameCamera, powerPacks, ref powerPackPointedAt, ref botOnPowerPack, hydroBot);
                    if (botOnPowerPack != null)
                    {
                        graphicsDevice.SetRenderTarget(normalDepthRenderTargetHigh);
                        botOnPowerPack.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalDepth");
                        edgeDetectionParameters["EdgeColor"].SetValue(Color.LightGray.ToVector4());
                    }
                    if (powerPackPointedAt != null)
                    {
                        graphicsDevice.SetRenderTarget(normalDepthRenderTargetHigh);
                        powerPackPointedAt.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalDepth");
                        edgeDetectionParameters["EdgeColor"].SetValue(Color.Gold.ToVector4());
                    }
                    else
                    {
                        Resource resourcePackPointedAt = null, botOnResource = null;
                        CursorManager.MouseOnWhichResource(cursor, gameCamera, resources, ref resourcePackPointedAt, ref botOnResource, hydroBot);
                        if (botOnResource != null)
                        {
                            graphicsDevice.SetRenderTarget(normalDepthRenderTargetHigh);
                            botOnResource.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalDepth");
                            edgeDetectionParameters["EdgeColor"].SetValue(Color.LightGray.ToVector4());
                        }
                        if (resourcePackPointedAt != null)
                        {
                            graphicsDevice.SetRenderTarget(normalDepthRenderTargetHigh);
                            resourcePackPointedAt.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalDepth");
                            edgeDetectionParameters["EdgeColor"].SetValue(Color.Gold.ToVector4());
                        }
                        else
                        {
                            TreasureChest chestPointedAt = CursorManager.MouseOnWhichChest(cursor, gameCamera, treasureChests);
                            if (chestPointedAt != null)
                            {
                                graphicsDevice.SetRenderTarget(normalDepthRenderTargetLow);
                                chestPointedAt.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalDepth");
                                edgeDetectionParameters["EdgeColor"].SetValue(Color.Gold.ToVector4());
                            }
                            Trash trashPointedAt = null, botOnTrash = null;
                            CursorManager.MouseOnWhichTrash(cursor, gameCamera, trashes, ref trashPointedAt, ref botOnTrash, hydroBot);
                            if (botOnTrash != null)
                            {
                                if (botOnTrash.sinking)
                                    graphicsDevice.SetRenderTarget(normalDepthRenderTargetHigh);
                                else graphicsDevice.SetRenderTarget(normalDepthRenderTargetLow);
                                botOnTrash.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalDepth");
                                edgeDetectionParameters["EdgeColor"].SetValue(Color.LightGray.ToVector4());
                            }
                            if (trashPointedAt != null)
                            {
                                if (trashPointedAt.sinking)
                                    graphicsDevice.SetRenderTarget(normalDepthRenderTargetHigh);
                                else graphicsDevice.SetRenderTarget(normalDepthRenderTargetLow);
                                trashPointedAt.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalDepth");
                                edgeDetectionParameters["EdgeColor"].SetValue(Color.Gold.ToVector4());
                            }
                            else
                            {
                                ShipWreck shipPointedAt = CursorManager.MouseOnWhichShipWreck(cursor, gameCamera, shipWrecks);
                                if (shipPointedAt != null)
                                {
                                    graphicsDevice.SetRenderTarget(normalDepthRenderTargetLow);
                                    shipPointedAt.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalDepth");
                                    edgeDetectionParameters["EdgeColor"].SetValue(Color.Gold.ToVector4());
                                }
                                else
                                {
                                    Factory factoryPointedAt = CursorManager.MouseOnWhichFactory(cursor, gameCamera, factories);
                                    if (factoryPointedAt != null)
                                    {
                                        graphicsDevice.SetRenderTarget(normalDepthRenderTargetLow);
                                        factoryPointedAt.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalDepth");
                                        edgeDetectionParameters["EdgeColor"].SetValue(Color.Gold.ToVector4());
                                    }
                                    else
                                    {
                                        if (CursorManager.MouseOnResearchFacility(cursor, gameCamera, researchFacility))
                                        {
                                            graphicsDevice.SetRenderTarget(normalDepthRenderTargetLow);
                                            researchFacility.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalDepth");
                                            edgeDetectionParameters["EdgeColor"].SetValue(Color.Gold.ToVector4());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }

        public void ApplyEdgeDetection(RenderTarget2D wholeScene, RenderTarget2D normalDepthRenderTarget, GraphicsDevice graphicsDevice, RenderTarget2D edgeDetectionRenderTarget)
        {
            Effect effectToUse = null;
            if (GameSettings.SpecialEffectsEnabled) effectToUse = edgeDetectionEffect;
            Vector2 resolution = new Vector2(wholeScene.Width, wholeScene.Height);
            Texture2D normalDepthTexture = normalDepthRenderTarget;
            edgeDetectionParameters["ScreenResolution"].SetValue(resolution);
            edgeDetectionParameters["NormalDepthTexture"].SetValue(normalDepthTexture);
            edgeDetectionEffect.CurrentTechnique = edgeDetectionEffect.Techniques["EdgeDetect"];

            graphicsDevice.SetRenderTarget(edgeDetectionRenderTarget);
            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, edgeDetectionEffect);
            spriteBatch.Draw(wholeScene, Vector2.Zero, Color.White);
            spriteBatch.End();
        }


        /// <summary>
        /// Helper for drawing a texture into a rendertarget, using
        /// a custom shader to apply postprocessing effects.
        /// </summary>
        void DrawFullscreenQuad(Texture2D texture, RenderTarget2D renderTarget,
                                Effect effect, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetRenderTarget(renderTarget);

            DrawFullscreenQuad(texture,
                               renderTarget.Width, renderTarget.Height,
                               effect);
        }


        /// <summary>
        /// Helper for drawing a texture into the current rendertarget,
        /// using a custom shader to apply postprocessing effects.
        /// </summary>
        void DrawFullscreenQuad(Texture2D texture, int width, int height,
                                Effect effect)
        {
            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, effect);
            spriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
            spriteBatch.End();
        }


        /// <summary>
        /// Computes sample weightings and texture coordinate offsets
        /// for one pass of a separable gaussian blur filter.
        /// </summary>
        void SetBlurEffectParameters(float dx, float dy)
        {
            // Look up the sample weight and offset effect parameters.
            EffectParameter weightsParameter, offsetsParameter;

            weightsParameter = customBlurEffect.Parameters["SampleWeights"];
            offsetsParameter = customBlurEffect.Parameters["SampleOffsets"];

            // Look up how many samples our gaussian blur effect supports.
            int sampleCount = weightsParameter.Elements.Count;

            // Create temporary arrays for computing our filter settings.
            float[] sampleWeights = new float[sampleCount];
            Vector2[] sampleOffsets = new Vector2[sampleCount];

            // The first sample always has a zero offset.
            sampleWeights[0] = ComputeGaussian(0);
            sampleOffsets[0] = new Vector2(0);

            // Maintain a sum of all the weighting values.
            float totalWeights = sampleWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (int i = 0; i < sampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeGaussian(i + 1);

                sampleWeights[i * 2 + 1] = weight;
                sampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                // To get the maximum amount of blurring from a limited number of
                // pixel shader samples, we take advantage of the bilinear filtering
                // hardware inside the texture fetch unit. If we position our texture
                // coordinates exactly halfway between two texels, the filtering unit
                // will average them for us, giving two samples for the price of one.
                // This allows us to step in units of two texels per sample, rather
                // than just one at a time. The 1.5 offset kicks things off by
                // positioning us nicely in between two texels.
                float sampleOffset = i * 2 + 1.5f;

                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative taps.
                sampleOffsets[i * 2 + 1] = delta;
                sampleOffsets[i * 2 + 2] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to one.
            for (int i = 0; i < sampleWeights.Length; i++)
            {
                sampleWeights[i] /= totalWeights;
            }

            // Tell the effect about our new filter settings.
            weightsParameter.SetValue(sampleWeights);
            offsetsParameter.SetValue(sampleOffsets);
        }


        /// <summary>
        /// Evaluates a single point on the gaussian falloff curve.
        /// Used for setting up the blur filter weightings.
        /// </summary>
        float ComputeGaussian(float n)
        {
            float theta;
            if (HydroBot.supersonicMode == true)
                theta = 20;
            else 
                theta = 2;//blur amount

            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
                           Math.Exp(-(n * n) / (2 * theta * theta)));
        }
    }
}
