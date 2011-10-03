using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon
{
    public class StaticObjects : GameObject
    {
            public void LoadContent(ContentManager content)
            {

                Model = content.Load<Model>("Models/starfish");
                Position = Vector3.Down;

            }

            public void Draw(Matrix view, Matrix projection)
            {
                Matrix[] transforms = new Matrix[Model.Bones.Count];
                Model.CopyAbsoluteBoneTransformsTo(transforms);
                Matrix translateMatrix = Matrix.CreateTranslation(Position);
                Matrix worldMatrix = translateMatrix;

                foreach (ModelMesh mesh in Model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World =
                            worldMatrix * transforms[mesh.ParentBone.Index];
                        effect.View = view;
                        effect.Projection = projection;

                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                    }
                    mesh.Draw();
                }
            }
    }
}
