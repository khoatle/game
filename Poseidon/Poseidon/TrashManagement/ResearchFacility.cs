using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Poseidon.Core;

namespace Poseidon
{
    public class ResearchFacility : GameObject
    {
        public float orientation;

        Game game;

        //Creation time
        double facilityCreationTime;

        //upgrade bools
        public bool bioUpgrade, plasticUpgrade; 

        //For Factory Configuration Screen
        SpriteFont facilityFont, facilityFont2;
        Texture2D background, upgradeButton, playJigsawButton, increaseAttributeButton;
        public Rectangle backgroundRect, bioUpgradeRect, plasticUpgradeRect, playSeaCowJigsawRect, playTurtleJigsawRect, playDolphinJigsawRect, increaseAttributeRect;
        public static bool playSeaCowJigsaw, playTurtleJigsaw, playDolphinJigsaw; // ensure that clicking on the rect when button is not drawn does not start game.
        public static bool seaCowLost, turtleLost, dolphinLost; // To diplay if the user failed to win the jigsaw game (timeout).
        public static bool seaCowWon, turtleWon, dolphinWon; //To display if the user won the jigsaw game.
        public bool mouseOnIncreaseAttributeIcon;

        //Rock Processing
        public List<double> listTimeRockProcessing;

        List<Model> modelStates; // 4 stages of model, the last one is fully constructed one
        public List<Model> ModelStates
        {
            set { modelStates = value; }
        }

        private bool underConstruction;
        public bool UnderConstruction
        {
            get { return underConstruction; }
        }
        private int constructionIndex;                  // 0, 1, 2, 3 states of constructions. When it reaches index 3, underConstruction is false
        private TimeSpan constructionSwitchSpan;        // Timespan to switch construction states, 3 secs
        private TimeSpan lastConstructionSwitchTime;    // Note down last time when construction state switch happened

        Random random;

        public bool sandDirturbedAnimationPlayed;
        ParticleManagement particleManager;
        public static SoundEffectInstance buildingSoundInstance = PoseidonGame.audio.buildingSound.CreateInstance();

        int screenWidth, screenHeight;

        public bool upgradeBotButtonHover = false, upgradeBotButtonPressed = false;
        public bool upgradeBioFacButtonHover = false, upgradeBioFacButtonPressed = false;
        public bool upgradePlasFacButtonHover = false, upgradePlasFacButtonPressed = false;
        public bool dolphinButtonHover = false, dolphinButtonPressed = false;
        public bool seacowButtonHover = false, seacowBotButtonPressed = false;
        public bool turtleButtonHover = false, turtleButtonPressed = false;


        public ResearchFacility(ParticleManagement particleManager)
            : base()
        {
            facilityCreationTime = PoseidonGame.playTime.TotalSeconds;
            random = new Random();
            bioUpgrade = plasticUpgrade = false;
            listTimeRockProcessing = new List<double>();
            underConstruction = true;
            constructionIndex = 0;
            lastConstructionSwitchTime = TimeSpan.Zero;
            constructionSwitchSpan = TimeSpan.FromSeconds(3);
            this.particleManager = particleManager;
            //buildingSoundInstance = PoseidonGame.audio.buildingSound.CreateInstance();
        }

        public void LoadContent(Game game, Vector3 position, float orientation)
        {
            Position = position;
            BoundingSphere = CalculateBoundingSphere();

            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;
            scaledSphere.Radius *= GameConstants.FactoryBoundingSphereFactor;
            BoundingSphere =
                new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);
            Vector3 tempCenter;
            tempCenter = BoundingSphere.Center;
            tempCenter.X = Position.X;
            tempCenter.Y = GameConstants.MainGameFloatHeight;
            tempCenter.Z = Position.Z;
            BoundingSphere = new BoundingSphere(tempCenter, BoundingSphere.Radius);

            this.orientation = orientation;

            this.facilityFont = IngamePresentation.facilityFont;
            this.facilityFont2 = IngamePresentation.facilityFont2;
            this.background = IngamePresentation.facilityBackground;
            this.upgradeButton = IngamePresentation.facilityUpgradeButton;
            this.playJigsawButton = IngamePresentation.factoryProduceButtonNormalTexture;
            this.increaseAttributeButton = IngamePresentation.increaseAttributeButtonNormalTexture;

            int backgroundWidth = (int)(game.Window.ClientBounds.Width * 0.78);
            int backgroundHeight = game.Window.ClientBounds.Height;
            screenWidth = game.Window.ClientBounds.Width;
            screenHeight = game.Window.ClientBounds.Height;
            backgroundRect = new Rectangle(0, 0, game.Window.ClientBounds.Width, game.Window.ClientBounds.Height);
            bioUpgradeRect = new Rectangle(game.Window.ClientBounds.Center.X - 300, backgroundRect.Top + 400, 50, 50);
            plasticUpgradeRect = new Rectangle(game.Window.ClientBounds.Center.X + 100, backgroundRect.Top + 400, 50, 50);
            playSeaCowJigsawRect = new Rectangle(backgroundRect.Center.X - 400, backgroundRect.Bottom - 250, 300, 65);
            playTurtleJigsawRect = new Rectangle(backgroundRect.Center.X - 100, backgroundRect.Bottom - 250, 300, 65);
            playDolphinJigsawRect = new Rectangle(backgroundRect.Center.X + 200, backgroundRect.Bottom - 250, 300, 65);
            increaseAttributeRect = new Rectangle(backgroundRect.Right - 100, backgroundRect.Top+40, 50, 50);

            this.game = game;

            if (modelStates != null)
            {
                foreach (Model model in modelStates)
                {
                    SetupShaderParameters(PoseidonGame.contentManager, model);
                }
                Model = modelStates[constructionIndex];
            }
            else
            {
                // Set up the parameters
                underConstruction = false;
                SetupShaderParameters(PoseidonGame.contentManager, Model);
            }
            EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);

        }

        public void Update(GameTime gameTime, Vector3 botPosition, ref List<Point> points)
        {
            EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);

            if (lastConstructionSwitchTime == TimeSpan.Zero)
            {
                lastConstructionSwitchTime = PoseidonGame.playTime;
            }

            if (underConstruction)
            {

                if (buildingSoundInstance.State == SoundState.Paused)
                    buildingSoundInstance.Resume();
                if (buildingSoundInstance.State != SoundState.Playing)
                {
                    buildingSoundInstance.Play();
                }
                //environment disturbance because of factory building
                if (!sandDirturbedAnimationPlayed)
                {
                    for (int k = 0; k < GameConstants.numSandParticles; k++)
                        particleManager.sandParticlesForFactory.AddParticle(Position, Vector3.Zero);
                    sandDirturbedAnimationPlayed = true;
                }

                Model = modelStates[constructionIndex];
                underConstruction = (constructionIndex < (modelStates.Count - 1));
                if (!underConstruction && buildingSoundInstance.State == SoundState.Playing) buildingSoundInstance.Stop();
                if (PoseidonGame.playTime - lastConstructionSwitchTime >= constructionSwitchSpan)
                {
                    //constructionIndex++;
                    constructionIndex += (int)((PoseidonGame.playTime - lastConstructionSwitchTime).TotalMilliseconds / constructionSwitchSpan.TotalMilliseconds);
                    if (constructionIndex >= modelStates.Count)
                    {
                        constructionIndex = modelStates.Count - 1;
                    }
                    lastConstructionSwitchTime = PoseidonGame.playTime;
                }
                return;
            }

            double processingTime = 12; //3 days
            int fossilType = random.Next(100);
            string point_string = "";
            bool boneFound = false;

            //Is strange rock finished processing?
            for (int i = 0; i < listTimeRockProcessing.Count; i++)
            {
                if (PoseidonGame.playTime.TotalSeconds - listTimeRockProcessing[i] >= processingTime)
                {
                    listTimeRockProcessing.RemoveAt(i);
                    //produce fossil/bone 75% of the times.
                    if (fossilType >= 74)
                    {
                        HydroBot.numTurtlePieces++;
                        point_string = "Research Centre found\na Meiolania Turtle fossil\nfrom the strange rock";
                        boneFound = true;
                    }
                    else if (fossilType >= 49)
                    {
                        HydroBot.numDolphinPieces++;
                        point_string = "Research Centre found\na Maui's Dolphin bone\nfrom the strange rock";
                        boneFound = true;
                    }
                    else if (fossilType >= 24)
                    {
                        HydroBot.numSeaCowPieces++;
                        boneFound = true;
                        point_string = "Research Centre found\na Stellar's SeaCow bone\nfrom the strange rock";
                    }
                }
                if (boneFound)
                {
                    boneFound = false;
                    Point point = new Point();
                    point.LoadContent(PoseidonGame.contentManager, point_string, botPosition, Color.LawnGreen);
                    points.Add(point);
                }
            }
            
        }

        // our custom shader
        Effect newBasicEffect;

        public void SetupShaderParameters(ContentManager content, Model model)
        {
            newBasicEffect = content.Load<Effect>("Shaders/NewBasicEffect");
            EffectHelpers.ChangeEffectUsedByModelToCustomBasicEffect(model, newBasicEffect);
        }

        public void Draw(Matrix view, Matrix projection, Camera gameCamera, string techniqueName)
        {
            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix translateMatrix = Matrix.CreateTranslation(Position);
            Matrix rotationYMatrix = Matrix.CreateRotationY(orientation);
            Matrix worldMatrix = rotationYMatrix * translateMatrix;

            foreach (ModelMesh mesh in Model.Meshes)
            {
                //foreach (BasicEffect effect in mesh.Effects)
                foreach (Effect effect in mesh.Effects)
                {
                    //effect.World =
                    //    worldMatrix * transforms[mesh.ParentBone.Index];
                    //effect.View = view;
                    //effect.Projection = projection;

                    //effect.EnableDefaultLighting();
                    //effect.PreferPerPixelLighting = true;

                    ////effect.DiffuseColor = Color.Green.ToVector3();

                    //effect.FogEnabled = true;
                    //effect.FogStart = GameConstants.FogStart;
                    //effect.FogEnd = GameConstants.FogEnd;
                    //effect.FogColor = GameConstants.FogColor.ToVector3();

                    //for our custom BasicEffect
                    effect.CurrentTechnique = effect.Techniques[techniqueName];
                    Matrix readlWorldMatrix = worldMatrix * transforms[mesh.ParentBone.Index];      
                    //effect.CurrentTechnique = effect.Techniques[techniqueName];
                    effect.Parameters["World"].SetValue(readlWorldMatrix);
                    effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Invert(readlWorldMatrix));
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                    effect.Parameters["EyePosition"].SetValue(new Vector4(gameCamera.AvatarHeadOffset, 0));
                    Matrix WorldView = readlWorldMatrix * view;
                    EffectHelpers.SetFogVector(ref WorldView, GameConstants.FogStart, GameConstants.FogEnd, effect.Parameters["FogVector"]);
                    effect.Parameters["FogColor"].SetValue(fogColor.ToVector3());

                    effect.Parameters["AmbientColor"].SetValue(ambientColor.ToVector4());
                    effect.Parameters["DiffuseColor"].SetValue(diffuseColor.ToVector4());
                    effect.Parameters["SpecularColor"].SetValue(specularColor.ToVector4());


                }
                mesh.Draw();
            }
        }

        public void DrawResearchFacilityConfigurationScene(SpriteBatch spriteBatch, SpriteFont menuSmall)
        {
            if (GameConstants.factoryTextScaleFactor == 0)
            {
                GameConstants.factoryTextScaleFactor = (float)game.Window.ClientBounds.Width / 1280 * (float)game.Window.ClientBounds.Height / 800;
                GameConstants.factoryTextScaleFactor = (float)Math.Sqrt(GameConstants.factoryTextScaleFactor);
                //GameConstants.lineSpacing = (int)(GameConstants.lineSpacing * GameConstants.textScaleFactor);
            }

            float textScaleFactor = GameConstants.factoryTextScaleFactor;
            float lineSpacing = GameConstants.lineSpacing;
                
            //HydroBot.numSeaCowPieces= 20; //REMOVE - JUST fOR TESTING
            float fadeFactor = 0.75f;
            string title = "RESEARCH FACILITY";
            string description = "Welcome to the Research Facility. Processing techniques are analyzed by AI here for processing plant upgradation. State of the art genetic engineering will help you resurrect extinct animals. You can also use experience points to upgrade your own parts here.";
            title = Poseidon.Core.IngamePresentation.wrapLine(title, backgroundRect.Width, facilityFont, 1.5f * textScaleFactor);

            //draw background
            spriteBatch.Draw(background, backgroundRect, Color.White);

            //draw title string
            Vector2 titlePos = new Vector2(backgroundRect.Center.X, backgroundRect.Top + 70 * textScaleFactor);
            spriteBatch.DrawString(facilityFont, title, titlePos, Color.Yellow * fadeFactor, 0, new Vector2(facilityFont.MeasureString(title).X / 2, facilityFont.MeasureString(title).Y / 2), 1.5f * textScaleFactor, SpriteEffects.None, 0);

            //draw description
            description = Poseidon.Core.IngamePresentation.wrapLine(description, backgroundRect.Width - 130, facilityFont, textScaleFactor);
            Vector2 descPos = titlePos + new Vector2(0, facilityFont.MeasureString(title).Y / 2 * 1.5f + lineSpacing + facilityFont.MeasureString(description).Y / 2) * textScaleFactor;
            spriteBatch.DrawString(facilityFont, description, descPos, Color.Purple * fadeFactor, 0, new Vector2(facilityFont.MeasureString(description).X / 2, facilityFont.MeasureString(description).Y / 2), textScaleFactor, SpriteEffects.None, 0);

            string botUpgradeTxt = "HYDROBOT UPGRADATION";
            Vector2 botUpgradeTxtPos = descPos + new Vector2(0, facilityFont.MeasureString(description).Y / 2 + lineSpacing + facilityFont.MeasureString(botUpgradeTxt).Y / 2) * textScaleFactor;
            spriteBatch.DrawString(facilityFont, botUpgradeTxt, botUpgradeTxtPos, Color.LawnGreen * fadeFactor, 0, new Vector2(facilityFont.MeasureString(botUpgradeTxt).X / 2, facilityFont.MeasureString(botUpgradeTxt).Y / 2), textScaleFactor, SpriteEffects.None, 0);
            //Draw Increase Attribute Button
            if (HydroBot.unassignedPts > 0)
            {
                if (upgradeBotButtonPressed) increaseAttributeButton = IngamePresentation.increaseAttributeButtonPressedTexture;
                else if (upgradeBotButtonHover) increaseAttributeButton = IngamePresentation.increaseAttributeButtonHoverTexture;
                else increaseAttributeButton = IngamePresentation.increaseAttributeButtonNormalTexture;
                increaseAttributeRect.X = (int)(botUpgradeTxtPos.X - increaseAttributeRect.Width / 2);
                increaseAttributeRect.Y = (int)(botUpgradeTxtPos.Y + facilityFont.MeasureString(botUpgradeTxt).Y / 2 + lineSpacing/2);
                spriteBatch.Draw(increaseAttributeButton, increaseAttributeRect, Color.White * fadeFactor);
            }
            else
            {
                string noUpgrade = "No upgrade currently available. Accumulate experience points for more upgrades.";
                Vector2 noUpgradePos = botUpgradeTxtPos + new Vector2(0, facilityFont.MeasureString(botUpgradeTxt).Y / 2 + lineSpacing + facilityFont.MeasureString(noUpgrade).Y / 2) * textScaleFactor;
                spriteBatch.DrawString(facilityFont, noUpgrade, noUpgradePos, Color.Red * fadeFactor, 0, new Vector2(facilityFont.MeasureString(noUpgrade).X / 2, facilityFont.MeasureString(noUpgrade).Y / 2), textScaleFactor, SpriteEffects.None, 0);
            }

            //draw factory upgrade heading:
            string upgradeTitle = "PROCESSING PLANT UPGRADATION";
            Vector2 upgradeFacTitlePos = botUpgradeTxtPos + new Vector2(0, facilityFont.MeasureString(description).Y / 2 + 50 + facilityFont.MeasureString(upgradeTitle).Y / 2) * textScaleFactor;
            spriteBatch.DrawString(facilityFont, upgradeTitle, upgradeFacTitlePos, Color.LawnGreen * fadeFactor, 0, new Vector2(facilityFont.MeasureString(upgradeTitle).X / 2, facilityFont.MeasureString(upgradeTitle).Y / 2), textScaleFactor, SpriteEffects.None, 0);

            string bioButtonText, plasticButtonText;
            Vector2 bioButtonPos = Vector2.Zero;

            if ((PoseidonGame.playTime.TotalSeconds - facilityCreationTime) < GameConstants.numDaysForUpgrade * GameConstants.DaysPerSecond)
            {
                bioButtonText = "No upgrades available. Research Facility takes atleast " + GameConstants.numDaysForUpgrade + " days to develop upgrades";
                bioButtonText = Poseidon.Core.IngamePresentation.wrapLine(bioButtonText, backgroundRect.Width-130, facilityFont, textScaleFactor);
                bioButtonPos = upgradeFacTitlePos + new Vector2(0, facilityFont.MeasureString(upgradeTitle).Y / 2 + lineSpacing + facilityFont.MeasureString(bioButtonText).Y / 2) * textScaleFactor;
                spriteBatch.DrawString(facilityFont, bioButtonText, bioButtonPos, Color.Red * fadeFactor, 0, new Vector2(facilityFont.MeasureString(bioButtonText).X / 2, facilityFont.MeasureString(bioButtonText).Y / 2), textScaleFactor, SpriteEffects.None, 0);
            }
            else
            {           
                string button_title_text = "ORGANIC PROCESSING PLANT";
                Vector2 organicUpgradePos = new Vector2();
                organicUpgradePos.X = (float)screenWidth / 3;
                organicUpgradePos.Y = upgradeFacTitlePos.Y + (facilityFont.MeasureString(upgradeTitle).Y / 2 + lineSpacing + facilityFont.MeasureString(button_title_text).Y / 2) * textScaleFactor;
                Vector2 plasticUpgradePos = organicUpgradePos;
                plasticUpgradePos.X = (float)2 * screenWidth / 3;
                spriteBatch.DrawString(facilityFont, button_title_text, organicUpgradePos, Color.White * fadeFactor, 0, new Vector2(facilityFont.MeasureString(button_title_text).X / 2, facilityFont.MeasureString(button_title_text).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                button_title_text = "PLASTIC PROCESSING PLANT";
                spriteBatch.DrawString(facilityFont, button_title_text, plasticUpgradePos, Color.White * fadeFactor, 0, new Vector2(facilityFont.MeasureString(button_title_text).X / 2, facilityFont.MeasureString(button_title_text).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                if (HydroBot.bioPlantLevel == 1 && HydroBot.totalBioTrashProcessed >= GameConstants.numTrashForUpgrade)
                {
                    bioUpgrade = true;
                    bioButtonText = "Upgrade to level 2";
                }
                else if (HydroBot.bioPlantLevel == 2 && HydroBot.totalBioTrashProcessed >= HydroBot.bioPlantLevel * GameConstants.numTrashForUpgrade)
                {
                    bioUpgrade = true;
                    bioButtonText = "Upgrade to level 3";
                }
                else
                {
                    bioUpgrade = false;
                    bioButtonText = "";
                }

                if (HydroBot.plasticPlantLevel == 1 && HydroBot.totalPlasticTrashProcessed >= GameConstants.numTrashForUpgrade)
                {
                    plasticUpgrade = true;
                    plasticButtonText = "Upgrade to level 2";
                }
                else if (HydroBot.plasticPlantLevel == 2 && HydroBot.totalPlasticTrashProcessed >= HydroBot.plasticPlantLevel * GameConstants.numTrashForUpgrade)
                {
                    plasticUpgrade = true;
                    plasticButtonText = "Upgrade to level 3";
                }
                else
                {
                    plasticUpgrade = false;
                    plasticButtonText = "";
                }

                Vector2 bioTextPos = organicUpgradePos + new Vector2(0, facilityFont.MeasureString(button_title_text).Y / 2 + lineSpacing) * textScaleFactor;// + facilityFont.MeasureString(bioButtonText).Y / 2);
                Vector2 plasticTextPos = plasticUpgradePos + new Vector2(0, facilityFont.MeasureString(button_title_text).Y / 2 + lineSpacing) * textScaleFactor;// + facilityFont.MeasureString(plasticButtonText).Y / 2);

                if(bioUpgrade)
                {
                    bioTextPos.Y += facilityFont.MeasureString(bioButtonText).Y / 2 * textScaleFactor;
                    spriteBatch.DrawString(facilityFont, bioButtonText, bioTextPos, Color.Gold * fadeFactor, 0, new Vector2(facilityFont.MeasureString(bioButtonText).X / 2, facilityFont.MeasureString(bioButtonText).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                    //draw bio upgrade buttons
                    bioUpgradeRect.X = (int)(bioTextPos.X - bioUpgradeRect.Width / 2);
                    bioUpgradeRect.Y = (int)(bioTextPos.Y + (facilityFont.MeasureString(bioButtonText).Y / 2 + lineSpacing) * textScaleFactor);
                    if (upgradeBioFacButtonPressed) upgradeButton = IngamePresentation.increaseAttributeButtonPressedTexture;
                    else if (upgradeBioFacButtonHover) upgradeButton = IngamePresentation.increaseAttributeButtonHoverTexture;
                    else upgradeButton = IngamePresentation.increaseAttributeButtonNormalTexture;
                    spriteBatch.Draw(upgradeButton, bioUpgradeRect, Color.White * fadeFactor);                
                }
                else if (HydroBot.bioPlantLevel == 3)
                {
                    bioButtonText = "Congratulations! You've got the best technology.";
                    bioButtonText = Poseidon.Core.IngamePresentation.wrapLine(bioButtonText, backgroundRect.Width / 3, facilityFont2, textScaleFactor);
                    bioTextPos.Y += facilityFont.MeasureString(bioButtonText).Y / 2 * textScaleFactor;
                    spriteBatch.DrawString(facilityFont, bioButtonText, bioTextPos, Color.Gold * fadeFactor, 0, new Vector2(facilityFont.MeasureString(bioButtonText).X / 2, facilityFont.MeasureString(bioButtonText).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                }
                else
                {
                    bioButtonText = "You need to process " + (HydroBot.bioPlantLevel * GameConstants.numTrashForUpgrade - HydroBot.totalBioTrashProcessed).ToString() + " more trash for upgrade to level " + (HydroBot.bioPlantLevel + 1).ToString();
                    bioButtonText = Poseidon.Core.IngamePresentation.wrapLine(bioButtonText, backgroundRect.Width/3, facilityFont2, textScaleFactor);
                    bioTextPos.Y += facilityFont.MeasureString(bioButtonText).Y / 2 * textScaleFactor;
                    spriteBatch.DrawString(facilityFont, bioButtonText, bioTextPos, Color.Red * fadeFactor, 0, new Vector2(facilityFont.MeasureString(bioButtonText).X / 2, facilityFont.MeasureString(bioButtonText).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                }

                if(plasticUpgrade)
                {
                    plasticTextPos.Y += facilityFont.MeasureString(plasticButtonText).Y / 2 * textScaleFactor;
                    spriteBatch.DrawString(facilityFont, plasticButtonText, plasticTextPos, Color.Gold * fadeFactor, 0, new Vector2(facilityFont.MeasureString(plasticButtonText).X / 2, facilityFont.MeasureString(plasticButtonText).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                    plasticUpgradeRect.X = (int)(plasticTextPos.X - plasticUpgradeRect.Width / 2);
                    plasticUpgradeRect.Y = (int)(plasticTextPos.Y + (facilityFont.MeasureString(plasticButtonText).Y / 2 + lineSpacing) * textScaleFactor);      
                    if (upgradePlasFacButtonPressed) upgradeButton = IngamePresentation.increaseAttributeButtonPressedTexture;
                    else if (upgradePlasFacButtonHover) upgradeButton = IngamePresentation.increaseAttributeButtonHoverTexture;
                    else upgradeButton = IngamePresentation.increaseAttributeButtonNormalTexture;
                    spriteBatch.Draw(upgradeButton, plasticUpgradeRect, Color.White * fadeFactor);
                }
                else if (HydroBot.plasticPlantLevel == 3)
                {
                    plasticButtonText = "Congratulations! You've got the best technology.";
                    plasticButtonText = Poseidon.Core.IngamePresentation.wrapLine(plasticButtonText, backgroundRect.Width / 3, facilityFont2, textScaleFactor);
                    plasticTextPos.Y += facilityFont.MeasureString(plasticButtonText).Y / 2 * textScaleFactor;
                    spriteBatch.DrawString(facilityFont, plasticButtonText, plasticTextPos, Color.Gold * fadeFactor, 0, new Vector2(facilityFont.MeasureString(plasticButtonText).X / 2, facilityFont.MeasureString(plasticButtonText).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                }
                else
                {
                    plasticButtonText = "You need to process " + (HydroBot.plasticPlantLevel * GameConstants.numTrashForUpgrade - HydroBot.totalPlasticTrashProcessed).ToString() + " more trash for upgrade to level " + (HydroBot.plasticPlantLevel + 1).ToString();
                    plasticButtonText = Poseidon.Core.IngamePresentation.wrapLine(plasticButtonText, backgroundRect.Width / 3, facilityFont2, textScaleFactor);
                    plasticTextPos.Y += facilityFont.MeasureString(plasticButtonText).Y / 2 * textScaleFactor;
                    spriteBatch.DrawString(facilityFont, plasticButtonText, plasticTextPos, Color.Red * fadeFactor, 0, new Vector2(facilityFont.MeasureString(plasticButtonText).X / 2, facilityFont.MeasureString(plasticButtonText).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                   
                }
            }

            //Draw resurrection title:
            string resurrectTitle = "EXTINCT ANIMAL RESURRECTION";
            Vector2 resurTitlePos = new Vector2(screenWidth / 2, upgradeFacTitlePos.Y + (170 + facilityFont.MeasureString(resurrectTitle).Y / 2) * textScaleFactor);
            spriteBatch.DrawString(facilityFont, resurrectTitle, resurTitlePos, Color.LawnGreen * fadeFactor, 0, new Vector2(facilityFont.MeasureString(resurrectTitle).X / 2, facilityFont.MeasureString(resurrectTitle).Y / 2), textScaleFactor, SpriteEffects.None, 0);

            Vector2 dolphinTextPos = new Vector2(2 * screenWidth / 4, resurTitlePos.Y + lineSpacing * textScaleFactor);
            Vector2 seacowTextPos = new Vector2(screenWidth / 4, resurTitlePos.Y + lineSpacing * textScaleFactor);
            Vector2 turtleTextPos = new Vector2(3 * screenWidth / 4, resurTitlePos.Y + lineSpacing * textScaleFactor);

            //seaCowWon = dolphinWon = turtleWon = true;

            if (seaCowLost)
            {
                string lost_Str = "Sorry, resurrection unsuccessful, the collected fragments can not be reused.";//"SORRY, RESURRECTION TIMED OUT. THE BONES CANNOT BE REUSED.";
                lost_Str = Poseidon.Core.IngamePresentation.wrapLine(lost_Str, screenWidth/4, facilityFont, textScaleFactor);
                seacowTextPos.Y += facilityFont.MeasureString(lost_Str).Y / 2 * textScaleFactor;
                spriteBatch.DrawString(facilityFont, lost_Str, seacowTextPos, Color.Red * fadeFactor, 0, new Vector2(facilityFont.MeasureString(lost_Str).X / 2, facilityFont.MeasureString(lost_Str).Y / 2), textScaleFactor, SpriteEffects.None, 0);
            }
            else if (seaCowWon && HydroBot.seaCowPower > 1f)
            {
                string seacowText = "With more understanding, you have become better friends!";
                seacowText = Poseidon.Core.IngamePresentation.wrapLine(seacowText, screenWidth / 4, facilityFont2, textScaleFactor);
                seacowTextPos.Y += facilityFont.MeasureString(seacowText).Y / 2 * textScaleFactor;
                spriteBatch.DrawString(facilityFont, seacowText, seacowTextPos, Color.Gold * fadeFactor, 0, new Vector2(facilityFont.MeasureString(seacowText).X / 2, facilityFont.MeasureString(seacowText).Y / 2), textScaleFactor, SpriteEffects.None, 0);
            }
            else if (seaCowWon)
            {
                string seacowText = "Congrats! You have successfully resurrected the Stellar's SeaCow. It will join you in your adventure.";
                seacowText = Poseidon.Core.IngamePresentation.wrapLine(seacowText, screenWidth/4, facilityFont2, textScaleFactor);
                seacowTextPos.Y += facilityFont.MeasureString(seacowText).Y / 2 * textScaleFactor;
                spriteBatch.DrawString(facilityFont, seacowText, seacowTextPos, Color.Gold * fadeFactor, 0, new Vector2(facilityFont.MeasureString(seacowText).X / 2, facilityFont.MeasureString(seacowText).Y / 2), textScaleFactor, SpriteEffects.None, 0);
            }
            else if (HydroBot.hasSeaCow)
            {
                if (HydroBot.numSeaCowPieces >= GameConstants.boneCountForSeaCowJigsaw)
                {
                    if (seacowBotButtonPressed) playJigsawButton = IngamePresentation.factoryProduceButtonPressedTexture;
                    else if (seacowButtonHover) playJigsawButton = IngamePresentation.factoryProduceButtonHoverTexture;
                    else playJigsawButton = IngamePresentation.factoryProduceButtonNormalTexture;
                    playSeaCowJigsaw = true;
                    string seacowJigsawButtonText = "Research Stellar's seacow";//"RESURRECT STELLAR'S SEACOW";
                    playSeaCowJigsawRect.Width = (int)(300 * textScaleFactor);
                    playSeaCowJigsawRect.Height = (int)(65 * textScaleFactor);
                    playSeaCowJigsawRect.X = (int)(seacowTextPos.X - playSeaCowJigsawRect.Width / 2);
                    playSeaCowJigsawRect.Y = (int)(seacowTextPos.Y + lineSpacing * textScaleFactor);
                    spriteBatch.Draw(playJigsawButton, playSeaCowJigsawRect, Color.White * fadeFactor);
                    spriteBatch.DrawString(facilityFont, seacowJigsawButtonText, new Vector2(playSeaCowJigsawRect.Center.X, playSeaCowJigsawRect.Center.Y), Color.Gold * fadeFactor, 0, new Vector2(facilityFont.MeasureString(seacowJigsawButtonText).X / 2, facilityFont.MeasureString(seacowJigsawButtonText).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                }
                else
                {
                    string seacowText = "You need " + (GameConstants.boneCountForSeaCowJigsaw - HydroBot.numSeaCowPieces).ToString() + " more bones to research more on Stellar's Seacow";
                    seacowText = Poseidon.Core.IngamePresentation.wrapLine(seacowText, screenWidth / 4, facilityFont2, textScaleFactor);
                    seacowTextPos.Y += facilityFont.MeasureString(seacowText).Y / 2 * textScaleFactor;
                    spriteBatch.DrawString(facilityFont, seacowText, seacowTextPos, Color.Red * fadeFactor, 0, new Vector2(facilityFont.MeasureString(seacowText).X / 2, facilityFont.MeasureString(seacowText).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                }
            }
            else
            {
                if (HydroBot.numSeaCowPieces >= GameConstants.boneCountForSeaCowJigsaw)
                {
                    if (seacowBotButtonPressed) playJigsawButton = IngamePresentation.factoryProduceButtonPressedTexture;
                    else if (seacowButtonHover) playJigsawButton = IngamePresentation.factoryProduceButtonHoverTexture;
                    else playJigsawButton = IngamePresentation.factoryProduceButtonNormalTexture;
                    //Draw Sea Cow Jigsaw Button
                    playSeaCowJigsaw = true;
                    string seacowJigsawButtonText = "Resurrect Stellar's seacow";//"RESURRECT STELLAR'S SEACOW";
                    playSeaCowJigsawRect.Width = (int)(300 * textScaleFactor);
                    playSeaCowJigsawRect.Height = (int)(65 * textScaleFactor);
                    playSeaCowJigsawRect.X = (int)(seacowTextPos.X - playSeaCowJigsawRect.Width / 2);
                    playSeaCowJigsawRect.Y = (int)(seacowTextPos.Y + lineSpacing * textScaleFactor);
                    spriteBatch.Draw(playJigsawButton, playSeaCowJigsawRect, Color.White * fadeFactor);
                    spriteBatch.DrawString(facilityFont, seacowJigsawButtonText, new Vector2(playSeaCowJigsawRect.Center.X, playSeaCowJigsawRect.Center.Y), Color.Gold * fadeFactor, 0, new Vector2(facilityFont.MeasureString(seacowJigsawButtonText).X / 2, facilityFont.MeasureString(seacowJigsawButtonText).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                }
                else if (HydroBot.numSeaCowPieces > 0)
                {
                    string seacowText = "You need " + (GameConstants.boneCountForSeaCowJigsaw - HydroBot.numSeaCowPieces).ToString() + " more bones to resurrect Stellar's Seacow";
                    seacowText = Poseidon.Core.IngamePresentation.wrapLine(seacowText, screenWidth/4, facilityFont2, textScaleFactor);
                    seacowTextPos.Y += facilityFont.MeasureString(seacowText).Y / 2 * textScaleFactor;
                    spriteBatch.DrawString(facilityFont, seacowText, seacowTextPos, Color.Red * fadeFactor, 0, new Vector2(facilityFont.MeasureString(seacowText).X / 2, facilityFont.MeasureString(seacowText).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                }
            }

            if (turtleLost)
            {
                string lost_Str = "Sorry, resurrection unsuccessful, the collected fragments can not be reused.";//"SORRY, RESURRECTION TIMED OUT. THE BONES CANNOT BE REUSED.";
                lost_Str = Poseidon.Core.IngamePresentation.wrapLine(lost_Str, screenWidth / 4, facilityFont, textScaleFactor);
                turtleTextPos.Y += facilityFont.MeasureString(lost_Str).Y / 2 * textScaleFactor;
                spriteBatch.DrawString(facilityFont, lost_Str, turtleTextPos, Color.Red * fadeFactor, 0, new Vector2(facilityFont.MeasureString(lost_Str).X / 2, facilityFont.MeasureString(lost_Str).Y / 2), textScaleFactor, SpriteEffects.None, 0);
            }
            else if (turtleWon && HydroBot.turtlePower>1f)
            {
                string turtleText = "With more understanding, you have become better friends!";
                turtleText = Poseidon.Core.IngamePresentation.wrapLine(turtleText, screenWidth / 4, facilityFont2, textScaleFactor);
                turtleTextPos.Y += facilityFont.MeasureString(turtleText).Y / 2 * textScaleFactor;
                spriteBatch.DrawString(facilityFont, turtleText, turtleTextPos, Color.Gold * fadeFactor, 0, new Vector2(facilityFont.MeasureString(turtleText).X / 2, facilityFont.MeasureString(turtleText).Y / 2), textScaleFactor, SpriteEffects.None, 0);
            }
            else if (turtleWon)
            {
                string turtleText = "Congrats! You have successfully resurrected the legend Meiolania. It will join you in your adventure.";
                turtleText = Poseidon.Core.IngamePresentation.wrapLine(turtleText, screenWidth / 4, facilityFont2, textScaleFactor);
                turtleTextPos.Y += facilityFont.MeasureString(turtleText).Y / 2 * textScaleFactor;
                spriteBatch.DrawString(facilityFont, turtleText, turtleTextPos, Color.Gold * fadeFactor, 0, new Vector2(facilityFont.MeasureString(turtleText).X / 2, facilityFont.MeasureString(turtleText).Y / 2), textScaleFactor, SpriteEffects.None, 0);
            }
            else if (HydroBot.hasTurtle)
            {
                if (HydroBot.numTurtlePieces >= GameConstants.boneCountForTurtleJigsaw)
                {
                    if (turtleButtonPressed) playJigsawButton = IngamePresentation.factoryProduceButtonPressedTexture;
                    else if (turtleButtonHover) playJigsawButton = IngamePresentation.factoryProduceButtonHoverTexture;
                    else playJigsawButton = IngamePresentation.factoryProduceButtonNormalTexture;
                    //Draw Sea Turtle Jigsaw Button
                    playTurtleJigsaw = true;
                    string turtleJigsawButtonText = "Research Meiolania";//"RESURRECT MEIOLANIA\nTURTLE";
                    playTurtleJigsawRect.Width = (int)(300 * textScaleFactor);
                    playTurtleJigsawRect.Height = (int)(65 * textScaleFactor);
                    playTurtleJigsawRect.X = (int)(turtleTextPos.X - playTurtleJigsawRect.Width / 2);
                    playTurtleJigsawRect.Y = (int)(turtleTextPos.Y + lineSpacing * textScaleFactor);
                    spriteBatch.Draw(playJigsawButton, playTurtleJigsawRect, Color.White * fadeFactor);
                    spriteBatch.DrawString(facilityFont, turtleJigsawButtonText, new Vector2(playTurtleJigsawRect.Center.X, playTurtleJigsawRect.Center.Y), Color.Gold * fadeFactor, 0, new Vector2(facilityFont.MeasureString(turtleJigsawButtonText).X / 2, facilityFont.MeasureString(turtleJigsawButtonText).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                }
                else
                {
                    string turtleText = "You need " + (GameConstants.boneCountForTurtleJigsaw - HydroBot.numTurtlePieces).ToString() + " more fossils to research more on Meiolania";
                    turtleText = Poseidon.Core.IngamePresentation.wrapLine(turtleText, screenWidth / 4, facilityFont2, textScaleFactor);
                    turtleTextPos.Y += facilityFont.MeasureString(turtleText).Y / 2 * textScaleFactor;
                    spriteBatch.DrawString(facilityFont, turtleText, turtleTextPos, Color.Red * fadeFactor, 0, new Vector2(facilityFont.MeasureString(turtleText).X / 2, facilityFont.MeasureString(turtleText).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                }
            }
            else
            {
                if (HydroBot.numTurtlePieces >= GameConstants.boneCountForTurtleJigsaw)
                {
                    if (turtleButtonPressed) playJigsawButton = IngamePresentation.factoryProduceButtonPressedTexture;
                    else if (turtleButtonHover) playJigsawButton = IngamePresentation.factoryProduceButtonHoverTexture;
                    else playJigsawButton = IngamePresentation.factoryProduceButtonNormalTexture;
                    //Draw Sea Turtle Jigsaw Button
                    playTurtleJigsaw = true;
                    string turtleJigsawButtonText = "Resurrect Meiolania";//"RESURRECT MEIOLANIA\nTURTLE";
                    playTurtleJigsawRect.Width = (int)(300 * textScaleFactor);
                    playTurtleJigsawRect.Height = (int)(65 * textScaleFactor);
                    playTurtleJigsawRect.X = (int)(turtleTextPos.X - playTurtleJigsawRect.Width / 2);
                    playTurtleJigsawRect.Y = (int)(turtleTextPos.Y + lineSpacing * textScaleFactor);
                    spriteBatch.Draw(playJigsawButton, playTurtleJigsawRect, Color.White * fadeFactor);
                    spriteBatch.DrawString(facilityFont, turtleJigsawButtonText, new Vector2(playTurtleJigsawRect.Center.X, playTurtleJigsawRect.Center.Y), Color.Gold * fadeFactor, 0, new Vector2(facilityFont.MeasureString(turtleJigsawButtonText).X / 2, facilityFont.MeasureString(turtleJigsawButtonText).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                }
                else if (HydroBot.numTurtlePieces > 0)
                {
                    string turtleText = "You need " + (GameConstants.boneCountForTurtleJigsaw - HydroBot.numTurtlePieces).ToString() + " more fossils to resurrect Meiolania";
                    turtleText = Poseidon.Core.IngamePresentation.wrapLine(turtleText, screenWidth/4, facilityFont2, textScaleFactor);
                    turtleTextPos.Y += facilityFont.MeasureString(turtleText).Y / 2 * textScaleFactor;
                    spriteBatch.DrawString(facilityFont, turtleText, turtleTextPos, Color.Red * fadeFactor, 0, new Vector2(facilityFont.MeasureString(turtleText).X / 2, facilityFont.MeasureString(turtleText).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                }
            }

            if (dolphinLost)
            {
                string lost_Str = "Sorry, resurrection unsuccessful, the collected fragments can not be reused.";//"SORRY, RESURRECTION TIMED OUT. THE BONES CANNOT BE REUSED.";
                lost_Str = Poseidon.Core.IngamePresentation.wrapLine(lost_Str, screenWidth / 4, facilityFont, textScaleFactor);
                dolphinTextPos.Y += facilityFont.MeasureString(lost_Str).Y / 2 * textScaleFactor;
                spriteBatch.DrawString(facilityFont, lost_Str, dolphinTextPos, Color.Red * fadeFactor, 0, new Vector2(facilityFont.MeasureString(lost_Str).X / 2, facilityFont.MeasureString(lost_Str).Y / 2), textScaleFactor, SpriteEffects.None, 0);
            }
            else if (dolphinWon && HydroBot.dolphinPower>1f)
            {
                string dolphinText = "With more understanding, you have become better friends!";
                dolphinText = Poseidon.Core.IngamePresentation.wrapLine(dolphinText, screenWidth / 4, facilityFont2, textScaleFactor);
                dolphinTextPos.Y += facilityFont.MeasureString(dolphinText).Y / 2 * textScaleFactor;
                spriteBatch.DrawString(facilityFont, dolphinText, dolphinTextPos, Color.Gold * fadeFactor, 0, new Vector2(facilityFont.MeasureString(dolphinText).X / 2, facilityFont.MeasureString(dolphinText).Y / 2), textScaleFactor, SpriteEffects.None, 0);
            }
            else if (dolphinWon)
            {
                string dolphinText = "Congrats! You have successfully resurrected the Maui's dolphin. It will join you in your adventure.";
                dolphinText = Poseidon.Core.IngamePresentation.wrapLine(dolphinText, screenWidth / 4, facilityFont2, textScaleFactor);
                dolphinTextPos.Y += facilityFont.MeasureString(dolphinText).Y / 2 * textScaleFactor;
                spriteBatch.DrawString(facilityFont, dolphinText, dolphinTextPos, Color.Gold * fadeFactor, 0, new Vector2(facilityFont.MeasureString(dolphinText).X / 2, facilityFont.MeasureString(dolphinText).Y / 2), textScaleFactor, SpriteEffects.None, 0);
            }
            else if (HydroBot.hasDolphin)
            {
                if (HydroBot.numDolphinPieces >= GameConstants.boneCountForDolphinJigsaw)
                {
                    if (dolphinButtonPressed) playJigsawButton = IngamePresentation.factoryProduceButtonPressedTexture;
                    else if (dolphinButtonHover) playJigsawButton = IngamePresentation.factoryProduceButtonHoverTexture;
                    else playJigsawButton = IngamePresentation.factoryProduceButtonNormalTexture;
                    //Draw Sea Dolphin Jigsaw Button
                    playDolphinJigsaw = true;
                    string dolphinJigsawButtonText = "Research Maui's dolphin";//"RESURRECT MAUI'S\nDOLPHIN";
                    playDolphinJigsawRect.Width = (int)(300 * textScaleFactor);
                    playDolphinJigsawRect.Height = (int)(65 * textScaleFactor);
                    playDolphinJigsawRect.X = (int)(dolphinTextPos.X - playDolphinJigsawRect.Width / 2);
                    playDolphinJigsawRect.Y = (int)(dolphinTextPos.Y + lineSpacing * textScaleFactor);
                    spriteBatch.Draw(playJigsawButton, playDolphinJigsawRect, Color.White * fadeFactor);
                    spriteBatch.DrawString(facilityFont, dolphinJigsawButtonText, new Vector2(playDolphinJigsawRect.Center.X, playDolphinJigsawRect.Center.Y), Color.Gold * fadeFactor, 0, new Vector2(facilityFont.MeasureString(dolphinJigsawButtonText).X / 2, facilityFont.MeasureString(dolphinJigsawButtonText).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                }
                else
                {
                    string dolphinText = "You need " + (GameConstants.boneCountForDolphinJigsaw - HydroBot.numDolphinPieces).ToString() + " more bones to research more on Maui's Dolphin";
                    dolphinText = Poseidon.Core.IngamePresentation.wrapLine(dolphinText, screenWidth / 4, facilityFont2, textScaleFactor);
                    dolphinTextPos.Y += facilityFont.MeasureString(dolphinText).Y / 2 * textScaleFactor;
                    spriteBatch.DrawString(facilityFont, dolphinText, dolphinTextPos, Color.Red * fadeFactor, 0, new Vector2(facilityFont.MeasureString(dolphinText).X / 2, facilityFont.MeasureString(dolphinText).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                }
            }
            else
            {
                if (HydroBot.numDolphinPieces >= GameConstants.boneCountForDolphinJigsaw)
                {
                    if (dolphinButtonPressed) playJigsawButton = IngamePresentation.factoryProduceButtonPressedTexture;
                    else if (dolphinButtonHover) playJigsawButton = IngamePresentation.factoryProduceButtonHoverTexture;
                    else playJigsawButton = IngamePresentation.factoryProduceButtonNormalTexture;
                    //Draw Sea Dolphin Jigsaw Button
                    playDolphinJigsaw = true;
                    string dolphinJigsawButtonText = "Resurrect Maui's dolphin";//"RESURRECT MAUI'S\nDOLPHIN";
                    playDolphinJigsawRect.Width = (int)(300 * textScaleFactor);
                    playDolphinJigsawRect.Height = (int)(65 * textScaleFactor);
                    playDolphinJigsawRect.X = (int)(dolphinTextPos.X - playDolphinJigsawRect.Width / 2);
                    playDolphinJigsawRect.Y = (int)(dolphinTextPos.Y + lineSpacing * textScaleFactor);
                    spriteBatch.Draw(playJigsawButton, playDolphinJigsawRect, Color.White * fadeFactor);
                    spriteBatch.DrawString(facilityFont, dolphinJigsawButtonText, new Vector2(playDolphinJigsawRect.Center.X, playDolphinJigsawRect.Center.Y), Color.Gold * fadeFactor, 0, new Vector2(facilityFont.MeasureString(dolphinJigsawButtonText).X / 2, facilityFont.MeasureString(dolphinJigsawButtonText).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                }
                else if (HydroBot.numDolphinPieces > 0)
                {
                    string dolphinText = "You need " + (GameConstants.boneCountForDolphinJigsaw - HydroBot.numDolphinPieces).ToString() + " more bones to resurrect Maui's Dolphin";
                    dolphinText = Poseidon.Core.IngamePresentation.wrapLine(dolphinText, screenWidth / 4, facilityFont2, textScaleFactor);
                    dolphinTextPos.Y += facilityFont.MeasureString(dolphinText).Y / 2 * textScaleFactor;
                    spriteBatch.DrawString(facilityFont, dolphinText, dolphinTextPos, Color.Red * fadeFactor, 0, new Vector2(facilityFont.MeasureString(dolphinText).X / 2, facilityFont.MeasureString(dolphinText).Y / 2), textScaleFactor, SpriteEffects.None, 0);
                }
            }
            if (!HydroBot.hasSeaCow && !HydroBot.hasTurtle && !HydroBot.hasDolphin && HydroBot.numSeaCowPieces == 0 && HydroBot.numDolphinPieces == 0 && HydroBot.numTurtlePieces == 0)
            {
                string txt = "Collect bones/fossils to resurrect extinct sea creatures. Bones and fossils are found while processing trash in the factories. They are also found in treasure chests inside shipwrecks.";
                txt = Poseidon.Core.IngamePresentation.wrapLine(txt, backgroundRect.Width - 130, facilityFont2, textScaleFactor);
                spriteBatch.DrawString(facilityFont2, txt, new Vector2(screenWidth / 2, resurTitlePos.Y + lineSpacing + facilityFont.MeasureString(resurrectTitle).Y / 2 + facilityFont.MeasureString(txt).Y / 2), Color.Purple, 0, new Vector2(facilityFont.MeasureString(txt).X / 2, facilityFont.MeasureString(txt).Y / 2), textScaleFactor, SpriteEffects.None, 0);
            }

            //string nextText = "Press Enter to continue";
            //Vector2 nextTextPosition = new Vector2(backgroundRect.Right - (menuSmall.MeasureString(nextText).X/2 - 50) * textScaleFactor, backgroundRect.Bottom - (menuSmall.MeasureString(nextText).Y/2 - 30)*textScaleFactor);
            //spriteBatch.DrawString(menuSmall, nextText, nextTextPosition, Color.White * fadeFactor, 0, new Vector2(menuSmall.MeasureString(nextText).X / 2, menuSmall.MeasureString(nextText).Y / 2), textScaleFactor, SpriteEffects.None, 0);
        }

        public void UpgradeBioFactory(List<Factory> factories)
        {
            HydroBot.bioPlantLevel++;
            HydroBot.totalBioTrashProcessed -= GameConstants.numTrashForUpgrade;
            bioUpgrade = false;
            foreach (Factory factory in factories)
            {
                if (factory.factoryType == FactoryType.biodegradable)
                    factory.SetUpgradeLevelDependentVariables();
            }
        }

        public void UpgradePlasticFactory(List<Factory> factories)
        {
            HydroBot.plasticPlantLevel++;
            HydroBot.totalPlasticTrashProcessed -= GameConstants.numTrashForUpgrade;
            plasticUpgrade = false;
            foreach (Factory factory in factories)
            {
                if (factory.factoryType == FactoryType.plastic)
                    factory.SetUpgradeLevelDependentVariables();
            }
        }

    }
}
