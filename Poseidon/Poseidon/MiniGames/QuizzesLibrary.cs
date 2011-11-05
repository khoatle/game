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
            quizz.options[0] = "The germs can get into the ocean and harm sea creatures";
            quizz.options[1] = "It might spread disease";
            quizz.options[2] = "Someone might walk over it";
            quizz.options[3] = "All of the above";
            quizz.answerID = 3;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Plastic bags does not hurt the ocean.";
            quizz.options[0] = "True";
            quizz.options[1] = "It kills 100,000 marine animals each year";
            quizz.options[2] = "It's food for the fishes";
            quizz.options[3] = "Plastic disolves in water";
            quizz.answerID = 1;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "According to Greenpeace, about 10 million tons of plastic ends up in the ocean annually; much of it has collected in a spiral in the north Pacific. How big is it?";
            quizz.options[0] = "The size of Texas";
            quizz.options[1] = "That's not true";
            quizz.options[2] = "10 meters";
            quizz.options[3] = "Not more than a mile";
            quizz.answerID = 0;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Why are dolphins cool?";
            quizz.options[0] = "They are the most intelligent animals";
            quizz.options[1] = "They are playful";
            quizz.options[2] = "They are very friendly";
            quizz.options[3] = "All of the above";
            quizz.answerID = 3;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Who have 360 degree binocular vision and can detect an electrical signal of half a billionth of a volt?";
            quizz.options[0] = "Humans";
            quizz.options[1] = "Dogs";
            quizz.options[2] = "Tigers";
            quizz.options[3] = "Hammerhead Shark";
            quizz.answerID = 3;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "What is the main threat to extinction of hammerhead sharks?";
            quizz.options[0] = "Excessive fishing for their fins";
            quizz.options[1] = "They are eaten by tilapia";
            quizz.options[2] = "They commit suicide";
            quizz.options[3] = "They hammer each other";
            quizz.answerID = 0;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Which shark often swim with other sharks, but you can easily spot it?";
            quizz.options[0] = "Tiger shark";
            quizz.options[1] = "Angel shark";
            quizz.options[2] = "Leopard shark";
            quizz.options[3] = "Goblin shark";
            quizz.answerID = 2;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "How many sharks on an average are hunted every year?";
            quizz.options[0] = "About 100";
            quizz.options[1] = "None";
            quizz.options[2] = "Over 30 million";
            quizz.options[3] = "About 1000";
            quizz.answerID = 2;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "How does global warming effect the seals?";
            quizz.options[0] = "They'll be happy if the arctics get warmer";
            quizz.options[1] = "They need the arctic ice for their habitat";
            quizz.options[2] = "They'll migrate to colder places";
            quizz.options[3] = "We can keep them in the aquarium";
            quizz.answerID = 1;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "What is the common name for orca?";
            quizz.options[0] = "Ocra";
            quizz.options[1] = "Killer whale";
            quizz.options[2] = "Tiger whale";
            quizz.options[3] = "Cute whale";
            quizz.answerID = 1;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "I have electro-receptors, my eyes are on top, and I can sting real bad. Who am I?";
            quizz.options[0] = "Cat";
            quizz.options[1] = "Monkey";
            quizz.options[2] = "Dinosaur";
            quizz.options[3] = "Sting ray";
            quizz.answerID = 3;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "What is manetee commonly known as?";
            quizz.options[0] = "Sea Buffalo";
            quizz.options[1] = "Sea Cow";
            quizz.options[2] = "Sea Ox";
            quizz.options[3] = "Sea Bull";
            quizz.answerID = 1;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Who drink salt water and cry to get rid of the salt?";
            quizz.options[0] = "Turtles";
            quizz.options[1] = "Girls";
            quizz.options[2] = "Babies";
            quizz.options[3] = "None of the above";
            quizz.answerID = 0;
            quizzesList.Add(quizz);
        }
    }
}
