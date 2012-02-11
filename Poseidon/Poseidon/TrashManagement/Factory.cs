using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon
{
    public class Factory : GameObject
    {
        public FactoryType factoryType;

        public float orientation;

        ContentManager Content;
        Game game;

        public int numTrashWaiting;
        public List<double> listTimeTrashProcessing;

        //Dependent of upgrade level
        int trashBlockSize;
        int processingTime;

        int numRadioActiveProducts = 5;
        int delayRadioActiveProducts = 4; //4sec,1day

        //For Factory Configuration Screen
        SpriteFont factoryFont;
        Texture2D background, produceButton;
        public Rectangle backgroundRect, produceRect;
        enum Produce { resource, powerpack };
        Produce produce;

        Random random;

        public Factory(FactoryType factorytype)
            : base()
        {
            this.factoryType = factorytype;
            numTrashWaiting = 0;
            listTimeTrashProcessing = new List<double>();
            random = new Random();
        }

        public void LoadContent(ContentManager content, Game game, Vector3 position, float orientation, SpriteFont font, Texture2D backgroundTexture, Texture2D produceButtonTexture)
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
            this.Content = content;
            this.orientation = orientation;

            produce = Produce.resource;
            factoryFont = font;
            background = backgroundTexture;
            produceButton = produceButtonTexture;
            backgroundRect = new Rectangle(game.Window.ClientBounds.Center.X - 500, game.Window.ClientBounds.Center.Y - 400, 1000, 800);
            produceRect = new Rectangle(backgroundRect.Center.X - 250, backgroundRect.Top + 120, 500, 65);

            SetUpgradeLevelDependentVariables();

            this.game = game;

            //if (orientation > 50) floatUp = true;
            //else floatUp = false;
            // Set up the parameters
            SetupShaderParameters(PoseidonGame.contentManager, Model);
        }

        // Overloading content load so that survival mode game compiles properly.
        public void LoadContent(ContentManager content, Game game, string modelname, Vector3 position, float orientation)
        {
            Model = content.Load<Model>(modelname);
            SpriteFont font = content.Load<SpriteFont>("Fonts/factoryConfig");
            Texture2D backgroundTexture = content.Load<Texture2D>("Image/TrashManagement/factory_config_background");
            Texture2D produceButtonTexture = content.Load<Texture2D>("Image/TrashManagement/ChangeFactoryProduceBox");
            LoadContent(content, game, position, orientation, font, backgroundTexture, produceButtonTexture);    
        }

        public void Update(GameTime gameTime, ref List<Powerpack> powerpacks,ref List<Resource> resources)
        {
            //Is trash finished processing?
            for (int i = 0; i < listTimeTrashProcessing.Count; i++)
            {
                if (PoseidonGame.playTime.TotalSeconds - listTimeTrashProcessing[i] >= processingTime)
                {
                    listTimeTrashProcessing.RemoveAt(i);
                    if (produce == Produce.powerpack)
                    {
                        producePowerPacks(ref powerpacks, resources);
                    }
                    else
                    {
                        ProduceResource(ref resources, powerpacks);
                    }

                    //Produce strange rock
                    if (random.Next(100) < 10) //10% probability
                    {
                        ProduceStrangeRock(ref powerpacks, resources);
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
                    //effect.FogEnd =  GameConstants.FogEnd;
                    //effect.FogColor = GameConstants.FogColor.ToVector3();

                    //for our custom BasicEffect
                    Matrix readlWorldMatrix = worldMatrix * transforms[mesh.ParentBone.Index];
                    effect.CurrentTechnique = effect.Techniques[techniqueName];
                    effect.Parameters["World"].SetValue(readlWorldMatrix);
                    effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Invert(readlWorldMatrix));
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                    effect.Parameters["EyePosition"].SetValue(new Vector4(gameCamera.AvatarHeadOffset, 0));
                    Matrix WorldView = readlWorldMatrix * view;
                    EffectHelpers.SetFogVector(ref WorldView, GameConstants.FogStart, GameConstants.FogEnd, effect.Parameters["FogVector"]);
                    effect.Parameters["FogColor"].SetValue(GameConstants.FogColor.ToVector3());
                }
                mesh.Draw();
            }
        }

        public void DrawFactoryConfigurationScene(SpriteBatch spriteBatch, SpriteFont menuSmall)
        {
            string title = "";
            string production_str = "PRODUCT: "+produce.ToString().ToUpper();
            string plant_basic_description = "";
            string plant_upgradeLevel_description = "";
            float numDays;
            switch (factoryType)
            {
                case FactoryType.biodegradable:
                    plant_basic_description = "Biodegradable trash decompose naturally. The organic matter in these great mounds of waste is consumed by bacteria that give off gas rich in methane, which is a harmful greenhouse gas. However, Methane's risk to global warming is also a great opportunity to supplying us with a bounty of fuel to take care of our social needs. Therefore these trashes must be processed in a factory not only to prevent these gases from escaping into the atmosphere, but also to use them to generate energy. In fact, power from landfill methane exceeds solar power in New York and New Jersey, and landfill methane in those states and in Connecticut powers generators that produce a total of 169 megawatts of electricity - almost as much as a small conventional generating station. The methane also provides 16.7 million cubic feet of gas daily for heating and other direct uses.\n";
                    numDays = (float)processingTime / GameConstants.DaysPerSecond;
                    production_str += " for "+trashBlockSize+" trash in " + numDays.ToString();
                    if(numDays > 1)
                        production_str += " days";
                    else
                        production_str += " day";
                    if (HydroBot.bioPlantLevel == 1)
                    {
                        title = "Biodegradable Trash Processing Plant (Basic technology)";
                        plant_upgradeLevel_description = "";
                    }
                    else if (HydroBot.bioPlantLevel == 2)
                    {
                        title = "Biodegradable Trash Processing Plant (Advanced)";
                        plant_upgradeLevel_description = "Put level 2 description here.";
                    }
                    else
                    {
                        title = "Biodegradable Trash Processing Plant (State of the Art)";
                        plant_upgradeLevel_description = "Need to write level 3 description";
                    }
                    break;
                case FactoryType.plastic:
                    title = "Plastic Recycling Plant";
                    plant_basic_description = "Basic steps for plastic recycling:\n1) Manual Sorting: All non-plastic materials are removed. Plastic is sorted into 3 types: PET, HDPE and 'others'.\n2) Chipping: The sorted plastic is cut into small pieces ready to be mented down.\n3) Washing: Contaminants are removed.\n4) Pelleting: The plastic is mented down and made into small pellets.\n\nTypes of plastic (with code and examples):\n1: PET - bottles\n2: HDPE - milk bottles, bags\n3: PVC - pipes, detergent bottles, raincoats\n4: LDPE - bread bags\n5: PP - straws, screw-on lids\n6: PS - foam, yogurt containers\n7: Others - ketchup bottles\nThe code numbers are printed within a recycle sign on most plastic containers.";
                    numDays = (float)processingTime / GameConstants.DaysPerSecond;
                    production_str += " for " + trashBlockSize + " trash in "+ numDays.ToString() +" day";
                    if (HydroBot.plasticPlantLevel == 1)
                    {
                        title += " (Basic technology)";
                        plant_upgradeLevel_description = " Usually only type 1 and 2 are recycled. Recycled PET is usually used to make threads which are used to make shoes, jackets, hats. Recycled HDPE is used to make durable products like tables, rulers, trashcans, etc. Other types are not recycled due to lack of incentive to invest in equipments required.";
                    }
                    else if (HydroBot.plasticPlantLevel == 2)
                    {
                        title += " (Advanced)";
                        plant_upgradeLevel_description = " Monomer Recycling: The polymers undergoes inverse of the polymerization reaction which is used during manufacturing. This creates same mix of chemicals that formed the original polymer, which can be purified and used to synthesize new polymer chains of the same type.";
                    }
                    else
                    {
                        title += " (State of the Art)";
                        plant_upgradeLevel_description = " Thermal Depolymerization: Melts plastic into petroleum that can be remade into a variety of products.\nBiodegradable plastics can also be produced which can decompose in composting plants where it is placed in a heated environment with moisture and oxygen for months.";
                    }
                    break;
                case FactoryType.radioactive:
                    title = "Radioactive Trash Processing Plant";
                    plant_basic_description = " Some part is reused to produce fuel. Remaining waste is concentrated to reduce the volume and stored it in a sealed container. It might take millions of years to lose its radioactive property completely.";
                    production_str = "PRODUCT: "+ numRadioActiveProducts +""+produce.ToString().ToUpper()+ "S for 1 trash in 1 day";
                    break;
            }

            title = Poseidon.Core.IngamePresentation.wrapLine(title, backgroundRect.Width, factoryFont, 1.5f);

            //draw background
            spriteBatch.Draw(background, backgroundRect, Color.White);

            //draw title string
            spriteBatch.DrawString(factoryFont, title, new Vector2(backgroundRect.Center.X - factoryFont.MeasureString(title).X * 0.75f, backgroundRect.Top + 30), Color.Red, 0, new Vector2(0, 0), 1.5f, SpriteEffects.None, 0);


            //draw production string
            spriteBatch.DrawString(menuSmall, production_str, new Vector2(produceRect.Center.X - menuSmall.MeasureString(production_str).X / 2, produceRect.Top - menuSmall.MeasureString(production_str).Y+2), Color.Red);

            //draw change_produce button
            spriteBatch.Draw(produceButton, produceRect, Color.White);

            //draw change_produce text
            string changeProduceButtonText;
            if (produce == Produce.resource)
                changeProduceButtonText = "CLICK TO PRODUCE POWERPACKS";
            else
                changeProduceButtonText = "CLICK TO PRODUCE RESOURCES";
            spriteBatch.DrawString(factoryFont, changeProduceButtonText, new Vector2(produceRect.Center.X - factoryFont.MeasureString(changeProduceButtonText).X / 2, produceRect.Center.Y - factoryFont.MeasureString(changeProduceButtonText).Y / 2), Color.White);
            
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
            beingProcessedStr = Poseidon.Core.IngamePresentation.wrapLine(beingProcessedStr, backgroundRect.Width - 100, factoryFont);
            spriteBatch.DrawString(factoryFont, beingProcessedStr, new Vector2(backgroundRect.Center.X - factoryFont.MeasureString(beingProcessedStr).X/2, produceRect.Bottom + 5), Color.Black);

            //draw description
            string text = Poseidon.Core.IngamePresentation.wrapLine(plant_basic_description + plant_upgradeLevel_description, backgroundRect.Width - 100, factoryFont);
            spriteBatch.DrawString(factoryFont, text, new Vector2(backgroundRect.Left + 50, produceRect.Bottom + 35), Color.DarkRed);

            string nextText = "Press Enter to continue";
            Vector2 nextTextPosition = new Vector2(backgroundRect.Right - menuSmall.MeasureString(nextText).X, backgroundRect.Bottom - menuSmall.MeasureString(nextText).Y);
            spriteBatch.DrawString(menuSmall, nextText, nextTextPosition, Color.Orange);

        }

        public void SwitchProductionItem()
        {
             produce = ((produce==Produce.powerpack)? produce = Produce.resource : produce = Produce.powerpack);
        }

        public void SetUpgradeLevelDependentVariables()
        {
            switch (factoryType)
            {
                case FactoryType.biodegradable:
                    if (HydroBot.bioPlantLevel == 1)
                    {
                        trashBlockSize = 5;
                        processingTime = 20; //5 days
                    }
                    else if (HydroBot.bioPlantLevel == 2)
                    {
                        trashBlockSize = 5; 
                        processingTime = 8; // 2 days
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

        void producePowerPacks(ref List<Powerpack> powerpacks, List<Resource> resources)
        {
            Vector3 powerpackPosition;
            int powerType = random.Next(4) + 1;
            Powerpack powerpack = new Powerpack(powerType);
            powerpackPosition = findResourcePowerpackPosition(Position, resources, powerpacks);
            powerpack.LoadContent(Content, powerpackPosition);
            powerpacks.Add(powerpack);
        }

        void ProduceResource(ref List<Resource> resources, List<Powerpack> powerpacks)
        {
            Vector3 resourcePosition;
            Resource resource = new Resource();
            resourcePosition = findResourcePowerpackPosition(Position, resources, powerpacks);
            resource.LoadContent(Content, resourcePosition);
            resources.Add(resource);
        }

        void ProduceStrangeRock(ref List<Powerpack> powerpacks, List<Resource> resources)
        {
            Vector3 powerpackPosition;
            int powerType = 5; //type 5 for strange rock
            Powerpack powerpack = new Powerpack(powerType);
            powerpackPosition = findResourcePowerpackPosition(Position, resources, powerpacks);
            powerpack.LoadContent(Content, powerpackPosition);
            powerpacks.Add(powerpack);
        }

        private Vector3 findResourcePowerpackPosition(Vector3 factoryPosition, List<Resource> resources, List<Powerpack> powerpacks)
        {
            Vector3 floatPosition = factoryPosition;
            float radius = 5f;
            floatPosition.Y = GameConstants.MainGameFloatHeight;
            for(int i=0; i<20; i++) // try 20 times
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
