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
        protected ClipPlayer clipPlayer;
        protected Matrix fishMatrix;
        protected Quaternion qRotation = Quaternion.Identity;
        double lastHealthUpdateTime;
        double healthChangeInterval;
        public string happy_talk;
        public string sad_talk;
        public bool flee = false;
        public BaseEnemy currentTarget = null;
        public bool isWandering = true;
        public bool isReturnBot;
        public bool isChasing;
        public bool isFighting;
        public bool isCasting;
        float turnAmount = 0;

        public Fish() : base() {
            basicExperienceReward = GameConstants.BasicExpHealingFish;
            lastHealthUpdateTime = 0;
            healthChangeInterval = GameConstants.maxHealthChangeInterval;
            health = GameConstants.DefaultFishHP;
        }

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

            float scale = 1.0f;
            if (Name.Contains("turtle")) scale = 0.4f;
            if (Name.Contains("dolphin")) scale = 0.25f;
            if (Name.Contains("manetee")) scale = 0.4f;
            if (Name.Contains("sting ray")) scale = 0.5f;
            if (Name.Contains("orca")) scale = 0.5f;
            if (Name.Contains("seal")) scale = 0.5f;
            if (Name.Contains("shark")) scale = 0.5f;
            if (isBigBoss)
            {
                scale *= 2.0f;
                maxHealth = 5000;
                health = 5000;
            }
            scaledSphere.Radius *= scale;

            BoundingSphere =
                new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);

            // Set up the parameters
            SetupShaderParameters(PoseidonGame.contentManager, Model);
            EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);
        }

        public virtual void Update(GameTime gameTime, SwimmingObject[] enemies, int enemiesSize, SwimmingObject[] fish, int fishSize, int changeDirection, HydroBot tank, List<DamageBullet> enemyBullet)
        {
            float lastForwardDir = ForwardDirection;
            float lastTurnAmount = turnAmount;
            EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);

            if (isPoissoned == true) {
                if (accumulatedHealthLossFromPoison < maxHPLossFromPoisson) {
                    health -= 0.1f;
                    accumulatedHealthLossFromPoison += 0.1f;
                }
                else {
                    isPoissoned = false;
                    accumulatedHealthLossFromPoison = 0;
                }
            }
            
            Vector3 futurePosition = Position;
            //int barrier_move
            Random random = new Random();
            
            //also try to change direction if we are stuck
            int rightLeft = random.Next(2);
            turnAmount = 0;
            if (stucked == true)
            {
                //ForwardDirection += MathHelper.PiOver4/2;
                if (lastTurnAmount == 0)
                {
                    if (rightLeft == 0)
                        turnAmount = 5;
                    else turnAmount = -5;
                }
                else turnAmount = lastTurnAmount;
            }
            else if (changeDirection >= 99)
            {
                if (rightLeft == 0)
                    turnAmount = 5;
                else turnAmount = -5;
            }

            Matrix orientationMatrix;
            // Vector3 speed;
            Vector3 movement = Vector3.Zero;

            movement.Z = 1;
            float prevForwardDir = ForwardDirection;
            Vector3 prevFuturePosition = futurePosition;
            // try upto 10 times to change direction if there is collision
            //for (int i = 0; i < 4; i++)
            //{
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
                    //break;
                }
                else
                {
                    stucked = true;
                    futurePosition = prevFuturePosition;
                }

                if (ForwardDirection - lastForwardDir > 0)
                {
                    if (!clipPlayer.inRange(29, 31))
                        clipPlayer.switchRange(29, 31);
                }
                else if (ForwardDirection - lastForwardDir < 0)
                {
                    if (!clipPlayer.inRange(40, 42))
                        clipPlayer.switchRange(40, 42);
                }
                else
                {
                    if (!clipPlayer.inRange(1, 24))
                        clipPlayer.switchRange(1, 24);
                }
            //}

            // if clip player has been initialized, update it
            if (clipPlayer != null)
            {
                qRotation = Quaternion.CreateFromAxisAngle(
                                Vector3.Up,
                                ForwardDirection);
                float scale = 1.0f;
                if (Name.Contains("dolphin") || Name.Contains("turtle")) scale = 0.5f;
                if (Name.Contains("manetee")) scale = 0.6f;
                if (Name.Contains("seal")) scale = 1.1f;
                if (isBigBoss) scale *= 2.0f;
                fishMatrix = Matrix.CreateScale(scale) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                                    Matrix.CreateFromQuaternion(qRotation) *
                                    Matrix.CreateTranslation(Position);
                clipPlayer.update(gameTime.ElapsedGameTime, true, fishMatrix);
            }

            if (PoseidonGame.playTime.TotalSeconds - lastHealthUpdateTime > healthChangeInterval)
            {
                double env_health = (double)HydroBot.currentEnvPoint / (double)HydroBot.maxEnvPoint;
                double env_deviation = 0;
                if ( env_health > 0.5)
                {
                    if (this.health >= this.maxHealth)
                        this.maxHealth += GameConstants.healthChangeValue;
                    this.health += GameConstants.healthChangeValue;
                    env_deviation = env_health - 0.5;
                }
                else if (env_health < 0.5)
                {
                    this.health -= GameConstants.healthChangeValue;
                    env_deviation = 0.5 - env_health;
                }
                lastHealthUpdateTime = PoseidonGame.playTime.TotalSeconds;
                healthChangeInterval = GameConstants.maxHealthChangeInterval - env_deviation*10;
                //System.Diagnostics.Debug.WriteLine(healthChangeInterval);
            }

        }

        // our custom shader
        Effect newSkinnedeffect;

        public void SetupShaderParameters(ContentManager content, Model model)
        {
            newSkinnedeffect = content.Load<Effect>("Shaders/NewSkinnedEffect");
            EffectHelpers.ChangeEffectUsedByModelToCustomSkinnedEffect(model, newSkinnedeffect);
        }

        public override void Draw(Matrix view, Matrix projection, Camera gameCamera, string techniqueName)
        {
            if (clipPlayer == null)
            {
                // just return for now. Some of the fishes do not have animation, so clipPlayer won't be initialized for them
                base.Draw(view, projection);
                return;
            }

            bones = clipPlayer.GetSkinTransforms();

            foreach (ModelMesh mesh in Model.Meshes)
            {
                //foreach (SkinnedEffect effect in mesh.Effects)
                foreach (Effect effect in mesh.Effects)
                {
                    //for standard Skinned Effect
                    //effect.SetBoneTransforms(bones);
                    //effect.View = view;
                    //effect.Projection = projection;

                    //if (isPoissoned) {
                    //    effect.DiffuseColor = Color.Green.ToVector3();
                    //}
                    //else {
                    //    effect.DiffuseColor = Color.White.ToVector3();
                    //}
                    
                    //effect.FogEnabled = true;
                    //effect.FogStart = GameConstants.FogStart;
                    //effect.FogEnd = GameConstants.FogEnd;
                    //effect.FogColor = GameConstants.FogColor.ToVector3();

                    //for our custom SkinnedEffect
                    effect.CurrentTechnique = effect.Techniques[techniqueName];
                    effect.Parameters["World"].SetValue(Matrix.Identity);

                    effect.Parameters["Bones"].SetValue(bones);
                    effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Invert(Matrix.Identity));
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                    effect.Parameters["EyePosition"].SetValue(new Vector4(gameCamera.AvatarHeadOffset, 0));
                    Matrix WorldView = Matrix.Identity * view;
                    effect.Parameters["AmbientColor"].SetValue(ambientColor.ToVector4());
                    effect.Parameters["DiffuseColor"].SetValue(diffuseColor.ToVector4());
                    effect.Parameters["SpecularColor"].SetValue(specularColor.ToVector4());
                    if (isPoissoned == true)
                    {
                        effect.Parameters["DiffuseColor"].SetValue(new Vector4(Color.Green.ToVector3(), 1));
                    }
                    else
                    {
                        effect.Parameters["DiffuseColor"].SetValue(new Vector4(Vector3.One, 1));
                    }
                    EffectHelpers.SetFogVector(ref WorldView, GameConstants.FogStart, GameConstants.FogEnd, effect.Parameters["FogVector"]);
                    effect.Parameters["FogColor"].SetValue(fogColor.ToVector3());
                    effect.Parameters["DiffuseIntensity"].SetValue(0.5f);
                }
                mesh.Draw();
            }
        }


        public BaseEnemy lookForEnemy(SwimmingObject[] enemies, int enemiesSize)
        {
            for (int i = 0; i < enemiesSize; i++)
                if (Vector3.Distance(Position, enemies[i].Position) < GameConstants.SideKick_Look_Radius)
                    return (BaseEnemy)enemies[i];
            return null;
        }

        public virtual void attack() {}

        public void randomWalk(int changeDirection, SwimmingObject[] enemies, int enemiesAmount, SwimmingObject[] fishes, int fishAmount, HydroBot hydroBot)
        {
            float lastForwardDir = ForwardDirection;
            float lastTurnAmount = turnAmount;

            Vector3 futurePosition = Position;
            //int barrier_move
            Random random = new Random();

            //also try to change direction if we are stuck
            int rightLeft = random.Next(2);
            turnAmount = 0;
            if (stucked == true)
            {
                //ForwardDirection += MathHelper.PiOver4/2;
                if (lastTurnAmount == 0)
                {
                    if (rightLeft == 0)
                        turnAmount = 5;
                    else turnAmount = -5;
                }
                else turnAmount = lastTurnAmount;
            }
            else if (changeDirection >= 99)
            {
                if (rightLeft == 0)
                    turnAmount = 5;
                else turnAmount = -5;
            }

            Matrix orientationMatrix;
            // Vector3 speed;
            Vector3 movement = Vector3.Zero;

            movement.Z = 1;
            float prevForwardDir = ForwardDirection;
            Vector3 prevFuturePosition = futurePosition;
            // try upto 10 times to change direction is there is collision
            //for (int i = 0; i < 4; i++)
            //{
                ForwardDirection += turnAmount * GameConstants.TurnSpeed;
                orientationMatrix = Matrix.CreateRotationY(ForwardDirection);
                Vector3 headingDirection = Vector3.Transform(movement, orientationMatrix);
                headingDirection *= GameConstants.FishSpeed;
                futurePosition = Position + headingDirection;

                if (Collision.isBarriersValidMove(this, futurePosition, enemies, enemiesAmount, hydroBot) &&
                    Collision.isBarriersValidMove(this, futurePosition, fishes, fishAmount, hydroBot))
                {
                    Position = futurePosition;

                    BoundingSphere updatedSphere;
                    updatedSphere = BoundingSphere;

                    updatedSphere.Center.X = Position.X;
                    updatedSphere.Center.Z = Position.Z;
                    BoundingSphere = new BoundingSphere(updatedSphere.Center,
                        updatedSphere.Radius);

                    stucked = false;
                    //break;
                }
                else
                {
                    stucked = true;
                    futurePosition = prevFuturePosition;
                }

                if (ForwardDirection - lastForwardDir > 0)
                {
                    if (!clipPlayer.inRange(29, 31))
                        clipPlayer.switchRange(29, 31);
                }
                else if (ForwardDirection - lastForwardDir < 0)
                {
                    if (!clipPlayer.inRange(40, 42))
                        clipPlayer.switchRange(40, 42);
                }
                else
                {
                    if (!clipPlayer.inRange(1, 24))
                        clipPlayer.switchRange(1, 24);
                }
            //}
        }

        
    }
}