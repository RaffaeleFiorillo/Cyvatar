using Cyvatar.src.Battle.Weapons;
using Cyvatar.src.GameScenes;
using Cyvatar.src.Utils.Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.IO;

namespace Cyvatar.src.Battle.Warriors
{
    public class SpearGoblin : Goblin
    {
        private const float _imageScale = 2.2f;

        public SpearGoblin() : base(
            GoblinType.SpearGoblin,
            5, // Random.Shared.Next(5, 8),
            Random.Shared.Next(0, 3),
            _imageScale, 
            100
        )
        {
            MoveSet = new SpearGoblinMoveSet(this, _imageScale);
            DeathAnimation = new SpearGoblinDeathAnimation(Coo, _imageScale);
            UpdateAfterMovement();
            HitBox = new Rectangle((int)Coo.X + 20, (int)Coo.Y + 30, 100, 120);

            MaxHp = 100;
            XMovingProbability = 0.025f;
            YMovingProbability = 0.055f;

            MaxMovingCooldownTime = 1f;
            AttackingProbability = 0.015f;
        }

        public override void UpdateAfterMovement()
        {
            Coo = new Vector2(20+X*188, Y*-145 + 820);
            MoveSet?.UpdateCoordinates(Coo);
            DeathAnimation?.UpdateCoordinates(Coo);
            HitValues.RemoveAll(hv => true);

            HitBox.X = (int)Coo.X + 20;
            HpRectangle.X = (int)Coo.X + 2;
            HitBox.Y = (int)Coo.Y + 30;
            HpRectangle.Y = (int)Coo.Y + 5;
        }

        public override void AddHp(float value)
        {
            base.AddHp(value);
        }

        protected override void Move()
        {
            RandomCroxMovement();
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

    public class SpearGoblinMoveSet : AttackMoveSet
    {
        public SpearGoblinMoveSet(Enemy moveSetOwner, float imageScale):base(moveSetOwner)
        {
            MoveSet.Add(new SpearThrowAttack(moveSetOwner.Coo, imageScale));
            MoveSet.Add(new SpearThrustAttack(moveSetOwner.Coo, imageScale));
        }
    }


    #region Throw Attack

    public class SpearThrowAttack : Attack
    {
        private bool _hasAttacked = false;

        public SpearThrowAttack(Vector2 goblinCoo, float imageScale) : base(
            new ThrowAnimation(goblinCoo, imageScale),
            2  // Cooldown time in seconds
        )
        { }

        public override void Reset()
        {
            base.Reset();
            _hasAttacked = false;
        }

        public override void Update(Enemy enemy)
        {
            HasEnded = AttackAnimation.HasEnded;

            // On the last frame, if the spear hasn't been thrown yet, it throws it
            if (AttackAnimation.CurrentFrame == AttackAnimation.FrameCount - 1 && !_hasAttacked)
            {
                // Adds the Spear to the list of damagers in the Battle Field
                BattleField.Damagers.Add(new GoblinThrowSpear(enemy.Coo));
                _hasAttacked = true;  // inform that the spear has been thrown
            }
        }
    }

    public class GoblinThrowSpear : SimpleProjectile
    {
        private static Texture2D Image;
        private const float _damage = 30;
        private const float _speed = 600;  // Speed of the spear in m/s

        public GoblinThrowSpear(Vector2 initialCoo) : base(initialCoo, _damage, _speed, OriginType.EnemyFire)
        {
            Coo = new Vector2(initialCoo.X, initialCoo.Y + 50);
            Image = GameSceneManager.LoadImage(Path.Combine(_baseImagePath, "GoblinSpear.png"), scale: 2.2f);
        }

        protected override Texture2D GetImage() { return Image; }

        public override Rectangle GetHitbox()
        {
            // Example: adjust these values to match the sprite size
            return new Rectangle((int)Coo.X, (int)Coo.Y, 100, 30);
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

    #endregion Throw Attack

    #region Thrust Attack

    public class SpearThrustAttack : Attack
    {
        private InvisibleDamager _spearHead;
        private bool _hasAttacked = false;
        private bool _enemyCooHasBeenReset = false;

        public SpearThrustAttack(Vector2 goblinCoo, float imageScale) : 
            base(
                new SpearThrustAnimation(goblinCoo, imageScale),
                2  // Cooldown time in seconds
            )
        { }

        public override void Reset()
        {
            base.Reset();
            _spearHead = null;
            _hasAttacked = false;
            _enemyCooHasBeenReset = false;
        }

        public override void Update(Enemy enemy)
        {
            HasEnded = AttackAnimation.HasEnded;

            // Damage is dealt when the goblin is fully extended forward.
            if (((BoomerangAnimation)AttackAnimation).AtMiddlePoint)
            {
                if (!_hasAttacked)
                {
                    enemy.Coo = new Vector2(enemy.Coo.X - 20, enemy.Coo.Y);  // Make the Goblin go a little further when thrusting.
                    AttackAnimation.UpdateCoordinates(enemy.Coo);
                    _spearHead = new GoblinThrustSpear(enemy.Coo);
                    BattleField.Damagers.Add(_spearHead);
                    _hasAttacked = true;
                }
                return;
            } 

            if (_hasAttacked && !_enemyCooHasBeenReset)
            {
                enemy.Coo = new Vector2(enemy.Coo.X +20, enemy.Coo.Y);  // Make the Goblin go back to normal position
                AttackAnimation.UpdateCoordinates(enemy.Coo);
                _enemyCooHasBeenReset = true;
            }

            // When the Goblin retracts the spear to go back to the idle position, the Damager is destroyed to simulate the end of the attack.
            _spearHead?.Destroy();
        }
    }

    public class SpearThrustAnimation : BoomerangAnimation
    {
        public SpearThrustAnimation(Vector2 goblinPosition, float imageScale) : base
            (
                Path.Combine("Battle", "Characters", "Enemies", "SpearGoblin", "Thrust"), // Path to the frames
                goblinPosition,  // InitialPosition
                3,               // FramesCount
                6f,              // Frame Speed (F/s)
                0.5f,            // Last Frame Time before animation starts going backwards.
                imageScale       // scale for the frame images in the animation
            )
        {

        }

        public override void UpdateCoordinates(Vector2 goblinPosition)
        {
            Coo = new Vector2(goblinPosition.X-60, goblinPosition.Y); ;
        }
    }

    public class GoblinThrustSpear : InvisibleDamager
    {
        private const float _damage = 50;

        public GoblinThrustSpear(Vector2 referenceCoo) : base(OriginType.EnemyFire, _damage, referenceCoo) { }

        protected override void GenerateHitbox(Vector2 referenceCoo)
        {
            _hitBox = new Rectangle((int)referenceCoo.X - 60, (int)referenceCoo.Y + 70, 30, 30);
        }
    }

    #endregion Thrust Attack
}