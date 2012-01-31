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
    class GraphicEffect
    {
        BloomComponent bloom;
        int bloomSettingsIndex = 0;

        KeyboardState lastKeyboardState = new KeyboardState();
        KeyboardState currentKeyboardState = new KeyboardState();

        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        Effect underWaterEffect, screenTransitionEffect, edgeDetectionEffect;
        bool underWaterEffectEnabled = true;
        bool bloomEffectEnabled = false;
        RenderTarget2D afterUnderWaterTexture, afterBloomTexture, afterEffectsRenderTarget;
        EffectParameterCollection edgeDetectionParameters;

        PresentationParameters pp;

        float m_Timer = 0;
        Random random;
        float fadeBetweenScenes = 1.0f;

        public GraphicEffect(GameScene gameScene, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            random = new Random();
            bloom = new BloomComponent(gameScene.Game);
            // Look up the resolution and format of our main backbuffer.
            pp = gameScene.Game.GraphicsDevice.PresentationParameters;

            afterUnderWaterTexture = new RenderTarget2D(gameScene.Game.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, DepthFormat.None);
            afterBloomTexture = new RenderTarget2D(gameScene.Game.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, DepthFormat.None); 
            afterEffectsRenderTarget = new RenderTarget2D(gameScene.Game.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, DepthFormat.None); 

            underWaterEffect = gameScene.Game.Content.Load<Effect>("Shaders/UnderWater");
            screenTransitionEffect = gameScene.Game.Content.Load<Effect>("Shaders/ScreenTransition");
            edgeDetectionEffect = gameScene.Game.Content.Load<Effect>("Shaders/EdgeDetectionEffect");


            edgeDetectionParameters = edgeDetectionEffect.Parameters;
            edgeDetectionEffect.CurrentTechnique = edgeDetectionEffect.Techniques["EdgeDetect"];

            this.spriteBatch = spriteBatch;
            this.spriteFont = spriteFont;
        }

        public void UpdateInput(GameTime gameTime)
        {
            lastKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            // Switch underwater effect on/off
            if (currentKeyboardState.IsKeyDown(Keys.U) &&
                 lastKeyboardState.IsKeyUp(Keys.U))
            {
                underWaterEffectEnabled = !underWaterEffectEnabled;
            }

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


        public RenderTarget2D DrawWithEffects(GameTime gameTime, Texture2D originalScene, GraphicsDeviceManager graphics)
        {
            //graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);
            graphics.GraphicsDevice.SetRenderTarget(afterUnderWaterTexture);
            if (underWaterEffectEnabled)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                {
                    underWaterEffect.Parameters["fTimer"].SetValue(m_Timer);
                    // Apply the underwater effect post process shader
                    underWaterEffect.CurrentTechnique.Passes[0].Apply();
                    {
                        spriteBatch.Draw(originalScene, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight),
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
            if (bloomEffectEnabled)
            {
                afterBloomTexture = new RenderTarget2D(graphics.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, DepthFormat.None);
                //afterBloomTexture = new RenderTarget2D(graphics.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight,
                //false, graphics.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24Stencil8);
                bloom.Draw(gameTime, afterUnderWaterTexture, afterBloomTexture);
            }
            else
            {
                afterBloomTexture = afterUnderWaterTexture;
            }
            graphics.GraphicsDevice.SetRenderTarget(afterEffectsRenderTarget);
            spriteBatch.Begin();
            spriteBatch.Draw(afterBloomTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
            spriteBatch.End();
            //graphics.GraphicsDevice.SamplerStates[1] = SamplerState.AnisotropicClamp;
            //graphics.GraphicsDevice.Textures[1] = afterBloomTexture;          
            DrawOverlayText();
            return afterEffectsRenderTarget;
        }

        void DrawOverlayText()
        {
            string text = "A = settings (" + bloom.Settings.Name + ")\n" +
                          "B = toggle bloom (" + (bloomEffectEnabled ? "on" : "off") + ")\n" +
                          "D = show buffer (" + bloom.ShowBuffer.ToString() + ")\n" +
                          "U = toggle underwater (" + (underWaterEffectEnabled ? "on" : "off") + ")";

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
            fadeBetweenScenes -= 0.01f;
            if (fadeBetweenScenes <= 0.0f)
            {
                fadeBetweenScenes = 1.0f;
                return true;
            }
            else return false;
        }

        public void PrepareEdgeDetect(Cursor cursor, Camera gameCamera, Fish[] fish, int fishAmount, BaseEnemy[] enemies, int enemiesAmount, GraphicsDevice graphicsDevice, RenderTarget2D normalDepthRenderTarget)
        {
            graphicsDevice.SetRenderTarget(normalDepthRenderTarget);
            graphicsDevice.Clear(Color.Black);
            graphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            Fish fishPointedAt = CursorManager.MouseOnWhichFish(cursor, gameCamera, fish, fishAmount);
            if (fishPointedAt != null)
            {
                fishPointedAt.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalDepth");
                edgeDetectionParameters["EdgeColor"].SetValue(new Vector4(0, 1, 0, 1));
            }
            else
            {
                BaseEnemy enemyPointedAt = CursorManager.MouseOnWhichEnemy(cursor, gameCamera, enemies, enemiesAmount);
                if (enemyPointedAt != null)
                {
                    enemyPointedAt.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix, gameCamera, "NormalDepth");
                    edgeDetectionParameters["EdgeColor"].SetValue(new Vector4(1, 0, 0, 1));
                }
            }

            graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }

        public void ApplyEdgeDetection(RenderTarget2D wholeScene, RenderTarget2D normalDepthRenderTarget, GraphicsDevice graphicsDevice, RenderTarget2D edgeDetectionRenderTarget)
        {
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
    }
}
