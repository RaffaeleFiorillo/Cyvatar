using Cyvatar.src.GameScenes;
using Cyvatar.src.Utils.Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyvatar.src.Weapons
{
    public enum OriginType
    {
        /// <summary>
        /// When a Damager was originated by the player or has been converted to be on the player's side.
        /// This means that this type of Damager only affects an Enemy.
        /// </summary>
        FriendlyFire,

        /// <summary>
        /// When a Damager was originated by an Enemy or has been converted to the enemy's side.
        /// This means that this type of Damager only affects an player.
        /// </summary>
        EnemyFire
    }

    public abstract class Damager
    {
        // Position in pixels
        public Vector2 Coo;

        /// <summary>
        /// Defines if the Damager damages the Player (EnemyFire) or an Enemy (FriendlyFire).
        /// </summary>
        public OriginType Type;

        /// <summary>
        /// When Integrity reaches 0, it means the Damager has been destroyed.
        /// </summary>
        protected float Integrity=1; 

        /// <summary>
        /// If this Flag is set to true, the Damager should be eliminated from the BattleScene.Damagers list.
        /// In other words when ToBeDeleted = True it ceases to exist.
        /// </summary>
        public bool ToBeDeleted { get => Coo.X > GameSceneManager.ScreenWidth || Integrity <= 0; }

        /// <summary>
        /// Defines if the damager deals a continuous amount of damage instead of a one-hit damage.
        /// </summary>
        public bool DealsContinuousDamage = false;

        /// <summary>
        /// Amount of damage dealt by this Damager.
        /// </summary>
        public float Damage;

        protected Damager(OriginType type, float damage) { Type = type; Damage = damage; }

        /// <summary>
        /// Returns the hitbox of this Damager.
        /// </summary>
        public abstract Rectangle GetHitbox();

        /// <summary>
        /// Defines the behaviour of the Damager when it hits something.
        /// As the Base Behaviour, The Damager's Integrity decreases of 1 unit.
        /// Since the default Integrity is 1, the Damager is usually Deleted after 1 Hit.
        /// Ex: Animations, Sound Effects, ToBeDeleted, ...
        /// </summary>
        public virtual void ApplyHitEffect() { Integrity -= 1; }

        public abstract void Update();

        public abstract void Draw();
    }

    public abstract class SimpleProjectile : Damager
    {
        protected readonly float Speed;  // Speed of the spear in m/s
        protected readonly string _baseImagePath = Path.Combine("Battle", "Weapons");

        public SimpleProjectile(Vector2 initialCoo, float damage, float speed, OriginType origin) : base(origin, damage)
        {
            Coo = new Vector2(initialCoo.X, initialCoo.Y + 50);
            Speed = speed;
        }

        protected abstract Texture2D GetImage();

        public override void Update()
        {
            Coo.X -= Speed * BattleScene.dt;
        }

        public override void Draw()
        {
            GraphicManager.Screen.Draw(GetImage(), Coo, Color.White);
        }
    }

    public class HitValue
    {
        #region Properties

        private static readonly SpriteFont Font = GraphicManager.LoadFont("HitFont");

        private readonly Rectangle _rectangle;

        public Color Color { get => _color*Solidity; set { _color = value; } }
        private Color _color = Color.Yellow;

        /// <summary>
        /// Hit's Numeric Damage Value displayed on the screen.
        /// </summary>
        private float Value;

        /// <summary>
        /// Current level of solidity the text has (1: fully visible; 0: transparent;)
        /// </summary>
        public float Solidity = 1;

        /// <summary>
        /// Speed in %/Second the text fades;
        /// value of 1: 100%/second
        /// value of 0.5 50%/second
        /// </summary>
        private float FadingSpeed;

        #endregion Propeerties

        public HitValue(float value, Vector2 location,  float fadingSpeed=1)
        {
            Value = value;
            FadingSpeed = fadingSpeed;
            _rectangle = new Rectangle((int)location.X, (int)location.Y, value.ToString().Length*20, 20);
        }

        public void Update()
        {
            Solidity -= FadingSpeed * BattleScene.dt;
        }

        public void Draw()
        {
            GraphicManager.DrawCenteredText(Value.ToString(), Font, _rectangle, Color, scale:1.5f);
        }
    }
}
