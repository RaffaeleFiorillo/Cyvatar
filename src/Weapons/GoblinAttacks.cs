using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;

namespace Cyvatar.src.Weapons
{
    public class GoblinSpear : SimpleProjectile
    {
        private static Texture2D Image;
        private const float _damage = 30;
        private const float _speed = 600;  // Speed of the spear in m/s

        public GoblinSpear(Vector2 initialCoo) : base(initialCoo, _damage, _speed, OriginType.EnemyFire)
        {
            Coo = new Vector2(initialCoo.X, initialCoo.Y+50);
            Image = GameSceneManager.LoadImage(Path.Combine(_baseImagePath, "GoblinSpear.png"), scale: 2.2f);
        }

        protected override Texture2D GetImage(){return Image;}

        public override Rectangle GetHitbox()
        {
            // Example: adjust these values to match the sprite size
            return new Rectangle((int)Coo.X, (int)Coo.Y, 100, 30);
        }
    }

    public class GoblinArrow : SimpleProjectile
    {
        private static Texture2D Image;
        private const float _speed = 900;  // Speed of the spear in m/s
        private const float _damage = 5;

        public GoblinArrow(Vector2 initialCoo) : base(initialCoo, _damage, _speed, OriginType.EnemyFire)
        {
            Coo = new Vector2(initialCoo.X, initialCoo.Y + 50);
            Image = GameSceneManager.LoadImage(Path.Combine(_baseImagePath, "GoblinArrow.png"), scale: 2.2f);
        }

        protected override Texture2D GetImage() { return Image; }

        public override Rectangle GetHitbox()
        {
            // Example: adjust these values to match the sprite size
            return new Rectangle((int)Coo.X, (int)Coo.Y, 100, 30);
        }
    }
}
