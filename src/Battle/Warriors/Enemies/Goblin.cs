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

        protected float XMovingProbability = 0.025f;
        protected float YMovingProbability = 0.035f;
        protected float MaxMovingCooldownTime = 1f;
        
        public Goblin(GoblinType type, int x, int y, float imageScale, float hp) : base(x, y, hp) 
        {
            Type = type;
            IdleImage = GameSceneManager.LoadImage(Path.Combine(FullImagesPath, "Idle.png"), scale: imageScale);
            UpdatePixelCoordinates();
            FieldRange = 3;
            HitBox = new Rectangle((int)Coo.X + 20, (int)Coo.Y + 30, 100, 120);
            HpRectangle = new Rectangle((int)Coo.X + 2, (int)Coo.Y + 5, 100, 10);
        }
    }
}