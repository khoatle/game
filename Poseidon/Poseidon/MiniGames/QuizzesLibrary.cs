using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poseidon.MiniGames
{
    public class Quizz
    {
        public string question;
        public string[] options = new string[4];
        public int answerID;
    }
    public class QuizzesLibrary
    {
        public List<Quizz> quizzesList = new List<Quizz>();
        public QuizzesLibrary()
        {
            Quizz quizz = new Quizz();
            quizz.question = "Which is the ocean friendly way to wash your car?";
            quizz.options[0] = "Wash it on open street";
            quizz.options[1] = "Let the detergent run off the street";
            quizz.options[2] = "Use a commercial car wash";
            quizz.options[3] = "Drive it through the bayou";
            quizz.answerID = 2;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Why should you pick up after your pet?";
            quizz.options[0] = "The germs get into the ocean and harm sea creatures";
            quizz.options[1] = "The locality smells bad";
            quizz.options[2] = "Someone might walk over it";
            quizz.options[3] = "All of the above";
            quizz.answerID = 3;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Plastic bags does not hurt the ocean.";
            quizz.options[0] = "True";
            quizz.options[1] = "It kills 100,000 marine animals each year";
            quizz.options[2] = "It's food for the fishes";
            quizz.options[3] = "It disolves in water";
            quizz.answerID = 2;
            quizzesList.Add(quizz);

            //quizz = new Quizz();
            //quizz.question = "Should we eat humans?";
            //quizz.options[0] = "Hell ya!";
            //quizz.options[1] = "Let's do it!";
            //quizz.options[2] = "Why not?";
            //quizz.options[3] = "We should";
            //quizz.answerID = 2;
            //quizzesList.Add(quizz);


        }
    }
}
