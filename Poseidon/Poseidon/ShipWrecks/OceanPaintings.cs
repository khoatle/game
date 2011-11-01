using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace Poseidon
{
    public class Painting
    {
        public Texture2D painting;
        public string caption;
        public string tip;
        public Color color;
    }
    public class OceanPaintings
    {
        public List<Painting> paintings = new List<Painting>();
        public OceanPaintings(ContentManager content)
        {
            Painting painting = new Painting();
            painting.painting = content.Load<Texture2D>("Image/Paintings/painting1");
            painting.caption = "Painting of coral reef (dated 1799)";
            painting.tip = "Reducing your carbon footprint, conserving water, eating organic food, being vegetarian, and buying nontoxic products can help lessen global warming, which is causing water temperatures to rise, upsetting the oceans' delicate balance.";
            painting.color = Color.Orange;
            paintings.Add(painting);

            painting = new Painting();
            painting.painting = content.Load<Texture2D>("Image/Paintings/painting2");
            painting.caption = "Painting of coral reef (dated 1809)";
            painting.tip = "Washing your car in the street use 60% more water than a commercial car wash. Also the detergent runoff ends up untreated in streams, lakes, and the ocean.";
            painting.color = Color.Orange;
            paintings.Add(painting);

            painting = new Painting();
            painting.painting = content.Load<Texture2D>("Image/Paintings/painting3");
            painting.caption = "Painting of coral reef (dated 1803)";
            painting.tip = "When you don't pick up after pets, the germs get into the ocean, and cause beach and shellfish-bed closures.";
            painting.color = Color.Orange;
            paintings.Add(painting);

            painting = new Painting();
            painting.painting = content.Load<Texture2D>("Image/Paintings/painting4");
            painting.caption = "Painting of coral reef (dated 1779)";
            painting.tip = "Plastic bags cause the deaths of 100,000 marine animals each year when the animals mistake them for food, so if you must use them, always recycle them in the bin at your supermarket.";
            painting.color = Color.Orange;
            paintings.Add(painting);

            painting = new Painting();
            painting.painting = content.Load<Texture2D>("Image/Paintings/painting5");
            painting.caption = "Painting of coral reef (dated 1853)";
            painting.tip = "According to Greenpeace, about 10 million tons of plastic ends up in the ocean annually; much of it has collected in a spiral in the north Pacific. The garbage-vortex is the size of Texas, and it's not getting any smaller.";
            painting.color = Color.Orange;
            paintings.Add(painting);

            painting = new Painting();
            painting.painting = content.Load<Texture2D>("Image/Paintings/painting6");
            painting.caption = "Painting of coral reef (dated 1867)";
            painting.tip = "Air pollution contributes to water pollution and increases acidity in oceans and lakes. You can reduce your output by avoiding aerosols and driving less, for starters.";
            painting.color = Color.Gold;
            paintings.Add(painting);

            painting = new Painting();
            painting.painting = content.Load<Texture2D>("Image/Paintings/painting7");
            painting.caption = "Painting of coral reef (dated 1812)";
            painting.tip = "Hormones, antidepressants, painkillers, and other drugs are showing up in our water supply and harming aquatic life. Crush unused pills and throw them away in kitty litter, used coffee grounds, or other unpalatable items.";
            painting.color = Color.White;
            paintings.Add(painting);

            painting = new Painting();
            painting.painting = content.Load<Texture2D>("Image/Paintings/painting8");
            painting.caption = "Painting of coral reef (dated 1836)";
            painting.tip = "Antibacterial soap's most common ingredient, triclosan, is not completely removed during waste-water treatment, and is toxic to marine organisms.";
            painting.color = Color.Orange;
            paintings.Add(painting);

            painting = new Painting();
            painting.painting = content.Load<Texture2D>("Image/Paintings/painting9");
            painting.caption = "Painting of coral reef (dated 1875)";
            painting.tip = "Soap is harmful for sea animals. Only three parts per million of soap can kill sea urchin embryos, for example. Don't use soap in or near open water";
            painting.color = Color.White;
            paintings.Add(painting);

            painting = new Painting();
            painting.painting = content.Load<Texture2D>("Image/Paintings/painting10");
            painting.caption = "Painting of coral reef (dated 1909)";
            painting.tip = "80% of all life on Earth is found in the oceans and those same maltreated oceans provide vital sources of protein, energy, and minerals.";
            painting.color = Color.Orange;
            paintings.Add(painting);

            painting = new Painting();
            painting.painting = content.Load<Texture2D>("Image/Paintings/painting11");
            painting.caption = "Painting of coral reef (dated 1956)";
            painting.tip = "The rolling of the sea across the planet creates over half our oxygen, drives weather systems and natural flows of energy and nutrients around the world, transports water masses many times greater than all the rivers on land combined, and keeps the Earth habitable.";
            painting.color = Color.Orange;
            paintings.Add(painting);

            painting = new Painting();
            painting.painting = content.Load<Texture2D>("Image/Paintings/painting12");
            painting.caption = "Painting of coral reef (dated 1947)";
            painting.tip = "Offshore oil drilling results in a wide range of health and reproductive problems for fish and other marine life, exposes wildlife to the threat of oil spills, and destroys kelp beds, reefs, and coastal wetlands.";
            painting.color = Color.Orange;
            paintings.Add(painting);
        }
    }
}
