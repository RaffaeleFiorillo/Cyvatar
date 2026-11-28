using Cyvatar.src.Battle.Warriors;
using Cyvatar.src.GameScenes;
using Cyvatar.src.Utils.Animations;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyvatar.src.Battle.Effects
{
    public class LavaFloorEffect: FieldCellEffect
    {
        /// <summary>
        /// Amount of Damage dealt every second to the Warrior that steps on the Cell where the effect was placed.
        /// </summary>
        private const float DamageSpeed = 5;

        /// <summary>
        /// Amount of seconds the Effect stays on the Cell before being removed.
        /// </summary>
        private float LifeTime = 60;

        public LavaFloorEffect(int x, int y) : base(x, y, 5)
        {
            CreationAnimation = new LavaFloorCreationAnimation(Coo);
            EffectAnimation = new LavaFloorAnimation(Coo);
        }

        public override void ApplyEffect(Warrior warrior)
        {
            warrior.AddHp(-DamageSpeed*BattleScene.dt);
        }

        public override void Update()
        {
            base.Update();

            LifeTime -= BattleScene.dt;
            ToBeDeleted = LifeTime <= 0;
        }
    }

    internal class LavaFloorAnimation : BoomerangAnimation
    {
        public LavaFloorAnimation(Vector2 cellPosition) : base
        (
            Path.Combine("Battle", "Effects", "Lava"),
            cellPosition,
            3,
            25,
            0.5f
        )
        {
            UpdateCoordinates(cellPosition);
        }

        /// <summary>
        /// Adjusts the position of the animation according to the position of the player.
        /// </summary>
        /// <param name="playerPosition"></param>
        public override void UpdateCoordinates(Vector2 coo)
        {
            Coo = coo;
        }

        public void Update() { base.Update(GameScene.dt); }
    }

    internal class LavaFloorCreationAnimation : SimpleAnimation
    {
        public LavaFloorCreationAnimation(Vector2 cellPosition) : base
            (
                Path.Combine("Battle", "Effects", "Lava", "Creation"), // Path to the frames
                cellPosition,  // InitialPosition
                10,            // FramesCount
                12f,           // Frame Speed (F/s)
                0              // Last Frame Time before animation Ends (s)
            )
        {
        }

        public override void UpdateCoordinates(Vector2 coo){Coo = coo;}
    }
}
