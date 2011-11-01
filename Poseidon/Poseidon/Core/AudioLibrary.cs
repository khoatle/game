using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Poseidon.Core
{
    public class AudioLibrary
    {
        public Song[] backgroundMusics;
        public Song[] bossMusics;
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

        private SoundEffect explosion;
        private SoundEffect newMeteor;
        private SoundEffect menuBack;
        private SoundEffect menuSelect;
        private SoundEffect menuScroll;
        private SoundEffect powerGet;
        private SoundEffect powerShow;
        private SoundEffect shooting;
        private SoundEffect explo1;
        
        private SoundEffect miniGunWindUp;
        
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
        public SoundEffect MinigunWindUp
        {
            get { return miniGunWindUp; }
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
            backgroundMusics[0] = Content.Load<Song>("Sounds/BackgroundMusics/normalBackground1");
            backgroundMusics[1] = Content.Load<Song>("Sounds/BackgroundMusics/normalBackground2");
            backgroundMusics[2] = Content.Load<Song>("Sounds/BackgroundMusics/normalBackground3");
            bossMusics[0] = Content.Load<Song>("Sounds/BackgroundMusics/bossBackground1");
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
            miniGunWindUp = Content.Load<SoundEffect>("Sounds/SoundEffects/MiinigunWindup#1");           
            bubble = Content.Load<SoundEffect>("Sounds/SoundEffects/bubble");
        }
    }
}
