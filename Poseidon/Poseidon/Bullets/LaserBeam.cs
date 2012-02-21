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
    public class LaserBeam : DamageBullet
    {
        public float forwardDir;

        Texture2D laserBeamTexture;
        SpriteBatch spriteBatch;
        GraphicsDevice graphicDevice;
        Camera gameCamera;
        GameMode gameMode;

        public LaserBeam() : base() { }

        public void initialize(Vector3 position, Vector3 headingDirection, float speed,
            int damage, GameObject target, BaseEnemy shooter, GameMode gameMode)
        {
            base.initialize(position, headingDirection, speed, damage, shooter);
            this.unitDirection = headingDirection; 
            this.unitDirection.Normalize();
            this.gameMode = gameMode;

            if (gameMode == GameMode.MainGame){
                this.graphicDevice = PlayGameScene.GraphicDevice;
                this.gameCamera = PlayGameScene.gameCamera;
                this.spriteBatch = PoseidonGame.spriteBatch;
            }
            else if (gameMode == GameMode.ShipWreck){
                this.graphicDevice = ShipWreckScene.GraphicDevice;
                this.gameCamera = ShipWreckScene.gameCamera;
                this.spriteBatch = PoseidonGame.spriteBatch;
            }
            else if (gameMode == GameMode.SurvivalMode){
                this.graphicDevice = SurvivalGameScene.GraphicDevice;
                this.gameCamera = SurvivalGameScene.gameCamera;
                this.spriteBatch = PoseidonGame.spriteBatch;
            }

            Vector3 direction2D = graphicDevice.Viewport.Project(position + headingDirection, gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity)
                - graphicDevice.Viewport.Project(shooter.Position, gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity);
            direction2D.Normalize();
            this.forwardDir = (float)Math.Atan2(direction2D.X, direction2D.Y);
            laserBeamTexture = PoseidonGame.contentManager.Load<Texture2D>("Image/BulletIcons/laserBeam");
        }

        public override void update(GameTime gameTime)
        {
            Position += unitDirection * projectionSpeed;
            BoundingSphere = new BoundingSphere(Position, BoundingSphere.Radius);

            EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);
        }

        public override void draw(Matrix view, Matrix projection)
        {
            Vector3 screenPos = graphicDevice.Viewport.Project(Position, gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity);
            Vector2 laserBeamPos;
            laserBeamPos.X = screenPos.X;
            laserBeamPos.Y = screenPos.Y;
            spriteBatch.Begin();
            spriteBatch.Draw(laserBeamTexture, laserBeamPos, null, Color.White, -forwardDir,new Vector2(laserBeamTexture.Width / 2, laserBeamTexture.Height / 2), 0.2f, SpriteEffects.None, 1); 
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
