using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyvatar.src.Weapons
{
    public abstract class Damager
    {
        // Position in pixels
        public Vector2 Coo { get; set; }

        public abstract void Update();

        public abstract void Draw();
    }
}
