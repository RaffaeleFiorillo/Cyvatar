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

namespace Cyvatar.src
{
    public class BattleScene : IGameScene
    {
        public string Link => "Battle";

        // Assets
        private Texture2D _background;
        private Texture2D _frontBase;

        // Game objects
        // private BlasterPlayer _player;
        private Player _player;

        // Transition control
        private string? _nextScene = null;

        private readonly GraphicsDevice _graphicsDevice;
        private readonly ContentManager _content;   // Optional if you use Content Pipeline
        public DualSenseController Controller { get; set; } = new DualSenseController();

        public static float dt = 0;

        public BattleScene(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public void LoadContent()
        {
            // === Load images (runtime loading style) ===
            _background = GameInterfaceManager.LoadImage(Path.Combine("Battle", "Fields", "GrassField", "Field.png"));
            _frontBase = GameInterfaceManager.LoadImage(Path.Combine("Battle", "Fields", "GrassField", "Base.png"));

            // Create player
            _player = new BlasterPlayer();
        }

        public void Update(GameTime gameTime)
        {
            // This Delta Time value will be used by all entities that this GameScene manages
            dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // --- Input ---
            var keyboard = Keyboard.GetState();

            // Example of quitting the battle
            if (keyboard.IsKeyDown(Keys.Escape))
            {
                _nextScene = "OpenWorld";          // Return to open world
                return;
            }

            // Player actions
            _player.Act(Controller.GetPressed());
            _player.Update();
        }

        public void Draw(SpriteBatch screen)
        {
            // Background
            GraphicManager.Screen.Draw(_background, Vector2.Zero, Color.White);

            // Player
            _player.Draw();

            // Front base (bottom bar)
            int screenHeight = _graphicsDevice.Viewport.Height;
            GraphicManager.Screen.Draw(_frontBase, new Vector2(0, screenHeight - _frontBase.Height), Color.White);
        }

        public string? GetNextScene()
        {
            string? next = _nextScene;
            _nextScene = null;     // Reset after reading
            return next;
        }
    }
}
