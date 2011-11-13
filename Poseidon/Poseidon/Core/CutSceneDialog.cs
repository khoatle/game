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
        // 2: bad guy
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
            int currentLevel = PlayGameScene.currentLevel;
            // For scene 0
            List<Sentence> cutScene = new List<Sentence>();
            Sentence sentence = new Sentence(3, "Sample cutscene that introduces the storyline. To be completed in future.", "Image/Cutscenes/blackScreen", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "Year 2100 ...", "Image/Cutscenes/blackScreen", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "Year 2100 ... and marine life is in grave danger", "Image/Cutscenes/blackScreen", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "Garbage dumping of harmful materials has taken its toll on the oceans", "Image/Cutscenes/Beach", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "Effectively, all ocean life is facing mass extinction. The black market is in high demand for marine animals",
                "Image/Cutscenes/tire trash", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "Consequently, poachers take to the oceans to hunt the remaining few ...", "Image/Cutscenes/poachers", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "However, hope is not lost ...", "Image/Cutscenes/blackScreen", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "Scientists from the University of Houston creates a highly intelligent, self-powered robot that is capable of restoring all ocean life",
                "Image/Cutscenes/Robot Creation", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "With the world's hope resting on his shoulders, the first-ever Hydrobot is deployed to the ocean. ",
                "Image/Cutscenes/Robot Creation", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "<Level objective can be viewed by clicking on the icon on top right corner of the screen>", "Image/Cutscenes/blackScreen", 0);
            cutScene.Add(sentence);
           
            cutScenes.Add(cutScene);

            // For scene 1
            cutScene = new List<Sentence>();
            sentence = new Sentence(3, "Unfortunately, during his quest, he encounters illegal poaching.", "Image/Cutscenes/Robot Shot", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "Caught off guard, the poachers try to silence his mission.", "Image/Cutscenes/Robot Shot", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "Hope and death seemed all but certain.", "Image/Cutscenes/Robot Shot", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "Luckily he is not alone ...", "Image/Cutscenes/Robot Whirlpool", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(3, "Luckily he is not alone ...", "Image/Cutscenes/Robot Poseidon", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Who are you?", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "People call me Poseidon.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Thank you for saving my life.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "The sea is in grave danger. I am glad you are here.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "I want to help. I have the power to clean the ocean and heal the sea animals!", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "I will send you near the coast of COSTA RICA. There are many divers hunting the sea creatures. I will give you the power to shoot a special bullet, after the hunters are defeated, they will be teleported to me and we will try to educate them.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Teleportation! Wow, that's exciting. Thank you so much. I will not disappoint you.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "You need to save at least " + (GameConstants.LevelObjective[currentLevel] * 100).ToString() + "% of the sea creatures at the end of " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);
 

            //For scene 2
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "Are you sure the humans are safe? I don't want to hurt them!", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Don't worry about them. They will be released when they learn their lesson. But there is something else to worry about. They took away a leopard shark and modified it's genes. It is a fierce creature now capable of poisonous bites.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Oh, no! How can I defeat such a ferocious creature?", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "There are ancient relics left forgotten inside the shipwrecks hundreds or thousands years ago. Their powers are incredible. Perhaps, obtaining them will give us some hope", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Ancient Relics??", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, " Just find the relic in " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days in one of the shipwrecks and I will explain it further.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);


            //For scene 3
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "I found an old sandal ... and most importantly, there is not even a pair and them, how am I supposed to wear this?", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Settle down bot, this sandal is not for wearing. This is the legendary winged sandal of Hermes which enabled him to move at super sonic mode.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "And can I use its power too?", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Unfortunately, an ancient relic's power can not be used by either a human or a bot. But I can help you to extract the power of this relic and use it periodically. However, everything has its price, using the relic will also hurt you a little bit.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Sounds interesting, I am fully prepared!", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "The winged sandal is very powerful if used at the correct moment. Your goal is to find and defeat the mutant shark in " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);


            //For scene 4
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "I did it. The mutant shark was very tough to defeat.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Congratulations, but it is not over yet. The enemies plan to capture our sharks and create more mutant sharks.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Oh my gosh, we have to stop that immediately!", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "They are currently capturing the sharks as we talk. Your mission is to go there and save at least " + (GameConstants.LevelObjective[currentLevel] * 100).ToString() + "% of the sharks during " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);


            //For scene 5
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "I encountered a very strong human enemy during my mission. Who is he?", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "He is known as the Terminator - enemy hired gun. He is very powerful and it is no surprise to me that your current power can not match his." + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Then what can I do to defeat him.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "There are more ancient relics lying around. Obtain them and you will have more special powers. Your mission is to explore the shipwrecks and find the Hercules's Bow in " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 6
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "I found the Hercules's Bow!", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Oh, the legendary bow of Hercules, it will help you a lot during battle. Use it wisely!", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "So what will be the next relic to find?", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "This time, find the Thor's hammer in " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 7
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "I found the Thor's hammer. It's power is truly unmatchable!", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Yes, it is. But to defeat the Terminator, you still need more power. This mission, find Achilles' armor in " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 8
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "Wow, the Achilles' armor makes me invincible!", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Yes, but remember that the power only last for a short period of time.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "So have we found all the relics?", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "There is yet one relic left to find. It is the The Aphrodite's belt. Go and find it in " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 9
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "We have found all the relics!", "Image/Cutscenes/backgroundDialog", 1);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Yes, the sea is incredible and I am proud of you.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Now, do you think I am capable of protecting the sea on my own?", "Image/Cutscenes/backgroundDialog", 1);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Yes, but I am worried. A new beast is in the sea. I don't know how powerful it is.", "Image/Cutscenes/backgroundDialog", 2);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 10
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "I am dying. Forgive me. I can't do this.", "Image/Cutscenes/backgroundDialog", 1);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Wait, I am sorry I underestimated his strength. I will give you all my strength. Here, take my trident.", "Image/Cutscenes/backgroundDialog", 4);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 11
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "I am dying. Forgive me. I can't do this.", "Image/Cutscenes/backgroundDialog", 1);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Wait, I am sorry I underestimated his strength. I will give you all my strength. Here, take my trident.", "Image/Cutscenes/backgroundDialog", 4);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 12
            cutScene = new List<Sentence>();
            sentence = new Sentence(1, "You have saved the sea. All the sea animals will always be grateful to you!", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

        }
    }
}
