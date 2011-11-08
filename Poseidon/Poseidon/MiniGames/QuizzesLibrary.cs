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

            quizz = new Quizz();
            quizz.question = "Algae is vanishing due to ocean warming. It is good or bad?";
            quizz.options[0] = "Good, because they are ugly.";
            quizz.options[1] = "Good, because they are dirt.";
            quizz.options[2] = "Good, because there's plenty in my backyard.";
            quizz.options[3] = "Bad, it produces food for marine life by photosynthesis.";
            quizz.answerID = 3;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Why are corals important?";
            quizz.options[0] = "They are leading source of ocean's food and livelihood.";
            quizz.options[1] = "They are good tourist attraction.";
            quizz.options[2] = "They make good jewelry.";
            quizz.options[3] = "They are completely useless.";
            quizz.answerID = 0;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Why is our planet called the 'blue planet'?";
            quizz.options[0] = "Blue is a popular shirt color.";
            quizz.options[1] = "The sky is blue.";
            quizz.options[2] = "70% of our planet is covered with water.";
            quizz.options[3] = "It is called the red planet.";
            quizz.answerID = 2;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Which of the following causes global warming?";
            quizz.options[0] = "Burning of fossil fuels.";
            quizz.options[1] = "Deforestation.";
            quizz.options[2] = " Methane emissions from animals and agriculture.";
            quizz.options[3] = "All of the above.";
            quizz.answerID = 3;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Which of the following is caused by global warming?";
            quizz.options[0] = "Dissapearance of coral reefs.";
            quizz.options[1] = "Widespread extinction of species.";
            quizz.options[2] = "More killer storms.";
            quizz.options[3] = "All of the above.";
            quizz.answerID = 3;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Why is Maldives called the sinking nation?";
            quizz.options[0] = "Scuba diving is very popular there.";
            quizz.options[1] = "It may submerge under water very soon.";
            quizz.options[2] = "Their officials have underwater meetings.";
            quizz.options[3] = "All of the above.";
            quizz.answerID = 1;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Why is it good to eat less sea food?";
            quizz.options[0] = "It will reduce over-expoitation of sea-animals.";
            quizz.options[1] = "Eat more chicken.";
            quizz.options[2] = "Sea food stinks.";
            quizz.options[3] = "They don't have protein.";
            quizz.answerID = 0;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Which of these activities harm the ocean?";
            quizz.options[0] = "Oil Drilling.";
            quizz.options[1] = "Swimming.";
            quizz.options[2] = "Surfing.";
            quizz.options[3] = "Rafting.";
            quizz.answerID = 0;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "If the Earth's ocean suddenly dried up, what might be the primary impact on the humans?";
            quizz.options[0] = "The Earth's food supply would increase.";
            quizz.options[1] = "The Ocean floor would be mined for gold.";
            quizz.options[2] = "The fresh water supply would dwindle.";
            quizz.options[3] = "The ocean floor could be used as farmland.";
            quizz.answerID = 2;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Which area of ocean has experienced maximum habitat loss?";
            quizz.options[0] = "Coasts.";
            quizz.options[1] = "Deep Sea.";
            quizz.options[2] = "Surface.";
            quizz.options[3] = "None.";
            quizz.answerID = 0;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Baby seahorses hatch and develop in a pouch on their _____'s body. Fill in the blank?";
            quizz.options[0] = "Mother.";
            quizz.options[1] = "Sister.";
            quizz.options[2] = "Boyfriend.";
            quizz.options[3] = "Father.";
            quizz.answerID = 3;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Flying fish ____. Fill in the blank?";
            quizz.options[0] = "Swim faster than other fish.";
            quizz.options[1] = "glide.";
            quizz.options[2] = "fly with the kites.";
            quizz.options[3] = "There's no such thing.";
            quizz.answerID = 1;
            quizzesList.Add(quizz);
            
            quizz = new Quizz();
            quizz.question = "Where would you most likely find a sting ray?";
            quizz.options[0] = "On the tree.";
            quizz.options[1] = "In the beach.";
            quizz.options[2] = "On the ocean floor.";
            quizz.options[3] = "All of the above.";
            quizz.answerID =2;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "What fish has rodlike growths with a fleshy tip on its head?";
            quizz.options[0] = "Anglerfish.";
            quizz.options[1] = "Redeye Salmon.";
            quizz.options[2] = "Hammer Shark.";
            quizz.options[3] = "Red Snapper";
            quizz.answerID = 0;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "What fish has whisker-like barbels on it's face?";
            quizz.options[0] = "Anglerfish.";
            quizz.options[1] = "Catfish.";
            quizz.options[2] = "Sardine.";
            quizz.options[3] = "Snapper.";
            quizz.answerID = 1;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Dolphins breathe through their _____?";
            quizz.options[0] = "Fins.";
            quizz.options[1] = "Skin.";
            quizz.options[2] = "Lungs.";
            quizz.options[3] = "Nose.";
            quizz.answerID = 2;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "The starfish generally have 5 identical members which are what?";
            quizz.options[0] = "Arms.";
            quizz.options[1] = "Legs.";
            quizz.options[2] = "Nose.";
            quizz.options[3] = "Eyes.";
            quizz.answerID = 0;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Shark is an excellent swimmer. What happens when it isn't swimming?";
            quizz.options[0] = "It flies.";
            quizz.options[1] = "It floats.";
            quizz.options[2] = "It glides.";
            quizz.options[3] = "It sinks to the bottom.";
            quizz.answerID = 3;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "A sea porcupine is difficult for another animal to eat because of its sharp what?";
            quizz.options[0] = "Teeth.";
            quizz.options[1] = "Spines.";
            quizz.options[2] = "Arms.";
            quizz.options[3] = "Hairs.";
            quizz.answerID = 1;
            quizzesList.Add(quizz);
        }
    }
}
