using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Poseidon.Core;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Poseidon
{
    public class HealthBullet : Projectiles {
        public int healthAmount;

        public override void initialize(Microsoft.Xna.Framework.Graphics.Viewport viewport, Microsoft.Xna.Framework.Vector3 position, float speed, float forwardDirection) {
            base.initialize(viewport, position, speed, forwardDirection);
            healthAmount = (int)GameConstants.HealingAmount;
        }
    }
}
