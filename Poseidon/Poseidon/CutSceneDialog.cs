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
        // For scene 1
        Sentence sce1Sen1 = new Sentence(0, "Hi");
        Sentence sce1Sen2 = new Sentence(1, "Who are you?");
        Sentence sce1Sen3 = new Sentence(0, "People call me Poseidon");
        List<Sentence> cutScene1 = new List<Sentence>();

        // For scene 2
        Sentence sce2Sen1 = new Sentence(0, "Ho");
        Sentence sce2Sen2 = new Sentence(1, "You again?");
        Sentence sce2Sen3 = new Sentence(0, "Yes, son");
        List<Sentence> cutScene2 = new List<Sentence>();

        //For scene 3
        Sentence sce3Sen1 = new Sentence(0, "This is level 3");
        List<Sentence> cutScene3 = new List<Sentence>();

        //For scene 4
        Sentence sce4Sen1 = new Sentence(0, "This is level 4");
        List<Sentence> cutScene4 = new List<Sentence>();

        //For scene 5
        Sentence sce5Sen1 = new Sentence(0, "This is level 5");
        List<Sentence> cutScene5 = new List<Sentence>();

        //For scene 6
        Sentence sce6Sen1 = new Sentence(0, "This is level 6");
        List<Sentence> cutScene6 = new List<Sentence>();

        public List<List<Sentence>> cutScenes = new List<List<Sentence>>();

        public CutSceneDialog()
        {
            cutScene1.Add(sce1Sen1);
            cutScene1.Add(sce1Sen2);
            cutScene1.Add(sce1Sen3);

            cutScene2.Add(sce2Sen1);
            cutScene2.Add(sce2Sen2);
            cutScene2.Add(sce2Sen3);

            cutScene3.Add(sce3Sen1);

            cutScene4.Add(sce4Sen1);

            cutScene5.Add(sce5Sen1);

            cutScene6.Add(sce6Sen1);

            cutScenes.Add(cutScene1);
            cutScenes.Add(cutScene2);
            cutScenes.Add(cutScene3);
            cutScenes.Add(cutScene4);
            cutScenes.Add(cutScene5);
            cutScenes.Add(cutScene6);
        }
    }
}
