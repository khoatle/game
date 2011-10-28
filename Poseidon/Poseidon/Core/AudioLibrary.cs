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
            explosion = Content.Load<SoundEffect>("Sound/SoundEffects/explosion");
            newMeteor = Content.Load<SoundEffect>("Sound/SoundEffects/newmeteor");
            backMusic = Content.Load<Song>("Sound/BackgroundMusics/backMusic");
            startMusic = Content.Load<Song>("Sound/BackgroundMusics/startMusic");
            menuBack = Content.Load<SoundEffect>("Sound/SoundEffects/menu_back");
            menuSelect = Content.Load<SoundEffect>("Sound/SoundEffects/menu_select3");
            menuScroll = Content.Load<SoundEffect>("Sound/SoundEffects/menu_scroll");
            powerShow = Content.Load<SoundEffect>("Sound/SoundEffects/powershow");
            powerGet = Content.Load<SoundEffect>("Sound/SoundEffects/powerget");
            shooting = Content.Load<SoundEffect>("Sound/SoundEffects/laserFire");
            explo1 = Content.Load<SoundEffect>("SoundSoundEffects//Explo1");
            changeBullet = Content.Load<SoundEffect>("Sound/SoundEffects/equip");
            miniGunWindUp = Content.Load<SoundEffect>("Sound/SoundEffects/MiinigunWindup#1");
            openChest = Content.Load<SoundEffect>("Sound/SoundEffects/openChest");
            bubble = Content.Load<SoundEffect>("Sound/SoundEffects/bubble");
        }
    }
}
