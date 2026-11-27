using Cyvatar.src.Battle.Weapons;
using Cyvatar.src.GameScenes;
using Cyvatar.src.Utils.Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Cyvatar.src.Battle.Warriors
{
    public enum EnemyState
    {
        /// <summary>
        /// The idle state is the one that holds the logic to switch to the other States during the enemy's existence.
        /// Once the other state has ended, the enemy should return to Idle.
        /// </summary>
        Idle,

        /// <summary>
        /// The moving State should only be useful when the enemy has multy-step movements.
        /// Otherwise, the Idle state should be the one handling the movement.
        /// Some Enemies could even be able to perform movements while attacking.
        /// </summary>
        Moving,

        /// <summary>
        /// An enemy can be able to perform one or more attacks. 
        /// This state defines being in the process of performing said attacks.
        /// Some Enemies could even be able to perform movements while attacking.
        /// </summary>
        Attacking,

        /// <summary>
        /// This state indicates that the player has reached an HP of 0. 
        /// But being in this State does not necessarily mean that the enemy instance has been, or should be, removed from the Battle.
        /// The removal of the Enemy istance only happens when the IsDead property is set to true.
        /// </summary>
        Dead
    }

    public abstract class Enemy : Warrior
    {
        #region Properties

        public EnemyState State = EnemyState.Idle;
        protected Texture2D IdleImage;

        protected static readonly string BaseImagesPath = Path.Combine("Battle", "Characters", "Enemies");

        protected static readonly Texture2D HpBarrBorder = GameSceneManager.LoadImage(Path.Combine("Battle", "Characters", "Enemies", "HpBarrBorder.png"));       
        protected Rectangle HpRectangle;
        protected Color HpColor = Color.Green;
        protected float MaxHp = 100;

        protected AttackMoveSet MoveSet;
        protected float AttackingProbability;
        public float AttackingCooldownTime;

        protected float MovingCooldownTime;

        /// <summary>
        /// Flag that indicates if the Enemy should be Removed from the Battle Field.
        /// </summary>
        new public bool IsDead { get => State == EnemyState.Dead && DeathAnimation.HasEnded; }

        protected SimpleAnimation DeathAnimation;

        #endregion Properties

        protected Enemy(int x, int y, float hp) : base(x, y, hp){}

        /// <summary>
        /// Moving logic of the enemy
        /// </summary>
        /// <returns>true if the player has moved; false: if the player stayed in the same position</returns>
        protected abstract void Move();

        public override void TakeDamageFrom(Damager damager)
        {
            float damageTaken = damager.DealsContinuousDamage ? damager.Damage * GameScene.dt : damager.Damage;
            if (HitValues.Count > 5) HitValues.RemoveAt(0);
            HitValues.Add(new HitValue(damageTaken, CreateRandomHitValuePosition()));  // Display the damage taken on the screen
            Hp -= damageTaken;

            // Debug.WriteLine($"Hit! Remaining {Hp}");

            float hpPercentage = Hp / MaxHp;
            if (hpPercentage >= 0.75)
                HpColor = Color.Green;
            else if (hpPercentage >= 0.35)
                HpColor = Color.Orange;
            else
                HpColor = Color.Red;

            State = Hp <= 0 ? EnemyState.Dead : EnemyState.Idle;

            MoveSet.EndAttackState();
            HpRectangle.Width = (int)(hpPercentage * MaxHp);
        }

        public override void Update()
        {
            base.Update();

            MovingCooldownTime -= GameScene.dt;

            switch (State)
            {
                case EnemyState.Dead:
                    DeathAnimation.Update(GameScene.dt);
                    break;

                case EnemyState.Attacking:
                    MoveSet.Update();
                    break;

                default:
                    // When not attacking, the Goblin can move at random
                    Move();
                    // When not attacking, the attacking cooldown progresses
                    AttackingCooldownTime -= GameScene.dt;
                    if (AttackingCooldownTime <= 0 && Random.Shared.NextDouble() <= AttackingProbability)
                    {
                        State = EnemyState.Attacking;
                    }
                    break;
            }
        }

        public override void Draw()
        {
            GraphicManager.DrawRectangle(HpRectangle, HpColor);
            GraphicManager.Screen.Draw(HpBarrBorder, Coo, Color.White);

            switch (State)
            {
                case EnemyState.Dead:
                    Debug.WriteLine("Is Dead");
                    DeathAnimation.Draw();
                    break;

                case EnemyState.Attacking:
                    MoveSet.Draw();
                    break;

                default:
                    GraphicManager.Screen.Draw(IdleImage, Coo, Color.White);
                    break;
            }

            foreach (var hitValue in HitValues) hitValue.Draw();
        }
    }
}
