using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Poseidon.Core
{
    public class AudioLibrary
    {
        public Song[] backgroundMusics;
        public Song[] bossMusics;
        public Song[] minigameMusics;
        public Song[] jigsawMusics;
        public SoundEffect botNormalShot;
        public SoundEffect herculesShot;
        public SoundEffect hermesSound;
        public SoundEffect hipnotizeSound;
        private SoundEffect openChest;
        private SoundEffect changeBullet;
        public SoundEffect armorSound;
        public SoundEffect enemyShot;
        public SoundEffect bossShot;
        public SoundEffect slashSound;
        public SoundEffect biteSound;
        public SoundEffect roarSound;
        public SoundEffect plantSound;
        public SoundEffect retrieveSound;
        public SoundEffect bossLaugh;
        public SoundEffect gameOver;
        public SoundEffect gameWon;
        public SoundEffect bodyHit;
        public SoundEffect animalYell;
        public SoundEffect animalHappy;
        public SoundEffect hunterYell;
        public SoundEffect botYell;
        public SoundEffect mutantSharkYell;
        public SoundEffect chasingBulletSound;
        public SoundEffect terminatorYell;
        public SoundEffect levelUpSound;
        public SoundEffect reelHit;
        public SoundEffect healingSound;
        public SoundEffect maneteeRoar;
        public SoundEffect frozenBreathe;
        public SoundEffect buildingSound;

        private SoundEffect explosion;
        private SoundEffect newMeteor;
        private SoundEffect menuBack;
        private SoundEffect menuSelect;
        private SoundEffect menuScroll;
        private SoundEffect powerGet;
        private SoundEffect powerShow;
        private SoundEffect shooting;
        private SoundEffect explo1;
        
        private SoundEffect bubble;
        private Song backMusic;
        private Song startMusic;

        public SoundEffect Explosion
        {
            get { return explosion; }
        }

        public SoundEffect NewMeteor
        {
            get { return newMeteor; }
        }

        public SoundEffect MenuBack
        {
            get { return menuBack; }
        }

        public SoundEffect MenuSelect
        {
            get { return menuSelect; }
        }

        public SoundEffect MenuScroll
        {
            get { return menuScroll; }
        }

        public SoundEffect PowerGet
        {
            get { return powerGet; }
        }

        public SoundEffect PowerShow
        {
            get { return powerShow; }
        }
        public SoundEffect Shooting
        {
            get { return shooting; }
        }
        public SoundEffect Explo1
        {
            get { return explo1; }
        }

        public SoundEffect ChangeBullet
        {
            get { return changeBullet; }
        }
        public SoundEffect OpenChest
        {
            get { return openChest; }
        }
        public SoundEffect Bubble
        {
            get { return bubble; }
        }
        public Song BackMusic
        {
            get { return backMusic; }
        }

        public Song StartMusic
        {
            get { return startMusic; }
        }

        public void LoadContent(ContentManager Content)
        {
            backgroundMusics = new Song[GameConstants.NumNormalBackgroundMusics];
            bossMusics = new Song[GameConstants.NumBossBackgroundMusics];
            minigameMusics = new Song[GameConstants.NumMinigameBackgroundMusics];
            jigsawMusics = new Song[GameConstants.NumJigsawBackgroundMusics];
            backgroundMusics[0] = Content.Load<Song>("Sounds/BackgroundMusics/normalBackground1");
            backgroundMusics[1] = Content.Load<Song>("Sounds/BackgroundMusics/normalBackground2");
            backgroundMusics[2] = Content.Load<Song>("Sounds/BackgroundMusics/normalBackground3");
            backgroundMusics[3] = Content.Load<Song>("Sounds/BackgroundMusics/normalBackground4");
            bossMusics[0] = Content.Load<Song>("Sounds/BackgroundMusics/bossBackground1");
            minigameMusics[0] = Content.Load<Song>("Sounds/BackgroundMusics/minigameBackground");
            jigsawMusics[0] = Content.Load<Song>("Sounds/BackgroundMusics/jigsawbackground1");
            jigsawMusics[1] = Content.Load<Song>("Sounds/BackgroundMusics/jigsawbackground2");
            botNormalShot = Content.Load<SoundEffect>("Sounds/SoundEffects/hydrobotNormalShot");
            herculesShot = Content.Load<SoundEffect>("Sounds/SoundEffects/herculesShot");
            hermesSound = Content.Load<SoundEffect>("Sounds/SoundEffects/hermesSound");
            hipnotizeSound = Content.Load<SoundEffect>("Sounds/SoundEffects/hipnotized");
            openChest = Content.Load<SoundEffect>("Sounds/SoundEffects/openChest");
            changeBullet = Content.Load<SoundEffect>("Sounds/SoundEffects/equip");
            armorSound = Content.Load<SoundEffect>("Sounds/SoundEffects/armorSound");
            enemyShot = Content.Load<SoundEffect>("Sounds/SoundEffects/enemyShot");
            bossShot = Content.Load<SoundEffect>("Sounds/SoundEffects/bossShot");
            slashSound = Content.Load<SoundEffect>("Sounds/SoundEffects/slashSound");
            biteSound = Content.Load<SoundEffect>("Sounds/SoundEffects/biteSound");
            roarSound = Content.Load<SoundEffect>("Sounds/SoundEffects/roarSound");
            plantSound = Content.Load<SoundEffect>("Sounds/SoundEffects/plantingSound");
            retrieveSound = Content.Load<SoundEffect>("Sounds/SoundEffects/retrievingSound");
            bossLaugh = Content.Load<SoundEffect>("Sounds/SoundEffects/bossLaugh");
            gameOver = Content.Load<SoundEffect>("Sounds/SoundEffects/gameOver");
            gameWon = Content.Load<SoundEffect>("Sounds/SoundEffects/gameWon");
            bodyHit = Content.Load<SoundEffect>("Sounds/SoundEffects/bodyHitSound");
            animalYell = Content.Load<SoundEffect>("Sounds/SoundEffects/animalYell");
            animalHappy = Content.Load<SoundEffect>("Sounds/SoundEffects/animalHappy");
            hunterYell = Content.Load<SoundEffect>("Sounds/SoundEffects/hunterYell");
            botYell = Content.Load<SoundEffect>("Sounds/SoundEffects/botYell");
            mutantSharkYell = Content.Load<SoundEffect>("Sounds/SoundEffects/mutantsharkYell");
            terminatorYell = Content.Load<SoundEffect>("Sounds/SoundEffects/terminatorYell");
            chasingBulletSound = Content.Load<SoundEffect>("Sounds/SoundEffects/chasingBulletSound");
            levelUpSound = Content.Load<SoundEffect>("Sounds/SoundEffects/levelingUpSound");
            reelHit = Content.Load<SoundEffect>("Sounds/SoundEffects/beep");
            healingSound = Content.Load<SoundEffect>("Sounds/SoundEffects/healingSound");
            maneteeRoar = Content.Load<SoundEffect>("Sounds/SoundEffects/maneteeRoar");
            frozenBreathe = Content.Load<SoundEffect>("Sounds/SoundEffects/frozenBreathe");
            buildingSound = Content.Load<SoundEffect>("Sounds/SoundEffects/buildingSound");

            explosion = Content.Load<SoundEffect>("Sounds/SoundEffects/explosion");
            newMeteor = Content.Load<SoundEffect>("Sounds/SoundEffects/newmeteor");
            backMusic = Content.Load<Song>("Sounds/BackgroundMusics/backMusic");
            startMusic = Content.Load<Song>("Sounds/BackgroundMusics/startMusic");
            menuBack = Content.Load<SoundEffect>("Sounds/SoundEffects/menu_back");
            menuSelect = Content.Load<SoundEffect>("Sounds/SoundEffects/menu_select3");
            menuScroll = Content.Load<SoundEffect>("Sounds/SoundEffects/menu_scroll");
            powerShow = Content.Load<SoundEffect>("Sounds/SoundEffects/powershow");
            powerGet = Content.Load<SoundEffect>("Sounds/SoundEffects/powerget");
            shooting = Content.Load<SoundEffect>("Sounds/SoundEffects/laserFire");
            explo1 = Content.Load<SoundEffect>("Sounds/SoundEffects//Explo1");                    
            bubble = Content.Load<SoundEffect>("Sounds/SoundEffects/bubble");
        }
    }
}
