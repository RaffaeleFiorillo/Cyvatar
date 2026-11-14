using Cyvatar.src.GameScenes;
using Cyvatar.src.Utils.Animations;
using Cyvatar.src.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
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
        private readonly Texture2D HpBarrBorder = GameSceneManager.LoadImage(Path.Combine("Battle", "Characters", "Enemies", "HpBarrBorder.png"));

        private float XMovingProbability = 0.025f;
        private float YMovingProbability = 0.035f;
        private float MovingCooldownTime = 0;
        private float MaxMovingCooldownTime = 1f;

        private bool IsAttacking = false;
        private bool HasThrown = false;
        private float AttackingCooldownTime = 0;
        private float MaxAttackingCooldownTime = 6f;
        private float AttackingProbability = 0.015f;

        private const float MaxHp = 100;
        private Rectangle HpRectangle;
        private Color HpColor;

        public SpearGoblin() : base(Random.Shared.Next(5, 8), Random.Shared.Next(0, 3), MaxHp)
        {
            float imageScale = 2.2f;
            IdleImage = GameSceneManager.LoadImage(Path.Combine(ImagesPath, "Idle.png"), scale: imageScale);
            _throwAnimation = new ThrowAnimation(Coo, imageScale);
            UpdatePixelCoordinates();
            FieldRange = 3;
            HitBox = new Rectangle((int)Coo.X+20, (int)Coo.Y + 30, 100, 120);
            HpRectangle = new Rectangle((int)Coo.X + 2, (int)Coo.Y + 5, 100, 10);
            HpColor = Color.Green;
        }

        public override void UpdatePixelCoordinates()
        {
            Coo = new Vector2(X*197, Y*-145 + 820);
            _throwAnimation?.UpdateCoordinates(Coo);
        }

        public override void TakeDamageFrom(Damager damager)
        {
            Hp -= damager.Damage;
            Debug.WriteLine($"Hit! Remaining {Hp}");

            float hpPercentage = Hp / MaxHp;
            if (hpPercentage >= 75)
                HpColor = Color.Green;
            else if (hpPercentage >= 35)
                HpColor = Color.Orange;
            else
                HpColor = Color.Red;
            HpRectangle.Width = (int)(hpPercentage*MaxHp);
        }

        private void Move()
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

                    HitBox.X = (int)Coo.X + 20;
                    HitBox.Y = (int)Coo.Y + 30;

                    HpRectangle.X = (int)Coo.X + 2;
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
                    HitBox.Y = (int)Coo.Y + 30;

                    HpRectangle.X = (int)Coo.X + 2;
                    HpRectangle.Y = (int)Coo.Y + 5;
                }
            }
        }

        public override void Update()
        {
            MovingCooldownTime -= BattleScene.dt;

            if (IsAttacking)
            {
                _throwAnimation.Update(BattleScene.dt);

                if (_throwAnimation.HasEnded)
                {
                    _throwAnimation.Reset();
                    IsAttacking = false;
                    HasThrown = false;
                    AttackingCooldownTime = MaxAttackingCooldownTime;
                }

                // On the last frame, if the spear hasn't been thrown yet, it throws it
                if (_throwAnimation.CurrentFrame == _throwAnimation.FrameCount-1 && !HasThrown)
                {
                    BattleScene.Damagers.Add(new GoblinSpear(Coo));
                    HasThrown = true;
                }      
            }
            else
            {
                // When not attacking, the Goblin can move at random
                Move();

                // When not attacking, the attacking cooldown progresses
                AttackingCooldownTime -= BattleScene.dt;
                IsAttacking = (AttackingCooldownTime <= 0 && Random.Shared.NextDouble() <= AttackingProbability);
            }
        }

        public override void Draw()
        {
            GraphicManager.DrawRectangle(HpRectangle, HpColor);
            GraphicManager.Screen.Draw(HpBarrBorder, Coo, Color.White);

            if (IsAttacking)
                _throwAnimation.Draw();
            else
                GraphicManager.Screen.Draw(IdleImage, Coo, Color.White);
        }
    }

    public class ThrowAnimation: SimpleAnimation
    {
        public ThrowAnimation(Vector2 goblinPosition, float imageScale): base
            (
                Path.Combine("Battle", "Characters", "Enemies", "SpearGoblin"), // Path to the frames
                goblinPosition,  // InitialPosition
                6,               // FramesCount
                6f,              // Frame Speed (F/s)
                1.5f,            // Last Frame Time before animation Ends (s)
                imageScale             // scale for the frame images in the animation
            )
        {
        }

        public override void UpdateCoordinates(Vector2 goblinPosition)
        {
            Coo = goblinPosition;
        }
    }
}
