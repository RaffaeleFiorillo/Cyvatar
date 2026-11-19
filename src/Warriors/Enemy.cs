using Cyvatar.src.GameScenes;
using Cyvatar.src.Utils.Animations;
using Cyvatar.src.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace Cyvatar.src.Warriors
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

        protected SimpleAnimation DeathAnimation;

        #endregion Properties

        protected Enemy(int x, int y, float hp) : base(x, y, hp){}

        protected abstract void Move();

        public override void TakeDamageFrom(Damager damager)
        {
            Hp -= damager.DealsContinuousDamage ? damager.Damage*BattleScene.dt : damager.Damage;
            // Debug.WriteLine($"Hit! Remaining {Hp}");

            float hpPercentage = Hp / MaxHp;
            if (hpPercentage >= 0.75)
                HpColor = Color.Green;
            else if (hpPercentage >= 0.35)
                HpColor = Color.Orange;
            else
                HpColor = Color.Red;

            State = Hp <= 0 ? EnemyState.Dead : EnemyState.Idle;

            MoveSet.Reset();
            HpRectangle.Width = (int)(hpPercentage * MaxHp);
        }

        public override void Update()
        {
            MovingCooldownTime -= BattleScene.dt;

            switch (State)
            {
                case EnemyState.Dead:
                    DeathAnimation.Update(BattleScene.dt);
                    break;

                case EnemyState.Attacking:
                    MoveSet.Update();
                    break;

                default:
                    // When not attacking, the Goblin can move at random
                    Move();
                    // When not attacking, the attacking cooldown progresses
                    AttackingCooldownTime -= BattleScene.dt;
                    if (AttackingCooldownTime <= 0 && Random.Shared.NextDouble() <= AttackingProbability)
                        State = EnemyState.Attacking;
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
                    DeathAnimation.Draw();
                    if (DeathAnimation.HasEnded)
                        IsDead = true;
                    break;

                case EnemyState.Attacking:
                    MoveSet.Draw();
                    break;

                default:
                    GraphicManager.Screen.Draw(IdleImage, Coo, Color.White);
                    break;
            }
        }
    }

    public class AttackMoveSet
    {
        protected Enemy MoveSetOwner;

        protected List<Attack> MoveSet = new List<Attack>();

        protected Attack ActiveAttack { get => MoveSet[_activeAttackIndex]; }
        protected int _activeAttackIndex = 0;
        protected bool HasChosenAttack = false;

        public AttackMoveSet(Enemy moveSetOwner)
        {
            MoveSetOwner = moveSetOwner;
        }

        public virtual void Reset()
        {
            ActiveAttack.AttackAnimation.Reset();
            HasChosenAttack = false;
        }

        public virtual void UpdateCoordinates(Vector2 coo) { }

        public void Update()
        {
            // If an attack has not yet been chosen, then choose one.
            if (!HasChosenAttack)
            {
                _activeAttackIndex = Random.Shared.Next(MoveSet.Count);
                HasChosenAttack = true;
            }
            ActiveAttack.PerformAttack(MoveSetOwner);

            ActiveAttack.AttackAnimation.Update(BattleScene.dt);
        }

        public void Draw()
        {
            ActiveAttack.AttackAnimation.Draw();
        }
    }

    /// <summary>
    /// Represents the act of performing an attack and therefore use a Damager.
    /// </summary>
    public abstract class Attack
    {
        /// <summary>
        /// Animation of the Enemy warrior when he performs this attack.
        /// </summary>
        public Animation AttackAnimation;

        /// <summary>
        /// Type of Damager to be instanciated when this attack is used.
        /// </summary>
        protected Type DamagerType;

        /// <summary>
        /// Time the enemy has to wait after using this attack.
        /// </summary>
        protected float CooldownTime;

        public Attack(Animation attackAnimation, Type damagerType, float cooldownTime)
        {
            AttackAnimation = attackAnimation;
            DamagerType = damagerType;
            CooldownTime = cooldownTime;
        }

        /// <summary>
        /// This method should be called, by the AttackMoveset class that holds it.
        /// The call should be made every time the enemy is in the Attacking state until the attack has reached an end.
        /// </summary>
        /// <param name="enemy">The Enemy Warrior performing the attack.</param>
        public abstract void PerformAttack(Enemy enemy);
    }
}
