using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Poseidon {
    public class DamageBullet : Projectiles {
        public int damage = GameConstants.DefaultBulletDamage;

        public void initialize(Vector3 position, Vector3 headingDirection, float speed, float strength, float strengthUp) {
            base.initialize(position, headingDirection, speed);
            damage = (int) (GameConstants.DefaultBulletDamage*strength*strengthUp);
        }

        public void initialize(Vector3 position, Vector3 headingDirection, float speed, int damage) {
            base.initialize(position, headingDirection, speed);
            this.damage = damage;
        }
    }
}
