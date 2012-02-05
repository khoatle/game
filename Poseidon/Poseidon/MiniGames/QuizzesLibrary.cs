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

            quizz = new Quizz();
            quizz.question = "Which is the largest creature the ever lived on this planet?";
            quizz.options[0] = "Dinosaur.";
            quizz.options[1] = "African Elephant.";
            quizz.options[2] = "Dragon.";
            quizz.options[3] = "Blue Whale.";
            quizz.answerID = 3;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "A whale is a ______";
            quizz.options[0] = "Mammal.";
            quizz.options[1] = "Fish.";
            quizz.options[2] = "Reptile.";
            quizz.options[3] = "Marsupial.";
            quizz.answerID = 0;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Is it possible to revive extinct species?";
            quizz.options[0] = "Yes, if its DNA can be found.";
            quizz.options[1] = "Yes, using a time machine.";
            quizz.options[2] = "Yes, only if we find their mummy.";
            quizz.options[3] = "Not possible at all.";
            quizz.answerID = 0;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "What causes ocean tides?";
            quizz.options[0] = "Earth's rotation.";
            quizz.options[1] = "Sun's gravitational pull.";
            quizz.options[2] = "Moon's gravitational pull.";
            quizz.options[3] = "All of the above.";
            quizz.answerID = 3;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Which is the largest ocean?";
            quizz.options[0] = "Indian Ocean.";
            quizz.options[1] = "Atlantic Ocean.";
            quizz.options[2] = "Pacific Ocean.";
            quizz.options[3] = "Arctic Ocean.";
            quizz.answerID = 2;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "The Bermuda Triangle is located in which ocean?";
            quizz.options[0] = "Indian Ocean.";
            quizz.options[1] = "Atlantic Ocean.";
            quizz.options[2] = "Pacific Ocean.";
            quizz.options[3] = "Arctic Ocean.";
            quizz.answerID = 1;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "How does plastic recycling help the environment?";
            quizz.options[0] = "It produces oxygen.";
            quizz.options[1] = "Prevents excessive landfills.";
            quizz.options[2] = "It makes the soil fertile.";
            quizz.options[3] = "Candies are made out of plastic.";
            quizz.answerID = 1;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "How are fossil fuels formed?";
            quizz.options[0] = "Decomposition of dead organisms for millions of years.";
            quizz.options[1] = "Melting of rocks.";
            quizz.options[2] = "Heating of soil.";
            quizz.options[3] = "Assembling of fossils.";
            quizz.answerID = 0;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Which of the following is a fossil fuel?";
            quizz.options[0] = "Coal.";
            quizz.options[1] = "Petroleum.";
            quizz.options[2] = "Natural Gas.";
            quizz.options[3] = "All of the above.";
            quizz.answerID = 3;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Greenhouse gases are bad because _______?";
            quizz.options[0] = "They are dark and ominous.";
            quizz.options[1] = "They smell bad.";
            quizz.options[2] = "They absorb and emit radiation.";
            quizz.options[3] = "All of the above.";
            quizz.answerID = 2;
            quizzesList.Add(quizz);
            
            quizz = new Quizz();
            quizz.question = "Which is the main source of greenhouse gases?";
            quizz.options[0] = "Combustion of fossil fuels.";
            quizz.options[1] = "Bonfires.";
            quizz.options[2] = "Forest fires.";
            quizz.options[3] = "Firecrackers.";
            quizz.answerID = 0;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Numbers within chasing arrow sign on plastic bottle denotes ____?";
            quizz.options[0] = "Type of plastic.";
            quizz.options[1] = "How many times it can be used.";
            quizz.options[2] = "How many times it can be recycled.";
            quizz.options[3] = "How many times it has been recycled.";
            quizz.answerID = 0;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "All plastic with a recycle sign is recycled, irrespective of its number.";
            quizz.options[0] = "True.";
            quizz.options[1] = "False, most recycling centers only recycle type 1 & 2.";
            quizz.options[2] = "Types doesn't matter. All kind of plastic can be melted together.";
            quizz.options[3] = "False, most of it is dumped.";
            quizz.answerID = 1;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "What are the challenges in recycling plastic?";
            quizz.options[0] = "Different types of plastic can not be recycled together.";
            quizz.options[1] = "Plastic recycling requires more processing.";
            quizz.options[2] = "Colors are hard to remove in plastic.";
            quizz.options[3] = "All of the above.";
            quizz.answerID = 3;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "What will reduce environment impact of plastics?";
            quizz.options[0] = "Reduce plastic use.";
            quizz.options[1] = "Reuse plastic containers.";
            quizz.options[2] = "Disposal in recycle trash containers.";
            quizz.options[3] = "All of the above.";
            quizz.answerID = 3;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Which country dumped radioactive waste in the ocean (till 1993)?";
            quizz.options[0] = "USA.";
            quizz.options[1] = "USSR.";
            quizz.options[2] = "UK.";
            quizz.options[3] = "All of the above.";
            quizz.answerID = 3;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Why can't nuclear waste be disposed in space?";
            quizz.options[0] = "Aliens will steal our technology.";
            quizz.options[1] = "It will contaminate other plants.";
            quizz.options[2] = "Failure of a launch vehicle poses high risk.";
            quizz.options[3] = "It will kill the plants in space.";
            quizz.answerID = 2;
            quizzesList.Add(quizz);
        }
    }
}