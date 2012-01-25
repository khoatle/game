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
        public int upgradeLevel = 1;

        public float orientation;

        public float fogEndValue = GameConstants.FogEnd;
        public float fogEndMaxVal = 1000.0f;
        public bool increaseFog = true;

        ContentManager Content;
        Game game;

        //For Factory Configuration Screen
        SpriteFont factoryFont;
        Texture2D background, produceButton;//, bioUpgradeButton, plasticUpgradeButton, nuclearUpgradeButton;
        public Rectangle backgroundRect, produceRect;//, bioUpgradeRect, plasticUpgradeRect, nuclearUpgradeRect;
        enum Produce { resource, powerpack };
        Produce produce;

        public Factory(FactoryType factorytype)
            : base()
        {
            this.factoryType = factorytype;
        }

        public void LoadContent(ContentManager content, Game game, string modelname, Vector3 position, float orientation)
        {
            Model = content.Load<Model>(modelname);
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
            this.Content = content;

            produce = Produce.resource;
            factoryFont = Content.Load<SpriteFont>("Fonts/factoryConfig");
            background = Content.Load<Texture2D>("Image/Miscellaneous/factory_config_background");
            produceButton = Content.Load<Texture2D>("Image/Miscellaneous/ChangeFactoryProduceBox");
            //bioUpgradeButton = Content.Load<Texture2D>("Image/MinigameTextures/quizButton");
            //plasticUpgradeButton = Content.Load<Texture2D>("Image/MinigameTextures/quizButton");
            //nuclearUpgradeButton = Content.Load<Texture2D>("Image/Minigame/Textures/quizButton");
            backgroundRect = new Rectangle(game.Window.ClientBounds.Center.X - 500, game.Window.ClientBounds.Center.Y - 400, 1000, 800);
            produceRect = new Rectangle(backgroundRect.Center.X - 250, backgroundRect.Top + 120, 500, 65);
            this.game = game;

            //if (orientation > 50) floatUp = true;
            //else floatUp = false;
        }
        public void Update(GameTime gameTime)
        {
            if (increaseFog)
                fogEndValue += 2.5f;
            else fogEndValue -= 2.5f;
            if (fogEndValue > fogEndMaxVal || fogEndValue < GameConstants.FogEnd)
                increaseFog = !increaseFog;
        }
        public void Draw(Matrix view, Matrix projection)
        {
            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix translateMatrix = Matrix.CreateTranslation(Position);
            Matrix rotationYMatrix = Matrix.CreateRotationY(orientation);
            Matrix worldMatrix = rotationYMatrix * translateMatrix;

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

                    //effect.DiffuseColor = Color.Green.ToVector3();

                    effect.FogEnabled = true;
                    effect.FogStart = GameConstants.FogStart;
                    effect.FogEnd = fogEndValue;// GameConstants.FogEnd;
                    effect.FogColor = GameConstants.FogColor.ToVector3();
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
            switch (factoryType)
            {
                case FactoryType.biodegradable:
                    plant_basic_description = "";
                    if (upgradeLevel == 1)
                    {
                        title = "Biodegradable Trash Processing Plant (Basic technology)";
                        plant_upgradeLevel_description = "Trash decompose naturally to produce methane.";
                        production_str += " for 5 trash in 10 days";
                    }
                    else if (upgradeLevel == 2)
                    {
                        title = "Biodegradable Trash Processing Plant (Advanced)";
                        plant_upgradeLevel_description = "Put level 2 description here.";
                        production_str += " for 5 trash in 1 day";
                    }
                    else
                    {
                        title = "Biodegradable Trash Processing Plant (State of the Art)";
                        plant_upgradeLevel_description = "Need to write level 3 description";
                        production_str += " for 5 trash in 1/4 day";
                    }
                    break;
                case FactoryType.plastic:
                    title = "Plastic Recycling Plant";
                    plant_basic_description = "Basic steps for plastic recycling:\n1) Manual Sorting: All non-plastic materials are removed. Plastic is sorted into 3 types: PET, HDPE and 'others'.\n2) Chipping: The sorted plastic is cut into small pieces ready to be mented down.\n3) Washing: Contaminants are removed.\n4) Pelleting: The plastic is mented down and made into small pellets.\n\nTypes of plastic (with code and examples):\n1: PET - bottles\n2: HDPE - milk bottles, bags\n3: PVC - pipes, detergent bottles, raincoats\n4: LDPE - bread bags\n5: PP - straws, screw-on lids\n6: PS - foam, yogurt containers\n7: Others - ketchup bottles\n\nThe code numbers are printed within a recycle sign on most plastic containers.";
                    if (upgradeLevel == 1)
                    {
                        title += " (Basic technology)";
                        plant_upgradeLevel_description = " Usually only type 1 and 2 are recycled. Recycled PET is usually used to make threads which are used to make shoes, jackets, hats. Recycled HDPE is used to make durable products like tables, rulers, trashcans, etc. Other types are not recycled due to lack of incentive to invest in equipments required.";
                        production_str += " for 10 trash in 1 day";
                    }
                    else if (upgradeLevel == 2)
                    {
                        title += " (Advanced)";
                        plant_upgradeLevel_description = " Monomer Recycling: The polymers undergoes inverse of the polymerization reaction which is used during manufacturing. This creates same mix of chemicals that formed the original polymer, which can be purified and used to synthesize new polymer chains of the same type.";
                        production_str += " for 3 trash in 1 day";
                    }
                    else
                    {
                        title += " (State of the Art)";
                        plant_upgradeLevel_description = " Thermal Depolymerization: Melts plastic into petroleum that can be remade into a variety of products.\nBiodegradable plastics can also be produced which can decompose in composting plants where it is placed in a heated environment with moisture and oxygen for months.";
                        production_str += " for 1 trash in 1 day";
                    }
                    break;
                case FactoryType.radioactive:
                    title = "Radioactive Trash Processing Plant";
                    plant_basic_description = " Some part is reused to produce fuel. Remaining waste is concentrated to reduce the volume and stored it in a sealed container. It might take millions of years to lose its radioactive property completely.";
                    production_str = "PRODUCT: 5 "+produce.ToString().ToUpper()+ "S for 1 trash in 1 day";
                    break;
                case FactoryType.research:
                    title = "Research Facility";
                    plant_basic_description = " Need to put upgradation options here. Also about fossils. How many more fragments required to recreate an extinct species";
                    break;
            }



            title = Poseidon.Core.IngamePresentation.wrapLine(title, backgroundRect.Width, factoryFont, 1.5f);

            //draw background
            spriteBatch.Draw(background, backgroundRect, Color.White);

            //draw title string
            spriteBatch.DrawString(factoryFont, title, new Vector2(backgroundRect.Center.X - factoryFont.MeasureString(title).X * 0.75f, backgroundRect.Top + 30), Color.Red, 0, new Vector2(0, 0), 1.5f, SpriteEffects.None, 0);


            if (factoryType != FactoryType.research)
            {
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
            }

            //draw description
            string text = Poseidon.Core.IngamePresentation.wrapLine(plant_basic_description + plant_upgradeLevel_description, backgroundRect.Width - 100, factoryFont);
            spriteBatch.DrawString(factoryFont, text, new Vector2(backgroundRect.Left + 50, produceRect.Bottom + 5), Color.DarkRed);

            string nextText = "Press Enter to continue";
            Vector2 nextTextPosition = new Vector2(backgroundRect.Right - menuSmall.MeasureString(nextText).X, backgroundRect.Bottom - menuSmall.MeasureString(nextText).Y);
            spriteBatch.DrawString(menuSmall, nextText, nextTextPosition, Color.Black);

        }

        public void SwitchProductionItem()
        {
             produce = ((produce==Produce.powerpack)? produce = Produce.resource : produce = Produce.powerpack);
        }
    }
}
