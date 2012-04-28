using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Poseidon
{
    public class Sentence
    {
        // Who speaks this sentence
        // 0: cyborg
        // 1: Poseidon
        // 2: terminator
        // 3: narrator (no face displayed, just words)
        public int speakerID;
        public string sentence;
        //background corresponding to this sentence
        public string backgroundName;
        //speaker speaks this with what type of emotion
        //0: normal
        //1: happy
        //2: sad
        //3: surprised
        //4: angry/determined
        public int emotionType;
        public Sentence(int speakerID, string sentence, string backgroundName, int emotionType)
        {
            this.speakerID = speakerID;
            this.sentence = sentence ;
            this.backgroundName = backgroundName;
            this.emotionType = emotionType;
        }
    }
    public class CutSceneDialog
    {
       
        public List<List<Sentence>> cutScenes = new List<List<Sentence>>();

        public CutSceneDialog()
        {
            //int currentLevel = PlayGameScene.currentLevel;
            // For scene 0
            List<Sentence> cutScene = new List<Sentence>();
            Sentence sentence;
            //sentence = new Sentence(3, "Sample cutscene that introduces the storyline. To be completed in future.", "Image/Cutscenes/blackScreen", 0);
            //cutScene.Add(sentence);
            sentence = new Sentence(3, "Not every story ends happily ...", "Image/Cutscenes/blackScreen", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "So is the story of our hydrobot ...", "Image/Cutscenes/terminatorKillBot", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "... Sorry everyone, I could not do it ...", "Image/Cutscenes/terminatorKillBot", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "The story begins around 3 years ago ...", "Image/Cutscenes/terminatorKillBot", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "Year 2100 ...", "Image/Cutscenes/blackScreen", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "Year 2100 ... and marine life is in grave danger.", "Image/Cutscenes/blackScreen", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "Garbage dumping of harmful materials has taken its toll on the oceans.", "Image/Cutscenes/Beach", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "All ocean life is facing mass extinction. The black market is in high demand for marine animals.",
                "Image/Cutscenes/tire trash", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "Consequently, poachers take to the oceans to hunt the remaining few ...", "Image/Cutscenes/poachers", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "However, hope is not lost ...", "Image/Cutscenes/blackScreen", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "Scientists from the University of Houston create a highly intelligent, self-powered robot that is capable of restoring all ocean life.",
                "Image/Cutscenes/Robot Creation", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "With the world's hope resting on his shoulders, the first-ever Hydrobot is deployed to the ocean. ",
                "Image/Cutscenes/Robot Creation", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "His first mission is to improve the polluted sea environment at Gulf of Mexico. ", "Image/Cutscenes/blackScreen", 0);
            cutScene.Add(sentence);
           
            cutScenes.Add(cutScene);

            // For scene 1
            cutScene = new List<Sentence>();
            sentence = new Sentence(3, "Unfortunately, during his quest, he encounters illegal poaching.", "Image/Cutscenes/poaching", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "What is that diver doing?! I have to stop him!", "Image/Cutscenes/poaching", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "Caught off guard, the poachers try to silence his mission.", "Image/Cutscenes/Robot Shot", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Ugh ...", "Image/Cutscenes/Robot Shot", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "Hope and death seemed all but certain.", "Image/Cutscenes/Robot Shot", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "However, a whirlpool suddenly appears and takes him away", "Image/Cutscenes/Robot Whirlpool", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "Luckily he is not alone ... ", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            //more work needed on this part
            sentence = new Sentence(0, "Wow ... what have just happened? I thought I was going to die there ...", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Not yet, the world still needs you, bot.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "An old man? In the middle of the sea? Who might you be?", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "I am known as Poseidon, the ruler of the sea.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Poseidon ... I thought you only exists in the myth. Thank you for saving my life.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "It was my pleasure, and besides, as I said ealier, we need you to help us, bot.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "I do not understand, how can a hydrobot like me help you with anything? I could not even protect myself from that poacher.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "As you may have already seen, the ocean life is in grave danger. The sea environment is completely polluted because of waste. Sea animals are being hunted by these illegal organization.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Then why don't you stop them? You are very powerful, right?", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "I can not interfere with human, that has been the rule for centuries and can not be violated ... but you can, and that's why we need your help.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "But why am I chosen?", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "I observed you tried to save the fishes, you have a kind heart and that's why I chose you.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Alright, I got it, so how do we start? I have the ability to clean the ocean and heal the sea animals!", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Impressive, but these power will not be enough for you to win this battle. I will give you the power to shoot a special type of bullet, after the hunters are defeated, they will be teleported to me and we will try to educate them.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "That's absolutely fantastic. I will not let you down.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "And from now on, for all the good deeds that you do for the ocean, I will repay you handsomely, pay attention to the good will indicator.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            //sentence = new Sentence(1, "I will now send you to near the coast of COSTA RICA. There are many divers hunting the sea creatures. You need to save at least " + (GameConstants.LevelObjective[1] * 100).ToString() + "% of the sea creatures at the end of " + ((GameConstants.RoundTime[1].Minutes * 60) + GameConstants.RoundTime[1].Seconds) / GameConstants.DaysPerSecond + " days to accomplish this mission.", "Image/Cutscenes/Robot Poseidon", 0);
            //cutScene.Add(sentence);
            sentence = new Sentence(1, "Now, on your way to Florida's seagrass meadow to stop the poachers there. You need to save at least " + (GameConstants.LevelObjective[1] * 100).ToString() + "% of the sea creatures at the end of " + ((GameConstants.RoundTime[1].Minutes * 60) + GameConstants.RoundTime[1].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);

            //sentence = new Sentence(0, "Who are you?", "Image/Cutscenes/Robot Poseidon", 0);
            //cutScene.Add(sentence);
            //sentence = new Sentence(1, "I am Poseidon, the ruler of the sea.", "Image/Cutscenes/Robot Poseidon", 0);
            //cutScene.Add(sentence);
            //sentence = new Sentence(0, "Why did you save my life?", "Image/Cutscenes/Robot Poseidon", 0);
            //cutScene.Add(sentence);
            //sentence = new Sentence(1, "You are a bot, you don't have a life. Besides, I want you to protect the ocean life.", "Image/Cutscenes/Robot Poseidon", 0);
            //cutScene.Add(sentence);
            //sentence = new Sentence(0, "Huh! I can't even protect myself. The poachers are after me.", "Image/Cutscenes/Robot Poseidon", 0);
            //cutScene.Add(sentence);
            //sentence = new Sentence(1, "I got some teleportation bullets from a UH scientist back in 2080. You can teleport the poachers to my prison. Do you think you can handle that?", "Image/Cutscenes/Robot Poseidon", 0);
            //cutScene.Add(sentence);
            //sentence = new Sentence(0, "Absolutely, that's fantastic. I will not let you down.", "Image/Cutscenes/Robot Poseidon", 0);
            //cutScene.Add(sentence);
            //sentence = new Sentence(1, "Go near the coast of Florida where there are lots of seagrasses. You will find many poachers. You need to save at least " + (GameConstants.LevelObjective[1] * 100).ToString() + "% of the sea creatures at the end of " + ((GameConstants.RoundTime[1].Minutes * 60) + GameConstants.RoundTime[1].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/Robot Poseidon", 0);
            //cutScene.Add(sentence);

            cutScenes.Add(cutScene);
 

            //For scene 2
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "I am back from the Florida coast. How are the captured poachers doing? ", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Don't worry about them. They will be released once they learn their lesson.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Cool! If they can be turned from bad to good, it will benefit the ocean life alot. ", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Yes, but there is a more urgent matter at the moment. The hunter organization took away a leopard shark and modified it's genes. It is now a fierce creature with corrosive bites. Seems that organization is taking a step to stop you.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "A mutant shark ...? Does not sound like something I can defeat ...", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Yes, not at the moment. But there is still a chance.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Chance?", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Among the pirate ships that sunk in the ocean thousands years ago, there are ones with ancient relics left forgotten inside. The incredible power of these relics may help us.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Ancient relics???", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Just obtain the relic from one of the shipwrecks and I will explain further. You have to return before " + ((GameConstants.RoundTime[2].Minutes * 60) + GameConstants.RoundTime[2].Seconds) / GameConstants.DaysPerSecond + " days have passed or else it will be too late.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);


            //For scene 3
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "I am back. You did not tell me that there are ghost pirates inside these shipwrecks...", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Well, I did not want to scare you. I hope they did not cause much problem for you?", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "I am not a kid anymore, ghost can not scare me, you know.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Oh ... uhm ... I see ... (Does he mean that he was once a kid Hydrobot???)", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Anyway, I found an old pair of sandals ... but they do not seem to fit, how am I supposed to wear this?", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Settle down bot, these sandals are not for wearing. They are the legendary winged sandals of Hermes which enabled him to move at super sonic speeed.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Awesome, can I use that power too?", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Unfortunately, an ancient relic's power can be used by neither a human nor a bot. But I can help you to extract the power of this relic and use it periodically.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Sounds interesting, I am eager to try it out!", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "However, everything has its price, using the relic will also hurt you a little bit. So use the power efficiently.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "I will keep that in mind.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Now go find and defeat the mutant shark. Finish the mission in " + ((GameConstants.RoundTime[3].Minutes * 60) + GameConstants.RoundTime[3].Seconds) / GameConstants.DaysPerSecond + " days before the enemies realize your appearance.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);


            //For scene 4
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "I did it! The mutant shark was very tough to defeat.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Congratulations, I am working on turning him into a normal good shark again", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "I hope with the lost of the mutant shark, the poachers will fall back.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "I fear it is not happening that way. At the moment, the hunter organization is already deploying more troops to capture our sharks and create more mutant sharks.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Oh my gosh, we have to stop that immediately! Where are they capturing the sharks?", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "The Shark sea - where sharks are plenty. When you are ready, travel there and and try save at least " + (GameConstants.LevelObjective[4] * 100).ToString() + "% of the sharks from being captured.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "I got it. I will go there right now.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "A moment later.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "(Hm, someone is trying to communicate with me over the radio wave, I wonder who it is.)", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(2, "Oh, are you the little robot? I heard you have been interfering with our organization's business.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "I am a Hydrobot, not a robot, and I will stop your oganization from doing bad things to the ocean!", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(2, "Hm, how confident. A small tip, do not let me catch you, bot.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);


            //For scene 5
            cutScene = new List<Sentence>();
            sentence = new Sentence(1, "You are back, how was the battle?", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "During the mission, I encountered a very strong hunter and barely escaped his grasp. Who might him be?", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Well, it must be the Terminator - enemy hired gun. He is very powerful and it is no surprise to me that your current power can not match his.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Then ... what can I possibly do? If he is there, the hunter organization is unstoppable.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Do not lose hope yet. There are more ancient relics hidden around. Obtain them and you will have more special powers. But you must travel far and wide to find these. The Hercules' bow is somewhere in the Artic sea. Your mission is to explore the shipwrecks in the icy waters and find the bow in " + ((GameConstants.RoundTime[5].Minutes * 60) + GameConstants.RoundTime[5].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 6
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "I am back with the bow!", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Ah, the legendary bow of Hercules, its power deals lethal damage to a single enemy. The stronger you are, the more damage it deals. ", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Okay, I will remember that. So what will be the next relic to find?", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Our next target is the mighty Mjolnir. It is lost somewhere in the Dead sea, where the water is so polluted that it turns black. Find the hammer and return before " + ((GameConstants.RoundTime[6].Minutes * 60) + GameConstants.RoundTime[6].Seconds) / GameConstants.DaysPerSecond + " days have passed.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Got it, I will be on my way now.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Be careful this time, the hunter organization is aware of your action and has sent their most advanced weapon - the Shark submarine to stop you. It is loaded with torpedoes and packed with hunters.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 7
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "I have found Mjolnir. It's power is truly unmatchable!", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Yes, it is. But remember, no matter how powerful are the relics, it is still your skill that decides the outcome of battles.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Yes, dodging the deadly bullets and hitting the enemies at the right time are very important too. I am working on these skills.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Good, now onto your next mission. Explore the beautiful Kelp forest of Equador and find Achilles' armor in " + ((GameConstants.RoundTime[7].Minutes * 60) + GameConstants.RoundTime[7].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 8
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "Wow, a shiny golen armor even after all these years sunken under the sea!", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Not only that, it will also make you invincible for a short period of time.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Awesome! So have we found all the relics?", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "There is yet one relic left to find. It is the Aphrodite's belt, lost somewhere the Red sea.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "The Red sea? Does the name come from its water color?", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Yes, but not its natural color, the water is polluted with tons of harmful chemical waste which can corrode your metals. Do not hang around for too long and bring back the belt within " + ((GameConstants.RoundTime[8].Minutes * 60) + GameConstants.RoundTime[8].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 9
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "The Red sea water is really detrimental for me and there were a lot of enemies trying to stop me too. Luckily, I could make it back with the belt!", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Congratulations, now we have all the relics and are ready to face the Terminator.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Once and for all!", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "In order to reach the Terminator, you will first have to break through the enemy defense.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "With all the powerful relics in our hand, I guess that will not be a big problem.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Be careful, as far as I know, the enemies somehow got their hands on the Dead Pirate King Crown. An evil artefact that gives them control over the ghost pirates. You will probably have to face the pirates out of their ships this time.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 10
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "The enemy defense is broken.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "The Terminator should be close, be careful, do not underestimate his strength.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(2, "You dare to come here? I really underestimated your bravery.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "And do not underestimate my strength too. I will defeat you once and for all. And you will have to come to our class everyday, learning how to save the ocean!", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(2, "Ho ho ho, I am eager to make a lot of friends in the class.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Are you serious?", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(2, "No, was just joking!", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 11
            cutScene = new List<Sentence>();
            sentence = new Sentence(2, "Is that all you have got? Looks like we will not be able to meet in the class. Be ready to be exterminated now, bot!", "Image/Cutscenes/terminatorKillBot", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "... Sorry everyone, I could not do it ...", "Image/Cutscenes/terminatorKillBot", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "A whirlpool appears again and saves our Hydrobot right at the moment ... ", "Image/Cutscenes/Robot Whirlpool", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "And Poseidon appears ...", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Are you alright?", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Sorry, I could not do it ... he is still too powerful for me", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "It seems that he is wearing an armor made out of unbreakable materials. However, my trident has the power to shatter anything. With the help from its power, your bullet should be able penetrate even his armor.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Really? Then bring me back to face him again. This time, I will not lose.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Our hope rests on your shoulder, bot.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(2, "Oh you are back, I thought you ran for your life, you weakling.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Let's see who will have to run this time!", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 12
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "I did it! With the Terminator's defeat, the enemies are fleeing from this ocean!", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "You have saved this sea. All the sea animals will always be grateful to you! But remember, the illegal hunting organization is still there and they will continue to destroy the ocean if we do not stop them completely.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "I understand and will always be ready for new adventures!", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "To be continued ...", "Image/Cutscenes/blackScreen", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "Guardian mode & Game Plus has been unlocked!", "Image/Cutscenes/blackScreen", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

        }
    }
}
