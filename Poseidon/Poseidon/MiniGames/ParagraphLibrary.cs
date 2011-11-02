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
            Paragraph paragraph = new Paragraph("This is paragraph 1");
            paragraphLib.Add(paragraph);

            paragraph = new Paragraph("This is paragraph 2");
            paragraphLib.Add(paragraph);

            paragraph = new Paragraph("This is paragraph 3");
            paragraphLib.Add(paragraph);

            paragraph = new Paragraph("This is paragraph 4");
            paragraphLib.Add(paragraph);

            paragraph = new Paragraph("This is paragraph 5");
            paragraphLib.Add(paragraph);

            paragraph = new Paragraph("This is paragraph 6");
            paragraphLib.Add(paragraph);

            paragraph = new Paragraph("This is paragraph 7");
            paragraphLib.Add(paragraph);

            paragraph = new Paragraph("This is paragraph 8");
            paragraphLib.Add(paragraph);

            paragraph = new Paragraph("This is paragraph 9");
            paragraphLib.Add(paragraph);

            paragraph = new Paragraph("This is paragraph 10");
            paragraphLib.Add(paragraph);

            paragraph = new Paragraph("This is paragraph 11");
            paragraphLib.Add(paragraph);

        }
    }
}
