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

        Effect underWaterEffect;
        bool underWaterEffectEnabled = true;
        bool bloomEffectEnabled = false;
        RenderTarget2D beforeBloomTexture, afterBloomTexture;

        PresentationParameters pp;

        float m_Timer = 0;

        public GraphicEffect(GameScene gameScene, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            bloom = new BloomComponent(gameScene.Game);
            // Look up the resolution and format of our main backbuffer.
            pp = gameScene.Game.GraphicsDevice.PresentationParameters;

            beforeBloomTexture = new RenderTarget2D(gameScene.Game.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, DepthFormat.None);
            afterBloomTexture = new RenderTarget2D(gameScene.Game.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, DepthFormat.None);

            underWaterEffect = gameScene.Game.Content.Load<Effect>("Shaders/UnderWater");
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


        public void Draw(GameTime gameTime, Texture2D originalScene, GraphicsDeviceManager graphics)
        {
            graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);
            graphics.GraphicsDevice.SetRenderTarget(beforeBloomTexture);
            if (underWaterEffectEnabled)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                {
                    underWaterEffect.Parameters["fTimer"].SetValue(m_Timer);
                    // Apply the underwater effect post process shader
                    underWaterEffect.CurrentTechnique.Passes[0].Apply();
                    {
                        spriteBatch.Draw(originalScene, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
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
                bloom.Draw(gameTime, beforeBloomTexture, afterBloomTexture);
            }
            else
            {
                afterBloomTexture = beforeBloomTexture;
            }

            graphics.GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin();
            spriteBatch.Draw(afterBloomTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
            spriteBatch.End();

            DrawOverlayText();
        }

        void DrawOverlayText()
        {
            string text = "A = settings (" + bloom.Settings.Name + ")\n" +
                          "B = toggle bloom (" + (bloomEffectEnabled ? "on" : "off") + ")\n" +
                          "D = show buffer (" + bloom.ShowBuffer.ToString() + ")\n" +
                          "U = toggle underwater (" + (underWaterEffectEnabled ? "on" : "off") + ")";

            spriteBatch.Begin();

            // Draw the string twice to create a drop shadow, first colored black
            // and offset one pixel to the bottom right, then again in white at the
            // intended position. This makes text easier to read over the background.
            spriteBatch.DrawString(spriteFont, text, new Vector2(65, 65), Color.Black);
            spriteBatch.DrawString(spriteFont, text, new Vector2(64, 64), Color.White);

            spriteBatch.End();
        }
    }
}
