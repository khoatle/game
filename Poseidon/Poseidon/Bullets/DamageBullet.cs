using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Poseidon {
    public class DamageBullet : Projectiles {
        public int damage = GameConstants.DefaultBulletDamage;
        public BaseEnemy shooter = null;

        Texture2D laserBeamTexture;
        SpriteBatch spriteBatch;
        GraphicsDevice graphicDevice;
        Camera gameCamera;
        GameMode gameMode;

        public void initialize(Vector3 position, Vector3 headingDirection, float speed, float strength, float strengthUp, GameMode gameMode)
        {
            base.initialize(position, headingDirection, speed);
            damage = (int) (GameConstants.DefaultBulletDamage*strength*strengthUp);

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

            laserBeamTexture = PoseidonGame.contentManager.Load<Texture2D>("Image/BulletIcons/redBall");
        }

        public void initialize(Vector3 position, Vector3 headingDirection, float speed, int damage, BaseEnemy shooter) {
            base.initialize(position, headingDirection, speed);
            this.damage = damage;
            this.shooter = shooter;
        }

        public override void draw(Matrix view, Matrix projection)
        {
            if (shooter == null)
            {
                Vector3 screenPos = graphicDevice.Viewport.Project(Position, gameCamera.ProjectionMatrix, gameCamera.ViewMatrix, Matrix.Identity);
                Vector2 laserBeamPos;
                laserBeamPos.X = screenPos.X;
                laserBeamPos.Y = screenPos.Y;
                spriteBatch.Begin();
                spriteBatch.Draw(laserBeamTexture, laserBeamPos, null, Color.White, 0, new Vector2(laserBeamTexture.Width / 2, laserBeamTexture.Height / 2), 0.2f, SpriteEffects.None, 1);
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
            else
            {
                base.draw(view, projection);
            }
        }
    }
}
