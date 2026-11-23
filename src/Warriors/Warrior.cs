using Cyvatar.src.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System;


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

        /// <summary>
        /// Every item in this list is a number that represents a damage taken.
        /// It is useful to later display the numeric value on the screen.
        /// </summary>
        public List<HitValue> HitValues = new List<HitValue>();

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
        /// It is automatically called every time the values of X or Y are changed;
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
        /// This method creates a random coordinate to where a Hit value should be displayed.
        /// The coordinates are created within a certain radius from a given central point.
        /// </summary>
        /// <returns></returns>
        protected Vector2 CreateRandomHitValuePosition(int radious=40, bool centerIsLeft=false)
        {
            int xShift = Random.Shared.Next(10, radious) * (Random.Shared.NextDouble() > 0.5 ? -1 : 1);
            int yShift = Random.Shared.Next(10, (int)(radious * 0.25)) * (Random.Shared.NextDouble() > 0.5 ? -1 : 1);
            return new Vector2(
                Coo.X + (centerIsLeft ? -50 : 50) + xShift,
                Coo.Y + 50 + yShift
            );
        }

        /// <summary>
        /// Updates the warrior every frame.
        /// </summary>
        public virtual void Update()
        {
            foreach (HitValue hitValue in HitValues)
                hitValue.Update();

            // Remove all faded hits
            HitValues = HitValues.Where(hv => hv.Solidity > 0).ToList();
        }

        /// <summary>
        /// Draws the warrior.
        /// </summary>
        public abstract void Draw();
    }
}
