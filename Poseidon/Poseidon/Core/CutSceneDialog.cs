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
            sentence = new Sentence(3, "The story begins around 2 years ago ...", "Image/Cutscenes/terminatorKillBot", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "Year 2100 ...", "Image/Cutscenes/blackScreen", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "Year 2100 ... and marine life is in grave danger", "Image/Cutscenes/blackScreen", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "Garbage dumping of harmful materials has taken its toll on the oceans", "Image/Cutscenes/Beach", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "All ocean life is facing mass extinction. The black market is in high demand for marine animals",
                "Image/Cutscenes/tire trash", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "Consequently, poachers take to the oceans to hunt the remaining few ...", "Image/Cutscenes/poachers", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "However, hope is not lost ...", "Image/Cutscenes/blackScreen", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "Scientists from the University of Houston create a highly intelligent, self-powered robot that is capable of restoring all ocean life",
                "Image/Cutscenes/Robot Creation", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "With the world's hope resting on his shoulders, the first-ever Hydrobot is deployed to the ocean. ",
                "Image/Cutscenes/Robot Creation", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "His first mission is to improve the polluted sea environment at Gulf of Mexico ", "Image/Cutscenes/blackScreen", 0);
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
            sentence = new Sentence(3, "Luckily he is not alone ... a whirlpool suddenly appears and takes him away", "Image/Cutscenes/Robot Whirlpool", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "Luckily he is not alone ... a whirlpool suddenly appears and takes him away", "Image/Cutscenes/Robot Poseidon", 0);
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
            sentence = new Sentence(1, "Now, travel to Florida's seagrass meadow to stop the poachers there. You need to save at least " + (GameConstants.LevelObjective[1] * 100).ToString() + "% of the sea creatures at the end of " + ((GameConstants.RoundTime[1].Minutes * 60) + GameConstants.RoundTime[1].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/Robot Poseidon", 0);
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
            sentence = new Sentence(0, "Are you sure the humans are safe? I don't want to hurt them!", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Don't worry about them. They will be released when they learn their lesson. But there is something else to worry about. They took away a leopard shark and modified it's genes. It is a fierce creature now capable of poisonous bites.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Oh, no! How can I defeat such a ferocious creature?", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "There are ancient relics left forgotten inside the shipwrecks hundreds or thousands years ago. Their powers are incredible. Perhaps, obtaining them will give us some hope", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Ancient Relics??", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, " Just find the relic in " + ((GameConstants.RoundTime[2].Minutes * 60) + GameConstants.RoundTime[2].Seconds) / GameConstants.DaysPerSecond + " days in one of the shipwrecks and I will explain it further.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);


            //For scene 3
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "I found an old pair of sandals ... but they do not seem to fit, how am I supposed to wear this?", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Settle down bot, these sandals are not for wearing. They are the legendary winged sandals of Hermes which enabled him to move at super sonic speeed.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Awesome, can I use that power too?", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Unfortunately, an ancient relic's power can not be used by either a human or a bot. But I can help you to extract the power of this relic and use it periodically. However, everything has its price, using the relic will also hurt you a little bit.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Sounds interesting, I am fully prepared!", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "The winged sandal is very powerful if used at the correct moment. Your goal is to find and defeat the mutant shark in " + ((GameConstants.RoundTime[3].Minutes * 60) + GameConstants.RoundTime[3].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);


            //For scene 4
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "I did it. The mutant shark was very tough to defeat.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Congratulations, but it is not over yet. The enemies plan to capture our sharks and create more mutant sharks.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Oh my gosh, we have to stop that immediately!", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "They are currently capturing the sharks as we talk. Your mission is to go there and save at least " + (GameConstants.LevelObjective[4] * 100).ToString() + "% of the sharks during " + ((GameConstants.RoundTime[4].Minutes * 60) + GameConstants.RoundTime[4].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "I got it.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(2, "Oh, a little robot, I heard that you have been interfering with our business.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "I am a hydrobot, not a robot, and I will stop you from doing bad things to the ocean!", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(2, "Hm, how confident. A small tip, do not let me catch you, bot.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);


            //For scene 5
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "I encountered a very strong human enemy during my mission. Who is he?", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "He is known as the Terminator - enemy hired gun. He is very powerful and it is no surprise to me that your current power can not match his.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Then what can I do to defeat him.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "There are more ancient relics hidden in treasure chests. Obtain it and you will have more special powers. But you must travel far and wide to find these. The Hercules' bow is somewhere in the Artic sea. Your mission is to explore the shipwrecks in the icy waters and find the Hercules's Bow in " + ((GameConstants.RoundTime[5].Minutes * 60) + GameConstants.RoundTime[5].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 6
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "I found the Hercules's Bow!", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Oh, the legendary bow of Hercules, it will help you a lot during battle. Use it wisely!", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "So what will be the next relic to find?", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "The next relic is the most powerful of all. It is somewhere in the Dead sea, where the water is so salty that very few fishes survive. Find the Thor's hammer in " + ((GameConstants.RoundTime[6].Minutes * 60) + GameConstants.RoundTime[6].Seconds) / GameConstants.DaysPerSecond + " days. Beware, you might face enemy submaries. They are loaded with torpedoes and packed with divers. Take care.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 7
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "I found the Thor's hammer. It's power is truly unmatchable!", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Yes, it is. But to defeat the Terminator, you still need more power. Explore the beautiful Kelp forest near Equador and find Achilles' armor in " + ((GameConstants.RoundTime[7].Minutes * 60) + GameConstants.RoundTime[7].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 8
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "Wow, the Achilles' armor makes me invincible!", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Yes, but remember that the power only last for a short period of time.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "So have we found all the relics?", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "There is yet one relic left to find. It is the The Aphrodite's belt. Go and find it in the Red sea within " + ((GameConstants.RoundTime[8].Minutes * 60) + GameConstants.RoundTime[8].Seconds) / GameConstants.DaysPerSecond + " days. The polluted water has harmful chemicals which may corrode your metals. Take care.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 9
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "We have found all the relics!", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Congratulations, now you are ready to face the Terminator.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "I am very much ready!", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "In order to reach the Terminator, you will first have to break the enemy defense. Be careful, there will be a lot of enemies.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 10
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "I have successfully defeat the enemy defense.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "The Terminator should be close, be careful, do not underestimate his strength.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(2, "You dare to come here? I really underestimated your bravery.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "And do not underestimate my strength too. I will defeat you once and for all. And you will have to come to our class everyday, learning how to save the ocean!", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(2, "Ho ho ho, let's see how well you do.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 11
            cutScene = new List<Sentence>();
            sentence = new Sentence(2, "Is that all you have got? Be ready to be exterminated now, bot!", "Image/Cutscenes/terminatorKillBot", 0);
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
            sentence = new Sentence(1, "It seems that he is wearing an unbreakable armor. However, my trident has the power the shatter anything. With the help of its power, your bullet should be able penetrate even his armor.", "Image/Cutscenes/Robot Poseidon", 0);
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
            sentence = new Sentence(0, "I did it! All of the enemies had to flee from this sea.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "You have saved the sea. All the sea animals will always be grateful to you! But remember, the illegal hunting organization is still there and they will continue to destroy the ocean if we do not stop them completely.", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "I understand and I will always be ready for new adventures!", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "To be continued ...", "Image/Cutscenes/blackScreen", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "Survival mode & Game Plus has been unlocked!", "Image/Cutscenes/blackScreen", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

        }
    }
}
