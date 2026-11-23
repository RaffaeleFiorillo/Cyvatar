using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Cyvatar.src.GameScenes;
using Cyvatar.src.Utils.Animations;

namespace Cyvatar.src.Warriors
{
    public abstract class BasePlayer : Warrior
    {
        protected Dictionary<PlayerState, Texture2D> Images = new Dictionary<PlayerState, Texture2D>
        {
            {PlayerState.Idle, GameSceneManager.LoadImage(Path.Combine("Battle","Characters","Player","Idle.png")) },
            {PlayerState.Charge, GameSceneManager.LoadImage(Path.Combine("Battle","Characters","Player","Charging.png")) },
            // {PlayerState.Block, GameSceneManager.LoadImage(Path.Combine("Battle","Characters","Player","Block.png")) },
            {PlayerState.Attack, GameSceneManager.LoadImage(Path.Combine("Battle","Characters","Player","Magic.png")) },
        };
        protected PlayerState State = PlayerState.Idle;
        protected AuraCharge auraCharge;
        protected ForceFieldBarrier forceField;
        // Action mapping (button name → method)
        protected Dictionary<string, System.Action> PlayerActions;

        #region Stats

        public const float MaxHp = 100f;
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

        public BasePlayer() : base(2, 2, MaxHp) // Starting grid position
        {
            Energy = MaxEnergy;
            SkillPoints = MaxSkillPoints;

            PlayerActions = new Dictionary<string, System.Action>
            {
                { "l1", Charge },
                { "r1", Attack },
                { "r2", Block }
            };

            auraCharge = new AuraCharge(Coo);
            forceField = new ForceFieldBarrier(Coo);
            HitBox = new Rectangle((int)Coo.X, (int)Coo.Y+100, 160, 120);
        }

        /// <summary>
        /// The position of the player is based on a discrete X, Y coordinates system that locates him on the 8X4 BattleField Grid.
        /// But those coordinates must be converted to pixel coordinates for him to be displayed on the Screen.
        /// This method does the conversion, and is automatically called every time the X and/or Y coordinates of the player are changed.
        /// </summary>
        public override void UpdatePixelCoordinates()
        {
            Coo = new Vector2(X * 190, Y * -150 + 757);

            auraCharge?.UpdateCoordinates(Coo);
            forceField?.UpdateCoordinates(Coo);
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
                auraCharge.Stop();
                forceField.Stop();
                State = PlayerState.Idle;
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
                HitValues.RemoveAll( hv => true);
                HitBox = new Rectangle((int)Coo.X, (int)Coo.Y + 100, 160, 120);
                AddEnergy(-MovementEnergyCost);
            }

            return hasMoved;
        }

        protected virtual void Charge()
        {
            State = PlayerState.Charge;
            auraCharge.IsActive = true;
            forceField.Stop();

            if (Mana < MaxMana && auraCharge.IsCharging)
                AddMana(ManaChargingSpeed * BattleScene.dt);

            if (Mana >= MaxMana)
            {
                Mana = MaxMana;
                auraCharge.IsAtMaxMana = true;
                ManaConservationInterval = 1.5f;
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

            auraCharge.Stop();
            forceField.Stop();
            State = PlayerState.Attack;
        }

        protected virtual void Block()
        {
            auraCharge.Stop();

            if (Energy > 0)
            {
                State = PlayerState.Block;
                if (forceField.BarrierIsComplete)
                    AddEnergy(-BlockEnergyConsumptionSpeed * BattleScene.dt);
                forceField.IsActive = true;
                return;
            }

            forceField.Stop();
            State = PlayerState.Idle;
            EnergyDepletionCooldown += BattleScene.dt;
        }

        #endregion Actions by Input

        public override void Update()
        {
            MovementCurrentCooldown -= BattleScene.dt;
            EnergyDepletionCooldown -= BattleScene.dt;
            AttackEnergyChargingCooldownTime -= BattleScene.dt;
            auraCharge.Update();
            forceField.Update();

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

            if (State == PlayerState.Charge)
                auraCharge.Draw();
            else if (State == PlayerState.Block)
                forceField.Draw();
        }
    }

    #region Mana Aura Charging

    public class AuraCharge
    {
        public ChargingAnimation _animation;
        private Sound _sparkSound;
        private Sound _chargeSound;
        private Sound _maxAuraSound;
        public bool IsActive = false;
        private bool IsStart = true;
        private float _timeFromStart = 0;
        public bool IsCharging { get => _timeFromStart >= 0.25 && !IsAtMaxMana; }
        public bool IsAtMaxMana = false;

        public AuraCharge(Vector2 playerPosition)
        {
            _animation = new ChargingAnimation(playerPosition);
            _sparkSound = new Sound(Path.Combine("Player", "Aura", "AuraSpark.wav"));
            _chargeSound = new Sound(Path.Combine("Player", "Aura", "AuraCharge.wav"), isLoop: true);
            _maxAuraSound = new Sound(Path.Combine("Player", "Aura", "MaxAura.wav"), isLoop: true, volume:0.7f);
        }

        public void UpdateCoordinates(Vector2 playerPosition)
        {
            _animation.UpdateCoordinates(playerPosition);
        }

        public void Stop()
        {
            IsActive = false;
            IsStart = true;
            _timeFromStart = 0;
            IsAtMaxMana = false;
            _maxAuraSound.Stop();
            _sparkSound.Stop();
            _chargeSound.Stop();
            _animation?.Reset();
        }

        public void Update()
        {
            if (!IsActive) return;

            _animation.Update();
            _timeFromStart += BattleScene.dt;

            // When the charging starts, and the Mana is not yet rising
            if (IsStart)
            {
                IsStart = false;
                _sparkSound.Play();
            }

            if (IsAtMaxMana)
            {
                _chargeSound.Stop();
                _maxAuraSound.Play();
            }
            // Charging Mana
            else if (IsCharging)
            {
                _chargeSound.Play();
                _maxAuraSound.Stop();
            }
        }

        public void Draw()
        {
            _animation.Draw();
        }
    }

    public class ChargingAnimation: LoopAnimatioon
    {
        public ChargingAnimation(Vector2 playerPosition) : base
        (
            Path.Combine("Battle", "Characters", "Player","Charging"),
            playerPosition,
            47,
            25,
            1.6f
        )
        {
            UpdateCoordinates(playerPosition);
        }

        /// <summary>
        /// Adjusts the position of the animation according to the position of the player.
        /// </summary>
        /// <param name="playerPosition"></param>
        public override void UpdateCoordinates(Vector2 playerPosition)
        {
            Coo = new Vector2(playerPosition.X - 205, playerPosition.Y - 195);
        }

        public void Update() { base.Update(BattleScene.dt); }
    }

    #endregion Mana Aura Charging

    #region Force Field Barrier

    public class ForceFieldBarrier
    {
        public BarrierAnimation _barrierAnimation;
        public BlockingAnimation _blockingAnimation;
        private Sound _invokingSound;
        private Sound _maxBarrierSound;
        public bool IsActive = false;
        private bool IsStart = true;
        public bool BarrierIsComplete { get => _barrierAnimation.HasEnded; }

        public ForceFieldBarrier(Vector2 playerPosition)
        {
            _barrierAnimation = new BarrierAnimation(playerPosition);
            _blockingAnimation = new BlockingAnimation(playerPosition);
            _invokingSound = new Sound(Path.Combine("Player", "ForceField", "Invoking.wav"), isLoop: false, volume:0.8f);
            _maxBarrierSound = new Sound(Path.Combine("Player", "ForceField", "FullBarrier.wav"), isLoop: true, volume: 0.15f);
        }

        public void UpdateCoordinates(Vector2 playerPosition)
        {
            _barrierAnimation.UpdateCoordinates(playerPosition);
            _blockingAnimation.UpdateCoordinates(playerPosition);
        }

        public void Stop()
        {
            IsActive = false;
            IsStart = true;
            _maxBarrierSound.Stop();
            _invokingSound.Stop();
            _barrierAnimation?.Reset();
            _blockingAnimation?.Reset();
        }

        public void Update()
        {
            if (!IsActive) return;

            _blockingAnimation.Update();
            _barrierAnimation.Update();

            // When the charging starts, and the Mana is not yet rising
            if (IsStart)
            {
                IsStart = false;
                _invokingSound.Play();
            }

            if (BarrierIsComplete)
            {
                _invokingSound.Stop();
                _maxBarrierSound.Play();
            }
        }

        public void Draw()
        {
            _blockingAnimation.Draw();
            _barrierAnimation.Draw();  
        }
    }

    public class BarrierAnimation : SimpleAnimation
    {
        public BarrierAnimation(Vector2 playerPosition) : base
        (
            Path.Combine("Battle", "Characters", "Player", "ForceField"),
            playerPosition,
            5,
            10,
            0
        )
        {
            UpdateCoordinates(playerPosition);
        }

        /// <summary>
        /// Adjusts the position of the animation according to the position of the player.
        /// </summary>
        /// <param name="playerPosition">Current position of the player.</param>
        public override void UpdateCoordinates(Vector2 playerPosition)
        {
            Coo = new Vector2(playerPosition.X + 130, playerPosition.Y);
        }

        public void Update() { base.Update(BattleScene.dt); }
    }

    public class BlockingAnimation : SimpleAnimation
    {
        public BlockingAnimation(Vector2 playerPosition) : base
        (
            Path.Combine("Battle", "Characters", "Player", "Blocking"),
            playerPosition,
            5,
            10,
            0,
            0.28f
        )
        {
            UpdateCoordinates(playerPosition);
        }

        /// <summary>
        /// Adjusts the position of the animation according to the position of the player.
        /// </summary>
        /// <param name="playerPosition">Current position of the player.</param>
        public override void UpdateCoordinates(Vector2 playerPosition)
        {
            Coo = new Vector2(playerPosition.X-10, playerPosition.Y -20);
        }

        public void Update() { base.Update(BattleScene.dt); }
    }

    #endregion Force Field Barrier
}