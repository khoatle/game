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
            quizz.question = "Who tastes the best among the below?";
            quizz.options[0] = "Hien";
            quizz.options[1] = "Deb";
            quizz.options[2] = "Khoa";
            quizz.options[3] = "Sushil";
            quizz.answerID = 0;
            quizzesList.Add(quizz);


            quizz = new Quizz();
            quizz.question = "Are you a gay?";
            quizz.options[0] = "Yes";
            quizz.options[1] = "Probably";
            quizz.options[2] = "Certainly";
            quizz.options[3] = "85%";
            quizz.answerID = 2;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Which fish looks the best among the below?";
            quizz.options[0] = "Shark";
            quizz.options[1] = "Sting Ray";
            quizz.options[2] = "Mutant Shark";
            quizz.options[3] = "Terminator";
            quizz.answerID = 2;
            quizzesList.Add(quizz);

            quizz = new Quizz();
            quizz.question = "Should we eat humans?";
            quizz.options[0] = "Hell ya!";
            quizz.options[1] = "Let's do it!";
            quizz.options[2] = "Why not?";
            quizz.options[3] = "We should";
            quizz.answerID = 2;
            quizzesList.Add(quizz);
        }
    }
}
