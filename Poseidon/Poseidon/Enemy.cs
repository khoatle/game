using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Poseidon {
    public class Enemy : Barrier {
        public Vector3 shootingDirection;
        public bool isShooting = false;
        private TimeSpan lastFire;

        public bool cyborgJustEscape = false;

        public Enemy() : base() {
            lastFire = new TimeSpan();
        }

        //public override void Update(GameTime gameTime, Barrier[] barriers, int size, Tank tank) {
        //    if (Vector3.Distance(tank.Position, perceptingSphere.Center) < perceptingSphere.Radius) {
        //        shootingDirection = tank.Position;
        //        isShooting = true;
        //    }
        //    else {
        //        isShooting = false;

        //        TimeSpan time = gameTime.TotalGameTime;
        //        if (time.TotalSeconds - lastFire.TotalSeconds > 30) {
        //            cyborgJustEscape = false;
        //            lastFire = time;
        //        }
        //        else {
        //            cyborgJustEscape = true;
        //        }
        //    }
        //}
    }
}
