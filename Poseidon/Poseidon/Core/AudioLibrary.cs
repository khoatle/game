using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Poseidon.Core
{
    public class AudioLibrary
    {
        private SoundEffect explosion;
        private SoundEffect newMeteor;
        private SoundEffect menuBack;
        private SoundEffect menuSelect;
        private SoundEffect menuScroll;
        private SoundEffect powerGet;
        private SoundEffect powerShow;
        private SoundEffect shooting;
        private SoundEffect explo1;
        private SoundEffect changeBullet;
        private SoundEffect miniGunWindUp;
        private SoundEffect openChest;
        private SoundEffect bubble;
        private Song backMusic;
        private Song startMusic;
        public Song[] backgroundMusics;
        public Song[] bossMusics;

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
            changeBullet = Content.Load<SoundEffect>("Sounds/SoundEffects/equip");
            miniGunWindUp = Content.Load<SoundEffect>("Sounds/SoundEffects/MiinigunWindup#1");
            openChest = Content.Load<SoundEffect>("Sounds/SoundEffects/openChest");
            bubble = Content.Load<SoundEffect>("Sounds/SoundEffects/bubble");
        }
    }
}
