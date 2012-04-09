using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SkinnedModel;
using Poseidon.Core;


namespace Poseidon
{
    public class BaseEnemy : SwimmingObject
    {
        // For the animation
        protected Matrix[] bones;
        protected SkinningData skd;
        protected ClipPlayer clipPlayer;
        protected Matrix enemyMatrix;
        protected Quaternion qRotation = Quaternion.Identity;

        // Fleeing stuff
        public bool isFleeing;
        public TimeSpan fleeingStart;
        public TimeSpan fleeingDuration;
        public Vector3 fleeingDirection;

        // Percept ID:
        // 0 = nothing detecteds
        // 1 = hydrobot detected - 1st priory
        // 2 = last target is still in range - 2nd priory
        // 3 = a fish is detected - 3rd priory
        public int[] perceptID;
        public bool[] configBits;

        // What direction I'm facing. This will
        // be calculated using ForwardDirection
        public Vector3 headingDirection;

        // Stunned bit and the timestamp since the last stun
        public bool stunned;
        public double stunnedStartTime;

        // Time stampt since the robot starts chasing
        public TimeSpan startChasingTime;
        public TimeSpan prevFire;
        protected float timeBetweenFire;

        // Give up chasing
        public TimeSpan giveupTime;

        // Detection range of the enemy
        public float perceptionRadius;

        public GameObject currentHuntingTarget;

        protected float speed;
        public int damage;

        public bool isHypnotise;
        protected TimeSpan startHypnotiseTime;
        public bool justBeingShot = false;

        //is this enemy released from the submarine?
        public bool releasedFromSubmarine = false;

        public float lastForwardDirection = 0;

        float turnAmount = 0;

        public virtual void Load(int clipStart, int clipEnd, int fps)
        {
            
            skd = Model.Tag as SkinningData;
            clipPlayer = new ClipPlayer(skd, fps);//ClipPlayer running at 24 frames/sec
            AnimationClip clip = skd.AnimationClips["Take 001"]; //Take name from the dude.fbx file
            clipPlayer.play(clip, clipStart, clipEnd, true);
            enemyMatrix = Matrix.CreateScale(0.1f) * Matrix.CreateRotationY((float)MathHelper.Pi * 2) *
                               Matrix.CreateTranslation(Position);
            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;
            float scale = 1.0f;
            if (Name.Contains("Shooting Enemy")) scale = 0.05f;
            if (Name.Contains("Combat Enemy")) scale = 0.06f;
            scaledSphere.Radius *= scale;
            BoundingSphere =
                new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);
            //isBigBoss = true;
            //random = new Random();
            //health = 1000;
            //maxHealth = 1000;
            //perceptionRadius = GameConstants.BossPerceptionRadius;
            //experienceReward = 400; //3000

            // Set up the parameters
            SetupShaderParameters(PoseidonGame.contentManager, Model);         
        }

        public BaseEnemy()
            : base()
        {
            fleeingStart = PoseidonGame.playTime;
            fleeingDuration = new TimeSpan(0, 0, 3);
            isFleeing = false;
            fleeingDirection = new Vector3();

            giveupTime = new TimeSpan(0, 0, 3);
            perceptionRadius = GameConstants.EnemyPerceptionRadius;// *(HydroBot.gamePlusLevel + 1);
            timeBetweenFire = 1.0f;
            stunned = false;
            prevFire = new TimeSpan();
            speed = GameConstants.EnemySpeed;
            health = GameConstants.DefaultEnemyHP;
            basicExperienceReward = 60;
            if (PoseidonGame.gamePlus)
            {
                speed *= (1.0f + HydroBot.gamePlusLevel / 2);
                health *= (HydroBot.gamePlusLevel + 1); //+1 as it starts from 0
                basicExperienceReward *= (HydroBot.gamePlusLevel + 1);
            }

            maxHealth = health;
            damage = GameConstants.DefaultEnemyDamage; //overwritten later
        }

        // Flee
        public void flee(SwimmingObject[] enemyList, int enemySize, SwimmingObject[] fishList, int fishSize, HydroBot hydroBot)
        {
            Vector2 tmp = new Vector2(fleeingDirection.X, fleeingDirection.Z);
            tmp.Normalize();
            tmp *= 100f; // Somewhere faraway

            Vector3 fleeTarget = Position + new Vector3(tmp.X, 0, tmp.Y);
            seekDestination(fleeTarget, enemyList, enemySize, fishList, fishSize, hydroBot, speedFactor);
        }

        public void setHypnotise()
        {
            isHypnotise = true;
            currentHuntingTarget = null;
            startHypnotiseTime = PoseidonGame.playTime;
            justBeingShot = false;
        }

        public void wearOutHypnotise()
        {
            isHypnotise = false;
            currentHuntingTarget = null;
            justBeingShot = false;
        }

        // Is it the time to forget about old target?
        protected bool clearMind(GameTime gameTime)
        {
            if (startChasingTime.TotalSeconds == 0 ||
                PoseidonGame.playTime.TotalSeconds - startChasingTime.TotalSeconds > giveupTime.TotalSeconds)
            {
                currentHuntingTarget = null;
                startChasingTime = PoseidonGame.playTime;
                justBeingShot = false;
                return true;
            }
            return false;
        }

        // Calculate the facing vector
        protected void calculateHeadingDirection()
        {
            ForwardDirection = HydroBot.CalculateAngle(currentHuntingTarget.Position, Position);
            headingDirection = currentHuntingTarget.Position - Position;
            headingDirection.Normalize();
        }

        // Go straight
        protected virtual void goStraight(SwimmingObject[] enemies, int enemiesAmount, SwimmingObject[] fishes, int fishAmount, HydroBot hydroBot)
        {
            //Vector3 futurePosition = Position + speed * headingDirection;
            //if (Collision.isBarriersValidMove(this, futurePosition, enemies, enemiesAmount, hydroBot)
            //        && Collision.isBarriersValidMove(this, futurePosition, fishes, fishAmount, hydroBot))
            //{
            //    Position = futurePosition;
            //    //BoundingSphere.Center = Position;
            //    BoundingSphere.Center.X += speed * headingDirection.X;
            //    BoundingSphere.Center.Z += speed * headingDirection.Z;
            //}
            bool isCombatEnemy = (currentHuntingTarget.GetType().Name.Equals("CombatEnemy"))? true : false;

            float pullDistance = Vector3.Distance(currentHuntingTarget.Position, Position);
            float timeFactor = (isCombatEnemy)? 1.25f:1f;
            Vector3 futurePosition;

            if (pullDistance > (BoundingSphere.Radius + currentHuntingTarget.BoundingSphere.Radius) * timeFactor)
            {
                Vector3 pull = (currentHuntingTarget.Position - Position) * (1 / pullDistance);
                Vector3 totalPush = Vector3.Zero;

                int contenders = 0;
                for (int i = 0; i < enemiesAmount; i++)
                {
                    if (enemies[i] != this)
                    {
                        Vector3 push = Position - enemies[i].Position;

                        float distance = (Vector3.Distance(Position, enemies[i].Position)) - enemies[i].BoundingSphere.Radius;
                        if (distance < BoundingSphere.Radius * 5)
                        {
                            contenders++;
                            if (distance < 0.0001f) // prevent divide by 0 
                            {
                                distance = 0.0001f;
                            }
                            float weight = 1.1f / distance;
                            // push away from big boss 
                            //if (enemies[i].isBigBoss) {
                            //    weight *= 1.5f;
                            //}
                            // If we stuck last time and this enemy is close by, get away from it
                            if (((BaseEnemy)enemies[i]).currentHuntingTarget == currentHuntingTarget && distance < BoundingSphere.Radius * 3) {
                                push *= 2;
                            }
                            totalPush += push * weight;
                        }
                    }
                }

                for (int i = 0; i < fishAmount; i++)
                {
                    if (fishes[i] != currentHuntingTarget)
                    {
                        Vector3 push = Position - fishes[i].Position;

                        float distance = (Vector3.Distance(Position, fishes[i].Position) - fishes[i].BoundingSphere.Radius) - BoundingSphere.Radius;
                        if (distance < BoundingSphere.Radius * 4)
                        {
                            contenders++;
                            if (distance < 0.0001f) // prevent divide by 0 
                            {
                                distance = 0.0001f;
                            }
                            float weight = 1 / distance;
                            totalPush += push * weight;
                        }
                    }
                }

                pull *= Math.Max(1, 4 * contenders);
                pull += totalPush;
                pull.Normalize();

                futurePosition = Position + (pull * speed * speedFactor);

                if (Collision.isBarriersValidMove(this, futurePosition, enemies, enemiesAmount, hydroBot)
                        && Collision.isBarriersValidMove(this, futurePosition, fishes, fishAmount, hydroBot))
                {
                    Position = futurePosition;
                    BoundingSphere.Center.X += (pull * speed * speedFactor).X;
                    BoundingSphere.Center.Z += (pull * speed * speedFactor).Z;
                    lastForwardDirection = ForwardDirection;
                    ForwardDirection = (float)Math.Atan2(pull.X, pull.Z);

                    if (Math.Abs(lastForwardDirection - ForwardDirection) > 45f) {
                        ForwardDirection = (lastForwardDirection + ForwardDirection) / 2 ;
                    }
                }
            }
        }

        public virtual void Update(SwimmingObject[] enemyList, ref int enemySize, SwimmingObject[] fishList, int fishSize, int changeDirection, HydroBot hydroBot, List<DamageBullet> enemyBullets, List<DamageBullet> alliesBullets, BoundingFrustum cameraFrustum, GameTime gameTime, GameMode gameMode)
        {
        }
        public virtual void ChangeBoundingSphere()
        { }

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
                // just return for now.. Some of the fishes do not have animation, so clipPlayer won't be initialized for them
                base.Draw(view, projection);
                return;
            }

            bones = clipPlayer.GetSkinTransforms();

            foreach (ModelMesh mesh in Model.Meshes)
            {
                //foreach (SkinnedEffect effect in mesh.Effects)
                foreach (Effect effect in mesh.Effects)
                {

                    //effect.SetBoneTransforms(bones);
                    //effect.View = view;
                    //effect.Projection = projection;
                    //if (isHypnotise)
                    //{
                    //    effect.DiffuseColor = Color.Red.ToVector3();
                    //}
                    //else
                    //    effect.DiffuseColor = Color.White.ToVector3();
                    effect.Parameters["AmbientColor"].SetValue(ambientColor.ToVector4());
                    effect.Parameters["DiffuseColor"].SetValue(diffuseColor.ToVector4());
                    effect.Parameters["SpecularColor"].SetValue(specularColor.ToVector4());
                    if (HydroBot.gameMode == GameMode.ShipWreck)
                    {
                        effect.Parameters["DiffuseIntensity"].SetValue(0.65f);
                        effect.Parameters["AmbientIntensity"].SetValue(0.65f);
                    }
                    else effect.Parameters["DiffuseIntensity"].SetValue(1.0f);

                    if (isHypnotise == true)
                    {
                        effect.Parameters["DiffuseColor"].SetValue(new Vector4(Color.Red.ToVector3(), 1));
                        //because somehow the red color is very hard to see
                        effect.Parameters["DiffuseIntensity"].SetValue(3.0f);
                    }
                    else
                    {
                        effect.Parameters["DiffuseColor"].SetValue(new Vector4(Vector3.One, 1));
                    }
                    //being freezed by turtle's frozen breath
                    if (speedFactor < 1.0f)
                    {
                        effect.Parameters["DiffuseColor"].SetValue(new Vector4(Color.Blue.ToVector3(), 1));
                    }

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
                    EffectHelpers.SetFogVector(ref WorldView, GameConstants.FogStart, GameConstants.FogEnd, effect.Parameters["FogVector"]);
                    effect.Parameters["FogColor"].SetValue(fogColor.ToVector3());

                }
                mesh.Draw();
            }
            //Matrix[] transforms = new Matrix[Model.Bones.Count];
            //Model.CopyAbsoluteBoneTransformsTo(transforms);
            ////Matrix translateMatrix = Matrix.CreateTranslation(Position);
            ////Matrix worldMatrix = translateMatrix;
            //Matrix worldMatrix = Matrix.Identity;
            //Matrix rotationYMatrix = Matrix.CreateRotationY(ForwardDirection);
            //Matrix translateMatrix = Matrix.CreateTranslation(Position);
            //worldMatrix = rotationYMatrix * translateMatrix;

            //foreach (ModelMesh mesh in Model.Meshes)
            //{
            //    foreach (BasicEffect effect in mesh.Effects)
            //    {
            //        effect.World =
            //            worldMatrix * transforms[mesh.ParentBone.Index];
            //        effect.View = view;
            //        effect.Projection = projection;
            //        if (isHypnotise)
            //        {
            //            effect.DiffuseColor = Color.Black.ToVector3();
            //        }
            //        else
            //            effect.DiffuseColor = Color.White.ToVector3();

            //        effect.EnableDefaultLighting();
            //        effect.PreferPerPixelLighting = true;

            //        effect.FogEnabled = true;
            //        effect.FogStart = GameConstants.FogStart;
            //        effect.FogEnd = GameConstants.FogEnd;
            //        effect.FogColor = GameConstants.FogColor.ToVector3();
            //    }
            //    mesh.Draw();
            //}
        }

        // Go randomly is default move
        protected void randomWalk(int changeDirection, SwimmingObject[] enemies, int enemiesAmount, SwimmingObject[] fishes, int fishAmount, HydroBot hydroBot, float speedFactor)
        {
            float lastForwardDir = lastForwardDirection = ForwardDirection;
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

            float prevForwardDir = ForwardDirection;
            Vector3 prevFuturePosition = futurePosition;
            // try upto 10 times to change direction is there is collision
            //for (int i = 0; i < 4; i++)
            //{
                ForwardDirection += turnAmount * GameConstants.TurnSpeed;
                Vector3 headingDirection = Vector3.Zero;
                headingDirection.X = (float)Math.Sin(ForwardDirection);
                headingDirection.Z = (float)Math.Cos(ForwardDirection);
                headingDirection.Normalize();

                headingDirection *= GameConstants.EnemySpeed * speedFactor;
                futurePosition = Position + headingDirection;

                if (Collision.isBarriersValidMove(this, futurePosition, enemies, enemiesAmount, hydroBot)
                    && Collision.isBarriersValidMove(this, futurePosition, fishes, fishAmount, hydroBot))
                {
                    Position = futurePosition;

                    BoundingSphere updatedSphere;
                    updatedSphere = BoundingSphere;

                    updatedSphere.Center.X += headingDirection.X;//Position.X;
                    updatedSphere.Center.Z += headingDirection.Z;// Position.Z;
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
            //}
        }
    }
}