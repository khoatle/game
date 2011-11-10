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
            sentence = new Sentence(1, "People call me Poseidon", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "The sea is in grave danger. I am glad you are here", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Can you clean the environment to " + (GameConstants.LevelObjective[currentLevel] * 100).ToString() + "% within " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            // For scene 1
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "That was easy. The sea is clean now. The sea animals are happy too.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "They keep dropping more trash. Moreover, there are divers killing the sea creatures. I give you the power to teleport them to me.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Save at least " + (GameConstants.LevelObjective[currentLevel] * 100).ToString() + "% of the sea creatures within " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);
 

            //For scene 2
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "Are you sure the humans are only teleported? I don't want to hurt them!", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Don't worry about them. They will be released when they learn their lesson. But there is something else to worry about. They took away a leopard shark and modified it's genes. It is a fierce creature now capable of poisonous bites.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Oh, no! My bullets are too weak. How can I teleport such a big shark?", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "There are ancient relics inside the shipwrecks. They have incredible power in them.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Find the relic in " + ((GameConstants.RoundTime[currentLevel].Minutes * 60) + GameConstants.RoundTime[currentLevel].Seconds) / GameConstants.DaysPerSecond + " days.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);


            //For scene 3
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "This is cutscene for level 3", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);


            //For scene 4
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "This is cutscene for level 4", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);


            //For scene 5
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "This is cutscene for level 5", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 6
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "This is cutscene for level 6", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 7
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "This is cutscene for level 7", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 8
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "This is cutscene for level 8", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 9
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "This is cutscene for level 9", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 10
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "This is cutscene for level 10", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 11
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "You have completed the game!", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

        }
    }
}
