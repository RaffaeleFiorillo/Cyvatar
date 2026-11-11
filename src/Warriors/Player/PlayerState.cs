using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyvatar.src.Warriors
{
    /// <summary>
    /// According to the current state of the player, he presents different behaviours.
    /// </summary>
    public enum PlayerState
    {
        Idle,
        Attack,
        Block,
        Charge,
        SpecialAttack
    }
}
