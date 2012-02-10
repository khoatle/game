﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon.Core
{
    class ButtonPanel
    {
        private Vector2 topLeft;
        private Vector2 infoTextPosition;
        private int maxButtons = 4;
        private List<Button> buttons;
        private float scale = 1.0f;
        private Rectangle panelRectangle;
        private SpriteFont infoTextFont;
        private string[] buildingNames = new string[] { "Research Facility", "Radioactive Processing Facility", "Biodegradable Processing Facility", "Plastic Processing Facility" };

        // Currently Anchored button (-ve if none)
        private int anchoredIndex = -1;
        public int AnchoredIndex
        {
            get { return AnchoredIndex; }
        }

        public ButtonPanel(int buttonCount, float scaleFactor)
        {
            if (buttonCount > 0 && buttonCount <= GameConstants.FactoryPanelMaxButtons)
            {
                maxButtons = buttonCount;
            }
            scale = scaleFactor;
        }

        public bool hasAnyAnchor()
        {
            return anchoredIndex >= 0 && anchoredIndex < buttons.Count;
        }

        public void removeAnchor()
        {
            if (anchoredIndex >= 0)
            {
                buttons[anchoredIndex].Anchored = false;
            }
        }

        public bool cursorOutsidePanelArea()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i].state != Button.InteractionState.OUT)
                {
                    return false;
                }
            }
            return true;
        }

        public BuildingType anchorIndexToBuildingType()
        {
            BuildingType anchoredBuildingType = BuildingType.biodegradable;
            // following switch case could be replaced by an array mapping between index and building type
            switch (anchoredIndex)
            {
                case 0:
                    anchoredBuildingType = BuildingType.researchlab;
                    break;
                case 1:
                    anchoredBuildingType = BuildingType.radioactive;
                    break;
                case 2:
                    anchoredBuildingType = BuildingType.biodegradable;
                    break;
                case 3:
                    anchoredBuildingType = BuildingType.plastic;
                    break;
            }
            return anchoredBuildingType;
        }

         public void Initialize(Texture2D buttonTexture, SpriteFont font, Vector2 position)
        {
            infoTextFont = font;
            topLeft = position;
            panelRectangle = new Rectangle((int)topLeft.X, (int)topLeft.Y, (int)(maxButtons * 64 * scale), 64);
            infoTextPosition = new Vector2(panelRectangle.X, panelRectangle.Y);
            buttons = new List<Button>();
            for (int i = 0; i < maxButtons; i++)
            {
                Button btn = new Button();
                btn.Initialize(i, buildingNames[i], buttonTexture, new Vector2(position.X + i * 64 * scale, position.Y + (64 - scale * 64)), 64, 64, scale);
                buttons.Add(btn);
            }
        }

        public void Update(GameTime gameTime, MouseState mouseState)
        {
            // this function ensures that only one button has an anchor at one time
            anchoredIndex = -1;
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Update(gameTime, mouseState, panelRectangle.Contains(mouseState.X, mouseState.Y));
                if (buttons[i].Anchored)
                {
                    anchoredIndex = i;
                }
                if (buttons[i].state != Button.InteractionState.OUT)
                {
                    // This condition will satisfy for at most one button
                    infoTextPosition.X = buttons[i].TopLeft.X;
                    infoTextPosition.Y = buttons[i].TopLeft.Y - 5;
                }
                
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Draw(spriteBatch);
                if (buttons[i].state != Button.InteractionState.OUT)
                {
                    // display info text
                    String displayText = buttons[i].Name + "\nRequired  : " + GameConstants.numResourcesForEachFactory + " resources\nAvailable : " + HydroBot.numResources + " resources";
                    infoTextPosition.Y -= (int)infoTextFont.MeasureString(displayText).Y;
                    spriteBatch.DrawString(infoTextFont, displayText, infoTextPosition, Color.Red);
                }
            }
        }

        public void DrawAnchor(SpriteBatch spriteBatch)
        {
            if (hasAnyAnchor() && cursorOutsidePanelArea())
            {
                buttons[anchoredIndex].DrawAnchor(spriteBatch);
            }
        }
    }
}
