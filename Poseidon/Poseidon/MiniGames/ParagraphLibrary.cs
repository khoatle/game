using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poseidon.MiniGames
{
    public class Paragraph
    {
        public string content;
        public Paragraph(string content)
        {
            this.content = content;
        }
    }
    public class ParagraphLibrary
    {
        public List<Paragraph> paragraphLib = new List<Paragraph>();
        public ParagraphLibrary()
        {
            Paragraph paragraph = new Paragraph("Seen from space the Earth is covered in a blue mantle. It is a planet on which the continents are dwarfed by the oceans surrounding them and the immensity of the marine realm.");
            paragraphLib.Add(paragraph);

            paragraph = new Paragraph("When we don't put our trash in the proper place, it often ends up in the ocean. How does this happen? Many times trash blows around on the ground and ends up in storm drains, rivers and streams which carry the trash directly to the ocean.");
            paragraphLib.Add(paragraph);

            paragraph = new Paragraph("Marine debris is more than just ugly - it's a serious problem that affects the wildlife, habitat and water quality of all of the world's inter-connected ocean and waterways. To learn more about marine debris and what you can do, please visit www.marinedebris.noaa.gov");
            paragraphLib.Add(paragraph);

            paragraph = new Paragraph("Ocean litter has many sources, from boats and oil rigs on the water to picnickers, fisherman, and beachgoers along the shore. Cigarettes, buckets, gloves, rugs, tires, shoes, diapers, you can find it all down there.");
            paragraphLib.Add(paragraph);

            paragraph = new Paragraph("It takes just a minute for an item to be carelessly discarded or blown by the wind into the ocean, but it can take many years for that item to completely decompose. A glass bottle will never decompose, while plastic bottle may takes 450 years.");
            paragraphLib.Add(paragraph);

            paragraph = new Paragraph("Because I want to help the ocean, I pledge to fo these things: conserve water, reduce waste, dispose trash properly, be considerate to ocean wildlife and do community service to clean up beaches and rivers. I will also tell my friends to do the same.");
            paragraphLib.Add(paragraph);

            paragraph = new Paragraph("There are fewer than 1500 Hawaiian monk seals left in the world. Baby monk seals (called pups) make a mwaa-mwaa sound when calm and a loud gaah when scared. Adults make a bubbling sound when alarmed. If this continues, our children will be eating jellyfish.");
            paragraphLib.Add(paragraph);

            paragraph = new Paragraph("The reality of modern fishing is that the industry is dominated by fishing vessels that far out-match nature's ability to replenish fish.");
            paragraphLib.Add(paragraph);

            paragraph = new Paragraph("Modern fishing practices are incredibly wasteful. Every year, fishing nets kill up to 300,000 whales, dolphins and porpoises. Some fishing practices destroy habitat. Bottom trawling, for example, destroys entire ancient deep-sea coral forests and other delicate ecosystems.");
            paragraphLib.Add(paragraph);

            paragraph = new Paragraph("Governments must set aside 40 percent of our oceans as marine reserves. Marine reserves can be defined as areas of the ocean in which the exploitation of all living resources is prevented, together with the exploitation of non-living resources like minerals.");
            paragraphLib.Add(paragraph);

            paragraph = new Paragraph("Scientists say that global warming, by increasing sea water temperatures, will raise sea levels and change ocean currents. The effects are already beginning to be felt. Whole species of marine animals and fish are at risk due to the temperature rise.");
            paragraphLib.Add(paragraph);

        }
    }
}
