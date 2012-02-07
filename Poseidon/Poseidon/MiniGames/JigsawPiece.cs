using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Poseidon.Core;

namespace Poseidon.MiniGames
{
    public class Piece
    {
        // The coordinate of the piece corresponding to the whole image
        public int trueRow;
        public int trueCol;

        // Top left position of the piece in pixel
        // It is to position where the piece is to be drawn
        public Vector2 topLeftPosition;

        // Initialize all attributes of a single Jigsaw piece
        public void initialize(int row, int col, float x, float y)
        {
            trueRow = row;
            trueCol = col;
            topLeftPosition = new Vector2(x, y);
        }

        // The draw method for a single Jigsaw Piece
        // This method will draw the piece, placing the "originOfTopLeftPiece" of the picture at (positionX, positionY)
        // It also automaticaly scale itself to desiredWidthPerPiece by desiredHeightPerPiece
        public void draw(SpriteBatch spriteBatch, Texture2D image, Vector2 origin,
            float desiredWidth, float desiredHeight, float trueWidth, float trueHeight)
        {

            // Scale the piece to the desired trueWidthOfPiece and trueHeightOfPiece
            Vector2 scaleFactor = new Vector2(desiredWidth / trueWidth, desiredHeight / trueHeight);
            // Get the "real" portion (piece) we want to draw
            Rectangle sourceRectangle = getTexelSourceRectangle(trueWidth, trueHeight);

            // Ready to draw
            spriteBatch.Begin();

            spriteBatch.Draw(image, topLeftPosition, sourceRectangle, Color.White, 0,
                origin, scaleFactor, SpriteEffects.None, 0);

            // End drawing
            spriteBatch.End();
        }

        // Is this piece correctly placed?
        protected bool isInPlace(int texelCoodinateX, int texelCoordinateY)
        {
            return trueRow == texelCoodinateX && trueCol == texelCoordinateY;
        }

        // Get the source rectangle of the piece, i.e what portion of the image we want to draw
        private Rectangle getTexelSourceRectangle(float trueWidth, float trueHeight)
        {
            // The order we access the pieces is (rowFrom, colFrom). But the coordinate is (X, Y) or (colFrom, rowFrom)
            return new Rectangle((int)(trueCol * trueWidth),
                (int)(trueRow * trueHeight), (int)trueWidth, (int)trueHeight);
        }
    }
}