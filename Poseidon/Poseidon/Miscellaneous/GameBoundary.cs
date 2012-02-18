#region File Description
//-----------------------------------------------------------------------------
// SampleGrid.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace Poseidon
{
    public class GameBoundary
    {
        #region Fields
        private Color gridColor;
        private Vector3 diffuseColor;
        private bool decreasing;
        // Rendering
        private VertexBuffer vertexBuffer;
        private BasicEffect effect;
        private GraphicsDevice device;
        #endregion

        #region Constructors and Loading
        public GameBoundary()
        {
            gridColor = new Color(0xFF, 0xFF, 0xFF, 0xFF);
            diffuseColor = new Vector3(1, 1, 1);
            decreasing = true;
        }

        public void LoadGraphicsContent(GraphicsDevice graphicsDevice)
        {
            device = graphicsDevice;

            effect = new BasicEffect(device);

            VertexPositionColor[] vertices = new VertexPositionColor[5];

            vertices[0] = new VertexPositionColor(new Vector3(-GameConstants.MainGameMaxRangeX, 0, GameConstants.MainGameMaxRangeZ), this.gridColor);
            vertices[1] = new VertexPositionColor(new Vector3(GameConstants.MainGameMaxRangeX, 0, GameConstants.MainGameMaxRangeZ), this.gridColor);
            vertices[2] = new VertexPositionColor(new Vector3(GameConstants.MainGameMaxRangeX, 0, -GameConstants.MainGameMaxRangeZ), this.gridColor);
            vertices[3] = new VertexPositionColor(new Vector3(-GameConstants.MainGameMaxRangeX, 0, -GameConstants.MainGameMaxRangeZ), this.gridColor);
            vertices[4] = new VertexPositionColor(new Vector3(-GameConstants.MainGameMaxRangeX, 0, GameConstants.MainGameMaxRangeZ), this.gridColor);


            this.vertexBuffer = new VertexBuffer(device, typeof(VertexPositionColor),
                                                 5,
                                                 BufferUsage.WriteOnly);
            this.vertexBuffer.SetData<VertexPositionColor>(vertices);
        }


        #endregion

        #region Drawing
        public void Draw(Matrix view, Matrix projection)
        {
            effect.World = Matrix.Identity;
            effect.View = view;
            effect.Projection = projection;
            effect.VertexColorEnabled = true;
            effect.LightingEnabled = false;
            effect.DiffuseColor = diffuseColor;
            if (decreasing)
            {
                diffuseColor.Y -= 0.01f;
                diffuseColor.Z -= 0.01f;
                if (diffuseColor.Y <= 0) decreasing = false;
            }
            else
            {
                diffuseColor.Y += 0.01f;
                diffuseColor.Z += 0.01f;
                if (diffuseColor.Y >= 1) decreasing = true;
            }

            device.SetVertexBuffer(this.vertexBuffer);
            
            for (int i = 0; i < this.effect.CurrentTechnique.Passes.Count; ++i)
            {
                this.effect.CurrentTechnique.Passes[i].Apply();
                device.DrawPrimitives(PrimitiveType.LineStrip, 0, 4);
                //device.DrawIndexedPrimitives(PrimitiveType.LineStrip, 0, 0, 3, 0, 2);
            }
        }
        #endregion
    }
}
