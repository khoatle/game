using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Poseidon
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
            explosion = Content.Load<SoundEffect>("Sound/explosion");
            newMeteor = Content.Load<SoundEffect>("Sound/newmeteor");
            backMusic = Content.Load<Song>("Sound/backMusic");
            startMusic = Content.Load<Song>("Sound/startMusic");
            menuBack = Content.Load<SoundEffect>("Sound/menu_back");
            menuSelect = Content.Load<SoundEffect>("Sound/menu_select3");
            menuScroll = Content.Load<SoundEffect>("Sound/menu_scroll");
            powerShow = Content.Load<SoundEffect>("Sound/powershow");
            powerGet = Content.Load<SoundEffect>("Sound/powerget");
            shooting = Content.Load<SoundEffect>("Sound/laserFire");
            explo1 = Content.Load<SoundEffect>("Sound/Explo1");
            changeBullet = Content.Load<SoundEffect>("Sound/equip");
        }
    }
}
