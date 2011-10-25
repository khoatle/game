using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Poseidon.Core;
namespace Poseidon
{
    public class HerculesBullet : DamageBullet
    {
        public GameObject target;
        public float forwardDir;
        Texture2D energyBallTexture;
        Vector2 energyBallPos;
        SpriteBatch spriteBatch;
        GameScene inGameScene;
        public HerculesBullet(ContentManager content, SpriteBatch spriteBatch, GameScene inGameScene)
            : base()
        {
            energyBallTexture = content.Load<Texture2D>("Image/energyBall-red");
            this.spriteBatch = spriteBatch;
            this.inGameScene = inGameScene;
        }

        public override void draw(Matrix view, Matrix projection)
        {
            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix translationMatrix = Matrix.CreateTranslation(Position);
            forwardDir += MathHelper.PiOver4 ;
            Matrix rotationMatrix = Matrix.CreateRotationY(forwardDir);
            Matrix worldMatrix = rotationMatrix * translationMatrix;

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = worldMatrix * transforms[mesh.ParentBone.Index];
                    effect.DiffuseColor = Color.Gold.ToVector3();
                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;

                    effect.FogEnabled = true;
                    effect.FogStart = GameConstants.FogStart;
                    effect.FogEnd = GameConstants.FogEnd;
                    effect.FogColor = GameConstants.FogColor.ToVector3();
                }
                mesh.Draw();
            }
            Vector3 screenPos;
            if (inGameScene.GetType().Name.Equals("PlayGameScene"))
                screenPos = PlayGameScene.GraphicDevice.Viewport.Project(Position, PlayGameScene.gameCamera.ProjectionMatrix, PlayGameScene.gameCamera.ViewMatrix, Matrix.Identity);
            else
                screenPos = ShipWreckScene.GraphicDevice.Viewport.Project(Position, ShipWreckScene.gameCamera.ProjectionMatrix, ShipWreckScene.gameCamera.ViewMatrix, Matrix.Identity);
            energyBallPos.X = screenPos.X;
            energyBallPos.Y = screenPos.Y;
            float scale = 1.0f;
            if (!inGameScene.GetType().Name.Equals("PlayGameScene")) scale = 1.5f;
            spriteBatch.Begin();
            spriteBatch.Draw(energyBallTexture, energyBallPos, null, Color.White, 0, new Vector2(energyBallTexture.Height / 2, energyBallTexture.Width / 2), scale, SpriteEffects.None, 0);
            //spriteBatch.Draw(energyBallTexture, energyBallPos, Color.White); 
            spriteBatch.End();
            if (inGameScene.GetType().Name.Equals("PlayGameScene"))
                PlayGameScene.RestoreGraphicConfig();
            else ShipWreckScene.RestoreGraphicConfig();
        }
    }
}
