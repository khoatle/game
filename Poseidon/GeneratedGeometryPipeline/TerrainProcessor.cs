#region File Description
//-----------------------------------------------------------------------------
// TerrainProcessor.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.ComponentModel;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System;
#endregion

namespace GeneratedGeometryPipeline
{
    /// <summary>
    /// Custom content processor for creating terrain meshes. Given an
    /// input heightfield texture, this processor uses the MeshBuilder
    /// class to programatically generate terrain geometry.
    /// </summary>
    [ContentProcessor]
    public class TerrainProcessor : ContentProcessor<Texture2DContent, ModelContent>
    {
        #region Properties


        private float terrainScale = 38.0f;
        [DisplayName("Terrain Scale")]
        [DefaultValue(30f)]
        [Description("Scale of the the terrain geometry width and length.")]
        public float TerrainScale
        {
            get { return terrainScale; }
            set { terrainScale = value; }
        }

        private float terrainBumpiness = 640f;
        [DisplayName("Terrain Bumpiness")]
        [DefaultValue(640f)]
        [Description("Scale of the the terrain geometry height.")]
        public float TerrainBumpiness
        {
            get { return terrainBumpiness; }
            set { terrainBumpiness = value; }
        }

        private float texCoordScale = 0.125f;
        [DisplayName("Texture Coordinate Scale")]
        [DefaultValue(0.1f)]
        [Description("Terrain texture tiling density.")]
        public float TexCoordScale
        {
            get { return texCoordScale; }
            set { texCoordScale = value; }
        }

        private string terrainTextureFilename;
        [DisplayName("Terrain Texture")]
        [DefaultValue("rockmoss.jpg")]
        [Description("The name of the terrain texture.")]
        public string TerrainTextureFilename
        {
            get { return terrainTextureFilename; }
            set { terrainTextureFilename = value; }
        }


        #endregion


        private void SetTerrainTextureFilename(string terrainName)
        {
            //Random random = new Random();
            //int id = random.Next(6) + 1;
            if (terrainName == "terrain0.bmp")
                terrainTextureFilename = "Image/TerrainTextures/SeaBedTerrain1.jpg";
            else if (terrainName == "terrain1.bmp")
                terrainTextureFilename = "Image/TerrainTextures/SeaBedTerrain2.jpg";
            else if (terrainName == "terrain2.bmp")
                terrainTextureFilename = "Image/TerrainTextures/SeaBedTerrain3.jpg";
            else if (terrainName == "terrain3.bmp")
                terrainTextureFilename = "Image/TerrainTextures/SeaBedTerrain4.jpg";
            else if (terrainName == "terrain4.bmp")
                terrainTextureFilename = "Image/TerrainTextures/SeaBedTerrain5.jpg";
            else if (terrainName == "terrain5.bmp")
                terrainTextureFilename = "Image/TerrainTextures/SeaBedTerrain6.jpg";
            else if (terrainName == "terrain6.bmp")
                terrainTextureFilename = "Image/TerrainTextures/SeaBedTerrain1.jpg";
            else if (terrainName == "terrain7.bmp")
                terrainTextureFilename = "Image/TerrainTextures/SeaBedTerrain2.jpg";
            else if (terrainName == "terrain8.bmp")
                terrainTextureFilename = "Image/TerrainTextures/SeaBedTerrain3.jpg";
            else if (terrainName == "terrain9.bmp")
                terrainTextureFilename = "Image/TerrainTextures/SeaBedTerrain4.jpg";
            else if (terrainName == "terrain10.bmp")
                terrainTextureFilename = "Image/TerrainTextures/SeaBedTerrain5.jpg";
            else if (terrainName == "terrain11.bmp")
                terrainTextureFilename = "Image/TerrainTextures/SeaBedTerrain6.jpg";
            else terrainTextureFilename = "Image/TerrainTextures/SeaBedTerrain6.jpg";   
        }

        /// <summary>
        /// Generates a terrain mesh from an input heightfield texture.
        /// </summary>
        public override ModelContent Process(Texture2DContent input,
                                             ContentProcessorContext context)
        {
            string terrain_name = Path.GetFileName(input.Identity.SourceFilename);
            SetTerrainTextureFilename(terrain_name);

            MeshBuilder builder = MeshBuilder.StartMesh("terrain");

            // Convert the input texture to float format, for ease of processing.
            input.ConvertBitmapType(typeof(PixelBitmapContent<float>));

            PixelBitmapContent<float> heightfield;
            heightfield = (PixelBitmapContent<float>)input.Mipmaps[0];

            // Create the terrain vertices.
            for (int y = 0; y < heightfield.Height; y++)
            {
                for (int x = 0; x < heightfield.Width; x++)
                {
                    Vector3 position;

                    // position the vertices so that the heightfield is centered
                    // around x=0,z=0
                    position.X = terrainScale * (x - ((heightfield.Width - 1) / 2.0f));
                    position.Z = terrainScale * (y - ((heightfield.Height - 1) / 2.0f));

                    position.Y = (heightfield.GetPixel(x, y) - 1) * terrainBumpiness;

                    builder.CreatePosition(position);
                }
            }

            // Create a material, and point it at our terrain texture.
            BasicMaterialContent material = new BasicMaterialContent();
            material.SpecularColor = new Vector3(.4f, .4f, .4f);
           
            string directory = Path.GetDirectoryName(input.Identity.SourceFilename);
            string texture = Path.Combine("", terrainTextureFilename);
            //directory
            material.Texture = new ExternalReference<TextureContent>(texture);

            builder.SetMaterial(material);

            //EffectMaterialContent myMaterial = new EffectMaterialContent();
            //string effectPath = Path.GetFullPath("Shaders/NewBasicEffect.fx");
            //myMaterial.Effect = new ExternalReference<EffectContent>(effectPath);
            //myMaterial.Textures.Add("Texture", new ExternalReference<TextureContent>(texture));
            //myMaterial.OpaqueData.Add("SpecularColor", new Vector3(.4f, .4f, .4f));

            //builder.SetMaterial(myMaterial);

            // Create a vertex channel for holding texture coordinates.
            int texCoordId = builder.CreateVertexChannel<Vector2>(
                                            VertexChannelNames.TextureCoordinate(0));

            // Create the individual triangles that make up our terrain.
            for (int y = 0; y < heightfield.Height - 1; y++)
            {
                for (int x = 0; x < heightfield.Width - 1; x++)
                {
                    AddVertex(builder, texCoordId, heightfield.Width, x, y);
                    AddVertex(builder, texCoordId, heightfield.Width, x + 1, y);
                    AddVertex(builder, texCoordId, heightfield.Width, x + 1, y + 1);

                    AddVertex(builder, texCoordId, heightfield.Width, x, y);
                    AddVertex(builder, texCoordId, heightfield.Width, x + 1, y + 1);
                    AddVertex(builder, texCoordId, heightfield.Width, x, y + 1);
                }
            }

            // Chain to the ModelProcessor so it can convert the mesh we just generated.
            MeshContent terrainMesh = builder.FinishMesh();

            ModelContent model = context.Convert<MeshContent, ModelContent>(terrainMesh,
                                                              "ModelProcessor");

            // generate information about the height map, and attach it to the finished
            // model's tag.
            model.Tag = new HeightMapInfoContent(heightfield, terrainScale,
                terrainBumpiness);

            return model;
        }


        /// <summary>
        /// Helper for adding a new triangle vertex to a MeshBuilder,
        /// along with an associated texture coordinate value.
        /// </summary>
        void AddVertex(MeshBuilder builder, int texCoordId, int w, int x, int y)
        {
            builder.SetVertexChannelData(texCoordId, new Vector2(x, y) * texCoordScale);

            builder.AddTriangleVertex(x + y * w);
        }
    }
}
