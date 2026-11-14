using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cyvatar.src.GameScenes;

namespace Cyvatar.src.Weapons
{
    public class GoblinSpear : Damager
    {
        private const float SPEED = 600;  // Speed of the spear in m/s
        private static Texture2D Image = GameSceneManager.LoadImage(Path.Combine("Battle", "Weapons", "GoblinSpear.png"), scale:2.2f);
        private const float _damage = 30;

        public GoblinSpear(Vector2 initialCoo) : base(OriginType.EnemyFire, _damage)
        {
            Coo = new Vector2(initialCoo.X, initialCoo.Y+50);
        }

        public override Rectangle GetHitbox()
        {
            // Example: adjust these values to match the sprite size
            return new Rectangle((int)Coo.X, (int)Coo.Y, 100, 30);
        }

        public override void ApplyHitEffect()
        {
            ToBeDeleted = true;
        }

        public override void Update()
        {
            Coo.X -= SPEED * BattleScene.dt;
        }

        public override void Draw()
        {
            GraphicManager.Screen.Draw(Image, Coo, Color.White);
        }
    }
}
