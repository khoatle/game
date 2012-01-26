using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon
{
    public class ResearchFacility : GameObject
    {
        public float orientation;

        public float fogEndValue = GameConstants.FogEnd;
        public float fogEndMaxVal = 1000.0f;
        public bool increaseFog = true;

        ContentManager Content;
        Game game;

        //Creation time
        double facilityCreationTime;

        //Total Trash count -- has to be static since we need to count even before research facility is created.
        public static int totalBioTrashProcessed = 0;
        public static int totalPlasticTrashProcessed = 0;
        public static int totalNuclearTrashProcessed = 0;

        //For Factory Configuration Screen
        SpriteFont facilityFont;
        Texture2D background, bioUpgradeButton, plasticUpgradeButton;
        public Rectangle backgroundRect, bioUpgradeRect, plasticUpgradeRect;

        Random random;

        public ResearchFacility()
            : base()
        {
            facilityCreationTime = PoseidonGame.playTime.TotalSeconds;
            random = new Random();
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
            BoundingSphere = new BoundingSphere(tempCenter, BoundingSphere.Radius);

            this.orientation = orientation;
            this.Content = content;

            facilityFont = Content.Load<SpriteFont>("Fonts/researchFacilityConfig");
            background = Content.Load<Texture2D>("Image/TrashManagement/ResearchFacilityBackground");
            bioUpgradeButton = Content.Load<Texture2D>("Image/TrashManagement/upgradeButton");
            plasticUpgradeButton = Content.Load<Texture2D>("Image/TrashManagement/upgradeButton");
            
            backgroundRect = new Rectangle(game.Window.ClientBounds.Center.X - 500, game.Window.ClientBounds.Center.Y - 400, 1000, 800);
            bioUpgradeRect = new Rectangle(game.Window.ClientBounds.Center.X - 400, backgroundRect.Top + 300, 200, 50);
            plasticUpgradeRect = new Rectangle(game.Window.ClientBounds.Center.X + 200, backgroundRect.Top + 300, 200, 50);

            this.game = game;

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

        public void DrawResearchFacilityConfigurationScene(SpriteBatch spriteBatch, SpriteFont menuSmall)
        {
            string title = "RESEARCH FACILITY";
            string description = "Welcome to the research facility. Scientists work here to upgrade processing factories. State of the art genetic engineering will help you resurrect extinct animals.";
            title = Poseidon.Core.IngamePresentation.wrapLine(title, backgroundRect.Width, facilityFont, 1.5f);

            //draw background
            spriteBatch.Draw(background, backgroundRect, Color.White);

            //draw title string
            spriteBatch.DrawString(facilityFont, title, new Vector2(backgroundRect.Center.X - facilityFont.MeasureString(title).X * 0.75f, backgroundRect.Top + 30), Color.White, 0, new Vector2(0, 0), 1.5f, SpriteEffects.None, 0);

            //draw description
            description = Poseidon.Core.IngamePresentation.wrapLine(description, backgroundRect.Width - 100, facilityFont);
            spriteBatch.DrawString(facilityFont, description, new Vector2(backgroundRect.Left + 50, backgroundRect.Top + 100), Color.White);

            //draw upgrade buttons
            spriteBatch.Draw(bioUpgradeButton, bioUpgradeRect, Color.White);
            spriteBatch.Draw(plasticUpgradeButton, plasticUpgradeRect, Color.White);

            //Draw text on button.
            string buttonText = "UpgradeL"+(HydroBot.bioPlantLevel+1).ToString();
            spriteBatch.DrawString(facilityFont, buttonText, new Vector2(bioUpgradeRect.Center.X - facilityFont.MeasureString(buttonText).X / 2, bioUpgradeRect.Center.Y - facilityFont.MeasureString(buttonText).Y / 2), Color.Red);
            buttonText = "UpgradeL" + (HydroBot.plasticPlantLevel + 1).ToString();
            spriteBatch.DrawString(facilityFont, buttonText, new Vector2(plasticUpgradeRect.Center.X - facilityFont.MeasureString(buttonText).X / 2, plasticUpgradeRect.Center.Y - facilityFont.MeasureString(buttonText).Y / 2), Color.Red);

            string nextText = "Press Enter to continue";
            Vector2 nextTextPosition = new Vector2(backgroundRect.Right - menuSmall.MeasureString(nextText).X, backgroundRect.Bottom - menuSmall.MeasureString(nextText).Y);
            spriteBatch.DrawString(menuSmall, nextText, nextTextPosition, Color.Black);

        }

        public void UpgradeBioFactory(List<Factory> factories)
        {
            //Need to check whether upgrade is allowed. TODO: Deb
            HydroBot.bioPlantLevel++;
            foreach (Factory factory in factories)
            {
                if (factory.factoryType == FactoryType.biodegradable)
                    factory.SetUpgradeLevelDependentVariables();
            }
        }

        public void UpgradePlasticFactory(List<Factory> factories)
        {
            //Need to check if upgrade is allowed. TODO: Deb
            HydroBot.plasticPlantLevel++;
            foreach (Factory factory in factories)
            {
                if (factory.factoryType == FactoryType.plastic)
                    factory.SetUpgradeLevelDependentVariables();
            }
        }

    }
}
