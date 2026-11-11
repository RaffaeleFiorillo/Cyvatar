using Cyvatar.src.GameScenes;
using Cyvatar.src.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyvatar.src.Warriors
{

    public class SpearGoblin : Warrior
    {
        private ThrowAnimation _throwAnimation;
        private static Texture2D IdleImage;
        private readonly string ImagesPath = Path.Combine("Battle", "Characters", "Enemies", "SpearGoblin");

        private float XMovingProbability = 0.025f;
        private float YMovingProbability = 0.035f;
        private float MovingCooldownTime = 0;
        private float MaxMovingCooldownTime = 1f;

        private bool IsAttacking = false;
        private float AttackingCooldownTime = 0;
        private float MaxAttackingCooldownTime = 3f;
        private float AttackingProbability = 0.015f;

        public SpearGoblin() : base(Random.Shared.Next(5, 8), Random.Shared.Next(0, 3))
        {
            IdleImage = GameSceneManager.LoadImage(Path.Combine(ImagesPath, "Idle.png"), scale: 3f);
            _throwAnimation = new ThrowAnimation(Coo);
            UpdatePixelCoordinates();
            FieldRange = 3;
        }

        public override void UpdatePixelCoordinates()
        {
            Coo = new Vector2(X*193, Y*-150 + 757);
            _throwAnimation?.UpdateCoordinates(Coo);
        }

        public override void Update()
        {
            MovingCooldownTime -= BattleScene.dt;
            AttackingCooldownTime -= BattleScene.dt;

            if (IsAttacking)
            {
                if (_throwAnimation.HasFinished)
                {
                    _throwAnimation.Reset();
                    IsAttacking = false;
                }
            } // When not attacking, the Goblin can move at random
            else if (MovingCooldownTime <= 0)
            {
                if (Random.Shared.NextDouble() < YMovingProbability)
                {
                    if (Random.Shared.NextDouble() > 0.5f)
                        Y += Y < 3 ? 1 : 0;
                    else
                        Y -= Y > 0 ? 1 : 0;
                    MovingCooldownTime = MaxMovingCooldownTime;
                }
                else if (Random.Shared.NextDouble() < XMovingProbability)
                {
                    if (Random.Shared.NextDouble() > 0.5f)
                        X += X < 9 ? 1 : 0;
                    else
                        X -= X >= 9-FieldRange ? 1 : 0;
                    MovingCooldownTime = MaxMovingCooldownTime;
                }
            }

            if (AttackingCooldownTime < 0 && Random.Shared.NextDouble() <= AttackingProbability)
            {
                IsAttacking = true;
            }
        }

        public override void Draw()
        {
            if (IsAttacking)
                _throwAnimation.Draw();
            else
                GraphicManager.Screen.Draw(IdleImage, Coo, Color.White);
        }
    }

    public class ThrowAnimation
    {
        private List<Texture2D> _frames = new();
        private float _currentFrame = 0f;
        private const float _frameSpeed = 5f;
        private const int _frameCount = 5;
        private readonly string _framesPath = Path.Combine("Battle", "Characters", "Enemies", "SpearGoblin");
        public bool HasFinished = true;

        private Vector2 Coo { get; set; }
        public int CurrentFrame { get => (int)_currentFrame; }

        public ThrowAnimation(Vector2 goblinPosition)
        {
            Coo = goblinPosition;
            _frames = Enumerable.Range(0, _frameCount).Select(
                i => GameSceneManager.LoadImage(Path.Combine(_framesPath, $"{i}.png"), scale: 3f)
            ).ToList();
        }

        public void UpdateCoordinates(Vector2 goblinPosition)
        {
            Coo = goblinPosition;
        }

        public void Draw()
        {
            // After frame 2 we consider the animation past the first loop
            if (_currentFrame > _frameCount)
                HasFinished = true;

            GraphicManager.Screen.Draw(_frames[(int)_currentFrame], Coo, Color.White);

            // Advance animation withing the boundaries of the existing frames
            _currentFrame = (_currentFrame + _frameSpeed * BattleScene.dt) % _frames.Count;
        }

        public void Reset()
        {
            _currentFrame = 0f;
            HasFinished = false;
        }
    }
}
