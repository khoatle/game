using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poseidon
{
    public class Sentence
    {
        // Who speaks this sentence
        // 0: cyborg
        // 1: Poseidon
        // 2: bad guy
        public int speakerID;
        public string sentence;
        public Sentence(int speakerID, string sentence)
        {
            this.speakerID = speakerID;
            this.sentence = sentence;
        }
    }
    public class CutSceneDialog
    {
       
        public List<List<Sentence>> cutScenes = new List<List<Sentence>>();

        public CutSceneDialog()
        {
            // For scene 0
            List<Sentence> cutScene = new List<Sentence>();
            Sentence sentence = new Sentence(0, "Hi");
            cutScene.Add(sentence);
            sentence = new Sentence(1, "Who are you?");
            cutScene.Add(sentence);
            sentence = new Sentence(0, "People call me Poseidon");
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            // For scene 1
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "Ho");
            cutScene.Add(sentence);
            sentence = new Sentence(1, "You again?");
            cutScene.Add(sentence);
            sentence = new Sentence(0, "Yes, son");
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);
 

            //For scene 2
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "This is level 2");
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);


            //For scene 3
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "This is level 3");
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);


            //For scene 4
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "This is level 4");
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);


            //For scene 5
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "This is level 5");
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 6
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "This is level 6");
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 7
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "This is level 7");
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 8
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "This is level 8");
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 9
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "This is level 9");
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);

            //For scene 10
            cutScene = new List<Sentence>();
            sentence = new Sentence(0, "This is level 10");
            cutScene.Add(sentence);
            cutScenes.Add(cutScene);


        }
    }
}
