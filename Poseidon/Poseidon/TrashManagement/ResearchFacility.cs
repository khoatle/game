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

        ContentManager Content;
        Game game;

        //Creation time
        double facilityCreationTime;

        //upgrade bools
        public bool bioUpgrade, plasticUpgrade; 

        //For Factory Configuration Screen
        SpriteFont facilityFont, facilityFont2;
        Texture2D background, upgradeButton, playJigsawButton;
        public Rectangle backgroundRect, bioUpgradeRect, plasticUpgradeRect, playSeaCowJigsawRect, playTurtleJigsawRect, playDolphinJigsawRect;

        Random random;

        public ResearchFacility()
            : base()
        {
            facilityCreationTime = PoseidonGame.playTime.TotalSeconds;
            random = new Random();
            bioUpgrade = plasticUpgrade = false;
        }

        public void LoadContent(ContentManager content, Game game, Vector3 position, float orientation)
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
            this.Content = content;

            facilityFont = Content.Load<SpriteFont>("Fonts/researchFacilityConfig");
            facilityFont2 = Content.Load<SpriteFont>("Fonts/researchFacilityConfig2");
            background = Content.Load<Texture2D>("Image/TrashManagement/ResearchFacilityBackground");
            upgradeButton = Content.Load<Texture2D>("Image/TrashManagement/upgradeButton");
            playJigsawButton = Content.Load<Texture2D>("Image/TrashManagement/upgradeButton");
            
            backgroundRect = new Rectangle(game.Window.ClientBounds.Center.X - 500, game.Window.ClientBounds.Center.Y - 400, 1000, 800);
            bioUpgradeRect = new Rectangle(game.Window.ClientBounds.Center.X - 300, backgroundRect.Top + 300, 200, 50);
            plasticUpgradeRect = new Rectangle(game.Window.ClientBounds.Center.X + 100, backgroundRect.Top + 300, 200, 50);
            playSeaCowJigsawRect = new Rectangle(backgroundRect.Center.X - 350, backgroundRect.Bottom - 300, 200, 50);
            playTurtleJigsawRect = new Rectangle(backgroundRect.Center.X - 100, backgroundRect.Bottom - 300, 200, 50);
            playDolphinJigsawRect = new Rectangle(backgroundRect.Center.X + 150, backgroundRect.Bottom - 300, 200, 50);

            this.game = game;

            // Set up the parameters
            SetupShaderParameters(PoseidonGame.contentManager, Model);

        }

        // Overloading content load so that survival mode game compiles properly.
        public void LoadContent(ContentManager content, Game game, string modelname, Vector3 position, float orientation)
        {
            Model = content.Load<Model>(modelname);
            LoadContent(content, game, position, orientation);

            // Set up the parameters
            SetupShaderParameters(PoseidonGame.contentManager, Model);
        }

        public void Update(GameTime gameTime)
        {

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

            string bioButtonText, plasticButtonText;

            if ((PoseidonGame.playTime.TotalSeconds - facilityCreationTime) < GameConstants.numDaysForUpgrade * GameConstants.DaysPerSecond)
            {
                bioButtonText = "No upgrades available. Research Facility takes atleast "+GameConstants.numDaysForUpgrade+" days to develop upgrades";
                bioButtonText = Poseidon.Core.IngamePresentation.wrapLine(bioButtonText, backgroundRect.Width-100, facilityFont2);
                spriteBatch.DrawString(facilityFont2, bioButtonText, new Vector2(backgroundRect.Left + 50, bioUpgradeRect.Top - 50), Color.White);
            }
            else
            {
                string button_title_text = "ORGANIC FACTORY";
                spriteBatch.DrawString(menuSmall, button_title_text, new Vector2(bioUpgradeRect.Center.X - menuSmall.MeasureString(button_title_text).X / 2, bioUpgradeRect.Top - 50), Color.White);
                button_title_text = "PLASTIC FACTORY";
                spriteBatch.DrawString(menuSmall, button_title_text, new Vector2(plasticUpgradeRect.Center.X - menuSmall.MeasureString(button_title_text).X / 2, plasticUpgradeRect.Top - 50), Color.White);
                if (HydroBot.bioPlantLevel == 1 && HydroBot.totalBioTrashProcessed >= GameConstants.numTrashForUpgrade)
                {
                    bioUpgrade = true;
                    bioButtonText = "UPGRADE TO L2";
                }
                else if (HydroBot.bioPlantLevel == 2 && HydroBot.totalBioTrashProcessed >= GameConstants.numTrashForUpgrade)
                {
                    bioUpgrade = true;
                    bioButtonText = "UPGRADE TO L3";
                }
                else
                {
                    bioUpgrade = false;
                    bioButtonText = "";
                }

                if (HydroBot.plasticPlantLevel == 1 && HydroBot.totalPlasticTrashProcessed >= GameConstants.numTrashForUpgrade)
                {
                    plasticUpgrade = true;
                    plasticButtonText = "UPGRADE TO L2";
                }
                else if (HydroBot.plasticPlantLevel == 2 && HydroBot.totalPlasticTrashProcessed >= GameConstants.numTrashForUpgrade)
                {
                    plasticUpgrade = true;
                    plasticButtonText = "UPGRADE TO L3";
                }
                else
                {
                    plasticUpgrade = false;
                    plasticButtonText = "";
                }

                if(bioUpgrade)
                {
                    //draw bio upgrade buttons
                    spriteBatch.Draw(upgradeButton, bioUpgradeRect, Color.White);
                    spriteBatch.DrawString(menuSmall, bioButtonText, new Vector2(bioUpgradeRect.Center.X - menuSmall.MeasureString(bioButtonText).X / 4, bioUpgradeRect.Center.Y - menuSmall.MeasureString(bioButtonText).Y / 4), Color.Red,  0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                }
                else if (HydroBot.bioPlantLevel == 3)
                {
                    bioButtonText= "This is the best technology available. Can not upgrade.";
                    bioButtonText = Poseidon.Core.IngamePresentation.wrapLine(bioButtonText, (backgroundRect.Width / 2) - 200, facilityFont2);
                    spriteBatch.DrawString(facilityFont2, bioButtonText, new Vector2(bioUpgradeRect.Center.X - facilityFont2.MeasureString(bioButtonText).X / 2, bioUpgradeRect.Center.Y - facilityFont2.MeasureString(bioButtonText).Y / 2), Color.White);
                }
                else
                {
                    bioButtonText = "You need to process "+(GameConstants.numTrashForUpgrade-HydroBot.totalBioTrashProcessed).ToString()+" more trash to upgrade";
                    bioButtonText = Poseidon.Core.IngamePresentation.wrapLine(bioButtonText, (backgroundRect.Width/2) - 200, facilityFont2);
                    spriteBatch.DrawString(facilityFont2, bioButtonText, new Vector2(bioUpgradeRect.Center.X - facilityFont2.MeasureString(bioButtonText).X/2, bioUpgradeRect.Center.Y - facilityFont2.MeasureString(bioButtonText).Y/2), Color.White);
                }

                if(plasticUpgrade)
                {
                    spriteBatch.Draw(upgradeButton, plasticUpgradeRect, Color.White);
                    spriteBatch.DrawString(menuSmall, plasticButtonText, new Vector2(plasticUpgradeRect.Center.X - menuSmall.MeasureString(plasticButtonText).X / 4, plasticUpgradeRect.Center.Y - menuSmall.MeasureString(plasticButtonText).Y / 4), Color.Red,  0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                }
                else if (HydroBot.plasticPlantLevel == 3)
                {
                    plasticButtonText = "This is the best technology available. Can not upgrade.";
                    plasticButtonText = Poseidon.Core.IngamePresentation.wrapLine(plasticButtonText, (backgroundRect.Width / 2) - 200, facilityFont2);
                    spriteBatch.DrawString(facilityFont2, plasticButtonText, new Vector2(plasticUpgradeRect.Center.X - facilityFont2.MeasureString(plasticButtonText).X / 2, plasticUpgradeRect.Center.Y - facilityFont2.MeasureString(plasticButtonText).Y / 2), Color.White);
                }
                else
                {
                    plasticButtonText = "You need to process " + (GameConstants.numTrashForUpgrade - HydroBot.totalPlasticTrashProcessed).ToString() + " more trash to upgrade";
                    plasticButtonText = Poseidon.Core.IngamePresentation.wrapLine(plasticButtonText, (backgroundRect.Width / 2) - 200, facilityFont2);
                    spriteBatch.DrawString(facilityFont2, plasticButtonText, new Vector2(plasticUpgradeRect.Center.X - facilityFont2.MeasureString(plasticButtonText).X / 2, plasticUpgradeRect.Center.Y - facilityFont2.MeasureString(plasticButtonText).Y / 2), Color.White);
                }
            }

            //Draw Sea Cow Jigsaw Button
            string seacowJigsawButtonText = "RESURRECT STELLAR'S\nSEACOW";
            spriteBatch.Draw(playJigsawButton, playSeaCowJigsawRect, Color.White);
            spriteBatch.DrawString(menuSmall, seacowJigsawButtonText, new Vector2(playSeaCowJigsawRect.Center.X - menuSmall.MeasureString(seacowJigsawButtonText).X / 4, playSeaCowJigsawRect.Center.Y - menuSmall.MeasureString(seacowJigsawButtonText).Y / 4), Color.Red, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);

            //Draw Sea Turtle Jigsaw Button
            string turtleJigsawButtonText = "RESURRECT MEIOLANIA\nTURTLE";
            spriteBatch.Draw(playJigsawButton, playTurtleJigsawRect, Color.White);
            spriteBatch.DrawString(menuSmall, turtleJigsawButtonText, new Vector2(playTurtleJigsawRect.Center.X - menuSmall.MeasureString(turtleJigsawButtonText).X / 4, playTurtleJigsawRect.Center.Y - menuSmall.MeasureString(turtleJigsawButtonText).Y / 4), Color.Red, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);

            //Draw Sea Dolphin Jigsaw Button
            string dolphinJigsawButtonText = "RESURRECT MAUI'S\nDOLPHIN";
            spriteBatch.Draw(playJigsawButton, playDolphinJigsawRect, Color.White);
            spriteBatch.DrawString(menuSmall, dolphinJigsawButtonText, new Vector2(playDolphinJigsawRect.Center.X - menuSmall.MeasureString(dolphinJigsawButtonText).X/4, playDolphinJigsawRect.Center.Y - menuSmall.MeasureString(dolphinJigsawButtonText).Y/4), Color.Red, 0, new Vector2(0,0), 0.5f, SpriteEffects.None, 0);

            string nextText = "Press Enter to continue";
            Vector2 nextTextPosition = new Vector2(backgroundRect.Right - menuSmall.MeasureString(nextText).X, backgroundRect.Bottom - menuSmall.MeasureString(nextText).Y);
            spriteBatch.DrawString(menuSmall, nextText, nextTextPosition, Color.Black);
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
