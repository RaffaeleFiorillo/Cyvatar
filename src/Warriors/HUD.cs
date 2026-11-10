using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyvatar.src.Warriors
{
    public class HUD
    {
        private readonly AttackPlayer _player;
        private readonly Vector2 _attackModeIconCoo = new Vector2(25, 54);
        private readonly Vector2 _statsImageCoo = new Vector2(0, 0);
        private readonly Vector2 _manaBarrImageCoo = new Vector2(169, 51);
        private readonly Vector2 _taroksHudImageCoo = new Vector2(732, 0);

        #region Images

        private Texture2D _playerHudImage;
        private Texture2D _taroksHudImage;
        private Texture2D[] _manaBarImages = new Texture2D[18];
        private Dictionary<int, Texture2D> _attackModeImages = new();

        #endregion

        #region HP

        private Color _hpColor = Color.Lime;
        private const int HpBarMaxSize = 537;
        private Rectangle _hpRect = new Rectangle(169, 10, HpBarMaxSize, 25);
        private readonly Dictionary<string, Color> _hpColors = new()
        {
            { "max_life", new Color(0, 255, 70) },
            { "mid_life", new Color(230, 230, 100) },
            { "low",     new Color(255, 0, 0) }
        };

        #endregion

        #region Energy

        private Color _energyColor = Color.Cyan;
        private const int EnergyBarMaxSize = 494;
        private Rectangle _energyRect = new Rectangle(169, 40, EnergyBarMaxSize, 20);

        #endregion

        #region Mana

        private int _manaFrame = 0;

        #endregion


        public HUD(AttackPlayer player)
        {
            _player = player;

            // Load main HUD images
            _playerHudImage = GameInterfaceManager.LoadImage(Path.Combine("Battle", "HUD", "Player_HUD.png"));

            _taroksHudImage = GameInterfaceManager.LoadImage(Path.Combine("Battle", "HUD", "Taroks_HUD.png"));

            // Load mana bar frames (0 to 17)
            for (int i = 0; i < 18; i++)
                _manaBarImages[i] = GameInterfaceManager.LoadImage(Path.Combine("Battle", "HUD", "ManaBarr", $"{i}.png"), size: new Vector2(450, 150));

            // Load attack mode icons
            for (int i = 0; i < player.AttackModesNames.Length; i++)
            {
                string modeName = player.AttackModesNames[i];
                string path = Path.Combine("Battle", "Weapons", player.WeaponName, modeName, "0.png");

                _attackModeImages[i] = GameInterfaceManager.LoadImage(path, scale: 4.2f);
            }
        }

        public void UpdateHp()
        {
            float percentage = _player.Hp / _player.MaxHp;

            if (percentage > 0.75f)
                _hpColor = _hpColors["max_life"];
            else if (percentage > 0.33f)
                _hpColor = _hpColors["mid_life"];
            else
                _hpColor = _hpColors["low"];

            _hpRect.Width = (int)(percentage * HpBarMaxSize);
        }

        public void UpdateEnergy()
        {
            float percentage = _player.Energy / _player.MaxEnergy;

            // Simple lerp between blue and cyan
            _energyColor = Color.Lerp(Color.Blue, Color.Cyan, percentage);
            _energyRect.Width = (int)(percentage * EnergyBarMaxSize);
        }

        public void UpdateMana()
        {
            float percentage = _player.Mana / _player.MaxMana;
            _manaFrame = (int)(percentage * 17);
            _manaFrame = MathHelper.Clamp(_manaFrame, 0, 17);
        }

        public void Draw(SpriteBatch screen)
        {
            // Draw bars first (behind the HUD frame)
            GraphicManager.DrawRectangle(_hpRect, _hpColor);
            GraphicManager.DrawRectangle(_energyRect, _energyColor);

            // Main HUD frame
            screen.Draw(_playerHudImage, _statsImageCoo, Color.White);

            // Current attack mode icon
            screen.Draw(_attackModeImages[_player.CurrentAttackMode], _attackModeIconCoo, Color.White);


            // Mana bar (frame based)
            screen.Draw(_manaBarImages[_manaFrame], _manaBarrImageCoo, Color.White);

            // Taroks HUD
            screen.Draw(_taroksHudImage, _taroksHudImageCoo, Color.White);
        }
    }
}
