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
            Sentence sentence = new Sentence(3, "Sample cutscene that introduces the storyline. To be completed in future.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Who are you?", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "People call me Poseidon.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "The sea is in grave danger. I am glad you are here.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            //sentence = new Sentence(1, "Can you clean the environment to " + (GameConstants.LevelObjective[currentLevel] * 100).ToString() + "% within " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/backgroundDialog", 0);
            sentence = new Sentence(1, "Can you clean the environment within " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            // For scene 1
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "That was easy. The sea is clean now. The sea animals are happy too.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "They keep dropping more trash. Moreover, there are divers killing the sea creatures. I give you the power to teleport them to me.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Teleportation by shooting. Wow, that's exciting.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Save at least " + (GameConstants.LevelObjective[currentLevel] * 100).ToString() + "% of the sea creatures at the end of " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);
 

            //For scene 2
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "Are you sure the humans are safe? I don't want to hurt them!", "Image/Cutscenes/backgroundDialog", 2);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Don't worry about them. They will be released when they learn their lesson. But there is something else to worry about. They took away a leopard shark and modified it's genes. It is a fierce creature now capable of poisonous bites.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Oh, no! My bullets are too weak. How can I teleport such a big shark?", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "There are ancient relics inside the shipwrecks. They have incredible power in them.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Ancient Relics??", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Find the relic in " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days and I will teach you how to unleash their power.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);


            //For scene 3
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "The Hermes' sandal is really cool", "Image/Cutscenes/backgroundDialog", 1);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "You will surely need it. Destroy the mutant shark in " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);


            //For scene 4
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "I did it. The mutant shark is actually pretty slow.", "Image/Cutscenes/backgroundDialog", 1);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "They plan to capture all the leopard sharks and turn them into mutant shark", "Image/Cutscenes/backgroundDialog", 2);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Oh, no, I really love them. They never try to harm the humans. Why are they doing this?", "Image/Cutscenes/backgroundDialog", 2);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Save at least " + (GameConstants.LevelObjective[currentLevel] * 100).ToString() + "% of the sharks within " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/backgroundDialog", 2);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);


            //For scene 5
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "I am tired. Using the sandal makes me weak.", "Image/Cutscenes/backgroundDialog", 1);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Explore the shipwrecks and find another relic in " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 6
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "I found the Hercules's Bow.", "Image/Cutscenes/backgroundDialog", 1);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Use it wisely!  Find another relic in " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 7
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "I found the Thor's hammer. I think I am invincible.", "Image/Cutscenes/backgroundDialog", 1);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "No you are not. You need more power. Find another relic in " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 8
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "Wow, the Achilles' armor is super strong.", "Image/Cutscenes/backgroundDialog", 1);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "There are more powers hidden in the sea. Find another relic  in " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 9
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "The Aphrodite's belt? I am astonished by the power's in the sea!", "Image/Cutscenes/backgroundDialog", 1);
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
            sentence = new Sentence(1, "You have saved the sea. All the sea animals will always be grateful to you!", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

        }
    }
}
