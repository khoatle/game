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
    public class FlyingHammer : DamageBullet
    {
        public float forwardDir;
        //the hammer has travelled long enough
        public bool explodeNow = false;
        public double timeShot;
        GameMode gameMode;
        Camera gameCamera;

        public FlyingHammer(GameMode gameMode)
            : base()
        {
            this.timeShot = PoseidonGame.playTime.TotalSeconds;
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
        }

        public override void draw(Matrix view, Matrix projection)
        {
            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix translationMatrix = Matrix.CreateTranslation(Position);
            forwardDir += MathHelper.PiOver4;
            Matrix rotationMatrix = Matrix.CreateRotationY(forwardDir);
            Matrix worldMatrix = rotationMatrix * translationMatrix;

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                //foreach (Effect effect in mesh.Effects)
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

                    //for our custom BasicEffect
                    //effect.CurrentTechnique = effect.Techniques["NormalShading"];
                    //Matrix readlWorldMatrix = worldMatrix * transforms[mesh.ParentBone.Index];
                    //effect.Parameters["World"].SetValue(readlWorldMatrix);
                    //effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Invert(readlWorldMatrix));
                    //effect.Parameters["View"].SetValue(view);
                    //effect.Parameters["Projection"].SetValue(projection);
                    //effect.Parameters["EyePosition"].SetValue(new Vector4(gameCamera.AvatarHeadOffset, 0));
                    //Matrix WorldView = readlWorldMatrix * view;
                    //EffectHelpers.SetFogVector(ref WorldView, GameConstants.FogStart, GameConstants.FogEnd, effect.Parameters["FogVector"]);
                    //effect.Parameters["FogColor"].SetValue(GameConstants.FogColor.ToVector3());
                    //effect.Parameters["AmbientColor"].SetValue(Vector4.One);
                    //effect.Parameters["DiffuseColor"].SetValue(Vector4.One);
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
                //    effect.Parameters["gGlowColor"].SetValue(new Vector3(1, 0, 0));
                //}
                //mesh.Draw();
            }
            
        }
    }
}
