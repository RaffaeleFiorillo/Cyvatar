using Cyvatar.src.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Cyvatar.src.Warriors
{
    public abstract class AttackPlayer : Player
    {
        #region Weapon

        public string WeaponName;
        public int WeaponLevel;
        protected Texture2D WeaponImage;
        protected Vector2 WeaponCoo = Vector2.Zero;
        protected bool DisplayWeapon = false;

        #endregion

        #region Attack

        public List<Action> AttackModes = new();
        public string[] AttackModesNames;
        public int AvailableAttackModes = 2;          // last available index
        public int CurrentAttackMode = 0;
        public bool AlreadySwitchedAttackMode = false;

        public List<Damager> Damagers = new List<Damager>();         // Replace 'object' later with a real Damager/Bullet base class

        public float ShootingTotalCooldown = 1.0f;
        public float ShootingCurrentCooldownTime = 0f;

        #endregion

        #region HUD

        protected HUD Hud;

        #endregion  

        public AttackPlayer(string attackPoseImagePath, string weaponName, int weaponLevel, string[] attackModesNames)
            : base()
        {
            WeaponName = weaponName;
            WeaponLevel = weaponLevel;
            AttackModesNames = attackModesNames;

            // Load attack pose (overrides the default "attack" image)
            // You should load this properly with your content loading system
            Images["attack"] = GameInterfaceManager.LoadImage(Path.Combine("Battle", "Characters", "BlackKnight", "Shooting.png"));

            // Load weapon image
            WeaponImage = GameInterfaceManager.LoadImage(Path.Combine("Battle", "Weapons", $"{weaponName}", $"{weaponLevel}.png"), scale: 0.6f);

            Hud = new HUD(this);
        }

        /// <summary>
        /// Must be called at the end of the constructor of any class that inherits from AttackPlayer.
        /// Used to initialize weapon_coo and attack_modes.
        /// </summary>
        protected abstract void UniquePropertiesInitialization();

        /// <summary>
        /// Every child must implement how the weapon position is updated when the player moves.
        /// </summary>
        protected abstract void UpdateWeaponCoo();

        public override void AddHp(float value)
        {
            Hp = MathHelper.Clamp(Hp + value, 0, MaxHp);
            Hud?.UpdateHp();
        }

        public override void AddEnergy(float value)
        {
            // Ignore positive energy gain while depletion cooldown is active
            if (EnergyDepletionCooldown > 0 && value > 0)
                return;

            float oldEnergy = Energy;
            Energy = MathHelper.Clamp(Energy + value, 0, MaxEnergy);

            // Start cooldown when energy reaches 0
            if (oldEnergy > 0 && Energy <= 0)
            {
                EnergyDepletionCooldown = EnergyDepletionTotalCooldown;
            }

            Hud?.UpdateEnergy();
        }

        public override void AddMana(float value)
        {
            Mana = MathHelper.Clamp(Mana + value, 0, MaxMana);
            Hud?.UpdateMana();
        }

        protected override bool ManageMovement(List<string> pressedButtons)
        {
            bool hasMoved = base.ManageMovement(pressedButtons);

            if (hasMoved)
                UpdateWeaponCoo();

            return hasMoved;
        }

        public override void Act(List<string> pressedButtons)
        {
            // Switch attack mode with L2
            if (pressedButtons.Contains("l2"))
            {
                // Only Switch to the next attack mode if the L2 button has been released
                if (!AlreadySwitchedAttackMode)
                {
                    CurrentAttackMode = (CurrentAttackMode + 1) % (AvailableAttackModes + 1);
                    AlreadySwitchedAttackMode = true;
                }
            } else
                AlreadySwitchedAttackMode = false;

            base.Act(pressedButtons);
        }

        public override void Draw()
        {
            // Draw HUD first
            Hud?.Draw(GraphicManager.Screen);

            // Draw player body
            base.Draw();

            // Draw weapon when attacking
            if (State == "attack" && WeaponImage != null)
                GraphicManager.Screen.Draw(WeaponImage, WeaponCoo, Color.White);

            // Draw all damagers (bullets, projectiles, etc.)
            foreach (var damager in Damagers)
                // Assuming damager has a Draw method (you will create a base class later)
                damager.Draw();
        }
    }
}
