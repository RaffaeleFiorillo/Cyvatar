using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
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
        /// If this Flag is set to true, the Damager should be eliminated from the BattleScene.Damagers list.
        /// In other words when ToBeDeleted = True it ceases to exist.
        /// </summary>
        public bool ToBeDeleted = false;


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
        /// Ex: Animations, Sound Effects, ToBeDeleted, ...
        /// </summary>
        public abstract void ApplyHitEffect();

        public abstract void Update();

        public abstract void Draw();
    }
}
