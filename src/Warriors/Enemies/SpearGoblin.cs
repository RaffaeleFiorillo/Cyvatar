using Cyvatar.src.GameScenes;
using Cyvatar.src.Utils.Animations;
using Cyvatar.src.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyvatar.src.Warriors
{
    public class SpearGoblin : Goblin
    {
        private const float _imageScale = 2.2f;

        public SpearGoblin() : base(
            GoblinType.SpearGoblin,
            Random.Shared.Next(5, 8),
            Random.Shared.Next(0, 3),
            _imageScale, 
            100
        )
        {
            MoveSet = new SpearGoblinMoveSet(this, _imageScale);
            DeathAnimation = new SpearGoblinDeathAnimation(Coo, _imageScale);
            UpdatePixelCoordinates();
            FieldRange = 3;
            HitBox = new Rectangle((int)Coo.X + 20, (int)Coo.Y + 30, 100, 120);

            MaxHp = 100;
            XMovingProbability = 0.025f;
            YMovingProbability = 0.055f;

            MaxMovingCooldownTime = 1f;
            AttackingProbability = 0.015f;
        }

        public override void UpdatePixelCoordinates()
        {
            Coo = new Vector2(X*197, Y*-145 + 820);
            MoveSet?.UpdateCoordinates(Coo);
            DeathAnimation?.UpdateCoordinates(Coo);
        }

        protected override void Move()
        {
            if (MovingCooldownTime <= 0)
            {
                if (Random.Shared.NextDouble() < YMovingProbability)
                {
                    if (Random.Shared.NextDouble() > 0.5f)
                        Y += Y < 3 ? 1 : 0;
                    else
                        Y -= Y > 0 ? 1 : 0;
                    MovingCooldownTime = MaxMovingCooldownTime;

                    HitBox.Y = (int)Coo.Y + 30;
                    HpRectangle.Y = (int)Coo.Y + 5;
                }
                else if (Random.Shared.NextDouble() < XMovingProbability)
                {
                    if (Random.Shared.NextDouble() > 0.5f)
                        X += X < 9 ? 1 : 0;
                    else
                        X -= X >= 9 - FieldRange ? 1 : 0;
                    MovingCooldownTime = MaxMovingCooldownTime;

                    HitBox.X = (int)Coo.X + 20;
                    HpRectangle.X = (int)Coo.X + 2;
                }
            }
        }
    }

    public class SpearGoblinDeathAnimation: SimpleAnimation
    {
        public SpearGoblinDeathAnimation(Vector2 goblinPosition, float imageScale) : base
            (
                Path.Combine("Battle", "Characters", "Enemies", "SpearGoblin", "Death"), // Path to the frames
                goblinPosition,  // InitialPosition
                8,               // FramesCount
                6f,              // Frame Speed (F/s)
                0.4f,            // Last Frame Time before animation Ends (s)
                imageScale       // scale for the frame images in the animation
            )
        {
        }

        public override void UpdateCoordinates(Vector2 goblinPosition)
        {
            Coo = goblinPosition;
        }
    }


    #region Attack MoveSet

    public class SpearGoblinMoveSet : AttackMoveSet
    {
        public SpearGoblinMoveSet(Enemy moveSetOwner, float imageScale):base(moveSetOwner)
        {
            MoveSet.Add(new SpearThrowAttack(moveSetOwner.Coo, imageScale));
        }

        public override void UpdateCoordinates(Vector2 coo)
        {
            ActiveAttack.AttackAnimation.UpdateCoordinates(coo);
        }
    }

    public class SpearThrowAttack : Attack
    {
        private bool HasAttacked = false;

        public SpearThrowAttack(Vector2 goblinCoo, float imageScale) : base(
            new ThrowAnimation(goblinCoo, imageScale),
            typeof(GoblinSpear),
            2  // Cooldown time in seconds
        )
        { }

        public override void PerformAttack(Enemy enemy)
        {
            if (AttackAnimation.HasEnded)
            {
                AttackAnimation.Reset();
                enemy.State = EnemyState.Idle;
                HasAttacked = false;
                // make the enemy wait a certain amount of time before attacking again
                enemy.AttackingCooldownTime = CooldownTime;
            }

            // On the last frame, if the spear hasn't been thrown yet, it throws it
            if (AttackAnimation.CurrentFrame == AttackAnimation.FrameCount - 1 && !HasAttacked)
            {
                // Adds the Spear to the list of damagers in the Battle Field
                BattleScene.Damagers.Add((Damager)Activator.CreateInstance(DamagerType, enemy.Coo));
                HasAttacked = true;  // inform that the spear has been thrown
            }
        }
    }

    public class ThrowAnimation : SimpleAnimation
    {
        public ThrowAnimation(Vector2 goblinPosition, float imageScale) : base
            (
                Path.Combine("Battle", "Characters", "Enemies", "SpearGoblin", "Throw"), // Path to the frames
                goblinPosition,  // InitialPosition
                6,               // FramesCount
                6f,              // Frame Speed (F/s)
                1.5f,            // Last Frame Time before animation Ends (s)
                imageScale       // scale for the frame images in the animation
            )
        {
        }

        public override void UpdateCoordinates(Vector2 goblinPosition)
        {
            Coo = goblinPosition;
        }
    }

    #endregion Attack MoveSet
}