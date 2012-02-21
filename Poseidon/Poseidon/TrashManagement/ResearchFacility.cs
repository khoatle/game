﻿using System;
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

        Random random;

        public ResearchFacility()
            : base()
        {
            facilityCreationTime = PoseidonGame.playTime.TotalSeconds;
            random = new Random();
            bioUpgrade = plasticUpgrade = false;
            listTimeRockProcessing = new List<double>();
        }

        public void LoadContent(Game game, Vector3 position, float orientation,ref SpriteFont facilityFont,ref SpriteFont facilityFont2,ref Texture2D background,ref Texture2D upgradeButton,ref Texture2D playJigsawButton,ref Texture2D increaseAttributeButton)
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

            this.facilityFont = facilityFont;
            this.facilityFont2 = facilityFont2;
            this.background = background;
            this.upgradeButton = upgradeButton;
            this.playJigsawButton = playJigsawButton;
            this.increaseAttributeButton = increaseAttributeButton;
            
            backgroundRect = new Rectangle(game.Window.ClientBounds.Center.X - 500, game.Window.ClientBounds.Center.Y - 400, 1000, 800);
            bioUpgradeRect = new Rectangle(game.Window.ClientBounds.Center.X - 300, backgroundRect.Top + 400, 200, 50);
            plasticUpgradeRect = new Rectangle(game.Window.ClientBounds.Center.X + 100, backgroundRect.Top + 400, 200, 50);
            playSeaCowJigsawRect = new Rectangle(backgroundRect.Center.X - 400, backgroundRect.Bottom - 250, 200, 50);
            playTurtleJigsawRect = new Rectangle(backgroundRect.Center.X - 100, backgroundRect.Bottom - 250, 200, 50);
            playDolphinJigsawRect = new Rectangle(backgroundRect.Center.X + 200, backgroundRect.Bottom - 250, 200, 50);
            increaseAttributeRect = new Rectangle(backgroundRect.Right - 100, backgroundRect.Top+40, 50, 50);

            this.game = game;

            // Set up the parameters
            SetupShaderParameters(PoseidonGame.contentManager, Model);
            EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);

        }

        public void Update(GameTime gameTime, Vector3 botPosition, ref List<Point> points)
        {
            EffectHelpers.GetEffectConfiguration(ref fogColor, ref ambientColor, ref diffuseColor, ref specularColor);

            double processingTime = 8; //2 days
            int fossilType = random.Next(100);
            string point_string = "";
            bool boneFound = false;

            //Is strange rock finished processing?
            for (int i = 0; i < listTimeRockProcessing.Count; i++)
            {
                if (PoseidonGame.playTime.TotalSeconds - listTimeRockProcessing[i] >= processingTime)
                {
                    listTimeRockProcessing.RemoveAt(i);
                    //produce fossil/bone 60% of the times.
                    if (fossilType >= 80)
                    {
                        HydroBot.numTurtlePieces++;
                        point_string = "Research Centre found a Meiolania Turtle\nfossil from the strange rock";
                        boneFound = true;
                    }
                    else if (fossilType >= 60)
                    {
                        HydroBot.numDolphinPieces++;
                        point_string = "Research Centre found a Maui's Dolphin\nbone from the strange rock";
                        boneFound = true;
                    }
                    else if (fossilType >= 40)
                    {
                        HydroBot.numSeaCowPieces++;
                        boneFound = true;
                        point_string = "Research Centre found a Stellar's SeaCow\nbone from the strange rock";
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
            string title = "RESEARCH FACILITY";
            string description = "Welcome to the research facility. Scientists work here to upgrade processing factories. State of the art genetic engineering will help you resurrect extinct animals. You can also use your experience points to increase the bot's attributes.";
            title = Poseidon.Core.IngamePresentation.wrapLine(title, backgroundRect.Width, facilityFont, 1.5f);

            //draw background
            spriteBatch.Draw(background, backgroundRect, Color.White);

            //draw title string
            spriteBatch.DrawString(facilityFont, title, new Vector2(backgroundRect.Center.X - facilityFont.MeasureString(title).X * 0.75f, backgroundRect.Top + 30), Color.White, 0, new Vector2(0, 0), 1.5f, SpriteEffects.None, 0);

            //draw description
            description = Poseidon.Core.IngamePresentation.wrapLine(description, backgroundRect.Width - 100, facilityFont);
            spriteBatch.DrawString(facilityFont, description, new Vector2(backgroundRect.Left + 50, backgroundRect.Top + 100), Color.White);


            //draw factory upgrade heading:
            string upgradeTitle = "FACTORY UPGRADATION";
            spriteBatch.DrawString(facilityFont2, upgradeTitle, new Vector2(backgroundRect.Center.X - facilityFont2.MeasureString(upgradeTitle).X / 2, bioUpgradeRect.Top - 100), Color.White);

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
                    bioButtonText= "Congratulations! You've got the best technology.";
                    bioButtonText = Poseidon.Core.IngamePresentation.wrapLine(bioButtonText, (backgroundRect.Width / 2) - 200, facilityFont2);
                    spriteBatch.DrawString(facilityFont2, bioButtonText, new Vector2(bioUpgradeRect.Center.X - facilityFont2.MeasureString(bioButtonText).X / 2, bioUpgradeRect.Center.Y - facilityFont2.MeasureString(bioButtonText).Y / 2), Color.White);
                }
                else
                {
                    bioButtonText = "You need to process "+(GameConstants.numTrashForUpgrade-HydroBot.totalBioTrashProcessed).ToString()+" more trash for upgrade to level"+ (HydroBot.bioPlantLevel+1).ToString();
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
                    plasticButtonText = "Congratulations! You've got the best technology.";
                    plasticButtonText = Poseidon.Core.IngamePresentation.wrapLine(plasticButtonText, (backgroundRect.Width / 2) - 200, facilityFont2);
                    spriteBatch.DrawString(facilityFont2, plasticButtonText, new Vector2(plasticUpgradeRect.Center.X - facilityFont2.MeasureString(plasticButtonText).X / 2, plasticUpgradeRect.Center.Y - facilityFont2.MeasureString(plasticButtonText).Y / 2), Color.White);
                }
                else
                {
                    plasticButtonText = "You need to process " + (GameConstants.numTrashForUpgrade - HydroBot.totalPlasticTrashProcessed).ToString() + " more trash for upgrade to level"+ (HydroBot.plasticPlantLevel+1).ToString();
                    plasticButtonText = Poseidon.Core.IngamePresentation.wrapLine(plasticButtonText, (backgroundRect.Width / 2) - 200, facilityFont2);
                    spriteBatch.DrawString(facilityFont2, plasticButtonText, new Vector2(plasticUpgradeRect.Center.X - facilityFont2.MeasureString(plasticButtonText).X / 2, plasticUpgradeRect.Center.Y - facilityFont2.MeasureString(plasticButtonText).Y / 2), Color.White);
                }
            }

            //Draw Increase Attribute Button
            if (HydroBot.unassignedPts > 0)
            {
                if (mouseOnIncreaseAttributeIcon)
                {
                    string desc_text = "INCREASE BOT ATTRIBUTES";
                    desc_text = Poseidon.Core.IngamePresentation.wrapLine(desc_text, increaseAttributeRect.Width + 200, facilityFont2);
                    spriteBatch.Draw(increaseAttributeButton, increaseAttributeRect, Color.Red);
                    spriteBatch.DrawString(facilityFont2, desc_text, new Vector2(increaseAttributeRect.Center.X - facilityFont2.MeasureString(desc_text).X / 4, increaseAttributeRect.Bottom), Color.Gold, 0f, new Vector2(0, 0), 0.7f, SpriteEffects.None, 0f);
                }
                else
                {
                    spriteBatch.Draw(increaseAttributeButton, increaseAttributeRect, Color.White);
                }
            }

            //Draw resurrection title:
            string resurrectTitle = "EXTINCT ANIMAL RESURRECTION";
            spriteBatch.DrawString(facilityFont2, resurrectTitle, new Vector2(backgroundRect.Center.X - facilityFont2.MeasureString(resurrectTitle).X / 2, playSeaCowJigsawRect.Top - 50), Color.White);

            if (seaCowLost)
            {
                string lost_Str = "SORRY, RESURRECTION TIMED OUT. THE BONES CANNOT BE REUSED.";
                lost_Str = Poseidon.Core.IngamePresentation.wrapLine(lost_Str, playSeaCowJigsawRect.Width + 100, facilityFont2);
                spriteBatch.DrawString(facilityFont2, lost_Str, new Vector2(playSeaCowJigsawRect.Left - 50, playSeaCowJigsawRect.Top), Color.White);
            }
            else if (seaCowWon && HydroBot.seaCowPower > 1f)
            {
                string seacowText = "Congrats! Your Stellar SeaCow's power has been increased " + ((int)HydroBot.seaCowPower).ToString() + " times.";
                seacowText = Poseidon.Core.IngamePresentation.wrapLine(seacowText, playSeaCowJigsawRect.Width + 100, facilityFont2);
                spriteBatch.DrawString(facilityFont2, seacowText, new Vector2(playSeaCowJigsawRect.Left - 50, playSeaCowJigsawRect.Top), Color.White);
            }
            else if (seaCowWon)
            {
                string seacowText = "Congrats! You have resurrected the Stellar's SeaCow. It is your minion.";
                seacowText = Poseidon.Core.IngamePresentation.wrapLine(seacowText, playSeaCowJigsawRect.Width + 100, facilityFont2);
                spriteBatch.DrawString(facilityFont2, seacowText, new Vector2(playSeaCowJigsawRect.Left - 50, playSeaCowJigsawRect.Top), Color.White);
            }
            else if (HydroBot.hasSeaCow)
            {
                if (HydroBot.numSeaCowPieces >= GameConstants.boneCountForSeaCowJigsaw)
                {
                    //Draw Sea Cow Jigsaw Button
                    playSeaCowJigsaw = true;
                    string seacowJigsawButtonText = "STRENGTHEN STELLAR'S\nSEACOW";
                    spriteBatch.Draw(playJigsawButton, playSeaCowJigsawRect, Color.White);
                    spriteBatch.DrawString(menuSmall, seacowJigsawButtonText, new Vector2(playSeaCowJigsawRect.Center.X - menuSmall.MeasureString(seacowJigsawButtonText).X / 4, playSeaCowJigsawRect.Center.Y - menuSmall.MeasureString(seacowJigsawButtonText).Y / 4), Color.Red, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                }
                else
                {
                    string seacowText = "You need " + (GameConstants.boneCountForSeaCowJigsaw - HydroBot.numSeaCowPieces).ToString() + " more bones to strengthen Stellar's Seacow";
                    seacowText = Poseidon.Core.IngamePresentation.wrapLine(seacowText, playSeaCowJigsawRect.Width + 100, facilityFont2);
                    spriteBatch.DrawString(facilityFont2, seacowText, new Vector2(playSeaCowJigsawRect.Left - 50, playSeaCowJigsawRect.Top), Color.White);
                }
            }
            else
            {
                if (HydroBot.numSeaCowPieces >= GameConstants.boneCountForSeaCowJigsaw)
                {
                    //Draw Sea Cow Jigsaw Button
                    playSeaCowJigsaw = true;
                    string seacowJigsawButtonText = "RESURRECT STELLAR'S\nSEACOW";
                    spriteBatch.Draw(playJigsawButton, playSeaCowJigsawRect, Color.White);
                    spriteBatch.DrawString(menuSmall, seacowJigsawButtonText, new Vector2(playSeaCowJigsawRect.Center.X - menuSmall.MeasureString(seacowJigsawButtonText).X / 4, playSeaCowJigsawRect.Center.Y - menuSmall.MeasureString(seacowJigsawButtonText).Y / 4), Color.Red, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                }
                else if (HydroBot.numSeaCowPieces > 0)
                {
                    string seacowText = "You need " + (GameConstants.boneCountForSeaCowJigsaw - HydroBot.numSeaCowPieces).ToString() + " more bones to resurrect Stellar's Seacow";
                    seacowText = Poseidon.Core.IngamePresentation.wrapLine(seacowText, playSeaCowJigsawRect.Width + 100, facilityFont2);
                    spriteBatch.DrawString(facilityFont2, seacowText, new Vector2(playSeaCowJigsawRect.Left - 50, playSeaCowJigsawRect.Top), Color.White);
                }
            }

            if (turtleLost)
            {
                string lostStr = "SORRY, RESURRECTION TIMED OUT. THE FOSSILS CANNOT BE REUSED.";
                lostStr = Poseidon.Core.IngamePresentation.wrapLine(lostStr, playTurtleJigsawRect.Width + 100, facilityFont2);
                spriteBatch.DrawString(facilityFont2, lostStr, new Vector2(playTurtleJigsawRect.Left - 50, playTurtleJigsawRect.Top), Color.White);
            }
            else if (turtleWon && HydroBot.turtlePower>1f)
            {
                string turtleText = "Congrats! Your Meiolania Turtle's power has been increased " + ((int)HydroBot.turtlePower).ToString() + " times.";
                turtleText = Poseidon.Core.IngamePresentation.wrapLine(turtleText, playTurtleJigsawRect.Width + 100, facilityFont2);
                spriteBatch.DrawString(facilityFont2, turtleText, new Vector2(playTurtleJigsawRect.Left - 50, playTurtleJigsawRect.Top), Color.White);
            }
            else if (turtleWon)
            {
                string turtleText = "Congrats! You have resurrected the Meiolania Turtle. It is your minion.";
                turtleText = Poseidon.Core.IngamePresentation.wrapLine(turtleText, playTurtleJigsawRect.Width + 100, facilityFont2);
                spriteBatch.DrawString(facilityFont2, turtleText, new Vector2(playTurtleJigsawRect.Left - 50, playTurtleJigsawRect.Top), Color.White);
            }
            else if (HydroBot.hasTurtle)
            {
                if (HydroBot.numTurtlePieces >= GameConstants.boneCountForTurtleJigsaw)
                {
                    //Draw Sea Turtle Jigsaw Button
                    playTurtleJigsaw = true;
                    string turtleJigsawButtonText = "STRENGTHEN MEIOLANIA\nTURTLE";
                    spriteBatch.Draw(playJigsawButton, playTurtleJigsawRect, Color.White);
                    spriteBatch.DrawString(menuSmall, turtleJigsawButtonText, new Vector2(playTurtleJigsawRect.Center.X - menuSmall.MeasureString(turtleJigsawButtonText).X / 4, playTurtleJigsawRect.Center.Y - menuSmall.MeasureString(turtleJigsawButtonText).Y / 4), Color.Red, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                }
                else
                {
                    string turtleText = "You need " + (GameConstants.boneCountForTurtleJigsaw - HydroBot.numTurtlePieces).ToString() + " more fossils to strengthen Meiolania Turtle";
                    turtleText = Poseidon.Core.IngamePresentation.wrapLine(turtleText, playTurtleJigsawRect.Width + 100, facilityFont2);
                    spriteBatch.DrawString(facilityFont2, turtleText, new Vector2(playTurtleJigsawRect.Left - 50, playTurtleJigsawRect.Top), Color.White);
                }
            }
            else
            {
                if (HydroBot.numTurtlePieces >= GameConstants.boneCountForTurtleJigsaw)
                {
                    //Draw Sea Turtle Jigsaw Button
                    playTurtleJigsaw = true;
                    string turtleJigsawButtonText = "RESURRECT MEIOLANIA\nTURTLE";
                    spriteBatch.Draw(playJigsawButton, playTurtleJigsawRect, Color.White);
                    spriteBatch.DrawString(menuSmall, turtleJigsawButtonText, new Vector2(playTurtleJigsawRect.Center.X - menuSmall.MeasureString(turtleJigsawButtonText).X / 4, playTurtleJigsawRect.Center.Y - menuSmall.MeasureString(turtleJigsawButtonText).Y / 4), Color.Red, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                }
                else if (HydroBot.numTurtlePieces > 0)
                {
                    string turtleText = "You need " + (GameConstants.boneCountForTurtleJigsaw - HydroBot.numTurtlePieces).ToString() + " more fossils to resurrect Meiolania Turtle";
                    turtleText = Poseidon.Core.IngamePresentation.wrapLine(turtleText, playTurtleJigsawRect.Width + 100, facilityFont2);
                    spriteBatch.DrawString(facilityFont2, turtleText, new Vector2(playTurtleJigsawRect.Left - 50, playTurtleJigsawRect.Top), Color.White);
                }
            }

            if (dolphinLost)
            {
                string lostStr = "SORRY, RESURRECTION TIMED OUT. THE BONES CANNOT BE REUSED.";
                lostStr = Poseidon.Core.IngamePresentation.wrapLine(lostStr, playDolphinJigsawRect.Width + 100, facilityFont2);
                spriteBatch.DrawString(facilityFont2, lostStr, new Vector2(playDolphinJigsawRect.Left - 50, playDolphinJigsawRect.Top), Color.White);
            }
            else if (dolphinWon && HydroBot.dolphinPower>1f)
            {
                string turtleText = "Congrats! Your Maui dolphin's strength has been increased " + (int)HydroBot.dolphinPower + " times.";
                turtleText = Poseidon.Core.IngamePresentation.wrapLine(turtleText, playDolphinJigsawRect.Width + 100, facilityFont2);
                spriteBatch.DrawString(facilityFont2, turtleText, new Vector2(playDolphinJigsawRect.Left - 50, playDolphinJigsawRect.Top), Color.White);
            }
            else if (dolphinWon)
            {
                string dolphinText = "Congrats! You have resurrected the Maui's dolphin. It is your minion.";
                dolphinText = Poseidon.Core.IngamePresentation.wrapLine(dolphinText, playDolphinJigsawRect.Width + 100, facilityFont2);
                spriteBatch.DrawString(facilityFont2, dolphinText, new Vector2(playDolphinJigsawRect.Left - 50, playDolphinJigsawRect.Top), Color.White);
            }
            else if (HydroBot.hasDolphin)
            {
                if (HydroBot.numDolphinPieces >= GameConstants.boneCountForDolphinJigsaw)
                {
                    //Draw Sea Dolphin Jigsaw Button
                    playDolphinJigsaw = true;
                    string dolphinJigsawButtonText = "STRENGTHEN MAUI'S\nDOLPHIN";
                    spriteBatch.Draw(playJigsawButton, playDolphinJigsawRect, Color.White);
                    spriteBatch.DrawString(menuSmall, dolphinJigsawButtonText, new Vector2(playDolphinJigsawRect.Center.X - menuSmall.MeasureString(dolphinJigsawButtonText).X / 4, playDolphinJigsawRect.Center.Y - menuSmall.MeasureString(dolphinJigsawButtonText).Y / 4), Color.Red, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                }
                else
                {
                    string dolphinText = "You need " + (GameConstants.boneCountForDolphinJigsaw - HydroBot.numDolphinPieces).ToString() + " more bones to strengthen Maui's Dolphin";
                    dolphinText = Poseidon.Core.IngamePresentation.wrapLine(dolphinText, playDolphinJigsawRect.Width + 100, facilityFont2);
                    spriteBatch.DrawString(facilityFont2, dolphinText, new Vector2(playDolphinJigsawRect.Left - 50, playDolphinJigsawRect.Top), Color.White);
                }
            }
            else
            {
                if (HydroBot.numDolphinPieces >= GameConstants.boneCountForDolphinJigsaw)
                {
                    //Draw Sea Dolphin Jigsaw Button
                    playDolphinJigsaw = true;
                    string dolphinJigsawButtonText = "RESURRECT MAUI'S\nDOLPHIN";
                    spriteBatch.Draw(playJigsawButton, playDolphinJigsawRect, Color.White);
                    spriteBatch.DrawString(menuSmall, dolphinJigsawButtonText, new Vector2(playDolphinJigsawRect.Center.X - menuSmall.MeasureString(dolphinJigsawButtonText).X / 4, playDolphinJigsawRect.Center.Y - menuSmall.MeasureString(dolphinJigsawButtonText).Y / 4), Color.Red, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                }
                else if (HydroBot.numDolphinPieces > 0)
                {
                    string dolphinText = "You need " + (GameConstants.boneCountForDolphinJigsaw - HydroBot.numDolphinPieces).ToString() + " more bones to resurrect Maui's Dolphin";
                    dolphinText = Poseidon.Core.IngamePresentation.wrapLine(dolphinText, playDolphinJigsawRect.Width + 100, facilityFont2);
                    spriteBatch.DrawString(facilityFont2, dolphinText, new Vector2(playDolphinJigsawRect.Left - 50, playDolphinJigsawRect.Top), Color.White);
                }
            }
            if (!HydroBot.hasSeaCow && !HydroBot.hasTurtle && !HydroBot.hasDolphin && HydroBot.numSeaCowPieces == 0 && HydroBot.numDolphinPieces == 0 && HydroBot.numTurtlePieces == 0)
            {
                string txt = "Collect bones/fossils to resurrect extinct sea creatures. Bones and fossils are found while processing trash in the factories. They are also found in treasure chests inside shipwrecks.";
                txt = Poseidon.Core.IngamePresentation.wrapLine(txt, backgroundRect.Width - 100, facilityFont2);
                spriteBatch.DrawString(facilityFont2, txt, new Vector2(backgroundRect.Left + 50, playSeaCowJigsawRect.Top), Color.White);
            }

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
