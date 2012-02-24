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
        public float rotation = 0;
        //Texture2D energyBallTexture;
        //Vector2 energyBallPos;
        //SpriteBatch spriteBatch;
        GameMode gameMode;
        Camera gameCamera;
        
        //whether the arrow should pierce through the enemies
        public bool piercingArrow = false;
        //enemies that have been hit by this piercing arrow
        //we don't want 1 enemy to get hit twice
        public List<BaseEnemy> hitEnemies;

        public HerculesBullet(ContentManager content, SpriteBatch spriteBatch, GameMode gameMode, float forwardDir, bool piercingArrow)
            : base()
        {
            //energyBallTexture = content.Load<Texture2D>("Image/Miscellaneous/energyBall-red");
            //this.spriteBatch = spriteBatch;
            this.forwardDir = forwardDir;
            this.gameMode = gameMode;
            if (gameMode == GameMode.MainGame)
            {
                gameCamera = PlayGameScene.gameCamera;
            }
            else if (gameMode == GameMode.ShipWreck)
            {
                gameCamera = ShipWreckScene.gameCamera;
            }
            else if (gameMode == GameMode.SurvivalMode)
            {
                gameCamera = SurvivalGameScene.gameCamera;
            }
            this.piercingArrow = piercingArrow;
            hitEnemies = new List<BaseEnemy>();
        }

        public override void draw(Matrix view, Matrix projection)
        {
            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix translationMatrix = Matrix.CreateTranslation(Position);
            rotation += MathHelper.PiOver4 ;
            Matrix rotationMatrix = Matrix.CreateFromAxisAngle(Vector3.UnitZ, rotation) * Matrix.CreateRotationY(forwardDir);
            Matrix worldMatrix = rotationMatrix * translationMatrix;

            foreach (ModelMesh mesh in Model.Meshes)
            {
                //foreach (BasicEffect effect in mesh.Effects)
                foreach (Effect effect in mesh.Effects)
                {
                    //effect.World = worldMatrix * transforms[mesh.ParentBone.Index];
                    //effect.DiffuseColor = Color.Gold.ToVector3();
                    //effect.View = view;
                    //effect.Projection = projection;

                    //effect.EnableDefaultLighting();
                    //effect.PreferPerPixelLighting = true;

                    //effect.FogEnabled = true;
                    //effect.FogStart = GameConstants.FogStart;
                    //effect.FogEnd = GameConstants.FogEnd;
                    //effect.FogColor = GameConstants.FogColor.ToVector3();

                    //for our custom BasicEffect
                    effect.CurrentTechnique = effect.Techniques["NormalShading"];
                    Matrix readlWorldMatrix = worldMatrix * transforms[mesh.ParentBone.Index];
                    effect.Parameters["World"].SetValue(readlWorldMatrix);
                    effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Invert(readlWorldMatrix));
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                    effect.Parameters["EyePosition"].SetValue(new Vector4(gameCamera.AvatarHeadOffset, 0));
                    Matrix WorldView = readlWorldMatrix * view;
                    //EffectHelpers.SetFogVector(ref WorldView, GameConstants.FogStart, GameConstants.FogEnd, effect.Parameters["FogVector"]);
                    //effect.Parameters["FogColor"].SetValue(GameConstants.FogColor.ToVector3());
                    //effect.Parameters["AmbientColor"].SetValue(Vector4.One);
                    if (!piercingArrow)
                        effect.Parameters["DiffuseColor"].SetValue(Color.Gold.ToVector4());
                    else
                        effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector4());
                    effect.Parameters["DiffuseIntensity"].SetValue(100.0f);
                    //effect.Parameters["SpecularColor"].SetValue(Vector4.One);

                }
                mesh.Draw();
                //foreach (Effect effect in mesh.Effects)
                //{
                //    effect.CurrentTechnique = effect.Techniques["BalloonShading"];
                //    Matrix readlWorldMatrix = worldMatrix * transforms[mesh.ParentBone.Index];
                //    effect.Parameters["gWorldXf"].SetValue(readlWorldMatrix);
                //    effect.Parameters["gWorldITXf"].SetValue(Matrix.Invert(readlWorldMatrix));
                //    effect.Parameters["gWvpXf"].SetValue(readlWorldMatrix * view * projection);
                //    effect.Parameters["gViewIXf"].SetValue(Matrix.Invert(view));
                //    effect.Parameters["gInflate"].SetValue(0.6f);
                //    effect.Parameters["gGlowColor"].SetValue(Color.Gold.ToVector3());
                //}
                //mesh.Draw();
            }
            //Vector3 screenPos = Vector3.Zero;
            //if (inGameScene.GetType().Name.Equals("PlayGameScene"))
            //    screenPos = PlayGameScene.GraphicDevice.Viewport.Project(Position, PlayGameScene.gameCamera.ProjectionMatrix, PlayGameScene.gameCamera.ViewMatrix, Matrix.Identity);
            //else if (inGameScene.GetType().Name.Equals("ShipWreckScene"))
            //    screenPos = ShipWreckScene.GraphicDevice.Viewport.Project(Position, ShipWreckScene.gameCamera.ProjectionMatrix, ShipWreckScene.gameCamera.ViewMatrix, Matrix.Identity);
            //else if (inGameScene.GetType().Name.Equals("SurvivalGameScene"))
            //    screenPos = SurvivalGameScene.GraphicDevice.Viewport.Project(Position, SurvivalGameScene.gameCamera.ProjectionMatrix, SurvivalGameScene.gameCamera.ViewMatrix, Matrix.Identity);
            //energyBallPos.X = screenPos.X;
            //energyBallPos.Y = screenPos.Y;
            //float scale = 1.0f;
            //if (!inGameScene.GetType().Name.Equals("PlayGameScene")) scale = 1.5f;
            //spriteBatch.Begin();
            //spriteBatch.Draw(energyBallTexture, energyBallPos, null, Color.White, 0, new Vector2(energyBallTexture.Height / 2, energyBallTexture.Width / 2), scale, SpriteEffects.None, 0);
            ////spriteBatch.Draw(energyBallTexture, energyBallPos, Color.White); 
            //spriteBatch.End();
            //if (inGameScene.GetType().Name.Equals("PlayGameScene"))
            //    PlayGameScene.RestoreGraphicConfig();
            //else if (inGameScene.GetType().Name.Equals("ShipWreckScene"))
            //    ShipWreckScene.RestoreGraphicConfig();
            //else if (inGameScene.GetType().Name.Equals("SurvivalGameScene"))
            //    SurvivalGameScene.RestoreGraphicConfig();
        }
    }
}
