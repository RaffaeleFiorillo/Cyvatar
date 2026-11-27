using Cyvatar.src.Battle.Effects;
using Cyvatar.src.Battle.Warriors;
using Cyvatar.src.Battle.Warriors.Player;
using Cyvatar.src.Battle.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyvatar.src.Battle
{
    public class BattleField
    {
        #region Properties

        public int ColumnCount = 10;
        public int RowCount = 4;
        public static List<List<FieldCell>> Grid = new List<List<FieldCell>>();

        // Game Actors
        private BasePlayer _player;
        public static List<Enemy> Enemies = new List<Enemy>();
        public static List<Damager> Damagers = new List<Damager>();
        public static List<FieldCellEffect> FieldEffects = new List<FieldCellEffect>();

        // Assets
        // Assets
        private Texture2D _background;
        private Texture2D _frontBase;

        #endregion Properties

        public BattleField()
        {
            for (int x = 0; x < ColumnCount; x++)
            {
                List<FieldCell> column = new List<FieldCell>();

                for (int y = 0; y < RowCount; y++)
                    column.Add(new FieldCell());

                Grid.Add(column);
            }
        }

        public void PrintGrid()
        {
            for (int c = 3; c >= 0; c--)
            {
                string line = "";
                for (int r = 0; r < 10; r++)
                    line += Grid[r][c].warrior != null ? "1" : "0";

                Debug.WriteLine(line);
            }
            Debug.WriteLine("\n");
        }

        public void LoadContent()
        {
            // === Load images (runtime loading style) ===
            _background = GameSceneManager.LoadImage(Path.Combine("Battle", "Fields", "GrassField", "Field.png"));
            _frontBase = GameSceneManager.LoadImage(Path.Combine("Battle", "Fields", "GrassField", "Base.png"));

            // Create player
            _player = new BlasterPlayer();
            Grid[_player.X][_player.Y].warrior = _player;

            // Field.PrintGrid();

            Enemies.Add(new SpearGoblin());
            Enemies.Add(new ArcherGoblin());
            Enemies.Add(new ArcherGoblin());
        }

        public void Update(float dt, List<string> pressedButtons)
        {
            // Player actions
            _player.Act(pressedButtons);
            _player.Update();

            foreach (var enemy in Enemies) { enemy.Update(); }
            foreach (var effect in FieldEffects) { effect.Update(); }

            // Go through all Damagers on the Player's side (The ones that damage the Enemies)
            foreach (var damager in Damagers.Where(d => d.Type == OriginType.FriendlyFire && !d.ToBeDeleted))
            {
                damager.Update();

                // Collision Logic for Enemy-Damager
                foreach (var enemy in Enemies.Where(e => !e.IsDead))
                {
                    if (enemy.CollidesWith(damager.GetHitbox()))
                    {
                        enemy.TakeDamageFrom(damager);
                        damager.ApplyHitEffect();
                    }
                }
            }

            // Go through all Damagers on the Enemies' side (The ones that damage the Player)
            foreach (var damager in Damagers.Where(d => d.Type == OriginType.EnemyFire && !d.ToBeDeleted))
            {
                // Updating the damager
                damager.Update();

                // Collision Logic for Enemy-Damager
                if (_player.CollidesWith(damager.GetHitbox()))
                {
                    _player.TakeDamageFrom(damager);
                    damager.ApplyHitEffect();
                }
            }

            // Remove all "dead" damagers and Enemies
            Damagers = Damagers.Where(d => !d.ToBeDeleted).ToList();
            FieldEffects = FieldEffects.Where(e => !e.ToBeDeleted).ToList();
            Enemies = Enemies.Where(e => !e.IsDead).ToList();
        }

        public void Draw()
        {
            // Background
            GraphicManager.Screen.Draw(_background, Vector2.Zero, Color.White);

            // Player
            _player.Draw();

            // Drawing the Enemies
            for (int i = 0; i < Enemies.Count; i++) { Enemies[i].Draw(); }

            // Drawing the Damagers
            for (int i = 0; i < Damagers.Count; i++) { Damagers[i].Draw(); }

            // Front Base
            GraphicManager.Screen.Draw(_frontBase, new Vector2(0, GameSceneManager.ScreenHeight - _frontBase.Height), Color.White);
        }
    }


    public enum FieldSide
    {
        /// <summary>
        /// The FieldCell with this FieldSide informs that only the player (and allies) can step on it when unoccupied.
        /// </summary>
        Player,

        /// <summary>
        /// The FieldCell with this FieldSide informs that only the enemies can step on it when unoccupied.
        /// </summary>
        Enemy
    }

    public class FieldCell
    {
        /// <summary>
        /// Informs which kind of warriors can step on it when unoccupied.
        /// </summary>
        public FieldSide Side;

        /// <summary>
        /// The instance of Warrior that is placed on this Cell of the Grid.
        /// </summary>
        public Warrior warrior;

        /// <summary>
        /// The Cell can have a special Effect in case some Warrior steps on it.
        /// </summary>
        public FieldCellEffect Effect;

        /// <summary>
        /// This Flag informs if a Warrior can move on this Cell (true) or not (false).
        /// </summary>
        public bool IsUnoccupied { get => warrior != null; }
    }
}
