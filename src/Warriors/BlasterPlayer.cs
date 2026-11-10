using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Cyvatar.src.Weapons.BlasterAttacks;

namespace Cyvatar.src.Warriors
{
    public class BlasterPlayer : AttackPlayer
    {
        public BlasterPlayer(): base(
            Path.Combine("Battle","Characters","BlackKnight","Shooting.png"),
            "Blaster",
            1,
            new string[] { BoltBullet.Name, ElectronBullet.Name, QuantumBullet.Name }
        )
        {
            UniquePropertiesInitialization();
        }

        // Additional initialization specific to BlasterPlayer
        protected override void UniquePropertiesInitialization()
        {
            UpdateWeaponCoo();
            AttackModes = new List<Action>{AttackMode1, AttackMode2, AttackMode3, AttackMode4};
        }

        // Updates weapon position
        protected override void UpdateWeaponCoo()
        {
            WeaponCoo = new Vector2(Coo.X + 135,Coo.Y + 55);
        }

        // Attack mode 1: Big Bolt of Electricity
        private void AttackMode1()
        {
            ShootingCurrentCooldownTime = BoltBullet.CooldownTime;
            Damagers.Add(new BoltBullet(Coo, WeaponLevel));
            AddEnergy(-BoltBullet.EnergyCost);
        }

        // Attack mode 2: Electron Bullet
        private void AttackMode2()
        {
            ShootingCurrentCooldownTime = ElectronBullet.CooldownTime;
            Damagers.Add(new ElectronBullet(Coo, WeaponLevel));
            AddEnergy(-ElectronBullet.EnergyCost);
        }

        // Attack mode 3: Quantum Bullet
        private void AttackMode3()
        {
            ShootingCurrentCooldownTime = QuantumBullet.CooldownTime;
            Damagers.Add(new QuantumBullet(Coo, WeaponLevel));
            AddEnergy(-QuantumBullet.EnergyCost);
        }

        // Attack mode 4: Not implemented yet
        private void AttackMode4()
        {
        }

        protected override void Attack()
        {
            base.Attack();

            // There is a pause between each attack
            if (ShootingCurrentCooldownTime <= 0 && Energy > 0)
            {
                AttackModes[CurrentAttackMode]();
                AttackEnergyChargingCooldownTime = AttackEnergyChargingMaxTime;

                // Remove bullets that have left the screen
                Damagers.RemoveAll(damager => damager.Coo.X >= GameInterfaceManager.ScreenWidth);
            }
        }

        public override void Update()
        {
            base.Update();

            // Update all active bullets
            foreach (var bullet in Damagers)
                bullet.Update();

            // Reduce shooting cooldown
            ShootingCurrentCooldownTime -= BattleScene.dt;
        }
    }
}
