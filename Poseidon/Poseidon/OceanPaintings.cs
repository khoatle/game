using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Poseidon
{
    public class Painting
    {
        public Texture2D painting;
        public string caption;
    }
    public class OceanPaintings
    {
        public List<Painting> paintings = new List<Painting>();
        public OceanPaintings(ContentManager content)
        {
            Painting painting = new Painting();
            painting.painting = content.Load<Texture2D>("Image/Paintings/painting1");
            painting.caption = "Coral reef in 1899 .... so beautiful, if only our ancester had protected it...";
            paintings.Add(painting);

            painting = new Painting();
            painting.painting = content.Load<Texture2D>("Image/Paintings/painting2");
            painting.caption = "Coral reef in 1900 .... so beautiful, if only our ancester had protected it...";
            paintings.Add(painting);

            painting = new Painting();
            painting.painting = content.Load<Texture2D>("Image/Paintings/painting3");
            painting.caption = "Coral reef in 1901 .... so beautiful, if only our ancester had protected it...";
            paintings.Add(painting);

            painting = new Painting();
            painting.painting = content.Load<Texture2D>("Image/Paintings/painting4");
            painting.caption = "Coral reef in 1902 .... so beautiful, if only our ancester had protected it...";
            paintings.Add(painting);

            painting = new Painting();
            painting.painting = content.Load<Texture2D>("Image/Paintings/painting5");
            painting.caption = "Coral reef in 1902 .... so beautiful, if only our ancester had protected it...";
            paintings.Add(painting);

            painting = new Painting();
            painting.painting = content.Load<Texture2D>("Image/Paintings/painting6");
            painting.caption = "Coral reef in 1902 .... so beautiful, if only our ancester had protected it...";
            paintings.Add(painting);

            painting = new Painting();
            painting.painting = content.Load<Texture2D>("Image/Paintings/painting7");
            painting.caption = "Coral reef in 1902 .... so beautiful, if only our ancester had protected it...";
            paintings.Add(painting);

            painting = new Painting();
            painting.painting = content.Load<Texture2D>("Image/Paintings/painting8");
            painting.caption = "Coral reef in 1902 .... so beautiful, if only our ancester had protected it...";
            paintings.Add(painting);

            painting = new Painting();
            painting.painting = content.Load<Texture2D>("Image/Paintings/painting9");
            painting.caption = "Coral reef in 1902 .... so beautiful, if only our ancester had protected it...";
            paintings.Add(painting);

            painting = new Painting();
            painting.painting = content.Load<Texture2D>("Image/Paintings/painting10");
            painting.caption = "Coral reef in 1902 .... so beautiful, if only our ancester had protected it...";
            paintings.Add(painting);
        }
    }
}
