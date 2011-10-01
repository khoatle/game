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

        public void initialize(Vector3 position, Vector3 headingDirection, float speed, float strength, float strengthUp)
        {
            base.initialize(position, headingDirection, speed);
            healthAmount = (int)(GameConstants.HealingAmount * strength * strengthUp);
        }
    }
}
