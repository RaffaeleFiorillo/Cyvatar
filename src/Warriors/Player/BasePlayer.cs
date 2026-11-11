using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cyvatar.src.GameScenes;

namespace Cyvatar.src.Warriors
{
    public abstract class BasePlayer : Warrior
    {
        protected Dictionary<PlayerState, Texture2D> Images = new Dictionary<PlayerState, Texture2D>
        {
            {PlayerState.Idle, GameSceneManager.LoadImage(Path.Combine("Battle","Characters","BlackKnight","Idle.png")) },
            {PlayerState.Charge, GameSceneManager.LoadImage(Path.Combine("Battle","Characters","BlackKnight","Charging.png")) },
            {PlayerState.Block, GameSceneManager.LoadImage(Path.Combine("Battle","Characters","BlackKnight","Block.png")) },
            {PlayerState.Attack, GameSceneManager.LoadImage(Path.Combine("Battle","Characters","BlackKnight","Magic.png")) },
        };
        protected PlayerState State = PlayerState.Idle;
        protected ChargingAnimation chargingAnimation;
        // Action mapping (button name → method)
        protected Dictionary<string, System.Action> PlayerActions;

        #region Stats

        public float MaxHp { get; protected set; } = 100f;
        public float Hp { get; protected set; }
        public float MaxEnergy { get; protected set; } = 100f;
        public float Energy { get; protected set; }
        public float MaxMana { get; protected set; } = 100f;
        public float Mana { get; protected set; } = 0f;
        public float MaxSkillPoints { get; protected set; } = 10f;
        public float SkillPoints { get; protected set; }

        #endregion Stats

        #region Movement

        protected float MovementEnergyCost = 10f;
        protected float MovementCurrentCooldown = 0f;
        protected float MovementTotalCooldown = 1.0f;

        #endregion Movement

        #region Energy

        protected float EnergyChargingSpeed = 15f;               // Energy per second
        protected float EnergyDepletionCooldown = 0f;
        protected float EnergyDepletionTotalCooldown = 2.0f;
        protected float BlockEnergyConsumptionSpeed = 20f;       // Energy per second while blocking
        protected float AttackEnergyChargingCooldownTime = 0f;
        protected float AttackEnergyChargingMaxTime = 1.5f;

        #endregion Energy

        #region Mana

        protected float ManaChargingSpeed = 40f;                 // Mana per second while charging
        protected float ManaDischargingSpeed = 20f;              // Mana per second when not charging
        protected float ManaConservationInterval = 0f;

        #endregion Mana

        public BasePlayer() : base(2, 2) // Starting grid position
        {
            Hp = MaxHp;
            Energy = MaxEnergy;
            SkillPoints = MaxSkillPoints;

            PlayerActions = new Dictionary<string, System.Action>
            {
                { "l1", Charge },
                { "r1", Attack },
                { "r2", Block }
            };
            chargingAnimation = new ChargingAnimation(Coo);
        }

        /// <summary>
        /// The position of the player is based on a discrete X, Y coordinates system that locates him on the 8X4 BattleField Grid.
        /// But those coordinates must be converted to pixel coordinates for him to be displayed on the Screen.
        /// This method does the conversion, and is automatically called every time the X and/or Y coordinates of the player are changed.
        /// </summary>
        public override void UpdatePixelCoordinates()
        {
            Coo = new Vector2(X * 190, Y * -150 + 757);

            if (chargingAnimation != null)
                chargingAnimation.UpdateCoordinates(Coo);
        }

        #region Stats Alteration
        // These will be properly implemented in the class AttackPlayer so that it can directly affect the HUD

        public abstract void AddHp(float value);
        public abstract void AddEnergy(float value);
        public abstract void AddMana(float value);

        #endregion Stats Alteration

        #region Actions by Input

        /// <summary>
        /// This is the method that should be called in order to make the Player act according to the inputs.
        /// </summary>
        /// <param name="pressedButtons"></param>
        public virtual void Act(List<string> pressedButtons)
        {
            // Movement only allowed in idle or attack state
            if (State == PlayerState.Idle || State == PlayerState.Attack)
            {
                if (MovementCurrentCooldown <= 0 && Energy > 0)
                {
                    if (Movement(pressedButtons))
                        return;
                }
            }

            // Process action buttons (L1, R1, R2...)
            foreach (string button in pressedButtons)
            {
                if (PlayerActions.ContainsKey(button))
                {
                    PlayerActions[button].Invoke();
                    return; // Only one action per frame
                }
            }

            // Return to idle if no special state is active
            if (State != PlayerState.SpecialAttack)
            {
                State = PlayerState.Idle;
                chargingAnimation?.Reset();
            }
        }

        protected virtual bool Movement(List<string> pressedButtons)
        {
            bool hasMoved = false;

            if (Y > 0 && pressedButtons.Contains("down"))
            {
                Y--;
                hasMoved = true;
            }
            else if (Y < 3 && pressedButtons.Contains("up"))
            {
                Y++;
                hasMoved = true;
            }
            else if (X > 0 && pressedButtons.Contains("left"))
            {
                X--;
                hasMoved = true;
            }
            else if (X <= FieldRange && pressedButtons.Contains("right"))
            {
                X++;
                hasMoved = true;
            }

            if (hasMoved)
            {
                MovementCurrentCooldown = MovementTotalCooldown;
                AddEnergy(-MovementEnergyCost);
            }

            return hasMoved;
        }

        protected virtual void Charge()
        {
            State = PlayerState.Charge;

            if (Mana < MaxMana && chargingAnimation != null && chargingAnimation.CurrentFrame > 2)
            {
                AddMana(ManaChargingSpeed * BattleScene.dt);

                if (Mana >= MaxMana)
                {
                    Mana = MaxMana;
                    ManaConservationInterval = 1.5f;
                }
            }
        }

        protected virtual void Attack()
        {
            if (Energy <= 0 && EnergyDepletionCooldown <= EnergyDepletionTotalCooldown * 0.5f)
            {
                State = PlayerState.Idle;
                EnergyDepletionCooldown += BattleScene.dt;
                return;
            }

            State = PlayerState.Attack;
        }

        protected virtual void Block()
        {
            if (Energy > 0)
            {
                State = PlayerState.Block;
                AddEnergy(-BlockEnergyConsumptionSpeed * BattleScene.dt);
                return;
            }

            State = PlayerState.Idle;
            EnergyDepletionCooldown += BattleScene.dt;
        }

        #endregion Actions by Input

        public override void Update()
        {
            MovementCurrentCooldown -= BattleScene.dt;
            EnergyDepletionCooldown -= BattleScene.dt;
            AttackEnergyChargingCooldownTime -= BattleScene.dt;

            if (State != PlayerState.Charge)
            {
                ManaConservationInterval -= BattleScene.dt;
                if (ManaConservationInterval <= 0)
                    AddMana(-ManaDischargingSpeed * BattleScene.dt);
            }

            // Energy regeneration
            if (State != PlayerState.Block)
            {
                if (State == PlayerState.Attack && AttackEnergyChargingCooldownTime > 0)
                    return;

                AddEnergy(EnergyChargingSpeed * BattleScene.dt);
            }
        }

        public override void Draw()
        {
            if (Images.ContainsKey(State))
                GraphicManager.Screen.Draw(Images[State], Coo, Color.White);
            else
                throw new Exception($"Player has  invalid State: {State.ToString()}");

            if (State == PlayerState.Charge)
                chargingAnimation.Draw();
        }
    }

    public class ChargingAnimation
    {
        private List<Texture2D> _frames = new();
        private float _currentFrame = 0f;
        private const float _frameSpeed = 25f;
        private bool _firstLoop = true;
        private const int _frameCount = 47;
        private readonly string _framesPath = Path.Combine("Battle", "Characters", "Charging");

        private Vector2 Coo { get; set; }
        public int CurrentFrame { get => (int)_currentFrame; }

        public ChargingAnimation(Vector2 playerPosition)
        {
            UpdateCoordinates(playerPosition);
            _frames = Enumerable.Range(0, _frameCount).Select(
                i => GameSceneManager.LoadImage(Path.Combine(_framesPath, $"{i}.png"), scale: 1.6f)
            ).ToList();
        }

        public void UpdateCoordinates(Vector2 playerPosition)
        {
            Coo = new Vector2(playerPosition.X - 205, playerPosition.Y - 195);
        }

        public void Draw()
        {
            // Avoid displaying the empty frames (0 and 1) after the first loop
            if (!_firstLoop && _currentFrame < 2f)
                _currentFrame = 2f;
            // After frame 2 we consider the animation past the first loop
            else if (_currentFrame > 2f)
                _firstLoop = false;

            GraphicManager.Screen.Draw(_frames[(int)_currentFrame], Coo, Color.White);

            // Advance animation withing the boundaries of the existing frames
            _currentFrame = (_currentFrame + _frameSpeed*BattleScene.dt) % _frames.Count;
        }

        public void Reset()
        {
            _currentFrame = 0f;
            _firstLoop = true;
        }
    }
}