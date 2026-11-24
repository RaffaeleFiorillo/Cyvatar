using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Cyvatar.src.GameScenes;
using Cyvatar.src.Utils.Animations;
using Cyvatar.src.Warriors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyvatar.src.Weapons
{
    public class AttackMoveSet
    {
        #region Properties

        protected Enemy MoveSetOwner;

        protected List<Attack> MoveSet = new List<Attack>();

        protected Attack ActiveAttack { get => MoveSet[_activeAttackIndex]; }
        protected int _activeAttackIndex = 0;
        protected bool HasChosenAttack = false;

        #endregion Properties

        /// <summary>
        /// The Childs of this class MUST add all their attacks in the MoveSet list, inside of their own constructor.
        /// </summary>
        /// <param name="moveSetOwner">The enemy that uses this Move Set.</param>
        public AttackMoveSet(Enemy moveSetOwner)
        {
            MoveSetOwner = moveSetOwner;
        }

        public virtual void EndAttackState()
        {
            // When the attack reaches an end, the Enemy goes back to the Idle state
            MoveSetOwner.State = MoveSetOwner.State == EnemyState.Attacking ? EnemyState.Idle : MoveSetOwner.State;
            // make the enemy wait a certain amount of time before attacking again
            MoveSetOwner.AttackingCooldownTime = ActiveAttack.CooldownTime;
            HasChosenAttack = false;  // Next time the Enemy enters Attacking state, it chooses again between the available attacks.
            ActiveAttack.Reset();
        }

        public void UpdateCoordinates(Vector2 coo)
        {
            foreach (var attack in MoveSet) attack.AttackAnimation.UpdateCoordinates(coo);
        }

        public void Update()
        {
            // If an attack has not yet been chosen, then choose one.
            if (!HasChosenAttack)
            {
                _activeAttackIndex = Random.Shared.Next(MoveSet.Count);
                HasChosenAttack = true;
                // Debug.WriteLine($"Has chosen attack {_activeAttackIndex}");
            }

            ActiveAttack.Update(MoveSetOwner);
            ActiveAttack.AttackAnimation.Update(BattleScene.dt);

            if (ActiveAttack.HasEnded) EndAttackState();
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
        /// Time the enemy has to wait after using this attack.
        /// </summary>
        public float CooldownTime;

        public bool HasEnded = false;

        public Attack(Animation attackAnimation, float cooldownTime)
        {
            AttackAnimation = attackAnimation;
            CooldownTime = cooldownTime;
        }

        public virtual void Reset()
        {
            HasEnded = false;
            AttackAnimation.Reset();
        }

        /// <summary>
        /// This method should be called, by the AttackMoveset class that holds it.
        /// The call should be made every time the enemy is in the Attacking state until the attack has reached an end.
        /// </summary>
        /// <param name="enemy">The Enemy Warrior performing the attack.</param>
        public abstract void Update(Enemy enemy);
    }
}
