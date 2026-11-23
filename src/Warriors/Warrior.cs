using Cyvatar.src.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Cyvatar.src.Warriors
{
    public abstract class Warrior
    {
        /// <summary>
        /// The player can only move on tiles with x < PLAYER_FIELD_RANGE.
        /// </summary>
        public int FieldRange = 3;

        /// <summary>
        /// Flag that indicates if the player should be removed from the BattleScene.
        /// It occurs when: Hp <= 0 AND DeathAnimation.HasEnded == true.
        /// </summary>
        public bool IsDead = false;

        public float Hp;

        public int X { get => _X; set { _X = value; UpdatePixelCoordinates(); } }
        private int _X;

        public int Y { get => _Y; set { _Y = value; UpdatePixelCoordinates(); } }
        private int _Y;

        /// <summary>
        /// Pixel coordinates that allow to display the warrior's image on the screen.
        /// </summary>
        public Vector2 Coo;
        public Rectangle HitBox;

        protected Warrior(int x, int y, float hp)
        {
            X = x;
            Y = y;
            Hp = hp;
        }

        /// <summary>
        /// Converts the grid position (X, Y) into actual pixel coordinates on the screen.
        /// </summary>
        public abstract void UpdatePixelCoordinates();

        public bool CollidesWith(Rectangle otherBounds)
        {
            return HitBox.Intersects(otherBounds);
        }

        /// <summary>
        /// Defines the logic of taking damage from a Damager.
        /// </summary>
        /// <param name="damager"></param>
        public abstract void TakeDamageFrom(Damager damager);

        /// <summary>
        /// Updates the warrior every frame.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Draws the warrior.
        /// </summary>
        public abstract void Draw();
    }
}
