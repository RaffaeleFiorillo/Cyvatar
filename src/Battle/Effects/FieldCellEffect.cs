using Cyvatar.src.GameScenes;
using Cyvatar.src.Utils.Animations;
using Cyvatar.src.Battle.Warriors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Cyvatar.src.Battle.Effects
{
    public enum CellEffectState
    {
        /// <summary>
        /// The CreationAnimation is still being displayed.
        /// </summary>
        Creation,

        /// <summary>
        /// The CreationAnimation is still being displayed, but now the EffectAnimation is displayed under (simulating the transition). 
        ///  The Effect is not Active yet. 
        /// </summary>
        Activation,

        /// <summary>
        /// The Effect of the cell affects the Warriors that step on the Cell containing it.
        /// </summary>
        Active
    }

    public abstract class FieldCellEffect
    {
        #region Properties

        public int X;
        public int Y;
        protected Vector2 Coo;

        protected Animation CreationAnimation;
        protected Animation EffectAnimation;
        public CellEffectState State = CellEffectState.Creation;

        /// <summary>
        /// When this Flag is set to true, the Effect is removed from the BattleField.
        /// </summary>
        public bool ToBeDeleted = false;

        /// <summary>
        /// When the CreationAnimation reaches this frame, the Effect enters the Activation State.
        /// </summary>
        protected int ActivationFrame;

        #endregion Properties

        public FieldCellEffect(int x, int y, int activationFrame)
        {
            X = x;
            Y = y;
            Coo = new Vector2(20+x*189, y*-143+872);
            ActivationFrame = activationFrame;
            BattleField.PlaceEffectInCell(this, x, y);
        }

        public abstract void ApplyEffect(Warrior warrior);

        public virtual void Update()
        {
            if (ToBeDeleted)
            {
                BattleField.RemoveEffectFromCell(this);
                return;
            }

            if (CreationAnimation.CurrentFrame < ActivationFrame)
            {
                State = CellEffectState.Creation;
                CreationAnimation.Update(BattleScene.dt);
            }
            else if (CreationAnimation.CurrentFrame >= ActivationFrame && !CreationAnimation.HasEnded)
            {
                State = CellEffectState.Activation;
                CreationAnimation.Update(BattleScene.dt);
                EffectAnimation.Update(BattleScene.dt);
            }
            else
            {
                State = CellEffectState.Active;
                EffectAnimation.Update(BattleScene.dt);
            }
        }

        public void Draw()
        {
            switch (State)
            {
                case CellEffectState.Creation:
                    CreationAnimation.Draw();
                    break;
                case CellEffectState.Activation:
                    EffectAnimation.Draw();
                    CreationAnimation.Draw();
                    break;
                case CellEffectState.Active:
                    EffectAnimation.Draw();
                    break;
            }
        }
    }
}