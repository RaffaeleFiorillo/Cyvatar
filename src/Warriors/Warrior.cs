using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Cyvatar.src.Warriors
{
    public abstract class Warrior
    {
        /// <summary>
        /// The player can only move on tiles with x < PLAYER_FIELD_RANGE
        /// </summary>
        public int PLAYER_FIELD_RANGE = 3;

        public int X { get => _X; set { _X = value; UpdatePixelCoordinates(); } }
        private int _X;

        public int Y { get => _Y; set { _Y = value; UpdatePixelCoordinates(); } }
        private int _Y;

        protected Warrior(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Converts the grid position (X, Y) into actual pixel coordinates on the screen.
        /// </summary>
        public abstract void UpdatePixelCoordinates();

        /// <summary>
        /// Draws the warrior.
        /// </summary>
        public abstract void Draw();
    }
}
