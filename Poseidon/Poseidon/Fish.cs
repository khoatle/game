using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SkinnedModel;
using Poseidon.Core;

namespace Poseidon {
    public class Fish : SwimmingObject {

        Matrix[] bones;
        SkinningData skd;
        ClipPlayer clipPlayer;
        Matrix fishMatrix;
        Quaternion qRotation = Quaternion.Identity;

        public void Load(int clipStart, int clipEnd, int fpsRate)
        {
            skd = Model.Tag as SkinningData;
            clipPlayer = new ClipPlayer(skd, fpsRate);//ClipPlayer running at 24 frames/sec
            AnimationClip clip = skd.AnimationClips["Take 001"]; //Take name from the dude.fbx file
            clipPlayer.play(clip, clipStart, clipEnd, true);
            fishMatrix = Matrix.CreateScale(1.0f) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                               Matrix.CreateTranslation(Position);
            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;
            scaledSphere.Radius *= 1.5f;
            BoundingSphere =
                new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);
        }

        public void Update(GameTime gameTime, SwimmingObject[] enemies, int enemiesSize, SwimmingObject[] fish, int fishSize, int changeDirection, Tank tank, List<DamageBullet> enemyBullet) {
            Vector3 futurePosition = Position;
            //int barrier_move
            Random random = new Random();
            float turnAmount = 0;
            //also try to change direction if we are stuck
            if (stucked == true)
            {
                ForwardDirection += MathHelper.PiOver4;
            }
            else if (changeDirection >= 95)
            {
                int rightLeft = random.Next(2);
                if (rightLeft == 0)
                    turnAmount = 20;
                else turnAmount = -20;
            }

            Matrix orientationMatrix;
            // Vector3 speed;
            Vector3 movement = Vector3.Zero;

            movement.Z = 1;
            float prevForwardDir = ForwardDirection;
            Vector3 prevFuturePosition = futurePosition;
            // try upto 10 times to change direction is there is collision
            for (int i = 0; i < 4; i++)
            {
                ForwardDirection += turnAmount * GameConstants.TurnSpeed;
                orientationMatrix = Matrix.CreateRotationY(ForwardDirection);
                Vector3 headingDirection = Vector3.Transform(movement, orientationMatrix);
                headingDirection *= GameConstants.FishSpeed;
                futurePosition = Position + headingDirection;

                if (Collision.isBarriersValidMove(this, futurePosition, enemies, enemiesSize, tank) &&
                    Collision.isBarriersValidMove(this, futurePosition, fish, fishSize, tank))
                {
                    Position = futurePosition;

                    BoundingSphere updatedSphere;
                    updatedSphere = BoundingSphere;

                    updatedSphere.Center.X = Position.X;
                    updatedSphere.Center.Z = Position.Z;
                    BoundingSphere = new BoundingSphere(updatedSphere.Center,
                        updatedSphere.Radius);

                    stucked = false;
                    break;
                }
                else
                {
                    stucked = true;
                    futurePosition = prevFuturePosition;
                }
            }

            // if clip player has been initialized, update it
            if (clipPlayer != null)
            {
                qRotation = Quaternion.CreateFromAxisAngle(
                                Vector3.Up,
                                ForwardDirection);
                fishMatrix = Matrix.CreateScale(1.0f) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                                    Matrix.CreateFromQuaternion(qRotation) *
                                    Matrix.CreateTranslation(Position);
                clipPlayer.update(gameTime.ElapsedGameTime, true, fishMatrix);
            }
        }
        public new void Draw(Matrix view, Matrix projection)
        {
            if (clipPlayer == null)
            {
                // just return for now.. Some of the fishes do not have animation, so clipPlayer won't be initialized for them
                base.Draw(view, projection);
                return;
            }

            bones = clipPlayer.GetSkinTransforms();

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {

                    effect.SetBoneTransforms(bones);
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }

        }
    }
}
