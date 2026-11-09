using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyvatar.src.Warriors
{
    public abstract class Player : Warrior
    {

        /// === Visual ===
        protected Dictionary<string, Texture2D> Images = new Dictionary<string, Texture2D>
        {
            {"idle", GameInterfaceManager.LoadImage(Path.Combine("Battle","Characters","BlackKnight","Idle.png"), scale:1.5f) },
            {"charging", GameInterfaceManager.LoadImage(Path.Combine("Battle","Characters","BlackKnight","Charging.png"), scale:1.5f) },
            {"block", GameInterfaceManager.LoadImage(Path.Combine("Battle","Characters","BlackKnight","Block.png"), scale:1.5f) },
            {"attack", GameInterfaceManager.LoadImage(Path.Combine("Battle","Characters","BlackKnight","Magic.png"), scale:1.5f) },
        };
        protected string State = "idle";
        protected Vector2 Coo;
        protected ChargingAnimation chargingAnimation;

        // === Stats ===
        public float MaxHp { get; protected set; } = 100f;
        public float Hp { get; protected set; }

        public float MaxEnergy { get; protected set; } = 100f;
        public float Energy { get; protected set; }

        public float MaxMana { get; protected set; } = 100f;
        public float Mana { get; protected set; } = 0f;

        public float MaxSkillPoints { get; protected set; } = 10f;
        public float SkillPoints { get; protected set; }

        // === Movement ===
        protected float MovementEnergyCost = 10f;
        protected float MovementCurrentCooldown = 0f;
        protected float MovementTotalCooldown = 1.0f;

        // === Energy ===
        protected float EnergyChargingSpeed = 15f;               // Energy per second
        protected float EnergyDepletionCooldown = 0f;
        protected float EnergyDepletionTotalCooldown = 2.0f;
        protected float BlockEnergyConsumptionSpeed = 20f;       // Energy per second while blocking
        protected float AttackEnergyChargingCooldownTime = 0f;
        protected float AttackEnergyChargingMaxTime = 1.5f;

        // === Mana ===
        protected float ManaChargingSpeed = 40f;                 // Mana per second while charging
        protected float ManaDischargingSpeed = 20f;              // Mana per second when not charging
        protected float ManaConservationInterval = 0f;

        // === Internal ===
        protected float Dt = 0f;

        // Action mapping (button name → method)
        protected Dictionary<string, System.Action> PlayerActions;

        public Player() : base(2, 2) // Starting grid position
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
        }

        // These will be properly implemented in a more concrete class (e.g. AttackPlayer)
        public abstract void AddHp(float value);
        public abstract void AddEnergy(float value);
        public abstract void AddMana(float value);

        public override void UpdatePixelCoordinates()
        {
            Coo = new Vector2(X*128, Y*-100 + 505);
        }

        protected virtual bool ManageMovement(List<string> pressedButtons)
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
            else if (X < PLAYER_FIELD_RANGE && pressedButtons.Contains("right"))
            {
                X++;
                hasMoved = true;
            }

            if (hasMoved)
            {
                MovementCurrentCooldown = MovementTotalCooldown;
                AddEnergy(-MovementEnergyCost);

                if (chargingAnimation != null)
                    chargingAnimation.Position = new Vector2(Coo.X - 130, Coo.Y - 140);
            }

            return hasMoved;
        }

        protected virtual void Charge()
        {
            State = "charging";

            if (Mana < MaxMana && chargingAnimation != null && chargingAnimation.CurrentFrame > 2)
            {
                AddMana(ManaChargingSpeed * Dt);

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
                State = "idle";
                EnergyDepletionCooldown += Dt;
                return;
            }

            State = "attack";
        }

        protected virtual void Block()
        {
            if (Energy > 0)
            {
                State = "block";
                AddEnergy(-BlockEnergyConsumptionSpeed * Dt);
                return;
            }

            State = "idle";
            EnergyDepletionCooldown += Dt;
        }

        public virtual void Act(List<string> pressedButtons)
        {
            // Movement only allowed in idle or attack state
            if (State == "idle" || State == "attack")
            {
                if (MovementCurrentCooldown <= 0 && Energy > 0)
                {
                    if (ManageMovement(pressedButtons))
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
            if (State != "special_attack")
            {
                State = "idle";
                chargingAnimation?.Reset();
            }
        }

        public virtual void Update(float dt)
        {
            MovementCurrentCooldown -= BattleScene.dt;
            EnergyDepletionCooldown -= BattleScene.dt;
            AttackEnergyChargingCooldownTime -= BattleScene.dt;

            if (State != "charging")
            {
                ManaConservationInterval -= BattleScene.dt;
                if (ManaConservationInterval <= 0)
                    AddMana(-ManaDischargingSpeed * BattleScene.dt);
            }

            // Energy regeneration
            if (State != "block")
            {
                if (State == "attack" && AttackEnergyChargingCooldownTime > 0)
                    return;

                AddEnergy(EnergyChargingSpeed * BattleScene.dt);
            }
        }

        public override void Draw(SpriteBatch screen)
        {
            if (Images.ContainsKey(State))
                screen.Draw(Images[State], Coo, Color.White);
            else
                throw new Exception($"Player has  invalid State: {State}");

            if (State == "charging" && chargingAnimation != null)
                chargingAnimation.Draw(screen);
        }
    }

    public class ChargingAnimation
    {
        private List<Texture2D> _frames = new();
        private float _currentFrame = 0f;
        private bool _firstLoop = true;

        public Vector2 Position { get; set; }
        public int CurrentFrame { get => (int)_currentFrame; }

        public ChargingAnimation(GraphicsDevice graphicsDevice, string folder, int frameCount, Vector2 position, Point? size = null)
        {
            Position = position;

            // Load frames: 0.png → (frameCount-2).png
            for (int i = 0; i < frameCount - 1; i++)
            {
                string path = Path.Combine(folder, $"{i}.png");
                Texture2D texture = Texture2D.FromFile(graphicsDevice, path);

                // Optional resizing (if you need it later)
                // For now we just load the original size
                _frames.Add(texture);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_frames.Count == 0) return;

            // Avoid displaying the empty frames (0 and 1) after the first loop
            if (!_firstLoop && _currentFrame < 2f)
                _currentFrame = 2f;

            // After frame 2 we consider the animation past the first loop
            if (_currentFrame > 2f)
                _firstLoop = false;
            

            int frameIndex = (int)_currentFrame;
            if (frameIndex >= _frames.Count)
                frameIndex = _frames.Count - 1;

            spriteBatch.Draw(_frames[frameIndex], Position, Color.White);

            // Advance animation
            _currentFrame = (_currentFrame + 0.2f) % _frames.Count;
        }

        public void Reset()
        {
            _currentFrame = 0f;
            _firstLoop = true;
        }
    }

}
