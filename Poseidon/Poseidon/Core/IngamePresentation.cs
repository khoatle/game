using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Poseidon.Core
{
    public static class IngamePresentation
    {
        public static void DrawActiveSkill(GraphicsDevice GraphicDevice, Texture2D[] skillTextures, SpriteBatch spriteBatch)
        {
            int xOffsetText, yOffsetText;
            Rectangle rectSafeArea;

            //Calculate str1 position
            rectSafeArea = GraphicDevice.Viewport.TitleSafeArea;

            //xOffsetText = rectSafeArea.Right - 400;
            xOffsetText = rectSafeArea.Center.X + 150;
            yOffsetText = rectSafeArea.Bottom - 100;

            //Vector2 skillIconPosition =
            //    new Vector2((int)xOffsetText, (int)yOffsetText);
            Rectangle destRectangle = new Rectangle(xOffsetText, yOffsetText, 96, 96);

            //spriteBatch.Draw(skillTextures[tank.activeSkillID], skillIconPosition, Color.White);
            spriteBatch.Draw(skillTextures[HydroBot.activeSkillID], destRectangle, Color.White);

            //draw the 2nd skill icon if skill combo activated
            if (HydroBot.skillComboActivated && HydroBot.secondSkillID != -1)
            {
                destRectangle = new Rectangle(xOffsetText + 100, yOffsetText, 96, 96);
                spriteBatch.Draw(skillTextures[HydroBot.secondSkillID], destRectangle, Color.White);
            }
        }
    }
}
