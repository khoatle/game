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
            this.sentence = sentence;
            this.backgroundName = backgroundName;
            this.emotionType = emotionType;
        }
    }
    public class CutSceneDialog
    {
       
        public List<List<Sentence>> cutScenes = new List<List<Sentence>>();

        public CutSceneDialog()
        {
            // For scene 0
            List<Sentence> cutScene = new List<Sentence>();
            Sentence sentence = new Sentence(3, "Sample cutscene that introduces the storyline. To be completed in future.", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Who are you?", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "People call me Poseidon", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "I am just testing the cutscene dialog", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "I am just testing too!", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            // For scene 1
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "Ho", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(1, "You again?", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Yes, son", "Image/Cutscenes/backgroundDialog", 0);
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);
 

            //For scene 2
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "This is cutscene for level 2", "Image/Cutscenes/backgroundDialog", 0);
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
