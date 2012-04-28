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
    public class Factory : GameObject
    {
        public FactoryType factoryType;

        GraphicsDevice graphicsDevice;

        public float orientation;

        Game game;

        public int numTrashWaiting;
        public List<double> listTimeTrashProcessing;

        //Dependent of upgrade level
        int trashBlockSize;
        int processingTime;

        int numRadioActiveProducts = 5;
        int delayRadioActiveProducts = 4; //4sec,1day

        // To decide animation state of some part
        List<Texture2D> animationTextures;
        Texture2D currentPartTexture;
        private int currentTextureIndex;
        private int partId;
        private TimeSpan cycleTime;
        private TimeSpan lastCycledTime;

        // Textures used for factory levels
        private List<Texture2D> levelTextures;
        public List<Texture2D> LevelTextures
        {
            set { levelTextures = value; }
        }
        private Texture2D currentLevelTexture;
        private int levelPartId;

        //For Factory Configuration Screen
        SpriteFont factoryFont;
        Texture2D background, produceButton;
        public Rectangle backgroundRect, produceRect;
        public bool produceButtonHover = false, produceButtonPress = false;
        enum Produce { resource, powerpack };
        Produce produce;
        static Produce defaultChoice = Produce.resource;
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
        Vector3 currentFogColor;

        public bool sandDirturbedAnimationPlayed;
        ParticleManagement particleManager;
        public static SoundEffectInstance buildingSoundInstance = PoseidonGame.audio.buildingSound.CreateInstance();


        public Factory(FactoryType factorytype, ParticleManagement particleManager, GraphicsDevice graphicsDevice)
            : base()
        {
            this.factoryType = factorytype;
            this.graphicsDevice = graphicsDevice;
            numTrashWaiting = 0;
            listTimeTrashProcessing = new List<double>();
            random = new Random();
            partId = -1; // Logic to Draw animated sprites depends on positive partID
            cycleTime = TimeSpan.FromMilliseconds(500.0);
            lastCycledTime = TimeSpan.Zero;
            currentTextureIndex = 0;
            currentFogColor = Vector3.One;

            // building construction state management
            underConstruction = true;
            constructionIndex = 0;
            lastConstructionSwitchTime = TimeSpan.Zero;
            constructionSwitchSpan = TimeSpan.FromSeconds(1);
            this.particleManager = particleManager;
            //buildingSoundInstance = PoseidonGame.audio.buildingSound.CreateInstance();

            //for animating trash processing
            switch (factoryType)
            {
                // These part id's were found out by experimentation. If we happen to change the model in future, 
                // I'll need to find these part ids again. So, I have made them configurable from GameConstant
                case FactoryType.biodegradable:
                    partId = GameConstants.biofactoryPartId;
                    break;
                case FactoryType.radioactive:
                    partId = GameConstants.nuclearfactoryPartId;
                    break;
                case FactoryType.plastic:
                    partId = GameConstants.plasticfactoryPartId;
                    break;
            }
        }

        public void setIsAnchor()
        {
            underConstruction = false;
        }

        public void LoadContent(Game game, Vector3 position, float orientation,ref SpriteFont font,ref Texture2D backgroundTexture, List<Texture2D> animationTextures)
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
            BoundingSphere = new BoundingSphere(tempCenter,BoundingSphere.Radius);
            this.orientation = orientation;
            this.animationTextures = animationTextures;
            if (animationTextures != null)
            {
                currentPartTexture = animationTextures[0];
            }

            produce = defaultChoice;
            factoryFont = font;
            background = backgroundTexture;

            int rectWidth = (int)(graphicsDevice.Viewport.TitleSafeArea.Width);
            int rectHeight = (int)(graphicsDevice.Viewport.TitleSafeArea.Height);
            backgroundRect = new Rectangle(graphicsDevice.Viewport.TitleSafeArea.Center.X - rectWidth / 2, graphicsDevice.Viewport.TitleSafeArea.Center.Y - rectHeight / 2, rectWidth, rectHeight);
            produceRect = new Rectangle(backgroundRect.Center.X - 150, backgroundRect.Top + 120, 300, 65);

            SetUpgradeLevelDependentVariables();

            this.game = game;
            // use construction state
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
                //if (orientation > 50) floatUp = true;
                //else floatUp = false;
                // Set up the parameters
                underConstruction = false;
                SetupShaderParameters(PoseidonGame.contentManager, Model);
            }
            //EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);
        }

        public void Update(GameTime gameTime, ref List<Powerpack> powerpacks,ref List<Resource> resources, ref Model[] powerpackModels, ref Model resourceModel, ref Model[] strangeRockModels)
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

            updateFactoryLevelTexture();

            //Is trash finished processing?
            for (int i = 0; i < listTimeTrashProcessing.Count; i++)
            {
                if (PoseidonGame.playTime.TotalSeconds - listTimeTrashProcessing[i] >= processingTime)
                {
                    listTimeTrashProcessing.RemoveAt(i);
                    if (produce == Produce.powerpack)
                    {
                        producePowerPacks(ref powerpacks, resources, ref powerpackModels);
                    }
                    else
                    {
                        ProduceResource(ref resources, powerpacks, ref resourceModel);
                    }

                    //Produce strange rock
                    if (random.Next(100) < 15) //15% probability
                    {
                        ProduceStrangeRock(ref powerpacks, resources, ref strangeRockModels);
                    }

                    if (factoryType == FactoryType.biodegradable)
                        HydroBot.totalBioTrashProcessed += trashBlockSize;
                    else if (factoryType == FactoryType.plastic)
                        HydroBot.totalPlasticTrashProcessed += trashBlockSize;
                    else
                        HydroBot.totalNuclearTrashProcessed += trashBlockSize;
                    break;
                }
            }

            //Is new trash ready to be processed?
            int numNewTrashBlock = numTrashWaiting / trashBlockSize;
            if (numNewTrashBlock > 0)
            {
                numTrashWaiting = numTrashWaiting % trashBlockSize;
                for (int i = 0; i < numNewTrashBlock; i++)
                {
                    listTimeTrashProcessing.Add(PoseidonGame.playTime.TotalSeconds);
                    if (factoryType == FactoryType.radioactive)
                    {
                        for (int j = 1; j < numRadioActiveProducts; j++)
                            listTimeTrashProcessing.Add(PoseidonGame.playTime.TotalSeconds + j*delayRadioActiveProducts);
                    }
                }
            }

            // If in a processing state, then update parameters for annimation cycles
            updateCycleTextures(gameTime);
        }

        private void updateFactoryLevelTexture()
        {
            int levelIndex = 0;
            if (levelTextures != null)
            {
                switch (factoryType)
                {
                    case FactoryType.biodegradable:
                        levelIndex = HydroBot.bioPlantLevel - 1;
                        levelPartId = GameConstants.biofactoryLevelPartId;
                        break;
                    case FactoryType.plastic:
                        levelIndex = HydroBot.plasticPlantLevel - 1;
                        levelPartId = GameConstants.plasticfactoryLevelPartId;
                        break;
                }
                currentLevelTexture = levelTextures[levelIndex];
            }
            else
            {
                levelPartId = -1;
                currentLevelTexture = null;
            }
        }

        private void updateCycleTextures(GameTime gameTime)
        {
            if (listTimeTrashProcessing.Count > 0)
            {
                // see if timespan for change has occurred
                
                if (gameTime.TotalGameTime - lastCycledTime > cycleTime)
                {
                    currentTextureIndex++;
                    if (currentTextureIndex >= animationTextures.Count)
                    {
                        currentTextureIndex = 0;
                    }
                    currentFogColor = (currentFogColor == Vector3.One) ? Vector3.Zero : Vector3.One;
                    lastCycledTime = gameTime.TotalGameTime;
                }
            }
            else
            {
                currentTextureIndex = 0;    // texture index set down to zero
            }
            currentPartTexture = animationTextures[currentTextureIndex];
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
                int effectId = 0;
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
                    //effect.FogEnd =  GameConstants.FogEnd;
                    //effect.FogColor = GameConstants.FogColor.ToVector3();

                    //for our custom BasicEffect
                    effect.Parameters["AmbientColor"].SetValue(ambientColor.ToVector4());
                    effect.Parameters["DiffuseColor"].SetValue(diffuseColor.ToVector4());
                    effect.Parameters["SpecularColor"].SetValue(specularColor.ToVector4());
                    Matrix readlWorldMatrix = worldMatrix * transforms[mesh.ParentBone.Index];
                    effect.CurrentTechnique = effect.Techniques[techniqueName];
                    effect.Parameters["World"].SetValue(readlWorldMatrix);
                    effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Invert(readlWorldMatrix));
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                    effect.Parameters["EyePosition"].SetValue(new Vector4(gameCamera.AvatarHeadOffset, 0));
                    Matrix WorldView = readlWorldMatrix * view;
                    EffectHelpers.SetFogVector(ref WorldView, GameConstants.FogStart, GameConstants.FogEnd, effect.Parameters["FogVector"]);
                    effect.Parameters["FogColor"].SetValue(fogColor.ToVector3());
                    if (partId == effectId)
                    {
                        //if (listTimeTrashProcessing.Count > 0)
                            effect.Parameters["Texture"].SetValue(currentPartTexture); // If execution enters this block, currentPartTexture has been already put in place in Update function. Just need to find a way to set texture here.
                        //effect.Parameters["FogColor"].SetValue(currentFogColor);
                        //else
                        //    effect.Parameters["Texture"].SetValue(animationTextures[0]);
                    }
                    // set texture for factory level if any
                    if (levelPartId >= 0 && levelPartId == effectId && currentLevelTexture != null)
                    {
                        effect.Parameters["Texture"].SetValue(currentLevelTexture);
                    }
                    effectId++;
                }
                mesh.Draw();
            }
        }

        public void DrawFactoryConfigurationScene(SpriteBatch spriteBatch, SpriteFont menuSmall)
        {
            if (GameConstants.factoryTextScaleFactor == 0)
            {
                GameConstants.factoryTextScaleFactor = (float)game.Window.ClientBounds.Width / 1280 * (float)game.Window.ClientBounds.Height / 800;
                GameConstants.factoryTextScaleFactor = (float)Math.Sqrt(GameConstants.factoryTextScaleFactor);
                //GameConstants.lineSpacing = (int)(GameConstants.lineSpacing * GameConstants.textScaleFactor);
            }

            float textScaleFactor = GameConstants.factoryTextScaleFactor;
            float lineSpacing = GameConstants.lineSpacing;

            float fadeFactor = 0.65f;
            string title = "";
            string production_str = "PRODUCT: "+produce.ToString().ToUpper();
            string plant_basic_description = "";
            string plant_upgradeLevel_description = "";
            string technologyLevel = "";
            float numDays;
            switch (factoryType)
            {
                case FactoryType.biodegradable:
                    plant_basic_description = "Biodegradable trash decompose naturally. The organic matter in these great mounds of waste is consumed by bacteria that give off gas rich in methane, which is a harmful greenhouse gas.\n However, Methane's risk to global warming is also a great opportunity to supplying us with a bounty of fuel to take care of our social needs. Therefore these trashes must be processed in a factory not only to prevent these gases from escaping into the atmosphere, but also to use them to generate energy.\n In fact, power from landfill methane exceeds solar power in New York and New Jersey, and landfill methane in those states and in Connecticut powers generators that produce a total of 169 megawatts of electricity - almost as much as a small conventional generating station. The methane also provides 16.7 million cubic feet of gas daily for heating and other direct uses.";
                    numDays = (float)processingTime / GameConstants.DaysPerSecond;
                    production_str += " for "+trashBlockSize+" trash in " + numDays.ToString();
                    title = "Biodegradable Trash Processing Plant";
                    if(numDays > 1)
                        production_str += " days";
                    else
                        production_str += " day";
                    if (HydroBot.bioPlantLevel == 1)
                    {
                        technologyLevel = "Basic technology";
                        plant_upgradeLevel_description = "In this plant the trash decompose naturally to produce methane. Some chemicals like fertilizers are used to quicken the decomposition.";
                    }
                    else if (HydroBot.bioPlantLevel == 2)
                    {
                        technologyLevel = "Advanced technology";
                        plant_upgradeLevel_description = "In this plant the trash is mixed at temperatures of up to 2000 degrees Fahrenheit. The heat then makes steam, which runs a turbine and produces electricity.";
                    }
                    else
                    {
                        technologyLevel = "State of the Art technology";
                        plant_upgradeLevel_description = "This plant uses flash carbonization to produce charcoal from biomass. This process involves the ignition of a flash fire at elevated pressure in a packed bed of biomass. Because of the elevated pressure, the fire quickly spreads through the bed, triggering the transformation of biomass to biocarbon.";
                    }
                    break;
                case FactoryType.plastic:
                    title = "Plastic Recycling Plant";
                    plant_basic_description = "Basic steps for plastic recycling:\n 1) Manual Sorting: All non-plastic materials are removed. Plastic is sorted into 3 types: PET, HDPE and 'others'.\n 2) Chipping: The sorted plastic is cut into small pieces ready to be mented down.\n 3) Washing: Contaminants are removed.\n 4) Pelleting: The plastic is mented down and made into small pellets.\n Types of plastic (with code and some examples): PET - bottles, HDPE - milk bottles, bags, PVC - pipes, detergent bottles, raincoats, LDPE - bread bags, PP - straws, screw-on lids, PS - foam, yogurt containers, Others - ketchup bottles.\n The code numbers are printed within a recycle sign on most plastic containers.";
                    numDays = (float)processingTime / GameConstants.DaysPerSecond;
                    production_str += " for " + trashBlockSize + " trash in "+ numDays.ToString() +" day";
                    if (HydroBot.plasticPlantLevel == 1)
                    {
                        technologyLevel = "Basic technology";
                        plant_upgradeLevel_description = "Usually only type 1 and 2 are recycled. Recycled PET is usually used to make threads which are used to make shoes, jackets, hats. Recycled HDPE is used to make durable products like tables, rulers, trashcans, etc. Other types are not recycled due to lack of incentive to invest in equipments required.";
                    }
                    else if (HydroBot.plasticPlantLevel == 2)
                    {
                        technologyLevel = "Advanced technology";
                        plant_upgradeLevel_description = "Monomer Recycling: The polymers undergoes inverse of the polymerization reaction which is used during manufacturing. This creates same mix of chemicals that formed the original polymer, which can be purified and used to synthesize new polymer chains of the same type.";
                    }
                    else
                    {
                        technologyLevel = "State of the Art technology";
                        plant_upgradeLevel_description = "Thermal Depolymerization: Melts plastic into petroleum that can be remade into a variety of products.\n Biodegradable plastics can also be produced which can decompose in composting plants where it is placed in a heated environment with moisture and oxygen for months.";
                    }
                    break;
                case FactoryType.radioactive:
                    title = "Radioactive Trash Processing Plant";
                    plant_basic_description = "Spent nuclear fuel from nuclear energy plants are illegally trashed in the ocean in steel casks. These containers will start leaking within 1 year as the radioactive waste is highly corrosive. Such toxic waste will cause cancer and birth defects to sea creatures. Hence, these casks need to be brought into this facility for safe storage.\n This plant reprocesses the trash to recover fissionable plutonium, which is used to build resources and powerpacks for the hydrobot.\n Radioactive waste takes thousands of years to become non-radioactive. Hence these are stored in containers designed to withstand corrosion, radiation and temperature extremes. These containers are sealed tightly and stored deep underground.\n These waste also contain low-level radioactive materials like machinery, tools, clothing, air masks etc which got exposed to radiation. These are burried near the surface of the earth as they are not very dangerous and usually lose their radioactivity within a couple hundred years.";
                    production_str = "PRODUCT: "+ numRadioActiveProducts +" "+produce.ToString().ToUpper()+ "S for 1 trash in 1 day";
                    break;
            }

            title = Poseidon.Core.IngamePresentation.wrapLine(title, backgroundRect.Width, factoryFont, 1.5f);

            //draw background
            spriteBatch.Draw(background, backgroundRect, Color.White);

            Vector2 titlePos = new Vector2(backgroundRect.Center.X, backgroundRect.Top + 70 * textScaleFactor);
            //draw title string
            spriteBatch.DrawString(factoryFont, title, titlePos, Color.Yellow * fadeFactor, 0, new Vector2(factoryFont.MeasureString(title).X/2, factoryFont.MeasureString(title).Y/2), 1.5f * textScaleFactor, SpriteEffects.None, 0);

            Vector2 technologyPos = titlePos + new Vector2(0, factoryFont.MeasureString(title).Y / 2 * 1.5f + lineSpacing + factoryFont.MeasureString(technologyLevel).Y/2) * textScaleFactor;
            //draw current technology string
            spriteBatch.DrawString(factoryFont, technologyLevel, technologyPos, Color.Red * fadeFactor, 0, new Vector2(factoryFont.MeasureString(technologyLevel).X / 2, factoryFont.MeasureString(technologyLevel).Y / 2), textScaleFactor, SpriteEffects.None, 0);

            Vector2 productionPos = technologyPos + new Vector2(0, factoryFont.MeasureString(technologyLevel).Y / 2 + lineSpacing + factoryFont.MeasureString(production_str).Y / 2) * textScaleFactor;
            //draw production string
            spriteBatch.DrawString(factoryFont, production_str, productionPos, Color.LawnGreen * fadeFactor, 0, new Vector2(factoryFont.MeasureString(production_str).X / 2, factoryFont.MeasureString(production_str).Y / 2), textScaleFactor, SpriteEffects.None, 0);

            if (produceButtonHover) produceButton = IngamePresentation.factoryProduceButtonHoverTexture;
            if (produceButtonPress) produceButton = IngamePresentation.factoryProduceButtonPressedTexture;
            if (!produceButtonHover && !produceButtonPress) produceButton = IngamePresentation.factoryProduceButtonNormalTexture;
            //draw change_produce button
            spriteBatch.Draw(produceButton, produceRect, Color.White * fadeFactor);

            produceRect.Y = (int)(productionPos.Y + (lineSpacing + factoryFont.MeasureString(production_str).Y / 2) * textScaleFactor);
            //draw change_produce text
            string changeProduceButtonText;
            if (produce == Produce.resource)
                changeProduceButtonText = "PRODUCE POWERPACKS";
            else
                changeProduceButtonText = "PRODUCE RESOURCES";
            spriteBatch.DrawString(factoryFont, changeProduceButtonText, new Vector2(produceRect.Center.X - factoryFont.MeasureString(changeProduceButtonText).X / 2, produceRect.Center.Y - factoryFont.MeasureString(changeProduceButtonText).Y / 2), Color.White * fadeFactor);
            
            //draw how many resources are being processed
            string beingProcessedStr;
            int numBeingProcessed;
            numBeingProcessed = listTimeTrashProcessing.Count;
            if (numBeingProcessed == 1)
                beingProcessedStr = "CURRENT STATUS: " + numBeingProcessed.ToString() + " " + produce.ToString().ToUpper() + " ARE BEING GENERATED.";
            else if (numBeingProcessed > 1)
                beingProcessedStr = "CURRENT STATUS: " + numBeingProcessed.ToString() + " " + produce.ToString().ToUpper() + "S ARE BEING GENERATED.";
            else
                beingProcessedStr = "CURRENT STATUS: " + numTrashWaiting.ToString() + " TRASH WAITING TO BE PROCESSED.";
            beingProcessedStr = Poseidon.Core.IngamePresentation.wrapLine(beingProcessedStr, backgroundRect.Width - 100, factoryFont, textScaleFactor);
            spriteBatch.DrawString(factoryFont, beingProcessedStr, new Vector2(backgroundRect.Center.X, produceRect.Bottom + (lineSpacing / 2 + factoryFont.MeasureString(beingProcessedStr).Y/2) * textScaleFactor), Color.Black * fadeFactor, 0, new Vector2(factoryFont.MeasureString(beingProcessedStr).X / 2, factoryFont.MeasureString(beingProcessedStr).Y / 2), textScaleFactor, SpriteEffects.None, 0);

            //draw description
            plant_basic_description = Poseidon.Core.IngamePresentation.wrapLine(plant_basic_description, backgroundRect.Width - 130, factoryFont, textScaleFactor);
            Vector2 basicDescPos = new Vector2(backgroundRect.Center.X, produceRect.Bottom + (45 + factoryFont.MeasureString(plant_basic_description).Y/2) * textScaleFactor);
            spriteBatch.DrawString(factoryFont, plant_basic_description, basicDescPos, Color.Purple * fadeFactor, 0, new Vector2(factoryFont.MeasureString(plant_basic_description).X/2, factoryFont.MeasureString(plant_basic_description).Y/2), textScaleFactor, SpriteEffects.None, 0);

            if (factoryType != FactoryType.radioactive)
            {
                string text = "Current technology:";
                Vector2 textPos = basicDescPos + new Vector2(0, factoryFont.MeasureString(plant_basic_description).Y / 2 + lineSpacing + factoryFont.MeasureString(text).Y/2) * textScaleFactor;
                spriteBatch.DrawString(factoryFont, text, textPos, Color.Red * fadeFactor, 0, new Vector2(factoryFont.MeasureString(text).X / 2, factoryFont.MeasureString(text).Y / 2), textScaleFactor, SpriteEffects.None, 0);

                plant_upgradeLevel_description = IngamePresentation.wrapLine(plant_upgradeLevel_description, backgroundRect.Width - 130, factoryFont, textScaleFactor);
                Vector2 upgradePos = textPos + new Vector2(0, factoryFont.MeasureString(text).Y / 2 + lineSpacing + factoryFont.MeasureString(plant_upgradeLevel_description).Y / 2) * textScaleFactor;
                spriteBatch.DrawString(factoryFont, plant_upgradeLevel_description, upgradePos, Color.LawnGreen * fadeFactor, 0, new Vector2(factoryFont.MeasureString(plant_upgradeLevel_description).X / 2, factoryFont.MeasureString(plant_upgradeLevel_description).Y / 2), textScaleFactor, SpriteEffects.None, 0);
            }

            //string nextText = "Press Enter to continue";
            //Vector2 nextTextPosition = new Vector2(backgroundRect.Right - menuSmall.MeasureString(nextText).X / 2 - 50, backgroundRect.Bottom - menuSmall.MeasureString(nextText).Y / 2 - 30);
            //spriteBatch.DrawString(menuSmall, nextText, nextTextPosition, Color.White * fadeFactor, 0, new Vector2(menuSmall.MeasureString(nextText).X / 2, menuSmall.MeasureString(nextText).Y / 2), textScaleFactor, SpriteEffects.None, 0);

        }

        public void SwitchProductionItem()
        {
            produce = ((produce==Produce.powerpack)? produce = Produce.resource : produce = Produce.powerpack);
            defaultChoice = produce;
        }

        public void SetUpgradeLevelDependentVariables()
        {
            switch (factoryType)
            {
                case FactoryType.biodegradable:
                    if (HydroBot.bioPlantLevel == 1)
                    {
                        trashBlockSize = 5;
                        processingTime = 4; //5 days
                    }
                    else if (HydroBot.bioPlantLevel == 2)
                    {
                        trashBlockSize = 5; 
                        processingTime = 4; // 2 days
                    }
                    else
                    {
                        trashBlockSize = 5;
                        processingTime = 4; //1 day
                    }
                    break;
                case FactoryType.plastic:
                    if (HydroBot.plasticPlantLevel == 1)
                    {
                        trashBlockSize = 5;
                        processingTime = 4;
                    }
                    else if (HydroBot.plasticPlantLevel == 2)
                    {
                        trashBlockSize = 3;
                        processingTime = 4;
                    }
                    else
                    {
                        trashBlockSize = 1;
                        processingTime = 4;
                    }
                    break;
                case FactoryType.radioactive:
                    trashBlockSize = 1;
                    processingTime = 4;
                    break;
            }
        }

        void producePowerPacks(ref List<Powerpack> powerpacks, List<Resource> resources, ref Model[] powerpackModels)
        {
            Vector3 powerpackPosition;
            int powerType = random.Next(4) + 1;
            Powerpack powerpack = new Powerpack(powerType);
            powerpackPosition = findResourcePowerpackPosition(Position, resources, powerpacks);
            powerpack.Model = powerpackModels[powerType];
            powerpack.LoadContent(powerpackPosition);
            powerpacks.Add(powerpack);
        }

        void ProduceResource(ref List<Resource> resources, List<Powerpack> powerpacks, ref Model resourceModel)
        {
            Vector3 resourcePosition;
            Resource resource = new Resource();
            resourcePosition = findResourcePowerpackPosition(Position, resources, powerpacks);
            resource.Model = resourceModel;
            resource.LoadContent(resourcePosition);
            resources.Add(resource);
        }

        void ProduceStrangeRock(ref List<Powerpack> powerpacks, List<Resource> resources, ref Model[] strangeRockModels)
        {
            Vector3 powerpackPosition;
            PowerPackType powerType = PowerPackType.StrangeRock; //type 5 for strange rock
            Powerpack powerpack = new Powerpack(powerType);
            powerpackPosition = findResourcePowerpackPosition(Position, resources, powerpacks);
            powerpack.Model = strangeRockModels[random.Next(2)];
            powerpack.LoadContent(powerpackPosition);
            powerpacks.Add(powerpack);
        }

        private Vector3 findResourcePowerpackPosition(Vector3 factoryPosition, List<Resource> resources, List<Powerpack> powerpacks)
        {
            Vector3 floatPosition = factoryPosition;
            float radius = 5f;
            //cuz this only appears in main game or survival mode
            floatPosition.Y = GameConstants.MainGameFloatHeight;
            for(int i = 0; i < GameConstants.MaxNumTries; i++) // try 20 times
            {
                if (Collision.isFloatPositionValid(floatPosition, radius, powerpacks, resources))
                {
                    return floatPosition;
                }
                else
                {
                    int movement = random.Next(4);
                    switch (movement)
                    {
                        case 0:
                            floatPosition.X += radius * 2;
                            break;
                        case 1:
                            floatPosition.Z += radius * 2;
                            break;
                        case 2:
                            floatPosition.X -= radius * 2;
                            break;
                        case 3:
                            floatPosition.Z -= radius * 2;
                            break;
                    }
                }
            }
            return floatPosition;
        }
    }
}
