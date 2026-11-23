using Cyvatar.src.Warriors;
using Cyvatar.src.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyvatar.src.GameScenes
{
    public class BattleScene : GameScene
    {
        // Assets
        private Texture2D _background;
        private Texture2D _frontBase;

        public static readonly SpriteFont HpFont = GraphicManager.LoadFont("HPFont");

        // Game objects
        // private BlasterPlayer _player;
        private BasePlayer _player;

        public static List<Enemy> Enemies = new List<Enemy>();
        public static List<Damager> Damagers = new List<Damager>();

        // Transition control
        private GameSceneLink _nextScene;

        
        // private readonly ContentManager _content;   // Optional if you use Content Pipeline

        public DualSenseController Controller { get; set; } = new DualSenseController();

        public BattleScene(GraphicsDevice graphicsDevice) : base(graphicsDevice, GameSceneLink.LBattleScene) { }

        public override void LoadContent()
        {
            // === Load images (runtime loading style) ===
            _background = GameSceneManager.LoadImage(Path.Combine("Battle", "Fields", "GrassField", "Field.png"));
            _frontBase = GameSceneManager.LoadImage(Path.Combine("Battle", "Fields", "GrassField", "Base.png"));

            // Create player
            _player = new BlasterPlayer();

            Enemies.Add(new SpearGoblin());
            Enemies.Add(new ArcherGoblin());
            Enemies.Add(new ArcherGoblin());
        }

        public override void Update(GameTime gameTime)
        {
            // This Delta Time value will be used by all entities that this GameScene manages
            dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // Debug.WriteLine(dt);

            // --- Input ---
            var keyboard = Keyboard.GetState();

            // Example of quitting the battle
            if (keyboard.IsKeyDown(Keys.Escape))
            {
                _nextScene = GameSceneLink.LOpenWorld;          // Return to open world
                return;
            }

            // Player actions
            _player.Act(Controller.GetPressed());
            _player.Update();

            for (int i = 0; i < Enemies.Count; i++) { Enemies[i].Update(); }

            // Go through all Damagers on the Player's side (The ones that damage the Enemies)
            foreach (var damager in Damagers.Where(d => d.Type == OriginType.FriendlyFire && !d.ToBeDeleted))
            {
                damager.Update();

                // Collision Logic for Enemy-Damager
                foreach (var enemy in Enemies.Where(e => !e.IsDead ))
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
            Enemies = Enemies.Where(e => !e.IsDead).ToList();
        }

        public override void Draw()
        {
            // Background
            GraphicManager.Screen.Draw(_background, Vector2.Zero, Color.White);

            // Player
            _player.Draw();

            // Drawing the Enemies
            for (int i=0; i < Enemies.Count; i++) { Enemies[i].Draw(); }

            // Drawing the Damagers
            for (int i = 0; i < Damagers.Count; i++) { Damagers[i].Draw(); }

            // Front base (bottom bar)
            int screenHeight = _graphicsDevice.Viewport.Height;
            GraphicManager.Screen.Draw(_frontBase, new Vector2(0, screenHeight - _frontBase.Height), Color.White);
        }

        public override void SetNextScene()
        {
            base.SetNextScene();
            GameSceneLink next = _nextScene;
        }
    }
}
