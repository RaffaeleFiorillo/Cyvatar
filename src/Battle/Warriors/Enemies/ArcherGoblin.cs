using Cyvatar.src.Battle.Weapons;
using Cyvatar.src.GameScenes;
using Cyvatar.src.Utils.Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;

namespace Cyvatar.src.Battle.Warriors
{
    public class ArcherGoblin : Goblin
    {
        private const float _imageScale = 2.2f;

        public ArcherGoblin() : base(
            GoblinType.ArcherGoblin,
            Random.Shared.Next(5, 8),
            Random.Shared.Next(0, 3),
            _imageScale,
            100
        )
        {
            MoveSet = new ArcherGoblinMoveSet(this, _imageScale);
            DeathAnimation = new ArrowGoblinDeathAnimation(Coo, _imageScale);
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
            Coo = new Vector2(X * 197, Y * -145 + 820);
            MoveSet?.UpdateCoordinates(Coo);
            DeathAnimation?.UpdateCoordinates(Coo);
            HitValues.RemoveAll(hv => true);
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


    public class ArrowGoblinDeathAnimation : SimpleAnimation
    {
        public ArrowGoblinDeathAnimation(Vector2 goblinPosition, float imageScale) : base
            (
                Path.Combine("Battle", "Characters", "Enemies", "ArcherGoblin", "Death"), // Path to the frames
                goblinPosition,  // InitialPosition
                10,               // FramesCount
                12f,              // Frame Speed (F/s)
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

    public class ArcherGoblinMoveSet : AttackMoveSet
    {
        public ArcherGoblinMoveSet(Enemy moveSetOwner, float imageScale) : base(moveSetOwner)
        {
            MoveSet.Add(new ArrowAttack(moveSetOwner.Coo, imageScale));
            MoveSet.Add(new MultiArrowAttack(moveSetOwner.Coo, imageScale));
        }
    }


    #region Attacks

    public class ArrowAttack : Attack
    {
        private bool HasAttacked = false;

        public ArrowAttack(Vector2 goblinCoo, float imageScale) : base(
            new ShootAnimation(goblinCoo, imageScale),
            0.5f  // Cooldown time in seconds
        )
        { }

        public override void Reset()  
        {
            base.Reset();
            HasAttacked = false;     
        }

        public override void Update(Enemy enemy)
        {
            HasEnded = AttackAnimation.HasEnded;

            // On the last frame, if the spear hasn't been thrown yet, it throws it
            if (AttackAnimation.CurrentFrame == AttackAnimation.FrameCount - 1 && !HasAttacked)
            {
                // Adds the Spear to the list of damagers in the Battle Field
                BattleField.Damagers.Add(new GoblinArrow(enemy.Coo));
                HasAttacked = true;  // inform that the spear has been thrown
            }
        }
    }

    public class MultiArrowAttack : Attack
    {
        private bool HasAttacked = false;

        /// <summary>
        /// The number of arrows to shoot has been decided.
        /// </summary>
        private bool HasChosenAttackNumber = false;
        private float FinalPoseTime = 0; // Amount of time (seconds) spent on the final pose;

        /// <summary>
        /// Number of arrows that are shot one after the other.
        /// </summary>
        private int attackNumber = 0;

        public MultiArrowAttack(Vector2 goblinCoo, float imageScale) : base(
            new ShootAnimation(goblinCoo, imageScale, framespeed:12,lastFrameTime:0),
            2f  // Cooldown time in seconds
        )
        { }

        public override void Reset()
        {
            base.Reset();
            HasChosenAttackNumber = false;
            HasAttacked = false;
            FinalPoseTime = 1;  // every final pose after a multi-shot lasts 1 second;
        }

        public override void Update(Enemy enemy)
        {
            if (!HasChosenAttackNumber)
            {
                attackNumber = Random.Shared.Next(1, 3);
                HasChosenAttackNumber = true;
            }

            if (AttackAnimation.HasEnded)
            {
                // if all arrows have been shot, the Attack reaches an end
                if (attackNumber == 0)
                {
                    HasEnded = FinalPoseTime <= 0; // Inform that the Attack has ended
                    FinalPoseTime -= GameScene.dt;
                    return;
                }

                AttackAnimation.Reset();
                attackNumber -= 1;
                HasAttacked = false;
            }

            // On the last frame, if the arrow hasn't been thrown yet, it throws it
            if (AttackAnimation.CurrentFrame == AttackAnimation.FrameCount - 1 && !HasAttacked)
            {
                // Adds the Spear to the list of damagers in the Battle Field
                BattleField.Damagers.Add(new GoblinArrow(enemy.Coo));
                HasAttacked = true;  // inform that the arrow has been shot
            }
        }
    }


    public class ShootAnimation : SimpleAnimation
    {
        public ShootAnimation(Vector2 goblinPosition, float imageScale, float framespeed = 6f, float lastFrameTime = 1.5f) : base
            (
                Path.Combine("Battle", "Characters", "Enemies", "ArcherGoblin", "Shoot"), // Path to the frames
                goblinPosition,  // InitialPosition
                5,               // FramesCount
                6f,              // Frame Speed (F/s)
                lastFrameTime,   // Last Frame Time before animation Ends (s)
                imageScale       // scale for the frame images in the animation
            )
        {
        }

        public override void UpdateCoordinates(Vector2 goblinPosition)
        {
            Coo = goblinPosition;
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

    #endregion Attacks
}
