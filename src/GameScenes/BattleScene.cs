using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Cyvatar.src.Warriors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Cyvatar.src.GameScenes
{
    public class BattleScene : GameScene
    {
        // Assets
        private Texture2D _background;
        private Texture2D _frontBase;

        // Game objects
        // private BlasterPlayer _player;
        private BasePlayer _player;

        private List<Warrior> Enemies = new List<Warrior>();

        // Transition control
        private GameSceneLink _nextScene;

        
        // private readonly ContentManager _content;   // Optional if you use Content Pipeline

        public DualSenseController Controller { get; set; } = new DualSenseController();

        public static float dt = 0;

        public BattleScene(GraphicsDevice graphicsDevice) : base(graphicsDevice, GameSceneLink.LBattleScene) { }

        public override void LoadContent()
        {
            // === Load images (runtime loading style) ===
            _background = GameSceneManager.LoadImage(Path.Combine("Battle", "Fields", "GrassField", "Field.png"));
            _frontBase = GameSceneManager.LoadImage(Path.Combine("Battle", "Fields", "GrassField", "Base.png"));

            // Create player
            _player = new BlasterPlayer();

            Enemies.Add(new SpearGoblin());
        }

        public override void Update(GameTime gameTime)
        {
            // This Delta Time value will be used by all entities that this GameScene manages
            dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

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
        }

        public override void Draw()
        {
            // Background
            GraphicManager.Screen.Draw(_background, Vector2.Zero, Color.White);

            // Player
            _player.Draw();

            // Drawing the Enemies
            for (int i=0; i < Enemies.Count; i++) { Enemies[i].Draw(); }

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
