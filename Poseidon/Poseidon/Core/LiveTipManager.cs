using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poseidon.Core
{
    public class TipItem
    {
        public string tipItemStr;
        public TipItem(string tipItemContent)
        {
            tipItemStr = tipItemContent;
        }
    }

    public class LiveTipManager
    {
        public List<List<TipItem>> allTips = new List<List<TipItem>>();

        public LiveTipManager()
        {
            List<TipItem> levelTips = new List<TipItem>();
            TipItem tipItem;

            //tip for level 1
            tipItem = new TipItem("Press 'Z'/'X'/'C' to collect bio/plastic/nuclear waste.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Press 'Z' to retrive resource box/powerpack.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Use the panel on lower left corner of the screen for facility constructions.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Double click on a plant to drop trash for processing.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("'Shift + Left Click' to open a facility's control panel");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Level objective and Tips can be viewed by clicking on icons on top right corner of the screen");
            levelTips.Add(tipItem);
            tipItem = new TipItem("'Left Click' on a living target to shoot energy bullet.");
            levelTips.Add(tipItem);
            allTips.Add(levelTips);

            //tip for level 2
            levelTips = new List<TipItem>();
            tipItem = new TipItem("Press 'Space Bar' to switch bullet type.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Hold 'Ctrl' to hold position and aim easier.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Powerpacks temporarily boost Hydrobot's attributes.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("When the environment is polluted, the sea creatures die easily.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Hydrobot and processing plant can be upgraded from Research center.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Press 'Caps Lock' while hovering cursor over a living object to lock target.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Strange rocks can be analyzed at the Research center.");
            levelTips.Add(tipItem);
            allTips.Add(levelTips);

            //tip for level 3
            levelTips = new List<TipItem>();
            tipItem = new TipItem("Doing well in minigame between levels will bring advantage.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("There are 3 shipwrecks. A shipwreck will appear on the radar once it is spotted.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Double click on a ship wreck to get into it. Press 'esc' to get out.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Cleaning the environment and healing make the fish happy, they will help you in return.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Writing on the paintings provides answer for question in Quiz minigame.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Pay attention to what the fishes are saying. They help in the Quiz minigame.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("When a sea animal dies, the environment degrades. Same when trash is dumped into ocean.");
            levelTips.Add(tipItem);
            allTips.Add(levelTips);

            //tip for level 4
            levelTips = new List<TipItem>();
            tipItem = new TipItem("'Right Click' to perform special power from relic.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Hermes's winged sandal's power deals a lot of damage if used at a correct angle.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("The mutant shark has corrosive bite. Keep distance.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Powerpacks are really helpful for tough battle.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("It is very wise to fight near your 'base of powerpacks'.");
            levelTips.Add(tipItem);
            allTips.Add(levelTips);

            //tip for level 5
            levelTips = new List<TipItem>();
            tipItem = new TipItem("The mutant shark kills sea animals very fast.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("You lose health every time you cast a special power.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Doing good deeds fills up the good will bar.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("The lower your health, the weaker your power.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Try to use the skill wisely and efficiently.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("You do not have to defeat the Terminator to win this level.");
            levelTips.Add(tipItem);
            allTips.Add(levelTips);

            //tip for level 6
            levelTips = new List<TipItem>();
            tipItem = new TipItem("Remember, there are 3 shipwrecks. A shipwreck will appear on the radar once it is spotted.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Your skills' power are linked to your attributes. Assign atttributes wisely.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("It is much easier to aim and shoot while holding 'Ctrl'.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Filling up the good will bar frequently is a very good way to build up your power.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Extinct animals can be resurrected in the Research center.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("The resurrected animals will join you in the battle with their unique abilities.");
            levelTips.Add(tipItem);
            allTips.Add(levelTips);

            //tip for level 7
            levelTips = new List<TipItem>();
            tipItem = new TipItem("Hercules's bow deals an enormous amount of damage to a single enemy.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Use the Number pad for switching among skills. Hercules's bow is '1', Hermes's winged sandal is '4'.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("The resurrected animals' power can be increased with further study in the Research center.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("The resurrected animal will be lost if killed.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Doing well in minigame between levels will bring advantage.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("The lower your health, the weaker your power.");
            levelTips.Add(tipItem);
            allTips.Add(levelTips);

            //tip for level 8
            levelTips = new List<TipItem>();
            tipItem = new TipItem("Thor's hammer stuns and pushes enemies away.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Use the Number pad for switching among skills. Thor's hammer is '2'.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("The powerpacks boots your attributes, hence boost the skill's power.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Pay attention to what the fishes are saying. They help in the Quiz minigame.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Your skills' power are linked to your attributes. Assign atttributes wisely.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Writing on the paintings provides answer for question in Quiz minigame.");
            levelTips.Add(tipItem);
            allTips.Add(levelTips);

            //tip for level 9
            levelTips = new List<TipItem>();
            tipItem = new TipItem("Put on the golden armor to be invincible.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Use the Number pad for switching among skills. Achilles's armor is '3'.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("The Red sea has a corrosive effect. Pay attention to your health.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Casting the skill after absorbing the right powerpack will produce incredible power.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Filling up the good will bar frequently is a very good way to build up your power.");
            levelTips.Add(tipItem);
            allTips.Add(levelTips);

            //tip for level 10
            levelTips = new List<TipItem>();
            tipItem = new TipItem("Aphrodite's belt makes enemies turn against each other.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Make use of all your skills. Press 1-5.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("'Base of powerpacks' can make your battle much easier.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("The resurrected animals' power can be increased with further study in the Research center.");
            levelTips.Add(tipItem);
            allTips.Add(levelTips);

            //tip for level 11
            levelTips = new List<TipItem>();
            tipItem = new TipItem("Casting all skills in a wise sequence will make the boss fight trivial.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Make use of all your skills. Press 1-5.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("The powerpacks boots your attributes, hence boost the skill's power.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("The lower your health, the weaker your power.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("There is less chance to get hit by a bullet when you run in a spiral motion.");
            levelTips.Add(tipItem);
            allTips.Add(levelTips);

            //tip for level 12
            levelTips = new List<TipItem>();
            tipItem = new TipItem("Casting all skills in a wise sequence will make the boss fight trivial.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("The lower your health, the weaker your power.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("It is very wise to fight near your 'base of powerpacks'.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Casting the skill after absorbing the right powerpack will produce incredible power.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("There is less chance to get hit by a bullet when you run in a spiral motion.");
            levelTips.Add(tipItem);
            tipItem = new TipItem("Goodluck and remember, there are secret to discover in the New Game Plus.");
            levelTips.Add(tipItem);
            allTips.Add(levelTips);

        }
    }
}
