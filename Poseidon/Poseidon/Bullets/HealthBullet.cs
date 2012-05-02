using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Poseidon.Core;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Poseidon
{
    public class HealthBullet : Projectiles {
        public int healthAmount;

        Texture2D laserBeamTexture;
        SpriteBatch spriteBatch;
        GraphicsDevice graphicDevice;
        Camera gameCamera;
        GameMode gameMode;
        float forwardDir;

        public void initialize(Vector3 position, Vector3 headingDirection, float speed, float strength, float strengthUp, GameMode gameMode)
        {
            base.initialize(position, headingDirection, speed);
            healthAmount = (int)(GameConstants.HealingAmount * strength * strengthUp);

            this.gameMode = gameMode;
            if (gameMode == GameMode.MainGame)
            {
                this.graphicDevice = PlayGameScene.GraphicDevice;
                this.gameCamera = PlayGameScene.gameCamera;
                this.spriteBatch = PoseidonGame.spriteBatch;
            }
            else if (gameMode == GameMode.ShipWreck)
            {
                this.graphicDevice = ShipWreckScene.GraphicDevice;
                this.gameCamera = ShipWreckScene.gameCamera;
                this.spriteBatch = PoseidonGame.spriteBatch;
            }
            else if (gameMode == GameMode.SurvivalMode)
            {
                this.graphicDevice = SurvivalGameScene.GraphicDevice;
                this.gameCamera = SurvivalGameScene.gameCamera;
                this.spriteBatch = PoseidonGame.spriteBatch;
            }

            laserBeamTexture = IngamePresentation.healLaserBeamTexture;


            Vector3 direction2D = graphicDevice.Viewport.Project(position + headingDirection, gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity)
                - graphicDevice.Viewport.Project(position, gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity);
            direction2D.Normalize();
            this.forwardDir = (float)Math.Atan2(direction2D.X, direction2D.Y);
        }

        public override void draw(Matrix view, Matrix projection)
        {
            Vector3 screenPos = graphicDevice.Viewport.Project(Position, gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity);
            Vector2 laserBeamPos;
            laserBeamPos.X = screenPos.X;
            laserBeamPos.Y = screenPos.Y;
            float scale = (float)Math.Sqrt(HydroBot.strength / GameConstants.MainCharStrength);
            scale = MathHelper.Clamp(scale, 1.0f, 1.5f);
            spriteBatch.Begin();
            spriteBatch.Draw(laserBeamTexture, laserBeamPos, null, Color.White, -forwardDir, new Vector2(laserBeamTexture.Width / 2, laserBeamTexture.Height / 2), 0.2f * scale, SpriteEffects.None, 1);
            spriteBatch.End();
            if (gameMode == GameMode.MainGame)
            {
                PlayGameScene.RestoreGraphicConfig();
            }
            else if (gameMode == GameMode.ShipWreck)
            {
                ShipWreckScene.RestoreGraphicConfig();
            }
            else if (gameMode == GameMode.SurvivalMode)
            {
                SurvivalGameScene.RestoreGraphicConfig();
            }
        }
    }
}
