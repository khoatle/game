using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poseidon {
    public class DamageBullet : Projectiles {
        public int damage = GameConstants.DefaultBulletDamage;

        public void initialize(Microsoft.Xna.Framework.Graphics.Viewport viewport, Microsoft.Xna.Framework.Vector3 position, float speed, float forwardDirection, float strengthUp) {
            base.initialize(viewport, position, speed, forwardDirection);
            damage = (int) (GameConstants.DefaultBulletDamage*strengthUp);
        }
    }
}
