using Cyvatar.src.Battle.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Cyvatar.src.Battle.Warriors
{
    public enum GoblinType
    {
        SpearGoblin,
        ArcherGoblin,
        ShamanGoblin
    }

    public abstract class Goblin: Enemy
    {
        protected GoblinType Type;
        protected string FullImagesPath { get => Path.Combine(BaseImagesPath, Type.ToString()); }
        
        public Goblin(GoblinType type, int x, int y, float imageScale, float hp) : base(x, y, hp) 
        {
            Type = type;
            IdleImage = GameSceneManager.LoadImage(Path.Combine(FullImagesPath, "Idle.png"), scale: imageScale);
            UpdateAfterMovement();
            HitBox = new Rectangle((int)Coo.X + 20, (int)Coo.Y + 30, 100, 120);
            HpRectangle = new Rectangle((int)Coo.X + 2, (int)Coo.Y + 5, 100, 10);
        }
    }
}